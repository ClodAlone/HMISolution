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

namespace Syncfusion.Windows.Tools.Controls 
{
	/// <summary>
	/// This class represents ribbon's tab strip.
	/// </summary>
	public class RibbonTabStrip :
		ScrollStrip,
		IRibbonTabItemSelector
	{
		#region Fields

		private IRibbonTabItemSelector selector;
		private bool showSelectedTab = true;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RibbonTabStrip"/> class.
		/// </summary>
		public RibbonTabStrip()
		{
			this.DefaultStyleKey = typeof(RibbonTabStrip);
		}

		#endregion

		#region Properties

        /// <summary>
        /// Gets or sets the selector.
        /// </summary>
        /// <value>The selector.</value>
		internal IRibbonTabItemSelector Selector
		{
			get
			{
				return this.selector;
			}

			set
			{
				this.selector = value;
			}
		}

        /// <summary>
        /// Gets or sets a value indicating whether [show selected tab].
        /// </summary>
        /// <value><c>true</c> if [show selected tab]; otherwise, <c>false</c>.</value>
		internal bool ShowSelectedTab
		{
			get
			{
				return this.showSelectedTab;
			}

			set
			{
				if (this.showSelectedTab != value)
				{
					this.showSelectedTab = value;

					for (int i = 0; i < this.Items.Count; i++)
					{
						TabButton tab = this.Items[i] as TabButton;

						if (tab != null && tab.IsSelected)
						{
							tab.UpdateVisualState();
						}
					}
				}
			}
		}

		#endregion

		#region Overrides

        /// <summary>
        /// Provides the behavior for the "Arrange" pass of Silverlight layout. Classes can override this method to define their own arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
		protected override Size ArrangeOverride(Size finalSize)
		{
			Size size = base.ArrangeOverride(finalSize);

			if (this.Arranged != null)
			{
				this.Arranged(this, EventArgs.Empty);
			}

			return size;
		}
		
		/// <summary>
		/// Prepares the specified element to display the specified item.
		/// </summary>
		/// <param name="element">The element used to display the specified item.</param>
		/// <param name="item">The item to display.</param>
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);

			TabButton rti = element as TabButton;
			
			if (rti != null)
			{
				rti.Selector = this;

				if (item != rti && item != null)
				{
					rti.Caption = item.ToString();
				}
			}
		}

		/// <summary>
		/// Undoes the effects of the <see cref="M:System.Windows.Controls.ItemsControl.PrepareContainerForItemOverride(System.Windows.DependencyObject,System.Object)"/> method.
		/// </summary>
		/// <param name="element">The container element.</param>
		/// <param name="item">The target item.</param>
		protected override void ClearContainerForItemOverride(DependencyObject element, object item)
		{
			base.ClearContainerForItemOverride(element, item);

			TabButton rti = element as TabButton;

			if (rti != null)
			{
				rti.Selector = null;
			}
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
			return item is TabButton;
		}

		/// <summary>
		/// Creates or identifies the element that is used to display the given item.
		/// </summary>
		/// <returns>
		/// The element that is used to display the given item.
		/// </returns>
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new TabButton();
		}

		#endregion

		#region Events

		/// <summary>
		/// Occurs when the arranging of the <see cref="RibbonTabStrip"/> is complete.
		/// </summary>
		public event EventHandler Arranged;

		#endregion

		#region Implementation

        /// <summary>
        /// Gets the item at.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
		internal TabButton GetItemAt(MouseEventArgs e)
		{
			Point pt = e.GetPosition(this);

			if (pt.X > 0 && pt.Y > 0 && pt.X < this.ActualWidth && pt.Y < this.ActualHeight)
			{
				for (int i = 0; i < this.Items.Count; i++)
				{
					TabButton item = this.Items[i] as TabButton;

					if (item != null && item.Visibility == Visibility.Visible)
					{
						pt = e.GetPosition(item);

						if (pt.X > 0 && pt.Y > 0 && pt.X < item.ActualWidth && pt.Y < item.ActualHeight)
						{
							return item;
						}
					}
				}
			}

			return null;
		}

		#endregion

		#region IRibbonTabItemSelector implementation

        /// <summary>
        /// Called when [double click].
        /// </summary>
        /// <param name="item">The item.</param>
		void IRibbonTabItemSelector.OnDoubleClick(TabButton item)
		{
			if (this.selector != null)
			{
				this.selector.OnDoubleClick(item);
			}
		}

        /// <summary>
        /// Called when [mouse down].
        /// </summary>
        /// <param name="item">The item.</param>
		void IRibbonTabItemSelector.OnMouseDown(TabButton item)
		{
			if (this.selector != null)
			{
				this.selector.OnMouseDown(item);
			}
		}

		#endregion
	}
}
