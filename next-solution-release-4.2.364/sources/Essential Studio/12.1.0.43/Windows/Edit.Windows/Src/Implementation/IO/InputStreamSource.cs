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
using System.IO;

using Syncfusion.Windows.Forms.Edit.Interfaces;

namespace Syncfusion.Windows.Forms.Edit.Implementation.IO
{
	/// <summary>
	/// Implementation of ISource, that read data from some stream.
	/// </summary>
	internal class InputStreamSource
		: ISource
	{
		#region Fields
		/// <summary>
		/// 
		/// </summary>
		private Stream m_input;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets position in stream.
		/// </summary>
		public long Position
		{
			get
			{
				return m_input.Position;
			}
			set
			{
				m_input.Position = value;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Initializes instance by stream.
		/// </summary>
		/// <param name="input"></param>
		public InputStreamSource( Stream input )
		{
			m_input = input;
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
				return m_input.Length;
			}
		}
		/// <summary>
		/// Reads data from the stream and writes it to array.
		/// </summary>
		/// <param name="position">Position of data in the source.</param>
		/// <param name="data">Array, where data will be written.</param>
		/// <param name="offset">Offset in array.</param>
		/// <param name="count">Size of data that have to be read.</param>
		/// <returns>Count of actually read bytes.</returns>
		public int GetData( long position, byte[] data, int offset, int count )
		{
			m_input.Position = position;
			return m_input.Read( data, offset, count );
		}
		#endregion
	}
}
