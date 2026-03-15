#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Gauge
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// Represents panel used in elements templates to set BorderThickness property.
    /// </summary>
    public class GaugeElementPanel : Panel
    {
        #region Dependency properties

        /// <summary>
        /// Identifies the <see cref="BorderWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderWidthProperty =
            DependencyProperty.Register("BorderWidth", typeof(Thickness), typeof(GaugeElementPanel), new PropertyMetadata(new PropertyChangedCallback(OnBorderWidthChanged)));

        #endregion

        #region Initialization
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GaugeElementPanel"/> class
        /// </summary>
        public GaugeElementPanel()
        {
            this.Loaded += new RoutedEventHandler(this.GaugeElementPanelLoaded);
        }

        #endregion

        #region Events
        
        /// <summary>
        /// Event that is raised when <see cref="BorderWidth"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback BorderWidthChanged;

        #endregion

        #region DP getters & setters

        /// <summary>
        /// Gets or sets the Border width
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>
        /// </value>
        /// <seealso cref="Thickness"/>
        public Thickness BorderWidth
        {
            get
            {
                return (Thickness)GetValue(BorderWidthProperty);
            }

            set
            {
                SetValue(BorderWidthProperty, value);
            }
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Provides the behavior for the Arrange pass of Silverlight layout. Classes can
        /// override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should
        /// use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
                          
        }

        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can
        /// override this method to define their own Measure pass behavior.
        /// </summary>
        /// <param name="availableSize">The available size that this object can give to
        /// child objects. Infinity (<see cref="F:System.Double.PositiveInfinity"/>) can be
        /// specified as a value to indicate that the object will size to whatever content
        /// is available.</param>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its
        /// calculations of the allocated sizes for child objects; or based on other
        /// considerations, such as a fixed container size.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="BorderWidthChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnBorderWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshBorderThickness();
            if (this.BorderWidthChanged != null)
            {
                this.BorderWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnBorderWidthChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnBorderWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeElementPanel instance = (GaugeElementPanel)d;
            instance.OnBorderWidthChanged(e);
        }

       /// <summary>
       /// Refreshes border thicknes when element loaded
       /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The object containing the event data.</param>
        private void GaugeElementPanelLoaded(object sender, RoutedEventArgs e)
        {
            this.RefreshBorderThickness();
        }

        /// <summary>
        /// Refreshes Border thickness
        /// </summary>
        private void RefreshBorderThickness()
        {
            foreach (FrameworkElement elem in this.Children)
            {
                if (elem is Path)
                {
                    Path elemPath = elem as Path;
                    elemPath.StrokeThickness = this.BorderWidth.Left;
                }
                if (elem is Ellipse)
                {
                    Ellipse elemPath = elem as Ellipse;
                    elemPath.StrokeThickness = this.BorderWidth.Left;
                }
            }
        }
        #endregion
    }
}
