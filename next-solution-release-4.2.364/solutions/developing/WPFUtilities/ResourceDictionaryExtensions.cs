using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Utilities
{
    public static class ResourceDictionaryExtensions
    {
        public static bool SetMediaElementAutoStart(MediaElement mp)
        {
            try
            {
                mp.LoadedBehavior = MediaState.Play;
                mp.UnloadedBehavior = MediaState.Stop;
                mp.MediaEnded += (o, ev) =>
                {
                    mp.LoadedBehavior = MediaState.Manual;
                    mp.Stop();
                    mp.Play();
                };
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void SetMediaElementAutoStart(List<MediaElement> list)
        {
            list.ForEach(mp => SetMediaElementAutoStart(mp));
        }

        public static Dictionary<Object, Object> GetAllResourceTypes(this ResourceDictionary source, Type type)
        {
            Dictionary<Object, Object> ret = new Dictionary<object, object>();
            foreach (var merge in source.MergedDictionaries)
            {
                var merging = GetAllResourceTypes(merge, type);
                foreach (var key in merging.Keys)
                {
                    if (ret.ContainsKey(key))
                        ret.Remove(key);
                    ret.Add(key, merging[key]);
                }
            }

            foreach (var resource in source.Keys)
            {
                if (source[resource].GetType() == type || source[resource].GetType().IsSubclassOf(type))
                {
                    if (ret.ContainsKey(resource))
                        ret.Remove(resource);
                    ret.Add(resource, source[resource]);
                }
            }

            return ret;
        }

        public static Dictionary<Object, Object> LoadFromFile(String file, Type type)
        {
            ResourceDictionary dict = new ResourceDictionary
            {
                Source = new Uri(file, UriKind.RelativeOrAbsolute)
            };

            return dict.GetAllResourceTypes(type);
        }

        public static Dictionary<Object, Object> GetAllResourceTypes(this FrameworkElement source, Type type, bool recursivesearch = true)
        {
            Dictionary<Object, Object> ret = new Dictionary<object, object>();
            foreach (var merge in source.Resources.MergedDictionaries)
            {
                var merging = GetAllResourceTypes(merge, type);
                foreach (var key in merging.Keys)
                {
                    if (ret.ContainsKey(key))
                        ret.Remove(key);
                    ret.Add(key, merging[key]);
                }
            }

            foreach (var resource in source.Resources.Keys)
            {
                if (source.Resources[resource] == null)
                    continue;

                var sourceType = source.Resources[resource].GetType();
                if (type == sourceType || sourceType.IsSubclassOf(type))
                {
                    if (ret.ContainsKey(resource))
                        ret.Remove(resource);
                    ret.Add(resource, source.Resources[resource]);
                }
            }

            if (recursivesearch)
            {
                FrameworkElement parent = LogicalTreeHelper.GetParent(source) as FrameworkElement;
                if (parent != null)
                {
                    var parentmap = parent.GetAllResourceTypes(type);
                    foreach (var p in parentmap.Keys)
                    {
                        if (ret.ContainsKey(p))
                            ret.Remove(p);
                        ret.Add(p, parentmap[p]);
                    }
                }
            }

            return ret;
        }

        static void GetAllResourceTypesInChildren(this FrameworkElement source,
                            Type type, Dictionary<Object, Object> map)
        {
            var children = LogicalTreeHelper.GetChildren(source).OfType<FrameworkElement>();
            foreach (var child in children)
            {
                child.GetAllResourceTypesInChildren(type, map);

                foreach (var merge in child.Resources.MergedDictionaries)
                {
                    var merging = GetAllResourceTypes(merge, type);
                    foreach (var key in merging.Keys)
                    {
                        if (map.ContainsKey(key))
                            map.Remove(key);
                        map.Add(key, merging[key]);
                    }
                }

                foreach (var resource in child.Resources.Keys)
                {
                    if (child.Resources[resource] == null)
                        continue;

                    Type childResourcesGetType = child.Resources[resource].GetType();
                    if (childResourcesGetType == type || childResourcesGetType.IsSubclassOf(type))
                    {
                        if (map.ContainsKey(resource))
                            map.Remove(resource);
                        map.Add(resource, source.Resources[resource]);
                    }
                }
            }
        }

        public static Dictionary<Object, Object> GetAllResourceTypesInChildren(this FrameworkElement source, 
                            Type type)
        {
            Dictionary<Object, Object> ret = new Dictionary<object, object>();
            foreach (var merge in source.Resources.MergedDictionaries)
            {
                var merging = GetAllResourceTypes(merge, type);
                foreach (var key in merging.Keys)
                {
                    if (ret.ContainsKey(key))
                        ret.Remove(key);
                    ret.Add(key, merging[key]);
                }
            }

            foreach (var resource in source.Resources.Keys)
            {
                if (source.Resources[resource] == null)
                    continue;
                var sourceType = source.Resources[resource].GetType();
                if (sourceType == type || sourceType.IsSubclassOf(type))
                {
                    if (ret.ContainsKey(resource))
                        ret.Remove(resource);
                    ret.Add(resource, source.Resources[resource]);
                }
            }

            source.GetAllResourceTypesInChildren(type, ret);

            return ret;
        }

        public static List<string> LoadResourceExclusions(string startFolder, string commonFolder = null)
        {
            try
            {
                var cFolder = commonFolder ?? ApplicationPropertiesHelper.GetProperty<String>("CommonFolder");
                var _excfile = String.Format("{0}.{1}\\ConfigFiles\\ResourceExclusions.xml", cFolder, AssemblyInfo.FileFormatMainVersion);
                var doc = XElement.Load(_excfile);
                var files = (from item in doc.Descendants("File")
                              where item.HasAttributes && item.Attribute("Path") != null
                              select
                              String.Format("{0}{1}", startFolder, item.Attribute("Path").Value)
                              ).ToList<string>();
                return files;
            }
            catch (Exception)
            {
                return new List<String>();
            }
        }

        public static List<String> GetAllResourceFileList(String pattern = "*.xaml", String startingFolder = null, String commonFolder = null)
        {
            string _startingFolder = null;
            if (string.IsNullOrEmpty(startingFolder))
            {
                List<String> _filelist = new List<string>();
                _startingFolder = String.Format("{0}\\Resources\\", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"));
                _filelist.AddRange(GetAllResourceFileList(pattern, _startingFolder, commonFolder));
                _startingFolder = String.Format("{0}\\Resources4\\", Utilities.ApplicationPropertiesHelper.GetProperty<String>("CommonFolder"));
                _filelist.AddRange(GetAllResourceFileList(pattern, _startingFolder, commonFolder));
                return _filelist;
            }
            else
                _startingFolder = String.Format("{0}\\", startingFolder);


            try
            {
                var filelist = pattern == null ? Directory.GetFiles(_startingFolder).ToList() : Directory.GetFiles(_startingFolder, pattern).ToList();

                Parallel.ForEach(Directory.GetDirectories(_startingFolder), d =>
                {
                    var _list = GetAllResourceFileList(pattern, d, commonFolder);
                    lock (filelist)
                    {
                        filelist.AddRange(_list);
                    }
                });

                var exclusions = LoadResourceExclusions(_startingFolder, commonFolder);
                exclusions.ForEach(excFile =>
                {
                    if (filelist.Contains(excFile))
                        filelist.Remove(excFile);
                });

                return filelist;
            }
            catch (Exception ex)
            {
                return new List<String>();
            }
        }

        static List<String> listCommonResources;
        public static void AddCommonResources(FrameworkElement fe, String startingFolder = null, String commonFolder = null)
        {
            if (listCommonResources == null)
                listCommonResources = GetAllResourceFileList(startingFolder: startingFolder, commonFolder: commonFolder);

            listCommonResources.ForEach(resource =>
            {
                try
                {
                    var resdict = new ResourceDictionary
                    {
                        Source = new Uri(resource, UriKind.RelativeOrAbsolute)
                    };
                    fe.Resources.MergedDictionaries.Add(resdict);
                }
                catch (Exception ex)
                {
                    if (Environment.UserInteractive)
                        MessageBox.Show(ex.Message);
                }
            });
        }
    }
}
