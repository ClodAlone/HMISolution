#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls 
{
    /// <summary>
    /// Represents the Ribbon Tab Class.
    /// </summary>
	public class RibbonTab : ScrollStrip
	{
		#region Constructors

		/// <summary>
		/// Initialize a new instance of <see cref="RibbonTab"/>
		/// </summary>
        public RibbonTab()
        {
            this.DefaultStyleKey = typeof(RibbonTab);                 
        }
		#endregion

		#region Properties

		#region Text

		/// <summary>
		/// Gets or sets the text to display on <see cref="RibbonTab"/> 
		/// </summary>
        public string Caption
		{
			get
			{
                return (string)GetValue(CaptionProperty);
			}

			set
			{
                SetValue(CaptionProperty, value);
			}
		}

        private Ribbon ribbon;
        /// <summary>
        /// Gets the parent Ribbon
        /// </summary>
        internal Ribbon ParentRibbon
        {
            get
            {
                if(ribbon == null)
                    ribbon = VisualUtils.FindAncestor(this, typeof(Ribbon)) as Ribbon;
               return ribbon;
            }


        }

		/// <summary>
        /// The identifier of <see cref="Caption"/> property
		/// </summary>
        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(RibbonTab), new PropertyMetadata(TextChangedCallback));

        /// <summary>
        /// Texts the changed callback.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void TextChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((RibbonTab)d).OnTextChanged();
		}



        /// <summary>
        /// 
        /// </summary>
        public Visibility TabButtonVisiblity
        {
            get { return (Visibility)GetValue(TabButtonVisiblityProperty); }
            set { SetValue(TabButtonVisiblityProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for TabButtonVisiblity.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TabButtonVisiblityProperty =
            DependencyProperty.Register("TabButtonVisiblity", typeof(Visibility), typeof(RibbonTab), new PropertyMetadata(Visibility.Visible, OnTabVisibilityChanged));

        private static void OnTabVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as RibbonTab;
            if (obj.ParentRibbon != null)
            {
                obj.ParentRibbon.UpdateTabsHeight();
                obj.ParentRibbon.UpdateTabStrip();
            }
        }

		#endregion

		#region Color

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
		internal Color Color
		{
			get
			{
				return this.color;
			}

			set
			{
				if (this.color != value)
				{
					this.color = value;

					this.UpdateColor();
				}
			}
		}

		#endregion

        #region IsChecked

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsChecked
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

		#endregion

		#region TabItem

        /// <summary>
        /// Gets the tab item.
        /// </summary>
        /// <value>The tab item.</value>
		internal TabButton TabItem
		{
			get
			{
				if (this.tabItem == null)
				{
					this.tabItem = new TabButton()
					{
						Caption = this.Caption,
						Style = this.TabButtonStyle,
						IsSelected = this.IsChecked
					};

					this.tabItem.IsSelectedChanged += new EventHandler(this.OnTabItemIsSelectedChanged);
				}
                
				return this.tabItem;
			}
            set { }
		}

		#endregion

		#region TabButtonStyle

        /// <summary>
        /// Gets or sets the tab item style.
        /// </summary>
        /// <value>The tab item style.</value>
		public Style TabButtonStyle
		{
            get { return (Style)GetValue(TabButtonStyleProperty); }
            set { SetValue(TabButtonStyleProperty, value); }
		}

        /// <summary>
        /// Identifies the TabItemStyle Dependency Property.
        /// </summary>
        public static readonly DependencyProperty TabButtonStyleProperty = DependencyProperty.Register(
			"TabButtonStyle", 
			typeof(Style), 
			typeof(RibbonTab), 
			new PropertyMetadata(new PropertyChangedCallback(TabItemStyleChangedCallback)));

        /// <summary>
        /// Tabs the item style changed callback.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void TabItemStyleChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((RibbonTab)d).OnTabItemStyleChanged();
		}

		#endregion

        /// <summary>
        /// Gets or sets a value indicating whether this instance is wrapper.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is wrapper; otherwise, <c>false</c>.
        /// </value>
		internal bool IsWrapper
		{
			get;
			set;
		}

        /// <summary>
        /// Gets or sets a value indicating whether this instance is collapsed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is collapsed; otherwise, <c>false</c>.
        /// </value>
        internal bool IsCollapsed
        {
            get;
            set;
        }
		#endregion

		#region Methods

		/// <summary>
		/// Selects the tab
		/// </summary>
		public void Select()
		{
			this.IsChecked = true;
		}

		#endregion

		#region Overrides

        internal Border Part_Background;

		/// <summary>
		/// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
		/// </summary>
		public override void OnApplyTemplate()
        {
            //base.OnApplyTemplate();
            this.OnTextChanged();
            this.originalBackground = this.Background;
            this.Part_Background = this.GetTemplateChild("Part_Background") as Border;
            if (Part_Background != null && Part_Background.Background != null)
                RibbonBarBackground = Part_Background.Background;
		}

        internal static Brush RibbonBarBackground = new SolidColorBrush(Colors.Transparent);

		/// <summary>
		/// Prepares the specified element to display the specified item.
		/// </summary>
		/// <param name="element">The element used to display the specified item.</param>
		/// <param name="item">The item to display.</param>
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			RibbonItemsControl ric = element as RibbonItemsControl;

			if (ric != null)
			{
				ric.ItemClicked += new System.EventHandler(this.OnItemClicked);

				RibbonBar rb = ric as RibbonBar;

				if (rb != null)
				{
                    rb.LauncherClick += new RoutedEventHandler(rb_LauncherClick);
				}
			}

			base.PrepareContainerForItemOverride(element, item);
		}

        /// <summary>
        /// Called when [item clicked].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnItemClicked(object sender, EventArgs e)
        {
            if (this.ItemClicked != null)
            {
                this.ItemClicked(this, e);
            }
        }

        void rb_LauncherClick(object sender, RoutedEventArgs e)
        {
            if (this.ItemClicked != null)
            {
                this.ItemClicked(this, new EventArgs());
            }
        }

		/// <summary>
		/// Undoes the effects of the <see cref="M:System.Windows.Controls.ItemsControl.PrepareContainerForItemOverride(System.Windows.DependencyObject,System.Object)"/> method.
		/// </summary>
		/// <param name="element">The container element.</param>
		/// <param name="item">The target item.</param>
		protected override void ClearContainerForItemOverride(DependencyObject element, object item)
		{
			RibbonItemsControl ric = element as RibbonItemsControl;

			if (ric != null)
			{
				ric.ItemClicked -= new System.EventHandler(this.OnItemClicked);

				RibbonBar rb = ric as RibbonBar;

				if (rb != null)
				{
					rb.LauncherClick -= new RoutedEventHandler(rb_LauncherClick);
				}
			}

			base.ClearContainerForItemOverride(element, item);
		}

        /// <summary>
        /// Called when [text changed].
        /// </summary>
		internal virtual void OnTextChanged()
		{
			if (this.tabItem != null)
			{
				this.tabItem.Caption = this.Caption;
			}
		}

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler OnRibbonTabClick = delegate { };

        /// <summary>
        /// Called when [is selected changed].
        /// </summary>
		internal virtual void OnIsSelectedChanged()
		{
			this.Visibility = this.IsChecked ? Visibility.Visible : Visibility.Collapsed;

			if (this.tabItem != null)
			{
				this.tabItem.IsSelected = this.IsChecked;
			}
            if (this.IsChecked)
            {
                OnRibbonTabClick(this, EventArgs.Empty);
            }

			if (this.IsSelectedChanged != null)
			{
				this.IsSelectedChanged(this, EventArgs.Empty);
			}
		}

        /// <summary>
        /// Called when [tab item style changed].
        /// </summary>
        internal virtual void OnTabItemStyleChanged()
        {
            if (this.tabItem != null)
            {
                if (this.Parent != null && (this.Parent.GetType().ToString() == "Syncfusion.Windows.Tools.Controls.Ribbon" || this.Parent.GetType().ToString() == "Syncfusion.Windows.Tools.Controls.RichTextRibbon"))
                if (((Ribbon)this.Parent).RibbonState != Controls.RibbonState.Hide)
                    this.tabItem.Style = this.TabButtonStyle;
            }
        }

		#endregion

		#region Events

		internal event EventHandler ItemClicked;

		internal event EventHandler IsSelectedChanged;

		#endregion

		#region Event handlers


        /// <summary>
        /// Called when [tab item is selected changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnTabItemIsSelectedChanged(object sender, EventArgs e)
		{
			TabButton tabItem = sender as TabButton;
			this.IsChecked = tabItem.IsSelected;
		}

		#endregion

		#region Implementation

        /// <summary>
        /// Updates the color.
        /// </summary>
		private void UpdateColor()
		{
			Color clr = this.Color;

            //this.Background = this.GetTabBackground(clr);

			TabButton tabItem = this.TabItem;
			
			if (tabItem != null)
			{
				tabItem.SelectedBrush = this.GetSelectedTabItemBackground(clr);
				tabItem.HighlightedBrush = this.GetHighlightedTabItemBackground(clr);
			}
		}

        /// <summary>
        /// Gets the tab background.
        /// </summary>
        /// <param name="clr">The CLR.</param>
        /// <returns></returns>
		private Brush GetTabBackground(Color clr)
		{
			if (clr.A > 0)
			{
				LinearGradientBrush brush = new LinearGradientBrush() { EndPoint = new Point(0, 1), ColorInterpolationMode = ColorInterpolationMode.ScRgbLinearInterpolation };

                brush.GradientStops.Add(new GradientStop() { Offset = 0.0, Color = GetAlphaBlendedColor(clr, WHITE, 0x40) });
                brush.GradientStops.Add(new GradientStop() { Offset = 0.2, Color = GetAlphaBlendedColor(clr, WHITE, 0x10) });

				return brush;
			}

			return this.originalBackground;
		}

        /// <summary>
        /// Gets the selected tab item background.
        /// </summary>
        /// <param name="clr">The CLR.</param>
        /// <returns></returns>
		private Brush GetSelectedTabItemBackground(Color clr)
		{
			LinearGradientBrush brush = null;

			if (clr.A > 0)
			{
				brush = new LinearGradientBrush() { EndPoint = new Point(0, 1), ColorInterpolationMode = ColorInterpolationMode.ScRgbLinearInterpolation };

				brush.GradientStops.Add(new GradientStop() { Offset = 0.0, Color = clr });
				brush.GradientStops.Add(new GradientStop() { Offset = 1.0, Color = GetAlphaBlendedColor(clr, WHITE, 0x40) });
			}

			return brush;
		}

        /// <summary>
        /// Gets the highlighted tab item background.
        /// </summary>
        /// <param name="clr">The CLR.</param>
        /// <returns></returns>
		private Brush GetHighlightedTabItemBackground(Color clr)
		{
			LinearGradientBrush brush = null;

			if (clr.A > 0)
			{
				brush = new LinearGradientBrush() { EndPoint = new Point(0, 1), ColorInterpolationMode = ColorInterpolationMode.ScRgbLinearInterpolation };

				brush.GradientStops.Add(new GradientStop() { Offset = 0.1, Color = TRANSPARENT });
				brush.GradientStops.Add(new GradientStop() { Offset = 0.9, Color = GetAlphaBlendedColor(clr, WHITE, 0x80) });
			}

			return brush;
		}

        /// <summary>
        /// Gets the color of the alpha blended.
        /// </summary>
        /// <param name="src">The SRC.</param>
        /// <param name="dest">The dest.</param>
        /// <param name="alpha">The alpha.</param>
        /// <returns></returns>
		private static Color GetAlphaBlendedColor(Color src, Color dest, byte alpha)
		{
			byte r = (byte)(((src.R * alpha) + ((0xff - alpha) * dest.R)) / 0xff);
			byte g = (byte)(((src.G * alpha) + ((0xff - alpha) * dest.G)) / 0xff);
			byte b = (byte)(((src.B * alpha) + ((0xff - alpha) * dest.B)) / 0xff);
			byte a = (byte)(((src.A * alpha) + ((0xff - alpha) * dest.A)) / 0xff);

			return Color.FromArgb(a, r, g, b);
		}

		#endregion

		#region Fields

		private TabButton tabItem;

		private Brush originalBackground;

		private Color color = TRANSPARENT;

		private bool isSelected = false;

		private static readonly Color WHITE = Color.FromArgb(255, 255, 255, 255);
		private static readonly Color TRANSPARENT = Color.FromArgb(0, 0, 0, 0);

		#endregion
	}
}
