#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.Generic;

namespace Syncfusion.Windows.Tools.Controls 
{
	/// <summary>
	/// Specifies the possible alignments with which the items of a RibbonBar can be displayed
	/// </summary>
	public enum LayoutMode
	{
		/// <summary>
		/// Table layout
		/// </summary>
		Table,

		/// <summary>
		/// Flow layout
		/// </summary>
		Flow
	}

	/// <summary>
    /// Provides a container for <see cref="RibbonBar"/>s. 
	/// </summary>
	[TemplateVisualState(GroupName = "RibbonBarStates", Name = "Normal")]
	[TemplateVisualState(GroupName = "RibbonBarStates", Name = "Selected")]
	[TemplateVisualState(GroupName = "RibbonBarStates", Name = "Disabled")]
	[TemplateVisualState(GroupName = "DropDownStates", Name = "DropDownNormal")]
	[TemplateVisualState(GroupName = "DropDownStates", Name = "DropDownSelected")]
	[TemplateVisualState(GroupName = "DropDownStates", Name = "DropDownPressed")]
	public class RibbonBar :
		RibbonItemsControl, IRibbonControl,IDisposable
	{
		#region Constants

		/// <summary>
		/// Identifies the constant MinHeightOfRibbonBar.
		/// </summary>
		internal const double MinHeightOfRibbonBar = 70;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes the <see cref="RibbonBar"/> class.
		/// </summary>
		static RibbonBar()
		{
			//resources = new RibbonBarResources();
		}

		/// <summary>
		/// Initialize new instance of <see cref="RibbonBar"/>
		/// </summary>
		public RibbonBar()
		{
			this.DefaultStyleKey = typeof(RibbonBar);

			this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(this.OnIsEnabledChanged);
            this.Unloaded += new RoutedEventHandler(RibbonBar_Unloaded);
		}

        void RibbonBar_Unloaded(object sender, RoutedEventArgs e)
        {            
           
        }

		#endregion

		#region Properties

		#region Header

		/// <summary>
		/// Gets or sets the <see cref="RibbonBar"/> Header's text.
		/// </summary>
		[Description("Represents the Name for Ribbon Bar")]
		[Category("Common Properties")]
		public string Header
		{
			get
			{
				return (string)GetValue(HeaderProperty);
			}

			set
			{
				SetValue(HeaderProperty, value);
			}
		}

		/// <summary>
		/// The identifier of <see cref="Header"/> property
		/// </summary>
		public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(RibbonBar), new PropertyMetadata(null));

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
				if (IsEnabled)
				{
					if (this.isSelected)
					{
						return "Selected";
					}

					return "Normal";
				}

				return "Disabled";
			}
		}

		#endregion

		#region DropDownState

		/// <summary>
		/// Gets the state of the drop down.
		/// </summary>
		/// <value>The state of the drop down.</value>
		internal string DropDownState
		{
			get
			{
				if (!this.IsDropDownOpen)
				{
					if (this.isDropDownSelected)
					{
						return "DropDownSelected";
					}

					return "DropDownNormal";
				}

				return "DropDownPressed";
			}
		}
		#endregion

		#region Selected

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="RibbonBar"/> is selected.
		/// </summary>
		/// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
		internal bool Selected
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

					this.UpdateVisualState();
				}
			}
		}

		#endregion

		#region DropDownSelected

		/// <summary>
		/// Gets or sets a value indicating whether [drop down selected].
		/// </summary>
		/// <value><c>true</c> if [drop down selected]; otherwise, <c>false</c>.</value>
		internal bool DropDownSelected
		{
			get
			{
				return this.isDropDownSelected;
			}

			set
			{
				if (this.isDropDownSelected != value)
				{
					this.isDropDownSelected = value;

					this.UpdateDropDownState();
				}
			}
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		[Description("Occurs when Launcher Button Clicked")]
		[Category("Common Properties")]
		public ICommand LauncherCommand
		{
			get { return (ICommand)GetValue(LauncherCommandProperty); }
			set { SetValue(LauncherCommandProperty, value); }
		}

		
		/// <summary>
        /// Using a DependencyProperty as the backing store for LauncherCommand.  This enables animation, styling, binding, etc...
		/// </summary>
		public static readonly DependencyProperty LauncherCommandProperty =
			DependencyProperty.Register("LauncherCommand", typeof(ICommand), typeof(RibbonBar), new PropertyMetadata(null));
		
		#region ShowLauncherButton

		/// <summary>
		/// Gets or sets a value indicating whether the launcher button is visible.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is show launcher button; otherwise, <c>false</c>.
		/// </value>
		[Description("Used to show/hide Launcher Button")]
		[Category("Common Properties")]
		public bool IsLauncherButtonVisible
		{
			get
			{
				return (bool)GetValue(IsLauncherButtonVisibleProperty);
			}

			set
			{
				SetValue(IsLauncherButtonVisibleProperty, value);
			}
		}

		/// <summary>
        /// The identifier of <see cref="IsLauncherButtonVisible"/> property
		/// </summary>
		public static readonly DependencyProperty IsLauncherButtonVisibleProperty = DependencyProperty.Register("IsLauncherButtonVisible", typeof(bool), typeof(RibbonBar), new PropertyMetadata(true, new PropertyChangedCallback(OnIsShowLauncherButtonChangedCallback)));

		/// <summary>
		/// Called when [is show launcher button changed callback].
		/// </summary>
		/// <param name="d">The d.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void OnIsShowLauncherButtonChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((RibbonBar)d).OnIsShowLauncherButtonChanged();
		}

		#endregion

		#region Collapsed

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="RibbonBar"/> is collapsed.
		/// </summary>
		/// <value><c>true</c> if collapsed; otherwise, <c>false</c>.</value>
		internal bool Collapsed
		{
			get
			{
				return this.isCollapsed;
			}

			set
			{
				if (this.isCollapsed != value)
				{
					this.isCollapsed = value;

					if (value)
						this.BarSeperatorVisibility = Visibility.Collapsed;
					else
						this.BarSeperatorVisibility = Visibility.Visible;

					this.OnCollapsedChanged();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Visibility BarSeperatorVisibility
		{
			get { return (Visibility)GetValue(BarSeperatorVisibilityProperty); }
			set { SetValue(BarSeperatorVisibilityProperty, value); }
		}

		// Using a DependencyProperty as the backing store for BarSeperatorVisibility.  This enables animation, styling, binding, etc...
		/// <summary>
		/// 
		/// </summary>
		public static readonly DependencyProperty BarSeperatorVisibilityProperty =
			DependencyProperty.Register("BarSeperatorVisibility", typeof(Visibility), typeof(RibbonBar), new PropertyMetadata(Visibility.Visible));

		


		#endregion

		#region IsDropDownOpen

		/// <summary>
		/// Gets or sets a value indicating whether this instance is drop down open.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is drop down open; otherwise, <c>false</c>.
		/// </value>
		internal bool IsDropDownOpen
		{
			get
			{
				return (bool)GetValue(IsDropDownOpenProperty);
			}

			set
			{
				SetValue(IsDropDownOpenProperty, value);
			}
		}

		/// <summary>
		/// Identifies the IsDropDownOpen Dependency Property.
		/// </summary>
		internal static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(RibbonBar), new PropertyMetadata(false, new PropertyChangedCallback(OnIsDropDownOpenChangedCallback)));

		/// <summary>
		/// Called when [is drop down open changed callback].
		/// </summary>
		/// <param name="d">The d.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void OnIsDropDownOpenChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((RibbonBar)d).OnIsDropDownOpenChanged((bool)e.NewValue);
		}

		#endregion

		#region CollapseImage

		/// <summary>
		/// Gets or sets the source of image to display in collapsed mode
		/// </summary>
		/// <value>The collapsed image.</value>
		[Description("Used to show when Ribbon Bar is in Collapsed State")]
		[Category("Common Properties")]
		public ImageSource CollapseImage
		{
			get
			{
				return (ImageSource)GetValue(CollapseImageProperty);
			}

			set
			{
				SetValue(CollapseImageProperty, value);
			}
		}

		/// <summary>
        /// The identifier of <see cref="CollapseImage"/> property
		/// </summary>
		public static readonly DependencyProperty CollapseImageProperty = DependencyProperty.Register("CollapseImage", typeof(ImageSource), typeof(RibbonBar), null);

		#endregion

		#region LayoutMode

		/// <summary>
		/// 
		/// </summary>
		public bool IsLargeButtonPanel
		{
			get { return (bool)GetValue(IsLargeButtonPanelProperty); }
			set { SetValue(IsLargeButtonPanelProperty, value); }
		}

		
		/// <summary>
        /// Using a DependencyProperty as the backing store for IsLargeButtonPanel.  This enables animation, styling, binding, etc...
		/// </summary>
		public static readonly DependencyProperty IsLargeButtonPanelProperty =
			DependencyProperty.Register("IsLargeButtonPanel", typeof(bool), typeof(RibbonBar), new PropertyMetadata(true, new PropertyChangedCallback(IsLargeButtonPanelChangedCallback)));
		
		/// <summary>
		/// Layouts the mode changed callback.
		/// </summary>
		/// <param name="d">The d.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void IsLargeButtonPanelChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var senderobj = d as RibbonBar;
			
			if (senderobj == null) return;

			if (senderobj.IsLargeButtonPanel == true)
				senderobj.LayoutMode = LayoutMode.Table;
			else
				senderobj.LayoutMode = LayoutMode.Flow;
		}
		
		/// <summary>
		/// Gets or sets a value indicating how the <see cref="RibbonBar"/> lays out the items.
		/// </summary>
		/// <value>The layout mode.</value>
		[Description("Represents the number of rows to be displayed")]
		[Category("Common Properties")]
		public LayoutMode LayoutMode
		{
			get
			{
				return (LayoutMode)GetValue(LayoutModeProperty);
			}

			set
			{
				SetValue(LayoutModeProperty, value);
			}
		}

		/// <summary>
		/// The identifier of <see cref="LayoutMode"/> property
		/// </summary>
		public static readonly DependencyProperty LayoutModeProperty = DependencyProperty.Register("LayoutMode", typeof(LayoutMode), typeof(RibbonBar), new PropertyMetadata(LayoutMode.Table, new PropertyChangedCallback(LayoutModeChangedCallback)));

		/// <summary>
		/// Layouts the mode changed callback.
		/// </summary>
		/// <param name="d">The d.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void LayoutModeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((RibbonBar)d).OnLayoutModeChanged();
		}

		#endregion

		#region LauncherButton

		/// <summary>
		/// Gets or sets the launcher button.
		/// </summary>
		/// <value>The launcher button.</value>
		internal RibbonButton LauncherButton
		{
			get
			{
				return this.launcherButton;
			}

			set
			{
				if (this.launcherButton != value)
				{
					if (this.launcherButton != null)
					{
						this.launcherButton.Click -= new RoutedEventHandler(this.OnLauncherClick);
					}

					this.launcherButton = value;

					if (this.launcherButton != null)
					{
						this.launcherButton.Click += new RoutedEventHandler(this.OnLauncherClick);
					}
				}
			}
		}
		#endregion

		#region OutsideRect

		/// <summary>
		/// Gets or sets the outside rect.
		/// </summary>
		/// <value>The outside rect.</value>
		internal Rectangle OutsideRect
		{
			get
			{
				return this.outsideRect;
			}

			set
			{
				if (this.outsideRect != value)
				{
					if (this.outsideRect != null)
					{
						this.outsideRect.MouseLeftButtonDown -= new MouseButtonEventHandler(this.OutsideRectMouseLeftButtonDown);
					}

					this.outsideRect = value;

					if (this.outsideRect != null)
					{
						this.outsideRect.MouseLeftButtonDown += new MouseButtonEventHandler(this.OutsideRectMouseLeftButtonDown);
					}
				}
			}
		}

		#endregion

		#region RibbonBarStrip

		/// <summary>
		/// Gets or sets the ribbon bar strip.
		/// </summary>
		/// <value>The ribbon bar strip.</value>
		internal FrameworkElement RibbonBarStrip
		{
			get
			{
				return this.ribbonBarStrip;
			}

			set
			{
				if (this.ribbonBarStrip != value)
				{
					if (this.ribbonBarStrip != null)
					{
						this.ribbonBarStrip.MouseEnter -= new MouseEventHandler(this.OnRibbonStripMouseEnter);
						this.ribbonBarStrip.MouseLeave -= new MouseEventHandler(this.OnRibbonStripMouseLeave);
					}

					this.ribbonBarStrip = value;

					if (this.ribbonBarStrip != null)
					{
						this.ribbonBarStrip.MouseEnter += new MouseEventHandler(this.OnRibbonStripMouseEnter);
						this.ribbonBarStrip.MouseLeave += new MouseEventHandler(this.OnRibbonStripMouseLeave);
					}
				}
			}
		}
		#endregion

		#region DropDownButton

		/// <summary>
		/// Gets or sets the drop down button.
		/// </summary>
		/// <value>The drop down button.</value>
		internal FrameworkElement DropDownButton
		{
			get
			{
				return this.dropdownButton;
			}

			set
			{
				if (this.dropdownButton != value)
				{
					if (this.dropdownButton != null)
					{
						this.dropdownButton.MouseEnter -= new MouseEventHandler(this.OnDropDownButtonMouseEnter);
						this.dropdownButton.MouseLeave -= new MouseEventHandler(this.OnDropDownButtonMouseLeave);
					}

					this.dropdownButton = value;

					if (this.dropdownButton != null)
					{
						this.dropdownButton.MouseEnter += new MouseEventHandler(this.OnDropDownButtonMouseEnter);
						this.dropdownButton.MouseLeave += new MouseEventHandler(this.OnDropDownButtonMouseLeave);
					}
				}
			}
		}
		#endregion

		#region PopupContent

		/// <summary>
		/// Gets or sets the content of the popup.
		/// </summary>
		/// <value>The content of the popup.</value>
		internal FrameworkElement PopupContent
		{
			get
			{
				return this.popupContent;
			}

			set
			{
				if (this.popupContent != value)
				{
					if (this.popupContent != null)
					{
						this.popupContent.SizeChanged -= new SizeChangedEventHandler(this.OnPopupContentSizeChanged);
					}

					this.popupContent = value;

					if (this.popupContent != null)
					{
						this.popupContent.SizeChanged += new SizeChangedEventHandler(this.OnPopupContentSizeChanged);
					}
				}
			}
		}

		#endregion

		#region Popup

		/// <summary>
		/// Gets or sets the popup.
		/// </summary>
		/// <value>The popup.</value>
		internal Popup Popup
		{
			get
			{
				return this.popup;
			}

			set
			{
				if (this.popup != value)
				{
					this.popup = value;
				}
			}
		}

		#endregion

		#endregion

		#region Overrides

	    /// <summary>
	    /// Called before the <see cref="E:System.Windows.UIElement.MouseMove"/> event occurs.
	    /// </summary>
	    /// <param name="e">The data for the event. </param>
	    protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			//ScreenTip.CurrentHorizontalOffset = e.GetPosition(this).X;
		}


		internal TextBlock PartTextBlock;
		/// <summary>
		/// Called when a new template is applied. 
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			this.expandedHost = this.GetTemplateChild("Part_ExpandedHost") as ContentPresenter;
			this.collapsedHost = this.GetTemplateChild("Part_CollapsedHost") as ContentPresenter;

			this.RibbonBarStrip = this.GetTemplateChild("Part_RibbonBarStrip") as FrameworkElement;
			this.DropDownButton = this.GetTemplateChild("Part_DropDown") as FrameworkElement;
			this.PopupContent = this.GetTemplateChild("Part_PopupContent") as FrameworkElement;

			this.LauncherButton = this.GetTemplateChild("Part_LauncherButton") as RibbonButton;

			this.OutsideRect = this.GetTemplateChild("Part_OutsideRect") as Rectangle;

			this.Popup = this.GetTemplateChild("Part_Popup") as Popup;

			this.PartTextBlock = this.GetTemplateChild("Part_Text") as TextBlock;

			this.OnIsShowLauncherButtonChanged();
			this.OnLayoutModeChanged();

			this.UpdateVisualState();
		}

		/// <summary>
		/// Called before the <see cref="E:System.Windows.UIElement.KeyDown"/> event occurs.
		/// </summary>
		/// <param name="e">The data for the event.</param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				if (this.IsDropDownOpen)
				{
					this.IsDropDownOpen = false;
					e.Handled = true;
				}
			}

			base.OnKeyDown(e);
		}

		internal bool IsRightClickNeeded = false;

	    /// <summary>
	    /// Called before the <see cref="E:System.Windows.UIElement.MouseRightButtonDown"/> event occurs.
	    /// </summary>
	    /// <param name="e">A <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data.</param>
	    protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
		{
			if (IsRightClickNeeded) { IsRightClickNeeded = false; return; }
			FrameworkElement realSource = RibbonContextMenu.GetRealSource(this, e);
			if (realSource == null) return;
			var parentRibbon = VisualUtils.FindAncestor(this, typeof(Ribbon));
			if (parentRibbon != null && parentRibbon is Ribbon && realSource != null)
			{
				((Ribbon)parentRibbon).AddQATinContextMenu(realSource);
			}            
			base.OnMouseRightButtonDown(e);

		}

		/// <summary>
		/// Called before the <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> event occurs.
		/// </summary>
		/// <param name="e">The data for the event.</param>
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);

			if (this.Collapsed)
            {
                List<Popup> popups = (List<Popup>)VisualTreeHelper.GetOpenPopups();
                foreach (Popup popup in popups)
                {
                    popup.IsOpen = false;
                }

				this.IsDropDownOpen = !this.IsDropDownOpen;
			}
		}

		/// <summary>
		/// Called when [item clicked].
		/// </summary>
		/// <param name="item">The item.</param>
		internal override void OnItemClicked(UIElement item)
		{
			base.OnItemClicked(item);

			if (this.Collapsed)
			{
				this.IsDropDownOpen = false;
			}
		}

		/// <summary>
		/// Called when [is show launcher button changed].
		/// </summary>
		internal virtual void OnIsShowLauncherButtonChanged()
		{
			if (this.launcherButton != null)
			{
				//this.launcherButton.Visibility = this.ShowLauncherButton ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		/// <summary>
		/// Called when [collapsed changed].
		/// </summary>
		internal virtual void OnCollapsedChanged()
		{
			this.IsDropDownOpen = false;

			if (this.expandedHost != null && this.collapsedHost != null)
			{
				if (this.Collapsed)
				{
					object content = this.expandedHost.Content;

					this.expandedHost.Content = null;
					this.collapsedHost.Content = content;

					this.expandedHost.Visibility = Visibility.Collapsed;

					if (this.dropdownButton != null)
					{
						this.dropdownButton.Visibility = Visibility.Visible;
					}
				}
				else
				{
					object content = this.collapsedHost.Content;

					this.collapsedHost.Content = null;
					this.expandedHost.Content = content;

					this.expandedHost.Visibility = Visibility.Visible;

					if (this.dropdownButton != null)
					{
						this.dropdownButton.Visibility = Visibility.Collapsed;
					}
				}
			}

			if (this.CollapsedChanged != null)
			{
				this.CollapsedChanged(this, new EventArgs());
			}
		}

        private void OnRootVisualMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsDropDownOpen)
                IsDropDownOpen = false;
        }
        private void OnRootVisualKeyDown(object sender, KeyEventArgs e)
        {
            if (IsDropDownOpen)
                IsDropDownOpen = false;
        }
       

		/// <summary>
		/// Called when [is drop down open changed].
		/// </summary>
		/// <param name="isOpen">if set to <c>true</c> [is open].</param>
		internal virtual void OnIsDropDownOpenChanged(bool isOpen)
		{
			Popup popup = this.Popup;
			this.ResetAll();

            if (isOpen)
            {
                Application.Current.RootVisual.AddHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnRootVisualMouseLeftButtonDown), true);
                Application.Current.RootVisual.AddHandler(FrameworkElement.KeyDownEvent, new KeyEventHandler(OnRootVisualKeyDown),true);
            }
            else
            {
                Application.Current.RootVisual.RemoveHandler(FrameworkElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnRootVisualMouseLeftButtonDown));
                Application.Current.RootVisual.RemoveHandler(FrameworkElement.KeyDownEvent, new KeyEventHandler(OnRootVisualKeyDown));
            }

			if (popup != null)
			{
				popup.IsOpen = isOpen;

				if (isOpen)
				{
					this.ArrangePopup();
					this.ArrangeOutsideRect();

					this.focusedElement = FocusManager.GetFocusedElement() as Control;

					this.Dispatcher.BeginInvoke(() => { this.Focus(); });
				}
				else
				{
					if (this.focusedElement != null)
					{
						this.focusedElement.Focus();
						this.focusedElement = null;
					}
				}
			}

			this.UpdateVisualState();
			this.UpdateDropDownState();
		}


		/// <summary>
		/// Called when [layout mode changed].
		/// </summary>
		internal virtual void OnLayoutModeChanged()
		{
			/*switch (this.LayoutMode)
			{
				//case LayoutMode.Flow:
				//    this.ItemsPanel = RibbonBar.resources["FlowLayout"] as ItemsPanelTemplate;
				//    break;
				//default:
				//    this.ItemsPanel = RibbonBar.resources["TableLayout"] as ItemsPanelTemplate;
				//    break;               
			}*/
		}

		internal bool ChangeLargeItems()
		{
			var largeItems = from item in Items
							 where item is IRibbonItem && ((IRibbonItem)item).SizeMode == SizeMode.Large && ((IRibbonItem)item).IsAutoSizeFormEnabled
							 select item;
			bool _flag = false;
			foreach (IRibbonItem item in largeItems)
			{

				RibbonButton _button = item as RibbonButton;
				if (_button != null && !_button._istempsize)
				{
					_button._istempsize = true;
					_button.SizeMode = SizeMode.Normal;
					_flag = true;

				}
				else
				{
					RibbonDropDownButton _dropdown = item as RibbonDropDownButton;
					if (_dropdown != null && !_dropdown._istempsize)
					{
						_dropdown._istempsize = true;
						_dropdown.SizeMode = SizeMode.Normal;
						_flag = true;

					}
				}

			}
			return _flag;
		}

		internal bool ChangeNormalItems()
		{
			var largeItems = from item in Items
							 where item is IRibbonItem && ((IRibbonItem)item).SizeMode == SizeMode.Normal && ((IRibbonItem)item).IsAutoSizeFormEnabled
							 select item;
			bool _flag = false;
			foreach (IRibbonItem item in largeItems)
			{

				RibbonButton _button = item as RibbonButton;
				if (_button != null && !_button._istempsize)
				{
					_button._istempsize = true;
					_button.SizeMode = SizeMode.Small;
					_flag = true;

				}
				else
				{
					RibbonDropDownButton _dropdown = item as RibbonDropDownButton;
					if (_dropdown != null && !_dropdown._istempsize)
					{
						_dropdown._istempsize = true;
						_dropdown.SizeMode = SizeMode.Small;
						_flag = true;

					}
				}

			}
			return _flag;
		}

		internal bool ResetNormalItems()
		{
			var largeItems = from item in Items
							 where item is IRibbonItem && ((IRibbonItem)item).SizeMode == SizeMode.Normal && ((IRibbonItem)item).IsAutoSizeFormEnabled
							 select item;
			bool _flag = false;
			foreach (IRibbonItem item in largeItems)
			{
				RibbonButton _button = item as RibbonButton;
				if (_button != null)
				{
					if (_button._istempsize)
					{
						_button.SizeMode = SizeMode.Large;
						_button._istempsize = false;
					}
				}
				else
				{
					RibbonDropDownButton _dropdown = item as RibbonDropDownButton;
					if (_dropdown != null)
					{
						if (_dropdown._istempsize)
						{
							_dropdown.SizeMode = SizeMode.Large;
							_dropdown._istempsize = false;
						}
					}
				}
				_flag = true;
			}
			
			return _flag;
		}

		internal bool ResetSmallItems()
		{
			var largeItems = from item in Items
							 where item is IRibbonItem && ((IRibbonItem)item).SizeMode == SizeMode.Small
							 select item;
			bool _flag = false;
			foreach (IRibbonItem item in largeItems)
			{
				RibbonButton _button = item as RibbonButton;
				if (_button != null)
				{
					if (_button._istempsize)
					{
						_button.SizeMode = SizeMode.Normal;
						_button._istempsize = false;
					}
				}
				else
				{
					RibbonDropDownButton _dropdown = item as RibbonDropDownButton;
					if (_dropdown != null)
					{
						if (_dropdown._istempsize)
						{
							_dropdown.SizeMode = SizeMode.Normal;
							_dropdown._istempsize = false;
						}
					}
				}
				_flag = true;
			}
			return _flag;
		}

		internal void ResetAll()
		{
			var items = from item in Items
						where item is IRibbonItem
						select item;

			foreach (IRibbonItem item in items)
			{
				RibbonButton _button = item as RibbonButton;
				if (_button != null)
				{
					if (_button._istempsize)
					{
						if (_button.SizeMode == SizeMode.Small)
						{
							_button.SizeMode = SizeMode.Normal;
							_button.Tag = "Reset";
						}
						else if(_button.SizeMode == SizeMode.Normal)
						{
							_button.SizeMode = SizeMode.Large; 
							_button.Tag = "Reset";
						}
						_button._istempsize = false;
					}
				}
				else
				{
					RibbonDropDownButton _dropdown = item as RibbonDropDownButton;
					if (_dropdown != null)
					{
						if (_dropdown._istempsize)
						{
							if (_dropdown.SizeMode == SizeMode.Small)
							{
								_dropdown.SizeMode = SizeMode.Normal;
								_dropdown.Tag = "Reset";
							}
							else if (_dropdown.SizeMode == SizeMode.Normal)
							{
								_dropdown.SizeMode = SizeMode.Large;
								_dropdown.Tag = "Reset";
							}
							_dropdown._istempsize = false;
						}
					}
				}
			}
		}

		internal void RevertAll()
		{
			var items = from item in Items
						where item is IRibbonItem
						select item;


			foreach (IRibbonItem item in items)
			{
				RibbonButton _button = item as RibbonButton;
				if (_button != null)
				{
					if (_button.Tag != null &&  _button.Tag.ToString() == "Reset")
					{
						if (_button.Tag != null && _button.SizeMode == SizeMode.Normal)
						{
							_button.SizeMode = SizeMode.Small;
						}
						else if (_button.SizeMode == SizeMode.Large)
						{
							_button.SizeMode = SizeMode.Normal;
						}
						_button._istempsize = false;
					}
				}
				else
				{
					RibbonDropDownButton _dropdown = item as RibbonDropDownButton;
					if (_dropdown != null)
					{
						if (_dropdown.Tag != null && _dropdown.Tag.ToString() == "Reset")
						{
							if (_dropdown.SizeMode == SizeMode.Normal)
							{
								_dropdown.SizeMode = SizeMode.Small;
							}
							else if (_dropdown.SizeMode == SizeMode.Large)
							{
								_dropdown.SizeMode = SizeMode.Normal;
							}
							_dropdown._istempsize = false;
						}
					}
				}
			}
		}
		double original = 0.0;

		internal bool ResizeGallery()
		{
			foreach (var item in Items)
			{
				if (item is RibbonGallery)
				{
					RibbonGallery _gallery = item as RibbonGallery;
					double _width = double.IsNaN(_gallery.Width) ? _gallery.ActualWidth : _gallery.Width;
					if (original == 0.0)
						original = _width;
					double _itemwidth = 0.0;

					foreach (var galleryitem in _gallery.Items)
					{
						if (galleryitem is RibbonGalleryItem)
						{
							_itemwidth = ((RibbonGalleryItem)galleryitem).ActualWidth;
							break;
						}
					}

					_width -= 60;

					if (_width > 0)
					{
						_gallery.Width = _width;
						_gallery.IsResized = true;
						return true;
					}
				}
			}
			return false;
		}


		internal void RevertGallery()
		{
			foreach (var item in Items)
			{
				if (item is RibbonGallery)
				{
					RibbonGallery _gallery = item as RibbonGallery;
					if (true)
					{
						double _width = double.IsNaN(_gallery.Width) ? _gallery.ActualWidth : _gallery.Width;
						double _itemwidth = 0.0;

						foreach (var galleryitem in _gallery.Items)
						{
							if (galleryitem is RibbonGalleryItem)
							{
								_itemwidth = ((RibbonGalleryItem)galleryitem).ActualWidth;
							}
						}

						if(original != 0.0)
						_width = original;

						if (_width > 0.0)
						{
							_gallery.Width = _width;
							_gallery.IsResized = false;
						}
					}
				}
			}
		}


		#endregion

		#region Events

		/// <summary>
		/// Occurs when a launcher button is clicked. 
		/// </summary>
		public event RoutedEventHandler LauncherClick;

		/// <summary>
		/// Event that is raised when <see cref="RibbonBar.Collapsed"/> property is changed.
		/// </summary>
		public event EventHandler CollapsedChanged;

		#endregion

		#region Event Handlers

		/// <summary>
		/// Called when [ribbon strip mouse enter].
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
		private void OnRibbonStripMouseEnter(object sender, MouseEventArgs e)
		{
			this.Selected = true;
		}

		/// <summary>
		/// Called when [ribbon strip mouse leave].
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
		private void OnRibbonStripMouseLeave(object sender, MouseEventArgs e)
		{
			this.Selected = false;
		}

		/// <summary>
		/// Called when [drop down button mouse enter].
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
		private void OnDropDownButtonMouseEnter(object sender, MouseEventArgs e)
		{
			this.DropDownSelected = true;
		}

		/// <summary>
		/// Called when [drop down button mouse leave].
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
		private void OnDropDownButtonMouseLeave(object sender, MouseEventArgs e)
		{
			this.DropDownSelected = false;
		}

		/// <summary>
		/// Called when [is enabled changed].
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this.UpdateVisualState();
		}

		/// <summary>
		/// Called when [launcher click].
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
		private void OnLauncherClick(object sender, RoutedEventArgs e)
		{
			if (this.Collapsed)
			{
				this.IsDropDownOpen = false;
			}

			if (this.LauncherClick != null)
			{
				this.LauncherClick(sender, e);
			}
		}

		/// <summary>
		/// Outsides the rect mouse left button down.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
		private void OutsideRectMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.IsDropDownOpen = false;
		}

		/// <summary>
		/// Called when [popup content size changed].
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.SizeChangedEventArgs"/> instance containing the event data.</param>
		private void OnPopupContentSizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.ArrangePopup();
		}

		#endregion

		#region Implementation

		/// <summary>
		/// Updates the state of the visual.
		/// </summary>
		internal void UpdateVisualState()
		{
			VisualStateManager.GoToState(this, this.VisualState, false);
		}

		/// <summary>
		/// Updates the state of the drop down.
		/// </summary>
		internal void UpdateDropDownState()
		{
			VisualStateManager.GoToState(this, this.DropDownState, false);
		}

		/// <summary>
		/// Arranges the popup.
		/// </summary>
		private void ArrangePopup()
		{
			FrameworkElement popupContent = this.PopupContent;
			if (popupContent != null)
			{
				popupContent.RenderTransform = null;

				double width = popupContent.ActualWidth;

				if (width > 0)
				{
					GeneralTransform gt = popupContent.TransformToVisual(null);
					Point pt = gt.Transform(new Point(0, 0));

					System.Windows.Interop.Content content = Application.Current.Host.Content;
					double hostWidth = content.ActualWidth;

					double offset = pt.X + width - hostWidth;

					if (offset > 1)
					{
						MatrixTransform mt = new MatrixTransform();
						mt.Matrix = new Matrix(1, 0, 0, 1, -offset, 0);

						popupContent.RenderTransform = mt;
					}
				}
			}
		}

		/// <summary>
		/// Arranges the outside rect.
		/// </summary>
		private void ArrangeOutsideRect()
		{
			Rectangle rcOutside = this.OutsideRect;

			if (rcOutside != null)
			{
				MatrixTransform m = this.Popup.TransformToVisual(null) as MatrixTransform;

				this.SetMatrixTransform(rcOutside, -m.Matrix.OffsetX, -m.Matrix.OffsetY);

				System.Windows.Interop.Content content = Application.Current.Host.Content;

				rcOutside.Width = content.ActualWidth;
				rcOutside.Height = content.ActualHeight;
			}
		}

		/// <summary>
		/// Sets the matrix transform.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <param name="offsetX">The offset X.</param>
		/// <param name="offsetY">The offset Y.</param>
		private void SetMatrixTransform(UIElement element, double offsetX, double offsetY)
		{
			Matrix matrix = new Matrix();

			matrix.OffsetX = offsetX;
			matrix.OffsetY = offsetY;

			MatrixTransform transform = new MatrixTransform();
			transform.Matrix = matrix;

			element.RenderTransform = transform;
		}

		#endregion

		#region Fields

		private ContentPresenter expandedHost = null;
		private ContentPresenter collapsedHost = null;
		private FrameworkElement ribbonBarStrip = null;
		private FrameworkElement dropdownButton = null;
		private FrameworkElement popupContent = null;
		private Control focusedElement = null;

		private bool isCollapsed = false;
		private bool isSelected = false;
		private bool isDropDownSelected = false;

		private RibbonButton launcherButton = null;
		private Popup popup = null;
		private Rectangle outsideRect = null;

		//internal static RibbonBarResources resources = null;

		#endregion

	    /// <summary>
	    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
	    /// </summary>
	    public void Dispose()
        {
            this.launcherButton = null;
            this.ribbonBarStrip = null;
            this.PartTextBlock = null;
            this.expandedHost = null;
            if (this.ribbonBarStrip != null)
            {
                this.ribbonBarStrip.MouseEnter -= new MouseEventHandler(this.OnRibbonStripMouseEnter);
                this.ribbonBarStrip.MouseLeave -= new MouseEventHandler(this.OnRibbonStripMouseLeave);
            }

            this.Unloaded -= new RoutedEventHandler(RibbonBar_Unloaded);
        }
    }

	/// <summary>
	/// 
	/// </summary>
	public static class RibbonContextMenu
	{
		internal static FrameworkElement GetRealSource(ItemsControl source, MouseButtonEventArgs e)
		{
			Point point = e.GetPosition(source);
			FrameworkElement realSource = GetElementFromPoint(source, point);
			return realSource;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parentItemsControl"></param>
		/// <param name="point"></param>
		/// <returns></returns>
		public static FrameworkElement GetElementFromPoint(ItemsControl parentItemsControl, Point point)
		{
			Rect rect;
			Point startPoint = new Point(0,0);
			Point endPoint = new Point(0, 0);
			foreach (object child in parentItemsControl.Items)
			{
				UIElement element;

				if (child is UIElement)
					element = child as UIElement;
				else
					element = parentItemsControl.ItemContainerGenerator.ContainerFromItem(child) as UIElement;

				if (element != null)
				{
					if (element.Visibility == Visibility.Visible)
					{
						try
						{
							GeneralTransform gstart = element.TransformToVisual(parentItemsControl);
							startPoint = gstart.Transform(new Point(0, 0));
							endPoint = gstart.Transform(new Point(element.RenderSize.Width, element.RenderSize.Height));
							rect = new Rect(startPoint, endPoint);
							if (rect.Contains(point))
							{
								return element as FrameworkElement;
							}
						}
						catch
						{

						}
					}
				}
			}
			//return null;
			return parentItemsControl;
		}
	}
}
