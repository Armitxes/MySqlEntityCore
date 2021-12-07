using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace MySqlEntityCore {

	class SqlQuery {
		public List<Dictionary<string, object>> Result { get; set; } = new List<Dictionary<string, object>>();

		public SqlQuery(string query, bool readResult=false)
		{
			MySqlCommand cmd = Connection.Default.Stream.CreateCommand();
			cmd.EnableCaching = false;
			cmd.CommandText = query;
			if (readResult)
			{
				MySqlDataReader rdr = cmd.ExecuteReader();
				while (rdr.Read())
				{
					Dictionary<string, object> row = new Dictionary<string, object>();
					for (int i = 0; i < rdr.FieldCount; i++) { row.Add(rdr.GetName(i), rdr.GetValue(i)); }
					Result.Add(row);
				}
				rdr.Close();
				return;
			}
			cmd.ExecuteNonQuery();
		}
	}
}
