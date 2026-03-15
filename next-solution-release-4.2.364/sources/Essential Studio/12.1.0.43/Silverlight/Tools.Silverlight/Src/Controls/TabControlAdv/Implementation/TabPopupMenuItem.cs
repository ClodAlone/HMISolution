#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Silverlight.Shared;
using System.Windows.Media.Imaging;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the tab popup menu item.
    /// </summary>
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    public class TabPopupMenuItem : Control
    {
        #region Class constants
        /// <summary>
        /// First column width.
        /// </summary>
        private const int FirstColumnWidth = 27;
        #endregion

        #region Class members
        /// <summary>
        /// Border used for drawing the item.
        /// </summary>
        private Border itemBorder;

        /// <summary>
        /// The parent.
        /// </summary>
        private TabPopupMenu menuParent;

        /// <summary>
        /// Whether it is custom popup menu item.
        /// </summary>
        private bool isCustomPopupMenuItem;

        #endregion

        #region Initialization
        /// <summary>
        /// Initializes new instance of the TabPopupMenuItem class.
        /// </summary>
        public TabPopupMenuItem()
        {
            this.DefaultStyleKey = typeof(TabPopupMenuItem);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the parent of the menuItem.
        /// </summary>
        public TabPopupMenu MenuParent
        {
            get
            {
                return menuParent;
            }

            set
            {
                menuParent = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is custom popup menu item.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is custom popup menu item; otherwise, <c>false</c>.
        /// </value>
        public bool IsCustomPopupMenuItem
        {

            get 
            {
                return this.isCustomPopupMenuItem;
            }
            set 
            {
                this.isCustomPopupMenuItem = value;
            }
        }

        #endregion

        #region DP getters and setters
        /// <summary>
        /// Gets or sets a value indicating whether the item is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }

            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the item is highLighted.
        /// </summary>
        public bool IsHighlighted
        {
            get
            {
                return (bool)GetValue(IsHighlightedProperty);
            }

            set
            {
                SetValue(IsHighlightedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text of the item.
        /// </summary>
        public  object HeaderText
        {
            get
            {
                return (object)GetValue(HeaderTextProperty);
            }

            set
            {
                SetValue(HeaderTextProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the header text template.
        /// </summary>
        /// <value>The header text template.</value>
        public  DataTemplate HeaderTextTemplate
        {
            get
            {
                return (DataTemplate)GetValue(HeaderTextTemplateProperty);
            }

            set
            {
                SetValue(HeaderTextTemplateProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the icon of the item.
        /// </summary>
        public ImageSource HeaderImage
        {
            get
            {
                return (ImageSource)GetValue(HeaderImageProperty);
            }

            set
            {
                SetValue(HeaderImageProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the id of the item.
        /// </summary>
        public int Id
        {
            get
            {
                return (int) GetValue(IdProperty);
            }

            set
            {
                SetValue(IdProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the parent id of the item.
        /// </summary>
        public int? ParentId
        {
            get
            {
                return (int?)GetValue(ParentIdProperty);
            }

            set
            {
                SetValue(ParentIdProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the pop up presenter.
        /// </summary>
        /// <value>The pop up presenter.</value>
        public ContentPresenter PopUpPresenter
        {
            get
            {
                return (ContentPresenter)GetValue(PopUpPresenterProperty);
            }

            set
            {
                SetValue(PopUpPresenterProperty, value);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="IsSelected"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IsSelectedChanged;

        /// <summary>
        /// Event that is raised when <see cref="IsHighlighted"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IsHighlightedChanged;
        #endregion

        #region Dependency properties

        /// <summary>
        /// Identifies the <see cref="PopUpPresenter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PopUpPresenterProperty =
         DependencyProperty.Register("PopUpPresenter", typeof(ContentPresenter), typeof(TabPopupMenuItem), new PropertyMetadata(null));


        /// <summary>
        /// Identifies the <see cref="IsSelected"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(TabPopupMenuItem), new PropertyMetadata(false, OnIsSelectedChanged));

        /// <summary>
        /// Identifies the <see cref="IsHighlighted"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsHighlightedProperty =
            DependencyProperty.Register("IsHighlighted", typeof(bool), typeof(TabPopupMenuItem), new PropertyMetadata(false, OnIsHighlightedChanged));

        /// <summary>
        /// Identifies the <see cref="HeaderText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register("HeaderText", typeof(object), typeof(TabPopupMenuItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="HeaderTextTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderTextTemplateProperty =
           DependencyProperty.Register("HeaderTextTemplate", typeof(DataTemplate), typeof(TabPopupMenuItem), new PropertyMetadata(null));


        /// <summary>
        /// Identifies the <see cref="HeaderImage"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderImageProperty =
            DependencyProperty.Register("HeaderImage", typeof(ImageSource), typeof(TabPopupMenuItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Id"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(int), typeof(TabPopupMenuItem), new PropertyMetadata(-1));

        /// <summary>
        /// Identifies the <see cref="ParentId"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ParentIdProperty =
            DependencyProperty.Register("ParentId", typeof(int?), typeof(TabPopupMenuItem), new PropertyMetadata(-1));
        #endregion

        #region Overrides
        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) 
        /// call System.Windows.Controls.Control.ApplyTemplate() method.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.itemBorder = this.GetTemplateChild("ItemBorder") as Border;

            this.PopUpPresenter = this.GetTemplateChild("PART_PopUpPresenter") as ContentPresenter;

            if (this.itemBorder != null)
            {
                this.itemBorder.MouseEnter += new MouseEventHandler(this.BorderMouseEnter);
                this.itemBorder.MouseLeave += new MouseEventHandler(this.BorderMouseLeave);
            }

        }

        /// <summary>
        /// Provides the behavior for the "measure" pass of Silverlight layout. Classes
        /// can override this method to define their own measure pass behavior.
        /// </summary>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity can
        /// be specified as a value to indicate that the object will size to whatever
        /// content is available.</param>
        /// <returns>The size that this object determines it needs during layout, based on its
        /// calculations of child object allotted sizes.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size desiredSize = Size.Empty;
            base.MeasureOverride(availableSize);

            if (itemBorder != null)
            {
                itemBorder.Measure(availableSize);
                desiredSize = itemBorder.DesiredSize;
            }

            return desiredSize;
        }

        /// <summary>
        ///  Called before the System.Windows.UIElement.MouseLeftButtonDown event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            CheckOtherItemsSelectedState();         
            this.IsSelected = true;
        }

        /// <summary>
        /// Called before the System.Windows.UIElement.MouseMove event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            this.IsHighlighted = true;
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Checks the state of the other items selected.
        /// </summary>
        private void CheckOtherItemsSelectedState()
        {
            if (!this.IsSelected)
            {
                foreach (TabPopupMenuItem menuItem in this.menuParent.MenuItems)
                {
                    if (menuItem != this)
                        menuItem.IsSelected = false;
                }
            }
        }

        /// <summary>
        /// Update visual state of the button.
        /// </summary>
        internal virtual void UpdateVisualState()
        {
            if (IsHighlighted)
            {
                VisualStateManager.GoToState(this, "MouseOver", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Normal", true);
            }
        }

        /// <summary>
        /// Occurs when mouse leaves the element.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void BorderMouseLeave(object sender, MouseEventArgs e)
        {
            this.IsHighlighted = false;
        }

        /// <summary>
        /// Occurs when mouse enters the element.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void BorderMouseEnter(object sender, MouseEventArgs e)
        {
            this.IsHighlighted = true;            
        }

        /// <summary>
        /// Calls OnIsHighlightedChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsHighlightedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabPopupMenuItem instance = (TabPopupMenuItem)d;
            instance.OnIsHighlightedChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises IsHighlightedChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnIsHighlightedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.MenuParent != null)
            {
                if (this.IsHighlighted)
                {
                    this.MenuParent.HighLightedMenuItem = this;
                    foreach (TabPopupMenuItem menuItem in this.menuParent.MenuItems)
                    {
                        if (menuItem != this)
                        {
                            menuItem.IsHighlighted = false;
                        }
                    }
                }                
            }

            this.UpdateVisualState();

            if (this.IsHighlightedChanged != null)
            {
                this.IsHighlightedChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnIsSelectedChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabPopupMenuItem instance = (TabPopupMenuItem)d;
            instance.OnIsSelectedChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises IsSelectedChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnIsSelectedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsSelectedChanged != null)
            {
                this.IsSelectedChanged(this, e);
            }
        }
        #endregion
    }
}
