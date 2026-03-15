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
using System.Diagnostics;
using System.Collections;
using System.Runtime;
using System.Runtime.InteropServices;

namespace Syncfusion.Windows.Forms.Edit.Utils
{
	/// <summary>
	/// The most accurate time counter.
	/// </summary>
	public struct TimeCounter
	{
		#region Class members
		private Int64 m_Start;
		private float m_difference;
		private bool m_bHaveResults;
		#endregion

		#region Class Public Methods
		/// <summary>
		/// Starts counting.
		/// </summary>
		[Conditional( "DEBUG" )]
		public void Start()
		{
			m_bHaveResults = false;
			m_Start = 0;
			QueryPerformanceCounter( ref m_Start );
		}

		/// <summary>
		/// Stops counting.
		/// </summary>
		[Conditional( "DEBUG" )]
		public void Finish()
		{
			Int64 finish = 0;
			QueryPerformanceCounter( ref finish );

			Int64 freq = 0;
			QueryPerformanceFrequency( ref freq );

			m_difference = ( ( ( float )( finish - m_Start ) / ( float )freq ) );
			m_bHaveResults = true;
		}

		#endregion

		#region Class Properties
		/// <summary>
		/// Result of the timer.
		/// </summary>
		public float Result
		{
			get
			{
				return ( m_bHaveResults ) ? m_difference : -1;
			}
		}

		#endregion

		#region DLL Import
		[DllImport( "Kernel32.dll" )]
		static extern bool QueryPerformanceCounter( ref Int64 performanceCount );
		[DllImport( "Kernel32.dll" )]
		static extern bool QueryPerformanceFrequency( ref Int64 frequency );
		#endregion
	}
}
