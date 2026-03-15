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
    /// This is compound file implementation based on standard COM-objects.
    /// </summary>
    public class CompoundFile : ICompoundFile
    {
        #region Members
        /// <summary>
        /// Root storage.
        /// </summary>
        private Storage m_rootStorage;
        /// <summary>
        /// Represents the locking bytes.
        /// </summary>
        private ILockBytes m_lockBytes;
#if DOCIO
        private Syncfusion.CompoundFile.DocIO.Net.Directory m_directory;
#endif
        #endregion

        #region Properties
#if DOCIO
        /// <summary>
        /// Gets the directory
        /// </summary>
        public Syncfusion.CompoundFile.DocIO.Net.Directory Directory
        {
            get
            {
                return m_directory;
            }
        }

#endif
        #endregion

        #region Methods
        /// <summary>
        /// Default constructor. Creates native compound file in memory.
        /// </summary>
        public CompoundFile()
        {
            CreateStorageOnILockBytes();
        }
        /// <summary>
        /// Creates new instance of the compound file based on the specified stream.
        /// </summary>
        /// <param name="stream">Stream to extract data from.</param>
        public CompoundFile(Stream stream)
        {
            Open(stream);
        }
#if !(WINRT || WP )
        /// <summary>
        /// Creates new instance of the compound file based on the file name and open flags.
        /// </summary>
        /// <param name="fileName">Name of the file to parse.</param>
        /// <param name="options">Storage options.</param>
        public CompoundFile(string fileName, STGM options)
        {
            if (fileName == null || fileName.Length == 0)
                throw new ArgumentOutOfRangeException("fileName");

            bool bCreate = (options & STGM.STGM_CREATE) != 0;

            if (!bCreate)
            {
                using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    Open(stream);
                }
            }
            else
            {

                IStorage storage;

                Guid guid = new Guid("0000000b-0000-0000-C000-000000000046");
                int error = API.StgCreateStorageEx(fileName,
                  options, STGFMT.STGFMT_DOCFILE, 0, IntPtr.Zero, IntPtr.Zero, ref guid, out storage);

                if ((uint)error == (uint)STG_ERRORS.STG_E_LOCKVIOLATION
                  || (uint)error == (uint)STG_ERRORS.STG_E_SHAREVIOLATION)
                {
                    throw new LockShareViolationException();
                }

                if (error != 0)
                    throw new ExternalException("Cannot open storage. File Name is: " + fileName, error);

                // Get names of streams.
                //CalculateSubItemsNames();

                //m_strFileName = fileName;
                //m_modeStorage = flags;
                m_rootStorage = new Storage(storage);
            }
        }
#endif
        /// <summary>
        /// Flushes all internal buffers.
        /// </summary>
        public void Flush()
        {
            m_rootStorage.Flush();

            if (m_lockBytes != null)
                m_lockBytes.Flush();
        }
        /// <summary>
        /// Cretes storage on ILockBytes.
        /// </summary>
        /// <returns>Created storage.</returns>
        private void CreateStorageOnILockBytes()
        {
            IStorage storage;

            int error = API.CreateILockBytesOnHGlobal(IntPtr.Zero, true, out m_lockBytes);

            if (error != 0)
#if !(WINRT || WP )
                throw new ExternalException("Can't create LockBytes.", error);
#else
                throw new Exception("Can't create LockBytes." + error);
#endif
            error = API.StgCreateDocfileOnILockBytes(m_lockBytes,
              STGM.STGM_SHARE_EXCLUSIVE | STGM.STGM_CREATE | STGM.STGM_READWRITE, 0, out storage);

            if (error != 0)
#if !(WINRT || WP )
                throw new ExternalException("Can't create storage on ILockBytes.", error);
#else
                throw new Exception("Can't create storage on ILockBytes." + error);
#endif
            m_rootStorage = new Storage(storage);
        }
        /// <summary>
        /// Saves internal ILockBytes into stream.
        /// </summary>
        /// <param name="stream">Stream to save into.</param>
        public void SaveILockBytesIntoStream(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (m_lockBytes == null)
                throw new ArgumentNullException("m_lockBytes");

            const int BufferSize = 32 * 1024;
            byte[] buffer = new byte[BufferSize];
            uint readBytes;

            for (long position = 0; ; position += BufferSize)
            {
                int result = m_lockBytes.ReadAt((ulong)position, buffer, BufferSize, out readBytes);

                if (result != 0)
#if ( WINRT || WP )
                    throw new Exception("Unable to read bytes from ILockBytes" + result);
#else
                    throw new ExternalException("Unable to read bytes from ILockBytes", result);
#endif
                stream.Write(buffer, 0, (int)readBytes);


                if (readBytes < BufferSize)
                    break;
            }
        }
        /// <summary>
        /// Opens specified stream.
        /// </summary>
        /// <param name="stream">Stream to open.</param>
        private void Open(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            int error = API.CreateILockBytesOnHGlobal(IntPtr.Zero, true, out m_lockBytes);
            int iSize = (int)(stream.Length - stream.Position);

            byte[] buffer = new byte[iSize];

            stream.Read(buffer, 0, iSize);
            uint uiWritten;

            m_lockBytes.WriteAt(0, buffer, (uint)buffer.Length,
              out uiWritten);

            m_lockBytes.Flush();

            //error = API.StgCreateDocfileOnILockBytes( m_lockBytes, flags, 0, out m_storage );
            IStorage storage;
            error = API.StgOpenStorageOnILockBytes(m_lockBytes, null, StgStream.DEF_STORAGE_READONLY,
              0, 0, out storage);

            m_rootStorage = new Storage(storage);
        }
        #endregion

        #region ICompoundFile Members
        /// <summary>
        /// Returns root storage object for this file.
        /// </summary>
        public ICompoundStorage RootStorage
        {
            get
            {
                return m_rootStorage;
            }
        }
        /// <summary>
        /// Saves compound file into stream
        /// </summary>
        /// <param name="stream">Stream to save data into.</param>
        public void Save(System.IO.Stream stream)
        {
            Flush();

            if (m_lockBytes != null)
            {
                SaveILockBytesIntoStream(stream);
            }
            else
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }
#if !(WINRT || WP )
        /// <summary>
        /// Saves compound file into file.
        /// </summary>
        /// <param name="fileName">Name of the file to save into.</param>
        public void Save(string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                Save(stream);
            }
        }
#endif
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Frees all allocated resources.
        /// </summary>
        public void Dispose()
        {
            if (m_rootStorage != null)
            {
                m_rootStorage.Dispose();
                m_rootStorage = null;

                if (m_lockBytes != null)
                {
                    Marshal.FinalReleaseComObject(m_lockBytes);
                    GC.SuppressFinalize(m_lockBytes);
                    m_lockBytes = null;
                }

                GC.SuppressFinalize(this);
            }
        }
        #endregion
    }
}
