using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace DriverCodeBase.Helpers
{
    /// <summary>
    /// Public driver information
    /// </summary>
    public static class DriverInfo
    {
        /// <summary>
        /// Return the name of the driver.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static String GetDriverName(object obj)
        {
            return Assembly.GetAssembly(obj.GetType()).GetName().Name;
        }

        /// <summary>
        /// Return the assembly name of the driver.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static String GetAssemblyName(object obj)
        {
            return Assembly.GetAssembly(obj.GetType()).GetName().Name;
        }

        /// <summary>
        /// Return the driver dynamic tags change file's extension.
        public static String GetChangeDynTagsExtension()
        {
            return Properties.Settings.Default.DefaultExchFileExt;
        }
    }
}
