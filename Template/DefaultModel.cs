using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace MySqlEntityCore.Template
{

    ///<summary>Create a model with an unsigned Id as primary key.</summary>
    public class DefaultModel : Template.Core
    {

        [Field(PrimaryKey = true, AutoIncrement = true)]
        public uint Id { get; internal set; }

        public DefaultModel() { }

        public new string CacheKey
        {
            get
            {
                if (_CacheKey != null)
                    return _CacheKey;

                var flags = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                _CacheKey = ChildType.Name;
                object val = ChildType.GetProperty("Id", flags).GetValue(this);
                if (val == null)
                    val = "0";
                _CacheKey += "." + val.ToString();
                return _CacheKey;
            }
        }

        ///<summary>Get the record with the corresponding id.</summary>
        ///<param name="id"></param>
        public DefaultModel(uint id)
        {
            this.Id = id;
            Fetch();
        }

        ///<summary>Fetch id related information from cache or database.</summary>
        public void Fetch()
        {
            // There is nothing to fetch
            if (this.Id == 0)
                return;

            // Try to build record from cache.
            dynamic record = Cache.Get(CacheKey);
            if (record != null)
                this.ConstructFromClass(record);
            else
            {
                // Get record from database.
                record = new Connection().Query(
                    $"SELECT {this.Instance.SqlFieldList} FROM {this.Instance.Table} WHERE id={this.Id};"
                ).FirstOrDefault();
                this.ConstructFromDictionary(record);
            }

            if (this.AttachedTransaction != null)
                return;

            this.StoreInCache();
            this.Origin = this.MemberwiseClone();
        }

        ///<summary>Get the record with the corresponding id.</summary>
        ///<param name="id"></param>
        ///<returns></returns>
        public static T Get<T>(uint id, Transaction transaction = null) where T : DefaultModel
        {
            return Get<T>(
                where: $"`id`={id}",
                transaction: transaction
            ).FirstOrDefault();
        }

        ///<summary>Get a list of fully loaded records with the corresponding ids.</summary>
        ///<param name="ids"></param>
        ///<returns></returns>
        public static List<T> Get<T>(uint[] ids, Transaction transaction = null) where T : DefaultModel
        {
            return Get<T>(
                where: $"`id` IN ({string.Join(",", ids)}",
                transaction: transaction
            );
        }

        ///<summary>Write record changes to the database.</summary>
        public void Write()
        {
            if (this.Origin == null || this.Id == 0)
                return;

            bool changes = false;
            string sql = $"UPDATE {Instance.Table} SET ";
            foreach (FieldAttribute field in Instance.Fields)
            {
                // Field is not writeable or generated. Skip.
                if (!field.Writeable || field.AutoIncrement)
                    continue;

                // Skip fields without changes.
                string strValue = field.ValueAsString(this);
                if (strValue == field.ValueAsString(this.Origin))
                    continue;

                // We had field changes, separate new changes by commata.
                if (changes)
                    sql += ", ";
                changes = true;

                // If field references to another model class -> we only care about the ID.
                sql += field.IsModelClass && strValue == "0" ? $"{field.Column}=NULL" : $"{field.Column}='{strValue}'";
            }
            if (!changes)
                return;
            sql += $" WHERE id={this.Id};";

            try
            {
                NonQuery(sql);
            }
            catch
            {
                // Revert to origin if anything goes wrong.
                Revert();
            }

            if (this.AttachedTransaction != null)
                return;

            this.StoreInCache();
            this.Origin = this.MemberwiseClone();
        }

        ///<summary>Delete current record from the database.</summary>
        public void Delete()
        {
            NonQuery($"DELETE FROM {Instance.Table} WHERE id={this.Id};");
            Cache.Remove(CacheKey);
            this.Origin = null;
        }

        ///<summary>
        ///Stores the current instance in cache.
        ///Records with attached transactions will be ignored.
        ///</summary>
        internal void StoreInCache(int keepSeconds = 60)
        {
            // We only cache records not isolated within transactions.
            if (AttachedTransaction == null && Id != 0)
                Cache.Set(CacheKey, this, keepSeconds);
        }
    }
}
