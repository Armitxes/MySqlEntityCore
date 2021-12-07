using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MySqlEntityCore {
	class Init {
		internal static List<Type> ModelClasses { get; set; } = new List<Type>();

		public Init()
		{
			Models();
		}

		public void Models()
		{
			LoadModels();
			ModelClasses.ForEach(t => ModelAttribute.Get(t).UpdateTable(dropColumns: true));
		}

		internal void LoadModels()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			IEnumerable<Type> classes = assemblies.SelectMany(t => t.GetTypes()).Where(t => t.IsClass);
			ModelClasses = classes.Where(t => t.GetCustomAttribute<ModelAttribute>() != null).ToList();
		}
	}
}
