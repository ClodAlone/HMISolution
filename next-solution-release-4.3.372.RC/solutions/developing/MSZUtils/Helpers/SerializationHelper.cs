using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;

namespace MSZUtils.Controls
{
    internal class SerializationHelper
    {
        internal static IsolatedStorageFile GetStorage()
        {
            try
            {
                return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            }
            catch
            { }

            return null;
        }
        internal static String GetStoreFileNameDocking(String title, String controlName = null)
        {
            return String.Format("{0}.{1}Docking{2}.dat", title, System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location), !String.IsNullOrEmpty(controlName) ? String.Format("_{0}", controlName) : "");
        }
        internal static void SaveGridLayout(string gridLayout, string title)
        {

            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage)
                    return;

                using (var stream = new IsolatedStorageFileStream(GetStoreFileNameDocking(title, null), FileMode.OpenOrCreate, isoStorage))
                {
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Encoding = System.Text.Encoding.UTF8,
                        Indent = true,
                        CloseOutput = true
                    };

                    using (XmlWriter writer = XmlDictionaryWriter.Create(stream, settings))
                    {
                        bool bRet = false;
                        try
                        {
                            DataContractSerializer serializer = new DataContractSerializer(typeof(string));
                            serializer.WriteObject(writer, gridLayout);
                            bRet = true;
                        }
                        finally
                        {
                            writer.Close();
                        }
                        return;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        internal static string LoadGridLayout(string title)
        {
            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage || string.IsNullOrEmpty(title))
                    return null;

                using (var stream = new IsolatedStorageFileStream(GetStoreFileNameDocking(title, null), FileMode.OpenOrCreate, isoStorage))
                {
                    XmlDocument document = new XmlDocument();
                    using (XmlReader reader = XmlDictionaryReader.Create(stream, new XmlReaderSettings()
                    { DtdProcessing = System.Xml.DtdProcessing.Prohibit, ValidationType = ValidationType.None, CloseInput = true }))
                    {
                        string gridLayout = null;
                        bool bRet = false;
                        try
                        {
                            DataContractSerializer formatter = new DataContractSerializer(typeof(string));
                            gridLayout = formatter.ReadObject(reader) as string;
                            bRet = true;
                        }
                        catch (Exception ex1)
                        {

                        }
                        finally
                        {
                            reader.Close();
                        }
                        return gridLayout;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }
        internal static bool WriteProjectDataStream<T>(Stream ostrm, T memories) where T : class
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Encoding = System.Text.Encoding.Unicode,
                Indent = true,
                CloseOutput = true
            };

            using (XmlWriter writer = XmlDictionaryWriter.Create(ostrm, settings))
            {
                bool bRet = false;
                try
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                    serializer.WriteObject(writer, memories);
                    bRet = true;
                }
                finally
                {
                    writer.Close();
                }

                return bRet;
            }
        }
        internal static T ReadProjectDataFromStream<T>(Stream stream) where T : class
        {
            try
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                T ret = serializer.ReadObject(stream) as T;
                return ret;
            }
            catch (Exception ex)
            {
                return (T)Activator.CreateInstance(typeof(T));
            }
        }
    }
}