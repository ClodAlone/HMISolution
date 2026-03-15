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
using System.Runtime.InteropServices;

using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
using System.Collections.Generic;
using Syncfusion.CompoundFile.DocIO;
using Syncfusion.CompoundFile.DocIO.Native;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for DocumentProperties.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class SummaryDocumentProperties : XDLSSerializableBase
    {
        #region Class members
        /// <summary>
        /// Sorted list of properties
        /// </summary>
        protected Dictionary<int, DocumentProperty> m_summaryHash;
        #endregion

        #region Class Summary properties
        /// <summary>
        /// Gets / sets author name
        /// </summary>
        public string Author
        {
            get
            {
                return m_summaryHash.ContainsKey((int)PIDSI.Author) ?
                  this[PIDSI.Author].Text
                  : null;
            }
            set
            {
                SetPropertyValue(PIDSI.Author, value);
                this[ PIDSI.Author ].Text = value;
            }
        }
        /// <summary>
        /// Gets / sets application name
        /// </summary>
        public string ApplicationName
        {
            get
            {
                return m_summaryHash.ContainsKey((int)PIDSI.Appname) ?
                  this[PIDSI.Appname].Text
                  : null;
            }
            set
            {
                SetPropertyValue(PIDSI.Appname, value);
                this[PIDSI.Appname].Text = value;
                //        this[ PIDSI.Appname ].Value = value;
            }
        }
        /// <summary>
        /// Gets / sets the document title
        /// </summary>
        public string Title
        {
            get
            {
                return m_summaryHash.ContainsKey((int)PIDSI.Title) ?
                  this[PIDSI.Title].Text
                  : null;
            }
            set
            {
                SetPropertyValue(PIDSI.Title, value);
                this[ PIDSI.Title ].Text = value;
            }
        }
        /// <summary>
        /// Gets / sets the subject of the document
        /// </summary>
        public string Subject
        {
            get
            {
                return m_summaryHash.ContainsKey((int)PIDSI.Subject) ?
                  this[PIDSI.Subject].Text
                  : null;
            }
            set
            {
                SetPropertyValue(PIDSI.Subject, value);
                 this[ PIDSI.Subject ].Text = value;
            }
        }
        /// <summary>
        /// Gets / sets the document keywords
        /// </summary>
        public string Keywords
        {
            get
            {
                return m_summaryHash.ContainsKey((int)PIDSI.Keywords) ?
                  this[PIDSI.Keywords].Text
                  : null;
            }
            set
            {
                SetPropertyValue(PIDSI.Keywords, value);
                this[ PIDSI.Keywords ].Text = value;
            }
        }
        /// <summary>
        /// Gets / sets the comments that provide additional information about the document
        /// </summary>
        public string Comments
        {
            get
            {
                return m_summaryHash.ContainsKey((int)PIDSI.Comments) ?
                  this[PIDSI.Comments].Text
                  : null;
            }
            set
            {
                SetPropertyValue(PIDSI.Comments, value);
                this[ PIDSI.Comments ].Text = value;
            }
        }
        /// <summary>
        /// Gets / sets the template name of the document
        /// </summary>
        public string Template
        {
            get
            {
                return m_summaryHash.ContainsKey((int)PIDSI.Template) ?
                  this[PIDSI.Template].Text
                  : null;
            }
            set
            {
                SetPropertyValue(PIDSI.Template, value);
                this[ PIDSI.Template ].Value = value;
            }
        }
        /// <summary>
        /// Gets / sets the last author name
        /// </summary>
        public string LastAuthor
        {
            get
            {
                return m_summaryHash.ContainsKey((int)PIDSI.LastAuthor) ?
                  this[PIDSI.LastAuthor].Text
                  : null;
            }
            set
            {
                SetPropertyValue(PIDSI.LastAuthor, value);
                this[ PIDSI.LastAuthor ].Text = value;
            }
        }
        /// <summary>
        /// Gets / sets the document revision number
        /// </summary>
        public string RevisionNumber
        {
            get
            {
                return m_summaryHash.ContainsKey((int)PIDSI.Revnumber) ?
                  this[PIDSI.Revnumber].Text
                  : null;
            }
            set
            {
                SetPropertyValue(PIDSI.Revnumber, value);
                this[ PIDSI.Revnumber ].Value = value;
            }
        }
        /// <summary>
        /// Gets / sets the document total editing time
        /// </summary>
        public TimeSpan TotalEditingTime
        {
            get
            {
                if (m_summaryHash.ContainsKey((int)PIDSI.EditTime))
                    return (this[PIDSI.EditTime].TimeSpan < TimeSpan.Zero) ?
                        TimeSpan.Zero 
                        : this[PIDSI.EditTime].TimeSpan;
                else
                    return TimeSpan.MinValue;
            }
            set
            {
                SetPropertyValue(PIDSI.EditTime, value);
                this[ PIDSI.EditTime ].Value = value;
            }
        }
        /// <summary>
        /// Gets / sets the last print date
        /// </summary>
        public DateTime LastPrinted
        {
            get
            {
                return m_summaryHash.ContainsKey((int)PIDSI.LastPrinted) ?
                  this[PIDSI.LastPrinted].DateTime
                  : DateTime.MinValue;
            }
            set
            {
                SetPropertyValue(PIDSI.LastPrinted, value);
                this[ PIDSI.LastPrinted ].DateTime = value;
            }
        }
        /// <summary>
        /// Gets / sets the document creation date
        /// </summary>
        public DateTime CreateDate
        {
            get
            {
                return m_summaryHash.ContainsKey((int)PIDSI.Create_dtm) ?
                  this[PIDSI.Create_dtm].DateTime
                  : DateTime.Now;
            }
            set
            {
                if (!value.Equals(new DateTime()))
                {

                    if (value.CompareTo(new DateTime(1900, 12, 31)) > 0)
                    {
                        SetPropertyValue(PIDSI.Create_dtm, value);
                        this[PIDSI.Create_dtm].DateTime = value;
                    }
                    else if (!this.Document.IsOpening)
                    {
                        throw new Exception("Date time value must be after 12/31/1900(MM/DD/YYYY).");
                    }
                }
                else
                {
                    if (m_summaryHash.ContainsKey((int)PIDSI.Create_dtm))
                        m_summaryHash.Remove((int)PIDSI.Create_dtm);
                }
            }
        }
        /// <summary>
        /// Gets / sets the last save date
        /// </summary>
        public DateTime LastSaveDate
        {
            get
            {
                return m_summaryHash.ContainsKey((int)PIDSI.LastSave_dtm) ?
                  this[PIDSI.LastSave_dtm].DateTime
                  : CreateDate;
            }
            set
            {
                if (!value.Equals(new DateTime()))
                {

                    if (value.CompareTo(new DateTime(1900, 12, 31)) > 0)
                    {
                        SetPropertyValue(PIDSI.LastSave_dtm, value);
                        this[PIDSI.LastSave_dtm].DateTime = value;
                    }
                    else if(!this.Document.IsOpening)
                    {
                        throw new Exception("Date time value must be after 12/31/1900(MM/DD/YYYY).");
                    }
                }
                else
                {
                    if(m_summaryHash.ContainsKey((int) PIDSI.LastSave_dtm))
                        m_summaryHash.Remove((int)PIDSI.LastSave_dtm);
                }
            }
        }
        /// <summary>
        /// Gets document pages count
        /// </summary>
        public int PageCount
        {
            get
            {
                return m_summaryHash.ContainsKey((int)PIDSI.Pagecount) ?
                  this[PIDSI.Pagecount].ToInt()
                  : int.MinValue;
            }
            internal set
            {
                SetPropertyValue(PIDSI.Pagecount, value);
                this[PIDSI.Pagecount].Int32 = value;
            }
        }
        /// <summary>
        /// Gets document words count
        /// </summary>
        public int WordCount
        {
            get
            {
                return m_summaryHash.ContainsKey((int)PIDSI.Wordcount) ?
                  this[PIDSI.Wordcount].ToInt()
                  : int.MinValue;
            }
            internal set
            {
                SetPropertyValue(PIDSI.Wordcount, value);
                this[PIDSI.Wordcount].Int32 = value;
                //m_summaryHash[(int)PIDSI.Wordcount] = value;
            }
        }
        /// <summary>
        /// Gets document characters count
        /// </summary>
        public int CharCount
        {
            get
            {
                return m_summaryHash.ContainsKey((int)PIDSI.Charcount) ?
                  this[PIDSI.Charcount].ToInt()
                  : int.MinValue;
            }
            internal set
            {
                SetPropertyValue(PIDSI.Charcount, value);
                this[PIDSI.Charcount].Int32 = value;
                //m_summaryHash[ (int)PIDSI.Charcount ] = value;
            }
        }
        /// <summary>
        /// Gets / sets thumbnail picture for document preview
        /// </summary>
        public string Thumbnail
        {
            get
            {
                return m_summaryHash.ContainsKey((int)PIDSI.Thumbnail) ?
                  this[PIDSI.Thumbnail].Text
                  : null;
            }
            set
            {
                SetPropertyValue(PIDSI.Thumbnail, value);
                this[ PIDSI.Thumbnail ].Text = value;
            }
        }
        /// <summary>
        /// Gets / sets document security level
        /// </summary>
        public int DocSecurity
        {
            get
            {
                return m_summaryHash.ContainsKey((int)PIDSI.Doc_security) ?
                  this[PIDSI.Doc_security].ToInt()
                  : int.MinValue;
            }
            set
            {
                SetPropertyValue(PIDSI.Doc_security, value);
                this[ PIDSI.Doc_security ].Int32 = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal DocumentProperty this[PIDSI pidsi]
        {
            get
            {
                return m_summaryHash[(int)pidsi];
            }
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets summary count of document properties
        /// </summary>
        public int Count
        {
            get
            {
                return m_summaryHash.Count;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal Dictionary<int, DocumentProperty> SummaryHash
        {
            get
            {
                return m_summaryHash;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// 
        /// </summary>
        internal SummaryDocumentProperties()
            : this(0)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        internal SummaryDocumentProperties(int count)
            : base(null, null)
        {
            m_summaryHash = new Dictionary<int, DocumentProperty>(count);
        }
        /// <summary>
        /// 
        /// </summary>
        internal SummaryDocumentProperties(WordDocument doc)
            : base(doc, null)
        {
            m_summaryHash = new Dictionary<int, DocumentProperty>();
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
            return m_summaryHash.ContainsKey(key);
        }

        /// <summary>
        /// Adds the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void Add(int key, DocumentProperty props)
        {
            m_summaryHash.Add(key, props);
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pidsi"></param>
        /// <param name="value"></param>
        internal void SetPropertyValue(PIDSI pidsi, object value )
        {
            if (m_summaryHash.ContainsKey((int)pidsi))
            {
                this[pidsi].Value = value;
            }
            else
            {
                DocumentProperty property = new DocumentProperty((BuiltInProperty)pidsi, value);
                m_summaryHash[(int)pidsi] = property;
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

            // Summary properties
            if (HasKey((int)PIDSI.Author))
            {
                writer.WriteValue(XDLSConstants.PropertiesAuthorAttr, Author);
            }
            if (HasKey((int)PIDSI.Appname))
            {
                writer.WriteValue(XDLSConstants.PropertiesApplicationNameAttr, ApplicationName);
            }
            if (HasKey((int)PIDSI.Title))
            {
                writer.WriteValue(XDLSConstants.PropertiesTitleAttr, Title);
            }
            if (HasKey((int)PIDSI.Subject))
            {
                writer.WriteValue(XDLSConstants.PropertiesSubjectAttr, Subject);
            }
            if (HasKey((int)PIDSI.Keywords))
            {
                writer.WriteValue(XDLSConstants.PropertiesKeywordsAttr, Keywords);
            }
            if (HasKey((int)PIDSI.Comments))
            {
                writer.WriteValue(XDLSConstants.PropertiesCommentsAttr, Comments);
            }
            if (HasKey((int)PIDSI.Template))
            {
                writer.WriteValue(XDLSConstants.PropertiesTemplateAttr, Template);
            }
            if (HasKey((int)PIDSI.LastAuthor))
            {
                writer.WriteValue(XDLSConstants.PropertiesLastAuthorAttr, LastAuthor);
            }
            if (HasKey((int)PIDSI.Revnumber))
            {
                writer.WriteValue(XDLSConstants.PropertiesRevisionNumberAttr, RevisionNumber);
            }
            if (HasKey((int)PIDSI.EditTime))
            {
                writer.WriteValue(XDLSConstants.PropertiesEditTimeAttr, TotalEditingTime.TotalMinutes.ToString() );
            }
            if (HasKey((int)PIDSI.LastPrinted))
            {
                writer.WriteValue(XDLSConstants.PropertiesLastPrintedAttr, LastPrinted);
            }
            if (HasKey((int)PIDSI.Create_dtm))
            {
                writer.WriteValue(XDLSConstants.PropertiesCreateDateAttr, CreateDate);
            }
            if (HasKey((int)PIDSI.LastSave_dtm))
            {
                writer.WriteValue(XDLSConstants.PropertiesLastSaveDateAttr, LastSaveDate);
            }
            if (HasKey((int)PIDSI.Pagecount))
            {
                writer.WriteValue(XDLSConstants.PropertiesPageCountAttr, PageCount);
            }
            if (HasKey((int)PIDSI.Wordcount))
            {
                writer.WriteValue(XDLSConstants.PropertiesWordCountAttr, WordCount);
            }
            if (HasKey((int)PIDSI.Charcount))
            {
                writer.WriteValue(XDLSConstants.PropertiesCharCountAttr, CharCount);
            }
            if (HasKey((int)PIDSI.Thumbnail))
            {
                writer.WriteValue(XDLSConstants.PropertiesThumbnailAttr, Thumbnail);
            }
            if (HasKey((int)PIDSI.Doc_security))
            {
                writer.WriteValue(XDLSConstants.PropertiesDocSecurityAttr, DocSecurity);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            // Summary properties
            if (reader.HasAttribute(XDLSConstants.PropertiesAuthorAttr))
            {
                Author = reader.ReadString(XDLSConstants.PropertiesAuthorAttr);
            }
            if (reader.HasAttribute(XDLSConstants.PropertiesApplicationNameAttr))
            {
                ApplicationName = reader.ReadString(XDLSConstants.PropertiesApplicationNameAttr);
            }
            if (reader.HasAttribute(XDLSConstants.PropertiesTitleAttr))
            {
                Title = reader.ReadString(XDLSConstants.PropertiesTitleAttr);
            }
            if (reader.HasAttribute(XDLSConstants.PropertiesSubjectAttr))
            {
                Subject = reader.ReadString(XDLSConstants.PropertiesSubjectAttr);
            }
            if (reader.HasAttribute(XDLSConstants.PropertiesKeywordsAttr))
            {
                Keywords = reader.ReadString(XDLSConstants.PropertiesKeywordsAttr);
            }
            if (reader.HasAttribute(XDLSConstants.PropertiesCommentsAttr))
            {
                Comments = reader.ReadString(XDLSConstants.PropertiesCommentsAttr);
            }
            if (reader.HasAttribute(XDLSConstants.PropertiesTemplateAttr))
            {
                Template = reader.ReadString(XDLSConstants.PropertiesTemplateAttr);
            }
            if (reader.HasAttribute(XDLSConstants.PropertiesLastAuthorAttr))
            {
                LastAuthor = reader.ReadString(XDLSConstants.PropertiesLastAuthorAttr);
            }
            if (reader.HasAttribute(XDLSConstants.PropertiesRevisionNumberAttr))
            {
                RevisionNumber = reader.ReadString(XDLSConstants.PropertiesRevisionNumberAttr);
            }
            if (reader.HasAttribute(XDLSConstants.PropertiesEditTimeAttr))
            {
                TotalEditingTime = TimeSpan.FromMinutes(reader.ReadInt(XDLSConstants.PropertiesEditTimeAttr));
            }
            if (reader.HasAttribute(XDLSConstants.PropertiesLastPrintedAttr))
            {
                LastPrinted = reader.ReadDateTime(XDLSConstants.PropertiesLastPrintedAttr);
            }
            if (reader.HasAttribute(XDLSConstants.PropertiesCreateDateAttr))
            {
                CreateDate = reader.ReadDateTime(XDLSConstants.PropertiesCreateDateAttr);
            }
            if (reader.HasAttribute(XDLSConstants.PropertiesLastSaveDateAttr))
            {
                LastSaveDate = reader.ReadDateTime(XDLSConstants.PropertiesLastSaveDateAttr);
            }
            if (reader.HasAttribute(XDLSConstants.PropertiesPageCountAttr))
            {
                SetPropertyValue(PIDSI.Pagecount, reader.ReadInt(XDLSConstants.PropertiesPageCountAttr));
            }
            if (reader.HasAttribute(XDLSConstants.PropertiesWordCountAttr))
            {
                SetPropertyValue(PIDSI.Wordcount, reader.ReadInt(XDLSConstants.PropertiesWordCountAttr));
            }
            if (reader.HasAttribute(XDLSConstants.PropertiesCharCountAttr))
            {
                SetPropertyValue(PIDSI.Charcount, reader.ReadInt(XDLSConstants.PropertiesCharCountAttr));
            }
            if (reader.HasAttribute(XDLSConstants.PropertiesThumbnailAttr))
            {
                Thumbnail = reader.ReadString(XDLSConstants.PropertiesThumbnailAttr);
            }
            if (reader.HasAttribute(XDLSConstants.PropertiesDocSecurityAttr))
            {
                DocSecurity = reader.ReadInt(XDLSConstants.PropertiesDocSecurityAttr);
            }
        }
        #endregion
#endif
    }
}
