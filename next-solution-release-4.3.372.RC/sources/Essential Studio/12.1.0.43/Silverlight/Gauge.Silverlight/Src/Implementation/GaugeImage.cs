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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Represents image visual element.
    /// </summary>
    /// <example>
    /// <para></para>
    /// <list type="table">
    /// <listheader>
    /// <term>C# :</term></listheader>
    /// <item>
    /// <description>using Syncfusion.Windows.Gauge; 
    /// <para></para>
    /// <para>GaugeImage gaugeImage = new GaugeImage();</para>
    /// <para>            gaugeImage.ImageWidth = 90;</para>
    /// <para>            gaugeImage.ImageHeight = 30; </para>
    /// <para>            BitmapImage bmp = new BitmapImage( new Uri(&quot;../Images/synclogo.jpg&quot;, UriKind.Relative) );</para>
    /// <para>            gaugeImage.ImageSource = bmp;</para>
    /// <para>            gaugeImage.Location = new Point( 50, 15 );</para>
    /// <para>            gaugeImage.ResizeMode = GaugeImageResizeMode.Stretch;</para>
    /// <para>            circulargauge.Images.Add( gaugeImage );</para></description></item></list>
    /// <para></para>
    /// <list type="table">
    /// <listheader>
    /// <term>Xaml :</term></listheader>
    /// <item>
    /// <description>xmlns:syncfusion=&quot;clr-namespace:Syncfusion.Windows.Gauge;assembly=Syncfusion.Gauge.Silverlight&quot;
    /// <para>    </para>
    /// <para>&lt;syncfusion:CircularGauge.Images&gt;</para>
    /// <para>&lt;syncfusion:GaugeImage ImageSource=&quot;../image/synclogo.jpg&quot;  Location=&quot;50,50&quot; ImageHeight=&quot;30&quot; ImageWidth=&quot;90&quot; ResizeMode=&quot;Stretch&quot; /&gt;</para>
    /// <para>&lt;/syncfusion:CircularGauge.Images&gt;</para>
    /// <para>&lt;/syncfusion:CircularGauge&gt;</para></description></item></list>
    /// </example>
    public class GaugeImage : LocalizableGaugeElement
    {
        #region Dependency properties

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Gauge.GaugeImage.ImageHeight">ImageHeight</see> dependency property.
        /// </summary>
        /// <returns>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </returns>
        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(double), typeof(GaugeImage), new PropertyMetadata(0d, new PropertyChangedCallback(OnImageHeightChanged)));

        /// <summary>
        /// Identifies the <see
        /// cref="P:Syncfusion.Windows.Gauge.GaugeImage.ImageSource">ImageSource</see>
        /// dependency property.
        /// </summary>
        /// <returns>
        /// <para>Type : <see
        /// cref="T:System.Windows.Media.ImageSource">System.Window.Media.ImageSource</see></para>
        /// </returns>
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(GaugeImage), new PropertyMetadata(new PropertyChangedCallback(OnImageSourceChanged)));

        /// <summary>
        /// Identifies the <see
        /// cref="P:Syncfusion.Windows.Gauge.GaugeImage.ImageWidth">ImageWidth</see>
        /// dependency property.
        /// </summary>
        /// <returns>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </returns>
        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(double), typeof(GaugeImage), new PropertyMetadata(0d, new PropertyChangedCallback(OnImageWidthChanged)));

        /// <summary>
        /// Identifies the <see
        /// cref="P:Syncfusion.Windows.Gauge.GaugeImage.ResizeMode">ResizeMode</see>
        /// dependency property.
        /// </summary>
        /// <returns>
        /// Type : <see
        /// cref="T:Syncfusion.Windows.Gauge.GaugeImageResizeMode">GaugeImageResizeMode</see>
        /// </returns>
        public static readonly DependencyProperty ResizeModeProperty =
            DependencyProperty.Register("ResizeMode", typeof(GaugeImageResizeMode), typeof(GaugeImage), new PropertyMetadata(GaugeImageResizeMode.Stretch, new PropertyChangedCallback(OnResizeModeChanged)));

        /// <summary>
        /// Identifies the <see cref="Angle"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(GaugeImage), new PropertyMetadata(0d, new PropertyChangedCallback(OnAngleChanged)));

        #endregion

        #region Private members

        /// <summary>
        /// mgauge image private variable
        /// </summary>
        private Image mgaugeImage;

        #endregion

        #region Initialization
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GaugeImage"/> class
        /// </summary>
        public GaugeImage()
        {
            DefaultStyleKey = typeof(GaugeImage);
            this.Loaded += new RoutedEventHandler(this.GaugeImageLoaded);
        }

        #endregion

        #region Events

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Gauge.GaugeImage.ImageHeight">ImageHeight</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback ImageHeightChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Gauge.GaugeImage.ImageSource">ImageSource</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback ImageSourceChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Gauge.GaugeImage.ImageWidth">ImageWidth</see>
        /// property is changed.
        /// </summary>
        public event PropertyChangedCallback ImageWidthChanged;

        /// <summary>
        /// Event that is raised when <see
        /// cref="P:Syncfusion.Windows.Gauge.GaugeImage.ResizeMode">ResizeMode</see> property is changed.
        /// </summary>
        public event PropertyChangedCallback ResizeModeChanged;

        /// <summary>
        /// Event that is raised when <see cref="Angle"/> property is changed.
        /// </summary>
        internal event PropertyChangedCallback AngleChanged;

        #endregion

        #region DP getters & setters

        /// <summary>
        /// Gets or sets the width of the element. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// <para>Default value is 0. </para>
        /// </remarks>
        /// <value>
        /// <para>Type : <see cref="T:System.Double">System.Double</see></para>
        /// <para> </para>
        /// </value>
        /// <seealso cref="double">double</seealso>
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
        /// Gets or sets resize mode of the image. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Default value is <see
        /// cref="F:Syncfusion.Windows.Gauge.GaugeImageResizeMode.None">GaugeImageResizeMode.None</see>.
        /// </remarks>
        /// <value>
        /// Type : <see
        /// cref="T:Syncfusion.Windows.Gauge.GaugeImageResizeMode">GaugeImageResizeMode</see>
        /// </value>
        /// <seealso cref="GaugeImageResizeMode">GaugeImageResizeMode</seealso>
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

        /// <summary>
        /// Gets or sets the height of the element. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Default value is 0.
        /// </remarks>
        /// <value>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </value>
        /// <seealso cref="double">double</seealso>
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
        /// Gets or sets the image source of the element. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type : <see
        /// cref="T:System.Windows.Media.ImageSource">System.Windows.Media.ImageSource</see>
        /// </value>
        /// <seealso cref="ImageSource">ImageSource</seealso>
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
        /// Gets or sets rotation angle of the image. This is a dependency property.
        /// </summary>
        /// <remarks>
        /// Default value is 0d.
        /// </remarks>
        /// <value>
        /// Type : <see cref="T:System.Double">System.Double</see>
        /// </value>
        /// <seealso cref="double">double</seealso>
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
        #endregion

        #region Overrides
        
        /// <summary>
        /// Builds the current template's visual tree if necessary.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.mgaugeImage = this.GetTemplateChild("GaugeImage") as Image;

            if (this.mgaugeImage != null)
            {
                this.mgaugeImage.Source = this.ImageSource;
            }
            this.UpdateVisualStyle();
            this.RefreshImage();
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Arranges the image according to the size given
        /// </summary>
        /// <param name="finalSize">Final size</param>
        /// <returns>Returns the size</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            base.ArrangeOverride(finalSize);
            return new Size(this.ImageWidth, this.ImageHeight);
        }

        /// <summary>
        /// Measures the size in layout required for child elements 
        /// and determines a size for the element.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements.</param>
        /// <returns>The size that this element determines it needs during layout.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            base.MeasureOverride(availableSize);
            return new Size(this.ImageWidth, this.ImageHeight);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="AngleChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnAngleChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshImage();
            if (this.AngleChanged != null)
            {
                this.AngleChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ImageHeightChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnImageHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshImage();
            if (this.ImageHeightChanged != null)
            {
                this.ImageHeightChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ImageSourceChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnImageSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.ImageSourceChanged != null)
            {
                this.ImageSourceChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ImageWidthChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnImageWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            this.RefreshImage();
            if (this.ImageWidthChanged != null)
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
            this.RefreshImage();
            if (this.ResizeModeChanged != null)
            {
                this.ResizeModeChanged(this, e);
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
        /// Calls OnImageSourceChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeImage instance = (GaugeImage)d;
            instance.OnImageSourceChanged(e);
        }

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
        /// Calls OnResizeModeChanged method of the instance, notifies of the depencency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occures on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnResizeModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugeImage instance = (GaugeImage)d;
            instance.OnResizeModeChanged(e);
        }

        /// <summary>
        /// Refreshes Gauge Image when loaded
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The object containing the event data.</param>
        private void GaugeImageLoaded(object sender, RoutedEventArgs e)
        {
            this.RefreshImage();
        }

        /// <summary>
        /// Refreshes Image
        /// </summary>
        private void RefreshImage()
        {
            if (this.mgaugeImage != null)
            {
                if (this.ResizeMode == GaugeImageResizeMode.None)
                {
                    this.mgaugeImage.Stretch = Stretch.None;
                    this.mgaugeImage.ClearValue(WidthProperty);
                    this.mgaugeImage.ClearValue(HeightProperty);
                }
                else if (this.ResizeMode == GaugeImageResizeMode.Stretch)
                {
                    this.mgaugeImage.Stretch = Stretch.Fill;
                    this.mgaugeImage.Width = this.ImageWidth;
                    this.mgaugeImage.Height = this.ImageHeight;
                }

                RotateTransform transform = new RotateTransform();
                transform.Angle = this.Angle;
                transform.CenterX = this.mgaugeImage.ActualWidth / 2;
                transform.CenterY = this.mgaugeImage.ActualHeight / 2;
                this.mgaugeImage.RenderTransform = transform;
            }
        }

        #endregion
    }
}
