// <copyright file="ChartAnnotationsPresenter.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Windows.Controls;
    using System.Windows;
    using System.Windows.Data;
    using System.Drawing;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents annotations items control. 
    /// </summary>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public class ChartAnnotationsPresenter : ItemsControl, IDisposable, IChartSerializer
    {
      
        #region Fields

        /// <summary>
        /// The IMultiValueConverter TemplateSwitcher
        /// </summary>
        private IMultiValueConverter m_templateSwitcher = new TemplateSwitcher();
        #endregion

        #region Members
        private bool isDragDrop = false;
        private System.Windows.Point DraggingPoint = new System.Windows.Point();
        private ChartAnnotationLabel SelectedLabel = null;
        internal Chart ParentChart = null;
        #endregion
        #region Implementation

        #region Events
        /// <summary>
        /// Event for ChartAnnoationDragged
        /// </summary>
        public event ChartAnnotationDragEventHandler ChartAnnoationDragged;
		//To invoke the Annotation Dragged event
        internal void OnChartAnnotationDragged(ChartAnnotationDragEventArgs args)
        {
            if (ChartAnnoationDragged != null)
            {
                ChartAnnoationDragged(this, args);
            }
        }

        /// <summary>
        /// Event for ChartAnnotationDragging
        /// </summary>
        public event ChartAnnotationDragEventHandler ChartAnnotationDragging;
		//To invoke the event for the Dragging Annotation
        internal void OnChartAnnotationDragging(ChartAnnotationDragEventArgs args)
        {
            if (ChartAnnotationDragging != null)
            {
                ChartAnnotationDragging(this, args);
            }
        }

        /// <summary>
        /// Event for ChartAnnotationDropped
        /// </summary>
        public event ChartAnnotationDropEventHandler ChartAnnotationDropped;
		//To invoke the Annotation Dropped event
        internal void OnChartAnnoationDropped(ChartAnnotationDropEventArgs args)
        {
            if (ChartAnnotationDropped != null)
            {
                ChartAnnotationDropped(this, args);
            }
        }

        /// <summary>
        /// Event for ChartAnnotationDropping
        /// </summary>
        public event ChartAnnotationDropEventHandler ChartAnnotationDropping;
		//To invoke the Annotation Dropping event
        internal void OnChartAnnoationDropping(ChartAnnotationDropEventArgs args)
        {
            if (ChartAnnotationDropping != null)
            {
                ChartAnnotationDropping(this, args);
            }
        }
        #endregion

		//Event Handler for the MouseLeftButtonDown
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonDown"/> routed event is raised on this element. Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.Items.Count > 0)
            {
                FrameworkElement fElement = e.Device.Target as FrameworkElement;
                if (fElement != null && fElement.DataContext is ChartAnnotationLabel)
                {
                    ChartAnnotationLabel annot = fElement.DataContext as ChartAnnotationLabel;
					//Checks wheather the condition is set for Annotation Drag and Drop
                    if (annot != null && annot.IsAnnotationDragDrop)
                    {
						//To set the Dragging Point
                        this.DraggingPoint = new System.Windows.Point(e.GetPosition(this).X - annot.OffsetX, e.GetPosition(this).Y - annot.OffsetY);
                        this.SelectedLabel = annot;// To set the annotaion being dragged
                        this.isDragDrop = true;
                        this.CaptureMouse();
                        this.OnChartAnnotationDragging(new ChartAnnotationDragEventArgs(annot));
                    }
                }
            }
            base.OnMouseLeftButtonDown(e);
        }
		//Event for the MouseMove
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseMove"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            if (this.isDragDrop == true)
            {
                ChartAnnotationLabel annot = this.SelectedLabel;
				//Checks the Condition for Annotation Drag and Drop
                if (annot != null && annot.IsAnnotationDragDrop)
                {
                    System.Windows.Point pt = e.GetPosition(this);//Gets the current position to which the Annotation is moved
                    annot.OffsetX = e.GetPosition(this).X - DraggingPoint.X;// Sets the calculated value for the OffsetX property
                    annot.OffsetY = e.GetPosition(this).Y - DraggingPoint.Y;// Sets the calculated value for the OffsetY property
                    this.OnChartAnnotationDragged(new ChartAnnotationDragEventArgs(annot));
                    this.OnChartAnnoationDropping(new ChartAnnotationDropEventArgs(annot));
                }
            }
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseUp"/> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the mouse button was released.</param>
        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.isDragDrop == true)
            {
                ChartAnnotationLabel annot = this.SelectedLabel;
                this.isDragDrop = false;
				//Release the Annotation which is being dragged
                this.ReleaseMouseCapture();
                this.OnChartAnnoationDropped(new ChartAnnotationDropEventArgs(annot));
                this.SelectedLabel = null;
            }
            base.OnMouseUp(e);
        }

		//Measures the original size of the constraint
        /// <summary>
        /// Called to remeasure a control. 
        /// </summary>
        /// <returns>
        /// The size of the control, up to the maximum specified by <paramref name="constraint"/>.
        /// </returns>
        /// <param name="constraint">The maximum size that the method can return.</param>
        protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
        {
			//Calls the DoLayout method for the Intersect Action
            return base.MeasureOverride(this.DoLayout(constraint, false));
        }
        private System.Windows.Size DoLayout(System.Windows.Size availabeSize, bool isMeasuring)
        {
            if (this.ParentChart != null)
            {
                switch (this.ParentChart.AnnotationIntersectAction)
                {
                    //IntersectAction None
                    case AnnotationIntersectActions.None:
                        {
                            for (int i = (this.Items.Count - 1); i >= 0; i--)
                            {
                                //Gets the Canvas of the AnnotationPresenter whcih includes all the Items of the AnnotationPresenter
                                DependencyObject obj = VisualTreeHelper.GetChild(this, 0);
                                obj = VisualTreeHelper.GetChild(obj, 0);
                                obj = VisualTreeHelper.GetChild(obj, 0);
                                Canvas canvas = obj as Canvas;
                                //Checks for Object reference 
                                if (canvas != null)
                                {
                                    //ContentPresenter of the Annotation 
                                    ContentPresenter element = canvas.Children[i] as ContentPresenter;
                                    element.Visibility = Visibility.Visible;//Sets the Visibility of the Annotation
                                }
                            }
                        }
                        break;
					//IntersectAction Hide
                    case AnnotationIntersectActions.Hide:
                        {
                            ObservableCollection<Rect> rect = new ObservableCollection<Rect>();
                            for (int i = (this.Items.Count - 1); i >= 0; i--)
                            {
								//Gets the Canvas of the AnnotationPresenter whcih includes all the Items of the AnnotationPresenter
                                DependencyObject obj = VisualTreeHelper.GetChild(this, 0);
                                obj = VisualTreeHelper.GetChild(obj, 0);
                                obj = VisualTreeHelper.GetChild(obj, 0);
                                Canvas canvas = obj as Canvas;
								//Checks for Object reference 
                                if (canvas != null)
                                {
									//ContentPresenter of the Annotation 
                                    ContentPresenter element = canvas.Children[i] as ContentPresenter;
									//Rect to identify the original size and the position
                                    Rect bounds = new Rect(new System.Windows.Size(element.ActualWidth, element.ActualHeight));
                                    bounds.X = (this.Items[i] as ChartAnnotationLabel).OffsetX;
                                    bounds.Y = (this.Items[i] as ChartAnnotationLabel).OffsetY;
                                    if (rect.Count > 0)
                                    {
										//Checks wheather the rect collection contains the bounds
                                        if (!rect.Contains(bounds))
                                        {
                                            rect.Add(bounds);
                                        }
                                        bool isVisibilitySet = false;
                                        foreach (Rect prerect in rect)
                                        {
											//Checks for the visibility for the Annotation
                                            if (prerect != bounds && bounds.IntersectsWith(prerect) && !isMeasuring)
                                            {
												//Sets the visibility of the Annotation
                                                element.Visibility = Visibility.Hidden;
                                                isVisibilitySet = true;
                                            }
                                            else if (!isMeasuring && !isVisibilitySet)
                                            {
												//Sets the visibility of the Annotation
                                                element.Visibility = Visibility.Visible;
                                            }
                                        }

                                    }
                                    else
                                    {
                                        if (!isMeasuring)
                                        {
												//Sets the visibility of the Annotation
                                            element.Visibility = Visibility.Visible;
                                        }
                                        rect.Add(bounds);
                                    }
                                }
                            }
                        }
                        break;
                }
            }
            return availabeSize;
        }
		//Prepares to Arrange the elements
        /// <summary>
        /// Called to arrange and size the content of a <see cref="T:System.Windows.Controls.Control"/> object. 
        /// </summary>
        /// <returns>
        /// The size of the control.
        /// </returns>
        /// <param name="arrangeBounds">The computed size that is used to arrange the content.</param>
        protected override System.Windows.Size ArrangeOverride(System.Windows.Size arrangeBounds)
        {
            return base.ArrangeOverride(this.DoLayout(arrangeBounds, false));
        }
        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">Element used to display the specified item.</param>
        /// <param name="item">Specified item.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            ContentPresenter annotationPresenter = element as ContentPresenter;
            ChartAnnotationLabel annotation = item as ChartAnnotationLabel;
            if(annotation != null && annotation.AnnotationChart!= null)
                annotation.AnnotationChart.Annot_Presenter = this;//Sets the presenter for the Annotation
            if(annotation != null)
                annotation.presenter = this;//Sets the Annotation presenter to the property in the Chart
            this.ParentChart = annotation.AnnotationChart;//Sets the Parent Chart
            if (annotation != null && annotationPresenter != null)
            {
                ////Preparing annotation's presenter.
                ConnectAnnotation(annotation, annotationPresenter);
            }
            else
            {
                ////Calling parent's implementation.
                base.PrepareContainerForItemOverride(element, item);
            }
        }

        /// <summary>
        /// Connects the annotation to its presenter.
        /// </summary>
        /// <param name="annotation">The annotation.</param>
        /// <param name="annotationPresenter">The annotation presenter.</param>
        private void ConnectAnnotation(ChartAnnotationLabel annotation, ContentPresenter annotationPresenter)
        {
            ////Template selection binding is used to provide either Chart.AnnotationLabelTemplate for 
            ////ContentTemplate property value, or do unset property if Content is Control derived
            ////instance that provides its own template for UI representation.
            MultiBinding templateSelectionBinding = new MultiBinding();

            MultiBinding multiBind = new MultiBinding();

         
            Binding annotationTemplateBinding = new Binding();
            annotationTemplateBinding.Path = new PropertyPath(ChartAnnotationLabel.TemplateProperty);
            //annotationTemplateBinding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(ChartAnnotationLabel), 1);
            annotationTemplateBinding.Source = annotation;

            ////Binding on Content property.
            Binding contentBinding = new Binding();
            contentBinding.Path = new PropertyPath(ChartAnnotationLabel.ContentProperty);
            contentBinding.Source = annotation;

            Binding contentBinding1 = new Binding();
            //contentBinding.Path = new PropertyPath(ChartAnnotationLabel.ContentProperty);
            contentBinding1.Source = annotation;


            ////Adding both bindings to multibinding.
            templateSelectionBinding.Bindings.Add(contentBinding);

            //templateSelectionBinding.Bindings.Add(labelTemplateBinding);
            templateSelectionBinding.Bindings.Add(annotationTemplateBinding);


            Binding colorBinding = new Binding();
            colorBinding.Path = new PropertyPath(ChartAnnotationLabel.FillProperty);
            colorBinding.Source = annotation;
            multiBind.Bindings.Add(contentBinding1);
            multiBind.Bindings.Add(contentBinding);
            multiBind.Bindings.Add(colorBinding);
            multiBind.Converter = new ColorSwitcher();

            ////TemplateSwitcher converter will decide whether setting ContentTemplate property
            ////to Chart.AnnotationLabelTemplate value or lefting it unset.
            templateSelectionBinding.Converter = m_templateSwitcher;
            ////Setting multibinding on ContentTemplate property.
            BindingOperations.SetBinding(annotationPresenter, ContentPresenter.ContentTemplateProperty, templateSelectionBinding);
            BindingOperations.SetBinding(annotationPresenter, ContentPresenter.ContentProperty, multiBind);

             ////Binding to Canvas attached properies in order to provide changes in OffsetX (OffsetY) 
            ////to hosting Canvas panel.
            Binding offsetBinding = new Binding();
            offsetBinding.Path = new PropertyPath(ChartAnnotationLabel.OffsetXProperty);
            offsetBinding.Source = annotation;
            BindingOperations.SetBinding(annotationPresenter, Canvas.LeftProperty, offsetBinding);

            offsetBinding = new Binding();
            offsetBinding.Path = new PropertyPath(ChartAnnotationLabel.OffsetYProperty);
            offsetBinding.Source = annotation;
            BindingOperations.SetBinding(annotationPresenter, Canvas.TopProperty, offsetBinding);



        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            m_templateSwitcher = null;
            ParentChart = null;
        }

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
