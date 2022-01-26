using System;

namespace MySqlEntityCore {
	public class ConnectionData {
        public string Host { get; set; }
		public string User { get; set; }
        public string Port { get; set; }
		internal string Password { get; set; }
        public string Database { get; set; }

        internal string GetConnectionString() {
            return $"server={Host};port={Port};uid={User};pwd={Password};database={Database};pooling=true;";
        }
        internal string GeDbLessConnectionString() {
            return $"server={Host};port={Port};uid={User};pwd={Password};pooling=false;";
        }
    }

    [Serializable]
    class MissingConnectionData : Exception
    {
        public MissingConnectionData()
            : base(
                "Missing Default Connection Data.\n" + 
                "Please call MySqlEntityCore.Connection.SetDefaultPoolingConnection() to define default connection parameters\n" +
                "Call the alternate MySqlEntityCore.Connection constructor for connections that differ from the default."
            )
        {

        }
    }
}
