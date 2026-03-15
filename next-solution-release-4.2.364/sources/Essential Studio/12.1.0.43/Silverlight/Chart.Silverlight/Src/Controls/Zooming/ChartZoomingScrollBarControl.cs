#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
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

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for ZoomingScroolBar
    /// </summary>
    public class ZoomingScrollBar : Control,IDisposable
    {
        /// <summary>
        /// Called when instance created for ZoomingScrollBar
        /// </summary>
        public ZoomingScrollBar()
        {
            DefaultStyleKey = typeof(ZoomingScrollBar);
        }

        #region Properties
        /// <summary>
        /// Gets or sets the Minimum.
        /// </summary>
        /// <value>The Minimum.</value>
        internal double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Maximum.
        /// </summary>
        /// <value>The Maximum.</value>
        internal double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Value.
        /// </summary>
        /// <value>The Value.</value>
        internal double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets the SmallChange.
        /// </summary>
        /// <value>The SmallChange.</value>
        internal double SmallChange
        {
            get { return (double)GetValue(SmallChangeProperty); }
            set { SetValue(SmallChangeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the LargeChange.
        /// </summary>
        /// <value>The LargeChange.</value>
        internal double LargeChange
        {
            get { return (double)GetValue(LargeChangeProperty); }
            set { SetValue(LargeChangeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the ViewportSize.
        /// </summary>
        /// <value>The ViewportSize.</value>
        internal double ViewportSize
        {
            get { return (double)GetValue(ViewportSizeProperty); }
            set { SetValue(ViewportSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the OpposedPosition. This is dependency property.
        /// </summary>
        /// <value>The OpposedPosition value is true or false.</value>
        internal bool OpposedPosition
        {
            get
            {
                return (bool)GetValue(OpposedPositionProperty);
            }

            set
            {
                SetValue(OpposedPositionProperty, value);
            }
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ZoomingScrollBar), new PropertyMetadata(Orientation.Horizontal));

        /// <summary>
        /// Idenfities OpposedPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty OpposedPositionProperty =
            DependencyProperty.Register("OpposedPosition", typeof(bool), typeof(ZoomingScrollBar), new PropertyMetadata(false));

        /// <summary>
        /// Minimum value of Zooming Scrollbar
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(ZoomingScrollBar), new PropertyMetadata(0d));

        /// <summary>
        /// SmallChange value of Zooming Scrollbar
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(ZoomingScrollBar), new PropertyMetadata(1d));

        /// <summary>
        /// Minimum value of Zooming Scrollbar
        /// </summary>
        public static readonly DependencyProperty SmallChangeProperty =
            DependencyProperty.Register("SmallChange", typeof(double), typeof(ZoomingScrollBar), new PropertyMetadata(0.1d));

        /// <summary>
        /// LargeChange value of Zooming Scrollbar
        /// </summary>
        public static readonly DependencyProperty LargeChangeProperty =
            DependencyProperty.Register("LargeChange", typeof(double), typeof(ZoomingScrollBar), new PropertyMetadata(1d));

        /// <summary>
        /// Value of Zooming Scrollbar
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(ZoomingScrollBar), new PropertyMetadata(0d, new PropertyChangedCallback(OnValueChanged)));

        /// <summary>
        /// ViewportSize value of Zooming Scrollbar
        /// </summary>
        public static readonly DependencyProperty ViewportSizeProperty =
            DependencyProperty.Register("ViewportSize", typeof(double), typeof(ZoomingScrollBar), new PropertyMetadata(double.MaxValue));

        #endregion

        #region Implementation

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Generate m_visibleInterval dependency property Changed.
        /// </summary>
        public event PropertyChangedCallback ValueChanged;

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ZoomingScrollBar scrollbar = (ZoomingScrollBar)d;
            scrollbar.OnValueChanged(e);
        }
        /// <summary>
        /// Called when value property changed
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnValueChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, e);
            }
        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            ValueChanged = null;
        }

        #endregion
    }


}
