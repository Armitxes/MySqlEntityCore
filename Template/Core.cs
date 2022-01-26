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
	}
}
