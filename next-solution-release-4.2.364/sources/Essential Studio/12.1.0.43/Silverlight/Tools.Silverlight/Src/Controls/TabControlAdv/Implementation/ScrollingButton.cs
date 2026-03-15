#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents scrolling button class.
    /// </summary>
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
    public class ScrollingButton : Button
    {
        #region Class constants
        /// <summary>
        /// Default button width.
        /// </summary>
        private const int DefaultWidth = 14;

        /// <summary>
        /// Default button height.
        /// </summary>
        private const int DefaultHeight = 14;
        #endregion

        #region Private members
        /// <summary>
        /// Path used for drawing button's content.
        /// </summary>
        private Path path1;

        /// <summary>
        /// Path used for drawing button's content.
        /// </summary>
        private Path path2;

        /// <summary>
        /// Path used for drawing button's content.
        /// </summary>
        private Path path3;

        /// <summary>
        /// Path used for drawing button's content.
        /// </summary>
        private Path path4;

        /// <summary>
        /// Path used for drawing button's content.
        /// </summary>
        private Path path5;

        /// <summary>
        /// Path used for drawing button's content.
        /// </summary>
        private Path path6;

        /// <summary>
        /// Border used for drawing the button.
        /// </summary>
        private FrameworkElement buttonBorder;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes new instance of the ScrollingButton class.
        /// </summary>
        public ScrollingButton()
        {
            this.DefaultStyleKey = typeof(ScrollingButton);

            this.UpdateVisualState();
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(ScrollingButtonIsEnabledChanged);
        }
        #endregion

        #region DP getters and setters
        /// <summary>
        /// Gets or sets the scrolling direction.
        /// </summary>
        public ScrollDirection ScrollDirection
        {
            get
            {
                return (ScrollDirection) GetValue(ScrollDirectionProperty);
            }

            set
            {
                SetValue(ScrollDirectionProperty, value);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when <see cref="ScrollDirection"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ScrollDirectionChanged;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies the <see cref="ScrollDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ScrollDirectionProperty =
            DependencyProperty.Register("ScrollDirection", typeof(ScrollDirection), typeof(ScrollingButton), new PropertyMetadata(new PropertyChangedCallback(OnScrollDirectionChanged)));
        #endregion

        #region Overrides
        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) 
        /// call System.Windows.Controls.Control.ApplyTemplate() method.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.path1 = this.GetTemplateChild("PART_Path1") as Path;
            this.path2 = this.GetTemplateChild("PART_Path2") as Path;
            this.path3 = this.GetTemplateChild("PART_Path3") as Path;
            this.path4 = this.GetTemplateChild("PART_Path4") as Path;
            this.path5 = this.GetTemplateChild("PART_Path5") as Path;
            this.path6 = this.GetTemplateChild("PART_Path6") as Path;
            this.buttonBorder = this.GetTemplateChild("ButtonBorder") as FrameworkElement;
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
            base.MeasureOverride(availableSize);
            return new Size(DefaultWidth, DefaultHeight);
        }

        /// <summary>
        /// Occurs when the mouse enters an element.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            this.UpdateVisualState();
        }

        /// <summary>
        /// Occurs when the mouse leaves an element.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.UpdateVisualState();
        }

        /// <summary>
        /// Occurs when the left mouse button is pressed while the mouse pointer
        ///  is over this control.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.UpdateVisualState();
            if (this.buttonBorder != null && this.buttonBorder is TabItemAdvBorderClassic)
            {
                if (!(this.buttonBorder as TabItemAdvBorderClassic).IsTabItemBorder)
                {
                    (this.buttonBorder as TabItemAdvBorderClassic).IsReversed = true;
                }
            }
        }

        /// <summary>
        /// Occurs when the left mouse button is released while the mouse pointer
        ///  is over this control.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            this.UpdateVisualState();
            if (this.buttonBorder != null && this.buttonBorder is TabItemAdvBorderClassic)
            {
                if (!(this.buttonBorder as TabItemAdvBorderClassic).IsTabItemBorder)
                {
                    (this.buttonBorder as TabItemAdvBorderClassic).IsReversed = false;
                }
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        ///  Occurs when IsEnabled property changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void ScrollingButtonIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.buttonBorder != null)
            {
                if (this.buttonBorder is TabItemAdvBorderClassic)
                {
                    this.Opacity = 1d;
                    (this.buttonBorder as TabItemAdvBorderClassic).RefreshBorderPaths();
                }
                else
                {
                    if (!this.IsEnabled)
                    {
                        VisualStateManager.GoToState(this, "Normal", true);
                        this.Opacity = 0.5d;
                    }
                    else
                    { 
                        this.Opacity = 1d; 
                    }
                }
            }
        }

        /// <summary>
        /// Rotates the button with specified angle.
        /// </summary>
        /// <param name="angle">Angle to rotate the button.</param>
        internal void RotateButton(double angle)
        {
            if (this.buttonBorder != null && this.buttonBorder is TabItemAdvBorderClassic)
            {
                (this.buttonBorder as TabItemAdvBorderClassic).RotateBorder(angle);
            }
        }

        /// <summary>
        /// Update visual state of the button.
        /// </summary>
        internal virtual void UpdateVisualState()
        {
            if (IsPressed)
            {
                VisualStateManager.GoToState(this, "Pressed", true);
            }
            else if (IsMouseOver)
            {
                VisualStateManager.GoToState(this, "MouseOver", true);
            }
        }

        /// <summary>
        /// Calls OnScrollDirectionChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnScrollDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollingButton instance = (ScrollingButton) d;
            instance.OnScrollDirectionChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises ScrollDirectionChanged event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnScrollDirectionChanged(DependencyPropertyChangedEventArgs e)
        {
            this.UpdatePaths();

            if (this.ScrollDirectionChanged != null)
            {
                this.ScrollDirectionChanged(this, e);
            }
        }

        /// <summary>
        /// Updates the paths.
        /// </summary>
        internal void UpdatePaths()
        {
            this.CollapsePaths();

            switch (this.ScrollDirection)
            {
                case ScrollDirection.FirstTab:
                    if (this.path6 != null)
                    {
                        this.path4.Visibility = Visibility.Visible;
                        this.path6.Visibility = Visibility.Visible;
                    }

                    break;
                case ScrollDirection.LastTab:
                    if (this.path3 != null)
                    {
                        this.path1.Visibility = Visibility.Visible;
                        this.path3.Visibility = Visibility.Visible;
                    }

                    break;
                case ScrollDirection.NextTab:
                    if (this.path1 != null)
                    {
                        this.path1.Visibility = Visibility.Visible;
                    }

                    break;
                case ScrollDirection.PrevTab:
                    if (this.path5 != null)
                    {
                        this.path5.Visibility = Visibility.Visible;
                    }

                    break;
                case ScrollDirection.NextPage:
                    if (this.path1 != null && this.path2 != null)
                    {
                        this.path1.Visibility = Visibility.Visible;
                        this.path2.Visibility = Visibility.Visible;
                    }

                    break;
                case ScrollDirection.PrevPage:
                    if (this.path4 != null && this.path5 != null)
                    {
                        this.path4.Visibility = Visibility.Visible;
                        this.path5.Visibility = Visibility.Visible;
                    }

                    break;
            }
        }

        /// <summary>
        /// Collapses the paths.
        /// </summary>
        private void CollapsePaths()
        {
            if (this.path1 != null)
            {
                this.path1.Visibility = Visibility.Collapsed;
            }

            if (this.path2 != null)
            {
                this.path2.Visibility = Visibility.Collapsed;
            }

            if (this.path3 != null)
            {
                this.path3.Visibility = Visibility.Collapsed;
            }

            if (this.path4 != null)
            {
                this.path4.Visibility = Visibility.Collapsed;
            }

            if (this.path5 != null)
            {
                this.path5.Visibility = Visibility.Collapsed;
            }

            if (this.path6 != null)
            {
                this.path6.Visibility = Visibility.Collapsed;
            }
        }
        #endregion
    }
}
