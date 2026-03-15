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
using System.Collections;
using System.Xml.Serialization;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Syncfusion.XmlSerializersCreator
{
	/// <summary>
	/// Creates files with code of XmlSerializationReader and XmlSerializationWriter derived classes
	/// for manual serialization of sepcified types.
	/// </summary>
	public class XmlSerializersCreator
	{
		#region Class Constants
		/// <summary>
		/// Temporary subdirectory for storing temporary files.
		/// </summary>
		private const string DEF_TEMP_DIR_NAME = @"SerializersCreatorTempDir\";
		/// <summary>
		/// Mask for searching .cs files.
		/// </summary>
		private const string DEF_SC_MASK = "*.cs";
		/// <summary>
		/// String that should be removed from created code.
		/// </summary>
		private const string DEF_ATTR_TO_REMOVE = "[assembly:System.Security.AllowPartiallyTrustedCallers()]";
		/// <summary>
		/// Initial namespace.
		/// </summary>
		private const string DEF_INITIAL_NAMESPACE = "Microsoft.Xml.Serialization.GeneratedAssembly";
		/// <summary>
		/// Place in code where writer base type is defined.
		/// </summary>
		private const string DEF_SER_WRITER_INHERIT = ": System.Xml.Serialization.XmlSerializationWriter";
		/// <summary>
		/// Place in code where reader base type is defined.
		/// </summary>
		private const string DEF_SER_READER_INHERIT = ": System.Xml.Serialization.XmlSerializationReader";
		/// <summary>
		/// Expression for implementing IXmlSerializationWriter.
		/// </summary>
		private const string DEF_WRITER_INTERFACE = ", Syncfusion.XmlSerializersCreator.IXmlSerializationWriter";
		/// <summary>
		/// Expression for implementing IXmlSerializationReader.
		/// </summary>
		private const string DEF_READER_INTERFACE = ", Syncfusion.XmlSerializersCreator.IXmlSerializationReader";
		/// <summary>
		/// Name of Xml reading method in reader class.
		/// </summary>
		private const string DEF_XML_READING_METHOD_NAME = "ReadDataFromXml";
		/// <summary>
		/// Name of Xml writing method in reader class.
		/// </summary>
		private const string DEF_XML_WRITING_METHOD_NAME = "WriteDataToXml";
		/// <summary>
		/// Initial name of reader class.
		/// </summary>
		private const string DEF_INITIAL_READER_CLASS_NAME = "XmlSerializationReader1";
		/// <summary>
		/// Initial name of writer class.
		/// </summary>
		private const string DEF_INITIAL_WRITER_CLASS_NAME = "XmlSerializationWriter1";
		#endregion

		#region Class Members
		/// <summary>
		/// Ouput path to store .cs files.
		/// </summary>
		private string m_strOutputPath;
		/// <summary>
		/// Namespace of output classes.
		/// </summary>
		private string m_strOutputNamespace;
		/// <summary>
		/// List of names of types to create serializer classes for.
		/// </summary>
		private IList m_inputTypes = new ArrayList();
		#endregion

		#region Private Static Members
		/// <summary>
		/// XmlSerializer.tempAssembly field info.
		/// </summary>
		private static FieldInfo _tempAssemblyInfo;
		/// <summary>
		/// XmlSerializer.methodIndex field info.
		/// </summary>
		private static FieldInfo _methodIndexInfo;
		/// <summary>
		/// TempAssembly.methods field info.
		/// </summary>
		private static FieldInfo _methodsInfo;
		/// <summary>
		/// TempAssembly.TempMethod.readMethod field info.
		/// </summary>
		private static FieldInfo _readMethodInfo;
		/// <summary>
		/// TempAssembly.TempMethod.writeMethod field info.
		/// </summary>
		private static FieldInfo _writeMethodInfo;
		#endregion

		#region Class Initialization
		/// <summary>
		/// Creates new instance of XmlSerializersCreator.
		/// </summary>
		/// <param name="strOutputPath">Ouput path to store .cs files.</param>
		/// <param name="strOutputNamespace">Namespace of output classes.</param>
		/// <param name="inputTypes">List of names of types to create serializer classes for.</param>
		public XmlSerializersCreator( string strOutputPath, string strOutputNamespace, IList inputTypes )
		{
			if( null == strOutputPath )
				throw new ArgumentNullException( "strOutputPath" );
			if( string.Empty == strOutputPath )
				throw new ArgumentOutOfRangeException( "strOutputPath" );
			if( null == strOutputNamespace )
				throw new ArgumentNullException( "strOutputNamespace" );
			if( string.Empty == strOutputNamespace )
				throw new ArgumentOutOfRangeException( "strOutputNamespace" );
			if( null == inputTypes )
				throw new ArgumentNullException( "inputTypes" );

      m_strOutputPath = strOutputPath;
			m_strOutputNamespace = strOutputNamespace;
			m_inputTypes = inputTypes;
		}
		/// <summary>
		/// Static constructor. Initializes data for reflexion.
		/// </summary>
		static XmlSerializersCreator()
		{
			Type serType = typeof( XmlSerializer );

			_tempAssemblyInfo = serType.GetField( "tempAssembly", BindingFlags.NonPublic | BindingFlags.Instance );
			_methodIndexInfo = serType.GetField( "methodIndex", BindingFlags.NonPublic | BindingFlags.Instance );
			_methodsInfo =
				_tempAssemblyInfo.FieldType.GetField( "methods", BindingFlags.NonPublic | BindingFlags.Instance );

			Type tempMethodType =
				_tempAssemblyInfo.FieldType.GetNestedType( "TempMethod", BindingFlags.NonPublic | BindingFlags.Instance );

			_readMethodInfo = tempMethodType.GetField( "readMethod", BindingFlags.NonPublic | BindingFlags.Instance );
			_writeMethodInfo = tempMethodType.GetField( "writeMethod", BindingFlags.NonPublic | BindingFlags.Instance );
		}
		#endregion

		#region Class Imports
		/// <summary>
		/// API function for setting value to environment variable.
		/// </summary>
		/// <param name="VariableName">Name of variable.</param>
		/// <param name="VariableValue">New value.</param>
		/// <returns>True if succeeded, otherwise false.</returns>
		[DllImport("KERNEL32.DLL")]
		public static extern bool SetEnvironmentVariable(string VariableName, string VariableValue);
		#endregion

		#region Class Public Methods
		/// <summary>
		/// Creates needed files with required classes.
		/// </summary>
		public void CreateSerializers()
		{
			string tempPath = Path.GetTempPath();
			tempPath += DEF_TEMP_DIR_NAME;

			if( Directory.Exists( tempPath ) ) Directory.Delete( tempPath, true );

			Directory.CreateDirectory( tempPath );
			SetEnvironmentVariable( "TMP", tempPath );

			XmlSerializer m_serializer = null;

			Hashtable hashMethodsToReplace = new Hashtable();

			foreach( Type type in m_inputTypes )
			{
				m_serializer = new XmlSerializer( type );
        hashMethodsToReplace.Add( type, GetSerializationMethodsNames( m_serializer ) );				
			}

			string path = m_strOutputPath;

			if( !Directory.Exists( path ) )
				Directory.CreateDirectory( path );
			else
			{
				foreach( string strFile in Directory.GetFiles( path, "*.cs" ) )
				{
					File.Delete( Path.Combine( path, strFile ) );
				}
			}


			Hashtable hashTypesNames = new Hashtable();

			IList types = new ArrayList();

			// Create new list of types where array types are in the first part.
			// It's useful for searching generated files content.
			foreach( Type type in m_inputTypes )
			{
				if( type.IsArray ) types.Insert( 0, type );
				else types.Insert( types.Count, type );
			}

			foreach( Type type in types )
			{
				string typeName = type.Name;

				if( type.IsArray )
				{
					typeName = typeName.Remove( typeName.Length - 2, 2 ).Insert( 0, "ArrayOf" );
				}

				hashTypesNames.Add( type, typeName );
			}
			
			string[] scFiles = Directory.GetFiles( tempPath, DEF_SC_MASK );

			string strNewPath;

			foreach( string fileName in scFiles )
			{
				strNewPath = path + Path.GetFileName( fileName );
				File.Copy( fileName, strNewPath );
			}

			scFiles = Directory.GetFiles( path, DEF_SC_MASK );

			foreach( string fileName in scFiles )
			{
				Stream fileStream = new FileStream( fileName, FileMode.Open );

				string fileContent;
				
				using( TextReader reader = new StreamReader( fileStream ) )
				{
					fileContent = reader.ReadToEnd();
				}

				File.Delete( fileName );
				
				fileContent = fileContent.Replace( DEF_ATTR_TO_REMOVE, string.Empty );
				fileContent = fileContent.Replace( DEF_INITIAL_NAMESPACE, m_strOutputNamespace );

        for( int i = 0; i < types.Count; i++ )
				{
					Type type = ( Type )types[ i ];

					string typeName = ( string )hashTypesNames[ type ];

					if( -1 != fileContent.IndexOf( "\"" + typeName + "\"" ) )
					{
						string[] methods = ( string[] )hashMethodsToReplace[ type ];
						fileContent = fileContent.Replace( methods[ 0 ], DEF_XML_READING_METHOD_NAME );
						fileContent = fileContent.Replace( methods[ 1 ], DEF_XML_WRITING_METHOD_NAME );

						fileContent = AddInterfacesToCode( fileContent );

						CorrectClassNamesAndSaveText( fileContent, typeName, path );

						types.RemoveAt( i );
						break;
					}
				}
			}
		}
		#endregion

		#region Class Utility Methods
		/// <summary>
		/// Gets names of Xml reading and writing methods from given Xml serializer.
		/// </summary>
		/// <param name="serializer">Xml serializer instance to extract data from.</param>
		/// <returns>Array of two strings with required names.</returns>
		private string[] GetSerializationMethodsNames( XmlSerializer serializer )
		{
			if( null == serializer )
				throw new ArgumentNullException( "serializer" );

			object tempAssembly = _tempAssemblyInfo.GetValue( serializer );
			int methodIndex = ( int )_methodIndexInfo.GetValue( serializer );
			object methods = _methodsInfo.GetValue( tempAssembly );
			object tempMethod = ( ( System.Array )methods ).GetValue( methodIndex );

			MethodInfo reader = ( MethodInfo )_readMethodInfo.GetValue( tempMethod );
			MethodInfo writer = ( MethodInfo )_writeMethodInfo.GetValue( tempMethod );

			string[] result = new string[ 2 ];
			result[ 0 ] = reader.Name;
			result[ 1 ] = writer.Name;

			return result;
		}
		/// <summary>
		/// Changes classes names and saves code to files.
		/// </summary>
		/// <param name="text">Generated text with code.</param>
		/// <param name="type">Name of type to create serializers for.</param>
		/// <param name="path">Path to the directory where result files should be stored.</param>
		private void CorrectClassNamesAndSaveText( string text, string type, string path )
		{
			if( null == text )
				throw new ArgumentNullException( "text" );
			if( null == type )
				throw new ArgumentNullException( "type" );
			if( null == path )
				throw new ArgumentNullException( "path" );
			if( string.Empty == text )
				throw new ArgumentOutOfRangeException( "text" );
			if( string.Empty == type )
				throw new ArgumentOutOfRangeException( "type" );
			if( string.Empty == path )
				throw new ArgumentOutOfRangeException( "path" );

			text = text.Replace( DEF_INITIAL_WRITER_CLASS_NAME, type + "SerializationWriter" );
			text = text.Replace( DEF_INITIAL_READER_CLASS_NAME, type + "SerializationReader" );
			StreamWriter writer = File.CreateText( path + type + "Serializer.cs" );
			writer.Write( text );
			writer.Close();
		}
		/// <summary>
		/// Adds interfaces implementation to code.
		/// </summary>
		/// <param name="text">String with code to modifie.</param>
		/// <returns>String with corrected code.</returns>
		private string AddInterfacesToCode( string text )
		{
			if( null == text )
				throw new ArgumentNullException( "text" );
			if( string.Empty == text )
				throw new ArgumentOutOfRangeException( "text" );

			int iInsPlace = text.IndexOf( DEF_SER_WRITER_INHERIT ) + DEF_SER_WRITER_INHERIT.Length;
			text = text.Insert( iInsPlace, DEF_WRITER_INTERFACE );
			iInsPlace = text.IndexOf( DEF_SER_READER_INHERIT ) + DEF_SER_READER_INHERIT.Length;
			text = text.Insert( iInsPlace, DEF_READER_INTERFACE );

			return text;
		}
		#endregion
	}
}