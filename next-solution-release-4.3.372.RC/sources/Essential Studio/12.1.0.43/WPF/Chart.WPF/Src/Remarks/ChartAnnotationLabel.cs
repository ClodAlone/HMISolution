// <copyright file="ChartAnnotationLabel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Markup;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Windows.Media;
    using System.ComponentModel;


    /// <summary>
    /// Represents ChartLabel class used to set custom labels on Chart
    /// </summary>
    /// <remarks>
    /// Chart for WPF also lets you add some annotations to the chart at specific
    /// control co-ordinates. By default, these annotations appear as simple text
    /// labels. But, their look and feel can be fully customized using custom templates.
    /// </remarks>
    /// <example>
    /// XAML:
    /// <code language="XAML">
    /// &lt;syncfusion:Chart Name="chart1"&gt;
    ///     &lt;!-- Template defining the custom look and feel of the annotations --&gt;
    ///     &lt;syncfusion:Chart.AnnotationLabelTemplate&gt;
    ///         &lt;DataTemplate&gt;
    ///             &lt;Border Background="MintCream" BorderBrush="Black"
    /// BorderThickness="1"&gt;
    ///                 &lt;TextBlock Text="{Binding}" Foreground="Black"
    /// FontFamily="Tahoma" FontSize="12" Margin="5"/&gt;
    ///             &lt;/Border&gt;
    ///         &lt;/DataTemplate&gt;
    ///     &lt;/syncfusion:Chart.AnnotationLabelTemplate&gt;
    ///     &lt;syncfusion:Chart.AnnotationLabels&gt;
    ///             &lt;!-- ChartAnnotationLabel instance representing the location and
    /// content of the annotation. --&gt;
    ///             &lt;syncfusion:ChartAnnotationLabel x:Name="label1" Content="Top 6
    /// Products" OffsetX="50" OffsetY="60"&gt;
    ///             &lt;/syncfusion:ChartAnnotationLabel&gt;
    ///     &lt;/syncfusion:Chart.AnnotationLabels&gt;
    /// &lt;/syncfusion:Chart&gt;
    /// </code>
    /// </example>
    /// <seealso cref="AnnotationsCollection"/>
    /// <seealso cref="ChartAnnotationLabelsCollection"/>
#if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [ContentProperty("Content")]
    public class ChartAnnotationLabel : DependencyObject, IChartSerializer
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the OffsetX dependency property.
        /// </summary>
        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.Register("OffsetX", typeof(double), typeof(ChartAnnotationLabel), new UIPropertyMetadata(0d));

        /// <summary>
        /// Identifies the OffsetY dependency property.
        /// </summary>
        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.Register("OffsetY", typeof(double), typeof(ChartAnnotationLabel), new UIPropertyMetadata(0d));

        /// <summary>
        /// Identifies the Content dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(ChartAnnotationLabel), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the Template dependency property.
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
              DependencyProperty.Register("Template", typeof(DataTemplate), typeof(ChartAnnotationLabel), new UIPropertyMetadata(null));
        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>The template.</value>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public DataTemplate Template
        {
            get { return (DataTemplate)GetValue(TemplateProperty); }
            set { SetValue(TemplateProperty, value); }
        }
        /// <summary>
        /// Identifies the AnnotationShape dependency property.
        /// </summary>
        public static readonly DependencyProperty AnnotationShapeProperty =
            DependencyProperty.Register("AnnotationShape", typeof(AnnotationShapes), typeof(ChartAnnotationLabel), new PropertyMetadata(AnnotationShapes.Rectangle, new PropertyChangedCallback(OnShapeChanged)));


        /// <summary>
        /// Gets or sets the annotation shape.
        /// </summary>
        /// <value>The annotation shape.</value>
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

        ResourceDictionary baseRd = null;
        private static void OnShapeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            ChartAnnotationLabel instance = (ChartAnnotationLabel)d;


            AnnotationShapes shape = (AnnotationShapes)Enum.Parse(typeof(AnnotationShapes), e.NewValue.ToString());

            if (instance.baseRd == null)
            {
                //instance.baseRd = ChartDictionaries.GenericBaseDictionary;
                instance.baseRd = new SharedResourceDictionary()
                {
                    Source = new Uri("/Syncfusion.Chart.Wpf;component/Remarks/AnnotationSymbolTemplates.xaml", UriKind.RelativeOrAbsolute)
                };
            }

            instance.Template = instance.baseRd["template" + shape.ToString()] as DataTemplate;
        }

        /// <summary>
        /// Identifies the Fill dependency property
        /// </summary>
        public static readonly DependencyProperty FillProperty =
               DependencyProperty.Register("Fill", typeof(Brush), typeof(ChartAnnotationLabel), new PropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Gets or sets the fill.
        /// </summary>
        /// <value>The fill.</value>
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        /// <summary>
        /// Dependency property for check AnnotationDragDrop or not
        /// </summary>
        public static readonly DependencyProperty IsAnnotationDragDropProperty =
          DependencyProperty.Register("IsAnnotationDragDrop", typeof(bool), typeof(ChartAnnotationLabel), new UIPropertyMetadata(false));
        /// <summary>
        /// Gets or sets the IsAnnotationDragDrop.
        /// </summary>
        /// <value>The IsAnnotationDragDrop.</value>        
        public bool IsAnnotationDragDrop
        {
            get { return (bool)GetValue(IsAnnotationDragDropProperty); }
            set { SetValue(IsAnnotationDragDropProperty, value); }
        }

        internal static readonly DependencyProperty ContentMarginProperty =
            DependencyProperty.Register("ContentMargin", typeof(Thickness), typeof(ChartAnnotationLabel), new UIPropertyMetadata(new Thickness(0,15,0,15)));
        /// <summary>
        /// Gets or sets the Margin
        /// </summary>
        /// <value>The ContentMargin.</value>        
        internal Thickness ContentMargin
        {
            get { return (Thickness)GetValue(ContentMarginProperty); }
            set { SetValue(ContentMarginProperty, value); }
        }


        #region members        
        internal ChartAnnotationsPresenter presenter = null;
        internal Chart AnnotationChart = null;
        internal Rect intersectRect = new Rect();
        #endregion
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
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAnnotationLabel"/> class.
        /// </summary>
        public ChartAnnotationLabel()
        {
            AnnotationShape = AnnotationShapes.None;
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
            AnnotationShape = AnnotationShapes.None;
        }

        internal void Dispose()
        {
            this.Content = null;
            this.Template = null;
        }
        #endregion

        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.DependencyObject"/> has been updated. The specific dependency property that changed is reported in the event data. 
        /// </summary>
        /// <param name="e">Event data that will contain the dependency property identifier of interest, the property metadata for the type, and old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
			//Calls the InvalidateMeasure on OffsetX and OffsetY property are changed
            if (e.Property == ChartAnnotationLabel.OffsetXProperty || e.Property == ChartAnnotationLabel.OffsetYProperty)
            {
                if (presenter != null)
                {
                    presenter.InvalidateMeasure();
                }
            }
            base.OnPropertyChanged(e);
        } 

        #region IChartSerializer Members

        /// <summary>
        /// Method declaration for Serialize
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            string _xamlString;
            _xamlString = XamlWriter.Save(this);
            return _xamlString;
        }

        /// <summary>
        /// Method declaration for DeSerialize
        /// </summary>
        /// <param name="xamlString"></param>
        /// <returns></returns>
        public object Deserialize(string xamlString)
        {
            return XamlReader.Parse(xamlString);
        }

        #endregion
    }
}
