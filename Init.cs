using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MySqlEntityCore
{
    public class Init
    {
        internal static List<Type> ModelClasses { get; set; } = new List<Type>();

        ///<summary>Sync models and fields to the database.</summary>
        ///<param name="dropColumn">Drop table columns not represented in the model.</param>
		public Init(
            bool dropColumn = false
        )
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            IEnumerable<Type> classes = assemblies.SelectMany(t => t.GetTypes()).Where(t => t.IsClass);
            ModelClasses = classes.Where(t => t.GetCustomAttribute<ModelAttribute>() != null).ToList();
            ModelClasses.ForEach(t => ModelAttribute.Get(t).UpdateTable(dropColumn: dropColumn));

            // All tables are created and updated at this point. Now we can handle constraints.
            ModelClasses.ForEach(t => ModelAttribute.Get(t).UpdateConstraints());
        }
    }
}
