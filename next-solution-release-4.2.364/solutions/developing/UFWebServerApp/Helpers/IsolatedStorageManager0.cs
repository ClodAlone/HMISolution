using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using System.IO;
using System.Runtime.Serialization;

namespace UFWebServer.Helpers
{
    public class IsolatedStorageManager0
    {
        public static T GetAppSettings<T>(String key) where T : class
        {
            return IsolatedStorageSettings.ApplicationSettings[key] as T;
        }

        public static void SetAppSettings(String key, Object o)
        {
            IsolatedStorageSettings.ApplicationSettings[key] = o;
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        private static String ValidateFileName(String fileName)
        {
            int nIndex = fileName.IndexOf(':');
            if (nIndex != -1)
                fileName = fileName.Substring(nIndex + 1);
            return fileName;
        }

        public static void SaveData(string data, string fileName)
        {
            fileName = ValidateFileName(fileName);

            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(fileName, FileMode.OpenOrCreate, isf))
                {
                    using (StreamWriter sw = new StreamWriter(isfs))
                    {
                        sw.Write(data);
                        sw.Close();
                    }
                }
            }
        }

        public static string LoadData(string fileName)
        {
            fileName = ValidateFileName(fileName);

            string data = String.Empty;
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(fileName, FileMode.Open, isf))
                {
                    using (StreamReader sr = new StreamReader(isfs))
                    {
                        string lineOfData = String.Empty;
                        while ((lineOfData = sr.ReadLine()) != null)
                            data += lineOfData;
                    }
                }
            }
            return data;
        }

        public static void SaveData(String fileName, Object data)
        {
            fileName = ValidateFileName(fileName);

            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                string dirName = System.IO.Path.GetDirectoryName(fileName);
                isf.CreateDirectory(dirName);

                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(fileName, FileMode.Create, isf))
                {
                    try
                    {
                        isfs.Position = 0;
                        DataContractSerializer serializer = new DataContractSerializer(data.GetType());
                        serializer.WriteObject(isfs, data);
                    }
                    finally
                    {
                    }
                }
            }
        }

        public static T LoadData<T>(String fileName) where T : class
        {
            fileName = ValidateFileName(fileName);

            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(fileName, FileMode.Open, isf))
                {
                    try
                    {
                        DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                        return serializer.ReadObject(isfs) as T;
                    }
                    finally
                    {
                    }
                }
            }
        }
    }
}
