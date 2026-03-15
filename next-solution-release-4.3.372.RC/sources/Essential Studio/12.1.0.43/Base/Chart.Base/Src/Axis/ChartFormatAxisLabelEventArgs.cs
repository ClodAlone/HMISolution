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
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Chart
{
	/// <summary>
	///     Delegate that is to be used with ChartControl.
	/// </summary>
	/// <param name="sender" type="object">
	///     <para>
	///     Sender.  
	///     </para>
	/// </param>
	/// <param name="e" type="Syncfusion.Windows.Forms.Chart.ChartFormatAxisLabelEventArgs">
	///     <para>
	///     Event argument.
	///     </para>
	/// </param>
	/// <remarks>
	///     
	/// </remarks>
	public delegate void ChartFormatAxisLabelEventHandler(object sender, ChartFormatAxisLabelEventArgs e);

	/// <summary>
	///     Delegate that is to be used with ChartAxis.
	/// </summary>
	/// <param name="sender" type="object">
	///     Sender.  
	/// </param>
	/// <param name="args" type="Syncfusion.Windows.Forms.Chart.ChartAxisZoomingArgs">
	///     Event argument.
	/// </param>
	public delegate void ChartAxisZoomingEventHandler(object sender, ChartAxisZoomingArgs args);

	/// <summary>
	///   Argument that is to be used with ChartControl.
	/// </summary>
	public class ChartFormatAxisLabelEventArgs : EventArgs
	{
		#region Members
		private ChartAxis m_axis = null;
		private bool m_handled;
		private bool m_isPrimary;

		private string m_label;
        private string m_toolTip;
		private double m_value;
        private ChartPlacement m_axisLabelPlacement;  
		#endregion

		#region Properties
		/// <summary>
		///     Returns the orientation of the axis for which the label is being generated.
		/// </summary>
		public ChartOrientation AxisOrientation
		{
			get
			{
				return m_axis.Orientation;
			}
		}
		/// <summary>
		///     Indicates whether this event was handled and no further processing is required from the chart.
		/// </summary>
		public bool Handled
		{
			get
			{
				return m_handled;
			}
			set
			{
				m_handled = value;
			}
		}
		/// <summary>
		///     Indicates whether the axis for which the label is being generated is a primary axis.
		/// </summary>
		public bool IsAxisPrimary
		{
			get
			{
				return m_isPrimary;
			}
		}
		/// <summary>
		/// Gets or sets the label that is to be rendered.
		/// </summary>
		public string Label
		{
			get
			{
				return m_label;
			}
			set
			{
				m_label = value;
			}
		}
        /// <summary>
        /// Gets or sets the tooltip for the label that is to be rendered.
        /// </summary>
        public string ToolTip
        {
            get
            {
                return m_toolTip;
            }
            set
            {
                m_toolTip = value;
            }
        }
		/// <summary>
		///     Returns the value associated with the position of the label.
		/// </summary>
		public double Value
		{
			get
			{
				return m_value;
			}
		}
		/// <summary>
		///     Returns the value associated with the position of the label as DateTime.
		/// </summary>
		public DateTime ValueAsDate
		{
			get
			{
				return DateTime.FromOADate(m_value);
			}
		}
		/// <summary>
		/// Gets the axis.
		/// </summary>
		/// <value>The axis.</value>
		public ChartAxis Axis
		{
			get
			{
				return m_axis;
			}
		}
        /// <summary>
        /// Gets or sets a value indicates whether every label is located Right or Left of Y-axis and Top or Bottom of X-Axis line.
        /// </summary>
         public ChartPlacement AxisLabelPlacement
        {
            get
            {
                return m_axisLabelPlacement; 
            }
            set
            {
                     m_axisLabelPlacement = value;
             }
        }
		#endregion

		#region Constructor
		/// <summary>
		///     Constructor.
		/// </summary>
		/// <param name="label" type="string">
		///     <para>
		///     The text of the label.    
		///     </para>
		/// </param>
		/// <param name="value" type="double">
		///     <para>
		///     The value of the point associated with the label.   
		///     </para>
		/// </param>
		/// <param name="axis" type="Syncfusion.Windows.Forms.Chart.ChartAxis">
		///     <para>
		///     The axis associated with the label.    
		///     </para>
		/// </param>
		public ChartFormatAxisLabelEventArgs(string label, double value, ChartAxis axis)
		{
			m_axis = axis;
			m_label = label;
			m_value = value;
			m_isPrimary = axis.Primary;
            m_axisLabelPlacement = axis.AxisLabelPlacement;
			m_handled = false;
		}
		#endregion
	}

	/// <summary>
	/// <see cref="ChartAxisZoomingArgs"/> is the base class for classes containing event data.
	/// </summary>
	public sealed class ChartAxisZoomingArgs : EventArgs
	{
		#region Members
		private readonly ChartAxis m_axis;
		private readonly MinMaxInfo m_range;
		private readonly double m_zoomFactor;
		private readonly double m_zoomPosition;
		private bool m_cancel = false;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the axis.
		/// </summary>
		/// <value>The axis.</value>
		public ChartAxis Axis
		{
			get { return m_axis; }
		}
		/// <summary>
		/// Gets the new visible range.
		/// </summary>
		/// <value>The range.</value>
		public MinMaxInfo VisibleRange
		{
			get { return m_range; }
		}
		/// <summary>
		/// Gets the zoom factor of current axis.
		/// </summary>
		/// <value>The zoom factor.</value>
		public double ZoomFactor
		{
			get { return m_zoomFactor; }
		}
		/// <summary>
		/// Gets the zoom position of current axis.
		/// </summary>
		/// <value>The zoom position.</value>
		public double ZoomPosition
		{
			get { return m_zoomPosition; }
		}
		/// <summary>
		/// Gets or sets a value indicating whether the zooming should be canceled. 
		/// </summary>
		/// <value><c>true</c> if zooming is canceled; otherwise, <c>false</c>.</value>
		public bool Cancel
		{
			get { return m_cancel; }
			set { m_cancel = value; }
		}
		#endregion

		#region Constrcutor
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartAxisZoomingArgs"/> class.
		/// </summary>
		/// <param name="axis">The axis.</param>
		/// <param name="range">The range.</param>
		/// <param name="zoomFactor">The zoom factor.</param>
		/// <param name="zoomPosition">The zoom position.</param>
		internal ChartAxisZoomingArgs(ChartAxis axis, MinMaxInfo range, double zoomFactor, double zoomPosition)
		{
			m_axis = axis;
			m_range = range;
			m_zoomFactor = zoomFactor;
			m_zoomPosition = zoomPosition;
		}
		#endregion
	}
}
