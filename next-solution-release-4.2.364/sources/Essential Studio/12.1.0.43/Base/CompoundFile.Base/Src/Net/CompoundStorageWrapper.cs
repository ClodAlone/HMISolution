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

#if DOCIO
namespace Syncfusion.CompoundFile.DocIO.Net
#else
namespace Syncfusion.CompoundFile.XlsIO.Net
#endif
{
    /// <summary>
    /// This is wrapper over compound stream object. Simply redirects all calls to it
    /// with one exception - it doesn't dispose underlying stream object.
    /// </summary>
    class CompoundStorageWrapper : ICompoundStorage
    {
        #region Members
        /// <summary>
        /// Wrapped storage object.
        /// </summary>
        private CompoundStorage m_storage;
        #endregion

        #region Methods
        /// <summary>
        /// Initializes new instance of the wrapper.
        /// </summary>
        /// <param name="wrapped">Object to wrap.</param>
        public CompoundStorageWrapper(CompoundStorage wrapped)
        {
            m_storage = wrapped;
        }
        /// <summary>
        /// Frees all allocated resources.
        /// </summary>
        public void Dispose()
        {
            if (m_storage != null)
            {
                //base.Dispose( disposing );

                // NOTE: we don't dispose wrapped item. This should be done by some other object.
                m_storage = null;
                GC.SuppressFinalize(this);
            }
        }
        /// <summary>
        /// Creates new stream inside this storage.
        /// </summary>
        /// <param name="streamName">Name of the stream to create.</param>
        /// <returns>Created stream object.</returns>
        public CompoundStream CreateStream(string streamName)
        {
            return m_storage.CreateStream(streamName);
        }
        /// <summary>
        /// Opens existing stream inside this storage.
        /// </summary>
        /// <param name="streamName">Name of the stream to open.</param>
        /// <returns></returns>
        public CompoundStream OpenStream(string streamName)
        {
            return m_storage.OpenStream(streamName);
        }
        /// <summary>
        /// Removes existing stream from this storage.
        /// </summary>
        /// <param name="streamName">Name of the stream to remove.</param>
        public void DeleteStream(string streamName)
        {
            m_storage.DeleteStream(streamName);
        }
        /// <summary>
        /// Determines whether storage contains specified stream.
        /// </summary>
        /// <param name="streamName">Name of the stream to check.</param>
        /// <returns>true if storage contains specified stream.</returns>
        public bool ContainsStream(string streamName)
        {
            return m_storage.ContainsStream(streamName);
        }
        /// <summary>
        /// Creates new substorage inside this one.
        /// </summary>
        /// <param name="storageName">Name of the storage to create.</param>
        /// <returns>Created storage object.</returns>
        public ICompoundStorage CreateStorage(string storageName)
        {
            return m_storage.CreateStorage(storageName);
        }
        /// <summary>
        /// Opens existing substorage inside this one.
        /// </summary>
        /// <param name="storageName">Name of the storage to open.</param>
        /// <returns>Created storage object.</returns>
        public ICompoundStorage OpenStorage(string storageName)
        {
            return m_storage.OpenStorage(storageName);
        }
        /// <summary>
        /// Removes exisiting substorage from this one.
        /// </summary>
        /// <param name="storageName">Name of the storage to remove.</param>
        public void DeleteStorage(string storageName)
        {
            m_storage.DeleteStorage(storageName);
        }
        /// <summary>
        /// Determines whether this storage contains substorage with specified name.
        /// </summary>
        /// <param name="storageName">Name of the storage to check.</param>
        /// <returns>true if storage contains substorage with specified name.</returns>
        public bool ContainsStorage(string storageName)
        {
            return m_storage.ContainsStorage(storageName);
        }
        /// <summary>
        /// Commits changes.
        /// </summary>
        public void Flush()
        {
            m_storage.Flush();
        }
        /// <summary>
        /// Returns all stream names that are placed inside this stream.
        /// </summary>
        public string[] Streams
        {
            get
            {
                return m_storage.Streams;
            }
        }
        /// <summary>
        /// Returns all storage names that are placed inside this stream.
        /// </summary>
        public string[] Storages
        {
            get
            {
                return m_storage.Storages;
            }
        }
        /// <summary>
        /// Returns name of the storage.
        /// </summary>
        public string Name
        {
            get
            {
                return m_storage.Name;
            }
        }

        public void InsertCopy(ICompoundStorage storageToCopy)
        {
            m_storage.InsertCopy(storageToCopy);
        }
        internal void UpdateStorageGuid(ICompoundStorage storageToCopy)
        {
            m_storage.Entry.StorageGuid = (storageToCopy as CompoundStorageWrapper).m_storage.Entry.StorageGuid;
        }
        public void InsertCopy(CompoundStream streamToCopy)
        {
            m_storage.InsertCopy(streamToCopy);
        }
        #endregion
    }
}
