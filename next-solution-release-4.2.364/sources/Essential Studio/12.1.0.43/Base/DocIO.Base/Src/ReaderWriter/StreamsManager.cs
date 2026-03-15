#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;
using System.IO;

using Syncfusion.CompoundFile.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.CompoundFile.DocIO.Native;
using Syncfusion.CompoundFile.DocIO.Net;
using System.Collections.Generic;
#endregion

namespace Syncfusion.DocIO.ReaderWriter
{
    /// <summary>
    /// 
    /// </summary>
    internal class StreamsManager
    {
        #region Constants
        public const string MacrosStorageName = "Macros";
        public const string ObjectPoolStorageName = "ObjectPool";
        //public const string MsoDataStoreName = "MsoDataStore";

        /// <summary>
        /// 
        /// </summary>
#if !SILVERLIGHT && !WP
        private const STGM c_flagsDenyWrite = STGM.STGM_READ | STGM.STGM_SHARE_DENY_WRITE;
        private const STGM c_flagsReadExclusive = STGM.STGM_READ | STGM.STGM_SHARE_EXCLUSIVE;
        private const STGM c_flagsReadOnly = STGM.STGM_DIRECT_SWMR | STGM.STGM_SHARE_DENY_NONE | STGM.STGM_READ;
        private const STGM c_flagsReadWriteExclusive = STGM.STGM_READ | STGM.STGM_READWRITE | STGM.STGM_SHARE_EXCLUSIVE;
#endif
        private const string c_mainStream = "WordDocument";
        private const string c_dataStream = "Data";
        private const string c_tableStream = "1Table";
        private const string c_summaryInfoStream = "\x5SummaryInformation";
        private const string c_documentSummaryInfoStream = "\x0005DocumentSummaryInformation";
        private byte[] m_compObjData = new byte[121] 
        {
      0x01, 0x00, 0xFE, 0xFF, 0x03, 0x0A, 0x00, 0x00, 0xFF, 
      0xFF, 0xFF, 0xFF, 0x06, 0x09, 0x02, 0x00, 0x00, 0x00, 
      0x00, 0x00, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
      0x46, 0x27, 0x00, 0x00, 0x00, 0x4D, 0x69, 0x63, 0x72, 
      0x6F, 0x73, 0x6F, 0x66, 0x74, 0x20, 0x4F, 0x66, 0x66,
      0x69, 0x63, 0x65, 0x20, 0x57, 0x6F, 0x72, 0x64, 0x20, 
      0x39, 0x37, 0x2D, 0x32, 0x30, 0x30, 0x33, 0x20, 0x44, 
      0x6F, 0x63, 0x75, 0x6D, 0x65, 0x6E, 0x74, 0x00, 0x0A, 
      0x00, 0x00, 0x00, 0x4D, 0x53, 0x57, 0x6F, 0x72, 0x64, 
      0x44, 0x6F, 0x63, 0x00, 0x10, 0x00, 0x00, 0x00, 0x57, 
      0x6F, 0x72, 0x64, 0x2E, 0x44, 0x6F, 0x63, 0x75, 0x6D, 
      0x65, 0x6E, 0x74, 0x2E, 0x38, 0x00, 0xF4, 0x39, 0xB2, 
      0x71, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
      0x00, 0x00, 0x00, 0x00 
        };
        #endregion

        #region Fields
        private string m_fileName = null;
        private Stream m_outStream = null;
#if !SILVERLIGHT && !WP
        private StgStream m_stgStream;
#endif
        private ICompoundFile m_compoundFile;
        private MemoryStream m_mainStream;
        private MemoryStream m_tableStream;
        private MemoryStream m_dataStream = null;
        private MemoryStream m_macrosStream = null;
        private MemoryStream m_summaryInfoStream = null;
        private MemoryStream m_documentSummaryInfoStream = null;
        private MemoryStream m_objectPoolStream = null;
        //private MemoryStream m_msoDataStore = null;
        private BinaryWriter m_mainWriter = null;
        private BinaryWriter m_tableWriter = null;
        private BinaryWriter m_dataWriter = null;
        private BinaryWriter m_summaryInfoWriter = null;
        private BinaryWriter m_documentSummaryInfoWriter = null;
        private BinaryReader m_mainReader = null;
        private BinaryReader m_tableReader = null;
        private BinaryReader m_dataReader = null;
        private BinaryReader m_summaryInfoReader = null;
        private BinaryReader m_documentSummaryInfoReader = null;
        private bool m_bNetStorage =
#if AllowUnsafeCode
#if !SILVERLIGHT && !WP
 false;
#else
 true;
#endif
#else
 true;
#endif
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamsManager"/> class.
        /// </summary>
        /// <param name="fileName"> Name of the file. </param>
        /// <param name="createNewStorage"> True - create new storage,
        /// false - load existing storage. </param>
        internal StreamsManager(string fileName, bool createNewStorage)
        {
            if (createNewStorage)
            {
                InitStreams();
                m_fileName = fileName;
                if (m_bNetStorage)
                    m_compoundFile = new Syncfusion.CompoundFile.DocIO.Net.CompoundFile() as ICompoundFile;
#if !SILVERLIGHT && !WP
                else
                    m_stgStream = StgStream.CreateStorage(fileName);
#endif
            }
            else
            {
                LoadStg(fileName);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamsManager"/> class.
        /// </summary>
        /// <param name="stream"> Name of the stream. </param>
        /// <param name="createNewStorage"> True - create new storage,
        /// false - load existing storage. </param>
        internal StreamsManager(Stream stream, bool createNewStorage)
        {
            if (createNewStorage)
            {
                InitStreams();
                m_outStream = stream;
                if (m_bNetStorage)
                    m_compoundFile = new Syncfusion.CompoundFile.DocIO.Net.CompoundFile() as ICompoundFile;
#if !SILVERLIGHT && !WP
                else
                    m_stgStream = StgStream.CreateStorageOnILockBytes();
#endif
            }
            else
            {
                LoadStg(stream);
            }
        }
        #endregion

        #region Properties
        internal ICompoundFile CompoundFile
        {
            get
            {
                return m_compoundFile;
            }
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Gets the storage.
        /// </summary>
        /// <value>The storage.</value>
        internal StgStream Storage
        {
            get
            {
                return m_stgStream;
            }
        }
#endif

        /// <summary>
        /// Gets main, table and data streams.
        /// </summary>
        internal MemoryStream MainStream
        {
            get
            {
                return m_mainStream;
            }
        }

        internal MemoryStream TableStream
        {
            get
            {
                return m_tableStream;
            }
        }

        internal MemoryStream DataStream
        {
            get
            {
                return m_dataStream;
            }
        }

        /// <summary>
        /// Gets macros and object pool streams
        /// </summary>
        internal MemoryStream MacrosStream
        {
            get
            {
                return m_macrosStream;
            }

            set
            {
                m_macrosStream = value;
            }
        }

        internal MemoryStream ObjectPoolStream
        {
            get
            {
                return m_objectPoolStream;
            }

            set
            {
                m_objectPoolStream = value;
            }
        }
        /// <summary>
        /// Gets or sets the SummaryInformation stream
        /// </summary>
        internal MemoryStream SummaryInfoStream
        {
            get
            {
                return m_summaryInfoStream;
            }
            set
            {
                m_summaryInfoStream = value;
            }
        }
        /// <summary>
        /// Gets or Sets the DocumentSummaryInformation stream
        /// </summary>
        internal MemoryStream DocumentSummaryInfoStream
        {
            get
            {
                return m_documentSummaryInfoStream;
            }
            set
            {
                m_documentSummaryInfoStream = value;
            }
        }
        ///// <summary>
        ///// 
        ///// </summary>
        //internal MemoryStream MsoDataStore
        //{
        //    get
        //    {
        //        return m_msoDataStore;
        //    }
        //    set
        //    {
        //        m_msoDataStore = value;
        //    }
        //}
        /// <summary>
        /// Gets or sets the SummaryInformation stream writer
        /// </summary>
        internal BinaryWriter SummaryInfoWriter
        {
            get
            {
                return m_summaryInfoWriter;
            }
            set
            {
                m_summaryInfoWriter = value;
            }
        }
        /// <summary>
        /// Gets or sets the DocumentSummaryInformation stream writer
        /// </summary>
        internal BinaryWriter DocumentSummaryInfoWriter
        {
            get
            {
                return m_documentSummaryInfoWriter;
            }
            set
            {
                m_documentSummaryInfoWriter = value;
            }
        }
        /// <summary>
        /// Gets or sets the SummaryInformation stream reader
        /// </summary>
        internal BinaryReader SummaryInfoReader
        {
            get
            {
                return m_summaryInfoReader;
            }
            set
            {
                m_summaryInfoReader = value;
            }
        }
        /// <summary>
        /// Gets or sets the DocumentSummaryInformation stream reader
        /// </summary>
        internal BinaryReader DocumentSummaryInfoReader
        {
            get
            {
                return m_documentSummaryInfoReader;
            }
            set
            {
                m_documentSummaryInfoReader = value;
            }
        }
        /// <summary>
        /// Gets the main, table and data writers.
        /// </summary>
        internal BinaryWriter MainWriter
        {
            get
            {
                return m_mainWriter;
            }
        }

        internal BinaryWriter TableWriter
        {
            get
            {
                return m_tableWriter;
            }
        }

        internal BinaryWriter DataWriter
        {
            get
            {
                return m_dataWriter;
            }
        }

        /// <summary>
        /// Gets the main, table and data readers.
        /// </summary>
        internal BinaryReader MainReader
        {
            get
            {
                return m_mainReader;
            }
        }

        internal BinaryReader TableReader
        {
            get
            {
                return m_tableReader;
            }
        }

        internal BinaryReader DataReader
        {
            get
            {
                return m_dataReader;
            }
        }
        #endregion

        #region Internal methods
        /// <summary>
        /// <summary>
        /// Loads the STG from file
        /// </summary>
        /// <param name="fileName"> File name </param>
#if WINRT
        internal async void LoadStg(string fileName)
        {
            if (m_bNetStorage)
            {
                m_compoundFile = new Syncfusion.CompoundFile.DocIO.Net.CompoundFile(fileName, false) as ICompoundFile;
                await (m_compoundFile as Syncfusion.CompoundFile.DocIO.Net.CompoundFile).Initialize(fileName, false);
            }
#else
        internal void LoadStg(string fileName)
        {
             if (m_bNetStorage)
                m_compoundFile = new Syncfusion.CompoundFile.DocIO.Net.CompoundFile( fileName, false ) as ICompoundFile;
#endif

#if !SILVERLIGHT && !WP
            else
                m_stgStream = new StgStream(fileName, c_flagsReadOnly);
#endif

            LoadStreams();
        }

        /// <summary>
        /// Loads the STG from stream
        /// </summary>
        /// <param name="stream"></param>
        internal void LoadStg(Stream stream)
        {
            if (m_bNetStorage)
                m_compoundFile = new Syncfusion.CompoundFile.DocIO.Net.CompoundFile(stream) as ICompoundFile;
#if !SILVERLIGHT && !WP
            else
                m_stgStream = new StgStream(stream, c_flagsReadOnly);
#endif
            LoadStreams();
        }

        /// <summary>
        /// Loads the table stream.
        /// </summary>
        /// <param name="tableStreamName"> Table stream name </param>
        internal void LoadTableStream(string tableStreamName)
        {
            if (m_bNetStorage)
                m_tableStream = LoadStreamFromCompound(tableStreamName);
#if !SILVERLIGHT && !WP
            else
                m_tableStream = LoadStreamFromStg(tableStreamName);
#endif

            m_tableReader = new BinaryReader(m_tableStream);
        }
        /// <summary>
        /// Loads the SummaryInformation stream
        /// </summary>
        internal void LoadSummaryInfoStream()
        {
            if (m_bNetStorage)
            {
                if (m_compoundFile.RootStorage.ContainsStream(c_summaryInfoStream))
                    m_summaryInfoStream = LoadStreamFromCompound(c_summaryInfoStream);
            }
#if !SILVERLIGHT && !WP
            else
            {
                if (m_stgStream.ContainsStream(c_summaryInfoStream))
                    m_summaryInfoStream = LoadStreamFromStg(c_summaryInfoStream);
            }
#endif
            if (m_summaryInfoStream != null)
                m_summaryInfoReader = new BinaryReader(m_summaryInfoStream);
        }
        /// <summary>
        /// Loads the DocumentSummaryInformation stream
        /// </summary>
        internal void LoadDocumentSummaryInfoStream()
        {
            if (m_bNetStorage)
            {
                if (m_compoundFile.RootStorage.ContainsStream(c_documentSummaryInfoStream))
                    m_documentSummaryInfoStream = LoadStreamFromCompound(c_documentSummaryInfoStream);
            }
#if !SILVERLIGHT && !WP
            else
            {
                if (m_stgStream.ContainsStream(c_documentSummaryInfoStream))
                    m_documentSummaryInfoStream = LoadStreamFromStg(c_documentSummaryInfoStream);
            }
#endif
            if (m_documentSummaryInfoStream != null)
                m_documentSummaryInfoReader = new BinaryReader(m_documentSummaryInfoStream);
        }
        /// <summary>
        /// Refreshs main and table streams data
        /// </summary>
        /// <param name="mainStream"> main stream from which to refresh current main stream </param>
        /// <param name="tableStream"> table stream from which to refresh current table stream </param>
        internal void UpdateStreams(MemoryStream mainStream, MemoryStream tableStream, MemoryStream dataStream)
        {
            m_mainStream = mainStream;
            m_tableStream = tableStream;
            m_dataStream = dataStream;
            //Handled to reinitialize the data reader with decrypted data stream.
            if (m_dataStream != null)
                m_dataReader = new BinaryReader(m_dataStream);
        }

        /// <summary>
        /// Writes the sub storage.
        /// </summary>
        /// <param name="stream"> The stream </param>
        /// <param name="storageName"> Name of the storage. </param>
        internal void WriteSubStorage(MemoryStream stream, string storageName)
        {
#if SILVERLIGHT || WP
            Syncfusion.CompoundFile.DocIO.Net.CompoundFile cmpFile = new Syncfusion.CompoundFile.DocIO.Net.CompoundFile(stream);
            ICompoundStorage storage = cmpFile.RootStorage.OpenStorage(storageName);
            m_compoundFile.RootStorage.InsertCopy(storage);
            cmpFile.Dispose();
            stream.Dispose();
#else
            if (!m_bNetStorage)
            {
                stream.Position = 0;
                StgStream tmpStg = new StgStream(stream);
                StgStream tmpSubStg = tmpStg.OpenSubStorage(storageName);
                StgStream.CopySourceStorages(tmpSubStg, m_stgStream);

                tmpStg.Dispose();
                tmpStg = null;
                tmpSubStg.Dispose();
                tmpSubStg = null;
            }
#endif
        }

        /// <summary>
        /// Saves storages data and closes streams
        /// </summary>
        internal void SaveStg()
        {
            SaveStream(c_mainStream, m_mainStream);
            SaveStream(c_tableStream, m_tableStream);

            SaveCompObjStream();

            if (m_dataStream.Length != 0)
            {
                SaveStream(c_dataStream, m_dataStream);
            }

            if (m_macrosStream != null)
            {
                WriteSubStorage(m_macrosStream, MacrosStorageName);
            }

            if (m_objectPoolStream != null)
            {
                WriteSubStorage(m_objectPoolStream, ObjectPoolStorageName);
            }

            if (m_summaryInfoStream != null && m_summaryInfoStream.Length != 0)
            {
                SaveStream(c_summaryInfoStream, m_summaryInfoStream);
            }

            if (m_documentSummaryInfoStream != null && m_documentSummaryInfoStream.Length != 0)
            {
                SaveStream(c_documentSummaryInfoStream, m_documentSummaryInfoStream);
            }

            //if (m_msoDataStore != null)
            //{
            //    WriteSubStorage(m_msoDataStore, "MsoDataStoreName");
            //}

            if (m_outStream != null)
            {
                if (m_bNetStorage)
                {
                    m_compoundFile.Save(m_outStream);
                    m_outStream.Flush();
                }
#if !SILVERLIGHT && !WP
                else
                {
                    m_stgStream.Flush();
                    m_stgStream.SaveILockBytesIntoStream(m_outStream);
                    m_outStream.Flush();
                }
#endif
            }

            CloseStg();
        }

        /// <summary>
        ///  Closes all streams and binary readers/writers
        /// </summary>
        internal void CloseStg()
        {
            if (m_bNetStorage)
            {
                m_compoundFile.RootStorage.Dispose();
                m_compoundFile = null;
            }
#if !SILVERLIGHT && !WP
            else
            {
                m_stgStream.Dispose();
                m_stgStream = null;
            }
#endif
#if WINRT
            if (m_mainStream != null)
            {
                m_mainStream.Dispose();
                m_mainStream = null;
            }
            if (m_tableStream != null)
            {
                m_tableStream.Dispose();
                m_tableStream = null;
            }
            if (m_dataStream != null)
            {
                m_dataStream.Dispose();
                m_dataStream = null;
            }
            if (m_macrosStream != null)
            {
                m_macrosStream.Dispose();
                m_macrosStream = null;
            }
            if (m_objectPoolStream != null)
            {
                m_objectPoolStream.Dispose();
                m_objectPoolStream = null;
            }
            if (m_summaryInfoStream != null)
            {
                m_summaryInfoStream.Dispose();
                m_summaryInfoStream = null;
            }
            if (m_documentSummaryInfoStream != null)
            {
                m_documentSummaryInfoStream.Dispose();
                m_documentSummaryInfoStream = null;
            }
#else
            if (m_mainStream != null)
            {
                m_mainStream.Close();
                m_mainStream = null;
            }
            if (m_tableStream != null)
            {
                m_tableStream.Close();
                m_tableStream = null;
            }
            if (m_dataStream != null)
            {
                m_dataStream.Close();
                m_dataStream = null;
            }
            if (m_macrosStream != null)
            {
                m_macrosStream.Close();
                m_macrosStream = null;
            }
            if (m_objectPoolStream != null)
            {
                m_objectPoolStream.Close();
                m_objectPoolStream = null;
            }
            if (m_summaryInfoStream != null)
            {
                m_summaryInfoStream.Close();
                m_summaryInfoStream  = null;
            }
            if (m_documentSummaryInfoStream != null)
            {
                m_documentSummaryInfoStream.Close();
                m_documentSummaryInfoStream = null;
            }
#endif

            m_mainWriter = null;
            m_tableWriter = null;
            m_dataWriter = null;
            m_summaryInfoWriter = null;
            m_documentSummaryInfoWriter = null;
            m_mainReader = null;
            m_tableReader = null;
            m_dataReader = null;
            m_summaryInfoReader = null;
            m_documentSummaryInfoReader = null;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Saves the stream.
        /// </summary>
        /// <param name="name"> The stream name. </param>
        /// <param name="stream"> The stream. </param>
        private void SaveStream(string name, MemoryStream stream)
        {
            if (m_bNetStorage)
            {
                using (CompoundStream compStream = m_compoundFile.RootStorage.CreateStream(name))
                {
#if WINRT
                    byte[] buffer = stream.ToArray();
                    compStream.Write(buffer, 0, (int)buffer.Length);
#else
                    compStream.Write(stream.GetBuffer(), 0, (int)stream.Length);
#endif
                }
            }
#if !SILVERLIGHT && !WP
            else
            {
                m_stgStream.CreateStream(name);
                m_stgStream.Write(stream.GetBuffer(), 0, (int)stream.Length);
                m_stgStream.Close();
            }
#endif
        }

        /// <summary>
        /// Saves the stream.
        /// </summary>
        /// <param name="name"> The stream name. </param>
        /// <param name="stream"> The stream. </param>
        private void SaveCompObjStream()
        {
            if (m_bNetStorage)
            {
                using (CompoundStream compStream = m_compoundFile.RootStorage.CreateStream("CompObj"))
                {
                    compStream.Write(m_compObjData, 0, m_compObjData.Length);
                }
            }
#if !SILVERLIGHT && !WP
            else
            {
                m_stgStream.CreateStream("CompObj");
                m_stgStream.Write(m_compObjData, 0, m_compObjData.Length);
                m_stgStream.Close();
            }
#endif
        }

        /// <summary>
        /// Initialize the streams and writers.
        /// </summary>
        private void InitStreams()
        {
            m_mainStream = new MemoryStream(4095);
            m_mainWriter = new BinaryWriter(m_mainStream);
            m_tableStream = new MemoryStream(4095);
            m_tableWriter = new BinaryWriter(m_mainStream);
            m_dataStream = new MemoryStream();
            m_dataWriter = new BinaryWriter(m_dataStream);
            m_documentSummaryInfoStream = new MemoryStream();
            m_documentSummaryInfoWriter = new BinaryWriter(m_documentSummaryInfoStream);
            m_summaryInfoStream = new MemoryStream();
            m_summaryInfoWriter = new BinaryWriter(m_summaryInfoStream);
        }

        /// <summary>
        /// Loads main, data, macros and object pool streams data
        /// </summary>
        private void LoadStreams()
        {
            if (m_bNetStorage)
            {
                m_mainStream = LoadStreamFromCompound(c_mainStream);

                if (m_compoundFile.RootStorage.ContainsStream(c_dataStream))
                {
                    m_dataStream = LoadStreamFromCompound(c_dataStream);
                }

                // Read document Macros and Object Pool
                m_macrosStream = ReadSubStorage(MacrosStorageName);
                m_objectPoolStream = ReadSubStorage(ObjectPoolStorageName);
            }
#if !SILVERLIGHT && !WP
            else
            {
                m_mainStream = LoadStreamFromStg(c_mainStream);

                if (m_stgStream.ContainsStream(c_dataStream))
                {
                    m_dataStream = LoadStreamFromStg(c_dataStream);
                }

                // Read document Macros and Object Pool
                m_macrosStream = ReadSubStorage(MacrosStorageName);
                m_objectPoolStream = ReadSubStorage(ObjectPoolStorageName);
                //m_msoDataStore = ReadSubStorage("MsoDataStoreName");
            }
#endif

            m_mainReader = new BinaryReader(m_mainStream);

            if (m_dataStream != null)
                m_dataReader = new BinaryReader(m_dataStream);
        }

        /// <summary>
        /// Loads the stream from compound file.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private MemoryStream LoadStreamFromCompound(string name)
        {
            byte[] buffer = null;
            using (CompoundStream compStream = m_compoundFile.RootStorage.OpenStream(name))
            {
                int length = (int)compStream.Length;
                buffer = new byte[length];
                compStream.Read(buffer, 0, length);
            }

            return new MemoryStream(buffer);
        }

        /// <summary>
        /// Loads the sub storage.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private MemoryStream LoadSubStorage(string name)
        {
            ////if( m_compoundFile.RootStorage.ContainsStorage( name ) )
            ////{
            ////  byte buffer = null;
            ////  using( ICompoundStorage subStore = m_compoundFile.RootStorage.OpenStorage( name ) )
            ////  { 
            ////    subStore.
            ////    buffer = new byte[ subStore.]
            ////  }
            ////}
            return null;
        }
#if SILVERLIGHT || WP
        /// <summary>
        /// Reads some sub storage from the main storage of the document
        ///   into the memory stream
        /// </summary>
        /// <param name="stgName"> sub storage name </param>
        /// <returns></returns>
        private MemoryStream ReadSubStorage(string stgName)
        {
            if (m_compoundFile.RootStorage.ContainsStorage(stgName))
            {
                Syncfusion.CompoundFile.DocIO.ICompoundStorage tempStorage = m_compoundFile.RootStorage.OpenStorage(stgName);
                Syncfusion.CompoundFile.DocIO.Net.CompoundFile destinationFile = new Syncfusion.CompoundFile.DocIO.Net.CompoundFile();
                destinationFile.RootStorage.InsertCopy(tempStorage);
                destinationFile.Flush();
                MemoryStream memStream = new MemoryStream();
                destinationFile.BaseStream.CopyTo(memStream);
                memStream.Position = 0;
                destinationFile.Dispose();
                return memStream;
            }
            return null;
        }
#else
        /// <summary>
        /// Reads some sub storage from the main storage of the document
        ///   into the memory stream
        /// </summary>
        /// <param name="stgName"> sub storage name </param>
        /// <returns></returns>
        private MemoryStream ReadSubStorage(string stgName)
        {
            if (m_bNetStorage)
            {
                if (m_compoundFile.RootStorage.ContainsStorage(stgName))
                {
                    Syncfusion.CompoundFile.DocIO.ICompoundStorage tempStorage = m_compoundFile.RootStorage.OpenStorage(stgName);
                    Syncfusion.CompoundFile.DocIO.Net.CompoundFile destinationFile = new Syncfusion.CompoundFile.DocIO.Net.CompoundFile();
                    destinationFile.RootStorage.InsertCopy(tempStorage);
                    destinationFile.Flush();
                    MemoryStream memStream = CopyStream(destinationFile.BaseStream);
                    memStream.Position = 0;
                    destinationFile.Dispose();
                    return memStream;
                }
            }
            else
            {
                if (m_stgStream.ContainsStorage(stgName))
                {
                    StgStream subStg = m_stgStream.OpenSubStorage(stgName);

                    StgStream destStream = StgStream.CreateStorageOnILockBytes();
                    StgStream.CopySourceStorages(subStg, destStream);

                    MemoryStream memStream = new MemoryStream();
                    destStream.SaveILockBytesIntoStream(memStream);

                    destStream.Close();
                    destStream.Dispose();
                    subStg.Close();
                    subStg.Dispose();
                    return memStream;
                }
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        private MemoryStream CopyStream(Stream inputStream)
        {
            try
            {
                byte[] buf = new byte[inputStream.Length];
                long pos = inputStream.Position;
                inputStream.Position = 0;

                inputStream.Read(buf, 0, buf.Length);
                inputStream.Position = pos;
                return new MemoryStream(buf);
            }
            catch
            {
                throw new ArgumentException("Cannot read data from stream ");
            }
        }
        /// <summary>
        /// Loads stream from storage into memory stream
        /// </summary>
        /// <param name="streamName"> the name of the stream </param>
        /// <returns></returns>
        private MemoryStream LoadStreamFromStg(string streamName)
        {
            try
            {
                m_stgStream.OpenStream(streamName, c_flagsReadExclusive);
                Stream entryStream = m_stgStream;

                byte[] buf = new byte[entryStream.Length];
                long pos = entryStream.Position;
                entryStream.Position = 0;

                entryStream.Read(buf, 0, buf.Length);
                entryStream.Position = pos;
                return new MemoryStream(buf);
            }
            catch
            {
                throw new ArgumentException("Cannot read data from stream ");
            }
            finally
            {
                m_stgStream.Close();
            }
        }
#endif
        #endregion
    }
}