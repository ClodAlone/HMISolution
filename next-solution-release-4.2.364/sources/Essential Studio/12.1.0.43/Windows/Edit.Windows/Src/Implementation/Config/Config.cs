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
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Diagnostics;
using System.Security.Permissions;

using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Enums;

namespace Syncfusion.Windows.Forms.Edit.Implementation.Config
{
	#region *** ConfigStringsData
	/// <summary>
	/// Structure for storing Xml data divided into independent strings.
	/// </summary>
	internal struct ConfigStringsData
	{
		/// <summary>
		/// Xml code for languages.
		/// </summary>
		public string Languages;
		/// <summary>
		/// Xml code for macros.
		/// </summary>
		public string Macros;
	}
	#endregion

	#region *** Config
	/// <summary>
	/// Class responsible for reading xml config, creating array of ConfigLanguage.
	/// (ConfigLanguage - object which describes concrete programming language: how to parse current source).
	/// </summary>
	public class Config
		: IConfig
		, IDisposable
	{
		#region Constants
		/// <summary>
		/// Reference on default config file provided with control as embedded resource.
		/// </summary>
		private const string DEF_CONFIG1 = "config.xml";
		/// <summary>
		/// Default language name.
		/// </summary>
		public const string DEF_DEFAULT_LANGUAGE_NAME = "default_language";
		/// <summary>
		/// Path to default configuration file.
		/// </summary>
		internal const string DEF_CONF_FILE = "Syncfusion.Windows.Forms.Edit.config.xml";
		/// <summary>
		/// Defines whether regular expresisons should be compiled.
		/// </summary>
		internal const RegexOptions DEF_COMPILED_REGEX = RegexOptions.Compiled;
		#endregion

		#region Static Members
		/// <summary>
		/// Stream of default configuration file.
		/// </summary>
		private static Stream _configStream = null;
		/// <summary>
		/// Design mode.
		/// </summary>
		private bool m_InDesign;
		/// <summary>
		/// Regex for rerieving macros from config file.
		/// </summary>
		private static Regex macroRegex = new Regex( @"\<(ArrayOfMacro)s?\>", DEF_COMPILED_REGEX );
		/// <summary>
		/// Default configuration languages.
		/// </summary>
		private static ConfigLanguage[] _languages = null;
		/// <summary>
		/// Last index of the configurator.
		/// </summary>
		private static int _index = 0;
		#endregion

		#region Fields
		/// <summary>
		/// Config file name which we read from.
		/// </summary>
		private string m_strFileName = string.Empty;
		/// <summary>
		/// Storage for array of IConfigLanguage.
		/// </summary>
		private ArrayList m_arrCL = new ArrayList();
#pragma warning disable
        /// <summary>
		/// Return IConfigLanguage by it's name (like "Delphi").
		/// </summary>
		private Hashtable m_hashLanguages = new Hashtable(new CaseInsensitiveHashCodeProvider(), new CaseInsensitiveComparer() );
        /// <summary>
		/// Return IConfigLanguage by extension
		/// (like .pas returns Delphi IConfigLanguage, .cpp or .c - CPP IConfigLanguage).
		/// </summary>
		private Hashtable m_hashExtentsion = new Hashtable( new CaseInsensitiveHashCodeProvider(), new CaseInsensitiveComparer() );
#pragma warning enable
        /// <summary>
		/// XML Serialization provider.
		/// </summary>
		private XmlSerializer m_serializer;
		/// <summary>
		/// Configurator index.
		/// </summary>
		private int m_index = 0;
		/// <summary>
		/// Manager of macros.
		/// </summary>
		private static MacrosManager m_macrosManager;
		#endregion

		#region Events
		/// <summary>
		/// Event, that is raised when 
		/// </summary>
		public event EventHandler ConfigurationChanged;
		/// <summary>
		/// Event that is raised when formats configuration has changed.
		/// </summary>
		public event EventHandler FormatsChanged;
		#endregion

		#region Static Properties
		/// <summary>
		/// Gets stream of default configuration document.
		/// </summary>
		public static Stream DefConfigStream
		{
			get
			{
				if( _configStream == null )
				{
					_configStream = Assembly.GetExecutingAssembly().GetManifestResourceStream( DEF_CONF_FILE );
				}
				_configStream.Position = 0;

				return _configStream;
			}
		}
		/// <summary>
		/// Gets currently used macros.
		/// </summary>
		internal MacrosManager MacrosManager
		{
			get
			{
				return m_macrosManager;
			}
		}
		#endregion

		#region Access Configuration Languages Objects
		/// <summary>
		/// Gets configuration for language by index.
		/// </summary>
		public IConfigLanguage this[ int iIndex ]
		{
			get
			{
				if( null == m_arrCL ) throw new NullReferenceException(
					Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_105 );

				if( iIndex > m_arrCL.Count - 1 ) throw new IndexOutOfRangeException(
					Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_106 );

				return ( IConfigLanguage )m_arrCL[ iIndex ];
			}
		}
		/// <summary>
		/// Gets configuration for language by specified name
		/// </summary>
		public IConfigLanguage this[ string name ]
		{
			get
			{
				if( name == null )
					throw new ArgumentNullException( "name" );

				if( name.Length == 0 )
					throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_14 );

				m_hashLanguages.ToString();

				if( !m_hashLanguages.ContainsKey( name ) )
				{
					string langList = "";
					foreach( object s in this.KnownLanguageNames )
					{
						langList += s.ToString() + ";";
					}

					throw new ArgumentNullException(
						"name", String.Format( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_107, name )
						+ " Known Languages " + langList );
				}

				return ( IConfigLanguage )m_hashLanguages[ name ];
			}
		}
		/// <summary>
		/// Gets list of known to configuration language names.
		/// </summary>
		public ArrayList KnownLanguageNames
		{
			get
			{
				return new ArrayList( m_hashLanguages.Keys );
			}
		}
		/// <summary>
		/// Gets list of known languages.
		/// </summary>
		public ArrayList KnownLanguages
		{
			get
			{
				return new ArrayList( m_hashLanguages.Values );
			}
		}
		/// <summary>
		/// Creates new language configuration and adds it to the configurations list.
		/// </summary>
		/// <param name="name">Name of the new language.</param>
		/// <returns>New instance of language configuration.</returns>
		public IConfigLanguage CreateLanguageConfiguration( string name )
		{
			if( null == name || string.Empty == name ) throw new ArgumentException( "Name should be non-empty string", "name" );
			if( m_hashLanguages.Contains( name ) ) throw new ArgumentException( "Language with the specified name already exists.", "name" );

			IConfigLanguage language = new ConfigLanguage( name );
			Add( language, DuplicatesOptions.DuplicatesNotAllowed );
			return language;
		}
		/// <summary>
		/// Detect language by file name extension.
		/// </summary>
		/// <param name="extension">File extension.</param>
		/// <returns> IConfigLanguage - config obj for parsing. </returns>
		public IConfigLanguage GetLanguage( string extension )
		{
			if( extension == null ) throw new ArgumentNullException( "extension" );
			if( extension.Length == 0 )
				throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_108 );

			// TODO: Remove this quick fix later.
			CreateLangHash();

			if( !m_hashExtentsion.Contains( extension ) ) return null;

			return ( IConfigLanguage )m_hashExtentsion[ extension ];
		}
		/// <summary>
		/// Gets default language configuration that is stored in embedded resource.
		/// </summary>
		public IConfigLanguage DefaultLanguage
		{
			get
			{
				AppendDefaultLanguage();

				return this[ DEF_DEFAULT_LANGUAGE_NAME ];
			}
		}
		#endregion

		#region Initialization & Finalization
		/// <summary>
		/// Loads default macroses.
		/// </summary>
		static Config()
		{
			InitMacros();
		}
		/// <summary>
		/// Creates and initializes new instance of the class.
		/// </summary>
		public Config()
			: this( false )
		{
		}
		/// <summary>
		/// Creates and initializes new instance of the class.
		/// </summary>
		/// <param name="design">Indicates whether underlying control is in design mode.</param>
		public Config( bool design )
			: this( design, true )
		{
		}
		/// <summary>
		/// Creates and initializes new instance of Config.
		/// </summary>
		/// <param name="bDesign">Indicates whether underlying control is in design mode.</param>
		/// <param name="bLoadConfigFile">Indicates whether config file should be loaded.</param>
		public Config( bool bDesign, bool bLoadConfigFile )
		{
			InitMacros();

			m_serializer = ( bDesign )
				? null
				: Syncfusion.XmlSerializersCreator.SerializersManager.GetSerializer( typeof( ConfigLanguage[] ) );

			m_index = ++_index;

			if( bLoadConfigFile )
			{
				m_InDesign = bDesign;
				m_strFileName = "none";
				Reset();
			}
			else
			{
				if (this.DefaultLanguage.Language != "default_language")
				{
				m_arrCL.Clear();
				AddDefaultLanguageFromCode();
				}
			}
		}
		/// <summary>
		/// Creates config using file name.
		/// </summary>
		/// <param name="strFileName">Xml file name.</param>
		public Config( string strFileName )
			: this( false )
		{
			if( strFileName == null ) throw new ArgumentNullException( "strFileName" );
			if( strFileName.Length == 0 )
				throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_109 );

			Open( strFileName );
		}
		/// <summary>
		/// Creates config using stream.
		/// </summary>
		/// <param name="cfgStream">Stream which filled with data.</param>
		public Config( Stream cfgStream )
			: this( false )
		{
			if( null == cfgStream ) throw new ArgumentNullException( "cfgStream" );

			Open( cfgStream );
		}
		/// <summary>
		/// Creates config using xmlDocument.
		/// </summary>
		/// <param name="xmlDoc">Obj which represents xml file.</param>
		public Config( XmlDocument xmlDoc )
			: this( false )
		{
			if( null == xmlDoc ) throw new ArgumentNullException( "xmlDoc" );

			Open( xmlDoc );
		}
		/// <summary>
		/// Frees resources.
		/// </summary>
		public void Dispose()
		{
			foreach( ConfigLanguage language in m_arrCL )
			{
				language.OnChanged -= new EventHandler( language_OnChanged );
				language.LanguageChanged -= new ValueChangedEventHandler( language_LanguageChanged );
			}
		}
		#endregion

		#region Open/Append/Save Config File Operations
		/// <summary>
		/// Open config file by  path.
		/// </summary>
		/// <param name="configFile">File name of config.</param>
		public void Open( string configFile )
		{
			if( configFile == null ) throw new ArgumentNullException( "configFile" );
			if( configFile.Length == 0 )
				throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_110 );

			using( FileStream file = new FileStream( configFile, FileMode.Open, FileAccess.Read, FileShare.Read ) )
			{
				Open( file );
				file.Close();
			}
		}
		/// <summary>
		/// Reads config from stream.
		/// </summary>
		/// <param name="configFile">stream which contains configuration for
		/// parsers.</param>
		public void Open( Stream configFile )
		{
			if( null == configFile ) throw new ArgumentNullException( "configFile" );

			if( m_serializer == null ) return;

			m_hashLanguages.Clear();
			m_hashExtentsion.Clear();
			m_arrCL.Clear();

			using( StreamReader reader = new StreamReader( configFile ) )
			{
				ConfigStringsData data = GetConfigStringsData( reader.ReadToEnd() );
				UpdateDataFromConfig( data, DuplicatesOptions.SkipDuplicates );
			}

			OnConfigurationChanged();
		}
		/// <summary>
		/// XML Document which contains configuration rules for parsers.
		/// </summary>
		/// <param name="configFile">XML Document.</param>
		public void Open( XmlDocument configFile )
		{
			if( null == configFile ) throw new ArgumentNullException( "configFile" );

			if( m_serializer == null ) return;

			m_arrCL.Clear();
			m_hashLanguages.Clear();
			m_hashExtentsion.Clear();

			using( StringReader sr = new StringReader( configFile.OuterXml ) )
			{
				ConfigStringsData data = GetConfigStringsData( sr.ReadToEnd() );
				UpdateDataFromConfig( data, DuplicatesOptions.SkipDuplicates );
			}

			OnConfigurationChanged();
		}
		/// <summary>
		/// If config file divided by user on many small one for each language then this methods will help him to load them in one call.
		/// </summary>
		/// <param name="configFile">File path to config file.</param>
		public void Append( string configFile )
		{
			Append( configFile, DuplicatesOptions.OverwriteDuplicates );
		}
		/// <summary>
		/// If config file divided by user on many small one for each language then this methods will help him to load them in one call.
		/// </summary>
		/// <param name="configFile">File path to config file.</param>
		/// <param name="duplicatesProcessing">Application`s behaviour on duplicates occurance.</param>
		public void Append( string configFile, DuplicatesOptions duplicatesProcessing )
		{
			if( configFile == null ) throw new ArgumentNullException( "configFile" );
			if( configFile.Length == 0 )
				throw new ArgumentNullException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_110 );

            using (FileStream fs = new FileStream(configFile, FileMode.Open, FileAccess.Read))
			{
				Append( fs, duplicatesProcessing );
				fs.Close();
			}

			CreateLangHash();
			OnConfigurationChanged();
		}
		/// <summary>
		/// If config file divided by user on many small one for each language then this methods will help him to load them in one call.
		/// </summary>
		/// <param name="configStream">Stream with XML config</param>
		public void Append( Stream configStream )
		{
			Append( configStream, DuplicatesOptions.OverwriteDuplicates );
		}
		/// <summary>
		/// If config file divided by user on many small one for each language then this methods will help him to load them in one call.
		/// </summary>
		/// <param name="configStream">Stream with XML config</param>
		/// <param name="duplicatesProcessing">Application's behaviour on duplicates occurance.</param>
		public void Append( Stream configStream, DuplicatesOptions duplicatesProcessing )
		{
			if( null == configStream ) throw new ArgumentNullException( "configStream" );
			if( duplicatesProcessing == DuplicatesOptions.MergeDuplicates )
				throw new NotImplementedException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_111 );

			if( m_serializer == null ) return;

			StreamReader reader = new StreamReader( configStream );
			ConfigStringsData data = GetConfigStringsData( reader.ReadToEnd() );
			UpdateDataFromConfig( data, duplicatesProcessing );
			reader.DiscardBufferedData();
		}
		/// <summary>
		/// If config file divided by user on many small one for each language then this methods will help him to load them in one call.
		/// </summary>
		/// <param name="configXml">XML document which contains formatting data</param>
		public void Append( XmlDocument configXml )
		{
			Append( configXml, DuplicatesOptions.OverwriteDuplicates );
		}
		/// <summary>
		/// If config file divided by user on many small one for each language then this methods will help him to load them in one call.
		/// </summary>
		/// <param name="configXml">XML document which contains formatting data</param>
		/// <param name="duplicatesProcessing">Application's behaviour on duplicates occurance.</param>
		public void Append( XmlDocument configXml, DuplicatesOptions duplicatesProcessing )
		{
			if( null == configXml ) throw new ArgumentNullException( "configXml" );

			if( m_serializer == null ) return;

			using( StringReader sr = new StringReader( configXml.OuterXml ) )
			{
				ConfigStringsData data = GetConfigStringsData( sr.ReadToEnd() );
				UpdateDataFromConfig( data, duplicatesProcessing );
			}
		}
		/// <summary>
		/// If config file divided by user on many small one for each language then this methods will help him to load them in one call.
		/// </summary>
		/// <param name="configFile">File path to config file.</param>
		[Obsolete( "This method has been renamed. It's new name is Append." )]
		public void AppendConfig( string configFile )
		{
			Append( configFile );
		}
		/// <summary>
		/// If config file divided by user on many small one for each language then this methods will help him to load them in one call.
		/// </summary>
		/// <param name="configFile">file path to config file</param>
		/// <param name="duplicatesProcessing">Application`s behaviour on duplicates occurance.</param>
		[Obsolete( "This method has been renamed. It's new name is Append." )]
		public void AppendConfig( string configFile, DuplicatesOptions duplicatesProcessing )
		{
			Append( configFile, duplicatesProcessing );
		}
		/// <summary>
		/// If config file divided by user on many small one for each language then this methods will help him to load them in one call.
		/// </summary>
		/// <param name="configStream">Stream with XML config</param>
		[Obsolete( "This method has been renamed. It's new name is Append." )]
		public void AppendConfig( Stream configStream )
		{
			Append( configStream );
		}
		/// <summary>
		/// If config file divided by user on many small one for each language then this methods will help him to load them in one call.
		/// </summary>
		/// <param name="configStream">Stream with XML config</param>
		/// <param name="duplicatesProcessing">Application`s behaviour on duplicates occurance.</param>
		[Obsolete( "This method has been renamed. It's new name is Append." )]
		public void AppendConfig( Stream configStream, DuplicatesOptions duplicatesProcessing )
		{
			Append( configStream, duplicatesProcessing );
		}
		/// <summary>
		/// If config file divided by user on many small one for each language then this methods will help him to load them in one call.
		/// </summary>
		/// <param name="configXml">XML document which contains formatting data</param>
		[Obsolete( "This method has been renamed. It's new name is Append." )]
		public void AppendConfig( XmlDocument configXml )
		{
			Append( configXml );
		}
		/// <summary>
		/// If config file divided by user on many small one for each language then this methods will help him to load them in one call.
		/// </summary>
		/// <param name="configXml">XML document which contains formatting data</param>
		/// <param name="duplicatesProcessing">Application's behaviour on duplicates occurance.</param>
		[Obsolete( "This method has been renamed. It's new name is Append." )]
		public void AppendConfig( XmlDocument configXml, DuplicatesOptions duplicatesProcessing )
		{
			Append( configXml, duplicatesProcessing );
		}
		/// <summary>
		/// Resets configuration languages list and loads default configuration.
		/// </summary>
		public void Reset()
		{
			m_arrCL.Clear();
			AppendDefaultLanguage();
		}
		/// <summary>
		/// Saves current configuration to file.
		/// </summary>
		/// <param name="fileName">Output file name.</param>
		public void Save( string fileName )
		{
			if( fileName == null ) throw new ArgumentNullException( "fileName" );
			if( 0 == fileName.Length )
				throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_68 );

			using( Stream writer = File.Create( fileName ) )
			{
				Save( writer );
				writer.Close();
			}
		}
		/// <summary>
		/// Save configuration to output stream.
		/// </summary>
		/// <param name="configStream">Stream for config saving.</param>
		public void Save( Stream configStream )
		{
			if( null == configStream ) throw new ArgumentNullException( "configStream" );

			m_serializer.Serialize( configStream, ( ConfigLanguage[] )m_arrCL.ToArray( typeof( ConfigLanguage ) ) );
		}
		/// <summary>
		/// Save configuration to XML Document.
		/// </summary>
		/// <param name="configXml">XML document to which data must be saved.</param>
		public void Save( XmlDocument configXml )
		{
			if( null == configXml ) throw new ArgumentNullException( "configXml" );

			using( StringWriter writer = new StringWriter() )
			{
				m_serializer.Serialize( writer, ( ConfigLanguage[] )m_arrCL.ToArray( typeof( ConfigLanguage ) ) );
				configXml.LoadXml( writer.ToString() );

				writer.Close();
			}
		}
		#endregion

		#region Utility Methods
		/// <summary>
		/// Adds configuration language to config.
		/// </summary>
		/// <param name="language">Language for adding.</param>
		/// <param name="duplicatesProcessing">Specifies how to process duplicates.</param>
		internal protected void Add( IConfigLanguage language, DuplicatesOptions duplicatesProcessing )
		{
			if( language == null ) throw new ArgumentNullException( "language" );

			ProcessAndAppend( new ConfigLanguage[] { ( ConfigLanguage )language }, duplicatesProcessing );
		}
		/// <summary>
		/// Appends languages to the configuration.
		/// </summary>
		/// <param name="languages">Array of languages to append.</param>
		/// <param name="duplicatesProcessing">Specifies how to process duplicates.</param>
		internal protected void ProcessAndAppend( ConfigLanguage[] languages, DuplicatesOptions duplicatesProcessing )
		{
			if( languages == null ) throw new ArgumentNullException( "languages" );

			bool bHashRebuildNeeded = false;

			for( int iIndex = 0; iIndex < languages.Length; ++iIndex )
			{
				ConfigLanguage language = languages[ iIndex ];
				string strCurLang = language.Language;
				language.MacrosManager = m_macrosManager;

				if( m_hashLanguages.ContainsKey( strCurLang ) )
				{
					switch( duplicatesProcessing )
					{
						case DuplicatesOptions.DuplicatesNotAllowed:
							throw new ApplicationException( "Language Conflict Found : Already Have Info About " + strCurLang );

						case DuplicatesOptions.OverwriteDuplicates:
							( m_hashLanguages[ strCurLang ] as ConfigLanguage ).Dispose();
							m_arrCL.Remove( m_hashLanguages[ strCurLang ] );
							m_arrCL.Add( language );
							bHashRebuildNeeded = true;
							break;

						case DuplicatesOptions.SkipDuplicates:
							break;

						case DuplicatesOptions.MergeDuplicates:
							throw new NotImplementedException(
								Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_112 );
					}
				}
				else
				{
					m_arrCL.Add( language );
					bHashRebuildNeeded = true;
				}
			}

			if( bHashRebuildNeeded )
				CreateLangHash();

			OnConfigurationChanged();
		}
		/// <summary>
		/// Util method for config hashes sync.
		/// </summary>
		private void CreateLangHash()
		{
			m_hashLanguages.Clear();
			m_hashExtentsion.Clear();

			for( int iIndex = 0; iIndex < m_arrCL.Count; ++iIndex )
			{
				ConfigLanguage clValue = ( ( ConfigLanguage )m_arrCL[ iIndex ] );
				string strKey = clValue.Language;
				m_hashLanguages.Add( strKey, clValue );

				foreach( string extention in clValue.Extensions )
				{
					ConfigLanguage languageToAdd = clValue;

					if( m_hashExtentsion.Contains( extention ) )
					{
						ConfigLanguage lang = ( ConfigLanguage )m_hashExtentsion[ extention ];

						if( lang.LanguageIndex > clValue.LanguageIndex )
							languageToAdd = lang;
					}

					m_hashExtentsion[ extention ] = languageToAdd;
				}
			}
		}
		/// <summary>
		/// Converts class object to string.
		/// </summary>
		/// <returns>String representation of config.</returns>
		public override string ToString()
		{
			string strResult = string.Empty;

			for( int iIndex = 0; iIndex < m_arrCL.Count; ++iIndex )
				strResult += m_arrCL[ iIndex ].ToString();
			return strResult;
		}
		/// <summary>
		/// Appends default language from embedded resource.
		/// </summary>
		protected void AppendDefaultLanguage()
		{
			bool bFirst = m_arrCL.Count == 0;

			if( !m_hashLanguages.Contains( DEF_DEFAULT_LANGUAGE_NAME ) )
			{
				if( !m_InDesign )
				{
					if( _languages == null )
					{
						Append( DefConfigStream, DuplicatesOptions.SkipDuplicates );

						if( bFirst )
						{
							_languages = new ConfigLanguage[ m_hashLanguages.Count ];
							m_hashLanguages.Values.CopyTo( _languages, 0 );
						}
					}
					else
					{
						ProcessAndAppend( _languages, DuplicatesOptions.SkipDuplicates );
					}
				}
				else
				{
					AddDefaultLanguageFromCode();
				}
			}
		}
		/// <summary>
		/// Removes language from collection.
		/// </summary>
		/// <param name="language">Language which must be removed.</param>
		/// <returns>TRUE - if language deleted, FALSE - otherwise.</returns>
		internal protected bool Remove( IConfigLanguage language )
		{
			if( language == null )
				throw new ArgumentNullException( "language" );

			if( language == this.DefaultLanguage ) return false;

			if( m_arrCL.Contains( language ) )
			{
				m_arrCL.Remove( language );

				CreateLangHash();

				return true;
			}

			return false;
		}
		/// <summary>
		/// Raises ConfigurationChanged event.
		/// </summary>
		protected void OnConfigurationChanged()
		{
			BindToLanguageEvents();
			if( ConfigurationChanged != null )
				ConfigurationChanged( this, EventArgs.Empty );
		}
		/// <summary>
		/// Binds events.
		/// </summary>
		protected void BindToLanguageEvents()
		{
			foreach( ConfigLanguage language in m_arrCL )
			{
				language.OnChanged -= new EventHandler( language_OnChanged );
				language.OnChanged += new EventHandler( language_OnChanged );
				language.LanguageChanged -= new ValueChangedEventHandler( language_LanguageChanged );
				language.LanguageChanged += new ValueChangedEventHandler( language_LanguageChanged );
			}
		}
		/// <summary>
		/// Returns ConfigStringsData structure with Xml code divided into different sections.
		/// </summary>
		/// <param name="strData">String with XML source with data.</param>
		/// <returns>ConfigStringsData structure with Xml code divided into different sections.</returns>
		private ConfigStringsData GetConfigStringsData( string strData )
		{
			Match match = macroRegex.Match( strData );
			ConfigStringsData result = new ConfigStringsData();

			if( match.Success )
			{
				int macrosIndex = match.Index;

				result.Macros = strData.Substring( macrosIndex, strData.Length - macrosIndex );
				result.Macros = result.Macros.Replace( "ArrayOfMacros>", "ArrayOfMacro>" );

				strData = strData.Remove( macrosIndex, strData.Length - macrosIndex );
			}

			result.Languages = strData;

			return result;
		}
		/// <summary>
		/// Updates all needed data from ConfigStringsData structure.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="duplProcessing"></param>
		[System.Security.Permissions.PermissionSet( System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust" )]
		private void UpdateDataFromConfig( ConfigStringsData data, DuplicatesOptions duplProcessing )
		{
			if( null != data.Languages )
			{
				XmlTextReader xmlReader = new XmlTextReader( new StringReader( data.Languages ) );
				xmlReader.WhitespaceHandling = WhitespaceHandling.Significant;
				xmlReader.Namespaces = false;

				ConfigLanguage[] languages =
					( ConfigLanguage[] )m_serializer.Deserialize( xmlReader );

				if( null != languages )
				{
					ProcessAndAppend( languages, duplProcessing );
				}
			}

			if( null != data.Macros )
			{
				m_macrosManager.AppendMacros( new XmlTextReader( new StringReader( data.Macros ) ) );
			}
		}
		/// <summary>
		/// Adds default language to the collection.
		/// </summary>
		private void AddDefaultLanguageFromCode()
		{
			ConfigLanguage[] languages = new ConfigLanguage[ 1 ];
			languages[ 0 ] = new ConfigLanguage( DEF_DEFAULT_LANGUAGE_NAME );
			ProcessAndAppend( languages, DuplicatesOptions.SkipDuplicates );
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Raises FormatsChanged event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void language_OnChanged( object sender, EventArgs e )
		{
			if( null != FormatsChanged )
			{
				FormatsChanged( sender, e );
			}
		}
		/// <summary>
		/// Raises configuration changed event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void language_LanguageChanged( object sender, ValueChangedEventArgs e )
		{
			OnConfigurationChanged();
		}
		#endregion

		#region Static Methods
		/// <summary>
		/// Clears static collection of default languages.
		/// </summary>
		public static void ClearStaticData()
		{
			_languages = null;
			m_macrosManager = null;
		}
		/// <summary>
		/// Initializes macros.
		/// </summary>
		private static void InitMacros()
		{
			if( m_macrosManager == null )
			{
				m_macrosManager = new MacrosManager();

				try
				{
					m_macrosManager.LoadDefault();
				}
				catch( InvalidOperationException )
				{
					Debug.WriteLine( "Failed to load default macroses." );
				}
			}
		}
		#endregion
	}
	#endregion
}