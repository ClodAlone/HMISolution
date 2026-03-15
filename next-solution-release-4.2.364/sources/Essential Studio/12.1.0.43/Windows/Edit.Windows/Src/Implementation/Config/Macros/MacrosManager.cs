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
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.IO;

using Syncfusion.Windows.Forms.Edit.Implementation.Config.Internal;
using Syncfusion.Windows.Forms.Edit.Utils;

namespace Syncfusion.Windows.Forms.Edit.Implementation.Config
{
	/// <summary>
	/// Class for macros managing, including work with XML resource.
	/// </summary>
	public class MacrosManager
		: CollectionBase
	{
		#region Constants
		/// <summary>
		/// Name of XML file with macros.
		/// </summary>
		const string XML_FILE_NAME = "Syncfusion.Windows.Forms.Edit.Implementation.Config.Macros.Macros.xml";
		#endregion

		#region Fields
		/// <summary>
		/// 
		/// </summary>
		private XmlSerializer m_serializer;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets macro by it's index.
		/// </summary>
		public IMacro this[ int index ]
		{
			get
			{
				return ( IMacro )List[ index ];
			}
			set
			{
				List[ index ] = ( Macro )value;
			}
		}
		/// <summary>
		/// Gets xml serializer for macros arrays.
		/// </summary>
		protected XmlSerializer Serializer
		{
			get
			{
				if( m_serializer == null )
				{
					m_serializer = Syncfusion.XmlSerializersCreator.SerializersManager.GetSerializer( typeof( Macro[] ) );
				}

				return m_serializer;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Creates new instance of MacrosManager.
		/// </summary>
		public MacrosManager()
		{
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Creates new macros and adds it to the macros manager.
		/// </summary>
		/// <param name="name">Name of the macro. If the macro with the specified name already exists, it is removed from the list.</param>
		/// <param name="regex">Regular expression string to be used instead of reference to the macro.</param>
		/// <returns>Newly created macro of type Macro.</returns>
		public IMacro Add( string name, string regex )
		{
			return Add( name, regex, true );
		}
		/// <summary>
		/// Creates new macros and adds it to the macros manager.
		/// </summary>
		/// <param name="name">Name of the macro. If the macro with the specified name already exists, it is removed from the list.</param>
		/// <param name="regex">Regular expression string to be used instead of reference to the macro.</param>
		/// <param name="enabled">Specifies whether this macro is enabled by default.</param>
		/// <returns>Newly created macro of type Macro.</returns>
		public IMacro Add( string name, string regex, bool enabled )
		{
			Macro result = new Macro( name, regex, enabled );
			Add( result );
			return result;
		}
		/// <summary>
		/// Adds macros to the list.
		/// </summary>
		/// <param name="macro">Macro object to be added to the list.
		/// If the macro with the specified name already exists, it is removed from the list.</param>
		internal void Add( IMacro macro )
		{
			Remove( macro );
			InnerList.Add( macro );
		}
		/// <summary>
		/// Removes macro from the list.
		/// </summary>
		/// <param name="macro">Macro to be removed.</param>
		public void Remove( IMacro macro )
		{
			if( null == macro ) throw new ArgumentNullException( "macro" );

			Remove( macro.Name );
		}
		/// <summary>
		/// Removes macro from the list.
		/// </summary>
		/// <param name="name">Name of the macro to be removed.</param>
		public void Remove( string name )
		{
			int index = IndexOf( name );

			if( index >= 0 )
				InnerList.RemoveAt( index );
		}
		/// <summary>
		/// Searches for the index of the specified index in the list.
		/// </summary>
		/// <param name="macro">Macro which's index should be found.</param>
		/// <returns>Index of the specified macro in the list or -1 if the specified macro is not present in the list.</returns>
		public int IndexOf( IMacro macro )
		{
			if( null == macro ) throw new ArgumentNullException( "macro" );

			return IndexOf( macro.Name );
		}
		/// <summary>
		/// Searches for the index of the specified index in the list.
		/// </summary>
		/// <param name="name">Name of the macro which's index should be found.</param>
		/// <returns>Index of the macro with the specified name in the list or -1 if the macro with the specified name is not present in the list.</returns>
		public int IndexOf( string name )
		{
			if( null == name ) throw new ArgumentNullException( "name" );
			if( string.Empty == name ) throw new ArgumentException( "Name can not be empty.", "name" );

			for( int i = 0, len = Count; i < len; i++ )
			{
				if( this[ i ].Name == name )
					return i;
			}

			return -1;
		}
		/// <summary>
		/// Checks whether macro manager contains macro with the specified name.
		/// </summary>
		/// <param name="name">Name of the macro to be found.</param>
		/// <returns>True if the macro with the specified name is present in the current macro list, otherwise false.</returns>
		public bool Contains( string name )
		{
			int index = IndexOf( name );

			return index >= 0;
		}
		/// <summary>
		/// Restores default list of macros.
		/// </summary>
		public void LoadDefault()
		{
			ReadXmlMacros();
		}
		/// <summary>
		/// Reads macros list from xmlreader and appends it to the current list.
		/// </summary>
		/// <param name="reader">XmlReader the data should be read from.</param>
		public void AppendMacros( XmlReader reader )
		{
			if( null == reader ) throw new ArgumentNullException( "reader" );

			IList list = GetMacrosListFromXml( reader );
			foreach( Macro macro in list )
			{
				Remove( macro );
				Add( macro );
			}
		}
		/// <summary>
		/// Reads macros list from the file and appends it to the current list.
		/// </summary>
		/// <param name="strXmlFilePath">Name of the file to read.</param>
		public void AppendMacros( string strXmlFilePath )
		{
			if( null == strXmlFilePath ) throw new ArgumentNullException( "strXmlFilePath" );
			if( String.Empty == strXmlFilePath ) throw new ArgumentOutOfRangeException( "strXmlFilePath" );

			XmlTextReader reader = new XmlTextReader( strXmlFilePath );

			try
			{
				AppendMacros( reader );
			}
			finally
			{
				reader.Close();
			}
		}
		/// <summary>
		/// Reads macros list from the stream and appends it to the current list.
		/// </summary>
		/// <param name="stream">Stream, the data should be read from.</param>
		public void AppendMacros( Stream stream )
		{
			if( null == stream ) throw new ArgumentNullException( "stream" );

			XmlReader reader = new XmlTextReader( stream );
			AppendMacros( reader );
		}
		/// <summary>
		/// Loads macros list from the XmlReader.
		/// </summary>
		/// <param name="reader">XmlReader the data should be read from.</param>
		public void LoadMacros( XmlReader reader )
		{
			if( null == reader ) throw new ArgumentNullException( "reader" );

			List.Clear();
			AppendMacros( reader );
		}
		/// <summary>
		/// Loads macros list from the file.
		/// </summary>
		/// <param name="strXmlFilePath">Name of the file data should be read from.</param>
		public void LoadMacros( string strXmlFilePath )
		{
			if( null == strXmlFilePath ) throw new ArgumentNullException( "strXmlFilePath" );
			if( String.Empty == strXmlFilePath ) throw new ArgumentOutOfRangeException( "strXmlFilePath" );

			List.Clear();
			AppendMacros( strXmlFilePath );
		}
		/// <summary>
		/// Loads macros list from the stream.
		/// </summary>
		/// <param name="stream">Stream the data should be read from.</param>
		public void LoadMacros( Stream stream )
		{
			if( null == stream ) throw new ArgumentNullException( "stream" );

			List.Clear();
			AppendMacros( stream );
		}
		/// <summary>
		/// Writes macros list to the XmlWriter.
		/// </summary>
		/// <param name="writer">XmlWriter the data should be written to.</param>
		public void SaveMacros( XmlWriter writer )
		{
			if( null == writer ) throw new ArgumentNullException( "writer" );

			Array macros = InnerList.ToArray( typeof( Macro ) );
			Serializer.Serialize( writer, macros );
		}
		/// <summary>
		/// Saves macros to Xml file.
		/// </summary>
		/// <param name="strXmlFilePath">Path to Xml file where macros should be stored.</param>
		public void SaveMacros( string strXmlFilePath )
		{
			if( null == strXmlFilePath ) throw new ArgumentNullException( "strXmlFilePath" );
			if( String.Empty == strXmlFilePath ) throw new ArgumentOutOfRangeException( "strXmlFilePath" );

			XmlWriter writer = new XmlTextWriter( strXmlFilePath, Encoding.UTF8 );

			try
			{
				SaveMacros( writer );
			}
			finally
			{
				writer.Close();
			}
		}
		/// <summary>
		/// Saves macros to the stream.	Data is saved in XML format.
		/// </summary>
		/// <param name="stream">Stream the data should be saved to.</param>
		public void SaveMacros( Stream stream )
		{
			if( null == stream ) throw new ArgumentNullException( "stream" );

			XmlWriter writer = new XmlTextWriter( stream, Encoding.UTF8 );

			try
			{
				SaveMacros( writer );
			}
			finally
			{
				writer.Close();
			}
		}
		#endregion

		#region Internal Methods
		/// <summary>
		/// Replaces all macros to corresponding regular expressions in given string.
		/// </summary>
		/// <param name="str">String to replace macros in.</param>
		/// <returns>Resulting string.</returns>
		internal string ReplaceMacrosInString( string str )
		{
			for( int i = 0, len = InnerList.Count; i < len; i++ )
			{
				IMacro macro = this[ i ];

				if( !macro.Enabled ) continue;

				str = Regex.Replace( str, Regex.Escape( macro.NameInConfig ), macro.Regex, RegexOptions.IgnoreCase );
			}

			return str;
		}
		#endregion

		#region Helper Methods
		/// <summary>
		/// Reads macros from XML file.
		/// </summary>
		private void ReadXmlMacros()
		{
			Assembly asmExecuting = Assembly.GetExecutingAssembly();

			using( Stream stream = asmExecuting.GetManifestResourceStream( XML_FILE_NAME ) )
			{
				LoadMacros( stream );
			}
		}
		/// <summary>
		/// Gets list of macros from Xml source.
		/// </summary>
		/// <param name="reader">Xml reader.</param>
		/// <returns>List of macros read from Xml.</returns>
		private IList GetMacrosListFromXml( XmlReader reader )
		{
			if( null == reader )
				throw new ArgumentNullException( "reader" );

			reader.MoveToContent();
			return ( Macro[] )Serializer.Deserialize( reader );
		}
		#endregion
	}
}