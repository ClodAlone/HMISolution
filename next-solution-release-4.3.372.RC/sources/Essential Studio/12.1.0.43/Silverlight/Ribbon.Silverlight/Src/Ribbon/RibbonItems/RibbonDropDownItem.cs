#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Syncfusion.Windows.Tools.Controls 
{
	/// <summary>
	/// Drop down opening direction.
	/// </summary>
	public enum DropDownDirection
	{
		/// <summary>
		/// Drop down is opened below and to left side of the owner.
		/// </summary>
		BelowLeft,

		/// <summary>
		/// Drop down is opened below and to right side of the owner.
		/// </summary>
		BelowRight,

		/// <summary>
		/// Drop down is opened above and to left side of the owner.
		/// </summary>
		AboveLeft,

		/// <summary>
		/// Drop down is opened above and to right side of the owner.
		/// </summary>
		AboveRight,

		/// <summary>
		/// Drop down is opened to right side of the owner.
		/// </summary>
		Right
	}

    /// <summary>
    /// Represents the Bounds Event Args Class.
    /// </summary>
	internal class BoundsEventArgs : EventArgs
	{
		#region Fields

		private Rect rect = Rect.Empty;

		#endregion

		#region Properties

        /// <summary>
        /// Gets or sets the rect.
        /// </summary>
        /// <value>The rect.</value>
		public Rect Rect
		{
			get
			{
				return this.rect;
			}

			set
			{
				this.rect = value;
			}
		}

		#endregion

		#region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundsEventArgs"/> class.
        /// </summary>
        /// <param name="rect">The rect.</param>
		public BoundsEventArgs(Rect rect)
		{
			this.rect = rect;
		}

		#endregion
	}

	/// <summary>
	/// Represents ribbon's drop-down item.
	/// </summary>
	public class RibbonDropDownItem :
		RibbonItemBase
	{
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="RibbonDropDownItem"/> class.
		/// </summary>
		public RibbonDropDownItem()
		{
			this.DefaultStyleKey = typeof(RibbonDropDownItem);
		}

		#endregion

		#region Properties

		#region DropDown

		/// <summary>
		/// Gets or sets the drop down.
		/// </summary>
		/// <value>The drop down.</value>
		public RibbonDropDown DropDown
		{
			get { return (RibbonDropDown)GetValue(DropDownProperty); }
			set { SetValue(DropDownProperty, value); }
		}

		/// <summary>
		/// Identifier for <see cref="DropDown"/> property.
		/// </summary>
		public static readonly DependencyProperty DropDownProperty = DependencyProperty.Register("DropDown", typeof(RibbonDropDown), typeof(RibbonDropDownItem), new PropertyMetadata(DropDownChangedCallback));

        /// <summary>
        /// Drops down changed callback.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="ea">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void DropDownChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs ea)
		{
			((RibbonDropDownItem)d).OnDropDownChanged(ea.OldValue as RibbonDropDown, ea.NewValue as RibbonDropDown);
		}

		#endregion

		#region IsDropDownOpen

		/// <summary>
		/// Gets or sets a value indicating whether drop down is open.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if drop down is open; otherwise, <c>false</c>.
		/// </value>
		public bool IsDropDownOpen
		{
			get
			{
				return (bool)GetValue(IsDropDownOpenProperty);
			}

			set
			{
				this.SetValue(IsDropDownOpenProperty, value && (this.DropDown != null));
			}
		}

		/// <summary>
		/// Identifier for <see cref="IsDropDownOpen"/> property.
		/// </summary>
		public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(RibbonDropDownItem), new PropertyMetadata(false, new PropertyChangedCallback(IsDropDownOpenChangedCallback)));

        /// <summary>
        /// Determines whether [is drop down open changed callback] [the specified d].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void IsDropDownOpenChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((RibbonDropDownItem)d).OnIsDropDownOpenChanged((bool)e.OldValue, (bool)e.NewValue);
		}

		#endregion

		#region DropDownDirection

		/// <summary>
		/// Gets or sets the drop down direction.
		/// </summary>
		/// <value>The drop down direction.</value>
		public DropDownDirection DropDownDirection
		{
			get { return (DropDownDirection)GetValue(DropDownDirectionProperty); }
			set { SetValue(DropDownDirectionProperty, value); }
		}

		/// <summary>
		/// Using a DependencyProperty as the backing store for Direction.  This enables animation, styling, binding, etc...
		/// </summary>
		public static readonly DependencyProperty DropDownDirectionProperty = DependencyProperty.Register(
            "DropDownDirection", 
			typeof(DropDownDirection), 
			typeof(RibbonDropDownItem), 
			new PropertyMetadata(DropDownDirection.BelowRight));

		#endregion

		#region DropDownLocation

		/// <summary>
		/// Gets preferred drop-down location, relative to the root element of RibbonDropDownItem.
		/// </summary>
		/// <value>The drop down location.</value>
		internal virtual Point DropDownLocation
		{
			get
			{
				Size szItem = this.root != null ? this.root.RenderSize : this.RenderSize;
				Size szPopup = this.DropDown != null ? this.DropDown.GetPreferredSize() : new Size(0, 0);

				switch (this.DropDownDirection)
				{
				case DropDownDirection.BelowLeft:
					return new Point(szItem.Width - szPopup.Width, szItem.Height);
				case DropDownDirection.AboveRight:
					return new Point(0, -szPopup.Height);
				case DropDownDirection.AboveLeft:
					return new Point(szItem.Width - szPopup.Width, -szPopup.Height);
				case DropDownDirection.Right:
					return new Point(szItem.Width, 0);
				}

				return new Point(0, szItem.Height);
			}
		}

		#endregion

		#region DropDownBounds

        /// <summary>
        /// Gets the drop down bounds.
        /// </summary>
        /// <value>The drop down bounds.</value>
		internal Rect DropDownBounds
		{
			get
			{
				Point pt = this.DropDownLocation;

				if (this.root != null)
				{
					GeneralTransform gt = this.root.TransformToVisual(this);

					pt = gt.Transform(pt);
				}

				return new Rect(pt, new Size(0, 0));
			}
		}

		#endregion

		#region VisualState

        /// <summary>
        /// Gets the state of the visual.
        /// </summary>
        /// <value>The state of the visual.</value>
		internal override string VisualState
		{
			get
			{
				if (this.IsDropDownOpen)
				{
					return "Pressed";
				}

				return base.VisualState;
			}
		}

		#endregion

		#region HasFocus

        /// <summary>
        /// Gets a value indicating whether this instance has focus.
        /// </summary>
        /// <value><c>true</c> if this instance has focus; otherwise, <c>false</c>.</value>
		private bool HasFocus
		{
			get
			{
				bool bResult = false;

				for (DependencyObject obj = FocusManager.GetFocusedElement() as DependencyObject; !bResult && obj != null;)
				{
					bResult = object.ReferenceEquals(obj, this);

					if (!bResult)
					{
						DependencyObject parent = VisualTreeHelper.GetParent(obj);

						if (parent == null)
						{
							FrameworkElement element = obj as FrameworkElement;

							if (element != null)
							{
								parent = element.Parent;
							}
						}

						obj = parent;
					}
				}

				return bResult;
			}
		}

		#endregion

		#region ShowPopup

        /// <summary>
        /// Gets or sets a value indicating whether [show popup].
        /// </summary>
        /// <value><c>true</c> if [show popup]; otherwise, <c>false</c>.</value>
		internal bool ShowPopup
		{
			get
			{
				return this.showPopup;
			}

			set
			{
				if (this.showPopup != value)
				{
					this.showPopup = value;

					this.OnShowPopupChanged();
				}
			}
		}

        /// <summary>
        /// Gets the show popup timer.
        /// </summary>
        /// <value>The show popup timer.</value>
		private DispatcherTimer ShowPopupTimer
		{
			get
			{
				if (this.showPopupTimer == null)
				{
					this.showPopupTimer = new DispatcherTimer();
					this.showPopupTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
					this.showPopupTimer.Tick += new EventHandler(this.OnShowPopupTick);
				}

				return this.showPopupTimer;
			}
		}

		#endregion

		#region IsAutoOpenEnabled

		/// <summary>
		/// Gets or sets a value indicating whether auto open of drop down is enabled.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if auto open of drop down is enabled; otherwise, <c>false</c>.
		/// </value>
		public bool IsAutoOpenEnabled
		{
			get { return (bool)GetValue(IsAutoOpenEnabledProperty); }
			set { SetValue(IsAutoOpenEnabledProperty, value); }
		}

		/// <summary>
		/// Using a DependencyProperty as the backing store for IsAutoOpenEnabled.  This enables animation, styling, binding, etc...
		/// </summary>
		public static readonly DependencyProperty IsAutoOpenEnabledProperty =
            DependencyProperty.Register("IsAutoOpenEnabled", typeof(bool), typeof(RibbonDropDownItem), new PropertyMetadata(false));

		#endregion

		#endregion

		#region Overrides

		/// <summary>
		/// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			this.root = this.GetTemplateChild("Part_Root") as UIElement;
		}

        /// <summary>
        /// Called when [show popup changed].
        /// </summary>
		private void OnShowPopupChanged()
		{
			if (this.ShowPopup)
			{
				this.ShowPopupTimer.Start();
			}
			else
			{
				this.ShowPopupTimer.Stop();
			}
		}

		/// <summary>
		/// Provides handling for the <see cref="E:System.Windows.UIElement.LostFocus"/> event.
		/// </summary>
		/// <param name="e">The event data for the <see cref="E:System.Windows.UIElement.LostFocus"/> event.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// 	<paramref name="e"/> is null.
		/// </exception>
		protected override void OnLostFocus(RoutedEventArgs e)
		{
			base.OnLostFocus(e);

			this.ProcessLostFocus(e);
		}

        /// <summary>
        /// Processes the mouse down.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
		internal override bool ProcessMouseDown(MouseButtonEventArgs e)
		{
			if (!this.IsAutoOpenEnabled)
			{
				this.IsDropDownOpen = true;
			}

			return true;
		}

        /// <summary>
        /// Called when [is selected changed].
        /// </summary>
        /// <param name="oldValue">if set to <c>true</c> [old value].</param>
        /// <param name="newValue">if set to <c>true</c> [new value].</param>
		internal override void OnIsSelectedChanged(bool oldValue, bool newValue)
		{
			base.OnIsSelectedChanged(oldValue, newValue);

			this.RaiseShowPopup(newValue);
		}

		#endregion

		#region Implementation

        /// <summary>
        /// Called when [drop down changed].
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
		private void OnDropDownChanged(RibbonDropDown oldValue, RibbonDropDown newValue)
		{
			if (oldValue != null)
			{
				oldValue.IsOpenChanged -= new EventHandler(this.OnDropDownIsOpenChanged);
				oldValue.OutsideRectMouseMove -= new MouseEventHandler(this.OnMenuOutsideRectMouseMove);
				oldValue.OutsideRectMouseDown -= new MouseButtonEventHandler(this.OnMenuOutsideRectMouseDown);
			}

			if (newValue != null)
			{
				newValue.IsOpenChanged += new EventHandler(this.OnDropDownIsOpenChanged);
				newValue.OutsideRectMouseMove += new MouseEventHandler(this.OnMenuOutsideRectMouseMove);
				newValue.OutsideRectMouseDown += new MouseButtonEventHandler(this.OnMenuOutsideRectMouseDown);
			}
		}

        /// <summary>
        /// Called when [is drop down open changed].
        /// </summary>
        /// <param name="oldValue">if set to <c>true</c> [old value].</param>
        /// <param name="newValue">if set to <c>true</c> [new value].</param>
		private void OnIsDropDownOpenChanged(bool oldValue, bool newValue)
		{
			if (this.DropDown != null)
			{
				this.DropDown.IsOpen = newValue;
			}

			if (this.IsDropDownOpenChanged != null)
			{
				this.IsDropDownOpenChanged(this, new EventArgs());
			}

			this.UpdateVisualState();
		}

        /// <summary>
        /// Processes the lost focus.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void ProcessLostFocus(RoutedEventArgs e)
		{
			if (!this.HasFocus)
			{
				this.IsDropDownOpen = false;
			}
		}

        /// <summary>
        /// Raises the show popup.
        /// </summary>
        /// <param name="bShow">if set to <c>true</c> [b show].</param>
		internal virtual void RaiseShowPopup(bool bShow)
		{
			if (this.IsAutoOpenEnabled)
			{
				this.ShowPopup = bShow;
			}
		}

		#endregion

		#region Event handlers

        /// <summary>
        /// Called when [menu outside rect mouse move].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
		internal virtual void OnMenuOutsideRectMouseMove(object sender, MouseEventArgs e)
		{
			if (this.IsAutoOpenEnabled)
			{
				if (!ContainsMouse(e, this))
				{
					this.ShowPopup = false;
					this.IsDropDownOpen = false;
				}
			}
		}

        /// <summary>
        /// Called when [menu outside rect mouse down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
		internal virtual void OnMenuOutsideRectMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (this.IsAutoOpenEnabled)
			{
				if (ContainsMouse(e, this))
				{
					e.Handled = true;
				}
			}
		}

        /// <summary>
        /// Called when [show popup tick].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnShowPopupTick(object sender, EventArgs e)
		{
			this.IsDropDownOpen = true;

			this.ShowPopup = false;
		}

        /// <summary>
        /// Called when [drop down is open changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnDropDownIsOpenChanged(object sender, EventArgs e)
		{
			RibbonDropDown dropDown = sender as RibbonDropDown;

			if (dropDown != null)
			{
				bool isOpen = dropDown.IsOpen;

				this.IsDropDownOpen = isOpen;

				if (isOpen)
				{
					UIElement menuParent = VisualTreeHelper.GetParent(dropDown) as UIElement;
					if (menuParent != null)
					{
						BoundsEventArgs bounds = new BoundsEventArgs(this.DropDownBounds);

						if (this.QueryDropDownBounds != null)
						{
							this.QueryDropDownBounds(this, bounds);
						}

						GeneralTransform gt = this.TransformToVisual(menuParent);
						Point pt = gt.Transform(new Point(bounds.Rect.X, bounds.Rect.Y));
						MatrixTransform transform = new MatrixTransform();
						transform.Matrix = new Matrix(1, 0, 0, 1, pt.X, pt.Y);

						dropDown.SetSize(bounds.Rect.Width, bounds.Rect.Height);

						dropDown.RenderTransform = transform;
					}
				}
			}
		}

        /// <summary>
        /// Called when [popup key down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
		private void OnPopupKeyDown(object sender, KeyEventArgs e)
		{
			if (!e.Handled)
			{
				switch (e.Key)
				{
				case Key.Escape:
				this.IsDropDownOpen = false;
				e.Handled = true;
				break;
				default:
				break;
				}
			}
		}

        /// <summary>
        /// Called when [popup lost focus].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void OnPopupLostFocus(object sender, RoutedEventArgs e)
		{
			this.ProcessLostFocus(e);
		}

		#endregion

		#region Events

        /// <summary>
        /// Occurs when [query drop down bounds].
        /// </summary>
		internal event EventHandler<BoundsEventArgs> QueryDropDownBounds;

        /// <summary>
        /// Occurs when [is drop down open changed].
        /// </summary>
		internal event EventHandler IsDropDownOpenChanged;

		#endregion

		#region Fields

		internal UIElement root;
		private DispatcherTimer showPopupTimer = null;
		private bool showPopup = false;

		#endregion
	}
}
