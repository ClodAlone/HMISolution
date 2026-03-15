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
using System.Windows.Input;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the NavigationToolbarItem UI element.
    /// </summary>
    public class NavigationToolbarItem : Control
    {
        #region Private members
        private GroupBarItem groupBarItem;
        private bool isPressed = false;
        private Image imageElement;        
        #endregion

        internal ContentControl headerContent=null;

        #region Properties
        /// <summary>
        /// Gets presented GroupBarItem.
        /// </summary>
        public GroupBarItem GroupBarItem
        {
            get
            {
                return this.groupBarItem;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is pressed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is pressed; otherwise, <c>false</c>.
        /// </value>
        public bool IsPressed
        {
            get
            {
                return this.isPressed;
            }

            set
            {
                this.isPressed = value;
            }
        }

        #endregion

        #region Dependency properties
        ///// <summary>
        ///// Identifies <see cref="ThemeProperty">Theme</see>
        ///// dependency Property
        ///// </summary>
      
        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="NavigationToolbarItem"/>
        /// </summary>
        public NavigationToolbarItem()
        {
            this.DefaultStyleKey = typeof(NavigationToolbarItem);
        }

        /// <summary>
        /// Initializes new instance of the NavigationToolbarItem class.
        /// </summary>
        /// <param name="item">Item to present.</param>
        public NavigationToolbarItem(GroupBarItem item)
        {
            this.DefaultStyleKey = typeof(NavigationToolbarItem);

            if (item != null)
            {
                this.groupBarItem = item;
            }
        }

        #endregion

        #region Overrides
        /// <summary>
        /// Builds the visual tree for the NavigationToolbarItem when a new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.imageElement = GetTemplateChild("ToolBarItemImage") as Image;
            this.imageElement.Source = this.groupBarItem.HeaderImageSource;
            this.DataContext = this.groupBarItem.DataContext;
            if (this.GroupBarItem.IsPressed)
            {
                VisualStateManager.GoToState(this, "Select", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Unselect", true);
            }
        }

        /// <summary>
        /// Invoked when the mouse left button clicked on the item.
        /// </summary>
        /// <param name="e">Contains information about the cursor position</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            foreach (var gbItem in this.GroupBarItem._groupBar.Items)
            {
                GroupBarItem item = null;

                //if (!(_groupBar.VisualMode == VisualMode.StackMode) && _groupBar.nonStackGridElement != null)
                //{
                //    item = _groupBar.nonStackGridElement.ItemContainerGenerator.ContainerFromItem(gbItem) as GroupBarItem;
                //}

                item = GroupBarItem._groupBar.stackGridElement.ItemContainerGenerator.ContainerFromItem(gbItem) as GroupBarItem;               


                if (item != null && item != this.GroupBarItem)
                {
                    item.IsPressed = false;
                    VisualStateManager.GoToState(item, "Collapsed", true);
                }
            }

            this.IsPressed = true;
            this.GroupBarItem.SetExpanded(groupBarItem.LogicalParent);
            this.GroupBarItem.IsPressed = true;
            this.GroupBarItem.IsSelected = true;
            if (this.GroupBarItem._groupBar != null)
            {
                this.GroupBarItem._groupBar.SelectedItem = this.GroupBarItem;
            }
            VisualStateManager.GoToState(this.GroupBarItem, "Expanded", true);

            if (this.GroupBarItem.IsPressed)
            {
                VisualStateManager.GoToState(this, "Select", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Unselect", true);
            }

            StackPanel parent = (StackPanel)this.Parent;

            for (int i = 0; i < parent.Children.Count; i++)
            {
                if ((parent.Children[i] as NavigationToolbarItem) != this)
                {
                    VisualStateManager.GoToState(parent.Children[i] as NavigationToolbarItem, "Unselect", true);
                }
                else
                {
                    VisualStateManager.GoToState(this, "Select", true);
                }
            }
        }

        /// <summary>
        /// Invoked when the mouse enters the item.
        /// </summary>
        /// <param name="e">Contains information about the cursor position</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (!this.GroupBarItem.IsPressed)
            {
                VisualStateManager.GoToState(this, "MouseOver", true);
            }
        }

        /// <summary>
        /// Invoked when mouse leaves the item.
        /// </summary>
        /// <param name="e">Contains information about the cursor position</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (!this.GroupBarItem.IsPressed)
            {
                VisualStateManager.GoToState(this, "MouseOut", true);
            }
        }

        #endregion
    }
}
