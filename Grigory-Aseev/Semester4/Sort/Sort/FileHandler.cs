namespace Sort
{
    public static class FileHandler
    {
        public static List<int> ReadFileToIntList(string path, string sep)
        {
            var data = "";

            try
            {
                // read file
                data = File.ReadAllText(path);
            }
            catch
            {
                Console.WriteLine($"Failed to read the file in the path {path}");
                return new List<int>();
            }

            // separate the data with the separator

            var separatedData = data.Split(sep).ToList();


            try
            {
                // parse integers
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