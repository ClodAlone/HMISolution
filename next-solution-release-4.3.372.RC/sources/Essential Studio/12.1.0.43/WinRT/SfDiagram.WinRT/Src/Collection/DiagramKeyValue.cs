#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
#if WINRT_USING
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup; 
#else
using System.Windows.Markup;
#endif

namespace Syncfusion.UI.Xaml.Diagram
{
#if WINRT
    [ContentProperty(Name = "Value")] 
#else
    [ContentProperty("Value")] 
#endif
    public class DiagramKeyValue<TValue> : DependencyObject
    {
        public object Key
        {
            get { return GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
            //get; set;
        }

        // Using a DependencyProperty as the backing store for Key.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyProperty =
            DependencyProperty.Register("Key", typeof(object), typeof(DiagramKeyValue<TValue>), new PropertyMetadata(null));

        //private TValue m_TValue;

        public TValue Value
        {
            get { return (TValue)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
            //get { return m_TValue; }
            //set
            //{
            //    m_TValue = value;
            //    OnValueChanged(value);
            //}
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(TValue), typeof(DiagramKeyValue<TValue>), new PropertyMetadata(null, OnValueChanged));

        internal IDiagramElement quickView;

        //private void OnValueChanged(TValue newValue)
        //{
        //    DiagramKeyValue<object, TValue> source = this;
        //    if (source != null && source.Value is DataTemplate)
        //    {
        //        DataTemplate template = source.Value as DataTemplate;
        //        source.quickView = template.LoadContent() as IDiagramElement<object>;
        //        if (source.Key == null && source.quickView.Key != null)
        //        {
        //            source.Key = source.quickView.Key;
        //        }
        //        if (source.quickView.Key == null && source.Key != null)
        //        {
        //            source.quickView.Key = source.Key;
        //        }
        //    }
        //}

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DiagramKeyValue<TValue> source = d as DiagramKeyValue<TValue>;
            if (source != null && source.Value is DataTemplate)
            {
                DataTemplate template = source.Value as DataTemplate;
                source.quickView = template.LoadContent() as IDiagramElement;
                if (source.Key == null && source.quickView.Key != null)
                {
                    source.Key = source.quickView.Key;
                }
                if (source.quickView.Key == null && source.Key != null)
                {
                    source.quickView.Key = source.Key;
                }
            }
        }

    }
}
