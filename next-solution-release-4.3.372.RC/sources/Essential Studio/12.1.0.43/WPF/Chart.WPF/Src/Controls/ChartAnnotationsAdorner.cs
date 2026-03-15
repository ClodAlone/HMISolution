// <copyright file="ChartAnnotationsAdorner.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Documents;
    using System.Collections.Specialized;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Controls;
    using System.Collections;
    using System.Windows.Data;
    using System.ComponentModel;
    using System.Globalization;

  /// <summary>
  /// Represent the <see cref="ChartSeries"/> annotations class.
  /// </summary>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    internal class ChartAnnotationsAdorner : Adorner,IDisposable 
  {
    #region Members
    /// <summary>
    /// Represents adorner's UI elements.
    /// </summary>
   private WrappedAnnotationCollection m_elements;

    /// <summary>
    /// Represents dicrionary record that holds line coordinates that correspond to each annotaion.
    /// </summary>
    public Dictionary<ChartSeriesAnnotation, Point[]> m_flagLines = new Dictionary<ChartSeriesAnnotation, Point[]>();

    /// <summary>
    /// Represents adorned ChartArea.
    /// </summary>
    public ChartArea m_area;
	//To identify the point at which the Annotation is dragged
    private Point DraggingPoint = new Point();
	//To identify wheather the Annotation is dragged
    private bool isDragging = false;
	//To identify the Annotation being dragged
    private ChartSeriesAnnotation DraggedSeriesAnnotation = null;
    #region Internal Classes
    /// <summary>
    /// Represents UIElementCollection class that is extended to hold annotaions and notify about collection changes.
    /// </summary>
    private class WrappedAnnotationCollection : UIElementCollection, INotifyCollectionChanged
    {
        #region Members
        /// <summary>
        /// Represents template converter used to provide proper template value for wrapped anotation.
        /// </summary>
        private AnnotationsTemplateConverter m_templateConverter;

        /// <summary>
        /// Provides selection for Annotation's template. As per behavior requirements,
        /// Annotaion should either use it's own (non null) DataTemplate or use template declared in parent collection.
        /// </summary>
        private class AnnotationsTemplateConverter : IMultiValueConverter
        {
            #region IMultiValueConverter Members

            /// <summary>
            /// Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.
            /// </summary>
            /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding"/> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the source binding has no value to provide for conversion.</param>
            /// <param name="targetType">The type of the binding target property.</param>
            /// <param name="parameter">The converter parameter to use.</param>
            /// <param name="culture">The culture to use in the converter.</param>
            /// <returns>
            /// A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty"/>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> if it is available, or else will use the default value.A return value of <see cref="T:System.Windows.Data.Binding"/>.<see cref="F:System.Windows.Data.Binding.DoNothing"/> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> or the default value.
            /// </returns>
            /// <seealso cref="AnnotationsTemplateConverter"/>
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                ////Retrieve parent and self Data templates.
                DataTemplate parentTemplate = values[0] as DataTemplate;
                DataTemplate selfTemplate = values[1] as DataTemplate;
                ////If current annotation's template is null, parent's template is to be selected.
                return (selfTemplate == null) ? parentTemplate : selfTemplate;
            }

            /// <summary>
            /// Converts a binding target value to the source binding values.
            /// </summary>
            /// <param name="value">The value that the binding target produces.</param>
            /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
            /// <param name="parameter">The converter parameter to use.</param>
            /// <param name="culture">The culture to use in the converter.</param>
            /// <returns>
            /// An array of values that have been converted from the target value back to the source values.
            /// </returns>
            /// <seealso cref="AnnotationsTemplateConverter"/>
            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotSupportedException("Not supported - ConvertBack should never be called in a OneWay Binding");
            }

            #endregion
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WrappedAnnotationCollection"/> class.
        /// </summary>
        /// <param name="visualParent">The <see cref="T:System.Windows.UIElement"/> parent of the collection.</param>
        /// <param name="logicalParent">The logical parent of the elements in the collection.</param>
        public WrappedAnnotationCollection(UIElement visualParent, FrameworkElement logicalParent)
            : base(visualParent, logicalParent)
        {
            m_templateConverter = new AnnotationsTemplateConverter();            
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Adds the specified annotation and wraps it to <see cref="ContentPresenter"/> internally.
        /// </summary>
        /// <param name="annotation">The annotation.</param>
        /// <seealso cref="WrappedAnnotationCollection"/>
        public void Add(ChartSeriesAnnotation annotation)
        {
            ////Wrap the annotation.
            UIElement ui = CreateContatinerFromAnnotation(annotation);
            ////Call native Add method in order to add it.
            Add(ui);
            
            ////Raise CollectionChanged event in order to about notify collection changes.
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new ArrayList() { ui }));
            }
        }

        
        /// <summary>
        /// Removes the specified annotation from wrapped annotations collection.
        /// </summary>
        /// <param name="annotation">The annotation.</param>
        public void Remove(ChartSeriesAnnotation annotation)
        {
            ////Search for annotation among all UIElements.
            foreach (UIElement ui in this)
            {
                if ((ui as ContentPresenter).Content == annotation)
                {
                    ////Call native Remove method in order to remove requested annotation.
                    Remove(ui);
                    ////Raise CollectionChanged event in order to about notify collection changes.
                    if (CollectionChanged != null)
                    {
                        CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new ArrayList() { ui }));
                    }
                    ////Stop the search.
                    break;
                }
            }
        }

        /// <summary>
        /// Creates the <see cref="ContentPresenter"/> container for annotation.
        /// </summary>
        /// <param name="annotation">The annotation.</param>
        /// <returns><see cref="ContentPresenter"/> that holds annotation.</returns>
        private ContentPresenter CreateContatinerFromAnnotation(ChartSeriesAnnotation annotation)
        {
            ////Retrieving parent annotation series as attached dependency property.
            ChartSeries ownerSeries = (ChartSeries)GetParentSeries(annotation);
            ////Creating new container for annotation.
            ContentPresenter container = new ContentPresenter();
            ////Selecting Content.
            container.Content = annotation;

            ////Binds ContentTemplate property of contatiner to parent template and self template via
            ////the MultiBinding in order to reflect template changes.

            MultiBinding multiBinding = new MultiBinding();

            Binding parentBinding = new Binding("AnnotationsTemplate");
            parentBinding.Source = ownerSeries.Annotations;

            Binding annotationBinding = new Binding("Template");
            annotationBinding.Source = annotation;

            multiBinding.Bindings.Add(parentBinding);
            multiBinding.Bindings.Add(annotationBinding);
            ////Assigning converter to select one of templates.
            multiBinding.Converter = m_templateConverter;
            BindingOperations.SetBinding(container, ContentPresenter.ContentTemplateProperty, multiBinding);

            return container;
        }
        #endregion

        #region INotifyCollectionChanged Members
        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

 
        #endregion
    }
    #endregion

    #endregion

    #region Properties
    /// <summary>
    /// Gets the number of visual child elements within this element.
    /// </summary>
    /// <value></value>
    /// <returns>The number of visual child elements for this element.</returns>
    protected override int VisualChildrenCount
    {
        get
        {
            return m_elements.Count;
        }
    }

    /// <summary>
    /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)"/>, and returns a child at the specified index from a collection of child elements.
    /// </summary>
    /// <param name="index">The zero-based index of the requested child element in the collection.</param>
    /// <returns>
    /// The requested child element. This should not return null; if the provided index is out of range, an exception is thrown.
    /// </returns>
    protected override Visual GetVisualChild(int index)
    {
        return m_elements[index];
    }
    #endregion

    #region Dependency Properties
    /// <summary>
    /// Identifies the ParentSeries attached dependency property.
    /// </summary>
    private static readonly DependencyProperty ParentSeriesProperty =
        DependencyProperty.RegisterAttached("ParentSeries", typeof(ChartSeries), typeof(ChartAnnotationsAdorner), new FrameworkPropertyMetadata(null));
    #endregion

    #region Constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartAnnotationsAdorner"/> class.
    /// </summary>
    /// <param name="adornedElement">The adorned element.</param>
    /// <param name="area">The area value.</param>
    public ChartAnnotationsAdorner(UIElement adornedElement, ChartArea area)
        : base(adornedElement)
    {
        m_elements = new WrappedAnnotationCollection(this, this);
        m_area = area;
        m_elements.CollectionChanged += new NotifyCollectionChangedEventHandler(OnWrappedAnnotationsCollectionChanged);
        Initialize();      
    }

    #endregion

    #region Implementation

    #region Collections changes handling methods
    /// <summary>
    /// Handles wrapped annotation collection changes.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
    private void OnWrappedAnnotationsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        ////Retrieving changed items.
        IList changedItems = (e.NewItems == null) ? e.OldItems : e.NewItems;
        ////Retrieving annotation.
        ChartSeriesAnnotation annotation = (changedItems[0] as ContentPresenter).Content as ChartSeriesAnnotation;
        ////Invalidating adorner in order to redraw flag lines. Arranging of adorner's elements is handeled automatically.
        if (annotation != null)
        {
            ////Determining whether line for changed annotation should be redrawn.
            if (annotation.OffsetX != 0 || annotation.OffsetY != 0)
            {
                this.InvalidateVisual();
            }
        }
    }
            
    
      /// <summary>
    /// Called when when adorned area's seires collection changes
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
    private void OnAdornedSeriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        ////New seires were added.
        if (e.NewItems != null)
        {
            ////Iterate over added series and add annotations if any exist.
            foreach (ChartSeries series in e.NewItems)
            {
                ////Subscribing for PropertyChanges on series.
                series.PropertyChanged += new DependencyPropertyChangedEventHandler(OnAdornedSeriesPropertyChanged);
                ////Adding annotations on series.
                AddAnnotations(series);
            }
        }
        ////Series were removed.
        if (e.OldItems != null)
        {
            ////Iterate over removed series and remove annotations if any exist.
            foreach (ChartSeries series in e.OldItems)
            {
                ////Unsubscribing for PropertyChanges on series.
                series.PropertyChanged -= new DependencyPropertyChangedEventHandler(OnAdornedSeriesPropertyChanged);
                ////Removing annotations on series.
                RemoveAnnotations(series);
            }
        }
        ////If series collection was cleared.
        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            ////Unsubscribing for PropertyChanges on all series.
            foreach (UIElement ui in m_elements)
            {
                GetParentSeries((ui as ContentPresenter).Content as ChartSeriesAnnotation).PropertyChanged -= new DependencyPropertyChangedEventHandler(OnAdornedSeriesPropertyChanged);
            }
            ////Removing all annotations.
            m_elements.Clear();
            ////Removing all flag lines.
            m_flagLines.Clear();
            ////Refreshing.
            InvalidateVisual();
        }
    }
   
      /// <summary>
    /// Called when adorned series annotations collection was changed.
    /// </summary>
   
    public void AnnotationReset()
    {
        if (m_elements == null)
        {
            return;
        }

        List<int> removedElementsIndexes = new List<int>(m_elements.Count);
        ////Saving disappeared annotations positions.
        for (int i = 0; i < m_elements.Count; i++)
        {
            if (!(m_elements[i] is ContentPresenter) || m_flagLines == null)
            {
                continue;
            }

            ////Retrieving annotation from UIelement
            ChartSeriesAnnotation annotation = (ChartSeriesAnnotation)(m_elements[i] as ContentPresenter).Content;
            ChartSeries parentSeries = GetParentSeries(annotation);
            ////Making sure annotaion's wrapper should be removed.
            if ((parentSeries != null && parentSeries.Annotations.Items.Count == 0) ||
                (parentSeries != null && m_area != null && m_area.Series != null && !m_area.Series.Contains(parentSeries)))
            {
                ////Saving position of element to be removed.
                removedElementsIndexes.Add(i);
                ////Unsubscribe from propety changed event.
                annotation.PropertyChanged -= new PropertyChangedEventHandler(OnAnnotationPropertyChanged);
                ////Removing corresponding flag line.
                m_flagLines.Remove(annotation);
            }
        }
        ////Iterating over indexed of elements to remove and removing them from the end.
        for (int i = removedElementsIndexes.Count - 1; i >= 0; i--)
        {
            m_elements.RemoveAt(removedElementsIndexes[i]);
        }
        ////Repainting itself.
        InvalidateVisual();
        return;
    }
    private void OnAdornedSeriesAnnotationsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            AnnotationReset();
        }
        ////If items were removed.
        if (e.OldItems != null)
        {
            ////Iterate over removed items.
            foreach (ChartSeriesAnnotation annotation in e.OldItems)
            {
                ////Remove annotation line flag.
                m_flagLines.Remove(annotation);
                ////Unsubscribe from propety changed event.
                annotation.PropertyChanged -= new PropertyChangedEventHandler(OnAnnotationPropertyChanged);
                ////Remove annotation from adorner's UIElements.
                m_elements.Remove(annotation);
            }
        }
        ////If new annotations were added.
        if (e.NewItems != null)
        {
            ChartSeries parentSeries = null;
            ////Search for series that holds changed annotations.
            foreach (ChartSeries series in m_area.Series)
            {
                if (series.Annotations != null && series.Annotations.Items == sender)
                {
                    parentSeries = series;
                    break;
                }
            }
            ////Adding annotations to wrapped uielemtn collection.
            foreach (ChartSeriesAnnotation annotation in e.NewItems)
            {
                ////Setting series as attachend property for annotation.
                SetParentSeries(annotation, parentSeries);
                ////Subscribing for annotation's property changes.
                annotation.PropertyChanged += new PropertyChangedEventHandler(OnAnnotationPropertyChanged);
                ////Adding annotation.
                m_elements.Add(annotation);
            }
        }   
    }
    #endregion
	//Event Handler for MouseLeftButtonDown
    protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
    {
        ChartSeriesAnnotation annot = ((System.Windows.Controls.ContentPresenter)(e.Source)).Content as ChartSeriesAnnotation;
        if (annot != null)
        {
			//Check wheather the property IsAnnotationDragDrop is set to drag the Annotation
            if (annot.IsAnnotationDragDrop)
            {
                DraggedSeriesAnnotation = annot;
                this.isDragging = true;
                Point pt = e.GetPosition(this);// sets the point at which the Annotation is being dragged
                this.DraggingPoint = new Point(this.m_area.PointToValue(m_area.PrimaryAxis, new Point(pt.X + m_area.AxesThickness.Left, pt.Y)) - annot.X, this.m_area.PointToValue(m_area.SecondaryAxis, new Point(pt.X, pt.Y + m_area.AxesThickness.Bottom)) - annot.Y);// converts the point to axis Value
                this.CaptureMouse();
            }
        }
        base.OnMouseLeftButtonDown(e);
    }
	//Event Handler for MouseMove
    protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
    {
        if(this.isDragging)
        {
            ChartSeriesAnnotation annot = this.DraggedSeriesAnnotation as ChartSeriesAnnotation;
            if (annot != null && annot.IsAnnotationDragDrop)
            {
				//Gets the Point where the Anotation is being moved
                Point pt = e.GetPosition(this);
                annot.X = this.m_area.PointToValue(m_area.PrimaryAxis, new Point(pt.X+m_area.AxesThickness.Left,pt.Y)) - DraggingPoint.X;// sets the calculated value for the X property for the Annotation
                annot.Y = this.m_area.PointToValue(m_area.SecondaryAxis, new Point(pt.X, pt.Y + m_area.AxesThickness.Bottom)) - DraggingPoint.Y; //Sets the calculated value for the Y Property for the Annotation
                
            }
        }
        base.OnMouseMove(e);
    }
	//Event Handler for MouseLeftButtonUp
    protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
    {
        if (this.isDragging)
        {
			//Release the Annotation that has been dragged
            this.isDragging = false;
            this.ReleaseMouseCapture();
        }
        base.OnMouseLeftButtonUp(e);
    }
    /// <summary>
    /// Initializes the adorner. Retrieves all annotations from existing series and subscribes to necessary events.
    /// </summary>
    private void Initialize()
    {
        ////Subscribing to adorned area's series collection changes.
        m_area.Series.CollectionChanged += new NotifyCollectionChangedEventHandler(OnAdornedSeriesCollectionChanged);
        ////Iterating over all series in area and subscribing to property changed event.
        foreach (ChartSeries series in m_area.Series)
        {
            series.PropertyChanged += new DependencyPropertyChangedEventHandler(OnAdornedSeriesPropertyChanged);
          
            if (series.Annotations != null)
            {
                series.Annotations.PropertyChanged += new PropertyChangedEventHandler(OnSeriesAnnotationsPropertyChanged);
                series.Annotations.Items.CollectionChanged += new NotifyCollectionChangedEventHandler(OnAdornedSeriesAnnotationsCollectionChanged);
                foreach (ChartSeriesAnnotation annotation in series.Annotations.Items)
                {
                    SetParentSeries(annotation, series);
                    annotation.PropertyChanged += new PropertyChangedEventHandler(OnAnnotationPropertyChanged);                    
                    m_elements.Add(annotation);                    
                }
            }
        }        
    }
    
    /// <summary>
    /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
    /// </summary>
    /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
    protected override void OnRender(DrawingContext drawingContext)
    {
        ////Iterating over flag lines dictionary.
        foreach (ChartSeriesAnnotation annotation in m_flagLines.Keys)
        {
            ////Retrieving parent series from annotation.
            ChartSeries parentSeries = (ChartSeries)GetParentSeries(annotation);
            ////Drawing connecting line.
            drawingContext.DrawLine(new Pen(parentSeries.Annotations.LineColor, 1), m_flagLines[annotation][0], m_flagLines[annotation][1]);
        }

        base.OnRender(drawingContext);
    }

    /// <summary>
    /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
    /// </summary>
    /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
    /// <returns>The actual size used.</returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
        ////Retrieving current adorner's boundary.
        Rect boundaries = new Rect(0, 0, ActualWidth, ActualHeight);
        IChartTransformer transformer = null;
        ChartSeries cachedSeries = null;
        Point originalPosition = new Point();
        ////Iterating over ui elements in order to arrange them.
        foreach (UIElement element in m_elements)
        {
            ContentPresenter wrappedAnnotation = element as ContentPresenter;
            ////Unwrapping an annotation.
            ChartSeriesAnnotation annotation = (ChartSeriesAnnotation)wrappedAnnotation.Content;
            ////Retrieving parent series.
            ChartSeries series = (ChartSeries)GetParentSeries(annotation);
            if (cachedSeries != series)
            {
                cachedSeries = series;
                transformer = ChartTransform.CreateCartesian(new Rect(new Point(0, 0), finalSize), series);
            }

            if (series.Annotations.IsRelative)
            {
                ////Calculating annotation's position.
                originalPosition = transformer.TransformToVisible(annotation.X, annotation.Y);
                originalPosition.X += annotation.OffsetX;
                originalPosition.Y += annotation.OffsetY;
            }
            else
            {
                ////Annotation will be placed in absolute coordinates of adorner, regardless chart type and axes type.
                originalPosition.X = annotation.X + annotation.OffsetX;
                originalPosition.Y = annotation.Y + annotation.OffsetY;
            }
            ////Verifying whether element's position falls into adorner's boundary.
            if (boundaries.Contains(originalPosition))
            {
                ////If offsets are set for annotation.
                if (annotation.OffsetX != 0 || annotation.OffsetY != 0)
                {
                    ////Creating shifted position.
                    Point shiftedPosition = new Point(originalPosition.X, originalPosition.Y);
                    shiftedPosition.X -= annotation.OffsetX;
                    shiftedPosition.Y -= annotation.OffsetY;
                    ////Verifying whether element's shifted position falls into adorner's boundary.
                    if (boundaries.Contains(shiftedPosition))
                    {
                        if (m_flagLines.ContainsKey(annotation))
                        {
                            m_flagLines.Remove(annotation);
                        }
                        ////Adding annotation and coordinated to draw connecting line.
                        m_flagLines.Add(annotation, new Point[] { originalPosition, shiftedPosition });
                    }
                    else
                    {
                        if (m_flagLines.ContainsKey(annotation))
                        {
                            m_flagLines.Remove(annotation);
                        }
                    }
                }

                if (element.Visibility == Visibility.Collapsed)
                {
                    element.Visibility = Visibility.Visible;
                }
                ////Arranging element to a new position.
                element.Arrange(new Rect(originalPosition.X - element.DesiredSize.Width / 2, originalPosition.Y - element.DesiredSize.Height / 2, element.DesiredSize.Width, element.DesiredSize.Height));
            }
            else
            {
                ////Removing element's line from dictionary.
                m_flagLines.Remove(annotation);
                ////Collapsing the element.
                if (element.Visibility == Visibility.Visible)
                {
                    element.Visibility = Visibility.Collapsed;
                }
            }
        }

        return base.ArrangeOverride(finalSize);
    }
//To set the IntersectAction for the Annotation
    private void DoLayout(bool isMeasuring, ChartSeries series)
    {
        if (series != null)
        {
            switch (series.SeriesAnnotationIntersectAction)
            {
                case AnnotationIntersectActions.Hide:
                    {
						//Codes for the IntersectAction Hide
                        ObservableCollection<Rect> rect = new ObservableCollection<Rect>();
                        for(int i= this.m_elements.Count -1 ; i>=0; i--)
                        {
                            ContentPresenter wrappedAnnotation = this.m_elements[i] as ContentPresenter;
                            //Unwrapping an annotation.
                            ChartSeriesAnnotation annotation = (ChartSeriesAnnotation)wrappedAnnotation.Content;
							//Rect to determine the Actual position, Width and Height of the Annotation
                            Rect bounds = new Rect(new System.Windows.Size(wrappedAnnotation.ActualWidth, wrappedAnnotation.ActualHeight));
                            bounds.X = this.m_area.ValueToPoint(series.XAxis, annotation.X);
                            bounds.Y = this.m_area.ValueToPoint(series.YAxis, annotation.Y);
                            if (rect.Count > 0)
                            {
								//Checks the condition to add the Rect of the Annotation to the collection, if it is not added to the collection
                                if (!rect.Contains(bounds))
                                {
                                    rect.Add(bounds);
                                }
                                bool isVisibilitySet = false;
                                foreach (Rect prerect in rect)
                                {
									//Checks wheather the Annotations gets overlapped
                                    if (prerect != bounds && bounds.IntersectsWith(prerect) && !isMeasuring)
                                    {
										//Sets the Visibility for the Annotation
                                        wrappedAnnotation.Visibility = Visibility.Hidden;
                                        isVisibilitySet = true;
                                    }
                                    else if (!isMeasuring && !isVisibilitySet)
                                    {
										//Sets the Visibility for the Annotation
                                        wrappedAnnotation.Visibility = Visibility.Visible;
                                    }
                                }

                            }
                            else
                            {
                                if (!isMeasuring)
                                {
										//Sets the Visibility for the Annotation
                                    wrappedAnnotation.Visibility = Visibility.Visible;
                                }
                                rect.Add(bounds);
                            }
                        }
                    }                   
                    break;
                case AnnotationIntersectActions.None:
                    {
                        for (int i = this.m_elements.Count - 1; i >= 0; i--)
                        {
                            //gets the each element of the Annotation Adorner.
                            ContentPresenter wrappedAnnotation = this.m_elements[i] as ContentPresenter;
                            wrappedAnnotation.Visibility = System.Windows.Visibility.Visible;
                        }
                    }
                    break;
            }
        }
    }

      /// <summary>
      /// Adds the annotations from specified series..
      /// </summary>
      /// <seealso>
      ///     <cref>ChartAnnotationAdorner</cref>
      /// </seealso>
      /// <param name="series">The series.</param>
      private void AddAnnotations(ChartSeries series)
    {
        if (series.Annotations != null)
        {
            ////Iterating over all annotations assigned to series.
            foreach (ChartSeriesAnnotation annotation in series.Annotations.Items)
            {
                ////Setting parent series for the annotation.
                SetParentSeries(annotation, series);
                ////Subscribing to property changes on annotation.
                annotation.PropertyChanged += new PropertyChangedEventHandler(OnAnnotationPropertyChanged);
                ////Adding annotation to adorner's children.
                m_elements.Add(annotation);
            }
        }
    }

            
    /// <summary>
    /// Removes the annotations.
    /// </summary>
    /// <param name="series">The series.</param>
    private void RemoveAnnotations(ChartSeries series)
    {
        if (series.Annotations != null)
        {
            foreach (ChartSeriesAnnotation annotation in series.Annotations.Items)
            {
               ////Removing flag line from the lines dictionary.
                m_flagLines.Remove(annotation);
                ////Unsubscribing to property changes on annotation.
                annotation.PropertyChanged -= new PropertyChangedEventHandler(OnAnnotationPropertyChanged);
                ////Setting parent series for the annotation in order to make Remove method work correctly.
                SetParentSeries(annotation, series);
                ////Adding annotation from adorner's children.
                m_elements.Remove(annotation);
            }
        }
    }

    /// <summary>
    /// Handles annotations property changes.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
    private void OnAnnotationPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
		//Calls the DoLayout to set the IntersectAction when the X property and Y property of the Annotation is changed
        if (e.PropertyName == ChartSeriesAnnotation.XProperty.ToString() || e.PropertyName == ChartSeriesAnnotation.YProperty.ToString())
        {
            this.DoLayout(false, (ChartSeries)GetParentSeries(sender as ChartSeriesAnnotation));
        }
        ////Invalidates adorner in order to reflect changes made to annotation.
        this.InvalidateVisual();
    }

    /// <summary>
    /// Called when adorned series property changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    private void OnAdornedSeriesPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        ChartSeries series = sender as ChartSeries;
        if (e.Property == ChartSeries.AnnotationsProperty && series != null)
        {
            if (e.OldValue != null)
            {
                if (series.Annotations != null)
                {
                    series.Annotations.Items.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnAdornedSeriesAnnotationsCollectionChanged);
                    series.Annotations.PropertyChanged -= new PropertyChangedEventHandler(OnSeriesAnnotationsPropertyChanged);
                }
                RemoveAnnotations(series);
            }

            if (e.NewValue != null)
            {
                series.Annotations.Items.CollectionChanged += new NotifyCollectionChangedEventHandler(OnAdornedSeriesAnnotationsCollectionChanged);
                series.Annotations.PropertyChanged += new PropertyChangedEventHandler(OnSeriesAnnotationsPropertyChanged);
                AddAnnotations(series);
            }

        }
		//Calls the DoLayout to sets the IntersectAction when the IntersectAction property changes
        if ((e.Property == ChartSeries.SeriesAnnotationIntersectActionProperty) && series != null)
        {
            this.DoLayout(false, series);
        }
    }
 
  
    /// <summary>
    /// Called when [series annotations property changed].
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
    private void OnSeriesAnnotationsPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        ////Invalidating this visual due to any Annotation property changes requires complete redraw pass.
        InvalidateVisual();
    }

    /// <summary>
    /// Gets the parent series.
    /// </summary>
    /// <param name="annotation">The annotation.</param>
    /// <returns>The parent series</returns>
    private static ChartSeries GetParentSeries(ChartSeriesAnnotation annotation)
    {
        return (ChartSeries)annotation.GetValue(ParentSeriesProperty);
    }

    /// <summary>
    /// Sets the parent series.
    /// </summary>
    /// <param name="annotation">The annotation.</param>
    /// <param name="series">The series.</param>
    private static void SetParentSeries(ChartSeriesAnnotation annotation, ChartSeries series)
    {
        annotation.SetValue(ParentSeriesProperty, series);
    }
    #endregion

    #region IDisposable Members

    public void Dispose()
    {
        this.m_area = null;
        //this.m_elements.Clear();
        //this.m_elements = null;
        //this.m_flagLines.Clear();
        //this.m_flagLines = null;
    }

    #endregion
  }
}