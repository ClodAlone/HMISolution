#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.Windows;

namespace Syncfusion.Windows.Tools.Controls 
{
    /// <summary>
    /// Represents the RibbonItemsGroup Class.
    /// </summary>
    public class RibbonItemsGroup : RibbonItemsControl, IRibbonControl
	{
		#region Constructor

		/// <summary>
		/// Initialize a new instance of <see cref="RibbonItemsGroup"/>
		/// </summary>
		public RibbonItemsGroup()
		{
			this.DefaultStyleKey = typeof(RibbonItemsGroup);
			this.IsTabStop = false;
		}

		#endregion

		#region Overrides

        protected override void OnMouseRightButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            FrameworkElement realSource = RibbonContextMenu.GetRealSource(this, e);
            if (realSource == null) return;
            var parentRibbon = VisualUtils.FindAncestor(this, typeof(Ribbon));
            var parentRibbonBar = VisualUtils.FindAncestor(this, typeof(RibbonBar));
            if (parentRibbonBar != null)
            {
                ((RibbonBar)parentRibbonBar).IsRightClickNeeded = true;
            }
            if (parentRibbon != null && parentRibbon is Ribbon && realSource != null)
            {
                ((Ribbon)parentRibbon).AddQATinContextMenu(realSource);
            }
            base.OnMouseRightButtonDown(e);
        }

        /// <summary>
        /// Called when [item clicked].
        /// </summary>
        /// <param name="item">The item.</param>
		internal override void OnItemClicked(UIElement item)
		{
			base.OnItemClicked(item);

			if (this.Selector != null)
			{
				this.Selector.OnItemClicked(item);
			}
		}

        /// <summary>
        /// Called when [item selected].
        /// </summary>
        /// <param name="item">The item.</param>
		internal override void OnItemSelected(UIElement item)
		{
			base.OnItemSelected(item);

			if (this.Selector != null)
			{
				this.Selector.OnItemSelected(item);
			}
		}

		#endregion

		#region Properties

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

		#region Fields

		private IRibbonSelector selector = null;

		#endregion
	}
}
