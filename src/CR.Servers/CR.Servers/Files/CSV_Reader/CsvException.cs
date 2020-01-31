using System;

namespace CR.Servers.Files.CSV_Reader
{

    [Serializable]
    public class CsvException : Exception
    {
        public CsvException()
        {
            // Space
        }

        public CsvException(string message) : base(message)
        {
            // Space
        }
    }
}