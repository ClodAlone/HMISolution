#region Copyright Syncfusion Inc. 2001 - 2014
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

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for ChartAnnotationLabel
    /// </summary>
    public class ChartAnnotationLabel:DependencyObject,IDisposable
    {

        AnnotationPanel m_annotationPanel = null;
        internal AnnotationPanel ChartAnnotationPanel
        {
            get { return m_annotationPanel; }
            set { m_annotationPanel = value; }
        }

        private static void onPositionchange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAnnotationLabel annot = d as ChartAnnotationLabel;

            if (annot != null && annot.ChartAnnotationPanel != null)
            {
                annot.ChartAnnotationPanel.InvalidateArrange();
            }
        }
       
        #region Dependency properties
        /// <summary>
        /// Identifies the OffsetX dependency property.
        /// </summary>
        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.Register("OffsetX", typeof(double), typeof(ChartAnnotationLabel), new PropertyMetadata(0d, new PropertyChangedCallback(onPositionchange)));

        /// <summary>
        /// Identifies the OffsetY dependency property.
        /// </summary>
        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.Register("OffsetY", typeof(double), typeof(ChartAnnotationLabel), new PropertyMetadata(0d, new PropertyChangedCallback(onPositionchange)));

        /// <summary>
        /// Identifies the Content dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(ChartAnnotationLabel), new PropertyMetadata(null));
        /// <summary>
        /// Identifies the Template dependency property.
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
              DependencyProperty.Register("Template", typeof(DataTemplate), typeof(ChartAnnotationLabel), new PropertyMetadata(null));
       /// <summary>
        /// Identifies the AnnotationShape dependency property.
       /// </summary>
        public static readonly DependencyProperty AnnotationShapeProperty =
            DependencyProperty.Register("AnnotationShape", typeof(AnnotationShapes), typeof(ChartAnnotationLabel), new PropertyMetadata(AnnotationShapes.None, new PropertyChangedCallback(OnShapeChanged)));
        /// <summary>
        /// Identifies the Fill dependency property.
        /// </summary>
        public static readonly DependencyProperty FillProperty =
          DependencyProperty.Register("Fill", typeof(Brush), typeof(ChartAnnotationLabel), new PropertyMetadata(null, OnFillChanged));
        /// <summary>
        /// Get or Set  FillProperty
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
          DependencyProperty.Register("FillString", typeof(string), typeof(ChartAnnotationLabel), new PropertyMetadata("Transparent"));
        /// <summary>
        /// Get or Set FillStringProperty
        /// </summary>
        public string FillString
        {
            get { return (string)GetValue(FillStringProperty); }
            set { SetValue(FillStringProperty, value); }
        }
        
        private static void OnFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            ChartAnnotationLabel label = d as ChartAnnotationLabel;

            if (label != null)
            {
                SolidColorBrush brush = label.Fill as SolidColorBrush;

                label.FillString = brush.Color.ToString();
            }


        }
        #endregion

        #region Properties
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

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        /// <summary>
        /// gets or sets the Template
        /// </summary>
        public DataTemplate Template
        {
            get { return (DataTemplate)GetValue(TemplateProperty); }
            set { SetValue(TemplateProperty, value); }
        }
        /// <summary>
        /// Gets or sets the AnnotationShape
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
        

        #endregion
        
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAnnotationLabel"/> class.
        /// </summary>
        public ChartAnnotationLabel()
        {
        }
        private static ResourceDictionary rd;

        private static void OnShapeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartAnnotationLabel instance = d as ChartAnnotationLabel;

            rd = new SharedResourceDictionary()
            {
                Source = new Uri("/Syncfusion.Chart.Silverlight;component/Themes/generic.xaml", UriKind.RelativeOrAbsolute)
            };
            string shape = Convert.ToString((AnnotationShapes)e.NewValue);
            instance.Template = rd["template" + shape.ToString()] as DataTemplate;

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAnnotationLabel"/> class.
        /// </summary>
        /// <param name="offsetX">The offset X.</param>
        /// <param name="offsetY">The offset Y.</param>
        public ChartAnnotationLabel(double offsetX, double offsetY)
        {
            this.OffsetX = offsetX;
            this.OffsetY = offsetY;
        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            rd = null;
            this.ClearValue(ChartAnnotationLabel.TemplateProperty);
            this.ClearValue(ChartAnnotationLabel.ContentProperty);
            this.ClearValue(ChartAnnotationLabel.AnnotationShapeProperty);
            this.ClearValue(ChartAnnotationLabel.ContentProperty);
            this.ClearValue(ChartAnnotationLabel.TemplateProperty);
            this.ClearValue(ChartAnnotationLabel.OffsetYProperty);
            this.ClearValue(ChartAnnotationLabel.OffsetXProperty);
        }

        #endregion
    }
}
