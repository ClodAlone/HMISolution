#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Controls.Theming
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Resources;
    using System.Linq;
    using Syncfusion.Windows.Controls.Theming;
    using System.ComponentModel;
    using Syncfusion.Windows.Shared;
    using System.Diagnostics;


    #region VisualStyle

    /// <summary>
    /// Defines identifiers that represent control themes.
    /// </summary>
    public enum VisualStyle : sbyte
    {
        /// <summary>
        /// Indicates usage of default control theme
        /// </summary>
        Default,

        /// <summary>
        /// Indicates usage of Blend theme
        /// </summary>
        Blend,

        /// <summary>
        /// Indicates usage of Office 2003 theme
        /// </summary>
        Office2003,

        /// <summary>
        /// Indicates usage of Office 2007 blue theme
        /// </summary>
        Office2007Blue,

        /// <summary>
        /// Indicates usage of Office 2007 black theme
        /// </summary>
        Office2007Black,

        /// <summary>
        /// Indicates usage of Office 2007 silver theme
        /// </summary>
        Office2007Silver,

        /// <summary>
        /// Indicates usage of Office 2010 blue theme
        /// </summary>
        Office2010Blue,

        /// <summary>
        /// Indicates usage of Office 2010 black theme
        /// </summary>
        Office2010Black,

        /// <summary>
        /// Indicates usage of Office 2010 silver theme
        /// </summary>
        Office2010Silver,

        /// <summary>
        /// Indicates usage of Windows7 theme
        /// </summary>
        Windows7,

        /// <summary>
        /// Indicates usage of VS2010 theme
        /// </summary>
        VS2010,

        /// <summary>
        /// Indicates usage of Metro Theme
        /// </summary>        
        Metro,

        /// <summary>
        /// Indicates usage of Transparent Theme
        /// </summary>        
        Transparent,
    }

    #endregion


    /// <summary>
    /// Represents SkinManager
    /// </summary>
    public class SkinManager : DependencyObject
    {
        #region Private Members

        /// <summary>
        /// Used to store the values from dictionary.
        /// </summary>
        private static object value;

        /// <summary>
        /// Used to store the values from dictionary.
        /// </summary>
        private static object themevalue;

        ///// <summary>
        ///// Used to store the active skin.
        ///// </summary>
        //private static VisualStyle activeSkin;

        ///// <summary>
        ///// Used to set the custombrush
        ///// </summary>
        //private bool isCustomColor = false;

        /// <summary>
        /// Used to store the custom color.
        /// </summary>
        private static Color color;

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the visual style of the control.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>VisualStyle of the control</returns>
        public static VisualStyle GetVisualStyle(DependencyObject obj)
        {
            return (VisualStyle)obj.GetValue(VisualStyleProperty);
        }

        


        /// <summary>
        /// Sets the visual style of the control.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetVisualStyle(DependencyObject obj, VisualStyle value)
        {
            if (obj != null)
            {
                obj.SetValue(VisualStyleProperty, value);
                FrameworkElement element = obj as FrameworkElement;
                if (element != null)
                    if (element.GetVisualChildren().Count() == 0)
                    element.Loaded += new RoutedEventHandler(control_Loaded);

                

            }
        }



        internal static void ApplyVisualStyle(DependencyObject obj, VisualStyle visualstyle)
        {
            if (obj != null)
            {
                ElementCollection.Clear();
                PropagateVisualTree(0, obj);

                ElementCollection.ForEach(element => MergeSyncControlsStyle(element, GetActiveColorScheme(obj), visualstyle, obj as FrameworkElement));

                if (obj is ISkinStylePropagator)
                {
                    ((ISkinStylePropagator)obj).OnStyleChanged(visualstyle);
                }

                var rd = new ResourceDictionary();

                try
                {
                    rd.Source = new Uri("/Syncfusion.Theming." + visualstyle.ToString() + ";component/MSControls.xaml", UriKind.RelativeOrAbsolute);
                    SkinManager.RemoveDictionaryIfExist(obj as FrameworkElement, rd);
                    (obj as FrameworkElement).Resources.MergedDictionaries.Add(rd);

                }
                catch { }

                //ImplicitStyleManager.SetApplyMode(obj as FrameworkElement, ImplicitStylesApplyMode.Auto);
                //ImplicitStyleManager.Apply(obj as FrameworkElement);
                ElementCollection.Clear();
            }
        }


        static void control_Loaded(object sender, RoutedEventArgs e)
        {

            FrameworkElement element = sender as FrameworkElement;
            if (element != null)
            {
                ApplyVisualStyle(element, GetVisualStyle(element));
                element.Loaded -= new RoutedEventHandler(control_Loaded);
            }
        }
        /// <summary>
        /// Gets the value of the ActiveColorScheme property for a given element.
        /// </summary>
        /// <param name="obj">The element for which to retrieve the ZIndex value.</param>
        /// <returns>Return the active color</returns>
        public static Color GetActiveColorScheme(DependencyObject obj)
        {
            return (Color)obj.GetValue(ActiveColorSchemeProperty);
        }

        /// <summary>
        /// Removes the dictionary if exist.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="dictionary">The dictionary.</param>
        internal static void RemoveDictionaryIfExist(FrameworkElement element, ResourceDictionary dictionary)
        {

            if (element != null)
            {

                for (int i = 0; i < element.Resources.MergedDictionaries.Count; i++)
                {
                    var rdic = element.Resources.MergedDictionaries[i];
                    if (rdic.Source == dictionary.Source)
                    {
                        element.Resources.MergedDictionaries.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
        /// <summary>
        /// Refreshes the skin manager. 
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <remarks>When elements added at runtime, skin manager should be refreshed.</remarks>
        public static void Refresh(DependencyObject obj)
        {
            if (obj is FrameworkElement)
            {

                var rd = new ResourceDictionary();

                try
                {
                    rd.Source = new Uri("/Syncfusion.Theming." + value.ToString() + ";component/MSControls.xaml", UriKind.RelativeOrAbsolute);
                    SkinManager.RemoveDictionaryIfExist(obj as FrameworkElement, rd);

                    if (!SkinManager.GetIsCustomBrushEnabled(obj))
                    {
                        rd = SkinColorScheme.ApplyCustomColorScheme(rd, SkinManager.GetActiveColorScheme(obj));
                    }
                    else
                    {
                        var customBrushDictionary = SkinManager.GetCustomBrushDictionary(obj);

                        if (customBrushDictionary is ResourceDictionary)
                        {
                            rd = SkinManager.CustomBrush(customBrushDictionary, rd);
                        }
                    }

                    (obj as FrameworkElement).Resources.MergedDictionaries.Add(rd);

                }
                catch { }
               
                ImplicitStyleManager.SetApplyMode(obj as FrameworkElement, ImplicitStylesApplyMode.Auto);
                ImplicitStyleManager.Apply(obj as FrameworkElement);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="obj"></param>
        public static void PropagateVisualTree(int depth, DependencyObject obj)
        {
            if (obj != null)
            {
                //Debug.WriteLine(new string(' ', depth) + obj);
                if (obj.GetType().ToString().StartsWith("Syncfusion."))
                    ElementCollection.Add(obj as FrameworkElement);
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                    PropagateVisualTree(depth + 1, VisualTreeHelper.GetChild(obj, i));
            }
        }

        //private static FrameworkElement SkinElement;

        //private static VisualStyle CurrentStyle;

        //private static Color CurrentColorScheme;

        private static ObservableCollection<FrameworkElement> ElementCollection = new ObservableCollection<FrameworkElement>();

        /// <summary>
        /// Merges the sync controls style.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="skincolor">The skincolor.</param>
        /// <param name="visualStyle">The visual style.</param>
        /// <param name="elementToMerge"></param>
        private static void MergeSyncControlsStyle(FrameworkElement element, Color skincolor, VisualStyle visualStyle, FrameworkElement elementToMerge)
        {
            VisualStyle currentskin = visualStyle;

            //BaseMergedStyleDictionary initialDictionary = ImplicitStyleManager.GetMergedStyleDictionary(element);

            //IEnumerable<Tuple<FrameworkElement, BaseMergedStyleDictionary>> elementsToStyleAndDictionaries =
            //    FunctionalProgramming.TraverseDepthFirst(
            //        new Tuple<FrameworkElement, BaseMergedStyleDictionary>(element, initialDictionary),
            //        (elementAndDictionary) =>
            //            elementAndDictionary
            //                .First
            //                .GetLogicalChildrenDepthFirst()
            //                .Select(childElement =>
            //                    new Tuple<FrameworkElement, BaseMergedStyleDictionary>(
            //                        childElement,
            //                        new MergedStyleResourceDictionary(
            //                            ImplicitStyleManager.GetExternalResourceDictionary(childElement) ?? childElement.Resources,
            //                            elementAndDictionary.Second))),
            //        (elementAndDictionary) => true ||
            //            (ImplicitStyleManager.GetApplyMode(elementAndDictionary.First) != ImplicitStylesApplyMode.OneTime ||
            //            !ImplicitStyleManager.GetHasBeenStyled(elementAndDictionary.First)));

            //foreach (Tuple<FrameworkElement, BaseMergedStyleDictionary> elementToStyleAndDictionary in elementsToStyleAndDictionaries)
            //{
                FrameworkElement elementToStyle = element;

                SkinTypeAttribute skinControl = GetSkinAttribute(element, currentskin);
                

                if (skinControl != null)
                {
                    var themeName = skinControl.SkinVisualStyle;
                    var xamlName = skinControl.XamlResource;
                    var type = skinControl.Type;
                    var rd = new ResourceDictionary();
                    rd.Source = new Uri(xamlName, UriKind.RelativeOrAbsolute);
                    if (SkinManager.GetActiveColorScheme(element) != new Color())
                    {
                        rd = SkinColorScheme.ApplyCustomColorScheme(rd, SkinManager.GetActiveColorScheme(element));
                    }
                    if (SkinManager.GetIsCustomBrushEnabled(element))
                    {
                        if (SkinManager.GetCustomBrushDictionary(element) is ResourceDictionary)
                        {
                            rd = SkinManager.CustomBrush(SkinManager.GetCustomBrushDictionary(element), rd);
                        }
                    }
                    SkinManager.RemoveDictionaryIfExist(elementToMerge, rd);
                    elementToMerge.Resources.MergedDictionaries.Add(rd);
                }

            //}
        }

        /// <summary>
        /// Gets the skin attribute.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="currentskin">The currentskin.</param>
        /// <returns></returns>
        private static SkinTypeAttribute GetSkinAttribute(FrameworkElement element, VisualStyle currentskin)
        {
            SkinTypeAttribute skinTypeAttr = null;
            if (element != null)
            {
                System.Reflection.MemberInfo inf = element.GetType();
                var attributes = inf.GetCustomAttributes(true);
                for (int i = 0; i < attributes.Length; i++)
                {
                    var attr = attributes[i] as SkinTypeAttribute;
                    if (attr != null && attr.SkinVisualStyle.ToString() == currentskin.ToString())
                    {
                        skinTypeAttr = attr;
                        break;
                    }
                }
            }
            return skinTypeAttr;
        }


        /// <summary>
        /// Sets the value of the ActiveColorScheme property for a given element.
        /// </summary>
        /// <param name="obj">The element on which to apply the property value.</param>
        /// <param name="value">Active scheme brush.</param>
        public static void SetActiveColorScheme(DependencyObject obj, Color value)
        {
            obj.SetValue(ActiveColorSchemeProperty, (Color)value);

            color = (Color)value;

            if (color != null)
            {
                if (SkinManager.GetActiveColorScheme(obj as FrameworkElement) != new Color())
                {
                    var rd = new ResourceDictionary();
                    try
                    {
                        rd.Source = new Uri("/Syncfusion.Theming." + SkinManager.GetVisualStyle(obj) + ";component/MSControls.xaml", UriKind.RelativeOrAbsolute);
                        (obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                        rd = SkinColorScheme.ApplyCustomColorScheme(rd, color);
                        // This is not removing dictionary from merged collection.
                        //(obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                        SkinManager.RemoveDictionaryIfExist(obj as FrameworkElement, rd);
                        (obj as FrameworkElement).Resources.MergedDictionaries.Add(rd);
                    }
                    catch { }

                    MergeSyncControlsStyle(obj as FrameworkElement, color, SkinManager.GetVisualStyle(obj), obj as FrameworkElement);
                    ImplicitStyleManager.SetApplyMode(obj as FrameworkElement, ImplicitStylesApplyMode.Auto);
                    ImplicitStyleManager.Apply(obj as FrameworkElement);

                }

            }
        }

        /// <summary>
        /// Gets the is custom brush enabled property for a given element.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetIsCustomBrushEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsCustomBrushEnabledProperty);
        }

        /// <summary>
        /// Sets the is custom brush enabled.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetIsCustomBrushEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsCustomBrushEnabledProperty, value);
        }

        /// <summary>
        /// Gets the custom brush ResourceRdictionary.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>Returns the ResourceDictionary</returns>
        public static ResourceDictionary GetCustomBrushDictionary(DependencyObject obj)
        {
            return (ResourceDictionary)obj.GetValue(CustomBrushDictionaryProperty);
        }

        /// <summary>
        /// Sets the custom brush resource dictionary.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetCustomBrushDictionary(DependencyObject obj, ResourceDictionary value)
        {
            obj.SetValue(CustomBrushDictionaryProperty, value);
        }


        #endregion

        #region Events

        /// <summary>
        /// Occurs when [visual style changed].
        /// </summary>
        public event PropertyChangedCallback VisualStyleChanged;

        /// <summary>
        /// Called when [visual style changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnVisualStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            SkinManager sender = d as SkinManager;
            ApplyVisualStyle(d, (VisualStyle)e.NewValue);
            if (sender != null)
            {
                sender.OnVisualStyleChanged(e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="VisualStyle"/> event.
        /// </summary>
        /// <param name="e">
        /// Property change details, such as old value and new value.</param>
        protected virtual void OnVisualStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (VisualStyleChanged != null)
            {
                VisualStyleChanged(this, e);
            }
        }

        #endregion

        #region Dependency Property
        /// <summary>
        /// Using a DependencyProperty as the backing store for VisualStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VisualStyleProperty =
            DependencyProperty.RegisterAttached("VisualStyle", typeof(VisualStyle), typeof(SkinManager), new PropertyMetadata(VisualStyle.Default, new PropertyChangedCallback(OnVisualStyleChanged)));

        /// <summary>
        /// Defines Custom Color Scheme is applied to control. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty ActiveColorSchemeProperty =
           DependencyProperty.RegisterAttached("ActiveColorScheme", typeof(Color), typeof(SkinManager), new PropertyMetadata(new Color()));

        /// <summary>
        /// Defines whether the Custom Brush is applied to the control. This is dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCustomBrushEnabledProperty =
          DependencyProperty.RegisterAttached("IsCustomBrushEnabled", typeof(bool), typeof(SkinManager), new PropertyMetadata(false));

        /// <summary>
        /// Defines the CustomBrush Resource Dictionary. This is dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomBrushDictionaryProperty =
         DependencyProperty.RegisterAttached("CustomBrushDictionary", typeof(ResourceDictionary), typeof(SkinManager), new PropertyMetadata(null, OnCustomBrushChanged));

        #endregion

        #region Methods

        /// <summary>
        /// Called when [custom brush changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCustomBrushChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {

            if (args.NewValue is ResourceDictionary)
            {
                ResourceDictionary dict = args.NewValue as ResourceDictionary;

                if (dict != null && SkinManager.GetIsCustomBrushEnabled(obj))
                {
                    var rd = new ResourceDictionary();
                    try
                    {
                        rd.Source = new Uri("/Syncfusion.Theming." + SkinManager.GetVisualStyle(obj) + ";component/MSControls.xaml", UriKind.RelativeOrAbsolute);
                        rd = CustomBrush(dict, rd);
                        (obj as FrameworkElement).Resources.MergedDictionaries.Remove(rd);
                        SkinManager.RemoveDictionaryIfExist(obj as FrameworkElement, rd);
                        (obj as FrameworkElement).Resources.MergedDictionaries.Add(rd);
                        ImplicitStyleManager.Apply(obj as FrameworkElement);
                    }
                    catch { }

                }

            }
        }

        /// <summary>
        /// Custom the brush.
        /// </summary>
        /// <param name="brushDict">The brush dict.</param>
        /// <param name="themeDictionary">The theme dictionary.</param>
        /// <returns></returns>
        internal static ResourceDictionary CustomBrush(ResourceDictionary brushDict, ResourceDictionary themeDictionary)
        {

            ResourceDictionary dictionary = null;
            foreach (ResourceDictionary mergeddic in themeDictionary.MergedDictionaries)
            {
                if (mergeddic.Source.ToString().EndsWith("Brushes.xaml"))
                {
                    dictionary = mergeddic;
                    break;
                }
            }

            if (dictionary != null)
            {
                dictionary = MergeCustomBrush(brushDict, dictionary);
                themeDictionary.MergedDictionaries.Remove(dictionary);
                themeDictionary.MergedDictionaries.Add(dictionary);
            }

            return MergeCustomBrush(brushDict, themeDictionary);
        }

        /// <summary>
        /// Merges the custom brush.
        /// </summary>
        /// <param name="brushDict">The brush dict.</param>
        /// <param name="themeDictionary">The theme dictionary.</param>
        /// <returns></returns>
        private static ResourceDictionary MergeCustomBrush(ResourceDictionary brushDict, ResourceDictionary themeDictionary)
        {
            foreach (string key in brushDict.Keys)
            {
                value = brushDict[key];
                themevalue = themeDictionary[key];

                if (themeDictionary.Contains(key))
                {

                    if (value != null)
                    {
                        if (value is LinearGradientBrush)
                        {
                            if (themevalue is LinearGradientBrush)
                            {

                                LinearGradientBrush brush = value as LinearGradientBrush;
                                LinearGradientBrush newBrush = new LinearGradientBrush();

                                newBrush.StartPoint = brush.StartPoint;
                                newBrush.EndPoint = brush.EndPoint;
                                newBrush.Transform = brush.Transform;

                                foreach (GradientStop stop in brush.GradientStops)
                                {
                                    GradientStop newStop = new GradientStop();
                                    newStop.Offset = stop.Offset;
                                    newStop.Color = stop.Color;
                                    newBrush.GradientStops.Add(newStop);
                                }

                                (themeDictionary[key] as LinearGradientBrush).StartPoint = newBrush.StartPoint;
                                (themeDictionary[key] as LinearGradientBrush).EndPoint = newBrush.EndPoint;
                                (themeDictionary[key] as LinearGradientBrush).Transform = newBrush.Transform;
                                (themeDictionary[key] as LinearGradientBrush).GradientStops.Clear();

                                foreach (GradientStop newstops in newBrush.GradientStops)
                                {

                                    GradientStop stop = clone(newstops, new GradientStop());
                                    (themeDictionary[key] as LinearGradientBrush).GradientStops.Add(stop);
                                }
                            }
                        }
                        else if (value is SolidColorBrush)
                        {
                            if (themevalue is SolidColorBrush)
                            {
                                SolidColorBrush newBrush = new SolidColorBrush();
                                (themeDictionary[key] as SolidColorBrush).Color = (value as SolidColorBrush).Color;
                            }

                        }
                        else if (value is RadialGradientBrush)
                        {
                            if (themevalue is RadialGradientBrush)
                            {
                                RadialGradientBrush brush = value as RadialGradientBrush;
                                RadialGradientBrush newBrush = new RadialGradientBrush();

                                newBrush.Center = brush.Center;
                                newBrush.GradientOrigin = brush.GradientOrigin;
                                newBrush.RadiusX = brush.RadiusX;
                                newBrush.RadiusY = brush.RadiusY;
                                newBrush.RelativeTransform = brush.RelativeTransform;

                                foreach (GradientStop stop in brush.GradientStops)
                                {
                                    GradientStop newStop = new GradientStop();
                                    newStop.Offset = stop.Offset;
                                    newStop.Color = stop.Color;
                                    newBrush.GradientStops.Add(newStop);
                                }

                                (themeDictionary[key] as RadialGradientBrush).Center = newBrush.Center;
                                (themeDictionary[key] as RadialGradientBrush).GradientOrigin = newBrush.GradientOrigin;
                                (themeDictionary[key] as RadialGradientBrush).RelativeTransform = newBrush.RelativeTransform;
                                (themeDictionary[key] as RadialGradientBrush).RadiusX = newBrush.RadiusX;
                                (themeDictionary[key] as RadialGradientBrush).RadiusY = newBrush.RadiusY;
                                (themeDictionary[key] as RadialGradientBrush).GradientStops.Clear();

                                foreach (GradientStop newstops in newBrush.GradientStops)
                                {
                                    GradientStop stop = clone(newstops, new GradientStop());
                                    (themeDictionary[key] as RadialGradientBrush).GradientStops.Add(stop);
                                }
                            }
                        }
                    }
                }
            }

            return themeDictionary;
        }

        /// <summary>
        /// Clones the specified parent.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="child">The child.</param>
        /// <returns>Returns the GradientStop.</returns>
        private static GradientStop clone(GradientStop parent, GradientStop child)
        {
            child.Color = parent.Color;
            child.Offset = parent.Offset;
            return child;
        }

        #endregion
    }
    /// <summary>
    /// 
    /// </summary>
    public interface ISkinStylePropagator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="visualStyle"></param>
        void OnStyleChanged(VisualStyle visualStyle);
    }
}

