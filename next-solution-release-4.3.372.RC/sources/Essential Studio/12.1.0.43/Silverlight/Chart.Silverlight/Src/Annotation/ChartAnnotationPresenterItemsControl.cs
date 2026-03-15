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
using System.Windows.Data;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for AnnotationPresenter 
    /// </summary>
    public class AnnotationPresenter: ItemsControl,IDisposable
    {
        #region Primitive Type
        private class Templateswitcher : IValueConverter
        { 
            #region IValueConverter Members
            
            public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                //Control control = values[0] as Control;
                DataTemplate template = values as DataTemplate;
                ////If content is control that provides its own template Chart.LabelTemplate should not
                ////be considered and ContentControl.ContentTemplate should be left unset.
                //if (control != null && control.Template != null)
                //{
                //    return DependencyProperty.UnsetValue;
                //}
                ////Otherwise we provide Chart.AnnotationLabelTemplate as a template for Content of ContentControl.
                return template;
            }
            public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotSupportedException("Multibinding template selection cannot be converted back");
            }


            #endregion



            //#region IValueConverter Members

            //object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            //{
            //    throw new NotImplementedException();
            //}

            //object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            //{
            //    throw new NotImplementedException();
            //}

            //#endregion
        }
        /// <summary>
        /// Return DataTemplate value from the given object
        /// </summary>
        public class contentswitcher : IValueConverter
        {
            /// <summary>
            /// Modifies the source data before passing it to the target for display in the UI.
            /// </summary>
            /// <returns>
            /// The value to be passed to the target dependency property.
            /// </returns>
            /// <param name="values">The source data being passed to the target.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
            public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                DataTemplate cnt = values as DataTemplate;
                return cnt;
            }

            /// <summary>
            /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
            /// </summary>
            /// <returns>
            /// The value to be passed to the source object.
            /// </returns>
            /// <param name="value">The target data being passed to the source.</param><param name="targetTypes">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
            public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotSupportedException("Multibinding template selection cannot be converted back");
            }

        }

        #endregion

        #region Fields
        //private static IValueConverter m_templateSwitcher = new Templateswitcher();
        #endregion

        #region implementation

        /// <summary>
        /// Prepares the specified element to display the specified item. 
        /// </summary>
        /// <param name="element">The element used to display the specified item.</param><param name="item">The item to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            ContentPresenter annotationPresenter = element as ContentPresenter;
            ChartAnnotationLabel annotation = item as ChartAnnotationLabel;
            ChartSeriesAnnotation seriesannotation = item as ChartSeriesAnnotation;
            if (seriesannotation!=null && annotationPresenter!=null)
            {
                annotationPresenter.Tag = seriesannotation;
                connectseriesAnnotation(seriesannotation, annotationPresenter);
                
            }
            else if (annotation != null && annotationPresenter != null)
            {
                ////Preparing annotation's presenter.
                annotationPresenter.Tag = annotation;
                ConnectAnnotation(annotation, annotationPresenter);
            }
            else
            {
                ////Calling parent's implementation.
                base.PrepareContainerForItemOverride(element, item);
            }
        }
        private void connectseriesAnnotation(ChartSeriesAnnotation seriesannotation, ContentPresenter annotationPresenter)
        {
            Binding contentBinding = new Binding();
            contentBinding.Path = new PropertyPath("Description");
            contentBinding.Source = seriesannotation;
            contentBinding.Mode = BindingMode.TwoWay;

            //BindingOperations.SetBinding(annotationPresenter, ContentPresenter.ContentTemplateProperty, contentBinding);
            BindingOperations.SetBinding(annotationPresenter, ContentPresenter.ContentProperty, contentBinding);
            RotateTransform rt = new RotateTransform();
            Binding LabelAngleBinding = new Binding();
            LabelAngleBinding.Path = new PropertyPath("RotateAngle");
            LabelAngleBinding.Source = seriesannotation;
            LabelAngleBinding.Mode = BindingMode.TwoWay;
           
            BindingOperations.SetBinding(rt, RotateTransform.AngleProperty, LabelAngleBinding);


            annotationPresenter.RenderTransform = rt;
            
            Binding annotationTemplateBinding = new Binding();
            annotationTemplateBinding.Path = new PropertyPath("Template");
            //annotationTemplateBinding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(ChartAnnotationLabel), 1);
            annotationTemplateBinding.Source = seriesannotation;
            //annotationTemplateBinding.Converter = new Templateswitcher();
            BindingOperations.SetBinding(annotationPresenter, ContentPresenter.ContentTemplateProperty, annotationTemplateBinding);

            //Binding fillBinidng = new Binding();
            //fillBinidng.Path = new PropertyPath("Fill");
            //fillBinidng.Source = seriesannotation;
            //BindingOperations.SetBinding(annotationPresenter, ContentPresenter.DataContextProperty, fillBinidng);


            //Binding offsetBinding = new Binding();
            //offsetBinding.Path = new PropertyPath("OffsetX");
            //offsetBinding.Source = seriesannotation;
            //offsetBinding.Mode = BindingMode.TwoWay;
            ////offsetBinding.Converter = new xpropertySwitcher();

            //BindingOperations.SetBinding(annotationPresenter, Canvas.LeftProperty, offsetBinding);

            //offsetBinding = new Binding();
            //offsetBinding.Path = new PropertyPath("OffsetY");
            //offsetBinding.Source = seriesannotation;
            //offsetBinding.Mode = BindingMode.TwoWay;
            //BindingOperations.SetBinding(annotationPresenter, Canvas.TopProperty, offsetBinding);

        }

        private void ConnectAnnotation(ChartAnnotationLabel annotation, ContentPresenter annotationPresenter)
        {
            ////Template selection binding is used to provide either Chart.AnnotationLabelTemplate for 
            ////ContentTemplate property value, or do unset property if Content is Control derived
            ////instance that provides its own template for UI representation.
            
           
            ////Binding on Content property.
            Binding contentBinding = new Binding();
            contentBinding.Path = new PropertyPath("Content");
            contentBinding.Source = annotation;
            contentBinding.Mode = BindingMode.TwoWay;

            //BindingOperations.SetBinding(annotationPresenter, ContentPresenter.ContentTemplateProperty, contentBinding);
            BindingOperations.SetBinding(annotationPresenter, ContentPresenter.ContentProperty, contentBinding);

            

            Binding annotationTemplateBinding = new Binding();
            annotationTemplateBinding.Path = new PropertyPath("Template");
            //annotationTemplateBinding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(ChartAnnotationLabel), 1);
            annotationTemplateBinding.Source = annotation;
            //annotationTemplateBinding.Converter = new Templateswitcher();
            BindingOperations.SetBinding(annotationPresenter, ContentPresenter.ContentTemplateProperty, annotationTemplateBinding);
            //BindingOperations.SetBinding(annotationPresenter, ContentPresenter.ContentTemplateProperty, contentBinding);

            //BindingOperations.SetBinding(annotationPresenter, ContentPresenter.ContentProperty, contentBinding);

            //Binding fillBinidng = new Binding();
            //fillBinidng.Path = new PropertyPath("Fill");
            //fillBinidng.Source = annotation;
            //BindingOperations.SetBinding(annotationPresenter, ContentPresenter.DataContextProperty, fillBinidng);
           
           
          
            //Binding offsetBinding = new Binding();
            //offsetBinding.Path = new PropertyPath("OffsetX");
            //offsetBinding.Source = annotation;
            //offsetBinding.Mode = BindingMode.TwoWay;
            ////offsetBinding.Converter = new xpropertySwitcher();

            //BindingOperations.SetBinding(annotationPresenter, Canvas.LeftProperty, offsetBinding);

            //offsetBinding = new Binding();
            //offsetBinding.Path = new PropertyPath("OffsetY");
            //offsetBinding.Source = annotation;
            //offsetBinding.Mode = BindingMode.TwoWay;
            //BindingOperations.SetBinding(annotationPresenter, Canvas.TopProperty, offsetBinding);
        
        }
        #endregion



        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.ClearValue(AnnotationPresenter.ItemsSourceProperty);
            this.Items.Clear();
            this.Resources.Clear();
            GC.Collect();
            GC.SuppressFinalize(this);
        }

        #endregion



    }
}
