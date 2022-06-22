using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace MySqlEntityCore
{
    public class Transaction
    {
        public MySqlConnection DbConnection { get; private set; }
        public MySqlTransaction DbTransaction { get; private set; }

        public Transaction()
        {
            DbConnection = Connection.GetMySqlConnection();
            DbConnection.Open();
            DbTransaction = DbConnection.BeginTransaction();
        }

        ///<summary>Execute query with resultset.</summary>
        ///<returns>List of matching rows.</returns>
        public List<Dictionary<string, object>> Query(string query)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            MySqlCommand cmd = DbConnection.CreateCommand();
            cmd.Connection = DbConnection;
            cmd.Transaction = DbTransaction;
            cmd.EnableCaching = false;
            cmd.CommandText = query;

            MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                Dictionary<string, object> row = new Dictionary<string, object>();
                for (int i = 0; i < rdr.FieldCount; i++) { row.Add(rdr.GetName(i), rdr.GetValue(i)); }
                result.Add(row);
            }
            rdr.Close();
            cmd.Dispose();
            return result;
        }

        ///<summary>Execute uncached query without resultset.</summary>
        public void NonQuery(string query)
        {
            MySqlCommand cmd = DbConnection.CreateCommand();
            cmd.Connection = DbConnection;
            cmd.Transaction = DbTransaction;
            cmd.EnableCaching = false;
            cmd.CommandText = query;
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        ///<summary>Commit the transaction to database.</summary>
        ///<exception>MySqlException on commit failure.</exception>
        public void Commit()
        {
            DbTransaction.Commit();
            DbTransaction.Dispose();
            DbTransaction = DbConnection.BeginTransaction();
        }

        ///<summary>Rollback the transaction.</summary>
        ///<exception>MySqlException on rollback failure.</exception>
        public void Rollback()
        {
            DbTransaction.Rollback();
            DbTransaction.Dispose();
            DbTransaction = DbConnection.BeginTransaction();
        }

        ///<summary>Commit the transaction to database. On failure try to rollback.</summary>
        ///<returns>true if commited, false if rolled back.</returns>
        public bool CommitOrRollback()
        {
            bool success = true;
            try
            {
                DbTransaction.Commit();
            }
            catch
            {
                DbTransaction.Rollback();
                success = false;
            }
            DbTransaction.Dispose();
            DbTransaction = DbConnection.BeginTransaction();
            return success;
        }
    }
}
