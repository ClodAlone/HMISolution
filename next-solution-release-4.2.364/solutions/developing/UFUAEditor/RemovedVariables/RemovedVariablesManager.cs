using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UFUAEditor.Document;
using VFS;

namespace UFUAEditor.RemovedVariables
{
    internal class RemovedVariablesManager
    {
        #region Declarations
        readonly String filePath;
        readonly FileSystemProviderBase fileSystemProvider;
        readonly RemovedVariablesDocument document;
        readonly ILog log;
        #endregion

        #region Constructors
        public RemovedVariablesManager(string projectPath) : this (projectPath, null, null)
        { }

        public RemovedVariablesManager(string projectPath, FileSystemProviderBase fileSystemProvider, ILog log)
        {
            this.filePath = GetRemovedVariablesFileName(projectPath);
            this.fileSystemProvider = fileSystemProvider;
            this.document = ReadVariables();
            this.log = log;
            if (this.document == null)
            {
                this.document = new RemovedVariablesDocument();
                if (log != null)
                    log.ErrorFormat(Properties.Resources.ErrorReadingDocument, filePath);
            }
        }
        #endregion

        #region Public Methods
        public void CopyFrom(RemovedVariablesManager manager)
        {
            if (manager.document == null)
                return;

            foreach (var key in manager.document.AddressSpaceTags.Keys)
                document.AddressSpaceTags[key] = manager.document.AddressSpaceTags[key];

            foreach (var key in manager.document.PrototypeMembers.Keys)
                document.PrototypeMembers[key] = manager.document.PrototypeMembers[key];
        }

        public bool ExistVariable(String key)
        {
            return document.AddressSpaceTags.Contains(key);
        }

        public bool ExistMember(String key)
        {
            return document.PrototypeMembers.Contains(key);
        }

        public bool ExistGuid(Guid guid)
        {
            return document.RemovedNodeIds.Contains(guid);
        }

        public void AddVariable(String key, Guid value)
        {
            if (value == Guid.Empty)
                return;

            if (!document.RemovedNodeIds.Contains(value))
            {
                document.AddressSpaceTags.Add(key, value);
                document.RemovedNodeIds.Add(value, key);
            }
        }

        public void AddMember(String key, Guid value)
        {
            if (value == Guid.Empty)
                return;

            if (!document.RemovedNodeIds.Contains(value))
            {
                document.PrototypeMembers.Add(key, value);
                document.RemovedNodeIds.Add(value, key);
            }
        }

        public void RemoveVariable(String key)
        {
            if (document.AddressSpaceTags.Contains(key))
            {
                var guid = document.AddressSpaceTags[key];
                document.AddressSpaceTags.Remove(key);
                document.RemovedNodeIds.Remove(guid);
            }
        }

        public void RemoveMember(String key)
        {
            if (document.PrototypeMembers.Contains(key))
            {
                var guid = document.PrototypeMembers[key];
                document.PrototypeMembers.Remove(key);
                document.RemovedNodeIds.Remove(guid);
            }
        }

        public Guid FindVariable(String key)
        {
            if (document.AddressSpaceTags.Contains(key))
                return (Guid)document.AddressSpaceTags[key];

            return Guid.Empty;
        }

        public Guid FindMember(String key)
        {
            if (document.PrototypeMembers.Contains(key))
                return (Guid)document.PrototypeMembers[key];

            return Guid.Empty;
        }

        public String FindGuid(Guid guid)
        {
            if (guid == Guid.Empty)
                return null;
            
            if (document.RemovedNodeIds.Contains(guid))
                return (String)document.RemovedNodeIds[guid];

            return null;
        }

        public void Save()
        {
            SaveVariables();
        }

        public void Delete()
        {
            DeleteVariables();
        }
        #endregion

        #region Properties
        String Extension
        {
            get
            {
                return Properties.Settings.Default.RemovedTagsExt;
            }
        }
        #endregion

        #region Methods
        RemovedVariablesDocument ReadVariables()
        {
            if (fileSystemProvider != null)
            {
                if (fileSystemProvider.Exists(new FileManagerFile(fileSystemProvider, filePath)))
                {
                    var data = fileSystemProvider.ReadFile(new FileManagerFile(fileSystemProvider, filePath));
                    using (var memoryStream = new MemoryStream(data))
                    {
                        return ReadDataStream(memoryStream);
                    }
                }
                else
                    return new RemovedVariablesDocument();
            }
            else
            {
                // check for valid file.
                if (!File.Exists(filePath))
                {
                    return new RemovedVariablesDocument();
                }

                if (!Utilities.IO.FileSystem.IsXmlFile(filePath))
                {
                    var data = WPFUtilities.CryptString.CryptString.DecryptString(File.ReadAllText(filePath));
                    using (var reader = new MemoryStream(Convert.FromBase64String(data)))
                    {
                        return ReadDataStream(reader);
                    }
                }
                else
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        return ReadDataStream(fileStream);
                    }
                }
            }
        }

        bool SaveVariables()
        {
            if (fileSystemProvider != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    if (!WriteDataStream(memoryStream))
                        return false;

                    fileSystemProvider.UploadFile(null, filePath, memoryStream.ToArray());
                }
            }
            else
            {
                using (var ostrm = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    if (!WriteDataStream(ostrm))
                        return false;
                }
            }

            return true;
        }

        void DeleteVariables()
        {
            if (fileSystemProvider != null)
            {
                var fileManagerFile = new FileManagerFile(fileSystemProvider, filePath);
                if (fileSystemProvider.Exists(fileManagerFile))
                {
                    fileSystemProvider.DeleteFile(fileManagerFile);
                }
            }
            else
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }

            document.Clear();
        }

        String GetRemovedVariablesFileName(String projectPath)
        {
            if (projectPath == null) throw new ArgumentNullException("projectPath");

            return String.Format("{0}/{1}/{2}{3}", projectPath,
                                Properties.Settings.Default.TypeLabel,
                                Properties.Settings.Default.DefaultProjectName,
                                Extension);
        }

        RemovedVariablesDocument ReadDataStream(Stream stream)
        {
            try
            {
                var formatter = new DataContractSerializer(typeof(RemovedVariablesDocument));
                var readObject = formatter.ReadObject(stream) as RemovedVariablesDocument;
                return readObject;
            }
            catch
            {
                return null;
            }
        }

        bool WriteDataStream(Stream ostrm)
        {
            var settings = new XmlWriterSettings
            {
                Encoding = System.Text.Encoding.UTF8,
                Indent = true,
                CloseOutput = true
            };

            using (var writer = XmlDictionaryWriter.Create(ostrm, settings))
            {
                bool bRet = false;
                try
                {
                    var serializer = new DataContractSerializer(typeof(RemovedVariablesDocument));
                    serializer.WriteObject(writer, document);
                    bRet = true;
                }
                finally
                {
                    writer.Close();
                }

                return bRet;
            }
        }
        #endregion
    }
}
