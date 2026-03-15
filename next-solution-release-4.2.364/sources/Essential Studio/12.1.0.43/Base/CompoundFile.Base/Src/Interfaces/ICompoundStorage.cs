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
namespace Syncfusion.CompoundFile.DocIO
#else
namespace Syncfusion.CompoundFile.XlsIO
#endif
{
    /// <summary>
    /// This interface represents storage object in the compound file.
    /// </summary>
    public interface ICompoundStorage : IDisposable
    {
        /// <summary>
        /// Creates new stream inside this storage.
        /// </summary>
        /// <param name="streamName">Name of the stream to create.</param>
        /// <returns>Created stream object.</returns>
        CompoundStream CreateStream(string streamName);
        /// <summary>
        /// Opens existing stream inside this storage.
        /// </summary>
        /// <param name="streamName">Name of the stream to open.</param>
        /// <returns></returns>
        CompoundStream OpenStream(string streamName);
        /// <summary>
        /// Removes existing stream from this storage.
        /// </summary>
        /// <param name="streamName">Name of the stream to remove.</param>
        void DeleteStream(string streamName);
        /// <summary>
        /// Determines whether storage contains specified stream.
        /// </summary>
        /// <param name="streamName">Name of the stream to check.</param>
        /// <returns>true if storage contains specified stream.</returns>
        bool ContainsStream(string streamName);
        /// <summary>
        /// Creates new substorage inside this one.
        /// </summary>
        /// <param name="storageName">Name of the storage to create.</param>
        /// <returns>Created storage object.</returns>
        ICompoundStorage CreateStorage(string storageName);
        /// <summary>
        /// Opens existing substorage inside this one.
        /// </summary>
        /// <param name="storageName">Name of the storage to open.</param>
        /// <returns>Created storage object.</returns>
        ICompoundStorage OpenStorage(string storageName);
        /// <summary>
        /// Removes exisiting substorage from this one.
        /// </summary>
        /// <param name="storageName">Name of the storage to remove.</param>
        void DeleteStorage(string storageName);
        /// <summary>
        /// Determines whether this storage contains substorage with specified name.
        /// </summary>
        /// <param name="storageName">Name of the storage to check.</param>
        /// <returns>true if storage contains substorage with specified name.</returns>
        bool ContainsStorage(string storageName);
        /// <summary>
        /// Commits changes.
        /// </summary>
        void Flush();
        /// <summary>
        /// Returns all stream names that are placed inside this stream.
        /// </summary>
        string[] Streams { get; }
        /// <summary>
        /// Returns all storage names that are placed inside this stream.
        /// </summary>
        string[] Storages { get; }
        /// <summary>
        /// Returns name of the storage.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Inserts copy of the storage and all subitems inside current storage.
        /// </summary>
        /// <param name="storageToCopy">Storage to copy.</param>
        void InsertCopy(ICompoundStorage storageToCopy);
        /// <summary>
        /// Inserts copy of the stream inside current storage.
        /// </summary>
        /// <param name="streamToCopy">Stream to copy.</param>
        void InsertCopy(CompoundStream streamToCopy);
    }
}
