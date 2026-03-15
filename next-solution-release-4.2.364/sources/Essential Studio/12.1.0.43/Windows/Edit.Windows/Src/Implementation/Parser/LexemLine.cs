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
using System.Xml;
using System.Diagnostics;

using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Implementation.IO;
using Syncfusion.Windows.Forms.Edit.Utils;
using Syncfusion.Windows.Forms.Edit.Enums;

namespace Syncfusion.Windows.Forms.Edit.Implementation.Parser
{
	/// <summary>
	/// Line of the lexems.
	/// </summary>
	public class LexemLine
		: UncachedLexemLine
		, ILexemLine
		, IXMLDataProvider
	{
		#region Fields
		/// <summary>
		/// List of lexems in line if line is parsed.
		/// </summary>
		private IList m_lexems;
		/// <summary>
		/// Stack at the end of the line.
		/// </summary>
		private ConfigStack m_endStack;
		/// <summary>
		/// <see cref="IParsePoint"/> to the end of the line.
		/// </summary>
		private IParsePoint m_endPoint;
		/// <summary>
		/// Position in stream before new-line.
		/// </summary>
		private IParsePoint m_positionBeforeNewLine;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets flag that determines, whether line is parsed. If line is parsed, then LineEndStack property contains Stack for the end of the line
		/// and LineLexems collection contains all lexems, that belong to current line. If line was changed, than Parsed will be set to false.
		/// </summary>
		public override bool Parsed
		{
			get
			{
				return ( m_lexems != null && m_positionBeforeNewLine != null && m_endStack != null && m_endPoint != null );
			}
			set
			{
				if( value != this.Parsed )
				{
					if( value )
					{
						m_lexems = GetLineLexems();
					}
					else
					{
						m_endStack = null;
						m_lexems = null;

						DisposeEndPoint();
						DisposePointBeforeNewline();
					}
				}
			}
		}
		/// <summary>
		/// Gets ParsePoint at the beginning of the line.
		/// </summary>
		public override IParsePoint LineEndPoint
		{
			get
			{
				if( m_endPoint == null )
				{
					GetLineLexems();
					if( m_endPoint == null )
						throw new NullReferenceException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_97 );
				}
				return m_endPoint;
			}
		}
		/// <summary>
		/// Gets ParsePoint that points to the position before newline.
		/// </summary>
		public IParsePoint PointBeforeNewline
		{
			get
			{
				if( m_positionBeforeNewLine == null )
				{
					this.Parsed = false;
					this.Parsed = true;
				}
				return m_positionBeforeNewLine;
			}
		}
		#endregion

		#region Initialization And Finalization
		/// <summary>
		/// Creates and initializes new instance of the class.
		/// </summary>
		/// <param name="parser">Parent parser.</param>
		/// <param name="pointLineStart">ParsePoint of the line start.</param>
		/// <param name="stack">Stack at the beginning of the line.</param>
		public LexemLine( LexemParser parser, IParsePoint pointLineStart, ConfigStack stack )
			: base( parser, pointLineStart, stack )
		{
		}
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		public override void Dispose()
		{
			DisposeEndPoint();
			DisposePointBeforeNewline();

			base.Dispose();
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Writes data to xml.
		/// </summary>
		/// <param name="parent">Parent xml element, data must be written to.</param>
		public void AppendToXML( XmlElement parent )
		{
			if( parent == null ) throw new ArgumentNullException( "parent" );

			IList list = LineLexems;
			XmlElement element = parent.OwnerDocument.CreateElement( "line" );
			element.SetAttribute( "index", LineIndex.ToString() );

			foreach( IXMLDataProvider data in list )
			{
				data.AppendToXML( element );
			}
			parent.AppendChild( element );
		}
		/// <summary>
		/// Writes data to xml.
		/// </summary>
		/// <param name="writer">XML writer, data must be written to.</param>
		public void AppendToXML( XmlTextWriter writer )
		{
			if( writer == null ) throw new ArgumentNullException( "writer" );

			IList list = LineLexems;
			writer.WriteStartElement( "line" );
			writer.WriteAttributeString( "index", LineIndex.ToString() );
			WriteLexemsToXML( writer, 0, LineLexems.Count - 1 );
			writer.WriteEndElement();
		}
		/// <summary>
		/// Writes end part of line to XML.
		/// </summary>
		/// <param name="start">Index of line column to start reading data from.</param>
		/// <param name="writer">XML writer, data must be written to.</param>
		public void AppendEndPartToXML( XmlTextWriter writer, int start )
		{
			if( writer == null ) throw new ArgumentNullException( "writer" );
			if( start < 1 || start > LineLength ) throw new ArgumentOutOfRangeException( "start" );

			ILexem lexem = base.FindLexemByColumn( start );

			if( lexem != null )
			{
				writer.WriteStartElement( "line" );
				writer.WriteAttributeString( "index", LineIndex.ToString() );
				int startPoint = start - ( ( RenderedLexem )lexem ).Column;
				WriteStringToXML( writer, lexem.Text.Substring( startPoint, lexem.Length - startPoint ), lexem.Config.Format );
				WriteLexemsToXML( writer, LineLexems.IndexOf( lexem ) + 1, LineLexems.Count - 1 );
				writer.WriteEndElement();
			}
		}
		/// <summary>
		/// Writes middle part of line to XML.
		/// </summary>
		/// <param name="writer">XML writer, data must be written to.</param>
		/// <param name="start">Index of line column to start reading data from.</param>
		/// <param name="end">Index of line column to end reading data at.</param>
		public void AppendMiddlePartToXML( XmlTextWriter writer, int start, int end )
		{
			if( writer == null ) throw new ArgumentNullException( "writer" );
			if( start < 1 || start > LineLength ) throw new ArgumentOutOfRangeException( "start" );
			if( end < 1 || end > LineLength + 1 ) throw new ArgumentOutOfRangeException( "end" );

			if( start <= end )
			{
				ILexem startLexem = base.FindLexemByColumn( start );
				ILexem endLexem;
				if( end == this.LineLength + 1 )
				{
					endLexem = ( ILexem )m_lexems[ m_lexems.Count - 1 ];
				}
				else
				{
					endLexem = base.FindLexemByColumn( end );
				}

				if( startLexem != null && endLexem != null )
				{
					writer.WriteStartElement( "line" );
					writer.WriteAttributeString( "index", LineIndex.ToString() );
					if( startLexem == endLexem )
					{
						WriteStringToXML(
							writer, startLexem.Text.Substring( start - ( ( RenderedLexem )startLexem ).Column, end - start ), startLexem.Config.Format );
					}
					else
					{
						int startPoint = start - ( ( RenderedLexem )startLexem ).Column;
						WriteStringToXML( writer, startLexem.Text.Substring( startPoint, startLexem.Length - startPoint ), startLexem.Config.Format );
						WriteLexemsToXML( writer, LineLexems.IndexOf( startLexem ) + 1, LineLexems.IndexOf( endLexem ) - 1 );
						WriteStringToXML( writer, endLexem.Text.Substring( 0, end - ( ( RenderedLexem )endLexem ).Column ), endLexem.Config.Format );
						writer.WriteEndElement();
					}
				}
			}
		}
		/// <summary>
		/// Writes start part of line to XML.
		/// </summary>
		/// <param name="writer">XML writer, data must be written to.</param>
		/// <param name="end">Index of line column to end reading data at.</param>
		public void AppendStartPartToXML( XmlTextWriter writer, int end )
		{
			if( writer == null ) throw new ArgumentNullException( "writer" );
			if( end < 1 || end > LineLength + 1 ) throw new ArgumentOutOfRangeException( "start" );

			ILexem lexem = base.FindLexemByColumn( end - 1 );
			if( lexem != null )
			{
				writer.WriteStartElement( "line" );
				writer.WriteAttributeString( "index", LineIndex.ToString() );
				WriteLexemsToXML( writer, 0, LineLexems.IndexOf( lexem ) - 1 );
				WriteStringToXML( writer, lexem.Text.Substring( 0, end - ( ( RenderedLexem )lexem ).Column ), lexem.Config.Format );
				writer.WriteEndElement();
			}
		}
		/// <summary>
		/// Writes string to XML.
		/// </summary>
		/// <param name="writer">XML writer, data must be written to.</param>
		/// <param name="text">string to write to XML.</param>
		/// <param name="format"></param>
		public void WriteStringToXML( XmlTextWriter writer, string text, ISnippetFormat format )
		{
			writer.WriteStartElement( "lexem" );
			writer.WriteAttributeString( "format", format.Name );
			writer.WriteStartAttribute( "text", null );
			writer.WriteString( text );
			writer.WriteEndAttribute();
			writer.WriteEndElement();
		}
		/// <summary>
		/// Write specified lexems to XML.
		/// </summary>
		/// <param name="writer">XML writer, data must be written to.</param>
		/// <param name="start">Index of first lexem to write.</param>
		/// <param name="end">Index of last lexem to write.</param>
		public void WriteLexemsToXML( XmlTextWriter writer, int start, int end )
		{
			ILexem last = null;
			for( int i = start; i <= end; i++ )
			{
				ILexem data = ( ILexem )LineLexems[ i ];
				if( last == null || last.Config.Format.Name != data.Config.Format.Name )
				{
					if( last != null )
					{
						writer.WriteEndAttribute();
						writer.WriteEndElement();
					}

					writer.WriteStartElement( "lexem" );
					writer.WriteAttributeString( "format", data.Config.Format.Name );
					writer.WriteStartAttribute( "text", null );
				}

				writer.WriteString( data.Text );
				last = data;
			}

			if( last != null )
			{
				writer.WriteEndAttribute();
				writer.WriteEndElement();
			}
		}
		/// <summary>
		/// Gets coordinate point that points to the start of the line.
		/// </summary>
		/// <returns>Start point of line.</returns>
		public CoordinatePoint GetStartPoint()
		{
			CoordinatePoint point = new CoordinatePoint( Parser, LineStartPoint, LineIndex, 1, true );
			return point;
		}
		/// <summary>
		/// Gets coordinate point that points to the end of the line.
		/// </summary>
		/// <returns>Coordinate point of the end of the line.</returns>
		public CoordinatePoint GetEndPoint()
		{
			return new CoordinatePoint( Parser, PointBeforeNewline, LineIndex, LineLength + 1, true );
		}
		/// <summary>
		/// Gets stack copy for the lexem at the specified column.
		/// </summary>
		/// <param name="column">Needed column.</param>
		/// <returns>Copy of the stack.</returns>
		public override ConfigStack GetStackByColumn( int column )
		{
			ILexem lexem = FindLexemByColumn( column );
			int index = ( lexem != null ) ? ( LineLexems.IndexOf( lexem ) ) : ( LineLexems.Count );

			if( index < 0 ) throw new ApplicationException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_98 );

			ConfigStack result = null;
			if( index == 0 )
			{
				result = ( ConfigStack )LineStartStack.Clone();
			}
			else
			{
				IEnumerator enumerator = Parser.GetEnumerator( LineStartStack, LineStartPoint );
				ILexemEnumeratorParserInfo enumeratorInfo = enumerator as ILexemEnumeratorParserInfo;
				int iNeedStackLength = enumeratorInfo.CurrentStack.Count - 1;
				int iCurrent = 0;

				if( enumeratorInfo == null )
					throw new NotSupportedException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_99 );

				while( iCurrent++ < index && enumerator.MoveNext() )
				{
					if( enumeratorInfo.CurrentStack.Count == iNeedStackLength )
					{
						break;
					}
				}

				if( enumeratorInfo.CurrentStack != null )
				{
					result = ( ConfigStack )enumeratorInfo.CurrentStack.Clone();
				}
			}
			return result;
		}
		/// <summary>
		/// Gets first line lexem that is not whitespace.
		/// </summary>
		/// <returns>First line lexem that is not whitespace.</returns>
		public Lexem GetFirstNonWitespaceLexem()
		{
			Lexem result = null;
			for( int i = 0; i < m_lexems.Count; i++ )
			{
				Lexem lexem = ( Lexem )m_lexems[ i ];
				if( lexem.Config.Type != FormatType.Whitespace )
				{
					result = lexem;
					break;
				}
			}
			return result;
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Gets stack at the end of line. Line will be reparsed.
		/// </summary>
		/// <returns>Stack at the end of line. It can be treated as start stack for the next line.</returns>
		protected override ConfigStack GetLineEndStack()
		{
			if( m_endStack == null )
			{
				long lLastPosition;
				GetLineLexems( out lLastPosition );
			}
			return m_endStack;
		}
		/// <summary>
		/// Collection of all lexems, that belongs to the line. Line will be reparsed.
		/// </summary>
		/// <returns>List of line lexems.</returns>
		protected IList GetLineLexems()
		{
			long outData;
			return GetLineLexems( out outData );
		}
		/// <summary>
		/// Collection of all lexems, that belongs to the line. Line will be reparsed.
		/// </summary>
		/// <returns>List of line lexems.</returns>
		protected override IList GetLineLexems( out long lPositionBeforeNewLine )
		{
			IList lexems = m_lexems;
			if( !this.Parsed )
			{
				m_lexems = base.GetLineLexems( out lPositionBeforeNewLine );
				m_endStack = m_parser.GetStackCopy();

				DisposeEndPoint();
				DisposePointBeforeNewline();

				m_endPoint = m_parser.BaseStream.GetParsePoint( m_parser.BaseStream.Position );
				m_endPoint.ParsePointParameterChanged += new ParsePointParameterChangedEventHandler( OnEndPointOffsetChanged );
				m_endPoint.Deleted += new ParsePointDeletedEventHandler( m_endPoint_Deleted );

				lexems = m_lexems;

				m_positionBeforeNewLine = m_parser.BaseStream.GetParsePoint( lPositionBeforeNewLine );
				m_positionBeforeNewLine.ParsePointParameterChanged += new ParsePointParameterChangedEventHandler( OnPositionBeforeNewLineOffsetChanged );
				m_positionBeforeNewLine.Deleted += new ParsePointDeletedEventHandler( OnPositionBeforeNewLineDeleted );

				if( m_endPoint == null )
					throw new NullReferenceException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_97 );

				ILexemLine lineNext = m_parser.FindLineInCache( LineIndex + 1 );
				if( lineNext != null && !lineNext.LineStartStack.Equals( m_endStack ) )
				{
					m_parser.ResetLines( lineNext, true );
				}
			}

			lPositionBeforeNewLine = m_positionBeforeNewLine.Offset;
			return lexems;
		}
		#endregion

		#region Nonpublic Methods
		/// <summary>
		/// Frees LineEndPoint.
		/// </summary>
		protected void DisposeEndPoint()
		{
			if( m_endPoint != null )
			{
				m_endPoint.ParsePointParameterChanged -= new ParsePointParameterChangedEventHandler( OnEndPointOffsetChanged );
				m_endPoint.Deleted -= new ParsePointDeletedEventHandler( m_endPoint_Deleted );
			}
			m_endPoint = null;
		}
		/// <summary>
		/// Disposes m_positionBeforeNewLine: detaches all event handlers.
		/// </summary>
		protected void DisposePointBeforeNewline()
		{
			if( m_positionBeforeNewLine != null )
			{
				m_positionBeforeNewLine.ParsePointParameterChanged -= new ParsePointParameterChangedEventHandler( OnPositionBeforeNewLineOffsetChanged );
				m_positionBeforeNewLine.Deleted -= new ParsePointDeletedEventHandler( OnPositionBeforeNewLineDeleted );
				m_positionBeforeNewLine = null;
			}
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Handler of OffsetChanged event of LineEndPoint.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnEndPointOffsetChanged( object sender, ParsePointParameterChangedEventArgs e )
		{
			if( e.OffsetChanged )
			{
				DisposeEndPoint();
			}
		}
		/// <summary>
		/// Handler of the Deleted event of the line end point.
		/// </summary>
		/// <param name="point"></param>
		/// <param name="lNewOffset"></param>
		private void m_endPoint_Deleted( ParsePoint point, long lNewOffset )
		{
			DisposeEndPoint();
		}
		/// <summary>
		/// Disposes m_positionBeforeNewLine.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPositionBeforeNewLineOffsetChanged( object sender, ParsePointParameterChangedEventArgs e )
		{
			if( e.OffsetChanged )
			{
				DisposePointBeforeNewline();
			}
		}
		/// <summary>
		/// Disposes m_positionBeforeNewLine.
		/// </summary>
		/// <param name="point"></param>
		/// <param name="lNewOffset"></param>
		private void OnPositionBeforeNewLineDeleted( ParsePoint point, long lNewOffset )
		{
			DisposePointBeforeNewline();
		}
		#endregion
	}
}