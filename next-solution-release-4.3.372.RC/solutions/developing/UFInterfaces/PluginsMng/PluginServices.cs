using System;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace UFInterfaces
{
	/// <summary>
	/// Summary description for PluginServices.
	/// </summary>
	public class PluginServices
	{
		private Types.AvailablePlugins colAvailablePlugins = new Types.AvailablePlugins();
        private Object lockObject = new Object();

		/// <summary>
		/// A Collection of all Plugins Found and Loaded by the FindPlugins() Method
		/// </summary>
		public Types.AvailablePlugins AvailablePlugins
		{
			get 
            {
                lock (lockObject)
                {
                    return colAvailablePlugins;
                }
            }

			set 
            {
                lock (lockObject)
                {
                    colAvailablePlugins = value;
                }
            }
		}
		
		/// <summary>
		/// Searches the Application's Startup Directory for Plugins
		/// </summary>
		public void FindPlugins()
		{
			FindPlugins(AppDomain.CurrentDomain.BaseDirectory);
		}
		/// <summary>
		/// Searches the passed Path for Plugins
		/// </summary>
		/// <param name="Path">Directory to search for Plugins in</param>
        public void FindPlugins(string Path, String pattern)
        {
            //First empty the collection, we're reloading them all
            // colAvailablePlugins.Clear();

            if (!Directory.Exists(Path))
                return;

            //Go through all the files in the plugin directory
            var directoryGetFiles = Directory.GetFiles(Path, pattern);
            Parallel.ForEach(directoryGetFiles, fileOn =>
            // Array.ForEach(Directory.GetFiles(Path, pattern), fileOn =>
            {
                FileInfo file = new FileInfo(fileOn);
                //Preliminary check, must be .dll
                if (file.Extension.Equals(".dll"))
                    //Add the 'plugin'
                    AddPlugin(fileOn);
            });

            // Array.ForEach(Directory.GetDirectories(Path), FindPlugins);
        }

		public void FindPlugins(string Path)
		{
            FindPlugins(Path, "*.dll");
		}
		
		/// <summary>
		/// Unloads and Closes all AvailablePlugins
		/// </summary>
		public void ClosePlugins()
		{
            lock (lockObject)
            {
                foreach (Types.AvailablePlugin pluginOn in colAvailablePlugins)
                {
                    if (pluginOn.Instance is IDisposable)
                    {
                        //Close all plugin instances
                        //We call the plugins Dispose sub first incase it has to do 
                        //Its own cleanup stuff
                        var dispose = pluginOn.Instance as IDisposable;
                        dispose.Dispose();
                    }

                    //After we give the plugin a chance to tidy up, get rid of it
                    pluginOn.Instance = null;
                }

                //Finally, clear our collection of available plugins
                colAvailablePlugins.Clear();
            }
		}
		
		private void AddPlugin(string FileName)
		{
			//Create a new assembly from the plugin file we're adding..
            try
            {
                Assembly pluginAssembly = Assembly.LoadFrom(FileName);


                var type = typeof(IUFInterfaceBase);
                var types = (from c in pluginAssembly.GetTypes() where 
                            type.IsAssignableFrom(c) select c);

                Parallel.ForEach(types, t =>
                {
                    //Create a new available plugin since the type implements the IPlugin interface
                    Types.AvailablePlugin newPlugin = new Types.AvailablePlugin();
                    //Set the filename where we found it
                    newPlugin.AssemblyPath = FileName;
                    System.Diagnostics.Trace.TraceInformation(String.Format("Loading Plugin {0}...", FileName));

                    //Create a new instance and store the instance in the collection for later use
                    //We could change this later on to not load an instance.. we have 2 options
                    //1- Make one instance, and use it whenever we need it.. it's always there
                    //2- Don't make an instance, and instead make an instance whenever we use it, then close it
                    //For now we'll just make an instance of all the plugins
                    try
                    {
                        newPlugin.Instance = Activator.CreateInstance(t) as IUFInterfaceBase;
                        lock (lockObject)
                        {
                            colAvailablePlugins.Add(newPlugin);
                        }
                    }
                    catch (Exception ex) {
                        System.Diagnostics.Trace.TraceWarning(String.Format("Loading Plugin {0}: {1}", FileName, ex));
                    }
                });


                /*
                //Next we'll loop through all the Types found in the assembly
                Parallel.ForEach(pluginAssembly.GetTypes(), pluginType =>
                // Array.ForEach(pluginAssembly.GetTypes(), pluginType =>
                {
                    if (pluginType.IsPublic)
                        //Only look at public types
                        if (!pluginType.IsAbstract)
                        {
                            //Only look at non-abstract types
                            //Gets a type object of the interface we need the plugins to match
                            Type typeInterface = pluginType.GetInterface("IUFInterfaceBase", true);
                            //Make sure the interface we want to use actually exists
                            if (typeInterface != null)
                            {
                                //Create a new available plugin since the type implements the IPlugin interface
                                Types.AvailablePlugin newPlugin = new Types.AvailablePlugin();
                                //Set the filename where we found it
                                newPlugin.AssemblyPath = FileName;
                                System.Diagnostics.Trace.TraceInformation(String.Format("Loading Plugin {0}...", FileName));

                                //Create a new instance and store the instance in the collection for later use
                                //We could change this later on to not load an instance.. we have 2 options
                                //1- Make one instance, and use it whenever we need it.. it's always there
                                //2- Don't make an instance, and instead make an instance whenever we use it, then close it
                                //For now we'll just make an instance of all the plugins
                             newPlugin.Instance = Activator.CreateInstance(pluginAssembly.GetType(pluginType.ToString())) as IUFInterfaceBase;
                                //Set the Plugin's host to this class which inherited IPluginHost
                                //Add the new plugin to our collection here
                                lock (lockObject)
                                {
                                    colAvailablePlugins.Add(newPlugin);
                                }
                                //cleanup a bit
                                newPlugin = null;
                                typeInterface = null; //Mr. Clean			
                            }
                        }
                });
                */

                pluginAssembly = null; //more cleanup
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
            }
		}

	}
	namespace Types
	{
	}	
}
