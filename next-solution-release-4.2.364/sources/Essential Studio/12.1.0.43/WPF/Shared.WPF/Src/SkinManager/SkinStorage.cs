// <copyright file="SkinStorage.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Windows;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Class attaches properties for work with skins.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public partial  class SkinStorage : DependencyObject
    {
        #region Constants
        /// <summary>
        /// Default skin name.
        /// </summary>
        private const string DefaultName = "Default";

        #region NewSkinCode Constants
        private static bool IsUserControlPresent, IsPageControlPresent;
        private static ObservableCollection<FrameworkElement> Root;
        private static bool IsSkinNotChanged = false;
        #endregion

        private static bool windowflag = false;
        #endregion
       
        #region Public method

        private static void RemoveDictionaryIfExist(FrameworkElement element, ResourceDictionary dictionary)
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
        /// Gets current skin name from given object.
        /// </summary>
        /// <param name="obj">Given object.</param>
        /// <returns><see cref="String"/> value that represents current skin name of given object.</returns>
        public static string GetVisualStyle(DependencyObject obj)
        {
            return (String)obj.GetValue(VisualStyleProperty);
        }
        /// <summary>
        /// Gets the flag which indicates overriding visual style from given object.
        /// </summary>
        /// <param name="obj">Given object.</param>
        /// <returns><see cref="String"/> value that represents current skin name of given object.</returns>
        public static bool GetOverrideVisualStyle(DependencyObject obj)
        {
            return (bool)obj.GetValue(OverrideVisualStyleProperty);
        }
        /// <summary>
        /// Sets new skin name for given object.
        /// </summary>
        /// <param name="obj">Given object.</param>
        /// <param name="value">New skin name.</param>
        public static void SetVisualStyle(DependencyObject obj, string value)
        {
            SkinStorage.IsThemeChangeNotNeeded = false;

            #region NewSkinCode

            if (obj!=null && SkinStorage.GetEnableOptimization(obj))
            {
                ResetResources(obj, value);
            }
            #endregion

            obj.SetValue(VisualStyleProperty, value);
            if (obj is ISkinStylePropagator)
            {
                ((ISkinStylePropagator)obj).OnStyleChanged(value);
            }
        }
        /// <summary>
        /// Sets the flag which indicates overriding visual style for given object.
        /// </summary>
        /// <param name="obj">Given object.</param>
        /// <param name="value">New skin name.</param>
        public static void SetOverrideVisualStyle(DependencyObject obj, bool value)
        {
            obj.SetValue(OverrideVisualStyleProperty, value);
        }

        #endregion

        #region Dependency Properties

        #region Newskincode DP

        internal static void SetMergedDictionaryPath(DependencyObject obj, string value)
        {
            obj.SetValue(MergedDictionaryPathProperty, value);
        }

        internal static ObservableCollection<string> GetMergedDictionaryPath(DependencyObject obj)
        {
            return (ObservableCollection<string>)obj.GetValue(MergedDictionaryPathProperty);
        }

        internal static readonly DependencyProperty MergedDictionaryPathProperty = DependencyProperty.RegisterAttached(
        "MergedDictionaryPath",
        typeof(ObservableCollection<string>),
        typeof(SkinStorage),
        new FrameworkPropertyMetadata(new ObservableCollection<string>()));


        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetEnableOptimization(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableOptimizationProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetEnableOptimization(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableOptimizationProperty, value);
        }

        // Using a DependencyProperty as the backing store for EnableOptimization.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty EnableOptimizationProperty =
            DependencyProperty.RegisterAttached("EnableOptimization", typeof(bool), typeof(SkinStorage), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnEnableOptimizationChanged)));
      
        /// <summary>
        /// Identifies <see cref="Syncfusion.Windows.Shared.SkinStorage.VisualStyleProperty"/> dependency attached property.
        /// </summary>
        public static readonly DependencyProperty VisualStyleProperty = DependencyProperty.RegisterAttached(
            "VisualStyle",
            typeof(string),
            typeof(SkinStorage),
            new FrameworkPropertyMetadata(DefaultName, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnVisualStyleChanged)));
        /// <summary>
        /// Identifies <see cref="Syncfusion.Windows.Shared.SkinStorage.OverrideVisualStyleProperty"/> dependency attached property.
        /// </summary>
        public static readonly DependencyProperty OverrideVisualStyleProperty = DependencyProperty.RegisterAttached(
           "OverrideVisualStyle",
           typeof(bool),
           typeof(SkinStorage),
           new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.OverridesInheritanceBehavior, new PropertyChangedCallback(OnOverrideVisualStyleChanged)));

        // Using a DependencyProperty as the backing store for IsMSDictionaryMerged.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsMSDictionaryMergedProperty =
            DependencyProperty.RegisterAttached("IsMSDictionaryMerged", typeof(bool), typeof(SkinStorage), new UIPropertyMetadata(false));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetEnableTouch(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableTouchProperty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetEnableTouch(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableTouchProperty, value);
        }

        // Using a DependencyProperty as the backing store for EnableTouch.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty EnableTouchProperty =
            DependencyProperty.RegisterAttached("EnableTouch", typeof(bool), typeof(SkinStorage), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
        #endregion

        internal static bool GetIsDictionaryMerged(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDictionaryMergedProperty);
        }

        internal static void SetIsDictionaryMerged(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDictionaryMergedProperty, value);
        }
        // Using a DependencyProperty as the backing store for IsDictionaryMerged.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsDictionaryMergedProperty =
            DependencyProperty.RegisterAttached("IsDictionaryMerged", typeof(bool), typeof(SkinStorage), new UIPropertyMetadata(false));



        internal static bool GetIsMSDictionaryMerged(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsMSDictionaryMergedProperty);
        }

        internal static void SetIsMSDictionaryMerged(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMSDictionaryMergedProperty, value);
        }
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        private static bool isThemeChangeNotNeeded=false;
        /// <summary>
        /// 
        /// </summary>
        public static bool IsThemeChangeNotNeeded
        { 
            get { return isThemeChangeNotNeeded; }
            set { isThemeChangeNotNeeded = value; }
        }
        
        #region Implementation


        private static void OnEnableOptimizationChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// Called when [visual style changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnVisualStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            string visualStyle = e.NewValue as string;
            //obj.SetValue(VisualStyleProperty, visualStyle);
            bool styleChangeNotNeeded=(bool)obj.GetValue(OverrideVisualStyleProperty);
            if (styleChangeNotNeeded)
                return;
            
            if (!SkinStorage.GetEnableOptimization(obj))
            {
                string s = GetVisualStyle(obj);
                if ((obj as FrameworkElement) != null && !IsThemeChangeNotNeeded)
                {

                    if (((obj as FrameworkElement).Parent is Window && !windowflag) || ((obj as FrameworkElement).Parent is ChromelessWindow && !windowflag) || (obj is Window) || (obj is ChromelessWindow) || (obj is UserControl) || (obj is Page))
                    {
                        if (obj is Window || obj is ChromelessWindow)
                        {
                            windowflag = true;
                        }

                        DependencyObject root = LogicalTreeHelper.GetParent(obj);
                        if (root == null)
                            root = VisualTreeHelper.GetParent(obj);
                        var rd = new ResourceDictionary();
                        String Style = SkinStorage.GetVisualStyle(obj).ToString();
                        String rootstyle = (root == null) ? (string.Empty) : (SkinStorage.GetVisualStyle(root).ToString());

                        try
                        {
                            rd.Source = new Uri("/Syncfusion.Shared.WPF;component/SkinManager/" + Style + "Style.xaml", UriKind.RelativeOrAbsolute);
                            if (rootstyle != Style)
                            {
                                SkinStorage.SetIsDictionaryMerged(obj, true);
                                RemoveDictionaryIfExist(obj as FrameworkElement, rd);
                                (obj as FrameworkElement).Resources.MergedDictionaries.Add(rd);
                            }
                            else
                            {
                                if (SkinStorage.GetIsDictionaryMerged(obj))
                                {
                                    RemoveDictionaryIfExist(obj as FrameworkElement, rd);
                                    (obj as FrameworkElement).Resources.MergedDictionaries.Add(rd);
                                }
                            }
                        }
                        catch { }
                    }

                    if (IsApply(obj as FrameworkElement))
                    {
                        ApplySkin(obj, GetVisualStyle(obj));
                    }
                }
            }
            else
            {
                ApplyOptimization(obj, e);
            }

        }
        /// <summary>
        /// Called when [overridevisualstyle changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnOverrideVisualStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
        }
        
        private static SkinTypeAttribute GetSkinAttribute(FrameworkElement element, string currentskin)
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


        private static SkinTypeAttribute GetSkinAttribute(Control element, string currentskin)
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
        /// Applies the skin.
        /// </summary>
        /// <param name="obj">The fe.</param>
        /// <param name="style">The style.</param>
        private static void ApplySkin(DependencyObject obj, string style)
        {
            if ((obj as Visual) != null)
            {
                if (obj is ContextMenu)
                {
                    FrameworkElement element = obj as FrameworkElement;
                    ResourceDictionary rd = MergeDic(new ResourceDictionary(), style);
                    RemoveDictionaryIfExist(element, rd);
                    element.Resources.MergedDictionaries.Add(rd);
                }
                IEnumerable<DependencyObject> child = VisualUtils.EnumLogicalChildrenOfType(obj, typeof(FrameworkElement));

                foreach (FrameworkElement fe in child)
                {
                    if (fe != null && IsApply(fe))
                    {

                        if (fe is Popup)
                        {
                            if ((fe as Popup).Child as Visual != null)
                            {
                                IEnumerable<Visual> popupChild = VisualUtils.EnumChildrenOfType((fe as Popup).Child as Visual, typeof(FrameworkElement));
                                if (popupChild != null)
                                {
                                    ControlIterate(popupChild, style);
                                }
                            }
                        }
                        if (fe is ScrollViewer && fe is Visual)
                        {
                            if (((fe as ScrollViewer).Content as Visual) != null)
                            {
                                IEnumerable<Visual> scrollChild = VisualUtils.EnumChildrenOfType((fe as ScrollViewer).Content as Visual, typeof(FrameworkElement));
                                if (scrollChild != null)
                                {
                                    ControlIterate(scrollChild, style);
                                }
                            }
                        }

                        OuterControlIterate(fe, style);

                        if (fe.GetType().FullName.Contains("Syncfusion"))
                            SetVisualStyle(fe, style);

                    }
                }


                if ((obj as FrameworkElement) != null)
                {
                    OuterControlIterate((obj as FrameworkElement), style);
                }

            }

        }

        private static bool IsMSDictionaryMrgedInParent(FrameworkElement fe)
        {
            if(VisualUtils.FindLogicalAncestor(fe,typeof(Window))!=null||VisualUtils.FindLogicalAncestor(fe,typeof(UserControl ))!=null|| VisualUtils.FindLogicalAncestor(fe,typeof(Page))!=null||VisualUtils.FindLogicalAncestor(fe,typeof(ChromelessWindow ))!=null)

            {
               if( SkinStorage.GetVisualStyle(fe)==SkinStorage.GetVisualStyle(fe.Parent) && !GetIsMSDictionaryMerged(fe))
                   return true;
               else
                   return false;
            }
            return false;
        }

        private static void OuterControlIterate(FrameworkElement fe, string style)
        {

            var skinControls1 = GetSkinAttribute(fe, style);

            if (skinControls1 != null)
            {
                var themeName = skinControls1.SkinVisualStyle;
                var xamlName = skinControls1.XamlResource;
                var type = skinControls1.Type;
                var rdict = new ResourceDictionary();
                rdict.Source = new Uri(xamlName, UriKind.RelativeOrAbsolute);
                if (!IsMSDictionaryMrgedInParent(fe))
                {
                    SetIsMSDictionaryMerged(fe, true);
                    rdict = MergeDic(rdict, style);
                }
                try
                {
                    RemoveDictionaryIfExist(fe, rdict);

                    fe.Resources.MergedDictionaries.Add(rdict);
                }
                catch { }
            }


        }


        private static void ControlIterate(IEnumerable<Visual> fe, string style)
        {

            foreach (FrameworkElement frameSkin in fe)
            {
                //frameSkin.GetType().BaseType.GetConstructor(frameSkin.GetType().BaseType.GetType()).Invoke();
                if (frameSkin != null && IsApply(frameSkin))
                {
                    var skinControls = GetSkinAttribute(frameSkin, style);

                    if (skinControls != null)
                    {
                        var themeName = skinControls.SkinVisualStyle;
                        var xamlName = skinControls.XamlResource;
                        var type = skinControls.Type;
                        var rd = new ResourceDictionary();
                        rd.Source = new Uri(xamlName, UriKind.RelativeOrAbsolute);

                        
                        rd = MergeDic(rd, style);
                        try
                        {
                            RemoveDictionaryIfExist(frameSkin, rd);
                            frameSkin.Resources.MergedDictionaries.Add(rd);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }


        private static bool IsApply(FrameworkElement frameSkin)
        {
            if (!(frameSkin is Panel) && !(frameSkin is Image) && !(frameSkin is Decorator) && !(frameSkin is Shape))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static ResourceDictionary MergeDic(ResourceDictionary rd, String skin)
        {
       
            ResourceDictionary rd1 = new ResourceDictionary();
            if (skin == "Blend")
            {
                rd1.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/BlendStyle.xaml", UriKind.RelativeOrAbsolute);
                rd.MergedDictionaries.Add(rd1);

            }
            if (skin == "Office2007Blue")
            {
                rd1.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/Office2007BlueStyle.xaml", UriKind.RelativeOrAbsolute);
                rd.MergedDictionaries.Add(rd1);

            }
            if (skin == "Office2007Black")
            {
                rd1.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/Office2007BlackStyle.xaml", UriKind.RelativeOrAbsolute);
                rd.MergedDictionaries.Add(rd1);
            }

            if (skin == "Office2007Silver")
            {
                rd1.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/Office2007SilverStyle.xaml", UriKind.RelativeOrAbsolute);
                rd.MergedDictionaries.Add(rd1);

            }
            if (skin == "Office2003")
            {
                rd1.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/Office2003Style.xaml", UriKind.RelativeOrAbsolute);
                rd.MergedDictionaries.Add(rd1);
                return rd;
            }
            if (skin == "SyncOrange")
            {
                rd1.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/SyncOrangeStyle.xaml", UriKind.RelativeOrAbsolute);
                rd.MergedDictionaries.Add(rd1);

            }
            if (skin == "ShinyRed")
            {
                rd1.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/ShinyRedStyle.xaml", UriKind.RelativeOrAbsolute);
                rd.MergedDictionaries.Add(rd1);

            }
            if (skin == "ShinyBlue")
            {
                rd1.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/ShinyBlueStyle.xaml", UriKind.RelativeOrAbsolute);
                rd.MergedDictionaries.Add(rd1);
            }
            if (skin == "Default")
            {
                rd1.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/DefaultStyle.xaml", UriKind.RelativeOrAbsolute);
                rd.MergedDictionaries.Add(rd1);
            }
            if (skin == "Office2010Blue")
            {
                rd1.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/Office2010BlueStyle.xaml", UriKind.RelativeOrAbsolute);
                rd.MergedDictionaries.Add(rd1);
            }
            if (skin == "Office2010Black")
            {
                rd1.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/Office2010BlackStyle.xaml", UriKind.RelativeOrAbsolute);
                rd.MergedDictionaries.Add(rd1);
            }
            if (skin == "Office2010Silver")
            {
                rd1.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/Office2010SilverStyle.xaml", UriKind.RelativeOrAbsolute);
                rd.MergedDictionaries.Add(rd1);
            }
            if (skin == "VS2010")
            {
                rd1.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/VS2010Style.xaml", UriKind.RelativeOrAbsolute);
                rd.MergedDictionaries.Add(rd1);
            }
            if (skin == "Metro")
            {
                rd1.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/MetroStyle.xaml", UriKind.RelativeOrAbsolute);
                rd.MergedDictionaries.Add(rd1);
            }
            if (skin == "Transparent")
            {
                rd1.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/TransparentStyle.xaml", UriKind.RelativeOrAbsolute);
                rd.MergedDictionaries.Add(rd1);
            }
            return rd;
        }

        #endregion

        #region NewSkinCode Helper methods

        private static void ApplyOptimization(DependencyObject obj,DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = obj as FrameworkElement;
            string appliedskin = e.NewValue as string;
            ResourceDictionary RootDict = null;
            if (!IsSkinNotChanged && element != null)
            {
                if ((element is Window || (element is UserControl && element.GetType().Name != "Splitter") || element is ChromelessWindow || element is Page))
                {
                    if (Root == null)
                        Root = new ObservableCollection<FrameworkElement>();
                    windowflag = true;
                    if (element is UserControl)
                    {
                        IsUserControlPresent = true;
                    }
                    if (element is Page)
                    {
                        IsPageControlPresent = true;
                    }
                    //ClearDictionary(element);
                    //RootDict = MergeMSControlDictionary(SkinStorage.GetVisualStyle(element));
                    //MergeDictionaryIntoElement(element, RootDict);
                   if (!Root.Contains(element))
                    {
                        element.Unloaded += new RoutedEventHandler(element_Unloaded);
                        Root.Add(element);
                        element.Loaded += new RoutedEventHandler(element_Loaded); 
                    }
                }

                if (element != null && IsApply(element))
                {
                    SkinTypeAttribute skinTypeAttribute = null;
                    appliedskin = SkinStorage.GetVisualStyle(obj).ToString();
                    skinTypeAttribute = GetSkinAttribute((element), appliedskin);

                    if (windowflag && skinTypeAttribute != null)
                    {
                        try
                        {
                            string skinDirectoryPath = skinTypeAttribute.XamlResource;
                            ResourceDictionary resourceDictionary = new ResourceDictionary();

                            resourceDictionary.Source = new Uri(skinDirectoryPath, UriKind.RelativeOrAbsolute);

                            FrameworkElement Parent = null;

                            if (Parent == null && IsUserControlPresent)
                            {
                                Parent = VisualUtils.FindSomeParent(element, typeof(UserControl));
                            }
                            if (Parent == null && IsPageControlPresent)
                            {
                                Parent = VisualUtils.FindSomeParent(element, typeof(Page));
                            }
                            if (element is Window || element is UserControl || element is Page)
                            {
                                Parent = element;
                            }
                            if (Parent == null)
                            {
                                Parent = VisualUtils.FindSomeParent(element, typeof(Window));
                            }

                            if (Parent != null)
                            {
                                if (Root.Contains(Parent))
                                {
                                    MergeDictionaryIntoElement(Parent, resourceDictionary);
                                }
                                else
                                {
                                    if(Root[0]!=null)
                                        MergeDictionaryIntoElement(Root[0], resourceDictionary);
                                }
                            }
                            else
                            {
                                if (Root[0] != null)
                                    MergeDictionaryIntoElement(Root[0], resourceDictionary);
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                    else if (skinTypeAttribute != null && skinTypeAttribute.XamlResource != string.Empty)
                    {
                        try
                        {
                            FrameworkElement control = obj as FrameworkElement;
                            ResourceDictionary resourceDictionary = new ResourceDictionary();
                            string skinDirectoryPath = skinTypeAttribute.XamlResource;

                            if (control.Resources.MergedDictionaries.Count == 0)
                                control.ClearMergedDictionaryPath();

                            resourceDictionary.Source = new Uri(skinDirectoryPath, UriKind.RelativeOrAbsolute);
                            RootDict = MergeMSControlDictionary(SkinStorage.GetVisualStyle(control));
                            MergeDictionaryIntoElement(control, RootDict);
                            MergeDictionaryIntoElement(control, resourceDictionary);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
        }

        static void element_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (sender as FrameworkElement);
            element.Loaded -= new RoutedEventHandler(element_Loaded); 
            ResourceDictionary RootDict = MergeMSControlDictionary(SkinStorage.GetVisualStyle(element));
            element.Resources.MergedDictionaries.Add(RootDict);
        }

        private static void ResetResources(DependencyObject obj, string appliedskin)
        {
            FrameworkElement element = obj as FrameworkElement;
            ResourceDictionary RootRd;
            try
            {
                if (GetVisualStyle(element).ToString() != appliedskin)
                {
                    IsThemeChangeNotNeeded = false;
                    IsSkinNotChanged = false;
                    ClearDictionary(element);
                    RootRd = MergeMSControlDictionary(appliedskin);
                    element.Resources.MergedDictionaries.Add(RootRd);
                    element.AddIntoMergedDictionaryPath(RootRd.Source == null ? "<Resouce Not found>" : RootRd.Source.ToString());
                }
            }
            catch (Exception)
            {
            }
        }

        private static ResourceDictionary MergeMSControlDictionary(string skin)
        {
            ResourceDictionary rd1 = new ResourceDictionary();
            switch (skin)
            {
                case "Blend":
                    rd1.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/BlendStyle.xaml", UriKind.RelativeOrAbsolute);
                    break;
                case "Office2007Blue":
                    rd1.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/Office2007BlueStyle.xaml", UriKind.RelativeOrAbsolute);
                    break;
                case "Office2007Black":
                    rd1.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/Office2007BlackStyle.xaml", UriKind.RelativeOrAbsolute);
                    break;
                case "Office2007Silver":
                    rd1.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/Office2007SilverStyle.xaml", UriKind.RelativeOrAbsolute);
                    break;
                case "Office2003":
                    rd1.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/Office2003Style.xaml", UriKind.RelativeOrAbsolute);
                    break;
                case "SyncOrange":
                    rd1.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/SyncOrangeStyle.xaml", UriKind.RelativeOrAbsolute);
                    break;
                case "ShinyRed":
                    rd1.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/ShinyRedStyle.xaml", UriKind.RelativeOrAbsolute);
                    break;
                case "ShinyBlue":
                    rd1.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/ShinyBlueStyle.xaml", UriKind.RelativeOrAbsolute);
                    break;
                case "Default":
                    rd1.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/DefaultStyle.xaml", UriKind.RelativeOrAbsolute);
                    break;
                case "Office2010Blue":
                    rd1.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/Office2010BlueStyle.xaml", UriKind.RelativeOrAbsolute);
                    break;
                case "Office2010Black":
                    rd1.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/Office2010BlackStyle.xaml", UriKind.RelativeOrAbsolute);
                    break;
                case "Office2010Silver":
                    rd1.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/Office2010SilverStyle.xaml", UriKind.RelativeOrAbsolute);
                    break;
                case "VS2010":
                    rd1.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/VS2010Style.xaml", UriKind.RelativeOrAbsolute);
                    break;
                case "Metro":
                    rd1.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/MetroStyle.xaml", UriKind.RelativeOrAbsolute);
                    break;
                case "Transparent":
                    rd1.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/TransparentStyle.xaml", UriKind.RelativeOrAbsolute);
                    break;
            }
            return rd1;
        }

        private static void MergeDictionaryIntoElement(FrameworkElement element, ResourceDictionary Dict)
        {
            if (Dict.Source != null && !element.IsContainsInMergedDictionaryPath(Dict.Source.ToString()))
            {
                element.Resources.MergedDictionaries.Add(Dict);
                element.AddIntoMergedDictionaryPath(Dict.Source.ToString());
            }
        }

        private static void ClearDictionary(FrameworkElement element)
        {
            if (element.Resources.MergedDictionaries.Count != 0 && element.GetMergedDictionaryPath().Count != 0)
            {
                Collection<ResourceDictionary> RdCollection = new Collection<ResourceDictionary>();
                foreach (ResourceDictionary dic in element.Resources.MergedDictionaries)
                {
                    RdCollection.Add(dic);
                }
                int count = RdCollection.Count;
                for (int i = 0; i <= count - 1; i++)
                {
                    ResourceDictionary rd = RdCollection[i];
                    if (rd.Source != null && element.GetMergedDictionaryPath().Contains(rd.Source.ToString()))
                    {
                        IsSkinNotChanged = true;
                        element.Resources.MergedDictionaries.Remove(rd);
                        IsSkinNotChanged = false;
                        element.GetMergedDictionaryPath().Remove(rd.Source.ToString());
                    }
                }
                RdCollection.Clear();
            }
        }

        static void element_Unloaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (sender as FrameworkElement);
            element.Unloaded -= element_Unloaded;
            element.ClearMergedDictionaryPath();
            Root.Remove(sender as FrameworkElement);
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
        void OnStyleChanged(string visualStyle);
    }
}
