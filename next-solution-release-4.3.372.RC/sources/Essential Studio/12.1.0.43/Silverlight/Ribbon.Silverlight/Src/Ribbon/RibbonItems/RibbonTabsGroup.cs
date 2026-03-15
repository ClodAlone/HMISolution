#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Syncfusion.Windows.Tools.Controls 
{
    /// <summary>
    /// Represents the Ribbon Tab Group Class.
    /// </summary>
	public class RibbonTabsGroup : ItemsControl
	{
		#region Constructor
		
		/// <summary>
		/// Initialize a new instance of <see cref="RibbonTabsGroup"/>
		/// </summary>
		public RibbonTabsGroup()
		{
			this.DefaultStyleKey = typeof(RibbonTabsGroup);

			this.tabs = new RibbonTabCollection();
			this.tabs.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnTabsChanged);
		}

		#endregion

		#region Properties

		#region Color

		/// <summary>
		/// Gets or sets the value of group's basic color
		/// </summary>
		public Color Color
		{
			get { return (Color)GetValue(GroupColorProperty); }
			set { SetValue(GroupColorProperty, value); }
		}

		
		/// <summary>
        /// Using a DependencyProperty as the backing store for GroupColor.  This enables animation, styling, binding, etc...
		/// </summary>
		public static readonly DependencyProperty GroupColorProperty = DependencyProperty.Register(
			"Color", 
			typeof(Color), 
			typeof(RibbonTabsGroup),
			new PropertyMetadata(new Color(), new PropertyChangedCallback(GroupColorChangedCallback)));

        /// <summary>
        /// Groups the color changed callback.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void GroupColorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((RibbonTabsGroup)d).OnColorChanged();
		}

		#endregion

		#region Caption

		/// <summary>
		/// Gets or sets the value of group's text
		/// </summary>
		public string Caption
		{
			get { return (string)GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}

		
		/// <summary>
        /// Using a DependencyProperty as the backing store for Caption.  This enables animation, styling, binding, etc...
		/// </summary>
		public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register(
			"Caption", 
			typeof(string), 
			typeof(RibbonTabsGroup), 
			new PropertyMetadata(CaptionChangedCallback));

        /// <summary>
        /// Captions the changed callback.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
		private static void CaptionChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((RibbonTabsGroup)d).OnCaptionChanged();
		}

		#endregion

		#region TabsGroupItem

        /// <summary>
        /// Gets or sets the tabs group item.
        /// </summary>
        /// <value>The tabs group item.</value>
		internal RibbonTabsGroupItem TabsGroupItem
		{
			get
			{
				return this.tabsGroupItem;
			}

			set
			{
				if (this.tabsGroupItem != value)
				{ 
					this.tabsGroupItem = value;
                    if (tabsGroupItem != null)
                    {
                        this.tabsGroupItem.UpdateBackground();
                        this.tabsGroupItem.UpdateCaption();
                    }
				}
			}
		}

		#endregion

		#region Tabs

        /// <summary>
        /// Gets the tabs.
        /// </summary>
        /// <value>The tabs.</value>
		internal RibbonTabCollection Tabs
		{
			get
			{
				return this.tabs;
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

			this.tabs.Clear();
		}

		/// <summary>
		/// Determines if the specified item is (or is eligible to be) its own container.
		/// </summary>
		/// <param name="item">The item to check.</param>
		/// <returns>
		/// true if the item is (or is eligible to be) its own container; otherwise, false.
		/// </returns>
		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is RibbonTab;
		}

		/// <summary>
		/// Creates or identifies the element that is used to display the given item.
		/// </summary>
		/// <returns>
		/// The element that is used to display the given item.
		/// </returns>
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new RibbonTab() { IsWrapper = true };
		}

		/// <summary>
		/// Prepares the specified element to display the specified item.
		/// </summary>
		/// <param name="element">The element used to display the specified item.</param>
		/// <param name="item">The item to display.</param>
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			RibbonTab tab = element as RibbonTab;
			if (tab != null)
			{
				if (!tab.Equals(item))
				{
					ContentPresenter itemPresenter = new ContentPresenter();
					
					itemPresenter.Content = item;

					tab.Items.Add(itemPresenter);
				}

				this.tabs.Add(tab);

				tab.Color = this.Color;
			}
		}

		/// <summary>
		/// Undoes the effects of the <see cref="M:System.Windows.Controls.ItemsControl.PrepareContainerForItemOverride(System.Windows.DependencyObject,System.Object)"/> method.
		/// </summary>
		/// <param name="element">The container element.</param>
		/// <param name="item">The <see cref="RibbonTabsGroup"/> item.</param>
		protected override void ClearContainerForItemOverride(DependencyObject element, object item)
		{
			base.ClearContainerForItemOverride(element, item);

			RibbonTab tab = element as RibbonTab;
			
			if (tab != null)
			{
				this.tabs.Remove(tab);
			
				tab.Color = new Color();
			}
		}
		
		#endregion

		#region Event handlers

        /// <summary>
        /// Called when [tabs changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		private void OnTabsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (this.TabsChanged != null)
			{
				this.TabsChanged(this, e);
			}
		}

		#endregion

		#region Implementation

        /// <summary>
        /// Called when [color changed].
        /// </summary>
		private void OnColorChanged()
		{
			Color color = this.Color;
			
			foreach (RibbonTab tab in this.tabs)
			{
				tab.Color = color;
			}

			if (this.tabsGroupItem != null)
			{
				this.tabsGroupItem.UpdateBackground();
			}
		}

        /// <summary>
        /// Called when [caption changed].
        /// </summary>
		private void OnCaptionChanged()
		{
			if (this.tabsGroupItem != null)
			{
				this.tabsGroupItem.UpdateCaption();
			}
		}

		#endregion

		#region Events

        /// <summary>
        /// Occurs when [tabs changed].
        /// </summary>
		internal event NotifyCollectionChangedEventHandler TabsChanged;
		
		#endregion

		#region Fields

		private RibbonTabCollection tabs;
		private RibbonTabsGroupItem tabsGroupItem;

		#endregion
	}
}
