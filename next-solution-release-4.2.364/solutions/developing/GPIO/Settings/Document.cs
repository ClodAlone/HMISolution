using DocumentManager.ComponentService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using UFInterfaces.Constants;
using UIMsgBoxAlertService.ComponentService;
using Utilities;
#if !WINDOWS_UWP
using VFS;
#endif

namespace GPIO.Settings
{
    public enum GPIOType
    {
        input,
        output
    };

    [DataContract(Name = "PINSettings", Namespace = Namespaces.UriProgea)]
    public class PINSettings : INotifyPropertyChanged
    {
#region Persistance

        [DataMember]
        int pin;

        [DataMember]
        int debounceTimeout;

        [DataMember]
        GPIOType type;

        [DataMember]
        bool pubnubPublish;

        [DataMember]
        String pubnubChannel;

#endregion

#region Properties

        public String Name
        {
            get
            {
                return String.Format("{0}-{1}", Type, Pin);
            }
        }

        public int Pin
        {
            get
            {
                return pin;
            }
            set
            {
                if (pin == value)
                    return;
                pin = value;
                OnPropertyChanged("Pin");
            }
        }

        public int DebounceTimeout
        {
            get
            {
                return debounceTimeout;
            }
            set
            {
                if (debounceTimeout == value)
                    return;
                debounceTimeout = value;
                OnPropertyChanged("DebounceTimeout");
            }
        }

        public GPIOType Type
        {
            get
            {
                return type;
            }
            set
            {
                if (type == value)
                    return;
                type = value;
                OnPropertyChanged("Type");
            }
        }

        public bool PubNubPublish
        {
            get
            {
                return pubnubPublish;
            }
            set
            {
                if (pubnubPublish == value)
                    return;
                pubnubPublish = value;
                OnPropertyChanged("PubNubPublish");
            }
        }

        public String PubNubChannel
        {
            get
            {
                return pubnubChannel;
            }
            set
            {
                if (pubnubChannel == value)
                    return;
                pubnubChannel = value;
                OnPropertyChanged("PubNubChannel");
            }
        }

#endregion

#region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                //DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                var e = new PropertyChangedEventArgs(propertyName);
                //// If the subscriber is a DispatcherObject and different thread
                //if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                //{
                //    // Invoke handler in the target dispatcher's thread
                //    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                //}
                //else // Execute handler as is
                handler(this, e);
            }
        }

#endregion
    }

    [DataContract(Name = "GPIOSettings", Namespace = Namespaces.UriProgea)]
    public class Document : INotifyPropertyChanged
    {
#region Persistance

        [DataMember]
        String pubnubPublishKey;

        [DataMember]
        String pubnubSubscribeKey;

        [DataMember]
        List<PINSettings> listPIN;

        [DataMember]
        Guid id;

#endregion

#region Properties

        public List<PINSettings> ListPIN
        {
            get
            {
                return listPIN;
            }
            set
            {
                if (listPIN == value)
                    return;
                listPIN = value;
                OnPropertyChanged("ListPIN");
                NeedsToSave = true;
            }
        }

        public String PubNubPublishKey
        {
            get
            {
                return pubnubPublishKey;
            }
            set
            {
                if (pubnubPublishKey == value)
                    return;
                pubnubPublishKey = value;
                OnPropertyChanged("PubNubPublishKey");
                NeedsToSave = true;
            }
        }

        public String PubNubSubscribeKey
        {
            get
            {
                return pubnubSubscribeKey;
            }
            set
            {
                if (pubnubSubscribeKey == value)
                    return;
                pubnubSubscribeKey = value;
                OnPropertyChanged("PubNubSubscribeKey");
                NeedsToSave = true;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid Id
        {
            get
            {
                if (Parent != null)
                    return Parent.Id;
                return Guid.Empty;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public bool Protected
        {
            get
            {
                if (Parent != null)
                    return Parent.Protected;
                return false;
            }
        }

        IDocument parent;
        public IDocument Parent
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value;
            }
        }

        String fullPath;
        public String FullPath
        {
            get
            {
                return fullPath;
            }
            set
            {
                fullPath = value;
            }
        }

        bool needsToSave;
        public bool NeedsToSave
        {
            get
            {
                return needsToSave;
            }
            set
            {
                needsToSave = value;
            }
        }

        #endregion

        #region Methods

        bool IsBelongFromParent(IDocument parent)
        {
            return id == Guid.Empty || id == parent.Id;
        }

        static Document ReadFromStream(Stream reader)
        {
            var formatter = new DataContractSerializer(typeof(Document));
            var document = formatter.ReadObject(reader) as Document;
            return document;
        }

        public static Document FromFile(String fullPath, IDocument parent, bool bThrowExceptions = false)
        {
            try
            {
#if !WINDOWS_UWP
                FileSystemProviderBase fileSystemProvider = parent.fileSystemProviderBase;
                if (fileSystemProvider != null)
                {
                    if (fileSystemProvider.Exists(new FileManagerFile(fileSystemProvider, fullPath)))
                    {
                        var data = fileSystemProvider.ReadFile(new FileManagerFile(fileSystemProvider, fullPath));
                        using (var memoryStream = new MemoryStream(data))
                        {
                            return ReadFromStream(memoryStream);
                        }
                    }

                    return null;
                }
#endif
                if (File.Exists(fullPath))
                {
#if !WINDOWS_UWP
                    if (parent.Protected || !Utilities.IO.FileSystem.IsXmlFile(fullPath))
                    {
                        var data = WPFUtilities.CryptString.CryptString.DecryptString(File.ReadAllText(fullPath));
                        using (var reader = new MemoryStream(Convert.FromBase64String(data)))
                        {
                            var document = ReadFromStream(reader);
                            if (!document.IsBelongFromParent(parent))
                            {
                                if (bThrowExceptions)
                                    throw new UnauthorizedAccessException(String.Format(Properties.Resources.ErrorValidatingDocument, fullPath));

                                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                if (uiMsgBox != null)
                                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorValidatingDocument, fullPath));
                                return null;
                            }

                            return document;
                        }
                    }
                    else
#endif
                    {
                        using (var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                        {
                            return ReadFromStream(fileStream);
                        }
                    }
                }
            }
            catch
            {
                if (bThrowExceptions)
                    throw new UnauthorizedAccessException(String.Format(Properties.Resources.ErrorValidatingDocument, fullPath));
#if !WINDOWS_UWP
                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (uiMsgBox != null)
                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, fullPath));
#endif
            }

            return null;
        }

#if !WINDOWS_UWP
        public static void CopyFile(String fullPath, String newPath, bool bCopy, IDocument parent)
        {
            FileSystemProviderBase fileSystemProvider = parent.fileSystemProviderBase;
            FileSystemProviderBase targetVFS = null;
            bool bDisposeTargetVFS = true;
            bool bTargetDataSource = XpoHelpers.XpoHelper.IsDataSource(newPath);
            if (bTargetDataSource)
            {
                targetVFS = new DataSourceFileSystemProvider("")
                {
                    ConnectionString = newPath
                };
            }
            else
            {
                if (!Path.IsPathRooted(newPath))
                {
                    targetVFS = fileSystemProvider;
                    bDisposeTargetVFS = false;
                }
            }

            try
            {
                if (fileSystemProvider != null)
                {
                    var fileManagerFile = new FileManagerFile(fileSystemProvider, fullPath);
                    if (fileSystemProvider.Exists(fileManagerFile))
                    {
                        var data = fileSystemProvider.ReadFile(fileManagerFile);
                        if (targetVFS != null)
                        {
                            if (bDisposeTargetVFS)
                                targetVFS.UploadFile(null, fullPath, data);
                            else
                                targetVFS.UploadFile(null, newPath, data);
                        }
                        else
                        {
                            File.WriteAllBytes(newPath, data);
                        }

                        if (!bCopy)
                            fileSystemProvider.DeleteFile(fileManagerFile);
                    }
                }
                else
                {
                    if (File.Exists(fullPath))
                    {
                        if (targetVFS != null)
                        {
                            targetVFS.UploadFile(null, fullPath.Replace(parent.rootBase, parent.rootBaseDB), 
                                File.ReadAllBytes(fullPath));
                        }
                        else
                            File.Copy(fullPath, newPath, true);
                    }

                    if (!bCopy)
                        RemoveFile(fullPath);

                }
            }
            finally
            {
                if (bDisposeTargetVFS && targetVFS != null && targetVFS is DataSourceFileSystemProvider)
                    (targetVFS as DataSourceFileSystemProvider).Dispose();
            }
        }

        public static void RemoveFile(string fullPath, FileSystemProviderBase fileSystemProvider = null)
        {
            if (fileSystemProvider != null)
            {
                var fileManagerFile = new FileManagerFile(fileSystemProvider, fullPath);
                if (fileSystemProvider.Exists(fileManagerFile))
                {
                    fileSystemProvider.DeleteFile(fileManagerFile);
                }
            }
            else
            {
                if (File.Exists(fullPath))
                    File.Delete(fullPath);
            }
        }

        public bool SaveToFile(bool forceEncryption = false)
        {
            try
            {
                if (Parent.fileSystemProviderBase != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        if (!WriteProjectDataStream(memoryStream))
                            return false;

                        Parent.fileSystemProviderBase.UploadFile(null, FullPath, memoryStream.ToArray());
                        return true;
                    }
                }


                Directory.CreateDirectory(Path.GetDirectoryName(FullPath));

                if (forceEncryption || Protected)
                {
                    id = Id;
                    using (var memoryStream = new MemoryStream())
                    {
                        if (!WriteProjectDataStream(memoryStream))
                            return false;

                        var str = Convert.ToBase64String(memoryStream.ToArray());
                        var toWrite = WPFUtilities.CryptString.CryptString.EncryptString(str);
                        File.WriteAllText(FullPath, toWrite);
                    }
                }
                else
                {
                    id = Guid.Empty;
                    using (var ostrm = File.Open(FullPath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        if (!WriteProjectDataStream(ostrm))
                            return false;
                    }
                }
            }
            catch (Exception ex)
            {
                if (Environment.UserInteractive)
                    MessageBox.Show(String.Format(Properties.Resources.ErrorSavingDocument, ex.Message),
                        FullPath, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private bool WriteProjectDataStream(Stream ostrm)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Encoding = System.Text.Encoding.UTF8,
                Indent = true,
                CloseOutput = true
            };

            using (XmlWriter writer = XmlDictionaryWriter.Create(ostrm, settings))
            {
                bool bRet = false;
                try
                {
                    var serializer = new DataContractSerializer(typeof(Document));
                    serializer.WriteObject(writer, this);
                    bRet = true;
                }
                finally
                {
                    writer.Close();
                }

                return bRet;
            }
        }
#endif
#endregion

#region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                //DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                var e = new PropertyChangedEventArgs(propertyName);
                //// If the subscriber is a DispatcherObject and different thread
                //if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                //{
                //    // Invoke handler in the target dispatcher's thread
                //    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                //}
                //else // Execute handler as is
                handler(this, e);
            }
        }

#endregion
    }
}
