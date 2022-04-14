using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace MySqlEntityCore.Template
{
    /// <summary>
    /// Core functionality for all models.
    /// </summary>
    public class Core
    {
        internal object Origin { get; set; }

        private Type _ChildType;
        internal Type ChildType
        {
            get
            {
                if (_ChildType == null)
                    _ChildType = this.GetType();
                return _ChildType;
            }
        }

        private ModelAttribute _Instance;
        internal ModelAttribute Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = ModelAttribute.Get(ChildType);
                return _Instance;
            }
        }

        internal string _CacheKey;
        public string CacheKey
        {
            get
            {
                if (_CacheKey != null)
                    return _CacheKey;

                string result = ChildType.Name;

                foreach (FieldAttribute field in Instance.Fields.Where(x => x.PrimaryKey))
                {
                    object val = ChildType.GetProperty(field.Name).GetValue(this);
                    if (val == null)
                        return null;  // A required PK field is null. Too unsafe to work with that.
                    result += "." + val.ToString();
                }

                _CacheKey = result;
                return _CacheKey;
            }
        }

        public void ConstructFromClass(dynamic instance)
        {
            PropertyInfo[] properties = this.ChildType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (!property.CanWrite)
                    continue;

                var oldVal = property.GetValue(instance, null);
                property.SetValue(this, oldVal);
            }
        }

        public void ConstructFromDictionary(Dictionary<string, object> dict)
        {
            if (dict == null)
                return;

            var flags = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            foreach (KeyValuePair<string, object> item in dict)
            {
                PropertyInfo property = ChildType.GetProperty(item.Key, flags);
                if (property == null)
                    continue;

                object value = item.Value;
                string strValue = value.ToString();
                if (strValue == "")
                    value = null;
                else if (property.PropertyType == typeof(string))
                    value = strValue;
                else if (property.PropertyType == typeof(uint))
                    value = Convert.ToUInt32(value);
                else if (property.PropertyType == typeof(DateTime))
                    value = DateTime.Parse(strValue);
                else if (property.PropertyType.IsSubclassOf(typeof(Template.DefaultModel)))
                {
                    object objRecord = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(property.PropertyType);
                    property.PropertyType.GetProperty("Id").SetValue(objRecord, Convert.ToUInt32(strValue));
                    value = objRecord;
                }
                property.SetValue(this, value);
            }
            this.Origin = this.MemberwiseClone();
        }

        /// <summary>Get a list of records by the given conditions. Leave null/0 for all lines.</summary>
        /// <param name="where">SQL "WHERE" condition</param>
        /// <param name="orderby">SQL "ORDER BY" statement</param>
        /// <param name="offset">Result offset</param>
        /// <param name="limit">Result limit</param>
        /// <returns></returns>
        public static List<T> Get<T>(
            string where = null,
            string orderby = null,
            uint offset = 0,
            uint limit = 0
        )
        {
            System.Type tType = typeof(T);
            List<T> records = new List<T>();
            ModelAttribute tTypeAttr = ModelAttribute.Get(tType);

            string sql = $"SELECT {tTypeAttr.SqlFieldList} FROM {tTypeAttr.Table}";
            if (where != null)
                sql += $" WHERE {where}";
            if (orderby != null)
                sql += $" ORDER BY {orderby}";
            if (offset > 0 || limit > 0)
            {
                sql += $" LIMIT";
                if (offset > 0 && limit > 0)
                    sql += $" {offset},{limit}";
                else if (limit > 0)
                    sql += $" {limit}";
            }
            sql += ";";

            List<Dictionary<string, object>> query = new Connection().Query(sql);
            if (query.Count == 0)
                return records;

            MethodInfo construct = tType.GetMethod("ConstructFromDictionary", new[] { query[0].GetType() });
            foreach (Dictionary<string, object> entry in query)
            {
                object objRecord = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(tType);
                construct.Invoke(objRecord, new object[] { entry });
                records.Add((T)System.Convert.ChangeType(objRecord, tType));
            }
            return records;
        }
    }
}
