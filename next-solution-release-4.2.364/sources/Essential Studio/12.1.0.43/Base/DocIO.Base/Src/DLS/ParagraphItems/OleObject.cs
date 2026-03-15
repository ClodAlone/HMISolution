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


#region File using directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Syncfusion.Layouting;
using Syncfusion.CompoundFile.DocIO.Native;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.OLEObject;
using Syncfusion.CompoundFile.DocIO.Net;
using Syncfusion.CompoundFile.DocIO;
using Syncfusion.DocIO.ReaderWriter;
#if !SILVERLIGHT && !WP
using Syncfusion.DocIO.Rendering;
#endif
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for WOleObject.
    /// </summary>
    public class WOleObject
      : ParagraphItem
#if !SILVERLIGHT && !WP
        , ILeafWidget
#endif
    {
        #region Constants
        private const string DEF_OBJECT_POOL_NAME = "ObjectPool";
        private const string DEF_INFO_STREAM_NAME = "ObjInfo";
        private const int DEF_STRUCT_SIZE = 6;
        #endregion

        #region Fields
        internal WPicture m_picture;
        private WField m_field;
        private string m_oleStorageName;
        private string m_linkAddress;
        private string m_strObjType;
        private OleObjectType m_oleObjType;
        internal OleLinkType m_linkType;
        private XmlParagraphItem m_oleXmlItem;
        private OLEObject m_oleObject;
        private static Random m_oleRandomIdGen;
        private string m_packageFileName = string.Empty;
        private bool m_displayAsIcon = true;
        #endregion

        #region Properties
        /// <summary>
        /// Gets/Sets whether the OLEObject is displayed as an Icon or Content. If True, the OLEObject is displayed as an icon
        /// </summary>
        /// <value>bool</value>
        public bool DisplayAsIcon
        {
            get
            {
                return m_displayAsIcon;
            }
            set
            {
                m_displayAsIcon = value;
                if (!this.Document.IsOpening)
                    UpdateOleObjInfoStream();
            }
        }
        /// <summary>
        /// Gets the OLE picture.
        /// </summary>
        /// <value>The OLE picture.</value>
        public WPicture OlePicture
        {
            get
            {
                return m_picture;
            }
        }
        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>The type of the entity.</value>
        public override EntityType EntityType
        {
            get
            {
                return EntityType.OleObject;
            }
        }
        /// <summary>
        /// Gets the OLE container.
        /// </summary>
        /// <value>The container.</value>
        public Stream Container
        {
            get
            {
                return GetOleContainer();
            }
        }
        /// <summary>
        /// Gets/sets the field.
        /// </summary>
        /// <value>The field.</value>
        internal WField Field
        {
            get
            {
                if (m_field == null)
                    m_field = new WField(m_doc);

                if (m_field.FieldType == FieldType.FieldNone)
                    SetFieldType();

                m_field.SetOwner(this);
                return m_field;
            }
            set
            {
                m_field = value;
            }
        }
        /// <summary>
        /// Gets or sets the name of the OLE Object storage.
        /// </summary>
        /// <value>The name of the OLE storage.</value>
        public string OleStorageName
        {
            get
            {
                if (string.IsNullOrEmpty(m_oleStorageName))
                {
                    m_oleStorageName = (new Random()).Next().ToString();
                }
                return m_oleStorageName;
            }
            set
            {
                m_oleStorageName = value;
            }
        }
        /// <summary>
        /// Gets or sets the link path.
        /// </summary>
        /// <value>The link address.</value>
        public string LinkPath
        {
            get
            {
                if (string.IsNullOrEmpty(m_linkAddress))
                {
                    UpdateProps();
                }
                return m_linkAddress;
            }
            set
            {
#if !SILVERLIGHT && !WP && !WINRT
                value = System.IO.Path.GetFullPath(value);
#endif
                m_linkAddress = value;
            }
        }
        /// <summary>
        /// Gets the type of the OLE object.
        /// </summary>
        /// <value>The type of the OLE obj.</value>
        public OleLinkType LinkType
        {
            get
            {
                return m_linkType;
            }
        }
        /// <summary>
        /// Gets or sets the not parsed from docx ole object.
        /// </summary>
        /// <value>The XmlParagraphItem.</value>
        internal XmlParagraphItem OleXmlItem
        {
            get
            {
                return m_oleXmlItem;
            }
            set
            {
                m_oleXmlItem = value;
            }
        }
        /// <summary>
        /// Gets the type of the OLE object.
        /// </summary>
        /// <value>The type of the OLE object.</value>
        internal OleObjectType OleObjectType
        {
            get
            {
                m_oleObjType = (m_oleObjType == OleObjectType.Undefined) ? OleObject.OleType : m_oleObjType;
                if (m_oleObjType == OleObjectType.Undefined)
                    UpdateProps();
                return m_oleObjType;
            }
            set
            {
                m_oleObjType = value;
            }
        }
        /// <summary>
        /// Gets or sets the type of the OLE object.
        /// </summary>
        /// <value>The type of the object.</value>
        public string ObjectType
        {
            get
            {
                return (m_strObjType == null) ?
                    OleTypeConvertor.ToString(OleObjectType, false) : m_strObjType;
            }
            set
            {
                m_strObjType = value;
                m_oleObjType = OleTypeConvertor.ToOleType(value);
            }
        }
        /// <summary>
        /// Gets the native data of embedded OLE object.
        /// </summary>
        /// <value>The native data.</value>
        public byte[] NativeData
        {
            get
            {
                if (!IsEmpty)
                {
                    return (GetOlePartStream() as MemoryStream).ToArray();
                }
                return null;
            }
        }
        /// <summary>
        /// Gets the OLE object.
        /// </summary>
        /// <value>The OLE object.</value>
        private OLEObject OleObject
        {
            get
            {
                if (m_oleObject == null)
                    m_oleObject = new OLEObject();
                return m_oleObject;
            }
        }
        /// <summary>
        /// Gets the next OLE object id.
        /// </summary>
        /// <value>The next OLE obj id.</value>
        internal static int NextOleObjId
        {
            get
            {
                if( m_oleRandomIdGen == null )
                {
                    m_oleRandomIdGen = new Random( ( new DateTime() ).Millisecond );
                }
                return m_oleRandomIdGen.Next();
            }
        }
        /// <summary>
        /// Gets the name of file embedded in the package (only if OleType is "Package").
        /// </summary>
        public String PackageFileName
        {
            get
            {
                return m_packageFileName; 
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        internal bool IsEmpty
        {
            get
            {
                return OleObject.Storage.Streams.Count == 0;
            }
        }
        /// <summary>
        /// Gets the GUID.
        /// </summary>
        /// <value>The GUID.</value>
        internal Guid Guid
        {
            get
            {
                if (OleObject.Guid == null || OleObject.Guid == Guid.Empty)
                    return OleTypeConvertor.GetGUID(OleObjectType);
                return OleObject.Guid;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the OleObject class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        public WOleObject(WordDocument doc)
            : base(doc)
        {
            m_oleStorageName = string.Empty;
            m_linkAddress = string.Empty;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Parses the object pool.
        /// </summary>
        /// <param name="objectPoolStream">The object pool stream.</param>
        internal void ParseObjectPool(Stream objectPoolStream)
        {
            OleObject.Storage.Streams.Clear();
            using (Syncfusion.CompoundFile.DocIO.Net.CompoundFile ole = new CompoundFile.DocIO.Net.CompoundFile(objectPoolStream))
            {
                foreach (DirectoryEntry entry in ole.Directory.Entries)
                {
                    if ("_" + OleStorageName == entry.Name)
                    {
                        OleObject.Guid = entry.StorageGuid;
                        break;
                    }
                }
                if ((Array.IndexOf(ole.RootStorage.Storages, DEF_OBJECT_POOL_NAME) != -1)
                    && (Array.IndexOf(ole.RootStorage.OpenStorage(DEF_OBJECT_POOL_NAME).Storages, "_" + OleStorageName) != -1))
                {
                    ICompoundStorage storage = ole.RootStorage.OpenStorage(DEF_OBJECT_POOL_NAME);
                    storage = storage.OpenStorage("_" + OleStorageName);
                    ParseStreams(storage);
                    OleObject.Storage.ParseStorages(storage);
                    CheckObjectInfoStream();
                }
            }
            objectPoolStream.Position = 0;
        }
        /// <summary>
        /// Parses the OLE part stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void ParseOlePartStream(Stream stream)
        {
            OleObject.Storage.Streams.Clear();
            if (CompoundFile.DocIO.Net.CompoundFile.CheckHeader(stream))
            {
                Syncfusion.CompoundFile.DocIO.Net.CompoundFile srcFile = new Syncfusion.CompoundFile.DocIO.Net.CompoundFile(stream);
                foreach (DirectoryEntry entry in srcFile.Directory.Entries)
                {
                    if (srcFile.RootStorage.Name == entry.Name)
                    {
                        OleObject.Guid = entry.StorageGuid;
                        break;
                    }
                }
                ParseStreams(srcFile.RootStorage);
                OleObject.Storage.ParseStorages(srcFile.RootStorage);
                CheckObjectInfoStream();
                srcFile.Dispose();
            }
            else
            {
                OleObject.Save(new MemoryStream((stream as MemoryStream).ToArray()), this);
            }
        }
        /// <summary>
        /// Parses the streams.
        /// </summary>
        /// <param name="storage">The storage.</param>
        private void ParseStreams(ICompoundStorage storage)
        {
            foreach (string streamName in storage.Streams)
            {
                CompoundStream cmpStream = storage.OpenStream(streamName);
                byte[] arrBuffer = new byte[cmpStream.Length];
                cmpStream.Read(arrBuffer, 0, arrBuffer.Length);
                cmpStream.Dispose();
                if (streamName == DEF_INFO_STREAM_NAME)
                    DisplayAsIcon = (arrBuffer[0] & 0x40) == 0x40 ? true : false;
                OleObject.Storage.Streams.Add(streamName, new MemoryStream(arrBuffer));
            }
        }
        /// <summary>
        /// Parses the OLE stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void ParseOleStream(Stream stream)
        {
            if (CompoundFile.DocIO.Net.CompoundFile.CheckHeader(stream))
            {
                Syncfusion.CompoundFile.DocIO.Net.CompoundFile cmpFile = new Syncfusion.CompoundFile.DocIO.Net.CompoundFile(stream);
                string storageName = string.Empty;
                if (cmpFile.RootStorage.Storages.Length > 0)
                    storageName = cmpFile.RootStorage.Storages[0].Replace("_", string.Empty);
                int oleStorageName;
                if (int.TryParse(storageName, out oleStorageName))
                {
                    m_oleStorageName = oleStorageName.ToString();
                    foreach (DirectoryEntry entry in cmpFile.Directory.Entries)
                    {
                        if (entry.Name == "_" + storageName)
                        {
                            OleObject.Guid = entry.StorageGuid;
                            break;
                        }
                    }
                    ICompoundStorage storage = cmpFile.RootStorage.OpenStorage("_" + storageName);
                    ParseStreams(storage);
                    OleObject.Storage.ParseStorages(storage);
                    CheckObjectInfoStream();
                    storage.Dispose();
                }
                else
                {
                    m_oleStorageName = NextOleObjId.ToString();
                    ParseStreams(cmpFile.RootStorage);
                    OleObject.Storage.ParseStorages(cmpFile.RootStorage);
                    CheckObjectInfoStream();
                }
                cmpFile.Dispose();
            }
            else
            {
                m_oleStorageName = NextOleObjId.ToString();
                byte[] buffer = new byte[(int)stream.Length];
                stream.Read(buffer, 0, (int)stream.Length);
                OleObject.Save(new MemoryStream(buffer), this);
            }
        }
        /// <summary>
        /// Checks the object info stream exists. Add new object info stream if not exists.
        /// </summary>
        private void CheckObjectInfoStream()
        {
            if (!OleObject.Storage.Streams.ContainsKey(DEF_INFO_STREAM_NAME))
            {
                MemoryStream objInfoStream = new MemoryStream(UpdateObjInfoBytes());
                OleObject.Storage.Streams.Add(DEF_INFO_STREAM_NAME, objInfoStream);
            }
        }
        /// <summary>
        /// Creates the OLE obj container.
        /// </summary>
        /// <param name="nativeData">The native data.</param>
        /// <param name="dataPath">The data path.</param>
        internal void CreateOleObjContainer(byte[] nativeData, string dataPath)
        {
            dataPath = (dataPath == null) ? string.Empty : dataPath;
            m_oleStorageName = NextOleObjId.ToString();
            m_packageFileName = dataPath;
            m_oleObject = new OLEObject();
            MemoryStream stream = new MemoryStream(nativeData);
            if (CompoundFile.DocIO.Net.CompoundFile.CheckHeader(stream))
                ParseOleStream(stream);
            else
                m_oleObject.Save(nativeData, dataPath, this);
            OleObjectType = m_oleObject.OleType;
            m_strObjType = OleTypeConvertor.ToString(OleObjectType, false);
            //Update Ole object info stream based on aspect type
            if (!m_displayAsIcon)
                UpdateOleObjInfoStream();
        }
        /// <summary>
        /// Gets the OLE part stream.
        /// </summary>
        /// <returns></returns>
        internal Stream GetOlePartStream()
        {
            if (IsNativeItem()
                && OleObject.Storage.Streams.ContainsKey("Package"))
                return new MemoryStream((OleObject.Storage.Streams["Package"] as MemoryStream).ToArray());
            else
            {
                Syncfusion.CompoundFile.DocIO.Net.CompoundFile cmpFile = new Syncfusion.CompoundFile.DocIO.Net.CompoundFile();
                UpdateGuid(cmpFile, 0);
                OleObject.Storage.WriteToStorage(cmpFile.RootStorage);
                //Removes the ole object specific stream from the native stream.
                if (IsNativeItem())
                {
                    if (Array.IndexOf(cmpFile.RootStorage.Streams, DEF_INFO_STREAM_NAME) != -1)
                        cmpFile.RootStorage.DeleteStream(DEF_INFO_STREAM_NAME);
                    if (Array.IndexOf(cmpFile.RootStorage.Streams, "Ole") != -1)
                        cmpFile.RootStorage.DeleteStream("Ole");
                    if (Array.IndexOf(cmpFile.RootStorage.Streams, "CompObj") != -1)
                        cmpFile.RootStorage.DeleteStream("CompObj");
                    if (Array.IndexOf(cmpFile.RootStorage.Streams, "LinkInfo") != -1)
                        cmpFile.RootStorage.DeleteStream("LinkInfo");
                    if (Array.IndexOf(cmpFile.RootStorage.Streams, "EPRINT") != -1)
                        cmpFile.RootStorage.DeleteStream("EPRINT");
                }
                cmpFile.Flush();
                MemoryStream oleStream = new MemoryStream();
                cmpFile.Save(oleStream);
                cmpFile.Dispose();
                oleStream.Position = 0;
                return oleStream;
            }
        }
        /// <summary>
        /// Determines whether [is native item].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is native item]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsNativeItem()
        {
            switch (OleObjectType)
            {
                case OleObjectType.Word_97_2003_Document:
                case OleObjectType.WordDocument:
                case OleObjectType.WordMacroDocument:
                case OleObjectType.Excel_97_2003_Worksheet:
                case OleObjectType.ExcelBinaryWorksheet:
                case OleObjectType.ExcelChart:
                case OleObjectType.ExcelMacroWorksheet:
                case OleObjectType.ExcelWorksheet:
                case OleObjectType.PowerPoint_97_2003_Presentation:
                case OleObjectType.PowerPointMacroPresentation:
                case OleObjectType.PowerPointPresentation:
                case OleObjectType.PowerPointMacroSlide:
                case OleObjectType.PowerPointSlide:
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Writes to storage.
        /// </summary>
        /// <param name="storage">The storage.</param>
        internal void WriteToStorage(ICompoundStorage storage)
        {
            OleObject.Storage.WriteToStorage(storage);
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
                if (entry.Name == "_" + OleStorageName
                    || entry.Name == "Root Entry")
                {
                    entry.StorageGuid = Guid;
                }
            }
        }
        /// <summary>
        /// Gets the OLE container stream.
        /// </summary>
        /// <returns></returns>
        private Stream GetOleContainer()
        {
            if (OleObject.Storage.Streams.Count == 0)
                return null;

            Syncfusion.CompoundFile.DocIO.Net.CompoundFile cmpFile = new Syncfusion.CompoundFile.DocIO.Net.CompoundFile();
            ICompoundStorage storage = cmpFile.RootStorage.CreateStorage("_" + OleStorageName);
            cmpFile.Directory.Entries[1].StorageGuid = OleObject.Guid;
            OleObject.Storage.WriteToStorage(storage);
            cmpFile.Flush();
            MemoryStream oleStream = new MemoryStream();
            cmpFile.Save(oleStream);
            cmpFile.Dispose();
            oleStream.Position = 0;
            return oleStream;
        }
        /// <summary>
        /// Updates the ole object properties.
        /// </summary>
        private void UpdateProps()
        {
            if (m_field != null)
            {
                string link = m_field.FieldValue;
                char[] separator = new char[1] { '"' };
                string[] fieldValues = link.Split(separator);

                if (m_oleObjType == OleObjectType.Undefined)
                    m_oleObjType = OleTypeConvertor.ToOleType(fieldValues[0].Trim());
                if (string.IsNullOrEmpty(m_linkAddress) && fieldValues.Length > 1)
                    m_linkAddress = fieldValues[1];
            }
        }
        /// <summary>
        /// Sets the OLE picture.
        /// </summary>
        /// <param name="picture">The picture.</param>
        internal void SetOlePicture(WPicture picture)
        {
            m_picture = picture;
        }
        /// <summary>
        /// Sets the type of the OLE.
        /// </summary>
        /// <param name="type">The type.</param>
        internal void SetLinkType(OleLinkType type)
        {
            m_linkType = type;
        }
        /// <summary>
        /// Sets the type of the field.
        /// </summary>
        internal void SetFieldType()
        {
            if (m_linkType == OleLinkType.Link)
                m_field.FieldType = FieldType.FieldLink;
            else
                m_field.FieldType = FieldType.FieldEmbed;
        }
        /// <summary>
        /// Update Ole object ObjInfo stream with Display as icon data
        /// </summary>
        private void UpdateOleObjInfoStream()
        {
            if (OleObject.Storage.Streams.ContainsKey(DEF_INFO_STREAM_NAME))
            {
                byte[] m_dataBytes = UpdateObjInfoBytes();
                Stream stream = OleObject.Storage.Streams[DEF_INFO_STREAM_NAME];
                stream.Position = 0;
                stream.Write(m_dataBytes, 0, m_dataBytes.Length);
                stream.Flush();
            }
        }
        /// <summary>
        /// Update Object info 
        /// </summary>
        /// <returns></returns>
        private byte[] UpdateObjInfoBytes()
        {
            byte[] dataBytes = new byte[DEF_STRUCT_SIZE];
            switch (OleObjectType)
            {
                case OleObjectType.WordDocument:
                case OleObjectType.PowerPoint_97_2003_Slide:
                    if (LinkType == OleLinkType.Embed)
                        dataBytes = new byte[DEF_STRUCT_SIZE] { 0, 0, 3, 0, 1, 0 };
                    else
                        dataBytes = new byte[DEF_STRUCT_SIZE] { 16, 0, 3, 0, 13, 0 };
                    break;
                case OleObjectType.Equation:
                    if (LinkType == OleLinkType.Embed)
                        dataBytes = new byte[DEF_STRUCT_SIZE] { 0, 0, 3, 0, 4, 0 };
                    break;
                case OleObjectType.GraphChart:
                case OleObjectType.ExcelChart:
                    if (LinkType == OleLinkType.Embed)
                        dataBytes = new byte[DEF_STRUCT_SIZE] { 0, 2, 3, 0, 13, 0 };
                    else
                        dataBytes = new byte[DEF_STRUCT_SIZE] { 16, 0, 3, 0, 13, 0 };
                    break;
                case OleObjectType.AdobeAcrobatDocument:
                case OleObjectType.WordMacroDocument:
                case OleObjectType.Excel_97_2003_Worksheet:
                case OleObjectType.ExcelBinaryWorksheet:
                case OleObjectType.ExcelWorksheet:
                case OleObjectType.ExcelMacroWorksheet:
                case OleObjectType.PowerPoint_97_2003_Presentation:
                case OleObjectType.PowerPointPresentation:
                case OleObjectType.PowerPointMacroPresentation:
                case OleObjectType.PowerPointMacroSlide:
                case OleObjectType.PowerPointSlide:
                case OleObjectType.VisioDrawing:
                case OleObjectType.OpenDocumentPresentation:
                case OleObjectType.OpenDocumentSpreadsheet:
                case OleObjectType.OpenOfficeSpreadsheet:
                case OleObjectType.OpenOfficeText:
                case OleObjectType.OpenOfficeSpreadsheet1_1:
                case OleObjectType.OpenOfficeText_1_1:
                    {
                        //Update display as icon data
                        if (LinkType == OleLinkType.Embed)
                            if (m_displayAsIcon)
                                dataBytes = new byte[6] { 64, 0, 3, 0, 4, 0 };
                            else
                                dataBytes = new byte[6] { 0, 0, 3, 0, 13, 0 };
                        else
                            dataBytes = new byte[6] { 16, 0, 3, 0, 13, 0 };
                    }
                    break;
                case OleObjectType.BitmapImage:
                case OleObjectType.VideoClip:
                case OleObjectType.MIDISequence:
                    {
                        if (LinkType == OleLinkType.Embed)
                            dataBytes = new byte[DEF_STRUCT_SIZE] { 0, 0, 3, 0, 4, 0 };
                        else
                            dataBytes = new byte[DEF_STRUCT_SIZE] { 16, 0, 3, 0, 4, 0 };
                    }
                    break;
                case OleObjectType.WaveSound:
                case OleObjectType.MediaClip:
                case OleObjectType.Package:
                    dataBytes = new byte[DEF_STRUCT_SIZE] { 64, 0, 3, 0, 4, 0 };
                    break;
                case OleObjectType.WordPadDocument:
                case OleObjectType.Undefined:
                    {
                        if (LinkType == OleLinkType.Embed)
                            dataBytes = new byte[DEF_STRUCT_SIZE] { 0, 0, 3, 0, 4, 0 };
                        else
                            dataBytes = new byte[DEF_STRUCT_SIZE] { 16, 2, 3, 0, 13, 0 };
                    }
                    break;
                case OleObjectType.Word_97_2003_Document:
                    {
                        if (LinkType == OleLinkType.Embed)
                            dataBytes = new byte[DEF_STRUCT_SIZE] { 0, 2, 3, 0, 1, 0 };
                        else
                            dataBytes = new byte[DEF_STRUCT_SIZE] { 16, 2, 3, 0, 13, 0 };
                    }
                    break;
            }
            return dataBytes;
        }
        #endregion

        #region Override methods
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Create layout info
        /// </summary>
        protected override void CreateLayoutInfo()
        {
            m_layoutInfo = new LayoutInfo(ChildrenLayoutDirection.Horizontal);
            m_layoutInfo.IsClipped = true;
        }
#endif
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            WOleObject oleObj = (WOleObject)base.CloneImpl();

            oleObj.OleStorageName = NextOleObjId.ToString();

            if (m_oleObject != null)
                oleObj.m_oleObject = m_oleObject.Clone();

            oleObj.m_oleObjType = OleObjectType;

            if (m_field != null)
                oleObj.Field = m_field.Clone() as WField;

            if (m_oleXmlItem != null)
                oleObj.OleXmlItem = m_oleXmlItem.Clone() as XmlParagraphItem;

            oleObj.Cloned = true;
            return oleObj;
        }
        /// <summary>
        /// Adds the self.
        /// </summary>
        internal override void AddSelf()
        {
            if (NextSibling is WFieldMark
                && (NextSibling as WFieldMark).Type == FieldMarkType.FieldSeparator
                && NextSibling.NextSibling is WPicture)
                m_picture = NextSibling.NextSibling as WPicture;
        }
        /// <summary>
        /// Clones the relations.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal override void CloneRelationsTo(WordDocument doc, OwnerHolder nextOwner)
        {
            base.CloneRelationsTo(doc, nextOwner);

            if (NextSibling is WFieldMark
                && (NextSibling as WFieldMark).Type == FieldMarkType.FieldSeparator
                && NextSibling.NextSibling is WPicture)
                m_picture = NextSibling.NextSibling as WPicture;
            Cloned = false;
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal override void Close()
        {
            if (m_picture != null)
            {
                m_picture.Close();
                m_picture = null;
            }
            if (m_oleObject != null)
            {
                m_oleObject.Close();
                m_oleObject = null;
            }
            m_field = null;
            m_oleXmlItem = null;
        }
        #endregion
        
#if !SILVERLIGHT && !WP
        #region ILeafWidget Members
        SizeF ILeafWidget.Measure(DrawingContext dc)
        {
            if (m_picture != null)
            {
                if (m_picture.HeightScale != 100.0f)
                {
                    m_picture.Height = m_picture.Height * m_picture.HeightScale / 100;
                    m_picture.HeightScale = 100.0f;
                }
                if (m_picture.WidthScale != 100.0f)
                {
                    m_picture.Width = m_picture.Width * m_picture.WidthScale / 100;
                    m_picture.WidthScale = 100.0f;
                }
                return m_picture.Size;
            }
            else
                return SizeF.Empty;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cg"></param>
        /// <param name="ltWidget"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        void IWidget.Draw(DrawingContext dc, LayoutedWidget ltWidget)
        {
            if (m_picture != null)
                dc.DrawPicture(m_picture, ltWidget);
        }
        /// <summary>
        /// Initializing LayoutInfo value to null
        /// </summary>
        void IWidget.InitLayoutInfo()
        {
            m_layoutInfo = null;
        }
        #endregion
#endif
    }
}
