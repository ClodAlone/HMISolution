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
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Xsl;
using System.Reflection;
using System.Collections;

using Syncfusion.Windows.Forms.Edit.Implementation.Parser;
using Syncfusion.Windows.Forms.Edit.Implementation.Config;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Implementation.IO;
using Syncfusion.IO;

namespace Syncfusion.Windows.Forms.Edit.Utils
{
	/// <summary>
	/// Class for exporting data to different formats.
	/// </summary>
	public class Exporter
	{
		#region Class Constants
		/// <summary>
		/// Name of the resource with XSL transformation for HTML representation of the text.
		/// </summary>
		private const string DEF_XSL_TRANSFORM = "Syncfusion.Windows.Forms.Edit.OutPutConvert.xslt";
		/// <summary>
		/// Name of the resource with XSL transformation for HTML representation of the text with BR tags.
		/// </summary>
		private const string DEF_XSL_TRANSFORM_WITH_BR = "Syncfusion.Windows.Forms.Edit.OutPutConvertWithBR.xslt";
		/// <summary>
		/// Name of the resource with XSL transformation for RTF representation of the text.
		/// </summary>
		private const string DEF_XSL_TRANSFORM_RTF = "Syncfusion.Windows.Forms.Edit.OutPutConvertRTF.xslt";
		/// <summary>
		/// Name of the resource with XSL transformation for CSS part of HTML representation of the text.
		/// </summary>
		private const string DEF_XSL_TRANSFORM_CSS = "Syncfusion.Windows.Forms.Edit.OutPutConvertCSS.xslt";
		/// <summary>
		/// Name of the resource with XSL transformation for CSS part of HTML representation of the text.
		/// </summary>
		private const string DEF_XSL_TRANSFORM_HTMLBODY = "Syncfusion.Windows.Forms.Edit.OutPutConvertHTMLBody.xslt";
		#endregion

		#region Class Members
		/// <summary>
		/// Underlying parser.
		/// </summary>
		private LexemParser m_parser;			 
		/// <summary>
		/// Indicates whether underlying parser was locally created.
		/// </summary>
		private bool m_bLocalParser;
		/// <summary>
		/// Caches parsers for different languages.
		/// </summary>
		private Hashtable m_hashParsers = new Hashtable();
		#endregion

		#region Class Properties
		/// <summary>
		/// Gets or sets underlying parser.
		/// </summary>
		public LexemParser Parser
		{
			get
			{
				return m_parser;
			}
			set
			{
				m_parser = value;
				m_bLocalParser = false;
			}
		}
		#endregion

		#region Class Initialization
		/// <summary>
		/// Creates new instance of Exporter.
		/// </summary>
		public Exporter()
		{
		}
		/// <summary>
		/// Creates and initializes new instance of Exporter.
		/// </summary>
		/// <param name="parser">Underlying parser.</param>
		public Exporter( LexemParser parser )
		{
      m_parser = parser;
		}
		/// <summary>
		/// Creates new instance of Exporter. Local underlying parser is created.
		/// </summary>
		/// <param name="language">Language to use for text colouring.</param>
		public Exporter( string language )
		{
			m_parser = ( LexemParser )m_hashParsers[ language ];

			if( null == m_parser )
			{
				Config config = new Config();

				IConfigLanguage configLang = null;

				bool bCache = true;

				try
				{
					configLang = config[ language ];
				}
				catch( Exception )
				{
					configLang = config.DefaultLanguage;
					bCache = false;
				}

				StreamsWrapper streamsWrapper = new StreamsWrapper( new MemoryStream( 0 ), NewLineStyle.Windows );

				m_parser = new LexemParser( streamsWrapper, configLang);

				if( bCache ) m_hashParsers.Add( language, m_parser );
			}

			m_bLocalParser = true;
		}
		#endregion

		#region Class Public Methods
		/// <summary>
		/// Gets text represented as XML.
		/// </summary>
		/// <returns>String with XML code.</returns>
		public string GetXML()
		{
			return GetXMLInternal( null, null );
		}
		/// <summary>
		/// Gets text represented as HTML.
		/// </summary>
		/// <returns>String with HTML code.</returns>
		public string GetHTML()
		{
			return GetTransformedXML(
				null, null, Assembly.GetExecutingAssembly().GetManifestResourceStream( DEF_XSL_TRANSFORM ) );
		}
		/// <summary>
		/// Gets text represented as HTML.
		/// </summary>
		/// <returns>String with HTML code.</returns>
		public string GetHTMLBreakTags()
		{
			return GetTransformedXML(
				null, null, Assembly.GetExecutingAssembly().GetManifestResourceStream( DEF_XSL_TRANSFORM_WITH_BR ) );
		}
		/// <summary>
		/// Gets text represented as RTF.
		/// </summary>
		/// <returns>String with RTF code.</returns>
		public string GetRTF()
		{
			return GetTransformedXML(
				null, null, Assembly.GetExecutingAssembly().GetManifestResourceStream( DEF_XSL_TRANSFORM_RTF ) );
		}
		/// <summary>
		/// Gets CSS part of HTML code.
		/// </summary>
		/// <returns>String with �SS code.</returns>
		public string GetCSS()
		{
			return GetTransformedXML(
				null, null, Assembly.GetExecutingAssembly().GetManifestResourceStream( DEF_XSL_TRANSFORM_CSS ) );
		}
		/// <summary>
		/// Gets text represented as HTML.
		/// </summary>
		/// <returns>String with HTML code.</returns>
		public string GetHTMLBody()
		{
			return GetTransformedXML(
				null, null, Assembly.GetExecutingAssembly().GetManifestResourceStream( DEF_XSL_TRANSFORM_HTMLBODY ) );
		}
		/// <summary>
		/// Saves document's XML representation to the file.
		/// </summary>
		/// <param name="filename">Name of the file, the document should be saved to.</param>
		public void SaveAsXML( string filename )
		{
			if( null == filename )
				throw new ArgumentNullException( "filename" );
			if( String.Empty == filename )
				throw new ArgumentOutOfRangeException( "filename" );

			XmlDocument doc = GenerateXMLDocument( null, null );
			doc.Save( filename );
		}
		/// <summary>
		/// Saves document's HTML representation to the file.
		/// </summary>
		/// <param name="filename">Name of the file, the document should be saved to.</param>
		/// <param name="bUseLineBreakTags">Indicates whether line break tags should be used.</param>
		public void SaveAsHTML( string filename, bool bUseLineBreakTags )
		{
			string str = String.Empty;

			if( bUseLineBreakTags )
			{
				str = GetHTMLBreakTags();
			}
			else
			{
				str = GetHTML();
			}

			StreamWriter writer = new StreamWriter( filename );
			writer.Write( str );
			writer.Close();
		}
		/// <summary>
		/// Saves document's RTF representation to the file.
		/// </summary>
		/// <param name="filename">Name of the file, the document should be saved to.</param>
		public void SaveAsRTF( string filename )
		{
			string str = GetRTF();

			StreamWriter writer = new StreamWriter( filename );
			writer.Write( str );
			writer.Close();
		}
		/// <summary>
		/// Returns text situated between specified coordinate points represented as XML.
		/// </summary>
		/// <param name="start">Point representing start of the text.</param>
		/// <param name="end">Point representing end of the text.</param>
		/// <returns>String with desired text represented as XML.</returns>
		public string GetXML( CoordinatePoint start, CoordinatePoint end )
		{
			return GetXMLInternal( start, end );
		}
		/// <summary>
		/// Returns text situated between specified coordinate points represented as HTML.
		/// </summary>
		/// <param name="start">Point representing start of the text.</param>
		/// <param name="end">Point representing end of the text.</param>
		/// <returns>String with desired text represented as HTML.</returns>
		public string GetHTML( CoordinatePoint start, CoordinatePoint end )
		{
			return GetTransformedXML(
				start, end, Assembly.GetExecutingAssembly().GetManifestResourceStream( DEF_XSL_TRANSFORM ) );
		}
		/// <summary>
		/// Returns text situated between specified coordinate points represented as RTF.
		/// </summary>
		/// <param name="start">Point representing start of the text.</param>
		/// <param name="end">Point representing end of the text.</param>
		/// <returns>String with desired text represented as RTF.</returns>
		public string GetRTF( CoordinatePoint start, CoordinatePoint end )
		{
			return GetTransformedXML(
				start, end, Assembly.GetExecutingAssembly().GetManifestResourceStream( DEF_XSL_TRANSFORM_RTF ) );
		}
		/// <summary>
		/// Assignes text to the underlying parser.
		/// </summary>
		/// <param name="text">Text to set.</param>
		public void SetText( string text )
		{
			if( !m_bLocalParser ) throw new Exception( "Text cannot be assigned to not locally created parser." );

			if( null == text )
				throw new ArgumentNullException( "text" );

			ILexemLine lastline = m_parser.GetLine( m_parser.TotalLines );

			if( null != lastline )
			{
				CoordinatePoint startPoint = m_parser.GetCoordinatePoint( 1, 1 );
				CoordinatePoint endPoint = m_parser.GetCoordinatePoint( lastline.LineEndPoint );

				if( startPoint != endPoint )
				{
					m_parser.DeleteText( startPoint, endPoint );
				}
			}

			m_parser.InsertText( text, m_parser.GetCoordinatePoint( 1, 1 ) );
		}
		#endregion

		#region Class Utility Methods
		/// <summary>
		/// Generates XML representation of the text situated between specified coordinate points.
		/// If points are null values, XML for whole text is created.
		/// </summary>
		/// <param name="start">Point representing start of the text.</param>
		/// <param name="end">Point representing end of the text.</param>
		/// <returns>Generated XML document.</returns>
		private XmlDocument GenerateXMLDocument( CoordinatePoint start, CoordinatePoint end )
		{
			bool bWhole = ( null == start && null == end );

			if( !bWhole )
			{
				int iTotalLines = Parser.TotalLines;

				if( ( start.VirtualLine < 1 ) || ( start.VirtualColumn < 1 ) || ( start.VirtualLine > iTotalLines ))
					throw new ArgumentOutOfRangeException( "start" );
				if( ( end.VirtualLine < 1 ) || ( end.VirtualColumn < 1 ) || ( end.VirtualLine > iTotalLines ))
					throw new ArgumentOutOfRangeException( "end" );
			}

			MemoryStream stream = new MemoryStream();
			XmlTextWriter writer = new XmlTextWriter( stream, Encoding.UTF8 );
			writer.WriteStartElement( "root" );

			if( bWhole )
			{
				m_parser.AppendToXML( writer );
			}
			else
			{
				m_parser.AppendToXML( writer, start, end );
			}

			( m_parser.Formats as IXMLDataProvider ).AppendToXML( writer );
			writer.WriteEndElement();
			writer.Close();

			XmlDocument doc = new XmlDocument();
			doc.Load( new MemoryStream( stream.GetBuffer() ) );
			return doc;
		}
		/// <summary>
		/// Gets text represented as XML.
		/// </summary>
		/// <param name="start">Point representing start of the text.</param>
		/// <param name="end">Point representing end of the text.</param>
		/// <returns>String with XML code.</returns>
		private string GetXMLInternal( CoordinatePoint start, CoordinatePoint end )
		{
			XmlDocument doc = GenerateXMLDocument( start, end );

			MemoryStream stream = new MemoryStream();
			doc.Save( stream );
			stream.Position = 0;
			StreamReader reader = new StreamReader( stream );
			string result = reader.ReadToEnd();
			reader.Close();

			return result;
		}
#pragma warning disable
        /// <summary>
		/// Returns string with XML transformed by given XSL.
		/// </summary>
		/// <param name="start">Point representing start of the text.</param>
		/// <param name="end">Point representing end of the text.</param>
		/// <param name="XSLInput">Input XSL stream.</param>
		/// <returns>String with XML transformed by given XSL.</returns>
		private string GetTransformedXML( CoordinatePoint start, CoordinatePoint end, Stream XSLInput )
		{
			if( null == XSLInput ) throw new ArgumentNullException( "XSLInput" );

			XmlDocument doc = GenerateXMLDocument( start, end );

			XslTransform trans = new XslTransform();
            
			XmlTextReader xslReader = new XmlTextReader( XSLInput );
#pragma warning enable
#if SyncfusionFramework1_0
			trans.Load( xslReader );
#else
            trans.Load( xslReader, new XmlUrlResolver(), this.GetType().Assembly.Evidence ); 
#endif

			xslReader.Close();

			MemoryStream stream = new MemoryStream();

#if SyncfusionFramework1_0
			trans.Transform( doc.CreateNavigator(), null, stream );
#else
			trans.Transform( doc.CreateNavigator(), null, stream, new XmlUrlResolver() );
#endif

			stream.Position = 0;
			string strData = string.Empty;

			using( StreamReader reader = new StreamReader( stream ) )
			{
				strData = reader.ReadToEnd();
			}

			return strData;
		}
		#endregion
	}
}