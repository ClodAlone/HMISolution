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
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Documentation;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Chart.Scrolling
{

	/// <summary>
	/// Placeholder for additional information. Currently does not hold any additional information.
	/// </summary>
	public class ChartScrollBarValueChangedEventArgs : EventArgs
	{

		/// <internalonly/>
		[ DocumentationExclude() ]
		public new static ChartScrollBarValueChangedEventArgs Empty;

		static ChartScrollBarValueChangedEventArgs()
		{
			ChartScrollBarValueChangedEventArgs.Empty = new ChartScrollBarValueChangedEventArgs();
		}
	}
    ///<exclude/>
	/// <summary>
	///     Delegate that is to be used when the chart scroll bar value changes.
	///     <seealso cref="IChartScrollBar"/>
	/// </summary>
	/// <param name="sender" type="object">
	///     <para>
	///     Sender.
	///     </para>
	/// </param>
	/// <param name="args" type="Syncfusion.Windows.Forms.Chart.Scrolling.ChartScrollBarValueChangedEventArgs">
	///     <para>
	///     Argument.
	///     </para>
	/// </param>
	public delegate void ChartScrollBarValueChangedEventHandler( object sender, ChartScrollBarValueChangedEventArgs args );

	/// <summary>
	/// Placeholder for additional information. Currently does not hold any additional information.
	/// </summary>
	public class ChartScrollBarZoomButtonClickedEventArgs : EventArgs
	{

		/// <internalonly/>
		[ DocumentationExclude() ]
		public new static ChartScrollBarZoomButtonClickedEventArgs Empty;

		static ChartScrollBarZoomButtonClickedEventArgs()
		{
			ChartScrollBarZoomButtonClickedEventArgs.Empty = new ChartScrollBarZoomButtonClickedEventArgs();
		}
	}
    ///<exclude/>
	/// <summary>
	///     Delegate that is to be used when the zoom button associated with a chart scroll bar is clicked.
	///     <seealso cref="IChartScrollBar"/>
	/// </summary>
	/// <param name="sender" type="object">
	///     <para>
	///     Sender.
	///     </para>
	/// </param>
	/// <param name="args" type="Syncfusion.Windows.Forms.Chart.Scrolling.ChartScrollBarZoomButtonClickedEventArgs">
	///     <para>
	///     Argument.
	///     </para>
	/// </param>
	public delegate void ChartScrollBarZoomButtonClickedEventHandler( object sender, ChartScrollBarZoomButtonClickedEventArgs args );

	/// <summary>
	/// This interface codifies interaction of scrollbars with the <see cref="ChartArea"/>.
	/// </summary>
	public interface IChartScrollBar
	{
		/// <summary>
		///     Event that is to be raised when the zoom buttom that is associated with this scroll bar is clicked.
		/// </summary>
		event ChartScrollBarZoomButtonClickedEventHandler ZoomButtonClicked;

		/// <summary>
    ///     Event that is to be raised when value of scroll bar changes.
    /// </summary>
    event ChartScrollBarValueChangedEventHandler ValueChanged;

		/// <summary>
		///     Informs this scroll bar that it's complementary scroll bar's visibilty has changed.
		/// </summary>
		/// <param name="b" type="bool">
		///     <para>
		///     Visibility flag.
		///     </para>
		/// </param>
		[Obsolete("This method isn't used anymore and might be deleted.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void SetOtherVisible(bool b);

		/// <summary>
		///     Sets the position of this scroll bar if the scroll bar is contained within the chart.
		/// </summary>
		/// <param name="rect" type="System.Drawing.Rectangle">
		///     <para>
		///     Bounding rectangle.
		///     </para>
		/// </param>
		void SetPosition( Rectangle rect );

		/// <summary>
		/// Resets the scroll bar.
		/// </summary>
		[Obsolete("This method isn't used anymore and might be deleted.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void ResetScrollBar();

		/// <summary>
		/// Returns the dimensions of the scroll bar. That is the width for vertical scroll bars and the height for horizontal ones.
		/// </summary>
		int Dimension { get; }

		/// <summary>
		///  Gets or sets the ccroll bar's large change value.
		/// </summary>
		int LargeChange { get; set; }

		/// <summary>
		///  Gets or sets the scroll bar's maximum value.
		/// </summary>
		int Maximum { get; set; }

		/// <summary>
		///  Gets or sets the scroll bar's minimum value.
		/// </summary>
		int Minimum { get; set; }

		/// <summary>
		///  Returns the Windows scrollbar instance.
		/// </summary>
		ScrollBar ScrollBar { get; }
		/// <summary>
		/// Gets the zoom button.
		/// </summary>
		/// <value>The zoom button.</value>
		ChartZoomButton ZoomButton { get; }

		/// <summary>
		///  Indicates whether the chart should attempt to position this scrollbar.
		/// </summary>
		bool ShouldPosition { get; }

		/// <summary>
		///  Gets or sets the scroll bar's small change value.
		/// </summary>
		int SmallChange { get; set; }

		/// <summary>
		///  Gets or sets the scroll bar's current value.
		/// </summary>
		int Value { get; set; }

		/// <summary>
		///  Indicates the scroll bar's visibility flag.
		/// </summary>
		bool Visible { get; set; }
		/// <summary>
		/// Used for storing chart area reference.
		/// </summary>
		IChartArea ChartArea { get; set; }
    /// <summary>
    /// Used for storing chart area reference.
    /// </summary>
    ChartAxis ChartAxis { get; set; }
	}
}