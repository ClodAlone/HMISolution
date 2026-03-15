#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading;
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
    /// Represents tab popup menu.
    /// </summary>
    public class DragMarker : Control
    {
        #region Fields
        /// <summary>
        /// Popup placement target.
        /// </summary>
        private FrameworkElement placementTarget;

        /// <summary>
        /// The popup.
        /// </summary>
        private Popup popup;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes new instance of the DragMarker class.
        /// </summary>
        public DragMarker()
        {
            this.DefaultStyleKey = typeof(DragMarker);            
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the placement target of the popup.
        /// </summary>
        public FrameworkElement PlacementTarget
        {
            get
            {
                return this.placementTarget;
            }

            set
            {
                this.placementTarget = value;
            }
        }

        /// <summary>
        /// Gets or sets the vertical offset of the popup.
        /// </summary>
        public double VerticalOffset
        {
            get
            {
                return (double) GetValue(VerticalOffsetProperty);
            }

            set
            {
                SetValue(VerticalOffsetProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the horizontal offset of the popup.
        /// </summary>
        public double HorizontalOffset
        {
            get
            {
                return (double) GetValue(HorizontalOffsetProperty);
            }

            set
            {
                SetValue(HorizontalOffsetProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the popup is open.
        /// </summary>
        internal bool IsOpen
        {
            get
            {
                bool b = false;
                if (this.popup != null)
                {
                    b = this.popup.IsOpen;
                }

                return b;
            }

            set
            {
                if (this.popup != null)
                {
                    this.popup.IsOpen = value;
                }
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when a particular instance of a TabPopupMenu opens.
        /// </summary>
        public event RoutedEventHandler Opened;

        /// <summary>
        /// Occurs when a particular instance of a TabPopupMenu closes.
        /// </summary>
        public event RoutedEventHandler Closed;
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies the <see cref="VerticalOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(DragMarker), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the <see cref="HorizontalOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(DragMarker), new PropertyMetadata(0d));
        #endregion

        #region Overrides
        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) 
        /// call System.Windows.Controls.Control.ApplyTemplate() method.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.popup = this.GetTemplateChild("Popup") as Popup;
            if (this.popup != null)
            {
                this.popup.Opened += new EventHandler(this.PopupOpened);
                this.popup.Closed += new EventHandler(this.PopupClosed);
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
            base.MeasureOverride(availableSize);
            return new Size(15, 15);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Occurs when popup is opened.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void PopupOpened(object sender, EventArgs e)
        {
            if (this.Opened != null)
            {
                this.Opened(sender, new RoutedEventArgs());
            }
        }

        /// <summary>
        /// Occurs when popup is closed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void PopupClosed(object sender, EventArgs e)
        {
            if (this.Closed != null)
            {
                this.Closed(sender, new RoutedEventArgs());
            }
        }

        /// <summary>
        /// Shows the popup.
        /// </summary>
        public void Show()
        {
            this.popup.IsOpen = true;
        }

        /// <summary>
        /// Closes the popup.
        /// </summary>
        public void Close()
        {
            this.popup.IsOpen = false;
        }
        #endregion
    }
}
