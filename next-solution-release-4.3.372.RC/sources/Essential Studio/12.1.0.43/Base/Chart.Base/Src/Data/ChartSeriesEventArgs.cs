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
using System.ComponentModel;
using System.Diagnostics;

using Syncfusion.Documentation;

namespace Syncfusion.Windows.Forms.Chart
{
	/// <summary>
	/// Provides data of <see cref="ChartSeriesChangedEventHandler"/>.
	/// </summary>
	public class ChartSeriesChangedEventArgs : EventArgs
	{
		#region Internal types
		/// <summary>
		/// Specifies the type of event that occurred.
		/// </summary>
		public enum Type
		{
			/// <summary>
			/// Specifies that the datasource was reset. All data is expected to have changed.
			/// </summary>
			Reset,
			/// <summary>
			/// Specifies that data has been inserted.
			/// </summary>
			Inserted,
			/// <summary>
			/// Specifies that data has been removed.
			/// </summary>
			Removed,
			/// <summary>
			/// Specifies that data has been changed.
			/// </summary>
			Changed
		}
		#endregion

		#region Members
		private Type m_type;
		#endregion

		#region Properties
		/// <summary>
		/// Returns the type of event that occurred.
		/// <seealso cref="ChartSeriesChangedEventArgs.Type"/>
		/// </summary>
		public Type EventType
		{
			get
			{
				return m_type;
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartSeriesChangedEventArgs"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		internal ChartSeriesChangedEventArgs(Type type)
		{
			m_type = type;
		}
		#endregion

		#region Implementation
		/// <summary>
		/// Helper method that creates ChartDataChangedEventArgs from ListChangedEventArgs.
		/// </summary>
		/// <param name="args" type="System.ComponentModel.ListChangedEventArgs">
		///     <para>
		///     ListChangedEventArgs object; information that will be used to create the ChartDataChangedEventArgs object.
		///     </para>
		/// </param>
		/// <param name="chartData" type="Syncfusion.Windows.Forms.Chart.IChartSeriesModel">
		///     <para>
		///     Not used in the current version.
		///     </para>
		/// </param>
		/// <returns>
		///     A Syncfusion.Windows.Forms.Chart.ChartDataChangedEventArgs value.
		/// </returns>
		public static ChartSeriesChangedEventArgs FromListChangedEventArgs( ListChangedEventArgs args, IChartSeriesModel chartData )
		{
			switch( args.ListChangedType )
			{
				case ListChangedType.ItemAdded:
				{
					return ChartSeriesChangedEventArgs.CreateInsertEventArgs();
				}

				case ListChangedType.ItemChanged:
				{
					return ChartSeriesChangedEventArgs.CreateChangedEventArgs();
				}

				case ListChangedType.ItemDeleted:
				{
					return ChartSeriesChangedEventArgs.CreateRemovedEventArgs();
				}

				case ListChangedType.Reset:
				{
					return ChartSeriesChangedEventArgs.CreateResetEventArgs();
				}
			}

			return null;
		}
		/// <summary>
		/// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </returns>
		/// <internalonly/>
		public override string ToString()
		{
			return string.Format( "Type: {0}, Index: {1}", m_type, -1 );
		}

		/// <summary>
		/// Creates the reset event args.
		/// </summary>
		/// <returns></returns>
    internal static ChartSeriesChangedEventArgs CreateResetEventArgs()
		{
      return new ChartSeriesChangedEventArgs( Type.Reset );
		}
		/// <summary>
		/// Creates the insert event args.
		/// </summary>
		/// <returns></returns>
    internal static ChartSeriesChangedEventArgs CreateInsertEventArgs()
    {
      return new ChartSeriesChangedEventArgs(Type.Inserted);
    }
		/// <summary>
		/// Creates the removed event args.
		/// </summary>
		/// <returns></returns>
    internal static ChartSeriesChangedEventArgs CreateRemovedEventArgs()
    {
      return new ChartSeriesChangedEventArgs(Type.Removed);
    }
		/// <summary>
		/// Creates the changed event args.
		/// </summary>
		/// <returns></returns>
    internal static ChartSeriesChangedEventArgs CreateChangedEventArgs()
    {
      return new ChartSeriesChangedEventArgs(Type.Changed);
    }
		#endregion
	}
}