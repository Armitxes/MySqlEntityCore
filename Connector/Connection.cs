using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace MySqlEntityCore
{
    public class Connection
    {
        public static ConnectionData DefaultPool { get; private set; }

        private static List<string> DBs { get; set; }
        private static List<string> TBLs { get; set; }

        ///<summary>Establish a database connection with the given parameters.</summary>
        public static void SetDefaultPoolingConnection(
            string host = "127.0.0.1",
            string port = "3306",
            string user = "",
            string password = "",
            string database = "mysqlentitycore"
        )
        {
            if (DefaultPool == null)
                DefaultPool = new ConnectionData();
            DefaultPool.Host = host;
            DefaultPool.Port = port;
            DefaultPool.User = user;
            DefaultPool.Password = password;
            DefaultPool.Database = database;
            new Connection().ChangeDatabase(database);  // Ensure that DB exists
        }

        public static List<TableInfo> InformationSchemaTables() => (DefaultPool == null) ? null : TableInfo.Get(DefaultPool.Database);

        ///<summary>Get available connection from connection pool.</summary>
        public Connection()
        {
            if (DefaultPool == null)
                throw new MissingConnectionData();
        }


        ///<summary>Establish a database connection with the given parameters.</summary>
        public Connection(
            string host = "127.0.0.1",
            string port = "3306",
            string user = "",
            string password = "",
            string database = "mysqlentitycore"
        )
        {
            if (DefaultPool == null)
                Connection.SetDefaultPoolingConnection(
                    host: host,
                    port: port,
                    user: user,
                    password: password,
                    database: database
                );
        }

        ///<summary>Execute uncached query without resultset.</summary>
        public void NonQuery(string query)
        {
            using (MySqlConnection connection = new MySqlConnection(DefaultPool.GetConnectionString()))
            {
                using (MySqlCommand cmd = connection.CreateCommand())
                {
                    cmd.EnableCaching = false;
                    cmd.CommandText = query;
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        ///<summary>Execute query with resultset.</summary>
        ///<returns>List of matching rows.</returns>
        public List<Dictionary<string, object>> Query(string query)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            using (MySqlConnection connection = new MySqlConnection(DefaultPool.GetConnectionString()))
            {
                using (MySqlCommand cmd = connection.CreateCommand())
                {
                    cmd.EnableCaching = false;
                    cmd.CommandText = query;

                    connection.Open();
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        Dictionary<string, object> row = new Dictionary<string, object>();
                        for (int i = 0; i < rdr.FieldCount; i++) { row.Add(rdr.GetName(i), rdr.GetValue(i)); }
                        result.Add(row);
                    }
                    rdr.Close();
                }
            }
            return result;
        }

        ///<summary>Get a list of available databases.</summary>
        public static List<string> Databases()
        {
            if (DBs != null)
                return DBs;

            DBs = new List<string>();
            using (MySqlConnection connection = new MySqlConnection(DefaultPool.GeDbLessConnectionString()))
            {
                using (MySqlCommand cmd = connection.CreateCommand())
                {
                    connection.Open();
                    cmd.CommandText = "show databases;";

                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                        DBs.Add(rdr.GetString(0));
                    rdr.Close();
                }
            }
            return DBs;
        }

        ///<summary>Get a list of available tables for the active database.</summary>
        public static List<string> Tables()
        {
            if (TBLs != null)
                return TBLs;
            TBLs = new List<string>();
            using (MySqlConnection connection = new MySqlConnection(DefaultPool.GetConnectionString()))
            {
                using (MySqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "show tables;";
                    connection.Open();
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                        TBLs.Add(rdr.GetString(0));
                    rdr.Close();
                }
            }
            return TBLs;
        }

        ///<summary>Switch to the given database. Attempt to create DB if not found.</summary>
        public void ChangeDatabase(string name)
        {
            name = name.ToLower();
            using (MySqlConnection connection = new MySqlConnection(DefaultPool.GeDbLessConnectionString()))
            {
                connection.Open();
                if (!Databases().Contains(name))
                {
                    using (MySqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.EnableCaching = false;
                        cmd.CommandText = $"CREATE DATABASE {name} CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;";
                        cmd.ExecuteNonQuery();
                    }
                }
                connection.ChangeDatabase(name);
            }
        }

#if DEBUG
        public void RecreateDatabase()
        {
            string dbName = Connection.DefaultPool.Database;
            NonQuery($"DROP DATABASE IF EXISTS {dbName};");
            DBs = null;
            TBLs = null;
            ChangeDatabase(dbName);
            Cache.Clear();
        }
#endif

    }
}
