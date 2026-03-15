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
	/// <summary>
	/// Instance of the change. Also it implements ISource interface, so it can be used as data source for data window.
	/// </summary>
	public class ChangeContext
		: IChange
		, ISource
		, ICloneable
		, IDisposable
	{
		#region Fields
		/// <summary>
		/// Type of change.
		/// </summary>
		private ChangeType m_type;
		/// <summary>
		/// Array of bytes with data changed.
		/// </summary>
		private byte[] m_data;
		/// <summary>
		/// Size of data changed.
		/// </summary>
		private long m_size;
		/// <summary>
		/// Position of change in stream.
		/// </summary>
		private long m_pos;
		#endregion

		#region Initialization
		/// <summary>
		/// Initializes instance. Size is auto-calculated from data.
		/// </summary>
		/// <param name="type">Type of change.</param>
		/// <param name="data">Data for change.</param>
		public ChangeContext( ChangeType type, byte[] data )
			: this( type, data, ( data != null ) ? data.Length : 0 )
		{
		}
		/// <summary>
		/// Initializes instance.
		/// </summary>
		/// <param name="type">Type of change.</param>
		/// <param name="data">Data for change.</param>
		/// <param name="size">Size of change (how much to delete or replace).</param>
		public ChangeContext( ChangeType type, byte[] data, long size )
		{
			if( size < 0 ) throw new ArgumentOutOfRangeException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_123,
				size, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_17 );

			if( type == ChangeType.Insert || type == ChangeType.Replace )
			{
				if( data == null ) throw new ArgumentNullException( "data" );
				if( data.Length == 0 ) throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_122 );
			}

			if( ( type == ChangeType.Replace || type == ChangeType.Delete ) && size == 0 )
				throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_123 );

			m_type = type;
			m_data = ( data != null ) ? ( byte[] )data.Clone() : null;
			m_size = size;
		}
		/// <summary>
		/// Destructor, calls Dispose.
		/// </summary>
		~ChangeContext()
		{
			Dispose();
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Format of the output string is "type: {0,8} length: {1,6} size: {2,6} position: {3,6}"
		/// Mostly for debug purposes.
		/// </summary>
		/// <returns>String representation.</returns>
		public override string ToString()
		{
			return string.Format( "type: {0,8} length: {1,6} size: {2,6} position: {3,6}", m_type, Length, m_size, m_pos );
		}
		#endregion

		#region IChange Members
		/// <summary>
		/// Gets or sets position in stream.
		/// </summary>
		public long Position
		{
			get
			{
				return m_pos;
			}
			set
			{
				if( value < 0 ) throw new ArgumentOutOfRangeException(
					"Position", value, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_17 );

				m_pos = value;
			}
		}
		/// <summary>
		/// Gets type of change.
		/// </summary>
		public ChangeType Type
		{
			get
			{
				return m_type;
			}
		}
		/// <summary>
		/// Gets data for change (for replace and insert).
		/// </summary>
		public byte[] Data
		{
			get
			{
				return m_data;
			}
		}
		/// <summary>
		/// Gets size of data to be affected (for delete and replace).
		/// </summary>
		public long Size
		{
			get
			{
				return m_size;
			}
		}
		#endregion

		#region ISource Members
		/// <summary>
		/// Length of data in source.
		/// </summary>
		public long Length
		{
			get
			{
				return ( m_data != null ) ? m_data.Length : 0;
			}
		}
		/// <summary>
		/// Reads data and writes it to array.
		/// </summary>
		/// <param name="position">Position of data in the source.</param>
		/// <param name="data">Array, where data will be written.</param>
		/// <param name="offset">Offset in array.</param>
		/// <param name="count">Size of data that have to be read.</param>
		/// <returns>Count of actually read bytes.</returns>
		public int GetData( long position, byte[] data, int offset, int count )
		{
			if( count < 0 || count > Length ) throw new ArgumentOutOfRangeException(
				"count", count, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_121 );

			int result = 0;
			if( this.Length != 0 )
			{
				long offsetSource = position;
				int copy = Math.Min( ( int )( this.Length - offsetSource ), count );
				if( copy < 0 ) throw new ArgumentException();
				Buffer.BlockCopy( m_data, ( int )offsetSource, data, offset, copy );
				result = copy;
			}
			return result;
		}
		#endregion

		#region ICloneable Members
		/// <summary>
		/// Clones Context
		/// </summary>
		/// <returns>New ChangeContext</returns>
		object ICloneable.Clone()
		{
			return this.MemberwiseClone();
		}
		/// <summary>
		/// Clones Context
		/// </summary>
		/// <returns>New ChangeContext</returns>
		public ChangeContext Clone()
		{
			return ( ( ICloneable )this ).Clone() as ChangeContext;
		}
		#endregion

		#region IDisposable Members
		/// <summary>
		/// Deletes link to source to speed up it`s finalization
		/// </summary>
		public void Dispose()
		{
			m_data = null;
		}
		#endregion
	}
}