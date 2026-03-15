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
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for WListFormat.
    /// </summary>
    public class WListFormat : FormatBase
    {
        #region Constants
        /// <summary>
        /// 
        /// </summary>
        internal const int ListLevelNumberKey = 0;
        /// <summary>
        /// 
        /// </summary>
        private const int ListTypeKey = 1;
        /// <summary>
        /// 
        /// </summary>
        private const int CustomStyleNameKey = 2;
        /// <summary>
        /// 
        /// </summary>
        private const int RestartKey = 3;
        /// <summary>
        /// 
        /// </summary>
        private const int LfoStyleNameKey = 4;
        /// <summary>
        /// Changed list level number key.
        /// </summary>
        private const int NewListLevelNumKey = 5;
        /// <summary>
        /// Changed list style name.
        /// </summary>
        private const int NewStyleNameKey = 6;
        /// <summary>
        /// Changed Lfo style name.
        /// </summary>
        private const int NewLfoStyleNameKey = 7;
        /// <summary>
        /// 
        /// </summary>
        internal const int DEF_START_LISTID = 1720085641;
        #endregion

        #region Fields
        /// <summary>
        /// Currently used list style
        /// </summary>
        [ThreadStatic]
        private static string m_currentStyleName;
        /// <summary>
        /// Current level number
        /// </summary>
        [ThreadStatic]
        private static int m_currLevelNumber;
        /// <summary>
        /// 
        /// </summary>
        private bool m_isListRemoved;
        /// <summary>
        /// 
        /// </summary>
        private bool m_isEmptyList;
        #endregion

        #region Properties
        /// <summary>
        /// Gets / sets list nesting level. 
        /// </summary>
        public int ListLevelNumber
        {
            get
            {
                return (int)this[ListLevelNumberKey];
            }
            set
            {
                if (value > 8 || value < 0)
                {
                    throw new ArgumentException("List level must be less 8 and greater then 0");
                }
                else
                {
                    this[ListLevelNumberKey] = value;
                    m_currLevelNumber = value;
                }
            }
        }
        /// <summary>
        /// Get / sets type of the list.
        /// </summary>
        public ListType ListType
        {
            get
            {
                return (ListType)this[ListTypeKey];
            }
        }
        /// <summary>
        /// Gets / sets whether numbering of the list must restart from previous list.
        /// </summary>
        public bool RestartNumbering
        {
            get
            {
                return (bool)this[RestartKey];
            }
            set
            {
                this[RestartKey] = value;
            }
        }
        /// <summary>
        /// Gets the name of custom style.
        /// </summary>
        public string CustomStyleName
        {
            get
            {
                return (string)this[CustomStyleNameKey];
            }
        }
        /// <summary>
        /// Get paragraph's list style.
        /// </summary>
        public ListStyle CurrentListStyle
        {
            get
            {
                if ((string)this[CustomStyleNameKey] != string.Empty)
                {
                    return Document.ListStyles.FindByName(CustomStyleName);
                }
                return null;
            }
        }
        /// <summary>
        /// Get set paragraph's ListLevel.
        /// </summary>
        public WListLevel CurrentListLevel
        {
            get
            {
                if ((string)this[CustomStyleNameKey] == string.Empty)
                    return null;
                if (ListLevelNumber >= CurrentListStyle.Levels.Count)
                    return null;
                return CurrentListStyle.Levels[ListLevelNumber];
            }
        }
        /// <summary>
        /// Gets or sets the name of the LFO style.
        /// </summary>
        /// <value>The name of the LFO style.</value>
        internal string LFOStyleName
        {
            get
            {
                return (string)this[LfoStyleNameKey];
            }
            set
            {
                this[LfoStyleNameKey] = value;
            }
        }
        /// <summary>
        /// Gets the owner paragraph.
        /// </summary>
        /// <value>The owner paragraph.</value>
        internal WParagraph OwnerParagraph
        {
            get
            {
                return (WParagraph)OwnerBase;
            }
        }
        /// <summary>
        /// Gets/sets IsListRemoved flag.
        /// </summary>
        internal bool IsListRemoved
        {
            get
            {
                return m_isListRemoved;
            }
            set
            {
                m_isListRemoved = value;
            }
        }
        /// <summary>
        /// Gets or sets the new name of the style for list format.
        /// </summary>
        /// <value>The new name of the style.</value>
        internal string NewStyleName
        {
            get
            {
                return (string)this[NewStyleNameKey];
            }
            set
            {
                this[NewStyleNameKey] = value;
            }
        }
        /// <summary>
        /// Gets or sets the new name of the style for list format.
        /// </summary>
        /// <value>The new name of the style.</value>
        internal string NewLfoStyleName
        {
            get
            {
                return (string)this[NewLfoStyleNameKey];
            }
            set
            {
                this[NewLfoStyleNameKey] = value;
            }
        }
        /// <summary>
        /// Gets or sets the new list level number for list format.
        /// </summary>
        /// <value>The new list level number.</value>
        internal int NewListLevelNumber
        {
            get
            {
                return (int)this[NewListLevelNumKey];
            }
            set
            {
                this[NewListLevelNumKey] = value;
            }
        }
        /// <summary>
        /// Gets/sets the value which specifies whether this is empty list.
        /// Such situation occurs when style (which doesn't have)
        /// list format inherits style which have list.
        /// </summary>
        /// <value>The is empty list.</value>
        internal bool IsEmptyList
        {
            get
            {
                return m_isEmptyList;
            }
            set
            {
                m_isEmptyList = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WListFormat"/> class.
        /// </summary>
        /// <param name="owner">The owner (paragraph).</param>
        public WListFormat(IWParagraph owner)
            : base(owner.Document, (Entity)owner)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="owner"></param>
        public WListFormat(WordDocument doc, WParagraphStyle owner)
            : base(doc)
        {
            this.SetOwner(owner);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="owner"></param>
        internal WListFormat(WordDocument doc, WNumberingStyle owner)
            : base(doc)
        {
            this.SetOwner(owner);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="WListFormat"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="owner">The owner.</param>
        internal WListFormat(WordDocument doc, WTableStyle owner)
            : base(doc)
        {
            this.SetOwner(owner);
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override object GetDefValue(int key)
        {
            switch (key)
            {
                case NewListLevelNumKey:
                    return -1;
                case ListLevelNumberKey:
                    return (int)0;
                case ListTypeKey:
                    return ListType.NoList;
                case RestartKey:
                    return false;
                case NewStyleNameKey:
                case CustomStyleNameKey:
                    return string.Empty;
                case NewLfoStyleNameKey:
                case LfoStyleNameKey:
                    return null;
            }

            throw new ArgumentException("key has invalid value");
        }
//#if !SILVERLIGHT
        /// <summary>
        /// Writes object data as xml attributes.
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);

            if (HasKey(ListLevelNumberKey))
            {
                writer.WriteValue(XDLSConstants.ListFormatLevelNumAttr, ListLevelNumber);
            }
            if (HasKey(CustomStyleNameKey))
            {
                writer.WriteValue(XDLSConstants.ListFormatStyleNameAttr, CustomStyleName);
            }
            if (HasKey(ListTypeKey))
            {
                writer.WriteValue(XDLSConstants.ListFormatTypeAttr, ListType);
            }
            if (HasKey(LfoStyleNameKey))
            {
                writer.WriteValue(XDLSConstants.ListFormatLfoStyleNameAttr, LFOStyleName);
            }
        }
        /// <summary>
        /// Reads object data from xml attributes.
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            if (reader.HasAttribute(XDLSConstants.ListFormatLfoStyleNameAttr))
            {
                LFOStyleName = reader.ReadString(XDLSConstants.ListFormatLfoStyleNameAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ListFormatLevelNumAttr))
            {
                this[ListLevelNumberKey] = reader.ReadInt(XDLSConstants.ListFormatLevelNumAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ListFormatStyleNameAttr))
            {
                this[CustomStyleNameKey] = reader.ReadString(XDLSConstants.ListFormatStyleNameAttr);
            }
            if (reader.HasAttribute(XDLSConstants.ListFormatTypeAttr))
            {
                this[ListTypeKey] = (ListType)reader.ReadEnum(XDLSConstants.ListFormatTypeAttr, typeof(ListType));
            }
        }
//#endif
        #endregion

        #region Class public methods
        /// <summary>
        /// Increase level indent.
        /// </summary>
        public void IncreaseIndentLevel()
        {
            if (m_currLevelNumber == 8)
                throw new ArgumentException("List level must be less 8 and greater then 0");
            this[ListLevelNumberKey] = ++m_currLevelNumber;
        }
        /// <summary>
        /// Decrease level indent.
        /// </summary>
        public void DecreaseIndentLevel()
        {
            if (m_currLevelNumber == 0)
                throw new ArgumentException("List level must be less 8 and greater then 0");
            this[ListLevelNumberKey] = --m_currLevelNumber;
        }
        /// <summary>
        /// Continue last list.
        /// </summary>
        public void ContinueListNumbering()
        {
            ApplyStyle(m_currentStyleName);
            ListLevelNumber = m_currLevelNumber;
        }
        /// <summary>
        /// Apply liststyle 
        /// </summary>
        /// <param name="styleName">Style Name</param>
        public void ApplyStyle(string styleName)
        {
            this[CustomStyleNameKey] = styleName;
            m_currentStyleName = styleName;
            this[ListTypeKey] = CurrentListStyle.ListType;

            //if( this.OwnerBase is WParagraph && ( this.OwnerBase as WParagraph ).ParaStyle == null )
            //  this.OwnerParagraph.ApplyListParaStyle();
        }
        /// <summary>
        /// Apply default bullet style for current paragraph.
        /// </summary>
        public void ApplyDefBulletStyle()
        {
            ApplyStyle("Bulleted");
        }
        /// <summary>
        /// Apply default numbered style for current paragraph.
        /// </summary>
        public void ApplyDefNumberedStyle()
        {
            ApplyStyle("Numbered");
        }
        /// <summary>
        /// Removes the list from current paragraph.
        /// </summary>
        public void RemoveList()
        {
            this[CustomStyleNameKey] = string.Empty;
            m_currentStyleName = string.Empty;
            this[ListTypeKey] = ListType.NoList;
            m_isListRemoved = true;
        }
        #endregion
    }
}
