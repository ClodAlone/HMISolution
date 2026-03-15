#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using Syncfusion.Windows.Controls.Grid;    
    using Syncfusion.Windows.Shared;
    using System.Windows.Interop;
    using System.IO;
    using System.Reflection;
    using System.Data;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System.Windows.Resources;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media.Effects;
    using System.Collections;
    using System.Collections.Specialized;
    using Syncfusion.Windows.GridCommon;
    using Syncfusion.Windows.Controls.Cells;

     public class LayoutControl : Control
    {
        static LayoutControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutControl), new FrameworkPropertyMetadata(typeof(LayoutControl)));
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LayoutControl()
        {
            this.UserOptionsView = new UserOptionViews();
            this.Initialized += new EventHandler(LayoutControl_Initialized);
        }

        void LayoutControl_Initialized(object sender, EventArgs e)
        {
            this.Initialized -= new EventHandler(LayoutControl_Initialized);

            bool alreadycontains = false;
            foreach (var rd in Application.Current.Resources.MergedDictionaries)
            {
                if (rd.Source.OriginalString.Equals(@"\Syncfusion.SampleLayout;component\Resources\MetroLayout.xaml"))
                    alreadycontains = true;
            }
            if (!alreadycontains)
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary(){Source =new Uri(@"\Syncfusion.SampleLayout;component\Resources\" +LayoutMode.ToString() + "Layout.xaml",UriKind.Relative)});
        }

        public LayoutMode LayoutMode
        {
            get { return (LayoutMode)GetValue(LayoutModeProperty); }
            set { SetValue(LayoutModeProperty, value); }
        }

        public static readonly DependencyProperty LayoutModeProperty =
            DependencyProperty.Register("LayoutMode", typeof(LayoutMode), typeof(LayoutControl), new UIPropertyMetadata(LayoutMode.Default));


        public static readonly DependencyProperty GridViewProperty = DependencyProperty.Register("GridView", typeof(object), typeof(LayoutControl));
        /// <summary>
        /// Displays the grid.
        /// </summary>
        public object GridView
        {
            get { return this.GetValue(LayoutControl.GridViewProperty); }
            set { this.SetValue(LayoutControl.GridViewProperty, value); }
        }

        public static readonly DependencyProperty UserOptionsViewProperty = DependencyProperty.Register("UserOptionsView", typeof(UserOptionViews), typeof(LayoutControl));


        public Brush LayoutBrush
        {
            get { return (Brush)this.GetValue(LayoutControl.LayoutBrushProperty); }
            set { this.SetValue(LayoutControl.LayoutBrushProperty, value); }
        }

        public static readonly DependencyProperty LayoutBrushProperty = DependencyProperty.Register("LayoutBrush", typeof(Brush), typeof(LayoutControl), new FrameworkPropertyMetadata(new SolidColorBrush(GridUtil.GetXamlConvertedValue<Color>("#FF119EDA")), null));
        /// <summary>
        /// Displays User Options panel.
        /// </summary>
        public UserOptionViews UserOptionsView
        {
            get { return (UserOptionViews)GetValue(LayoutControl.UserOptionsViewProperty); }
            set { this.SetValue(LayoutControl.UserOptionsViewProperty, value); }
        }

        public static readonly DependencyProperty UserOptionsVisibilityProperty = DependencyProperty.Register("UserOptionsVisibility", typeof(Visibility), typeof(LayoutControl), new FrameworkPropertyMetadata(Visibility.Visible, OnPanesVisibilityChanged));

        /// <summary>
        /// Gets or sets a value indicating whether to show or hide the User Options panel.
        /// </summary>
        public Visibility UserOptionsVisibility
        {
            get { return (Visibility)this.GetValue(LayoutControl.UserOptionsVisibilityProperty); }
            set { this.SetValue(LayoutControl.UserOptionsVisibilityProperty, value); }
        }

        public static readonly DependencyProperty ColSpanProperty = DependencyProperty.Register("ColSpan", typeof(int), typeof(LayoutControl));

        internal int ColSpan
        {
            get { return (int)this.GetValue(LayoutControl.ColSpanProperty); }
            set { this.SetValue(LayoutControl.ColSpanProperty, value); }
        }

        public static readonly DependencyProperty OptionsRowProperty = DependencyProperty.Register("OptionsRow", typeof(int), typeof(LayoutControl));

        internal int OptionsRow
        {
            get { return (int)this.GetValue(LayoutControl.OptionsRowProperty); }
            set { this.SetValue(LayoutControl.OptionsRowProperty, value); }
        }

        public static readonly DependencyProperty OptionsRowSpanProperty = DependencyProperty.Register("OptionsRowSpan", typeof(int), typeof(LayoutControl));

        internal int OptionsRowSpan
        {
            get { return (int)this.GetValue(LayoutControl.OptionsRowSpanProperty); }
            set { this.SetValue(LayoutControl.OptionsRowSpanProperty, value); }
        }

        public static readonly DependencyProperty SkinPickerVisibilityProperty = DependencyProperty.Register("SkinPickerVisibility", typeof(Visibility), typeof(LayoutControl), new FrameworkPropertyMetadata(Visibility.Collapsed, OnPanesVisibilityChanged));
        /// <summary>
        /// Gets or sets a value indicating whether to show or hide the skin picker.
        /// </summary>
        public Visibility SkinPickerVisibility
        {
            get { return (Visibility)this.GetValue(LayoutControl.SkinPickerVisibilityProperty); }
            set { this.SetValue(LayoutControl.SkinPickerVisibilityProperty, value); }
        }

        public static readonly DependencyProperty VisualStyleProperty = 
            DependencyProperty.Register("VisualStyle", typeof(VisualStyle), typeof(LayoutControl), new FrameworkPropertyMetadata(VisualStyle.Metro, OnVisualStyleChanged));
        /// <summary>
        /// Gets or sets the grid skin.
        /// </summary>
        public VisualStyle VisualStyle
        {
            get { return (VisualStyle)this.GetValue(LayoutControl.VisualStyleProperty); }
            set { this.SetValue(LayoutControl.VisualStyleProperty, value); }
        }

        #region Header Panel Dependency Properties

        public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register("HeaderText", typeof(string), typeof(LayoutControl), new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the grid skin.
        /// </summary>
        public string HeaderText
        {
            get { return (string)this.GetValue(LayoutControl.HeaderTextProperty); }
            set { this.SetValue(LayoutControl.HeaderTextProperty, value); }
        }

        public static readonly DependencyProperty GridViewBackgroundProperty = DependencyProperty.Register(
          "GridViewBackground", typeof(Brush), typeof(LayoutControl), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the grid skin.
        /// </summary>
        public Brush GridViewBackground
        {
            get { return (Brush)this.GetValue(LayoutControl.GridViewBackgroundProperty); }
            set { this.SetValue(LayoutControl.GridViewBackgroundProperty, value); }
        }

        public static readonly DependencyProperty HeaderPanelBackgroundProperty = 
            DependencyProperty.Register("HeaderPanelBackground", typeof(Brush), typeof(LayoutControl), new FrameworkPropertyMetadata(null));
        /// <summary>
        /// Gets or sets the grid skin.
        /// </summary>
        public Brush HeaderPanelBackground
        {
            get { return (Brush)this.GetValue(LayoutControl.HeaderPanelBackgroundProperty); }
            set { this.SetValue(LayoutControl.HeaderPanelBackgroundProperty, value); }
        }

        public static readonly DependencyProperty GridViewHeaderTextProperty = 
            DependencyProperty.Register("GridViewHeaderText", typeof(string), typeof(LayoutControl), new FrameworkPropertyMetadata("GridDataControl"));
        /// <summary>
        /// Gets or sets the grid skin.
        /// </summary>
        public string GridViewHeaderText
        {
            get { return (string)this.GetValue(LayoutControl.GridViewHeaderTextProperty); }
            set { this.SetValue(LayoutControl.GridViewHeaderTextProperty, value); }
        }

        public static readonly DependencyProperty HeaderDescriptionTextProperty = 
            DependencyProperty.Register("HeaderDescriptionText", typeof(string), typeof(LayoutControl), new FrameworkPropertyMetadata(string.Empty));
        /// <summary>
        /// Gets or sets the grid skin.
        /// </summary>
        public string HeaderDescriptionText
        {
            get { return (string)this.GetValue(LayoutControl.HeaderDescriptionTextProperty); }
            set { this.SetValue(LayoutControl.HeaderDescriptionTextProperty, value); }
        }

        public static readonly DependencyProperty HeaderPanelVisibilityProperty = 
            DependencyProperty.Register("HeaderPanelVisibility", typeof(Visibility), typeof(LayoutControl), new FrameworkPropertyMetadata(Visibility.Visible, OnHeaderPanelVisibilityChanged));
        /// <summary>
        /// Gets or sets the grid skin.
        /// </summary>
        public Visibility HeaderPanelVisibility
        {
            get { return (Visibility)this.GetValue(LayoutControl.HeaderPanelVisibilityProperty); }
            set { this.SetValue(LayoutControl.HeaderPanelVisibilityProperty, value); }
        }

        private static void OnHeaderPanelVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            LayoutControl layout = d as LayoutControl;
            if (layout.HeaderPanelVisibility == Visibility.Collapsed || layout.HeaderPanelVisibility == Visibility.Hidden)
            {
                layout.HeaderRowCount = 0;
                layout.HeaderRowSpan = 2;
            }
            else
            {
                layout.HeaderRowCount = 1;
                layout.HeaderRowSpan = 1;
            }
        }

        public static readonly DependencyProperty HeaderRowCountProperty =
            DependencyProperty.Register("HeaderRowCount", typeof(int), typeof(LayoutControl), new FrameworkPropertyMetadata(1));

        internal int HeaderRowCount
        {
            get { return (int)this.GetValue(LayoutControl.HeaderRowCountProperty); }
            set { this.SetValue(LayoutControl.HeaderRowCountProperty, value); }
        }

        public static readonly DependencyProperty HeaderRowSpanProperty =
            DependencyProperty.Register("HeaderRowSpan", typeof(int), typeof(LayoutControl), new FrameworkPropertyMetadata(1));

        internal int HeaderRowSpan
        {
            get { return (int)this.GetValue(LayoutControl.HeaderRowSpanProperty); }
            set { this.SetValue(LayoutControl.HeaderRowSpanProperty, value); }
        }

        #endregion

        #region User Guide Navigator & Metor Mode

        bool HasUserGuideUri = false;
        private Button UserGuideNavigator;

        /// <summary>
        /// Gets or sets the user guide URI.
        /// </summary>
        /// <value>The user guide URI.</value>
        public string UserGuideUri
        {
            get { return (string)GetValue(UserGuideUriProperty); }
            set { SetValue(UserGuideUriProperty, value); }
        }

        // Dependency Property Registartion for UserGuidUri
        public static readonly DependencyProperty UserGuideUriProperty =
            DependencyProperty.Register("UserGuideUri", typeof(string), typeof(LayoutControl), new PropertyMetadata(string.Empty, OnUserGuideUri));

        private static void OnUserGuideUri(DependencyObject sender, DependencyPropertyChangedEventArgs arg)
        {
            LayoutControl control = sender as LayoutControl;

            if (control.IsLoaded)
                control.UserGuideNavigator.Visibility = Visibility.Collapsed;
            else
                control.HasUserGuideUri = true;
        }

        // Dependency Property Registartion for SetMetroMode
        public static readonly DependencyProperty SetMetroModeProperty =
            DependencyProperty.RegisterAttached("SetMetroMode", typeof(bool), typeof(LayoutControl), new PropertyMetadata(false, OnSetMetroModeChanged));

        #endregion

        private ComboBox SkinChooser;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            SkinChooser = this.GetTemplateChild("SkinChooser") as ComboBox;
            UserGuideNavigator = this.GetTemplateChild("PART_UserGuide") as Button;

            this.Unloaded += OnLayoutControlUnloaded;

            if (UserGuideNavigator != null)
            {
                UserGuideNavigator.Click += OnUserGuideNavigatorClick;
            }

            if (HasUserGuideUri && UserGuideNavigator != null)
                UserGuideNavigator.Visibility = Visibility.Collapsed;

            if (SkinChooser != null)
            {
                IEnumerable coll = LayoutHelper.GetValues(typeof(VisualStyle));

                List<VisualStyle> visualstylecollection = new List<VisualStyle>();
                foreach (var item in coll)
                {
                    if ((VisualStyle)item != VisualStyle.Custom)
                        visualstylecollection.Add((VisualStyle)item);
                }
                SkinChooser.ItemsSource = coll;
                SkinChooser.SelectedItem = this.VisualStyle;
            }
        }

        private static void OnVisualStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            LayoutControl layout = d as LayoutControl;
            //IGridDataVisualStyle visualStyle;
            var value = ((VisualStyle)args.NewValue).ToString();
            switch (value)
            {
                case "Office14Blue":
                    //visualStyle = new GridDataBlueVisualStyle();
                    SkinStorage.SetVisualStyle(layout, "Office2010Blue");
                    break;
                case "Office14Black":
                    //visualStyle = new GridDataBlueVisualStyle();
                    SkinStorage.SetVisualStyle(layout, "Office2010Black");
                    break;
                case "Office14Silver":
                    //visualStyle = new GridDataBlueVisualStyle();
                    SkinStorage.SetVisualStyle(layout, "Office2010Silver");
                    break;
                case "Office2007Blue":
                    //visualStyle = new GridDataBlueVisualStyle();
                    SkinStorage.SetVisualStyle(layout, "Office2007Blue");
                    break;
                case "Office2007Black":
                    //visualStyle = new GridDataBlackVisualStyle();
                    SkinStorage.SetVisualStyle(layout, "Office2007Black");
                    break;
                case "Office2007Silver":
                    //visualStyle = new GridDataSilverVisualStyle();
                    SkinStorage.SetVisualStyle(layout, "Office2007Silver");
                    break;
                case "Blend":
                    //visualStyle = new GridDataBlendVisualStyle();
                    SkinStorage.SetVisualStyle(layout, "Blend");
                    break;
                case "Office2003":
                    //visualStyle = new GridDataSyncBlueVisualStyle();
                    SkinStorage.SetVisualStyle(layout, "Office2003");
                    break;
                case "DefaultOffice2007Blue":
                    //visualStyle = new GridDataOffice2007BlueVisualStyle();
                    SkinStorage.SetVisualStyle(layout, "Office2007Blue");
                    break;
                case "DefaultOffice2007Black":
                    //visualStyle = new GridDataOffice2007BlackVisualStyle();
                    SkinStorage.SetVisualStyle(layout, "Office2007Black");
                    break;
                case "DefaultOffice2007Silver":
                    //visualStyle = new GridDataOffice2007SilverVisualStyle();
                    SkinStorage.SetVisualStyle(layout, "Office2007Silver");
                    break;
                case "GlassyGreen":
                    //visualStyle = new GridDataGlassyGreenVisualStyle();
                    SkinStorage.SetVisualStyle(layout, "Metro");
                    break;
                case "SunBlack":
                    //visualStyle = new GridDataSunBlackVisualStyle();
                    SkinStorage.SetVisualStyle(layout, "Metro");
                    break;
                case "ShinyRed":
                    //visualStyle = new GridDataShinyRedVisualStyle();
                    SkinStorage.SetVisualStyle(layout, "Metro");
                    break;
                case "ShinyBlue":
                    //visualStyle = new GridDataShinyBlueVisualStyle();
                    SkinStorage.SetVisualStyle(layout, "Default");
                    break;
                case "BureauBlue":
                    //visualStyle = new GridDataBureauBlueVisualStyle();
                    SkinStorage.SetVisualStyle(layout, "Metro");
                    break;
                case "BureauBlack":
                    //visualStyle = new GridDataBureauBlackVisualStyle();
                    SkinStorage.SetVisualStyle(layout, "Metro");
                    break;
                case "TwilightBlue":
                    //visualStyle = new GridDataTwilightBlueVisualStyle();
                    SkinStorage.SetVisualStyle(layout, "Metro");
                    break;
                case "Metro":
                    //visualStyle = new GridDataMetroVisualStyle();
                    SkinStorage.SetVisualStyle(layout, "Metro");
                    break;
                default:
                    //visualStyle = new GridDataDefaultGridVisualStyle();
                    SkinStorage.SetVisualStyle(layout, "Default");
                    break;
            }

            if (layout.GridView is GridDataControl)
            {
                var grid = layout.GridView as GridDataControl;
                grid.VisualStyle = layout.VisualStyle;
            }
            else if (layout.GridView is GridTreeControl)
            {
                var grid = layout.GridView as GridTreeControl;
                grid.VisualStyle = layout.VisualStyle;
            }
        }

        private static void OnPanesVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            LayoutControl layout = d as LayoutControl;
            if (layout.UserOptionsVisibility == Visibility.Collapsed || layout.UserOptionsVisibility == Visibility.Hidden)
            {
                layout.UserOptionsVisibility = Visibility.Collapsed;
                layout.ColSpan = 2;
            }
            else
            {
                layout.UserOptionsVisibility = Visibility.Visible;
                layout.ColSpan = 1;
            }
        }

        /// <summary>
        /// Retruns true, if the application is in design mode; false otherwise.
        /// </summary>
        public static bool IsInDesignMode
        {
            get
            {
                return DesignerProperties.GetIsInDesignMode(new DependencyObject());
            }
        }

        /// <summary>
        /// Helper method to find a given database file.
        /// </summary>
        /// <param name="fileName">Database file name.</param>
        /// <returns>Path of the database file.</returns>
        public static string FindFile(string fileName)
        {
            int levelsToCheck = 12;
            string dataFolder = @"Common\Data";
            
            string rootPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().CodeBase.Replace(@"file:///", ""));

            fileName = System.IO.Path.GetFileName(fileName);

            for (int n = 0; n < levelsToCheck; n++)
            {
                string filePath = System.IO.Path.Combine(rootPath, fileName);
                string fileDataPath = System.IO.Path.Combine(rootPath, dataFolder);

                fileDataPath = System.IO.Path.Combine(fileDataPath, fileName);

                if (System.IO.File.Exists(filePath))
                    return new FileInfo(filePath).FullName;

                if (System.IO.File.Exists(fileDataPath))
                    return new FileInfo(fileDataPath).FullName;
                
                rootPath = Directory.GetParent(rootPath).FullName;
            }

            return fileName;
        }

        #region User Guide Navigator

        /// <summary>
        /// Called when [layout control unloaded].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void OnLayoutControlUnloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= OnLayoutControlUnloaded;
            if (this.UserGuideNavigator != null)
            {
                this.UserGuideNavigator.Click -= OnUserGuideNavigatorClick;
            }
        }

        /// <summary>
        /// Called when [user guide navigator click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void OnUserGuideNavigatorClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.UserGuideUri))
            {
                System.Diagnostics.Process.Start(this.UserGuideUri.ToString());
            }
        }

        #endregion

        #region Metro Mode

        /// <summary>
        /// Gets the set metro mode.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static bool GetSetMetroMode(DependencyObject obj)
        {
            return (bool)obj.GetValue(SetMetroModeProperty);
        }

        /// <summary>
        /// Sets the set metro mode.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetSetMetroMode(DependencyObject obj, bool value)
        {
            obj.SetValue(SetMetroModeProperty, value);
        }

        /// <summary>
        /// Called when [set metro mode changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSetMetroModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            if (sender is ChromelessWindow)
            {
                ChromelessWindow window = sender as ChromelessWindow;
                window.AllowsTransparency = false;
                window.Height = 720;
                window.Width = 1200;
                window.Title = String.Empty;
                window.ShowIcon = false;
                window.ResizeBorderBrush = Brushes.LightGray; 
                window.ResizeBorderThickness = new Thickness(0.5);
                window.ResizeMode = ResizeMode.CanResizeWithGrip;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.UseNativeChrome = true;
                SkinStorage.SetVisualStyle(window, "Metro");
            }
            else if (sender is Window)
            {
                Window window = sender as Window;
                window.AllowsTransparency = false;
                window.Height = 720;
                window.Width = 1200;
                window.Title = String.Empty;
                window.ResizeMode = ResizeMode.CanResizeWithGrip;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                SkinStorage.SetVisualStyle(window, "Metro");
            }
            else if (sender is GridControlBase)
            {
                GridControlBase grid = sender as GridControlBase;
                grid.Model.TableStyle.Font = new GridFontInfo()
                {
                    FontFamily = new FontFamily("Segoe WP"),
                    FontSize = 14,
                };
                grid.Model.TableStyle.VerticalAlignment = VerticalAlignment.Center;
                grid.Model.HeaderStyle.HorizontalAlignment = HorizontalAlignment.Center;
                grid.Model.TableStyle.Borders.All = new Pen()
                {
                    Brush = Brushes.DarkGray,
                    Thickness = 0.18
                };
                grid.Model.RowHeights.DefaultLineSize = 25;
                if (grid.Model.HeaderRows > 0)
                    grid.Model.ColumnWidths[0] = 50;
            }
            else if (sender is GridTreeControl)
            {
                GridTreeControl grid = sender as GridTreeControl;
                grid.Model.TableStyle.VerticalAlignment = VerticalAlignment.Center;
            }
        }
        #endregion

        #region Obsolete Properties

        private bool enableThemedGrid = false;
        [Obsolete("This property is obsolete")]
        public bool EnableThemedGrid
        {
            get
            {
                return enableThemedGrid;
            }
            set
            {
                enableThemedGrid = value;
            }
        }

        public static readonly DependencyProperty RightPaneVisibilityProperty = DependencyProperty.Register("RightPaneVisibility", typeof(Visibility), typeof(LayoutControl));

        [Obsolete("This property is obsolete")]
        public Visibility RightPaneVisibility
        {
            get { return (Visibility)this.GetValue(LayoutControl.RightPaneVisibilityProperty); }
            set { this.SetValue(LayoutControl.RightPaneVisibilityProperty, value); }
        }

        private static void OnTraceVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
        }

        public static readonly DependencyProperty TraceVisibilityProperty = DependencyProperty.Register("TraceVisibility", typeof(Visibility), typeof(LayoutControl), new FrameworkPropertyMetadata(Visibility.Collapsed, OnTraceVisibilityChanged));

        [Obsolete("This property is obsolete")]
        public Visibility TraceVisibility
        {
            get { return (Visibility)this.GetValue(LayoutControl.TraceVisibilityProperty); }
            set { this.SetValue(LayoutControl.TraceVisibilityProperty, value); }
        }

        private static readonly DependencyProperty IgnoreStyleProperty = DependencyProperty.RegisterAttached("IgnoreStyle", typeof(bool), typeof(LayoutControl), new PropertyMetadata(false));

        [Obsolete("This property is obsolete")]
        public static bool GetIgnoreStyle(DependencyObject dpo)
        {
            return (bool)dpo.GetValue(LayoutControl.IgnoreStyleProperty);
        }

        [Obsolete("This property is obsolete")]
        public static void SetIgnoreStyle(DependencyObject dpo, bool value)
        {
            dpo.SetValue(LayoutControl.IgnoreStyleProperty, value);
        }

        private static readonly DependencyProperty ApplyStyleProperty = DependencyProperty.RegisterAttached("ApplyStyle", typeof(bool), typeof(LayoutControl), new PropertyMetadata(OnApplyStyleChanged));

        private static void OnApplyStyleChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {

        }

        private static void SetStyle(DependencyObject dpo, FrameworkElement parent)
        {
            if (LayoutControl.IsInDesignMode)
                return;
        }

        [Obsolete("This property is obsolete")]
        public static bool GetApplyStyle(DependencyObject dpo)
        {
            return (bool)dpo.GetValue(LayoutControl.ApplyStyleProperty);
        }

        [Obsolete("This property is obsolete")]
        public static void SetApplyStyle(DependencyObject dpo, bool value)
        {
            if (LayoutControl.IsInDesignMode)
            {
                return;
            }
            dpo.SetValue(LayoutControl.ApplyStyleProperty, value);
        }
        #endregion
    }

     public enum LayoutMode
     {
         Default,
         Metro
     }

    internal static class LayoutHelper
    {

        /// <summary>  
        /// Get values of an enumeration of type T  
        /// </summary>  
        /// <typeparam name="T">Enumeration type</typeparam>  
        /// <returns>List of types in a enumeration</returns>  
        public static T[] GetValues<T>()
        {
            Type enumType = typeof(T);

            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type '" + enumType.Name + "' is not an enum");
            }

            List<T> values = new List<T>();

            var fields = from field in enumType.GetFields()
                         where field.IsLiteral
                         select field;

            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue(enumType);
                values.Add((T)value);
            }

            return values.ToArray();
        }

        /// <summary>  
        /// Get all the values of an enumeration of Type enumType  
        /// </summary>  
        /// <param name="enumType">Enumeration type</param>  
        /// <returns>Array of objects</returns>  
        public static object[] GetValues(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type '" + enumType.Name + "' is not an enum");
            }

            List<object> values = new List<object>();

            //var fields = from field in enumType.GetFields()
            //             where field.IsLiteral
            //             select field;

            //foreach (FieldInfo field in fields)
            //{
            //object value = field.GetValue(enumType);
            values.Add(VisualStyle.Blend);
            values.Add(VisualStyle.BureauBlue);
            values.Add(VisualStyle.Default);
            values.Add(VisualStyle.GlassyGreen);
            values.Add(VisualStyle.Metro);
            values.Add(VisualStyle.Office14Black);
            values.Add(VisualStyle.Office14Blue);
            values.Add(VisualStyle.Office14Silver);
            values.Add(VisualStyle.Office2007Black);
            values.Add(VisualStyle.Office2007Blue);
            values.Add(VisualStyle.Office2007Silver);
            values.Add(VisualStyle.ShinyBlue);
            values.Add(VisualStyle.ShinyRed);
            values.Add(VisualStyle.SunBlack);
            values.Add(VisualStyle.SyncfusionTheme);
            values.Add(VisualStyle.TwilightBlue);
            values.Add(VisualStyle.VS2010);
            values.Add(VisualStyle.Windows7);
            //}

            return values.ToArray();
        }

        public static IEnumerable<T> FindElementsOfType<T>(this FrameworkElement element) where T : class
        {
            T correctlyTyped = element as T;
            if (correctlyTyped != null)
            {
                yield return correctlyTyped;
            }

            if (element != null)
            {
                int numChildren = VisualTreeHelper.GetChildrenCount(element);
                for (int i = 0; i < numChildren; i++)
                {
                    var children = FindElementsOfType<T>(VisualTreeHelper.GetChild(element, i) as FrameworkElement);
                    foreach (var child in children)
                    {
                        yield return child;
                    }
                }

                // Popups continue in another window, jump to that tree
                Popup popup = element as Popup;
                if (popup != null)
                {
                    var popupChildren = FindElementsOfType<T>(popup.Child as FrameworkElement);
                    foreach (var child in popupChildren)
                    {
                        yield return child;
                    }
                }
            }

            yield return null;
        }

        public static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            // get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            // we’ve reached the end of the tree
            if (parentObject == null) return null;

            // check if the parent matches the type we’re looking for
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                // use recursion to proceed with next level
                return FindVisualParent<T>(parentObject);
            }
        }
    }

    public class UserOptionViews : FreezableCollection<UserOptions>
    {
        public UserOptionViews()
        {
            //((INotifyCollectionChanged)this).CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);
        }
    }

    public class UserOptions : ContentControl
    {
        public UserOptions()
        {
            this.DefaultStyleKey = typeof(UserOptions);
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(UserOptions), new FrameworkPropertyMetadata(typeof(UserOptions)));
        }

        public static readonly DependencyProperty HeaderBackgroundProperty = DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(UserOptions), new FrameworkPropertyMetadata(null));
        /// <summary>
        /// Displays the grid.
        /// </summary>
        public Brush HeaderBackground
        {
            get { return (Brush)GetValue(UserOptions.HeaderBackgroundProperty); }
            set { this.SetValue(UserOptions.HeaderBackgroundProperty, value); }
        }

        public static readonly DependencyProperty HeaderTextProperty = DependencyProperty.Register("HeaderText", typeof(string), typeof(UserOptions));
        /// <summary>
        /// Displays the grid.
        /// </summary>
        public string HeaderText
        {
            get { return (string)GetValue(UserOptions.HeaderTextProperty); }
            set { this.SetValue(UserOptions.HeaderTextProperty, value); }
        }
    }
}