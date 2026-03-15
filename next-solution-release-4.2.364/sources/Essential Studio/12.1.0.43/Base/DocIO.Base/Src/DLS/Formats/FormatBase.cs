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
using System.Collections.Specialized;
using System.Diagnostics;
using System.Xml;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using System.Collections.Generic;
using System.IO;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents Base Formatting.
    /// </summary>
    public abstract class FormatBase
      : XDLSSerializableBase
    {
        #region Class constants
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_OFFSET_STEP = 8;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_OFFSET_LEVELS_MAX = 4;
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_OFFSET_MAX = DEF_OFFSET_STEP * DEF_OFFSET_LEVELS_MAX; // = 32
        /// <summary>
        /// 
        /// </summary>
        private const int DEF_KEY_MAX = 128;
        #endregion

        #region Class members
        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<int, Object> m_propertiesHash;
        /// <summary>
        /// 
        /// </summary>
        private FormatBase m_baseFormat = null;
        /// <summary>
        /// 
        /// </summary>
        private FormatBase m_parentFormat;
        /// <summary>
        /// 
        /// </summary>
        private int m_parentKey = 0;
        /// <summary>
        /// 
        /// </summary>
        protected int m_keysOffset = 0;
        /// <summary>
        /// 
        /// </summary>
        private bool m_bDefault = true;
        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<int, bool> m_propsUpdateFlags;        
        /// <summary>
        /// 
        /// </summary>
        private List<Stream> m_xmlProps;
        /// <summary>
        /// 
        /// </summary>
        internal SinglePropertyModifierArray m_sprms;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets / sets whether format is default.
        /// </summary>
        internal bool IsDefault
        {
            get
            {
                return m_bDefault;
            }
            set
            {
                if (value == false)
                {
                    MarkNoDefault();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal Dictionary<int, Object> PropertiesHash
        {
            get
            {
                if (m_propertiesHash == null)
                {
                    m_propertiesHash = new Dictionary<int, Object>();
                }
                return m_propertiesHash;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal FormatBase BaseFormat
        {
            get
            {
                return m_baseFormat;
            }
            set
            {
                m_baseFormat = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal int KeysOffset
        {
            get
            {
                return m_keysOffset;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected object this[int key]
        {
            get
            {
                UpdateFormat(key);
                int fullKey = GetFullKey(key);

                // 1. Gets SELF VALUE
                object value = null;

                // 2.1. If VALUE is NULL get COMPOSITE DEFAULT VALUE
                if (!PropertiesHash.ContainsKey(fullKey))
                {
                    // Gets and updates composite value in hash.
                    value = GetDefComposite(key);
                }
                else
                {
                    value = PropertiesHash[fullKey];
                }

                // 2.2 If SELF VALUE is NULL get BASE VALUE
                if (value == null && BaseFormat != null && BaseFormat.m_propertiesHash != null)
                {
                    value = GetBaseFormatValue(key);
                }
                // 2.3 If VALUE not in hash, get CharStyle VALUE
                if (CheckCharacterStyle(key))
                {
                    value = (this as WCharacterFormat).CharStyle.CharacterFormat[key];
                }
                // 3. If VALUE is NULL get DEFAULT VALUE
                if (value == null)
                {
                    value = GetDefValue(key);
                }

                return value;
            }
            set
            {
                int fullkey = GetFullKey(key);

                //if( !this[ key ].Equals( value ) )
                {
                    PropertiesHash[fullkey] = value;
                    IsDefault = false;
                }

                OnChange(this, key);
            }
        }
        /// <summary>
        /// Gets parent format.
        /// </summary>
        internal FormatBase ParentFormat
        {
            get
            {
                return m_parentFormat;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal List<Stream> XmlProps
        {
            get
            {
                if (m_xmlProps == null)
                {
                    m_xmlProps = new List<Stream>();
                }
                return m_xmlProps;
            }
        }
        /// <summary>
        /// Gets the unparsed .docx properties.
        /// </summary>
        /// <value>The properties.</value>

        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatBase"/> class.
        /// </summary>
        public FormatBase()
            : this(null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatBase"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        public FormatBase(IWordDocument doc)
            : this(doc, null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatBase"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="owner">The owner.</param>
        public FormatBase(IWordDocument doc, Entity owner)
            : base(doc as WordDocument, owner)
        {
            m_propertiesHash = new Dictionary<int, Object>();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatBase"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="parentKey">The parent key.</param>
        public FormatBase(FormatBase parent, int parentKey)
            : this(null)
        {
            if (parent.KeysOffset + DEF_OFFSET_STEP > DEF_OFFSET_MAX)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (parentKey > DEF_KEY_MAX)
            {
                throw new ArgumentOutOfRangeException("parentKey");
            }

            m_propertiesHash = parent.PropertiesHash;
            m_parentKey = parentKey;
            m_parentFormat = parent;
            m_keysOffset = parent.KeysOffset + DEF_OFFSET_STEP;
        }
        /// <summary>
        /// Initializing constructor.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="parentKey">The parent key.</param>
        /// <param name="parentOffset">The parent offset.</param>
        public FormatBase(FormatBase parent, int parentKey, int parentOffset)
            : this(parent, parentKey)
        { }
        #endregion

        #region Class public methods
        /// <summary>
        /// Imports the container.
        /// </summary>
        /// <param name="format">The format.</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal protected void ImportContainer(FormatBase format)
        {
            if (!(format is WParagraphFormat) &&
              !(format is WCharacterFormat) &&
              !(format is RowFormat))
            {
                CopyProperties(format);
            }

            EnsureComposites();
            IsDefault = false;

            if (m_propsUpdateFlags != null)
            {
                m_propsUpdateFlags.Clear();
            }

            ImportMembers(format);
#if !SILVERLIGHT && !WP
            ImportXmlProps(format);
#endif
        }
        /// <summary>
        /// Imports the members.
        /// </summary>
        /// <param name="format">The format.</param>
        protected virtual void ImportMembers(FormatBase format)
        {
            //Not implemented here
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseFormat"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal virtual void ApplyBase(FormatBase baseFormat)
        {
            m_baseFormat = baseFormat;
        }
        /// <summary>
        /// Checks if Key exists.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	if the specified key has key, set to <c>true</c>.
        /// </returns>
        public bool HasKey(int key)
        {
            if (PropertiesHash == null)
                return false;

            return PropertiesHash.ContainsKey(GetFullKey(key));
        }
        /// <summary>
        /// Checks if Key exists and their corresponding boolean value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	if the specified key has key, set to <c>true</c>.
        /// </returns>
        public bool HasBoolKey(int key)
        {
            if (PropertiesHash == null)
                return false;

            if (PropertiesHash.ContainsKey(key) && (bool) PropertiesHash[key])
                return true;

            return false;
        }
        /// <summary>
        /// Clears the formatting.
        /// </summary>
        public void ClearFormatting()
        {
            m_propertiesHash.Clear();
        }
        /// <summary>
        /// Sets the flag that defines that current property was updated.
        /// </summary>
        /// <param name="propKey">The property key.</param>
        protected void SetPropUpdateFlag(int propKey)
        {
            CheckUpdateFlagsColl();

            if (!m_propsUpdateFlags.ContainsKey(propKey))
            {
                m_propsUpdateFlags.Add(propKey, true);
            }
        }
        /// <summary>
        /// Checks if specified property was set before.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        /// <returns>
        /// 	if property is set, set to <c>true</c>.
        /// </returns>
        protected bool IsPropertyUpdated(int propertyKey)
        {
            CheckUpdateFlagsColl();
            if (m_propsUpdateFlags.ContainsKey(propertyKey))
            {
                return true;
            }
            return false;
            //return (m_propsUpdateFlags[propertyKey] == null) ? false : true;
        }
        /// <summary>
        /// Checks the update flags collection.
        /// </summary>
        protected void CheckUpdateFlagsColl()
        {
            if (m_propsUpdateFlags == null)
            {
                m_propsUpdateFlags = new Dictionary<int, bool>();
            }
        }
        /// <summary>
        /// Sets complex boolean value.
        /// </summary>
        /// <param name="propKey">The prop key.</param>
        /// <param name="val">The value.</param>
        internal void SetComplexBoolValue(short propKey, byte val)
        {
            if (val < 0 || (val > 1 && val < 128) || val > 129)
                return;

            int option = GetSprmOption(propKey);
            SinglePropertyModifierRecord sprm = m_sprms[option];
            if (sprm != null)
            {
                sprm.ByteValue = val;
            }
        }
        #endregion

        #region Class virtual methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected abstract object GetDefValue(int key);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected virtual FormatBase GetDefComposite(int key)
        {
            return null;
        }
        /// <summary>
        /// Action on format change.
        /// </summary>
        /// <param name="format">The format.</param>
        protected virtual void OnChange(FormatBase format, int propKey)
        {
            if (m_parentFormat != null)
            {
                this.ParentFormat.OnChange(format, propKey);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyKey"></param>
        /// <returns></returns>
        internal virtual bool HasValue(int propertyKey)
        {
            return false;
        }
        /// <summary>
        /// Gets the Single Property Modifier Record option value.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        /// <returns></returns>
        protected virtual int GetSprmOption(int propertyKey)
        {
            return int.MaxValue;
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal virtual void Close()
        {
            if (m_propertiesHash != null)
            {
                m_propertiesHash.Clear();
                m_propertiesHash = null;
            }

            if (m_propsUpdateFlags != null)
            {
                m_propsUpdateFlags.Clear();
                m_propsUpdateFlags = null;
            }

            if (m_xmlProps != null)
            {
                m_xmlProps.Clear();
                m_xmlProps = null;
            }

            if (m_sprms != null)
            {
                m_sprms = null;
                //SinglePropertyModifierRecord sprm = null;
                //for (int i = 0, cnt = m_sprms.Count; i < cnt; i++)
                //{
                //    sprm = m_sprms.Modifiers[i] as SinglePropertyModifierRecord;
                //    sprm.ByteArray = null;
                //    sprm = null;
                //}
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// 
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected internal virtual void EnsureComposites()
        { }
        /// <summary>
        /// 
        /// </summary>
        protected void EnsureComposites(params int[] keys)
        {
            foreach (int key in keys)
            {
                FormatBase format = GetDefComposite(key);
                format.EnsureComposites();
                format.IsDefault = false;
                //        DBG_CheckChildComposites( format.PropertiesHash );
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected int GetBaseKey(int key)
        {
            return key - (m_parentKey << m_keysOffset);
        }
        /// <summary>
        /// 
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected int GetFullKey(int key)
        {
            if (key > DEF_KEY_MAX)
            {
                throw new ArgumentOutOfRangeException("key");
            }

            return key + (m_parentKey << m_keysOffset);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected FormatBase GetDefComposite(int key, FormatBase value)
        {
            int fullKey = GetFullKey(key);

            // Updates composite value in hash
            PropertiesHash[fullKey] = value;

            if (BaseFormat != null && BaseFormat.PropertiesHash != null)
            {
                FormatBase format;
                if (BaseFormat.PropertiesHash.ContainsKey(fullKey))
                    format = BaseFormat.PropertiesHash[fullKey] as FormatBase;
                else
                    format = BaseFormat[fullKey] as FormatBase;
                // Makes link to base composite property
                value.ApplyBase(format);
            }

            return value;
        }
        /// <summary>
        /// 
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        private void MarkNoDefault()
        {
            m_bDefault = false;

            if (m_parentFormat != null)
            {
                m_parentFormat.IsDefault = false;
            }
        }
        /// <summary>
        /// Determines whether format has xml properties.
        /// </summary>
        /// <returns>
        /// 	if has xml properties, set to <c>true</c>.
        /// </returns>
        internal bool HasXmlProps()
        {
            return (m_xmlProps == null || m_xmlProps.Count == 0) ? false : true;
        }
        /// <summary>
        /// Removes the format changes.
        /// </summary>
        internal virtual void RemoveChanges()
        {
            if (m_sprms == null)
                return;

            int changeOption = GetChangeOption();
            if (changeOption == 0)
                return;

            SinglePropertyModifierRecord sprm = m_sprms[changeOption];
            if (sprm != null)
            {
                int index = m_sprms.Modifiers.IndexOf(sprm);
                for (int i = index; i < m_sprms.Modifiers.Count; i++)
                {
                    m_sprms.Modifiers.RemoveAt(i);
                    i -= 1;
                }

                if (m_propsUpdateFlags != null)
                    m_propsUpdateFlags.Clear();

                if (m_propertiesHash != null)
                    m_propertiesHash.Clear();
            }
        }
        /// <summary>
        /// Accepts the changes.
        /// </summary>
        internal virtual void AcceptChanges()
        {
            if (m_sprms == null || m_sprms.Length == 0)
                return;

            int changeOption = GetChangeOption();
            if (changeOption == 0)
                return;

            SinglePropertyModifierRecord sprm = m_sprms.TryGetSprm(changeOption);
            if (sprm != null)
            {
                int styleChange = GetStyleChangeOption();
                if (m_sprms.Contain(styleChange))
                {
                    //Remove the sprms previous to revision SPRM - sprmCWall/sprmPWall/sprmTWall
                    int count = m_sprms.Modifiers.IndexOf(sprm);
                    for (int i = 0; i <= count; i++)
                    {
                        m_sprms.Modifiers.RemoveAt(0);
                    }
                    if (this is WParagraphFormat)
                    {
                        if (m_sprms.Contain(WordSprmOptions.sprmPIlfo))
                        {
                            m_sprms.RemoveValue(styleChange);
                        }
                    }
                    else
                    {
                        m_sprms.RemoveValue(styleChange);
                    }
                }
                else
                {
                    // Remove all duplicate sprms previous to revision SPRM - sprmCWall/sprmPWall/sprmTWall.
                    int startIndex = m_sprms.Modifiers.IndexOf(sprm) + 1;
                    List<SinglePropertyModifierRecord> sprms = null;
                    if (startIndex < m_sprms.Count)
                    {
                        sprms = new List<SinglePropertyModifierRecord>();
                        for (int i = startIndex, cnt = m_sprms.Count; i < cnt; i++)
                            sprms.Add(m_sprms.GetSprmByIndex(i));

                        foreach (SinglePropertyModifierRecord modifier in sprms)
                        {
                            m_sprms.RemoveValue(modifier.Options);
                            m_sprms.Add(modifier);
                        }
                    }
                    m_sprms.RemoveValue(changeOption);
                }
                //Clear the keys added based on the sprms before revision mark.
                if (m_propsUpdateFlags != null)
                    m_propsUpdateFlags.Clear();
                PropertiesHash.Clear();
            }
        }
        /// <summary>
        /// Gets the style change option.
        /// </summary>
        /// <returns></returns>
        private int GetStyleChangeOption()
        {
            if (this is WCharacterFormat)
                return WordSprmOptions.sprmCIstd;
            else if (this is WParagraphFormat)
                return WordSprmOptions.sprmPIstd;
            else
                return 0;
        }
        /// <summary>
        /// Gets the change option.
        /// </summary>
        /// <returns></returns>
        private int GetChangeOption()
        {
            if (this is WCharacterFormat)
                return WordSprmOptions.sprmCWall;
            else if (this is WParagraphFormat)
                return WordSprmOptions.sprmPWall;
            else if (this is RowFormat)
                return WordSprmOptions.sprmTWall;
            else
                return 0;
        }
        /// <summary>
        /// Removes the positioning.
        /// </summary>
        internal virtual void RemovePositioning()
        {
        }
        /// <summary>
        /// Imports the XML properties.
        /// </summary>
        /// <param name="format">The format.</param>
        private void ImportXmlProps(FormatBase format)
        {
            if (format.m_xmlProps != null && format.m_xmlProps.Count > 0)
            {
                foreach (Stream stream in format.XmlProps)
                {
                    XmlProps.Add(CloneStream(stream));
                }
            }
        }
        private Stream CloneStream(Stream input)
        {
            MemoryStream output = new MemoryStream();
            byte[] buffer = new byte[input.Length];
            input.Seek(0, SeekOrigin.Begin);
            int read = input.Read(buffer, 0, buffer.Length);
            if (read >0)
                output.Write(buffer, 0, read);
            return output;
        }
        /// <summary>
        /// Copies property hash.
        /// </summary>
        /// <param name="format">The format.</param>
        internal void CopyProperties(FormatBase format)
        {
            Dictionary<int, Object> srcHD = format.PropertiesHash;
            m_propertiesHash = new Dictionary<int, Object>(srcHD.Count);

            IDictionaryEnumerator dicEn = srcHD.GetEnumerator();

            while (dicEn.MoveNext())
            {
                m_propertiesHash.Add((int)dicEn.Key, dicEn.Value);
            }
        }
        /// <summary>
        /// Updates the properties.
        /// </summary>
        /// <param name="format">The format.</param>
        internal void UpdateProperties(FormatBase format)
        {
            m_propertiesHash = format.PropertiesHash;
        }
        /// <summary>
        /// Copies the format.
        /// </summary>
        /// <param name="format">The format.</param>
        internal void CopyFormat(FormatBase format)
        {
            foreach (KeyValuePair<int, Object> keyValue in format.PropertiesHash)
            {
                if (!(keyValue.Value is Borders
                    || keyValue.Value is Border
                    || keyValue.Value is Paddings
                    || keyValue.Value is RowFormat.TablePositioning))
                {
                    if (PropertiesHash.ContainsKey(keyValue.Key))
                        PropertiesHash[keyValue.Key] = keyValue.Value;
                    else
                        PropertiesHash.Add(keyValue.Key, keyValue.Value);
                }
            }
        }
        /// <summary>
        /// Checks the char style.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        private bool CheckCharacterStyle(int key)
        {
            int fullKey = GetFullKey(key);
            if ((this is WCharacterFormat) && !PropertiesHash.ContainsKey(fullKey) 
                && (this as WCharacterFormat).CharStyle != null
                && (this as WCharacterFormat).CharStyle.CharacterFormat[key] != null
                && (this as WCharacterFormat).CharStyle.CharacterFormat.HasValue(key))
                return true;
            return false;
        }
        /// <summary>
        /// Gets the base format value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        private object GetBaseFormatValue(int key)
        {
            object value = BaseFormat[key];
            if (((this is WCharacterFormat)
                && (this as WCharacterFormat).TableStyleCharacterFormat != null
                && !(value is bool))
                || ((this is WParagraphFormat)
                && (this as WParagraphFormat).TableStyleParagraphFormat != null))
            {
                FormatBase baseFormat = BaseFormat;
                int fullKey = GetFullKey(key);
                while (!baseFormat.PropertiesHash.ContainsKey(fullKey))
                {
                    if (baseFormat.BaseFormat != null)
                        baseFormat = baseFormat.BaseFormat;
                    else
                    {
                        if (this is WCharacterFormat)
                            return (this as WCharacterFormat).TableStyleCharacterFormat[key];
                        else
                            return (this as WParagraphFormat).TableStyleParagraphFormat[key];
                    }
                }
                return value;
            }
            else if (this is WParagraphFormat
                && (key == WParagraphFormat.LeftIndentKey
                || key == WParagraphFormat.FirstLineIndentKey
                || key == WParagraphFormat.LeftIndentBiKey
                || key == WParagraphFormat.FirstLineIndentBiKey))
            {
                WListFormat listFormat = null;
                if ((this as WParagraphFormat).OwnerBase is WParagraph)
                    listFormat = ((this as WParagraphFormat).OwnerBase as WParagraph).ListFormat;
                else if ((this as WParagraphFormat).OwnerBase is WParagraphStyle)
                    listFormat = ((this as WParagraphFormat).OwnerBase as WParagraphStyle).ListFormat;
                if (listFormat != null
                    && listFormat.IsEmptyList)
                    return null;
                else
                    return value;
            }
            else
                return value;
        }
        /// <summary>
        /// Updates the format.
        /// </summary>
        /// <param name="key">The key.</param>
        private void UpdateFormat(int key)
        {
            if (this is WCharacterFormat && key != WCharacterFormat.BorderKey)
                (this as WCharacterFormat).UpdateCharacterFormat(key);
            else if (this is WParagraphFormat && key != WParagraphFormat.BordersKey)
                (this as WParagraphFormat).UpdateParaFormat(key);
        }
        #endregion

        #region DEBUG
        //    private void DBG_TraceFormat( FormatBase format )
        //    {
        //      Trace.WriteLine( "----------------------------" );
        //      Trace.WriteLine( format.GetType().Name );
        //      Trace.WriteLine( "----------------------------" );
        //      Trace.WriteLine( string.Format( "IsDefault: {0}", format.IsDefault ) );
        //      
        //      IDictionaryEnumerator dicEn = format.PropertiesHash.GetEnumerator();
        //      while(dicEn.MoveNext())
        //      {
        //        FormatBase fb = dicEn.Value as FormatBase;
        //        if( fb != null )
        //        {
        //          Trace.WriteLine( string.Format( "{0} | IsDefault: {1}", 
        //                                          fb.GetType().Name, 
        //                                          fb.IsDefault) 
        //            );
        //        }
        //      }
        //    }
        //    private void DBG_CheckChildComposites( Hashtable props )
        //    {
        //      IDictionaryEnumerator dicEn = props.GetEnumerator();
        //      while(dicEn.MoveNext())
        //      {
        //        FormatBase fb = dicEn.Value as FormatBase;
        //        if( fb != null )
        //        {
        //          if( fb.PropertiesHash != props )
        //          {
        //            DBG_TraceFormat( fb );
        //            throw new InvalidOperationException( "Child format base must have the same Hash with parent!!!" );
        //          }
        //        }
        //      }
        //    }
        #endregion DEBUG
    }
}
