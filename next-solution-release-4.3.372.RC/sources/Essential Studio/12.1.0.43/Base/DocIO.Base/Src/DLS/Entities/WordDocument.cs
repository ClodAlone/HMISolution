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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
#if !WINRT
#if !WP
using System.Drawing;
#endif
using System.Security.Permissions;
#endif
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using Syncfusion.DocIO.DLS.XML;
using Syncfusion.DocIO.ReaderWriter;
using Syncfusion.DocIO.ReaderWriter.Biff_Records.Structures;
using Syncfusion.DocIO.ReaderWriter.DataStreamParser.Escher;
using Syncfusion.Layouting;
using Syncfusion.DocIO.DLS.Convertors;
using Syncfusion.CompoundFile.DocIO;
using Syncfusion.DocIO.ReaderWriter.Security;
using Syncfusion.DocIO.ReaderWriter.Biff_Records;

#if SILVERLIGHT || WP
using Image = Syncfusion.DocIO.DLS.Entities.Image;
#if WINRT || WP
using System.Threading.Tasks;
using Windows.Storage;
#endif
#else
using Image = System.Drawing.Image;
using System.Web;
using System.Collections.Specialized;
#endif

#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents an MS Word document.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class WordDocument
      : WidgetContainer,
      IWordDocument
#if !SILVERLIGHT && !WP
      ,IXmlSerializable
#endif
      ,IWidgetContainer
    {
        #region Constants
        /// <summary>
        /// 
        /// </summary>
        private const string DEF_NORMAL_STYLE = "Normal";

        /// <summary>
        /// 
        /// </summary>
        internal const string DEF_BULLETS_STYLE = "Bulleted";

        /// <summary>
        /// 
        /// </summary>
        internal const string DEF_NUMBERING_STYLE = "Numbered";
        #endregion

        #region Fields
        internal bool IsOpening = false;
        internal bool IsMailMerge = false;
        internal bool IsCloning = false;
        /// <summary>
        /// Determines whether to create base paragraph style - Normal.
        /// Handled to avoid repeated looping, while creating base paragraph style - Normal (Recursive call).
        /// </summary>
        internal bool CreateBaseStyle = true;
        /// <summary>
        /// Determines whether the Normal style is defined or not.
        /// </summary>
        internal bool IsNormalStyleDefined;
        internal TextBodyItem m_prevClonedEntity = null;
        private FormatType m_actualFormatType;
        /// <summary>
        /// 
        /// </summary>
        internal BuiltinDocumentProperties m_builtinProp = new BuiltinDocumentProperties();

        /// <summary>
        /// 
        /// </summary>
        internal CustomDocumentProperties m_customProp = new CustomDocumentProperties();

        /// <summary>
        /// Collection of document sections
        /// </summary>
        protected WSectionCollection m_sections;

        /// <summary>
        /// Collection of document styles
        /// </summary>
        protected IStyleCollection m_styles;

        /// <summary>
        /// Collection of list styles
        /// </summary>
        protected ListStyleCollection m_listStyles;

        /// <summary>
        /// 
        /// </summary>
        private ListOverrideStyleCollection m_listOverrides;

        /// <summary>
        /// Collection of bookmarks
        /// </summary>
        private BookmarkCollection m_bookmarks = null;
        /// <summary>
        /// Collection of fields
        /// </summary>
        private FieldCollection m_fields = null;
        /// <summary>
        /// Collection of textboxes
        /// </summary>
        private TextBoxCollection m_txbxItems = null;

        /// <summary>
        /// Collection of Comments.
        /// </summary>
        private CommentsCollection m_Comments = null;

        /// <summary>
        /// default value for DefaultTabWidth
        /// </summary>
        private float m_defaultTabWidth = 36f;

        /// <summary>
        /// 
        /// </summary>
        private MailMerge m_mailMerge;
        /// <summary>
        /// 
        /// </summary>
        private ViewSetup m_viewSetup;

        /// <summary>
        /// Document's watermark;
        /// </summary>
        private Watermark m_watermark;

        /// <summary>
        /// 
        /// </summary>
        private Background m_background;

        /// <summary>
        /// 
        /// </summary>
        private DOPDescriptor m_dop;

        /// <summary>
        /// 
        /// </summary>
        private GrammarSpelling m_grammarSpellingData;

        /// <summary>
        /// 
        /// </summary>
        private EscherClass m_escher;

        /// <summary>
        /// 
        /// </summary>
        private string m_password;

        /// <summary>
        /// 
        /// </summary>
        private byte[] m_macrosData;

        /// <summary>
        /// 
        /// </summary>
        private byte[] m_escherDataContainers;

        /// <summary>
        /// 
        /// </summary>
        private byte[] m_escherContainers;

        /// <summary>
        /// 
        /// </summary>
        private byte[] m_macroCommands;

        /// <summary>
        /// 
        /// </summary>
        private int m_defShapeId = 1;

        /// <summary>
        /// 
        /// </summary>
        private string m_standardAsciiFont;

        /// <summary>
        /// 
        /// </summary>
        private string m_standardFarEastFont;

        /// <summary>
        /// 
        /// </summary>
        private string m_standardNonFarEastFont;
        /// <summary>
        /// 
        /// </summary>
        private string m_standardBidiFont;
        /// <summary>
        /// 
        /// </summary>
        private bool m_throwUnsupportedExceptions = false;

        /// <summary>
        /// 
        /// </summary>
        private WSection m_curClonedSection;

        static readonly object m_threadLocker = new object();
#if !SILVERLIGHT && !WP
        /// <summary>
        /// 
        /// </summary>
        private XHTMLValidationType m_htmlValidationOption = XHTMLValidationType.Transitional;
#endif
        /// <summary>
        /// 
        /// </summary>
#if !SILVERLIGHT && !WP
        private XmlNode m_latentStyles;
#else
        private Stream m_latentStyles;
#endif
        private MemoryStream m_latentStyles2010;
        /// <summary>
        /// 
        /// </summary>
        private WCharacterFormat m_defCharFormat;

        /// <summary>
        /// 
        /// </summary>
        internal WParagraphFormat m_defParaFormat;

        /// <summary>
        /// Object which holds structured docx format objects with data stream.
        /// </summary>
        private Package m_docxPackage;
        /// <summary>
        /// 
        /// </summary>
        private ImportOptions m_importOption = ImportOptions.UseDestinationStyles;
        /// <summary>
        /// 
        /// </summary>
        private bool m_importStyles = true;

        /// <summary>
        /// 
        /// </summary>
        private DocVariables m_variables;

        /// <summary>
        /// 
        /// </summary>
        private DocProperties m_props;

        /// <summary>
        /// 
        /// </summary>
        private bool m_bReplaceFirst = false;

        /// <summary>
        /// Field is used for FindNext functionality to define next
        /// paragraph item.
        /// </summary>
        private ParagraphItem m_nextParaItem;

        /// <summary>
        /// 
        /// </summary>
        private TextBodyItem m_prevBodyItem;

        /// <summary>
        /// 
        /// </summary>
        private SaveOptions m_saveOptions;

        /// <summary>
        /// Non parsed properties read from docx file.
        /// </summary>
        private List<Stream> m_docxProps;
        /// <summary>
        /// 
        /// </summary>
        internal bool m_isReadOnly;
        /// <summary>
        /// 
        /// </summary>
        private SttbfAssoc m_assocStrings;
        /// <summary>
        /// 
        /// </summary>
        private bool m_isEvalExpired;
        /// <summary>
        /// 
        /// </summary>
        private bool m_isWarnInserted;
        /// <summary>
        /// Defines whether document is encrypted
        /// </summary>
        private bool m_isEncrypted;
        /// <summary>
        /// 
        /// </summary>
        private bool m_updateFields;
        /// <summary>
        /// Specifies that applications should provide user interface 
        /// recommending that the user open this document in write protected state.
        /// </summary>
        private bool m_isWriteProtected;
        /// <summary>
        /// Stores the IDs of containers inside object pool
        /// </summary>
        private List<string> m_objPoolContainers;
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, string> m_styleNameIds;
        /// <summary>
        /// 
        /// </summary>
        private bool m_bIsClosing;
        /// <summary>
        /// paragraphs count in document
        /// </summary>
        private int m_paraCount;
        /// <summary>
        /// words count in document
        /// </summary>
        private int m_wordCount;
        /// <summary>
        /// characters count in document
        /// </summary>
        private int m_charCount;
        /// <summary>
        /// 
        /// </summary>
        private bool m_hasPicture;
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, string> m_fontSubstitutionTable;
        /// <summary>
        /// 
        /// </summary>
        private string m_htmlBaseUrl = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        private TableOfContent m_tableOfContent = null;
        /// <summary>
        /// 
        /// </summary>
        private List<Font> m_usedFonts;
        /// <summary>
        ///
        /// </summary>
        private Settings m_settings;
        private Stream m_vbaProject;
        private PartContainer m_CustomUIPartContainer;
        private PartContainer m_CustomXMLContainer;
        private List<MacroData> m_vbaData;
        private List<string> m_docEvents;
        private FormatType m_saveFormatType;
        private ushort m_wordVersion;
        private Stack<WField> m_clonedFields;
        internal List<WField> UpdatedFields = new List<WField>();
        private int m_altChunkCount = 0;
        private ImageCollection m_imageCollection;
        /// <summary>
        /// True if the document has different headers and footers 
        /// for odd-numbered and even-numbered pages. 
        /// </summary>
        private bool m_differentOddEvenPages;
        private Footnote m_footnotes=null;
        private Endnote m_endnotes=null;
        private Dictionary<string, string> m_listStyleNames;
        /// <summary>
        /// 
        /// </summary>
        private FontFamilyNameStringTable m_ffnStringTable;
        private Dictionary<string,Color> m_SchemeColor;
        private HTMLImportSettings m_htmlImportSettings;
#if !SILVERLIGHT && !WP
        private Dictionary<string, Dictionary<int, int>> m_lists;
        private HybridDictionary m_listNames;
        private Dictionary<string, int> m_previousListLevel;
        private List<string> m_previousListLevelOverrideStyle;
        internal int PageCount = 0;
#endif
        #endregion

        #region Properties
        internal Dictionary<string,Color> SchemeColor
        {
            get {
                if (m_SchemeColor == null)
                    m_SchemeColor = new Dictionary<string, Color>();
                return m_SchemeColor; }
            set
            {
                m_SchemeColor = value;
            }
        }
        /// <summary>
        /// Gets or sets the footnote separators.
        /// </summary>
        /// <value>The footnotes.</value>
        public Footnote Footnotes
        {
            get
            {
                if (m_footnotes == null)
                    m_footnotes = new Footnote(this);
                return m_footnotes;
            }
            set
            {
                m_footnotes = value;
                if (m_footnotes != null)
                    m_footnotes.SetOwner(this);
            }
        }
        /// <summary>
        /// Gets or sets the endnote separators.
        /// </summary>
        /// <value>The endnotes.</value>
        public Endnote Endnotes
        {
            get
            {
                if (m_endnotes == null)
                    m_endnotes = new Endnote(this);
                return m_endnotes;
            }
            set
            {
                m_endnotes = value;
                if (m_endnotes != null)
                    m_endnotes.SetOwner(this);
            }
        }
        /// <summary>
        /// True if the document has different headers and footers 
        /// for odd-numbered and even-numbered pages. 
        /// </summary>
        internal bool DifferentOddAndEvenPages
        {
            get
            {
                return m_differentOddEvenPages;
            }
            set
            {
                m_differentOddEvenPages = value;
            }
        }

        /// <summary>
        /// Gets or sets the default tab stop value.
        /// </summary>
        /// <value>The width of the default tab.</value>
        public float DefaultTabWidth
        {
            get
            {
                return m_defaultTabWidth;
            }
            set
            {
                m_defaultTabWidth = value;
            }
        }
        /// <summary>
        /// Gets or sets the word version based on the nFib and nFibNew.
        /// </summary>
        /// <value>The word version.</value>
        internal ushort WordVersion
        {
            get
            {
                return m_wordVersion;
            }
            set
            {
                m_wordVersion = value;
            }
        }
        internal List<Font> UsedFontNames
        {
            get
            {
                if (m_usedFonts == null)
                    m_usedFonts = new List<Font>();
                return m_usedFonts;
            }
            set
            {
                m_usedFonts = value;
            }
        }
        /// <summary>
        /// Returns Whether the document has Table of Contents or not.
        /// </summary>
        internal bool HasTOC
        {
            get
            {
                return (m_tableOfContent != null);
            }
        }
        /// <summary>
        /// Returns the TOC element of the word document.
        /// </summary>
        internal TableOfContent TOC
        {
            get
            {
                if (HasTOC)
                    return m_tableOfContent;
                else
                    return null;
            }
            set
            {
                m_tableOfContent = value;
            }
        }
        /// <summary>
        /// Gets or sets the Base path which is used to convert the relative path to absolute path.
        /// </summary>
        internal string HtmlBaseUrl
        {
            get
            {
                return m_htmlBaseUrl;
            }
            set
            {
                m_htmlBaseUrl = value;
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
                return EntityType.WordDocument;
            }
        }

        /// <summary>
        /// Gets document built-in properties object.
        /// </summary>
        public BuiltinDocumentProperties BuiltinDocumentProperties
        {
            get
            {
                return m_builtinProp;
            }
        }

        /// <summary>
        ///Gets the attached template.
        /// </summary>
        public Template AttachedTemplate
        {
            get
            {
                return new Template(AssociatedStrings);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to automatically update styles of the document from the attached template each time the document is opened.
        /// </summary>
        public bool UpdateStylesOnOpen 
        {
            get
            {
                return m_dop.LinkStyles;
            }
            set
            {
                m_dop.LinkStyles = value;
            }
        }

        /// <summary>
        /// Gets document custom properties object.
        /// </summary>
        public CustomDocumentProperties CustomDocumentProperties
        {
            get
            {
                return m_customProp;
            }
        }
        /// <summary>
        /// Gets document sections.
        /// </summary>
        public WSectionCollection Sections
        {
            get
            {
                return m_sections;
            }
        }

        /// <summary>
        /// Gets document styles.
        /// </summary>
        public IStyleCollection Styles
        {
            get
            {
                return m_styles;
            }
        }

        /// <summary>
        /// Gets document list styles.
        /// </summary>
        public ListStyleCollection ListStyles
        {
            get
            {
                if (m_listStyles.Count == 0)
                {
                    CreateDefListStyles();
                }

                return m_listStyles;
            }
        }

        /// <summary>
        /// Gets document bookmarks.
        /// </summary>
        public BookmarkCollection Bookmarks
        {
            get
            {
                if (m_bookmarks == null)
                {
                    m_bookmarks = new BookmarkCollection(this);
                }
                return m_bookmarks;
            }
        }
        /// <summary>
        /// Gets document fields.
        /// </summary>
        internal FieldCollection Fields
        {
            get
            {
                if (m_fields == null)
                {
                    m_fields = new FieldCollection(this);
                }
                return m_fields;
            }
        }
        /// <summary>
        /// Get comments item of the document.
        /// </summary>
        public CommentsCollection Comments
        {
            get
            {
                if (m_Comments == null)
                {
                    m_Comments = new CommentsCollection(this);
                }
                return m_Comments;
            }
        }

        /// <summary>
        /// Get/set textbox items of main document
        /// </summary>
        public TextBoxCollection TextBoxes
        {
            get
            {
                return m_txbxItems;
            }
            set
            {
                m_txbxItems = value;
            }
        }
        private List<Shape> m_AutoShapeCollection = new List<Shape>();
        internal List<Shape> AutoShapeCollection
        {
            get
            {
                if (m_AutoShapeCollection == null)
                    m_AutoShapeCollection = new List<Shape>();
                return m_AutoShapeCollection;
            }
            set { m_AutoShapeCollection = value; }
        }

        /// <summary>
        /// Gets last section of the document.
        /// </summary>
        public WSection LastSection
        {
            get
            {
                int cnt = Sections.Count;

                if (cnt > 0)
                {
                    return Sections[cnt - 1];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets last section object.
        /// </summary>
        /// <value></value>
        public WParagraph LastParagraph
        {
            get
            {
                WSection sec = LastSection;

                if (sec != null)
                {
                    (sec.Body.Paragraphs as WParagraphCollection).ClearIndexes();
                    int pCount = sec.Body.Paragraphs.Count;

                    if (pCount > 0)
                    {
                        return sec.Body.Paragraphs[pCount - 1];
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets / sets endnote numbering format
        /// </summary>
        public FootEndNoteNumberFormat EndnoteNumberFormat
        {
            get
            {
                return (FootEndNoteNumberFormat)m_dop.EndnoteNumberFormat;
            }
            set
            {
                m_dop.EndnoteNumberFormat = (byte)value;
            }
        }

        /// <summary>
        /// Gets / sets footnote numbering format
        /// </summary>
        public FootEndNoteNumberFormat FootnoteNumberFormat
        {
            get
            {
                return (FootEndNoteNumberFormat)m_dop.FootnoteNumberFormat;
            }
            set
            {
                m_dop.FootnoteNumberFormat = (byte)value;
            }
        }

        /// <summary>
        /// Gets / sets the restart index for endnote
        /// </summary>
        public EndnoteRestartIndex RestartIndexForEndnote
        {
            get
            {
                return (EndnoteRestartIndex)m_dop.RestartIndexForEndnote;
            }
            set
            {
                m_dop.RestartIndexForEndnote = (byte)value;
            }
        }

        /// <summary>
        /// Gets / sets endnote position in the document
        /// </summary>
        public EndnotePosition EndnotePosition
        {
            get
            {
                return (EndnotePosition)m_dop.EndnotePosition;
            }
            set
            {
                m_dop.EndnotePosition = (byte)value;
            }
        }

        /// <summary>
        /// Gets / sets the restart index for footnotes
        /// </summary>
        public FootnoteRestartIndex RestartIndexForFootnotes
        {
            get
            {
                return (FootnoteRestartIndex)m_dop.RestartIndexForFootnotes;
            }
            set
            {
                m_dop.RestartIndexForFootnotes = (byte)value;
            }
        }

        /// <summary>
        /// Gets / sets footnote position in the document
        /// </summary>
        public FootnotePosition FootnotePosition
        {
            get
            {
                return (FootnotePosition)m_dop.FootnotePosition;
            }
            set
            {
                m_dop.FootnotePosition = (byte)value;
            }
        }

        /// <summary>
        /// Get/set document's watermark.
        /// </summary>
        public Watermark Watermark
        {
            get
            {
                return m_watermark;
            }
            set
            {
                ResetWatermark();
                m_watermark = value;

                if (m_watermark != null)
                {
                    UpdateWriteWatermark();
                    m_watermark.SetOwner(this);
                    if (m_watermark is PictureWatermark)
                    {
                        (m_watermark as PictureWatermark).WordPicture.SetOwner(this);
                        (m_watermark as PictureWatermark).UpdateImage();
                    }
                }
            }
        }

        /// <summary>
        /// Gets document's background
        /// </summary>
        public Background Background
        {
            get
            {
                return m_background;
            }
        }

        /// <summary>
        /// Gets mail merge engine.
        /// </summary>
        public MailMerge MailMerge
        {
            get
            {
                return m_mailMerge;
            }
        }

        /// <summary>
        /// Gets/sets the type of protection of the document.
        /// </summary>
        public ProtectionType ProtectionType
        {
            get
            {
                return m_dop.ProtectionType;
            }
            set
            {
                //SetProtection(value);
                m_dop.ProtectionType = value;
            }
        }

        /// <summary>
        /// Gets view setup options in MSWord.
        /// </summary>
        public ViewSetup ViewSetup
        {
            get
            {
                return m_viewSetup;
            }
        }

        /// <summary>
        /// Get / sets whether to throw exceptions for unsupported elements.
        /// </summary>
        public bool ThrowExceptionsForUnsupportedElements
        {
            get
            {
                return m_throwUnsupportedExceptions;
            }
            set
            {
                m_throwUnsupportedExceptions = value;
            }
        }

        /// <summary>
        /// Gets / sets the initial footnote number
        /// </summary>
        public int InitialFootnoteNumber
        {
            get
            {
                return m_dop.InitialFootnoteNumber;
            }
            set
            {
                m_dop.InitialFootnoteNumber = value;
            }
        }

        /// <summary>
        /// Gets / sets the initial endnote number
        /// </summary>
        public int InitialEndnoteNumber
        {
            get
            {
                return m_dop.InitialEndnoteNumber;
            }
            set
            {
                m_dop.InitialEndnoteNumber = value;
            }
        }

        /// <summary>
        /// Gets the child entities.
        /// </summary>
        /// <value>The child entities.</value>
        public EntityCollection ChildEntities
        {
            get
            {
                return m_sections as EntityCollection;
            }
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Gets or sets the HTML validate option.
        /// </summary>
        /// <value>The HTML validate option.</value>
        public XHTMLValidationType XHTMLValidateOption
        {
            get
            {
                return m_htmlValidationOption;
            }
            set
            {
                m_htmlValidationOption = value;
            }
        }
#endif
        /// <summary>
        /// Gets / sets image that represents background the image of a document.
        /// </summary>
        [ObsoleteAttribute]
#if SILVERLIGHT || WP
        public byte[] BackgroundImage
#else
		public Image BackgroundImage
#endif
        {
            get
            {
                return GetBackGndImage();
            }
            set
            {
                SetBackgroundImage(value);
            }
        }

        /// <summary>
        /// Gets or sets the document variables.
        /// </summary>
        /// <value>The variables.</value>
        public DocVariables Variables
        {
            get
            {
                if (m_variables == null)
                    m_variables = new DocVariables();
                return m_variables;
            }
        }

        /// <summary>
        /// Gets the document properties.
        /// </summary>
        /// <value>The properties.</value>
        public DocProperties Properties
        {
            get
            {
                if (m_props == null)
                {
                    m_props = new DocProperties(m_dop);
                }
                return m_props;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the document has tracked changes.
        /// </summary>
        /// <value>
        /// 	if the document has tracked changes, set to <c>true</c>.
        /// </value>
        public bool HasChanges
        {
            get
            {
                return HasTrackedChanges();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether tracking changes is turn on.
        /// </summary>
        /// <value>if track changes in on, set to <c>true</c>.</value>
        public bool TrackChanges
        {
            get
            {
                return m_dop.TrackChanges;
            }
            set
            {
                m_dop.TrackChanges = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether need first replacing.
        /// </summary>
        /// <value>If need first replacing, set to <c>true</c>.</value>
        public bool ReplaceFirst
        {
            get
            {
                return m_bReplaceFirst;
            }
            set
            {
                m_bReplaceFirst = value;
            }
        }

        /// <summary>
        /// Gets or sets the HTML Import Settings
        /// </summary>
        public HTMLImportSettings HTMLImportSettings
        {
            get
            {
                if (m_htmlImportSettings == null)
                    m_htmlImportSettings = new HTMLImportSettings();
                return m_htmlImportSettings;
            }
            set { m_htmlImportSettings = value; }
        }
        /// <summary>
        /// Gets the save options.
        /// </summary>
        /// <value>The save options.</value>
        public SaveOptions SaveOptions
        {
            get
            {
                if (m_saveOptions == null)
                    m_saveOptions = new SaveOptions();
                return m_saveOptions;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to update fields in the document.
        /// </summary>
        /// <value>if update fields, set to <c>true</c>.</value>
        [ObsoleteAttribute("This property has been deprecated. Use the UpdateDocumentFields method of WordDocument class to update the fields in the document.")]
        public bool UpdateFields
        {
            get
            {
                return m_updateFields;
            }
            set
            {
                m_updateFields = value;
            }
        }
        /// <summary>
        /// Gets not parsed docx properties.
        /// </summary>
        internal List<Stream> DocxProps
        {
            get
            {
                if (m_docxProps == null)
                {
                    m_docxProps = new List<Stream>();
                }

                return m_docxProps;
            }
        }

        /// <summary>
        /// Gets the list style Names in Docx parser alone
        /// </summary>
        /// <value>The list styles.</value>
        internal Dictionary<string, string> ListStyleNames
        {
            get
            {
                if (m_listStyleNames == null)
                {
                    m_listStyleNames = new Dictionary<string, string>();
                }
                return m_listStyleNames;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance has docx props.
        /// </summary>
        /// <value>
        /// 	if this instance has docx prop, set to <c>true</c>.
        /// </value>
        internal bool HasDocxProps
        {
            get
            {
                return (m_docxProps == null) ? false : true;
            }
        }
		
        /// <summary>
        /// Indicates whether the document is currently closing.
        /// </summary>
        internal bool IsClosing
        {
            get
            {
                return m_bIsClosing;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal Dictionary<string, string> StyleNameIds
        {
            get 
            {
                if (m_styleNameIds == null)
                {
                    m_styleNameIds = new Dictionary<string, string>();
                }
                return m_styleNameIds;
            }
        }
        /// <summary>
        /// Returns the actual format type of the document which was loaded. While creating new document, it returns FormatType.Doc value.
        /// </summary>
        public FormatType ActualFormatType
        {
            get
            {
                return m_actualFormatType;
            }
           internal set
            {
                m_actualFormatType = value;
            }
        }
        /// <summary>
        /// A dictionary object which represents the font substitution table. The key should be the font name and the value should be the alternate font name.
        /// </summary>
        /// <value>The font substitution table.</value>
        public Dictionary<string, string> FontSubstitutionTable
        {
            get
            {
                if (m_fontSubstitutionTable == null)
                {
                    m_fontSubstitutionTable = new Dictionary<string, string>();
                }
                return m_fontSubstitutionTable;
            }
            set
            {
                m_fontSubstitutionTable = value;
            }
        }
        /// <summary>
        /// Gets a value indicating whether the document has macros.
        /// </summary>
        /// <value><c>true</c> if the document has macros; otherwise, <c>false</c>.</value>
        public bool HasMacros
        {
            get
            {
                return (VbaProject != null
                    && (VbaData.Count > 0
                    || DocEvents.Count > 0));
            }
        }
        #endregion

        #region Properties / internal
        /// <summary>
        /// Gets/sets FontFamilyNameStringTable 
        /// </summary>
        internal FontFamilyNameStringTable FFNStringTable
        {
            get
            {
                return m_ffnStringTable;
            }

            set
            {
                m_ffnStringTable = value;
            }
        }
        /// <summary>
        /// Gets the images.
        /// </summary>
        /// <value>The images.</value>
        internal ImageCollection Images
        {
            get
            {
                if (m_imageCollection == null)
                    m_imageCollection = new ImageCollection(this);
                return m_imageCollection;
            }
        }
        /// <summary>
        /// Gets the field stack.
        /// </summary>
        /// <value>The field stack.</value>
        internal Stack<WField> ClonedFields
        {
            get
            {
                if (m_clonedFields == null)
                {
                    m_clonedFields = new Stack<WField>();
                }
                return m_clonedFields;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal ListOverrideStyleCollection ListOverrides
        {
            get
            {
                return m_listOverrides;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal GrammarSpelling GrammarSpellingData
        {
            get
            {
                return m_grammarSpellingData;
            }
            set
            {
                m_grammarSpellingData = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal DOPDescriptor DOP
        {
            get
            {
                return m_dop;
            }
            set
            {
                m_dop = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal EscherClass Escher
        {
            get
            {
                return m_escher;
            }
            set
            {
                m_escher = value;
            }
        }

        /// <summary>
        /// Gets currently processed section.
        /// </summary>
        internal WSection CurClonedSection
        {
            get
            {
                return m_curClonedSection;
            }
            set
            {
                m_curClonedSection = value;
            }
        }
        /// <summary>
        /// Gets or sets the type of the save format.
        /// </summary>
        /// <value>The type of the save format.</value>
        internal FormatType SaveFormatType
        {
            get
            {
                return m_saveFormatType;
            }
            set
            {
                m_saveFormatType = value;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance is macro enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is macro enabled; otherwise, <c>false</c>.
        /// </value>
        internal bool IsMacroEnabled
        {
            get
            {
                return (SaveFormatType == FormatType.Word2007Docm
                    || SaveFormatType == FormatType.Word2010Docm
                    || SaveFormatType == FormatType.Word2007Dotm
                    || SaveFormatType == FormatType.Word2010Dotm);
            }
        }
        /// <summary>
        /// Gets or sets the vba project.
        /// </summary>
        /// <value>The vba project.</value>
        internal Stream VbaProject
        {
            get
            {
                return m_vbaProject;
            }
            set
            {
                m_vbaProject = value;
            }
        }
        /// <summary>
        /// Gets or sets the custom UI part container.
        /// </summary>
        /// <value>The custom UI part container.</value>
        internal PartContainer CustomUIPartContainer
        {
            get
            {
                return m_CustomUIPartContainer;
            }
            set
            {
                m_CustomUIPartContainer = value;
            }
        }
        /// <summary>
        /// Gets or sets the custom XML container.
        /// </summary>
        /// <value>The custom XML container.</value>
        internal PartContainer CustomXMLContainer
        {
            get
            {
                return m_CustomXMLContainer;
            }
            set
            {
                m_CustomXMLContainer = value;
            }
        }   
        /// <summary>
        /// Gets or sets the vba data.
        /// </summary>
        /// <value>The vba data.</value>
        internal List<MacroData> VbaData
        {
            get
            {
                if (m_vbaData == null)
                    m_vbaData = new List<MacroData>();
                return m_vbaData;
            }
            set
            {
                m_vbaData = value;
            }
        }
        /// <summary>
        /// Gets or sets the doc events.
        /// </summary>
        /// <value>The doc events.</value>
        internal List<string> DocEvents
        {
            get
            {
                if (m_docEvents == null)
                    m_docEvents = new List<string>();
                return m_docEvents;
            }
            set
            {
                m_docEvents = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal byte[] MacrosData
        {
            get
            {
                return m_macrosData;
            }
            set
            {
                m_macrosData = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal byte[] MacroCommands
        {
            get
            {
                return m_macroCommands;
            }
            set
            {
                m_macroCommands = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal string StandardAsciiFont
        {
            get
            {
                return m_standardAsciiFont;
            }
            set
            {
                m_standardAsciiFont = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal string StandardFarEastFont
        {
            get
            {
                return m_standardFarEastFont;
            }
            set
            {
                m_standardFarEastFont = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal string StandardNonFarEastFont
        {
            get
            {
                return m_standardNonFarEastFont;
            }
            set
            {
                m_standardNonFarEastFont = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal string StandardBidiFont
        {
            get
            {
                return m_standardBidiFont;
            }
            set
            {
                m_standardBidiFont = value;
            }
        }
        /// <summary>
        /// Gets or Sets the password.
        /// </summary>
        /// <value>The password.</value>
        internal string Password
        {
            get
            {
                return m_password;
            }
            set
            {
                m_password = value;
            }
        }
        /// <summary>
        /// Gets or sets document latent styles.
        /// </summary>
        /// <value>The latent styles.</value>
        internal MemoryStream LatentStyles2010
        {
            get
            {
                return m_latentStyles2010;
            }
            set
            {
                m_latentStyles2010 = value;
            }
        }
        /// <summary>
        /// Gets or sets document latent styles.
        /// </summary>
        /// <value>The latent styles.</value>
#if !SILVERLIGHT && !WP
        internal XmlNode LatentStyles
        {
            get
            {
                return m_latentStyles;
            }
            set
            {
                m_latentStyles = value;
            }
        }
#else
        internal Stream LatentStyles
        {
            get
            {
                return m_latentStyles;
            }
            set
            {
                m_latentStyles = value;
            }
        }
#endif

        /// <summary>
        /// Gets or sets the docx package.
        /// </summary>
        /// <value>The docx package.</value>
        internal Package DocxPackage
        {
            get
            {
                return m_docxPackage;
            }
            set
            {
                m_docxPackage = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether to import styles.
        /// </summary>
        /// <value>if import styles, set to <c>true</c>.</value>
        internal bool ImportStyles
        {
            get
            {
                return m_importStyles;
            }
            set
            {
                m_importStyles = value;
            }
        }
        /// <summary>
        /// Gets or sets the import option.
        /// </summary>
        /// <value>The import option.</value>
        internal ImportOptions ImportOption
        {
            get
            {
                return m_importOption;
            }
            set
            {
                m_importOption = value;
            }
        }

        /// <summary>
        /// Gets or sets the default character format.
        /// </summary>
        /// <value>The default char format.</value>
        internal WCharacterFormat DefCharFormat
        {
            get
            {
                return m_defCharFormat;
            }
            set
            {
                m_defCharFormat = value;
            }
        }

        /// <summary>
        /// Gets or sets the default paragraph format.
        /// </summary>
        /// <value>The default paragraph format.</value>
        internal WParagraphFormat DefParaFormat
        {
            get
            {
                if (m_defParaFormat == null && !IsOpening)
                    InitDefaultParagraphFormat();
                return m_defParaFormat;
            }
            set
            {
                m_defParaFormat = value;
            }
        }

        /// <summary>
        /// Gets the associated strings.
        /// </summary>
        /// <value>The def para format.</value>
        internal SttbfAssoc AssociatedStrings
        {
            get
            {
                if (m_assocStrings == null)
                    m_assocStrings = new SttbfAssoc();
                return m_assocStrings;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is encrypted.
        /// </summary>
        /// <value>
        /// 	if this instance is encrypted, set to <c>true</c>.
        /// </value>
        internal bool IsEncrypted
        {
            get
            {
                return m_isEncrypted;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has pictures.
        /// </summary>
        /// <value>
        /// if this instance contains picture, set to <c>true</c>.
        /// </value>
        internal bool HasPicture
        {
            get
            {
                return m_hasPicture;
            }
            set
            {
                m_hasPicture = value;
            }
        }
        /// <summary>
        /// Gets a value indicating whether to write evaluation expired warning.
        /// </summary>
        /// <value>if write warning, set to <c>true</c>.</value>
        internal bool WriteWarning
        {
            get
            {
                return (m_isEvalExpired && !m_isWarnInserted) ? true : false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [write protected].
        /// </summary>
        /// <value><c>true</c> if [write protected]; otherwise, <c>false</c>.</value>
        internal bool WriteProtected
        {
            get
            {
                return m_isWriteProtected;
            }
            set
            {
                m_isWriteProtected = value;
            }
        }

        /// <summary>
        /// Gets the object pool containers.
        /// </summary>
        /// <value>The obj pool containers.</value>
        internal List<String> ObjPoolContainers
        {
            get
            {
                if (m_objPoolContainers == null)
                {
                    m_objPoolContainers = new List<String>();
                }
                return m_objPoolContainers;
            }
        }
        
#if !SILVERLIGHT && !WP
        // Handled this case for updating list in Doc to PDF conversion.
        /// <summary>
        /// Gets the list names.
        /// </summary>
        /// <value>The list names.</value>
        private HybridDictionary ListNames
        {
            get
            {
                if (m_listNames == null)
                {
                    m_listNames = new HybridDictionary();
                }
                return m_listNames;
            }

        }
        /// <summary>
        /// Gets the list and its level number.
        /// </summary>
        /// <value>The lists.</value>
        private Dictionary<string, Dictionary<int, int>> Lists
        {
            get
            {
                if (m_lists == null)
                {
                    m_lists = new Dictionary<string, Dictionary<int, int>>();
                }
                return m_lists;
            }
        }
        /// <summary>
        /// Specifies the previous list level of a list style.
        /// </summary>
        /// <value>The lists.</value>
        private Dictionary<string, int> PreviousListLevel
        {
            get
            {
                if (m_previousListLevel == null)
                {
                    m_previousListLevel = new Dictionary<string, int>();
                }
                return m_previousListLevel;
            }
        }

        /// <summary>
        /// Specifies the override style name of all the previous lists
        /// </summary>
        /// <value>The lists.</value>
        private List<string> PreviousListLevelOverrideStyle
        {
            get
            {
                if (m_previousListLevelOverrideStyle == null)
                {
                    m_previousListLevelOverrideStyle = new List<string>();
                }
                return m_previousListLevelOverrideStyle;
            }
        }
        /// <summary>
        /// Specifies whether to use hanging indent as tab stop for lists in Doc to PDF lay outing.
        /// </summary>
        /// <value><c>true</c> if use hanging indent as tab stop for lists; otherwise, <c>false</c>.</value>
        internal bool UseHangingIndentAsListTab
        {
            get
            {
                return !(ActualFormatType == FormatType.Doc
                    || ((ActualFormatType == FormatType.Docx
                    || ActualFormatType == FormatType.Word2007
                    || ActualFormatType == FormatType.Word2010
                    || ActualFormatType == FormatType.Word2013)
                    && Settings.CompatibilityOptions[CompatibilityOption.DontUseIndentAsNumberingTabStop]));
            }
        }
#endif

        /// <summary>
        /// Gets the compatibility settings of the document.
        /// </summary>
        /// <value>The compatibility settings.</value>
        internal Settings Settings
        {
            get
            {
                if (m_settings == null)
                    m_settings = new Settings(m_dop);
                return m_settings;
            }
        }
        /// <summary>
        /// Gets/ Set the total number of alternate chunk in the document.
        /// </summary>
        /// <value>The alternate chunk count.</value>
        internal int AlternateChunkCount
        {
            get
            {
               return ++m_altChunkCount;
            }            
        }
        #endregion

        #region Constructors
#if !SILVERLIGHT && !WP
       /// <summary>
        /// Initializes a new instance of the WordDocument class from Word document.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <example>
        /// <code lang="C#">
        /// // Loading an existing word document.
        /// WordDocument doc = new WordDocument("Sample.doc");
        /// </code>
        /// </example>
        public WordDocument(string fileName)
            : this()
        {
            if (fileName == null)
                throw new ArgumentNullException("FileName");
            this.WordDocumentType(fileName, null);
        }

        /// <summary>
        /// Initializes a new instance of the WordDocument class from existing Word document,
        /// which is protected with password.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="password">The password.</param>
        /// <example>
        /// <code lang="C#">
        /// // Loading an existing word document.
        /// WordDocument doc = new WordDocument("Sample.doc", "password");
        /// </code>
        /// </example>
        public WordDocument(string fileName, string password)
            : this()
        {
            if (fileName == null)
                throw new ArgumentNullException("FileName");
            if (password == null)
                throw new ArgumentNullException("Password");
            this.WordDocumentType(fileName, password);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WordDocument"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="type">The type.</param>
        /// <example>
        /// <code lang="C#">
        /// // Loading an existing word document.
        /// WordDocument doc = new WordDocument("Sample.docx", FormatType.docx);
        /// </code>
        /// </example>
        public WordDocument(string fileName, FormatType type)
            : this()
        {
            if (fileName == null)
                throw new ArgumentNullException("FileName");
            OpenInternal(fileName, type, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WordDocument"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="type">The type of the opened document.</param>
        /// <param name="validationType">The XHTML validation type.</param>
        /// <example>
        /// <code lang="C#">
        /// // Loading an existing word document.
        /// WordDocument docSource1 = new WordDocument("sample.doc",FormatType.Doc,XHTMLValidationType.Strict); 
        /// </code>
        /// </example>
        public WordDocument(string fileName, FormatType type, XHTMLValidationType validationType)
            : this()
        {
            if (type == FormatType.Automatic)
            {
                type = this.GetFormatType(fileName);
            }
            Open(fileName, type, validationType);
        }

        /// <summary>
        /// Initializes a new instance of the WordDocument class from
        /// existing file of specified type protected with password.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="type">The type.</param>
        /// <param name="password">The password.</param>
        /// <example>
        /// <code lang="C#">
        /// // Loading an existing word document.
        /// WordDocument docSource1 = new WordDocument("sample.doc",FormatType.Doc, "password"); 
        /// </code>
        /// </example>
        public WordDocument(string fileName, FormatType type, string password)
            : this()
        {
            if (type == FormatType.Automatic)
            {
                type = this.GetFormatType(fileName);
            }

            Open(fileName, type, password);
        }

        /// <summary>
        /// Gets the File Type for the Given fileName.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="password"></param>
        private void WordDocumentType(string fileName, string password)
        {
            FormatType type;
            fileName = fileName.ToLower();
            if (File.Exists(fileName))
            {
                string extension = Path.GetExtension(fileName);
                if (!string.IsNullOrEmpty(extension))
                {
                    type = this.GetFormatType(fileName);
                    OpenInternal(fileName, type, password);
                }
                else
                {
                    // try to choose the correct format.
                    try
                    {
                        OpenInternal(fileName, FormatType.Doc, password);
                        return;
                    }
                    catch { }

                    try
                    {
                        OpenInternal(fileName, FormatType.Docx, password);
                        return;
                    }
                    catch
                    {
                        throw new Exception("Cannot recognize file format");
                    }
                }
            }
            else
            {
                throw new Exception("Cannot recognize current file path");
            }
        }
        /// <summary>
        /// Initializes a new instance of the WordDocument class
        /// </summary>
        /// <param name="stream">The file stream.</param>
        /// <param name="type">The type of the opened document.</param>
        /// <param name="validationType">Type of the validation.</param>
        /// <example>
        /// <code lang="C#">
        /// // Loading an existing word document.
        /// Stream file2 = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        /// WordDocument docSource1 = new WordDocument(file2,FormatType.Doc,XHTMLValidationType.Strict); 
        /// </code>
        /// </example>
        public WordDocument(Stream stream, FormatType type, XHTMLValidationType validationType)
            : this()
        {
            Open(stream, type, validationType);
        }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="WordDocument"/> class.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// //A new document is created.
        /// WordDocument document = new WordDocument();
        /// </code>
        /// </example>
        public WordDocument()
            : base(null, null)
        {
            //#if AllowUnsafeCode
#if !SILVERLIGHT && !WP
            if (IsSecurityGranted())
            {
                CheckLicense();
            }
#endif
            //#endif

            m_doc = this;
            Init();
            //EnsureListStyles();
        }

       

        /// <summary>
        /// Initializes a new instance of the WordDocument class from the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <example>
        /// <code lang="C#">
        /// Stream file2 = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        /// WordDocument docSource1 = new WordDocument(file2);
        /// </code>
        /// </example>
        public WordDocument(Stream stream)
            : this()
        {
            FormatType type = FormatType.Doc;
#if !SILVERLIGHT && !WP
            type = FormatType.Automatic;
#endif
            if (stream == null)
                throw new ArgumentNullException("Stream");
            OpenInternal(stream, type, null);
        }

        /// <summary>
        /// Initializes a new instance of the WordDocument class from the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <example>
        /// <code lang="C#">
        /// Stream file2 = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        /// WordDocument docSource1 = new WordDocument(file2, FormatType.doc);
        /// </code>
        /// </example>
        public WordDocument(Stream stream, FormatType type)
            : this()
        {
            Open(stream, type);
        }

        /// <summary>
        /// Initializes a new instance of the WordDocument class from the Word document�s stream, 
        /// which is protected with password.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="password">The password.</param>
        /// <example>
        /// <code lang="C#">
        /// Stream file2 = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        /// WordDocument docSource1 = new WordDocument(file2, "password");
        /// </code>
        /// </example>
        public WordDocument(Stream stream, string password)
            : this()
        {
            FormatType type = FormatType.Doc;
#if !SILVERLIGHT && !WP
            type = FormatType.Automatic;
#endif
            Open(stream, type, password);
        }

        /// <summary>
        /// Initializes a new instance of the WordDocument class
        /// from the stream of specified type protected with password.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="type">The type.</param>
        /// <param name="password">The password.</param>
        /// <example>
        /// <code lang="C#">
        /// Stream file2 = new FileStream(dataPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        /// WordDocument docSource1 = new WordDocument(file2, FormatType.doc, "password");
        /// </code>
        /// </example>
        public WordDocument(Stream stream, FormatType type, string password)
            : this()
        {
            Open(stream, type, password);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        protected WordDocument(WordDocument doc)
            : this()
        {
            m_standardAsciiFont = doc.StandardAsciiFont;
            m_standardFarEastFont = doc.StandardFarEastFont;
            m_standardNonFarEastFont = doc.StandardNonFarEastFont;
            m_standardBidiFont = doc.m_standardBidiFont;
            m_viewSetup = doc.ViewSetup.Clone(this);

            if (doc.BuiltinDocumentProperties != null)
                m_builtinProp = doc.BuiltinDocumentProperties.Clone();

            if (doc.CustomDocumentProperties != null)
                m_customProp = doc.CustomDocumentProperties.Clone();


            // Clones watermark
            if (doc.Watermark.Type != WatermarkType.NoWatermark)
            {
                Watermark = (Watermark)doc.Watermark.Clone();
            }
            // Clones background
            if (doc.Background.Type != BackgroundType.NoBackground)
            {
                m_background = (Background)doc.Background.Clone();
                m_background.SetOwner(this);
                m_background.UpdateImageRecord(this);
            }
            // Clones DOP
            if (doc.DOP != null)
            {
                m_dop = doc.DOP.Clone();
            }

            // Clone default character format
            if (doc.DefCharFormat != null)
            {
                m_defCharFormat = new WCharacterFormat(this);
                m_defCharFormat.ImportContainer(doc.DefCharFormat);
            }
            // Clone default paragraph format
            if (doc.DefParaFormat != null)
            {
                m_defParaFormat = new WParagraphFormat(this);
                m_defParaFormat.ImportContainer(doc.DefParaFormat);
            }
            //clone the font substitution table
            foreach (KeyValuePair<string, string> entry in doc.FontSubstitutionTable)
            {
                if (!FontSubstitutionTable.ContainsKey(entry.Key))
                    FontSubstitutionTable.Add(entry.Key, entry.Value);
                else
                    FontSubstitutionTable[entry.Key] = entry.Value;
            }
            //clones footnote separators
            Footnotes = doc.Footnotes.Clone();
            //Clones endnote separators
            Endnotes = doc.Endnotes.Clone();
            // Clones content
            ImportContent(doc);
        }

        /// <summary>
        /// Returns the format type based on the file extension of the given file name.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private FormatType GetFormatType(string fileName)
        {
            FormatType type;
            string extension = Path.GetExtension(fileName).ToLower();
            switch (extension)
            {
                case ".doc":
                case ".dot":
                    type = FormatType.Doc;
                    break;
                case ".docx":
                    type = FormatType.Docx;
                    break;
                case ".dotx":
                    type = FormatType.Word2007Dotx;
                    break;
                case ".docm":
                    type = FormatType.Word2007Docm;
                    break;
                case ".dotm":
                    type = FormatType.Word2007Dotm;
                    break;
#if !SILVERLIGHT && !WP
                case ".txt":
                    type = FormatType.Txt;
                    break;
                case ".xml":
                    type = FormatType.Xml;
                    break;
                case ".html":
                    type = FormatType.Html;
                    break;
                case ".rtf":
                    type = FormatType.Rtf;
                    break;
                case ".epub":
                    type = FormatType.EPub;
                    break;
#endif
                default:
                    throw new Exception("Cannot recognize current file type");
                    break;
            }
            return type;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Creates the paragraph.
        /// </summary>
        /// <returns></returns>
        public IWParagraph CreateParagraph()
        {
            return new WParagraph(this);
        }

        /// <summary>
        /// Adds one empty section to the document and one empty paragraph to created section.
        /// </summary>
        public void EnsureMinimal()
        {
            if (Sections.Count == 0)
            {
                AddSection().Body.AddParagraph();
            }
        }

        /// <summary>
        /// Adds new section to document.
        /// </summary>
        /// <returns></returns>
        public IWSection AddSection()
        {
            WSection sec = new WSection(Document);
#if !SILVERLIGHT && !WP
            if (m_sections.Count > 0)
            {
                WPageSetup prevSectPageSet = m_sections[m_sections.Count - 1].PageSetup;
                WPageSetup curSectPageSet = sec.PageSetup;
                curSectPageSet.Margins = prevSectPageSet.Margins.Clone();
                curSectPageSet.PageSize = prevSectPageSet.ClonePageSize();
                curSectPageSet.Orientation = prevSectPageSet.Orientation;
            }
#endif
            m_sections.Add(sec);
            return sec;
        }

        /// <summary>
        /// Adds new paragraph style to the document.
        /// </summary>
        /// <param name="styleName">Paragraph style name</param>
        /// <returns></returns>
        public IWParagraphStyle AddParagraphStyle(string styleName)
        {
            return AddStyle(StyleType.ParagraphStyle, styleName) as IWParagraphStyle;
        }

        /// <summary>
        /// Adds new list style to document.
        /// </summary>
        /// <param name="listType">List type</param>
        /// <param name="styleName">Paragraph style name</param>
        /// <returns></returns>
        public ListStyle AddListStyle(ListType listType, string styleName)
        {
            ListStyle style = new ListStyle(this, listType);
            ListStyles.Add(style);
            style.Name = styleName;
            return style;
        }
#if !(SILVERLIGHT || WP) || WINRT
        /// <summary>
        /// Gets the document's text.
        /// </summary>
        public string GetText()
        {
            TextConverter textConverter = new TextConverter();
            return textConverter.GetText(this);
        }
#endif

        /// <summary>
        /// Clones itself.
        /// </summary>
        /// <returns></returns>
        new public WordDocument Clone()
        {
            return (WordDocument)CloneImpl();
        }

        /// <summary>
        /// Imports section into document.
        /// </summary>
        /// <param name="section">The section.</param>
        public void ImportSection(IWSection section)
        {
            IWSection clonedSection = section.Clone();
            Sections.Add(clonedSection);
        }

        /// <summary>
        /// Imports all content into the document.
        /// </summary>
        /// <param name="doc">The doc.</param>
        public void ImportContent(IWordDocument doc)
        {
            ImportContent(doc, true);
        }
        /// <summary>
        /// Imports the content into the document.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="importOptions">The import options.</param>
        public void ImportContent(IWordDocument doc, ImportOptions importOptions)
        {
            (doc as WordDocument).IsCloning = true;
            m_importOption = importOptions;
            if (m_importOption == ImportOptions.UseDestinationStyles)
                m_importStyles = false;

            if (m_importOption == ImportOptions.KeepTextOnly)
            {
                ImportDocumentText(doc);
            }
            else
            {
                doc.Sections.CloneTo(m_sections);
                // Clone binary data
                CopyBinaryData((doc as WordDocument).MacrosData, ref m_macrosData);
                CopyBinaryData((doc as WordDocument).MacroCommands, ref m_macroCommands);
#if !SILVERLIGHT && !WP
                if (m_docxPackage == null
                    && (doc as WordDocument).DocxPackage != null)
                    m_docxPackage = (doc as WordDocument).DocxPackage.Clone() as Package;
#endif
                m_docxProps = (doc as WordDocument).m_docxProps;
            }
            (doc as WordDocument).IsCloning = false;
            m_importOption = ImportOptions.UseDestinationStyles;
            m_importStyles = true;
        }
        /// <summary>
        /// Imports the document text.
        /// </summary>
        /// <param name="doc">The doc.</param>
        private void ImportDocumentText(IWordDocument doc)
        {
            string text = doc.Sections.GetText();
            m_prevClonedEntity = null;
            IWSection section = AddSection();
            string[] splittedText = text.Split('\r');
            for (int i = 0; i < splittedText.Length; i++)
            {
                section.AddParagraph().AppendText(splittedText[i]);
            }
        }
        /// <summary>
        /// Imports all content into document.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="importStyles">If document styles which have same names will be also imported
        /// to the destination document,set to <c>true</c>.</param>
        public void ImportContent(IWordDocument doc, bool importStyles)
        {
            (doc as WordDocument).IsCloning = true;
            m_importStyles = importStyles;
            doc.Sections.CloneTo(m_sections);

            // Clones styles items
            Style style = null;
            //Updates style name ids collection
            foreach (KeyValuePair <string, string> styleName in (doc as WordDocument).StyleNameIds)
            {
                if (!StyleNameIds.ContainsKey(styleName.Key))
                {
                    StyleNameIds.Add(styleName.Key, styleName.Value);
                }
            }
            for (int i = 0, cnt = doc.Styles.Count; i < cnt; i++)
            {
                style = doc.Styles[i] as Style;
                Style foundStyle = Styles.FindByName(style.Name, style.StyleType) as Style;

                if (foundStyle == null)
                {
                    Styles.Add(style.Clone());
                }
            }

            // Clone list styles
            ListStyle listStyle = null;
            for (int index = 0, counter = doc.ListStyles.Count; index < counter; index++)
            {
                listStyle = doc.ListStyles[index];
                if (ListStyles.FindByName(listStyle.Name) == null)
                {
                    ListStyles.Add((ListStyle)listStyle.Clone());
                }
            }
            //clone the font substitution table
            foreach (KeyValuePair<string, string> entry in (doc as WordDocument).FontSubstitutionTable)
            {
                if (!FontSubstitutionTable.ContainsKey(entry.Key))
                    FontSubstitutionTable.Add(entry.Key, entry.Value);
                else
                    FontSubstitutionTable[entry.Key] = entry.Value;
            }

            //Clone list override styles
            ListOverrideStyle overrideStyle = null;
            for (int j = 0, lstCounter = (doc as WordDocument).ListOverrides.Count; j < lstCounter; j++)
            {
                overrideStyle = (doc as WordDocument).ListOverrides[j];
                if (ListOverrides.FindByName(overrideStyle.Name) == null)
                {
                    ListOverrides.Add((ListOverrideStyle)overrideStyle.Clone());
                }
            }

            // Clone binary data
            CopyBinaryData((doc as WordDocument).MacrosData, ref m_macrosData);
            CopyBinaryData((doc as WordDocument).MacroCommands, ref m_macroCommands);
            // Imports default document style
            if ((doc as WordDocument).DefCharFormat != null)
            {
                if (m_defCharFormat == null)
                    m_defCharFormat = new WCharacterFormat(m_doc);
                m_defCharFormat.ImportContainer((doc as WordDocument).DefCharFormat);
            }

            if (m_defParaFormat == null)
            {
                m_defParaFormat = new WParagraphFormat(m_doc);
                m_defParaFormat.ImportContainer((doc as WordDocument).DefParaFormat);
            }
#if !SILVERLIGHT && !WP
            if (m_docxPackage == null 
                && (doc as WordDocument).DocxPackage != null)
            {
                m_docxPackage = (doc as WordDocument).DocxPackage.Clone() as Package;
            }
#endif
            m_docxProps = (doc as WordDocument).m_docxProps;
            (doc as WordDocument).IsCloning = false;
        }

        /// <summary>
        /// Adds the style to the document style.
        /// </summary>
        /// <param name="builtinStyle">The built-in style.</param>
        public IStyle AddStyle(BuiltinStyle builtinStyle)
        {
            CheckNormalStyle();

            string builtinName = Style.BuiltInToName(builtinStyle);
            IStyle style = Document.Styles.FindByName(builtinName);

            if (style == null)
            {
                style = Style.CreateBuiltinStyle(builtinStyle, Document);
                Document.Styles.Add(style);

                if (builtinStyle != BuiltinStyle.MacroText && builtinStyle != BuiltinStyle.CommentSubject)
                {
                    IStyle newStyle = Document.Styles.FindByName(builtinName);
                    (newStyle as Style).ApplyBaseStyle(DEF_NORMAL_STYLE);
                }
            }

            return style;
        }

        /// <summary>
        /// Accepts changes tracked from the moment of last change acceptance.
        /// </summary>
        public void AcceptChanges()
        {
            foreach (WSection section in Sections)
            {
                section.MakeChanges(true);
            }
        }

        /// <summary>
        /// Rejects changes tracked from the moment of last change acceptance.
        /// </summary>
        public void RejectChanges()
        {
            foreach (WSection section in Sections)
            {
                section.MakeChanges(false);
            }
        }

        /// <summary>
        /// Protects the document.
        /// </summary>
        /// <param name="type">The type of the protection.</param>
        public void Protect(ProtectionType type)
        {
            //SetProtection(type);
            Protect(type, null);
        }

        /// <summary>
        /// Protects the document.
        /// </summary>
        /// <param name="type">The type of the protection</param>
        /// <param name="password">The password used for protection.</param>
        public void Protect(ProtectionType type, string password)
        {
           //SetProtection(type);
           m_dop.SetProtection(type, password);
        }

        /// <summary>
        /// Encrypts the document.
        /// </summary>
        /// <param name="password">The password.</param>
        public void EncryptDocument(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new Exception("Password cannot be null or empty!");
            m_password = password;
        }

        /// <summary>
        /// Removes the encryption.
        /// </summary>
        public void RemoveEncryption()
        {
            m_password = null;
        }

        /// <summary>
        /// Adds new style to document.
        /// </summary>
        /// <param name="styleType">Style type</param>
        /// <param name="styleName">Style name</param>
        /// <returns></returns>
        internal IStyle AddStyle(StyleType styleType, string styleName)
        {
            if (styleType == StyleType.OtherStyle)
                throw new NotSupportedException();

            IStyle style = null;

            switch (styleType)
            {
                case StyleType.ParagraphStyle:
                    style = new WParagraphStyle(Document);
                    break;
                case StyleType.CharacterStyle:
                    style = new CharacterStyle(Document);
                    break;
                default:
                    break;
            }

            if (style != null)
            {
                if (styleName != null && styleName.Length > 0)
                {
                    style.Name = styleName;
                }
                m_styles.Add(style);
            }

            return style;
        }

        /// <summary>
        /// Checks the normal style.
        /// </summary>
        private void CheckNormalStyle()
        {
            WParagraphStyle pStyle = Document.Styles.FindByName("Normal", StyleType.ParagraphStyle) as WParagraphStyle;
            if (pStyle == null)
            {
                pStyle = (WParagraphStyle)Style.CreateBuiltinStyle(BuiltinStyle.Normal, Document);
                Document.Styles.Add(pStyle);
            }
        }
        #endregion

        #region Public methods / open & save
#if  !SILVERLIGHT && !WP
        private void OpenDocx(string fileName)
        {
            DocxParser wordReader = new DocxParser();
            this.IsOpening = true;
            wordReader.Read(fileName, this);
            this.IsOpening = false;
        }
#endif

        /// <summary>
        /// Opens the Word Docx document
        /// </summary>
        /// <param name="stream">The stream contains the document content</param>
        private void OpenDocx(Stream stream)
        {
            DocxParser wordReader = new DocxParser();
            this.IsOpening = true;
            wordReader.Read(stream, this);
            this.IsOpening = false;
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Opens the document from xml format file.
        /// </summary>
        /// <param name="fileName">The name of file</param>
        internal void OpenXml(string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Open))
            {
                OpenXml(stream);
            }
        }
        
        /// <summary>
        /// Opens the XML document from stream.
        /// </summary>
        /// <param name="stream">The stream object</param>
        internal void OpenXml(Stream stream)
        {
#if SyncfusionFramework1_0 || SyncfusionFramework1_1
      XmlTextReader reader = new XmlTextReader(stream);
#elif SyncfusionFramework2_0
            // Set the validation settings.
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
#if DEBUG
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
            settings.CheckCharacters = false;
            settings.IgnoreComments = true;
            settings.IgnoreProcessingInstructions = true;
            settings.Schemas.Add(GetSchema());
#endif
            //Create the XmlReader object.
            XmlReader reader = XmlReader.Create(stream, settings);
#endif
            ReadXml(reader);
        }
        /// <summary>
        /// Display any warnings or errors.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void ValidationCallBack(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
            {
                Debug.WriteLine("\tWarning: Matching schema not found.  No validation occurred." + args.Message);
            }
            else
            {
                string errmess = "\tValidation error: " + args.Message;
                Debug.WriteLine(errmess);
                throw new XDLSException(errmess);
            }
        }
        /// <summary>
        /// Saves the document in xml format.
        /// </summary>
        /// <param name="fileName">The name of target file</param>
        internal void SaveXml(string fileName)
        {
            CheckEvalExpired();
            XmlTextWriter writer = new XmlTextWriter(fileName, Encoding.Unicode);
            writer.Formatting = Formatting.Indented;
            WriteXml(writer);
            writer.Close();
        }
        /// <summary>
        /// Saves the document in xml format.
        /// </summary>
        /// <param name="stream">The target stream</param>
        internal void SaveXml(Stream stream)
        {
            CheckEvalExpired();
            XmlTextWriter writer = new XmlTextWriter(stream, Encoding.Unicode);

            try
            {
                writer.Formatting = Formatting.Indented;
                WriteXml(writer);
            }
            finally
            {
                writer.Flush();
            }
        }
        /// <summary>
        /// Saves the document in text format.
        /// </summary>
        /// <param name="fileName">The name of target file</param>
        internal void SaveTxt(string fileName)
        {
            SaveTxt(fileName, Encoding.UTF8);
        }
        /// <summary>
        /// Saves to text document with specified encoding.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="encoding">The encoding.</param>
        public void SaveTxt(string fileName, Encoding encoding)
        {
            CheckEvalExpired();
            StreamWriter writer = new StreamWriter(fileName, false, encoding);
            TextConverter textConverter = new TextConverter();
            textConverter.Write(writer, this);
            writer.Close();
        }
#endif

#if !SILVERLIGHT && !WP
        /// <summary>
        /// Saves the document in EPub format.
        /// </summary>
        /// <param name="fileName"></param>
        private void SaveEPub(string fileName, WPicture coverImage)
        {
            EPubConverter epubConverter = new EPubConverter();
            epubConverter.FileName = Path.GetFileName(fileName).Replace(Path.GetExtension(fileName), string.Empty);
            epubConverter.CoverImage = coverImage;
            epubConverter.ConvertToEPub(fileName, this);
        }
        /// <summary>
        /// Saves the document as EPub stream
        /// </summary>
        /// <param name="stream"></param>
        private void SaveEPub(Stream stream, WPicture coverImage)
        {
            EPubConverter epubConverter = new EPubConverter();
            epubConverter.CoverImage = coverImage;
            epubConverter.ConvertToEPub(stream, this);
        }
        /// <summary>
        /// Saves the document in open xml format (Docx).
        /// </summary>
        /// <param name="fileName"></param>
        private void SaveDocx(string fileName)
        {
            CheckEvalExpired();
            DocxSerializator serializator = new DocxSerializator();
            serializator.Serialize(fileName, this);
        }
#endif
        /// <summary>
        /// Saves the document in open xml format (Docx).
        /// </summary>
        /// <param name="stream"></param>
        private void SaveDocx(Stream stream)
        {
            CheckEvalExpired();
            DocxSerializator serializator = new DocxSerializator();
            serializator.Serialize(stream, this);
        }
        /// <summary>
        /// Saves the RTF.
        /// </summary>
        /// <param name="stream">The stream.</param>
        private void SaveRtf(Stream stream)
        {
            CheckEvalExpired();
            RtfWriter rtfWriter = new RtfWriter();
            rtfWriter.Write(stream, this);
        }
        /// <summary>
        /// Gets the RTF text.
        /// </summary>
        /// <returns></returns>
        internal string GetRtfText()
        {
            RtfWriter rtfWriter = new RtfWriter();
            return rtfWriter.GetRtfText(this);
        }
        /// <summary>
        /// Saves the document in text format.
        /// </summary>
        /// <param name="stream">The name of target file</param>
        internal void SaveTxt(Stream stream)
        {
            CheckEvalExpired();
            StreamWriter writer = new StreamWriter(stream);
            try
            {
                TextConverter textConverter = new TextConverter();
                textConverter.Write(writer, this);
            }
            finally
            {
                writer.Flush();
            }
        }
        /// <summary>
        /// Opens the document in text format.
        /// </summary>
        /// <param name="stream">The name of target file</param>
        internal void OpenTxt(Stream stream)
        {
#if (SILVERLIGHT || WP) && !WINRT
            Encoding encoding = new Windows1252Encoding();
#else
            Encoding encoding = Encoding.GetEncoding(DLSConstants.WindowsCodePage);
#endif
            if (Utf8Checker.IsUtf8(stream))
                encoding = Encoding.UTF8;
            StreamReader reader = new StreamReader(stream, encoding);
            TextConverter textConverter = new TextConverter();
            textConverter.Read(reader, this);
        }
        /// <summary>
        /// Opens the text.
        /// </summary>
        /// <param name="text">The text.</param>
        internal void OpenText(string text)
        {
            TextConverter textConverter = new TextConverter();
            textConverter.Read(text, this);
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Opens the document in HTML format
        /// </summary>
        /// <param name="stream">The file stream</param>
        /// <param name="validationType">XHTML validation type.</param>
        internal void OpenHTML(Stream stream, XHTMLValidationType validationType)
        {
            StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(DLSConstants.WindowsCodePage));
            string html = reader.ReadToEnd();
            reader.Close();

            if (this.Sections.Count == 0)
                this.AddSection();

            XHTMLValidateOption = validationType;
            LastSection.PageSetup.Margins.All = 72;
            LastSection.Body.InsertXHTML(html, 0);
            //Add Empty Paragraph to the Text Body Items
            if (LastSection.Body.Items.Count == 0 || LastSection.Body.Items.LastItem is WTable)
                LastSection.Body.Items.Insert(LastSection.Body.Items.Count, new WParagraph(Document));
        }
        /// <summary>
        /// Opens doc file.
        /// </summary>
        /// <param name="fileName"></param>
        private void OpenDoc(string fileName)
        {
            DocReaderAdapter adapter = new DocReaderAdapter();
            using (WordReader reader = new WordReader(fileName))
            {
                adapter.Read(reader, this);
            }
            adapter = null;
        }
        /// <summary>
        /// Opens the document.
        /// </summary>
        /// <param name="fileName"></param>
        public void Open(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException("FileName");
            this.WordDocumentType(fileName, null);
        }
        /// <summary>
        /// Opens the document from file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="formatType"></param>
        public void Open(string fileName, FormatType formatType)
        {
            if (fileName == null)
                throw new ArgumentNullException("FileName");
            OpenInternal(fileName, formatType, null);
        }
        /// <summary>
        /// Opens the HTML document from file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="formatType">Type of the format.</param>
        /// <param name="validationType">Type of the validation.</param>
        public void Open(string fileName, FormatType formatType, XHTMLValidationType validationType, string baseUrl)
        {
            if (fileName == null)
                throw new ArgumentNullException("FileName");
            if (baseUrl == null)
                throw new ArgumentNullException("BaseUrl");
            this.HtmlBaseUrl = baseUrl;
            OpenInternal(fileName, formatType, validationType);
        }
        /// <summary>
        /// Opens the HTML document from file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="formatType">Type of the format.</param>
        /// <param name="validationType">Type of the validation.</param>
        public void Open(string fileName, FormatType formatType, XHTMLValidationType validationType)
        {
            if (fileName == null)
                throw new ArgumentNullException("FileName");
            OpenInternal(fileName, formatType, validationType);
        }
        /// <summary>
        /// Opens the HTML document from file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="formatType">Type of the format.</param>
        /// <param name="validationType">Type of the validation.</param>
        private void OpenInternal(string fileName, FormatType formatType, XHTMLValidationType validationType)
        {
            if (formatType == FormatType.Automatic)
                formatType = this.GetFormatType(fileName);
            fileName = this.CheckExtension(fileName, formatType);
            UpdateFormatType(fileName, ref formatType);
            ActualFormatType = formatType;
            if (FormatType.Html == formatType)
            {
                using (Stream stream = new FileStream(fileName, FileMode.Open))
                {
                    Init();
                    HtmlBaseUrl = Path.GetDirectoryName(fileName).TrimEnd(new char[] {  '\\' });
                    OpenHTML(stream, validationType);
                }
            }
            else
            {
                Open(fileName, formatType);
            }
        }
        /// <summary>
        /// Opens the document from file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="formatType">Type of the format.</param>
        /// <param name="password">The password.</param>
        public void Open(string fileName, FormatType formatType, string password)
        {
            if (fileName == null)
                throw new ArgumentNullException("FileName");
            if (password == null)
                throw new ArgumentNullException("Password");
            OpenInternal(fileName, formatType, password);
        }
        /// <summary>
        /// Opens the document from file in Xml or Microsoft Word format.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="formatType">Type of the format.</param>
        /// <param name="password">The password.</param>
        private void OpenInternal(string fileName, FormatType formatType, string password)
        {
            Init();
            CheckFileName(fileName);
            this.Password = password;
            if (formatType == FormatType.Automatic)
                formatType = this.GetFormatType(fileName);
            fileName = CheckExtension(fileName, formatType);
            UpdateFormatType(fileName, ref formatType);
            ActualFormatType = formatType;
            switch (formatType)
            {
                case FormatType.Xml:
                    OpenXml(fileName);
                    break;
                case FormatType.Dot:
                case FormatType.Doc:
                    OpenDoc(fileName);
                    break;
                case FormatType.Docx:
                case FormatType.Word2007:
                case FormatType.Word2007Dotx:
                case FormatType.Word2007Docm:
                case FormatType.Word2007Dotm:               
                case FormatType.Word2010:
                case FormatType.Word2010Dotx:
                case FormatType.Word2010Docm:
                case FormatType.Word2010Dotm:
                case FormatType.Word2013:
                case FormatType.Word2013Dotx:
                case FormatType.Word2013Docm:
                case FormatType.Word2013Dotm:
                    OpenDocx(fileName);
                    break;
                case FormatType.Txt:
                    using (Stream stream = new FileStream(fileName, FileMode.Open))
                    {
                        OpenTxt(stream);
                    }
                    break;
                case FormatType.Html:
                    using (Stream stream = new FileStream(fileName, FileMode.Open))
                    {
                        this.HtmlBaseUrl = Path.GetDirectoryName(fileName);
                        Open(stream, formatType, XHTMLValidationType.Transitional);
                    }
                    break;
                case FormatType.Rtf:
                    using (Stream stream = new FileStream(fileName, FileMode.Open))
                    {
                        OpenRtf(stream );
                    }
                    break;
                default:
                    throw new NotSupportedException("DocIO do not support this file format");
            }
        }
        /// <summary>
        /// Updates the type of the format.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="formatType">Type of the format.</param>
        private void UpdateFormatType(string fileName, ref FormatType formatType)
        {
            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            stream.Position = 0L;
            // Checks whether the stream is encrypted Docx format document.
            if (CheckForEncryption(stream))
            {
                using (ICompoundFile file = this.CreateCompoundFile(stream))
                {
                    ICompoundStorage storage = file.RootStorage;
                    SecurityHelper securityHelper = new SecurityHelper();
                    // Gets the encryption type used in the document stream (Standard - Word2007 format and Agile - Word2010 format).
                    SecurityHelper.EncrytionType encryptionType = securityHelper.GetEncryptionType(storage);
                    stream.Position = 0L;
                    if (encryptionType != SecurityHelper.EncrytionType.None)
                    {
                        formatType = FormatType.Docx;
                        stream.Close();
                        return;
                    }
                }
            }
            stream.Position = 0L;
            byte[] buffer = new byte[5];
            // Checks whether the stream is Docx format document.
            if (((stream.Read(buffer, 0, 5) == 5) && (buffer[0] == 80))
                && (buffer[1] == 0x4b))
            {
                stream.Position = 0L;
                formatType = FormatType.Docx;
            }
#if !SILVERLIGHT && !WP
            // Checks whether the stream is RTF format document.
            else if ((((buffer[0] == 0x7b) && (buffer[1] == 0x5c))
                && ((buffer[2] == 0x72) && (buffer[3] == 0x74)))
                && (buffer[4] == 0x66))
            {
                stream.Position = 0L;
                formatType = FormatType.Rtf;
            }
#endif
            // Checks whether the stream is Doc format document.
            else
            {
                try
                {
                    WordReader reader = new WordReader(fileName);
                    // Updates the format type as Doc, if the document is Doc format.
                    formatType = FormatType.Doc;
                    // Close all the streams in the word reader.
                    reader.ReadDocumentEnd();
                    reader = null;
                }
                catch (Exception ex)
                {
                    // If the document is not Doc format then, preserves the initial format type.
                }
            }
            stream.Close();
        }

        /// <summary>
        /// Open new document in read-only mode.
        /// </summary>
        /// <param name="strFileName">File to open.</param>
        /// <param name="formatType">Type of the format.</param>   
        public void OpenReadOnly(string strFileName, FormatType formatType)
        {
            using (FileStream stream = new FileStream(strFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                m_isReadOnly = true;
                Open(stream, formatType);
            }
        }
        /// <summary>
        /// Saves to file in Microsoft Word format.
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string fileName)
        {
            Save(fileName, FormatType.Automatic);
        }
        /// <summary>
        /// Saves the document to file in specified format type.
        /// </summary>
        /// <param name="fileName">The FileName.</param>
        /// <param name="formatType">The FormatType.</param>
        public void Save(string fileName, FormatType formatType)
        {
            if (fileName == null)
                throw new ArgumentNullException("FileName");
            SaveInternal(fileName, formatType);
        }
        /// <summary>
        /// Saves the document into file in specified format type.
        /// </summary>
        /// <param name="fileName">The FileName.</param>
        /// <param name="formatType">The FormatType.</param>
        private void SaveInternal(string fileName, FormatType formatType)
        {
            if (UpdateFields)
                UpdateDocumentFields();
            // Updates the format type based the file extension.
            if (formatType == FormatType.Automatic)
                formatType = GetFormatType(fileName);
            SaveFormatType = formatType;
            switch (formatType)
            {
                case FormatType.Xml:
                    SaveXml(fileName);
                    break;
                case FormatType.Dot:
                    SaveDot(fileName);
                    break;
                case FormatType.Doc:
                    SaveDoc(fileName);
                    break;
                case FormatType.Txt:
                    SaveTxt(fileName);
                    break;
                case FormatType.Docx:
                case FormatType.Word2007:
                case FormatType.Word2007Dotx:
                case FormatType.Word2007Docm:
                case FormatType.Word2007Dotm:
                case FormatType.Word2010:
                case FormatType.Word2010Dotx:
                case FormatType.Word2010Docm:
                case FormatType.Word2010Dotm:
                case FormatType.Word2013:
                case FormatType.Word2013Dotx:
                case FormatType.Word2013Docm:
                case FormatType.Word2013Dotm:
                    SaveDocx(fileName);
                    break;
                case FormatType.Html:
                    HTMLExport htmlExport = new HTMLExport();
                    htmlExport.SaveAsXhtml(this, fileName);
                    break;
                case FormatType.Rtf:
                    SaveRtf(fileName);
                    break;
                case FormatType.EPub:
                    SaveEPub(fileName, null);
                    break;
            }
        }
        /// <summary>
        /// Saves the document as EPUB
        /// </summary>
        /// <param name="fileName">The Name of the file</param>
        /// <param name="coverImage">The cover image of the EPUB</param>
        public void SaveAsEpub(string fileName, WPicture coverImage)
        {
            if (UpdateFields)
                UpdateDocumentFields();
            SaveEPub(fileName, coverImage);
        }
        /// <summary>
        /// Saves the document as Doc format.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private void SaveDoc(string fileName)
        {
            CheckEvalExpired();
            DocWriterAdapter adapter = new DocWriterAdapter();

            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
            {
                using (WordWriter writer = new WordWriter(fileStream))
                {
                    adapter.Write(writer, this);
                    writer.Close();
                }
            }
            adapter.Close();
            adapter = null;
        }
#if MVC
        /// <summary>
        /// Save as ActionResult
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="formatType">The Type.</param>
        /// <param name="response">The Response.</param>
        /// <param name="contentDisposition">The Content.</param>
        /// <returns></returns>
        public DocumentResult SaveAsActionResult(string fileName, FormatType formatType, HttpResponse response,
          HttpContentDisposition contentDisposition)
        {
            return new DocumentResult(this, fileName, formatType, response, contentDisposition);
        }
#endif
#if !CLIENTPROFILE
        /// <summary>
        /// Saves the specified file name.
        /// </summary>
        /// <param name="fileName">The FileName.</param>
        /// <param name="formatType">The FormatType.</param>
        /// <param name="response">The response.</param>
        /// <param name="contentDisposition">The content disposition.</param>
        public void Save(string fileName, FormatType formatType, HttpResponse response, HttpContentDisposition contentDisposition)
        {
            if (fileName == null)
                throw new ArgumentNullException("FileName");
            if (response == null)
                throw new ArgumentNullException("HttpResponse");
            SaveInternal(fileName, formatType, response, contentDisposition);
        }
        /// <summary>
        /// Saves the specified file name.
        /// </summary>
        /// <param name="fileName">The FileName.</param>
        /// <param name="formatType">The FormatType.</param>
        /// <param name="response">The response.</param>
        /// <param name="contentDisposition">The content disposition.</param>
        private void SaveInternal(string fileName, FormatType formatType, HttpResponse response, HttpContentDisposition contentDisposition)
        {
            if (UpdateFields) 
                UpdateDocumentFields();
            CheckEvalExpired();
            fileName = Path.GetFileName(fileName);
            // Updates the format type based the file extension.
            if (formatType == FormatType.Automatic)
                formatType = GetFormatType(fileName);
            SaveFormatType = formatType;
            response.Clear();
            //response.ClearHeaders();
            //response.ClearContent();

            string strContentType = string.Empty;

            switch (formatType)
            {
                case FormatType.Dot:
                case FormatType.Doc:
                    strContentType = "application/msword";
                    break;
                case FormatType.Docx:
                case FormatType.Word2007:
                case FormatType.Word2007Dotx:
                case FormatType.Word2007Docm:
                case FormatType.Word2007Dotm:
                    strContentType = "application/vnd.ms-word.document.12";
                    break;
                case FormatType.Word2010:
                case FormatType.Word2010Dotx:
                case FormatType.Word2010Docm:
                case FormatType.Word2010Dotm:
                    strContentType = "application/vnd.ms-word.document.14";
                    break;
                case FormatType.Word2013:
                case FormatType.Word2013Dotx:
                case FormatType.Word2013Docm:
                case FormatType.Word2013Dotm:
                    strContentType = "application/vnd.ms-word.document.15";
                    break;
                case FormatType.Xml:
                    strContentType = "application/xml";
                    break;
                case FormatType.EPub:
                    strContentType = "application/epub+zip";
                    break;
            }

            string strDispType = (contentDisposition == HttpContentDisposition.InBrowser)
              ? "inline"
              : "attachment";

            response.AddHeader("Content-Type", strContentType);
            response.AddHeader("Content-Disposition", string.Format("{0};filename={1};"
              , strDispType, fileName));

            Save(response.OutputStream, formatType);
            response.End();
        }
        /// <summary>
        /// Saves the word document as EPUB
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="coverImage">The Cover image for the EPUB.</param>
        /// <param name="response">The response.</param>
        /// <param name="contentDisposition">The content disposition.</param>
        public void SaveAsEpub(string fileName, WPicture coverImage, HttpResponse response,
          HttpContentDisposition contentDisposition)
        {
            if (UpdateFields)
                UpdateDocumentFields();
            CheckEvalExpired();
            fileName = Path.GetFileName(fileName);
            SaveFormatType = FormatType.EPub;
            response.Clear();
            string strContentType = "application/epub+zip";

            string strDispType = (contentDisposition == HttpContentDisposition.InBrowser)
              ? "inline"
              : "attachment";

            response.AddHeader("Content-Type", strContentType);
            response.AddHeader("Content-Disposition", string.Format("{0};filename={1};"
              , strDispType, fileName));

            SaveEPub(response.OutputStream,coverImage);
            response.End();
        }
#endif
        /// <summary>
        /// Saves the dot.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private void SaveDot(string fileName)
        {
            CheckEvalExpired();
            DocWriterAdapter adapter = new DocWriterAdapter();

            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
            {
                using (WordWriter writer = new WordWriter(fileStream))
                {
                    writer.IsTemplate = true;
                    adapter.Write(writer, this);
                }
            }
            adapter = null;
        }
        /// <summary>
        /// Saves the document in RTF format.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private void SaveRtf(string fileName)
        {
            CheckEvalExpired();
            RtfWriter rtfWriter = new RtfWriter();
            rtfWriter.Write(fileName, this);
        }
        /// <summary>
        /// Opens the HTML document from stream .
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="formatType">Type of the format.</param>
        /// <param name="validationType">Type of the validation.</param>
        /// <param name="baseUrl">Base url</param>
        public void Open(Stream stream, FormatType formatType, XHTMLValidationType validationType, string baseUrl)
        {
            if (stream == null)
                throw new ArgumentNullException("Stream");
            if (baseUrl == null)
                throw new ArgumentNullException("BaseUrl");
            this.HtmlBaseUrl = baseUrl;
            OpenInternal(stream, formatType, validationType);
        }
        /// <summary>
        /// Opens the HTML document from stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="formatType">Type of the format.</param>
        /// <param name="validationType">Type of the validation.</param>
        public void Open(Stream stream, FormatType formatType, XHTMLValidationType validationType)
        {
            if (stream == null)
                throw new ArgumentNullException("Stream");
            OpenInternal(stream, formatType, validationType);
        }
        /// <summary>
        /// Opens the HTML document from stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="formatType">Type of the format.</param>
        /// <param name="validationType">Type of the validation.</param>
        private void OpenInternal(Stream stream, FormatType formatType, XHTMLValidationType validationType)
        {
            UpdateFormatType(stream, ref formatType);
            ActualFormatType = formatType;
            if (FormatType.Html == formatType)
            {
                Init();
                OpenHTML(stream, validationType);
            }
            else
            {
                Open(stream, formatType);
            }
        }       
#endif
#if WINRT || WP
        /// <summary>
        /// Opens the document from storage file.
        /// </summary>
        /// <param name="file">The StorageFile.</param>
        /// <returns>A task that represents the asynchronous open operation.</returns>
        public async Task<bool> OpenAsync(StorageFile file)
        {
            if (file == null)
                throw new ArgumentNullException("StorageFile");
            return await OpenAsyncInternal(file, FormatType.Doc, null);
        }
        /// <summary>
        /// Opens the document from storage file.
        /// </summary>
        /// <param name="file">The StorageFile.</param>
        /// <param name="formatType">The FormatType.</param>
        /// <returns>A task that represents the asynchronous open operation.</returns>
        public async Task<bool> OpenAsync(StorageFile file, FormatType formatType)
        {
            if (file == null)
                throw new ArgumentNullException("StorageFile");
            return await OpenAsyncInternal(file, formatType, null);
        }
        /// <summary>
        /// Opens the document from storage file.
        /// </summary>
        /// <param name="file">The StorageFile.</param>
        /// <param name="formatType">The FormatType.</param>
        /// <param name="password">The Password.</param>
        /// <returns>A task that represents the asynchronous open operation.</returns>
        public async Task<bool> OpenAsync(StorageFile file, FormatType formatType, string password)
        {
            if (file == null)
                throw new ArgumentNullException("StorageFile");
            if (password == null)
                throw new ArgumentNullException("Password");
            return await OpenAsyncInternal(file, formatType, password);
        }
        /// <summary>
        /// Opens the document from storage file.
        /// </summary>
        /// <param name="file">The StorageFile.</param>
        /// <param name="formatType">The FormatType.</param>
        /// <param name="password">The Password.</param>
        /// <returns>A task that represents the asynchronous open operation.</returns>
        private async Task<bool> OpenAsyncInternal(StorageFile file, FormatType formatType, string password)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            await Task.Run(async () =>
            {
                try
                {
                    Stream stream = await file.OpenStreamForReadAsync();
                    OpenInternal(stream, formatType, password);
                    stream.Dispose();
                    tcs.SetResult(true);
                }
                catch (Exception exp)
                {
                    tcs.SetException(exp);
                }
            });
            return await tcs.Task;
        }
        /// <summary>
        /// Opens the document from stream.
        /// </summary>
        /// <param name="stream">The Stream.</param>
        /// <param name="formatType">The FormatType.</param>
        /// <returns>A task that represents the asynchronous open operation.</returns>
        public async Task<bool> OpenAsync(Stream stream, FormatType formatType)
        {
            if (stream == null)
                throw new ArgumentNullException("Stream");
            return await OpenAsyncInternal(stream, formatType, null);
        }
        /// <summary>
        /// Opens the document from stream.
        /// </summary>
        /// <param name="stream">The Stream.</param>
        /// <param name="formatType">The FormatType.</param>
        /// <param name="password">The Password.</param>
        /// <returns>A task that represents the asynchronous open operation.</returns>
        public async Task<bool> OpenAsync(Stream stream, FormatType formatType, string password)
        {
            if (stream == null)
                throw new ArgumentNullException("Stream");
            if (password == null)
                throw new ArgumentNullException("Password");
             return await OpenAsyncInternal(stream, formatType, password);
        }
        /// <summary>
        /// Opens the document from stream.
        /// </summary>
        /// <param name="stream">The Stream.</param>
        /// <param name="formatType">The FormatType.</param>
        /// <param name="password">The Password.</param>
        /// <returns>A task that represents the asynchronous open operation.</returns>
        private async Task<bool> OpenAsyncInternal(Stream stream, FormatType formatType, string password)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            await Task.Run(() =>
            {
                try
                {
                    OpenInternal(stream, formatType, password);
                    tcs.SetResult(true);
                }
                catch (Exception exp)
                {
                    tcs.SetException(exp);
                }
            });
            return await tcs.Task;
        }
        /// <summary>
        /// Saves the document into stream in specified format type.
        /// </summary>
        /// <param name="file">The StorageFile.</param>
        /// <param name="formatType">The FormatType.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        public async Task<bool> SaveAsync(StorageFile file, FormatType formatType)
        {
            if (file == null)
                throw new ArgumentNullException("StorageFile");
            return await SaveAsyncInternal(file, formatType);
        }
        /// <summary>
        /// Saves the document into stream in specified format type.
        /// </summary>
        /// <param name="file">The StorageFile.</param>
        /// <param name="formatType">The FormatType.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        private async Task<bool> SaveAsyncInternal(StorageFile file, FormatType formatType)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            await Task.Run(async () =>
            {
                try
                {
                    Stream stream = await file.OpenStreamForWriteAsync();
                    SaveInternal(stream, formatType);
                    stream.Dispose();
                    tcs.SetResult(true);
                }
                catch (Exception exp)
                {
                    tcs.SetException(exp);
                }
            });
            return await tcs.Task;
        }
        /// <summary>
        /// Saves the document into stream in specified format type.
        /// </summary>
        /// <param name="stream">The Stream.</param>
        /// <param name="formatType">The FormatType.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        public async Task<bool> SaveAsync(Stream stream, FormatType formatType)
        {
            if (stream == null)
                throw new ArgumentNullException("Stream");
            return await SaveAsyncInternal(stream, formatType);
        }
        /// <summary>
        /// Saves the document into stream in specified format type.
        /// </summary>
        /// <param name="stream">The Stream.</param>
        /// <param name="formatType">The FormatType.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        private async Task<bool> SaveAsyncInternal(Stream stream, FormatType formatType)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            await Task.Run(() =>
            {
                try
                {
                    SaveInternal(stream, formatType);
                    tcs.SetResult(true);
                }
                catch (Exception exp)
                {
                    tcs.SetException(exp);
                }
            });
            return await tcs.Task;
        }
#endif
        /// <summary>
        /// Opens the document from stream.
        /// </summary>
        /// <param name="stream">The Stream.</param>
        /// <param name="formatType">The FormatType.</param>
        public void Open(Stream stream, FormatType formatType)
        {
            if (stream == null)
                throw new ArgumentNullException("Stream");
            OpenInternal(stream, formatType, null);
        }
        /// <summary>
        /// Opens the document from stream.
        /// </summary>
        /// <param name="stream">The Stream.</param>
        /// <param name="formatType">The FormatType.</param>
        /// <param name="password">The Password.</param>
        public void Open(Stream stream, FormatType formatType, string password)
        {
            if (stream == null)
                throw new ArgumentNullException("Stream");
            if (password == null)
                throw new ArgumentNullException("Password");
            OpenInternal(stream, formatType, password);
        }
        /// <summary>
        /// Opens the document from stream.
        /// </summary>
        /// <param name="stream">The Stream.</param>
        /// <param name="formatType">The FormatType.</param>
        /// <param name="password">The Password.</param>
        private void OpenInternal(Stream stream, FormatType formatType, string password)
        {
            Init();
            this.Password = password;
            UpdateFormatType(stream, ref formatType);
            ActualFormatType = formatType;
            switch (formatType)
            {
                case FormatType.Dot:
                case FormatType.Doc:
                    DocReaderAdapter adapter = new DocReaderAdapter();
                    using (WordReader reader = new WordReader(stream))
                    {
                        adapter.Read(reader, this);
                    }
                    adapter = null;
                    break;
                case FormatType.Word2007:
                case FormatType.Docx:
                case FormatType.Word2007Dotx:
                case FormatType.Word2007Docm:
                case FormatType.Word2007Dotm:
                case FormatType.Word2010:
                case FormatType.Word2010Dotx:
                case FormatType.Word2010Docm:
                case FormatType.Word2010Dotm:
                case FormatType.Word2013:
                case FormatType.Word2013Dotx:
                case FormatType.Word2013Docm:
                case FormatType.Word2013Dotm:
                    OpenDocx(stream);
                    break;
                case FormatType.Rtf:
                    OpenRtf(stream);
                    break;
                case FormatType.Txt:
                    OpenTxt(stream);
                    break;
#if !SILVERLIGHT && !WP
                case FormatType.Xml:
                    OpenXml(stream);
                    break;
                case FormatType.Html:
                    Open(stream, formatType, XHTMLValidationType.Transitional);
                    break;
#endif
#if WINRT
                case FormatType.Html:
                    OpenHTML(stream);
                    break;
#endif
                default:
                    throw new NotSupportedException("DocIO do not support this file format");
            }
        }
        /// <summary>
        /// Open an Rtf file
        /// </summary>
        internal void OpenRtf(Stream stream)
        {
            this.IsOpening = true;

            RtfParser parser = new RtfParser(this, stream);
            parser.ParseToken();

            this.IsOpening = false;

        }
        /// <summary>
        /// Opens the RTF.
        /// </summary>
        /// <param name="rtfText">The RTF text.</param>
        internal void OpenRtf(string rtfText)
        {
            this.IsOpening = true;
            MemoryStream stream = new MemoryStream();
#if (SILVERLIGHT || WP) && !WINRT
            StreamWriter writer = new StreamWriter(stream, new ASCIIEncoding());
#else
            StreamWriter writer = new StreamWriter(stream, Encoding.GetEncoding("ASCII"));
#endif
            writer.Write(rtfText);
            rtfText = "";
            writer.Flush();
            stream.Position = 0;
            RtfParser parser = new RtfParser(this, stream);
            parser.ParseToken();
            writer.Dispose();
            this.IsOpening = false;
        }
#if WINRT
        /// <summary>
        /// Opens the document in HTML format
        /// </summary>
        /// <param name="stream">The file stream</param>
        /// <param name="validationType">XHTML validation type.</param>
        internal void OpenHTML(Stream stream)
        {
            StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(DLSConstants.WindowsCodePage));
            string html = reader.ReadToEnd();
            reader.Dispose();

            if (this.Sections.Count == 0)
                this.AddSection();
            LastSection.PageSetup.Margins.All = 72;
            LastSection.Body.InsertXHTML(html, 0);
            //Add Empty Paragraph to the Text Body Items
            if (LastSection.Body.Items.Count == 0 || LastSection.Body.Items.LastItem is WTable)
                LastSection.Body.Items.Insert(LastSection.Body.Items.Count, new WParagraph(Document));
        }
#endif
        /// <summary>
        /// Updates the type of the format.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="formatType">Type of the format.</param>
        private void UpdateFormatType(Stream stream, ref FormatType formatType)
        {
#if !SILVERLIGHT && !WP
            if (formatType == FormatType.Automatic)
            {
                if (stream is FileStream)
                    formatType = this.GetFormatType((stream as FileStream).Name);
                else
                    formatType = FormatType.Doc;
            }
#endif
            stream.Position = 0L;
            // Checks whether the stream is encrypted Docx format document.
            if (CheckForEncryption(stream))
            {
                using (ICompoundFile file = this.CreateCompoundFile(stream))
                {
                    ICompoundStorage storage = file.RootStorage;
                    SecurityHelper securityHelper = new SecurityHelper();
                    // Gets the encryption type used in the document stream (Standard - Word2007 format and Agile - Word2010 format).
                    SecurityHelper.EncrytionType encryptionType = securityHelper.GetEncryptionType(storage);
                    stream.Position = 0L;
                    if (encryptionType != SecurityHelper.EncrytionType.None)
                    {
                        formatType = FormatType.Docx;
                        return;
                    }
                }
            }
            stream.Position = 0L;
            byte[] buffer = new byte[5];
            // Checks whether the stream is Docx format document.
            if (((stream.Read(buffer, 0, 5) == 5) && (buffer[0] == 80))
                && (buffer[1] == 0x4b))
            {
                stream.Position = 0L;
                formatType = FormatType.Docx;
            }
#if (!SILVERLIGHT && !WP) || WINRT
            // Checks whether the stream is RTF format document.
            else if ((((buffer[0] == 0x7b) && (buffer[1] == 0x5c))
                && ((buffer[2] == 0x72) && (buffer[3] == 0x74)))
                && (buffer[4] == 0x66))
            {
                stream.Position = 0L;
                formatType = FormatType.Rtf;
            }
#endif
            // Checks whether the stream is Doc format document.
            else
            {
                try
                {
                    stream.Position = 0L;
                    WordReader reader = new WordReader(stream);
                    // Updates the format type as Doc, if the document is Doc format.
                    formatType = FormatType.Doc;
                    // Close all the streams in the word reader.
                    reader.ReadDocumentEnd();
                    reader = null;
                }
                catch (Exception ex)
                {
                    // If the document is not Doc format then, preserves the initial format type.
                }
            }
            stream.Position = 0L;
        }
        /// <summary>
        /// Saves the document into stream in specified format type.
        /// </summary>
        /// <param name="stream">The Stream.</param>
        /// <param name="formatType">The FormatType.</param>
        public void Save(Stream stream, FormatType formatType)
        {
            if (stream == null)
                throw new ArgumentNullException("Stream");
            SaveInternal(stream, formatType);
        }
        /// <summary>
        /// Saves the document into stream in specified format type.
        /// </summary>
        /// <param name="stream">The Stream.</param>
        /// <param name="formatType">The FormatType.</param>
        private void SaveInternal(Stream stream, FormatType formatType)
        {
            //Resets the stream length to origin 0.
            if (stream.CanSeek)
                stream.SetLength(0);
            if (UpdateFields) 
                UpdateDocumentFields();
            SaveFormatType = formatType;
            switch (formatType)
            {
                case FormatType.Dot:
                case FormatType.Doc:
                    DocWriterAdapter adapter = new DocWriterAdapter();
                    using (WordWriter writer = new WordWriter(stream))
                    {
                        if (formatType == FormatType.Dot)
                            writer.IsTemplate = true;
                        adapter.Write(writer, this);
                    }
                    adapter = null;
                    break;
                case FormatType.Docx:
                case FormatType.Word2007:
                case FormatType.Word2007Dotx:
                case FormatType.Word2007Docm:
                case FormatType.Word2007Dotm:
                case FormatType.Word2010:
                case FormatType.Word2010Dotx:
                case FormatType.Word2010Docm:
                case FormatType.Word2010Dotm:
                case FormatType.Word2013:
                case FormatType.Word2013Dotx:
                case FormatType.Word2013Docm:
                case FormatType.Word2013Dotm:
                    SaveDocx(stream);
                    break;
                case FormatType.Rtf:
                    SaveRtf(stream);
                    break;
                case FormatType.Txt:
                    SaveTxt(stream);
                    break;
#if !SILVERLIGHT && !WP
                case FormatType.EPub:
                    SaveEPub(stream, null);
                    break;
                case FormatType.Xml:
                    SaveXml(stream);
                    break;
                    break;
                case FormatType.Html:
                    HTMLExport htmlExport = new HTMLExport();
                    htmlExport.SaveAsXhtml(this, stream);
                    break;
                case FormatType.Automatic:
                    throw new Exception("Please provide appropriate format type other than Automatic.");
                    break;
#endif
#if WINRT
                case FormatType.Html:
                    SaveHTML(stream);
                    break;
#endif
            }
        }
        #region Implementation to retreive Ordinal List Value
        /// <summary>
        /// Get's Ordinal List Value
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        internal string GetOrdinal(int num, WCharacterFormat characterFormat)
        {
            switch (characterFormat.LocaleIdASCII)
            {
                case 1069:
                case 8218:
                case 5146:
                case 4122:
                case 1050:
                case 1029:
                case 1061:
                case 1035:
                case 3079:
                case 1031:
                case 5127:
                case 4103:
                case 2055:
                case 1038:
                case 1060:
                case 1055:
                case 1044:
                case 2068:
                case 1045:
                case 6170:
                case 2074:
                    //Returns ordinal in Czech
                    return num.ToString() + ".";
                case 2060:
                case 11276:
                case 3084:
                case 9228:
                case 12300:
                case 1036:
                case 15372:
                case 5132:
                case 13324:
                case 6156:
                case 14348:
                case 8204:
                case 10252:
                case 4108:
                    //Returns Ordinal in French
                    if (num == 1)
                        return num.ToString() + "er";
                    else
                        return num.ToString() + "e";
                case 2067:
                case 1043:
                    //Returns Ordinal in Dutch
                    return num.ToString() + "e";
                case 1032:
                    //Returns Ordinal in Greek 
                    return num.ToString() + "o";
                case 1040:
                case 2064:
                    //Returns Ordinal in Italian
                    return num.ToString() + (char)176;
                case 5130:
                case 7178:
                case 12298:
                case 17418:
                case 4106:
                case 1046:
                case 2070:
                case 11274:
                case 16394:
                case 13322:
                case 9226:
                case 18442:
                case 2058:
                case 19466:
                case 6154:
                case 15370:
                case 10250:
                case 20490:
                case 3082:
                case 1034:
                case 21514:
                case 14346:
                case 8202:
                    //Returns Ordinal in Spanish
                    return num.ToString() + (char)186;
                case 1049:
                case 2073:
                    //Returns Ordinal in Russian
                    return num.ToString() + "-" + (char)1081;
                case 2077:
                case 1053:
                    //Returns Ordinal in Swedish
                    return GetOrdinalInSwedish(num);
                case 1027:
                    //Returns Ordinal in Catalan
                    return GetOrdinalInCatalan(num);
                case 1030:
                    //Returns Ordinal in Danish
                    return GetOrdinalInDanish(num);
                default:
                    //Retruns Ordinal in English (Default)
                    return GetOrdinalInEnglish(num);
            }
        }
        /// <summary>
        /// Get's Ordinal in Swedish
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private string GetOrdinalInSwedish(int num)
        {
            if (num == 11 || num == 12)
            {
                return num.ToString() + ":e";
            }
            else if ((num % 10) == 1 || (num % 10) == 2)
            {
                return num.ToString() + ":a";
            }
            else
                return num.ToString() + ":e";
        }
        /// <summary>
        /// Get's Ordinal in Catalan
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private string GetOrdinalInCatalan(int num)
        {
            switch (num)
            {
                case 1:
                    return num.ToString() + ".";
                case 2:
                    return num.ToString() + "n";
                case 3:
                    return num.ToString() + "r";
                case 4:
                    return num.ToString() + "t";
                case 14:
                    return num.ToString() + (char)232 + "h";
                default:
                    return num.ToString() + (char)232;
            }
        }
        /// <summary>
        /// Get's Ordinal in Danish
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private string GetOrdinalInDanish(int num)
        {
            if (num == 0)
                return num.ToString() + "te";
            switch (num % 100)
            {
                case 0:
                    return num.ToString() + "ende";
                case 1:
                    return num.ToString() + "ste";
                case 2:
                    return num.ToString() + "nden";
                case 3:
                    return num.ToString() + "dje";
                case 4:
                    return num.ToString() + "rde";
                case 5:
                case 6:
                case 11:
                case 12:
                case 30:
                    return num.ToString() + "te";
                default:
                    return num.ToString() + "nde";
            }
        }
        /// <summary>
        /// Get's Ordinal in English (Default)
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private string GetOrdinalInEnglish(int num)
        {
            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num.ToString() + "th";
            }
            switch (num % 10)
            {
                case 1:
                    return num.ToString() + "st";
                case 2:
                    return num.ToString() + "nd";
                case 3:
                    return num.ToString() + "rd";
                default:
                    return num.ToString() + "th";
            }
        }
        #endregion
#if WINRT
        /// <summary>
        /// Saves the HTML
        /// </summary>
        /// <param name="stream">The stream.</param>
        private void SaveHTML(Stream stream)
        {
            CheckEvalExpired();
            HTMLExport htmlExport = new HTMLExport();
            htmlExport.SaveAsXhtml(this, stream);
        }
#endif
       
        #endregion

        #region Public methods / close
        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            m_bIsClosing = true;
            // Clear collections
            CloseContent();
            //GC.Collect causes performance overhead 
            //GC.Collect();
            GC.WaitForPendingFinalizers();
            //GC.Collect();

            m_doc = this;
            Init();
            m_bIsClosing = false;
        }
        /// <summary>
        /// 
        /// </summary>
        private void CloseContent()
        {
//#if !SILVERLIGHT
            m_builtinProp = null;
            m_customProp = null;
//#endif

            CloseSecContent();
            CloseStyles();
            if (m_imageCollection != null)
            {
                m_imageCollection.Clear();
                m_imageCollection = null;
            }

            if (m_escher != null)
            {
                m_escher.Close();
                m_escher = null;
            }

            if (m_bookmarks != null)
            {
                m_bookmarks.Clear();
                m_bookmarks = null;
            }
            if (m_txbxItems != null)
            {
                m_txbxItems.Clear();
                m_txbxItems = null;
            }

            m_mailMerge = null;

            m_viewSetup = null;
            m_watermark = null;
            m_background = null;
            m_dop = null;
            m_grammarSpellingData = null;

            m_password = null;
            m_macrosData = null;
            m_escherDataContainers = null;
            m_escherContainers = null;
            m_macroCommands = null;
            m_defShapeId = 1;
            m_standardAsciiFont = null;
            m_standardFarEastFont = null;
            m_standardNonFarEastFont = null;
            m_throwUnsupportedExceptions = false;
            m_curClonedSection = null;

            if (m_defCharFormat != null)
            {
                m_defCharFormat.Close();
                m_defCharFormat = null;
            }
            if (m_defParaFormat != null)
            {
                m_defParaFormat.Close();
                m_defParaFormat = null;
            }
            m_fontSubstitutionTable = null;
#if !SILVERLIGHT && !WP
            m_htmlValidationOption = XHTMLValidationType.Transitional;
#endif
            m_latentStyles = null;
            
            m_docxPackage = null;
            
            m_docxProps = null;
            m_importStyles = true;
            m_variables = null;
            m_curClonedSection = null;

            m_nextParaItem = null;
            m_prevBodyItem = null;
        }
        /// <summary>
        /// Closes the content of the sections.
        /// </summary>
        private void CloseSecContent()
        {
            // Clear document data information.
            if (m_sections != null && m_sections.Count > 0)
            {
                WSection sec = null;
                for (int i = 0; i < m_sections.Count; i++)
                {
                    sec = m_sections[i];
                    sec.Close();
                    sec = null;
                }
            }

            m_sections.Clear();
            m_sections = null;
        }
        /// <summary>
        /// Closes the styles.
        /// </summary>
        private void CloseStyles()
        {
            // Clear Styles
            int stylesCnt = m_styles.Count;
            Style style = null;
            for (int j = 0; j < stylesCnt; j++)
            {
                style = m_styles[j] as Style;
                style.Close();
                style = null;
            }

            (m_styles as StyleCollection).InnerList.Clear();
            m_styles = null;

            // Close list styles
            if (m_listStyles != null)
            {
                stylesCnt = m_listStyles.Count;
                ListStyle listStyle = null;
                for (int i = 0; i < stylesCnt; i++)
                {
                    listStyle = m_listStyles[i];
                    listStyle.Close();
                    listStyle = null;
                }

                (m_listStyles as ListStyleCollection).InnerList.Clear();
                m_listStyles = null;
            }

            // Close list override styles
            if (m_listOverrides != null)
            {
                stylesCnt = m_listOverrides.Count;
                ListOverrideStyle overStyle = null;
                for (int k = 0; k < stylesCnt; k++)
                {
                    overStyle = m_listOverrides[k];
                    overStyle.Close();
                }

                (m_listOverrides as StyleCollection).InnerList.Clear();
                m_listOverrides = null;
            }
        }

        #endregion

        #region Public methods / Doc to Image
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Converts the whole document into images
        /// </summary>
        /// <param name="type">The ImageType</param>
        /// <returns>Return the images</returns>
        /// <remarks>Layouting of the pages is not exactly the same as the layouting made by Microsoft Word. The total number of pages and layouting of the elements may vary.</remarks>
        public Image[] RenderAsImages(ImageType type)
        {
            Syncfusion.DocIO.DLS.Convertors.WordToImageConverter converter = new Syncfusion.DocIO.DLS.Convertors.WordToImageConverter();
            return converter.ConvertToImage(this, type);
        }
        /// <summary>
        /// Converts the specified page into image
        /// </summary>
        /// <param name="pageIndex">Zero based page index</param>
        /// <param name="imageFormat">The ImageFormat</param>
        /// <returns>Returns the image as stream</returns>
        /// <remarks>Layouting of the pages is not exactly the same as the layouting made by MS-Word. The total number of pages and layouting of the elements may vary.</remarks>
        public Stream RenderAsImages(int pageIndex, System.Drawing.Imaging.ImageFormat imageFormat)
        {
            Syncfusion.DocIO.DLS.Convertors.WordToImageConverter converter = new Syncfusion.DocIO.DLS.Convertors.WordToImageConverter();
            return converter.ConvertToImage(pageIndex, this, imageFormat);
        }
        /// <summary>
        /// Converts the specified page into image
        /// </summary>
        /// <param name="pageIndex">Zero based page index</param>
        /// <param name="type"> The ImageType</param>
        /// <returns>Returns the image</returns>
        /// <remarks>Layouting of the pages is not exactly the same as the layouting made by MS-Word. The total number of pages and layouting of the elements may vary.</remarks>
        public Image RenderAsImages(int pageIndex, ImageType type)
        {
            Syncfusion.DocIO.DLS.Convertors.WordToImageConverter converter = new Syncfusion.DocIO.DLS.Convertors.WordToImageConverter();
            Image[] images = converter.ConvertToImage(pageIndex, 1, this, type);
            if (images != null && images[pageIndex] != null)
                return images[pageIndex];
            else
                return null;
        }
        /// <summary>
        /// Converts the specified range of pages into images
        /// </summary>
        /// <param name="pageIndex">Starting page index (Zero based)</param>
        /// <param name="noOfPages">Number of pages</param>
        /// <param name="type">The ImageType</param>
        /// <returns>Return the images</returns>
        /// <remarks>Layouting of the pages is not exactly the same as the layouting made by MS-Word. The total number of pages and layouting of the elements may vary.</remarks>
        public Image[] RenderAsImages(int pageIndex, int noOfPages, ImageType type)
        {
            Syncfusion.DocIO.DLS.Convertors.WordToImageConverter converter = new Syncfusion.DocIO.DLS.Convertors.WordToImageConverter();
            Image[] Allimages = converter.ConvertToImage(pageIndex, noOfPages, this, type);
            List<Image> list = new List<Image>();
            for (int i = 0; i < Allimages.Length; i++)
            {
                if (Allimages[i] != null)
                    list.Add(Allimages[i]);
            }
            Image[] images = new Image[list.Count];
            for ( int i = 0; i < list.Count; i++)
            {
                images[i] = list[i] as Image;
            }
            return images;
        }
#endif

        #endregion

        #region Public methods / text find, replace

        /// <summary>
        /// Finds and returns entry of specified regular expression along with formatting.
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public TextSelection Find(Regex pattern)
        {
            foreach (WSection section in Sections)
            {
                foreach (WTextBody textBody in section.ChildEntities)
                {
                    TextSelection textSel = textBody.Find(pattern);

                    if (textSel != null)
                    {
                        return textSel;
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// Finds the first entry of specified pattern in single-line mode.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        public TextSelection[] FindSingleLine(Regex pattern)
        {
            TextSelection[] selections = null;
            foreach (WSection section in Sections)
            {
                selections = TextFinder.Instance.FindSingleLine(section.Body, pattern);
                if (selections != null)
                    break;
            }

            return selections;
        }
        /// <summary>
        /// Finds and returns entry of specified string along with formatting,
        /// taking into consideration caseSensitive and wholeWord options.
        /// </summary>
        /// <param name="given"></param>
        /// <param name="caseSensitive"></param>
        /// <param name="wholeWord"></param>
        /// <returns></returns>
        public TextSelection Find(string given, bool caseSensitive, bool wholeWord)
        {
            Regex pattern = FindUtils.StringToRegex(given, caseSensitive, wholeWord);

            return Find(pattern);
        }
        /// <summary>
        /// Finds the first entry of given text in single-line mode.
        /// </summary>
        /// <param name="given">The string to find.</param>
        /// <param name="caseSensitive">if set to <c>true</c> use case sensitive search.</param>
        /// <param name="wholeWord">if it search the whole word, set to <c>true</c>.</param>
        /// <returns></returns>
        public TextSelection[] FindSingleLine(string given, bool caseSensitive, bool wholeWord)
        {
            Regex pattern = FindUtils.StringToRegex(given, caseSensitive, wholeWord);
            return FindSingleLine(pattern);
        }
        /// <summary>
        /// Returns all entries of given regex.
        /// </summary>
        /// <param name="pattern"></param>
        public TextSelection[] FindAll(Regex pattern)
        {
            TextSelectionList allSelections = null;

            foreach (WSection section in Sections)
            {
                foreach (WTextBody textBody in section.ChildEntities)
                {
                    TextSelectionList selections = textBody.FindAll(pattern);

                    if (selections != null && selections.Count > 0)
                    {
                        if (allSelections == null)
                        {
                            allSelections = selections;
                        }
                        else
                        {
                            allSelections.AddRange(selections);
                        }
                    }
                }
            }

            return allSelections != null ? allSelections.ToArray() : null;
        }
        /// <summary>
        /// Returns all entries of given string, taking into consideration caseSensitive
        /// and wholeWord options.
        /// </summary>
        /// <param name="given"></param>
        /// <param name="caseSensitive"></param>
        /// <param name="wholeWord"></param>
        /// <returns></returns>
        public TextSelection[] FindAll(string given, bool caseSensitive, bool wholeWord)
        {
            Regex pattern = FindUtils.StringToRegex(given, caseSensitive, wholeWord);

            return FindAll(pattern);
        }
        /// <summary>
        /// Replaces all entries of given regular expression with replace string.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public int Replace(Regex pattern, string replace)
        {
            int changesMade = 0;

            foreach (WSection section in Sections)
            {
                foreach (WTextBody body in section.ChildEntities)
                {
                    changesMade += body.Replace(pattern, replace);

                    if (ReplaceFirst && changesMade > 0)
                    {
                        return changesMade;
                    }
                }
            }

            return changesMade;
        }
        /// <summary>
        /// Replaces all entries of given string with replace string, taking into
        /// consideration caseSensitive and wholeWord options.
        /// </summary>
        /// <param name="given"></param>
        /// <param name="replace"></param>
        /// <param name="caseSensitive"></param>
        /// <param name="wholeWord"></param>
        public int Replace(string given, string replace, bool caseSensitive, bool wholeWord)
        {
            Regex pattern = FindUtils.StringToRegex(given, caseSensitive, wholeWord);

            return Replace(pattern, replace);
        }
        /// <summary>
        /// Replaces all entries of given string with TextSelection, taking into
        /// consideration caseSensitive and wholeWord options.
        /// </summary>
        /// <param name="given">The given.</param>
        /// <param name="textSelection">The text selection.</param>
        /// <param name="caseSensitive">if it is case sensitive, set to <c>true</c>.</param>
        /// <param name="wholeWord">if it specifies whole word, set to <c>true</c>.</param>
        /// <returns></returns>
        public int Replace(string given, TextSelection textSelection, bool caseSensitive, bool wholeWord)
        {
            return Replace(given, textSelection, caseSensitive, wholeWord, false);
        }
        /// <summary>
        /// Replaces all entries of given string with TextRangesHolder, taking into
        /// consideration caseSensitive and wholeWord options.
        /// </summary>
        /// <param name="given">The given.</param>
        /// <param name="textSelection">The text selection.</param>
        /// <param name="caseSensitive">if it is case sensitive, set to <c>true</c>.</param>
        /// <param name="wholeWord">if it specifies whole word, set to <c>true</c>.</param>
        /// <param name="saveFormatting">if it specifies save formatting, set to <c>true</c>.</param>
        /// <returns></returns>
        public int Replace(string given, TextSelection textSelection, bool caseSensitive, bool wholeWord, bool saveFormatting)
        {
            Regex pattern = FindUtils.StringToRegex(given, caseSensitive, wholeWord);

            return Replace(pattern, textSelection, saveFormatting);
        }
        /// <summary>
        /// Replaces all entries of given regular expression with TextRangesHolder.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="textSelection">The text selection.</param>
        /// <returns></returns>
        public int Replace(Regex pattern, TextSelection textSelection)
        {
            return Replace(pattern, textSelection, false);
        }
        /// <summary>
        /// Replaces all entries of given regular expression with TextRangesHolder.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="textSelection">The text selection.</param>
        /// <param name="saveFormatting">if it save source formatting, set to <c>true</c>.</param>
        /// <returns></returns>
        public int Replace(Regex pattern, TextSelection textSelection, bool saveFormatting)
        {
            textSelection.CacheRanges();
            int changesMade = 0;

            foreach (WSection section in Sections)
            {
                foreach (WTextBody textBody in section.ChildEntities)
                {
                    changesMade += textBody.Replace(pattern, textSelection, saveFormatting);

                    if (ReplaceFirst && changesMade > 0)
                    {
                        return changesMade;
                    }
                }
            }

            return changesMade;
        }
        /// <summary>
        /// Replaces the specified given.
        /// </summary>
        /// <param name="given">The given.</param>
        /// <param name="bodyPart">The body part.</param>
        /// <param name="caseSensitive">if it is case sensitive, set to <c>true</c>.</param>
        /// <param name="wholeWord">if it specifies whole word, set to <c>true</c>.</param>
        public int Replace(string given, TextBodyPart bodyPart, bool caseSensitive, bool wholeWord)
        {
            return Replace(given, bodyPart, caseSensitive, wholeWord, false);
        }
        /// <summary>
        /// Replaces the specified given.
        /// </summary>
        /// <param name="given">The given.</param>
        /// <param name="bodyPart">The body part.</param>
        /// <param name="caseSensitive">if it case sensitive, set to <c>true</c>.</param>
        /// <param name="wholeWord">if specifies whole word, set to <c>true</c>.</param>
        /// <param name="saveFormatting">if it specifies save source formatting, set to <c>true</c>.</param>
        /// <returns></returns>
        public int Replace(string given, TextBodyPart bodyPart, bool caseSensitive, bool wholeWord, bool saveFormatting)
        {
            Regex pattern = FindUtils.StringToRegex(given, caseSensitive, wholeWord);

            return Replace(pattern, bodyPart, saveFormatting);
        }
        /// <summary>
        /// Replaces the specified pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="bodyPart">The body part.</param>
        public int Replace(Regex pattern, TextBodyPart bodyPart)
        {
            return Replace(pattern, bodyPart, false);
        }
        /// <summary>
        /// Replaces the specified pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="bodyPart">The body part.</param>
        /// <param name="saveFormating">if it specifies save source formatting, set to <c>true</c>.</param>
        /// <returns></returns>
        public int Replace(Regex pattern, TextBodyPart bodyPart, bool saveFormatting)
        {
            int changesMade = 0;

            foreach (WSection section in Sections)
            {
                foreach (WTextBody textBody in section.ChildEntities)
                {
                    changesMade += textBody.Replace(pattern, bodyPart, saveFormatting);

                    if (ReplaceFirst && changesMade > 0)
                    {
                        return changesMade;
                    }
                }
            }

            return changesMade;
        }
        /// <summary>
        /// Replaces the specified given.
        /// </summary>
        /// <param name="given">The given.</param>
        /// <param name="replaceDoc">The replace doc.</param>
        /// <param name="caseSensitive">if it is case sensitive, set to <c>true</c>.</param>
        /// <param name="wholeWord">if specifies whole word,set to <c>true</c>.</param>
        /// <returns></returns>
        public int Replace(string given, IWordDocument replaceDoc, bool caseSensitive, bool wholeWord)
        {
            return Replace(given, replaceDoc, caseSensitive, wholeWord, false);
        }
        /// <summary>
        /// Replaces the specified given.
        /// </summary>
        /// <param name="given">The given.</param>
        /// <param name="replaceDoc">The replace doc.</param>
        /// <param name="caseSensitive">if it is case sensitive, set to <c>true</c>.</param>
        /// <param name="wholeWord">if it specifies whole word, set to <c>true</c>.</param>
        /// <param name="saveFormatting">if it specifies save source formatting, set to <c>true</c>.</param>
        /// <returns></returns>
        public int Replace(string given, IWordDocument replaceDoc, bool caseSensitive, bool wholeWord, bool saveFormatting)
        {
            Regex pattern = FindUtils.StringToRegex(given, caseSensitive, wholeWord);

            return Replace(pattern, replaceDoc, saveFormatting);
        }
        /// <summary>
        /// Replaces the specified pattern.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="replaceDoc">The replace doc.</param>
        /// <param name="saveFormatting">if it specifies save source formatting, set to <c>true</c>.</param>
        /// <returns></returns>
        public int Replace(Regex pattern, IWordDocument replaceDoc, bool saveFormatting)
        {
            int changesMade = 0;

            foreach (WSection section in Sections)
            {
                foreach (WTextBody textBody in section.ChildEntities)
                {
                    changesMade += textBody.Replace(pattern, replaceDoc, saveFormatting);

                    if (ReplaceFirst && changesMade > 0)
                    {
                        return changesMade;
                    }
                }
            }

            return changesMade;
        }

        #endregion


        #region Implementation / Update words count
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Update Paragraphs count, Word count and Character count
        /// </summary>
        public void UpdateWordCount()
        {
            UpdateWordCount(false);
        }
        /// <summary>
        /// Updates Paragraphs count, Word count and Character count. Updates page count if performLayout set to true using Doc to PDF layouting engine.
        /// </summary>
        /// <param name="performRelayout">By default performLayout set to false, to update page count using Doc to PDF layouting engine, set to <c>true</c>.</param>
        public void UpdateWordCount(bool performlayout)
        {
            m_paraCount = m_wordCount = m_charCount = 0;

            foreach (WSection sec in Sections)
            {
                CalculateForTextBody(sec.Body.Items);
            }
            if (performlayout)
            {
                Rendering.DocumentLayouter layouter = new Rendering.DocumentLayouter();
                layouter.UpdatePageFields(this);
                this.BuiltinDocumentProperties.PageCount = PageCount = layouter.Pages.Count;
                layouter.InitLayoutInfo();
            }
            BuiltinDocumentProperties.ParagraphCount = m_paraCount;
            BuiltinDocumentProperties.WordCount = m_wordCount;
            BuiltinDocumentProperties.CharCount = m_charCount;
        }
#endif
        /// <summary>
        /// Update fields present in the document.
        /// </summary>
        public void UpdateDocumentFields()
        {
#if !SILVERLIGHT && !WP
            //Parse the document using Doc to PDF layouting to update page fields.
            if (IsContainNumPagesField())
            {
                Rendering.DocumentLayouter layouter = new Rendering.DocumentLayouter();
                layouter.UpdatePageFields(this);
                PageCount = layouter.Pages.Count;
                layouter.InitLayoutInfo();
            }
#endif
            for (int i = 0; i < m_doc.Fields.Count; i++)
            {
                if (UpdatedFields.Contains(m_doc.Fields[i]))
                    continue;
                m_doc.Fields[i].Update();
            }
            UpdatedFields.Clear();
        }
        /// <summary>
        /// Checking for NumPages field present in the document
        /// </summary>
        private bool IsContainNumPagesField()
        {
            if (m_fields != null)
            {
                for (int i = 0; i < m_fields.Count; i++)
                {
                    if (m_doc.Fields[i].FieldType == FieldType.FieldNumPages)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// check all text body items
        /// </summary>
        /// <param name="bodyItems"></param>
        private void CalculateForTextBody(BodyItemCollection bodyItems)
        {
            foreach (TextBodyItem bodyItem in bodyItems)
            {
                if (bodyItem is WParagraph)
                {
                    CalculateForParagraphs(bodyItem as WParagraph);
                }
                else
                {
                    CalculateForTabls(bodyItem as WTable);
                }
            }
        }
        /// <summary>
        /// Check all cell in table
        /// </summary>
        /// <param name="table"></param>
        private void CalculateForTabls(WTable table)
        {
            foreach (WTableRow row in table.Rows)
            {
                foreach (WTableCell cell in row.Cells)
                {
                    CalculateForTextBody(cell.Items);
                }
            }
        }
        /// <summary>
        /// Calculate paragraphs count, word count and character count
        /// </summary>
        /// <param name="para"></param>
        private void CalculateForParagraphs(WParagraph para)
        {
            string paraText = para.Text;
            if (para.Text != string.Empty)
            {
                m_paraCount++;

                string[] words = paraText.Split(" ".ToCharArray());
                foreach (string word in words)
                {
                    if (word != string.Empty)
                        m_wordCount++;
                }

                paraText = paraText.Replace(" ", string.Empty);
                m_charCount += paraText.Length;
            }
        }
        #endregion

#if !SILVERLIGHT && !WP
        #region Implementation / Update TableOfContents
        /// <summary>
        /// Update Table of contents in the document.
        /// </summary>
        public void UpdateTableOfContents()
        {
            if (HasTOC)
            {
                TOC.UpdateTOCField();
                ClearLists();
            }
        }
        #endregion

        #region Implementation / Updates List
        // Handled this case for updating list in Doc to PDF conversion.
        /// <summary>
        /// Updates the list.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        internal string UpdateListValue(WParagraph paragraph, WListFormat listFormat, WListLevel level)
        {
            string styleName = listFormat.CustomStyleName;
            if (paragraph.IsInCell
                && (paragraph.OwnerTextBody as WTableCell).OwnerRow.OwnerTable.m_isTextBox)
                styleName += "_textbox";
            ListOverrideStyle listOverrideStyle = null;

            if (listFormat.LFOStyleName != null && listFormat.LFOStyleName.Length > 0)
                listOverrideStyle = ListOverrides.FindByName(listFormat.LFOStyleName);

            if ((listOverrideStyle != null
                && listOverrideStyle.OverrideLevels.HasOverrideLevel(level.LevelNumber)
                && listOverrideStyle.OverrideLevels[level.LevelNumber].OverrideStartAtValue
                && !PreviousListLevelOverrideStyle.Contains(listOverrideStyle.Name)))
            {
                EnsureLevelRestart(listFormat, styleName, true, level);
                //Add List Override style
                PreviousListLevelOverrideStyle.Add(listOverrideStyle.Name);
            }
            else if (paragraph.ListFormat.RestartNumbering)
                EnsureLevelRestart(listFormat, styleName, true, level);
            else if (PreviousListLevel.ContainsKey(styleName)
                && level.LevelNumber > PreviousListLevel[styleName])
                EnsureLevelRestart(listFormat, styleName, false, level);

            if (PreviousListLevel.ContainsKey(styleName))
                PreviousListLevel[styleName] = level.LevelNumber;
            else
                PreviousListLevel.Add(styleName, level.LevelNumber);
            string listValue = string.Empty;
            int startAt = 0;
            int listItemIndex = GetListItemIndex(listFormat, styleName, level);
            listValue = level.GetListItemText(listItemIndex, listFormat.ListType);
            startAt = GetListStartValue(listFormat, styleName, level);

            if (level.NumberPrefix != null
                && level.NumberPrefix.StartsWith("\0."))
                listValue = GetListValue(listFormat, styleName, level, startAt, listItemIndex);

            if (level.PatternType == ListPatternType.Bullet)
            {
                listValue = level.BulletCharacter;
            }
            return listValue;
        }
        /// <summary>
        /// Clears the list collection.
        /// </summary>
        /// <remarks></remarks>
        internal void ClearLists()
        {
            PreviousListLevel.Clear();
            PreviousListLevelOverrideStyle.Clear();
            Lists.Clear();
            ListNames.Clear();
        }
        /// <summary>
        /// Ensures the level restart.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="styleName">Name of the style.</param>
        /// <param name="fullRestart">if set to <c>true</c> full restart is performed.</param>
        private void EnsureLevelRestart(WListFormat format, string styleName, bool fullRestart,WListLevel listLevel)
        {
            if (m_listNames == null)
                return;
            ListOverrideStyle listOverrideStyle = null;
            if (format.LFOStyleName != null && format.LFOStyleName.Length > 0)
                listOverrideStyle = ListOverrides.FindByName(format.LFOStyleName);

            HybridDictionary levels = ListNames[styleName] as HybridDictionary;

            if (levels == null)
                return;

            ICollection keys = levels.Keys;
            IEnumerator enumerator = keys.GetEnumerator();
            int count = keys.Count;
            int[] levelNums = new int[count];
            int index = 0;

            while (enumerator.MoveNext())
            {
                levelNums[index] = (int)enumerator.Current;
                index++;
            }

            for (int i = 0; i < count; i++)
            {
                if (!fullRestart)
                {
                    if (levelNums[i] < listLevel.LevelNumber || format.CurrentListStyle.Levels[levelNums[i]].NoRestartByHigher)
                        continue;
                }
                int startAt = format.CurrentListStyle.Levels[listLevel.LevelNumber].StartAt;
                if (listOverrideStyle != null
                    && listOverrideStyle.OverrideLevels.HasOverrideLevel(listLevel.LevelNumber))
                {
                    startAt = 0;
                    if (listOverrideStyle.OverrideLevels[listLevel.LevelNumber].OverrideStartAtValue)
                    startAt = listOverrideStyle.OverrideLevels[format.ListLevelNumber].StartAt;
                }
                //Reset the list level start at value, based on the override style
                if (listLevel.LevelNumber == levelNums[i])
                    levels[levelNums[i]] = startAt;
            }
        }
        /// <summary>
        /// Gets the list item index value.
        /// </summary>
        /// <param name="format">The List format</param>
        /// <param name="styleName">Name of the style.</param>
        /// <returns></returns>
        private int GetListItemIndex(WListFormat format, string styleName,WListLevel listLevel)
        {
            ListOverrideStyle listOverrideStyle = null;
            if (format.LFOStyleName != null && format.LFOStyleName.Length > 0)
                listOverrideStyle = ListOverrides.FindByName(format.LFOStyleName);

            HybridDictionary lstStyle = ListNames[styleName] as HybridDictionary;
            int startAt = 0;
            if (lstStyle == null)
            {
                HybridDictionary startVal = new HybridDictionary();
                ListNames.Add(styleName, startVal);
                startAt = format.CurrentListStyle.Levels[listLevel.LevelNumber].StartAt;
                if (listOverrideStyle != null
                    && listOverrideStyle.OverrideLevels.HasOverrideLevel(listLevel.LevelNumber)
                    && listOverrideStyle.OverrideLevels[listLevel.LevelNumber].OverrideStartAtValue)
                    startAt = listOverrideStyle.OverrideLevels[listLevel.LevelNumber].StartAt;
                startVal.Add(listLevel.LevelNumber, startAt + 1);
                return startAt - 1;
            }
            else
            {
                if (lstStyle[listLevel.LevelNumber] != null)
                {
                    startAt = (int)lstStyle[listLevel.LevelNumber];
                    lstStyle[listLevel.LevelNumber] = startAt + 1;
                    return startAt - 1;
                }
                else
                {
                    startAt = format.CurrentListStyle.Levels[listLevel.LevelNumber].StartAt;
                    if (listOverrideStyle != null
                        && listOverrideStyle.OverrideLevels.HasOverrideLevel(listLevel.LevelNumber)
                        && listOverrideStyle.OverrideLevels[listLevel.LevelNumber].OverrideStartAtValue)
                        startAt = listOverrideStyle.OverrideLevels[listLevel.LevelNumber].StartAt;
                    lstStyle.Add(listLevel.LevelNumber, startAt + 1);
                    return startAt - 1;
                }
            }
        }
        /// <summary>
        /// Gets the list start value.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="styleName">Name of the style.</param>
        /// <returns></returns>
        private int GetListStartValue(WListFormat format, string styleName,WListLevel listLevel)
        {
            if (listLevel != null && listLevel.PatternType == ListPatternType.Bullet)
                return 1;

            if (!Lists.ContainsKey(styleName))
            {
                Dictionary<int, int> startVal = new Dictionary<int, int>();
                Lists.Add(styleName, startVal);
                if (format.CurrentListStyle != null)
                {
                    WListLevel level = format.CurrentListStyle.Levels[listLevel.LevelNumber];
                    for (int i = 0; i <= level.LevelNumber; i++)
                    {
                        startVal.Add(i, format.CurrentListStyle.Levels[i].StartAt + 1);
                    }
                    return level.StartAt;
                }
                return 1;
            }
            else
            {
                Dictionary<int, int> lstStyle = Lists[styleName];
                if (lstStyle.ContainsKey(listLevel.LevelNumber))
                {
                    int startAt = lstStyle[listLevel.LevelNumber];
                    lstStyle[listLevel.LevelNumber] = startAt + 1;
                    int levelno = listLevel.LevelNumber;
                    while (lstStyle.ContainsKey(levelno + 1))
                    {
                        lstStyle[levelno + 1] = 1;
                        levelno++;
                    }
                    return startAt;
                }
                else
                {
                    WListLevel level = format.CurrentListStyle.Levels[listLevel.LevelNumber];
                    lstStyle.Add(level.LevelNumber, level.StartAt + 1);
                    return level.StartAt;
                }
            }
        }
        /// <summary>
        /// Gets the list value.
        /// </summary>
        /// <param name="listFormat">The list format.</param>
        /// <param name="styleName">Name of the style.</param>
        /// <param name="level">The level.</param>
        /// <param name="startAt">The start at.</param>
        /// <param name="listItemIndex">Index of the list item.</param>
        /// <returns></returns>
        private string GetListValue(WListFormat listFormat, string styleName, WListLevel level, int startAt, int listItemIndex)
        {
            string listValue = string.Empty;
            int levelno = level.LevelNumber;
            if (Lists.ContainsKey(styleName))
            {
                string value = string.Empty;
                Dictionary<int, int> levels = Lists[styleName];
                int[] keys = new int[levels.Count];
                levels.Keys.CopyTo(keys, 0);
                keys = SortKeys(keys);
                int minKey = keys[0];
                for (int i = minKey; (minKey == levelno) ? i <= levelno : i < levelno; i++)
                {
                    if (levels.ContainsKey(i))
                    {
                        value += Convert.ToString(Convert.ToInt32(levels[i]) - 1) + ".";
                    }
                }
                if (level.PatternType == ListPatternType.LeadingZero && startAt < 10)
                    value += "0";
                value += startAt.ToString();
                listValue = value + level.NumberSufix;
            }
            return listValue;
        }
        /// <summary>
        /// Sort the Keys
        /// </summary>
        /// <param name="keys">keys</param>
        /// <returns> returns keys value</returns>
        private int[] SortKeys(int[] keys)
        {
            int temp;

            for (int i = 0; i < keys.Length - 1; i++)
            {
                for (int j = i + 1; j < keys.Length; j++)
                {
                    if (keys[i] > keys[j])
                    {
                        temp = keys[i];
                        keys[i] = keys[j];
                        keys[j] = temp;
                    }
                }
            }
            return keys;
        }
        #endregion
#endif

        #region Implementation / Replace single line
        /// <summary>
        /// Replaces all entries of given text with replace text in single-line mode.
        /// </summary>
        /// <param name="given">The given.</param>
        /// <param name="replace">The replace.</param>
        /// <param name="caseSensative">if it specifies case sensative replace, set to <c>true</c>.</param>
        /// <param name="wholeWord">if it specifies only whole word will be replaced, set to <c>true</c>.</param>
        /// <returns></returns>
        public int ReplaceSingleLine(string given, string replace, bool caseSensitive, bool wholeWord)
        {
            Regex pattern = FindUtils.StringToRegex(given, caseSensitive, wholeWord);

            return ReplaceSingleLine(pattern, replace);
        }
        /// <summary>
        /// Replaces all entries with specified pattern with replace text in single-line mode.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="replace">The replace.</param>
        /// <returns></returns>
        public int ReplaceSingleLine(Regex pattern, string replace)
        {
            TextBodyItem startItem = this.Sections[0].Body.Items[0];
            int changesMade = ReplaceSingleLine(pattern, replace, startItem);
            if (ReplaceFirst && changesMade > 0)
                return changesMade;

            changesMade += ReplaceHFSingleLine(pattern, replace);
            return changesMade;
        }
        /// <summary>
        /// Replaces the given text with replacement in single-line mode.
        /// </summary>
        /// <param name="given">The given.</param>
        /// <param name="replacement">The replacement.</param>
        /// <param name="caseSensitive">if it is case sensitive replace, set to <c>true</c>.</param>
        /// <param name="wholeWord">if it replaces only whole word, set to <c>true</c>.</param>
        /// <returns>The number of performed replaces.</returns>
        public int ReplaceSingleLine(string given, TextSelection replacement, bool caseSensitive, bool wholeWord)
        {
            Regex pattern = FindUtils.StringToRegex(given, caseSensitive, wholeWord);
            return ReplaceSingleLine(pattern, replacement);
        }
        /// <summary>
        /// Replaces the given pattern with replacement in single-line mode.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="replacement">The replacement.</param>
        /// <returns>The number of performed replaces.</returns>
        public int ReplaceSingleLine(Regex pattern, TextSelection replacement)
        {
            int changesMade = 0;

            TextBodyItem startItem = this.Sections[0].Body.Items[0];
            TextSelection[] selection = FindNextSingleLine(startItem, pattern);

            while (selection != null)
            {
                TextReplacer.Instance.ReplaceSingleLine(selection, replacement);
                changesMade += 1;

                if (ReplaceFirst)
                    break;

                selection = FindNextSingleLine(startItem, pattern);
            }

            return changesMade;
        }
        /// <summary>
        /// Replaces the given text with specified replacement in single-line mode.
        /// </summary>
        /// <param name="given">The given text.</param>
        /// <param name="replacement">The replacement.</param>
        /// <param name="caseSensitive">if it is case sensitive replace, set to <c>true</c>.</param>
        /// <param name="wholeWord">if it replace whole word, set to <c>true</c>.</param>
        /// <returns>The number of performed replaces.</returns>
        public int ReplaceSingleLine(string given, TextBodyPart replacement, bool caseSensitive, bool wholeWord)
        {
            Regex pattern = FindUtils.StringToRegex(given, caseSensitive, wholeWord);
            return ReplaceSingleLine(pattern, replacement);
        }
        /// <summary>
        /// Replaces the pattern with specified replacement in single-line mode.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="replacement">The replacement.</param>
        /// <returns>The number of performed replaces.</returns>
        public int ReplaceSingleLine(Regex pattern, TextBodyPart replacement)
        {
            int changesMade = 0;

            TextBodyItem startItem = this.Sections[0].Body.Items[0];
            TextSelection[] selection = FindNextSingleLine(startItem, pattern);

            while (selection != null)
            {
                TextReplacer.Instance.ReplaceSingleLine(selection, replacement);
                changesMade += 1;

                if (ReplaceFirst)
                    break;

                selection = FindNextSingleLine(startItem, pattern);
            }

            return changesMade;
        }
        /// <summary>
        /// Replaces the HF single line.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="replace">The replace.</param>
        /// <returns></returns>
        private int ReplaceHFSingleLine(Regex pattern, string replace)
        {
            ResetSingleLineReplace();
            int changesMade = 0;
            foreach (WSection section in Sections)
            {
                foreach (HeaderFooter hf in section.HeadersFooters)
                {
                    if (hf.Items.Count > 0)
                        changesMade += ReplaceSingleLine(pattern, replace, hf.Items[0]);
                }
            }
            ResetSingleLineReplace();

            return changesMade;
        }
        /// <summary>
        /// Resets the single line replace.
        /// </summary>
        private void ResetSingleLineReplace()
        {
            ResetFindNext();
            TextFinder.Instance.SingleLinePCol.Clear();
        }
        /// <summary>
        /// Replaces the single line.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="replace">The replace.</param>
        /// <param name="startItem">The start item.</param>
        /// <returns></returns>
        private int ReplaceSingleLine(Regex pattern, string replace, TextBodyItem startItem)
        {
            int changesMade = 0;
            TextSelection[] selection = FindNextSingleLine(startItem, pattern);

            while (selection != null)
            {
                TextReplacer.Instance.ReplaceSingleLine(selection, replace);
                changesMade += 1;

                if (ReplaceFirst)
                    break;

                selection = FindNextSingleLine(startItem, pattern);
            }

            return changesMade;
        }
        #endregion

        #region Implementation / FindNext
        /// <summary>
        /// Finds the next entry of given string, taking into consideration caseSensitive
        /// and wholeWord options.
        /// </summary>
        /// <param name="startTextBodyItem">The text body item at which search starts (paragraph or table).</param>
        /// <param name="given">The string to find.</param>
        /// <param name="caseSensitive">if it specifies case sensitive search, set to <c>true</c> .</param>
        /// <param name="wholeWord">if it search for the whole word, set to <c>true</c> .</param>
        /// <returns></returns>
        public TextSelection FindNext(TextBodyItem startTextBodyItem, string given, bool caseSensitive,
          bool wholeWord)
        {
            Regex pattern = FindUtils.StringToRegex(given, caseSensitive, wholeWord);
            return FindNext(startTextBodyItem, pattern);
        }
        /// <summary>
        /// Finds the next entry of given pattern.
        /// </summary>
        /// <param name="startBodyItem">The start body item at which search starts (paragraph or table).</param>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        public TextSelection FindNext(TextBodyItem startBodyItem, Regex pattern)
        {
            if (startBodyItem == null)
            {
                throw new ArgumentException("Start body item can't be null", "startBodyItem");
            }

            if (m_prevBodyItem == null)
            {
                m_prevBodyItem = startBodyItem;
            }
            else if (m_prevBodyItem != startBodyItem)
            {
                m_nextParaItem = null;
                m_prevBodyItem = startBodyItem;
            }

            TextSelection selection = null;
            if (m_nextParaItem != null && m_nextParaItem.OwnerParagraph != null)
            {
                selection = FindNext(pattern);
                if (selection != null)
                {
                    selection.GetAsOneRange();
                    UpdateNextItem(selection);
                    return selection;
                }
                else
                {
                    startBodyItem = m_nextParaItem.OwnerParagraph.NextTextBodyItem;
                    if (startBodyItem == null)
                    {
                        m_nextParaItem = null;
                        return null;
                    }
                }
            }

            TextBodyItem tbItem = startBodyItem;
            do
            {
                selection = tbItem.Find(pattern);
                if (CheckSelection(selection))
                {
                    selection.GetAsOneRange();
                    UpdateNextItem(selection);
                    return selection;
                }
                tbItem = tbItem.NextTextBodyItem;
            }
            while (tbItem != null);

            return null;
        }
        /// <summary>
        /// Finds the next selection.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        private TextSelection FindNext(Regex pattern)
        {
            WParagraph para = m_nextParaItem.OwnerParagraph;
            TextSelectionList selList = para.FindAll(pattern);
            if (selList.Count > 0)
            {
                int itemIndex = m_nextParaItem.GetIndexInOwnerCollection();
                int startIndex = 0;
                int endIndex = 0;
                foreach (TextSelection textSel in selList)
                {
                    if (!CheckSelection(textSel))
                        continue;

                    startIndex = textSel.StartTextRange.GetIndexInOwnerCollection();

                    endIndex = textSel.EndTextRange.GetIndexInOwnerCollection();
                    if (itemIndex > endIndex)
                        continue;
                    else return textSel;
                }
            }

            return null;
        }
        /// <summary>
        /// Updates the next item at which next find will start.
        /// </summary>
        /// <param name="selection">The selection.</param>
        private void UpdateNextItem(TextSelection selection)
        {
            m_nextParaItem = null;
            WTextRange[] textRanges = selection.GetRanges();
            if (textRanges != null)
            {
                WTextRange lastRange = textRanges[textRanges.Length - 1];
                if (lastRange.NextSibling != null)
                {
                    m_nextParaItem = lastRange.NextSibling as ParagraphItem;
                    return;
                }
            }

            TextBodyItem tbItem = selection.OwnerParagraph as TextBodyItem;
            while (tbItem.NextTextBodyItem != null)
            {
                tbItem = tbItem.NextTextBodyItem;
                m_nextParaItem = GetNextItem(tbItem);
                if (m_nextParaItem != null)
                    break;
            }
        }
        /// <summary>
        /// Gets the next paragraph item in textbody item.
        /// </summary>
        /// <param name="tbItem">The textbody item.</param>
        /// <returns></returns>
        private ParagraphItem GetNextItem(TextBodyItem tbItem)
        {
            if (tbItem == null)
                return null;

            if (tbItem is WTable)
            {
                ParagraphItem item = null;
                WTable table = tbItem as WTable;
                foreach (WTableRow row in table.Rows)
                {
                    foreach (WTableCell cell in row.Cells)
                    {
                        item = GetNextItem(cell as WTextBody);
                        if (item != null)
                            return item;
                    }
                }
            }
            else
            {
                if ((tbItem as WParagraph).Items.Count > 0)
                {
                    return (tbItem as WParagraph).Items[0];
                }
                else if (tbItem.NextSibling != null)
                {
                    return GetNextItem(tbItem.NextSibling as TextBodyItem);
                }
            }

            return null;
        }
        /// <summary>
        /// Gets the next paragraph item in text body.
        /// </summary>
        /// <param name="textBody">The text body.</param>
        /// <returns></returns>
        private ParagraphItem GetNextItem(WTextBody textBody)
        {
            ParagraphItem pItem = null;
            foreach (TextBodyItem item in textBody.Items)
            {
                pItem = GetNextItem(item);
                if (item != null)
                {
                    return pItem;
                }
            }

            return null;
        }
        /// <summary>
        /// Checks the selection.
        /// </summary>
        /// <param name="textSel">The text selection.</param>
        private bool CheckSelection(TextSelection textSel)
        {
            if (textSel != null && textSel.Count > 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Finds the next given text starting from specified
        /// TextBodyItem using single-line mode.
        /// </summary>
        /// <param name="startTextBodyItem">The start text body item.</param>
        /// <param name="given">The given.</param>
        /// <param name="caseSensitive">if it is case sensitive search, set to <c>true</c>.</param>
        /// <param name="wholeWord">if it search for whole word, set to <c>true</c> .</param>
        /// <returns></returns>
        public TextSelection[] FindNextSingleLine(TextBodyItem startTextBodyItem, string given, bool caseSensitive,
          bool wholeWord)
        {
            Regex pattern = FindUtils.StringToRegex(given, caseSensitive, wholeWord);
            return FindNextSingleLine(startTextBodyItem, pattern);
        }
        /// <summary>
        /// Finds the next text which fit the specified pattern starting from start TextBodyItem
        /// using single-line mode.
        /// </summary>
        /// <param name="startBodyItem">The start body item.</param>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        public TextSelection[] FindNextSingleLine(TextBodyItem startBodyItem, Regex pattern)
        {
            if (startBodyItem == null)
            {
                throw new ArgumentException("Start body item can't be null", "startBodyItem");
            }

            if (m_prevBodyItem == null)
            {
                m_prevBodyItem = startBodyItem;
            }
            else if (m_prevBodyItem != startBodyItem)
            {
                m_nextParaItem = null;
                m_prevBodyItem = startBodyItem;
            }

            TextSelection[] selection = null;
            if (m_nextParaItem == null)
            {
                // Get first paragraphItem
                m_nextParaItem = GetNextItem(startBodyItem);
            }

            selection = FindNextSingleLine(pattern);
            if (selection != null)
            {
                TextSelection sel = selection[selection.Length - 1];
                sel.GetAsOneRange();
                UpdateNextItem(sel);
                return selection;
            }
            else
            {
                m_nextParaItem = null;
                return null;
            }
        }
        /// <summary>
        /// Finds the next text which fit the specified pattern using single-line mode.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns></returns>
        private TextSelection[] FindNextSingleLine(Regex pattern)
        {
            if (m_nextParaItem == null)
                return null;

            WParagraph ownerPara = m_nextParaItem.OwnerParagraph;
            int pIndex = ownerPara.GetIndexInOwnerCollection();
            // If this is first paragraph in text body, clear the collection of textbody paragraphs
            if (pIndex == 0)
                TextFinder.Instance.SingleLinePCol.Clear();

            int itemIndex = m_nextParaItem.GetIndexInOwnerCollection();
            TextSelection[] selection = TextFinder.Instance.FindInItems(ownerPara, pattern, itemIndex,
              ownerPara.Items.Count - 1);

            if (selection == null)
            {
                WTextBody tb = ownerPara.OwnerTextBody;
                if (tb != null)
                    selection = TextFinder.Instance.FindSingleLine(tb, pattern, pIndex + 1, tb.Items.Count - 1);

                if (selection == null)
                {
                    TextBodyItem lastTbItem = tb.Items[tb.Items.Count - 1];
                    TextBodyItem nextTbItem = lastTbItem.NextTextBodyItem;

                    while (nextTbItem != null)
                    {
                        if (nextTbItem.GetIndexInOwnerCollection() == 0)
                            TextFinder.Instance.SingleLinePCol.Clear();

                        m_nextParaItem = GetNextItem(nextTbItem);
                        if (m_nextParaItem == null)
                            nextTbItem = nextTbItem.NextTextBodyItem;
                        else
                            break;
                    }

                    if (nextTbItem != null)
                    {
                        selection = FindNextSingleLine(pattern);
                    }
                }
            }

            return selection;
        }
        /// <summary>
        /// Resets the FindNext.
        /// </summary>
        public void ResetFindNext()
        {
            m_nextParaItem = null;
            m_prevBodyItem = null;
        }
        #endregion

        #region Class factory methods / overrides
        /// <summary>
        /// Creates new paragraph item instance.
        /// </summary>
        /// <param name="itemType">Paragraph item type</param>
        /// <returns></returns>
        public ParagraphItem CreateParagraphItem(ParagraphItemType itemType)
        {
            switch (itemType)
            {
                case ParagraphItemType.Break:
                    return new Break(this);
                case ParagraphItemType.TextRange:
                    return new WTextRange(this);
                case ParagraphItemType.Picture:
                    return new WPicture(this);
                case ParagraphItemType.BookmarkStart:
                    return new BookmarkStart(this);
                case ParagraphItemType.BookmarkEnd:
                    return new BookmarkEnd(this);
                case ParagraphItemType.Field:
                    return new WField(this);
                case ParagraphItemType.TextBox:
                    return new WTextBox(this);
                case ParagraphItemType.MergeField:
                    return new WMergeField(this);
                case ParagraphItemType.EmbedField:
                    return new WEmbedField(this);
                case ParagraphItemType.Symbol:
                    return new WSymbol(this);
                case ParagraphItemType.FieldMark:
                    return new WFieldMark(this);
                case ParagraphItemType.CheckBox:
                    return new WCheckBox(this);
                case ParagraphItemType.TextFormField:
                    return new WTextFormField(this);
                case ParagraphItemType.DropDownFormField:
                    return new WDropDownFormField(this);
                case ParagraphItemType.Comment:
                    return new WComment(this);
                case ParagraphItemType.Footnote:
                    return new WFootnote(this);
                case ParagraphItemType.ShapeObject:
                    return new ShapeObject(this);
                case ParagraphItemType.InlineShapeObject:
                    return new InlineShapeObject(this);
                case ParagraphItemType.TOC:
                    return new TableOfContent(this);
                case ParagraphItemType.OleObject:
                    return new WOleObject(this);
            }

            throw new ArgumentException("Ivalid type of paragraph item");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override object CloneImpl()
        {
            lock (m_threadLocker)
            {
                return new WordDocument(this);
            }
        }
        #endregion

        #region Class factory methods / styles
        /// <summary>
        /// Implementation of character format creating
        /// </summary>
        /// <returns>Character format object.</returns>
        protected internal WCharacterFormat CreateCharacterFormatImpl()
        {
            return new WCharacterFormat(this);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected internal ListStyle CreateListStyleImpl()
        {
            return new ListStyle(this);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected internal WListLevel CreateListLevelImpl(ListStyle style)
        {
            return new WListLevel(style);
        }
        /// <summary>
        /// Implementation of paragraph format creating
        /// </summary>
        /// <returns>Paragraph format object.</returns>
        protected internal WParagraphFormat CreateParagraphFormatImpl()
        {
            return new WParagraphFormat(this);
        }
        /// <summary>
        /// Implementation of table format creating
        /// </summary>
        /// <returns>Table format instance</returns>
        protected internal RowFormat CreateTableFormatImpl()
        {
            return new RowFormat();
        }
        /// <summary>
        /// Implementation of cell format creating
        /// </summary>
        /// <returns></returns>
        protected internal CellFormat CreateCellFormatImpl()
        {
            return new CellFormat();
        }
        /// <summary>
        /// Implementation of textbox format creating
        /// </summary>
        /// <returns>textbox format object.</returns>
        protected internal WTextBoxFormat CreateTextboxFormatImpl()
        {
            return new WTextBoxFormat(this);
        }
        /// <summary>
        /// Text
        /// </summary>
        /// <returns></returns>
        protected internal WTextBoxCollection CreateTextBoxCollectionImpl()
        {
            return new WTextBoxCollection(this);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        protected internal WListFormat CreateListFormatImpl(IWParagraph owner)
        {
            return new WListFormat(owner);
        }
        #endregion

        #region Implementation Encryption/Decryption
        /// <summary>
        /// Creates the compound file.
        /// </summary>
        /// <returns></returns>
        internal ICompoundFile CreateCompoundFile()
        {
            ICompoundFile file = new Syncfusion.CompoundFile.DocIO.Net.CompoundFile();
            return file;
        }
        /// <summary>
        /// Creates the compound file.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        internal ICompoundFile CreateCompoundFile(Stream stream)
        {
            ICompoundFile file = new Syncfusion.CompoundFile.DocIO.Net.CompoundFile(stream);
            return file;
        }
        /// <summary>
        /// Checks for encryption.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        internal bool CheckForEncryption(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            bool bEncrypted = false;

            if (CompoundFile.DocIO.Net.CompoundFile.CheckHeader(stream))
            {
                // The document is encrypted and decryption is required.
                bEncrypted = true;
            }
            return bEncrypted;
        }
        #endregion

        #region Internal methods
        /// <summary>
        /// Ensures the paragraph style.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        internal void EnsureParagraphStyle(IWParagraph paragraph)
        {
            if (paragraph.StyleName == null)
            {
                if (Styles.FindByName(DEF_NORMAL_STYLE) == null)
                {
                    AddStyle(StyleType.ParagraphStyle, DEF_NORMAL_STYLE);
                }

                paragraph.ApplyStyle(DEF_NORMAL_STYLE);
            }
        }
        /// <summary>
        /// Clones the shape escher.
        /// </summary>
        /// <param name="destDoc">The destination doc.</param>
        /// <param name="shapeItem">The shape item.</param>
        internal void CloneShapeEscher(WordDocument destDoc, IParagraphItem shapeItem)
        {
            if (this.Escher != null)
            {
                if (destDoc.m_escher == null)
                {
                    destDoc.m_escher = new EscherClass(destDoc);
                }
                if (shapeItem != null)
                {
                    if (shapeItem is IWPicture)
                    {
                        ClonePictureContainer(destDoc, shapeItem as WPicture);
                    }
                    else if (shapeItem is IWTextBox)
                    {
                        CloneTextBoxContainer(destDoc, shapeItem as WTextBox);
                    }
                    else if (shapeItem is ShapeObject)
                    {
                        CloneAutoShapeContainer(destDoc, shapeItem as ShapeObject);
                    }

                    m_defShapeId += 1;
                }
            }
        }
        /// <summary>
        /// Gets the password.
        /// </summary>
        /// <returns></returns>
        internal string GetPassword()
        {
            return m_password;
        }
        /// <summary>
        /// Inserts the watermark.
        /// </summary>
        /// <param name="type">The type.</param>
        internal void InsertWatermark(WatermarkType type)
        {
            ResetWatermark();
            if (type != WatermarkType.NoWatermark && m_escher == null)
            {
                m_escher = new EscherClass(this);
            }
            if (type == WatermarkType.PictureWatermark)
            {
                m_watermark = new PictureWatermark(this);
            }
            else if (type == WatermarkType.TextWatermark)
            {
                m_watermark = new TextWatermark(this);
            }
            else
            {
                m_watermark = new Watermark(this, type);
            }
        }
        /// <summary>
        /// Reads document's background fill effects.
        /// </summary>
        internal void ReadBackground()
        {
            if (m_escher != null)
            {
                m_background = new Background(this);
            }
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Saves the document to the detailed XML stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal void ToDetailedDlsStream(MemoryStream stream)
        {
            SaveXml(stream);

        }
#endif
        /// <summary>
        /// Determines whether list has style
        /// </summary>
        internal bool HasListStyle()
        {
            return (m_listStyles.Count > 0) ? true : false;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Inits the default paragraph format.
        /// </summary>
        private void InitDefaultParagraphFormat()
        {
            m_defParaFormat = new WParagraphFormat(this);
            //Imports the paragraph properties of default normal style.
            WParagraphStyle style = Styles.FindByName("Normal", StyleType.ParagraphStyle) as WParagraphStyle;
            if (style != null)
            {
                m_defParaFormat.ImportContainer(style.ParagraphFormat);
                m_defParaFormat.CopyProperties(style.ParagraphFormat);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void Init()
        {
            IsNormalStyleDefined = false;
            m_mailMerge = new MailMerge(this);
            m_viewSetup = new ViewSetup(this);
            m_dop = new DOPDescriptor();

            m_sections = new WSectionCollection(this);
            m_styles = new StyleCollection(this);
            m_listStyles = new ListStyleCollection(this);
            m_listOverrides = new ListOverrideStyleCollection(this);
            m_txbxItems = new TextBoxCollection(this);

            m_watermark = new Watermark(this, WatermarkType.NoWatermark);
            m_background = new Background(this, BackgroundType.NoBackground);
            m_builtinProp = new BuiltinDocumentProperties(this);            
            m_customProp = new CustomDocumentProperties();
            if (m_styleNameIds != null)
                m_styleNameIds.Clear();
            if (m_settings!= null)
                m_settings.CompatibilityOptions.PropertiesHash.Clear();
            if (m_usedFonts != null)
                m_usedFonts.Clear();
            if (m_fontSubstitutionTable != null)
                m_fontSubstitutionTable.Clear();
            if (m_objPoolContainers != null)
                m_objPoolContainers.Clear();
            if (m_docxProps != null)
                m_docxProps.Clear();
            if (m_Comments != null)
                m_Comments.Clear();
            if (m_fields != null)
                m_fields.Clear();
            if (m_bookmarks != null)
                m_bookmarks.Clear();
#if !SILVERLIGHT && !WP
            m_htmlValidationOption = XHTMLValidationType.Transitional;
#endif
            m_throwUnsupportedExceptions = false;
            m_hasPicture = false;
            m_isWriteProtected = false;
            m_updateFields = false;
            m_isEncrypted = false;
            m_bReplaceFirst = false;
            m_isReadOnly = false;
            m_importStyles = true;
            m_defShapeId = 1;
            m_tableOfContent = null;
            m_latentStyles2010 = null;
            m_latentStyles = null;
            m_standardBidiFont = null;
            m_standardNonFarEastFont = null;
            m_standardFarEastFont = null;
            m_standardAsciiFont = null;
            m_macroCommands = null;
            m_macrosData = null;
            m_password = null;
            m_assocStrings = null;
            m_saveOptions = null;
            m_prevBodyItem = null;
            m_nextParaItem = null;
            m_props = null;
            m_variables = null;
            m_docxPackage = null;
            m_grammarSpellingData = null;
            if (m_imageCollection != null)
            {
                m_imageCollection.Clear();
                m_imageCollection = null;
            }
            if (m_defParaFormat != null)
            {
                m_defParaFormat.Close();
                m_defParaFormat = null;
            }
            if (m_defCharFormat != null)
            {
                m_defCharFormat.Close();
                m_defCharFormat = null;
            }
            if (m_escher != null)
            {
                m_escher.Close();
                m_escher = null;
            }
            RemoveMacros();
            if (ClonedFields != null)
                ClonedFields.Clear();
            UpdatedFields.Clear();
        }
        /// <summary>
        /// Creates the default list styles.
        /// </summary>
        private void CreateDefListStyles()
        {
            //Create Numbered Style
            ListStyle numStyle = new ListStyle(this, ListType.Numbered);
            numStyle.Name = DEF_NUMBERING_STYLE;
            numStyle.ListType = ListType.Numbered;
            m_listStyles.Add(numStyle);

            //Create Bulleted Style
            ListStyle bulletStyle = new ListStyle(this, ListType.Bulleted);
            bulletStyle.Name = DEF_BULLETS_STYLE;
            bulletStyle.ListType = ListType.Bulleted;
            m_listStyles.Add(bulletStyle);
        }
        /// <summary>
        /// Copies the binary data.
        /// </summary>
        /// <param name="srcData">The SRC data.</param>
        /// <param name="destData">The destination data.</param>
        private void CopyBinaryData(byte[] srcData, ref byte[] destData)
        {
            if (srcData != null)
            {
                destData = new byte[srcData.Length];
                srcData.CopyTo(destData, 0);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="destDoc"></param>
        /// <param name="picture"></param>
        private void ClonePictureContainer(WordDocument destDoc, WPicture picture)
        {
            int spid = (int)picture.ShapeId;

            if (!CheckContainer(EscherShapeType.msosptPictureFrame, spid))
                return;

            WordSubdocument docType = (picture.IsHeaderPicture) ? WordSubdocument.HeaderFooter : WordSubdocument.Main;
            m_defShapeId = this.Escher.CloneContainerBySpid(destDoc, docType, spid, m_defShapeId);

            if (m_defShapeId != -1)
                picture.ShapeId = m_defShapeId;

        }
        /// <summary>
        /// Clones the textbox container.
        /// </summary>
        /// <param name="destDoc">The destination doc.</param>
        /// <param name="textBox">The textbox.</param>
        private void CloneTextBoxContainer(WordDocument destDoc, WTextBox textBox)
        {
            int spid = (int)textBox.TextBoxFormat.TextBoxShapeID;
            if (!CheckContainer(EscherShapeType.msosptTextBox, spid))
                return;

            WordSubdocument docType = (textBox.TextBoxFormat.IsHeaderTextBox) ? WordSubdocument.HeaderFooter : WordSubdocument.Main;
            m_defShapeId = this.Escher.CloneContainerBySpid(destDoc, docType, spid, m_defShapeId);

            if (m_defShapeId != -1)
                textBox.TextBoxFormat.TextBoxShapeID = m_defShapeId;
        }
        /// <summary>
        /// Clones the auto shape container.
        /// </summary>
        /// <param name="destDoc">The destination doc.</param>
        /// <param name="shapeObj">The shape obj.</param>
        private void CloneAutoShapeContainer(WordDocument destDoc, ShapeObject shapeObj)
        {
            int spid = shapeObj.FSPA.Spid;
            WordSubdocument docType = (shapeObj.IsHeaderAutoShape) ? WordSubdocument.HeaderFooter : WordSubdocument.Main;
            m_defShapeId = this.Escher.CloneContainerBySpid(destDoc, docType, spid, m_defShapeId);

            if (m_defShapeId != -1)
                shapeObj.FSPA.Spid = m_defShapeId;
        }
        /// <summary>
        /// Checks the container.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="spid">The spid.</param>
        /// <returns></returns>
        private bool CheckContainer(EscherShapeType type, int spid)
        {
            if (!Escher.Containers.ContainsKey(spid) ||
                ((Escher.Containers[spid] as MsofbtSpContainer) != null
                && (Escher.Containers[spid] as MsofbtSpContainer).Shape.ShapeType != type))
            {
                m_defShapeId = -1;
                return false;
            }

            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
#if SILVERLIGHT || WP
        private byte[] GetBackGndImage()
#else
        private Image GetBackGndImage()
#endif
        {
            if (m_background.Type == BackgroundType.Picture)
            {
                return m_background.Picture;
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
#if SILVERLIGHT || WP
		private void SetBackgroundImage(byte[] imageBytes)
        {
            m_background.Picture = imageBytes;
#else
		private void SetBackgroundImage(Image image)
        {
			m_background.Picture = image;
#endif
            m_background.Type = BackgroundType.Picture;
        }
        /// <summary>
        /// Determines whether document has tracked changes.
        /// </summary>
        /// <returns>
        /// 	It it has tracked changes, set to <c>true</c>.
        /// </returns>
        private bool HasTrackedChanges()
        {
            if (m_sections == null || m_sections.Count == 0)
            {
                return false;
            }

            foreach (WSection section in m_sections)
            {
                if (section.HasTrackedChanges())
                    return true;
            }

            return false;
        }
        /// <summary>
        /// Checks whether security permission can be granted.
        /// </summary>
        internal bool IsSecurityGranted()
        {
#if !SILVERLIGHT && !WP
            SecurityPermission perm = new SecurityPermission(PermissionState.Unrestricted);
            bool bResult = false;

            try
            {
                perm.Demand();
                bResult = true;
            }
            catch (System.Security.SecurityException)
            {
            }

            return bResult;
#else
			return false;
#endif
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Checks the extension with respect to formatType
        /// </summary>
        /// <param name="fileName">The filename</param>
        /// <param name="formatType">The FormatType</param>
        /// <returns></returns>
        private string CheckExtension(string fileName, FormatType formatType)
        {
            FileInfo info = new FileInfo(fileName);
            if (!info.Exists && (formatType != FormatType.Html))
            {
                string extension = info.Extension;
                if (extension != formatType.ToString())
                {
                    if (extension != string.Empty)
                    {
                        int startIndex = fileName.LastIndexOf(extension);
                        fileName = fileName.Remove(startIndex);
                    }
                    fileName = fileName + "." + formatType.ToString();
                }
            }
            return fileName;
        }
#endif
        /// <summary>
        /// Checks the name of the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private void CheckFileName(string fileName)
        {
            bool nameTooLong = false;
            if (fileName.Length >= 260)
                nameTooLong = true;
            else
            {
                string directoryName = Path.GetDirectoryName(fileName);
                if (directoryName.Length >= 248)
                    nameTooLong = true;
            }

            if (nameTooLong)
#if WINRT
                 throw new Exception("The file name is too long. The fully qualified file name must be less " +
                  "than 260 characters and the directory name must be less than 248 characters");
#else
                throw new PathTooLongException("The file name is too long. The fully qualified file name must be less " +
                  "than 260 characters and the directory name must be less than 248 characters");
#endif
        }
        /// <summary>
        /// Checks whether the evaluation expired.
        /// </summary>
        internal void CheckEvalExpired()
        {
            if (WriteWarning)
            {
                try
                {
                    WParagraph para = new WParagraph(this);
                    IWTextRange text = para.AppendText("This document was created with evaluation version of Syncfusion Essential DocIO");
                    text.CharacterFormat.TextColor = Color.Red;
                    text.CharacterFormat.FontSize = 12f;
                    m_sections[0].Body.ChildEntities.Insert(0, para);
                    m_isWarnInserted = true;
                }
                catch
                {
                }
            }
        }
        /// <summary>
        /// Resets the watermark.
        /// </summary>
        private void ResetWatermark()
        {
            foreach (WSection sec in this.Sections)
            {
                sec.HeadersFooters.EvenHeader.WriteWatermark = false;
                sec.HeadersFooters.OddHeader.WriteWatermark = false;
                sec.HeadersFooters.FirstPageHeader.WriteWatermark = false;
            }
            if (m_watermark is PictureWatermark
                && (m_watermark as PictureWatermark).WordPicture != null
                && (m_watermark as PictureWatermark).WordPicture.Document != null
                && (m_watermark as PictureWatermark).WordPicture.ImageRecord != null)
                (m_watermark as PictureWatermark).WordPicture.ImageRecord.OccurenceCount--;
        }
        /// <summary>
        /// update the write watermark.
        /// </summary>
        private void UpdateWriteWatermark()
        {
            foreach (WSection sec in this.Sections)
            {
                sec.HeadersFooters.EvenHeader.WriteWatermark = true;
                sec.HeadersFooters.OddHeader.WriteWatermark = true;
                sec.HeadersFooters.FirstPageHeader.WriteWatermark = true;
            }
        }
        /// <summary>
        /// Sets the protection.
        /// </summary>
        private void SetProtection(ProtectionType type)
        {
            if (type == ProtectionType.AllowOnlyFormFields)
            {
                foreach (WSection section in this.Sections)
                {
                    section.ProtectForm = true;
                }
            }
        }
        #endregion

        #region Implementation / xml
//#if !SILVERLIGHT
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Create default liststyles and add them to ListStyleCollection 
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        private void WriteXml(XmlWriter writer)
        {
            XDLSWriter xdlsWriter = new XDLSWriter(writer);
            xdlsWriter.Serialize(this);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        [Syncfusion.Documentation.DocumentationExclude()]
        private void ReadXml(XmlReader reader)
        {
            XDLSReader dlsXmlReader = new XDLSReader(reader);
            dlsXmlReader.Deserialize(this);
        }
        /// <summary>
        /// Gets xml schema
        /// </summary>
        /// <returns></returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return GetSchema();
        }
        /// <summary>
        /// Reads document using XmlReader
        /// </summary>
        /// <param name="reader"></param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            ReadXml(reader);
        }
        /// <summary>
        /// Writes document using XmlWriter
        /// </summary>
        /// <param name="writer"></param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            WriteXml(writer);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected XmlSchema GetSchema()
        {
            return DocIOXsdGenerator.GetDocIOLocalSchema();
        }
#endif
        /// <summary>
        /// 
        /// </summary>
        protected override void InitXDLSHolder()
        {
            XDLSHolder.AddElement(XDLSConstants.StylesTag, Styles);
            XDLSHolder.AddElement(XDLSConstants.ListStylesTag, m_listStyles);
            XDLSHolder.AddElement(XDLSConstants.SectionsTag, Sections);
            XDLSHolder.AddElement(XDLSConstants.ViewSetupTag, ViewSetup);
//#if !SILVERLIGHT
            XDLSHolder.AddElement(XDLSConstants.BuiltinPropertiesTag, BuiltinDocumentProperties);
            XDLSHolder.AddElement(XDLSConstants.CustomPropertiesTag, CustomDocumentProperties);
//#endif
            XDLSHolder.AddElement(XDLSConstants.ListOverridesTag, ListOverrides);
            XDLSHolder.AddElement(XDLSConstants.BackgroundTag, Background);
            XDLSHolder.AddElement(XDLSConstants.WatermarkTag, Watermark);
            //XDLSHolder.AddElement("FontFamilyNames", m_fontNameCol);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlContent(IXDLSContentWriter writer)
        {
            base.WriteXmlContent(writer);
            if (MacrosData != null)
            {
                writer.WriteChildBinaryElement(XDLSConstants.MacrosTag, MacrosData);
            }
            if (MacroCommands != null)
            {
                writer.WriteChildBinaryElement(XDLSConstants.MacroCommandsTag, MacroCommands);
            }
            if (m_escher != null)
            {
                //m_escher.RemoveEscherOle();
                MemoryStream escherDataStream = new MemoryStream();
                m_escher.WriteContainersData(escherDataStream);
                m_escherDataContainers = escherDataStream.ToArray();
                writer.WriteChildBinaryElement(XDLSConstants.EscherDataConatinersTag, m_escherDataContainers);
#if WINRT
                escherDataStream.Dispose();
#else
                escherDataStream.Close();
#endif

                MemoryStream escherStream = new MemoryStream();
                m_escher.WriteContainers(escherStream);
                m_escherContainers = escherStream.ToArray();
                writer.WriteChildBinaryElement(XDLSConstants.EscherContainersTag, m_escherContainers);
#if WINRT
                escherStream.Dispose();
#else
                escherStream.Close();
#endif

                escherDataStream = null;
                escherStream = null;
                m_escherDataContainers = null;
                m_escherContainers = null;
            }
            if (m_dop != null)
            {
                MemoryStream dopStream = new MemoryStream();
                m_dop.Write(dopStream);
                byte[] dopData = dopStream.ToArray();
                writer.WriteChildBinaryElement(XDLSConstants.DOPInternalData, dopData);
            }
            if (m_grammarSpellingData != null && GrammarSpellingData.PlcfgramData != null &&
              GrammarSpellingData.PlcfsplData != null)
            {
                writer.WriteChildBinaryElement(XDLSConstants.GrammarDataTag, GrammarSpellingData.PlcfgramData);
                writer.WriteChildBinaryElement(XDLSConstants.SpellingDataTag, GrammarSpellingData.PlcfsplData);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected override bool ReadXmlContent(IXDLSContentReader reader)
        {
            if (reader.TagName == XDLSConstants.MacrosTag)
            {
                MacrosData = reader.ReadChildBinaryElement();
            }
            if (reader.TagName == XDLSConstants.MacroCommandsTag)
            {
                MacroCommands = reader.ReadChildBinaryElement();
            }

            //Read escher containers
            if (reader.TagName == XDLSConstants.EscherContainersTag)
            {
                m_escherContainers = reader.ReadChildBinaryElement();
            }

            //Read escher data containers
            if (reader.TagName == XDLSConstants.EscherDataConatinersTag)
            {
                m_escherDataContainers = reader.ReadChildBinaryElement();

            }

            if (m_escherDataContainers != null && m_escherContainers != null)
            {
                MemoryStream escherDataStream = new MemoryStream(m_escherDataContainers, 0, m_escherDataContainers.Length);
                MemoryStream escherStream = new MemoryStream(m_escherContainers, 0, m_escherContainers.Length);
                m_escher = new EscherClass(escherStream, escherDataStream, 0, (int)escherStream.Length, this);
#if WINRT
                escherDataStream.Dispose();
                escherStream.Dispose();
#else
                escherDataStream.Close();
                escherStream.Close();
#endif
                escherDataStream = null;
                escherStream = null;
                m_escherDataContainers = null;
                m_escherContainers = null;
            }

            if (reader.TagName == XDLSConstants.DOPInternalData)
            {
                byte[] dopData = reader.ReadChildBinaryElement();
                MemoryStream dopStream = new MemoryStream(dopData);
                m_dop = new DOPDescriptor(dopStream, 0, (int)dopStream.Length, false);
#if WINRT
                dopStream.Dispose();
#else
                dopStream.Close();
#endif
                dopStream = null;
                dopData = null;
            }

            // Read grammar and spelling checking data
            if (m_grammarSpellingData == null)
            {
                m_grammarSpellingData = new GrammarSpelling();
            }

            if (reader.TagName == XDLSConstants.GrammarDataTag)
            {
                m_grammarSpellingData.PlcfgramData = reader.ReadChildBinaryElement();
            }

            if (reader.TagName == XDLSConstants.SpellingDataTag)
            {
                m_grammarSpellingData.PlcfsplData = reader.ReadChildBinaryElement();
            }

            return base.ReadXmlContent(reader);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);

            if (m_standardAsciiFont != null)
            {
                writer.WriteValue(XDLSConstants.StandardAsciiFont, m_standardAsciiFont);
            }
            if (m_standardFarEastFont != null)
            {
                writer.WriteValue(XDLSConstants.StandardFarEastFont, m_standardFarEastFont);
            }
            if (m_standardNonFarEastFont != null)
            {
                writer.WriteValue(XDLSConstants.StandardNonFarEastFont, m_standardNonFarEastFont);
            }
            if (m_watermark.Type != WatermarkType.NoWatermark)
            {
                writer.WriteValue(XDLSConstants.WatermarkTypeAttr, m_watermark.Type);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            if (reader.HasAttribute(XDLSConstants.StandardAsciiFont))
            {
                m_standardAsciiFont = reader.ReadString(XDLSConstants.StandardAsciiFont);
            }
            if (reader.HasAttribute(XDLSConstants.StandardFarEastFont))
            {
                m_standardFarEastFont = reader.ReadString(XDLSConstants.StandardFarEastFont);
            }
            if (reader.HasAttribute(XDLSConstants.StandardNonFarEastFont))
            {
                m_standardNonFarEastFont = reader.ReadString(XDLSConstants.StandardNonFarEastFont);
            }
            if (reader.HasAttribute(XDLSConstants.WatermarkTypeAttr))
            {
                WatermarkType watermarkType = (WatermarkType)reader.ReadEnum(XDLSConstants.WatermarkTypeAttr, typeof(WatermarkType));
                this.InsertWatermark(watermarkType);
            }
        }
//#endif
        #endregion

        #region Implementation / layout
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Creates and initializes layout data
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void CreateLayoutInfo()
        {
            m_layoutInfo = new LayoutInfo(ChildrenLayoutDirection.Vertical);
        }
#endif
        /// <summary>
        /// Gets sub widgets
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override IEntityCollectionBase WidgetCollection
        {
            get
            {
                return Sections;
            }
        }
        #endregion

        #region Implementation / Licensing
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Checks license.
        /// </summary>
        private void CheckLicense()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
#if AllowUnsafeCode
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(DocIOConfig));
                //new Syncfusion.Core.Licensing.LicensedComponent( typeof( DocIOConfig ), out m_isEvalExpired );
#else
            new Syncfusion.Core.Licensing.LicensedWebComponent(typeof(DocIOConfig));
#endif
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }
        }
#endif
        #endregion

        #region Implementation / Macros
        /// <summary>
        /// Removes the macros in the document.
        /// </summary>
        public void RemoveMacros()
        {
            if (VbaProject != null)
            {
#if WINRT
                VbaProject.Dispose();
#else
                VbaProject.Close();
#endif
                VbaProject = null;
            }
            VbaData.Clear();
            DocEvents.Clear();
        }
        #endregion
    }

    /// <summary>
    /// Class represents document properties
    /// </summary>
    public class DocProperties
    {
        #region Field
        /// <summary>
        /// 
        /// </summary>
        private DOPDescriptor m_dop;
        private DocumentVersion m_version;
        private Hyphenation m_hyphenation;
        #endregion

        #region Properties
        /// <summary>
        /// Specifies whether to apply shading on form fields.
        /// </summary>
        /// <value>if form field shading is applied, set to <c>true</c>.</value>
        public bool FormFieldShading
        {
            get
            {
                return m_dop.FormFieldShading;
            }
            set
            {
                m_dop.FormFieldShading = value;
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
                return m_version;
            }
        }
        /// <summary>
        /// Specifies the hyphenation settings of the document.
        /// </summary>
        /// <value>The hyphenation.</value>
        public Hyphenation Hyphenation
        {
            get
            {
                if (m_hyphenation == null)
                    m_hyphenation = new Hyphenation(m_dop);
                return m_hyphenation;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Properties"/> class.
        /// </summary>
        /// <param name="dop">The DOPDescriptor.</param>
        internal DocProperties(DOPDescriptor dop)
        {
            m_dop = dop;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Sets the version.
        /// </summary>
        /// <param name="version">The version.</param>
        internal void SetVersion(DocumentVersion version)
        {
            m_version = version;
        }
        #endregion
    }
    /// <summary>
    /// Class represents Hyphenation settings of the document.
    /// </summary>
    public class Hyphenation
    {
        #region Field
        private DOPDescriptor m_dop;
        #endregion

        #region Properties
        /// <summary>
        /// Specifies whether to hyphenate the document contents automatically.
        /// </summary>
        /// <value>if automatically hyphenate the document contents, set to <c>true</c>.</value>
        public bool AutoHyphenation
        {
            get
            {
                return m_dop.AutoHyphen;
            }
            set
            {
                m_dop.AutoHyphen = value;
            }
        }
        /// <summary>
        /// Specifies whether to hyphenate words in ALL CAPITAL LETTERS.
        /// </summary>
        /// <value>
        /// if hyphenate words in ALL CAPITAL LETTERS, set to <c>true</c>.
        /// </value>
        public bool HyphenateCaps
        {
            get
            {
                return m_dop.HyphCapitals;
            }
            set
            {
                m_dop.HyphCapitals = value;
            }
        }
        /// <summary>
        /// Specifies the amount of whitespace which can be left at the end of a line (or added to justified lines) before hyphenation.
        /// </summary>
        /// <value>The hyphenation zone.</value>
        public float HyphenationZone
        {
            get
            {
                return m_dop.DxaHotZ / DLSConstants.TwipsInOnePoint;
            }
            set
            {
                if (value < 0.05 || value > 1584)
                    throw new ArgumentOutOfRangeException("Hyphenation zone must be between 0.05 pt and 1584 pt.");
                m_dop.DxaHotZ = (int)(value * DLSConstants.TwipsInOnePoint);
            }
        }
        /// <summary>
        /// Specifies the maximum number of consecutive lines of text that can end with a hyphen.
        /// </summary>
        /// <value>The consecutive hyphens limit.</value>
        public int ConsecutiveHyphensLimit
        {
            get
            {
                return m_dop.ConsecHypLim;
            }
            set
            {
                if (value < 0 || value > 32767)
                    throw new ArgumentOutOfRangeException("Consecutive hyphens limit must be between 0 and 32767.");
                m_dop.ConsecHypLim = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Hyphenation"/> class.
        /// </summary>
        /// <param name="dop">The DOPDescriptor.</param>
        internal Hyphenation(DOPDescriptor dop)
        {
            m_dop = dop;
        }
        #endregion
    }
    /// <summary>
    /// Class represents Attached tempalte of the document.
    /// </summary>
    public class Template
    {
        #region Field
        private SttbfAssoc m_assocStrings;      
        #endregion

        #region Properties
        /// <summary>
        ///Gets or sets the path of the attached template.
        /// </summary>
        /// <value>The path to attached template document</value>
        public string Path
        {
            get
            {
                return  m_assocStrings.AttachedTemplate;
            }
            set
            {              
                m_assocStrings.AttachedTemplate = value;               
            }
        }        
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AttachedTemplate"/> class.
        /// </summary>
        /// <param name="assocStrings">The SttbfAssoc.</param>
        internal Template(SttbfAssoc assocStrings)
        {
            m_assocStrings = assocStrings;
        }
        #endregion
    }
    /// <summary>
    /// Class represents settings of the document.
    /// </summary>
    internal class Settings
    {
        #region Field
        private CompatibilityOptions m_compatibilityOptions;
        //Should be update the default compatibility mode to the latest version
        private CompatibilityMode m_CompatibilityMode=CompatibilityMode.Word2013;
        private DOPDescriptor m_dop;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the compatibility settings of the document.
        /// </summary>
        /// <value>The compatibility settings.</value>
        internal CompatibilityOptions CompatibilityOptions
        {
            get
            {
                if (m_compatibilityOptions == null)
                    m_compatibilityOptions = new CompatibilityOptions(m_dop);
                return m_compatibilityOptions;
            }
        }
        /// <summary>
        /// Gets the compatibility mode of the document.
        /// </summary>
        /// <value>The compatibility mode.</value>
        internal CompatibilityMode CompatibilityMode
        {
            get
            {
                return m_CompatibilityMode;
            }
            set
            {
                m_CompatibilityMode = value;
            }
        }
        #endregion

        #region Constructor
        internal Settings(DOPDescriptor dop)
        {
            m_dop = dop;
        }
        #endregion
    }
    /// <summary>
    /// Class represents compatibility options of the document.
    /// </summary>
    internal class CompatibilityOptions
    {
        #region Field
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<CompatibilityOption, bool> m_propertiesHash;
        /// <summary>
        /// 
        /// </summary>
        private DOPDescriptor m_dop;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the <see cref="System.Boolean"/> with the specified key.
        /// </summary>
        /// <value></value>
        internal bool this[CompatibilityOption key]
        {
            get
            {               
                return GetValue(key);
            }
            set
            {
                SetValue(key, value);
            }
        }
        /// <summary>
        /// Gets the properties hash.
        /// </summary>
        /// <value>The properties hash.</value>
        internal Dictionary<CompatibilityOption, bool> PropertiesHash
        {
            get
            {
                if (m_propertiesHash == null)
                {
                    m_propertiesHash = new Dictionary<CompatibilityOption, bool>();
                }
                return m_propertiesHash;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CompatibilityOptions"/> class.
        /// </summary>
        internal CompatibilityOptions(DOPDescriptor dop)
        {
            m_dop = dop;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Sets the CompatibilityOptions based on key.
        /// </summary>
        /// <param name="key">The CompatibilityOptions key.</param>
        /// <param name="value">The value.</param>
        private void SetValue(CompatibilityOption key, bool value)
        {
            switch (key)
            {
                case CompatibilityOption.NoTabForInd:
                    m_dop.Dop2000.Copts.Copts80.Copts60.NoTabForInd = value;
                    break;
                case CompatibilityOption.NoSpaceRaiseLower:
                    m_dop.Dop2000.Copts.Copts80.Copts60.NoSpaceRaiseLower = value;
                    break;
                case CompatibilityOption.SuppressSpBfAfterPgBrk:
                    m_dop.Dop2000.Copts.Copts80.Copts60.SuppressSpBfAfterPgBrk = value;
                    break;
                case CompatibilityOption.WrapTrailSpaces:
                    m_dop.Dop2000.Copts.Copts80.Copts60.WrapTrailSpaces = value;
                    break;
                case CompatibilityOption.MapPrintTextColor:
                    m_dop.Dop2000.Copts.Copts80.Copts60.MapPrintTextColor = value;
                    break;
                case CompatibilityOption.NoColumnBalance:
                    m_dop.Dop2000.Copts.Copts80.Copts60.NoColumnBalance = value;
                    break;
                case CompatibilityOption.ConvMailMergeEsc:
                    m_dop.Dop2000.Copts.Copts80.Copts60.ConvMailMergeEsc = value;
                    break;
                case CompatibilityOption.SuppressTopSpacing:
                    m_dop.Dop2000.Copts.Copts80.Copts60.SuppressTopSpacing = value;
                    break;
                case CompatibilityOption.OrigWordTableRules:
                    m_dop.Dop2000.Copts.Copts80.Copts60.OrigWordTableRules = value;
                    break;
                case CompatibilityOption.ShowBreaksInFrames:
                    m_dop.Dop2000.Copts.Copts80.Copts60.ShowBreaksInFrames = value;
                    break;
                case CompatibilityOption.SwapBordersFacingPgs:
                    m_dop.Dop2000.Copts.Copts80.Copts60.SwapBordersFacingPgs = value;
                    break;
                case CompatibilityOption.LeaveBackslashAlone:
                    m_dop.Dop2000.Copts.Copts80.Copts60.LeaveBackslashAlone = value;
                    break;
                case CompatibilityOption.ExpShRtn:
                    m_dop.Dop2000.Copts.Copts80.Copts60.ExpShRtn = value;
                    break;
                case CompatibilityOption.DntULTrlSpc:
                    m_dop.Dop2000.Copts.Copts80.Copts60.DntULTrlSpc = value;
                    break;
                case CompatibilityOption.DntBlnSbDbWid:
                    m_dop.Dop2000.Copts.Copts80.Copts60.DntBlnSbDbWid = value;
                    break;
                case CompatibilityOption.SuppressTopSpacingMac5:
                    m_dop.Dop2000.Copts.Copts80.SuppressTopSpacingMac5 = value;
                    break;
                case CompatibilityOption.TruncDxaExpand:
                    m_dop.Dop2000.Copts.Copts80.TruncDxaExpand = value;
                    break;
                case CompatibilityOption.PrintBodyBeforeHdr:
                    m_dop.Dop2000.Copts.Copts80.PrintBodyBeforeHdr = value;
                    break;
                case CompatibilityOption.NoExtLeading:
                    m_dop.Dop2000.Copts.Copts80.NoExtLeading = value;
                    break;
                case CompatibilityOption.DontMakeSpaceForUL:
                    m_dop.Dop2000.Copts.Copts80.DontMakeSpaceForUL = value;
                    break;
                case CompatibilityOption.MWSmallCaps:
                    m_dop.Dop2000.Copts.Copts80.MWSmallCaps = value;
                    break;
                case CompatibilityOption.F2ptExtLeadingOnly:
                    m_dop.Dop2000.Copts.Copts80.F2ptExtLeadingOnly = value;
                    break;
                case CompatibilityOption.TruncFontHeight:
                    m_dop.Dop2000.Copts.Copts80.TruncFontHeight = value;
                    break;
                case CompatibilityOption.SubOnSize:
                    m_dop.Dop2000.Copts.Copts80.SubOnSize = value;
                    break;
                case CompatibilityOption.LineWrapLikeWord6:
                    m_dop.Dop2000.Copts.Copts80.LineWrapLikeWord6 = value;
                    break;
                case CompatibilityOption.WW6BorderRules:
                    m_dop.Dop2000.Copts.Copts80.WW6BorderRules = value;
                    break;
                case CompatibilityOption.ExactOnTop:
                    m_dop.Dop2000.Copts.Copts80.ExactOnTop = value;
                    break;
                case CompatibilityOption.ExtraAfter:
                    m_dop.Dop2000.Copts.Copts80.ExtraAfter = value;
                    break;
                case CompatibilityOption.WPSpace:
                    m_dop.Dop2000.Copts.Copts80.WPSpace = value;
                    break;
                case CompatibilityOption.WPJust:
                    m_dop.Dop2000.Copts.Copts80.WPJust = value;
                    break;
                case CompatibilityOption.PrintMet:
                    m_dop.Dop2000.Copts.Copts80.PrintMet = value;
                    break;
                case CompatibilityOption.SpLayoutLikeWW8:
                    m_dop.Dop2000.Copts.SpLayoutLikeWW8 = value;
                    break;
                case CompatibilityOption.FtnLayoutLikeWW8:
                    m_dop.Dop2000.Copts.FtnLayoutLikeWW8 = value;
                    break;
                case CompatibilityOption.DontUseHTMLParagraphAutoSpacing:
                    m_dop.Dop2000.Copts.DontUseHTMLParagraphAutoSpacing = value;
                    break;
                case CompatibilityOption.DontAdjustLineHeightInTable:
                    m_dop.Dop2000.Copts.DontAdjustLineHeightInTable = value;
                    break;
                case CompatibilityOption.ForgetLastTabAlign:
                    m_dop.Dop2000.Copts.ForgetLastTabAlign = value;
                    break;
                case CompatibilityOption.UseAutospaceForFullWidthAlpha:
                    m_dop.Dop2000.Copts.UseAutospaceForFullWidthAlpha = value;
                    break;
                case CompatibilityOption.AlignTablesRowByRow:
                    m_dop.Dop2000.Copts.AlignTablesRowByRow = value;
                    break;
                case CompatibilityOption.LayoutRawTableWidth:
                    m_dop.Dop2000.Copts.LayoutRawTableWidth = value;
                    break;
                case CompatibilityOption.LayoutTableRowsApart:
                    m_dop.Dop2000.Copts.LayoutTableRowsApart = value;
                    break;
                case CompatibilityOption.UseWord97LineBreakingRules:
                    m_dop.Dop2000.Copts.UseWord97LineBreakingRules = value;
                    break;
                case CompatibilityOption.DontBreakWrappedTables:
                    m_dop.Dop2000.Copts.DontBreakWrappedTables = value;
                    break;
                case CompatibilityOption.DontSnapToGridInCell:
                    m_dop.Dop2000.Copts.DontSnapToGridInCell = value;
                    break;
                case CompatibilityOption.DontAllowFieldEndSelect:
                    m_dop.Dop2000.Copts.DontAllowFieldEndSelect = value;
                    break;
                case CompatibilityOption.ApplyBreakingRules:
                    m_dop.Dop2000.Copts.ApplyBreakingRules = value;
                    break;
                case CompatibilityOption.DontWrapTextWithPunct:
                    m_dop.Dop2000.Copts.DontWrapTextWithPunct = value;
                    break;
                case CompatibilityOption.DontUseAsianBreakRules:
                    m_dop.Dop2000.Copts.DontUseAsianBreakRules = value;
                    break;
                case CompatibilityOption.UseWord2002TableStyleRules:
                    m_dop.Dop2000.Copts.UseWord2002TableStyleRules = value;
                    break;
                case CompatibilityOption.GrowAutoFit:
                    m_dop.Dop2000.Copts.GrowAutoFit = value;
                    break;
                case CompatibilityOption.UseNormalStyleForList:
                    m_dop.Dop2000.Copts.UseNormalStyleForList = value;
                    break;
                case CompatibilityOption.DontUseIndentAsNumberingTabStop:
                    m_dop.Dop2000.Copts.DontUseIndentAsNumberingTabStop = value;
                    break;
                case CompatibilityOption.FELineBreak11:
                    m_dop.Dop2000.Copts.FELineBreak11 = value;
                    break;
                case CompatibilityOption.AllowSpaceOfSameStyleInTable:
                    m_dop.Dop2000.Copts.AllowSpaceOfSameStyleInTable = value;
                    break;
                case CompatibilityOption.WW11IndentRules:
                    m_dop.Dop2000.Copts.WW11IndentRules = value;
                    break;
                case CompatibilityOption.DontAutofitConstrainedTables:
                    m_dop.Dop2000.Copts.DontAutofitConstrainedTables = value;
                    break;
                case CompatibilityOption.AutofitLikeWW11:
                    m_dop.Dop2000.Copts.AutofitLikeWW11 = value;
                    break;
                case CompatibilityOption.UnderlineTabInNumList:
                    m_dop.Dop2000.Copts.UnderlineTabInNumList = value;
                    break;
                case CompatibilityOption.HangulWidthLikeWW11:
                    m_dop.Dop2000.Copts.HangulWidthLikeWW11 = value;
                    break;
                case CompatibilityOption.SplitPgBreakAndParaMark:
                    m_dop.Dop2000.Copts.SplitPgBreakAndParaMark = value;
                    break;
                case CompatibilityOption.DontVertAlignCellWithSp:
                    m_dop.Dop2000.Copts.DontVertAlignCellWithSp = value;
                    break;
                case CompatibilityOption.DontBreakConstrainedForcedTables:
                    m_dop.Dop2000.Copts.DontBreakConstrainedForcedTables = value;
                    break;
                case CompatibilityOption.DontVertAlignInTxbx:
                    m_dop.Dop2000.Copts.DontVertAlignInTxbx = value;
                    break;
                case CompatibilityOption.Word11KerningPairs:
                    m_dop.Dop2000.Copts.Word11KerningPairs = value;
                    break;
                case CompatibilityOption.CachedColBalance:
                    m_dop.Dop2000.Copts.CachedColBalance = value;
                    break;
                default:
                    if (PropertiesHash.ContainsKey(key))
                        PropertiesHash[key] = value;
                    else
                        PropertiesHash.Add(key, value);
                    break;
            }
        }
        /// <summary>
        /// Gets the CompatibilityOptions value based on key.
        /// </summary>
        /// <param name="key">The CompatibilityOptions key.</param>       
        private bool GetValue(CompatibilityOption key)
        {
            switch (key)
            {
                case CompatibilityOption.NoTabForInd:
                    return m_dop.Dop2000.Copts.Copts80.Copts60.NoTabForInd;
                    break;
                case CompatibilityOption.NoSpaceRaiseLower:
                    return m_dop.Dop2000.Copts.Copts80.Copts60.NoSpaceRaiseLower;
                    break;
                case CompatibilityOption.SuppressSpBfAfterPgBrk:
                    return m_dop.Dop2000.Copts.Copts80.Copts60.SuppressSpBfAfterPgBrk;
                    break;
                case CompatibilityOption.WrapTrailSpaces:
                    return m_dop.Dop2000.Copts.Copts80.Copts60.WrapTrailSpaces;
                    break;
                case CompatibilityOption.MapPrintTextColor:
                    return m_dop.Dop2000.Copts.Copts80.Copts60.MapPrintTextColor;
                    break;
                case CompatibilityOption.NoColumnBalance:
                    return m_dop.Dop2000.Copts.Copts80.Copts60.NoColumnBalance;
                    break;
                case CompatibilityOption.ConvMailMergeEsc:
                    return m_dop.Dop2000.Copts.Copts80.Copts60.ConvMailMergeEsc;
                    break;
                case CompatibilityOption.SuppressTopSpacing:
                    return m_dop.Dop2000.Copts.Copts80.Copts60.SuppressTopSpacing;
                    break;
                case CompatibilityOption.OrigWordTableRules:
                    return m_dop.Dop2000.Copts.Copts80.Copts60.OrigWordTableRules;
                    break;
                case CompatibilityOption.ShowBreaksInFrames:
                    return m_dop.Dop2000.Copts.Copts80.Copts60.ShowBreaksInFrames;
                    break;
                case CompatibilityOption.SwapBordersFacingPgs:
                    return m_dop.Dop2000.Copts.Copts80.Copts60.SwapBordersFacingPgs;
                    break;
                case CompatibilityOption.LeaveBackslashAlone:
                    return m_dop.Dop2000.Copts.Copts80.Copts60.LeaveBackslashAlone;
                    break;
                case CompatibilityOption.ExpShRtn:
                    return m_dop.Dop2000.Copts.Copts80.Copts60.ExpShRtn;
                    break;
                case CompatibilityOption.DntULTrlSpc:
                    return m_dop.Dop2000.Copts.Copts80.Copts60.DntULTrlSpc;
                    break;
                case CompatibilityOption.DntBlnSbDbWid:
                    return m_dop.Dop2000.Copts.Copts80.Copts60.DntBlnSbDbWid;
                    break;
                case CompatibilityOption.SuppressTopSpacingMac5:
                    return m_dop.Dop2000.Copts.Copts80.SuppressTopSpacingMac5;
                    break;
                case CompatibilityOption.TruncDxaExpand:
                    return m_dop.Dop2000.Copts.Copts80.TruncDxaExpand;
                    break;
                case CompatibilityOption.PrintBodyBeforeHdr:
                    return m_dop.Dop2000.Copts.Copts80.PrintBodyBeforeHdr;
                    break;
                case CompatibilityOption.NoExtLeading:
                    return m_dop.Dop2000.Copts.Copts80.NoExtLeading;
                    break;
                case CompatibilityOption.DontMakeSpaceForUL:
                    return m_dop.Dop2000.Copts.Copts80.DontMakeSpaceForUL;
                    break;
                case CompatibilityOption.MWSmallCaps:
                    return m_dop.Dop2000.Copts.Copts80.MWSmallCaps;
                    break;
                case CompatibilityOption.F2ptExtLeadingOnly:
                    return m_dop.Dop2000.Copts.Copts80.F2ptExtLeadingOnly;
                    break;
                case CompatibilityOption.TruncFontHeight:
                    return m_dop.Dop2000.Copts.Copts80.TruncFontHeight;
                    break;
                case CompatibilityOption.SubOnSize:
                    return m_dop.Dop2000.Copts.Copts80.SubOnSize;
                    break;
                case CompatibilityOption.LineWrapLikeWord6:
                    return m_dop.Dop2000.Copts.Copts80.LineWrapLikeWord6;
                    break;
                case CompatibilityOption.WW6BorderRules:
                    return m_dop.Dop2000.Copts.Copts80.WW6BorderRules;
                    break;
                case CompatibilityOption.ExactOnTop:
                    return m_dop.Dop2000.Copts.Copts80.ExactOnTop;
                    break;
                case CompatibilityOption.ExtraAfter:
                    return m_dop.Dop2000.Copts.Copts80.ExtraAfter;
                    break;
                case CompatibilityOption.WPSpace:
                    return m_dop.Dop2000.Copts.Copts80.WPSpace;
                    break;
                case CompatibilityOption.WPJust:
                    return m_dop.Dop2000.Copts.Copts80.WPJust;
                    break;
                case CompatibilityOption.PrintMet:
                    return m_dop.Dop2000.Copts.Copts80.PrintMet;
                    break;
                case CompatibilityOption.SpLayoutLikeWW8:
                    return m_dop.Dop2000.Copts.SpLayoutLikeWW8;
                    break;
                case CompatibilityOption.FtnLayoutLikeWW8:
                    return m_dop.Dop2000.Copts.FtnLayoutLikeWW8;
                    break;
                case CompatibilityOption.DontUseHTMLParagraphAutoSpacing:
                    return m_dop.Dop2000.Copts.DontUseHTMLParagraphAutoSpacing;
                    break;
                case CompatibilityOption.DontAdjustLineHeightInTable:
                    return m_dop.Dop2000.Copts.DontAdjustLineHeightInTable;
                    break;
                case CompatibilityOption.ForgetLastTabAlign:
                    return m_dop.Dop2000.Copts.ForgetLastTabAlign;
                    break;
                case CompatibilityOption.UseAutospaceForFullWidthAlpha:
                    return m_dop.Dop2000.Copts.UseAutospaceForFullWidthAlpha;
                    break;
                case CompatibilityOption.AlignTablesRowByRow:
                    return m_dop.Dop2000.Copts.AlignTablesRowByRow;
                    break;
                case CompatibilityOption.LayoutRawTableWidth:
                    return m_dop.Dop2000.Copts.LayoutRawTableWidth;
                    break;
                case CompatibilityOption.LayoutTableRowsApart:
                    return m_dop.Dop2000.Copts.LayoutTableRowsApart;
                    break;
                case CompatibilityOption.UseWord97LineBreakingRules:
                    return m_dop.Dop2000.Copts.UseWord97LineBreakingRules;
                    break;
                case CompatibilityOption.DontBreakWrappedTables:
                    return m_dop.Dop2000.Copts.DontBreakWrappedTables;
                    break;
                case CompatibilityOption.DontSnapToGridInCell:
                    return m_dop.Dop2000.Copts.DontSnapToGridInCell;
                    break;
                case CompatibilityOption.DontAllowFieldEndSelect:
                    return m_dop.Dop2000.Copts.DontAllowFieldEndSelect;
                    break;
                case CompatibilityOption.ApplyBreakingRules:
                    return m_dop.Dop2000.Copts.ApplyBreakingRules;
                    break;
                case CompatibilityOption.DontWrapTextWithPunct:
                    return m_dop.Dop2000.Copts.DontWrapTextWithPunct;
                    break;
                case CompatibilityOption.DontUseAsianBreakRules:
                    return m_dop.Dop2000.Copts.DontUseAsianBreakRules;
                    break;
                case CompatibilityOption.UseWord2002TableStyleRules:
                    return m_dop.Dop2000.Copts.UseWord2002TableStyleRules;
                    break;
                case CompatibilityOption.GrowAutoFit:
                    return m_dop.Dop2000.Copts.GrowAutoFit;
                    break;
                case CompatibilityOption.UseNormalStyleForList:
                    return m_dop.Dop2000.Copts.UseNormalStyleForList;
                    break;
                case CompatibilityOption.DontUseIndentAsNumberingTabStop:
                    return m_dop.Dop2000.Copts.DontUseIndentAsNumberingTabStop;
                    break;
                case CompatibilityOption.FELineBreak11:
                    return m_dop.Dop2000.Copts.FELineBreak11;
                    break;
                case CompatibilityOption.AllowSpaceOfSameStyleInTable:
                    return m_dop.Dop2000.Copts.AllowSpaceOfSameStyleInTable;
                    break;
                case CompatibilityOption.WW11IndentRules:
                    return m_dop.Dop2000.Copts.WW11IndentRules;
                    break;
                case CompatibilityOption.DontAutofitConstrainedTables:
                    return m_dop.Dop2000.Copts.DontAutofitConstrainedTables;
                    break;
                case CompatibilityOption.AutofitLikeWW11:
                    return m_dop.Dop2000.Copts.AutofitLikeWW11;
                    break;
                case CompatibilityOption.UnderlineTabInNumList:
                    return m_dop.Dop2000.Copts.UnderlineTabInNumList;
                    break;
                case CompatibilityOption.HangulWidthLikeWW11:
                    return m_dop.Dop2000.Copts.HangulWidthLikeWW11;
                    break;
                case CompatibilityOption.SplitPgBreakAndParaMark:
                    return m_dop.Dop2000.Copts.SplitPgBreakAndParaMark;
                    break;
                case CompatibilityOption.DontVertAlignCellWithSp:
                    return m_dop.Dop2000.Copts.DontVertAlignCellWithSp;
                    break;
                case CompatibilityOption.DontBreakConstrainedForcedTables:
                    return m_dop.Dop2000.Copts.DontBreakConstrainedForcedTables;
                    break;
                case CompatibilityOption.DontVertAlignInTxbx:
                    return m_dop.Dop2000.Copts.DontVertAlignInTxbx;
                    break;
                case CompatibilityOption.Word11KerningPairs:
                    return m_dop.Dop2000.Copts.Word11KerningPairs;
                    break;
                case CompatibilityOption.CachedColBalance:
                    return m_dop.Dop2000.Copts.CachedColBalance;
                    break;
                default:
                    if (PropertiesHash.ContainsKey(key))
                        return PropertiesHash[key];
                    return false;
                    break;
            }
        }
        #endregion
    }
    internal class MacroData
    {
        #region Field
        private string m_name;
        private string m_bEncrypt;
        private string m_cmg;
        #endregion

        #region Properties
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
        internal string Encrypt
        {
            get
            {
                return m_bEncrypt;
            }
            set
            {
                m_bEncrypt = value;
            }
        }
        internal string Cmg
        {
            get
            {
                return m_cmg;
            }
            set
            {
                m_cmg = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MacroData"/> class.
        /// </summary>
        internal MacroData()
        {
        }
        #endregion
    }
    internal class SttbfAssoc
    {
        #region Field
        //fExtend (2 bytes): This value MUST be 0xFFFF.
        private ushort m_fExtend = 0xFFFF;
        //cData (2 bytes): This value MUST be 0x0012.
        private ushort m_cData = 0x0012;
        //cbExtra (2 bytes): This value MUST be 0.
        private ushort m_cbExtra;
        //0x01 - The path of the associated document template (2), if it is not the default Normal template.
        private string m_template;
        //0x02 - The title of the document. This MUST be ignored if title information, as specified in [MS-OLEPS] section 3.1.2, exists in the Summary Information Stream.
        private string m_title;
        //0x03 - The subject of the document. This MUST be ignored if subject information, as specified in [MS-OLEPS] section 3.1.3, exists in the Summary Information Stream.
        private string m_subject;
        //0x04 - Key words associated with the document. This MUST be ignored if key word information, as specified in [MS-OLEPS] section 3.1.5, exists in the Summary Information Stream.
        private string m_keyWords;
        //0x06 - The author of the document. This index MUST be ignored if author information, as specified in [MS-OLEPS] section 3.1.4, exists in the Summary Information Stream.
        private string m_author;
        //0x07 - The user who last revised the document. This index MUST be ignored if last author information, as specified in [MS-OLEPS] section 3.1.8, exists in the Summary Information Stream.
        private string m_lastModifiedBy;
        //0x08 - The path of the associated mail merge data source.
        private string m_dataSource;
        //0x09 - The path of the associated mail merge header document.
        private string m_headerDocument;
        //0x11 - The write-reservation password of the document. This value MUST not exceed 15 characters in length.
        private string m_writePassword;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the attached template.
        /// </summary>
        /// <value>The attached template.</value>
        internal string AttachedTemplate
        {
            get
            {
                return m_template;
            }
            set
            {
                m_template = value;
            }
        }
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        internal string Title
        {
            get
            {
                return m_title;
            }
            set
            {
                m_title = value;
            }
        }
        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        internal string Subject
        {
            get
            {
                return m_subject;
            }
            set
            {
                m_subject = value;
            }
        }
        /// <summary>
        /// Gets or sets the key words.
        /// </summary>
        /// <value>The key words.</value>
        internal string KeyWords
        {
            get
            {
                return m_keyWords;
            }
            set
            {
                m_keyWords = value;
            }
        }
        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        /// <value>The author.</value>
        internal string Author
        {
            get
            {
                return m_author;
            }
            set
            {
                m_author = value;
            }
        }
        /// <summary>
        /// Gets or sets the last modified by.
        /// </summary>
        /// <value>The last modified by.</value>
        internal string LastModifiedBy
        {
            get
            {
                return m_lastModifiedBy;
            }
            set
            {
                m_lastModifiedBy = value;
            }
        }
        /// <summary>
        /// Gets or sets the mail merge data source.
        /// </summary>
        /// <value>The mail merge data source.</value>
        internal string MailMergeDataSource
        {
            get
            {
                return m_dataSource;
            }
            set
            {
                m_dataSource = value;
            }
        }
        /// <summary>
        /// Gets or sets the mail merge header document.
        /// </summary>
        /// <value>The mail merge header document.</value>
        internal string MailMergeHeaderDocument
        {
            get
            {
                return m_headerDocument;
            }
            set
            {
                m_headerDocument = value;
            }
        }
        /// <summary>
        /// Gets or sets the write password.
        /// </summary>
        /// <value>The write password.</value>
        internal string WritePassword
        {
            get
            {
                return m_writePassword;
            }
            set
            {
                m_writePassword = value;
            }
        }
        #endregion
 
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SttbfAssoc"/> class.
        /// </summary>
        internal SttbfAssoc()
        {
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Parses the specified associated strings.
        /// </summary>
        /// <param name="associatedStrings">The associated strings.</param>
        internal void Parse(byte[] associatedStrings)
        {
            if (associatedStrings.Length < 42)
                return;
            int offset = 0;
            //fExtend (2 bytes): This value MUST be 0xFFFF.
            m_fExtend = BaseWordRecord.ReadUInt16(associatedStrings, offset);
            offset += DLSConstants.ShortSize;
            //cData (2 bytes): This value MUST be 0x0012.
            m_cData = BaseWordRecord.ReadUInt16(associatedStrings, offset);
            offset += DLSConstants.ShortSize;
            //cbExtra (2 bytes): This value MUST be 0.
            m_cbExtra = BaseWordRecord.ReadUInt16(associatedStrings, offset);
            offset += DLSConstants.ShortSize;

            //This STTB MUST contain 18 strings.
            //0x00 - Unused. MUST be ignored.
            ushort charSize = BaseWordRecord.ReadUInt16(associatedStrings, offset);
            offset += DLSConstants.ShortSize;
            offset += charSize * Constants.BytesInWord;
            //0x01 - The path of the associated document template (2), if it is not the default Normal template.
            charSize = BaseWordRecord.ReadUInt16(associatedStrings, offset);
            offset += DLSConstants.ShortSize;
            m_template = BaseWordRecord.ReadString(associatedStrings, offset, (ushort)(charSize * Constants.BytesInWord));
            offset += charSize * Constants.BytesInWord;
            //0x02 - The title of the document. This MUST be ignored if title information, as specified in [MS-OLEPS] section 3.1.2, exists in the Summary Information Stream.
            charSize = BaseWordRecord.ReadUInt16(associatedStrings, offset);
            offset += DLSConstants.ShortSize;
            m_title = BaseWordRecord.ReadString(associatedStrings, offset, (ushort)(charSize * Constants.BytesInWord));
            offset += charSize * Constants.BytesInWord;
            //0x03 - The subject of the document. This MUST be ignored if subject information, as specified in [MS-OLEPS] section 3.1.3, exists in the Summary Information Stream.
            charSize = BaseWordRecord.ReadUInt16(associatedStrings, offset);
            offset += DLSConstants.ShortSize;
            m_subject = BaseWordRecord.ReadString(associatedStrings, offset, (ushort)(charSize * Constants.BytesInWord));
            offset += charSize * Constants.BytesInWord;
            //0x04 - Key words associated with the document. This MUST be ignored if key word information, as specified in [MS-OLEPS] section 3.1.5, exists in the Summary Information Stream.
            charSize = BaseWordRecord.ReadUInt16(associatedStrings, offset);
            offset += DLSConstants.ShortSize;
            m_keyWords = BaseWordRecord.ReadString(associatedStrings, offset, (ushort)(charSize * Constants.BytesInWord));
            offset += charSize * Constants.BytesInWord;
            //0x05 - Unused. This index MUST be ignored.
            charSize = BaseWordRecord.ReadUInt16(associatedStrings, offset);
            offset += DLSConstants.ShortSize;
            offset += charSize * Constants.BytesInWord;
            //0x06 - The author of the document. This index MUST be ignored if author information, as specified in [MS-OLEPS] section 3.1.4, exists in the Summary Information Stream.
            charSize = BaseWordRecord.ReadUInt16(associatedStrings, offset);
            offset += DLSConstants.ShortSize;
            m_author = BaseWordRecord.ReadString(associatedStrings, offset, (ushort)(charSize * Constants.BytesInWord));
            offset += charSize * Constants.BytesInWord;
            //0x07 - The user who last revised the document. This index MUST be ignored if last author information, as specified in [MS-OLEPS] section 3.1.8, exists in the Summary Information Stream.
            charSize = BaseWordRecord.ReadUInt16(associatedStrings, offset);
            offset += DLSConstants.ShortSize;
            m_lastModifiedBy = BaseWordRecord.ReadString(associatedStrings, offset, (ushort)(charSize * Constants.BytesInWord));
            offset += charSize * Constants.BytesInWord;
            //0x08 - The path of the associated mail merge data source.
            charSize = BaseWordRecord.ReadUInt16(associatedStrings, offset);
            offset += DLSConstants.ShortSize;
            m_dataSource = BaseWordRecord.ReadString(associatedStrings, offset, (ushort)(charSize * Constants.BytesInWord));
            offset += charSize * Constants.BytesInWord;
            //0x09 - The path of the associated mail merge header document.
            charSize = BaseWordRecord.ReadUInt16(associatedStrings, offset);
            offset += DLSConstants.ShortSize;
            m_headerDocument = BaseWordRecord.ReadString(associatedStrings, offset, (ushort)(charSize * Constants.BytesInWord));
            offset += charSize * Constants.BytesInWord;
            //0x0A - Unused. This index MUST be ignored.
            charSize = BaseWordRecord.ReadUInt16(associatedStrings, offset);
            offset += DLSConstants.ShortSize;
            offset += charSize * Constants.BytesInWord;
            //0x0B - Unused. This index MUST be ignored.
            charSize = BaseWordRecord.ReadUInt16(associatedStrings, offset);
            offset += DLSConstants.ShortSize;
            offset += charSize * Constants.BytesInWord;
            //0x0C - Unused. This index MUST be ignored.
            charSize = BaseWordRecord.ReadUInt16(associatedStrings, offset);
            offset += DLSConstants.ShortSize;
            offset += charSize * Constants.BytesInWord;
            //0x0D - Unused. This index MUST be ignored.
            charSize = BaseWordRecord.ReadUInt16(associatedStrings, offset);
            offset += DLSConstants.ShortSize;
            offset += charSize * Constants.BytesInWord;
            //0x0E - Unused. This index MUST be ignored.
            charSize = BaseWordRecord.ReadUInt16(associatedStrings, offset);
            offset += DLSConstants.ShortSize;
            offset += charSize * Constants.BytesInWord;
            //0x0F - Unused. This index MUST be ignored.
            charSize = BaseWordRecord.ReadUInt16(associatedStrings, offset);
            offset += DLSConstants.ShortSize;
            offset += charSize * Constants.BytesInWord;
            //0x10 - Unused. This index MUST be ignored.
            charSize = BaseWordRecord.ReadUInt16(associatedStrings, offset);
            offset += DLSConstants.ShortSize;
            offset += charSize * Constants.BytesInWord;
            //0x11 - The write-reservation password of the document. This value MUST not exceed 15 characters in length.
            charSize = BaseWordRecord.ReadUInt16(associatedStrings, offset);
            offset += DLSConstants.ShortSize;
            if (charSize > 0)
            m_writePassword = BaseWordRecord.ReadString(associatedStrings, offset, (ushort)(charSize * Constants.BytesInWord));
            offset += charSize * Constants.BytesInWord;
        }
        /// <summary>
        /// Gets the associated strings as byte array.
        /// </summary>
        /// <returns></returns>
        internal byte[] GetAssociatedStrings()
        {
            MemoryStream stream = new MemoryStream();
            //fExtend (2 bytes): This value MUST be 0xFFFF.
            BaseWordRecord.WriteUInt16(stream, m_fExtend);
            //cData (2 bytes): This value MUST be 0x0012.
            BaseWordRecord.WriteUInt16(stream, m_cData);
            //cbExtra (2 bytes): This value MUST be 0.
            BaseWordRecord.WriteUInt16(stream, m_cbExtra);

            //This STTB MUST contain 18 strings.
            //0x00 - Unused. MUST be ignored.
            BaseWordRecord.WriteUInt16(stream, 0);
            //0x01 - The path of the associated document template (2), if it is not the default Normal template.
            if (string.IsNullOrEmpty(m_template))
                BaseWordRecord.WriteUInt16(stream, 0);
            else
                BaseWordRecord.WriteString(stream, m_template);
            //0x02 - The title of the document. This MUST be ignored if title information, as specified in [MS-OLEPS] section 3.1.2, exists in the Summary Information Stream.
            if (string.IsNullOrEmpty(m_title))
                BaseWordRecord.WriteUInt16(stream, 0);
            else
                BaseWordRecord.WriteString(stream, m_title);
            //0x03 - The subject of the document. This MUST be ignored if subject information, as specified in [MS-OLEPS] section 3.1.3, exists in the Summary Information Stream.
            if (string.IsNullOrEmpty(m_subject))
                BaseWordRecord.WriteUInt16(stream, 0);
            else
                BaseWordRecord.WriteString(stream, m_subject);
            //0x04 - Key words associated with the document. This MUST be ignored if key word information, as specified in [MS-OLEPS] section 3.1.5, exists in the Summary Information Stream.
            if (string.IsNullOrEmpty(m_keyWords))
                BaseWordRecord.WriteUInt16(stream, 0);
            else
                BaseWordRecord.WriteString(stream, m_keyWords);
            //0x05 - Unused. This index MUST be ignored.
            BaseWordRecord.WriteUInt16(stream, 0);
            //0x06 - The author of the document. This index MUST be ignored if author information, as specified in [MS-OLEPS] section 3.1.4, exists in the Summary Information Stream.
            if (string.IsNullOrEmpty(m_author))
                BaseWordRecord.WriteUInt16(stream, 0);
            else
                BaseWordRecord.WriteString(stream, m_author);
            //0x07 - The user who last revised the document. This index MUST be ignored if last author information, as specified in [MS-OLEPS] section 3.1.8, exists in the Summary Information Stream.
            if (string.IsNullOrEmpty(m_lastModifiedBy))
                BaseWordRecord.WriteUInt16(stream, 0);
            else
                BaseWordRecord.WriteString(stream, m_lastModifiedBy);
            //0x08 - The path of the associated mail merge data source.
            if (string.IsNullOrEmpty(m_dataSource))
                BaseWordRecord.WriteUInt16(stream, 0);
            else
                BaseWordRecord.WriteString(stream, m_dataSource);
            //0x09 - The path of the associated mail merge header document.
            if (string.IsNullOrEmpty(m_headerDocument))
                BaseWordRecord.WriteUInt16(stream, 0);
            else
                BaseWordRecord.WriteString(stream, m_headerDocument);
            //0x0A - Unused. This index MUST be ignored.
            BaseWordRecord.WriteUInt16(stream, 0);
            //0x0B - Unused. This index MUST be ignored.
            BaseWordRecord.WriteUInt16(stream, 0);
            //0x0C - Unused. This index MUST be ignored.
            BaseWordRecord.WriteUInt16(stream, 0);
            //0x0D - Unused. This index MUST be ignored.
            BaseWordRecord.WriteUInt16(stream, 0);
            //0x0E - Unused. This index MUST be ignored.
            BaseWordRecord.WriteUInt16(stream, 0);
            //0x0F - Unused. This index MUST be ignored.
            BaseWordRecord.WriteUInt16(stream, 0);
            //0x10 - Unused. This index MUST be ignored.
            BaseWordRecord.WriteUInt16(stream, 0);
            //0x11 - The write-reservation password of the document. This value MUST not exceed 15 characters in length.
            if (string.IsNullOrEmpty(m_writePassword))
                BaseWordRecord.WriteUInt16(stream, 0);
            else
            {
                if (m_writePassword.Length > 15)
                    m_writePassword = m_writePassword.Substring(0, 15);
                BaseWordRecord.WriteString(stream, m_writePassword);
            }
            byte[] associateString = new byte[stream.Length];
#if WINRT
            byte[] buffer = stream.ToArray();
            Buffer.BlockCopy(buffer, 0, associateString, 0, (int)stream.Length);
#else
            Buffer.BlockCopy(stream.GetBuffer(), 0, associateString, 0, (int)stream.Length);
#endif
            return associateString;
        }
        #endregion
    }

    public class Footnote
    {
        #region Class Members
        /// <summary>
        /// Footnote separator.
        /// </summary>
        private WTextBody m_separator;
        /// <summary>
        /// Footnote continuation Separator.
        /// </summary>
        private WTextBody m_continuationSeparator;
        /// <summary>
        /// Footnote continuation notice.
        /// </summary>
        private WTextBody m_continuationNotice;
        /// <summary>
        /// The owner Word document.
        /// </summary>
        private WordDocument m_ownerDoc;
        #endregion

        #region class Properties
        /// <summary>
        /// Gets or sets the separator.
        /// </summary>
        /// <value>The separator.</value>
        public WTextBody Separator
        {
            get
            {
                if (m_separator == null || (m_separator.ChildEntities.Count == 0 && !m_ownerDoc.IsOpening && !m_ownerDoc.IsCloning))
                {
                    m_separator = new WTextBody(m_ownerDoc, null);
                    IWParagraph separatorPara = m_separator.AddParagraph();
                    separatorPara.AppendText(SpecialCharacters.Separator.ToString()).CharacterFormat.Special = true;
                }
                return m_separator;
            }
            set
            {
                m_separator = value;
                if (m_separator != null)
                    m_separator.SetOwner(m_ownerDoc, null);
            }
        }
        /// <summary>
        /// Gets or sets the continuation separator.
        /// </summary>
        /// <value>The continuation separator.</value>
        public WTextBody ContinuationSeparator
        {
            get
            {
                if (m_continuationSeparator == null || (m_continuationSeparator.ChildEntities.Count == 0 && !m_ownerDoc.IsOpening && !m_ownerDoc.IsCloning))
                {
                    m_continuationSeparator = new WTextBody(m_ownerDoc, null);
                    IWParagraph separatorPara = m_continuationSeparator.AddParagraph();
                    separatorPara.AppendText(SpecialCharacters.ContinuationSeparator.ToString()).CharacterFormat.Special = true;
                }
                return m_continuationSeparator;
            }
            set
            {
                m_continuationSeparator = value;
                if (m_continuationSeparator != null)
                    m_continuationSeparator.SetOwner(m_ownerDoc, null);
            }
        }
        /// <summary>
        /// Gets or sets the continuation notice.
        /// </summary>
        /// <value>The continuation notice.</value>
        public WTextBody ContinuationNotice
        {
            get
            {
                if (m_continuationNotice == null)
                    m_continuationNotice = new WTextBody(m_ownerDoc, null);
                return m_continuationNotice;
            }
            set
            {
                m_continuationNotice = value;
                if (m_continuationNotice != null)
                    m_continuationNotice.SetOwner(m_ownerDoc, null);
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Footnote" /> class.
        /// </summary>
        /// <param name="document">The document.</param>
        public Footnote(WordDocument document)
        {
            m_ownerDoc = document;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Footnote" /> class.
        /// </summary>
        /// <param name="footnote">The footnote.</param>
        internal Footnote(Footnote footnote)
        {
            m_separator = footnote.Separator.Clone() as WTextBody;
            m_continuationSeparator = footnote.ContinuationSeparator.Clone() as WTextBody;
            m_continuationNotice = footnote.ContinuationNotice.Clone() as WTextBody;
        }
        #endregion

        #region methods
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>Footnote.</returns>
        public Footnote Clone()
        {
            return new Footnote(this);
        }
        /// <summary>
        /// Sets the owner.
        /// </summary>
        /// <param name="document">The document.</param>
        internal void SetOwner(WordDocument document)
        {
            m_ownerDoc = document;
            if (m_separator != null)
                m_separator.SetOwner(m_ownerDoc, null);
            if (m_continuationSeparator != null)
                m_continuationSeparator.SetOwner(m_ownerDoc, null);
            if (m_continuationNotice != null)
                m_continuationNotice.SetOwner(m_ownerDoc, null);
        }
        #endregion
    }

    public class Endnote
    {
        #region Class Members
        /// <summary>
        /// Endnote separator
        /// </summary>
        private WTextBody m_separator;
        /// <summary>
        /// Endnote continuation Separator
        /// </summary>
        private WTextBody m_continuationSeparator;
        /// <summary>
        /// Endnote continuation notice
        /// </summary>
        private WTextBody m_continuationNotice;
        /// <summary>
        /// The owner Word document.
        /// </summary>
        private WordDocument m_ownerDoc;
        #endregion

        #region class Properties
        /// <summary>
        /// Gets or sets the separator.
        /// </summary>
        /// <value>The separator.</value>
        public WTextBody Separator
        {
            get
            {
                if (m_separator == null || (m_separator.ChildEntities.Count == 0 && !m_ownerDoc.IsOpening && !m_ownerDoc.IsCloning))
                {
                    m_separator = new WTextBody(m_ownerDoc, null);
                    IWParagraph separatorPara = m_separator.AddParagraph();
                    separatorPara.AppendText(SpecialCharacters.Separator.ToString()).CharacterFormat.Special = true;
                }
                return m_separator;
            }
            set
            {
                m_separator = value;
                if (m_separator != null)
                    m_separator.SetOwner(m_ownerDoc, null);
            }
        }
        /// <summary>
        /// Gets or sets the continuation separator.
        /// </summary>
        /// <value>The continuation separator.</value>
        public WTextBody ContinuationSeparator
        {
            get
            {
                if (m_continuationSeparator == null || (m_continuationSeparator.ChildEntities.Count == 0 && !m_ownerDoc.IsOpening && !m_ownerDoc.IsCloning))
                {
                    m_continuationSeparator = new WTextBody(m_ownerDoc, null);
                    IWParagraph separatorPara = m_continuationSeparator.AddParagraph();
                    separatorPara.AppendText(SpecialCharacters.ContinuationSeparator.ToString()).CharacterFormat.Special = true;
                }
                return m_continuationSeparator;
            }
            set
            {
                m_continuationSeparator = value;
                if (m_continuationSeparator != null)
                    m_continuationSeparator.SetOwner(m_ownerDoc, null);
            }
        }
        /// <summary>
        /// Gets or sets the continuation notice.
        /// </summary>
        /// <value>The continuation notice.</value>
        public WTextBody ContinuationNotice
        {
            get
            {
                if (m_continuationNotice == null)
                    m_continuationNotice = new WTextBody(m_ownerDoc, null);
                return m_continuationNotice;
            }
            set
            {
                m_continuationNotice = value;
                if (m_continuationNotice != null)
                    m_continuationNotice.SetOwner(m_ownerDoc, null);
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Endnote" /> class.
        /// </summary>
        /// <param name="document">The document.</param>
        public Endnote(WordDocument document)
        {
            m_ownerDoc = document;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Endnote" /> class.
        /// </summary>
        /// <param name="endnote">The endnote.</param>
        internal Endnote(Endnote endnote)
        {
            m_separator = endnote.Separator.Clone() as WTextBody;
            m_continuationSeparator = endnote.ContinuationSeparator.Clone() as WTextBody;
            m_continuationNotice = endnote.ContinuationNotice.Clone() as WTextBody;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>Endnote.</returns>
        public Endnote Clone()
        {
            return new Endnote(this);
        }
        /// <summary>
        /// Sets the owner.
        /// </summary>
        /// <param name="document">The document.</param>
        internal void SetOwner(WordDocument document)
        {
            m_ownerDoc = document;
            if (m_separator != null)
                m_separator.SetOwner(m_ownerDoc, null);
            if (m_continuationSeparator != null)
                m_continuationSeparator.SetOwner(m_ownerDoc, null);
            if (m_continuationNotice != null)
                m_continuationNotice.SetOwner(m_ownerDoc, null);
        }
        #endregion
    }
    /// <summary>
    /// Image downloading failed event handler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void ImageDownloadingFailedEventHandler(object sender, ImageDownloadingFailedEventArgs args);
    /// <summary>
    /// Image downloading failed event arguments
    /// </summary>
    public class ImageDownloadingFailedEventArgs : EventArgs
    {
        #region Class members
        private string m_URI;
        private string m_UserName;
        private string m_Password;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the image URI present in the input HTML
        /// </summary>
        public string URI
        {
            get
            {
                return m_URI;
            }
            internal set
            {
                m_URI = value;
            }
        }
        /// <summary>
        /// Gets or sets the user name
        /// </summary>
        public string UserName
        {
            get
            {
                return m_UserName;
            }
            set
            {
                m_UserName = value;
            }
        }
        /// <summary>
        /// Gets or sets the password
        /// </summary>
        public string Password
        {
            get
            {
                return m_Password;
            }
            set
            {
                m_Password = value;
            }
        }

        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initialize ImageDownloadingFailedEventArgs
        /// </summary>
        /// <param name="URI"></param>
        internal ImageDownloadingFailedEventArgs(string URI)
        {
            this.m_URI = URI;
        }
        #endregion

    }
    /// <summary>
    /// HTML Import Settings
    /// </summary>
    public class HTMLImportSettings
    {
        /// <summary>
        /// Throws event when downloading of HTTP or FTP image fails
        /// </summary>
        public event ImageDownloadingFailedEventHandler ImageDownloadingFailed;
        /// <summary>
        /// Execute Image downloading failed event
        /// </summary>
        /// <param name="URI"></param>
        /// <returns></returns>
        internal ImageDownloadingFailedEventArgs ExecuteImageDownloadingFailedEvent(string URI)
        {
            ImageDownloadingFailedEventArgs args = new ImageDownloadingFailedEventArgs(URI);

            if (ImageDownloadingFailed != null)
            {
                ImageDownloadingFailed(this, args);
            }

            return args;
        }
    }
 }
