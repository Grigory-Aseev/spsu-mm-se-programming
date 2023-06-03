using System.Security.Cryptography;
using MPI;

namespace Sort
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            using var env = new MPI.Environment(ref args);
            if (args.Length != 3)
            {
                Console.WriteLine("Insufficient arguments");
                return;
            }

            var comm = Communicator.world;
            var rank = comm.Rank;
            var size = comm.Size;

            var data = new List<int>();

            if (rank == 0)
            {
                var numbers = FileHandler.ReadFileToIntList(args[0], args[2]);

                if (numbers.Count != 0)
                {
                    comm.Scatter(new bool[size].Select(_ => true).ToArray(), 0);
                    data = numbers;
                }
                else
                {
                    comm.Scatter(new bool[size].Select(_ => false).ToArray(), 0);
                    Console.WriteLine("Error at the file reading stage");
                    return;
                }
            }
            else
            {
                if (!comm.Scatter<bool>(0)) return;
            }

            for (var i = 0; i < size + 1; i++)
            {
                List<int> subList;
                if (rank == 0)
                {
                    var subLists = new List<int>[size];
                    var startIndex = 0;

                    for (var j = 0; j < size; j++)
                    {
                        var sublistCount = (data.Count / size) + (data.Count % size > j ? 1 : 0);
                        subLists[j] = data.GetRange(startIndex, sublistCount);
                        startIndex += sublistCount;
                    }

                    subList = comm.Scatter(subLists.ToArray(), 0);
                }
                else
                    subList = comm.Scatter<List<int>>(0);

                subList.Sort();


                switch ((rank + i) % 2)
                {
                    case 0 when rank + 1 != size:
                        comm.Send(subList, rank + 1, 0);

                        subList = comm.Receive<List<int>>(rank + 1, 0);
                        break;
                    case 1 when rank != 0:
                    {
                        var prevList = comm.Receive<List<int>>(rank - 1, 0);
                        subList.AddRange(prevList);
                        subList.Sort();

                        comm.Send(subList.GetRange(0, subList.Count / 2), rank - 1, 0);
                        subList = subList.GetRange(subList.Count / 2, subList.Count - subList.Count / 2);
                        break;
                    }
                }

                if (rank == 0)
                    data = comm.Gather(subList, 0).SelectMany(newSubList => newSubList).ToList();
                else
                    comm.Gather(subList, 0);
            }

            if (rank != 0) return;
            FileHandler.WriteStringToFile(string.Join("\n", data), args[1]);
            Console.WriteLine("Sorting was successful");
        }
    }
}