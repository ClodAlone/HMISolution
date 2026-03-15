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
using Syncfusion.Windows.Forms.Edit.Implementation.IO;
using Syncfusion.Windows.Forms.Edit.Implementation.Formatting;
using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Windows.Forms.Edit.Utils;

namespace Syncfusion.Windows.Forms.Edit.Implementation.Parser
{
	/// <summary>
	/// Lexem parser, that uses RenderedLine instead of LexemLine class.
	/// </summary>
	public class RenderableLexemParser
		: LexemParser
		, IPositionConverter
		, IDisposable
	{
		#region Constants
		/// <summary>
		/// Text of the exception that is raised when line was not measured.
		/// </summary>
		private const string DEF_ERROR_NOINFO = "Line has no info about it's sizes.";
		/// <summary>
		/// Width and height of the bitmap used for creating default graphics.
		/// </summary>
		private const int DEF_DEFAULT_BITMAP_SIZE = 5;
        internal bool TooltipHide = false;
		#endregion

		#region Classes
		/// <summary>
		/// Comparer, used for the by-Y comparision of lines.
		/// </summary>
		private class ByYComparer
			: IComparer
		{
			#region IComparer Members
			/// <summary>
			/// Compares rendered line object with float value. 
			/// </summary>
			/// <param name="objLine">Rendered line</param>
			/// <param name="objY">float</param>
			/// <returns>Standart comparision results.</returns>
			public int Compare( object objLine, object objY )
			{
				RenderedLine line = objLine as RenderedLine;
				float y = ( float )objY;

				if( line.Y > y )
				{
					return 1;
				}
				else if( line.Y + line.Height <= y )
				{
					return -1;
				}
				else
				{
					return 0;
				}
			}
			#endregion
		}
		#endregion

		#region Fields
		/// <summary>
		/// Comparer, used to compare lines by y.
		/// </summary>
		private IComparer m_comparer = new ByYComparer();
		/// <summary>
		/// Maximum width of the lines. Used for word wrapping.
		/// </summary>
		private int m_MaxWidth;
		/// <summary>
		/// Default line height.
		/// </summary>
		private float m_DefaultLineHeight = -1;
		/// <summary>
		/// Default graphics object, used for measuring lines.
		/// </summary>
		private Graphics m_GraphicsDefault;
		/// <summary>
		/// Bitmap, used to create default graphics object for measuring lines.
		/// </summary>
		private Bitmap m_BitmapDefault;
		/// <summary>
		/// Indicates whether word wrapping should be performed by chars.
		/// </summary>
		private bool m_bCharWrap = false;
		/// <summary>
		/// Offset of paragraphs.
		/// </summary>
		private int m_iParagraphOffset = 0;
		/// <summary>
		/// Offset of wrapped lines.
		/// </summary>
		private int m_iWrappedLinesOffset = 0;
		/// <summary>
		/// Indicates whether native GDI should be used for text output.
		/// </summary>
		private bool m_bNativeDrawing;
		/// <summary>
		/// Space between lines.
		/// </summary>
		private int m_spaceBetweenLines = StreamEditControl.SPACE_BETWEEN_LINES;
		#endregion

		#region Properties
		/// <summary>
		/// Gets default height of the line.
		/// </summary>
		public float DefaultLineHeight
		{
			get
			{
				if( m_DefaultLineHeight == -1 )
				{
					m_DefaultLineHeight = ( Formats[ FormatType.Whitespace ] as Format ).MeasureText(
						GraphicsUtils.DefaultGraphics, "The quick brown fox jumps over the lazy dog.", true, this.UseNativeDrawing, this.SpaceBetweenLines ).Height;
				}
				return m_DefaultLineHeight;
			}
		}
		/// <summary>
		/// Gets or sets maximum width for measuring lines.
		/// </summary>
		public int MaxWidth
		{
			get
			{
				return m_MaxWidth;
			}
			set
			{
				m_MaxWidth = value;
			}
		}
		/// <summary>
		/// Gets or sets bool that indicates whether word wrapping should be performed by chars.
		/// </summary>
		public bool CharWrap
		{
			get
			{
				return m_bCharWrap;
			}
			set
			{
				m_bCharWrap = value;
			}
		}
		/// <summary>
		/// Gets or sets offset of paragraphs.
		/// </summary>
		public int ParagraphOffset
		{
			get
			{
				return m_iParagraphOffset;
			}
			set
			{
				if( value < 0 ) throw new ArgumentOutOfRangeException( "ParagraphOffset" );

				if( value != m_iParagraphOffset )
				{
					m_iParagraphOffset = value;
					DropMeasuringInfo();
				}
			}
		}
		/// <summary>
		/// Gets or sets offset of wrapped lines.
		/// </summary>
		public int WrappedLinesOffset
		{
			get
			{
				return m_iWrappedLinesOffset;
			}
			set
			{
				if( value < 0 ) throw new ArgumentOutOfRangeException( "WrappedLinesOffset" );

				if( value != m_iWrappedLinesOffset )
				{
					m_iWrappedLinesOffset = value;
					DropMeasuringInfo();
				}
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether native GDI should be used for text output.
		/// </summary>
		public bool UseNativeDrawing
		{
			get
			{
				return m_bNativeDrawing;
			}
			set
			{
				if( m_bNativeDrawing != value )
				{
					m_bNativeDrawing = value;
					this.DropMeasuringInfo();
				}
			}
		}
		/// <summary>
		/// Gets or sets space between lines.
		/// </summary>
		public int SpaceBetweenLines
		{
			get
			{
				return m_spaceBetweenLines;
			}
			set
			{
				if( m_spaceBetweenLines != value )
				{
					m_spaceBetweenLines = value;
					this.DropMeasuringInfo();
				}
			}
		}
		#endregion

		#region Initialization And Finalization
		/// <summary>
		/// Creates new instance of the parser and initializes it
		/// </summary>
		/// <param name="source">Input source</param>
		/// <param name="language">Language configuration</param>
		/// <param name="undoData"></param>
		internal RenderableLexemParser( StreamsWrapper source, IConfigLanguage language, UndoRedoData undoData )
			: base( source, language, undoData )
		{
			Init();
		}
		/// <summary>
		/// Creates new instance of the parser and initializes it
		/// </summary>
		/// <param name="source">Input source</param>
		/// <param name="language">Language configuration</param>
		public RenderableLexemParser( StreamsWrapper source, IConfigLanguage language )
			: base( source, language )
		{
			Init();
		}
		/// <summary>
		/// Initializes parser.
		/// </summary>
		private void Init()
		{
			m_BitmapDefault = new Bitmap( DEF_DEFAULT_BITMAP_SIZE, DEF_DEFAULT_BITMAP_SIZE );
			m_GraphicsDefault = Graphics.FromImage( m_BitmapDefault );
			BaseStream.UndoBufferFlushed += new EventHandler( OnSourceUndoBufferFlushed );
		}
		/// <summary>
		/// Calls Dispose method to dispose all used resources.
		/// </summary>
		~RenderableLexemParser()
		{
			Dispose();
		}
		/// <summary>
		/// Disposes all used resources.
		/// </summary>
		public void Dispose()
		{
			GC.SuppressFinalize( this );
			ClearEventHandlers();

			if( m_GraphicsDefault != null )
			{
				m_GraphicsDefault.Dispose();
			}
			if( m_BitmapDefault != null )
			{
				m_BitmapDefault.Dispose();
			}

			m_GraphicsDefault = null;
			m_BitmapDefault = null;
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Starts updating regions.
		/// </summary>
		protected override void BeginUpdateRegionsInternal()
		{
			base.BeginUpdateRegionsInternal();
		}
		/// <summary>
		/// Ends updating regions.
		/// </summary>
		protected override void EndUpdateRegionsInternal()
		{
			base.EndUpdateRegionsInternal();
			FixLineRenderingPositions();
		}
		/// <summary>
		/// Creates new instance of the RenderedLine class.
		/// </summary>
		/// <param name="pointLineStart">ParsePoint of the line start.</param>
		/// <param name="stack">Stack at the beginning of the line.</param>
		/// <returns>ILexemLine interface of the line</returns>
		protected override ILexemLine CreateLine( IParsePoint pointLineStart, ConfigStack stack )
		{
			if( pointLineStart == null ) throw new ArgumentNullException( "pointLineStart" );
			if( stack == null ) throw new ArgumentNullException( "stack" );

			RenderedLine line = new RenderedLine( this, pointLineStart, stack );
			line.Parsed = true;
			line.Height = this.DefaultLineHeight;
			line.SubLineHeight = new float[] { this.DefaultLineHeight };
			RaiseLineInstanceCreatedEvent( line );
			return line;
		}
		/// <summary>
		/// Creates new lexem. Can be overriden.
		/// </summary>
		/// <param name="text">Text for the lexem.</param>
		/// <param name="config">Config of lexem.</param>
		/// <returns>New lexem.</returns>
		protected override Lexem CreateLexem( string text, IConfigLexem config )
		{
			if( text == null ) throw new ArgumentNullException( "text" );
			if( text == string.Empty )
				throw new ArgumentOutOfRangeException( "text", text, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_10 );
			if( config == null ) throw new ArgumentNullException( "config" );

			return new RenderedLexem( text, config );
		}
		/// <summary>
		/// Parses line, that is next to given one. Cache is not used.
		/// </summary>
		/// <param name="line">Current line.</param>
		/// <returns>New line.</returns>
		protected override ILexemLine GetNextLine( ILexemLine line )
		{
			if( line == null ) throw new ArgumentNullException( "line" );

			RenderedLine oldLine = line as RenderedLine;
			RenderedLine newLine = base.GetNextLine( line ) as RenderedLine;

			if( newLine != null )
			{
				newLine.Y = oldLine.Y + oldLine.Height;
			}

			return newLine;
		}
		/// <summary>
		/// On every collapsing/uncollapsing of region moves graphical positions of the lines.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">EventArgs.</param>
		protected override void OnRegNewCollapsedStateChanged( object sender, EventArgs e )
		{
			if( CollapsingEnabled )
			{
				base.OnRegNewCollapsedStateChanged( sender, e );
				FixLineRenderingPositions();
			}
		}
		/// <summary>
		/// Makes all needed updates after changing CollapsingEnabled state.
		/// </summary>
		protected override void OnCollapsingEnabledChanged()
		{
			base.OnCollapsingEnabledChanged();
			FixLineRenderingPositions();
		}
		/// <summary>
		/// Creates lexem line with plain text formatting. Also calls FixLineRenderingPositions methods.
		/// </summary>
		/// <param name="iLineIndexVirtual">Virtual line index.</param>
		/// <param name="iLineIndexPhysical">Phisical line index.</param>
		/// <param name="index">Index of the line in the lines list.</param>
		/// <returns>newly created lexem line.</returns>
		protected override ILexemLine CreatePlainTextLine( int iLineIndexVirtual, int iLineIndexPhysical, int index )
		{
			ILexemLine line = base.CreatePlainTextLine( iLineIndexVirtual, iLineIndexPhysical, index );
			FixLineRenderingPositions();
			return line;
		}
		/// <summary>
		/// Checks line list integrity.
		/// </summary>
		protected override void CheckConsistence()
		{
			if( !ConsistenceChecksLocked )
			{
				float y = float.MinValue;
				for( int i = 0; i < LinesList.Count; i++ )
				{
					RenderedLine line = ( RenderedLine )LinesList[ i ];

					if( line.Y <= y ) throw new Exception( "Integrity check failure" );

					y = line.Y;
				}

				base.CheckConsistence();
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Changes dpi of the graphics object, used for measuring lines. If dpi is not equal to the currently used, new graphics object 
		/// is created and all lines are remeasured.
		/// </summary>
		/// <param name="g">Graphics object with desired resolution.</param>
		public void SetDPIFromGraphics( Graphics g )
		{
			if( g == null ) throw new ArgumentNullException( "g" );

			if( g.DpiX != m_GraphicsDefault.DpiX || g.DpiY != m_GraphicsDefault.DpiY )
			{
				if( m_GraphicsDefault != null )
				{
					m_GraphicsDefault.Dispose();
				}
				if( m_BitmapDefault != null )
				{
					m_BitmapDefault.Dispose();
				}

				m_BitmapDefault = new Bitmap( DEF_DEFAULT_BITMAP_SIZE, DEF_DEFAULT_BITMAP_SIZE, g );
				m_GraphicsDefault = Graphics.FromImage( m_BitmapDefault );
				RemeasureLines();
			}
		}
		/// <summary>
		/// Checks whether line was rendered and raises exception if it was not.
		/// </summary>
		/// <param name="line">Line to be checked.</param>
		public static void CheckLine( RenderedLine line )
		{
			if( !line.IsMeasured ) throw new Exception( DEF_ERROR_NOINFO );
		}
		/// <summary>
		/// Measures line if needed.
		/// </summary>
		/// <param name="line">Line to be measured.</param>
		public void MeasureLine( RenderedLine line )
		{
			line.Parsed = true;
			if( !line.IsMeasured )
			{
				line.Measure( m_GraphicsDefault, m_MaxWidth, m_bCharWrap, TabLength, m_iParagraphOffset, m_iWrappedLinesOffset );
			}
		}
		/// <summary>
		/// Gets line by specified y position.
		/// </summary>
		/// <param name="y">Y position.</param>
		/// <returns>RenderedLine object.</returns>
		public RenderedLine GetLineByY( float y )
		{
			return GetLineByY( y, int.MaxValue );
		}
		/// <summary>
		/// Gets line by specified y position.
		/// </summary>
		/// <param name="y">Y position.</param>
		/// <param name="iMaxLines">Max. number of lines.</param>
		/// <returns>RenderedLine object.</returns>
		public RenderedLine GetLineByY( float y, int iMaxLines )
		{
			if( y < 0 )
				throw new ArgumentOutOfRangeException( "y", y, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_62 );

			RenderedLine result = null;

			DateTime timeStart = DateTime.Now;

			FixLineRenderingPositions(); // fix for def. OT7001

			int index = this.LinesList.BinarySearch( y, m_comparer );
			if( index >= 0 )
			{
				result = this.LinesList[ index ] as RenderedLine;
			}
			else
			{
				index = ~index - 1;
				RenderedLine firstLine;

				// if there is no lines found in cache that are before needed one.
				if( index < 0 )
				{
					firstLine = CreateLine( this.BaseStream.GetParsePoint( 1, 1, true ), CreateDefaultStack() ) as RenderedLine;
					InsertLexemLineIntoList( firstLine, 0 );
					index = 0;
				}
				else
				{
					firstLine = this.LinesList[ index ] as RenderedLine;
				}

				ILongOperation operation = null;
				bool bParsingSpeedUp = ( this.ParsingMode != TextParsingMode.FullParsing );

				if( ( y - firstLine.Y ) / this.DefaultLineHeight > 200 )
				{
					operation = StartOperation( "Parsing" );
				}

				while( firstLine.Y < y )
				{
					index++;

					TimeSpan timeSpent = DateTime.Now - timeStart;

					if( bParsingSpeedUp && ( y - firstLine.Y > this.DefaultLineHeight * DEF_LINES_BEFORE_SPEEDUP ) )
					{
						int iLine = ( int )Math.Floor( ( y - firstLine.Y - firstLine.Height ) / this.DefaultLineHeight );
						iLine += firstLine.LineIndex;

						int iLineNeeded = firstLine.LineStartPoint.Line + ( iLine - firstLine.LineIndex );

						iLine = Math.Min( iLine, this.TotalLines );
						iLineNeeded = Math.Min( iLineNeeded, this.TotalLines );

						if( iLineNeeded > firstLine.LineIndex )
						{
							firstLine = ( RenderedLine )CreatePlainTextLine( iLine, iLineNeeded, index );
							break;
						}
					}

					iMaxLines--;
					double indexDivided = index / 100.0;
					firstLine = GetNextLine( firstLine ) as RenderedLine;

					if( firstLine == null || iMaxLines <= 0 )
					{
						break;
					}

					if( Math.Round( indexDivided ) == ( indexDivided ) )
					{
						MeasureLine( ( RenderedLine )firstLine );
					}

					ILexemLine lineNext = ( index > 0 && index < this.LinesList.Count - 1 ) ? ( ( ILexemLine )this.LinesList[ index ] ) : ( null );

					if( null != lineNext && lineNext.LineIndex <= firstLine.LineIndex )
					{
						FixLineRenderingPositions();
						break;
					}

					InsertLexemLineIntoList( firstLine, index );
				}

				if( operation != null )
				{
					operation.Stop();
				}

				if( iMaxLines > 0 )
				{
					result = firstLine;
				}
			}

			return result;
		}
		/// <summary>
		/// Fixes y coordinates of all lines. All unparsed lines are treated to have default height.
		/// </summary>
		public void FixLineRenderingPositions()
		{
			if( !this.IsCollapsingLocked )
			{
				IEnumerator enumerator = GetLineEnumerator();
				int iLastLine = 0;
				// Position for new line.
				float lastY = 0;

				while( enumerator.MoveNext() )
				{
					if( enumerator.Current == null )
						throw new ApplicationException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_63 );

					RenderedLine line = enumerator.Current as RenderedLine;

					if( line == null ) throw new ApplicationException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_64 );

					float skippedHeight = ( line.LineIndex - iLastLine - 1 ) * this.DefaultLineHeight;
					line.Y = lastY + skippedHeight;

					float fHeight = line.Height;

					lastY = line.Y + fHeight;
					iLastLine = line.LineIndex;
				}
			}
		}
		/// <summary>
		/// Drops measuring info for all cached lines.
		/// </summary>
		public void DropMeasuringInfo()
		{
			foreach( RenderedLine line in LinesList )
			{
				line.DropMeasureInfo();
			}
		}
		/// <summary>
		/// Drops measuring info and remeasures all cached lines.
		/// </summary>
		public void RemeasureLines()
		{
			DropMeasuringInfo();

			foreach( RenderedLine line in LinesList )
			{
				line.Measure( m_GraphicsDefault, m_MaxWidth, m_bCharWrap, this.TabLength, m_iParagraphOffset, m_iWrappedLinesOffset );
			}

			FixLineRenderingPositions();
		}
		#endregion

		#region IPositionConverter Members
		/// <summary>
		/// Converts Virtual position to physical positions.
		/// </summary>
		/// <param name="point">Virtual point.</param>
		/// <returns><see cref="IParsePoint"/> with reference to physical coordinates.</returns>
		public IParsePoint VirtualToPhysical( Point point )
		{
			IParsePoint ppoint = GetParsePoint( point.Y, point.X );
			return ppoint;
		}
		/// <summary>
		/// Converts physical coordinates to virtual.
		/// </summary>
		/// <param name="point">ParsePoint with physical position.</param>
		/// <returns>Point with virtual position.</returns>
		public Point PhysicalToVirtual( IParsePoint point )
		{
			if( point == null ) throw new ArgumentNullException( "point" );

			CoordinatePoint cpoint = GetCoordinatePoint( point );

			if( cpoint == null )
				throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_65 );

			return new Point( cpoint.VirtualColumn, cpoint.VirtualLine );
		}
		/// <summary>
		/// Converts graphical coordinates to virtual.
		/// </summary>
		/// <param name="point">Point with graphical position.</param>
		/// <returns>Point with virtual position.</returns>
		public Point GraphicalToVirtual( Point point )
		{
			return GraphicalToVirtual( point, true );
		}
		/// <summary>
		/// Converts graphical coordinates to virtual.
		/// </summary>
		/// <param name="point">Point with graphical position.</param>
		/// <param name="allowWhiteSpace">Specifies if whitespace after last character in line should be treated like regular characters.</param>
		/// <returns>Point with virtual position.</returns>
		public Point GraphicalToVirtual( Point point, bool allowWhiteSpace )
		{
			Point result = Point.Empty;

			if( point.X >= 0 && point.Y >= 0 )
			{
                TooltipHide = false;
				int y = point.Y;
				RenderedLine ln = GetLineByY( y );
				if( ln == null )
				{
                    TooltipHide = true;
					// May be cursor is set after the text.
					// Then we have to set cursor to the lasty line of the text.
					ln = GetLine( this.TotalLines ) as RenderedLine;
					if( ln.Y + ln.Height < point.Y )
					{
						y = ( int )Math.Round( ln.Y + ln.Height / 2 );
					}
				}

				MeasureLine( ln );

				int x = point.X;

				IList lexems = ln.LineLexems;
				IRenderedLexem lexem = ln.FindLexemByX( x, y - ln.Y ) as IRenderedLexem;
				if( lexem != null )
				{
					// If it is not in virtual space.
					TextInfo info = ( lexem.Config.Format as Format ).MeasureText(
						GraphicsUtils.DefaultGraphics, lexem.Text, false, this.UseNativeDrawing, this.SpaceBetweenLines );
					for( int i = 0; i < info.Characters.Length; i++ )
					{
						float charLeft = info.Characters[ i ].CharLeft + lexem.XOffset;
						float charWidth = info.Characters[ i ].CharWidth;

						if( charLeft <= x && x < charLeft + charWidth )
						{
							result.Y = ln.LineIndex;
							result.X = lexem.Column + i;
							return result;
						}
					}

					result.Y = ln.LineIndex;
					result.X = lexem.Column + ( ( x > lexem.XOffset ) ? lexem.Length : 0 );
				}
				else
				{
					// If it is in virtual space.
					TextInfo info = ( Formats[ FormatType.Whitespace ] as Format ).MeasureText(
						GraphicsUtils.DefaultGraphics, " ", true, this.UseNativeDrawing, this.SpaceBetweenLines );
					float whitespaceWidth = info.Width;

					IRenderedLexem lastLexem = ln.GetLastSublineLexem( y - ln.Y );
					bool bAfterText = ( lastLexem == null || lastLexem.SubLine == ln.SubLineHeight.Length - 1 );

					result.Y = ln.LineIndex;

					float firstX = ( lastLexem != null ) ? ( lastLexem.XOffset + lastLexem.Width ) : ( 0 );
					int iWhitespacesCount = ( int )Math.Floor( ( point.X - firstX ) / whitespaceWidth );
					result.X = ( lastLexem != null ) ? ( lastLexem.Column + lastLexem.Length ) : ( 1 );

					if( bAfterText && allowWhiteSpace )
					{
						result.X += iWhitespacesCount;
					}
				}
			}

			return result;
		}
		/// <summary>
		/// Converts virtual coordinates to graphical.
		/// </summary>
		/// <param name="point">Point with virtual position.</param>
		/// <returns>Rectangle, occupied by character in given coordinates.</returns>
		public RectangleF VirtualToGraphical( Point point )
		{
			int iColumn = point.X;
			int iLine = point.Y;
			RectangleF result = Rectangle.Empty;
			float offset = 0;
			RenderedLine line = GetLine( iLine ) as RenderedLine;

			if( line == null )
			{
				return Rectangle.Empty;
			}

			MeasureLine( line );
			IRenderedLexem lexemWithColumn = line.FindLexemByColumn( iColumn ) as IRenderedLexem;

			if( lexemWithColumn == null )
			{
				IList lexems = line.LineLexems;
				// Get last lexem in line, needed if line have multiple sub-lines
				IRenderedLexem lastLexem = ( lexems.Count > 0 ) ? ( ( IRenderedLexem )lexems[ lexems.Count - 1 ] ) : ( null );
				// Offset of the last lexem
				float lastLexemX = ( lastLexem != null ) ? ( lastLexem.XOffset ) : ( 0 );
				// Y of the last lexem
				float lineY = line.Y + ( ( lastLexem != null ) ? ( lastLexem.YOffset ) : ( 0 ) );
				// Height of the last sub-line
				float subLineHeight = ( lastLexem != null ) ? ( line.GetSubLineTextHeight( lastLexem.SubLine ) ) : ( line.GetTextHeight() );
				// Width of the last lexem
				float lastLexemWidth = ( lastLexem != null ) ? ( lastLexem.Width ) : ( 0 );
				// Width of the space
				float spaceWidth = ( Formats[ "Whitespace" ] as Format ).MeasureText(
					GraphicsUtils.DefaultGraphics, " ", true, this.UseNativeDrawing, this.SpaceBetweenLines ).Width;
				// Index of the first column in virtual space in current line
				int firstVirtualColumn = ( lastLexem != null ) ? ( lastLexem.Column + lastLexem.Length ) : ( 1 );
				// Count of virtual spaces
				int spacesCount = iColumn - firstVirtualColumn;

				result.Y = lineY;
				result.Width = spaceWidth;
				result.Height = subLineHeight;
				result.X = offset + lastLexemX + lastLexemWidth + spacesCount * spaceWidth;
			}
			else
			{
				TextInfo txtInfo = ( lexemWithColumn.Config.Format as Format ).MeasureText(
					GraphicsUtils.DefaultGraphics, lexemWithColumn.Text, false, this.UseNativeDrawing, this.SpaceBetweenLines );
				CharInfo charInfo = txtInfo.Characters[ iColumn - lexemWithColumn.Column ];
				result.X = offset + lexemWithColumn.XOffset + charInfo.CharLeft;
				result.Width = charInfo.CharWidth;
				result.Y = line.Y + lexemWithColumn.YOffset;
				result.Height = line.GetSubLineTextHeight( lexemWithColumn.SubLine );
			}

			return result;
		}
		/// <summary>
		/// Corrects virtual coordinates.
		/// </summary>
		/// <param name="point">Virtual coordinates to be corrected.</param>
		/// <param name="virtualSpaceEnabled">Specifies whether virtual space is enabled.</param>
		/// <returns>Virtual point with correct coordinates.</returns>
		public Point CorrectVirtual( Point point, bool virtualSpaceEnabled )
		{
			Point result = point;
			result.Y = Math.Max( 1, Math.Min( result.Y, TotalLines ) );
			result.X = Math.Max( 1, result.X );

			if( !virtualSpaceEnabled )
			{
				RenderedLine line = GetLine( result.Y ) as RenderedLine;

				MeasureLine( line );

				result.X = Math.Min( line.LineLength, result.X );

				RenderedLexem lexem = line.FindLexemByColumn( result.X ) as RenderedLexem;

				if( lexem != null && lexem.Config.Format.UseCustomControl )
					result.X = lexem.Column;
			}

			return result;
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Sets all lines to unchanged state.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSourceUndoBufferFlushed( object sender, EventArgs e )
		{
			foreach( RenderedLine line in LinesList )
			{
				line.ResetChanged();
			}
		}
		#endregion
	}
}