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
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using Syncfusion.Windows.Shared;
#if WPF
using Syncfusion.Licensing;
#endif

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the DropDownMenGroup Class.
    /// </summary>
#if WPF
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
      Type = typeof(DropDownMenuGroup), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/DropDownButton/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(DropDownMenuGroup), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/DropDownButton/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(DropDownMenuGroup), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/DropDownButton/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
      Type = typeof(DropDownMenuGroup), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/DropDownButton/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(DropDownMenuGroup), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/DropDownButton/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(DropDownMenuGroup), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/DropDownButton/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(DropDownMenuGroup), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/DropDownButton/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
    Type = typeof(DropDownMenuGroup), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/DropDownButton/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(DropDownMenuGroup), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/DropDownButton/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(DropDownMenuGroup), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/DropDownButton/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(DropDownMenuGroup), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/DropDownButton/Themes/DropDownButton.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
   Type = typeof(DropDownMenuGroup), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/DropDownButton/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
   Type = typeof(DropDownMenuGroup), XamlResource = "/Syncfusion.Shared.WPF;component/Controls/ButtonControls/DropDownButton/Themes/TransparentStyle.xaml")]
#endif
    public class DropDownMenuGroup : HeaderedItemsControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DropDownMenuGroup"/> class.
        /// </summary>
        public DropDownMenuGroup()
        {
#if WPF
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(DropDownMenuGroup));
            }
#endif
            DefaultStyleKey = typeof(DropDownMenuGroup);
            MoreItems = new ObservableCollection<UIElement>();
            this.Loaded += new RoutedEventHandler(DropDownMenuGroup_Loaded);
            this.KeyDown += new KeyEventHandler(DropDownMenuGroup_KeyDown);
        }

#if WPF
        static DropDownMenuGroup()
        {
            //EnvironmentTest.ValidateLicense(typeof(DropDownMenuGroup));
        }
#endif

#if SILVERLIGHT
        /// <summary>
        /// Initializes the <see cref="DropDownMenuGroup"/> class.
        /// </summary>
        static DropDownMenuGroup()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                LoadDependentAssemblies load = new LoadDependentAssemblies();
                load = null;
            }
        }
        int downClickCount;
#endif

        private Thumb _resizethumb;
        /// <summary>
        /// 
        /// </summary>
        public int menuButtonDownCount;
        

        #region Dependency Properties

        /// <summary>
        /// Gets or sets a value indicating whether [icon bar enabled].
        /// </summary>
        /// <value><c>true</c> if [icon bar enabled]; otherwise, <c>false</c>.</value>
        [Category("Appearance")]
        public bool IconBarEnabled
        {
            get { return (bool)GetValue(IconBarEnabledProperty); }
            set { SetValue(IconBarEnabledProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsIconTrayEnabled.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconBarEnabledProperty =
            DependencyProperty.Register("IconBarEnabled", typeof(bool), typeof(DropDownMenuGroup), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is more items icon tray enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is more items icon tray enabled; otherwise, <c>false</c>.
        /// </value>
        [Category("Apperance")]
        public bool IsMoreItemsIconTrayEnabled
        {
            get { return (bool)GetValue(IsMoreItemsIconTrayEnabledProperty); }
            set { SetValue(IsMoreItemsIconTrayEnabledProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsMoreItemsIconTrayEnabled.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsMoreItemsIconTrayEnabledProperty =
            DependencyProperty.Register("IsMoreItemsIconTrayEnabled", typeof(bool), typeof(DropDownMenuGroup), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets the scroll bar visibility.
        /// </summary>
        /// <value>The scroll bar visibility.</value>
        [Category("Apperance")]
        public ScrollBarVisibility ScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(ScrollBarVisibilityProperty); }
            set { SetValue(ScrollBarVisibilityProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for ScrollBarVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ScrollBarVisibilityProperty =
            DependencyProperty.Register("ScrollBarVisibility", typeof(ScrollBarVisibility), typeof(DropDownMenuGroup), new PropertyMetadata(ScrollBarVisibility.Disabled));

        /// <summary>
        /// Gets or sets the more items.
        /// </summary>
        /// <value>The more items.</value>
        [Category("Common Properties")]
        public ObservableCollection<UIElement> MoreItems
        {
            get { return (ObservableCollection<UIElement>)GetValue(MoreItemsProperty); }
            set { SetValue(MoreItemsProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for MoreItems.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MoreItemsProperty =
            DependencyProperty.Register("MoreItems", typeof(ObservableCollection<UIElement>), typeof(DropDownMenuGroup), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is resizable.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is resizable; otherwise, <c>false</c>.
        /// </value>
        [Category("Layout")]
        public bool IsResizable
        {
            get { return (bool)GetValue(IsResizableProperty); }
            set { SetValue(IsResizableProperty, value); }
        }

        
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsResizable.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsResizableProperty =
            DependencyProperty.Register("IsResizable", typeof(bool), typeof(DropDownMenuGroup), new PropertyMetadata(false));

        #endregion

        /// <summary>
        /// Builds the visual tree for the <see cref="T:System.Windows.Controls.HeaderedItemsControl"/> when a new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            _resizethumb = GetTemplateChild("PART_ResizeThumb") as Thumb;
            if (_resizethumb != null)
            {
                _resizethumb.DragDelta += new DragDeltaEventHandler(_resizethumb_DragDelta);
            }
            base.OnApplyTemplate();
        }

        void _resizethumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (double.IsNaN(Height))
            {
                if (ActualHeight + e.VerticalChange > 0)
                {
                    Height = ActualHeight + e.VerticalChange;
                }
            }
            else
            {
                if (Height + e.VerticalChange > 0)
                {
                    Height = Height + e.VerticalChange;
                }
            }
        }

        void DropDownMenuGroup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (this.Parent is DropDownButtonAdv)
                    ((DropDownButtonAdv)this.Parent).IsDropDownOpen = false;
#if SILVERLIGHT
                else if (this.Parent is DropDown)
                    ((DropDown)this.Parent).IsOpen = false;
#else
                 else if (this.Parent is Popup)
                    ((Popup)this.Parent).IsOpen = false;
#endif
            }
        }

        void DropDownMenuGroup_Loaded(object sender, RoutedEventArgs e)
        {
#if SILVERLIGHT
            if (this.Items.Count > 0)
            {
                HideMouseOverItems();
                downClickCount = 0;
                this.menuButtonDownCount = 0;
                var firstItem = this.ItemContainerGenerator.ContainerFromIndex(0);
                if (firstItem is HeaderedItemsControl)
                    ((HeaderedItemsControl)firstItem).Focus();
                else if (firstItem is ButtonBase)
                    ((ButtonBase)firstItem).Focus();
            }
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        public void HandleLoadedEvent()
        {
            this.Loaded -= new RoutedEventHandler(DropDownMenuGroup_Loaded);
            this.Loaded += new RoutedEventHandler(DropDownMenuGroup_Loaded);
        }
        
#if SILVERLIGHT
        /// <summary>
        /// 
        /// </summary>
        /// <param name="focusedMenuItem"></param>
        /// <param name="enteredKey"></param>
        public void FindAndFocusItem(DropDownMenuItem focusedMenuItem, Key enteredKey)
        {
            int count = Items.Count;
            int startingIndex = 0;

            HideMouseOverItems();

            if (focusedMenuItem != null && (focusedMenuItem.Parent == this))
                startingIndex = ItemContainerGenerator.IndexFromContainer(focusedMenuItem);

            int index = startingIndex;
            do
            {
                if (enteredKey == Key.Up)
                    index = (index + count - 1) % count;
                else if (enteredKey == Key.Down)
                {
                    if (downClickCount == 0)
                    { index = 0; downClickCount++; }
                    else
                        index = (index + count + 1) % count;
                }
                else if (enteredKey == Key.Home)
                    index = 0;
                else
                    index = count - 1;

                DropDownMenuItem container = ItemContainerGenerator.ContainerFromIndex(index) as DropDownMenuItem;
                if (null != container)
                {
                    if (container.IsEnabled)
                    {
                        container.IsMouseOver = true;
                        container.Focus();
                        break;
                    }
                }
            }
            while (index != startingIndex);
        }


        /// <summary>
        /// 
        /// </summary>
        public void HideMouseOverItems()
        {
            foreach (var item in this.Items)
            {
                var menuItem = item as DropDownMenuItem;
                if (menuItem != null)
                {
                    if (menuItem.IsMouseOver)
                        menuItem.IsMouseOver = false;
                }
            }
        }
#endif
    }
}
