using System;

namespace UFInterfaces
{
    namespace Types
    {
        /// <summary>
        /// Collection for AvailablePlugin Type
        /// </summary>
        public class AvailablePlugins : System.Collections.CollectionBase
        {
            public AvailablePlugins()
            {
                
            }
            public AvailablePlugins(int capacity)
                : base(capacity)
            {
                
            }
            //A Simple Home-brew class to hold some info about our Available Plugins

            /// <summary>
            /// Add a Plugin to the collection of Available plugins
            /// </summary>
            /// <param name="pluginToAdd">The Plugin to Add</param>
            public void Add(Types.AvailablePlugin pluginToAdd)
            {
                List.Add(pluginToAdd);
            }

            /// <summary>
            /// Remove a Plugin to the collection of Available plugins
            /// </summary>
            /// <param name="pluginToRemove">The Plugin to Remove</param>
            public void Remove(Types.AvailablePlugin pluginToRemove)
            {
                List.Remove(pluginToRemove);
            }

            /// <summary>
            /// Finds a plugin in the available Plugins
            /// </summary>
            /// <param name="pluginNameOrPath">The name or File path of the plugin to find</param>
            /// <returns>Available Plugin, or null if the plugin is not found</returns>
            public Types.AvailablePlugin Find(string pluginNameOrPath)
            {
                Types.AvailablePlugin toReturn = null;

                //Loop through all the plugins
                foreach (Types.AvailablePlugin pluginOn in List)
                {
                    //Find the one with the matching name or filename
                    if (pluginOn.AssemblyPath.Equals(pluginNameOrPath))
                    {
                        toReturn = pluginOn;
                        break;
                    }
                }
                return toReturn;
            }
        }
    }
}
