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
using System.ComponentModel;
using System.Resources;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Diagnostics;
using System.Reflection;
using System.Collections;

namespace Syncfusion.Windows.Forms.Localization
{
	/// <exclude/>
	/// <summary>
	/// Specifies the name of the localizable item.
	/// </summary>
	[AttributeUsage( AttributeTargets.Field, AllowMultiple = false, Inherited = true )]
	public sealed class StrLocalized
		: Attribute
	{
		#region Private Class Members
		/// <summary>
		/// Name of the localizable item.
		/// </summary>
		private string m_name;
		#endregion

		#region Class Properties
		/// <summary>
		/// Gets or sets name of the localizable item.
		/// </summary>
		public string Name
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

		#endregion

		#region Class Initialization/Finalization
		/// <summary>
		/// Creates and initializes attribute.
		/// </summary>
		/// <param name="localizedStringName">Name of the localizable item.</param>
		public StrLocalized( string localizedStringName )
		{
			m_name = localizedStringName;
		}
		#endregion
	}

	/// <summary>
	/// Localizer provides localized access to string resources specific
	/// from the assembly manifest Syncfusion.Windows.Forms.Localization.SR.resources.
	/// Can be used from any assembly, resources will be loaded from the calling assembly.
	/// </summary>
	public class Localizer
	{
		#region Class Constants
		/// <summary>
		/// Localizable resources namespace.
		/// </summary>
		private const string DEF_RESOURCE_BASENAME_ENDING_REGEX = @"(?<basename>.*Localization\.SR(\..*)*)\.resources";
		#endregion

		#region Class Private Members
		/// <summary>
		/// Resources manager.
		/// </summary>
		private ResourceManager resources;
		#endregion

		#region Class Private Static Members
		/// <summary>
		/// List of items that where not localized at all.
		/// </summary>
		private static ArrayList m_nonLocalizedNames = new ArrayList();
		/// <summary>
		/// List of items that where not localized for current culture.
		/// </summary>
		private static ArrayList m_nonLocalizedForCurrentCulture = new ArrayList();
		/// <summary>
		/// Hashtable of localizers for different assemblies.
		/// Key - assembly name, Value - localizer.
		/// </summary>
		private static Hashtable m_assemblies = new Hashtable();
		/// <summary>
		/// 
		/// </summary>
		private static Regex m_regexAssemblyName
			= new Regex( @"^[^,]*(?=,)", Syncfusion.Windows.Forms.Edit.Implementation.Config.Config.DEF_COMPILED_REGEX );
		/// <summary>
		/// 
		/// </summary>
		private static Regex _besenameEndingRegex = new Regex( DEF_RESOURCE_BASENAME_ENDING_REGEX, RegexOptions.IgnoreCase | RegexOptions.Compiled );
		#endregion

		#region Class Static Localizable Members
		/// <summary>
		/// ID of the localizable string with "{0} of {1}" format.
		/// </summary>
		[StrLocalized( "ContextPromptIndexFormat" )]
		public static string DEF_PROMPT_INDEX;
		///<exclude/>
		[StrLocalized( "errArgEnumExpected" )]
		public static string DEF_ERROR_ENUMEXPECTED;
		///<exclude/>
		[StrLocalized( "errEnumValueNotDefined" )]
		public static string DEF_ERROR_ENUM_VALUE_NOT_DEFINED;
		///<exclude/>
		[StrLocalized( "colorEmpty" )]
		public static string DEF_COLOR_EMPTY;
		///<exclude/>
		[StrLocalized( "noneHatch" )]
		public static string DEF_NONE_HATCH;
		///<exclude/>
		[StrLocalized( "backGroundSolidFill" )]
		public static string DEF_SOLID_FILL;
		///<exclude/>
		[StrLocalized( "errInvalidLocalizedEnumValueName" )]
		public static string DEF_ERROR_INVALID_ENUM_VALUE_NAME;
		///<exclude/>
		[StrLocalized( "LexemTreeRoot" )]
		public static string DEF_LEXEM_TREE_ROOT;
		///<exclude/>
		[StrLocalized( "FormatConfigSampleText" )]
		public static string DEF_FORMAT_SETTINGS_SAMPLE_TEXT;
		///<exclude/>
		[StrLocalized( "ConfigCreateLanguage" )]
		public static string DEF_CONFIG_CREATE_LANGUAGE;
		///<exclude/>
		[StrLocalized( "ConfigSaveOthersConfig" )]
		public static string DEF_CONFIG_SAVE_OTHERS_CHANGES;
		///<exclude/>
		[StrLocalized( "ConfigSaveChanges" )]
		public static string DEF_CONFIG_SAVE_CHANGES;
		///<exclude/>
		[StrLocalized( "errNotImplemented" )]
		public static string DEF_ERROR_NOTIMPLEMENTED;
		///<exclude/>
		[StrLocalized( "formatGoToDialogCaption" )]
		public static string DEF_GOTO_CAPTION_FORMAT = "Line Number {0}-{1}:";
		///<exclude/>
		[StrLocalized( "contextmenuEdit" )]
		public static string DEF_MENU_EDIT;
		///<exclude/>
		[StrLocalized( "contextmenuFile" )]
		public static string DEF_MENU_FILE;
		///<exclude/>
		[StrLocalized( "contextmenuAdvanced" )]
		public static string DEF_MENU_ADVANCED;
		///<exclude/>
		[StrLocalized( "contextmenuBookmarks" )]
		public static string DEF_MENU_BOOKMARKS;
		///<exclude/>
		[StrLocalized( "contextmenuOptions" )]
		public static string DEF_MENU_OPTIONS;
		///<exclude/>
		[StrLocalized( "contextmenuCut" )]
		public static string DEF_MENU_CUT;
		///<exclude/>
		[StrLocalized( "contextmenuCopy" )]
		public static string DEF_MENU_COPY;
		///<exclude/>
		[StrLocalized( "contextmenuPaste" )]
		public static string DEF_MENU_PASTE;
		///<exclude/>
		[StrLocalized( "contextmenuDelete" )]
		public static string DEF_MENU_DELETE;
		///<exclude/>
		[StrLocalized( "contextmenuUndo" )]
		public static string DEF_MENU_UNDO;
		///<exclude/>
		[StrLocalized( "contextmenuRedo" )]
		public static string DEF_MENU_REDO;
		///<exclude/>
		[StrLocalized( "contextmenuFind" )]
		public static string DEF_MENU_FIND;
		///<exclude/>
		[StrLocalized( "contextmenuReplace" )]
		public static string DEF_MENU_REPLACE;
		///<exclude/>
		[StrLocalized( "contextmenuGoto" )]
		public static string DEF_MENU_GOTO;
		///<exclude/>
		[StrLocalized( "contextmenuSelectAll" )]
		public static string DEF_MENU_SELECTALL;
		///<exclude/>
		[StrLocalized( "contextmenuDeleteAll" )]
		public static string DEF_MENU_DELETEALL;
		///<exclude/>
		[StrLocalized( "contextmenuNew" )]
		public static string DEF_MENU_NEW;
		///<exclude/>
		[StrLocalized( "contextmenuOpen" )]
		public static string DEF_MENU_OPEN;
		///<exclude/>
		[StrLocalized( "contextmenuClose" )]
		public static string DEF_MENU_CLOSE;
		///<exclude/>
		[StrLocalized( "contextmenuSave" )]
		public static string DEF_MENU_SAVE;
		///<exclude/>
		[StrLocalized( "contextmenuSaveAs" )]
		public static string DEF_MENU_SAVEAS;
		///<exclude/>
		[StrLocalized( "contextmenuPrintPreview" )]
		public static string DEF_MENU_PRINTPREVIEW;
		///<exclude/>
		[StrLocalized( "contextmenuPrint" )]
		public static string DEF_MENU_PRINT;
		///<exclude/>
		[StrLocalized( "contextmenuTabifySelection" )]
		public static string DEF_MENU_TABIFYSELECTION;
		///<exclude/>
		[StrLocalized( "contextmenuUntabifySelection" )]
		public static string DEF_MENU_UNTABIFYSELECTION;
		///<exclude/>
		[StrLocalized( "contextmenuIndentSelection" )]
		public static string DEF_MENU_INDENTSELECTION;
		///<exclude/>
		[StrLocalized( "contextmenuUnindentSelection" )]
		public static string DEF_MENU_UNINDENTSELECTION;
		///<exclude/>
		[StrLocalized( "contextmenuCommentSelection" )]
		public static string DEF_MENU_COMMENTSELECTION;
		///<exclude/>
		[StrLocalized( "contextmenuUncommentSelection" )]
		public static string DEF_MENU_UNCOMMENTSELECTION;
		///<exclude/>
		[StrLocalized( "contextmenuCollapseAll" )]
		public static string DEF_MENU_COLLAPSEALL;
		///<exclude/>
		[StrLocalized( "contextmenuExpandAll" )]
		public static string DEF_MENU_EXPANDALL;
		///<exclude/>
		[StrLocalized( "contextmenuToggleBookmark" )]
		public static string DEF_MENU_TOGGLEBOOKMARK;
		///<exclude/>
		[StrLocalized( "contextmenuNextBookmark" )]
		public static string DEF_MENU_NEXTBOOKMARK;
		///<exclude/>
		[StrLocalized( "contextmenuPrevBookmark" )]
		public static string DEF_MENU_PREVBOOKMARK;
		///<exclude/>
		[StrLocalized( "contextmenuClearBookmarks" )]
		public static string DEF_MENU_CLEARBOOKMARKS;
		///<exclude/>
		[StrLocalized( "msgLinuNumberingProblem" )]
		public static string DEF_MSG_LINE_NUMBERING_PROBLEMS = "There are some problems with line numbering.";
		///<exclude/>
		[StrLocalized( "msgSaveModified" )]
		public static string DEF_MSG_SAVE_MODIFIED = "File was changed, do you want to save changes?";
		///<exclude/>
		[StrLocalized( "msgSaveModifiedCaption" )]
		public static string DEF_MSG_SAVE_MODIFIED_CAPTION = "Save changes";
		///<exclude/>
		[StrLocalized( "msgKeyBindingsSetDefault" )]
		public static string DEF_MSG_KEYBINDINGS_SET_DEFAULT = "All your changes will be lost. Do you really want reset key bindings to defaults?";
		///<exclude/>
		[StrLocalized( "msgKeyBindingsSetDefaultCaption" )]
		public static string DEF_MSG_KEYBINDINGS_SET_DEFAULT_CAPTION = "Key bindings";
		///<exclude/>
		[StrLocalized( "msgFindNothingFound" )]
		public static string DEF_MSG_FOUND_NOTHING = "No text was found.";
		///<exclude/>
		[StrLocalized( "msgFindNothingFoundCaption" )]
		public static string DEF_MSG_FOUND_NOTHING_CAPTION = "Find text";
		///<exclude/>
		[StrLocalized( "msgXmlConfigLoadError" )]
		public static string DEF_MSG_XML_CONFIG_LOAD_ERROR = "Can't load defined XML document.";
		///<exclude/>
		[StrLocalized( "printPageNamuberPrefix" )]
		public static string DEF_PRINT_PAGE_PREFIX = "Page:";
		///<exclude/>
		[StrLocalized( "printDocumentNamePrefix" )]
		public static string DEF_PRINT_FILE_PREFIX = "File:";
		///<exclude/>
		[StrLocalized( "printDocumentUnnamed" )]
		public static string DEF_PRINT_NONAME_DOCUMENT = "New document";
		///<exclude/>
		[StrLocalized( "msgSaveFileUsingEncoding" )]
		public static string DEF_MSG_SAVE_FILE_USING_ENCODING = "If you save file using this encoding some data may be lost. Do you want to proceed?";
		///<exclude/>
		[StrLocalized( "msgSaveFileUsingEncodingCaption" )]
		public static string DEF_MSG_SAVE_FILE_USING_ENCODING_CAPTION = "Saving file using new encoding";
		///<exclude/>
		[StrLocalized( "msgSaveStreamUsingEncoding" )]
		public static string DEF_MSG_SAVE_STREAM_USING_ENCODING = "If you save stream using this encoding some data may be lost. Do you want to proceed?";
		///<exclude/>
		[StrLocalized( "msgSaveStreamUsingEncodingCaption" )]
		public static string DEF_MSG_SAVE_STREAM_USING_ENCODING_CAPTION = "Saving stream using new encoding";
		///<exclude/>
		[StrLocalized( "statusbarReadonly" )]
		public static string DEF_STATUSBAR_READONLY = "Read only";
		///<exclude/>
		[StrLocalized( "statusbarNotReadonly" )]
		public static string DEF_STATUSBAR_NOT_READONLY = "";
		///<exclude/>
		[StrLocalized( "statusbarOverwrite" )]
		public static string DEF_STATUSBAR_OVERWRITE = "OVR";
		///<exclude/>
		[StrLocalized( "statusbarInsert" )]
		public static string DEF_STATUSBAR_INSERT = "INS";
		///<exclude/>
		[StrLocalized( "statusbarPosition" )]
		public static string DEF_STATUSBAR_POSITION = "Line: {0,8}; Column: {1,8}";
		///<exclude/>
		[StrLocalized( "msgIncorrectRegex" )]
		public static string DEF_MSG_REGEX_INCORRECT = "The regular expression you have entered is incorrect.";
		///<exclude/>
		[StrLocalized( "msgIncorrectRegexCaption" )]
		public static string DEF_MSG_REGEX_INCORRECT_CAPTION = "Incorrect search request";
		///<exclude/>
		[StrLocalized( "optionsAppearance" )]
		public static string DEF_OPTS_APPEARANCE;
		///<exclude/>
		[StrLocalized( "optionsAppearanceAreas" )]
		public static string DEF_OPTS_APPEARANCEAREAS;
		///<exclude/>
		[StrLocalized( "optionsAppearanceText" )]
		public static string DEF_OPTS_APPEARANCETEXT;
		///<exclude/>
		[StrLocalized( "optionsAppearanceControl" )]
		public static string DEF_OPTS_APPEARANCECONTROL;
		///<exclude/>
		[StrLocalized( "optionsBehavior" )]
		public static string DEF_OPTS_BEHAVIOUR;
		///<exclude/>
		[StrLocalized( "optionsBehaviorGeneral" )]
		public static string DEF_OPTS_BEHAVIOURGENERAL;
		///<exclude/>
		[StrLocalized( "optionsBehaviorTabs" )]
		public static string DEF_OPTS_BEHAVIOURTABS;
		///<exclude/>
		[StrLocalized( "error" )]
		public static string ERROR;
		///<exclude/>
		[StrLocalized( "msgEncodingChanged" )]
		public static string DEF_MSG_ENCODING_CHANGED = "Encoding was changed due to insertion of unsupported symbols. Do you want to save using new encoding?";
        ///<exclude/>
        [StrLocalized("msgFindCompleteEventArgs")]
        public static string StringFindCompleteEventArgs = "Find reached the starting point of the search.";
		#endregion

		#region Class Initialilzatio/Finalization
		/// <summary>
		/// Statis constructor, calls LoadLocalizedMembers() method.
		/// </summary>
		static Localizer()
		{
			LocalizeFields( typeof( Localizer ), null );
		}
		/// <summary>
		/// Initializes localizer.
		/// </summary>
		/// <param name="assembly">Assembly, the localizer should be created for.</param>
		private Localizer( Assembly assembly )
		{
			if( assembly == null )
				throw new ArgumentNullException( "assembly" );

			this.resources = null;

			foreach( string name in assembly.GetManifestResourceNames() )
			{
				Match match = _besenameEndingRegex.Match( name );
				if( match.Success && match.Groups[ "basename" ].Success )
				{
					this.resources = new ResourceManager( match.Groups[ "basename" ].Value, assembly );
					break;
				}
			}
		}

		#endregion

		#region Class Static Helper methods
		/// <summary>
		/// Saves list of resources in file with the specified filename.
		/// </summary>
		/// <param name="assembly">Assembly with resources.</param>
		/// <param name="filename">File name.</param>
		protected static void DumpResources( Assembly assembly, string filename )
		{
			using( StreamWriter writer = new StreamWriter( string.Format( filename, assembly.FullName ) ) )
			{
				writer.WriteLine( assembly.FullName );
				writer.WriteLine( string.Empty );

				string[] resources = Assembly.GetCallingAssembly().GetManifestResourceNames();

				foreach( string name in resources )
					writer.WriteLine( name );
			}
		}

		/// <summary>
		/// Gets assembly-dependent localizer instance.
		/// </summary>
		/// <param name="assembly">Assembly, the localizer should be created for.</param>
		/// <returns></returns>
		private static Localizer GetLoader( Assembly assembly )
		{
			if( assembly == null )
				throw new ArgumentNullException( "assembly" );

			lock( typeof( Localizer ) )
			{
				string assemblyName = assembly.FullName;

				if( m_assemblies.Contains( assemblyName ) )
					return ( Localizer )m_assemblies[ assemblyName ];

				Localizer localizer = new Localizer( assembly );
				m_assemblies[ assemblyName ] = localizer;

				return localizer;
			}
		}
		/// <summary>
		/// Get localized version of the string.
		/// </summary>
		/// <param name="name">Name of the localized resource.</param>
		/// <param name="asm">Calling assembly.</param>
		/// <param name="culture">Culture to be used for getting localized version of the resource.</param>
		/// <returns>Localized version of the string.</returns>
		internal protected static string GetString( Assembly asm, CultureInfo culture, string name )
		{
			Localizer localizer = Localizer.GetLoader( asm );

			try
			{
                if (LocalizationProvider.Provider != null)
                {
                    String result = string.Empty;
                    result = LocalizationProvider.Provider.GetLocalizedString(culture, name, null);
                    if(result!=String.Empty)
                    	return result;
                }
				if( culture == null )
					culture = System.Threading.Thread.CurrentThread.CurrentUICulture;

				bool nonLocalizedInCulture = false;
				bool haveLocalizer = !( localizer == null || localizer.resources == null );

#if DEBUG
				if( haveLocalizer )
				{
					try
					{
						ResourceSet resourceSet =
							localizer.resources.GetResourceSet( culture, false, false );

						string locValue = ( null != resourceSet ) ? resourceSet.GetString( name ) : null;

						if( null == locValue || string.Empty == locValue )
							nonLocalizedInCulture = true;
					}
					catch
					{
						nonLocalizedInCulture = true;
					}
				}
#endif

				string str =
					( haveLocalizer )
					? localizer.resources.GetString( name, culture )
					: null;

				if( str == null )
				{
					string prefixedName = GetAssemblyName( asm ) + "." + name;

					foreach( Localizer loc in m_assemblies.Values )
					{
						if( null != loc.resources )
							str = loc.resources.GetString( prefixedName, culture );

						if( string.Empty != str && null != str ) break;
					}
				}

				nonLocalizedInCulture &= haveLocalizer;

				if( str == null )
				{
					ReportNonLocalized( asm, name, false );
				}
				else if( nonLocalizedInCulture )
				{
					ReportNonLocalized( asm, name, true );
				}

                if (str == null)
                    str = name;
				return str;
			}
			catch( Exception ex )
			{
				ReportNonLocalized( asm, name, false );
				Debug.WriteLine( ex.ToString() );
				return null;
			}
		}
		/// <summary>
		/// Gets short assembly name without version info, etc.
		/// </summary>
		/// <param name="asm">Assembly.</param>
		/// <returns>Short assembly name. Example: Syncfusion.Windows.Forms.Edit</returns>
		private static string GetAssemblyName( Assembly asm )
		{
			Match match = m_regexAssemblyName.Match( asm.FullName );

			if( match.Success ) return match.Value;

			return string.Empty;
		}
		/// <summary>
		/// Adds resource name to the list of non-localized items.
		/// </summary>
		/// <param name="asm">Assembly.</param>
		/// <param name="name">Item name that has not been localized.</param>
		/// <param name="justCurrentCulture">Specifies that item has native culture localization but does not has localization for current culture.</param>
		[Conditional( "DEBUG" )]
		internal static void ReportNonLocalized( Assembly asm, string name, bool justCurrentCulture )
		{
			name = "\"" + name + "\" from \"" + GetAssemblyName( asm ) + "\"";

			if( justCurrentCulture && !m_nonLocalizedForCurrentCulture.Contains( name ) )
				m_nonLocalizedForCurrentCulture.Add( name );

			if( !justCurrentCulture && !m_nonLocalizedNames.Contains( name ) )
				m_nonLocalizedNames.Add( name );
		}
		/// <summary>
		/// Get localized version of the object.
		/// </summary>
		/// <param name="name">Name of the localized resource.</param>
		/// <param name="asm">Calling assembly.</param>
		/// <param name="culture">Culture to be used for getting localized version of the resource.</param>
		/// <returns>Localized version of the object.</returns>
		internal protected static object GetObject( Assembly asm, CultureInfo culture, string name )
		{
			Localizer localizer = Localizer.GetLoader( asm );

			if( localizer == null || localizer.resources == null ) return null;

			throw new NotImplementedException( DEF_ERROR_NOTIMPLEMENTED );
		}
		/// <summary>
		/// Writes report about non-localized items.
		/// </summary>
		/// <param name="writer">Text writer.</param>
		internal static void WriteNonLocalizedItemsReport( TextWriter writer )
		{
			if( writer == null )
				throw new ArgumentNullException( "writer" );

			if( m_nonLocalizedNames.Count == 0 )
				writer.WriteLine( "All queiried items are localized." );
			else
			{
				writer.WriteLine( "There are {0} items that are not localized at all.", m_nonLocalizedNames.Count );

				foreach( string name in m_nonLocalizedNames )
				{
					writer.WriteLine( name );
				}

				writer.WriteLine( "" );
			}

			if( m_nonLocalizedForCurrentCulture.Count == 0 )
				writer.WriteLine( "All queiried items are localized for current culture." );
			else
			{
				writer.WriteLine( "There are {0} items that are not localized for current culture.",
					m_nonLocalizedForCurrentCulture.Count );

				foreach( string name in m_nonLocalizedForCurrentCulture )
				{
					writer.WriteLine( name );
				}

				writer.WriteLine( "" );
			}
		}
		/// <summary>
		/// Gets localized names of the enum.
		/// </summary>
		/// <param name="enumType">Type of the enum.</param>
		/// <param name="assembly">Calling assembly.</param>
		/// <param name="culture">Culture to be used for getting localized version of the resource.</param>
		/// <returns>Array of item names in enumeration.</returns>
		internal protected static string[] GetEnumNames( Assembly assembly, CultureInfo culture, Type enumType )
		{
			if( enumType == null )
				throw new ArgumentNullException( "enumType" );

			if( !enumType.IsEnum )
				throw new ArgumentException( DEF_ERROR_ENUMEXPECTED, "enumType" );

			string[] nativeNames = Enum.GetNames( enumType );
			string[] localizedNames = new string[ nativeNames.Length ];
			string namePrefix = "enum_" + enumType.Name + "_";

			for( int i = 0; i < nativeNames.Length; i++ )
			{
				string name = namePrefix + nativeNames[ i ];
				localizedNames[ i ] = GetString( assembly, culture, name );

				if( localizedNames[ i ] == string.Empty
					|| localizedNames[ i ] == null ) localizedNames[ i ] = nativeNames[ i ];
			}

			return localizedNames;
		}
		/// <summary>
		/// Tryies to read all localizable data of the localizable type descriptor.
		/// </summary>
		/// <param name="descriptor">Localizable type descriptor.</param>
		[Conditional( "DEBUG" )]
		internal static void PullLocalizableTypeDescriptor( ILocalizableTypeDescriptor descriptor )
		{
			if( null == descriptor )
				throw new ArgumentNullException( "descriptor" );

			foreach( PropertyDescriptor descriptorProperty in descriptor.GetProperties() )
			{
				if( descriptorProperty.IsBrowsable )
				{
					string strName = descriptorProperty.Name;
					string strDisplay = descriptorProperty.DisplayName;
					string strCategory = descriptorProperty.Category;
					string strDescription = descriptorProperty.Description;
				}
			}

			foreach( EventDescriptor descriptorProperty in descriptor.GetEvents() )
			{
				if( descriptorProperty.IsBrowsable )
				{
					string strName = descriptorProperty.Name;
					string strDisplay = descriptorProperty.DisplayName;
					string strCategory = descriptorProperty.Category;
					string strDescription = descriptorProperty.Description;
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="enumerableList"></param>
		/// <param name="writer"></param>
		/// <param name="typeComponent"></param>
		internal static void WriteResourcesListFromEnumerator( IEnumerable enumerableList, ResXResourceWriter writer, Type typeComponent )
		{
			if( null == writer )
				throw new ArgumentNullException( "writer" );

			if( null == enumerableList )
				throw new ArgumentNullException( "enumerableList" );

			string nameComponentNamespace = typeComponent.Namespace;

			foreach( MemberDescriptor descriptor in enumerableList )
			{
				EventDescriptor descriptorEvent = descriptor as EventDescriptor;
				PropertyDescriptor descriptorProperty = descriptor as PropertyDescriptor;

				Type memberParentType =
					( null != descriptorProperty )
					? descriptorProperty.ComponentType
					: descriptorEvent.ComponentType;

				string nameComponentType = memberParentType.Name;
				string name = descriptor.Name;
				string resourceDescriptionName = GetControlDescriptionResourceName( nameComponentType, name );
				string resourceMemberName =
					( null != descriptorProperty )
					? GetControlPropertyResourceName( nameComponentType, name )
					: GetControlEventResourceName( nameComponentType, name );

				if( descriptor.IsBrowsable )
				{
					if( memberParentType.Namespace != nameComponentNamespace )
					{
						resourceMemberName = memberParentType.Namespace + "." + resourceMemberName;
						resourceDescriptionName = memberParentType.Namespace + "." + resourceDescriptionName;
					}

					writer.AddResource( resourceMemberName, descriptor.DisplayName );
					writer.AddResource( resourceDescriptionName, descriptor.Description );
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="descriptor"></param>
		/// <param name="writer"></param>
		internal static void WriteResourcesList( ILocalizableTypeDescriptor descriptor, ResXResourceWriter writer )
		{
			if( null == descriptor )
				throw new ArgumentNullException( "descriptor" );

			if( null == writer )
				throw new ArgumentNullException( "writer" );

			PropertyDescriptorCollection properties =
				TypeDescriptor.GetProperties( descriptor, null, true );

			EventDescriptorCollection events =
				TypeDescriptor.GetEvents( descriptor, null, true );

			Type componentType = descriptor.GetType();
			WriteResourcesListFromEnumerator( properties, writer, componentType );
			WriteResourcesListFromEnumerator( events, writer, componentType );

		}
		/// <summary>
		/// Gets resource name for control's property description.
		/// </summary>
		/// <param name="controlName">Control type name.</param>
		/// <param name="propertyName">Property name.</param>
		/// <returns>Resource name.</returns>
		internal static string GetControlDescriptionResourceName( string controlName, string propertyName )
		{
			return "descr_" + controlName + "_" + propertyName;
		}
		/// <summary>
		/// Gets resource name for control's name.
		/// </summary>
		/// <param name="controlName">Control type name.</param>
		/// <param name="propertyName">Property name.</param>
		/// <returns>Resource name.</returns>
		internal static string GetControlPropertyResourceName( string controlName, string propertyName )
		{
			return "propName_" + controlName + "_" + propertyName;
		}
		/// <summary>
		/// Gets resource name for control's event.
		/// </summary>
		/// <param name="controlName">Control type name.</param>
		/// <param name="eventName">Event name.</param>
		/// <returns>Resource name.</returns>
		internal static string GetControlEventResourceName( string controlName, string eventName )
		{
			return "eventName_" + controlName + "_" + eventName;
		}
		/// <summary>
		/// Gets resource name for category.
		/// </summary>
		/// <param name="categoryName">Category name.</param>
		/// <returns>Resource name.</returns>
		internal static string GetCategoryResourceName( string categoryName )
		{
			return categoryName;
		}
		#endregion

		#region Class Static Public Methods
		/// <summary>
		/// Loads values for all instance or static fields of the class that have StrLocalized attribute.
		/// </summary>
		/// <param name="localizerType">Type to be localized.</param>
		/// <param name="instance">Type instance to be localized. If null, static fields of class will be localized.</param>
		public static void LocalizeFields( Type localizerType, object instance )
		{
			BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public;
			flags |= ( instance == null ) ? BindingFlags.Static : BindingFlags.Instance;

			FieldInfo[] fields = localizerType.GetFields( flags );

			if( fields == null ) return;

			foreach( FieldInfo field in fields )
			{
				object[] attributes = field.GetCustomAttributes( typeof( StrLocalized ), true );

				if( attributes != null && attributes.Length == 1 )
				{
					StrLocalized attribute = ( StrLocalized )attributes[ 0 ];
					string localizedName = attribute.Name;

					string localizedValue = GetString(
						field.DeclaringType.Assembly, null, localizedName );

					string strCurrent = ( string )field.GetValue( instance );

					if( !( strCurrent != null && strCurrent.Length > 0 && ( localizedValue == null || localizedValue.Length == 0 ) ) )
						field.SetValue( instance, localizedValue );
				}
			}
		}
		/// <summary>
		/// Get localized version of the string.
		/// </summary>
		/// <param name="name">Name of the localized resource.</param>
		/// <returns>Localized version of the string.</returns>
		public static string GetString( string name )
		{
            if (LocalizationProvider.Provider != null)
            {
                String result = string.Empty;
                result = LocalizationProvider.Provider.GetLocalizedString(null, name, null);
                if (result != String.Empty)
                    return result;
                else
                    return name;
            }
			if( name == null )
				throw new ArgumentNullException( "name" );

			Assembly assembly = Assembly.GetCallingAssembly();
			return GetString( assembly, null, name );
		}
		/// <summary>
		/// Get localized version of the object.
		/// </summary>
		/// <param name="name">Name of the localized resource.</param>
		/// <returns>Localized version of the object.</returns>
		public static object GetObject( string name )
		{
			if( name == null )
				throw new ArgumentNullException( "name" );

			Assembly assembly = Assembly.GetCallingAssembly();
			return Localizer.GetObject( assembly, null, name );
		}
		/// <summary>
		/// Gets localized names of the enum.
		/// </summary>
		/// <param name="enumType">Type of the enum.</param>
		/// <returns>Array of item names in enumeration.</returns>
		public static string[] GetEnumNames( Type enumType )
		{
			Assembly assembly = Assembly.GetCallingAssembly();

			if( enumType == null )
				throw new ArgumentNullException( "enumType" );

			if( !enumType.IsEnum )
				throw new ArgumentException( DEF_ERROR_ENUMEXPECTED, "enumType" );

			return GetEnumNames( assembly, null, enumType );
		}
		/// <summary>
		/// Gets native name of the enum value.
		/// </summary>
		/// <param name="enumType">Enum type.</param>
		/// <param name="localizedName">Localized name of the item.</param>
		/// <returns>Native name of the enum value.</returns>
		public static string GetNativeEnumValueName( Type enumType, string localizedName )
		{
			Assembly assembly = Assembly.GetCallingAssembly();

			if( enumType == null )
				throw new ArgumentNullException( "enumType" );

			if( !enumType.IsEnum )
				throw new ArgumentException( DEF_ERROR_ENUMEXPECTED, "enumType" );

			string[] namesLocalized = GetEnumNames( assembly, null, enumType );
			string[] namesNative = Enum.GetNames( enumType );

			for( int i = 0; i < namesLocalized.Length; i++ )
			{
				if( namesLocalized[ i ] == localizedName )
					return namesNative[ i ];
			}

			throw new ArgumentOutOfRangeException(
				"localizedName", localizedName, DEF_ERROR_INVALID_ENUM_VALUE_NAME );
		}
		/// <summary>
		/// Gets native name of the enum value.
		/// </summary>
		/// <param name="enumType">Enum type.</param>
		/// <param name="localizedName">Localized name of the item.</param>
		/// <returns>Native name of the enum value.</returns>
		public static object GetEnumValue( Type enumType, string localizedName )
		{
			Assembly assembly = Assembly.GetCallingAssembly();

			if( enumType == null )
				throw new ArgumentNullException( "enumType" );

			if( !enumType.IsEnum )
				throw new ArgumentException( DEF_ERROR_ENUMEXPECTED, "enumType" );

			string[] namesLocalized = GetEnumNames( assembly, null, enumType );
			string[] namesNative = Enum.GetNames( enumType );

			for( int i = 0; i < namesLocalized.Length; i++ )
			{
				if( namesLocalized[ i ] == localizedName )
					return Enum.Parse( enumType, namesNative[ i ] );
			}

			throw new ArgumentOutOfRangeException(
				"localizedName", localizedName, DEF_ERROR_INVALID_ENUM_VALUE_NAME );
		}
		/// <summary>
		/// Gets localized name of the enum value.
		/// </summary>
		/// <param name="enumType">Enumeration type.</param>
		/// <param name="value">Enum value.</param>
		/// <returns>Enum value localized name.</returns>
		public static string GetEnumValueName( Type enumType, object value )
		{
			Assembly assembly = Assembly.GetCallingAssembly();

			if( enumType == null )
				throw new ArgumentNullException( "enumType" );

			if( !enumType.IsEnum )
				throw new ArgumentException( DEF_ERROR_ENUMEXPECTED, "enumType" );

			if( !Enum.IsDefined( enumType, value ) )
				throw new ArgumentOutOfRangeException( "value", value, DEF_ERROR_ENUM_VALUE_NOT_DEFINED );

			string namePrefix = "enum_" + enumType.Name + "_";
			string name = namePrefix + value.ToString();

			return GetString( assembly, null, name );
		}
		#endregion
        /// <summary>
        /// ToolsResourceIdentifiers contains Ids specific to the Syncfusion.Windows.Forms.Tools namespace. 
        /// </summary>
        public sealed class EditResourceIdentifiers
        {
            #region Class Static Localizable Members
            /// <summary>
            /// ID of the localizable string with "{0} of {1}" format.
            /// </summary>
            [StrLocalized("ContextPromptIndexFormat")]
            public const string DEF_PROMPT_INDEX="ContextPromptIndexFormat";
            ///<exclude/>
            [StrLocalized("errArgEnumExpected")]
            public const string DEF_ERROR_ENUMEXPECTED="errArgEnumExpected";
            ///<exclude/>
            [StrLocalized("errEnumValueNotDefined")]
            public const string DEF_ERROR_ENUM_VALUE_NOT_DEFINED="errEnumValueNotDefined";
            ///<exclude/>
            [StrLocalized("colorEmpty")]
            public const string DEF_COLOR_EMPTY="colorEmpty";
            ///<exclude/>
            [StrLocalized("noneHatch")]
            public const string DEF_NONE_HATCH="noneHatch";
            ///<exclude/>
            [StrLocalized("backGroundSolidFill")]
            public const string DEF_SOLID_FILL="backGroundSolidFill";
            ///<exclude/>
            [StrLocalized("errInvalidLocalizedEnumValueName")]
            public const string DEF_ERROR_INVALID_ENUM_VALUE_NAME="errInvalidLocalizedEnumValueName";
            ///<exclude/>
            [StrLocalized("LexemTreeRoot")]
            public const string DEF_LEXEM_TREE_ROOT="LexemTreeRoot";
            ///<exclude/>
            [StrLocalized("FormatConfigSampleText")]
            public const string DEF_FORMAT_SETTINGS_SAMPLE_TEXT="FormatConfigSampleText";
            ///<exclude/>
            [StrLocalized("ConfigCreateLanguage")]
            public const string DEF_CONFIG_CREATE_LANGUAGE="ConfigCreateLanguage";
            ///<exclude/>
            [StrLocalized("ConfigSaveOthersConfig")]
            public const string DEF_CONFIG_SAVE_OTHERS_CHANGES ="ConfigSaveOthersConfig";
            ///<exclude/>
            [StrLocalized("ConfigSaveChanges")]
            public const string DEF_CONFIG_SAVE_CHANGES ="ConfigSaveChanges";
            ///<exclude/>
            [StrLocalized("errNotImplemented")]
            public const string DEF_ERROR_NOTIMPLEMENTED ="errNotImplemented";
            ///<exclude/>
            [StrLocalized("formatGoToDialogCaption")]
            public const string DEF_GOTO_CAPTION_FORMAT = "Line Number {0}-{1}:";
            ///<exclude/>
            [StrLocalized("contextmenuEdit")]
            public const string DEF_MENU_EDIT ="contextmenuEdit";
            ///<exclude/>
            [StrLocalized("contextmenuFile")]
            public const string DEF_MENU_FILE ="contextmenuFile";
            ///<exclude/>
            [StrLocalized("contextmenuAdvanced")]
            public const string DEF_MENU_ADVANCED ="contextmenuAdvanced";
            ///<exclude/>
            [StrLocalized("contextmenuBookmarks")]
            public const string DEF_MENU_BOOKMARKS ="contextmenuBookmarks";
            ///<exclude/>
            [StrLocalized("contextmenuOptions")]
            public const string DEF_MENU_OPTIONS ="contextmenuOptions";
            ///<exclude/>
            [StrLocalized("contextmenuCut")]
            public const string DEF_MENU_CUT ="contextmenuCut";
            ///<exclude/>
            [StrLocalized("contextmenuCopy")]
            public const string DEF_MENU_COPY = "contextmenuCopy";
            ///<exclude/>
            [StrLocalized("contextmenuPaste")]
            public static string DEF_MENU_PASTE ="contextmenuPaste";
            ///<exclude/>
            [StrLocalized("contextmenuDelete")]
            public const string DEF_MENU_DELETE ="contextmenuDelete";
            ///<exclude/>
            [StrLocalized("contextmenuUndo")]
            public const string DEF_MENU_UNDO ="contextmenuUndo";
            ///<exclude/>
            [StrLocalized("contextmenuRedo")]
            public const string DEF_MENU_REDO ="contextmenuRedo";
            ///<exclude/>
            [StrLocalized("contextmenuFind")]
            public const string DEF_MENU_FIND ="contextmenuFind";
            ///<exclude/>
            [StrLocalized("contextmenuReplace")]
            public const string DEF_MENU_REPLACE ="contextmenuReplace";
            ///<exclude/>
            [StrLocalized("contextmenuGoto")]
            public const string DEF_MENU_GOTO ="contextmenuGoto";
            ///<exclude/>
            [StrLocalized("contextmenuSelectAll")]
            public const string DEF_MENU_SELECTALL ="contextmenuSelectAll";
            ///<exclude/>
            [StrLocalized("contextmenuDeleteAll")]
            public const string DEF_MENU_DELETEALL ="contextmenuDeleteAll";
            ///<exclude/>
            [StrLocalized("contextmenuNew")]
            public const string DEF_MENU_NEW ="contextmenuNew";
            ///<exclude/>
            [StrLocalized("contextmenuOpen")]
            public const string DEF_MENU_OPEN ="contextmenuOpen";
            ///<exclude/>
            [StrLocalized("contextmenuClose")]
            public const string DEF_MENU_CLOSE ="contextmenuClose";
            ///<exclude/>
            [StrLocalized("contextmenuSave")]
            public const string DEF_MENU_SAVE ="contextmenuSave";
            ///<exclude/>
            [StrLocalized("contextmenuSaveAs")]
            public const string DEF_MENU_SAVEAS ="contextmenuSaveAs";
            ///<exclude/>
            [StrLocalized("contextmenuPrintPreview")]
            public const string DEF_MENU_PRINTPREVIEW ="contextmenuPrintPreview";
            ///<exclude/>
            [StrLocalized("contextmenuPrint")]
            public const string DEF_MENU_PRINT ="contextmenuPrint";
            ///<exclude/>
            [StrLocalized("contextmenuTabifySelection")]
            public const string DEF_MENU_TABIFYSELECTION ="contextmenuTabifySelection";
            ///<exclude/>
            [StrLocalized("contextmenuUntabifySelection")]
            public const string DEF_MENU_UNTABIFYSELECTION = "contextmenuUntabifySelection";
            ///<exclude/>
            [StrLocalized("contextmenuIndentSelection")]
            public const string DEF_MENU_INDENTSELECTION = "contextmenuIndentSelection";
            ///<exclude/>
            [StrLocalized("contextmenuUnindentSelection")]
            public const string DEF_MENU_UNINDENTSELECTION = "contextmenuUnindentSelection";
            ///<exclude/>
            [StrLocalized("contextmenuCommentSelection")]
            public const string DEF_MENU_COMMENTSELECTION = "contextmenuCommentSelection";
            ///<exclude/>
            [StrLocalized("contextmenuUncommentSelection")]
            public const string DEF_MENU_UNCOMMENTSELECTION = "contextmenuUncommentSelection";
            ///<exclude/>
            [StrLocalized("contextmenuCollapseAll")]
            public const string DEF_MENU_COLLAPSEALL ="contextmenuCollapseAll";
            ///<exclude/>
            [StrLocalized("contextmenuExpandAll")]
            public const string DEF_MENU_EXPANDALL ="contextmenuExpandAll";
            ///<exclude/>
            [StrLocalized("contextmenuToggleBookmark")]
            public const string DEF_MENU_TOGGLEBOOKMARK ="contextmenuToggleBookmark";
            ///<exclude/>
            [StrLocalized("contextmenuNextBookmark")]
            public const string DEF_MENU_NEXTBOOKMARK ="contextmenuNextBookmark";
            ///<exclude/>
            [StrLocalized("contextmenuPrevBookmark")]
            public const string DEF_MENU_PREVBOOKMARK ="contextmenuPrevBookmark";
            ///<exclude/>
            [StrLocalized("contextmenuClearBookmarks")]
            public const string DEF_MENU_CLEARBOOKMARKS="contextmenuClearBookmarks";
            ///<exclude/>
            [StrLocalized("msgLinuNumberingProblem")]
            public const string DEF_MSG_LINE_NUMBERING_PROBLEMS = "There are some problems with line numbering.";
            ///<exclude/>
            [StrLocalized("msgSaveModified")]
            public const string DEF_MSG_SAVE_MODIFIED = "File was changed, do you want to save changes?";
            ///<exclude/>
            [StrLocalized("msgSaveModifiedCaption")]
            public const string DEF_MSG_SAVE_MODIFIED_CAPTION = "Save changes";
            ///<exclude/>
            [StrLocalized("msgKeyBindingsSetDefault")]
            public const string DEF_MSG_KEYBINDINGS_SET_DEFAULT = "All your changes will be lost. Do you really want reset key bindings to defaults?";
            ///<exclude/>
            [StrLocalized("msgKeyBindingsSetDefaultCaption")]
            public const string DEF_MSG_KEYBINDINGS_SET_DEFAULT_CAPTION = "Key bindings";
            ///<exclude/>
            [StrLocalized("msgFindNothingFound")]
            public const string DEF_MSG_FOUND_NOTHING = "No text was found.";
            ///<exclude/>
            [StrLocalized("msgFindNothingFoundCaption")]
            public const string DEF_MSG_FOUND_NOTHING_CAPTION = "Find text";
            ///<exclude/>
            [StrLocalized("msgXmlConfigLoadError")]
            public const string DEF_MSG_XML_CONFIG_LOAD_ERROR = "Can't load defined XML document.";
            ///<exclude/>
            [StrLocalized("printPageNamuberPrefix")]
            public const string DEF_PRINT_PAGE_PREFIX = "Page:";
            ///<exclude/>
            [StrLocalized("printDocumentNamePrefix")]
            public const string DEF_PRINT_FILE_PREFIX = "File:";
            ///<exclude/>
            [StrLocalized("printDocumentUnnamed")]
            public const string DEF_PRINT_NONAME_DOCUMENT = "New document";
            ///<exclude/>
            [StrLocalized("msgSaveFileUsingEncoding")]
            public const string DEF_MSG_SAVE_FILE_USING_ENCODING = "If you save file using this encoding some data may be lost. Do you want to proceed?";
            ///<exclude/>
            [StrLocalized("msgSaveFileUsingEncodingCaption")]
            public const string DEF_MSG_SAVE_FILE_USING_ENCODING_CAPTION = "Saving file using new encoding";
            ///<exclude/>
            [StrLocalized("msgSaveStreamUsingEncoding")]
            public const string DEF_MSG_SAVE_STREAM_USING_ENCODING = "If you save stream using this encoding some data may be lost. Do you want to proceed?";
            ///<exclude/>
            [StrLocalized("msgSaveStreamUsingEncodingCaption")]
            public const string DEF_MSG_SAVE_STREAM_USING_ENCODING_CAPTION = "Saving stream using new encoding";
            ///<exclude/>
            [StrLocalized("statusbarReadonly")]
            public const string DEF_STATUSBAR_READONLY = "Read only";
            ///<exclude/>
            [StrLocalized("statusbarNotReadonly")]
            public const string DEF_STATUSBAR_NOT_READONLY = "";
            ///<exclude/>
            [StrLocalized("statusbarOverwrite")]
            public const string DEF_STATUSBAR_OVERWRITE = "OVR";
            ///<exclude/>
            [StrLocalized("statusbarInsert")]
            public const string DEF_STATUSBAR_INSERT = "INS";
            ///<exclude/>
            [StrLocalized("statusbarPosition")]
            public const string DEF_STATUSBAR_POSITION = "Line: {0,8}; Column: {1,8}";
            ///<exclude/>
            [StrLocalized("msgIncorrectRegex")]
            public const string DEF_MSG_REGEX_INCORRECT = "The regular expression you have entered is incorrect.";
            ///<exclude/>
            [StrLocalized("msgIncorrectRegexCaption")]
            public const string DEF_MSG_REGEX_INCORRECT_CAPTION = "Incorrect search request";
            ///<exclude/>
            [StrLocalized("optionsAppearance")]
            public const string DEF_OPTS_APPEARANCE = "optionsAppearance";
            ///<exclude/>
            [StrLocalized("optionsAppearanceAreas")]
            public const string DEF_OPTS_APPEARANCEAREAS = "optionsAppearanceAreas";
            ///<exclude/>
            [StrLocalized("optionsAppearanceText")]
            public const string DEF_OPTS_APPEARANCETEXT = "optionsAppearanceText";
            ///<exclude/>
            [StrLocalized("optionsAppearanceControl")]
            public const string DEF_OPTS_APPEARANCECONTROL = "optionsAppearanceControl";
            ///<exclude/>
            [StrLocalized("optionsBehavior")]
            public const string DEF_OPTS_BEHAVIOUR = "optionsBehavior";
            ///<exclude/>
            [StrLocalized("optionsBehaviorGeneral")]
            public const string DEF_OPTS_BEHAVIOURGENERAL = "optionsBehaviorGeneral";
            ///<exclude/>
            [StrLocalized("optionsBehaviorTabs")]
            public const string DEF_OPTS_BEHAVIOURTABS = "optionsBehaviorTabs";
            ///<exclude/>
            [StrLocalized("error")]
            public const string ERROR = "error";
            ///<exclude/>
            [StrLocalized("msgEncodingChanged")]
            public const string DEF_MSG_ENCODING_CHANGED = "Encoding was changed due to insertion of unsupported symbols. Do you want to save using new encoding?";
            public const string FDMain = "Fi&nd what:";
            public const string FDbtnFind = "&Find Next";
            public const string FDbtnClose = "Close";
            public const string FDbtnTempaltes = ">";
            public const string FDbtnMarkAll = "&Mark All";
            public const string FDchkWholeWord = "Match &whole word";
            public const string FDchkHidden = "Search &hidden text";
            public const string FDchkUp = "Search &up";
            public const string FDchkRegular = "Use &regular expressions";
            public const string FDchkWrap = "Wrap around";
            public const string FDrdbDocument = "Current &document";
            public const string FDrdbSelection = "Current &selection";
            public const string FDchkCase = "Match &case";
            public const string FDTitle = "Find";
            public const string FDGroupTitle = "Search";
            public const string FGoNumber = "Line Number {0}-{1}";
            public const string FGobtnCancel = "&Cancel";
            public const string FGobtnOK = "&OK";
            public const string FGoTitle = "Go To Line";
            public const string FRchkCase = "Match &case";
            public const string FRbtnClose = "Close";
            public const string FRlblFind = "Fi&nd what:";
            public const string FRGroupTitle = "Search";
            public const string FRrdbSelection = "Current &selection";
            public const string FRlblReplace = "Replace";
            public const string FRbtnReplace = "Replace";
            public const string FRbtnReplaceAll = "Replace All";
            public const string FRTitle = "Replace";

            public const string RegexSingleChar = ". Any single character";
            public const string RegexZeroOrMore = "* Zero or more";
            public const string RegexOneorMore = "+ One or more";
            public const string RegexLineBegining = "^ Beginning of line";
            public const string RegexLineEnd = "$ End of line";
            public const string RegexBeginEndWord = "\\b Beginning or End of word ";
            public const string RegexLineBreak = "\\n Line break";
            public const string RegexAnyOneCharset = "[] Any one character in the set";
            public const string RegexAnyCharset = "[^ ] Any character not in the set";
            public const string RegexOr = "| Or";
            public const string RegexEscapeChar = "\\ Escape special character";
            public const string RegexTag = "{} Tag expression";
            public const string RegexIdentifier = ":i C/C++ identifier";
            public const string RegexQuotedString = ":q Quoted string";
            public const string RegexSpaceorTab = ":b Space or Tab";
            public const string RegexInteger = ":z Integer";
            public const string StringFindCompleteEventArgs = "Find reached the starting point of the search.";
            #endregion
        }
	}
}