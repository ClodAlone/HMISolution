#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Syncfusion.Windows.Tools.Controls 
{
	/// <summary>
	/// Defines <see cref="RibbonItemBase"/>'s apperance.
	/// </summary>
	public enum Appearance
	{
		/// <summary>
		/// Small mode (only image).
		/// </summary>
		ExtraSmall,
		
		/// <summary>
		/// Small mode (image before label).
		/// </summary>
		Small,
		
		/// <summary>
		/// Large mode (large image above label).
		/// </summary>
		Large
	}

	/// <summary>
	/// Represent an anstract base class for ribbon's items.
	/// </summary>
	[TemplateVisualState(GroupName = "RibbonItemStates", Name = "Normal")]
	[TemplateVisualState(GroupName = "RibbonItemStates", Name = "Selected")]
	[TemplateVisualState(GroupName = "RibbonItemStates", Name = "Pressed")]
	[TemplateVisualState(GroupName = "RibbonItemStates", Name = "Disabled")]
	[TemplateVisualState(GroupName = "AppearanceStates", Name = "SmallIitem")]
	[TemplateVisualState(GroupName = "AppearanceStates", Name = "LargeIitem")]
	[TemplateVisualState(GroupName = "AppearanceStates", Name = "ExtraSmallIitem")]
	public abstract class RibbonItemBase : ButtonBase
	{
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="RibbonItemBase"/> class.
		/// </summary>
		public RibbonItemBase()
		{
			this.DefaultStyleKey = typeof(RibbonItemBase);
			this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(this.OnIsEnabledChanged);
			this.IsTabStop = false;
		}

		#endregion

		#region Properties

		#region Label

		/// <summary>
		/// Gets or sets content of the item's text part
		/// </summary>
		public object Label
		{
			get { return (object)GetValue(LabelProperty); }
			set { SetValue(LabelProperty, value); }
		}

		/// <summary>
		/// Using a DependencyProperty as the backing store for Caption.  This enables animation, styling, binding, etc...
		/// </summary>
		public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(object), typeof(RibbonItemBase), new PropertyMetadata(new PropertyChangedCallback(LabelChangedCallback)));

        /// <summary>
        /// Labels the changed callback.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void LabelChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((RibbonItemBase)d).OnLabelChanged();
		}

		#endregion

		#region Image

		/// <summary>
		/// Gets or sets the image.
		/// </summary>
		/// <value>The image.</value>
		public ImageSource Image
		{
			get { return (ImageSource)GetValue(ImageProperty); }
			set { SetValue(ImageProperty, value); }
		}

		/// <summary>
		/// Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
		/// </summary>
		public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(
			"Image", typeof(ImageSource), typeof(RibbonItemBase), new PropertyMetadata(null, new PropertyChangedCallback(ImageChangedCallback)));

        /// <summary>
        /// Images the changed callback.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void ImageChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((RibbonItemBase)d).OnImageChanged();
		}

		/// <summary>
		/// Gets or sets the large image.
		/// </summary>
		/// <value>The large image.</value>
		public ImageSource LargeImage
		{
			get { return (ImageSource)GetValue(LargeImageProperty); }
			set { SetValue(LargeImageProperty, value); }
		}

		/// <summary>
		/// Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
		/// </summary>
		public static readonly DependencyProperty LargeImageProperty = DependencyProperty.Register(
			"LargeImage", typeof(ImageSource), typeof(RibbonItemBase), new PropertyMetadata(null, new PropertyChangedCallback(LargeImageChangedCallback)));

        /// <summary>
        /// Larges the image changed callback.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void LargeImageChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((RibbonItemBase)d).OnImageChanged();
		}

		#endregion

		#region IsSelected

		/// <summary>
		/// Gets or sets a value indicating whether this item is selected.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this item is selected; otherwise, <c>false</c>.
		/// </value>
		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}

		/// <summary>
		/// Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
		/// </summary>
		public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(RibbonItemBase), new PropertyMetadata(false, new PropertyChangedCallback(IsSelectedChangedCallback)));

        /// <summary>
        /// Determines whether [is selected changed callback] [the specified d].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void IsSelectedChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((RibbonItemBase)d).OnIsSelectedChanged((bool)e.OldValue, (bool)e.NewValue);
		}

		#endregion

		#region Appearance

		/// <summary>
		/// Gets the appearance.
		/// </summary>
		/// <param name="obj">The dependency object.</param>
		/// <returns>Value from <see cref="Appearance"/> enumaration.</returns>
		public static Appearance GetAppearance(DependencyObject obj)
		{
			return (Appearance)obj.GetValue(AppearanceProperty);
		}

		/// <summary>
		/// Sets the appearance.
		/// </summary>
		/// <param name="obj">The dependency object.</param>
		/// <param name="value">Value from <see cref="Appearance"/> enumaration.</param>
		public static void SetAppearance(DependencyObject obj, Appearance value)
		{
			obj.SetValue(AppearanceProperty, value);
		}

		/// <summary>
		/// Identifier for <see cref="Appearance"/> property.
		/// </summary>
		public static readonly DependencyProperty AppearanceProperty =
            DependencyProperty.RegisterAttached("Appearance", typeof(Appearance), typeof(RibbonItemBase), new PropertyMetadata(Appearance.Small, AppearanceChangedCallback));

        /// <summary>
        /// Appearances the changed callback.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void AppearanceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is RibbonItemBase)
			{
				((RibbonItemBase)d).OnAppearanceChanged((Appearance)e.NewValue);
			}
		}

		#endregion

		#region Selector

        /// <summary>
        /// Gets or sets the selector.
        /// </summary>
        /// <value>The selector.</value>
		internal IRibbonSelector Selector
		{
			get { return this.selector; }
			set { this.selector = value; }
		}

		#endregion

		#region VisualState

        /// <summary>
        /// Gets the state of the visual.
        /// </summary>
        /// <value>The state of the visual.</value>
		internal virtual string VisualState
		{
			get
			{
				if (this.IsEnabled)
				{
					if (this.IsSelected)
					{
						if (this.mouseDown)
						{
							return "Pressed";
						}

						return "Selected";
					}

					return "Normal";
				}

				return "Disabled";
			}
		}

		#endregion

		#region ImageState

        /// <summary>
        /// Gets the state of the appearance.
        /// </summary>
        /// <value>The state of the appearance.</value>
		internal virtual string AppearanceState
		{
			get
			{
				switch ((Appearance)this.GetValue(AppearanceProperty))
				{
					case Appearance.Large: return "LargeIitem";
					case Appearance.ExtraSmall: return "ExtraSmallIitem";
				}

				return "SmallIitem";
			}
		}

		#endregion

		#region MouseDown

        /// <summary>
        /// Gets or sets a value indicating whether [mouse down].
        /// </summary>
        /// <value><c>true</c> if [mouse down]; otherwise, <c>false</c>.</value>
		internal bool MouseDown
		{
			get
			{
				return this.mouseDown;
			}

			set
			{
				if (this.mouseDown != value)
				{
					this.mouseDown = value;

					this.UpdateVisualState();
				}
			}
		}

		#endregion

		#endregion

		#region Overrides

		/// <summary>
		/// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			this.partLabel = this.GetTemplateChild("Part_Text") as UIElement;
			this.partImage = this.GetTemplateChild("Part_Image") as Image;

			this.OnLabelChanged();
			this.OnImageChanged();

			this.UpdateVisualState();
			this.UpdateAppearanceState();
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Controls.Primitives.ButtonBase.Click"/> event.
		/// </summary>
		protected override void OnClick()
		{
			base.OnClick();

			if (this.Selector != null)
			{
				this.Selector.OnItemClicked(this);
			}
		}

		/// <summary>
		/// Provides class handling for the <see cref="E:System.Windows.UIElement.KeyDown"/> event that occurs when the user presses a key while this control has focus.
		/// </summary>
		/// <param name="e">The event data.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// 	<paramref name="e"/> is null.
		/// </exception>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
		}

		/// <summary>
		/// Provides class handling for the <see cref="E:System.Windows.UIElement.MouseMove"/> event that occurs when the mouse pointer moves while over this element.
		/// </summary>
		/// <param name="e">The event data.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// 	<paramref name="e"/> is null.
		/// </exception>
		protected override void OnMouseMove(MouseEventArgs e)
		{
			this.IsSelected = this.ContainsMouse(e, this);

			if (!this.IsPressed)
			{
				base.OnMouseMove(e);
			}
		}

		/// <summary>
		/// Provides class handling for the <see cref="E:System.Windows.UIElement.MouseLeave"/> routed event that occurs when the mouse leaves an element.
		/// </summary>
		/// <param name="e">The event data for the <see cref="E:System.Windows.UIElement.MouseLeave"/> event.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// 	<paramref name="e"/> is null.
		/// </exception>
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);

			this.IsSelected = false;
		}

		/// <summary>
		/// Provides class handling for the <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> event that occurs when the left mouse button is pressed while the mouse pointer is over this control.
		/// </summary>
		/// <param name="e">The event data.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// 	<paramref name="e"/> is null.
		/// </exception>
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			if (this.ProcessMouseDown(e))
			{
				e.Handled = true;
			}
			else
			{
				this.MouseDown = this.ClickMode == ClickMode.Release;
			}

			base.OnMouseLeftButtonDown(e);
		}

		/// <summary>
		/// Provides class handling for the <see cref="E:System.Windows.UIElement.MouseLeftButtonUp"/> event that occurs when the left mouse button is released while the mouse pointer is over this control.
		/// </summary>
		/// <param name="e">The event data.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// 	<paramref name="e"/> is null.
		/// </exception>
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			this.MouseDown = false;

			base.OnMouseLeftButtonUp(e);
		}

        /// <summary>
        /// Processes the mouse down.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
		internal virtual bool ProcessMouseDown(MouseButtonEventArgs e)
		{
			return false;
		}

		internal virtual void OnIsSelectedChanged(bool oldValue, bool newValue)
		{
			if (this.selector != null && newValue)
			{
				this.selector.OnItemSelected(this);
			}

			this.UpdateVisualState();
		}

		/// <summary>
		/// Called when text is changed.
		/// </summary>
		internal virtual void OnLabelChanged()
		{
			if (this.partLabel != null)
			{
				this.partLabel.Visibility = (this.Label == null) ? Visibility.Collapsed : Visibility.Visible;
			}
		}

		/// <summary>
		/// Called when image is changed.
		/// </summary>
		internal virtual void OnImageChanged()
		{
			if (this.partImage != null)
			{
				ImageSource image = ((Appearance)this.GetValue(AppearanceProperty)) == Appearance.Large ? this.LargeImage : this.Image;

				this.partImage.Source = image;

				this.partImage.Visibility = (image == null) ? Visibility.Collapsed : Visibility.Visible;
			}
		}

        /// <summary>
        /// Called when [appearance changed].
        /// </summary>
        /// <param name="newValue">The new value.</param>
		internal virtual void OnAppearanceChanged(Appearance newValue)
		{
			if (this.partImage != null)
			{
				this.OnImageChanged();
				this.UpdateAppearanceState();
			}
		}

        /// <summary>
        /// Determines whether the specified e contains mouse.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        /// <param name="element">The element.</param>
        /// <returns>
        /// 	<c>true</c> if the specified e contains mouse; otherwise, <c>false</c>.
        /// </returns>
		internal bool ContainsMouse(MouseEventArgs e, UIElement element)
		{
			bool bRes = false;

			if (element != null)
			{
				int w = (int)element.RenderSize.Width;

				if (w > 0)
				{
					int h = (int)element.RenderSize.Height;

					if (h > 0)
					{
						Rect bounds = new Rect(0, 0, w, h);

						bRes = bounds.Contains(GetMousePos(e, element));
					}
				}
			}

			return bRes;
		}

		#endregion

		#region Event Handlers

        /// <summary>
        /// Called when [is enabled changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this.UpdateVisualState();
		}

		#endregion

		#region Implementation

        /// <summary>
        /// Raises the key down.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
		internal void RaiseKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
		}

        /// <summary>
        /// Updates the state of the visual.
        /// </summary>
		internal void UpdateVisualState()
		{
			VisualStateManager.GoToState(this, this.VisualState, true);
		}

        /// <summary>
        /// Updates the state of the appearance.
        /// </summary>
		internal void UpdateAppearanceState()
		{
			VisualStateManager.GoToState(this, this.AppearanceState, true);
		}

        /// <summary>
        /// Gets the mouse pos.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        /// <param name="element">The element.</param>
        /// <returns></returns>
		internal static Point GetMousePos(MouseEventArgs e, UIElement element)
		{
			Point pt = e.GetPosition(null);

			MatrixTransform mt = element.TransformToVisual(null) as MatrixTransform;

			if (mt != null)
			{
				Matrix m = mt.Matrix;
				pt.X -= m.OffsetX;
				pt.Y -= m.OffsetY;
			}

			return pt;
		}

		#endregion

		#region Fields

		private IRibbonSelector selector = null;
		private bool mouseDown = false;
		internal Image partImage = null;
		internal UIElement partLabel = null;

		#endregion
	}
}
