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

using System;
using System.Collections.Generic;
using System.IO;

using Syncfusion.Pdf.Primitives;
using Syncfusion.Pdf.Security;

namespace Syncfusion.Pdf.IO
{
    /// <summary>
    /// CrossTable is a class that performs low-level I/O
    /// for parsing a PDF cross reference table.
    /// </summary>
#if NETFX_CORE || WP
    public class CrossTable
#else
    internal class CrossTable
#endif
    {
        #region Fields
        /// <summary>
        /// Holds the stream.
        /// </summary>
        private Stream m_stream;
        /// <summary>
        /// Holds the stream reader.
        /// </summary>
        private PdfReader m_reader;
        /// <summary>
        /// The parser of the reader.
        /// </summary>
        private PdfParser m_parser;
        /// <summary>
        /// A container of all object offsets.
        /// </summary>
        internal Dictionary<long, ObjectInformation> m_objects;
        /// <summary>
        /// The document trailer.
        /// </summary>
        private PdfDictionary m_trailer;
        /// <summary>
        /// A chache variable for chaching the document catalog offset.
        /// </summary>
        private PdfReferenceHolder m_documentCatalog;
        /// <summary>
        /// The last cross-reference table.
        /// </summary>
        private long m_startXRef;
        /// <summary>
        /// The storage for stream object readers.
        /// </summary>
        private Dictionary<PdfStream, PdfParser> m_readersTable = new Dictionary<PdfStream, PdfParser>();
        /// <summary>
        /// The table of the archives within the document.
        /// </summary>
        private Dictionary<long, PdfStream> m_archives = new Dictionary<long, PdfStream>();
#if !SILVERLIGHT && !WP
        /// <summary>
        /// The document encryptor.
        /// </summary>
        private PdfEncryptor m_encryptor;
#endif
        /// <summary>
        /// The high level cross-table. It's required for convertion
        /// PDFReferences into PDFReferenceHolders.
        /// </summary>
        private PdfCrossTable m_crossTable;
        /// <summary>
        // Read the initial number of the subsection.
        /// </summary>
        internal long m_initialNumberOfSubsection;
        /// <summary>
        // Read the total number of the subsection.
        /// </summary>
        internal long m_totalNumberOfSubsection;
        /// <summary>
        // Check whether the PDF document objects are altered
        /// </summary>
        private bool m_isStructureAltered = false;

        private const int m_generationNumber = 65535;
        #endregion

        #region Properties
        /// <summary>
        /// Returns the offset of an obkect specified.
        /// </summary>
        internal ObjectInformation this[long index]
        {
            get
            {
                object obj = m_objects.ContainsKey(index) ? m_objects[index] : null;

                ObjectInformation result = (obj != null)
                    ? obj as ObjectInformation
                    : null;

                return result;
            }
        }

        /// <summary>
        /// Returns the number of the objects which have been found.
        /// </summary>
        public long Count
        {
            get
            {
                return m_objects.Count;
            }
        }
        /// <summary>
        /// Gets the document catalog address.
        /// </summary>
        public PdfReferenceHolder DocumentCatalog
        {
            get
            {
                if (m_documentCatalog == null)
                {
                    PdfDictionary trailer = Trailer;

                    IPdfPrimitive obj = trailer[DictionaryProperties.Root];

                    if (obj is PdfReferenceHolder)
                    {
                        m_documentCatalog = obj as PdfReferenceHolder;
                    }
                    else
                    {
                        throw new PdfDocumentException(PdfMessages.InvalidFormat);
                    }
                }
                return m_documentCatalog;
            }
        }

        /// <summary>
        /// Returns the stream.
        /// </summary>
        /// <remarks>Use with caution.</remarks>
        internal Stream Stream
        {
            get
            {
                return m_stream;
            }
        }
        /// <summary>
        /// Returns the offset of the last cross-reference table.
        /// </summary>
        internal long XRefOffset
        {
            get
            {
                return m_startXRef;
            }
        }
        /// <summary>
        /// Returns the steream reader.
        /// </summary>
        public PdfReader Reader
        {
            get
            {
                if (m_reader == null)
                {
                    m_reader = new PdfReader(m_stream);
                }

                return m_reader;
            }
        }
        /// <summary>
        /// Gets the parser.
        /// </summary>
        /// <value>The parser.</value>
        public PdfParser Parser
        {
            get
            {
                if (m_parser == null)
                {
                    m_parser = new PdfParser(this, Reader, m_crossTable);
                }

                return m_parser;
            }
        }
        /// <summary>
        /// Returns the document's trailer.
        /// </summary>
        internal PdfDictionary Trailer
        {
            get
            {
                return m_trailer;
            }
        }
        /// <summary>
        // Check whether the PDF document objects are altered
        /// </summary>
        internal bool IsStructureAltered
        {
            get
            {
                return m_isStructureAltered;
            }
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Gets or sets the encryptor.
        /// </summary>
        internal PdfEncryptor Encryptor
        {
            get
            {
                return m_encryptor;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("m_encryptor");

                m_encryptor = value;
            }
        }
#endif
        #endregion

        #region Constructors
        /// <summary>
        /// Searches for all instances of reference tables and parses them.
        /// </summary>
        /// <param name="docStream">The stream with the document.</param>
        /// <param name="crossTable">The high-level cross table.</param>
        public CrossTable(Stream docStream, PdfCrossTable crossTable)
        {
            if (docStream == null)
                throw new ArgumentNullException("docStream");

            if (!docStream.CanSeek || !docStream.CanRead)
            {
                throw new PdfDocumentException("Ivalid stream.");
            }

            if (crossTable == null)
                throw new ArgumentNullException("crossTable");

            m_stream = docStream;
            int startingOffset = CheckJunk();
            m_crossTable = crossTable;
            m_objects = new Dictionary<long,ObjectInformation>();

            PdfReader reader = Reader;
            PdfParser parser = Parser;
            reader.Position = startingOffset;
            reader.SkipWS();
            long whiteSpace = reader.Position;
            // seek to the end of the file.
            long position = reader.Seek(0, SeekOrigin.End);

            // Search back for...
            // %%EOF
            long eofPosition = reader.SearchBack(Operators.EOF);

            if (position != (eofPosition + 5))
            {
                reader.Position = eofPosition + 5;
                string token = reader.GetNextToken();

                if (token != string.Empty && (token[0]!='\0'))
                {
                    // Create a new stream
                    Stream outputStream = new MemoryStream();
                    m_stream.Position = 0;
                    byte[] buffer = new byte[eofPosition + 5];
                    int read = m_stream.Read(buffer, 0, buffer.Length);
                    // Copy the stream
                    outputStream.Write(buffer, 0, buffer.Length);

                    reader = new PdfReader(outputStream);
                    parser = new PdfParser(this, reader, m_crossTable);
                }
            }

            // startxref
            position = reader.SearchBack(Operators.startxref);

            // Read the address of the first (last) cross-reference table.
            //position = ReadStartXRef( reader );
            parser.SetOffset(position);
            position = parser.StartXRef();


            m_startXRef = position;

            //Stack xRefTables = new Stack();

            // Parse XRef-table and trailer.
            parser.SetOffset(position);
            if (whiteSpace != 0)
            {
                position = reader.SearchForward(Operators.xref);
                parser.SetOffset(position);
            }
            string tempString = reader.ReadLine();
            if (!tempString.Contains(Operators.xref) && !tempString.Contains(Operators.obj))
            {
                long tempOffset= reader.SearchBack(Operators.xref);
                if (tempOffset != -1)
                    position = tempOffset;
                parser.SetOffset(position);
            }
            reader.Position = position;

            m_trailer = parser.ParseXRefTable(m_objects, this) as PdfDictionary;
            PdfDictionary trailer = m_trailer;

            while (trailer.ContainsKey(DictionaryProperties.Prev))
            {
                position = (trailer[DictionaryProperties.Prev] as PdfNumber).IntValue;
                PdfReader tokenReader = new PdfReader(m_reader.Stream);
                tokenReader.Position = position;
                string token = tokenReader.GetNextToken();
                if (!token.Equals(DictionaryProperties.Xref))
                {
                    token = tokenReader.GetNextToken();
                    //check the coditon for valid object number
                    if (token.Equals("0"))
                    {
                        token = tokenReader.GetNextToken();
                        if (token.Equals(DictionaryProperties.Obj))
                        {
                            parser.SetOffset(position);
                            trailer = parser.ParseXRefTable(m_objects, this) as PdfDictionary;
                            continue;
                        }
                    }
                    //Rebuild the xref table once the document contain the invalid crosstable
                    parser.RebuildXrefTable(m_objects, this);
                    break;
                }
                else
                {
                    parser.SetOffset(position);
                    trailer = parser.ParseXRefTable(m_objects, this) as PdfDictionary;
                }
            }
            if (whiteSpace != 0)
            {
                
                for (int i = 1; i <= m_objects.Count; i++)
                {
                    if (m_objects.ContainsKey(i))
                    {
                        ObjectInformation info = m_objects[i];
                        m_objects[i] = new ObjectInformation(ObjectType.Normal, info.Offset + whiteSpace, null, this);
                    }
                }
                m_isStructureAltered = true;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Retrieves an object by its reference.
        /// </summary>
        /// <param name="pointer">The reference of the object.</param>
        /// <returns>The object read from its reference.</returns>
        public IPdfPrimitive GetObject(IPdfPrimitive pointer)
        {
            if (pointer == null)
                throw new ArgumentNullException("pointer");

            if (pointer is PdfReference)
            {
                IPdfPrimitive obj;
                PdfReference reference = pointer as PdfReference;
                ObjectInformation oi = this[(long)reference.ObjNum];

                if (oi == null) return new PdfNull();
                
                if(m_crossTable.Encrypted)
                {
                    oi.Parser.Encrypted = true;
                }

                PdfParser parser = oi.Parser;
                long position = oi.Offset;

                if (oi.Obj != null)
                {
                    obj = oi.Obj;
                }
                else if (oi.Archive == null)
                {
                    obj = parser.Parse(position);
                }
                else
                {
                    obj = GetObject(parser, position);

#if !SILVERLIGHT && !NETFX_CORE && !WP
                    if (Encryptor != null)
                    {
                        if (obj is PdfDictionary)
                        {
                            PdfDictionary dictionary = obj as PdfDictionary;
                            dictionary.IsDecrypted = true;
                            foreach(object element in dictionary.Items.Values)
                            {
                                if (element is PdfString)
                                    (element as PdfString).IsParentDecrypted = true;
                            }
                        }

                        IPdfDecryptable encryptedObj = obj as IPdfDecryptable;
                        if (encryptedObj != null)
                            encryptedObj.Decrypt(Encryptor, reference.ObjNum);
                    }
#endif
                }
                oi.Obj = obj;
                //oi.Parser.Encrypted = false;
                return obj;
            }
            else
            {
                return pointer;
            }


        }
        /// <summary>
        /// Retrieves a PDF stream from a PDF document.
        /// </summary>
        /// <param name="streamRef">The reference object to the stream.</param>
        /// <returns>The array of bytes taken from the PDF stream.</returns>
        public byte[] GetStream(IPdfPrimitive streamRef)
        {
            if (streamRef == null)
                throw new ArgumentNullException("streamRef");

            // Retrieve the dictionary.
            PdfStream streamObj = GetObject(streamRef) as PdfStream;

            if (streamObj != null)
            {
                return streamObj.Data;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Parses the new table.
        /// </summary>
        /// <param name="stream">The cross-reference table stream.</param>
        /// <param name="hashTable">The hash table of the objects.</param>
        internal void ParseNewTable(PdfStream stream, Dictionary<long, ObjectInformation> hashTable)
        {
            if (stream == null)
                throw new PdfDocumentException(PdfMessages.InvalidFormat);

            // Decompress the stream.
            stream.Decompress();
            // Parse the stream.
            List<SubSection> ssections = GetSections(stream);
            int ssIndex = 0;

            foreach (SubSection ss in ssections)
            {
                ssIndex = ParseSubsection(stream, ss, hashTable, ssIndex);
            }
        }
        /// <summary>
        /// Parses current subsection.
        /// </summary>
        /// <param name="parser">The PDF stream reader with the file in.</param>
        /// <param name="table">The table with the offsets foud.</param>
        internal void ParseSubsection(PdfParser parser, Dictionary<long, ObjectInformation> table)
        {

            // Read the initial number of the subsection.
            PdfNumber integer = parser.Simple() as PdfNumber;

            m_initialNumberOfSubsection = integer.IntValue;
            // Read the total number of subsection.
            integer = parser.Simple() as PdfNumber;

            m_totalNumberOfSubsection  = integer.IntValue;

            for (int i = 0; i < m_totalNumberOfSubsection; ++i)
            {
                integer = parser.Simple() as PdfNumber;
                long offset = integer.IntValue;
                integer = parser.Simple() as PdfNumber;
                int genNum = integer.IntValue;
                char flag = parser.GetObjectFlag();

                if (flag == 'n')
                {
                    ObjectInformation oi =
                        new ObjectInformation(ObjectType.Normal, offset, null, this);

                    long objectOffset = (long)(m_initialNumberOfSubsection + i);

                    if (!table.ContainsKey(objectOffset))
                    {
                        table[objectOffset] = oi;
                    }
                }
                else
                {
                    if (m_initialNumberOfSubsection != 0 &&offset==0&&genNum == m_generationNumber)
                    {
                       m_initialNumberOfSubsection = m_initialNumberOfSubsection - 1;
                    }
                }
            }
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Retrieves a PDF reader by the archive information.
        /// </summary>
        /// <param name="archive"></param>
        private PdfParser RetrieveParser(ArchiveInformation archive)
        {
            if (archive == null)
            {
                return m_parser;
            }
            else
            {
                PdfStream stream = archive.Archive;

                PdfParser parser = null;
                
                if(m_readersTable.ContainsKey(stream))
                    parser = m_readersTable[stream] as PdfParser;

                if (parser == null)
                {
                    PdfReader reader = new PdfReader(new MemoryStream(stream.Data, false));
                    parser = new PdfParser(this, reader, m_crossTable);

                    m_readersTable[stream] = parser;
                }

                return parser;
            }
        }
        /// <summary>
        /// Retrieves an archive by its number.
        /// </summary>
        /// <returns>The archive.</returns>
        private PdfStream RetrieveArchive(long archiveNumber)
        {
            PdfStream archive = null;
            
            if(m_archives.ContainsKey(archiveNumber))
                archive = m_archives[archiveNumber] as PdfStream;

            if (archive == null)
            {
                ObjectInformation oi = this[archiveNumber];
                PdfParser parser = oi.Parser;

                archive = parser.Parse(oi.Offset) as PdfStream;
#if !SILVERLIGHT && !WP
                archive.Decrypt(Encryptor, archiveNumber);
#endif
                archive.Decompress();

                m_archives[archiveNumber] = archive;
            }

            return archive;
        }
        /// <summary>
        /// Parses the dictionary and retrieves the subsection information
        /// in the ArrayList.
        /// </summary>
        /// <param name="stream">A PDF stream representing a cross-reference
        /// stream dictionary.</param>
        /// <returns>The information about subsections.</returns>
        private List<SubSection> GetSections(PdfStream stream)
        {
            List<SubSection> ss = new List<SubSection>();

            int count = 0;

            count = (stream[DictionaryProperties.Size] as PdfNumber).IntValue;

            if (count == 0)
                throw new PdfDocumentException(PdfMessages.InvalidFormat);

            IPdfPrimitive obj = stream[DictionaryProperties.Index];

            if (obj == null)
            {
                ss.Add(new SubSection(count));
            }
            else
            {
                PdfArray indices = GetObject(obj) as PdfArray;

                if (indices == null)
                    throw new PdfDocumentException(PdfMessages.InvalidFormat);

                if ((indices.Count & 1) != 0) // Threre are odd number of objects.
                    throw new PdfDocumentException(PdfMessages.InvalidFormat);

                for (int i = 0; i < indices.Count; ++i)
                {
                    int n = 0, c = 0;

                    n = (indices[i] as PdfNumber).IntValue;
                    ++i;
                    c = (indices[i] as PdfNumber).IntValue;

                    ss.Add(new SubSection(n, c));
                }
            }

            return ss;
        }
        /// <summary>
        /// Parses a subsection within a cros-reference stream.
        /// </summary>
        /// <param name="stream">A cross-reference stream.</param>
        /// <param name="subsection">A structure that specifies a subsection.</param>
        /// <param name="table">The table with the offsets foud.</param>
        /// <param name="startIndex">The start position within the stream data.</param>
        /// <returns>The start position of the next subsection.</returns>
        private int ParseSubsection(PdfStream stream, SubSection subsection,
            Dictionary<long, ObjectInformation> table, int startIndex)
        {
            int index = startIndex;

            PdfArray wEntry = GetObject(stream[DictionaryProperties.W]) as PdfArray;

            int fields = wEntry.Count;
            int[] format = new int[fields];

            for (int i = 0; i < fields; ++i)
            {
                format[i] = (wEntry[i] as PdfNumber).IntValue;
            }

            long[] reference = new long[fields];

            byte[] buf = stream.Data;

            for (int i = 0, total = subsection.Count; i < total; ++i)
            {
                for (int j = 0; j < fields; ++j)
                {
                    int field = 0;

                    for (int k = 0; k < format[j]; ++k)
                    {
                        field <<= 8;
                        field += buf[index++];
                    }

                    reference[j] = field;
                }

                // Store information about the object into the hashtable.
                // Required information:
                // 1. type
                // 2. offset
                // 3. Archive info
                // | 3.1 reader
                // | 3.2 archive number
                // | 3.3 archive index
                long offset = 0;
                ArchiveInformation ai = null;

                if (reference[0] == (long)ObjectType.Normal)
                {
                    offset = (long)reference[1];
                }
                else if (reference[0] == (long)ObjectType.Packed)
                {
                    ai = new ArchiveInformation(reference[1], reference[2],
                        new GetArchive(RetrieveArchive));
                }
				else
                {
                    PdfReader reader = Reader;
                    reader.Position = offset;
                    string temp=reader.ReadLine();
                    if (!temp.Contains("%") && !temp.Contains(DictionaryProperties.Obj))
                    {
                        reader.Position = 0;
                        offset = reader.SearchForward(i.ToString() + " 0 obj");
                        if (offset != -1)
                            reference[0] = (long)ObjectType.Normal;
                    }
                }

                ObjectInformation oi = null;

                // NOTE: do not store removed objects.
                if (reference[0] != (long)ObjectType.Free)
                {
                    oi = new ObjectInformation((ObjectType)reference[0],
                        offset, ai, this);
                }

                if (oi != null)
                {
                    long objectOffset = (long)(subsection.StartNumber + i);

                    if (!table.ContainsKey(objectOffset))
                    {
                        table[objectOffset] = oi;
                    }
                }
            }

            return index;
        }
        /// <summary>
        /// Retrieves an object by its reference.
        /// </summary>
        /// <param name="parser">The PDF parser.</param>
        /// <param name="position">The position within the reader's stream.</param>
        /// <returns>The object read from its reference.</returns>
        private IPdfPrimitive GetObject(PdfParser parser, long position)
        {
            parser.StartFrom(position);
            // read the object.
            IPdfPrimitive obj = parser.Simple();

            return obj;
        }
       
        /// <summary>
        /// Skip junk string from the PDF
        /// </summary>
        /// <returns></returns>
        private int CheckJunk()
        {
            byte[] data = new byte[m_stream.Length];
            m_stream.Position = 0;
            m_stream.Read(data, 0, (int)m_stream.Length);
#if SILVERLIGHT || NETFX_CORE || WP
            string header = System.Text.Encoding.UTF8.GetString(data, 0, data.Length);
#else
           string header = System.Text.Encoding.Default.GetString(data);
#endif
            int index = header.IndexOf("%PDF-");
            m_stream.Position = 0;
            return index;
        }

        #endregion

        #region Internals
        /// <summary>
        /// Represents a type of an object.
        /// </summary>
#if NETFX_CORE || WP
        public enum ObjectType
#else
        internal enum ObjectType
#endif
        {
            Free = 0,
            Normal = 1,
            Packed = 2
        }
        /// <summary>
        /// Stores information about a PDF packed object.
        /// </summary>
#if NETFX_CORE || WP
        public class ArchiveInformation
#else
        internal class ArchiveInformation
#endif
        {
            #region Fields
            /// <summary>
            /// A number of an object stream that holds the object.
            /// </summary>
            private long m_archiveNumber;
            /// <summary>
            /// The index of the object within the archive.
            /// </summary>
            private long m_index;
            /// <summary>
            /// The archive.
            /// </summary>
            private PdfStream m_archive;
            /// <summary>
            /// Delegate that retrieves an archive by its number.
            /// </summary>
            private GetArchive m_getArchive;
            #endregion

            #region Properties
            /// <summary>
            /// Gets a number of an object stream that holds the object.
            /// </summary>
            public PdfStream Archive
            {
                get
                {
                    if (m_archive == null)
                    {
                        m_archive = m_getArchive(m_archiveNumber);
                    }

                    return m_archive;
                }
            }
            /// <summary>
            /// Gets the index of the object within the archive.
            /// </summary>
            public long Index
            {
                get
                {
                    return m_index;
                }
            }
            /// <summary>
            /// Gets the archive number.
            /// </summary>
            internal long ArchiveNumber
            {
                get
                {
                    return m_archiveNumber;
                }
            }
            #endregion

            #region Constructors
            /// <summary>
            /// Initialize the archive information class with the archive number and
            /// the index within the archive.
            /// </summary>
            /// <param name="arcNum">The archive number.</param>
            /// <param name="index">The index within the archive.</param>
            /// <param name="getArchive">The GetArchive delegade.</param>
            public ArchiveInformation(long arcNum, long index, GetArchive getArchive)
            {
                m_archiveNumber = arcNum;
                m_index = index;
                m_getArchive = getArchive;
            }
            #endregion

        }
#if NETFX_CORE || WP
        public delegate PdfStream GetArchive(long archiveNumber);
#else
        internal delegate PdfStream GetArchive(long archiveNumber);
#endif
        /// <summary>
        /// Stores information about an PDF indirect object.
        /// </summary>
# if NETFX_CORE || WP
        public class ObjectInformation
#else
        internal class ObjectInformation
#endif
        {
            #region Fields
            /// <summary>
            /// The type of the object.
            /// </summary>
            private ObjectType m_type;
            /// <summary>
            /// The archive information.
            /// </summary>
            private ArchiveInformation m_archive;
            /// <summary>
            /// The PDF reader wich can read from the archive.
            /// </summary>
            private PdfParser m_parser;
            /// <summary>
            /// The offset of the object.
            /// </summary>
            private long m_offset;
            /// <summary>
            /// The CrossTable class instance.
            /// </summary>
            private CrossTable m_crossTable;
            #endregion

            #region Properties
            /// <summary>
            /// Gets the type of the object.
            /// </summary>
            public ObjectType Type
            {
                get
                {
                    return m_type;
                }
            }
            /// <summary>
            /// The PDF reader wich can read from the archive.
            /// </summary>
            public PdfParser Parser
            {
                get
                {
                    if (m_parser == null)
                    {
                        m_parser = m_crossTable.RetrieveParser(m_archive);
                    }

                    return m_parser;
                }
            }
            /// <summary>
            /// The offset of the object.
            /// </summary>
            public long Offset
            {
                get
                {
                    if (m_offset == 0)
                    {
                        PdfParser parser = Parser;
                        parser.StartFrom(0);

                        int pairs = 0;

                        // Read indices.
                        if (Archive!=null)
                        {
                            pairs = (Archive.Archive[DictionaryProperties.N] as PdfNumber).IntValue;

                            int[] indices = new int[pairs * 2];

                            for (int i = 0; i < pairs; ++i)
                            {
                                IPdfPrimitive obj = parser.Simple();
                                indices[i * 2] = (obj as PdfNumber).IntValue;
                                obj = parser.Simple();
                                indices[i * 2 + 1] = (obj as PdfNumber).IntValue;
                            }

                            long index = Archive.Index;

                            if (index * 2 >= indices.Length)
                                throw new PdfDocumentException("Missing indexes in archive #" + Archive.ArchiveNumber);

                            m_offset = indices[index * 2 + 1];

                            long first =
                                (Archive.Archive[DictionaryProperties.First] as PdfNumber).IntValue;

                            m_offset += first;
                        }
                    }
                    return m_offset;
                }
            }
            /// <summary>
            /// Gets the archive information.
            /// </summary>
            public ArchiveInformation Archive
            {
                get
                {
                    return m_archive;
                }
            }
            /// <summary>
            /// Converts an Object information into the offset.
            /// </summary>
            /// <param name="oi">An ObjectInformation class instance.</param>
            /// <returns>The offset of zero.</returns>
            public static implicit operator long(ObjectInformation oi)
            {
                return oi.Offset;
            }
            /// <summary>
            /// Holds the parsed object.
            /// </summary>
            public IPdfPrimitive Obj;
            #endregion

            #region Constructors
            /// <summary>
            /// Initialize the object information object.
            /// </summary>
            /// <param name="type">The object's type.</param>
            /// <param name="offset">The object's offset or <b>zero</b>.</param>
            /// <param name="arciveInfo">The archive info or <b>null</b>.</param>
            /// <param name="crossTable">The low-level reading cross table.</param>
            public ObjectInformation(ObjectType type, long offset,
                ArchiveInformation arciveInfo, CrossTable crossTable)
            {
                m_type = type;
                m_offset = offset;
                m_archive = arciveInfo;
                m_crossTable = crossTable;
            }
            #endregion
        }
        /// <summary>
        /// Represents a subsection in a cross-reference stream.
        /// </summary>
        private struct SubSection
        {
            #region Properties
            /// <summary>
            /// The first object number in the subsection.
            /// </summary>
            public int StartNumber;
            /// <summary>
            /// The total number of the objects within the subsection.
            /// </summary>
            public int Count;
            #endregion

            #region Constructors
            /// <summary>
            /// Initialize the subsection with start number and count.
            /// </summary>
            /// <param name="start">The first object number in the subsection.</param>
            /// <param name="count">The total number of the objects within the subsection.
            /// </param>
            public SubSection(int start, int count)
            {
                StartNumber = start;
                Count = count;
            }
            /// <summary>
            /// Initialize the subsection with count.
            /// </summary>
            /// <param name="count">The total number of the objects within the subsection.
            /// </param>
            public SubSection(int count)
            {
                StartNumber = 0;
                Count = count;
            }
            #endregion
        }
        #endregion

    }
}
