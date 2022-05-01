using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MySqlEntityCore
{

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class FieldAttribute : Attribute
    {
        public string Column { get; set; }
        public string Name { get; private set; }
        public uint Size { get; set; }

        public uint Decimal { get; set; } = 2;

        ///<summary>Required primary key.</summary>
        public bool PrimaryKey { get; set; } = false;

        ///<summary>
        /// Automatically up the increment by 1 on insert.
        ///<para>Forces Unique and Required property.</para>
        ///</summary>
        public bool AutoIncrement { get; set; } = false;
        public bool Unique { get; set; } = false;
        public bool Required { get; set; } = false;

        ///<summary>If false, model field is readonly after initial create.</summary>
        public bool Writeable { get; set; } = true;

        public ModelAttribute Model { get; private set; }
        public PropertyInfo PropInfo { get; private set; }

        private string StrSize
        {
            get
            {
                if (Size == 0)
                    return "";
                return Size.ToString();
            }
        }

        public bool IsModelClass
        {
            get
            {
                return PropInfo.PropertyType.IsSubclassOf(typeof(Template.DefaultModel));
            }
        }

        internal static FieldAttribute Get(PropertyInfo property)
        {
            FieldAttribute field = property.GetCustomAttribute<FieldAttribute>();
            if (field == null)
                return null;
            field.PropInfo = property;
            field.Name = property.Name;
            field.Column ??= field.Name;

            if (field.PrimaryKey)
                field.Required = true;

            if (field.AutoIncrement)
            {
                field.Required = true;
                field.Unique = true;
            }

            return field;
        }

        internal string ValueAsString(object obj)
        {
            if (obj == null)
                return null;
            object value = PropInfo.GetValue(obj);
            if (value is null)
                return null;
            if (value is DateTime)
                return Convert.ToDateTime(value).ToString("yyyy-MM-dd HH:mm:ss");
            if (IsModelClass)
                return value == null ? null : (value as Template.DefaultModel).Id.ToString();
            return value.ToString();
        }

        internal string SqlCreate()
        {
            string[] strings = new string[] {
                "`"+Column+"`", SqlType(), SqlAutoIncrement(), SqlRequired(), SqlUnique()
            };
            return string.Join(" ", strings.Where(str => str != ""));
        }

        internal string SqlAlter(ModelAttribute model)
        {
            Model = model;
            Dictionary<string, object> fieldInfo = model.DbFieldInfo(Column);
            if (fieldInfo == null)
                return SqlAlterAddColumn();

            bool tblRequired = fieldInfo["IS_NULLABLE"].ToString() == "NO";
            bool tblAutoIncrement = fieldInfo["EXTRA"].ToString().Contains("auto_increment");
            string size = fieldInfo["CHARACTER_MAXIMUM_LENGTH"].ToString();
            if (size == "")
                size = "0";
            if (
                Required == tblRequired
                && Size.ToString() == size
                && AutoIncrement == tblAutoIncrement
            )
                return "";

            string sql = SqlAlterModify() + ";";
            if (this.Column == "Id")
                sql = "SET FOREIGN_KEY_CHECKS=0; " + sql + " SET FOREIGN_KEY_CHECKS=1;";
            return sql;
        }

        private string SqlAlterAddColumn()
        {
            string[] strings = new string[] {
                "`"+Column+"`", SqlType(), SqlAutoIncrement(), SqlRequired(), SqlUnique()
            };
            return $"ALTER TABLE `{Model.Table}` ADD COLUMN " + string.Join(" ", strings.Where(str => str != "")) + "; ";
        }

        ///<summary>Return the required sql to modify the current model field</summary>
        public string SqlAlterModify(bool sqlShort = false)
        {
            if (sqlShort)
                return $"MODIFY `{Column}` {SqlType()} {SqlAutoIncrement()} {SqlRequired()}";
            return $"ALTER TABLE `{Model.Table}` MODIFY `{Column}` {SqlType()} {SqlAutoIncrement()} {SqlRequired()}";
        }

        ///<summary>Return the required SQL to drop auto increment</summary>
        public string SqlDropAutoIncrement(bool sqlShort = false)
        {
            if (sqlShort)
                return $"MODIFY `{Column}` {SqlType()} {SqlRequired()}";
            return $"ALTER TABLE `{Model.Table}` MODIFY `{Column}` {SqlType()} {SqlRequired()}";
        }

        ///<summary>Return MySQL compatible data type for a C# data type.</summary>
        public string SqlType()
        {
            string typeName = PropInfo.PropertyType.Name.ToLower();
            if (typeName == "string" && Size > 8000)
                return "TEXT";

            if (IsModelClass)
                return "INT UNSIGNED";

            return typeName switch
            {
                "string" => $"VARCHAR({Size})",
                "int64" => "BIGINT",
                "int32" => "INT",
                "int16" => "SMALLINT",
                "uint64" => "BIGINT UNSIGNED",
                "uint32" => "INT UNSIGNED",
                "uint16" => "SMALLINT UNSIGNED",
                "decimal" => $"DECIMAL({Size}, {Decimal})",
                "double" => $"DOUBLE({Size}, {Decimal})",
                "single" => $"FLOAT({Size}, {Decimal})",
                "char" => "CHAR(1)",
                "boolean" => "CHAR(0)",
                "datetime" => "DATETIME",
                _ => "",
            };
        }

        public string SqlAutoIncrement() => AutoIncrement ? "AUTO_INCREMENT" : "";

        public string SqlRequired() => Required ? "NOT NULL" : "NULL";

        public string SqlUnique() => Unique ? "UNIQUE KEY" : "";
    }
}
