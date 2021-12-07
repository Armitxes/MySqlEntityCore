using System;
using System.Collections.Generic;
using System.Reflection;

namespace MySqlEntityCore.Template {
	/// <summary>
	/// Core functionality for all models.
	/// </summary>
	class Core {
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
                    value = Convert.ToUInt32(value);  // PSQL doesn't support unsigned types
                else if (property.PropertyType == typeof(DateTime))
                    value = DateTime.Parse(strValue);
                property.SetValue(this, value);
            }
            this.Origin = this.MemberwiseClone();
        }
	}
}
