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
using Syncfusion.Windows.Tools.Controls;
using System.Windows.Data;
using System.Reflection;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.PropertyGrid
{
#if WPF
    public class BrushSelectorEditor : ITypeEditor
    {
        public void Attach(PropertyViewItem property, PropertyItem info)
        {
            if (info.CanWrite)
            {
                var binding = new Binding("Value")
                {
                    Mode = BindingMode.TwoWay,
                    Source = info,
                    Converter = new BrushConverter(),
                    UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged,
                    ValidatesOnExceptions = true,
                    ValidatesOnDataErrors = true
                };
                BindingOperations.SetBinding(colorPicker, ColorPicker.BrushProperty, binding);
            }
            else
            {
                colorPicker.IsHitTestVisible = false;
                var binding = new Binding("Value")
                {
                    Mode = BindingMode.OneWay,
                    Source = info,
                    Converter = new BrushConverter(),
                    UpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged,
                    ValidatesOnExceptions = true,
                    ValidatesOnDataErrors = true
                };
                BindingOperations.SetBinding(colorPicker, ColorPicker.BrushProperty, binding);
            }
        }

        ColorPicker colorPicker;
        Brush c = null;
        bool isCalledFromGradientChanged = false;
        public object Create(PropertyInfo propertyInfo)
        {
            colorPicker = new ColorPicker();
            colorPicker.IsGradientPropertyEnabledChanged += new PropertyChangedCallback(colorPicker_IsGradientPropertyEnabledChanged);
            colorPicker.SelectedBrushChanged += new PropertyChangedCallback(colorPicker_SelectedBrushChanged);
            colorPicker.EnableToolTip = false;
            return colorPicker;
        }

        void colorPicker_SelectedBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (isCalledFromGradientChanged)
            {
                isCalledFromGradientChanged = false;
                colorPicker.Brush = c;
            }          
            isCalledFromGradientChanged = false;
        }
        void colorPicker_IsGradientPropertyEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            isCalledFromGradientChanged = true;
            c = colorPicker.Brush;
        }

        public void Detach(PropertyViewItem property)
        {
            BindingOperations.ClearBinding(colorPicker, ColorPicker.BrushProperty);
            colorPicker = null;
        }
    }
#endif

#if SILVERLIGHT
    public class BrushSelectorEditor : ITypeEditor
    {
        /// <summary>
        /// 
        /// </summary>
        //public event System.ComponentModel.PropertyChangedEventHandler ValueChanged;

        public void Attach(PropertyViewItem property, PropertyItem info)
        {
            if (info.CanWrite)
            {
                var binding = new Binding("Value")
                {
                    Mode = BindingMode.TwoWay,
                    Source = info,
                    Converter = new BrushConverter(),
                    ValidatesOnExceptions = true,
                    ValidatesOnDataErrors = true
                };
                BindingOperations.SetBinding(brushSelector, BrushSelector.SelectedBrushProperty, binding);
            }
            else
            {
                brushSelector.IsHitTestVisible = false;
                var binding = new Binding("Value")
                {
                    Mode = BindingMode.OneWay,
                    Source = info,
                    Converter = new BrushConverter(),
                    ValidatesOnExceptions = true,
                    ValidatesOnDataErrors = true
                };
                BindingOperations.SetBinding(brushSelector, BrushSelector.SelectedBrushProperty, binding);
            }
        }

        BrushSelector brushSelector;
        public object Create(PropertyInfo propertyInfo)
        {
            brushSelector = new BrushSelector();
            return brushSelector;
        }

        public void Detach(PropertyViewItem property)
        {
            if (brushSelector != null)
            {
#if SILVERLIGHT
                brushSelector.ClearValue(BrushSelector.SelectedBrushProperty);
#endif
            }
            brushSelector = null;

        }
    }
#endif

}

