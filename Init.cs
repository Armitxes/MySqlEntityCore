using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MySqlEntityCore {
	class Init {
        ///<summary>If true: drop columns not present as model field.</summary>
        public bool DropColumns { get; set; } = false;
		internal static List<Type> ModelClasses { get; set; } = new List<Type>();

        ///<summary>Sync models and fields to the database.</summary>
		public Init()
		{
			InitModels();
		}

		internal void InitModels()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			IEnumerable<Type> classes = assemblies.SelectMany(t => t.GetTypes()).Where(t => t.IsClass);
			ModelClasses = classes.Where(t => t.GetCustomAttribute<ModelAttribute>() != null).ToList();
            ModelClasses.ForEach(t => ModelAttribute.Get(t).UpdateTable(dropColumns: DropColumns));
            
            // All tables are created and updated at this point. Now we can handle foreign keys
            ModelClasses.ForEach(t => ModelAttribute.Get(t).UpdateConstraints());
		}
	}
}
