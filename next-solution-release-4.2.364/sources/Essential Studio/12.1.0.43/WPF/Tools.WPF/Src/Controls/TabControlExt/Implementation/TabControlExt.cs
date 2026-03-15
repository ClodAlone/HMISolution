// <copyright file="TabControlExt.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using Microsoft.Win32;
using Syncfusion.Windows.Shared;
using Syncfusion.Licensing;
using System.Threading;
using System.Windows.Interop;
using System.Linq;
using Syncfusion.Windows.Tools.Controls.Resources;
using System.Windows.Threading;
using System.Windows.Data;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// TabControlExt, an application can define multiple pages for the same area of a
    /// window. TabControlExt contains the TabItemExt which is used to define Tab Items
    /// for the TabControlExt. Clicking on a Tab Item Header, will display the data
    /// corresponding to that particular Tab Item.
    /// </summary>
    /// <example>
    /// 	<code lang="XAML">
    /// 		<Window x:Class="TabControlExt.Window1" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" Title="Window1" Height="300" Width="300" xmlns:syncfusion="http://schemas.syncfusion.com/wpf">
    /// 			<Grid>
    /// 				<syncfusion:TabControlExt Name="tabControlExt">
    /// 				</syncfusion:TabControlExt>
    /// 			</Grid>
    /// 		</Window>
    /// 	</code>
    /// 	<code lang="C#">
    /// using System;
    /// using System.Collections.Generic;
    /// using System.Linq;
    /// using System.Text;
    /// using System.Windows;
    /// using System.Windows.Controls;
    /// using System.Windows.Data;
    /// using System.Windows.Documents;
    /// using System.Windows.Input;
    /// using System.Windows.Media;
    /// using System.Windows.Media.Imaging;
    /// using System.Windows.Navigation;
    /// using System.Windows.Shapes;
    /// using Syncfusion.Windows.Tools.Controls;
    /// namespace TabControlExt
    /// {
    /// /// <summary>
    /// /// Interaction logic for Window1.xaml
    /// /// </summary>
    /// public partial class Window1 : Window
    /// {
    /// public Window1()
    /// {
    /// InitializeComponent();
    /// TabControlExt tabControlExt = new TabControlExt();
    /// StackPanel stackPanel = new StackPanel();
    /// stackPanel.Children.Add(tabControlExt);
    /// }
    /// }
    /// }
    /// </code>
    /// </example>

  
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
   Type = typeof(TabControlExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(TabControlExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(TabControlExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
   Type = typeof(TabControlExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(TabControlExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(TabControlExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(TabControlExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(TabControlExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
    Type = typeof(TabControlExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(TabControlExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(TabControlExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(TabControlExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
   Type = typeof(TabControlExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
  Type = typeof(TabControlExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent ,
Type = typeof(TabControlExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/TransparentStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2013,
Type = typeof(TabControlExt), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2013Style.xaml")] 
    public class TabControlExt : TabControl,IDisposable
    {
        #region Constants
        /// <summary>
        /// Contains "cTabControlExtValues" sub key name.
        /// </summary>
        private const string C_regSubKeyName = "TabControlExtValues";

        /// <summary>
        /// Contains "SaveTabState" param name.
        /// </summary>
        private const string C_regParamName = "SaveTabState";

        /// <summary>
        /// Contains "close other tabs" menu item name.
        /// </summary>
        private const string MENU_CLOSE_OTHER_NAME = "Close All But This";

        /// <summary>
        /// Contains "close tab" menu item name.
        /// </summary>
        private const string MENU_CLOSE_NAME = "Close";

        /// <summary>
        /// Contains "close all tabs" menu item name.
        /// </summary>
        private const string MENU_CLOSE_ALL_NAME = "Close All";

        /// <summary>
        /// Contains name of the main tab control Grid.
        /// </summary>
        private const string C_tabControlGridName = "TabControlGrid";

        /// <summary>
        /// Path to the DragMarkerStyle for themes.
        /// </summary>
        private const string C_office2007DragMarkerStyle = "pack://application:,,,/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/Office2007BlueStyle.xaml";

        /// <summary>
        /// Path to the DragMarkerStyle for office skins.
        /// </summary>
        private const string C_office2008DragMarkerStyle = "pack://application:,,,/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/generic.xaml";

        /// <summary>
        /// Name of the DragMarkerStyle for themes.
        /// </summary>
        private const string C_office2008DragMarkerStyleName = "VS2008DragMarkerStyle";

        /// <summary>
        /// Name of the DragMarkerStyle for office skins.
        /// </summary>
        private const string C_office2007DragMarkerStyleName = "Office2007DragMarkerStyle";

        /// <summary>
        /// Part of name of the office skins.
        /// </summary>
        private const string C_officeStyles = "Office";

        /// <summary>
        /// Name of Blend skin.
        /// </summary>
        private const string C_blend = "Blend";

        /// <summary>
        /// Name of the classic theme.
        /// </summary>
        private const string C_classic = "Classic";

        #endregion

        #region Private members

        /// <summary>
        /// Represent object reference of Resource wrapper class.
        /// </summary>
        ResourceWrapper wrapper = new ResourceWrapper();

        /// <summary>
        /// internal variable which has loadstate
        /// </summary>
        internal bool m_loadstate = false;

        /// <summary>
        /// Stores the bool value representing IsDragging.
        /// </summary>
        private static bool bIsDragging;

        /// <summary>
        /// Contains activated tab item.
        /// </summary>
        private TabItemExt m_activatedItem;

        /// <summary>
        /// Presents file name for saving in the internal isolated storage.
        /// </summary>
        private readonly string m_StoreFileName = AppDomain.CurrentDomain.SetupInformation.ApplicationName + ".dat";

        /// <summary>
        /// Command Binding for Close tab.
        /// </summary>
        private readonly CommandBinding m_CloseTabCommandBinding;

        /// <summary>
        /// Command Binding for Close current tab.
        /// </summary>
        private readonly CommandBinding m_CloseCurrentTabCommandBinding;

        /// <summary>
        /// Command Binding for Close tabs.
        /// </summary>
        private readonly CommandBinding m_CloseTabsCommandBinding;

        /// <summary>
        /// Stores the Tab panel.
        /// </summary>
        private TabPanelAdv m_tabPanel;

        /// <summary>
        /// Stores the Tab layout panel.
        /// </summary>
        private TabLayoutPanel m_tabLayoutPanel;

        /// <summary>
        /// Stores the Tab item ext.
        /// </summary>
        internal TabItemExt lastSelected = null;

        /// <summary>
        /// Stores the Toggle button.
        /// </summary>
        private ToggleButton m_listMenuButton;

        /// <summary>
        /// Contains height of the selected tab item.
        /// </summary>
        internal double M_tabItemHeight = 0;

        /// <summary>
        /// Contains default drag marker style.
        /// </summary>
        private string m_dragMarkerStyle = C_office2007DragMarkerStyle;

        /// <summary>
        /// Contains name of the default drag marker style.
        /// </summary>
        private string m_dragMarkerStyleName = C_office2007DragMarkerStyleName;

        /// <summary>
        /// internal variable which has window preview style
        /// </summary>
        private WindowStyle m_prevwindowstyle;

        /// <summary>
        /// internal variable which has window preview state
        /// </summary>
        private WindowState m_prevwindowstate;

        /// <summary>
        /// internal variable which has flag
        /// </summary>
        public bool m_Flag = true;

        internal bool allowdrop = true;

        internal bool m_loaded = false;

        #endregion

        #region Attached DP Getters & Setters
        /// <summary>
        /// Gets the value of the UseCustomEditableTemplate dependency property.
        /// </summary>
        /// <param name="obj">Sender Object.</param>
        /// <returns>returns bool value</returns>
        public static bool GetUseCustomEditableTemplate(DependencyObject obj)
        {
            return (bool)obj.GetValue(UseCustomEditableTemplateProperty);
        }

        /// <summary>
        /// Sets the value of the UseCustomEditableTemplate dependency property.
        /// </summary>
        /// <param name="obj">Sender Object.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetUseCustomEditableTemplate(DependencyObject obj, bool value)
        {
            obj.SetValue(UseCustomEditableTemplateProperty, value);
        }

        /// <summary>
        /// Gets the value of the CustomEditableTemplate dependency property.
        /// </summary>
        /// <param name="obj">Sender Object.</param>
        /// <returns>returns a DataTemplate</returns>
        public static DataTemplate GetCustomEditableTemplate(DependencyObject obj)
        {
            return (DataTemplate)obj.GetValue(CustomEditableTemplateProperty);
        }

        /// <summary>
        /// Sets the value of the CustomEditableTemplate dependency property.
        /// </summary>
        /// <param name="obj">Sender Object.</param>
        /// <param name="value">The value.</param>
        public static void SetCustomEditableTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(CustomEditableTemplateProperty, value);
        }

        /// <summary>
        /// Gets the value of the IsEditing dependency property.
        /// </summary>
        /// <param name="obj">Sender Object.</param>
        /// <returns>returns bool value</returns>
        public static bool GetIsEditing(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEditingProperty);
        }

        /// <summary>
        /// Sets the value of the IsEditing dependency property.
        /// </summary>
        /// <param name="obj">Sender Object.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        protected internal static void SetIsEditing(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEditingPropertyKey, value);
        }

        /// <summary>
        /// Gets the value of the ContextMenuItems dependency property.
        /// </summary>
        /// <param name="obj">Sender Object.</param>
        /// <returns>returns an ObservableCollection</returns>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public static ObservableCollection<object> GetContextMenuItems(DependencyObject obj)
        {
            return (ObservableCollection<object>)obj.GetValue(ContextMenuItemsProperty);
        }

        /// <summary>
        /// Sets the value of the ContextMenuItems dependency property.
        /// </summary>
        /// <param name="obj">Sender Object.</param>
        /// <param name="value">The value.</param>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        protected static void SetContextMenuItems(DependencyObject obj, ObservableCollection<object> value)
        {
            obj.SetValue(ContextMenuItemsPropertyKey, value);

            if (obj is TabItemExt)
            {
                TabItemExt itemExt = (TabItemExt)obj;
                itemExt.SetValue(TabItemExt.ContextMenuItemsPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets the value of the HoverBackground dependency property.
        /// </summary>
        /// <param name="obj">Sender Object.</param>
        /// <returns>returns a Brush</returns>
        public static Brush GetHoverBackground(DependencyObject obj)
        {
            return (Brush)obj.GetValue(HoverBackgroundProperty);
        }

        /// <summary>
        /// Sets the value of the HoverBackground dependency property.
        /// </summary>
        /// <param name="obj">Sender Object.</param>
        /// <param name="value">The value.</param>
        public static void SetHoverBackground(DependencyObject obj, Brush value)
        {
            obj.SetValue(HoverBackgroundProperty, value);
        }

        /// <summary>
        /// Gets the value of the ImageAlignment dependency property.
        /// </summary>
        /// <param name="obj">Sender Object.</param>
        /// <returns>returns a ImageAlignment</returns>
        public static ImageAlignment GetImageAlignment(DependencyObject obj)
        {
            return (ImageAlignment)obj.GetValue(ImageAlignmentProperty);
        }

        /// <summary>
        /// Sets the value of the ImageAlignment dependency property.
        /// </summary>
        /// <param name="obj">Sender Object.</param>
        /// <param name="value">The value.</param>
        public static void SetImageAlignment(DependencyObject obj, ImageAlignment value)
        {
            obj.SetValue(ImageAlignmentProperty, value);
        }

        /// <summary>
        /// Gets the value of the Image dependency property.
        /// </summary>
        /// <param name="obj">Sender Object.</param>
        /// <returns>returns an ImageSource</returns>
        public static ImageSource GetImage(DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(ImageProperty);
        }

        /// <summary>
        /// Sets the value of the Image dependency property.
        /// </summary>
        /// <param name="obj">Sender Object.</param>
        /// <param name="value">The value.</param>
        public static void SetImage(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(ImageProperty, value);
        }

        /// <summary>
        /// Gets the menu icon.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static ImageSource GetMenuIcon(DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(MenuIconProperty);
        }

        /// <summary>
        /// Sets the menu icon.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="value">The value.</param>
        public static void SetMenuIcon(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(MenuIconProperty, value);
        }
        /// <summary>
        /// Gets the value of the ImageHeight dependency property.
        /// </summary>
        /// <param name="obj">Sender Object.</param>
        /// <returns>returns a double value</returns>
        public static double GetImageHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(ImageHeightProperty);
        }

        /// <summary>
        /// Sets the value of the ImageHeight dependency property.
        /// </summary>
        /// <param name="obj">Sender Object.</param>
        /// <param name="value">The value.</param>
        public static void SetImageHeight(DependencyObject obj, double value)
        {
            obj.SetValue(ImageHeightProperty, value);
        }

        /// <summary>
        /// Gets the value of the ImageWidth dependency property.
        /// </summary>
        /// <param name="obj">Sender Object.</param>
        /// <returns>returns a double value</returns>
        public static double GetImageWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(ImageWidthProperty);
        }

        /// <summary>
        /// Sets the value of the ImageWidth dependency property.
        /// </summary>
        /// <param name="obj">Sender Object.</param>
        /// <param name="value">The value.</param>
        public static void SetImageWidth(DependencyObject obj, double value)
        {
            obj.SetValue(ImageWidthProperty, value);
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets default style of the DragMarker. 
        /// </summary>
        public Style DefaultDragMarkerStyle
        {
            get
            {
                ResourceDictionary dictionary = new ResourceDictionary
                {
                    Source = new Uri(m_dragMarkerStyle, UriKind.RelativeOrAbsolute)
                };

                return (Style)dictionary[m_dragMarkerStyleName];
            }
        }

        /// <summary>
        /// Gets or Sets the Value of NewButtonAlignment Dependency Property
        /// </summary>
        public NewButtonAlignment NewButtonAlignment
        {
            get
            {
                return (NewButtonAlignment)GetValue(NewButtonAlignmentProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(NewButtonAlignmentProperty, value);
#else
                SetValue(NewButtonAlignmentProperty, value);
#endif
            }

        }

        /// <summary>
        /// Gets or Sets the value of NewButtonTemplate Dependency property
        /// </summary>
        public ControlTemplate NewButtonTemplate
        {
            get
            {
                return (ControlTemplate)GetValue(NewButtonTemplateProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(NewButtonTemplateProperty, value);
#else
                SetValue(NewButtonTemplateProperty, value);
#endif

            }
        }

        /// <summary>
        /// Gets or sets the value of NewButtonBackground Dependency Property.
        /// </summary>
        public Brush NewButtonBackground
        {
            get
            {
                return (Brush)GetValue(NewButtonBackgroundProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(NewButtonBackgroundProperty, value);
#else
                SetValue(NewButtonBackgroundProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or Sets the value of NewButtonBorderThickness Dependency Property.
        /// </summary>
        public Thickness NewButtonBorderThickness
        {
            get
            {
                return (Thickness)GetValue(NewButtonBorderThicknessProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(NewButtonBorderThicknessProperty, value);
#else
                SetValue(NewButtonBorderThicknessProperty, value);
#endif

            }
        }


        /// <summary>
        /// Gets or Sets the value of NewButtonStyle Dependency property
        /// </summary>
        public Style NewButtonStyle
        {
            get
            {
                return (Style)GetValue(NewButtonStyleProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(NewButtonStyleProperty, value);
#else
                SetValue(NewButtonStyleProperty, value);
#endif
            }
        }

        public Brush TabItemSelectedBackground
        {
            get
            {
                return (Brush)GetValue(TabItemSelectedBackgroundProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(TabItemSelectedBackgroundProperty, value);
#else
                SetValue(TabItemSelectedBackgroundProperty, value);
#endif
            }
        }

        public Brush TabItemSelectedForeground
        {
            get
            {
                return (Brush)GetValue(TabItemSelectedForegroundProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(TabItemSelectedForegroundProperty, value);
#else
                SetValue(TabItemSelectedForegroundProperty, value);
#endif
            }
        }

        public Brush TabItemSelectedBorderBrush
        {
            get
            {
                return (Brush)GetValue(TabItemSelectedBorderBrushProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(TabItemSelectedBorderBrushProperty, value);
#else
                SetValue(TabItemSelectedBorderBrushProperty, value);
#endif
            }
        }

        public Brush TabItemHoverBackground
        {
            get
            {
                return (Brush)GetValue(TabItemHoverBackgroundProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(TabItemHoverBackgroundProperty, value);
#else
                SetValue(TabItemHoverBackgroundProperty, value);
#endif
            }
        }

        public Brush TabItemHoverForeground
        {
            get
            {
                return (Brush)GetValue(TabItemHoverForegroundProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(TabItemHoverForegroundProperty, value);
#else
                SetValue(TabItemHoverForegroundProperty, value);
#endif
            }
        }

        public Brush TabItemHoverBorderBrush
        {
            get
            {
                return (Brush)GetValue(TabItemHoverBorderBrushProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(TabItemHoverBorderBrushProperty, value);
#else
                SetValue(TabItemHoverBorderBrushProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the value of the TabListContextMenuItemTemplate dependency property.
        /// </summary>
        public DataTemplate TabListContextMenuItemTemplate
        {
            get
            {
                return (DataTemplate)GetValue(TabListContextMenuItemTemplateProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(TabListContextMenuItemTemplateProperty, value);
#else
                SetValue(TabListContextMenuItemTemplateProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the tab list context menu style.
        /// </summary>
        /// <value>The tab list context menu style.</value>
        public Style TabListContextMenuStyle
        {
            get
            {
                return (Style)GetValue(TabListContextMenuStyleProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(TabListContextMenuStyleProperty, value);
#else
                SetValue(TabListContextMenuStyleProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the tab list context menu template.
        /// </summary>
        /// <value>The tab list context menu template.</value>
        public ControlTemplate TabListContextMenuTemplate
        {
            get
            {
                return (ControlTemplate)GetValue(TabListContextMenuTemplateProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(TabListContextMenuTemplateProperty, value);
#else
                SetValue(TabListContextMenuTemplateProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the tab list context menu item style.
        /// </summary>
        /// <value>The tab list context menu item style.</value>
        public Style TabListContextMenuItemStyle
        {
            get
            {
                return (Style)GetValue(TabListContextMenuItemStyleProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(TabListContextMenuItemStyleProperty, value);
#else
                SetValue(TabListContextMenuItemStyleProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets tabs scrolling time in milliseconds.
        /// </summary>        
        public int ScrollingTime
        {
            get
            {
                return (int)GetValue(ScrollingTimeProperty);
            }

            set
            {

                SetValue(ScrollingTimeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the DragMarkerStyle is true.
        /// </summary>
        public bool IsAllTabsClosed
        {
            get
            {
                return (bool)GetValue(IsAllTabsClosedProperty);
            }

            protected internal set
            {
                SetValue(IsAllTabsClosedPropertyKey, value);
            }
        }




        public CloseMode CloseMode
        {
            get { return (CloseMode)GetValue(CloseModeProperty); }
            set { SetValue(CloseModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CloseMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CloseModeProperty =
            DependencyProperty.Register("CloseMode", typeof(CloseMode), typeof(TabControlExt), new UIPropertyMetadata(CloseMode.Delete));


        /// <summary>
        /// Gets or sets the value of the DragMarkerStyle dependency property.
        /// </summary>
        public Style DragMarkerStyle
        {
            get
            {
                return (Style)GetValue(DragMarkerStyleProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(DragMarkerStyleProperty, value);
#else
                SetValue(DragMarkerStyleProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the value of the FullScreenMode dependency property.
        /// </summary>
        public FullScreenMode FullScreenMode
        {
            get
            {
                return (FullScreenMode)GetValue(FullScreenModeProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(FullScreenModeProperty, value);
#else
                SetValue(FullScreenModeProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the edit header item style.
        /// </summary>
        /// <value>The edit header item style.</value>
        public Style EditHeaderItemStyle
        {
            get
            {
                return (Style)GetValue(EditHeaderItemStyleProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(EditHeaderItemStyleProperty, value);
#else
                SetValue(EditHeaderItemStyleProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the value of the ToolBarTray dependency property.
        /// </summary>
        public ToolBarTray ToolBarTray
        {
            get
            {
                return (ToolBarTray)GetValue(ToolBarTrayProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(ToolBarTrayProperty, value);
#else
                SetValue(ToolBarTrayProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the value of the DragMarkerColor dependency property.
        /// </summary>
        public Brush DragMarkerColor
        {
            get
            {
                return (Brush)GetValue(DragMarkerColorProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(DragMarkerColorProperty, value);
#else
                SetValue(DragMarkerColorProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the tab panel background.
        /// </summary>
        /// <value>The tab panel background.</value>
        public Brush TabPanelBackground
        {
            get
            {
                return (Brush)GetValue(TabPanelBackgroundProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(TabPanelBackgroundProperty, value);
#else
                SetValue(TabPanelBackgroundProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the EnableLabelEdit is true.
        /// </summary>
        public bool EnableLabelEdit
        {
            get
            {
                return (bool)GetValue(EnableLabelEditProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(EnableLabelEditProperty, value);
#else
                SetValue(EnableLabelEditProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the value of the TabPanelItem dependency property.
        /// </summary>
        public object TabPanelItem
        {
            get
            {
                return GetValue(TabPanelItemProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(TabPanelItemProperty, value);
#else
                SetValue(TabPanelItemProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is disable unload tab item ext content.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is disable unload tab item ext content; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisableUnloadTabItemExtContent
        {
            get
            {
                return (bool)GetValue(IsDisableUnloadTabItemExtContentProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(IsDisableUnloadTabItemExtContentProperty, value);
#else
                SetValue(IsDisableUnloadTabItemExtContentProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is focus.
        /// </summary>
        /// <value><c>true</c> if this instance is focus; otherwise, <c>false</c>.</value>
        internal bool IsFocus
        {
            get
            {
                return (bool)GetValue(IsFocusProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(IsFocusProperty, value);
#else
                SetValue(IsFocusProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is tab group focus.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is tab group focus; otherwise, <c>false</c>.
        /// </value>
        internal bool IsTabGroupFocus
        {
            get
            {
                return (bool)GetValue(IsTabGroupFocusProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(IsTabGroupFocusProperty, value);
#else
                SetValue(IsTabGroupFocusProperty, value);
#endif
            }
        }

        public bool SelectOnCreatingNewItem
        {
            get
            {
                return (bool)GetValue(SelectOnCreatingNewItemProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(SelectOnCreatingNewItemProperty, value);
#else
                SetValue(SelectOnCreatingNewItemProperty, value);
#endif
            }
        }





        /// <summary>
        /// Gets or sets a value indicating whether the ShowTabListContextMenu is true.
        /// </summary>
        public bool ShowTabListContextMenu
        {
            get
            {
                return (bool)GetValue(ShowTabListContextMenuProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(ShowTabListContextMenuProperty, value);
#else
                SetValue(ShowTabListContextMenuProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the ShowTabItemContextMenu is true.
        /// </summary>
        public bool ShowTabItemContextMenu
        {
            get
            {
                return (bool)GetValue(ShowTabItemContextMenuProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(ShowTabItemContextMenuProperty, value);
#else
                SetValue(ShowTabItemContextMenuProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the value of the CloseButtonType dependency property.
        /// </summary>
        public CloseButtonType CloseButtonType
        {
            get
            {
                return (CloseButtonType)GetValue(CloseButtonTypeProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(CloseButtonTypeProperty, value);
#else
                SetValue(CloseButtonTypeProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the HotTrackingEnabled is true.
        /// </summary>
        public bool HotTrackingEnabled
        {
            get
            {
                return (bool)GetValue(HotTrackingEnabledProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(HotTrackingEnabledProperty, value);
#else
                SetValue(HotTrackingEnabledProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the RotateTextWhenVertical is true.
        /// </summary>
        public bool RotateTextWhenVertical
        {
            get
            {
                return (bool)GetValue(RotateTextWhenVerticalProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(RotateTextWhenVerticalProperty, value);
#else
                SetValue(RotateTextWhenVerticalProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the value of the TabPanelTemplate dependency property.
        /// </summary>
        public ControlTemplate TabPanelTemplate
        {
            get
            {
                return (ControlTemplate)GetValue(TabPanelTemplateProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(TabPanelTemplateProperty, value);
#else
                SetValue(TabPanelTemplateProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the value of the TabPanelStyle dependency property.
        /// </summary>
        public Style TabPanelStyle
        {
            get
            {
                return (Style)GetValue(TabPanelStyleProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(TabPanelStyleProperty, value);
#else
                SetValue(TabPanelStyleProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the value of the TabScrollButtonVisibility dependency property.
        /// </summary>
        public TabScrollButtonVisibility TabScrollButtonVisibility
        {
            get
            {
                return (TabScrollButtonVisibility)GetValue(TabScrollButtonVisibilityProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(TabScrollButtonVisibilityProperty, value);
#else
                SetValue(TabScrollButtonVisibilityProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the value of the TabScrollStyle dependency property.
        /// </summary>
        public TabScrollStyle TabScrollStyle
        {
            get
            {
                return (TabScrollStyle)GetValue(TabScrollStyleProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(TabScrollStyleProperty, value);
#else
                SetValue(TabScrollStyleProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the value of the TabItemLayout dependency property.
        /// </summary>
        public TabItemLayoutType TabItemLayout
        {
            get
            {
                return (TabItemLayoutType)GetValue(TabItemLayoutProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(TabItemLayoutProperty, value);
#else
                SetValue(TabItemLayoutProperty, value);
#endif
            }
        }

        internal bool IsTouchEnabled
        {
            get { return (bool)GetValue(IsTouchEnabledProperty); }
            set { SetValue(IsTouchEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value of the TabItemSize dependency property.
        /// </summary>
        public TabItemSizeMode TabItemSize
        {
            get
            {
                return (TabItemSizeMode)GetValue(TabItemSizeProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(TabItemSizeProperty, value);
#else
                SetValue(TabItemSizeProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the KeepTabInFront is true.
        /// </summary>
        public bool KeepTabInFront
        {
            get
            {
                return (bool)GetValue(KeepTabInFrontProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(KeepTabInFrontProperty, value);
#else
                SetValue(KeepTabInFrontProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the name of the menu item close tabs.
        /// </summary>
        /// <value>The name of the menu item close tabs.</value>
        public string MenuItemCloseTabsName
        {
            get
            {
                return (string)GetValue(MenuItemCloseTabsNameProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(MenuItemCloseTabsNameProperty, value);
#else
                SetValue(MenuItemCloseTabsNameProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the name of the menu item close other tabs.
        /// </summary>
        /// <value>The name of the menu item close other tabs.</value>
        public string MenuItemCloseOtherTabsName
        {
            get
            {
                return (string)GetValue(MenuItemCloseOtherTabsNameProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(MenuItemCloseOtherTabsNameProperty, value);
#else
                SetValue(MenuItemCloseOtherTabsNameProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [disable resize on selection].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [disable resize on selection]; otherwise, <c>false</c>.
        /// </value>
        public bool DisableResizeOnSelection
        {
            get
            {
                return (bool)GetValue(DisableResizeOnSelectionProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(DisableResizeOnSelectionProperty, value);
#else
                SetValue(DisableResizeOnSelectionProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the name of the menu item close all tabs.
        /// </summary>
        /// <value>The name of the menu item close all tabs.</value>
        public string MenuItemCloseAllTabsName
        {
            get
            {
                return (string)GetValue(MenuItemCloseAllTabsNameProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(MenuItemCloseAllTabsNameProperty, value);
#else
                SetValue(MenuItemCloseAllTabsNameProperty, value);
#endif

            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the IsCustomTabItemContextMenuEnabled is true.
        /// </summary>
        public bool IsCustomTabItemContextMenuEnabled
        {
            get
            {
                return (bool)GetValue(IsCustomTabItemContextMenuEnabledProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(IsCustomTabItemContextMenuEnabledProperty, value);
#else
                SetValue(IsCustomTabItemContextMenuEnabledProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [collapse default tab list context menu items].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [collapse default tab list context menu items]; otherwise, <c>false</c>.
        /// </value>
        public bool CollapseDefaultTabListContextMenuItems
        {
            get
            {
                return (bool)GetValue(CollapseDefaultTabListContextMenuItemsProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(CollapseDefaultTabListContextMenuItemsProperty, value);
#else
                SetValue(CollapseDefaultTabListContextMenuItemsProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the tab list context menu items.
        /// </summary>
        /// <value>The tab list context menu items.</value>
        public DocumentTabItemMenuItemCollection TabListContextMenuItems
        {
            get
            {
                return (DocumentTabItemMenuItemCollection)GetValue(TabListContextMenuItemsProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(TabListContextMenuItemsProperty, value);
#else
                SetValue(TabListContextMenuItemsProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the value of the SelectedItemFontWeight dependency property.
        /// </summary>
        public FontWeight SelectedItemFontWeight
        {
            get
            {
                return (FontWeight)GetValue(SelectedItemFontWeightProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(SelectedItemFontWeightProperty, value);
#else
                SetValue(SelectedItemFontWeightProperty, value);
#endif
            }
        }


        internal TabItemExt m_TabItem { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether AllowDragDrop is true.
        /// </summary>
        public bool AllowDragDrop
        {
            get
            {
                return (bool)GetValue(AllowDragDropProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(AllowDragDropProperty, value);
#else
                SetValue(AllowDragDropProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets activated tab item.
        /// </summary>
        public TabItemExt ActivatedItem
        {
            get
            {
                return m_activatedItem;
            }

            set
            {
                m_activatedItem = value;
            }
        }

        /// <summary>
        /// Gets or Sets IsNewTabEnabled which indicates whether to create new tab is displayed or not
        /// </summary>
        public bool IsNewButtonEnabled
        {
            get
            {
                return (bool)GetValue(IsNewButtonEnabledProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(IsNewButtonEnabledProperty, value);
#else
                SetValue(IsNewButtonEnabledProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or Sets IsNewButtonClosedonNoChild which indicates whether to close new tab or not when no child in Tabcontrol
        /// </summary>
        public bool IsNewButtonClosedonNoChild
        {
            get
            {
                return (bool)GetValue(IsNewButtonClosedonNoChildProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(IsNewButtonClosedonNoChildProperty, value);
#else
                SetValue(IsNewButtonClosedonNoChildProperty, value);
#endif
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is dragging.
        /// </summary>
        /// <value>
        ///    <c>true</c> if this instance is dragging; otherwise, <c>false</c>.
        /// </value>
        protected internal bool IsDragging
        {
            get
            {
                return bIsDragging;
            }

            set
            {
                bIsDragging = value;
            }
        }



        /// <summary>
        /// Gets or sets a value indicating whether IsCloseTabProcessEnabled is true.
        /// </summary>
        internal bool IsCloseTabProcessEnabled
        {
            get
            {
                return (bool)GetValue(IsCloseTabProcessEnabledProperty);
            }

            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(IsCloseTabProcessEnabledProperty, value);
#else
                SetValue(IsCloseTabProcessEnabledProperty, value);
#endif
            }
        }
        /// <summary>
        /// Gets or sets the value of the ScrollingButtonStyle dependency property.
        /// </summary>
        public Style ScrollingButtonStyle
        {
            get
            {
                return (Style)GetValue(ScrollingButtonStyleProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(ScrollingButtonStyleProperty, value);
#else
                SetValue(ScrollingButtonStyleProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets the tab layout panel.
        /// </summary>
        /// <value>The tab layout panel.</value>
        internal TabLayoutPanel TabLayoutPanel
        {
            get
            {
                if (TabPanel != null)
                {
                    TabLayoutPanel m_panel = (TabLayoutPanel)GetTemplateChild("PART_TabLayoutPanel");
                    m_tabLayoutPanel = m_panel != null ? m_panel : m_tabLayoutPanel;
                }

                return m_tabLayoutPanel;
            }
        }

        internal TabPanelAdv paneladv = null;

        /// <summary>
        /// Occurs when [new button click].
        /// </summary>
        public event EventHandler NewButtonClick;

        /// <summary>
        /// internal variable which has new tab flag
        /// </summary>
        bool m_newtabflag = false;

        /// <summary>
        /// internal variable which has new item
        /// </summary>
        object m_addedItem = null;

        /// <summary>
        /// Invokes the new tab item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public object InvokeNewTabItem(object sender, EventArgs args)
        {
           if (NewButtonClick != null)
            {
                m_newtabflag = true;
                NewButtonClick(sender, args);
                m_newtabflag = false;
            }
            return m_addedItem;
        }

        /// <summary>
        /// Gets the tab panel.
        /// </summary>
        /// <value>The tab panel.</value>
        internal TabPanelAdv TabPanel
        {
            get
            {
                if (m_tabPanel == null)
                {
                    m_tabPanel = (TabPanelAdv)GetTemplateChild("PART_TabPanel");
                    TabItemExt ext = GetTemplateChild("PART_NewTab") as TabItemExt;
                }

                return m_tabPanel;
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="TabControlExt"/> class.
        /// </summary>
        static TabControlExt()
        {
            // This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            // This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabControlExt), new FrameworkPropertyMetadata(typeof(TabControlExt)));
            TabStripPlacementProperty.OverrideMetadata(typeof(TabControlExt), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnTabStripPlacementChanged)));
            FlowDirectionProperty.OverrideMetadata(typeof(TabControlExt), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnFlowDirectionChanged)));
            SelectedIndexProperty.OverrideMetadata(typeof(TabControlExt), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnSelectedIndexChanged)));
                       
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TabControlExt"/> class.
        /// </summary>
        public TabControlExt()
        {

            if (EnvironmentTestTools.IsSecurityGranted)
            {
                EnvironmentTestTools.StartValidateLicense(typeof(TabControlExt));
            }

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                try
                {
                    if (Application.Current != null && !BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                        System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
                }
                catch (InvalidOperationException)
                {
                    if (Application.Current != null && Application.Current.Dispatcher != null)
                    {
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(
                            () => {
                                if (Application.Current != null && !BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                                    System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
                            }));
                    }
                }
            }
            m_CloseTabCommandBinding = new CommandBinding(TabControlCommands.CloseTabItem, ProcessCloseTabItemCommand, CanProcessCloseTabItemCommand);
            m_CloseCurrentTabCommandBinding = new CommandBinding(TabControlCommands.CloseCurrentTabItem, ProcessCloseCurrentTabItemCommand, CanProcessCloseCurrentTabItemCommand);
            m_CloseTabsCommandBinding = new CommandBinding(TabControlCommands.CloseTabs, ProcessCloseTabsCommand, CanProcessCloseTabsCommand);

            if (IsCloseTabProcessEnabled)
            {
                CommandBindings.Add(m_CloseTabCommandBinding);
                CommandBindings.Add(m_CloseCurrentTabCommandBinding);
                CommandBindings.Add(m_CloseTabsCommandBinding);
            }


            
            this.MenuItemCloseTabsName = wrapper.TabClose; 
            this.MenuItemCloseOtherTabsName = wrapper.CloseAllButThis;  
            this.MenuItemCloseAllTabsName = wrapper.TabCloseAll;  

            CommandBinding openContextMenuCommandBinding = new CommandBinding(TabControlCommands.OpenContextMenu, ProcessOpenContextMenu, CanProcessOpenContextMenu);
            CommandBindings.Add(openContextMenuCommandBinding);
            this.Loaded += new RoutedEventHandler(TabControlExt_Loaded);
            this.ItemContainerGenerator.StatusChanged += new EventHandler(ItemContainerGenerator_StatusChanged);
            this.LayoutUpdated += new EventHandler(TabControlExt_LayoutUpdated);
            this.Unloaded += new RoutedEventHandler(TabControlExt_Unloaded);
        }

        /// <summary>
        /// Handles the Unloaded event of the TabControlExt control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void TabControlExt_Unloaded(object sender, RoutedEventArgs e)
        {
            this.Unloaded -= new RoutedEventHandler(TabControlExt_Unloaded);
            ActivatedItem = null;
            if (this.TabPanel != null && TabPanel.Template != null)
            {
                ToggleButton closebutton = TabPanel.Template.FindName("PART_CloseButton", TabPanel) as ToggleButton;
                if (closebutton != null)
                {
                    closebutton.MouseEnter -= new MouseEventHandler(closebutton_MouseEnter);
#if !SyncfusionFramework3_5
                    //closebutton.TouchEnter -= closebutton_TouchEnter;
#endif
                }
            }
        }

        bool m_SkinChangeFlag = false;


        #region IDisposable Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Loaded -= new RoutedEventHandler(TabControlExt_Loaded);
            this.ItemContainerGenerator.StatusChanged -= new EventHandler(ItemContainerGenerator_StatusChanged);
            this.LayoutUpdated -= new EventHandler(TabControlExt_LayoutUpdated);
            this.Unloaded -= new RoutedEventHandler(TabControlExt_Unloaded);
            
            ItemContainerGenerator.StatusChanged -= new EventHandler(OnGeneratorStatusChanged);

            ActivatedItem = null;
            selectionStack.Clear();
            LocalValueEnumerator locallySetProperties = this.GetLocalValueEnumerator();
            while (locallySetProperties.MoveNext())
            {
                DependencyProperty propertyToClear = locallySetProperties.Current.Property;
                if (propertyToClear.Name == "SelectedItem")
                {
                    this.ClearValue(propertyToClear);
                }
            }
            if (previewselectedargs != null)
            {
                previewselectedargs.OldSelectedItem = null;
                previewselectedargs.NewSelectedItem = null;
            }
            if (SelectedContentPresenter != null)
            {
                this.SelectedContentPresenter.Content = null;
            }
            if (this.TabPanel != null && TabPanel.Template != null)
            {
                ToggleButton closebutton = TabPanel.Template.FindName("PART_CloseButton", TabPanel) as ToggleButton;
                if (closebutton != null)
                {
                    closebutton.MouseEnter -= new MouseEventHandler(closebutton_MouseEnter);
#if !SyncfusionFramework3_5
                    //closebutton.TouchEnter -= closebutton_TouchEnter;
#endif
                }
            }
            Border3D border3d = GetTemplateChild("ContentPanel") as Border3D;
            if (border3d != null)
            {
                border3d.MouseMove -= new MouseEventHandler(border3d_MouseMove);
#if !SyncfusionFramework3_5
                //if (this.IsTouchEnabled)
                //border3d.TouchMove -= border3d_TouchMove;
#endif
            }
            if (m_listMenuButton != null)
            {
                m_listMenuButton.PreviewMouseRightButtonDown -= new MouseButtonEventHandler(M_ListMenuButton_PreviewMouseRightButtonDown);
#if !SyncfusionFramework3_5
                //m_listMenuButton.PreviewTouchMove -= m_listMenuButton_PreviewTouchMove;
#endif
            }

        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Dispose();
        }

        #endregion


        /// <summary>
        /// Handles the LayoutUpdated event of the TabControlExt control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void TabControlExt_LayoutUpdated(object sender, EventArgs e)
        {
            if (IsDisableUnloadTabItemExtContent)
            {
                if (m_ContentPanel == null)
                {
                    m_ContentPanel = GetTemplateChild("PART_ContentHolder") as Panel;
                    UpdateSelectedItem();
                }
                if (m_SkinChangeFlag)
                {
                    UpdateSelectedItem();
                    m_SkinChangeFlag = false;
                }
            }
        }


        /// <summary>
        /// Handles the StatusChanged event of the ItemContainerGenerator control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (this.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                this.ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
                UpdateSelectedItem();
            }
        }
        private Panel m_ContentPanel = null;

        /// <summary>
        /// Updates the selected item.
        /// </summary>
        private void UpdateSelectedItem()
        {
            if (m_ContentPanel == null)
            {
                return;
            }

            TabItemExt item = GetSelectedTabItem();
            if (item != null)
            {
                CreateChildContentPresenter(item);
            }

            foreach (ContentPresenter child in m_ContentPanel.Children)
            {
                child.Visibility = ((child.Tag as TabItem).IsSelected) ? Visibility.Visible : Visibility.Collapsed;
            }
         
        }

        /// <summary>
        /// Creates the child content presenter.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private ContentPresenter CreateChildContentPresenter(object item)
        {
            if (item == null)
            {
                return null;
            }

            ContentPresenter contentPresenter = FindChildContentPresenter(item);

            if (contentPresenter != null)
            {
                return contentPresenter;
            }

            contentPresenter = new ContentPresenter();
            contentPresenter.Content = (item is TabItemExt) ? (item as TabItemExt).Content : item;
            contentPresenter.ContentTemplate = this.SelectedContentTemplate;
            contentPresenter.ContentTemplateSelector = this.SelectedContentTemplateSelector;
            contentPresenter.ContentStringFormat = this.SelectedContentStringFormat;
            if (item is TabItemExt && this.SelectedContentTemplate == null &&
                this.SelectedContentTemplateSelector == null && this.SelectedContentStringFormat == null)
            {
                contentPresenter.ContentTemplate = (item as TabItemExt).ContentTemplate;
                contentPresenter.ContentTemplateSelector = (item as TabItemExt).ContentTemplateSelector;
                contentPresenter.ContentStringFormat = (item as TabItemExt).ContentStringFormat;
            }
            contentPresenter.Visibility = Visibility.Collapsed;
            contentPresenter.Tag = (item is TabItem) ? item : (this.ItemContainerGenerator.ContainerFromItem(item));
            m_ContentPanel.Children.Add(contentPresenter);
            return contentPresenter;
        }

        /// <summary>
        /// Finds the child content presenter.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private ContentPresenter FindChildContentPresenter(object data)
        {
            if (data is TabItem)
            {
                data = (data as TabItem).Content;
            }

            if (data == null)
            {
                return null;
            }

            if (m_ContentPanel == null)
            {
                return null;
            }

            foreach (ContentPresenter contentPresenter in m_ContentPanel.Children)
            {
                if (contentPresenter.Content == data)
                {
                    return contentPresenter;
                }
            }
            return null;
        }
        /// <summary>
        /// Handles the Loaded event of the TabControlExt control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void TabControlExt_Loaded(object sender, RoutedEventArgs e)
        {
            if (!SelectionStack.Contains(SelectedItem))
            {
                SelectionStack.Add(SelectedItem);
            }
            this.Loaded -= new RoutedEventHandler(TabControlExt_Loaded);
            if (Items != null && Items.Count == 0 && TabPanel != null && ((!IsNewButtonEnabled) || (IsNewButtonEnabled && IsNewButtonClosedonNoChild)))
            {                
                TabPanel.Visibility = Visibility.Collapsed;
            }
            if (TabPanel != null && TabPanel.Template!=null)
            {
                ToggleButton closebutton = TabPanel.Template.FindName("PART_CloseButton", TabPanel) as ToggleButton;
                if (closebutton != null)
                {
                    closebutton.MouseEnter += new MouseEventHandler(closebutton_MouseEnter);
#if !SyncfusionFramework3_5
                    //if(this.IsTouchEnabled)
                        //closebutton.TouchEnter += closebutton_TouchEnter;
#endif
                }
            }
			VerifyZIndex();
            UpdateIndex();
            m_loaded = true;
        }

        

        private void UpdateIndex()
        {
            List<TabItemExt> TabIndexList = new List<TabItemExt>();

            for (int i = 0; i < Items.Count; i++)
            {
                TabItemExt tabitem = GetTabItem(Items[i]);
                if (tabitem != null)
                {
                    TabIndexList.Add(tabitem);
                }
            }

            TabIndexList.Sort((a, b) => a.TabIndex.CompareTo(b.TabIndex));
            
            for (int i = 0; i < TabIndexList.Count; i++)
            {
                TabItemExt item = (TabItemExt)TabIndexList[i];
                if (item.TabIndex > -1 && item.TabIndex !=i)
                {
                    UpdateIndex(item.TabIndex, item);
                }
            }
            SetIndex();
        }

        internal void SetIndex()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                TabItemExt item = GetTabItem(Items[i]);;
                if (item != null)
                {
                    item.TabIndex = i;
                }
            }
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Launch LabelEdit operation on the specified item.
        /// </summary>
        /// <param name="item">Item which header will be editing.</param>
        public void LabelEditStart(TabItemExt item)
        {
            if (TabLayoutPanel != null)
            {
                if (item != null && EnableLabelEdit && Items.Contains(item))
                {
                    TabLayoutPanel.LabelEditStartInternal(item);
                }
            }
        }

        /// <summary>
        /// Complete editing process on the specifies TabItemExt.
        /// </summary>
        /// <param name="editableItem">TabItemExt which is editing in the current moment.</param>
        /// <param name="applyChanges">Specifies whether editing changes should be applied or no.</param>
        public void CompleteHeaderEdit(TabItemExt editableItem, bool applyChanges)
        {
            if (TabLayoutPanel != null)
            {
                if (editableItem != null && EnableLabelEdit && Items.Contains(editableItem) && TabControlExt.GetIsEditing(editableItem))
                {
                    TabLayoutPanel.CompleteHeaderEditInternal(editableItem, applyChanges);
                }
            }
        }

        /// <summary>
        /// Saves the state persisted for the current <see cref="TabControlExt"/> location.
        /// </summary>
        /// <param name="mode">Value of the mode.</param>
        public void SaveTabState(SaveMode mode)
        {
            if (mode == SaveMode.IsolatedSorage)
            {
                IsolatedStorageFile isoStorage = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
                SaveTabState(isoStorage, m_StoreFileName);
            }
            else
            {
                SaveToRegistry();
            }
        }

        /// <summary>
        /// Loads the state persisted.
        /// </summary>
        /// <param name="mode">Value of the mode.</param>
        public void LoadTabState(SaveMode mode)
        {
            if (mode == SaveMode.IsolatedSorage)
            {
                IsolatedStorageFile isoStorage = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
                LoadTabStateFromIsoStorage(isoStorage, m_StoreFileName);
            }
            else
            {
                TabControlSettings settings = LoadFromRegistry();
                ApplyState(settings);
            }
        }
        #endregion

        #region public properties


        /// <summary>
        /// Gets the selected content presenter.
        /// </summary>
        /// <value>The selected content presenter.</value>
        internal ContentPresenter SelectedContentPresenter
        {
            get
            {
                return GetTemplateChild("PART_SelectedContentHost") as ContentPresenter;
            }
        }
        /// <summary>
        /// Hides the Header On Single Child
        /// </summary>
        public bool HideHeaderOnSingleChild
        {
            get
            {
                return (bool)GetValue(HideHeaderOnSingleChildProperty);
            }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(HideHeaderOnSingleChildProperty, value);
#else
                SetValue(HideHeaderOnSingleChildProperty, value);
#endif
            }
        }

        /// <summary>
        /// Gets or sets the default context menu item visibility.
        /// </summary>
        /// <value>The default context menu item visibility.</value>
        public Visibility DefaultContextMenuItemVisibility
        {
            get
            {
                return (Visibility)GetValue(DefaultContextMenuItemVisibilityProperty);
            }

            set
            {
                #if !SyncfusionFramework3_5
                SetCurrentValue(DefaultContextMenuItemVisibilityProperty, value);
#else
                SetValue(DefaultContextMenuItemVisibilityProperty, value);
#endif
            }
        }

        #endregion

        #region Implementation
        /// <summary>
        /// Gets the child.
        /// </summary>
        /// <param name="childName">Name of the child.</param>
        /// <returns>returns a DependencyObject</returns>
        internal DependencyObject GetChild(string childName)
        {
            return GetTemplateChild(childName);
        }

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="item">Value of the item.</param>
        internal virtual void AddItem(object item)
        {
            Items.Add(item);
       }

        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="item">Value of the item.</param>
        internal virtual void RemoveItem(object item)
        {
            Items.Remove(item);
        }

        /// <summary>
        /// Removes the item at.
        /// </summary>
        /// <param name="index">The index.</param>
        internal virtual void RemoveItemAt(int index)
        {
            Items.RemoveAt(index);
            
        }

        /// <summary>
        /// Inserts the item.
        /// </summary>
        /// <param name="index">Value of the index.</param>
        /// <param name="item">Value of the item.</param>
        internal virtual void InsertItem(int index, object item)
        {
            try
            {
                Items.Insert(index, item);
            }
            //SU I78477
            //catch (Exception e)
            catch (Exception)
                //EU I78477
            {
            }
        }

        /// <summary>
        /// Verifies the index of the Z.
        /// </summary>
        protected internal void VerifyZIndex()
        {
            int count = Items.Count - 1;
            int firstTabIndex;

            for (firstTabIndex = 0; firstTabIndex <= count; firstTabIndex++)
            {
                TabItemExt element = GetTabItem(Items[firstTabIndex]);

                if (element != null && element.Visibility == Visibility.Visible)
                {
                    break;
                }
            }

            if (!RotateTextWhenVertical)
            {
                if (TabStripPlacement == Dock.Top || TabStripPlacement == Dock.Right)
                {
                    for (int i = 0; i <= count; i++)
                    {
                        TabItemExt element = GetTabItem(Items[i]);

                        if (element != null)
                        {
                            if (i == SelectedIndex)
                            {
                                Panel.SetZIndex(element, 10000);
                            }
                            else
                            {
                                Panel.SetZIndex(element, (count - i));
                            }

                            if (i == firstTabIndex)
                            {
                                Panel.SetZIndex(element, 9999);
                            }
                        }
                    }
                }
                else
                {
                    int counter = 0;
                    for (int i = count; i >= 0; i--)
                    {                        
                        TabItemExt element = GetTabItem(Items[i]);

                        if (TabVisualStyle == TabVisualStyle.None)
                        {
                            if (element != null)
                            {
                                if (i == SelectedIndex)
                                {
                                    Panel.SetZIndex(element, 10000);
                                }
                                else
                                {
                                    Panel.SetZIndex(element, i);
                                }

                                if (i == count && (element.Visibility == Visibility.Visible))
                                {
                                    Panel.SetZIndex(element, 9999);
                                }
                            }
                        }
                        else
                        {
                            /* Logic for Excel Tab Style */                            
                            if (element != null)
                            {
                                if (i == SelectedIndex)
                                {
                                    Panel.SetZIndex(element, 10000 + count);
                                }
                                else
                                {
                                    Panel.SetZIndex(element, 10000 + counter);
                                }

                                if (i == count && (element.Visibility == Visibility.Visible) && i != SelectedIndex)
                                {
                                    Panel.SetZIndex(element, 9999);
                                }
                            }
                            counter++;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i <= count; i++)
                {
                    TabItemExt element = GetTabItem(Items[i]);

                    if (element != null)
                    {
                        if (element != SelectedItem)
                        {
                            if (SkinStorage.GetVisualStyle(this) != "Classic")
                            {
                                Panel.SetZIndex(element, 0);
                            }
                            else
                            {
                                Panel.SetZIndex(element, i);
                            }

                            if (i == count && (element.Visibility == Visibility.Visible))
                            {
                                Panel.SetZIndex(element, 9999);
                            }
                        }
                        else
                        {
                            if (SkinStorage.GetVisualStyle(this) != "Classic")
                            {
                                element.ClearValue(Panel.ZIndexProperty);
                            }
                            else
                            {
                                Panel.SetZIndex(element, 10000);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Preforms Measure process
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size constraint)
        {
            if (TabItemLayout == TabItemLayoutType.MultiLine || m_templayout)
            {
                CheckHiden();
            }
            return base.MeasureOverride(constraint);
        }

        /// <summary>
        /// internal variable which has  tab item layout type
        /// </summary>
        TabItemLayoutType temptype;

        /// <summary>
        /// internal variable which has temp layout
        /// </summary>
        bool m_templayout = false;

        /// <summary>
        /// used to check the number of hidden elements
        /// </summary>
        private void CheckHiden()
        {
            int count = TotalHidenItems();
            if (count == Items.Count - 1)
            {
                temptype = TabItemLayout;
                m_templayout = true;
            }
            else if (m_templayout)
            {
                TabItemLayout = temptype;
                m_templayout = false;
            }
        }

        /// <summary>
        /// Totals the hiden items.
        /// </summary>
        /// <returns></returns>
        private int TotalHidenItems()
        {
            int retValue = 0;
            foreach (object obj in Items)
            {
                if (obj is FrameworkElement)
                {
                    if (!(obj as FrameworkElement).IsVisible)
                    {
                        retValue++;
                    }
                }
            }
            return retValue;
        }

        /// <summary>
        /// Refresh measure and arrange. 
        /// </summary>
        private void RefreshLayout()
        {
            if (TabLayoutPanel != null)
            {
                TabLayoutPanel.InvalidateMeasure();
                TabLayoutPanel.InvalidateArrange();
            }
        }

        /// <summary>
        /// Close TabItemExt.
        /// </summary>
        /// <param name="item">TabItemExt that intend to be closed.</param>
        private void CloseTabItem(TabItemExt item)
        {
            if (item != null)
            {
               
                CloseTabEventArgs closeButtonEvent = new CloseTabEventArgs(item);
                FireOnCloseButtonClick(closeButtonEvent);
                if (!closeButtonEvent.Cancel && Items.Count > 0)
                {
                    int lastSelectedIndex = 0;
                    TabItemExt previousItem = null;
                    int itemIndex = ItemContainerGenerator.IndexFromContainer(item);
                    if (this.CloseMode == CloseMode.Hide)
                    {
                        item.Visibility = Visibility.Collapsed;
                    }
                    else if (this.CloseMode == CloseMode.Delete)
                    {
                        if (this.ItemsSource == null)
                        {
                            this.Items.Remove(item);
                        }
                        else
                        {
							var objSource = GetSourceObject(item);

							if (objSource == null) return;

							TabLayoutPanel.SourceItems.Remove(objSource);
                        }
                    }
                    SelectionStack.Remove(item);
                    for (int i = SelectionStack.Count - 1; i >= 0; i--)
                    {
                        previousItem = SelectionStack[i] as TabItemExt;
                        if (previousItem != null && previousItem != item && previousItem.IsVisible)
                        {
                            break;
                        }
                    }

                    lastSelected = previousItem;

                    if (lastSelected != null)
                    {
                        lastSelectedIndex = ItemContainerGenerator.IndexFromContainer(lastSelected);

                        if (lastSelectedIndex < 0)
                        {
                            lastSelectedIndex = itemIndex;
                        }
                      
                      }
                    else
                    {
                        if (itemIndex != lastSelectedIndex && itemIndex == 0)
                        {
                            lastSelectedIndex = itemIndex + 1;
                        }
                        else if (itemIndex != lastSelectedIndex)
                        {
                            lastSelectedIndex = itemIndex - 1;
                        }
                        else
                        {
                            lastSelectedIndex = itemIndex;
                        }
                       
                      }

                    if (itemIndex == SelectedIndex)
                    {
                        if (itemIndex < Items.Count + 1)
                        {
                            IsAllTabsClosed = false;

                            for (int i = lastSelectedIndex; i < Items.Count; i++)
                            {
                                if (GetTabItem(Items[i]).Visibility == Visibility.Visible)
                                {
                                    SelectedIndex = i;
                                    break;
                                }
                                else if (i != 0 && (GetTabItem(Items[i - 1]).Visibility == Visibility.Visible))
                                {
                                    SelectedIndex = i - 1;
                                    break;
                                }
                                else
                                {
                                    for (int j = lastSelectedIndex; j >= 0; j--)
                                    {
                                        if (j != 0 && (GetTabItem(Items[j - 1]).Visibility == Visibility.Visible))
                                        {
                                            SelectedIndex = j - 1;
                                            break;
                                        }
                                    }
                                }
                            }
							if (item == GetTabItem(Items[SelectedIndex]))
                            {
                                if (this.CloseMode != CloseMode.Hide)
                                {
                                    SelectedIndex = -1;
                                    IsAllTabsClosed = true;
                                    if (ItemsSource == null)
                                    {
                                        Items.Clear();
                                    }
                                }
                            }
                        }
                    }
                }

                if (Items.Count == 0)
                {
                    if (selectionStack.Count > 0)
                    {
                        selectionStack.Clear();
                    }
                    LocalValueEnumerator locallySetProperties = this.GetLocalValueEnumerator();
                    while (locallySetProperties.MoveNext())
                    {
                        DependencyProperty propertyToClear = locallySetProperties.Current.Property;
                        if (propertyToClear.Name == "SelectedItem")
                        {
                            this.ClearValue(propertyToClear);
                        }
                    }
                }
                if (previewselectedargs!=null && previewselectedargs.OldSelectedItem == item)
                {
                    previewselectedargs.OldSelectedItem = null;
                }
                if (previewselectedargs != null && previewselectedargs.NewSelectedItem == item)
                {
                    previewselectedargs.NewSelectedItem = null;
                }
                
                FireOnClosedTab(closeButtonEvent);
            }

            lastSelected = null;
            
        }

        /// <summary>
        /// Saves the state persisted for the current <see cref="TabControlExt"/> location.
        /// </summary>
        /// <param name="isoStorage">Reference in isolated storage for saving the current <see cref="TabControlExt"/> location.</param>
        /// <param name="storeFileName">File name for the isolated storage.</param>
        private void SaveTabState(IsolatedStorageFile isoStorage, string storeFileName)
        {
           
            if (null != isoStorage && !String.IsNullOrEmpty(storeFileName))
            {
                Stream stream = new IsolatedStorageFileStream(storeFileName, FileMode.Create, isoStorage);

                try
                {
                    TabControlSettings settings = new TabControlSettings(this);
                    XamlWriter.Save(settings, stream);
                }
                finally
                {
                    stream.Close();
                }
            }
        }

        /// <summary>
        /// Saves tab state to the registry.
        /// </summary>
        /// <example>
        /// 	<para/>This example shows how to use SaveTabState( BinaryFormatter Serializer ) in C#.
        /// <code language="C#">
        /// BinaryFormatter formatter1 = new BinaryFormatter();
        /// tabControl.SaveTabState( formatter1 );
        /// </code>
        /// </example>
        /// <seealso cref="BinaryFormatter"/>
        private void SaveToRegistry()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream memStream = new MemoryStream();
            TabControlSettings settings = new TabControlSettings(this);
            try
            {
                formatter.Serialize(memStream, settings);
            }
            catch (SerializationException ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }

            RegistryKey regKey = Registry.CurrentUser.CreateSubKey(C_regSubKeyName);
            Registry.SetValue(regKey.ToString(), C_regParamName, memStream.ToArray(), RegistryValueKind.Binary);
            memStream.Close();
        }

        /// <summary>
        /// Loads the state persisted.
        /// </summary>
        /// <param name="isoStorage">Reference in isolated storage for
        /// load current TabControl location.</param>
        /// <param name="storeFileName">Present file name for isolated
        /// storage.</param>
        private void LoadTabStateFromIsoStorage(IsolatedStorageFile isoStorage, string storeFileName)
        {
            if (null != isoStorage && !String.IsNullOrEmpty(storeFileName)
                && 0 < isoStorage.GetFileNames(storeFileName).Length)
            {
                Stream stream = new IsolatedStorageFileStream(storeFileName, FileMode.OpenOrCreate, isoStorage);
                TabControlSettings settings = null;

                try
                {
                    settings = XamlReader.Load(stream) as TabControlSettings;
                }
                catch (XamlParseException e)
                {
                    Debug.Print(e.Message);
                }

                ApplyState(settings);
            }
        }

        /// <summary>
        /// Applies the state.
        /// </summary>
        /// <param name="settings">The settings.</param>
        private void ApplyState(TabControlSettings settings)
        {
            Dictionary<string, object> tabItems = settings.Items;
            if (TabStripPlacement != settings.TabStripPlacement)
            {
                TabStripPlacement = settings.TabStripPlacement;
            }

            ArrayList changedItems = new ArrayList();

            foreach (TabItemExt item in Items)
            {
                ItemInfo info = (ItemInfo)tabItems[item.Name];
                item.Header = item.Header != info.Header ? info.Header : item.Header;
                item.Visibility = item.Visibility != info.Visibility ? info.Visibility : item.Visibility;

                if (Items.IndexOf(item) != info.Index)
                {
                    changedItems.Add(item);
                }
            }

            if (changedItems.Count > 0)
            {
                foreach (TabItemExt item in changedItems)
                {
                    ItemInfo info = (ItemInfo)tabItems[item.Name];
                    Items.Remove(item);
                    Items.Insert(info.Index, item);
                  }
            }

            if (SkinStorage.GetVisualStyle(this) == "Default" || SkinStorage.GetVisualStyle(this) == "OneNote")
            {
                VerifyZIndex();
            }
        }

		/// <summary>
		/// Gets the tab item.
		/// </summary>
		/// <param name="item">Value of the item.</param>
		/// <returns>returns TabItemExt</returns>
		internal TabItemExt GetTabItem(object item)
		{
			return (TabItemExt)(item is TabItemExt ? item : ItemContainerGenerator.ContainerFromItem(item));
		}

		/// <summary>
		/// Gets the index of the TabItemExt.
		/// </summary>
		/// <remarks>Preferred way of getting the index when business model are binded in MVVM pattern.</remarks>
		internal int GetTabItemIndex(object item)
		{
			if (item == null) return -1;

			var tabItem = GetTabItem(item);

			return (tabItem == null) ? -1 : ItemContainerGenerator.IndexFromContainer(tabItem);
		}

		/// <summary>
		/// Gets the source object associated with this container item. 
		/// </summary>
		/// <param name="item">the container item.</param>
		/// <returns>the source object. [business model]</returns>
		internal object GetSourceObject(object item)
		{
			if (item == null) return null;

			var tabItem = GetTabItem(item);

			return (tabItem == null) ? null : ItemContainerGenerator.ItemFromContainer(tabItem);
		}

        /// <summary>
        /// Determines whether the specified raiser is canceled.
        /// </summary>
        /// <returns>returns a bool value</returns>
        private bool CanceledClosed()
        {
            CancelingRoutedEventArgs args = new CancelingRoutedEventArgs(TabControlExt.TabClosingEvent)
            {
                Source = SelectedItem,
            };

            RaiseEvent(args);
            return args.Cancel;
        }

        /// <summary>
        /// Determines whether the specified raiser is canceled.
        /// </summary>
        /// <returns>returns a bool value</returns>
        internal bool CanceledClosed(FrameworkElement source)
        {
            CancelingRoutedEventArgs args = new CancelingRoutedEventArgs(TabControlExt.TabClosingEvent)
            {
                Source = source,
            };

            RaiseEvent(args);
            return args.Cancel;
        }

        /// <summary>
        /// Loads Tab sate from registry.
        /// </summary>
        /// <returns>returns TabControlSettings</returns>
        /// <example>
        ///       <para/>This example shows how to use LoadTabState( BinaryFormatter Serializer ) in C#.
        /// <code language="C#">
        /// BinaryFormatter formatter1 = new BinaryFormatter();
        /// tabControl.LoadTabState( formatter1 );
        /// </code>
        /// </example>
        /// <seealso cref="BinaryFormatter"/>
        private static TabControlSettings LoadFromRegistry()
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(C_regSubKeyName);
            TabControlSettings settings = null;

            if (regKey != null)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream memStream = new MemoryStream();
                byte[] byteArr = (byte[])regKey.GetValue(C_regParamName);
                memStream.Write(byteArr, 0, byteArr.Length);
                memStream.Position = 0;
                try
                {
                    settings = (TabControlSettings)formatter.Deserialize(memStream);
                }
                catch (SerializationException ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw;
                }
                finally
                {
                    memStream.Close();
                }
            }

            return settings;
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when TabListContextMenuItemTemplate property is changed.
        /// </summary>
        public event PropertyChangedCallback TabListContextMenuItemTemplateChanged;

        /// <summary>
        /// Event that is raised when ScrollingTime property is changed.
        /// </summary>
        public event PropertyChangedCallback ScrollingTimeChanged;

        /// <summary>
        /// Event that is raised when SelectedIndex property is changed.
        /// </summary>
        public event PropertyChangedCallback SelectedIndexChanged;

        /// <summary>
        /// Event that is raised when FlowDirection property is changed.
        /// </summary>
        public event PropertyChangedCallback FlowDirectionChanged;

        /// <summary>
        /// Event that is raised when TabStripPlacement property is changed.
        /// </summary>
        public event PropertyChangedCallback TabStripPlacementChanged;

        /// <summary>
        /// Event that is raised when DragMarkerStyle property is changed.
        /// </summary>
        public event PropertyChangedCallback DragMarkerStyleChanged;

        /// <summary>
        /// Event that is raised when DragMarkerColor property is changed.
        /// </summary>
        public event PropertyChangedCallback DragMarkerColorChanged;

        /// <summary>
        /// Event that is raised when TabPanelBackground proeprty is changed.
        /// </summary>
        public event PropertyChangedCallback TabPanelBackgroundChanged;

        /// <summary>
        /// Event that is raised when EnableLabelEdit property is changed.
        /// </summary>
        public event PropertyChangedCallback EnableLabelEditChanged;

        /// <summary>
        /// Event that is raised when TabPanelItem property is changed.
        /// </summary>
        public event PropertyChangedCallback TabPanelItemChanged;

        /// <summary>
        /// Event that is raised when ShowTabListContextMenu property is changed.
        /// </summary>
        public event PropertyChangedCallback ShowTabListContextMenuChanged;

        /// <summary>
        /// Event that is raised when ShowTabItemContextMenu property is changed.
        /// </summary>
        public event PropertyChangedCallback ShowTabItemContextMenuChanged;

        /// <summary>
        /// Event that is raised when CloseButtonType property is changed.
        /// </summary>
        public event PropertyChangedCallback CloseButtonTypeChanged;

        /// <summary>
        /// Event that is raised when HotTrackingEnabled property is changed.
        /// </summary>
        public event PropertyChangedCallback HotTrackingEnabledChanged;

        /// <summary>
        /// Event that is raised when TabPanelTemplate property is changed.
        /// </summary>
        public event PropertyChangedCallback TabPanelTemplateChanged;

        /// <summary>
        /// Event that is raised when TabPanelStyle property is changed.
        /// </summary>
        public event PropertyChangedCallback TabPanelStyleChanged;

        /// <summary>
        /// Event that is raised when TabScrollButtonVisibility property is changed.
        /// </summary>
        public event PropertyChangedCallback TabScrollButtonVisibilityChanged;

        /// <summary>
        /// Event that is raised when TabScrollStyle property is changed.
        /// </summary>
        public event PropertyChangedCallback TabScrollStyleChanged;

        /// <summary>
        /// Event that is raised when TabItemLayout property is changed.
        /// </summary>
        public event PropertyChangedCallback TabItemLayoutChanged;

        /// <summary>
        /// Event that is raised when TabItemSize property is changed.
        /// </summary>
        public event PropertyChangedCallback TabItemSizeChanged;

        /// <summary>
        /// Event that is raised when KeepTabInFront property is changed.
        /// </summary>
        public event PropertyChangedCallback KeepTabInFrontChanged;

        /// <summary>
        /// Event that is raised when RotateTextWhenVertical property is changed.
        /// </summary>
        public event PropertyChangedCallback RotateTextWhenVerticalChanged;

        /// <summary>
        /// Event that is raised when IsCloseTabProcessEnabled property is changed.
        /// </summary>
        public event OnCloseTabsEventHandler OnCloseOtherTabs;

        /// <summary>
        /// Occurs when [on close all tabs].
        /// </summary>
        public event OnCloseTabsEventHandler OnCloseAllTabs;

        /// <summary>
        /// Occurs when [on close button click].
        /// </summary>
        public event OnCloseTabsEventHandler OnCloseButtonClick;

        /// <summary>
        /// Occurs when [before drop down context menu open].
        /// </summary>
        public event EventHandler BeforeDropDownContextMenuOpen;

        /// <summary>
        /// Occurs when [before drop down context menu close].
        /// </summary>
        public event EventHandler BeforeDropDownContextMenuClose;

        /// <summary>
        /// Occurs when [before label edit].
        /// </summary>
        public event BeforeLabelEditHandler BeforeLabelEdit;

        /// <summary>
        /// Occurs when [after label edit].
        /// </summary>
        public event AfterLabelEditHandler AfterLabelEdit;

       

        /// <summary>
        /// Occurs when [take drag item event].
        /// </summary>
        public event TakeDragItemHandler TakeDragItemEvent;

        /// <summary>
        /// Occurs when [selected item changed event].
        /// </summary>
        public event SelectedItemChangedEventHandler SelectedItemChangedEvent;

        public event PreviewSelectedItemChangedEventHandler PreviewSelectedItemChangedEvent;
        /// <summary>
        /// Event that is raised when SelectedItemFontWeight property is changed.
        /// </summary>
        public event PropertyChangedCallback SelectedItemFontWeightChanged;

        /// <summary>
        /// Event that is raised when AllowDragDrop property is changed.
        /// </summary>
        public event PropertyChangedCallback AllowDragDropChanged;

        /// <summary>
        /// Occurs when [document closing].
        /// </summary>
        public event CancelingRoutedEventHandler TabClosing
        {
            add
            {
                AddHandler(TabClosingEvent, value);
            }

            remove
            {
                RemoveHandler(TabClosingEvent, value);
            }
        }

        /// <summary>
        /// Routed event that is raised when the tab is about to close. 
        /// User can cancel the closure by setting Cancel to true.
        /// </summary>
        public static readonly RoutedEvent TabClosingEvent =
            EventManager.RegisterRoutedEvent("TabClosing", RoutingStrategy.Direct, typeof(CancelingRoutedEventHandler), typeof(TabControlExt));

        public event TabClosedEventHandler TabClosed;

        /// <summary>
        /// Occurs when [items changed].
        /// </summary>
        internal event NotifyCollectionChangedEventHandler ItemsChanged;

        /// <summary>
        /// Occurs when [is close tab process enabled changed].
        /// </summary>
        internal event PropertyChangedCallback IsCloseTabProcessEnabledChanged;
        #endregion

        #region Drag and Drop Cancel Event Implementation

        /// <summary>
        /// Represents the DragSource object
        /// </summary>
        public static FrameworkElement DragSourceObject;

        /// <summary>
        /// Represents the Dragged Item object
        /// </summary>
        public static FrameworkElement DraggedItem;

        #region Events
        /// <summary>
        /// Event represents the DragStartEvent
        /// </summary>
        public static readonly RoutedEvent DragStartEvent = EventManager.RegisterRoutedEvent(
           "DragStart",
           RoutingStrategy.Bubble,
           typeof(TabControlExtDragHandler),
           typeof(TabControlExt));

        /// <summary>
        /// Event represents the DragEndEvent
        /// </summary>
        public static readonly RoutedEvent DragEndEvent = EventManager.RegisterRoutedEvent(
            "DragEnd",
            RoutingStrategy.Bubble,
            typeof(TabControlExtDragHandler),
            typeof(TabControlExt));

        #endregion

        #region Event methods
        /// <summary>
        /// Occurs when [drag start].
        /// </summary>
        public event TabControlExtDragHandler DragStart
        {
            add
            {
                AddHandler(DragStartEvent, value);
            }

            remove
            {
                RemoveHandler(DragStartEvent, value);
            }
        }

        /// <summary>
        /// Occurs when [drag end].
        /// </summary>
        public event TabControlExtDragHandler DragEnd
        {
            add
            {
                AddHandler(DragEndEvent, value);
            }

            remove
            {
                RemoveHandler(DragEndEvent, value);
            }
        }

        /// <summary>
        /// Delegate represents the TabControlExtDragHandler
        /// </summary>
        /// <param name="sender">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>        
        public delegate void TabControlExtDragHandler(object sender, TabControlExtDragEventArgs e);
        #endregion
        #endregion

        #region Event handlers
        /// <summary>
        /// Calls OnTabListContextMenuItemTemplateChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTabListContextMenuItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt)d;
            instance.OnTabListContextMenuItemTemplateChanged(e);
        }

        /// <summary>
        /// Called when [new button alignment change].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnNewButtonAlignmentChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt)d;
            if (instance.TabLayoutPanel != null)
            {
                instance.TabLayoutPanel.InvalidateMeasure();
                instance.TabLayoutPanel.InvalidateArrange();
            }
        }

        /// <summary>
        /// Called when [new button border thickness change].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnNewButtonBorderThicknessChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt)d;
            if (instance.TabLayoutPanel != null)
            {
                instance.TabLayoutPanel.InvalidateMeasure();
                instance.TabLayoutPanel.InvalidateArrange();
            }
        }


        /// <summary>
        /// Updates property value cache and raises TabListContextMenuItemTemplateChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnTabListContextMenuItemTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (TabListContextMenuItemTemplateChanged != null)
            {
                TabListContextMenuItemTemplateChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnScrollingTimeChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnScrollingTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt)d;
            instance.OnScrollingTimeChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises ScrollingTimeChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnScrollingTimeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ScrollingTimeChanged != null)
            {
                ScrollingTimeChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnDragMarkerColorChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnDragMarkerColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt)d;
            instance.OnDragMarkerColorChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises DragMarkerColorChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnDragMarkerColorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DragMarkerColorChanged != null)
            {
                DragMarkerColorChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [tab panel background changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnTabPanelBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt)d;
            instance.OnTabPanelBackgroundChanged(e);
        }

        private static void OnIsNewButtonEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt)d;
            if (instance.TabLayoutPanel != null)
            {
                if ((bool)e.NewValue)
                {
                    instance.TabLayoutPanel.AddNewTab();
                    instance.TabLayoutPanel.InvalidateMeasure();
                    instance.TabLayoutPanel.InvalidateArrange();
                }
                else
                {
                    instance.TabLayoutPanel.RemoveNewTab();
                    instance.TabLayoutPanel.InvalidateMeasure();
                    instance.TabLayoutPanel.InvalidateArrange();
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:TabPanelBackgroundChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnTabPanelBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (TabPanelBackgroundChanged != null)
            {
                TabPanelBackgroundChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnDragMarkerStyleChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnDragMarkerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt)d;
            instance.OnDragMarkerStyleChanged(e);
        }

        /// <summary>
        /// Calls OnFullScreenModeChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFullScreenModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt)d;
            instance.ApplyFullScreen();
        }

        

        /// <summary>
        /// Updates property value cache and raises DragMarkerStyleChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnDragMarkerStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DragMarkerStyleChanged != null)
            {
                DragMarkerStyleChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnEnableLabelEditChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnEnableLabelEditChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt)d;
            instance.OnEnableLabelEditChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises EnableLabelEditChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnEnableLabelEditChanged(DependencyPropertyChangedEventArgs e)
        {
            if (EnableLabelEditChanged != null)
            {
                EnableLabelEditChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnTabPanelItemChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTabPanelItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt)d;
            instance.OnTabPanelItemChanged(e);
           
        }

       
        
        /// <summary>
        /// Updates property value cache and raises TabPanelItemChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnTabPanelItemChanged(DependencyPropertyChangedEventArgs e)
        {
            if (TabPanelItemChanged != null)
            {
                TabPanelItemChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnShowTabListContextMenuChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnShowTabListContextMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt)d;
            instance.OnShowTabListContextMenuChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises ShowTabListContextMenuChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnShowTabListContextMenuChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ShowTabListContextMenuChanged != null)
            {
                ShowTabListContextMenuChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnShowTabItemContextMenuChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnShowTabItemContextMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt)d;
            instance.OnShowTabItemContextMenuChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises ShowTabItemContextMenuChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnShowTabItemContextMenuChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ShowTabItemContextMenuChanged != null)
            {
                ShowTabItemContextMenuChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnCloseButtonTypeChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCloseButtonTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt)d;

            #region CloseButtonVisibilityChecking

            if (instance.TabPanel != null && instance.TabPanel.Template != null)
            {
                ToggleButton closebutton = instance.TabPanel.Template.FindName("PART_CloseButton", instance.TabPanel) as ToggleButton;
                instance.CheckCloseButtonVisibility(closebutton);
            }

            foreach (TabItemExt item in instance.Items)
            {
                if (item != null && item.IsLoaded)
                {
                    ContentPresenter presenter = item.Content as ContentPresenter;
                    if (presenter != null && presenter.Content != null)
                    {
                        if (item.Template != null)
                        {
                            ToggleButton closebutton = item.Template.FindName("PART_CloseButton", item) as ToggleButton;
                            if (closebutton != null)
                            {
                                DockingManager owner = DockingManager.ResolveManager(presenter.Content as UIElement);
                                if (owner != null)
                                {
                                    bool close = DockingManager.GetCanClose(presenter.Content as DependencyObject);
                                    item.CheckCloseButtonVisibilityOnLoad(closebutton, close, owner.DisabledCloseButtonsBehavior);
                                }
                                else
                                {
                                    DocumentContainer container = VisualUtils.FindAncestor((Visual)instance, typeof(DocumentContainer)) as DocumentContainer;
                                    if (container != null)
                                    {
                                        bool close = DocumentContainer.GetCanClose(presenter.Content as DependencyObject);
                                        item.CheckCloseButtonVisibilityOnLoad(closebutton, close, container.DisabledButtonsBehavior);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            instance.OnCloseButtonTypeChanged(e);
        }

        /// <summary>
        /// Checks the close button visibility.
        /// </summary>
        /// <param name="closebutton">The closebutton.</param>
        internal void CheckCloseButtonVisibility(ToggleButton closebutton)
        {
            if (closebutton != null)
            {
                DockingManager owner = VisualUtils.FindAncestor((Visual)this, typeof(DockingManager)) as DockingManager;
                if (owner != null)
                {
                    if (this.SelectedItem != null && this.SelectedItem is TabItemExt)
                    {
                        FrameworkElement element = (this.SelectedItem as TabItemExt).GetContents();
                        if (element != null)
                        {
                            bool close = DockingManager.GetCanClose(element);
                            CheckTabPanelCloseButtonVisibility(closebutton, close, owner.DisabledCloseButtonsBehavior);
                        }
                    }
                }
                else
                {
                    DocumentContainer container = VisualUtils.FindAncestor((Visual)this, typeof(DocumentContainer)) as DocumentContainer;
                    if (container != null)
                    {
                        if (this.SelectedItem != null && this.SelectedItem is TabItemExt)
                        {
                            FrameworkElement element = (this.SelectedItem as TabItemExt).GetContents();
                            if (element != null)
                            {
                                bool close = DocumentContainer.GetCanClose(element);
                                CheckTabPanelCloseButtonVisibility(closebutton, close, container.DisabledButtonsBehavior);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the MouseEnter event of the closebutton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void closebutton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                if (TabPanel != null)
                {
                    ToggleButton closebutton = sender as ToggleButton;
                    CheckCloseButtonVisibility(closebutton);
                }
            }
        }

        /// <summary>
        /// Updates property value cache and raises CloseButtonTypeChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnCloseButtonTypeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CloseButtonTypeChanged != null)
            {
                CloseButtonTypeChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnHotTrackingEnabledChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnHotTrackingEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt)d;
            instance.OnHotTrackingEnabledChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises HotTrackingEnabledChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnHotTrackingEnabledChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HotTrackingEnabledChanged != null)
            {
                HotTrackingEnabledChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnRotateTextWhenVerticalChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnRotateTextWhenVerticalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt)d;
            if (SkinStorage.GetVisualStyle(instance) == "Default" || SkinStorage.GetVisualStyle(instance) == "OneNote")
            {
                instance.VerifyZIndex();
                instance.RefreshLayout();
            }
            else
            {
                instance.RefreshLayout();
            }

            instance.OnRotateTextWhenVerticalChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises RotateTextWhenVerticalChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnRotateTextWhenVerticalChanged(DependencyPropertyChangedEventArgs e)
        {
            if (RotateTextWhenVerticalChanged != null)
            {
                RotateTextWhenVerticalChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnTabPanelTemplateChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTabPanelTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt)d;
            instance.OnTabPanelTemplateChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises TabPanelTemplateChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnTabPanelTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (TabPanelTemplateChanged != null)
            {
                TabPanelTemplateChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnTabPanelStyleChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTabPanelStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt)d;
            instance.OnTabPanelStyleChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises TabPanelStyleChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnTabPanelStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (TabPanelStyleChanged != null)
            {
                TabPanelStyleChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnTabScrollButtonVisibilityChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTabScrollButtonVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt)d;
            instance.RefreshLayout();
            instance.OnTabScrollButtonVisibilityChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises TabScrollButtonVisibilityChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnTabScrollButtonVisibilityChanged(DependencyPropertyChangedEventArgs e)
        {
            if (TabScrollButtonVisibilityChanged != null)
            {
                TabScrollButtonVisibilityChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnTabScrollStyleChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTabScrollStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt)d;
            instance.RefreshLayout();
            instance.OnTabScrollStyleChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises TabScrollStyleChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnTabScrollStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (TabScrollStyleChanged != null)
            {
                TabScrollStyleChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnTabItemLayoutChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTabItemLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt)d;
            instance.RefreshLayout();
            instance.OnTabItemLayoutChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises TabItemLayoutChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnTabItemLayoutChanged(DependencyPropertyChangedEventArgs e)
        {
            if (TabItemLayoutChanged != null)
            {
                TabItemLayoutChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnTabItemSizeChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTabItemSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt)d;
            instance.CoerceValue(TabItemLayoutProperty);
            if (SkinStorage.GetVisualStyle(instance) == "Default" || SkinStorage.GetVisualStyle(instance) == "OneNote")
            {
                instance.VerifyZIndex();
                instance.RefreshLayout();
            }
            else
            {
                instance.RefreshLayout();
            }

            instance.OnTabItemSizeChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises TabItemSizeChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnTabItemSizeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (TabItemSizeChanged != null)
            {
                TabItemSizeChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [disable resize on selection changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDisableResizeOnSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt)d;
            if (instance != null && instance.DisableResizeOnSelection)
            {
                foreach (FrameworkElement element in instance.Items)
                {
                    TabItemExt tabitem = element is TabItemExt ? element as TabItemExt : VisualUtils.FindAncestor((Visual)element, typeof(TabItemExt)) as TabItemExt;
                    if (tabitem != null)
                    {
                        instance.CheckResizeItem(tabitem);
                    }
                }
            }
        }

        /// <summary>
        /// Calls OnKeepTabInFrontChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnKeepTabInFrontChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt)d;
            instance.RefreshLayout();
            instance.OnKeepTabInFrontChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises KeepTabInFrontChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnKeepTabInFrontChanged(DependencyPropertyChangedEventArgs e)
        {
            if (KeepTabInFrontChanged != null)
            {
                KeepTabInFrontChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [coerce tab item layout].
        /// </summary>
        /// <param name="d">Sender Object.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns>returns an object</returns>
        private static object OnCoerceTabItemLayout(DependencyObject d, object baseValue)
        {
            TabControlExt instance = (TabControlExt)d;
            TabItemLayoutType newValue = (TabItemLayoutType)baseValue;

            if (newValue != TabItemLayoutType.SingleLine && instance.TabItemSize == TabItemSizeMode.ShrinkToFit)
            {
                return TabItemLayoutType.SingleLine;
            }

            return newValue;
        }

        /// <summary>
        /// Called when [rotate text when vertical].
        /// </summary>
        /// <param name="d">Sender Object.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns>returns an object</returns>
        private static object OnRotateTextWhenVertical(DependencyObject d, object baseValue)
        {
            TabControlExt instance = (TabControlExt)d;
            bool newValue = (bool)baseValue;

            if (instance.TabStripPlacement == Dock.Top || instance.TabStripPlacement == Dock.Bottom)
            {
                return false;
            }

            return newValue;
        }

        /// <summary>
        /// Calls OnIsCloseTabProcessEnabledChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsCloseTabProcessEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool newValue = (bool)e.NewValue;
            TabControlExt instance = (TabControlExt)d;

            if (newValue)
            {
                instance.CommandBindings.Add(instance.m_CloseTabCommandBinding);
            }
            else
            {
                instance.CommandBindings.Remove(instance.m_CloseTabCommandBinding);
            }

            instance.OnIsCloseTabProcessEnabledChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises IsCloseTabProcessEnabledChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnIsCloseTabProcessEnabledChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsCloseTabProcessEnabledChanged != null)
            {
                IsCloseTabProcessEnabledChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnTabStripPlacementChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnTabStripPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt)d;
            instance.CoerceValue(RotateTextWhenVerticalProperty);

            if (instance.TabLayoutPanel != null)
            {
                instance.TabLayoutPanel.ValidateScrollingPanel();
            }

            if (SkinStorage.GetVisualStyle(instance) == "Default" || SkinStorage.GetVisualStyle(instance) == "OneNote")
            {
                instance.VerifyZIndex();
            }

            #if SyncfusionFramework4_0
                if (e.NewValue.ToString() != "Top" && !instance.RotateTextWhenVertical)
                {
                    TextOptions.SetTextFormattingMode(instance, TextFormattingMode.Display);
                }
            #endif

            instance.OnTabStripPlacementChanged(e);
        }
        

        /// <summary>
        /// Updates property value cache and raises TabStripPlacementChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnTabStripPlacementChanged(DependencyPropertyChangedEventArgs e)
        {
            if (TabStripPlacementChanged != null)
            {
                TabStripPlacementChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnFlowDirectionChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnFlowDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt)d;
            instance.OnFlowDirectionChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises FlowDirectionChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnFlowDirectionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (FlowDirectionChanged != null)
            {
                FlowDirectionChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnFlowDirectionChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt)d;
            instance.OnSelectedIndexChanged(e);
        }

        /// <summary>
        /// Handles the PreviewMouseRightButtonDown event of the m_ListMenuButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void M_ListMenuButton_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
                m_listMenuButton.ContextMenu = null;
        }

        /// <summary>
        /// Called when [list context menu item_ click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnListContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;

            if (menuItem.Tag is TabItemExt)
            {
                TabLayoutPanel.SelectItemInternal(menuItem.Tag as TabItemExt);
            }
        }

        /// <summary>
        /// Handles the Closed event of the ListContextMenu control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ListContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            if ((sender as ContextMenu) != null)
            {
                (sender as ContextMenu).Items.Clear();
            }
            FireBeforeDropDownContextMenuClose();
        }

        internal bool m_SelectionFlag = true;
        /// <summary>
        /// Called when [generator status changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnGeneratorStatusChanged(object sender, EventArgs e)
        {
            if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                if (!TabLayoutPanel.AddAtLast && this.HasItems && SelectedItem != null && !DragDropHelper.dragTabItemFlag)
                {
                    if (Items.Count > 0)
                    {
                        TabItemExt item = GetTabItem(Items[0]);
                        if (item.Visibility != Visibility.Collapsed && m_SelectionFlag)
                        {
                            this.SelectedIndex = 0;
                        }
                    }                    
                }
            }
        }

        /// <summary>
        /// Calls OnAllowDragDropChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnAllowDragDropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt)d;
            instance.OnAllowDragDropChanged(e);
        }

        /// <summary>
        /// Called when [hide header on single child changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnHideHeaderOnSingleChildChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt)d;
            instance.UpdateChildLayout();
        }

        /// <summary>
        /// Updates the child layout.
        /// </summary>
        private void UpdateChildLayout()
        {
            if (Items.Count > 0)
            {
                if (Items[0] is TabItemExt)
                {
                    foreach (TabItemExt item in Items)
                    {
                        item.InvalidateMeasure();
                        item.InvalidateArrange();
                    }
                }
                else
                {
                    foreach (object obj in Items)
                    {
                        TabItemExt item = GetTabItem(obj);
                        if (item != null)
                        {
                            item.InvalidateMeasure();
                            item.InvalidateArrange();
                        }
                    }
                }
            }
        }

        internal void UpdateIndex(int index, object item)
        {
            int actualindex = Items.IndexOf(item);
            if (actualindex > -1)
            {
                if (actualindex < Items.Count)
                {
                    Items.RemoveAt(actualindex);
                }
                if (index < Items.Count)
                {
                    InsertItem(index, item);
                }
                else
                {
                    Items.Add(item);
                }
            }
        }

        /// <summary>
        /// Called when [coerce allow drag drop].
        /// </summary>
        /// <param name="d">Sender Object.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns>returns an object</returns>
        private static object OnCoerceAllowDragDrop(DependencyObject d, object baseValue)
        {
            TabControlExt instance = (TabControlExt)d;
            bool newValue = (bool)baseValue;

            if ((newValue && instance.ItemsSource != null)
                || !PermissionHelper.HasUnmanagedCodePermission)
            {
                return true;
            }

            return newValue;
        }
        #endregion

        #region Command handlers
        /// <summary>
        /// Represents the method that will handle the CanExecute event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        /// <property name="flag" value="Finished"/>
        protected virtual void CanProcessCloseTabItemCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Processes CloseTabItemCommand command.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The instance containing the event data.</param>
        /// <property name="flag" value="Finished"/>
        private void ProcessCloseTabItemCommand(object sender, ExecutedRoutedEventArgs e)
        {
            TabItemExt item = e.OriginalSource is TabItemExt ? e.OriginalSource as TabItemExt :
                VisualUtils.FindAncestor(e.OriginalSource as Visual, typeof(TabItemExt)) as TabItemExt;
         
            if (item != null && SelectedItem != null && !CanceledClosed(item))
            {
                item.Focus();
                CloseTabItem(item);
                VerifyZIndex();
            }

            if (TabLayoutPanel.VisibleItemsCount == 0)
            {
                if ((IsNewButtonEnabled && IsNewButtonClosedonNoChild) || !IsNewButtonEnabled)
                TabPanel.Visibility = Visibility.Collapsed;
                
                    SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Represents the method that will handle the CanExecute event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        /// <property name="flag" value="Finished"/>
        protected virtual void CanProcessCloseCurrentTabItemCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Determines whether this instance [can close tab item] the specified item.
        /// </summary>
        /// <param name="item">The tab item</param>
        /// <returns>
        /// <c>true</c> if this instance [can close tab item] the specified item; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanCloseTabItem(TabItemExt item)
        {
            return true;
        }

        /// <summary>
        /// Processes CloseCurrentTabItemCommand command.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The instance containing the event data.</param>
        /// <property name="flag" value="Finished"/>
        private void ProcessCloseCurrentTabItemCommand(object sender, ExecutedRoutedEventArgs e)
        {
           
            if (SelectedItem != null && !CanceledClosed())
            {
                CloseTabItem(GetTabItem(SelectedItem));
            }

            if (TabLayoutPanel.VisibleItemsCount == 0)
            {
                if ((IsNewButtonEnabled && IsNewButtonClosedonNoChild) || !IsNewButtonEnabled)
                TabPanel.Visibility = Visibility.Collapsed;
                SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Represents the method that will handle the CanExecute event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        /// <property name="flag" value="Finished"/>
        protected virtual void CanProcessCloseTabsCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            int visibleItemsCount = 0;
            TabItemExt selItem = ActivatedItem;

            foreach (object element in Items)
            {
                TabItemExt item = GetTabItem(element);

                if (item.Visibility == Visibility.Visible)
                {
                    visibleItemsCount++;
                }
            }

            e.CanExecute = visibleItemsCount > 1;
        }

        /// <summary>
        /// Processes CloseTabsCommand command.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The instance containing the event data.</param>
        /// <property name="flag" value="Finished"/>
        private void ProcessCloseTabsCommand(object sender, ExecutedRoutedEventArgs e)
        {
          
            TabItemExt currentItem = e.OriginalSource is TabItemExt
                ? e.OriginalSource as TabItemExt
                : VisualUtils.FindAncestor(e.OriginalSource as Visual, typeof(TabItemExt)) as TabItemExt;

            if (currentItem != null)
            {
                SelectedIndex = ItemContainerGenerator.IndexFromContainer(currentItem);
                string parameter = (string)e.Parameter;

                if (parameter == "All")
                {
                    CloseTabEventArgs args = new CloseTabEventArgs(currentItem,"CloseAllTabs");
                    FireOnCloseAllTabs(args);                                     
                    if (!args.Cancel)
                    {
                        SelectedIndex = -1;
                        if (this.CloseMode == CloseMode.Hide)
                        {
                            foreach (object item in Items)
                            {
                                TabItemExt tabItem = ItemContainerGenerator.ContainerFromItem(item) as TabItemExt;

                                if (tabItem != null && CanCloseTabItem(tabItem))
                                {
                                    if (!CanceledClosed(tabItem))
                                    {
                                        tabItem.Visibility = Visibility.Collapsed;
                                    }
                                }
                            }
                        }
                        else if (this.CloseMode == CloseMode.Delete)
                        {
                            if (this.ItemsSource == null)
                            {
                                this.Items.Clear();
                            }
                            else
                            {
                                TabLayoutPanel.TabItemExtCollection.Clear();
                            }

                        }
                    }                    
                }
                else
                {
                    CloseTabEventArgs args = new CloseTabEventArgs(currentItem,"CloseOtherTabs");
                    FireOnCloseOtherTabs(args);
                    if (!args.Cancel)
                    {
                        if (this.CloseMode == CloseMode.Hide)
                        {
                            foreach (object item in Items)
                            {
                                TabItemExt tabItem = ItemContainerGenerator.ContainerFromItem(item) as TabItemExt;
                                int tabindex = ItemContainerGenerator.IndexFromContainer(tabItem);
                                if (tabItem != null)
                                {
                                    if (tabItem == currentItem)
                                    {
                                        continue;
                                    }
                                    if (CanCloseTabItem(tabItem) && !CanceledClosed(tabItem))
                                    {
                                        tabItem.Visibility = Visibility.Collapsed;
                                    }
                                }
                            }
                        }
                        else if (this.CloseMode == CloseMode.Delete)
                        {
                            if (this.ItemsSource == null)
                            {
                                var TempCollection = from TabItemExt tempitem in Items
                                                     where tempitem != currentItem
                                                     select tempitem;

                                foreach (var item in TempCollection.ToList())
                                {
                                    Items.Remove(item);
                                }
                            }
                            else
                            {
                                var TempCollection = from Object tempitem in TabLayoutPanel.TabItemExtCollection
                                                     where tempitem != currentItem
                                                     select tempitem;

                                foreach (var item in TempCollection.ToList())
                                {
                                    TabLayoutPanel.TabItemExtCollection.Remove(item);
                                }
                            }
                        }
                    }
                }
            }

            if (TabLayoutPanel.VisibleItemsCount == 0)
            {
                if ((IsNewButtonEnabled && IsNewButtonClosedonNoChild)||!IsNewButtonEnabled)
                {
                    TabPanel.Visibility = Visibility.Collapsed;
                }               
                SelectedIndex = -1;
            }

            VerifyZIndex();
        }

        /// <summary>
        /// Processes OpenContextMenu command.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The instance containing the event data.</param>
        /// <property name="flag" value="Finished"/>
        private void ProcessOpenContextMenu(object sender, ExecutedRoutedEventArgs e)
        {
            FireBeforeDropDownContextMenuOpen();
           
            Style contextMenuStyle=null;
             if(TabListContextMenuStyle!=null)
                 contextMenuStyle = TabListContextMenuStyle;
            ContextMenu contextMenu = new ContextMenu
            {
                Style = contextMenuStyle,
                FlowDirection = FlowDirection
            };

            if (TabListContextMenuStyle != null)
            {
                contextMenu.Style = TabListContextMenuStyle;
            }

            if (TabListContextMenuTemplate != null)
            {
                contextMenu.Template = TabListContextMenuTemplate;
            }

            if (m_listMenuButton != null)
            {
                m_listMenuButton.PreviewMouseRightButtonDown -= new MouseButtonEventHandler(M_ListMenuButton_PreviewMouseRightButtonDown);
#if !SyncfusionFramework3_5
                //m_listMenuButton.PreviewTouchMove -= m_listMenuButton_PreviewTouchMove;
#endif
            }

            m_listMenuButton = e.OriginalSource as ToggleButton;
            m_listMenuButton.ContextMenu = contextMenu;
            contextMenu.PlacementTarget = m_listMenuButton;
            m_listMenuButton.PreviewMouseRightButtonDown += new MouseButtonEventHandler(M_ListMenuButton_PreviewMouseRightButtonDown);
#if !SyncfusionFramework3_5
            //m_listMenuButton.PreviewTouchMove += m_listMenuButton_PreviewTouchMove;
#endif

            if (!CollapseDefaultTabListContextMenuItems)
            {
                foreach (object element in Items)
                {
                    TabItemExt item = GetTabItem(element);
                    if (item.Visibility != Visibility.Visible)
                    {
                        continue;
                    }

                    MenuItem newItem = new MenuItem();
                    ImageSource imageSource = TabControlExt.GetImage(item);


                    if (imageSource != null)
                    {
                        Image image = new Image
                        {
                            Source = imageSource
                        };
                        newItem.Icon = image;
                    }

                    ImageSource menuicon = TabControlExt.GetMenuIcon(item);

                    if (menuicon != null)
                    {
                        Image image = new Image
                        {
                            Source = menuicon
                        };
                        newItem.Icon = image;
                    }

                    if (ItemTemplate != null)
                    {
                        newItem.HeaderTemplate = ItemTemplate;
                    }

                    if (TabListContextMenuItemTemplate != null)
                    {
                        newItem.HeaderTemplate = TabListContextMenuItemTemplate;
                    }

                    if (TabListContextMenuItemStyle != null)
                    {
                        newItem.Style = TabListContextMenuItemStyle;
                    }

                    newItem.Header = item.Header;
                    newItem.Tag = item;
                    newItem.Click += new RoutedEventHandler(OnListContextMenuItem_Click);
                    contextMenu.Items.Add(newItem);
                }
            }
            if (TabListContextMenuItems!=null && TabListContextMenuItems.Count > 0)
            {
                Style menuitemstyle=null;
                ResourceDictionary rd = new ResourceDictionary();
                if (SkinStorage.GetVisualStyle(this) == "Office2007Blue")
                {
                    rd.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/Office2007BlueStyle.xaml", UriKind.RelativeOrAbsolute);
                    menuitemstyle = rd["Office2007BlueMenuItemStyle"] as Style;
                }
                else if (SkinStorage.GetVisualStyle(this) == "Blend")
                {
                    rd.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/BlendStyle.xaml", UriKind.RelativeOrAbsolute);
                    menuitemstyle = rd["BlendMenuItemStyle"] as Style;
                }
                else if (SkinStorage.GetVisualStyle(this) == "Office2007Silver")
                {
                    rd.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/Office2007SilverStyle.xaml", UriKind.RelativeOrAbsolute);
                    menuitemstyle = rd["Office2007SilverMenuItemStyle"] as Style;
                }
                else if (SkinStorage.GetVisualStyle(this) == "Office2007Black")
                {
                    rd.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/Office2007BlackStyle.xaml", UriKind.RelativeOrAbsolute);
                    menuitemstyle = rd["Office2007BlackMenuItemStyle"] as Style;
                }
                else if (SkinStorage.GetVisualStyle(this) == "Office2010Blue")
                {
                    rd.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/Office2010BlueStyle.xaml", UriKind.RelativeOrAbsolute);
                    menuitemstyle = rd["Office2010BlueMenuItemStyle"] as Style;
                }
                else if (SkinStorage.GetVisualStyle(this) == "Office2010Silver")
                {
                    rd.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/Office2010SilverStyle.xaml", UriKind.RelativeOrAbsolute);
                    menuitemstyle = rd["Office2010SilverMenuItemStyle"] as Style;
                }
                else if (SkinStorage.GetVisualStyle(this) == "Office2010Black")
                {
                    rd.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/Office2010BlackStyle.xaml", UriKind.RelativeOrAbsolute);
                    menuitemstyle = rd["Office2010BlackMenuItemStyle"] as Style;
                }
                else if (SkinStorage.GetVisualStyle(this) == "Office2003")
                {
                    rd.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/Office2003Style.xaml", UriKind.RelativeOrAbsolute);
                    menuitemstyle = rd["Office2003MenuItemStyle"] as Style;
                }
                else if (SkinStorage.GetVisualStyle(this) == "SyncOrange")
                {
                    rd.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/SyncOrangeStyle.xaml", UriKind.RelativeOrAbsolute);
                    menuitemstyle = rd["SyncOrangeMenuItemStyle"] as Style;
                }
                else if (SkinStorage.GetVisualStyle(this) == "ShinyRed")
                {
                    rd.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/ShinyRedStyle.xaml", UriKind.RelativeOrAbsolute);
                    menuitemstyle = rd["ShinyRedMenuItemStyle"] as Style;
                }
                else if (SkinStorage.GetVisualStyle(this) == "ShinyBlue")
                {
                    rd.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/ShinyBlueStyle.xaml", UriKind.RelativeOrAbsolute);
                    menuitemstyle = rd["ShinyBlueMenuItemStyle"] as Style;
                }
                else if (SkinStorage.GetVisualStyle(this) == "Default")
                {
                    rd.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/DefaultStyle.xaml", UriKind.RelativeOrAbsolute);
                    menuitemstyle = rd["DefaultMenuItemStyle"] as Style;
                }
                else if (SkinStorage.GetVisualStyle(this) == "VS2010")
                {
                    rd.Source = new Uri(@"/Syncfusion.Shared.WPF;component/SkinManager/VS2010Style.xaml", UriKind.RelativeOrAbsolute);
                    menuitemstyle = rd["VS2010MenuItemStyle"] as Style;
                }
                for (int i = 0; i < TabListContextMenuItems.Count; i++)
                {
                    MenuItem item = TabListContextMenuItems[i] as MenuItem;
                    if (item != null)
                    {
                        if (TabListContextMenuItemTemplate != null)
                        {
                            item.HeaderTemplate = TabListContextMenuItemTemplate;
                        }

                        if (TabListContextMenuItemStyle != null)
                        {
                            item.Style = TabListContextMenuItemStyle;
                        }

                        if (TabListContextMenuItemStyle == null && TabListContextMenuItemTemplate == null)
                        {
                            item.Style = menuitemstyle;
                        }
                        contextMenu.Items.Add(item);
                    }
                }
            }
            if (contextMenu.Items.Count > 0)
            {
                contextMenu.Closed += new RoutedEventHandler(ListContextMenu_Closed);
                contextMenu.IsOpen = true;
            }
        }

        /// <summary>
        /// Represents the method that will handle the CanExecute event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        /// <property name="flag" value="Finished"/>
        private static void CanProcessOpenContextMenu(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Calls OnSelectedItemFontWeightChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSelectedItemFontWeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt)d;
            instance.OnSelectedItemFontWeightChanged(e);
        }
        #endregion

        #region Event raisers
        /// <summary>
        /// Raises OnCloseAllTabs event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void FireOnCloseAllTabs(CloseTabEventArgs e)
        {
            if (OnCloseAllTabs != null)
            {
                OnCloseAllTabs(this, e);
            }
        }

        /// <summary>
        /// internal variable which has focus flag
        /// </summary>
        internal bool m_focusflag = false;

        internal int m_tabControlExtTouchDeviceId = -1;

        internal SystemGesture m_tabControlExtSystemGesture;

        #region TouchEvents
#if !SyncfusionFramework3_5

        //protected override void OnTouchEnter(TouchEventArgs e)
        //{
        //    m_tabControlExtTouchDeviceId = (m_tabControlExtTouchDeviceId == -1) ? e.TouchDevice.Id : m_tabControlExtTouchDeviceId;
        //    base.OnTouchEnter(e);
        //}

        //protected override void OnTouchLeave(TouchEventArgs e)
        //{
        //    if (this.IsTouchEnabled && m_tabControlExtTouchDeviceId == e.TouchDevice.Id)
        //    {
        //        m_tabControlExtTouchDeviceId = -1;
        //        m_tabControlExtSystemGesture = SystemGesture.None;
        //    }
        //    base.OnTouchLeave(e);
        //}

        //void m_listMenuButton_PreviewTouchMove(object sender, TouchEventArgs e)
        //{
        //    if (this.IsTouchEnabled && m_tabControlExtTouchDeviceId == e.TouchDevice.Id && m_tabControlExtSystemGesture == SystemGesture.HoldEnter)
        //    {
        //        #region m_listMenuButton_PreviewTouchRightFingerDown
        //        m_listMenuButton.ContextMenu = null;
        //        #endregion
        //    }
        //}

        //void closebutton_TouchEnter(object sender, TouchEventArgs e)
        //{
        //    if (this.IsTouchEnabled && m_tabControlExtTouchDeviceId == e.TouchDevice.Id)
        //    {
        //        if (TabPanel != null)
        //        {
        //            ToggleButton closebutton = sender as ToggleButton;
        //            CheckCloseButtonVisibility(closebutton);
        //        }
        //    }
        //}

        //void border3d_TouchMove(object sender, TouchEventArgs e)
        //{
        //    if (this.IsTouchEnabled && m_tabControlExtTouchDeviceId == e.TouchDevice.Id)
        //    {
        //        if (FullScreenMode != FullScreenMode.None)
        //        {
        //            Border3D border = sender as Border3D;
        //            HeaderPanel panel = GetTemplateChild("HeaderPanel") as HeaderPanel;
        //            double OFFSET = 5;
        //            if (border != null && panel != null)
        //            {
        //                Point point = e.GetTouchPoint(border).Position;
        //                if (this.TabStripPlacement == Dock.Top)
        //                {
        //                    if (point.Y <= OFFSET)
        //                    {
        //                        panel.Visibility = Visibility.Visible;
        //                    }
        //                    else
        //                    {
        //                        panel.Visibility = Visibility.Collapsed;
        //                    }
        //                }
        //                else if (TabStripPlacement == Dock.Left)
        //                {
        //                    if (point.X <= OFFSET)
        //                    {
        //                        panel.Visibility = Visibility.Visible;
        //                    }
        //                    else
        //                    {
        //                        panel.Visibility = Visibility.Collapsed;
        //                    }
        //                }
        //                else if (TabStripPlacement == Dock.Right)
        //                {
        //                    if (point.X >= border.RenderSize.Width - OFFSET)
        //                    {
        //                        panel.Visibility = Visibility.Visible;
        //                    }
        //                    else
        //                    {
        //                        panel.Visibility = Visibility.Collapsed;
        //                    }
        //                }
        //                else if (TabStripPlacement == Dock.Bottom)
        //                {
        //                    if (point.Y >= border.RenderSize.Height - OFFSET)
        //                    {
        //                        panel.Visibility = Visibility.Visible;
        //                    }
        //                    else
        //                    {
        //                        panel.Visibility = Visibility.Collapsed;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

#endif
        #endregion

        protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e)
        {
            m_tabControlExtSystemGesture = e.SystemGesture;
            base.OnStylusSystemGesture(e);
        }
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.PreviewKeyDown"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            m_focusflag = true;
            base.OnPreviewKeyDown(e);

			if (EnableLabelEdit && TabLayoutPanel != null && SelectedItem != null)
			{
				if (!e.Handled && e.Key == Key.F2)
				{
					TabItemExt item = ((SelectedItem is TabItemExt) ? SelectedItem : ItemContainerGenerator.ContainerFromItem(SelectedItem)) as TabItemExt;

					if (item != null && item.IsFocused)
						TabLayoutPanel.LabelEditStartInternal(item);
				}
			}
        }

       
        /// <summary>
        /// Raises after a Tab is Closed.
        /// </summary>
        /// <param name="e">Property Changed Details such as TargetItem</param>
        protected virtual void FireOnClosedTab(CloseTabEventArgs e)
        {
            if (TabClosed != null)
            {
                TabClosed(this, e);
            }
        }

        /// <summary>
        /// Raises OnCloseOtherTabs event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void FireOnCloseOtherTabs(CloseTabEventArgs e)
        {
            if (OnCloseOtherTabs != null)
            {
                OnCloseOtherTabs(this, e);
            }
        }

        /// <summary>
        /// Raises OnCloseButtonClick event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void FireOnCloseButtonClick(CloseTabEventArgs e)
        {
            if (OnCloseButtonClick != null)
            {
                OnCloseButtonClick(this, e);
            }
        }

        /// <summary>
        /// Fires the before drop down context menu open.
        /// </summary>
        protected virtual void FireBeforeDropDownContextMenuOpen()
        {
            if (BeforeDropDownContextMenuOpen != null)
            {
                BeforeDropDownContextMenuOpen(this, new EventArgs());
            }
        }

        /// <summary>
        /// Fires the before drop down context menu close.
        /// </summary>
        protected virtual void FireBeforeDropDownContextMenuClose()
        {
            if (BeforeDropDownContextMenuClose != null)
            {
                BeforeDropDownContextMenuClose(this, new EventArgs());
            }
        }

        /// <summary>
        /// Fires the before label edit.
        /// </summary>
        /// <param name="headerBeforeLabelEdit">The header before label edit.</param>
        protected internal virtual void FireBeforeLabelEdit(TabItemExt tabitem)
        {
            if (BeforeLabelEdit != null)
            {
                BeforeLabelEdit(this, new BeforeLabelEditEventArgs(tabitem));
            }
        }

        /// <summary>
        /// Fires the after label edit.
        /// </summary>
        /// <param name="headerAfterLabelEdit">The header after label edit.</param>
        protected internal virtual void FireAfterLabelEdit(TabItemExt tabitem)
        {
            if (AfterLabelEdit != null)
            {
                AfterLabelEdit(this, new AfterLabelEditEventArgs(tabitem));
            }
        }

        /// <summary>
        /// Fires the drag start.
        /// </summary>
        /// <param name="item">Value of the item.</param>
        /// <returns>returns TabControlExtDragEventArgs</returns>
        protected internal virtual TabControlExtDragEventArgs FireDragStart(TabItemExt item)
        {
            TabControlExtDragEventArgs args = new TabControlExtDragEventArgs(DragStartEvent);
            args.Data = new DataObject(item);
            args.DragSource = TabControlExt.DragSourceObject;
            args.DropSource = this;
            RaiseEvent(args);

            return args;
        }

        /// <summary>
        /// Fires the drag end.
        /// </summary>
        /// <param name="item">Value of the item.</param>
        /// <returns>returns TabControlExtDragEventArgs</returns>
        protected internal virtual TabControlExtDragEventArgs FireDragEnd(TabItemExt item)
        {
            TabControlExtDragEventArgs args = new TabControlExtDragEventArgs(DragEndEvent);
            args.Data = new DataObject(item);
            args.DragSource = TabControlExt.DragSourceObject;
            args.DropSource = this;
            RaiseEvent(args);

            return args;
        }

        /// <summary>
        /// Called when [take drag item event].
        /// </summary>
        /// <param name="item">Value of the item.</param>
        protected internal virtual void OnTakeDragItemEvent(TabItemExt item)
        {
            if (null != TakeDragItemEvent)
            {
                TakeDragItemEvent(this, new TakeDragItemEventArgs(item));
            }
        }

        internal virtual void FirePreviewSelectedItemChangedEvent(PreviewSelectedItemChangedEventArgs e)
        {
            if (PreviewSelectedItemChangedEvent != null)
            {
                PreviewSelectedItemChangedEvent(this, e);
            }
        }
        /// <summary>
        /// Raises SelectedItemChanged event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Tools.Controls.SelectedItemChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void FireSelectedItemChangedEvent(SelectedItemChangedEventArgs e)
        {
            if (SelectedItemChangedEvent != null)
            {
                SelectedItemChangedEvent(this, e);
            }
        }

        /// <summary>
        /// Raises SelectedItemFontWeightChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnSelectedItemFontWeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (null != SelectedItemFontWeightChanged)
            {
                SelectedItemFontWeightChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises AllowDragDropChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnAllowDragDropChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AllowDragDropChanged != null)
            {
                AllowDragDropChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises SelectedIndexChanged event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnSelectedIndexChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectedIndexChanged != null)
            {
                SelectedIndexChanged(this, e);
            }

            if ((int)e.NewValue != -1 && SelectedIndex != -1)
            {
                TabItemExt item = (TabItemExt)ItemContainerGenerator.ContainerFromIndex(SelectedIndex);
                if (item != null)
                {
                    M_tabItemHeight = item.ActualHeight;
                }
            }
            else
            {
                if (CloseMode == CloseMode.Delete)
                {
                    if (Items.Count > 0)
                    {
                        SelectedIndex = Items.Count - 1;
                    }
                }
                else if (CloseMode == CloseMode.Hide)
                {
                    List<object> visibleitems = new List<object>();

                    foreach (object item in Items)
                    {
                        UIElement element = (UIElement)ItemContainerGenerator.ContainerFromItem(item);
                        if (element !=null && element.Visibility == Visibility.Visible)
                        {
                            visibleitems.Add(element);
                        }
                    }
                    if (visibleitems.Count > 0)
                    {
                        SelectedIndex = visibleitems.Count - 1;
                    }
                }
            }
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Called when some dependencyProperty is changed.
        /// </summary>
        /// <param name="e">Provides data for various property changed events. Typically these events report effective value changes in the value of a read-only dependency property.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == SkinStorage.EnableTouchProperty)
            {
                IsTouchEnabled = (bool)e.NewValue;
            }
            if (SkinStorage.GetVisualStyle(this).ToString() == "VS2010")
            {
                Border border = GetTemplateChild("btborder") as Border;
                if (border != null)
                {
                    ItemCollection items = this.Items;
                    border.BorderThickness = new Thickness(0, 0, 0, 0);
                    if (items != null)
                    {
                        foreach (object obj1 in items)
                        {
                            if (obj1 is TabItemExt)
                            {
                                border.BorderThickness = new Thickness(0, 0, 0, 4);
                                break;
                            }

                        }
                    }
                }
            }
            if (e.Property == SkinStorage.VisualStyleProperty)
            {
                m_SkinChangeFlag = true;
                string newVal = (string)e.NewValue;

                if (newVal.Contains(C_officeStyles) || newVal == C_blend || newVal == C_classic)
                {
                    m_dragMarkerStyle = C_office2007DragMarkerStyle;
                    m_dragMarkerStyleName = C_office2007DragMarkerStyleName;
                }
                else
                {
                    m_dragMarkerStyle = C_office2008DragMarkerStyle;
                    m_dragMarkerStyleName = C_office2008DragMarkerStyleName;
                }

                if (DragMarkerStyle != null)
                {
                    DragMarkerStyle = DefaultDragMarkerStyle;
                }               

            }
            else if (e.Property == SkinManager.ActiveColorSchemeProperty)
            {

                foreach (object obj in Items)
                {
                    TabItemExt item = obj as TabItemExt;
                    if (item != null)
                    {
                        UIElement element = item.Content as UIElement;

                        if (element != null)
                        {
                            SkinManager.SetActiveColorScheme(element, (SolidColorBrush)e.NewValue);
                        }

                    }
                }
            }
        }

        private static void OnTabVisualStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExt instance = (TabControlExt) d;
            instance.TabVisualStyleChanged(e);
        }

        private void TabVisualStyleChanged(DependencyPropertyChangedEventArgs args)
        {           
            // To get  Excel like TabControl
            ResourceDictionary rd = new ResourceDictionary();
            if (TabVisualStyle == TabVisualStyle.ExcelBlue)
            {
                rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/ExcelBlueStyle.xaml", UriKind.RelativeOrAbsolute);
                RemoveDictionaryIfExist(this, rd);
                this.Resources.MergedDictionaries.Add(rd);
                
            }
            else if (TabVisualStyle == TabVisualStyle.ExcelBlack)
            {
                rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/ExcelBlackStyle.xaml", UriKind.RelativeOrAbsolute);
                RemoveDictionaryIfExist(this, rd);
                this.Resources.MergedDictionaries.Add(rd);
            }
            else if (TabVisualStyle == TabVisualStyle.ExcelSilver)
            {
                rd.Source = new Uri(@"/Syncfusion.Tools.WPF;component/Controls/TabControlExt/Themes/ExcelSilverStyle.xaml", UriKind.RelativeOrAbsolute);
                RemoveDictionaryIfExist(this, rd);
                this.Resources.MergedDictionaries.Add(rd);
            }
        }

        /// <summary>
        /// Called when an internal process or application calls ApplyTemplate, which is used to build the current template's visual tree.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            UpdateGridTemplate();

            if (Items.Count == 0)
            {
                IsAllTabsClosed = true;
            }
            if (SkinStorage.GetVisualStyle(this).ToString() == "VS2010")
            {
                Border border = GetTemplateChild("btborder") as Border;
                if (border != null)
                {
                    ItemCollection items = this.Items;
                    border.BorderThickness = new Thickness(0, 0, 0, 0);
                    if (items != null)
                    {
                        foreach (object obj1 in items)
                        {
                            if (obj1 is TabItemExt)
                            {
                                border.BorderThickness = new Thickness(0, 0, 0, 4);
                                break;
                            }

                        }
                    }
                }
            }
            Border3D border3d = GetTemplateChild("ContentPanel") as Border3D;
            if (border3d != null)
            {
                border3d.MouseMove += new MouseEventHandler(border3d_MouseMove);
#if !SyncfusionFramework3_5
                //if (this.IsTouchEnabled)
                    //border3d.TouchMove += border3d_TouchMove;
#endif
            }
            if (!BrowserInteropHelper.IsBrowserHosted)
            {
                Window window = VisualUtils.FindRootVisual(this) as Window;
                if (window != null)
                {
                    m_prevwindowstate = window.WindowState;
                    m_prevwindowstyle = window.WindowStyle;
                }
            }
            ApplyFullScreen();

            m_ContentPanel = GetTemplateChild("PART_ContentHolder") as Panel;
            UpdateSelectedItem();
        }

        /// <summary>
        /// internal variable which has state flag
        /// </summary>
        bool m_stateflag = false;

        /// <summary>
        /// internal variable which has control mode flag
        /// </summary>
        bool m_controlmode = true;

        /// <summary>
        /// Applies the full screen.
        /// </summary>
        internal void ApplyFullScreen()
        {
            HeaderPanel panel = GetTemplateChild("HeaderPanel") as HeaderPanel;
            if (FullScreenMode != FullScreenMode.None)
            {
                if (panel != null)
                {
                    if (FullScreenMode == FullScreenMode.ControlMode)
                    {
                        panel.Visibility = Visibility.Collapsed;
                        m_controlmode = false;
                    }
                    else
                    {
                        if (!BrowserInteropHelper.IsBrowserHosted)
                        {
                            Window window = VisualUtils.FindRootVisual(this) as Window;
                            if (window != null)
                            {
                                window.WindowState = WindowState.Maximized;
                                window.WindowStyle = WindowStyle.None;
                                m_stateflag = true;
                            }
                        }
                        m_controlmode = true;
                        panel.Visibility = Visibility.Collapsed;
                    }
                }
            }
            else if (panel != null)
            {
                panel.Visibility = Visibility.Visible;
                if (!BrowserInteropHelper.IsBrowserHosted)
                {
                    Window window = VisualUtils.FindRootVisual(this) as Window;
                    if (window != null && m_stateflag)
                    {
                        if (m_stateflag && m_controlmode)
                        {
                            window.WindowStyle = m_prevwindowstyle;
                            window.WindowState = m_prevwindowstate;
                            m_stateflag = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the MouseMove event of the border3d control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void border3d_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.StylusDevice == null || e.StylusDevice != null)
            {
                if (FullScreenMode != FullScreenMode.None)
                {
                    Border3D border = sender as Border3D;
                    HeaderPanel panel = GetTemplateChild("HeaderPanel") as HeaderPanel;
                    double OFFSET = 5;
                    if (border != null && panel != null)
                    {
                        Point point = e.GetPosition(border);
                        if (this.TabStripPlacement == Dock.Top)
                        {
                            if (point.Y <= OFFSET)
                            {
                                panel.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                panel.Visibility = Visibility.Collapsed;
                            }
                        }
                        else if (TabStripPlacement == Dock.Left)
                        {
                            if (point.X <= OFFSET)
                            {
                                panel.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                panel.Visibility = Visibility.Collapsed;
                            }
                        }
                        else if (TabStripPlacement == Dock.Right)
                        {
                            if (point.X >= border.RenderSize.Width - OFFSET)
                            {
                                panel.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                panel.Visibility = Visibility.Collapsed;
                            }
                        }
                        else if (TabStripPlacement == Dock.Bottom)
                        {
                            if (point.Y >= border.RenderSize.Height - OFFSET)
                            {
                                panel.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                panel.Visibility = Visibility.Collapsed;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates tab control flow direction when TabControl is in the DocumentContainer TDI panel.
        /// </summary>
        private void UpdateGridTemplate()
        {
            if (Template != null)
            {
                Grid grid = Template.FindName(C_tabControlGridName, this) as Grid;

                if (grid != null && grid.TemplatedParent is DocumentTabControl)
                {
                    DocumentTabControl docTabControl = (DocumentTabControl)grid.TemplatedParent;
                    TDILayoutPanel panel = docTabControl.Parent as TDILayoutPanel;


                    if (panel != null && panel.FlowDirection != grid.FlowDirection)
                    {
                        grid.FlowDirection = panel.FlowDirection;
                    }

                    TDISplitPanel splitPanel = docTabControl.Parent as TDISplitPanel;

                    if (splitPanel != null && splitPanel.FlowDirection != grid.FlowDirection)
                    {
                        grid.FlowDirection = splitPanel.FlowDirection;
                    }
                }
            }
        }

        /// <summary>
        /// Called when <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true.
        /// </summary>
        /// <param name="e">Provides data for the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            ItemContainerGenerator.StatusChanged += new EventHandler(OnGeneratorStatusChanged);
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own ItemContainer.
        /// </summary>
        /// <param name="item">Specified item.</param>
        /// <returns>
        /// Returns true if the item is its own ItemContainer; otherwise, false.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            DependencyObject obj = GetTabItem(item);

            if (obj != null && TabControlExt.GetContextMenuItems(obj) == null)
            {
                SetContextMenuItems(obj, new ObservableCollection<object>());
            }

            return item is TabItemExt;
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TabItemExt();
        }

        /// <summary>
        /// Represents the selection stack.
        /// </summary>
        internal List<object> selectionStack = new List<object>();

        /// <summary>
        /// Gets the selection stack.
        /// </summary>
        /// <value>The selection stack.</value>
        internal List<object> SelectionStack
        {
            get
            {
                if (selectionStack == null)
                {
                    return selectionStack = new List<object>();
                }
                else
                {
                    return selectionStack;
                }
            }
        }

        /// <summary>
        /// Gets the selected tab item.
        /// </summary>
        /// <returns></returns>
        internal TabItemExt GetSelectedTabItem()
        {
            object selectedItem = SelectedItem;
            if (selectedItem != null)
            {
                TabItemExt tabItem = selectedItem as TabItemExt;
                if (tabItem == null)
                {
                    tabItem = ItemContainerGenerator.ContainerFromIndex(SelectedIndex) as TabItemExt;
                }

                return tabItem;
            }

            return null;
        }
       
        internal bool IsFirstSelected
        {
            get
            {
                return (bool)GetValue(IsFirstSelectedProeprty);
            }
            set
            {
                #if !SyncfusionFramework3_5
                SetCurrentValue(IsFirstSelectedProeprty, value);
#else
                SetValue(IsFirstSelectedProeprty, value);
#endif
            }
        }

        internal static readonly DependencyProperty IsFirstSelectedProeprty =
           DependencyProperty.Register("IsFirstSelected", typeof(bool), typeof(TabControlExt), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));
        /// <summary>
        /// internal variable which has cancel flag
        /// </summary>
        bool m_Cancel = true;

        internal PreviewSelectedItemChangedEventArgs previewselectedargs;

        /// <summary>
        /// Checks the tab panel close button visibility.
        /// </summary>
        /// <param name="closebutton">The closebutton.</param>
        /// <param name="close">if set to <c>true</c> [close].</param>
        /// <param name="behavior">The behavior.</param>
        private void CheckTabPanelCloseButtonVisibility(ToggleButton closebutton, bool close, DisabledButtonsBehavior behavior)
        {
            if (CloseButtonType == CloseButtonType.Common || CloseButtonType == CloseButtonType.Both || CloseButtonType == CloseButtonType.Extended)
            {
                closebutton.Visibility = Visibility.Visible;
            }
            else if (CloseButtonType == CloseButtonType.Hide)
            {
                closebutton.Visibility = Visibility.Hidden;
            }
            else
            {
                closebutton.Visibility = Visibility.Collapsed;
            }
            closebutton.IsEnabled = true;

            if (!close)
            {
                switch (behavior)
                {
                    case DisabledButtonsBehavior.Collapse:
                        closebutton.Visibility = Visibility.Collapsed;
                        break;
                    case DisabledButtonsBehavior.Hide:
                        closebutton.Visibility = Visibility.Hidden;
                        break;
                    case DisabledButtonsBehavior.Disable:
                        closebutton.IsEnabled = false;
                        break;
                }
            }
        }

        /// <summary>
        /// Checks the resize item.
        /// </summary>
        /// <param name="item">The item.</param>
        private void CheckResizeItem(TabItemExt item)
        {
            double diffwidth = item.CalculateTextElementSizeDifference();
            if (!IsLoaded && item.m_defaultHeaderMargin.Left == 0)
                item.m_defaultHeaderMargin = item.HeaderMargin;
            if (!item.IsSelected)
            {
                item.HeaderContainerMargin = new Thickness(Math.Ceiling(item.m_defaultHeaderMargin.Left + diffwidth), 0, 0, 0);
                item.HeaderMargin = new Thickness(item.m_defaultHeaderMargin.Left + diffwidth, item.m_defaultHeaderMargin.Top, item.m_defaultHeaderMargin.Right + diffwidth, item.m_defaultHeaderMargin.Bottom);
            }
            else
            {
                item.HeaderContainerMargin = new Thickness(Math.Ceiling(item.m_defaultHeaderMargin.Left + diffwidth), 0, 0, 0);
                item.HeaderMargin = new Thickness(item.m_defaultHeaderMargin.Left, item.m_defaultHeaderMargin.Top, item.m_defaultHeaderMargin.Right, item.m_defaultHeaderMargin.Bottom);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Controls.Primitives.Selector.SelectionChanged"/> routed event.
        /// </summary>
        /// <param name="e">Provides data for <see cref="T:System.Windows.Controls.SelectionChangedEventArgs"/>.</param>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            if (SkinStorage.GetVisualStyle(this).ToString() == "VS2010")
            {
                if (this.SelectedIndex == 0)

                    this.IsFirstSelected = true;
                else
                    this.IsFirstSelected = false;                
            }
            UpdateSelectedItem();
            TabItemExt oldItem = null;
            TabItemExt newItem = null;
            SelectedItemChangedEventArgs selectedArgs = null;

            if (TabPanel != null && TabLayoutPanel != null
                && TabPanel.Visibility != Visibility.Visible
                && TabLayoutPanel.VisibleItemsCount > 0)
            {
                TabPanel.Visibility = Visibility.Visible;
            }

            if (TabLayoutPanel != null)
            {
                if (!IsDragging && m_Flag)
                {
                    TabLayoutPanel.SelectItemInternal(GetTabItem(SelectedItem));
                }
               
            }

            IsAllTabsClosed = false;

            if (e.RemovedItems.Count > 0)
            {
                oldItem = GetTabItem(e.RemovedItems[0]);

                #region CloseButtonVisibilityChecking

                if (oldItem != null && oldItem.IsLoaded)
                {
                    ContentPresenter presenter = oldItem.Content as ContentPresenter;
                    if (presenter != null && presenter.Content != null)
                    {
                        if (oldItem.Template != null)
                        {
                            ToggleButton closebutton = oldItem.Template.FindName("PART_CloseButton", oldItem) as ToggleButton;
                            if (closebutton != null)
                            {
                                DockingManager owner = DockingManager.ResolveManager(presenter.Content as UIElement);
                                if (owner != null)
                                {
                                    bool close = DockingManager.GetCanClose(presenter.Content as DependencyObject);
                                    oldItem.CheckCloseButtonVisibilityOnLoad(closebutton, close, owner.DisabledCloseButtonsBehavior);
                                }
                                else
                                {
                                    DocumentContainer container = VisualUtils.FindAncestor((Visual)this, typeof(DocumentContainer)) as DocumentContainer;
                                    if (container != null)
                                    {
                                        bool close = DocumentContainer.GetCanClose(presenter.Content as DependencyObject);
                                        oldItem.CheckCloseButtonVisibilityOnLoad(closebutton, close, container.DisabledButtonsBehavior);
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                if (SelectionStack.Contains(oldItem))
                {
                    SelectionStack.Remove(oldItem);
                }

                if (oldItem != null)
                {
                    if(DisableResizeOnSelection && IsLoaded)
                        CheckResizeItem(oldItem);   
                    SelectionStack.Add(oldItem);
                }
            }

            if (e.AddedItems.Count > 0)
            {
                newItem = GetTabItem(e.AddedItems[0]);
            }

            if (!IsDragging)
            {
                if (m_Cancel)
                {
                    if (newItem != null && !newItem.IsNewTab)
                    {
                        if(DisableResizeOnSelection && IsLoaded)
                            CheckResizeItem(newItem);

                        #region CloseButtonVisibilityChecking

                        ContentPresenter presenter = newItem.Content as ContentPresenter;
                        if (presenter != null && presenter.Content != null)
                        {
                            if (this.TabPanel != null && TabPanel.Template != null)
                            {
                                ToggleButton closebutton = TabPanel.Template.FindName("PART_CloseButton", TabPanel) as ToggleButton;
                                if (closebutton != null)
                                {
                                    DockingManager owner = DockingManager.ResolveManager(presenter.Content as UIElement);
                                    if (owner != null)
                                    {
                                        bool close = DockingManager.GetCanClose(presenter.Content as DependencyObject);
                                        CheckTabPanelCloseButtonVisibility(closebutton, close, owner.DisabledCloseButtonsBehavior);
                                    }
                                    else
                                    {
                                        DocumentContainer container = VisualUtils.FindAncestor((Visual)this, typeof(DocumentContainer)) as DocumentContainer;
                                        if (container != null)
                                        {
                                            bool close = DocumentContainer.GetCanClose(presenter.Content as DependencyObject);
                                            CheckTabPanelCloseButtonVisibility(closebutton, close, container.DisabledButtonsBehavior);
                                        }
                                    }
                                }
                            }
                        }

                        #endregion

                        
                        selectedArgs = new SelectedItemChangedEventArgs(oldItem, newItem);
                            FireSelectedItemChangedEvent(selectedArgs);
                            if (selectedArgs.Cancel && oldItem != null)
                            {
                                m_Cancel = false;
                                newItem.IsSelected = false;
                                this.SelectedItem = oldItem;
                                
                            }
                    }
                }
                else
                {
                    m_Cancel = true;
                }
            }

            if (SelectedIndex != -1)
            {
                if (SkinStorage.GetVisualStyle(this) == "Classic")
                {
                    InvalidateMeasure();
                    InvalidateArrange();

                    UIElement element = SelectedItem as UIElement;
                    if (element != null)
                    {
                        element.InvalidateMeasure();
                        element.InvalidateArrange();
                    }
                }
            }

            VerifyZIndex();
        }

        /// <summary>
        /// Called to update the current selection when items change.
        /// </summary>
        /// <param name="e">The event data for the <see cref="E:System.Windows.Controls.ItemContainerGenerator.ItemsChanged"/> event.</param>
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
        
            if (m_ContentPanel == null && IsDisableUnloadTabItemExtContent)
            {
                return;
            }
            if (SkinStorage.GetVisualStyle(this).ToString() == "VS2010")
            {
                Border border = GetTemplateChild("btborder") as Border;
                if (border != null)
                {
                    ItemCollection items = this.Items;
                    border.BorderThickness = new Thickness(0, 0, 0, 0);
                    if (items != null)
                    {

                        foreach (object obj1 in items)
                        {
                            if (obj1 is TabItemExt)
                            {
                                border.BorderThickness = new Thickness(0, 0, 0, 4);
                                break;
                            }

                        }
                    }
                }
            }
            if (IsDisableUnloadTabItemExtContent)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Reset:
                        m_ContentPanel.Children.Clear();
                        break;

                    case NotifyCollectionChangedAction.Add:
                       if (e.NewItems.Count > 0)
                        {
                            SelectedItem = e.NewItems[0];
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                       
                        if (e.OldItems != null)
                        {
                            foreach (var item in e.OldItems)
                            {
                                ContentPresenter cp = FindChildContentPresenter(item);
                                if (cp != null)
                                {
                                    m_ContentPanel.Children.Remove(cp);
                                   
                                }
                            }
                        }

                        UpdateSelectedItem();
                        break;

                    case NotifyCollectionChangedAction.Replace:
                        throw new NotImplementedException("Replace not implemented yet");
                }
            }
            if (e.NewStartingIndex != e.OldStartingIndex)
            {
                base.OnItemsChanged(e);
               
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    if (m_newtabflag)
                    {
                        m_addedItem = e.NewItems[0];
                    }
                    TabItemExt tabitem = (TabItemExt)ItemContainerGenerator.ContainerFromItem(e.NewItems[0]);
                    if (tabitem != null)
                    {
                        if (SelectOnCreatingNewItem && tabitem.TabIndex != Items.IndexOf(e.NewItems[0]))
                        {
                            SelectedItem = e.NewItems[0];
                        }
                    }
                }

                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    TabItemExt previousItem = null; ;
                    int lastSelectedIndex;

                    for (int i = SelectionStack.Count - 1; i >= 0; i--)
                    {
                        previousItem = SelectionStack[i] as TabItemExt;
                        if (previousItem != null && previousItem.IsVisible)
                        {
                            break;
                        }
                    }

                    lastSelected = previousItem;

                    if (lastSelected != null)
                    {
                        lastSelectedIndex = ItemContainerGenerator.IndexFromContainer(lastSelected);
                        if (lastSelectedIndex >= 0)
                            SelectedItem = Items[lastSelectedIndex];
                    }

					if (Items.Count > 0 && (SelectedItem == null || e.OldItems.Contains(SelectedItem)))
					{
						SelectedItem = Items[0];
					}

                    if (Items != null && Items.Count == 0 && TabPanel != null && ((!IsNewButtonEnabled) || (IsNewButtonEnabled && IsNewButtonClosedonNoChild)))
                        TabPanel.Visibility = Visibility.Collapsed;

                    if (e.OldItems.Count > 0 && e.OldItems[0] is TabItemExt)
                    {
                        if (previewselectedargs != null && previewselectedargs.OldSelectedItem == e.OldItems[0] as TabItemExt)
                            previewselectedargs.OldSelectedItem = null;
                        if (previewselectedargs != null && previewselectedargs.NewSelectedItem == e.OldItems[0] as TabItemExt)
                            previewselectedargs.NewSelectedItem = null;
                        if (ActivatedItem == e.OldItems[0] as TabItemExt)
                            ActivatedItem = null;
                        if (lastSelected == e.OldItems[0] as TabItemExt)
                            lastSelected = null;
                    }
                    
                }

                if (SkinStorage.GetVisualStyle(this) == "Default" || SkinStorage.GetVisualStyle(this) == "OneNote")
                {
                    VerifyZIndex();

                }
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    if (e.NewItems[0] is TabItemExt)
                    {
                        foreach (TabItemExt element in e.NewItems)
                        {
                            FrameworkElement cont = element.Content as FrameworkElement;
                            if (cont != null)
                            {
                                cont.AllowDrop = true;
                            }
                        }
                    }
                    else
                    {
                        foreach (object obj in e.NewItems)
                        {
                            TabItemExt element = GetTabItem(obj);
                            if (element != null)
                            {
                                FrameworkElement cont = element.Content as FrameworkElement;
                                if (cont != null)
                                {
                                    cont.AllowDrop = true;
                                }
                            }
                        }

                    }
                }
                if (ItemsChanged != null)
                {
                    ItemsChanged(this, e);
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (Items.Count == 0)
                {
                    if (selectionStack.Count > 0)
                    {
                        selectionStack.Clear();
                    }
                    LocalValueEnumerator locallySetProperties = this.GetLocalValueEnumerator();
                    while (locallySetProperties.MoveNext())
                    {
                        DependencyProperty propertyToClear = locallySetProperties.Current.Property;
                        if (propertyToClear.Name == "SelectedItem")
                        {
                            this.ClearValue(propertyToClear);
                        }
                    }
                    if (previewselectedargs != null)
                    {
                        previewselectedargs.OldSelectedItem = null;
                        previewselectedargs.NewSelectedItem = null;
                    }
                    lastSelected = null;
                }
            }
        }


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
        /// Called when the source of an item in a selector changes.
        /// </summary>
        /// <param name="oldValue">Old value of the source.</param>
        /// <param name="newValue">New value of the source.</param>
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            CoerceValue(AllowDragDropProperty);

            if (SkinStorage.GetVisualStyle(this) == "Default" || SkinStorage.GetVisualStyle(this) == "OneNote")
            {
                VerifyZIndex();
            }

            if (this.Items.Count > 0)
            {
                this.SelectedItem = this.Items[0];
            }
        }
        #endregion

        #region Dependency properties

        /// <summary>
        /// Represents the HideHeaderOnSingleChild Dependency Property
        /// </summary>
        public static readonly DependencyProperty HideHeaderOnSingleChildProperty =
          DependencyProperty.Register("HideHeaderOnSingleChild", typeof(bool), typeof(TabControlExt), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnHideHeaderOnSingleChildChanged)));

        /// <summary>
        /// Represents the DefaultContextMenuItemVisiblity Dependency Property
        /// </summary>
        public static readonly DependencyProperty DefaultContextMenuItemVisibilityProperty =
          DependencyProperty.Register("DefaultContextMenuItemVisibility", typeof(Visibility), typeof(TabControlExt), new FrameworkPropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Represents the AllowDragDropProperty Dependency property
        /// </summary>
        public static readonly DependencyProperty AllowDragDropProperty =
            DependencyProperty.Register("AllowDragDrop", typeof(bool), typeof(TabControlExt), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnAllowDragDropChanged), new CoerceValueCallback(OnCoerceAllowDragDrop)));

        /// <summary>
        /// Represents the TabListContextMenuItemTemplateProperty Dependency property
        /// </summary>
        public static readonly DependencyProperty TabListContextMenuItemTemplateProperty =
            DependencyProperty.Register("TabListContextMenuItemTemplate", typeof(DataTemplate), typeof(TabControlExt), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnTabListContextMenuItemTemplateChanged)));

        /// <summary>
        /// Represents the TabListContextMenuStyle Dependency property
        /// </summary>
        public static readonly DependencyProperty TabListContextMenuStyleProperty =
          DependencyProperty.Register("TabListContextMenuStyle", typeof(Style), typeof(TabControlExt), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Represents the TabListContextMenuTemplate Dependency property
        /// </summary>
        public static readonly DependencyProperty TabListContextMenuTemplateProperty =
         DependencyProperty.Register("TabListContextMenuTemplate", typeof(ControlTemplate), typeof(TabControlExt), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Represents the TabListContextMenuItemStyle Dependency property
        /// </summary>
        public static readonly DependencyProperty TabListContextMenuItemStyleProperty =
         DependencyProperty.Register("TabListContextMenuItemStyle", typeof(Style), typeof(TabControlExt), new FrameworkPropertyMetadata(null,FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Represents the NewButtonTemplateProperty Dependency property
        /// </summary>
        public static readonly DependencyProperty NewButtonTemplateProperty =
          DependencyProperty.Register("NewButtonTemplate", typeof(ControlTemplate), typeof(TabControlExt), new FrameworkPropertyMetadata(null));
        /// <summary>
        /// Represents the NewButtonBackground Property
        /// </summary>
        public static readonly DependencyProperty NewButtonBackgroundProperty =
         DependencyProperty.Register("NewButtonBackground", typeof(Brush), typeof(TabControlExt), new FrameworkPropertyMetadata(null));
        /// <summary>
        /// Represents the NewButtonBorderThickness Dependency property
        /// </summary>
        public static readonly DependencyProperty NewButtonBorderThicknessProperty =
        DependencyProperty.Register("NewButtonBorderThickness", typeof(Thickness), typeof(TabControlExt), new FrameworkPropertyMetadata(null));
        /// <summary>
        /// Represents the NewButtonStyle Dependency property
        /// </summary>
        public static readonly DependencyProperty NewButtonAlignmentProperty =
         DependencyProperty.Register("NewButtonAlignment", typeof(NewButtonAlignment), typeof(TabControlExt), new FrameworkPropertyMetadata(NewButtonAlignment.Last, new PropertyChangedCallback(OnNewButtonAlignmentChange)));
        /// <summary>
        /// Represents the NewButtonAlignment
        /// </summary>
        public static readonly DependencyProperty NewButtonStyleProperty =
         DependencyProperty.Register("NewButtonStyle", typeof(Style), typeof(TabControlExt), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Represents the TabItemSelectedBackground Property
        /// </summary>
        public static readonly DependencyProperty TabItemSelectedBackgroundProperty =
         DependencyProperty.Register("TabItemSelectedBackground", typeof(Brush), typeof(TabControlExt), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Represents the TabItemSelectedForeground Property
        /// </summary>
        public static readonly DependencyProperty TabItemSelectedForegroundProperty =
         DependencyProperty.Register("TabItemSelectedForeground", typeof(Brush), typeof(TabControlExt), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Represents the TabItemSelectedBorderBrush Property
        /// </summary>
        public static readonly DependencyProperty TabItemSelectedBorderBrushProperty =
         DependencyProperty.Register("TabItemSelectedBorderBrush", typeof(Brush), typeof(TabControlExt), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Represents the TabItemHoverBackground Property
        /// </summary>
        public static readonly DependencyProperty TabItemHoverBackgroundProperty =
         DependencyProperty.Register("TabItemHoverBackground", typeof(Brush), typeof(TabControlExt), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Represents the TabItemHoverBorderBrush Property
        /// </summary>
        public static readonly DependencyProperty TabItemHoverBorderBrushProperty =
         DependencyProperty.Register("TabItemHoverBorderBrush", typeof(Brush), typeof(TabControlExt), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Represents the TabItemSelectedForeground Property
        /// </summary>
        public static readonly DependencyProperty TabItemHoverForegroundProperty =
         DependencyProperty.Register("TabItemHoverForeground", typeof(Brush), typeof(TabControlExt), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Represents the ScrollingTimeProperty Dependency property
        /// </summary>
        public static readonly DependencyProperty ScrollingTimeProperty =
            DependencyProperty.Register("ScrollingTime", typeof(int), typeof(TabControlExt), new FrameworkPropertyMetadata(100, new PropertyChangedCallback(OnScrollingTimeChanged)));

        /// <summary>
        /// Represents the IsAllTabsClosedPropertyKey Dependency property
        /// </summary>
        protected static readonly DependencyPropertyKey IsAllTabsClosedPropertyKey =
            DependencyProperty.RegisterReadOnly("IsAllTabsClosed", typeof(bool), typeof(TabControlExt), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Represents the IsAllTabsClosedProperty Dependency property
        /// </summary>
        public static readonly DependencyProperty IsAllTabsClosedProperty = IsAllTabsClosedPropertyKey.DependencyProperty;

        /// <summary>
        /// Represents the DragMarkerStyleProperty Dependency property
        /// </summary>
        public static readonly DependencyProperty DragMarkerStyleProperty =
            DependencyProperty.Register("DragMarkerStyle", typeof(Style), typeof(TabControlExt), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDragMarkerStyleChanged)));

        /// <summary>
        /// Represents the FullScreenMode Dependency property
        /// </summary>
        public static readonly DependencyProperty FullScreenModeProperty =
            DependencyProperty.Register("FullScreenMode", typeof(FullScreenMode), typeof(TabControlExt), new FrameworkPropertyMetadata(FullScreenMode.None, new PropertyChangedCallback(OnFullScreenModeChanged)));

        /// <summary>
        /// Represents the EditHeaderItemStyle Dependency property
        /// </summary>
        public static readonly DependencyProperty EditHeaderItemStyleProperty =
            DependencyProperty.Register("EditHeaderItemStyle", typeof(Style), typeof(TabControlExt), new FrameworkPropertyMetadata(null)); //new PropertyChangedCallback(OnFullScreenModeChanged)));

        /// <summary>
        /// Represents the ToolBarTray Dependency property
        /// </summary>
        public static readonly DependencyProperty ToolBarTrayProperty =
            DependencyProperty.Register("ToolBarTray", typeof(ToolBarTray), typeof(TabControlExt), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnFullScreenModeChanged)));


        /// <summary>
        ///  Represents the DragMarkerColorProperty Dependency property
        /// </summary>
        public static readonly DependencyProperty DragMarkerColorProperty =
            DependencyProperty.Register("DragMarkerColor", typeof(Brush), typeof(TabControlExt), new FrameworkPropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnDragMarkerColorChanged)));

        /// <summary>
        ///  Represents the IsAllTabsClosedProperty Dependency property
        /// </summary>
        public static readonly DependencyProperty CustomEditableTemplateProperty =
            DependencyProperty.RegisterAttached("CustomEditableTemplate", typeof(DataTemplate), typeof(TabControlExt), new FrameworkPropertyMetadata(null));

        /// <summary>
        ///  Represents the UseCustomEditableTemplateProperty Dependency property
        /// </summary>
        public static readonly DependencyProperty UseCustomEditableTemplateProperty =
            DependencyProperty.RegisterAttached("UseCustomEditableTemplate", typeof(bool), typeof(TabControlExt), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Represents the IsEditingPropertyKey 
        /// </summary>
        protected static readonly DependencyPropertyKey IsEditingPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("IsEditing", typeof(bool), typeof(TabControlExt), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Represents the IsEditingProperty Dependency property
        /// </summary>
        public static readonly DependencyProperty IsEditingProperty = IsEditingPropertyKey.DependencyProperty;

        /// <summary>
        /// Represents the EnableLabelEditProperty Dependency property
        /// </summary>
        public static readonly DependencyProperty EnableLabelEditProperty =
            DependencyProperty.Register("EnableLabelEdit", typeof(bool), typeof(TabControlExt), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnEnableLabelEditChanged)));

        /// <summary>
        /// Represents the TabPanelItemProperty Dependency property
        /// </summary>
        public static readonly DependencyProperty TabPanelItemProperty =
            DependencyProperty.Register("TabPanelItem", typeof(object), typeof(TabControlExt), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnTabPanelItemChanged)));

        /// <summary>
        /// Represents the IsDisableUnLoadTabItemExtContent dependency property
        /// </summary>
        public static readonly DependencyProperty IsDisableUnloadTabItemExtContentProperty =
            DependencyProperty.Register("IsDisableUnloadTabItemExtContent", typeof(bool), typeof(TabControlExt), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Represents the IsFocus dependency property
        /// </summary>
        internal static readonly DependencyProperty IsFocusProperty =
            DependencyProperty.Register("IsFocus", typeof(bool), typeof(TabControlExt), new FrameworkPropertyMetadata(true,FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Represents the IsTabGroupFocus dependency property
        /// </summary>
        internal static readonly DependencyProperty IsTabGroupFocusProperty =
            DependencyProperty.Register("IsTabGroupFocus", typeof(bool), typeof(TabControlExt), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Represents the IsDisableUnLoadTabItemExtContent dependency property
        /// </summary>
        public static readonly DependencyProperty SelectOnCreatingNewItemProperty =
            DependencyProperty.Register("SelectOnCreatingNewItem", typeof(bool), typeof(TabControlExt), new FrameworkPropertyMetadata(true));



        /// <summary>
        /// Represents the ShowTabListContextMenuProperty Dependency property
        /// </summary>
        public static readonly DependencyProperty ShowTabListContextMenuProperty =
            DependencyProperty.Register("ShowTabListContextMenu", typeof(bool), typeof(TabControlExt), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnShowTabListContextMenuChanged)));

        /// <summary>
        /// Represents the ContextMenuItemsPropertyKey
        /// </summary>
        public static readonly DependencyPropertyKey ContextMenuItemsPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("ContextMenuItems", typeof(ObservableCollection<object>), typeof(TabControlExt), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// ContextMenuItems Dependency property key.
        /// </summary>
        public static readonly DependencyProperty ContextMenuItemsProperty = ContextMenuItemsPropertyKey.DependencyProperty;

        /// <summary>
        /// Represents the ShowTabItemContextMenuProperty
        /// </summary>
        public static readonly DependencyProperty ShowTabItemContextMenuProperty =
            DependencyProperty.Register("ShowTabItemContextMenu", typeof(bool), typeof(TabControlExt), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnShowTabItemContextMenuChanged)));

        /// <summary>
        ///  Represents the CloseButtonTypeProperty
        /// </summary>
        public static readonly DependencyProperty CloseButtonTypeProperty =
            DependencyProperty.Register("CloseButtonType", typeof(CloseButtonType), typeof(TabControlExt), new FrameworkPropertyMetadata(CloseButtonType.Individual, new PropertyChangedCallback(OnCloseButtonTypeChanged)));

        /// <summary>
        /// Represents the ImageProperty
        /// </summary>
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.RegisterAttached("Image", typeof(ImageSource), typeof(TabControlExt), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Represents the MenuIconProperty
        /// </summary>
        public static readonly DependencyProperty MenuIconProperty =
           DependencyProperty.RegisterAttached("MenuIcon", typeof(ImageSource), typeof(TabControlExt), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Represents the IsNewButtonEnabledProperty
        /// </summary>
        public static readonly DependencyProperty IsNewButtonEnabledProperty =
           DependencyProperty.RegisterAttached("IsNewButtonEnabled", typeof(bool), typeof(TabControlExt), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsNewButtonEnabledChanged)));

        public static readonly DependencyProperty IsNewButtonClosedonNoChildProperty = DependencyProperty.RegisterAttached("IsNewButtonClosedonNoChild", typeof(bool), typeof(TabControlExt), new FrameworkPropertyMetadata(true));


        /// <summary>
        /// Represents the ImageHeightProperty
        /// </summary>
        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.RegisterAttached("ImageHeight", typeof(double), typeof(TabControlExt), new FrameworkPropertyMetadata(double.NaN));

        /// <summary>
        /// Represents the ImageWidthProperty
        /// </summary>
        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.RegisterAttached("ImageWidth", typeof(double), typeof(TabControlExt), new FrameworkPropertyMetadata(double.NaN));

        /// <summary>
        ///  Represents the HoverBackgroundProperty
        /// </summary>
        public static readonly DependencyProperty HoverBackgroundProperty =
            DependencyProperty.RegisterAttached("HoverBackground", typeof(Brush), typeof(TabControlExt), new FrameworkPropertyMetadata(null));

        /// <summary>
        ///  Represents the HoverBackgroundProperty
        /// </summary>
        public static readonly DependencyProperty TabPanelBackgroundProperty =
            DependencyProperty.RegisterAttached("TabPanelBackground", typeof(Brush), typeof(TabControlExt), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnTabPanelBackgroundChanged)));

        /// <summary>
        ///  Represents the HotTrackingEnabledProperty
        /// </summary>
        public static readonly DependencyProperty HotTrackingEnabledProperty =
            DependencyProperty.Register("HotTrackingEnabled", typeof(bool), typeof(TabControlExt), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnHotTrackingEnabledChanged)));

        /// <summary>
        /// Represents the ImageAlignmentProperty
        /// </summary>
        public static readonly DependencyProperty ImageAlignmentProperty =
            DependencyProperty.RegisterAttached("ImageAlignment", typeof(ImageAlignment), typeof(TabControlExt), new FrameworkPropertyMetadata(ImageAlignment.LeftOfText));

        /// <summary>
        /// Represents the RotateTextWhenVerticalProperty
        /// </summary>
        public static readonly DependencyProperty RotateTextWhenVerticalProperty =
            DependencyProperty.Register("RotateTextWhenVertical", typeof(bool), typeof(TabControlExt), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnRotateTextWhenVerticalChanged), new CoerceValueCallback(OnRotateTextWhenVertical)));

        /// <summary>
        /// Represents the TabPanelTemplateProperty
        /// </summary>
        public static readonly DependencyProperty TabPanelTemplateProperty =
            DependencyProperty.Register("TabPanelTemplate", typeof(ControlTemplate), typeof(TabControlExt), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnTabPanelTemplateChanged)));

        /// <summary>
        ///  Represents the TabPanelStyleProperty
        /// </summary>
        public static readonly DependencyProperty TabPanelStyleProperty =
            DependencyProperty.Register("TabPanelStyle", typeof(Style), typeof(TabControlExt), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnTabPanelStyleChanged)));

        /// <summary>
        /// Represents the TabItemLayoutProperty
        /// </summary>
        public static readonly DependencyProperty TabItemLayoutProperty =
            DependencyProperty.Register("TabItemLayout", typeof(TabItemLayoutType), typeof(TabControlExt), new FrameworkPropertyMetadata(TabItemLayoutType.SingleLine, new PropertyChangedCallback(OnTabItemLayoutChanged), new CoerceValueCallback(OnCoerceTabItemLayout)));

        /// <summary>
        /// Identifies TabControlExt.IsTouchEnabled dependency property.
        /// </summary>
        internal static readonly DependencyProperty IsTouchEnabledProperty =
            DependencyProperty.Register("IsTouchEnabled", typeof(bool), typeof(TabControlExt), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Represents the TabItemSizeProperty
        /// </summary>
        public static readonly DependencyProperty TabItemSizeProperty =
            DependencyProperty.Register("TabItemSize", typeof(TabItemSizeMode), typeof(TabControlExt), new FrameworkPropertyMetadata(TabItemSizeMode.Normal, new PropertyChangedCallback(OnTabItemSizeChanged)));

        /// <summary>
        ///  Represents the TabScrollButtonVisibilityProperty
        /// </summary>
        public static readonly DependencyProperty TabScrollButtonVisibilityProperty =
            DependencyProperty.Register("TabScrollButtonVisibility", typeof(TabScrollButtonVisibility), typeof(TabControlExt), new FrameworkPropertyMetadata(TabScrollButtonVisibility.Auto, new PropertyChangedCallback(OnTabScrollButtonVisibilityChanged)));

        /// <summary>
        /// Represents the TabScrollStyleProperty
        /// </summary>
        public static readonly DependencyProperty TabScrollStyleProperty =
            DependencyProperty.Register("TabScrollStyle", typeof(TabScrollStyle), typeof(TabControlExt), new FrameworkPropertyMetadata(TabScrollStyle.Normal, new PropertyChangedCallback(OnTabScrollStyleChanged)));

        /// <summary>
        ///  Represents the KeepTabInFrontProperty
        /// </summary>
        public static readonly DependencyProperty KeepTabInFrontProperty =
            DependencyProperty.Register("KeepTabInFront", typeof(bool), typeof(TabControlExt), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnKeepTabInFrontChanged)));

        /// <summary>
        /// Represents the DisableResizeOnSelectionProperty
        /// </summary>
        public static readonly DependencyProperty DisableResizeOnSelectionProperty =
             DependencyProperty.Register("DisableResizeOnSelection", typeof(bool), typeof(TabControlExt), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnDisableResizeOnSelectionChanged)));

        /// <summary>
        /// Represents the MenuItemCloseOtherTabsNameProperty
        /// </summary>
        public static readonly DependencyProperty MenuItemCloseOtherTabsNameProperty =
             DependencyProperty.Register("MenuItemCloseOtherTabsName", typeof(string), typeof(TabControlExt), new FrameworkPropertyMetadata(MENU_CLOSE_OTHER_NAME));

        /// <summary>
        /// Represents the MenuItemCloseTabsNameProperty
        /// </summary>
        public static readonly DependencyProperty MenuItemCloseTabsNameProperty =
             DependencyProperty.Register("MenuItemCloseTabsName", typeof(string), typeof(TabControlExt), new FrameworkPropertyMetadata(MENU_CLOSE_NAME));

        /// <summary>
        /// Represents the MenuItemCloseAllTabsNameProperty
        /// </summary>
        public static readonly DependencyProperty MenuItemCloseAllTabsNameProperty =
             DependencyProperty.Register("MenuItemCloseAllTabsName", typeof(string), typeof(TabControlExt), new FrameworkPropertyMetadata(MENU_CLOSE_ALL_NAME));

        /// <summary>
        /// Represents the IsCustomTabItemContextMenuEnabledProperty 
        /// </summary>
        public static readonly DependencyProperty IsCustomTabItemContextMenuEnabledProperty =
             DependencyProperty.Register("IsCustomTabItemContextMenuEnabled", typeof(bool), typeof(TabControlExt), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Represents the CollapseDefaultTabListContextMenuItemsProperty 
        /// </summary>
        public static readonly DependencyProperty CollapseDefaultTabListContextMenuItemsProperty =
             DependencyProperty.Register("CollapseDefaultTabListContextMenuItems", typeof(bool), typeof(TabControlExt), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Represents the TabListContextMenuProperty 
        /// </summary>
        public static readonly DependencyProperty TabListContextMenuItemsProperty =
             DependencyProperty.Register("TabListContextMenuItems", typeof(DocumentTabItemMenuItemCollection), typeof(TabControlExt), new FrameworkPropertyMetadata(new DocumentTabItemMenuItemCollection()));

        /// <summary>
        /// Represents the SelectedItemFontWeightProperty
        /// </summary>
        public static readonly DependencyProperty SelectedItemFontWeightProperty =
            DependencyProperty.Register("SelectedItemFontWeight", typeof(FontWeight), typeof(TabControlExt), new FrameworkPropertyMetadata(FontWeights.SemiBold, new PropertyChangedCallback(OnSelectedItemFontWeightChanged)));

        /// <summary>
        /// Represents the IsCloseTabProcessEnabledProperty
        /// </summary>
        internal static readonly DependencyProperty IsCloseTabProcessEnabledProperty =
            DependencyProperty.Register("IsCloseTabProcessEnabled", typeof(bool), typeof(TabControlExt), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnIsCloseTabProcessEnabledChanged)));
        /// <summary>
        /// Represents the ScrollingButtonStyleProperty
        /// </summary>
        public static readonly DependencyProperty ScrollingButtonStyleProperty =
         DependencyProperty.Register("ScrollingButtonStyle", typeof(Style), typeof(TabControlExt), new FrameworkPropertyMetadata(null));




        public TabVisualStyle TabVisualStyle
        {
            get { return (TabVisualStyle)GetValue(TabVisualStyleProperty); }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(TabVisualStyleProperty, value); 
#else
                SetValue(TabVisualStyleProperty, value);
#endif
            }
        }

        // Using a DependencyProperty as the backing store for TabVisualStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TabVisualStyleProperty =
            DependencyProperty.Register("TabVisualStyle", typeof(TabVisualStyle), typeof(TabControlExt), new PropertyMetadata(TabVisualStyle.None, new PropertyChangedCallback(OnTabVisualStyleChanged)));



        public bool IsLazyLoaded
        {
            get { return (bool)GetValue(IsLazyLoadedProperty); }
            set
            {
#if !SyncfusionFramework3_5
                SetCurrentValue(IsLazyLoadedProperty, value); 
#else
                SetValue(IsLazyLoadedProperty, value);
#endif
            }
        }

        // Using a DependencyProperty as the backing store for IsLazyLoaded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLazyLoadedProperty =
            DependencyProperty.Register("IsLazyLoaded", typeof(bool), typeof(TabControlExt), new PropertyMetadata(false));

        

        #endregion
    }

    /// <summary>
    /// collection of new button alignment
    /// </summary>
    public enum NewButtonAlignment
    {
        /// <summary>
        /// Represent the first
        /// </summary>
        First,
        /// <summary>
        /// Represent the last
        /// </summary>
        Last
    }

    /// <summary>
    /// collection of full screnn mode
    /// </summary>
    public enum FullScreenMode
    {
        /// <summary>
        /// Represent the control mode
        /// </summary>
        ControlMode,
        /// <summary>
        /// represent the window mode
        /// </summary>
        WindowMode,
        /// <summary>
        /// represents none
        /// </summary>
        None
    }

    public enum TabVisualStyle
    {
        None,
        ExcelBlue,
        ExcelBlack,
        ExcelSilver,
    }


    #region Custom Events

    /// <summary>
    /// Class Represents the TabContolExt Event Args
    /// </summary>
    public class TabControlExtDragEventArgs : RoutedEventArgs
    {
        #region Member

        /// <summary>
        /// Stores the data element.
        /// </summary>
        private DataObject m_data = null;

        /// <summary>
        /// Stores the target element.
        /// </summary>
        private FrameworkElement m_target;

        /// <summary>
        /// Stores the source element.
        /// </summary>
        private FrameworkElement m_source;

        /// <summary>
        /// Stores a bool value.
        /// </summary>
        private bool m_cancel;

        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="TabControlExtDragEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The routed event.</param>
        public TabControlExtDragEventArgs(RoutedEvent routedEvent)
            : base(routedEvent)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the data.
        /// </summary>        
        public DataObject Data
        {
            get
            {
                return m_data;
            }

            set
            {
                if (value != m_data)
                {
                    m_data = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the drop source.
        /// </summary>
        /// <value>The drop source.</value>
        public FrameworkElement DropSource
        {
            get
            {
                return m_target;
            }

            set
            {
                if (value != m_target)
                {
                    m_target = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the drag source.
        /// </summary>
        /// <value>The drag source.</value>
        public FrameworkElement DragSource
        {
            get
            {
                return m_source;
            }

            set
            {
                if (value != m_source)
                {
                    m_source = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TabControlExtDragEventArgs"/> is cancel.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel
        {
            get
            {
                return m_cancel;
            }

            set
            {
                if (value != m_cancel)
                {
                    m_cancel = value;
                }
            }
        }
        #endregion
    }
    #endregion
}