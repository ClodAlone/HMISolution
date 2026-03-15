using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Utilities.AssemblyResolver
{
    public class AssemblyResolver
    {
        #region Declarations
        readonly String[] searchingPaths;
        #endregion

        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static byte[] assemblyResolverValue { get; private set; } // bytes value for 'UFUAServerBase.dll'

        #region Static Constructors
#if !DEBUG
        static AssemblyResolver()
        {
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var assemblyPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(assembly.Location),
                    WPFUtilities.CryptString.CryptString.DecryptString("7ZAOPGDyt5+p5ZWbLnUi4Mpz/BJuDGQuKGK+83VZ1VM=")); // "UFUAServerBase.dll"
                assemblyResolverValue = System.IO.File.ReadAllBytes(assemblyPath);
            }
            catch
            { }
        }
#endif
        #endregion

        #region Constructors
        public AssemblyResolver(params string[] customPaths) : 
            this(true, customPaths)
        { }

        public AssemblyResolver(bool addRootPath, params string[] customPaths)
        {
            List<String> listpath = new List<String>();

            Assembly assembly = Assembly.GetEntryAssembly();
            string rootPath = System.IO.Path.GetDirectoryName(assembly.Location);
#if NET_STANDARD
            listpath.Add(rootPath);
#endif
            if (addRootPath)
            {
                var index = rootPath.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
                if (index != -1)
                    rootPath = rootPath.Substring(0, index);
                // Add root path for assembly resolve.
                listpath.Add(rootPath);
            }

            // Add probing's privatePath for assembly resolve.
            var config = ConfigurationManager.OpenExeConfiguration(assembly.Location);
            if (config != null)
            {
                var section = config.GetSection("runtime");
                string xml = section.SectionInformation.GetRawXml();
                if (!String.IsNullOrEmpty(xml))
                {
                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                    doc.LoadXml(xml);
                    var element = doc.GetElementsByTagName("probing");
                    if (element != null && element.Count > 0)
                    {
                        var attribute = element[0].Attributes["privatePath"];
                        if (attribute != null)
                        {
                            var privatePaths = attribute.Value.Split(';');
                            for (int ii = 0; ii < privatePaths.Length; ii++)
                            {
                                var lowerPath = String.Format("{0}{1}{2}", rootPath, System.IO.Path.DirectorySeparatorChar, privatePaths[ii]);
#if !NET_STANDARD
                                lowerPath = lowerPath.ToLower();
#endif
                                if (!listpath.Contains(lowerPath))
                                    listpath.Add(lowerPath);
                            }
                        }
                    }
                }
            }

            // Add custom path for assembly resolve.
            foreach (var path in customPaths)
            {
#if !NET_STANDARD
                var lowerPath = path.ToLower();
#else
                var lowerPath = path;
#endif
                if (!listpath.Contains(lowerPath))
                    listpath.Add(lowerPath);
            }

            searchingPaths = listpath.ToArray();

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }
        #endregion

        #region Assembly Resolver
        System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly[] currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            for (int ii = 0; ii < currentAssemblies.Length; ii++)
            {
                if (String.Compare(currentAssemblies[ii].FullName, args.Name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return currentAssemblies[ii];
                }
            }

            return FindAssembliesInDirectory(args.Name, searchingPaths);
        }

        Assembly FindAssembliesInDirectory(string assemblyName, string[] directories)
        {
            if (assemblyName.Contains("mscorlib"))
                return null;

            //Use the AssemblyName class to get the informations
            var assembly = new AssemblyName(assemblyName);
            var name = assembly.Name;
            var cultureName = String.Empty;
            if (!String.IsNullOrEmpty(assembly.CultureName))
                cultureName = String.Format("{0}{1}", assembly.CultureName, System.IO.Path.DirectorySeparatorChar);
            foreach (string directory in directories)
            {
                var filepath = String.Format("{0}{1}{2}{3}.dll", directory, System.IO.Path.DirectorySeparatorChar, cultureName, name);
                if (!System.IO.Directory.Exists(directory) || !System.IO.File.Exists(filepath))
                    continue;

                Assembly assm;
                if (TryLoadAssemblyFromFile(filepath, assembly, out assm))
                    return assm;
            }

            return null;
        }

        bool TryLoadAssemblyFromFile(string file, AssemblyName assemblyName, out Assembly assm)
        {
            try
            {
                // Convert the filename into an absolute file name for 
                // use with LoadFile. 
                file = new System.IO.FileInfo(file).FullName;
                var assembly = AssemblyName.GetAssemblyName(file);

                if (String.Compare(assemblyName.FullName, assembly.FullName, StringComparison.OrdinalIgnoreCase) == 0 ||
                    (String.Compare(assemblyName.Name, assembly.Name, StringComparison.OrdinalIgnoreCase) == 0/* && 
                    (assemblyName.Version == null || assemblyName.Version == assembly.Version) &&
                    (assemblyName.CultureName == null || assemblyName.CultureName == assembly.CultureName) ||
                    (assemblyName.GetPublicKeyToken() == null || assemblyName.GetPublicKeyToken() == assembly.GetPublicKeyToken())*/))
                {
                    assm = Assembly.LoadFile(file);
                    return true;
                }
            }
            catch
            {
                /* Do Nothing */
            }
            assm = null;
            return false;
        }
        #endregion
    }
}
