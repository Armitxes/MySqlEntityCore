using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace MySqlEntityCore.Template {

    /// <summary>Create a model with an unsigned Id as primary key.</summary>
    public class DefaultModel : Template.Core {

        [Field(PrimaryKey = true, AutoIncrement = true)]
        public uint Id { get; internal set; }

        private bool FullyQueried { get; set; } = false;

        public DefaultModel() { }

        /// <summary>Get the record with the corresponding id.</summary>
        /// <param name="id"></param>
        public DefaultModel(uint id) {
            this.Id = id;
            Load();
        }

        /// <summary>
        /// Load all Id related information from DB. Can only happen once.
        /// Useful for performant use of related fields.
        /// </summary>
        public void Load() {
            if (FullyQueried)
                return;
            Reload();
        }

        /// <summary>Reload the record from database.</summary>
        public void Reload() {
            this.ConstructFromDictionary(
                new Connection().Query($"SELECT {this.Instance.SqlFieldList} FROM {this.Instance.Table} WHERE id={this.Id};").FirstOrDefault()
            );
            FullyQueried = true;
        }

        /// <summary>Get a list of fully loaded records with the corresponding ids.</summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static List<T> Browse<T>(uint[] ids)
        {
            return Get<T>(
                where: $"`id` IN ({string.Join(",", ids)}"
            );
        }

        /// <summary>Create a new record in the database.</summary>
        /// <returns>Created model.</returns>
        public void Create()
        {          
            string fieldNames = "";
            string fieldValues = "";
            foreach (FieldAttribute field in this.Instance.Fields)
            {
                if (field.AutoIncrement)
                    continue;
                
                string value = field.ValueAsString(this);
                if (value == null)
                    continue;
                    
                fieldNames += field.Column + ",";
                fieldValues += "'" + value + "',";
            }

            if (fieldNames == "" || fieldValues == "")
                return;

            if (fieldNames[^1..] == ",")
                fieldNames = fieldNames[..^1];
            if (fieldValues[^1..] == ",")
                fieldValues = fieldValues[..^1];

            this.ConstructFromDictionary(
                new Connection().Query(
                    $"INSERT INTO {this.Instance.Table} ({fieldNames}) VALUES ({fieldValues}); SELECT LAST_INSERT_ID() AS Id;",
                    true
                ).FirstOrDefault()
            );
        }

        /// <summary>Write record changes to the database.</summary>
        public void Write()
        {
            if (this.Origin == null || this.Id == 0)
                return;

            bool changes = false;
            string sql = $"UPDATE {Instance.Table} SET ";
            foreach (FieldAttribute field in Instance.Fields)
            {
                if(!field.Writeable || field.AutoIncrement)
                    continue;
                string strValue = field.ValueAsString(this);
                if (strValue == field.ValueAsString(this.Origin))
                    continue;
                if (changes)
                    sql += ", ";
                changes = true;
                sql += field.IsModelClass && strValue == "0" ? $"{field.Column}=NULL" : $"{field.Column}='{strValue}'";
            }
            if (!changes)
                return;
            sql += $" WHERE id={this.Id};";
            new Connection().NonQuery(sql);
        }

        /// <summary>Delete current record from the database.</summary>
        public void Delete()
        {
            new Connection().NonQuery($"DELETE FROM {Instance.Table} WHERE id={this.Id};");
        }
    }
}
