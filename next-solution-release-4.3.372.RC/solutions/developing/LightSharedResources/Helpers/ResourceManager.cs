using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SharedResources.Helpers
{
    public static class ResourceManager
    {
        static ResourceDictionary CommonResources { get; set; }
        static Dictionary<string, ResourceDictionary> EditorResources { get; set; }
        public static BitmapImage GetCommonImage(string editor, string resourceName, bool bShared = false)
        {
            if (bShared)
            {
                if (CommonResources == null)
                    CommonResources = InitDictionary(Properties.Settings.Default.SharedImages);
                return GetImage(CommonResources, resourceName);
            }
            else
            {
                if (EditorResources == null)
                    EditorResources = new Dictionary<string, ResourceDictionary>();
                if (!EditorResources.ContainsKey(editor))
                    EditorResources.Add(editor, InitDictionary(editor));
                return GetImage(EditorResources[editor], resourceName);
            }
        }

        private static BitmapImage GetImage(ResourceDictionary resourceDictionary, string resourceName)
        {
            try
            {
                return resourceDictionary[resourceName] as BitmapImage;
            }
            catch
            {
            }
            return null;
        }

        private static ResourceDictionary InitDictionary(string basename)
        {
            try
            {
                string path = string.Format(@"pack://application:,,,/SharedResources;component/Resources/{0}.xaml", basename);
                Uri uri = new Uri(path, UriKind.RelativeOrAbsolute);
                ResourceDictionary resourceDictionary = new ResourceDictionary()
                { Source = uri };
                return resourceDictionary;
            }
            catch (Exception ex)
            {
                return new ResourceDictionary();
            }
        }

    }
}
