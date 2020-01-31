using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CR.Servers.Files.CSV_Reader
{
    [DebuggerDisplay("Name = {" + nameof(Name) + "}")]
    public class Column
    {
        public Column(Table table, string name)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table));

            Name = name;
            Table = table;
            Table._columns.Add(this);

            Data = new List<string>();
        }

        public readonly List<string> Data;

        public Table Table { get; }

        public string Name { get; }

        internal string Get(int _Row)
        {
            return this.Data.Count > _Row ? this.Data[_Row] : null;
        }

    }
}
