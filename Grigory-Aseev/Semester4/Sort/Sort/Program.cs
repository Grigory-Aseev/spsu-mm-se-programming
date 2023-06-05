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
                // read all numbers from the file into the int list
                var numbers = FileHandler.ReadFileToIntList(args[0], args[2]);

                if (numbers.Count != 0)
                {
                    // continue to work because a list of numbers is correctly obtained
                    comm.Scatter(new bool[size].Select(_ => true).ToArray(), 0);
                    data = numbers;
                }
                else
                {
                    // no numbers were received, we send a completion flag to all processors
                    comm.Scatter(new bool[size].Select(_ => false).ToArray(), 0);
                    Console.WriteLine("Error at the file reading stage");
                    return;
                }
            }
            else
            {
                // finish the job
                if (!comm.Scatter<bool>(0)) return;
            }

            // iterations of even-odd sorting

            for (var i = 0; i < size + 1; i++)
            {
                List<int> subList;
                if (rank == 0)
                {
                    // distribute pieces of the original list of numbers to all processors
                    var subLists = new List<int>[size];
                    var startIndex = 0;

                    for (var j = 0; j < size; j++)
                    {
                        // var countProcessors = 2;
                        // [3 4 5] [2 6]
                        var sublistCount = (data.Count / size) + (data.Count % size > j ? 1 : 0);
                        subLists[j] = data.GetRange(startIndex, sublistCount);
                        startIndex += sublistCount;
                    }

                    // get the list for the processor
                    subList = comm.Scatter(subLists.ToArray(), 0);
                }
                else
                    subList = comm.Scatter<List<int>>(0);

                subList.Sort();

                // find out the location of the processor in the current iteration

                switch ((rank + i) % 2)
                {
                    // left processor
                    // if not the last one, we can pass our list to the right processor and get a new sorted list
                    case 0 when rank + 1 != size:
                        comm.Send(subList, rank + 1, 0);

                        subList = comm.Receive<List<int>>(rank + 1, 0);
                        break;
                    // right processor
                    // check that there is a left processor
                    case 1 when rank != 0:
                    {
                        // compare and split
                        var prevList = comm.Receive<List<int>>(rank - 1, 0);
                        subList.AddRange(prevList);
                        subList.Sort();

                        comm.Send(subList.GetRange(0, subList.Count / 2), rank - 1, 0);
                        subList = subList.GetRange(subList.Count / 2, subList.Count - subList.Count / 2);
                        break;
                    }
                }
                // pair sorted lists

                if (rank == 0)
                    data = comm.Gather(subList, 0).SelectMany(newSubList => newSubList).ToList();
                else
                    comm.Gather(subList, 0);
            }

            if (rank != 0) return;
            // record the result
            FileHandler.WriteStringToFile(string.Join("\n", data), args[1]);
            Console.WriteLine("Sorting was successful");
        }
    }
}