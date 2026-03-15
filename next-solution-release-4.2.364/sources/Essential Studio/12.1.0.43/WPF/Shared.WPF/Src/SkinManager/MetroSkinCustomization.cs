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
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Shared
{
    public partial class SkinStorage
    {
        #region attached proeprties

        /// <summary>
        /// Attached proeprty used to customize Metro Theme SelectedBrush
        /// </summary>
        public static readonly DependencyProperty MetroBrushProperty =
            DependencyProperty.RegisterAttached("MetroBrush", typeof(Brush), typeof(SkinStorage), new FrameworkPropertyMetadata(metrobrush, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnMetroBrushChanged)));


        /// <summary>
        /// Attached proeprty used to customize MetroThemeForegroundBrush
        /// </summary>

        public static readonly DependencyProperty MetroForegroundBrushProperty =
            DependencyProperty.RegisterAttached("MetroForegroundBrush", typeof(Brush), typeof(SkinStorage), new FrameworkPropertyMetadata(metroforegroundbrush,FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnMetroBrushChanged)));


        /// <summary>
        /// Attached proeprty used to customize MetroThemeHoverBrush
        /// </summary>
        public static readonly DependencyProperty MetroHoverBrushProperty =
            DependencyProperty.RegisterAttached("MetroHoverBrush", typeof(Brush), typeof(SkinStorage), new FrameworkPropertyMetadata(metrohoverbrush,FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnMetroBrushChanged)));


        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty MetroBorderBrushProperty =
          DependencyProperty.RegisterAttached("MetroBorderBrush", typeof(Brush), typeof(SkinStorage), new FrameworkPropertyMetadata(metroborderbrush, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnMetroBrushChanged)));


        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty MetroFocusedBorderBrushProperty =
          DependencyProperty.RegisterAttached("MetroFocusedBorderBrush", typeof(Brush), typeof(SkinStorage), new FrameworkPropertyMetadata(metrofocusedbrush, FrameworkPropertyMetadataOptions.Inherits,  new PropertyChangedCallback(OnMetroBrushChanged)));


        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty MetroBackgroundBrushProperty =
         DependencyProperty.RegisterAttached("MetroBackgroundBrush", typeof(Brush), typeof(SkinStorage), new FrameworkPropertyMetadata(metrobackgroundbrush, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnMetroBrushChanged)));



        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty MetroFontFamilyProperty =
          DependencyProperty.RegisterAttached("MetroFontFamily", typeof(FontFamily), typeof(SkinStorage), new FrameworkPropertyMetadata(metrofontfamily,FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnMetroBrushChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty MetroPanelBackgroundBrushProperty =
         DependencyProperty.RegisterAttached("MetroPanelBackgroundBrush", typeof(Brush), typeof(SkinStorage), new FrameworkPropertyMetadata(metropanelbackgroundbrush, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnMetroBrushChanged)));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty MetroHighlightedForegroundBrushProperty =
        DependencyProperty.RegisterAttached("MetroHighlightedForegroundBrush", typeof(Brush), typeof(SkinStorage), new FrameworkPropertyMetadata(metrohighlightedforegroundbrush, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnMetroBrushChanged)));

        #endregion


        #region constants


        private static Brush metrobrush = new SolidColorBrush(((Color)ColorConverter.ConvertFromString("#FF119EDA")));

        private static Brush metrohoverbrush = new SolidColorBrush(((Color)ColorConverter.ConvertFromString("#FFD8D8D9")));
        private static Brush metroforegroundbrush = new SolidColorBrush(((Color)ColorConverter.ConvertFromString("#FF333333")));
        private static Brush metroborderbrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCCCCCC"));
        private static Brush metrofocusedbrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF2ABFF1"));
        private static Brush metrobackgroundbrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEBEBEB"));
        private static Brush metropanelbackgroundbrush = Brushes.White;
        private static Brush metrohighlightedforegroundbrush = Brushes.White;
        private static FontFamily metrofontfamily = new FontFamily("Segoe UI");

        #endregion


        #region public methods
        /// <summary>
        /// Get MetroTheme Selected Brush
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Brush GetMetroBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(MetroBrushProperty);
        }

        /// <summary>
        /// Set Metro Theme SelectedBrush
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetMetroBrush(DependencyObject obj, Brush value)
        {
            obj.ClearValue(MetroBrushProperty);
            obj.SetValue(MetroBrushProperty, value);
        }

        /// <summary>
        /// Get MetroThemeForegroundBrush
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Brush GetMetroForegroundBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(MetroForegroundBrushProperty);
        }

        /// <summary>
        /// Set Metro Theme ForegroundBrush
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetMetroForegroundBrush(DependencyObject obj, Brush value)
        {
            obj.ClearValue(MetroForegroundBrushProperty);
            obj.SetValue(MetroForegroundBrushProperty, value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Brush GetMetroHoverBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(MetroHoverBrushProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetMetroHoverBrush(DependencyObject obj, Brush value)
        {
            obj.ClearValue(MetroHoverBrushProperty);
            obj.SetValue(MetroHoverBrushProperty, value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Brush GetMetroBorderBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(MetroBorderBrushProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetMetroBorderBrush(DependencyObject obj, Brush value)
        {

            obj.ClearValue(MetroBorderBrushProperty);

            obj.SetValue(MetroBorderBrushProperty, value);


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Brush GetMetroFocusedBorderBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(MetroFocusedBorderBrushProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetMetroFocusedBorderBrush(DependencyObject obj, Brush value)
        {

            obj.ClearValue(MetroFocusedBorderBrushProperty);

            obj.SetValue(MetroFocusedBorderBrushProperty, value);


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Brush GetMetroBackgroundBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(MetroBackgroundBrushProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetMetroBackgroundBrush(DependencyObject obj, Brush value)
        {
            obj.ClearValue(MetroBackgroundBrushProperty);
            obj.SetValue(MetroBackgroundBrushProperty, value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Brush GetMetroPanelBackgroundBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(MetroPanelBackgroundBrushProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetMetroPanelBackgroundBrush(DependencyObject obj, Brush value)
        {
            obj.ClearValue(MetroPanelBackgroundBrushProperty);
            obj.SetValue(MetroPanelBackgroundBrushProperty, value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Brush GetMetroHighlightedForegroundBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(MetroHighlightedForegroundBrushProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetMetroHighlightedForegroundBrush(DependencyObject obj, Brush value)
        {
            obj.ClearValue(MetroHighlightedForegroundBrushProperty);
            obj.SetValue(MetroHighlightedForegroundBrushProperty, value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static FontFamily GetMetroFontFamily(DependencyObject obj)
        {
            return (FontFamily)obj.GetValue(MetroFontFamilyProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetMetroFontFamily(DependencyObject obj, FontFamily value)
        {
            obj.ClearValue(MetroFontFamilyProperty);

            obj.SetValue(MetroFontFamilyProperty, value);

        }


        #endregion

        #region implementation
        /// <summary>
        /// 
        /// </summary>
        /// <param name="metroskindictionary"></param>
        /// <param name="obj"></param>
        public static void MergeMetroBrush(ResourceDictionary metroskindictionary, DependencyObject obj)
        {
            ResourceDictionary dictionary = metroskindictionary;
            try
            {
                if (GetMetroBrush(obj) != null)
                    dictionary["MetroBrush"] = GetMetroBrush(obj);
                if (GetMetroHoverBrush(obj) != null)
                    dictionary["MetroHoverBrush"] = GetMetroHoverBrush(obj);
                if (GetMetroForegroundBrush(obj) != null)
                    dictionary["MetroForegroundBrush"] = GetMetroForegroundBrush(obj);
                if (GetMetroFontFamily(obj) != null)
                    dictionary["MetroFontFamily"] = GetMetroFontFamily(obj);
                if (GetMetroBorderBrush(obj) != null)
                    dictionary["MetroBorderBrush"] = GetMetroBorderBrush(obj); ;
                if (GetMetroFocusedBorderBrush(obj) != null)
                    dictionary["MetroFocusedBorderBrush"] = GetMetroFocusedBorderBrush(obj);
                if (GetMetroBackgroundBrush(obj) != null)
                    dictionary["MetroBackgroundBrush"] = GetMetroBackgroundBrush(obj);
                if (GetMetroPanelBackgroundBrush(obj) != null)
                    dictionary["MetroPanelBackgroundBrush"] = GetMetroPanelBackgroundBrush(obj);
                if (GetMetroHighlightedForegroundBrush(obj) != null)
                    dictionary["MetroHighlightedForegroundBrush"] = GetMetroHighlightedForegroundBrush(obj);

            }
            catch { }


        }

        private static void MergeMetroBrushDictionaries(FrameworkElement element, ResourceDictionary dictionary)
        {
            //if (GetVisualStyle(element as DependencyObject).ToString() == "Metro")
            //{
            var skincontrols = GetSkinAttribute(element, "Metro");
            if (skincontrols != null)
            {
                ResourceDictionary rd1 = new ResourceDictionary();
                rd1.Source = new Uri(skincontrols.XamlResource, UriKind.RelativeOrAbsolute);
                for (int i = 0; i < rd1.MergedDictionaries.Count; i++)
                {
                    var rdic = rd1.MergedDictionaries[i];
                    if (rdic.Source == dictionary.Source)
                    {
                        try
                        {
                            rd1.MergedDictionaries.RemoveAt(i);
                        }
                        catch
                        {

                        }
                        i--;
                    }
                }
                SkinManager.RemoveDictionaryIfExist(element, dictionary);
                element.Resources.MergedDictionaries.Add(dictionary);
                //}
            }
        }
        private static void ApplyMetroBrush(DependencyObject obj)
        {
            var rd = new ResourceDictionary();
            rd.Source = new Uri("/Syncfusion.Shared.WPF;component/SkinManager/MetroThemeBrushes.xaml", UriKind.RelativeOrAbsolute);
            MergeMetroBrush(rd, obj);

            if (obj.GetType().ToString().StartsWith("Syncfusion"))
            {
                MergeMetroBrushDictionaries(obj as FrameworkElement, rd);
            }
            else
            {
                if (obj as FrameworkElement != null)
                {
                    SkinManager.RemoveDictionaryIfExist(obj as FrameworkElement, rd);
                    (obj as FrameworkElement).Resources.MergedDictionaries.Add(rd);
                }
            }

            //IEnumerable<DependencyObject> child = VisualUtils.EnumLogicalChildrenOfType(obj, typeof(FrameworkElement));
            //foreach (FrameworkElement fe in child)
            //{
            //    if (fe.GetType().ToString().StartsWith("Syncfusion"))
            //    {

            //        MergeMetroBrushDictionaries(fe, rd);

            //    }
            //}

        }

        /// <summary>
        /// Called when [any of MetroTheme Brush  changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMetroBrushChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!SkinStorage.GetEnableOptimization(obj))
            {
                if (obj != null)
                {
                    FrameworkElement element = obj as FrameworkElement;
                    if (element != null)
                    {
                        if (element.IsLoaded == false)
                            element.Loaded += new RoutedEventHandler(element_Loaded1);
                    }

                    ApplyMetroBrush(obj);
                }
            }
            else
            {
                FrameworkElement element = obj as FrameworkElement;
                int count = 0;
                if (element != null && GetVisualStyle(element) == "Metro" && IsApply(element) && (count = element.Resources.MergedDictionaries.Count) > 0)
                {
                    UpdateMetroBrush(element, element.Resources.MergedDictionaries, count);

                }
            }
        }

        private static void UpdateMetroBrush(FrameworkElement element,Collection<ResourceDictionary> MergedDictionaries, int count)
        {
            for (int i = 0; i < count; i++)
            {
                ResourceDictionary rd = MergedDictionaries[i] as ResourceDictionary;
                if (rd.Source.ToString().Contains("SkinManager/MetroThemeBrushes.xaml"))
                {
                    MergeMetroBrush(rd, element);
                    if (!SkinStorage.GetEnableOptimization(element))
                    {
                        if (MergedDictionaries.Contains(rd))
                            MergedDictionaries.Remove(rd);
                        MergedDictionaries.Add(rd);
                    }
                    break;
                }
                if (rd.MergedDictionaries.Count > 0)
                {
                    UpdateMetroBrush(element, rd.MergedDictionaries, rd.MergedDictionaries.Count);
                }
            }
        }

        static void element_Loaded1(object sender, RoutedEventArgs e)
        {
            ApplyMetroBrush(sender as DependencyObject);
            (sender as FrameworkElement).Loaded -= new RoutedEventHandler(element_Loaded1);

        }

        #endregion
    }
}
