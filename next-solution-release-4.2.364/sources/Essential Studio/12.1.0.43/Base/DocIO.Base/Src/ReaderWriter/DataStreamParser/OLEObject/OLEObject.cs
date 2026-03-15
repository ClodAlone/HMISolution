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
using System.Text;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
using Syncfusion.CompoundFile.DocIO;
using System.IO;
using Syncfusion.DocIO.DLS;
using Syncfusion.CompoundFile.DocIO.Net;
using Syncfusion.CompoundFile.DocIO.Native;

namespace Syncfusion.DocIO.ReaderWriter.DataStreamParser.OLEObject
{
    internal class OLEObject
    {
        #region Constants
        /// <summary>
        /// The ole stream name
        /// </summary>
        private const string DEF_OLE_STREAM_NAME = "Ole";
        private const string DEF_CONTENT_STREAM_NAME = "CONTENTS";
        private const string DEF_WP_STREAM_NAME = "Contents";
        private const string DEF_INFO_STREAM_NAME = "ObjInfo";
        private const string DEF_COMP_STREAM_NAME = "CompObj";
        private const string DEF_LINK_INFO_STREAM_NAME = "LinkInfo";
        private const string DEF_NATIVE_STREAM_NAME = "Ole10Native";
        private const string DEF_PRINT_STREAM_NAME = "EPRINT";
        private const string DEF_OLE_PRES000_NAME = "OlePres000";
        private const string DEF_END_INFO_MARKER = "???";
        private const string DEF_EQUATION_STREAM_NAME = "Equation Native";
        private const string DEF_WORKBOOK_STREAM_NAME = "Workbook";
        private const string DEF_PACKAGE_STREAM_NAME = "Package";
        private const string DEF_PPT_STREAM_NAME = "PowerPoint Document";
        private const string DEF_WORD_STREAM_NAME = "WordDocument";
        private const string DEF_VISIO_STREAM_NAME = "VisioDocument";
        private const string DEF_ODP_STREAM_NAME = "EmbeddedOdf";
        private const string DEF_OOPACKAGE_STREAM_NAME = "package_stream";
        private const string DEF_SUMMARY_STREAM_NAME = "SummaryInformation";
        private const string DEF_DOC_SUMMARY_STREAM_NAME = "DocumentSummaryInformation";
        private const string DEF_OBJECT_POOL_NAME = "ObjectPool";
        #endregion

        #region Fields
        /// <summary>
        /// 
        /// </summary>
        private OleObjectType m_oleType;
        private Storage m_storage = new Storage("Ole");
        private Guid m_guid;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the GUID.
        /// </summary>
        /// <value>The GUID.</value>
        internal Guid Guid
        {
            get
            {
                return m_guid;
            }
            set
            {
                m_guid = value;
            }
        }
        /// <summary>
        /// Gets the storage.
        /// </summary>
        /// <value>The storage.</value>
        internal Storage Storage
        {
            get
            {
                return m_storage;
            }
        }
        /// <summary>
        /// Gets the type of the OLE object.
        /// </summary>
        /// <value>The type of the OLE.</value>
        internal OleObjectType OleType
        {
            get
            {
                if (Storage.Streams.ContainsKey(DEF_COMP_STREAM_NAME))
                {
                   CompObjectStream compObject = new CompObjectStream(Storage.Streams[DEF_COMP_STREAM_NAME]);
                   m_oleType = OleTypeConvertor.ToOleType(compObject.ObjectType);
                }
                else
                {
                    m_oleType = OleObjectType.Undefined;
                }
                return m_oleType;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="OLEObject"/> class.
        /// </summary>
        internal OLEObject()
        { 
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Parses the object pool.
        /// </summary>
        /// <param name="objectPoolStream">The object pool stream.</param>
        /// <param name="oleStorageName">Name of the OLE storage.</param>
        internal void ParseObjectPool(Stream objectPoolStream, string oleStorageName)
        {
            Storage.Streams.Clear();
            Storage.StorageName = oleStorageName;
            using (Syncfusion.CompoundFile.DocIO.Net.CompoundFile ole = new CompoundFile.DocIO.Net.CompoundFile(objectPoolStream))
            {
                foreach (DirectoryEntry entry in ole.Directory.Entries)
                {
                    if (oleStorageName == entry.Name)
                    {
                        Guid = entry.StorageGuid;
                        break;
                    }
                }
                if ((Array.IndexOf(ole.RootStorage.Storages, DEF_OBJECT_POOL_NAME) != -1)
                    && (Array.IndexOf(ole.RootStorage.OpenStorage(DEF_OBJECT_POOL_NAME).Storages, oleStorageName) != -1))
                {
                    ICompoundStorage storage = ole.RootStorage.OpenStorage(DEF_OBJECT_POOL_NAME);
                    storage = storage.OpenStorage(oleStorageName);
                    Storage.ParseStreams(storage);
                    Storage.ParseStorages(storage);
                }
            }
            objectPoolStream.Position = 0;
        }
        /// <summary>
        /// Updates the GUID.
        /// </summary>
        /// <param name="cmpFile">The CMP file.</param>
        /// <param name="index">The index.</param>
        internal void UpdateGuid(Syncfusion.CompoundFile.DocIO.Net.CompoundFile cmpFile, int index)
        {
            for (int i = index; i < cmpFile.Directory.Entries.Count; i++)
            {
                DirectoryEntry entry = cmpFile.Directory.Entries[i];
                if (entry.Name == Storage.StorageName
                    || entry.Name == "Root Entry")
                {
                    entry.StorageGuid = Guid;
                }
            }
        }
        /// <summary>
        /// Saves the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="oleObject">The OLE object.</param>
        internal void Save(Stream stream, WOleObject oleObject)
        {
            // Write "Ole" stream if needed 
            WriteOleStream(oleObject.LinkType, oleObject.OleObjectType, string.Empty);
            WriteObjInfoStream(oleObject.LinkType, oleObject.OleObjectType);
            WriteCompObjStream(oleObject.OleObjectType);
            if (oleObject.OleObjectType == OleObjectType.Undefined)
                Storage.Streams.Add(DEF_PACKAGE_STREAM_NAME, stream);
            else
                WriteNativeData((stream as MemoryStream).ToArray(), string.Empty, oleObject.OleObjectType);
        }
        /// <summary>
        /// Saves the specified native data.
        /// </summary>
        /// <param name="nativeData">The native data.</param>
        /// <param name="dataPath">The data path.</param>
        /// <param name="oleObject">The OLE object.</param>
        internal void Save(byte[] nativeData, string dataPath, WOleObject oleObject)
        {
            // Write "Ole" stream if needed 
            WriteOleStream(oleObject.LinkType, oleObject.OleObjectType, dataPath);
            WriteObjInfoStream(oleObject.LinkType, oleObject.OleObjectType);

            if (oleObject.LinkType == OleLinkType.Embed)
            {
                WriteCompObjStream(oleObject.OleObjectType);
                WriteNativeData(nativeData, dataPath, oleObject.OleObjectType);
            }
            else
            {
                WriteLinkInfoStream(oleObject.OleObjectType, dataPath);
            }
        }
        /// <summary>
        /// Writes the native data.
        /// </summary>
        /// <param name="nativeData">The native data.</param>
        /// <param name="dataPath">The data path.</param>
        /// <param name="objType">Type of the object.</param>
        private void WriteNativeData(byte[] nativeData, String dataPath, OleObjectType objType)
        {
            switch (objType)
            {
                case OleObjectType.WordDocument:
                case OleObjectType.ExcelBinaryWorksheet:
                case OleObjectType.ExcelMacroWorksheet:
                case OleObjectType.ExcelWorksheet:
                case OleObjectType.PowerPointMacroPresentation:
                case OleObjectType.PowerPointMacroSlide:
                case OleObjectType.PowerPointPresentation:
                case OleObjectType.PowerPointSlide:
                case OleObjectType.WordMacroDocument:
                    WriteNativeData(nativeData, DEF_PACKAGE_STREAM_NAME);
                    break;
                case OleObjectType.WordPadDocument:
                    WriteNativeData(nativeData, DEF_WP_STREAM_NAME);
                    break;
                case OleObjectType.AdobeAcrobatDocument:
                    WriteNativeData(nativeData, DEF_CONTENT_STREAM_NAME);
                    break;
                case OleObjectType.ExcelChart:
                case OleObjectType.Excel_97_2003_Worksheet:
                case OleObjectType.PowerPoint_97_2003_Presentation:
                case OleObjectType.PowerPoint_97_2003_Slide:
                case OleObjectType.Word_97_2003_Document:
                case OleObjectType.VisioDrawing:
                case OleObjectType.OpenOfficeSpreadsheet:
                case OleObjectType.OpenOfficeText:
                case OleObjectType.OpenOfficeSpreadsheet1_1:
                case OleObjectType.OpenOfficeText_1_1:
                    MemoryStream stream = new MemoryStream(nativeData);
                    WriteNativeStreams(stream);
                    break;
                case OleObjectType.Equation:
                    WriteNativeData(nativeData, DEF_EQUATION_STREAM_NAME);
                    break;
                case OleObjectType.GraphChart:
                    WriteNativeData(nativeData, DEF_WORKBOOK_STREAM_NAME);
                    break;
                case OleObjectType.OpenDocumentPresentation:
                case OleObjectType.OpenDocumentSpreadsheet:
                    WriteNativeData(nativeData, DEF_ODP_STREAM_NAME);
                    break;
                case OleObjectType.BitmapImage:
                    WritePBrush(nativeData);
                    break;
                case OleObjectType.Package:
                    WritePackage(nativeData, dataPath);
                    break;
            }
        }
        /// <summary>
        /// Writes the native data.
        /// </summary>
        /// <param name="nativeData">The native data.</param>
        /// <param name="streamName">Name of the stream.</param>
        private void WriteNativeData(byte[] nativeData, String streamName)
        {
            MemoryStream stream = new MemoryStream(nativeData);
            stream.Position = 0;
            Storage.Streams.Add(streamName, stream);
        }
        /// <summary>
        /// Writes the embedded drawing.
        /// </summary>
        /// <param name="nativeData">The native data.</param>
        private void WritePBrush(byte[] nativeData)
        {
            int iOffset = 0;
            byte[] buffer = new byte[nativeData.Length + 4];
            DataStructure.WriteInt32(buffer, ref iOffset, nativeData.Length);
            DataStructure.WriteBytes(buffer, ref iOffset, nativeData);

            MemoryStream stream = new MemoryStream(buffer);
            stream.Position = 0;
            Storage.Streams.Add(DEF_NATIVE_STREAM_NAME, stream);
        }
        /// <summary>
        /// Writes the native streams.
        /// </summary>
        /// <param name="stream">The stream.</param>
        private void WriteNativeStreams(Stream stream)
        {
            Syncfusion.CompoundFile.DocIO.Net.CompoundFile nativeStream = new Syncfusion.CompoundFile.DocIO.Net.CompoundFile(stream);
            String[] streamNames = nativeStream.RootStorage.Streams;

            for (int i = 0, cnt = streamNames.Length; i < cnt; i++)
            {
                CompoundStream dataStream = nativeStream.RootStorage.OpenStream(streamNames[i]);
                byte[] bytes = new byte[dataStream.Length];
                dataStream.Read(bytes, 0, bytes.Length);
                dataStream.Dispose();
                Storage.Streams.Add(streamNames[i], new MemoryStream(bytes));
            }
            nativeStream.Dispose();
        }
        /// <summary>
        /// Writes the "CompObj" stream.
        /// </summary>
        /// <param name="objType">Type of the obj.</param>
        private void WriteCompObjStream(OleObjectType objType)
        {
            switch (objType)
            {
                case OleObjectType.AdobeAcrobatDocument:
                case OleObjectType.WordDocument:
                case OleObjectType.ExcelBinaryWorksheet:
                case OleObjectType.ExcelMacroWorksheet:
                case OleObjectType.ExcelWorksheet:
                case OleObjectType.Excel_97_2003_Worksheet:
                case OleObjectType.Equation:
                case OleObjectType.ExcelChart:
                case OleObjectType.GraphChart:
                case OleObjectType.PowerPoint_97_2003_Presentation:
                case OleObjectType.PowerPoint_97_2003_Slide:
                case OleObjectType.PowerPointMacroPresentation:
                case OleObjectType.PowerPointMacroSlide:
                case OleObjectType.PowerPointSlide:
                case OleObjectType.PowerPointPresentation:
                case OleObjectType.Word_97_2003_Document:
                case OleObjectType.WordMacroDocument:
                case OleObjectType.VisioDrawing:
                case OleObjectType.OpenDocumentPresentation:
                case OleObjectType.OpenDocumentSpreadsheet:
                case OleObjectType.OpenOfficeSpreadsheet:
                case OleObjectType.OpenOfficeText:
                case OleObjectType.OpenOfficeSpreadsheet1_1:
                case OleObjectType.OpenOfficeText_1_1:
                case OleObjectType.BitmapImage:
                case OleObjectType.Package:
                    if (!Storage.Streams.ContainsKey(DEF_COMP_STREAM_NAME))
                    {
                        MemoryStream stream = new MemoryStream();
                        CompObjectStream compObjectStream = new CompObjectStream(objType);
                        compObjectStream.SaveTo(stream);
                        stream.Flush();
                        stream.Position = 0;
                        Storage.Streams.Add(DEF_COMP_STREAM_NAME, stream);
                    }
                    break;
            }
        }
        /// <summary>
        /// Writes the "CompObj" stream.
        /// </summary>
        /// <param name="objType">Type of the obj.</param>
        /// <param name="dataPath">The data path.</param>
        private void WriteLinkInfoStream(OleObjectType objType, String dataPath)
        {
            MemoryStream stream = new MemoryStream();
            LinkInfoStream linkInfoStream = new LinkInfoStream(dataPath);
            linkInfoStream.SaveTo(stream);
            stream.Flush();
            stream.Position = 0;
            Storage.Streams.Add(DEF_LINK_INFO_STREAM_NAME, stream);
        }
        /// <summary>
        /// Writes the "Ole" stream.
        /// </summary>
        /// <param name="linkType">Type of the link.</param>
        /// <param name="objType">Type of the obj.</param>
        /// <param name="dataPath">The data path.</param>
        private void WriteOleStream(OleLinkType linkType, OleObjectType objType, String dataPath)
        {
            switch( objType )
            {
                case OleObjectType.AdobeAcrobatDocument:
                case OleObjectType.ExcelBinaryWorksheet:
                case OleObjectType.ExcelMacroWorksheet:
                case OleObjectType.ExcelWorksheet:
                case OleObjectType.Excel_97_2003_Worksheet:
                case OleObjectType.Equation:
                case OleObjectType.ExcelChart:
                case OleObjectType.GraphChart:
                case OleObjectType.PowerPoint_97_2003_Presentation:
                case OleObjectType.PowerPoint_97_2003_Slide:
                case OleObjectType.PowerPointMacroPresentation:
                case OleObjectType.PowerPointMacroSlide:
                case OleObjectType.PowerPointSlide:
                case OleObjectType.VisioDrawing:
                case OleObjectType.OpenDocumentPresentation:
                case OleObjectType.OpenDocumentSpreadsheet:
                case OleObjectType.OpenOfficeText:
                case OleObjectType.OpenOfficeSpreadsheet1_1:
                case OleObjectType.OpenOfficeText_1_1:
                case OleObjectType.WordPadDocument:
                case OleObjectType.BitmapImage:
                case OleObjectType.Package:
                    MemoryStream stream = new MemoryStream();
                    OLEStream oleStream = new OLEStream(linkType, dataPath);
                    oleStream.SaveTo(stream);
                    stream.Flush();
                    stream.Position = 0;
                    Storage.Streams.Add(DEF_OLE_STREAM_NAME, stream);
                    break;
            }
        }
        /// <summary>
        /// Writes the "ObjInfo" stream.
        /// </summary>
        /// <param name="linkType">Type of the link.</param>
        /// <param name="objType">Type of the obj.</param>
        private void WriteObjInfoStream(OleLinkType linkType, OleObjectType objType)
        {
            MemoryStream stream = new MemoryStream();
            ObjectInfoStream objectInfoStream = new ObjectInfoStream();
            objectInfoStream.SaveTo(stream, linkType, objType);
            stream.Flush();
            stream.Position = 0;
            Storage.Streams.Add(DEF_INFO_STREAM_NAME, stream);
        }

        /// <summary>
        /// Writes the package.
        /// </summary>
        /// <param name="nativeData">The native data.</param>
        /// <param name="dataPath">The data path.</param>
        private void WritePackage(byte[] nativeData, string dataPath)
        {
            Encoding asciiEnc =
#if !SILVERLIGHT && !WP && !WINRT
                new ASCIIEncoding();
#else
                new UTF8Encoding();
#endif
            string fileName = Path.GetFileName(dataPath);
            byte[] nameBytes = asciiEnc.GetBytes(fileName);
            byte[] pathBytes = asciiEnc.GetBytes(dataPath);
            byte[] marker1 = new byte[2] { 2, 0 };
            byte[] marker2 = new byte[4] { 0, 0, 3, 0 };             

            // Data length 
            int dataLen = Constants.BytesInInt;
            // 2-bytes marker
            dataLen += marker1.Length;
            // File name length + "\0"
            dataLen += nameBytes.Length + 1;
            // File path length + "\0"
            dataLen += pathBytes.Length + 1;
            // 4-bytes marker
            dataLen += marker2.Length;
            // "DOS" file path length
            dataLen += Constants.BytesInInt;
            // "DOS" file path + "\0"
            dataLen += pathBytes.Length + 1;
            // Native data length
            dataLen += Constants.BytesInInt;
            dataLen += nativeData.Length;
            // "\0\0" at the end
            dataLen += 2;

            int iOffset = 0;
            byte[] data = new byte[dataLen];
            // Write data length
            DataStructure.WriteInt32(data, ref iOffset, dataLen - Constants.BytesInInt);
            // Write marker 1
            DataStructure.WriteBytes(data, ref iOffset, marker1);
            // Write file name
            DataStructure.WriteBytes(data, ref iOffset, nameBytes);
            iOffset += 1; // "\0"
            // Write path
            DataStructure.WriteBytes(data, ref iOffset, pathBytes);
            iOffset += 1; // "\0"
            // Write marker 2
            DataStructure.WriteBytes(data, ref iOffset, marker2);
            // Write path name length
            DataStructure.WriteInt32(data, ref iOffset, pathBytes.Length + 1);
            // Write path
            DataStructure.WriteBytes(data, ref iOffset, pathBytes);
            iOffset += 1; // "\0"
            // Write native data length
            DataStructure.WriteInt32(data, ref iOffset, nativeData.Length);
            // Write native data
            DataStructure.WriteBytes(data, ref iOffset, nativeData);
           
            // Write data to stream
            Storage.Streams.Add(DEF_NATIVE_STREAM_NAME, new MemoryStream(data));
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal OLEObject Clone()
        {
            OLEObject oleObject = new OLEObject();
            oleObject.m_guid = m_guid;
            oleObject.m_oleType = m_oleType;
            oleObject.m_storage = m_storage.Clone();
            return oleObject;
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal void Close()
        {
            m_storage.Close();
        }
        #endregion
    }

    /// <summary>
    /// Class performs converting string to OleObjectType enum and vice versa.
    /// </summary>
    internal class OleTypeConvertor
    {
        /// <summary>
        /// Converts the string to "OleObjectType"
        /// </summary>
        /// <param name="oleTypeStr">The OLE type STR.</param>
        /// <returns></returns>
        internal static OleObjectType ToOleType(string oleTypeStr)
        {
            oleTypeStr = oleTypeStr.TrimEnd('\0');
            OleObjectType oleType = OleObjectType.Undefined;
            if (oleTypeStr.StartsWith("Acrobat Document") || oleTypeStr.StartsWith("AcroExch.Document.7"))
                oleType = OleObjectType.AdobeAcrobatDocument;
            else if (oleTypeStr.StartsWith("Package"))
                oleType = OleObjectType.Package;
            else if (oleTypeStr.StartsWith("PBrush"))
                oleType = OleObjectType.BitmapImage;
            else if (oleTypeStr.StartsWith("Media Clip") || oleTypeStr.StartsWith("MPlayer"))
                oleType = OleObjectType.MediaClip;
            else if (oleTypeStr.StartsWith("Microsoft Equation 3.0") || oleTypeStr.StartsWith("Equation.3"))
                oleType = OleObjectType.Equation;
            else if (oleTypeStr.StartsWith("Microsoft Graph Chart") || oleTypeStr.StartsWith("MSGraph.Chart.8"))
                oleType = OleObjectType.GraphChart;
            else if (oleTypeStr.Contains("Excel 2003 Worksheet") || oleTypeStr.StartsWith("Excel.Sheet.8"))
                oleType = OleObjectType.Excel_97_2003_Worksheet;
            else if (oleTypeStr.Contains("Excel Binary Worksheet") || oleTypeStr.StartsWith("Excel.SheetBinaryMacroEnabled.12"))
                oleType = OleObjectType.ExcelBinaryWorksheet;
            else if (oleTypeStr.Contains("Excel Chart") || oleTypeStr.StartsWith("Excel.Chart.8"))
                oleType = OleObjectType.ExcelChart;
            else if (oleTypeStr.Contains("Excel Worksheet (code)") || oleTypeStr.StartsWith("Excel.SheetMacroEnabled.12"))
                oleType = OleObjectType.ExcelMacroWorksheet;
            else if (oleTypeStr.Contains("Excel Worksheet") || oleTypeStr.StartsWith("Excel.Sheet.12"))
                oleType = OleObjectType.ExcelWorksheet;
            else if (oleTypeStr.Contains("PowerPoint 97-2003 Presentation") || oleTypeStr.StartsWith("PowerPoint.Show.8"))
                oleType = OleObjectType.PowerPoint_97_2003_Presentation;
            else if (oleTypeStr.Contains("PowerPoint 97-2003 Slide") || oleTypeStr.StartsWith("PowerPoint.Slide.8"))
                oleType = OleObjectType.PowerPoint_97_2003_Slide;
            else if (oleTypeStr.Contains("PowerPoint Macro-Enabled Presentation") || oleTypeStr.StartsWith("PowerPoint.ShowMacroEnabled.12"))
                oleType = OleObjectType.PowerPointMacroPresentation;
            else if (oleTypeStr.Contains("PowerPoint Macro-Enabled Slide") || oleTypeStr.StartsWith("PowerPoint.SlideMacroEnabled.12"))
                oleType = OleObjectType.PowerPointMacroSlide;
            else if (oleTypeStr.Contains("PowerPoint Presentation") || oleTypeStr.StartsWith("PowerPoint.Show.12"))
                oleType = OleObjectType.PowerPointPresentation;
            else if (oleTypeStr.Contains("PowerPoint Slide") || oleTypeStr.StartsWith("PowerPoint.Slide.12"))
                oleType = OleObjectType.PowerPointSlide;
            else if (oleTypeStr.Contains("Word 97-2003 Document") || oleTypeStr.StartsWith("Word.Document.8"))
                oleType = OleObjectType.Word_97_2003_Document;
            else if (oleTypeStr.Contains("Word Document") || oleTypeStr.StartsWith("Word.Document.12"))
                oleType = OleObjectType.WordDocument;
            else if (oleTypeStr.Contains("Word Macro-Enabled Document") || oleTypeStr.StartsWith("Word.DocumentMacroEnabled.12"))
                oleType = OleObjectType.WordMacroDocument;
            else if (oleTypeStr.StartsWith("Microsoft Visio Drawing") || oleTypeStr.StartsWith("Visio.Drawing.11"))
                oleType = OleObjectType.VisioDrawing;
            else if (oleTypeStr.StartsWith("OpenDocument Presentation") || oleTypeStr.StartsWith("PowerPoint.OpenDocumentPresentation.12"))
                oleType = OleObjectType.OpenDocumentPresentation;
            else if (oleTypeStr.StartsWith("OpenDocument Spreadsheet") || oleTypeStr.StartsWith("Excel.OpenDocumentSpreadsheet.12"))
                oleType = OleObjectType.OpenDocumentSpreadsheet;
            else if (oleTypeStr.StartsWith("opendocument.CalcDocument.1"))
                oleType = OleObjectType.OpenOfficeSpreadsheet;
            else if (oleTypeStr.StartsWith("opendocument.WriterDocument.1"))
                oleType = OleObjectType.OpenOfficeText;
            else if (oleTypeStr.StartsWith("soffice.StarCalcDocument.6"))
                oleType = OleObjectType.OpenOfficeSpreadsheet1_1;
            else if (oleTypeStr.StartsWith("soffice.StarWriterDocument.6"))
                oleType = OleObjectType.OpenOfficeText_1_1;
            else if (oleTypeStr.StartsWith("Video Clip") || oleTypeStr.StartsWith("AVIFile"))
                oleType = OleObjectType.VideoClip;
            else if( oleTypeStr.StartsWith("WaveSound") || oleTypeStr.StartsWith("SoundRec"))
                oleType = OleObjectType.WaveSound;
            else if ( oleTypeStr.StartsWith("WordPad Document") || oleTypeStr.StartsWith("WordPad.Document.1"))
                oleType = OleObjectType.WordPadDocument;

            return oleType;
        }
        /// <summary>
        /// Converts the string to "OleObjectType"
        /// </summary>
        /// <param name="oleTypeStr">The OLE type STR.</param>
        /// <returns></returns>
        internal static string ToString(OleObjectType oleType, bool isWord2003 )
        {
            string strOleType = string.Empty;
            switch (oleType)
            {
                case OleObjectType.AdobeAcrobatDocument:
                    strOleType = isWord2003 ? "Acrobat Document" : "AcroExch.Document.7";
                    break;
                case OleObjectType.Package:
                    strOleType = "Package";
                    break;
                case OleObjectType.BitmapImage:
                    strOleType = "PBrush";
                    break;
                case OleObjectType.MediaClip:
                    strOleType = isWord2003 ? "Media Clip" : "MPlayer";
                    break;
                case OleObjectType.Equation:
                    strOleType = isWord2003 ? "Microsoft Equation 3.0" : "Equation.3";
                    break;
                case OleObjectType.GraphChart:
                    strOleType = isWord2003 ? "Microsoft Graph Chart" : "MSGraph.Chart.8";
                    break;
                case OleObjectType.Excel_97_2003_Worksheet:
                    strOleType = isWord2003 ? "Microsoft Office Excel 2003 Worksheet" : "Excel.Sheet.8";
                    break;
                case OleObjectType.ExcelBinaryWorksheet:
                    strOleType = isWord2003 ? "Microsoft Office Excel Binary Worksheet" : "Excel.SheetBinaryMacroEnabled.12";
                    break;
                case OleObjectType.ExcelChart:
                    strOleType = isWord2003 ? "Microsoft Office Excel Chart" : "Excel.Chart.8";
                    break;
                case OleObjectType.ExcelMacroWorksheet:
                    strOleType = isWord2003 ? "Microsoft Office Excel Worksheet (code)" : "Excel.SheetMacroEnabled.12";
                    break;
                case OleObjectType.ExcelWorksheet:
                    strOleType = isWord2003 ? "Microsoft Office Excel Worksheet" : "Excel.Sheet.12";
                    break;
                case OleObjectType.PowerPoint_97_2003_Presentation:
                    strOleType = isWord2003 ? "Microsoft Office PowerPoint 97-2003 Presentation" : "PowerPoint.Show.8";
                    break;
                case OleObjectType.PowerPoint_97_2003_Slide:
                    strOleType = isWord2003 ? "Microsoft Office PowerPoint 97-2003 Slide" : "PowerPoint.Slide.8";
                    break;
                case OleObjectType.PowerPointMacroPresentation:
                    strOleType = isWord2003 ? "Microsoft Office PowerPoint Macro-Enabled Presentation" : "PowerPoint.ShowMacroEnabled.12";
                    break;
                case OleObjectType.PowerPointMacroSlide:
                    strOleType = isWord2003 ? "Microsoft Office PowerPoint Macro-Enabled Slide" : "PowerPoint.SlideMacroEnabled.12";
                    break;
                case OleObjectType.PowerPointPresentation:
                    strOleType = isWord2003 ? "Microsoft Office PowerPoint Presentation" : "PowerPoint.Show.12";
                    break;
                case OleObjectType.PowerPointSlide:
                    strOleType = isWord2003 ? "Microsoft Office PowerPoint Slide" : "PowerPoint.Slide.12";
                    break;
                case OleObjectType.Word_97_2003_Document:
                    strOleType = isWord2003 ? "Microsoft Office Word 97-2003 Document" : "Word.Document.8";
                    break;
                case OleObjectType.WordDocument:
                    strOleType = isWord2003 ? "Microsoft Office Word Document" : "Word.Document.12";
                    break;
                case OleObjectType.WordMacroDocument:
                    strOleType = isWord2003 ? "Microsoft Office Word Macro-Enabled Document" : "Word.DocumentMacroEnabled.12";
                    break;
                case OleObjectType.VisioDrawing:
                    strOleType = isWord2003 ? "Microsoft Visio Drawing" : "Visio.Drawing.11";
                    break;
                case OleObjectType.OpenDocumentPresentation:
                    strOleType = isWord2003 ? "OpenDocument Presentation" : "PowerPoint.OpenDocumentPresentation.12";
                    break;
                case OleObjectType.OpenDocumentSpreadsheet:
                    strOleType = isWord2003 ? "OpenDocument Spreadsheet" : "Excel.OpenDocumentSpreadsheet.12";
                    break;
                case OleObjectType.OpenOfficeSpreadsheet:
                    strOleType = "opendocument.CalcDocument.1";
                    break;
                case OleObjectType.OpenOfficeText:
                    strOleType = "opendocument.WriterDocument.1";
                    break;
                case OleObjectType.OpenOfficeSpreadsheet1_1:
                    strOleType = "soffice.StarCalcDocument.6";
                    break;
                case OleObjectType.OpenOfficeText_1_1:
                    strOleType = "soffice.StarWriterDocument.6";
                    break;
                case OleObjectType.VideoClip:
                    strOleType = isWord2003 ? "Video Clip" : "AVIFile";
                    break;
                case OleObjectType.WaveSound:
                    strOleType = isWord2003 ? "WaveSound" : "SoundRec";
                    break;
                case OleObjectType.WordPadDocument:
                    strOleType = isWord2003 ? "WordPad Document" : "WordPad.Document.1";
                    break;
                case OleObjectType.MIDISequence:
                    strOleType = "MIDI Sequence";
                    break;
            }
            return strOleType;
        }
        /// <summary>
        /// Gets the GUID for specified type of object.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        internal static Guid GetGUID(OleObjectType type)
        {
            Guid guid = Guid.NewGuid();
            string strGuid = null;

            switch (type)
            {
                case OleObjectType.AdobeAcrobatDocument:
                    strGuid = "b801ca65-a1fc-11d0-85ad-444553540000";
                    break;
                case OleObjectType.Equation:
                    strGuid = "0002ce02-0000-0000-c000-000000000046";
                    break;
                case OleObjectType.GraphChart:
                    strGuid = "00020803-0000-0000-c000-000000000046";
                    break;
                case OleObjectType.Excel_97_2003_Worksheet:
                    strGuid = "00020820-0000-0000-c000-000000000046";
                    break;
                case OleObjectType.ExcelChart:
                    strGuid = "00020821-0000-0000-c000-000000000046";
                    break;
                case OleObjectType.ExcelWorksheet:
                    strGuid = "00020830-0000-0000-c000-000000000046";
                    break;
                case OleObjectType.ExcelMacroWorksheet:
                    strGuid = "00020832-0000-0000-c000-000000000046";
                    break;
                case OleObjectType.ExcelBinaryWorksheet:
                    strGuid = "00020833-0000-0000-c000-000000000046";
                    break;
                case OleObjectType.PowerPoint_97_2003_Presentation:
                    strGuid = "64818d10-4f9b-11cf-86ea-00aa00b929e8";
                    break;
                case OleObjectType.PowerPoint_97_2003_Slide:
                    strGuid = "64818d11-4f9b-11cf-86ea-00aa00b929e8";
                    break;
                case OleObjectType.PowerPointMacroPresentation:
                    strGuid = "dc020317-e6e2-4a62-b9fa-b3efe16626f4";
                    break;
                case OleObjectType.PowerPointMacroSlide:
                    strGuid = "3c18eae4-bc25-4134-b7df-1eca1337dddc";
                    break;
                case OleObjectType.PowerPointPresentation:
                    strGuid = "cf4f55f4-8f87-4d47-80bb-5808164bb3f8";
                    break;
                case OleObjectType.PowerPointSlide:
                    strGuid = "048eb43e-2059-422f-95e0-557da96038af";
                    break;
                case OleObjectType.WordDocument:
                    strGuid = "f4754c9b-64f5-4b40-8af4-679732ac0607";
                    break;
                case OleObjectType.Word_97_2003_Document:
                    strGuid = "00020906-0000-0000-c000-000000000046";
                    break;
                case OleObjectType.WordMacroDocument:
                    strGuid = "18a06b6b-2f3f-4e2b-a611-52be631b2d22";
                    break;
                case OleObjectType.VisioDrawing:
                    strGuid = "00021a14-0000-0000-c000-000000000046";
                    break;
                case OleObjectType.OpenDocumentPresentation:
                    strGuid = "c282417b-2662-44b8-8a94-3bff61c50900";
                    break;
                case OleObjectType.OpenDocumentSpreadsheet:
                    strGuid = "eabcecdb-cc1c-4a6f-b4e3-7f888a5adfc8";
                    break;
                case OleObjectType.OpenOfficeSpreadsheet:
                    strGuid = "7fa8ae11-b3e3-4d88-aabf-255526cd1ce8";
                    break;
                case OleObjectType.OpenOfficeText:
                    strGuid = "f616b81f-7bb8-4f22-b8a5-47428d59f8ad";
                    break;
                case OleObjectType.OpenOfficeSpreadsheet1_1:
                    strGuid = "7b342dc4-139a-4a46-8a93-db0827ccee9c";
                    break;
                case OleObjectType.OpenOfficeText_1_1:
                    strGuid = "30a2652a-ddf7-45e7-aca6-3eab26fc8a4e";
                    break;
                case OleObjectType.WordPadDocument:
                    strGuid = "73fddc80-aea9-101a-98a7-00aa00374959";
                    break;
                case OleObjectType.BitmapImage:
                    strGuid = "0003000a-0000-0000-c000-000000000046";
                    break;
                case OleObjectType.Package:
                    strGuid = "0003000c-0000-0000-c000-000000000046";
                    break;
                case OleObjectType.MIDISequence:
                    strGuid = "00022603-0000-0000-c000-000000000046";
                    break;
            }            

            if (strGuid != null)
            {
                guid = new Guid(strGuid);
            }
            return guid;
        }
    }

    /// <summary>
    /// Class specifies storage with sub storages and streams.
    /// </summary>
    internal class Storage
    {
        #region Fields
        private Dictionary<string, Stream> m_streams = new Dictionary<string, Stream>();
        private List<Storage> m_storages = new List<Storage>();
        private string m_storageName;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the name of the storage.
        /// </summary>
        /// <value>The name of the storage.</value>
        internal string StorageName
        {
            get
            {
                return m_storageName;
            }
            set
            {
                m_storageName = value;
            }
        }
        /// <summary>
        /// Gets the streams.
        /// </summary>
        /// <value>The streams.</value>
        internal Dictionary<string, Stream> Streams
        {
            get
            {
                return m_streams;
            }
        }
        /// <summary>
        /// Gets the storages.
        /// </summary>
        /// <value>The storages.</value>
        internal List<Storage> Storages
        {
            get
            {
                return m_storages;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Storage"/> class.
        /// </summary>
        /// <param name="storageName">Name of the storage.</param>
        internal Storage(string storageName)
        {
            m_storageName = storageName;
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Parses the storages.
        /// </summary>
        /// <param name="storage">The storage.</param>
        internal void ParseStorages(ICompoundStorage storage)
        {
            foreach (string storageName in storage.Storages)
            {
                ICompoundStorage subStorage = storage.OpenStorage(storageName);
                Storage newStorage = new Storage(storageName);
                newStorage.ParseStorages(subStorage);
                newStorage.ParseStreams(subStorage);
                Storages.Add(newStorage);
            }
        }
        /// <summary>
        /// Parses the streams.
        /// </summary>
        /// <param name="storage">The storage.</param>
        internal void ParseStreams(ICompoundStorage storage)
        {
            foreach (string streamName in storage.Streams)
            {
                CompoundStream cmpStream = storage.OpenStream(streamName);
                byte[] arrBuffer = new byte[cmpStream.Length];
                cmpStream.Read(arrBuffer, 0, arrBuffer.Length);
                cmpStream.Dispose();
                Streams.Add(streamName, new MemoryStream(arrBuffer));
            }
        }
        /// <summary>
        /// Writes to storage.
        /// </summary>
        /// <param name="storage">The storage.</param>
        internal void WriteToStorage(ICompoundStorage storage)
        {
            foreach (Storage subStorage in Storages)
            {
                ICompoundStorage newStorage = storage.CreateStorage(subStorage.StorageName);
                subStorage.WriteToStorage(newStorage);
            }
            foreach (KeyValuePair<string, Stream> keyValue in Streams)
            {
                CompoundStream cmpStream = storage.CreateStream(keyValue.Key);
                cmpStream.Write((keyValue.Value as MemoryStream).ToArray(), 0, (int)keyValue.Value.Length);
                cmpStream.Flush();
            }
            storage.Flush();
        }
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal Storage Clone()
        {
            Storage newStorage = new Storage(StorageName);
            foreach (KeyValuePair<string, Stream> keyValue in Streams)
            {
                MemoryStream newStream = new MemoryStream((keyValue.Value as MemoryStream).ToArray());
                newStorage.Streams.Add(keyValue.Key, newStream);
            }
            foreach (Storage storage in Storages)
            {
                newStorage.Storages.Add(storage.Clone());
            }
            return newStorage;
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal void Close()
        {
            foreach (KeyValuePair<string, Stream> keyValue in Streams)
            {
#if WINRT
                keyValue.Value.Dispose();
#else
                keyValue.Value.Close();
#endif
            }
            Streams.Clear();
            foreach (Storage storage in Storages)
            {
                storage.Close();
            }
            Storages.Clear();
        }
        #endregion
    }
 }
