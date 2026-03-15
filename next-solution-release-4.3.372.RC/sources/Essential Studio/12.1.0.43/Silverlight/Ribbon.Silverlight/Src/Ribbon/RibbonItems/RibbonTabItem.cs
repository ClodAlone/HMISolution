#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Syncfusion.Windows.Tools.Controls 
{
	/// <summary>
	/// Reprisents tab item of ribbons' tab strip.
	/// </summary>
    /// 

    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
    Type = typeof(TabButton), XamlResource = "/Syncfusion.Theming.Blend;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(TabButton), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(TabButton), XamlResource = "/Syncfusion.Theming.Office2007Black;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(TabButton), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
         Type = typeof(TabButton), XamlResource = "/Syncfusion.Ribbon.Silverlight;component/themes/generic.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
        Type = typeof(TabButton), XamlResource = "/Syncfusion.Theming.Office2003;component/Ribbon.xaml")]

    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
       Type = typeof(TabButton), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
       Type = typeof(TabButton), XamlResource = "/Syncfusion.Theming.Office2010Black;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
       Type = typeof(TabButton), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
       Type = typeof(TabButton), XamlResource = "/Syncfusion.Theming.VS2010;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
       Type = typeof(TabButton), XamlResource = "/Syncfusion.Theming.Metro;component/Ribbon.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent,
   Type = typeof(TabButton), XamlResource = "/Syncfusion.Theming.Transparent;component/Ribbon.xaml")]

	[TemplateVisualState(GroupName = "CommonStates", Name = "Normal")]
	[TemplateVisualState(GroupName = "CommonStates", Name = "MouseOver")]
	[TemplateVisualState(GroupName = "CommonStates", Name = "Selected")]
	[TemplateVisualState(GroupName = "CommonStates", Name = "SelectedMouseOver")]
	public class TabButton : Control
	{
		#region Constants

		private const int DOUBLECLICKINTERVALMS = 500;	// Milliseconds.
		private readonly static TimeSpan TSDOUBLECLICKINTERVAL = new TimeSpan(0, 0, 0, 0, DOUBLECLICKINTERVALMS);

		#endregion

		#region Fields

		private bool mouseOver = false;
		private IRibbonTabItemSelector selector;
		private DateTime? lastLeftMouseDown = null;
		private Border borderSelected = null;
		private Border borderHighlighted = null;

		private Brush brushSelected = null;
		private Brush brushHighlighted = null;

		private Brush brushSelectedOriginal = null;
		private Brush brushHighlightedOriginal = null;
		#endregion

		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="TabButton"/> class.
		/// </summary>
		public TabButton()
		{
			this.DefaultStyleKey = typeof(TabButton);

			this.MinWidth = 50;
		}

		#endregion

		#region Caption

		/// <summary>
		/// Gets or sets the caption.
		/// </summary>
		public string Caption
		{
			get { return (string)GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}

		/// <summary>
		/// Event that is raised when <see cref="TabButton.Caption"/> property is changed.
		/// </summary>
		public event PropertyChangedCallback CaptionChanged;

		/// <summary>
		/// The identifier for the <see cref="TabButton.Caption"/> dependency property. 
		/// </summary>
		public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register(
			"Caption", 
			typeof(string), 
			typeof(TabButton), 
			new PropertyMetadata(null, new PropertyChangedCallback(OnCaptionChanged)));

		/// <summary>
		/// Calls OnCaptionChanged method of the instance, notifies of the depencency property value changes.
		/// </summary>
		/// <param name="d">Dependency object, the change occures on.</param>
		/// <param name="e">Property change details, such as old value and new value.</param>
		private static void OnCaptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TabButton instance = (TabButton)d;
			instance.OnCaptionChanged(e);
		}

		/// <summary>
		/// Raises CaptionChanged event.
		/// </summary>
		/// <param name="e">Property change details, such as old value and new value.</param>
		private void OnCaptionChanged(DependencyPropertyChangedEventArgs e)
		{
			if (this.CaptionChanged != null)
			{
				this.CaptionChanged(this, e);
			}
		}

		#endregion

		#region SeparatorOpacity

		/// <summary>
		/// Gets or sets SeparatorOpacity.
		/// </summary>
		public double SeparatorOpacity
		{
			get { return (double)GetValue(SeparatorOpacityProperty); }
			set { SetValue(SeparatorOpacityProperty, value); }
		}

		/// <summary>
		/// Event that is raised when <see cref="TabButton.SeparatorOpacity"/> property is changed.
		/// </summary>
		public event PropertyChangedCallback SeparatorOpacityChanged;

		/// <summary>
		/// The identifier for the <see cref="TabButton.SeparatorOpacity"/> dependency property. 
		/// </summary>
		public static readonly DependencyProperty SeparatorOpacityProperty = DependencyProperty.Register(
			"SeparatorOpacity", 
			typeof(double),
			typeof(TabButton), 
			new PropertyMetadata(0.0, new PropertyChangedCallback(OnSeparatorOpacityChanged)));

		/// <summary>
		/// Calls OnSeparatorOpacityChanged method of the instance, notifies of the depencency property value changes.
		/// </summary>
		/// <param name="d">Dependency object, the change occures on.</param>
		/// <param name="e">Property change details, such as old value and new value.</param>
		private static void OnSeparatorOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			TabButton instance = (TabButton)d;
			instance.OnSeparatorOpacityChanged(e);
		}

		/// <summary>
		/// Raises SeparatorOpacityChanged event.
		/// </summary>
		/// <param name="e">Property change details, such as old value and new value.</param>
		private void OnSeparatorOpacityChanged(DependencyPropertyChangedEventArgs e)
		{
			if (this.SeparatorOpacityChanged != null)
			{
				this.SeparatorOpacityChanged(this, e);
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets a value indicating whether this tab item is selected.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this tab item is selected; otherwise, <c>false</c>.
		/// </value>
		public bool IsSelected
		{
			get
			{
				return this.isSelected;
			}

			set
			{
				if (this.isSelected != value)
				{
					this.isSelected = value;

					this.OnIsSelectedChanged();
				}
			}
		}

        /// <summary>
        /// Gets the state of the visual.
        /// </summary>
        /// <value>The state of the visual.</value>
		internal string VisualState
		{
			get
			{
				string state = "Normal";
				bool isSelected = this.IsSelected;

				if (isSelected)
				{
					RibbonTabStrip rts = this.Parent as RibbonTabStrip;

					if (rts != null)
					{
						isSelected = rts.ShowSelectedTab;
					}
				}

				if (isSelected)
				{
					if (this.mouseOver)
					{
						state = "SelectedMouseOver";
					}
					else
					{
						state = "Selected";
					}
				}
				else
				{
					if (this.mouseOver)
					{
						state = "MouseOver";
					}
				}

				return state;
			}
		}

        /// <summary>
        /// Gets or sets the selector.
        /// </summary>
        /// <value>The selector.</value>
		internal IRibbonTabItemSelector Selector
		{
			get { return this.selector; }
			set { this.selector = value; }
		}

        /// <summary>
        /// Sets the selected brush.
        /// </summary>
        /// <value>The selected brush.</value>
		internal Brush SelectedBrush
		{
			set
			{
				if (this.brushSelected != value)
				{
					this.brushSelected = value;

					this.OnSelectedBrushChanged();
				}
			}
		}

        /// <summary>
        /// Sets the highlighted brush.
        /// </summary>
        /// <value>The highlighted brush.</value>
		internal Brush HighlightedBrush
		{
			set
			{
				if (this.brushHighlighted != value)
				{
					this.brushHighlighted = value;
					
					this.OnHighlightedBrushChanged();
				}
			}
		}

		#endregion

		#region Overrides

		/// <summary>
		/// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			this.borderSelected = GetTemplateChild("SelectedBackgroundBorder") as Border;

			if (this.borderSelected != null)
			{
				this.brushSelectedOriginal = this.borderSelected.Background;

				this.OnSelectedBrushChanged();
			}

			this.borderHighlighted = GetTemplateChild("HighlightBorder") as Border;
			
			if (this.borderHighlighted != null)
			{
				this.brushHighlightedOriginal = this.borderHighlighted.Background;

				this.OnHighlightedBrushChanged();
			}

			this.UpdateVisualState();
		}		

		/// <summary>
		/// Called before the <see cref="E:System.Windows.UIElement.MouseEnter"/> event occurs.
		/// </summary>
		/// <param name="e">The data for the event.</param>
		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);

			this.mouseOver = true;

			this.UpdateVisualState();
		}

		/// <summary>
		/// Called before the <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> event occurs.
		/// </summary>
		/// <param name="e">The data for the event.</param>
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);

			this.ProcessMouseDown(e);
		}

		/// <summary>
		/// Called before the <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/> event occurs.
		/// </summary>
		/// <param name="e">The data for the event.</param>
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonUp(e);
		}

		/// <summary>
		/// Called before the <see cref="E:System.Windows.UIElement.MouseLeave"/> event occurs.
		/// </summary>
		/// <param name="e">The data for the event.</param>
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);

			this.mouseOver = false;

			this.UpdateVisualState();
		}

		#endregion

		#region Event handlers

        /// <summary>
        /// Called when [tab is selected changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OntaTabIsSelectedChanged(object sender, EventArgs e)
		{
			RibbonTab tab = sender as RibbonTab;

			if (tab != null)
			{
				this.IsSelected = tab.IsChecked;
			}
		}

		#endregion

		#region Implementation

        /// <summary>
        /// Updates the state of the visual.
        /// </summary>
		internal void UpdateVisualState()
		{
			VisualStateManager.GoToState(this, this.VisualState, true);
		}

        internal event EventHandler OnItemClick;

        /// <summary>
        /// Processes the mouse down.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
		internal void ProcessMouseDown(MouseButtonEventArgs e)
		{
			if (this.ContainsMouse(e))
			{
				if (this.selector != null)
				{
					this.selector.OnMouseDown(this);

					DateTime? dtLastMouseDown = this.lastLeftMouseDown;

					this.lastLeftMouseDown = DateTime.Now;

					if (dtLastMouseDown.HasValue)
					{
						TimeSpan timeSpan = new TimeSpan(this.lastLeftMouseDown.Value.Ticks - dtLastMouseDown.Value.Ticks);

						if (timeSpan <= TSDOUBLECLICKINTERVAL && this.selector != null)
						{
							this.selector.OnDoubleClick(this);
						}
					}
				}
                if (OnItemClick != null)
                    OnItemClick(this, e);
				this.IsSelected = true;
			}
		}

        /// <summary>
        /// Determines whether the specified e contains mouse.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>
        /// 	<c>true</c> if the specified e contains mouse; otherwise, <c>false</c>.
        /// </returns>
		private bool ContainsMouse(MouseEventArgs e)
		{
			Point pt = e.GetPosition(this);

			return pt.X > 0 && pt.Y > 0 && pt.X < this.ActualWidth && pt.Y < this.ActualHeight;
		}

        /// <summary>
        /// Called when [selected brush changed].
        /// </summary>
		private void OnSelectedBrushChanged()
		{
			if (this.borderSelected != null)
			{
				Brush brush = this.brushSelected != null ? this.brushSelected : this.brushSelectedOriginal;

				this.borderSelected.Background = brush;
			}
		}

        /// <summary>
        /// Called when [highlighted brush changed].
        /// </summary>
		private void OnHighlightedBrushChanged()
		{
			if (this.borderHighlighted != null)
			{
				Brush brush = this.brushHighlighted != null ? this.brushHighlighted : this.brushHighlightedOriginal;

				this.borderHighlighted.Background = brush;
			}
		}

        /// <summary>
        /// Called when [is selected changed].
        /// </summary>
		private void OnIsSelectedChanged()
		{
			this.UpdateVisualState();

			if (this.IsSelectedChanged != null)
			{
				this.IsSelectedChanged(this, EventArgs.Empty);
			}
		}

		#endregion

		#region Events

		internal event EventHandler IsSelectedChanged;

		#endregion

		#region Fields

		private bool isSelected = false;

		#endregion
	}
}
