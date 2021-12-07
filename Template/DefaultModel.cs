using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace MySqlEntityCore.Template {

    /// <summary>Create a model with an unsigned Id as primary key.</summary>
    class DefaultModel : Template.Core {

        [Field(PrimaryKey = true, AutoIncrement = true)]
        public uint Id { get; set; }

        public DefaultModel() { }

        /// <summary>Get the record with the corresponding id.</summary>
        /// <param name="id"></param>
        public DefaultModel(uint id) {
            this.ConstructFromDictionary(
                new SqlQuery($"SELECT {this.Instance.SqlFieldList} FROM {this.Instance.Table} WHERE id={id};").Result.FirstOrDefault()
            );
        }

        /// <summary>Get a list of records with the corresponding ids.</summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static List<T> Browse<T>(int[] ids)
        {
            System.Type tType = typeof(T);
            List<T> records = new List<T>();
            ModelAttribute tTypeAttr = ModelAttribute.Get(tType);

            SqlQuery query = new SqlQuery(
                $"SELECT {tTypeAttr.SqlFieldList} FROM {tTypeAttr.Table} WHERE id IN ({string.Join(",", ids)});",
                true
            );
            ConstructorInfo ctor = tType.GetConstructor(System.Type.EmptyTypes);
            if (
                ctor == null
                || query.Result.Count == 0
            )
                return records;

            MethodInfo construct = tType.GetMethod("ConstructFromDictionary", new[] { query.Result[0].GetType() });

            foreach (Dictionary<string, object> entry in query.Result)
            {
                object objRecord = ctor.Invoke(System.Type.EmptyTypes);
                construct.Invoke(objRecord, new object[] { entry });
                records.Add((T)System.Convert.ChangeType(objRecord, tType));
            }
            return records;
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
                new SqlQuery(
                    $"INSERT INTO {this.Instance.Table} ({fieldNames}) VALUES ({fieldValues}); SELECT LAST_INSERT_ID();",
                    true
                ).Result.FirstOrDefault()
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
            new SqlQuery(sql);
        }

        /// <summary>Delete current record from the database.</summary>
        public void Delete()
        {
            new SqlQuery($"DELETE FROM {Instance.Table} WHERE id={this.Id};");
        }
    }
}
