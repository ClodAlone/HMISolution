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
using System.Reflection;

using Syncfusion.Windows.Forms.Edit.Interfaces;

namespace Syncfusion.Windows.Forms.Edit.Implementation.IO
{
	/// <summary>
	/// Class, that keeps list of ParsePoints and manages it.
	/// </summary>
	public class ParsePointManager
		: IEnumerable
		, IDisposable
	{
		#region Classes
		/// <summary>
		/// Class, used for search of ParsePoint by it's offset.
		/// </summary>
		public class ParsePointSearch
			: IComparer
		{
			#region IComparer Members
			/// <summary>
			/// Compares two elements by their offset.
			/// </summary>
			/// <param name="x">WeakParsePoint.</param>
			/// <param name="y">WeakParsePoint or long.</param>
			/// <returns>Standard IComparer return value.</returns>
			public int Compare( object x, object y )
			{
				ParsePoint px = x as ParsePoint;
				ParsePoint py = y as ParsePoint;

				return px.Offset.CompareTo( ( py != null ) ? py.Offset : ( long )y );
			}
			#endregion
		}

		/// <summary>
		/// Class, used for serach of ParsePoint by it`s position.
		/// </summary>
		public class ParsePointSearchByPosition
			: IComparer
		{
			#region IComparer Members
			/// <summary>
			/// Compares two elements by their offset.
			/// </summary>
			/// <param name="x">WeakParsePoint.</param>
			/// <param name="y">ParsePoint or int.</param>
			/// <returns>Standard IComparer return value.</returns>
			public int Compare( object x, object y )
			{
				ParsePoint px = x as ParsePoint;
				ParsePoint py = y as ParsePoint;

				int CmpResult;

				if( py != null )
				{
					CmpResult = px.Line.CompareTo( py.Line );

					if( CmpResult == 0 )
						return px.Position.CompareTo( py.Position );
				}
				else
				{
					CmpResult = px.Line.CompareTo( ( int )y );

					if( CmpResult == 0 )
						return px.Position.CompareTo( 1 );
				}

				return CmpResult;

			}
			#endregion
		}
		#endregion

		#region Fields
		/// <summary>
		/// Internal list of parsepoints.
		/// </summary>
		private ArrayList m_List = new ArrayList( 128 );
		/// <summary>
		/// Comparer for search by position in stream
		/// </summary>
		internal static IComparer DEF_PARSEPOINT_SEARCH = new ParsePointSearch();
		/// <summary>
		/// Comparer for search by position of cursor
		/// </summary>
		internal static IComparer DEF_PARSEPOINT_SEARCH_POS = new ParsePointSearchByPosition();
		/// <summary>
		/// Specifies whether manager is currently updating parsepoint`s data.
		/// </summary>
		private bool m_bInUpdateState;
		/// <summary>
		/// List of delegates that should be invoked after the update of all parse points.
		/// </summary>
		private ArrayList m_delegatesToInvokeAfterUpdate;
		#endregion

		#region Properties
		/// <summary>
		/// Gets count of ParsePoints
		/// </summary>
		public int Count
		{
			get
			{
				return m_List.Count;
			}
		}
		#endregion

		#region Initialization And Finalization
		/// <summary>
		/// Creates and initializes new instance of ParsePointManager.
		/// </summary>
		public ParsePointManager()
		{
			m_delegatesToInvokeAfterUpdate = new ArrayList();
		}
		/// <summary>
		/// Deletes all parse points.
		/// </summary>
		public void Dispose()
		{
			foreach( ParsePoint point in m_List )
			{
				point.Dispose();
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Searches for the parsepoint, associated with given offset, or creates new parsepoint.
		/// </summary>
		/// <param name="streamOffset">Needed offset in the stream.</param>
		/// <param name="defLine">Line, that must be set if ParsePoint is created.</param>
		/// <param name="defColumn">Column, that must be set if ParsePoint is created.</param>
		/// <returns>IParsePoint of the found/created ParsePoint.</returns>
		public IParsePoint GetParsePointAtPosition( long streamOffset, int defLine, int defColumn )
		{
			if( streamOffset < 0 )
				throw new ArgumentOutOfRangeException( "streamOffset", streamOffset,
					Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_17 );

			if( m_bInUpdateState )
				throw new Exception( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_18 );

			IParsePoint parsePoint;

			int Left;
			int Right;
			bool res = GetNearestParsePoints( streamOffset, out Left, out Right );

			if( res && Left == Right )
			{
				parsePoint = ( ParsePoint )m_List[ Left ];
			}
			else
			{
				Left = Left + 1; //Insert must be made after left position
				parsePoint = CreateNewParsePoint( defLine, defColumn, streamOffset );
				m_List.Insert( Left, parsePoint );
			}

			return parsePoint;
		}
		/// <summary>
		/// Creates new ParsePoint and inserts it to given position.
		/// </summary>
		/// <param name="streamOffset">Position in stream. Must be between two nearest ParsePoint`s positions.</param>
		/// <param name="line">ParsePoints line. No checks are done.</param>
		/// <param name="column">ParsePoints column. No checks are done.</param>
		/// <param name="index">Index, the ParsePoint is to be inserted at.</param>
		/// <returns>Newly created ParsePoint</returns>
		public IParsePoint InsertPointInPosition( long streamOffset, int line, int column, int index )
		{
			if( index < 0 || index > m_List.Count )
				throw new ArgumentOutOfRangeException( "index", index,
					Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_19 );

			if( ( index > 0 && ( ( ParsePoint )m_List[ index - 1 ] ).Offset >= streamOffset ) ||
					( index < m_List.Count && ( ( ParsePoint )m_List[ index ] ).Offset <= streamOffset ) )
				throw new ArgumentException(
					Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_20, "streamOffset" );

			if( m_bInUpdateState )
				throw new Exception( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_18 );

			ParsePoint newParsePoint = CreateNewParsePoint( line, column, streamOffset );
			m_List.Insert( index, newParsePoint );

			return newParsePoint;
		}
		/// <summary>
		/// Returns ParsePoint by it's index in array.
		/// </summary>
		/// <param name="index">Index of the ParsePoint</param>
		/// <returns>IParsePoint interface</returns>
		public IParsePoint GetParsePointByIndex( int index )
		{
			if( index < 0 || index > m_List.Count - 1 )
				throw new ArgumentOutOfRangeException(
					"index", index, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_21 );

			if( m_bInUpdateState )
				throw new Exception( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_18 );

			return ( ParsePoint )m_List[ index ];
		}
		/// <summary>
		/// Updates offsets of all ParsePoints, starting from the given position.
		/// If data was deleted, than iBytesInserted must be negative, everything else must be positive.
		/// </summary>
		/// <param name="startPoint">StartPoint, that is in position, that was moved.</param>
		/// <param name="iBytesInserted">Count of inserted byte (if negative, then bytes were deleted)</param>
		/// <param name="iLinesInData">Count of lines in inserted/deleted data</param>
		/// <param name="iLastLineLength">Position offset for the last line.</param>
		public void OffsetChanged( IParsePoint startPoint, long iBytesInserted, int iLinesInData, int iLastLineLength )
		{
			if( startPoint == null )
				throw new ArgumentNullException( "startPoint" );

			if( iBytesInserted == 0 )
				throw new ArgumentOutOfRangeException( "iBytesInserted", iBytesInserted,
					Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_22 );

			m_bInUpdateState = true;
			IParsePoint StartPoint = startPoint;

			int startPointIndex = BinarySearch( startPoint, DEF_PARSEPOINT_SEARCH );

			if( startPointIndex < 0 )
				throw new ArgumentException(
					Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_23, "startPoint" );

			long NewPosition = StartPoint.Offset + iBytesInserted;
			int StartPos = StartPoint.Position;
			int StartLine = StartPoint.Line;
			int Right = startPointIndex;
			bool bDeleteMode = ( iBytesInserted < 0 );

			// If it is delition than we must use index not of
			// the first line, but of the last one.
			if( bDeleteMode )
			{
				iLinesInData = -iLinesInData;

				if( iLinesInData != 0 )
					iLastLineLength++;

				iLastLineLength = -iLastLineLength;
				StartPos = 0;
			}

			if( m_List.Count > 0 )
			{
				// MUST delete all parsepoints in range [StartPosition - iBytesInserted, StartPosition)
				if( bDeleteMode )
				{
					// Offset of new parse point that will be possibly created ater deleting the current one.
					long lNewPointOffset = startPoint.Offset + iBytesInserted;

					IList toRemove = new ArrayList();

					for( ; Right >= 0; Right-- )
					{
						ParsePoint point = m_List[ Right ] as ParsePoint;

						if( point.Offset < NewPosition )
							break;

						if( point.Offset != startPoint.Offset )
						{
							DictionaryEntry entry = new DictionaryEntry( point, lNewPointOffset );
							toRemove.Add( entry );
						}
					}

					for( int i = 0, len = toRemove.Count; i < len; i++ )
					{
						DictionaryEntry entry = ( DictionaryEntry )toRemove[ i ];

						DeleteParsePointInternal( entry );
					}

					Right++;
				}

				if( Right != -1 )
				{
					bool MultiLine = ( iLinesInData != 0 );

					for( int n = Right; n < m_List.Count; n++ )
					{
						ParsePoint point = ( ParsePoint )m_List[ n ];

						long lNewOffset = point.Offset;
						int iNewPos = point.Position;
						int iNewLine = point.Line;

						long lOldOffset = lNewOffset;
						int iOldPos = iNewPos;
						int iOldLine = iNewLine;

						lNewOffset += iBytesInserted;

						if( !MultiLine )
						{
							if( iNewLine == StartLine )
								iNewPos += iLastLineLength;
						}
						else
						{
							if( iNewLine == StartLine )
							{
								iNewPos = iNewPos - StartPos + iLastLineLength + 1;
							}

							iNewLine += iLinesInData;
						}

						point.Offset = lNewOffset;

						point.Position = iNewPos;
						point.Line = iNewLine;

						point.RaiseParsePointParameterChanged( lOldOffset, lNewOffset, iOldPos, iNewPos, iOldLine, iNewLine );
					}
				}
			}

			m_bInUpdateState = false;

			InvokeDelegatesAfterUpdate();
		}
		/// <summary>
		/// Checks integrity of the internal list of parsepoints.
		/// </summary>
		/// <returns>True if everything is OK. Otherwise it returns false.</returns>
		public bool CheckIntegrity()
		{
			long lastOffset = 0;
			int iLastLine = 1;
			int iLastColumn = 1;

			for( int n = 0; n < m_List.Count; n++ )
			{
				ParsePoint pp = ( ParsePoint )m_List[ n ];

				if( n != 0 )
				{
					if( lastOffset >= pp.Offset )
						return false;

					if( iLastLine > pp.Line )
						return false;

					if( iLastLine == pp.Line && iLastColumn >= pp.Position )
						return false;
				}

				// Saving data
				lastOffset = pp.Offset;
				iLastLine = pp.Line;
				iLastColumn = pp.Position;
			}

			return true;
		}
		/// <summary>
		/// Deletes ParsePoint.
		/// </summary>
		/// <param name="parsePoint">DictionaryEntry instance with point and new offset.</param>
		protected void DeleteParsePointInternal( DictionaryEntry parsePoint )
		{
			ParsePoint point = ( ParsePoint )parsePoint.Key;

			if( point == null ) throw new ArgumentNullException( "parsePoint" );

			long lNewOffset = ( long )parsePoint.Value;

			int index = BinarySearch( point, DEF_PARSEPOINT_SEARCH );

			if( index >= 0 )
			{
				point.RaiseDeletedEvent( lNewOffset );

				( point as IDisposable ).Dispose();

				m_List.RemoveAt( index );
			}
		}
		/// <summary>
		/// Gets enumerator for the specified range of ParsePoints.
		/// </summary>
		/// <param name="start">ParsePoint of the range start.</param>
		/// <param name="end">ParsePoint of the range end.</param>
		/// <returns>Enumerator for array.</returns>
		public IEnumerator GetEnumerator( IParsePoint start, IParsePoint end )
		{
			if( start == null )
				throw new ArgumentNullException( "start" );

			if( end == null )
				throw new ArgumentNullException( "end" );

			int dummy, startIndex, endIndex;

			if( !GetNearestParsePoints( start.Offset, out dummy, out startIndex ) ||
				!GetNearestParsePoints( end.Offset, out endIndex, out dummy ) )
				throw new Exception( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_24 );

			if( startIndex < 0 || endIndex < 0 || endIndex < startIndex )
				throw new Exception( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_25 );

			return GetEnumerator( startIndex, endIndex - startIndex + 1 );
		}
		/// <summary>
		/// Gets enumerator for the specified range of ParsePoints.
		/// </summary>
		/// <param name="start">ParsePoint of the range start.</param>
		/// <returns>Enumerator for array.</returns>
		public IEnumerator GetEnumerator( IParsePoint start )
		{
			if( start == null )
				throw new ArgumentNullException( "start" );

			int dummy, startIndex;

			if( !GetNearestParsePoints( start.Offset, out dummy, out startIndex ) )
				throw new Exception( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_24 );

			if( startIndex < 0 )
				throw new Exception( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_25 );

			return GetEnumerator( startIndex, m_List.Count - startIndex );
		}
		/// <summary>
		/// Gets enumerator for the specified range of ParsePoints.
		/// </summary>
		/// <param name="start">Start index.</param>
		/// <param name="count">Count of ParsePoints.</param>
		/// <returns>Enumerator for array.</returns>
		public IEnumerator GetEnumerator( int start, int count )
		{
			return m_List.GetEnumerator( start, count );
		}
		/// <summary>
		/// Gets enumerator for all ParsePoints.
		/// </summary>
		/// <returns>Enumerator for array.</returns>
		public IEnumerator GetEnumerator()
		{
			return GetEnumerator( 0, m_List.Count );
		}
		/// <summary>
		/// Adds delegate to the after update invoke list.
		/// </summary>
		/// <param name="d">Delegate to add.</param>
		internal void AddDelegateToAfterUpdateInvokeList( Delegate d )
		{
			m_delegatesToInvokeAfterUpdate.Add( d );
		}
		/// <summary>
		/// Checks whether after update invoke list contains specified delegate.
		/// </summary>
		/// <param name="d">Delegate to check.</param>
		/// <returns>True if list contains specified delegate; otherwise false.</returns>
		internal bool AfterUpdateListContains( Delegate d )
		{
			return m_delegatesToInvokeAfterUpdate.Contains( d );
		}
		#endregion

		#region Utility Methods
		/// <summary>
		/// Creates new ParsePoint object and weak reference to it.
		/// </summary>
		/// <param name="iLine">Line for the new ParsePoint.</param>
		/// <param name="iPosition">Position for new ParsePoint.</param>
		/// <param name="lOffset">Offset for new ParsePoint.</param>
		/// <returns>WeakParsePoint instance with newly created ParsePoint as it`s Target.</returns>
		protected ParsePoint CreateNewParsePoint( int iLine, int iPosition, long lOffset )
		{
			ParsePoint point = new ParsePoint( iLine, iPosition, lOffset );
			return point;
		}
		/// <summary>
		/// Invokes binary search in list of ParsePoints. 
		/// </summary>
		/// <param name="value">Value to be searched.</param>
		/// <param name="comparer">Comparer.</param>
		/// <returns>Standart result of the BinarySearch.</returns>
		private int BinarySearch( object value, IComparer comparer )
		{
			if( comparer == null )
				throw new ArgumentNullException( "comparer" );

			int index;

			index = m_List.BinarySearch( value, comparer );

			return index;
		}
		/// <summary>
		/// Invokes delegates that should be invoked after parse points update.
		/// </summary>
		private void InvokeDelegatesAfterUpdate()
		{
			foreach( Delegate dlgt in m_delegatesToInvokeAfterUpdate )
			{
				MethodInfo method = dlgt.Method;

				if( method.GetParameters().Length > 0 )
					throw new Exception();

				method.Invoke( dlgt.Target, null );
			}

			m_delegatesToInvokeAfterUpdate.Clear();
		}
		#endregion

		#region ParsePoint Search Methods
		/// <summary>
		/// Looks for the nearest ParsePoints for the given offset.
		/// </summary>
		/// <param name="StreamOffset">Offset in the stream</param>
		/// <param name="LeftPoint">Returns left nearest point</param>
		/// <param name="RightPoint">Returns right nearest point</param>
		/// <returns>Returns false if there is no parsepoints at all, otherwise returns true</returns>
		public bool GetNearestParsePoints( long StreamOffset, out IParsePoint LeftPoint, out IParsePoint RightPoint )
		{
			int LeftIndex = 0;
			int RightIndex = 0;

			bool res = GetNearestParsePoints( StreamOffset, out LeftIndex, out RightIndex );

			LeftPoint = ( LeftIndex > -1 ) ? ( ParsePoint )m_List[ LeftIndex ] : null;
			RightPoint = ( RightIndex > -1 ) ? ( ParsePoint )m_List[ RightIndex ] : null;

			return res;
		}
		/// <summary>
		/// Looks for the nearest ParsePoints for the given position.
		/// </summary>
		/// <param name="Line">Positions line</param>
		/// <param name="Column">Positions column</param>
		/// <param name="LeftPoint">Returns left nearest point</param>
		/// <param name="RightPoint">Returns right nearest point</param>
		/// <returns>Returns false if there is no parsepoints at all, otherwise returns true</returns>
		public bool GetNearestParsePoints( int Line, int Column, out IParsePoint LeftPoint, out IParsePoint RightPoint )
		{
			int LeftIndex = 0;
			int RightIndex = 0;

			bool res = GetNearestParsePoints( Line, Column, out LeftIndex, out RightIndex );

			LeftPoint = ( LeftIndex > -1 ) ? ( ParsePoint )m_List[ LeftIndex ] : null;
			RightPoint = ( RightIndex > -1 ) ? ( ParsePoint )m_List[ RightIndex ] : null;

			return res;
		}
		/// <summary>
		/// Looks for the nearest ParsePoints for the given offset.
		/// </summary>
		/// <param name="StreamOffset">Offset in the stream</param>
		/// <param name="LeftPointIndex">Index of the left nearest point</param>
		/// <param name="RightPointIndex">Index of the right nearest point</param>
		/// <returns>Returns false if there is no parsepoints at all, otherwise returns true</returns>
		public bool GetNearestParsePoints( long StreamOffset, out int LeftPointIndex, out int RightPointIndex )
		{
			if( m_bInUpdateState )
				throw new Exception( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_18 );

			RightPointIndex = LeftPointIndex = -1;

			int pos = BinarySearch( StreamOffset, DEF_PARSEPOINT_SEARCH );

			if( pos >= 0 )
			{
				LeftPointIndex = RightPointIndex = pos;
			}
			else
			{
				// negative result is a bitwise compliment to the
				// position of an element, that is greater, then given
				pos = ~pos;
				RightPointIndex = ( pos != m_List.Count ) ? pos : -1;
				LeftPointIndex = pos - 1;
			}

			return ( pos >= 0 && pos < m_List.Count );
		}
		/// <summary>
		/// Looks for the nearest ParsePoints for the given position.
		/// </summary>
		/// <param name="Line">Positions line</param>
		/// <param name="Column">Positions column</param>
		/// <param name="LeftPointIndex">Index of the left nearest point</param>
		/// <param name="RightPointIndex">Index of the right nearest point</param>
		/// <returns>Returns false if there is no parsepoints at all, otherwise returns true</returns>
		public bool GetNearestParsePoints( int Line, int Column, out int LeftPointIndex, out int RightPointIndex )
		{
			if( Column < 1 ) throw new ArgumentOutOfRangeException(
				"Column", Column, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_26 );

			if( Line < 1 ) throw new ArgumentOutOfRangeException(
				"Line", Line, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_26 );

			if( m_bInUpdateState )
				throw new Exception( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_18 );

			RightPointIndex = LeftPointIndex = -1;

			IParsePoint TempSearchParsePoint = new ParsePoint( Line, Column, 0 );
			int pos = BinarySearch( TempSearchParsePoint, DEF_PARSEPOINT_SEARCH_POS );

			if( pos >= 0 )
			{
				LeftPointIndex = RightPointIndex = pos;
			}
			else
			{
				// negative result is a bitwise compliment to the position of
				// an element, that is greater, then given
				pos = ~pos;
				RightPointIndex = ( pos != m_List.Count ) ? pos : -1;
				LeftPointIndex = pos - 1;
			}

			return ( m_List.Count > 0 );
		}
		/// <summary>
		/// Looks for ParsePoint, that is left to the given value.
		/// </summary>
		/// <param name="value">Value, to be found</param>
		/// <param name="comparer">Comparer, that is used to find needed value</param>
		/// <returns>ParsePoint, that is on given Value or left to it</returns>
		public IParsePoint GetParsePointByComparer( object value, IComparer comparer )
		{
			if( m_bInUpdateState )
				throw new Exception( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_18 );

			int index = BinarySearch( value, comparer );

			if( index >= 0 )
			{
				return ( ParsePoint )m_List[ index ];
			}
			else
			{
				index = ~index - 1;

				return ( index >= 0 ) ? ( ParsePoint )m_List[ index ] : null;
			}
		}
		#endregion
	}
}