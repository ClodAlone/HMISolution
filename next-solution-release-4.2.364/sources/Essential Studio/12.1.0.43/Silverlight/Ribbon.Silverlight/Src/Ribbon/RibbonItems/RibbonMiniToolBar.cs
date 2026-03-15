#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Tools.Controls 
{
	using System;
	using System.Collections;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Controls.Primitives;
	using System.Windows.Input;
	using System.Windows.Interop;
	using System.Windows.Media;
    using Syncfusion.Windows.Shared;
    using System.ComponentModel;

	/// <summary>
	/// Represents a ribbon's mini toolbar control.
	/// </summary>
	[TemplateVisualState(GroupName = "Common states", Name = "Normal")]
	[TemplateVisualState(GroupName = "Common states", Name = "Visible")]
	public class RibbonMiniToolBar : ContentControl
	{
		#region Fields

		private bool templateLoaded = false;
        private double popUpHeight = 0.0;
        private double popUpWidth = 0.0;
		internal Popup popup;
        internal Canvas outerCanvas;
        internal Panel contentPanel;
        internal FrameworkElement border;
		private double distance;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RibbonMiniToolBar"/> class.
		/// </summary>
		public RibbonMiniToolBar()
		{
			this.DefaultStyleKey = typeof(RibbonMiniToolBar);
            this.Loaded += new RoutedEventHandler(RibbonMiniToolBar_Loaded);      
		}

        /// <summary>
        /// Initializes the <see cref="RibbonMiniToolBar"/> class.
        /// </summary>
        static RibbonMiniToolBar()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                LoadDependentAssemblies load = new LoadDependentAssemblies();
                load = null;
            }
        }
		#endregion

		#region Dependency properties

		#region IsOpened

		/// <summary>
		/// Gets or sets a value indicating whether miniu toolbar's ppopu is opened.
		/// </summary>
		/// <value><c>true</c> if this instance is opened; otherwise, <c>false</c>.</value>
		public bool IsOpen
		{
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
		}

		/// <summary>
		/// Event that is raised when <see cref="RibbonMiniToolBar.IsOpen"/> property is changed.
		/// </summary>
		public event PropertyChangedCallback IsOpenedChanged;

		/// <summary>
		/// The identifier for the <see cref="RibbonMiniToolBar.IsOpen"/> dependency property. 
		/// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(RibbonMiniToolBar), new PropertyMetadata(false, new PropertyChangedCallback(OnIsOpenedChanged)));

		/// <summary>
		/// Calls OnIsOpenedChanged method of the instance, notifies of the depencency property value changes.
		/// </summary>
		/// <param name="d">Dependency object, the change occures on.</param>
		/// <param name="e">Property change details, such as old value and new value.</param>
		private static void OnIsOpenedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			RibbonMiniToolBar instance = (RibbonMiniToolBar)d;
			instance.OnIsOpenedChanged(e);
		}

		/// <summary>
		/// Raises IsOpenedChanged event.
		/// </summary>
		/// <param name="e">Property change details, such as old value and new value.</param>
		private void OnIsOpenedChanged(DependencyPropertyChangedEventArgs e)
		{
			if (this.IsOpen && this.templateLoaded)
			{
				this.distance = -1;
				this.contentPanel.Opacity = 1.0;
				this.popup.IsOpen = this.IsOpen;              
				this.PlacePopup();
			}

			this.UpdateVisualState();

			if (this.IsOpenedChanged != null)
			{
				this.IsOpenedChanged(this, e);
			}
		}

		#endregion

		#region HorizontalOffset

		/// <summary>
		/// Gets or sets the horizontal distance between the target origin and the mini toolbars's popup alignment point.
		/// </summary>
		/// <value>The horizontal offset.</value>
		public double HorizontalOffset
		{
			get { return (double)GetValue(HorizontalOffsetProperty); }
			set { SetValue(HorizontalOffsetProperty, value); }
		}

		/// <summary>
		/// Event that is raised when <see cref="RibbonMiniToolBar.HorizontalOffset"/> property is changed.
		/// </summary>
		public event PropertyChangedCallback HorizontalOffsetChanged;

		/// <summary>
		/// The identifier for the <see cref="RibbonMiniToolBar.HorizontalOffset"/> dependency property. 
		/// </summary>
		public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.Register(
			"HorizontalOffset", typeof(double), typeof(RibbonMiniToolBar), new PropertyMetadata(0.0, new PropertyChangedCallback(OnHorizontalOffsetChanged)));

		/// <summary>
		/// Calls OnHorizontalOffsetChanged method of the instance, notifies of the depencency property value changes.
		/// </summary>
		/// <param name="d">Dependency object, the change occures on.</param>
		/// <param name="e">Property change details, such as old value and new value.</param>
		private static void OnHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			RibbonMiniToolBar instance = (RibbonMiniToolBar)d;
			instance.OnHorizontalOffsetChanged(e);
		}

		/// <summary>
		/// Raises HorizontalOffsetChanged event.
		/// </summary>
		/// <param name="e">Property change details, such as old value and new value.</param>
		private void OnHorizontalOffsetChanged(DependencyPropertyChangedEventArgs e)
		{
			this.PlacePopup();

			if (this.HorizontalOffsetChanged != null)
			{
				this.HorizontalOffsetChanged(this, e);
			}
		}

		#endregion

		#region VerticalOffset

		/// <summary>
		/// Gets or sets the vertical distance between the target origin and the mini toolbar's popup alignment point.
		/// </summary>
		public double VerticalOffset
		{
			get { return (double)GetValue(VerticalOffsetProperty); }
			set { SetValue(VerticalOffsetProperty, value); }
		}

		/// <summary>
		/// Event that is raised when <see cref="RibbonMiniToolBar.VerticalOffset"/> property is changed.
		/// </summary>
		public event PropertyChangedCallback VerticalOffsetChanged;

		/// <summary>
		/// The identifier for the <see cref="RibbonMiniToolBar.VerticalOffset"/> dependency property. 
		/// </summary>
		public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.Register(
			"VerticalOffset", typeof(double), typeof(RibbonMiniToolBar), new PropertyMetadata(0.0, new PropertyChangedCallback(OnVerticalOffsetChanged)));

		/// <summary>
		/// Calls OnVerticalOffsetChanged method of the instance, notifies of the depencency property value changes.
		/// </summary>
		/// <param name="d">Dependency object, the change occures on.</param>
		/// <param name="e">Property change details, such as old value and new value.</param>
		private static void OnVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			RibbonMiniToolBar instance = (RibbonMiniToolBar)d;
			instance.OnVerticalOffsetChanged(e);
		}

		/// <summary>
		/// Raises VerticalOffsetChanged event.
		/// </summary>
		/// <param name="e">Property change details, such as old value and new value.</param>
		private void OnVerticalOffsetChanged(DependencyPropertyChangedEventArgs e)
		{
			this.PlacePopup();

			if (this.VerticalOffsetChanged != null)
			{
				this.VerticalOffsetChanged(this, e);
			}
		}

		#endregion

		#region Target

		/// <summary>
		/// Gets or sets the target element.
		/// </summary>
		public UIElement Target
		{
			get { return (UIElement)GetValue(TargetProperty); }
			set { SetValue(TargetProperty, value); }
		}

		/// <summary>
		/// Event that is raised when <see cref="RibbonMiniToolBar.Target"/> property is changed.
		/// </summary>
		public event PropertyChangedCallback TargetChanged;

		/// <summary>
		/// The identifier for the <see cref="RibbonMiniToolBar.Target"/> dependency property. 
		/// </summary>
		public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
			"Target", typeof(UIElement), typeof(RibbonMiniToolBar), new PropertyMetadata(null, new PropertyChangedCallback(OnTargetChanged)));

		/// <summary>
		/// Calls OnTargetChanged method of the instance, notifies of the depencency property value changes.
		/// </summary>
		/// <param name="d">Dependency object, the change occures on.</param>
		/// <param name="e">Property change details, such as old value and new value.</param>
		private static void OnTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			RibbonMiniToolBar instance = (RibbonMiniToolBar)d;
			instance.OnTargetChanged(e);
		}

		/// <summary>
		/// Raises TargetChanged event.
		/// </summary>
		/// <param name="e">Property change details, such as old value and new value.</param>
		private void OnTargetChanged(DependencyPropertyChangedEventArgs e)
		{
			if (this.TargetChanged != null)
			{
				this.TargetChanged(this, e);
			}
		}

		#endregion

		#region Range

		/// <summary>
		/// Gets or sets the range, in which mini toolbar does not fades.
		/// </summary>
		/// <value>Default value is 0.0</value>
		public double Range
		{
			get { return (double)GetValue(RangeProperty); }
			set { SetValue(RangeProperty, value); }
		}

		/// <summary>
		/// Event that is raised when <see cref="RibbonMiniToolBar.Range"/> property is changed.
		/// </summary>
		public event PropertyChangedCallback RangeChanged;

		/// <summary>
		/// The identifier for the <see cref="RibbonMiniToolBar.Range"/> dependency property. 
		/// </summary>
		public static readonly DependencyProperty RangeProperty = DependencyProperty.Register(
			"Range", 
			typeof(double), 
			typeof(RibbonMiniToolBar), 
			new PropertyMetadata(40.0, new PropertyChangedCallback(OnRangeChanged)));

		/// <summary>
		/// Calls OnRangeChanged method of the instance, notifies of the depencency property value changes.
		/// </summary>
		/// <param name="d">Dependency object, the change occures on.</param>
		/// <param name="e">Property change details, such as old value and new value.</param>
		private static void OnRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			RibbonMiniToolBar instance = (RibbonMiniToolBar)d;
			instance.OnRangeChanged(e);
		}

		/// <summary>
		/// Raises RangeChanged event.
		/// </summary>
		/// <param name="e">Property change details, such as old value and new value.</param>
		private void OnRangeChanged(DependencyPropertyChangedEventArgs e)
		{
			if (this.RangeChanged != null)
			{
				this.RangeChanged(this, e);
			}
		}

		#endregion

		#endregion

		#region Overrides

		/// <summary>
		/// Builds the visual tree for the <see cref="T:System.Windows.Controls.ToolTip"/> when a new template is applied.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			this.templateLoaded = true;

			this.SetupPopup();

			IList groups = VisualStateManager.GetVisualStateGroups(this.popup);

			if (groups != null)
			{
				VisualStateGroup commonStates = (VisualStateGroup)groups[0];

				commonStates.CurrentStateChanged += new EventHandler<VisualStateChangedEventArgs>(this.OnCurrentStateChanged);
			}
		}

		#endregion

		#region Implementation

        /// <summary>
        /// Handles the Loaded event of the RibbonMiniToolBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void RibbonMiniToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            if (Application.Current != null)
            {
                Application.Current.Exit += new EventHandler(Current_Exit);
            }
        }

        /// <summary>
        /// Handles the Exit event of the Current control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
       private void Current_Exit(object sender, EventArgs e)
        {
            if (this.popup != null)
            {
                if (this.popup.IsOpen == true)
                {
                    this.popup.IsOpen = false;
                }
            }
        }

       /// <summary>
       /// Called when [current state changed].
       /// </summary>
       /// <param name="sender">The sender.</param>
       /// <param name="e">The <see cref="System.Windows.VisualStateChangedEventArgs"/> instance containing the event data.</param>
		private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
		{
			this.popup.IsOpen = this.IsOpen;
		}

        /// <summary>
        /// Setups the popup.
        /// </summary>
		private void SetupPopup()
		{
			this.popup = (Popup)GetTemplateChild("Popup");
			this.contentPanel = (Panel)GetTemplateChild("ContentPanel");
			this.outerCanvas = (Canvas)GetTemplateChild("OuterCanvas");
			this.border = (FrameworkElement)GetTemplateChild("Border");

			if (this.popup != null)
			{
				this.popup.HorizontalOffset = this.popup.VerticalOffset = 0.0;

				if (this.contentPanel != null)
				{
					this.outerCanvas.MouseLeftButtonDown += new MouseButtonEventHandler(this.OnOuterCanvasMouseLeftButtonDown);
					this.outerCanvas.MouseMove += new MouseEventHandler(this.OnOuterCanvasMouseMove);

					Content content = Application.Current.Host.Content;
                    if (content != null)
                    {
                        popUpHeight = content.ActualHeight; 
                        popUpWidth = content.ActualWidth;
                    }
					content.Resized += new EventHandler(this.OnContentResized);
				}
			}
		}

        /// <summary>
        /// Updates the state of the visual.
        /// </summary>
		private void UpdateVisualState()
		{
			string stateName = this.IsOpen ? "Visible" : "Normal";

			VisualStateManager.GoToState(this, stateName, true);
		}

        /// <summary>
        /// Called when [content resized].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnContentResized(object sender, EventArgs e)
		{
			this.ArrangeOuterCanvas();
		}

        /// <summary>
        /// Called when [outer canvas mouse left button down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
		private void OnOuterCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.IsOpen = false;
		}

        /// <summary>
        /// Called when [outer canvas mouse move].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
		private void OnOuterCanvasMouseMove(object sender, MouseEventArgs e)
		{
			Point pt = e.GetPosition(this.popup);
			this.UpdateOpacity(pt);
		}

        /// <summary>
        /// Updates the opacity.
        /// </summary>
        /// <param name="pt">The pt.</param>
		private void UpdateOpacity(Point pt)
		{
			Rect rc = new Rect(new Point(0.0, 0.0), this.border.RenderSize);

			double dx = 0;

			if (pt.X < rc.X)
			{
				dx = rc.X - pt.X;
			}
			else if (pt.X > rc.Right)
			{
				dx = pt.X - rc.Right;
			}

			double dy = 0;

			if (pt.Y < rc.Top)
			{
				dy = rc.Y - pt.Y;
			}
			else if (pt.Y > rc.Bottom)
			{
				dy = pt.Y - rc.Bottom;
			}

			double current_distance = Math.Max(dx, dy);

			if (this.distance >= 0 && current_distance > this.distance)
			{
				double range = current_distance - this.distance;

				if (range < (this.Range * 2))
				{
					this.contentPanel.Opacity = 1.0f - (range / this.Range);
				}
				else
				{
					this.IsOpen = false;
				}
			}
			else
			{
				this.distance = current_distance;
			}
		}

        /// <summary>
        /// Places the popup.
        /// </summary>
		private void PlacePopup()
		{
			if (this.IsOpen)
			{
				UIElement owner = this.Target;

				if (owner != null)
				{
					MatrixTransform mt = null;

					try
					{
						mt = owner.TransformToVisual(this) as MatrixTransform;
					}
					catch
					{
					}

					if (mt != null)
					{
						Matrix m = mt.Matrix;
						mt.Matrix = m;
                        if(popup != null)
						this.popup.RenderTransform = mt;
					}
				}

                if (popup != null)
                {
                    GeneralTransform gt = this.popup.TransformToVisual(null);
                    Point ptPopupLT = gt.Transform(new Point(this.HorizontalOffset, this.VerticalOffset));
                    if (this.outerCanvas.ActualHeight == 0.0 && this.outerCanvas.ActualWidth == 0.0)
                    {
                        this.outerCanvas.Height = popUpHeight;
                        this.outerCanvas.Width = popUpWidth;
                    }
                    Rect rcOuter = new Rect(0.0, 0.0, this.outerCanvas.ActualWidth, this.outerCanvas.ActualHeight);

                    this.border.Measure(new Size(rcOuter.Width, rcOuter.Height));

                    ptPopupLT.Y -= this.border.DesiredSize.Height;

                    Rect rcPopup = new Rect(ptPopupLT, this.border.DesiredSize);

                    Point ptPrev = gt.Transform(new Point());

                    PlacePopupLocation(ref rcPopup, ref rcOuter);

                    MatrixTransform mtPopup = this.popup.RenderTransform as MatrixTransform;

                    if (mtPopup == null)
                    {
                        mtPopup = new MatrixTransform();
                    }

                    Matrix m2 = mtPopup.Matrix;

                    m2.OffsetX += rcPopup.X - ptPrev.X;
                    m2.OffsetY += rcPopup.Y - ptPrev.Y;

                    mtPopup.Matrix = m2;
                    this.popup.RenderTransform = mtPopup;

                    this.ArrangeOuterCanvas();
                }
			}
		}

        /// <summary>
        /// Places the popup location.
        /// </summary>
        /// <param name="rcPopup">The rc popup.</param>
        /// <param name="rcOuter">The rc outer.</param>
		private static void PlacePopupLocation(ref Rect rcPopup, ref Rect rcOuter)
		{
			if (rcPopup.Right > rcOuter.Width)
			{
				rcPopup.X = rcOuter.Width - rcPopup.Width;
			}

			if (rcPopup.X < 0.0)
			{
				rcPopup.X = 0.0;
			}

			if (rcPopup.Bottom > rcOuter.Height)
			{
				rcPopup.Y = rcOuter.Height - rcPopup.Height;
			}

			if (rcPopup.Y < 0.0)
			{
				rcPopup.Y = 0.0;
			}
		}

        /// <summary>
        /// Arranges the outer canvas.
        /// </summary>
		private void ArrangeOuterCanvas()
		{
			Application app = Application.Current;
			Content content = app.Host.Content;

			this.outerCanvas.Height = content.ActualHeight;
			this.outerCanvas.Width = content.ActualWidth;

			UIElement owner = this.Target;

			if (owner != null)
			{
				MatrixTransform mt = this.popup.RenderTransform as MatrixTransform;

				if (mt != null)
				{
					Matrix m = mt.Matrix;
					this.InvertMatrix(ref m);

					mt = new MatrixTransform();
					mt.Matrix = m;

					MatrixTransform mt2 = null;

					try
					{
						mt2 = TransformToVisual(null) as MatrixTransform;

						Matrix m2 = mt2.Matrix;
						this.InvertMatrix(ref m2);

						mt2.Matrix = m2;
					}
					catch
					{
					}

					if (mt2 != null)
					{
						TransformGroup tg = new TransformGroup();

						tg.Children.Add(mt);
						tg.Children.Add(mt2);

						this.outerCanvas.RenderTransform = tg;
					}
				}
			}
		}

        /// <summary>
        /// Inverts the matrix.
        /// </summary>
        /// <param name="m">The m.</param>
		private void InvertMatrix(ref Matrix m)
		{
			double num = (m.M11 * m.M22) - (m.M12 * m.M21);

			if (num != 0.0)
			{
				Matrix m2 = m;
				m.M11 = m2.M22 / num;
				m.M12 = (-1.0 * m2.M12) / num;
				m.M21 = (-1.0 * m2.M21) / num;
				m.M22 = m2.M11 / num;
				m.OffsetX = ((m2.OffsetY * m2.M21) - (m2.OffsetX * m2.M22)) / num;
				m.OffsetY = ((m2.OffsetX * m2.M12) - (m2.OffsetY * m2.M11)) / num;
			}
		}

		#endregion
	}
}
