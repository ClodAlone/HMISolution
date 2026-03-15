using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ADPluginBase.Helpers
{
    public static class PluginInfo
    {
        private static Assembly Plugin;
        internal static void Initialize(object plugin)
        {
            if (Plugin == null)
                Plugin = Assembly.GetAssembly(plugin.GetType());
        }

        private static string _PluginName;
        public static string GetPluginName()
        {
            if (Plugin == null)
                throw new MemberAccessException("class not properly initialized.");

            if (_PluginName == null)
                _PluginName = Plugin.GetName().Name;

            return _PluginName;
        }
        public static string GetPluginName(object obj)
        {
            return Assembly.GetAssembly(obj.GetType()).GetName().Name;
        }
    }
}
