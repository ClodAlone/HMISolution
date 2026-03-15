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
using System.Collections.Generic;
using System.Windows.Data;
using System.Reflection;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.PropertyGrid
{
    public class EnumComboEditor : ITypeEditor
    {
        public void Attach(PropertyViewItem property, PropertyItem info)
        {
            if (info.CanWrite)
            {
                var binding = new Binding("Value")
                {
                    Mode = BindingMode.TwoWay,
                    Source = info,
                    ValidatesOnExceptions = true,
                    ValidatesOnDataErrors = true
                };
                BindingOperations.SetBinding(enumCombo, ComboBox.SelectedItemProperty, binding);
            }
            else
            {
                enumCombo.IsHitTestVisible = false;
                var binding = new Binding("Value")
                {
#if WPF
                    Mode = BindingMode.OneWay,
#endif
#if SILVERLIGHT
                    Mode = BindingMode.OneWay,
#endif
                    Source = info,
                    ValidatesOnExceptions = true,
                    ValidatesOnDataErrors = true
                };
                BindingOperations.SetBinding(enumCombo, ComboBox.SelectedItemProperty, binding);
            }
        }

        ComboBox enumCombo;
        public object Create(PropertyInfo propertyInfo)
        {
            enumCombo = new ComboBox() { ItemsSource = EnumHelper.GetValues(propertyInfo.PropertyType) };
            return enumCombo;
        }

        public void Detach(PropertyViewItem property)
        {
            if (enumCombo != null)
            {
#if SILVERLIGHT
                enumCombo.ClearValue(ComboBox.SelectedItemProperty);
#endif
#if WPF
                BindingOperations.ClearBinding(enumCombo, ComboBox.SelectedItemProperty);
#endif
            }
            enumCombo = null;
        }
    }

    public class FontComboEditor : ITypeEditor
    {
        public void Attach(PropertyViewItem property, PropertyItem info)
        {
            if (info.CanWrite)
            {
                var binding = new Binding("Value")
                {
                    Mode = BindingMode.TwoWay,
                    Source = info,
                    ValidatesOnExceptions = true,
                    ValidatesOnDataErrors = true
                };
                BindingOperations.SetBinding(fontCombo, ComboBox.SelectedItemProperty, binding);
            }
            else
            {
                fontCombo.IsHitTestVisible = true;
                var binding = new Binding("Value")
                {
                    Mode = BindingMode.OneWay,
                    Source = info,
                    ValidatesOnExceptions = true,
                    ValidatesOnDataErrors = true
                };
                BindingOperations.SetBinding(fontCombo, ComboBox.SelectedItemProperty, binding);
            }
        }

        ComboBox fontCombo;
        public object Create(PropertyInfo propertyInfo)
        {
            fontCombo = new ComboBox();
            if (propertyInfo.PropertyType == typeof(FontFamily))
            {
                ObservableCollection<FontFamily> fonts = new ObservableCollection<FontFamily>();
                fonts.Add(new FontFamily("Arial"));
                fonts.Add(new FontFamily("Courier New"));
                fonts.Add(new FontFamily("Times New Roman"));
                fonts.Add(new FontFamily("Batang"));
                fonts.Add(new FontFamily("BatangChe"));
                fonts.Add(new FontFamily("DFKai-SB"));
                fonts.Add(new FontFamily("Dotum"));
                fonts.Add(new FontFamily("DutumChe"));
                fonts.Add(new FontFamily("FangSong"));
                fonts.Add(new FontFamily("GulimChe"));
                fonts.Add(new FontFamily("Gungsuh"));
                fonts.Add(new FontFamily("GungsuhChe"));
                fonts.Add(new FontFamily("KaiTi"));
                fonts.Add(new FontFamily("Malgun Gothic"));
                fonts.Add(new FontFamily("Meiryo"));
                fonts.Add(new FontFamily("Microsoft JhengHei"));
                fonts.Add(new FontFamily("Microsoft YaHei"));
                fonts.Add(new FontFamily("MingLiU"));
                fonts.Add(new FontFamily("MingLiu_HKSCS"));
                fonts.Add(new FontFamily("MingLiu_HKSCS-ExtB"));
                fonts.Add(new FontFamily("MingLiu-ExtB"));
                fontCombo.ItemsSource = fonts;
            }
            else if (propertyInfo.PropertyType == typeof(FontWeight))
            {
                List<FontWeight> list = new List<FontWeight>() 
                    {
                        FontWeights.Black, FontWeights.Bold, FontWeights.ExtraBlack, FontWeights.ExtraBold, 
                        FontWeights.ExtraLight, FontWeights.Light, FontWeights.Medium, FontWeights.Normal, FontWeights.SemiBold, 
                        FontWeights.Thin 
                    };
                fontCombo.ItemsSource = list;
            }
            else if (propertyInfo.PropertyType == typeof(FontStyle))
            {
                List<FontStyle> list = new List<FontStyle>() 
                    {
                        FontStyles.Italic,FontStyles.Normal
                    };
                fontCombo.ItemsSource = list;
            }
            else if (propertyInfo.PropertyType == typeof(FontStretch))
            {
                List<FontStretch> list = new List<FontStretch>() 
                    {
                        FontStretches.Condensed,FontStretches.Expanded,FontStretches.ExtraCondensed,FontStretches.ExtraExpanded,FontStretches.Normal,FontStretches.SemiCondensed,FontStretches.SemiExpanded,FontStretches.UltraCondensed,FontStretches.UltraExpanded
                    };
                fontCombo.ItemsSource = list;
            }
            return fontCombo;
        }

        public void Detach(PropertyViewItem property)
        {
            if (fontCombo != null)
            {
#if SILVERLIGHT
                fontCombo.ClearValue(ComboBox.SelectedItemProperty);
#endif
#if WPF
                BindingOperations.ClearBinding(fontCombo, ComboBox.SelectedItemProperty);
#endif

            }
            fontCombo = null;
        }
    }
}
