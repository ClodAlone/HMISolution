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

using System.Collections;
using System;

using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Utils;

namespace Syncfusion.Windows.Forms.Edit.Implementation.Formatting
{
	/// <summary>
	/// Corresponding for special formating for selecetion, etc.
	/// </summary>
	public class DynamicFormat
		: IDynamicFormat
		, IComparable
	{
		#region Internal Classes
		/// <summary>
		/// Comparer, used for search of dynamic format by the coordinate point, 
		/// that is treated like the start of dymanic format region.
		/// </summary>
		private class CoordinatePointComparerWithoutEnd
			: IComparer
		{
			#region IComparer Implementation
			/// <summary>
			/// Compares dynamic format object with coordinates point. 
			/// Coordinate point is treated as start coordinate of the new region.
			/// </summary>
			/// <param name="x">Dynamic format.</param>
			/// <param name="y">Coordinate point. </param>
			/// <returns>Standart comparision results.</returns>
			public int Compare( object x, object y )
			{
				DynamicFormat format = ( DynamicFormat )x;
				CoordinatePoint point = ( CoordinatePoint )y;

				if( point > format.End )
					return -1;

				if( point < format.Start )
					return 1;

				return 0;
			}
			#endregion
		}

		/// <summary>
		/// Comparer, used for search of dynamic format by the coordinate point, 
		/// that is treated like the end of dymanic format region.
		/// </summary>
		private class CoordinatePointComparerIncludingEnd
			: IComparer
		{
			#region IComparer Implementation
			/// <summary>
			/// Compares dynamic format object with coordinates point. 
			/// Coordinate point is treated as end coordinate of the new region.
			/// </summary>
			/// <param name="x">Dynamic format.</param>
			/// <param name="y">Coordinate point. </param>
			/// <returns>Standart comparision results.</returns>
			public int Compare( object x, object y )
			{
				DynamicFormat format = ( DynamicFormat )x;
				CoordinatePoint point = ( CoordinatePoint )y;

				if( point >= format.End )
					return -1;

				if( point < format.Start )
					return 1;

				return 0;
			}
			#endregion
		}
		#endregion

		#region Class Static Members
		/// <summary>
		/// Default comparer for dynamic formats.
		/// </summary>
		private static IComparer _DefaultStartComparer;
		/// <summary>
		/// Default comparer for dynamic formats.
		/// </summary>
		private static IComparer _DefaultEndComparer;
		#endregion

		#region Class Static Properties
		/// <summary>
		/// Default comparer for dynamic formats. 
		/// </summary>
		public static IComparer DefaultStartComparer
		{
			get
			{
				if( _DefaultStartComparer == null )
					_DefaultStartComparer = new CoordinatePointComparerWithoutEnd();

				return _DefaultStartComparer;
			}
		}
		/// <summary>
		/// Default comparer for dynamic formats. 
		/// </summary>
		public static IComparer DefaultEndComparer
		{
			get
			{
				if( _DefaultEndComparer == null )
					_DefaultEndComparer = new CoordinatePointComparerIncludingEnd();

				return _DefaultEndComparer;
			}
		}
		#endregion

		#region Class Members
		/// <summary>
		/// Start point of the format.
		/// </summary>
		private CoordinatePoint m_start;
		/// <summary>
		/// End point of the format.
		/// </summary>
		private CoordinatePoint m_end;
		/// <summary>
		/// Format.
		/// Just FontColor, ForeColor and BackColor will be used.
		/// </summary>
		private ISnippetFormat m_format;
		/// <summary>
		/// Offset of start coordinate point. Used for recreating start point when it's deleted.
		/// </summary>
		private long m_lStartOffset;
		/// <summary>
		/// Offset of end coordinate point. Used for recreating end point when it's deleted.
		/// </summary>
		private long m_lEndOffset;
		/// <summary>
		/// Indicates whether start point has been deleted.
		/// </summary>
		private bool m_bStartDeleted;
		/// <summary>
		/// Indicates whether end point has been deleted.
		/// </summary>
		private bool m_bEndDeleted;
		#endregion

		#region Class Properties
		/// <summary>
		/// Gets or sets start point.
		/// </summary>
		public CoordinatePoint Start
		{
			get
			{
				return m_start;
			}
			set
			{
				if( value == null ) throw new ArgumentNullException( "Start" );

				m_start = value;
				m_lStartOffset = m_start.PhysicalPoint.Offset;
				m_bStartDeleted = false;
			}
		}
		/// <summary>
		/// Gets or sets end point.
		/// </summary>
		public CoordinatePoint End
		{
			get
			{
				return m_end;
			}
			set
			{
				if( value == null ) throw new ArgumentNullException( "End" );

				m_end = value;
				m_lEndOffset = m_end.PhysicalPoint.Offset;
				m_bEndDeleted = false;
			}
		}
		/// <summary>
		/// Gets or sets format to be used.
		/// </summary>
		public ISnippetFormat Format
		{
			get
			{
				return m_format;
			}
			set
			{
				if( value == null ) throw new ArgumentNullException( "Format" );

				m_format = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether start point has been deleted.
		/// </summary>
		public bool StartDeleted
		{
			get
			{
				return m_bStartDeleted;
			}
			set
			{
				m_bStartDeleted = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicating whether end point has been deleted.
		/// </summary>
		public bool EndDeleted
		{
			get
			{
				return m_bEndDeleted;
			}
			set
			{
				m_bEndDeleted = value;
			}
		}
		/// <summary>
		/// Gets or sets offset of start coordinate point. Used for creating new point after the old one is deleted.
		/// </summary>
		public long StartOffset
		{
			get
			{
				return m_lStartOffset;
			}
			set
			{
				m_lStartOffset = value;
			}
		}
		/// <summary>
		/// Gets or sets offset of end coordinate point. Used for creating new point after the old one is deleted.
		/// </summary>
		public long EndOffset
		{
			get
			{
				return m_lEndOffset;
			}
			set
			{
				m_lEndOffset = value;
			}
		}
		#endregion

		#region Class Initialize/Finalize Methods
		/// <summary>
		/// Creates new instance of the class and initializes it.
		/// </summary>
		/// <param name="start">Start point.</param>
		/// <param name="end">End point.</param>
		/// <param name="format">Format to be used.</param>
		public DynamicFormat( CoordinatePoint start, CoordinatePoint end, ISnippetFormat format )
		{
			if( start == null ) throw new ArgumentNullException( "start" );
			if( end == null ) throw new ArgumentNullException( "end" );
			if( format == null ) throw new ArgumentNullException( "format" );

			this.Start = start;
			this.End = end;
			m_format = format;
		}
		#endregion

		#region IComparable Members
		/// <summary>
		/// Compares current DynamicFormat with other DynamicFormat.
		/// </summary>
		/// <param name="obj">ParsePoint</param>
		/// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared. </returns>
		public int CompareTo( object obj )
		{
			IDynamicFormat frm = ( IDynamicFormat )obj;

			if( frm.Start >= m_end )
				return -1;

			if( frm.End < m_start )
				return 1;

			return 0;
		}
		#endregion

		#region Class Public Methods
		/// <summary>
		/// Checks state of start and end points of format. Reassignes it if needed.
		/// </summary>
		/// <returns>True if format has been successfully updated.
		/// False if format can't be updated and must be deleed.</returns>
		public bool UpdateState()
		{
			bool bResult = false;

			if( !m_bStartDeleted || !m_bEndDeleted )
			{
				if( m_lEndOffset != m_lStartOffset )
				{
					if( m_bStartDeleted )
					{
						ILexemParser parser = m_start.Parser;
						IParsePoint parsePoint = parser.BaseStream.GetParsePoint( m_lStartOffset );

						if( BeforeStartReassigned != null )
						{
							BeforeStartReassigned( this, EventArgs.Empty );
						}

						Start = parser.GetCoordinatePoint( parsePoint, false, true );

						if( StartReassigned != null )
						{
							StartReassigned( this, EventArgs.Empty );
						}
					}

					if( m_bEndDeleted )
					{
						ILexemParser parser = m_end.Parser;
						IParsePoint parsePoint = parser.BaseStream.GetParsePoint( m_lEndOffset );

						if( BeforeEndReassigned != null )
						{
							BeforeEndReassigned( this, EventArgs.Empty );
						}

						End = parser.GetCoordinatePoint( parsePoint, false, true );

						if( EndReassigned != null )
						{
							EndReassigned( this, EventArgs.Empty );
						}
					}

					if( Start.PhysicalPoint.Offset < End.PhysicalPoint.Offset )
					{
						bResult = true;
					}
				}
			}

			return bResult;
		}
		#endregion

		#region Class Events
		/// <summary>
		/// Raised before start point is reassigned.
		/// </summary>
		public event EventHandler StartReassigned;
		/// <summary>
		/// Raised before end point is reassigned.
		/// </summary>
		public event EventHandler EndReassigned;
		/// <summary>
		/// Raised after start point is reassigned.
		/// </summary>
		public event EventHandler BeforeStartReassigned;
		/// <summary>
		/// Raised after end point is reassigned.
		/// </summary>
		public event EventHandler BeforeEndReassigned;
		#endregion
	}
}