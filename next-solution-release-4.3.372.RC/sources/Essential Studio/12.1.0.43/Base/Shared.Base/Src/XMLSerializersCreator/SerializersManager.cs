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
using System.Reflection;
using System.Xml.Serialization;

namespace Syncfusion.XmlSerializersCreator
{
	/// <summary>
	/// Class for managing custom Xml serializers.
	/// </summary>
	public class SerializersManager
	{
		/// <summary>
		/// Gets serializer for given type.
		/// </summary>
		/// <param name="type">Type to get serializer for.</param>
		/// <returns>Custom serializer instance or standard XmlSerializer instance if it canot be found.</returns>
		public static XmlSerializer GetSerializer( Type type )
		{
			string typeName = type.Name;

			if( type.IsArray )
			{
				typeName = typeName.Remove( typeName.Length - 2, 2 ).Insert( 0, "ArrayOf" );
			}

			string fullWriterName = typeName + "SerializationWriter";
			string fullReaderName = typeName + "SerializationReader";

			Assembly callingAsm = Assembly.GetCallingAssembly();

			Type[] asmTypes = callingAsm.GetTypes();

      string asmTypeName = string.Empty;
			Type readerType = null;
			Type writerType = null;

			bool bTypesFound = false;

			foreach( Type asmType in asmTypes )
			{
				asmTypeName = asmType.FullName;

				if( asmType.IsArray )
				{
					asmTypeName = asmTypeName.Remove( typeName.Length - 2, 2 ).Insert( 0, "ArrayOf" );
				}

				if( asmTypeName.EndsWith( fullReaderName ) ) readerType = asmType;
				if( asmTypeName.EndsWith( fullWriterName ) ) writerType = asmType;

				if( null != readerType && null != writerType )
				{
					bTypesFound = true;
					break;
				}
			}

			if( bTypesFound )
			{
				return new SerialiserWrapper(
					( XmlSerializationReader )Activator.CreateInstance( readerType ),
					( XmlSerializationWriter )Activator.CreateInstance( writerType ) );
			}
			else
			{
				return new XmlSerializer( type );
			}
		}
	}
}