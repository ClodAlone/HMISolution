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
using System.Xml.Serialization;
using System.Reflection;

namespace Syncfusion.XmlSerializersCreator
{
	/// <summary>
	/// Inherits XmlSerializer and uses manual Xml reader and writer.
	/// </summary>
	public class SerialiserWrapper
		: XmlSerializer
	{
		#region Class Members
		/// <summary>
		/// Serialization reader. Used in overriden CreateReader() method.
		/// </summary>
		XmlSerializationReader m_reader;
		/// <summary>
		/// Serialization writer. Used in overriden CreateWriter() method.
		/// </summary>
		XmlSerializationWriter m_writer;
		#endregion

		#region Class Initialization
		/// <summary>
		/// Creates new instance of SerialiserWrapper.
		/// </summary>
		/// <param name="reader">Serialization reader.</param>
		/// <param name="writer">Serialization writer.</param>
		public SerialiserWrapper( XmlSerializationReader reader, XmlSerializationWriter writer )
			: base()
		{
			if( null == reader )
				throw new ArgumentNullException( "reader" );
			if( null == writer )
				throw new ArgumentNullException( "writer" );

			m_reader = reader;
			m_writer = writer;
		}
		#endregion

		#region Class Overrides
		/// <summary>
		/// Forces using of manual reader.
		/// </summary>
		/// <returns>Xml serialization reader to use.</returns>
		protected override XmlSerializationReader CreateReader()
		{
			return m_reader;
		}
		/// <summary>
		/// Forces using of manual writer.
		/// </summary>
		/// <returns>Xml serialization writer to use.</returns>
		protected override XmlSerializationWriter CreateWriter()
		{
			return m_writer;
		}
		/// <summary>
		/// Serialization.
		/// </summary>
		/// <param name="o"></param>
		/// <param name="writer"></param>
		protected override void Serialize(object o, XmlSerializationWriter writer)
		{
			( ( IXmlSerializationWriter )m_writer ).WriteDataToXml( o );
		}
		/// <summary>
		/// Deserialization.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		protected override object Deserialize(XmlSerializationReader reader)
		{
			return ( ( IXmlSerializationReader )m_reader ).ReadDataFromXml();
		}
		#endregion
	}
}
