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
using System.Drawing;

using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Utils;
using Syncfusion.Windows.Forms.Edit.Implementation.Formatting;
using Syncfusion.Windows.Forms.Edit.Enums;

namespace Syncfusion.Windows.Forms.Edit.Implementation.Parser
{
	/// <summary>
	/// Lexem line, that contains information, needed for faster rendering.
	/// </summary>
	public class RenderedLine
		: LexemLine
		, IDisposable
		, IEnumerable
	{
		#region Fields
		/// <summary>
		/// Line render position
		/// </summary>
		private float m_y;
		/// <summary>
		/// Height of the line
		/// </summary>
		private float m_height;
		/// <summary>
		/// Array of heights of the sublines. SubLines indexes start from 0.
		/// </summary>
		private float[] m_SubLineHeight;
		/// <summary>
		/// Flag that determines whether line was already measured.
		/// </summary>
		private bool m_bMeasured;
		/// <summary>
		/// Line width.
		/// </summary>
		private float m_width;
		/// <summary>
		/// Indicates whether line was changed after the last save.
		/// </summary>
		private bool m_bChanged;
		/// <summary>
		/// Is set to true if line is changed and there's no other way to know about it than just directly set it.
		/// </summary>
		private bool m_bForcedChanged;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets line render position.
		/// </summary>
		public float Y
		{
			get
			{
				return m_y;
			}
			set
			{
				if( m_y != value )
				{
					m_y = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets height of the line
		/// </summary>
		public float Height
		{
			get
			{
				return m_height;
			}
			set
			{
				m_height = value;
			}
		}
		/// <summary>
		/// Gets or sets array of heights of the sublines. SubLines indexes start from 0.
		/// </summary>
		public float[] SubLineHeight
		{
			get
			{
				if( m_SubLineHeight == null )
				{
					m_SubLineHeight = new float[] { m_height };
				}
				return m_SubLineHeight;
			}
			set
			{
				m_SubLineHeight = value;
			}
		}
		/// <summary>
		/// Gets length of the line.
		/// </summary>
		public new int LineLength
		{
			get
			{
				long lDummy;
				IList lexems = GetLineLexems( out lDummy );
				// Get last lexem in line, needed if line have multiple sub-lines
				IRenderedLexem lastLexem = ( lexems.Count > 0 ) ? ( IRenderedLexem )lexems[ lexems.Count - 1 ] : null;
				return ( lastLexem != null ) ? ( lastLexem.Column + lastLexem.Length ) : ( 1 );
			}
		}
		/// <summary>
		/// Gets flag that determines whether line was already measured.
		/// </summary>
		public bool IsMeasured
		{
			get
			{
				return m_bMeasured;
			}
		}
		/// <summary>
		/// Flag that determines, whether line is parsed. If line is parsed, than LineEndStack property contains Stack for the end of the line
		/// and LineLexems collection contains all lexems, that belong to current line. If line was changed, than Parsed will be set to false.
		/// </summary>
		public override bool Parsed
		{
			get
			{
				return base.Parsed;
			}
			set
			{
				if( value != base.Parsed )
				{
					m_bMeasured = false;
					base.Parsed = value;
				}
			}
		}
		/// <summary>
		/// Gets width of the line.
		/// </summary>
		public float Width
		{
			get
			{
				RenderableLexemParser.CheckLine( this );
				return m_width;
			}
		}
		/// <summary>
		/// Gets bool indicating whether line was changed after the last save.
		/// </summary>
		public bool Changed
		{
			get
			{
				return m_bChanged;
			}
		}
		/// <summary>
		/// Gets count of sublines.
		/// </summary>
		public int SubLinesCount
		{
			get
			{
				return m_SubLineHeight.Length;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether line is changed. Is set to true if line is changed and there's no other way to know about it
		/// than just directly set it.
		/// </summary>
		internal bool ForcedChanged
		{
			get
			{
				return m_bForcedChanged;
			}
			set
			{
				m_bForcedChanged = value;
				m_bChanged = value;
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
		public RenderedLine( LexemParser parser, IParsePoint pointLineStart, ConfigStack stack )
			: base( parser, pointLineStart, stack )
		{
			pointLineStart.ParsePointParameterChanged += new ParsePointParameterChangedEventHandler( OnPointLineStartLineChanged );
		}
		/// <summary>
		/// Disposes object.
		/// </summary>
		public override void Dispose()
		{
			if( IsValid )
			{
				LineStartPoint.ParsePointParameterChanged -= new ParsePointParameterChangedEventHandler( OnPointLineStartLineChanged );
			}
			base.Dispose();
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Enumerator of the lexems.
		/// </summary>
		/// <returns>Enumerator.</returns>
		public IEnumerator GetEnumerator()
		{
			return LineLexems.GetEnumerator();
		}
		/// <summary>
		/// Measures line. Sets lexems parameters correctly.
		/// </summary>
		/// <param name="g">Graphics object.</param>
		/// <param name="maxSize">Maximum width of the line.</param>
		/// <param name="bCharWrap">Indicates whether line should be wrapped by chars.</param>
		/// <param name="tabSize">Number of positions in tab symbol.</param>
		/// <param name="iParagraphOffset">Offset of paragraphs.</param>
		/// <param name="iWrappedLinesOffset">Offset of wrapped lines.</param>
		/// <returns>Measured size.</returns>
		public SizeF Measure( Graphics g, float maxSize, bool bCharWrap, int tabSize, int iParagraphOffset, int iWrappedLinesOffset )
		{
			SizeF result = new SizeF( 0, 0 );
			float currentX = 0;
			float currentLineHeight = ( ( RenderableLexemParser )Parser ).DefaultLineHeight;
			ArrayList heightsList = new ArrayList( 2 );
			int currentColumn = 1;
			m_width = 0;
			bool bDivider = false;
			RenderableLexemParser parser = ( RenderableLexemParser )m_parser;

			IList lexems = LineLexems;
			UniteCharWrappedLexems( lexems );

			if( lexems.Count == 0 )
			{
				TextInfo info = ( ( Format )m_parser.Formats[ FormatType.Whitespace ] ).MeasureText(
					g, " ", true,  parser.UseNativeDrawing,  parser.SpaceBetweenLines );
				currentLineHeight = ( float )Math.Ceiling( info.Height );
			}

			for( int i = 0; i < lexems.Count; i++ )
			{
				RenderedLexem lexem = ( RenderedLexem )lexems[ i ];

				bDivider |= ( lexem.Config.ContentDivider && ( !lexem.Config.IsComplex || lexem.Config.IsEqualToEnd( lexem.Text ) ) );
				TextInfo info = ( ( Format )lexem.Config.Format ).MeasureText( g, lexem.Text, true, parser.UseNativeDrawing, parser.SpaceBetweenLines );

				if( lexem.IsPartOfCharWrap )
				{
					currentX = 0;
					result.Height += currentLineHeight;
					heightsList.Add( currentLineHeight );
					currentLineHeight = ( ( RenderableLexemParser )Parser ).DefaultLineHeight;
				}

				lexem.Column = currentColumn;
				int offset = ( heightsList.Count > 0 ) ? ( iWrappedLinesOffset ) : ( iParagraphOffset );

				// If lexem has to be char wrapped.
				if( ( bCharWrap || 0 == currentX ) && ( currentX + info.Width + offset > maxSize ) )
				{
					RenderedLexem newLexem = WrapLexem( lexem, g, maxSize - currentX - offset, ref info );
					if( null != newLexem )
					{
						lexems.Insert( i + 1, newLexem );
					}
				}

				if( ( currentX == 0 || currentX + info.Width + offset <= maxSize ) )
				{
					lexem.XOffset = currentX + offset;
					currentX += info.Width;
				}
				else
				{
					if( info.Width + iWrappedLinesOffset > maxSize )
					{
						RenderedLexem newLexem = WrapLexem( lexem, g, maxSize - iWrappedLinesOffset, ref info );
						if( null != newLexem )
						{
							lexems.Insert( i + 1, newLexem );
						}
					}

					currentX = info.Width;
					lexem.XOffset = iWrappedLinesOffset;
					result.Height += currentLineHeight;
					heightsList.Add( currentLineHeight );
					currentLineHeight = ( ( RenderableLexemParser )Parser ).DefaultLineHeight;
				}

				currentColumn += info.Length;

				// Update line width.
				if( currentX > m_width )
				{
					m_width = currentX;
				}

				result.Width = Math.Max( currentX, result.Width );
				currentLineHeight = ( float )Math.Ceiling( info.Height );

				// Lexem info update
				lexem.Width = info.Width;
				lexem.YOffset = result.Height;
				lexem.SubLine = heightsList.Count;
			}

			if( bDivider )
			{
				currentLineHeight++;
			}

			result.Height += currentLineHeight;
			heightsList.Add( currentLineHeight );

			SubLineHeight = ( float[] )heightsList.ToArray( typeof( float ) );
			Height = result.Height;
			m_bMeasured = true;

			long lStartOffset = this.LineStartPoint.Offset;

			// For to remember about previous line when enter was pressed at the first column.
			if( this.LineIndex > 1 )
			{
				lStartOffset--;
			}

			m_bChanged = ( m_bForcedChanged || m_parser.BaseStream.RangeChanged(
				this.LineStartPoint.Offset - ( ( this.LineIndex == 1 ) ? ( 0 ) : ( 1 ) ), this.PointBeforeNewline.Offset ) );

			return result;
		}
		/// <summary>
		/// Wraps lexem into two parts; first part has maximal width that can be drawn in available space.
		/// </summary>
		/// <param name="lexem">Lexem that as to be wrapped.</param>
		/// <param name="g">Graphics object used for lexems measuring.</param>
		/// <param name="availableSpace">Width of available space.</param>
		/// <param name="info">TextInfo object what will be filled with information about new lexem.</param>
		/// <returns>Lexem that is the second part of the initial lexem.</returns>
		public RenderedLexem WrapLexem( IRenderedLexem lexem, Graphics g, float availableSpace, ref TextInfo info )
		{
			if( null == lexem ) throw new ArgumentNullException( "lexem" );
			if( null == g ) throw new ArgumentNullException( "g" );

			string text = lexem.Text;
			Format format = ( Format )lexem.Config.Format;
			info = format.MeasureText( g, text, false, ( ( RenderableLexemParser )m_parser ).UseNativeDrawing, ( ( RenderableLexemParser )m_parser ).SpaceBetweenLines );
			CharInfo[] chars = info.Characters;

			// Width of first part.
			float tempWidth = 0;
			int j = 0;

			for( int length = chars.Length; j < length; j++ )
			{
				tempWidth += chars[ j ].CharWidth;
				// If needed substring found.
				if( tempWidth > availableSpace )
				{
					break;
				}
			}

			// If substring's length isn't 0.
			RenderedLexem result = null;
			if( j != 0 )
			{
				// Second part of divided text.
				string newText = text.Substring( j, info.Length - j );
				// First part of divided text.
				( ( Lexem )lexem ).Text = text.Substring( 0, j );
				info = format.MeasureText( g, lexem.Text, true, ( ( RenderableLexemParser )m_parser ).UseNativeDrawing, ( ( RenderableLexemParser )m_parser ).SpaceBetweenLines );
				RenderedLexem newLexem = new RenderedLexem( newText, lexem.Config );
				newLexem.Collapser = lexem.Collapser;
				newLexem.IsPartOfCharWrap = true;
				result = newLexem;
			}
			return result;
		}
		/// <summary>
		/// Unite lexems wrapped by char wrap.
		/// </summary>
		/// <param name="lexems">List of lexems.</param>
		public void UniteCharWrappedLexems( IList lexems )
		{
			if( null == lexems ) throw new ArgumentNullException( "lexems" );

			for( int i = lexems.Count - 1; i >= 0; i-- )
			{
				RenderedLexem lexem = ( RenderedLexem )lexems[ i ];
				if( lexem.IsPartOfCharWrap )
				{
					if( i == 0 ) throw new Exception( "First lexem can't be part of char wrap" );

					RenderedLexem uniteLexem = ( RenderedLexem )lexems[ i - 1 ];

					if( lexem.Config != uniteLexem.Config ) throw new Exception( "Lexems must have same configs" );

					uniteLexem.Unite( lexem );
					lexems.RemoveAt( i );
				}
			}
		}
		/// <summary>
		/// Drops all information about measuring.
		/// </summary>
		public void DropMeasureInfo()
		{
			m_bMeasured = false;
		}
		/// <summary>
		/// Searches lexem, that contains given column index.
		/// </summary>
		/// <param name="column">Needed column.</param>
		/// <returns>Found lexem, or null if needed column is in virtual space.</returns>
		public override ILexem FindLexemByColumn( int column )
		{
			ILexem result = null;
			foreach( IRenderedLexem lexem in this )
			{
				int length = m_parser.GetLexemLength( lexem );

				if( column >= lexem.Column && column < lexem.Column + length )
				{
					result = lexem;
					break;
				}
			}
			return result;
		}
		/// <summary>
		/// Searches lexem, that contains given x offset.
		/// </summary>
		/// <param name="x">Needed x offset.</param>
		/// <param name="yOffset">Offset by y. Used when line is word-wrapped, so it can occupy more then one line.</param>
		/// <returns>Found lexem, or null if needed offset is in virtual space.</returns>
		public ILexem FindLexemByX( float x, float yOffset )
		{
			foreach( IRenderedLexem lexem in this )
			{
				float lineHeight = this.SubLineHeight[ lexem.SubLine ];

				if( lexem.YOffset <= yOffset && lexem.YOffset + lineHeight >= yOffset )
				{
					if( x >= lexem.XOffset && x <= lexem.XOffset + lexem.Width )
						return lexem;

					// If there is no lexem at the point - return next lexem.
					if( lexem.XOffset > x ) return lexem;
				}
			}

			return null;
		}
		/// <summary>
		/// Collection of all lexems, that belongs to the line. Line will be reparsed.
		/// </summary>
		/// <returns>List of line lexems.</returns>
		protected override IList GetLineLexems( out long lPositionBeforeNewLine )
		{
			m_bMeasured = m_bMeasured && Parsed;
			return base.GetLineLexems( out lPositionBeforeNewLine );
		}
		/// <summary>
		/// Gets last lexem in the subline.
		/// </summary>
		/// <param name="y">Y offset of subline.</param>
		/// <returns>Last lexem in the subline with offset Y.</returns>
		public IRenderedLexem GetLastSublineLexem( float y )
		{
			IList lexems = this.LineLexems;

			if( 0 == lexems.Count ) return null;

			RenderedLexem lex = ( RenderedLexem )lexems[ 0 ];

			for( int i = 0, len = lexems.Count; i < len; i++ )
			{
				float height = this.SubLineHeight[ lex.SubLine ];

				if( i == len - 1 )
				{
					if( lex.YOffset <= y && lex.YOffset + height >= y ) return lex;
					else break;
				}

				RenderedLexem nextLex = ( RenderedLexem )lexems[ i + 1 ];

				if( nextLex.SubLine == lex.SubLine + 1 && lex.YOffset <= y && lex.YOffset + height >= y ) return lex;

				lex = nextLex;
			}

			return null;
		}
		/// <summary>
		/// Resets line's Changed state.
		/// </summary>
		internal void ResetChanged()
		{
			m_bChanged = false;
		}
		/// <summary>
		/// Get line's text height.
		/// </summary>
		/// <returns>Line's text height.</returns>
		public float GetTextHeight()
		{
			return this.Height - ( ( m_parser as RenderableLexemParser ).SpaceBetweenLines );
		}
		/// <summary>
		/// Get subline's text height.
		/// </summary>
		/// <param>Number of subline.</param>
		/// <returns>Subline's text height.</returns>
		public float GetSubLineTextHeight( int num )
		{
			return this.SubLineHeight[ num ] - ( m_parser as RenderableLexemParser ).SpaceBetweenLines;
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Handler of LineDeleted event.
		/// </summary>
		/// <param name="sender">Line, that was deleted.</param>
		/// <param name="e">Empty EventArgs.</param>
		protected void OnLineDelete( object sender, EventArgs e )
		{
			Dispose();
		}
		/// <summary>
		/// Handler of the LineChanged event of the LineStartPoint.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPointLineStartLineChanged( object sender, ParsePointParameterChangedEventArgs e )
		{
			if( e.LineChanged && IsValid )
			{
				int delta = e.NewLine - e.OldLine;
				LineIndex += delta;
				this.Y += delta * ( ( RenderableLexemParser )m_parser ).DefaultLineHeight;
				DropMeasureInfo();
			}
		}
		#endregion
	}
}