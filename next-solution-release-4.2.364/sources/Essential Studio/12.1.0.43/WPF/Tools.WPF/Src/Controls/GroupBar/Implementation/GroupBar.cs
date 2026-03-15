// <copyright file="GroupBar.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

#region file using
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;
using System.Linq;
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the GroupBar UI element.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(true)]
#endif
	
	 [SkinType(SkinVisualStyle = Skin.Office2007Blue,
   Type = typeof(GroupBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(GroupBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(GroupBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
  Type = typeof(GroupBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(GroupBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(GroupBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
     Type = typeof(GroupBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
     Type = typeof(GroupBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
     Type = typeof(GroupBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
     Type = typeof(GroupBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(GroupBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(GroupBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/DefaultStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
     Type = typeof(GroupBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
    Type = typeof(GroupBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent ,
  Type = typeof(GroupBar), XamlResource = "/Syncfusion.Tools.Wpf;component/Controls/GroupBar/Themes/TransparentStyle.xaml")]
   
	public class GroupBar : ItemsControl,IDisposable
    {
        #region Constants
        /// <summary>
        /// The height (for vertical layout) or the width (for horizontal layout) of
        /// the area, that hosts the content of the selected <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/>.
        /// </summary>
        private const double DEF_ITEMCONTENT_LENGTH = 0d;

        /// <summary>
        /// Default height for all <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> headers.
        /// </summary>
        private const double DEF_HEADER_HEIGHT = 50d;

        /// <summary>
        /// Default height for the <see cref="Syncfusion.Windows.Tools.Controls.NavigationToolbar"/>.
        /// </summary>
        private const double DEF_TOOLBAR_HEIGHT = 50d;

        /// <summary>
        /// Default orientation of the control.
        /// </summary>
        private const Orientation DEF_DEFAULT_ORIENTATION = Orientation.Vertical;

        /// <summary>
        /// Minimal height for the view content.
        /// </summary>
        private const double DEF_VIEWCONTENT_HEIGHT = 32d;

        /// <summary>
        /// Null index for the collection.
        /// </summary>
        private const int C_nullIndex = -1;

        /// <summary>
        /// Name of the property to sort the <see cref="GroupBar"/> list by.
        /// </summary>
        private const string C_sortGroupBarProperty = "HeaderText";

        /// <summary>
        /// Name of the property to sort the GroupView list by.
        /// </summary>
        private const string C_sortGroupViewProperty = "Text";

        /// <summary>
        /// Name of the format to use in the clipboard.
        /// </summary>
        private const string C_formatGroupViewItem = "GroupViewItem";

        /// <summary>
        /// Name of the Default (VS2005) visual style.
        /// </summary>
        private const string C_nameDefaultVisualStyle = "Default";

        /// <summary>
        /// Name of the Office2003 visual style.
        /// </summary>
        private const string C_nameOffice2003VisualStyle = "Office2003";

        /// <summary>
        /// Name of the Blend visual style.
        /// </summary>
        private const string C_nameBlendVisualStyle = "Blend";

        /// <summary>
        /// Name of the LunaNormalColor visual style.
        /// </summary>
        private const string C_nameLunaNormalColorVisualStyle = "Luna.NormalColor";

        /// <summary>
        /// Name of the LunaHomestead visual style.
        /// </summary>
        private const string C_nameLunaHomesteadVisualStyle = "Luna.Homestead";

        /// <summary>
        /// Name of the LunaMetallic visual style.
        /// </summary>
        private const string C_nameLunaMetallicVisualStyle = "Luna.Metallic";

        /// <summary>
        /// Name of the RoyaleNormalColor visual style.
        /// </summary>
        private const string C_nameRoyaleNormalColorVisualStyle = "Royale.NormalColor";

        /// <summary>
        /// Name of the ZuneNormalColor visual style.
        /// </summary>
        private const string C_nameZuneNormalColorVisualStyle = "Zune.NormalColor";

        /// <summary>
        /// Name of the AeroNormalColor visual style.
        /// </summary>
        private const string C_nameAeroNormalColorVisualStyle = "Aero.NormalColor";

        /// <summary>
        /// Name of the Office2007Blue visual style.
        /// </summary>
        private const string C_nameOffice2007BlueVisualStyle = "Office2007Blue";

        /// <summary>
        /// Name of the Office2007Black visual style.
        /// </summary>
        private const string C_nameOffice2007BlackVisualStyle = "Office2007Black";

        /// <summary>
        /// Name of the Office2007Silver visual style.
        /// </summary>
        private const string C_nameOffice2007SilverVisualStyle = "Office2007Silver";

        /// <summary>
        /// Name of the main host from the template.
        /// </summary>
        private const string C_nameMainHost = "MainHost";

        /// <summary>
        /// Message for the main host if it is not found.
        /// </summary>
        private const string C_errorMainHost = "MainHost is not found";

        /// <summary>
        /// Name of the content host from the template.
        /// </summary>
        private const string C_nameContentHost = "ContentHost";

        /// <summary>
        /// Message for the content host if it is not found.
        /// </summary>
        private const string C_errorContentHost = "ContentHost is not found";

        /// <summary>
        /// Name of the navigation toolbar from the template.
        /// </summary>
        private const string C_nameNavigationToolbar = "NavigationToolbar";

        /// <summary>
        /// Message for the navigation toolbar if it is not found.
        /// </summary>
        private const string C_errorNavigationToolbar = "NavigationToolbar is not found";

        /// <summary>
        /// Name of the splitter from the template.
        /// </summary>
        private const string C_nameSplitter = "Splitter";

        /// <summary>
        /// Message for the splitter if it is not found.
        /// </summary>
        private const string C_errorSplitter = "Splitter is not found";

        /// <summary>
        /// Name of the scroll viewer from the template.
        /// </summary>
        private const string C_nameScrollViewer = "ScrollViewer";

        /// <summary>
        /// Message for the scroll viewer if it is not found.
        /// </summary>
        private const string C_errorScrollViewer = "ScrollViewer is not found";

        /// <summary>
        /// Name of the collapse button from the template.
        /// </summary>
        private const string C_nameCollapseButton = "CollapseButton";

        /// <summary>
        /// Message for the collapse button if it is not found.
        /// </summary>
        private const string C_errorCollapseButton = "CollapseButton is not found";

        /// <summary>
        /// Name of the popup from the template.
        /// </summary>
        private const string C_namePopup = "PART_Popup";

        /// <summary>
        /// Name of the content button from the template.
        /// </summary>
        private const string C_nameContentButton = "CollapsedContentToggleButton";

        /// <summary>
        /// Name of the resize border of the popup.
        /// </summary>
        private const string C_nameResizeBorder = "PopupResizePart";

        /// <summary>
        /// Name of the HeaderContent in GroupBarItem.
        /// </summary>
        private const string C_nameHeaderContent = "HeaderContent";

        /// <summary>
        /// Name of the StackItemsHost border element.
        /// </summary>
        private const string C_stackItemsHost = "StackItemsHost";

        /// <summary>
        /// Name of the ItemsHost border.
        /// </summary>
        private const string C_itemsHost = "ItemsHost";

        /// <summary>
        /// Default width of the collapsed GroupBar.
        /// </summary>
        private const double C_defaultCollapsedWidth = 34.0;

        /// <summary>
        /// Default popup margin.
        /// </summary>
        private const int C_defaultMargin = 7;

        /// <summary>
        /// Contains default visual style key name.
        /// </summary>
        private const string C_defaultVisStyleName = "Default";

        /// <summary>
        /// Presents file name to reset the internal isolated storage.
        /// </summary>
        private const string C_ResetStoreFileName = "reset.dat";

        /// <summary>
        /// Contains path to default image.
        /// </summary>
        private const string C_pathToDefaultImage = "pack://application:,,,/Syncfusion.Tools.WPF;component/Controls/GroupBar/Resources/DefaultItemImage.png";

        private const string C_headerElement = "HeaderHost";
        #endregion

        #region Private members

        /// <summary>
        /// Contains default size of the popup. This size is used when size of the popup content cannot be defined.
        /// </summary>
        private Size m_defaultPopupSize = new Size(300, 300);

        /// <summary>
        /// Contains ItemsHost border.
        /// </summary>
        private Border m_itemsHost = null;

        /// <summary>
        /// List of indices of the hidden items.
        /// </summary>
        private readonly List<int> m_hiddenIndices = new List<int>();

        /// <summary>
        /// Indicates whether control template is changing.
        /// </summary>
        private bool m_templateChanging = false;

        /// <summary>
        /// Splitter that used for showing wanted count of the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> 
        /// objects in the <see cref="GroupBar"/> when <see cref="VisualMode"/> property is set to VisualMode.StackMode. 
        /// </summary>
        /// <remarks>
        /// This reference is changed each time when new template is applied.
        /// </remarks>
        private GroupBarSplitter m_splitter;

        /// <summary>
        /// Used for reflecting <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> objects
        /// being scrolled down and for containing the control's menu. 
        /// </summary>
        /// <remarks>
        /// This reference is changed each time when new template is applied.
        /// </remarks>
        private NavigationToolbar m_toolbar;

        /// <summary>
        /// Main host in the control's visual tree. Used as a target for
        /// orientation changing animation.
        /// </summary>
        /// <remarks>
        /// This reference is changed each time when new template is applied.
        /// </remarks>
        private FrameworkElement m_mainHost;

        /// <summary>
        /// ScrollViewer host in the control's visual tree. 
        /// </summary>
        /// <remarks>
        /// This reference is changed each time when new template is applied.
        /// </remarks>
        private ScrollViewer m_scrollViewerHost;

        /// <summary>
        /// Height of top and bottom borders of the control.
        /// </summary>
        private double m_bordersHeight;

        /// <summary>
        /// Storyboard that is applied to selected content during orientation
        /// changing animation. Used only in stack mode.
        /// </summary>
        private Storyboard m_orientationChangedContentStoryboard;

        /// <summary>
        /// Storyboard that is applied to the control's main grid when orientation
        /// changes to vertical.
        /// </summary>
        private Storyboard m_verticalOrientationStoryboard;

        /// <summary>
        /// Storyboard that is applied to the control's main grid when orientation
        /// changes to horizontal.
        /// </summary>
        private Storyboard m_horizontalOrientationStoryboard;

        /// <summary>
        /// Element in the visual tree which hosts the selected content. Used
        /// only in stack mode.
        /// </summary>
        private ContentPresenter m_visualContent;

        /// <summary>
        /// Indicates whether the keyboard navigation is enabled.
        /// </summary>
        private bool m_bEnabledKeyBoardNavigation = true;

        /// <summary>
        /// Presents file name for saving in the internal isolated storage.
        /// </summary>
        private readonly string m_StoreFileName = AppDomain.CurrentDomain.SetupInformation.ApplicationName + ".dat";

        /// <summary>
        /// Field that determines whether load state mode runs. It is necessary for holding the correct
        /// height of the group bar after load state action. Wrong height sets in <see cref="OnItemsChanged"/> method,
        /// when Load state is done.
        /// </summary>
        private bool m_isInLoadStateMode = false;

        /// <summary>
        /// Indicates whether dragging of the item is in progress.
        /// </summary>
        private bool m_draggingItemInProgress = false;

        /// <summary>
        /// Used for saving the content height before copying <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> to the clipboard.
        /// </summary>
        private double m_contentHeight = 0;

       
        /// <summary>
        /// Popup used to display the groupBarItems.
        /// </summary>
        private Popup m_contentPopup;

        /// <summary>
        /// Contains GroupViewItem width to start horizontal scrolling from.
        /// </summary>
        private double m_minWidthOfList = 0;

        /// <summary>
        /// Content button used to display the groupBarItems.
        /// </summary>
        private ToggleButton m_contentButton;

        /// <summary>
        /// Used for resizing the popup.
        /// </summary>
        private Border m_popupResizeBorder;

        /// <summary>
        /// Indicates whether resizing of the popup is vertical.
        /// </summary>
        private bool m_isHorizontalResizing = false;

        /// <summary>
        /// Indicates whether resizing of the popup is horizontal.
        /// </summary>
        private bool m_isVerticalResizing = false;

        /// <summary>
        /// Indicates whether resizing of the popup is vertical and horizontal.
        /// </summary>
        private bool m_isCornerResizing = false;

        /// <summary>
        /// Used for displaying the content in the popup.
        /// </summary>
        private ContentPresenter m_popupContentHost;

        /// <summary>
        /// Used for saving the width before collapsing.
        /// </summary>
        private double m_cachedWidth;

        /// <summary>
        /// Used while resizing the popup.
        /// </summary>
        private double m_adjustWidth;

        /// <summary>
        /// Used while resizing the popup.
        /// </summary>
        private double m_adjustHeight;

        /// <summary>
        /// Contains true if splitting is in progress.
        /// </summary>
        private bool m_isSplitting = false;

        /// <summary>
        /// Stores the orientation changed width.
        /// </summary>
        private double m_orientationChangedWidth;

        /// <summary>
        /// Stores the orientation changed height.
        /// </summary>
        private double m_orientationChangedHeight;

       
        /// <summary>
        /// Contains StackItemsHost border.
        /// </summary>
        private Border m_stackItemsHost = null;

        /// <summary>
        /// Contains header content of the selected GBI.
        /// </summary>
        private ContentPresenter m_headerContent = null;

        /// <summary>
        /// Contains visible and hidden items count.
        /// </summary>
        private int m_visibleItemsCount = 0;

       
        /// <summary>
        /// Contains default item image.
        /// </summary>
        private Image m_defaultImage = new Image();

        /// <summary>
        /// Contains previous height of the GroupBar popup.
        /// </summary>
        private double m_oldPopupHeight = 0;

        /// <summary>
        /// Contains true when VisualSkin property changed
        /// </summary>
        private bool m_isVisualSkinChanged = false;

        /// <summary>
        /// Contains true if orientation of the group bar is changing.
        /// </summary>
        private bool m_isOrientationChanging = false;

        /// <summary>
        /// Specifies IsSplitted variable
        /// </summary>
        private bool IsSplitted = false;


        private ContentPresenter m_Presenter = null;
        
        private Border HeaderHost=null;

        private NavigationToolbar m_Naavigationtoolbar = null;
        #endregion

        #region Commands
        /// <summary>
        /// Command that makes the splitter to go down.
        /// </summary>
        public static RoutedCommand SplitterDownCommand =
            new RoutedCommand("SplitterDown", typeof(GroupBar), null);

        /// <summary>
        /// Command that makes the splitter to go up.
        /// </summary>
        public static RoutedCommand SplitterUpCommand =
            new RoutedCommand("SplitterUp", typeof(GroupBar), null);

        /// <summary>
        /// Command that makes application options open. Not implemented yet.
        /// </summary>
        public static RoutedCommand OpenOptionsCommand =
            new RoutedCommand("OpenOptions", typeof(GroupBar), null);

        /// <summary>
        /// Command that is responsible for adding the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> 
        /// to the <see cref="GroupBar"/>.
        /// </summary>
        public readonly static RoutedUICommand AddTabCommand =
            new RoutedUICommand("AddTabCommand", "AddTabCommand", typeof(GroupBar));

        /// <summary>
        /// Command that is responsible for adding the <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> 
        /// to the <see cref="Syncfusion.Windows.Tools.Controls.GroupView"/>.
        /// </summary>
        public static RoutedUICommand AddItemCommand =
            new RoutedUICommand("AddItemCommand", "AddItemCommand", typeof(GroupBar));

        /// <summary>
        /// Command that is responsible for renaming the <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/>.
        /// </summary>
        public static RoutedUICommand RenameItemCommand =
            new RoutedUICommand("RenameItemCommand", "RenameItemCommand", typeof(GroupBar));

        /// <summary>
        /// Command that is responsible for deleting the <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/>
        /// from the <see cref="Syncfusion.Windows.Tools.Controls.GroupView"/>.
        /// </summary>
        public static RoutedUICommand DeleteItemCommand =
            new RoutedUICommand("DeleteItemCommand", "DeleteItemCommand", typeof(GroupBar));

        /// <summary>
        /// Command that is responsible for renaming the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/>.
        /// </summary>
        public static RoutedUICommand RenameTabCommand =
            new RoutedUICommand("RenameTabCommand", "RenameTabCommand", typeof(GroupBar));

        /// <summary>
        /// Command that is responsible for deleting the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/>
        /// from the <see cref="GroupBar"/>.
        /// </summary>
        public static RoutedUICommand DeleteTabCommand =
            new RoutedUICommand("DeleteTabCommand", "DeleteTabCommand", typeof(GroupBar));

        /// <summary>
        /// Command that is responsible for changing list view mode.
        /// </summary>
        public readonly static RoutedUICommand ChangeListViewModeCommand =
            new RoutedUICommand("ChangeListViewModeCommand", "ChangeListViewModeCommand", typeof(GroupBar));

        /// <summary>
        /// Command that is responsible for sorting <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> 
        /// or <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> objects in ascending order.
        /// </summary>
        public readonly static RoutedUICommand SortAscCommand =
            new RoutedUICommand("SortAscCommand", "SortAscCommand", typeof(GroupBar));

        /// <summary>
        /// Command that is responsible for sorting <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> 
        /// or <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> objects in descending order.
        /// </summary>
        public readonly static RoutedUICommand SortDesCommand =
            new RoutedUICommand("SortDesCommand", "SortDesCommand", typeof(GroupBar));

        /// <summary>
        /// Command that is responsible for moving up the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> 
        /// or the <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/>.
        /// </summary>
        public readonly static RoutedUICommand MoveUpCommand =
            new RoutedUICommand("MoveUpCommand", "MoveUpCommand", typeof(GroupBar));

        /// <summary>
        /// Command that is responsible for moving down the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> 
        /// or the <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/>.
        /// </summary>
        public readonly static RoutedUICommand MoveDownCommand =
            new RoutedUICommand("MoveDownCommand", "MoveDownCommand", typeof(GroupBar));

        /// <summary>
        /// Command that is responsible for cutting the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> 
        /// or the <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/>.
        /// </summary>
        public readonly static RoutedUICommand CutItemCommand =
            new RoutedUICommand("CutItemCommand", "CutItemCommand", typeof(GroupBar));

        /// <summary>
        /// Command that is responsible for copping the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> 
        /// or the <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/>.
        /// </summary>
        public readonly static RoutedUICommand CopyItemCommand =
            new RoutedUICommand("CopyItemCommand", "CopyItemCommand", typeof(GroupBar));

        /// <summary>
        /// Command that is responsible for pasting the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> 
        /// or the <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/>.
        /// </summary>
        public readonly static RoutedUICommand PasteItemCommand =
            new RoutedUICommand("PasteItemCommand", "PasteItemCommand", typeof(GroupBar));

        /// <summary>
        /// Command for the popup.
        /// </summary>
        public readonly static RoutedCommand m_popupCommand = new RoutedCommand("m_popupCommand", typeof(GroupBar));

        /// <summary>
        /// Command for the close button.
        /// </summary>
        public readonly static RoutedCommand m_closeButtonCommand = new RoutedCommand("m_closeButtonCommand", typeof(GroupBar));

        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="GroupBar"/> class.. 
        /// Overrides some dependency properties.
        /// </summary>
        static GroupBar()
        {
            //EnvironmentTest.ValidateLicense(typeof(GroupBar));

            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(GroupBar),
                new FrameworkPropertyMetadata(typeof(GroupBar)));
            FlowDirectionProperty.OverrideMetadata(
                typeof(GroupBar),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnFlowDirectionChanged)));
            HeightProperty.OverrideMetadata(
                typeof(GroupBar),
                new FrameworkPropertyMetadata(OnHeightChanged));
            WidthProperty.OverrideMetadata(
                typeof(GroupBar),
                new FrameworkPropertyMetadata(OnWidthChanged));

            CommandBinding splitterDownBinding = new CommandBinding(SplitterDownCommand, HandleSplitterDownCommand);
            CommandManager.RegisterClassCommandBinding(typeof(GroupBar), splitterDownBinding);

            CommandBinding splitterUpBinding = new CommandBinding(SplitterUpCommand, HandleSplitterUpCommand);
            CommandManager.RegisterClassCommandBinding(typeof(GroupBar), splitterUpBinding);

            CommandBinding openOptionsBinding = new CommandBinding(OpenOptionsCommand, HandleOpenOptionsCommand);
            CommandManager.RegisterClassCommandBinding(typeof(GroupBar), openOptionsBinding);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupBar"/> class.
        /// </summary>
        public GroupBar()
        {
            if (EnvironmentTestTools.IsSecurityGranted)
            {
                EnvironmentTestTools.StartValidateLicense(typeof(GroupBar));
            }
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (Application.Current != null && !BindingUtils.GetEnableBindingErrors(Application.Current.MainWindow))
                    System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
            }
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            Initialize();
            this.Loaded += new RoutedEventHandler(GroupBar_Loaded);
            //this.Unloaded += new RoutedEventHandler(GroupBar_Unloaded);
        }

        internal bool _isItemsLoadedRunTime = false;
        void GroupBar_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Items.Count == 0)
                _isItemsLoadedRunTime = true;
        }

       
        //void GroupBar_Unloaded(object sender, RoutedEventArgs e)
        //{
        //    this.Dispose();
        //}

        #endregion

        #region Events
        /// <summary>
        /// Identifies <see cref="OrientationChanging"/> event.
        /// </summary>
        public static readonly RoutedEvent OrientationChangingEvent = EventManager.RegisterRoutedEvent(
            "OrientationChanging",
            RoutingStrategy.Bubble,
            typeof(OrientationChangeEventHandler),
            typeof(GroupBar));

        /// <summary>
        /// Bubbling routed event is fired when orientation of the control
        /// is changing, i.e. when orientation changing animation is
        /// in progress.
        /// </summary>
        public event OrientationChangeEventHandler OrientationChanging
        {
            add
            {
                AddHandler(OrientationChangingEvent, value);
            }

            remove
            {
                RemoveHandler(OrientationChangingEvent, value);
            }
        }

        /// <summary>
        /// Identifies <see cref="OrientationChanged"/> event.
        /// </summary>
        public static readonly RoutedEvent OrientationChangedEvent = EventManager.RegisterRoutedEvent(
            "OrientationChanged",
            RoutingStrategy.Bubble,
            typeof(OrientationChangeEventHandler),
            typeof(GroupBar));

        /// <summary>
        /// Bubbling routed event is fired when orientation of the control
        /// is changed, i.e. when orientation changing animation is
        /// completed.
        /// </summary>
        public event OrientationChangeEventHandler OrientationChanged
        {
            add
            {
                AddHandler(OrientationChangedEvent, value);
            }

            remove
            {
                RemoveHandler(OrientationChangedEvent, value);
            }
        }

        /// <summary>
        /// Identifies <see cref="FlowDirectionChanged"/> event.
        /// </summary>
        public static readonly RoutedEvent FlowDirectionChangedEvent = EventManager.RegisterRoutedEvent(
            "FlowDirectionChanged",
            RoutingStrategy.Bubble,
            typeof(FlowDirectionChangedEventHandler),
            typeof(GroupBar));

        /// <summary>
        /// Bubbling routed event is fired when flow direction of the
        /// control is changed.
        /// </summary>
        public event FlowDirectionChangedEventHandler FlowDirectionChanged
        {
            add
            {
                AddHandler(FlowDirectionChangedEvent, value);
            }

            remove
            {
                RemoveHandler(FlowDirectionChangedEvent, value);
            }
        }

        /// <summary>
        /// Identifies <see cref="VisualModeChanged"/> event.
        /// </summary>
        public static readonly RoutedEvent VisualModeChangedEvent = EventManager.RegisterRoutedEvent(
            "VisualModeChanged",
            RoutingStrategy.Bubble,
            typeof(EventHandler),
            typeof(GroupBar));

        /// <summary>
        /// Bubbling routed event is fired when stack mode is changed.
        /// </summary>
        public event RoutedEventHandler VisualModeChanged
        {
            add
            {
                AddHandler(VisualModeChangedEvent, value);
            }

            remove
            {
                RemoveHandler(VisualModeChangedEvent, value);
            }
        }

        /// <summary>
        /// Identifies <see cref="BeforeSplitUp"/> event.
        /// </summary>
        public static readonly RoutedEvent BeforeSplitUpEvent = EventManager.RegisterRoutedEvent(
            "BeforeSplitUp",
            RoutingStrategy.Bubble,
            typeof(EventHandler),
            typeof(GroupBar));

        /// <summary>
        /// Bubbling routed event is fired before SplitUp is changed.
        /// </summary>
        public event RoutedEventHandler BeforeSplitUp
        {
            add
            {
                AddHandler(BeforeSplitUpEvent, value);
            }

            remove
            {
                RemoveHandler(BeforeSplitUpEvent, value);
            }
        }

        /// <summary>
        /// Identifies <see cref="BeforeSplitDown"/> event.
        /// </summary>
        public static readonly RoutedEvent BeforeSplitDownEvent = EventManager.RegisterRoutedEvent(
            "BeforeSplitDown",
            RoutingStrategy.Bubble,
            typeof(EventHandler),
            typeof(GroupBar));

        /// <summary>
        /// Bubbling routed event is fired before SplitDown is changed.
        /// </summary>
        public event RoutedEventHandler BeforeSplitDown
        {
            add
            {
                AddHandler(BeforeSplitDownEvent, value);
            }

            remove
            {
                RemoveHandler(BeforeSplitDownEvent, value);
            }
        }

        /// <summary>
        /// Identifies <see cref="AfterSplitUp"/> event.
        /// </summary>
        public static readonly RoutedEvent AfterSplitUpEvent = EventManager.RegisterRoutedEvent(
            "AfterSplitUp",
            RoutingStrategy.Bubble,
            typeof(EventHandler),
            typeof(GroupBar));

        /// <summary>
        /// Bubbling routed event is fired after SplitUp is changed.
        /// </summary>
        public event RoutedEventHandler AfterSplitUp
        {
            add
            {
                AddHandler(AfterSplitUpEvent, value);
            }

            remove
            {
                RemoveHandler(AfterSplitUpEvent, value);
            }
        }

        /// <summary>
        /// Identifies <see cref="AfterSplitDown"/> event.
        /// </summary>
        public static readonly RoutedEvent AfterSplitDownEvent = EventManager.RegisterRoutedEvent(
            "AfterSplitDown",
            RoutingStrategy.Bubble,
            typeof(EventHandler),
            typeof(GroupBar));

        /// <summary>
        /// Bubbling routed event is fired after SplitDown is changed.
        /// </summary>
        public event RoutedEventHandler AfterSplitDown
        {
            add
            {
                AddHandler(AfterSplitDownEvent, value);
            }

            remove
            {
                RemoveHandler(AfterSplitDownEvent, value);
            }
        }

        /// <summary>
        /// Identifies <see cref="SelectedObjectChanged"/> event.
        /// </summary>
        public static readonly RoutedEvent SelectedObjectChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectedObjectChanged",
            RoutingStrategy.Bubble,
            typeof(EventHandler),
            typeof(GroupBar));

        /// <summary>
        /// Bubbling routed event is fired when <see cref="SelectedObject"/> property is changed.
        /// </summary>
        public event RoutedEventHandler SelectedObjectChanged
        {
            add
            {
                AddHandler(SelectedObjectChangedEvent, value);
            }

            remove
            {
                RemoveHandler(SelectedObjectChangedEvent, value);
            }
        }

        /// <summary>
        /// Identifies <see cref="SelectedItemChanged"/> event.
        /// </summary>
        public static readonly RoutedEvent SelectedItemChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectedItemChanged",
            RoutingStrategy.Bubble,
            typeof(EventHandler),
            typeof(GroupBar));

        /// <summary>
        /// Bubbling routed event is fired when <see cref="SelectedItem"/> property is changed.
        /// </summary>
        public event RoutedEventHandler SelectedItemChanged
        {
            add
            {
                AddHandler(SelectedItemChangedEvent, value);
            }

            remove
            {
                RemoveHandler(SelectedItemChangedEvent, value);
            }
        }

        /// <summary>
        /// Identifies <see cref="SelectedTabChanged"/> event.
        /// </summary>
        public static readonly RoutedEvent SelectedTabChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectedTabChanged",
            RoutingStrategy.Bubble,
            typeof(EventHandler),
            typeof(GroupBar));

        /// <summary>
        /// Bubbling routed event is fired when <see cref="SelectedTab"/> property is changed.
        /// </summary>
        public event RoutedEventHandler SelectedTabChanged
        {
            add
            {
                AddHandler(SelectedTabChangedEvent, value);
            }

            remove
            {
                RemoveHandler(SelectedTabChangedEvent, value);
            }
        }

        /// <summary>
        /// Occurs when the Navigation Menu Opening.
        /// </summary>
        public event EventHandler NavigationMenuOpening;

        /// <summary>
        /// Occurs when the Navigation Menu Closing.
        /// </summary>
        public event CancelEventHandler NavigationMenuClosing;

        /// <summary>
        /// Occurs when the NavigationOptionsMenuItem Clicked.
        /// </summary>
        public event EventHandler NavigationOptionsMenuItemClick;

        /// <summary>
        /// Occurs when any of the  Group Bar Context Menu item Clicked.
        /// </summary>
        public event GroupBarContextMenuItemEventHandler ContextMenuItemClick;

        /// <summary>
        /// Occurs when a new <see cref="GroupBarItem"/> is added into the <see cref="GroupBar"/>.
        /// </summary>
        public event NotifyCollectionChangedEventHandler GroupBarItemAdded;

        /// <summary>
        /// Occurs when an existing <see cref="GroupBarItem"/> is removed from the <see cref="GroupBar"/>.
        /// </summary>
        public event NotifyCollectionChangedEventHandler GroupBarItemRemoved;

        /// <summary>
        /// Occurs when the collapse button is clicked.
        /// </summary>
        public event PropertyChangedCallback CollapsedChanged;

        /// <summary>
        /// Occurs before displaying the <see cref="GroupBarItem"/> in the popup container during the collapsed mode.
        /// </summary>
        public event BeforeGroupBarItemPopupOpenedEventHandler BeforeGroupBarItemPopupOpened;

        /// <summary>
        /// Occurs after the popup container is closed.
        /// </summary>
        public event EventHandler AfterGroupBarItemPopupClosed;

        /// <summary>
        /// Occurs when value of the CollapsedWidth property is changed.
        /// </summary>
        public event PropertyChangedCallback CollapsedWidthChanged;

        /// <summary>
        /// Occurs when value of the PopupResizeDirection property is changed.
        /// </summary>
        public event PropertyChangedCallback PopupResizeDirectionChanged;

        /// <summary>
        /// Occurs when value of the ShowGripper property is changed.
        /// </summary>
        public event PropertyChangedCallback ShowGripperChanged;

        /// <summary>
        /// Occurs when value of the StackItemHostVisibilityChanged property is changed.
        /// </summary>
        public event PropertyChangedCallback StackItemHostVisibilityChanged;

        /// <summary>
        /// Occurs when value of the NavigationPopupSizeChanged property is changed.
        /// </summary>
        public event PropertyChangedCallback NavigationPopupSizeChanged;

        /// <summary>
        /// Occurs when value of the CornerRadiusChanged property is changed.
        /// </summary>
        public event PropertyChangedCallback CornerRadiusChanged;

        /// <summary>
        /// Occurs when value of the StackItemHostHeightChanged property is changed.
        /// </summary>
        public event PropertyChangedCallback StackItemHostHeightChanged;

        /// <summary>
        /// Occurs when [group bar header style changed].
        /// </summary>
        public event PropertyChangedCallback GroupBarHeaderStyleChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets maximum bottom corner radius.
        /// </summary>
        private double MaxBottomCornerRadius
        {
            get
            {
                return (CornerRadius.BottomLeft > CornerRadius.BottomRight) ? CornerRadius.BottomLeft : CornerRadius.BottomRight;
            }
        }

        /// <summary>
        /// Gets maximum top corner radius.
        /// </summary>
        private double MaxTopCornerRadius
        {
            get
            {
                return (CornerRadius.TopLeft > CornerRadius.TopRight) ? CornerRadius.TopLeft : CornerRadius.TopRight;
            }
        }

        /// <summary>
        /// Gets ItemsHost border.
        /// </summary>
        internal Border ItemsHostBorder
        {
            get
            {
                if (m_itemsHost == null && SelectedContent is GroupView)
                {
                    GroupView view = (GroupView)SelectedContent;
                    m_itemsHost = view.Template.FindName(C_itemsHost, view) as Border;
                }

                return m_itemsHost;
            }
        }

        /// <summary>
        /// Gets GroupViewItem width to start horizontal scrolling from.
        /// </summary>
        internal double MinWidthOfList
        {
            get
            {
                return m_minWidthOfList;
            }
        }

        /// <summary>
        /// Gets Popup used to display the groupBarItems.
        /// </summary>
        internal Popup ContentPopup
        {
            get
            {
                return m_contentPopup;
            }
        }

        /// <summary>
        /// Gets content host of the popup.
        /// </summary>
        internal ContentPresenter PopupContentHost
        {
            get
            {
                return m_popupContentHost;
            }
        }

        /// <summary>
        /// Gets default item image.
        /// </summary>
        internal Image DefaultImage
        {
            get
            {
                m_defaultImage.Source = DefaultItemImage;
                return m_defaultImage;
            }
        }

        /// <summary>
        /// Gets or sets StackItemsHost border.
        /// </summary>
        internal Border StackItemsHost
        {
            get
            {
                return m_stackItemsHost;
            }

            set
            {
                m_stackItemsHost = value;
            }
        }

        /// <summary>
        /// Gets or sets visible and hidden items count.
        /// </summary>
        internal int VisibleItemsCount
        {
            get
            {
                return m_visibleItemsCount;
            }

            set
            {
                m_visibleItemsCount = value;
            }
        }

        /// <summary>
        /// Gets or sets header content.
        /// </summary>
        internal ContentPresenter HeaderContent
        {
            get
            {
                if (m_headerContent == null && Template != null)
                {
                    m_headerContent = Template.FindName(C_nameHeaderContent, this) as ContentPresenter;
                }

                return m_headerContent;
            }

            set
            {
                m_headerContent = value;
            }
        }

        /////// <summary>
        /////// Gets or sets the default background.
        /////// </summary>
        /////// <value>The default background.</value>
        ////internal Brush DefaultBackground
        ////{
        ////    get
        ////    {
        ////        return (Brush)GetValue(DefaultBackgroundProperty);
        ////    }

        ////    set
        ////    {
        ////        SetValue(DefaultBackgroundProperty, value);
        ////    }
        ////}

        /////// <summary>
        /////// Gets or sets the default border brush.
        /////// </summary>
        /////// <value>The default border brush.</value>
        ////internal Brush DefaultBorderBrush
        ////{
        ////    get
        ////    {
        ////        return (Brush)GetValue(DefaultBorderBrushProperty);
        ////    }

        ////    set
        ////    {
        ////        SetValue(DefaultBorderBrushProperty, value);
        ////    }
        ////}

        /// <summary>
        /// Gets or sets text horizontal alignment for the header of the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/>.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="HorizontalAlignment"/>
        /// Default value is <see cref="HorizontalAlignment.Left"/>.
        /// </value>
        /// <seealso cref="HorizontalAlignment"/>
        /// <example> 
        ///  // Create a new instance of the GroupBar
        ///  GroupBar groupBar = new GroupBar();
        ///  // Add GroupBar to grid
        ///  grid1.Children.Add( groupBar );
        ///  // Create a new instance of the GroupBarItem
        ///  GroupBarItem barItem1 = new GroupBarItem();
        ///  // Set caption of the item
        ///  barItem1.HeaderText = "Item1";
        ///  // Create a new instance of the GroupBarItem
        ///  GroupBarItem barItem2 = new GroupBarItem();
        ///  // Set caption of the item
        ///  barItem2.HeaderText = "Item2";
        ///  // Add items to GroupBar.Items collection
        ///  groupBar.Items.Add( barItem1 );
        ///  groupBar.Items.Add( barItem2 );
        ///  // Set alignment of the text to HorizontalAlignment.Right
        ///  groupBar.TextAlignment = HorizontalAlignment.Right;
        /// /*
        /// Result: 
        ///     Items header text will be aligned on the right side.
        /// */ 
        /// </example> 
        public HorizontalAlignment TextAlignment
        {
            get
            {
                return (HorizontalAlignment)GetValue(TextAlignmentProperty);
            }

            set
            {
                SetValue(TextAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets height (for vertical layout) or width (for
        /// horizontal layout) available for the content of the selected item.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="double"/>
        public double ItemContentLength
        {
            get
            {
                return (double)GetValue(ItemContentLengthProperty);
            }

            set
            {
                SetValue(ItemContentLengthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the angle of displaying the items content.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="double"/>
        public double ContentRotationAngle
        {
            get
            {
                return (double)GetValue(ContentRotationAngleProperty);
            }

            set
            {
                SetValue(ContentRotationAngleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height of topmost container that hosts
        /// the selected header. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 50.
        /// </value>
        /// <seealso cref="double"/>
        public double HeaderHeight
        {
            get
            {
                return (double)GetValue(HeaderHeightProperty);
            }

            set
            {
                SetValue(HeaderHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled context menu.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is enabled context menu; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabledContextMenu
        {
            get
            {
                return (bool)GetValue(IsEnabledContextMenuProperty);
            }

            set
            {
                SetValue(IsEnabledContextMenuProperty, value);
            }
        }

        /// <summary>
        /// Gets the hidden items count.
        /// </summary>
        /// <value>The hidden items count.</value>
        public int HiddenItemsCount
        {
            get
            {
                return m_hiddenIndices.Count;
            }
        }

        /// <summary>
        /// Gets or sets visual style of the control according to
        /// the <see cref="VisualStyle"/> enumeration. The visual style contains all
        /// brushes used for painting the control and all animations applied to the control.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="VisualStyle"/>
        /// Default value is VisualStyle.Default/>.
        /// </value>
        /// <seealso cref="VisualStyle"/>
        public VisualStyle VisualStyle
        {
            get
            {
                return (VisualStyle)GetValue(VisualStyleProperty);
            }

            set
            {
                SetValue(VisualStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets visual mode of the control according to
        /// the <see cref="VisualMode"/> enumeration.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="VisualMode"/>
        /// Default value is VisualMode.Default/>.
        /// </value>
        /// <seealso cref="VisualMode"/>
        public VisualMode VisualMode
        {
            get
            {
                return (VisualMode)GetValue(VisualModeProperty);
            }

            set
            {
                SetValue(VisualModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the collection of the custom animations applied to the
        /// control. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="CustomAnimationsCollection"/>
        /// Default value is null.
        /// </value>
        /// <seealso cref="CustomAnimationsCollection"/>
        [TypeConverter(typeof(CustomAnimationsConverter))]
        public CustomAnimationsCollection CustomAnimations
        {
            get
            {
                return (CustomAnimationsCollection)GetValue(CustomAnimationsProperty);
            }

            set
            {
                SetValue(CustomAnimationsProperty, value);
            }
        }

        /// <summary>
        /// Gets the logical parent as <see cref="FrameworkElement"/>.
        /// </summary>
        /// <value>
        /// Type: <see cref="FrameworkElement"/>
        /// </value>
        /// <seealso cref="FrameworkElement"/>
        public FrameworkElement LogicalParent
        {
            get
            {
                return base.Parent as FrameworkElement;
            }
        }

        /// <summary>
        /// Gets or sets the content of the selected item.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="object"/>
        /// Default value is null.
        /// </value>
        /// <seealso cref="object"/>
        public object SelectedContent
        {
            get
            {
                return GetValue(SelectedContentProperty);
            }

            set
            {
                SetValue(SelectedContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header of the selected item.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="object"/>
        /// Default value is null.
        /// </value>
        /// <seealso cref="object"/>
        public object SelectedHeader
        {
            get
            {
                return GetValue(SelectedHeaderProperty);
            }

            set
            {
                SetValue(SelectedHeaderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height for the each item's header.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 50.
        /// </value>
        /// <seealso cref="double"/>
        public double ItemHeaderHeight
        {
            get
            {
                return (double)GetValue(ItemHeaderHeightProperty);
            }

            set
            {
                SetValue(ItemHeaderHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the size for the navigation popup.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Size"/>
        /// </value>
        /// <seealso cref="Size"/>
        public Size NavigationPopupSize
        {
            get
            {
                return (Size)GetValue(NavigationPopupSizeProperty);
            }

            set
            {
                SetValue(NavigationPopupSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the control.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Orientation"/>
        /// Default value is Orientation.Vertical/>.
        /// </value>
        /// <seealso cref="Orientation"/>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }

            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets the reference to the <see cref="Syncfusion.Windows.Tools.Controls.NavigationToolbar"/>.
        /// </summary>
        /// <value>
        /// Type: <see cref="NavigationToolbar"/>
        /// </value>
        /// <seealso cref="NavigationToolbar"/>
        internal NavigationToolbar Toolbar
        {
            get
            {
                return m_toolbar;
            }
        }

        /// <summary>
        /// Gets all space that can be allocated for the item's headers.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// </value>
        /// <seealso cref="double"/>
        internal double AvailableHeadersHeight
        {
            get
            {
                double returnValue = Orientation == Orientation.Vertical ? Height : Width;
                returnValue -= m_bordersHeight;

                if (VisualMode.StackMode == VisualMode)
                {
                    returnValue -= StackModeHeightCorrection;
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Gets or sets the height of the <see cref="Syncfusion.Windows.Tools.Controls.NavigationToolbar"/>.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 50.
        /// </value>
        /// <seealso cref="double"/>
        public double StackItemHostHeight
        {
            get
            {
                return (double)GetValue(StackItemHostHeightProperty);
            }

            set
            {
                SetValue(StackItemHostHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the angle of displaying the <see cref="GroupBar"/>.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 0.
        /// </value>
        /// <seealso cref="double"/>
        public double RotationAngle
        {
            get
            {
                return (double)GetValue(RotationAngleProperty);
            }

            set
            {
                SetValue(RotationAngleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [animation in progress].
        /// </summary>
        /// <value><c>true</c> if [animation in progress]; otherwise, <c>false</c>.</value>
        private bool AnimationInProgress { get; set; }

        /// <summary>
        /// Gets the main host.
        /// </summary>
        /// <value>The main host.</value>
        private FrameworkElement MainHost
        {
            get
            {
                return m_mainHost;
            }
        }

        /// <summary>
        /// Gets the scroll viewer host.
        /// </summary>
        /// <value>The scroll viewer host.</value>
        internal ScrollViewer ScrollViewerHost
        {
            get
            {
                return m_scrollViewerHost;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [template changing].
        /// </summary>
        /// <value><c>true</c> if [template changing]; otherwise, <c>false</c>.</value>
        private bool TemplateChanging
        {
            get
            {
                return m_templateChanging;
            }
        }

        /// <summary>
        /// Gets the stack mode height correction.
        /// </summary>
        /// <value>The stack mode height correction.</value>
        private double StackModeHeightCorrection
        {
            get
            {
                double height = 0d;

                if (m_splitter != null && m_toolbar != null)
                {
                    height += StackItemHostHeight + HeaderHeight;
                    height += Splitter.Height + Splitter.Margin.Bottom + Splitter.Margin.Top;
                    if (VisualStyle != VisualStyle.Default)
                    {
                        height--;
                    }
                }

                return height;
            }
        }

        /// <summary>
        /// Gets the splitter.
        /// </summary>
        /// <value>The splitter.</value>
        private GroupBarSplitter Splitter
        {
            get
            {
                return m_splitter;
            }
        }

        /// <summary>
        /// Gets the stack items count.
        /// </summary>
        /// <value>The stack items count.</value>
        internal int StackItemsCount
        {
            get
            {
                return Items.Count - HiddenItemsCount - ToolbarItemsCount;
            }
        }

        /// <summary>
        /// Gets the hidden indices.
        /// </summary>
        /// <value>The hidden indices.</value>
        internal List<int> HiddenIndices
        {
            get
            {
                return m_hiddenIndices;
            }
        }

        /// <summary>
        /// Gets the toolbar items count.
        /// </summary>
        /// <value>The toolbar items count.</value>
        private int ToolbarItemsCount
        {
            get
            {
                if (null != m_toolbar)
                {
                    return m_toolbar.Items.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets the selected tab.
        /// </summary>
        /// <value>The selected tab.</value>
        public GroupBarItem SelectedTab
        {
            get
            {
                return (GroupBarItem)GetValue(SelectedTabProperty);
            }

            internal set
            {
                SetValue(SelectedTabPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public GroupViewItem SelectedItem
        {
            get
            {
                return (GroupViewItem)GetValue(SelectedItemProperty);
            }

            internal set
            {
                SetValue(SelectedItemPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected object.
        /// </summary>
        /// <value>The selected object.</value>
        public object SelectedObject
        {
            get
            {
                return (object)GetValue(SelectedObjectProperty);
            }

            set
            {
                SetValue(SelectedObjectProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enabled key board navigation].
        /// </summary>
        /// <value>
        /// <c>true</c> if [enabled key board navigation]; otherwise, <c>false</c>.
        /// </value>
        internal bool EnabledKeyBoardNavigation
        {
            get
            {
                return m_bEnabledKeyBoardNavigation;
            }

            set
            {
                if (value != m_bEnabledKeyBoardNavigation)
                {
                    m_bEnabledKeyBoardNavigation = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the group bar item cursor.
        /// </summary>
        /// <value>The type of the group bar item cursor.</value>
        public ItemCursorType GroupBarItemCursorType
        {
            get
            {
                return (ItemCursorType)GetValue(GroupBarItemCursorTypeProperty);
            }

            set
            {
                SetValue(GroupBarItemCursorTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets the group bar item cursor.
        /// </summary>
        /// <value>The group bar item cursor.</value>
        public Cursor GroupBarItemCursor
        {
            get
            {
                ItemCursorType cursorType = (ItemCursorType)GetValue(GroupBarItemCursorTypeProperty);

                if (cursorType == ItemCursorType.Default)
                {
                    return Cursors.Arrow;
                }

                return Cursors.Hand;
            }
        }

        /// <summary>
        /// Gets or sets the brush for the drag marker.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Default value is <see cref="Brushes.Black"/>.
        /// </value>
        /// <seealso cref="Brush"/>
        public Brush DragMarkerBrush
        {
            get
            {
                return (Brush)GetValue(DragMarkerBrushProperty);
            }

            set
            {
                SetValue(DragMarkerBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the default item image.
        /// </summary>
        /// <value>The default item image.</value>
        ////[TypeConverter( typeof( StringToImageTypeConverter ) )]
        public ImageSource DefaultItemImage
        {
            get
            {
                return (ImageSource)GetValue(DefaultItemImageProperty);
            }

            set
            {
                SetValue(DefaultItemImageProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the storyboard that is applied to the control's main grid when orientation
        /// changes to horizontal. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Storyboard"/>
        /// Default value is null.
        /// </value>
        /// <seealso cref="Storyboard"/>
        public Storyboard HorizontalOrientationStoryboard
        {
            get
            {
                return (Storyboard)GetValue(HorizontalOrientationStoryboardProperty);
            }

            set
            {
                SetValue(HorizontalOrientationStoryboardProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the storyboard that is applied to the control's main grid when orientation
        /// changes to vertical. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Storyboard"/>
        /// Default value is null.
        /// </value>
        /// <seealso cref="Storyboard"/>
        public Storyboard VerticalOrientationStoryboard
        {
            get
            {
                return (Storyboard)GetValue(VerticalOrientationStoryboardProperty);
            }

            set
            {
                SetValue(VerticalOrientationStoryboardProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the storyboard that is applied to selected content during orientation
        /// changing animation. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Storyboard"/>
        /// Default value is null.
        /// </value>
        /// <seealso cref="Storyboard"/>
        public Storyboard OrientationChangedContentStoryboard
        {
            get
            {
                return (Storyboard)GetValue(OrientationChangedContentStoryboardProperty);
            }

            set
            {
                SetValue(OrientationChangedContentStoryboardProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [dragging item in progress].
        /// </summary>
        /// <value>
        /// <c>true</c> if [dragging item in progress]; otherwise, <c>false</c>.
        /// </value>
        public bool DraggingItemInProgress
        {
            get
            {
                return m_draggingItemInProgress;
            }

            set
            {
                if (value != m_draggingItemInProgress)
                {
                    m_draggingItemInProgress = value;
                    UpdateItemsContentVisibility();
                }
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether [save original state].
        /// </summary>
        /// <value><c>true</c> if [save original state]; otherwise, <c>false</c>.</value>
        [Description("Indicates whether to save state persisted on loading.")]
        public bool SaveOriginalState
        {
            get
            {
                return (bool)GetValue(SaveOriginalStateProperty);
            }

            set
            {
                SetValue(SaveOriginalStateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the background of the collapse button.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Default value is Brushes.Transparent.
        /// </value>
        /// <seealso cref="Brush"/>
        public Brush CollapseButtonBackground
        {
            get
            {
                return (Brush)GetValue(CollapseButtonBackgroundProperty);
            }

            set
            {
                SetValue(CollapseButtonBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the background of the collapse button when
        /// mouse is over it. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Default value is Brushes.Transparent.
        /// </value>
        /// <seealso cref="Brush"/>
        public Brush CollapseButtonMouseOverBackground
        {
            get
            {
                return (Brush)GetValue(CollapseButtonMouseOverBackgroundProperty);
            }

            set
            {
                SetValue(CollapseButtonMouseOverBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the template to be applied to the collapse button.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="ControlTemplate"/>
        /// </value>
        /// <seealso cref="ControlTemplate"/>
        public ControlTemplate CollapseButtonTemplate
        {
            get
            {
                return (ControlTemplate)GetValue(CollapseButtonTemplateProperty);
            }

            set
            {
                SetValue(CollapseButtonTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the type of animation to be applied.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="AnimationsType"/>
        /// Default value is AnimationsType.Fade.
        /// </value>
        /// <seealso cref="AnimationsType"/>
        public AnimationsType AnimationType
        {
            get
            {
                return (AnimationsType)GetValue(AnimationTypeProperty);
            }

            set
            {
                SetValue(AnimationTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the speed of the animation.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="int"/>
        /// Default value is 300.
        /// </value>
        /// <seealso cref="int"/>
        public int AnimationSpeed
        {
            get
            {
                return (int)GetValue(AnimationSpeedProperty);
            }

            set
            {
                SetValue(AnimationSpeedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the tooltip to be displayed when the mouse is hover 
        /// over the collapse button. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="ToolTip"/>
        /// </value>
        /// <seealso cref="ToolTip"/>
        ////[TypeConverter( typeof( StringToTooltipTypeConverter ) )]
        public object CollapseButtonToolTip
        {
            get
            {
                return (object)GetValue(CollapseButtonToolTipProperty);
            }

            set
            {
                SetValue(CollapseButtonToolTipProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the tooltip to be displayed when the mouse is hover 
        /// over the expand button. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="ToolTip"/>
        /// </value>
        /// <seealso cref="ToolTip"/>
        ////[TypeConverter( typeof( StringToTooltipTypeConverter ) )]
        public object ExpandButtonToolTip
        {
            get
            {
                return (object)GetValue(ExpandButtonToolTipProperty);
            }

            set
            {
                SetValue(ExpandButtonToolTipProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow collapse].
        /// </summary>
        /// <value><c>true</c> if [allow collapse]; otherwise, <c>false</c>.</value>
        public bool AllowCollapse
        {
            get
            {
                return (bool)GetValue(AllowCollapseProperty);
            }

            set
            {
                SetValue(AllowCollapseProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is collapsed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is collapsed; otherwise, <c>false</c>.
        /// </value>
        public bool IsCollapsed
        {
            get
            {
                return (bool)GetValue(IsCollapsedProperty);
            }

            set
            {
                SetValue(IsCollapsedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is drop down open.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is drop down open; otherwise, <c>false</c>.
        /// </value>
        public bool IsDropDownOpen
        {
            get
            {
                return (bool)GetValue(IsDropDownOpenProperty);
            }

            set
            {
                SetValue(IsDropDownOpenProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is tool bar enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is tool bar enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsToolBarEnabled
        {
            get
            {
                return (bool)GetValue(IsToolBarEnabledProperty);
            }

            set
            {
                SetValue(IsToolBarEnabledProperty, value);
            }
        }

        private static bool IsInSorting = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is close button enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is close button enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsCloseButtonEnabled
        {
            get
            {
                return (bool)GetValue(IsCloseButtonEnabledProperty);
            }

            set
            {
                SetValue(IsCloseButtonEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is splitting.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is splitting; otherwise, <c>false</c>.
        /// </value>
        internal bool IsSplitting
        {
            get
            {
                return m_isSplitting;
            }
        }

        /// <summary>
        /// Gets or sets value indicating width of the Navigation pane in collapsed mode.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// Default value is 34.
        /// </value>
        public double CollapsedWidth
        {
            get
            {
                return (double)GetValue(CollapsedWidthProperty);
            }

            set
            {
                SetValue(CollapsedWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets value indicating resize direction(s) of the Navigation pane's popup.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="PopupResizeDirection"/>
        /// Default value is PopupResizeDirection.Both.
        /// </value>
        public PopupResizeDirection PopupResizeDirection
        {
            get
            {
                return (PopupResizeDirection)GetValue(PopupResizeDirectionProperty);
            }

            set
            {
                SetValue(PopupResizeDirectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show gripper].
        /// </summary>
        /// <value><c>true</c> if [show gripper]; otherwise, <c>false</c>.</value>
        public bool ShowGripper
        {
            get
            {
                return (bool)GetValue(ShowGripperProperty);
            }

            set
            {
                SetValue(ShowGripperProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets value indicating visibility of StackItemHost in the StackMode.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Visibility"/>
        /// Default value is Visible.
        /// </value>
        public Visibility StackItemHostVisibility
        {
            get
            {
                return (Visibility)GetValue(StackItemHostVisibilityProperty);
            }

            set
            {
                SetValue(StackItemHostVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the navigation pane text.
        /// </summary>
        /// <value>The navigation pane text.</value>
        public string NavigationPaneText
        {
            get
            {
                return (string)GetValue(NavigationPaneTextProperty);
            }

            set
            {
                SetValue(NavigationPaneTextProperty, value);
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
        /// Gets or sets the margin of the ScrollViewer.
        /// </summary>
        internal Thickness ScrollViewerMargin
        {
            get
            {
                return (Thickness)GetValue(ScrollViewerMarginProperty);
            }

            set
            {
                SetValue(ScrollViewerMarginProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the top CornerRadius of the header border in the Stack mode.
        /// </summary>
        internal CornerRadius HeaderTopCornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(HeaderTopCornerRadiusProperty);
            }

            set
            {
                SetValue(HeaderTopCornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the bottom CornerRadius of the navigation toolbar in the Stack mode.
        /// </summary>
        internal CornerRadius ToolbarBottomCornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(ToolbarBottomCornerRadiusProperty);
            }

            set
            {
                SetValue(ToolbarBottomCornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the right margin of the navigation toolbar menu in the Stack mode.
        /// </summary>
        internal Thickness ToolbarMenuMarginRight
        {
            get
            {
                return (Thickness)GetValue(ToolbarMenuMarginRightProperty);
            }

            set
            {
                SetValue(ToolbarMenuMarginRightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header margin in the Stack mode.
        /// </summary>
        internal Thickness HeaderMarginRight
        {
            get
            {
                return (Thickness)GetValue(HeaderMarginRightProperty);
            }

            set
            {
                SetValue(HeaderMarginRightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the NavigationOptionsMenuItemHeader. This is a dependency property.
        /// </summary>
        public Object NavigationOptionsMenuItemHeader
        {
            get
            {
                return (object)GetValue(NavigationOptionsMenuItemHeaderProperty);
            }

            set
            {
                SetValue(NavigationOptionsMenuItemHeaderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [dynamic resizing].
        /// </summary>
        /// <value><c>true</c> if [dynamic resizing]; otherwise, <c>false</c>.</value>
        public bool DynamicResizing
        {
            get
            {
                return (bool)GetValue(DynamicResizingProperty);
            }

            set
            {
                SetValue(DynamicResizingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Header Style for Groupbar. This is a dependency property.
        /// </summary>
        public Style GroupBarHeaderStyle
        {
            get
            {
                return (Style)GetValue(GroupBarHeaderStyleProperty);
            }

            set
            {
                SetValue(GroupBarHeaderStyleProperty, value);
            }
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Builds the current template's visual tree if necessary.
        /// </summary>
        public override void OnApplyTemplate()
        {
            m_templateChanging = true;
            base.OnApplyTemplate();
            Initialize(Template);
            m_templateChanging = false;
            m_itemsHost = GetTemplateChild(C_itemsHost) as Border;
            HeaderHost = GetTemplateChild(C_headerElement) as Border;
            m_Presenter = GetTemplateChild(C_nameContentHost) as ContentPresenter;
            m_stackItemsHost = GetTemplateChild(C_stackItemsHost) as Border;
            m_headerContent = GetTemplateChild(C_nameHeaderContent) as ContentPresenter;
            m_Naavigationtoolbar = GetTemplateChild("NavigationToolbar") as NavigationToolbar;
            UpdateSelectedTab();

            if (m_contentPopup != null && NavigationPopupSize != Size.Empty)
            {
                SetContentPopupSize();
            }
        }

        /// <summary>
        /// Measures the size in layout required for child elements.
        /// </summary>
        /// <param name="constraint">The available size that this element can give to child elements.</param>
        /// <returns>The size that this element determines it needs during layout, based on its calculations of child element sizes.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            if (!m_isOrientationChanging)
            {
                if (Orientation == Orientation.Vertical)
                {
                    if (MainHost != null && MainHost.ActualWidth != ActualWidth)
                    {
                        if (MainHost.ActualWidth == 0 || MainHost.ActualWidth < ActualWidth)
                        {
                            //MainHost.Width = ActualWidth;
                        }
                    }
                }
            }

            return base.MeasureOverride(constraint);
        }

        /// <summary>
        /// Adds the item to the collection of items.
        /// </summary>
        /// <param name="caption">Displayed text of the item.</param>
        /// <param name="image">Image source to the image.</param>
        /// <param name="index">Index in the collection where the specified item is located.</param>
        /// <param name="tagObject">Arbitrary object value.</param>
        /// <returns>New <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> object.</returns>
        public GroupBarItem AddTab(string caption, Image image, int index, object tagObject)
        {
            GroupBarItem item = new GroupBarItem { HeaderText = caption, Tag = tagObject };

            if (image != null)
            {
                item.HeaderImageSource = image.Source;
            }

            if (null == ItemsSource)
            {
                if (index > C_nullIndex)
                {
                    Items.Insert(index, item);
                }
                else
                {
                    if (VisualMode == VisualMode.StackMode && Toolbar != null)
                    {
                        index = Items.Count - Toolbar.Items.Count;
                        Items.Insert(index, item);
                        ItemContentLength -= ItemHeaderHeight;
                    }
                    else
                    {
                        index = Items.Add(item);
                    }
                }
            }
            
            return item;
        }

        /// <summary>
        /// Adds the item to collection of items.
        /// </summary>
        /// <returns>New <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> object.</returns>
        public GroupBarItem AddTab()
        {
            return AddTab(String.Empty, null, C_nullIndex, null);
        }

        /// <summary>
        /// Adds the item to the collection of items.
        /// </summary>
        /// <param name="index">Index in collection where the specified item is located.</param>
        /// <returns>New <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> object.</returns>
        public GroupBarItem AddTab(int index)
        {
            return AddTab(String.Empty, null, index, null);
        }

        /// <summary>
        /// Adds the item to the collection of items.
        /// </summary>
        /// <param name="caption">Displayed text of the item.</param>
        /// <returns>New <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> object.</returns>
        public GroupBarItem AddTab(string caption)
        {
            return AddTab(caption, null, C_nullIndex, null);
        }

        /// <summary>
        /// Adds the item to the collection of items.
        /// </summary>
        /// <param name="caption">Displayed text of the item.</param>
        /// <param name="image">Image source to the image.</param>
        /// <returns>New <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> object.</returns>
        public GroupBarItem AddTab(string caption, Image image)
        {
            return AddTab(caption, image, C_nullIndex, null);
        }

        /// <summary>
        /// Adds the item to the collection of items.
        /// </summary>
        /// <param name="caption">Displayed text of the item.</param>
        /// <param name="image">Image source to the image.</param>
        /// <param name="tagObject">Arbitrary object value.</param>
        /// <returns>New <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> object.</returns>
        public GroupBarItem AddTab(string caption, Image image, object tagObject)
        {
            return AddTab(caption, image, C_nullIndex, tagObject);
        }

        /// <summary>
        /// Rename the item from the collection of items.
        /// </summary>
        /// <param name="index">Index in collection where the specified item is located.</param>
        /// <param name="caption">Text to set.</param>
        /// <returns>Renamed <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> object.</returns>
        public GroupBarItem RenameTab(int index, string caption)
        {
            GroupBarItem item = null;

            if (index > -1 && index < Items.Count)
            {
                item = Items[index] as GroupBarItem;

                if (item != null)
                {
                    item.HeaderText = caption;
                }
            }

            return item;
        }

        /// <summary>
        /// Gets visual mode of the control according to
        /// <see cref="Syncfusion.Windows.Tools.VisualMode"/> enumeration.
        /// </summary>
        /// <param name="obj">Dependency object to get the visual mode from.</param>
        /// <returns>VisualMode VisualModeProperty</returns>
        public static VisualMode GetVisualMode(DependencyObject obj)
        {
            return (VisualMode)obj.GetValue(VisualModeProperty);
        }

        /// <summary>
        /// Sets visual mode of the control according to
        /// <see cref="Syncfusion.Windows.Tools.VisualMode"/> enumeration.
        /// </summary>
        /// <param name="obj">Dependency object to set the visual mode to.</param>
        /// <param name="value">The value of <see cref="Syncfusion.Windows.Tools.VisualMode"/> enumeration.</param>
        public static void SetVisualMode(DependencyObject obj, VisualMode value)
        {
            obj.SetValue(VisualModeProperty, value);
        }

        /// <summary>
        /// Gets the value indicating whether the context menu is enabled.
        /// </summary>
        /// <param name="obj">Dependency object to get the value from.</param>
        /// <returns>bool IsEnabledContextMenuProperty</returns>
        public static bool GetIsEnabledContextMenu(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledContextMenuProperty);
        }

        /// <summary>
        /// Sets the value indicating whether the context menu is enabled.
        /// </summary>
        /// <param name="obj">Dependency object to set the value to.</param>
        /// <param name="value">The Boolean value indicating whether the context menu is enabled.</param>
        public static void SetIsEnabledContextMenu(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEnabledContextMenuProperty, value);
        }

        /// <summary>
        /// Saves the state persisted for the current <see cref="GroupBar"/> location.
        /// </summary>
        public void SaveBarState()
        {
            IsolatedStorageFile isoStorage = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

            SaveBarState(isoStorage, m_StoreFileName);
        }

        /// <summary>
        /// Saves the state persisted for the current <see cref="GroupBar"/> location.
        /// </summary>
        /// <param name="isoStorage">Reference in isolated storage for saving the current <see cref="GroupBar"/> location.</param>
        /// <param name="storeFileName">File name for the isolated storage.</param>
        public void SaveBarState(IsolatedStorageFile isoStorage, string storeFileName)
        {
            if (null != isoStorage && !String.IsNullOrEmpty(storeFileName))
            {
                Stream stream = new IsolatedStorageFileStream(storeFileName, FileMode.Create, isoStorage);

                if (null != stream)
                {
                    try
                    {
                        GroupBarParams gbParamsList = new GroupBarParams(this);
                        XamlWriter.Save(gbParamsList, stream);
                    }
                    //SU I78477
                    //catch (InvalidOperationException ioe)
                    catch (InvalidOperationException)
                        //EU I78477
                    {
                        //Xaml writer has known issue while serializing generic list, this catch block will avoid throwing exception while saving and loading.
                    }
                    finally
                    {
                        stream.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Loads the state persisted.
        /// </summary>
        public void LoadBarState()
        {
            IsolatedStorageFile isoStorage = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

            m_isInLoadStateMode = true;
            LoadBarStateFromIsoStorage(isoStorage, m_StoreFileName);
        }

        /// <summary>
        /// Loads the state persisted.
        /// </summary>
        /// <param name="isoStorage">Reference in the isolated storage for
        /// loading the current docking location.</param>
        /// <param name="storeFileName">File name for the isolated storage.</param>
        public void LoadBarState(IsolatedStorageFile isoStorage, string storeFileName)
        {
            if (null != isoStorage && !String.IsNullOrEmpty(storeFileName)
                && 0 < isoStorage.GetFileNames(storeFileName).Length)
            {
                Stream stream = new IsolatedStorageFileStream(storeFileName, FileMode.OpenOrCreate, isoStorage);

                if (stream != null)
                {
                    m_isInLoadStateMode = true;
                    SetNewGroupBar(stream);
                    //if (IsSplitted)
                    //{
                    //    this.StackItemsHost.Height = this.StackItemsHost.ActualHeight - this.ItemHeaderHeight;
                    //    this.IsSplitted = false;
                    //}
                    //else
                    //{
                    //    this.StackItemsHost.Height = this.StackItemsHost.ActualHeight + this.ItemHeaderHeight;
                    //}
                }
            }
        }

        /// <summary>
        /// Resets the state persisted.
        /// </summary>
        public void ResetBarState()
        {
            if (SaveOriginalState)
            {
                IsolatedStorageFile isoStorage = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

                m_isInLoadStateMode = true;
                LoadBarStateFromIsoStorage(isoStorage, C_ResetStoreFileName);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Used for initializing the control.
        /// </summary>
        private void Initialize()
        {
            //ResourceDictionary dictionary = new ResourceDictionary
            //                                    {
            //                                        Source =
            //                                            new Uri(
            //                                            "pack://application:,,,/Syncfusion.Tools.WPF;component/Controls/GroupBar/Themes/LunaTemplate.xaml",
            //                                            UriKind.RelativeOrAbsolute)
            //                                    };
            //CollapseButtonTemplate = (ControlTemplate)dictionary["CollapseButtonTemplateKey"];
            CommandBinding popupBinding = new CommandBinding(m_popupCommand);
            popupBinding.Executed += new ExecutedRoutedEventHandler(PopupBinding_Executed);
            CommandBinding closeButtonBinding = new CommandBinding(m_closeButtonCommand);
            closeButtonBinding.Executed += new ExecutedRoutedEventHandler(CloseButton_Executed);
            CommandBindings.Add(popupBinding);
            CommandBindings.Add(closeButtonBinding);           
            BitmapImage defaultImage = new BitmapImage(new Uri("/Syncfusion.Tools.WPF;component/Controls/GroupBar/Resources/DefaultItemImage.png", UriKind.RelativeOrAbsolute));
            SetValue(GroupBar.DefaultItemImageProperty, defaultImage);
            FlowDirectionChanged += new FlowDirectionChangedEventHandler(GroupBar_FlowDirectionChanged);
        }

        /// <summary>
        /// When loading the GB, and setting selected other item, than 1st, than Content of the selected item is hidden, because Template is unknown.
        /// This method updates the selected tab when template is applied.
        /// </summary>
        private void UpdateSelectedTab()
        {
            GroupBarItem selectedTab = SelectedTab;

            if (selectedTab != null)
            {
                SelectNewTab(selectedTab);
            }
        }

        /// <summary>
        /// Occurs when push on the content button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void PopupBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (m_contentButton != null && m_contentButton.IsChecked == true)
            {
                IsDropDownOpen = true;
                m_popupContentHost.Focus();
            }

            GroupView view = SelectedContent as GroupView;
            if (view != null)
            {
                view.UpdateScrollViewer(this);
            }
        }

        /// <summary>
        /// Occurs when push on the close button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void CloseButton_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Checks whether there is enough space for showing the header of
        /// the given item.
        /// </summary>
        /// <param name="item">The <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> to check.</param>
        /// <returns>
        /// True, if there is enough space to allocate the item header;
        /// false, otherwise.
        /// </returns>
        internal bool IsItemFullyVisible(GroupBarItem item)
        {
            int index = Items.IndexOf(item);
            double condition = 0d;

            condition = (VisualMode == VisualMode.StackMode) ?
                AvailableHeadersHeight - (++index - HiddenItemsCount) * ItemHeaderHeight :
                AvailableHeadersHeight - (++index) * ItemHeaderHeight;

            bool returnValue = condition >= 0;
            return returnValue;
        }

        /// <summary>
        /// Hides the given item.
        /// </summary>
        /// <param name="item">The <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> to be hidden.</param>
        internal void HideItem(GroupBarItem item)
        {
            int index = Items.IndexOf(item);
            if (index == -1 && item.DataContext != null) 
                index = Items.IndexOf(item.DataContext);

            if (IsItemInToolbar(item))
            {
                NavigationToolbarItem toolBarItem = GetItemFromToolbar(item);
                m_toolbar.Items.Remove(toolBarItem);
            }
            else
            {
                ItemContentLength += m_splitter.DragIncrement;
            }

            item.SetAdornersVisibility(item.VisualHeader, Visibility.Collapsed);
            item.ShowInGroupBar = false;
            item.IsHidden = true;
            if (m_hiddenIndices.Count == Items.Count)
            {
                HeaderHost.Child.Visibility = Visibility.Collapsed;
                if (m_Presenter != null)
                {
                    m_Presenter.Visibility = Visibility.Collapsed;
                }
            }
            if (!m_hiddenIndices.Contains(index))
            {
                m_hiddenIndices.Add(index);
            }

            // set the proper selected tab when the selected tab is hide
            bool hasSelectedTab = true;

            if (SelectedTab == item)
            {
                if (this.ItemsSource == null)
                {
                    foreach (GroupBarItem groupBarItem in this.Items)
                    {
                        if (groupBarItem != item && groupBarItem.ShowInGroupBar == true)
                        {
                            SelectNewTab(groupBarItem);
                            hasSelectedTab = true;
                            break;
                        }
                        else
                        {
                            hasSelectedTab = false;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Items.Count; i++)
                    {
                        GroupBarItem gritem = ItemContainerGenerator.ContainerFromIndex(i) as GroupBarItem;
                        if (gritem != item && gritem.ShowInGroupBar == true)
                        {
                            SelectNewTab(gritem);
                            hasSelectedTab = true;
                            break;
                        }
                        else
                        {
                            hasSelectedTab = false;
                        }
                    }
                }
            }

            if (hasSelectedTab == false)
            {
                if (Toolbar != null && Toolbar.Items.Count > 0)
                {
                    ((NavigationToolbarItem)(Toolbar.Items[Toolbar.Items.Count - 1])).IsSelected = true;
                    hasSelectedTab = true;
                }
                else
                {
                    hasSelectedTab = false; 
                }
                //foreach (NavigationToolbarItem navItem in Toolbar.Items)
                //{
                //    if (navItem.Visibility == Visibility.Visible)
                //    {
                //        navItem.IsSelected = true;
                //        hasSelectedTab = true;
                //        break;
                //    }
                //    else
                //    {
                //        hasSelectedTab = false;
                //    }
                //}                         
            }
            if(!hasSelectedTab)
            {
                SelectedTab = null;
                SelectedContent = null;
                SelectedHeader = null;
                SelectedObject = null;                
            }
        }

        /// <summary>
        /// Shows the given item.
        /// </summary>
        /// <param name="item">The <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> to be shown.</param>
        internal void ShowItem(GroupBarItem item)
        {
            NavigationToolbarItem navItem;
            int index = Items.IndexOf(item);
            if (!IsItemInToolbar(item))
            {
                if (HeaderHost != null && m_Presenter != null)
                {
                    HeaderHost.Child.Visibility = Visibility.Visible;
                    m_Presenter.Visibility = Visibility.Visible;
                }
                navItem = new NavigationToolbarItem(item);
                int indexInToolbar = FindToolBarIndexToInsert(item);
                m_toolbar.Items.Insert(indexInToolbar, navItem);
                m_hiddenIndices.Remove(index);

                if (SelectedTab == null)
                    navItem.IsSelected = true;
            }
        }

        /// <summary>
        /// Updates indices of the hidden items after drag and drop is
        /// completed.
        /// </summary>
        /// <param name="dragSourceIndex">Index of the item being dragged.</param>
        /// <param name="dropTargetIndex">Index of the item on which drop was performed.</param>
        internal void UpdateHiddenItems(int dragSourceIndex, int dropTargetIndex)
        {
            for (int i = m_hiddenIndices.Count - 1; i >= 0; --i)
            {
                int index = m_hiddenIndices[i];

                if (index > dropTargetIndex && index < dragSourceIndex)
                {
                    ++m_hiddenIndices[i];
                }
                else if (index > dragSourceIndex && index < dropTargetIndex)
                {
                    --m_hiddenIndices[i];
                }
            }
        }

        /// <summary>
        /// Invoked when a new <see cref="GroupBarItem"/> is added into the <see cref="GroupBar"/>.
        /// </summary>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnGroupBarItemAdded(NotifyCollectionChangedEventArgs e)
        {
            if (GroupBarItemAdded != null)
            {
                GroupBarItemAdded(this, e);
            }
        }

        /// <summary>
        /// Invoked when an existing <see cref="GroupBarItem"/> is removed from the <see cref="GroupBar"/>.
        /// </summary>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnGroupBarItemRemoved(NotifyCollectionChangedEventArgs e)
        {
            if (GroupBarItemRemoved != null)
            {
                GroupBarItemRemoved(this, e);
            }
        }

        /// <summary>
        /// Invoked when the collapse button is clicked.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnCollapsedChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                CollapseHeaderContent();
            }
            else
            {
                ShowHeaderContent();
            }

            if (CollapsedChanged != null)
            {
                CollapsedChanged(this, e);
            }
        }

        /// <summary>
        /// Invoked before displaying the <see cref="GroupBarItem"/> in the popup container during the collapsed mode.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Tools.BeforeGroupBarItemPopupOpenedEventArgs"/> that contains the event data.</param>
        protected virtual void OnBeforeGroupBarItemPopupOpened(BeforeGroupBarItemPopupOpenedEventArgs e)
        {
            if (BeforeGroupBarItemPopupOpened != null)
            {
                BeforeGroupBarItemPopupOpened(this, e);
            }
        }

        /// <summary>
        /// Invoked after the popup container is closed.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> that contains the event data.</param>
        protected virtual void OnAfterGroupBarItemPopupClosed(EventArgs e)
        {
            if (AfterGroupBarItemPopupClosed != null)
            {
                AfterGroupBarItemPopupClosed(this, e);
            }
        }

        /// <summary>
        /// Invoked when the value of <see cref="Orientation"/> property is changed.
        /// </summary>
        protected virtual void OnOrientationChanged()
        {
            UpdateItemsSize();
            UpdateLayout();

            if (IsCollapsed)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    MaxHeight = MaxWidth;
                    MaxWidth = double.MaxValue;
                    MainHost.MaxWidth = double.MaxValue;
                }
                else
                {
                    MaxWidth = MaxHeight;
                    MaxHeight = double.MaxValue;
                    MainHost.MaxWidth = double.MaxValue;
                }
            }

            FireOrientationChanged();

            m_isOrientationChanging = false;
        }

        /// <summary>
        /// Invoked when the value of <see cref="Orientation"/> property is changing.
        /// </summary>
        protected virtual void OnOrientationChanging()
        {
            m_isOrientationChanging = true;
            UpdateItemsSize();
            RecalculateItemContentLength();
            UpdateLayout();

            FireOrientationChanging();
        }

        /// <summary>
        /// Invoked when the value of <see cref="VisualMode"/> property is changed.
        /// </summary>
        protected virtual void OnVisualModeChanged()
        {
            FireVisualModeChanged();
        }

        /// <summary>
        /// Invoked when the value of <see cref="FlowDirection"/> property is changed.
        /// </summary>
        protected virtual void OnFlowDirectionChanged()
        {
            FireFlowDirectionChanged();
        }

        /// <summary>
        /// Invoked when the control is ready for presentation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected virtual void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (CustomAnimations != null)
            {
                CustomAnimations.InitializeResources(LogicalParent);
            }

            InitializeRotationAngle();
            InitializeFreeSpace();

            if (HasItems)
            {
                if (Items.Count > 0 && SelectedObject == null)
                {
                    GroupBarItem item = Items[0] as GroupBarItem;

                    if (item != null)
                    {
                        SelectedObject = item;
                    }
                }
                else
                {
                    if (SelectedContent == null)
                    {
                        SelectedContent = SelectedTab.Content;
                    }
                    if (SelectedHeader == null)
                    {
                        SelectedHeader = SelectedTab.Header;
                    }
                }

                RefreshLastItem();
            }

            if (SaveOriginalState)
            {
                SaveInitialState();
            }

            //if (Orientation == Orientation.Horizontal)
            //{
            //    Height = ActualHeight;
            //    Width = ActualWidth;
            //    ContentRotationAngle = 90d;
            //}

            UpdateItemsSize();
            UpdateItemsContentVisibility();
            RecalculateItemContentLength();

            if (VisualMode == VisualMode.StackMode)
            {
                for (int i = 0; i < this.Items.Count; i++)
                {
                    GroupBarItem gitem = this.ItemContainerGenerator.ContainerFromIndex(i) as GroupBarItem;
                    if (gitem != null)
                    {
                        if (gitem.IsSelected == true && gitem.Visibility == Visibility.Visible)
                        {
                            FrameworkElement tempcontent = gitem.Content as FrameworkElement;
                            FrameworkElement tempheader = this.HeaderContent as FrameworkElement;
                            GroupBarItem.SetContentVisibility(Visibility.Visible, tempcontent, tempheader);
                        }
                        else if (gitem.IsSelected == true && !gitem.ShowInGroupBar)
                        {
                            FrameworkElement tempcontent = gitem.Content as FrameworkElement;
                            FrameworkElement tempheader = this.HeaderContent as FrameworkElement;
                            GroupBarItem.SetContentVisibility(Visibility.Visible, tempcontent, tempheader);
                        }
                    }
                }
            }

            SetNavigationPopupSizeProperty();

            ////SelectFirstShowedItem();
        }

        /// <summary>
        /// Sets content size to the popup if user didn't set the NavigationPopupSize property.
        /// </summary>
        private void SetNavigationPopupSizeProperty()
        {
            if (NavigationPopupSize == Size.Empty)
            {
                double newWidth = m_defaultPopupSize.Width;
                double newHeight = m_defaultPopupSize.Height;

                if (SelectedContent is FrameworkElement)
                {
                    FrameworkElement elem = SelectedContent as FrameworkElement;

                    if (elem.ActualWidth != 0)
                    {
                        newWidth = elem.ActualWidth + C_defaultMargin * 2;
                    }
                    else
                    {
                        if (m_cachedWidth != 0)
                        {
                            newWidth = m_cachedWidth + C_defaultMargin * 2;
                        }
                    }

                    if (elem.ActualHeight > 0)
                    {
                        double fullHeight = elem.ActualHeight + C_defaultMargin * 2;

                        if (fullHeight < m_oldPopupHeight)
                        {
                            fullHeight = m_oldPopupHeight;
                        }

                        newHeight = fullHeight;
                        m_oldPopupHeight = fullHeight;
                    }
                }
                else
                {
                    newWidth = m_cachedWidth + C_defaultMargin * 2;
                }

                NavigationPopupSize = new Size(newWidth, newHeight);
            }
        }

        /// <summary>
        /// Selects first item that has ShowInGroupBar=true.
        /// </summary>
        private void SelectFirstShowedItem()
        {
            if (Items.Count > 1 && !SelectedTab.ShowInGroupBar)
            {
                int index = Items.IndexOf(SelectedTab);

                if (index < Items.Count - 1)
                {
                    for (int i = index, cnt = Items.Count; i < cnt; i++)
                    {
                        GroupBarItem item = (GroupBarItem)Items[i];

                        if (item.ShowInGroupBar)
                        {
                            SelectedTab = item;
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = Items.Count - 1; i > 0; i--)
                    {
                        GroupBarItem item = (GroupBarItem)Items[i];

                        if (item.ShowInGroupBar)
                        {
                            SelectedTab = item;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Recalculates internal content height.
        /// </summary>
        protected void RecalculateItemContentLength()
        {
            int itemsCount = Items.Count;

            m_bordersHeight = BorderThickness.Top + BorderThickness.Bottom;
            double imHeight = Orientation == Orientation.Vertical ? Height : Width;
            if (LogicalParent != null)
            {
                if (double.IsNaN(imHeight))
                {
                    imHeight = Orientation == Orientation.Vertical ? LogicalParent.ActualHeight : LogicalParent.ActualWidth;
                }
            }

            if (VisualMode == VisualMode.StackMode && IsToolBarEnabled)
            {
                imHeight -= StackModeHeightCorrection;
                itemsCount -= ToolbarItemsCount;
            }

            double length = imHeight - (itemsCount - HiddenItemsCount) * ItemHeaderHeight - m_bordersHeight;
            length = length - (MaxBottomCornerRadius + MaxTopCornerRadius);

            if (VisualMode == VisualMode.StackMode && !IsToolBarEnabled)
            {
                length = imHeight - HeaderHeight;
            }

            if (length > 0)
            {
                ItemContentLength = length;
            }
            else
            {
                ItemContentLength = 0;
            }
        }

        /// <summary>
        /// Invoked whenever an unhandled <see cref="System.Windows.UIElement.GotFocus"/> event reaches this element in its route. 
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.KeyUp"/> attached event
        /// reaches an element in its route that is derived from this class.
        /// Is used for key up / key down navigation. 
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            GroupBarItem container = GetGroupBarItemFromChildren(e.Source as FrameworkElement);

            if (container != null && !(container.Content is GroupView))
            {
                SelectedObject = container;
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.PreviewKeyDown"/> attached event reaches 
        /// an element in its route that is derived from this class. 
        /// Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
        }

        /// <summary>
        /// Called when some dependencyProperty is changed.
        /// </summary>
        /// <param name="e">Provides data for various property changed events. Typically these events report effective value changes in the value of a read-only dependency property.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == SkinStorage.VisualStyleProperty)
            {
                m_isVisualSkinChanged = true;

                ////if ((string)e.NewValue == C_defaultVisStyleName)
                ////{
                ////    ChangePropertiesToDefaultValues();
                ////}
                ////else
                ////{
                ////    UserUpdateDefaultSkin(e);
                ////}

                if (m_visualContent != null)
                {
                    m_visualContent.Content = null;
                    m_visualContent.Content = SelectedContent;
                }
            }

            ////else
            ////{
            ////    UserUpdateDefaultSkin(e);
            ////}
        }

        /////// <summary>
        /////// Changes default skin properties values to values that user had been set.
        /////// </summary>
        /////// <param name="e">Provides data for various property changed events. Typically these events report effective value changes in the value of a read-only dependency property.</param>
        ////private void UserUpdateDefaultSkin(DependencyPropertyChangedEventArgs e)
        ////{
        ////    if (e.Property == BackgroundProperty)
        ////    {
        ////        m_defaultBackground = Background;
        ////    }
        ////    else if (e.Property == BorderBrushProperty)
        ////    {
        ////        m_defaultBorderBrush = BorderBrush;
        ////    }
        ////}

        /////// <summary>
        /////// Sets default values to the properties.
        /////// </summary>
        ////private void ChangePropertiesToDefaultValues()
        ////{
        ////    Background = m_defaultBackground;
        ////    BorderBrush = m_defaultBorderBrush;
        ////}

        #region Overrides

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container. 
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>Returns true if the item is (or is eligible to be) its own container; 
        /// otherwise, false. </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is GroupBarItem;
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            GroupBarItem con_element = new GroupBarItem();
            return con_element;
        }

        /// <summary>
        /// Prepare Container for the particular item.
        /// </summary>
        /// <param name="element">The container element used to display the given item.</param>
        /// <param name="item">The item for which the container has to be prepared.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            GroupBarItem con_element = element as GroupBarItem;

            if (item is GroupBarItem)
            {
                base.PrepareContainerForItemOverride(con_element, item);
            }
            else
            {
                if (con_element.Header == null)
                    con_element.Header = item;
                if(con_element.Content == null)
                    con_element.Content = item;
               

                if (ItemTemplate!=null && con_element.HeaderTemplate==null)
                {
                    con_element.HeaderTemplate = this.ItemTemplate;
                }

                if (ItemTemplateSelector != null)
                {
                    con_element.HeaderTemplateSelector = ItemTemplateSelector;
                }
                if (null != HeaderContent)
                {
                    this.HeaderContent.ContentTemplate = con_element.HeaderTemplate;
                    if (con_element.HeaderTemplateSelector != null)
                    {
                        this.HeaderContent.ContentTemplateSelector = con_element.HeaderTemplateSelector;
                    }
                    else
                    {
                        this.HeaderContent.ContentTemplateSelector = ItemTemplateSelector;
                    }
                }
                if (null != m_visualContent)
                {
                    m_visualContent.ContentTemplate = con_element.ContentTemplate;
                    m_visualContent.ContentTemplateSelector = con_element.ContentTemplateSelector;
                }
                if (PopupContentHost != null)
                {
                    PopupContentHost.ContentTemplate = con_element.ContentTemplate;
                    PopupContentHost.ContentTemplateSelector = con_element.ContentTemplateSelector;
                }
                
                base.PrepareContainerForItemOverride(con_element, con_element);
                int index = Items.IndexOf(item);

                if (index == 0)
                {
                    SelectedTab = con_element;
                    SelectFirstShowedItem();
                }
                if (Toolbar != null)
                {
                    Toolbar.RefreshButtonsMenu();
                }
            }
        }
        #endregion

        /// <summary>
        /// Invoked when the Items property changes. 
        /// </summary>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> that contains the event data.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            UpdateItemsSize();

            if (IsLoaded)
            {
                if (!m_isInLoadStateMode && e.Action == NotifyCollectionChangedAction.Add)
                {
                    VisibleItemsCount++;
                    double newContentLength = ItemContentLength - ItemHeaderHeight;

                    if (newContentLength >= DEF_VIEWCONTENT_HEIGHT)
                    {
                        ItemContentLength = newContentLength;
                    }
                    else if (e.NewItems != null)
                    {
                        for (int i = Items.Count - 1; i >= 0; --i)
                        {
                            GroupBarItem item = Items[i] as GroupBarItem;

                            if (e.NewItems.Contains(item))
                            {
                                item.ShowInGroupBar = false;
                                m_hiddenIndices.Add(i);

                                if (VisualMode == VisualMode.StackMode)
                                {
                                    ShowItem(item);
                                }
                            }
                        }
                    }
                    if (HeaderHost != null)
                    {
                        if (HeaderHost.Child.Visibility == Visibility.Collapsed)
                        {
                            HeaderHost.Child.Visibility = Visibility.Visible;
                        }
                    }
                    if (m_toolbar != null)
                    {
                        m_toolbar.RefreshButtonsMenu();
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    VisibleItemsCount--;
                    ItemContentLength += ItemHeaderHeight;
                    if (this.Items.Count == 0 )
                    {
                        if (HeaderHost != null)
                        {
                            HeaderHost.Child.Visibility = Visibility.Collapsed;
                        }
                        if (m_Presenter != null)
                        {
                            m_Presenter.Visibility = Visibility.Collapsed;
                        }
                    }
                    if (m_Naavigationtoolbar != null)
                    {
                        if (m_Naavigationtoolbar.Items.Count > 0)
                        {
                            int index = ToolbarItemsCount - 1;
                            NavigationToolbarItem navItem = m_toolbar.Items[index] as NavigationToolbarItem;
                            m_toolbar.Items.Remove(navItem);
                        }
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    if (m_Presenter != null)
                    {
                        if (m_Presenter.Content != null)
                        {
                            //Commented to fix the issue in GroupBar while sorting, the GroupViewItems gets clear.
                            // m_Presenter.Visibility = Visibility.Collapsed; 
                            if (m_Presenter.Content == null)
                            {
                                HeaderHost.Child.Visibility = Visibility.Collapsed;
                            }

                            if (m_Naavigationtoolbar != null && !IsInSorting)
                                m_Naavigationtoolbar.Items.Clear();
                        }
                    }
                }

                if (m_toolbar != null)
                {
                    m_toolbar.RefreshButtonsMenu();
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                IList list = new ArrayList();
                for (int i = 0; i < e.NewItems.Count; i++)
                {
                    GroupBarItem item = ItemContainerGenerator.ContainerFromIndex(i) as GroupBarItem;
                    if (null != item)
                    {
                        list.Add(item);
                    }
                }

                if (list.Count > 0)
                {
                    NotifyCollectionChangedEventArgs args =
                        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, e.NewItems, e.NewStartingIndex);
                    OnGroupBarItemAdded(args);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems.Count > 0)
                {
                    OnGroupBarItemRemoved(e);
                }
            }

            RefreshLastItem();
            UpdateLayout();
            VisibleItemsCount = Items.Count;

            //newly added
            m_hiddenIndices.Clear();

            for (int i = 0, cnt = Items.Count; i < cnt; i++)
            {
                FrameworkElement item;

                if (Items[i] is GroupBarItem)
                {
                    item = (FrameworkElement)Items[i];
                    
                }
                else
                {
                    item = (FrameworkElement)ItemContainerGenerator.ContainerFromIndex(i);
                    RecalculateItemContentLength();
                }

                if (null != item)
                {
                    GroupBarItem hideItem = item as GroupBarItem;
                    if (!hideItem.IsHidden)
                    {
                        m_hiddenIndices.Remove(i);
                    }
                    else
                    {
                        m_hiddenIndices.Add(i);
                    }
                }
            }
            RecalculateItemContentLength();

            if (this.Items.Count == 0 && this.MainHost != null)
                this.MainHost.Visibility = Visibility.Collapsed;
            else if (this.MainHost != null)
                this.MainHost.Visibility = Visibility.Visible;

            base.OnItemsChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            for (int i = Items.Count - 1; i >= 0; --i)
            {
                GroupBarItem item = Items[i] as GroupBarItem;

                if (null != item)
                {
                    //if (!item.ShowInGroupBar || item.Visibility != Visibility.Visible)
                    if (item.IsHidden)
                    {
                        if (!m_hiddenIndices.Contains(i))
                        {
                            m_hiddenIndices.Add(i);
                        }
                        ////if (!item.ShowInGroupBar && item.IsSelected)
                        ////{
                        ////   GroupBarItem.SetContentVisibility(Visibility.Visible, SelectedContent as FrameworkElement, HeaderContent as FrameworkElement);
                        ////}
                    }
                }
            }

            Loaded += new RoutedEventHandler(OnLoaded);
            ApplyTemplate();

            ////Shared.DictionaryList list1 = SkinStorage.GetVisualStylesList(this);
            ////if (list1 != null)
            ////{
            ////    Shared.DictionaryList list2 = list1["Default"] as Shared.DictionaryList;
            ////    if (list2 != null)
            ////    {
            ////        if (list2.ContainsKey("GroupBar_Content") &&
            ////            m_defaultBackground == null)
            ////        {
            ////            m_defaultBackground = list2["GroupBar_Content"] as Brush;
            ////            Background = m_defaultBackground;
            ////        }

            ////        if (list2.ContainsKey("GroupBar_BorderBrush") &&
            ////            m_defaultBorderBrush == null)
            ////        {
            ////            m_defaultBorderBrush = list2["GroupBar_BorderBrush"] as Brush;
            ////            BorderBrush = m_defaultBorderBrush;
            ////        }
            ////    }
            ////}
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.FrameworkElement.SizeChanged"/> event, 
        /// using the specified information as part of the eventual event data. 
        /// </summary>
        /// <param name="sizeInfo">Details of the old and new size involved in the change.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (!AnimationInProgress)
            {
                UpdateItemsSize();

                //if (Orientation == Orientation.Vertical)
                //{
                    RecalculateItemContentLength();
                //}               
                if (!IsCollapsed)
                {
                    if (Orientation == Orientation.Vertical)
                    {
                        m_orientationChangedWidth = ActualWidth;
                        m_orientationChangedHeight = ActualHeight;
                    }
                    else
                    {
                        m_orientationChangedWidth = ActualHeight;
                        m_orientationChangedHeight = ActualWidth;
                    }
                }               
            }
        }

        /// <summary>
        /// Raises <see cref="OrientationChanging"/> event.
        /// </summary>
        private void FireOrientationChanging()
        {
            OrientationChangeEventArgs args = new OrientationChangeEventArgs(OrientationChangingEvent) { To = Orientation };
            RaiseEvent(args);
        }

        /// <summary>
        /// Raises <see cref="OrientationChanged"/> event.
        /// </summary>
        private void FireOrientationChanged()
        {
            OrientationChangeEventArgs args = new OrientationChangeEventArgs(OrientationChangedEvent) { To = Orientation };
            RaiseEvent(args);
        }

        /// <summary>
        /// Raises <see cref="FlowDirectionChanged"/> event.
        /// </summary>
        private void FireFlowDirectionChanged()
        {
            FlowDirectionChangedEventArgs args = new FlowDirectionChangedEventArgs(FlowDirectionChangedEvent) { To = FlowDirection };
            RaiseEvent(args);
        }

        /// <summary>
        /// Raises <see cref="VisualModeChanged"/> event.
        /// </summary>
        private void FireVisualModeChanged()
        {
            RoutedEventArgs args = new RoutedEventArgs(VisualModeChangedEvent);
            RaiseEvent(args);
        }

        /// <summary>
        /// Raises <see cref="BeforeSplitUp"/> event.
        /// </summary>
        internal void FireBeforeSplitUp()
        {
            RoutedEventArgs args = new RoutedEventArgs(BeforeSplitUpEvent);
            RaiseEvent(args);
        }

        /// <summary>
        /// Raises <see cref="BeforeSplitDown"/> event.
        /// </summary>
        internal void FireBeforeSplitDown()
        {
            RoutedEventArgs args = new RoutedEventArgs(BeforeSplitDownEvent);
            RaiseEvent(args);
        }

        /// <summary>
        /// Raises <see cref="AfterSplitUp"/> event.
        /// </summary>
        internal void FireAfterSplitUp()
        {
            RoutedEventArgs args = new RoutedEventArgs(AfterSplitUpEvent);
            RaiseEvent(args);
        }

        /// <summary>
        /// Raises <see cref="AfterSplitDown"/> event.
        /// </summary>
        internal void FireAfterSplitDown()
        {
            RoutedEventArgs args = new RoutedEventArgs(AfterSplitDownEvent);
            RaiseEvent(args);
        }

        /// <summary>
        /// Raises <see cref="SelectedObjectChanged"/> event.
        /// </summary>
        private void FireSelectedObjectChanged()
        {
            RoutedEventArgs args = new RoutedEventArgs(SelectedObjectChangedEvent);
            RaiseEvent(args);
        }

        /// <summary>
        /// Raises <see cref="SelectedItemChanged"/> event.
        /// </summary>
        private void FireSelectedItemChanged()
        {
            RoutedEventArgs args = new RoutedEventArgs(SelectedItemChangedEvent);
            RaiseEvent(args);
        }

        /// <summary>
        /// Raises <see cref="SelectedTabChanged"/> event.
        /// </summary>
        private void FireSelectedTabChanged()
        {
            RoutedEventArgs args = new RoutedEventArgs(SelectedTabChangedEvent);
            RaiseEvent(args);
        }

        /// <summary>
        /// Gets the last visible item in stack.
        /// </summary>
        /// <returns>
        /// The last visible item in stack.
        /// </returns>
        private GroupBarItem GetLastStackItem()
        {
            GroupBarItem item = null;

            for (int i = Items.Count - 1; i >= 0; --i)
            {
                item = ItemContainerGenerator.ContainerFromIndex(i) as GroupBarItem;
                if (item != null)
                {
                    if (!IsItemInToolbar(item) && item.ShowInGroupBar)
                    {
                        break;
                    }
                }
                
            }

            return item;
        }

        /// <summary>
        /// Updates the value of <see cref="ItemContentLength"/> property according to the
        /// change of height or width. Method checks items with not fully
        /// visible adorners in non-stack mode and collapses those
        /// adorners.
        /// </summary>
        /// <param name="oldValue">Old value of height or width.</param>
        /// <param name="newValue">New value of height or width.</param>
        private void UpdateItemContentLength(double oldValue, double newValue)
        {
            double propertyChange = newValue - oldValue;
            /* if moving up */
            if (propertyChange < 0)
            {
                /* try decreasing content area height - it will actually be equal 0,
                 * if trying to set negative value */
                ItemContentLength += propertyChange;
                /* checking whether we are "eating" items. In stack mode items are pushed to
                 * to toolbar when attempted to be "eaten" */
                if (ItemContentLength == 0 && VisualMode != VisualMode.StackMode)
                {
                    double newFreeSpace = GetFreeSpace(newValue);
                    /* previous value of free space in control */
                    double oldFreeSpace = GetFreeSpace(oldValue);

                    /* number of items with already removed adorners */
                    int processedItemsCount = 0;
                    /* if we have already "eaten" items, then old free space is negative.
                     * oldFreeSpace / HeaderHeight -> how many items are not fully visible */
                    if (oldFreeSpace <= 0)
                    {
                        processedItemsCount = (int)Math.Ceiling(Math.Abs(oldFreeSpace) / ItemHeaderHeight);
                    }

                    /* index of the last (during this iteration) not fully visible item */
                    int i = Items.Count - 1 - processedItemsCount;

                    /* i >= 0  -> there are unprocessed items in the control
                     * new free space > processed items * header height -> more items are not fully visible */
                    if (i >= 0
                        && Math.Abs(newFreeSpace) > processedItemsCount * ItemHeaderHeight)
                    {
                        double counter = newFreeSpace;
                        /* taking all items in current free space change, incrementing new free space
                         * by header height */
                        double condition = oldFreeSpace > 0 ? 0 : oldFreeSpace;

                        while (counter < condition && i >= 0)
                        {
                            GroupBarItem item = Items[i--] as GroupBarItem;
                            item.SetAdornersVisibility(item.VisualHeader, Visibility.Collapsed);
                            counter += ItemHeaderHeight;
                        }
                    }
                }
            }
            else if (propertyChange > 0)
            {
                double newFreeSpace = GetFreeSpace(newValue);
                double prevItemContentLength = ItemContentLength;

                ItemContentLength += newFreeSpace;

                if (VisualMode != VisualMode.StackMode)
                {
                    double oldFreeSpace = GetFreeSpace(oldValue);

                    int processedItemsCount = 0;

                    if (oldFreeSpace < 0)
                    {
                        processedItemsCount = (int)Math.Ceiling(Math.Abs(oldFreeSpace) / ItemHeaderHeight);
                    }

                    int i = Items.Count - processedItemsCount;

                    if (i < 0)
                    {
                        i = 0;
                        processedItemsCount = Items.Count;
                    }

                    if ((i < Items.Count && Math.Abs(newFreeSpace) <= (processedItemsCount - 1) * ItemHeaderHeight) || (prevItemContentLength == 0 && ItemContentLength > 0))
                    {
                        double counter = oldFreeSpace;
                        double condition = newFreeSpace > 0 ? 0 : newFreeSpace;

                        while (counter <= condition && i < Items.Count)
                        {
                            GroupBarItem item = Items[i++] as GroupBarItem;
                            item.SetAdornersVisibility(item.VisualHeader, Visibility.Visible);
                            counter += ItemHeaderHeight;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when orientation change animation is completed. Method
        /// sets new values for height and width properties of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> that contains the event data.</param>
        private void OnOrientationStoryboardCompleted(object sender, EventArgs e)
        {
            if (Orientation == Orientation.Vertical)
            {
                if (IsCollapsed)
                {
                    Width = MainHost.ActualWidth;
                    Height = MainHost.ActualHeight;
                }
                else
                {
                    Width = m_orientationChangedWidth;
                    Height = m_orientationChangedHeight;
                }

                if (m_contentPopup != null)
                {
                    m_contentPopup.LayoutTransform = new RotateTransform(0);
                }
            }
            else
            {
                if (IsCollapsed)
                {
                    Height = MainHost.ActualWidth;
                    Width = MainHost.ActualHeight;
                }
                else
                {
                    Width = m_orientationChangedHeight;
                    Height = m_orientationChangedWidth;
                }

                if (m_contentPopup != null)
                {
                    m_contentPopup.LayoutTransform = new RotateTransform(90);
                }
            }

            if (VisualMode == VisualMode.MultipleExpansion)
            {
                MainHost.Height = double.NaN;
                MainHost.Width = double.NaN;
            }

            AnimationInProgress = false;

            OnOrientationChanged();
        }

        /// <summary>
        /// Gets the free space made by height or width change.
        /// </summary>
        /// <param name="imHeight">The height of the control (within vertical orientation),
        /// the width of the control (within horizontal orientation).</param>
        /// <returns>
        /// The amount of free space to be allocated.
        /// </returns>
        private double GetFreeSpace(double imHeight)
        {
            double ret = imHeight - VisibleItemsCount * ItemHeaderHeight - ItemContentLength - m_bordersHeight;

            if (VisualMode == VisualMode.StackMode)
            {
                ret -= StackModeHeightCorrection - (HiddenItemsCount + ToolbarItemsCount) * ItemHeaderHeight;
            }

            if (ret < 0)
                ret = 0;

            return ret;
        }

        /// <summary>
        /// Called when splitter fires DragIncremented event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        private void OnSplitterDragIncremented(object sender, RoutedEventArgs e)
        {
            GroupBarSplitter splitter = sender as GroupBarSplitter;

            if (splitter != null)
            {
                DoSplitting(splitter.DragDirection);
            }
        }

        /// <summary>
        /// Performs the splitting of stack host.
        /// </summary>
        /// <param name="direction">Direction of splitting.</param>
        private void DoSplitting(DragDirection direction)
        {
            m_isSplitting = true;
            
            if (direction == DragDirection.Down && StackItemsCount > 0)
            {
                GroupBarItem lastStackItem = GetLastStackItem();
                if (lastStackItem != null)
                {
                    lastStackItem.SetAdornersVisibility(lastStackItem.VisualHeader, Visibility.Collapsed);
                    lastStackItem.Visibility = Visibility.Collapsed;
                    NavigationToolbarItem navItem = new NavigationToolbarItem(lastStackItem);
                    if (lastStackItem.ShowInGroupBar)
                        m_toolbar.Items.Add(navItem);
                    //  lastStackItem.ShowInGroupBar = false;
                    double bordersHeight = BorderThickness.Top + BorderThickness.Bottom;
                    double splitterMargin = Splitter.Margin.Top + Splitter.Margin.Bottom;
                    double elementsHeight = HeaderHeight + Splitter.Height + splitterMargin + bordersHeight + m_toolbar.ActualHeight;
                    if (!(ItemContentLength + elementsHeight >= this.DesiredSize.Height))
                        ItemContentLength += ItemHeaderHeight;
                
                }
            }
            else if (direction == DragDirection.Up && ToolbarItemsCount > 0)
            {
                int index = ToolbarItemsCount - 1;
                NavigationToolbarItem navItem = m_toolbar.Items[index] as NavigationToolbarItem;
                GroupBarItem item = navItem.GroupBarItem;
                item.SetAdornersVisibility(item.VisualHeader, Visibility.Visible);
                item.Visibility = Visibility.Visible;
                item.ShowInGroupBar = true;
                m_toolbar.Items.Remove(navItem);
                ItemContentLength -= ItemHeaderHeight;
                if (StackItemHostHeight + ItemHeaderHeight < Height)
                {
                    //UnCommented the below line for fixing the issue with GroupBar while move the Splitter, navigation toolbar position changed.
                    this.StackItemsHost.Height = this.StackItemsHost.ActualHeight + this.ItemHeaderHeight;
                }
            }
            IsSplitted = true;
            m_isSplitting = false;
        }

        /// <summary>
        /// Checks whether the item with the given index is in the toolbar.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>
        /// True, if item is in toolbar; false, otherwise.
        /// </returns>
        private bool IsItemInToolbar(GroupBarItem item)
        {
            bool contains = false;
            foreach (NavigationToolbarItem toolBarItem in Toolbar.Items)
            {
                if (toolBarItem.GroupBarItem == item)
                {
                    contains = true;
                    break;
                }
            }

            return contains;
        }

        /// <summary>
        /// Gets the <see cref="NavigationToolbarItem"/> that corresponds <see cref="GroupBarItem"/> 
        /// from the toolBar.
        /// </summary>
        /// <param name="item">Index of the item to check.</param>
        /// <returns>
        /// True, if item is in toolbar; false, otherwise.
        /// </returns>
        private NavigationToolbarItem GetItemFromToolbar(GroupBarItem item)
        {
            NavigationToolbarItem itemToReturn = null;
            foreach (NavigationToolbarItem toolBarItem in Toolbar.Items)
            {
                if (toolBarItem.GroupBarItem == item)
                {
                    itemToReturn = toolBarItem;
                    break;
                }
            }

            return itemToReturn;
        }

        /// <summary>
        /// Finds the index for inserting a new item in the toolBar.
        /// </summary>
        /// <param name="item">The item to find the insertion index for.</param>
        /// <returns>
        /// True, if item is in toolbar; false, otherwise.
        /// </returns>
        private int FindToolBarIndexToInsert(GroupBarItem item)
        {
            int index = -1;
            int itemIndex = Items.IndexOf(item);

            for (int i = m_toolbar.Items.Count - 1; i >= 0; i--)
            {
                NavigationToolbarItem toolBarItem = m_toolbar.Items[i] as NavigationToolbarItem;
                if (itemIndex < Items.IndexOf(toolBarItem.GroupBarItem))
                {
                    index = m_toolbar.Items.Count;
                    break;
                }
                else if (i - 1 >= 0)
                {
                    NavigationToolbarItem nextToolBarItem = m_toolbar.Items[i - 1] as NavigationToolbarItem;
                    if (itemIndex > Items.IndexOf(toolBarItem.GroupBarItem) &&
                        itemIndex < Items.IndexOf(nextToolBarItem.GroupBarItem))
                    {
                        index = m_toolbar.Items.IndexOf(toolBarItem);
                        break;
                    }
                }
            }

            if (m_toolbar.Items.Count == 0)
            {
                index = m_toolbar.Items.Count;
            }

            if (index == -1)
            {
                index = 0;
            }

            return index;
        }

        /// <summary>
        /// Gets the index in the toolbar of the item with the given index.
        /// </summary>
        /// <param name="itemIndex">Index of the item.</param>
        /// <returns>
        /// Index of the toolbar item that corresponds to the item with the
        /// given index.
        /// </returns>
        private int GetItemIndexInToolbar(int itemIndex)
        {
            return Items.Count - HiddenItemsCount - GetItemUIIndex(itemIndex) - 1;
        }

        /// <summary>
        /// Calculates the item index in the toolbar when the item is shown after
        /// been hidden.
        /// </summary>
        /// <param name="itemIndex">Index of the item.</param>
        /// <returns>
        /// Index of the toolbar item that corresponds to the item with the
        /// given index.
        /// </returns>
        private int CalculateItemIndexInToolbar(int itemIndex)
        {
            return GetItemIndexInToolbar(itemIndex) + 1;
        }

        /// <summary>
        /// Gets the item index as it is presented in stack host. Methods
        /// corrects the real item index by the number of possibly hidden
        /// items before the given item.
        /// </summary>
        /// <param name="itemIndex">Index of the item.</param>
        /// <returns>
        /// Item index as it is presented in the stack host.
        /// </returns>
        private int GetItemUIIndex(int itemIndex)
        {
            int uiIndex = itemIndex;

            foreach (int index in m_hiddenIndices)
            {
                if (index < itemIndex)
                {
                    --uiIndex;
                }
            }

            return uiIndex;
        }

        /// <summary>
        /// Initializes control by the template.
        /// </summary>
        /// <param name="inTemplate">Template to initialize control by.</param>
        private void Initialize(ControlTemplate inTemplate)
        {
            if (CustomAnimations != null)
            {
                CustomAnimations.Initialize(this, Template);
            }

            switch (VisualMode)
            {
                case VisualMode.StackMode:
                    {
                        InitializeStackMode(inTemplate);
                        break;
                    }

                case VisualMode.MultipleExpansion:
                    {
                        InitializeMultipleExpansionMode(inTemplate);
                        break;
                    }

                default:
                    {
                        InitializeNonStackMode(inTemplate);
                        break;
                    }
            }

            if (IsLoaded)
            {
                InitializeRotationAngle();
                InitializeFreeSpace();
                OnVisualModeChanged();
                RecalculateItemContentLength();
            }

            InitializeContextMenu();
        }

        /// <summary>
        /// Initializes the context menu.
        /// </summary>
        private void InitializeContextMenu()
        {
            if (ContextMenu != null)
            {
                ContextMenu.Opened += new RoutedEventHandler(ContextMenu_Opened);
                ContextMenu.LostMouseCapture += new MouseEventHandler(ContextMenu_LostMouseCapture);

                CommandBinding bindingAddItemCommand = new CommandBinding(GroupBar.AddItemCommand, GroupBar.AddItemCommandExecute, GroupBar.AddItemCommandCanExecute);
                ContextMenu.CommandBindings.Add(bindingAddItemCommand);

                CommandBinding bindingRenameItemCommand = new CommandBinding(GroupBar.RenameItemCommand, GroupBar.RenameItemCommandExecute, GroupBar.RenameItemCommandCanExecute);
                ContextMenu.CommandBindings.Add(bindingRenameItemCommand);

                CommandBinding bindingDeleteItemCommand = new CommandBinding(GroupBar.DeleteItemCommand, GroupBar.DeleteItemCommandExecute, GroupBar.DeleteItemCommandCanExecute);
                ContextMenu.CommandBindings.Add(bindingDeleteItemCommand);

                CommandBinding bindingRenameTabCommand = new CommandBinding(GroupBar.RenameTabCommand, GroupBar.RenameTabCommandExecute, GroupBar.RenameTabCommandCanExecute);
                ContextMenu.CommandBindings.Add(bindingRenameTabCommand);

                CommandBinding bindingDeleteTabCommand = new CommandBinding(GroupBar.DeleteTabCommand, GroupBar.DeleteTabCommandExecute, GroupBar.DeleteTabCommandCanExecute);
                ContextMenu.CommandBindings.Add(bindingDeleteTabCommand);

                CommandBinding bindingAddTabCommand = new CommandBinding(GroupBar.AddTabCommand, GroupBar.AddTabCommandExecute, GroupBar.AddTabCommandCanExecute);
                ContextMenu.CommandBindings.Add(bindingAddTabCommand);

                CommandBinding changeListViewModeCommandBinding = new CommandBinding(GroupBar.ChangeListViewModeCommand, GroupBar.ChangeListViewModeCommandExecute, GroupBar.ChangeListViewModeCommandCanExecute);
                ContextMenu.CommandBindings.Add(changeListViewModeCommandBinding);

                CommandBinding sortAscCommandBinding = new CommandBinding(GroupBar.SortAscCommand, GroupBar.SortAscCommandExecute, GroupBar.SortAscCommandCanExecute);
                ContextMenu.CommandBindings.Add(sortAscCommandBinding);

                CommandBinding sortDesCommandBinding = new CommandBinding(GroupBar.SortDesCommand, GroupBar.SortDesCommandExecute, GroupBar.SortDesCommandCanExecute);
                ContextMenu.CommandBindings.Add(sortDesCommandBinding);

                CommandBinding moveUpCommandBinding = new CommandBinding(GroupBar.MoveUpCommand, GroupBar.MoveUpCommandExecute, GroupBar.MoveUpCommandCanExecute);
                ContextMenu.CommandBindings.Add(moveUpCommandBinding);

                CommandBinding moveDownCommandBinding = new CommandBinding(GroupBar.MoveDownCommand, GroupBar.MoveDownCommandExecute, GroupBar.MoveDownCommandCanExecute);
                ContextMenu.CommandBindings.Add(moveDownCommandBinding);

                CommandBinding cutItemCommandBinding = new CommandBinding(GroupBar.CutItemCommand, GroupBar.CutItemCommandExecute, GroupBar.CutItemCommandCanExecute);
                ContextMenu.CommandBindings.Add(cutItemCommandBinding);

                CommandBinding copyItemCommandBinding = new CommandBinding(GroupBar.CopyItemCommand, GroupBar.CopyItemCommandExecute, GroupBar.CopyItemCommandCanExecute);
                ContextMenu.CommandBindings.Add(copyItemCommandBinding);

                CommandBinding pasteItemCommandBinding = new CommandBinding(GroupBar.PasteItemCommand, GroupBar.PasteItemCommandExecute, GroupBar.PasteItemCommandCanExecute);
                ContextMenu.CommandBindings.Add(pasteItemCommandBinding);
            }
        }

        /// <summary>
        /// Invoked when context menu loses mouse capture.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        private void ContextMenu_LostMouseCapture(object sender, MouseEventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            if (menu != null && menu.PlacementTarget != null)
            {
                GroupBarItem item = menu.PlacementTarget as GroupBarItem;

                if (item != null)
                {
                    item.IsContextMenuOpened = false;
                }
            }

            //m_bCanPasteExecute = true;
        }

        /// <summary>
        /// Invoked when context menu is opened.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;

            if (menu != null && menu.PlacementTarget != null)
            {
                if (menu.PlacementTarget is GroupBarItem)
                {
                    GroupBarItem item = menu.PlacementTarget as GroupBarItem;
                    item.IsContextMenuOpened = true;
                }
            }
        }

        /// <summary>
        /// Initializes control in stack mode in its template.
        /// </summary>
        /// <param name="inTemplate">Template to initialize control in.</param>
        private void InitializeStackMode(FrameworkTemplate inTemplate)
        {
            InitMainHost(inTemplate);
            InitVisualContent(inTemplate);
            InitToolbar(inTemplate);
            InitSplitter(inTemplate);
            InitOtherContent(inTemplate);
            object obj = this.Items;
            SetFocusedItem(SelectedTab);
        }

        /// <summary>
        /// Initializes MainHost of the control in stack mode in its template.
        /// </summary>
        /// <param name="inTemplate">Template to initialize control in.</param>
        private void InitMainHost(FrameworkTemplate inTemplate)
        {
            m_mainHost = inTemplate.FindName(C_nameMainHost, this) as Grid;

            if (m_mainHost == null)
            {
                throw new ApplicationException(C_errorMainHost);
            }
        }

        /// <summary>
        /// Initializes VisualContent of the control in stack mode in its template.
        /// </summary>
        /// <param name="inTemplate">Template to initialize control in.</param>
        private void InitVisualContent(FrameworkTemplate inTemplate)
        {
            m_visualContent = inTemplate.FindName(C_nameContentHost, this) as ContentPresenter;

            if (m_visualContent == null)
            {
                throw new ApplicationException(C_errorContentHost);
            }
            else
            {
                if (!IsCollapsed)
                {
                    m_visualContent.Content = SelectedContent;
                }
            }
        }

        /// <summary>
        /// Initializes Toolbar of the control in stack mode in its template.
        /// </summary>
        /// <param name="inTemplate">Template to initialize control in.</param>
        private void InitToolbar(FrameworkTemplate inTemplate)
        {
            m_toolbar = inTemplate.FindName(C_nameNavigationToolbar, this) as NavigationToolbar;

            if (m_toolbar == null)
            {
                throw new ApplicationException(C_errorNavigationToolbar);
            }

            if (m_toolbar.Visibility != StackItemHostVisibility)
            {
                m_toolbar.Visibility = StackItemHostVisibility;
            }

            m_toolbar.ApplyTemplate();
        }

        /// <summary>
        /// Initializes Splitter of the control in stack mode in its template.
        /// </summary>
        /// <param name="inTemplate">Template to initialize control in.</param>
        private void InitSplitter(FrameworkTemplate inTemplate)
        {
            if (m_splitter != null)
            {
                m_splitter.DragIncremented -= new RoutedEventHandler(OnSplitterDragIncremented);
            }

            m_splitter = inTemplate.FindName(C_nameSplitter, this) as GroupBarSplitter;

            if (m_splitter == null)
            {
                throw new ApplicationException(C_errorSplitter);
            }

            m_splitter.ApplyTemplate();
            m_splitter.DragIncremented += new RoutedEventHandler(OnSplitterDragIncremented);
        }

        /// <summary>
        /// Initializes ContentButton and Popup of the control in stack mode in its template.
        /// </summary>
        /// <param name="inTemplate">Template to initialize control in.</param>
        private void InitOtherContent(FrameworkTemplate inTemplate)
        {
            if (m_contentPopup != null)
            {
                m_contentPopup.Opened -= new EventHandler(PopupOpened);
                m_contentPopup.Closed -= new EventHandler(PopupClosed);
                m_contentPopup.IsKeyboardFocusWithinChanged -= new DependencyPropertyChangedEventHandler(PopupIsKeyboardFocusWithinChanged);
            }

            m_contentButton = inTemplate.FindName(C_nameContentButton, this) as ToggleButton;
            m_contentPopup = inTemplate.FindName(C_namePopup, this) as Popup;

            if (m_contentPopup != null)
            {
                m_contentPopup.Opened += new EventHandler(PopupOpened);
                m_contentPopup.Closed += new EventHandler(PopupClosed);
                m_contentPopup.IsKeyboardFocusWithinChanged += new DependencyPropertyChangedEventHandler(PopupIsKeyboardFocusWithinChanged);
                m_contentPopup.PlacementTarget = m_contentButton;

                if (Orientation == Orientation.Vertical)
                    m_contentPopup.LayoutTransform = new RotateTransform(0);
                else
                    m_contentPopup.LayoutTransform = new RotateTransform(90);
                }

            if (m_popupContentHost != null)
                m_popupContentHost.KeyDown -= new KeyEventHandler(PopupContentHostKeyDown);

            m_popupContentHost = inTemplate.FindName("PopupContentHost", this) as ContentPresenter;
            if (m_popupContentHost != null)
            m_popupContentHost.KeyDown += new KeyEventHandler(PopupContentHostKeyDown);

            if (m_popupResizeBorder != null)
            {
                m_popupResizeBorder.MouseDown -= new MouseButtonEventHandler(PopupResizeBorderMouseDown);
                m_popupResizeBorder.MouseMove -= new MouseEventHandler(PopupResizeBorderMouseMove);
                m_popupResizeBorder.MouseUp -= new MouseButtonEventHandler(PopupResizeBorderMouseUp);
            }
            m_popupResizeBorder = inTemplate.FindName(C_nameResizeBorder, this) as Border;
            if (m_popupResizeBorder != null)
            {
            m_popupResizeBorder.MouseDown += new MouseButtonEventHandler(PopupResizeBorderMouseDown);
            m_popupResizeBorder.MouseMove += new MouseEventHandler(PopupResizeBorderMouseMove);
            m_popupResizeBorder.MouseUp += new MouseButtonEventHandler(PopupResizeBorderMouseUp);
        }
        }

        /// <summary>
        /// Invoked when to the IsKeyboardFocusWithin event is received.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs that contains the event data.</param>
        private void PopupIsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            m_popupContentHost.Focus();
        }

        /// <summary>
        /// Invoked when to the <see cref="E:System.Windows.Input.Keyboard.KeyDown"/> event is received.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> that contains the event data.</param>
        private void PopupContentHostKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                m_contentPopup.IsOpen = false;
            }
        }

        /// <summary>
        /// Initializes the control in non-stack mode in its template.
        /// </summary>
        /// <param name="inTemplate">Template to initialize control in.</param>
        private void InitializeNonStackMode(FrameworkTemplate inTemplate)
        {
            InitMainHost(inTemplate);

            if (IsLoaded)
            {
                if (m_toolbar != null)
                {
                    foreach (NavigationToolbarItem navItem in m_toolbar.Items)
                    {
                        GroupBarItem item = navItem.GroupBarItem;
                        item.Visibility = Visibility.Visible;
                    }

                    m_toolbar.Items.Clear();
                }

                foreach (int i in m_hiddenIndices)
                {
                    GroupBarItem item = null;
                    if (ItemsSource == null)
                        item = Items[i] as GroupBarItem;
                    else
                        item = ItemContainerGenerator.ContainerFromIndex(i) as GroupBarItem;
                    if(item!=null)
                        item.ShowInGroupBar = true;
                }
                if (this.ItemsSource == null)
                {
                    foreach (GroupBarItem gbItem in Items)
                    {
                        gbItem.ShowInGroupBar = true;
                        gbItem.IsHidden = false;
                    }
                }
                else
                {
                    for (int i = 0; i < Items.Count; i++)
                    {
                        GroupBarItem item = ItemContainerGenerator.ContainerFromIndex(i) as GroupBarItem;
                        if (item != null)
                        {
                            item.ShowInGroupBar = true;
                            item.IsHidden = false;
                        }
                    }
                }
            }

            RecalculateItemContentLength();

            if (IsLoaded)
            {
                m_hiddenIndices.Clear();
            }
        }

        /// <summary>
        /// Initializes the control in multiple expansion mode in its template.
        /// </summary>
        /// <param name="inTemplate">Template to initialize control in.</param>
        private void InitializeMultipleExpansionMode(FrameworkTemplate inTemplate)
        {
            InitMainHost(inTemplate);

            m_scrollViewerHost = inTemplate.FindName(C_nameScrollViewer, this) as ScrollViewer;

            if (m_scrollViewerHost == null)
            {
                throw new ApplicationException(C_errorScrollViewer);
            }

            ItemContentLength += StackModeHeightCorrection;

            if (IsLoaded)
            {
                if (m_toolbar != null)
                {
                    foreach (NavigationToolbarItem navItem in m_toolbar.Items)
                    {
                        GroupBarItem item = navItem.GroupBarItem;
                        item.Visibility = Visibility.Visible;
                        ItemContentLength -= ItemHeaderHeight;
                    }

                    m_toolbar.Items.Clear();
                }

                for (int i = 0; i < m_hiddenIndices.Count; i++)
                {
                    GroupBarItem item = null;
                    if (i < Items.Count && Items[i] is GroupBarItem)
                    {
                        item = Items[i] as GroupBarItem;
                    }
                    else
                    {
                        item = ItemContainerGenerator.ContainerFromIndex(i) as GroupBarItem;
                    }
                    if (item != null)
                    {
                        item.ShowInGroupBar = true;
                        ItemContentLength -= ItemHeaderHeight;
                    }
                }

                m_hiddenIndices.Clear();
            }
        }

        /// <summary>
        /// Initializes the rotation angles of the control and the selected
        /// content.
        /// </summary>
        private void InitializeRotationAngle()
        {
            if (RotationAngle == 0d)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    ContentRotationAngle = 90d;
                    //ContentRotationAngle = ContentRotationAngle + 90d;
                    RotationAngle = -90d;
                }
                else
                {
                    ContentRotationAngle = 0d;
                    RotationAngle = 0d;
                }
            }
        }

        /// <summary>
        /// Gets the free space to allocate for the <see cref="ItemContentLength"/> property and
        /// corrects the property if there is not enough space to show
        /// all headers.
        /// </summary>
        private void InitializeFreeSpace()
        {
            double freeSpace = (Orientation == Orientation.Horizontal) ?
                GetFreeSpace(Width) : GetFreeSpace(Height);

            if (double.IsNaN(freeSpace))
            {
                freeSpace = (Orientation == Orientation.Horizontal) ? GetFreeSpace(ActualWidth) : GetFreeSpace(ActualHeight);
            }

            ItemContentLength += freeSpace;
        }

        /// <summary>
        /// Updates the size for all items.
        /// </summary>
        internal void UpdateItemsSize()
        {
            for(int i=0;i<Items.Count;i++)
            {
                GroupBarItem item = ItemContainerGenerator.ContainerFromIndex(i) as GroupBarItem;
                if (item != null)
                {
                    if (item.IsLoaded && item.Visibility == Visibility.Visible)
                    {
                        item.UpdateContentSize(item.IsExpanded);
                        item.UpdateContentAnimation();
                        item.UpdateLayout();

                        GroupView view = item.Content as GroupView;

                        if (view != null)
                        {
                            view.UpdateInternalContentLength();

                            view.UpdatePropertyCoerce();
                            view.InvalidateMeasureItems();
                        }
                    }
                }
            }            
        }

        /// <summary>
        /// Collapses all items.
        /// </summary>
        private void CollapseAll()
        {
            for(int i = 0; i< Items.Count;i++)
            {
                GroupBarItem item = ItemContainerGenerator.ContainerFromIndex(i) as GroupBarItem;
                if (item != null)
                {
                    item.IsExpanded = false;
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="SplitterDownCommand"/>.
        /// </summary>
        /// <param name="target">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private static void HandleSplitterDownCommand(object target, ExecutedRoutedEventArgs e)
        {
            GroupBar owner = target as GroupBar;

            if (owner != null && owner.StackItemsCount > 0)
            {
                owner.DoSplitting(DragDirection.Down);
            }
        }

        /// <summary>
        /// Handles the <see cref="SplitterUpCommand"/>.
        /// </summary>
        /// <param name="target">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private static void HandleSplitterUpCommand(object target, ExecutedRoutedEventArgs e)
        {
            GroupBar owner = target as GroupBar;

            if (owner != null && owner.StackItemsCount < owner.Items.Count)
            {
                owner.DoSplitting(DragDirection.Up);
            }
        }

        /// <summary>
        /// Handles <see cref="OpenOptionsCommand"/>.
        /// </summary>
        /// <remarks>
        /// Not implemented yet.
        /// </remarks>
        /// <param name="target">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private static void HandleOpenOptionsCommand(object target, ExecutedRoutedEventArgs e)
        {
        }

        /// <summary>
        /// Adds the rename item can execute.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        /// <returns>bool bCanExecute</returns>
        private static bool AddRenameItemCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            bool bCanExecute = false;
            ContextMenu menu = sender as ContextMenu;

            if (menu != null && menu.PlacementTarget != null)
            {
                if (menu.PlacementTarget is GroupView)
                {
                    GroupView view = (GroupView)menu.PlacementTarget;

                    if (view.ItemsSource == null)
                    {
                        bCanExecute = true;
                    }
                }

                if (menu.PlacementTarget is GroupBarItem)
                {
                    GroupBarItem barItem = (GroupBarItem)menu.PlacementTarget;

                    if (barItem.Content is GroupView)
                    {
                        GroupView view = (GroupView)barItem.Content;

                        if (view.ItemsSource == null)
                        {
                            bCanExecute = true;
                        }
                    }
                }
            }

            return bCanExecute;
        }

        /// <summary>
        /// The <see cref="AddItemCommand"/> command handler.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private static void AddItemCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            GroupBarItem tab = null;
            GroupViewItem item = null;
            GroupBarContextMenuItemEventArgs args = GroupBar.GetEventArgs(e);
            GroupBar.OnContextMenuItemClick(sender, args);

            if (!args.Handled)
            {
                if (menu != null && menu.PlacementTarget != null)
                {
                    if (menu.PlacementTarget is GroupView)
                    {
                        GroupView groupView = (GroupView)menu.PlacementTarget;
                        tab = groupView.LogicalParent;
                        item = groupView.AddItem();
                        item.SetBinding(GroupViewItem.ImageSourceProperty, Binder.Bind(tab.LogicalParent.DefaultImage, "Source"));
                        item.IsInEditMode = true;
                        tab.UpdateContentSize(true);
                        tab.UpdateContentAnimation();
                    }

                    if (menu.PlacementTarget is GroupBarItem)
                    {
                        tab = (GroupBarItem)menu.PlacementTarget;
                        GroupBar grouBar = tab.LogicalParent;

                        if (tab.Content is GroupView)
                        {
                            GroupView groupView = (GroupView)tab.Content;

                            item = groupView.AddItem();
                            item.IsInEditMode = true;
                            grouBar.UpdateItemsSize();
                            grouBar.UpdateLayout();

                            if (tab.IsExpanded == false)
                            {
                                if (grouBar.VisualMode != VisualMode.MultipleExpansion)
                                {
                                    if (grouBar.SelectedTab != null)
                                    {
                                        grouBar.SelectedTab.IsExpanded = false;
                                    }
                                    else if (grouBar.SelectedItem.LogicalParent != null)
                                    {
                                        grouBar.SelectedItem.ParentTab.IsExpanded = false;
                                    }

                                    grouBar.SelectedObject = tab;
                                }

                                tab.IsExpanded = true;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Indicates whether the <see cref="AddItemCommand"/> can be executed in its current state.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private static void AddItemCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AddRenameItemCanExecute(sender, e);
        }

        /// <summary>
        /// The <see cref="RenameItemCommand"/> command handler.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private static void RenameItemCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            GroupView groupView = menu.PlacementTarget as GroupView;
            GroupBarContextMenuItemEventArgs args = GroupBar.GetEventArgs(e);
            GroupBar.OnContextMenuItemClick(sender, args);

            if (!args.Handled)
            {
                if (menu != null && groupView != null)
                {
                    GroupBarItem barItem = groupView.LogicalParent;
                    GroupViewItem item = GetViewItemFromGroupBar(barItem);

                    if (item != null)
                    {
                        item.IsInEditMode = true;
                        barItem.UpdateContentSize(true);
                        barItem.UpdateContentAnimation();
                    }
                }
            }
        }

        /// <summary>
        /// Indicates whether the <see cref="RenameItemCommand"/> can be executed in its current state.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private static void RenameItemCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            bool bCanExecute = AddRenameItemCanExecute(sender, e);
            ContextMenu menu = sender as ContextMenu;

            if (menu.PlacementTarget is GroupBarItem)
            {
                bCanExecute = false;
            }

            e.CanExecute = bCanExecute;
        }

        /// <summary>
        /// The <see cref="DeleteItemCommand"/> command handler.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private static void DeleteItemCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            UIElement placementTarget = menu.PlacementTarget;

            GroupBarContextMenuItemEventArgs args = GroupBar.GetEventArgs(e);
            GroupBar.OnContextMenuItemClick(sender, args);

            if (!args.Handled)
            {
                if (menu != null && placementTarget != null)
                {
                    if (placementTarget is GroupView)
                    {
                        GroupView groupView = (GroupView)placementTarget;
                        GroupBarItem barItem = groupView.LogicalParent;
                        GroupViewItem item = GetViewItemFromGroupBar(barItem);

                        if (item != null)
                        {
                            int index = groupView.Items.IndexOf(item);
                            groupView.Items.Remove(item);

                            if (index >= 1)
                            {
                                index = (groupView.Items.Count <= index) ? index - 1 : index;
                                barItem.LogicalParent.SelectedObject = groupView.Items[index];
                            }
                            else
                            {
                                barItem.LogicalParent.SelectedObject = barItem;
                            }

                            barItem.UpdateContentSize(true);
                            barItem.UpdateContentAnimation();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Indicates whether the <see cref="DeleteItemCommand"/> can be executed in its current state.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private static void DeleteItemCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = AddRenameItemCanExecute(sender, e);
        }

        /// <summary>
        /// The <see cref="DeleteTabCommand"/> command handler.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private static void DeleteTabCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            UIElement placementTarget = menu.PlacementTarget;

            GroupBarContextMenuItemEventArgs args = GroupBar.GetEventArgs(e);
            GroupBar.OnContextMenuItemClick(sender, args);

            if (!args.Handled)
            {
                if (menu != null && placementTarget != null)
                {
                    if (placementTarget is GroupBarItem)
                    {
                        GroupBarItem barItem = (GroupBarItem)placementTarget;
                        barItem.Visibility = Visibility.Collapsed;

                        if (barItem != null && barItem.LogicalParent != null)
                        {
                            GroupBar bar = barItem.LogicalParent;

                            if (bar.ItemsSource == null)
                            {
                                bar.Items.Remove(barItem);
                                bar.deleted++;
                                bar.RecalculateItemContentLength();
                                if (bar.Items.Count > 0)
                                {
                                    bar.SelectedObject = bar.Items[0];
                                }
                                else
                                {
                                    bar.SelectedHeader = String.Empty;
                                    bar.SelectedContent = null;
                                }
                            }
                            else
                            {
                                ((IList)bar.ItemsSource).Remove(barItem.Content);
                                bar.RecalculateItemContentLength();

                                if (bar.Items.Count <= 0)
                                {
                                    bar.SelectedHeader = String.Empty;
                                    bar.SelectedContent = null;
                                } 
                                bar.Items.Refresh();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Indicates whether the <see cref="DeleteTabCommand"/> can be executed in its current state.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private static void DeleteTabCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            bool bCanExecute = false;
            ContextMenu menu = sender as ContextMenu;

            if (menu.PlacementTarget is GroupBarItem)
            {
                bCanExecute = true;
            }

            e.CanExecute = bCanExecute;
        }

        /// <summary>
        /// The <see cref="RenameTabCommand"/> command handler.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private static void RenameTabCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            GroupBarContextMenuItemEventArgs args = GroupBar.GetEventArgs(e);
            GroupBar.OnContextMenuItemClick(sender, args);

            if (!args.Handled)
            {
                if (menu != null && menu.PlacementTarget != null)
                {
                    if (menu.PlacementTarget is GroupBarItem)
                    {
                        GroupBarItem barItem = menu.PlacementTarget as GroupBarItem;
                        barItem.IsInEditMode = true;
                        barItem.UpdateContentSize(true);
                        barItem.UpdateContentAnimation();
                    }
                }
            }
        }

        /// <summary>
        /// Indicates whether the <see cref="RenameTabCommand"/> can be executed in its current state.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private static void RenameTabCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            bool bCanExecute = false;
            ContextMenu menu = sender as ContextMenu;
            UIElement placementTarget = menu.PlacementTarget;

            if (menu != null && placementTarget != null)
            {
                if (placementTarget is GroupView)
                {
                    GroupView view = (GroupView)placementTarget;

                    if (view.ItemsSource == null)
                    {
                        bCanExecute = false;
                    }
                }

                if (placementTarget is GroupBarItem)
                {
                    bCanExecute = true;
                }
            }

            e.CanExecute = bCanExecute;
        }

        /// <summary>
        /// The <see cref="AddTabCommand"/> command handler.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private static void AddTabCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            bool bCreated = false;
            GroupBar bar = null;
            GroupBarItem item = null;
            GroupBarContextMenuItemEventArgs args = GroupBar.GetEventArgs(e);

            if (menu != null && menu.PlacementTarget != null)
            {
                if (menu.PlacementTarget is GroupView)
                {
                    GroupView groupView = (GroupView)menu.PlacementTarget;

                    if (groupView.LogicalParent != null
                        && groupView.LogicalParent.LogicalParent != null)
                    {
                        bar = groupView.LogicalParent.LogicalParent;

                        GroupBar.OnContextMenuItemClick(menu, args);

                        if (!args.Handled)
                        {
                            item = bar.AddTab();
                            bCreated = true;
                        }
                    }
                }

                if (menu.PlacementTarget is GroupBarItem)
                {
                    GroupBarItem barItem = (GroupBarItem)menu.PlacementTarget;

                    if (barItem.LogicalParent != null)
                    {
                        bar = barItem.LogicalParent;

                        GroupBar.OnContextMenuItemClick(menu, args);

                        if (!args.Handled)
                        {
                            item = bar.AddTab();
                            bCreated = true;
                        }
                    }
                }

                if (menu.PlacementTarget is GroupBar)
                {
                    bar = (GroupBar)menu.PlacementTarget;

                    GroupBar.OnContextMenuItemClick(menu, args);

                    if (!args.Handled)
                    {
                        item = bar.AddTab();
                        bCreated = true;
                    }
                }
            }

            if (bCreated)
            {
                GroupView view = new GroupView();
                item.Content = view;
                view.ApplyTemplate();
                bar.UpdateItemsSize();
                item.IsInEditMode = true;
                if (bar.IsSplitted)
                {
                    bar.StackItemsHost.Height = bar.StackItemsHost.ActualHeight + bar.ItemHeaderHeight;
                    //IsSplitted = false;
                }
            }
        }

        /// <summary>
        /// Used to get the <see cref="GroupBarContextMenuItemEventArgs"/> args.
        /// </summary>
        /// <param name="e">ExecutedRoutedEventargs e</param>
        /// <returns>The <see cref="GroupBarContextMenuItemEventArgs"/> that contains the event data.</returns>
        private static GroupBarContextMenuItemEventArgs GetEventArgs(ExecutedRoutedEventArgs e)
        {
            GroupBarContextMenuItemEventArgs args = new GroupBarContextMenuItemEventArgs();
            args.RoutedEvent = e.RoutedEvent;
            args.MenuItem = ((MenuItem)e.OriginalSource).Header;
            args.Source = e.Source;
            
            return args;
        }

        /// <summary>
        /// Indicates whether the <see cref="AddTabCommand"/> can be executed in its current state.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private static void AddTabCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            bool bCanExecute = false;
            ContextMenu menu = sender as ContextMenu;

            if (menu != null && menu.PlacementTarget != null)
            {
                if (menu.PlacementTarget is GroupView)
                {
                    GroupView groupView = (GroupView)menu.PlacementTarget;

                    if (groupView.LogicalParent != null
                        && groupView.LogicalParent.LogicalParent != null)
                    {
                        bCanExecute = true;
                    }
                }

                if (menu.PlacementTarget is GroupBarItem)
                {
                    GroupBarItem barItem = (GroupBarItem)menu.PlacementTarget;

                    if (barItem.LogicalParent != null)
                    {
                        bCanExecute = true;
                    }
                }

                if (menu.PlacementTarget is GroupBar)
                {
                    bCanExecute = true;
                }
            }

            e.CanExecute = bCanExecute;
        }

        /// <summary>
        /// The <see cref="ChangeListViewModeCommand"/> command handler.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private static void ChangeListViewModeCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            GroupBarContextMenuItemEventArgs args = GroupBar.GetEventArgs(e);
            GroupBar.OnContextMenuItemClick(sender, args);
            if (!args.Handled)
            {
            }
        }

        /// <summary>
        /// Indicates whether the <see cref="ChangeListViewModeCommand"/> can be executed in its current state.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private static void ChangeListViewModeCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            bool bCanExecute = false;
            ContextMenu menu = sender as ContextMenu;

            if (menu != null && menu.PlacementTarget != null)
            {
                if (menu.PlacementTarget is GroupView)
                {
                    GroupView groupView = (GroupView)menu.PlacementTarget;

                    if (groupView.LogicalParent != null)
                    {
                        bCanExecute = true;
                    }
                }

                if (menu.PlacementTarget is GroupBarItem)
                {
                    GroupBarItem item = (GroupBarItem)menu.PlacementTarget;

                    if (item.Content != null && item.Content is GroupView)
                    {
                        bCanExecute = true;
                    }
                }
            }

            e.CanExecute = bCanExecute;
        }

        /// <summary>
        /// Adds sort description to the <see cref="GroupBar"/>.
        /// </summary>
        /// <param name="groupBar">The <see cref="GroupBar"/> instance.</param>
        /// <param name="sortDirection">Alphabetic or descending sort order.</param>
        private static void AddGroupBarSortDescription(GroupBar groupBar, ListSortDirection sortDirection)
        {
            groupBar.Items.SortDescriptions.Clear();
            groupBar.Items.SortDescriptions.Add(new SortDescription(C_sortGroupBarProperty, sortDirection));
        }

        /// <summary>
        /// Adds the group view sort description.
        /// </summary>
        /// <param name="groupView">The group view.</param>
        /// <param name="sortDirection">The sort direction.</param>
        private static void AddGroupViewSortDescription(GroupView groupView, ListSortDirection sortDirection)
        {
            groupView.Items.SortDescriptions.Clear();
            groupView.Items.SortDescriptions.Add(new SortDescription(C_sortGroupViewProperty, sortDirection));
        }

        /// <summary>
        /// Adds alphabetic or descending sort order to the <see cref="GroupViewItem"/> or <see cref="GroupBarItem"/> elements.
        /// </summary>
        /// <param name="sender">Context menu object.</param>
        /// <param name="sortDirection">Alphabetic or descending sort order.</param>
        private static void AddSortToGroupItem(object sender, ListSortDirection sortDirection)
        {
            ContextMenu menu = sender as ContextMenu;
            UIElement placementTarget = menu.PlacementTarget;
            IsInSorting = true;
            if (menu != null && placementTarget != null)
            {
                if (placementTarget is GroupView)
                {
                    GroupView groupView = (GroupView)placementTarget;
                    AddGroupViewSortDescription(groupView, sortDirection);
                }
                else if (placementTarget is GroupBar)
                {
                    GroupBar groupBar = (GroupBar)placementTarget;
                    AddGroupBarSortDescription(groupBar, sortDirection);
                }
                else if (placementTarget is GroupBarItem)
                {
                    GroupBarItem item = (GroupBarItem)placementTarget;
                    GroupBar groupBar = item.LogicalParent;
                    AddGroupBarSortDescription(groupBar, sortDirection);
                }
            }
            IsInSorting = false;
        }

        /// <summary>
        /// Sorts the <see cref="GroupBarItem"/> or <see cref="GroupViewItem"/> elements in alphabetic order.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private static void SortAscCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            GroupBarContextMenuItemEventArgs args = GroupBar.GetEventArgs(e);
            GroupBar.OnContextMenuItemClick(sender, args);

            if (!args.Handled)
            {
                AddSortToGroupItem(sender, ListSortDirection.Ascending);
            }
        }

        /// <summary>
        /// Indicates whether the <see cref="GroupBarItem"/> or <see cref="GroupViewItem"/> elements can be sorted in alphabetic order.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private static void SortAscCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SortAscDesCanExecute(sender, e);
        }

        /// <summary>
        /// Sorts the <see cref="GroupBarItem"/> or <see cref="GroupViewItem"/> elements in descending order.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private static void SortDesCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            GroupBarContextMenuItemEventArgs args = GroupBar.GetEventArgs(e);
            GroupBar.OnContextMenuItemClick(sender, args);

            if (!args.Handled)
            {
                AddSortToGroupItem(sender, ListSortDirection.Descending);
            }
        }

        /// <summary>
        /// Indicates whether the <see cref="GroupBarItem"/> or <see cref="GroupViewItem"/> elements can be sorted in descending order.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private static void SortDesCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SortAscDesCanExecute(sender, e);
        }

        /// <summary>
        /// Indicates whether the <see cref="GroupBarItem"/> or <see cref="GroupViewItem"/> elements can be sorted in descending or alphabetic order.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        /// <returns>Value indicating whether GroupBarItem or GroupViewItem elements can be sorted in descending or alphabetic order.</returns>
        private static bool SortAscDesCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            bool bCanExecute = false;
            ContextMenu menu = (ContextMenu)sender;
            UIElement placementTarget = menu.PlacementTarget;

            if (menu != null && placementTarget != null
                && (placementTarget is GroupView
                || placementTarget is GroupBar
                || placementTarget is GroupBarItem))
            {
                bCanExecute = true;
            }

            return bCanExecute;
        }

        /// <summary>
        /// Moves the <see cref="GroupBarItem"/> or <see cref="GroupViewItem"/> up.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private static void MoveUpCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            GroupBarContextMenuItemEventArgs args = GroupBar.GetEventArgs(e);
            GroupBar.OnContextMenuItemClick(sender, args);

            if (!args.Handled)
            {
                MoveUpDownExecute(sender, true);
            }
        }

        /// <summary>
        /// Indicates whether the <see cref="GroupBarItem"/> or <see cref="GroupViewItem"/> can be moved up.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private static void MoveUpCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MoveUpDownCanExecute(sender, true);
        }

        /// <summary>
        /// Moves the <see cref="GroupBarItem"/> or <see cref="GroupViewItem"/> down.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private static void MoveDownCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            GroupBarContextMenuItemEventArgs args = GroupBar.GetEventArgs(e);
            GroupBar.OnContextMenuItemClick(sender, args);

            if (!args.Handled)
            {
                MoveUpDownExecute(sender, false);
            }
        }

        /// <summary>
        /// Indicates whether the <see cref="GroupBarItem"/> or <see cref="GroupViewItem"/> can be moved down.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private static void MoveDownCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MoveUpDownCanExecute(sender, false);
        }

        /// <summary>
        /// Indicates whether the <see cref="GroupBarItem"/> or <see cref="GroupViewItem"/> can be moved up or down.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="isUpMoving">Indicates whether it is up or down moving.</param>
        /// <returns>Value indicating whether GroupBarItem or GroupViewItem can be moved up or down.</returns>
        private static bool MoveUpDownCanExecute(object sender, bool isUpMoving)
        {
            bool bCanExecute = false;
            ContextMenu menu = sender as ContextMenu;
            UIElement placementTarget = menu.PlacementTarget;
            int counter = 0;

            if (menu != null && placementTarget != null)
            {
                if (placementTarget is GroupBarItem)
                {
                    GroupBarItem gBarItem = (GroupBarItem)placementTarget;
                    GroupBar gBar = gBarItem.LogicalParent;

                    if (gBar != null)
                    {
                        if (isUpMoving == false)
                        {
                            counter = gBar.Items.Count - 1;
                        }

                        if (gBarItem != gBar.ItemContainerGenerator.ContainerFromIndex(counter))
                        {
                            bCanExecute = true;
                        }
                    }
                }
                else if (placementTarget is GroupView)
                {
                    GroupView groupView = (GroupView)menu.PlacementTarget;

                    if (groupView.ItemsSource == null)
                    {
                        GroupBarItem barItem = groupView.LogicalParent;
                        GroupViewItem item = GetViewItemFromGroupBar(barItem);

                        if (isUpMoving == false)
                        {
                            counter = groupView.Items.Count - 1;
                        }

                        if (item != null && counter >= 0 && counter < groupView.Items.Count
                            && groupView.Items[counter] != item)
                        {
                            bCanExecute = true;
                        }
                    }
                }
            }

            return bCanExecute;
        }

        /// <summary>
        /// Moves the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> or 
        /// the <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> up or down.
        /// </summary>
        /// <param name="sender">Context menu object.</param>
        /// <param name="isUpMoving">Indicates whether it is up or down moving.</param>
        private static void MoveUpDownExecute(object sender, bool isUpMoving)
        {
            ContextMenu menu = sender as ContextMenu;
            UIElement placementTarget = menu.PlacementTarget;

            if (menu != null && placementTarget != null)
            {
                if (placementTarget is GroupView)
                {
                    GroupView groupView = placementTarget as GroupView;
                    GroupBarItem barItem = groupView.LogicalParent;

                    MoveItem(groupView, GetViewItemFromGroupBar(barItem), isUpMoving);
                }
                else if (placementTarget is GroupBarItem)
                {
                    GroupBarItem barItem = placementTarget as GroupBarItem;
                    GroupBar groupBar = barItem.LogicalParent;

                    MoveItem(groupBar, barItem, isUpMoving);

                    if (groupBar.VisualMode != VisualMode.StackMode &&
                        barItem.IsExpanded && barItem.VisualContent.Height == 0)
                    {
                        barItem.UpdateContentLength();
                        barItem.VisualContent.Height = barItem.ContentLength;
                    }
                }
            }
        }

        /// <summary>
        /// Moves the item.
        /// </summary>
        /// <param name="groupView">The group view groupViewItems.</param>
        /// <param name="item">The item groupViewItems.</param>
        /// <param name="isMovingUp">if set to <c>true</c> [is moving up].</param>
        private static void MoveItem(GroupView groupView, GroupViewItem item, bool isMovingUp)
        {
            ItemCollection groupViewItems = groupView.Items;
            int index = groupViewItems.IndexOf(item);
            groupViewItems.RemoveAt(index);
            int insertIndex = index + 1;

            if (isMovingUp == true)
            {
                insertIndex = index - 1;
            }

            groupViewItems.Insert(insertIndex, item);
        }

        /// <summary>
        /// Moves the item.
        /// </summary>
        /// <param name="groupBar">The group bar groupViewItems.</param>
        /// <param name="item">The item groupViewItems.</param>
        /// <param name="isMovingUp">if set to <c>true</c> [is moving up].</param>
        private static void MoveItem(GroupBar groupBar, GroupBarItem item, bool isMovingUp)
        {
            int barIndex, insertIndex;

            if (null == groupBar.ItemsSource)
            {
                ItemCollection groupBarItems = groupBar.Items;
                barIndex = groupBarItems.IndexOf(item);
                groupBarItems.RemoveAt(barIndex);
                insertIndex = barIndex + 1;

                if (isMovingUp == true)
                {
                    insertIndex = barIndex - 1;
                }

                groupBarItems.Insert(insertIndex, item);
            }
            else
            {
                Object tempItem = item.DataContext;
                barIndex = ((IList)groupBar.ItemsSource).IndexOf(tempItem);
                ((IList)groupBar.ItemsSource).RemoveAt(barIndex);
                insertIndex = barIndex + 1;

                if (isMovingUp)
                {
                    insertIndex = barIndex - 1;
                }

                ((IList)groupBar.ItemsSource).Insert(insertIndex, tempItem);
                groupBar.Items.Refresh();
            }
        }

        /// <summary>
        /// Gets the selected <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> from the <see cref="GroupBar"/>.
        /// </summary>
        /// <param name="barItem">The <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> object.</param>
        /// <returns>The selected <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/>.</returns>
        private static GroupViewItem GetViewItemFromGroupBar(GroupBarItem barItem)
        {
            GroupBar groupBar = barItem.LogicalParent;
            GroupViewItem item = groupBar.SelectedItem;
            return item;
        }

        /// <summary>
        /// Cuts the <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private static void CutItemCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            GroupBarContextMenuItemEventArgs args = GroupBar.GetEventArgs(e);
            GroupBar.OnContextMenuItemClick(sender, args);

            if (!args.Handled)
            {
                CutCopyItemExecute(sender, true);
            }
        }

        /// <summary>
        /// Indicates whether the <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> can be cut.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private static void CutItemCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            bool bCanExecute = CutCopyPasteCanExecute(sender);

            ContextMenu menu = sender as ContextMenu;
            if (menu != null)
            {
                if (menu.PlacementTarget is GroupView && (menu.PlacementTarget as GroupView).Items.Count == 0)
                {
                    bCanExecute = false;
                }
            }

            e.CanExecute = bCanExecute;
        }

        /// <summary>
        /// Copies the <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> that contains the event data.</param>
        private static void CopyItemCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            GroupBarContextMenuItemEventArgs args = GroupBar.GetEventArgs(e);
            GroupBar.OnContextMenuItemClick(sender, args);

            if (!args.Handled)
            {
                CutCopyItemExecute(sender, false);
            }
        }

        /// <summary>
        /// Indicates whether the <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> can be copied
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private static void CopyItemCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            bool bCanExecute = CutCopyPasteCanExecute(sender);

            ContextMenu menu = sender as ContextMenu;
            if (menu != null)
            {
                if (menu.PlacementTarget is GroupView && (menu.PlacementTarget as GroupView).Items.Count == 0)
                {
                    bCanExecute = false;
                }
            }

            e.CanExecute = bCanExecute;
        }

        /// <summary>
        /// Pastes the <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The instance containing the event data.</param>
        private static void PasteItemCommandExecute(object sender, ExecutedRoutedEventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            UIElement placementTarget = menu.PlacementTarget;

            GroupBarContextMenuItemEventArgs args = GroupBar.GetEventArgs(e);
            GroupBar.OnContextMenuItemClick(sender, args);

            if (!args.Handled)
            {
                if (menu != null && placementTarget != null)
                {
                    if (placementTarget is GroupView)
                    {
                        GroupView groupView = placementTarget as GroupView;
                        IList list = (groupView.ItemsSource == null) ?
                            groupView.Items as IList : groupView.ItemsSource as IList;

                        if (list != null)
                        {
                            object loadedItem = null;
                            try
                            {
                                DataObject dataObject = Clipboard.GetDataObject() as DataObject;
                                loadedItem = GetItemFromClipboard(dataObject);

                            }
                            //SU I78477
                            //catch (Exception ex)
                            catch (Exception)
                                //EU I78477
                            {
                            }


                            GroupViewItem gViewSelectedItem = GetViewItemFromGroupBar(groupView.LogicalParent);
                            if (loadedItem != null && loadedItem is GroupViewItem)
                            {
                                if (gViewSelectedItem != null && gViewSelectedItem.LogicalParent != null)
                                {
                                    object selectedObject = (gViewSelectedItem.LogicalParent.ItemsSource == null) ?
                                    gViewSelectedItem : gViewSelectedItem.LogicalParent.ItemContainerGenerator.ItemFromContainer(gViewSelectedItem);

                                    int index = list.IndexOf(selectedObject);
                                    list.Insert(index + 1, loadedItem);
                                }
                                else
                                {
                                    list.Add(loadedItem);
                                }

                                GroupBar groupBar = groupView.LogicalParent.LogicalParent;
                                groupBar.SelectedObject = null;

                                int itemIndex = list.IndexOf(loadedItem);
                                GroupViewItem gvSelectedItem = list[itemIndex] as GroupViewItem;
                                DependencyPropertyChangedEventArgs ev = new DependencyPropertyChangedEventArgs(SelectedObjectProperty, null, gvSelectedItem);
                                OnSelectedObjectChanged(groupView.LogicalParent.LogicalParent as DependencyObject, ev);
                            }
                        }
                    }
                    else if (placementTarget is GroupBarItem)
                    {
                        GroupBarItem barItem = placementTarget as GroupBarItem;
                        object loadedItem = null;
                        try
                        {
                            DataObject dataObject = Clipboard.GetDataObject() as DataObject;
                            loadedItem = GetItemFromClipboard(dataObject);
                        }
                        //SU I78477
                        //catch (Exception ex)
                        catch (Exception)
                            //EU I78477
                        {
                        }


                        if (barItem != null && loadedItem != null)
                        {
                            int index;
                            GroupBar groupB = barItem.LogicalParent;
                            if (loadedItem is GroupBarItem)
                            {
                                if (barItem.LogicalParent.ItemsSource == null)
                                {
                                    index = barItem.LogicalParent.Items.IndexOf(barItem);
                                    barItem.LogicalParent.Items.Insert(index + 1, loadedItem);
                                    barItem.LogicalParent.SelectedTab = loadedItem as GroupBarItem;
                                }
                                
                                GroupBarItem barItemToPaste = loadedItem as GroupBarItem;
                                if (barItemToPaste.IsExpanded && barItemToPaste.VisualContent != null && barItemToPaste.VisualContent.Height == 0)
                                {
                                    barItemToPaste.VisualContent.Height = barItemToPaste.LogicalParent.m_contentHeight;
                                }

                                if (groupB.IsSplitted)
                                    groupB.StackItemsHost.Height = groupB.StackItemsHost.ActualHeight + groupB.ItemHeaderHeight;
                            }
                            else
                            {
                                index = ((IList)barItem.LogicalParent.ItemsSource).IndexOf(barItem.DataContext);

                                ((IList)barItem.LogicalParent.ItemsSource).Insert(index + 1, loadedItem);
                                index = ((IList)barItem.LogicalParent.ItemsSource).IndexOf(loadedItem);
                                groupB.Items.Refresh();
                                loadedItem = groupB.ItemContainerGenerator.ContainerFromIndex(index) as GroupBarItem;

                                GroupBarItem barItemToPaste = loadedItem as GroupBarItem;
                                if (barItemToPaste.IsExpanded && barItemToPaste.VisualContent != null && barItemToPaste.VisualContent.Height == 0)
                                {
                                    barItemToPaste.VisualContent.Height = barItemToPaste.LogicalParent.m_contentHeight;
                                }

                                if (groupB.IsSplitted)
                                    groupB.StackItemsHost.Height = groupB.StackItemsHost.ActualHeight + groupB.ItemHeaderHeight;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Indicates whether the <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> can be pasted.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> that contains the event data.</param>
        private static void PasteItemCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            UIElement placementTarget = menu.PlacementTarget;
            DataObject dataObject = null;
            try
            {
                dataObject = Licensing.EnvironmentTest.IsSecurityGranted
                    ? Clipboard.GetDataObject() as DataObject : null;

                UIElement loadedItem = null;

                if (dataObject != null)
                {
                    loadedItem = GetItemFromClipboard(dataObject) as UIElement;
                }

                bool bCanExecute = CutCopyPasteCanExecute(sender);

                if ((EnvironmentTest.IsSecurityGranted && (Clipboard.GetData(C_formatGroupViewItem) as MemoryStream) == null)
                    || (placementTarget is GroupView && (loadedItem == null || !(loadedItem is GroupViewItem)))
                    || (!(placementTarget is GroupView) && loadedItem is GroupViewItem))
                {
                    bCanExecute = false;
                }
                e.CanExecute = bCanExecute;
            }
            //SU I78477
            //catch (Exception ex)
            catch (Exception)
                //EU I78477
            { }


        }
        /// <summary>
        /// Indicates whether the <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> can be cut, copied and pasted.
        /// </summary>
        /// <param name="sender">The context menu object.</param>
        /// <returns>Value indicating whether the <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> can be 
        /// cut, copied and pasted.</returns>
        private static bool CutCopyPasteCanExecute(object sender)
        {
            bool bCanExecute = false;
            ContextMenu menu = sender as ContextMenu;

            if (menu != null && (menu.PlacementTarget is GroupView || menu.PlacementTarget is GroupBarItem))
            {
                bCanExecute = true;
            }

            return bCanExecute;
        }

        /// <summary>
        /// Copies or cuts the <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="isCutCommand">Indicates whether it is cut command.</param>
        private static void CutCopyItemExecute(object sender, bool isCutCommand)
        {
            ContextMenu menu = sender as ContextMenu;
            UIElement placementTarget = menu.PlacementTarget;

            if (menu != null && placementTarget != null)
            {
                if (placementTarget is GroupView)
                {
                    GroupView view = placementTarget as GroupView;

                    if (view != null)
                    {
                        IList list = (view.ItemsSource == null) ?
                            view.Items as IList : view.ItemsSource as IList;
                        if (list != null)
                        {
                            GroupViewItem item = GetViewItemFromGroupBar(view.LogicalParent);

                            object copyObject = (view.ItemsSource == null) ?
                                item : view.ItemContainerGenerator.ItemFromContainer(item);

                            SetItemToClipboard(copyObject, item.Text, item.ImageSource);

                            if (isCutCommand)
                            {
                                GroupViewItem selectedItem = SetSelectedItemInGroup(view, item);

                                object removeObject = (view.ItemsSource == null) ?
                                    item : view.ItemContainerGenerator.ItemFromContainer(item);

                                list.Remove(removeObject);
                                SetGroupBarSelectedObject(view, selectedItem);
                            }
                        }
                    }
                }
                else
                {
                    if (placementTarget is GroupBarItem)
                    {
                        GroupBarItem barItem = placementTarget as GroupBarItem;
                        if (barItem != null)
                        {
                            DataObject dataObj = new DataObject();
                            SetItemToClipboard(barItem, dataObj);

                            if (isCutCommand)
                            {
                                GroupBar gBar = barItem.LogicalParent;
                                gBar.m_contentHeight = barItem.ContentLength;

                                if (gBar.ItemsSource == null)
                                {
                                    gBar.Items.Remove(barItem);
                                }
                                else
                                {
                                    SetItemToClipboard(barItem.DataContext, dataObj);
                                    ((IList)gBar.ItemsSource).Remove(barItem.DataContext);
                                    gBar.Items.Refresh();
                                }
                                //// if (gBar.m_visualContent.Content == gBar.SelectedContent)
                                if (gBar.SelectedTab == barItem)
                                {
                                    gBar.SelectedContent = null;
                                    gBar.SelectedHeader = null;

                                    bool newTabSelected = false;
                                    foreach (GroupBarItem itm in gBar.Items)
                                    {
                                        if (itm.IsHidden == false && itm.ShowInGroupBar == true)
                                        {
                                            gBar.SelectNewTab(itm);
                                            newTabSelected = true;
                                            break;
                                        }
                                    }
                                    if (newTabSelected == false)
                                    {
                                        if (gBar.m_toolbar != null && gBar.m_toolbar.Items.Count > 0)
                                        {
                                            ((NavigationToolbarItem)(gBar.m_toolbar.Items[gBar.m_toolbar.Items.Count - 1])).IsSelected = true;
                                        }
                                    }
                                }
                                if (gBar.IsSplitted)
                                    gBar.StackItemsHost.Height = gBar.StackItemsHost.ActualHeight - gBar.ItemHeaderHeight;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the item to clipboard.
        /// </summary>
        /// <param name="item">The item groupViewItems.</param>
        /// <param name="dataObj">The data obj groupViewItems.</param>
        private static void SetItemToClipboard(object item, DataObject dataObj)
        {
            try
            {
                if (item != null)
                {
                    MemoryStream stream = new MemoryStream();
                    XamlWriter.Save(item, stream);
                    dataObj.SetData(C_formatGroupViewItem, stream);
                    Clipboard.SetDataObject(dataObj);
                }
            }
            //SU I78477
            //catch (Exception ex)
            catch (Exception)
                //EU I78477
            { }
        }

        /// <summary>
        /// Preparing the data to set to the clipboard.
        /// If data is not of <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> type, 
        /// it is converted to <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> type.
        /// </summary>
        /// <param name="item">Object to save.</param>
        /// <param name="text">Text of the object.</param>
        /// <param name="imageSource">Image of the object.</param>
        private static void SetItemToClipboard(object item, string text, ImageSource imageSource)
        {
            if (item != null)
            {
                DataObject dataObj = new DataObject();

                if (item is GroupViewItem == false && text != null && imageSource != null)
                {
                    GroupViewItem newItem = new GroupViewItem
                                                {
                                                    Text = text,
                                                    ImageSource = imageSource
                                                };
                    SetItemToClipboard(newItem, dataObj);
                }
                else
                {
                    SetItemToClipboard(item, dataObj);
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> from the clipboard.
        /// </summary>
        /// <param name="dataObject">The data object.</param>
        /// <returns>
        /// The <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/>.
        /// </returns>
        private static object GetItemFromClipboard(DataObject dataObject)
        {
            MemoryStream ms = dataObject.GetData(C_formatGroupViewItem) as MemoryStream;
            object loadedItem = null;
            if (ms != null)
            {
                loadedItem = XamlReader.Load(ms);
            }

            return loadedItem;
        }

        /// <summary>
        /// Sets <see cref="SelectedObject"/> of the <see cref="GroupBar"/>.
        /// </summary>
        /// <param name="groupView">The <see cref="Syncfusion.Windows.Tools.Controls.GroupView"/> object.</param>
        /// <param name="viewItem">The <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> object to set.</param>
        private static void SetGroupBarSelectedObject(GroupView groupView, GroupViewItem viewItem)
        {
            GroupBarItem barItem = groupView.LogicalParent;
            GroupBar parentGroupBar = barItem.LogicalParent;
            parentGroupBar.SelectedObject = viewItem;
        }

        /// <summary>
        /// Gets the selected item in the <see cref="Syncfusion.Windows.Tools.Controls.GroupView"/>.
        /// </summary>
        /// <param name="groupView">The <see cref="Syncfusion.Windows.Tools.Controls.GroupView"/> object.</param>
        /// <param name="item">The <see cref="Syncfusion.Windows.Tools.Controls.GroupViewItem"/> object.</param>
        /// <returns>The selected item of the <see cref="Syncfusion.Windows.Tools.Controls.GroupView"/>.</returns>
        private static GroupViewItem SetSelectedItemInGroup(GroupView groupView, GroupViewItem item)
        {
            GroupViewItem selectedItem = item;
            ItemCollection grViewItems = groupView.Items;
            int index = grViewItems.IndexOf(item);
            int cnt = grViewItems.Count;

            if (grViewItems.Count > 0)
            {
                if (item == grViewItems[0])
                {
                    if (grViewItems.Count >= 2)
                    {
                        selectedItem = grViewItems[1] as GroupViewItem;
                    }
                }
                else if (item == grViewItems[cnt - 1])
                {
                    if (cnt - 2 >= 0 && cnt - 2 < grViewItems.Count)
                    {
                        selectedItem = grViewItems[cnt - 2] as GroupViewItem;
                    }
                }
                else
                {
                    if (index + 1 >= 0 && index + 1 < grViewItems.Count)
                    {
                        selectedItem = grViewItems[index + 1] as GroupViewItem;
                    }
                }
            }

            return selectedItem;
        }

        /// <summary>
        /// Invoked when height of the control is changed. Updates the
        /// value of <see cref="ItemContentLength"/> property within vertical layout.
        /// </summary>
        /// <param name="d">The <see cref="GroupBar"/> object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar owner = d as GroupBar;

            if (owner != null && owner.IsLoaded && !owner.AnimationInProgress)
            {
                if (owner.Orientation == Orientation.Vertical)
                {
                    owner.UpdateItemContentLength((double)e.OldValue, (double)e.NewValue);
                }
            }
        }

        /// <summary>
        /// Invoked when width of the control is changed. Updates the
        /// value of <see cref="ItemContentLength"/> property within horizontal layout.
        /// </summary>
        /// <param name="d">The <see cref="GroupBar"/> object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar owner = d as GroupBar;

            if (owner != null && owner.IsLoaded && !owner.AnimationInProgress)
            {
                if (owner.Orientation == Orientation.Horizontal)
                {
                    owner.UpdateItemContentLength((double)e.OldValue, (double)e.NewValue);
                }
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseUp"/> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that the mouse button was released.</param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.ChangedButton == MouseButton.Right && e.RightButton == MouseButtonState.Released
                && !(e.Source is GroupBarItem))
            {
                //m_bCanPasteExecute = false;
            }
        }

        /// <summary>
        /// Invoked when the value of <see cref="ItemContentLength"/> property is changed.
        /// </summary>
        /// <param name="d">The <see cref="GroupBar"/> object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnItemContentLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar owner = d as GroupBar;
            if (owner != null
                && owner.IsLoaded
                && owner.VisualMode != VisualMode.StackMode
                && !owner.TemplateChanging)
            {
                for (int i = 0; i < owner.Items.Count; i++)
                {
                    GroupBarItem item = (GroupBarItem)owner.ItemContainerGenerator.ContainerFromIndex(i);
                    if (item != null)
                    {
                        if (item.IsLoaded)
                        {
                            item.UpdateContentAnimation();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Coerces the value of <see cref="ItemContentLength"/> property. The value of this
        /// property cannot be negative. If negative value is going to be
        /// set: in stack mode splitting is automatically made; in
        /// non-stack mode nothing happens.
        /// </summary>
        /// <param name="d">The <see cref="GroupBar"/> object.</param>
        /// <param name="value">The instance containing the event data.</param>
        /// <returns>
        /// Value that is always non-negative.
        /// </returns>
        private static object OnItemContentLengthCoerce(DependencyObject d, object value)
        {
            GroupBar owner = d as GroupBar;

            if (owner != null && !owner._isItemsLoadedRunTime)
            {
                if ((double)value < 0d)
                {
                    if (owner.VisualMode != VisualMode.StackMode || owner.StackItemsCount == 0)
                    {
                        return 0d;
                    }
                    if((double)value > owner.ItemHeaderHeight)
                        owner.DoSplitting(DragDirection.Down);
                }

                if (owner.VisualMode == VisualMode.StackMode && !owner.IsSplitting && owner.StackItemsHost != null)
                {
                    double height = owner.ActualHeight - (double)value;
                    double itemsHeight = owner.VisibleItemsCount * owner.ItemHeaderHeight;
                    double bordersHeight = owner.BorderThickness.Top + owner.BorderThickness.Bottom;
                    double splitterMargin = owner.Splitter.Margin.Top + owner.Splitter.Margin.Bottom;
                    double elementsHeight = owner.HeaderHeight + owner.Splitter.Height + splitterMargin + itemsHeight + bordersHeight;
                    if (owner.StackItemHostVisibility != Visibility.Collapsed)
                    {
                        elementsHeight += owner.StackItemHostHeight;
                    }

                    if (height != elementsHeight)
                    {
                        if (owner.Orientation == Orientation.Vertical)
                        {
                            return owner.ActualHeight - elementsHeight;
                        }
                        else
                        {
                            return owner.ActualWidth - elementsHeight;
                        }
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// Invoked when <see cref="FlowDirection"/> property is changed.
        /// </summary>
        /// <param name="d">The <see cref="GroupBar"/> object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnFlowDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar owner = d as GroupBar;

            if (owner != null && owner.IsLoaded)
            {
                owner.OnFlowDirectionChanged();
            }
        }

        /// <summary>
        /// Invoked when the <see cref="StackItemHostHeight"/> property is changed.
        /// </summary>
        /// <param name="d">The <see cref="GroupBar"/> object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnStackItemHostHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar instance = (GroupBar)d;
            instance.OnStackItemHostHeightChanged(e);
        }

        /// <summary>
        /// Invoked when the <see cref="StackItemHostHeight"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnStackItemHostHeightChanged(DependencyPropertyChangedEventArgs e)
        {
            if (StackItemHostHeightChanged != null)
            {
                StackItemHostHeightChanged(this, e);
            }

            if (m_stackItemsHost != null)
            {
                CalculateItemContentLength((double)e.NewValue);
            }
        }

        /// <summary>
        /// Coerces the value of <see cref="StackItemHostHeight"/> property.
        /// </summary>
        /// <param name="d">Object for which property coerce is done.</param>
        /// <param name="value">Value that should be checked.</param>
        /// <returns>
        /// Checked value.
        /// </returns>
        private static object OnStackItemHostHeightCoerce(DependencyObject d, object value)
        {
            GroupBar groupBar = d as GroupBar;
            double correctValue = (double)value;

            if (groupBar != null && correctValue < 0)
            {
                correctValue = GroupBar.DEF_TOOLBAR_HEIGHT;
            }

            return correctValue;
        }

        /// <summary>
        /// Coerces the value of <see cref="HeaderHeight"/> property.
        /// </summary>
        /// <param name="d">Object for which property coerce is done.</param>
        /// <param name="value">Value that should be checked.</param>
        /// <returns>
        /// Checked value.
        /// </returns>
        private static object OnHeaderHeightCoerce(DependencyObject d, object value)
        {
            GroupBar groupBar = d as GroupBar;
            double correctValue = (double)value;

            if (groupBar != null && correctValue <= 0)
            {
                correctValue = GroupBar.DEF_HEADER_HEIGHT;
            }

            return correctValue;
        }

        /// <summary>
        /// Invoked when the value of <see cref="HeaderHeight"/> property is changed.
        /// </summary>
        /// <param name="d">The <see cref="GroupBar"/> object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnHeaderHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Invoked when the value ItemsHeaderHeight property is changed.
        /// </summary>
        /// <param name="d">The <see cref="GroupBar"/> object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnItemsHeaderHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar owner = d as GroupBar;
            if (owner != null
                && owner.VisualMode == VisualMode.StackMode
                && e != null)
            {
                double newValue = (double)e.NewValue;
            }
        }

        /// <summary>
        /// Invoked when <see cref="VisualMode"/> property is changed. Method sets a new template
        /// for the control.
        /// </summary>
        /// <param name="d">The <see cref="GroupBar"/> object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnVisualModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar owner = d as GroupBar;

            if (owner != null && owner.IsLoaded)
            {
                owner.AllowCollapse = (bool)owner.CoerceAllowCollapse(owner.AllowCollapse);
                owner.IsCollapsed = (bool)owner.CoerceIsCollapsed(owner.IsCollapsed);

                owner.CollapseAll();

                if (owner.SelectedTab != null)
                {
                    owner.SelectedTab.SetExpanded();
                }
                else if (owner.SelectedItem != null
                    && owner.SelectedItem.LogicalParent != null
                    && owner.SelectedItem.LogicalParent.LogicalParent != null)
                {
                    owner.SelectedObject = owner.SelectedItem.LogicalParent.LogicalParent;

                    if (owner.SelectedTab != null)
                    {
                        owner.SelectedTab.SetExpanded();
                    }
                }

                owner.RecalculateItemContentLength();
            }
        }

        /// <summary>
        /// Invoked when <see cref="VisualStyle"/> property is changed. Method sets a new skin
        /// for the control.
        /// </summary>
        /// <param name="d">The <see cref="GroupBar"/> object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnVisualStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar owner = d as GroupBar;

            if (owner != null)
            {
                switch (owner.VisualStyle)
                {
                    case VisualStyle.Office2007Blue:
                        {
                            SkinStorage.SetVisualStyle(owner, C_nameOffice2007BlueVisualStyle);
                            break;
                        }

                    case VisualStyle.Office2007Silver:
                        {
                            SkinStorage.SetVisualStyle(owner, C_nameOffice2007SilverVisualStyle);
                            break;
                        }

                    case VisualStyle.Office2007Black:
                        {
                            SkinStorage.SetVisualStyle(owner, C_nameOffice2007BlackVisualStyle);
                            break;
                        }

                    case VisualStyle.Office2003:
                        {
                            SkinStorage.SetVisualStyle(owner, C_nameOffice2003VisualStyle);
                            break;
                        }

                    case VisualStyle.Blend:
                        {
                            SkinStorage.SetVisualStyle(owner, C_nameBlendVisualStyle);
                            break;
                        }

                    case VisualStyle.LunaNormalColor:
                        {
                            SkinStorage.SetVisualStyle(owner, C_nameLunaNormalColorVisualStyle);
                            break;
                        }

                    case VisualStyle.LunaHomestead:
                        {
                            SkinStorage.SetVisualStyle(owner, C_nameLunaHomesteadVisualStyle);
                            break;
                        }

                    case VisualStyle.LunaMetallic:
                        {
                            SkinStorage.SetVisualStyle(owner, C_nameLunaMetallicVisualStyle);
                            break;
                        }

                    case VisualStyle.RoyaleNormalColor:
                        {
                            SkinStorage.SetVisualStyle(owner, C_nameRoyaleNormalColorVisualStyle);
                            break;
                        }

                    case VisualStyle.ZuneNormalColor:
                        {
                            SkinStorage.SetVisualStyle(owner, C_nameZuneNormalColorVisualStyle);
                            break;
                        }

                    case VisualStyle.AeroNormalColor:
                        {
                            SkinStorage.SetVisualStyle(owner, C_nameAeroNormalColorVisualStyle);
                            break;
                        }

                    default:
                        {
                            SkinStorage.SetVisualStyle(owner, C_nameDefaultVisualStyle);
                            break;
                        }
                }
                owner.CollapseAll();

                if (owner.SelectedTab != null)
                {
                    owner.SelectedTab.SetExpanded();
                }
                else if (owner.SelectedItem != null
                    && owner.SelectedItem.LogicalParent != null
                    && owner.SelectedItem.LogicalParent.LogicalParent != null)
                {
                    owner.SelectedObject = owner.SelectedItem.LogicalParent.LogicalParent;

                    if (owner.SelectedTab != null)
                    {
                        owner.SelectedTab.SetExpanded();
                    }
                }

                UpdateMenuFlowDirection(owner);

                owner.UpdateItemsSize();
                owner.UpdateLayout();
            }
        }

        /// <summary>
        /// Sets FlowDirection of the GroupBar to MainMenu when they are different.
        /// </summary>
        /// <param name="owner">GroupBar object</param>
        private static void UpdateMenuFlowDirection(GroupBar owner)
        {
            if (owner.Toolbar != null)
            {
                owner.Toolbar.MainMenu.FlowDirection = owner.FlowDirection;
            }
        }

        /// <summary>
        /// Invoked when <see cref="Orientation"/> property is changed. Methods manages
        /// orientation change animations.
        /// </summary>
        /// <param name="d">The GroupBar object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar owner = d as GroupBar;

            if (owner != null && owner.IsLoaded && e != null)
            {
                Orientation newOrientation = (Orientation)e.NewValue;

                //if (newOrientation == Orientation.Vertical)
                //{
                //    owner.RotationAngle = 0d;                   
                //}
                //else
                //{
                //    owner.RotationAngle = -90d;                   
                //}

                if (!owner.AnimationInProgress)
                {
                    owner.AnimationInProgress = true;

                if (newOrientation == Orientation.Vertical)
                {
                        owner.MainHost.Height = owner.ActualWidth;
                        owner.MainHost.Width = owner.ActualHeight;

                        owner.ContentRotationAngle = 0d;
                        owner.m_verticalOrientationStoryboard.Begin(owner.MainHost, true);
                }
                else
                {
                        owner.MainHost.Width = owner.ActualWidth;
                        owner.MainHost.Height = owner.ActualHeight;

                        owner.ContentRotationAngle = 90d;
                        owner.m_horizontalOrientationStoryboard.Begin(owner.MainHost, true);
                }

                    owner.Height = double.NaN;
                    owner.Width = double.NaN;

                    if (owner.VisualMode == VisualMode.StackMode)
                    {
                        owner.m_orientationChangedContentStoryboard.Begin(owner.m_visualContent);
                    }
                }
                else
                {
                    if (newOrientation == Orientation.Vertical)
                    {
                        owner.ContentRotationAngle = 0d;

                        owner.m_horizontalOrientationStoryboard.Pause(owner.MainHost);
                        owner.m_verticalOrientationStoryboard.Begin(owner.MainHost, true);
                    }
                    else
                    {
                        owner.ContentRotationAngle = 90d;

                        owner.m_verticalOrientationStoryboard.Pause(owner.MainHost);
                        owner.m_horizontalOrientationStoryboard.Begin(owner.MainHost, true);
                    }
                }

                owner.OnOrientationChanging();
            }
        }

        /// <summary>
        /// Called when [selected tab changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedTabChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar instance = (GroupBar)d;
            instance.OnSelectedTabChanged(e);
        }

        /// <summary>
        /// Invoked when <see cref="SelectedTab"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnSelectedTabChanged(DependencyPropertyChangedEventArgs e)
        {
            GroupBarItem item = e.OldValue as GroupBarItem;

            if (item != null)
            {
                item.IsSelected = false;
            }

            item = e.NewValue as GroupBarItem;

            if (item != null)
            {
                SelectNewTab(item);
            }

            if (SelectedTab != null)
            {
                if (Toolbar != null && Toolbar.GetToolbarItem(item) == null)
                {
                    foreach (var toolbaritem in Toolbar.Items)
                    {
                        NavigationToolbarItem toolbarItem = toolbaritem as NavigationToolbarItem;
                        if (toolbarItem.IsSelected == true)
                            toolbarItem.IsSelected = false;
                    }
                }
            }


            if (SelectedTab != null)
            {
                //if (HeaderContent != null && HeaderContent.Visibility == Visibility.Visible)
                //{
                //    HeaderContent.Visibility = Visibility.Visible;
                //}
                //else
                if (HeaderContent != null && HeaderContent.Visibility != Visibility.Visible)
                {
                    HeaderContent.Visibility = Visibility.Visible;
                }
                //if (SelectedContent != null && (SelectedContent as FrameworkElement).Visibility == Visibility.Visible)
                //{
                //    (SelectedContent as FrameworkElement).Visibility = Visibility.Visible;
                //}
                //else
                FrameworkElement element = SelectedContent as FrameworkElement;
                if (element != null)
                {
                    if (SelectedContent != null && (SelectedContent as FrameworkElement).Visibility != Visibility.Visible)
                    {
                        (SelectedContent as FrameworkElement).Visibility = Visibility.Visible;
                    }
                }
                //// if (SelectedContent != null && (SelectedContent as FrameworkElement).Visibility == Visibility.Visible)
                //// {
                ////    (SelectedContent as FrameworkElement).Visibility = Visibility.Visible;

                //// }
                //// else if (SelectedContent != null && (SelectedContent as FrameworkElement).Visibility != Visibility.Visible)
                //// {
                ////    (SelectedContent as FrameworkElement).Visibility = Visibility.Collapsed;
                //// }

                FireSelectedTabChanged();
            }
        }

        /// <summary>
        /// Stores the bool value to denote whether navigation item is clicked.
        /// </summary>
        internal static bool isNavigatonItemClicked = false;

        /// <summary>
        /// Selects new tab in the group bar.
        /// </summary>
        /// <param name="item">tab to select</param>
        internal void SelectNewTab(GroupBarItem item)
        {
            item.IsSelected = true;
            SelectedContent = item.Content;
            GroupBarItemHeader header = null;
            string _headerString = string.Empty;
            if (item.Header != null)
            {
                if (item.Header is GroupBarItemHeader)
                {
                    header = item.Header as GroupBarItemHeader;
                    _headerString = header.Text;
                }
                else if (item.HeaderTemplate == null)
                    _headerString = item.Header.ToString();
            }

            if (Toolbar != null && Toolbar.GetToolbarItem(item) == null)
            {
                FrameworkElement selected = SelectedContent as FrameworkElement;

                if (selected != null)
                {
                    selected.Visibility = item.Visibility;
                }

                if (GroupBar.isNavigatonItemClicked && selected != null)
                {                  
                        selected.Visibility = Visibility.Visible;
                        if (_headerString != string.Empty)
                        {
                            SelectedHeader = _headerString;
                        }
                        else
                        {
                            SelectedHeader = SelectedContent;
                        }
                        GroupBar.isNavigatonItemClicked = false;                    
                }
                else
                {
                    if (IsCollapsed || item.Visibility != Visibility.Visible)
                    {
                        CollapseHeaderContent();
                    }

                    if (_headerString != string.Empty)
                    {
                        SelectedHeader = _headerString;
                    }
                    else
                    {
                        SelectedHeader = SelectedContent;
                    }
                }
            }
            else
            {
                if (IsCollapsed)
                {
                    CollapseHeaderContent();
                }

                SetSelectedHeader(header);
            }
        }

        /// <summary>
        /// Sets header or collapses header content.
        /// </summary>
        /// <param name="header">Selected Header</param>
        private void SetSelectedHeader(GroupBarItemHeader header)
        {
            if (header != null)
            {
                SelectedHeader = header.Text;
            }
            else
                SelectedHeader = SelectedContent;
        }

        /// <summary>
        /// Hides header content of the selected tab.
        /// </summary>
        private void CollapseHeaderContent()
        {
            if (HeaderContent != null)
            {
                HeaderContent.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Shows header content of the selected tab.
        /// </summary>
        private void ShowHeaderContent()
        {
            if (HeaderContent != null)
            {
                HeaderContent.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Called when [selected item changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar instance = (GroupBar)d;
            instance.OnSelectedItemChanged(e);
        }

        /// <summary>
        /// Invoked when <see cref="SelectedItem"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnSelectedItemChanged(DependencyPropertyChangedEventArgs e)
        {
            GroupViewItem groupItem = e.OldValue as GroupViewItem;

            if (groupItem != null)
            {
                groupItem.IsSelected = false;
            }

            groupItem = e.NewValue as GroupViewItem;

            if (groupItem != null)
            {
                groupItem.IsSelected = true;
            }

            if (SelectedItem != null)
            {
                FireSelectedItemChanged();
            }
        }

        /// <summary>
        /// Called when [selected object changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar instance = (GroupBar)d;
            instance.OnSelectedObjectChanged(e);
        }

        /// <summary>
        /// Called when [save original state changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSaveOriginalStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar instance = (GroupBar)d;
            instance.OnSaveOriginalStateChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:SaveOriginalStateChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSaveOriginalStateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Invoked when the <see cref="VerticalOrientationStoryboard"/> property is changed.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnVerticalOrientationStoryboardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar instance = d as GroupBar;

            if (instance != null && instance.VerticalOrientationStoryboard != null)
            {
                instance.m_verticalOrientationStoryboard = instance.VerticalOrientationStoryboard.Clone();
                instance.m_verticalOrientationStoryboard.Completed += new EventHandler(instance.OnOrientationStoryboardCompleted);
            }
        }

        /// <summary>
        /// Invoked when the <see cref="HorizontalOrientationStoryboard"/> property is changed.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnHorizontalOrientationStoryboardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar instance = d as GroupBar;

            if (instance != null && instance.HorizontalOrientationStoryboard != null)
            {
                instance.m_horizontalOrientationStoryboard = instance.HorizontalOrientationStoryboard.Clone();
                instance.m_horizontalOrientationStoryboard.Completed += new EventHandler(instance.OnOrientationStoryboardCompleted);
            }
        }

        /// <summary>
        /// Invoked when the <see cref="OrientationChangedContentStoryboard"/> property is changed.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnOrientationChangedContentStoryboardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar instance = d as GroupBar;

            if (instance != null && instance.OrientationChangedContentStoryboard != null)
            {
                instance.m_orientationChangedContentStoryboard = instance.OrientationChangedContentStoryboard.Clone();
                instance.m_orientationChangedContentStoryboard.Completed += new EventHandler(instance.OnOrientationStoryboardCompleted);
            }
        }

        /// <summary>
        /// Invoked when the <see cref="SelectedObject"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnSelectedObjectChanged(DependencyPropertyChangedEventArgs e)
        {
            GroupBarItem groupItem = e.OldValue as GroupBarItem;
            GroupViewItem viewItem = e.OldValue as GroupViewItem;

            if (groupItem != null && e.NewValue as GroupBarItem != null)
            {
                SelectedTab = null;
            }
            else if (viewItem != null)
            {
                SelectedItem = null;
            }

            groupItem = e.NewValue as GroupBarItem;
            viewItem = e.NewValue as GroupViewItem;

            if (groupItem != null)
            {
                SelectedTab = groupItem;
            }
            if (viewItem != null)
            {
                SelectedItem = viewItem;
            }

            if (Toolbar != null)
            {
                Toolbar.RefreshSelectedItems();
            }

            FireSelectedObjectChanged();
        }

        /// <summary>
        /// Used for the keyboard navigation.
        /// </summary>
        internal void MoveIn()
        {
            if (SelectedTab != null)
            {
                if (!SelectedTab.IsExpanded && VisualMode == VisualMode.MultipleExpansion)
                {
                    SelectedTab.IsExpanded = true;
                }
                else
                {
                    GroupView view = SelectedTab.Content as GroupView;

                    if (view != null && view.Items.Count > 0)
                    {
                        GroupViewItem viewItem = view.GetItemContainer(view.Items[0]);

                        if (viewItem != null)
                        {
                            viewItem.Focus();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Used for the keyboard navigation.
        /// </summary>
        internal void MoveOut()
        {
            if (SelectedItem != null
                && SelectedItem.LogicalParent != null
                && SelectedItem.LogicalParent.LogicalParent != null)
            {
                if (!SelectedItem.LogicalParent.LogicalParent.IsSelected)
                {
                    SelectedItem.LogicalParent.LogicalParent.FocusHeader();
                }
            }
            else if (SelectedTab != null && SelectedTab.IsExpanded
                && VisualMode == VisualMode.MultipleExpansion)
            {
                SelectedTab.FocusHeader();
                SelectedTab.IsExpanded = false;
            }
        }

        /// <summary>
        /// Refreshes IsLastItem property for all items of the collection.
        /// </summary>
        private void RefreshLastItem()
        {
            GroupBarItem item = null;

            if (Items.Count > 0)
            {
                for (int i = 0; i < Items.Count - 1; i++)
                {
                    item = Items[i] as GroupBarItem;

                    if (item != null)
                    {
                        item.IsLastItem = false;
                    }
                }

                item = Items[Items.Count - 1] as GroupBarItem;

                if (item != null)
                {
                    item.IsLastItem = true;
                }
            }
        }
        int deleted = 0;
        /// <summary>
        /// Sets new GroupBar from the saved state.
        /// </summary>
        /// <param name="stream">File stream with saved GroupBar state.</param>
        private void SetNewGroupBar(Stream stream)
        {
            try
            {
                GroupBarParams gbParamsList = XamlReader.Load(stream) as GroupBarParams;
                ArrayList barItems = gbParamsList.GroupBarItems;
                Orientation barOrentation = gbParamsList.Orentation;
                int barItemsCount = barItems.Count;
                ObservableCollection<Binding> listViewBindings = ClearListViewBindings();

                Items.Clear();
                int index = -1;
                int indexSelectTab = -1;
                int indexSelectItem = -1;

                while (barItemsCount > 0)
                {
                    GroupBarItem gbItem = barItems[0] as GroupBarItem;

                    barItems.Remove(gbItem);
                    barItemsCount--;
                    index = Items.Add(gbItem);

                    if (gbItem.Visibility == Visibility.Collapsed)
                    {
                        gbItem.Visibility = Visibility.Visible;
                    }
                    if (m_Presenter != null && m_Presenter.Visibility == Visibility.Collapsed)
                    {
                        m_Presenter.Visibility = Visibility.Visible;
                    }
                    if (HeaderHost != null && HeaderHost.Visibility == Visibility.Collapsed)
                    {
                        HeaderHost.Visibility = Visibility.Visible;
                    }

                    RecalculateItemContentLength();

                    gbItem.ApplyTemplate();
                    gbItem.UpdateContentSize(gbItem.IsExpanded);

                    if (gbItem.IsSelected && indexSelectTab == -1)
                    {
                        indexSelectTab = index;
                        this.SelectedTab = gbItem;
                    }
                    else if (gbItem.IsExpanded && indexSelectItem == -1)
                    {
                        GroupBarItem barItem = Items[index] as GroupBarItem;

                        if (barItem != null)
                        {
                            GroupView view = barItem.Content as GroupView;

                            if (view != null)
                            {
                                indexSelectItem = view.GetSelectedItemIndex();

                                if (indexSelectItem != -1)
                                {
                                    indexSelectTab = index;
                                }
                            }
                        }
                    }
                }
                
                if (indexSelectTab != -1)
                {
                    GroupBarItem barItem = Items[indexSelectTab] as GroupBarItem;

                    if (barItem != null && barItem.Content is GroupView)
                    {
                        GroupView view = barItem.Content as GroupView;
                        SelectedObject = (view == null || indexSelectItem == -1) ?
                        barItem : view.Items[indexSelectItem];
                    }
                }

                this.Orientation = barOrentation;
                UpdateItemsSize();
                UpdateLayout();
                
                if (null != m_toolbar)
                {
                    m_hiddenIndices.Clear();
                    m_toolbar.Items.Clear();
                    m_toolbar.RefreshButtonsMenu();

                    for (int i = 0; i < Items.Count; i++)
                    {
                        GroupBarItem item = ItemContainerGenerator.ContainerFromIndex(i) as GroupBarItem;

                        if (item.IsHidden && !item.ShowInGroupBar)
                        {
                            this.HideItem(item);
                        }

                        if (!item.IsHidden && !item.ShowInGroupBar)
                        {
                            item.Visibility = Visibility.Collapsed;
                            this.ShowItem(item);
                            m_toolbar.RefreshButtonsMenu();
                        }
                    }
                    RecalculateItemContentLength();
                }
                CreateListViewBindings(listViewBindings);
            }
            catch (XamlParseException e)
            {
                Debug.Print(e.Message);
            }
        }

        /// <summary>
        /// Loads the state persisted.
        /// </summary>
        /// <param name="isoStorage">Reference in isolated storage for
        /// load current docking location.</param>
        /// <param name="storeFileName">Present file name for isolated
        /// storage.</param>
        private void LoadBarStateFromIsoStorage(IsolatedStorageFile isoStorage, string storeFileName)
        {
            if (null != isoStorage && !String.IsNullOrEmpty(storeFileName)
                && 0 < isoStorage.GetFileNames(storeFileName).Length)
            {
                Stream stream = new IsolatedStorageFileStream(storeFileName, FileMode.OpenOrCreate, isoStorage);

                if (stream != null)
                {
                    SetNewGroupBar(stream);
                    m_isInLoadStateMode = false;
                }
            }
        }

        /// <summary>
        /// Updates the visibility of the content for all items.
        /// </summary>
        private void UpdateItemsContentVisibility()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                GroupBarItem item;
                if (Items[i] is GroupBarItem)
                {
                    item = Items[i] as GroupBarItem;
                }
                else
                {
                    item = ItemContainerGenerator.ContainerFromIndex(i) as GroupBarItem;
                }
                if (null != item)
                {
                    item.UpdateContentVisibility();
                }
            }
        }

        /// <summary>
        /// Finds the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> parent for the given element.
        /// </summary>
        /// <param name="element">The given element to find the <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> parent.</param>
        /// <returns>The <see cref="Syncfusion.Windows.Tools.Controls.GroupBarItem"/> object. </returns>
        private GroupBarItem GetGroupBarItemFromChildren(FrameworkElement element)
        {
            GroupBarItem item = null;

            if (element != null)
            {
                item = element as GroupBarItem;

                if (item == null)
                {
                    while (element != null)
                    {
                        element = VisualTreeHelper.GetParent(element) as FrameworkElement;

                        if (element is GroupBarItem)
                        {
                            item = (GroupBarItem)element;
                            break;
                        }
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Returns the first adorner layer in the visual tree above a specified Visual
        /// except for ScrollContentPresenter. 
        /// </summary>
        /// <param name="visual">The visual element for which to find an adorner layer.</param>
        /// <returns>An adorner layer for the specified visual, 
        /// or null if no adorner layer can be found.</returns>
        internal static AdornerLayer GetAdornerLayer(DependencyObject visual)
        {
            AdornerLayer layer = null;

            if (visual != null)
            {
                for (DependencyObject visual2 = VisualTreeHelper.GetParent(visual); visual2 != null; visual2 = VisualTreeHelper.GetParent(visual2))
                {
                    if (visual2 is AdornerDecorator)
                    {
                        layer = ((AdornerDecorator)visual2).AdornerLayer;
                        break;
                    }
                }
            }

            return layer;
        }

        /// <summary>
        /// Saves the state of the <see cref="GroupBar"/> for further resetting
        /// </summary>
        internal void SaveInitialState()
        {
            IsolatedStorageFile isoStorage = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            SaveBarState(isoStorage, C_ResetStoreFileName);
        }

        /// <summary>
        /// Gets the collection of IsListViewMode property bindings.
        /// </summary>
        /// <returns>The collection of IsListViewMode property bindings.</returns>
        private ObservableCollection<Binding> ClearListViewBindings()
        {
            ObservableCollection<Binding> listViewBindings = new ObservableCollection<Binding>();

            foreach (GroupBarItem item in Items)
            {
                GroupView v = item.Content as GroupView;
                if (v != null)
                {
                    Binding b = BindingOperations.GetBinding(v, GroupView.IsListViewModeProperty);
                    listViewBindings.Add(b);
                    BindingOperations.ClearBinding(v, GroupView.IsListViewModeProperty);
                }
            }

            return listViewBindings;
        }

        /// <summary>
        /// Creates the IsListViewMode property bindings for the 
        /// <see cref="Syncfusion.Windows.Tools.Controls.GroupView"/> objects.
        /// </summary>
        /// <param name="listViewBindings">The collection of the IsListViewMode property bindings.</param>
        private void CreateListViewBindings(ObservableCollection<Binding> listViewBindings)
        {
            foreach (GroupBarItem item in Items)
            {
                GroupView v = item.Content as GroupView;
                if (v != null)
                {
                    if (listViewBindings.Count > 0)
                    {
                        if (listViewBindings[0] != null)
                        {
                            Binding b = listViewBindings[0];
                            v.SetBinding(GroupView.IsListViewModeProperty, listViewBindings[0]);
                            listViewBindings.RemoveAt(0);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when [collapse button background changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCollapseButtonBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar instance = (GroupBar)d;
            instance.OnCollapseButtonBackgroundChanged(e);
        }

        /// <summary>
        /// Invoked when the <see cref="CollapseButtonBackground"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnCollapseButtonBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when [collapse button mouse over background changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCollapseButtonMouseOverBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar instance = (GroupBar)d;
            instance.OnCollapseButtonMouseOverBackgroundChanged(e);
        }

        /// <summary>
        /// Invoked when the <see cref="CollapseButtonMouseOverBackground"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnCollapseButtonMouseOverBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when [collapse button template changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCollapseButtonTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar instance = (GroupBar)d;
            instance.OnCollapseButtonTemplateChanged(e);
        }

        /// <summary>
        /// Invoked when the <see cref="CollapseButtonTemplate"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnCollapseButtonTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when [animation type changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAnimationTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar instance = (GroupBar)d;
            instance.OnAnimationTypeChanged(e);
        }

        /// <summary>
        /// Invoked when the <see cref="AnimationType"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnAnimationTypeChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when [animation speed changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAnimationSpeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar instance = (GroupBar)d;
            instance.OnAnimationSpeedChanged(e);
        }

        /// <summary>
        /// Invoked when the <see cref="AnimationSpeed"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnAnimationSpeedChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when [collapse button tool tip changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCollapseButtonToolTipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar instance = (GroupBar)d;
            instance.OnCollapseButtonToolTipChanged(e);
        }

        /// <summary>
        /// Invoked when the <see cref="CollapseButtonToolTip"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnCollapseButtonToolTipChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when [expand button tool tip changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnExpandButtonToolTipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar instance = (GroupBar)d;
            instance.OnExpandButtonToolTipChanged(e);
        }

        /// <summary>
        /// Invoked when the <see cref="ExpandButtonToolTip"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnExpandButtonToolTipChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when [allow collapse changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAllowCollapseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar instance = (GroupBar)d;
            instance.OnAllowCollapseChanged(e);
        }

        /// <summary>
        /// Invoked when the <see cref="AllowCollapse"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnAllowCollapseChanged(DependencyPropertyChangedEventArgs e)
        {
            IsCollapsed = (bool)CoerceIsCollapsed(IsCollapsed);
        }

        /// <summary>
        /// Coerces the value of the <see cref="AllowCollapse"/> property.
        /// </summary>
        /// <param name="d">The <see cref="GroupBar"/> object.</param>
        /// <param name="value">The instance containing the event data.</param>
        /// <returns>
        /// The checked value.
        /// </returns>
        private static object CoerceAllowCollapse(DependencyObject d, object value)
        {
            GroupBar owner = d as GroupBar;
            return owner.CoerceAllowCollapse(value);
        }

        /// <summary>
        /// Fulfils the logic before setting the value of <see cref="AllowCollapse"/> dependency property.
        /// </summary>
        /// <param name="value">The value that should be corrected.</param>
        /// <returns>The corrected value.</returns>
        protected internal virtual object CoerceAllowCollapse(object value)
        {
            return value;
        }

        /// <summary>
        /// Coerces the value of the <see cref="IsCollapsed"/> property.
        /// </summary>
        /// <param name="d">The <see cref="GroupBar"/> object.</param>
        /// <param name="value">The instance containing the event data.</param>
        /// <returns>
        /// The checked value.
        /// </returns>
        private static object CoerceIsCollapsed(DependencyObject d, object value)
        {
            GroupBar owner = d as GroupBar;
            return owner.CoerceIsCollapsed(value);
        }

        /// <summary>
        /// Fulfils the logic before setting the value of <see cref="IsCollapsed"/> dependency property.
        /// </summary>
        /// <param name="value">The value that should be corrected.</param>
        /// <returns>The corrected value.</returns>
        protected internal virtual object CoerceIsCollapsed(object value)
        {
            bool retVal = (bool)value;
            return retVal && AllowCollapse && VisualMode == VisualMode.StackMode;
        }

        /// <summary>
        /// Called when [is collapsed changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsCollapsedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar instance = (GroupBar)d;
            instance.OnIsCollapsedChanged(e);
        }

        /// <summary>
        /// Invoked when the <see cref="IsCollapsed"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnIsCollapsedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsCollapsed)
            {
                if (m_popupContentHost != null)
                {
                    m_popupContentHost.Content = SelectedContent;
                }
            }
            else
            {
                if (m_visualContent != null)
                {
                    m_visualContent.Content = SelectedContent;
                }
            }

            DependencyProperty propertyToAnimate = (Orientation == Orientation.Horizontal) ? MaxHeightProperty : MaxWidthProperty;

            switch (AnimationType)
            {
                case AnimationsType.Fade:
                    Opacity = 0.01;
                    DoubleAnimation opacityAnimation = new DoubleAnimation();
                    opacityAnimation.From = 0.01;
                    opacityAnimation.To = 1;
                    opacityAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(AnimationSpeed));
                    opacityAnimation.SpeedRatio = 0.7;
                    BeginAnimation(OpacityProperty, opacityAnimation);
                    break;
                case AnimationsType.Slide:
                    DoubleAnimation widthAnimation = new DoubleAnimation();
                    if (IsCollapsed)
                    {
                        widthAnimation.From = ActualWidth; //Width;
                        widthAnimation.To =  CollapsedWidth;
                    }
                    else
                    {
                        widthAnimation.From = CollapsedWidth;
                        widthAnimation.To = m_cachedWidth;// Width;
                    }

                    widthAnimation.Completed += new EventHandler(AnimationCompleted);
                    widthAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(AnimationSpeed));
                    BeginAnimation(propertyToAnimate, widthAnimation);
                    break;
            }

            if (Orientation == Orientation.Vertical)
            {
                if (IsCollapsed)
                {
                    if (!double.IsNaN(Width))
                    {
                        m_cachedWidth = Width;
                    }
                    else
                    {
                        m_cachedWidth = ActualWidth;
                    }

                    MaxWidth = CollapsedWidth;
                    MainHost.MaxWidth = CollapsedWidth;
                }
                else
                {
                    MaxWidth = double.MaxValue;
                    if (DynamicResizing)
                    {
                        Width = m_cachedWidth;
                        MainHost.Width = m_cachedWidth;
                    }
                    MainHost.MaxWidth = double.MaxValue;                   
                }

                m_contentPopup.LayoutTransform = new RotateTransform(0);
            }
            else
            {
                if (IsCollapsed)
                {
                    if (!double.IsNaN(Height))
                    {
                    m_cachedWidth = Height;
                    }
                    else
                    {
                        m_cachedWidth = ActualHeight;
                    }
                    MaxHeight = CollapsedWidth;
                    MainHost.MaxWidth = CollapsedWidth;
                }
                else
                {
                    MaxHeight = double.MaxValue;
                    if (!DynamicResizing)
                    {
                        Height = m_cachedWidth;
                        MainHost.Width = m_cachedWidth;
                    }
                    MainHost.MaxWidth = double.MaxValue;
                }

                m_contentPopup.LayoutTransform = new RotateTransform(90);
            }

            m_contentPopup.Height = 0;

            OnCollapsedChanged(e);
        }

        /// <summary>
        /// Occurs slide animation is finished.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> that contains the event data.</param>
        private void AnimationCompleted(object sender, EventArgs e)
        {
            if (Orientation == Orientation.Vertical)
            {
                BeginAnimation(MaxWidthProperty, null);
            }
            else
            {
                BeginAnimation(MaxHeightProperty, null);
            }
        }

        /// <summary>
        /// Occurs when dragging of the nodes is started.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DragTreeViewItemAdvEventArgs"/> that contains the event data.</param>
        private void TreeDragStart(object sender, DragTreeViewItemAdvEventArgs e)
        {
            if (m_contentPopup != null)
            {
                m_contentPopup.StaysOpen = true;
            }
        }

        /// <summary>
        /// Occurs when dragging of the nodes is finished.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DragTreeViewItemAdvEventArgs"/> that contains the event data.</param>
        private void TreeDragEnd(object sender, DragTreeViewItemAdvEventArgs e)
        {
            if (m_contentPopup != null)
            {
                m_contentPopup.StaysOpen = false;
            }
        }

        /// <summary>
        /// Called when [is drop down open changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar instance = (GroupBar)d;
            instance.OnIsDropDownOpenChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:IsDropDownOpenChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnIsDropDownOpenChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!IsDropDownOpen)
            {
                OnAfterGroupBarItemPopupClosed(new EventArgs());
            }
        }

        /// <summary>
        /// Coerces the value of the <see cref="IsDropDownOpen"/> property.
        /// </summary>
        /// <param name="d">The <see cref="GroupBar"/> object.</param>
        /// <param name="value">The instance containing the event data.</param>
        /// <returns>
        /// The checked value.
        /// </returns>
        public static object CoerceIsDropDownOpen(DependencyObject d, object value)
        {
            GroupBar owner = d as GroupBar;
            return owner.CoerceIsDropDownOpen(value);
        }

        /// <summary>
        /// Fulfils the logic before setting the value of <see cref="IsDropDownOpen"/> dependency property.
        /// </summary>
        /// <param name="value">The value that should be corrected.</param>
        /// <returns>The corrected value.</returns>
        protected internal virtual object CoerceIsDropDownOpen(object value)
        {
            bool retVal = (bool)value && IsCollapsed;
            if (retVal)
            {
                OnBeforeGroupBarItemPopupOpened(new BeforeGroupBarItemPopupOpenedEventArgs(SelectedTab));
            }

            return retVal;
        }

        /// <summary>
        /// Occurs when popup is opened.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void PopupOpened(object sender, EventArgs e)
        {
            if (m_popupContentHost != null)
            {
                SetContentPopupSize();

                if (IsCollapsed && m_isVisualSkinChanged)
                {
                    m_isVisualSkinChanged = false;
                    m_contentPopup.Width = NavigationPopupSize.Width - 1;
                }

                m_contentPopup.Placement = PlacementMode.Relative;
                if (Orientation == Orientation.Vertical)
                {
                    m_contentPopup.HorizontalOffset = m_contentButton.ActualWidth;
                }
                else
                {
                    m_contentPopup.HorizontalOffset = 0;
                }

                m_popupContentHost.Content = SelectedContent;
                ApplyPopupStaysOpenLogic();

                CalculateMinWidthOfList();
            }

            if (m_visualContent != null)
            {
                m_visualContent.Content = null;
            }
        }

        /// <summary>
        /// Calculates value of m_minWidthOfList to show the Horizontal scroll.
        /// </summary>
        private void CalculateMinWidthOfList()
        {
            GroupView content = SelectedContent as GroupView;

            if (content != null)
            {
                double maxWidth = 0;

                for (int i = 0, cnt = content.Items.Count; i < cnt; i++)
                {
                    GroupViewItem item = content.Items[i] as GroupViewItem;

                    if (item != null)
                    {
                        double sumWidth = item.GetInternalContentLength();

                        if (sumWidth > maxWidth)
                        {
                            maxWidth = sumWidth;
                        }
                    }
                }

                m_minWidthOfList = maxWidth;
            }
        }

        /// <summary>
        /// Sets size of the ContentPopup.
        /// </summary>
        private void SetContentPopupSize()
        {
            m_contentPopup.Width = NavigationPopupSize.Width;
            m_contentPopup.Height = NavigationPopupSize.Height;
        }

        /// <summary>
        /// Sets the focused item.
        /// </summary>
        /// <param name="item">The item FocusedItem. </param>
        private void SetFocusedItem(FrameworkElement item)
        {
            if (item != null)
            {
                if (!item.Focusable)
                {
                    item.Focusable = true;
                }

                DependencyObject scope = FocusManager.GetFocusScope(item);
                FocusManager.SetFocusedElement(scope, item);
            }
        }

        /// <summary>
        /// Used to make popup stay open while dragging the node in <see cref="TreeViewAdv"/>.
        /// </summary>
        private void ApplyPopupStaysOpenLogic()
        {
            if (m_popupContentHost.Content is TreeViewAdv)
            {
                TreeViewAdv tree = m_popupContentHost.Content as TreeViewAdv;
                tree.DragStart += new DragTreeViewItemAdvHandler(TreeDragStart);
                tree.DragEnd += new DragTreeViewItemAdvHandler(TreeDragEnd);
            }
            else if (m_popupContentHost.Content is Visual)
            {
                foreach (TreeViewAdv tree in VisualUtils.EnumChildrenOfType(m_popupContentHost.Content as Visual, typeof(TreeViewAdv)))
                {
                    tree.DragStart += new DragTreeViewItemAdvHandler(TreeDragStart);
                    tree.DragEnd += new DragTreeViewItemAdvHandler(TreeDragEnd);
                }
            }
        }

        /// <summary>
        /// Occurs when popup is closed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void PopupClosed(object sender, EventArgs e)
        {
            if (m_contentButton != null && Mouse.DirectlyOver != m_contentButton)
            {
                m_contentButton.IsChecked = false;
            }

            if (m_visualContent != null)
            {
                m_visualContent.Content = m_popupContentHost.Content;
            }

            if (m_popupContentHost != null)
            {
                m_popupContentHost.Content = null;
            }
        }

        /// <summary>
        /// Indicates whether resizing of the popup is vertical.
        /// </summary>
        /// <param name="pos">Point to check.</param>
        /// <returns>Returns the value indicating whether resizing of the popup is vertical.</returns>
        private bool IsVerticalResizing(Point pos)
        {
            bool val = false;
            if (PopupResizeDirection == PopupResizeDirection.Both || PopupResizeDirection == PopupResizeDirection.Vertical)
            {
                if (m_popupResizeBorder != null)
                {
                    val = pos.X > 0 && pos.X < m_popupResizeBorder.ActualWidth
                       && pos.Y > m_popupResizeBorder.ActualHeight - C_defaultMargin && pos.Y <= m_popupResizeBorder.ActualHeight;
                }
            }

            return val;
        }

        /// <summary>
        /// Indicates whether resizing of the popup is horizontal.
        /// </summary>
        /// <param name="pos">Point to check.</param>
        /// <returns>Returns the value indicating whether resizing of the popup is horizontal.</returns>
        private bool IsHorizontalResizing(Point pos)
        {
            bool val = false;
            if (PopupResizeDirection == PopupResizeDirection.Both || PopupResizeDirection == PopupResizeDirection.Horizontal)
            {
                if (m_popupResizeBorder != null)
                {
                    if (!IsReverseResizing())
                    {
                        val = pos.Y > 0 && pos.Y < m_popupResizeBorder.ActualHeight
                            && pos.X > m_popupResizeBorder.ActualWidth - C_defaultMargin && pos.X <= m_popupResizeBorder.ActualWidth;
                    }
                    else
                    {
                        val = pos.Y > 0 && pos.Y < m_popupResizeBorder.ActualHeight
                            && pos.X >= 0 && pos.X <= C_defaultMargin;
                    }
                }
            }

            return val;
        }

        /// <summary>
        /// Indicates whether resizing of the popup is horizontal and vertical.
        /// </summary>
        /// <param name="pos">Point to check.</param>
        /// <returns>Returns the value indicating whether resizing of the popup is horizontal.</returns>
        private bool IsCornerResizing(Point pos)
        {
            bool val = false;
            if (PopupResizeDirection == PopupResizeDirection.Both)
            {
                if (m_popupResizeBorder != null)
                {
                    if (!IsReverseResizing())
                    {
                        val = pos.Y > m_popupResizeBorder.ActualHeight - C_defaultMargin * 2 && pos.Y <= m_popupResizeBorder.ActualHeight
                            && pos.X > m_popupResizeBorder.ActualWidth - C_defaultMargin * 2 && pos.X <= m_popupResizeBorder.ActualWidth;
                    }
                    else
                    {
                        val = pos.Y > m_popupResizeBorder.ActualHeight - C_defaultMargin * 2 && pos.Y <= m_popupResizeBorder.ActualHeight
                            && pos.X >= 0 && pos.X <= C_defaultMargin * 2;
                    }
                }
            }

            return val;
        }

        /// <summary>
        /// Indicates whether resizing of the popup should be reverse.
        /// </summary>
        /// <returns>bool val IsReverseResizing</returns>
        private bool IsReverseResizing()
        {
            bool val = false;
            if (m_popupResizeBorder != null)
            {
                Point popupPoint = m_popupResizeBorder.PointToScreen(new Point(0, 0));
                Point groupBarPoint = PointToScreen(new Point(0, 0));
                if (FlowDirection == FlowDirection.RightToLeft)
                {
                    val = popupPoint.X > groupBarPoint.X;
                }
                else
                {
                    val = popupPoint.X < groupBarPoint.X;
                }
            }

            return val;
        }

        /// <summary>
        /// Occurs when mouse is down on the popup border.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> containing the event data.</param>
        private void PopupResizeBorderMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (m_popupResizeBorder != null)
            {
                Point pos = e.GetPosition((IInputElement)m_popupResizeBorder);
                m_isHorizontalResizing = IsHorizontalResizing(pos);
                m_isVerticalResizing = IsVerticalResizing(pos);
                m_isCornerResizing = IsCornerResizing(pos);
                m_adjustHeight = m_popupResizeBorder.ActualHeight - pos.Y;
                if (!IsReverseResizing())
                {
                    m_adjustWidth = m_popupResizeBorder.ActualWidth - pos.X;
                }
                else
                {
                    m_adjustWidth = pos.X;
                }
            }

            if (m_isVerticalResizing || m_isHorizontalResizing || m_isCornerResizing)
            {
                Mouse.Capture(m_popupResizeBorder);
            }
        }

        /// <summary>
        /// Occurs when mouse moves over the popup border.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> containing the event data.</param>
        private void PopupResizeBorderMouseMove(object sender, MouseEventArgs e)
        {
            if (m_popupResizeBorder != null)
            {
                Point pos = e.GetPosition((IInputElement)m_popupResizeBorder);
                SetPopupCursor(pos);

                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (m_isCornerResizing)
                    {
                        if (pos.Y > 0)
                        {
                            m_contentPopup.Height = pos.Y + m_adjustHeight;
                        }

                        if (!IsReverseResizing())
                        {
                            if (pos.X > 10)
                            {
                                SetContentPopupWidth(pos);
                            }
                        }
                        else
                        {
                            if (m_popupResizeBorder.ActualWidth - pos.X + m_adjustWidth > 10)
                            {
                                m_contentPopup.Width = m_popupResizeBorder.ActualWidth - pos.X + m_adjustWidth;
                            }
                        }
                    }
                    else if (m_isVerticalResizing)
                    {
                        if (pos.Y > 10)
                        {
                            m_contentPopup.Height = pos.Y + m_adjustHeight;
                        }
                    }
                    else if (m_isHorizontalResizing)
                    {
                        if (!IsReverseResizing())
                        {
                            if (pos.X > 10)
                            {
                                SetContentPopupWidth(pos);
                            }
                        }
                        else
                        {
                            if (m_popupResizeBorder.ActualWidth - pos.X + m_adjustWidth > 10)
                            {
                                m_contentPopup.Width = m_popupResizeBorder.ActualWidth - pos.X + m_adjustWidth;
                            }
                        }
                    }
                }

                NavigationPopupSize = new Size(m_contentPopup.Width, m_contentPopup.Height);
            }
        }

        /// <summary>
        /// Sets width value for content popup.
        /// </summary>
        /// <param name="pos">position point of the popup border</param>
        private void SetContentPopupWidth(Point pos)
        {
            Point point = PointToScreen(pos);
            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.PrimaryScreen;
            const double ScrEndOffset = 4;
            double offset = ActualHeight;

            if (Orientation == Orientation.Vertical)
            {
                offset = ActualWidth;
            }

            if (FlowDirection == FlowDirection.RightToLeft)
            {
                if (ScrEndOffset < point.X - offset)
                {
                    m_contentPopup.Width = pos.X + m_adjustWidth;
                }
            }
            else if (screen.Bounds.Width - ScrEndOffset > point.X + offset)
            {
                m_contentPopup.Width = pos.X + m_adjustWidth;
            }
        }

        /// <summary>
        /// Occurs when mouse is up on the popup border.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> containing the event data.</param>
        private void PopupResizeBorderMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (m_isHorizontalResizing || m_isVerticalResizing || m_isCornerResizing)
            {
                Mouse.Capture(null);
                m_isHorizontalResizing = false;
                m_isVerticalResizing = false;
                m_isCornerResizing = false;

                m_oldPopupHeight = m_contentPopup.ActualHeight;
            }
        }

        /// <summary>
        /// Sets necessary popup cursor.
        /// </summary>
        /// <param name="pos">The pos PopupCursor.</param>
        private void SetPopupCursor(Point pos)
        {
            if (IsCornerResizing(pos))
            {
                if (FlowDirection == FlowDirection.LeftToRight)
                {
                    if (!IsReverseResizing())
                    {
                        m_popupResizeBorder.Cursor = Cursors.SizeNWSE;
                    }
                    else
                    {
                        m_popupResizeBorder.Cursor = Cursors.SizeNESW;
                    }
                }
                else
                {
                    if (!IsReverseResizing())
                    {
                        m_popupResizeBorder.Cursor = Cursors.SizeNESW;
                    }
                    else
                    {
                        m_popupResizeBorder.Cursor = Cursors.SizeNWSE;
                    }
                }
            }
            else if (IsVerticalResizing(pos))
            {
                m_popupResizeBorder.Cursor = Cursors.SizeNS;
            }
            else if (IsHorizontalResizing(pos))
            {
                m_popupResizeBorder.Cursor = Cursors.SizeWE;
            }
            else
            {
                m_popupResizeBorder.Cursor = Cursors.Arrow;
            }
        }

        /// <summary>
        /// Calls OnIsToolBarEnabledChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsToolBarEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar instance = (GroupBar)d;
            instance.OnIsToolBarEnabledChanged(e);
        }

        /// <summary>
        /// Invoked when the OnIsToolBarEnabledChanged property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnIsToolBarEnabledChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Toolbar != null)
            {
                while (Toolbar.Items.Count > 0)
                {
                    DoSplitting(DragDirection.Up);
                }

                Toolbar.SetMenuEnabled((bool)e.NewValue);
            }

            RecalculateItemContentLength();
        }

        /// <summary>
        /// Coerces the value of the <see cref="IsToolBarEnabled"/> property.
        /// </summary>
        /// <param name="d">The <see cref="GroupBar"/> object.</param>
        /// <param name="value">The instance containing the event data.</param>
        /// <returns>
        /// The checked value.
        /// </returns>
        public static object CoerceIsToolBarEnabled(DependencyObject d, object value)
        {
            GroupBar owner = d as GroupBar;
            return owner.CoerceIsToolBarEnabled(value);
        }

        /// <summary>
        /// Fulfils the logic before setting the value of <see cref="IsToolBarEnabled"/> dependency property.
        /// </summary>
        /// <param name="value">The value that should be corrected.</param>
        /// <returns>The corrected value.</returns>
        protected internal virtual object CoerceIsToolBarEnabled(object value)
        {
            return value;
        }

        /// <summary>
        /// Calls OnSelectedContentChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSelectedContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar instance = (GroupBar)d;
            instance.OnSelectedContentChanged(e);
        }

        /// <summary>
        /// Invoked when the OnSelectedContentChanged property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnSelectedContentChanged(DependencyPropertyChangedEventArgs e)
        {
            if (VisualMode == VisualMode.StackMode)
            {
                ScrollViewer scrollviewer=null;
                if (IsCollapsed)
                {
                    scrollviewer = Template.FindName("popup_ScrollViewer", this) as ScrollViewer;
                    if (m_popupContentHost != null)
                    {
                        m_popupContentHost.Content = SelectedContent;
                    }
                }
                else
                {
                    scrollviewer = Template.FindName("ScrollViewer", this) as ScrollViewer;
                    if (m_visualContent != null)
                    {
                        m_visualContent.Content = SelectedContent;
                    }
                }
                var item = SelectedTab as GroupBarItem;
                if (scrollviewer != null && item!=null)
                {
                    if (item.ReadLocalValue(ScrollViewer.VerticalScrollBarVisibilityProperty) != DependencyProperty.UnsetValue)
                    {
                        scrollviewer.ClearValue(ScrollViewer.VerticalScrollBarVisibilityProperty);
                        scrollviewer.VerticalScrollBarVisibility = ScrollViewer.GetVerticalScrollBarVisibility(item);
                    }
                    if (item.ReadLocalValue(ScrollViewer.HorizontalScrollBarVisibilityProperty) != DependencyProperty.UnsetValue)
                    {
                        scrollviewer.ClearValue(ScrollViewer.HorizontalScrollBarVisibilityProperty);
                        scrollviewer.HorizontalScrollBarVisibility = ScrollViewer.GetHorizontalScrollBarVisibility(item);
                    }
                    
                }
            }
        }

        /// <summary>
        /// Calls OnIsCloseButtonEnabledChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnIsCloseButtonEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar instance = (GroupBar)d;
            instance.OnIsCloseButtonEnabledChanged(e);
        }

        /// <summary>
        /// Invoked when the OnIsCloseButtonEnabledChanged property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnIsCloseButtonEnabledChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Coerces the value of the <see cref="IsCloseButtonEnabled"/> property.
        /// </summary>
        /// <param name="d">The <see cref="GroupBar"/> object.</param>
        /// <param name="value">The instance containing the event data.</param>
        /// <returns>
        /// The checked value.
        /// </returns>
        public static object CoerceIsCloseButtonEnabled(DependencyObject d, object value)
        {
            GroupBar owner = d as GroupBar;
            return owner.CoerceIsCloseButtonEnabled(value);
        }

        /// <summary>
        /// Fulfils the logic before setting the value of <see cref="IsCloseButtonEnabled"/> dependency property.
        /// </summary>
        /// <param name="value">The value that should be corrected.</param>
        /// <returns>The corrected value.</returns>
        protected internal virtual object CoerceIsCloseButtonEnabled(object value)
        {
            return value;
        }

        /// <summary>
        /// Calls OnCollapsedWidthChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnCollapsedWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar instance = (GroupBar)d;
            instance.OnCollapsedWidthChanged(e);
        }

        /// <summary>
        /// Invoked when the <see cref="CollapsedWidth"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnCollapsedWidthChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsCollapsed)
            {
                if (Orientation == Orientation.Vertical)
                {
                    m_cachedWidth = Width;
                    MaxWidth = CollapsedWidth;
                    MainHost.MaxWidth = CollapsedWidth;
                }
                else
                {
                    m_cachedWidth = Height;
                    MaxHeight = CollapsedWidth;
                    MainHost.MaxWidth = CollapsedWidth;
                }
            }

            if (CollapsedWidthChanged != null)
            {
                CollapsedWidthChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnPopupResizeDirectionChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnPopupResizeDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar instance = (GroupBar)d;
            instance.OnPopupResizeDirectionChanged(e);
        }

        /// <summary>
        /// Invoked when the <see cref="PopupResizeDirection"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnPopupResizeDirectionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PopupResizeDirectionChanged != null)
            {
                PopupResizeDirectionChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnShowGripperChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnShowGripperChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar instance = (GroupBar)d;
            instance.OnShowGripperChanged(e);
        }

        /// <summary>
        /// Invoked when the <see cref="ShowGripper"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnShowGripperChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ShowGripperChanged != null)
            {
                ShowGripperChanged(this, e);
            }
        }

        /// <summary>
        /// Called when NavigationPopupSize changed.
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnNavigationPopupSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar instance = (GroupBar)d;
            instance.OnNavigationPopupSizeChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:NavigationPopupSizeChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnNavigationPopupSizeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (NavigationPopupSizeChanged != null)
            {
                NavigationPopupSizeChanged(this, e);
            }

            Size newSize = (Size)e.NewValue;

            if (m_contentPopup != null)
            {
                m_contentPopup.Width = newSize.Width;
                m_contentPopup.Height = newSize.Height;
            }
        }

        /// <summary>
        /// Called when CornerRadius changed.
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar instance = (GroupBar)d;
            instance.OnCornerRadiusChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:CornerRadiusChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCornerRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CornerRadiusChanged != null)
            {
                CornerRadiusChanged(this, e);
            }

            CornerRadius newValue = (CornerRadius)e.NewValue;
            double margin = (newValue.TopLeft + newValue.TopRight) / 2;

            ScrollViewerMargin = new Thickness(0, margin, 0, margin);
            HeaderTopCornerRadius = new CornerRadius(newValue.TopLeft, newValue.TopRight, 0, 0);
            ToolbarBottomCornerRadius = new CornerRadius(0, 0, newValue.BottomRight, newValue.BottomLeft);

            double menuMargin = newValue.BottomRight - (newValue.BottomRight / 5);
            ToolbarMenuMarginRight = new Thickness(menuMargin, 0, 0, 0);
            HeaderMarginRight = new Thickness(0, 0, menuMargin, 0);
        }

        /// <summary>
        /// Called when [stack item host visibility changed].
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnStackItemHostVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar instance = (GroupBar)d;
            instance.OnStackItemHostVisibilityChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:StackItemHostVisibilityChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnStackItemHostVisibilityChanged(DependencyPropertyChangedEventArgs e)
        {
            if (StackItemHostVisibilityChanged != null)
            {
                StackItemHostVisibilityChanged(this, e);
            }

            Visibility newValue = (Visibility)e.NewValue;

            if (Toolbar != null && Toolbar.Visibility != newValue)
            {
                Toolbar.Visibility = newValue;
            }

            if (m_stackItemsHost != null)
            {
                CalculateItemContentLength(Toolbar.ActualHeight);
            }
        }

        /// <summary>
        /// Calculates ItemContentLength in the stack mode.
        /// </summary>
        /// <param name="toolbarHeight">Height of the toolbar.</param>
        internal void CalculateItemContentLength(double toolbarHeight)
        {
            double itemsHeaderHeight = VisibleItemsCount * ItemHeaderHeight;

            ItemContentLength = ActualHeight - (itemsHeaderHeight + HeaderHeight + Splitter.ActualHeight + toolbarHeight);
        }

        /// <summary>
        /// Invoked when the <see cref="FlowDirectionChanged"/> event is raised.
        /// </summary>
        /// <param name="sender">the change occurs on</param>
        /// <param name="e">contains the event data</param>
        private void GroupBar_FlowDirectionChanged(object sender, FlowDirectionChangedEventArgs e)
        {
            if (ContextMenu != null)
            {
                ContextMenu.FlowDirection = FlowDirection;
            }

            UpdateMenuFlowDirection(this);

            if (Toolbar != null)
            {
                Toolbar.MainMenu.Click += new RoutedEventHandler(MainMenu_Click);
            }
        }

        /// <summary>
        /// Invoked when the <see cref="MainMenu_Click"/> event is raised.
        /// </summary>
        /// <param name="sender">the change occurs on</param>
        /// <param name="e">contains the event data</param>
        private void MainMenu_Click(object sender, RoutedEventArgs e)
        {
            if (Toolbar != null)
            {
                Toolbar.MainMenu.FlowDirection = FlowDirection;
            }
        }

        /// <summary>
        /// Called when [group bar header style changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnGroupBarHeaderStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GroupBar instance = (GroupBar)d;
            instance.OnGroupBarHeaderStyleChanged(e);
        }

     
        /// <summary>
        /// Raises the <see cref="E:GroupBarHeaderStyleChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnGroupBarHeaderStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (GroupBarHeaderStyleChanged != null)
            {
                GroupBarHeaderStyleChanged(this, e);
            }
        }


        /// <summary>
        /// Called when [navigation menu closing executed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        internal void OnNavigationMenuClosingExecuted(object sender, CancelEventArgs e)
        {
            if (NavigationMenuClosing != null)
            {
                NavigationMenuClosing(sender, e);
            }
        }

        /// <summary>
        /// Called when the Navigation Menu Opening.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        internal void OnNavigationMenuOpeningExecuted(object sender, EventArgs e)
        {
            if (NavigationMenuOpening != null)
            {
                NavigationMenuOpening(sender, e);
            }
        }

        /// <summary>
        /// Called when the Navigation options menu item clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        internal void OnNavigationOptionsMenuItemClickExecuted(object sender, RoutedEventArgs e)
        {
            if (NavigationOptionsMenuItemClick != null)
            {
                NavigationOptionsMenuItemClick(sender, e);
            }
        }

        /// <summary>
        /// Called when [context menu item click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Tools.GroupBarContextMenuItemEventArgs"/> instance containing the event data.</param>
        private static void OnContextMenuItemClick(object sender, GroupBarContextMenuItemEventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            GroupBar bar = null;

            if (menu.PlacementTarget is GroupBar)
            {
                bar = menu.PlacementTarget as GroupBar;
            }
            else if (menu.PlacementTarget is GroupBarItem)
            {
                bar = ((GroupBarItem)menu.PlacementTarget).LogicalParent;
            }
            else if (menu.PlacementTarget is GroupView)
            {
                bar = ((GroupView)menu.PlacementTarget).LogicalParent.LogicalParent;
            }

            if (bar.ContextMenuItemClick != null)
            {
                bar.ContextMenuItemClick(sender, e);
            }
        }

        #endregion

        #region IDisposable members

        public void Dispose()
        {
            //this.Items.Clear();
            if (m_popupResizeBorder != null)
            {
                m_popupResizeBorder.MouseDown -= new MouseButtonEventHandler(PopupResizeBorderMouseDown);
                m_popupResizeBorder.MouseMove -= new MouseEventHandler(PopupResizeBorderMouseMove);
                m_popupResizeBorder.MouseUp -= new MouseButtonEventHandler(PopupResizeBorderMouseUp);
            }
            if (m_contentPopup != null)
            {
                m_contentPopup.Opened -= new EventHandler(PopupOpened);
                m_contentPopup.Closed -= new EventHandler(PopupClosed);
                m_contentPopup.IsKeyboardFocusWithinChanged -= new DependencyPropertyChangedEventHandler(PopupIsKeyboardFocusWithinChanged);
            }
            if (m_popupContentHost != null)
            {
                m_popupContentHost.KeyDown -= new KeyEventHandler(PopupContentHostKeyDown);
            }
            FlowDirectionChanged -= new FlowDirectionChangedEventHandler(GroupBar_FlowDirectionChanged);
            if (ContextMenu != null)
            {
                ContextMenu.Opened -= new RoutedEventHandler(ContextMenu_Opened);
                ContextMenu.LostMouseCapture -= new MouseEventHandler(ContextMenu_LostMouseCapture);
            }
        }

        #endregion

        #region Dependency properties
        /////// <summary>
        /////// Identifies the <see cref="DefaultBackground"/> dependency property.
        /////// </summary>
        ////internal static readonly DependencyProperty DefaultBackgroundProperty = DependencyProperty.Register("DefaultBackground", typeof(Brush), typeof(GroupBar), new FrameworkPropertyMetadata(Brushes.Transparent));

        /////// <summary>
        /////// Identifies the <see cref="DefaultBorderBrush"/> dependency property.
        /////// </summary>
        ////internal static readonly DependencyProperty DefaultBorderBrushProperty = DependencyProperty.Register("DefaultBorderBrush", typeof(Brush), typeof(GroupBar), new FrameworkPropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Identifies <see cref="VisualStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VisualStyleProperty = DependencyProperty.Register("VisualStyle", typeof(VisualStyle), typeof(GroupBar), new FrameworkPropertyMetadata(VisualStyle.Default, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnVisualStyleChanged)));

        /// <summary>
        /// Identifies <see cref="VisualMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VisualModeProperty = DependencyProperty.RegisterAttached("VisualMode", typeof(VisualMode), typeof(GroupBar), new FrameworkPropertyMetadata(VisualMode.Default, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnVisualModeChanged)));

        /// <summary>
        /// Identifies <see cref="CustomAnimations"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomAnimationsProperty = DependencyProperty.Register("CustomAnimations", typeof(CustomAnimationsCollection), typeof(GroupBar), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies <see cref="SelectedContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedContentProperty = DependencyProperty.Register("SelectedContent", typeof(object), typeof(GroupBar), new UIPropertyMetadata(OnSelectedContentChanged));

        /// <summary>
        /// Identifies <see cref="SelectedHeader"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedHeaderProperty = DependencyProperty.Register("SelectedHeader", typeof(object), typeof(GroupBar), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies <see cref="ItemHeaderHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemHeaderHeightProperty = DependencyProperty.Register("ItemHeaderHeight", typeof(double), typeof(GroupBar), new UIPropertyMetadata(DEF_HEADER_HEIGHT, OnItemsHeaderHeightChanged));

        /// <summary>
        /// Identifies <see cref="NavigationOptionsMenuItemHeader"/> dependency property.
        /// </summary>
        private static readonly DependencyProperty NavigationOptionsMenuItemHeaderProperty = DependencyProperty.Register("NavigationOptionsMenuItemHeader", typeof(Object), typeof(GroupBar), new PropertyMetadata("Options"));

        /// <summary>
        /// Identifies <see cref="DynamicResizing"/> dependency property.
        /// </summary>
        private static readonly DependencyProperty DynamicResizingProperty = DependencyProperty.Register("DynamicResizing", typeof(bool), typeof(GroupBar), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies <see cref="Orientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(GroupBar), new UIPropertyMetadata(DEF_DEFAULT_ORIENTATION, OnOrientationChanged));

        /// <summary>
        /// Identifies <see cref="TextAlignmentProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register("TextAlignment", typeof(HorizontalAlignment), typeof(GroupBar), new UIPropertyMetadata(HorizontalAlignment.Left));

        /// <summary>
        /// Identifies <see cref="IsEnabledContextMenu"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsEnabledContextMenuProperty = DependencyProperty.RegisterAttached("IsEnabledContextMenu", typeof(bool), typeof(GroupBar), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Identifies <see cref="ItemContentLength"/> dependency property.
        /// </summary>
        public static DependencyProperty ItemContentLengthProperty = DependencyProperty.Register("ItemContentLength", typeof(double), typeof(GroupBar), new UIPropertyMetadata(DEF_ITEMCONTENT_LENGTH, OnItemContentLengthChanged, OnItemContentLengthCoerce));

        /// <summary>
        /// Identifies <see cref="HeaderHeight"/> dependency property.
        /// </summary>
        public static DependencyProperty HeaderHeightProperty = DependencyProperty.Register("HeaderHeight", typeof(double), typeof(GroupBar), new UIPropertyMetadata(DEF_HEADER_HEIGHT, OnHeaderHeightChanged, OnHeaderHeightCoerce));

        /// <summary>
        /// Identifies <see cref="StackItemHostHeight"/> dependency property.
        /// </summary>
        public static DependencyProperty StackItemHostHeightProperty = DependencyProperty.Register("StackItemHostHeight", typeof(double), typeof(GroupBar), new UIPropertyMetadata(DEF_TOOLBAR_HEIGHT, OnStackItemHostHeightChanged, OnStackItemHostHeightCoerce));

        /// <summary>
        /// Identifies <see cref="RotationAngle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RotationAngleProperty = DependencyProperty.Register("RotationAngle", typeof(double), typeof(GroupBar), new UIPropertyMetadata(0d));

        /// <summary>
        /// Identifies <see cref="ContentRotationAngle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentRotationAngleProperty = DependencyProperty.Register("ContentRotationAngle", typeof(double), typeof(GroupBar), new UIPropertyMetadata(0d));

        /// <summary>
        /// Identifies <see cref="SelectedTab"/> dependency property key.
        /// </summary>
        protected static readonly DependencyPropertyKey SelectedTabPropertyKey = DependencyProperty.RegisterReadOnly("SelectedTab", typeof(GroupBarItem), typeof(GroupBar), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSelectedTabChanged)));

        /// <summary>
        ///  Identifies <see cref="SelectedTab"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedTabProperty = SelectedTabPropertyKey.DependencyProperty;

        /// <summary>
        /// Identifies <see cref="SelectedItem"/> dependency property key.
        /// </summary>
        protected static readonly DependencyPropertyKey SelectedItemPropertyKey = DependencyProperty.RegisterReadOnly("SelectedItem", typeof(GroupViewItem), typeof(GroupBar), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSelectedItemChanged)));

        /// <summary>
        /// Identifies <see cref="SelectedItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty = SelectedItemPropertyKey.DependencyProperty;

        /// <summary>
        /// Identifies <see cref="SelectedObject"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedObjectProperty = DependencyProperty.Register("SelectedObject", typeof(object), typeof(GroupBar), new FrameworkPropertyMetadata(null, OnSelectedObjectChanged));

        /// <summary>
        /// Identifies <see cref="GroupBarItemCursorType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupBarItemCursorTypeProperty = DependencyProperty.Register("GroupBarItemCursorType", typeof(ItemCursorType), typeof(GroupBar), new UIPropertyMetadata(ItemCursorType.Default));

        /// <summary>
        /// Identifies <see cref="DragMarkerBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DragMarkerBrushProperty = DependencyProperty.Register("DragMarkerBrush", typeof(Brush), typeof(GroupBar), new UIPropertyMetadata(Brushes.Black));

        /// <summary>
        /// Identifies <see cref="DefaultItemImage"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultItemImageProperty = DependencyProperty.Register("DefaultItemImage", typeof(ImageSource), typeof(GroupBar));

        /// <summary>
        /// Identifies <see cref="VerticalOrientationStoryboard"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalOrientationStoryboardProperty = DependencyProperty.Register("VerticalOrientationStoryboard", typeof(Storyboard), typeof(GroupBar), new UIPropertyMetadata(null, OnVerticalOrientationStoryboardChanged));

        /// <summary>
        /// Identifies <see cref="HorizontalOrientationStoryboard"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalOrientationStoryboardProperty = DependencyProperty.Register("HorizontalOrientationStoryboard", typeof(Storyboard), typeof(GroupBar), new UIPropertyMetadata(null, OnHorizontalOrientationStoryboardChanged));

        /// <summary>
        /// Identifies <see cref="OrientationChangedContentStoryboard"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationChangedContentStoryboardProperty = DependencyProperty.Register("OrientationChangedContentStoryboard", typeof(Storyboard), typeof(GroupBar), new UIPropertyMetadata(null, OnOrientationChangedContentStoryboardChanged));

        /// <summary>
        /// Identifies the <see cref="SaveOriginalState"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SaveOriginalStateProperty = DependencyProperty.Register("SaveOriginalState", typeof(bool), typeof(GroupBar), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnSaveOriginalStateChanged)));

        /// <summary>
        /// Identifies the <see cref="CollapseButtonBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CollapseButtonBackgroundProperty = DependencyProperty.Register("CollapseButtonBackground", typeof(Brush), typeof(GroupBar), new FrameworkPropertyMetadata(Brushes.Transparent, new PropertyChangedCallback(OnCollapseButtonBackgroundChanged)));

        /// <summary>
        /// Identifies the <see cref="CollapseButtonMouseOverBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CollapseButtonMouseOverBackgroundProperty = DependencyProperty.Register("CollapseButtonMouseOverBackground", typeof(Brush), typeof(GroupBar), new FrameworkPropertyMetadata(Brushes.Transparent, new PropertyChangedCallback(OnCollapseButtonMouseOverBackgroundChanged)));

        /// <summary>
        /// Identifies the <see cref="CollapseButtonTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CollapseButtonTemplateProperty = DependencyProperty.Register("CollapseButtonTemplate", typeof(ControlTemplate), typeof(GroupBar), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnCollapseButtonTemplateChanged)));

        /// <summary>
        /// Identifies the <see cref="AnimationType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimationTypeProperty = DependencyProperty.Register("AnimationType", typeof(AnimationsType), typeof(GroupBar), new FrameworkPropertyMetadata(AnimationsType.Fade, new PropertyChangedCallback(OnAnimationTypeChanged)));

        /// <summary>
        /// Identifies the <see cref="AnimationSpeed"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimationSpeedProperty = DependencyProperty.Register("AnimationSpeed", typeof(int), typeof(GroupBar), new FrameworkPropertyMetadata(300, new PropertyChangedCallback(OnAnimationSpeedChanged)));

        /// <summary>
        /// Identifies the <see cref="CollapseButtonToolTip"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CollapseButtonToolTipProperty = DependencyProperty.Register("CollapseButtonToolTip", typeof(object), typeof(GroupBar), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnCollapseButtonToolTipChanged)));

        /// <summary>
        /// Identifies the <see cref="ExpandButtonToolTip"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpandButtonToolTipProperty = DependencyProperty.Register("ExpandButtonToolTip", typeof(object), typeof(GroupBar), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnExpandButtonToolTipChanged)));

        /// <summary>
        /// Identifies the <see cref="AllowCollapse"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowCollapseProperty = DependencyProperty.Register("AllowCollapse", typeof(bool), typeof(GroupBar), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnAllowCollapseChanged), CoerceAllowCollapse));

        /// <summary>
        /// Identifies the <see cref="IsCollapsed"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCollapsedProperty = DependencyProperty.Register("IsCollapsed", typeof(bool), typeof(GroupBar), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsCollapsedChanged), CoerceIsCollapsed));

        /// <summary>
        /// Identifies the <see cref="IsDropDownOpen"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(GroupBar), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsDropDownOpenChanged), CoerceIsDropDownOpen));

        /// <summary>
        /// Identifies the <see cref="IsToolBarEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsToolBarEnabledProperty = DependencyProperty.Register("IsToolBarEnabled", typeof(bool), typeof(GroupBar), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnIsToolBarEnabledChanged), CoerceIsToolBarEnabled));

        /// <summary>
        /// Identifies the <see cref="IsCloseButtonEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCloseButtonEnabledProperty = DependencyProperty.Register("IsCloseButtonEnabled", typeof(bool), typeof(GroupBar), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsCloseButtonEnabledChanged), CoerceIsCloseButtonEnabled));

        /// <summary>
        /// Identifies the <see cref="CollapsedWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CollapsedWidthProperty = DependencyProperty.Register("CollapsedWidth", typeof(double), typeof(GroupBar), new FrameworkPropertyMetadata(C_defaultCollapsedWidth, new PropertyChangedCallback(OnCollapsedWidthChanged)));

        /// <summary>
        /// Identifies the <see cref="PopupResizeDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PopupResizeDirectionProperty = DependencyProperty.Register("PopupResizeDirection", typeof(PopupResizeDirection), typeof(GroupBar), new FrameworkPropertyMetadata(PopupResizeDirection.Both, new PropertyChangedCallback(OnPopupResizeDirectionChanged)));

        /// <summary>
        /// Identifies the <see cref="ShowGripper"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowGripperProperty = DependencyProperty.Register("ShowGripper", typeof(bool), typeof(GroupBar), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnShowGripperChanged)));

        /// <summary>
        /// Identifies the StackItemHostVisibility dependency property. Used only in StackMode.
        /// </summary>
        public static readonly DependencyProperty StackItemHostVisibilityProperty = DependencyProperty.Register("StackItemHostVisibility", typeof(Visibility), typeof(GroupBar), new FrameworkPropertyMetadata(Visibility.Visible, new PropertyChangedCallback(OnStackItemHostVisibilityChanged)));

        /// <summary>
        /// Identifies <see cref="NavigationPaneText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NavigationPaneTextProperty = DependencyProperty.Register("NavigationPaneText", typeof(string), typeof(GroupBar), new FrameworkPropertyMetadata("Navigation Pane", FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies <see cref="NavigationPopupSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NavigationPopupSizeProperty = DependencyProperty.Register("NavigationPopupSize", typeof(Size), typeof(GroupBar), new FrameworkPropertyMetadata(Size.Empty, new PropertyChangedCallback(OnNavigationPopupSizeChanged)));

        /// <summary>
        /// Identifies <see cref="CornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(GroupBar), new FrameworkPropertyMetadata(new CornerRadius(0, 0, 0, 0), new PropertyChangedCallback(OnCornerRadiusChanged)));

        /// <summary>
        /// Identifies <see cref="ScrollViewerMargin"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty ScrollViewerMarginProperty = DependencyProperty.Register("ScrollViewerMargin", typeof(Thickness), typeof(GroupBar), new FrameworkPropertyMetadata(new Thickness(0, 0, 0, 0)));

        /// <summary>
        /// Identifies <see cref="HeaderTopCornerRadius"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty HeaderTopCornerRadiusProperty = DependencyProperty.Register("HeaderTopCornerRadius", typeof(CornerRadius), typeof(GroupBar), new FrameworkPropertyMetadata(new CornerRadius(0, 0, 0, 0)));

        /// <summary>
        /// Identifies <see cref="ToolbarBottomCornerRadius"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty ToolbarBottomCornerRadiusProperty = DependencyProperty.Register("ToolbarBottomCornerRadius", typeof(CornerRadius), typeof(GroupBar), new FrameworkPropertyMetadata(new CornerRadius(0, 0, 0, 0)));

        /// <summary>
        /// Identifies <see cref="ToolbarMenuMarginRight"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty ToolbarMenuMarginRightProperty = DependencyProperty.Register("ToolbarMenuMarginRight", typeof(Thickness), typeof(GroupBar), new FrameworkPropertyMetadata(new Thickness(0, 0, 0, 0)));

        /// <summary>
        /// Identifies <see cref="HeaderMarginRight"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty HeaderMarginRightProperty = DependencyProperty.Register("HeaderMarginRight", typeof(Thickness), typeof(GroupBar), new FrameworkPropertyMetadata(new Thickness(0, 0, 0, 0)));

        /// <summary>
        /// Dependency property which has group bar header style
        /// </summary>
        public static readonly DependencyProperty GroupBarHeaderStyleProperty = DependencyProperty.Register("GroupBarHeaderStyle", typeof(Style), typeof(GroupBar), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.OverridesInheritanceBehavior, new PropertyChangedCallback(OnGroupBarHeaderStyleChanged)));


        /// <summary>
        /// Dependency property which has Dragitem visibility
        /// </summary>
        public static readonly DependencyProperty DragItemVisibilityProperty = DependencyProperty.Register("DragItemVisibility", typeof(Visibility), typeof(GroupBar), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Dependency property which has item header template
        /// </summary>
        private static readonly DependencyProperty ItemHeaderTemplateProperty = DependencyProperty.Register("ItemHeaderTemplate", typeof(DataTemplate), typeof(GroupBar));


        public static readonly DependencyProperty FitContentProperty = DependencyProperty.Register("FitContent", typeof(bool), typeof(GroupBar), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the FitContent.
        /// </summary>
        /// <value>The FitContent option.</value>
        [CLSCompliant(false)]
        public bool FitContent 
        {
            get
            {
                return (bool)GetValue(FitContentProperty);
            }
            set
            {
                SetValue(FitContentProperty, value);

            }

        }

        /// <summary>
        /// Gets or sets the drag item visibility.
        /// </summary>
        /// <value>The drag item visibility.</value>
        public Visibility DragItemVisibility
        {
            get
            {
                return (Visibility)GetValue(DragItemVisibilityProperty);
            }
            set
            {
                SetValue(DragItemVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the item header template.
        /// </summary>
        /// <value>The item header template.</value>
        public DataTemplate ItemHeaderTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ItemHeaderTemplateProperty);
            }

            set
            {
                SetValue(ItemHeaderTemplateProperty, value);
            }
        }

       

        #endregion
    }
}
