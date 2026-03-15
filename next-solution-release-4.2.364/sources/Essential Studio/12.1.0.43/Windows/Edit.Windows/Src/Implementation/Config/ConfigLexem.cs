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

using System;
using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Text;
using System.Xml.Serialization;

using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Enums;

namespace Syncfusion.Windows.Forms.Edit.Implementation.Config
{
	/// <summary>
	/// Config lexem - construction that has to be parsed in some defined way.
	/// </summary>
	[XmlRoot( "lexem" )]
	public class ConfigLexem
		: ICollapsableConfigLexem
		, IComparable
	{
		#region Class Constants
		/// <summary>
		/// Regular expression that substitutes new line checks.
		/// </summary>
		private const string DEF_REGEX_NEWLINE_REPLACEMENT = @"(\r\n|\n\r|\n|\r)";
		/// <summary>
		/// Default name for collapsed lexem.
		/// </summary>
		private const string DEF_DEFAULT_COLLAPSE_NAME = @"...";
		#endregion

		#region Class Static Members
		/// <summary>
		/// Last auto assigned index.
		/// </summary>
		private static int m_lastIndex = -100;
		#endregion

		#region Class Members
		/// <summary>
		/// Begin symbol or word for lexem.
		/// </summary>
		private string m_strBeginBlock = string.Empty;
		/// <summary>
		///  If lexem has begin symbol and end symbol then use this property for
		/// setting end symbol. if lexem is "keyword" then this property must be
		/// set to null value.
		/// </summary>
		private string m_strEndBlock = string.Empty;
		/// <summary>
		/// If lexem can be divided into several lines or has some special rules
		/// which can continue lexem then use this setting.
		/// </summary>
		private string m_strContinueBlock = string.Empty;
		/// <summary>
		/// If many lexems have the same begin string then parser
		/// must control the order in which to
		/// interpret input as lexem.
		/// </summary>
		private int m_iPriority;
		/// <summary>
		/// Format which must be used for coloring. If format is Custom then
		/// used FormatName property for format identification.
		/// </summary>
		private FormatType m_Type = FormatType.Text;
		/// <summary>
		/// FormatName which must be used for coloring.
		/// </summary>
		private string m_strFormatName = string.Empty;
		/// <summary>
		/// Indicates whether BeginBlock property contains Regular expression or it doesn't.
		/// </summary>
		private bool m_bIsBeginRegex;
		/// <summary>
		/// Indicates whether EndBlock property contains Regular expression or it doesn't.
		/// </summary>
		private bool m_bIsEndRegex;
		/// <summary>
		/// Indicates whether ContinueBlock property contains Regular expression or it doesn't.
		/// </summary>
		private bool m_bIsContinueRegex;
		/// <summary>
		/// Indicates whether parser parses lexem internals or it doesn't. In
		/// complex constructions data between begin and end blocks can have own
		/// formats.
		/// </summary>
		private bool m_bIsComplex;
		/// <summary>
		/// Sublexems that must be skipped if they are found after BeginBlock
		/// string before EndBock string.
		/// </summary>
		private ArrayList m_arrSubLexems;
		/// <summary>
		/// List of references. If config was not found in sub lexems, then search is done in this list.
		/// </summary>
		private ArrayList m_arrReferences;
		/// <summary>
		/// This flag indicates whether parser should look for lexem's config just in local array
		/// or also look in parents.
		/// </summary>
		private bool m_bOnlyLocalSublexems;
		/// <summary>
		/// Parent of the config.
		/// </summary>
		private IConfigLexem m_ParentConfig;
		/// <summary>
		/// RegExp for begin block.
		/// </summary>
		private Regex m_beginRegExp;
		/// <summary>
		/// RegExp for middle block.
		/// </summary>
		private Regex m_middleRegExp;
		/// <summary>
		/// RegExp for end block.
		/// </summary>
		private Regex m_endRegExp;
		/// <summary>
		/// Config search manager.
		/// </summary>
		private LexemConfigsKeeper m_keeper;
		/// <summary>
		/// Flag that determines whether lexem can be collapsed.
		/// </summary>
		private bool m_bIsCollapsable;
		/// <summary>
		/// Format of the lexem config.
		/// </summary>
		private ISnippetFormat m_format;
		/// <summary>
		/// Link to the virtual config for current lexem.
		/// </summary>
		private IConfigLexem m_virtualConfig;
		/// <summary>
		/// Flag that specifies whether name of the collapsed region lexem
		/// should read by itself.
		/// </summary>
		private bool m_bIsCollapseAutoNamed;
		/// <summary>
		/// Static name of the collapsing.
		/// </summary>
		private string m_CollapseName = string.Empty;
		/// <summary>
		/// Specifies condition of being present.
		/// </summary>
		private string m_Condition = string.Empty;
		/// <summary>
		/// ID of the configuration.
		/// </summary>
		private int m_ID = -1;
		/// <summary>
		/// Expression for auto-naming.
		/// </summary>
		/// <remarks>
		/// When parsing collapsed region, parser reads text from stream until text matches given expression.
		/// You can specify different named groups here to use them later in AutoNameTemplate.
		/// </remarks>
		private string m_AutoNameExpression = string.Empty;
		/// <summary>
		/// Template of resulting text for auto-naming of collapse.
		/// </summary>
		/// <remarks>
		/// To specify "$" symbol you must use $$.
		/// To use result of some named group, use ${name}.
		/// </remarks>
		private string m_AutoNameTemplate = string.Empty;
		/// <summary>
		/// Regex for autonaming.
		/// </summary>
		private Regex m_AutoNameRegex;
		/// <summary>
		/// Language configuration this instance belongs to.
		/// </summary>
		private IConfigLanguage m_language;
		/// <summary>
		/// Sign of pseudo-ending.
		/// </summary>
		private bool m_bPseudoEnd;
		/// <summary>
		/// Autoindent lexem sign.
		/// </summary>
		private bool m_bIndent;
		/// <summary>
		/// ID of the next lexem.
		/// </summary>
		private int m_iNextID;
		/// <summary>
		/// Resolved next lexem.
		/// </summary>
		private IConfigLexem m_nextLexem;
		/// <summary>
		/// If true, choice list must be shown after entering this lexem.
		/// </summary>
		private bool m_bDropContextChoise;
		/// <summary>
		/// If true, context prompt must be shown after typing this lexem.
		/// </summary>
		private bool m_bDropContextPrompt;
		/// <summary>
		/// If true, content divider should be shown after the line,
		/// that contains lexem with this configuration.
		/// </summary>
		private bool m_bContentDivider;
		/// <summary>
		/// Specifies whether IndentationGuidelines should be visible for lexems with this config.
		/// </summary>
		private bool m_bIndentationGuideline;
		/// <summary>
		/// Value indicating if lexem should be used if there are more than one config found on one priority level.
		/// </summary>
		private bool m_bDefault;
		/// <summary>
		/// Specifies whether custom control should be used.
		/// </summary>
		private bool m_bUseCustomControl;
		/// <summary>
		/// Specifies format used in collapsed state.
		/// </summary>
		private ISnippetFormat m_formatCollapse;
		/// <summary>
		/// Name of the format that should be used in collapsed state.
		/// </summary>
		private string m_strFormatCollapseName = string.Empty;
		/// <summary>
		/// Indicates whether autoreplace triggers can be used.
		/// </summary>
		private bool m_bAllowTriggers = true;
		#endregion

		#region Class Properties
		/// <summary>
		/// Gets link to the virtual config for current lexem.
		/// </summary>
		/// <remarks>
		/// Virtual configs does not support collapsed state.
		/// </remarks>
		[XmlIgnore]
		public IConfigLexem VirtualConfig
		{
			get
			{
				if( m_virtualConfig == null )
				{
					m_virtualConfig = new VirtualConfigLexem( this );
				}

				return m_virtualConfig;
			}
		}
		/// <summary>
		/// Gets or sets format by Type and FormatName.
		/// </summary>
		[XmlIgnore]
		public ISnippetFormat Format
		{
			get
			{
				if( m_format == null )
				{
					IFormatManager manager = GetFormatManager();

					m_format = ( Type != FormatType.Custom ) ? ( manager[ Type ] ) : ( manager[ FormatName ] );
				}

				return m_format;
			}
		}
		/// <summary>
		/// Gets format used for drawing lexems in collapsed state.
		/// </summary>
		[XmlIgnore]
		public ISnippetFormat FormatCollapse
		{
			get
			{
				if( null == m_formatCollapse )
				{
					IFormatManager manager = GetFormatManager();

					if( m_strFormatCollapseName != string.Empty && null != m_strFormatCollapseName )
						m_formatCollapse = manager[ m_strFormatCollapseName ];
					else
						m_formatCollapse = manager[ FormatType.CollapsedText ];
				}

				return m_formatCollapse;
			}
		}
		/// <summary>
		/// Gets or sets parent of the config.
		/// </summary>
		[XmlIgnore]
		public IConfigLexem ParentConfig
		{
			get
			{
				return m_ParentConfig;
			}
			set
			{
				m_format = null;
				m_ParentConfig = value;
			}
		}
		/// <summary>
		/// Gets or sets begin symbol or word for lexem.
		/// </summary>
		[XmlAttribute]
		[DefaultValue( "" )]
		public string BeginBlock
		{
			get
			{
				return m_strBeginBlock;
			}
			set
			{
				if( value != m_strBeginBlock )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_strBeginBlock, value );
					m_strBeginBlock = value;
					OnBeginBlockChanged( args );
				}
			}
		}
		/// <summary>
		/// If lexem has begin symbol and end symbol then use this property for
		/// setting end symbol. if lexem is "keyword" then this property must be
		/// set to null value.
		/// </summary>
		[DefaultValue( "" )]
		[XmlAttribute]
		public string EndBlock
		{
			get
			{
				return m_strEndBlock;
			}
			set
			{
				if( value != m_strEndBlock )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_strEndBlock, value );
					m_strEndBlock = value;
					OnEndBlockChanged( args );
				}
			}
		}
		/// <summary>
		/// If lexem can be divided into several lines or has some special rules
		/// which can continue lexem then use this setting.
		/// </summary>
		[DefaultValue( "" )]
		[XmlAttribute]
		public string ContinueBlock
		{
			get
			{
				return m_strContinueBlock;
			}
			set
			{
				if( value != m_strContinueBlock )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_strContinueBlock, value );
					m_strContinueBlock = value;
					OnContinueBlockChanged( args );
				}
			}
		}
		/// <summary>
		/// If many lexems have the same begin string then parser
		/// must control the order in which to
		/// interpret input as lexem.
		/// </summary>
		[DefaultValue( 0 )]
		[XmlAttribute]
		public int Priority
		{
			get
			{
				return m_iPriority;
			}
			set
			{
				if( value != m_iPriority )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_iPriority, value );
					m_iPriority = value;
					OnPriorityChanged( args );
				}
			}
		}
		/// <summary>
		/// Gets or sets format which must be used for coloring. If format is Custom then
		/// used FormatName property for format identification.
		/// </summary>
		[XmlIgnore]
		public FormatType Type
		{
			get
			{
				return m_Type;
			}
			set
			{
				if( value != m_Type )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_Type, value );
					m_Type = value;
					OnTypeChanged( args );
					m_format = null;
				}
			}
		}
		/// <summary>
		/// Format which must be used for coloring. If format is Custom then
		/// used FormatName property for format identification.
		/// </summary>
		[DefaultValue( "Text" )]
		[XmlAttribute( "Type" )]
		public string TypeXML
		{
			get
			{
				return m_Type.ToString();
			}
			set
			{
				if( value == TypeXML )
					return;


				string[] names = Enum.GetNames( typeof( FormatType ) );
				string name = FormatType.Custom.ToString();

				for( int i = 0; i < names.Length; i++ )
				{
					if( names[ i ] == value )
					{
						name = value;
					}
				}

				Type = ( FormatType )Enum.Parse( typeof( FormatType ), name, false );

				if( Type == FormatType.Custom )
					FormatName = value;
			}
		}
		/// <summary>
		/// Gets or sets name of the format to be used in collapsed state.
		/// </summary>
		[DefaultValue( "" )]
		[XmlAttribute( "TypeCollapsed" )]
		public string TypeCollapsed
		{
			get
			{
				if( null == m_strFormatCollapseName )
					m_strFormatCollapseName = string.Empty;

				return m_strFormatCollapseName;
			}
			set
			{
				if( value != m_strFormatCollapseName )
				{
					m_strFormatCollapseName = value;
					m_formatCollapse = null;
				}
			}
		}
		/// <summary>
		/// Gets or sets FormatName which must be used for coloring.
		/// </summary>
		[DefaultValue( "Text" )]
		[XmlAttribute]
		public string FormatName
		{
			get
			{
				return m_strFormatName;
			}
			set
			{
				if( value != m_strFormatName )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_strFormatName, value );
					m_strFormatName = value;
					OnFormatNameChanged( args );
					m_format = null;
				}
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether BeginBlock property contains Regular expression or it doesn't.
		/// </summary>
		[DefaultValue( false )]
		[XmlAttribute]
		public bool IsBeginRegex
		{
			get
			{
				return m_bIsBeginRegex;
			}
			set
			{
				if( value != m_bIsBeginRegex )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_bIsBeginRegex, value );
					m_bIsBeginRegex = value;
					OnIsBeginRegexChanged( args );
				}
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether EndBlock property contains Regular expression or it doesn't.
		/// </summary>
		[DefaultValue( false )]
		[XmlAttribute]
		public bool IsEndRegex
		{
			get
			{
				return m_bIsEndRegex;
			}
			set
			{
				if( value != m_bIsEndRegex )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_bIsEndRegex, value );
					m_bIsEndRegex = value;
					OnIsEndRegexChanged( args );
				}
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether ContinueBlock property contains Regular expression or it doesn't.
		/// </summary>
		[DefaultValue( false )]
		[XmlAttribute]
		public bool IsContinueRegex
		{
			get
			{
				return m_bIsContinueRegex;
			}
			set
			{
				if( value != m_bIsContinueRegex )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_bIsContinueRegex, value );
					m_bIsContinueRegex = value;
					OnIsContinueRegexChanged( args );
				}
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether parser parses lexem internals or it doesn't. In
		/// complex constructions data between begin and end blocks can have own
		/// formats.
		/// </summary>
		[DefaultValue( false )]
		[XmlAttribute]
		public bool IsComplex
		{
			get
			{
				return m_bIsComplex;
			}
			set
			{
				if( value != m_bIsComplex )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_bIsComplex, value );
					m_bIsComplex = value;
					OnIsComplexChanged( args );
				}
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether parser should look for lexem's config just in local array
		/// or also look in parents.
		/// </summary>
		[DefaultValue( false )]
		[XmlAttribute]
		public bool OnlyLocalSublexems
		{
			get
			{
				return m_bOnlyLocalSublexems;
			}
			set
			{
				if( m_bOnlyLocalSublexems != value )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_bOnlyLocalSublexems, value );
					m_bOnlyLocalSublexems = value;
					OnOnlyLocalSublexemsChanged( args );
				}
			}
		}
		/// <summary>
		/// Gets or sets sublexems that must be skipped if they are found after BeginBlock
		/// string before EndBock string.
		/// </summary>
		[XmlArrayItem( ElementName = "lexem", IsNullable = true, Type = typeof( ConfigLexem ) )
	 , XmlArray]
		public ArrayList SubLexems
		{
			get
			{
				if( null == m_arrSubLexems )
					m_arrSubLexems = new ArrayList();

				return m_arrSubLexems;
			}
			set
			{
				if( value != m_arrSubLexems )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_arrSubLexems, value );
					m_arrSubLexems = value;
					OnSubLexemsChanged( args );
				}
			}
		}
		/// <summary>
		/// Gets or sets list of references. If config was not found in sub lexems, then search is done in this list.
		/// </summary>
		[XmlArrayItem( ElementName = "References", IsNullable = true )
	 , XmlArrayItem( ElementName = "reference", IsNullable = true, Type = typeof( ReferenceConfig ) )
	 , XmlArray]
		public ArrayList References
		{
			get
			{
				if( null == m_arrReferences )
					m_arrReferences = new ArrayList();

				return m_arrReferences;
			}
			set
			{
				if( value != m_arrReferences )
				{
					m_arrReferences = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether lexem can be collapsed.
		/// </summary>
		[DefaultValue( false )]
		[XmlAttribute]
		public bool IsCollapsable
		{
			get
			{
				return m_bIsCollapsable;
			}
			set
			{
				if( value != m_bIsCollapsable )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_bIsCollapsable, value );
					m_bIsCollapsable = value;
					OnIsBeginRegexChanged( args );
				}
			}
		}
		/// <summary>
		/// Gets or sets expression for auto-naming.
		/// </summary>
		/// <remarks>
		/// When parsing collapsed region, parser reads text from stream until text matches given expression.
		/// You can specify different named groups here to use them later in AutoNameTemplate.
		/// </remarks>
		[XmlAttribute]
		[DefaultValue( "" )]
		public string AutoNameExpression
		{
			get
			{
				return m_AutoNameExpression;
			}
			set
			{
				m_AutoNameExpression = value;
				m_AutoNameRegex = null;
			}
		}
		/// <summary>
		/// Gets or sets template of resulting text for auto-naming of collapse.
		/// </summary>
		/// <remarks>
		/// To specify "$" symbol you must use $$.
		/// To use result of some named group, use ${name}.
		/// </remarks>
		[XmlAttribute]
		[DefaultValue( "" )]
		public string AutoNameTemplate
		{
			get
			{
				return m_AutoNameTemplate;
			}
			set
			{
				m_AutoNameTemplate = value;
			}
		}
		/// <summary>
		/// Gets regular expression instance for AutoNameExpression.
		/// </summary>
		[XmlIgnore]
		public Regex AutoNameRegex
		{
			get
			{
				if( m_AutoNameRegex == null )
				{
					m_AutoNameRegex = new Regex( ConfigLexem.ReplaceNewLine( m_AutoNameExpression ), RegexOptions.IgnoreCase |
						RegexOptions.ExplicitCapture | Syncfusion.Windows.Forms.Edit.Implementation.Config.Config.DEF_COMPILED_REGEX );
				}

				return m_AutoNameRegex;
			}
		}
		/// <summary>
		/// Gets flag that specifies whether name of the collapsed region lexem
		/// should read by itself.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If this property is false, CollapseName will
		/// be used to name the region.
		/// </para>
		/// <para>
		/// Note: you should know that if this property is true, then on
		/// by-lexem reading you'll have to wait while sub-lexems will be loaded,
		/// then processed by RegEx, and only then you'll have your collapsible lexem.
		/// </para>
		/// </remarks>
		[XmlAttribute]
		[DefaultValue( false )]
		public bool IsCollapseAutoNamed
		{
			get
			{
				return m_bIsCollapseAutoNamed;
			}
			set
			{
				m_bIsCollapseAutoNamed = value;
			}
		}
		/// <summary>
		/// Gets or sets name of the collapsed region.
		/// </summary>
		/// <remarks>
		/// If <see cref="IsCollapseAutoNamed"/> is true, then
		/// this property's value will be used only when
		/// found name is empty.
		/// </remarks>
		[XmlAttribute]
		public string CollapseName
		{
			get
			{
				return m_CollapseName;
			}
			set
			{
				m_CollapseName = value;
			}
		}
		/// <summary>
		/// Gets or sets condition needed to pass check. Format: name=ON|OFF.
		/// </summary>
		[XmlAttribute]
		[DefaultValue( "" )]
		public string Condition
		{
			get
			{
				return m_Condition;
			}
			set
			{
				m_Condition = value;
			}
		}
		/// <summary>
		/// Gets or sets static unique ID of configuration node.
		/// </summary>
		[XmlAttribute]
		public int ID
		{
			get
			{
				return m_ID;
			}
			set
			{
				m_ID = value;
			}
		}
		/// <summary>
		/// Language lexem belongs to.
		/// </summary>
		[XmlIgnore]
		public IConfigLanguage Language
		{
			get
			{
				if( m_language == null )
				{
					IConfigLexem parent = m_ParentConfig;

					while( !( parent is IConfigLanguage ) )
					{
						parent = parent.ParentConfig;
					}

					m_language = parent as IConfigLanguage;

					if( m_language == null )
						throw new ApplicationException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_43 );
				}

				return m_language;
			}
		}
		/// <summary>
		/// Gets or sets sign indicating whether the end-block is just the way to exit higher by stack, or it is real ending of lexem.
		/// </summary>
		[DefaultValue( false )]
		[XmlAttribute]
		public bool IsPseudoEnd
		{
			get
			{
				return m_bPseudoEnd;
			}
			set
			{
				m_bPseudoEnd = value;
			}
		}
		/// <summary>
		/// Gets or sets sign of auto-indenting after lexem with such config.
		/// </summary>
		[DefaultValue( false )]
		[XmlAttribute]
		public bool Indent
		{
			get
			{
				return m_bIndent;
			}
			set
			{
				m_bIndent = value;
			}
		}
		/// <summary>
		/// Gets or sets ID of the lexem configuration, that follows right after current 
		/// one is parsed. Such lexem must be complex and "OnlyLocalSublexems",
		/// without beginblock and with endblock.
		/// </summary>
		[DefaultValue( 0 )]
		[XmlAttribute]
		public int NextID
		{
			get
			{
				return m_iNextID;
			}
			set
			{
				m_iNextID = value;
				m_nextLexem = null;
			}
		}
		/// <summary>
		/// gets resolved next-lexem.
		/// </summary>
		[XmlIgnore]
		public IConfigLexem NextLexem
		{
			get
			{
				if( m_nextLexem == null && m_iNextID > 0 )
				{
					IConfigLexem parent = m_ParentConfig;

					while( parent != null && !( parent is IConfigLanguage ) )
					{
						parent = parent.ParentConfig;
					}

					if( parent == null )
						throw new Exception( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_44 );

					IConfigLanguage m_language = ( IConfigLanguage )parent;
					m_nextLexem = m_language.FindConfig( m_iNextID );

					if( m_nextLexem == null )
						throw new Exception( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_45 );

					if( !m_nextLexem.IsComplex )
						throw new Exception( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_46 );

					if( m_nextLexem.EndBlock == string.Empty )
						throw new Exception( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_48 );

					ICollapsableConfigLexem collapsible = m_nextLexem as ICollapsableConfigLexem;
				}

				return m_nextLexem;
			}
		}
		/// <summary>
		/// Gets or sets sign of dropping down context choice list after entering text of the current lexem.
		/// </summary>
		/// <remarks>
		/// Can be set only on non-complex lexems.
		/// </remarks>
		[XmlAttribute]
		[DefaultValue( false )]
		public bool DropContextChoiceList
		{
			get
			{
				return m_bDropContextChoise;
			}
			set
			{
				//        if( value && IsComplex )
				//          throw new ApplicationException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_50 );

				m_bDropContextChoise = value;
			}
		}
		/// <summary>
		/// Gets or sets value that shows whether context prompt should be shown after typing text of the current lexem.
		/// </summary>
		/// <remarks>
		/// Can be set only on non-complex lexems.
		/// </remarks>
		[XmlAttribute]
		[DefaultValue( false )]
		public bool DropContextPrompt
		{
			get
			{
				return m_bDropContextPrompt;
			}
			set
			{
				m_bDropContextPrompt = value;
			}
		}
		/// <summary>
		/// Gets or Sets whether content divider should be shown below lexem.
		/// </summary>
		[XmlAttribute]
		[DefaultValue( false )]
		public bool ContentDivider
		{
			get
			{
				return m_bContentDivider;
			}
			set
			{
				if( value != m_bContentDivider )
				{
					m_bContentDivider = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether IndentationGuideline should be shown.
		/// </summary>
		[XmlAttribute]
		[DefaultValue( false )]
		public bool IndentationGuideline
		{
			get
			{
				return m_bIndentationGuideline;
			}
			set
			{
				if( value != m_bIndentationGuideline )
				{
					m_bIndentationGuideline = value;
				}
			}
		}
		/// <summary>
		/// Gets value indicating whether lexem should be used if there are more than one config found on one priority level.
		/// </summary>
		[XmlAttribute]
		[DefaultValue( false )]
		public bool DefaultInGroup
		{
			get
			{
				return m_bDefault;
			}
			set
			{
				if( m_bDefault != value )
				{
					m_bDefault = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets value indicating if custom control should be used instead of the simple lexem rendering.
		/// </summary>
		[XmlAttribute]
		[DefaultValue( false )]
		public bool UseCustomControl
		{
			get
			{
				return m_bUseCustomControl;
			}
			set
			{
				if( value != m_bUseCustomControl )
				{
					m_bUseCustomControl = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether autoreplace triggers can be used.
		/// </summary>
		[XmlAttribute]
		[DefaultValue( true )]
		public bool AllowTriggers
		{
			get
			{
				return m_bAllowTriggers;
			}
			set
			{
				m_bAllowTriggers = value;
			}
		}
		#endregion

		#region Private Properties
		/// <summary>
		/// Gets currently used macros manager.
		/// </summary>
		private MacrosManager MacrosManager
		{
			get
			{
				ConfigLanguage language = ( ConfigLanguage )this.Language;
				return language.MacrosManager;
			}
		}
		#endregion

		#region Class Events
		/// <summary>
		/// Raised when BeginBlock is changed.
		/// </summary>
		public event EventHandler BeginBlockChanged;
		/// <summary>
		/// Raised when EndBlock is changed.
		/// </summary>
		public event EventHandler EndBlockChanged;
		/// <summary>
		/// Raised when ContinueBlock is changed.
		/// </summary>
		public event EventHandler ContinueBlockChanged;
		/// <summary>
		/// Raised when BeginBlock is changed.
		/// </summary>
		public event EventHandler PriorityChanged;
		/// <summary>
		/// Raised when Type is changed.
		/// </summary>
		public event EventHandler TypeChanged;
		/// <summary>
		/// Raised when FormatName is changed.
		/// </summary>
		public event EventHandler FormatNameChanged;
		/// <summary>
		/// Raised when IsBeginRegex is changed.
		/// </summary>
		public event EventHandler IsBeginRegexChanged;
		/// <summary>
		/// Raised when IsEndRegex is changed.
		/// </summary>
		public event EventHandler IsEndRegexChanged;
		/// <summary>
		/// Raised when IsContinueRegex is changed.
		/// </summary>
		public event EventHandler IsContinueRegexChanged;
		/// <summary>
		/// Raised when IsComplex is changed.
		/// </summary>
		public event EventHandler IsComplexChanged;
		/// <summary>
		/// Raised when SubLexems is changed.
		/// </summary>
		public event EventHandler SubLexemsChanged;
		/// <summary>
		/// Raised when IsCollapsable is changed.
		/// </summary>
		public event EventHandler IsCollapsableChanged;
		/// <summary>
		/// Raised when OnlyLocalSublexems is changed.
		/// </summary>
		public event EventHandler OnlyLocalSublexemsChanged;
		#endregion

		#region Class Initialization & Finalization
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ConfigLexem()
			: this( "", "", FormatType.Text, false )
		{
		}
		/// <summary>
		/// Creates Lexem based on begin, end, format.
		/// </summary>
		/// <param name="begin">Begin of lexem configuration.</param>
		/// <param name="end">End of lexem configuration.</param>
		/// <param name="format">Format of the lexem configuration.</param>
		/// <param name="isComplex">indicates whether lexem should be complex.</param>
		public ConfigLexem( string begin, string end, FormatType format, bool isComplex )
		{
			m_strBeginBlock = begin;
			m_strEndBlock = end;
			m_Type = format;
			m_bIsComplex = isComplex;

			m_ID = --m_lastIndex;

			m_iPriority = 0;
			m_strFormatName = format.ToString();
		}
		#endregion

		#region Class Event Raisers
		/// <summary>
		/// Raises property changed event for BeginBlock.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected void RaiseBeginBlockChanged( ValueChangedEventArgs args )
		{
			if( BeginBlockChanged != null )
			{
				BeginBlockChanged( this, args );
			}
		}
		/// <summary>
		/// Raises property changed event for EndBlock.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected void RaiseEndBlockChanged( ValueChangedEventArgs args )
		{
			if( EndBlockChanged != null )
			{
				EndBlockChanged( this, args );
			}
		}
		/// <summary>
		/// Raises property changed event for ContinueBlock.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected void RaiseContinueBlockChanged( ValueChangedEventArgs args )
		{
			if( ContinueBlockChanged != null )
			{
				ContinueBlockChanged( this, args );
			}
		}
		/// <summary>
		/// Raises property changed event for Priority.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected void RaisePriorityChanged( ValueChangedEventArgs args )
		{
			if( PriorityChanged != null )
			{
				PriorityChanged( this, args );
			}
		}
		/// <summary>
		/// Raises property changed event for Type.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected void RaiseTypeChanged( ValueChangedEventArgs args )
		{
			if( TypeChanged != null )
			{
				TypeChanged( this, args );
			}
		}
		/// <summary>
		/// Raises property changed event for FormatName.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected void RaiseFormatNameChanged( ValueChangedEventArgs args )
		{
			if( FormatNameChanged != null )
			{
				FormatNameChanged( this, args );
			}
		}
		/// <summary>
		/// Raises property changed event for IsBeginRegex.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected void RaiseIsBeginRegexChanged( ValueChangedEventArgs args )
		{
			if( IsBeginRegexChanged != null )
			{
				IsBeginRegexChanged( this, args );
			}
		}
		/// <summary>
		/// Raises property changed event for IsEndRegex.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected void RaiseIsEndRegexChanged( ValueChangedEventArgs args )
		{
			if( IsEndRegexChanged != null )
			{
				IsEndRegexChanged( this, args );
			}
		}
		/// <summary>
		/// Raises property changed event for IsContinue.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected void RaiseIsContinueRegexChanged( ValueChangedEventArgs args )
		{
			if( IsContinueRegexChanged != null )
			{
				IsContinueRegexChanged( this, args );
			}
		}
		/// <summary>
		/// Raises property changed event for IsComplex.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected void RaiseIsComplexChanged( ValueChangedEventArgs args )
		{
			if( IsComplexChanged != null )
			{
				IsComplexChanged( this, args );
			}
		}
		/// <summary>
		/// Raises property changed event for SubLexems.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected void RaiseSubLexemsChanged( ValueChangedEventArgs args )
		{
			if( SubLexemsChanged != null )
			{
				SubLexemsChanged( this, args );
			}
		}
		/// <summary>
		/// Raises property changed event for IsCollapsable.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected void RaiseIsCollapsableChanged( ValueChangedEventArgs args )
		{
			if( IsCollapsableChanged != null )
			{
				IsCollapsableChanged( this, args );
			}
		}
		/// <summary>
		/// Raises property changed event for OnlyLocalSublexems.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected void RaiseOnlyLocalSublexemsChanged( ValueChangedEventArgs args )
		{
			if( OnlyLocalSublexemsChanged != null )
			{
				OnlyLocalSublexemsChanged( this, args );
			}
		}
		#endregion

		#region Class Overrides
		/// <summary>
		/// Calls raiser for BeginBlock.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected virtual void OnBeginBlockChanged( ValueChangedEventArgs args )
		{
			m_beginRegExp = null;
			RaiseBeginBlockChanged( args );
		}
		/// <summary>
		/// Calls raiser for EndBlock.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected virtual void OnEndBlockChanged( ValueChangedEventArgs args )
		{
			m_endRegExp = null;
			RaiseEndBlockChanged( args );
		}
		/// <summary>
		/// Calls raiser for ContinueBlock.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected virtual void OnContinueBlockChanged( ValueChangedEventArgs args )
		{
			m_middleRegExp = null;
			RaiseContinueBlockChanged( args );
		}
		/// <summary>
		/// Calls raiser for Priority.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected virtual void OnPriorityChanged( ValueChangedEventArgs args )
		{
			RaisePriorityChanged( args );
		}
		/// <summary>
		/// Calls raiser for Type.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected virtual void OnTypeChanged( ValueChangedEventArgs args )
		{
			RaiseTypeChanged( args );
		}
		/// <summary>
		/// Calls raiser for FormatName.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected virtual void OnFormatNameChanged( ValueChangedEventArgs args )
		{
			RaiseFormatNameChanged( args );
		}
		/// <summary>
		/// Calls raiser for IsBeginRegex.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected virtual void OnIsBeginRegexChanged( ValueChangedEventArgs args )
		{
			RaiseIsBeginRegexChanged( args );
		}
		/// <summary>
		/// Calls raiser for IsEndRegex.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected virtual void OnIsEndRegexChanged( ValueChangedEventArgs args )
		{
			RaiseIsEndRegexChanged( args );
		}
		/// <summary>
		/// Calls raiser for IsContinueRegex.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected virtual void OnIsContinueRegexChanged( ValueChangedEventArgs args )
		{
			RaiseIsContinueRegexChanged( args );
		}
		/// <summary>
		/// Calls raiser for IsComplex.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected virtual void OnIsComplexChanged( ValueChangedEventArgs args )
		{
			RaiseIsComplexChanged( args );
		}
		/// <summary>
		/// Calls raiser for OnlyLocalSublexems.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected virtual void OnOnlyLocalSublexemsChanged( ValueChangedEventArgs args )
		{
			RaiseOnlyLocalSublexemsChanged( args );
		}
		/// <summary>
		/// Calls raiser for SubLexems.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected virtual void OnSubLexemsChanged( ValueChangedEventArgs args )
		{
			RaiseSubLexemsChanged( args );
		}
		/// <summary>
		/// Calls raiser for IsCollapsable.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected virtual void OnIsCollapsableChanged( ValueChangedEventArgs args )
		{
			RaiseIsCollapsableChanged( args );
		}
		/// <summary>
		/// Gets description of the configuration.
		/// </summary>
		/// <returns>�escription of the configuration.</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append( "\n-----ConfigLexem-----\n" );
			sb.Append( "m_strBeginBlock       : " + m_strBeginBlock );
			sb.Append( "\nm_strEndBlock         : " + m_strEndBlock.ToString() );
			sb.Append( "\nm_strContinueBlock    : " + m_strContinueBlock.ToString() );
			sb.Append( "\nm_iPriority           : " + m_iPriority.ToString() );
			sb.Append( "\nm_iPriority           : " + m_bIsComplex.ToString() );
			sb.Append( "\nm_strFormatName       : " + m_strFormatName.ToString() );

			return sb.ToString();
		}
		#endregion

		#region Class Utility Methods
		/// <summary>
		/// Creates regular expression object for begin/continue/end block.
		/// </summary>
		/// <param name="str">Regular expression string.</param>
		/// <returns>Regex object with Compiled, Singleline and IgnoreCase(if needed) options set.</returns>
		private Regex CreateRegex( string str )
		{
			bool bCaseInsensitive = Language.CaseInsensitive;
			RegexOptions options =
				RegexOptions.Singleline | Syncfusion.Windows.Forms.Edit.Implementation.Config.Config.DEF_COMPILED_REGEX;

			if( bCaseInsensitive )
				options |= RegexOptions.IgnoreCase;

			return new Regex( ReplaceNewLine( str ), options );
		}
		#endregion

		#region Class Serialization Helpers
		/// <summary>
		/// Determines whether ID property value should be serialized.
		/// </summary>
		/// <returns>True if ID property value should be serialized.</returns>
		public bool ShouldSerializeID()
		{
			return m_ID >= 0;
		}
		/// <summary>
		/// Resets ID property value.
		/// </summary>
		public void ResetID()
		{
			m_ID = -1;
		}
        /// <summary>
        /// Shoulds serialize sub lexems.
        /// </summary>
        /// <returns></returns>
		public bool ShouldSerializeSubLexems()
		{
			return ( this.SubLexems.Count > 0 );
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		internal bool ShouldSerializeReferences()
		{
			return ( this.References.Count > 0 );
		}
		/// <summary>
		/// Should serialize collapse name.
		/// </summary>
		/// <returns></returns>
		internal bool ShouldSerializeCollapseName()
		{
			return ( this.IsCollapsable && m_CollapseName != DEF_DEFAULT_COLLAPSE_NAME );
		}
		#endregion

		#region Class Public Methods
		/// <summary>
		/// Drops cached links to formats.
		/// </summary>
		public void DropFormats()
		{
			m_format = null;

			for( int i = 0; i < SubLexems.Count; i++ )
			{
				ConfigLexem lex = SubLexems[ i ] as ConfigLexem;
				lex.DropFormats();
			}
		}
		/// <summary>
		/// Replaces all newline symbols in regex to the unified pattern.
		/// </summary>
		/// <param name="str">Regular expression.</param>
		/// <returns>String with changes.</returns>
		public static string ReplaceNewLine( string str )
		{
			return str.Replace( @"\n", DEF_REGEX_NEWLINE_REPLACEMENT );
		}
		/// <summary>
		/// Updates ParentConfig property of all sublexems.
		/// </summary>
		public void UpdateSublexems()
		{
			SubLexems.Sort();

			for( int i = 0; i < References.Count; i++ )
			{
				( References[ i ] as ReferenceConfig ).SetParent( this );
			}

			for( int i = 0; i < SubLexems.Count; i++ )
			{
				ConfigLexem lex = SubLexems[ i ] as ConfigLexem;
				lex.ParentConfig = this;
				lex.UpdateSublexems();
			}

			m_keeper = null;
			m_nextLexem = null;
			m_beginRegExp = null;
			m_endRegExp = null;
			m_middleRegExp = null;

			if( Language.CaseInsensitive )
			{
				m_strBeginBlock = m_strBeginBlock.ToLower();
				m_strEndBlock = m_strEndBlock.ToLower();
				m_strContinueBlock = m_strContinueBlock.ToLower();
			}
		}
		/// <summary>
		/// Checks string to the equalization to begin block.
		/// If begin block is regular expression, input string will be checked by RegExp.
		/// </summary>
		/// <param name="str">String to be checked</param>
		/// <returns>True if it can be treated as begin block.</returns>
		public bool IsEqualToBegin( string str )
		{
			if( str == null )
				throw new ArgumentNullException( "str" );

			if( str == string.Empty ) throw new ArgumentOutOfRangeException(
				"str", str, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_10 );

			if( m_bIsBeginRegex )
			{
				if( m_beginRegExp == null )
				{
					// Macroes should not be instantly replaced.
					string strBeginBlock = m_strBeginBlock;

					if( MacrosManager != null ) strBeginBlock = MacrosManager.ReplaceMacrosInString( strBeginBlock );

					m_beginRegExp = CreateRegex( strBeginBlock );
				}

				Match match = m_beginRegExp.Match( str );
				return match.Success && ( match.Value == str );
			}
			else
			{
				return string.Compare( m_strBeginBlock, str, Language.CaseInsensitive ) == 0;
			}

		}
		/// <summary>
		/// Checks string to the equalization to continue block. If continue block is regular expression, input string will be checked by RegExp.
		/// </summary>
		/// <param name="str">String to be checked.</param>
		/// <returns>True if it can be treated as continue block.</returns>
		public bool IsEqualToContinue( string str )
		{
			if( str == null ) throw new ArgumentNullException( "str" );

			if( str == string.Empty ) throw new ArgumentOutOfRangeException(
				"str", str, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_10 );

			if( m_bIsContinueRegex )
			{
				if( m_middleRegExp == null )
				{
					string strContinueBlock = m_strContinueBlock;

					if( MacrosManager != null ) strContinueBlock = MacrosManager.ReplaceMacrosInString( strContinueBlock );

					m_middleRegExp = CreateRegex( strContinueBlock );
				}

				Match match = m_middleRegExp.Match( str );
				return match.Success && ( match.Value == str );
			}
			else
			{
				return string.Compare( m_strContinueBlock, str, Language.CaseInsensitive ) == 0;
			}
		}
		/// <summary>
		/// Checks string to the equalization to end block.
		/// If end block is regular expression, input string will be checked by RegExp.
		/// </summary>
		/// <param name="str">String to be checked.</param>
		/// <returns>True if it can be treated as end block.</returns>
		public bool IsEqualToEnd( string str )
		{
			if( str == null )
				throw new ArgumentNullException( "str" );

			if( str == string.Empty ) throw new ArgumentOutOfRangeException(
				"str", str, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_10 );

			if( m_bIsEndRegex )
			{
				if( m_endRegExp == null )
				{
					string strEndBlock = m_strEndBlock;

					if( MacrosManager != null ) strEndBlock = MacrosManager.ReplaceMacrosInString( strEndBlock );

					m_endRegExp = CreateRegex( strEndBlock );
				}

				Match match = m_endRegExp.Match( str );
				return match.Success && ( match.Value == str );
			}
			else
			{
				return string.Compare( m_strEndBlock, str, Language.CaseInsensitive ) == 0;
			}
		}
		/// <summary>
		/// Searches for configs in sub-lexems. Current lexem config is not tested for equalization.
		/// If config was not found in sub-lexems, it will be searched in parent.
		/// </summary>
		/// <param name="str">String to find.</param>
		/// <returns>List of config lexems.</returns>
		public IList FindConfigs( string str )
		{
			if( str == null ) throw new ArgumentNullException( "str" );
			if( str == string.Empty )
				throw new ArgumentOutOfRangeException( "str", str, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_10 );

			if( m_keeper == null )
			{
				m_keeper = new LexemConfigsKeeper( this );
			}
			return m_keeper.GetConfigs( str );
		}
		#endregion

		#region Protected Methods
		/// <summary>
		/// Searches up the parent that supports IFormatManager interface.
		/// </summary>
		/// <returns>Format manager.</returns>
		protected IFormatManager GetFormatManager()
		{
			IConfigLexem parentConfig = m_ParentConfig;

			while( !( parentConfig is IFormatManager ) )
				parentConfig = parentConfig.ParentConfig;

			IFormatManager manager = parentConfig as IFormatManager;

			return manager;
		}
		#endregion

		#region IComparable Members
		/// <summary>
		/// Implements IComparable.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.</returns>
		public int CompareTo( object obj )
		{
			if( obj == null )
				throw new ArgumentNullException( "obj" );

			IConfigLexem secLexem = obj as IConfigLexem;

			// Possibly we are compared to string.
			if( secLexem == null )
			{
				// If we has string then compare it to begin Block.
				return ( obj is string ) ? m_strBeginBlock.CompareTo( ( string )obj ) : -1;
			}

			int res = -Priority.CompareTo( secLexem.Priority );

			if( res == 0 && secLexem.IsComplex != IsComplex )
				res = ( IsComplex ) ? -1 : 1;

			if( res == 0 )
				res = m_strBeginBlock.CompareTo( secLexem.BeginBlock );

			return res;
		}
		#endregion
	}
}