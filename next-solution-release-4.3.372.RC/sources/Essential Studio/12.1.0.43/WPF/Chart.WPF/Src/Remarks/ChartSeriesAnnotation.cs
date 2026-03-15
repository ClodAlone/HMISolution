// <copyright file="ChartSeriesAnnotation.cs" company="Syncfusion">t
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Markup;

    /// <summary>
    /// Represents chart series annotation class.
    /// </summary>
    /// <remarks>
    /// Annotations at specific X-Y coordinates can be added to the chart
    /// programmatically.
    /// </remarks>
    /// <example>
    /// XAML:
    /// <code language="XAML">
    ///  &lt;sfchart:ChartSeries Name="series1" Label="Series1" Type="Area"
    /// Interior="LightSkyBlue"&gt;
    /// &lt;sfchart:ChartSeries.Annotations&gt;
    /// &lt;sfchart:AnnotationsCollection LineColor="White" x:Uid="Annot"&gt;
    ///             &lt;!-- Here we define the look and feel of the annotation. --&gt;
    ///              &lt;sfchart:AnnotationsCollection.AnnotationsTemplate&gt;
    ///                  &lt;DataTemplate&gt;
    ///                      &lt;Button Content="{Binding Y}" ToolTip="{Binding
    /// Description}" Background="LightGray" Name="Button1" Click="Button_Click" /&gt;
    ///                  &lt;/DataTemplate&gt;
    ///              &lt;/sfchart:AnnotationsCollection.AnnotationsTemplate&gt;
    ///          &lt;/sfchart:AnnotationsCollection&gt;
    ///         &lt;!-- The annotations are added to this collection in code-behind --&gt;
    ///      &lt;/sfchart:ChartSeries.Annotations&gt;
    ///  &lt;/sfchart:ChartSeries&gt;
    /// </code>
    /// C#:
    /// <code language="C#">
    /// // Series1 annotations
    /// ChartSeriesAnnotation ser1LowPoint = new ChartSeriesAnnotation() { X = 1, Y =
    /// 20, Description = "Series 1 Low Point" }; 
    /// ChartSeriesAnnotation ser1HighPoint = new ChartSeriesAnnotation() { X = 7, Y =
    /// 56, Description = "Series 1 High Point" }; 
    /// this.Chart1.Areas[0].Series[0].Annotations.Items.Add(ser1LowPoint); 
    /// this.Chart1.Areas[0].Series[0].Annotations.Items.Add(ser1HighPoint);
    /// </code>
    /// </example>
    /// <seealso cref="ChartAnnotationLabel"/>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public class ChartSeriesAnnotation : DependencyObject, INotifyPropertyChanged, IChartSerializer
    {
        #region dependencyProperties
        
        /// <summary>
        /// Identifies the X dependency property.
        /// </summary>
        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register("X", typeof(double), typeof(ChartSeriesAnnotation), new UIPropertyMetadata(0d));

        /// <summary>
        /// Identifies the Y dependency property.
        /// </summary>
        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register("Y", typeof(double), typeof(ChartSeriesAnnotation), new UIPropertyMetadata(0d));

        ///// <summary>
        ///// Identifies the Header dependency property.
        ///// </summary>
        //public static readonly DependencyProperty HeaderProperty =
        //    DependencyProperty.Register("Header", typeof(string), typeof(ChartSeriesAnnotation), new UIPropertyMetadata(String.Empty));

        /// <summary>
        /// Identifies the Description dependency property.
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(ChartSeriesAnnotation), new UIPropertyMetadata(String.Empty));

        /// <summary>
        /// Identifies the Template dependency property.
        /// </summary>

        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register("Template", typeof(DataTemplate), typeof(ChartSeriesAnnotation), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the OffsetX dependency property.
        /// </summary>
        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.Register("OffsetX", typeof(double), typeof(ChartSeriesAnnotation), new UIPropertyMetadata(0d));

        /// <summary>
        /// Identifies the OffsetY dependency property.
        /// </summary>
        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.Register("OffsetY", typeof(double), typeof(ChartSeriesAnnotation), new UIPropertyMetadata(0d));

        /// <summary>
        /// Identifies the Annotation shapes
        /// </summary>
        public static readonly DependencyProperty AnnotationShapeProperty =
        DependencyProperty.Register("AnnotationShape", typeof(AnnotationShapes), typeof(ChartSeriesAnnotation), new PropertyMetadata(AnnotationShapes.None, new PropertyChangedCallback(OnShapeChanged)));


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

            ChartSeriesAnnotation instance = (ChartSeriesAnnotation)d;

            AnnotationShapes shape = (AnnotationShapes)Enum.Parse(typeof(AnnotationShapes), e.NewValue.ToString());

            if (instance.baseRd == null)
            {
                //instance.baseRd = ChartDictionaries.GenericBaseDictionary;
                instance.baseRd = new SharedResourceDictionary()
                 {
                     Source = new Uri("/Syncfusion.Chart.Wpf;component/Remarks/AnnotationSymbolTemplates.xaml", UriKind.RelativeOrAbsolute)
                 };
            }
            instance.Template = instance.baseRd["seriestemplate" + shape.ToString()] as DataTemplate;

           
        }
        /// <summary>
        /// Identifies the Fill dependency property
        /// </summary>
        public static readonly DependencyProperty FillProperty =
           DependencyProperty.Register("Fill", typeof(Brush), typeof(ChartSeriesAnnotation), new UIPropertyMetadata(Brushes.AliceBlue));
        /// <summary>
        /// Identifies the Stroke Dependency property
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
           DependencyProperty.Register("Stroke", typeof(Brush), typeof(ChartSeriesAnnotation), new PropertyMetadata(Brushes.Black));
        /// <summary>
        /// Gets or sets the stroke.
        /// </summary>
        /// <value>The stroke.</value>
        public Brush Stroke
        {
            get
            {
                return (Brush)GetValue(StrokeProperty);
            }

            set
            {
                SetValue(StrokeProperty, value);
            }
        }

        /// <summary>
        /// Identifies the IsAnnotationDragDrop dependency property.
        /// </summary>
        public static readonly DependencyProperty IsAnnotationDragDropProperty =
      DependencyProperty.Register("IsAnnotationDragDrop", typeof(bool), typeof(ChartSeriesAnnotation), new UIPropertyMetadata(false));
        /// <summary>
        /// Gets or sets the IsAnnotationDragDrop.
        /// </summary>
        /// <value>The IsAnnotationDragDrop.</value>        
        public bool IsAnnotationDragDrop
        {
            get { return (bool)GetValue(IsAnnotationDragDropProperty); }
            set { SetValue(IsAnnotationDragDropProperty, value); }
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
        /// Gets or sets the fill.
        /// </summary>
        /// <value>The fill.</value>
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
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

        ///// <summary>
        ///// Gets or sets the Header. This is a dependency property.
        ///// </summary>
        ///// <value>The Header value.</value>
        //public string Header
        //{
        //    get { return (string)GetValue(HeaderProperty); }
        //    set { SetValue(HeaderProperty, value); }
        //}

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
        /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.DependencyObject"/> has been updated. The specific dependency property that changed is reported in the event data.
        /// </summary>
        /// <param name="e">Event data that will contain the dependency property identifier of interest, the property metadata for the type, and old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(e.Property.Name));
            }
        }
        #endregion
        
        #region Constructor
        /// <summary>
        /// Empty constructor for ChartSeriesAnnotation
        /// </summary>
        public ChartSeriesAnnotation()
        {            
            
        }
        #endregion

        #region INotifyPropertyChanged Members
        /// <summary>
        /// Occurs when any property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
        
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
