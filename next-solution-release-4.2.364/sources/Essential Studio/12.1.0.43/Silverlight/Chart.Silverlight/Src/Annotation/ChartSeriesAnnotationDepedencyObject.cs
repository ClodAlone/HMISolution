#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for ChartSeriesAnnotation
    /// </summary>
    public class ChartSeriesAnnotation : DependencyObject,IDisposable
    {
        #region dependencyProperties

        AnnotationPanel m_annotationPanel = null;
        internal AnnotationPanel SeriesAnnotationPanel
        {
            get { return m_annotationPanel; }
            set { m_annotationPanel = value; }
        }

        /// <summary>
        /// Identifies the X dependency property.
        /// </summary>
        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register("X", typeof(double), typeof(ChartSeriesAnnotation), new PropertyMetadata(0d, new PropertyChangedCallback(onPositionchange)));
        /// <summary>
        /// Get or Set RotationAngle
        /// </summary>
        public static readonly  DependencyProperty RotateAngleProperty =
            DependencyProperty.Register("RotateAngle", typeof(double), typeof(ChartSeriesAnnotation), new PropertyMetadata(0d, new PropertyChangedCallback(onPositionchange)));
      
        /// <summary>
        /// Identifies the Y dependency property.
        /// </summary>
        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register("Y", typeof(double), typeof(ChartSeriesAnnotation), new PropertyMetadata(0d, new PropertyChangedCallback(onPositionchange)));

        /// <summary>
        /// Identifies the Header dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(ChartSeriesAnnotation), new PropertyMetadata(String.Empty));

        /// <summary>
        /// Identifies the Description dependency property.
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(ChartSeriesAnnotation), new PropertyMetadata(String.Empty));

        /// <summary>
        /// Identifies the Template dependency property.
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register("Template", typeof(DataTemplate), typeof(ChartSeriesAnnotation), new PropertyMetadata(null));

        private static void onPositionchange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
            ChartSeriesAnnotation annot = d as ChartSeriesAnnotation;
            
            if (annot != null && annot.SeriesAnnotationPanel != null)
            {
                annot.SeriesAnnotationPanel.InvalidateArrange();
            }
        }

        /// <summary>
        /// Identifies the OffsetX dependency property.
        /// </summary>
        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.Register("OffsetX", typeof(double), typeof(ChartSeriesAnnotation), new PropertyMetadata(0d, new PropertyChangedCallback(onoffsetXchange)));
        private static void onoffsetXchange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeriesAnnotation annot = d as ChartSeriesAnnotation;

            annot.X -= Convert.ToDouble(e.OldValue);    
            
                annot.X += annot.OffsetX;

                if (annot != null && annot.SeriesAnnotationPanel != null)
                {
                    annot.SeriesAnnotationPanel.InvalidateArrange();
                }
           
        }
        
        /// <summary>
        /// Identifies the OffsetY dependency property.
        /// </summary>
        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.Register("OffsetY", typeof(double), typeof(ChartSeriesAnnotation), new PropertyMetadata(0d, new PropertyChangedCallback(onoffsetYchange)));
        private static void onoffsetYchange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartSeriesAnnotation annot = d as ChartSeriesAnnotation;

            annot.Y -= Convert.ToDouble(e.OldValue);

            annot.Y += annot.OffsetY;

            if (annot != null && annot.SeriesAnnotationPanel != null)
            {
                annot.SeriesAnnotationPanel.InvalidateArrange();
            }
        }
        /// <summary>
        ///  Identifies the AnnotationShape dependency property.
        /// </summary>
        public static readonly DependencyProperty AnnotationShapeProperty =
        DependencyProperty.Register("AnnotationShape", typeof(AnnotationShapes), typeof(ChartSeriesAnnotation), new PropertyMetadata(AnnotationShapes.None, new PropertyChangedCallback(OnShapeChanged)));


        /// <summary>
        /// Get or Set AnnotationShape
        /// </summary>
        public AnnotationShapes AnnotationShape
        {
            get
            {
                return (AnnotationShapes)GetValue(AnnotationShapeProperty);
            }

            set
            {
                SetValue(AnnotationShapeProperty, value);
            }
        }
        /// <summary>
        /// Get or Set RotationAngle
        /// </summary>
        public double RotateAngle
        {
            get
            {
                return (double)GetValue(RotateAngleProperty);
            }

            set
            {
                SetValue(RotateAngleProperty, value);
            }
        }

        private static ResourceDictionary rd;
        private static void OnShapeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
            
            //DependencyObject obj = VisualTreeHelper.GetParent(d);

            ChartSeriesAnnotation instance = d as ChartSeriesAnnotation;            
            string Shape = Convert.ToString((AnnotationShapes)e.NewValue);

            rd = new SharedResourceDictionary()
            {
                Source = new Uri("/Syncfusion.Chart.Silverlight;component/Themes/generic.xaml", UriKind.Relative)
            };
            //instance.Template = rd["SeriesTemplate" + Shape.ToString()] as DataTemplate;
             instance.Template = rd["SeriesTemplate" + Shape] as DataTemplate;
            
            //instance.SeriesAnnotationPanel

        }
        /// <summary>
        ///  Identifies the Fill dependency property.
        /// </summary>
        public static readonly DependencyProperty FillProperty =
          DependencyProperty.Register("Fill", typeof(Brush), typeof(ChartSeriesAnnotation), new PropertyMetadata(null, OnFillChanged));

        /// <summary>
        /// Get or Set Fill property
        /// </summary>
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        /// <summary>
        ///  Identifies the FillString dependency property.
        /// </summary>
        public static readonly DependencyProperty FillStringProperty =
          DependencyProperty.Register("FillString", typeof(string), typeof(ChartSeriesAnnotation), new PropertyMetadata("Transparent"));
        private static void OnFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
            ChartSeriesAnnotation label = d as ChartSeriesAnnotation;

            if (label != null)
            {
                SolidColorBrush brush = label.Fill as SolidColorBrush;

                label.FillString = brush.Color.ToString();
            }


        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the X. This is a dependency property.
        /// </summary>
        /// <value>The X value.</value>
        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        /// <summary>
        /// Get or Set FillStringProperty
        /// </summary>
        public string FillString
        {
            get { return (string)GetValue(FillStringProperty); }
            set { SetValue(FillStringProperty, value); }
        }
        /// <summary>
        /// Gets or sets the Y. This is a dependency property.
        /// </summary>
        /// <value>The Y value.</value>
        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Header. This is a dependency property.
        /// </summary>
        /// <value>The Header value.</value>
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Description. This is a dependency property.
        /// </summary>
        /// <value>The Description.</value>
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Annotation Template. This is a dependency property.
        /// </summary>
        /// <value>The Template.</value>
        public DataTemplate Template
        {
            get { return (DataTemplate)GetValue(TemplateProperty); }
            set { SetValue(TemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the OffsetX. This is a dependency property.
        /// </summary>
        /// <value>The OffsetX.</value>
        public double OffsetX
        {
            get { return (double)GetValue(OffsetXProperty); }
            set { SetValue(OffsetXProperty, value); }
        }

        /// <summary>
        /// Gets or sets the OffsetY. This is a dependency property.
        /// </summary>
        /// <value>The OffsetY.</value>
        public double OffsetY
        {
            get { return (double)GetValue(OffsetYProperty); }
            set { SetValue(OffsetYProperty, value); }
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Virtual method implementation for OnPropertyChanged
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        protected virtual void OnPropertyChanged(object obj, PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(obj, e);
            }
        }

        #endregion

        #region INotifyPropertyChanged Members
        /// <summary>
        /// Initializes PropertyChanged
        /// </summary>    
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            PropertyChanged = null;
            this.ClearValue(ChartSeriesAnnotation.AnnotationShapeProperty);
            rd = null;
        }

        #endregion
    }
}
