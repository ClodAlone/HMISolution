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
using System.Xml.Serialization;
using System.Xml;
using System.Collections;

namespace Syncfusion.Windows.Forms.Edit.Utils.CodeSnippets
{
	/// <summary>
	/// Represents VS2005-like code snippet.
	/// </summary>
	public class CodeSnippet
		: IXmlSerializable
	{
		#region Constants
		/// <summary>																																				
		/// Code snippet Xml element name.
		/// </summary>
		private const string DEF_STR_CODESNIPPET = "CodeSnippet";
		/// <summary>
		/// Code snippet title Xml element name.
		/// </summary>
		private const string DEF_STR_TITLE = "Title";
		/// <summary>
		/// Code snippet literal Xml element name.
		/// </summary>
		private const string DEF_STR_LITERAL = "Literal";
		/// <summary>
		/// Code snippet object Xml element name.
		/// </summary>
		private const string DEF_STR_OBJECT = "Object";
		/// <summary>
		/// Code snippet code Xml element name.
		/// </summary>
		private const string DEF_STR_CODE = "Code";
		/// <summary>
		/// Literal ID Xml element name.
		/// </summary>
		private const string DEF_STR_ID = "ID";
		/// <summary>
		/// Literal default text Xml element name.
		/// </summary>
		private const string DEF_STR_DEFAULT = "Default";
		/// <summary>
		/// Header Xml element name.
		/// </summary>
		private const string DEF_STR_HEADER = "Header";
		/// <summary>
		/// Snippet Xml element name.
		/// </summary>
		private const string DEF_STR_SNIPPET = "Snippet";
		/// <summary>
		/// Declarations Xml element name.
		/// </summary>
		private const string DEF_STR_DECLARATIONS = "Declarations";
		/// <summary>
		/// Format Xml attribute name.
		/// </summary>
		private const string DEF_STR_FORMAT = "Format";
		/// <summary>
		/// Shortcut Xml element name.
		/// </summary>
		private const string DEF_STR_SHORTCUT = "Shortcut";
		/// <summary>
		/// Description Xml element name.
		/// </summary>
		private const string DEF_STR_DESCRIPTION = "Description";
		/// <summary>
		/// Author Xml element name.
		/// </summary>
		private const string DEF_STR_AUTHOR = "Author";
		/// <summary>
		/// ToolTip Xml element name.
		/// </summary>
		private const string DEF_STR_TOOLTIP = "ToolTip";
		/// <summary>
		/// Language Xml attribute name.
		/// </summary>
		private const string DEF_STR_LANGUAGE = "Language";
		#endregion

		#region Fields
		/// <summary>
		/// Title of code snippet.
		/// </summary>
		private string m_strTitle;
		/// <summary>
		/// List of code snippet literals.
		/// </summary>
		private ArrayList m_arrLiterals;
		/// <summary>
		/// Text of code snippet.
		/// </summary>
		private string m_strCode;
		/// <summary>
		/// CodeSnippet format identifier.
		/// </summary>
		private string m_strFormat;
		/// <summary>
		/// CodeSnippet shortcut.
		/// </summary>
		private string m_strShortcut;
		/// <summary>
		/// Snippet description.
		/// </summary>
		private string m_strDescription;
		/// <summary>
		/// Snippet author.
		/// </summary>
		private string m_strAuthor;
		/// <summary>
		/// CodeSnippet language.
		/// </summary>
		private string m_strLanguage;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets title of code snippet.
		/// </summary>
		public string Title
		{
			get
			{
				return m_strTitle;
			}
			set
			{
				if( value == null ) throw new ArgumentNullException( "Title" );

				if( m_strTitle != value )
				{
					if( TitleChanging != null )
					{
						TitleChanging( this, new ValueChangedEventArgs( m_strTitle, value ) );
					}

					m_strTitle = value;
				}
			}
		}
		/// <summary>
		/// Gets list of code snippet literals.
		/// </summary>
		public ArrayList Literals
		{
			get
			{
				return m_arrLiterals;
			}
		}
		/// <summary>
		/// Gets or sets text of code snippet.
		/// </summary>
		public string Code
		{
			get
			{
				return m_strCode;
			}
			set
			{
				m_strCode = value;
			}
		}
		/// <summary>
		/// Gets or sets snippet format identifier.
		/// </summary>
		public string Format
		{
			get
			{
				return m_strFormat;
			}
			set
			{
				m_strFormat = value;
			}
		}
		/// <summary>
		/// Gets or sets snippet shortcut.
		/// </summary>
		public string Shortcut
		{
			get
			{
				return m_strShortcut;
			}
			set
			{
				m_strShortcut = value;
			}
		}
		/// <summary>
		/// Gets or sets snippet description.
		/// </summary>
		public string Description
		{
			get
			{
				return m_strDescription;
			}
			set
			{
				m_strDescription = value;
			}
		}
		/// <summary>
		/// Gets or sets snippet author.
		/// </summary>
		public string Author
		{
			get
			{
				return m_strAuthor;
			}
			set
			{
				m_strAuthor = value;
			}
		}
		/// <summary>
		/// Gets or sets snippet language.
		/// </summary>
		public string Language
		{
			get
			{
				return m_strLanguage;
			}
			set
			{
				m_strLanguage = value;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Creates and initializes new instance of CodeSnippet.
		/// </summary>
		public CodeSnippet()
		{
			m_arrLiterals = new ArrayList();
		}
		/// <summary>
		/// Creates and initializes new instance of CodeSnippet.
		/// </summary>
		/// <param name="title">Snippet title.</param>
		/// <param name="literals">List of literals.</param>
		/// <param name="code">Snippet code.</param>
		public CodeSnippet( string title, ArrayList literals, string code )
			: this()
		{
			m_strTitle = title;
			m_arrLiterals = literals;
			m_strCode = code;
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Gets literal's default text by ID.
		/// </summary>
		/// <param name="id">Literal ID.</param>
		/// <returns>Literal text.</returns>
		public string GetLiteralDefault( string id )
		{
			string result = string.Empty;

			foreach( Literal lit in m_arrLiterals )
			{
				if( lit.ID == id )
				{
					result = lit.Default;
				}
			}

			if( !StringOK( result ) ) result = id;

			return result;
		}
        /// <summary>
        /// Gets literal's default text by ID.
        /// </summary>
        /// <param name="id">Literal ID.</param>
        /// <returns>Literal toolTip text.</returns>
        public string GetLiteralToolTip(string id)
        {
            string result = string.Empty;

            foreach (Literal lit in m_arrLiterals)
            {
                if (lit.ID == id)
                {
                    result = lit.ToolTip;
                }
            }

            if (!StringOK(result)) result = id;

            return result;
        }
		#endregion

		#region Events
		/// <summary>
		/// Raised when title of the snippet is going to be changed.
		/// </summary>
		public event ValueChangedEventHandler TitleChanging;
		#endregion

		#region IXmlSerializable Members
		/// <summary>
		/// Not used. For proper interface implementation.
		/// </summary>
		/// <returns>XmlSchema.</returns>
		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}
		/// <summary>
		/// Custom Xml deserialization.
		/// </summary>
		/// <param name="reader">XmlReader.</param>
		public void ReadXml( System.Xml.XmlReader reader )
		{
			reader.MoveToContent();

			while( reader.NodeType != System.Xml.XmlNodeType.EndElement || reader.Name != DEF_STR_CODESNIPPET )
			{
				if( reader.NodeType != System.Xml.XmlNodeType.EndElement )
				{
					switch( reader.Name )
					{
						case DEF_STR_TITLE:
							ReadXmlTitle( reader );
							break;

						case DEF_STR_LITERAL:
						case DEF_STR_OBJECT:
							ReadXmlLiteral( reader );
							break;

						case DEF_STR_CODE:
							ReadXmlCode( reader );
							break;

						case DEF_STR_CODESNIPPET:
							ReadFormat( reader );
							break;

						case DEF_STR_SHORTCUT:
							ReadXmlShortcut( reader );
							break;

						case DEF_STR_DESCRIPTION:
							ReadXmlDescription( reader );
							break;

						case DEF_STR_AUTHOR:
							ReadXmlAuthor( reader );
							break;
					}
				}

				reader.Read();
			}

			reader.Read(); // Read "CodeSnippet" EndElement.
		}
		/// <summary>
		/// Custom Xml serialization.
		/// </summary>
		/// <param name="writer">XmlWriter</param>
		public void WriteXml( System.Xml.XmlWriter writer )
		{
			// Format.
			if( StringOK( m_strFormat ) )
			{
				writer.WriteAttributeString( DEF_STR_FORMAT, m_strFormat );
			}

			writer.WriteStartElement( DEF_STR_HEADER );

			// Title.
			writer.WriteStartElement( DEF_STR_TITLE );
			writer.WriteString( m_strTitle );
			writer.WriteEndElement();

			// Shortcut.
			if( StringOK( m_strShortcut ) )
			{
				writer.WriteStartElement( DEF_STR_SHORTCUT );
				writer.WriteString( m_strShortcut );
				writer.WriteEndElement();
			}

			// Description.
			if( StringOK( m_strDescription ) )
			{
				writer.WriteStartElement( DEF_STR_DESCRIPTION );
				writer.WriteString( m_strDescription );
				writer.WriteEndElement();
			}

			// Author.
			if( StringOK( m_strAuthor ) )
			{
				writer.WriteStartElement( DEF_STR_AUTHOR );
				writer.WriteString( m_strAuthor );
				writer.WriteEndElement();
			}

			writer.WriteEndElement();

			writer.WriteStartElement( DEF_STR_SNIPPET );
			writer.WriteStartElement( DEF_STR_DECLARATIONS );

			// Literals.
			foreach( Literal literal in m_arrLiterals )
			{
				writer.WriteStartElement( DEF_STR_LITERAL );
				writer.WriteStartElement( DEF_STR_ID );
				writer.WriteString( literal.ID );
				writer.WriteEndElement();

				if( StringOK( literal.ToolTip ) )
				{
					writer.WriteStartElement( DEF_STR_TOOLTIP );
					writer.WriteString( literal.ToolTip );
					writer.WriteEndElement();
				}

				if( StringOK( literal.Default ) )
				{
					writer.WriteStartElement( DEF_STR_DEFAULT );
					writer.WriteString( literal.Default );
					writer.WriteEndElement();
				}

				writer.WriteEndElement();
			}

			writer.WriteEndElement();

			// Code.
			writer.WriteStartElement( DEF_STR_CODE );

			if( StringOK( m_strLanguage ) )
			{
				writer.WriteAttributeString( DEF_STR_LANGUAGE, m_strLanguage );
			}

			writer.WriteCData( m_strCode );
			writer.WriteEndElement();

			writer.WriteEndElement();
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Reads code snippet text from Xml.
		/// </summary>
		/// <param name="reader"></param>
		private void ReadXmlCode( System.Xml.XmlReader reader )
		{
			reader.MoveToAttribute( DEF_STR_LANGUAGE );

			if( reader.Value != string.Empty )
			{
				m_strLanguage = reader.Value;
			}

			reader.Read();
			m_strCode = reader.Value.Trim();
		}
		/// <summary>
		/// Reads code snippet literal from Xml.
		/// </summary>
		/// <param name="reader"></param>
		private void ReadXmlLiteral( System.Xml.XmlReader reader )
		{
			Literal literal = new Literal();

			while( reader.NodeType != System.Xml.XmlNodeType.EndElement
				|| reader.Name != DEF_STR_LITERAL && reader.Name != DEF_STR_OBJECT )
			{
				if( reader.NodeType != System.Xml.XmlNodeType.EndElement )
				{
					switch( reader.Name )
					{
						case DEF_STR_ID:
							reader.Read();
							literal.ID = reader.Value.Trim();
							break;

						case DEF_STR_DEFAULT:
							reader.Read();
							literal.Default = reader.Value.Trim();
							break;

						case DEF_STR_TOOLTIP:
							reader.Read();
							literal.ToolTip = reader.Value.Trim();
							break;
					}
				}

				reader.Read();
			}

			m_arrLiterals.Add( literal );
		}
		/// <summary>
		/// Reads code snippet title from Xml.
		/// </summary>
		/// <param name="reader"></param>
		private void ReadXmlTitle( System.Xml.XmlReader reader )
		{
			reader.Read();
			m_strTitle = reader.Value.Trim();
		}
		/// <summary>
		/// Reads code snippet format from Xml.
		/// </summary>
		/// <param name="reader"></param>
		private void ReadFormat( XmlReader reader )
		{
			reader.MoveToAttribute( DEF_STR_FORMAT );

			if( reader.Value != string.Empty )
			{
				m_strFormat = reader.Value;
			}
		}
		/// <summary>
		/// Reads code snippet shortcut from Xml.
		/// </summary>
		/// <param name="reader"></param>
		private void ReadXmlShortcut( System.Xml.XmlReader reader )
		{
			reader.Read();
			m_strShortcut = reader.Value.Trim();
		}
		/// <summary>
		/// Reads code snippet description from Xml.
		/// </summary>
		/// <param name="reader"></param>
		private void ReadXmlDescription( System.Xml.XmlReader reader )
		{
			reader.Read();
			m_strDescription = reader.Value.Trim();
		}
		/// <summary>
		/// Reads code snippet author from Xml.
		/// </summary>
		/// <param name="reader"></param>
		private void ReadXmlAuthor( System.Xml.XmlReader reader )
		{
			reader.Read();
			m_strAuthor = reader.Value.Trim();
		}
		/// <summary>
		/// Checks whether string is assigned.
		/// </summary>
		/// <param name="s">String to check.</param>
		/// <returns>Result.</returns>
		private bool StringOK( string s )
		{
			return ( s != null && s != string.Empty );
		}
		#endregion
	}
}