namespace CR.Servers.CoC.Files.CSV_Helpers
{
    using System;
    using System.Reflection;
    using CR.Servers.Files.CSV_Data;
    using CR.Servers.Files.CSV_Reader;

    internal class Data : IData
    {
        internal DataTable DataTable;
        internal Row Row;

        internal Data()
        {
            // Data.
        }

        internal Data(Row Row, DataTable DataTable)
        {
            this.Row = Row;
            this.DataTable = DataTable;
            this.Type = DataTable.Index;
            this.InstanceId = DataTable.Datas.Count;
            this.GlobalId = CSV_Helpers.GlobalId.Create(this.Type, this.InstanceId);

            this.Load(Row);
        }

        public int InstanceId { get; set; }

        public int Type { get; set; }

        public virtual string Name { get; set; }

        public int GlobalId { get; set; }

        public void Load(Row row)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            Type type = this.GetType();
            PropertyInfo[] properties = type.GetProperties();
            Table table = row.Table;

            foreach (PropertyInfo property in properties)
            {
                if (property.DeclaringType == typeof(Data))
                {
                    continue;
                }

                Column column = table.Columns[property.Name];
                Type propertyType = property.PropertyType;

                if (propertyType.IsArray)
                {
                    // Calculate how many upgrade levels the logic data has.
                    int lvls = row.End - row.Start;
                    // Base type of the array.
                    Type arrayType = propertyType.GetElementType();
                    // Array instance we're going to set the property value to.
                    Array array = Array.CreateInstance(arrayType, lvls);

                    string prevStrValue = null;
                    for (int j = 0; j < lvls; j++)
                    {
                        string strValue = column.Data[row.Start + j];

                        // If the data value is empty, we check if we
                        // have a non-empty previous data value to use.
                        if (strValue == string.Empty)
                        {
                            if (prevStrValue != null)
                            {
                                // No need to change the type of the value since its
                                // already a string and the property type is string.
                                if (propertyType == typeof(string))
                                {
                                    array.SetValue(prevStrValue, j);
                                }
                                else
                                {
                                    object value = Convert.ChangeType(prevStrValue, arrayType);
                                    array.SetValue(value, j);
                                }
                            }
                        }
                        // Else if the data is not empty, we use it directly.
                        else
                        {
                            // Mark the current value as the previous value or parent value.
                            prevStrValue = strValue;

                            // Update the current value to the one in the column data.
                            object value = Convert.ChangeType(strValue, arrayType);
                            array.SetValue(value, j);
                        }
                    }

                    property.SetValue(this, array);
                }
                else
                {
                    // Take the first value of the data in column from the row.
                    string strValue = column.Data[row.Start];
                    if (strValue != string.Empty)
                    {
                        // No need to change the type of the value since its
                        // already a string and the property type is string.
                        if (propertyType == typeof(string))
                        {
                            property.SetValue(this, strValue);
                        }
                        else
                        {
                            object value = Convert.ChangeType(strValue, propertyType);
                            property.SetValue(this, value);
                        }
                    }
                }
            }
        }

        internal int GetId()
        {
            return CSV_Helpers.GlobalId.GetID(this.GlobalId);
        }

        internal int GetDataType()
        {
            return this.DataTable.Index;
        }

        internal virtual void Process()
        {
        }
    }

    /*internal class DataConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Data Data = (Data) value;

            writer.WriteValue(Data?.GlobalId ?? 0);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            int Id = (int) (long) reader.Value;

            if (Id != 0)
            {
                Data Data = CSV.Tables.GetWithGlobalId(Id);

                if (objectType == typeof(Data) || Data.GetType() == objectType)
                {
                    return Data;
                }
#if DEBUG
                //Logging.Error(this.GetType(), "Data is not equals with objectType. Data:" + Data.GetType() + " objectType:" + objectType + ".");
#endif
            }

            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.BaseType == typeof(Data) || objectType == typeof(Data);
        }
    }*/
}