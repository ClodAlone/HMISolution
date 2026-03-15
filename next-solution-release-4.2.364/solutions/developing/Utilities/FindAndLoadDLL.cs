using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Utilities
{
    public static class FindAndLoadDLL
    {
        public static List<T> LoadDLLs<T>(string path, string pattern, bool bThreadSearch = true)
        {
#if DEBUG
            String[] listFiles = Directory.GetFiles(Path.GetFullPath(path), pattern);
#endif
            List<Type> listOfType = new List<Type>();
            Parallel.ForEach(Directory.GetFiles(Path.GetFullPath(path), pattern), f =>
                {
                    try
                    {
                        var types = Assembly.LoadFile(f).GetTypes();
                        lock (listOfType)
                            listOfType.AddRange(types);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.TraceError("{0} : {1}", f.ToString(), ex.Message);
                        if (ex is System.Reflection.ReflectionTypeLoadException)
                        {
                            var loadEx = ex as System.Reflection.ReflectionTypeLoadException;
                            foreach (var item in loadEx.LoaderExceptions)
                                System.Diagnostics.Trace.TraceError(item.Message);
                        }
                    }
                });

            if (bThreadSearch)
            {
                return (from t in listOfType/*.AsParallel()*/
                            where !t.IsAbstract && typeof(T).IsAssignableFrom(t)
                            select (T)Activator.CreateInstance(t)).ToList();
            }
            else
            {
                return (from t in listOfType
                        where !t.IsAbstract && typeof(T).IsAssignableFrom(t)
                        select (T)Activator.CreateInstance(t)).ToList();
            }

            //var list = Directory.GetFiles(Path.GetFullPath(path), pattern)/*.AsParallel()*/
            //    .SelectMany(f => Assembly.LoadFile(f).GetTypes()
            //        .Where(t => !t.IsAbstract && typeof(T).IsAssignableFrom(t))
            //        .Select(t => (T)Activator.CreateInstance(t)))
            //    .ToList();
        }
    }
}