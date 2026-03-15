using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CommandManager.Extensions
{
    public static class CommandManagerExtensions
    {
        readonly static Object lockCache = new object();

        readonly static Dictionary<Type, Dictionary<Type, IList<PropertyInfo>>> mapCacheBrowsableProperties;
        static CommandManagerExtensions()
        {
            lock (lockCache)
            {
                mapCacheBrowsableProperties = new Dictionary<Type, Dictionary<Type, IList<PropertyInfo>>>();
            }
        }

        public static IList<PropertyInfo> GetBrowsablePropertiesOfType<T>(this CommandManager command)
        {
            var commandType = command.GetType();
            var propertyType = typeof(T);

            lock (lockCache)
            {
                if (!mapCacheBrowsableProperties.ContainsKey(commandType))
                    mapCacheBrowsableProperties.Add(commandType, new Dictionary<Type, IList<PropertyInfo>>());
                if (!mapCacheBrowsableProperties[commandType].ContainsKey(propertyType))
                {
                    var properties = (from p in commandType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                      where p.PropertyType == propertyType && p.CanRead && p.CanWrite &&
                                      (!Attribute.IsDefined(p, typeof(BrowsableAttribute)) ||
                                      (p.GetCustomAttributes(typeof(BrowsableAttribute), true).FirstOrDefault() as BrowsableAttribute).Browsable)
                                      select p).ToList();
                    mapCacheBrowsableProperties[commandType].Add(propertyType, properties);
                }

                return mapCacheBrowsableProperties[commandType][propertyType];
            }
        }

        public static void CleanPropertiesOfTypeCache()
        {
            lock (lockCache)
            {
                mapCacheBrowsableProperties.Clear();
            }
        }
    }
}
