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
using System.Collections.Specialized;

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Syncfusion.DocIO.DLS.XML;
#if !SILVERLIGHT && !WP
using Syncfusion.DocIO.DLS.Rendering;
using Syncfusion.Layouting;
using Syncfusion.DocIO.Rendering;
using System.Collections;
using System.Drawing;
#endif
using System.Collections.Generic;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represent a table of content.
    /// </summary>
    public class TableOfContent : ParagraphItem
#if !SILVERLIGHT && !WP
, ILayoutInfo
#endif
    {
        #region Constants
        private const int DEF_UPPER_HEADING_LEVEL = 3;
        private const int DEF_LOWER_HEADING_LEVEL = 1;
        private const char DEF_HEADING_LEVELS_SWITCH = 'o';
        private const char DEF_HYPERLINK_SWITCH = 'h';
        private const char DEF_PAGE_NUMBERS_SWITCH = 'n';
        private const char DEF_SEPARATOR_SWITCH = 'p';
        private const char DEF_USE_OUTLINE_SWITCH = 'u';
        private const char DEF_USE_FIELDS_SWITCH = 'f';
        private const char DEF_STYLES_SWITCH = 't';
        #endregion

        #region Fields
        private WField m_tocField;
        private bool m_useHeadingStyles = true;
        private int m_upperHeadingLevel = DEF_UPPER_HEADING_LEVEL;
        private int m_lowerHeadingLevel = DEF_LOWER_HEADING_LEVEL;
        private bool m_useFields;
        private string m_tableID;
        private bool m_rightAlignPageNumbers = true;
        private bool m_useHyperlinks = true;
        private bool m_useOutlineLevels;
        private bool m_includePageNumbers = true;

        private Dictionary<int, List<WParagraphStyle>> m_tocStyles;
        private bool m_invalidFormatString;
        private bool m_formattingParsed;
        private string m_lstSepar = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
#if !SILVERLIGHT && !WP
        private Dictionary<int, List<string>> m_tocLevels;
        private List<int> m_tocEntryPageNumbers;
        private int m_tocBookmarkID = 0;
        private WParagraph m_tocParagraph;
#endif
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether to use default heading styles.
        /// </summary>
        /// <value>if it uses heading styles, set to <c>true</c>.</value>
        public bool UseHeadingStyles
        {
            get
            {
                OnGetValue();
                return m_useHeadingStyles;
            }
            set
            {
                OnChange();
                m_useHeadingStyles = value;
            }
        }
        /// <summary>
        /// Gets or sets the ending heading level of the table of content. Default value is 3.
        /// </summary>
        /// <value>The upper heading level.</value>
        public int UpperHeadingLevel
        {
            get
            {
                OnGetValue();
                return m_upperHeadingLevel;
            }
            set
            {
                CheckLevelNumber("UpperHeadingLevel", value);
                OnChange();
                m_upperHeadingLevel = value;
            }
        }
        /// <summary>
        /// Gets or sets the starting heading level of the table of content. Default value is 1
        /// </summary>
        /// <value>The starting heading level.</value>
        public int LowerHeadingLevel
        {
            get
            {
                OnGetValue();
                return m_lowerHeadingLevel;
            }
            set
            {
                CheckLevelNumber("LowerHeadingLevel", value);
                OnChange();
                m_lowerHeadingLevel = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether to use table entry fields.Default value is false.
        /// </summary>
        /// <value>
        /// 	if it uses table entry fields, set to <c>true</c>.
        /// </value>
        public bool UseTableEntryFields
        {
            get
            {
                return m_useFields;
            }
            set
            {
                m_useFields = value;
            }
        }
        /// <summary>
        /// Gets or sets the table ID.
        /// </summary>
        /// <value>The table ID.</value>
        public string TableID
        {
            get
            {
                return m_tableID;
            }
            set
            {
                m_tableID = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether to show page numbers from right side. Default value is true.
        /// </summary>
        /// <value>
        /// 	if right align of page numbers, set to <c>true</c>.
        /// </value>
        public bool RightAlignPageNumbers
        {
            get
            {
                OnGetValue();
                return m_rightAlignPageNumbers;
            }
            set
            {
                OnChange();
                m_rightAlignPageNumbers = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether to show page numbers. Default value is true.
        /// </summary>
        /// <value>if it includes page numbers, set to <c>true</c>.</value>
        public bool IncludePageNumbers
        {
            get
            {
                OnGetValue();
                return m_includePageNumbers;
            }
            set
            {
                OnChange();
                m_includePageNumbers = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether to use hyperlinks.Default value is true.
        /// </summary>
        /// <value>if it uses hyperlinks, set to <c>true</c>.</value>
        public bool UseHyperlinks
        {
            get
            {
                OnGetValue();
                return m_useHyperlinks;
            }
            set
            {
                OnChange();
                m_useHyperlinks = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether use outline levels.Default value is false.
        /// </summary>
        /// <value>if it uses outline levels, set to <c>true</c>.</value>
        public bool UseOutlineLevels
        {
            get
            {
                OnGetValue();
                return m_useOutlineLevels;
            }
            set
            {
                OnChange();
                m_useOutlineLevels = value;
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
                return EntityType.TOC;
            }
        }
        /// <summary>
        /// Gets or sets the formatting string.
        /// </summary>
        /// <value>The formatting string.</value>
        internal string FormattingString
        {
            get
            {
                return m_tocField.m_formattingString;
            }
            set
            {
                m_tocField.m_formattingString = value;
            }
        }
        /// <summary>
        /// Gets TOC field
        /// </summary>
        internal WField TOCField
        {
            get
            {
                return m_tocField;
            }
        }
        /// <summary>
        /// Gets the TOC styles.
        /// </summary>
        /// <value>The TOC styles.</value>
        internal Dictionary<int, List<WParagraphStyle>> TOCStyles
        {
            get
            {
                if (m_tocStyles == null)
                {
                    m_tocStyles = new Dictionary<int, List<WParagraphStyle>>();
                }

                return m_tocStyles;
            }
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Gets the TOC levels.
        /// </summary>
        /// <value>The TOC levels.</value>
        internal Dictionary<int, List<string>> TOCLevels
        {
            get
            {
                if (m_tocLevels == null)
                {
                    m_tocLevels = new Dictionary<int, List<string>>();
                }
                return m_tocLevels;
            }
        }
        /// <summary>
        /// Gets the TOC entry page numbers.
        /// </summary>
        /// <value>The TOC entry page numbers.</value>
        private List<int> TOCEntryPageNumbers
        {
            get
            {
                if (m_tocEntryPageNumbers == null)
                {
                    m_tocEntryPageNumbers = new List<int>();
                }
                return m_tocEntryPageNumbers;
            }
            set
            {
                m_tocEntryPageNumbers = value;
            }
        }
        /// <summary>
        /// Gets the last TOC paragraph.
        /// </summary>
        /// <value>The last TOC paragraph.</value>
        private WParagraph LastTOCParagraph
        {
            get
            {
                if (m_tocParagraph == null)
                {
                    m_tocParagraph = this.OwnerParagraph;
                }
                return m_tocParagraph;
            }
        }
#endif
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TableOfContent"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        public TableOfContent(IWordDocument doc)
            : base(doc as WordDocument)
        {
            m_tocField = new WField(doc);
            m_tocField.FieldType = FieldType.FieldTOC;
            m_tableID = string.Empty;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TableOfContent"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="switches">The switches.</param>
        public TableOfContent(IWordDocument doc, string switches)
            : this(doc)
        {
            this.TOCField.m_formattingString = switches;
            //sets false value to this property when there is no switch for hyperlink.
            m_useHyperlinks = false;
            ParseSwitches();
        }
        #endregion

        #region Implementation / public
        /// <summary>
        /// Sets the style for TOC level.
        /// </summary>
        /// <param name="levelNumber">The level number.</param>
        /// <param name="styleName">Name of the style.</param>
        public void SetTOCLevelStyle(int levelNumber, string styleName)
        {
            CheckLevelNumber("levelNumber", levelNumber);
            SetStyleForTOCLevel(levelNumber, styleName, true);
        }
        /// <summary>
        /// Gets the style name for TOC level.
        /// </summary>
        /// <param name="levelNumber">The level number.</param>
        /// <returns></returns>
        public String GetTOCLevelStyle(int levelNumber)
        {
            List<string> toc_StyleNames = GetTOCLevelStyles(levelNumber);
            return toc_StyleNames[0];
        }
        /// <summary>
        /// Get the list of styles defined for this Level
        /// </summary>
        /// <param name="levelNumber"></param>
        /// <returns></returns>
        public List<string> GetTOCLevelStyles(int levelNumber)
        {
            if (levelNumber < m_lowerHeadingLevel || levelNumber > m_upperHeadingLevel)
            {
                throw new ArgumentException("Level index must be >= LowerHeadingLevel and <= UpperHeadingLevel");
            }

            ParseSwitches();
            List<string> tocLevelStyleNames = new List<string>();
            if (m_tocStyles.ContainsKey(levelNumber))
            {
                foreach (WParagraphStyle toc_Style in m_tocStyles[levelNumber])
                {
                    tocLevelStyleNames.Add(toc_Style.Name);
                }
            }
            else
            {
                BuiltinStyle builtInStyle = (BuiltinStyle)levelNumber;
                WParagraphStyle tocLevelStyle = GetBuiltinStyle(builtInStyle) as WParagraphStyle;
                tocLevelStyleNames.Add(tocLevelStyle.Name);
            }
            return tocLevelStyleNames;
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Parses the switches.
        /// </summary>
        private void ParseSwitches()
        {
            if (m_formattingParsed)
                return;

            string switches = this.TOCField.m_formattingString;
            if (switches.Contains("\\* MERGEFORMAT"))
                switches = switches.Remove(switches.IndexOf("\\* MERGEFORMAT")).Trim();
            else if (switches.Contains("\\* Mergeformat"))
                switches = switches.Remove(switches.IndexOf("\\* Mergeformat")).Trim();
            string[] options = switches.Split('\\');
            bool isHeadingLevelDefined = false;
            for (int i = 0, cnt = options.Length; i < cnt; i++)
            {
                string optSwitch = options[i];

                if (optSwitch.Length == 0)
                    continue;

                switch (optSwitch[0])
                {
                    case DEF_HEADING_LEVELS_SWITCH:
                        m_useHeadingStyles = true;
                        isHeadingLevelDefined = true;
                        ParseHeadingLevels(optSwitch);
                        break;
                    case DEF_HYPERLINK_SWITCH:
                        m_useHyperlinks = true;
                        break;
                    case DEF_PAGE_NUMBERS_SWITCH:
                        m_includePageNumbers = false;
                        break;
                    case DEF_SEPARATOR_SWITCH:
                        m_rightAlignPageNumbers = false;
                        break;
                    case DEF_USE_OUTLINE_SWITCH:
                        m_useOutlineLevels = true;
                        break;
                    case DEF_USE_FIELDS_SWITCH:
                        ParseUseField(optSwitch);
                        break;
                    case DEF_STYLES_SWITCH:
                        ParseHeaderStyles(optSwitch);
                        break;
                }
            }
            if (TOCStyles.Count == 0
                && !isHeadingLevelDefined)
                m_upperHeadingLevel = 9;
            m_formattingParsed = true;
        }
        /// <summary>
        /// Gets the built-in style.
        /// </summary>
        /// <param name="builtinStyle">The built-in style.</param>
        /// <returns></returns>
        private IWParagraphStyle GetBuiltinStyle(BuiltinStyle builtinStyle)
        {
            string builtinName = Style.BuiltInToName(builtinStyle);
            IWParagraphStyle pStyle = m_doc.Styles.FindByName(builtinName, StyleType.ParagraphStyle) as IWParagraphStyle;

            if (pStyle == null)
            {
                pStyle = (IWParagraphStyle)Style.CreateBuiltinStyle(builtinStyle, m_doc);
                m_doc.Styles.Add(pStyle);
            }

            return pStyle;
        }
        /// <summary>
        /// Creates the default styles collection.
        /// </summary>
        private void CreateDefStylesColl()
        {
            for (int level = 1; level <= 9; level++)
            {
                List<WParagraphStyle> tocLevelStyles = new List<WParagraphStyle>();
                BuiltinStyle builtInStyle = (BuiltinStyle)level;
                tocLevelStyles.Add(GetBuiltinStyle(builtInStyle) as WParagraphStyle);
                TOCStyles.Add(level, tocLevelStyles);
            }
        }
        /// <summary>
        /// Updates the formatting string of TOC field.
        /// </summary>
        internal void UpdateFormattingString()
        {
            if (!m_invalidFormatString)
            {
                return;
            }
            else
            {
                this.TOCField.m_formattingString = string.Empty;
            }

            if (m_useHeadingStyles)
            {
                UpdateTOCLevels();
            }

            UpdateHeaderStyles();
            UpdateUsePageNumbers();
            UpdatePageNumberAlign();
            UpdateUseField();
            UpdateHyperlinks();
            UpdateUseOutlineLevels();

            m_formattingParsed = true;
        }
        /// <summary>
        /// Updates TOC heigher and lower levels.
        /// </summary>
        private void UpdateTOCLevels()
        {
            string levelStr = string.Format("\\o \"{0}-{1}\" ", m_lowerHeadingLevel, m_upperHeadingLevel);
            this.TOCField.m_formattingString += levelStr;
        }
        /// <summary>
        /// Updates the hyperlinks.
        /// </summary>
        private void UpdateHyperlinks()
        {
            if (m_useHyperlinks)
            {
                this.TOCField.m_formattingString += "\\h \\z ";
            }
        }
        /// <summary>
        /// Updates the formatting string with UsePageNumbers value.
        /// </summary>
        private void UpdateUsePageNumbers()
        {
            if (!m_includePageNumbers)
            {
                this.TOCField.m_formattingString += "\\" + DEF_PAGE_NUMBERS_SWITCH + " ";
            }
        }
        /// <summary>
        /// Updates the page number alignment.
        /// </summary>
        private void UpdatePageNumberAlign()
        {
            if (!m_rightAlignPageNumbers)
            {
                this.TOCField.m_formattingString += "\\" + DEF_SEPARATOR_SWITCH + " \" \" ";
            }
        }
        /// <summary>
        /// Updates the use outline levels property in formatting string.
        /// </summary>
        private void UpdateUseOutlineLevels()
        {
            if (m_useOutlineLevels)
            {
                this.TOCField.m_formattingString += "\\" + DEF_USE_OUTLINE_SWITCH + " ";
            }
        }
        /// <summary>
        /// Updates the use table entry field property.
        /// </summary>
        private void UpdateUseField()
        {
            if (m_useFields)
            {
                this.TOCField.m_formattingString += "\\" + DEF_USE_FIELDS_SWITCH + " " + m_tableID;
            }
        }
        /// <summary>
        /// Updates the header styles.
        /// </summary>
        private void UpdateHeaderStyles()
        {
            if (m_tocStyles == null || m_tocStyles.Count == 0)
                return;

            TOCField.m_formattingString += "\\" + DEF_STYLES_SWITCH + " \"";
            for (int level = m_lowerHeadingLevel; level <= m_upperHeadingLevel; level++)
            {
                if (!TOCStyles.ContainsKey(level))
                    continue;
                foreach (WParagraphStyle levelStyle in TOCStyles[level])
                {
                    if (Style.BuiltinStyleLoader.BuiltinStyleNames[level] != (levelStyle as IStyle).Name)
                    {
                        TOCField.m_formattingString += ((levelStyle as IStyle).Name) + m_lstSepar + level + m_lstSepar;
                    }
                }
            }

            TOCField.m_formattingString += "\"";
        }
        /// <summary>
        /// Parses the heading levels.
        /// </summary>
        /// <param name="optionString">The option string.</param>
        private void ParseHeadingLevels(string optionString)
        {
            Regex headRegex = new Regex("[0-9]");
            MatchCollection matches = headRegex.Matches(optionString);
            if (matches.Count == 2)
            {
                m_lowerHeadingLevel = Int32.Parse((matches[0] as Match).Groups[0].Value);
                m_upperHeadingLevel = Int32.Parse((matches[1] as Match).Groups[0].Value);
            }
            else if (matches.Count == 0)
                m_upperHeadingLevel = 9;
        }
        /// <summary>
        /// Parses the number alignment.
        /// </summary>
        /// <param name="optionString">The option string.</param>
        private void ParseNumberAlignment(string optionString)
        {
            Regex alignRegex = new Regex("[\"][ ][\"]");
            Match match = alignRegex.Match(optionString);
            if (match.Captures.Count == 1)
            {
                m_rightAlignPageNumbers = false;
            }
        }
        /// <summary>
        /// Parses the use field.
        /// </summary>
        /// <param name="optionString">The option string.</param>
        private void ParseUseField(string optionString)
        {
            m_useFields = true;
            m_tableID = optionString.Substring(1, optionString.Length - 1).Trim();
        }
        /// <summary>
        /// Parses the header styles.
        /// </summary>
        private void ParseHeaderStyles(string optionString)
        {
            char styleSep = (m_lstSepar.ToCharArray())[0];
            string[] partStrings = optionString.Split('"');
            string[] difStyles = partStrings[1].Split(styleSep);

            int level = -1;
            for (int i = 0, cnt = difStyles.Length; i + 1 < cnt; i += 2)
            {
                level = Int32.Parse(difStyles[i + 1]);
                SetStyleForTOCLevel(level, difStyles[i], false);
            }
            if (TOCStyles.Count > 0)
                m_useHeadingStyles = false;

            m_invalidFormatString = false;
        }
        /// <summary>
        /// Called when [set value].
        /// </summary>
        private void OnChange()
        {
            ParseSwitches();
            m_invalidFormatString = true;
        }
        /// <summary>
        /// Called when [get value].
        /// </summary>
        private void OnGetValue()
        {
            ParseSwitches();
        }
        /// <summary>
        /// Sets the style for TOC level.
        /// </summary>
        /// <param name="levelNumber">The level number.</param>
        /// <param name="styleName">Name of the style.</param>
        /// <param name="onSetProperty">if it is on set property, set to <c>true</c>.</param>
        private void SetStyleForTOCLevel(int levelNumber, string styleName, bool onSetProperty)
        {
            if (onSetProperty)
            {
                OnChange();
            }

            BuiltinStyle builinStyle = Style.NameToBuiltIn(styleName);
            IWParagraphStyle pStyle = m_doc.Styles.FindByName(styleName, StyleType.ParagraphStyle) as IWParagraphStyle;

            BuiltinStyle builinStyleAlt = Style.NameToBuiltIn(styleName.ToLower());
            IWParagraphStyle pStyleAlt = m_doc.Styles.FindByName(styleName.ToLower(), StyleType.ParagraphStyle) as IWParagraphStyle;

            if (pStyle == null && builinStyle == BuiltinStyle.User)
            {
                //Word automation ignores and does not throw exception if the style doesn't exist in the document.
                //throw new ArgumentException("User's style with name " + styleName + " doesn't exist in the document.");
            }
            else if (pStyle == null && pStyleAlt == null)
            {
                pStyle = (IWParagraphStyle)Style.CreateBuiltinStyle(builinStyle, m_doc);
                m_doc.Styles.Add(pStyle);
            }
            IWParagraphStyle tocLevelStyle = null;
            if (pStyle != null)
            {
                tocLevelStyle = pStyle;
            }
            else if (pStyleAlt != null)
            {
                tocLevelStyle = pStyleAlt;
            }
            if (tocLevelStyle != null)
            {
                if (TOCStyles.ContainsKey(levelNumber))
                {
                    List<WParagraphStyle> toc_LevelStyles = TOCStyles[levelNumber];
                    if (!toc_LevelStyles.Contains(tocLevelStyle as WParagraphStyle))
                        toc_LevelStyles.Add(tocLevelStyle as WParagraphStyle);
                }
                else
                {
                    List<WParagraphStyle> toc_LevelStyles = new List<WParagraphStyle>();
                    toc_LevelStyles.Add(tocLevelStyle as WParagraphStyle);
                    TOCStyles.Add(levelNumber, toc_LevelStyles);
                }
            }
        }
        /// <summary>
        /// Checks the level number.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="levelNumber">The level number.</param>
        private void CheckLevelNumber(string parameterName, int levelNumber)
        {
            if (levelNumber < 1 || levelNumber > 9)
                throw new ArgumentOutOfRangeException(parameterName, "Level number value must be greater than 1 and smaller than 10.");
        }
        #endregion
#if !SILVERLIGHT && !WP
        #region Implementation / Update table of contents
        internal void UpdateTOCStyleLevels()
        {
            if (TOCLevels.Count > 0)
                TOCLevels.Clear();
            if (TOCStyles.Count > 0 && !UseHeadingStyles)
            {
                foreach (KeyValuePair<int, List<WParagraphStyle>> keyValue in TOCStyles)
                {
                    List<string> levelStyles = new List<string>();
                    foreach (WParagraphStyle tocLevelStyle in keyValue.Value)
                    {
                        levelStyles.Add(tocLevelStyle.Name);
                    }
                    TOCLevels.Add(keyValue.Key, levelStyles);
                }
            }
            else
            {
                string name = "heading ";
                for (int level = m_lowerHeadingLevel; level <= m_upperHeadingLevel; level++)
                {
                    List<string> levelStyles = new List<string>();
                    levelStyles.Add(name + level.ToString());
                    TOCLevels.Add(level, levelStyles);
                }
                //Update TOCLevels based on OutLineLevel of the Paragraph styles
                for (int i = 0; i < Document.Styles.Count; i++)
                {
                    if (Document.Styles[i] is WParagraphStyle)
                    {
                        int outLineLevel = (byte)(Document.Styles[i] as WParagraphStyle).ParagraphFormat.OutlineLevel + 1;
                        string styleName = (Document.Styles[i] as WParagraphStyle).Name;
                        if (outLineLevel > 0 && outLineLevel <= 9 && outLineLevel >= m_lowerHeadingLevel && outLineLevel <= m_upperHeadingLevel
                            && !styleName.ToLower().StartsWith("heading") && !styleName.ToLower().StartsWith("normal"))
                        {
                            if (TOCLevels.ContainsKey(outLineLevel) && !TOCLevels[outLineLevel].Contains(styleName))
                            {
                                TOCLevels[outLineLevel].Add(styleName);
                            }
                            else if (!TOCLevels.ContainsKey(outLineLevel))
                            {
                                List<string> levelStyles = new List<string>();
                                levelStyles.Add(styleName);
                                TOCLevels.Add(outLineLevel, levelStyles);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Updates the TOC field.
        /// </summary>
        internal void UpdateTOCField()
        {
            UpdateTOCStyleLevels();
            RemoveUpdatedTocEntries();
            RemoveExistingTocBookmarks();
            ParseDocument();
            if (IncludePageNumbers)
            {
                //Parse the document using Doc to PDF layouting.
                Syncfusion.DocIO.DLS.Rendering.DocumentLayouter layouter = new Syncfusion.DocIO.DLS.Rendering.DocumentLayouter();
                layouter.TOCLevels = TOCLevels;
                layouter.UseTCFields = UseTableEntryFields;
                TOCEntryPageNumbers = layouter.GetTOCEntryPageNumbers(this.Document);
                UpdatePageNumbers();
                layouter.InitLayoutInfo();
            }
        }

        #region Document Parsing
        /// <summary>
        /// Parses the document.
        /// </summary>
        private void ParseDocument()
        {
            WordDocument doc = this.Document;
            foreach (IWSection section in doc.Sections)
            {
                ParseTextBody(section.Body);
            }
        }
        /// <summary>
        /// Parses the text body.
        /// </summary>
        /// <param name="textBody">The text body.</param>
        private void ParseTextBody(WTextBody textBody)
        {
            for (int i = 0; i < textBody.Items.Count; i++)
            {
                if (textBody.Items[i] is WParagraph)
                {
                    IWParagraph paragraph = textBody.Items[i] as WParagraph;
                    ParseParagraph(paragraph);
                    //Updates the index of the current paragraph, if new paragraph is added in table of contents.
                    i = textBody.Items.IndexOf(paragraph);
                }
                else if (textBody.Items[i] is WTable)
                {
                    IWTable table = textBody.ChildEntities[i] as WTable;
                    ParseTable(table);
                    //Updates the index of the current table, if new paragraph is added in table of contents.
                    i = textBody.Items.IndexOf(table);
                }
            }
        }
        /// <summary>
        /// Parses the table.
        /// </summary>
        /// <param name="table">The table.</param>
        private void ParseTable(IWTable table)
        {
            for (int i = 0; i < table.Rows.Count; i++)
            {
                WTableRow tableRow = table.Rows[i] as WTableRow;
                for (int j = 0; j < tableRow.Cells.Count; j++)
                {
                    WTableCell cell = tableRow.Cells[j] as WTableCell;
                    ParseTextBody(cell as WTextBody);
                }
            }
        }
        /// <summary>
        /// Parses the paragraph.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        private void ParseParagraph(IWParagraph paragraph)
        {
            if (CheckParagraphStyle(paragraph.StyleName))
            {
                CheckAndSplitParagraph(paragraph);
                if (!string.IsNullOrEmpty(paragraph.Text))
                {
                    int startIndex = 0;
                    foreach (ParagraphItem item in paragraph.Items)
                    {
                        if (item is WTextRange && (item as WTextRange).Text != "\t")
                        {
                            if (this.OwnerParagraph == paragraph && (item as WTextRange).Text == "TOC")
                                continue;
                            startIndex = paragraph.Items.IndexOf(item);
                            break;
                        }
                    }
                    InsertBookmark(paragraph, null, startIndex, paragraph.Items.Count + 1);
                }
            }
            List<int> tcFieldIndexes = new List<int>();
            for (int i = 0; i < paragraph.Items.Count; i++)
            {
                if (paragraph.Items[i] is WTextBox)
                {
                    WTextBox textBox = paragraph.Items[i] as WTextBox;
                    ParseTextBody(textBox.TextBoxBody);
                }
                else if (paragraph.Items[i] is Shape)
                {
                    Shape shape = paragraph.Items[i] as Shape;
                    ParseTextBody(shape.TextBody);
                }
                else if (paragraph.Items[i] is WField)
                {
                    WField field = paragraph.Items[i] as WField;
                    if (field.FieldType == FieldType.FieldTOCEntry && this.UseTableEntryFields)
                        tcFieldIndexes.Add(i);
                }
            }
            //Insert bookmark to the TC fields.
            for (int j = 0; j < tcFieldIndexes.Count; j++)
            {
                int index = tcFieldIndexes[j] + j * 2;
                tcFieldIndexes.RemoveAt(j);
                InsertBookmark(paragraph, paragraph.Items[index] as WField, index, index + 2);
            }
        }
        /// <summary>
        /// Checks and splits the paragraph.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        private void CheckAndSplitParagraph(IWParagraph paragraph)
        {
            if (paragraph.Text.Contains("\t") || paragraph.Text.Contains("\n") || paragraph.Text.Contains("\r"))
            {
                for (int i = 0; i < paragraph.Items.Count; i++)
                {
                    if (paragraph.Items[i] is WTextRange)
                    {
                        WTextRange textRange = paragraph.Items[i] as WTextRange;
                        string text = textRange.Text;
                        if (textRange.Text != "\t" && textRange.Text.Contains("\t"))
                            UpdateTabCharacters(textRange);
                        if (textRange.Text.Contains("\n"))
                        {
                            UpdateNewLineCharacters(textRange, "\n");
                            break;
                        }
                        else if (textRange.Text.Contains("\r"))
                        {
                            UpdateNewLineCharacters(textRange, "\r");
                            break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Updates the tab characters.
        /// </summary>
        /// <param name="textRange">The text range.</param>
        private void UpdateTabCharacters(WTextRange textRange)
        {
            WParagraph paragraph = textRange.OwnerParagraph;
            string text = textRange.Text;
            int index = text.IndexOf("\t");
            int txtIndex = paragraph.Items.IndexOf(textRange);
            string rem = text.Substring(index + 1);
            //Split text range.
            WTextRange txtRange = textRange.Clone() as WTextRange;
            if (index > 0)
            {
                txtRange.Text = text.Substring(index);
                textRange.Text = text.Substring(0, index);
            }
            else if (rem != string.Empty)
            {
                txtRange.Text = rem;
                textRange.Text = "\t";
            }
            paragraph.Items.Insert(txtIndex + 1, txtRange);
        }
        /// <summary>
        /// Updates the new line characters.
        /// </summary>
        /// <param name="textRange">The text range.</param>
        /// <param name="splitText">The split text.</param>
        private void UpdateNewLineCharacters(WTextRange textRange, string splitText)
        {
            WParagraph paragraph = textRange.OwnerParagraph;
            string text = textRange.Text;
            int index = text.IndexOf(splitText);
            int txtIndex = paragraph.Items.IndexOf(textRange);
            string rem = text.Substring(index + 1);
            //Split text range.
            if (rem != string.Empty)
            {
                WTextRange txtRange = textRange.Clone() as WTextRange;
                txtRange.Text = rem;
                paragraph.Items.Insert(txtIndex + 1, txtRange);
                textRange.Text = text.Substring(0, index);
            }
            else
            {
                textRange.Text = text.Substring(0, index);
            }
            CreateParagraph(paragraph as WParagraph, txtIndex + 1);
            if (textRange.Text == string.Empty)
                paragraph.Items.Remove(textRange);
        }
        /// <summary>
        /// Creates the paragraph.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="index">The index.</param>
        private void CreateParagraph(WParagraph paragraph, int index)
        {
            WTextBody textBody = paragraph.OwnerTextBody as WTextBody;
            int paraIndex = textBody.Items.IndexOf(paragraph);
            WParagraph para = paragraph.Clone() as WParagraph;
            int cnt = para.Items.Count;
            for (int i = 0; i < cnt; i++)
            {
                para.Items.RemoveAt(0);
            }
            textBody.Items.Insert(paraIndex + 1, para);
            cnt = paragraph.Items.Count;
            for (int i = index; i < cnt; i++)
            {
                para.Items.Insert(para.Items.Count, paragraph.Items[index]);
            }
        }
        /// <summary>
        /// Removes the updated toc entries.
        /// </summary>
        private void RemoveUpdatedTocEntries()
        {
            WParagraph paragraph = this.OwnerParagraph;
            WTextBody textBody = paragraph.OwnerTextBody;
            int paraIndex = textBody.Items.IndexOf(paragraph);
            bool isTOCUpdated = true;
            bool isTOCFieldEnd = false;
            for (int i = paraIndex, count = 0; i < textBody.Items.Count; i++)
            {
                if (textBody.Items[i] is WParagraph)
                {
                    WParagraph para = textBody.Items[i] as WParagraph;
                    int itemIndex = 0;
                    if (i == paraIndex)
                        itemIndex = para.Items.IndexOf(this);
                    for (int j = itemIndex; j < para.Items.Count; j++)
                    {
                        if (para.Items[j] == this && para.Items[j + 1] is WFieldMark && (para.Items[j + 1] as WFieldMark).Type == FieldMarkType.FieldSeparator)
                        {
                            count++;
                            j++;
                            continue;
                        }
                        if (para.Items[j] is WFieldMark)
                        {
                            WFieldMark field = para.Items[j] as WFieldMark;
                            if (field.Type == FieldMarkType.FieldSeparator)
                                count++;
                            else if (field.Type == FieldMarkType.FieldEnd)
                            {
                                isTOCFieldEnd = this.TOCField.FieldEnd == field;
                                if ((j == 0 || isTOCFieldEnd)&& count == 1)
                                {
                                    //Need to combine the paragraphs.
                                    break;
                                }
                                else
                                    count--;
                            }
                        }
                        else if (para.Items[j] is WTextRange && (para.Items[j] as WTextRange).Text == "TOC")
                        {
                            isTOCUpdated = false;
                            break;
                        }
                        para.Items.Remove(para.Items[j]);
                        j--;
                    }
                    if (para.Items.Count == 0)
                    {
                        textBody.Items.Remove(para);
                        i--;
                    }
                    else if ((para.Items[0] is WFieldMark || isTOCFieldEnd) && isTOCUpdated)
                    {
                        para.Items.Insert(0, this);
                        int fieldMarkIndex = 0;
                        for (int k = paragraph.ChildEntities.Count - 1; k >= 0; k--)
                        {
                            if (paragraph.Items[k] is WFieldMark && (paragraph.Items[k] as WFieldMark).Type == FieldMarkType.FieldSeparator)
                            {
                                fieldMarkIndex = k;
                                break;
                            }
                        }
                        para.Items.Insert(1, paragraph.Items[fieldMarkIndex]);
                        WTextRange txtRange = new WTextRange(this.Document);
                        txtRange.Text = "TOC";
                        para.Items.Insert(2, txtRange);
                        if (para != paragraph)
                            textBody.Items.Remove(paragraph);
                        break;
                    }
                    else if (!isTOCUpdated)
                        break;
                }
            }
        }
        /// <summary>
        /// Removes the existing toc bookmarks.
        /// </summary>
        private void RemoveExistingTocBookmarks()
        {
            for (int i = this.Document.Bookmarks.Count - 1; i >= 0; i--)
            {
                Bookmark bkmark = this.Document.Bookmarks[i];
                if (bkmark.Name.StartsWith("_Toc"))
                    this.Document.Bookmarks.Remove(bkmark);
            }
        }
        /// <summary>
        /// Checks the paragraph style.
        /// </summary>
        /// <param name="styleName">Name of the style.</param>
        /// <returns></returns>
        private bool CheckParagraphStyle(string styleName)
        {
            if (styleName != null)
                styleName = styleName.ToLower().Replace(" ", "");
            else
                styleName = "normal";
            foreach (KeyValuePair<int, List<string>> tocLevel in TOCLevels)
            {
                foreach (string tocLevelName in tocLevel.Value)
                {
                    string levelStyle = tocLevelName.ToLower().Replace(" ", "");
                    if (styleName.StartsWith(levelStyle))
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Gets the TOC level.
        /// </summary>
        /// <param name="styleName">Name of the style.</param>
        /// <returns></returns>
        private int GetTOCLevel(string styleName)
        {
            int level = 0;
            styleName = styleName.ToLower().Replace(" ", "");
            foreach (KeyValuePair<int, List<string>> keyValue in TOCLevels)
            {
                foreach (string levelName in keyValue.Value)
                {
                    string levelStyle = levelName.ToLower().Replace(" ", "");
                    if (styleName.StartsWith(levelStyle))
                    {
                        level = keyValue.Key;
                        return level;
                    }
                }
            }
            return level;
        }
        /// <summary>
        /// Inserts the bookmark.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="field">The field.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="endIndex">The end index.</param>
        private void InsertBookmark(IWParagraph paragraph, WField field, int startIndex, int endIndex)
        {
            string bkName = GenerateBookmarkName();
            BookmarkStart bkStart = new BookmarkStart(this.Document, bkName);
            paragraph.Items.Insert(startIndex, bkStart);

            //Insert bookmark hyperlink for the paragraph in Table of contents.
            InsertBookmarkHyperlink(paragraph, field, bkName);

            if (field == null)
                endIndex = paragraph.Items.Count;
            BookmarkEnd bkEnd = new BookmarkEnd(this.Document, bkName);
            paragraph.Items.Insert(endIndex, bkEnd);
        }
        /// <summary>
        /// Inserts the bookmark hyperlink.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="field">The field.</param>
        /// <param name="bookmark">The bookmark.</param>
        private void InsertBookmarkHyperlink(IWParagraph paragraph, WField field, string bookmark)
        {
            int level = GetTOCLevel(paragraph.StyleName);
            string text = string.Empty;
            if (field != null)
            {
                text = field.FieldValue;
                string option = field.FormattingString;
                option = option.Replace("\\l", "").Replace(" ", "");
                level = int.Parse(option);
            }
            //Create a TOC paragraph
            WParagraph tocPara = CreateTOCParagraph(level);
            //Insert bookmark hyperlink of the paragraph.
            CreateHyperlink(paragraph, tocPara, text, bookmark);

            if (IncludePageNumbers)
                AddTabsAndPageRefField(tocPara, bookmark);
        }
        /// <summary>
        /// Creates the hyperlink.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="tocParagraph">The toc paragraph.</param>
        /// <param name="text">The text.</param>
        /// <param name="bookmark">The bookmark.</param>
        private void CreateHyperlink(IWParagraph paragraph, WParagraph tocParagraph, string text, string bookmark)
        {
            WField fieldStart = new WField(Document);
            fieldStart.FieldType = FieldType.FieldHyperlink;
            tocParagraph.Items.Add(fieldStart);
            fieldStart.FieldSeparator = tocParagraph.AppendFieldMark(FieldMarkType.FieldSeparator);
            IWTextRange txtRange;
            if (!string.IsNullOrEmpty(text))
            {
                txtRange = tocParagraph.AppendText(text);
                txtRange.CharacterFormat.CharStyleName = "Hyperlink";
            }
            else
            {
                UpdateList(paragraph, tocParagraph);
                WParagraphStyle pStyle = (paragraph as WParagraph).ParaStyle as WParagraphStyle;
                foreach (ParagraphItem item in paragraph.Items)
                {
                    if (item is WTextRange && (item as WTextRange).Text != "\t")
                    {
                        txtRange = tocParagraph.AppendText((item as WTextRange).Text);
                        if (!pStyle.CharacterFormat.HasValue(WCharacterFormat.BoldKey) && (item as WTextRange).CharacterFormat.HasValue(WCharacterFormat.BoldKey))
                            txtRange.CharacterFormat.Bold = (item as WTextRange).CharacterFormat.Bold;
                        if (!pStyle.CharacterFormat.HasValue(WCharacterFormat.ItalicKey) && (item as WTextRange).CharacterFormat.HasValue(WCharacterFormat.ItalicKey))
                            txtRange.CharacterFormat.Italic = (item as WTextRange).CharacterFormat.Italic;
                        txtRange.CharacterFormat.CharStyleName = "Hyperlink";
                    }
                }
            }

            //Insert field mark end.
            WFieldMark end = new WFieldMark(Document, FieldMarkType.FieldEnd);
            tocParagraph.Items.Add(end);
            fieldStart.FieldEnd = end;

            Hyperlink hl = new Hyperlink(fieldStart);
            hl.Type = HyperlinkType.Bookmark;
            hl.BookmarkName = bookmark;
        }
        /// <summary>
        /// Updates the tabs.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        private void AddTabsAndPageRefField(WParagraph paragraph, string bookmark)
        {
            WTextRange txtRange;
            //Insert tabs
            if (RightAlignPageNumbers)
            {
                WParagraphStyle tocStyle = paragraph.ParaStyle as WParagraphStyle;
                if (tocStyle.ParagraphFormat.Tabs.Count == 0)
                    paragraph.ParagraphFormat.Tabs.AddTab(GetTabPosition(paragraph), TabJustification.Right, TabLeader.Dotted);
                txtRange = new WTextRange(this.Document);
                txtRange.Text = "\t";
                paragraph.Items.Insert(paragraph.Items.Count - 1, txtRange);
            }

            //Insert page reference field.
            WField pageRef = new WField(this.Document);
            pageRef.FieldType = FieldType.FieldPageRef;
            pageRef.FieldCode = "PAGEREF " + bookmark + " \\h";
            pageRef.m_fieldValue = bookmark + " \\h";
            paragraph.Items.Insert(paragraph.Items.Count - 1, pageRef);

            //Insert field mark separate.
            WFieldMark fieldMark = new WFieldMark(this.Document, FieldMarkType.FieldSeparator);
            paragraph.Items.Insert(paragraph.Items.Count - 1, fieldMark);

            //Insert text range for page number.
            txtRange = new WTextRange(this.Document);
            paragraph.Items.Insert(paragraph.Items.Count - 1, txtRange);

            //Insert field mark end.
            fieldMark = new WFieldMark(this.Document, FieldMarkType.FieldEnd);
            paragraph.Items.Insert(paragraph.Items.Count - 1, fieldMark);
        }
        /// <summary>
        /// Gets the tab position.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        private float GetTabPosition(Entity entity)
        {
            float position = 0;
            Entity ent = entity;
            while (!(ent is WSection))
            {
                if (ent.Owner == null)
                    break;
                else
                    ent = ent.Owner as Entity;
            }
            if (ent is WSection)
            {
                position = (float)((ent as WSection).PageSetup.ClientWidth - 0.5);
            }
            return position;
        }
        /// <summary>
        /// Creates the TOC paragraph.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        private WParagraph CreateTOCParagraph(int level)
        {
            WTextBody textBody = LastTOCParagraph.OwnerTextBody;
            int paraIndex = textBody.Items.IndexOf(LastTOCParagraph);
            int tocIndex = LastTOCParagraph.Items.IndexOf(this);
            if (tocIndex > 0)
            {
                CreateParagraph(LastTOCParagraph, tocIndex);
                m_tocParagraph = this.OwnerParagraph;
            }
            paraIndex = textBody.Items.IndexOf(LastTOCParagraph);
            WParagraph para = new WParagraph(Document);
            textBody.Items.Insert(paraIndex, para);
            //Updates TOC level style index.
            level = level + 18;
            //Get TOC style name
            string styleName = Style.BuiltInToName((BuiltinStyle)level);
            para.ApplyStyle((BuiltinStyle)level);
            if (LastTOCParagraph == this.OwnerParagraph)
            {
                for (int i = 0; i < LastTOCParagraph.Items.Count; i++)
                {
                    if (LastTOCParagraph.Items[i] == this)
                    {
                        para.Items.Insert(para.Items.Count, LastTOCParagraph.Items[i]);
                        i--;
                    }
                    else if (LastTOCParagraph.Items[i] is WFieldMark)
                    {
                        para.Items.Insert(para.Items.Count, LastTOCParagraph.Items[i]);
                        i--;
                    }
                    else if (LastTOCParagraph.Items[i] is WTextRange)
                    {
                        LastTOCParagraph.Items.Remove(LastTOCParagraph.Items[i]);
                        i--;
                        break;
                    }
                }
            }
            return para;
        }
        /// <summary>
        /// Generates the name of the bookmark.
        /// </summary>
        /// <returns></returns>
        private string GenerateBookmarkName()
        {
            m_tocBookmarkID++;
            string bookmarkName = "_Toc" + string.Format("{0:0000000000}", m_tocBookmarkID);
            return bookmarkName;
        }
        #endregion

        #region Updating page number
        /// <summary>
        /// Updates the page numbers.
        /// </summary>
        private void UpdatePageNumbers()
        {
            WParagraph paragraph = this.OwnerParagraph;
            WTextBody textBody = paragraph.OwnerTextBody;
            int paraIndex = textBody.ChildEntities.IndexOf(paragraph);
            int lastTocPara = textBody.ChildEntities.IndexOf(LastTOCParagraph);
            for (int i = paraIndex, index = 0; i < lastTocPara; i++)
            {
                if (textBody.ChildEntities[i] is WParagraph)
                {
                    WParagraph para = textBody.ChildEntities[i] as WParagraph;
                    (para.Items[para.Items.Count - 3] as WTextRange).Text = TOCEntryPageNumbers[index].ToString();
                    index++;
                }
            }
        }
        #endregion

        #region Updating List
        /// <summary>
        /// Updates the list.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="tocParagraph">The toc paragraph.</param>
        private void UpdateList(IWParagraph paragraph, WParagraph tocParagraph)
        {
            WParagraph para = paragraph as WParagraph;
            WListFormat listFormat = null;
            WParagraphStyle pStyle = para.ParaStyle as WParagraphStyle;
            if (para.ListFormat.ListType != ListType.NoList || para.ListFormat.IsEmptyList)
                listFormat = para.ListFormat;
            else if (pStyle.ListFormat.ListType != ListType.NoList || pStyle.ListFormat.IsEmptyList)
                listFormat = pStyle.ListFormat;

            if (listFormat != null
                && listFormat.CurrentListStyle != null)
            {
                ListStyle listStyle = listFormat.CurrentListStyle;
                int levelNumber = 0;
                if (para.ListFormat.HasKey(WListFormat.ListLevelNumberKey))
                    levelNumber = para.ListFormat.ListLevelNumber;
                else if (pStyle.ListFormat.HasKey(WListFormat.ListLevelNumberKey))
                    levelNumber = pStyle.ListFormat.ListLevelNumber;

                WListLevel level = listStyle.GetNearLevel(levelNumber);

                ListOverrideStyle listOverrideStyle = null;
                if (listFormat.LFOStyleName != null
                    && listFormat.LFOStyleName.Length > 0)
                    listOverrideStyle = Document.ListOverrides.FindByName(listFormat.LFOStyleName);
                if (listOverrideStyle != null
                    && listOverrideStyle.OverrideLevels.HasOverrideLevel(levelNumber)
                    && listOverrideStyle.OverrideLevels[levelNumber].OverrideFormatting)
                    level = listOverrideStyle.OverrideLevels[levelNumber].OverrideListLevel;
                string value = Document.UpdateListValue(para, listFormat, level);

                if (value != string.Empty)
                    AddListValueAndTab(paragraph, tocParagraph, pStyle, value);
            }
        }
        /// <summary>
        /// Adds the list value and tab.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <param name="tocParagraph">The toc paragraph.</param>
        /// <param name="listValue">The list value.</param>
        private void AddListValueAndTab(IWParagraph paragraph, WParagraph tocParagraph, WParagraphStyle tocStyle, string listValue)
        {
            IWTextRange txtRange;
            txtRange = tocParagraph.AppendText(listValue);
            WParagraphStyle pStyle = (paragraph as WParagraph).ParaStyle as WParagraphStyle;
            if (!pStyle.CharacterFormat.HasValue(WCharacterFormat.BoldKey) && paragraph.BreakCharacterFormat.HasValue(WCharacterFormat.BoldKey))
                txtRange.CharacterFormat.Bold = paragraph.BreakCharacterFormat.Bold;
            if (!pStyle.CharacterFormat.HasValue(WCharacterFormat.ItalicKey) && paragraph.BreakCharacterFormat.HasValue(WCharacterFormat.ItalicKey))
                txtRange.CharacterFormat.Italic = paragraph.BreakCharacterFormat.Italic;
            txtRange.CharacterFormat.CharStyleName = "Hyperlink";

            int len = 0;
            if (tocParagraph.ParagraphFormat.HasValue(WParagraphFormat.LeftIndentKey))
                len = (int)tocParagraph.ParagraphFormat.LeftIndent;
            else if (tocParagraph.ParagraphFormat.BaseFormat != null && tocParagraph.ParagraphFormat.BaseFormat.HasValue(WParagraphFormat.LeftIndentKey))
                len = (int)(tocParagraph.ParagraphFormat.BaseFormat as WParagraphFormat).LeftIndent;
            DrawingContext dc = new DrawingContext();
            len += (int)dc.MeasureTextRange(txtRange as WTextRange, txtRange.Text).Width;
            len += 14;
            len = (int)Math.Ceiling((decimal)len / 11) * 11;
            if (pStyle.ParagraphFormat.Tabs.Count == 0)
                tocParagraph.ParagraphFormat.Tabs.AddTab(len, TabJustification.Left, TabLeader.NoLeader);
            txtRange = tocParagraph.AppendText("\t");
        }
        #endregion

        #endregion
#endif
        #region Overrides
        //#if !SILVERLIGHT
#if !SILVERLIGHT && !WP
        /// <summary>
        /// 
        /// </summary>
        protected override void CreateLayoutInfo()
        {
            m_layoutInfo = new LayoutInfo();
        }
#endif
        /// <summary>
        /// Registers child objects in XDSL holder.
        /// </summary>
        protected override void InitXDLSHolder()
        {
            if (m_invalidFormatString)
            {
                UpdateFormattingString();
            }
            XDLSHolder.AddElement(XDLSConstants.TOCFieldTag, m_tocField);
        }
        /// <summary>
        /// Writes the XML attributes.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);
            writer.WriteValue(XDLSConstants.TypeTag, ParagraphItemType.TOC);
        }
        //#endif
        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns>Returns cloned object.</returns>
        protected override object CloneImpl()
        {
            if (m_invalidFormatString)
            {
                UpdateFormattingString();
            }
            TableOfContent toc = (TableOfContent)base.CloneImpl();
            toc.m_tocField = (WField)this.m_tocField.Clone();
            return toc;
        }

        #endregion

        #region Layouting
#if !(SILVERLIGHT || WP)
        #region ILayoutInfo Members
        /// <summary>
        /// Gets/Sets the Text Size of the TextRange
        /// </summary>
        SizeF ILayoutInfo.Size
        {
            get
            {
                throw new NotImplementedException();

            }
            set
            {
                throw new NotImplementedException();

            }
        }
        bool ILayoutInfo.IsClipped
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        bool ILayoutInfo.IsVerticalText
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        bool ILayoutInfo.IsSkip
        {
            get
            {
                return true;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        bool ILayoutInfo.IsSkipBottomAlign
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        bool ILayoutInfo.IsLineContainer
        {
            get { throw new NotImplementedException(); }
        }

        ChildrenLayoutDirection ILayoutInfo.ChildrenLayoutDirection
        {
            get { throw new NotImplementedException(); }
        }

        bool ILayoutInfo.IsLineBreak
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        bool ILayoutInfo.TextWrap
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        bool ILayoutInfo.IsPageBreakItem
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        bool ILayoutInfo.IsFirstItemInPage
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        bool ILayoutInfo.IsKeepWithNext
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region ILayoutSpacingsInfo Members
        Spacings ILayoutSpacingsInfo.Paddings
        {
            get { throw new NotImplementedException(); }
        }

        Spacings ILayoutSpacingsInfo.Margins
        {
            get { throw new NotImplementedException(); }
        }
        #endregion
#endif
        #endregion
    }
}
