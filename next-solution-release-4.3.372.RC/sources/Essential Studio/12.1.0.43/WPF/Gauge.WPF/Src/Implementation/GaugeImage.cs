// <copyright file="GaugeImage.cs" company="Syncfusion Software">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Syncfusion.Windows.Gauge
{
    /// <summary>
    /// Represents an image visual element that can be placed in the Gauge.
    /// </summary>
    /// <remarks>
    /// To specify the source of the image use the <see cref="ImageSource"/> property.0The image
    /// can be rotated to a specific angle using the <see cref="Angle"/> property.
    /// </remarks>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class GaugeImage : LocalizableGaugeElement
    {
        #region Events
        /// <summary>
        /// Event that is raised when <see cref="Angle"/> property is changed.
        /// </summary>
        internal event PropertyChangedCallback AngleChanged;

        /// <summary>
        /// Event that is raised when <see cref="ImageHeight"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ImageHeightChanged;

        /// <summary>
        /// Event that is raised when <see cref="ImageSource"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ImageSourceChanged;

        /// <summary>
        /// Event that is raised when <see cref="ImageWidth"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ImageWidthChanged;

        /// <summary>
        /// Event that is raised when <see cref="ResizeMode"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback ResizeModeChanged;
        #endregion Events

        #region Dependency Properties
        /// <summary>
        /// Identifies the <see cref="Angle"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(GaugeImage), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnAngleChanged)));

        /// <summary>
        /// Identifies the <see cref="ImageHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(double), typeof(GaugeImage), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnImageHeightChanged)));

        /// <summary>
        /// Identifies the <see cref="Image"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(GaugeImage), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnImageSourceChanged)));

        /// <summary>
        /// Identifies the <see cref="ImageWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(double), typeof(GaugeImage), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnImageWidthChanged)));

        /// <summary>
        /// Identifies the <see cref="ResizeMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ResizeModeProperty =
            DependencyProperty.Register("ResizeMode", typeof(GaugeImageResizeMode), typeof(GaugeImage), new FrameworkPropertyMetadata(GaugeImageResizeMode.None, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnResizeModeChanged)));
        #endregion Dependency Properties

        #region DP Getters & Setters
        /// <summary>
        /// Gets or sets rotation angle of the image.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0d.
        /// </value>
        internal double Angle
        {
            get
            {
                return (double)GetValue(AngleProperty);
            }

            set
            {
                SetValue(AngleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height of the element.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="ImageWidth"/>
        public double ImageHeight
        {
            get
            {
                return (double)GetValue(ImageHeightProperty);
            }

            set
            {
                SetValue(ImageHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the image source of the element.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="ImageSource"/>
        /// </value>
        public ImageSource ImageSource
        {
            get
            {
                return (ImageSource)GetValue(ImageSourceProperty);
            }

            set
            {
                SetValue(ImageSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the element.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="ImageHeight"/>
        public double ImageWidth
        {
            get
            {
                return (double)GetValue(ImageWidthProperty);
            }

            set
            {
                SetValue(ImageWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the resize mode of the image.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="GaugeImageResizeMode"/>
        /// Default value is GaugeImageResizeMode.None.
        /// </value>
        public GaugeImageResizeMode ResizeMode
        {
            get
            {
                return (GaugeImageResizeMode)GetValue(ResizeModeProperty);
            }

            set
            {
                SetValue(ResizeModeProperty, value);
            }
        }
        #endregion DP Getters & Setters

        #region Overrides
        /// <summary>
        /// Measures the size in layout required for child elements 
        /// and determines a size for the element.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements.</param>
        /// <returns>The size that this element determines it needs during layout.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(this.ImageWidth, this.ImageHeight);
        }

        /// <summary>
        /// Participates in rendering operations that are directed by the layout system.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            RotateTransform transform = new RotateTransform(this.Angle, this.DesiredSize.Width / 2, this.DesiredSize.Height / 2);
            drawingContext.PushTransform(transform);
            if (this.ImageSource != null)
            {
                if (this.ResizeMode == GaugeImageResizeMode.Stretch)
                {
                    //drawingContext.DrawImage(this.ImageSource, new Rect(0, 0, this.DesiredSize.Width, this.DesiredSize.Height));
                    if (ImageWidth != 0 || ImageHeight != 0)
                    {
                        drawingContext.DrawImage(this.ImageSource, new Rect(0, 0, this.ImageWidth, this.ImageHeight));
                    }
                    else
                    {
                        drawingContext.DrawImage(this.ImageSource, new Rect(0, 0, this.Width, this.Height));
                    }
                }
                else
                {
                    if (ImageWidth != 0 || ImageHeight != 0)
                    {
                        drawingContext.DrawImage(this.ImageSource, new Rect(0, 0, this.ImageWidth, this.ImageHeight));
                    }
                    else
                    {
                        drawingContext.DrawImage(this.ImageSource, new Rect(0, 0, this.ImageSource.Width, this.ImageSource.Height));
                    }
                }
            }

            drawingContext.Pop();
        }
        #endregion Overrides

        #region Implementation
        /// <summary>
        /// Calls OnAngleChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeImage instance = (GaugeImage)d;
            instance.OnAngleChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="AngleChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnAngleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AngleChanged != null)
            {
                this.AngleChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnImageHeightChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnImageHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeImage instance = (GaugeImage)d;
            instance.OnImageHeightChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ImageHeightChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnImageHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ImageHeightChanged != null)
            {
                this.ImageHeightChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnImageSourceChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeImage instance = (GaugeImage)d;
            instance.OnImageSourceChanged(e);
            instance.InvalidateVisual();
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ImageSourceChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnImageSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ImageSourceChanged != null)
            {
                this.ImageSourceChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnImageWidthChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnImageWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeImage instance = (GaugeImage)d;
            instance.OnImageWidthChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ImageWidthChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnImageWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ImageWidthChanged != null)
            {
                this.ImageWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ResizeModeChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnResizeModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ResizeModeChanged != null)
            {
                this.ResizeModeChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnResizeModeChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnResizeModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeImage instance = (GaugeImage)d;
            instance.OnResizeModeChanged(e);
        }
        #endregion Implementation       

        #region Added Code
        /// <summary>
        /// Raises the <see cref="System.Windows.FrameworkElement.Initialized"/> event. 
        /// This method is invoked whenever <see cref="System.Windows.FrameworkElement.IsInitialized"/> property 
        /// is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            NameScope.SetNameScope(this, null);
        }

        /// <summary>
        /// Sets the scope to <see cref="CircularGauge"/>to facilitate Binding. 
        /// This method is invoked during initialization.
        /// </summary>
        /// <param name="gauge">The <see cref="GaugeBase"/> that contains the reference to the Gauge.</param>
        internal void SetScope(GaugeBase gauge)
        {
            CalculateScope(gauge);
        }

        /// <summary>
        /// Calculates the scope. 
        /// This method is invoked during initialization.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> which contains the element to set the scope for.</param>
        internal void CalculateScope(DependencyObject obj)
        {
            DependencyObject ele = obj;
            while (ele != null)
            {
                INameScope ns = NameScope.GetNameScope(ele);
                if (ns != null)
                {
                    if (!(ns is System.Windows.NameScope))
                    {
                        break;
                    }

                    NameScope.SetNameScope(this, ns);
                    break;
                }

                ele = LogicalTreeHelper.GetParent(ele) ?? VisualTreeHelper.GetParent(ele);
            }
        }
        #endregion Added Code
    }
}
