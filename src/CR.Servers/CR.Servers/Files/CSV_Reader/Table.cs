using System;
using System.Collections.Generic;
using System.IO;

namespace CR.Servers.Files.CSV_Reader
{
    public class Table
    {
        public Table(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (!File.Exists(path))
                throw new FileNotFoundException($"File at '{Path.GetFullPath(path)}' does not exists.");

            _rows = new RowCollection();
            _columns = new ColumnCollection();
            this.Headers = new List<string>();
            using (var file = new FileStream(path, FileMode.Open))
                ParseTable(file);
        }

        internal readonly RowCollection _rows;
        internal readonly ColumnCollection _columns;

        internal List<string> Headers;

        public RowCollection Rows => _rows;
        public ColumnCollection Columns => _columns;

        public Row GetRowAt(int _Index)
        {
            return this.Rows[_Index];
        }

        public int GetRowCount()
        {
            return this.Rows.Count;
        }

        private void ParseTable(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                var namesRow = reader.ReadLine();
                if (namesRow == null)
                    throw new CsvException("CSV table does not contain a row representing the column names.");

                var typesRow = reader.ReadLine();
                if (typesRow == null)
                    throw new CsvException("CSV table does not contain a row representing the column types.");

                var names = ParseLine(namesRow);

                var types = ParseLine(typesRow);
                var tableWidth = names.Length;

                if (types.Length != tableWidth)
                    throw new CsvException("CSV table has inconsistent table width.");

                for (int i = 0; i < types.Length; i++)
                {
                    new Column(this, names[i]);
                    this.Headers.Add(names[i]);
                }

                var rowCount = 0;
                var prev = default(Row);
                var row = default(string);
                while ((row = reader.ReadLine()) != null)
                {
                    // Column values.
                    var values = ParseLine(row);

                    // Figure out if the current CSV line has correct table width.
                    if (values.Length != tableWidth)
                        throw new CsvException($"CSV table has inconsistent table width at row {rowCount}.");

                    var rowName = values[0];
                    // Figure out if the CSV row has no names,
                    // If the row have a name, we add it to our list of rows.
                    if (rowName != string.Empty)
                    {
                        if (prev != null)
                            prev.End = rowCount;

                        prev = new Row(this, rowName)
                        {
                            Start = rowCount
                        };
                    }

                    // Add the column values into their respective columns.
                    for (int i = 0; i < values.Length; i++)
                        _columns[i].Data.Add(values[i]);

                    rowCount++;
                }

                // Check for last rows.
                if (prev != null)
                    prev.End = rowCount;
            }
        }

        private static string[] ParseLine(string line)
        {
            var token = string.Empty;
            var columns = new List<string>(16);

            var inCommas = false;
            foreach (var c in line)
            {
                // Figure out if we're in between inverted commas.
                if (c == '"')
                {
                    inCommas = !inCommas;
                }
                else if (c == ',' && !inCommas)
                {
                    columns.Add(token);
                    token = string.Empty;
                }
                else
                {
                    token += c;
                }
            }
            columns.Add(token);

            return columns.ToArray();
        }
        public string GetValue(string _Name, int _Level)
        {
            int _Index = this.Headers.IndexOf(_Name);
            return this.GetValueAt(_Index, _Level);
        }

        public string GetValueAt(int _Column, int _Row)
        {
            if (_Column > -1 && _Row > -1)
            {
                return this.Columns[_Column].Get(_Row);
            }

            return null;
        }
    }
}

