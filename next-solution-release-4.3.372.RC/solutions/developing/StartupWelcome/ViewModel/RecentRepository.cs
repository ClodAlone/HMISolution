using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using StartupWelcome.Model;
using System.Threading;

namespace StartupWelcome.View_Model
{
    public class RecentRepository
    {
        #region Properties
       
       string connectionString = string.Empty;
       ObservableCollection<RecentInfo> _RecentList = new ObservableCollection<RecentInfo>();

       public ObservableCollection<RecentInfo> RecentList
       {
           get { return _RecentList; }
           set { _RecentList = value; }
       }
       #endregion

       #region Constructor

       public RecentRepository()
       {
           RecentList = new ObservableCollection<RecentInfo>();
           this.PopulateRecentInfo();
       }

       #endregion

       #region Methods
       readonly String StoreFileName = String.Format("{0}.dat", System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));

       /// <summary>
       /// Populates the zip codes.
       /// </summary>
       public void PopulateRecentInfo()
       {
            try
            {
                IsolatedStorageFile isoStorage = GetStorage();
                if (null == isoStorage || string.IsNullOrEmpty(StoreFileName) ||
                    isoStorage.GetFileNames(StoreFileName).Length <= 0)
                    return;

                var name = Assembly.GetExecutingAssembly().GetName().Name;
                using (var Mutex = new Mutex(false, name))
                {
                    try
                    {
                        Mutex.WaitOne();
                    }
                    catch (AbandonedMutexException ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }

                    try
                    {
                        using (Stream stream = new IsolatedStorageFileStream(StoreFileName, FileMode.OpenOrCreate, isoStorage))
                        {
                            XmlReaderSettings settings = new XmlReaderSettings
                            {
                                ConformanceLevel = ConformanceLevel.Document,
                                CloseInput = true
                            };

                            using (XmlReader reader = XmlReader.Create(stream, settings))
                            {
                                try
                                {
                                    DataContractSerializer serializer = new DataContractSerializer(typeof(ObservableCollection<RecentInfo>));
                                    this.RecentList = serializer.ReadObject(reader) as ObservableCollection<RecentInfo>;
                                }
                                catch (Exception ex)
                                {
                                    RecentList.Clear();
                                    reader.Close();
                                }
                            }
                        }
                    }
                    catch
                    {

                    }
                    finally
                    {
                        Mutex.ReleaseMutex();
                    }
                }
            }
            catch
            { }
       }
       static IsolatedStorageFile GetStorage()
       {
            try
            {
                return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            }
            catch
            { }

            return null;
        }
        public void SaveRecentFileList()
       {
           IsolatedStorageFile isoStorage = GetStorage();
           if (null == isoStorage || string.IsNullOrEmpty(StoreFileName))
               return;

            var name = Assembly.GetExecutingAssembly().GetName().Name;
            using (var Mutex = new Mutex(false, name))
            {
                try
                {
                    Mutex.WaitOne();
                }
                catch (AbandonedMutexException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }

                try
                {
                    using (Stream stream = new IsolatedStorageFileStream(StoreFileName, FileMode.Create, isoStorage))
                    {
                        XmlWriterSettings settings = new XmlWriterSettings
                        {
                            Indent = true,
                            OmitXmlDeclaration = false,
                            Encoding = Encoding.UTF8
                        };

                        using (XmlWriter writer = XmlWriter.Create(stream, settings))
                        {
                            try
                            {
                                DataContractSerializer serializer = new DataContractSerializer(typeof(ObservableCollection<RecentInfo>));
                                serializer.WriteObject(writer, this.RecentList);
                            }
                            catch (Exception ex)
                            {
                                writer.Close();
                            }
                        }
                    }
                }
                catch
                {

                }
                finally
                {
                    Mutex.ReleaseMutex();
                }
            }
       }
       public void ClearRecent()
       {
           this.RecentList.Clear();
       }
        #endregion
   }
}
