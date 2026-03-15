#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.IO;
using System.Text;
#if !NETFX_CORE && !WP
using Syncfusion.Compression;
#endif
using Syncfusion.Pdf.Compression;
using Syncfusion.Pdf.IO;
using Syncfusion.Pdf.Security;

namespace Syncfusion.Pdf.Primitives
{
#if NETFX_CORE || WP
    public class PdfStream:
#else
    internal class PdfStream:
#endif
        PdfDictionary,
        IPdfDecryptable
    {
        #region Constants
        private const string Prefix = "stream";
        private const string Suffix = "endstream";
        #endregion

        #region Fields
        private MemoryStream m_dataStream;
        private bool m_blockEncryption;
        private bool m_bDecrypted;
        private bool m_bCompress;
        /// <summary>
        /// Internal variable to hold cloned object.
        /// </summary>
        private PdfStream m_clonedObject = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the internal stream.
        /// </summary>
#if NETFX_CORE || WP
        public MemoryStream InternalStream
#else
        internal MemoryStream InternalStream
#endif
        {
            get
            {
                return m_dataStream;
            }
            set
            {
                m_dataStream = value;
            }
        }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <remarks>The modifications of the returned stream
        /// have no impact on the real data.</remarks>
#if NETFX_CORE || WP
        public byte[] Data
#else
        internal byte[] Data
#endif
        {
            get
            {
                return m_dataStream.ToArray();
            }
            set
            {
                m_dataStream.SetLength(0);
                m_dataStream.Write(value, 0, value.Length);
                Modify();
            }
        }
        /// <summary>
        /// Gets or sets compression flag.
        /// </summary>
        /// <value><c>true</c> if compress; otherwise, <c>false</c>.</value>
        internal bool Compress
        {
            get
            {
                return m_bCompress;
            }
            set
            {
                m_bCompress = value;
                Modify();
            }
        }

        /// <summary>
        /// Returns cloned object.
        /// </summary>
        public override IPdfPrimitive ClonedObject
        {
            get
            {
                return m_clonedObject;
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfStream"/> class.
        /// </summary>
        internal PdfStream()
            : base()
        {
            m_dataStream = new MemoryStream(100);
            m_bCompress = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfStream"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="data">The data.</param>
        internal PdfStream(PdfDictionary dictionary, byte[] data)
            : base(dictionary)
        {
            m_dataStream = new MemoryStream(data.Length);
            Data = data;
            m_bCompress = false;
            this[DictionaryProperties.Length] = new PdfNumber(data.Length);
            if (this.ContainsKey(DictionaryProperties.Length3) && this.ContainsKey(DictionaryProperties.Filter))
            {
                if (this[DictionaryProperties.Length3] is PdfReferenceHolder)
                {
                    byte[] fontData = Decompress(data, "FlateDecode");
                    string fontProgram = UTF8Encoding.UTF8.GetString(fontData, 0, fontData.Length);
                    string[] fontEntries = fontProgram.Split(new string[] { "eexec" }, StringSplitOptions.None);
                    string eexecZeros = string.Empty;
                    //32 * 16 = 512 ASCII zero's (fixed-content portion)
                    for (int j = 0; j < 32; j++)
                    {
                        eexecZeros += "0";
                    }
                    //eexec and carriage return characters length are 7
                    int length1 = fontEntries[0].Length + 7;
                    if (fontEntries.Length > 1)
                    {
                        string[] fixedContent = fontEntries[1].Split(new string[] { eexecZeros }, StringSplitOptions.None);
                        int cleartomarkCount = 0;
                        for (int i = 1; i < fixedContent.Length; i++)
                        {
                            cleartomarkCount += fixedContent[i].Length;
                        }
                        int length3 = (32 * (fixedContent.Length - 1)) + cleartomarkCount;

                        byte[] encodeData = UTF8Encoding.UTF8.GetBytes(fixedContent[0]);
                        this[DictionaryProperties.Length1] = new PdfNumber(length1);
                        this[DictionaryProperties.Length2] = new PdfNumber(fontData.Length - length1 - length3);
                        this[DictionaryProperties.Length3] = new PdfNumber(length3);
                    }
                }
            }
            if (this.ContainsKey(DictionaryProperties.Length1) && this.ContainsKey(DictionaryProperties.Filter))
            {
                if (this[DictionaryProperties.Length1] is PdfReferenceHolder && this[DictionaryProperties.Filter] is PdfArray)
                {
                    this[DictionaryProperties.Length1] = new PdfNumber(data.Length);
                }
            }
        }
        #endregion

        #region Static methods
        /// <summary>
        /// Gets bytes of the stream.
        /// </summary>
        /// <param name="stream">Stream to be converted.</param>
        /// <returns>Destination bytes.</returns>
        public static byte[] StreamToBytes(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            return StreamToBytes(stream, false);
        }

        /// <summary>
        /// Gets bytes of the stream.
        /// </summary>
        /// <param name="stream">Stream to be converted.</param>
        /// <param name="writeWholeStream">Indicates whether to write the whole stream.</param>
        /// <returns>Destination bytes.</returns>
        public static byte[] StreamToBytes(Stream stream, bool writeWholeStream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            long oldPos = stream.Position;

            long pos = (stream.Position != 0) ? stream.Position : stream.Length;
            pos = (writeWholeStream) ? stream.Length : pos;

            byte[] result = new byte[pos];

            stream.Position = 0;
            // NOTE: need to write code to process coreectly if Pos > max(int32)
            stream.Read(result, 0, (int)pos);
            stream.Position = oldPos;

            return result;
        }

        /// <summary>
        /// Converts a stream to bigendian format.
        /// </summary>
        /// <param name="stream">A stream containing data.</param>
        /// <returns>A stream in bigendian format.</returns>
        public static byte[] StreamToBigEndian(Stream stream)
        {
            byte[] lEndian = StreamToBytes(stream);
            return Encoding.Convert(Encoding.Unicode, Encoding.BigEndianUnicode, lEndian);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Writes the specified symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        internal void Write(char symbol)
        {
            Write(symbol.ToString());
        }

        /// <summary>
        /// Writes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        internal void Write(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            if (text.Length <= 0)
                throw new ArgumentException("Can't write an empty string.", "text");

            byte[] data = Encoding.UTF8.GetBytes(text);

            Write(data);
        }

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        internal void Write(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (data.Length <= 0)
                throw new ArgumentException("Can't write an empty array.", "data");

            m_dataStream.Write(data, 0, data.Length);
            Modify();
        }

        /// <summary>
        /// Blocks the encryption.
        /// </summary>
        internal void BlockEncryption()
        {
            m_blockEncryption = true;
        }

        /// <summary>
        /// Decompresses this instance.
        /// </summary>
#if NETFX_CORE || WP
        public void Decompress()
#else
        internal void Decompress()
#endif
        {
            // Decompress the data.
            IPdfPrimitive obj = this[DictionaryProperties.Filter];

            if (obj is PdfReferenceHolder)
            {
                PdfReferenceHolder rh = obj as PdfReferenceHolder;
                obj = rh.Object;
            }

            if (obj != null)
            {
                if (obj is PdfName)
                {
                    PdfName filter = obj as PdfName;

                    Data = Decompress(Data, filter.Value);
                }
                else if (obj is PdfArray)
                {
                    PdfArray filter = obj as PdfArray;

                    foreach (IPdfPrimitive o in filter)
                    {
                        string filterName = (o as PdfName).Value;

                        if (filterName == null)
                            throw new PdfDocumentException(PdfMessages.InvalidFormat);

                        Data = Decompress(Data, filterName);
                    }
                }
                else
                {
                    throw new PdfDocumentException(PdfMessages.InvalidFormat + "Unexpected object for filter.");
                }
            }

            Remove(DictionaryProperties.Filter);
            m_bCompress = true;
        }

        /// <summary>
        /// Cleares a stream.
        /// </summary>
        new internal void Clear()
        {
            InternalStream.SetLength(0);
            InternalStream.Position = 0;
            Remove(DictionaryProperties.Filter);
            m_bCompress = true;
            Modify();
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Saves the object using the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public override void Save(IPdfWriter writer)
        {
            // Raise event.
            SavePdfPrimitiveEventArgs args = new SavePdfPrimitiveEventArgs(writer);
            OnBeginSave(args);

            byte[] data = CompressContent(writer);
            data = EncryptContent(data, writer);

            this[DictionaryProperties.Length] = new PdfNumber(data.Length); //m_dataStream.Length );
            if (this.ContainsKey(DictionaryProperties.Length1) && this.ContainsKey(DictionaryProperties.Filter))
            {
                if (this[DictionaryProperties.Length1] is PdfReferenceHolder && this[DictionaryProperties.Filter] is PdfArray)
                {
                    this[DictionaryProperties.Length1] = new PdfNumber(data.Length);
                }
            }
            if (this.ContainsKey(DictionaryProperties.Length3) && this.ContainsKey(DictionaryProperties.Filter))
            {
                if (this[DictionaryProperties.Length3] is PdfReferenceHolder)
                {
                    byte[] fontData = Decompress(data, "FlateDecode");
                    string fontProgram = UTF8Encoding.UTF8.GetString(fontData, 0, fontData.Length); 
                    string[] fontEntries = fontProgram.Split(new string[] { "eexec" }, StringSplitOptions.None);
                    string eexecZeros = string.Empty;
                    //32 * 16 = 512 ASCII zero's (fixed-content portion)
                    for (int j = 0; j < 32; j++)
                    {
                        eexecZeros += "0";
                    }
                    //eexec and carriage return characters length are 7
                    int length1 = fontEntries[0].Length + 7;
                    if (fontEntries.Length > 1)
                    {
                        string[] fixedContent = fontEntries[1].Split(new string[] { eexecZeros }, StringSplitOptions.None);
                        int cleartomarkCount = 0;
                        for (int i = 1; i < fixedContent.Length; i++)
                        {
                            cleartomarkCount += fixedContent[i].Length;
                        }
                        int length3 = (32 * (fixedContent.Length - 1)) + cleartomarkCount;

                        byte[] encodeData = UTF8Encoding.UTF8.GetBytes(fixedContent[0]);
                        this[DictionaryProperties.Length1] = new PdfNumber(length1);
                        this[DictionaryProperties.Length2] = new PdfNumber(fontData.Length - length1 - length3);
                        this[DictionaryProperties.Length3] = new PdfNumber(length3);
                    }
                }
            }
            base.Save(writer, false);

            writer.Write(Prefix);
            writer.Write(Operators.NewLine);

            if (data.Length > 0)
            {
                writer.Write(data);
                writer.Write(Operators.NewLine);
            }

            writer.Write(Suffix);
            writer.Write(Operators.NewLine);

            // Raise event.
            SavePdfPrimitiveEventArgs args2 = new SavePdfPrimitiveEventArgs(writer);
            OnEndSave(args2);

            if (m_bCompress)
            {
                Remove(DictionaryProperties.Filter);
            }
        }

        /// <summary>
        /// Creates a copy of PdfStream.
        /// </summary>
        public override IPdfPrimitive Clone(PdfCrossTable crossTable)
        {
            if (this.m_clonedObject != null && this.m_clonedObject.CrossTable == crossTable)
                return this.m_clonedObject;
            else
                this.m_clonedObject = null;

            // Else clone the object.
            PdfDictionary dict = base.Clone(crossTable) as PdfDictionary;

            PdfStream newStream = new PdfStream(dict, m_dataStream.ToArray());
            newStream.Compress = m_bCompress;
            newStream.m_bDecrypted = m_bDecrypted;

            this.m_clonedObject = newStream;

            return newStream;
        }
        #endregion

        #region IPdfDecryptable Members
        /// <summary>
        /// Gets a value indicating whether the object was encrypted.
        /// </summary>
        public bool WasEncrypted
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="PdfStream"/> is decrypted.
        /// </summary>
        /// <value><c>true</c> if decrypted; otherwise, <c>false</c>.</value>
        public bool Decrypted
        {
            get
            {
                return m_bDecrypted;
            }
        }

#if !SILVERLIGHT && !WP
        /// <summary>
        /// Decrypts the data using the specified encryptor.
        /// </summary>
        /// <param name="encryptor">The encryptor.</param>
        /// <param name="currObjNumber">The current object number.</param>
        public void Decrypt(PdfEncryptor encryptor, long currObjNumber)
        {
            if (encryptor != null && !m_bDecrypted)
            {
                m_bDecrypted = true;

                Data = encryptor.EncryptData(currObjNumber, Data,false);
            }
        }
#endif
        #endregion

        #region Helper methods
        /// <summary>
        /// Decompresses the stream data.
        /// </summary>
        /// <param name="data">The data to decompress.</param>
        /// <param name="filter">The filter name.</param>
        /// <returns>Uncompressed byte array.</returns>
        private byte[] Decompress(byte[] data, string filter)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (filter == null)
                throw new ArgumentNullException("filter");

            if (data.Length == 0) return data;

            if (filter != DictionaryProperties.Crypt)
            {
                if(filter.Equals(DictionaryProperties.RunLengthDecode))
                {
                    Stream compressed = new MemoryStream(data);
                    MemoryStream outStream = new MemoryStream();
                    int dupAmount = -1;
                    byte[] buffer = new byte[128];
                    while ((dupAmount = compressed.ReadByte()) != -1 && dupAmount != 128)
                    {
                        if (dupAmount <= 127)
                        {

                            int amountTocopy = dupAmount + 1;
                            int compressedRead = 0;
                            while (amountTocopy > 0)
                            {
                                compressedRead = compressed.Read(buffer, 0, amountTocopy);
                                outStream.Write(buffer, 0, compressedRead);
                                amountTocopy -= compressedRead;
                            }
                        }

                        else
                        {
                            int dupByte = compressed.ReadByte();
                            for (int i = 0; i < 257 - dupAmount; i++)
                            {
                                outStream.WriteByte((byte)dupByte);
                            }
                        }

                    }
                    outStream.Position = 0;

                    return outStream.ToArray();
                 
                }
                else
                {
                IPdfCompressor compressor = DetermineCompressor(filter);

                return PostProcess(compressor.Decompress(data), filter);
                }
            }
            else
                return data;
        }

        /// <summary>
        /// Returnes a compressor by its name.
        /// </summary>
        /// <param name="filter">The name of the compressor.</param>
        /// <returns>IPDFCompressor interface.</returns>
        private IPdfCompressor DetermineCompressor(string filter)
        {
            if (filter == null)
                throw new ArgumentNullException("filter");

            switch (filter)
            {
                case DictionaryProperties.FlateDecode:
                case DictionaryProperties.FlateDecodeShort:
                    return new PdfZlibCompressor();

                case DictionaryProperties.LZWDecode:
                case DictionaryProperties.LZWDecodeShort:
                    return new PdfLzwCompressor();

                case DictionaryProperties.ASCII85Decode:
                case DictionaryProperties.ASCII85DecodeShort:
                    return new PdfASCII85Compressor();

                case DictionaryProperties.DCTDecode:
                case DictionaryProperties.DCTDecodeShort:
                default:
                    throw new PdfDocumentException(PdfMessages.InvalidFormat
                        + " Unsupported compressor (" + filter + ").");
            }
        }

        /// <summary>
        /// Performs postprocessing of the data for the filter specified.
        /// </summary>
        /// <param name="data">The data to process.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>Restored data.</returns>
        private byte[] PostProcess(byte[] data, string filter)
        {
            IPdfPrimitive obj;

            if (filter == DictionaryProperties.FlateDecode)
            {
                obj = this[DictionaryProperties.DecodeParms];

                if (obj == null) return data;

                PdfDictionary decodeParams = (obj) as PdfDictionary;

                PdfArray decodeParamsArr = (obj) as PdfArray;

                if (decodeParams == null)
                {
                    if (decodeParamsArr == null)
                        throw new PdfDocumentException(PdfMessages.InvalidFormat);
                }


                int predictor = 1; // The default value, which means no prediction.

                predictor = (decodeParams != null) ?
                    (int)(decodeParams[DictionaryProperties.Predictor] as PdfNumber).IntValue
                    : ((decodeParamsArr[0] as PdfDictionary) != null) ? (decodeParamsArr[0] as PdfDictionary).GetInt(DictionaryProperties.Predictor) : 1;


                if (predictor == 1) // No prediction.
                {
                    return data;
                }
                else if (predictor == 2) // TIFF Predictor 2.
                {
                    throw new PdfDocumentException("Unsupported predictor: TIFF 2.");
                }
                else if (predictor < 16 && predictor > 2) // PNG filter.
                {
                    int colors = 1;
                    int columns = 1;
                    int bitsPerComponent = 8;

                    obj = decodeParams[DictionaryProperties.Colors];

                    if (obj != null)
                        colors = (int)(obj as PdfNumber).IntValue;

                    obj = decodeParams[DictionaryProperties.Columns];

                    if (obj != null)
                        columns = (int)(obj as PdfNumber).IntValue;

                    obj = decodeParams[DictionaryProperties.BitsPerComponent];

                    if (obj != null)
                        bitsPerComponent = (int)(obj as PdfNumber).IntValue;

                    return PdfPngFilter.Decompress(data, colors * columns);
                }
                else
                {
                    throw new PdfDocumentException(PdfMessages.InvalidFormat
                        + " Unknown predictor code: " + predictor.ToString());
                }
            }

            return data;
        }

        /// <summary>
        /// Normalizes the filter. If the filter array has only one element store that element
        /// instead of entire array.
        /// </summary>
        private void NormalizeFilter()
        {
            PdfArray filter = this[DictionaryProperties.Filter] as PdfArray;

            if (filter != null && filter.Count == 1)
            {
                this[DictionaryProperties.Filter] = filter[0];
            }
        }

        /// <summary>
        /// Compresses the content if it's required.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <returns>The compressed data.</returns>
        private byte[] CompressContent(IPdfWriter writer)
        {
            PdfCompressionLevel level = writer.Document.Compression;
            bool compress = (level != Syncfusion.Pdf.PdfCompressionLevel.None);

            byte[] data = Data;

            if (compress && m_bCompress)
            {
                PdfZlibCompressor compressor = new PdfZlibCompressor(level);
                data = compressor.Compress(data);
                AddFilter(DictionaryProperties.FlateDecode);
                //m_bCompress = false;
            }

            return data;
        }

        /// <summary>
        /// Adds a filter to the filter array.
        /// </summary>
        /// <param name="filterName">Name of the filter.</param>
        private void AddFilter(string filterName)
        {
            IPdfPrimitive obj = this[DictionaryProperties.Filter];

            if (obj is PdfReferenceHolder)
            {
                PdfReferenceHolder rh = obj as PdfReferenceHolder;
                obj = rh.Object;
            }

            PdfArray array = obj as PdfArray;
            PdfName name = obj as PdfName;

            if (name != null)
            {
                array = new PdfArray();
                array.Insert(0, name);
                this[DictionaryProperties.Filter] = array;
            }

            name = new PdfName(filterName);

            if (array == null)
            {
                this[DictionaryProperties.Filter] = name;
            }
            else
            {
                array.Insert(0, name);
            }
        }

        /// <summary>
        /// Encrypts the stream content.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="writer">The writer.</param>
        /// <returns>The encrypted content.</returns>
        private byte[] EncryptContent(byte[] data, IPdfWriter writer)
        {
            PdfDocumentBase doc = writer.Document;
#if !SILVERLIGHT && !WP
            PdfEncryptor encryptor = doc.Security.Encryptor;

            if (encryptor.Encrypt && !m_blockEncryption)
            {
                data = encryptor.EncryptData(doc.CurrentSavingObj.ObjNum, data,true);
            }
#endif
            return data;
        }
        #endregion
    }
}
