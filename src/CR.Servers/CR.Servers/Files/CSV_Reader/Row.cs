using System;

namespace CR.Servers.Files.CSV_Reader
{
    public class Row
    {
        public Row(Table table, string name)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table));

            Name = name;
            Table = table;
            Table._rows.Add(this);
        }

        public int Start;
        public int End;

        public Table Table { get; }

        public string Name { get; }

        public string GetValue(string Name, int Level)
        {
            return this.Table.GetValue(Name, Level + this.Start);
        }
    }
}
