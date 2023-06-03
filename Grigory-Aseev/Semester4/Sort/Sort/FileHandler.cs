namespace Sort
{
    public static class FileHandler
    {
        public static List<int> ReadFileToIntList(string path, string sep)
        {
            var data = "";

            try
            {
                data = File.ReadAllText(path);
            }
            catch
            {
                Console.WriteLine($"Failed to read the file in the path {path}");
                return new List<int>();
            }

            var separatedData = data.Split(sep).ToList();


            try
            {
                var numbers = separatedData
                    .Select(s => int.TryParse(s, out var n) ? n : throw new Exception("Invalid data format")).ToList();
                return numbers;
            }
            catch
            {
                Console.WriteLine("The input data contain non-numbers");
                return new List<int>();
            }
        }

        public static void WriteStringToFile(string data, string path)
        {
            try
            {
                File.WriteAllText(path, data);
            }
            catch
            {
                Console.WriteLine($"Failed to write data to the file in the path {path}");
            }
        }
    }
}