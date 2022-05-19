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
                dynamic autoIncrement = row.GetValueOrDefault("AUTO_INCREMENT", 0);
                if (System.DBNull.Value.Equals(autoIncrement))
                    autoIncrement = 0;

                result.Add(new TableInfo()
                {
                    Name = row.GetValueOrDefault("TABLE_NAME", "-"),
                    Rows = (uint)row.GetValueOrDefault("TABLE_ROWS", 0),
                    AvgRowLength = (uint)row.GetValueOrDefault("AVG_ROW_LENGTH", 0),
                    DataLength = (uint)row.GetValueOrDefault("DATA_LENGTH", 0),
                    MaxDataLength = (uint)row.GetValueOrDefault("MAX_DATA_LENGTH", 0),
                    IndexLength = (uint)row.GetValueOrDefault("INDEX_LENGTH", 0),
                    AutoIncrement = (uint)autoIncrement,
                    Encoding = row.GetValueOrDefault("TABLE_COLLATION", ""),
                });
            }
            Cache.Set("DBTableInfo", result, 300);
            return result;
        }
    }

}
