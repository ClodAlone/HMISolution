#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the tab item's border.
    /// </summary>
    public class TabItemAdvBorder : ContentControl
    {
        #region Initialization
        /// <summary>
        /// Initializes new instance of the TabItemAdvBorder class.
        /// </summary>
        public TabItemAdvBorder()
        {
            this.Loaded += new RoutedEventHandler(this.TabItemAdvBorderLoaded);
        }
        #endregion

        #region DP getters and setters
        /// <summary>
        /// Gets or sets the tabControl parent of the border.
        /// </summary>
        public TabControlAdv TabControlParent
        {
            get
            {
                return (TabControlAdv) GetValue(TabControlParentProperty);
            }

            set
            {
                SetValue(TabControlParentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the tabItem parent of the border.
        /// </summary>
        internal TabItemAdv TabParent
        {
            get
            {
                return (TabItemAdv)GetValue(TabParentProperty);
            }

            set
            {
                SetValue(TabParentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether border should be reversed.
        /// </summary>
        public bool IsReversed
        {
            get
            {
                return (bool)GetValue(IsReversedProperty);
            }

            set
            {
                SetValue(IsReversedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether border is used for drawing of the tab items.
        /// </summary>
        public bool IsTabItemBorder
        {
            get
            {
                return (bool)GetValue(IsTabItemBorderProperty);
            }

            set
            {
                SetValue(IsTabItemBorderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to rotate content when border is rotated.
        /// </summary>
        public bool RotateContent
        {
            get
            {
                return (bool)GetValue(RotateContentProperty);
            }

            set
            {
                SetValue(RotateContentProperty, value);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="TabControlParent"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback TabControlParentChanged;

        /// <summary>
        /// Event that is raised when <see cref="TabParent"/> property is changed.
        /// </summary>
        internal event PropertyChangedCallback TabParentChanged;

        /// <summary>
        /// Event that is raised when <see cref="IsReversed"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IsReversedChanged;

        /// <summary>
        /// Event that is raised when <see cref="IsTabItemBorder"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback IsTabItemBorderChanged;

        /// <summary>
        /// Event that is raised when <see cref="RotateContent"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback RotateContentChanged;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies the <see cref="TabControlParent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TabControlParentProperty =
            DependencyProperty.Register("TabControlParent", typeof(TabControlAdv), typeof(TabItemAdvBorder), new PropertyMetadata(OnTabControlParentChanged));

        /// <summary>
        /// Identifies the <see cref="TabParent"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty TabParentProperty =
            DependencyProperty.Register("TabParent", typeof(TabItemAdv), typeof(TabItemAdvBorder), new PropertyMetadata(new PropertyChangedCallback(OnTabParentChanged)));

        /// <summary>
        /// Identifies the <see cref="IsReversed"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsReversedProperty =
            DependencyProperty.Register("IsReversed", typeof(bool), typeof(TabItemAdvBorder), new PropertyMetadata(new PropertyChangedCallback(OnIsReversedChanged)));

        /// <summary>
        /// Identifies the <see cref="IsTabItemBorder"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTabItemBorderProperty =
            DependencyProperty.Register("IsTabItemBorder", typeof(bool), typeof(TabItemAdvBorder), new PropertyMetadata(new PropertyChangedCallback(OnIsTabItemBorderChanged)));
        
        /// <summary>
        /// Identifies the <see cref="RotateContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RotateContentProperty =
            DependencyProperty.Register("RotateContent", typeof(bool), typeof(TabItemAdvBorder), new PropertyMetadata(true, new PropertyChangedCallback(OnRotateContentChanged)));
        #endregion

        #region Overrides
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
            Size desiredSize = base.MeasureOverride(availableSize);
            if (this.Content is FrameworkElement)
            {
                FrameworkElement content = this.Content as FrameworkElement;
                double width = double.PositiveInfinity;
                if (this.TabControlParent != null && this.TabControlParent.TabItemSizeMode == TabItemSizeMode.ShrinkToFit)
                {
                    width = availableSize.Width;
                }

                content.Measure(new Size(width, availableSize.Height));
                desiredSize = content.DesiredSize;
            }

            if (this.TabControlParent != null && this.TabControlParent.RotateTextWhenVertical &&
                (this.TabControlParent.TabStripPlacement == TabStripPlacement.Left || this.TabControlParent.TabStripPlacement == TabStripPlacement.Right))
            {
                desiredSize = new Size(desiredSize.Height, desiredSize.Width);
            }

            return desiredSize;
        }

        /// <summary>
        /// Provides the behavior for the "Arrange" pass of Silverlight layout. Classes
        ///  can override this method to define their own arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself
        /// and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Size size = base.ArrangeOverride(finalSize);
            this.RefreshBorderPaths(finalSize);            

            if (this.Content is FrameworkElement)
            {
                FrameworkElement elem = this.Content as FrameworkElement;

                if (this.TabParent != null && this.TabControlParent != null)
                {
                    RotateTransform rotateTransform = new RotateTransform();
                    if (this.TabControlParent.TabStripPlacement == TabStripPlacement.Left || this.TabControlParent.TabStripPlacement == TabStripPlacement.Right)
                    {
                        if (this.TabControlParent.RotateTextWhenVertical)
                        {
                            if (this.TabControlParent.TabStripPlacement == TabStripPlacement.Left)
                            {
                                rotateTransform.Angle = 90d;
                            }
                            else if (this.TabControlParent.TabStripPlacement == TabStripPlacement.Right)
                            {
                                rotateTransform.Angle = -90d;
                            }

                            elem.Arrange(new Rect(0, 0, finalSize.Height, finalSize.Width));

                            TranslateTransform translateTransform = new TranslateTransform
                            {
                                X = (-finalSize.Height / 2) + (elem.ActualHeight / 2),
                                Y = (-finalSize.Width / 2) + (elem.ActualWidth / 2)
                            };

                            TransformGroup transformGroup = new TransformGroup();
                            transformGroup.Children.Add(rotateTransform);
                            transformGroup.Children.Add(translateTransform);
                            rotateTransform.CenterX = elem.ActualWidth / 2;
                            rotateTransform.CenterY = elem.ActualHeight / 2;
                            elem.RenderTransform = transformGroup;
                        }
                        else
                        {
                            elem.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
                            rotateTransform.Angle = 0;
                            elem.RenderTransform = rotateTransform;
                        }
                    }
                    else
                    {
                        elem.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
                        if (this.TabControlParent.TabStripPlacement == TabStripPlacement.Top)
                        {
                            rotateTransform.Angle = 0d;
                        }
                        else
                        {
                            rotateTransform.Angle = 180d;
                        }

                        rotateTransform.CenterX = elem.ActualWidth / 2;
                        rotateTransform.CenterY = elem.ActualHeight / 2;
                        elem.RenderTransform = rotateTransform;
                    }
                }
                else
                {
                    elem.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
                }
            }

            return size;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Updates visual state of the border.
        /// </summary>
        /// <param name="isOver">Indicates whether mouse is over the border.</param>
        /// <param name="isRightRotated">Indicates whether tab is rotated when tab is placed on the right.</param>
        /// <param name="isLeftRotated">Indicates whether tab is rotated when tab is placed on the left.</param>
        internal virtual void SetVisualState(bool isOver, bool isRightRotated, bool isLeftRotated)
        {
        }

       
        /// <summary>
        /// Occurs when a System.Windows.FrameworkElement has completed layout passes,
        /// has rendered, and is ready for interaction.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void TabItemAdvBorderLoaded(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Refreshes border paths.
        /// </summary>
        internal virtual void RefreshBorderPaths()
        {
            this.RefreshBorderPaths(new Size(this.ActualWidth, this.ActualHeight));
        }

        /// <summary>
        /// Refreshes border paths.
        /// </summary>
        /// <param name="borderSize">Size of the border.</param>
        protected virtual void RefreshBorderPaths(Size borderSize)
        {
        }

        /// <summary>
        /// Rotates the border according to the angle.
        /// </summary>
        /// <param name="angle">Angle to rotate the border.</param>
        internal virtual void RotateBorder(double angle)
        {
            if (this.Content is FrameworkElement && !this.RotateContent)
            {
                RotateTransform rotate1 = new RotateTransform();
                rotate1.Angle = -angle;
                rotate1.CenterX = (this.Content as FrameworkElement).ActualWidth / 2;
                rotate1.CenterY = (this.Content as FrameworkElement).ActualHeight / 2;
                (this.Content as FrameworkElement).RenderTransform = rotate1;
            }

            RotateTransform rotate2 = new RotateTransform();
            rotate2.Angle = angle;
            rotate2.CenterX = this.ActualWidth / 2;
            rotate2.CenterY = this.ActualHeight / 2;
            this.RenderTransform = rotate2;
        }

        /// <summary>
        /// Calls OnTabParentChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTabParentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemAdvBorder instance = (TabItemAdvBorder)d;
            instance.OnTabParentChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises TabParentChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnTabParentChanged(DependencyPropertyChangedEventArgs e)
        {
            this.InvalidateArrange();
            if (this.TabParentChanged != null)
            {
                this.TabParentChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnTabControlParentChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTabControlParentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemAdvBorder instance = (TabItemAdvBorder)d;
            instance.OnTabControlParentChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises TabControlParentChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnTabControlParentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.TabControlParentChanged != null)
            {
                this.TabControlParentChanged(this, e);
            }
        }        

        /// <summary>
        /// Calls OnIsReversedChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsReversedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemAdvBorder instance = (TabItemAdvBorder)d;
            instance.OnIsReversedChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises IsReversedChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnIsReversedChanged(DependencyPropertyChangedEventArgs e)
        {
            this.InvalidateArrange();
            if (this.IsReversedChanged != null)
            {
                this.IsReversedChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnIsTabItemBorderChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsTabItemBorderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemAdvBorder instance = (TabItemAdvBorder)d;
            instance.OnIsTabItemBorderChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises IsTabItemBorderChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnIsTabItemBorderChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsTabItemBorderChanged != null)
            {
                this.IsTabItemBorderChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnRotateContentChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnRotateContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItemAdvBorder instance = (TabItemAdvBorder)d;
            instance.OnRotateContentChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises RotateContentChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnRotateContentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.RotateContentChanged != null)
            {
                this.RotateContentChanged(this, e);
            }
        }
        #endregion
    }
}
