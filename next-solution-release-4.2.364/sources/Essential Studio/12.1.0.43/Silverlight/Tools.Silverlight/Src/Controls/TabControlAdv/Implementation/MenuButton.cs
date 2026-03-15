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
    /// Represents menu button class.
    /// </summary>
    [TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
    public class MenuButton : ToggleButton
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

        #region Class members
        /// <summary>
        /// Border used for drawing button.
        /// </summary>
        private FrameworkElement buttonBorder;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes new instance of the MenuButton class.
        /// </summary>
        public MenuButton()
        {
            this.DefaultStyleKey = typeof(MenuButton);
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) 
        /// call System.Windows.Controls.Control.ApplyTemplate() method.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
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
        /// Occurs when the left mouse button is pressed while the mouse pointer
        ///  is over this control.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (this.buttonBorder != null)
            {
                if (buttonBorder is TabItemAdvBorderClassic)
                {
                    if (!(this.buttonBorder as TabItemAdvBorderClassic).IsTabItemBorder)
                    {
                        (this.buttonBorder as TabItemAdvBorderClassic).IsReversed = true;
                    }
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
            if (this.buttonBorder != null)
            {
                if (buttonBorder is TabItemAdvBorderClassic)
                {
                    if (!(this.buttonBorder as TabItemAdvBorderClassic).IsTabItemBorder)
                    {
                        (this.buttonBorder as TabItemAdvBorderClassic).IsReversed = false;
                    }
                }
            }
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
        #endregion

        #region Implementation
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
        /// Rotates the button with specified angle.
        /// </summary>
        /// <param name="angle">Angle to rotate the button.</param>
        internal void RotateButton(double angle)
        {
            if (this.buttonBorder != null)
            {
                if (buttonBorder is TabItemAdvBorderClassic)
                {
                    (this.buttonBorder as TabItemAdvBorderClassic).RotateBorder(angle);
                }
                else
                {
                    RotateTransform rotate = new RotateTransform();
                    rotate.Angle = angle;
                    rotate.CenterX = buttonBorder.ActualWidth / 2;
                    rotate.CenterY = buttonBorder.ActualHeight / 2;
                    buttonBorder.RenderTransform = rotate;
                }
            }
        }
        #endregion
    }
}
