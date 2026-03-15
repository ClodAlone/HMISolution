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
	/// Class that implements some part of data for output stream.
	/// </summary>
	public class DataWindow
		: IDataWindow
		, ICloneable
		, IDisposable
	{
		#region Fields
		/// <summary>
		/// Source for reading data of this stream. More than one windows can use this source.
		/// </summary>
		private ISource m_source;
		/// <summary>
		/// Index of first byte of window's data in Source.
		/// </summary>
		private long m_start;
		/// <summary>
		/// Size of window's data.
		/// </summary>
		private long m_size;
		/// <summary>
		/// Position of window in output stream.
		/// </summary>
		private long m_position;
		#endregion

		#region Properties
		/// <summary>
		/// Gets source for reading data of this stream. More than one windows can use this source.
		/// </summary>
		public ISource Source
		{
			get
			{
				return m_source;
			}
		}
		/// <summary>
		/// Gets or sets index of first byte of window's data in Source.
		/// </summary>
		public long Start
		{
			get
			{
				return m_start;
			}
			set
			{
				if( value < 0 ) throw new ArgumentOutOfRangeException(
					"Start", value, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_53 );

				m_start = value;
			}
		}
		/// <summary>
		/// Gets or sets size of window's data.
		/// </summary>
		public long Size
		{
			get
			{
				return m_size;
			}
			set
			{
				if( value < 0 ) throw new ArgumentOutOfRangeException(
					"Size", value, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_54 );

				m_size = value;
			}
		}
		/// <summary>
		/// Position of window in output stream.
		/// </summary>
		public long Position
		{
			get
			{
				return m_position;
			}
			set
			{
				if( value < 0 ) throw new ArgumentOutOfRangeException(
					"Position", value, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_55 );

				m_position = value;
			}
		}
		/// <summary>
		/// True - if class was disposed
		/// </summary>
		public bool IsDisposed
		{
			get
			{
				return ( m_source == null );
			}
		}
		#endregion

		#region Initialization And Finalization
		/// <summary>
		/// Initializes data window by full size of source.
		/// </summary>
		/// <param name="source">Source of data for window.</param>
		public DataWindow( ISource source )
			: this( source, 0, source.Length )
		{
		}
		/// <summary>
		/// Initializes data window.
		/// </summary>
		/// <param name="source">Source of data for window.</param>
		/// <param name="start">Start position of the window's data in source.</param>
		/// <param name="size">Size of the window's data in source.</param>
		public DataWindow( ISource source, long start, long size )
		{
			if( source == null ) throw new ArgumentNullException( "source" );
			if( start < 0 ) throw new ArgumentOutOfRangeException(
				"start", start, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_17 );
			if( size < 0 ) throw new ArgumentOutOfRangeException(
				"size", size, Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_17 );

			m_source = source;
			m_start = start;
			m_size = size;
		}
		/// <summary>
		/// Destoroy Class
		/// </summary>
		~DataWindow()
		{
			Dispose();
		}
		/// <summary>
		/// Deletes link to source to speed up it`s finalization
		/// </summary>
		public void Dispose()
		{
			if( !IsDisposed )
			{
				m_source = null;
				GC.SuppressFinalize( this );
			}
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Represents data window in format "source:   xxx start:   xxx  position:    xxx". Mostly needed for debug purposes.
		/// </summary>
		public override string ToString()
		{
			return string.Format( "source: [{0,60}] start: {1,6} size: {2,4} position: {3,6}", m_source, m_start, m_size, m_position );
		}
		#endregion

		#region ICloneable Members
		/// <summary>
		/// Clones DataWindow
		/// </summary>
		/// <returns>New DataWindow</returns>
		object ICloneable.Clone()
		{
			if( IsDisposed ) throw new ArgumentException( Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_56 );

			return this.MemberwiseClone();
		}
		/// <summary>
		/// Clones DataWindow
		/// </summary>
		/// <returns>New DataWindow</returns>
		public DataWindow Clone()
		{
			return ( ( ICloneable )this ).Clone() as DataWindow;
		}
		#endregion
	}
}
