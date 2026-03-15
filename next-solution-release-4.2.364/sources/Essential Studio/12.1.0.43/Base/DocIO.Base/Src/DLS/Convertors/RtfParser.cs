#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.IO;
#if !WINRT && !WP
using System.Drawing;
#endif
using Syncfusion.DocIO.ReaderWriter.Biff_Records;
using System.Globalization;
namespace Syncfusion.DocIO.DLS.Convertors
{
   internal class RtfParser
    {
        #region Constants
        private const string c_groupStart = "{";
        private const string c_groupEnd = "}";
        private const string c_controlStart = "\\";
        private const string c_space = " ";
        private const string c_whiteSpace = "\r";
        private const string c_newLine = "\n";
        private const string c_semiColon = ";";
        #endregion

        #region fields
        private RtfLexer m_lexer;
        private RtfReader m_rtfReader;
        private string m_token;
        private string m_previousToken;
        private string m_previousTokenKey;
        private string m_previousTokenValue;
        private string m_previousControlString;
        private string m_fontID;
        private string m_fontName ;
        private string m_listID ;
        private int m_redID = 0;
        private int m_blueID = 0;
        private int m_greenID = 0;
        private bool m_bIsContinousList;
        private bool m_bIsPreviousList;
        private RtfTableType m_currentTableType=RtfTableType .None;
        Dictionary<string, RtfFont> m_fontTable = new Dictionary<string, RtfFont>();
        Dictionary<int, RtfColor> m_colorTable = new Dictionary<int,RtfColor>();
        Dictionary<int, CellFormat> m_cellFormatTable = new Dictionary<int, CellFormat>();
        private BodyItemCollection m_bodyItems;
        private IWParagraph m_currParagraph ;
        private IWSection m_currSection;
        private WordDocument m_document ;
        private TextFormat m_currTextFormat;
        private ShapeFormat m_currShapeFormat;
        private PictureFormat m_picFormat;
        private Stack<string> m_stack=new Stack<string>();
        private Stack<string> m_pictureStack = new Stack<string>();
        private Stack<string> m_destStack = new Stack<string>();
        private Stack<string> m_headerFooterStack = new Stack<string>();
        private Stack<Dictionary<int, CellFormat>> m_CellFormatStack = new Stack<Dictionary<int, CellFormat>>();
        private Stack<Dictionary<int, CellFormat>> m_prevCellFormatStack = new Stack<Dictionary<int, CellFormat>>();
        private RtfFont m_rtfFont;
        private RtfTokenType m_tokenType;
        private RtfTokenType m_prevTokenType;
        private RtfColor m_rtfColorTable;      
        private bool m_bIsBorderTop;
        private bool m_bIsBorderBottom;
        private bool m_bIsBorderLeft;
        private bool m_bIsBorderRight;
        private bool m_bIsPicture;
        private bool m_bIsShape;
        private bool m_bIsHorizontalBorder;
        private bool m_bIsVerticalBorder;
        private IWPicture m_currPicture;
        private WParagraphFormat m_prevFormat;
        private WListFormat m_prevListFormat;     
        private IWTextRange tr;
        private bool m_bIsDocumentInfo;
        private bool m_bIsShapePicture;
        private SecionFormat m_secFormat;
        private ListStyle m_currListStyle;
        private WListLevel m_currListLevel;
        private int m_currLevelIndex = -1;
        private Dictionary<string, ListStyle> m_listTable = new Dictionary<string, ListStyle>();
        private Dictionary<string, string> m_listOverrideTable = new Dictionary<string, string>();
        private Dictionary<string, IWParagraphStyle> m_styleTable = new Dictionary<string, IWParagraphStyle>();
        private Dictionary<string, CharacterStyle> m_charStyleTable = new Dictionary<string, CharacterStyle>();
        private string m_currStyleName;
        private bool m_bIsListText;
        private bool m_bIsList;
        private IWTable m_currTable;
        private WTableRow m_currRow;
        private WTableCell m_currCell;
        private int m_currRowIndex = -1;
        private int m_currCellIndex = -1;
        private int m_currCellFormatIndex = -1;
        private bool m_bIsRow ;
        private bool m_bIsLastRow ;
        private CellFormat m_currCellFormat;
        private RowFormat m_currRowFormat;
        private bool m_bIsBookmarkStart ;
        private bool m_bIsBookmarkEnd ;
        private  bool m_bIsHeader;
        private bool m_bIsFooter;
        private HeaderFooterType m_headerFooterType;
        private float m_tabPosition;
        private TabJustification m_tabJustification;
        private TabLeader m_tabLeader;
        private bool m_bIsLevelText;
        private WTextBody m_textBody;
        private int m_previousLevel;
        private int m_currentLevel;
        private bool m_bIsCustomProperties;
        private string m_currPropertyName ;
        private object m_currPropertyValue;
        private Syncfusion.CompoundFile.DocIO.PropertyType m_currPropertyType;
        private bool m_bInTable;
        private Stack<WTextBody> m_nestedTextBody = new Stack<WTextBody>();
        private Stack<WTable> m_nestedTable = new Stack<WTable>();
        private bool m_bCellFinished ;
        private bool m_bRowFinished ;
        private Column m_currColumn;
        private IWParagraphStyle m_currStyle;
        private CharacterStyle m_currCharStyle;
        private string m_currStyleID;
        private int m_secCount = 0;
        private int m_paraCount = 0;
        private Dictionary<int, TabFormat> m_tabCollection = new Dictionary<int, TabFormat>();
        private TabFormat m_currTabFormat;
        private int m_tabCount = 0;
        private bool m_bIsLinespacingRule ;
        private bool m_bIsAccentChar ;
        private int m_currCellBoundary = 0;
        private int m_currRowLeftIndent = 0;
        private Stack<RowFormat> m_currRowFormatStack = new Stack<RowFormat>();
        private Stack<RowFormat> m_prevRowFormatStack = new Stack<RowFormat>();
        private Stack<string> m_backgroundCollectionStack = new Stack<string>();
        private bool m_bIsBackgroundCollection;
        private bool m_bIsMetaFile ;
        private bool m_bIsDefaultSectionFormat;
        private SecionFormat m_defaultSectionFormat;
        private bool m_isLevelOld;
        private string m_styleName;
        private Stack<string> m_listLevelStack = new Stack<string>();
        private bool m_bIsListLevel;
        private IWParagraph m_prevParagraph;
        private TextFormat m_prevTextFormat;
        private WParagraphFormat m_listLevelParaFormat;
        private WCharacterFormat m_listLevelCharFormat;
        private int m_pnLevelNumber = -1;
        private FormFieldData m_currentFormField;
        private int m_checkboxCount = 1;
        private int m_dropDownCount = 1;
        private int m_textfieldCount = 1;
        private Stack<int> m_unicodeCountStack = new Stack<int>();
        private int m_unicodeCount = 0;
        private bool m_bIsUnicode = false;
        private int m_currColorIndex = -1;
        private Stack<Dictionary<int, TabFormat>> m_tabFormatStack = new Stack<Dictionary<int, TabFormat>>();
        private Stack<TextFormat> m_textFormatStack = new Stack<TextFormat>();
        private Stack<string> m_rtfCollectionStack = new Stack<string>();
        private Stack<string> m_shapeInstructionStack = new Stack<string>();
        private bool m_bIsShapeInstruction;
       private bool m_bIsShapePictureAdded;
        private Stack<string> m_objectStack = new Stack<string>();
        private bool m_bIsObject;
        private bool m_bIsStandardPictureSizeNeedToBePreserved;
        private string m_drawingFieldName;
        private string m_drawingFieldValue;
        private Stack<int> m_fieldResultGroupStack = new Stack<int>();
        private Stack<int> m_fieldInstructionGroupStack = new Stack<int>();
        private Stack<int> m_fieldGroupStack = new Stack<int>();
        private Stack<WField> m_fieldCollectionStack = new Stack<WField>();
        private Stack<string> m_formFieldDataStack = new Stack<string>();
        private string m_currentFieldGroupData;
        private Stack<FieldGroupType> m_fieldGroupTypeStack = new Stack<FieldGroupType>();
        private string m_defaultCodePage;
        private int m_defaultFontIndex;
        private bool m_bIsRowBorderTop;
        private bool m_bIsRowBorderBottom;
        private bool m_bIsRowBorderLeft;
        private bool m_bIsRowBorderRight;
        private float m_leftcellspace = 0;
        private float m_rightcellspace = 0;
        private float m_bottomcellspace = 0;
        private float m_topcellspace = 0;
        private bool m_bIsWord97StylePadding = false;
        #endregion


        #region Properties
        /// <summary>
        /// Get's or set's the Default code page of the document
        /// </summary>
        internal string DefaultCodePage
        {
            get
            {
                if (m_defaultCodePage == null)
#if WINRT 
                    return "windows-1252";
#elif !SILVERLIGHT && !WP
                    if (IsSupportedCodePage(CultureInfo.CurrentCulture.TextInfo.ANSICodePage))
                        return GetSupportedCodePage(CultureInfo.CurrentCulture.TextInfo.ANSICodePage);
                    else
                        return "windows-1252";
#else
                    return m_defaultCodePage;
#endif
                return m_defaultCodePage;
            }
            set
            {
                m_defaultCodePage = value;
            }
        }
       /// <summary>
       /// Get's or set's the Default font index value
       /// </summary>
        internal int DefaultFontIndex
        {
            get
            {
                return m_defaultFontIndex;
            }
            set
            {
                m_defaultFontIndex = value;
            }
        }
       /// <summary>
       /// Checks whether the control word is destination control word
       /// </summary>
        public bool IsDestinationControlWord
        {
            get
            {
                if (m_destStack.Count > 0)
                    return true;
                else
                    return false;
            }
        }
       
       /// <summary>
       /// Gets whether the group is form field group
       /// </summary>
        public bool IsFormFieldGroup
        {
            get
            {
                if (m_formFieldDataStack.Count > 0)
                    return true;
                else
                    return false;
            }
        }
       /// <summary>
       /// Gets whether the group is field group
       /// </summary>
        public bool IsFieldGroup
        {
            get
            {
                if (m_fieldGroupStack.Count > 0)
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
       /// Gets the current paragraph level
       /// </summary>
        public int CurrentLevel
        {
            get
            {
                return m_currentLevel;
            }
        }
       /// <summary>
       /// Gets the previous paragraph level
       /// </summary>
        public int PreviousLevel
        {
            get
            {
                return m_previousLevel;
            }
        }
       /// <summary>
       /// Gets the current TextBody
       /// </summary>
        public WTextBody CurrentTextBody
        {
            get
            {
                return m_textBody;
            }
        }
        /// <summary>
        /// Gets the current para.
        /// </summary>
        /// <value>The current para.</value>
        protected IWParagraph CurrentPara
        {
            get
            {
                if (m_currParagraph == null)
               {
                   m_currParagraph = new WParagraph(m_document);              
               }

                return m_currParagraph;
            }
            set
            {
                m_currParagraph = value;
            }
        }
       /// <summary>
       /// Gets & Sets the current column
       /// </summary>
        protected Column CurrColumn
        {
            get
            {
                if (m_currColumn == null)
                    m_currColumn = CurrentSection.AddColumn(1, 1);
                return m_currColumn;                  
            }
            set
            {
                m_currColumn = value;
            }
        }
       /// <summary>
       /// Gets the current section
       /// </summary>
        protected IWSection CurrentSection
        {
            get
            {
                if (m_currSection == null)
                {
                    m_currSection = m_document.AddSection();
                    m_currSection.PageSetup.EqualColumnWidth = true;
                    m_textBody = m_currSection.Body;
                }
                return m_currSection;
            }
        }

       /// <summary>
       /// Gets and sets the current list style
       /// </summary>
        protected ListStyle CurrListStyle
        {
            get
            {
                return m_currListStyle;
            }
            set
            {
                m_currListStyle = value;
            }

        }
       /// <summary>
       /// Gets and Sets the current list level
       /// </summary>
        protected WListLevel CurrListLevel
        {
            get
            {
                return m_currListLevel;
            }
            set
            {
                m_currListLevel = value;
            }
        }
       /// <summary>
       /// Gets and Sets the current Rtf font
       /// </summary>
        public RtfFont CurrRtfFont
        {
            get
            {
                return m_rtfFont;
            }
            set
            {
                m_rtfFont = value;
            }
        }
       /// <summary>
       /// Gets and sets the current color table
       /// </summary>
        public RtfColor CurrColorTable
        {
            get
            {
                return m_rtfColorTable;
            }
            set
            {
                m_rtfColorTable = value;
            }
        }
       /// <summary>
       /// Gets and Sets the current table
       /// </summary>
        public IWTable CurrTable
        {
            get
            {
                return m_currTable;
            }
            set
            {
                m_currTable = value;
            }
        }
       /// <summary>
       /// Gets and Sets the current row
       /// </summary>
        public WTableRow CurrRow
        {
            get
            {
               // if (m_currRow == null)
               //     m_currRow = new WTableRow(m_document);
                return m_currRow;
            }
            set
            {
                m_currRow = value;
            }
        }
       /// <summary>
       /// Gets and Sets the current cell
       /// </summary>
        public WTableCell CurrCell
        {
            get
            {
                return m_currCell;
            }
            set
            {
                m_currCell = value;
            }
        }
       /// <summary>
       /// Gets and Sets the current cell format
       /// </summary>
        public CellFormat CurrCellFormat
        {
            get
            {
                return m_currCellFormat;
            }
            set
            {
                m_currCellFormat = value;
            }
        }
       /// <summary>
       /// Gets/Sets the current row format
       /// </summary>
        public RowFormat CurrRowFormat
        {
            get
            {
                if (m_currRowFormat == null)
                    m_currRowFormat = new RowFormat(m_document);
                return m_currRowFormat; 
            }
            set
            {
                m_currRowFormat = value;
            }
        }
        /// <summary>
        /// Gets/Sets the current tab format
        /// </summary>
        public TabFormat CurrTabFormat
        {
            get
            {
                if (m_currTabFormat == null)
                    m_currTabFormat = new TabFormat();
                return m_currTabFormat;
            }
            set
            {
                m_currTabFormat = value;
            }
        }
        #endregion


        #region Constructor
        public RtfParser(WordDocument document, Stream stream)
        {
            m_rtfReader = new RtfReader(stream);
            m_lexer = new RtfLexer(m_rtfReader);
            m_document = document;
            m_currTextFormat = new TextFormat();
            m_secFormat = new SecionFormat();
            CurrTable = null;
            m_currRow = null;
            m_currCell = null;
        }
        #endregion


        #region Implementation

        /// <summary>
        /// Parse each token from the rtf
        /// </summary>
        public void ParseToken()
        {
            //Set Compatability option default value for RTF document
            m_document.DOP.Dop2000.Copts.DontUseHTMLParagraphAutoSpacing = true;
            m_token = m_lexer.ReadNextToken(m_previousTokenKey);
            while (m_rtfReader.Position <= m_rtfReader.Length)
            {
                if (m_token == c_groupStart)
                {
                    m_tokenType = RtfTokenType.GroupStart;
                    ParseGroupStart();              
                }
                else if (m_token == c_groupEnd)
                {
                    m_tokenType = RtfTokenType.GroupEnd;
                    ParseGroupEnd();                 
                    if (m_rtfCollectionStack.Count == 0)
                        break;
                }
                else if (m_token == c_semiColon)
                {
                    if (m_previousToken == "colortbl")
                        m_currColorIndex = 0;
                    if (m_bIsLevelText == true)
                        m_bIsLevelText = false;
                    m_tokenType = RtfTokenType.Unknown;
                    m_lexer.CurrRtfTokenType = RtfTokenType.Unknown;
                    if (m_currentTableType == RtfTableType.ColorTable && m_previousTokenKey == "blue")
                    {
                        AddColorTableEntry();
                    }
                    else if (m_currentTableType == RtfTableType.FontTable)
                    {
                        AddFontTableEntry();                     
                    }
                    else if (m_currentTableType == RtfTableType.StyleSheet)
                    {
                        AddStyleSheetEntry();
                    }
                    else
                    {
                        ParseDocumentElement(m_token);
                    }
                }
                else if (m_token.StartsWith(c_controlStart))
                {
                    m_tokenType = RtfTokenType.ControlWord;
                    ParseControlStart();                   
                }
                else if (m_token == c_whiteSpace || m_token == c_newLine || m_token == string.Empty)
                {
                    m_tokenType = RtfTokenType.Unknown;
                }
                else
                {
                    ParseDocumentElement(m_token);
                }
                // Parse paragraph end whether the previous token as empty control word.
                if (m_previousToken == string.Empty && m_prevTokenType == RtfTokenType.ControlWord
                    && (m_token == c_whiteSpace || m_token == c_newLine))
                    ParseParagraphEnd();
                if (m_token != null && m_token != c_whiteSpace && m_token != c_newLine)
                    m_previousControlString = m_token;
                if (m_token != null && m_tokenType == RtfTokenType.ControlWord && m_token != " ")
                {
                    m_previousToken = m_token;
                }
                if (m_token == "emdash" || m_token == "endash")
                {
                    m_prevTokenType = RtfTokenType.Text;
                }
                else if (!(m_prevTokenType == RtfTokenType.Text && (m_token == c_whiteSpace || m_token == c_newLine)))
                    m_prevTokenType = m_tokenType;
                m_token = m_lexer.ReadNextToken(m_previousTokenKey);
                if (m_token.Contains("\\formfield"))
                {
                    m_currentFormField = new FormFieldData();
                 
                    m_formFieldDataStack.Push(c_groupStart);
                }
                if (m_token == "\\jpegblip"
                    || m_token == "\\wmetafile8"
                    || m_token =="\\blipuid" 
                    || m_token.StartsWith("\\pngblip") 
                    || m_token =="\\emfblip" 
                    ||m_token =="\\macpict")
                {
                    m_lexer.IsImageBytes = true; 
                }
                if ((m_token == "\\wmetafile8" || m_token == "\\emfblip")
                    && m_bIsShapePicture )
                    m_bIsMetaFile = true;
                if (m_token == "\\wmetafile8")
                    m_bIsStandardPictureSizeNeedToBePreserved = true;

            }
            //Closes Rtf Lexer instance
            m_lexer.Close();
            // Parse Paragraph End whether the current text body last item is not added into the current section body items.
            m_currentLevel = 0;
            m_bInTable = false;
            if (m_textBody !=null && m_textBody.Items.LastItem != CurrentSection.Body.Items.LastItem)
                ParseParagraphEnd();
            if (CurrentPara.Items.Count > 0)
                CurrentSection.Paragraphs.Add(CurrentPara);
            AddNewSection(CurrentSection);
            ApplySectionFormatting(); 
            Close();
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            m_lexer = null;
            m_rtfReader = null;
            m_previousToken = null;
            m_previousTokenKey = null;
            m_previousTokenValue = null;
            m_fontTable.Clear();
            m_listOverrideTable.Clear();
            m_listTable.Clear();
            m_colorTable.Clear();
            m_styleTable.Clear();
            m_fontTable = null;
            m_listOverrideTable = null;
            m_listTable = null;
            m_colorTable = null;
            m_styleTable = null;
            m_tabFormatStack.Clear();
            m_tabFormatStack = null;
            m_stack.Clear();
            m_stack = null;
            m_textFormatStack.Clear();
            m_textFormatStack = null;
            m_unicodeCountStack.Clear();
            m_unicodeCountStack = null;
            m_shapeInstructionStack.Clear();
            m_shapeInstructionStack = null;
            m_rtfCollectionStack.Clear();
            m_rtfCollectionStack = null;
            m_prevRowFormatStack.Clear();
            m_prevRowFormatStack = null;
            m_prevCellFormatStack.Clear();
            m_prevCellFormatStack = null;
            m_pictureStack.Clear();
            m_pictureStack = null;
            m_listLevelStack.Clear();
            m_listLevelStack = null;
            m_headerFooterStack.Clear();
            m_headerFooterStack = null;
            m_formFieldDataStack.Clear();
            m_formFieldDataStack = null;
            m_fieldResultGroupStack.Clear();
            m_fieldResultGroupStack = null;
            m_fieldGroupTypeStack.Clear();
            m_fieldGroupTypeStack = null;
            m_fieldGroupStack.Clear();
            m_fieldGroupStack = null;
            m_fieldCollectionStack.Clear();
            m_fieldCollectionStack = null;
            m_destStack.Clear();
            m_destStack = null;
            m_currRowFormatStack.Clear();
            m_currRowFormatStack = null;
            m_CellFormatStack.Clear();
            m_CellFormatStack = null;
        }
       /// <summary>
       /// Add font to the font table collection
       /// </summary>
        private void AddFontTableEntry()
        {
            bool isDuplicate = false;
            foreach (KeyValuePair<string, RtfFont> fontTable in m_fontTable)
            {
                if (fontTable.Key == m_rtfFont.FontID)
                {
                    isDuplicate = true;
                }
            }
            if (!isDuplicate && m_rtfFont.FontName != null && m_rtfFont.FontID != null)
            {
                m_rtfFont.FontName = m_rtfFont.FontName.Trim();
                m_fontTable.Add(m_rtfFont.FontID, m_rtfFont);
                //Update Alternate font
                if (m_rtfFont.AlternateFontName != null)
                {
                    if (m_document.FontSubstitutionTable.ContainsKey(m_rtfFont.FontName))
                        m_document.FontSubstitutionTable[m_rtfFont.FontName] = m_rtfFont.AlternateFontName;
                    else
                        m_document.FontSubstitutionTable.Add(m_rtfFont.FontName, m_rtfFont.AlternateFontName);
                }
            }

        }
       /// <summary>
       /// Add color to the color table
       /// </summary>
        private void AddColorTableEntry()
        {
            m_colorTable.Add(++ m_currColorIndex, m_rtfColorTable);
            m_rtfColorTable = new RtfColor();

        }
       /// <summary>
       /// Add styles to the style table collection
       /// </summary>
        private void AddStyleSheetEntry()
        {
            if (m_styleName == null || (m_styleName != null && m_styleName.Length == 0))
                m_styleName = "Style" + Guid.NewGuid().ToString();
            if (m_currStyle != null)
            {
                if ((m_currStyle as Style).BuiltinStyles.ContainsKey(m_styleName.ToLower()))
                    m_styleName = (m_currStyle as Style).BuiltinStyles[m_styleName.ToLower()];
                (m_currStyle as Style).SetStyleName(m_styleName);
                //TODO:Need to create an independant WParagraphFormat object to populate the paragraph formatting 
                //defined in the document as like m_currTextFormat (CharacterFormat)
                //Below implementation of copying the paragraph format of current paragraph to current style misleads.
                if (!IsStylePresent(m_currStyle.Name))
                {
                    IWParagraphStyle paraStyle = m_document.AddParagraphStyle(m_currStyle.Name);
                    CopyParagraphFormatting(m_currParagraph.ParagraphFormat, paraStyle.ParagraphFormat);
                    //Update Paragraph tabs collection
                    UpdateTabsCollection(paraStyle.ParagraphFormat);
                    CopyTextFormatToCharFormat(paraStyle.CharacterFormat, m_currTextFormat);
                    if (!m_styleTable.ContainsKey(m_currStyleID))
                        m_styleTable.Add(m_currStyleID, m_currStyle);

                }
                else
                {
                    CopyParagraphFormatting(m_currParagraph.ParagraphFormat, m_currStyle.ParagraphFormat);
                    //Update Paragraph tabs collection
                    UpdateTabsCollection(m_currStyle.ParagraphFormat);
                }
            }           
            if (m_currCharStyle != null)
            {
                m_currCharStyle.SetStyleName(m_styleName);
                CopyTextFormatToCharFormat(m_currCharStyle .CharacterFormat ,m_currTextFormat);
                if (!IsStylePresent(m_currCharStyle.Name))
                {
                    m_document.Styles.Add(m_currCharStyle as Style);                  
                    if (!m_charStyleTable.ContainsKey(m_currStyleID))
                        m_charStyleTable.Add(m_currStyleID, m_currCharStyle);
                }
            }
            m_styleName = string.Empty;
            m_currStyle = null;
            m_currCharStyle = null;
        }
        /// <summary>
        /// Check whether the style is present in the document
        /// </summary>
        /// <param name="styleName"></param>
        /// <returns></returns>
        private bool IsStylePresent(string styleName)
        {
            foreach (Style style in m_document.Styles)
            {
                if (style.Name == styleName)
                    return true;
            }
            return false;

        }
       /// <summary>
       /// Parse Control start
       /// </summary>
        private void ParseControlStart()
        {
            m_bIsAccentChar = false;
            m_lexer.CurrRtfTokenType = RtfTokenType.ControlWord;
            if (m_token.EndsWith("?"))
                m_token = m_token.TrimEnd('?');
            m_token = m_token.Trim();
            m_token = m_token.Substring(1);
            if (m_token == c_controlStart || m_token ==c_groupStart|| m_token ==c_groupEnd )
                ParseDocumentElement(m_token);
            if (m_token == "*")
            {
                if (!IsDestinationControlWord)
                {
                    m_destStack.Push(c_groupStart );
                }
            }
            string[] value = SeperateToken(m_token);
          
            ParseControlWords(m_token, value[0], value[1]);
          
            m_previousTokenKey = value[0];
            m_previousTokenValue = value[1];
        }
       /// <summary>
       /// Gets whether the group is  a nested group
       /// </summary>
       /// <returns></returns>
        private bool IsNestedGroup()
        {
            if (m_currentTableType != RtfTableType.None 
                || m_bIsListText 
                || m_bIsDocumentInfo 
                || m_bIsCustomProperties)
                return true;
            else
                return false;
        }
       /// <summary>
       /// Parse group start
       /// </summary>
        private void ParseGroupStart()
        {
            m_lexer.CurrRtfTokenType = RtfTokenType.GroupStart;            
           if(IsNestedGroup ())
            {
                m_stack.Push(c_groupStart );
            }
            if (m_bIsPicture)
               m_pictureStack.Push(c_groupStart);
            if (IsDestinationControlWord)
                m_destStack.Push(c_groupStart );
            if (m_bIsHeader || m_bIsFooter)
                m_headerFooterStack.Push(c_groupStart );
            if (IsFieldGroup)
            {
                //The following stack is maintained to increment the stack value for each group start within \fldrslt group and to ensure the end of \fldrslt group.
                if (m_fieldResultGroupStack.Count > 0)
                    m_fieldResultGroupStack.Push(m_fieldResultGroupStack.Pop() + 1);
                //The following stack is maintained to increment the stack value for each group start within \fldinst group and to ensure the end of \fldinst group.
                if (m_fieldInstructionGroupStack.Count > 0)
                    m_fieldInstructionGroupStack.Push(m_fieldInstructionGroupStack.Pop() + 1);
                //The following stack is maintained to increment the stack value for each group start within \field group and to ensure the end of \field group.
                if (m_fieldGroupStack.Count > 0)
                    m_fieldGroupStack.Push(m_fieldGroupStack.Pop() + 1);
            }
            if (m_bIsListLevel)
                m_listLevelStack.Push(c_controlStart);
            if (m_bIsBackgroundCollection)
                m_backgroundCollectionStack.Push(c_controlStart);
            if (m_currentTableType ==RtfTableType.None)
            {
                m_textFormatStack.Push(m_currTextFormat.Clone());
                m_currTextFormat = m_textFormatStack.Peek();
                m_tabFormatStack.Push(new Dictionary<int, TabFormat>(m_tabCollection));
                m_tabCollection = m_tabFormatStack.Peek();
            }
            if (m_rtfCollectionStack.Count > 0)
                m_rtfCollectionStack.Push(c_controlStart);

            if (m_bIsShapeInstruction)
                m_shapeInstructionStack.Push(c_groupStart );

            if (m_bIsObject)
                m_objectStack.Push(c_groupStart );

            if (IsFormFieldGroup)
                m_formFieldDataStack.Push(c_groupStart);
            
          
        }
       /// <summary>
       /// Parse Group End
       /// </summary>
        private void ParseGroupEnd()
        {
            m_lexer.CurrRtfTokenType = RtfTokenType.GroupEnd;
            if (m_unicodeCountStack.Count > 0)
            {
                m_bIsUnicode = false;
                m_unicodeCountStack.Pop();
            }
            m_unicodeCount = 0;
           if(IsNestedGroup ())
            {
                m_stack.Pop();
                if (m_stack.Count == 0)
                {
                    if (m_bIsListText)
                    {
                        m_bIsListText = false;
                        CopyParagraphFormatting(CurrentPara.ParagraphFormat, m_listLevelParaFormat);
                        CopyTextFormatToCharFormat(m_listLevelCharFormat, m_currTextFormat);
                        CurrentPara = m_prevParagraph;
                        m_currTextFormat = m_prevTextFormat;
                       
                    }                   
                    m_bIsDocumentInfo = false;
                    m_bIsCustomProperties = false;
                    m_currentTableType = RtfTableType.None;
                    m_lexer.CurrRtfTableType = RtfTableType.None;
                }
            }
            if (IsDestinationControlWord)
            {
                m_destStack.Pop();
            }
            if (m_currentTableType == RtfTableType.StyleSheet)
            {
                m_tabCollection.Clear();
                m_tabCount = 0;
                m_tabFormatStack.Clear();
            }
            if (m_bIsHeader || m_bIsFooter)
            {
                m_headerFooterStack.Pop();
                if (m_headerFooterStack.Count == 0)
                {
                    // Parse Paragraph End whether the current paragraph is not added into the text body items
                    if (m_currParagraph != null && m_currParagraph.ChildEntities.Count != 0)
                    {
                        ParseParagraphEnd();
                    }
                    m_bIsHeader = false;
                    m_bIsFooter = false;
                    m_textBody = null;
                    // Intializes the current text format
                    m_currTextFormat = new TextFormat();
                    m_textFormatStack.Clear();
                    m_textFormatStack.Push(m_currTextFormat);
                    m_tabFormatStack.Clear();
                }
            }
            if (IsFieldGroup)
            {
                ParseGroupEndWithinFieldGroup();
            }
            if (m_bIsListLevel)
            {
                if(m_listLevelStack .Count > 0)
                    m_listLevelStack.Pop();
                if (m_listLevelStack.Count == 0)
                {
                    CopyParagraphFormatting(CurrentPara.ParagraphFormat, CurrListLevel.ParagraphFormat);
                    CopyTextFormatToCharFormat(CurrListLevel.CharacterFormat, m_currTextFormat);
                    CurrentPara = null;
                    m_currTextFormat = new TextFormat();
                    m_bIsListLevel = false;
                }
                if (m_bIsLevelText)
                    m_bIsLevelText = false;
            }
            if (m_bIsPicture)
            {
                if (m_pictureStack.Count > 0)
                    m_pictureStack.Pop();
                if (m_pictureStack.Count == 0)
                {
                    m_bIsPicture = false;
                    m_lexer.IsImageBytes = false;
                }
            }
            if (m_bIsBackgroundCollection)
            {
                if (m_backgroundCollectionStack .Count >0)
                    m_backgroundCollectionStack.Pop();
                if (m_backgroundCollectionStack.Count == 0)
                {
                    m_bIsBackgroundCollection = false;
                }
            }
            if (m_currentTableType == RtfTableType.None)
            {
                if (m_textFormatStack.Count > 1)
                {
                    m_textFormatStack.Pop();                  
                }
                if (m_textFormatStack.Count > 0)
                    m_currTextFormat = m_textFormatStack.Peek();
                if (m_tabFormatStack.Count > 1)
                {
                    m_tabFormatStack.Pop();
                }
                if (m_tabFormatStack.Count > 0)
                    m_tabCollection = m_tabFormatStack.Peek();
            }
            if (m_drawingFieldName != null && m_drawingFieldValue != null)
            {
                if (m_currPicture != null && m_bIsPicture && m_bIsShapePicture)
                    ParseShapeToken(m_drawingFieldName, m_drawingFieldName, m_drawingFieldValue);
                m_drawingFieldValue = null;
                m_drawingFieldName = null;
            }
            if (m_rtfCollectionStack.Count > 0)
                m_rtfCollectionStack.Pop();      

            if (m_bIsShapeInstruction)
            {
                m_shapeInstructionStack.Pop();
                if (m_shapeInstructionStack.Count == 0)
                    m_bIsShapeInstruction = false;                    
            }

            if (m_bIsObject)
            {
                m_objectStack.Pop();
                if (m_objectStack.Count == 0)
                    m_bIsObject = false;
            }

            if (IsFormFieldGroup)
            {
                m_formFieldDataStack.Pop();   
                if (m_formFieldDataStack.Count == 0)
                {
                    WriteFormFieldProperties();
                }
            }
        }
        /// <summary>
        /// Parse group end within field group
        /// </summary>
        private void ParseGroupEndWithinFieldGroup()
        {
            if (m_currentFieldGroupData != string.Empty)
            {
                ParseFieldGroupData(m_currentFieldGroupData);
                m_currentFieldGroupData = string.Empty;
            }
            EnsureFieldSubGroupEnd (FieldGroupType.FieldResult  );
            EnsureFieldSubGroupEnd (FieldGroupType.FieldInstruction );
            EnsureFieldGroupEnd ();
        }
       /// <summary>
       /// Ensure whether the end of field sub group(field instruction and field end)
       /// </summary>
       /// <param name="groupStack"></param>
       /// <param name="fieldGroupType"></param>
        private void EnsureFieldSubGroupEnd(FieldGroupType fieldGroupType)
        {
            int groupCount;
            switch (fieldGroupType)
            {
                case FieldGroupType .FieldInstruction :
                    if (m_fieldInstructionGroupStack.Count > 0)
                    {
                        groupCount = m_fieldInstructionGroupStack.Pop();
                        groupCount--;
                        if (groupCount == 0)
                        {
                            if (m_fieldGroupTypeStack.Count > 0 && m_fieldGroupTypeStack.Peek() == fieldGroupType)
                                m_fieldGroupTypeStack.Pop();
                        }
                        else
                        {
                            m_fieldInstructionGroupStack.Push(groupCount);
                        }
                    }
                    break;
                case FieldGroupType .FieldResult :
                    if (m_fieldResultGroupStack.Count > 0)
                    {
                        groupCount = m_fieldResultGroupStack.Pop();
                        groupCount--;
                        if (groupCount == 0)
                        {
                            if (m_fieldGroupTypeStack.Count > 0 && m_fieldGroupTypeStack.Peek() == fieldGroupType)
                                m_fieldGroupTypeStack.Pop();
                        }
                        else
                        {
                            m_fieldResultGroupStack.Push(groupCount);
                        }
                    }
                    break;
            }
        }
        /// <summary>
        /// Ensure field group end
        /// </summary>
        private void EnsureFieldGroupEnd()
        {
            int groupCount;
            groupCount = m_fieldGroupStack.Pop();
            groupCount--;
            if (groupCount == 0)
            {
                if (m_fieldCollectionStack.Count > 0
                    && m_fieldCollectionStack.Peek().FieldType != FieldType.FieldMergeField
                    && m_fieldCollectionStack.Peek().FieldType != FieldType.FieldNext
                    && m_fieldCollectionStack.Peek().FieldType != FieldType.FieldShape)
                {
                    if (CurrentPara.Items.Count > 0)
                    {
                        //The following code removes FieldSeperator from the current paragraph, if the empty \fldrslt group encounters
                        if (CurrentPara.Items.LastItem != null
                            && CurrentPara.Items.LastItem.EntityType == EntityType.FieldMark
                            && (CurrentPara.Items.LastItem as WFieldMark).Type == FieldMarkType.FieldSeparator)
                            CurrentPara.Items.Remove(CurrentPara.Items.LastItem);
                    }
                    WFieldMark fieldMark = new WFieldMark(m_document);
                    fieldMark.Type = FieldMarkType.FieldEnd;
                    CurrentPara.ChildEntities.Add(fieldMark);
                    //Set FieldEnd for the WField object
                    m_fieldCollectionStack.Peek().FieldEnd = fieldMark;
                }
                if (m_fieldCollectionStack.Count > 0)
                {
                    m_fieldCollectionStack.Peek().ParseFieldCode(m_fieldCollectionStack.Peek().FieldCode);
                    m_fieldCollectionStack.Pop();
                }
            }
            else
                m_fieldGroupStack.Push(groupCount);
        }
        
        /// <summary>
        /// Write Form field properties
        /// </summary>
        private void WriteFormFieldProperties()
        {
            WFormField formField = null;
            if (m_fieldCollectionStack.Count > 0)
                formField = m_fieldCollectionStack.Peek() as WFormField;
            if (formField == null)
                return;
            ApplyFormFieldProperties(formField);
            switch (formField.FormFieldType)
            {
                case FormFieldType.TextInput:
                    ApplyTextFormFieldProperties(formField as WTextFormField);
                    break;
                case FormFieldType.DropDown:
                    ApplyDropDownFormFieldProperties(formField as WDropDownFormField);
                    break;
                case FormFieldType.CheckBox:
                    ApplyCheckboxPorperties(formField as WCheckBox);
                    break;
            }
        }
        /// <summary>
        /// Remove Delimiter space from the document text
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private string RemoveDelimiterSpace(string token)
        {
            if (!(m_previousControlString.StartsWith("u") && m_previousControlString.Length > 1
                && char.IsNumber(m_previousControlString[1]) && m_previousTokenKey == "u")
                && ((m_previousControlString == c_groupEnd || m_lexer.CurrRtfTokenType == RtfTokenType.Text || m_token.StartsWith("u"))
                || (m_tokenType == RtfTokenType.Unknown
                && (!m_bIsListText && token != null && !m_bIsBackgroundCollection))))
                return token;
            else if (m_token.Length > 1)
            {
                //Removes delimited space and extract remaining string
                token = token.Substring(1, token.Length - 1);
                return token;
            }
            else
                return null;

        }
       /// <summary>
       /// Identify whether the token is a picture token
       /// </summary>
       /// <returns></returns>
        private bool IsPictureToken()
        {
            if (m_bIsPicture && m_bIsShapePicture && m_lexer.IsImageBytes)
                return true;
            else
                return false;
        }

       /// <summary>
       /// Parse document element
       /// </summary>
        private void ParseDocumentElement(string m_token)
        {
             if (m_token.StartsWith(" "))
                  m_token = RemoveDelimiterSpace(m_token);
             if (m_previousControlString.StartsWith("u") && m_previousControlString.Length > 1
                 && char.IsNumber(m_previousControlString[1]) && m_previousTokenKey == "u")
             {
                 if (m_bIsUnicode)
                     m_bIsUnicode = false;
                 else if (m_token != null && m_token.Length >= 1)
                     m_token = m_token.Substring(1);
             }
             if (!m_bIsListText && m_token !=null  && !m_bIsBackgroundCollection )
                    {
                        m_tokenType = RtfTokenType.Text;
                        m_lexer.CurrRtfTokenType = RtfTokenType.Text;
                        if (IsPictureToken())
                            ParseImageBytes();
                        else if (m_bIsBookmarkStart)
                        {
                            CurrentPara.AppendBookmarkStart(m_token);
                            m_bIsBookmarkStart = false;
                        }
                        else if (m_bIsBookmarkEnd)
                        {
                            if (!(CurrentPara.ChildEntities.LastItem is BookmarkEnd
                                && (CurrentPara.ChildEntities.LastItem as BookmarkEnd).Name == m_token))
                            {
                                CurrentPara.AppendBookmarkEnd(m_token);
                            }
                            m_bIsBookmarkEnd = false;
                        }
                        else if (m_bIsCustomProperties)
                        {
                            ParseCustomDocumentProperties();
                        }
                        else if (m_bIsDocumentInfo)
                        {
                            ParseBuiltInDocumentProperties();
                        }
                        else if (IsFormFieldGroup && m_currentFormField != null)
                        {
                            ParseFormFieldDestinationWords(m_token);
                        }
                        else if (IsFieldGroup && !m_bIsPicture)
                        {
                            m_currentFieldGroupData += m_token;
                        }
                        else if (m_currentTableType == RtfTableType.None 
                            && !IsDestinationControlWord
                            && !m_bIsDocumentInfo
                            && !m_bIsPicture)
                        {
                            if (m_previousToken.StartsWith("'") && m_bIsAccentChar)
                            {
                                m_token = " " + m_token;
                                m_bIsAccentChar = false;
                            }
							if (m_prevTokenType == RtfTokenType.Text && tr!=null)
                            {
                                tr.Text += m_token;
                            }
                            else
                            {
                                //Update List format of the paragraph
                                if (m_bIsList && CurrentPara.ListFormat != null
                                    && CurrentPara.ListFormat.CurrentListLevel == null
                                    && CurrentPara.ListFormat.CurrentListStyle == null)
                                    CurrentPara.ListFormat.ContinueListNumbering();
                                tr = CurrentPara.AppendText(m_token);
                                CopyTextFormatToCharFormat(tr.CharacterFormat, m_currTextFormat);
                            }
                            m_bIsBookmarkEnd = false;
                        }
                        //Update Current List level suffix character
                        else if (IsDestinationControlWord && m_previousTokenKey == "pntxta"
                                && CurrentPara.ListFormat != null && CurrentPara.ListFormat.CurrentListLevel != null)
                            CurrentPara.ListFormat.CurrentListLevel.NumberSufix = m_token;
                        //Update Current List level prefix character
                        else if (IsDestinationControlWord && m_previousTokenKey == "pntxtb"
                                && CurrentPara.ListFormat != null && CurrentPara.ListFormat.CurrentListLevel != null)
                            CurrentPara.ListFormat.CurrentListLevel.NumberPrefix = m_token;
                        else if (m_currentTableType == RtfTableType.FontTable && !IsDestinationControlWord)
                        {
                            m_rtfFont.FontName += m_token;
                        }
                        else if (m_currentTableType == RtfTableType.FontTable && m_previousTokenKey == "falt")
                        {
                            m_rtfFont.AlternateFontName += m_token;
                        }
                        else if (m_currentTableType == RtfTableType.StyleSheet)
                        {
                            m_styleName += m_token;
                        }
                        else if (m_previousToken == "sn" && m_bIsPicture && m_bIsShapePicture)
                        {
                            m_drawingFieldName = m_token;
                        }
                        else if (m_previousToken == "sv" && m_drawingFieldName != null && m_bIsPicture && m_bIsShapePicture)
                        {
                            m_drawingFieldValue = m_token;
                        }
                    }               
        }
        /// <summary>
        /// Parse field group
        /// </summary>
        /// <param name="token"></param>
        private void ParseFieldGroupData(string token)
        {
            FieldType fieldType = FieldType.FieldUnknown;

            if (m_fieldGroupTypeStack.Count > 0 && m_fieldGroupTypeStack.Peek() == FieldGroupType.FieldInstruction)
            {
                if (m_fieldCollectionStack.Count < m_fieldGroupStack.Count)
                    fieldType = GetFieldType(token.Trim());
            }
            if (m_fieldCollectionStack.Count > 0 && m_fieldCollectionStack.Peek().FieldType == FieldType.FieldShape)
                return;
            
            switch (fieldType)
            {
                case FieldType.FieldFormTextInput:
                    WTextFormField textFormField = new WTextFormField(m_document);
                    ApplyFieldProperties(textFormField as WField, token, fieldType);
                    break;
                case FieldType.FieldFormDropDown:
                    WDropDownFormField dropDownFormField = new WDropDownFormField(m_document);
                    ApplyFieldProperties(dropDownFormField as WField, token, fieldType);
                    break;
                case FieldType.FieldFormCheckBox:
                    WCheckBox checkBox = new WCheckBox(m_document);
                    ApplyFieldProperties(checkBox as WField, token, fieldType);
                    break;
                case FieldType.FieldMergeField:
                    WMergeField mergField = new WMergeField(m_document);
                    ApplyFieldProperties(mergField as WField, token, fieldType);
                    break;
                case FieldType.FieldIf:
                    WIfField ifField = new WIfField(m_document);
                    ApplyFieldProperties(ifField as WField, token, fieldType);
                    break;
                case FieldType.FieldUnknown:
                    ParseUnknownField(token, fieldType);
                    break;
                case FieldType .FieldTOC :
                    ParseTOCField(token, fieldType);
                    break;
                case FieldType .FieldShape :
                    WField shapeField = new WField(m_document);
                    shapeField.FieldType = FieldType.FieldShape;
                    m_fieldCollectionStack.Push(shapeField);
                    return;
                default:
                    WField field = new WField(m_document);
                    ApplyFieldProperties(field, token, fieldType);
                    break;
            }
        }
       /// <summary>
       /// Parse TOC field
       /// </summary>
       /// <param name="token"></param>
       /// <param name="fieldType"></param>
        private void ParseTOCField(string token, FieldType fieldType)
        {
            WField tocField = new WField(m_document);
            tocField.FieldType = fieldType;
            tocField.FieldCode = token;
            m_fieldCollectionStack.Push(tocField);
            TableOfContent toc = new TableOfContent(m_document, token);
            m_document.TOC = toc;
            CurrentPara.ChildEntities.Add(toc);
        }
       /// <summary>
       /// Parse unknown field
       /// </summary>
       /// <param name="token"></param>
       /// <param name="fieldType"></param>
        private void ParseUnknownField(string token, FieldType fieldType)
        {           
            if (m_previousToken != "datafield")
            {
                switch (m_fieldGroupTypeStack.Peek())
                {
                    case FieldGroupType.FieldInstruction:
                        if (m_fieldCollectionStack.Count < m_fieldGroupStack.Count)
                        {
                            if (token.Trim() == string.Empty)
                                return;
                            WField newfield = new WField(m_document);
                            ApplyFieldProperties(newfield, token, fieldType);
                        }
                        else if (m_fieldCollectionStack.Count > 0)
                        {
                            switch (m_fieldCollectionStack.Peek().FieldType)
                            {
                                case FieldType.FieldUnknown:
                                    m_fieldCollectionStack.Peek().FieldCode += token;
                                    m_fieldCollectionStack.Peek().FieldType = GetFieldType(m_fieldCollectionStack.Peek().FieldCode.Trim());
                                    CopyTextFormatToCharFormat(m_fieldCollectionStack.Peek().CharacterFormat, m_currTextFormat);
                                    if (m_fieldCollectionStack.Peek().FieldType == FieldType.FieldMergeField)
                                        ReplaceWfieldWithWMergeFieldObject();
                                    break;
                                case FieldType .FieldNext :
                                case FieldType.FieldMergeField:
                                    m_fieldCollectionStack.Peek().FieldCode += token;
                                    CopyTextFormatToCharFormat(m_fieldCollectionStack.Peek().CharacterFormat, m_currTextFormat);
                                    break;
                                default:
                                    AppendTextRange(token);
                                    break;
                            }
                        }
                        break;
                    case FieldGroupType.FieldResult:
                        if (m_fieldCollectionStack.Count > 0
                            && (m_fieldCollectionStack.Peek().FieldType == FieldType.FieldMergeField || m_fieldCollectionStack.Peek().FieldType == FieldType.FieldNext))
                        {
                            m_fieldCollectionStack.Peek().Text += token;
                            CopyTextFormatToCharFormat(m_fieldCollectionStack.Peek().CharacterFormat, m_currTextFormat);
                        }
                        else
                            AppendTextRange(token);
                        break;
                }
            }
        }
       
        /// <summary>
        /// Append Textrange
        /// </summary>
        /// <param name="token"></param>
        private void AppendTextRange(string token)
        {
            WTextRange tr = new WTextRange(m_document);
            tr.Text = token;
            CopyTextFormatToCharFormat(tr.CharacterFormat, m_currTextFormat);
            CurrentPara.ChildEntities.Add(tr);
        }
        /// <summary>
        /// Replace Wfield object with WMergeField object
        /// </summary>
        private void ReplaceWfieldWithWMergeFieldObject()
        {
            WMergeField mergeField = new WMergeField(m_document);
            mergeField.FieldCode = m_fieldCollectionStack.Peek().FieldCode;
            mergeField.FieldType = FieldType.FieldMergeField;
            CopyTextFormatToCharFormat(mergeField.CharacterFormat, m_currTextFormat);
            if (m_currParagraph.Items.LastItem.EntityType == EntityType.Field 
                && (m_currParagraph.Items.LastItem as WField).FieldType == FieldType.FieldMergeField)
            {
                m_currParagraph.Items.Remove(m_currParagraph.Items.LastItem);
                m_currParagraph.Items.Add(mergeField);
                m_fieldCollectionStack.Pop();
                m_fieldCollectionStack.Push(mergeField as WField);
            }
        }
        /// <summary>
        /// Apply field properties
        /// </summary>
        /// <param name="field"></param>
        /// <param name="token"></param>
        /// <param name="fieldType"></param>
        private void ApplyFieldProperties(WField field, string token, FieldType fieldType)
        {
            field.FieldType = fieldType;
            CopyTextFormatToCharFormat(field.CharacterFormat, m_currTextFormat);
            CurrentPara.ChildEntities.Add(field);
            field.FieldCode = token;
            m_fieldCollectionStack.Push(field);
        }
        /// <summary>
        /// Parse the form fields destination control words
        /// </summary>
        /// <param name="token"></param>
        private void ParseFormFieldDestinationWords(string token)
        {
            m_token = m_token.TrimStart();
            switch (m_previousToken)
            {
                case "ffname":
                    m_currentFormField.Name = token;
                    break;
                case "ffdeftext":
                    m_currentFormField.DefaultText = token;
                    break;
                case "ffformat":
                    m_currentFormField.StringFormat = token;
                    break;
                case "ffhelptext":
                    m_currentFormField.HelpText = token;
                    break;
                case "ffstattext":
                    m_currentFormField.StatusHelpText = token;
                    break;
                case "ffentrymcr":
                    m_currentFormField.MarcoOnStart = token;
                    break;
                case "ffexitmcr":
                    m_currentFormField.MacroOnExit = token;
                    break;
                case "ffl":
                    m_currentFormField.DropDownItems.Add(token);
                    break;
            }
        }
       /// <summary>
       /// Parse Image bytes
       /// </summary>
        private void ParseImageBytes()
        {
            if (IsFieldGroup && //Ensure within field group
                (m_fieldCollectionStack.Count > 0 && m_fieldCollectionStack.Peek().FieldType == FieldType.FieldShape)) // Ensure whether the current field type is shape field
                return;
            if (m_previousToken.StartsWith("blipuid"))
            {
                if (m_previousControlString == c_groupEnd)
                    AppendPictureToParagraph(m_token);
            }
            else
                AppendPictureToParagraph(m_token);
        }
       /// <summary>
       /// Parse custom document properties
       /// </summary>
        private void ParseCustomDocumentProperties()
        {
            switch (m_previousToken)
            {
                case "propname":
                    m_currPropertyName = m_token;
                    break;
                case "staticval":
                    switch (m_currPropertyType)
                    {
                        case Syncfusion.CompoundFile.DocIO.PropertyType.Int:
                        case Syncfusion.CompoundFile.DocIO.PropertyType.Int16:
                        case Syncfusion.CompoundFile.DocIO.PropertyType.Int32:
                            m_currPropertyValue = Convert.ToInt32(m_token);
                            break;
                        case Syncfusion.CompoundFile.DocIO.PropertyType.Double:
                            m_currPropertyValue = Convert.ToDouble(m_token);
                            break;
                        case Syncfusion.CompoundFile.DocIO.PropertyType.Bool:
                            m_currPropertyValue = Convert.ToBoolean(Convert.ToInt32(m_token));
                            break;
                        case Syncfusion.CompoundFile.DocIO.PropertyType.DateTime:
                            m_currPropertyValue = Convert.ToDateTime(m_token);
                            break;
                        default:
                            m_currPropertyValue = m_token;
                            break;
                    }
                    if (m_currPropertyName != null && m_currPropertyValue != null)
                    {
                        m_document.CustomDocumentProperties.Add(m_currPropertyName, m_currPropertyValue);
                        m_document.CustomDocumentProperties[m_currPropertyName].PropertyType = m_currPropertyType;
                    }
                    m_currPropertyName = null;
                    m_currPropertyValue = null;
                    break;

            }

        }
       /// <summary>
       /// Parse built-in document properties
       /// </summary>
        private void ParseBuiltInDocumentProperties()
        {
            switch (m_previousToken)
            {
                case "title":
                    m_document.BuiltinDocumentProperties.Title = m_token;
                    break;
                case "category":
                    m_document.BuiltinDocumentProperties.Category = m_token;
                    break;
                case "doccomm":
                    m_document.BuiltinDocumentProperties.Comments = m_token;
                    break;
                case "operator":
                    m_document.BuiltinDocumentProperties.Author = m_token;
                    break;
                case "manager":
                    m_document.BuiltinDocumentProperties.Manager = m_token;
                    break;
                case "company":
                    m_document.BuiltinDocumentProperties.Company = m_token;
                    break;
                case "keywords":
                    m_document.BuiltinDocumentProperties.Keywords = m_token;
                    break;
                case "subject":
                    m_document.BuiltinDocumentProperties.Subject = m_token;
                    break;
            }
        }
       /// <summary>
       /// Gets the field type
       /// </summary>
       /// <param name="fieldInstruction"></param>
       /// <returns></returns>
        private FieldType GetFieldType(string token)
        {
            FieldType fieldType = FieldType.FieldUnknown;
            string fieldTypeString;
            if (token.Contains(" "))
            {
                int startIndex = token.IndexOf(" ");
                fieldTypeString = token.Substring(0, startIndex);
            }
            else
                fieldTypeString = token;

            fieldType = FieldTypeDefiner.GetFieldType(fieldTypeString.Trim());

            return fieldType;           
        }
        /// <summary>
        /// Get the formatting string 
        /// </summary>
        /// <param name="fieldInstruction"></param>
        /// <returns></returns>
        private string GetFormattingString(string fieldInstruction, string fieldTypeString)
        {
            string result = string.Empty;
            result = fieldInstruction.Replace(fieldTypeString, string.Empty);
            return result.Trim();
        }
        /// <summary>
        /// Apply the drop down field specific properties
        /// </summary>
        /// <param name="dropDownFormField"></param>
        private void ApplyDropDownFormFieldProperties(WDropDownFormField dropDownFormField)
        {
            dropDownFormField.DefaultDropDownValue = m_currentFormField.Ffdefres;

            for (int i = 0; i < m_currentFormField.DropDownItems.Count; i++)
            {
                if (m_currentFormField.DropDownItems[i].Text != null && m_currentFormField.DropDownItems[i].Text != string.Empty)
                    dropDownFormField.DropDownItems.Add(m_currentFormField.DropDownItems[i].Text);
            }
        }
        /// <summary>
        /// Apply the textform field specific properties
        /// </summary>
        /// <param name="textField"></param>
        private void ApplyTextFormFieldProperties(WTextFormField textField)
        {
            if (m_currentFormField.DefaultText == null && m_currentFormField.MaxLength < WTextFormField.DEF_TEXT.Length && m_currentFormField.MaxLength != 0)
                textField.DefaultText = string.Empty;
            else
                textField.DefaultText = m_currentFormField.DefaultText != null ? m_currentFormField.DefaultText : WTextFormField.DEF_TEXT;
            if (m_currentFormField.MaxLength > 0)
                textField.MaximumLength = m_currentFormField.MaxLength;
            textField.StringFormat = m_currentFormField.StringFormat != null ? m_currentFormField.StringFormat : string.Empty;
        }
        /// <summary>
        /// Apply the checkbox specific properties
        /// </summary>
        /// <param name="checkbox"></param>
        private void ApplyCheckboxPorperties(WCheckBox checkbox)
        {
            checkbox.SizeType = m_currentFormField.CheckboxSizeType;
            checkbox.CheckBoxSize = m_currentFormField.CheckboxSize;
            checkbox.DefaultCheckBoxValue = (m_currentFormField.Ffdefres == 1) ? true : false;
            checkbox.Checked = (m_currentFormField.Ffres == 1) ? true : (m_currentFormField.Ffres == 25) ? checkbox.DefaultCheckBoxValue : false;
        }
        /// <summary>
        /// Apply the common formfield properties
        /// </summary>
        /// <param name="formField"></param>
        private void ApplyFormFieldProperties(WFormField formField)
        {
            if (m_currentFormField.Name != null && m_document.Bookmarks[m_currentFormField.Name] == null)
                formField.Name = m_currentFormField.Name;
            formField.Help = m_currentFormField.HelpText;
            formField.StatusBarHelp = m_currentFormField.StatusHelpText;
            formField.MacroOnStart = m_currentFormField.MarcoOnStart;
            formField.MacroOnEnd = m_currentFormField.MacroOnExit;
            formField.Enabled = m_currentFormField.Enabled;
            formField.CalculateOnExit = m_currentFormField.CalculateOnExit;
        }
       /// <summary>
       /// Check whether the current picture/shape need to be skipped or added
       /// </summary>
       /// <returns></returns>
       private bool IsSupportedPicture()
       {
           if (m_bIsObject)
               return false;
           else if (m_bIsShape == true && m_bIsShapePictureAdded)
               return false;
           else
               return true;
       }
       /// <summary>
       /// Append Picture to the current paragraph
       /// </summary>
        private void AppendPictureToParagraph(string token)
        {
             byte[] buffer = GetImageByteArray(token);
             if (IsSupportedPicture())
             {
                 m_currPicture = CurrentPara.AppendPicture(buffer);
                 ApplyPictureFormatting(m_currPicture, m_picFormat);
                 m_bIsShapePictureAdded = true;
             }
            m_bIsMetaFile = false;
            m_bIsStandardPictureSizeNeedToBePreserved = false;
        }
       /// <summary>
       /// Get Image bytes from the token
       /// </summary>
       /// <param name="token"></param>
       /// <returns></returns>
        private byte[] GetImageByteArray(string token)
        {
            //Read Binary Data of the Image.
            if (token.StartsWith("bin") || m_previousToken.StartsWith("bin"))
            {
                string[] value = SeperateToken(m_token);
                if (m_previousToken.StartsWith("bin"))
                    value = SeperateToken(m_previousToken);
                int bufferSize = Convert.ToInt32(value[1]);
                byte[] imageBytes = new byte[bufferSize];
                Array.Copy(m_rtfReader.RtfData, m_rtfReader.Position - 1, imageBytes, 0 , bufferSize);
                m_rtfReader.Position += bufferSize -1;
                return imageBytes;
            }
            else
            {
                token = token.Replace("\r", "");
                token = token.Replace("\n", "");
                token = token.Replace(" ", "");
                //Convert image string to Upper case to handle generic conversion
                token = token.ToUpper();
                // Gets the ASCII bytes for image string
                byte[] asciiBuffer = m_rtfReader.Encoding.GetBytes(token);
                byte[] hexBuffer = new byte[asciiBuffer.Length / 2];
                // Converts two ASCII bytes to one hexadecimal byte
                for (int i = 0, j = 0; j < asciiBuffer.Length / 2; i += 2)
                {
                    // If byte is A-F ,  subtract ASCII for 'A'(65) and add 10 (-65+10 = -55) , else subtract ASCII for 0(48)
                    int first = asciiBuffer[i] > 57 ? asciiBuffer[i] - 55 : asciiBuffer[i] - 48;
                    int second = asciiBuffer[i + 1] > 57 ? asciiBuffer[i + 1] - 55 : asciiBuffer[i + 1] - 48;
                    // Shift fisrt byte to 4 digits left and then perform OR with second byte to get equivalent hexa byte
                    hexBuffer[j++] = (byte)((first << 4) | second);
                }
                m_lexer.IsImageBytes = false;
                return hexBuffer;
            }
        }
       /// <summary>
       /// Apply picture Formatting
       /// </summary>
        private void ApplyPictureFormatting(IWPicture currPicture,PictureFormat pictureFormat)
        {
            //Picture size is calculated using \picw and \pich control word if the size is not explicitly specified through \picwgoal and pichgoal control word           
            if (pictureFormat.HeightScale <= 0)
                pictureFormat.HeightScale = 100;
            if (pictureFormat.WidthScale <= 0)
                pictureFormat.WidthScale = 100;

            if (m_bIsStandardPictureSizeNeedToBePreserved)
            {
                if (m_bIsShape && m_currShapeFormat.Size != new SizeF())
                {
                    SizeF size = m_currShapeFormat.Size;
                    pictureFormat.Height = size.Height;//Preserves the height of shape
                    pictureFormat.Width = size.Width; //Preserves the width of shape
                }
                else
                {
                    if (pictureFormat.Height <= 0 || pictureFormat.Height > 1584)
                        pictureFormat.Height = 216;//Preserves the standard height as in MS Word if \wmetafile8 control word encountered 
                    if (pictureFormat.Width <= 0 || pictureFormat.Width > 1584)
                        pictureFormat.Width = 216; //Preserves the standard width as in MS Word behavior if \wmetafile8 control word encountered
                }
            }
            
           
           if (!m_bIsShape || m_currShapeFormat.Size == new SizeF())
           {
               if (pictureFormat.Height > 0 && pictureFormat.Height <= 1584)
                   currPicture.Height = pictureFormat.Height;
               if (pictureFormat.Width > 0 && pictureFormat.Width <= 1584)
                   currPicture.Width = pictureFormat.Width;
               if (pictureFormat.HeightScale > 0)
                   currPicture.HeightScale = pictureFormat.HeightScale;
               if (pictureFormat.WidthScale > 0)
                   currPicture.WidthScale = pictureFormat.WidthScale;
           }
           else
           {
               SizeF size = m_currShapeFormat.Size;
               if (pictureFormat.Height > 0 && pictureFormat.Height <= 1584)
                   currPicture.Height = size.Height;
               if (pictureFormat.Width > 0 && pictureFormat.Width <= 1584)
                   currPicture.Width = size.Width;
           }
           if (m_bIsShapePicture)
            {                
                if (m_currShapeFormat.m_horizOrgin != null)
                    currPicture.HorizontalOrigin = m_currShapeFormat.m_horizOrgin;
                if (m_currShapeFormat.m_vertOrgin != null)
                    currPicture.VerticalOrigin = m_currShapeFormat.m_vertOrgin;
                if (m_currShapeFormat.m_horizAlignment != null)
                    currPicture.HorizontalAlignment = m_currShapeFormat.m_horizAlignment;
                if (m_currShapeFormat.m_textWrappingStyle != null)
                    currPicture.TextWrappingStyle = m_currShapeFormat.m_textWrappingStyle;
                if (m_currShapeFormat.m_textWrappingType != null)
                    currPicture.TextWrappingType = m_currShapeFormat.m_textWrappingType;
                if (m_currShapeFormat.m_vertPosition != null)
                    currPicture.VerticalPosition = m_currShapeFormat.m_vertPosition;
                if (m_currShapeFormat.m_horizPosition != null)
                    currPicture.HorizontalPosition = m_currShapeFormat.m_horizPosition;
                currPicture.IsBelowText = m_currShapeFormat.m_isBelowText;
            }
           

        }

       /// <summary>
       /// Copy paragraph formatting
       /// </summary>
       /// <param name="sourceParaFormat"></param>
       /// <param name="destParaFormat"></param>
        private void CopyParagraphFormatting(WParagraphFormat sourceParaFormat, WParagraphFormat destParaFormat)
        {
            //apply the properties to the newly created paragraph
            destParaFormat.ImportContainer(sourceParaFormat as FormatBase);

            if(sourceParaFormat.HasValue (WParagraphFormat.BackColorKey))
               destParaFormat.BackColor = sourceParaFormat.BackColor;
            if(sourceParaFormat.HasValue (WParagraphFormat.BeforeSpacingKey ))
               destParaFormat.BeforeSpacing = sourceParaFormat.BeforeSpacing;
            if (sourceParaFormat.HasValue(WParagraphFormat.AfterSpacingKey))
                destParaFormat.AfterSpacing = sourceParaFormat.AfterSpacing;
            if (sourceParaFormat .HasValue (WParagraphFormat .BidiKey ) &&  sourceParaFormat.Bidi == true)
                destParaFormat.Bidi = true;
            if (sourceParaFormat .HasValue (WParagraphFormat.ColumnBreakAfterKey)&& sourceParaFormat.ColumnBreakAfter == true)
                destParaFormat.ColumnBreakAfter = true;
            if (sourceParaFormat .HasValue (WParagraphFormat.ContextualSpacingKey )&&  sourceParaFormat.ContextualSpacing == true)
                destParaFormat.ContextualSpacing = true;
            if(sourceParaFormat .HasValue (WParagraphFormat.FirstLineIndentKey ))
            destParaFormat.FirstLineIndent = sourceParaFormat.FirstLineIndent;
            if(sourceParaFormat .HasValue (WParagraphFormat.FirstLineIndentBiKey ))
            destParaFormat.FirstLineIndentBi = sourceParaFormat.FirstLineIndentBi;
            if(sourceParaFormat .HasValue (WParagraphFormat.ForeColorKey ))
            destParaFormat.ForeColor = sourceParaFormat.ForeColor;
            if(sourceParaFormat.HasValue (WParagraphFormat.HrAlignmentKey ))
            destParaFormat.HorizontalAlignment = sourceParaFormat.HorizontalAlignment;
            if(sourceParaFormat .HasValue (WParagraphFormat.KeepKey )&& sourceParaFormat .Keep ==true)
                destParaFormat.Keep = true;
            if(sourceParaFormat .HasValue (WParagraphFormat.KeepFollowKey  )&& sourceParaFormat .KeepFollow ==true )
                destParaFormat.KeepFollow = true;
            if(sourceParaFormat .HasValue (WParagraphFormat.LeftIndentKey ))
            destParaFormat.LeftIndent = sourceParaFormat.LeftIndent;                    
            destParaFormat.FirstLineIndent = sourceParaFormat.FirstLineIndent;      
            if (m_bIsLinespacingRule)
            {
                destParaFormat.LineSpacing = sourceParaFormat.LineSpacing;
                destParaFormat.LineSpacingRule = sourceParaFormat.LineSpacingRule;
            }
            if (sourceParaFormat .HasValue (WParagraphFormat.OutlineLevelKey  )&&  sourceParaFormat.OutlineLevel != OutlineLevel.BodyText)
                destParaFormat.OutlineLevel = sourceParaFormat.OutlineLevel;
            if (sourceParaFormat .HasValue (WParagraphFormat.PageBreakAfterKey )&& sourceParaFormat.PageBreakAfter == true)
                destParaFormat.PageBreakAfter = true;
            if (sourceParaFormat .HasValue (WParagraphFormat.PageBreakBeforeKey )&& sourceParaFormat.PageBreakBefore == true)
                destParaFormat.PageBreakBefore = true;
            if(sourceParaFormat .HasValue (WParagraphFormat.RightIndentKey))
            destParaFormat.RightIndent = sourceParaFormat.RightIndent;
            if (destParaFormat.Bidi == true)
            {
                destParaFormat.LeftIndentBi = sourceParaFormat.LeftIndentBi;
                destParaFormat.RightIndentBi = sourceParaFormat.RightIndentBi;
            }
            if (sourceParaFormat .HasValue (WParagraphFormat.SpacingAfterAutoKey )&& sourceParaFormat.SpaceAfterAuto  == true)
                destParaFormat.SpaceAfterAuto  = true;
            if (sourceParaFormat.HasValue (WParagraphFormat.SpacingBeforeAutoKey )&&  sourceParaFormat.SpaceBeforeAuto   == true)
                destParaFormat.SpaceBeforeAuto   = true;
            if(sourceParaFormat .HasValue (WParagraphFormat.TextureStyleKey ))
            destParaFormat.TextureStyle = sourceParaFormat.TextureStyle;
            if (sourceParaFormat .HasValue (WParagraphFormat.WidowControlKey )&& sourceParaFormat.WidowControl == true)
                destParaFormat.WidowControl = true;
            if (sourceParaFormat.HasValue(WParagraphFormat.WrapFrameAroundKey))
                destParaFormat.WrapFrameAround = sourceParaFormat.WrapFrameAround;

            if (sourceParaFormat.HasValue(WParagraphFormat.TextureStyleKey))
                destParaFormat.TextureStyle = sourceParaFormat.TextureStyle;
            if (sourceParaFormat.HasValue(WParagraphFormat.WordWrapKey))
                destParaFormat.WordWrap = sourceParaFormat.WordWrap;
        }
        /// <summary>
        /// Parse control word from the rtf file
        /// </summary>
        /// <param name="token"></param>
        private void ParseControlWords(string token,string tokenKey,string tokenValue)
        {
            if (m_currentTableType != RtfTableType.None)
            {
                switch (m_currentTableType)
                {
                    case RtfTableType.FontTable:
                        ParseFontTable(m_token, tokenKey , tokenValue );
                        break;
                    case RtfTableType.ColorTable:
                        ParseColorTable(m_token,tokenKey ,tokenValue );
                        break;
                    case RtfTableType .StyleSheet :
                        ParseFormattingToken(token, tokenKey, tokenValue);
                        break;
                    case RtfTableType.ListOverrideTable:
                    case RtfTableType .ListTable :
                        ParseListTable(m_token ,tokenKey ,tokenValue );
                        break;
                }
            }
         
            else
            {
                switch (tokenKey)
                {  
                    case "rtf":
                        m_rtfCollectionStack.Push(c_controlStart);
                        break;
                    case "fonttbl":
                        m_rtfFont = new RtfFont();
                        m_currentTableType = RtfTableType.FontTable;
                        m_lexer.CurrRtfTableType = RtfTableType.FontTable;
                        m_stack.Push(c_groupStart );
                        break;
                    case "stylesheet":
                        m_currentTableType = RtfTableType.StyleSheet;
                        m_lexer.CurrRtfTableType = RtfTableType.StyleSheet ;
                        m_stack.Push(c_groupStart);
                        break;
                    case "listtable":
                        m_currentTableType = RtfTableType.ListTable;
                        m_lexer.CurrRtfTableType = RtfTableType.ListTable ;
                        m_stack.Push(c_groupStart);
                        break;
                    case "listoverridetable":
                        m_currentTableType = RtfTableType.ListOverrideTable;
                        m_lexer.CurrRtfTableType = RtfTableType.ListOverrideTable ;
                        m_stack.Push(c_groupStart);
                        break;
                    case "colortbl":
                        m_currentTableType = RtfTableType.ColorTable;
                        m_lexer.CurrRtfTableType = RtfTableType.ColorTable;
                        m_rtfColorTable = new RtfColor();
                        m_stack.Push(c_groupStart);
                        break;
                    case "info":
                        m_bIsDocumentInfo = true;
                        m_stack.Push(c_groupStart);
                        break;
                    case "userprops":
                        m_bIsCustomProperties = true;
                        m_stack.Push(c_groupStart);
                        break;
                    case "ansicpg":
                        if (IsSupportedCodePage(Convert.ToInt32(tokenValue)))
                            DefaultCodePage = GetSupportedCodePage(Convert.ToInt32(tokenValue));
                        break;
                    case "deff":
                    case "adeff":
                        DefaultFontIndex = Convert.ToInt32(tokenValue);
                        break;
                    case "htmautsp":
                        m_document.DOP.Dop2000.Copts.DontUseHTMLParagraphAutoSpacing = false;
                        break;
                    default:
                        ParseFormattingToken(token, tokenKey, tokenValue);                      
                        break;
                }
            }
        }
       /// <summary>
       /// Parse list table
       /// </summary>
       /// <param name="token"></param>
       /// <param name="tokenKey"></param>
       /// <param name="tokenValue"></param>
        private void ParseListTable(string token, string tokenKey, string tokenValue)
        {
            int value;
            float textPosition;
            string styleName = null;
            if (m_bIsLevelText && CurrListLevel.PatternType == ListPatternType.Arabic && m_previousToken != "leveltext")
            {
                if (token.StartsWith("'01"))
                    CurrListLevel.NumberPrefix = ((char)(0X0000)).ToString() + ".";
                else if (token.StartsWith("'02"))
                    CurrListLevel.NumberPrefix = ((char)(0X0000)).ToString() + "." + ((char)(0X0001)).ToString() + ".";
                else if (token.StartsWith("'03"))
                    CurrListLevel.NumberPrefix = ((char)(0X0000)).ToString() + "." + ((char)(0X0001)).ToString() + "." + ((char)(0X0002)).ToString() + ".";
                else if (token.StartsWith("'04"))
                    CurrListLevel.NumberPrefix = ((char)(0X0000)).ToString() + "." + ((char)(0X0001)).ToString() + "." + ((char)(0X0002)).ToString() + "." + ((char)(0X0003)).ToString() + ".";
                else if (token.StartsWith("'05"))
                    CurrListLevel.NumberPrefix = ((char)(0X0000)).ToString() + "." + ((char)(0X0001)).ToString() + "." + ((char)(0X0002)).ToString() + "." + ((char)(0X0003)).ToString() + "." + ((char)(0X0004)).ToString() + ".";
                else if (token.StartsWith("'06"))
                    CurrListLevel.NumberPrefix = ((char)(0X0000)).ToString() + "." + ((char)(0X0001)).ToString() + "." + ((char)(0X0002)).ToString() + "." + ((char)(0X0003)).ToString() + "." + ((char)(0X0004)).ToString() + "." + ((char)(0X0005)).ToString() + ".";
                else if (token.StartsWith("'07"))
                    CurrListLevel.NumberPrefix = ((char)(0X0000)).ToString() + "." + ((char)(0X0001)).ToString() + "." + ((char)(0X0002)).ToString() + "." + ((char)(0X0003)).ToString() + "." + ((char)(0X0004)).ToString() + "." + ((char)(0X0005)).ToString() + "." + ((char)(0X0006)).ToString() + ".";
                else if (token.StartsWith("'00"))
                    CurrListLevel.NumberPrefix = null;
                if (token.EndsWith("."))
                    CurrListLevel.NumberSufix = ".";
                else
                    CurrListLevel.NumberSufix = string.Empty;
            }
            else if (m_bIsLevelText && CurrListLevel != null && CurrListLevel.PatternType == ListPatternType.Bullet)
            {
                string bulletChar = String.Empty;
                string hexValue = token.Replace("'", string.Empty);
                if (token.Length >= 3)
                {
                    string lvlNumber = token.Substring(0, 3);
                    switch (lvlNumber)
                    {
                        case "'00":
                        case "'01":
                        case "'02":
                        case "'03":
                        case "'04":
                        case "'05":
                        case "'06":
                        case "'07":
                        case "'08":
                            bulletChar = token.Replace(lvlNumber, string.Empty);
                            bulletChar = hexValue = bulletChar.Replace("'", string.Empty);
                            break;
                    }
                }
                int charCode;
                //Parses the bullet character of the current list level from hex string.
                if (int.TryParse(hexValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out charCode))
                {
                    if (charCode == 149) // Char code 149 is represent ".", It's not a UI visible character. As per MSWord behavior, we have used char code 8226 (.) instead of 149.
                    {
                        bulletChar = ((char)8226).ToString();
                    }
                    else
                        bulletChar = ((char)charCode).ToString();
                }
                if (bulletChar != string.Empty)
                    CurrListLevel.BulletCharacter = bulletChar;
            }
          
            switch (tokenKey)
            {
                case "list":
                    CurrListStyle = new ListStyle(m_document);
                    Guid id = Guid.NewGuid();
                    string uniqueID = id.ToString();
                    styleName = "ListStyle" + uniqueID;
                    m_currStyleName = styleName;
                    CurrListStyle.Name = styleName;
                   // CurrListStyle = m_document.AddListStyle(ListType.Bulleted, styleName);
                    m_currLevelIndex = -1;
                    break;
                case "ls":
                    if(m_currentTableType ==RtfTableType .ListOverrideTable)
                       m_listOverrideTable.Add(token,CurrListStyle .Name );
                    break;
                case "listlevel":
                    ParselistLevelStart();
                    break;
                case "levelfollow":
                    {
                        switch (Convert.ToInt32(tokenValue))
                        {
                            case 0:
                                CurrListLevel.FollowCharacter = FollowCharacterType.Tab;
                                break;
                            case 1:
                                CurrListLevel.FollowCharacter = FollowCharacterType.Space;
                                break;
                            case 3:
                                CurrListLevel.FollowCharacter = FollowCharacterType.Nothing;
                                break;
                        }
                    }
                    break;
                case "levelstartat":
                    CurrListLevel.StartAt = Convert.ToInt32(tokenValue);
                    break;
                case "levelnfcn":
                case "levelnfc":
                    value =Convert .ToInt32 (tokenValue );
                    if (value == 23)
                    {
                        CurrListStyle.ListType = ListType.Bulleted;
                        CurrListLevel.PatternType = ListPatternType.Bullet;
   
                    }
                    else
                    {
                        CurrListStyle.ListType = ListType.Numbered;
                        switch (value)
                        {
                            case 0:                              
                                CurrListLevel.PatternType  = ListPatternType.Arabic;
                                break;
                            case 1:
                                CurrListLevel.PatternType = ListPatternType.UpRoman;
                                break;
                            case 2:                            
                                CurrListLevel.PatternType = ListPatternType.LowRoman;
                                break;
                            case 3:                               
                                CurrListLevel.PatternType = ListPatternType.UpLetter;
                                break;
                            case 4:                              
                                CurrListLevel.PatternType = ListPatternType.LowLetter;
                                break;                       
                            default:
                                CurrListLevel.PatternType = ListPatternType.Arabic;
                                break;
                        }
                    }
                    break;
                case "leveljc":
                    switch (Convert.ToInt32(tokenValue))
                    {
                        case 0:
                            CurrListLevel.NumberAlignment = ListNumberAlignment.Left;
                            break;
                        case 1:
                            CurrListLevel.NumberAlignment = ListNumberAlignment.Center;
                            break;
                        case 2:
                            CurrListLevel.NumberAlignment = ListNumberAlignment.Right;
                            break;
                    }
                    break;
                case "levelnorestart":
                    value = Convert.ToInt32(tokenValue);
                    if (value == 1)
                        CurrListLevel.NoRestartByHigher = true;
                    else if (value == 0)
                        CurrListLevel.NoRestartByHigher = false;
                    break;
                case "lin-":
                case "li-":
                    textPosition = ExtractTwipsValue(tokenValue);
                    CurrListLevel.ParagraphFormat.LeftIndent = -textPosition;
                    CurrListLevel.TextPosition = -textPosition;
                    break;
                case "lin":
                case "li":
                    textPosition = ExtractTwipsValue(tokenValue);
                    CurrListLevel.ParagraphFormat.LeftIndent = textPosition;
                    CurrListLevel.TextPosition = textPosition;
                    break;
                case "fi":
                    float firstLineIndent = ExtractTwipsValue(tokenValue);
                    CurrListLevel.ParagraphFormat.FirstLineIndent = firstLineIndent;
                    break;
                case "fi-":
                    float indent = ExtractTwipsValue(tokenValue);
                    CurrListLevel.ParagraphFormat.FirstLineIndent = -indent;
                    break;
                case "listid-":
                case "listid":
                    if (m_currentTableType==RtfTableType .ListTable )
                        m_listTable .Add (token,CurrListStyle);
                    else if (m_currentTableType == RtfTableType.ListOverrideTable)
                    {
                        m_currLevelIndex = -1;
                        foreach (KeyValuePair<string, ListStyle> listTable in m_listTable)
                        {
                            if (listTable.Key == token)
                            {
                                styleName = "LfoStyle_" + Guid.NewGuid().ToString();
                                CurrListStyle = m_document.AddListStyle(listTable.Value.ListType, styleName);
                                CopyListStyle(listTable.Value, CurrListStyle);
                                CurrListStyle.Name = styleName;
                            }
                        }
                    }
                    break;
                case "levelold":
                    m_isLevelOld = true;
                    break;
                case "levelspace":
                    float tabSpaceAfter = ExtractTwipsValue(tokenValue);
                    if (m_isLevelOld)
                        CurrListLevel.TabSpaceAfter = tabSpaceAfter;
                    break;
                case "tx":
                    CurrListLevel.TabSpaceAfter = ExtractTwipsValue(tokenValue);
                    break;
                case "f":
                case "af":
                    foreach (KeyValuePair<string, RtfFont> fontTable in m_fontTable)
                    {
                        string fontKey = fontTable.Key;
                        string[] tokenSplit = SeperateToken(fontKey);
                        if (tokenSplit[1] == tokenValue)
                        {
                            CurrRtfFont = fontTable.Value;
                            CurrListLevel.CharacterFormat.FontName = CurrRtfFont.FontName;
                        }
                    }                   
                    break;
                case "fs":
                    if (tokenValue != null)
                        CurrListLevel.CharacterFormat.FontSize = float.Parse(tokenValue, CultureInfo.InvariantCulture) / 2;
                    break;
                case "b":
                    if (tokenValue != null && Convert.ToInt32(tokenValue) == 0)
                        CurrListLevel.CharacterFormat.Bold = false;
                    else
                        CurrListLevel.CharacterFormat.Bold = true;
                    break;
                case "u-":
                     value = 65536 - Convert .ToInt32 (tokenValue );
                    char unicodeChar = (char)value;
                    CurrListLevel.BulletCharacter = unicodeChar.ToString();
                    break;
                case "leveltext":
                    m_bIsLevelText = true;
                    break;
                default :
                    ParseFormattingToken(token, tokenKey, tokenValue);
                    break;
            }
           
        }
       /// <summary>
       /// Parse list level start
       /// </summary>
        private void ParselistLevelStart()
        {
            m_listLevelStack.Push("{");
            m_bIsListLevel = true;
            m_currLevelIndex++;
            if (m_currentTableType == RtfTableType.ListTable)
            {
                CurrListLevel = new WListLevel(CurrListStyle);
                m_currLevelIndex = CurrListStyle.Levels.Add(CurrListLevel);
            }
            else if (m_currentTableType == RtfTableType.ListOverrideTable)
            {
                CurrListLevel = CurrListStyle.Levels[m_currLevelIndex];
            }
            m_bIsLevelText = false;
            m_isLevelOld = false;
            CurrentPara = new WParagraph(m_document);
            m_currTextFormat = new TextFormat();

        }
       /// <summary>
       /// Copy Character formatting
       /// </summary>
       /// <param name="destFormat"></param>
       /// <param name="sourceFormat"></param>
        private void CopyCharacterFormatting(WCharacterFormat sourceFormat, WCharacterFormat destFormat)
        {
            if (sourceFormat.HasValue(WCharacterFormat.FontSizeKey))
                destFormat.FontSize = sourceFormat.FontSize;
            if (sourceFormat.HasValue(WCharacterFormat.TextColorKey))
                destFormat.TextColor = sourceFormat.TextColor;
            if (sourceFormat.HasValue(WCharacterFormat.FontNameKey) && sourceFormat.FontName != "Times New Roman")
            {
                destFormat.FontName = sourceFormat.FontName;
                if (sourceFormat.FontName == "Monotype Corsiva" || sourceFormat.FontName == "Brush Script MT")
                    destFormat.Italic = true;
            }
            if (sourceFormat.HasValue(WCharacterFormat.FontNameAsciiKey) && sourceFormat.FontNameAscii != "Times New Roman")
                destFormat.FontNameAscii = sourceFormat.FontNameAscii;
            if (sourceFormat.HasValue(WCharacterFormat.FontNameBidiKey) && sourceFormat.FontNameBidi != "Times New Roman")
                destFormat.FontNameBidi = sourceFormat.FontNameBidi;
            if (sourceFormat.HasValue(WCharacterFormat.FontNameFarEastKey) && sourceFormat.FontNameFarEast != "Times New Roman")
                destFormat.FontNameFarEast = sourceFormat.FontNameFarEast;
            if (sourceFormat.HasValue(WCharacterFormat.FontNameNonFarEastKey) && sourceFormat.FontNameNonFarEast != "Times New Roman")
                destFormat.FontNameNonFarEast = sourceFormat.FontNameNonFarEast;
            if (sourceFormat.HasValue(WCharacterFormat.BoldKey))
                destFormat.Bold = sourceFormat.Bold;
            if (sourceFormat.HasValue(WCharacterFormat.ItalicKey))
                destFormat.Italic = sourceFormat.Italic;
            if (sourceFormat.HasValue(WCharacterFormat.UnderlineKey) && sourceFormat.UnderlineStyle != UnderlineStyle.None)
                destFormat.UnderlineStyle = sourceFormat.UnderlineStyle;
            if (sourceFormat.HasValue(WCharacterFormat.HighlightColorKey))
                destFormat.HighlightColor = sourceFormat.HighlightColor;
            if (sourceFormat.HasValue(WCharacterFormat.ShadowKey))
                destFormat.Shadow = sourceFormat.Shadow;
            if (sourceFormat.HasValue(WCharacterFormat.SpacingKey))
                destFormat.CharacterSpacing = sourceFormat.CharacterSpacing;
            if (sourceFormat.HasValue(WCharacterFormat.DoubleStrikeKey))
                destFormat.DoubleStrike = sourceFormat.DoubleStrike;
            if (sourceFormat.HasValue(WCharacterFormat.EmbossKey))
                destFormat.Emboss = sourceFormat.Emboss;
            if (sourceFormat.HasValue(WCharacterFormat.EngraveKey))
                destFormat.Engrave = sourceFormat.Engrave;
            if (sourceFormat.HasValue(WCharacterFormat.SubSuperScriptKey))
                destFormat.SubSuperScript = sourceFormat.SubSuperScript;
            if (sourceFormat.HasValue(WCharacterFormat.TextBkgColorKey))
                destFormat.TextBackgroundColor = sourceFormat.TextBackgroundColor;
            if (sourceFormat.HasValue(WCharacterFormat.AllCapsKey))
                destFormat.AllCaps = sourceFormat.AllCaps;
            if (sourceFormat.Bidi == true)
            {
                destFormat.Bidi = true;
                destFormat.FontNameBidi = sourceFormat.FontNameBidi;
                destFormat.FontSizeBidi = sourceFormat.FontSizeBidi;
            }
            if (sourceFormat.HasValue(WCharacterFormat.BoldBidiKey))
                destFormat.BoldBidi = sourceFormat.BoldBidi;
            if (sourceFormat.HasValue(WCharacterFormat.FieldVanishKey))
                destFormat.FieldVanish = sourceFormat.FieldVanish;
            if (sourceFormat.HasValue(WCharacterFormat.HiddenKey))
                destFormat.Hidden = sourceFormat.Hidden;
            if (sourceFormat.HasValue(WCharacterFormat.SmallCapsKey))
                destFormat.SmallCaps = sourceFormat.SmallCaps;
            
        }
       /// <summary>
       /// Copy base list style to overrided list style
       /// </summary>
       /// <param name="sourceStyle"></param>
       /// <param name="destStyle"></param>
        private void CopyListStyle(ListStyle sourceListStyle,ListStyle destListStyle)
        {    
            destListStyle.ListType = sourceListStyle.ListType;
            destListStyle.Name = sourceListStyle.Name;
            for (int i = 0; i < sourceListStyle.Levels.Count; i++)
            {
                destListStyle.Levels[i].ParagraphFormat.LeftIndent = sourceListStyle.Levels[i].ParagraphFormat.LeftIndent;
                destListStyle.Levels[i].ParagraphFormat.FirstLineIndent = sourceListStyle.Levels[i].ParagraphFormat.FirstLineIndent;
                WCharacterFormat destCharFormat = destListStyle.Levels[i].CharacterFormat;
                WCharacterFormat sourceCharFormat = sourceListStyle.Levels[i].CharacterFormat;
                if (sourceCharFormat.HasValue(WCharacterFormat.FontNameKey) && sourceCharFormat.FontName != "Times New Roman")
                    destCharFormat.FontName = sourceCharFormat.FontName;
                if (sourceCharFormat.HasValue(WCharacterFormat.FontNameAsciiKey) && sourceCharFormat.FontNameAscii != "Times New Roman")
                    destCharFormat.FontNameAscii = sourceCharFormat.FontNameAscii;
                if (sourceCharFormat.HasValue(WCharacterFormat.FontNameBidiKey) && sourceCharFormat.FontNameBidi != "Times New Roman")
                    destCharFormat.FontNameBidi = sourceCharFormat.FontNameBidi;
                if (sourceCharFormat.HasValue(WCharacterFormat.FontNameFarEastKey) && sourceCharFormat.FontNameFarEast != "Times New Roman")
                    destCharFormat.FontNameFarEast = sourceCharFormat.FontNameFarEast;
                if (sourceCharFormat.HasValue(WCharacterFormat.FontNameNonFarEastKey) && sourceCharFormat.FontNameNonFarEast != "Times New Roman")
                    destCharFormat.FontNameNonFarEast = sourceCharFormat.FontNameNonFarEast;
                if (sourceCharFormat.HasValue(WCharacterFormat.FontSizeKey))
                    destCharFormat.FontSize = sourceCharFormat.FontSize;
                if (sourceListStyle.Levels[i].BulletCharacter != null)
                    destListStyle.Levels[i].BulletCharacter = sourceListStyle.Levels[i].BulletCharacter;
                if (sourceListStyle.Levels[i].FollowCharacter != null)
                    destListStyle.Levels[i].FollowCharacter = sourceListStyle.Levels[i].FollowCharacter;
                if (sourceListStyle.Levels[i].NoLevelText == true)
                    destListStyle.Levels[i].NoLevelText = sourceListStyle.Levels[i].NoLevelText;
                if (sourceListStyle.Levels[i].NoRestartByHigher == true)
                    destListStyle.Levels[i].NoRestartByHigher = sourceListStyle.Levels[i].NoRestartByHigher;
                destListStyle.Levels[i].NumberAlignment = sourceListStyle.Levels[i].NumberAlignment;
                destListStyle.Levels[i].NumberPosition = sourceListStyle.Levels[i].NumberPosition;
                if (sourceListStyle.Levels[i].NumberPrefix != null)
                    destListStyle.Levels[i].NumberPrefix = sourceListStyle.Levels[i].NumberPrefix;
                if (sourceListStyle.Levels[i].NumberSufix != null)
                    destListStyle.Levels[i].NumberSufix = sourceListStyle.Levels[i].NumberSufix;
                if (sourceListStyle.Levels[i].PatternType != null)
                    destListStyle.Levels[i].PatternType = sourceListStyle.Levels[i].PatternType;
                destListStyle.Levels[i].StartAt = sourceListStyle.Levels[i].StartAt;
                destListStyle.Levels[i].TabSpaceAfter = sourceListStyle.Levels[i].TabSpaceAfter;
                destListStyle.Levels[i].TextPosition = sourceListStyle.Levels[i].TextPosition;
            }

        }
      
       /// <summary>
       /// Parse pagenumbering of the section
       /// </summary>
       /// <param name="token"></param>
       /// <param name="tokenKey"></param>
       /// <param name="tokenValue"></param>
        private void ParsePageNumberingToken(string token, string tokenKey, string tokenValue)
        {
            switch (tokenKey)
            {
                case "pgnstarts":
                    CurrentSection.PageSetup.PageStartingNumber = Convert.ToInt32(tokenValue);
                    break;
                case "pgnrestart":
                    CurrentSection.PageSetup.RestartPageNumbering = true;
                    CurrentSection.PageSetup.PageStartingNumber = 1;
                    break;
                case "pgndec":
                    CurrentSection.PageSetup.PageNumberStyle = PageNumberStyle.Arabic;
                    break;
                case "pgnucrm":
                    CurrentSection.PageSetup.PageNumberStyle = PageNumberStyle.RomanUpper;
                    break;
                case "pgnlcrm":
                    CurrentSection.PageSetup.PageNumberStyle = PageNumberStyle.RomanLower ;
                    break;
                case "pgnucltr":
                    CurrentSection.PageSetup.PageNumberStyle = PageNumberStyle.LetterUpper ;
                    break;
                case "pgnlcltr":
                    CurrentSection.PageSetup.PageNumberStyle = PageNumberStyle.LetterLower ;
                    break;
                default :
                    CurrentSection.PageSetup.PageNumberStyle = PageNumberStyle.Arabic;
                    break;
            }
        }
       /// <summary>
       /// Parse line numbering of the section
       /// </summary>
       /// <param name="token"></param>
       /// <param name="tokenKey"></param>
       /// <param name="tokenValue"></param>
        private void ParseLineNumberingToken(string token, string tokenKey, string tokenValue)
        {
            switch (tokenKey)
            {
                case "linex":
                   CurrentSection.PageSetup.LineNumberingDistanceFromText = Convert.ToSingle(tokenValue);
                    break;
                case "linestarts":
                    CurrentSection.PageSetup.LineNumberingStartValue = Convert.ToInt32(tokenValue);
                    break;
                case "lineppage":
                    CurrentSection.PageSetup.LineNumberingMode = LineNumberingMode.RestartPage;
                    break;
                case "linecont":
                    CurrentSection.PageSetup.LineNumberingMode = LineNumberingMode.Continuous;
                    break;
                case "line":
                    Break linebreak = CurrentPara.AppendBreak(BreakType.LineBreak);
                    CopyTextFormatToCharFormat(linebreak.TextRange.CharacterFormat, m_currTextFormat);
                    break;
            }
        }
       /// <summary>
       /// Parse font table
       /// </summary>
       /// <param name="token"></param>
       /// <param name="tokenKey"></param>
       /// <param name="tokenValue"></param>
        private void ParseFontTable(string token, string tokenKey, string tokenValue)
        {
            if (tokenKey == "f" || tokenKey == "af")
            {
                m_rtfFont = new RtfFont();
                m_rtfFont.FontID = token;
                m_rtfFont.FontNumber = Convert.ToInt32(tokenValue);
            }
            if (tokenKey == "fcharset" && m_rtfFont != null)
            {
                m_rtfFont.FontCharSet = Convert.ToInt16(tokenValue);
            }
        }

        private void ParseColorTable(string token, string tokenKey, string tokenValue)
        {         
            switch (tokenKey)
            {
                case "red":
                    m_rtfColorTable.RedN = Convert.ToInt32 (tokenValue);
                    break;
                case "green":
                    m_rtfColorTable.GreenN = Convert.ToInt32(tokenValue);            
                    break;
                case "blue":
                    m_rtfColorTable.BlueN = Convert.ToInt32 (tokenValue);
                    break;                              
            }
        }
        /// <summary>
        /// Parse Formatting controls
        /// </summary>
        /// <param name="token"></param>
        /// <param name="tokenKey"></param>
        /// <param name="tokenValue"></param>
      
        private void ParseFormattingToken(string token,string tokenKey,string tokenValue)
        {
            string text;
            float value = 0;
            if (m_bIsPicture == true && m_bIsShapePicture == false)
            {
                ParsePictureToken(token, tokenKey, tokenValue);
            }
            if (m_bIsPicture == true && m_bIsShapePicture == true)
            {
                if (token.Contains("pic") || token.StartsWith("bin"))
                    ParsePictureToken(token, tokenKey, tokenValue);
                else
                    ParseShapeToken(token, tokenKey, tokenValue);
            }
            else  if (token.StartsWith("line"))
            { 
                ParseLineNumberingToken(token ,tokenKey ,tokenValue);
            }
            else if (token.StartsWith("chpgn"))
            {
                //Append page field to the current paragraph
                m_currParagraph.AppendField("", FieldType.FieldPage);
            }
            else if (token.StartsWith("pgn"))
            {
                ParsePageNumberingToken(token, tokenKey, tokenValue);
            }
            else if (token.StartsWith("brdr"))
            {
                ParseParagraphBorders(token, tokenKey, tokenValue);
            }
            else if (token.StartsWith("vert"))
            {
                ParsePageVerticalAlignment(token, tokenKey, tokenValue);
            }
            else if (m_bIsCustomProperties)
            {
                if (m_token.StartsWith("proptype"))
                    m_currPropertyType = (Syncfusion.CompoundFile.DocIO.PropertyType)Convert.ToInt64(tokenValue);
                
            }
            else
            {
                switch (tokenKey)
                {
                    case "s":
                        if (m_currentTableType == RtfTableType.StyleSheet)
                        {
                            m_currStyleID = m_token;
                            m_currStyle = new WParagraphStyle(m_document);
                            m_currParagraph = new WParagraph(m_document);
                            m_currTextFormat = new TextFormat();
                            m_styleName = string.Empty; 
                        }
                        else
                        {
                            foreach (KeyValuePair<string, IWParagraphStyle> style in m_styleTable)
                            {
                                if (style.Key == m_token)
                                {
                                    if (!m_bIsListText)
                                    {
                                        ResetParagraphFormat();
                                        ResetCharacterFormat();
                                    }
                                    CurrentPara.ApplyStyle(style.Value.Name);
                                }
                            }
                            
                        }
                        break;
                    case "cs":
                        if (m_currentTableType == RtfTableType.StyleSheet)
                        {
                            m_currStyleID = m_token;
                            m_currCharStyle = new CharacterStyle(m_document);
                            m_currTextFormat = new TextFormat();
                            m_styleName = string.Empty;
                        }
                        else
                        {
                            foreach (KeyValuePair<string, CharacterStyle> charStyle in m_charStyleTable)
                            {
                                if (charStyle.Key == m_token)
                                {
                                    m_currTextFormat.CharacterStyleName = charStyle.Value.Name;
                                }
                            }
                        }
                        break;
                    case "cols":
                        float equalWidth = GetEqualColumnWidth(Convert.ToInt32(tokenValue));
                        for (int i = 0; i < Convert.ToInt32(tokenValue); i++)
                        {
                            CurrentSection.AddColumn(equalWidth, 36);
                        }
                        if (CurrentSection.Columns.Count > 0)
                            CurrColumn = CurrentSection.Columns[0];
                        break;
                    case "colno":
                        if (CurrentSection.Columns.Count >= Convert.ToInt32(tokenValue))
                        {
                            CurrColumn = CurrentSection.Columns[Convert.ToInt32(tokenValue) - 1];
                        }
                        break;
                    case "colsx":
                    case "colsr":
                        CurrColumn .Space  = ExtractTwipsValue(tokenValue);
                        break;
                    case "colw":
                        CurrentSection.PageSetup.EqualColumnWidth = false;
                        CurrColumn.Width = ExtractTwipsValue(tokenValue);
                        break;
                    case "viewkind":
                        switch (Convert.ToInt32(tokenValue))
                        {
                            case 0:
                                m_document.ViewSetup.DocumentViewType = DocumentViewType.None; 
                                break;
                            case 1:
                                m_document.ViewSetup.DocumentViewType = DocumentViewType.NormalLayout ;
                                break;
                            case 2:
                                m_document.ViewSetup.DocumentViewType = DocumentViewType.OutlineLayout ;
                                break;
                            case 3:
                                m_document.ViewSetup.DocumentViewType = DocumentViewType.PrintLayout ;
                                break;
                            case 4:
                                m_document.ViewSetup.DocumentViewType = DocumentViewType.WebLayout;
                                break;
                            case 5:
                                m_document.ViewSetup.DocumentViewType = DocumentViewType.OutlineLayout ;
                                break;
                        }
                        break;
                    case "viewscale":
                        m_document.ViewSetup.SetZoomPercent(Convert.ToInt32(tokenValue));
                        break;
                    case "viewzk":
                        switch (Convert.ToInt32(tokenValue))
                        {
                            case 0:
                                m_document.ViewSetup.ZoomType = ZoomType.None;
                                break;
                            case 1:
                                m_document.ViewSetup.ZoomType = ZoomType.FullPage ;
                                break;
                            case 2:
                                m_document.ViewSetup.ZoomType = ZoomType.PageWidth;
                                break;
                            case 3:
                                m_document.ViewSetup.ZoomType = ZoomType.TextFit;
                                break;
                        }
                        break;
                    case "facingp":
                        m_secFormat.DifferentOddAndEvenPage = true;
                        break;
                    case "lndscpsxn":
                        m_secFormat.PageOrientation = PageOrientation.Landscape;
                        break;
                    case "titlepg":
                        CurrentSection.PageSetup.DifferentFirstPage = true;
                        CurrentSection.HeadersFooters.LinkToPrevious = true;
                        break;
                    case "header":
                    case "headerr":
                        m_bIsHeader = true;
                        m_headerFooterStack.Push(c_groupStart );
                        m_headerFooterType = Syncfusion.DocIO.DLS.HeaderFooterType.OddHeader ;
                        m_textBody = CurrentSection.HeadersFooters.OddHeader;
                        //Clear the textbody items
                        m_textBody.Items.Clear();
                        break;                  
                    case "headerl":
                        m_bIsHeader = true;
                        m_headerFooterStack.Push(c_groupStart);
                        m_headerFooterType = Syncfusion.DocIO.DLS.HeaderFooterType.EvenHeader;
                        m_textBody = CurrentSection.HeadersFooters.EvenHeader;
                        //Clear the textbody items
                        m_textBody.Items.Clear();
                        break;
                    case "headerf":
                        m_bIsHeader = true;
                        m_headerFooterStack.Push(c_groupStart);
                        m_headerFooterType = Syncfusion.DocIO.DLS.HeaderFooterType.FirstPageHeader;
                        m_textBody = CurrentSection.HeadersFooters.FirstPageHeader;
                        //Clear the textbody items
                        m_textBody.Items.Clear();
                        break;
                    case "footerl":
                        m_bIsFooter = true;
                        m_headerFooterStack.Push(c_groupStart);
                        m_headerFooterType = Syncfusion.DocIO.DLS.HeaderFooterType.EvenFooter;
                        m_textBody = CurrentSection.HeadersFooters.EvenFooter;
                        //Clear the textbody items
                        m_textBody.Items.Clear();
                        break;
                    case "footerf":
                        m_bIsFooter = true;
                        m_headerFooterStack.Push(c_groupStart);
                        m_headerFooterType = Syncfusion.DocIO.DLS.HeaderFooterType.FirstPageFooter;
                        m_textBody = CurrentSection.HeadersFooters.FirstPageFooter;
                        //Clear the textbody items
                        m_textBody.Items.Clear();
                        break;
                    case "footer":
                    case "footerr":
                        m_bIsFooter =true ;
                        m_headerFooterStack.Push(c_groupStart);
                        m_headerFooterType =Syncfusion .DocIO .DLS .HeaderFooterType .OddFooter ;
                        m_textBody = CurrentSection.HeadersFooters.OddFooter;
                        //Clear the textbody items
                        m_textBody.Items.Clear();
                        break ;
                    case "pard":
                       if(!m_bIsListText)
                        ParseParagraphStart();                
                        break;
                    case "plain":
                        m_currTextFormat = new TextFormat();
                        if (!m_bIsListText)
                        {
                            m_textFormatStack.Push(m_currTextFormat);
                        }
                        break;
                    case "par":
                        if (IsFieldGroup && m_currentFieldGroupData != string.Empty)
                        {
                            ParseFieldGroupData(m_currentFieldGroupData);
                            m_currentFieldGroupData = string.Empty;
                        }
                        ParseParagraphEnd();                     
                        break;
                    case "rtlch":
                        m_currTextFormat.Bidi = ThreeState.True;
                        break;
                    case "ltrch":
                        m_currTextFormat.Bidi = ThreeState.False;
                        break;
                    case "shad":
                        if (tokenValue != null && Convert.ToInt32(tokenValue) == 0)
                            m_currTextFormat.Shadow  = false;
                        else
                            m_currTextFormat.Shadow  = true;
                        break;
                    //TODO: Below implementation is not proper. Need to create an independant WParagraphFormat object to 
                    //populate the paragraph formatting defined in the document as like m_currTextFormat (CharacterFormat).
                    case "sa":
                        float spaceAfter = 0;
                        if (tokenValue != null)
                            spaceAfter = ExtractTwipsValue(tokenValue);
                        CurrentPara.ParagraphFormat.AfterSpacing = spaceAfter;
                        break;
                    case "saauto":
                        if (Convert.ToInt32(tokenValue) == 1)
                            CurrentPara.ParagraphFormat.SpaceAfterAuto = true;
                        break;
                    case "sb":
                        float spaceBefore;
                        spaceBefore = ExtractTwipsValue(tokenValue);
                        CurrentPara.ParagraphFormat.BeforeSpacing = spaceBefore;
                        break;
                    case "sbauto":
                        if (Convert.ToInt32(tokenValue) == 1)
                            CurrentPara.ParagraphFormat.SpaceBeforeAuto = true;
                        break;
                    case "fs":
                        float fontSize = 0;
                        if (tokenValue != null)
                            fontSize = float.Parse(tokenValue, CultureInfo.InvariantCulture) / 2;
                        m_currTextFormat.FontSize = fontSize;
                        break;
                    case "sl":
                        m_bIsLinespacingRule = true;
                        float lineSpacing = ExtractTwipsValue(tokenValue);
                        if (lineSpacing != 0)
                        {
                            CurrentPara.ParagraphFormat.LineSpacing = lineSpacing;
                            CurrentPara.ParagraphFormat.LineSpacingRule = LineSpacingRule.AtLeast;
                        }
                        break;
					//Parse Exactly linespace
                    case "sl-":
                        m_bIsLinespacingRule = true;
                        lineSpacing = ExtractTwipsValue(tokenValue);
                        if (lineSpacing != 0)
                        {
                            CurrentPara.ParagraphFormat.LineSpacing = -lineSpacing;
                            CurrentPara.ParagraphFormat.LineSpacingRule = LineSpacingRule.Exactly;
                        }
                        break;
                    case "slmult":
                        m_bIsLinespacingRule = true;
                        int nValue = Convert.ToInt32(tokenValue);
                        if (CurrentPara.ParagraphFormat.LineSpacing < 0)
                            CurrentPara.ParagraphFormat.LineSpacingRule = LineSpacingRule.Exactly;
                        else
                        {
                            if (nValue == 1)
                                CurrentPara.ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                            else if (nValue == 0)
                                CurrentPara.ParagraphFormat.LineSpacingRule = LineSpacingRule.AtLeast;
                        }
                        break;
                    case "f":
                        foreach (KeyValuePair<string, RtfFont> fontTable in m_fontTable)
                        {
                            if (fontTable.Key == token)
                            {
                                CurrRtfFont = fontTable.Value;
                                m_currTextFormat.FontFamily = CurrRtfFont.FontName.Trim();
                            }
                        }
                        break;
                    case "cf":
                        if (Convert.ToInt32(tokenValue) == 0)
                        {
                            m_currTextFormat.FontColor = Color.Black;
                        }
                        else
                        {
                            int colorIndex = Convert.ToInt32(tokenValue);
                            foreach (KeyValuePair<int, RtfColor> colorTable in m_colorTable)
                            {
                                if (colorTable.Key == colorIndex)
                                    CurrColorTable = colorTable.Value;
                            }
                            ApplyColorTable(CurrColorTable);
                        }
                        break;
                    case "u"://Represents unicode character
                        if (m_unicodeCountStack.Count > 0)
                        {
                            m_unicodeCount = m_unicodeCountStack.Peek();
                            m_bIsUnicode = true;
                        }
                        text = ((char)(Convert.ToInt32(tokenValue))).ToString();
                        ParseDocumentElement(text);
                        break;
                    case "bullet":
                        string bulletChar = ((char)8226).ToString();
                        tr = CurrentPara.AppendText(bulletChar);
                        CopyTextFormatToCharFormat(tr.CharacterFormat, m_currTextFormat);
                        break;
                    case "u*"://Destination is represented in Unicode character
#if !(SILVERLIGHT || WP) || WINRT
                        System.Text.Encoding enc = System.Text.UnicodeEncoding.GetEncoding(GetCodePage());
#else
                        System.Text.Encoding enc = new Windows1252Encoding();
#endif
                        byte[] buffer = BitConverter.GetBytes(Convert.ToInt16(tokenValue));
                        text = enc.GetString(buffer, 0, buffer.Length);
                        text = text.Replace("\0", "");
                        ParseDocumentElement(text);
                        break;
                    case "u-"://Unicode value greater than 32767 is represented with negative value
                        if (m_unicodeCountStack.Count > 0)
                            m_unicodeCount = m_unicodeCountStack.Peek();
                        text = ((char)(65536 - Convert.ToInt32(tokenValue))).ToString();
                        ParseDocumentElement(text);
                        break;
                    case "uc"://Represent number of bytes followed by \uN that corresponds to unicode charcter
                        m_unicodeCountStack.Push(Convert.ToInt32(tokenValue));
                        break;
                    case "qc":
                        CurrentPara.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                        break;
                    case "qj":
                        CurrentPara.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Justify;
                        break;
                    case "ql":
                        CurrentPara.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                        break;
                    case "qr":
                        CurrentPara.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
                        break;
                    case "tx":
                        if (!m_bIsListLevel && !m_bIsListText )
                        {
                            CurrTabFormat.TabPosition = ExtractTwipsValue(tokenValue);
                            m_tabCollection.Add(m_tabCollection.Count + 1, m_currTabFormat);
                            CurrTabFormat = new TabFormat();
                        }
                        break;
                    case "tqc":
                        if (!m_bIsListText && !m_bIsListLevel)
                        {
                            CurrTabFormat.TabJustification = TabJustification.Centered;
                        }
                        break;
                    case "tqr":
                        if (!m_bIsListText && !m_bIsListLevel)
                        {
                            CurrTabFormat.TabJustification = TabJustification.Right;
                        }
                        break;
                    case "tqdec":
                        if (!m_bIsListText && !m_bIsListLevel)
                        {
                            CurrTabFormat.TabJustification = TabJustification.Decimal;
                        }
                        break;
                    case "tb":
                        if (!m_bIsListText && !m_bIsListLevel)
                        {
                            CurrTabFormat.TabJustification = TabJustification.Bar;
                            CurrTabFormat.TabPosition = ExtractTwipsValue(tokenValue);
                        }
                        break;
                    case "tlmdot":
                    case "tldot":
                        if (!m_bIsListText && !m_bIsListLevel)
                        {
                            CurrTabFormat.TabLeader = TabLeader.Dotted;
                        }
                        break;
                    case "tleq":
                    case "tlhyph":
                        if (!m_bIsListText && !m_bIsListLevel)
                        {
                            CurrTabFormat.TabLeader = TabLeader.Hyphenated;
                        }
                        break;
                    case "tlul":
                        if (!m_bIsListText && !m_bIsListLevel)
                        {
                            CurrTabFormat.TabLeader = TabLeader.Single;
                        }
                        break;
                    case "tlth":
                        if (!m_bIsListText && !m_bIsListLevel)
                        {
                            CurrTabFormat.TabLeader = TabLeader.Heavy;
                        }
                        break;
                    case "tab":
                        if (m_currentTableType != RtfTableType.StyleSheet && !m_bIsListText && !m_bIsListLevel  )
                        {
                            m_tabCount++;
                            if (m_tabCollection.Count == 0 || (m_tabCount > m_tabCollection.Count))
                            {
                                tr = CurrentPara.AppendText("\t");
                                CopyTextFormatToCharFormat(tr.CharacterFormat, m_currTextFormat);
                            }
                            else
                            {
								//Sort Tab collection
                                SortTabCollection();
                                foreach (KeyValuePair<int, TabFormat> tabCollection in m_tabCollection)
                                {
                                    if (tabCollection.Key == m_tabCount)
                                    {
                                        CurrentPara.ParagraphFormat.Tabs.AddTab(tabCollection.Value.TabPosition, tabCollection.Value.TabJustification, tabCollection.Value.TabLeader);
                                        tr = CurrentPara.AppendText("\t");
                                        CopyTextFormatToCharFormat(tr.CharacterFormat, m_currTextFormat);
                                    }
                                }
                            }
                        }
                        break;
                    case "fi-":
                        value = ExtractTwipsValue(tokenValue);
                        CurrentPara.ParagraphFormat.FirstLineIndent = -value;
                        break;
                    case "fi":
                        value = ExtractTwipsValue(tokenValue);
                        CurrentPara.ParagraphFormat.FirstLineIndent = value;
                        break;
                    case "cufi":
                        break;
                    case "li":                     
                            float leftIndent = ExtractTwipsValue(tokenValue);
                            CurrentPara.ParagraphFormat.LeftIndent = leftIndent;                    
                        break;
                    case "li-":
                        value = ExtractTwipsValue(tokenValue);
                        CurrentPara.ParagraphFormat.LeftIndent = -value;
                        break;
                    case "culi":
                        break;
                    case "ri":
                        float rightIndent = ExtractTwipsValue(tokenValue);
                        CurrentPara.ParagraphFormat.RightIndent = rightIndent;
                        break;
                    case "ri-":
                        value = ExtractTwipsValue(tokenValue);
                        CurrentPara.ParagraphFormat.RightIndent = -value;
                        break;
                    case "curi":
                        break;
                    case "indmirror":
                        CurrentPara.ParagraphFormat.MirrorIndents = true;
                        break;
                    case "hyphpar":
                        int hValue = GetIntValue(tokenValue);
                        if (hValue == 0)
                            CurrentPara.ParagraphFormat.SuppressAutoHyphens = true;
                        break;
                    case "keep":
                        if (tokenValue != null && Convert.ToInt32(tokenValue) == 0)
                            CurrentPara.ParagraphFormat.Keep = false;
                        else
                            CurrentPara.ParagraphFormat.Keep = true;
                        break;
                    case "keepn":
                        if (tokenValue != null && Convert.ToInt32(tokenValue) == 0)
                            CurrentPara.ParagraphFormat.KeepFollow  = false;
                        else
                            CurrentPara.ParagraphFormat.KeepFollow  = true;
                        break;
                    case "outlinelevel":
                    case "level":
                        ParseOutLineLevel(token, tokenKey, tokenValue);
                        break;
                    case "pagebb":
                        if (tokenValue != null && Convert.ToInt32(tokenValue) == 0)
                            CurrentPara.ParagraphFormat.PageBreakBefore = false;
                        else
                            CurrentPara.ParagraphFormat.PageBreakBefore = true;
                       break;
                    case "contextualspace":
                       CurrentPara.ParagraphFormat.ContextualSpacing = true;
                       break;
                    case "widctlpar":
                       CurrentPara.ParagraphFormat.WidowControl = true;
                       break;
                    case "nowidctlpar":
                       CurrentPara.ParagraphFormat.WidowControl = false;
                       break;
                    case "nowwrap":
                       CurrentPara.ParagraphFormat.WordWrap = false;
                       break;
                    case "page":
                        CurrentPara.AppendBreak(BreakType.PageBreak);
                        break;
                    case "column":
                        CurrentPara.AppendBreak(BreakType.ColumnBreak);
                        break;
                    case "wraparound":
                        CurrentPara.ParagraphFormat.WrapFrameAround = FrameWrapMode.Around;
                        break;
                    case "shading":
                        CurrentPara.ParagraphFormat.TextureStyle = GetTextureStyle(Convert.ToInt32(tokenValue));
                        break;
                    case "bgcross":
                        CurrentPara.ParagraphFormat.TextureStyle = TextureStyle.TextureCross; 
                        break;
                    case "bgdkcross":
                        CurrentPara.ParagraphFormat.TextureStyle = TextureStyle.TextureDarkCross; 
                        break;
                    case "bgdkdcross":
                        CurrentPara.ParagraphFormat.TextureStyle = TextureStyle.TextureDarkDiagonalCross; 
                        break;
                    case "bgdkbdiag":
                        CurrentPara.ParagraphFormat.TextureStyle = TextureStyle.TextureDarkDiagonalDown;
                        break;
                    case "bgdkfdiag":
                        CurrentPara.ParagraphFormat.TextureStyle = TextureStyle.TextureDarkDiagonalUp;
                        break;
                    case "bgdkhoriz":
                        CurrentPara.ParagraphFormat.TextureStyle = TextureStyle.TextureDarkHorizontal;
                        break;
                    case "bgdkvert":
                        CurrentPara.ParagraphFormat.TextureStyle = TextureStyle.TextureDarkVertical;
                        break;
                    case "bgdcross":
                        CurrentPara.ParagraphFormat.TextureStyle = TextureStyle.TextureDiagonalCross;
                        break;
                    case "bgbdiag":
                        CurrentPara.ParagraphFormat.TextureStyle = TextureStyle.TextureDiagonalDown;
                        break;
                    case "bgfdiag":
                        CurrentPara.ParagraphFormat.TextureStyle = TextureStyle.TextureDiagonalUp;
                        break;
                    case "bghoriz":
                        CurrentPara.ParagraphFormat.TextureStyle = TextureStyle.TextureHorizontal;
                        break;
                    case "bgvert":
                        CurrentPara.ParagraphFormat.TextureStyle = TextureStyle.TextureVertical;
                        break;
                    case "caps":
                        if (tokenValue != null && Convert.ToInt32(tokenValue) == 0)
                            m_currTextFormat.AllCaps  = ThreeState.False;
                        else
                            m_currTextFormat.AllCaps  = ThreeState.True;
                        break;
                    case "scaps":
                        if (tokenValue != null && Convert.ToInt32(tokenValue) == 0)
                            m_currTextFormat.SmallCaps  = ThreeState.False;
                        else
                            m_currTextFormat.SmallCaps  = ThreeState.True;
                        break;
                    case "b":
                        if (tokenValue != null && Convert.ToInt32(tokenValue) == 0)
                            m_currTextFormat.Bold = ThreeState.False;
                        else
                            m_currTextFormat.Bold = ThreeState.True;
                        break;
                    case "i":
                        if (tokenValue != null && Convert.ToInt32(tokenValue) == 0)
                            m_currTextFormat.Italic = ThreeState.False;
                        else
                            m_currTextFormat.Italic = ThreeState.True;
                        break;
                    case "ul":
                        if (tokenValue != null && Convert.ToInt32(tokenValue) == 0)
                        {
                            m_currTextFormat.Underline = ThreeState.False;
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.None;
                        }
                        else
                        {
                            m_currTextFormat.Underline = ThreeState.True;
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.Single;
                        }
                      
                        break;
                    case "ulnone":
                        m_currTextFormat.UnderlineStyle = UnderlineStyle.None;
                        m_currTextFormat.Underline = ThreeState.False;
                        break;
                    case "uldb":
                        if (tokenValue != null && Convert.ToInt32(tokenValue) == 0)
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.None;
                        else
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.Double;
                        break;
                    case "uld":
                        if (tokenValue != null && Convert.ToInt32(tokenValue) == 0)
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.None;
                        else
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.Dotted;
                        break;
                    case "uldash":
                        if (tokenValue != null && Convert.ToInt32(tokenValue) == 0)
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.None;
                        else
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.Dash;
                        break;
                    case "uldashd":
                        if (tokenValue != null && Convert.ToInt32(tokenValue) == 0)
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.None;
                        else
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.DotDash;
                        break;
                    case "uldashdd":
                        if (tokenValue != null && Convert.ToInt32(tokenValue) == 0)
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.None;
                        else
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.DotDotDash;
                        break;
                    case "ulwave":
                        if (tokenValue != null && Convert.ToInt32(tokenValue) == 0)
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.None;
                        else
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.Wavy;
                        break;
                    case "ulhwave":
                        if (tokenValue != null && Convert.ToInt32(tokenValue) == 0)
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.None;
                        else
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.WavyHeavy;
                        break;
                    case "ulldash":
                        if (tokenValue != null && Convert.ToInt32(tokenValue) == 0)
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.None;
                        else
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.DashLong;
                        break;
                    case "ulth":
                        if (tokenValue != null && Convert.ToInt32(tokenValue) == 0)
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.None;
                        else
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.Thick;
                        break;
                    case "ulthd":
                        if (tokenValue != null && Convert.ToInt32(tokenValue) == 0)
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.None;
                        else
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.DottedHeavy;
                        break;
                    case "ululdbwave":
                        if (tokenValue != null && Convert.ToInt32(tokenValue) == 0)
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.None;
                        else
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.WavyDouble;
                        break;
                    case "ulw":
                        if (tokenValue != null && Convert.ToInt32(tokenValue) == 0)
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.None;
                        else
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.Words;
                        break;
                    case "ulthldash":
                        if (tokenValue != null && Convert.ToInt32(tokenValue) == 0)
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.None;
                        else
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.DashLongHeavy;
                        break;
                    case "ulthdashdd":
                        if (tokenValue != null && Convert.ToInt32(tokenValue) == 0)
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.None;
                        else
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.DotDotDashHeavy;
                        break;
                    case "ulthdashd":
                        if (tokenValue != null && Convert.ToInt32(tokenValue) == 0)
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.None;
                        else
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.DotDashHeavy;
                        break;
                    case "ulthdash":
                        if (tokenValue != null && Convert.ToInt32(tokenValue) == 0)
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.None;
                        else
                            m_currTextFormat.UnderlineStyle = UnderlineStyle.DashHeavy;
                        break;               
                    case "sub":
                        m_currTextFormat.SubSuperScript = SubSuperScript.SubScript;
                        break;
                    case "super":
                        m_currTextFormat.SubSuperScript = SubSuperScript.SuperScript;
                        break;
                    case "nosupersub":
                        m_currTextFormat.SubSuperScript = SubSuperScript.None;
                        break;
                    case "up":
                        if (tokenValue != null)
                        {
                            m_currTextFormat.Position = ((float)Convert.ToInt32(tokenValue) / RtfNavigator.c_two);
                        }
                        else
                            m_currTextFormat.Position = 3; //Set Default Position value
                        break;
                    case "lang":
                        if (tokenValue != null)
                        {
                            m_currTextFormat.LocalIdASCII = Convert.ToInt16(tokenValue);
                        }
                        break;
                    case "langfe":
                        if (tokenValue != null)
                        {
                            m_currTextFormat.LocalIdForEast = Convert.ToInt16(tokenValue);
                        }
                        break;
                    case "dn":
                        if (tokenValue != null)
                        {
                            m_currTextFormat.Position = -((float)Convert.ToInt32(tokenValue) / RtfNavigator.c_two);
                        }
                        else
                            m_currTextFormat.Position = -3; //Set Default Position value
                        break;
                    case "strike":
                        if (tokenValue != null && Convert.ToInt32(tokenValue) == 0)
                            m_currTextFormat.Strike  = ThreeState.False;
                        else
                            m_currTextFormat.Strike  = ThreeState.True;
                        break;
                    case "striked":
                        switch (Convert.ToInt32(tokenValue))
                        {
                            case 1:
                                m_currTextFormat.DoubleStrike = ThreeState.True;
                                break;
                            case 0:
                                m_currTextFormat.DoubleStrike = ThreeState.False;
                                break;
                        }
                        break;
                    case "embo":
                        if (tokenValue != null && Convert.ToInt32(tokenValue) == 0)
                            m_currTextFormat.Emboss = ThreeState.False;
                        else
                            m_currTextFormat.Emboss = ThreeState.True;
                        break;
                    case "impr":
                        if (tokenValue != null && Convert.ToInt32(tokenValue) == 0)
                            m_currTextFormat.Engrave = ThreeState.False;
                        else
                            m_currTextFormat.Engrave  = ThreeState.True;
                        break;
                    case "sectd":
                        ParseSectionStart();
                        break;
                    case "sbknone":
                        CurrentSection.BreakCode = SectionBreakCode.NoBreak;
                        break;
                    case "sbkcol":
                        CurrentSection.BreakCode = SectionBreakCode.NewColumn;
                        break;
                    case "sbkpage":
                        CurrentSection.BreakCode = SectionBreakCode.NewPage;
                        break;
                    case "sbkeven":
                        CurrentSection.BreakCode = SectionBreakCode.EvenPage;
                        break;
                    case "sbkodd":
                        CurrentSection.BreakCode = SectionBreakCode.Oddpage;
                        break;
                    case "sect":
                        ProcessTableInfo();
                        AddNewParagraph(CurrentPara);
                        AddNewSection(CurrentSection);
                        ApplySectionFormatting();
                        m_currSection = new WSection(m_document);
                        break;
                    case "marglsxn":
                    case "margl":
                        m_secFormat.LeftMargin = ExtractTwipsValue(tokenValue);
                        break;
                    case "margtsxn":
                    case "margt":                     
                        m_secFormat.TopMargin = ExtractTwipsValue(tokenValue);
                        break;
                    case "margrsxn":
                    case "margr":                      
                        m_secFormat.RightMargin = ExtractTwipsValue(tokenValue);
                        break;
                    case "margbsxn":
                    case "margb":                        
                        m_secFormat.BottomMargin = ExtractTwipsValue(tokenValue);
                        break;
                    case "pgwsxn":
                    case "paperw":
                        m_secFormat.PageSize .Width  = ExtractTwipsValue(tokenValue );
                        break ;
                    case "pghsxn":
                    case "paperh":
                        m_secFormat.PageSize .Height  = ExtractTwipsValue(tokenValue);
                        break;
                    case "headery":
                        m_secFormat.HeaderDistance = ExtractTwipsValue(tokenValue);
                        break;
                    case "footery":
                        m_secFormat.FooterDistance = ExtractTwipsValue(tokenValue);
                        break;
                    case "deftab":
                        m_secFormat.DefaultTabWidth = ExtractTwipsValue(tokenValue);
                        break;
                    case "rquote":
                    case "lquote":
                        tr = CurrentPara.AppendText("'");
                        CopyTextFormatToCharFormat(tr.CharacterFormat, m_currTextFormat);
                        break;
                    case "rdblquote":
                    case "ldblquote":
                        tr = CurrentPara.AppendText("\"");
                        CopyTextFormatToCharFormat(tr.CharacterFormat, m_currTextFormat);
                        break;
                    case "endash":
                        char enDash = (char)8211; //Visible Character code for EnDash is 8211.
                        if (m_prevTokenType == RtfTokenType.Text && tr != null)
                            tr.Text += enDash.ToString();
                        else
                        {
                            tr = CurrentPara.AppendText(enDash.ToString());
                            CopyTextFormatToCharFormat(tr.CharacterFormat, m_currTextFormat);
                        }
                        break;
                    case "emdash":
                        char emDash = (char)8212; //Visible Character code for EmDash is 8212.
                        if (m_prevTokenType == RtfTokenType.Text && tr != null)
                            tr.Text += emDash.ToString();
                        else
                        {
                            tr = CurrentPara.AppendText(emDash.ToString());
                            CopyTextFormatToCharFormat(tr.CharacterFormat, m_currTextFormat);
                        }
                        break;
                    case "cb":
                    case "chcbpat":
                        int backColorIndex = Convert.ToInt32(tokenValue);
                        foreach (KeyValuePair<int, RtfColor> colorTable in m_colorTable)
                        {
                            if (colorTable.Key == backColorIndex)
                            {
                                CurrColorTable = colorTable.Value;
                                m_currTextFormat.BackColor = Color.FromArgb(CurrColorTable.RedN, CurrColorTable.GreenN, CurrColorTable.BlueN);
                            }
                        }
                        break;
                    //Parse paragraph back ground color
                    case "cbpat":
                        int paraBackColorIndex = Convert.ToInt32(tokenValue);
                        foreach (KeyValuePair<int, RtfColor> colorTable in m_colorTable)
                        {
                            if (colorTable.Key == paraBackColorIndex)
                            {
                                CurrColorTable = colorTable.Value;
                                CurrentPara.ParagraphFormat.BackColor = Color.FromArgb(CurrColorTable.RedN, CurrColorTable.GreenN, CurrColorTable.BlueN);
                            }
                        }
                        break;
                    //Parse paragraph fore color
                    case "cfpat":
                        int paraForeColorIndex = Convert.ToInt32(tokenValue);
                        foreach (KeyValuePair<int, RtfColor> colorTable in m_colorTable)
                        {
                            if (colorTable.Key == paraForeColorIndex)
                            {
                                CurrColorTable = colorTable.Value;
                                CurrentPara.ParagraphFormat.ForeColor = Color.FromArgb(CurrColorTable.RedN, CurrColorTable.GreenN, CurrColorTable.BlueN);
                            }
                        }
                        break;
                    case "highlight":
                        int highlightColorIndex = Convert.ToInt32(tokenValue);
                        foreach (KeyValuePair<int, RtfColor> colorTable in m_colorTable)
                        {
                            if (colorTable.Key == highlightColorIndex)
                            {
                                CurrColorTable = colorTable.Value;
                                m_currTextFormat.HighlightColor = Color.FromArgb(CurrColorTable.RedN, CurrColorTable.GreenN, CurrColorTable.BlueN);
                                //Preserve Dark Green
                                if (m_currTextFormat.HighlightColor.ToArgb() == Color.Green.ToArgb())
                                    m_currTextFormat.HighlightColor = Color.DarkGreen;
                            }
                        }
                        break;
                    case "nonshppict":
                        m_bIsShapePicture = false;
                        m_bIsShape = false;
                        if (!m_bIsPicture)
                        {
                            m_pictureStack.Push(c_groupStart);
                            m_bIsPicture = true;
                            m_currPicture = null;
                            m_picFormat = new PictureFormat();
                        }
                        break;
                    case "shppict":
                        m_bIsShapePicture = true;
                        m_currShapeFormat = new ShapeFormat();
                        if (!m_bIsPicture)
                        {
                            m_pictureStack.Push(c_groupStart);
                            m_bIsPicture = true;
                            m_currPicture = null;
                            m_picFormat = new PictureFormat();
                        }
                        break;
                    case "shp":
                        m_pictureStack.Push(c_groupStart);
                        m_bIsShape = true;
                        m_bIsShapePictureAdded = false;
                        m_bIsShapePicture = true;
                        m_currShapeFormat = new ShapeFormat();
                        m_currPicture = null;
                        m_bIsPicture = true;
                        m_picFormat = new PictureFormat();
                        break;
                    case "pict":
                        if (!m_bIsPicture)
                        {
                            m_bIsShapePicture = true;
                            m_currShapeFormat = new ShapeFormat();
                            m_currPicture = null;
                            m_pictureStack.Push(c_groupStart);
                            m_bIsPicture = true;
                            m_picFormat = new PictureFormat();
                        }
                        break;
                    case "object":
                        m_bIsObject = true;
                        m_objectStack.Push(c_groupStart);
                        break;
                    case "bkmkstart":
                        m_bIsBookmarkStart = true;
                        break;
                    case "bkmkend":
                        m_bIsBookmarkEnd = true;
                        break;
                    case "pntext":
                    case "listtext":
                        ParseListTextStart();
                        break;
                    case "ls":
                        if (!IsDestinationControlWord)
                        {
                            ApplyListFormatting(token, tokenKey, tokenValue, CurrentPara.ListFormat);
                            if (m_currentTableType == RtfTableType.None)
                                m_bIsList = true;
                        }
                        m_bIsContinousList = false;
                        break;
                    case "rtlpar":
                        CurrentPara.ParagraphFormat.Bidi = true;
                        break;
                    case "lin":
                        if (CurrentPara.ParagraphFormat.Bidi)
                            CurrentPara.ParagraphFormat.RightIndent = ExtractTwipsValue(tokenValue);
                        else
                            CurrentPara.ParagraphFormat.LeftIndent = ExtractTwipsValue(tokenValue);
                        break;
                    case "rin":
                        if (CurrentPara.ParagraphFormat.Bidi)
                            CurrentPara.ParagraphFormat.LeftIndent = ExtractTwipsValue(tokenValue);
                        else
                            CurrentPara.ParagraphFormat.RightIndent = ExtractTwipsValue(tokenValue);
                        break;
                    case "ilvl":
                        if (!IsDestinationControlWord)
                        {
                            if (Convert.ToInt32(tokenValue) >= 9 && Convert.ToInt32(tokenValue) <= 12)
                            {
                                return;
                            }
                            else
                            {
                                CurrentPara.ListFormat.ListLevelNumber = Convert.ToInt32(tokenValue);
                            }
                        }

                        break;
                    case "pn":
                        m_pnLevelNumber++;
                        if (m_pnLevelNumber >= 9)
                            m_pnLevelNumber = 0;
                        break;
                    case "pnlvlblt":
                        if (m_currentTableType == RtfTableType.None)
                            m_bIsList = true;
                        CurrentPara.ListFormat.ApplyDefBulletStyle();
                        CurrentPara.ListFormat.ListLevelNumber = m_pnLevelNumber;
                        m_bIsContinousList = false;
                        break;
                    case "pnf":
                        foreach (KeyValuePair<string, RtfFont> fontTable in m_fontTable)
                        {
                            if (fontTable.Key == token)
                            {
                                CurrRtfFont = fontTable.Value;
                                CurrentPara.ListFormat.CurrentListLevel.CharacterFormat.FontName = CurrRtfFont.FontName;
                            }
                        }
                        break;
                    case "pnlvlbody":
                        if (m_currentTableType == RtfTableType.None)
                            m_bIsList = true;
                        if ((m_bIsPreviousList && m_bIsContinousList) || !m_bIsContinousList)
                        {
                            CurrentPara.ListFormat.ApplyDefNumberedStyle();
                            CurrentPara.ListFormat.ListLevelNumber = m_pnLevelNumber;
                            CurrentPara.ListFormat.CurrentListLevel.NumberSufix = string.Empty;
                            CurrentPara.ListFormat.CurrentListLevel.NumberAlignment = ListNumberAlignment.Left;
                            m_bIsContinousList=false;
                        }
                        else if (m_bIsContinousList)
                        {
                            CurrentPara.ListFormat.ContinueListNumbering();
                            m_bIsContinousList = false;
                        }
                        break;
                    case "pnlvl":
                        if (CurrentPara.ListFormat != null && CurrentPara.ListFormat.CurrentListLevel != null)
                            CurrentPara.ListFormat.ListLevelNumber = Convert.ToInt32(tokenValue);
                        break;
                    case "pnstart":
                        if(CurrentPara .ListFormat!=null && CurrentPara .ListFormat .CurrentListLevel !=null  )
                        CurrentPara.ListFormat.CurrentListLevel.StartAt = Convert.ToInt32(tokenValue );
                        break;
                    case "pndec":
                        if (CurrentPara.ListFormat != null && CurrentPara.ListFormat.CurrentListLevel != null)
                        CurrentPara.ListFormat.CurrentListLevel.PatternType = ListPatternType.Arabic ;
                        break;
                    case "pnlcrm":
                        if (CurrentPara.ListFormat != null && CurrentPara.ListFormat.CurrentListLevel != null)
                        CurrentPara.ListFormat.CurrentListLevel.PatternType = ListPatternType.LowRoman;
                        break;
                    case "pnucrm":
                        if (CurrentPara.ListFormat != null && CurrentPara.ListFormat.CurrentListLevel != null)
                        CurrentPara.ListFormat.CurrentListLevel.PatternType = ListPatternType.UpRoman;
                        break;
                    case "pnucltr":
                        if (CurrentPara.ListFormat != null && CurrentPara.ListFormat.CurrentListLevel != null)
                        CurrentPara.ListFormat.CurrentListLevel.PatternType = ListPatternType.UpLetter;
                        break;
                    case "pnlcltr":
                        if (CurrentPara.ListFormat != null && CurrentPara.ListFormat.CurrentListLevel != null)
                        CurrentPara.ListFormat.CurrentListLevel.PatternType = ListPatternType.LowLetter;
                        break;
                    case "pnord":
                        if (CurrentPara.ListFormat != null && CurrentPara.ListFormat.CurrentListLevel != null)
                        CurrentPara.ListFormat.CurrentListLevel.PatternType = ListPatternType.Ordinal;
                        break;
                    case "pnordt":
                        if (CurrentPara.ListFormat != null && CurrentPara.ListFormat.CurrentListLevel != null)
                        CurrentPara.ListFormat.CurrentListLevel.PatternType = ListPatternType.OrdinalText;
                        break;
                    case "pntxta.":
                        if (CurrentPara.ListFormat != null && CurrentPara.ListFormat.CurrentListLevel != null)
                        CurrentPara.ListFormat.CurrentListLevel.NumberSufix = ".";
                        break;
                    case "pnindent":
                        if (CurrentPara.ListFormat != null && CurrentPara.ListFormat.CurrentListLevel != null)
                        {
                            CurrentPara.ListFormat.CurrentListLevel.ParagraphFormat.LeftIndent = ExtractTwipsValue(tokenValue);
                            CurrentPara.ListFormat.CurrentListLevel.TextPosition = ExtractTwipsValue(tokenValue);
                        }
                        break;
                    case "pnsp":
                        if (CurrentPara.ListFormat != null && CurrentPara.ListFormat.CurrentListLevel != null)
                            CurrentPara.ListFormat.CurrentListLevel.TabSpaceAfter = ExtractTwipsValue(tokenValue);
                        break;
                    case "pnb*":
                    case "pnb":
                        if (CurrentPara.ListFormat != null && CurrentPara.ListFormat.CurrentListLevel != null)
                        CurrentPara.ListFormat.CurrentListLevel.CharacterFormat.Bold = true;
                        break;
                    case "pni*":
                    case "pni":
                        if (CurrentPara.ListFormat != null && CurrentPara.ListFormat.CurrentListLevel != null)
                        CurrentPara.ListFormat.CurrentListLevel.CharacterFormat.Italic = true;
                        break;
                    case "pncaps*":
                    case "pncaps":
                        if (CurrentPara.ListFormat != null && CurrentPara.ListFormat.CurrentListLevel != null)
                        CurrentPara.ListFormat.CurrentListLevel.CharacterFormat.AllCaps = true;
                        break;
                    case "pnul*":
                    case "pnul":
                        if (CurrentPara.ListFormat != null && CurrentPara.ListFormat.CurrentListLevel != null)
                        CurrentPara.ListFormat.CurrentListLevel.CharacterFormat.UnderlineStyle = UnderlineStyle.Single;
                        break;
                    case "pnuld*":
                    case "pnuld":
                        if (CurrentPara.ListFormat != null && CurrentPara.ListFormat.CurrentListLevel != null)
                        CurrentPara.ListFormat.CurrentListLevel.CharacterFormat.UnderlineStyle = UnderlineStyle.Dotted;
                        break;
                    case "pnuldash*":
                    case "pnuldash":
                        if (CurrentPara.ListFormat != null && CurrentPara.ListFormat.CurrentListLevel != null)
                        CurrentPara.ListFormat.CurrentListLevel.CharacterFormat.UnderlineStyle = UnderlineStyle.Dash;
                        break;
                    case "pnulwave*":
                    case "pnulwave":
                        if (CurrentPara.ListFormat != null && CurrentPara.ListFormat.CurrentListLevel != null)
                        CurrentPara.ListFormat.CurrentListLevel.CharacterFormat.UnderlineStyle = UnderlineStyle.Wavy;
                        break;
                    case "pnuldb*":
                    case "pnuldb":
                        if (CurrentPara.ListFormat != null && CurrentPara.ListFormat.CurrentListLevel != null)
                        CurrentPara.ListFormat.CurrentListLevel.CharacterFormat.UnderlineStyle = UnderlineStyle.Double ;
                        break;
                    case "pnulth*":
                    case "pnulth":
                        if (CurrentPara.ListFormat != null && CurrentPara.ListFormat.CurrentListLevel != null)
                        CurrentPara.ListFormat.CurrentListLevel.CharacterFormat.UnderlineStyle = UnderlineStyle.Thick;
                        break;
                    case "pnulnone*":
                    case "pnulnone":
                        if (CurrentPara.ListFormat != null && CurrentPara.ListFormat.CurrentListLevel != null)
                        CurrentPara.ListFormat.CurrentListLevel.CharacterFormat.UnderlineStyle = UnderlineStyle.None;
                        break;
                    case "pnfs":
                        if (CurrentPara.ListFormat != null && CurrentPara.ListFormat.CurrentListLevel != null)
                        CurrentPara.ListFormat.CurrentListLevel.CharacterFormat.FontSize = Convert.ToInt32(tokenValue) / 2;
                        break;
                    case "pnqc":
                        if (CurrentPara.ListFormat != null && CurrentPara.ListFormat.CurrentListLevel != null)
                            CurrentPara.ListFormat.CurrentListLevel.NumberAlignment = ListNumberAlignment.Center;
                        break;
                    case "pnql":
                        if (CurrentPara.ListFormat != null && CurrentPara.ListFormat.CurrentListLevel != null)
                            CurrentPara.ListFormat.CurrentListLevel.NumberAlignment = ListNumberAlignment.Left;
                        break;
                    case "pnqr":
                        if (CurrentPara.ListFormat != null && CurrentPara.ListFormat.CurrentListLevel != null)
                            CurrentPara.ListFormat.CurrentListLevel.NumberAlignment = ListNumberAlignment.Right;
                        break;
                    case "pnlvlcont":
                        if (CurrentPara.ListFormat != null && CurrentPara.ListFormat.CurrentListLevel != null)
                        {
                            CurrentPara.ListFormat.ContinueListNumbering();
                            CurrentPara.ListFormat.CurrentListLevel.NoLevelText = true;
                        }
                        if (m_bIsPreviousList)
                            m_bIsContinousList = true;
                        break;
                    //Parse Paragraph border space
                    case "brsp":
                        float space = ExtractTwipsValue(tokenValue);
                        if (!m_bIsRow)
                        {
                            if (m_bIsBorderBottom)
                                CurrentPara.ParagraphFormat.Borders.Bottom.Space = space;
                            if (m_bIsBorderLeft)
                                CurrentPara.ParagraphFormat.Borders.Left.Space = space;
                            if (m_bIsBorderTop)
                                CurrentPara.ParagraphFormat.Borders.Top.Space = space;
                            if (m_bIsBorderRight)
                                CurrentPara.ParagraphFormat.Borders.Right.Space = space;
                        }
                        break;
                    case "background":
                        m_bIsBackgroundCollection = true;
                        m_backgroundCollectionStack.Push("{");
                        break;
                    //Table formatting control words
                    case "trowd":
                        ParseRowStart();
                        break;
                    case "trql":
                        CurrRowFormat.HorizontalAlignment = RowAlignment.Left;
                        break;
                    case "trqr":
                        CurrRowFormat.HorizontalAlignment = RowAlignment.Right;
                        break;
                    case "trqc":
                        CurrRowFormat.HorizontalAlignment = RowAlignment.Center;
                        break;
                    case "ltrrow":
                        m_bIsRow = true;
                        break;
                    case "row":
                    case "nestrow":
                        ParseRowEnd();                    
                        break;
                    case "cellx":
                    case "cellx-":
                        ParseCellBoundary(token, tokenKey, tokenValue);
                        break;
                    case "cell":
                    case "nestcell":
                        if (IsFieldGroup && m_currentFieldGroupData != string.Empty)
                        {
                            ParseFieldGroupData(m_currentFieldGroupData);
                            m_currentFieldGroupData = string.Empty;
                        }
                        ProcessTableInfo();
                        AddNewParagraph(CurrentPara);
                        if (CurrCell != null)
                            CopyTextFormatToCharFormat(CurrCell.CharacterFormat, m_currTextFormat);
                        m_bCellFinished = true;
                        m_currParagraph = new WParagraph(m_document);
                        CopyParagraphFormatting(m_prevFormat, m_currParagraph.ParagraphFormat);
                        break;
                    case "intbl":
                        m_bInTable = true;
                        m_bIsRow = true;
                        m_currentLevel = 1;
                        break;
                    case "itap":
                        int prevLevel = m_nestedTextBody.Count == 0 ? 0 : m_nestedTable.Count;
                        m_currentLevel = Convert.ToInt32(tokenValue);
                        break;
                    case "lastrow":
                        m_bIsLastRow = true;
                        break;
                        //Parsing row formattings
                    case "tdfrmtxtLeft":
                        CurrRowFormat.Positioning.DistanceFromLeft = ExtractTwipsValue(tokenValue);
                        break;
                    case "tdfrmtxtRight":
                        CurrRowFormat.Positioning.DistanceFromRight = ExtractTwipsValue(tokenValue);
                        break;
                    case "tdfrmtxtTop":
                        CurrRowFormat.Positioning.DistanceFromTop = ExtractTwipsValue(tokenValue);
                        break;
                    case "tdfrmtxtBottom":
                        CurrRowFormat.Positioning.DistanceFromBottom = ExtractTwipsValue(tokenValue);
                        break;
                    case "tphcol":
                        CurrRowFormat.Positioning.HorizRelationTo = HorizontalRelation.Column;
                        break;
                    case "tphmrg":
                        CurrRowFormat.Positioning.HorizRelationTo = HorizontalRelation.Margin;
                        break;
                    case "tphpg":
                        CurrRowFormat.Positioning.HorizRelationTo = HorizontalRelation.Page;
                        break;
                    case "tposx":
                        CurrRowFormat.Positioning.HorizPosition = ExtractTwipsValue(tokenValue);
                        break;
                    case "tposxc":
                        CurrRowFormat.Positioning.HorizPositionAbs = HorizontalPosition.Center;
                        break;
                    case "tposxi":
                        CurrRowFormat.Positioning.HorizPositionAbs = HorizontalPosition.Inside;
                        break;
                    case "tposxl":
                        CurrRowFormat.Positioning.HorizPositionAbs = HorizontalPosition.Left;
                        break;
                    case "tposxo":
                        CurrRowFormat.Positioning.HorizPositionAbs = HorizontalPosition.Outside;
                        break;
                    case "tposxr":
                        CurrRowFormat.Positioning.HorizPositionAbs = HorizontalPosition.Right;
                        break;
                    case "tposy":
                        CurrRowFormat.Positioning.VertPosition = ExtractTwipsValue(tokenValue);
                        break;
                    case "tposyb":
                        CurrRowFormat.Positioning.VertPositionAbs = VerticalPosition.Bottom;
                        break;
                    case "tposyc":
                        CurrRowFormat.Positioning.VertPositionAbs = VerticalPosition.Center;
                        break;
                    case "tposyin":
                        CurrRowFormat.Positioning.VertPositionAbs = VerticalPosition.Inside;
                        break;
                    case "tposyout":
                        CurrRowFormat.Positioning.VertPositionAbs = VerticalPosition.Outside;
                        break;
                    case "tposyt":
                        CurrRowFormat.Positioning.VertPositionAbs = VerticalPosition.Top;
                        break;
                    case "tpvmrg":
                        CurrRowFormat.Positioning.VertRelationTo = VerticalRelation.Margin;
                        break;
                    case "tpvpara":
                        CurrRowFormat.Positioning.VertRelationTo = VerticalRelation.Paragraph;
                        break;
                    case "tpvpg":
                        CurrRowFormat.Positioning.VertRelationTo = VerticalRelation.Page;
                        break;
                    case "taprtl":
                            CurrRowFormat.Bidi = true;
                        break;
                    case "trhdr":
                        if (CurrRow != null)
                            CurrRow.IsHeader = true;
                        break;
                    case "trkeep":
                        CurrRowFormat.IsBreakAcrossPages = true;
                        break;
                    case "trrh":
                        value = ExtractTwipsValue(tokenValue);
                        CurrRowFormat.Height = value;
                        break;
					//Parse Exactly Row Height
                    case "trrh-":
                        value = ExtractTwipsValue(tokenValue);
                        CurrRowFormat.Height = -value;
                        break;
                    case "trpaddb":
                        value = ExtractTwipsValue(tokenValue);
                        CurrRowFormat.Paddings.Bottom = value;
                        break;
                    case "trpaddl":
                        value = ExtractTwipsValue(tokenValue);
                        CurrRowFormat.Paddings.Left = value;
                        break;
                    case "trpaddr":
                        value = ExtractTwipsValue(tokenValue);
                        CurrRowFormat.Paddings.Right = value;
                        break;
                    case "trpaddt":
                        value = ExtractTwipsValue(tokenValue);
                            CurrRowFormat.Paddings.Top = value;
                        break;
                    case "trpaddfb":
                        if (Convert.ToInt32(tokenValue) == 0)
                                CurrRowFormat.Paddings.Bottom = 0;
                        break;
                    case "trpaddft":
                        if (Convert.ToInt32(tokenValue) == 0 )
                            CurrRowFormat.Paddings.Top = 0;
                        break;
                    case "trpaddfr":
                        if (Convert.ToInt32(tokenValue) == 0 )
                            CurrRowFormat.Paddings.Right = 0;
                        break;
                    case "trpaddfl":
                        if (Convert.ToInt32(tokenValue) == 0 )
                            CurrRowFormat.Paddings.Left = 0;
                        break;
                    case "trspdb":
                        value = ExtractTwipsValue(tokenValue);
                            m_bottomcellspace = value;
                        break;
                    case "trspdl":
                        value = ExtractTwipsValue(tokenValue);
                            m_leftcellspace = value;
                        break;
                    case "trspdr":
                        value = ExtractTwipsValue(tokenValue);
                            m_rightcellspace = value;
                        break;
                    case "trspdt":
                        value = ExtractTwipsValue(tokenValue);
                            m_topcellspace = value;
                        break;
                    case "trspdfb":
                        if (Convert.ToInt32(tokenValue) == 3)
                            CurrRowFormat.CellSpacing = m_bottomcellspace;
                        break;
                    case "trspdft":
                        if (Convert.ToInt32(tokenValue) == 3)
                            CurrRowFormat.CellSpacing = m_topcellspace;
                        break;
                    case "trspdfl":
                        if (Convert.ToInt32(tokenValue) == 3)
                            CurrRowFormat.CellSpacing = m_leftcellspace;
                        break;
                    case "trspdfr":
                        if (Convert.ToInt32(tokenValue) == 3)
                            CurrRowFormat.CellSpacing = m_rightcellspace;
                        break;
                    case "trgaph":
                        value = ExtractTwipsValue(tokenValue);
                        CurrRowFormat.Paddings.Left = CurrRowFormat.Paddings.Right = value;
                        m_bIsWord97StylePadding = true;
                        break;
                    case "trleft":
                        if (m_bIsWord97StylePadding)
                        {
                            value = ExtractTwipsValue(tokenValue);
                            CurrRowFormat.LeftIndent = CurrRowFormat.Paddings.Left + value;
                            m_currRowLeftIndent = (int)(CurrRowFormat.LeftIndent * 20.0);
                        }
                        m_bIsWord97StylePadding = false;
                        break;
                    case "trleft-":
                        if (m_bIsWord97StylePadding)
                        {
                            value = -ExtractTwipsValue(tokenValue);
                            CurrRowFormat.LeftIndent = CurrRowFormat.Paddings.Left + value;
                            m_currRowLeftIndent = (int)(CurrRowFormat.LeftIndent * 20.0);
                        }
                        m_bIsWord97StylePadding = false;
                        break;
                    case "tblind":
                        value = ExtractTwipsValue(tokenValue);
                        m_currRowLeftIndent = Convert.ToInt32(tokenValue);
                        CurrRowFormat.LeftIndent = value;
                        break;
                    case "tblind-":
                        value = ExtractTwipsValue(tokenValue);
                        m_currRowLeftIndent = -Convert.ToInt32(tokenValue);
                        CurrRowFormat.LeftIndent = -value;
                        break;
                    case "trautofit":
                        if(tokenValue == "1")
                            CurrRowFormat.IsAutoResized = true;
                        break;
                    case "trftsWidth":
                        CurrRowFormat.PreferredWidth.WidthType = (FtsWidth)Convert.ToInt32(tokenValue);
                        break;
                    case "trwWidth":
                        if (CurrRowFormat.PreferredWidth.WidthType == FtsWidth.Percentage)
                            CurrRowFormat.PreferredWidth.Width = (float)GetIntValue(tokenValue) / DLSConstants.PercentageFactor;
                        else if (CurrRowFormat.PreferredWidth.WidthType == FtsWidth.Point)
                            CurrRowFormat.PreferredWidth.Width = (float)GetIntValue(tokenValue) / DLSConstants.TwipsInOnePoint;
                        break;
                    case "trftsWidthB":
                        CurrRowFormat.GridBeforeWidth.WidthType = (FtsWidth)Convert.ToInt32(tokenValue);
                        break;
                    case "trwWidthB":
                        if (CurrRowFormat.GridBeforeWidth.WidthType == FtsWidth.Percentage)
                            CurrRowFormat.GridBeforeWidth.Width = (float)GetIntValue(tokenValue) / DLSConstants.PercentageFactor;
                        else if (CurrRowFormat.GridBeforeWidth.WidthType == FtsWidth.Point)
                            CurrRowFormat.GridBeforeWidth.Width = (float)GetIntValue(tokenValue) / DLSConstants.TwipsInOnePoint;
                        break;
                    case "trftsWidthA":
                        CurrRowFormat.GridAfterWidth.WidthType = (FtsWidth)Convert.ToInt32(tokenValue);
                        break;
                    case "trwWidthA":
                        if (CurrRowFormat.GridAfterWidth.WidthType == FtsWidth.Percentage)
                            CurrRowFormat.GridAfterWidth.Width = (float)GetIntValue(tokenValue) / DLSConstants.PercentageFactor;
                        else if (CurrRowFormat.GridAfterWidth.WidthType == FtsWidth.Point)
                            CurrRowFormat.GridAfterWidth.Width = (float)GetIntValue(tokenValue) / DLSConstants.TwipsInOnePoint;
                        break;
                        //Parse cell formatting
                    case "clFitText":
                        CurrCellFormat.FitText = true;
                        break;
                    case "clNoWrap":
                        CurrCellFormat.TextWrap = false;
                        break;
                    case "clpadt":
                        CurrCellFormat.Paddings.Left = ExtractTwipsValue(tokenValue );
                        break;
                    case "clpadl":
                        CurrCellFormat.Paddings.Top = ExtractTwipsValue(tokenValue);
                        break;
                    case "clpadb":
                        CurrCellFormat.Paddings.Bottom = ExtractTwipsValue(tokenValue);
                        break;
                    case "clpadr":
                        CurrCellFormat.Paddings.Right = ExtractTwipsValue(tokenValue);
                        break;
                    case "clpadfl":
                        break;
                    case "clpadft":
                        break;
                    case "clpadfb":
                        break;
                    case "clpadfr":
                        break;
                    case "clhidemark":
                        CurrRowFormat.Hidden = true;
                        break;
                    case "clwWidth":
                        if (Convert.ToInt32(m_previousTokenValue) == 3)
                        {
                            CurrCellFormat.PreferredWidth.Width = ExtractTwipsValue(tokenValue);
                            CurrCellFormat.PreferredWidth.WidthType = FtsWidth.Point;
                        }
                        break;
                    case "clvertalt":
                        CurrCellFormat.VerticalAlignment = VerticalAlignment.Top;
                        break;
                    case "clvertalc":
                        CurrCellFormat.VerticalAlignment = VerticalAlignment.Middle ;
                        break;
                    case "clvertalb":
                        CurrCellFormat.VerticalAlignment = VerticalAlignment.Bottom;
                        break;
                    case "cltxlrtb":
                        CurrCellFormat.TextDirection = TextDirection.Horizontal;
                        break;
                    case "cltxtbrl":
                        CurrCellFormat.TextDirection = TextDirection.VerticalTopToBottom;
                        break;
                    case "cltxbtlr":
                        CurrCellFormat.TextDirection = TextDirection.VerticalBottomToTop;
                        break;
                    case "cltxlrtbv":
                        CurrCellFormat.TextDirection = TextDirection.VerticalTopToBottom;
                        break;
                    case "cltxtbrlv":
                        CurrCellFormat.TextDirection = TextDirection.VerticalTopToBottom;
                        break;
                    case "clbrdrt":
                        m_bIsBorderTop = true;
                        m_bIsBorderBottom = false;
                        m_bIsBorderLeft = false;
                        m_bIsBorderRight = false;
                        m_bIsRowBorderBottom = false;
                        m_bIsRowBorderLeft = false;
                        m_bIsRowBorderRight = false;
                        m_bIsRowBorderTop = false;
                        break;
                    case "clbrdrr":
                        m_bIsBorderRight = true;
                        m_bIsBorderTop = false ;
                        m_bIsBorderBottom = false;
                        m_bIsBorderLeft = false;
                        m_bIsRowBorderBottom = false;
                        m_bIsRowBorderLeft = false;
                        m_bIsRowBorderRight = false;
                        m_bIsRowBorderTop = false;
                        break;
                    case "clbrdrl":
                        m_bIsBorderLeft  = true;
                        m_bIsBorderRight = false ;
                        m_bIsBorderTop = false;
                        m_bIsBorderBottom = false;
                        m_bIsRowBorderBottom = false;
                        m_bIsRowBorderLeft = false;
                        m_bIsRowBorderRight = false;
                        m_bIsRowBorderTop = false;
                        break;
                    case "clbrdrb":
                        m_bIsBorderBottom  = true;
                        m_bIsBorderLeft = false;
                        m_bIsBorderRight = false;
                        m_bIsBorderTop = false;
                        m_bIsRowBorderBottom = false;
                        m_bIsRowBorderLeft = false;
                        m_bIsRowBorderRight = false;
                        m_bIsRowBorderTop = false;
                        break;
                    case "trbrdrt":
                        m_bIsRowBorderTop = true;
                        m_bIsRowBorderBottom = false;
                        m_bIsRowBorderLeft = false;
                        m_bIsRowBorderRight = false;
                        m_bIsBorderBottom  = false;
                        m_bIsBorderLeft = false;
                        m_bIsBorderRight = false;
                        m_bIsBorderTop = false;
                        break;
                    case "trbrdrr":
                        m_bIsRowBorderRight = true;
                        m_bIsRowBorderTop = false;
                        m_bIsRowBorderBottom = false;
                        m_bIsRowBorderLeft = false;
                        m_bIsBorderBottom  = false;
                        m_bIsBorderLeft = false;
                        m_bIsBorderRight = false;
                        m_bIsBorderTop = false;
                        break;
                    case "trbrdrl":
                        m_bIsRowBorderLeft = true;
                        m_bIsRowBorderRight = false;
                        m_bIsRowBorderTop = false;
                        m_bIsRowBorderBottom = false;
                        m_bIsBorderBottom  = false;
                        m_bIsBorderLeft = false;
                        m_bIsBorderRight = false;
                        m_bIsBorderTop = false;
                        break;
                    case "trbrdrb":
                        m_bIsRowBorderBottom = true;
                        m_bIsRowBorderLeft = false;
                        m_bIsRowBorderRight = false;
                        m_bIsRowBorderTop = false;
                        m_bIsBorderBottom  = false;
                        m_bIsBorderLeft = false;
                        m_bIsBorderRight = false;
                        m_bIsBorderTop = false;
                        break;
                    case "trbrdrh":
                        m_bIsHorizontalBorder = true;
                        m_bIsVerticalBorder = false;
                        break;
                    case "trbrdrv":
                        m_bIsVerticalBorder = true;
                        m_bIsHorizontalBorder = false;
                        break;
                    case "clcbpat":
                        int cellColorIndex = Convert.ToInt32(tokenValue);
                        foreach (KeyValuePair<int, RtfColor> colorTable in m_colorTable)
                        {
                            if (colorTable.Key == cellColorIndex)
                            {
                                CurrColorTable = colorTable.Value;
                                CurrCellFormat .BackColor  = Color.FromArgb(CurrColorTable.RedN, CurrColorTable.GreenN, CurrColorTable.BlueN);
                            }
                        }
                        break;
                    case "clshdngraw":
                    case "clshdng":
                        CurrCellFormat.TextureStyle = GetTextureStyle(Convert.ToInt32(tokenValue));
                        break;
                    case "clbghoriz":
                    case "rawclbghoriz":
                        CurrCellFormat.TextureStyle = TextureStyle.TextureHorizontal;
                        break;
                    case "clbgvert":
                    case "rawclbgvert":
                        CurrCellFormat.TextureStyle = TextureStyle.TextureVertical;
                        break;
                    case "clbgfdiag":
                    case "rawclbgfdiag":
                        CurrCellFormat.TextureStyle = TextureStyle.TextureDiagonalDown;
                        break;
                    case "clbgbdiag":
                    case "rawclbgbdiag":
                        CurrCellFormat.TextureStyle = TextureStyle.TextureDiagonalUp;
                        break;
                    case "clbgcross":
                    case "rawclbgcross":
                        CurrCellFormat.TextureStyle = TextureStyle.TextureCross;
                        break;
                    case "clbgdcross":
                    case "rawclbgdcross":
                        CurrCellFormat.TextureStyle = TextureStyle.TextureDiagonalCross;
                        break;
                    case "clbgdkhor":
                    case "rawclbgdkhor":
                        CurrCellFormat.TextureStyle = TextureStyle.TextureDarkHorizontal;
                        break;
                    case "clbgdkvert":
                    case "rawclbgdkvert":
                        CurrCellFormat.TextureStyle = TextureStyle.TextureDarkVertical;
                        break;
                    case "clbgdkfdiag":
                    case "rawclbgdkfdiag":
                        CurrCellFormat.TextureStyle = TextureStyle.TextureDarkDiagonalDown;
                        break;
                    case "clbgdkbdiag":
                    case "rawclbgdkbdiag":
                        CurrCellFormat.TextureStyle = TextureStyle.TextureDarkDiagonalUp;
                        break;
                    case "clbgdkcross":
                    case "rawclbgdkcross":
                        CurrCellFormat.TextureStyle = TextureStyle.TextureDarkCross;
                        break;
                    case "clbgdkdcross":
                    case "rawclbgdkdcross":
                        CurrCellFormat.TextureStyle = TextureStyle.TextureDarkDiagonalCross;
                        break;
                    case "clvmgf":
                        CurrCellFormat.VerticalMerge  = CellMerge.Start;
                        break;
                    case "clvmrg":
                        CurrCellFormat.VerticalMerge = CellMerge.Continue;
                        break;
                    case "field":
                        if (!IsFieldGroup)
                        {
                            m_currentFieldGroupData = string.Empty;
                            m_fieldInstructionGroupStack = new Stack<int>();
                            m_fieldResultGroupStack = new Stack<int>();
                            m_fieldGroupStack = new Stack<int>();
                        }
                        if (m_fieldGroupStack.Count > 0)
                            m_fieldGroupStack.Push(m_fieldGroupStack.Pop() - 1);
                        m_fieldGroupStack.Push(1); //Push integer 1 to ensure the start(group start '{' that exist before \field control word) of the \field group.
                        break;
                    case "fldinst":
                        if (m_fieldInstructionGroupStack.Count > 0)
                            m_fieldInstructionGroupStack.Push(m_fieldInstructionGroupStack.Pop() - 1);
                        m_fieldInstructionGroupStack.Push(1); //Push integer 1 to ensure the start(group start '{' that exist before \fldinst control word) of the \fldinst group.
                        m_fieldGroupTypeStack.Push(FieldGroupType.FieldInstruction);
                        break;
                    case "fldrslt":
                        if (m_fieldCollectionStack.Count > 0 && 
                            (m_fieldCollectionStack.Peek().FieldType != FieldType.FieldMergeField 
                            && m_fieldCollectionStack.Peek().FieldType != FieldType.FieldNext 
                            && m_fieldCollectionStack.Peek().FieldType != FieldType.FieldShape))
                        {
                            WFieldMark fieldMark = new WFieldMark(m_document);
                            fieldMark.Type = FieldMarkType.FieldSeparator;
                            CurrentPara.ChildEntities.Add(fieldMark);
                            //Set FieldSeperator for the WField object
                            m_fieldCollectionStack.Peek().FieldSeparator = fieldMark;
                        }
                        if (m_fieldResultGroupStack.Count > 0)
                            m_fieldResultGroupStack.Push(m_fieldResultGroupStack.Pop() - 1);
                        m_fieldResultGroupStack.Push(1); //Push integer 1 to ensure the start(group start '{' that exist before \fldrslt control word) of the \fldrslt group.
                        m_fieldGroupTypeStack.Push(FieldGroupType.FieldResult);
                        break;
                    case "v":
                        if( tokenValue !=null &&  Convert .ToInt32 (tokenValue)==0)
                            m_currTextFormat.IsHiddenText = false; 
                        else
                            m_currTextFormat.IsHiddenText = true;                                               
                        break;
                    case "fftype":
                        if (m_currentFormField != null)
                        {
                            switch (Convert.ToInt32(tokenValue))
                            {
                                case 0:
                                    m_currentFormField.FormFieldType = FormFieldType.TextInput;
                                    break;
                                case 1:
                                    m_currentFormField.FormFieldType = FormFieldType.CheckBox;
                                    break;
                                case 2:
                                    m_currentFormField.FormFieldType = FormFieldType.DropDown;
                                    break;
                            }
                        }
                        break;
                    case "ffprot":
                        if (m_currentFormField != null && tokenValue == "1")
                            m_currentFormField.Enabled = false;
                        break;
                    case "ffsize":
                        if (m_currentFormField != null)
                        {
                            if (tokenValue == null || (tokenValue != null && tokenValue == "1"))
                                m_currentFormField.CheckboxSizeType = CheckBoxSizeType.Exactly;
                            else
                                m_currentFormField.CheckboxSizeType = CheckBoxSizeType.Auto;
                        }
                        break;
                    case "ffrecalc":
                        if (m_currentFormField != null)
                        {
                            if (tokenValue == null || (tokenValue != null && tokenValue == "1"))
                                m_currentFormField.CalculateOnExit = true;
                            else
                                m_currentFormField.CalculateOnExit = false;
                        }
                        break;
                    case "ffhaslistbox":
                        if (m_currentFormField != null)
                        {
                            if (tokenValue == null || (tokenValue != null && tokenValue == "1"))
                            {
                                m_currentFormField.IsListBox = true;
                                m_currentFormField.DropDownItems = new WDropDownCollection(m_document);
                            }
                            else
                                m_currentFormField.IsListBox = false;
                        }
                        break;
                    case "ffmaxlen":
                        if (m_currentFormField != null && tokenValue != null && tokenValue != string.Empty)
                        {
                            m_currentFormField.MaxLength = Convert.ToInt32(tokenValue);
                        }
                        break;
                    case "ffhps":
                        if (m_currentFormField != null && tokenValue != null && tokenValue != string.Empty && m_currentFormField.CheckboxSizeType == CheckBoxSizeType.Exactly)
                        {
                            m_currentFormField.CheckboxSize = Convert.ToInt32(tokenValue) / 2;
                        }
                        break;
                    case "ffres":
                        if (m_currentFormField != null && tokenValue != null && tokenValue != string.Empty)
                        {
                            m_currentFormField.Ffres = Convert.ToInt32(tokenValue);
                        }
                        break;
                    case "ffdefres":
                        if (m_currentFormField != null && tokenValue != null && tokenValue != string.Empty)
                        {
                            m_currentFormField.Ffdefres = Convert.ToInt32(tokenValue);
                        }
                        break;
                    default:
                        ParseSpecialCharacters(token);
                        break;
                }
            }
        }
       /// <summary>
       /// Reset the paragraph formatting
       /// </summary>
        private void ResetParagraphFormat()
        {
            //Reset the paragraph format
            WParagraphFormat sourceParaFormat = CurrentPara.ParagraphFormat;
            if (!sourceParaFormat.HasKey(WParagraphFormat.BackColorKey))
                sourceParaFormat.BackColor = Color.Empty;
            if (!sourceParaFormat.HasKey(WParagraphFormat.BeforeSpacingKey))
                sourceParaFormat.BeforeSpacing = 0;
            if (!sourceParaFormat.HasKey(WParagraphFormat.AfterSpacingKey))
                sourceParaFormat.AfterSpacing = 0;
            if (!sourceParaFormat.HasKey(WParagraphFormat.BidiKey))
                sourceParaFormat.Bidi = false;
            if (!sourceParaFormat.HasKey(WParagraphFormat.ColumnBreakAfterKey))
                sourceParaFormat.ColumnBreakAfter = false;
            if (!sourceParaFormat.HasKey(WParagraphFormat.ContextualSpacingKey))
                sourceParaFormat.ContextualSpacing = false;
            if (!sourceParaFormat.HasKey(WParagraphFormat.FirstLineIndentKey))
                sourceParaFormat.FirstLineIndent = 0;
            if (!sourceParaFormat.HasKey(WParagraphFormat.FirstLineIndentBiKey))
                sourceParaFormat.FirstLineIndentBi = 0;
            if (!sourceParaFormat.HasKey(WParagraphFormat.ForeColorKey))
                sourceParaFormat.ForeColor = Color.Empty;
            if (!sourceParaFormat.HasKey(WParagraphFormat.HrAlignmentKey))
                sourceParaFormat.HorizontalAlignment = HorizontalAlignment.Left;
            if (!sourceParaFormat.HasKey(WParagraphFormat.KeepKey))
                sourceParaFormat.Keep = false;
            if (!sourceParaFormat.HasKey(WParagraphFormat.KeepFollowKey))
                sourceParaFormat.KeepFollow = false;
            if (!sourceParaFormat.HasKey(WParagraphFormat.LeftIndentKey))
                sourceParaFormat.LeftIndent = 0;
            if (!sourceParaFormat.HasKey(WParagraphFormat.LineSpacingKey))
                sourceParaFormat.LineSpacing = 12.0f;
            if (!sourceParaFormat.HasKey(WParagraphFormat.LineSpacingRuleKey))
                sourceParaFormat.LineSpacingRule = LineSpacingRule.Multiple;
            if (!sourceParaFormat.HasKey(WParagraphFormat.OutlineLevelKey))
                sourceParaFormat.OutlineLevel = OutlineLevel.BodyText;
            if (!sourceParaFormat.HasKey(WParagraphFormat.PageBreakAfterKey))
                sourceParaFormat.PageBreakAfter = false;
            if (!sourceParaFormat.HasKey(WParagraphFormat.PageBreakBeforeKey))
                sourceParaFormat.PageBreakBefore = false;
            if (!sourceParaFormat.HasKey(WParagraphFormat.RightIndentKey))
                sourceParaFormat.RightIndent = 0;
            if (!sourceParaFormat.HasKey(WParagraphFormat.SpacingAfterAutoKey))
                sourceParaFormat.SpaceAfterAuto = false;
            if (!sourceParaFormat.HasKey(WParagraphFormat.SpacingBeforeAutoKey))
                sourceParaFormat.SpaceBeforeAuto = false;
            if (!sourceParaFormat.HasKey(WParagraphFormat.TextureStyleKey))
                sourceParaFormat.TextureStyle = TextureStyle.TextureNone;
            if (!sourceParaFormat.HasKey(WParagraphFormat.WidowControlKey))
                sourceParaFormat.WidowControl = false;
            if (!sourceParaFormat.HasKey(WParagraphFormat.WordWrapKey))
                sourceParaFormat.WordWrap = true;
            //Reset Border
            ResetBorders(sourceParaFormat);
        }
       /// <summary>
       /// Reset Borders
       /// </summary>
       /// <param name="sourceParaFormat"></param>
        private void ResetBorders(WParagraphFormat sourceParaFormat)
        {
            if (!sourceParaFormat.HasValue(WParagraphFormat.TopBorderKey) &&
                 !sourceParaFormat.HasValue(WParagraphFormat.TopBorderNewKey))
            {
                ResetBorder(sourceParaFormat.Borders.Top);
            }

            if (!sourceParaFormat.HasValue(WParagraphFormat.LeftBorderKey) &&
              !sourceParaFormat.HasValue(WParagraphFormat.LeftBorderNewKey))
            {
                ResetBorder(sourceParaFormat.Borders.Left);
            }

            if (!sourceParaFormat.HasValue(WParagraphFormat.BottomBorderKey) &&
              !sourceParaFormat.HasValue(WParagraphFormat.BottomBorderNewKey))
            {
                ResetBorder(sourceParaFormat.Borders.Bottom);
            }

            if (!sourceParaFormat.HasValue(WParagraphFormat.RightBorderKey) &&
              !sourceParaFormat.HasValue(WParagraphFormat.RightBorderNewKey))
            {
                ResetBorder(sourceParaFormat.Borders.Right);
            }
        }
       /// <summary>
       /// Reset Border
       /// </summary>
       /// <param name="border"></param>
        private void ResetBorder(Border border)
        {
            border.LineWidth = 0;
            border.Color = Color.Empty;
            border.BorderType = BorderStyle.None;
            border.Shadow = false;
            border.Space = 0;
        }
       /// <summary>
       /// Reset the character format
       /// </summary>
        private void ResetCharacterFormat()
        {
            WCharacterFormat sourceFormat = CurrentPara.BreakCharacterFormat;
            if (sourceFormat.FontSize != WCharacterFormat.DEF_FONTSIZE && !sourceFormat.HasKey(WCharacterFormat.FontSizeKey))
                sourceFormat.FontSize = WCharacterFormat.DEF_FONTSIZE;
            if (sourceFormat.TextColor != Color.Empty && !sourceFormat.HasKey(WCharacterFormat.TextColorKey))
                sourceFormat.TextColor = Color.Empty;
            if (sourceFormat.FontName != WCharacterFormat.DEF_FONTFAMILY && !sourceFormat.HasKey(WCharacterFormat.FontNameKey))
                sourceFormat.FontName = WCharacterFormat.DEF_FONTFAMILY;
            if (sourceFormat.Bold && !sourceFormat.HasKey(WCharacterFormat.BoldKey))
                sourceFormat.Bold = false;
            if (sourceFormat.Italic && !sourceFormat.HasKey(WCharacterFormat.ItalicKey))
                sourceFormat.Italic = false;
            if (sourceFormat.UnderlineStyle != UnderlineStyle.None && !sourceFormat.HasKey(WCharacterFormat.UnderlineKey))
                sourceFormat.UnderlineStyle = UnderlineStyle.None;
            if (sourceFormat.HighlightColor != Color.Empty && !sourceFormat.HasKey(WCharacterFormat.HighlightColorKey))
                sourceFormat.HighlightColor = Color.Empty;
            if (sourceFormat.Shadow && !sourceFormat.HasKey(WCharacterFormat.ShadowKey))
                sourceFormat.Shadow = false;
            if (sourceFormat.CharacterSpacing != 0 && !sourceFormat.HasKey(WCharacterFormat.SpacingKey))
                sourceFormat.CharacterSpacing = 0;
            if (sourceFormat.DoubleStrike && !sourceFormat.HasKey(WCharacterFormat.DoubleStrikeKey))
                sourceFormat.DoubleStrike = false;
            if (sourceFormat.Emboss && !sourceFormat.HasKey(WCharacterFormat.EmbossKey))
                sourceFormat.Emboss = false;
            if (sourceFormat.Engrave && !sourceFormat.HasKey(WCharacterFormat.EngraveKey))
                sourceFormat.Engrave = false;
            if (sourceFormat.SubSuperScript != SubSuperScript.None && !sourceFormat.HasKey(WCharacterFormat.SubSuperScriptKey))
                sourceFormat.SubSuperScript = SubSuperScript.None;
            if (sourceFormat.TextBackgroundColor != Color.Empty && !sourceFormat.HasKey(WCharacterFormat.TextBkgColorKey))
                sourceFormat.TextBackgroundColor = Color.Empty;
            if (sourceFormat.AllCaps && !sourceFormat.HasKey(WCharacterFormat.AllCapsKey))
                sourceFormat.AllCaps = false;
            if (sourceFormat.BoldBidi && !sourceFormat.HasKey(WCharacterFormat.BoldBidiKey))
                sourceFormat.BoldBidi = false;
            if (sourceFormat.FieldVanish && !sourceFormat.HasKey(WCharacterFormat.FieldVanishKey))
                sourceFormat.FieldVanish = false;
            if (sourceFormat.Hidden && !sourceFormat.HasKey(WCharacterFormat.HiddenKey))
                sourceFormat.Hidden = false;
            if (sourceFormat.SmallCaps && !sourceFormat.HasKey(WCharacterFormat.SmallCapsKey))
                sourceFormat.SmallCaps = false;
        }
        /// <summary>
        /// Get equal column width of the section
        /// </summary>
        /// <param name="columnCount"></param>
        /// <returns></returns>
        private float GetEqualColumnWidth(int columnCount)
        {
            float totalWidthIncludingSpacing = m_secFormat.PageSize.Width - (m_secFormat.LeftMargin + m_secFormat.RightMargin);
            float totalWidthExcludingSpacing = totalWidthIncludingSpacing - (36 * (columnCount - 1));
            float actualWidth = totalWidthExcludingSpacing / columnCount;
            return actualWidth;
        }
        /// <summary>
        /// Parse special characters
        /// </summary>
        /// <param name="token"></param>
        private void ParseSpecialCharacters(string token)
        {
            string specialCharacter = null;
            if ((!IsDestinationControlWord || IsFieldGroup))
            {
                if (token.StartsWith("'") && !IsAccentCharacterNeedToBeOmitted())
                {
                    m_bIsAccentChar = true;
                    specialCharacter = GetAccentCharacter(token);
                }
                else if (token.StartsWith("_"))
                {
                    specialCharacter = token.Replace("_", SpecialCharacters.NonBreakingHyphen.ToString());
                }
                else if (token.StartsWith("~"))
                {
                    specialCharacter = token.Replace("~", SpecialCharacters.NonBreakingSpace.ToString());
                }
                else if (token.StartsWith("-"))
                {
                    specialCharacter = token;
                }
                else if (token.StartsWith(":"))
                {
                    specialCharacter = token;
                }
            }

            if (specialCharacter != null)
                ParseDocumentElement(specialCharacter);

        }
        /// <summary>
        /// Check whether the accent character need to be omitted
        /// </summary>
        /// <returns></returns>
        private bool IsAccentCharacterNeedToBeOmitted()
        {
            if (m_unicodeCount > 0)
            {
                if (--m_unicodeCount >= 0)
                {
                    return true;
                }
            }
            return false;
        }
       /// <summary>
       /// Get Texture style
       /// </summary>
       /// <param name="textureValue"></param>
       /// <returns></returns>
        private TextureStyle GetTextureStyle(int textureValue)
        {
            switch (textureValue)
            {
                case 500:
                    return TextureStyle.Texture5Percent;
                case 250:
                    return TextureStyle.Texture2Pt5Percent;
                case 750:
                    return TextureStyle.Texture7Pt5Percent;
                case 1000:
                    return TextureStyle.Texture10Percent;
                case 1250:
                    return TextureStyle.Texture12Pt5Percent;
                case 1500:
                    return TextureStyle.Texture15Percent;
                case 1750:
                    return TextureStyle.Texture17Pt5Percent;
                case 2000:
                    return TextureStyle.Texture20Percent;
                case 2250:
                    return TextureStyle.Texture22Pt5Percent;
                case 2500:
                    return TextureStyle.Texture25Percent;
                case 2750:
                    return TextureStyle.Texture27Pt5Percent;
                case 3000:
                    return TextureStyle.Texture30Percent;
                case 3250:
                    return TextureStyle.Texture32Pt5Percent;
                case 3500:
                    return TextureStyle.Texture35Percent;
                case 3750:
                    return TextureStyle.Texture37Pt5Percent;
                case 4000:
                    return TextureStyle.Texture40Percent;
                case 4250:
                    return TextureStyle.Texture42Pt5Percent;
                case 4500:
                    return TextureStyle.Texture45Percent;
                case 4750:
                    return TextureStyle.Texture47Pt5Percent;
                case 5000:
                    return TextureStyle.Texture50Percent;
                case 5250:
                    return TextureStyle.Texture52Pt5Percent;
                case 5500:
                    return TextureStyle.Texture55Percent;
                case 5750:
                    return TextureStyle.Texture57Pt5Percent;
                case 6000:
                    return TextureStyle.Texture60Percent;
                case 6250:
                    return TextureStyle.Texture62Pt5Percent;
                case 6500:
                    return TextureStyle.Texture65Percent;
                case 6750:
                    return TextureStyle.Texture67Pt5Percent;
                case 7000:
                    return TextureStyle.Texture70Percent;
                case 7250:
                    return TextureStyle.Texture72Pt5Percent;
                case 7500:
                    return TextureStyle.Texture75Percent;
                case 7750:
                    return TextureStyle.Texture77Pt5Percent;
                case 8000:
                    return TextureStyle.Texture80Percent;
                case 8250:
                    return TextureStyle.Texture82Pt5Percent;
                case 8500:
                    return TextureStyle.Texture85Percent;
                case 8750:
                    return TextureStyle.Texture87Pt5Percent;
                case 9000:
                    return TextureStyle.Texture90Percent;
                case 9250:
                    return TextureStyle.Texture92Pt5Percent;
                case 9500:
                    return TextureStyle.Texture95Percent;
                case 9750:
                    return TextureStyle.Texture97Pt5Percent;
                default :
                    return TextureStyle.TextureNone;
            }
        }
       /// <summary>
       /// Parse listtext start
       /// </summary>
        private void ParseListTextStart()
        {           
            m_stack.Push(c_groupStart);
            if (m_bIsList)
            {
                CurrentPara.ListFormat.ContinueListNumbering();
                if (m_prevFormat != null)
                    CopyParagraphFormatting(m_prevFormat, CurrentPara.ParagraphFormat);
            }
            m_prevParagraph = CurrentPara.Clone () as WParagraph ;
            m_prevTextFormat = m_currTextFormat.Clone ();
            m_currTextFormat.Underline = ThreeState.Unknown;
            m_currTextFormat.UnderlineStyle = UnderlineStyle.None;
            CurrentPara = new WParagraph(m_document);
            m_listLevelCharFormat = new WCharacterFormat(m_document);
            m_listLevelParaFormat = new WParagraphFormat(m_document);
            m_bIsListText = true;

        }
       /// <summary>
       /// Parse paragraph end
       /// </summary>
        private void ParseParagraphEnd()
        {
            if (CurrentPara.ListFormat.CurrentListStyle != null && m_bIsList)
            {
                if (m_listLevelParaFormat != null)
                    CopyParagraphFormatting(m_listLevelParaFormat, CurrentPara.ListFormat.CurrentListLevel.ParagraphFormat);
                if (m_listLevelCharFormat != null)
                    CopyCharacterFormatting(m_listLevelCharFormat, CurrentPara.ListFormat.CurrentListLevel.CharacterFormat);
            }
           
            m_tabCount = 0;
            m_prevFormat = CurrentPara.ParagraphFormat;
            IWParagraph prevPara = CurrentPara;
            if (IsFieldGroup || 
                (!IsDestinationControlWord && m_previousToken != "nonesttables"))
            {
                if (m_previousToken == "row" && m_previousLevel == m_currentLevel && m_bInTable && m_currentLevel <= 1)
                {
                    m_bInTable = false;
                    m_currentLevel = 0;
                }
                ProcessTableInfo();
                AddNewParagraph(CurrentPara);           
                m_currParagraph = new WParagraph(m_document);
                if (prevPara != null && prevPara.StyleName != null)
                {
                    if (!m_bIsListText)
                    {
                        // Only handled properties are set here, as of now
                        CurrentPara.ParagraphFormat.SetDefaultProperties();
                        CurrentPara.BreakCharacterFormat.SetDefaultProperties();
                    }
                    m_currParagraph.ApplyStyle(prevPara.StyleName);
                }
                CopyParagraphFormatting(m_prevFormat, CurrentPara.ParagraphFormat);
                m_currParagraph.ParagraphFormat.Tabs.Clear();
            }

        }
       /// <summary>
       /// Parse paragraph start
       /// </summary>
        private void ParseParagraphStart()
        {
            m_bIsLinespacingRule = false;
            m_tabCount = 0;
            m_tabCollection.Clear();
            CurrTabFormat = new TabFormat();
            m_bInTable = false;
            if (m_bIsList)
                m_bIsPreviousList = true;
            else
                m_bIsPreviousList = false;
            m_bIsList = false;
            if (CurrentPara.Items.Count > 0)
            {
                ParagraphItemCollection itemCollection = new ParagraphItemCollection(m_document);
                CurrentPara.Items.CloneItemsTo(itemCollection);
                CurrentPara = new WParagraph(m_document);
                foreach (Entity ent in itemCollection)
                {
                    CurrentPara.Items.Add(ent);
                }
            }
            else
            {
                CurrentPara = new WParagraph(m_document);
            }
          
            m_bIsBorderTop = false;
            m_bIsBorderBottom = false;
            m_bIsBorderLeft = false;
            m_bIsBorderRight = false;
            m_currentLevel = 0;
        }
       /// <summary>
       /// Parse section start
       /// </summary>
        private void ParseSectionStart()
        {
            if (m_bIsDefaultSectionFormat == false)
            {
                m_bIsDefaultSectionFormat = true;
                m_defaultSectionFormat = new SecionFormat();
                CopySectionFormat(m_secFormat, m_defaultSectionFormat);
            }  
            if (!m_bIsHeader && !m_bIsFooter && !m_bIsRow)
            {
                if (m_previousToken == "sect")
                {
                    m_secCount++;                                   
                    m_currSection = new WSection(m_document);
                    m_textBody = m_currSection.Body;
                    m_secFormat = new SecionFormat();
                    if (m_bIsDefaultSectionFormat)
                    {
                        CopySectionFormat(m_defaultSectionFormat, m_secFormat);
                    }
                    CurrColumn = null;
                }
                else
                {
                    CurrColumn = null;
                    CurrentSection.Columns.InnerList.Clear();
                    CurrentSection.BreakCode = SectionBreakCode.NewPage; 
                }
                CurrentSection.PageSetup.EqualColumnWidth = true;
            }
        }
       /// <summary>
       /// Copy Section formatting
       /// </summary>
       /// <param name="sourceFormat"></param>
       /// <param name="destFormat"></param>
        private void CopySectionFormat(SecionFormat sourceFormat, SecionFormat destFormat)
        {
            destFormat.BottomMargin = sourceFormat.BottomMargin;
            destFormat.DefaultTabWidth = sourceFormat.DefaultTabWidth;
            destFormat.DifferentFirstPage = sourceFormat.DifferentFirstPage;
            destFormat.DifferentOddAndEvenPage = sourceFormat.DifferentOddAndEvenPage;
            destFormat.FooterDistance = sourceFormat.FooterDistance;
            destFormat.HeaderDistance = sourceFormat.HeaderDistance;
            destFormat.IsFrontPageBorder = sourceFormat.IsFrontPageBorder;
            destFormat.LeftMargin = sourceFormat.LeftMargin;
            destFormat.PageOrientation = sourceFormat.PageOrientation;
            destFormat.PageSize = sourceFormat.PageSize;
            destFormat.RightMargin = sourceFormat.RightMargin;
            destFormat.TopMargin = sourceFormat.TopMargin;
            destFormat.VertAlignment = sourceFormat.VertAlignment;
        }
       /// <summary>
       /// Parse row Start
       /// </summary>
        private void ParseRowStart()
        {
            m_bIsRow = true;
            m_bInTable = true;
            m_currCellIndex = -1;
            m_currCellFormatIndex = -1;
            CurrCellFormat = new CellFormat();
            if (m_currentLevel <= 1)
            {
                m_CellFormatStack.Clear();
                m_currRowFormatStack.Clear();
            }
            m_cellFormatTable = new Dictionary<int, CellFormat>();
            m_CellFormatStack.Push(m_cellFormatTable);
            CurrRowFormat = new RowFormat(m_document);
            m_currRowFormatStack.Push(CurrRowFormat);
        }
       /// <summary>
       /// Parse Row End
       /// </summary>
        private void ParseRowEnd()
        {
            m_bIsRow = false;
            int cellIndex = 0;
            m_prevCellFormatStack = new Stack<Dictionary<int, CellFormat>>(m_CellFormatStack.ToArray());
            m_prevRowFormatStack = new Stack<RowFormat>(m_currRowFormatStack.ToArray());
            if (m_currRowFormatStack.Count > 0)
                CurrRowFormat = m_currRowFormatStack.Pop();
            if (m_currTable != null)
            {
                ApplyRowFormatting(m_currTable.LastRow, CurrRowFormat);
                CopyTextFormatToCharFormat(m_currTable.LastRow.CharacterFormat, m_currTextFormat);
            }
            m_cellFormatTable = m_CellFormatStack.Pop();
            foreach (KeyValuePair<int, CellFormat> cellFormatTable in m_cellFormatTable)
            {
                if (m_currTable != null)
                    ApplyCellFormatting(m_currTable.LastRow.Cells[cellIndex], cellFormatTable.Value);
                cellIndex++;
            }
            CurrRowFormat = new RowFormat(m_document);
            if (m_currParagraph != null)
                SetDefaultValue(m_currParagraph.ParagraphFormat);
            m_currCellFormatIndex = -1;
            m_currRow = null;
            m_leftcellspace = 0;
            m_rightcellspace = 0;
            m_bottomcellspace = 0;
            m_topcellspace = 0;
            //Copy previous cell format stack to current cell format stack
            m_CellFormatStack = new Stack<Dictionary<int, CellFormat>>(m_prevCellFormatStack.ToArray());
            //Copy previous rowformat stack to current row format stack
            m_currRowFormatStack =new Stack<RowFormat> (m_prevRowFormatStack .ToArray ());
            if (m_currentLevel > 1)
            {
                m_currRowFormatStack.Pop();
                m_CellFormatStack.Pop();
                m_currentLevel = 0;
            }
           
        }
       /// <summary>
       /// Parse Cell boundary
       /// </summary>
       /// <param name="token"></param>
       /// <param name="tokenKey"></param>
       /// <param name="tokenValue"></param>
        private void ParseCellBoundary(string token, string tokenKey, string tokenValue)
        {
            int prevCellBoundary = m_currCellBoundary;
            if (tokenKey.EndsWith("-"))
                m_currCellBoundary = -Convert.ToInt32(tokenValue);
            else
                m_currCellBoundary = Convert.ToInt32(tokenValue);
            m_currCellFormatIndex++;
            m_cellFormatTable = m_CellFormatStack.Pop();
            //Calculating cell width from the cell boundary if cell width is not explicitly specified
            if (m_cellFormatTable.Count > 0)
                CurrCellFormat.CellWidth = ExtractTwipsValue((m_currCellBoundary - prevCellBoundary).ToString());
            else
            {
                int tableIndent = 0;
                float paddingLeft = (CurrCellFormat.SamePaddingsAsTable) ? CurrRowFormat.Paddings.Left : CurrCellFormat.Paddings.Left;
                if (m_currRowLeftIndent != 0 && !GetTextWrapAround(CurrRowFormat.Positioning) && CurrRowFormat.HorizontalAlignment == RowAlignment.Left)
                {
                    tableIndent = (int)m_currRowLeftIndent;
                    tableIndent -= (paddingLeft != 0) ? (int)Math.Round(paddingLeft * DLSConstants.TwipsInOnePoint) : 5;
                   
                }
                else
                {
                    float leftIndent = -paddingLeft;
                    tableIndent = (int)Math.Round(leftIndent * DLSConstants.TwipsInOnePoint);
                    tableIndent = (paddingLeft != 0) ? tableIndent : -5;
                }
                if (CurrRowFormat.GridBeforeWidth.Width > 0)
                {
                    tableIndent += (int)Math.Round((CurrRowFormat.GridBeforeWidth.WidthType == FtsWidth.Point) ? (CurrRowFormat.GridBeforeWidth.Width * DLSConstants.TwipsInOnePoint) : ((CurrRowFormat.GridBeforeWidth.WidthType == FtsWidth.Percentage) ? (CurrRowFormat.GridBeforeWidth.Width * DLSConstants.PercentageFactor) : 0));
                }
                CurrCellFormat.CellWidth = ExtractTwipsValue((m_currCellBoundary - tableIndent).ToString());
            }
            
            m_cellFormatTable.Add(m_currCellFormatIndex, CurrCellFormat);
            m_CellFormatStack.Push(m_cellFormatTable);
            CurrCellFormat = new CellFormat();
            m_bIsBorderTop = false;
            m_bIsBorderRight = false;
            m_bIsBorderLeft = false;
            m_bIsBorderBottom = false;
        }
        /// <summary>
        /// Gets the text wrap around.
        /// </summary>
        /// <param name="positioning">The positioning.</param>
        /// <returns></returns>
        private bool GetTextWrapAround(RowFormat.TablePositioning positioning)
        {
            if (positioning.HasValue(RowFormat.TablePositioning.HorizRelKey))
                return true;
            else if (positioning.HasValue(RowFormat.TablePositioning.HorizPosKey))
                return positioning.HorizPosition != 0;
            else if (positioning.HasValue(RowFormat.TablePositioning.VertRelKey))
            {
                if (positioning.VertRelationTo == VerticalRelation.Paragraph)
                    return true;
                else if (positioning.HasValue(RowFormat.TablePositioning.VertPosKey))
                    return positioning.VertPosition != 0;
            }
            else if (positioning.HasValue(RowFormat.TablePositioning.VertPosKey))
                return positioning.VertPosition != 0;
            return false;
        }
        /// <summary>
        /// Parses Accented character
        /// </summary>
        /// <param name="token"></param>
        /// <param name="tokenKey"></param>
        /// <param name="tokenValue"></param>
        private string GetAccentCharacter(string token)
        {
            int intValue = 0;
            string text = "";
            int totalStringLength = token.Length;
            string hexString = token.Substring(1, 2);
            if (hexString != "3f")
            {
                intValue = int.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
#if !(SILVERLIGHT || WP) || WINRT
                System.Text.Encoding enc = System.Text.UnicodeEncoding.GetEncoding(GetCodePage());
                //to handle the double-byte character set encoding
                if (!IsSingleByte())
                {
                    //read the next token
                    string nextToken = m_lexer.ReadNextToken(m_previousTokenKey);
                    string[] value = SeperateToken(nextToken);
                    m_previousTokenKey = value[0];
                    m_previousTokenValue = value[1];
                    nextToken = nextToken.Trim();
                    nextToken = nextToken.Substring(1);
                    hexString = nextToken.Substring(1, 2) + hexString;
                    intValue = int.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
                }
#else
                System.Text.Encoding enc = new Windows1252Encoding();
#endif
                byte[] buffer = BitConverter.GetBytes((short)intValue);
                text = enc.GetString(buffer, 0, buffer.Length);
                text = text.Replace("\0", "");
            }
            else if (m_previousToken.StartsWith("u"))
            {
                text = ((char)(Convert.ToInt32(m_previousTokenValue))).ToString();
            }
            if (totalStringLength > 3)
            {
                text += token.Substring(3, totalStringLength - 3);
            }
            return text;
        }
       /// <summary>
        /// Get the code page for the current Font character set 
       /// </summary>
       /// <returns></returns>
        private string GetCodePage()
        {
            #region GetCodePage
            switch (CurrRtfFont.FontCharSet)
            {

                case 0:
                case 1:
                    return "Windows-1252";
                case 77:
                    return "macintosh";
                case 78:
                    return "x-mac-japanese";
                case 79:
                    return "x-mac-korean";
                case 80:
                    return "x-mac-chinesesimp";
                case 81:
                case 82:
                    return "x-mac-chinesetrad";
                case 83:
                    return "x-mac-hebrew";
                case 84:
                    return "x-mac-arabic";
                case 85:
                    return "x-mac-greek";
                case 86:
                    return "x-mac-turkish";
                case 87:
                    return "x-mac-thai";
                case 88:
                    return "x-mac-ce";
                case 89:
                    return "x-mac-cyrillic";
                case 128:
                    return "shift_jis";
                case 129:
                    return "ks_c_5601-1987";
                case 130:
                    return "Johab";
                case 134:
                    return "gb2312";
                case 136:
                    return "big5";
                case 161:
                    return "windows-1253";
                case 162:
                    return "windows-1254";
                case 163:
                    return "windows-1258";
                case 177:
                    return "windows-1255";
                case 178:
                case 179:
                case 180:
                case 181:
                    return "windows-1256";
                case 186:
                    return "windows-1257";
                case 204:
                    return "windows-1251";
                case 222:
                    return "windows-874";
                case 238:
                    return "windows-1250";
                case 254:
                    return "IBM437";
                case 255:
                    return "ibm850";
                default:
                    return DefaultCodePage;
            }
            #endregion
        }
        /// <summary>
        /// Determines whether current code page is single byte encoding.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is single byte]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsSingleByte()
        {
            switch (CurrRtfFont.FontCharSet)
            {
                // character set x-mac-japanese, x-mac-korean, x-mac-chineseimp, x-mac-chinesetrad,
                //shift_jis, ks_c_5601_1987, johab, gb2312, big5 are double byte character set encoding
                case 78:
                case 79:
                case 80:
                case 81:
                case 82:
                case 128:
                case 129:
                case 130:
                case 134:
                case 136:
                    return false;
                default:
                    return true;
            }
        }
       /// <summary>
       /// Determine Whether the code page is supported for Encoding
       /// </summary>
       /// <param name="codePage"></param>
       /// <returns></returns>
        private bool IsSupportedCodePage(int codePage)
        {
            switch (codePage)
            {
                #region Supported Code Pages
                case 37:
                case 437:
                case 500:
                case 708:
                case 720:
                case 737:
                case 775:
                case 850:
                case 852:
                case 855:
                case 857:
                case 858:
                case 860:
                case 861:
                case 862:
                case 863:
                case 864:
                case 865:
                case 866:
                case 869:
                case 870:
                case 874:
                case 875:
                case 932:
                case 936:
                case 949:
                case 950:
                case 1026:
                case 1047:
                case 1140:
                case 1141:
                case 1142:
                case 1143:
                case 1144:
                case 1145:
                case 1146:
                case 1147:
                case 1148:
                case 1149:
                case 1200:
                case 1201:
                case 1250:
                case 1251:
                case 1252:
                case 1253:
                case 1254:
                case 1255:
                case 1256:
                case 1257:
                case 1258:
                case 1361:
                case 10000:
                case 10001:
                case 10002:
                case 10003:
                case 10004:
                case 10005:
                case 10006:
                case 10007:
                case 10008:
                case 10010:
                case 10017:
                case 10021:
                case 10029:
                case 10079:
                case 10081:
                case 10082:
                case 12000:
                case 12001:
                case 20000:
                case 20001:
                case 20002:
                case 20003:
                case 20004:
                case 20005:
                case 20105:
                case 20106:
                case 20107:
                case 20108:
                case 20127:
                case 20261:
                case 20269:
                case 20273:
                case 20277:
                case 20278:
                case 20280:
                case 20284:
                case 20285:
                case 20290:
                case 20297:
                case 20420:
                case 20423:
                case 20424:
                case 20833:
                case 20838:
                case 20866:
                case 20871:
                case 20880:
                case 20905:
                case 20924:
                case 20932:
                case 20936:
                case 20949:
                case 21025:
                case 21866:
                case 28591:
                case 28592:
                case 28593:
                case 28594:
                case 28595:
                case 28596:
                case 28597:
                case 28598:
                case 28599:
                case 28603:
                case 28605:
                case 29001:
                case 38598:
                case 50220:
                case 50221:
                case 50222:
                case 50225:
                case 50227:
                case 51932:
                case 51936:
                case 51949:
                case 52936:
                case 54936:
                case 57002:
                case 57003:
                case 57004:
                case 57005:
                case 57006:
                case 57007:
                case 57008:
                case 57009:
                case 57010:
                case 57011:
                case 65000:
                case 65001:
                    return true;
                #endregion
                default:
                    return false;
            }
        }
        /// <summary>
        /// Get the code page is supported for Encoding
        /// </summary>
        /// <param name="codePage"></param>
        /// <returns></returns>
        private string GetSupportedCodePage(int codePage)
        {
            switch (codePage)
            {
                #region Supported Code Pages
                case 37:
                    return "IBM037";
                case 437:
                    return "IBM437";
                case 500:
                    return "IBM500";
                case 708:
                    return "ASMO-708";
                case 720:
                    return "DOS-720";
                case 737:
                    return "ibm737";
                case 775:
                    return "ibm775";
                case 850:
                    return "ibm850";
                case 852:
                    return "ibm852";
                case 855:
                    return "IBM855";
                case 857:
                    return "ibm857";
                case 858:
                    return "IBM00858";
                case 860:
                    return "IBM860";
                case 861:
                    return "ibm861";
                case 862:
                    return "DOS-862";
                case 863:
                    return "IBM863";
                case 864:
                    return "IBM864";
                case 865:
                    return "IBM865";
                case 866:
                    return "cp866";
                case 869:
                    return "ibm869";
                case 870:
                    return "IBM870";
                case 874:
                    return "windows-874";
                case 875:
                    return "cp875";
                case 932:
                    return "shift_jis";
                case 936:
                    return "gb2312";
                case 949:
                    return "ks_c_5601-1987";
                case 950:
                    return "big5";
                case 1026:
                    return "IBM1026";
                case 1047:
                    return "IBM01047";
                case 1140:
                    return "IBM01140";
                case 1141:
                    return "IBM01141";
                case 1142:
                    return "IBM01142";
                case 1143:
                    return "IBM01143";
                case 1144:
                    return "IBM01144";
                case 1145:
                    return "IBM01145";
                case 1146:
                    return "IBM01146";
                case 1147:
                    return "IBM01147";
                case 1148:
                    return "IBM01148";
                case 1149:
                    return "IBM01149";
                case 1200:
                    return "utf-16";
                case 1201:
                    return "unicodeFFFE";
                case 1250:
                    return "windows-1250";
                case 1251:
                    return "windows-1251";
                case 1252:
                    return "windows-1252";
                case 1253:
                    return "windows-1253";
                case 1254:
                    return "windows-1254";
                case 1255:
                    return "windows-1255";
                case 1256:
                    return "windows-1256";
                case 1257:
                    return "windows-1257";
                case 1258:
                    return "windows-1258";
                case 1361:
                    return "Johab";
                case 10000:
                    return "macintosh";
                case 10001:
                    return "x-mac-japanese";
                case 10002:
                    return "x-mac-chinesetrad";
                case 10003:
                    return "x-mac-korean";
                case 10004:
                    return "x-mac-arabic";
                case 10005:
                    return "x-mac-hebrew";
                case 10006:
                    return "x-mac-greek";
                case 10007:
                    return "x-mac-cyrillic";
                case 10008:
                    return "x-mac-chinesesimp";
                case 10010:
                    return "x-mac-romanian";
                case 10017:
                    return "x-mac-ukrainian";
                case 10021:
                    return "x-mac-thai";
                case 10029:
                    return "x-mac-ce";
                case 10079:
                    return "x-mac-icelandic";
                case 10081:
                    return "x-mac-turkish";
                case 10082:
                    return "x-mac-croatian";
                case 12000:
                    return "utf-32";
                case 12001:
                    return "utf-32BE";
                case 20000:
                    return "x-Chinese_CNS";
                case 20001:
                    return "x-cp20001";
                case 20002:
                    return "x_Chinese-Eten";
                case 20003:
                    return "x-cp20003";
                case 20004:
                    return "x-cp20004";
                case 20005:
                    return "x-cp20005";
                case 20105:
                    return "x-IA5";
                case 20106:
                    return "x-IA5-German";
                case 20107:
                    return "x-IA5-Swedish";
                case 20108:
                    return "x-IA5-Norwegian";
                case 20127:
                    return "us-ascii";
                case 20261:
                    return "x-cp20261";
                case 20269:
                    return "x-cp20269";
                case 20273:
                    return "IBM273";
                case 20277:
                    return "IBM277";
                case 20278:
                    return "IBM278";
                case 20280:
                    return "IBM280";
                case 20284:
                    return "IBM284";
                case 20285:
                    return "IBM285";
                case 20290:
                    return "IBM290";
                case 20297:
                    return "IBM297";
                case 20420:
                    return "IBM420";
                case 20423:
                    return "IBM423";
                case 20424:
                    return "IBM424";
                case 20833:
                    return "x-EBCDIC-KoreanExtended";
                case 20838:
                    return "IBM-Thai";
                case 20866:
                    return "koi8-r";
                case 20871:
                    return "IBM871";
                case 20880:
                    return "IBM880";
                case 20905:
                    return "IBM905";
                case 20924:
                    return "IBM00924";
                case 20932:
                    return "EUC-JP";
                case 20936:
                    return "x-cp20936";
                case 20949:
                    return "x-cp20949";
                case 21025:
                    return "cp1025";
                case 21866:
                    return "koi8-u";
                case 28591:
                    return "iso-8859-1";
                case 28592:
                    return "iso-8859-2";
                case 28593:
                    return "iso-8859-3";
                case 28594:
                    return "iso-8859-4";
                case 28595:
                    return "iso-8859-5";
                case 28596:
                    return "iso-8859-6";
                case 28597:
                    return "iso-8859-7";
                case 28598:
                    return "iso-8859-8";
                case 28599:
                    return "iso-8859-9";
                case 28603:
                    return "iso-8859-13";
                case 28605:
                    return "iso-8859-15";
                case 29001:
                    return "x-Europa";
                case 38598:
                    return "iso-8859-8-i";
                case 50220:
                    return "iso-2022-jp";
                case 50221:
                    return "csISO2022JP";
                case 50222:
                    return "iso-2022-jp";
                case 50225:
                    return "iso-2022-kr";
                case 50227:
                    return "x-cp50227";
                case 51932:
                    return "euc-jp";
                case 51936:
                    return "EUC-CN";
                case 51949:
                    return "euc-kr";
                case 52936:
                    return "hz-gb-2312";
                case 54936:
                    return "GB18030";
                case 57002:
                    return "x-iscii-de";
                case 57003:
                    return "x-iscii-be";
                case 57004:
                    return "x-iscii-ta";
                case 57005:
                    return "x-iscii-te";
                case 57006:
                    return "x-iscii-as";
                case 57007:
                    return "x-iscii-or";
                case 57008:
                    return "x-iscii-ka";
                case 57009:
                    return "x-iscii-ma";
                case 57010:
                    return "x-iscii-gu";
                case 57011:
                    return "x-iscii-pa";
                case 65000:
                    return "utf-7";
                case 65001:
                    return "utf-8";
                default:
                    return DLSConstants.WindowsCodePage;
                #endregion
            }
        }
       /// <summary>
       /// Copy text formatting
       /// </summary>
       /// <param name="sourceFormat"></param>
       /// <param name="destFormat"></param>
        private void CopyTextFormatting(TextFormat sourceFormat, TextFormat destFormat)
        {
            destFormat.BackColor  = sourceFormat.BackColor;
            destFormat.HighlightColor = sourceFormat.HighlightColor;
            destFormat.Bidi = sourceFormat.Bidi;
            destFormat.Bold = sourceFormat.Bold;
            destFormat.FontColor = sourceFormat.FontColor;
            destFormat.FontFamily = sourceFormat.FontFamily;
            destFormat.FontSize = sourceFormat.FontSize ;
            destFormat.Italic = sourceFormat.Italic;
            destFormat.Strike = sourceFormat.Strike;
            destFormat.Style = sourceFormat.Style;
            destFormat.TextAlign = sourceFormat.TextAlign;
            destFormat.Underline = sourceFormat.Underline;
            destFormat.Shadow = sourceFormat.Shadow;
            destFormat.IsHiddenText = sourceFormat.IsHiddenText;
            destFormat.SubSuperScript = sourceFormat.SubSuperScript;
            destFormat.DoubleStrike = sourceFormat.DoubleStrike;
            destFormat.Emboss = sourceFormat.Emboss;
            destFormat.Engrave = sourceFormat.Engrave;
            destFormat.CharacterStyleName = sourceFormat.CharacterStyleName;
            destFormat.Position = sourceFormat.Position;
            destFormat.LocalIdASCII = sourceFormat.LocalIdASCII;
            destFormat.LocalIdForEast = sourceFormat.LocalIdForEast;
        }
       /// <summary>
       /// Set the default value of the paragraph format
       /// </summary>
       /// <param name="wParagraphFormat"></param>
        private void SetDefaultValue(WParagraphFormat paragraphFormat)
        {
            paragraphFormat.AfterSpacing = 0;
            paragraphFormat.BeforeSpacing = 0;
            paragraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
            paragraphFormat.LeftIndent = 0;
            paragraphFormat.RightIndent = 0;
            paragraphFormat.RightIndentBi = 0;
            paragraphFormat.FirstLineIndentBi = 0;
            paragraphFormat.BackColor = Color.Empty;
            paragraphFormat.ForeColor = Color.Empty;
            paragraphFormat.LineSpacing = 0;
            paragraphFormat.LineSpacingRule = LineSpacingRule.AtLeast;
            paragraphFormat.TextureStyle = TextureStyle.TextureNone;
        }
       /// <summary>
       /// Process the table information based on the levels
       /// </summary>
        private void ProcessTableInfo()
        {
            if (m_bInTable && m_currentLevel == 0)
                m_currentLevel = 1;
            PrepareTableInfo tableInfo = new PrepareTableInfo(m_bInTable, m_currentLevel, m_previousLevel);
            WTextBody currentTextBody = (m_textBody != null) ? m_textBody : CurrentSection.Body;
            if (tableInfo.InTable && tableInfo.State != DocReaderAdapterBase.PrepareTableState.LeaveTable)
            {
                if (m_currRow == null)
                {
                    if (m_currTable == null)
                        m_currTable = new WTable(m_document);
                    m_currRow = m_currTable.AddRow(false, false);
                    m_currCell = m_currRow.AddCell(false);
                    m_textBody = m_currCell;
                   // m_cellFormatTable.Clear();
                }
                else if (m_bCellFinished)
                {
                    m_currCell = m_currTable.LastRow.AddCell();
                    m_textBody = m_currCell;
                }
            }
            if (m_bCellFinished)
                m_bCellFinished = false;
            switch (tableInfo.State)
            {
                case Syncfusion.DocIO.DLS.DocReaderAdapterBase.PrepareTableState.EnterTable:
                    if (tableInfo.PrevLevel == 0)
                    {
                        m_nestedTextBody.Push(currentTextBody);
                    }
                    EnsureUpperTable(tableInfo.Level);
                    break;

                case Syncfusion.DocIO.DLS.DocReaderAdapterBase.PrepareTableState.LeaveTable:
                    EnsureLowerTable(tableInfo.Level);
                    break;
            }
        }
       /// <summary>
       /// Ensure the lower level tables
       /// </summary>
       /// <param name="level">Current paragraph level</param>
        private void EnsureLowerTable(int level)
        {
            while (m_nestedTable.Count > level)
            {
                m_nestedTable.Pop();
            }

            if (level == 0)
            {
                m_textBody = m_nestedTextBody.Pop();
                m_textBody.Items.Add(m_currTable);
                //m_currParagraph = null;
                m_currTable = null;
                m_currRow = null;
                m_previousLevel = level;
            }
            else
            {
                WTable currTable = m_currTable as WTable;
                m_currTable = m_nestedTable.Pop();
                m_currRow = m_currTable.LastRow;
                m_textBody = m_currTable.LastCell;
                m_textBody.Items.Add(currTable);
            }
        }
       /// <summary>
       /// Ensure the upper level tables
       /// </summary>
       /// <param name="level">Currentl paragraph level</param>
        private void EnsureUpperTable(int level)
        {
            while (m_nestedTable.Count < level - 1)
            {
                if (m_currTable != null)
                {
                    m_nestedTable.Push(m_currTable as WTable);
                }

                m_currTable = new WTable(m_document);
                m_textBody = m_currTable.AddRow(false, false).AddCell(false);
            }
        }
   
       /// <summary>
       /// Apply list formatting
       /// </summary>
        private void ApplyListFormatting(string token, string tokenKey, string tokenValue,WListFormat listFormat)
        {         
            string styleName=null ;
   
            foreach (KeyValuePair<string, string> listOverrideTable in m_listOverrideTable)
            {
                if (listOverrideTable.Key == token)
                {
                    styleName = listOverrideTable.Value;
                    listFormat.ApplyStyle(styleName);
                    listFormat.ListLevelNumber = 0;
                    break;
                }

            }
        }
       /// <summary>
       /// Apply Section formatting
       /// </summary>
       private void ApplySectionFormatting()
        {
            if (m_secFormat.BottomMargin >= 0)
                CurrentSection.PageSetup.Margins.Bottom = m_secFormat.BottomMargin;
            if (m_secFormat.LeftMargin  >= 0)
                CurrentSection.PageSetup.Margins.Left  = m_secFormat.LeftMargin ;
            if (m_secFormat.RightMargin  >= 0)
                CurrentSection.PageSetup.Margins.Right  = m_secFormat.RightMargin ;
            if (m_secFormat.TopMargin  >= 0)
                CurrentSection.PageSetup.Margins.Top  = m_secFormat.TopMargin ;

            if (m_secFormat.PageSize != null)
            {
                if (m_secFormat.PageSize.Width > 0 && m_secFormat .PageSize .Height >0)
                   CurrentSection.PageSetup.PageSize = m_secFormat.PageSize;            
            }         
            CurrentSection.PageSetup.HeaderDistance = m_secFormat.HeaderDistance;
            CurrentSection.PageSetup.FooterDistance = m_secFormat.FooterDistance;
            m_document.DefaultTabWidth = m_secFormat.DefaultTabWidth;
            CurrentSection.PageSetup.VerticalAlignment = m_secFormat.VertAlignment;
            if (m_secFormat.PageOrientation != PageOrientation.Portrait)
                CurrentSection.PageSetup.Orientation = m_secFormat.PageOrientation;
            m_document.DifferentOddAndEvenPages = m_secFormat.DifferentOddAndEvenPage;
        }
       private void ParseShapeToken(string token, string tokenKey, string tokenValue)
       {
           if (m_bIsShapePicture)
           {
               switch (tokenKey)
               {
                   case "posh":
                       switch (tokenValue)
                       {
                           case "1":
                               m_currPicture.HorizontalAlignment = ShapeHorizontalAlignment.Left;
                               break;
                           case "2":
                               m_currPicture.HorizontalAlignment = ShapeHorizontalAlignment.Center;
                               break;
                           case "3":
                               m_currPicture.HorizontalAlignment = ShapeHorizontalAlignment.Right;
                               break;
                           case "4":
                               m_currPicture.HorizontalAlignment = ShapeHorizontalAlignment.Inside;
                               break;
                           case "5":
                               m_currPicture.HorizontalAlignment = ShapeHorizontalAlignment.Outside;
                               break;
                       }
                       break;
                   case "posv":
                       switch (tokenValue)
                       {
                           case "1":
                               m_currPicture.VerticalAlignment = ShapeVerticalAlignment.Top;
                               break;
                           case "2":
                               m_currPicture.VerticalAlignment = ShapeVerticalAlignment.Center;
                               break;
                           case "3":
                               m_currPicture.VerticalAlignment = ShapeVerticalAlignment.Bottom;
                               break;
                           case "4":
                               m_currPicture.VerticalAlignment = ShapeVerticalAlignment.Inside;
                               break;
                           case "5":
                               m_currPicture.VerticalAlignment = ShapeVerticalAlignment.Outside;
                               break;
                       }
                       break;
                   case "posrelh":
                       switch (tokenValue)
                       {
                           case "0":
                               m_currPicture.HorizontalOrigin = HorizontalOrigin.Margin;
                               break;
                           case "1":
                               m_currPicture.HorizontalOrigin = HorizontalOrigin.Page;
                               break;
                           case "2":
                               m_currPicture.HorizontalOrigin = HorizontalOrigin.Column;
                               break;
                           case "3":
                               m_currPicture.HorizontalOrigin = HorizontalOrigin.Character;
                               break;
                       }
                       break;
                   case "posrelv":
                       switch (tokenValue)
                       {
                           case "0":
                               m_currPicture.VerticalOrigin = VerticalOrigin.Margin;
                               break;
                           case "1":
                               m_currPicture.VerticalOrigin = VerticalOrigin.Page;
                               break;
                           case "2":
                               m_currPicture.VerticalOrigin = VerticalOrigin.Paragraph;
                               break;
                           case "3":
                               m_currPicture.VerticalOrigin = VerticalOrigin.Line;
                               break;
                       }
                       break;
                   case "fLayoutInCell":
                       if (tokenValue == "1")
                           (m_currPicture as WPicture).LayoutInCell = true;
                       else
                           (m_currPicture as WPicture).LayoutInCell = false;
                       break;
                   case "shprslt":
                       m_bIsShapePicture = true;
                       break;
                   case "nonshppict":
                       m_bIsShapePicture = false;
                       break;
                   case "shppict":
                       m_bIsShapePicture = true;
                       break;
                   case "shpwr":
                       switch (Convert.ToInt32(tokenValue))
                       {
                           case 1:
                               m_currShapeFormat.m_textWrappingStyle = TextWrappingStyle.TopAndBottom;
                               break;
                           case 2:
                               m_currShapeFormat.m_textWrappingStyle = TextWrappingStyle.Square;
                               break;
                           case 3:
                               m_currShapeFormat.m_textWrappingStyle = TextWrappingStyle.InFrontOfText;
                               break;
                           case 4:
                               m_currShapeFormat.m_textWrappingStyle = TextWrappingStyle.Tight;
                               break;
                           case 5:
                               m_currShapeFormat.m_textWrappingStyle = TextWrappingStyle.Through;
                               break;
                           case 6:
                               m_currShapeFormat.m_textWrappingStyle = TextWrappingStyle.Behind;
                               break;
                       }
                       break;
                   case "shpwrk":
                       switch (Convert.ToInt32(tokenValue))
                       {
                           case 0:
                               m_currShapeFormat.m_textWrappingType = TextWrappingType.Both;
                               break;
                           case 1:
                               m_currShapeFormat.m_textWrappingType = TextWrappingType.Left;
                               break;
                           case 2:
                               m_currShapeFormat.m_textWrappingType = TextWrappingType.Right;
                               break;
                           case 3:
                               m_currShapeFormat.m_textWrappingType = TextWrappingType.Largest;
                               break;
                       }
                       break;
                   case "shpbypara":
                       m_currShapeFormat.m_vertOrgin = VerticalOrigin.Paragraph;
                       break;
                   case "shpbymargin":
                       m_currShapeFormat.m_vertOrgin = VerticalOrigin.Margin;
                       break;
                   case "shpbypage":
                       m_currShapeFormat.m_vertOrgin = VerticalOrigin.Page;
                       break;
                   case "shpbxpage":
                       m_currShapeFormat.m_horizOrgin = HorizontalOrigin.Page;
                       break;
                   case "shpbxmargin":
                       m_currShapeFormat.m_horizOrgin = HorizontalOrigin.Margin;
                       break;
                   case "shpbxcolumn":
                       m_currShapeFormat.m_horizOrgin = HorizontalOrigin.Column;
                       break;
                   case "shpleft":
                       m_currShapeFormat.m_horizPosition = ExtractTwipsValue(tokenValue);
                       m_currShapeFormat.m_left = GetIntValue(tokenValue);
                       break;
                   case "shpleft-":
                       float left = ExtractTwipsValue(tokenValue);
                       m_currShapeFormat.m_horizPosition = -left;
                       left = GetIntValue(tokenValue);
                       m_currShapeFormat.m_left = -left;
                       break;
                   case "shptop":                       
                       m_currShapeFormat.m_vertPosition = ExtractTwipsValue(tokenValue);
                       m_currShapeFormat.m_top = GetIntValue(tokenValue);
                       break;
                   case "shptop-":
                       float top = ExtractTwipsValue(tokenValue);
                       m_currShapeFormat.m_vertPosition = -top;
                       top = GetIntValue(tokenValue);
                       m_currShapeFormat.m_top = -top;
                       break;
                   case "shpright":
                       m_currShapeFormat.m_right = GetIntValue(tokenValue);
                       break;
                   case "shpright-":
                       float right = GetIntValue(tokenValue);
                       m_currShapeFormat.m_right = -right;
                       break;
                   case "shpbottom":
                       m_currShapeFormat.m_bottom = GetIntValue(tokenValue);
                       break;
                   case "shpbottom-":
                       float bottom = GetIntValue(tokenValue);
                       m_currShapeFormat.m_bottom = -bottom;
                       break;
                   case "shpfblwtxt":
                       if (Convert.ToInt32(tokenValue) == 1)
                       {
                           m_currShapeFormat.m_isBelowText = true;
                           if (m_currShapeFormat.m_textWrappingStyle == TextWrappingStyle.Inline)
                               m_currShapeFormat.m_textWrappingStyle = TextWrappingStyle.Behind;
                       }
                       else
                       {
                           m_currShapeFormat.m_isBelowText = false;
                           if (m_currShapeFormat.m_textWrappingStyle == TextWrappingStyle.Inline)
                               m_currShapeFormat.m_textWrappingStyle = TextWrappingStyle.InFrontOfText;
                       }
                       break;
                   case "shpfhdr":
                       if (Convert.ToInt32(tokenValue) == 1)
                           m_currShapeFormat.m_isInHeader = true;
                       else
                           m_currShapeFormat.m_isInHeader = false;
                       break;
                   case "shplockanchor":
                       m_currShapeFormat.m_isLockAnchor = true;
                       break;
                   case "shpz":
                       m_currShapeFormat.m_zOrder = GetIntValue(tokenValue);
                       break;
                   case "shplid":
                       m_currShapeFormat.m_uniqueId = GetIntValue(tokenValue);
                       break;
                   case "shpinst":
                       m_bIsShapeInstruction = true;
                       m_shapeInstructionStack.Push(c_groupStart);
                       break;
                   case "object":
                       m_bIsObject = true;
                       m_objectStack.Push(c_controlStart);
                       break;
               }
           }
       }
       /// <summary>
       /// Parse picture token
       /// </summary>
       /// <param name="token"></param>
       /// <param name="tokenKey"></param>
       /// <param name="tokenValue"></param>
        private void ParsePictureToken(string token, string tokenKey, string tokenValue)
        {
            if (m_bIsShapePicture)
            {
                switch (tokenKey)
                {
                    case "picscalex":
                        m_picFormat.WidthScale = Convert.ToSingle(tokenValue);
                        break;
                    case "picscaley":
                        m_picFormat.HeightScale = Convert.ToSingle(tokenValue);
                        break;
                    case "picwgoal":
                        m_picFormat.Width = ExtractTwipsValue(tokenValue);
                        break;
                    case "pichgoal":
                        m_picFormat.Height = ExtractTwipsValue(tokenValue);
                        break;
                    case "picw":
                        m_picFormat.PicW = GetIntValue (tokenValue);
                        break;
                    case "pich":
                        m_picFormat.picH = GetIntValue(tokenValue);
                        break;
                    case "object":
                        m_bIsObject = true;
                        m_objectStack.Push(c_controlStart );
                        break;
                }
                if ((tokenKey == "bin" || m_previousTokenKey == "bin") && m_lexer.m_prevChar == (char)1)
                    AppendPictureToParagraph(token);
            }
        }
       /// <summary>
       /// Get Int Value
       /// </summary>
       /// <param name="tokenValue"></param>
       /// <returns></returns>
        private int GetIntValue(string tokenValue)
        {
            int value;
            Int32.TryParse(tokenValue,out value);
            return value;
        }
       /// <summary>
       /// Parse Vertical alignment of the page
       /// </summary>
       /// <param name="token"></param>
       /// <param name="tokenKey"></param>
       /// <param name="tokenValue"></param>
        private void ParsePageVerticalAlignment(string token, string tokenKey, string tokenValue)
        {
            switch (tokenKey)
            {
                case "vertal":             
                case "vertalb":
                    m_secFormat .VertAlignment  = PageAlignment.Bottom;
                    break;
                case "vertalt":
                    m_secFormat.VertAlignment = PageAlignment.Top;
                    break;
                case "vertalc":
                    m_secFormat.VertAlignment = PageAlignment.Middle;
                    break;
                case "vertalj":
                    m_secFormat.VertAlignment = PageAlignment.Justified;
                    break;
            }
        }
       /// <summary>
       /// Parse outline levels
       /// </summary>
       /// <param name="token"></param>
       /// <param name="tokenKey"></param>
       /// <param name="tokenValue"></param>
        private void ParseOutLineLevel(string token, string tokenKey, string tokenValue)
        {
            switch (Convert.ToInt32(tokenValue))
            {
                case 0:
                    CurrentPara.ParagraphFormat.OutlineLevel = OutlineLevel.Level1;
                    break;
                case 1:
                    CurrentPara.ParagraphFormat.OutlineLevel = OutlineLevel.Level2 ;
                    break;
                case 2:
                    CurrentPara.ParagraphFormat.OutlineLevel = OutlineLevel.Level3 ;
                    break;
                case 3:
                    CurrentPara.ParagraphFormat.OutlineLevel = OutlineLevel.Level4 ;
                    break;
                case 4:
                    CurrentPara.ParagraphFormat.OutlineLevel = OutlineLevel.Level5 ;
                    break;
                case 5:
                    CurrentPara.ParagraphFormat.OutlineLevel = OutlineLevel.Level6 ;
                    break;
                case 6:
                    CurrentPara.ParagraphFormat.OutlineLevel = OutlineLevel.Level7 ;
                    break;
                case 7:
                    CurrentPara.ParagraphFormat.OutlineLevel = OutlineLevel.Level8 ;
                    break;
                case 8:
                    CurrentPara.ParagraphFormat.OutlineLevel = OutlineLevel.Level9 ;
                    break;
                default :
                    CurrentPara.ParagraphFormat.OutlineLevel = OutlineLevel.BodyText;
                    break;

                   
            }
        }
       /// <summary>
       /// Parse Borders
       /// </summary>
       /// <param name="token"></param>
       /// <param name="tokenKey"></param>
       /// <param name="tokenValue"></param>
        private void ParseParagraphBorders(string token,string tokenKey,string tokenValue)
         {
            switch (tokenKey)
            {
                case "brdrtbl":
                    if (m_bIsRow)
                    {
                        if (m_bIsBorderBottom)
                            CurrCellFormat.Borders.Bottom.BorderType = BorderStyle.Cleared ;
                        if (m_bIsBorderLeft)
                            CurrCellFormat.Borders.Left.BorderType = BorderStyle.Cleared ;
                        if (m_bIsBorderTop)
                            CurrCellFormat.Borders.Top.BorderType = BorderStyle.Cleared ;
                        if (m_bIsBorderRight)
                            CurrCellFormat.Borders.Right.BorderType = BorderStyle.Cleared ;
                        if (m_bIsRowBorderBottom)
                            CurrRowFormat.Borders.Bottom.BorderType = BorderStyle.Cleared;
                        if (m_bIsRowBorderLeft)
                            CurrRowFormat.Borders.Left.BorderType = BorderStyle.Cleared;
                        if (m_bIsRowBorderTop)
                            CurrRowFormat.Borders.Top.BorderType = BorderStyle.Cleared;
                        if (m_bIsRowBorderRight)
                            CurrRowFormat.Borders.Right.BorderType = BorderStyle.Cleared;
                    }
                    break;
                case "brdrt":
                    m_bIsBorderTop = true;
                    m_bIsBorderBottom = false;
                    m_bIsBorderLeft = false ;
                    m_bIsBorderRight = false;
                    break;
                case "brdrb":
                    m_bIsBorderBottom = true;
                    m_bIsBorderLeft = false;
                    m_bIsBorderRight = false;
                    m_bIsBorderTop = false;
                    break;
                case "brdrl":
                    m_bIsBorderLeft = true;
                    m_bIsBorderRight = false;
                    m_bIsBorderTop = false;
                    m_bIsBorderBottom = false;
                    break;
                case "brdrr":
                    m_bIsBorderRight = true;
                    m_bIsBorderBottom = false;
                    m_bIsBorderLeft = false ;
                    m_bIsBorderTop = false;
                    
                    break;
                case "box":
                    m_bIsBorderTop = true;
                    m_bIsBorderBottom = true;
                    m_bIsBorderLeft = true;
                    m_bIsBorderRight = true;
                    break;
                case "brdrsh":
                    if (m_bIsRow)
                    {
                        if (m_bIsHorizontalBorder)
                            CurrRowFormat.Borders.Horizontal.Shadow = true;
                        if (m_bIsVerticalBorder)
                            CurrRowFormat.Borders.Vertical.Shadow = true;
                        if (m_bIsRowBorderBottom)
                            CurrRowFormat.Borders.Bottom.Shadow = true;
                        if (m_bIsRowBorderLeft)
                            CurrRowFormat.Borders.Left.Shadow = true;
                        if (m_bIsRowBorderTop)
                            CurrRowFormat.Borders.Top.Shadow = true;
                        if (m_bIsRowBorderRight)
                            CurrRowFormat.Borders.Right.Shadow = true;
                        if (m_bIsBorderBottom)
                            CurrCellFormat.Borders.Bottom.Shadow = true;
                        if (m_bIsBorderLeft)
                            CurrCellFormat.Borders.Left.Shadow = true;
                        if (m_bIsBorderTop)
                            CurrCellFormat.Borders.Top.Shadow = true;
                        if (m_bIsBorderRight)
                            CurrCellFormat.Borders.Right.Shadow = true;
                    }
                    else
                    {
                        if (m_bIsBorderBottom)
                            CurrentPara.ParagraphFormat.Borders.Bottom.Shadow = true;
                        if (m_bIsBorderLeft)
                            CurrentPara.ParagraphFormat.Borders.Left.Shadow = true;
                        if (m_bIsBorderTop)
                            CurrentPara.ParagraphFormat.Borders.Top.Shadow = true;
                        if (m_bIsBorderRight)
                            CurrentPara.ParagraphFormat.Borders.Right.Shadow = true;
                    }
                    break;
                case "brdrs":
                    if (m_bIsRow)
                    {
                        if (m_bIsHorizontalBorder)
                            CurrRowFormat.Borders.Horizontal.BorderType = BorderStyle.Single;
                        if (m_bIsVerticalBorder)
                            CurrRowFormat.Borders.Vertical.BorderType = BorderStyle.Single;
                        if (m_bIsRowBorderBottom)
                            CurrRowFormat.Borders.Bottom.BorderType = BorderStyle.Single;
                        if (m_bIsRowBorderLeft)
                            CurrRowFormat.Borders.Left.BorderType = BorderStyle.Single;
                        if (m_bIsRowBorderTop)
                            CurrRowFormat.Borders.Top.BorderType = BorderStyle.Single;
                        if (m_bIsRowBorderRight)
                            CurrRowFormat.Borders.Right.BorderType = BorderStyle.Single;
                        if (m_bIsBorderBottom)
                            CurrCellFormat.Borders.Bottom.BorderType = BorderStyle.Single;
                        if (m_bIsBorderLeft)
                            CurrCellFormat.Borders.Left .BorderType = BorderStyle.Single;
                        if (m_bIsBorderTop)
                            CurrCellFormat.Borders.Top.BorderType = BorderStyle.Single;
                        if (m_bIsBorderRight)
                            CurrCellFormat.Borders.Right.BorderType = BorderStyle.Single;
                    }
                    else
                    {
                        if (m_bIsBorderBottom)
                            CurrentPara.ParagraphFormat.Borders.Bottom.BorderType = BorderStyle.Single;
                        if (m_bIsBorderLeft)
                            CurrentPara.ParagraphFormat.Borders.Left.BorderType = BorderStyle.Single;
                        if (m_bIsBorderTop)
                            CurrentPara.ParagraphFormat.Borders.Top.BorderType = BorderStyle.Single;
                        if (m_bIsBorderRight)
                            CurrentPara.ParagraphFormat.Borders.Right.BorderType = BorderStyle.Single;
                    }
                    break;
             
                case "brdrth":
                    if (m_bIsRow)
                    {
                        if (m_bIsHorizontalBorder)
                            CurrRowFormat.Borders.Horizontal.BorderType = BorderStyle.Thick;
                        if (m_bIsVerticalBorder)
                            CurrRowFormat.Borders.Vertical.BorderType = BorderStyle.Thick;
                        if (m_bIsRowBorderBottom)
                            CurrRowFormat.Borders.Bottom.BorderType = BorderStyle.Thick;
                        if (m_bIsRowBorderLeft)
                            CurrRowFormat.Borders.Left.BorderType = BorderStyle.Thick;
                        if (m_bIsRowBorderTop)
                            CurrRowFormat.Borders.Top.BorderType = BorderStyle.Thick;
                        if (m_bIsRowBorderRight)
                            CurrRowFormat.Borders.Right.BorderType = BorderStyle.Thick;

                        if (m_bIsBorderBottom)
                            CurrCellFormat.Borders.Bottom.BorderType = BorderStyle.Thick ;
                        if (m_bIsBorderLeft)
                            CurrCellFormat.Borders.Left.BorderType = BorderStyle.Thick ;
                        if (m_bIsBorderTop)
                            CurrCellFormat.Borders.Top.BorderType = BorderStyle.Thick ;
                        if (m_bIsBorderRight)
                            CurrCellFormat.Borders.Right.BorderType = BorderStyle.Thick ;
                    }
                    else
                    {
                        if (m_bIsBorderBottom)
                            CurrentPara.ParagraphFormat.Borders.Bottom.BorderType = BorderStyle.Thick;
                        if (m_bIsBorderLeft)
                            CurrentPara.ParagraphFormat.Borders.Left.BorderType = BorderStyle.Thick;
                        if (m_bIsBorderTop)
                            CurrentPara.ParagraphFormat.Borders.Top.BorderType = BorderStyle.Thick;
                        if (m_bIsBorderRight)
                            CurrentPara.ParagraphFormat.Borders.Right.BorderType = BorderStyle.Thick;
                    }
                    break;
                case "brdrdb":
                    if (m_bIsRow)
                    {
                        if (m_bIsHorizontalBorder)
                            CurrRowFormat.Borders.Horizontal.BorderType = BorderStyle.Double;
                        if (m_bIsVerticalBorder)
                            CurrRowFormat.Borders.Vertical.BorderType = BorderStyle.Double;
                        if (m_bIsRowBorderBottom)
                            CurrRowFormat.Borders.Bottom.BorderType = BorderStyle.Double;
                        if (m_bIsRowBorderLeft)
                            CurrRowFormat.Borders.Left.BorderType = BorderStyle.Double;
                        if (m_bIsRowBorderTop)
                            CurrRowFormat.Borders.Top.BorderType = BorderStyle.Double;
                        if (m_bIsRowBorderRight)
                            CurrRowFormat.Borders.Right.BorderType = BorderStyle.Double;

                        if (m_bIsBorderBottom)
                            CurrCellFormat.Borders.Bottom.BorderType = BorderStyle.Double ;
                        if (m_bIsBorderLeft)
                            CurrCellFormat.Borders.Left.BorderType = BorderStyle.Double ;
                        if (m_bIsBorderTop)
                            CurrCellFormat.Borders.Top.BorderType = BorderStyle.Double ;
                        if (m_bIsBorderRight)
                            CurrCellFormat.Borders.Right.BorderType = BorderStyle.Double ;
                    }
                    else
                    {
                        if (m_bIsBorderBottom)
                            CurrentPara.ParagraphFormat.Borders.Bottom.BorderType = BorderStyle.Double;
                        if (m_bIsBorderLeft)
                            CurrentPara.ParagraphFormat.Borders.Left.BorderType = BorderStyle.Double;
                        if (m_bIsBorderTop)
                            CurrentPara.ParagraphFormat.Borders.Top.BorderType = BorderStyle.Double;
                        if (m_bIsBorderRight)
                            CurrentPara.ParagraphFormat.Borders.Right.BorderType = BorderStyle.Double;
                    }
                    break;
                case "brdrdot":
                    if (m_bIsRow)
                    {
                        if (m_bIsHorizontalBorder)
                            CurrRowFormat.Borders.Horizontal.BorderType = BorderStyle.Dot;
                        if (m_bIsVerticalBorder)
                            CurrRowFormat.Borders.Vertical.BorderType = BorderStyle.Dot;
                        if (m_bIsRowBorderBottom)
                            CurrRowFormat.Borders.Bottom.BorderType = BorderStyle.Dot;
                        if (m_bIsRowBorderLeft)
                            CurrRowFormat.Borders.Left.BorderType = BorderStyle.Dot;
                        if (m_bIsRowBorderTop)
                            CurrRowFormat.Borders.Top.BorderType = BorderStyle.Dot;
                        if (m_bIsRowBorderRight)
                            CurrRowFormat.Borders.Right.BorderType = BorderStyle.Dot;

                        if (m_bIsBorderBottom)
                            CurrCellFormat.Borders.Bottom.BorderType = BorderStyle.Dot ;
                        if (m_bIsBorderLeft)
                            CurrCellFormat.Borders.Left.BorderType = BorderStyle.Dot ;
                        if (m_bIsBorderTop)
                            CurrCellFormat.Borders.Top.BorderType = BorderStyle.Dot ;
                        if (m_bIsBorderRight)
                            CurrCellFormat.Borders.Right.BorderType = BorderStyle.Dot ;
                    }
                    else
                    {
                        if (m_bIsBorderBottom)
                            CurrentPara.ParagraphFormat.Borders.Bottom.BorderType = BorderStyle.Dot;
                        if (m_bIsBorderLeft)
                            CurrentPara.ParagraphFormat.Borders.Left.BorderType = BorderStyle.Dot;
                        if (m_bIsBorderTop)
                            CurrentPara.ParagraphFormat.Borders.Top.BorderType = BorderStyle.Dot;
                        if (m_bIsBorderRight)
                            CurrentPara.ParagraphFormat.Borders.Right.BorderType = BorderStyle.Dot;
                    }

                    break;
                case "brdrdashsm":
                    if (m_bIsRow)
                    {
                        if (m_bIsHorizontalBorder)
                            CurrRowFormat.Borders.Horizontal.BorderType = BorderStyle.DashSmallGap;
                        if (m_bIsVerticalBorder)
                            CurrRowFormat.Borders.Vertical.BorderType = BorderStyle.DashSmallGap;
                        if (m_bIsRowBorderBottom)
                            CurrRowFormat.Borders.Bottom.BorderType = BorderStyle.DashSmallGap;
                        if (m_bIsRowBorderLeft)
                            CurrRowFormat.Borders.Left.BorderType = BorderStyle.DashSmallGap;
                        if (m_bIsRowBorderTop)
                            CurrRowFormat.Borders.Top.BorderType = BorderStyle.DashSmallGap;
                        if (m_bIsRowBorderRight)
                            CurrRowFormat.Borders.Right.BorderType = BorderStyle.DashSmallGap;

                        if (m_bIsBorderBottom)
                            CurrCellFormat.Borders.Bottom.BorderType = BorderStyle.DashSmallGap ;
                        if (m_bIsBorderLeft)
                            CurrCellFormat.Borders.Left.BorderType = BorderStyle.DashSmallGap ;
                        if (m_bIsBorderTop)
                            CurrCellFormat.Borders.Top.BorderType = BorderStyle.DashSmallGap ;
                        if (m_bIsBorderRight)
                            CurrCellFormat.Borders.Right.BorderType = BorderStyle.DashSmallGap ;
                    }
                    else
                    {
                        if (m_bIsBorderBottom)
                            CurrentPara.ParagraphFormat.Borders.Bottom.BorderType = BorderStyle.DashSmallGap;
                        if (m_bIsBorderLeft)
                            CurrentPara.ParagraphFormat.Borders.Left.BorderType = BorderStyle.DashSmallGap;
                        if (m_bIsBorderTop)
                            CurrentPara.ParagraphFormat.Borders.Top.BorderType = BorderStyle.DashSmallGap;
                        if (m_bIsBorderRight)
                            CurrentPara.ParagraphFormat.Borders.Right.BorderType = BorderStyle.DashSmallGap;
                    }
                    break;
                case "brdrdash":
                    if (m_bIsRow)
                    {
                        if (m_bIsHorizontalBorder)
                            CurrRowFormat.Borders.Horizontal.BorderType = BorderStyle.DashLargeGap;
                        if (m_bIsVerticalBorder)
                            CurrRowFormat.Borders.Vertical.BorderType = BorderStyle.DashLargeGap;
                        if (m_bIsRowBorderBottom)
                            CurrRowFormat.Borders.Bottom.BorderType = BorderStyle.DashLargeGap;
                        if (m_bIsRowBorderLeft)
                            CurrRowFormat.Borders.Left.BorderType = BorderStyle.DashLargeGap;
                        if (m_bIsRowBorderTop)
                            CurrRowFormat.Borders.Top.BorderType = BorderStyle.DashLargeGap;
                        if (m_bIsRowBorderRight)
                            CurrRowFormat.Borders.Right.BorderType = BorderStyle.DashLargeGap;

                        if (m_bIsBorderBottom)
                            CurrCellFormat.Borders.Bottom.BorderType = BorderStyle.DashLargeGap;
                        if (m_bIsBorderLeft)
                            CurrCellFormat.Borders.Left.BorderType = BorderStyle.DashLargeGap;
                        if (m_bIsBorderTop)
                            CurrCellFormat.Borders.Top.BorderType = BorderStyle.DashLargeGap;
                        if (m_bIsBorderRight)
                            CurrCellFormat.Borders.Right.BorderType = BorderStyle.DashLargeGap;
                    }
                    else
                    {
                        if (m_bIsBorderBottom)
                            CurrentPara.ParagraphFormat.Borders.Bottom.BorderType = BorderStyle.DashLargeGap;
                        if (m_bIsBorderLeft)
                            CurrentPara.ParagraphFormat.Borders.Left.BorderType = BorderStyle.DashLargeGap;
                        if (m_bIsBorderTop)
                            CurrentPara.ParagraphFormat.Borders.Top.BorderType = BorderStyle.DashLargeGap;
                        if (m_bIsBorderRight)
                            CurrentPara.ParagraphFormat.Borders.Right.BorderType = BorderStyle.DashLargeGap;
                    }
                    break;
                case "brdrdashdd":
                    if (m_bIsRow)
                    {
                        if (m_bIsHorizontalBorder)
                            CurrRowFormat.Borders.Horizontal.BorderType = BorderStyle.DotDotDash;
                        if (m_bIsVerticalBorder)
                            CurrRowFormat.Borders.Vertical.BorderType = BorderStyle.DotDotDash;
                        if (m_bIsRowBorderBottom)
                            CurrRowFormat.Borders.Bottom.BorderType = BorderStyle.DotDotDash;
                        if (m_bIsRowBorderLeft)
                            CurrRowFormat.Borders.Left.BorderType = BorderStyle.DotDotDash;
                        if (m_bIsRowBorderTop)
                            CurrRowFormat.Borders.Top.BorderType = BorderStyle.DotDotDash;
                        if (m_bIsRowBorderRight)
                            CurrRowFormat.Borders.Right.BorderType = BorderStyle.DotDotDash;

                        if (m_bIsBorderBottom)
                            CurrCellFormat.Borders.Bottom.BorderType = BorderStyle.DotDotDash;
                        if (m_bIsBorderLeft)
                            CurrCellFormat.Borders.Left.BorderType = BorderStyle.DotDotDash;
                        if (m_bIsBorderTop)
                            CurrCellFormat.Borders.Top.BorderType = BorderStyle.DotDotDash;
                        if (m_bIsBorderRight)
                            CurrCellFormat.Borders.Right.BorderType = BorderStyle.DotDotDash;
                    }
                    else
                    {
                        if (m_bIsBorderBottom)
                            CurrentPara.ParagraphFormat.Borders.Bottom.BorderType = BorderStyle.DotDotDash;
                        if (m_bIsBorderLeft)
                            CurrentPara.ParagraphFormat.Borders.Left.BorderType = BorderStyle.DotDotDash;
                        if (m_bIsBorderTop)
                            CurrentPara.ParagraphFormat.Borders.Top.BorderType = BorderStyle.DotDotDash;
                        if (m_bIsBorderRight)
                            CurrentPara.ParagraphFormat.Borders.Right.BorderType = BorderStyle.DotDotDash;
                    }
                    break;
                case "brdrtnthmg":
                    if (m_bIsRow)
                    {
                        if (m_bIsHorizontalBorder)
                            CurrRowFormat.Borders.Horizontal.BorderType = BorderStyle.ThickThinMediumGap;
                        if (m_bIsVerticalBorder)
                            CurrRowFormat.Borders.Vertical.BorderType = BorderStyle.ThickThinMediumGap;
                        if (m_bIsRowBorderBottom)
                            CurrRowFormat.Borders.Bottom.BorderType = BorderStyle.ThickThinMediumGap;
                        if (m_bIsRowBorderLeft)
                            CurrRowFormat.Borders.Left.BorderType = BorderStyle.ThickThinMediumGap;
                        if (m_bIsRowBorderTop)
                            CurrRowFormat.Borders.Top.BorderType = BorderStyle.ThickThinMediumGap;
                        if (m_bIsRowBorderRight)
                            CurrRowFormat.Borders.Right.BorderType = BorderStyle.ThickThinMediumGap;

                        if (m_bIsBorderBottom)
                            CurrCellFormat.Borders.Bottom.BorderType = BorderStyle.ThickThinMediumGap;
                        if (m_bIsBorderLeft)
                            CurrCellFormat.Borders.Left.BorderType = BorderStyle.ThickThinMediumGap;
                        if (m_bIsBorderTop)
                            CurrCellFormat.Borders.Top.BorderType = BorderStyle.ThickThinMediumGap;
                        if (m_bIsBorderRight)
                            CurrCellFormat.Borders.Right.BorderType = BorderStyle.ThickThinMediumGap;
                    }
                    else
                    {
                        if (m_bIsBorderBottom)
                            CurrentPara.ParagraphFormat.Borders.Bottom.BorderType = BorderStyle.ThickThinMediumGap;
                        if (m_bIsBorderLeft)
                            CurrentPara.ParagraphFormat.Borders.Left.BorderType = BorderStyle.ThickThinMediumGap;
                        if (m_bIsBorderTop)
                            CurrentPara.ParagraphFormat.Borders.Top.BorderType = BorderStyle.ThickThinMediumGap;
                        if (m_bIsBorderRight)
                            CurrentPara.ParagraphFormat.Borders.Right.BorderType = BorderStyle.ThickThinMediumGap;
                    }
                        break;
                case "brdrtnthsg":
                        if (m_bIsRow)
                        {
                            if (m_bIsHorizontalBorder)
                                CurrRowFormat.Borders.Horizontal.BorderType = BorderStyle.ThinThinSmallGap;
                            if (m_bIsVerticalBorder)
                                CurrRowFormat.Borders.Vertical.BorderType = BorderStyle.ThinThinSmallGap;
                            if (m_bIsRowBorderBottom)
                                CurrRowFormat.Borders.Bottom.BorderType = BorderStyle.ThinThinSmallGap;
                            if (m_bIsRowBorderLeft)
                                CurrRowFormat.Borders.Left.BorderType = BorderStyle.ThinThinSmallGap;
                            if (m_bIsRowBorderTop)
                                CurrRowFormat.Borders.Top.BorderType = BorderStyle.ThinThinSmallGap;
                            if (m_bIsRowBorderRight)
                                CurrRowFormat.Borders.Right.BorderType = BorderStyle.ThinThinSmallGap;

                            if (m_bIsBorderBottom)
                                CurrCellFormat.Borders.Bottom.BorderType = BorderStyle.ThinThinSmallGap;
                            if (m_bIsBorderLeft)
                                CurrCellFormat.Borders.Left.BorderType = BorderStyle.ThinThinSmallGap;
                            if (m_bIsBorderTop)
                                CurrCellFormat.Borders.Top.BorderType = BorderStyle.ThinThinSmallGap;
                            if (m_bIsBorderRight)
                                CurrCellFormat.Borders.Right.BorderType = BorderStyle.ThinThinSmallGap;
                        }
                        else
                        {
                            if (m_bIsBorderBottom)
                                CurrentPara.ParagraphFormat.Borders.Bottom.BorderType = BorderStyle.ThinThinSmallGap;
                            if (m_bIsBorderLeft)
                                CurrentPara.ParagraphFormat.Borders.Left.BorderType = BorderStyle.ThinThinSmallGap;
                            if (m_bIsBorderTop)
                                CurrentPara.ParagraphFormat.Borders.Top.BorderType = BorderStyle.ThinThinSmallGap;
                            if (m_bIsBorderRight)
                                CurrentPara.ParagraphFormat.Borders.Right.BorderType = BorderStyle.ThinThinSmallGap;
                        }
                        break;
                    
                case "brdrtnthtnsg":
                    if (m_bIsRow)
                        {
                            if (m_bIsHorizontalBorder)
                                CurrRowFormat.Borders.Horizontal.BorderType = BorderStyle.ThinThickThinSmallGap;
                            if (m_bIsVerticalBorder)
                                CurrRowFormat.Borders.Vertical.BorderType = BorderStyle.ThinThickThinSmallGap;
                            if (m_bIsRowBorderBottom)
                                CurrRowFormat.Borders.Bottom.BorderType = BorderStyle.ThinThickThinSmallGap;
                            if (m_bIsRowBorderLeft)
                                CurrRowFormat.Borders.Left.BorderType = BorderStyle.ThinThickThinSmallGap;
                            if (m_bIsRowBorderTop)
                                CurrRowFormat.Borders.Top.BorderType = BorderStyle.ThinThickThinSmallGap;
                            if (m_bIsRowBorderRight)
                                CurrRowFormat.Borders.Right.BorderType = BorderStyle.ThinThickThinSmallGap;

                            if (m_bIsBorderBottom)
                                CurrCellFormat.Borders.Bottom.BorderType = BorderStyle.ThinThickThinSmallGap;
                            if (m_bIsBorderLeft)
                                CurrCellFormat.Borders.Left.BorderType = BorderStyle.ThinThickThinSmallGap;
                            if (m_bIsBorderTop)
                                CurrCellFormat.Borders.Top.BorderType = BorderStyle.ThinThickThinSmallGap;
                            if (m_bIsBorderRight)
                                CurrCellFormat.Borders.Right.BorderType = BorderStyle.ThinThickThinSmallGap;
                        }
                        else
                        {
                            if (m_bIsBorderBottom)
                                CurrentPara.ParagraphFormat.Borders.Bottom.BorderType = BorderStyle.ThinThickThinSmallGap;
                            if (m_bIsBorderLeft)
                                CurrentPara.ParagraphFormat.Borders.Left.BorderType = BorderStyle.ThinThickThinSmallGap;
                            if (m_bIsBorderTop)
                                CurrentPara.ParagraphFormat.Borders.Top.BorderType = BorderStyle.ThinThickThinSmallGap;
                            if (m_bIsBorderRight)
                                CurrentPara.ParagraphFormat.Borders.Right.BorderType = BorderStyle.ThinThickThinSmallGap;
                        }
                    break;
                case "brdrthtnmg":
                        if (m_bIsRow)
                        {
                            if (m_bIsHorizontalBorder)
                                CurrRowFormat.Borders.Horizontal.BorderType = BorderStyle.ThickThinMediumGap;
                            if (m_bIsVerticalBorder)
                                CurrRowFormat.Borders.Vertical.BorderType = BorderStyle.ThickThinMediumGap;
                            if (m_bIsRowBorderBottom)
                                CurrRowFormat.Borders.Bottom.BorderType = BorderStyle.ThinThickMediumGap;
                            if (m_bIsRowBorderLeft)
                                CurrRowFormat.Borders.Left.BorderType = BorderStyle.ThinThickMediumGap;
                            if (m_bIsRowBorderTop)
                                CurrRowFormat.Borders.Top.BorderType = BorderStyle.ThinThickMediumGap;
                            if (m_bIsRowBorderRight)
                                CurrRowFormat.Borders.Right.BorderType = BorderStyle.ThinThickMediumGap;

                            if (m_bIsBorderBottom)
                                CurrCellFormat.Borders.Bottom.BorderType = BorderStyle.ThinThickMediumGap;
                            if (m_bIsBorderLeft)
                                CurrCellFormat.Borders.Left.BorderType = BorderStyle.ThinThickMediumGap;
                            if (m_bIsBorderTop)
                                CurrCellFormat.Borders.Top.BorderType = BorderStyle.ThinThickMediumGap;
                            if (m_bIsBorderRight)
                                CurrCellFormat.Borders.Right.BorderType = BorderStyle.ThinThickMediumGap;
                        }
                        else
                        {
                            if (m_bIsBorderBottom)
                                CurrentPara.ParagraphFormat.Borders.Bottom.BorderType = BorderStyle.ThinThickMediumGap;
                            if (m_bIsBorderLeft)
                                CurrentPara.ParagraphFormat.Borders.Left.BorderType = BorderStyle.ThinThickMediumGap;
                            if (m_bIsBorderTop)
                                CurrentPara.ParagraphFormat.Borders.Top.BorderType = BorderStyle.ThinThickMediumGap;
                            if (m_bIsBorderRight)
                                CurrentPara.ParagraphFormat.Borders.Right.BorderType = BorderStyle.ThinThickMediumGap;
                        }
                    break;
                case "brdrtnthlg":
                    if (m_bIsRow)
                    {
                        if (m_bIsHorizontalBorder)
                            CurrRowFormat.Borders.Horizontal.BorderType = BorderStyle.ThickThinLargeGap;
                        if (m_bIsVerticalBorder)
                            CurrRowFormat.Borders.Vertical.BorderType = BorderStyle.ThickThinLargeGap;
                        if (m_bIsRowBorderBottom)
                            CurrRowFormat.Borders.Bottom.BorderType = BorderStyle.ThickThinLargeGap;
                        if (m_bIsRowBorderLeft)
                            CurrRowFormat.Borders.Left.BorderType = BorderStyle.ThickThinLargeGap;
                        if (m_bIsRowBorderTop)
                            CurrRowFormat.Borders.Top.BorderType = BorderStyle.ThickThinLargeGap;
                        if (m_bIsRowBorderRight)
                            CurrRowFormat.Borders.Right.BorderType = BorderStyle.ThickThinLargeGap;

                        if (m_bIsBorderBottom)
                            CurrCellFormat.Borders.Bottom.BorderType = BorderStyle.ThickThinLargeGap;
                        if (m_bIsBorderLeft)
                            CurrCellFormat.Borders.Left.BorderType = BorderStyle.ThickThinLargeGap;
                        if (m_bIsBorderTop)
                            CurrCellFormat.Borders.Top.BorderType = BorderStyle.ThickThinLargeGap;
                        if (m_bIsBorderRight)
                            CurrCellFormat.Borders.Right.BorderType = BorderStyle.ThickThinLargeGap;
                    }
                    else
                    {
                        if (m_bIsBorderBottom)
                            CurrentPara.ParagraphFormat.Borders.Bottom.BorderType = BorderStyle.ThickThinLargeGap;
                        if (m_bIsBorderLeft)
                            CurrentPara.ParagraphFormat.Borders.Left.BorderType = BorderStyle.ThickThinLargeGap;
                        if (m_bIsBorderTop)
                            CurrentPara.ParagraphFormat.Borders.Top.BorderType = BorderStyle.ThickThinLargeGap;
                        if (m_bIsBorderRight)
                            CurrentPara.ParagraphFormat.Borders.Right.BorderType = BorderStyle.ThickThinLargeGap;
                    }
                    break;
                case "brdrthtnlg":
                    if (m_bIsRow)
                    {
                        if (m_bIsHorizontalBorder)
                            CurrRowFormat.Borders.Horizontal.BorderType = BorderStyle.ThinThickLargeGap;
                        if (m_bIsVerticalBorder)
                            CurrRowFormat.Borders.Vertical.BorderType = BorderStyle.ThinThickLargeGap;
                        if (m_bIsRowBorderBottom)
                            CurrRowFormat.Borders.Bottom.BorderType = BorderStyle.ThinThickLargeGap;
                        if (m_bIsRowBorderLeft)
                            CurrRowFormat.Borders.Left.BorderType = BorderStyle.ThinThickLargeGap;
                        if (m_bIsRowBorderTop)
                            CurrRowFormat.Borders.Top.BorderType = BorderStyle.ThinThickLargeGap;
                        if (m_bIsRowBorderRight)
                            CurrRowFormat.Borders.Right.BorderType = BorderStyle.ThinThickLargeGap;

                        if (m_bIsBorderBottom)
                            CurrCellFormat.Borders.Bottom.BorderType = BorderStyle.ThinThickLargeGap;
                        if (m_bIsBorderLeft)
                            CurrCellFormat.Borders.Left.BorderType = BorderStyle.ThinThickLargeGap;
                        if (m_bIsBorderTop)
                            CurrCellFormat.Borders.Top.BorderType = BorderStyle.ThinThickLargeGap;
                        if (m_bIsBorderRight)
                            CurrCellFormat.Borders.Right.BorderType = BorderStyle.ThinThickLargeGap;
                    }
                    else
                    {
                        if (m_bIsBorderBottom)
                            CurrentPara.ParagraphFormat.Borders.Bottom.BorderType = BorderStyle.ThinThickLargeGap;
                        if (m_bIsBorderLeft)
                            CurrentPara.ParagraphFormat.Borders.Left.BorderType = BorderStyle.ThinThickLargeGap;
                        if (m_bIsBorderTop)
                            CurrentPara.ParagraphFormat.Borders.Top.BorderType = BorderStyle.ThinThickLargeGap;
                        if (m_bIsBorderRight)
                            CurrentPara.ParagraphFormat.Borders.Right.BorderType = BorderStyle.ThinThickLargeGap;
                    }
                    break;

                case"brdremboss":
                    if (m_bIsRow)
                    {
                        if (m_bIsHorizontalBorder)
                            CurrRowFormat.Borders.Horizontal.BorderType = BorderStyle.Emboss3D;
                        if (m_bIsVerticalBorder)
                            CurrRowFormat.Borders.Vertical.BorderType = BorderStyle.Emboss3D;
                        if (m_bIsRowBorderBottom)
                            CurrRowFormat.Borders.Bottom.BorderType = BorderStyle.Emboss3D;
                        if (m_bIsRowBorderLeft)
                            CurrRowFormat.Borders.Left.BorderType = BorderStyle.Emboss3D;
                        if (m_bIsRowBorderTop)
                            CurrRowFormat.Borders.Top.BorderType = BorderStyle.Emboss3D;
                        if (m_bIsRowBorderRight)
                            CurrRowFormat.Borders.Right.BorderType = BorderStyle.Emboss3D;

                        if (m_bIsBorderBottom)
                            CurrCellFormat.Borders.Bottom.BorderType = BorderStyle.Emboss3D;
                        if (m_bIsBorderLeft)
                            CurrCellFormat.Borders.Left.BorderType = BorderStyle.Emboss3D;
                        if (m_bIsBorderTop)
                            CurrCellFormat.Borders.Top.BorderType = BorderStyle.Emboss3D;
                        if (m_bIsBorderRight)
                            CurrCellFormat.Borders.Right.BorderType = BorderStyle.Emboss3D;
                    }
                    else
                    {
                        if (m_bIsBorderBottom)
                            CurrentPara.ParagraphFormat.Borders.Bottom.BorderType = BorderStyle.Emboss3D;
                        if (m_bIsBorderLeft)
                            CurrentPara.ParagraphFormat.Borders.Left.BorderType = BorderStyle.Emboss3D;
                        if (m_bIsBorderTop)
                            CurrentPara.ParagraphFormat.Borders.Top.BorderType = BorderStyle.Emboss3D;
                        if (m_bIsBorderRight)
                            CurrentPara.ParagraphFormat.Borders.Right.BorderType = BorderStyle.Emboss3D;
                    }
                    break;
                case "brdrengrave":
                    if (m_bIsRow)
                    {
                        if (m_bIsHorizontalBorder)
                            CurrRowFormat.Borders.Horizontal.BorderType = BorderStyle.Engrave3D;
                        if (m_bIsVerticalBorder)
                            CurrRowFormat.Borders.Vertical.BorderType = BorderStyle.Engrave3D;
                        if (m_bIsRowBorderBottom)
                            CurrRowFormat.Borders.Bottom.BorderType = BorderStyle.Engrave3D;
                        if (m_bIsRowBorderLeft)
                            CurrRowFormat.Borders.Left.BorderType = BorderStyle.Engrave3D;
                        if (m_bIsRowBorderTop)
                            CurrRowFormat.Borders.Top.BorderType = BorderStyle.Engrave3D;
                        if (m_bIsRowBorderRight)
                            CurrRowFormat.Borders.Right.BorderType = BorderStyle.Engrave3D;

                        if (m_bIsBorderBottom)
                            CurrCellFormat.Borders.Bottom.BorderType = BorderStyle.Engrave3D;
                        if (m_bIsBorderLeft)
                            CurrCellFormat.Borders.Left.BorderType = BorderStyle.Engrave3D;
                        if (m_bIsBorderTop)
                            CurrCellFormat.Borders.Top.BorderType = BorderStyle.Engrave3D;
                        if (m_bIsBorderRight)
                            CurrCellFormat.Borders.Right.BorderType = BorderStyle.Engrave3D;
                    }
                    else
                    {
                        if (m_bIsBorderBottom)
                            CurrentPara.ParagraphFormat.Borders.Bottom.BorderType = BorderStyle.Engrave3D;
                        if (m_bIsBorderLeft)
                            CurrentPara.ParagraphFormat.Borders.Left.BorderType = BorderStyle.Engrave3D;
                        if (m_bIsBorderTop)
                            CurrentPara.ParagraphFormat.Borders.Top.BorderType = BorderStyle.Engrave3D;
                        if (m_bIsBorderRight)
                            CurrentPara.ParagraphFormat.Borders.Right.BorderType = BorderStyle.Engrave3D;
                    }
                    break;
                case "brdrnone":
                case "brdrnil":
                    if (m_bIsRow)
                    {
                        if (m_bIsHorizontalBorder)
                            CurrRowFormat.Borders.Horizontal.BorderType = BorderStyle.Cleared;
                        if (m_bIsVerticalBorder)
                            CurrRowFormat.Borders.Vertical.BorderType = BorderStyle.Cleared;
                        if (m_bIsRowBorderBottom)
                            CurrRowFormat.Borders.Bottom.BorderType = BorderStyle.Cleared;
                        if (m_bIsRowBorderLeft)
                            CurrRowFormat.Borders.Left.BorderType = BorderStyle.Cleared;
                        if (m_bIsRowBorderTop)
                            CurrRowFormat.Borders.Top.BorderType = BorderStyle.Cleared;
                        if (m_bIsRowBorderRight)
                            CurrRowFormat.Borders.Right.BorderType = BorderStyle.Cleared;

                        if (m_bIsBorderBottom)
                            CurrCellFormat.Borders.Bottom.BorderType = BorderStyle.Cleared ;
                        if (m_bIsBorderLeft)
                            CurrCellFormat.Borders.Left.BorderType = BorderStyle.Cleared ;
                        if (m_bIsBorderTop)
                            CurrCellFormat.Borders.Top.BorderType = BorderStyle.Cleared ;
                        if (m_bIsBorderRight)
                            CurrCellFormat.Borders.Right.BorderType = BorderStyle.Cleared ;
                    }
                    else
                    {
                        if (m_bIsBorderBottom)
                            CurrentPara.ParagraphFormat.Borders.Bottom.BorderType = BorderStyle.None;
                        if (m_bIsBorderLeft)
                            CurrentPara.ParagraphFormat.Borders.Left.BorderType = BorderStyle.None;
                        if (m_bIsBorderTop)
                            CurrentPara.ParagraphFormat.Borders.Top.BorderType = BorderStyle.None;
                        if (m_bIsBorderRight)
                            CurrentPara.ParagraphFormat.Borders.Right.BorderType = BorderStyle.None;
                    }
                    break;
                case "brdrtnthtnmg":
                    if (m_bIsRow)
                    {
                        if (m_bIsHorizontalBorder)
                            CurrRowFormat.Borders.Horizontal.BorderType = BorderStyle.ThickThickThinMediumGap;
                        if (m_bIsVerticalBorder)
                            CurrRowFormat.Borders.Vertical.BorderType = BorderStyle.ThickThickThinMediumGap;
                        if (m_bIsRowBorderBottom)
                            CurrRowFormat.Borders.Bottom.BorderType = BorderStyle.ThickThickThinMediumGap;
                        if (m_bIsRowBorderLeft)
                            CurrRowFormat.Borders.Left.BorderType = BorderStyle.ThickThickThinMediumGap;
                        if (m_bIsRowBorderTop)
                            CurrRowFormat.Borders.Top.BorderType = BorderStyle.ThickThickThinMediumGap;
                        if (m_bIsRowBorderRight)
                            CurrRowFormat.Borders.Right.BorderType = BorderStyle.ThickThickThinMediumGap;

                        if (m_bIsBorderBottom)
                            CurrCellFormat.Borders.Bottom.BorderType = BorderStyle.ThickThickThinMediumGap;
                        if (m_bIsBorderLeft)
                            CurrCellFormat.Borders.Left.BorderType = BorderStyle.ThickThickThinMediumGap;
                        if (m_bIsBorderTop)
                            CurrCellFormat.Borders.Top.BorderType = BorderStyle.ThickThickThinMediumGap;
                        if (m_bIsBorderRight)
                            CurrCellFormat.Borders.Right.BorderType = BorderStyle.ThickThickThinMediumGap;
                    }
                    else
                    {
                        if (m_bIsBorderBottom)
                            CurrentPara.ParagraphFormat.Borders.Bottom.BorderType = BorderStyle.ThickThickThinMediumGap;
                        if (m_bIsBorderLeft)
                            CurrentPara.ParagraphFormat.Borders.Left.BorderType = BorderStyle.ThickThickThinMediumGap;
                        if (m_bIsBorderTop)
                            CurrentPara.ParagraphFormat.Borders.Top.BorderType = BorderStyle.ThickThickThinMediumGap;
                        if (m_bIsBorderRight)
                            CurrentPara.ParagraphFormat.Borders.Right.BorderType = BorderStyle.ThickThickThinMediumGap;
                    }
                    break;
                case "brdrhair":
                    if (m_bIsRow)
                    {
                        if (m_bIsHorizontalBorder)
                            CurrRowFormat.Borders.Horizontal.BorderType = BorderStyle.Hairline;
                        if (m_bIsVerticalBorder)
                            CurrRowFormat.Borders.Vertical.BorderType = BorderStyle.Hairline;
                        if (m_bIsRowBorderBottom)
                            CurrRowFormat.Borders.Bottom.BorderType = BorderStyle.Hairline;
                        if (m_bIsRowBorderLeft)
                            CurrRowFormat.Borders.Left.BorderType = BorderStyle.Hairline;
                        if (m_bIsRowBorderTop)
                            CurrRowFormat.Borders.Top.BorderType = BorderStyle.Hairline;
                        if (m_bIsRowBorderRight)
                            CurrRowFormat.Borders.Right.BorderType = BorderStyle.Hairline;

                        if (m_bIsBorderBottom)
                            CurrCellFormat.Borders.Bottom.BorderType = BorderStyle.Hairline;
                        if (m_bIsBorderLeft)
                            CurrCellFormat.Borders.Left.BorderType = BorderStyle.Hairline;
                        if (m_bIsBorderTop)
                            CurrCellFormat.Borders.Top.BorderType = BorderStyle.Hairline;
                        if (m_bIsBorderRight)
                            CurrCellFormat.Borders.Right.BorderType = BorderStyle.Hairline;
                    }
                    else
                    {
                        if (m_bIsBorderBottom)
                            CurrentPara.ParagraphFormat.Borders.Bottom.BorderType = BorderStyle.Hairline;
                        if (m_bIsBorderLeft)
                            CurrentPara.ParagraphFormat.Borders.Left.BorderType = BorderStyle.Hairline;
                        if (m_bIsBorderTop)
                            CurrentPara.ParagraphFormat.Borders.Top.BorderType = BorderStyle.Hairline;
                        if (m_bIsBorderRight)
                            CurrentPara.ParagraphFormat.Borders.Right.BorderType = BorderStyle.Hairline;
                    }
                    break;
                case "brdrdashd":
                    if (m_bIsRow)
                    {
                        if (m_bIsHorizontalBorder)
                            CurrRowFormat.Borders.Horizontal.BorderType = BorderStyle.DotDash;
                        if (m_bIsVerticalBorder)
                            CurrRowFormat.Borders.Vertical.BorderType = BorderStyle.DotDash;
                        if (m_bIsRowBorderBottom)
                            CurrRowFormat.Borders.Bottom.BorderType = BorderStyle.DotDash;
                        if (m_bIsRowBorderLeft)
                            CurrRowFormat.Borders.Left.BorderType = BorderStyle.DotDash;
                        if (m_bIsRowBorderTop)
                            CurrRowFormat.Borders.Top.BorderType = BorderStyle.DotDash;
                        if (m_bIsRowBorderRight)
                            CurrRowFormat.Borders.Right.BorderType = BorderStyle.DotDash;

                        if (m_bIsBorderBottom)
                            CurrCellFormat.Borders.Bottom.BorderType = BorderStyle.DotDash;
                        if (m_bIsBorderLeft)
                            CurrCellFormat.Borders.Left.BorderType = BorderStyle.DotDash;
                        if (m_bIsBorderTop)
                            CurrCellFormat.Borders.Top.BorderType = BorderStyle.DotDash;
                        if (m_bIsBorderRight)
                            CurrCellFormat.Borders.Right.BorderType = BorderStyle.DotDash;
                    }
                    else
                    {
                        if (m_bIsBorderBottom)
                            CurrentPara.ParagraphFormat.Borders.Bottom.BorderType = BorderStyle.DotDash;
                        if (m_bIsBorderLeft)
                            CurrentPara.ParagraphFormat.Borders.Left.BorderType = BorderStyle.DotDash;
                        if (m_bIsBorderTop)
                            CurrentPara.ParagraphFormat.Borders.Top.BorderType = BorderStyle.DotDash;
                        if (m_bIsBorderRight)
                            CurrentPara.ParagraphFormat.Borders.Right.BorderType = BorderStyle.DotDash;
                    }
                    break;
                case "brdrtriple":
                    if (m_bIsRow)
                    {
                        if (m_bIsHorizontalBorder)
                            CurrRowFormat.Borders.Horizontal.BorderType = BorderStyle.Triple;
                        if (m_bIsVerticalBorder)
                            CurrRowFormat.Borders.Vertical.BorderType = BorderStyle.Triple;
                        if (m_bIsRowBorderBottom)
                            CurrRowFormat.Borders.Bottom.BorderType = BorderStyle.Triple;
                        if (m_bIsRowBorderLeft)
                            CurrRowFormat.Borders.Left.BorderType = BorderStyle.Triple;
                        if (m_bIsRowBorderTop)
                            CurrRowFormat.Borders.Top.BorderType = BorderStyle.Triple;
                        if (m_bIsRowBorderRight)
                            CurrRowFormat.Borders.Right.BorderType = BorderStyle.Triple;

                        if (m_bIsBorderBottom)
                            CurrCellFormat.Borders.Bottom.BorderType = BorderStyle.Triple;
                        if (m_bIsBorderLeft)
                            CurrCellFormat.Borders.Left.BorderType = BorderStyle.Triple;
                        if (m_bIsBorderTop)
                            CurrCellFormat.Borders.Top.BorderType = BorderStyle.Triple;
                        if (m_bIsBorderRight)
                            CurrCellFormat.Borders.Right.BorderType = BorderStyle.Triple;
                    }
                    else
                    {
                        if (m_bIsBorderBottom)
                            CurrentPara.ParagraphFormat.Borders.Bottom.BorderType = BorderStyle.Triple;
                        if (m_bIsBorderLeft)
                            CurrentPara.ParagraphFormat.Borders.Left.BorderType = BorderStyle.Triple;
                        if (m_bIsBorderTop)
                            CurrentPara.ParagraphFormat.Borders.Top.BorderType = BorderStyle.Triple;
                        if (m_bIsBorderRight)
                            CurrentPara.ParagraphFormat.Borders.Right.BorderType = BorderStyle.Triple;
                    }
                    break;
                case "brdrthtnsg":
                    if (m_bIsRow)
                    {
                        if (m_bIsHorizontalBorder)
                            CurrRowFormat.Borders.Horizontal.BorderType = BorderStyle.ThinThickSmallGap;
                        if (m_bIsVerticalBorder)
                            CurrRowFormat.Borders.Vertical.BorderType = BorderStyle.ThinThickSmallGap;
                        if (m_bIsRowBorderBottom)
                            CurrRowFormat.Borders.Bottom.BorderType = BorderStyle.ThinThickSmallGap;
                        if (m_bIsRowBorderLeft)
                            CurrRowFormat.Borders.Left.BorderType = BorderStyle.ThinThickSmallGap;
                        if (m_bIsRowBorderTop)
                            CurrRowFormat.Borders.Top.BorderType = BorderStyle.ThinThickSmallGap;
                        if (m_bIsRowBorderRight)
                            CurrRowFormat.Borders.Right.BorderType = BorderStyle.ThinThickSmallGap;

                        if (m_bIsBorderBottom)
                            CurrCellFormat.Borders.Bottom.BorderType = BorderStyle.ThinThickSmallGap;
                        if (m_bIsBorderLeft)
                            CurrCellFormat.Borders.Left.BorderType = BorderStyle.ThinThickSmallGap;
                        if (m_bIsBorderTop)
                            CurrCellFormat.Borders.Top.BorderType = BorderStyle.ThinThickSmallGap;
                        if (m_bIsBorderRight)
                            CurrCellFormat.Borders.Right.BorderType = BorderStyle.ThinThickSmallGap;
                    }
                    else
                    {
                        if (m_bIsBorderBottom)
                            CurrentPara.ParagraphFormat.Borders.Bottom.BorderType = BorderStyle.ThinThickSmallGap;
                        if (m_bIsBorderLeft)
                            CurrentPara.ParagraphFormat.Borders.Left.BorderType = BorderStyle.ThinThickSmallGap;
                        if (m_bIsBorderTop)
                            CurrentPara.ParagraphFormat.Borders.Top.BorderType = BorderStyle.ThinThickSmallGap;
                        if (m_bIsBorderRight)
                            CurrentPara.ParagraphFormat.Borders.Right.BorderType = BorderStyle.ThinThickSmallGap;
                    }
                    break;
                case "brdrtnthtnlg":
                    if (m_bIsRow)
                    {
                        if (m_bIsHorizontalBorder)
                            CurrRowFormat.Borders.Horizontal.BorderType = BorderStyle.ThinThickThinLargeGap;
                        if (m_bIsVerticalBorder)
                            CurrRowFormat.Borders.Vertical.BorderType = BorderStyle.ThinThickThinLargeGap;
                        if (m_bIsRowBorderBottom)
                            CurrRowFormat.Borders.Bottom.BorderType = BorderStyle.ThinThickThinLargeGap;
                        if (m_bIsRowBorderLeft)
                            CurrRowFormat.Borders.Left.BorderType = BorderStyle.ThinThickThinLargeGap;
                        if (m_bIsRowBorderTop)
                            CurrRowFormat.Borders.Top.BorderType = BorderStyle.ThinThickSmallGap;
                        if (m_bIsRowBorderRight)
                            CurrRowFormat.Borders.Right.BorderType = BorderStyle.ThinThickSmallGap;

                        if (m_bIsBorderBottom)
                            CurrCellFormat.Borders.Bottom.BorderType = BorderStyle.ThinThickThinLargeGap;
                        if (m_bIsBorderLeft)
                            CurrCellFormat.Borders.Left.BorderType = BorderStyle.ThinThickThinLargeGap;
                        if (m_bIsBorderTop)
                            CurrCellFormat.Borders.Top.BorderType = BorderStyle.ThinThickSmallGap;
                        if (m_bIsBorderRight)
                            CurrCellFormat.Borders.Right.BorderType = BorderStyle.ThinThickSmallGap;
                    }
                    else
                    {
                        if (m_bIsBorderBottom)
                            CurrentPara.ParagraphFormat.Borders.Bottom.BorderType = BorderStyle.ThinThickThinLargeGap;
                        if (m_bIsBorderLeft)
                            CurrentPara.ParagraphFormat.Borders.Left.BorderType = BorderStyle.ThinThickThinLargeGap;
                        if (m_bIsBorderTop)
                            CurrentPara.ParagraphFormat.Borders.Top.BorderType = BorderStyle.ThinThickThinLargeGap;
                        if (m_bIsBorderRight)
                            CurrentPara.ParagraphFormat.Borders.Right.BorderType = BorderStyle.ThinThickThinLargeGap;
                    }
                    break;
                case "brdrwavy":
                    if (m_bIsRow)
                    {
                        if (m_bIsHorizontalBorder)
                            CurrRowFormat.Borders.Horizontal.BorderType = BorderStyle.Wave;
                        if (m_bIsVerticalBorder)
                            CurrRowFormat.Borders.Vertical.BorderType = BorderStyle.Wave;
                        if (m_bIsRowBorderBottom)
                            CurrRowFormat.Borders.Bottom.BorderType = BorderStyle.Wave;
                        if (m_bIsRowBorderLeft)
                            CurrRowFormat.Borders.Left.BorderType = BorderStyle.Wave;
                        if (m_bIsRowBorderTop)
                            CurrRowFormat.Borders.Top.BorderType = BorderStyle.Wave;
                        if (m_bIsRowBorderRight)
                            CurrRowFormat.Borders.Right.BorderType = BorderStyle.Wave;

                        if (m_bIsBorderBottom)
                            CurrCellFormat.Borders.Bottom.BorderType = BorderStyle.Wave;
                        if (m_bIsBorderLeft)
                            CurrCellFormat.Borders.Left.BorderType = BorderStyle.Wave;
                        if (m_bIsBorderTop)
                            CurrCellFormat.Borders.Top.BorderType = BorderStyle.Wave;
                        if (m_bIsBorderRight)
                            CurrCellFormat.Borders.Right.BorderType = BorderStyle.Wave;
                    }
                    else
                    {
                        if (m_bIsBorderBottom)
                            CurrentPara.ParagraphFormat.Borders.Bottom.BorderType = BorderStyle.Wave;
                        if (m_bIsBorderLeft)
                            CurrentPara.ParagraphFormat.Borders.Left.BorderType = BorderStyle.Wave;
                        if (m_bIsBorderTop)
                            CurrentPara.ParagraphFormat.Borders.Top.BorderType = BorderStyle.Wave;
                        if (m_bIsBorderRight)
                            CurrentPara.ParagraphFormat.Borders.Right.BorderType = BorderStyle.Wave;
                    }
                    break;
                case "brdrwavydb":
                    if (m_bIsRow)
                    {
                        if (m_bIsHorizontalBorder)
                            CurrRowFormat.Borders.Horizontal.BorderType = BorderStyle.DoubleWave;
                        if (m_bIsVerticalBorder)
                            CurrRowFormat.Borders.Vertical.BorderType = BorderStyle.DoubleWave;
                        if (m_bIsRowBorderBottom)
                            CurrRowFormat.Borders.Bottom.BorderType = BorderStyle.DoubleWave;
                        if (m_bIsRowBorderLeft)
                            CurrRowFormat.Borders.Left.BorderType = BorderStyle.DoubleWave;
                        if (m_bIsRowBorderTop)
                            CurrRowFormat.Borders.Top.BorderType = BorderStyle.DoubleWave;
                        if (m_bIsRowBorderRight)
                            CurrRowFormat.Borders.Right.BorderType = BorderStyle.DoubleWave;

                        if (m_bIsBorderBottom)
                            CurrCellFormat.Borders.Bottom.BorderType = BorderStyle.DoubleWave;
                        if (m_bIsBorderLeft)
                            CurrCellFormat.Borders.Left.BorderType = BorderStyle.DoubleWave;
                        if (m_bIsBorderTop)
                            CurrCellFormat.Borders.Top.BorderType = BorderStyle.DoubleWave;
                        if (m_bIsBorderRight)
                            CurrCellFormat.Borders.Right.BorderType = BorderStyle.DoubleWave;
                    }
                    else
                    {
                        if (m_bIsBorderBottom)
                            CurrentPara.ParagraphFormat.Borders.Bottom.BorderType = BorderStyle.DoubleWave;
                        if (m_bIsBorderLeft)
                            CurrentPara.ParagraphFormat.Borders.Left.BorderType = BorderStyle.DoubleWave;
                        if (m_bIsBorderTop)
                            CurrentPara.ParagraphFormat.Borders.Top.BorderType = BorderStyle.DoubleWave;
                        if (m_bIsBorderRight)
                            CurrentPara.ParagraphFormat.Borders.Right.BorderType = BorderStyle.DoubleWave;
                    }
                    break;
                case "brdrdashdotstr":
                    if (m_bIsRow)
                    {
                        if (m_bIsHorizontalBorder)
                            CurrRowFormat.Borders.Horizontal.BorderType = BorderStyle.DashDotStroker;
                        if (m_bIsVerticalBorder)
                            CurrRowFormat.Borders.Vertical.BorderType = BorderStyle.DashDotStroker;
                        if (m_bIsRowBorderBottom)
                            CurrRowFormat.Borders.Bottom.BorderType = BorderStyle.DashDotStroker;
                        if (m_bIsRowBorderLeft)
                            CurrRowFormat.Borders.Left.BorderType = BorderStyle.DashDotStroker;
                        if (m_bIsRowBorderTop)
                            CurrRowFormat.Borders.Top.BorderType = BorderStyle.DashDotStroker;
                        if (m_bIsRowBorderRight)
                            CurrRowFormat.Borders.Right.BorderType = BorderStyle.DashDotStroker;

                        if (m_bIsBorderBottom)
                            CurrCellFormat.Borders.Bottom.BorderType = BorderStyle.DashDotStroker;
                        if (m_bIsBorderLeft)
                            CurrCellFormat.Borders.Left.BorderType = BorderStyle.DashDotStroker;
                        if (m_bIsBorderTop)
                            CurrCellFormat.Borders.Top.BorderType = BorderStyle.DashDotStroker;
                        if (m_bIsBorderRight)
                            CurrCellFormat.Borders.Right.BorderType = BorderStyle.DashDotStroker;
                    }
                    else
                    {
                        if (m_bIsBorderBottom)
                            CurrentPara.ParagraphFormat.Borders.Bottom.BorderType = BorderStyle.DashDotStroker;
                        if (m_bIsBorderLeft)
                            CurrentPara.ParagraphFormat.Borders.Left.BorderType = BorderStyle.DashDotStroker;
                        if (m_bIsBorderTop)
                            CurrentPara.ParagraphFormat.Borders.Top.BorderType = BorderStyle.DashDotStroker;
                        if (m_bIsBorderRight)
                            CurrentPara.ParagraphFormat.Borders.Right.BorderType = BorderStyle.DashDotStroker;
                    }
                    break;
                case "brdrinset":
                    if (m_bIsRow)
                    {
                        if (m_bIsHorizontalBorder)
                            CurrRowFormat.Borders.Horizontal.BorderType = BorderStyle.Inset;
                        if (m_bIsVerticalBorder)
                            CurrRowFormat.Borders.Vertical.BorderType = BorderStyle.Inset;
                        if (m_bIsRowBorderBottom)
                            CurrRowFormat.Borders.Bottom.BorderType = BorderStyle.Inset;
                        if (m_bIsRowBorderLeft)
                            CurrRowFormat.Borders.Left.BorderType = BorderStyle.Inset;
                        if (m_bIsRowBorderTop)
                            CurrRowFormat.Borders.Top.BorderType = BorderStyle.Inset;
                        if (m_bIsRowBorderRight)
                            CurrRowFormat.Borders.Right.BorderType = BorderStyle.Inset;

                        if (m_bIsBorderBottom)
                            CurrCellFormat.Borders.Bottom.BorderType = BorderStyle.Inset;
                        if (m_bIsBorderLeft)
                            CurrCellFormat.Borders.Left.BorderType = BorderStyle.Inset;
                        if (m_bIsBorderTop)
                            CurrCellFormat.Borders.Top.BorderType = BorderStyle.Inset;
                        if (m_bIsBorderRight)
                            CurrCellFormat.Borders.Right.BorderType = BorderStyle.Inset;
                    }
                    else
                    {
                        if (m_bIsBorderBottom)
                            CurrentPara.ParagraphFormat.Borders.Bottom.BorderType = BorderStyle.Inset;
                        if (m_bIsBorderLeft)
                            CurrentPara.ParagraphFormat.Borders.Left.BorderType = BorderStyle.Inset;
                        if (m_bIsBorderTop)
                            CurrentPara.ParagraphFormat.Borders.Top.BorderType = BorderStyle.Inset;
                        if (m_bIsBorderRight)
                            CurrentPara.ParagraphFormat.Borders.Right.BorderType = BorderStyle.Inset;
                    }
                    break;
                case "brdroutset":
                    if (m_bIsRow)
                    {
                        if (m_bIsHorizontalBorder)
                            CurrRowFormat.Borders.Horizontal.BorderType = BorderStyle.Outset;
                        if (m_bIsVerticalBorder)
                            CurrRowFormat.Borders.Vertical.BorderType = BorderStyle.Outset;
                        if (m_bIsRowBorderBottom)
                            CurrRowFormat.Borders.Bottom.BorderType = BorderStyle.Outset;
                        if (m_bIsRowBorderLeft)
                            CurrRowFormat.Borders.Left.BorderType = BorderStyle.Outset;
                        if (m_bIsRowBorderTop)
                            CurrRowFormat.Borders.Top.BorderType = BorderStyle.Outset;
                        if (m_bIsRowBorderRight)
                            CurrRowFormat.Borders.Right.BorderType = BorderStyle.Outset;

                        if (m_bIsBorderBottom)
                            CurrCellFormat.Borders.Bottom.BorderType = BorderStyle.Outset;
                        if (m_bIsBorderLeft)
                            CurrCellFormat.Borders.Left.BorderType = BorderStyle.Outset;
                        if (m_bIsBorderTop)
                            CurrCellFormat.Borders.Top.BorderType = BorderStyle.Outset;
                        if (m_bIsBorderRight)
                            CurrCellFormat.Borders.Right.BorderType = BorderStyle.Outset;
                    }
                    else
                    {
                        if (m_bIsBorderBottom)
                            CurrentPara.ParagraphFormat.Borders.Bottom.BorderType = BorderStyle.Outset;
                        if (m_bIsBorderLeft)
                            CurrentPara.ParagraphFormat.Borders.Left.BorderType = BorderStyle.Outset;
                        if (m_bIsBorderTop)
                            CurrentPara.ParagraphFormat.Borders.Top.BorderType = BorderStyle.Outset;
                        if (m_bIsBorderRight)
                            CurrentPara.ParagraphFormat.Borders.Right.BorderType = BorderStyle.Outset;
                    }
                    break;

                case "brdrw":
                    float lineWidth = ExtractTwipsValue(tokenValue );
                    if (m_bIsRow)
                    {
                        if (m_bIsHorizontalBorder)
                            CurrRowFormat.Borders.Horizontal.LineWidth = lineWidth;
                        if (m_bIsVerticalBorder)
                            CurrRowFormat.Borders.Vertical.LineWidth = lineWidth;
                        if (m_bIsRowBorderBottom)
                            CurrRowFormat.Borders.Bottom.LineWidth = lineWidth;
                        if (m_bIsRowBorderLeft)
                            CurrRowFormat.Borders.Left.LineWidth = lineWidth;
                        if (m_bIsRowBorderTop)
                            CurrRowFormat.Borders.Top.LineWidth = lineWidth;
                        if (m_bIsRowBorderRight)
                            CurrRowFormat.Borders.Right.LineWidth = lineWidth;

                        if (m_bIsBorderBottom)
                            CurrCellFormat.Borders.Bottom.LineWidth = lineWidth;
                        if (m_bIsBorderLeft)
                            CurrCellFormat.Borders.Left.LineWidth = lineWidth;
                        if (m_bIsBorderTop)
                            CurrCellFormat.Borders.Top.LineWidth = lineWidth;
                        if (m_bIsBorderRight)
                            CurrCellFormat.Borders.Right.LineWidth = lineWidth;
                    }
                    else
                    {
                        if (m_bIsBorderBottom)
                            CurrentPara.ParagraphFormat.Borders.Bottom.LineWidth = lineWidth;
                        if (m_bIsBorderLeft)
                            CurrentPara.ParagraphFormat.Borders.Left.LineWidth = lineWidth;
                        if (m_bIsBorderTop)
                            CurrentPara.ParagraphFormat.Borders.Top.LineWidth = lineWidth;
                        if (m_bIsBorderRight)
                            CurrentPara.ParagraphFormat.Borders.Right.LineWidth = lineWidth;
                    }

                    break;
                case "brdrcf":
                            
                        int colorIndex = Convert.ToInt32(tokenValue);
                        foreach (KeyValuePair<int, RtfColor> colorTable in m_colorTable)
                        {
                            if (colorTable.Key == colorIndex)
                                CurrColorTable = colorTable.Value;
                        }
                        if (m_bIsRow)
                        {
                            if (m_bIsHorizontalBorder)
                                CurrRowFormat.Borders.Horizontal.Color = Color.FromArgb(CurrColorTable.RedN, CurrColorTable.GreenN, CurrColorTable.BlueN);
                            if (m_bIsVerticalBorder)
                                CurrRowFormat.Borders.Vertical.Color = Color.FromArgb(CurrColorTable.RedN, CurrColorTable.GreenN, CurrColorTable.BlueN);
                            if (m_bIsRowBorderBottom)
                                CurrRowFormat.Borders.Bottom.Color = Color.FromArgb(CurrColorTable.RedN, CurrColorTable.GreenN, CurrColorTable.BlueN);
                            if (m_bIsRowBorderLeft)
                                CurrRowFormat.Borders.Left.Color = Color.FromArgb(CurrColorTable.RedN, CurrColorTable.GreenN, CurrColorTable.BlueN);
                            if (m_bIsRowBorderTop)
                                CurrRowFormat.Borders.Top.Color = Color.FromArgb(CurrColorTable.RedN, CurrColorTable.GreenN, CurrColorTable.BlueN);
                            if (m_bIsRowBorderRight)
                                CurrRowFormat.Borders.Right.Color = Color.FromArgb(CurrColorTable.RedN, CurrColorTable.GreenN, CurrColorTable.BlueN);

                            if (m_bIsBorderBottom)
                                CurrCellFormat.Borders.Bottom.Color  = Color.FromArgb(CurrColorTable.RedN, CurrColorTable.GreenN, CurrColorTable.BlueN);
                            if (m_bIsBorderLeft)
                                CurrCellFormat.Borders.Left.Color = Color.FromArgb(CurrColorTable.RedN, CurrColorTable.GreenN, CurrColorTable.BlueN);
                            if (m_bIsBorderTop)
                                CurrCellFormat.Borders.Top.Color = Color.FromArgb(CurrColorTable.RedN, CurrColorTable.GreenN, CurrColorTable.BlueN);
                            if (m_bIsBorderRight)
                                CurrCellFormat.Borders.Right.Color = Color.FromArgb(CurrColorTable.RedN, CurrColorTable.GreenN, CurrColorTable.BlueN);
                        }
                        else
                        {
                            if (m_bIsBorderBottom)
                                CurrentPara.ParagraphFormat.Borders.Bottom.Color = Color.FromArgb(CurrColorTable.RedN, CurrColorTable.GreenN, CurrColorTable.BlueN);
                            if (m_bIsBorderLeft)
                                CurrentPara.ParagraphFormat.Borders.Left.Color = Color.FromArgb(CurrColorTable.RedN, CurrColorTable.GreenN, CurrColorTable.BlueN);
                            if (m_bIsBorderTop)
                                CurrentPara.ParagraphFormat.Borders.Top.Color = Color.FromArgb(CurrColorTable.RedN, CurrColorTable.GreenN, CurrColorTable.BlueN);
                            if (m_bIsBorderRight)
                                CurrentPara.ParagraphFormat.Borders.Right.Color = Color.FromArgb(CurrColorTable.RedN, CurrColorTable.GreenN, CurrColorTable.BlueN);
                        }
                      
                    break;
            }
        }

       /// <summary>
       /// Appply cell formatting
       /// </summary>
       /// <param name="cell"></param>
       /// <param name="cellFormat"></param>
        private void ApplyCellFormatting(WTableCell cell, CellFormat cellFormat)
        {
            //Set SamePaddingsAsTable as false
            if (cellFormat.Paddings.HasKey(Paddings.TopKey) || cellFormat.Paddings.HasKey(Paddings.LeftKey)
                || cellFormat.Paddings.HasKey(Paddings.RightKey) || cellFormat.Paddings.HasKey(Paddings.BottomKey))
            {
                cell.CellFormat.SamePaddingsAsTable = false;
            }
            else
                cell.CellFormat.SamePaddingsAsTable = true;
            cell.CellFormat.BackColor = cellFormat.BackColor;
            ApplyBorder(cell.CellFormat.Borders.Left, cellFormat.Borders.Left);
            ApplyBorder(cell.CellFormat.Borders.Right, cellFormat.Borders.Right);
            ApplyBorder(cell.CellFormat.Borders.Top, cellFormat.Borders.Top);
            ApplyBorder(cell.CellFormat.Borders.Bottom , cellFormat.Borders.Bottom);
            cell.CellFormat.CellWidth = cellFormat.CellWidth;
            cell.CellFormat.FitText = cellFormat.FitText;
            cell.CellFormat.ForeColor = cellFormat.ForeColor;
            cell.CellFormat.PreferredWidth.WidthType = cellFormat.PreferredWidth.WidthType;
            cell.CellFormat.PreferredWidth.Width = cellFormat.PreferredWidth.Width;
            //Apply Cell Padding
            if (cellFormat.Paddings.HasKey(Paddings.BottomKey))
            {
                cell.CellFormat.Paddings.Bottom = cellFormat.Paddings.Bottom;
            }
            if (cellFormat.Paddings.HasKey(Paddings.RightKey))
            {
                cell.CellFormat.Paddings.Right = cellFormat.Paddings.Right;
            }
            if (cellFormat.Paddings.HasKey(Paddings.LeftKey))
            {
                cell.CellFormat.Paddings.Left = cellFormat.Paddings.Left;
            }
            if (cellFormat.Paddings.HasKey(Paddings.TopKey))
            {
                cell.CellFormat.Paddings.Top = cellFormat.Paddings.Top;
            }
            cell.CellFormat.TextDirection = cellFormat.TextDirection;
            cell.CellFormat.TextWrap = cellFormat.TextWrap;
            cell.CellFormat.VerticalAlignment = cellFormat.VerticalAlignment;
            cell.CellFormat.VerticalMerge = cellFormat.VerticalMerge;
            cell.CellFormat.HorizontalMerge = cellFormat.HorizontalMerge;
            cell.CellFormat.TextureStyle = cellFormat.TextureStyle;
        }
       /// <summary>
       /// Apply Row formatting
       /// </summary>
       /// <param name="row"></param>
       /// <param name="rowFormat"></param>
        private void ApplyRowFormatting(WTableRow row, RowFormat rowFormat)
        {
            if (rowFormat.HasValue(RowFormat.IsAutoResizedCellsKey))
                row.RowFormat.IsAutoResized = rowFormat.IsAutoResized;
            else
                row.RowFormat.IsAutoResized = false;
            if (rowFormat.HasValue(RowFormat.HiddenKey))
                row.RowFormat.Hidden = rowFormat.Hidden;
            if (rowFormat.HasValue(RowFormat.ShadingColorKey))
                row.RowFormat.BackColor = rowFormat.BackColor;
            if (rowFormat.HasValue(RowFormat.BidiTableKey))
                row.RowFormat.Bidi = rowFormat.Bidi;
            if (rowFormat.HasValue(RowFormat.CellSpacingKey))
                row.RowFormat.CellSpacing = rowFormat.CellSpacing;
            if (rowFormat.HasValue(RowFormat.RowAlignmentKey))
                row.RowFormat.HorizontalAlignment = rowFormat.HorizontalAlignment;
            if (rowFormat.HasValue(RowFormat.RowHeightKey))
                row.RowFormat.Height = rowFormat.Height;
            if (rowFormat.HasValue(RowFormat.IsBreakAcrossPagesKey))
                row.RowFormat.IsBreakAcrossPages = rowFormat.IsBreakAcrossPages;
            if (rowFormat.HasValue(RowFormat.LeftIndentKey))
                row.RowFormat.LeftIndent = (float)Math.Round(rowFormat.LeftIndent, 2);
            if (rowFormat.HasValue(RowFormat.TextureStyleKey))
                row.RowFormat.TextureStyle = rowFormat.TextureStyle;
            if (rowFormat.HasValue(RowFormat.TablePositioning.DistanceFromLeftKey))
                row.RowFormat.Positioning.DistanceFromLeft = rowFormat.Positioning.DistanceFromLeft;
            if (rowFormat.HasValue(RowFormat.TablePositioning.DistanceFromRightKey))
                row.RowFormat.Positioning.DistanceFromRight = rowFormat.Positioning.DistanceFromRight;
            if (rowFormat.HasValue(RowFormat.TablePositioning.DistanceFromTopKey))
                row.RowFormat.Positioning.DistanceFromTop = rowFormat.Positioning.DistanceFromTop;
            if (rowFormat.HasValue(RowFormat.TablePositioning.DistanceFromBottomKey))
                row.RowFormat.Positioning.DistanceFromTop = rowFormat.Positioning.DistanceFromBottom;
            if (rowFormat.HasValue(RowFormat.TablePositioning.HorizPosKey))
                row.RowFormat.Positioning.HorizPosition = rowFormat.Positioning.HorizPosition;
            if (rowFormat.HasValue(RowFormat.TablePositioning.HorizRelKey))
                row.RowFormat.Positioning.HorizRelationTo = rowFormat.Positioning.HorizRelationTo;
            if (rowFormat.HasValue(RowFormat.TablePositioning.VertPosKey))
                row.RowFormat.Positioning.VertPosition = rowFormat.Positioning.VertPosition;
            if (rowFormat.HasValue(RowFormat.TablePositioning.VertRelKey))
                row.RowFormat.Positioning.VertRelationTo = rowFormat.Positioning.VertRelationTo;
            row.RowFormat.Paddings.Left = rowFormat.Paddings.Left;
            row.RowFormat.Paddings.Right = rowFormat.Paddings.Right;
            row.RowFormat.Paddings.Top = rowFormat.Paddings.Top;
            row.RowFormat.Paddings.Bottom = rowFormat.Paddings.Bottom;
            if (rowFormat.HasValue(RowFormat.PreferredWidthKey))
                CurrTable.TableFormat.PreferredWidth.Width = rowFormat.PreferredWidth.Width;
            if (rowFormat.HasValue(RowFormat.PreferredWidthTypeKey))
                CurrTable.TableFormat.PreferredWidth.WidthType = rowFormat.PreferredWidth.WidthType;
            ApplyBorder(row.RowFormat.Borders.Left, rowFormat.Borders.Left);
            ApplyBorder(row.RowFormat.Borders.Right, rowFormat.Borders.Right);
            ApplyBorder(row.RowFormat.Borders.Top, rowFormat.Borders.Top);
            ApplyBorder(row.RowFormat.Borders.Bottom, rowFormat.Borders.Bottom);
            ApplyBorder(row.RowFormat.Borders.Horizontal, rowFormat.Borders.Horizontal);
            ApplyBorder(row.RowFormat.Borders.Vertical, rowFormat.Borders.Vertical);
        }
       /// <summary>
       /// Apply cell border
       /// </summary>
       /// <param name="destBorder"></param>
       /// <param name="sourceBorder"></param>
        private void ApplyBorder(Border destBorder, Border sourceBorder)
        {
            if (sourceBorder.BorderType == BorderStyle.Cleared || sourceBorder.BorderType == BorderStyle.None)
            {
                destBorder.BorderType = sourceBorder.BorderType;
            }
            else
            {
                destBorder.BorderType = sourceBorder.BorderType;
                destBorder.Color = sourceBorder.Color;
                destBorder.LineWidth = sourceBorder.LineWidth;

            }         
        }
        /// <summary>
        /// Adds the new paragraph to the current section
        /// </summary>
        private void AddNewParagraph(IWParagraph newParagraph)
        {
            //Update paragraph Tabs collection
            UpdateTabsCollection(newParagraph.ParagraphFormat);
            if (m_currSection == null)
            {
                m_currSection = m_document.AddSection();
                m_textBody = m_currSection.Body;
            }
            if (m_textBody == null)
                m_textBody = m_currSection.Body;
            m_textBody.Items.Add(newParagraph);
            m_previousLevel = m_currentLevel;
            m_prevFormat = newParagraph.ParagraphFormat;
           //Applies break character formatting
            CopyTextFormatToCharFormat(newParagraph.BreakCharacterFormat, m_currTextFormat);
            m_currParagraph = null;
        }
       /// <summary>
       /// Update Paragraph Tabs collection
       /// </summary>
       /// <param name="newParagraph"></param>
        private void UpdateTabsCollection(WParagraphFormat paraFormat)
        {
            if (m_tabCollection.Count > 0 && paraFormat.Tabs.Count == 0)
            {
                foreach (KeyValuePair<int, TabFormat> tabCollection in m_tabCollection)
                {
                    paraFormat.Tabs.AddTab(tabCollection.Value.TabPosition, tabCollection.Value.TabJustification, tabCollection.Value.TabLeader);
                }
            }
            else if (m_tabCollection.Count > paraFormat.Tabs.Count)
            {
                for (int i = paraFormat.Tabs.Count + 1; i <= m_tabCollection.Count; i++)
                {
                    if (m_tabCollection.ContainsKey(i))
                        paraFormat.Tabs.AddTab(m_tabCollection[i].TabPosition, m_tabCollection[i].TabJustification, m_tabCollection[i].TabLeader);
                }
            }
            //RTF doesn't consider the tabs preserved in the base style when the tabs are not defined in inline and it preserve baseformat tabs with type as clear in paragraph direct formatting
            if (m_currentTableType == RtfTableType.None)
                UpdateDeleteTabsCollection(paraFormat, paraFormat.BaseFormat as WParagraphFormat);
        }
       /// <summary>
       /// Update tabs with type as clear from BaseFormat to direct paragraph formatting
       /// </summary>
       /// <param name="paraFormat"></param>
       /// <param name="baseFormat"></param>
        private void UpdateDeleteTabsCollection(WParagraphFormat destFormat, WParagraphFormat baseFormat)
        {
            bool isTabExist = false;
            while (baseFormat != null)
            {
                for (int i = 0; i < baseFormat.Tabs.Count; i++)
                {
                    for (int j = 0; j < destFormat.Tabs.Count; j++)
                    {
                        if (baseFormat.Tabs[i].Position == destFormat.Tabs[j].Position)
                        {
                            isTabExist = true;
                        }
                    }
                    if (!isTabExist)
                    {
                        Tab tab = destFormat.Tabs.AddTab();
                        tab.DeletePosition = baseFormat.Tabs[i].Position * DLSConstants.TwipsInOnePoint;
                    }
                    isTabExist = false;
                }
                baseFormat = baseFormat.BaseFormat as WParagraphFormat;
            }
        }
       /// <summary>
       /// Add new section to the current document
       /// </summary>
        private void AddNewSection(IWSection newSection)
        {           
            if (newSection.Columns.Count == 0)
            {
                CurrColumn = new Column(m_document);
                CurrColumn.Space = 36;
                newSection.Columns.Add(CurrColumn);
            }
            m_document.ChildEntities.Add(newSection );
            m_textBody = m_document.LastSection.Body;
        }
        /// <summary>
        /// Extract the Twips value from the token
        /// </summary>
        /// <returns></returns>
        private float ExtractTwipsValue(string nValue)
        {
            int twipsValue = GetIntValue(nValue);
            float value = Convert.ToSingle ((twipsValue/20.0));
            return value;
        }
       /// <summary>
       /// Sort Tab Collection based on Tab position
       /// </summary>
        private void SortTabCollection()
        {
            TabFormat temp;
            for (int i = 1; i < m_tabCollection.Count; i++)
            {
                for (int j = i + 1; j < m_tabCollection.Count + 1; j++)
                {
                    if (m_tabCollection[i].TabPosition > m_tabCollection[j].TabPosition)
                    {
                        temp = m_tabCollection[i];
                        m_tabCollection[i] = m_tabCollection[j];
                        m_tabCollection[j] = temp;
                    }
                }
            }
        }
       /// <summary>
       /// Seperate Token keyword from the token Value
       /// </summary>
       /// <param name="token"></param>
       /// <returns></returns>
        private string[] SeperateToken(string token)
        {
            string[] value = new string[2];
            char ch;
            for (int i = 0; i < token.Length; i++)
            {
                ch = token[i];
                if (Char.IsDigit(ch))
                    value[1] += ch;
                else 
                    value[0] += ch;             
            }
            return value;


        }

       /// <summary>
       /// Copy Textformating to the character format
       /// </summary>
       /// <param name="charFormat"></param>
       /// <param name="textFormat"></param>
        private void CopyTextFormatToCharFormat(WCharacterFormat charFormat,TextFormat textFormat)
        {
            if (textFormat.FontSize > 0)
                charFormat.FontSize = textFormat.FontSize;
            charFormat.TextColor = textFormat.FontColor;               
            if (textFormat.FontFamily.Length > 0)
            {
                charFormat.FontName = textFormat.FontFamily;
            }
            else if (m_currentTableType == RtfTableType.None)
            {
                //Update Default Font
                foreach (KeyValuePair<string, RtfFont> fontTable in m_fontTable)
                {
                    string[] fontKey = fontTable.Key.Split('f');
                    if (fontKey[fontKey.Length - 1] == DefaultFontIndex.ToString())
                    {
                        charFormat.FontName = fontTable.Value.FontName;
                    }
                }
            }
            if (textFormat.CharacterStyleName != string.Empty)
                charFormat.CharStyleName = textFormat.CharacterStyleName;
            charFormat.LocaleIdASCII = textFormat.LocalIdASCII;
            charFormat.LocaleIdFarEast = textFormat.LocalIdForEast;
            charFormat.Position = textFormat.Position;
            if (textFormat.Bold != ThreeState.Unknown)
            {
                if (textFormat.Bold == ThreeState.True)
                    charFormat.Bold = true;
                else
                    charFormat.Bold = false;
                charFormat.UpdateComplexProperty(WCharacterFormat.BoldKey, charFormat.Bold);
            }
            
            if (textFormat.Italic != ThreeState.Unknown)
            {
                if (textFormat.Italic == ThreeState.True)
                    charFormat.Italic = true;
                else
                    charFormat.Italic = false;
                charFormat.UpdateComplexProperty(WCharacterFormat.ItalicKey, charFormat.Italic);
            }

            if (textFormat.Underline != ThreeState .Unknown )
            {
                if (textFormat.Underline == ThreeState.True)
                    charFormat.UnderlineStyle = UnderlineStyle.Single;
                else
                    charFormat.UnderlineStyle = UnderlineStyle.None;
                
            }
            if (textFormat.BackColor != Color.Empty)
                charFormat.TextBackgroundColor = textFormat.BackColor;
            if (textFormat.HighlightColor != Color.Empty)
                charFormat.HighlightColor = textFormat.HighlightColor;
            charFormat.UnderlineStyle = textFormat.UnderlineStyle;               
            if(textFormat .Shadow ==true)
               charFormat.Shadow = textFormat.Shadow;

            if (textFormat.IsHiddenText == true)
                charFormat.Hidden = true;

            if (textFormat.SubSuperScript != SubSuperScript.None)
                charFormat.SubSuperScript = textFormat.SubSuperScript;

            if (textFormat.Strike != ThreeState.Unknown)
            {
                if (textFormat.Strike==ThreeState .True )
                    charFormat.Strikeout = true;
                else
                    charFormat.Strikeout = false;
                charFormat.UpdateComplexProperty(WCharacterFormat.StrikeKey, charFormat.Strikeout);
            }
            if (textFormat.DoubleStrike != ThreeState.Unknown)
            {
                if (textFormat.DoubleStrike ==ThreeState .True )
                    charFormat.DoubleStrike  = true;
                else
                    charFormat.DoubleStrike  = false;
                charFormat.UpdateComplexProperty(WCharacterFormat.DoubleStrikeKey, charFormat.DoubleStrike);
            }
            if (textFormat.Emboss  != ThreeState.Unknown)
            {
                if (textFormat.Emboss  == ThreeState.True)
                    charFormat.Emboss  = true;
                else
                    charFormat.Emboss = false;
                charFormat.UpdateComplexProperty(WCharacterFormat.EmbossKey, charFormat.Emboss);
            }
            if (textFormat.Engrave  != ThreeState.Unknown)
            {
                if (textFormat.Engrave  == ThreeState.True)
                    charFormat.Engrave  = true;
                else
                    charFormat.Engrave  = false;
                charFormat.UpdateComplexProperty(WCharacterFormat.EngraveKey, charFormat.Engrave);
            }
            if (textFormat.AllCaps != ThreeState.Unknown)
            {
                if (textFormat.AllCaps == ThreeState.True)
                    charFormat.AllCaps = true;
                else if (textFormat.AllCaps == ThreeState.False)
                    charFormat.AllCaps = false;
                charFormat.UpdateComplexProperty(WCharacterFormat.AllCapsKey, charFormat.AllCaps);
            }
            if (textFormat.SmallCaps  != ThreeState.Unknown)
            {
                if (textFormat.SmallCaps  == ThreeState.True)
                    charFormat.SmallCaps = true;
                else if (textFormat.SmallCaps  == ThreeState.False)
                    charFormat.SmallCaps  = false;
                charFormat.UpdateComplexProperty(WCharacterFormat.SmallCapsKey, charFormat.SmallCaps);
            }
        }
       /// <summary>
       /// Apply font for the current paragraph
       /// </summary>
        private void ApplyParagraphFont(RtfFont rtfFontTable)
        {
            m_currTextFormat.FontFamily = rtfFontTable.FontName;
           
        }
        /// <summary>
        /// Apply font color for the current text
        /// </summary>
        /// <param name="rtfColor"></param>
        private void ApplyColorTable(RtfColor rtfColor)
        {
            m_currTextFormat.FontColor = Color.FromArgb(rtfColor.RedN, rtfColor.GreenN, rtfColor.BlueN);

        }
        #endregion


        #region InternalClass
        /// <summary>
        /// Text format class
        /// </summary>
        internal class TextFormat
        {
            /// <summary>
            /// Specifies Vertical position of the Character
            /// </summary>
            private float m_position;
            #region Properties
            /// <summary>
            /// Specifies Bold format.
            /// </summary>
            internal ThreeState Bold;
            /// <summary>
            /// Specifies Italic format.
            /// </summary>
            internal ThreeState Italic;
            /// <summary>
            /// Specifies Underline format.
            /// </summary>
            internal ThreeState Underline;
            /// <summary>
            /// Specifies Strike format.
            /// </summary>
            internal ThreeState Strike;
            /// <summary>
            /// Specifies DoubleStrike format.
            /// </summary>
            internal ThreeState DoubleStrike;
            /// <summary>
            /// Specifies Emboss format.
            /// </summary>
            internal ThreeState Emboss;
            /// <summary>
            /// Specifies Engrave format.
            /// </summary>
            internal ThreeState Engrave;
            /// <summary>
            /// Specifies SubSuperscript Format
            /// </summary>
            internal SubSuperScript SubSuperScript;
            /// <summary>
            /// Specifies font color of the text.
            /// </summary>
            internal Color FontColor;
            /// <summary>
            /// Specifies back color of the text.
            /// </summary>
            internal Color BackColor;
            /// <summary>
            /// Specifies Hightlight color of the text.
            /// </summary>
            private Color m_highlightColor;
            /// <summary>
            /// Specifies the font family.
            /// </summary>
            internal string FontFamily;
            /// <summary>
            /// Specifies the font size.
            /// </summary>
            internal float FontSize;
            /// <summary>
            /// Specifies the text alignment.
            /// </summary>
            internal HorizontalAlignment TextAlign;
            /// <summary>
            /// Specifies the text style.
            /// </summary>
            internal BuiltinStyle Style;
            internal ThreeState Bidi;
            internal ThreeState AllCaps;
            internal ThreeState SmallCaps;

            internal UnderlineStyle UnderlineStyle;
            internal bool Shadow ;
            internal bool IsHiddenText;
            internal string CharacterStyleName;
            private short m_localIdASCII;
            private short m_localIdForEast;
            /// <summary>
            /// Gets or sets the ASCII locale id .
            /// </summary>
            /// <value>The ASCII locale id .</value>
            internal short LocalIdASCII
            {
                get
                {
                    return m_localIdASCII;
                }
                set
                {
                    m_localIdASCII = value;
                }
            }
            /// <summary>
            /// Gets or sets the far east locale id .
            /// </summary>
            /// <value>The far east locale id .</value>
            internal short LocalIdForEast
            {
                get
                {
                    return m_localIdForEast;
                }
                set
                {
                    m_localIdForEast = value;
                }
            }
            /// <summary>
            /// Specifies Vertical position of the Character
            /// </summary>
            internal float Position
            {
                get
                {
                    return m_position;
                }
                set
                {
                    m_position = value;
                }
            }
            /// <summary>
            /// Specifies Highlight color of the Text
            /// </summary>
            internal Color HighlightColor
            {
                get
                {
                    return m_highlightColor;
                }
                set
                {
                    m_highlightColor = value;
                }
            }
            #endregion

            internal TextFormat()
            {
                FontSize = 0;
                Bold = ThreeState.False;
                Italic = ThreeState.False;
                Underline = ThreeState.False;
                Strike = ThreeState.False;
                DoubleStrike = ThreeState.False;
                Emboss = ThreeState.False;
                Engrave = ThreeState.False;
                FontColor = Color.Empty;
                BackColor = Color.Empty;
                HighlightColor = Color.Empty;
                FontFamily = string.Empty;
                TextAlign = HorizontalAlignment.Left;
                Style = BuiltinStyle.Normal;
                Bidi = ThreeState.Unknown;
                UnderlineStyle = UnderlineStyle.None;
                SubSuperScript = SubSuperScript.None;
                AllCaps = ThreeState.False;
                SmallCaps = ThreeState.False;
                CharacterStyleName = string.Empty;
                Position = 0;
                LocalIdASCII = (short)1033;
                LocalIdForEast = (short)1033;

            }

            #region Methods
            /// <summary>
            /// Clones this instance.
            /// </summary>
            /// <returns></returns>
            public TextFormat Clone()
            {
                return (TextFormat)MemberwiseClone();
            }
            #endregion
        }

        /// <summary>
        /// Section Format class
        /// </summary>
        internal class SecionFormat
        {
            #region Fields
            private float m_leftMargin = -1;
            private float m_rightMargin = -1;
            private float m_topMargin = -1;
            private float m_bottomMargin = -1;
            internal float HeaderDistance = Convert.ToSingle((720 / 20));
            internal float FooterDistance = Convert.ToSingle((720 / 20));
            internal bool DifferentFirstPage = false;
            internal bool DifferentOddAndEvenPage = false;
            internal bool IsFrontPageBorder = false;
            internal float DefaultTabWidth = Convert.ToSingle((720 / 20));
            internal PageAlignment VertAlignment = PageAlignment.Top;
            internal SizeF PageSize = new SizeF();
            internal PageOrientation PageOrientation = PageOrientation.Portrait;
                
            #endregion

            #region Properties
            internal float LeftMargin
            {
                get
                {
                    return m_leftMargin;
                }
                set
                {
                    m_leftMargin = value;
                }
            }
            internal float RightMargin
            {
                get
                {
                    return m_rightMargin;
                }
                set
                {
                    m_rightMargin = value;
                }
            }
            internal  float TopMargin
            {
                get
                {
                    return m_topMargin;
                }
                set
                {
                    m_topMargin = value;
                }
            }
            internal  float BottomMargin
            {
                get
                {
                    return m_bottomMargin;
                }
                set
                {
                    m_bottomMargin = value;
                }
            }
            #endregion

        }

       /// <summary>
       /// Picture format class
       /// </summary>
        internal class PictureFormat
        {
            #region fields
            internal  float Height = 0;
            internal float Width = 0;
            internal float HeightScale = 0;
            internal float WidthScale = 0;
            internal int PicW;
            internal int picH;
            #endregion
        }

        /// <summary>
       /// Shape Format Class
       /// </summary>
        internal class ShapeFormat
        {
            #region Fields
            internal float m_width;
            internal float m_height;
            internal float m_left;
            internal float m_right;
            internal float m_top;
            internal float m_bottom;
            internal float m_horizPosition;
            internal float m_vertPosition;
            internal int m_uniqueId;
            internal int m_zOrder;
            internal bool m_isBelowText;
            internal bool m_isInHeader;
            internal bool m_isLockAnchor;
            internal ShapeHorizontalAlignment m_horizAlignment;
            internal TextWrappingType m_textWrappingType;
            internal TextWrappingStyle m_textWrappingStyle;
            internal VerticalOrigin m_vertOrgin;
            internal HorizontalOrigin m_horizOrgin;
            private SizeF m_size;
            #endregion           

            #region Properties
            /// <summary>
            /// Gets the Size of Shape
            /// </summary>
            internal SizeF Size 
            {
                get 
                {
                    return GetSize();
                }
            }
            #endregion

            #region Methods
            /// <summary>
            /// Returns the Size of shape
            /// </summary>            
            private SizeF GetSize() 
            {
                m_width = (m_right - m_left) / 20;
                m_height = (m_bottom - m_top) / 20;
                
                m_size = new SizeF(m_width, m_height);

                return m_size;
            }
            #endregion
        }
      
        internal enum ThreeState
        {
            False = 0,
            True = 1,
            Unknown = 2
        }

#endregion


    }

        #region Helperclass
   /// <summary>
   /// Represents the form fields information
   /// </summary>
   internal class FormFieldData
   {
       #region fields
       /// <summary>
       /// represents the value of ffname token
       /// </summary>
       private string m_name;
       /// <summary>
       /// Represents the value of ffhelptext
       /// </summary>
       private string m_helpText;
       /// <summary>
       /// Represents the value of ffstattext
       /// </summary>
       private string m_statusHelpText;
       /// <summary>
       /// Represents the field type w.r.t the value of the fftypeN
       /// </summary>
       private FormFieldType m_formFieldType;
       /// <summary>
       /// Represents the value of ffrecalcN
       /// </summary>
       private bool m_bCalculateOnExit;
       /// <summary>
       /// Represents the value of ffentrymcr
       /// </summary>
       private string m_marcoOnStart;
       /// <summary>
       /// Represents the value of ffexitmcr
       /// </summary>
       private string m_marcoOnEnd;
       /// <summary>
       /// Represents the value of ffprotN
       /// </summary>
       private bool m_enabled = true;
       /// <summary>
       /// Represents the value of ffhpsN
       /// </summary>
       private int m_checkboxSize;
       /// <summary>
       /// Represents the checkbox type specified by the token ffsizeN
       /// </summary>
       private CheckBoxSizeType m_checkboxSizeType = CheckBoxSizeType.Auto;
       /// <summary>
       /// Represents the value of ffdeftext
       /// </summary>
       private string m_defaultText;
       /// <summary>
       /// Represents the value of ffformat
       /// </summary>
       private string m_stringFormat;
       /// <summary>
       /// Represents the value of ffmaxlenN
       /// </summary>
       private int m_maxLength;
       /// <summary>
       /// Represents the collection of values of ffl control word
       /// </summary>
       internal WDropDownCollection DropDownItems;
       /// <summary>
       /// Represents the value of ffhaslistboxN
       /// </summary>
       private bool m_bIsListBox;
       /// <summary>
       /// Represents the value of ffres
       /// </summary>
       internal int Ffres;
       /// <summary>
       /// Respresents the value of ffdefres
       /// </summary>
       internal int Ffdefres;
       /// <summary>
       /// Respresents the value of ffdefres for check box
       /// </summary>
       internal bool m_bIsChecked;
       #endregion fields

       #region Properties
       /// <summary>
       /// Gets/Sets the checked status of the check box
       /// </summary>
       internal bool IsChecked
       {
           get
           {
               return m_bIsChecked;
           }
           set
           {
               m_bIsChecked = value;
           }
       }
       /// <summary>
       /// Gets or sets
       /// </summary>
       internal bool IsListBox
       {
           get
           {
               return m_bIsListBox;
           }
           set
           {
               m_bIsListBox = value;
           }
       }
       /// <summary>
       /// Gets or sets the maximum number of character allowed in text form field.
       /// </summary>
       internal int MaxLength
       {
           get
           {
               return m_maxLength;
           }
           set
           {
               m_maxLength = value;
           }
       }
       /// <summary>
       /// Gets or sets the string format
       /// </summary>
       internal string StringFormat
       {
           get
           {
               return m_stringFormat;
           }
           set
           {
               m_stringFormat = value;
           }
       }
       /// <summary>
       /// Gets or sets the default text for the textform field
       /// </summary>
       internal string DefaultText
       {
           get
           {
               return m_defaultText;
           }
           set
           {
               m_defaultText = value;
           }
       }
       /// <summary>
       /// Gets or sets the checkbox size type (Auto/Exactly)
       /// </summary>
       internal CheckBoxSizeType CheckboxSizeType
       {
           get
           {
               return m_checkboxSizeType;
           }
           set
           {
               m_checkboxSizeType = value;
           }
       }
       /// <summary>
       /// Gets or sets the checkbox size. This size will be reflected only when the checkbox size is of Exactly type
       /// </summary>
       internal int CheckboxSize
       {
           get
           {
               return m_checkboxSize;
           }
           set
           {
               m_checkboxSize = value;
           }
       }
       /// <summary>
       /// Gets or sets the value which protects this form field
       /// </summary>
       internal bool Enabled
       {
           get
           {
               return m_enabled;
           }
           set
           {
               m_enabled = value;
           }
       }
       /// <summary>
       /// Gets or sets the Macro name to execute upon exit into this form fie
       /// </summary>
       internal string MacroOnExit
       {
           get
           {
               return m_marcoOnEnd;
           }
           set
           {
               m_marcoOnEnd = value;
           }
       }
       /// <summary>
       /// Gets or sets the Macro name to execute upon entry into this form fie
       /// </summary>
       internal string MarcoOnStart
       {
           get
           {
               return m_marcoOnStart;
           }
           set
           {
               m_marcoOnStart = value;
           }
       }
       /// <summary>
       /// Gets or ets the form field name.
       /// </summary>
       internal string Name
       {
           get
           {
               return m_name;
           }
           set
           {
               m_name = value;
           }
       }
       /// <summary>
       /// Gets or sets the help text string.
       /// </summary>
       internal string HelpText
       {
           get
           {
               return m_helpText;
           }
           set
           {
               m_helpText = value;
           }
       }
       /// <summary>
       /// Gets or sets the boolean value which represents whether the field should be calculated on exit or not
       /// </summary>
       internal bool CalculateOnExit
       {
           get
           {
               return m_bCalculateOnExit;
           }
           set
           {
               m_bCalculateOnExit = value;
           }
       }
       /// <summary>
       /// Gets or sets the status line text
       /// </summary>
       internal string StatusHelpText
       {
           get
           {
               return m_statusHelpText;
           }
           set
           {
               m_statusHelpText = value;
           }
       }
       /// <summary>
       /// Gets or sets the formfield type.
       /// </summary>
       internal FormFieldType FormFieldType
       {
           get
           {
               return m_formFieldType;
           }
           set
           {
               m_formFieldType = value;
           }
       }
       #endregion Properties
   }
   internal class RtfFont
   {
       #region fields
       private int m_fontNumber = 0;
       private string m_fontID=null ;      
       private string m_fontName =null ;
       private short m_fontCharSet = 1;
       private string m_alternateFontName = null;
       #endregion

       #region Properties

       internal int FontNumber
       {
           get
           {
               return m_fontNumber;
           }
           set
           {
               m_fontNumber = value;
           }
       }
       /// <summary>
       /// Get/Set the alternate font name to use if the font specified in the font table is not available. 
       /// </summary>
       internal string AlternateFontName
       {
           get
           {
               return m_alternateFontName;
           }
           set
           {
               m_alternateFontName = value;
           }
       }
       internal string FontID
       {
           get
           {
               return m_fontID;
           }
           set
           {
               m_fontID = value;
           }
       }
       internal string FontName
       {
           get
           {
               return m_fontName ;
           }
           set
           {
               m_fontName =value ;
           }
       }  
       internal short FontCharSet
       {
           get
           {
               return m_fontCharSet;
           }
           set
           {
               m_fontCharSet = value;
           }
       }
       #endregion
   }
  
   internal class TabFormat
   {
       #region Fields
       private float m_tabPosition=36;
       private TabJustification m_tabJustification = TabJustification.Left;
       private TabLeader m_tabLeader = TabLeader.NoLeader;
       #endregion

       #region Properties
       /// <summary>
       /// Gets and Sets the tab position
       /// </summary>
       internal float TabPosition 
       {
           get
           {
               return m_tabPosition;
           }
           set
           {
               m_tabPosition = value;
           }
       }
       /// <summary>
       /// Gets and Sets the justification for the tab
       /// </summary>
       internal TabJustification TabJustification
       {
           get
           {
              return  m_tabJustification;
           }
           set
           {
               m_tabJustification  = value;
           }
       }
       /// <summary>
       /// Gets and sets the tab leader
       /// </summary>
       internal TabLeader TabLeader
       {
           get
           {
               return m_tabLeader;
           }
           set
           {
               m_tabLeader = value;
           }
           
       }
       #endregion
   }


 
   internal class RtfColor
   {
       #region fields
       private int m_redN = 0;
       private int m_greenN = 0;
       private int m_blueN = 0;
       private int m_ctintN = 0;
       private int m_cshadeN = 0;
       #endregion

       #region properties
       /// <summary>
       /// Gets and Sets the Red component value
       /// </summary>
       internal int RedN
       {
           get
           {
               return m_redN;
           }
           set
           {
               m_redN = value;
           }
       }
       /// <summary>
       /// Gets and Sets the green component value
       /// </summary>
       internal int GreenN
       {
           get
           {
               return m_greenN;
           }
           set
           {
               m_greenN = value;
           }
       }
       /// <summary>
       /// Gets and Sets the blue component value
       /// </summary>
       internal int BlueN
       {
           get
           {
               return m_blueN;
           }
           set
           {
               m_blueN = value;
           }
       }
       #endregion
   }


   /// <summary>
   /// Represents the details for the table representation based on the paragraph level.
   /// </summary>
   internal struct PrepareTableInfo
   {
       internal bool InTable;
       internal int Level;
       internal int PrevLevel;
       internal Syncfusion.DocIO.DLS.DocReaderAdapterBase.PrepareTableState State;
       /// <summary>
       /// Updates the specified reader.
       /// </summary>
       /// <param name="reader">The reader.</param>
       /// <param name="prevLevel">The prev level.</param>
       internal PrepareTableInfo(bool inTable, int currLevel, int prevLevel)
       {
           this.InTable = inTable;
           PrevLevel = prevLevel;
           Level = currLevel;

           if (Level > PrevLevel)
           {
               State = Syncfusion.DocIO.DLS.DocReaderAdapterBase.PrepareTableState.EnterTable;
           }
           else if (Level < PrevLevel)
           {
               State = Syncfusion.DocIO.DLS.DocReaderAdapterBase.PrepareTableState.LeaveTable;
           }
           else
           {
               State = Syncfusion.DocIO.DLS.DocReaderAdapterBase.PrepareTableState.NoChange;
           }
       }
   }
        # endregion

}
  
  

