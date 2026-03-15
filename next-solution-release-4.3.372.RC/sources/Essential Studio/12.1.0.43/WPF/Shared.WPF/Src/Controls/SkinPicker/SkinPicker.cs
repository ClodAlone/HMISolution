// <copyright file="SkinPicker.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Syncfusion.Licensing;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents the SkinPicker control.
    /// SkinPicker is used to pick the available skins from SkinManager. And also it well it will apply skin for root element <see cref="Window"> automatically</see>/&gt; without writing single line of code.
    /// </summary>
    /// <example>
    /// <para/>This example shows how to use the SkinPicker control in XAML.
    /// <code><![CDATA[<Window x:Class="MagnifierDemo.SampleWindow"
    /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:shared="clr-namespace:Syncfusion.Windows.Shared;assembly=Syncfusion.Shared.WPF"/>
    /// <Grid Name="grid">
    /// <syncfusion:SkinPicker TargetElement="window1"/>
    /// </Grid>
    /// </Window> ]]></code>
    /// <para/>
    /// <para/>This example shows how to use the SkinPicker control in C#.
    /// <code>
    /// using System;
    /// using System.Windows;
    /// using Syncfusion.Windows.Shared;
    /// namespace SkinPickerDemo
    /// {
    /// public partial class SampleWindow : Window
    /// {
    /// <para/>
    /// private SkinPicker skinPicker = new SkinPicker();
    /// <para/>
    /// public SampleWindow()
    /// {
    /// <para/>
    /// skinPicker.TargetElement = "window1";
    /// this.Content=skinPicker;
    /// }
    /// }
    /// }
    /// </code>
    /// </example>
    /// <remarks>
    /// TargetElement property is optional. We can change the target element if we want to apply skin for specific container in application.
    /// </remarks>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class SkinPicker : ItemsControl
    {
        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="SkinPicker"/> class.
        /// </summary>
        static SkinPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SkinPicker), new FrameworkPropertyMetadata(typeof(SkinPicker)));
            EnvironmentTest.ValidateLicense(typeof(SkinPicker));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkinPicker"/> class.
        /// </summary>
        public SkinPicker()
        {
            this.Loaded += new RoutedEventHandler(SkinPicker_Loaded);
            this.SelectedItemChanged += new PropertyChangedCallback(SkinPicker_SelectedItemChanged);
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(SkinPicker));
            }
        }

        #endregion Initialization

        #region Event

        /// <summary>
        /// Event that is raised when <see cref="SelectedItem"> property is changed.</see>/&gt;
        /// </summary>
        public event PropertyChangedCallback SelectedItemChanged;

        /// <summary>
        /// Event that is raised when <see cref="Skin"> property is changed.</see>/&gt;
        /// </summary>
        public event PropertyChangedCallback SkinChanged;

        /// <summary>
        /// Event that raise when selected item changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SkinPicker instance = sender as SkinPicker;
            instance.OnSelectedItemChanged(e);
        }

        /// <summary>
        /// Event that raise when Skin changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSkinChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SkinPicker instance = sender as SkinPicker;
            instance.OnSkinChanged(e);
        }

        /// <summary>
        /// Event that raise when VS2010 Visibility changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnVS2010VisibilityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            //SkinPicker instance = sender as SkinPicker;
            //instance.OnSkinChanged(e);
        }

        /// <summary>
        /// virtual method for raising SelecteItemChanged event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSelectedItemChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectedItemChanged != null)
            {
                SelectedItemChanged(this, e);
            }
        }

        /// <summary>
        /// virtual method for raising SkinChanged event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSkinChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SkinChanged != null)
            {
                SkinChanged(this, e);
            }
        }

        #endregion Event

        #region implementation

        /////// <summary>
        /////// skin changed event
        /////// </summary>
        /////// <param name="d">The d object.</param>
        /////// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        //private void SkinPicker_SkinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    foreach (StackPanel item in this.Items)
        //    {
        //        SkinPickerItem sitem = item.Children[0] as SkinPickerItem;
        //        if (sitem.Tag.Equals(this.Skin.ToString()))
        //        {
        //            sitem.IsSelected = true;
        //            SelectedItem = sitem;
        //        }
        //    }
        //}

        /// <summary>
        /// selected item changed event.
        /// </summary>
        /// <param name="d">The d object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void SkinPicker_SelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement ww = GetRootElement(this.Parent as FrameworkElement);
            if (ww != null)
            {
                SkinStorage.SetVisualStyle(ww, this.SelectedItem.Tag.ToString());
                this.Skin = (Skins)Enum.Parse(typeof(Skins), this.SelectedItem.Tag.ToString());
            }
        }

        /// <summary>
        /// Loaded event getting skins list.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void SkinPicker_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LoadSkinByDefault == true)
                {
                    ResourceDictionary rd = this.Template.Resources.MergedDictionaries[0];
                    List<FrameworkElement> itemsList = new List<FrameworkElement>();
                    int keys = rd.Keys.Count;
                    if (!EnableOffice2010Skins)
                    {
                        keys = rd.Keys.Count - 3;
                    }
                    for (int i = 0; i < keys; i++)
                    {
                        if (EnableOffice2010Skins)
                        {
                            SkinPickerItem item = new SkinPickerItem();
                            object obj1 = rd["DefaultSkinImage"];
                            object obj = "";
                            if (i == 0)
                            {
                                obj1 = rd["DefaultSkinImage"];
                                obj = "Default SkinSkinImage";
                                item.Tag = "Default";
                            }
                            else if (i == 1)
                            {
                                obj1 = rd["BlendSkinImage"];
                                obj = "Expression BlendSkinImage";
                                item.Tag = "Blend";
                            }
                            else if (i == 2)
                            {
                                obj1 = rd["VS2010SkinImage"];
                                obj = "VS2010     SkinSkinImage";
                                if (IsVS2010Visible)
                                    item.Tag = "VS2010";
                                else
                                    continue;
                            }
                            else if (i == 3)
                            {
                                obj1 = rd["BlueSkinImage"];
                                obj = "2007     BlueSkinImage";
                                item.Tag = "Office2007Blue";
                            }
                            else if (i == 4)
                            {
                                obj1 = rd["BlackSkinImage"];
                                obj = "2007     BlackSkinImage";
                                item.Tag = "Office2007Black";
                            }
                            else if (i == 5)
                            {
                                obj1 = rd["SilverSkinImage"];
                                obj = "2007     SilverSkinImage";
                                item.Tag = "Office2007Silver";
                            }

                            if (i == 6)
                            {
                                obj1 = rd["2010 BlueSkinImage"];
                                obj = "2010     BlueSkinImage";
                                item.Tag = "Office2010Blue";
                            }
                            if (i == 7)
                            {
                                obj1 = rd["2010 BlackSkinImage"];
                                obj = "2010     BlackSkinImage";
                                item.Tag = "Office2010Black";
                            }
                            else if (i == 8)
                            {
                                obj1 = rd["2010 SilverSkinImage"];
                                obj = "2010      SilverSkinImage";
                                item.Tag = "Office2010Silver";
                            }
                            else if (i == 9)
                            {
                                obj1 = rd["2003SkinImage"];
                                obj = "Office    2003SkinImage";
                                item.Tag = "Office2003";
                            }

                            StackPanel panel = new StackPanel();
                            panel.Tag = item.Tag;
                            Image img = new Image();
                            item.Width = 26;
                            item.Height = 26;
                            img.Source = obj1 as DrawingImage;
                            item.Content = img;
                            item.MouseLeftButtonDown += new MouseButtonEventHandler(item_MouseLeftButtonDown);

                            panel.Children.Add(item);

                            TextBlock text = new TextBlock();

                            text.Width = 60;
                            text.TextWrapping = TextWrapping.Wrap;
                            text.Margin = new Thickness(0, 1, 0, 2);
                            text.Text = obj.ToString().Replace("SkinImage", string.Empty);
                            text.TextAlignment = TextAlignment.Center;
                            text.HorizontalAlignment = HorizontalAlignment.Center;

                            panel.Children.Add(text);
                            this.Items.Add(panel);
                        }
                        else
                        {
                            SkinPickerItem item = new SkinPickerItem();
                            object obj1 = rd["DefaultSkinImage"];
                            object obj = "";
                            if (i == 0)
                            {
                                obj1 = rd["DefaultSkinImage"];
                                obj = "DefaultSkinImage";
                                item.Tag = "Default";
                            }
                            else if (i == 1)
                            {
                                obj1 = rd["BlendSkinImage"];
                                obj = "BlendSkinImage";
                                item.Tag = "Blend";
                            }
                            else if (i == 2)
                            {
                                obj1 = rd["VS2010SkinImage"];
                                obj = "VS2010SkinImage";
                                if (IsVS2010Visible)
                                    item.Tag = "VS2010";
                                else
                                    continue;
                            }
                            else if (i == 3)
                            {
                                obj1 = rd["BlueSkinImage"];
                                obj = "BlueSkinImage";
                                item.Tag = "Office2007Blue";
                            }
                            else if (i == 4)
                            {
                                obj1 = rd["BlackSkinImage"];
                                obj = "BlackSkinImage";
                                item.Tag = "Office2007Black";
                            }
                            else if (i == 5)
                            {
                                obj1 = rd["SilverSkinImage"];
                                obj = "SilverSkinImage";
                                item.Tag = "Office2007Silver";
                            }
                            else if (i == 6)
                            {
                                obj1 = rd["2003SkinImage"];
                                obj = "2003SkinImage";
                                item.Tag = "Office2003";
                            }

                            StackPanel panel = new StackPanel();
                            panel.Tag = item.Tag;
                            Image img = new Image();
                            item.Width = 28;
                            item.Height = 28;
                            img.Source = rd[obj] as DrawingImage;
                            item.Content = img;
                            item.MouseLeftButtonDown += new MouseButtonEventHandler(item_MouseLeftButtonDown);

                            panel.Children.Add(item);

                            TextBlock text = new TextBlock();
                            text.Margin = new Thickness(4, 1, 4, 2);
                            text.Text = obj.ToString().Replace("SkinImage", string.Empty);
                            panel.Children.Add(text);
                            this.Items.Add(panel);
                        }
                    }

                    this.Loaded -= new RoutedEventHandler(SkinPicker_Loaded);
                }
            }
            catch (Exception)
            { }
        }

        private void item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SkinPickerItem item = sender as SkinPickerItem;
            if (item != null)
            {
                if (SelectedItem != null)
                {
                    SelectedItem.IsSelected = false;
                }

                item.IsSelected = true;
                this.SelectedItem = item;
            }
        }

        /// <summary>
        /// Getting root element(Window) if <see cref="GetRootElement"></see> property is null/&gt;
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>Return the framework element</returns>
        private FrameworkElement GetRootElement(FrameworkElement element)
        {
            while (element != null && !((element is Window) || (element is ChromelessWindow)))
            {
                if (element.Name.Length > 0)
                {
                    if (element.Name.Equals(TargetElement))
                    {
                        return element as FrameworkElement;
                    }
                }

                element = VisualTreeHelper.GetParent((DependencyObject)element) as FrameworkElement;
            }

            return element as FrameworkElement;
        }

        #endregion implementation

        #region Dp's

        /// <summary>
        ///  Identifies the <see cref="LoadSkinByDefault"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LoadSkinByDefaultProperty = DependencyProperty.Register("LoadSkinByDefault", typeof(bool), typeof(SkinPicker), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="SelectedItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(SkinPickerItem), typeof(SkinPicker), new PropertyMetadata(null, OnSelectedItemChanged));

        /// <summary>
        /// Identifies the <see cref="Skin"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SkinProperty = DependencyProperty.Register("Skin", typeof(Skins), typeof(SkinPicker), new PropertyMetadata(Skins.Office2007Blue, OnSkinChanged));

        /// <summary>
        /// Identifies the <see cref="IsVS2010Visible"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsVS2010VisibleProperty = DependencyProperty.Register("IsVS2010Visible", typeof(bool), typeof(SkinPicker), new PropertyMetadata(true, OnVS2010VisibilityChanged));

        /// <summary>
        ///
        /// </summary>
        public bool EnableOffice2010Skins
        {
            get { return (bool)GetValue(EnableOffice2010SkinsProperty); }
            set { SetValue(EnableOffice2010SkinsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableOffice2010Skins.  This enables animation, styling, binding, etc...
        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty EnableOffice2010SkinsProperty =
            DependencyProperty.Register("EnableOffice2010Skins", typeof(bool), typeof(SkinPicker), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="TargetElement"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TargetElementProperty = DependencyProperty.Register("TargetElement", typeof(string), typeof(SkinPicker), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Identifies the <see cref="CornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(SkinPicker), new PropertyMetadata(new CornerRadius(0d)));

        #endregion Dp's

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether [load skin by default].
        /// </summary>
        /// <value><c>true</c> if [load skin by default]; otherwise, <c>false</c>.</value>
        public bool LoadSkinByDefault
        {
            get
            {
                return (bool)GetValue(LoadSkinByDefaultProperty);
            }

            set
            {
                SetValue(LoadSkinByDefaultProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether VS2010 is visible.
        /// </summary>
        /// <value><c>true</c> if [load skin by default]; otherwise, <c>false</c>.</value>

        public bool IsVS2010Visible
        {
            get
            {
                return (bool)GetValue(IsVS2010VisibleProperty);
            }

            set
            {
                SetValue(IsVS2010VisibleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        internal SkinPickerItem SelectedItem
        {
            get
            {
                return (SkinPickerItem)GetValue(SelectedItemProperty);
            }

            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the corner radius.
        /// </summary>
        /// <value>The corner radius.</value>
        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(CornerRadiusProperty);
            }

            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the skin.
        /// </summary>
        /// <value>The skin for the controls.</value>
        public Skins Skin
        {
            get
            {
                return (Skins)GetValue(SkinProperty);
            }

            set
            {
                SetValue(SkinProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the target element.
        /// </summary>
        /// <value>The target element.</value>
        public string TargetElement
        {
            get
            {
                return (string)GetValue(TargetElementProperty);
            }

            set
            {
                SetValue(TargetElementProperty, value);
            }
        }

        #endregion Public Properties
    }

    #region Enums

    /// <summary>
    /// Skins list which is available in SkinManager.
    /// </summary>
    public enum Skins
    {
        /// <summary>
        /// Office2007Blue Theme
        /// </summary>
        Office2007Blue,

        /// <summary>
        /// Default theme
        /// </summary>
        Default,

        /// <summary>
        /// Office2007Black theme
        /// </summary>
        Office2007Black,

        /// <summary>
        /// Office2007Silver theme
        /// </summary>
        Office2007Silver,

        /// <summary>
        /// Blend theme
        /// </summary>
        Blend,

        /// <summary>
        /// VS2010 theme
        /// </summary>
        VS2010,

        /// <summary>
        /// Office2010 Blue
        /// </summary>
        Office2010Blue,

        /// <summary>
        /// Office2010 Black
        /// </summary>
        Office2010Black,

        /// <summary>
        /// Office2010 Silver
        /// </summary>
        Office2010Silver,

        /// <summary>
        /// Office2003
        /// </summary>
        Office2003
    }

    #endregion Enums
}