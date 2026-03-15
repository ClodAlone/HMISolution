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

using Syncfusion.Windows.Forms.Edit.Interfaces;

namespace Syncfusion.Windows.Forms.Edit.Implementation.IO
{
	/// <exclude/>
	/// <summary>
	/// ParsePoint is an object, used to keep data about some position in text stream.
	/// All changes must be tracked and all parsepoints must be correctly updated 
	/// to ensure that all parsepoints are reliable.
	/// </summary>
	public sealed class ParsePoint
		: IParsePoint
		, IComparable
		, IDisposable
	{
		#region Constants
		/// <summary>
		/// Offset for the parse point, that specifies it's unreliability. It is set when parsepoint is deleted.
		/// </summary>
		private const int DEF_UNRELIABLE_OFFSET = -128;
		#endregion

		#region Fields
		/// <summary>
		/// ParsePoint's line.
		/// </summary>
		private int m_iLine;
		/// <summary>
		/// ParsePoint's column in line.
		/// </summary>
		private int m_iPosition;
		/// <summary>
		/// ParsePoint's offset in stream.
		/// </summary>
		private long m_lOffset;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets position in stream.
		/// </summary>
		public long Offset
		{
			get
			{
				if( m_lOffset == DEF_UNRELIABLE_OFFSET ) throw new Exception(
					Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_52 );

				return m_lOffset;
			}
			set
			{
				if( value != Offset )
				{
					m_lOffset = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets column in line.
		/// </summary>
		public int Position
		{
			get
			{
				if( m_lOffset == DEF_UNRELIABLE_OFFSET ) throw new Exception(
					Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_52 );

				return m_iPosition;
			}
			set
			{
				if( value != m_iPosition )
				{
					m_iPosition = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets line index.
		/// </summary>
		public int Line
		{
			get
			{
				if( m_lOffset == DEF_UNRELIABLE_OFFSET ) throw new Exception(
					Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_52 );

				return m_iLine;
			}
			set
			{
				if( value != m_iLine )
				{
					m_iLine = value;
				}
			}
		}
		/// <summary>
		/// Gets sing of validity of parsepoint.
		/// </summary>
		public bool IsValid
		{
			get
			{
				return ( m_lOffset != DEF_UNRELIABLE_OFFSET );
			}
		}
		#endregion

		#region Initialization And Finalization
		/// <summary>
		/// Creates new instance of ParsePoint class.
		/// </summary>
		/// <param name="iLine">Initial line.</param>
		/// <param name="iPosition">Initial column.</param>
		/// <param name="lOffset">Initial position in stream.</param>
		internal ParsePoint( int iLine, int iPosition, long lOffset )
		{
			if( iLine < 0 ) throw new ArgumentOutOfRangeException( "iLine" );
			if( iPosition < 0 ) throw new ArgumentOutOfRangeException( "iPosition" );
			if( lOffset < 0 ) throw new ArgumentOutOfRangeException( "lOffset" );

			m_iLine = iLine;
			m_iPosition = iPosition;
			m_lOffset = lOffset;
		}
		/// <summary>
		/// Disposes object and frees used resources.
		/// </summary>
		public void Dispose()
		{
			GC.SuppressFinalize( this );
			m_iLine = -1;
			m_iPosition = -1;
			m_lOffset = DEF_UNRELIABLE_OFFSET;
		}
		#endregion

		#region Events
		/// <summary>
		/// Raised when some parameter of parse point is changed.
		/// </summary>
		public event ParsePointParameterChangedEventHandler ParsePointParameterChanged;
		/// <summary>
		/// Event that is raised when point is deleted from collection and becomes unreliable.
		/// </summary>
		public event ParsePointDeletedEventHandler Deleted;
		#endregion

		#region Overrides
		/// <summary>
		/// Gives full informatio about ParsePoint. This information includes Line, Position and Offset.
		/// </summary>
		/// <returns>String with information.</returns>
		public override string ToString()
		{
			return string.Format( "Line: {0}  Position: {1} Offset: {2}", m_iLine, m_iPosition, m_lOffset );
		}
		#endregion

		#region IComparable Members
		/// <summary>
		/// Comparer for array-sorters.
		/// </summary>
		/// <param name="obj">Another ParsePoint object, or int value, that specifies the offset.</param>
		public int CompareTo( object obj )
		{
			ParsePoint secondPoint = obj as ParsePoint;
			long SecondOffset = ( secondPoint != null ) ? ( secondPoint.Offset ) : ( ( long )obj );
			return Offset.CompareTo( SecondOffset );
		}
		#endregion

		#region Operators
		/// <summary>
		/// Defines operator.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static bool operator <( ParsePoint x, ParsePoint y )
		{
			return ( x.Line < y.Line ) || ( x.Line == y.Line && x.Position < y.Position );
		}
		/// <summary>
		/// Defines operator.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static bool operator <=( ParsePoint x, ParsePoint y )
		{
			return ( x.Line < y.Line ) || ( x.Line == y.Line && x.Position <= y.Position );
		}
		/// <summary>
		/// Defines operator.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static bool operator >( ParsePoint x, ParsePoint y )
		{
			return ( x.Line > y.Line ) || ( x.Line == y.Line && x.Position > y.Position );
		}
		/// <summary>
		/// Defines operator.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static bool operator >=( ParsePoint x, ParsePoint y )
		{
			return ( x.Line > y.Line ) || ( x.Line == y.Line && x.Position >= y.Position );
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Raises Deleted event.
		/// </summary>
		/// <param name="lNewOffset">Offset for newly created parse point.</param>
		public void RaiseDeletedEvent( long lNewOffset )
		{
			if( Deleted != null )
			{
				Deleted( this, lNewOffset );
			}
		}
		/// <summary>
		/// Raises ParsePointParameterChanged event.
		/// </summary>
		/// <param name="oldOffset">Old point offset.</param>
		/// <param name="newOffset">New point offset.</param>
		/// <param name="oldPos">Old point position.</param>
		/// <param name="newPos">New point position.</param>
		/// <param name="oldLine">Old point line.</param>
		/// <param name="newLine">New point line.</param>
		public void RaiseParsePointParameterChanged(
			long oldOffset, long newOffset, int oldPos, int newPos, int oldLine, int newLine )
		{
			if( ParsePointParameterChanged != null )
			{
				ParsePointParameterChangedEventArgs args = new ParsePointParameterChangedEventArgs( oldOffset, newOffset, oldPos, newPos, oldLine, newLine );
				ParsePointParameterChanged( this, args );
			}
		}
		#endregion
	}
}