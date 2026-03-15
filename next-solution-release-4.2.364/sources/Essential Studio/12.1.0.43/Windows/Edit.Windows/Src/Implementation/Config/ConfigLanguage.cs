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
using System.IO;
using System.Xml;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

using Syncfusion.XmlSerializersCreator;
using Syncfusion.Windows.Forms.Edit;
using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Implementation;
using Syncfusion.Windows.Forms.Edit.Implementation.Formatting;
using Syncfusion.Windows.Forms.Localization;
using Syncfusion.Windows.Forms.Edit.Utils.CodeSnippets;
using Syncfusion.Windows.Forms.Edit.Utils;

namespace Syncfusion.Windows.Forms.Edit.Implementation.Config
{
	/// <summary>
	/// Class will describe how to parse source file of this type.
	/// </summary>
	public class ConfigLanguage
		: FormatManager
		, IConfigLanguage
		, IConfigLexem
		, IObjectInitialize
	{
		#region Fields
		/// <summary>
		/// Language friendly name ( like "Delphi" ).
		/// </summary>
		private string m_strLanguage = string.Empty;
		/// <summary>
		/// file extensions by which this language can be automatically linked
		/// to source file.
		/// ( like .cpp .c it's for c plus plus etc. )
		/// </summary>
		private ArrayList m_arrExtensions;
		/// <summary>
		/// Container of code snippets.
		/// </summary>
		private CodeSnippetsContainer m_snippetsContainer;
		/// <summary>
		/// storage of some complex constructions for this language
		/// like comments , string etc.
		/// ILexem represents each complex construction
		/// </summary>
		private ArrayList m_arrLexem;
		/// <summary>
		/// Storage of some very small complex constructions for this language
		/// like ++ at cpp.
		/// </summary>
		private ArrayList m_arrSplitters;
		/// <summary>
		/// One char token.
		/// </summary>
		private string m_strOneCharToken = string.Empty;
		/// <summary>
		/// Known for object formats. Here is list of defined in config file
		/// formats. If format not defined, but it belong to default formats
		/// specified by FormatType enum then will be used default configuration
		/// for it. Each string hold one extension.
		/// </summary>
		private ArrayList m_arrKnownFormats = new ArrayList();
		/// <summary>
		/// Config search manager.
		/// </summary>
		private LexemConfigsKeeper m_keeper;
		/// <summary>
		/// Cache of the default format.
		/// </summary>
		private ISnippetFormat m_format;
		/// <summary>
		/// Empty list of references.
		/// </summary>
		private ArrayList m_emptyReferencesList = new ArrayList();
		/// <summary>
		/// Case insensitivity of the language.
		/// </summary>
		private bool m_bCaseInsensetive;
		/// <summary>
		/// Known language, associated with the configuration.
		/// </summary>
		private KnownLanguages m_knownLanguage;
		/// <summary>
		/// Specifies macros manager.
		/// </summary>
		private MacrosManager m_macrosManager;
		/// <summary>
		/// Internal language index.
		/// </summary>
		private int m_iLanguageIndex = _iLastLoadedLanguageIndex++;
		/// <summary>
		/// Last loaded language index.
		/// </summary>
		private static int _iLastLoadedLanguageIndex = 0;
		/// <summary>
		/// String representing beginning of comment for this language.
		/// </summary>
		private string m_strStartComment = string.Empty;
		/// <summary>
		/// String representing end of comment for this language.
		/// </summary>
		private string m_strEndComment = string.Empty;
		/// <summary>
		/// List of autoreplace triggers.
		/// </summary>
		private ArrayList m_arrAutoReplaceTriggers = new ArrayList();
		/// <summary>
		/// Array of autoreplace triggets activators
		/// </summary>
		private char[] m_arrTriggersActivators;
		/// <summary>
		/// String of triggers activators.
		/// </summary>
		private string m_strTriggersActivatorsString = string.Empty;
		#endregion

		#region Properties
		/// <summary>
		/// Gets language index.
		/// </summary>
		internal int LanguageIndex
		{
			get
			{
				return m_iLanguageIndex;
			}
		}
		/// <summary>
		/// Gets or sets language friendly name ( like "Delphi" ).
		/// </summary>
		[XmlAttribute]
		public string Language
		{
			get
			{
				return m_strLanguage;
			}
			set
			{
				if( value != m_strLanguage )
				{
					ValueChangedEventArgs args = new ValueChangedEventArgs( m_strLanguage, value );
					m_strLanguage = value;
					OnLanguageChanged( args );
				}
			}
		}
		/// <summary>
		/// Gets or sets one char token splits.
		/// </summary>
		[XmlIgnore]
		public string OneCharTokenSplits
		{
			get
			{
				return m_strOneCharToken;
			}
			set
			{
				m_strOneCharToken = value;
			}
		}
		/// <summary>
		/// Gets or sets storage of some complex constructions for this language like comments, string etc.
		/// ILexem represents each complex construction.
		/// </summary>
		[XmlArrayItem( ElementName = "lexems", IsNullable = true )
	 , XmlArrayItem( ElementName = "lexem", IsNullable = true, Type = typeof( ConfigLexem ) )
	 , XmlArray]
		public ArrayList Lexems
		{
			get
			{
				if( null == m_arrLexem )
					m_arrLexem = new ArrayList();

				return m_arrLexem;
			}
			set
			{
				if( null == value )
					throw new ArgumentNullException( "value" );

				m_arrLexem.Clear();
				m_arrLexem.AddRange( value );

				m_keeper = null;
			}
		}
		/// <summary>
		/// Gets or sets storage of some very small complex constructions for this language like ++ at cpp.
		/// </summary>
		[XmlArrayItem( ElementName = "splits", IsNullable = true )
	 , XmlArrayItem( ElementName = "split", IsNullable = true, Type = typeof( string ) )
	 , XmlArray]
		public ArrayList Splits
		{
			get
			{
				return m_arrSplitters;
			}
			set
			{
				if( null == value )
					throw new ArgumentNullException( "value" );

				m_arrSplitters = value;
			}
		}
		/// <summary>
		/// Gets or sets file extensions by which this language can be automatically linked to source file ( like .cpp .c it's for c plus plus etc.).
		/// </summary>
		[XmlArrayItem( ElementName = "extensions", IsNullable = true )
	 , XmlArrayItem( ElementName = "extension", IsNullable = true, Type = typeof( string ) )
	 , XmlArray]
		public ArrayList Extensions
		{
			get
			{
				return m_arrExtensions;
			}
			set
			{
				m_arrExtensions.Clear();
				m_arrExtensions.AddRange( value );
			}
		}
		/// <summary>
		/// Gets container of code snippets.
		/// </summary>
		public CodeSnippetsContainer SnippetsContainer
		{
			get
			{
				if( m_snippetsContainer == null ) m_snippetsContainer = new CodeSnippetsContainer();

				return m_snippetsContainer;
			}
		}
		/// <summary>
		/// Gets or sets list of autoreplace triggers.
		/// </summary>
		public ArrayList AutoReplaceTriggers
		{
			get
			{
				return m_arrAutoReplaceTriggers;
			}
			set
			{
				m_arrAutoReplaceTriggers.Clear();
				m_arrAutoReplaceTriggers.AddRange( value );
			}
		}
		/// <summary>
		/// Known for object formats. Here is list of defined in config file
		/// formats. If format not defined, but it belong to default formats
		/// specified by FormatType enum then will be used default configuration
		/// for it. Each string hold one extension.
		/// </summary>
		[XmlIgnore]
		public ArrayList KnownFormats
		{
			get
			{
				return m_arrKnownFormats;
			}
		}
		/// <summary>
		/// Case insensitivity of the language.
		/// </summary>
		[XmlAttribute( "CaseInsensitive" )]
		public bool CaseInsensitive
		{
			get
			{
				return m_bCaseInsensetive;
			}
			set
			{
				m_bCaseInsensetive = value;
			}
		}
		/// <summary>
		/// Gets or sets currently assigned known language.
		/// </summary>
		public KnownLanguages KnownLanguage
		{
			get
			{
				return m_knownLanguage;
			}
			set
			{
				m_knownLanguage = value;
			}
		}
		/// <summary>
		/// Gets or sets macros manager used to process macroses in regular expressions.
		/// </summary>
		internal MacrosManager MacrosManager
		{
			get
			{
				return m_macrosManager;
			}
			set
			{
				m_macrosManager = value;
			}
		}
		/// <summary>
		/// Gets or sets string representing beginning of comment for this language.
		/// If EndComment is empty string, BeginComment is inserted in each of the commented lines.
		/// </summary>
		[XmlAttribute( "StartComent" )]
		public string StartComment
		{
			get
			{
				return m_strStartComment;
			}
			set
			{
				m_strStartComment = value;
			}
		}
        /// <summary>
        /// Gets or sets the value for Cached.     
        /// </summary>
        public bool Cached
        {
            get
            {
                return cached;
            }
            set
            {
                cached = value;
            }
        }
		/// <summary>
		/// Gets or sets string representing end of comment for this language.
		/// If EndComment is empty string, BeginComment is inserted in each of the commented lines.
		/// </summary>
		[XmlAttribute( "EndComment" )]
		public string EndComment
		{
			get
			{
				return m_strEndComment;
			}
			set
			{
				m_strEndComment = value;
			}
		}
		/// <summary>
		///	Gets or sets array of autoreplace triggers activators.
		/// </summary>
		public char[] TriggersActivators
		{
			get
			{
				return m_arrTriggersActivators;
			}
			set
			{
				m_arrTriggersActivators = value;
				m_strTriggersActivatorsString = new String( m_arrTriggersActivators );
			}
		}
		/// <summary>
		/// Gets string with autoreplace triggers activators.
		/// </summary>
		public string TriggersActivatorsString
		{
			get
			{
				return m_strTriggersActivatorsString;
			}
		}
		#endregion

		#region Events
		/// <summary>
		/// Someone changed language
		/// </summary>
		[Category( "Property Changed" )]
		public event ValueChangedEventHandler LanguageChanged;
		/// <summary>
		/// Someone changed MaxLineHeight
		/// </summary>
		[Category( "Property Changed" )]
		public event ValueChangedEventHandler MaxLineHeightChanged;
		/// <summary>
		/// Someone changed MinLineHeight
		/// </summary>
		[Category( "Property Changed" )]
		public event ValueChangedEventHandler MinLineHeightChanged;
		/// <summary>
		/// Someone changed MaxCharWidth
		/// </summary>
		[Category( "Property Changed" )]
		public event ValueChangedEventHandler MaxCharWidthChanged;
		/// <summary>
		/// Someone changed MinCharWidth
		/// </summary>
		[Category( "Property Changed" )]
		public event ValueChangedEventHandler MinCharWidthChanged;
		#endregion

		#region Initialize/Finalize Methods
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ConfigLanguage()
		{
			m_arrExtensions = new ArrayList();
			m_arrLexem = new ArrayList();
			m_arrSplitters = new ArrayList();

			Initialize();
		}
		/// <summary>
		/// Creates config language using one name.
		/// </summary>
		/// <param name="name">Name of language ( like "Delphi" ).</param>
		public ConfigLanguage( string name )
			: this()
		{
			if( name == null ) throw new ArgumentNullException( "name" );
			if( name.Length == 0 ) throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_14 );

			m_strLanguage = name;
		}
		/// <summary>
		/// Creates config language using name and extensions array.
		/// </summary>
		/// <param name="name">Name of language.</param>
		/// <param name="extensions">File extensions which CL supports.</param>
		public ConfigLanguage( string name, string[] extensions )
			: this( name )
		{
			if( extensions == null ) throw new ArgumentNullException( "extensions" );

			m_arrExtensions.AddRange( extensions );
		}
		/// <summary>
		/// Creates config language using name and extensions array and lexem array.
		/// </summary>
		/// <param name="name">Language name</param>
		/// <param name="arrExt">File extensions which CL supports.</param>
		/// <param name="arrLexem">Lexems array.</param>
		public ConfigLanguage( string name, string[] arrExt, IConfigLexem[] arrLexem )
			: this( name, arrExt )
		{
			if( null == arrLexem ) throw new ArgumentNullException( "arrLexem" );
			if( null == arrExt ) throw new ArgumentNullException( "arrExt" );

			m_arrLexem.AddRange( arrLexem );
		}
		/// <summary>
		/// Creates config language using name and extensions array, lexem array and splits array.
		/// </summary>
		/// <param name="name">Language name.</param>
		/// <param name="arrExt">File extensions which CL supports.</param>
		/// <param name="arrLexem">Lexems array.</param>
		/// <param name="arrSplits">Splits array.</param>
		public ConfigLanguage( string name, string[] arrExt, IConfigLexem[] arrLexem, Split[] arrSplits )
			: this( name, arrExt, arrLexem )
		{
			if( null == arrLexem ) throw new ArgumentNullException( "arrLexem" );
			if( null == arrSplits ) throw new ArgumentNullException( "arrSplits" );
			if( null == arrExt ) throw new ArgumentNullException( "arrExt" );

			m_arrSplitters.AddRange( arrSplits );
		}
		#endregion

		#region Event Raisers
		/// <summary>
		/// Raises change language event.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs.</param>
		protected void RaiseLanguageChanged( ValueChangedEventArgs args )
		{
			if( LanguageChanged != null )
			{
				LanguageChanged( this, args );
			}
		}
		/// <summary>
		/// Raises change MaxLineHeight event.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs.</param>
		protected void RaiseMaxLineHeightChanged( ValueChangedEventArgs args )
		{
			if( MaxLineHeightChanged != null )
			{
				MaxLineHeightChanged( this, args );
			}
		}
		/// <summary>
		/// Raises change MinLineHeight event.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected void RaiseMinLineHeightChanged( ValueChangedEventArgs args )
		{
			if( MinLineHeightChanged != null )
			{
				MinLineHeightChanged( this, args );
			}
		}
		/// <summary>
		/// Raises change MaxCharWidth event.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected void RaiseMaxCharWidthChanged( ValueChangedEventArgs args )
		{
			if( MaxCharWidthChanged != null )
			{
				MaxCharWidthChanged( this, args );
			}
		}
		/// <summary>
		/// Raises change MinCharWidth event.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected void RaiseMinCharWidthChanged( ValueChangedEventArgs args )
		{
			if( MinCharWidthChanged != null )
			{
				MinCharWidthChanged( this, args );
			}
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Call raiser for language.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected virtual void OnLanguageChanged( ValueChangedEventArgs args )
		{
			RaiseLanguageChanged( args );
		}
		/// <summary>
		/// Call raiser for MaxLineHeight.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected virtual void OnMaxLineHeightChanged( ValueChangedEventArgs args )
		{
			RaiseMaxLineHeightChanged( args );
		}
		/// <summary>
		/// Call raiser for MinLineHeight.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected virtual void OnMinLineHeightChanged( ValueChangedEventArgs args )
		{
			RaiseMinLineHeightChanged( args );
		}
		/// <summary>
		/// Call raiser for MaxCharWidth.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected virtual void OnMaxCharWidthChanged( ValueChangedEventArgs args )
		{
			RaiseMaxCharWidthChanged( args );
		}
		/// <summary>
		/// Call raiser for MinCharWidth.
		/// </summary>
		/// <param name="args">ValueChangedEventArgs</param>
		protected virtual void OnMinCharWidthChanged( ValueChangedEventArgs args )
		{
			RaiseMinCharWidthChanged( args );
		}
		/// <summary>
		/// Represents string representation of the object.
		/// </summary>
		/// <returns>String representation of the object.</returns>
		public override string ToString()
		{
			return this.Language;
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Gets lexem configuration by its ID.
		/// </summary>
		/// <param name="iConfigID">ID of configuration.</param>
		public IConfigLexem FindConfig( int iConfigID )
		{
			return GetConfigLexem( this, iConfigID );
		}
		/// <summary>
		/// Resets all cached data. Must be called after every change of the configuration inside the language.
		/// </summary>
		public void ResetCaches()
		{
			foreach( ConfigLexem lexem in SubLexems )
			{
				lexem.ParentConfig = this;
				lexem.UpdateSublexems();
			}

			// Reset internal data.
			m_keeper = null;
			m_arrLexem.Sort();
            this.Cached = true;
            OnLanguageChanged(new ValueChangedEventArgs("", ""));
            this.Cached = false;
		}
        bool cached = false;
		/// <summary>
		/// Drops caches of formats.
		/// </summary>
		/// <param name="format">Changed format.</param>
		protected override void OnFormatChanged( Format format )
		{
			foreach( ConfigLexem lex in SubLexems )
			{
				lex.DropFormats();
			}

			base.OnFormatChanged( format );
		}
		/// <summary>
		/// Adds new code snippet to the language.
		/// </summary>
		/// <param name="title">Title of code snippet.</param>
		/// <param name="literals">List of literals.</param>
		/// <param name="code">Code of snippet.</param>
		public void AddCodeSnippet( string title, ArrayList literals, string code )
		{
			if( title == null || title == string.Empty ) throw new ArgumentNullException( "title" );
			if( code == null || code == string.Empty ) throw new ArgumentOutOfRangeException( "code" );

			if( literals == null ) literals = new ArrayList();

			this.SnippetsContainer.AddSnippet( new CodeSnippet( title, literals, code ) );
		}
		/// <summary>
		/// Adds code snippet to the language.
		/// </summary>
		/// <param name="snippet">Code snippet to be added.</param>
		public void AddCodeSnippet( CodeSnippet snippet )
		{
			this.SnippetsContainer.AddSnippet( snippet );
		}
		/// <summary>
		/// Adds new code snippets container to the language.
		/// </summary>
		/// <param name="container">Code snippets container to be added.</param>
		public void AddCodeSnippetsContainer( CodeSnippetsContainer container )
		{
			this.SnippetsContainer.AddContainer( container );
		}
		#endregion

		#region Utility Methods
		/// <summary>
		/// Searches lexem configuration by its ID in given configuration and its sub-configurations.
		/// </summary>
		/// <param name="root">Root of the search.</param>
		/// <param name="iID">ID if the configuration to be found.</param>
		/// <returns>Lexem configuration with given ID, or null if nothing was found.</returns>
		protected IConfigLexem GetConfigLexem( IConfigLexem root, int iID )
		{
			if( root == null ) throw new ArgumentNullException( "root" );

			if( root.ID == iID )
				return root;

			foreach( IConfigLexem config in root.SubLexems )
			{
				IConfigLexem result = GetConfigLexem( config, iID );

				if( result != null )
					return result;
			}

			return null;
		}
		#endregion

		#region IObjectInitialize Members
		/// <summary>
		/// Initialize some objects.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
			m_arrKnownFormats.AddRange( m_hashNames.Keys );
		}
		#endregion

		#region IXmlSerializable Members
		/// <summary>
		/// Writes current instance to Xml.
		/// Attention! Start element "ConfigLanguage" must be already written and must be closed after execution of this method.
		/// </summary>
		/// <param name="writer">XmlWriter.</param>
		public override void WriteXml( XmlWriter writer )
		{
			if( null == writer ) throw new ArgumentNullException( "writer" );

			WriteConfigLanguage( writer );
		}
		/// <summary>
		/// Reads instance from Xml.
		/// </summary>
		/// <param name="reader">XmlReader.</param>
		public override void ReadXml( XmlReader reader )
		{
			if( null == reader ) throw new ArgumentNullException( "reader" );

			ReadConfigLanguage( reader );
		}
		/// <summary>
		///
		/// </summary>
		/// <param name="reader"></param>
		private void ReadConfigLanguage( XmlReader reader )
		{
			if( null == reader )
				throw new ArgumentNullException( "reader" );

			reader.MoveToContent();
			// language name
			reader.MoveToElement();

			reader.MoveToAttribute( "name" );
			reader.ReadAttributeValue();
			m_strLanguage = reader.Value;

			if( reader.MoveToAttribute( "CaseInsensitive" ) )
			{
				reader.ReadAttributeValue();
				m_bCaseInsensetive = bool.Parse( reader.Value );
			}

			if( reader.MoveToAttribute( "TriggersActivators" ) )
			{
				reader.ReadAttributeValue();
				m_strTriggersActivatorsString = reader.Value;
				m_arrTriggersActivators = m_strTriggersActivatorsString.ToCharArray();
			}

			if( reader.MoveToAttribute( "Known" ) )
			{
				reader.ReadAttributeValue();
				m_knownLanguage = ( KnownLanguages )Enum.Parse( typeof( KnownLanguages ), reader.Value, true );
			}
			else
			{
				m_knownLanguage = KnownLanguages.Undefined;
			}

			if( reader.MoveToAttribute( "StartComment" ) )
			{
				reader.ReadAttributeValue();
				m_strStartComment = reader.Value;
			}

			if( reader.MoveToAttribute( "EndComment" ) )
			{
				reader.ReadAttributeValue();
				m_strEndComment = reader.Value;
			}

			reader.MoveToElement();
			reader.ReadStartElement( "ConfigLanguage" );

			// formats
			do
			{
				if( reader.NodeType == XmlNodeType.Comment || reader.IsEmptyElement )
				{
					reader.Read();
				}
				else if( 0 == string.Compare( "formats", reader.Name, true ) )
				{
					base.ReadXml( reader );
				}
				else if( 0 == string.Compare( "extensions", reader.Name, true ) )
				{
					ReadExtension( reader );
				}
				else if( 0 == string.Compare( "lexems", reader.Name, true ) )
				{
					ReadLexems( reader );
				}
				else if( 0 == string.Compare( "splits", reader.Name, true ) )
				{
					ReadSplits( reader );
				}
				else if( 0 == string.Compare( "CodeSnippetsContainer", reader.Name, true ) )
				{
					ReadCodeSnippetsContainer( reader );
				}
				else if( 0 == string.Compare( "AutoReplaceTriggers", reader.Name, true ) )
				{
					ReadAutoReplaceTriggers( reader );
				}
				else if( 0 == string.Compare( "one-char", reader.Name, true ) )
				{
					if( !reader.IsEmptyElement )
					{
						this.OneCharTokenSplits = reader.ReadString();
					}
					reader.Read();
				}

				if( 0 == string.Compare( reader.Name, "configlanguage", true ) )
					break;
			}
			while( true );
			reader.ReadEndElement();
		}
		/// <summary>
		///
		/// </summary>
		/// <param name="writer"></param>
		private void WriteConfigLanguage( XmlWriter writer )
		{
			if( null == writer )
				throw new ArgumentNullException( "writer" );

			// language name
			writer.WriteAttributeString( "name", m_strLanguage );

			if( m_bCaseInsensetive == true )
			{
				writer.WriteAttributeString( "CaseInsensitive", m_bCaseInsensetive.ToString() );
			}
            if (m_strStartComment != string.Empty)
            {
                writer.WriteAttributeString("StartComment", m_strStartComment);
            }
            if (m_strEndComment != string.Empty)
            {
                writer.WriteAttributeString("EndComment", m_strEndComment);
            }      
			string triggersActivators = new string( m_arrTriggersActivators );

			if( triggersActivators != string.Empty )
			{
				writer.WriteAttributeString( "TriggersActivators", triggersActivators );
			}

			if( KnownLanguages.Undefined != m_knownLanguage )
				writer.WriteAttributeString( "Known", m_knownLanguage.ToString() );

			// formats
			base.WriteXml( writer );

			// extensions
			if( m_arrExtensions.Count > 0 )
			{
				writer.WriteStartElement( "extensions" );
				foreach( string key in m_arrExtensions )
					writer.WriteElementString( "extension", key );
				writer.WriteEndElement();
			}

			// lexems
			XmlSerializer valueSer = SerializersManager.GetSerializer( typeof( ConfigLexem ) );
			writer.WriteStartElement( "lexems" );
			foreach( IConfigLexem key in m_arrLexem )
				valueSer.Serialize( writer, key, null );
			writer.WriteEndElement();

			// splits
			if( m_arrSplitters.Count > 0 )
			{
				valueSer = SerializersManager.GetSerializer( typeof( Split ) );
				writer.WriteStartElement( "splits" );
				foreach( Split split in m_arrSplitters )
					valueSer.Serialize( writer, split, null );
				writer.WriteEndElement();
			}

			// Autoreplace triggers.
			if( m_arrAutoReplaceTriggers.Count > 0 )
			{
				valueSer = SerializersManager.GetSerializer( typeof( AutoReplaceTrigger ) );
				writer.WriteStartElement( "AutoReplaceTriggers" );
				foreach( AutoReplaceTrigger trigger in m_arrAutoReplaceTriggers )
					valueSer.Serialize( writer, trigger, null );
				writer.WriteEndElement();
			}

			// Code snippets container.
			if( m_snippetsContainer != null && !m_snippetsContainer.IsEmpty )
			{
				valueSer = SerializersManager.GetSerializer( typeof( CodeSnippetsContainer ) );
				valueSer.Serialize( writer, this.SnippetsContainer );
			}
		}
		/// <summary>
		///  read extensions from config
		/// </summary>
		/// <param name="reader"></param>
		private void ReadExtension( XmlReader reader )
		{
			if( null == reader )
				throw new ArgumentNullException( "reader" );

			if( !reader.IsEmptyElement )
			{
				reader.ReadStartElement( "extensions" );

				while( reader.NodeType != XmlNodeType.EndElement )
				{
					if( reader.NodeType == XmlNodeType.Comment )
					{
						reader.Read();
					}
					else
					{
						string strExt = reader.ReadElementString( "extension" );
						m_arrExtensions.Add( strExt );
					}
				}

				reader.ReadEndElement();
			}
			else
			{
				reader.Read();
			}
		}
		/// <summary>
		/// read lexems from config
		/// </summary>
		/// <param name="reader"></param>
		private void ReadLexems( XmlReader reader )
		{
			if( null == reader )
				throw new ArgumentNullException( "reader" );

			if( !reader.IsEmptyElement )
			{
				XmlSerializer lexSer = SerializersManager.GetSerializer( typeof( ConfigLexem ) );
				reader.ReadStartElement( "lexems" );

				while( reader.NodeType != XmlNodeType.EndElement )
				{
					ConfigLexem confLexem = ( ConfigLexem )lexSer.Deserialize( reader );
					confLexem.ParentConfig = this;
					confLexem.UpdateSublexems();
					m_arrLexem.Add( confLexem );
					reader.MoveToContent();
				}

				reader.ReadEndElement();

				m_arrLexem.Sort();
			}
			else
			{
				reader.Read();
			}
		}
		/// <summary>
		/// Reads autoreplace triggers from config Xml.
		/// </summary>
		/// <param name="reader"></param>
		private void ReadAutoReplaceTriggers( XmlReader reader )
		{
			if( !reader.IsEmptyElement )
			{
				XmlSerializer lexSer = SerializersManager.GetSerializer( typeof( AutoReplaceTrigger ) );
				reader.ReadStartElement( "AutoReplaceTriggers" );

				while( reader.NodeType != XmlNodeType.EndElement )
				{
					AutoReplaceTrigger trigger = ( AutoReplaceTrigger )lexSer.Deserialize( reader );
					m_arrAutoReplaceTriggers.Add( trigger );
					reader.MoveToContent();
				}

				reader.ReadEndElement();
			}
			else
			{
				reader.Read();
			}
		}
		/// <summary>
		/// read splits from config
		/// </summary>
		/// <param name="reader"></param>
		private void ReadSplits( XmlReader reader )
		{
			if( null == reader )
				throw new ArgumentNullException( "reader" );

			if( !reader.IsEmptyElement )
			{
				reader.ReadStartElement( "splits" );
				XmlSerializer splitSer = SerializersManager.GetSerializer( typeof( Split ) );

				while( reader.NodeType != XmlNodeType.EndElement )
				{
					Split split = ( Split )splitSer.Deserialize( reader );
					m_arrSplitters.Add( split );
					reader.MoveToContent();
				}

				reader.ReadEndElement();
			}
			else
			{
				reader.Read();
			}
		}
		/// <summary>
		/// Reads container of code snippets from Xml.
		/// </summary>
		/// <param name="reader"></param>
		private void ReadCodeSnippetsContainer( XmlReader reader )
		{
			if( null == reader )
				throw new ArgumentNullException( "reader" );

			if( !reader.IsEmptyElement )
			{
				XmlSerializer snippetSer = SerializersManager.GetSerializer( typeof( CodeSnippetsContainer ) );

				m_snippetsContainer = ( CodeSnippetsContainer )snippetSer.Deserialize( reader );
			}
			else
			{
				reader.Read();
			}
		}
		#endregion

		#region IConfigLexem Members
		/// <summary>
		/// Gets name of the format to be used in collapsed state.
		/// </summary>
		[XmlIgnore]
		public string TypeCollapsed
		{
			get
			{
				return FormatType.CollapsedText.ToString();
			}
		}
		/// <summary>
		/// Gets begin symbol or word for lexem.
		/// </summary>
		[XmlIgnore]
		public string BeginBlock
		{
			get
			{
				return string.Empty;
			}
		}
		/// <summary>
		/// If lexem has begin symbol and end symbol then use this property for
		/// setting end symbol. if lemex is "keyword" then this property must be
		/// set to null value.
		/// </summary>
		[XmlIgnore]
		public string EndBlock
		{
			get
			{
				return string.Empty;
			}
		}
		/// <summary>
		/// If lexem can be divided on multi lines or has some special rules
		/// which can continue lexem then us this setting.
		/// </summary>
		[XmlIgnore]
		public string ContinueBlock
		{
			get
			{
				return string.Empty;
			}
		}
		/// <summary>
		/// If many lexems has the same begin string then on parsing
		/// must be controlled order in which lexem parser will try to
		/// interpret input as lexem...
		/// </summary>
		[XmlIgnore]
		public int Priority
		{
			get
			{
				return 0;
			}
		}
		/// <summary>
		/// Format which must be used for coloring. If format is Custom then
		/// used FormatName property for format identification.
		/// </summary>
		[XmlIgnore]
		public FormatType Type
		{
			get
			{
				return FormatType.Text;
			}
		}
		/// <summary>
		/// FormatName which must be used for coloring.
		/// </summary>
		[XmlIgnore]
		public string FormatName
		{
			get
			{
				return string.Empty;
			}
		}
		/// <summary>
		/// Is BeginBlock property contains Regular expression or not.
		/// </summary>
		[XmlIgnore]
		public bool IsBeginRegex
		{
			get
			{
				return false;
			}
		}
		/// <summary>
		/// Is EndBlock property contains Regular expression or not.
		/// </summary>
		[XmlIgnore]
		public bool IsEndRegex
		{
			get
			{
				return false;
			}
		}
		/// <summary>
		/// Is ContinueBlock property contains Regular expression or not.
		/// </summary>
		[XmlIgnore]
		public bool IsContinueRegex
		{
			get
			{
				return false;
			}
		}
		/// <summary>
		/// this flag indicate must parser parse lexem internals or not. For
		/// complex constructions data between begin and end blocks can have own
		/// formats.
		/// </summary>
		[XmlIgnore]
		public bool IsComplex
		{
			get
			{
				return true;
			}
		}
		/// <summary>
		/// List of sub lexems.
		/// </summary>
		[XmlIgnore]
		public ArrayList SubLexems
		{
			get
			{
				return Lexems;
			}
		}
		/// <summary>
		/// List of references.
		/// </summary>
		[XmlIgnore]
		public ArrayList References
		{
			get
			{
				return m_emptyReferencesList;
			}
		}
		/// <summary>
		/// Parent config.
		/// </summary>
		[XmlIgnore]
		public IConfigLexem ParentConfig
		{
			get
			{
				return null;
			}
		}
		/// <summary>
		/// This flag indicates, whether parser should look for lexem`s config just in local array,
		/// or also look in parents
		/// </summary>
		[XmlIgnore]
		public bool OnlyLocalSublexems
		{
			get
			{
				return true;
			}
		}
		/// <summary>
		/// Checks string to the equalization to begin block.
		/// If begin block is regular expression, input string will be checked by RegExp
		/// </summary>
		/// <param name="str">String to be checked</param>
		/// <returns>True if it can be treated as begin block.</returns>
		public bool IsEqualToEnd( string str )
		{
			return false;
		}
		/// <summary>
		/// Checks string to the equalization to continue block.
		/// If continue block is regular expression, input string will be checked by RegExp
		/// </summary>
		/// <param name="str">String to be checked</param>
		/// <returns>True if it can be treated as continue block.</returns>
		public bool IsEqualToContinue( string str )
		{
			return false;
		}
		/// <summary>
		/// Checks string to the equalization to end block.
		/// If end block is regular expression, input string will be checked by RegExp
		/// </summary>
		/// <param name="str">String to be checked</param>
		/// <returns>True if it can be treated as end block.</returns>
		public bool IsEqualToBegin( string str )
		{
			return false;
		}
		/// <summary>
		/// Searches for configs in sub-lexems.
		/// Current lexem config is not tested for equalization.
		/// If config was not found in sub-lexems,
		/// it will be searched in parent.
		/// </summary>
		/// <param name="str">String to find.</param>
		/// <returns>List of config lexems.</returns>
		public IList FindConfigs( string str )
		{
			if( m_keeper == null )
				m_keeper = new LexemConfigsKeeper( this );

			return m_keeper.GetConfigs( str );
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
					m_format = ( Type != FormatType.Custom ) ?
						this[ Type ] : this[ FormatName ];
				}

				return m_format;
			}
		}
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
				return this;
			}
		}
		/// <summary>
		/// Condition, needed to pass check. Format: name=ON|OFF
		/// </summary>
		[XmlIgnore]
		public string Condition
		{
			get
			{
				return string.Empty;
			}
			set { }
		}
		/// <summary>
		/// Static unique ID of configuration node.
		/// </summary>
		[XmlIgnore]
		public int ID
		{
			get
			{
				return 0;
			}
		}
		/// <summary>
		/// Language, lexem belongs to.
		/// </summary>
		IConfigLanguage IConfigLexem.Language
		{
			get
			{
				return this;
			}
		}
		/// <summary>
		/// Gets or sets sign whether the end-block is just the way to exit higher by stack, or it is real ending of lexem.
		/// </summary>
		[XmlIgnore]
		public bool IsPseudoEnd
		{
			get
			{
				return false;
			}
		}
		/// <summary>
		/// Gets sign of auto-indenting after lexem with such config.
		/// </summary>
		[XmlIgnore]
		public bool Indent
		{
			get
			{
				return false;
			}
		}
		/// <summary>
		/// Gets ID of the lexem configuration, that follows right after current 
		/// one is parsed. Such lexem must be complex and "OnlyLocalSublexems",
		/// without beginblock and with endblock.
		/// </summary>
		[XmlIgnore]
		public int NextID
		{
			get
			{
				return -1;
			}
		}
		/// <summary>
		/// Gets sign of dropping down context choice list after entering text of the current lexem.
		/// </summary>
		[XmlIgnore]
		public bool DropContextChoiceList
		{
			get
			{
				return false;
			}
		}
		/// <summary>
		/// Gets value, that shows whether context prompt should be shown after typing text of the current lexem.
		/// </summary>
		[XmlIgnore]
		public bool DropContextPrompt
		{
			get
			{
				return false;
			}
		}
		/// <summary>
		/// Gets whether content divider should be shown below lexem.
		/// </summary>
		[XmlIgnore]
		public bool ContentDivider
		{
			get
			{
				return false;
			}
		}
		/// <summary>
		/// Gets whether IndentationGuideline should be shown.
		/// </summary>
		[XmlIgnore]
		public bool IndentationGuideline
		{
			get
			{
				return false;
			}
		}
		/// <summary>
		/// Gets value indicating if lexem should be used if there are more than one config found on one priority level.
		/// </summary>
		[XmlIgnore]
		public bool DefaultInGroup
		{
			get
			{
				return true;
			}
		}
		/// <summary>
		/// Gets value indicating if custom control should be used instead of the simple lexem rendering.
		/// </summary>
		[XmlIgnore]
		public bool UseCustomControl
		{
			get
			{
				return false;
			}
		}
		/// <summary>
		/// Gets value indicating whether triggers can be used.
		/// </summary>
		[XmlIgnore]
		public bool AllowTriggers
		{
			get
			{
				return true;
			}
		}
		#endregion
	}
}