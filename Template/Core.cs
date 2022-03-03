using System;
using System.Collections.Generic;
using System.Reflection;

namespace MySqlEntityCore.Template {
	/// <summary>
	/// Core functionality for all models.
	/// </summary>
	public class Core {
		internal object Origin { get; set; }

        private Type _ChildType;
        internal Type ChildType {
            get {
                if (_ChildType == null)
                    _ChildType = this.GetType();
                return _ChildType; 
            }
        }

        private ModelAttribute _Instance;
        internal ModelAttribute Instance {
            get {
                if (_Instance == null)
                    _Instance = ModelAttribute.Get(ChildType);
                return _Instance; 
            }
        }

        public Core() {}

        public void ConstructFromDictionary(Dictionary<string, object> dict) {
            if (dict == null)
                return;

            var flags = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            foreach (KeyValuePair<string, object> item in dict) {
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
                else if (property.PropertyType.IsSubclassOf(typeof(Template.DefaultModel))) {
                    // We get uint value for class relation fields.
                    // For performance reasons we only provide the related ID to the instance - additionaly data is only loaded when accessed.
                    value = property.PropertyType.GetConstructor(System.Type.EmptyTypes).Invoke(System.Type.EmptyTypes);
                    property.PropertyType.GetProperty("Id").SetValue(value, Convert.ToUInt32(strValue));
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
            string where=null,
            string orderby=null,
            uint offset=0,
            uint limit=0
        ) {
            System.Type tType = typeof(T);
            List<T> records = new List<T>();
            ModelAttribute tTypeAttr = ModelAttribute.Get(tType);

            string sql = $"SELECT {tTypeAttr.SqlFieldList} FROM {tTypeAttr.Table}";
            if (where != null)
                sql += $" WHERE {where}";
            if (orderby != null)
                sql += $" ORDER BY {orderby}";
            if (offset > 0 || limit > 0) {
                sql += $" LIMIT";
                if (offset > 0 && limit > 0) 
                    sql += $" {offset},{limit}";
                else if (limit > 0)
                    sql += $" {limit}";
            }
            sql += ";";
                

            List<Dictionary<string, object>> query = new Connection().Query(sql, true);
            ConstructorInfo ctor = tType.GetConstructor(System.Type.EmptyTypes);
            if (
                ctor == null
                || query.Count == 0
            )
                return records;

            MethodInfo construct = tType.GetMethod("ConstructFromDictionary", new[] { query[0].GetType() });

            foreach (Dictionary<string, object> entry in query)
            {
                object objRecord = ctor.Invoke(System.Type.EmptyTypes);
                construct.Invoke(objRecord, new object[] { entry });
                records.Add((T)System.Convert.ChangeType(objRecord, tType));
            }
            return records;
        }
	}
}
