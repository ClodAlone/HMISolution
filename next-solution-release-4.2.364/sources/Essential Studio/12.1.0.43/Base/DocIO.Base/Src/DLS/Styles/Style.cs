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
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using Syncfusion.DocIO.DLS.XML;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using System.Collections.Generic;
#if WINRT
using Windows.ApplicationModel.Resources.Core;
#elif !WP
using System.Drawing;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Base class for paragraph and character styles. 
    /// </summary>
    public abstract class Style
      : XDLSSerializableBase
      , IStyle
    {
        #region Constants
        protected const int DEF_USER_STYLE_ID = 4094;
        #endregion

        #region Fields
        private int m_styleId = DEF_USER_STYLE_ID;
        private string m_strName;
        protected IStyle m_baseStyle = null;
        protected WCharacterFormat m_chFormat;
        protected string m_nextStyle;
        protected string m_linkStyle;
        protected bool m_isPrimaryStyle;
        protected bool m_isSemiHidden;
        protected bool m_unhideWhenUsed;
        protected bool m_isCustom;
        protected WordStyleType m_typeCode;
        protected byte[] m_tapx = null;
        private Dictionary<string, string> m_builtinStyles;
        private Dictionary<string, int> m_builtinStyleIds;
        #endregion

        #region Properties
        /// <summary>
        /// Represents the table style data (.doc format)
        /// </summary>
        /// <remarks>This property is specific for .doc format</remarks>
        internal byte[] TableStyleData
        {
            get
            {
                return m_tapx;
            }
            set
            {
                m_tapx = value;
            }
        }
        /// <summary>
        /// Gets or sets the type code of the style.
        /// </summary>
        /// <remarks>This property is specific for .doc format</remarks>
        internal WordStyleType TypeCode
        {
            get
            {
                return m_typeCode;
            }
            set
            {
                if (StyleType == StyleType.ParagraphStyle && value != WordStyleType.ParagraphStyle)
                    RemoveBaseStyle();
                m_typeCode = value;
            }
        }
        /// <summary>
        /// Gets the character format.
        /// </summary>
        /// <value>The character format.</value>
        public WCharacterFormat CharacterFormat
        {
            get
            {
                return m_chFormat;
            }
        }
        /// <summary>
        /// Gets / sets style name.
        /// </summary>
        /// <value></value>
        public string Name
        {
            get
            {
                return m_strName;
            }
            set
            {
                if (value == null || value.Length == 0)
                    throw new ArgumentNullException("Name");
                if (StyleType == StyleType.ParagraphStyle && value == "Normal" && !Document.IsNormalStyleDefined)
                {
                    (Document.Styles as StyleCollection).InnerList.Remove(m_baseStyle);
                    RemoveBaseStyle();
                    Document.IsNormalStyleDefined = true;
                }
                if (!Document.IsOpening && !Document.IsMailMerge && !Document.IsCloning)
                {
                    if (Document != null && Document.Styles.FindByName(value, StyleType) != null)
                        throw new ArgumentException("Name of style already exists");
                }
                //Updates the style id based on the Word builtin style id.
                string name = value.Replace(" ", string.Empty).ToLower();
                if (BuiltinStyleIds.ContainsKey(name))
                    StyleId = BuiltinStyleIds[name];
                else
                    StyleId = DEF_USER_STYLE_ID;
                m_strName = value;
            }
        }
        /// <summary>
        /// Gets the base style.
        /// </summary>
        /// <value>The base style.</value>
        internal IStyle BaseStyle
        {
            get
            {
                return m_baseStyle;
            }
        }
        /// <summary>
        /// Gets or sets the style id.
        /// </summary>
        /// <value>The style id.</value>
        internal int StyleId
        {
            get
            {
                return m_styleId;
            }
            set
            {
                m_styleId = value;
            }
        }
        /// <summary>
        /// Gets the type of the style.
        /// </summary>
        /// <value>The type of the style.</value>
        public abstract StyleType StyleType
        {
            get;
        }
        /// <summary>
        /// Identifies the style as built-in Word style.
        /// </summary>
        /// <value>The built in style identifier.</value>
        public BuiltinStyle BuiltInStyleIdentifier
        {
            get
            {
                return NameToBuiltIn(this.Name);
            }
        }
        /// <summary>
        /// Gets or sets the next style.
        /// </summary>
        /// <value>The next style.</value>
        internal string NextStyle
        {
            get
            {
                if (m_nextStyle != null)
                {
                    //Returns the next style name if the style present within the document else its own style name is returned as a next style name
                    if (this.Document.ActualFormatType == FormatType.Doc)
                    {
                        if (this.Document.Styles.FindByName(m_nextStyle) != null)
                            return m_nextStyle;
                        else
                            return this.Name;
                    }
                    else
                    {
                        if (this.Document.StyleNameIds.ContainsKey(m_nextStyle))
                            return this.Document.StyleNameIds[m_nextStyle];
                        else
                            return this.Name;
                    }
                }
                else
                    return null;
            }
            set
            {
                m_nextStyle = value;
            }
        }
        /// <summary>
        /// Gets or sets the link style name.
        /// </summary>
        /// <value>The link style.</value>
        internal string LinkStyle
        {
            get
            {
                return m_linkStyle;
            }
            set
            {
                m_linkStyle = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is primary style.
        /// </summary>
        /// <value>
        /// 	if this instance is primary style, set to <c>true</c>.
        /// </value>
        public bool IsPrimaryStyle
        {
            get
            {
                return m_isPrimaryStyle;
            }
            set
            {
                m_isPrimaryStyle = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is semi hidden.
        /// </summary>
        /// <value>
        /// 	if this instance is semi hidden, set to <c>true</c>.
        /// </value>
        internal bool IsSemiHidden
        {
            get
            {
                return m_isSemiHidden;
            }
            set
            {
                m_isSemiHidden = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether to unhide when used.
        /// </summary>
        /// <value>if unhide when used, set to <c>true</c>.</value>
        internal bool UnhideWhenUsed
        {
            get
            {
                return m_unhideWhenUsed;
            }
            set
            {
                m_unhideWhenUsed = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is custom.
        /// </summary>
        /// <value>if this instance is custom, set to <c>true</c>.</value>
        internal bool IsCustom
        {
            get
            {
                return m_isCustom;
            }
            set
            {
                m_isCustom = value;
            }
        }
        /// <summary>
        /// Gets the Builtinstyles
        /// </summary>
        internal Dictionary<string, string> BuiltinStyles
        {
            get
            {
                if (m_builtinStyles == null)
                    LoadBuiltinStyles();
                return m_builtinStyles;
            }
        }
        /// <summary>
        /// Gets the builtin style ids.
        /// </summary>
        /// <value>The builtin style ids.</value>
        internal Dictionary<string, int> BuiltinStyleIds
        {
            get
            {
                if (m_builtinStyleIds == null)
                    LoadBuiltinStyleIds();
                return m_builtinStyleIds;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Style"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        protected Style(WordDocument doc)
            : base(doc, doc)
        {
            m_chFormat = new WCharacterFormat(Document);
            m_chFormat.SetOwner(this);
            m_strName = "Style" + doc.Styles.Count;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Apply base style for current style.
        /// </summary>
        /// <param name="styleName"></param>
        public virtual void ApplyBaseStyle(string styleName)
        {
            m_baseStyle = m_doc.Styles.FindByName(styleName, StyleType);
            if (m_baseStyle == null)
            {
                Debug.WriteLine("No base style " + styleName + " with specified style type");
                m_baseStyle = m_doc.Styles.FindByName(styleName);
            }

            if (m_baseStyle == null)
            {
                m_baseStyle = new WParagraphStyle(m_doc);
            }

            CharacterFormat.ApplyBase(((Style)BaseStyle).CharacterFormat);
        }
        /// <summary>
        /// Applies the base style.
        /// </summary>
        /// <param name="bStyle">The built-in style.</param>
        public void ApplyBaseStyle(BuiltinStyle bStyle)
        {
            IStyle style = m_doc.AddStyle(bStyle);
            if (style != null)
            {
                ApplyBaseStyle(style.Name);
            }
        }
        /// <summary>
        /// Clones itself
        /// </summary>
        /// <returns></returns>
        public abstract IStyle Clone();
        #endregion

        #region Implementation
        /// <summary>
        /// Removes the base style.
        /// </summary>
        internal void RemoveBaseStyle()
        {
            WParagraphStyle pstyle = this as WParagraphStyle;
            pstyle.CharacterFormat.CharacterProps.BaseProperties = null;
            pstyle.CharacterFormat.BaseFormat = null;
            pstyle.ParagraphFormat.ParaProps.BaseProperties = null;
            pstyle.ParagraphFormat.BaseFormat = null;
            pstyle.m_baseStyle = null;
        }
        /// <summary>
        /// Set the name of the style.
        /// </summary>
        /// <param name="name"></param>
        internal void SetStyleName(string name)
        {
            if (name == null || name.Length == 0)
                throw new ArgumentNullException("Style Name should not be null or empty");
            
            m_strName = name;
        }
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            Style st = (Style)base.CloneImpl();
            st.m_chFormat = new WCharacterFormat(Document);
            st.m_chFormat.ImportContainer(CharacterFormat);
            return st;
        }
        /// <summary>
        /// Clones the relations.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal override void CloneRelationsTo(WordDocument doc, OwnerHolder nextOwner)
        {
            if (m_baseStyle != null)
            {
                if (doc.Styles.FindByName(m_baseStyle.Name, StyleType) == null)
                {
                    doc.Styles.Add(m_baseStyle.Clone());
                    string styleNameId = GetStyleNameId(m_baseStyle.Name);
                    if (Document.StyleNameIds.ContainsValue(m_baseStyle.Name)
                        && !doc.StyleNameIds.ContainsValue(m_baseStyle.Name)
                        && !doc.StyleNameIds.ContainsKey(styleNameId))
                        doc.StyleNameIds.Add(styleNameId, m_baseStyle.Name);
                }
                else if (doc.CurClonedSection != null)
                {
                    Dictionary<string, string> sourceCollection = (m_baseStyle.StyleType == StyleType.CharacterStyle) ?
                      doc.CurClonedSection.OldCharStylesHolder :
                      doc.CurClonedSection.OldParaStylesHolder;
                    if (sourceCollection.ContainsKey(m_baseStyle.Name))
                    {
                        (m_baseStyle as Style).SetStyleName(sourceCollection[m_baseStyle.Name]);
                    }
                    else
                    {
                        if (doc.Styles.FindByName(m_baseStyle.Name, StyleType) == null
                            || doc.ImportStyles)
                            m_baseStyle = AddNewStyle(m_baseStyle, doc);
                        else
                            m_baseStyle = doc.Styles.FindByName(m_baseStyle.Name, StyleType);
                    }
                }

                CharacterFormat.ApplyBase(((Style)BaseStyle).CharacterFormat);
            }
            //      if( doc.StandardAsciiFont == null )
            //      {
            //        doc.StandardAsciiFont = Document.StandardAsciiFont;
            //      }
            //      if( doc.StandardFarEastFont == null )
            //      {
            //        doc.StandardFarEastFont = Document.StandardFarEastFont;
            //      }
            //      if( doc.StandardNonFarEastFont == null )
            //      {
            //        doc.StandardNonFarEastFont = Document.StandardNonFarEastFont;
            //      }
        }
        /// <summary>
        /// Adds new style to the document
        /// </summary>
        /// <param name="sourceStyle">The source style.</param>
        /// <param name="doc">The doc.</param>
        /// <returns></returns>
        private IStyle AddNewStyle(IStyle sourceStyle, WordDocument doc)
        {
            IStyle newStyle = null;
            if (sourceStyle != null)
            {
                newStyle = sourceStyle.Clone();
                string guid;
                if(HasGuid(newStyle,out guid))
                    (newStyle as Style).SetStyleName(newStyle.Name.Replace(guid, Guid.NewGuid().ToString()));
                else
                    (newStyle as Style).SetStyleName(newStyle.Name + "_" + Guid.NewGuid().ToString());
                (newStyle as Style).StyleId = DEF_USER_STYLE_ID;
                if (newStyle.Name.Length > 63)
                {
                    (newStyle as Style).SetStyleName(newStyle.Name.Substring(0, 63));
                }
                doc.Styles.Add(newStyle);
                if (doc.CurClonedSection != null)
                {
                    if (sourceStyle.StyleType == StyleType.ParagraphStyle && !doc.CurClonedSection.OldParaStylesHolder.ContainsKey(sourceStyle.Name))
                    {
                        doc.CurClonedSection.OldParaStylesHolder.Add(sourceStyle.Name, newStyle.Name);
                    }
                    else if (sourceStyle.StyleType == StyleType.CharacterStyle && !doc.CurClonedSection.OldCharStylesHolder.ContainsKey(sourceStyle.Name))
                    {
                        doc.CurClonedSection.OldCharStylesHolder.Add(sourceStyle.Name, newStyle.Name);
                    }
                }
            }

            return newStyle;
        }
        /// <summary>
        /// Determines whether the style has Guid
        /// </summary>
        /// <param name="style">Style</param>
        /// <param name="guid">guid</param>
        /// <returns>
        /// Returns true, if style name contains Guid.
        /// </returns>
        private bool HasGuid(IStyle style, out string guid)
        {
            guid = string.Empty;
            char[] splitChar = { '-' };
            if (style.Name.Contains("_") && style.Name.Contains("-"))
            {
                int guidStartIndex = style.Name.LastIndexOf("_") + 1;
                if (style.Name.Length > guidStartIndex)
                    guid = style.Name.Substring(guidStartIndex);
                string[] guidColl = guid.Split(splitChar);
                if (guidColl.Length == 5       //Guid contains 32 digits in 5 groups separated by "-"
                    && guid.Length - 4 == 32   //Guid contains 32 digits
                    && guidColl[0].Length==8   //Guid structure (8 digits)-(4 digits)-(4 digits)-(4 digits)-(12 digits)
                    && guidColl[1].Length==4 
                    && guidColl[2].Length==4 
                    && guidColl[3].Length==4
                    && guidColl[4].Length== 12)
                { 
                    return true; 
                }
            }
            return false;
        }
        /// <summary>
        /// Imports the style to document.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal void ImportStyleTo(WordDocument doc)
        {
            IStyle newStyle = this.Clone();
            if (doc.Styles.FindByName(newStyle.Name, newStyle.StyleType) == null)
            {
                doc.Styles.Add(newStyle);
                string styleNameId = GetStyleNameId(newStyle.Name);
                if (Document.StyleNameIds.ContainsValue(newStyle.Name)
                    && !doc.StyleNameIds.ContainsValue(newStyle.Name)
                    && !doc.StyleNameIds.ContainsKey(styleNameId))
                    doc.StyleNameIds.Add(styleNameId, newStyle.Name);
            }

            //ApplyStyle( newStyle );
            if (doc.CurClonedSection != null)
            {
                if (this.StyleType == StyleType.CharacterStyle
                    && !doc.CurClonedSection.OldCharStylesHolder.ContainsKey(Name))
                {
                    doc.CurClonedSection.OldCharStylesHolder.Add(Name, Name);
                }
                else if (this.StyleType == StyleType.ParagraphStyle 
                    && !doc.CurClonedSection.OldParaStylesHolder.ContainsKey(Name))
                {
                    doc.CurClonedSection.OldParaStylesHolder.Add(Name, Name);
                }
            }
        }
        /// <summary>
        /// Gets style name Id.
        /// </summary>
        /// <param name="styleName"></param>
        private string GetStyleNameId(string styleName)
        {
            string styleNameId = "";
            foreach (KeyValuePair<string, string> pair in Document.StyleNameIds)
            {
                if (pair.Value == styleName)
                {
                    styleNameId = pair.Key;
                    break;
                }
            }
            return styleNameId;
        }
        /// <summary>
        /// Applies or adds style to document.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="foundStyle">The found style.</param>
        /// <returns></returns>
        internal IStyle ApplyOrImportStyleTo(WordDocument doc, IStyle foundStyle)
        {
            if (this.StyleType == foundStyle.StyleType)
            {
                string styleName = string.Empty;
                if (foundStyle.StyleType == StyleType.CharacterStyle
                    && doc.CurClonedSection.OldCharStylesHolder.ContainsKey(foundStyle.Name))
                {
                    styleName = doc.CurClonedSection.OldCharStylesHolder[foundStyle.Name];
                }
                else if (doc.CurClonedSection.OldParaStylesHolder.ContainsKey(foundStyle.Name))
                {
                    styleName = doc.CurClonedSection.OldParaStylesHolder[foundStyle.Name];
                }

                if (styleName != null && styleName.Length > 0 )
                {
                    IStyle style = doc.Styles.FindByName(styleName, foundStyle.StyleType);
                    if (style != null)
                    {
                        return style;
                    }
                }
                else
                {
                    return AddNewStyle(this, doc);
                }
            }

            return this;
        }
        /// <summary>
        /// Creates the built-in paragraph style.
        /// </summary>
        /// <param name="bstyle">The built in style.</param>
        /// <param name="doc">The document.</param>
        /// <returns></returns>
        public static IStyle CreateBuiltinStyle(BuiltinStyle bStyle, WordDocument doc)
        {
            IStyle pStyle = new WParagraphStyle(doc);
            WParagraphStyle existingStyle = doc.Styles.FindByName(Style.BuiltInToName(bStyle), StyleType.ParagraphStyle) as WParagraphStyle;
            if (existingStyle != null)
                return existingStyle;
            BuiltinStyleLoader.LoadStyle(pStyle, bStyle);
            if (pStyle.Name == "Normal" && pStyle.StyleType == StyleType.ParagraphStyle)
            {
                (pStyle as WParagraphStyle).CharacterFormat.LocaleIdASCII = 1033;
            }
            return pStyle;
        }
        /// <summary>
        /// Creates the built-in table style.
        /// </summary>
        /// <param name="bstyle">The built in style.</param>
        /// <param name="doc">The document.</param>
        /// <returns></returns>
        internal static IStyle CreateBuiltinStyle(BuiltinTableStyle bStyle, WordDocument doc)
        {
            IStyle tStyle = new WTableStyle(doc);
            BuiltinStyleLoader.LoadStyle(tStyle, bStyle);

            return tStyle;
        }
        /// <summary>
        /// Creates the built-in style.
        /// </summary>
        /// <param name="bStyle">The built-in style.</param>
        /// <param name="type">The type.</param>
        /// <param name="doc">The document.</param>
        /// <returns></returns>
        public static IStyle CreateBuiltinStyle(BuiltinStyle bStyle, StyleType type, WordDocument doc)
        {
            IStyle style = null;
            switch (type)
            {
                case StyleType.CharacterStyle:
                    style = new CharacterStyle(doc);
                    break;
                case StyleType.ParagraphStyle:
                    style = new WParagraphStyle(doc);
                    break;
                case StyleType.OtherStyle:
                    style = new ListStyle(doc);
                    break;
            }

            BuiltinStyleLoader.LoadStyle(style, bStyle);
            return style;
        }
        /// <summary>
        /// Built into name.
        /// </summary>
        /// <param name="bstyle">The built in style.</param>
        /// <returns></returns>
        internal static string BuiltInToName(BuiltinStyle bstyle)
        {
            return BuiltinStyleLoader.BuiltinStyleNames[(int)bstyle];
        }
        /// <summary>
        /// Built into name.
        /// </summary>
        /// <param name="bstyle">The built in style.</param>
        /// <returns></returns>
        internal static string BuiltInToName(BuiltinTableStyle bstyle)
        {
            return BuiltinStyleLoader.BuiltinTableStyleNames[(int)bstyle];
        }
        /// <summary>
        /// Converts string style names to BuiltinStyle.
        /// </summary>
        /// <param name="styleName">Name of the style.</param>
        /// <returns></returns>
        public static BuiltinStyle NameToBuiltIn(string styleName)
        {
            string name = styleName.Trim();
            BuiltinStyle builtInStyle = BuiltinStyle.User;
            int stylesCount = BuiltinStyleLoader.BuiltinStyleNames.Length;
            for (int i = 0; i < stylesCount; i++)
            {
                if (BuiltinStyleLoader.BuiltinStyleNames[i] == name)
                {
                    builtInStyle = (BuiltinStyle)i;
                    break;
                }
            }

            return builtInStyle;
        }
        /// <summary>
        /// Determines whether is list style the specified built in style.
        /// </summary>
        /// <param name="bstyle">The built in style.</param>
        /// <returns>
        /// 	<c>true</c> if it specifies list style, set to <c>true</c>.
        /// </returns>
        internal static bool IsListStyle(BuiltinStyle bstyle)
        {
            return BuiltinStyleLoader.IsListStyle(bstyle);
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal virtual void Close()
        {
            if (m_chFormat != null)
            {
                m_chFormat.Close();
                m_chFormat = null;
            }
        }
        /// <summary>
        /// Loads Builtin styles to dictionary
        /// </summary>
        internal void LoadBuiltinStyles()
        {
            m_builtinStyles = new Dictionary<string, string>();
            // Built in Paragraph styles
            m_builtinStyles.Add("normal", "Normal");
            m_builtinStyles.Add("heading 1", "Heading 1");
            m_builtinStyles.Add("heading 2", "Heading 2");
            m_builtinStyles.Add("heading 3", "Heading 3");
            m_builtinStyles.Add("heading 4", "Heading 4");
            m_builtinStyles.Add("heading 5", "Heading 5");
            m_builtinStyles.Add("heading 6", "Heading 6");
            m_builtinStyles.Add("heading 7", "Heading 7");
            m_builtinStyles.Add("heading 8", "Heading 8");
            m_builtinStyles.Add("heading 9", "Heading 9");
            m_builtinStyles.Add("index 1", "Index 1");
            m_builtinStyles.Add("index 2", "Index 2");
            m_builtinStyles.Add("index 3", "Index 3");
            m_builtinStyles.Add("index 4", "Index 4");
            m_builtinStyles.Add("index 5", "Index 5");
            m_builtinStyles.Add("index 6", "Index 6");
            m_builtinStyles.Add("index 7", "Index 7");
            m_builtinStyles.Add("index 8", "Index 8");
            m_builtinStyles.Add("index 9", "Index 9");
            m_builtinStyles.Add("toc 1", "TOC 1");
            m_builtinStyles.Add("toc 2", "TOC 2");
            m_builtinStyles.Add("toc 3", "TOC 3");
            m_builtinStyles.Add("toc 4", "TOC 4");
            m_builtinStyles.Add("toc 5", "TOC 5");
            m_builtinStyles.Add("toc 6", "TOC 6");
            m_builtinStyles.Add("toc 7", "TOC 7");
            m_builtinStyles.Add("toc 8", "TOC 8");
            m_builtinStyles.Add("toc 9", "TOC 9");
            m_builtinStyles.Add("normal indent", "Normal Indent");
            m_builtinStyles.Add("footnote text", "Footnote Text");
            m_builtinStyles.Add("comment text", "Comment Text");
            m_builtinStyles.Add("header", "Header");
            m_builtinStyles.Add("footer", "Footer");
            m_builtinStyles.Add("index heading", "Index Heading");
            m_builtinStyles.Add("caption", "Caption");
            m_builtinStyles.Add("table of figures", "Table of Figures");
            m_builtinStyles.Add("footnote reference", "Footnote Reference");
            m_builtinStyles.Add("comment reference", "Comment Reference");
            m_builtinStyles.Add("line number", "Line Number");
            m_builtinStyles.Add("page number", "Page Number");
            m_builtinStyles.Add("endnote reference", "Endnote Reference");
            m_builtinStyles.Add("endnote text", "Endnote Text");
            m_builtinStyles.Add("table of authorities", "Table of Authorities");
            m_builtinStyles.Add("macro", "Macro Text");
            m_builtinStyles.Add("toa heading", "TOA Heading");
            m_builtinStyles.Add("list", "List");
            m_builtinStyles.Add("list bullet", "List Bullet");
            m_builtinStyles.Add("list number", "List Number");
            m_builtinStyles.Add("list 2", "List 2");
            m_builtinStyles.Add("list 3", "List 3");
            m_builtinStyles.Add("list 4", "List 4");
            m_builtinStyles.Add("list 5", "List 5");
            m_builtinStyles.Add("list bullet 2", "List Bullet 2");
            m_builtinStyles.Add("list bullet 3", "List Bullet 3");
            m_builtinStyles.Add("list bullet 4", "List Bullet 4");
            m_builtinStyles.Add("list bullet 5", "List Bullet 5");
            m_builtinStyles.Add("list number 2", "List Number 2");
            m_builtinStyles.Add("list number 3", "List Number 3");
            m_builtinStyles.Add("list number 4", "List Number 4");
            m_builtinStyles.Add("list number 5", "List Number 5");
            m_builtinStyles.Add("title", "Title");
            m_builtinStyles.Add("closing", "Closing");
            m_builtinStyles.Add("signature", "Signature");
            m_builtinStyles.Add("default paragraph font", "Default Paragraph Font");
            m_builtinStyles.Add("body text", "Body Text");
            m_builtinStyles.Add("body text indent", "Body Text Indent");
            m_builtinStyles.Add("list continue", "List Continue");
            m_builtinStyles.Add("list continue 2", "List Continue 2");
            m_builtinStyles.Add("list continue 3", "List Continue 3");
            m_builtinStyles.Add("list continue 4", "List Continue 4");
            m_builtinStyles.Add("list continue 5", "List Continue 5");
            m_builtinStyles.Add("message header", "Message Header");
            m_builtinStyles.Add("subtitle", "Subtitle");
            m_builtinStyles.Add("salutation", "Salutation");
            m_builtinStyles.Add("date", "Date");
            m_builtinStyles.Add("body text first indent", "Body Text First Indent");
            m_builtinStyles.Add("body text first indent 2", "Body Text First Indent 2");
            m_builtinStyles.Add("note heading", "Note Heading");
            m_builtinStyles.Add("body text 2", "Body Text 2");
            m_builtinStyles.Add("body text 3", "Body Text 3");
            m_builtinStyles.Add("body text indent 2", "Body Text Indent 2");
            m_builtinStyles.Add("body text indent 3", "Body Text Indent 3");
            m_builtinStyles.Add("block text", "Block Text");
            m_builtinStyles.Add("hyperlink", "Hyperlink");
            m_builtinStyles.Add("followedhyperlink", "FollowedHyperlink");
            m_builtinStyles.Add("strong", "Strong");
            m_builtinStyles.Add("emphasis", "Emphasis");
            m_builtinStyles.Add("document map", "Document Map");
            m_builtinStyles.Add("plain text", "Plain Text");
            m_builtinStyles.Add("e-mail signature", "E-mail Signature");
            m_builtinStyles.Add("normal (web)", "Normal (Web)");
            m_builtinStyles.Add("html acronym", "HTML Acronym");
            m_builtinStyles.Add("html address", "HTML Address");
            m_builtinStyles.Add("html cite", "HTML Cite");
            m_builtinStyles.Add("html code", "HTML Code");
            m_builtinStyles.Add("html definition", "HTML Definition");
            m_builtinStyles.Add("html keyboard", "HTML Keyboard");
            m_builtinStyles.Add("html preformatted", "HTML Preformatted");
            m_builtinStyles.Add("html sample", "HTML Sample");
            m_builtinStyles.Add("html typewriter", "HTML Typewriter");
            m_builtinStyles.Add("html variable", "HTML Variable");
            m_builtinStyles.Add("comment subject", "Comment Subject");
            m_builtinStyles.Add("no list", "No List");
            m_builtinStyles.Add("balloon text", "Balloon Text");
            m_builtinStyles.Add("user", "User");
            m_builtinStyles.Add("nostyle", "NoStyle");

            //Builtin Table styles
            m_builtinStyles.Add("normal table", "Normal Table");
            m_builtinStyles.Add("table grid", "Table Grid");
            m_builtinStyles.Add("light shading", " Light Shading");
            m_builtinStyles.Add("light shading accent 1", "Light Shading Accent 1");
            m_builtinStyles.Add("light shading accent 2", "Light Shading Accent 2");
            m_builtinStyles.Add("light shading accent 3", "Light Shading Accent 3");
            m_builtinStyles.Add("light shading accent 4", "Light Shading Accent 4");
            m_builtinStyles.Add("light shading accent 5", "Light Shading Accent 5");
            m_builtinStyles.Add("light shading accent 6", "Light Shading Accent 6");
            m_builtinStyles.Add("light list", "Light List");
            m_builtinStyles.Add("light list accent 1", "Light List Accent 1");
            m_builtinStyles.Add("light list accent 2", "Light List Accent 2");
            m_builtinStyles.Add("light list accent 3", "Light List Accent 3");
            m_builtinStyles.Add("light list accent 4", "Light List Accent 4");
            m_builtinStyles.Add("light list accent 5", "Light List Accent 5");
            m_builtinStyles.Add("light list accent 6", "Light List Accent 6");
            m_builtinStyles.Add("light grid", "Light Grid");
            m_builtinStyles.Add("light grid accent 1", "Light Grid Accent 1");
            m_builtinStyles.Add("light grid accent 2", "Light Grid Accent 2");
            m_builtinStyles.Add("light grid accent 3", " Light Grid Accent 3");
            m_builtinStyles.Add("light grid accent 4", "Light Grid Accent 4");
            m_builtinStyles.Add("light grid accent 5", "Light Grid Accent 5");
            m_builtinStyles.Add("light grid accent 6", "Light Grid Accent 6");
            m_builtinStyles.Add("medium shading 1", "Medium Shading 1");
            m_builtinStyles.Add("medium shading 1 accent 1", "Medium Shading 1 Accent 1");
            m_builtinStyles.Add("medium shading 1 accent 2", "Medium Shading 1 Accent 2");
            m_builtinStyles.Add("medium shading 1 accent 3", "Medium Shading 1 Accent 3");
            m_builtinStyles.Add("medium shading 1 accent 4", "Medium Shading 1 Accent 4");
            m_builtinStyles.Add("medium shading 1 accent 5", "Medium Shading 1 Accent 5");
            m_builtinStyles.Add("medium shading 1 accent 6", "Medium Shading 1 Accent 6");
            m_builtinStyles.Add("medium shading 2", "Medium Shading 2");
            m_builtinStyles.Add("medium shading 2 accent 1", "Medium Shading 2 Accent 1");
            m_builtinStyles.Add("medium shading 2 accent 2", "Medium Shading 2 Accent 2");
            m_builtinStyles.Add("medium shading 2 accent 3", "Medium Shading 2 Accent 3");
            m_builtinStyles.Add("medium shading 2 accent 4", "Medium Shading 2 Accent 4");
            m_builtinStyles.Add("medium shading 2 accent 5", "Medium Shading 2 Accent 5");
            m_builtinStyles.Add("medium shading 2 accent 6", "Medium Shading 2 Accent 6");
            m_builtinStyles.Add("medium list 1", "Medium List 1");
            m_builtinStyles.Add("medium list 1 accent 1", "Medium List 1 Accent 1");
            m_builtinStyles.Add("medium list 1 accent 2", "Medium List 1 Accent 2");
            m_builtinStyles.Add("medium list 1 accent 3", "Medium List 1 Accent 3");
            m_builtinStyles.Add("medium list 1 accent 4", "Medium List 1 Accent 4");
            m_builtinStyles.Add("medium list 1 accent 5", "Medium List 1 Accent 5");
            m_builtinStyles.Add("medium list 1 accent 6", "Medium List 1 Accent 6");
            m_builtinStyles.Add("medium list 2", "Medium List 2");
            m_builtinStyles.Add("medium list 2 accent 1", "Medium List 2 Accent 1");
            m_builtinStyles.Add("medium list 2 accent 2", "Medium List 2 Accent 2");
            m_builtinStyles.Add("medium list 2 accent 3", "Medium List 2 Accent 3");
            m_builtinStyles.Add("medium list 2 accent 4", "Medium List 2 Accent 4");
            m_builtinStyles.Add("medium list 2 accent 5", "Medium List 2 Accent 5");
            m_builtinStyles.Add("medium list 2 accent 6", "Medium List 2 Accent 6");
            m_builtinStyles.Add("medium grid 1", "Medium Grid 1");
            m_builtinStyles.Add("medium grid 1 accent 1", "Medium Grid 1 Accent 1");
            m_builtinStyles.Add("medium grid 1 accent 2", "Medium Grid 1 Accent 2");
            m_builtinStyles.Add("medium grid 1 accent 3", "Medium Grid 1 Accent 3");
            m_builtinStyles.Add("medium grid 1 accent 4", "Medium Grid 1 Accent 4");
            m_builtinStyles.Add("medium grid 1 accent 5", "Medium Grid 1 Accent 5");
            m_builtinStyles.Add("medium grid 1 accent 6", "Medium Grid 1 Accent 6");
            m_builtinStyles.Add("medium grid 2", "Medium Grid 2");
            m_builtinStyles.Add("medium grid 2 accent 1", "Medium Grid 2 Accent 1");
            m_builtinStyles.Add("medium grid 2 accent 2", "Medium Grid 2 Accent 2");
            m_builtinStyles.Add("medium grid 2 accent 3", "Medium Grid 2 Accent 3");
            m_builtinStyles.Add("medium grid 2 accent 4", "Medium Grid 2 Accent 4");
            m_builtinStyles.Add("medium grid 2 accent 5", "Medium Grid 2 Accent 5");
            m_builtinStyles.Add("medium grid 2 accent 6", "Medium Grid 2 Accent 6");
            m_builtinStyles.Add("medium grid 3", "Medium Grid 3");
            m_builtinStyles.Add("medium grid 3 accent 1", "Medium Grid 3 Accent 1");
            m_builtinStyles.Add("medium grid 3 accent 2", "Medium Grid 3 Accent 2");
            m_builtinStyles.Add("medium grid 3 accent 3", "Medium Grid 3 Accent 3");
            m_builtinStyles.Add("medium grid 3 accent 4", "Medium Grid 3 Accent 4");
            m_builtinStyles.Add("medium grid 3 accent 5", "Medium Grid 3 Accent5");
            m_builtinStyles.Add("medium grid 3 accent 6", "Medium Grid 3 Accent 6");
            m_builtinStyles.Add("dark list", "Dark List");
            m_builtinStyles.Add("dark list accent 1", "Dark List Accent 1");
            m_builtinStyles.Add("dark list accent 2", "Dark List Accent 2");
            m_builtinStyles.Add("dark list accent 3", "Dark List Accent 3");
            m_builtinStyles.Add("dark list accent 4", "Dark List Accent 4");
            m_builtinStyles.Add("dark list accent 5", "Dark List Accent 5");
            m_builtinStyles.Add("dark list accent 6", "Dark List Accent 6");
            m_builtinStyles.Add("colorful shading", "Colorful Shading");
            m_builtinStyles.Add("colorful shading accent 1", "Colorful Shading Accent 1");
            m_builtinStyles.Add("colorful shading accent 2", "Colorful Shading Accent 2");
            m_builtinStyles.Add("colorful shading accent 3", "Colorful Shading Accent 3");
            m_builtinStyles.Add("colorful shading accent 4", "Colorful Shading Accent 4");
            m_builtinStyles.Add("colorful shading accent 5", "Colorful Shading Accent 5");
            m_builtinStyles.Add("colorful shading accent 6", "Colorful Shading Accent 6");
            m_builtinStyles.Add("colorful list", "Colorful List");
            m_builtinStyles.Add("colorful list accent 1", "Colorful List Accent 1");
            m_builtinStyles.Add("colorful list accent 2", "Colorful List Accent 2");
            m_builtinStyles.Add("colorful list accent 3", "Colorful List Accent 3");
            m_builtinStyles.Add("colorful list accent 4", "Colorful List Accent 4");
            m_builtinStyles.Add("colorful list accent 5", "Colorful List Accent 5");
            m_builtinStyles.Add("colorful list accent 6", "Colorful List Accent 6");
            m_builtinStyles.Add("colorful grid", "Colorful Grid");
            m_builtinStyles.Add("colorful grid accent 1", "Colorful Grid Accent 1");
            m_builtinStyles.Add("colorful grid accent 2", "Colorful Grid Accent 2");
            m_builtinStyles.Add("colorful grid accent 3", "Colorful Grid Accent 3");
            m_builtinStyles.Add("colorful grid accent 4", "Colorful Grid Accent 4");
            m_builtinStyles.Add("colorful grid accent 5", "Colorful Grid Accent 5");
            m_builtinStyles.Add("colorful grid accent 6", "Colorful Grid Accent 6");
            m_builtinStyles.Add("table 3d effects 1", "Table 3D effects 1");
            m_builtinStyles.Add("table 3d effects 2", "Table 3D effects 2");
            m_builtinStyles.Add("table 3d effects 3", "Table 3D effects 3");
            m_builtinStyles.Add("table classic 1", "Table Classic 1");
            m_builtinStyles.Add("table classic 2", "Table Classic 2");
            m_builtinStyles.Add("table classic 3", "Table Classic 3");
            m_builtinStyles.Add("table classic 4", "Table Classic 4");
            m_builtinStyles.Add("table colorful 1", "Table Colorful 1");
            m_builtinStyles.Add("table colorful 2", "Table Colorful 2");
            m_builtinStyles.Add("table colorful 3", "Table Colorful 3");
            m_builtinStyles.Add("table columns 1", "Table Columns 1");
            m_builtinStyles.Add("table columns 2", "Table Columns 2");
            m_builtinStyles.Add("table columns 3", "Table Columns 3");
            m_builtinStyles.Add("table columns 4", "Table Columns 4");
            m_builtinStyles.Add("table columns 5", "Table Columns 5");
            m_builtinStyles.Add("table contemporary", "Table Contemporary");
            m_builtinStyles.Add("table elegant", "Table Elegant");
            m_builtinStyles.Add("table grid 1", "Table Grid 1");
            m_builtinStyles.Add("table grid 2", "Table Grid 2");
            m_builtinStyles.Add("table grid 3", "Table Grid 3");
            m_builtinStyles.Add("table grid 4", "Table Grid 4");
            m_builtinStyles.Add("table grid 5", "Table Grid 5");
            m_builtinStyles.Add("table grid 6", "Table Grid 6");
            m_builtinStyles.Add("table grid 7", "Table Grid 7");
            m_builtinStyles.Add("table grid 8", "Table Grid 8");
            m_builtinStyles.Add("table list 1", "Table List 1");
            m_builtinStyles.Add("table list 2", "Table List 2");
            m_builtinStyles.Add("table list 3", "Table List 3");
            m_builtinStyles.Add("table list 4", "Table List 4");
            m_builtinStyles.Add("table list 5", "Table List 5");
            m_builtinStyles.Add("table list 6", "Table List 6");
            m_builtinStyles.Add("table list 7", "Table List 7");
            m_builtinStyles.Add("table list 8", "Table List 8");
            m_builtinStyles.Add("table professional", "Table Professional");
            m_builtinStyles.Add("table simple 1", "Table Simple 1");
            m_builtinStyles.Add("table simple 2", "Table Simple 2");
            m_builtinStyles.Add("table simple 3", "Table Simple 3");
            m_builtinStyles.Add("table subtle 1", "Table Subtle 1");
            m_builtinStyles.Add("table subtle 2", "Table Subtle 2");
            m_builtinStyles.Add("table theme", "Table Theme");
            m_builtinStyles.Add("table web 1", "Table Web 1");
            m_builtinStyles.Add("table web 2", "Table Web 2");
            m_builtinStyles.Add("table web 3", "Table Web 3");
        }
        /// <summary>
        /// Loads the builtin style ids.
        /// </summary>
        private void LoadBuiltinStyleIds()
        {
            m_builtinStyleIds = new Dictionary<string, int>();
            m_builtinStyleIds.Add("normal", 0);
            m_builtinStyleIds.Add("defaultparagraphfont", 65);
            m_builtinStyleIds.Add("nospacing", 157);
            m_builtinStyleIds.Add("heading1", 1);
            m_builtinStyleIds.Add("heading2", 2);
            m_builtinStyleIds.Add("heading3", 3);
            m_builtinStyleIds.Add("heading4", 4);
            m_builtinStyleIds.Add("heading5", 5);
            m_builtinStyleIds.Add("heading6", 6);
            m_builtinStyleIds.Add("heading7", 7);
            m_builtinStyleIds.Add("heading8", 8);
            m_builtinStyleIds.Add("heading9", 9);
            m_builtinStyleIds.Add("title", 62);
            m_builtinStyleIds.Add("subtitle", 74);
            m_builtinStyleIds.Add("subtleemphasis", 260);
            m_builtinStyleIds.Add("emphasis", 88);
            m_builtinStyleIds.Add("intenseemphasis", 261);
            m_builtinStyleIds.Add("strong", 87);
            m_builtinStyleIds.Add("quote", 180);
            m_builtinStyleIds.Add("intensequote", 181);
            m_builtinStyleIds.Add("subtlereference", 262);
            m_builtinStyleIds.Add("intensereference", 263);
            m_builtinStyleIds.Add("booktitle", 264);
            m_builtinStyleIds.Add("listparagraph", 179);
            m_builtinStyleIds.Add("caption", 34);
            m_builtinStyleIds.Add("bibliography", 265);
            m_builtinStyleIds.Add("toc1", 19);
            m_builtinStyleIds.Add("toc2", 20);
            m_builtinStyleIds.Add("toc3", 21);
            m_builtinStyleIds.Add("toc4", 22);
            m_builtinStyleIds.Add("toc5", 23);
            m_builtinStyleIds.Add("toc6", 24);
            m_builtinStyleIds.Add("toc7", 25);
            m_builtinStyleIds.Add("toc8", 26);
            m_builtinStyleIds.Add("toc9", 27);
            m_builtinStyleIds.Add("tocheading", 266);
            m_builtinStyleIds.Add("tablegrid", 154);
            m_builtinStyleIds.Add("lightshading", 158);
            m_builtinStyleIds.Add("lightshadingaccent1", 172);
            m_builtinStyleIds.Add("lightshadingaccent2", 190);
            m_builtinStyleIds.Add("lightshadingaccent3", 204);
            m_builtinStyleIds.Add("lightshadingaccent4", 218);
            m_builtinStyleIds.Add("lightshadingaccent5", 232);
            m_builtinStyleIds.Add("lightshadingaccent6", 246);
            m_builtinStyleIds.Add("lightlist", 159);
            m_builtinStyleIds.Add("lightlistaccent1", 173);
            m_builtinStyleIds.Add("lightlistaccent2", 191);
            m_builtinStyleIds.Add("lightlistaccent3", 205);
            m_builtinStyleIds.Add("lightlistaccent4", 219);
            m_builtinStyleIds.Add("lightlistaccent5", 233);
            m_builtinStyleIds.Add("lightlistaccent6", 247);
            m_builtinStyleIds.Add("lightgrid", 160);
            m_builtinStyleIds.Add("lightgridaccent1", 174);
            m_builtinStyleIds.Add("lightgridaccent2", 192);
            m_builtinStyleIds.Add("lightgridaccent3", 206);
            m_builtinStyleIds.Add("lightgridaccent4", 220);
            m_builtinStyleIds.Add("lightgridaccent5", 234);
            m_builtinStyleIds.Add("lightgridaccent6", 248);
            m_builtinStyleIds.Add("mediumshading1", 161);
            m_builtinStyleIds.Add("mediumshading1accent1", 175);
            m_builtinStyleIds.Add("mediumshading1accent2", 193);
            m_builtinStyleIds.Add("mediumshading1accent3", 207);
            m_builtinStyleIds.Add("mediumshading1accent4", 221);
            m_builtinStyleIds.Add("mediumshading1accent5", 235);
            m_builtinStyleIds.Add("mediumshading1accent6", 249);
            m_builtinStyleIds.Add("mediumshading2", 162);
            m_builtinStyleIds.Add("mediumshading2accent1", 176);
            m_builtinStyleIds.Add("mediumshading2accent2", 194);
            m_builtinStyleIds.Add("mediumshading2accent3", 208);
            m_builtinStyleIds.Add("mediumshading2accent4", 222);
            m_builtinStyleIds.Add("mediumshading2accent5", 236);
            m_builtinStyleIds.Add("mediumshading2accent6", 250);
            m_builtinStyleIds.Add("mediumlist1", 163);
            m_builtinStyleIds.Add("mediumlist1accent1", 177);
            m_builtinStyleIds.Add("mediumlist1accent2", 195);
            m_builtinStyleIds.Add("mediumlist1accent3", 209);
            m_builtinStyleIds.Add("mediumlist1accent4", 223);
            m_builtinStyleIds.Add("mediumlist1accent5", 237);
            m_builtinStyleIds.Add("mediumlist1accent6", 251);
            m_builtinStyleIds.Add("mediumlist2", 164);
            m_builtinStyleIds.Add("mediumlist2accent1", 182);
            m_builtinStyleIds.Add("mediumlist2accent2", 196);
            m_builtinStyleIds.Add("mediumlist2accent3", 210);
            m_builtinStyleIds.Add("mediumlist2accent4", 224);
            m_builtinStyleIds.Add("mediumlist2accent5", 238);
            m_builtinStyleIds.Add("mediumlist2accent6", 252);
            m_builtinStyleIds.Add("mediumgrid1", 165);
            m_builtinStyleIds.Add("mediumgrid1accent1", 183);
            m_builtinStyleIds.Add("mediumgrid1accent2", 197);
            m_builtinStyleIds.Add("mediumgrid1accent3", 211);
            m_builtinStyleIds.Add("mediumgrid1accent4", 225);
            m_builtinStyleIds.Add("mediumgrid1accent5", 239);
            m_builtinStyleIds.Add("mediumgrid1accent6", 253);
            m_builtinStyleIds.Add("mediumgrid2", 166);
            m_builtinStyleIds.Add("mediumgrid2accent1", 184);
            m_builtinStyleIds.Add("mediumgrid2accent2", 198);
            m_builtinStyleIds.Add("mediumgrid2accent3", 212);
            m_builtinStyleIds.Add("mediumgrid2accent4", 226);
            m_builtinStyleIds.Add("mediumgrid2accent5", 240);
            m_builtinStyleIds.Add("mediumgrid2accent6", 254);
            m_builtinStyleIds.Add("mediumgrid3", 167);
            m_builtinStyleIds.Add("mediumgrid3accent1", 185);
            m_builtinStyleIds.Add("mediumgrid3accent2", 199);
            m_builtinStyleIds.Add("mediumgrid3accent3", 213);
            m_builtinStyleIds.Add("mediumgrid3accent4", 227);
            m_builtinStyleIds.Add("mediumgrid3accent5", 241);
            m_builtinStyleIds.Add("mediumgrid3accent6", 255);
            m_builtinStyleIds.Add("darklist", 168);
            m_builtinStyleIds.Add("darklistaccent1", 186);
            m_builtinStyleIds.Add("darklistaccent2", 200);
            m_builtinStyleIds.Add("darklistaccent3", 214);
            m_builtinStyleIds.Add("darklistaccent4", 228);
            m_builtinStyleIds.Add("darklistaccent5", 242);
            m_builtinStyleIds.Add("darklistaccent6", 256);
            m_builtinStyleIds.Add("colorfulshading", 169);
            m_builtinStyleIds.Add("colorfulshadingaccent1 ", 187);
            m_builtinStyleIds.Add("colorfulshadingaccent2", 201);
            m_builtinStyleIds.Add("colorfulshadingaccent3 ", 215);
            m_builtinStyleIds.Add("colorfulshadingaccent4", 229);
            m_builtinStyleIds.Add("colorfulshadingaccent5", 243);
            m_builtinStyleIds.Add("colorfulshadingaccent6", 257);
            m_builtinStyleIds.Add("colorfullist", 170);
            m_builtinStyleIds.Add("colorfullistaccent1", 188);
            m_builtinStyleIds.Add("colorfullistaccent2", 202);
            m_builtinStyleIds.Add("colorfullistaccent3", 216);
            m_builtinStyleIds.Add("colorfullistaccent4", 230);
            m_builtinStyleIds.Add("colorfullistaccent5", 244);
            m_builtinStyleIds.Add("colorfullistaccent6", 258);
            m_builtinStyleIds.Add("colorfulgrid", 171);
            m_builtinStyleIds.Add("colorfulgridaccent1", 189);
            m_builtinStyleIds.Add("colorfulgridaccent2", 203);
            m_builtinStyleIds.Add("colorfulgridaccent3", 217);
            m_builtinStyleIds.Add("colorfulgridaccent4", 231);
            m_builtinStyleIds.Add("colorfulgridaccent5", 245);
            m_builtinStyleIds.Add("colorfulgridaccent6", 259);
            m_builtinStyleIds.Add("balloontext", 153);
            m_builtinStyleIds.Add("blocktext", 84);
            m_builtinStyleIds.Add("bodytext", 66);
            m_builtinStyleIds.Add("bodytext2", 80);
            m_builtinStyleIds.Add("bodytext3", 81);
            m_builtinStyleIds.Add("bodytextfirstindent", 77);
            m_builtinStyleIds.Add("bodytextfirstindent2", 78);
            m_builtinStyleIds.Add("bodytextindent", 67);
            m_builtinStyleIds.Add("bodytextindent2", 82);
            m_builtinStyleIds.Add("bodytextindent3", 83);
            m_builtinStyleIds.Add("closing", 63);
            m_builtinStyleIds.Add("commentreference", 39);
            m_builtinStyleIds.Add("commentsubject", 106);
            m_builtinStyleIds.Add("commenttext", 30);
            m_builtinStyleIds.Add("date", 76);
            m_builtinStyleIds.Add("documentmap", 89);
            m_builtinStyleIds.Add("e-mailsignature", 91);
            m_builtinStyleIds.Add("endnotereference", 42);
            m_builtinStyleIds.Add("endnotetext", 43);
            m_builtinStyleIds.Add("envelopeaddress", 36);
            m_builtinStyleIds.Add("envelopereturn", 37);
            m_builtinStyleIds.Add("followedhyperlink", 86);
            m_builtinStyleIds.Add("footer", 32);
            m_builtinStyleIds.Add("footnotereference", 38);
            m_builtinStyleIds.Add("footnotetext", 29);
            m_builtinStyleIds.Add("header", 31);
            m_builtinStyleIds.Add("htmlacronym", 95);
            m_builtinStyleIds.Add("htmladdress", 96);
            m_builtinStyleIds.Add("htmlcite", 97);
            m_builtinStyleIds.Add("htmlcode", 98);
            m_builtinStyleIds.Add("htmldefinition", 99);
            m_builtinStyleIds.Add("htmlkeyboard", 100);
            m_builtinStyleIds.Add("htmlpreformatted", 101);
            m_builtinStyleIds.Add("htmlsample", 102);
            m_builtinStyleIds.Add("htmltypewriter", 103);
            m_builtinStyleIds.Add("htmlvariable", 104);
            m_builtinStyleIds.Add("hyperlink", 85);
            m_builtinStyleIds.Add("index1", 10);
            m_builtinStyleIds.Add("index2", 11);
            m_builtinStyleIds.Add("index3", 12);
            m_builtinStyleIds.Add("index4", 13);
            m_builtinStyleIds.Add("index5", 14);
            m_builtinStyleIds.Add("index6", 15);
            m_builtinStyleIds.Add("index7", 16);
            m_builtinStyleIds.Add("index8", 17);
            m_builtinStyleIds.Add("index9", 18);
            m_builtinStyleIds.Add("indexheading", 33);
            m_builtinStyleIds.Add("linenumber", 40);
            m_builtinStyleIds.Add("list", 47);
            m_builtinStyleIds.Add("list2", 50);
            m_builtinStyleIds.Add("list3", 51);
            m_builtinStyleIds.Add("list4", 52);
            m_builtinStyleIds.Add("list5", 53);
            m_builtinStyleIds.Add("listbullet", 48);
            m_builtinStyleIds.Add("listbullet2", 54);
            m_builtinStyleIds.Add("listbullet3", 55);
            m_builtinStyleIds.Add("listbullet4", 56);
            m_builtinStyleIds.Add("listbullet5", 57);
            m_builtinStyleIds.Add("listcontinue", 68);
            m_builtinStyleIds.Add("listcontinue2", 69);
            m_builtinStyleIds.Add("listcontinue3", 70);
            m_builtinStyleIds.Add("listcontinue4", 71);
            m_builtinStyleIds.Add("listcontinue5", 72);
            m_builtinStyleIds.Add("listnumber", 49);
            m_builtinStyleIds.Add("listnumber2", 58);
            m_builtinStyleIds.Add("listnumber3", 59);
            m_builtinStyleIds.Add("listnumber4", 60);
            m_builtinStyleIds.Add("listnumber5", 61);
            m_builtinStyleIds.Add("macrotext", 45);
            m_builtinStyleIds.Add("messageheader", 73);
            m_builtinStyleIds.Add("nolist", 107);
            m_builtinStyleIds.Add("normal(web)", 94);
            m_builtinStyleIds.Add("normalindent", 28);
            m_builtinStyleIds.Add("noteheading", 79);
            m_builtinStyleIds.Add("pagenumber", 41);
            m_builtinStyleIds.Add("placeholdertext", 156);
            m_builtinStyleIds.Add("plaintext", 90);
            m_builtinStyleIds.Add("salutation", 75);
            m_builtinStyleIds.Add("signature", 64);
            m_builtinStyleIds.Add("table3deffects1", 142);
            m_builtinStyleIds.Add("table3deffects2", 143);
            m_builtinStyleIds.Add("table3deffects3", 144);
            m_builtinStyleIds.Add("tableclassic1", 114);
            m_builtinStyleIds.Add("tableclassic2", 115);
            m_builtinStyleIds.Add("tableclassic3", 116);
            m_builtinStyleIds.Add("tableclassic4", 117);
            m_builtinStyleIds.Add("tablecolorful1", 118);
            m_builtinStyleIds.Add("tablecolorful2", 119);
            m_builtinStyleIds.Add("tablecolorful3", 120);
            m_builtinStyleIds.Add("tablecolumns1", 121);
            m_builtinStyleIds.Add("tablecolumns2", 122);
            m_builtinStyleIds.Add("tablecolumns3", 123);
            m_builtinStyleIds.Add("tablecolumns4", 124);
            m_builtinStyleIds.Add("tablecolumns5", 125);
            m_builtinStyleIds.Add("tablecontemporary", 145);
            m_builtinStyleIds.Add("tableelegant", 146);
            m_builtinStyleIds.Add("tablegrid1", 126);
            m_builtinStyleIds.Add("tablegrid2", 127);
            m_builtinStyleIds.Add("tablegrid3", 128);
            m_builtinStyleIds.Add("tablegrid4", 129);
            m_builtinStyleIds.Add("tablegrid5", 130);
            m_builtinStyleIds.Add("tablegrid6", 131);
            m_builtinStyleIds.Add("tablegrid7", 132);
            m_builtinStyleIds.Add("tablegrid8", 133);
            m_builtinStyleIds.Add("tablelist1", 134);
            m_builtinStyleIds.Add("tablelist2", 135);
            m_builtinStyleIds.Add("tablelist3", 136);
            m_builtinStyleIds.Add("tablelist4", 137);
            m_builtinStyleIds.Add("tablelist5", 138);
            m_builtinStyleIds.Add("tablelist6", 139);
            m_builtinStyleIds.Add("tablelist7", 140);
            m_builtinStyleIds.Add("tablelist8", 141);
            m_builtinStyleIds.Add("tablenormal", 105);
            m_builtinStyleIds.Add("normaltable", 105);
            m_builtinStyleIds.Add("tableofauthorities", 44);
            m_builtinStyleIds.Add("tableoffigures", 35);
            m_builtinStyleIds.Add("tableprofessional", 147);
            m_builtinStyleIds.Add("tablesimple1", 111);
            m_builtinStyleIds.Add("tablesimple2", 112);
            m_builtinStyleIds.Add("tablesimple3", 113);
            m_builtinStyleIds.Add("tablesubtle1", 148);
            m_builtinStyleIds.Add("tablesubtle2", 149);
            m_builtinStyleIds.Add("tabletheme", 155);
            m_builtinStyleIds.Add("tableweb1", 150);
            m_builtinStyleIds.Add("tableweb2", 151);
            m_builtinStyleIds.Add("tableweb3", 152);
            m_builtinStyleIds.Add("toaheading", 46);
            m_builtinStyleIds.Add("htmltopofform", 92);
            m_builtinStyleIds.Add("htmlbottomofform", 93);
            m_builtinStyleIds.Add("revision", 178);
            m_builtinStyleIds.Add("outlinelist1", 108);
            m_builtinStyleIds.Add("outlinelist2", 109);
            m_builtinStyleIds.Add("outlinelist3", 110);
        }
        #endregion

        #region Implementation / xml
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            writer.WriteValue(XDLSConstants.StyleNameAttr, Name);
            writer.WriteValue(XDLSConstants.StyleIdAttr, m_styleId);
            writer.WriteValue(XDLSConstants.TypeTag, (Enum)StyleType);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            m_strName = reader.ReadString(XDLSConstants.StyleNameAttr);
            m_styleId = reader.ReadInt(XDLSConstants.StyleIdAttr);
        }
//#if !SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void InitXDLSHolder()
        {
            XDLSHolder.EnableID = true;
            XDLSHolder.AddRefElement(XDLSConstants.StyleBaseTag, m_baseStyle);
            XDLSHolder.AddElement(XDLSConstants.CharacterFormatTag, m_chFormat);
        }
//#endif
        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="index"></param>
        protected override void RestoreReference(string name, int index)
        {
            if (index > -1)
            {
                m_baseStyle = Document.Styles[index];
            }
        }
        #endregion

        /// <summary>
        /// The base class BuiltinStyleLoader specifies Built-in styles.
        /// </summary>
        public class BuiltinStyleLoader
        {
            #region Constants
            /// <summary>
            /// 
            /// </summary>
            private const string DEF_DOCIO_RESOURCES = "Syncfusion.DocIO.Resources";
            private const string DEF_STYLE_TAG = "builtin-styles";
            private const int DEF_LIST_STYLES_NUMBER = 10;
            /// <summary>
            /// Built-in style names.
            /// </summary>
            public static readonly string[] BuiltinStyleNames = new string[]
      {
        "Normal",
        "Heading 1",
        "Heading 2",
        "Heading 3",
        "Heading 4",
        "Heading 5",
        "Heading 6",
        "Heading 7",
        "Heading 8",
        "Heading 9",
        "Index 1",
        "Index 2",
        "Index 3",
        "Index 4",
        "Index 5",
        "Index 6",
        "Index 7",
        "Index 8",
        "Index 9",
        "TOC 1",
        "TOC 2",
        "TOC 3",
        "TOC 4",
        "TOC 5",
        "TOC 6",
        "TOC 7",
        "TOC 8",
        "TOC 9",
        "Normal Indent",
        "Footnote Text",
        "Comment Text",
        "Header",
        "Footer",
        "Index Heading",
        "Caption",
        "Table of Figures",
        "Footnote Reference",
        "Comment Reference",
        "Line Number",
        "Page Number",
        "Endnote Reference",
        "Endnote Text",
        "Table of Authorities",
        "Macro Text",
        "TOA Heading",
        "List",
        "List Bullet",
        "List Number",
        "List 2",
        "List 3",
        "List 4",
        "List 5",
        "List Bullet 2",
        "List Bullet 3",
        "List Bullet 4",
        "List Bullet 5",
        "List Number 2",
        "List Number 3",
        "List Number 4",
        "List Number 5",
        "Title",
        "Closing",
        "Signature",
        "Default Paragraph Font",
        "Body Text",
        "Body Text Indent",
        "List Continue",
        "List Continue 2",
        "List Continue 3",
        "List Continue 4",
        "List Continue 5",
        "Message Header",
        "Subtitle",
        "Salutation",
        "Date",
        "Body Text First Indent",
        "Body Text First Indent 2",
        "Note Heading",
        "Body Text 2",
        "Body Text 3",
        "Body Text Indent 2",
        "Body Text Indent 3",
        "Block Text",
        "Hyperlink",
        "FollowedHyperlink",
        "Strong",
        "Emphasis",
        "Document Map",
        "Plain Text",
        "E-mail Signature",
        "Normal (Web)",
        "HTML Acronym",
        "HTML Address",
        "HTML Cite",
        "HTML Code",
        "HTML Definition",
        "HTML Keyboard",
        "HTML Preformatted",
        "HTML Sample",
        "HTML Typewriter",
        "HTML Variable",
        "Comment Subject",
        "No List",               
        "Balloon Text",
        "User",
        "NoStyle"
      };
            /// <summary>
            /// Built-in table style names.
            /// </summary>
            internal static readonly string[] BuiltinTableStyleNames = new string[]
      {
          "Normal Table",
          "Table Grid",
          "Light Shading", 
          "Light Shading Accent 1",
          "Light Shading Accent 2",
          "Light Shading Accent 3",
          "Light Shading Accent 4",
          "Light Shading Accent 5",
          "Light Shading Accent 6",
          "Light List",
          "Light List Accent 1",
          "Light List Accent 2",
          "Light List Accent 3",
          "Light List Accent 4",
          "Light List Accent 5",
          "Light List Accent 6",
          "Light Grid",
          "Light Grid Accent 1",
          "Light Grid Accent 2",
          "Light Grid Accent 3",
          "Light Grid Accent 4",
          "Light Grid Accent 5",
          "Light Grid Accent 6",
          "Medium Shading 1",
          "Medium Shading 1 Accent 1",
          "Medium Shading 1 Accent 2",
          "Medium Shading 1 Accent 3",
          "Medium Shading 1 Accent 4",
          "Medium Shading 1 Accent 5",
          "Medium Shading 1 Accent 6",
          "Medium Shading 2",
          "Medium Shading 2 Accent 1",
          "Medium Shading 2 Accent 2",
          "Medium Shading 2 Accent 3",
          "Medium Shading 2 Accent 4",
          "Medium Shading 2 Accent 5",
          "Medium Shading 2 Accent 6",
          "Medium List 1",
          "Medium List 1 Accent 1",
          "Medium List 1 Accent 2",
          "Medium List 1 Accent 3",
          "Medium List 1 Accent 4",
          "Medium List 1 Accent 5",
          "Medium List 1 Accent 6",
          "Medium List 2",
          "Medium List 2 Accent 1",
          "Medium List 2 Accent 2",
          "Medium List 2 Accent 3",
          "Medium List 2 Accent 4",
          "Medium List 2 Accent 5",
          "Medium List 2 Accent 6",
          "Medium Grid 1",
          "Medium Grid 1 Accent 1",
          "Medium Grid 1 Accent 2",
          "Medium Grid 1 Accent 3",
          "Medium Grid 1 Accent 4",
          "Medium Grid 1 Accent 5",
          "Medium Grid 1 Accent 6",
          "Medium Grid 2",
          "Medium Grid 2 Accent 1",
          "Medium Grid 2 Accent 2",
          "Medium Grid 2 Accent 3",
          "Medium Grid 2 Accent 4",
          "Medium Grid 2 Accent 5",
          "Medium Grid 2 Accent 6",
          "Medium Grid 3",
          "Medium Grid 3 Accent 1",
          "Medium Grid 3 Accent 2",
          "Medium Grid 3 Accent 3",
          "Medium Grid 3 Accent 4",
          "Medium Grid 3 Accent5",
          "Medium Grid 3 Accent 6",
          "Dark List",
          "Dark List Accent 1",
          "Dark List Accent 2",
          "Dark List Accent 3",
          "Dark List Accent 4",
          "Dark List Accent 5",
          "Dark List Accent 6",
          "Colorful Shading",
          "Colorful Shading Accent 1",
          "Colorful Shading Accent 2",
          "Colorful Shading Accent 3",
          "Colorful Shading Accent 4",
          "Colorful Shading Accent 5",
          "Colorful Shading Accent 6",
          "Colorful List",
          "Colorful List Accent 1",
          "Colorful List Accent 2",
          "Colorful List Accent 3",
          "Colorful List Accent 4",
          "Colorful List Accent 5",
          "Colorful List Accent 6",
          "Colorful Grid",
          "Colorful Grid Accent 1",
          "Colorful Grid Accent 2",
          "Colorful Grid Accent 3",
          "Colorful Grid Accent 4",
          "Colorful Grid Accent 5",
          "Colorful Grid Accent 6",
          "Table 3D effects 1",
          "Table 3D effects 2",
          "Table 3D effects 3",
          "Table Classic 1",
          "Table Classic 2",
          "Table Classic 3",
          "Table Classic 4",
          "Table Colorful 1",
          "Table Colorful 2",
          "Table Colorful 3",
          "Table Columns 1",
          "Table Columns 2",
          "Table Columns 3",
          "Table Columns 4",
          "Table Columns 5",
          "Table Contemporary",
          "Table Elegant",
          "Table Grid 1",
          "Table Grid 2",
          "Table Grid 3",
          "Table Grid 4",
          "Table Grid 5",
          "Table Grid 6",
          "Table Grid 7",
          "Table Grid 8",
          "Table List 1",
          "Table List 2",
          "Table List 3",
          "Table List 4",
          "Table List 5",
          "Table List 6",
          "Table List 7",
          "Table List 8",
          "Table Professional",
          "Table Simple 1",
          "Table Simple 2",
          "Table Simple 3",
          "Table Subtle 1",
          "Table Subtle 2",
          "Table Theme",
          "Table Web 1",
          "Table Web 2",
          "Table Web 3"
      };
            #endregion

            #region Fields
            /// <summary>
            /// 
            /// </summary>      
            [ThreadStatic]
            private static Stream m_xmlStream = null;
            #endregion

            #region Implementation
            /// <summary>
            /// Loads the style.
            /// </summary>
            /// <param name="style">The style.</param>
            /// <param name="bStyle">The b style.</param>
            internal static void LoadStyle(IStyle style, BuiltinStyle bStyle)
            {
                UpdateXMLResAndReader();

                m_xmlStream.Position = 0;
#if SILVERLIGHT || WP
        XmlReader xmlReader = XmlReader.Create( m_xmlStream );

#else
                XmlReader xmlReader = new XmlTextReader(m_xmlStream);

#endif
                while (xmlReader.Name != DEF_STYLE_TAG)
                {
                    xmlReader.Read();
                }

                xmlReader.Read();
                string styleName = Style.BuiltInToName(bStyle);

                string attrValue = string.Empty;

                while (!xmlReader.EOF)
                {
                    if (xmlReader.NodeType == XmlNodeType.Element)
                    {
                        attrValue = xmlReader.GetAttribute("Name");
                        if (attrValue == styleName)
                        {
                            XDLSReader xdlsReader = new XDLSReader(xmlReader);
                            xdlsReader.ReadChildElement(style);
                            return;
                        }
                        else
                        {
                            xmlReader.Skip();
                        }
                    }
                    else
                    {
                        xmlReader.Read();
                    }
                }
            }
            /// <summary>
            /// Loads the style.
            /// </summary>
            /// <param name="style">The style.</param>
            /// <param name="bStyle">The b style.</param>
            internal static void LoadStyle(IStyle style, BuiltinTableStyle bStyle)
            {
                style.Name = Style.BuiltInToName(bStyle);
                switch(bStyle)
                {
                    case BuiltinTableStyle.TableNormal:
                        #region Table Normal
                        LoadStyleTableNormal(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableGrid:
                        #region Table Grid
                        LoadStyleTableGrid(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.LightShading:
                        #region LightShading
                        LoadStyleLightShading(style, Color.Black, Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 192, 192, 192));
                        #endregion
                        break;
                    case BuiltinTableStyle.LightShadingAccent1:
                        #region LightShading Accent 1
                        LoadStyleLightShading(style, Color.FromArgb(255, 54, 95, 145), Color.FromArgb(255, 79, 129, 189), Color.FromArgb(255, 211, 223, 238));
                        #endregion
                        break;
                    case BuiltinTableStyle.LightShadingAccent2:
                        #region LightShading Accent 2
                        LoadStyleLightShading(style, Color.FromArgb(255, 148, 54, 52), Color.FromArgb(255, 192, 80, 77), Color.FromArgb(255, 239, 211, 210));
                        #endregion
                        break;
                    case BuiltinTableStyle.LightShadingAccent3:
                        #region LightShading Accent 3
                        LoadStyleLightShading(style, Color.FromArgb(255, 118, 146, 60), Color.FromArgb(255, 155, 187, 89), Color.FromArgb(255, 230, 238, 213));
                        #endregion
                        break;
                    case BuiltinTableStyle.LightShadingAccent4:
                        #region LightShading Accent 4
                        LoadStyleLightShading(style, Color.FromArgb(255, 95, 73, 122), Color.FromArgb(255, 128, 100, 162), Color.FromArgb(255, 223, 216, 232));
                        #endregion
                        break;
                    case BuiltinTableStyle.LightShadingAccent5:
                        #region LightShading Accent 5
                        LoadStyleLightShading(style, Color.FromArgb(255, 49, 132, 155), Color.FromArgb(255, 75, 172, 198), Color.FromArgb(255, 210, 234, 241));
                        #endregion
                        break;
                    case BuiltinTableStyle.LightShadingAccent6:
                        #region LightShading Accent 6
                        LoadStyleLightShading(style, Color.FromArgb(255, 227, 108, 10), Color.FromArgb(255, 247, 150, 70), Color.FromArgb(255, 253, 228, 208));
                        #endregion
                        break;
                    case BuiltinTableStyle.LightList:
                        #region Light List
                        LoadStyleLightList(style, Color.FromArgb(255, 0, 0, 0), Color.Black);
                        #endregion
                        break;
                    case BuiltinTableStyle.LightListAccent1:
                        #region Light List Accent 1
                        LoadStyleLightList(style, Color.FromArgb(255, 79, 129, 189), Color.FromArgb(255, 79, 129, 189));
                        #endregion
                        break;
                    case BuiltinTableStyle.LightListAccent2:
                        #region Light List Accent 2
                        LoadStyleLightList(style, Color.FromArgb(255, 192, 80, 77), Color.FromArgb(255, 192, 80, 77));
                        #endregion
                        break;
                    case BuiltinTableStyle.LightListAccent3:
                        #region Light List Accent 3
                        LoadStyleLightList(style, Color.FromArgb(255, 155, 187, 89), Color.FromArgb(255, 155, 187, 89));
                        #endregion
                        break;
                    case BuiltinTableStyle.LightListAccent4:
                        #region Light List Accent 4
                        LoadStyleLightList(style, Color.FromArgb(255, 128, 100, 162), Color.FromArgb(255, 128, 100, 162));
                        #endregion
                        break;
                    case BuiltinTableStyle.LightListAccent5:
                        #region Light List Accent 5
                        LoadStyleLightList(style, Color.FromArgb(255, 75, 172, 198), Color.FromArgb(255, 75, 172, 198));
                        #endregion
                        break;
                    case BuiltinTableStyle.LightListAccent6:
                        #region Light List Accent 6
                        LoadStyleLightList(style, Color.FromArgb(255, 247, 150, 70), Color.FromArgb(255, 247, 150, 70));
                        #endregion
                        break;
                    case BuiltinTableStyle.LightGrid:
                        #region Light Grid
                        LoadStyleLightGrid(style, Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 192, 192, 192));
                        #endregion
                        break;
                    case BuiltinTableStyle.LightGridAccent1:
                        #region Light Grid Accent 1
                        LoadStyleLightGrid(style, Color.FromArgb(255, 79, 129, 189), Color.FromArgb(255, 211, 223, 238));
                        #endregion
                        break;
                    case BuiltinTableStyle.LightGridAccent2:
                        #region Light Grid Accent 2
                        LoadStyleLightGrid(style, Color.FromArgb(255, 192, 80, 77), Color.FromArgb(255, 239, 211, 210));
                        #endregion
                        break;
                    case BuiltinTableStyle.LightGridAccent3:
                        #region Light Grid Accent 3
                        LoadStyleLightGrid(style, Color.FromArgb(255, 155, 187, 89), Color.FromArgb(255, 230, 238, 213));
                        #endregion
                        break;
                    case BuiltinTableStyle.LightGridAccent4:
                        #region Light Grid Accent 4
                        LoadStyleLightGrid(style, Color.FromArgb(255, 128, 100, 162), Color.FromArgb(255, 223, 216, 232));
                        #endregion
                        break;
                    case BuiltinTableStyle.LightGridAccent5:
                        #region Light Grid Accent 5
                        LoadStyleLightGrid(style, Color.FromArgb(255, 75, 172, 198), Color.FromArgb(255, 210, 234, 241));
                        #endregion
                        break;
                    case BuiltinTableStyle.LightGridAccent6:
                        #region Light Grid Accent 6
                        LoadStyleLightGrid(style, Color.FromArgb(255, 247, 150, 70), Color.FromArgb(255, 253, 228, 208));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumShading1:
                        #region Medium Shading 1
                        LoadStyleMediumShading1(style, Color.FromArgb(255, 64, 64, 64), Color.Black, Color.FromArgb(255, 192, 192, 192));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumShading1Accent1:
                        #region Medium Shading 1 Accent 1
                        LoadStyleMediumShading1(style, Color.FromArgb(255, 123, 160, 205), Color.FromArgb(255, 79, 129, 189), Color.FromArgb(255, 211, 223, 238));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumShading1Accent2:
                        #region Medium Shading 1 Accent 2
                        LoadStyleMediumShading1(style, Color.FromArgb(255, 207, 123, 121), Color.FromArgb(255, 192, 80, 77), Color.FromArgb(255, 239, 211, 210));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumShading1Accent3:
                        #region Medium Shading 1 Accent 3
                        LoadStyleMediumShading1(style, Color.FromArgb(255, 179, 204, 130), Color.FromArgb(255, 155, 187, 89), Color.FromArgb(255, 230, 238, 213));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumShading1Accent4:
                        #region Medium Shading 1 Accent 4
                        LoadStyleMediumShading1(style, Color.FromArgb(255, 159, 138, 185), Color.FromArgb(255, 128, 100, 162), Color.FromArgb(255, 223, 216, 232));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumShading1Accent5:
                        #region Medium Shading 1 Accent 5
                        LoadStyleMediumShading1(style, Color.FromArgb(255, 120, 192, 212), Color.FromArgb(255, 75, 172, 198), Color.FromArgb(255, 210, 234, 241));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumShading1Accent6:
                        #region Medium Shading 1 Accent 6
                        LoadStyleMediumShading1(style, Color.FromArgb(255, 249, 176, 116), Color.FromArgb(255, 247, 150, 70), Color.FromArgb(255, 253, 228, 208));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumShading2:
                        #region Medium Shading 2
                        LoadStyleMediumShading2(style, Color.Black);
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumShading2Accent1:
                        #region Medium Shading 2 Accent 1
                        LoadStyleMediumShading2(style, Color.FromArgb(255, 79, 129, 189));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumShading2Accent2:
                        #region Medium Shading 2 Accent 2
                        LoadStyleMediumShading2(style, Color.FromArgb(255, 192, 80, 77));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumShading2Accent3:
                        #region Medium Shading 2 Accent 3
                        LoadStyleMediumShading2(style, Color.FromArgb(255, 155, 187, 89));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumShading2Accent4:
                        #region Medium Shading 2 Accent 4
                        LoadStyleMediumShading2(style, Color.FromArgb(255, 128, 100, 162));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumShading2Accent5:
                        #region Medium Shading 2 Accent 5
                        LoadStyleMediumShading2(style, Color.FromArgb(255, 75, 172, 198));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumShading2Accent6:
                        #region Medium Shading 2 Accent 6
                        LoadStyleMediumShading2(style, Color.FromArgb(255, 247, 150, 70));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumList1:
                        #region Medium List 1
                        LoadStyleMediumList1(style, Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 192, 192, 192));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumList1Accent1:
                        #region Medium List 1 Accent 1
                        LoadStyleMediumList1(style, Color.FromArgb(255, 79, 129, 189), Color.FromArgb(255, 211, 223, 238));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumList1Accent2:
                        #region Medium List 1 Accent 2
                        LoadStyleMediumList1(style,  Color.FromArgb(255, 192, 80, 77), Color.FromArgb(255, 239, 211, 210));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumList1Accent3:
                        #region Medium List 1 Accent 3
                        LoadStyleMediumList1(style, Color.FromArgb(255, 155, 187, 89), Color.FromArgb(255, 230, 238, 213));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumList1Accent4:
                        #region Medium List 1 Accent 4
                        LoadStyleMediumList1(style, Color.FromArgb(255, 128, 100, 162), Color.FromArgb(255, 223, 216, 232));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumList1Accent5:
                        #region Medium List 1 Accent 5
                        LoadStyleMediumList1(style, Color.FromArgb(255, 75, 172, 198), Color.FromArgb(255, 210, 234, 241));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumList1Accent6:
                        #region Medium List 1 Accent 6
                        LoadStyleMediumList1(style, Color.FromArgb(255, 247, 150, 70), Color.FromArgb(255, 253, 228, 208));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumList2:
                        #region Medium List 2
                        LoadStyleMediumList2(style, Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 192, 192, 192));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumList2Accent1:
                        #region Medium List 2 Accent 1
                        LoadStyleMediumList2(style, Color.FromArgb(255, 79, 129, 189), Color.FromArgb(255, 211, 223, 238));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumList2Accent2:
                        #region Medium List 2 Accent 2
                        LoadStyleMediumList2(style, Color.FromArgb(255, 192, 80, 77), Color.FromArgb(255, 239, 211, 210));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumList2Accent3:
                        #region Medium List 2 Accent 3
                        LoadStyleMediumList2(style, Color.FromArgb(255, 155, 187, 89), Color.FromArgb(255, 230, 238, 213));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumList2Accent4:
                        #region Medium List 2 Accent 4
                        LoadStyleMediumList2(style, Color.FromArgb(255, 128, 100, 162), Color.FromArgb(255, 223, 216, 232));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumList2Accent5:
                        #region Medium List 2 Accent 5
                        LoadStyleMediumList2(style, Color.FromArgb(255, 75, 172, 198), Color.FromArgb(255, 210, 234, 241));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumList2Accent6:
                        #region Medium List 2 Accent 6
                        LoadStyleMediumList2(style, Color.FromArgb(255, 247, 150, 70), Color.FromArgb(255, 253, 228, 208));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumGrid1:
                        #region Medium Grid 1
                        LoadStyleMediumGrid1(style, Color.FromArgb(255, 64, 64, 64), Color.FromArgb(255, 192, 192, 192), Color.FromArgb(255, 128, 128, 128));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumGrid1Accent1:
                        #region Medium Grid 1 Accent 1
                        LoadStyleMediumGrid1(style, Color.FromArgb(255, 123, 160, 205), Color.FromArgb(255, 211, 223, 238), Color.FromArgb(255, 167, 191, 222));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumGrid1Accent2:
                        #region Medium Grid 1 Accent 2
                        LoadStyleMediumGrid1(style, Color.FromArgb(255, 207, 123, 121), Color.FromArgb(255, 239, 211, 210), Color.FromArgb(255, 223, 167, 166));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumGrid1Accent3:
                        #region Medium Grid 1 Accent 3
                        LoadStyleMediumGrid1(style, Color.FromArgb(255, 179, 204, 130), Color.FromArgb(255, 230, 238, 213), Color.FromArgb(255, 205, 221, 172));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumGrid1Accent4:
                        #region Medium Grid 1 Accent 4
                        LoadStyleMediumGrid1(style, Color.FromArgb(255, 159, 138, 185), Color.FromArgb(255, 223, 216, 232), Color.FromArgb(255, 191, 177, 208));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumGrid1Accent5:
                        #region Medium Grid 1 Accent 5
                        LoadStyleMediumGrid1(style, Color.FromArgb(255, 120, 192, 212), Color.FromArgb(255, 210, 234, 241), Color.FromArgb(255, 165, 213, 226));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumGrid1Accent6:
                        #region Medium Grid 1 Accent 6
                        LoadStyleMediumGrid1(style, Color.FromArgb(255, 249, 176, 116), Color.FromArgb(255, 253, 228, 208), Color.FromArgb(255, 251, 202, 162));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumGrid2:
                        #region Medium Grid 2
                        LoadStyleMediumGrid2(style, Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 192, 192, 192), Color.FromArgb(255, 230, 230, 230), Color.FromArgb(255, 204, 204, 204), Color.FromArgb(255, 128, 128, 128));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumGrid2Accent1:
                        #region Medium Grid 2 Accent 1
                        LoadStyleMediumGrid2(style, Color.FromArgb(255, 79, 129, 189), Color.FromArgb(255, 211, 223, 238), Color.FromArgb(255, 237, 242, 248), Color.FromArgb(255, 219, 229, 241), Color.FromArgb(255, 167, 191, 222));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumGrid2Accent2:
                        #region Medium Grid 2 Accent 2
                        LoadStyleMediumGrid2(style, Color.FromArgb(255, 192, 80, 77), Color.FromArgb(255, 239, 211, 210), Color.FromArgb(255, 248, 237, 237), Color.FromArgb(255, 242, 219, 219), Color.FromArgb(255, 223, 167, 166));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumGrid2Accent3:
                        #region Medium Grid 2 Accent 3
                        LoadStyleMediumGrid2(style, Color.FromArgb(255, 155, 187, 89), Color.FromArgb(255, 230, 238, 213), Color.FromArgb(255, 245, 248, 238), Color.FromArgb(255, 234, 241, 221), Color.FromArgb(255, 205, 221, 172));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumGrid2Accent4:
                        #region Medium Grid 2 Accent 4
                        LoadStyleMediumGrid2(style, Color.FromArgb(255, 128, 100, 162), Color.FromArgb(255, 223, 216, 232), Color.FromArgb(255, 242, 239, 246), Color.FromArgb(255, 229, 223, 236), Color.FromArgb(255, 191, 177, 208));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumGrid2Accent5:
                        #region Medium Grid 2 Accent 5
                        LoadStyleMediumGrid2(style, Color.FromArgb(255, 75, 172, 198), Color.FromArgb(255, 210, 234, 241), Color.FromArgb(255, 237, 246, 249), Color.FromArgb(255, 218, 238, 243), Color.FromArgb(255, 165, 213, 226));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumGrid2Accent6:
                        #region Medium Grid 2 Accent 6
                        LoadStyleMediumGrid2(style, Color.FromArgb(255, 247, 150, 70), Color.FromArgb(255, 253, 228, 208), Color.FromArgb(255, 254, 244, 236), Color.FromArgb(255, 253, 233, 217), Color.FromArgb(255, 251, 202, 162));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumGrid3:
                        #region Medium Grid 3
                        LoadStyleMediumGrid3(style, Color.FromArgb(255, 192, 192, 192), Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 128, 128, 128));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumGrid3Accent1:
                        #region Medium Grid 3 Accent 1
                        LoadStyleMediumGrid3(style, Color.FromArgb(255, 211, 223, 238), Color.FromArgb(255, 79, 129, 189), Color.FromArgb(255, 167, 191, 222));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumGrid3Accent2:
                        #region Medium Grid 3 Accent 2
                        LoadStyleMediumGrid3(style, Color.FromArgb(255, 239, 211, 210), Color.FromArgb(255, 192, 80, 77), Color.FromArgb(255, 223, 167, 166));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumGrid3Accent3:
                        #region Medium Grid 3 Accent 3
                        LoadStyleMediumGrid3(style, Color.FromArgb(255, 230, 238, 213), Color.FromArgb(255, 155, 187, 89), Color.FromArgb(255, 205, 221, 172));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumGrid3Accent4:
                        #region Medium Grid 3 Accent 4
                        LoadStyleMediumGrid3(style, Color.FromArgb(255, 223, 216, 232), Color.FromArgb(255, 128, 100, 162), Color.FromArgb(255, 191, 177, 208));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumGrid3Accent5:
                        #region Medium Grid 3 Accent 5
                        LoadStyleMediumGrid3(style, Color.FromArgb(255, 210, 234, 241), Color.FromArgb(255, 75, 172, 198), Color.FromArgb(255, 165, 213, 226));
                        #endregion
                        break;
                    case BuiltinTableStyle.MediumGrid3Accent6:
                        #region Medium Grid 3 Accent 6
                        LoadStyleMediumGrid3(style, Color.FromArgb(255, 253, 228, 208), Color.FromArgb(255, 247, 150, 70), Color.FromArgb(255, 251, 202, 162));
                        #endregion
                        break;
                    case BuiltinTableStyle.DarkList:
                        #region Dark List
                        LoadStyleDarkList(style, Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 0, 0, 0));
                        #endregion
                        break;
                    case BuiltinTableStyle.DarkListAccent1:
                        #region Dark List Accent 1
                        LoadStyleDarkList(style, Color.FromArgb(255, 79, 129, 189), Color.FromArgb(255, 36, 63, 96), Color.FromArgb(255, 54, 95, 145));
                        #endregion
                        break;
                    case BuiltinTableStyle.DarkListAccent2:
                        #region Dark List Accent 2
                        LoadStyleDarkList(style, Color.FromArgb(255, 192, 80, 77), Color.FromArgb(255, 98, 36, 35), Color.FromArgb(255, 148, 54, 52));
                        #endregion
                        break;
                    case BuiltinTableStyle.DarkListAccent3:
                        #region Dark List Accent 3
                        LoadStyleDarkList(style, Color.FromArgb(255, 155, 187, 89), Color.FromArgb(255, 78, 97, 40), Color.FromArgb(255, 118, 146, 60));
                        #endregion
                        break;
                    case BuiltinTableStyle.DarkListAccent4:
                        #region Dark List Accent 4
                        LoadStyleDarkList(style, Color.FromArgb(255, 128, 100, 162), Color.FromArgb(255, 63, 49, 81), Color.FromArgb(255, 95, 73, 122));
                        #endregion
                        break;
                    case BuiltinTableStyle.DarkListAccent5:
                        #region Dark List Accent 5
                        LoadStyleDarkList(style, Color.FromArgb(255, 75, 172, 198), Color.FromArgb(255, 32, 88, 103), Color.FromArgb(255, 49, 132, 155));
                        #endregion
                        break;
                    case BuiltinTableStyle.DarkListAccent6:
                        #region Dark List Accent 6
                        LoadStyleDarkList(style, Color.FromArgb(255, 247, 150, 70), Color.FromArgb(255, 151, 71, 6), Color.FromArgb(255, 227, 108, 10));
                        #endregion
                        break;
                    case BuiltinTableStyle.ColorfulShading:
                        #region Colorful Shading
                        LoadStyleColorfulShading(style, Color.FromArgb(255, 192, 80, 77), Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 230, 230, 230), Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 153, 153, 153), Color.FromArgb(255, 128, 128, 128));
                        #endregion
                        break;
                    case BuiltinTableStyle.ColorfulShadingAccent1:
                        #region Colorful Shading Accent 1
                        LoadStyleColorfulShading(style, Color.FromArgb(255, 192, 80, 77), Color.FromArgb(255, 79, 129, 189), Color.FromArgb(255, 237, 242, 248), Color.FromArgb(255, 44, 76, 116), Color.FromArgb(255, 184, 204, 228), Color.FromArgb(255, 167, 191, 222));
                        #endregion
                        break;
                    case BuiltinTableStyle.ColorfulShadingAccent2:
                        #region Colorful Shading Accent 2
                        LoadStyleColorfulShading(style, Color.FromArgb(255, 192, 80, 77), Color.FromArgb(255, 192, 80, 77), Color.FromArgb(255, 248, 237, 237), Color.FromArgb(255, 119, 44, 42), Color.FromArgb(255, 229, 184, 183), Color.FromArgb(255, 223, 167, 166));
                        #endregion
                        break;
                    case BuiltinTableStyle.ColorfulShadingAccent3:
                        #region Colorful Shading Accent 3
                        LoadStyleColorfulShading(style, Color.FromArgb(255, 128, 100, 162), Color.FromArgb(255, 155, 187, 89), Color.FromArgb(255, 245, 248, 238), Color.FromArgb(255, 94, 117, 48), Color.FromArgb(255, 214, 227, 188), Color.FromArgb(255, 205, 221, 172));
                        #endregion
                        break;
                    case BuiltinTableStyle.ColorfulShadingAccent4:
                        #region Colorful Shading Accent 4
                        LoadStyleColorfulShading(style, Color.FromArgb(255, 155, 187, 89), Color.FromArgb(255, 128, 100, 162), Color.FromArgb(255, 242, 239, 246), Color.FromArgb(255, 76, 59, 98), Color.FromArgb(255, 204, 192, 217), Color.FromArgb(255, 191, 177, 208));
                        #endregion
                        break;
                    case BuiltinTableStyle.ColorfulShadingAccent5:
                        #region Colorful Shading Accent 5
                        LoadStyleColorfulShading(style, Color.FromArgb(255, 247, 150, 70), Color.FromArgb(255, 75, 172, 198), Color.FromArgb(255, 237, 246, 249), Color.FromArgb(255, 39, 106, 124), Color.FromArgb(255, 182, 221, 232), Color.FromArgb(255, 165, 213, 226));
                        #endregion
                        break;
                    case BuiltinTableStyle.ColorfulShadingAccent6:
                        #region Colorful Shading Accent 6
                        LoadStyleColorfulShading(style, Color.FromArgb(255, 75, 172, 198), Color.FromArgb(255, 247, 150, 70), Color.FromArgb(255, 254, 244, 236), Color.FromArgb(255, 182, 86, 8), Color.FromArgb(255, 251, 212, 180), Color.FromArgb(255, 251, 202, 162));
                        #endregion
                        break;
                    case BuiltinTableStyle.ColorfulList:
                        #region Colorful List
                        LoadStyleColorfulList(style, Color.FromArgb(255, 230, 230, 230), Color.FromArgb(255, 158, 58, 56), Color.FromArgb(255, 192, 192, 192), Color.FromArgb(255, 204, 204, 204));
                        #endregion
                        break;
                    case BuiltinTableStyle.ColorfulListAccent1:
                        #region Colorful List Accent 1
                        LoadStyleColorfulList(style, Color.FromArgb(255, 237, 242, 248), Color.FromArgb(255, 158, 58, 56), Color.FromArgb(255, 211, 223, 238), Color.FromArgb(255, 219, 229, 241));
                        #endregion
                        break;
                    case BuiltinTableStyle.ColorfulListAccent2:
                        #region Colorful List Accent 2
                        LoadStyleColorfulList(style, Color.FromArgb(255, 248, 237, 237), Color.FromArgb(255, 158, 58, 56), Color.FromArgb(255, 239, 211, 210), Color.FromArgb(255, 242, 219, 219));
                        #endregion
                        break;
                    case BuiltinTableStyle.ColorfulListAccent3:
                        #region Colorful List Accent 3
                        LoadStyleColorfulList(style, Color.FromArgb(255, 245, 248, 238), Color.FromArgb(255, 102, 78, 130), Color.FromArgb(255, 230, 238, 213), Color.FromArgb(255, 234, 241, 221));
                        #endregion
                        break;
                    case BuiltinTableStyle.ColorfulListAccent4:
                        #region Colorful List Accent 4
                        LoadStyleColorfulList(style, Color.FromArgb(255, 242, 239, 246), Color.FromArgb(255, 126, 156, 64), Color.FromArgb(255, 223, 216, 232), Color.FromArgb(255, 229, 223, 236));
                        #endregion
                        break;
                    case BuiltinTableStyle.ColorfulListAccent5:
                        #region Colorful List Accent 5
                        LoadStyleColorfulList(style, Color.FromArgb(255, 237, 246, 249), Color.FromArgb(255, 242, 115, 10), Color.FromArgb(255, 210, 234, 241), Color.FromArgb(255, 218, 238, 243));
                        #endregion
                        break;
                    case BuiltinTableStyle.ColorfulListAccent6:
                        #region Colorful List Accent 6
                        LoadStyleColorfulList(style, Color.FromArgb(255, 254, 244, 236), Color.FromArgb(255, 52, 141, 165), Color.FromArgb(255, 253, 228, 208), Color.FromArgb(255, 253, 233, 217));
                        #endregion
                        break;
                    case BuiltinTableStyle.ColorfulGrid:
                        #region Colorful Grid
                        LoadStyleColorfulGrid(style, Color.FromArgb(255, 204, 204, 204), Color.FromArgb(255, 153, 153, 153), Color.FromArgb(255, 0, 0, 0), Color.FromArgb(255, 128, 128, 128));
                        #endregion
                        break;
                    case BuiltinTableStyle.ColorfulGridAccent1:
                        #region Colorful Grid Accent 1
                        LoadStyleColorfulGrid(style, Color.FromArgb(255, 219, 229, 241), Color.FromArgb(255, 184, 204, 228), Color.FromArgb(255, 54, 95, 145), Color.FromArgb(255, 167, 191, 222));
                        #endregion
                        break;
                    case BuiltinTableStyle.ColorfulGridAccent2:
                        #region Colorful Grid Accent 2
                        LoadStyleColorfulGrid(style, Color.FromArgb(255, 242, 219, 219), Color.FromArgb(255, 229, 184, 183), Color.FromArgb(255, 148, 54, 52), Color.FromArgb(255, 223, 167, 166));
                        #endregion
                        break;
                    case BuiltinTableStyle.ColorfulGridAccent3:
                        #region Colorful Grid Accent 3
                        LoadStyleColorfulGrid(style, Color.FromArgb(255, 234, 241, 221), Color.FromArgb(255, 214, 227, 188), Color.FromArgb(255, 118, 146, 60), Color.FromArgb(255, 205, 221, 172));
                        #endregion
                        break;
                    case BuiltinTableStyle.ColorfulGridAccent4:
                        #region Colorful Grid Accent 4
                        LoadStyleColorfulGrid(style, Color.FromArgb(255, 229, 223, 236), Color.FromArgb(255, 204, 192, 217), Color.FromArgb(255, 95, 73, 122), Color.FromArgb(255, 191, 177, 208));
                        #endregion
                        break;
                    case BuiltinTableStyle.ColorfulGridAccent5:
                        #region Colorful Grid Accent 5
                        LoadStyleColorfulGrid(style, Color.FromArgb(255, 218, 238, 243), Color.FromArgb(255, 182, 221, 232), Color.FromArgb(255, 49, 132, 155), Color.FromArgb(255, 165, 213, 226));
                        #endregion
                        break;
                    case BuiltinTableStyle.ColorfulGridAccent6:
                        #region Colorful Grid Accent 6
                        LoadStyleColorfulGrid(style, Color.FromArgb(255, 253, 233, 217), Color.FromArgb(255, 251, 212, 180), Color.FromArgb(255, 227, 108, 10), Color.FromArgb(255, 251, 202, 162));
                        #endregion
                        break;
                    case BuiltinTableStyle.Table3Deffects1:
                        #region Table 3D effects 1
                        LoadStyleTable3Deffects1(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.Table3Deffects2:
                        #region Table 3D effects 2
                        LoadStyleTable3Deffects2(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.Table3Deffects3:
                        #region Table 3D effects 3
                        LoadStyleTable3Deffects3(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableClassic1:
                        #region Table Classic 1
                        LoadStyleTableClassic1(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableClassic2:
                        #region Table Classic 2
                        LoadStyleTableClassic2(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableClassic3:
                        #region Table Classic 3
                        LoadStyleTableClassic3(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableClassic4:
                        #region Table Classic 4
                        LoadStyleTableClassic4(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableColorful1:
                        #region Table Colorful 1
                        LoadStyleTableColorful1(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableColorful2:
                        #region Table Colorful 2
                        LoadStyleTableColorful2(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableColorful3:
                        #region Table Colorful 3
                        LoadStyleTableColorful3(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableColumns1:
                        #region Table Columns 1
                        LoadStyleTableColumns1(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableColumns2:
                        #region Table Columns 2
                        LoadStyleTableColumns2(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableColumns3:
                        #region Table Columns 3
                        LoadStyleTableColumns3(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableColumns4:
                        #region Table Columns 4
                        LoadStyleTableColumns4(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableColumns5:
                        #region Table Columns 5
                        LoadStyleTableColumns5(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableContemporary:
                        #region Table Contemporary
                        LoadStyleTableContemporary(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableElegant:
                        #region Table Elegant
                        LoadStyleTableElegant(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableGrid1:
                        #region Table Grid 1
                        LoadStyleTableGrid1(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableGrid2:
                        #region Table Grid 2
                        LoadStyleTableGrid2(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableGrid3:
                        #region Table Grid 3
                        LoadStyleTableGrid3(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableGrid4:
                        #region Table Grid 4
                        LoadStyleTableGrid4(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableGrid5:
                        #region Table Grid 5
                        LoadStyleTableGrid5(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableGrid6:
                        #region Table Grid 6
                        LoadStyleTableGrid6(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableGrid7:
                        #region Table Grid 7
                        LoadStyleTableGrid7(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableGrid8:
                        #region Table Grid 8
                        LoadStyleTableGrid8(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableList1:
                        #region Table List 1
                        LoadStyleTableList1(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableList2:
                        #region Table List 2
                        LoadStyleTableList2(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableList3:
                        #region Table List 3
                        LoadStyleTableList3(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableList4:
                        #region Table List 4
                        LoadStyleTableList4(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableList5:
                        #region Table List 5
                        LoadStyleTableList5(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableList6:
                        #region Table List 6
                        LoadStyleTableList6(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableList7:
                        #region Table List 7
                        LoadStyleTableList7(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableList8:
                        #region Table List 8
                        LoadStyleTableList8(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableProfessional:
                        #region Table Professional
                        LoadStyleTableProfessional(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableSimple1:
                        #region Table Simple 1
                        LoadStyleTableSimple1(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableSimple2:
                        #region Table Simple 2
                        LoadStyleTableSimple2(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableSimple3:
                        #region Table Simple 3
                        LoadStyleTableSimple3(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableSubtle1:
                        #region Table Subtle 1
                        LoadStyleTableSubtle1(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableSubtle2:
                        #region Table Subtle 2
                        LoadStyleTableSubtle2(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableTheme:
                        #region Table Theme
                        LoadStyleTableTheme(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableWeb1:
                        #region Table Web 1
                        LoadStyleTableWeb1(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableWeb2:
                        #region Table Web 2
                        LoadStyleTableWeb2(style);
                        #endregion
                        break;
                    case BuiltinTableStyle.TableWeb3:
                        #region Table Web 3
                        LoadStyleTableWeb3(style);
                        #endregion
                        break;
                }
            }
            /// <summary>
            /// Loads the table style Normal Table.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableNormal(IStyle style)
            {
                (style as Style).IsSemiHidden = true;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.LeftIndent = 0;
                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Grid.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableGrid(IStyle style)
            {
                #region Paragraph format
                (style as WTableStyle).ParagraphFormat.AfterSpacing = 0;
                (style as WTableStyle).ParagraphFormat.LineSpacing = 12;
                (style as WTableStyle).ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                #endregion

                #region Table Properties
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = 0.5f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = Color.Black;
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = 0.5f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = Color.Black;
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = 0.5f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.Black;
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 0.5f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.Black;
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Horizontal.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Horizontal.LineWidth = 0.5f;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Color = Color.Black;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Vertical.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Vertical.LineWidth = 0.5f;
                (style as WTableStyle).TableProperties.Borders.Vertical.Color = Color.Black;
                (style as WTableStyle).TableProperties.Borders.Vertical.Space = 0;
                #endregion
            }
            /// <summary>
            /// Loads the table style Light Shading.
            /// </summary>
            /// <param name="style">The style.</param>
            /// <param name="textColor">The text Color.</param>
            /// <param name="borderColor">The border Color.</param>
            /// <param name="backColor">The back Color.</param>
            private static void LoadStyleLightShading(IStyle style, Color textColor, Color borderColor, Color backColor)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle, lastColumnStyle, oddColumnBandingStyle, oddRowBandingStyle;
                #region Character format
                (style as WTableStyle).CharacterFormat.TextColor = textColor;
                #endregion

                #region Paragraph format
                (style as WTableStyle).ParagraphFormat.AfterSpacing = 0;
                (style as WTableStyle).ParagraphFormat.LineSpacing = 12;
                (style as WTableStyle).ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                #endregion

                #region Table Properties
                (style as WTableStyle).TableProperties.RowStripe = 1;
                (style as WTableStyle).TableProperties.ColumnStripe = 1;
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Paragraph format
                firstRowStyle.ParagraphFormat.BeforeSpacing = 0;
                firstRowStyle.ParagraphFormat.AfterSpacing = 0;
                firstRowStyle.ParagraphFormat.LineSpacing = 12;
                firstRowStyle.ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                #endregion

                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Top.LineWidth = 1f;
                firstRowStyle.CellProperties.Borders.Top.Color = borderColor;
                firstRowStyle.CellProperties.Borders.Top.Space = 0;

                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = 1f;
                firstRowStyle.CellProperties.Borders.Bottom.Color = borderColor;
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;

                firstRowStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Cleared;
                firstRowStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Cleared;
                firstRowStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;
                firstRowStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Paragraph format
                lastRowStyle.ParagraphFormat.BeforeSpacing = 0;
                lastRowStyle.ParagraphFormat.AfterSpacing = 0;
                lastRowStyle.ParagraphFormat.LineSpacing = 12;
                lastRowStyle.ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                #endregion

                #region Character format
                lastRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowStyle.CharacterFormat.Bold);
                lastRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = 1f;
                lastRowStyle.CellProperties.Borders.Top.Color = borderColor;
                lastRowStyle.CellProperties.Borders.Top.Space = 0;

                lastRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Bottom.LineWidth = 1f;
                lastRowStyle.CellProperties.Borders.Bottom.Color = borderColor;
                lastRowStyle.CellProperties.Borders.Bottom.Space = 0;

                lastRowStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Cleared;
                lastRowStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Cleared;
                lastRowStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;
                lastRowStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Character format
                firstColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, firstColumnStyle.CharacterFormat.Bold);
                firstColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstColumnStyle.CharacterFormat.BoldBidi);
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Character format
                lastColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat,WCharacterFormat.BoldKey,lastColumnStyle.CharacterFormat.Bold);
                lastColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastColumnStyle.CharacterFormat.BoldBidi);
                #endregion
                #endregion

                #region Odd Column Banding Conditional Formatting Style
                oddColumnBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddColumnBanding);
                #region Table Cell Properties
                oddColumnBandingStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Cleared;
                oddColumnBandingStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Cleared;
                oddColumnBandingStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;
                oddColumnBandingStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                oddColumnBandingStyle.CellProperties.BackColor = backColor;
                oddColumnBandingStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                oddColumnBandingStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Odd Row Banding Conditional Formatting Style
                oddRowBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddRowBanding);
                #region Table Cell Properties
                oddRowBandingStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Cleared;
                oddRowBandingStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Cleared;
                oddRowBandingStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;
                oddRowBandingStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                oddRowBandingStyle.CellProperties.BackColor = backColor;
                oddRowBandingStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                oddRowBandingStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Light List.
            /// </summary>
            /// <param name="style">The style.</param>
            /// <param name="borderColor">The border Color.</param>
            /// <param name="backColor">The back Color.</param>
            private static void LoadStyleLightList(IStyle style, Color borderColor, Color backColor)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle, lastColumnStyle, oddColumnBandingStyle, oddRowBandingStyle;
                #region Paragraph format
                (style as WTableStyle).ParagraphFormat.AfterSpacing = 0;
                (style as WTableStyle).ParagraphFormat.LineSpacing = 12;
                (style as WTableStyle).ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                #endregion

                #region Table Properties
                (style as WTableStyle).TableProperties.RowStripe = 1;
                (style as WTableStyle).TableProperties.ColumnStripe = 1;
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Paragraph format
                firstRowStyle.ParagraphFormat.BeforeSpacing = 0;
                firstRowStyle.ParagraphFormat.AfterSpacing = 0;
                firstRowStyle.ParagraphFormat.LineSpacing = 12;
                firstRowStyle.ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                #endregion

                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                firstRowStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.BackColor = backColor;
                firstRowStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                firstRowStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Paragraph format
                lastRowStyle.ParagraphFormat.BeforeSpacing = 0;
                lastRowStyle.ParagraphFormat.AfterSpacing = 0;
                lastRowStyle.ParagraphFormat.LineSpacing = 12;
                lastRowStyle.ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                #endregion

                #region Character format
                lastRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowStyle.CharacterFormat.Bold);
                lastRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Double;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = .75f;
                lastRowStyle.CellProperties.Borders.Top.Color = borderColor;
                lastRowStyle.CellProperties.Borders.Top.Space = 0;

                lastRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Bottom.LineWidth = 1f;
                lastRowStyle.CellProperties.Borders.Bottom.Color = borderColor;
                lastRowStyle.CellProperties.Borders.Bottom.Space = 0;

                lastRowStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Left.LineWidth = 1f;
                lastRowStyle.CellProperties.Borders.Left.Color = borderColor;
                lastRowStyle.CellProperties.Borders.Left.Space = 0;

                lastRowStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Right.LineWidth = 1f;
                lastRowStyle.CellProperties.Borders.Right.Color = borderColor;
                lastRowStyle.CellProperties.Borders.Right.Space = 0;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Character format
                firstColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, firstColumnStyle.CharacterFormat.Bold);
                firstColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstColumnStyle.CharacterFormat.BoldBidi);
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Character format
                lastColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, lastColumnStyle.CharacterFormat.Bold);
                lastColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastColumnStyle.CharacterFormat.BoldBidi);
                #endregion
                #endregion

                #region Odd Column Banding Conditional Formatting Style
                oddColumnBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddColumnBanding);
                #region Table Cell Properties
                oddColumnBandingStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                oddColumnBandingStyle.CellProperties.Borders.Top.LineWidth = 1f;
                oddColumnBandingStyle.CellProperties.Borders.Top.Color = borderColor;
                oddColumnBandingStyle.CellProperties.Borders.Top.Space = 0;

                oddColumnBandingStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                oddColumnBandingStyle.CellProperties.Borders.Bottom.LineWidth = 1f;
                oddColumnBandingStyle.CellProperties.Borders.Bottom.Color = borderColor;
                oddColumnBandingStyle.CellProperties.Borders.Bottom.Space = 0;

                oddColumnBandingStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Single;
                oddColumnBandingStyle.CellProperties.Borders.Left.LineWidth = 1f;
                oddColumnBandingStyle.CellProperties.Borders.Left.Color = borderColor;
                oddColumnBandingStyle.CellProperties.Borders.Left.Space = 0;

                oddColumnBandingStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Single;
                oddColumnBandingStyle.CellProperties.Borders.Right.LineWidth = 1f;
                oddColumnBandingStyle.CellProperties.Borders.Right.Color = borderColor;
                oddColumnBandingStyle.CellProperties.Borders.Right.Space = 0;
                #endregion
                #endregion

                #region Odd Row Banding Conditional Formatting Style
                oddRowBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddRowBanding);
                #region Table Cell Properties
                oddRowBandingStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                oddRowBandingStyle.CellProperties.Borders.Top.LineWidth = 1f;
                oddRowBandingStyle.CellProperties.Borders.Top.Color = borderColor;
                oddRowBandingStyle.CellProperties.Borders.Top.Space = 0;

                oddRowBandingStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                oddRowBandingStyle.CellProperties.Borders.Bottom.LineWidth = 1f;
                oddRowBandingStyle.CellProperties.Borders.Bottom.Color = borderColor;
                oddRowBandingStyle.CellProperties.Borders.Bottom.Space = 0;

                oddRowBandingStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Single;
                oddRowBandingStyle.CellProperties.Borders.Left.LineWidth = 1f;
                oddRowBandingStyle.CellProperties.Borders.Left.Color = borderColor;
                oddRowBandingStyle.CellProperties.Borders.Left.Space = 0;

                oddRowBandingStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Single;
                oddRowBandingStyle.CellProperties.Borders.Right.LineWidth = 1f;
                oddRowBandingStyle.CellProperties.Borders.Right.Color = borderColor;
                oddRowBandingStyle.CellProperties.Borders.Right.Space = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Light Grid.
            /// </summary>
            /// <param name="style">The style.</param>
            /// <param name="borderColor">The border Color.</param>
            /// <param name="backColor">The back Color.</param>
            private static void LoadStyleLightGrid(IStyle style, Color borderColor, Color backColor)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle, lastColumnStyle, oddColumnBandingStyle, oddRowBandingStyle, evenRowBandingStyle;
                #region Paragraph format
                (style as WTableStyle).ParagraphFormat.AfterSpacing = 0;
                (style as WTableStyle).ParagraphFormat.LineSpacing = 12;
                (style as WTableStyle).ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                #endregion

                #region Table Properties
                (style as WTableStyle).TableProperties.RowStripe = 1;
                (style as WTableStyle).TableProperties.ColumnStripe = 1;
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Horizontal.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Horizontal.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Vertical.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Vertical.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Vertical.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Vertical.Space = 0;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Paragraph format
                firstRowStyle.ParagraphFormat.BeforeSpacing = 0;
                firstRowStyle.ParagraphFormat.AfterSpacing = 0;
                firstRowStyle.ParagraphFormat.LineSpacing = 12;
                firstRowStyle.ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                #endregion

                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Top.LineWidth = 1f;
                firstRowStyle.CellProperties.Borders.Top.Color = borderColor;
                firstRowStyle.CellProperties.Borders.Top.Space = 0;

                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = 2.25f;
                firstRowStyle.CellProperties.Borders.Bottom.Color = borderColor;
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;

                firstRowStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Left.LineWidth = 1f;
                firstRowStyle.CellProperties.Borders.Left.Color = borderColor;
                firstRowStyle.CellProperties.Borders.Left.Space = 0;

                firstRowStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Right.LineWidth = 1f;
                firstRowStyle.CellProperties.Borders.Right.Color = borderColor;
                firstRowStyle.CellProperties.Borders.Right.Space = 0;

                firstRowStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;

                firstRowStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Vertical.LineWidth = 1f;
                firstRowStyle.CellProperties.Borders.Vertical.Color = borderColor;
                firstRowStyle.CellProperties.Borders.Vertical.Space = 0;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Paragraph format
                lastRowStyle.ParagraphFormat.BeforeSpacing = 0;
                lastRowStyle.ParagraphFormat.AfterSpacing = 0;
                lastRowStyle.ParagraphFormat.LineSpacing = 12;
                lastRowStyle.ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                #endregion

                #region Character format
                lastRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowStyle.CharacterFormat.Bold);
                lastRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Double;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = .75f;
                lastRowStyle.CellProperties.Borders.Top.Color = borderColor;
                lastRowStyle.CellProperties.Borders.Top.Space = 0;

                lastRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Bottom.LineWidth = 1f;
                lastRowStyle.CellProperties.Borders.Bottom.Color = borderColor;
                lastRowStyle.CellProperties.Borders.Bottom.Space = 0;

                lastRowStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Left.LineWidth = 1f;
                lastRowStyle.CellProperties.Borders.Left.Color = borderColor;
                lastRowStyle.CellProperties.Borders.Left.Space = 0;

                lastRowStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Right.LineWidth = 1f;
                lastRowStyle.CellProperties.Borders.Right.Color = borderColor;
                lastRowStyle.CellProperties.Borders.Right.Space = 0;

                lastRowStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;

                lastRowStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Vertical.LineWidth = 1f;
                lastRowStyle.CellProperties.Borders.Vertical.Color = borderColor;
                lastRowStyle.CellProperties.Borders.Vertical.Space = 0;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Character format
                firstColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, firstColumnStyle.CharacterFormat.Bold);
                firstColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstColumnStyle.CharacterFormat.BoldBidi);
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Character format
                lastColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, lastColumnStyle.CharacterFormat.Bold);
                lastColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastColumnStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastColumnStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                lastColumnStyle.CellProperties.Borders.Top.LineWidth = 1f;
                lastColumnStyle.CellProperties.Borders.Top.Color = borderColor;
                lastColumnStyle.CellProperties.Borders.Top.Space = 0;

                lastColumnStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                lastColumnStyle.CellProperties.Borders.Bottom.LineWidth = 1f;
                lastColumnStyle.CellProperties.Borders.Bottom.Color = borderColor;
                lastColumnStyle.CellProperties.Borders.Bottom.Space = 0;

                lastColumnStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Single;
                lastColumnStyle.CellProperties.Borders.Left.LineWidth = 1f;
                lastColumnStyle.CellProperties.Borders.Left.Color = borderColor;
                lastColumnStyle.CellProperties.Borders.Left.Space = 0;

                lastColumnStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Single;
                lastColumnStyle.CellProperties.Borders.Right.LineWidth = 1f;
                lastColumnStyle.CellProperties.Borders.Right.Color = borderColor;
                lastColumnStyle.CellProperties.Borders.Right.Space = 0;
                #endregion
                #endregion

                #region Odd Column Banding Conditional Formatting Style
                oddColumnBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddColumnBanding);
                #region Table Cell Properties
                oddColumnBandingStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                oddColumnBandingStyle.CellProperties.Borders.Top.LineWidth = 1f;
                oddColumnBandingStyle.CellProperties.Borders.Top.Color = borderColor;
                oddColumnBandingStyle.CellProperties.Borders.Top.Space = 0;

                oddColumnBandingStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                oddColumnBandingStyle.CellProperties.Borders.Bottom.LineWidth = 1f;
                oddColumnBandingStyle.CellProperties.Borders.Bottom.Color = borderColor;
                oddColumnBandingStyle.CellProperties.Borders.Bottom.Space = 0;

                oddColumnBandingStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Single;
                oddColumnBandingStyle.CellProperties.Borders.Left.LineWidth = 1f;
                oddColumnBandingStyle.CellProperties.Borders.Left.Color = borderColor;
                oddColumnBandingStyle.CellProperties.Borders.Left.Space = 0;

                oddColumnBandingStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Single;
                oddColumnBandingStyle.CellProperties.Borders.Right.LineWidth = 1f;
                oddColumnBandingStyle.CellProperties.Borders.Right.Color = borderColor;
                oddColumnBandingStyle.CellProperties.Borders.Right.Space = 0;

                oddColumnBandingStyle.CellProperties.BackColor = backColor;
                oddColumnBandingStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                oddColumnBandingStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Odd Row Banding Conditional Formatting Style
                oddRowBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddRowBanding);
                #region Table Cell Properties
                oddRowBandingStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                oddRowBandingStyle.CellProperties.Borders.Top.LineWidth = 1f;
                oddRowBandingStyle.CellProperties.Borders.Top.Color = borderColor;
                oddRowBandingStyle.CellProperties.Borders.Top.Space = 0;

                oddRowBandingStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                oddRowBandingStyle.CellProperties.Borders.Bottom.LineWidth = 1f;
                oddRowBandingStyle.CellProperties.Borders.Bottom.Color = borderColor;
                oddRowBandingStyle.CellProperties.Borders.Bottom.Space = 0;

                oddRowBandingStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Single;
                oddRowBandingStyle.CellProperties.Borders.Left.LineWidth = 1f;
                oddRowBandingStyle.CellProperties.Borders.Left.Color = borderColor;
                oddRowBandingStyle.CellProperties.Borders.Left.Space = 0;

                oddRowBandingStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Single;
                oddRowBandingStyle.CellProperties.Borders.Right.LineWidth = 1f;
                oddRowBandingStyle.CellProperties.Borders.Right.Color = borderColor;
                oddRowBandingStyle.CellProperties.Borders.Right.Space = 0;

                oddRowBandingStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Single;
                oddRowBandingStyle.CellProperties.Borders.Vertical.LineWidth = 1f;
                oddRowBandingStyle.CellProperties.Borders.Vertical.Color = borderColor;
                oddRowBandingStyle.CellProperties.Borders.Vertical.Space = 0;

                oddRowBandingStyle.CellProperties.BackColor = backColor;
                oddRowBandingStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                oddRowBandingStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Even Row Banding Conditional Formatting Style
                evenRowBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.EvenRowBanding);
                #region Table Cell Properties
                evenRowBandingStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                evenRowBandingStyle.CellProperties.Borders.Top.LineWidth = 1f;
                evenRowBandingStyle.CellProperties.Borders.Top.Color = borderColor;
                evenRowBandingStyle.CellProperties.Borders.Top.Space = 0;

                evenRowBandingStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                evenRowBandingStyle.CellProperties.Borders.Bottom.LineWidth = 1f;
                evenRowBandingStyle.CellProperties.Borders.Bottom.Color = borderColor;
                evenRowBandingStyle.CellProperties.Borders.Bottom.Space = 0;

                evenRowBandingStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Single;
                evenRowBandingStyle.CellProperties.Borders.Left.LineWidth = 1f;
                evenRowBandingStyle.CellProperties.Borders.Left.Color = borderColor;
                evenRowBandingStyle.CellProperties.Borders.Left.Space = 0;

                evenRowBandingStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Single;
                evenRowBandingStyle.CellProperties.Borders.Right.LineWidth = 1f;
                evenRowBandingStyle.CellProperties.Borders.Right.Color = borderColor;
                evenRowBandingStyle.CellProperties.Borders.Right.Space = 0;

                evenRowBandingStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Single;
                evenRowBandingStyle.CellProperties.Borders.Vertical.LineWidth = 1f;
                evenRowBandingStyle.CellProperties.Borders.Vertical.Color = borderColor;
                evenRowBandingStyle.CellProperties.Borders.Vertical.Space = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Medium Shading 1.
            /// </summary>
            /// <param name="style">The style.</param>
            /// <param name="borderColor">The border Color.</param>
            /// <param name="firstRowBackColor">The first Row Back Color.</param>
            /// <param name="backColor">The back Color.</param>
            private static void LoadStyleMediumShading1(IStyle style, Color borderColor, Color firstRowBackColor, Color backColor)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle, lastColumnStyle, oddColumnBandingStyle, oddRowBandingStyle, evenRowBandingStyle;
                #region Paragraph format
                (style as WTableStyle).ParagraphFormat.AfterSpacing = 0;
                (style as WTableStyle).ParagraphFormat.LineSpacing = 12;
                (style as WTableStyle).ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                #endregion

                #region Table Properties
                (style as WTableStyle).TableProperties.RowStripe = 1;
                (style as WTableStyle).TableProperties.ColumnStripe = 1;
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Horizontal.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Horizontal.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Space = 0;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Paragraph format
                firstRowStyle.ParagraphFormat.BeforeSpacing = 0;
                firstRowStyle.ParagraphFormat.AfterSpacing = 0;
                firstRowStyle.ParagraphFormat.LineSpacing = 12;
                firstRowStyle.ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                #endregion

                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                firstRowStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Top.LineWidth = 1f;
                firstRowStyle.CellProperties.Borders.Top.Color = borderColor;
                firstRowStyle.CellProperties.Borders.Top.Space = 0;

                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = 1f;
                firstRowStyle.CellProperties.Borders.Bottom.Color = borderColor;
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;

                firstRowStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Left.LineWidth = 1f;
                firstRowStyle.CellProperties.Borders.Left.Color = borderColor;
                firstRowStyle.CellProperties.Borders.Left.Space = 0;

                firstRowStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Right.LineWidth = 1f;
                firstRowStyle.CellProperties.Borders.Right.Color = borderColor;
                firstRowStyle.CellProperties.Borders.Right.Space = 0;

                firstRowStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;

                firstRowStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                firstRowStyle.CellProperties.BackColor = firstRowBackColor;
                firstRowStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                firstRowStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Paragraph format
                lastRowStyle.ParagraphFormat.BeforeSpacing = 0;
                lastRowStyle.ParagraphFormat.AfterSpacing = 0;
                lastRowStyle.ParagraphFormat.LineSpacing = 12;
                lastRowStyle.ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                #endregion

                #region Character format
                lastRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowStyle.CharacterFormat.Bold);
                lastRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Double;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = .75f;
                lastRowStyle.CellProperties.Borders.Top.Color = borderColor;
                lastRowStyle.CellProperties.Borders.Top.Space = 0;

                lastRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Bottom.LineWidth = 1f;
                lastRowStyle.CellProperties.Borders.Bottom.Color = borderColor;
                lastRowStyle.CellProperties.Borders.Bottom.Space = 0;

                lastRowStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Left.LineWidth = 1f;
                lastRowStyle.CellProperties.Borders.Left.Color = borderColor;
                lastRowStyle.CellProperties.Borders.Left.Space = 0;

                lastRowStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Right.LineWidth = 1f;
                lastRowStyle.CellProperties.Borders.Right.Color = borderColor;
                lastRowStyle.CellProperties.Borders.Right.Space = 0;

                lastRowStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;

                lastRowStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Character format
                firstColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, firstColumnStyle.CharacterFormat.Bold);
                firstColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstColumnStyle.CharacterFormat.BoldBidi);
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Character format
                lastColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, lastColumnStyle.CharacterFormat.Bold);
                lastColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastColumnStyle.CharacterFormat.BoldBidi);
                #endregion
                #endregion

                #region Odd Column Banding Conditional Formatting Style
                oddColumnBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddColumnBanding);
                #region Table Cell Properties
                oddColumnBandingStyle.CellProperties.BackColor = backColor;
                oddColumnBandingStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                oddColumnBandingStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Odd Row Banding Conditional Formatting Style
                oddRowBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddRowBanding);
                #region Table Cell Properties
                oddRowBandingStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;

                oddRowBandingStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                oddRowBandingStyle.CellProperties.BackColor = backColor;
                oddRowBandingStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                oddRowBandingStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Even Row Banding Conditional Formatting Style
                evenRowBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.EvenRowBanding);
                #region Table Cell Properties
                evenRowBandingStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;

                evenRowBandingStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Medium Shading 2.
            /// </summary>
            /// <param name="style">The style.</param>
            /// <param name="backColor">The back Color.</param>
            private static void LoadStyleMediumShading2(IStyle style, Color backColor)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle, lastColumnStyle, oddColumnBandingStyle, oddRowBandingStyle, firstRowLastCellStyle, firstRowFirstCellStyle;
                #region Paragraph format
                (style as WTableStyle).ParagraphFormat.AfterSpacing = 0;
                (style as WTableStyle).ParagraphFormat.LineSpacing = 12;
                (style as WTableStyle).ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                #endregion

                #region Table Properties
                (style as WTableStyle).TableProperties.RowStripe = 1;
                (style as WTableStyle).TableProperties.ColumnStripe = 1;
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = 2.25f;
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 2.25f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Paragraph format
                firstRowStyle.ParagraphFormat.BeforeSpacing = 0;
                firstRowStyle.ParagraphFormat.AfterSpacing = 0;
                firstRowStyle.ParagraphFormat.LineSpacing = 12;
                firstRowStyle.ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                #endregion

                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                firstRowStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Top.LineWidth = 2.25f;
                firstRowStyle.CellProperties.Borders.Top.Space = 0;

                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = 2.25f;
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;

                firstRowStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Cleared;

                firstRowStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Cleared;

                firstRowStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;

                firstRowStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                firstRowStyle.CellProperties.BackColor = backColor;
                firstRowStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                firstRowStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Paragraph format
                lastRowStyle.ParagraphFormat.BeforeSpacing = 0;
                lastRowStyle.ParagraphFormat.AfterSpacing = 0;
                lastRowStyle.ParagraphFormat.LineSpacing = 12;
                lastRowStyle.ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                #endregion

                #region Character format
                lastRowStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Double;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = .75f;
                lastRowStyle.CellProperties.Borders.Top.Space = 0;

                lastRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Bottom.LineWidth = 2.25f;
                lastRowStyle.CellProperties.Borders.Bottom.Space = 0;

                lastRowStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Cleared;

                lastRowStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Cleared;

                lastRowStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;

                lastRowStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                lastRowStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                lastRowStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                lastRowStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Character format
                firstColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, firstColumnStyle.CharacterFormat.Bold);
                firstColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstColumnStyle.CharacterFormat.BoldBidi);
                firstColumnStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
                #endregion

                #region Table Cell Properties
                firstColumnStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Cleared;

                firstColumnStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstColumnStyle.CellProperties.Borders.Bottom.LineWidth = 2.25f;
                firstColumnStyle.CellProperties.Borders.Bottom.Space = 0;

                firstColumnStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Cleared;

                firstColumnStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Cleared;

                firstColumnStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;

                firstColumnStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                firstColumnStyle.CellProperties.BackColor = backColor;
                firstColumnStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                firstColumnStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Character format
                lastColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, lastColumnStyle.CharacterFormat.Bold);
                lastColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastColumnStyle.CharacterFormat.BoldBidi);
                lastColumnStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
                #endregion

                #region Table Cell Properties
                lastColumnStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Cleared;

                lastColumnStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Cleared;

                lastColumnStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;

                lastColumnStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                lastColumnStyle.CellProperties.BackColor = backColor;
                lastColumnStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                lastColumnStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Odd Column Banding Conditional Formatting Style
                oddColumnBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddColumnBanding);
                #region Table Cell Properties
                oddColumnBandingStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Cleared;

                oddColumnBandingStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Cleared;

                oddColumnBandingStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;

                oddColumnBandingStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                oddColumnBandingStyle.CellProperties.BackColor = Color.FromArgb(255, 216, 216, 216);
                oddColumnBandingStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                oddColumnBandingStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Odd Row Banding Conditional Formatting Style
                oddRowBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddRowBanding);
                #region Table Cell Properties
                oddRowBandingStyle.CellProperties.BackColor = Color.FromArgb(255, 216, 216, 216);
                oddRowBandingStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                oddRowBandingStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region First Row Last Cell Conditional Formatting Style
                firstRowLastCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRowLastCell);
                #region Table Cell Properties
                firstRowLastCellStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                firstRowLastCellStyle.CellProperties.Borders.Top.LineWidth = 2.25f;
                firstRowLastCellStyle.CellProperties.Borders.Top.Space = 0;

                firstRowLastCellStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowLastCellStyle.CellProperties.Borders.Bottom.LineWidth = 2.25f;
                firstRowLastCellStyle.CellProperties.Borders.Bottom.Space = 0;

                firstRowLastCellStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Cleared;

                firstRowLastCellStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Cleared;

                firstRowLastCellStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;

                firstRowLastCellStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;
                #endregion
                #endregion

                #region First Row First Cell Conditional Formatting Style
                firstRowFirstCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRowFirstCell);
                #region Character format
                firstRowFirstCellStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
                #endregion

                #region Table Cell Properties
                firstRowFirstCellStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                firstRowFirstCellStyle.CellProperties.Borders.Top.LineWidth = 2.25f;
                firstRowFirstCellStyle.CellProperties.Borders.Top.Space = 0;

                firstRowFirstCellStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowFirstCellStyle.CellProperties.Borders.Bottom.LineWidth = 2.25f;
                firstRowFirstCellStyle.CellProperties.Borders.Bottom.Space = 0;

                firstRowFirstCellStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Cleared;

                firstRowFirstCellStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Cleared;

                firstRowFirstCellStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;

                firstRowFirstCellStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Medium List 1.
            /// </summary>
            /// <param name="style">The style.</param>
            /// <param name="borderColor">The border Color.</param>
            /// <param name="backColor">The back Color.</param>
            private static void LoadStyleMediumList1(IStyle style, Color borderColor, Color backColor)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle, lastColumnStyle, oddColumnBandingStyle, oddRowBandingStyle;
                #region Character format
                (style as WTableStyle).CharacterFormat.TextColor = Color.Black;
                #endregion

                #region Paragraph format
                (style as WTableStyle).ParagraphFormat.AfterSpacing = 0;
                (style as WTableStyle).ParagraphFormat.LineSpacing = 12;
                (style as WTableStyle).ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                #endregion

                #region Table Properties
                (style as WTableStyle).TableProperties.RowStripe = 1;
                (style as WTableStyle).TableProperties.ColumnStripe = 1;
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                //Font theme
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Cleared;
                
                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = 1f;
                firstRowStyle.CellProperties.Borders.Bottom.Color = borderColor;
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Character format
                lastRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowStyle.CharacterFormat.Bold);
                lastRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowStyle.CharacterFormat.BoldBidi);
                lastRowStyle.CharacterFormat.TextColor = Color.FromArgb(255, 31, 73, 125);
                #endregion

                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = 1f;
                lastRowStyle.CellProperties.Borders.Top.Color = borderColor;
                lastRowStyle.CellProperties.Borders.Top.Space = 0;

                lastRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Bottom.LineWidth = 1f;
                lastRowStyle.CellProperties.Borders.Bottom.Color = borderColor;
                lastRowStyle.CellProperties.Borders.Bottom.Space = 0;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Character format
                firstColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, firstColumnStyle.CharacterFormat.Bold);
                firstColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstColumnStyle.CharacterFormat.BoldBidi);
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Character format
                lastColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, lastColumnStyle.CharacterFormat.Bold);
                lastColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastColumnStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastColumnStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                lastColumnStyle.CellProperties.Borders.Top.LineWidth = 1f;
                lastColumnStyle.CellProperties.Borders.Top.Color = borderColor;
                lastColumnStyle.CellProperties.Borders.Top.Space = 0;

                lastColumnStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                lastColumnStyle.CellProperties.Borders.Bottom.LineWidth = 1f;
                lastColumnStyle.CellProperties.Borders.Bottom.Color = borderColor;
                lastColumnStyle.CellProperties.Borders.Bottom.Space = 0;
                #endregion
                #endregion

                #region Odd Column Banding Conditional Formatting Style
                oddColumnBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddColumnBanding);
                #region Table Cell Properties
                oddColumnBandingStyle.CellProperties.BackColor = backColor;
                oddColumnBandingStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                oddColumnBandingStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Odd Row Banding Conditional Formatting Style
                oddRowBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddRowBanding);
                #region Table Cell Properties
                oddRowBandingStyle.CellProperties.BackColor = backColor;
                oddRowBandingStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                oddRowBandingStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Medium List 2.
            /// </summary>
            /// <param name="style">The style.</param>
            /// <param name="borderColor">The border Color.</param>
            /// <param name="backColor">The back Color.</param>
            private static void LoadStyleMediumList2(IStyle style, Color borderColor, Color backColor)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle, lastColumnStyle, oddColumnBandingStyle, oddRowBandingStyle, firstRowFirstCellStyle, lastRowFirstCellStyle;
                #region Character format
                (style as WTableStyle).CharacterFormat.TextColor = Color.Black;
                #endregion

                #region Paragraph format
                (style as WTableStyle).ParagraphFormat.AfterSpacing = 0;
                (style as WTableStyle).ParagraphFormat.LineSpacing = 12;
                (style as WTableStyle).ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                #endregion

                #region Table Properties
                (style as WTableStyle).TableProperties.RowStripe = 1;
                (style as WTableStyle).TableProperties.ColumnStripe = 1;
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.FontSize = 12;
                firstRowStyle.CharacterFormat.FontSizeBidi = 12;
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Cleared;

                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = 3f;
                firstRowStyle.CellProperties.Borders.Bottom.Color = borderColor;
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;

                firstRowStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Cleared;
                firstRowStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Cleared;
                firstRowStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;
                firstRowStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                firstRowStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstRowStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                firstRowStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = 1f;
                lastRowStyle.CellProperties.Borders.Top.Color = borderColor;
                lastRowStyle.CellProperties.Borders.Top.Space = 0;

                lastRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Cleared;
                lastRowStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Cleared;
                lastRowStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Cleared;
                lastRowStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;
                lastRowStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                lastRowStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                lastRowStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                lastRowStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Table Cell Properties
                firstColumnStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Cleared;
                firstColumnStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Cleared;
                firstColumnStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Cleared;

                firstColumnStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Single;
                firstColumnStyle.CellProperties.Borders.Right.LineWidth = 1f;
                firstColumnStyle.CellProperties.Borders.Right.Color = borderColor;
                firstColumnStyle.CellProperties.Borders.Right.Space = 0;

                firstColumnStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;
                firstColumnStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                firstColumnStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstColumnStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                firstColumnStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Table Cell Properties
                lastColumnStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Cleared;
                lastColumnStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Cleared;

                lastColumnStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Single;
                lastColumnStyle.CellProperties.Borders.Left.LineWidth = 1f;
                lastColumnStyle.CellProperties.Borders.Left.Color = borderColor;
                lastColumnStyle.CellProperties.Borders.Left.Space = 0;

                lastColumnStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Cleared;
                lastColumnStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;
                lastColumnStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                lastColumnStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                lastColumnStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                lastColumnStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Odd Column Banding Conditional Formatting Style
                oddColumnBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddColumnBanding);
                #region Table Cell Properties
                oddColumnBandingStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Cleared;
                oddColumnBandingStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Cleared;
                oddColumnBandingStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;
                oddColumnBandingStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                oddColumnBandingStyle.CellProperties.BackColor = backColor;
                oddColumnBandingStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                oddColumnBandingStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Odd Row Banding Conditional Formatting Style
                oddRowBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddRowBanding);
                #region Table Cell Properties
                oddRowBandingStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Cleared;
                oddRowBandingStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Cleared;
                oddRowBandingStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;
                oddRowBandingStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                oddRowBandingStyle.CellProperties.BackColor = backColor;
                oddRowBandingStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                oddRowBandingStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region First Row First Cell Conditional Formatting Style
                firstRowFirstCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRowFirstCell);
                #region Table Cell Properties
                firstRowFirstCellStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstRowFirstCellStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                firstRowFirstCellStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Last Row First Cell Conditional Formatting Style
                lastRowFirstCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRowFirstCell);
                #region Table Cell Properties
                lastRowFirstCellStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Cleared;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Medium Grid 1.
            /// </summary>
            /// <param name="style">The style.</param>
            /// <param name="borderColor">The border Color.</param>
            /// <param name="backColor">The back Color.</param>
            /// <param name="bandCellColor">The band Cell Color.</param>
            private static void LoadStyleMediumGrid1(IStyle style, Color borderColor, Color backColor, Color bandCellColor)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle, lastColumnStyle, oddColumnBandingStyle, oddRowBandingStyle;
                #region Paragraph format
                (style as WTableStyle).ParagraphFormat.AfterSpacing = 0;
                (style as WTableStyle).ParagraphFormat.LineSpacing = 12;
                (style as WTableStyle).ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                #endregion

                #region Table Properties
                (style as WTableStyle).TableProperties.RowStripe = 1;
                (style as WTableStyle).TableProperties.ColumnStripe = 1;
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Horizontal.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Horizontal.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Vertical.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Vertical.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Vertical.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Vertical.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = backColor;
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Character format
                lastRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowStyle.CharacterFormat.Bold);
                lastRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = 2.25f;
                lastRowStyle.CellProperties.Borders.Top.Color = borderColor;
                lastRowStyle.CellProperties.Borders.Top.Space = 0;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Character format
                firstColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, firstColumnStyle.CharacterFormat.Bold);
                firstColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstColumnStyle.CharacterFormat.BoldBidi);
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Character format
                lastColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, lastColumnStyle.CharacterFormat.Bold);
                lastColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastColumnStyle.CharacterFormat.BoldBidi);
                #endregion
                #endregion

                #region Odd Column Banding Conditional Formatting Style
                oddColumnBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddColumnBanding);
                #region Table Cell Properties
                oddColumnBandingStyle.CellProperties.BackColor = bandCellColor;
                oddColumnBandingStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                oddColumnBandingStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Odd Row Banding Conditional Formatting Style
                oddRowBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddRowBanding);
                #region Table Cell Properties
                oddRowBandingStyle.CellProperties.BackColor = bandCellColor;
                oddRowBandingStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                oddRowBandingStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Medium Grid 2.
            /// </summary>
            /// <param name="style">The style.</param>
            /// <param name="borderColor">The border Color.</param>
            /// <param name="backColor">The back Color.</param>
            /// <param name="firstRowColor">The first Row Color.</param>
            /// <param name="lastColumnColor">The last Column Color.</param>
            /// <param name="bandCellColor">The band Cell Color.</param>
            private static void LoadStyleMediumGrid2(IStyle style, Color borderColor, Color backColor, Color firstRowColor, Color lastColumnColor, Color bandCellColor)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle, lastColumnStyle, oddColumnBandingStyle, oddRowBandingStyle, firstRowFirstCellStyle;
                #region Character format
                (style as WTableStyle).CharacterFormat.TextColor = Color.Black;
                #endregion

                #region Paragraph format
                (style as WTableStyle).ParagraphFormat.AfterSpacing = 0;
                (style as WTableStyle).ParagraphFormat.LineSpacing = 12;
                (style as WTableStyle).ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                #endregion

                #region Table Properties
                (style as WTableStyle).TableProperties.RowStripe = 1;
                (style as WTableStyle).TableProperties.ColumnStripe = 1;
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Horizontal.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Horizontal.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Vertical.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Vertical.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Vertical.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Vertical.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = backColor;
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                firstRowStyle.CharacterFormat.TextColor = Color.Black;
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.BackColor = firstRowColor;
                firstRowStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                firstRowStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Character format
                lastRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowStyle.CharacterFormat.Bold);
                lastRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowStyle.CharacterFormat.BoldBidi);
                lastRowStyle.CharacterFormat.TextColor = Color.Black;
                #endregion

                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = 1.5f;
                lastRowStyle.CellProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                lastRowStyle.CellProperties.Borders.Top.Space = 0;

                lastRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Cleared;
                lastRowStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Cleared;
                lastRowStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Cleared;
                lastRowStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;
                lastRowStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                lastRowStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                lastRowStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                lastRowStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Character format
                firstColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, firstColumnStyle.CharacterFormat.Bold);
                firstColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstColumnStyle.CharacterFormat.BoldBidi);
                firstColumnStyle.CharacterFormat.TextColor = Color.Black;
                #endregion

                #region Table Cell Properties
                firstColumnStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Cleared;
                firstColumnStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Cleared;
                firstColumnStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Cleared;
                firstColumnStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Cleared;
                firstColumnStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;
                firstColumnStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                firstColumnStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstColumnStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                firstColumnStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Character format
                lastColumnStyle.CharacterFormat.Bold = false;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, lastColumnStyle.CharacterFormat.Bold);
                lastColumnStyle.CharacterFormat.BoldBidi = false;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastColumnStyle.CharacterFormat.BoldBidi);
                lastColumnStyle.CharacterFormat.TextColor = Color.Black;
                #endregion

                #region Table Cell Properties
                lastColumnStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Cleared;
                lastColumnStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Cleared;
                lastColumnStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Cleared;
                lastColumnStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Cleared;
                lastColumnStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;
                lastColumnStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                lastColumnStyle.CellProperties.BackColor = lastColumnColor;
                lastColumnStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                lastColumnStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Odd Column Banding Conditional Formatting Style
                oddColumnBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddColumnBanding);
                #region Table Cell Properties
                oddColumnBandingStyle.CellProperties.BackColor = bandCellColor;
                oddColumnBandingStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                oddColumnBandingStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Odd Row Banding Conditional Formatting Style
                oddRowBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddRowBanding);
                #region Table Cell Properties
                oddRowBandingStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Single;
                oddRowBandingStyle.CellProperties.Borders.Horizontal.LineWidth = .75f;
                oddRowBandingStyle.CellProperties.Borders.Horizontal.Color = borderColor;
                oddRowBandingStyle.CellProperties.Borders.Horizontal.Space = 0;

                oddRowBandingStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Single;
                oddRowBandingStyle.CellProperties.Borders.Vertical.LineWidth = .75f;
                oddRowBandingStyle.CellProperties.Borders.Vertical.Color = borderColor;
                oddRowBandingStyle.CellProperties.Borders.Vertical.Space = 0;

                oddRowBandingStyle.CellProperties.BackColor = bandCellColor;
                oddRowBandingStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                oddRowBandingStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region First Row First Cell Conditional Formatting Style
                firstRowFirstCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRowFirstCell);
                #region Table Cell Properties
                firstRowFirstCellStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstRowFirstCellStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                firstRowFirstCellStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Medium Grid 3.
            /// </summary>
            /// <param name="style">The style.</param>
            /// <param name="backColor">The back Color.</param>
            /// <param name="firstRowColor">The first Row Color.</param>
            /// <param name="bandCellColor">The band Cell Color.</param>
            private static void LoadStyleMediumGrid3(IStyle style, Color backColor, Color firstRowColor, Color bandCellColor)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle, lastColumnStyle, oddColumnBandingStyle, oddRowBandingStyle;
                #region Paragraph format
                (style as WTableStyle).ParagraphFormat.AfterSpacing = 0;
                (style as WTableStyle).ParagraphFormat.LineSpacing = 12;
                (style as WTableStyle).ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                #endregion

                #region Table Properties
                (style as WTableStyle).TableProperties.RowStripe = 1;
                (style as WTableStyle).TableProperties.ColumnStripe = 1;
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.FromArgb(255, 255, 255, 255);
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.FromArgb(255, 255, 255, 255);
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = Color.FromArgb(255, 255, 255, 255);
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = 1f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = Color.FromArgb(255, 255, 255, 255);
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Horizontal.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Horizontal.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Color = Color.FromArgb(255, 255, 255, 255);
                (style as WTableStyle).TableProperties.Borders.Horizontal.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Vertical.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Vertical.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Vertical.Color = Color.FromArgb(255, 255, 255, 255);
                (style as WTableStyle).TableProperties.Borders.Vertical.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = backColor;
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                firstRowStyle.CharacterFormat.Italic = false;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.ItalicKey, firstRowStyle.CharacterFormat.Italic);
                firstRowStyle.CharacterFormat.ItalicBidi = false;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.ItalicBidiKey, firstRowStyle.CharacterFormat.ItalicBidi);
                firstRowStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Top.LineWidth = 1f;
                firstRowStyle.CellProperties.Borders.Top.Color = Color.FromArgb(255, 255, 255, 255);
                firstRowStyle.CellProperties.Borders.Top.Space = 0;

                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = 3f;
                firstRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 255, 255, 255);
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;

                firstRowStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Left.LineWidth = 1f;
                firstRowStyle.CellProperties.Borders.Left.Color = Color.FromArgb(255, 255, 255, 255);
                firstRowStyle.CellProperties.Borders.Left.Space = 0;

                firstRowStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Right.LineWidth = 1f;
                firstRowStyle.CellProperties.Borders.Right.Color = Color.FromArgb(255, 255, 255, 255);
                firstRowStyle.CellProperties.Borders.Right.Space = 0;

                firstRowStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;
                
                firstRowStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Vertical.LineWidth = 1f;
                firstRowStyle.CellProperties.Borders.Vertical.Color = Color.FromArgb(255, 255, 255, 255);
                firstRowStyle.CellProperties.Borders.Vertical.Space = 0;

                firstRowStyle.CellProperties.BackColor = firstRowColor;
                firstRowStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                firstRowStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Character format
                lastRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowStyle.CharacterFormat.Bold);
                lastRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowStyle.CharacterFormat.BoldBidi);
                lastRowStyle.CharacterFormat.Italic = false;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.ItalicKey, lastRowStyle.CharacterFormat.Italic);
                lastRowStyle.CharacterFormat.ItalicBidi = false;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.ItalicBidiKey, lastRowStyle.CharacterFormat.ItalicBidi);
                lastRowStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
                #endregion

                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = 3f;
                lastRowStyle.CellProperties.Borders.Top.Color = Color.FromArgb(255, 255, 255, 255);
                lastRowStyle.CellProperties.Borders.Top.Space = 0;

                lastRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Bottom.LineWidth = 1f;
                lastRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 255, 255, 255);
                lastRowStyle.CellProperties.Borders.Bottom.Space = 0;

                lastRowStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Left.LineWidth = 1f;
                lastRowStyle.CellProperties.Borders.Left.Color = Color.FromArgb(255, 255, 255, 255);
                lastRowStyle.CellProperties.Borders.Left.Space = 0;

                lastRowStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Right.LineWidth = 1f;
                lastRowStyle.CellProperties.Borders.Right.Color = Color.FromArgb(255, 255, 255, 255);
                lastRowStyle.CellProperties.Borders.Right.Space = 0;
                
                lastRowStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;

                lastRowStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Vertical.LineWidth = 1f;
                lastRowStyle.CellProperties.Borders.Vertical.Color = Color.FromArgb(255, 255, 255, 255);
                lastRowStyle.CellProperties.Borders.Vertical.Space = 0;

                lastRowStyle.CellProperties.BackColor = firstRowColor;
                lastRowStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                lastRowStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Character format
                firstColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, firstColumnStyle.CharacterFormat.Bold);
                firstColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstColumnStyle.CharacterFormat.BoldBidi);
                firstColumnStyle.CharacterFormat.Italic = false;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.ItalicKey, firstColumnStyle.CharacterFormat.Italic);
                firstColumnStyle.CharacterFormat.ItalicBidi = false;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.ItalicBidiKey, firstColumnStyle.CharacterFormat.ItalicBidi);
                firstColumnStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
                #endregion

                #region Table Cell Properties
                firstColumnStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Single;
                firstColumnStyle.CellProperties.Borders.Left.LineWidth = 1f;
                firstColumnStyle.CellProperties.Borders.Left.Color = Color.FromArgb(255, 255, 255, 255);
                firstColumnStyle.CellProperties.Borders.Left.Space = 0;

                firstColumnStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Single;
                firstColumnStyle.CellProperties.Borders.Right.LineWidth = 3f;
                firstColumnStyle.CellProperties.Borders.Right.Color = Color.FromArgb(255, 255, 255, 255);
                firstColumnStyle.CellProperties.Borders.Right.Space = 0;

                firstColumnStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;
                firstColumnStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                firstColumnStyle.CellProperties.BackColor = firstRowColor;
                firstColumnStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                firstColumnStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Character format
                lastColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, lastColumnStyle.CharacterFormat.Bold);
                lastColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastColumnStyle.CharacterFormat.BoldBidi);
                lastColumnStyle.CharacterFormat.Italic = false;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.ItalicKey, lastColumnStyle.CharacterFormat.Italic);
                lastColumnStyle.CharacterFormat.ItalicBidi = false;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.ItalicBidiKey, lastColumnStyle.CharacterFormat.ItalicBidi);
                lastColumnStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
                #endregion

                #region Table Cell Properties
                lastColumnStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Cleared;
                lastColumnStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Cleared;
                
                lastColumnStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Single;
                lastColumnStyle.CellProperties.Borders.Left.LineWidth = 3f;
                lastColumnStyle.CellProperties.Borders.Left.Color = Color.FromArgb(255, 255, 255, 255);
                lastColumnStyle.CellProperties.Borders.Left.Space = 0;
                
                lastColumnStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Cleared;
                lastColumnStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;
                lastColumnStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                lastColumnStyle.CellProperties.BackColor = firstRowColor;
                lastColumnStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                lastColumnStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Odd Column Banding Conditional Formatting Style
                oddColumnBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddColumnBanding);
                #region Table Cell Properties
                oddColumnBandingStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                oddColumnBandingStyle.CellProperties.Borders.Top.LineWidth = 1f;
                oddColumnBandingStyle.CellProperties.Borders.Top.Color = Color.FromArgb(255, 255, 255, 255);
                oddColumnBandingStyle.CellProperties.Borders.Top.Space = 0;

                oddColumnBandingStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                oddColumnBandingStyle.CellProperties.Borders.Bottom.LineWidth = 1f;
                oddColumnBandingStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 255, 255, 255);
                oddColumnBandingStyle.CellProperties.Borders.Bottom.Space = 0;

                oddColumnBandingStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Single;
                oddColumnBandingStyle.CellProperties.Borders.Left.LineWidth = 1f;
                oddColumnBandingStyle.CellProperties.Borders.Left.Color = Color.FromArgb(255, 255, 255, 255);
                oddColumnBandingStyle.CellProperties.Borders.Left.Space = 0;

                oddColumnBandingStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Single;
                oddColumnBandingStyle.CellProperties.Borders.Right.LineWidth = 1f;
                oddColumnBandingStyle.CellProperties.Borders.Right.Color = Color.FromArgb(255, 255, 255, 255);
                oddColumnBandingStyle.CellProperties.Borders.Right.Space = 0;

                oddColumnBandingStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;

                oddColumnBandingStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                oddColumnBandingStyle.CellProperties.BackColor = bandCellColor;
                oddColumnBandingStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                oddColumnBandingStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Odd Row Banding Conditional Formatting Style
                oddRowBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddRowBanding);
                #region Table Cell Properties
                oddRowBandingStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                oddRowBandingStyle.CellProperties.Borders.Top.LineWidth = 1f;
                oddRowBandingStyle.CellProperties.Borders.Top.Color = Color.FromArgb(255, 255, 255, 255);
                oddRowBandingStyle.CellProperties.Borders.Top.Space = 0;

                oddRowBandingStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                oddRowBandingStyle.CellProperties.Borders.Bottom.LineWidth = 1f;
                oddRowBandingStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 255, 255, 255);
                oddRowBandingStyle.CellProperties.Borders.Bottom.Space = 0;

                oddRowBandingStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Single;
                oddRowBandingStyle.CellProperties.Borders.Left.LineWidth = 1f;
                oddRowBandingStyle.CellProperties.Borders.Left.Color = Color.FromArgb(255, 255, 255, 255);
                oddRowBandingStyle.CellProperties.Borders.Left.Space = 0;

                oddRowBandingStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Single;
                oddRowBandingStyle.CellProperties.Borders.Right.LineWidth = 1f;
                oddRowBandingStyle.CellProperties.Borders.Right.Color = Color.FromArgb(255, 255, 255, 255);
                oddRowBandingStyle.CellProperties.Borders.Right.Space = 0;
                
                oddRowBandingStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Single;
                oddRowBandingStyle.CellProperties.Borders.Horizontal.LineWidth = 1f;
                oddRowBandingStyle.CellProperties.Borders.Horizontal.Color = Color.FromArgb(255, 255, 255, 255);
                oddRowBandingStyle.CellProperties.Borders.Horizontal.Space = 0;

                oddRowBandingStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Single;
                oddRowBandingStyle.CellProperties.Borders.Vertical.LineWidth = 1f;
                oddRowBandingStyle.CellProperties.Borders.Vertical.Color = Color.FromArgb(255, 255, 255, 255);
                oddRowBandingStyle.CellProperties.Borders.Vertical.Space = 0;

                oddRowBandingStyle.CellProperties.BackColor = bandCellColor;
                oddRowBandingStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                oddRowBandingStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Dark List.
            /// </summary>
            /// <param name="style">The style.</param>
            /// <param name="backColor">The back Color.</param>
            /// <param name="lastRowColor">The last Row Color.</param>
            /// <param name="bandCellColor">The band Cell Color.</param>
            private static void LoadStyleDarkList(IStyle style, Color backColor, Color lastRowColor, Color bandCellColor)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle, lastColumnStyle, oddColumnBandingStyle, oddRowBandingStyle;
                #region Character format
                (style as WTableStyle).CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
                #endregion
                
                #region Paragraph format
                (style as WTableStyle).ParagraphFormat.AfterSpacing = 0;
                (style as WTableStyle).ParagraphFormat.LineSpacing = 12;
                (style as WTableStyle).ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                #endregion

                #region Table Properties
                (style as WTableStyle).TableProperties.RowStripe = 1;
                (style as WTableStyle).TableProperties.ColumnStripe = 1;
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = backColor;
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Cleared;
                
                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = 2.25f;
                firstRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 255, 255, 255);
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;

                firstRowStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Cleared;
                firstRowStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Cleared;
                firstRowStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;
                firstRowStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                firstRowStyle.CellProperties.BackColor = Color.FromArgb(255, 0, 0, 0);
                firstRowStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                firstRowStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = 2.25f;
                lastRowStyle.CellProperties.Borders.Top.Color = Color.FromArgb(255, 255, 255, 255);
                lastRowStyle.CellProperties.Borders.Top.Space = 0;

                lastRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Cleared;
                lastRowStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Cleared;
                lastRowStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Cleared;
                lastRowStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;
                lastRowStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                lastRowStyle.CellProperties.BackColor = lastRowColor;
                lastRowStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                lastRowStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Table Cell Properties
                firstColumnStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Cleared;
                firstColumnStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Cleared;
                firstColumnStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Cleared;

                firstColumnStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Single;
                firstColumnStyle.CellProperties.Borders.Right.LineWidth = 2.25f;
                firstColumnStyle.CellProperties.Borders.Right.Color = Color.FromArgb(255, 255, 255, 255);
                firstColumnStyle.CellProperties.Borders.Right.Space = 0;

                firstColumnStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;
                firstColumnStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                firstColumnStyle.CellProperties.BackColor = bandCellColor;
                firstColumnStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                firstColumnStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Table Cell Properties
                lastColumnStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Cleared;
                lastColumnStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Cleared;

                lastColumnStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Single;
                lastColumnStyle.CellProperties.Borders.Left.LineWidth = 2.25f;
                lastColumnStyle.CellProperties.Borders.Left.Color = Color.FromArgb(255, 255, 255, 255);
                lastColumnStyle.CellProperties.Borders.Left.Space = 0;

                lastColumnStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Cleared;
                lastColumnStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;
                lastColumnStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                lastColumnStyle.CellProperties.BackColor = bandCellColor;
                lastColumnStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                lastColumnStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Odd Column Banding Conditional Formatting Style
                oddColumnBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddColumnBanding);
                #region Table Cell Properties
                oddColumnBandingStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Cleared;
                oddColumnBandingStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Cleared;
                oddColumnBandingStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Cleared;
                oddColumnBandingStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Cleared;
                oddColumnBandingStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;
                oddColumnBandingStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                oddColumnBandingStyle.CellProperties.BackColor = bandCellColor;
                oddColumnBandingStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                oddColumnBandingStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Odd Row Banding Conditional Formatting Style
                oddRowBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddRowBanding);
                #region Table Cell Properties
                oddRowBandingStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Cleared;
                oddRowBandingStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Cleared;
                oddRowBandingStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Cleared;
                oddRowBandingStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Cleared;
                oddRowBandingStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;
                oddRowBandingStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                oddRowBandingStyle.CellProperties.BackColor = bandCellColor;
                oddRowBandingStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                oddRowBandingStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Colorful Shading.
            /// </summary>
            /// <param name="style">The style.</param>
            /// <param name="topBorderColor">The top Border Color.</param>
            /// <param name="borderColor">The border Color.</param>
            /// <param name="backColor">The back Color.</param>
            /// <param name="lastRowColor">The last Row Color.</param>
            /// <param name="bandColumnColor">The band Column Color.</param>
            /// <param name="bandRowColor">The band Row Color.</param>
            private static void LoadStyleColorfulShading(IStyle style, Color topBorderColor, Color borderColor, Color backColor, Color lastRowColor, Color bandColumnColor, Color bandRowColor)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle, lastColumnStyle, oddColumnBandingStyle, oddRowBandingStyle, firstRowFirstCellStyle, firstRowLastCellStyle;
                #region Character format
                (style as WTableStyle).CharacterFormat.TextColor = Color.FromArgb(255, 0, 0, 0);
                #endregion

                #region Paragraph format
                (style as WTableStyle).ParagraphFormat.AfterSpacing = 0;
                (style as WTableStyle).ParagraphFormat.LineSpacing = 12;
                (style as WTableStyle).ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                #endregion

                #region Table Properties
                (style as WTableStyle).TableProperties.RowStripe = 1;
                (style as WTableStyle).TableProperties.ColumnStripe = 1;
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = 3f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = topBorderColor;
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = .5f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = .5f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = .5f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = borderColor;
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Horizontal.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Horizontal.LineWidth = .5f;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Color = Color.FromArgb(255, 255, 255, 255);
                (style as WTableStyle).TableProperties.Borders.Horizontal.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Vertical.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Vertical.LineWidth = .5f;
                (style as WTableStyle).TableProperties.Borders.Vertical.Color = Color.FromArgb(255, 255, 255, 255);
                (style as WTableStyle).TableProperties.Borders.Vertical.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = backColor;
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Cleared;

                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = 3f;
                firstRowStyle.CellProperties.Borders.Bottom.Color = topBorderColor;
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;

                firstRowStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Cleared;
                firstRowStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Cleared;
                firstRowStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;
                firstRowStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                firstRowStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstRowStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                firstRowStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Character format
                lastRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowStyle.CharacterFormat.Bold);
                lastRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowStyle.CharacterFormat.BoldBidi);
                lastRowStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
                #endregion
                
                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = .75f;
                lastRowStyle.CellProperties.Borders.Top.Color = Color.FromArgb(255, 255, 255, 255);
                lastRowStyle.CellProperties.Borders.Top.Space = 0;

                lastRowStyle.CellProperties.BackColor = lastRowColor;
                lastRowStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                lastRowStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Character format
                firstColumnStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
                #endregion

                #region Table Cell Properties
                firstColumnStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Cleared;
                firstColumnStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Cleared;
                firstColumnStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Cleared;
                firstColumnStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Cleared;

                firstColumnStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Single;
                firstColumnStyle.CellProperties.Borders.Horizontal.LineWidth = .5f;
                firstColumnStyle.CellProperties.Borders.Horizontal.Color = lastRowColor;
                firstColumnStyle.CellProperties.Borders.Horizontal.Space = 0;

                firstColumnStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                firstColumnStyle.CellProperties.BackColor = lastRowColor;
                firstColumnStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                firstColumnStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Character format
                lastColumnStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
                #endregion

                #region Table Cell Properties
                lastColumnStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Cleared;
                lastColumnStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Cleared;
                lastColumnStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Cleared;
                lastColumnStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Cleared;
                lastColumnStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;
                lastColumnStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                lastColumnStyle.CellProperties.BackColor = lastRowColor;
                lastColumnStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                lastColumnStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Odd Column Banding Conditional Formatting Style
                oddColumnBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddColumnBanding);
                #region Table Cell Properties
                oddColumnBandingStyle.CellProperties.BackColor = bandColumnColor;
                oddColumnBandingStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                oddColumnBandingStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Odd Row Banding Conditional Formatting Style
                oddRowBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddRowBanding);
                #region Table Cell Properties
                oddRowBandingStyle.CellProperties.BackColor = bandRowColor;
                oddRowBandingStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                oddRowBandingStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                if (style.Name != "Colorful Shading Accent 3")
                {
                    #region First Row Last Cell Conditional Formatting Style
                    firstRowLastCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRowLastCell);
                    #region Character format
                    firstRowLastCellStyle.CharacterFormat.TextColor = Color.FromArgb(255, 0, 0, 0);
                    #endregion
                    #endregion

                    #region First Row First Cell Conditional Formatting Style
                    firstRowFirstCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRowFirstCell);
                    #region Character format
                    firstRowFirstCellStyle.CharacterFormat.TextColor = Color.FromArgb(255, 0, 0, 0);
                    #endregion
                    #endregion
                }
                #endregion
            }
            /// <summary>
            /// Loads the table style Colorful List.
            /// </summary>
            /// <param name="style">The style.</param>
            /// <param name="backColor">The back Color.</param>
            /// <param name="rowColor">The row Color.</param>
            /// <param name="bandColumnColor">The band Column Color.</param>
            /// <param name="bandRowColor">The band Row Color.</param>
            private static void LoadStyleColorfulList(IStyle style, Color backColor, Color rowColor, Color bandColumnColor, Color bandRowColor)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle, lastColumnStyle, oddColumnBandingStyle, oddRowBandingStyle;
                #region Character format
                (style as WTableStyle).CharacterFormat.TextColor = Color.FromArgb(255, 0, 0, 0);
                #endregion

                #region Paragraph format
                (style as WTableStyle).ParagraphFormat.AfterSpacing = 0;
                (style as WTableStyle).ParagraphFormat.LineSpacing = 12;
                (style as WTableStyle).ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                #endregion

                #region Table Properties
                (style as WTableStyle).TableProperties.RowStripe = 1;
                (style as WTableStyle).TableProperties.ColumnStripe = 1;
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = backColor;
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                firstRowStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = 1.5f;
                firstRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 255, 255, 255);
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;

                firstRowStyle.CellProperties.BackColor = rowColor;
                firstRowStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                firstRowStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Character format
                lastRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowStyle.CharacterFormat.Bold);
                lastRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowStyle.CharacterFormat.BoldBidi);
                lastRowStyle.CharacterFormat.TextColor = rowColor;
                #endregion

                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = 1.5f;
                lastRowStyle.CellProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                lastRowStyle.CellProperties.Borders.Top.Space = 0;

                lastRowStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                lastRowStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                lastRowStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Character format
                firstColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, firstColumnStyle.CharacterFormat.Bold);
                firstColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstColumnStyle.CharacterFormat.BoldBidi);
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Character format
                lastColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, lastColumnStyle.CharacterFormat.Bold);
                lastColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastColumnStyle.CharacterFormat.BoldBidi);
                #endregion
                #endregion

                #region Odd Column Banding Conditional Formatting Style
                oddColumnBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddColumnBanding);
                #region Table Cell Properties
                oddColumnBandingStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Cleared;
                oddColumnBandingStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Cleared;
                oddColumnBandingStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Cleared;
                oddColumnBandingStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Cleared;
                oddColumnBandingStyle.CellProperties.Borders.Horizontal.BorderType = BorderStyle.Cleared;
                oddColumnBandingStyle.CellProperties.Borders.Vertical.BorderType = BorderStyle.Cleared;

                oddColumnBandingStyle.CellProperties.BackColor = bandColumnColor;
                oddColumnBandingStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                oddColumnBandingStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Odd Row Banding Conditional Formatting Style
                oddRowBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddRowBanding);
                #region Table Cell Properties
                oddRowBandingStyle.CellProperties.BackColor = bandRowColor;
                oddRowBandingStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                oddRowBandingStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Colorful Grid.
            /// </summary>
            /// <param name="style">The style.</param>
            /// <param name="backColor">The back Color.</param>
            /// <param name="rowColor">The row Color.</param>
            /// <param name="columnColor">The column Color.</param>
            /// <param name="bandColor">The band Color.</param>
            private static void LoadStyleColorfulGrid(IStyle style, Color backColor, Color rowColor, Color columnColor, Color bandColor)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle, lastColumnStyle, oddColumnBandingStyle, oddRowBandingStyle, firstRowFirstCellStyle, firstRowLastCellStyle;
                #region Character format
                (style as WTableStyle).CharacterFormat.TextColor = Color.FromArgb(255, 0, 0, 0);
                #endregion

                #region Paragraph format
                (style as WTableStyle).ParagraphFormat.AfterSpacing = 0;
                (style as WTableStyle).ParagraphFormat.LineSpacing = 12;
                (style as WTableStyle).ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                #endregion

                #region Table Properties
                (style as WTableStyle).TableProperties.RowStripe = 1;
                (style as WTableStyle).TableProperties.ColumnStripe = 1;
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Horizontal.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Horizontal.LineWidth = .5f;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Color = Color.FromArgb(255, 255, 255, 255);
                (style as WTableStyle).TableProperties.Borders.Horizontal.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = backColor;
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.BackColor = rowColor;
                firstRowStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                firstRowStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Character format
                lastRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowStyle.CharacterFormat.Bold);
                lastRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowStyle.CharacterFormat.BoldBidi);
                lastRowStyle.CharacterFormat.TextColor = Color.FromArgb(255, 0, 0, 0);
                #endregion

                #region Table Cell Properties
                lastRowStyle.CellProperties.BackColor = rowColor;
                lastRowStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                lastRowStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Character format
                firstColumnStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
                #endregion

                #region Table Cell Properties
                firstColumnStyle.CellProperties.BackColor = columnColor;
                firstColumnStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                firstColumnStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Character format
                lastColumnStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
                #endregion

                #region Table Cell Properties
                lastColumnStyle.CellProperties.BackColor = columnColor;
                lastColumnStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                lastColumnStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Odd Column Banding Conditional Formatting Style
                oddColumnBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddColumnBanding);
                #region Table Cell Properties
                oddColumnBandingStyle.CellProperties.BackColor = bandColor;
                oddColumnBandingStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                oddColumnBandingStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion

                #region Odd Row Banding Conditional Formatting Style
                oddRowBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddRowBanding);
                #region Table Cell Properties
                oddRowBandingStyle.CellProperties.BackColor = bandColor;
                oddRowBandingStyle.CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                oddRowBandingStyle.CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table 3D effects 1.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTable3Deffects1(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle, lastColumnStyle, firstRowFirstCellStyle, firstRowLastCellStyle, lastRowFirstCellStyle, lastRowLastCellStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(255, 192, 192, 192);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                firstRowStyle.CharacterFormat.TextColor = Color.FromArgb(255, 128, 0, 128);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 128, 128, 128);
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = .75f;

                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Top.Color = Color.FromArgb(255, 255, 255, 255);
                lastRowStyle.CellProperties.Borders.Top.Space = 0;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = .75f;

                lastRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Character format
                firstColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, firstColumnStyle.CharacterFormat.Bold);
                firstColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstColumnStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstColumnStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Single;
                firstColumnStyle.CellProperties.Borders.Right.Color = Color.FromArgb(255, 128, 128, 128);
                firstColumnStyle.CellProperties.Borders.Right.Space = 0;
                firstColumnStyle.CellProperties.Borders.Right.LineWidth = .75f;

                firstColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Table Cell Properties
                lastColumnStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Single;
                lastColumnStyle.CellProperties.Borders.Left.Color = Color.FromArgb(255, 255, 255, 255);
                lastColumnStyle.CellProperties.Borders.Left.Space = 0;
                lastColumnStyle.CellProperties.Borders.Left.LineWidth = .75f;

                lastColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region First Row Last Cell Conditional Formatting Style
                firstRowLastCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRowLastCell);
                #region Table Cell Properties
                firstRowLastCellStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.None;
                firstRowLastCellStyle.CellProperties.Borders.Bottom.Color = Color.Black;
                firstRowLastCellStyle.CellProperties.Borders.Bottom.Space = 0;
                firstRowLastCellStyle.CellProperties.Borders.Bottom.LineWidth = 0;

                firstRowLastCellStyle.CellProperties.Borders.Left.BorderType = BorderStyle.None;
                firstRowLastCellStyle.CellProperties.Borders.Left.Color = Color.Black;
                firstRowLastCellStyle.CellProperties.Borders.Left.Space = 0;
                firstRowLastCellStyle.CellProperties.Borders.Left.LineWidth = 0;

                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region First Row First Cell Conditional Formatting Style
                firstRowFirstCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRowFirstCell);
                #region Table Cell Properties
                firstRowFirstCellStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.None;
                firstRowFirstCellStyle.CellProperties.Borders.Bottom.Color = Color.Black;
                firstRowFirstCellStyle.CellProperties.Borders.Bottom.Space = 0;
                firstRowFirstCellStyle.CellProperties.Borders.Bottom.LineWidth = 0;

                firstRowFirstCellStyle.CellProperties.Borders.Right.BorderType = BorderStyle.None;
                firstRowFirstCellStyle.CellProperties.Borders.Right.Color = Color.Black;
                firstRowFirstCellStyle.CellProperties.Borders.Right.Space = 0;
                firstRowFirstCellStyle.CellProperties.Borders.Right.LineWidth = 0;

                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Row Last Cell Conditional Formatting Style
                lastRowLastCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRowLastCell);
                #region Table Cell Properties
                lastRowLastCellStyle.CellProperties.Borders.Top.BorderType = BorderStyle.None;
                lastRowLastCellStyle.CellProperties.Borders.Top.Color = Color.Black;
                lastRowLastCellStyle.CellProperties.Borders.Top.Space = 0;
                lastRowLastCellStyle.CellProperties.Borders.Top.LineWidth = 0;

                lastRowLastCellStyle.CellProperties.Borders.Left.BorderType = BorderStyle.None;
                lastRowLastCellStyle.CellProperties.Borders.Left.Color = Color.Black;
                lastRowLastCellStyle.CellProperties.Borders.Left.Space = 0;
                lastRowLastCellStyle.CellProperties.Borders.Left.LineWidth = 0;

                lastRowLastCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowLastCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowLastCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowLastCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowLastCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowLastCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowLastCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowLastCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Row First Cell Conditional Formatting Style
                lastRowFirstCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRowFirstCell);
                #region Character format
                lastRowFirstCellStyle.CharacterFormat.TextColor = Color.FromArgb(255, 0, 0, 128);
                #endregion
                
                #region Table Cell Properties
                lastRowFirstCellStyle.CellProperties.Borders.Top.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.Top.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.Top.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.Top.LineWidth = 0;

                lastRowFirstCellStyle.CellProperties.Borders.Right.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.Right.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.Right.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.Right.LineWidth = 0;

                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table 3D effects 2.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTable3Deffects2(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, firstColumnStyle, lastColumnStyle, oddRowBandingStyle, lastRowFirstCellStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.RowStripe = 1;
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(255, 192, 192, 192);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Table Cell Properties
                firstColumnStyle.CellProperties.Borders.Top.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.Top.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.Top.Space = 0;
                firstColumnStyle.CellProperties.Borders.Top.LineWidth = 0;

                firstColumnStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.Bottom.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.Bottom.Space = 0;
                firstColumnStyle.CellProperties.Borders.Bottom.LineWidth = 0;

                firstColumnStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Single;
                firstColumnStyle.CellProperties.Borders.Right.Color = Color.FromArgb(255, 128, 128, 128);
                firstColumnStyle.CellProperties.Borders.Right.Space = 0;
                firstColumnStyle.CellProperties.Borders.Right.LineWidth = .75f;

                firstColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Table Cell Properties
                lastColumnStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Single;
                lastColumnStyle.CellProperties.Borders.Right.Color = Color.FromArgb(255, 255, 255, 255);
                lastColumnStyle.CellProperties.Borders.Right.Space = 0;
                lastColumnStyle.CellProperties.Borders.Right.LineWidth = .75f;

                lastColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Odd Row Banding Conditional Formatting Style
                oddRowBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddRowBanding);
                #region Table Cell Properties
                oddRowBandingStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                oddRowBandingStyle.CellProperties.Borders.Top.Color = Color.FromArgb(255, 128, 128, 128);
                oddRowBandingStyle.CellProperties.Borders.Top.Space = 0;
                oddRowBandingStyle.CellProperties.Borders.Top.LineWidth = .75f;

                oddRowBandingStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                oddRowBandingStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 255, 255, 255);
                oddRowBandingStyle.CellProperties.Borders.Bottom.Space = 0;
                oddRowBandingStyle.CellProperties.Borders.Bottom.LineWidth = .75f;

                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Row First Cell Conditional Formatting Style
                lastRowFirstCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRowFirstCell);
                #region Character format
                lastRowFirstCellStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowFirstCellStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowFirstCellStyle.CharacterFormat.Bold);
                lastRowFirstCellStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowFirstCellStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowFirstCellStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table 3D effects 3.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTable3Deffects3(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, firstColumnStyle, lastColumnStyle, oddColumnBandingStyle, evenColumnBandingStyle, oddRowBandingStyle, lastRowFirstCellStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.RowStripe = 1;
                (style as WTableStyle).TableProperties.ColumnStripe = 1;
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Table Cell Properties
                firstColumnStyle.CellProperties.Borders.Top.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.Top.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.Top.Space = 0;
                firstColumnStyle.CellProperties.Borders.Top.LineWidth = 0;

                firstColumnStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.Bottom.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.Bottom.Space = 0;
                firstColumnStyle.CellProperties.Borders.Bottom.LineWidth = 0;

                firstColumnStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Single;
                firstColumnStyle.CellProperties.Borders.Right.Color = Color.FromArgb(255, 128, 128, 128);
                firstColumnStyle.CellProperties.Borders.Right.Space = 0;
                firstColumnStyle.CellProperties.Borders.Right.LineWidth = .75f;

                firstColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Table Cell Properties
                lastColumnStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Single;
                lastColumnStyle.CellProperties.Borders.Right.Color = Color.FromArgb(255, 255, 255, 255);
                lastColumnStyle.CellProperties.Borders.Right.Space = 0;
                lastColumnStyle.CellProperties.Borders.Right.LineWidth = .75f;

                lastColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Odd Column Banding Conditional Formatting Style
                oddColumnBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddColumnBanding);
                #region Character format
                oddColumnBandingStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                oddColumnBandingStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                oddColumnBandingStyle.CellProperties.ForeColor = Color.FromArgb(255, 192, 192, 192);
                oddColumnBandingStyle.CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion
                #endregion

                #region Even Column Banding Conditional Formatting Style
                evenColumnBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.EvenColumnBanding);
                #region Character format
                evenColumnBandingStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                evenColumnBandingStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                evenColumnBandingStyle.CellProperties.ForeColor = Color.FromArgb(255, 192, 192, 192);
                evenColumnBandingStyle.CellProperties.TextureStyle = TextureStyle.Texture50Percent;
                #endregion
                #endregion

                #region Odd Row Banding Conditional Formatting Style
                oddRowBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddRowBanding);
                #region Table Cell Properties
                oddRowBandingStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                oddRowBandingStyle.CellProperties.Borders.Top.Color = Color.FromArgb(255, 128, 128, 128);
                oddRowBandingStyle.CellProperties.Borders.Top.Space = 0;
                oddRowBandingStyle.CellProperties.Borders.Top.LineWidth = .75f;

                oddRowBandingStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                oddRowBandingStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 255, 255, 255);
                oddRowBandingStyle.CellProperties.Borders.Bottom.Space = 0;
                oddRowBandingStyle.CellProperties.Borders.Bottom.LineWidth = .75f;

                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Row First Cell Conditional Formatting Style
                lastRowFirstCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRowFirstCell);
                #region Character format
                lastRowFirstCellStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowFirstCellStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowFirstCellStyle.CharacterFormat.Bold);
                lastRowFirstCellStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowFirstCellStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowFirstCellStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Classic 1.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableClassic1(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle, firstRowLastCellStyle, lastRowFirstCellStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Italic = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.ItalicKey, firstRowStyle.CharacterFormat.Italic);
                firstRowStyle.CharacterFormat.ItalicBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.ItalicBidiKey, firstRowStyle.CharacterFormat.ItalicBidi);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = .75f;

                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Character format
                lastRowStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                lastRowStyle.CellProperties.Borders.Top.Space = 0;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = .75f;

                lastRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Table Cell Properties
                firstColumnStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Single;
                firstColumnStyle.CellProperties.Borders.Right.Color = Color.FromArgb(255, 0, 0, 0);
                firstColumnStyle.CellProperties.Borders.Right.Space = 0;
                firstColumnStyle.CellProperties.Borders.Right.LineWidth = .75f;

                firstColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region First Row Last Cell Conditional Formatting Style
                firstRowLastCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRowLastCell);
                #region Character format
                firstRowLastCellStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowLastCellStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowLastCellStyle.CharacterFormat.Bold);
                firstRowLastCellStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowLastCellStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowLastCellStyle.CharacterFormat.BoldBidi);
                firstRowLastCellStyle.CharacterFormat.Italic = false;
                UpdateComplexProperty(firstRowLastCellStyle.CharacterFormat, WCharacterFormat.ItalicKey, firstRowLastCellStyle.CharacterFormat.Italic);
                firstRowLastCellStyle.CharacterFormat.ItalicBidi = false;
                UpdateComplexProperty(firstRowLastCellStyle.CharacterFormat, WCharacterFormat.ItalicBidiKey, firstRowLastCellStyle.CharacterFormat.ItalicBidi);
                #endregion

                #region Table Cell Properties
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Row First Cell Conditional Formatting Style
                lastRowFirstCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRowFirstCell);
                #region Character format
                lastRowFirstCellStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowFirstCellStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowFirstCellStyle.CharacterFormat.Bold);
                lastRowFirstCellStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowFirstCellStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowFirstCellStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Classic 2.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableClassic2(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle, firstRowLastCellStyle, firstRowFirstCellStyle, lastRowFirstCellStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = .75f;

                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                firstRowStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstRowStyle.CellProperties.ForeColor = Color.FromArgb(255, 128, 0, 128);
                firstRowStyle.CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                lastRowStyle.CellProperties.Borders.Top.Space = 0;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = .75f;

                lastRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Character format
                firstColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, firstColumnStyle.CharacterFormat.Bold);
                firstColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstColumnStyle.CharacterFormat.BoldBidi);
                #endregion
                
                #region Table Cell Properties
                firstColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                firstColumnStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstColumnStyle.CellProperties.ForeColor = Color.FromArgb(255, 192, 192, 192);
                firstColumnStyle.CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion
                #endregion

                #region First Row Last Cell Conditional Formatting Style
                firstRowLastCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRowLastCell);
                #region Character format
                firstRowLastCellStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowLastCellStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowLastCellStyle.CharacterFormat.Bold);
                firstRowLastCellStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowLastCellStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowLastCellStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region First Row First Cell Conditional Formatting Style
                firstRowFirstCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRowFirstCell);
                #region Table Cell Properties
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                firstRowFirstCellStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstRowFirstCellStyle.CellProperties.ForeColor = Color.FromArgb(255, 128, 0, 128);
                firstRowFirstCellStyle.CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion
                #endregion

                #region Last Row First Cell Conditional Formatting Style
                lastRowFirstCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRowFirstCell);
                #region Character format
                lastRowFirstCellStyle.CharacterFormat.TextColor = Color.FromArgb(255, 0, 0, 128);
                #endregion

                #region Table Cell Properties
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Classic 3.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableClassic3(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Character format
                (style as WTableStyle).CharacterFormat.TextColor = Color.FromArgb(255, 0, 0, 128);
                #endregion

                #region Table Properties
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(255, 192, 192, 192);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                firstRowStyle.CharacterFormat.Italic = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.ItalicKey, firstRowStyle.CharacterFormat.Italic);
                firstRowStyle.CharacterFormat.ItalicBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.ItalicBidiKey, firstRowStyle.CharacterFormat.ItalicBidi);
                firstRowStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = .75f;

                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                firstRowStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstRowStyle.CellProperties.ForeColor = Color.FromArgb(255, 0, 0, 128);
                firstRowStyle.CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Character format
                lastRowStyle.CharacterFormat.TextColor = Color.FromArgb(255, 0, 0, 128);
                #endregion

                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                lastRowStyle.CellProperties.Borders.Top.Space = 0;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = 1.5f;

                lastRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                lastRowStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                lastRowStyle.CellProperties.ForeColor = Color.FromArgb(255, 255, 255, 255);
                lastRowStyle.CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Character format
                firstColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, firstColumnStyle.CharacterFormat.Bold);
                firstColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstColumnStyle.CharacterFormat.BoldBidi);
                firstColumnStyle.CharacterFormat.TextColor = Color.FromArgb(255, 0, 0, 0);
                #endregion

                #region Table Cell Properties
                firstColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Classic 4.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableClassic4(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle, firstRowFirstCellStyle, lastRowFirstCellStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                firstRowStyle.CharacterFormat.Italic = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.ItalicKey, firstRowStyle.CharacterFormat.Italic);
                firstRowStyle.CharacterFormat.ItalicBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.ItalicBidiKey, firstRowStyle.CharacterFormat.ItalicBidi);
                firstRowStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = .75f;

                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                firstRowStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstRowStyle.CellProperties.ForeColor = Color.FromArgb(255, 0, 0, 128);
                firstRowStyle.CellProperties.TextureStyle = TextureStyle.Texture50Percent;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Character format
                lastRowStyle.CharacterFormat.TextColor = Color.FromArgb(255, 0, 0, 128);
                #endregion

                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                lastRowStyle.CellProperties.Borders.Bottom.Space = 0;
                lastRowStyle.CellProperties.Borders.Bottom.LineWidth = .75f;

                lastRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                lastRowStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                lastRowStyle.CellProperties.ForeColor = Color.FromArgb(255, 0, 0, 0);
                lastRowStyle.CellProperties.TextureStyle = TextureStyle.Texture50Percent;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Character format
                firstColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, firstColumnStyle.CharacterFormat.Bold);
                firstColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstColumnStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region First Row First Cell Conditional Formatting Style
                firstRowFirstCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRowFirstCell);
                #region Character format
                firstRowFirstCellStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowFirstCellStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowFirstCellStyle.CharacterFormat.Bold);
                firstRowFirstCellStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowFirstCellStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowFirstCellStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Row First Cell Conditional Formatting Style
                lastRowFirstCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRowFirstCell);
                #region Character format
                lastRowFirstCellStyle.CharacterFormat.TextColor = Color.FromArgb(255, 0, 0, 128);
                #endregion

                #region Table Cell Properties
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Colorful 1.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableColorful1(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, firstColumnStyle, firstRowFirstCellStyle, lastRowFirstCellStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Character format
                (style as WTableStyle).CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
                #endregion

                #region Table Properties
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.FromArgb(255, 0, 128, 128);
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 128, 128);
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = Color.FromArgb(255, 0, 128, 128);
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = Color.FromArgb(255, 0, 128, 128);
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Horizontal.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Horizontal.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Color = Color.FromArgb(255, 0, 255, 255);
                (style as WTableStyle).TableProperties.Borders.Horizontal.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(255, 0, 128, 128);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                firstRowStyle.CharacterFormat.Italic = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.ItalicKey, firstRowStyle.CharacterFormat.Italic);
                firstRowStyle.CharacterFormat.ItalicBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.ItalicBidiKey, firstRowStyle.CharacterFormat.ItalicBidi);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                firstRowStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstRowStyle.CellProperties.ForeColor = Color.FromArgb(255, 0, 0, 0);
                firstRowStyle.CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Character format
                firstColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, firstColumnStyle.CharacterFormat.Bold);
                firstColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstColumnStyle.CharacterFormat.BoldBidi);
                firstColumnStyle.CharacterFormat.Italic = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.ItalicKey, firstColumnStyle.CharacterFormat.Italic);
                firstColumnStyle.CharacterFormat.ItalicBidi = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.ItalicBidiKey, firstColumnStyle.CharacterFormat.ItalicBidi);
                #endregion

                #region Table Cell Properties
                firstColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                firstColumnStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstColumnStyle.CellProperties.ForeColor = Color.FromArgb(255, 0, 0, 128);
                firstColumnStyle.CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion
                #endregion

                #region First Row First Cell Conditional Formatting Style
                firstRowFirstCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRowFirstCell);
                #region Table Cell Properties
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                firstRowFirstCellStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstRowFirstCellStyle.CellProperties.ForeColor = Color.FromArgb(255, 0, 0, 0);
                firstRowFirstCellStyle.CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion
                #endregion

                #region Last Row First Cell Conditional Formatting Style
                lastRowFirstCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRowFirstCell);
                #region Character format
                lastRowFirstCellStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowFirstCellStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowFirstCellStyle.CharacterFormat.Bold);
                lastRowFirstCellStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowFirstCellStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowFirstCellStyle.CharacterFormat.BoldBidi);
                lastRowFirstCellStyle.CharacterFormat.Italic = false;
                UpdateComplexProperty(lastRowFirstCellStyle.CharacterFormat, WCharacterFormat.ItalicKey, lastRowFirstCellStyle.CharacterFormat.Italic);
                lastRowFirstCellStyle.CharacterFormat.ItalicBidi = false;
                UpdateComplexProperty(lastRowFirstCellStyle.CharacterFormat, WCharacterFormat.ItalicBidiKey, lastRowFirstCellStyle.CharacterFormat.ItalicBidi);
                #endregion

                #region Table Cell Properties
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Colorful 2.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableColorful2(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, firstColumnStyle, lastColumnStyle, lastRowFirstCellStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(255, 255, 255, 0);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.Texture20Percent;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                firstRowStyle.CharacterFormat.Italic = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.ItalicKey, firstRowStyle.CharacterFormat.Italic);
                firstRowStyle.CharacterFormat.ItalicBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.ItalicBidiKey, firstRowStyle.CharacterFormat.ItalicBidi);
                firstRowStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = 1.5f;
                firstRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;

                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                firstRowStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstRowStyle.CellProperties.ForeColor = Color.FromArgb(255, 128, 0, 0);
                firstRowStyle.CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Character format
                firstColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, firstColumnStyle.CharacterFormat.Bold);
                firstColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstColumnStyle.CharacterFormat.BoldBidi);
                firstColumnStyle.CharacterFormat.Italic = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.ItalicKey, firstColumnStyle.CharacterFormat.Italic);
                firstColumnStyle.CharacterFormat.ItalicBidi = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.ItalicBidiKey, firstColumnStyle.CharacterFormat.ItalicBidi);
                #endregion

                #region Table Cell Properties
                firstColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Table Cell Properties
                lastColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                lastColumnStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                lastColumnStyle.CellProperties.ForeColor = Color.FromArgb(255, 192, 192, 192);
                lastColumnStyle.CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion
                #endregion

                #region Last Row First Cell Conditional Formatting Style
                lastRowFirstCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRowFirstCell);
                #region Character format
                lastRowFirstCellStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowFirstCellStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowFirstCellStyle.CharacterFormat.Bold);
                lastRowFirstCellStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowFirstCellStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowFirstCellStyle.CharacterFormat.BoldBidi);
                lastRowFirstCellStyle.CharacterFormat.Italic = false;
                UpdateComplexProperty(lastRowFirstCellStyle.CharacterFormat, WCharacterFormat.ItalicKey, lastRowFirstCellStyle.CharacterFormat.Italic);
                lastRowFirstCellStyle.CharacterFormat.ItalicBidi = false;
                UpdateComplexProperty(lastRowFirstCellStyle.CharacterFormat, WCharacterFormat.ItalicBidiKey, lastRowFirstCellStyle.CharacterFormat.ItalicBidi);
                #endregion

                #region Table Cell Properties
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Colorful 3.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableColorful3(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, firstColumnStyle, firstRowFirstCellStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = 2.25f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 2.25f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = 2.25f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = 2.25f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Horizontal.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Horizontal.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Color = Color.FromArgb(255, 192, 192, 192);
                (style as WTableStyle).TableProperties.Borders.Horizontal.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(255, 0, 128, 128);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.Texture25Percent;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = .75f;
                firstRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;

                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                firstRowStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstRowStyle.CellProperties.ForeColor = Color.FromArgb(255, 0, 128, 128);
                firstRowStyle.CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Table Cell Properties
                firstColumnStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Single;
                firstColumnStyle.CellProperties.Borders.Left.LineWidth = 4.5f;
                firstColumnStyle.CellProperties.Borders.Left.Color = Color.FromArgb(255, 0, 0, 0);
                firstColumnStyle.CellProperties.Borders.Left.Space = 0;

                firstColumnStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Single;
                firstColumnStyle.CellProperties.Borders.Right.LineWidth = .75f;
                firstColumnStyle.CellProperties.Borders.Right.Color = Color.FromArgb(255, 0, 0, 0);
                firstColumnStyle.CellProperties.Borders.Right.Space = 0;

                firstColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                firstColumnStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstColumnStyle.CellProperties.ForeColor = Color.FromArgb(255, 0, 128, 128);
                firstColumnStyle.CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion
                #endregion

                #region First Row First Cell Conditional Formatting Style
                firstRowFirstCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRowFirstCell);
                #region Character format
                firstRowFirstCellStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowFirstCellStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowFirstCellStyle.CharacterFormat.Bold);
                firstRowFirstCellStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowFirstCellStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowFirstCellStyle.CharacterFormat.BoldBidi);
                firstRowFirstCellStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
                #endregion
                
                #region Table Cell Properties
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                firstRowFirstCellStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstRowFirstCellStyle.CellProperties.ForeColor = Color.FromArgb(255, 0, 0, 0);
                firstRowFirstCellStyle.CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Columns 1.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableColumns1(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle, lastColumnStyle, oddColumnBandingStyle, evenColumnBandingStyle, firstRowLastCellStyle, lastRowFirstCellStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Character format
                (style as WTableStyle).CharacterFormat.Bold = true;
                UpdateComplexProperty((style as WTableStyle).CharacterFormat, WCharacterFormat.BoldKey, (style as WTableStyle).CharacterFormat.Bold);
                (style as WTableStyle).CharacterFormat.BoldBidi = true;
                UpdateComplexProperty((style as WTableStyle).CharacterFormat, WCharacterFormat.BoldBidiKey, (style as WTableStyle).CharacterFormat.BoldBidi);
                #endregion
                
                #region Table Properties
                (style as WTableStyle).TableProperties.ColumnStripe = 1;
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = false;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = false;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Double;
                firstRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = .75f;

                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Character format
                lastRowStyle.CharacterFormat.Bold = false;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowStyle.CharacterFormat.Bold);
                lastRowStyle.CharacterFormat.BoldBidi = false;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Character format
                firstColumnStyle.CharacterFormat.Bold = false;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, firstColumnStyle.CharacterFormat.Bold);
                firstColumnStyle.CharacterFormat.BoldBidi = false;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstColumnStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Character format
                lastColumnStyle.CharacterFormat.Bold = false;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, lastColumnStyle.CharacterFormat.Bold);
                lastColumnStyle.CharacterFormat.BoldBidi = false;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastColumnStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Odd Column Banding Conditional Formatting Style
                oddColumnBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddColumnBanding);
                #region Character format
                oddColumnBandingStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                oddColumnBandingStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                oddColumnBandingStyle.CellProperties.ForeColor = Color.FromArgb(255, 0, 0, 0);
                oddColumnBandingStyle.CellProperties.TextureStyle = TextureStyle.Texture25Percent;
                #endregion
                #endregion

                #region Even Column Banding Conditional Formatting Style
                evenColumnBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.EvenColumnBanding);
                #region Character format
                evenColumnBandingStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                evenColumnBandingStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                evenColumnBandingStyle.CellProperties.ForeColor = Color.FromArgb(255, 255, 255, 0);
                evenColumnBandingStyle.CellProperties.TextureStyle = TextureStyle.Texture25Percent;
                #endregion
                #endregion

                #region First Row Last Cell Conditional Formatting Style
                firstRowLastCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRowLastCell);
                #region Character format
                firstRowLastCellStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowLastCellStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowLastCellStyle.CharacterFormat.Bold);
                firstRowLastCellStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowLastCellStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowLastCellStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Row First Cell Conditional Formatting Style
                lastRowFirstCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRowFirstCell);
                #region Character format
                lastRowFirstCellStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowFirstCellStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowFirstCellStyle.CharacterFormat.Bold);
                lastRowFirstCellStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowFirstCellStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowFirstCellStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Columns 2.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableColumns2(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle, lastColumnStyle, oddColumnBandingStyle, evenColumnBandingStyle, firstRowLastCellStyle, lastRowFirstCellStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Character format
                (style as WTableStyle).CharacterFormat.Bold = true;
                UpdateComplexProperty((style as WTableStyle).CharacterFormat, WCharacterFormat.BoldKey, (style as WTableStyle).CharacterFormat.Bold);
                (style as WTableStyle).CharacterFormat.BoldBidi = true;
                UpdateComplexProperty((style as WTableStyle).CharacterFormat, WCharacterFormat.BoldBidiKey, (style as WTableStyle).CharacterFormat.BoldBidi);
                #endregion

                #region Table Properties
                (style as WTableStyle).TableProperties.ColumnStripe = 1;
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                firstRowStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstRowStyle.CellProperties.ForeColor = Color.FromArgb(255, 0, 0, 128);
                firstRowStyle.CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Character format
                lastRowStyle.CharacterFormat.Bold = false;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowStyle.CharacterFormat.Bold);
                lastRowStyle.CharacterFormat.BoldBidi = false;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Character format
                firstColumnStyle.CharacterFormat.Bold = false;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, firstColumnStyle.CharacterFormat.Bold);
                firstColumnStyle.CharacterFormat.BoldBidi = false;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstColumnStyle.CharacterFormat.BoldBidi);
                firstColumnStyle.CharacterFormat.TextColor = Color.FromArgb(255, 0, 0, 0);
                #endregion

                #region Table Cell Properties
                firstColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Character format
                lastColumnStyle.CharacterFormat.Bold = false;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, lastColumnStyle.CharacterFormat.Bold);
                lastColumnStyle.CharacterFormat.BoldBidi = false;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastColumnStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Odd Column Banding Conditional Formatting Style
                oddColumnBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddColumnBanding);
                #region Character format
                oddColumnBandingStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                oddColumnBandingStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                oddColumnBandingStyle.CellProperties.ForeColor = Color.FromArgb(255, 0, 0, 0);
                oddColumnBandingStyle.CellProperties.TextureStyle = TextureStyle.Texture30Percent;
                #endregion
                #endregion

                #region Even Column Banding Conditional Formatting Style
                evenColumnBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.EvenColumnBanding);
                #region Character format
                evenColumnBandingStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                evenColumnBandingStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                evenColumnBandingStyle.CellProperties.ForeColor = Color.FromArgb(255, 0, 255, 0);
                evenColumnBandingStyle.CellProperties.TextureStyle = TextureStyle.Texture25Percent;
                #endregion
                #endregion

                #region First Row Last Cell Conditional Formatting Style
                firstRowLastCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRowLastCell);
                #region Character format
                firstRowLastCellStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowLastCellStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowLastCellStyle.CharacterFormat.Bold);
                firstRowLastCellStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowLastCellStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowLastCellStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Row First Cell Conditional Formatting Style
                lastRowFirstCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRowFirstCell);
                #region Character format
                lastRowFirstCellStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowFirstCellStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowFirstCellStyle.CharacterFormat.Bold);
                lastRowFirstCellStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowFirstCellStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowFirstCellStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Columns 3.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableColumns3(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle, lastColumnStyle, oddColumnBandingStyle, evenColumnBandingStyle, firstRowLastCellStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Character format
                (style as WTableStyle).CharacterFormat.Bold = true;
                UpdateComplexProperty((style as WTableStyle).CharacterFormat, WCharacterFormat.BoldKey, (style as WTableStyle).CharacterFormat.Bold);
                (style as WTableStyle).CharacterFormat.BoldBidi = true;
                UpdateComplexProperty((style as WTableStyle).CharacterFormat, WCharacterFormat.BoldBidiKey, (style as WTableStyle).CharacterFormat.BoldBidi);
                #endregion

                #region Table Properties
                (style as WTableStyle).TableProperties.ColumnStripe = 1;
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 128);
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 128);
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = Color.FromArgb(255, 0, 0, 128);
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = Color.FromArgb(255, 0, 0, 128);
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Vertical.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Vertical.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Vertical.Color = Color.FromArgb(255, 0, 0, 128);
                (style as WTableStyle).TableProperties.Borders.Vertical.Space = 0;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                firstRowStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstRowStyle.CellProperties.ForeColor = Color.FromArgb(255, 0, 0, 128);
                firstRowStyle.CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Character format
                lastRowStyle.CharacterFormat.Bold = false;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowStyle.CharacterFormat.Bold);
                lastRowStyle.CharacterFormat.BoldBidi = false;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = .75f;
                lastRowStyle.CellProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 128);
                lastRowStyle.CellProperties.Borders.Top.Space = 0;

                lastRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Character format
                firstColumnStyle.CharacterFormat.Bold = false;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, firstColumnStyle.CharacterFormat.Bold);
                firstColumnStyle.CharacterFormat.BoldBidi = false;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstColumnStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Character format
                lastColumnStyle.CharacterFormat.Bold = false;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, lastColumnStyle.CharacterFormat.Bold);
                lastColumnStyle.CharacterFormat.BoldBidi = false;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastColumnStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Odd Column Banding Conditional Formatting Style
                oddColumnBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddColumnBanding);
                #region Character format
                oddColumnBandingStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                oddColumnBandingStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                oddColumnBandingStyle.CellProperties.ForeColor = Color.FromArgb(255, 192, 192, 192);
                oddColumnBandingStyle.CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion
                #endregion

                #region Even Column Banding Conditional Formatting Style
                evenColumnBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.EvenColumnBanding);
                #region Character format
                evenColumnBandingStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                evenColumnBandingStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                evenColumnBandingStyle.CellProperties.ForeColor = Color.FromArgb(255, 0, 0, 0);
                evenColumnBandingStyle.CellProperties.TextureStyle = TextureStyle.Texture10Percent;
                #endregion
                #endregion

                #region First Row Last Cell Conditional Formatting Style
                firstRowLastCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRowLastCell);
                #region Character format
                firstRowLastCellStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowLastCellStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowLastCellStyle.CharacterFormat.Bold);
                firstRowLastCellStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowLastCellStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowLastCellStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Columns 4.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableColumns4(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, lastColumnStyle, oddColumnBandingStyle, evenColumnBandingStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.ColumnStripe = 1;
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                firstRowStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstRowStyle.CellProperties.ForeColor = Color.FromArgb(255, 0, 0, 0);
                firstRowStyle.CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Character format
                lastRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowStyle.CharacterFormat.Bold);
                lastRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Character format
                lastColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, lastColumnStyle.CharacterFormat.Bold);
                lastColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastColumnStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Odd Column Banding Conditional Formatting Style
                oddColumnBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddColumnBanding);
                #region Character format
                oddColumnBandingStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                oddColumnBandingStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                oddColumnBandingStyle.CellProperties.ForeColor = Color.FromArgb(255, 0, 128, 128);
                oddColumnBandingStyle.CellProperties.TextureStyle = TextureStyle.Texture50Percent;
                #endregion
                #endregion

                #region Even Column Banding Conditional Formatting Style
                evenColumnBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.EvenColumnBanding);
                #region Character format
                evenColumnBandingStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                evenColumnBandingStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                evenColumnBandingStyle.CellProperties.ForeColor = Color.FromArgb(255, 0, 0, 0);
                evenColumnBandingStyle.CellProperties.TextureStyle = TextureStyle.Texture10Percent;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Columns 5.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableColumns5(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle, lastColumnStyle, oddColumnBandingStyle, evenColumnBandingStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.ColumnStripe = 1;
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.FromArgb(255, 128, 128, 128);
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.FromArgb(255, 128, 128, 128);
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = Color.FromArgb(255, 128, 128, 128);
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = Color.FromArgb(255, 128, 128, 128);
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Vertical.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Vertical.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Vertical.Color = Color.FromArgb(255, 192, 192, 192);
                (style as WTableStyle).TableProperties.Borders.Vertical.Space = 0;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                firstRowStyle.CharacterFormat.Italic = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.ItalicKey, firstRowStyle.CharacterFormat.Italic);
                firstRowStyle.CharacterFormat.ItalicBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.ItalicBidiKey, firstRowStyle.CharacterFormat.ItalicBidi);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 128, 128, 128);
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = .75f;

                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Character format
                lastRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowStyle.CharacterFormat.Bold);
                lastRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Top.Color = Color.FromArgb(255, 128, 128, 128);
                lastRowStyle.CellProperties.Borders.Top.Space = 0;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = .75f;

                lastRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Character format
                firstColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, firstColumnStyle.CharacterFormat.Bold);
                firstColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstColumnStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Character format
                lastColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, lastColumnStyle.CharacterFormat.Bold);
                lastColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastColumnStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Odd Column Banding Conditional Formatting Style
                oddColumnBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddColumnBanding);
                #region Character format
                oddColumnBandingStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                oddColumnBandingStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                oddColumnBandingStyle.CellProperties.ForeColor = Color.FromArgb(255, 192, 192, 192);
                oddColumnBandingStyle.CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion
                #endregion

                #region Even Column Banding Conditional Formatting Style
                evenColumnBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.EvenColumnBanding);
                #region Character format
                evenColumnBandingStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Contemporary.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableContemporary(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, oddRowBandingStyle, evenRowBandingStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.RowStripe = 1;
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Horizontal.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Horizontal.LineWidth = 2.25f;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Color = Color.FromArgb(255, 255, 255, 255);
                (style as WTableStyle).TableProperties.Borders.Horizontal.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Vertical.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Vertical.LineWidth = 2.25f;
                (style as WTableStyle).TableProperties.Borders.Vertical.Color = Color.FromArgb(255, 255, 255, 255);
                (style as WTableStyle).TableProperties.Borders.Vertical.Space = 0;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                firstRowStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                firstRowStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstRowStyle.CellProperties.ForeColor = Color.FromArgb(255, 0, 0, 0);
                firstRowStyle.CellProperties.TextureStyle = TextureStyle.Texture20Percent;
                #endregion
                #endregion
                
                #region Odd Row Banding Conditional Formatting Style
                oddRowBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddRowBanding);
                #region Character format
                oddRowBandingStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                oddRowBandingStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                oddRowBandingStyle.CellProperties.ForeColor = Color.FromArgb(255, 0, 0, 0);
                oddRowBandingStyle.CellProperties.TextureStyle = TextureStyle.Texture5Percent;
                #endregion
                #endregion

                #region Even Column Banding Conditional Formatting Style
                evenRowBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.EvenRowBanding);
                #region Character format
                evenRowBandingStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                evenRowBandingStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                evenRowBandingStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                evenRowBandingStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                evenRowBandingStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                evenRowBandingStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                evenRowBandingStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                evenRowBandingStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                evenRowBandingStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                evenRowBandingStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                evenRowBandingStyle.CellProperties.ForeColor = Color.FromArgb(255, 0, 0, 0);
                evenRowBandingStyle.CellProperties.TextureStyle = TextureStyle.Texture20Percent;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Elegant.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableElegant(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Double;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Double;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Double;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Double;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Horizontal.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Horizontal.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Horizontal.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Vertical.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Vertical.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Vertical.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Vertical.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.AllCaps = true;
                firstRowStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Grid 1.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableGrid1(IStyle style)
            {
                ConditionalFormattingStyle lastRowStyle, lastColumnStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Horizontal.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Horizontal.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Horizontal.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Vertical.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Vertical.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Vertical.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Vertical.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion

                #region Conditional Formatting Properties
                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Character format
                lastRowStyle.CharacterFormat.Italic = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.ItalicKey, lastRowStyle.CharacterFormat.Italic);
                lastRowStyle.CharacterFormat.ItalicBidi = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.ItalicBidiKey, lastRowStyle.CharacterFormat.ItalicBidi);
                #endregion

                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                
                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Character format
                lastColumnStyle.CharacterFormat.Italic = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.ItalicKey, lastColumnStyle.CharacterFormat.Italic);
                lastColumnStyle.CharacterFormat.ItalicBidi = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.ItalicBidiKey, lastColumnStyle.CharacterFormat.ItalicBidi);
                #endregion

                #region Table Cell Properties
                lastColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Grid 2.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableGrid2(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle, lastColumnStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Horizontal.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Horizontal.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Horizontal.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Vertical.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Vertical.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Vertical.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Vertical.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Character format
                lastRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowStyle.CharacterFormat.Bold);
                lastRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                lastRowStyle.CellProperties.Borders.Top.Space = 0;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = .75f;

                lastRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Character format
                firstColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, firstColumnStyle.CharacterFormat.Bold);
                firstColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstColumnStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Character format
                lastColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, lastColumnStyle.CharacterFormat.Bold);
                lastColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastColumnStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Grid 3.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableGrid3(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, lastColumnStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Vertical.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Vertical.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Vertical.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Vertical.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = .75f;

                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                firstRowStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstRowStyle.CellProperties.ForeColor = Color.FromArgb(255, 255, 255, 0);
                firstRowStyle.CellProperties.TextureStyle = TextureStyle.Texture30Percent;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Character format
                lastRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowStyle.CharacterFormat.Bold);
                lastRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                
                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Character format
                lastColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, lastColumnStyle.CharacterFormat.Bold);
                lastColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastColumnStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Grid 4.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableGrid4(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, lastColumnStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Horizontal.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Horizontal.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Horizontal.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Vertical.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Vertical.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Vertical.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Vertical.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = .75f;

                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                firstRowStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstRowStyle.CellProperties.ForeColor = Color.FromArgb(255, 255, 255, 0);
                firstRowStyle.CellProperties.TextureStyle = TextureStyle.Texture30Percent;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Character format
                lastRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowStyle.CharacterFormat.Bold);
                lastRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowStyle.CharacterFormat.BoldBidi);
                lastRowStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                lastRowStyle.CellProperties.Borders.Top.Space = 0;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = .75f;

                lastRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                lastRowStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                lastRowStyle.CellProperties.ForeColor = Color.FromArgb(255, 255, 255, 0);
                lastRowStyle.CellProperties.TextureStyle = TextureStyle.Texture30Percent;
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Character format
                lastColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, lastColumnStyle.CharacterFormat.Bold);
                lastColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastColumnStyle.CharacterFormat.BoldBidi);
                lastColumnStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                lastColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Grid 5.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableGrid5(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, lastColumnStyle, firstRowFirstCellStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Horizontal.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Horizontal.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Horizontal.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Vertical.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Vertical.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Vertical.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Vertical.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = 1.5f;

                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Character format
                lastRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowStyle.CharacterFormat.Bold);
                lastRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Character format
                lastColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, lastColumnStyle.CharacterFormat.Bold);
                lastColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastColumnStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region First Row First Cell Conditional Formatting Style
                firstRowFirstCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRowFirstCell);
                #region Table Cell Properties
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.Single;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.FromArgb(255, 0, 0, 0);
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = .75f;

                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Grid 6.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableGrid6(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle, firstRowFirstCellStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Vertical.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Vertical.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Vertical.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Vertical.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = .75f;

                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Character format
                lastRowStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                lastRowStyle.CellProperties.Borders.Top.Space = 0;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = .75f;

                lastRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Character format
                firstColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, firstColumnStyle.CharacterFormat.Bold);
                firstColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstColumnStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region First Row First Cell Conditional Formatting Style
                firstRowFirstCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRowFirstCell);
                #region Table Cell Properties
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.Single;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.FromArgb(255, 0, 0, 0);
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = .75f;

                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Grid 7.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableGrid7(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle, lastColumnStyle, firstRowFirstCellStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Character format
                (style as WTableStyle).CharacterFormat.Bold = true;
                UpdateComplexProperty((style as WTableStyle).CharacterFormat, WCharacterFormat.BoldKey, (style as WTableStyle).CharacterFormat.Bold);
                (style as WTableStyle).CharacterFormat.BoldBidi = true;
                UpdateComplexProperty((style as WTableStyle).CharacterFormat, WCharacterFormat.BoldBidiKey, (style as WTableStyle).CharacterFormat.BoldBidi);
                #endregion

                #region Table Properties
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Horizontal.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Horizontal.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Horizontal.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Vertical.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Vertical.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Vertical.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Vertical.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = false;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = false;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = 1.5f;

                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Character format
                lastRowStyle.CharacterFormat.Bold = false;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowStyle.CharacterFormat.Bold);
                lastRowStyle.CharacterFormat.BoldBidi = false;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                lastRowStyle.CellProperties.Borders.Top.Space = 0;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = .75f;

                lastRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Character format
                firstColumnStyle.CharacterFormat.Bold = false;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, firstColumnStyle.CharacterFormat.Bold);
                firstColumnStyle.CharacterFormat.BoldBidi = false;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstColumnStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Character format
                lastColumnStyle.CharacterFormat.Bold = false;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, lastColumnStyle.CharacterFormat.Bold);
                lastColumnStyle.CharacterFormat.BoldBidi = false;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastColumnStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region First Row First Cell Conditional Formatting Style
                firstRowFirstCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRowFirstCell);
                #region Table Cell Properties
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.Single;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.FromArgb(255, 0, 0, 0);
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = .75f;

                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowFirstCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Grid 8.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableGrid8(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, lastColumnStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 128);
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 128);
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = Color.FromArgb(255, 0, 0, 128);
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = Color.FromArgb(255, 0, 0, 128);
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Horizontal.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Horizontal.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Color = Color.FromArgb(255, 0, 0, 128);
                (style as WTableStyle).TableProperties.Borders.Horizontal.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Vertical.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Vertical.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Vertical.Color = Color.FromArgb(255, 0, 0, 128);
                (style as WTableStyle).TableProperties.Borders.Vertical.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                firstRowStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                firstRowStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstRowStyle.CellProperties.ForeColor = Color.FromArgb(255, 0, 0, 128);
                firstRowStyle.CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Character format
                lastRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowStyle.CharacterFormat.Bold);
                lastRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowStyle.CharacterFormat.BoldBidi);
                lastRowStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Character format
                lastColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, lastColumnStyle.CharacterFormat.Bold);
                lastColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastColumnStyle.CharacterFormat.BoldBidi);
                lastColumnStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                lastColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table List 1.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableList1(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, oddRowBandingStyle, evenRowBandingStyle, lastRowFirstCellStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.RowStripe = 1;
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.FromArgb(255, 0, 128, 128);
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 128, 128);
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = Color.FromArgb(255, 0, 128, 128);
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = Color.FromArgb(255, 0, 128, 128);
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                firstRowStyle.CharacterFormat.Italic = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.ItalicKey, firstRowStyle.CharacterFormat.Italic);
                firstRowStyle.CharacterFormat.ItalicBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.ItalicBidiKey, firstRowStyle.CharacterFormat.ItalicBidi);
                firstRowStyle.CharacterFormat.TextColor = Color.FromArgb(255, 128, 0, 0);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = .75f;

                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                firstRowStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstRowStyle.CellProperties.ForeColor = Color.FromArgb(255, 192, 192, 192);
                firstRowStyle.CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                lastRowStyle.CellProperties.Borders.Top.Space = 0;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = .75f;

                lastRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Odd Row Banding Conditional Formatting Style
                oddRowBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddRowBanding);
                #region Character format
                oddRowBandingStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                oddRowBandingStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                oddRowBandingStyle.CellProperties.ForeColor = Color.FromArgb(255, 192, 192, 192);
                oddRowBandingStyle.CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion
                #endregion

                #region Even Row Banding Conditional Formatting Style
                evenRowBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.EvenRowBanding);
                #region Character format
                evenRowBandingStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                evenRowBandingStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                evenRowBandingStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                evenRowBandingStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                evenRowBandingStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                evenRowBandingStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                evenRowBandingStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                evenRowBandingStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                evenRowBandingStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Row First Cell Conditional Formatting Style
                lastRowFirstCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRowFirstCell);
                #region Character format
                lastRowFirstCellStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowFirstCellStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowFirstCellStyle.CharacterFormat.Bold);
                lastRowFirstCellStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowFirstCellStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowFirstCellStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table List 2.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableList2(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, oddRowBandingStyle, evenRowBandingStyle, lastRowFirstCellStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.RowStripe = 2;
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.FromArgb(255, 128, 128, 128);
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                firstRowStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = .75f;

                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                firstRowStyle.CellProperties.BackColor = Color.FromArgb(255, 0, 128, 0);
                firstRowStyle.CellProperties.ForeColor = Color.FromArgb(255, 0, 128, 128);
                firstRowStyle.CellProperties.TextureStyle = TextureStyle.Texture75Percent;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                lastRowStyle.CellProperties.Borders.Top.Space = 0;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = .75f;

                lastRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Odd Row Banding Conditional Formatting Style
                oddRowBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddRowBanding);
                #region Character format
                oddRowBandingStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                oddRowBandingStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                oddRowBandingStyle.CellProperties.ForeColor = Color.FromArgb(255, 0, 255, 0);
                oddRowBandingStyle.CellProperties.TextureStyle = TextureStyle.Texture20Percent;
                #endregion
                #endregion

                #region Even Row Banding Conditional Formatting Style
                evenRowBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.EvenRowBanding);
                #region Character format
                evenRowBandingStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                evenRowBandingStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                evenRowBandingStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                evenRowBandingStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                evenRowBandingStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                evenRowBandingStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                evenRowBandingStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                evenRowBandingStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                evenRowBandingStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Row First Cell Conditional Formatting Style
                lastRowFirstCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRowFirstCell);
                #region Character format
                lastRowFirstCellStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowFirstCellStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowFirstCellStyle.CharacterFormat.Bold);
                lastRowFirstCellStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowFirstCellStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowFirstCellStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table List 3.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableList3(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, lastRowFirstCellStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Horizontal.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Horizontal.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Horizontal.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                firstRowStyle.CharacterFormat.TextColor = Color.FromArgb(255, 0, 0, 128);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = 1.5f;

                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                lastRowStyle.CellProperties.Borders.Top.Space = 0;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = 1.5f;

                lastRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Row First Cell Conditional Formatting Style
                lastRowFirstCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRowFirstCell);
                #region Character format
                lastRowFirstCellStyle.CharacterFormat.Italic = true;
                UpdateComplexProperty(lastRowFirstCellStyle.CharacterFormat, WCharacterFormat.ItalicKey, lastRowFirstCellStyle.CharacterFormat.Italic);
                lastRowFirstCellStyle.CharacterFormat.ItalicBidi = true;
                UpdateComplexProperty(lastRowFirstCellStyle.CharacterFormat, WCharacterFormat.ItalicBidiKey, lastRowFirstCellStyle.CharacterFormat.ItalicBidi);
                lastRowFirstCellStyle.CharacterFormat.TextColor = Color.FromArgb(255, 0, 0, 128);
                #endregion

                #region Table Cell Properties
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table List 4.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableList4(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Horizontal.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Horizontal.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Horizontal.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                firstRowStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = 1.5f;

                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                firstRowStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstRowStyle.CellProperties.ForeColor = Color.FromArgb(255, 128, 128, 128);
                firstRowStyle.CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table List 5.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableList5(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, firstColumnStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Horizontal.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Horizontal.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Horizontal.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = 1.5f;

                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Character format
                firstColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, firstColumnStyle.CharacterFormat.Bold);
                firstColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstColumnStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table List 6.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableList6(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, firstColumnStyle, oddRowBandingStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.RowStripe = 1;
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.Texture50Percent;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = 1.5f;

                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Character format
                firstColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, firstColumnStyle.CharacterFormat.Bold);
                firstColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstColumnStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstColumnStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Single;
                firstColumnStyle.CellProperties.Borders.Right.Color = Color.FromArgb(255, 0, 0, 0);
                firstColumnStyle.CellProperties.Borders.Right.Space = 0;
                firstColumnStyle.CellProperties.Borders.Right.LineWidth = 1.5f;

                firstColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                
                #region Odd Row Banding Conditional Formatting Style
                oddRowBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddRowBanding);
                #region Table Cell Properties
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                oddRowBandingStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                oddRowBandingStyle.CellProperties.ForeColor = Color.FromArgb(255, 0, 0, 0);
                oddRowBandingStyle.CellProperties.TextureStyle = TextureStyle.Texture25Percent;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table List 7.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableList7(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle, lastColumnStyle, oddRowBandingStyle, evenRowBandingStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.RowStripe = 1;
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.FromArgb(255, 0, 128, 0);
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 128, 0);
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = Color.FromArgb(255, 0, 128, 0);
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = Color.FromArgb(255, 0, 128, 0);
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Horizontal.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Horizontal.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Horizontal.Space = 0;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 128, 0);
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = 1.5f;

                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                firstRowStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstRowStyle.CellProperties.ForeColor = Color.FromArgb(255, 192, 192, 192);
                firstRowStyle.CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Character format
                lastRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowStyle.CharacterFormat.Bold);
                lastRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Top.Color = Color.FromArgb(255, 0, 128, 0);
                lastRowStyle.CellProperties.Borders.Top.Space = 0;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = 1.5f;

                lastRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Character format
                firstColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, firstColumnStyle.CharacterFormat.Bold);
                firstColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstColumnStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Character format
                lastColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, lastColumnStyle.CharacterFormat.Bold);
                lastColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastColumnStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Odd Row Banding Conditional Formatting Style
                oddRowBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddRowBanding);
                #region Character format
                oddRowBandingStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                oddRowBandingStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                oddRowBandingStyle.CellProperties.ForeColor = Color.FromArgb(255, 0, 0, 0);
                oddRowBandingStyle.CellProperties.TextureStyle = TextureStyle.Texture20Percent;
                #endregion
                #endregion

                #region Even Row Banding Conditional Formatting Style
                evenRowBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.EvenRowBanding);
                #region Table Cell Properties
                evenRowBandingStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                evenRowBandingStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                evenRowBandingStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                evenRowBandingStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                evenRowBandingStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                evenRowBandingStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                evenRowBandingStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                evenRowBandingStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                evenRowBandingStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                evenRowBandingStyle.CellProperties.ForeColor = Color.FromArgb(255, 255, 255, 0);
                evenRowBandingStyle.CellProperties.TextureStyle = TextureStyle.Texture25Percent;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table List 8.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableList8(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle, lastColumnStyle, oddRowBandingStyle, evenRowBandingStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.RowStripe = 1;
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Vertical.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Vertical.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Vertical.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Vertical.Space = 0;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                firstRowStyle.CharacterFormat.Italic = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.ItalicKey, firstRowStyle.CharacterFormat.Italic);
                firstRowStyle.CharacterFormat.ItalicBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.ItalicBidiKey, firstRowStyle.CharacterFormat.ItalicBidi);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = .75f;

                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                firstRowStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstRowStyle.CellProperties.ForeColor = Color.FromArgb(255, 255, 255, 0);
                firstRowStyle.CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Character format
                lastRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowStyle.CharacterFormat.Bold);
                lastRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                lastRowStyle.CellProperties.Borders.Top.Space = 0;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = .75f;

                lastRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Character format
                firstColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, firstColumnStyle.CharacterFormat.Bold);
                firstColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstColumnStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Character format
                lastColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, lastColumnStyle.CharacterFormat.Bold);
                lastColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastColumnStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Odd Row Banding Conditional Formatting Style
                oddRowBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddRowBanding);
                #region Character format
                oddRowBandingStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                oddRowBandingStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                oddRowBandingStyle.CellProperties.ForeColor = Color.FromArgb(255, 255, 255, 0);
                oddRowBandingStyle.CellProperties.TextureStyle = TextureStyle.Texture25Percent;
                #endregion
                #endregion

                #region Even Row Banding Conditional Formatting Style
                evenRowBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.EvenRowBanding);
                #region Table Cell Properties
                evenRowBandingStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                evenRowBandingStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                evenRowBandingStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                evenRowBandingStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                evenRowBandingStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                evenRowBandingStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                evenRowBandingStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                evenRowBandingStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                evenRowBandingStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                evenRowBandingStyle.CellProperties.ForeColor = Color.FromArgb(255, 255, 0, 0);
                evenRowBandingStyle.CellProperties.TextureStyle = TextureStyle.Texture50Percent;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Professional.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableProfessional(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Horizontal.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Horizontal.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Horizontal.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Vertical.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Vertical.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Vertical.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Vertical.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                firstRowStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                firstRowStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstRowStyle.CellProperties.ForeColor = Color.FromArgb(255, 0, 0, 0);
                firstRowStyle.CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Simple 1.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableSimple1(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.FromArgb(255, 0, 128, 0);
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 128, 0);
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 128, 0);
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = .75f;

                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Top.Color = Color.FromArgb(255, 0, 128, 0);
                lastRowStyle.CellProperties.Borders.Top.Space = 0;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = .75f;

                lastRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Simple 2.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableSimple2(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle, lastColumnStyle, firstRowLastCellStyle, lastRowFirstCellStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = 1.5f;

                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Character format
                lastRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowStyle.CharacterFormat.Bold);
                lastRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowStyle.CharacterFormat.BoldBidi);
                lastRowStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                lastRowStyle.CellProperties.Borders.Top.Space = 0;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = .75f;

                lastRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Character format
                firstColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, firstColumnStyle.CharacterFormat.Bold);
                firstColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstColumnStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstColumnStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Single;
                firstColumnStyle.CellProperties.Borders.Right.Color = Color.FromArgb(255, 0, 0, 0);
                firstColumnStyle.CellProperties.Borders.Right.Space = 0;
                firstColumnStyle.CellProperties.Borders.Right.LineWidth = 1.5f;

                firstColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Character format
                lastColumnStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldKey, lastColumnStyle.CharacterFormat.Bold);
                lastColumnStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastColumnStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastColumnStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastColumnStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Single;
                lastColumnStyle.CellProperties.Borders.Left.Color = Color.FromArgb(255, 0, 0, 0);
                lastColumnStyle.CellProperties.Borders.Left.Space = 0;
                lastColumnStyle.CellProperties.Borders.Left.LineWidth = .75f;

                lastColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region First Row Last Cell Conditional Formatting Style
                firstRowLastCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRowLastCell);
                #region Character format
                firstRowLastCellStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowLastCellStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowLastCellStyle.CharacterFormat.Bold);
                firstRowLastCellStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowLastCellStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowLastCellStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstRowLastCellStyle.CellProperties.Borders.Left.BorderType = BorderStyle.None;
                firstRowLastCellStyle.CellProperties.Borders.Left.Color = Color.Black;
                firstRowLastCellStyle.CellProperties.Borders.Left.Space = 0;
                firstRowLastCellStyle.CellProperties.Borders.Left.LineWidth = 0;

                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Row First Cell Conditional Formatting Style
                lastRowFirstCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRowFirstCell);
                #region Character format
                lastRowFirstCellStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowFirstCellStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowFirstCellStyle.CharacterFormat.Bold);
                lastRowFirstCellStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowFirstCellStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowFirstCellStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastRowFirstCellStyle.CellProperties.Borders.Top.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.Top.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.Top.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.Top.LineWidth = 0;

                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Simple 3.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableSimple3(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = 1.5f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowStyle.CharacterFormat.Bold);
                firstRowStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowStyle.CharacterFormat.BoldBidi);
                firstRowStyle.CharacterFormat.TextColor = Color.FromArgb(255, 255, 255, 255);
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                firstRowStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstRowStyle.CellProperties.ForeColor = Color.FromArgb(255, 0, 0, 0);
                firstRowStyle.CellProperties.TextureStyle = TextureStyle.TextureSolid;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Subtle 1.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableSubtle1(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle, lastColumnStyle, oddRowBandingStyle, firstRowLastCellStyle, lastRowFirstCellStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.RowStripe = 1;
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                firstRowStyle.CellProperties.Borders.Top.Space = 0;
                firstRowStyle.CellProperties.Borders.Top.LineWidth = .75f;

                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = 1.5f;

                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                lastRowStyle.CellProperties.Borders.Top.Space = 0;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = 1.5f;
                
                lastRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                lastRowStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                lastRowStyle.CellProperties.ForeColor = Color.FromArgb(255, 128, 0, 128);
                lastRowStyle.CellProperties.TextureStyle = TextureStyle.Texture25Percent;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Table Cell Properties
                firstColumnStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Single;
                firstColumnStyle.CellProperties.Borders.Right.Color = Color.FromArgb(255, 0, 0, 0);
                firstColumnStyle.CellProperties.Borders.Right.Space = 0;
                firstColumnStyle.CellProperties.Borders.Right.LineWidth = 1.5f;

                firstColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Table Cell Properties
                lastColumnStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Single;
                lastColumnStyle.CellProperties.Borders.Left.Color = Color.FromArgb(255, 0, 0, 0);
                lastColumnStyle.CellProperties.Borders.Left.Space = 0;
                lastColumnStyle.CellProperties.Borders.Left.LineWidth = 1.5f;

                lastColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Odd Row Banding Conditional Formatting Style
                oddRowBandingStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.OddRowBanding);
                #region Table Cell Properties
                oddRowBandingStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                oddRowBandingStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                oddRowBandingStyle.CellProperties.Borders.Bottom.Space = 0;
                oddRowBandingStyle.CellProperties.Borders.Bottom.LineWidth = .75f;

                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                oddRowBandingStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                oddRowBandingStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                oddRowBandingStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                oddRowBandingStyle.CellProperties.ForeColor = Color.FromArgb(255, 128, 128, 0);
                oddRowBandingStyle.CellProperties.TextureStyle = TextureStyle.Texture25Percent;
                #endregion
                #endregion

                #region First Row Last Cell Conditional Formatting Style
                firstRowLastCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRowLastCell);
                #region Character format
                firstRowLastCellStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowLastCellStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowLastCellStyle.CharacterFormat.Bold);
                firstRowLastCellStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowLastCellStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowLastCellStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Row First Cell Conditional Formatting Style
                lastRowFirstCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRowFirstCell);
                #region Character format
                lastRowFirstCellStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowFirstCellStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowFirstCellStyle.CharacterFormat.Bold);
                lastRowFirstCellStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowFirstCellStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowFirstCellStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Subtle 2.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableSubtle2(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle, lastRowStyle, firstColumnStyle, lastColumnStyle, firstRowLastCellStyle, lastRowFirstCellStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = Color.FromArgb(255, 0, 0, 0);
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                firstRowStyle.CellProperties.Borders.Bottom.Color = Color.FromArgb(255, 0, 0, 0);
                firstRowStyle.CellProperties.Borders.Bottom.Space = 0;
                firstRowStyle.CellProperties.Borders.Bottom.LineWidth = 1.5f;

                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Row Conditional Formatting Style
                lastRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRow);
                #region Table Cell Properties
                lastRowStyle.CellProperties.Borders.Top.BorderType = BorderStyle.Single;
                lastRowStyle.CellProperties.Borders.Top.Color = Color.FromArgb(255, 0, 0, 0);
                lastRowStyle.CellProperties.Borders.Top.Space = 0;
                lastRowStyle.CellProperties.Borders.Top.LineWidth = 1.5f;

                lastRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region First Column Conditional Formatting Style
                firstColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstColumn);
                #region Table Cell Properties
                firstColumnStyle.CellProperties.Borders.Right.BorderType = BorderStyle.Single;
                firstColumnStyle.CellProperties.Borders.Right.Color = Color.FromArgb(255, 0, 0, 0);
                firstColumnStyle.CellProperties.Borders.Right.Space = 0;
                firstColumnStyle.CellProperties.Borders.Right.LineWidth = 1.5f;

                firstColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                firstColumnStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                firstColumnStyle.CellProperties.ForeColor = Color.FromArgb(255, 0, 128, 0);
                firstColumnStyle.CellProperties.TextureStyle = TextureStyle.Texture25Percent;
                #endregion
                #endregion

                #region Last Column Conditional Formatting Style
                lastColumnStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastColumn);
                #region Table Cell Properties
                lastColumnStyle.CellProperties.Borders.Left.BorderType = BorderStyle.Single;
                lastColumnStyle.CellProperties.Borders.Left.Color = Color.FromArgb(255, 0, 0, 0);
                lastColumnStyle.CellProperties.Borders.Left.Space = 0;
                lastColumnStyle.CellProperties.Borders.Left.LineWidth = 1.5f;

                lastColumnStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastColumnStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastColumnStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;

                lastColumnStyle.CellProperties.BackColor = Color.FromArgb(255, 255, 255, 255);
                lastColumnStyle.CellProperties.ForeColor = Color.FromArgb(255, 128, 128, 0);
                lastColumnStyle.CellProperties.TextureStyle = TextureStyle.Texture25Percent;
                #endregion
                #endregion

                #region First Row Last Cell Conditional Formatting Style
                firstRowLastCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRowLastCell);
                #region Character format
                firstRowLastCellStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(firstRowLastCellStyle.CharacterFormat, WCharacterFormat.BoldKey, firstRowLastCellStyle.CharacterFormat.Bold); 
                firstRowLastCellStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(firstRowLastCellStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, firstRowLastCellStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowLastCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion

                #region Last Row First Cell Conditional Formatting Style
                lastRowFirstCellStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.LastRowFirstCell);
                #region Character format
                lastRowFirstCellStyle.CharacterFormat.Bold = true;
                UpdateComplexProperty(lastRowFirstCellStyle.CharacterFormat, WCharacterFormat.BoldKey, lastRowFirstCellStyle.CharacterFormat.Bold);
                lastRowFirstCellStyle.CharacterFormat.BoldBidi = true;
                UpdateComplexProperty(lastRowFirstCellStyle.CharacterFormat, WCharacterFormat.BoldBidiKey, lastRowFirstCellStyle.CharacterFormat.BoldBidi);
                #endregion

                #region Table Cell Properties
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                lastRowFirstCellStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Theme.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableTheme(IStyle style)
            {
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = .5f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.Black;
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = .5f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.Black;
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = .5f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = Color.Black;
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = .5f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = Color.Black;
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Horizontal.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Horizontal.LineWidth = .5f;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Color = Color.Black;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Vertical.BorderType = BorderStyle.Single;
                (style as WTableStyle).TableProperties.Borders.Vertical.LineWidth = .5f;
                (style as WTableStyle).TableProperties.Borders.Vertical.Color = Color.Black;
                (style as WTableStyle).TableProperties.Borders.Vertical.Space = 0;
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Web 1.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableWeb1(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.CellSpacing = 1;
                (style as WTableStyle).TableProperties.LeftIndent = 0;
                
                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Outset;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.Black;
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Outset;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.Black;
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Outset;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = Color.Black;
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Outset;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = Color.Black;
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Horizontal.BorderType = BorderStyle.Outset;
                (style as WTableStyle).TableProperties.Borders.Horizontal.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Color = Color.Black;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Vertical.BorderType = BorderStyle.Outset;
                (style as WTableStyle).TableProperties.Borders.Vertical.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Vertical.Color = Color.Black;
                (style as WTableStyle).TableProperties.Borders.Vertical.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion

                #region Table Row Properties
                (style as WTableStyle).RowProperties.CellSpacing = 1;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Web 2.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableWeb2(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.CellSpacing = 1;
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Inset;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.Black;
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Inset;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.Black;
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Inset;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Left.Color = Color.Black;
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Inset;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Right.Color = Color.Black;
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Horizontal.BorderType = BorderStyle.Inset;
                (style as WTableStyle).TableProperties.Borders.Horizontal.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Color = Color.Black;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Vertical.BorderType = BorderStyle.Inset;
                (style as WTableStyle).TableProperties.Borders.Vertical.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Vertical.Color = Color.Black;
                (style as WTableStyle).TableProperties.Borders.Vertical.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion

                #region Table Row Properties
                (style as WTableStyle).RowProperties.CellSpacing = 1;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Loads the table style Table Web 3.
            /// </summary>
            /// <param name="style">The style.</param>
            private static void LoadStyleTableWeb3(IStyle style)
            {
                ConditionalFormattingStyle firstRowStyle;
                (style as Style).UnhideWhenUsed = true;
                #region Table Properties
                (style as WTableStyle).TableProperties.CellSpacing = 1;
                (style as WTableStyle).TableProperties.LeftIndent = 0;

                (style as WTableStyle).TableProperties.Paddings.Top = 0;
                (style as WTableStyle).TableProperties.Paddings.Bottom = 0;
                (style as WTableStyle).TableProperties.Paddings.Left = 5.4f;
                (style as WTableStyle).TableProperties.Paddings.Right = 5.4f;

                (style as WTableStyle).TableProperties.Borders.Top.BorderType = BorderStyle.Outset;
                (style as WTableStyle).TableProperties.Borders.Top.LineWidth = 3;
                (style as WTableStyle).TableProperties.Borders.Top.Color = Color.Black;
                (style as WTableStyle).TableProperties.Borders.Top.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Bottom.BorderType = BorderStyle.Outset;
                (style as WTableStyle).TableProperties.Borders.Bottom.LineWidth = 3;
                (style as WTableStyle).TableProperties.Borders.Bottom.Color = Color.Black;
                (style as WTableStyle).TableProperties.Borders.Bottom.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Left.BorderType = BorderStyle.Outset;
                (style as WTableStyle).TableProperties.Borders.Left.LineWidth = 3;
                (style as WTableStyle).TableProperties.Borders.Left.Color = Color.Black;
                (style as WTableStyle).TableProperties.Borders.Left.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Right.BorderType = BorderStyle.Outset;
                (style as WTableStyle).TableProperties.Borders.Right.LineWidth = 3;
                (style as WTableStyle).TableProperties.Borders.Right.Color = Color.Black;
                (style as WTableStyle).TableProperties.Borders.Right.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Horizontal.BorderType = BorderStyle.Outset;
                (style as WTableStyle).TableProperties.Borders.Horizontal.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Color = Color.Black;
                (style as WTableStyle).TableProperties.Borders.Horizontal.Space = 0;

                (style as WTableStyle).TableProperties.Borders.Vertical.BorderType = BorderStyle.Outset;
                (style as WTableStyle).TableProperties.Borders.Vertical.LineWidth = .75f;
                (style as WTableStyle).TableProperties.Borders.Vertical.Color = Color.Black;
                (style as WTableStyle).TableProperties.Borders.Vertical.Space = 0;
                #endregion

                #region Table Cell Properties
                (style as WTableStyle).CellProperties.BackColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.ForeColor = Color.FromArgb(0, 255, 255, 255);
                (style as WTableStyle).CellProperties.TextureStyle = TextureStyle.TextureNone;
                #endregion

                #region Table Row Properties
                (style as WTableStyle).RowProperties.CellSpacing = 1;
                #endregion

                #region Conditional Formatting Properties
                #region First Row Conditional Formatting Style
                firstRowStyle = (style as WTableStyle).ConditionalFormat(ConditionalFormattingCode.FirstRow);
                #region Character format
                firstRowStyle.CharacterFormat.TextColor = Color.Empty;
                #endregion

                #region Table Cell Properties
                firstRowStyle.CellProperties.Borders.DiagonalDown.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalDown.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalDown.LineWidth = 0;

                firstRowStyle.CellProperties.Borders.DiagonalUp.BorderType = BorderStyle.None;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Color = Color.Black;
                firstRowStyle.CellProperties.Borders.DiagonalUp.Space = 0;
                firstRowStyle.CellProperties.Borders.DiagonalUp.LineWidth = 0;
                #endregion
                #endregion
                #endregion
            }
            /// <summary>
            /// Updates the complex boolean value.
            /// </summary>
            /// <param name="format">The character format.</param>
            /// <param name="propKey">The property key.</param>
            /// <param name="val">Boolean value.</param>
            private static void UpdateComplexProperty(FormatBase format, short propertyKey, bool value)
            {
                byte complexValue = (value) ? WCharacterFormat.DEF_NEGCOMPLEX_VALUE : WCharacterFormat.DEF_POSCOMPLEX_VALUE;
                
                if (complexValue != 0)
                {
                    format.SetComplexBoolValue(propertyKey, complexValue);
                }
            }

            /// <summary>
            /// Updates the XML resource and reader.
            /// </summary>
            private static void UpdateXMLResAndReader()
            {
                if (m_xmlStream != null)
                    return;

                if (m_xmlStream == null)
                {
#if WINRT
                    Assembly execAssm = typeof(BuiltinStyleLoader).GetTypeInfo().Assembly;
#else
                    Assembly execAssm = Assembly.GetExecutingAssembly();
#endif
                    m_xmlStream = execAssm.GetManifestResourceStream(DEF_DOCIO_RESOURCES + ".builtin-styles.xml");
                }

                if (m_xmlStream == null)
                {
                    throw new Exception("Resource file builtin-styles.xml not found.");
                }
            }
            /// <summary>
            /// Determines whether is list style the specified BuiltinStyle.
            /// </summary>
            /// <param name="bstyle">The built in style.</param>
            /// <returns>
            /// 	 if the list style specified in BuiltinStyle, set to <c>true</c>.
            /// </returns>
            internal static bool IsListStyle(BuiltinStyle bstyle)
            {
                bool isListStyle = false;
                for (int i = 0; i < DEF_LIST_STYLES_NUMBER; i++)
                {
                    if (bstyle.ToString() == ((BuiltinListStyle)i).ToString())
                    {
                        isListStyle = true;
                        break;
                    }
                }

                return isListStyle;
            }
            #endregion
        }
    }
}
