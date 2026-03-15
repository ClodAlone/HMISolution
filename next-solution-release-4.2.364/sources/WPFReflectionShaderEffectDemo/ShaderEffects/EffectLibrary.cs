using System;
using System.Reflection;

namespace ShaderEffects
{
    internal static class Global
    {
        public static Uri MakePackUri(string relativeFile)
        {
#if SILVERLIGHT
            string uriString = @"/ShaderEffects;component/" + relativeFile;
            return new Uri(uriString, UriKind.RelativeOrAbsolute);
#else
            string uriString = "pack://application:,,,/" + AssemblyShortName + ";component/" + relativeFile;
            return new Uri(uriString);
#endif
        }

        private static string AssemblyShortName
        {
            get
            {
                if (_assemblyShortName == null)
                {
                    Assembly a = typeof(Global).Assembly;

                    // Pull out the short name.
                    _assemblyShortName = a.ToString().Split(',')[0];
                }

                return _assemblyShortName;
            }
        }

        private static string _assemblyShortName;
    }
}