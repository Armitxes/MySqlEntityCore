using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace MySqlEntityCore {
	class Connection {
		public static Connection Default { get; private set; }

		public string Host { get; private set; }
		public string User { get; private set; }
        public string Port { get; private set; }
		private string Password { get; set; }

		private static List<string> DBs { get; set; }
		private static List<string> TBLs { get; set; }

		private MySqlConnection PrivateStream { get; set; }
		internal MySqlConnection Stream
		{
			get
			{
				if (PrivateStream == null)
					PrivateStream = new MySqlConnection($"server={Host};port={Port};uid={User};pwd={Password}");
				Open();
				return PrivateStream;
			}
			private set
			{
				PrivateStream = value;
			}
		}

		public Connection(
			string host="127.0.0.1",
            string port="3306",
			string user="root",
			string password="",
			bool isDefault=true
		) {
			Host = host;
            Port = port;
			User = user;
			Password = password;
			if (isDefault)
				Default = this;
		}

		private void Open()
		{
			if (PrivateStream.State != ConnectionState.Open)
				PrivateStream.Open();
		}

		private void Close()
		{
			if (Stream == null || Stream.State == ConnectionState.Closed)
				return;
			Stream.Close();
		}

		///<summary>Get a list of available databases.</summary>
		public static List<string> Databases()
		{
			if (DBs == null)
			{
				DBs = new List<string>();
				MySqlCommand cmd = Default.Stream.CreateCommand();
				cmd.CommandText = "show databases;";

				MySqlDataReader rdr = cmd.ExecuteReader();
				while (rdr.Read())
					DBs.Add(rdr.GetString(0));
				rdr.Close();
			}
			return DBs;
		}

		///<summary>Get a list of available tables for the active database.</summary>
		public static List<string> Tables()
		{
			if (TBLs == null)
			{
				TBLs = new List<string>();
				MySqlCommand cmd = Default.Stream.CreateCommand();
				cmd.CommandText = "show tables;";

				MySqlDataReader rdr = cmd.ExecuteReader();
				while (rdr.Read())
					DBs.Add(rdr.GetString(0));
				rdr.Close();
			}
			return TBLs;
		}

		public void ChangeDatabase(string name)
		{
			name = name.ToLower();
			if (!Databases().Contains(name))
				new SqlQuery($"CREATE DATABASE {name} CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;");
			Default.Stream.ChangeDatabase(name);
		}

	}
}
