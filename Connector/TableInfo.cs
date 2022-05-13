using System.Collections.Generic;

namespace MySqlEntityCore
{
    public class TableInfo
    {
        ///<summary>Table name in database</summary>
        public string Name { get; internal set; }

        ///<summary>Total count of model entries in database.</summary>
        public uint Rows { get; internal set; }

        ///<summary>Average row length in database.</summary>
        public uint AvgRowLength { get; internal set; }

        ///<summary>Database table size (in bytes).</summary>
        public uint DataLength { get; internal set; }

        ///<summary>Maximum database table size (in bytes). No limit if 0.</summary>
        public uint MaxDataLength { get; internal set; }

        ///<summary>Index length.</summary>
        public uint IndexLength { get; internal set; }

        ///<summary>Current auto increment value.</summary>
        public uint AutoIncrement { get; internal set; }

        ///<summary>Table encoding/collation.</summary>
        public string Encoding { get; internal set; }

        internal static List<TableInfo> Get(string dbName)
        {
            dynamic cached = Cache.Get("DBTableInfo");
            if (cached != null)
                return cached;

            List<TableInfo> result = new List<TableInfo>();
            List<Dictionary<string, object>> rows = new Connection().Query(
                $"SELECT `table_name`, `table_rows`, `avg_row_length`, `data_length`, `max_data_length`, `index_length`, `auto_increment`, `table_collation` FROM information_schema.TABLES WHERE TABLES.TABLE_SCHEMA = '{dbName}';"
            );
            foreach (Dictionary<string, dynamic> row in rows)
            {
                result.Add(
                    new TableInfo()
                    {
                        Name = row.GetValueOrDefault("table_name", "-"),
                        Rows = row.GetValueOrDefault("table_rows", 0),
                        AvgRowLength = row.GetValueOrDefault("avg_row_length", 0),
                        DataLength = row.GetValueOrDefault("data_length", 0),
                        MaxDataLength = row.GetValueOrDefault("max_data_length", 0),
                        IndexLength = row.GetValueOrDefault("index_length", 0),
                        AutoIncrement = row.GetValueOrDefault("auto_increment", 0),
                        Encoding = row.GetValueOrDefault("table_collation", ""),
                    }
                );
            }
            Cache.Set("DBTableInfo", result, 300);
            return result;
        }
    }

}
