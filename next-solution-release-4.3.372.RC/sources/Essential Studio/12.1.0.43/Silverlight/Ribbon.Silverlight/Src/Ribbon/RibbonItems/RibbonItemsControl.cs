#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls 
{
	/// <summary>
    /// Represents generic container control for <see cref="RibbonItemsControl"/>s.
	/// </summary>
	public class RibbonItemsControl :
		ItemsControl,
		IRibbonSelector
	{
		#region Constructor

		/// <summary>
		/// Initialize a new instance of <see cref="RibbonItemsControl"/>
		/// </summary>
		public RibbonItemsControl()
		{
            this.Loaded += new RoutedEventHandler(RibbonItemsControl_Loaded);
		}

        /// <summary>
        /// 
        /// </summary>
        public event NotifyCollectionChangedEventHandler ItemsChanged;

	    /// <summary>
	    /// Called when the value of the <see cref="P:System.Windows.Controls.ItemsControl.Items"/> property changes.
	    /// </summary>
	    /// <param name="e">A <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> that contains the event data</param>
	    protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (ItemsChanged != null)
                ItemsChanged(this, e);
            base.OnItemsChanged(e);
        }

        void RibbonItemsControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.downCount = 0;
            if (this.Items.Count > 0)
            {
                foreach (var item in this.Items)
                {
                    if (item is SimpleMenuButton)
                    {
                        ((SimpleMenuButton)item).IsMouseOver = true ; ((SimpleMenuButton)item).IsMouseOver = false;
                    }
                    else if (item is SplitMenuButton)
                    {
                        ((SplitMenuButton)item).IsMouseOver = true ; ((SplitMenuButton)item).IsMouseOver = false;
                    }
                }
            }
        }

		#endregion

		#region Properties

        internal int downCount;

		#region RibbonItemTemplates

		/// <summary>
		/// Gets or sets the ribbon item templates.
		/// </summary>
		/// <value>The ribbon item templates.</value>
		public RibbonTemplatesCollection RibbonItemTemplates
		{
			get { return (RibbonTemplatesCollection)GetValue(RibbonItemTemplatesProperty); }
			set { SetValue(RibbonItemTemplatesProperty, value); }
		}

		/// <summary>
		/// Using a DependencyProperty as the backing store for ItemTemplates.  This enables animation, styling, binding, etc...
		/// </summary>
		public static readonly DependencyProperty RibbonItemTemplatesProperty = DependencyProperty.Register("RibbonItemTemplates", typeof(RibbonTemplatesCollection), typeof(RibbonItemsControl), null);

        /// <summary>
        /// Gets or sets the item container style.
        /// </summary>
        /// <value>The item container style.</value>
        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemContainerStyle.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the ItemContainerStyle Dependency Property. This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(RibbonItemsControl), new PropertyMetadata(null));

		#endregion

		#endregion

		#region Overrides

		/// <summary>
		/// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
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
			if (item is UIElement)
            {
                return true;
            }
            else
            {
                return false;
            }
		}

		/// <summary>
		/// Prepares the specified element to display the specified item.
		/// </summary>
		/// <param name="element">The element used to display the specified item.</param>
		/// <param name="item">The item to display.</param>
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
            if (!object.ReferenceEquals(element, item))
            {
                if (element is IRibbonItem)
                {
                    var ritem = element as IRibbonItem ;
                    if (ritem == null)
                    {
                        //ritem = new RibbonItem();
                        ritem = new RibbonButton();
                        ((RibbonButton)ritem).ContentTemplate = this.ItemTemplate;
                        element = (RibbonButton)ritem;
                    }

                    //if (ritem != null)
                    //{
                    //    //ritem.Label = item.ToString();
                    //    ritem.Selector = this;
                    //}

                    //if (this.RibbonItemTemplates != null)
                    //{
                    //    Type type = ritem.GetType();

                    //    foreach (ControlTemplate ct in this.RibbonItemTemplates)
                    //    {
                    //        if (ct != null)
                    //        {
                    //            if (ct.TargetType == type)
                    //            {
                    //                ritem.Template = ct;
                    //                break;
                    //            }
                    //        }
                    //    }
                    //}


                    //RibbonDropDownButton rddi = element as RibbonDropDownButton;

                    //if (rddi != null)
                    //{
                    //    rddi.QueryDropDownBounds += new EventHandler<BoundsEventArgs>(this.QueryDropDownBoundsHandler);
                    //}
                }
            }
            else
            {
                RibbonItemBase ri = item as RibbonItemBase;

                if (ri != null)
                {
                    ri.Selector = this;

                    if (this.RibbonItemTemplates != null)
                    {
                        Type type = ri.GetType();

                        foreach (ControlTemplate ct in this.RibbonItemTemplates)
                        {
                            if (ct.TargetType == type)
                            {
                                ri.Template = ct;
                                break;
                            }
                        }
                    }

                }
            }

            ButtonPanel rig = element as ButtonPanel;

            if (rig != null)
            {
                rig.Selector = this;
            }

            RibbonGallery rgi = element as RibbonGallery;

            if (rgi != null)
            {
                rgi.Selector = this;
            }
		}

		/// <summary>
		/// Undoes the effects of the <see cref="M:System.Windows.Controls.ItemsControl.PrepareContainerForItemOverride(System.Windows.DependencyObject,System.Object)"/> method.
		/// </summary>
		/// <param name="element">The container element.</param>
		/// <param name="item">The item to clear.</param>
		protected override void ClearContainerForItemOverride(DependencyObject element, object item)
		{
			base.ClearContainerForItemOverride(element, item);

			RibbonItemBase ri = item as RibbonItemBase;

			if (ri != null)
			{
				ri.Selector = null;

				RibbonDropDownItem rddi = ri as RibbonDropDownItem;

				if (rddi != null)
				{
					rddi.QueryDropDownBounds -= new EventHandler<BoundsEventArgs>(this.QueryDropDownBoundsHandler);
				}
			}
		}

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            RibbonButton item = new RibbonButton();
            //item.Selector = this;
            if (ItemContainerStyle != null)
            {
                item.Style = this.ItemContainerStyle;
            }

            return item;

        }

        /// <summary>
        /// Called when [item selected].
        /// </summary>
        /// <param name="item">The item.</param>
		internal virtual void OnItemSelected(UIElement item)
		{
			if (this.ItemSelected != null)
			{
				this.ItemSelected(item, EventArgs.Empty);
			}
		}

        /// <summary>
        /// Called when [item clicked].
        /// </summary>
        /// <param name="item">The item.</param>
		internal virtual void OnItemClicked(UIElement item)
		{
			if (this.ItemClicked != null)
			{
				this.ItemClicked(item, EventArgs.Empty);
			}
		}

        /// <summary>
        /// Called when [query drop down bounds].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Syncfusion.Windows.Tools.Controls.BoundsEventArgs"/> instance containing the event data.</param>
		internal virtual void OnQueryDropDownBounds(object sender, BoundsEventArgs args)
		{
			if (this.QueryDropDownBounds != null)
			{
				this.QueryDropDownBounds(sender, args);
			}
		}

		#endregion

		#region Events

        /// <summary>
        /// Occurs when [item selected].
        /// </summary>
		internal event EventHandler ItemSelected;

        /// <summary>
        /// Occurs when [item clicked].
        /// </summary>
		internal event EventHandler ItemClicked;

        /// <summary>
        /// Occurs when [Query Drop Down Bound].
        /// </summary>
		internal EventHandler<BoundsEventArgs> QueryDropDownBounds;

		#endregion

		#region Event handlers

        /// <summary>
        /// Queries the drop down bounds handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Syncfusion.Windows.Tools.Controls.BoundsEventArgs"/> instance containing the event data.</param>
		private void QueryDropDownBoundsHandler(object sender, BoundsEventArgs args)
		{
			this.OnQueryDropDownBounds(sender, args);
		}

		#endregion

		#region IRibbonSelector Members

        /// <summary>
        /// Called when [item selected].
        /// </summary>
        /// <param name="item">The item.</param>
		void IRibbonSelector.OnItemSelected(UIElement item)
		{
			this.OnItemSelected(item);
		}

        /// <summary>
        /// Called when [item clicked].
        /// </summary>
        /// <param name="item">The item.</param>
		void IRibbonSelector.OnItemClicked(UIElement item)
		{
			this.OnItemClicked(item);
		}

		#endregion
	}
}