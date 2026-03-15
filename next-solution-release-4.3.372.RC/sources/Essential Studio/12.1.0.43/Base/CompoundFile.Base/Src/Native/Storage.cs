#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
#if DOCIO
namespace Syncfusion.CompoundFile.DocIO.Native
#else
namespace Syncfusion.CompoundFile.XlsIO.Native
#endif
{
    /// <summary>
    /// Compound storage implementation based on standard COM-object.
    /// </summary>
    public class Storage : ICompoundStorage
    {
        #region Constants
        /// <summary>
        /// Default storage name. Used for root storage, others should assign some other value.
        /// </summary>
        private const string DefaultStorageName = "_Root";
        #endregion

        #region Members
        /// <summary>
        /// 
        /// </summary>
        private IStorage m_storage;
        /// <summary>
        /// Collection with storage names.
        /// </summary>
        private List<string> m_arrStorages = new List<string>();
        /// <summary>
        /// Collection with stream names.
        /// </summary>
        private List<string> m_arrStreams = new List<string>();
        /// <summary>
        /// Name of the storage.
        /// </summary>
        private string m_strName;
        #endregion

        #region Properties
        /// <summary>
        /// Returns internal COM storage. This property will be removed after implementing
        /// some document properties reading.
        /// </summary>
        [CLSCompliant(false)]
        public IStorage COMStorage
        {
            get
            {
                return m_storage;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Calculates subItems names.
        /// </summary>
        private void CalculateSubItemsNames()
        {
            m_arrStorages.Clear();
            m_arrStreams.Clear();

            CalculateSubItems(new SubItemNameEventHandler(ByTypeAccumulate_All), null);
        }
        /// <summary>
        /// Adds data.
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <param name="userData">Collection to add.</param>
        private void ByTypeAccumulate_Streams(STATSTG item, object userData)
        {
            if (item.type == STGTY.STGTY_STREAM)
            {
                ((List<string>)userData).Add(item.pwcsName);
            }
        }
        /// <summary>
        /// Adds data as stream type.
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <param name="userData">Collection where adding is.</param>
        private void ByTypeAccumulate_Storages(STATSTG item, object userData)
        {
            if (item.type == STGTY.STGTY_STORAGE)
            {
                ((List<string>)userData).Add(item.pwcsName);
            }
        }
        /// <summary>
        /// Adds data as all type.
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <param name="userData">Collection where adding is.</param>
        private void ByTypeAccumulate_All(STATSTG item, object userData)
        {
            if (item.type == STGTY.STGTY_STREAM)
            {
                if (m_arrStreams != null)
                    m_arrStreams.Add(item.pwcsName);
            }
            else if (item.type == STGTY.STGTY_STORAGE)
            {
                if (m_arrStorages != null)
                    m_arrStorages.Add(item.pwcsName);
            }
        }
        /// <summary>
        /// Calculates subItems.
        /// </summary>
        /// <param name="caller">SubItem event handler.</param>
        /// <param name="userData">User data.</param>
        private void CalculateSubItems(SubItemNameEventHandler caller, object userData)
        {
            if (caller == null)
                throw new ArgumentNullException("caller");

            const string DEF_FAIL = "Stream Enumeration Operation failed";

            IEnumSTATSTG enm = null;

            int error = m_storage.EnumElements(0, IntPtr.Zero, 0, ref enm);

#if ( WINRT || WP )
            if (error != 0)
                throw new Exception(DEF_FAIL+error);

            if (enm == null)
                throw new Exception("Cannot get IEnumSTATSTG interface refernce from storage");

            error = enm.Reset();
            if (error != 0)
                throw new Exception(DEF_FAIL+error);
#else
                    if (error != 0)
                throw new ExternalException(DEF_FAIL, error);

            if (enm == null)
                throw new SystemException("Cannot get IEnumSTATSTG interface refernce from storage");

            error = enm.Reset();
            if (error != 0)
                throw new ExternalException(DEF_FAIL, error);


#endif
            STATSTG item = new STATSTG();
            uint fetch = 0;

            error = enm.Next(1, ref item, ref fetch);

            while (0 == error && 1 == fetch)
            {
                //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "found item with Name : " + item.pwcsName );
                caller(item, userData);
                error = enm.Next(1, ref item, ref fetch);
            }

            if (error > 1 || error < 0)
#if ( WINRT || WP )

                throw new Exception(DEF_FAIL+ error);
#else
            throw new ExternalException(DEF_FAIL, error);
#endif
            // Release interface.
            Marshal.FinalReleaseComObject(enm);
            enm = null;
        }
        /// <summary>
        /// Delegate that represents subItem name event.
        /// </summary>
        private delegate void SubItemNameEventHandler(STATSTG item, object userData);
        #endregion

        #region ICompoundStorage Members
        /// <summary>
        /// Initializes new instance of the class.
        /// </summary>
        /// <param name="fileName">Name of the file to open.</param>
        /// <param name="storageOptions">Storage options.</param>
        public Storage(string fileName, STGM storageOptions)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Initializes new instance of the storage.
        /// </summary>
        /// <param name="root">Root substorage.</param>
        [CLSCompliant(false)]
        public Storage(IStorage root)
            :
          this(root, DefaultStorageName)
        {
        }
        /// <summary>
        /// Initializes new instance of te storage.
        /// </summary>
        /// <param name="root">Root substorage.</param>
        /// <param name="storageName">Name of the storage.</param>
        [CLSCompliant(false)]
        public Storage(IStorage root, string storageName)
        {
            if (root == null)
                throw new ArgumentNullException("root");

            if (storageName != null && storageName.Length != 0)
                m_strName = storageName;

            m_storage = root;
            CalculateSubItemsNames();
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~Storage()
        {
            Dispose();
        }

        /// <summary>
        /// Creates new stream inside this storage.
        /// </summary>
        /// <param name="streamName">Name of the stream to create.</param>
        /// <returns>Created stream object.</returns>
        public CompoundStream CreateStream(string streamName)
        {
            IStream result = null;
            m_storage.CreateStream(streamName, StgStream.DEF_STREAM_CREATE, 0, 0, ref result);
            m_arrStreams.Add(streamName);
            return new NativeStream(result, streamName);
        }
        /// <summary>
        /// Opens existing stream inside this storage.
        /// </summary>
        /// <param name="streamName">Name of the stream to open.</param>
        /// <returns></returns>
        public CompoundStream OpenStream(string streamName)
        {
            IStream stream;
            m_storage.OpenStream(streamName, 0, STGM.STGM_READ | STGM.STGM_SHARE_EXCLUSIVE, 0, out stream);
            return new NativeStream(stream, streamName);
        }
        /// <summary>
        /// Removes existing stream from this storage.
        /// </summary>
        /// <param name="streamName">Name of the stream to remove.</param>
        public void DeleteStream(string streamName)
        {
            if (ContainsStream(streamName))
            {
                m_storage.DestroyElement(streamName);
                m_arrStreams.Remove(streamName);
            }
        }
        /// <summary>
        /// Determines whether storage contains specified stream.
        /// </summary>
        /// <param name="streamName">Name of the stream to check.</param>
        /// <returns>true if storage contains specified stream.</returns>
        public bool ContainsStream(string streamName)
        {
            return m_arrStreams.Contains(streamName);
        }
        /// <summary>
        /// Creates new substorage inside this one.
        /// </summary>
        /// <param name="storageName">Name of the storage to create.</param>
        /// <returns>Created storage object.</returns>
        public ICompoundStorage CreateStorage(string storageName)
        {
            if (storageName == null || storageName.Length == 0)
                throw new ArgumentOutOfRangeException("storageName");

            IStorage storage;
            int iResult = m_storage.CreateStorage(storageName, StgStream.DEF_STREAM_CREATE, 0, 0, out storage);
            if (iResult != 0)
#if ( WINRT || WP )
                throw new Exception("Problems during storage creation"+ iResult);
#else
                throw new ExternalException("Problems during storage creation", iResult);
#endif
            m_arrStorages.Add(storageName);
            return new Storage(storage, storageName);
        }
        /// <summary>
        /// Opens existing substorage inside this one.
        /// </summary>
        /// <param name="storageName">Name of the storage to open.</param>
        /// <returns>Created storage object.</returns>
        public ICompoundStorage OpenStorage(string storageName)
        {
            if (storageName == null || storageName.Length == 0)
                throw new ArgumentOutOfRangeException("storageName");

            IStorage storage;
            int iResult = m_storage.OpenStorage(storageName, IntPtr.Zero, StgStream.DEF_STREAM_READONLY,
                //StgStream.DEF_STORAGE_READONLY, //StgStream.DEF_STORE_READONLY,
              IntPtr.Zero, 0, out storage);

            if (iResult != 0)
#if ( WINRT || WP )
                throw new Exception("Problems during storage creation"+ iResult);
#else
                throw new ExternalException("Problems during storage creation", iResult);
#endif
            return new Storage(storage, storageName);
        }
        /// <summary>
        /// Removes exisiting substorage from this one.
        /// </summary>
        /// <param name="storageName">Name of the storage to remove.</param>
        public void DeleteStorage(string storageName)
        {
            if (ContainsStorage(storageName))
            {
                m_storage.DestroyElement(storageName);
                m_arrStorages.Remove(storageName);
            }
        }
        /// <summary>
        /// Determines whether this storage contains substorage with specified name.
        /// </summary>
        /// <param name="storageName">Name of the storage to check.</param>
        /// <returns>true if storage contains substorage with specified name.</returns>
        public bool ContainsStorage(string storageName)
        {
            return m_arrStorages.Contains(storageName);
        }
        /// <summary>
        /// Commits changes.
        /// </summary>
        public void Flush()
        {
            m_storage.Commit(0);
            //throw new NotImplementedException();
        }
        /// <summary>
        /// Returns all stream names that are placed inside this stream.
        /// </summary>
        public string[] Streams
        {
            get
            {
                return m_arrStreams.ToArray();
            }
        }
        /// <summary>
        /// Returns all storage names that are placed inside this stream.
        /// </summary>
        public string[] Storages
        {
            get
            {
                return m_arrStorages.ToArray();
            }
        }
        /// <summary>
        /// Returns name of the storage.
        /// </summary>
        public string Name
        {
            get
            {
                return m_strName;
            }
        }
        /// <summary>
        /// Inserts copy of the storage and all subitems inside current storage.
        /// </summary>
        /// <param name="storageToCopy">Storage to copy.</param>
        public void InsertCopy(ICompoundStorage storageToCopy)
        {
            Storage sourceStorage = storageToCopy as Storage;

            if (sourceStorage == null)
                throw new NotImplementedException("Copying between different storage types is not implemented");

            string storageName = storageToCopy.Name;
            if (ContainsStorage(storageName))
            {
                DeleteStorage(storageName);
            }

            using (Storage newStorage = CreateStorage(storageName) as Storage)
            {
                sourceStorage.m_storage.CopyTo(0, IntPtr.Zero, IntPtr.Zero, newStorage.m_storage);
            }
        }
        /// <summary>
        /// Inserts copy of the stream inside current storage.
        /// </summary>
        /// <param name="streamToCopy">Stream to copy.</param>
        public void InsertCopy(CompoundStream streamToCopy)
        {
            //IStream stream;
            NativeStream nativeStream = streamToCopy as NativeStream;
            string streamName = streamToCopy.Name;

            using (CompoundStream stream = ContainsStream(streamName) ?
              OpenStream(streamName) :
              CreateStream(streamToCopy.Name))
            {
                stream.SetLength(0);
                streamToCopy.CopyTo(stream);
            }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (m_storage != null)
            {
                Marshal.FinalReleaseComObject(m_storage);
                GC.SuppressFinalize(m_storage);
                m_storage = null;
            }
        }
        #endregion
    }
}
