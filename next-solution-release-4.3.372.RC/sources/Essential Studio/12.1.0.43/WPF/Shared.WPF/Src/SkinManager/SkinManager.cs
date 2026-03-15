
// <copyright file="SkinManager.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Windows.Shared;
using System.Reflection;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents a SkinManager class that have a ActiveColorScheme DependencyProperty.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class SkinManager : DependencyObject
    {
        #region Dependency Properties

        /// <summary>
        /// Defines Custom Color Scheme is applied to control. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ActiveColorSchemeProperty =
           DependencyProperty.RegisterAttached("ActiveColorScheme", typeof(Brush), typeof(SkinManager), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnActiveColorSchemeChanged)));
        #endregion

        #region Implementation

        /// <summary>
        /// Removes the dictionary if exist.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="dictionary">The dictionary.</param>
        public static void RemoveDictionaryIfExist(FrameworkElement element, ResourceDictionary dictionary)
        {

            if (element != null)
            {

                for (int i = 0; i < element.Resources.MergedDictionaries.Count; i++)
                {
                    var rdic = element.Resources.MergedDictionaries[i];
                    if (rdic.Source == dictionary.Source)
                    {
                        try
                        {
                            element.Resources.MergedDictionaries.RemoveAt(i);
                        }
                        catch { }
                        i--;
                    }
                }
            }
        }

        internal static SkinTypeAttribute GetSkinAttribute(FrameworkElement element, string currentskin)
        {
            SkinTypeAttribute skinTypeAttr = null;
            if (element != null)
            {
                System.Reflection.MemberInfo inf = element.GetType();
                var attributes = inf.GetCustomAttributes(true);
                for (int i = 0; i < attributes.Length; i++)
                {
                    var attr = attributes[i] as SkinTypeAttribute;
                    if (attr != null && attr.SkinVisualStyle.ToString() == currentskin)
                    {
                        skinTypeAttr = attr;
                        break;
                    }
                }
            }
            return skinTypeAttr;
        }


        /// <summary>
        /// Calls OnActiveColorSchemeChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">The object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnActiveColorSchemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {            
            Brush brush = e.NewValue as Brush;

            if (brush != null)
            {
                if (d is Window)
                {
                    Window win = d as Window;
                    win.Loaded += new RoutedEventHandler(win_Loaded);
                }
            }

            string actskin = SkinStorage.GetVisualStyle(d);
            if (brush != null)
            {
                if (brush is SolidColorBrush)
                {
                    Color color = (brush as SolidColorBrush).Color;

                    if (color != null && (d as Visual)!=null)
                    {

                        IEnumerable<DependencyObject> child = VisualUtils.EnumLogicalChildrenOfType(d, typeof(FrameworkElement));

                        foreach (FrameworkElement fe in child)
                        {
                            if (fe != null)
                            {

                                if (fe is Popup)
                                {
                                    if ((fe as Popup).Child as Visual != null)
                                    {
                                        IEnumerable<DependencyObject> popupChild = VisualUtils.EnumLogicalChildrenOfType((fe as Popup).Child as DependencyObject, typeof(FrameworkElement));
                                        if (popupChild != null)
                                        {
                                            foreach (FrameworkElement frameSkin in popupChild)
                                            {
                                                if (frameSkin is ScrollViewer)
                                                {
                                                    var skinControls = GetSkinAttribute(frameSkin, actskin);

                                                    if (skinControls != null)
                                                    {
                                                        var themeName = skinControls.SkinVisualStyle;
                                                        var xamlName = skinControls.XamlResource;
                                                        var type = skinControls.Type;
                                                        var rd = new ResourceDictionary();
                                                        rd.Source = new Uri(xamlName, UriKind.RelativeOrAbsolute);

                                                        if (color != null)
                                                        {
                                                            rd = SkinColorScheme.ApplyCustomColorScheme(rd, color);
                                                        }
                                                        SkinManager.RemoveDictionaryIfExist(frameSkin, rd);
                                                        frameSkin.Resources.MergedDictionaries.Add(rd);
                                                    }
                                                    SkinStorage.SetVisualStyle(frameSkin, actskin);
                                                }
                                            }
                                        }
                                    }
                                }
                                if (fe is ScrollViewer && fe is Visual)
                                {
                                    if ((fe as ScrollViewer).Content as Visual != null)
                                    {
                                        IEnumerable<DependencyObject> scrollChild = VisualUtils.EnumLogicalChildrenOfType((fe as ScrollViewer).Content as DependencyObject, typeof(FrameworkElement));

                                        if (scrollChild != null)
                                        {
                                            foreach (FrameworkElement frameSkin in scrollChild)
                                            {
                                                if (frameSkin != null)
                                                {
                                                    var skinControls = GetSkinAttribute(frameSkin, actskin);

                                                    if (skinControls != null)
                                                    {
                                                        var themeName = skinControls.SkinVisualStyle;
                                                        var xamlName = skinControls.XamlResource;
                                                        var type = skinControls.Type;
                                                        var rd = new ResourceDictionary();
                                                        rd.Source = new Uri(xamlName, UriKind.RelativeOrAbsolute);

                                                        if (color != null)
                                                        {
                                                            rd = SkinColorScheme.ApplyCustomColorScheme(rd, color);
                                                        }
                                                        try
                                                        {
                                                            SkinManager.RemoveDictionaryIfExist(frameSkin, rd);
                                                            frameSkin.Resources.MergedDictionaries.Add(rd);
                                                        }
                                                        catch { }
                                                    }

                                                    SkinStorage.SetVisualStyle(frameSkin, actskin);

                                                }
                                            }
                                        }

                                    }
                                }

                                var skinControls1 = GetSkinAttribute(fe, actskin);

                                if (skinControls1 != null)
                                {
                                    var themeName = skinControls1.SkinVisualStyle;
                                    var xamlName = skinControls1.XamlResource;
                                    var type = skinControls1.Type;
                                    var rd = new ResourceDictionary();
                                    rd.Source = new Uri(xamlName, UriKind.RelativeOrAbsolute);

                                    if (color != null)
                                    {
                                        rd = SkinColorScheme.ApplyCustomColorScheme(rd, color);
                                    }
                                    SkinManager.RemoveDictionaryIfExist(fe, rd);
                                    fe.Resources.MergedDictionaries.Add(rd);
                                }
                               SkinStorage.SetVisualStyle(fe, actskin);
                            }
                        }


                        if ((d as FrameworkElement) != null)
                        {
                            var skinControls = GetSkinAttribute((d as FrameworkElement), actskin);


                            if (skinControls != null)
                            {
                                var themeName = skinControls.SkinVisualStyle;
                                var xamlName = skinControls.XamlResource;
                                var type = skinControls.Type;
                                var rd = new ResourceDictionary();
                                rd.Source = new Uri(xamlName, UriKind.RelativeOrAbsolute);

                                if (color != null)
                                {
                                    rd = SkinColorScheme.ApplyCustomColorScheme(rd, color);
                                }
                                SkinManager.RemoveDictionaryIfExist((d as FrameworkElement), rd);
                                (d as FrameworkElement).Resources.MergedDictionaries.Add(rd);

                             
                            }
                            //SkinStorage.SetVisualStyle((d as FrameworkElement), actskin);

                        }

                    }

                }
            }
        }

        static void win_Loaded(object sender, RoutedEventArgs e)
        {
            SolidColorBrush s = SkinManager.GetActiveColorScheme((DependencyObject)sender);
            if (s != null)
            {
               ApplyActiveColorToMSControls(sender as DependencyObject, s);               
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the value of the ActiveColorScheme property for a given element.
        /// </summary>
        /// <param name="obj">The element for which to retrieve the ZIndex value.</param>
        /// <returns>Return the active color</returns>
        [TypeConverter(typeof(BrushConverter))]
        public static SolidColorBrush GetActiveColorScheme(DependencyObject obj)
        {
            return (SolidColorBrush)obj.GetValue(ActiveColorSchemeProperty);
        }

        /// <summary>
        /// Sets the value of the ActiveColorScheme property for a given element.
        /// </summary>
        /// <param name="obj">The element on which to apply the property value.</param>
        /// <param name="value">Active scheme brush.</param>

        static void ApplyActiveColorToMSControls(DependencyObject obj, SolidColorBrush value)
        {
            string visualStyle = SkinStorage.GetVisualStyle(obj);
            var rd = new ResourceDictionary();
            if ((obj as FrameworkElement) != null)
            {

                if ((obj as FrameworkElement).Parent is Window || (obj as FrameworkElement).Parent is ChromelessWindow || (obj is Window) || (obj is ChromelessWindow))
                {

                    if (visualStyle == "Office2007Black")
                    {
                        try
                        {
                            rd.Source = new Uri("/Syncfusion.Shared.WPF;component/SkinManager/Office2007BlackStyle.xaml", UriKind.RelativeOrAbsolute);
                            (obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                            rd = SkinColorScheme.ApplyCustomColorScheme(rd, (value as SolidColorBrush).Color);
                            // This is not removing dictionary from merged collection.
                            //(obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                            SkinManager.RemoveDictionaryIfExist(obj as FrameworkElement, rd);
                            (obj as FrameworkElement).Resources.MergedDictionaries.Add(rd);
                        }
                        catch
                        {
                        }
                    }
                    else if (visualStyle == "Blend")
                    {
                        try
                        {
                            rd.Source = new Uri("/Syncfusion.Shared.WPF;component/SkinManager/BlendStyle.xaml", UriKind.RelativeOrAbsolute);
                            (obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                            rd = SkinColorScheme.ApplyCustomColorScheme(rd, (value as SolidColorBrush).Color);
                            // This is not removing dictionary from merged collection.
                            //(obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                            SkinManager.RemoveDictionaryIfExist(obj as FrameworkElement, rd);
                            (obj as FrameworkElement).Resources.MergedDictionaries.Add(rd);
                        }
                        catch
                        {
                        }
                    }
                    else if (visualStyle == "Office2003")
                    {
                        try
                        {
                            rd.Source = new Uri("/Syncfusion.Shared.WPF;component/SkinManager/Office2003Style.xaml", UriKind.RelativeOrAbsolute);
                            (obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                            rd = SkinColorScheme.ApplyCustomColorScheme(rd, (value as SolidColorBrush).Color);
                            // This is not removing dictionary from merged collection.
                            //(obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                            SkinManager.RemoveDictionaryIfExist(obj as FrameworkElement, rd);
                            (obj as FrameworkElement).Resources.MergedDictionaries.Add(rd);
                        }
                        catch
                        {
                        }
                    }
                    else if (visualStyle == "Office2007Blue")
                    {
                        try
                        {
                            rd.Source = new Uri("/Syncfusion.Shared.WPF;component/SkinManager/Office2007BlueStyle.xaml", UriKind.RelativeOrAbsolute);
                            (obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                            rd = SkinColorScheme.ApplyCustomColorScheme(rd, (value as SolidColorBrush).Color);
                            // This is not removing dictionary from merged collection.
                            //(obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                            SkinManager.RemoveDictionaryIfExist(obj as FrameworkElement, rd);
                            (obj as FrameworkElement).Resources.MergedDictionaries.Add(rd);
                        }
                        catch
                        {
                        }
                    }
                    else if (visualStyle == "Office2007Silver")
                    {
                        try
                        {
                            rd.Source = new Uri("/Syncfusion.Shared.WPF;component/SkinManager/Office2007SilverStyle.xaml", UriKind.RelativeOrAbsolute);
                            (obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                            rd = SkinColorScheme.ApplyCustomColorScheme(rd, (value as SolidColorBrush).Color);
                            // This is not removing dictionary from merged collection.
                            //(obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                            SkinManager.RemoveDictionaryIfExist(obj as FrameworkElement, rd);
                            (obj as FrameworkElement).Resources.MergedDictionaries.Add(rd);
                        }
                        catch
                        {
                        }
                    }
                    else if (visualStyle == "SyncOrange")
                    {
                        try
                        {
                            rd.Source = new Uri("/Syncfusion.Shared.WPF;component/SkinManager/SyncOrangeStyle.xaml", UriKind.RelativeOrAbsolute);
                            (obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                            rd = SkinColorScheme.ApplyCustomColorScheme(rd, (value as SolidColorBrush).Color);
                            // This is not removing dictionary from merged collection.
                            //(obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                            SkinManager.RemoveDictionaryIfExist(obj as FrameworkElement, rd);
                            (obj as FrameworkElement).Resources.MergedDictionaries.Add(rd);
                        }
                        catch
                        {
                        }
                    }
                    else if (visualStyle == "ShinyRed")
                    {
                        try
                        {
                            rd.Source = new Uri("/Syncfusion.Shared.WPF;component/SkinManager/ShinyRedStyle.xaml", UriKind.RelativeOrAbsolute);
                            (obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                            rd = SkinColorScheme.ApplyCustomColorScheme(rd, (value as SolidColorBrush).Color);
                            // This is not removing dictionary from merged collection.
                            //(obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                            SkinManager.RemoveDictionaryIfExist(obj as FrameworkElement, rd);
                            (obj as FrameworkElement).Resources.MergedDictionaries.Add(rd);
                        }
                        catch
                        {
                        }
                    }
                    else if (visualStyle == "ShinyBlue")
                    {
                        try
                        {
                            rd.Source = new Uri("/Syncfusion.Shared.WPF;component/SkinManager/ShinyBlueStyle.xaml", UriKind.RelativeOrAbsolute);
                            (obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                            rd = SkinColorScheme.ApplyCustomColorScheme(rd, (value as SolidColorBrush).Color);
                            // This is not removing dictionary from merged collection.
                            //(obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                            SkinManager.RemoveDictionaryIfExist(obj as FrameworkElement, rd);
                            (obj as FrameworkElement).Resources.MergedDictionaries.Add(rd);
                        }
                        catch
                        {
                        }
                    }
                    else if (visualStyle == "Default")
                    {
                        try
                        {
                            rd.Source = new Uri("/Syncfusion.Shared.WPF;component/SkinManager/DefaultStyle.xaml", UriKind.RelativeOrAbsolute);
                            (obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                            rd = SkinColorScheme.ApplyCustomColorScheme(rd, (value as SolidColorBrush).Color);
                            // This is not removing dictionary from merged collection.
                            //(obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                            SkinManager.RemoveDictionaryIfExist(obj as FrameworkElement, rd);
                            (obj as FrameworkElement).Resources.MergedDictionaries.Add(rd);
                        }
                        catch
                        {
                        }
                    }
                    else if (visualStyle == "VS2010")
                    {
                        try
                        {
                            rd.Source = new Uri("/Syncfusion.Shared.WPF;component/SkinManager/VS2010Style.xaml", UriKind.RelativeOrAbsolute);
                            (obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                            rd = SkinColorScheme.ApplyCustomColorScheme(rd, (value as SolidColorBrush).Color);
                            // This is not removing dictionary from merged collection.
                            //(obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                            SkinManager.RemoveDictionaryIfExist(obj as FrameworkElement, rd);
                            (obj as FrameworkElement).Resources.MergedDictionaries.Add(rd);
                        }
                        catch
                        {
                        }
                    }
                    else if (visualStyle == "Office2010Blue")
                    {
                        try
                        {
                            rd.Source = new Uri("/Syncfusion.Shared.WPF;component/SkinManager/Office2010BlueStyle.xaml", UriKind.RelativeOrAbsolute);
                            (obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                            rd = SkinColorScheme.ApplyCustomColorScheme(rd, (value as SolidColorBrush).Color);
                            // This is not removing dictionary from merged collection.
                            //(obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                            SkinManager.RemoveDictionaryIfExist(obj as FrameworkElement, rd);
                            (obj as FrameworkElement).Resources.MergedDictionaries.Add(rd);
                        }
                        catch
                        {
                        }
                    }
                    else if (visualStyle == "Office2010Black")
                    {
                        try
                        {
                            rd.Source = new Uri("/Syncfusion.Shared.WPF;component/SkinManager/Office2010BlackStyle.xaml", UriKind.RelativeOrAbsolute);
                            (obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                            rd = SkinColorScheme.ApplyCustomColorScheme(rd, (value as SolidColorBrush).Color);
                            // This is not removing dictionary from merged collection.
                            //(obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                            SkinManager.RemoveDictionaryIfExist(obj as FrameworkElement, rd);
                            (obj as FrameworkElement).Resources.MergedDictionaries.Add(rd);
                        }
                        catch
                        {
                        }
                    }
                    else if (visualStyle == "Office2010Silver")
                    {
                        try
                        {
                            rd.Source = new Uri("/Syncfusion.Shared.WPF;component/SkinManager/Office2010SilverStyle.xaml", UriKind.RelativeOrAbsolute);
                            (obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                            rd = SkinColorScheme.ApplyCustomColorScheme(rd, (value as SolidColorBrush).Color);
                            // This is not removing dictionary from merged collection.
                            //(obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                            SkinManager.RemoveDictionaryIfExist(obj as FrameworkElement, rd);
                            (obj as FrameworkElement).Resources.MergedDictionaries.Add(rd);
                        }
                        catch
                        {
                        }
                    }
                    else if (visualStyle == "Metro")
                    {
                        try
                        {
                            rd.Source = new Uri("/Syncfusion.Shared.WPF;component/SkinManager/MetroStyle.xaml", UriKind.RelativeOrAbsolute);
                            (obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                            rd = SkinColorScheme.ApplyCustomColorScheme(rd, (value as SolidColorBrush).Color);
                            // This is not removing dictionary from merged collection.
                            //(obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                            SkinManager.RemoveDictionaryIfExist(obj as FrameworkElement, rd);
                            (obj as FrameworkElement).Resources.MergedDictionaries.Add(rd);
                        }
                        catch
                        {
                        }
                    }
                    else if (visualStyle == "Transparent")
                    {
                        try
                        {
                            rd.Source = new Uri("/Syncfusion.Shared.WPF;component/SkinManager/TransparentStyle.xaml", UriKind.RelativeOrAbsolute);
                            (obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                            rd = SkinColorScheme.ApplyCustomColorScheme(rd, (value as SolidColorBrush).Color);
                            // This is not removing dictionary from merged collection.
                            //(obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                            SkinManager.RemoveDictionaryIfExist(obj as FrameworkElement, rd);
                            (obj as FrameworkElement).Resources.MergedDictionaries.Add(rd);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetActiveColorScheme(DependencyObject obj, SolidColorBrush value)
        {
            obj.ClearValue(ActiveColorSchemeProperty);          
            obj.SetValue(ActiveColorSchemeProperty, value);
            ApplyActiveColorToMSControls(obj, value);
        }
        #endregion
    }
}
