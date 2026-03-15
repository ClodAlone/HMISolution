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

#region file using directives

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Documentation;

#endregion

namespace Syncfusion.Windows.Forms.Chart.Scrolling
{
	/// <summary>
	/// Abstract class that implements the basic functionality of <see cref="IChartScrollBar"/> interface.
	/// </summary>
	/// <internalonly/>
	[ ToolboxItem( false ), DocumentationExclude() ]
	public abstract class ChartScrollBar : UserControl, IChartScrollBar
	{
		#region Class members
		/// <summary>
		/// The <see cref="ScrollBar"/> instance.
		/// </summary>
		protected ScrollBar m_scrollBar = null;
		/// <summary>
		/// The <see cref="ChartZoomButton"/> instance.
		/// </summary>
		protected ChartZoomButton m_zoomButton;
		/// <summary>
		/// Indicates whether complementary scrollbar is visible.
		/// </summary>
		protected bool m_otherVisible = false;
		/// <summary>
		/// Indicates the visibility of control.
		/// </summary>
		protected bool m_visibleInst = false;

		private IChartArea chartArea;
    private ChartAxis chartAxis;
		#endregion

		#region Class properties
		/// <summary>
		/// Gets or sets the ccroll bar's large change value.
		/// </summary>
		/// <value></value>
		public int LargeChange
		{
			get
			{
				return m_scrollBar.LargeChange;
			}
			set
			{
				m_scrollBar.LargeChange = value;
			}
		}
		/// <summary>
		/// Gets or sets the scroll bar's maximum value.
		/// </summary>
		/// <value></value>
		public int Maximum
		{
			get
			{
				return m_scrollBar.Maximum;
			}
			set
			{
				m_scrollBar.Maximum = value;
			}
		}
		/// <summary>
		/// Gets or sets the scroll bar's minimum value.
		/// </summary>
		/// <value></value>
		public int Minimum
		{
			get
			{
				return m_scrollBar.Minimum;
			}
			set
			{
				m_scrollBar.Minimum = value;
			}
		}
		/// <summary>
		/// Returns the Windows scrollbar instance.
		/// </summary>
		/// <value></value>
		public ScrollBar ScrollBar
		{
			get
			{
				return m_scrollBar;
			}
		}
		/// <summary>
		/// Gets the zoom button.
		/// </summary>
		/// <value>The zoom button.</value>
		public ChartZoomButton ZoomButton
		{
			get { return m_zoomButton; }
		}
		/// <summary>
		/// Indicates whether the chart should attempt to position this scrollbar.
		/// </summary>
		/// <value></value>
		public bool ShouldPosition
		{
			get
			{
				return true;
			}
		}
		/// <summary>
		/// Gets or sets the scroll bar's small change value.
		/// </summary>
		/// <value></value>
		public int SmallChange
		{
			get
			{
				return  m_scrollBar.SmallChange;
			}
			set
			{
				m_scrollBar.SmallChange = value;
			}
		}
		/// <summary>
		/// Gets or sets the scroll bar's current value.
		/// </summary>
		/// <value></value>
		public int Value
		{
			get
			{
				return m_scrollBar.Value;
			}
			set
			{
				if (value < m_scrollBar.Minimum)
				{
					value = m_scrollBar.Minimum;
				}
				else if(value > m_scrollBar.Maximum)
				{
					value = m_scrollBar.Maximum;
				}

				m_scrollBar.Value = value;
			}
		}
		/// <summary>
		/// Gets or sets value indicates visibility of scroll bar.
		/// </summary>
		public new virtual bool Visible
		{
			get
			{
				return m_visibleInst;
			}
			set
			{
        if( m_visibleInst != value )
        {
          m_visibleInst = value;
          base.Visible = value;
        }
			}
		}
		/// <summary>
		/// Used for storing chart area reference.
		/// </summary>
		/// <value></value>
		public IChartArea ChartArea 
		{ 
			get
			{
				return chartArea;
			}
			set
			{

				if( chartArea != value )
				{
					chartArea = value;
				}
			} 
		}
		/// <summary>
		/// Used for storing chart area reference.
		/// </summary>
		/// <value></value>
    public ChartAxis ChartAxis 
    { 
      get
      {
        return chartAxis;
      }
      set
      {

        if( chartAxis != value )
        {
          chartAxis = value;
        }
      } 
    }
		/// <summary>
		/// Returns the dimensions of the scroll bar. That is the width for vertical scroll bars and the height for horizontal ones.
		/// </summary>
		/// <value></value>
		/// <internalonly/>
		public int Dimension
		{
			get
			{
				return SystemInformation.VerticalScrollBarWidth * 6 / 7;
			}
		}
		#endregion

		#region Class events
		/// <summary>
		/// Occurs when zoom button is clicked.
		/// </summary>
		public event ChartScrollBarZoomButtonClickedEventHandler ZoomButtonClicked;
		/// <summary>
		/// Occurs when scroll value is changed.
		/// </summary>
    public event ChartScrollBarValueChangedEventHandler ValueChanged;
		#endregion

		#region Class Initialize/Finalize methods
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartScrollBar"/> class.
		/// </summary>
		public ChartScrollBar()
		{
			this.SetStyle( ControlStyles.UserPaint |
				ControlStyles.ContainerControl |
				ControlStyles.DoubleBuffer | 
				ControlStyles.SupportsTransparentBackColor, true );

			this.SetStyle( ControlStyles.AllPaintingInWmPaint | 
				ControlStyles.Selectable, false );

			m_scrollBar = this.CreateScrollBar();
			m_scrollBar.ValueChanged += new EventHandler(this.OnValueChanged);

			m_zoomButton = new ChartZoomButton();
			m_zoomButton.Name = "zoomButton";
			m_zoomButton.ForeColor = Color.Black;
			m_zoomButton.Size = new Size(this.Dimension, this.Dimension);
			m_zoomButton.TabIndex = 0;
			m_zoomButton.Click += new EventHandler(this.OnZoomButtonClick);
			m_zoomButton.VisibleChanged += new EventHandler(OnZoomButtonVisibleChanged);
			m_zoomButton.SizeChanged += new EventHandler(OnZoomButtonSizeChanged);
			m_zoomButton.LocationChanged += new EventHandler(ZoomButtonLocationChanged);

			this.Controls.Add(m_zoomButton);
			this.Controls.Add(m_scrollBar);

			this.BackColor = Control.DefaultBackColor;
			base.Visible = m_visibleInst;
		}
		#endregion

		#region Class public methods
		/// <summary>
		/// Informs this scroll bar that it's complementary scroll bar's visibilty has changed.
		/// </summary>
		/// <param name="b">Visibility flag.</param>
		[Obsolete("This method isn't used anymore and might be deleted.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetOtherVisible(bool b)
		{
			m_otherVisible = b;
		}
		/// <summary>
		/// Sets the position of this scroll bar if the scroll bar is contained within the chart.
		/// </summary>
		/// <param name="rect">Bounding rectangle.</param>
		public virtual void SetPosition(Rectangle rect)
		{
			this.Bounds = rect;
		}
		#endregion

		#region Class abstract methods
		/// <summary>
		/// Creates the scroll bar.
		/// </summary>
		/// <returns></returns>
		protected abstract ScrollBar CreateScrollBar();
		/// <summary>
		/// Arranges controls.
		/// </summary>
		protected abstract void OnLayout();
		/// <summary>
		/// Resets the scrollbar.
		/// </summary>
		/// <internalonly/>
		[Obsolete("This method isn't used anymore and might be deleted.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public abstract void ResetScrollBar();
		#endregion
    
		#region Class utility methods
		/// <summary>
		/// Zooms the button location changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void ZoomButtonLocationChanged(object sender, EventArgs e)
		{
			this.OnSizeChanged(e);
		}
		/// <summary>
		/// Called when zoom button is size changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnZoomButtonSizeChanged(object sender, EventArgs e)
		{
			this.OnSizeChanged(e);
		}
		/// <summary>
		/// Called when zoom button is visible changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnZoomButtonVisibleChanged(object sender, EventArgs e)
		{
			this.OnSizeChanged(e);
		}
		/// <summary>
		/// Handles the Click event of the zoomButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnZoomButtonClick( object sender, EventArgs e )
		{
			if (this.ZoomButtonClicked != null)
			{
				this.ZoomButtonClicked(this, ChartScrollBarZoomButtonClickedEventArgs.Empty);
			}
		}
		/// <summary>
		/// Handles the ValueChanged event of the scrollbar control.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void OnValueChanged( object sender, EventArgs e )
    {
			if (this.ValueChanged != null)
			{
				this.ValueChanged(this, ChartScrollBarValueChangedEventArgs.Empty);
			}
		}
		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.SizeChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnSizeChanged(EventArgs e)
		{
			this.OnLayout();
			base.OnSizeChanged(e);
		}
		#endregion
	}
}