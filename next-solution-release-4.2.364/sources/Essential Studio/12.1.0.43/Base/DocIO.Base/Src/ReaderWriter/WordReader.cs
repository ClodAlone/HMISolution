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
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher;
using Syncfusion.DocIO.ReaderWriter.Escher;
using Syncfusion.DocIO.ReaderWriter.Security;
using Syncfusion.Layouting;
using System.Collections.Generic;
using Syncfusion.DocIO.DLS.Entities;
using Syncfusion.DocIO.Utilities;
using Syncfusion.CompoundFile.DocIO.Net;
using Syncfusion.CompoundFile.DocIO;
#if !SILVERLIGHT && !WP
using Syncfusion.CompoundFile.DocIO.Native;
#endif
#if !WINRT && !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.ReaderWriter
{
    #region Delegates
    /// <summary>
    /// 
    /// </summary>
    public delegate string NeedPasswordEventHandler();
    #endregion

    /// <summary>
    /// Implemented of IWordReader interface.
    /// </summary>
    /// <remarks>
    /// IWordReader : interface for forward-only reading data from word file.
    /// </remarks>
    [CLSCompliant(false)]
    [Documentation.DocumentationExclude()]
    internal class WordReader
      : WordReaderBase,
        IWordReader,
        IDisposable
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_PID_CODEPAGE = 1;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        protected bool m_bDisposed = false;

        /// <summary>
        /// 
        /// </summary>
        protected bool m_bDestroyStream = false;

        /// <summary>
        /// 
        /// </summary>
        private SectionProperties m_secProperties = null;

        /// <summary>
        /// 
        /// </summary>
        private bool m_bHeaderRead = false;

        /// <summary>
        /// Contains current subdocument reader
        /// </summary>
        private IWordSubdocumentReader m_lastReader;
        /// <summary>
        /// 
        /// </summary>
        private BuiltinDocumentProperties m_builtinProp = new BuiltinDocumentProperties(); 

        /// <summary>
        /// 
        /// </summary>
        private CustomDocumentProperties m_custProp = new CustomDocumentProperties();

        /// <summary>
        /// 
        /// </summary>
        private Encoding m_strEncoding;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="WordReader"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public WordReader(Stream stream)
        {
            m_streamsManager = new StreamsManager(stream, false);
            InitClass();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WordReader"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public WordReader(string fileName)
        {
            m_streamsManager = new StreamsManager(fileName, false);
            InitClass();
        }
        #endregion

        #region Class events
        /// <summary>
        /// 
        /// </summary>
        public event NeedPasswordEventHandler NeedPassword;
        #endregion

        #region Class properties
        /// <summary>
        /// 
        /// </summary>
        public DOPDescriptor DOP
        {
            get
            {
                return m_docInfo.TablesData.DOP;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public MainStatePositions StatePositions
        {
            get
            {
                return (MainStatePositions)m_statePositions;
            }
        }

        /// <summary>
        /// Gets current section number.
        /// </summary>
        public int SectionNumber
        {
            get
            {
                return StatePositions.SectionIndex + 1;
            }
        }

        /// <summary>
        /// Gets section's properties
        /// </summary>
        public SectionProperties SectionProperties
        {
            get
            {
                return m_secProperties;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public BuiltinDocumentProperties BuiltinDocumentProperties
        {
            get
            {
                return m_builtinProp;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public CustomDocumentProperties CustomDocumentProperties
        {
            get
            {
                return m_custProp;
            }
        }

        /// <summary>
        /// Get document's Macros data
        /// </summary>
        public MemoryStream MacrosStream
        {
            get
            {
                return m_streamsManager.MacrosStream;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte[] MacroCommands
        {
            get
            {
                return m_docInfo.TablesData.MacroCommands;
            }

            set
            {
                m_docInfo.TablesData.MacroCommands = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte[] Variables
        {
            get
            {
                return m_docInfo.TablesData.Variables;
            }

            set
            {
                m_docInfo.TablesData.Variables = value;
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        //public MemoryStream MsoDataStore
        //{
        //    get
        //    {
        //        return m_streamsManager.MsoDataStore;
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        public GrammarSpelling GrammarSpellingData
        {
            get
            {
                return m_docInfo.TablesData.GrammarSpellingData;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsFootnote
        {
            get
            {
                if (TextChunk.Length == 0)
                    return false;
                int textLength = TextChunk.Length;
                if (TextChunk.TrimStart(' ') != string.Empty)
                    textLength = TextChunk.TrimStart(' ').Length;

                int currentFootnoteMarkerStartPosition = CurrentTextPosition - textLength;
               
                //Maximum text length for a footnote/endnote marker is 10 in MS Word generated document
                //Hence limit the footnote marker length to 10 to avoid extra looping
                int footnoteLength = (textLength >= 10) ? 10 : textLength;

                for (int i = 0; i < footnoteLength ; i++)
                {
                    //Return true if footnote has a reference position
                    if (m_docInfo.TablesData.Footnotes.HasReference(currentFootnoteMarkerStartPosition + i))
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsEndnote
        {
            get
            {
                if (TextChunk.Length == 0)
                    return false;
                int textLength = TextChunk.Length;
                if (TextChunk.TrimStart(' ') != string.Empty)
                    textLength = TextChunk.TrimStart(' ').Length;

                int currentEndnoteMarkerStartPosition = CurrentTextPosition - textLength;

                //Maximum text length for a footnote/endnote marker is 10 in MS Word generated document
                //Hence limit the endnote marker length to 10 to avoid extra looping
                int endnoteLength = (textLength >= 10) ? 10 : textLength;

                for (int i = 0; i < endnoteLength; i++)
                {
                    //Return true if endnote has a reference position
                    if (m_docInfo.TablesData.Endnotes.HasReference(currentEndnoteMarkerStartPosition + i))
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string StandardAsciiFont
        {
            get
            {
                return m_docInfo.TablesData.StandardAsciiFont;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string StandardFarEastFont
        {
            get
            {
                return m_docInfo.TablesData.StandardFarEastFont;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string StandardNonFarEastFont
        {
            get
            {
                return m_docInfo.TablesData.StandardNonFarEastFont;
            }
        }
        /// <summary>
        /// Gets the standard/default bidi font
        /// </summary>
        public string StandardBidiFont
        {
            get
            {
                return m_docInfo.TablesData.StandardBidiFont;
            }
        }
        /// <summary>
        /// Gets a value indicating whether the document is encrypted.
        /// </summary>
        /// <value>
        /// <c>true</c> if the document is encrypted; otherwise, <c>false</c>.
        /// </value>
        public bool IsEncrypted
        {
            get
            {
                return m_docInfo.FibData.fEncrypted;
            }
        }

        /// <summary>
        /// Gets or sets the associated strings.
        /// </summary>
        /// <value>The associated strings.</value>
        internal byte[] AssociatedStrings
        {
            get
            {
                return m_docInfo.TablesData.AsociatedStrings;
            }

            set
            {
                m_docInfo.TablesData.AsociatedStrings = value;
            }
        }

        #endregion

        #region Class public methods
        /// <summary>
        /// Gets subdocument reader of specified type.
        /// </summary>
        /// <returns></returns>
        public IWordSubdocumentReader GetSubdocumentReader(WordSubdocument subDocumentType)
        {
            if (!m_bHeaderRead)
            {
                throw new InvalidOperationException("Call ReadDocumentHeader() before this method");
            }

            switch (subDocumentType)
            {
                case WordSubdocument.Footnote:
                    return m_lastReader = new WordFootnoteReader(this);
                case WordSubdocument.HeaderFooter:
                    return m_lastReader = new WordHeaderFooterReader(this);
                case WordSubdocument.Annotation:
                    return m_lastReader = new WordAnnotationReader(this);
                case WordSubdocument.Endnote:
                    return m_lastReader = new WordEndnoteReader(this);
                case WordSubdocument.TextBox:
                    return m_lastReader = new WordTextBoxReader(this);
                case WordSubdocument.HeaderTextBox:
                    return m_lastReader = new WordHFTextBoxReader(this);
            }

            return null;
        }

        /// <summary>
        /// Read header from the document.
        /// </summary>
        /// <returns></returns>     
        public void ReadDocumentHeader(WordDocument doc)
        {
            if (m_bHeaderRead)
            {
                throw new InvalidOperationException("Method ReadDocumentHeader() already called!");
            }
            // Mark that we called ReadDocumentHeader() method
            m_bHeaderRead = true;
            // Reads FIB data
            m_docInfo.FibData.Read(m_streamsManager.MainStream);

            if (m_docInfo.FibData.IsComplexFile)
            {
                throw new NotImplementedException("Complex format is not supported");
            }

            m_streamsManager.LoadTableStream(m_docInfo.FibData.TableStreamName);

            if (m_docInfo.FibData.fEncrypted)
            {
                if (NeedPassword == null)
                    throw new ArgumentException("Document is encrypted, password is needed to open the document");

                string password = NeedPassword();
                WordDecryptor wDecryptor = new WordDecryptor(m_streamsManager.TableStream, (MemoryStream)m_streamsManager.MainStream, m_streamsManager.DataStream, m_docInfo.FibData);
                bool passwordCorrect = wDecryptor.CheckPassword(password);

                if (passwordCorrect)
                {
                    wDecryptor.Decrypt();
                    m_streamsManager.UpdateStreams(wDecryptor.MainStream, wDecryptor.TableStream, wDecryptor.DataStream);
                }
                else
                    throw new Exception("Specified password \"" + password + "\" is incorrect!");
            }
            //Specific to preserve document as non encrypted. Removes the password which is passed as parameter in open method.
            doc.Password = null;

            m_docInfo.TablesData.Read(m_streamsManager.TableStream);

            UpdateBookmarks();
            UpdateStyleSheet();

            // Reads FKPs
            m_docInfo.FkpData.Read((MemoryStream)m_streamsManager.MainStream);

            // Updates properties
            UpdateCharacterProperties();
            UpdateParagraphProperties();
            UpdateSectionProperties();

            // Read document properties
#if SILVERLIGHT || WP || !AllowUnsafeCode
            ReadSummaryManaged();
#else
            ReadSummaryNative();
#endif

            if (m_docInfo.FibData.lcbDggInfo != 0)
            {
                Escher = new EscherClass(m_streamsManager.TableStream, m_streamsManager.MainStream, m_docInfo.FibData.fcDggInfo, m_docInfo.FibData.lcbDggInfo, doc);
                //Escher.RemoveEscherOle();
            }
            //      FileStream dgg1 = new FileStream("d:\\dgg1.dat", FileMode.Create, FileAccess.ReadWrite);
            //      for (int i = 0; i < m_docInfo.FibData.lcbDggInfo; i++)
            //      {
            //        m_tableStream.Position = m_docInfo.FibData.fcDggInfo;
            //        dgg1.WriteByte((byte)m_tableStream.ReadByte());
            //      }
            //      dgg1.Close();
            //
            //      FileStream store = new FileStream("d:\\store.dat", FileMode.Create, FileAccess.ReadWrite);
            //      FileStream dgg = new FileStream("d:\\dgg.dat", FileMode.Create, FileAccess.ReadWrite);
            //        
            //      Escher.WriteContainersData(store);
            //      Escher.WriteContainers(dgg);
            //      store.Close();
            //      dgg.Close();            

            m_statePositions.InitStartEndPos();
            m_streamsManager.MainStream.Position = m_statePositions.StartText;
        }
#if !SILVERLIGHT && !WP
        private void ReadSummaryNative()
        {
            IPropertySetStorage setProp = null;

            try
            {
                //Guid guidPropSet = new Guid("0000013a-0000-0000-c000-000000000046");

                int error = API.StgCreatePropSetStgOle(m_streamsManager.Storage.COMStorage, 0, out setProp);
                if (error != 0)
                {
                    throw new ExternalException("Cannot create Storage properties stream", error);
                }

                Guid guidSummary = new Guid("F29F85E0-4FF9-1068-AB91-08002B27B3D9");
                Guid guidDocument = new Guid("D5CDD502-2E9C-101B-9397-08002B2CF9AE");
                Guid guidCustom = new Guid("D5CDD505-2E9C-101B-9397-08002B2CF9AE");

                m_builtinProp = new BuiltinDocumentProperties();
                m_custProp = new CustomDocumentProperties();

                ReadProps(setProp, guidSummary, m_builtinProp.SummaryHash, PropertyType.Summary);
                ReadProps(setProp, guidDocument, m_builtinProp.DocumentHash, PropertyType.DocumentSummary);
                ReadProps(setProp, guidCustom, m_custProp.CustomHash, PropertyType.Custom);
            }
            catch 
            {
                m_builtinProp.DocumentHash.Clear();
                m_builtinProp.SummaryHash.Clear();
                m_custProp.CustomHash.Clear();
                ReadSummaryManaged();
            }
            if (setProp != null)
            {
                Marshal.ReleaseComObject(setProp);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="setProp"></param>
        /// <param name="guid"></param>
        /// <param name="propHash"></param>
        /// <param name="propertyType"></param>
        private void ReadProps(IPropertySetStorage setProp, Guid guid, IDictionary propHash, PropertyType propertyType)
        {
            IPropertyStorage storProp = null;
            IEnumSTATPROPSTG enumStatPropStg = null;
            try
            {
                int hr = setProp.Open(ref guid, (STGM.STGM_READ | STGM.STGM_SHARE_EXCLUSIVE), out storProp);
                //      }
                //      catch (COMException e)
                //      {
                //        uint hr = (uint)e.ErrorCode;
                // handle error codes
                if (hr == 0x80030002)
                {
                    return;
                }

                if (hr != 0)
                {
                    Marshal.ThrowExceptionForHR((int)hr);
                    return;
                }

                //Read CodePage
                PROPSPEC[] propspec = new PROPSPEC[1];
                propspec[0].ulKind = (IntPtr)PRSPEC.PRSPEC_PROPID;
                propspec[0].propid = (IntPtr)DEF_PID_CODEPAGE;
                PROPVARIANT[] specialProps = new PROPVARIANT[1];
                storProp.ReadMultiple(1, propspec, specialProps);
                PROPVARIANT propVar = specialProps[0];
                m_strEncoding = Encoding.GetEncoding(propVar.intVal);

                storProp.Enum(out enumStatPropStg);

                while (true)
                {
                    int structCount;
                    tagSTATPROPSTG[] tagSTATPROPSTGArr = new tagSTATPROPSTG[1];
                    enumStatPropStg.Next(1, tagSTATPROPSTGArr, out structCount);
                    if (structCount == 0)
                    {
                        return;
                    }

                    int propId = (int)tagSTATPROPSTGArr[0].propid;
                    PROPSPEC[] propspecArr = new PROPSPEC[1];
                    propspecArr[0].ulKind = (IntPtr)PRSPEC.PRSPEC_PROPID;
                    propspecArr[0].propid = (IntPtr)propId;
                    PROPVARIANT[] propvarArr = new PROPVARIANT[1];
                    storProp.ReadMultiple(1, propspecArr, propvarArr);
                    string text = tagSTATPROPSTGArr[0].lpwstrName;

                    object value = null;
                    propVar = propvarArr[0];
                    if ((VarEnum)propVar.vt == VarEnum.VT_BLOB)
                        continue;
                    if (propId == 10 && propertyType != PropertyType.Custom)
                    {
                        value = TimeSpan.FromTicks(propVar.fileTime);
                    }
                    else
                    {
                        value = ConvertPropVarToObject(propVar);
                    }

                    DocumentProperty property = null;

                    if (propertyType == PropertyType.DocumentSummary && text == null)
                    {
                        property = new DocumentProperty(ConvertPIDDSIToBuiltInProperty(propId), value);
                    }
                    else if ((propertyType == PropertyType.Summary && text == null))
                    {
                        property = new DocumentProperty(ConvertPIDSIToBuiltInProperty(propId), value);
                    }
                    else
                        property = new DocumentProperty(text, value, DocumentProperty.DetectPropertyType(value));

                    if (value != null)
                    {
                        if (propertyType == PropertyType.Custom)
                        {
                            propHash.Add(text, property);
                        }
                        else
                        {
                            if (propertyType == PropertyType.Summary)
                                propHash.Add((int)ConvertPIDSIToBuiltInProperty(propId), property);
                            else
                            {
                                if (property.PropertyId == BuiltInProperty.Company || property.PropertyId == BuiltInProperty.Category || property.PropertyId == BuiltInProperty.Manager)
                                    propHash.Add((int)ConvertPIDDSIToBuiltInProperty(propId), property);
                                else
                                    propHash.Add((int)propId, property);
                            }
                        }
                    }
                }
            }
            catch
            {
                m_builtinProp.DocumentHash.Clear();
                m_builtinProp.SummaryHash.Clear();
                m_custProp.CustomHash.Clear();
                ReadSummaryManaged();
            }
            finally
            {
                if (storProp != null)
                {
                    Marshal.ReleaseComObject(storProp);
                }

                if (enumStatPropStg != null)
                {
                    Marshal.ReleaseComObject(enumStatPropStg);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propId"></param>
        /// <returns></returns>
        private BuiltInProperty ConvertPIDSIToBuiltInProperty(int propId)
        {
            switch ((PIDSI)propId)
            {
                case PIDSI.Appname:
                    return BuiltInProperty.ApplicationName;
                case PIDSI.Author:
                    return BuiltInProperty.Author;
                case PIDSI.Charcount:
                    return BuiltInProperty.CharCount;
                case PIDSI.Comments:
                    return BuiltInProperty.Comments;
                case PIDSI.Create_dtm:
                    return BuiltInProperty.CreationDate;
                case PIDSI.Doc_security:
                    return BuiltInProperty.Security;
                case PIDSI.EditTime:
                    return BuiltInProperty.EditTime;
                case PIDSI.Keywords:
                    return BuiltInProperty.Keywords;
                case PIDSI.LastAuthor:
                    return BuiltInProperty.LastAuthor;
                case PIDSI.LastPrinted:
                    return BuiltInProperty.LastPrinted;
                case PIDSI.LastSave_dtm:
                    return BuiltInProperty.LastSaveDate;
                case PIDSI.Pagecount:
                    return BuiltInProperty.PageCount;
                case PIDSI.Revnumber:
                    return BuiltInProperty.RevisionNumber;
                case PIDSI.Subject:
                    return BuiltInProperty.Subject;
                case PIDSI.Template:
                    return BuiltInProperty.Template;
                case PIDSI.Thumbnail:
                    return BuiltInProperty.Thumbnail;
                case PIDSI.Title:
                    return BuiltInProperty.Title;
                case PIDSI.Wordcount:
                    return BuiltInProperty.WordCount;
                default:
                    return (BuiltInProperty)propId;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propId"></param>
        /// <returns></returns>
        private BuiltInProperty ConvertPIDDSIToBuiltInProperty(int propId)
        {
            switch (propId)
            {
                case 2:
                    return BuiltInProperty.Category;
                case 4:
                    return BuiltInProperty.ByteCount;
                case 5:
                    return BuiltInProperty.LineCount;
                case 6:
                    return BuiltInProperty.ParagraphCount;
                case 7:
                    return BuiltInProperty.SlideCount;
                case 8:
                    return BuiltInProperty.NoteCount;
                case 9:
                    return BuiltInProperty.HiddenCount;
                case 10:
                    return BuiltInProperty.MultimediaClipCount;
                case 11:
                    return BuiltInProperty.ScaleCrop;
                case 12:
                    return BuiltInProperty.HeadingPair;
                case 13:
                    return BuiltInProperty.DocParts;
                case 14:
                    return BuiltInProperty.Manager;
                case 15:
                    return BuiltInProperty.Company;
                case 16:
                    return BuiltInProperty.LinksDirty;
                case 17:
                    return BuiltInProperty.CharCount;
                default:
                    return (BuiltInProperty)propId;

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propVar"></param>
        /// <returns></returns>
        private object ConvertPropVarToObject(PROPVARIANT propVar)
        {
            VarEnum varEnum = (VarEnum)propVar.vt;
            if (varEnum <= VarEnum.VT_BOOL)
            {

                switch (varEnum)
                {
                    case VarEnum.VT_I2:
                        {
                            return propVar.shortVal;
                        }

                    case VarEnum.VT_I4:
                        {
                            return propVar.intVal;
                        }

                    // float value
                    case VarEnum.VT_R4:
                        {
                            return null;
                        }

                    case VarEnum.VT_R8:
                        {
                            return propVar.doubleVal;
                        }

                    case VarEnum.VT_BOOL:
                        {
                            return propVar.boolVal;
                        }
                }
            }
            else
            {
                switch (varEnum)
                {
                    case VarEnum.VT_LPSTR:
                        {
                            int length = 0;
                            if (propVar.intPtr != IntPtr.Zero)
                            {
                                while (Marshal.ReadByte(propVar.intPtr, length) != 0)
                                {
                                    length++;
                                }

                                byte[] buffer = new byte[length];
                                Marshal.Copy(propVar.intPtr, buffer, 0, length);
                                return m_strEncoding.GetString(buffer);
                            }
                            else
                                return string.Empty;
                        }

                    case VarEnum.VT_LPWSTR:
                        {
                            return Marshal.PtrToStringUni(propVar.intPtr);
                        }
                    case VarEnum.VT_INT:
                        {
                            return propVar.intVal;
                        }
                    case VarEnum.VT_FILETIME:
                        {
                            if ((propVar.fileTime >= 0) && (propVar.fileTime <= DateTime.MaxValue.ToFileTime()))
                                return DateTime.FromFileTime(propVar.fileTime);
                            else
                                return DateTime.MinValue;
                        }

                    case VarEnum.VT_BLOB:
                        {
                            int length = propVar.intVal;
                            byte[] blob = new byte[length];
                            Marshal.Copy(propVar.intPtr2, blob, 0, blob.Length);
                            return blob;
                        }

                    case VarEnum.VT_CF:
                        {
                            //ClipDataWrapper clipdata = new ClipDataWrapper();
                            //clipdata.Read(propVar);
                            //return clipdata;
                            // TODO: Don't work correct, need deep investigation.
                            return null;
                        }

                    default:
                        {
                            //Debug.WriteLine("Unknown varEnum: " + varEnum);
                            break;
                        }
                }
            }

            return null;
        }
#endif
        /// <summary>
        /// Reads the Summary information and Document Summary information
        /// </summary>
        private void ReadSummaryManaged()
        {
            //load the SummaryInformation stream and reads it
            m_streamsManager.LoadSummaryInfoStream();
            if (m_streamsManager.SummaryInfoStream != null && m_streamsManager.SummaryInfoStream.Length > 0)
            {
                DocumentPropertyCollection properties = new DocumentPropertyCollection(m_streamsManager.SummaryInfoStream);
                ReadDocumentProperties(properties);
            }
            //loads the DocumentSummaryInformation Stream and reads it.
            m_streamsManager.LoadDocumentSummaryInfoStream();
            if (m_streamsManager.DocumentSummaryInfoStream != null && m_streamsManager.DocumentSummaryInfoStream.Length > 0)
            {
                DocumentPropertyCollection properties = new DocumentPropertyCollection(m_streamsManager.DocumentSummaryInfoStream);
                ReadDocumentProperties(properties);
            }
        }
        /// <summary>
        /// Reads the document properties
        /// </summary>
        /// <param name="properties"></param>
        private void ReadDocumentProperties(DocumentPropertyCollection properties)
        {

            Guid guidSummary = new Guid("F29F85E0-4FF9-1068-AB91-08002B27B3D9");
            Guid guidDocument = new Guid("D5CDD502-2E9C-101B-9397-08002B2CF9AE");
            Guid guidCustom = new Guid("D5CDD505-2E9C-101B-9397-08002B2CF9AE");

            List<PropertySection> lstSections = properties.Sections;

            for (int i = 0, len = lstSections.Count; i < len; i++)
            {
                PropertySection section = lstSections[i];

                if (section.Id == guidSummary)
                {
                    ReadProperties(section, m_builtinProp.SummaryHash, true, true);
                }
                else if (section.Id == guidDocument)
                {
                    ReadProperties(section, m_builtinProp.DocumentHash, false, true);
                }
                else if (section.Id == guidCustom)
                {
                    ReadProperties(section, m_custProp.CustomHash, true, false);
                }
            }
        }
        /// <summary>
        /// Read the properties in the PropertySection
        /// </summary>
        /// <param name="section"></param>
        /// <param name="dicProperties"></param>
        /// <param name="bSummary"></param>
        /// <param name="bBuiltIn"></param>
        private void ReadProperties(PropertySection section, IDictionary dicProperties, bool bSummary, bool bBuiltIn)
        {
            Dictionary<int, DocumentProperty> hashPropById = null;

            if (!bBuiltIn)
            {
                hashPropById = new Dictionary<int, DocumentProperty>();
            }

            List<PropertyData> arrProperties = section.Properties;
            for (int i = 0, len = arrProperties.Count; i < len; i++)
            {
                PropertyData propertyData = arrProperties[i];

                if (propertyData.IsLinkToSource)
                {
                    int parentId = propertyData.ParentId;
                    DocumentProperty property = hashPropById[parentId];
                    property.SetLinkSource(propertyData);
                }
                else if(!(propertyData.Data is ClipboardData))
                {
                   DocumentProperty property = new DocumentProperty(propertyData, bSummary);

                    object key = bBuiltIn
                      ? (object)(int)property.PropertyId
                      : (object)property.Name;

                    if (!bBuiltIn)
                    {
                        hashPropById.Add(propertyData.Id, property);
                    }
                    
                    if (property.Value != null)
                    {
                        //Checks for document summary information
                        if (!bSummary && bBuiltIn && !dicProperties.Contains(propertyData.Id))
                            dicProperties.Add(propertyData.Id, property);
                        else
                            dicProperties[key] = property;
                    }
                }

            }
        }

        /// <summary>
        /// Read next elementary text string*.
        /// </summary>
        /// <remarks>
        /// * - "elementary text string" - string, in which all symbols have the 
        /// identical character/paragraph/section properties.
        /// </remarks>
        /// <returns></returns>
        public override WordChunkType ReadChunk()
        {
            if (!m_bHeaderRead)
            {
                throw new InvalidOperationException("Call ReadDocumentHeader() before this method");
            }

            WordChunkType ct = base.ReadChunk();

            return ct;
        }

        /// <summary>
        /// Reads end of the document
        /// </summary>
        /// <returns></returns>
        public void ReadDocumentEnd()
        {
            m_streamsManager.CloseStg();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override FieldDescriptor GetFld()
        {
            int cp = CalcCP(StatePositions.StartText, 1);

            return m_docInfo.TablesData.Fields.FindFld(m_type, cp);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override FileShapeAddress GetFSPA()
        {
            int cp = CalcCP(StatePositions.StartText, 1);

            return m_docInfo.TablesData.FileArtObjects.FindFileShape(m_type, cp);
        }

        /// <summary>
        /// Freeze position in stream
        /// </summary>
        public override void FreezeStreamPos()
        {
            //Before freeze stream for subdocument readers.      
            if (m_lastReader != null)
            {
                (m_lastReader as WordReaderBase).FreezeStreamPos();
            }
            // After freeze main stream.
            base.FreezeStreamPos();
        }

        /// <summary>
        /// Unfreeze position in stream
        /// </summary>
        public override void UnfreezeStreamPos()
        {
            if (m_lastReader != null)
            {
                // At first freeze stream for subdocument readers. 
                (m_lastReader as WordReaderBase).FreezeStreamPos();
                // After unfreeze main stream.
            }

            base.UnfreezeStreamPos();
        }

        /// <summary>
        /// Class initialization.
        /// </summary>
        protected override void InitClass()
        {
            m_secProperties = new SectionProperties();
            base.InitClass();

            m_docInfo = new DocInfo(m_streamsManager);
            m_statePositions = new MainStatePositions(m_docInfo.FkpData);
            m_type = WordSubdocument.Main;
            m_startTextPos = 0;
            m_endTextPos = 0;
        }

        /// <summary>
        /// Updates end positions of CHPx/PAPx/SEP
        /// </summary>
        /// <param name="iEndPos"></param>
        protected override void UpdateEndPositions(long iEndPos)
        {
            base.UpdateEndPositions(iEndPos);

            if (StatePositions.UpdateSepxEndPos(iEndPos))
            {
                UpdateSectionProperties();
            }
        }

        /// <summary>
        /// Update section properties to current
        /// </summary>
        private void UpdateSectionProperties()
        {
            m_secProperties = new SectionProperties(StatePositions.CurrentSepx);
        }

        /// <summary>
        /// Updates stylesheet data
        /// </summary>
        private void UpdateStyleSheet()
        {
            WPTablesData tablesData = m_docInfo.TablesData;
            StyleSheetInfoRecord info = tablesData.StyleSheetInfo;
            FontFamilyNameRecord[] ffnRecords = tablesData.FFNStringTable.FontFamilyNameRecords;
            string[] fontNames = new string[ffnRecords.Length];

            // Exstracts font names
            for (int i = 0, len = fontNames.Length; i < len; i++)
            {
                fontNames[i] = ffnRecords[i].FontName;
                StyleSheet.UpdateFontSubstitutionTable(ffnRecords[i]);
            }

            // Updates stylesheet fontnames
            StyleSheet.ClearFontNames();
            StyleSheet.UpdateFontNames(fontNames);

            // Extract styles, converts and append to WordStyleSheet object
            StyleDefinitionRecord[] styleDef = tablesData.StyleDefinitions;

            for (int i = 0, len = styleDef.Length; i < 15 && i < len; i++)
            {
                StyleDefinitionRecord styleDefinition = styleDef[i];

                if (styleDefinition.CharacterProperty != null && styleDefinition.CharacterProperty.FontAscii == ushort.MaxValue)
                {
                    // If style don't have base style and don't have default font
                    // then set it
                    if (styleDefinition.BaseStyle == 4095)
                    {
                        styleDefinition.CharacterProperty.FontAscii = info.StandardChpStsh[0];
                        styleDefinition.CharacterProperty.FontFarEast = info.StandardChpStsh[1];
                        styleDefinition.CharacterProperty.FontNonFarEast = info.StandardChpStsh[2];
                    }
                }

                if (styleDefinition.StyleName != null)
                {
                    WordStyle wStyle = StyleSheet.UpdateStyle(i, styleDefinition.StyleName);
                    wStyle.ID = styleDefinition.StyleId;
                    wStyle.BaseStyleIndex = styleDefinition.BaseStyle;
                    wStyle.NextStyleIndex = styleDefinition.NextStyleId;
                    wStyle.LinkStyleIndex = styleDefinition.LinkStyleId;
                    wStyle.IsCharacterStyle = (styleDefinition.TypeCode == WordStyleType.CharacterStyle);
                    wStyle.IsPrimary = styleDefinition.IsQFormat;
                    wStyle.IsSemiHidden = styleDefinition.IsSemiHidden;
                    wStyle.UnhideWhenUsed = styleDefinition.UnhideWhenUsed;
                    wStyle.TypeCode = styleDefinition.TypeCode;
                    if (styleDefinition.UpxNumber == 3 && styleDefinition.Tapx != null)
                    {
                        wStyle.TableStyleData = new byte[styleDefinition.Tapx.Length];
                        Buffer.BlockCopy(styleDefinition.Tapx, 0, wStyle.TableStyleData, 0, styleDefinition.Tapx.Length);
                    }
                    UpdateStyleProperties(wStyle, styleDefinition);
                }
            }

            for (int i = StyleSheet.StylesCount, len = styleDef.Length; i < len; i++)
            {
                StyleDefinitionRecord styleDefinition = styleDef[i];

                if (styleDefinition.StyleName == null)
                {
                    StyleSheet.AddEmptyStyle();
                }
                else
                {
                    WordStyle wStyle = StyleSheet.CreateStyle(styleDefinition.StyleName, false);
                    wStyle.ID = styleDefinition.StyleId;
                    wStyle.BaseStyleIndex = styleDefinition.BaseStyle;
                    wStyle.NextStyleIndex = styleDefinition.NextStyleId;
                    wStyle.LinkStyleIndex = styleDefinition.LinkStyleId;
                    wStyle.IsPrimary = styleDefinition.IsQFormat;
                    wStyle.IsSemiHidden = styleDefinition.IsSemiHidden;
                    wStyle.UnhideWhenUsed = styleDefinition.UnhideWhenUsed;
                    wStyle.IsCharacterStyle = (styleDefinition.TypeCode == WordStyleType.CharacterStyle);
                    wStyle.TypeCode = styleDefinition.TypeCode;
                    if (styleDefinition.UpxNumber == 3 && styleDefinition.Tapx != null)
                    {
                        wStyle.TableStyleData = new byte[styleDefinition.Tapx.Length];
                        Buffer.BlockCopy(styleDefinition.Tapx, 0, wStyle.TableStyleData, 0, styleDefinition.Tapx.Length);
                    }
                    UpdateStyleProperties(wStyle, styleDefinition);
                }
            }

            tablesData.StyleDefinitions = null;
            tablesData.StyleSheetInfo = null;

            //      m_standardAsciiFont = fontNames[info.StandardChpStsh[0]];
            //      m_standardFarEastFont = fontNames[info.StandardChpStsh[1]];
            //      m_standardNonFarEastFont = fontNames[info.StandardChpStsh[2]];
        }

        /// <summary>
        /// Converts characters/paragraphs properties to style properties.
        /// </summary>
        /// <param name="style"></param>
        /// <param name="record"></param>
        private void UpdateStyleProperties(WordStyle style, StyleDefinitionRecord record)
        {
            CharacterProperties charProp = new CharacterProperties(record.CharacterProperty, StyleSheet);
            style.UpdateCharactersProperties(charProp);

            if (record.ParagraphProperty != null)
            {
                ParagraphProperties paragrProps = new ParagraphProperties(record.ParagraphProperty);
                style.UpdateParagraphsProperties(paragrProps);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entryStream"></param>
        /// <returns></returns>
        private MemoryStream CopyStream(Stream entryStream)
        {
            try
            {
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
            //      if (entryStream == null)
            //      {
            //        return null;
            //      }
            //      try
            //      {
            //        MemoryStream stream2 = new MemoryStream((int) entryStream.Length);
            //        stream2.SetLength((long) ((int) entryStream.Length));
            //        entryStream.Read(stream2.GetBuffer(), 0, (int) entryStream.Length);
            //        resultStream = stream2;
            //      }
            //      finally
            //      {
            //        entryStream.Close();
            //      }
            //      return resultStream;
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }
        #endregion
    }

    /// <summary>
    /// Base implementation for WordReader and WordSubdocumentReader classes.
    /// </summary>
    [CLSCompliant(false)]
    [Documentation.DocumentationExclude()]
    internal abstract class WordReaderBase : IWordReaderBase
    {
        #region Class constants
        private const int INVALID_CHUNK_LENGTH = -1;
        private const int DEF_WORD9_DOP_LEN = 544;
        private const int DEF_WORD10_DOP_LEN = 594;
        private const int DEF_WORD11_DOP_LEN = 616;
        #endregion

        #region Class members
        /// <summary>
        /// Used to convert managed memory block (byte array)
        /// into managed object
        /// </summary>
        public StreamsManager m_streamsManager;

        /// <summary>
        /// DocInfo contains stream component data blocks.
        /// </summary>
        public DocInfo m_docInfo;

        protected WordStyleSheet m_styleSheet = null;
        protected string m_textChunk = string.Empty;
        protected WordChunkType m_chunkType = WordChunkType.Text;

        /// <summary>
        /// The index of style, applied to current text chunk.
        /// </summary>
        protected int m_currStyleIndex = 0;

        /// <summary>
        /// Saves all needed positions in stream for current reader
        /// </summary>
        protected StatePositionsBase m_statePositions;

        protected WordSubdocument m_type;
        protected int m_startTextPos;
        protected int m_endTextPos;

        /// <summary>
        /// 
        /// </summary>
        private CharacterProperties m_characterProps = null;
        //    private CharacterProperties m_fullCharacterProps = null;
        private ParagraphProperties m_paragraphProps = null;
        private ParagraphProperties m_fullParagraphProps = null;
        private BookmarkInfo[] m_bookmarks;
        private long m_iSavedStreamPosition = -1;
        private bool m_bStreamPosSaved = false;
        private BookmarkInfo m_currentBookmark;
        private BookmarkInfo m_bookmarkAfterParaEnd;
        private bool m_isBookmarkStart;
        private bool m_isBKMKStartAfterParaEnd;
        private int m_cellCounter;
        private bool m_isCellMark;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="WordReaderBase"/> class.
        /// </summary>
        /// <param name="streamsManager">The streams manager.</param>
        public WordReaderBase(StreamsManager streamsManager)
        {
            m_streamsManager = streamsManager;
        }

        /// <summary>
        /// Hide default constructor.
        /// </summary>
        protected WordReaderBase()
        {
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets index of current style .
        /// </summary>
        public int CurrentStyleIndex
        {
            get
            {
                return m_currStyleIndex;
            }
        }

        /// <summary>
        /// Gets document stylesheet.
        /// </summary>
        public WordStyleSheet StyleSheet
        {
            get
            {
                return m_styleSheet;
            }
        }

        /// <summary>
        /// Gets type of current text chunk.
        /// </summary>
        public WordChunkType ChunkType
        {
            get
            {
                return m_chunkType;
            }
        }

        /// <summary>
        /// Current read text chunk.
        /// </summary>
        public string TextChunk
        {
            get
            {
                return m_textChunk;
            }

            set
            {
                m_textChunk = value;
            }
        }

        /// <summary>
        /// Gets character properties for current text chunk.
        /// </summary>
        public CharacterProperties CharacterProperties
        {
            get
            {
                return m_characterProps;
            }
        }

        //    /// <summary>
        //    /// Gets character properties for current text chunk.
        //    /// </summary>
        //    public CharacterProperties FullCharacterProperties
        //    {
        //      get
        //      {
        //        return m_fullCharacterProps;
        //      }
        //    }

        /// <summary>
        /// Gets paragraph properties for current text chunk.
        /// </summary>
        public ParagraphProperties ParagraphProperties
        {
            get
            {
                return m_paragraphProps;
            }
        }

        /// <summary>
        /// Gets full paragraph properties for current text chunk.
        /// </summary>
        public ParagraphProperties FullParagraphProperties
        {
            get
            {
                return m_fullParagraphProps;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ListInfo ListInfo
        {
            get
            {
                return m_docInfo.TablesData.ListInfo;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HasTableBody
        {
            get
            {
                return ParagraphProperties.IsCellMark;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public EscherClass Escher
        {
            get
            {
                return m_docInfo.TablesData.Escher;
            }

            set
            {
                m_docInfo.TablesData.Escher = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Fields Fields
        {
            get
            {
                return m_docInfo.TablesData.Fields;
            }
        }

        /// <summary>
        /// Gets WPTablesData object.
        /// </summary>
        public WPTablesData TablesData
        {
            get
            {
                return m_docInfo.TablesData;
            }
        }

        /// <summary>
        /// Gets current text position relative to document text start(2048).
        /// </summary>
        public int CurrentTextPosition
        {
            get
            {
                //        return m_statePositions.CurrentTextPosition;//m_curTextPosition;
                //        return (int)(m_docStream.Position - m_statePositions.StartText);
                int cp = (int)m_docInfo.TablesData.ConvertFCToCP((uint)m_streamsManager.MainStream.Position);
                return cp;
                //        uint startCP = m_docInfo.TablesData.ConvertFCToCP((uint)StatePositions.StartText);
                //        return (int)(currCP - startCP - 1);
            }

            set
            {
                uint pos = m_docInfo.TablesData.ConvertCharPosToFileCharPos((uint)value);
                m_streamsManager.MainStream.Position = pos;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public BookmarkInfo[] Bookmarks
        {
            get
            {
                if (m_bookmarks == null)
                {
                    m_bookmarks = m_docInfo.TablesData.GetBookmarks();
                }

                return m_bookmarks;
            }

            set
            {
                m_bookmarks = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public BookmarkInfo CurrentBookmark
        {
            get
            {
                return m_currentBookmark;
            }

            set
            {
                m_currentBookmark = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public BookmarkInfo BookmarkAfterParaEnd
        {
            get
            {
                return m_bookmarkAfterParaEnd;
            }
            set
            {
                m_bookmarkAfterParaEnd = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsBKMKStartAfterParaEnd
        {
            get
            {
                return m_isBKMKStartAfterParaEnd;
            }
            set
            {
                m_isBKMKStartAfterParaEnd = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsBookmarkStart
        {
            get
            {
                return m_isBookmarkStart;
            }
        }

        /// <summary>
        /// Gets the document version.
        /// </summary>
        /// <value>The version.</value>
        public DocumentVersion Version
        {
            get
            {
                return GetDocVersion();
            }
        }

        /// <summary>
        /// Encoding of current read text chunk.
        /// </summary>
        protected Encoding Encoding
        {
            get
            {
                //m_fibData.Encoding;
                return m_docInfo.TablesData.GetEncodingByFC(m_streamsManager.MainStream.Position);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected internal int EncodingCharSize
        {
            get
            {
                int charSize = 1;
#if SILVERLIGHT || WP
                if ( Encoding == Encoding.UTF8)
#else
				if (Encoding == Encoding.ASCII || Encoding == Encoding.UTF8)
#endif
                {
                    charSize = 1;
                }

                else if (Encoding == Encoding.Unicode)
                {
                    charSize = 2;
                }

                return charSize;
            }
        }

        /// <summary>
        /// Gets current style
        /// </summary>
        protected WordStyle CurrentStyle
        {
            get
            {
                return StyleSheet.GetStyleByIndex(CurrentStyleIndex);
            }
        }

        /// <summary>
        /// Gets the start text position.
        /// </summary>
        /// <value>The start text pos.</value>
        internal int StartTextPos
        {
            get
            {
                return m_startTextPos;
            }
        }

        /// <summary>
        /// Gets the end text position.
        /// </summary>
        /// <value>The end text pos.</value>
        internal int EndTextPos
        {
            get
            {
                return m_endTextPos;
            }
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Read next elementary text string*.
        /// </summary>
        /// <remarks>
        /// * - "elementary text string" - string, in which all symbols have the 
        /// identical character/paragraph/section properties.
        /// </remarks>
        /// <returns></returns>
        public virtual WordChunkType ReadChunk()
        {
            // If stream position was saved - unfreeze it.
            UnfreezeStreamPos();

            // 
            // 1) Gets next chunk length
            //
            m_textChunk = string.Empty;
            m_chunkType = WordChunkType.Text;
            int iLength = CalculateChunkLength();

            if (m_chunkType != WordChunkType.DocumentEnd && m_chunkType != WordChunkType.EndOfSubdocText
              && iLength > 0)
            {
                //
                // 2) Read and parse text chunk.
                //    (NOTE: If text chunk consists special symbols - it splits.)
                m_startTextPos = m_endTextPos;
                m_statePositions.CurrentTextPosition += ReadAndParseTextChunk(iLength);
                m_endTextPos = m_startTextPos + m_textChunk.Length;

                //
                // 3) Sets type of current chunk.
                //
                UpdateChunkType();
            }

            //Special case for Consecutive  symbol
            bool isSymbol = false;
            foreach (char c in m_textChunk)
            {
                if (c == SpecialCharacters.SymbolAscii)
                    isSymbol = true;
                else
                {
                    isSymbol = false;
                    break;
                }
            }
            if (isSymbol)
            {
                if (CharacterProperties.Sprms[WordSprmOptions.sprmCSymbol] != null)
                {
                    m_chunkType = WordChunkType.Symbol;
                }
                else
                {
                    m_chunkType = WordChunkType.Text;
                }
            }
            return m_chunkType;
        }

        /// <summary>
        /// Get interface for reading images from word file
        /// </summary>
        /// <returns></returns>
        virtual public IWordImageReader GetImageReader(WordDocument doc)
        {
            return m_docInfo.GetImageReader(m_streamsManager, CharacterProperties.PicLocation, doc);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual FileShapeAddress GetFSPA()
        {
            return null;//throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ShapeBase GetDrawingObject()
        {
            UnfreezeStreamPos();
            FileShapeAddress fspa = GetFSPA();

            if (fspa == null)
            {
                return null;
            }

            MsofbtSpContainer spContainer = null;
            if (Escher.Containers.ContainsKey(fspa.Spid))
            {
                spContainer = Escher.Containers[fspa.Spid] as MsofbtSpContainer;
            }

            ShapeBase shape = null;
            if (spContainer != null)
            {
                EscherShapeType shapeType = spContainer.Shape.ShapeType;
                switch (shapeType)
                {
                    // Textbox founded in document
                    case EscherShapeType.msosptTextBox:
                        shape = ReadTextBoxProps(spContainer, fspa);
                        break;
                    //Picture shape founded
                    case EscherShapeType.msosptPictureFrame:
                        shape = ReadPictureProps(spContainer, fspa);
                        break;
                }
            }

            return shape;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public FormField GetFormField(FieldType fieldType)
        {
            m_streamsManager.DataStream.Position = CharacterProperties.PicLocation;

            FormField formField = new FormField(fieldType, m_streamsManager.DataReader);

            return formField;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual FieldDescriptor GetFld()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public bool ReadWatermark(WordDocument doc)
        {
            bool retVal = false;
            if (this is WordHeaderFooterReader)
            {
                UnfreezeStreamPos();
                FileShapeAddress fspa = GetFSPA();
                if (fspa == null)
                    return false;

                MsofbtSpContainer spContainer = null;
                if (Escher.Containers.ContainsKey(fspa.Spid))
                {
                    spContainer = Escher.Containers[fspa.Spid] as MsofbtSpContainer;
                }

                if (spContainer != null && IsWatermark(spContainer))
                {
                    retVal = true;
                    if (doc.Watermark.Type == WatermarkType.NoWatermark)
                    {
                        if (spContainer.Shape.ShapeType == EscherShapeType.msosptTextPlainText)
                        {
                            ReadTextWatermark(spContainer, doc);
                            (doc.Watermark as TextWatermark).ShapeHeightInPixels = fspa.Height;
                            (doc.Watermark as TextWatermark).ShapeWidthInPixels = fspa.Width;
                        }
                        else if (spContainer.Shape.ShapeType == EscherShapeType.msosptPictureFrame ||
                          spContainer.Shape.ShapeType == EscherShapeType.msosptCustomShape)
                        {
                            ReadPictureWatermark(spContainer, doc, fspa);
                        }
                    }
                    else
                    {
                        Escher.RemoveContainerBySpid(spContainer.Shape.ShapeId, true);
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// Get array of Bookmarks from doc file.
        /// </summary>
        /// <returns></returns>
        public BookmarkInfo[] GetBookmarks()
        {
            m_bookmarks = m_docInfo.TablesData.GetBookmarks();
            return m_bookmarks;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool SubdocumentExist()
        {
            bool retVal = true;
            long iCurrentPos = m_streamsManager.MainStream.Position;
            if (m_statePositions.IsEndOfText(iCurrentPos) || GetChunkEndPosition(iCurrentPos) < 0)
            {
                retVal = false;
            }

            return retVal;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Saved publicy current stream position.
        /// </summary>
        public virtual void FreezeStreamPos()
        {
            if (!m_bStreamPosSaved)
            {
                m_iSavedStreamPosition = m_streamsManager.MainStream.Position;
                m_bStreamPosSaved = true;
            }
        }

        /// <summary>
        /// If stream position has been saved - restore it.
        /// </summary>
        public virtual void UnfreezeStreamPos()
        {
            if (m_bStreamPosSaved)
            {
                m_streamsManager.MainStream.Position = m_iSavedStreamPosition;
                m_bStreamPosSaved = false;
            }
        }
        /// <summary>
        /// Determines whether this instance has list.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance has list; otherwise, <c>false</c>.
        /// </returns>
        public bool HasList()
        {
            return m_docInfo.TablesData.HasList();
        }

        /// <summary>
        /// Restores bookmark.
        /// </summary>
        internal void RestoreBookmark()
        {
            if (m_currentBookmark == null)
                return;

            int index = m_currentBookmark.Index;
            if (m_isBookmarkStart)
            {
                m_bookmarks[index].StartPos = m_currentBookmark.StartPos;
                m_bookmarks[index].StartCellIndex = m_currentBookmark.StartCellIndex;
            }
            else
            {
                m_bookmarks[index].EndPos = m_currentBookmark.EndPos;
                m_bookmarks[index].EndCellIndex = m_currentBookmark.EndCellIndex;
            }

            m_currentBookmark = null;
        }

        /// <summary>
        /// Updates the bookmarks.
        /// </summary>
        protected void UpdateBookmarks()
        {
            if (m_bookmarks == null)
            {
                m_bookmarks = m_docInfo.TablesData.GetBookmarks();
            }
        }

        /// <summary>
        /// Class initialization.
        /// </summary>
        protected virtual void InitClass()
        {
            if (m_styleSheet == null)
            {
                m_styleSheet = new WordStyleSheet();
            }

            m_currStyleIndex = m_styleSheet.DefaultStyleIndex;
        }

        /// <summary>
        /// Gets text chunk end position.
        /// </summary>
        /// <returns></returns>
        protected virtual long GetChunkEndPosition(long iCurrentPos)
        {
            if (!m_statePositions.IsFirstPass(iCurrentPos))
            {
                UpdateEndPositions(iCurrentPos);
            }

            return m_statePositions.GetMinEndPos(iCurrentPos);
        }

        /// <summary>
        /// Updates end position of current chpx/papx
        /// </summary>
        /// <param name="iEndPos"></param>
        protected virtual void UpdateEndPositions(long iEndPos)
        {
            if (m_statePositions.UpdateCHPxEndPos(iEndPos))
            {
                UpdateCharacterProperties();
            }

            if (m_statePositions.UpdatePAPxEndPos(iEndPos))
            {
                UpdateParagraphProperties();
            }
        }

        /// <summary>
        /// Analizing type of text chunk.
        /// </summary>
        protected virtual void UpdateChunkType()
        {
            if (m_textChunk.Length > 1)
            {
                m_chunkType = WordChunkType.Text;
            }
            else if (m_textChunk.Length == 1) // Maybe text equal Special symbol.
            {
                //        byte[] buf = Encoding.GetBytes(m_textChunk);
                //        switch ((char)buf[0])
                switch (m_textChunk[0])
                {
                    case SpecialCharacters.AnnotationAscii:
                        m_chunkType = WordChunkType.Annotation;
                        break;
                    case SpecialCharacters.ParagraphEnd:
                        if (ParagraphProperties.TablesNestingLevel > 1)
                        {
                            if (ParagraphProperties.IsSubRow)
                            {
                                m_chunkType = WordChunkType.TableRow;
                            }
                            else if (ParagraphProperties.IsSubCell)
                            {
                                if (m_streamsManager.MainStream.Position >= m_statePositions.m_iEndPAPxPos)
                                    m_chunkType = WordChunkType.TableCell;
                                else
                                    m_chunkType = WordChunkType.Text;
                            }
                            else
                            {
                                m_chunkType = WordChunkType.ParagraphEnd;
                            }
                        }
                        else
                        {
                            if (Array.BinarySearch(m_docInfo.TablesData.SectionsTable.Positions, CurrentTextPosition) > 0
                              && CurrentTextPosition != m_docInfo.FibData.ccpText)
                            {
                                //                Debug.WriteLine("If section table positions contained current text position (marker SectionEnd" 
                                //                  + "is absent) then chunk type forcibly set SectionEnd");
                                m_chunkType = WordChunkType.SectionEnd;
                            }
                            else
                            {
                                m_chunkType = WordChunkType.ParagraphEnd;
                            }
                        }

                        break;
                    case SpecialCharacters.ImageAscii:
                        m_chunkType = WordChunkType.Image;
                        break;
                    case SpecialCharacters.ShapeAscii:
                        m_chunkType = WordChunkType.Shape;
                        break;
                    case SpecialCharacters.PageBreak:
                        int[] pos = m_docInfo.FkpData.Tables.SectionsTable.Positions;
                        int cur = 0;
                        if (m_statePositions is MainStatePositions)
                            cur = pos[(m_statePositions as MainStatePositions).SectionIndex + 1];
                        else if (m_statePositions is HFStatePositions)
                            cur = pos[(m_statePositions as HFStatePositions).SectionIndex + 1];
                        uint i = m_docInfo.FkpData.Tables.ConvertCharPosToFileCharPos((uint)cur);
                        if (i == m_streamsManager.MainStream.Position)
                        {
                            m_chunkType = WordChunkType.SectionEnd;
                        }
                        else
                        {
                            m_chunkType = WordChunkType.PageBreak;
                        }

                        break;
                    case SpecialCharacters.ColumnBreak:
                        m_chunkType = WordChunkType.ColumnBreak;
                        break;
                    case SpecialCharacters.TableAscii:
                        if (ParagraphProperties.IsRowMark
                          || ParagraphProperties.Sprms.GetByteArray(WordSprmOptions.sprmTTlp) != null)
                        {
                            m_chunkType = WordChunkType.TableRow;
                        }
                        else if (ParagraphProperties.IsCellMark)
                        {
                            m_chunkType = WordChunkType.TableCell;
                        }
                        else
                        {
                            m_chunkType = WordChunkType.Table;
                        }

                        break;
                    case SpecialCharacters.FootnoteAscii:
                        m_chunkType = WordChunkType.Footnote;
                        break;
                    case SpecialCharacters.FieldBeginMark:
                        m_chunkType = WordChunkType.FieldBeginMark;
                        break;
                    case SpecialCharacters.FieldSeparator:
                        m_chunkType = WordChunkType.FieldSeparator;
                        break;
                    case SpecialCharacters.FieldEndMark:
                        m_chunkType = WordChunkType.FieldEndMark;
                        break;
                    case SpecialCharacters.LineBreakAscii:
                        m_chunkType = WordChunkType.LineBreak;
                        break;
                    case SpecialCharacters.SymbolAscii:
                        if (CharacterProperties.Sprms[WordSprmOptions.sprmCSymbol] != null)
                        {
                            m_chunkType = WordChunkType.Symbol;
                        }
                        else
                        {
                            m_chunkType = WordChunkType.Text;
                        }

                        break;
                    case SpecialCharacters.CurrPageNumber:
                        m_chunkType = WordChunkType.CurrentPageNumber;
                        break;
                    default:
                        m_chunkType = WordChunkType.Text;
                        break;
                }
            }
            else if (m_textChunk.Length == 0)
            {
                m_chunkType = WordChunkType.Text;
            }
        }

        /// <summary>
        /// Updates character properties
        /// </summary>
        protected void UpdateCharacterProperties()
        {
            // Gets current chpx
            CharacterPropertyException chpx = m_statePositions.CurrentChpx;

            m_characterProps = new CharacterProperties(chpx, StyleSheet);
            //      m_fullCharacterProps = new CharacterProperties(chpx, StyleSheet, CurrentStyle.CharacterProperties);
        }

        /// <summary>
        /// Updates paragraph properties
        /// </summary>
        protected void UpdateParagraphProperties()
        {
            // Gets current papx
            ParagraphPropertyException papx = m_statePositions.CurrentPapx;
            ParagraphPropertyException hugePapx = null;
            List<SinglePropertyModifierRecord> papxModifiers = papx.PropertyModifiers.Modifiers;

            // Try to add support of HUGE PAPX

            //      if (papx.PropertyModifiers[WordSprmOptions.sprmPHugePapx3] != null &&  
            //        papx.PropertyModifiers[WordSprmOptions.sprmPHugePapx3].IntValue == 2520 )
            //      {
            //        int dataStreamPos = papx.PropertyModifiers[WordSprmOptions.sprmPHugePapx3].IntValue;
            //        m_dataStream.Position = dataStreamPos;
            //        byte[] arr = new byte[2]; 
            //        m_dataStream.Read(arr, 0, arr.Length);
            //        short papxLength = BitConverter.ToInt16(arr, 0);
            //
            //        ParagraphPropertyException hugePapx3 = new ParagraphPropertyException(m_dataStream, papxLength, m_memConverter, true);
            //        dataStreamPos = 0;
            //      
            //      }
            if (papx.PropertyModifiers[WordSprmOptions.sprmPHugePapx] != null || papx.PropertyModifiers[WordSprmOptions.sprmPHugePapx2] != null)
            {
                byte[] byteArr = papx.PropertyModifiers.GetByteArray(WordSprmOptions.sprmPHugePapx2);
                if (byteArr == null)
                {
                    byteArr = papx.PropertyModifiers.GetByteArray(WordSprmOptions.sprmPHugePapx);
                }

                int dataStreamPos = BitConverter.ToInt32(byteArr, 0);
                m_streamsManager.DataStream.Position = dataStreamPos;
                byteArr = new byte[2];
                m_streamsManager.DataStream.Read(byteArr, 0, byteArr.Length);
                short papxLength = BitConverter.ToInt16(byteArr, 0);

                hugePapx = new ParagraphPropertyException(m_streamsManager.DataStream, papxLength, true);

                //        if (hugePapx.PropertyModifiers[WordSprmOptions.sprmPHugePapx3] != null)
                //        {
                //          dataStreamPos = hugePapx.PropertyModifiers.GetInt(WordSprmOptions.sprmPHugePapx3, -1);
                //          SinglePropertyModifierArray modifiers = ReadHugePapxModifiers(dataStreamPos);
                //        
                //          hugePapx.PropertyModifiers.Modifiers.AddRange(modifiers);
                //        }
            }

            if (hugePapx == null)
            {
                hugePapx = papx;
            }

            m_currStyleIndex = papx.StyleIndex;
            WordStyle style = CurrentStyle;
            m_paragraphProps = new ParagraphProperties(hugePapx);
            m_fullParagraphProps = new ParagraphProperties(hugePapx, style.ParagraphProperties);

#if DEBUG      
      if (style.CharacterProperties != null)
      {
#endif
            //        m_fullCharacterProps.UpdateBaseCharacterProperties(style.CharacterProperties);
#if DEBUG      
      }
#endif
        }

        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    /// <returns></returns>
        //    private SinglePropertyModifierArray ReadHugePapxModifiers(int dataStreamPos)
        //    {
        //      m_dataStream.Position = dataStreamPos;
        //      byte[] byteArr = new byte[2]; 
        //      m_dataStream.Read(byteArr, 0, byteArr.Length);
        //      short papxLength = BitConverter.ToInt16(byteArr, 0);
        //      
        //      ParagraphPropertyException hugePapx = new ParagraphPropertyException(m_dataStream, papxLength, m_memConverter, true);
        //      return hugePapx.PropertyModifiers;
        //    }

        /// <summary>
        /// Reads string from stream using current encoding.
        /// </summary>
        /// <param name="length"></param>
        protected void ReadChunkString(int length)
        {
            if (length < 1)
            {
                throw new ArgumentOutOfRangeException("length must be larger than 0");
            }

            byte[] buff = new byte[length];
            m_streamsManager.MainStream.Read(buff, 0, length);
#if SILVERLIGHT || WP
            if (Encoding == Encoding.UTF8)
                m_textChunk = DocIOEncoding.GetString(buff);
            else
                m_textChunk = Encoding.GetString(buff, 0, buff.Length);
#else
            m_textChunk = Encoding.GetString(buff);
#endif


            // Added for normal support of mergefield symbols
            //      if (buff[0] == 0xAB && buff[buff.Length - 1] == 0xBB)
            //      {
            //        m_textChunk = '�' + m_textChunk + '�';
            //      }
        }

        /// <summary>
        /// Calculates the CP depending on specified startPos and length
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        protected int CalcCP(int startPos, int length)
        {
            uint currCP = m_docInfo.TablesData.ConvertFCToCP((uint)m_streamsManager.MainStream.Position);
            uint startCP = m_docInfo.TablesData.ConvertFCToCP((uint)startPos);
            return (int)(currCP - startCP - length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spContainer"></param>
        /// <returns></returns>
        private bool IsWatermark(MsofbtSpContainer spContainer)
        {
            bool retVal = false;
            byte[] prop = spContainer.GetComplexPropValue((int)FOPTEGroupShape.wzName);
            if (prop != null)
            {
#if SILVERLIGHT || WP
        string shapeName = Encoding.Unicode.GetString( prop, 0, prop.Length );
#else
                string shapeName = Encoding.Unicode.GetString(prop);
#endif

                if (shapeName.StartsWith(MsofbtSpContainer.DEF_PICTMARK_STRING) ||
                    shapeName.StartsWith(MsofbtSpContainer.DEF_TEXTMARK_STRING))
                {
                    retVal = true;
                }
            }

            return retVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spContainer"></param>
        /// <param name="fspa"></param>
        /// <returns></returns>
        private TextBoxShape ReadTextBoxProps(MsofbtSpContainer spContainer, FileShapeAddress fspa)
        {
            TextBoxShape shape = new TextBoxShape();
            InitBaseShapeProps(fspa, shape, spContainer);
            InitTextBoxProps(shape, spContainer);

            return shape;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spContainer"></param>
        /// <param name="fspa"></param>
        /// <returns></returns>
        private PictureShape ReadPictureProps(MsofbtSpContainer spContainer, FileShapeAddress fspa)
        {
            _Blip blip = MsofbtSpContainer.GetBlipFromShapeContainer(spContainer);
            PictureShape shape = null;
            if (blip != null)
            {
                try
                {
                    shape = new PictureShape(blip.ImageRecord);
                }
                catch (ArgumentException)
                {
                    throw new ArgumentException("Document image format is incorrect.");
                }

                InitBaseShapeProps(fspa, shape, spContainer);
                InitPictureProps(shape as PictureShape, spContainer);
            }

            return shape;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spContainer"></param>
        /// <param name="doc"></param>
        private void ReadTextWatermark(MsofbtSpContainer spContainer, WordDocument doc)
        {
            doc.InsertWatermark(WatermarkType.TextWatermark);
            TextWatermark textWatermark = doc.Watermark as TextWatermark;
            //Get text
            byte[] complexProp = spContainer.GetComplexPropValue((int)FOPTEGeoText.gtextUNICODE);
            if (complexProp != null)
            {
#if SILVERLIGHT || WP
        textWatermark.Text = Encoding.Unicode.GetString( complexProp, 0, complexProp.Length );
#else
                textWatermark.Text = Encoding.Unicode.GetString(complexProp);
#endif
            }

            //Get text size
            uint prop = spContainer.GetPropertyValue((int)FOPTEGeoText.gtextSize);
            if (prop != uint.MaxValue)
            {
                textWatermark.Size = (float)(prop >> 16);
            }

            //Get font name
            complexProp = spContainer.GetComplexPropValue((int)FOPTEGeoText.gtextFont);
            if (complexProp != null)
            {
#if SILVERLIGHT || WP
        textWatermark.FontName = Encoding.Unicode.GetString( complexProp, 0, complexProp.Length );
#else
                textWatermark.FontName = Encoding.Unicode.GetString(complexProp);
#endif
            }

            //Get text color
            prop = spContainer.GetPropertyValue((int)FOPTEFillStyle.fillColor);
            if (prop != uint.MaxValue)
            {
                textWatermark.Color = WordColor.ConvertRGBToColor(prop);
            }

            //Get semitransparent property
            prop = spContainer.GetPropertyValue((int)FOPTEFillStyle.fillOpacity);
            if (prop == uint.MaxValue)
            {
                textWatermark.Semitransparent = false;
            }

            //Get watermark layout property.
            prop = spContainer.GetPropertyValue((int)FOPTETransform.rotation);
            if (prop == uint.MaxValue)
            {
                textWatermark.Layout = WatermarkLayout.Horizontal;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spContainer"></param>
        /// <param name="doc"></param>
        /// <param name="fspa"></param>
        private void ReadPictureWatermark(MsofbtSpContainer spContainer, WordDocument doc,
                                           FileShapeAddress fspa)
        {
            doc.InsertWatermark(WatermarkType.PictureWatermark);
            PictureWatermark pictWatermark = doc.Watermark as PictureWatermark;
            //Get watermark picture
            if (spContainer.Pib > 0)
            {
                int imageIndex = spContainer.Pib - 1;
                MsofbtBSE bse = Escher.m_msofbtDggContainer.BstoreContainer.Children[imageIndex] as MsofbtBSE;
                if (bse != null && bse.Blip != null)
                {
                    pictWatermark.WordPicture.LoadImage(bse.Blip.ImageRecord);
                    pictWatermark.OriginalPib = spContainer.Pib;
                    // Apply shape properties to the watermark picture
                    ApplyShapeProperties(pictWatermark.WordPicture, fspa, spContainer.ShapePosition);
                }
            }

            //Get washout property 
            uint prop = spContainer.GetPropertyValue((int)FOPTEBlip.pictureBrightness);
            uint prop1 = spContainer.GetPropertyValue((int)FOPTEBlip.pictureContrast);
            pictWatermark.Washout = (prop == uint.MaxValue && prop1 == uint.MaxValue) ? false : true;
        }

        /// <summary>
        /// Apply shape properties to the watermark picture
        /// </summary>
        /// <param name="picture"></param>
        /// <param name="fspa"></param>
        /// <param name="shapePosition"></param>
        private void ApplyShapeProperties(WPicture picture, FileShapeAddress fspa, MsofbtTertiaryFOPT shapePosition)
        {
            picture.Height = (float)fspa.Height / DLSConstants.TwipsInOnePoint;
            picture.Width = (float)fspa.Width / DLSConstants.TwipsInOnePoint;
            picture.VerticalPosition = (float)fspa.YaTop / DLSConstants.TwipsInOnePoint;
            picture.HorizontalPosition = (float)fspa.XaLeft / DLSConstants.TwipsInOnePoint;
            if (shapePosition.YRelTo != uint.MaxValue)
                picture.VerticalOrigin = (VerticalOrigin)shapePosition.YRelTo;
            if (shapePosition.XRelTo != uint.MaxValue)
                picture.HorizontalOrigin = (HorizontalOrigin)shapePosition.XRelTo;
            if (shapePosition.XAlign != uint.MaxValue)
                picture.HorizontalAlignment = (ShapeHorizontalAlignment)shapePosition.XAlign;
            if (shapePosition.YAlign != uint.MaxValue)
                picture.VerticalAlignment = (ShapeVerticalAlignment)shapePosition.YAlign;
            picture.TextWrappingStyle = fspa.TextWrappingStyle;
            picture.TextWrappingType = fspa.TextWrappingType;
            picture.IsBelowText = fspa.IsBelowText;
            picture.ShapeId = fspa.Spid;
        }
        /// <summary>
        /// Initialize base shape properties.
        /// </summary>
        /// <param name="fspa"></param>
        /// <param name="shape"></param>
        /// <param name="container"></param>
        private void InitBaseShapeProps(FileShapeAddress fspa, ShapeBase shape, MsofbtSpContainer container)
        {
            shape.ShapeProps.Height = fspa.Height;
            shape.ShapeProps.RelHrzPos = fspa.RelHrzPos;
            shape.ShapeProps.RelVrtPos = fspa.RelVrtPos;
            shape.ShapeProps.Spid = fspa.Spid;
            shape.ShapeProps.TextWrappingStyle = fspa.TextWrappingStyle;
            shape.ShapeProps.TextWrappingType = fspa.TextWrappingType;
            shape.ShapeProps.Width = fspa.Width;
            shape.ShapeProps.XaLeft = fspa.XaLeft;
            shape.ShapeProps.XaRight = fspa.XaRight;
            shape.ShapeProps.YaBottom = fspa.YaBottom;
            shape.ShapeProps.YaTop = fspa.YaTop;
            shape.ShapeProps.Spid = fspa.Spid;
            shape.ShapeProps.IsHeaderShape = (this is WordHeaderFooterReader) ? true : false;

            if (container.ShapePosition != null)
            {
                switch (container.ShapePosition.XAlign)
                {
                    case 1:
                        shape.ShapeProps.HorizontalAlignment = ShapeHorizontalAlignment.Left;
                        break;
                    case 2:
                        shape.ShapeProps.HorizontalAlignment = ShapeHorizontalAlignment.Center;
                        break;
                    case 3:
                        shape.ShapeProps.HorizontalAlignment = ShapeHorizontalAlignment.Right;
                        break;
                }
                switch (container.ShapePosition.YAlign)
                {
                    case 1:
                        shape.ShapeProps.VerticalAlignment = ShapeVerticalAlignment.Top;
                        break;
                    case 2:
                        shape.ShapeProps.VerticalAlignment = ShapeVerticalAlignment.Center;
                        break;
                    case 3:
                        shape.ShapeProps.VerticalAlignment = ShapeVerticalAlignment.Bottom;
                        break;
                    case 4:
                        shape.ShapeProps.VerticalAlignment = ShapeVerticalAlignment.Inside;
                        break;
                    case 5:
                        shape.ShapeProps.VerticalAlignment = ShapeVerticalAlignment.Outside;
                        break;
                }
                if (container.ShapePosition.XRelTo != uint.MaxValue)
                {
                    shape.ShapeProps.RelHrzPos = (HorizontalOrigin)container.ShapePosition.XRelTo;
                }

                if (container.ShapePosition.YRelTo != uint.MaxValue)
                {
                    shape.ShapeProps.RelVrtPos = (VerticalOrigin)container.ShapePosition.YRelTo;
                }
            }

            uint prop = container.GetPropertyValue((int)FOPTEGroupShape.fPrint);
            if (prop != uint.MaxValue)
            {
                shape.ShapeProps.IsBelowText = ((prop & 0x20) == 32);
            }
            else
            {
                shape.ShapeProps.IsBelowText = false;
            }
        }

        /// <summary>
        /// Read textbox properties from SpContainer.
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="container"></param>
        private void InitTextBoxProps(TextBoxShape shape, MsofbtSpContainer container)
        {
            //Get textbox line width value
            uint prop = container.GetPropertyValue((int)FOPTELineStyle.lineWidth);
            if (prop != uint.MaxValue)
            {
                shape.TextBoxProps.TxbxLineWidth = (float)prop / msofbtRGFOPTE.DEF_LINE_WIDTH_PT;
            }

            //Get textbox line style property
            prop = container.GetPropertyValue((int)FOPTELineStyle.lineStyle);
            if (prop != uint.MaxValue)
            {
                shape.TextBoxProps.LineStyle = (TextBoxLineStyle)prop;
            }

            //Get textbox line dashing property
            prop = container.GetPropertyValue((int)FOPTELineStyle.lineDashing);
            if (prop != uint.MaxValue)
            {
                shape.TextBoxProps.LineDashing = (LineDashing)prop;
            }

            //Get textbox wrap text property
            prop = container.GetPropertyValue((int)FOPTEText.WrapText);
            if (prop != uint.MaxValue)
            {
                shape.TextBoxProps.WrapText = (WrapMode)prop;
            }

            //Get textbox fill color property
            prop = container.GetPropertyValue((int)FOPTEFillStyle.fillColor);
            if (prop != uint.MaxValue)
            {
                shape.TextBoxProps.FillColor = WordColor.ConvertRGBToColor(prop);
            }

            //Get line color
            prop = container.GetPropertyValue((int)FOPTELineStyle.lineColor);
            if (prop != uint.MaxValue)
            {
                shape.TextBoxProps.LineColor = WordColor.ConvertRGBToColor(prop); ;
            }

            //Has fill color?
            prop = container.GetPropertyValue((int)FOPTEFillStyle.fNoFillHitTest);
            bool hasFill = ((prop & 0x10) == 16);
            if (!hasFill)
            {
                shape.TextBoxProps.FillColor = Color.Empty;
            }

            //Get NoLine property
            prop = container.GetPropertyValue((int)FOPTELineStyle.lineStyleBooleanProperties);
            if (prop != uint.MaxValue)
            {
                (shape.TextBoxProps).NoLine = ((prop & 0x08) == 0);
            }

            //Get textbox TxId  property
            shape.TextBoxProps.TXID = container.GetPropertyValue((int)MsofbtOPT.DEF_TXID);

            //Get shape position properties
            if (container.ShapePosition != null)
            {
                if (container.ShapePosition.XAlign != uint.MaxValue)
                {
                    shape.TextBoxProps.HorizontalAlignment = (ShapeHorizontalAlignment)container.ShapePosition.XAlign;
                }

                if (container.ShapePosition.YAlign != uint.MaxValue)
                {
                    shape.TextBoxProps.VerticalAlignment = (ShapeVerticalAlignment)container.ShapePosition.YAlign;
                }

                if (container.ShapePosition.XRelTo != uint.MaxValue)
                {
                    shape.TextBoxProps.RelHrzPos = (HorizontalOrigin)container.ShapePosition.XRelTo;
                }

                if (container.ShapePosition.YRelTo != uint.MaxValue)
                {
                    shape.TextBoxProps.RelVrtPos = (VerticalOrigin)container.ShapePosition.YRelTo;
                }
            }

            // Read internal textbox text margins
            prop = container.GetPropertyValue((int)FOPTEText.dxTextLeft);
            if (prop != uint.MaxValue)
            {
                shape.TextBoxProps.LeftMargin = prop;
            }

            prop = container.GetPropertyValue((int)FOPTEText.dxTextRight);
            if (prop != uint.MaxValue)
            {
                shape.TextBoxProps.RightMargin = prop;
            }

            prop = container.GetPropertyValue((int)FOPTEText.dyTextTop);
            if (prop != uint.MaxValue)
            {
                shape.TextBoxProps.TopMargin = prop;
            }

            prop = container.GetPropertyValue((int)FOPTEText.dyTextBottom);
            if (prop != uint.MaxValue)
            {
                shape.TextBoxProps.BottomMargin = prop;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pictShape"></param>
        /// <param name="spContainer"></param>
        private void InitPictureProps(PictureShape pictShape, MsofbtSpContainer spContainer)
        {
            // Get alternative text for the picture
            byte[] complexProp = spContainer.GetComplexPropValue((int)FOPTEGroupShape.wzDescription);
            if (complexProp != null)
            {
#if SILVERLIGHT || WP
        pictShape.PictureProps.AlternativeText = Encoding.Unicode.GetString( complexProp, 0, complexProp.Length ).Replace( "\0", string.Empty );
#else
                pictShape.PictureProps.AlternativeText = Encoding.Unicode.GetString(complexProp).Replace("\0", string.Empty);
#endif

            }
            //Get picture brightness
            //      uint prop = MsofbtSpContainer.GetPropertyValue(spContainer, (int)FOPTEBlip.pictureBrightness); 
            //      if (prop != uint.MaxValue)
            //      {
            //        if (prop > msofbtOptProperties.DEF_BRIGHTNESS_BORDER)
            //        {
            //          shape.PictureProps.PictureBrightness  = (prop - msofbtOptProperties.DEF_BRIGHTNESS_BORDER)/
            //            msofbtOptProperties.DEF_BRIGHTNESS_STEP;
            //        }
            //        else
            //        {
            //          shape.PictureProps.PictureBrightness  = 50 +  prop / msofbtOptProperties.DEF_BRIGHTNESS_STEP;
            //        }
            //      }
            //
            //      //Get picture contrast
            //      prop = MsofbtSpContainer.GetPropertyValue(spContainer, (int)FOPTEBlip.pictureContrast);
            //      if (prop != uint.MaxValue)
            //      {
            //        if (prop < msofbtOptProperties.DEF_CONTRAST_BORDER)
            //        {
            //          shape.PictureProps.PictureContrast =  prop / msofbtOptProperties.DEF_CONTRAST_STEP;
            //        }
            //      }
            //
            //      //Get picture color
            //      prop = MsofbtSpContainer.GetPropertyValue(spContainer, (int)FOPTEBlip.pictureActive);
            //      if (prop != uint.MaxValue)
            //      {
            //        shape.PictureProps.PictureColor = PictureColor.Automatic;
            //        if (shape.PictureProps.PictureBrightness == 85 && shape.PictureProps.PictureContrast == 15)
            //        {
            //          shape.PictureProps.PictureColor = PictureColor.Washout;
            //        }
            //        else if ((prop & 0x06) == 6)
            //        {
            //          shape.PictureProps.PictureColor = PictureColor.BlackAndWhite;
            //        }
            //        else if ((prop & 0x04) != 0)
            //        {  
            //          shape.PictureProps.PictureColor = PictureColor.Grayscale;
            //        }        
            //      }

            //      prop = MsofbtSpContainer.GetPropertyValue(spContainer, (int)FOPTEBlip.cropFromLeft);
            //      if (prop != uint.MaxValue)
            //      {
            //        shape.PictureProps.CropFromLeft =  (float)prop/ msofbtOptProperties.DEF_CROP_STEP;
            //      }
            //
            //      prop = MsofbtSpContainer.GetPropertyValue(spContainer, (int)FOPTEBlip.cropFromRight);
            //      if (prop != uint.MaxValue)
            //      {
            //        shape.PictureProps.CropFromRight = (float)prop/ msofbtOptProperties.DEF_CROP_STEP;
            //      }
            //
            //      prop = MsofbtSpContainer.GetPropertyValue(spContainer, (int)FOPTEBlip.cropFromTop);
            //      if (prop != uint.MaxValue)
            //      {
            //        shape.PictureProps.CropFromTop = (float) prop/ msofbtOptProperties.DEF_CROP_STEP;
            //      }
            //
            //      prop = MsofbtSpContainer.GetPropertyValue(spContainer, (int)FOPTEBlip.cropFromBottom);
            //      if (prop != uint.MaxValue)
            //      {
            //        shape.PictureProps.CropFromBottom = (float) prop/ msofbtOptProperties.DEF_CROP_STEP;
            //      }
            //      byte[] prop = MsofbtSpContainer.GetComplexPropValue(spContainer, (int)FOPTEGroupShape.wzName);
            //      if (prop != null)
            //      {
            //        string shapeName = Encoding.Unicode.GetString(prop);
            //        pictShape.IsWatermark = shapeName.StartsWith(DEF_WATERMARK_STRING);
            //      }
        }

        /// <summary>
        /// Read and parse text chunk.
        /// <remark>If text chunk consists special symbols - it is splitted.</remark>
        /// </summary>
        ///  <param name="iLength"></param>
        /// <returns></returns>
        private int ReadAndParseTextChunk(int iLength)
        {
            ReadChunkString(iLength);

            // Checks if text chunk consists Special symbols
            if (m_textChunk.Length > 1)
            {
                int iSpecCharPos = -1;

                // Handle special cases section (when there are '\0' symbols included in text).
                if (m_textChunk.Length >= 10 && CharacterProperties.Special && CharacterProperties.Sprms[WordSprmOptions.sprmCPicLocation] == null)
                {
                    m_textChunk = string.Empty;
                    return iLength;
                }

                else if (m_textChunk.Length > 10)
                {
                    if (IsZeroChunk())
                    {
                        m_textChunk = string.Empty;
                        return iLength;
                    }
                }

                m_textChunk = m_textChunk.Trim(new char[] { '\0' });
                // end of special section

                if ((iSpecCharPos = m_textChunk.IndexOfAny(SpecialCharacters.SpecialSymbolArr)) > -1)
                {
                    // If first char is Special symbol - get it.
                    if (iSpecCharPos == 0)
                    {
                        iSpecCharPos = 1;
                    }

                    // If chunk consist special symbols - split it,
                    // corrects text chunk and rollback stream cursor
                    m_textChunk = m_textChunk.Substring(0, iSpecCharPos);
                    m_streamsManager.MainStream.Position -= iLength - iSpecCharPos * EncodingCharSize;
                    //m_stgStream.Position -= iLength - iSpecCharPos;          

                    return iSpecCharPos;
                }
                //        else if (m_textChunk.Length > 10 && CharacterProperties.Special)
                //        {
                //            m_textChunk = "";
                //        }
            }

            return iLength;
        }

        /// <summary>
        /// Checks id chunk includes special characters.
        /// </summary>
        /// <returns></returns>
        private int CheckSpecCharacters(int iLength)
        {
            int iSpecCharPos = -1;
            if ((iSpecCharPos = m_textChunk.IndexOfAny(SpecialCharacters.SpecialSymbolArr)) > -1)
            {
                // If first char is Special symbol - get it.
                if (iSpecCharPos == 0)
                {
                    iSpecCharPos = 1;
                }

                // If chunk consist special symbols - split it,
                // corrects text chunk and rollback stream cursor
                m_textChunk = m_textChunk.Substring(0, iSpecCharPos);
                m_streamsManager.MainStream.Position -= iLength - iSpecCharPos * EncodingCharSize;
                iLength = iSpecCharPos;
            }

            return iLength;
        }

        /// <summary>
        /// Defines if current chunk is "Zero" chunk. 
        /// </summary>
        /// <returns></returns>
        private bool IsZeroChunk()
        {
            bool retVal = false;
            if (m_textChunk[0] == '\0')
            {
                retVal = true;
                for (int i = 1; i < m_textChunk.Length - 1; i++)
                {
                    if (m_textChunk[i] != m_textChunk[i + 1])
                    {
                        retVal = false;
                        break;
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int CalculateChunkLength()
        {
            // 1) Gets START position of text chunk.
            long iCurrentPos = m_streamsManager.MainStream.Position;

            // 2) Checks if current position is less from end of text run.
            if (m_statePositions.IsEndOfText(iCurrentPos))
            {
                m_chunkType = WordChunkType.DocumentEnd;
                return 0; // Return from function because it is document end.
            }

            // 3) Gets END position of text chunk.
            long iEndPos = GetChunkEndPosition(iCurrentPos);
            //Update main stream position if current position is not in the range of current piece table start position
            if (iCurrentPos < m_statePositions.m_iStartPieceTablePos)
            {
               iCurrentPos = m_streamsManager.MainStream.Position = m_statePositions.m_iStartPieceTablePos;
            }
            // 4) Checks if current position is less then the end of subdocument item 
            if (m_statePositions.IsEndOfSubdocItemText(iCurrentPos))
            {
                // End of text for specific header/footer
                m_chunkType = WordChunkType.EndOfSubdocText;
                return 0;
            }

            if (iEndPos < 0)
            {
                m_chunkType = WordChunkType.DocumentEnd;
                return 0; // Return from function because it is document end.
            }

            // Check whether current text position "belongs" to next subdocument item.
            if (this is WordSubdocumentReader && (this as WordSubdocumentReader).IsNextItemPos)
            {
                m_chunkType = WordChunkType.Text;
                return 0;
            }

            if (m_bookmarks != null && m_bookmarks.Length != 0)
            {
                // 5) Define if current chunk contains bookmarks ans split this chunk
                return GetBookmarkChunkLen((int)iCurrentPos, (int)iEndPos);
            }
            else
            {
                return (int)(iEndPos - iCurrentPos);
            }

        }

        /// <summary>
        /// Checks if current body belongs to next paragraph item.
        /// </summary>
        /// <returns></returns>
        private bool IsNextItem()
        {
            if (this is WordEndnoteReader)
            {
                return (this as WordEndnoteReader).IsNextItem;
            }
            else if (this is WordFootnoteReader)
            {
                return (this as WordFootnoteReader).IsNextItem;
            }

            return false;
        }

        /// <summary>
        /// Counts minimal chunk length with field.
        /// </summary>
        /// <param name="curPos"></param>
        /// <param name="endPos"></param>
        /// <returns></returns>
        private int GetFldChunkLen(int curPos, int endPos)
        {
            int retVal = endPos - curPos;
            DocIOSortedList<int, FieldDescriptor> subDocFields = m_docInfo.TablesData.Fields.GetFieldsForSubDoc(m_type);
            if (subDocFields != null)
            {
                int startTextPos = CurrentTextPosition;
                int endTextPos = startTextPos + retVal;
                List<Int32> fldPosArray = new List<Int32>();
                foreach (int key in subDocFields.Keys)
                {
                    if (key >= startTextPos || key <= endPos)
                        fldPosArray.Add(key);
                }
                if (fldPosArray.Count > 0)
                {
                    fldPosArray.Sort();
                    retVal = fldPosArray[0] - startTextPos;
                    retVal += (retVal == 0) ? 1 : 0;
                }
            }

            return retVal;
        }

        /// <summary>
        /// Checks the current text chunk.
        /// </summary>
        /// <returns></returns>
        private bool CheckCurTextChunk(int curChunkPosLen)
        {
            m_isCellMark = false;

            if (curChunkPosLen < 0)
                return false;

            //Update data for table cell bookmarks 
            if (m_chunkType == WordChunkType.TableCell)
            {
                m_cellCounter += 1;
                m_isCellMark = true;
            }
            else if (m_chunkType == WordChunkType.TableRow)
            {
                m_cellCounter = 0;
            }

            //Check current chunk on trash
            if (m_textChunk == string.Empty || IsZeroChunk() ||
              (m_textChunk.Length > 10 && CharacterProperties.Special &&
              CharacterProperties.Sprms[WordSprmOptions.sprmCPicLocation] == null))
            {
                m_textChunk = string.Empty;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get new chunk length according bookmark.
        /// </summary>
        /// <param name="startStreamPos"></param>
        /// <param name="endStreamPos"></param>
        /// <returns></returns>
        private int CalculateBkmkChunkLen(int startStreamPos, int endStreamPos)
        {
            int bkmkCharPos = (m_isBookmarkStart) ? m_currentBookmark.StartPos : m_currentBookmark.EndPos;
            int bkmkFilePos = (int)m_docInfo.TablesData.ConvertCharPosToFileCharPos((uint)bkmkCharPos);

            if (bkmkFilePos > endStreamPos)
            {
                bkmkFilePos = (int)endStreamPos;
            }

            return bkmkFilePos - (int)startStreamPos;
        }

        /// <summary>
        /// Gets the bookmark chunk len.
        /// </summary>
        /// <param name="curDocStreamPos">The cur doc stream pos.</param>
        /// <param name="endDocStreamPos">The end doc stream pos.</param>
        /// <returns></returns>
        private int GetBookmarkChunkLen(long curDocStreamPos, long endDocStreamPos)
        {
            m_currentBookmark = null;
            int newChunkLen = (int)(endDocStreamPos - curDocStreamPos);

            int realChunkLen = CheckAndGetCurChunkLen(newChunkLen);
            if (realChunkLen == -1)
            {
                return newChunkLen;
            }

            int startTextPos = CurrentTextPosition;
            int endTextPos = startTextPos + realChunkLen;

            if (this.HasTableBody)
            {
                CheckTableBookmark(startTextPos, endTextPos);
                if (m_currentBookmark != null)
                {
                    return m_isCellMark ? newChunkLen : 0;
                }
            }

            SetCurrentBookmark(startTextPos, endTextPos);

            if (m_currentBookmark != null)
            {
                int bkmkCharPos = (m_isBookmarkStart) ? m_currentBookmark.StartPos : m_currentBookmark.EndPos;
                int bkmkFilePos = (int)m_docInfo.TablesData.ConvertCharPosToFileCharPos((uint)bkmkCharPos);

                if (bkmkFilePos > endDocStreamPos)
                {
                    bkmkFilePos = (int)endDocStreamPos;
                }

                newChunkLen = bkmkFilePos - (int)curDocStreamPos;
            }

            return newChunkLen;
        }


        /// <summary>
        /// Checks the table bookmark.
        /// </summary>
        private void CheckTableBookmark(int startTextPos, int endTextPos)
        {
            int bkmkIndex = -1;
            BookmarkInfo bkmkInfo = null;
            for (int i = 0, cnt = m_bookmarks.Length; i < cnt; i++)
            {
                bkmkInfo = m_bookmarks[i];

                if (bkmkInfo.StartCellIndex == m_cellCounter && m_isCellMark)
                {
                    if (bkmkInfo.StartPos <= startTextPos && bkmkInfo.EndPos > startTextPos)
                    {
                        m_isBookmarkStart = true;
                        bkmkIndex = i;
                        break;
                    }
                }
                else if (bkmkInfo.EndCellIndex == m_cellCounter - 1 && m_isCellMark)
                {
                    if (bkmkInfo.EndPos == endTextPos || bkmkInfo.StartCellIndex == -1)
                    {
                        m_isBookmarkStart = false;
                        bkmkIndex = i;
                        break;
                    }
                }
            }

            DisableBookmark(bkmkIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        private void SetCurrentBookmark(int startPos, int endPos)
        {
            int bkmkIndex = -1;
            int minBkmkPos = int.MaxValue;

            // Find bookmark
            for (int i = 0; i < m_bookmarks.Length; i++)
            {
                BookmarkInfo bkmkInfo = m_bookmarks[i];
                bool isStart = true;

                while (true)
                {
                    int checkPos = isStart ? bkmkInfo.StartPos : bkmkInfo.EndPos;

                    if (checkPos != int.MaxValue)
                    {
                        if (checkPos >= startPos && checkPos < minBkmkPos && checkPos <= endPos)
                        {
                            minBkmkPos = checkPos;
                            bkmkIndex = i;
                            m_isBookmarkStart = isStart;
                        }
                    }

                    if (isStart) isStart = false;
                    else break;
                }
            }

            DisableBookmark(bkmkIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bookmarkIndex"></param>
        private void DisableBookmark(int bookmarkIndex)
        {
            if (bookmarkIndex != -1)
            {
                BookmarkInfo bkmkInfo = (m_bookmarks[bookmarkIndex] as BookmarkInfo);
                m_currentBookmark = bkmkInfo.Clone();

                if (m_isBookmarkStart)
                {
                    bkmkInfo.StartPos = int.MaxValue;
                    bkmkInfo.StartCellIndex = -1;
                }
                else
                {
                    bkmkInfo.EndPos = int.MaxValue;
                    bkmkInfo.EndCellIndex = -1;
                }
            }
            else
            {
                m_currentBookmark = null;
            }
        }

        /// <summary>
        /// Checks if current chunk is FieldBeginChunk or FieldEndChunk, 
        /// return real length of current chunk.
        /// </summary>
        /// <param name="curChunkPosLen"></param>
        /// <returns>Current chunk length</returns>
        private int CheckAndGetCurChunkLen(int curChunkPosLen)
        {
            m_isCellMark = false;
            int realChunkLen = -1;
            if (curChunkPosLen < 1)
            {
                return realChunkLen;
            }

            long iCurrentPos = m_streamsManager.MainStream.Position;
            ReadChunkString(curChunkPosLen);
            UpdateChunkType();

            //Update data for table cell bookmarks 
            if (m_chunkType == WordChunkType.TableCell)
            {
                m_cellCounter += 1;
                m_isCellMark = true;
            }
            else if (m_chunkType == WordChunkType.TableRow)
            {
                m_cellCounter = 0;
            }

            //Check current chunk on trash
            if (m_textChunk == string.Empty || IsZeroChunk() ||
               (m_textChunk.Length > 10 && CharacterProperties.Special &&
                CharacterProperties.Sprms[WordSprmOptions.sprmCPicLocation] == null))
            {
                realChunkLen = -1;
            }
            else
            {
                CheckSpecCharacters(curChunkPosLen);
                realChunkLen = m_textChunk.Length;
            }

            m_textChunk = string.Empty;
            m_chunkType = WordChunkType.Text;
            m_streamsManager.MainStream.Position = iCurrentPos;

            return realChunkLen;
        }

        /// <summary>
        /// 
        /// </summary>
        private int CheckForSymbols(int chunkLen)
        {
            int newChunkLen = chunkLen;
            if (CharacterProperties.Sprms[WordSprmOptions.sprmCSymbol] != null && chunkLen > 1)
            {
                long iCurrentPos = m_streamsManager.MainStream.Position;
                ReadChunkString(chunkLen);
                if (m_textChunk[0] == SpecialCharacters.SymbolAscii)
                {
                    newChunkLen = 1;
                }

                m_streamsManager.MainStream.Position = iCurrentPos;
                m_textChunk = string.Empty;
            }

            return newChunkLen;
        }

        /// <summary>
        /// Gets the document version.
        /// </summary>
        /// <returns></returns>
        private DocumentVersion GetDocVersion()
        {
            int dopLen = m_docInfo.FibData.lcbDop;

            if (dopLen < DEF_WORD9_DOP_LEN)
            {
                return DocumentVersion.Word97;
            }
            else if (dopLen == DEF_WORD9_DOP_LEN)
            {
                return DocumentVersion.Word2000;
            }
            else if (dopLen <= DEF_WORD10_DOP_LEN)
            {
                return DocumentVersion.Word2002;
            }
            else if (dopLen <= DEF_WORD11_DOP_LEN)
            {
                return DocumentVersion.Word2003;
            }
            else
            {
                return DocumentVersion.Word2007;
            }
        }
        #endregion
    }
}
