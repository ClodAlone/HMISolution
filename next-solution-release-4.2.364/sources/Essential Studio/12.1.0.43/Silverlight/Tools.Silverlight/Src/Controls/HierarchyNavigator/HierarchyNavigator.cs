#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Tools.Controls
{
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
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections;
#if WPF
using Syncfusion.Licensing;
#endif

    /// <summary>
    /// 
    /// </summary>
    [TemplatePart(Name = "PART_Root", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_HierarchyNavigatorItemsControl", Type = typeof(HierarchyNavigatorItemsControl))]
#if SILVERLIGHT
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
        Type = typeof(HierarchyNavigator), XamlResource = "/Syncfusion.Theming.Blend;component/HierarchyNavigator.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(HierarchyNavigator), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/HierarchyNavigator.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(HierarchyNavigator), XamlResource = "/Syncfusion.Theming.Office2007Black;component/HierarchyNavigator.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(HierarchyNavigator), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/HierarchyNavigator.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
        Type = typeof(HierarchyNavigator), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/HierarchyNavigator.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(HierarchyNavigator), XamlResource = "/Syncfusion.Theming.Office2010Black;component/HierarchyNavigator.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(HierarchyNavigator), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/HierarchyNavigator.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(HierarchyNavigator), XamlResource = "/Syncfusion.Theming.Default;component/HierarchyNavigator.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
        Type = typeof(HierarchyNavigator), XamlResource = "/Syncfusion.Theming.Office2003;component/HierarchyNavigator.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
        Type = typeof(HierarchyNavigator), XamlResource = "/Syncfusion.Theming.Windows7;component/HierarchyNavigator.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
        Type = typeof(HierarchyNavigator), XamlResource = "/Syncfusion.Theming.VS2010;component/HierarchyNavigator.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
        Type = typeof(HierarchyNavigator), XamlResource = "/Syncfusion.Theming.Metro;component/HirarchyNavigator.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent ,
     Type = typeof(HierarchyNavigator), XamlResource = "/Syncfusion.Theming.Transparent;component/HierarchyNavigator.xaml")]
#else
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2007Blue,
      Type = typeof(HierarchyNavigator), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/HierarchyNavigator/HierarchyThemes/Office2007BlueStyle.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2007Black,
    Type = typeof(HierarchyNavigator), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/HierarchyNavigator/HierarchyThemes/Office2007BlackStyle.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2007Silver,
    Type = typeof(HierarchyNavigator), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/HierarchyNavigator/HierarchyThemes/Office2007SilverStyle.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2010Blue,
      Type = typeof(HierarchyNavigator), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/HierarchyNavigator/HierarchyThemes/Office2010BlueStyle.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2010Black,
    Type = typeof(HierarchyNavigator), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/HierarchyNavigator/HierarchyThemes/Office2010BlackStyle.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Office2010Silver,
    Type = typeof(HierarchyNavigator), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/HierarchyNavigator/HierarchyThemes/Office2010SilverStyle.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Blend,
    Type = typeof(HierarchyNavigator), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/HierarchyNavigator/HierarchyThemes/BlendStyle.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Default,
    Type = typeof(HierarchyNavigator), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/HierarchyNavigator/HierarchyThemes/HierarchyResources.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.VS2010,
   Type = typeof(HierarchyNavigator), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/HierarchyNavigator/HierarchyThemes/VS2010Style.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Metro,
  Type = typeof(HierarchyNavigator), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/HierarchyNavigator/HierarchyThemes/MetroStyle.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.Skin.Transparent ,
Type = typeof(HierarchyNavigator), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/HierarchyNavigator/HierarchyThemes/TransparentStyle.xaml")]
#endif
    public class HierarchyNavigator : Control
    {
        /// <summary>
        /// 
        /// </summary>
        public HierarchyNavigator()
        {
            this.DefaultStyleKey = typeof(HierarchyNavigator);
            Items = new HierarchyNavigatorItemsCollection();
#if WPF
            if (EnvironmentTestTools.IsSecurityGranted)
            {
                EnvironmentTestTools.StartValidateLicense(typeof(HierarchyNavigator));
            }
#endif
        }

#if WPF
        static HierarchyNavigator()
        {
            //EnvironmentTest.ValidateLicense(typeof(HierarchyNavigator));
        }
#endif
        /// <summary>
        /// 
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(HierarchyNavigator), new PropertyMetadata(new CornerRadius(0)));
        
        private Grid PART_Root;
        private Grid PART_ProgressBar;
        private HierarchyNavigatorItemsControl PART_HierarchyNavigatorItemsControl;
        private Storyboard ProgressBarStoryBoard;
        private TimeSpan defaulttimeSpanInterval = new TimeSpan(0, 0, 0, 0, 500);

        /// <summary>
        /// 
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.PART_HierarchyNavigatorItemsControl = this.GetTemplateChild("PART_HierarchyNavigatorItemsControl") as HierarchyNavigatorItemsControl;
            this.PART_Root = this.GetTemplateChild("PART_Root") as Grid;
            this.PART_ProgressBar = this.GetTemplateChild("PART_ProgressBar") as Grid;
            this.SetHierarchyViewModel();        
        }

        /// <summary>
        /// 
        /// </summary>
        public HierarchyNavigatorItemsControl ItemsHost
        {
            get
            {
                return PART_HierarchyNavigatorItemsControl;
            }
        }

        private void SetHierarchyViewModel()
        {
            if (this.PART_HierarchyNavigatorItemsControl != null)
            {
                this.PART_HierarchyNavigatorItemsControl.HierarchyNavigatorSelectedItemChanged += new HierarchyNavigatorSelectedItemChangedEventHandler(PART_HierarchyNavigatorItemsControl_HierarchyNavigatorSelectedItemChanged);
                this.PART_HierarchyNavigatorItemsControl.NavigationPopupOpening += new EventHandler(PART_HierarchyNavigatorItemsControl_NavigationPopupOpening);
                this.PART_HierarchyNavigatorItemsControl.NavigationPopupOpened += new EventHandler(PART_HierarchyNavigatorItemsControl_NavigationPopupOpened);
                this.PART_HierarchyNavigatorItemsControl.NavigationPopupClosing += new EventHandler(PART_HierarchyNavigatorItemsControl_NavigationPopupClosing);
                this.PART_HierarchyNavigatorItemsControl.NavigationPopupClosed += new EventHandler(PART_HierarchyNavigatorItemsControl_NavigationPopupClosed);
                this.PART_HierarchyNavigatorItemsControl.HierarchyNavigatorRefreshButtonClick += new EventHandler(PART_HierarchyNavigatorItemsControl_HierarchyNavigatorRefreshButtonClick);
                this.PART_HierarchyNavigatorItemsControl.SetHierarchyNavigationViewModel(this.Model);
                this.PART_HierarchyNavigatorItemsControl.ShowRefreshButton = this.ShowRefreshButton;
                this.PART_HierarchyNavigatorItemsControl.ShowDropDownButton = this.ShowDropDownButton;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnRefreshButtonClick()
        {
            if (HierarchyNavigatorRefreshButtonClick != null)
                HierarchyNavigatorRefreshButtonClick(this, new EventArgs());
        }

        private void PART_HierarchyNavigatorItemsControl_HierarchyNavigatorRefreshButtonClick(object sender, EventArgs e)
        {
            if (HierarchyNavigatorRefreshButtonClick != null)
                HierarchyNavigatorRefreshButtonClick(this, e);
        }

        private Object tempSelectedItem = null;

        private void PART_HierarchyNavigatorItemsControl_HierarchyNavigatorSelectedItemChanged(object sender, HierarchyNavigatorSelectedItemChangedEventArgs e)
        {
            if(this.PART_HierarchyNavigatorItemsControl != null)
                this.PART_HierarchyNavigatorItemsControl.AddHistory();
            if (tempSelectedItem == e.HierarchyNavigatorSelectedItem) return;
            tempSelectedItem = e.HierarchyNavigatorSelectedItem;
            if (e.HierarchyNavigatorSelectedItem is HierarchyNavigatorDropDownItem)
                tempSelectedItem = (e.HierarchyNavigatorSelectedItem as HierarchyNavigatorDropDownItem).OriginalItem;
            if (HierarchyNavigatorSelectedItemChanged != null)
                HierarchyNavigatorSelectedItemChanged(sender, new HierarchyNavigatorSelectedItemChangedEventArgs(tempSelectedItem));

            #region Command Properties
            if (this.Command != null)
            {
                var hierarchyitm = e.HierarchyNavigatorSelectedItem as HierarchyNavigatorItem;
                if (hierarchyitm == null && e.HierarchyNavigatorSelectedItem as HierarchyNavigatorDropDownItem != null)
                {
                    hierarchyitm = (e.HierarchyNavigatorSelectedItem as HierarchyNavigatorDropDownItem).CloneHierarchyNavigatorItem();
                }
                this.Command.Execute(hierarchyitm);
            }
            #endregion
        }

        private void PART_HierarchyNavigatorItemsControl_NavigationPopupOpened(object sender, EventArgs e)
        {
            var itm = (sender as HierarchyNavigatorItem);
            if (NavigationPopupOpened != null)
                NavigationPopupOpened(itm, e);
        }

        private void PART_HierarchyNavigatorItemsControl_NavigationPopupClosed(object sender, EventArgs e)
        {
            var itm = (sender as HierarchyNavigatorItem);
            if (NavigationPopupClosed != null)
                NavigationPopupClosed(itm, e);
        }

        private void PART_HierarchyNavigatorItemsControl_NavigationPopupClosing(object sender, EventArgs e)
        {
            var itm = (sender as HierarchyNavigatorItem);
            if (NavigationPopupClosing != null)
                NavigationPopupClosing(itm, e);
        }

        private void PART_HierarchyNavigatorItemsControl_NavigationPopupOpening(object sender, EventArgs e)
        {
            var itm = (sender as HierarchyNavigatorItem);
            if (NavigationPopupOpening != null)
                NavigationPopupOpening((itm == null) ? sender : itm, e);
        }
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler HierarchyNavigatorRefreshButtonClick = delegate { };
        /// <summary>
        /// 
        /// </summary>
        public event HierarchyNavigatorSelectedItemChangedEventHandler HierarchyNavigatorSelectedItemChanged = delegate { };
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler NavigationPopupOpened = delegate { };
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler NavigationPopupOpening = delegate { };
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler NavigationPopupClosing = delegate { };
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler NavigationPopupClosed = delegate { };

        #region Command Support

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(HierarchyNavigator), null);

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(HierarchyNavigator), null);

        /// <summary>
        /// 
        /// </summary>
        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public object CommandParameter
        {
            get
            {
                return GetValue(CommandParameterProperty);
            }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }
        #endregion


        
        /// <summary>
        /// 
        /// </summary>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(HierarchyNavigator), new PropertyMetadata(null,OnSelectedItemChanged));

        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            HierarchyNavigator hN = sender as HierarchyNavigator;
            hN.OnSelectedItemChanged(args);
           
        }

        internal void OnSelectedItemChanged(DependencyPropertyChangedEventArgs args)
        {
#if WPF
            if (PART_HierarchyNavigatorItemsControl != null)
            {
                if (this.SelectedHierarchyNavigatorItem != null)
                {
                    if (!this.SelectedHierarchyNavigatorItem.Equals(args.NewValue))
                        PART_HierarchyNavigatorItemsControl.SelectNavigationItem(args.NewValue);
                }
                else
                {
                    //if (!IsLoaded)
                        SelectNavigationItem(args.NewValue);                        
                    //else
                    //    PART_HierarchyNavigatorItemsControl.SelectNavigationItem(args.NewValue);
                }
            }
#else
            if (this.SelectedHierarchyNavigatorItem != null)
            {
                if (!this.SelectedHierarchyNavigatorItem.Equals(args.NewValue))
                    SelectNavigationItem(args.NewValue);
            }
            else
            {
                SelectNavigationItem(args.NewValue);
            }
#endif
            
        }
        /// <summary>
        /// 
        /// </summary>
        public Visibility ShowDropDownButton
        {
            get { return (Visibility)GetValue(ShowDropDownButtonProperty); }
            set { SetValue(ShowDropDownButtonProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowDropDownButton.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowDropDownButtonProperty =
            DependencyProperty.Register("ShowDropDownButton", typeof(Visibility), typeof(HierarchyNavigator), new PropertyMetadata(Visibility.Visible, OnShowDropDownButtonChanged));

        private static void OnShowDropDownButtonChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var senderObj = sender as HierarchyNavigator;
            if (senderObj.Model != null)
            {
                senderObj.Model.ShowDropDownButton = senderObj.ShowDropDownButton;
            }
            if (senderObj.PART_HierarchyNavigatorItemsControl != null)
                senderObj.PART_HierarchyNavigatorItemsControl.ShowDropDownButton = senderObj.ShowDropDownButton;
        }

        /// <summary>
        /// 
        /// </summary>
        public Visibility ShowRefreshButton
        {
            get { return (Visibility)GetValue(ShowRefreshButtonProperty); }
            set { SetValue(ShowRefreshButtonProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowRefreshButton.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowRefreshButtonProperty =
            DependencyProperty.Register("ShowRefreshButton", typeof(Visibility), typeof(HierarchyNavigator), new PropertyMetadata(Visibility.Visible, OnShowRefreshButtonChanged));

        private static void OnShowRefreshButtonChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var senderObj = sender as HierarchyNavigator;
            if (senderObj.Model != null)
            {
                senderObj.Model.ShowRefreshButton = senderObj.ShowRefreshButton;
            }
            if (senderObj.PART_HierarchyNavigatorItemsControl != null)
                senderObj.PART_HierarchyNavigatorItemsControl.ShowRefreshButton = senderObj.ShowRefreshButton;
        }

#if WPF
        public TimeSpan AnimationSpeed
        {
            get
            {
                return (TimeSpan)GetValue(AnimationSpeedProperty);
            }
            set
            {
                SetValue(AnimationSpeedProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for AnimationSpeed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnimationSpeedProperty =
            DependencyProperty.Register("AnimationSpeed", typeof(TimeSpan), typeof(HierarchyNavigator), new PropertyMetadata(TimeSpan.FromMilliseconds(300), null));

#endif  
        /// <summary>
        /// 
        /// </summary>
        public bool IsEnableEditMode
        {
            get { return (bool)GetValue(IsEnableEditModeProperty); }
            set { SetValue(IsEnableEditModeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsEnableEditMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsEnableEditModeProperty =
            DependencyProperty.Register("IsEnableEditMode", typeof(bool), typeof(HierarchyNavigator), new PropertyMetadata(false, OnIsEnableEditModeChanged));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnIsEnableEditModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var senderObj = sender as HierarchyNavigator;
            if (senderObj.Model != null)
            {
                senderObj.Model.IsEnableEditMode = senderObj.IsEnableEditMode;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsEnableHistory
        {
            get { return (bool)GetValue(IsEnableHistoryProperty); }
            set { SetValue(IsEnableHistoryProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsEnableHistory.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsEnableHistoryProperty =
            DependencyProperty.Register("IsEnableHistory", typeof(bool), typeof(HierarchyNavigator), new PropertyMetadata(false, OnIsEnableHistoryChanged));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnIsEnableHistoryChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var senderObj = sender as HierarchyNavigator;
            if (senderObj.Model != null)
            {
                senderObj.Model.IsEnableHistory = senderObj.IsEnableHistory;
            }
        }

        private bool showToolTip = false;
        /// <summary>
        /// 
        /// </summary>
        public bool ShowToolTip
        {
            get
            {
                return this.showToolTip;
            }

            set
            {
                if (this.showToolTip != value)
                {
                    this.showToolTip = value;
                    if (this.Model != null)
                    {
                        this.Model.ShowToolTip = this.ShowToolTip;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MaxDrillDownLevel
        {
            get { return (int)GetValue(MaxDrillDownLevelProperty); }
            set { SetValue(MaxDrillDownLevelProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MaxDrillDownLevel.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaxDrillDownLevelProperty =
            DependencyProperty.Register("MaxDrillDownLevel", typeof(int), typeof(HierarchyNavigator), new PropertyMetadata(-1, OnMaxDrillDownLevelChanged));

        private static void OnMaxDrillDownLevelChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var senderObj = sender as HierarchyNavigator;
            if (senderObj.Model != null)
                senderObj.Model.MaxDrillDownLevel = senderObj.MaxDrillDownLevel;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DisplayMemberPath.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(HierarchyNavigator), new PropertyMetadata(string.Empty, OnDisplayMemberPathChanged));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnDisplayMemberPathChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var sendr = (HierarchyNavigator)sender;
            if (sendr != null && sendr.Model != null)
                sendr.Model.DisplayMemberPath = (string)e.NewValue;
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(HierarchyNavigator), new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var sendr = (HierarchyNavigator)sender;
            sendr.Model.ItemsSource = (IEnumerable)e.NewValue;
        }

        /// <summary>
        /// 
        /// </summary>
        public ItemsPanelTemplate ItemsPanel
        {
            get { return (ItemsPanelTemplate)GetValue(ItemsPanelProperty); }
            set { SetValue(ItemsPanelProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemsPanel.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemsPanelProperty =
            DependencyProperty.Register("ItemsPanel", typeof(ItemsPanelTemplate), typeof(HierarchyNavigator), new PropertyMetadata(null, OnItemsPanelChanged));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnItemsPanelChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var sendr = (HierarchyNavigator)sender;
            sendr.Model.ItemsPanel = (ItemsPanelTemplate)e.NewValue;
        }

        /// <summary>
        /// 
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(HierarchyNavigator), new PropertyMetadata(null, OnItemTemplateChanged));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnItemTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var sendr = (HierarchyNavigator)sender;
            sendr.Model.ItemTemplate = (DataTemplate)e.NewValue;
        }

        /// <summary>
        /// 
        /// </summary>
        public HierarchyNavigatorItem SelectedHierarchyNavigatorItem
        {
            get { return (HierarchyNavigatorItem)GetValue(SelectedHierarchyNavigatorItemProperty); }
            internal set { SetValue(SelectedHierarchyNavigatorItemProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedHierarchyNavigatorItem.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedHierarchyNavigatorItemProperty =
            DependencyProperty.Register("SelectedHierarchyNavigatorItem", typeof(HierarchyNavigatorItem), typeof(HierarchyNavigator), new PropertyMetadata(null, OnSelectedHierarchyNavigatorItemChanged));

        private static void OnSelectedHierarchyNavigatorItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private HierarchyNavigatorModel OnCreateHierarchyNavigatorModel()
        {
            var model = new HierarchyNavigatorModel();
            return model;
        }

        private void model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedHierarchyNavigatorItem")
            {
                this.SelectedHierarchyNavigatorItem = this.Model.SelectedHierarchyNavigatorItem;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public HierarchyNavigatorItemsCollection Items
        {
            get { return this.Model.HierarchyNavigatorItems; }
            set
            {
                this.Model.HierarchyNavigatorItems = value;
            }
        }

        private HierarchyNavigatorModel model;
        /// <summary>
        /// 
        /// </summary>
        public HierarchyNavigatorModel Model
        {
            get
            {
                if (this.model == null)
                {
                    this.model = this.OnCreateHierarchyNavigatorModel();
                    this.model.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(model_PropertyChanged);
                }
                return this.model;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ShowProgressBar()
        {
            this.ShowProgressBar(defaulttimeSpanInterval);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeSpanInterval"></param>
        public void ShowProgressBar(TimeSpan timeSpanInterval)
        {
            //VisualStateManager.GoToState(this, "ShowProgress", false);
            if (timeSpanInterval.Ticks <= 0) return;
            if (this.PART_ProgressBar == null) return;
            defaulttimeSpanInterval = timeSpanInterval;
            ProgressBarStoryBoard = new Storyboard();
            ProgressBarStoryBoard.Stop();
            ProgressBarStoryBoard.Children.Clear();
            DoubleAnimationUsingKeyFrames DblAnim = new DoubleAnimationUsingKeyFrames();
            DblAnim.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath(("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)")));
            DblAnim.BeginTime = new TimeSpan(0, 0, 0);
            Storyboard.SetTarget(DblAnim, PART_ProgressBar);
            ProgressBarStoryBoard.Children.Add(DblAnim);
            SplineDoubleKeyFrame EsingKey = new SplineDoubleKeyFrame();
            EsingKey.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
            DblAnim.KeyFrames.Add(EsingKey);
            EsingKey.Value = 0;
            SplineDoubleKeyFrame EsingKey1 = new SplineDoubleKeyFrame();
            EsingKey1.KeyTime = KeyTime.FromTimeSpan(timeSpanInterval);
            DblAnim.KeyFrames.Add(EsingKey1);
            EsingKey1.Value = 1;
            SplineDoubleKeyFrame EsingKey2 = new SplineDoubleKeyFrame();
            EsingKey2.KeyTime = KeyTime.FromTimeSpan(timeSpanInterval + new TimeSpan(0, 0, 0, 0, 100));
            DblAnim.KeyFrames.Add(EsingKey2);
            EsingKey2.Value = 1;
            SplineDoubleKeyFrame EsingKey3 = new SplineDoubleKeyFrame();
            EsingKey3.KeyTime = KeyTime.FromTimeSpan(timeSpanInterval + new TimeSpan(0, 0, 0, 0, 200));
            DblAnim.KeyFrames.Add(EsingKey3);
            EsingKey3.Value = 0;

            DoubleAnimationUsingKeyFrames DblAnimOpa = new DoubleAnimationUsingKeyFrames();
            DblAnimOpa.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath(("(UIElement.Opacity)")));
            DblAnimOpa.BeginTime = new TimeSpan(0, 0, 0);
            Storyboard.SetTarget(DblAnimOpa, PART_ProgressBar);
            ProgressBarStoryBoard.Children.Add(DblAnimOpa);
            SplineDoubleKeyFrame EsingKeyOpa = new SplineDoubleKeyFrame();
            EsingKeyOpa.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
            DblAnimOpa.KeyFrames.Add(EsingKeyOpa);
            EsingKeyOpa.Value = 0.8;
            SplineDoubleKeyFrame EsingKey1Opa = new SplineDoubleKeyFrame();
            EsingKey1Opa.KeyTime = KeyTime.FromTimeSpan(timeSpanInterval);
            DblAnimOpa.KeyFrames.Add(EsingKey1Opa);
            EsingKey1Opa.Value = 0.8;
            SplineDoubleKeyFrame EsingKey2Opa = new SplineDoubleKeyFrame();
            EsingKey2Opa.KeyTime = KeyTime.FromTimeSpan(timeSpanInterval + new TimeSpan(0, 0, 0, 0, 100));
            DblAnimOpa.KeyFrames.Add(EsingKey2Opa);
            EsingKey2Opa.Value = 0;
            ProgressBarStoryBoard.Begin();
        }

        /// <summary>
        /// 
        /// </summary>
        public void CancelProgressBar()
        {
            if (this.ProgressBarStoryBoard != null)
            {
                this.CancelProgressBar(defaulttimeSpanInterval);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeSpanInterval"></param>
        public void CancelProgressBar(TimeSpan timeSpanInterval)
        {
            if (this.PART_ProgressBar == null) return;
            ProgressBarStoryBoard = new Storyboard();
            ProgressBarStoryBoard.Stop();
            ProgressBarStoryBoard.Children.Clear();
            DoubleAnimationUsingKeyFrames DblAnim = new DoubleAnimationUsingKeyFrames();
            DblAnim.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath(("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)")));
            DblAnim.BeginTime = new TimeSpan(0, 0, 0);
            Storyboard.SetTarget(DblAnim, PART_ProgressBar);
            ProgressBarStoryBoard.Children.Add(DblAnim);
            SplineDoubleKeyFrame EsingKey1 = new SplineDoubleKeyFrame();
            EsingKey1.KeyTime = KeyTime.FromTimeSpan(timeSpanInterval);
            DblAnim.KeyFrames.Add(EsingKey1);
            EsingKey1.Value = 0;

            DoubleAnimationUsingKeyFrames DblAnimOpa = new DoubleAnimationUsingKeyFrames();
            DblAnimOpa.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath(("(UIElement.Opacity)")));
            DblAnimOpa.BeginTime = new TimeSpan(0, 0, 0);
            Storyboard.SetTarget(DblAnimOpa, PART_ProgressBar);
            ProgressBarStoryBoard.Children.Add(DblAnimOpa);
            SplineDoubleKeyFrame EsingKeyOpa = new SplineDoubleKeyFrame();
            EsingKeyOpa.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0));
            DblAnimOpa.KeyFrames.Add(EsingKeyOpa);
            EsingKeyOpa.Value = 0.8;
            SplineDoubleKeyFrame EsingKey1Opa = new SplineDoubleKeyFrame();
            EsingKey1Opa.KeyTime = KeyTime.FromTimeSpan(timeSpanInterval);
            DblAnimOpa.KeyFrames.Add(EsingKey1Opa);
            EsingKey1Opa.Value = 0.8;
            SplineDoubleKeyFrame EsingKey2Opa = new SplineDoubleKeyFrame();
            EsingKey2Opa.KeyTime = KeyTime.FromTimeSpan(timeSpanInterval + new TimeSpan(0, 0, 0, 0, 100));
            DblAnimOpa.KeyFrames.Add(EsingKey2Opa);
            EsingKey2Opa.Value = 0;
            ProgressBarStoryBoard.Begin();
            defaulttimeSpanInterval = new TimeSpan(0, 0, 0, 0, 500);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hierarchyNavigatorItem"></param>
        /// <returns></returns>
        public bool ShowNavigationPopupItems(object hierarchyNavigatorItem)
        {
            if (this.PART_HierarchyNavigatorItemsControl != null)
            {
                return this.PART_HierarchyNavigatorItemsControl.ShowNavigationPopupItems(hierarchyNavigatorItem);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public bool SelectNavigationItem(object sender)
        {
            bool res = false;
            if (this.PART_HierarchyNavigatorItemsControl != null)
            {
               
                if (this.Model != null && this.PART_HierarchyNavigatorItemsControl.Items.Count > 0)
                {
                    this.Model.SelectedLevel = this.PART_HierarchyNavigatorItemsControl.Items.Count - 1;
                    if (this.ItemsSource == null)
                    {
                        if (sender is HierarchyNavigatorItem)
                        {
                            this.Model.SelectedHierarchyNavigatorItem = sender as HierarchyNavigatorItem;
                            (sender as HierarchyNavigatorItem).Level = this.Model.SelectedLevel + 1;
                        }
                         res = this.PART_HierarchyNavigatorItemsControl.SelectNavigationItem(sender);
                         if (res)
                             this.SelectedItem = sender;
                         return res;
                    }
                    else
                    {
                        IList barItems = (PART_HierarchyNavigatorItemsControl.ItemsSource as IList);



                        if (barItems != null)
                        {
                            object item = barItems[0];

                            List<object> oldCollection = new List<object>();
                            foreach (var obj in barItems)
                                oldCollection.Add(obj);
                            barItems.Clear();
                            this.Model.SelectedLevel = 0;
                            barItems.Add(item);
#if SILVERLIGHT
                            UpdateLayout();                
#endif
                            HierarchyNavigatorBarContent barcontent = PART_HierarchyNavigatorItemsControl.ItemContainerGenerator.ContainerFromItem(PART_HierarchyNavigatorItemsControl.Items[0]) as HierarchyNavigatorBarContent;
                            if(barcontent !=null)
                            res = IternateHierarichalCollection(barcontent, sender);
                            if (res)
                                this.SelectedItem = sender;
                            else
                            {
                                barItems.Clear();
                                foreach (var obj in oldCollection)
                                    barItems.Add(obj);
                            }
                        }


                            return res;
                        }                   
                    
                }
              
            }
            return false;
        }

        List<object> processedCollection = new List<object>();
        List<object> iterateProcessedCollection = new List<object>();

        internal bool IternateHierarichalCollection(HierarchyNavigatorBarContent hbarContent, object sender)
        {
            if (hbarContent.Header == sender)
            {
               return true;
            }
            if (hbarContent.Items.Count > 0)
            {
                foreach (var obj in hbarContent.Items)
                {
                    (PART_HierarchyNavigatorItemsControl.ItemsSource as IList).Add(obj);
                    HierarchyNavigatorBarContent barcontent = PART_HierarchyNavigatorItemsControl.ItemContainerGenerator.ContainerFromItem(PART_HierarchyNavigatorItemsControl.Items[PART_HierarchyNavigatorItemsControl.Items.Count - 1]) as HierarchyNavigatorBarContent;
                    if (barcontent != null)
                    {
                     bool res=   IternateHierarichalCollection(barcontent, sender);
                     this.Model.SelectedLevel++;
                     if (res)
                         return true;
                     else
                         (PART_HierarchyNavigatorItemsControl.ItemsSource as IList).Remove(obj);
                    }

                }
            }
            return false;
        }
    }


      
   

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void HierarchyNavigatorSelectedItemChangedEventHandler(object sender, HierarchyNavigatorSelectedItemChangedEventArgs e);

    /// <summary>
    /// 
    /// </summary>
    public class HierarchyNavigatorItemsCollection : ObservableCollection<HierarchyNavigatorItem>
    {
        /// <summary>
        /// 
        /// </summary>
        public HierarchyNavigatorItemsCollection()
        {

        }

        internal bool IsMatched(HierarchyNavigatorItemsCollection history)
        {
            var cnt = 0;
            foreach (var item in history)
            {
                if (this[cnt].IsMatched(item))
                {
                    cnt++;
                }
            }
            if (cnt == history.Count) return true;
            return false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HierarchyNavigatorSelectedItemChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public Object HierarchyNavigatorSelectedItem { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public HierarchyNavigatorSelectedItemChangedEventArgs()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hierarchyNavigatorSelectedItem"></param>
        public HierarchyNavigatorSelectedItemChangedEventArgs(object hierarchyNavigatorSelectedItem)
        {
            this.HierarchyNavigatorSelectedItem = hierarchyNavigatorSelectedItem;
        }
    }
}