using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MySqlEntityCore {

    [AttributeUsage(AttributeTargets.Class)]
    public class ModelAttribute : Attribute {
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
                        _Fields.Insert(0, field);
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
            return new Connection().Query(
                "SELECT column_name, data_type, is_nullable, column_key, column_type, numeric_precision, CHARACTER_MAXIMUM_LENGTH, EXTRA " +
                "FROM information_schema.columns " +
                $"WHERE table_schema='{Connection.DefaultPool.Database}' AND table_name='{Table}';",
                true
            );
        }

        ///<summary>Return constraints of the current model.</summary>
        internal List<Dictionary<string, object>> DbTableConstraints()
        {
            return new Connection().Query(
                "SELECT kcu.`CONSTRAINT_NAME`, kcu.`COLUMN_NAME`, tc.`CONSTRAINT_TYPE`, kcu.`REFERENCED_COLUMN_NAME`, kcu.`REFERENCED_TABLE_NAME` " +
                "FROM information_schema.KEY_COLUMN_USAGE AS kcu " +
                "LEFT JOIN information_schema.table_constraints AS tc ON tc.`CONSTRAINT_SCHEMA`=kcu.table_schema AND tc.`table_name`=kcu.`table_name` AND tc.`CONSTRAINT_NAME`=kcu.`CONSTRAINT_NAME` " +
                $"WHERE kcu.`table_schema`='{Connection.DefaultPool.Database}' AND kcu.`table_name`='{Table}';",
                true
            );
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

            string sql = $"CREATE TABLE IF NOT EXISTS `{this.Table}` (";
            foreach (FieldAttribute field in Fields)
                sql += field.SqlCreate() + ",";
            sql = sql[..^1] + ") ENGINE=INNODB;";
            new Connection().NonQuery(sql);
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
                new Connection().NonQuery(sql);
            if (dropColumns)
                CleanupColumns();
        }

        internal void UpdateConstraints() {
            UpdatePrimaryKeys();
            UpdateForeignKeys();
        }

        internal void UpdateForeignKeys() {
            List<string> sql = new List<string>();
            List<string> cNames = new List<string>();
            IEnumerable<FieldAttribute> modelConstraints = Fields.Where(q => q.IsModelClass);
            List<Dictionary<string, object>> dbConstraints = DbTableConstraints();
            
            foreach (Dictionary<string, object> dC in dbConstraints) {
                string cType = (dC["CONSTRAINT_TYPE"] as string);
                if (cType != "FOREIGN KEY")
                    continue;
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
                string cName = (dC["CONSTRAINT_NAME"] as string);
                string column = (dC["COLUMN_NAME"] as string);
                string rTable = (dC["REFERENCED_TABLE_NAME"] as string);
                string rColumn = (dC["REFERENCED_COLUMN_NAME"] as string);
                FieldAttribute field = modelConstraints.Where(q => q.Column == column).FirstOrDefault();
                ModelAttribute refModel = ModelAttribute.Get(field.PropInfo.PropertyType);

                if (
                    field == null
                    || rColumn == ""
                    || refModel.Table != rTable
                )
                    sql.Add($"DROP FOREIGN KEY {cName}");
                else
                    cNames.Add(cName);
            }

            foreach (FieldAttribute mC in modelConstraints) {
                if (cNames.Contains(mC.Column))
                    continue;

                ModelAttribute refModel = ModelAttribute.Get(mC.PropInfo.PropertyType);

                sql.Add(
                    "ADD CONSTRAINT " +
                    $"`FK_{mC.Column}_x_{refModel.Table}` FOREIGN KEY (`{mC.Column}`) " +
                    $"REFERENCES `{refModel.Table}` (`Id`)"
                );

            }

            if (sql.Count() > 0)
                new Connection().NonQuery($"ALTER TABLE `{this.Table}` {string.Join(',', sql)};"); 
        }

        ///<summary>Update mismatching primary keys</summary>
        internal void UpdatePrimaryKeys() {
            List<string> sql = new List<string>();
            List<string> primaryFields = new List<string>();

            // Get all primary key fields from model
            foreach(FieldAttribute field in Fields) {
                if (!field.PrimaryKey)
                    continue;
                primaryFields.Add("`"+field.Column+"`");
                sql.Add(field.SqlDropAutoIncrement(sqlShort: true));
            }

            // Get all PK constraints from database
            List<Dictionary<string, object>> primaryConstraints = DbTableConstraints().Where(
                q => (q["CONSTRAINT_TYPE"] as string) == "PRIMARY KEY"
            ).ToList();
            
            // Check for mismatches between model and table
            bool validPKs = primaryConstraints.Count() == primaryFields.Count();
            foreach (Dictionary<string, object> primaryConstraint in primaryConstraints) {
                string column = (primaryConstraint["COLUMN_NAME"] as string);
                FieldAttribute field = Fields.Where(x => x.Column == column).FirstOrDefault();

                if (field == null || !field.PrimaryKey) {
                    validPKs = false;
                    if (field == null)
                        sql.Add($"DROP COLUMN `{column}`");
                    else {
                        sql.Add(field.SqlDropAutoIncrement(sqlShort: true));
                        sql.Add(field.SqlAlterModify(sqlShort: true));
                    }
                }
            }

            if (validPKs)
                return;
            if (primaryConstraints.Count() > 0)
                sql.Add("DROP PRIMARY KEY");
            if (primaryFields.Count() > 0)
                sql.Add($"ADD PRIMARY KEY ({string.Join(',', primaryFields)})");
            new Connection().NonQuery($"ALTER TABLE `{this.Table}` {string.Join(',', sql)};"); 
        }

        private void CleanupColumns()
        {
            string sql = "";
            foreach (Dictionary<string, object> x in DbTableInfo()) {
                string column = (x["COLUMN_NAME"] as string);
                FieldAttribute field = Fields.Where(q => q.Column == column).FirstOrDefault();
                if (field == null)
                    sql += $"DROP COLUMN `{column}`,";
            }
            if (sql == "")
                return;
            new Connection().NonQuery($"ALTER TABLE `{this.Table}` " + sql[..^1] + ";");
        }
    }
}
