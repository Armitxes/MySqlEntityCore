using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MySqlEntityCore {

    [AttributeUsage(AttributeTargets.Class)]
    class ModelAttribute : Attribute {
        // Do not use constructors as they execute with every GetCustomAttributes call
        public string Table { get; set; }
        public string OrderBy { get; set; }

        internal Type ExternalType { get; set; }

        private List<FieldAttribute> _Fields { get; set; }
        public List<FieldAttribute> Fields {
            get
            {
                if (_Fields != null)
                    return _Fields;

                _Fields = new List<FieldAttribute>();

                if (ExternalType == null)
                    return _Fields;
                
                foreach (PropertyInfo property in ExternalType.GetRuntimeProperties())
                {
                    FieldAttribute field = FieldAttribute.Get(property);
                    if (field != null)
                        _Fields.Add(field);
                }
                return _Fields;
            }
        }

        internal string SqlFieldList {
            get {
                string result = "";
                foreach (FieldAttribute field in this.Fields)
                    result += field.Column + ",";
                if (result[^1..] == ",")
                    result = result[..^1];
                return result;
            }
        }

        internal static ModelAttribute Get(Type type)
        {
            ModelAttribute model = type.GetCustomAttribute<ModelAttribute>();
            model.Table ??= type.Name;
            model.ExternalType = type;
            return model;
        }

        ///<summary>Return DB information about the current model.</summary>
        internal List<Dictionary<string, object>> DbTableInfo()
        {
            return new SqlQuery(
                "SELECT column_name, data_type, is_nullable, column_key, column_type, numeric_precision, CHARACTER_MAXIMUM_LENGTH, EXTRA " +
                "FROM information_schema.columns " +
                $"WHERE table_schema='{Connection.Default.Stream.Database}' AND table_name='{Table}';",
                true
            ).Result;
        }

        ///<summary>Return constraints of the current model.</summary>
        internal List<Dictionary<string, object>> DbTableConstraints()
        {
            return new SqlQuery(
                "SELECT COLUMN_NAME, CONSTRAINT_NAME, REFERENCED_COLUMN_NAME, REFERENCED_TABLE_NAME " +
                "FROM information_schema.KEY_COLUMN_USAGE " +
                $"WHERE table_schema='{Connection.Default.Stream.Database}' AND table_name='{Table}';",
                true
            ).Result;
        }

        internal Dictionary<string, object> DbFieldInfo(string fieldName)
        {
            return DbTableInfo().Where(
                x => (x["COLUMN_NAME"] as string) == fieldName
            ).FirstOrDefault();
        }

        ///<summary>Check if a table exists and create it if required.</summary>
        internal void CreateTable()
        {
            if (Connection.Tables().Contains(this.Table))
                return;

            string sql = $"CREATE TABLE IF NOT EXISTS {this.Table} (";
            foreach (FieldAttribute field in Fields)
                sql += field.SqlCreate() + ",";
            sql = sql[..^1] + ") ENGINE=INNODB;";
            new SqlQuery(sql);
        }

        ///<summary>Matches the existing table to the model.</summary>
        ///<param name="dropColumns">Drop table columns not represented in the model.</param>
        internal void UpdateTable(bool dropColumns = false)
        {
            if (!Connection.Tables().Contains(this.Table))
                CreateTable();

            string sql = "";
            foreach (FieldAttribute field in Fields)
                sql += field.SqlAlter(this);
            if (sql != "")
                new SqlQuery(sql);
            if (dropColumns)
                CleanupColumns();
            UpdateConstraints();
        }

        private void UpdateConstraints() {
            string sql = "";
            List<Dictionary<string, object>> dbConstraints = DbTableConstraints();
            IEnumerable<FieldAttribute> modelConstraints = Fields.Where(q => q.IsModelClass);

            // Remove deprecated constraints
            foreach (Dictionary<string, object> dbConstraint in dbConstraints) {
                string column = (dbConstraint["COLUMN_NAME"] as string);
                string cName = (dbConstraint["CONSTRAINT_NAME"] as string);
                if (cName == "PRIMARY" || cName == column)
                    continue;  // Handled by SqlAlter
   
                FieldAttribute field = modelConstraints.Where(q => q.Column == column).FirstOrDefault();
                if (field == null)
                    sql += $"ALTER TABLE {this.Table} DROP FOREIGN KEY {cName}; ";
            }

            // Sync and add missing constrains
            foreach (FieldAttribute modelConstraint in modelConstraints) {
                Dictionary<string, object> dbConstraint = dbConstraints.Where(x => (x["COLUMN_NAME"] as string) == modelConstraint.Column).FirstOrDefault();
                if (dbConstraint == null) {
                    // Create Relation
                    ModelAttribute refModel = ModelAttribute.Get(modelConstraint.PropInfo.PropertyType);
                    sql += (
                        $"ALTER TABLE {this.Table} ADD CONSTRAINT " +
                        $"`FK_{modelConstraint.Column}_x_{refModel.Table}` FOREIGN KEY (`{modelConstraint.Column}`) " +
                        $"REFERENCES `{refModel.Table}` (`Id`); "
                    );
                    continue;
                }
                // Sync Relation
            }

            if (sql != "")
                new SqlQuery(sql);
        }

        private void CleanupColumns()
        {
            string sql = "";
            foreach (Dictionary<string, object> x in DbTableInfo()) {
                string column = (x["COLUMN_NAME"] as string);
                FieldAttribute field = Fields.Where(q => q.Column == column).FirstOrDefault();
                if (field == null)
                    sql += $"DROP COLUMN {column},";
            }
            if (sql == "")
                return;
            new SqlQuery($"ALTER TABLE {this.Table} " + sql[..^1] + ";");
        }
    }
}
