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
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls 
{
    /// <summary>
    /// Represents the Ribbon Drop Down Class.
    /// </summary>   
	public class DropDown : ContentControl
	{
		#region Constructor

		/// <summary>
		/// Initialize new instance of <see cref="DropDown"/> class
		/// </summary>
		public DropDown()
		{
			this.DefaultStyleKey = typeof(DropDown);

			Application.Current.Host.Content.Resized += new EventHandler(this.OnHostContentResized);
            this.Unloaded += new RoutedEventHandler(DropDown_Unloaded);

		}

        void DropDown_Unloaded(object sender, RoutedEventArgs e)
        {
            Application.Current.Host.Content.Resized -= new EventHandler(this.OnHostContentResized);
            this.Unloaded -= new RoutedEventHandler(DropDown_Unloaded);
        }

        /// <summary>
        /// Initializes the <see cref="DropDown"/> class.
        /// </summary>
        static DropDown()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                LoadDependentAssemblies load = new LoadDependentAssemblies();
                load = null;
            }
        }

		#endregion

		#region Properties

		#region IsOpen

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="DropDown"/> is opened
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
		public bool IsOpen
		{
			get { return (bool)GetValue(IsOpenProperty); }
			set { SetValue(IsOpenProperty, value && (this.popup != null)); }
		}

		/// <summary>
		/// The identifier of <see cref="IsOpen"/> property
		/// </summary>
		public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(DropDown), new PropertyMetadata(false, IsOpenChangedCallback));

        /// <summary>
        /// Determines whether [is open changed callback] [the specified d].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void IsOpenChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((DropDown)d).OnIsOpenChanged((bool)e.NewValue);
		}



        /// <summary>
        /// 
        /// </summary>
        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalOffset.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(DropDown), new PropertyMetadata(0.0));



        /// <summary>
        /// 
        /// </summary>
        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HorizontalOffset.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(DropDown), new PropertyMetadata(0.0));

        

		#endregion

		#endregion

		#region Methods

        /// <summary>
        /// Sets the size.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="h">The h.</param>
		public void SetSize(double w, double h)
		{
			if (this.root != null)
			{
				this.root.Width = w > 0 ? w : double.NaN;
				this.root.Height = h > 0 ? h : double.NaN;
			}
		}

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <returns></returns>
        public  Size GetSize()
        {
            Size size = new Size();

            if (this.root != null)
            {
                size.Width = this.root.Width;
                size.Height = this.root.Height;
            }

            return size;
        }

		#endregion

		#region Overrides

		/// <summary>
		/// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			this.InitPopup();
		}

		/// <summary>
		/// Called before the <see cref="E:System.Windows.UIElement.KeyDown"/> event occurs.
		/// </summary>
		/// <param name="e">The data for the event.</param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (!e.Handled)
			{
				this.ProcessKeyDown(e);

				e.Handled = true;
			}
		}

		/// <summary>
		/// Called when [is open changed].
		/// </summary>
		/// <param name="isOpen">if set to <c>true</c> [is open].</param>
		public virtual void OnIsOpenChanged(bool isOpen)
		{
			if (this.popup != null)
			{
				this.popup.IsOpen = isOpen;

				if (this.IsOpenChanged != null)
				{
					this.IsOpenChanged(this, EventArgs.Empty);
				}

				if (isOpen)
				{
                    Application.Current.RootVisual.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnRootVisualMouseLeftButtonDown), true);
					this.UpdateOutsideRect();

					this.SetFocus();
				}
				else
				{
                    Application.Current.RootVisual.RemoveHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnRootVisualMouseLeftButtonDown));
					this.ResetFocus();
				}
			}
		}

        internal void OnRootVisualMouseLeftButtonDown(object sender,MouseButtonEventArgs e)
        {
            IsOpen = false;
        }

        /// <summary>
        /// Processes the key down.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
		public  virtual void ProcessKeyDown(KeyEventArgs e)
		{
			if (!e.Handled && this.IsOpen)
			{
				switch (e.Key)
				{
					case Key.Escape:
						this.IsOpen = false;
						e.Handled = true;
						break;
				}
			}
		}

        /// <summary>
        /// Gets the size of the preferred.
        /// </summary>
        /// <returns></returns>
		public  virtual Size GetPreferredSize()
		{
			if (this.root != null)
			{
				this.root.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

				return this.root.DesiredSize;
			}

			return new Size(0, 0);
		}
		#endregion

		#region Events

		/// <summary>
		/// Occurs when <see cref="IsOpen"/> property is changed
		/// </summary>
		public event EventHandler IsOpenChanged;

        /// <summary>
        /// Occurs when [outside rect mouse move].
        /// </summary>
		public  event MouseEventHandler OutsideRectMouseMove;

        /// <summary>
        /// Occurs when [outside rect mouse down].
        /// </summary>
		public  event MouseButtonEventHandler OutsideRectMouseDown;

		#endregion

		#region Event handlers

        /// <summary>
        /// Called when [outside rect mouse move].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
		private void OnOutsideRectMouseMove(object sender, MouseEventArgs e)
		{
			if (this.OutsideRectMouseMove != null)
			{
				this.OutsideRectMouseMove(this, e);
			}
		}

        /// <summary>
        /// Called when [outside rect mouse left button down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
		private void OnOutsideRectMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (this.OutsideRectMouseDown != null)
			{
				this.OutsideRectMouseDown(this, e);
			}

			if (!e.Handled)
			{
				this.IsOpen = false;
			}
		}

        /// <summary>
        /// Called when [host content resized].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnHostContentResized(object sender, EventArgs e)
		{
			if (this.IsOpen)
			{
				this.UpdateOutsideRect();
			}
		}

		#endregion

		#region Implementation

        /// <summary>
        /// Inits the popup.
        /// </summary>
		private void InitPopup()
		{
			if (this.outsideRect != null)
			{
				this.outsideRect.MouseMove -= new MouseEventHandler(this.OnOutsideRectMouseMove);
				this.outsideRect.MouseLeftButtonDown -= new MouseButtonEventHandler(this.OnOutsideRectMouseLeftButtonDown);
			}

            if(this.Presenter != null)
                this.Presenter.MouseLeftButtonUp -= new MouseButtonEventHandler(this.Presenter_MouseLeftButtonUp);

			this.root = this.GetTemplateChild("Part_GridPopup") as Grid;
			this.popup = this.GetTemplateChild("Part_Popup") as Popup;
            this.outsideRect = this.GetTemplateChild("Part_OutsideRect") as Rectangle;
            this.Presenter = this.GetTemplateChild("Part_ContentPresenter") as ContentPresenter;
            if (this.Presenter != null)
                this.Presenter.MouseLeftButtonUp += new MouseButtonEventHandler(this.Presenter_MouseLeftButtonUp);
        	if (this.outsideRect != null)
			{
				this.outsideRect.Fill = new SolidColorBrush(Colors.Transparent);

				this.outsideRect.MouseMove += new MouseEventHandler(this.OnOutsideRectMouseMove);
				this.outsideRect.MouseLeftButtonDown += new MouseButtonEventHandler(this.OnOutsideRectMouseLeftButtonDown);
                
			}
		}

       void Presenter_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                this.OnOutsideRectMouseLeftButtonDown(sender,e);
            }
       }

      

       
        /// <summary>
        /// Updates the outside rect.
        /// </summary>
		private void UpdateOutsideRect()
		{
			try
			{
				GeneralTransform gt = this.TransformToVisual(null);

				if (gt != null)
				{
					Point pt = gt.Transform(new Point(0, 0));

					MatrixTransform transform = new MatrixTransform();
					transform.Matrix = new Matrix(1, 0, 0, 1, -pt.X, -pt.Y);

					if (this.outsideRect != null)
					{
						this.outsideRect.RenderTransform = transform;

						System.Windows.Interop.Content content = Application.Current.Host.Content;

						this.outsideRect.Width = content.ActualWidth;
						this.outsideRect.Height = content.ActualHeight;
					}
				}
			}
			catch
			{
			}
		}

        /// <summary>
        /// Sets the focus.
        /// </summary>
		private void SetFocus()
		{
			this.focusedElement = FocusManager.GetFocusedElement() as Control;

			this.Focus();
		}

        /// <summary>
        /// Resets the focus.
        /// </summary>
		private void ResetFocus()
		{
			if (this.focusedElement != null)
			{
				this.focusedElement.Focus();
				this.focusedElement = null;
			}
		}

		#endregion

		#region Fields

		internal Grid root = null;
		internal Rectangle outsideRect;
		internal Popup popup;
        internal ContentPresenter Presenter;
		private Control focusedElement = null;

		#endregion
	}
}
