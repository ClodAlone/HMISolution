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

using Syncfusion.Windows.Forms.Edit.Interfaces;

namespace Syncfusion.Windows.Forms.Edit.Implementation.Parser
{
	/// <summary>
	/// Part of the text (one word/token).
	/// </summary>
	public class Lexem
		: ILexem
		, IEditableLexem
		, IXMLDataProvider
	{
		#region Fields
		/// <summary>
		/// Text of the lexem.
		/// </summary>
		private string m_Text;
		/// <summary>
		/// Config of the lexem. It also keeps it's format.
		/// </summary>
		private IConfigLexem m_config;
		/// <summary>
		/// Collapsable region, this lexem belongs to.
		/// </summary>
		private CollapsableRegion m_collapser;
		/// <summary>
		/// Column index, where lexem starts in line.
		/// </summary>
		private int m_column = -1;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets config of the lexem. It also keeps it's format.
		/// </summary>
		public IConfigLexem Config
		{
			get
			{
				return m_config;
			}
			set
			{
				m_config = value;
			}
		}
		/// <summary>
		/// Gets or sets text of the lexem.
		/// </summary>
		public string Text
		{
			get
			{
				return m_Text;
			}
			set
			{
				m_Text = value;
			}
		}
		/// <summary>
		/// Gets or sets collapsible region, this lexem belongs to.
		/// </summary>
		public CollapsableRegion Collapser
		{
			get
			{
				return m_collapser;
			}
			set
			{
				m_collapser = value;
			}
		}
		/// <summary>
		/// Gets text length.
		/// </summary>
		public virtual int Length
		{
			get
			{
				int result = 0;
				if( this.Text != null )
				{
					result = Text.Length;
				}
				return result;
			}
		}
		/// <summary>
		/// Y offset of the lexem.
		/// </summary>
		public int Column
		{
			get
			{
				return m_column;
			}
			set
			{
				m_column = value;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Main constructor for the lexem.
		/// </summary>
		/// <param name="text">Text of the lexem.</param>
		/// <param name="config">Configuration of the lexem.</param>
		public Lexem( string text, IConfigLexem config )
		{
			if( text == null ) throw new ArgumentNullException( "text" );
			if( text.Length == 0 ) throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_94 );
			if( config == null ) throw new ArgumentNullException( "config" );

			m_Text = text;
			m_config = config;
		}
		#endregion

		#region IXMLDataProvider Members
		/// <summary>
		/// Writes lexem's data to xml.
		/// </summary>
		/// <param name="parent">Parent xml element, data must be written to.</param>
		public void AppendToXML( XmlElement parent )
		{
			XmlElement element = parent.OwnerDocument.CreateElement( "lexem" );
			element.SetAttribute( "text", m_Text );
			element.SetAttribute( "format", m_config.Format.Name );
			XmlElement node = parent.SelectSingleNode( @"lexem[last()][@format=""" + m_config.Format.Name + @"""]" ) as XmlElement;

			if( node != null )
			{
				node.SetAttribute( "text", node.GetAttribute( "text" ) + m_Text );
			}
			else
			{
				parent.AppendChild( element );
			}
		}
		/// <summary>
		/// Writes lexem's data to xml.
		/// </summary>
		/// <param name="writer">XML writer, data must be written to.</param>
		public void AppendToXML( XmlTextWriter writer )
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}