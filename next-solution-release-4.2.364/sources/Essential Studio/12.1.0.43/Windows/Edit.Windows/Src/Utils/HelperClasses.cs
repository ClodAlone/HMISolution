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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;

using Syncfusion.Windows.Forms.Edit.Implementation.Parser;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Windows.Forms.Edit.Implementation.Formatting;
using Syncfusion.Windows.Forms.Edit.Implementation.IO;

namespace Syncfusion.Windows.Forms.Edit.Utils
{
	/// <summary>
	/// Class that contains coordinates in two coordinate systems.
	/// </summary>
	public class CoordinatePoint
		: IDisposable
	{
		#region Fields
		/// <summary>
		/// Point in stream.
		/// </summary>
		private IParsePoint m_point;
		/// <summary>
		/// Virtual line.
		/// </summary>
		private int m_line;
		/// <summary>
		/// Virtual column.
		/// </summary>
		private int m_column;
		/// <summary>
		/// Parser.
		/// </summary>
		private ILexemParser m_parser;
		/// <summary>
		/// Value that indicates whether event handlers 
		/// should be attached to ParsePoint events.
		/// </summary>
		private bool b_AttachToEvents;
		#endregion

		#region Properties
		/// <summary>
		/// Link to point to stream.
		/// </summary>
		public IParsePoint PhysicalPoint
		{
			[DebuggerStepThrough]
			get
			{
				if( m_point == null && m_column > 0 && m_line > 0 && m_parser != null )
					UpdatePhisicalCoordinates();

				return m_point;
			}
		}
		/// <summary>
		/// Virtual line (Visible line on the screen).
		/// </summary>
		public int VirtualLine
		{
			get
			{
                if (m_line == 0 && m_point != null && m_parser != null)
                {
                    UpdateVirtualCoordinates();
                }
				
				return m_line;
			}
		}
		/// <summary>
		/// Virtual column (Visible column on the screen).
		/// </summary>
		public int VirtualColumn
		{
			[DebuggerStepThrough]
			get
			{
                if (m_column == 0 && m_point != null && m_parser != null)
                {
                    UpdateVirtualCoordinates();
                }                

				return m_column;
			}
		}
		/// <summary>
		/// Parser, coordinates belongs to.
		/// </summary>
		public ILexemParser Parser
		{
			[DebuggerStepThrough]
			get
			{
				return m_parser;
			}
		}
		/// <summary>
		/// Checks whether coordinate point is valid.
		/// </summary>
		public bool IsValid
		{
			[DebuggerStepThrough]
			get
			{
				return ( m_point != null && m_line > 0 && m_column > 0 );
			}
		}
		/// <summary>
		/// Gets or Sets value that indicates whether coordinate 
		/// point should handle changing of the position of ParsePoint.
		/// </summary>
		public bool AttachToEvents
		{
			[DebuggerStepThrough]
			get
			{
				return b_AttachToEvents;
			}
			set
			{
				if( value != b_AttachToEvents )
				{
					DetachEvents();
					b_AttachToEvents = value;
					AttachEvents();
				}
			}
		}
        /// <summary>
        /// Gets the virtual point.
        /// </summary>
        /// <value>The virtual point.</value>
		public Point VirtualPoint
		{
			get
			{
				return new Point( this.VirtualColumn, this.VirtualLine );
			}
		}
		#endregion

		#region Initialization & Finalization
		/// <summary>
		/// Creates and initializes new class instance.
		/// </summary>
		/// <param name="parser">Lexem parser.</param>
		/// <param name="point">ParsePoint, coordinate point is associated to. Can be null.</param>
		/// <param name="line">Virtual line number.</param>
		/// <param name="column">Virtual column number.</param>
		/// <param name="bAttachToEvents">Indicates whether point should be attached to text changing events and remain at the same place.</param>
		public CoordinatePoint( ILexemParser parser, IParsePoint point, int line, int column, bool bAttachToEvents )
		{
			m_parser = parser;
			m_point = point;
			m_line = line;
			m_column = column;

			if( m_point == null && ( line == 0 || column == 0 ) )
				throw new ApplicationException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_117 );

			b_AttachToEvents = bAttachToEvents;

			AttachEvents();
		}
		/// <summary>
		/// Creates and initializes new class instance.
		/// </summary>
		/// <param name="p">CoordinatePoint to create new instance from.</param>
		/// <param name="b_attachToEvents">Indicates whether point should be attached to text changing events and remain at the same place.</param>
		public CoordinatePoint( CoordinatePoint p, bool b_attachToEvents )
		{
			m_point = p.m_point;
			m_line = p.m_line;
			m_column = p.m_column;
			m_parser = p.m_parser;
			this.b_AttachToEvents = b_attachToEvents;

			AttachEvents();
		}
		/// <summary>
		/// Creates and initializes new class instance.
		/// </summary>
		/// <param name="p">CoordinatePoint to create new instance from.</param>
		public CoordinatePoint( CoordinatePoint p ) : this( p, false ) { }
		/// <summary>
		/// Disposer of the object.
		/// </summary>
		public void Dispose()
		{
			DetachEvents();
			m_point = null;
		}
		#endregion

		#region Events
		/// <summary>
		/// Event, that is raised when ParsePoint, the point is attached to, is deleted.
		/// </summary>
		public event CoordinatePointDeletedEventHandler Deleted;
		/// <summary>
		/// Event, that is raised when coordinate point coordinates are reset.
		/// </summary>
		public event EventHandler PointReset;
		#endregion

		#region Public Methods
		/// <summary>
		/// Updates virtual coordinates using physical position.
		/// </summary>
		public void UpdateVirtualCoordinates()
		{
			CheckParser();

			if( m_point == null )
				throw new ApplicationException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_115 );

			CoordinatePoint point = m_parser.GetCoordinatePoint( m_point, true );
			m_column = point.VirtualColumn;
			m_line = point.VirtualLine;
			point.Dispose();
		}
		/// <summary>
		/// Updates physical coordinates using virtual position.
		/// </summary>
		public void UpdatePhisicalCoordinates()
		{
			CheckParser();

			if( m_column <= 0 || m_line <= 0 )
				throw new ApplicationException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_116 );

			CoordinatePoint point = m_parser.GetNearestParsePointRight( m_line, m_column );
			SetPoint( point.PhysicalPoint );
			point.Dispose();
		}
		#endregion

		#region Nonpublic Methods
		/// <summary>
		/// Attaches events to parsepoint and parser.
		/// </summary>
		private void AttachEvents()
		{
			if( m_point != null && m_parser != null && AttachToEvents )
			{
				m_point.ParsePointParameterChanged += new ParsePointParameterChangedEventHandler( PointOffsetChanged );
				m_point.Deleted += new ParsePointDeletedEventHandler( PointDeleted );
				( ( RenderableLexemParser )m_parser ).OutliningCollapse += new OutliningEventHandler( CoordinatePoint_OutliningCollapse );
				( ( RenderableLexemParser )m_parser ).OutliningBeforeExpand += new OutliningCancellableEventHandler( CoordinatePoint_OutliningBeforeExpand );
			}
		}
		/// <summary>
		/// Detaches events from parsepoint and parser.
		/// </summary>
		private void DetachEvents()
		{
			if( m_point != null )
			{
				m_point.ParsePointParameterChanged -= new ParsePointParameterChangedEventHandler( PointOffsetChanged );
				m_point.Deleted -= new ParsePointDeletedEventHandler( PointDeleted );
				( ( RenderableLexemParser )m_parser ).OutliningCollapse -= new OutliningEventHandler( CoordinatePoint_OutliningCollapse );
				( ( RenderableLexemParser )m_parser ).OutliningBeforeExpand -= new OutliningCancellableEventHandler( CoordinatePoint_OutliningBeforeExpand );
			}
		}
		/// <summary>
		/// Detaches events from old ParsePoint and attaches to new one.
		/// </summary>
		/// <param name="point">New ParsePoint.</param>
		private void SetPoint( IParsePoint point )
		{
			if( m_point != null )
			{
				DetachEvents();
			}
			m_point = point;
			AttachEvents();
		}
		/// <summary>
		/// Checks parser availability.
		/// </summary>
		private void CheckParser()
		{
			if( m_parser == null )
				throw new NotSupportedException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_114 );
		}
		/// <summary>
		/// Tracks changes of ParsePoint's offset.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PointOffsetChanged( object sender, ParsePointParameterChangedEventArgs e )
		{
            if (e.OffsetChanged)
            {
                Reset();
                UpdateVirtualCoordinates();
            }
		}
		/// <summary>
		/// Detaches from the ParsePoint.
		/// </summary>
		/// <param name="point"></param>
		/// <param name="lNewOffset"></param>
		private void PointDeleted( ParsePoint point, long lNewOffset )
		{
			SetPoint( null );

			bool bAbsolutlyDead = !IsValid || m_parser == null;
			if( Deleted != null && bAbsolutlyDead )
			{
				Deleted( this, lNewOffset );
			}
			else if( !bAbsolutlyDead )
			{
				UpdatePhisicalCoordinates();
			}
		}
		/// <summary>
		/// Tracks changes of ParsePoint's offset.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CoordinatePoint_OutliningCollapse( object sender, CollapseEventArgs e )
		{
			Reset();
		}
		/// <summary>
		/// Resets position forcing it to be recalculated.
		/// </summary>
		private void Reset()
		{
			m_line = 0;
			m_column = 0;

			if( PointReset != null ) PointReset( this, EventArgs.Empty );
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Compares two <see cref="CoordinatePoint"/> instances by overrided == operator.
		/// </summary>
		/// <param name="obj">Object to compare.</param>
		/// <returns>Result of comparation.</returns>
		public override bool Equals( object obj )
		{
			return ( this == ( obj as CoordinatePoint ) );
		}
		/// <summary>
		/// Generates hashcode to be used in hashtables.
		/// </summary>
		/// <returns>Hash code.</returns>
		public override int GetHashCode()
		{
			return ( m_line ^ m_column );
		}
		/// <summary>
		/// Gets string representation of the coordinate point.
		/// </summary>
		/// <returns>String representation of the coordinate point.</returns>
		public override string ToString()
		{
			if( IsValid )
			{
				return string.Format( "{{VLine: {0}; VColumn: {1}}}", VirtualLine, VirtualColumn );
			}
			else
			{
				return "{Invalid}";
			}
		}

		#endregion

		#region Operator Overrides
		/// <summary>
		/// "Less than" operator.
		/// </summary>
		/// <param name="leftPoint">First point to compare.</param>
		/// <param name="rightPoint">Second point to compare.</param>
		/// <returns>Result of comparation.</returns>
		[DebuggerStepThrough]
		public static bool operator <( CoordinatePoint leftPoint, CoordinatePoint rightPoint )
		{
			return ( leftPoint.VirtualLine < rightPoint.VirtualLine ||
				( leftPoint.VirtualLine == rightPoint.VirtualLine && leftPoint.VirtualColumn < rightPoint.VirtualColumn ) );
		}
		/// <summary>
		/// "Bigger than" operator.
		/// </summary>
		/// <param name="leftPoint">First point to compare.</param>
		/// <param name="rightPoint">Second point to compare.</param>
		/// <returns>Result of comparation.</returns>
		[DebuggerStepThrough]
		public static bool operator >( CoordinatePoint leftPoint, CoordinatePoint rightPoint )
		{
			return ( leftPoint.VirtualLine > rightPoint.VirtualLine ||
				( leftPoint.VirtualLine == rightPoint.VirtualLine && leftPoint.VirtualColumn > rightPoint.VirtualColumn ) );
		}
		/// <summary>
		/// "Less than or equal" operator.
		/// </summary>
		/// <param name="leftPoint">First point to compare.</param>
		/// <param name="rightPoint">Second point to compare.</param>
		/// <returns>Result of comparation.</returns>
		[DebuggerStepThrough]
		public static bool operator <=( CoordinatePoint leftPoint, CoordinatePoint rightPoint )
		{
			return ( leftPoint.VirtualLine < rightPoint.VirtualLine ||
				( leftPoint.VirtualLine == rightPoint.VirtualLine && leftPoint.VirtualColumn <= rightPoint.VirtualColumn ) );
		}
		/// <summary>
		/// "Bigger than or equal" operator.
		/// </summary>
		/// <param name="leftPoint">First point to compare.</param>
		/// <param name="rightPoint">Second point to compare.</param>
		/// <returns>Result of comparation.</returns>
		[DebuggerStepThrough]
		public static bool operator >=( CoordinatePoint leftPoint, CoordinatePoint rightPoint )
		{
			return ( leftPoint.VirtualLine > rightPoint.VirtualLine ||
				( leftPoint.VirtualLine == rightPoint.VirtualLine && leftPoint.VirtualColumn >= rightPoint.VirtualColumn ) );
		}
		/// <summary>
		/// "Equals" operator.
		/// </summary>
		/// <param name="leftPoint">First point to compare.</param>
		/// <param name="rightPoint">Second point to compare.</param>
		/// <returns>Result of comparation.</returns>
		[DebuggerStepThrough]
		public static bool operator ==( CoordinatePoint leftPoint, CoordinatePoint rightPoint )
		{
			object leftPointObj = leftPoint;
			object rightPointObj = rightPoint;

			if( ( leftPointObj == null ) || ( rightPointObj == null ) )
				return ( leftPointObj == rightPointObj );

			return ( leftPoint.VirtualLine == rightPoint.VirtualLine && leftPoint.VirtualColumn == rightPoint.VirtualColumn );
		}
		/// <summary>
		/// "Not equal" operator.
		/// </summary>
		/// <param name="leftPoint">First point to compare.</param>
		/// <param name="rightPoint">Second point to compare.</param>
		/// <returns>Result of comparation.</returns>
		[DebuggerStepThrough]
		public static bool operator !=( CoordinatePoint leftPoint, CoordinatePoint rightPoint )
		{
			object leftPointObj = leftPoint;
			object rightPointObj = rightPoint;

			if( ( leftPointObj == null ) || ( rightPointObj == null ) )
				return ( leftPointObj != rightPointObj );

			return ( leftPoint.VirtualLine != rightPoint.VirtualLine || leftPoint.VirtualColumn != rightPoint.VirtualColumn );
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CoordinatePoint_OutliningBeforeExpand( object sender, OutliningEventArgs e )
		{
			Reset();
		}
		#endregion
	}

	/// <summary>
	/// Structure used to keep one dynamic formatting.
	/// </summary>
	public struct AdditionalFormatting
	{
		#region Public Fields
		/// <summary>
		/// Index of the first letter in formatted range.
		/// </summary>
		public int StartLetterIndex;
		/// <summary>
		/// Index of the last letter in formatted range.
		/// </summary>
		public int EndLetterIndex;
		/// <summary>
		/// Format, that will be applied to the range.
		/// </summary>
		public ISnippetFormat Format;
		#endregion
	}

	/// <summary>
	/// Structure, used to keep information, used for drawing of text.
	/// </summary>
	public struct TextDrawInfo
	{
		#region Public Fields
		/// <summary>
		/// Text, to be drawn.
		/// </summary>
		public string Text;
		/// <summary>
		/// Rectangle, where it must be drawn.
		/// </summary>
		public Rectangle DrawRectangle;
		/// <summary>
		/// Vertical alignment of the text.
		/// </summary>
		public StringAlignment VerticalAlignment;
		/// <summary>
		/// Array of the dynamic formatting, applied to the range.
		/// </summary>
		public AdditionalFormatting[] DynamicFormattings;
		/// <summary>
		/// Height of text itself.
		/// </summary>
		public int TextHeight;
		#endregion
	}

	/// <summary>
	/// Structure keeps info about bordering.
	/// </summary>
	public struct BorderInfo
	{
		#region Public Fields
		/// <summary>
		/// Border rectangle.
		/// </summary>
		public RectangleF Rect;
		/// <summary>
		/// Format that keeps info about border.
		/// </summary>
		public Format format;
		/// <summary>
		/// Indicates whether border should be forced to draw in the end of the line.
		/// </summary>
		public bool bFinish;
		#endregion
	}

	/// <summary>
	/// Structure, used to keep information about one char in the word.
	/// </summary>
	public struct CharInfo
	{
		#region Public Fields
		/// <summary>
		/// Char, the information is about.
		/// </summary>
		public char Char;
		/// <summary>
		/// Width of the char.
		/// </summary>
		public float CharWidth;
		/// <summary>
		/// Left position of the char.
		/// </summary>
		public float CharLeft;
		#endregion
	}

	/// <summary>
	/// Structure, that keeps information about measured text.
	/// </summary>
	public struct TextInfo
	{
		#region Public Fields
		/// <summary>
		/// Array of characters info.
		/// </summary>
		public CharInfo[] Characters;
		/// <summary>
		/// Width of the entire string.
		/// </summary>
		public float Width;
		/// <summary>
		/// Height of the string.
		/// </summary>
		public float Height;
		/// <summary>
		/// Count of caracters in string. Used to calculate position.
		/// </summary>
		public int Length;
		#endregion
	}

	/// <summary>
	/// Range of the text.
	/// </summary>
	internal class TextRange
		: ITextRange
		, ICloneable
	{
		#region Members
		/// <summary>
		/// Start of the range.
		/// </summary>
		private CoordinatePoint m_start;
		/// <summary>
		/// End of the range.
		/// </summary>
		private CoordinatePoint m_end;
		#endregion

		#region Properties
		/// <summary>
		/// Start of the range.
		/// </summary>
		public CoordinatePoint Start
		{
			[DebuggerStepThrough]
			get
			{
				return m_start;
			}
			set
			{
				m_start = value;
			}
		}
		/// <summary>
		/// End of the range.
		/// </summary>
		public CoordinatePoint End
		{
			[DebuggerStepThrough]
			get
			{
				return m_end;
			}
			set
			{
				m_end = value;
			}
		}
		/// <summary>
		/// Top of the range.
		/// </summary>
		public CoordinatePoint Top
		{
			[DebuggerStepThrough]
			get
			{
				return ( Start < End ) ? Start : End;
			}
		}
		/// <summary>
		/// Bottom of the range.
		/// </summary>
		public CoordinatePoint Bottom
		{
			[DebuggerStepThrough]
			get
			{
				return ( Start > End ) ? Start : End;
			}
		}
		#endregion

		#region ICloneable Members
		/// <summary>
		/// Creates object clone.
		/// </summary>
		/// <returns>Clone of the object.</returns>
		public object Clone()
		{
			TextRange range = new TextRange();
			range.Start = Start;
			range.End = End;
			return range;
		}
		#endregion
	}

	/// <summary>
	/// Complex text range. Able to contain multiple parts.
	/// </summary>
	internal class ComplexTextRange
		: IComplexTextRange
		, ICloneable
	{
		#region Fields
		/// <summary>
		/// Collection of internal ranges.
		/// </summary>
		private ArrayList m_ranges;
		/// <summary>
		/// Location of visual start.
		/// </summary>
		private StreamEditControl.VisualLocation m_visualStart;
		/// <summary>
		/// Location of visual end.
		/// </summary>
		private StreamEditControl.VisualLocation m_visualEnd;
		#endregion

		#region Initialization
		/// <summary>
		/// Creates new instance of ComplexTextRange.
		/// </summary>
		public ComplexTextRange()
		{
			m_ranges = new ArrayList();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Clears collection of ranges.
		/// </summary>
		public void Clear()
		{
			m_ranges.Clear();
		}
		#endregion

		#region IComplexTextRange Members
		/// <summary>
		/// Gets collection of internal ranges in complex text range.
		/// </summary>
		public ArrayList Ranges
		{
			get
			{
				return m_ranges;
			}
		}
		/// <summary>
		/// Checks whether range is empty, i.e. contains no internal ranges.
		/// </summary>
		/// <returns>True if range is empty; otherwise false.</returns>
		public bool IsEmpty()
		{
			return ( m_ranges.Count == 0 );
		}
		/// <summary>
		/// Checks whether selection exists.
		/// </summary>
		/// <returns>True if selection exists.</returns>
		public bool Exists()
		{
			return ( !IsEmpty() && this.Start != this.End && ( !IsBlock() || this.VisualTopLeft != this.VisualBottomRight ) );
		}
		/// <summary>
		/// Checks whether selection is block.
		/// </summary>
		/// <returns>True if selection is block.</returns>
		public bool IsBlock()
		{
			return ( !m_visualStart.IsEmpty() && !m_visualEnd.IsEmpty() );
		}
		/// <summary>
		/// Gets or sets visual start.
		/// </summary>
		public StreamEditControl.VisualLocation VisualStart
		{
			get
			{
				return m_visualStart;
			}
			set
			{
				m_visualStart = value;
			}
		}
		/// <summary>
		/// Gets or sets visual end.
		/// </summary>
		public StreamEditControl.VisualLocation VisualEnd
		{
			get
			{
				return m_visualEnd;
			}
			set
			{
				m_visualEnd = value;
			}
		}
		/// <summary>
		/// Gets visual top left corner.
		/// </summary>
		public StreamEditControl.VisualLocation VisualTopLeft
		{
			get
			{
				int offset = ( m_visualStart.Offset < m_visualEnd.Offset ) ? ( m_visualStart.Offset ) : ( m_visualEnd.Offset );
				int line, subLine;
				if( m_visualStart.Line < m_visualEnd.Line )
				{
					line = m_visualStart.Line;
					subLine = m_visualStart.SubLine;
				}
				else if( m_visualStart.Line > m_visualEnd.Line )
				{
					line = m_visualEnd.Line;
					subLine = m_visualEnd.SubLine;
				}
				else
				{
					line = m_visualStart.Line;
					subLine = ( m_visualStart.SubLine < m_visualEnd.SubLine ) ? ( m_visualStart.SubLine ) : ( m_visualEnd.SubLine );
				}

				return new StreamEditControl.VisualLocation( line, subLine, offset );
			}
		}
		/// <summary>
		/// Gets visual bottom right corner.
		/// </summary>
		public StreamEditControl.VisualLocation VisualBottomRight
		{
			get
			{
				int offset = ( m_visualStart.Offset > m_visualEnd.Offset ) ? ( m_visualStart.Offset ) : ( m_visualEnd.Offset );
				int line, subLine;
				if( m_visualStart.Line > m_visualEnd.Line )
				{
					line = m_visualStart.Line;
					subLine = m_visualStart.SubLine;
				}
				else if( m_visualStart.Line < m_visualEnd.Line )
				{
					line = m_visualEnd.Line;
					subLine = m_visualEnd.SubLine;
				}
				else
				{
					line = m_visualStart.Line;
					subLine = ( m_visualStart.SubLine > m_visualEnd.SubLine ) ? ( m_visualStart.SubLine ) : ( m_visualEnd.SubLine );
				}

				return new StreamEditControl.VisualLocation( line, subLine, offset );
			}
		}
		#endregion

		#region ITextRange Members
		/// <summary>
		/// Gets start point of the first range. Sets start point of the first range and removes all the rest ranges on setter.
		/// </summary>
		public CoordinatePoint Start
		{
			get
			{
				CoordinatePoint result = null;
				if( m_ranges.Count > 0 )
				{
					result = ( ( ITextRange )m_ranges[ 0 ] ).Start;
				}
				return result;
			}
			set
			{
				TextRange range = null;
				if( m_ranges.Count > 0 )
				{
					range = ( TextRange )m_ranges[ 0 ];
					m_ranges.Clear();
				}
				else
				{
					range = new TextRange();
				}
				range.Start = value;
				m_ranges.Add( range );
			}
		}
		/// <summary>
		/// Gets end point of the last range. Sets end point of the first range and removes all the rest ranges on setter.
		/// </summary>
		public CoordinatePoint End
		{
			get
			{
				CoordinatePoint result = null;
				if( m_ranges.Count > 0 )
				{
					result = ( ( ITextRange )m_ranges[ m_ranges.Count - 1 ] ).End;
				}
				return result;
			}
			set
			{
				TextRange range = null;
				if( m_ranges.Count > 0 )
				{
					range = ( TextRange )m_ranges[ 0 ];
					m_ranges.Clear();
				}
				else
				{
					range = new TextRange();
				}
				range.End = value;
				m_ranges.Add( range );
			}
		}
		/// <summary>
		/// Gets top point of the first range.
		/// </summary>
		public CoordinatePoint Top
		{
			get
			{
				CoordinatePoint result = null;
				if( m_ranges.Count > 0 )
				{
					result = ( ( ITextRange )m_ranges[ 0 ] ).Top;
				}
				return result;
			}
		}
		/// <summary>
		/// Gets bottom point of the last range.
		/// </summary>
		public CoordinatePoint Bottom
		{
			get
			{
				CoordinatePoint result = null;
				if( m_ranges.Count > 0 )
				{
					result = ( ( ITextRange )m_ranges[ m_ranges.Count - 1 ] ).Bottom;
				}
				return result;
			}
		}
		#endregion

		#region IClonable Members
		/// <summary>
		/// Clones current instance of ComplexTextRange.
		/// </summary>
		/// <returns>Cloned object.</returns>
		public object Clone()
		{
			ComplexTextRange result = new ComplexTextRange();
			foreach( TextRange range in m_ranges )
			{
				result.Ranges.Add( range.Clone() );
			}
			result.VisualStart = m_visualStart;
			result.VisualEnd = m_visualEnd;
			return result;
		}
		#endregion
	}

	/// <summary>
	/// Interface for accessing textrange.
	/// </summary>
	public interface ITextRange
	{
		#region Properties
		/// <summary>
		/// Start of the range.
		/// </summary>
		CoordinatePoint Start { get;}
		/// <summary>
		/// End of the range.
		/// </summary>
		CoordinatePoint End { get;}
		/// <summary>
		/// Top of the range.
		/// </summary>
		CoordinatePoint Top { get;}
		/// <summary>
		/// Bottom of the range.
		/// </summary>
		CoordinatePoint Bottom { get;}
		#endregion
	}

	/// <summary>
	/// Complex text range. Able to contain multiple parts.
	/// </summary>
	// This interface was created for backward compatibility during implementing block selection.
	public interface IComplexTextRange
		: ITextRange
	{
		#region Properties
		/// <summary>
		/// Gets collection of internal ranges in complex text range.
		/// </summary>
		ArrayList Ranges { get; }
		#endregion

		#region Methods
		/// <summary>
		/// Checks whether range is empty, i.e. contains no internal ranges.
		/// </summary>
		/// <returns>True if range is empty; otherwise false.</returns>
		bool IsEmpty();
		/// <summary>
		/// Checks whether selection is block.
		/// </summary>
		/// <returns></returns>
		bool IsBlock();
		#endregion
	}

	/// <summary>
	/// Structure that contains information about the single repacement.
	/// </summary>
	public struct TextReplacementRecord
	{
		#region Public Fields
		/// <summary>
		/// Start point of the text to be replaced.
		/// </summary>
		public IParsePoint PointStart;
		/// <summary>
		/// End point of the text to be replaced.
		/// </summary>
		public IParsePoint PointEnd;
		/// <summary>
		/// New text to be placed instead of the existing one.
		/// </summary>
		public string NewText;
		#endregion

		#region Initialization
		/// <summary>
		/// Initializes TextReplacementRecord structure with start and end points and new text.
		/// </summary>
		/// <param name="pointStart">Start point of the text to be replaced.</param>
		/// <param name="pointEnd">End point of the text to be replaced.</param>
		/// <param name="text">New text to be placed instead of the existing one.</param>
		public TextReplacementRecord( IParsePoint pointStart, IParsePoint pointEnd, string text )
		{
			PointStart = pointStart;
			PointEnd = pointEnd;
			NewText = text;
		}
		#endregion
	}

	/// <summary>
	/// Class for managing Show white space mode.
	/// </summary>
	public class ShowWhiteSpaceProperties
	{
		#region Constants.
		/// <summary>
		/// Default new line replacing string.
		/// </summary>
		public const string DEF_NEWLINE_STRING_DEFAULT = "¶";
		/// <summary>
		/// Default tab replacing string.
		/// </summary>
		public const string DEF_TAB_STRING_DEFAULT = "→";
		/// <summary>
		/// Default space replacing char.
		/// </summary>
		public const string DEF_SPACE_CHAR_DEFAULT = "•";
		#endregion

		#region Internal Classes
		/// <summary>
		/// 
		/// </summary>
		internal class ShowWhiteSpacePropertiesConverter
			: ExpandableObjectConverter
		{
			/// <summary>
			/// 
			/// </summary>
			/// <param name="context"></param>
			/// <returns></returns>
			public override bool GetCreateInstanceSupported( ITypeDescriptorContext context )
			{
				return false;
			}
		}
		#endregion

		#region Fields
		/// <summary>
		/// Whether Tabs should be replaces with symbols.
		/// </summary>
		private bool m_bShowTabs = true;
		/// <summary>
		/// Whether New lines should be replaces with symbols.
		/// </summary>
		private bool m_bShowNewLines = true;
		/// <summary>
		/// Whether Spaces should be replaces with symbols.
		/// </summary>
		private bool m_bShowSpaces = true;
		/// <summary>
		/// String representing new line in Show white space mode.
		/// </summary>
		private string m_newLineString = DEF_NEWLINE_STRING_DEFAULT;
		/// <summary>
		/// String representing Tab in Show white space mode.
		/// </summary>
		private string m_tabString = DEF_TAB_STRING_DEFAULT;
		/// <summary>
		/// Char representing space in Show white space mode.
		/// </summary>
		private string m_spaceChar = DEF_SPACE_CHAR_DEFAULT;
		#endregion

		#region Properties
		/// <summary>
		/// Whether Tabs should be replaces with symbols.
		/// </summary>
		[DefaultValue( true )]
		public bool ShowTabs
		{
			get
			{
				return m_bShowTabs;
			}
			set
			{
				if( value != m_bShowTabs )
				{
					m_bShowTabs = value;
					OnChange();
				}
			}
		}
		/// <summary>
		/// Whether New lines should be replaces with symbols.
		/// </summary>
		[DefaultValue( true )]
		public bool ShowNewLines
		{
			get
			{
				return m_bShowNewLines;
			}
			set
			{
				if( value != m_bShowNewLines )
				{
					m_bShowNewLines = value;
					OnChange();
				}
			}
		}
		/// <summary>
		/// Whether Spaces should be replaces with symbols.
		/// </summary>
		[DefaultValue( true )]
		public bool ShowSpaces
		{
			get
			{
				return m_bShowSpaces;
			}
			set
			{
				if( value != m_bShowSpaces )
				{
					m_bShowSpaces = value;
					OnChange();
				}
			}
		}
		/// <summary>
		/// Gets or sets string that represents line feed in White space mode.
		/// </summary>
		[DefaultValue( DEF_NEWLINE_STRING_DEFAULT )]
		public string NewLineString
		{
			get
			{
				return m_newLineString;
			}
			set
			{
				if( value == m_newLineString ) return;

				if( value == string.Empty || value == null )
				{
					m_newLineString = DEF_NEWLINE_STRING_DEFAULT;
				}
				else
				{
					m_newLineString = value;
				}

				OnChange();
			}
		}
		/// <summary>
		/// Gets or sets string that represents Tab in White space mode.
		/// </summary>
		[DefaultValue( DEF_TAB_STRING_DEFAULT )]
		public string TabString
		{
			get
			{
				return m_tabString;
			}
			set
			{
				if( value == m_tabString ) return;

				if( value == string.Empty || value == null )
				{
					m_tabString = DEF_TAB_STRING_DEFAULT;
				}
				else
				{
					m_tabString = value;
				}

				OnChange();
			}
		}
		/// <summary>
		/// Gets or sets char that represents line feed in White space mode.
		/// </summary>
		[DefaultValue( DEF_SPACE_CHAR_DEFAULT )]
		public string SpaceChar
		{
			get
			{
				return m_spaceChar;
			}
			set
			{
				if( value == m_spaceChar ) return;

				if( value == "\0" )
				{
					m_spaceChar = DEF_SPACE_CHAR_DEFAULT;
				}
				else
				{
					m_spaceChar = value;
				}

				OnChange();
			}
		}
		#endregion

		#region Event Raisers
		/// <summary>
		/// Calls Change event handlers.
		/// </summary>
		public void OnChange()
		{
			if( null != Change )
			{
				Change( null, EventArgs.Empty );
			}
		}
		#endregion

		#region Events
		/// <summary>
		/// Raises when any public property changes.
		/// </summary>
		public event EventHandler Change;
		#endregion
	}
}