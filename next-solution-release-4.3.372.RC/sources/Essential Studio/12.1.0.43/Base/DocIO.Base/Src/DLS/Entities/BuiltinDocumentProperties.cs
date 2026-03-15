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

using Syncfusion.DocIO.DLS.XML;
using System.Collections.Generic;
using Syncfusion.CompoundFile.DocIO;
using Syncfusion.CompoundFile.DocIO.Native;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents the document properties in a MS Word document.
    /// </summary>
    public class BuiltinDocumentProperties : SummaryDocumentProperties
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<int, DocumentProperty> m_documentHash = new Dictionary<int, DocumentProperty>();
        #endregion

        #region Class Document properties
        /// <summary>
        /// Gets / Sets the category of the document.
        /// </summary>
        public string Category
        {
            get
            {
                return m_documentHash.ContainsKey((int)BuiltInProperty.Category) ?
                  this[(PIDDSI)BuiltInProperty.Category].Text
                  : null;
            }
            set
            {
                SetPropertyValue((PIDDSI)1000, value);
                this[(PIDDSI)BuiltInProperty.Category].Text = value;
                //        this[ PIDDSI.Category ].Value = value;
            }
        }
        /// <summary>
        /// Represents the number of bytes in the document.
        /// </summary>
        public int BytesCount
        {
            get
            {
                return m_documentHash.ContainsKey((int)PIDDSI.ByteCount) ?
                  this[PIDDSI.ByteCount].Int32//ToInt()
                  : int.MinValue;
            }
            internal set
            {
                SetPropertyValue(PIDDSI.ByteCount, value);
                this[PIDDSI.ByteCount].Int32 = value;
            }
        }
        /// <summary>
        /// Gets the number of lines in the document.
        /// </summary>
        public int LinesCount
        {
            get
            {
                return m_documentHash.ContainsKey((int)PIDDSI.LineCount) ?
                  this[PIDDSI.LineCount].ToInt()
                  : int.MinValue;
            }
            internal set
            {
                SetPropertyValue(PIDDSI.LineCount, value);
                this[PIDDSI.LineCount].Int32 = value;
            }
        }
        /// <summary>
        /// Gets the number of paragraphs in the document.
        /// </summary>
        public int ParagraphCount
        {
            get
            {
                return m_documentHash.ContainsKey((int)PIDDSI.ParCount) ?
                  this[PIDDSI.ParCount].ToInt()
                  : int.MinValue;
            }
            internal set
            {
                SetPropertyValue(PIDDSI.ParCount, value);
                this[PIDDSI.ParCount].Int32 = value;
                //        m_documentHash[ ( int )PIDDSI.ParCount ] = value;
            }
        }
        /// <summary>
        /// Gets slide count.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public int SlideCount
        {
            get
            {
                return m_documentHash.ContainsKey((int)PIDDSI.SlideCount) ?
                  this[PIDDSI.SlideCount].ToInt()
                  : int.MinValue;
            }
            internal set
            {
                SetPropertyValue((PIDDSI)BuiltInProperty.SlideCount, value);
                this[(PIDDSI)BuiltInProperty.SlideCount].Int32 = value;
            }
        }
        /// <summary>
        /// Gets Note count.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public int NoteCount
        {
            get
            {
                return m_documentHash.ContainsKey((int)PIDDSI.NoteCount) ?
                  this[PIDDSI.NoteCount].ToInt()
                  : int.MinValue;
            }
            internal set
            {
                SetPropertyValue((PIDDSI)BuiltInProperty.NoteCount, value);
                this[(PIDDSI)BuiltInProperty.NoteCount].Int32 = value;
            }
        }
        /// <summary>
        /// Gets hidden count
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public int HiddenCount
        {
            get
            {
                return m_documentHash.ContainsKey((int)PIDDSI.HiddenCount) ?
                  this[PIDDSI.HiddenCount].ToInt()
                  : int.MinValue;
            }
            internal set
            {
                SetPropertyValue((PIDDSI)BuiltInProperty.HiddenCount, value);
                this[(PIDDSI)BuiltInProperty.HiddenCount].Int32 = value;
            }
        }
        /// <summary>
        /// Gets / Sets Company property.
        /// </summary>
        public string Company
        {
            get
            {
                return m_documentHash.ContainsKey((int)BuiltInProperty.Company) ?
                  this[(PIDDSI)BuiltInProperty.Company].Text
                  : null;
            }
            set
            {
                //        this[ PIDDSI.Company ].Value = value;
                SetPropertyValue((PIDDSI)BuiltInProperty.Company, value);
                this[(PIDDSI)BuiltInProperty.Company].Text = value;
            }
        }
        /// <summary>
        /// Gets / Sets Manager property.
        /// </summary>
        public string Manager
        {
            get
            {
                return m_documentHash.ContainsKey((int)BuiltInProperty.Manager) ?
                  this[(PIDDSI)BuiltInProperty.Manager].Text
                  : null;
            }
            set
            {
                SetPropertyValue((PIDDSI)BuiltInProperty.Manager, value);
                this[(PIDDSI)BuiltInProperty.Manager].Text = value;
                //        this[ PIDDSI.Manager ].Value = value;
            }
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets document hash
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal Dictionary<int, DocumentProperty> DocumentHash
        {
            get
            {
                return m_documentHash;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal DocumentProperty this[PIDDSI piddsi]
        {
            get
            {
                return m_documentHash[(int)piddsi];
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initialize
        /// </summary>
        internal BuiltinDocumentProperties()
            : this(0, 0)
        {
        }
        /// <summary>
        /// Initialize
        /// </summary>
        internal BuiltinDocumentProperties(int docCount, int summCount)
            : base(summCount)
        {
            m_documentHash = new Dictionary<int, DocumentProperty>(docCount);
        }
        /// <summary>
        /// Initialize
        /// </summary>
        internal BuiltinDocumentProperties(WordDocument doc)
            : base(doc)
        {
            m_documentHash = new Dictionary<int, DocumentProperty>();
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// isSummary = true - summary property
        /// isSummary = false - document summary property
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool HasKey(int key)
        {
            return m_documentHash.ContainsKey(key);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public BuiltinDocumentProperties Clone()
        {
            BuiltinDocumentProperties builtinDocumentProperties = new BuiltinDocumentProperties(m_documentHash.Count, m_summaryHash.Count);

            foreach (int key in m_documentHash.Keys)
            {
                DocumentProperty property = m_documentHash[key];
                builtinDocumentProperties.m_documentHash.Add(key, property.Clone()as DocumentProperty);
            }

            foreach (int key in m_summaryHash.Keys)
            {
                DocumentProperty property = m_summaryHash[key];
                builtinDocumentProperties.m_summaryHash.Add(key, property.Clone() as DocumentProperty);
            }

            return builtinDocumentProperties;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="piddsi"></param>
        /// <param name="value"></param>
        internal void SetPropertyValue(PIDDSI piddsi, object value)
        {
            if (m_documentHash.ContainsKey((int)piddsi))
            {
                this[piddsi].Value = value;
            }
            else
            {
                DocumentProperty property = new DocumentProperty((BuiltInProperty)piddsi, value);
                m_documentHash[(int)piddsi] = property;
            }
        }
        #endregion
#if !SILVERLIGHT && !WP
        #region Class XDLSSerializable implementation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);

            // Document summary properties
            if (HasKey((int)PIDDSI.Company))
            {
                writer.WriteValue(XDLSConstants.PropertiesCompanyAttr, Company);
            }
            if (HasKey((int)PIDDSI.Manager))
            {
                writer.WriteValue(XDLSConstants.PropertiesManagerAttr, Manager);
            }
            if (HasKey((int)PIDDSI.Category))
            {
                writer.WriteValue(XDLSConstants.PropertiesCategoryAttr, Category);
            }
            if (HasKey((int)PIDDSI.ByteCount))
            {
                writer.WriteValue(XDLSConstants.PropertiesBytesCountAttr, BytesCount);
            }
            if (HasKey((int)PIDDSI.LineCount))
            {
                writer.WriteValue(XDLSConstants.PropertiesLinesCountAttr, LinesCount);
            }
            if (HasKey((int)PIDDSI.ParCount))
            {
                writer.WriteValue(XDLSConstants.PropertiesParagraphCountAttr, ParagraphCount);
            }
            if (HasKey((int)PIDDSI.SlideCount))
            {
                writer.WriteValue(XDLSConstants.PropertiesSlideCountAttr, SlideCount);
            }
            if (HasKey((int)PIDDSI.NoteCount))
            {
                writer.WriteValue(XDLSConstants.PropertiesNoteCountAttr, NoteCount);
            }
            if (HasKey((int)PIDDSI.HiddenCount))
            {
                writer.WriteValue(XDLSConstants.PropertiesHiddenCountAttr, HiddenCount);
            }

            //      // Summary properties
            //      if( HasKey( ( int )PIDSI.Author, true ) )
            //      {
            //        writer.WriteValue( DocIOXMLConstants.PropertiesAuthorAttr, Author );
            //      }
            //      if( HasKey( ( int )PIDSI.Appname, true ) )
            //      {
            //        writer.WriteValue( DocIOXMLConstants.PropertiesApplicationNameAttr, ApplicationName );
            //      }
            //      if( HasKey( ( int )PIDSI.Title, true ) )
            //      {
            //        writer.WriteValue( DocIOXMLConstants.PropertiesTitleAttr, Title );
            //      }
            //      if( HasKey( ( int )PIDSI.Subject, true ) )
            //      {
            //        writer.WriteValue( DocIOXMLConstants.PropertiesSubjectAttr, Subject );
            //      }
            //      if( HasKey( ( int )PIDSI.Keywords, true ) )
            //      {
            //        writer.WriteValue( DocIOXMLConstants.PropertiesKeywordsAttr, Keywords );
            //      }
            //      if( HasKey( ( int )PIDSI.Comments, true ) )
            //      {
            //        writer.WriteValue( DocIOXMLConstants.PropertiesCommentsAttr, Comments );
            //      }
            //      if( HasKey( ( int )PIDSI.Template, true ) )
            //      {
            //        writer.WriteValue( DocIOXMLConstants.PropertiesTemplateAttr, Template );
            //      }
            //      if( HasKey( ( int )PIDSI.LastAuthor, true ) )
            //      {
            //        writer.WriteValue( DocIOXMLConstants.PropertiesLastAuthorAttr, LastAuthor );
            //      }
            //      if( HasKey( ( int )PIDSI.Revnumber, true ) )
            //      {
            //        writer.WriteValue( DocIOXMLConstants.PropertiesRevisionNumberAttr, RevisionNumber );
            //      }
            //      if( HasKey( ( int )PIDSI.EditTime, true ) )
            //      {
            //        writer.WriteValue( DocIOXMLConstants.PropertiesEditTimeAttr, EditTime );
            //      }
            //      if( HasKey( ( int )PIDSI.LastPrinted, true ) )
            //      {
            //        writer.WriteValue( DocIOXMLConstants.PropertiesLastPrintedAttr, LastPrinted );
            //      }
            //      if( HasKey( ( int )PIDSI.Create_dtm, true ) )
            //      {
            //        writer.WriteValue( DocIOXMLConstants.PropertiesCreateDateAttr, CreateDate );
            //      }
            //      if( HasKey( ( int )PIDSI.LastSave_dtm, true ) )
            //      {
            //        writer.WriteValue( DocIOXMLConstants.PropertiesLastSaveDateAttr, LastSaveDate );
            //      }
            //      if( HasKey( ( int )PIDSI.Pagecount, true ) )
            //      {
            //        writer.WriteValue( DocIOXMLConstants.PropertiesPageCountAttr, PageCount );
            //      }
            //      if( HasKey( ( int )PIDSI.Wordcount, true ) )
            //      {
            //        writer.WriteValue( DocIOXMLConstants.PropertiesWordCountAttr, WordCount );
            //      }
            //      if( HasKey( ( int )PIDSI.Charcount, true ) )
            //      {
            //        writer.WriteValue( DocIOXMLConstants.PropertiesCharCountAttr, CharCount );
            //      }
            //      if( HasKey( ( int )PIDSI.Thumbnail, true ) )
            //      {
            //        writer.WriteValue( DocIOXMLConstants.PropertiesThumbnailAttr, Thumbnail );
            //      }
            //      if( HasKey( ( int )PIDSI.Doc_security, true ) )
            //      {
            //        writer.WriteValue( DocIOXMLConstants.PropertiesDocSecurityAttr, DocSecurity );
            //      }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            // Document summary properties
            if (reader.HasAttribute(XDLSConstants.PropertiesCompanyAttr))
            {
                Company = reader.ReadString(XDLSConstants.PropertiesCompanyAttr);
            }
            if (reader.HasAttribute(XDLSConstants.PropertiesManagerAttr))
            {
                Manager = reader.ReadString(XDLSConstants.PropertiesManagerAttr);
            }
            if (reader.HasAttribute(XDLSConstants.PropertiesCategoryAttr))
            {
                Category = reader.ReadString(XDLSConstants.PropertiesCategoryAttr);
            }
            if (reader.HasAttribute(XDLSConstants.PropertiesBytesCountAttr))
            {
                SetPropertyValue(PIDDSI.ByteCount, reader.ReadInt(XDLSConstants.PropertiesBytesCountAttr));
            }
            if (reader.HasAttribute(XDLSConstants.PropertiesLinesCountAttr))
            {
                SetPropertyValue(PIDDSI.LineCount, reader.ReadInt(XDLSConstants.PropertiesLinesCountAttr));
            }
            if (reader.HasAttribute(XDLSConstants.PropertiesParagraphCountAttr))
            {
                SetPropertyValue(PIDDSI.ParCount, reader.ReadInt(XDLSConstants.PropertiesParagraphCountAttr));
            }
            if (reader.HasAttribute(XDLSConstants.PropertiesSlideCountAttr))
            {
                SetPropertyValue(PIDDSI.SlideCount, reader.ReadInt(XDLSConstants.PropertiesSlideCountAttr));
            }
            if (reader.HasAttribute(XDLSConstants.PropertiesNoteCountAttr))
            {
                SetPropertyValue(PIDDSI.NoteCount, reader.ReadInt(XDLSConstants.PropertiesNoteCountAttr));
            }
            if (reader.HasAttribute(XDLSConstants.PropertiesHiddenCountAttr))
            {
                SetPropertyValue(PIDDSI.HiddenCount, reader.ReadInt(XDLSConstants.PropertiesHiddenCountAttr));
            }

            //      // Summary properties
            //      if( reader.HasAttribute( DocIOXMLConstants.PropertiesAuthorAttr ) )
            //      {
            //        Author = reader.ReadString( DocIOXMLConstants.PropertiesAuthorAttr );
            //      }
            //      if( reader.HasAttribute( DocIOXMLConstants.PropertiesApplicationNameAttr ) )
            //      {
            //        ApplicationName = reader.ReadString( DocIOXMLConstants.PropertiesApplicationNameAttr );
            //      }
            //      if( reader.HasAttribute( DocIOXMLConstants.PropertiesTitleAttr ) )
            //      {
            //        Title = reader.ReadString( DocIOXMLConstants.PropertiesTitleAttr );
            //      }
            //      if( reader.HasAttribute( DocIOXMLConstants.PropertiesSubjectAttr ) )
            //      {
            //        Subject = reader.ReadString( DocIOXMLConstants.PropertiesSubjectAttr );
            //      }
            //      if( reader.HasAttribute( DocIOXMLConstants.PropertiesKeywordsAttr ) )
            //      {
            //        Keywords = reader.ReadString( DocIOXMLConstants.PropertiesKeywordsAttr );
            //      }
            //      if( reader.HasAttribute( DocIOXMLConstants.PropertiesCommentsAttr ) )
            //      {
            //        Comments = reader.ReadString( DocIOXMLConstants.PropertiesCommentsAttr );
            //      }
            //      if( reader.HasAttribute( DocIOXMLConstants.PropertiesTemplateAttr ) )
            //      {
            //        Template = reader.ReadString( DocIOXMLConstants.PropertiesTemplateAttr );
            //      }
            //      if( reader.HasAttribute( DocIOXMLConstants.PropertiesLastAuthorAttr ) )
            //      {
            //        LastAuthor = reader.ReadString( DocIOXMLConstants.PropertiesLastAuthorAttr );
            //      }
            //      if( reader.HasAttribute( DocIOXMLConstants.PropertiesRevisionNumberAttr ) )
            //      {
            //        RevisionNumber = reader.ReadString( DocIOXMLConstants.PropertiesRevisionNumberAttr );
            //      }
            //      if( reader.HasAttribute( DocIOXMLConstants.PropertiesEditTimeAttr ) )
            //      {
            //        EditTime = reader.ReadDateTime( DocIOXMLConstants.PropertiesEditTimeAttr );
            //      }
            //      if( reader.HasAttribute( DocIOXMLConstants.PropertiesLastPrintedAttr ) )
            //      {
            //        LastPrinted = reader.ReadDateTime( DocIOXMLConstants.PropertiesLastPrintedAttr );
            //      }
            //      if( reader.HasAttribute( DocIOXMLConstants.PropertiesCreateDateAttr ) )
            //      {
            //        CreateDate = reader.ReadDateTime( DocIOXMLConstants.PropertiesCreateDateAttr );
            //      }
            //      if( reader.HasAttribute( DocIOXMLConstants.PropertiesLastSaveDateAttr ) )
            //      {
            //        LastSaveDate = reader.ReadDateTime( DocIOXMLConstants.PropertiesLastSaveDateAttr );
            //      }
            //      if( reader.HasAttribute( DocIOXMLConstants.PropertiesPageCountAttr ) )
            //      {
            //        m_summaryHash[ (int)PIDSI.Pagecount ] = reader.ReadInt( DocIOXMLConstants.PropertiesPageCountAttr );
            //      }
            //      if( reader.HasAttribute( DocIOXMLConstants.PropertiesWordCountAttr ) )
            //      {
            //        m_summaryHash[ (int)PIDSI.Wordcount ] = reader.ReadInt( DocIOXMLConstants.PropertiesWordCountAttr );
            //      }
            //      if( reader.HasAttribute( DocIOXMLConstants.PropertiesCharCountAttr ) )
            //      {
            //        m_summaryHash[ (int)PIDSI.Charcount ] = reader.ReadInt( DocIOXMLConstants.PropertiesCharCountAttr );
            //      }
            //      if( reader.HasAttribute( DocIOXMLConstants.PropertiesThumbnailAttr ) )
            //      {
            //        Thumbnail = reader.ReadString( DocIOXMLConstants.PropertiesThumbnailAttr );
            //      }
            //      if( reader.HasAttribute( DocIOXMLConstants.PropertiesDocSecurityAttr ) )
            //      {
            //        DocSecurity = reader.ReadInt( DocIOXMLConstants.PropertiesDocSecurityAttr );
            //      }
        }
        #endregion
#endif
    }
}

