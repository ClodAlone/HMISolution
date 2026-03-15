using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace NextAR
{
    public static class Storage
    {
        public static async Task<string> ReadTextFileAsync(string path)
        {
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.GetFileAsync(path);
            return await FileIO.ReadTextAsync(file);
        }

        public static async void WriteTotextFileAsync(string fileName, string contents)
        {
            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, contents);
        }

        public static void SaveSettings(string key, string contents)
        {
            ApplicationData.Current.LocalSettings.Values[key] = contents;
        }

        public static bool IsSettingSet(string key)
        {
            var settings = ApplicationData.Current.LocalSettings;
            return settings.Values.ContainsKey(key);
        }

        public static string LoadSettings(string key)
        {
            var settings = ApplicationData.Current.LocalSettings;
            if (!settings.Values.ContainsKey(key))
                return String.Empty;
            return settings.Values[key].ToString();
        }

        public static void SaveSettingsInContainer(string user, string key, string contents)
        {
            var localSetting = ApplicationData.Current.LocalSettings;

            localSetting.CreateContainer(user, ApplicationDataCreateDisposition.Always);

            if (localSetting.Containers.ContainsKey(user))
            {
                localSetting.Containers[user].Values[key] = contents;
            }
        }
    }
}
