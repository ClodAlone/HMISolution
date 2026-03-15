// <copyright file="TreeViewAdv.cs" company="Syncfusion">
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
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Threading;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;

#endregion file using

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a control that displays hierarchical data
    /// in a tree structure that has items that can expand and collapse.
    /// </summary>
    /// <list type="table">
    /// <listheader>
    /// <term>Help Page</term>
    /// <description>Syntax</description>
    /// </listheader>
    /// <example>
    /// <list type="table">
    /// <listheader>
    /// <description>C#</description>
    /// </listheader>
    /// <example><code>public partial class TreeViewAdv : <see cref="ItemsControl"/></code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example>
    /// <code language="XAML">
    /// <![CDATA[
    /// <local:TreeViewAdv>
    /// <see cref="ItemsControl.Items" >
    /// <local:TreeViewAdv/>
    /// ]]>
    /// </code>
    /// </example>
    /// </list>
    /// </example>
    /// </list>
    /// <remarks>
    /// Content Model: TreeViewAdv is an <see cref="ItemsControl"/>
    /// and has two content properties: <see cref="System.Windows.Controls.ItemsControl.Items"/> and <see cref="System.Windows.Controls.ItemsControl.ItemsSourceProperty"/>.
    /// <para/>The contents of a TreeViewAdv are <see cref="TreeViewItemAdv"/>
    /// controls that can contain rich content, such as <see cref="Button"/>
    /// and <see cref="Image"/> controls. A TreeViewItemAdv can contain one
    /// or more TreeViewItemAdv objects as its descendants.
    /// A TreeViewAdv is defined as a hierarchy of TreeViewItemAdv objects.
    /// <para/>To create a TreeViewAdv using C# you can use the TreeViewAdv method.
    /// <para/>A TreeViewAdv supports Windows themes (Default, Silver, Metallic, Zune, Royale and Aero)
    /// and skins (Office2003, Office2007Blue, Office2007Black, Office2007Silver and Blend).
    /// <para/>A TreeViewAdv supports multiple selection. <see cref="SelectedItems"/> property
    /// contains the list of all selected items.
    /// </remarks>
    /// <example>
    /// <para/>This example shows how to create a TreeViewAdv in XAML.
    /// <code language="XAML">
    /// <![CDATA[
    /// <Window x:Class="WpfApplication1.Window1" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
    /// Title="Window1" Height="300" Width="300">
    /// <Grid>
    /// <local:TreeViewAdv>
    ///     <local:TreeViewItemAdv Header="Employee1">
    ///         <local:TreeViewItemAdv Header="Jesper"/>
    ///         <local:TreeViewItemAdv Header="Aaberg"/>
    ///         <local:TreeViewItemAdv Header="12345"/>
    ///     </local:TreeViewItemAdv>
    ///     <local:TreeViewItemAdv Header="Employee2">
    ///         <local:TreeViewItemAdv Header="Dominik"/>
    ///         <local:TreeViewItemAdv Header="Paiha"/>
    ///         <local:TreeViewItemAdv Header="98765"/>
    ///     </local:TreeViewItemAdv>
    /// </local:TreeViewAdv>
    /// </Grid>
    /// </Window>
    /// ]]>
    /// </code>
    /// </example>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(true)]
#endif
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
  Type = typeof(TreeViewAdv), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TreeViewAdv/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(TreeViewAdv), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TreeViewAdv/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(TreeViewAdv), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TreeViewAdv/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
  Type = typeof(TreeViewAdv), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TreeViewAdv/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(TreeViewAdv), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TreeViewAdv/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(TreeViewAdv), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TreeViewAdv/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(TreeViewAdv), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TreeViewAdv/Themes/Office2003Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(TreeViewAdv), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TreeViewAdv/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.SyncOrange,
    Type = typeof(TreeViewAdv), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TreeViewAdv/Themes/SyncOrangeStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyRed,
    Type = typeof(TreeViewAdv), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TreeViewAdv/Themes/ShinyRedStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.ShinyBlue,
    Type = typeof(TreeViewAdv), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TreeViewAdv/Themes/ShinyBlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(TreeViewAdv), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TreeViewAdv/Themes/generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
    Type = typeof(TreeViewAdv), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TreeViewAdv/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
    Type = typeof(TreeViewAdv), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TreeViewAdv/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
    Type = typeof(TreeViewAdv), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/TreeViewAdv/Themes/TransparentStyle.xaml")]

    //[StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(TreeViewItemAdv))]
    public class TreeViewAdv : ItemsControl, IItemsPanelRef, IItemContainer
    {
        /// <summary>
        /// Gets or sets Drag mode whether by Mouse Left button , Right button or Both
        /// the <see cref="DragMode"/> enumeration. The Drag mode is used to
        /// choose the mouse button to perform drag operation
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="DragMode"/>
        /// Its Default drag mode is LeftButton.
        /// </value>
        /// <seealso cref="DragMode"/>
        public DragMode DragMode
        {
            get { return (DragMode)GetValue(DragModeProperty); }
            set { SetValue(DragModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DragMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DragModeProperty =
            DependencyProperty.Register("DragMode", typeof(DragMode), typeof(TreeViewAdv), new UIPropertyMetadata(DragMode.LeftButton));

        internal bool isCalledByChildMeasure = false;

        #region Constants

        /// <summary>
        /// Default property name for sorting.
        /// </summary>
        private const string C_sortingPropertyName = "Header";

        internal bool isLinearItemsChanged = false;
        internal bool isPageDown = false;
        internal bool isSourceChanged = false;
        private int addeditemscount;
        private int removeditemscount;
        private bool issingleselection = false;
        internal ObservableCollection<object> oldselectedItems = new ObservableCollection<object>();
        internal int count;
        internal int drag_count;
        private bool isSet = false;
        private int tempTargetIndex = 0;
        private int tempCurrentIndex = 0;
        private bool isInternalItem = false;
        private bool isParentItem = false;
        private double treeitemheight = 0.0;
        private bool isRightMouseButtonDown = false;

        /// <summary>
        /// Name of the ScrolView from template.
        /// </summary>
        private const string C_scrollViewName = "PART_ScrollViwer";

        /// <summary>
        /// Name of the ItemsPresenter from template.
        /// </summary>
        private const string C_itemsPresenterName = "PART_ItemPresenter";

        /// <summary>
        /// Name of the HeaderRowPresenter from template.
        /// </summary>
        private const string C_headerRowPresenter = "PART_HeaderRowPresenter";

        /// <summary>
        /// Opacity for dragging element.
        /// </summary>
        private const double C_dragOpacity = 0.7;

        /// <summary>
        /// Vertical offset for hit test.
        /// </summary>
        private const int C_hittestOffset = 7;

        /// <summary>
        /// Name of the Default visual style.
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
        /// Name of the Metro visual style.
        /// </summary>
        private const string C_nameMetroVisualStyle = "Metro";

        /// <summary>
        /// Name of the Transparent visual style.
        /// </summary>
        private const string C_nameTransparentVisualStyle = "Transparent";

        /// <summary>
        /// Name of the ShinyRed visual style.
        /// </summary>
        private const string C_nameShinyRedVisualStyle = "ShinyRed";

        /// <summary>
        /// Name of the ShinyBlue visual style.
        /// </summary>
        private const string C_nameShinyBlueVisualStyle = "ShinyBlue";

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
        /// Name of the Office2010Silver visual style.
        /// </summary>
        private const string C_nameOffice2010SilverVisualStyle = "Office2010Silver";

        /// <summary>
        /// Name of the Office2010Black visual style.
        /// </summary>
        private const string C_nameOffice2010BlackVisualStyle = "Office2010Black";

        /// <summary>
        /// Name of the Office2010Blue visual style.
        /// </summary>
        private const string C_nameOffice2010BlueVisualStyle = "Office2010Blue";

        /// <summary>
        /// Name of the default theme.
        /// </summary>
        private const string C_nameTheme = "Default";

        /// <summary>
        /// internal variable which has space between two rectange
        /// </summary>
        private double spaceBetweenTwoRect = 20;

        /// <summary>
        /// internal variable which has scroll bounds
        /// </summary>
        private Rect scrollBounds = Rect.Empty;

        /// <summary>
        /// internal variable which has  latest position
        /// </summary>
        private Point latestMousePossition = new Point(0, 0);

        /// <summary>
        /// internal variable which has dispatcher time
        /// </summary>
        private DispatcherTimer autoScrollerTimer = null;

        /// <summary>
        /// stores the current expanded item
        /// </summary>
        internal TreeViewItemAdv m_expandeditem = null;

        internal static TreeViewItemAdv ParentContainer = null;

        /// <summary>
        /// stores the item to be viewed
        /// </summary>
        internal TreeViewItemAdv m_viewableitem = null;

        /// <summary>
        /// Value indicates bringintoviewhandled status
        /// </summary>
        internal bool bringintoviewstatus = false;

        /// <summary>
        /// to determine dropped item cancel status
        /// </summary>
        private bool dropcancelstatus = false;

        /// <summary>
        /// to determine whether treeviewadv expand by doubleclick
        /// </summary>
        protected internal bool isdoubleclick = false;

        /// <summary>
        /// Presents width after resizing
        /// </summary>
        internal double ResizingWidth;

        internal TreeViewItemAdv tempDraggedItem = null;

        internal TreeViewAdv DraggedTreeView = null;

        internal TreeViewAdv DropTreeView = null;

        /// <summary>
        /// stores the treeviewitems
        /// </summary>
        internal List<TreeViewItemAdv> m_treeviewadv = new List<TreeViewItemAdv>();

        /// <summary>
        /// stores the first TreeViewItemAdvHeight
        /// </summary>
        internal double m_treeviewitemadvHeight = 20.0;

        /// <summary>
        /// To enable the shortcut keys for drag operations
        /// </summary>
        internal bool m_dragshortcutkeysenabled = false;

        /// <summary>
        /// stores the itemsource object
        /// </summary>
        internal List<Object> m_itemObject = new List<object>();

        /// <summary>
        /// stores the sortdirection new value
        /// </summary>
        internal SortDirection newvalue = SortDirection.None;

        /// <summary>
        /// stores the sortdirection old value
        /// </summary>
        internal SortDirection oldvalue = SortDirection.None;

        /// <summary>
        /// stores drag drop effects of treeviewitem
        /// </summary>
        internal static TreeViewItemAdvDragDropEffects dragdropeffects = TreeViewItemAdvDragDropEffects.None;

        /// <summary>
        /// stores drag drop effects of treeviewitem
        /// </summary>
        internal TreeViewItemAdvDragDropEffects changeddragdropeffects = TreeViewItemAdvDragDropEffects.None;

        internal TreeViewItemAdvDragDropEffects argsdragdropeffetschanged = TreeViewItemAdvDragDropEffects.None;

        internal Dictionary<int, double> cell_width = new Dictionary<int, double>();

        internal Dictionary<double, TreeViewRowPresenter> row = new Dictionary<double, TreeViewRowPresenter>();

        internal Dictionary<int, double> rowWidth = new Dictionary<int, double>();

        internal ObservableCollection<TreeViewColumn> columnsState = new ObservableCollection<TreeViewColumn>();

        internal ObservableCollection<TreeViewRowPresenter> rowPresenterCollection = new ObservableCollection<TreeViewRowPresenter>();

        internal ObservableCollection<TreeViewHeaderRowPresenter> rowHeaderPresenterCollection = new ObservableCollection<TreeViewHeaderRowPresenter>();

        internal TreeViewRowPresenter rowPresenter;

        internal bool AllowUpdate = true;

        internal bool AllowCalculate = false;

        internal bool allowArrange = false;

        internal bool Flag_Width = false;

        internal bool externalSelect = true;

        internal bool allowSelectedTreeItem = true;

        internal bool isSelectedFromPrepareContainer = false;

        #endregion Constants

        #region Members

        /// <summary>
        /// finds the difference between position of the dragover control and dragover mouse point position
        /// </summary>
        internal double m_pointovercontroldiff = 0;

        internal TreeViewItemAdv dragOverItem;

        private TreeViewItemAdv temptargetItem;

        private TreeViewItemAdv tempCurrentNode;

        private TreeViewItemAdv tempTargetNode;

        private bool isSelectItemsChanged = false;

        private bool isTargetNodeParentNotVisible = false;

        private ItemsControl TempParentItemsControl;

        private int startindex = -1;

        internal double scalefractionx = 1;

        internal double scalefractiony = 1;

        private int internalcurrentindex = -1;

        private TreeViewItemAdv internalcurrentnode = null;

        /// <summary>
        /// stores the panel from TreeViewAdv
        /// </summary>
        internal VirtualizingPanel m_virtualizingpanel = null;

        internal TreeViewAdvVirtualizingPanel m_treeviewadvVirtualizingPanel = null;

        private List<TreeModel> modelItems;

        internal double tempViewPortHeight = 0.0;

        /// <summary>
        /// stores the scrollinfo of TreeViewAdv
        /// </summary>
        internal IScrollInfo m_scrollinfo;

        internal bool m_loaded = false;

        internal bool m_itemsselectedfromlinearlist = false;

        internal bool m_itemschanged = false;

        internal TreeViewItemAdv DragTreeItem = null;

        internal bool isDragSelectedItem = false;

        internal bool m_itemsexpanded = false;

        /// <summary>
        /// Value indicates the collection for the expanded items.
        /// </summary>
        internal ObservableCollection<TreeViewItemAdv> ExpandedItems = new ObservableCollection<TreeViewItemAdv>();

        /// <summary>
        /// Value indicates the collection for the index of the expanded items.
        /// </summary>
        internal ObservableCollection<int> ExpandedItemsIndexCollection = new ObservableCollection<int>();

        /// <summary>
        /// Value indicates the collection for the expanded treeview items along with their index.
        /// </summary>
        internal Dictionary<int, TreeViewItemAdv> ExpandedTreeViewAdvItems = new Dictionary<int, TreeViewItemAdv>();

        internal FakeItemsPanel m_fakeItemsPanel = null;

        internal double availableTopOffset = 0.0;

        internal double extendHeight = 0.0;

        internal bool scrollToHome = false;

        internal bool scrollToEnd = false;

        /// <summary>
        /// Value indicates the container for the visual drag marker text.
        /// </summary>
        private Border dragvisualborder = new Border();

        /// <summary>
        /// Value indicates the dragged item index in the visual tree.
        /// </summary>
        internal int m_draggedItemindex = 0;

        /// <summary>
        /// Value indicates whether visual drag marker text.
        /// </summary>
        private TextBlock visualtext = new TextBlock();

        /// <summary>
        /// Contains the previous dropped objects
        /// </summary>
        internal ArrayList droppedobjects = new ArrayList();

        /// <summary>
        /// Value indicates whether selected item(s) is changing.
        /// </summary>
        private bool m_bIsSelectionChangeActive = false;

        /// <summary>
        /// Value indicates previous dragover TreeViewItemAdv
        /// </summary>
        internal TreeViewItemAdv m_previoustargetitem = null;

        /// <summary>
        /// Disable Drag Option when it is in editing Mode(TextBox in Visible Mode).
        /// </summary>
        internal bool IsDragInEditingState = false;

        /// <summary>
        /// Value indicates whether VisualStyle is changing.
        /// </summary>
        private bool m_bVisualStyleChanging = false;

        /// <summary>
        /// Value indicates whether start drag is available.
        /// </summary>
        private bool m_bIsAvailableStartDrag = false;

        /// <summary>
        /// Container of the selected item.
        /// </summary>
        private TreeViewItemAdv m_selectedContainer = null;

        /// <summary>
        /// Start select container. Use for Multiselect.
        /// </summary>
        internal TreeViewItemAdv m_startSelectContainer = null;

        /// <summary>
        /// Dropped select container.
        /// </summary>
        internal TreeViewItemAdv dropSelectContainer = null;

        /// <summary>
        /// Storage to containers of the selected items.
        /// </summary>
        internal TreeViewItemAdvCollection tempselectedContainers = new TreeViewItemAdvCollection();

        internal bool AllowChange = true;

        internal int m_startIndex = 0;

        internal bool InternalDrop = false;

        /// <summary>
        /// Storage to containers of the selected items.
        /// </summary>
        internal TreeViewItemAdvCollection m_selectedContainers = new TreeViewItemAdvCollection();

        /// <summary>
        /// Indicating whether allows users to select multiple nodes.
        /// </summary>
        private bool m_bIsMultiselection = false;

        /// <summary>
        /// Items that dragging.
        /// </summary>
        private TreeObjectCollection m_draggingItmes = new TreeObjectCollection();

        /// <summary>
        /// Containers for items that dragging.
        /// </summary>
        internal TreeViewItemAdvCollection m_draggingItmesContiners = new TreeViewItemAdvCollection();

        /// <summary>
        /// Parents for dragging items.
        /// </summary>
        private TreeItemsControlCollection m_draggingParentItmes = new TreeItemsControlCollection();

        /// <summary>
        /// Indicating that the item is selected by SelectedItems collection changed event handler.
        /// </summary>
        internal bool m_selectedInCollectionChanged = false;

        /// <summary>
        /// contains Item to be expanded when IsVirtualizing set to true
        /// </summary>
        internal System.Collections.ArrayList ExpandStateLinearList = new System.Collections.ArrayList();

        internal System.Collections.ArrayList AlreadyExpandedLinearList = new System.Collections.ArrayList();

        /// <summary>
        /// contains Item to be selected when IsVirtualizing set to true
        /// </summary>
        internal System.Collections.ArrayList SelectedStateLinearList = new System.Collections.ArrayList();

        /// <summary>
        /// contains selected treeviewitems
        /// </summary>
        internal List<TreeViewItemAdv> SelectedTreeViewItems;

        /// <summary>
        /// Drag start point
        /// </summary>
        private Point m_dragStart;

        /// <summary>
        /// Indicating whether MouseDown on ComplateHeader.
        /// </summary>
        private bool m_clickOnHeader = false;

        /// <summary>
        /// Indicating whether shift+ctrl selection flag
        /// </summary>
        internal bool canExecuteshift = false;

        /// <summary>
        /// Cursor position offset from dragged item.
        /// </summary>
        private Point m_dragOffset;

        /// <summary>
        /// Indicating drag process started.
        /// </summary>
        private static bool m_bIsDragging = false;

        /// <summary>
        /// Drag item popup.
        /// </summary>
        private Popup m_dragPopup = new TreeViewItemAdvDragPopup();

        /// <summary>
        /// Contains last visible item when ScrollView is visible.
        /// </summary>
        private TreeViewItemAdv m_lastVisibleItem = null;

        /// <summary>
        ///  Contains first visible item when ScrollView is visible.
        /// </summary>
        private TreeViewItemAdv m_firstVisibleItem = null;

        /// <summary>
        /// Stack for TreeViewAdv items navigation.
        /// </summary>
        private Stack<TreeViewItemAdv> m_itemsStack = new Stack<TreeViewItemAdv>();

        /// <summary>
        /// ItemsPresenter from template.
        /// </summary>
        private ItemsPresenter m_itemsHost = null;

        /// <summary>
        /// Last drag over item.
        /// </summary>
        private static TreeViewItemAdv m_lastDragOverItem = null;

        /// <summary>
        /// Value indicates whether was pressed Up or Down key.
        /// </summary>
        private bool m_bUpdownNavigation = false;

        /// <summary>
        /// TreeViewAdv under mouse when drag-and-drop in progress.
        /// </summary>
        private static TreeViewAdv m_dragOverTreeView = null;

        /// <summary>
        /// TreeViewAdv of the started drag-and-drop.
        /// </summary>
        private static TreeViewAdv m_dragStartTreeView = null;

        /// <summary>
        /// Control under mouse when drag-and-drop in progress.
        /// </summary>
        private static FrameworkElement m_dragOverElement = null;

        /// <summary>
        /// Target ItemsControl for drop when drag-and-drop.
        /// </summary>
        private static ItemsControl m_dragOverControl = null;

        /// <summary>
        /// Storage for drag data.
        /// </summary>
        private static TreeObjectCollection m_dragData = null;

        /// <summary>
        /// Indicates whether last showed position for fake item was top.
        /// </summary>
        private static bool m_lastTopPosition = true;

        /// <summary>
        /// Item which editing now.
        /// </summary>
        private TreeViewItemAdv m_editingItem = null;

        /// <summary>
        /// Container of the simulate selected item.
        /// </summary>
        private TreeViewItemAdv m_selectedFalseItem = null;

        /// <summary>
        /// Value indicates whether that item was selected by mouse click.
        /// </summary>
        internal bool m_wasSelectedByMouseClick = false;

        /// <summary>
        /// Indicating whether OnDrop event was raised.
        /// </summary>
        //private bool m_bIsDroped = false;

        /// <summary>
        /// Indicating DoubleCollection
        /// </summary>
        private static DoubleCollection dcollection;

        /// <summary>
        /// Presenter for row.
        /// </summary>
        private TreeViewHeaderRowPresenter m_headerRowPresenter = null;

        /// <summary>
        /// Flag for programatic selection
        /// </summary>
        internal bool m_ismouseSelection = false;

        /// <summary>
        /// Background worker for handling select all method
        /// </summary>
        private DispatcherTimer handleVisualConnector = null;

        /// <summary>
        /// Stores the count of Sorting changed event respective to TreeViewAdv.
        /// </summary>
        internal int SortingChangedCount = 0;

        #endregion Members

        #region Events

        /// <summary>
        /// Identifies the SelectedItemChanged routed event.
        /// </summary>
        public static readonly RoutedEvent SelectedItemChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectedItemChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<object>),
            typeof(TreeViewAdv));

        /// <summary>
        /// Identifies the <see cref="ItemGenerated"/> routed_event.
        /// </summary>
        public static readonly RoutedEvent ItemGeneratedEvent = EventManager.RegisterRoutedEvent(
            "ItemGenerated",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(TreeViewAdv));

        /// <summary>
        /// Occurs when [item generated].
        /// </summary>
        public event RoutedEventHandler ItemGenerated
        {
            add
            {
                AddHandler(TreeViewAdv.ItemGeneratedEvent, value);
            }

            remove
            {
                RemoveHandler(TreeViewAdv.ItemGeneratedEvent, value);
            }
        }

        /// <summary>
        /// Identified by the <see cref="Expanding"/> routed_event.
        /// </summary>
        public static readonly RoutedEvent ExpandingEvent = EventManager.RegisterRoutedEvent(
            "Expanding",
            RoutingStrategy.Bubble,
            typeof(ExpandingCollapsingHandler),
            typeof(TreeViewAdv));

        /// <summary>
        /// </summary>
        /// <value>
        /// Type: <see cref="SortModeChangeHandler"/>
        /// This event raise before the <see cref="TreeViewItemAdv"/> enters the Expanding mode.
        /// </value>
        /// <example>
        /// <para/>The following example shows how to assign an event handler
        /// for the <see cref="BeforeItemSort"/> event to a <see cref="TreeViewItemAdv"/>,
        /// and how to define the event handler in the code-behind.
        /// <code language="XAML">
        /// <![CDATA[
        ///     <TreeViewAdv Header="Work Days"
        ///                   Name="emp2WorkDays"
        ///                   Expanding="TreeViewAdv_Expanding">
        ///         <TreeViewItemAdv Header="Monday" />
        ///         <TreeViewItemAdv Header="Wednesday"/>
        ///     </TreeViewAdv>
        /// ]]>
        /// </code>
        /// <code language="C#">
        /// private void TreeViewAdv_Expanding( object sender, ExpandCollapseEventArgs e )
        /// {
        ///
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="ExpandCollapseHandler"/>
        public event ExpandingCollapsingHandler Expanding
        {
            add
            {
                AddHandler(TreeViewAdv.ExpandingEvent, value);
            }

            remove
            {
                RemoveHandler(TreeViewAdv.ExpandingEvent, value);
            }
        }

        /// <summary>
        /// Identified by the <see cref="Collapsing"/> routed_event.
        /// </summary>
        public static readonly RoutedEvent CollapsingEvent = EventManager.RegisterRoutedEvent(
            "Collapsing",
            RoutingStrategy.Bubble,
            typeof(ExpandingCollapsingHandler),
            typeof(TreeViewAdv));

        /// <summary>
        /// </summary>
        /// <value>
        /// Type: <see cref="SortModeChangeHandler"/>
        /// This event raise before the <see cref="TreeViewItemAdv"/> enters the Collapsing mode.
        /// </value>
        /// <example>
        /// <para/>The following example shows how to assign an event handler
        /// for the <see cref="BeforeItemSort"/> event to a <see cref="TreeViewItemAdv"/>,
        /// and how to define the event handler in the code-behind.
        /// <code language="XAML">
        /// <![CDATA[
        ///     <TreeViewAdv Header="Work Days"
        ///                   Name="emp2WorkDays"
        ///                   Collapsing="TreeViewAdv_Collapsing">
        ///         <TreeViewItemAdv Header="Monday" />
        ///         <TreeViewItemAdv Header="Wednesday"/>
        ///     </TreeViewAdv>
        /// ]]>
        /// </code>
        /// <code language="C#">
        /// private void TreeViewAdv_Collapsing( object sender, ExpandCollapseEventArgs e )
        /// {
        ///
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="ExpandCollapseHandler"/>
        public event ExpandingCollapsingHandler Collapsing
        {
            add
            {
                AddHandler(TreeViewAdv.CollapsingEvent, value);
            }

            remove
            {
                RemoveHandler(TreeViewAdv.CollapsingEvent, value);
            }
        }

        /// <summary>
        /// Identified by the <see cref="Expanded"/> routed_event.
        /// </summary>
        public static readonly RoutedEvent ExpandedEvent = EventManager.RegisterRoutedEvent(
            "Expanded",
            RoutingStrategy.Bubble,
            typeof(ExpandedCollapsedHandler),
            typeof(TreeViewAdv));

        /// <summary>
        /// </summary>
        /// <value>
        /// Type: <see cref="SortModeChangeHandler"/>
        /// This event raise before the <see cref="TreeViewItemAdv"/> enters the Expanded mode.
        /// </value>
        /// <example>
        /// <para/>The following example shows how to assign an event handler
        /// for the <see cref="BeforeItemSort"/> event to a <see cref="TreeViewItemAdv"/>,
        /// and how to define the event handler in the code-behind.
        /// <code language="XAML">
        /// <![CDATA[
        ///     <TreeViewAdv Header="Work Days"
        ///                   Name="emp2WorkDays"
        ///                   Expanded="TreeViewAdv_Expanded">
        ///         <TreeViewItemAdv Header="Monday" />
        ///         <TreeViewItemAdv Header="Wednesday"/>
        ///     </TreeViewAdv>
        /// ]]>
        /// </code>
        /// <code language="C#">
        /// private void TreeViewAdv_Expanded( object sender, ExpandCollapseEventArgs e )
        /// {
        ///
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="ExpandCollapseHandler"/>
        public event ExpandedCollapsedHandler Expanded
        {
            add
            {
                AddHandler(TreeViewAdv.ExpandedEvent, value);
            }

            remove
            {
                RemoveHandler(TreeViewAdv.ExpandedEvent, value);
            }
        }

        /// <summary>
        /// Identified by the <see cref="Collapsed"/> routed_event.
        /// </summary>
        public static readonly RoutedEvent CollapsedEvent = EventManager.RegisterRoutedEvent(
            "Collapsed",
            RoutingStrategy.Bubble,
            typeof(ExpandedCollapsedHandler),
            typeof(TreeViewAdv));

        /// <summary>
        /// </summary>
        /// <value>
        /// Type: <see cref="SortModeChangeHandler"/>
        /// This event raise before the <see cref="TreeViewItemAdv"/> enters the Collapsed mode.
        /// </value>
        /// <example>
        /// <para/>The following example shows how to assign an event handler
        /// for the <see cref="BeforeItemSort"/> event to a <see cref="TreeViewItemAdv"/>,
        /// and how to define the event handler in the code-behind.
        /// <code language="XAML">
        /// <![CDATA[
        ///     <TreeViewAdv Header="Work Days"
        ///                   Name="emp2WorkDays"
        ///                   Collapsed="TreeViewAdv_Collapsed">
        ///         <TreeViewItemAdv Header="Monday" />
        ///         <TreeViewItemAdv Header="Wednesday"/>
        ///     </TreeViewAdv>
        /// ]]>
        /// </code>
        /// <code language="C#">
        /// private void TreeViewAdv_Collapsed( object sender, ExpandCollapseEventArgs e )
        /// {
        ///
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="ExpandCollapseHandler"/>
        public event ExpandedCollapsedHandler Collapsed
        {
            add
            {
                AddHandler(TreeViewAdv.CollapsedEvent, value);
            }

            remove
            {
                RemoveHandler(TreeViewAdv.CollapsedEvent, value);
            }
        }

        /// <summary>
        /// Identified by the <see cref="BeforeItemSort"/> routed_event.
        /// </summary>
        public static readonly RoutedEvent BeforeItemSortEvent = EventManager.RegisterRoutedEvent(
            "BeforeItemSort",
            RoutingStrategy.Bubble,
            typeof(SortModeChangeHandler),
            typeof(TreeViewAdv));

        /// <summary>
        /// </summary>
        /// <value>
        /// Type: <see cref="SortModeChangeHandler"/>
        /// This event raise before the <see cref="TreeViewItemAdv"/> enters the Sort mode.
        /// </value>
        /// <example>
        /// <para/>The following example shows how to assign an event handler
        /// for the <see cref="BeforeItemSort"/> event to a <see cref="TreeViewItemAdv"/>,
        /// and how to define the event handler in the code-behind.
        /// <code language="XAML">
        /// <![CDATA[
        ///     <TreeViewAdv Header="Work Days"
        ///                   Name="emp2WorkDays"
        ///                   AfterItemSort="TreeViewAdv_AfterItemSort"
        ///                   BeforeItemSort="TreeViewAdv_BeforeItemSort">
        ///         <TreeViewItemAdv Header="Monday" />
        ///         <TreeViewItemAdv Header="Wednesday"/>
        ///     </TreeViewAdv>
        /// ]]>
        /// </code>
        /// <code language="C#">
        /// private void TreeViewAdv_AfterItemSort( object sender, SortModeChangeEventArgs e )
        /// {
        ///     Debug.WriteLine( "AfterItemSort: old( " + e.OldValue + "), new( " + e.NewValue + ")"  );
        /// }
        /// private void TreeViewAdv_BeforeItemSort( object sender, SortModeChangeEventArgs e )
        /// {
        ///     Debug.WriteLine( "BeforeItemSort: old( " + e.OldValue + "), new( " + e.NewValue + ")" );
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="SortModeChangeHandler"/>
        public event SortModeChangeHandler BeforeItemSort
        {
            add
            {
                AddHandler(TreeViewAdv.BeforeItemSortEvent, value);
            }

            remove
            {
                RemoveHandler(TreeViewAdv.BeforeItemSortEvent, value);
            }
        }

        /// <summary>
        /// Identified by the <see cref="AfterItemSort"/> routed_event.
        /// </summary>
        public static readonly RoutedEvent AfterItemSortEvent = EventManager.RegisterRoutedEvent(
            "AfterItemSort",
            RoutingStrategy.Bubble,
            typeof(SortModeChangeHandler),
            typeof(TreeViewItemAdv));

        /// <summary>
        /// </summary>
        /// <value>
        /// Type: <see cref="SortModeChangeHandler"/>
        /// This event raise after the Sort operations are completed.
        /// </value>
        /// <example>
        /// <para/>The following example shows how to assign an event handler
        /// for the <see cref="AfterItemSort"/> event to a <see cref="TreeViewItemAdv"/>,
        /// and how to define the event handler in the code-behind.
        /// <code language="XAML">
        /// <![CDATA[
        ///     <TreeViewAdv Header="Work Days"
        ///                   Name="emp2WorkDays"
        ///                   AfterItemSort="TreeViewAdv_AfterItemSort"
        ///                   BeforeItemSort="TreeViewAdv_BeforeItemSort">
        ///         <TreeViewItemAdv Header="Monday" />
        ///         <TreeViewItemAdv Header="Wednesday"/>
        ///     </TreeViewAdv>
        /// ]]>
        /// </code>
        /// <code language="C#">
        /// private void TreeViewAdv_AfterItemSort( object sender, SortModeChangeEventArgs e )
        /// {
        ///     Debug.WriteLine( "AfterItemSort: old( " + e.OldValue + "), new( " + e.NewValue + ")"  );
        /// }
        /// private void TreeViewAdv_BeforeItemSort( object sender, SortModeChangeEventArgs e )
        /// {
        ///     Debug.WriteLine( "BeforeItemSort: old( " + e.OldValue + "), new( " + e.NewValue + ")" );
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="SortModeChangeHandler"/>
        public event SortModeChangeHandler AfterItemSort
        {
            add
            {
                AddHandler(TreeViewAdv.AfterItemSortEvent, value);
            }

            remove
            {
                RemoveHandler(TreeViewAdv.AfterItemSortEvent, value);
            }
        }

        /// <summary>
        ///   Identifies the SelectionChanged routed event.
        /// </summary>
        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(
           "SelectionChangedEvent",
           RoutingStrategy.Bubble,
           typeof(SelectionChangedEventHandler),
           typeof(TreeViewAdv));

        /// <summary>
        /// Occurs when the <see cref="SelectedItem"/> changes.
        /// </summary>
        /// <value>
        /// Type: <see cref="RoutedEventHandler"/>
        /// </value>
        /// <example>
        /// <para/>The following example shows how to specify an event handler
        /// for the SelectedItemChanged event in XAML.
        /// <code language="XAML">
        /// <TreeViewAdv SelectedItemChanged="SelectionChanged">
        ///     <TreeViewItemAdv Header="Employee1" IsSelected="True">
        ///         <TreeViewItemAdv Header="Jesper"/>
        ///         <TreeViewItemAdv Header="Aaberg"/>
        ///         <TreeViewItemAdv Header="12345"/>
        ///     </TreeViewItemAdv>
        ///     <TreeViewItemAdv Header="Employee2">
        ///         <TreeViewItemAdv Header="Dominik"/>
        ///         <TreeViewItemAdv Header="Paiha"/>
        ///         <TreeViewItemAdv Header="98765"/>
        ///     </TreeViewItemAdv>
        /// </TreeViewAdv>
        /// </code>
        /// <para/>The following example shows how to define the event handler.
        /// <code language="C#">
        /// public void SelectionChanged(object sender, RoutedEventArgs e)
        /// {
        ///     //Perform actions when SelectedItem changes
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="RoutedEventHandler"/>
        public event RoutedPropertyChangedEventHandler<object> SelectedItemChanged
        {
            add
            {
                AddHandler(TreeViewAdv.SelectedItemChangedEvent, value);
            }

            remove
            {
                RemoveHandler(TreeViewAdv.SelectedItemChangedEvent, value);
            }
        }

        /// <summary>
        /// Occurs when [selection changed].
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged
        {
            add
            {
                AddHandler(TreeViewAdv.SelectionChangedEvent, value);
            }

            remove
            {
                RemoveHandler(TreeViewAdv.SelectionChangedEvent, value);
            }
        }

        /// <summary>
        /// Identifies the PreviewSelectedItemChanged routed event.
        /// </summary>
        public static readonly RoutedEvent PreviewSelectedItemChangedEvent = EventManager.RegisterRoutedEvent(
            "PreviewSelectedItemChanged",
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<object>),
            typeof(TreeViewAdv));

        /// <summary>
        /// Occurs before the <see cref="SelectedItem"/> changes.
        /// </summary>
        /// <value>
        /// Type: <see cref="RoutedEventHandler"/>
        /// </value>
        /// <example>
        /// <para/>The following example shows how to specify an event handler
        /// for the SelectedItemChanged event in XAML.
        /// <code language="XAML">
        /// <TreeViewAdv PreviewSelectedItemChanged="PreviewSelectionChanged">
        ///     <TreeViewItemAdv Header="Employee1" IsSelected="True">
        ///         <TreeViewItemAdv Header="Jesper"/>
        ///         <TreeViewItemAdv Header="Aaberg"/>
        ///         <TreeViewItemAdv Header="12345"/>
        ///     </TreeViewItemAdv>
        ///     <TreeViewItemAdv Header="Employee2">
        ///         <TreeViewItemAdv Header="Dominik"/>
        ///         <TreeViewItemAdv Header="Paiha"/>
        ///         <TreeViewItemAdv Header="98765"/>
        ///     </TreeViewItemAdv>
        /// </TreeViewAdv>
        /// </code>
        /// <para/>The following example shows how to define the event handler.
        /// <code language="C#">
        /// public void PreviewSelectionChanged(object sender, RoutedEventArgs e)
        /// {
        ///     //Perform actions when SelectedItem changes
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="RoutedEventHandler"/>
        public event RoutedPropertyChangedEventHandler<object> PreviewSelectedItemChanged
        {
            add
            {
                AddHandler(TreeViewAdv.PreviewSelectedItemChangedEvent, value);
            }

            remove
            {
                RemoveHandler(TreeViewAdv.PreviewSelectedItemChangedEvent, value);
            }
        }

        /// <summary>
        /// Identifies TreeViewAdv DragStart event.
        /// </summary>
        public static readonly RoutedEvent DragStartEvent = EventManager.RegisterRoutedEvent(
            "DragStart",
            RoutingStrategy.Bubble,
            typeof(DragTreeViewItemAdvHandler),
            typeof(TreeViewAdv));

        /// <summary>
        /// Occurs when the items start dragging.
        /// </summary>
        /// <value>
        /// Type: <see cref="DragTreeViewItemAdvHandler"/>
        /// </value>
        /// <example>
        /// <para/>The following example shows how to specify an event handler
        /// for the DragStart event in XAML.
        /// <code language="XAML">
        /// <TreeViewAdv DragStart="OnDragStart">
        ///     <TreeViewItemAdv Header="Employee1">
        ///         <TreeViewItemAdv Header="Jesper"/>
        ///         <TreeViewItemAdv Header="Aaberg"/>
        ///         <TreeViewItemAdv Header="12345"/>
        ///     </TreeViewItemAdv>
        ///     <TreeViewItemAdv Header="Employee2">
        ///         <TreeViewItemAdv Header="Dominik"/>
        ///         <TreeViewItemAdv Header="Paiha"/>
        ///         <TreeViewItemAdv Header="98765"/>
        ///     </TreeViewItemAdv>
        /// </TreeViewAdv>
        /// </code>
        /// <para/>The following example shows how to define the event handler.
        /// <code language="C#">
        /// public void OnDragStart(object sender, DragTreeViewItemAdvEventArgs e)
        /// {
        ///     //Perform actions when DragEnd changes
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="RoutedEventHandler"/>
        public event DragTreeViewItemAdvHandler DragStart
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
        /// Identifies TreeViewAdv DragOver event.
        /// </summary>
        //SU I78477
        /*public static readonly RoutedEvent DragOverEvent = EventManager.RegisterRoutedEvent(
            "DragOver",
            RoutingStrategy.Bubble,
            typeof(DragTreeViewItemAdvHandler),
            typeof(TreeViewAdv));*/

        public new static readonly RoutedEvent DragOverEvent = EventManager.RegisterRoutedEvent(
            "DragOver",
            RoutingStrategy.Bubble,
            typeof(DragTreeViewItemAdvHandler),
            typeof(TreeViewAdv));

        //EU I78477

        /// <summary>
        /// Occurs when the items start dragging.
        /// </summary>
        /// <value>
        /// Type: <see cref="DragTreeViewItemAdvHandler"/>
        /// </value>
        /// <example>
        /// <para/>The following example shows how to specify an event handler
        /// for the DragOver event in XAML.
        /// <code language="XAML">
        /// <TreeViewAdv DragOver="OnDragOver">
        ///     <TreeViewItemAdv Header="Employee1">
        ///         <TreeViewItemAdv Header="Jesper"/>
        ///         <TreeViewItemAdv Header="Aaberg"/>
        ///         <TreeViewItemAdv Header="12345"/>
        ///     </TreeViewItemAdv>
        ///     <TreeViewItemAdv Header="Employee2">
        ///         <TreeViewItemAdv Header="Dominik"/>
        ///         <TreeViewItemAdv Header="Paiha"/>
        ///         <TreeViewItemAdv Header="98765"/>
        ///     </TreeViewItemAdv>
        /// </TreeViewAdv>
        /// </code>
        /// <para/>The following example shows how to define the event handler.
        /// <code language="C#">
        /// public void OnDragOver(object sender, DragTreeViewItemAdvEventArgs e)
        /// {
        ///     //Perform actions when DragEnd changes
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="RoutedEventHandler"/>
        //SU I78477
        //public event DragTreeViewItemAdvHandler DragOver
        public new event DragTreeViewItemAdvHandler DragOver
        //EU I78477
        {
            add
            {
                AddHandler(DragOverEvent, value);
            }

            remove
            {
                RemoveHandler(DragOverEvent, value);
            }
        }

        /// <summary>
        /// Identifies TreeViewAdv DragEnd event.
        /// </summary>
        public static readonly RoutedEvent DragEndEvent = EventManager.RegisterRoutedEvent(
            "DragEnd",
            RoutingStrategy.Bubble,
            typeof(DragTreeViewItemAdvHandler),
            typeof(TreeViewAdv));

        /// <summary>
        /// Occurs when end dragging.
        /// </summary>
        /// <value>
        /// Type: <see cref="DragTreeViewItemAdvHandler"/>
        /// </value>
        /// <example>
        /// <para/>The following example shows how to specify an event handler
        /// for the DragEnd event in XAML.
        /// <code language="XAML">
        /// <TreeViewAdv DragStart="OnDragEnd">
        ///     <TreeViewItemAdv Header="Employee1">
        ///         <TreeViewItemAdv Header="Jesper"/>
        ///         <TreeViewItemAdv Header="Aaberg"/>
        ///         <TreeViewItemAdv Header="12345"/>
        ///     </TreeViewItemAdv>
        ///     <TreeViewItemAdv Header="Employee2">
        ///         <TreeViewItemAdv Header="Dominik"/>
        ///         <TreeViewItemAdv Header="Paiha"/>
        ///         <TreeViewItemAdv Header="98765"/>
        ///     </TreeViewItemAdv>
        /// </TreeViewAdv>
        /// </code>
        /// <para/>The following example shows how to define the event handler.
        /// <code language="C#">
        /// public void OnDragEnd(object sender, DragTreeViewItemAdvEventArgs e)
        /// {
        ///     //Perform actions when DragEnd changes
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="RoutedEventHandler"/>
        public event DragTreeViewItemAdvHandler DragEnd
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

        public event PropertyChangedCallback AllowDynamicResizingChanged;

        #endregion Events

        protected static void OnLineBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewAdv treeViewAdv = (TreeViewAdv)d;
            if (treeViewAdv != null)
            {
                Pen linePen = new Pen();
                linePen.Brush = treeViewAdv.LineBrush;
                linePen.DashStyle = DashStyles.Dot;
                linePen.Thickness = 1;
                linePen.DashCap = PenLineCap.Square;
                linePen.StartLineCap = PenLineCap.Square;
                linePen.EndLineCap = PenLineCap.Square;
                linePen.LineJoin = PenLineJoin.Miter;

                treeViewAdv.LinePen = linePen;
            }
        }

        public bool IsImageSourceFreeze
        {
            get { return (bool)GetValue(IsImageSourceFreezeProperty); }
            set { SetValue(IsImageSourceFreezeProperty, value); }
        }

        #region Dependency property

        /// <summary>
        /// Identifies TreeViewAdv. LineBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty LineBrushProperty =
            DependencyProperty.Register("LineBrush", typeof(Brush), typeof(TreeViewAdv), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnLineBrushPropertyChanged)));

        // Using a DependencyProperty as the backing store for IsImageSourceFreeze.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsImageSourceFreezeProperty =
            DependencyProperty.Register("IsImageSourceFreeze", typeof(bool), typeof(TreeViewAdv), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies TreeViewAdv. LinePen dependency property.
        /// </summary>
        public static readonly DependencyProperty LinePenProperty =
            DependencyProperty.Register("LinePen", typeof(Pen), typeof(TreeViewAdv), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies TreeViewAdv. LineStrokeThickness dependency property.
        /// </summary>
        public static readonly DependencyProperty LineStrokeThicknessProperty =
            DependencyProperty.Register("LineStrokeThickness", typeof(double), typeof(TreeViewAdv), new FrameworkPropertyMetadata(1.0d, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies TreeViewAdv. LinePen dependency property.
        /// </summary>
        public static readonly DependencyProperty LineStrokeDashArrayProperty =
            DependencyProperty.Register("LineStrokeDashArray", typeof(DoubleCollection), typeof(TreeViewAdv), new FrameworkPropertyMetadata(dcollection, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies TreeViewAdv. LinePen dependency property.
        /// </summary>
        public static readonly DependencyProperty LineStrokeDashOffsetProperty =
            DependencyProperty.Register("LineStrokeDashOffset", typeof(double), typeof(TreeViewAdv), new FrameworkPropertyMetadata(0.5d, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies TreeViewAdv. ShowRootLines dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowRootLinesProperty =
            DependencyProperty.RegisterAttached("ShowRootLines", typeof(bool), typeof(TreeViewAdv), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender, OnShowRootLinesChanged));

        /// <summary>
        /// Identifies TreeViewAdv. Sorting dependency property.
        /// </summary>
        public static readonly DependencyProperty SortingProperty =
            DependencyProperty.RegisterAttached("Sorting", typeof(SortDirection), typeof(TreeViewAdv), new FrameworkPropertyMetadata(SortDirection.None, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnSortingChanged)));

        /// <summary>
        /// Identifies TreeViewAdv. EnabledRecursiveSortingProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty EnabledRecursiveSortingProperty =
            DependencyProperty.RegisterAttached("EnabledRecursiveSorting", typeof(bool), typeof(TreeViewAdv), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnEnabledRecursiveSortingChanged)));

        /// <summary>
        /// Identifies TreeViewAdv. SortingField dependency property.
        /// </summary>
        public static readonly DependencyProperty SortingFieldProperty =
            DependencyProperty.RegisterAttached("SortingField", typeof(string), typeof(TreeViewAdv), new FrameworkPropertyMetadata(C_sortingPropertyName, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnSortingFieldChanged)));

        /// <summary>
        /// Identifies TreeViewAdv. ExpanderStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpanderStyleProperty =
            DependencyProperty.RegisterAttached("ExpanderStyle", typeof(Style), typeof(TreeViewAdv), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnExpanderStyleChanged)));

        /// <summary>
        /// Identifies TreeViewAdv. SelectedTreeItem dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedTreeItemProperty =
            DependencyProperty.Register("SelectedTreeItem", typeof(object), typeof(TreeViewAdv), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnSelectedTreeItemChanged)));

        /// <summary>
        /// Identifies TreeViewAdv. SelectedTreeItemObject dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedTreeItemObjectProperty =
            DependencyProperty.Register("SelectedTreeItemObject", typeof(object), typeof(TreeViewAdv), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnSelectedTreeItemObjectChanged)));

        /// <summary>
        /// Identifies TreeViewAdv. SelectedItem dependency property.
        /// </summary>
        public static readonly DependencyPropertyKey SelectedItemPropertyKey =
            DependencyProperty.RegisterReadOnly("SelectedItem", typeof(object), typeof(TreeViewAdv), new FrameworkPropertyMetadata(null));

        /// <summary>
        ///  Identifies TreeViewAdv.SelectedItemPropertyKey dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            SelectedItemPropertyKey.DependencyProperty;

        /// <summary>
        /// Identifies TreeViewAdv. SelectedItems dependency property.
        /// </summary>
        public static readonly DependencyPropertyKey SelectedItemsPropertyKey =
            DependencyProperty.RegisterReadOnly("SelectedItems", typeof(TreeObjectCollection), typeof(TreeViewAdv), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// identifies TreeViewAdv.SelectedItems dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty =
            SelectedItemsPropertyKey.DependencyProperty;

        /// <summary>
        /// Identifies TreeViewAdv. SelectedValue dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register("SelectedValue", typeof(object), typeof(TreeViewAdv), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies TreeViewAdv. SelectedValuePath dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedValuePathProperty =
            DependencyProperty.Register("SelectedValuePath", typeof(string), typeof(TreeViewAdv), new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(TreeViewAdv.OnSelectedValuePathChanged)));

        /// <summary>
        /// Identifies TreeViewItemAdv. EditedItemTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty EditedItemTemplateProperty =
            DependencyProperty.RegisterAttached("EditedItemTemplate", typeof(DataTemplate), typeof(TreeViewAdv), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Identifies TreeViewItemAdv. EditedItemTemplateSelector dependency property.
        /// </summary>
        public static readonly DependencyProperty EditedItemTemplateSelectorProperty =
            DependencyProperty.RegisterAttached("EditedItemTemplateSelector", typeof(DataTemplateSelector), typeof(TreeViewAdv), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Identifies TreeViewItemAdv. ItemsSize dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSizeProperty =
            DependencyProperty.RegisterAttached("ItemsSize", typeof(Size), typeof(TreeViewAdv), new FrameworkPropertyMetadata(Size.Empty, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Identifies TreeViewItemAdv. AnimationType dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimationTypeProperty =
            DependencyProperty.Register("AnimationType", typeof(AnimationType), typeof(TreeViewAdv), new UIPropertyMetadata(AnimationType.None));

        /// <summary>
        /// Identifies TreeViewAdv. DragIndicatorStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty DragIndicatorStyleProperty =
            DependencyProperty.Register("DragIndicatorStyle", typeof(Style), typeof(TreeViewAdv), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies TreeViewAdv. IsFakeDragIndicator dependency property.
        /// </summary>
        public static readonly DependencyProperty IsFakeDragIndicatorProperty =
            DependencyProperty.Register("IsFakeDragIndicator", typeof(bool), typeof(TreeViewAdv), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies TreeViewAdv. IsSelectOnRightMouseClickProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectOnRightMouseClickProperty =
            DependencyProperty.Register("IsSelectOnRightMouseClick", typeof(bool), typeof(TreeViewAdv), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the CornerRadius dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            Border.CornerRadiusProperty.AddOwner(typeof(TreeViewAdv), new FrameworkPropertyMetadata(Border.CornerRadiusProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.None));

        /// <summary>
        /// Identifies the MouseOverBackground dependency property.
        /// </summary>
        public static readonly DependencyProperty MouseOverBackgroundProperty =
            DependencyProperty.RegisterAttached("MouseOverBackground", typeof(Brush), typeof(TreeViewAdv), new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Identifies the MouseOverBorderBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty MouseOverBorderBrushProperty =
            DependencyProperty.RegisterAttached("MouseOverBorderBrush", typeof(Brush), typeof(TreeViewAdv), new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Identifies the DraggingItemContainerStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty DraggingContainerProperty =
            DependencyProperty.RegisterAttached("DraggingContainer", typeof(ItemsControl), typeof(TreeViewAdv), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies the MouseOverForeground dependency property.
        /// </summary>
        public static readonly DependencyProperty MouseOverForegroundProperty =
            DependencyProperty.RegisterAttached("MouseOverForeground", typeof(Brush), typeof(TreeViewAdv), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Identifies the SelectedBackground dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedBackgroundProperty =
            DependencyProperty.RegisterAttached("SelectedBackground", typeof(Brush), typeof(TreeViewAdv), new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Identifies the SelectedBorderBrush dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedBorderBrushProperty =
            DependencyProperty.RegisterAttached("SelectedBorderBrush", typeof(Brush), typeof(TreeViewAdv), new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Identifies the SelectionUnfocussedBackcolor dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionUnfocussedBackcolorProperty =
            DependencyProperty.Register("SelectionUnfocussedBackcolor", typeof(Brush), typeof(TreeViewAdv), new UIPropertyMetadata(Brushes.LightGray));

        /// <summary>
        /// Identifies the SelectedForeground dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedForegroundProperty =
            DependencyProperty.RegisterAttached("SelectedForeground", typeof(Brush), typeof(TreeViewAdv), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Identifies the UniversalForeground dependency property.
        /// </summary>
        public static readonly DependencyProperty UniversalForegroundProperty =
            DependencyProperty.RegisterAttached("UniversalForeground", typeof(Brush), typeof(TreeViewAdv), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies the AnimationSpeed dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimationSpeedProperty =
            DependencyProperty.Register("AnimationSpeed", typeof(double), typeof(TreeViewAdv), new UIPropertyMetadata(1d, new PropertyChangedCallback(OnAnimationSpeedChanged), new CoerceValueCallback(CoerceOnAnimationSpeed)));

        /// <summary>
        /// Identifies TreeViewAdv. SelectedPath dependency property.
        /// </summary>
        public static readonly DependencyPropertyKey SelectedPathPropertyKey =
            DependencyProperty.RegisterReadOnly("SelectedPath", typeof(string), typeof(TreeViewAdv), new UIPropertyMetadata(String.Empty));

        /// <summary>
        /// Identifies TreeViewAdv. PathSeparator dependency property.
        /// </summary>
        public static readonly DependencyProperty PathSeparatorProperty = DependencyProperty.Register("PathSeparator", typeof(string), typeof(TreeViewAdv), new UIPropertyMetadata("\\"));

        /// <summary>
        /// Setting the key for the TreeViewAdv.SelectedPathProperty
        /// </summary>
        public static readonly DependencyProperty SelectedPathProperty = SelectedPathPropertyKey.DependencyProperty;

        /// <summary>
        /// Identifies <see cref="VisualStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VisualStyleProperty =
            DependencyProperty.Register("VisualStyle", typeof(TreeViewAdvVisualStyle), typeof(TreeViewAdv), new FrameworkPropertyMetadata(TreeViewAdvVisualStyle.Default, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(OnVisualStyleChanged)));

        /// <summary>
        /// Identifies FakeItemForeground dependency property.
        /// </summary>
        public static readonly DependencyProperty FakeItemForegroundProperty =
            DependencyProperty.Register("FakeItemForeground", typeof(Brush), typeof(TreeViewAdv), new UIPropertyMetadata(Brushes.Red));

        /// <summary>
        /// Identifies TreeViewAdv. MultiColumnEnable dependency property.
        /// </summary>
        public static readonly DependencyProperty MultiColumnEnableProperty =
            DependencyProperty.Register("MultiColumnEnable", typeof(bool), typeof(TreeViewAdv), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies TreeViewAdv. Columns dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(TreeViewColumnCollection), typeof(TreeViewAdv), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.NotDataBindable, new PropertyChangedCallback(TreeViewAdv.OnColumnsChanged)));

        /// <summary>
        /// Identifies TreeViewAdv. WrappedColumns dependency property.
        /// </summary>
        public static readonly DependencyPropertyKey WrappedColumnsPropertyKey =
            DependencyProperty.RegisterReadOnly("WrappedColumns", typeof(TreeViewColumnCollection), typeof(TreeViewAdv), new FrameworkPropertyMetadata());

        /// <summary>
        /// Identifies key for TreeViewAdv. WrappedColumns dependency property.
        /// </summary>
        public static readonly DependencyProperty WrappedColumnsProperty = WrappedColumnsPropertyKey.DependencyProperty;

        /// <summary>
        /// Identifies TreeViewAdv. AllowDragDrop dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowDragDropProperty =
            DependencyProperty.Register("AllowDragDrop", typeof(bool), typeof(TreeViewAdv), new UIPropertyMetadata(true));

        
        /// <summary>
        /// Identifies TreeViewAdv. IsColumnHeaderAutoWidthEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty IsColumnHeaderAutoWidthEnabledProperty =
    DependencyProperty.Register("IsColumnHeaderAutoWidthEnabled", typeof(bool), typeof(TreeViewAdv), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies TreeViewAdv. AllowDragDrop dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowsColumnReorderProperty =
            DependencyProperty.Register("AllowsColumnReorder", typeof(bool), typeof(TreeViewAdv), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies TreeViewAdv. AllowMultiSelect dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowMultiSelectProperty =
            DependencyProperty.Register("AllowMultiSelect", typeof(bool), typeof(TreeViewAdv), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies TreeViewAdv. IsVirtualizing dependency property.
        /// </summary>
        public static readonly DependencyProperty IsVirtualizingProperty =
            DependencyProperty.RegisterAttached("IsVirtualizing", typeof(bool), typeof(TreeViewAdv), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, OnIsVirtualizingChanged));

        /// <summary>
        /// Identifies TreeViewAdv. SortingDirectionArrowStyle dependency property of the <see cref="TreeViewAdv"/>.
        /// </summary>
        public static readonly DependencyProperty SortingDirectionArrowStyleProperty = DependencyProperty.Register("SortingDirectionArrowStyle", typeof(Style), typeof(TreeViewAdv), new FrameworkPropertyMetadata(null));

        /// <summary>
        ///  Identifies TreeViewAdv. DragDropEffect dependency property of the <see cref="TreeViewAdv"/>.
        /// </summary>
        public static readonly DependencyProperty DragDropEffectProperty =
            DependencyProperty.Register("DragDropEffect", typeof(TreeViewItemAdvDragDropEffects), typeof(TreeViewAdv), new FrameworkPropertyMetadata(TreeViewItemAdvDragDropEffects.Move, new PropertyChangedCallback(TreeViewAdv.OnDragDropEffectChanged)));

        /// <summary>
        /// Identifies TreeViewAdv. DragAdornerWidth dependency property of the <see cref="TreeViewAdv"/>.
        /// </summary>
        public static readonly DependencyProperty DragAdornerWidthProperty =
            DependencyProperty.Register("DragAdornerWidth", typeof(double), typeof(TreeViewAdv), new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(TreeViewAdv.OnDragAdornerWidthChanged)));

        /// <summary>
        /// Identifies TreeViewAdv. IsScrollOnExpand dependency property of the <see cref="TreeViewAdv"/>.
        /// </summary>
        public static readonly DependencyProperty IsScrollOnExpandProperty =
            DependencyProperty.Register("IsScrollOnExpand", typeof(bool), typeof(TreeViewAdv), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(TreeViewAdv.OnIsScrollOnExpandChanged)));

        public bool UnSubscribeSelectionChangedEvent
        {
            get { return (bool)GetValue(UnSubscribeSelectionChangedEventProperty); }
            set { SetValue(UnSubscribeSelectionChangedEventProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UnSubscribeSelectionChangedEvent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnSubscribeSelectionChangedEventProperty =
            DependencyProperty.Register("UnSubscribeSelectionChangedEvent", typeof(bool), typeof(TreeViewAdv), new UIPropertyMetadata(false));

        public bool ForceSingleSelection
        {
            get { return (bool)GetValue(ForceSingleSelectionProperty); }
            set { SetValue(ForceSingleSelectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForceSingleSelection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForceSingleSelectionProperty =
            DependencyProperty.Register("ForceSingleSelection", typeof(bool), typeof(TreeViewAdv), new UIPropertyMetadata(false));


        /// <summary>
        /// Identifies TreeViewAdv. IsExpandOnDrop dependency property.
        /// </summary>
        public static readonly DependencyProperty IsExpandOnDropProperty =
            DependencyProperty.Register("IsExpandOnDrop", typeof(bool), typeof(TreeViewAdv), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Gets or sets allow dynamic resizing
        /// The allow dynamic resizing is used to resize
        /// the multicolumn treeview by double click on the column divider
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="AllowDynamicResizing"/>
        /// Its Default allow dynamic resizing is false.
        /// </value>
        /// <seealso cref="AllowDynamicResizing"/>
        public bool AllowDynamicResizing
        {
            get { return (bool)GetValue(AllowDynamicResizingProperty); }
            set { SetValue(AllowDynamicResizingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowDynamicResizing.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowDynamicResizingProperty =
            DependencyProperty.Register("AllowDynamicResizing", typeof(bool), typeof(TreeViewAdv), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnAllowDynamicResizingChanged)));

        #endregion Dependency property

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is scroll on expand.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is scroll on expand; otherwise, <c>false</c>.
        /// </value>
        public bool IsScrollOnExpand
        {
            get
            {
                return (bool)GetValue(IsScrollOnExpandProperty);
            }
            set
            {
                SetValue(IsScrollOnExpandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the drag drop effect.
        /// </summary>
        /// <value>The drag drop effect.</value>
        public TreeViewItemAdvDragDropEffects DragDropEffect
        {
            get
            {
                return (TreeViewItemAdvDragDropEffects)GetValue(DragDropEffectProperty);
            }
            set
            {
                SetValue(DragDropEffectProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the drag adorner.
        /// </summary>
        /// <value>The width of the drag adorner.</value>
        public double DragAdornerWidth
        {
            get
            {
                return (double)GetValue(DragAdornerWidthProperty);
            }
            set
            {
                SetValue(DragAdornerWidthProperty, value);
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
        /// Its Default visual style is Default.
        /// </value>
        /// <seealso cref="VisualStyle"/>
        public TreeViewAdvVisualStyle VisualStyle
        {
            get
            {
                return (TreeViewAdvVisualStyle)GetValue(VisualStyleProperty);
            }

            set
            {
                SetValue(VisualStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets the selected items in a <see cref="TreeViewAdv"/>.
        /// </summary>
        /// <value>
        /// Type: <see cref="TreeObjectCollection"/>
        /// The collection selected objects in the TreeViewAdv,
        /// or a empty collection if no multiply selected items.
        /// The default value is a empty <see cref="TreeObjectCollection"/>.
        /// </value>
        /// <remarks>
        /// The SelectedItems property on the TreeViewAdv control is a read-only property
        /// and contains items when the IsSelected property value of the item TreeViewAdv
        /// is set to true more than one item.
        /// </remarks>
        /// <example>
        /// <para/>The following example shows how to get the value of the SelectedItems property in C#.
        /// <code language="C#">
        /// TreeObjectCollection selectedTVI = (TreeObjectCollection)myTreeView.SelectedItems;
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="SelectedItem"/>
        /// <seealso cref="SelectedValue"/>
        /// <seealso cref="SelectedValuePath"/>
        /// <seealso cref="TreeObjectCollection"/>
        [Bindable(true),
        ReadOnly(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Category("Appearance")]
        public TreeObjectCollection SelectedItems
        {
            get
            {
                return (TreeObjectCollection)base.GetValue(SelectedItemsProperty);
            }
        }

        /// <summary>
        /// Gets the selected item in a <see cref="TreeViewAdv"/>.
        /// </summary>
        /// <value>
        /// Type: <see cref="Object"/>
        /// The selected object in the TreeViewAdv, or a null reference if no item is selected.
        /// The default value is a null reference.
        /// </value>
        /// <remarks>
        /// The SelectedItem property on the TreeViewAdv control is a read-only property
        /// and is set to an item when the IsSelected property value of the item TreeViewAdv
        /// is set to true.
        /// </remarks>
        /// <example>
        /// <para/>The following example shows how to get the value of the SelectedItem property in C#.
        /// <code language="C#">
        /// TreeViewItemAdv selectedTVI = (TreeViewItemAdv)myTreeView.SelectedItem;
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="SelectedItems"/>
        /// <seealso cref="SelectedValue"/>
        /// <seealso cref="SelectedValuePath"/>
        [Bindable(true),
        ReadOnly(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Category("Appearance")]
        public object SelectedItem
        {
            get
            {
                return base.GetValue(SelectedItemProperty);
            }
        }

        /// <summary>
        /// Gets the value of the component of the <see cref="SelectedItem"/>
        /// at the specified <see cref="SelectedValuePath"/>.
        /// </summary>
        /// <value>
        /// Type: <see cref="Object"/>
        /// The object that is at the specified SelectedValuePath of the SelectedItem,
        /// or a null reference if no item is selected.
        /// The default value is a null reference.
        /// </value>
        /// <remarks>
        /// The SelectedValue is determined by the value of the SelectedValuePath property.
        /// <para/>The SelectedValue property is a read-only property.
        /// To change the value of a selected item in a TreeViewAdv,
        /// use the SelectedItem property to access the TreeViewItemAdv.
        /// </remarks>
        /// <example>
        /// <para/>The following example shows how to get the value of the SelectedValue property.
        /// <code language="C#">
        /// TreeViewItemAdv selectedTVValue = (TreeViewItemAdv)myTreeView.SelectedValue;
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="SelectedItem"/>
        /// <seealso cref="SelectedItems"/>
        /// <seealso cref="SelectedValuePath"/>
        [ReadOnly(true),
        Bindable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Category("Appearance")]
        public object SelectedValue
        {
            get
            {
                return base.GetValue(SelectedValueProperty);
            }
        }

        /// <summary>
        /// Gets or sets the selected tree item.
        /// </summary>
        /// <value>The selected tree item.</value>
        public object SelectedTreeItem
        {
            get
            {
                return (object)base.GetValue(SelectedTreeItemProperty);
            }

            set
            {
                base.SetValue(SelectedTreeItemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected tree item object.
        /// </summary>
        /// <value>The selected tree item object.</value>
        public object SelectedTreeItemObject
        {
            get
            {
                return (object)base.GetValue(SelectedTreeItemObjectProperty);
            }

            internal set
            {
                base.SetValue(SelectedTreeItemObjectProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the path that is used to get the <see cref="SelectedValue"/>
        /// of the <see cref="SelectedItem"/> in a <see cref="TreeViewAdv"/>.
        /// </summary>
        /// <value>
        /// Type: <see cref="string"/>
        /// A string that contains the path that is used to get the SelectedValue.
        /// The default value is String.Empty.
        /// </value>
        /// <remarks>
        /// The value of the SelectedValuePath is used to determine the SelectedValue for the
        /// SelectedItem in a TreeViewAdv.
        /// </remarks>
        /// <example>
        /// <para/>The following example shows how to access the SelectedValuePath property.
        /// <code language="C#">
        /// TreeViewItemAdv selectedTVValue = (TreeViewItemAdv)myTreeView.SelectedValue;
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="SelectedItem"/>
        /// <seealso cref="SelectedItems"/>
        /// <seealso cref="SelectedValue"/>
        [Bindable(true),
        Category("Appearance")]
        public string SelectedValuePath
        {
            get
            {
                return (string)base.GetValue(SelectedValuePathProperty);
            }

            set
            {
                base.SetValue(SelectedValuePathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether
        /// should be shown line near the top-level nodes.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// true if the line is showed; otherwise, false. The default value is true.
        /// </value>
        /// <remarks>
        /// If you set ShowRootLines to False, lines shows near the top-level nodes.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set ShowRootLines property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             myTreeView.ShowRootLines = false;
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set ShowRootLines property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview" ShowRootLines="true">
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="bool"/>
        public bool ShowRootLines
        {
            get
            {
                return (bool)GetValue(ShowRootLinesProperty);
            }

            set
            {
                SetValue(ShowRootLinesProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the path separator which is used in GetNodeFromPath method to search the node in treeview.
        /// </summary>
        /// <value>The path separator.</value>
        public string PathSeparator
        {
            get
            {
                return (string)GetValue(PathSeparatorProperty);
            }

            set
            {
                SetValue(PathSeparatorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets sorting direction of the nodes.
        /// </summary>
        /// <value>
        /// Type: <see cref="SortDirection"/>
        /// Specifies the direction of a sort operation. The default value is None.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set SortDirection property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             myTreeView.SortDirection = SortDirection.Descending;
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set SortDirection property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview" SortDirection="Descending">
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="SortDirection"/>
        public SortDirection Sorting
        {
            get
            {
                return (SortDirection)GetValue(SortingProperty);
            }

            set
            {
                SetValue(SortingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance have recursive sorting enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is recursive sorting enabled; otherwise, <c>false</c>.
        /// </value>
        public bool EnabledRecursiveSorting
        {
            get
            {
                return (bool)GetValue(EnabledRecursiveSortingProperty);
            }

            set
            {
                SetValue(EnabledRecursiveSortingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the property name being used as the sorting criteria.
        /// </summary>
        /// <value>
        /// Type: <see cref="String"/>
        /// The default value is Header.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set SortingField property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             myTreeView.SortingField = "Header";
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set SortingField property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview" SortingField="Header">
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="String"/>
        public string SortingField
        {
            get
            {
                return (string)GetValue(SortingFieldProperty);
            }

            set
            {
                SetValue(SortingFieldProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a brush that describes the background of a node line.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// The brush that is used to fill the line's node.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set LineBrush property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             myTreeView.LineBrush = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set LineBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview" LineBrush="Red">
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="Brush"/>
        public Brush LineBrush
        {
            get
            {
                return (Brush)GetValue(LineBrushProperty);
            }

            set
            {
                SetValue(LineBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a stroke thickness that describes the thickness of a node line.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// The double that is used to draw the node line.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set StrokeThickness property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             myTreeView.LineStrokeThickness = 2;
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set StrokeThickness property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview" LineStrokeThickness="2">
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="double"/>
        public double LineStrokeThickness
        {
            get
            {
                return (double)GetValue(LineStrokeThicknessProperty);
            }

            set
            {
                SetValue(LineStrokeThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a collection of Double values that indicate the pattern of dashes and gaps that is used to outline node line.
        /// </summary>
        /// <value>
        /// Type: <see cref="System.Windows.Media.DoubleCollection" />
        /// A collection of Double values that specify the pattern of dashes and gaps.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set LineStrokeDashArray property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             DoubleCollection dcollection= new DoubleCollection();
        ///             dcollection.Add(2);
        ///             myTreeView.StrokeDashArray = dcollection;
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set LineStrokeDashArray property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview" LineStrokeDashArray="2">
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="DoubleCollection"/>
        public DoubleCollection LineStrokeDashArray
        {
            get
            {
                return (DoubleCollection)GetValue(LineStrokeDashArrayProperty);
            }

            set
            {
                SetValue(LineStrokeDashArrayProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a Double that specifies the distance within the dash pattern where a dash begins in node line.
        /// </summary>
        /// <value>
        /// Type: <see cref="System.Windows.Media.DoubleCollection" />
        /// A collection of Double values that specify the pattern of dashes and gaps.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set LineStrokeDashOffset property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             myTreeView.LineStrokeDashOffset = 0.5d;
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set LineStrokeDashOffset property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview" LineStrokeDashOffset="0.5">
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="DoubleCollection"/>
        public double LineStrokeDashOffset
        {
            get
            {
                return (double)GetValue(LineStrokeDashOffsetProperty);
            }

            set
            {
                SetValue(LineStrokeDashOffsetProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a brush that describes the pen of a node line.
        /// </summary>
        /// <value>
        /// Type: <see cref="Pen"/>
        /// The pen that is used to fill the line's node.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set LinePen property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             myTreeView.LinePen = new Pen( Brushes.Red, 1 );
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set LinePen property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview">
        ///     <local:TreeViewAdv.LinePen>
        ///         <Pen Brush="Red" Thickness="2"/>
        ///     </local:TreeViewAdv.LinePen>
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="Pen"/>
        public Pen LinePen
        {
            get
            {
                return (Pen)GetValue(LinePenProperty);
            }

            set
            {
                SetValue(LinePenProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a property that enables customization of appearance, effects,
        /// or other style expander that will apply to nodes.
        /// </summary>
        /// <value>
        /// Type: <see cref="Style"/>
        /// The desired style to apply on expander.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set ExpanderStyle property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <Window.Resources>
        /// <!-- Template for TreeViewAdvExpander -->
        /// <ControlTemplate x:Key="MyFxExpanderTemplateKey" TargetType="{x:Type Expander}">
        ///     <ToggleButton Name="Expander"
        ///               ClickMode="Press"
        ///               IsChecked="{Binding Path=IsExpanded, RelativeSource={RelativeSource TemplatedParent}}">
        ///         <ToggleButton.Style>
        ///             <Style TargetType="ToggleButton">
        ///                 <Setter Property="FrameworkElement.Focusable" Value="False"/>
        ///                 <Setter Property="FrameworkElement.Width" Value="19"/>
        ///                 <Setter Property="FrameworkElement.Height" Value="13"/>
        ///                 <Setter Property="Control.Template">
        ///                     <Setter.Value>
        ///                         <ControlTemplate TargetType="ToggleButton">
        ///                             <Border Height="10" Width="10" BorderBrush="Black" BorderThickness="1">
        ///                                 <Border Name="BagroundBorder"  Background="Blue">
        ///                                 </Border>
        ///                             </Border>
        ///                             <ControlTemplate.Triggers>
        ///                                 <Trigger Property="ToggleButton.IsChecked" Value="True">
        ///                                     <Setter Property="Background" TargetName="BagroundBorder" Value="Red"/>
        ///                                 </Trigger>
        ///                             </ControlTemplate.Triggers>
        ///                         </ControlTemplate>
        ///                     </Setter.Value>
        ///                 </Setter>
        ///             </Style>
        ///         </ToggleButton.Style>
        ///     </ToggleButton>
        /// </ControlTemplate>
        /// <!-- Style for TreeViewAdvExpander -->
        /// <Style x:Key="MyEStyle" TargetType="{x:Type Expander}">
        ///     <Setter Property="Template" Value="{StaticResource MyFxExpanderTemplateKey}"/>
        /// </Style>
        /// </Window.Resources>
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview" ExpanderStyle="{DynamicResource MyEStyle}">
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="Style"/>
        public Style ExpanderStyle
        {
            get
            {
                return (Style)GetValue(ExpanderStyleProperty);
            }

            set
            {
                SetValue(ExpanderStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets value indicating whether collapsing and expanding using animation.
        /// </summary>
        /// <value>
        /// Type: <see cref="AnimationType"/>
        /// Specifies the type of a animation. The default value is Slide.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set SortDirection property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             myTreeView.AnimationType = AnimationType.Fade;
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set AnimationType property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview" AnimationType="Fade">
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="AnimationType"/>
        public AnimationType AnimationType
        {
            get
            {
                return (AnimationType)GetValue(AnimationTypeProperty);
            }

            set
            {
                SetValue(AnimationTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the edited item template selector.
        /// </summary>
        /// <value>
        /// Type: <see cref="DataTemplate"/>
        /// A custom <see cref="DataTemplateSelector"/> object that provides logic
        /// and returns a <see cref="DataTemplate"/>. The default value is a null reference.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set EditedItemTemplateSelector property in XAML.
        /// In the following example, the <I>auctionItemDataTemplateSelector</I>
        /// resource name (corresponding to an <I>AuctionItemDataTemplateSelector</I> class)
        /// is assigned to the EditedItemTemplateSelector property of the TreeViewAdv.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview"
        /// EditedItemTemplateSelector="{StaticResource auctionItemDataTemplateSelector}">
        /// <local:TreeViewItemAdv Header="Employee1">
        /// <local:TreeViewItemAdv Header="Jesper"/>
        /// <local:TreeViewItemAdv Header="Aaberg"/>
        /// <local:TreeViewItemAdv Header="12345"/>
        /// </local:TreeViewItemAdv>
        /// <local:TreeViewItemAdv Header="Employee2">
        /// <local:TreeViewItemAdv Header="Dominik"/>
        /// <local:TreeViewItemAdv Header="Paiha"/>
        /// <local:TreeViewItemAdv Header="98765"/>
        /// </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// </code>
        /// <para/>The following example shows the implementation
        /// of the <I>AuctionItemDataTemplateSelector</I> class with
        /// an override of the SelectTemplate method:
        /// <code language="C#">
        /// using System.Windows;
        /// using System.Windows.Controls;
        /// namespace SDKSample
        /// {
        /// public class AuctionItemDataTemplateSelector : DataTemplateSelector
        /// {
        /// public override DataTemplate
        /// SelectTemplate(object item, DependencyObject container)
        /// {
        /// if (item != null && item is AuctionItem)
        /// {
        /// AuctionItem auctionItem = item as AuctionItem;
        /// Window window = Application.Current.MainWindow;
        /// switch (auctionItem.SpecialFeatures)
        /// {
        /// case SpecialFeatures.None:
        /// return window.FindResource("AuctionItem_None") as DataTemplate;
        /// case SpecialFeatures.Color:
        /// return window.FindResource("AuctionItem_Color") as DataTemplate;
        /// }
        /// }
        /// return null;
        /// }
        /// }
        /// }
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="DataTemplate"/>
        public DataTemplateSelector EditedItemTemplateSelector
        {
            get
            {
                return (DataTemplateSelector)base.GetValue(TreeViewAdv.EditedItemTemplateSelectorProperty);
            }

            set
            {
                base.SetValue(TreeViewAdv.EditedItemTemplateSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the DataTemplate used to display each item in edit mode.
        /// </summary>
        /// <value>
        /// Type: <see cref="DataTemplate"/>
        /// A <see cref="DataTemplate"/> that specifies the visualization of the data objects.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set EditedItemTemplate property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <Window.Resources>
        /// <DataTemplate x:Key="CustomEditedItemTemplate" DataType="{x:Type des:TreeViewItemAdv}">
        ///     <Border BorderBrush="Red" BorderThickness="1" Margin="1" Padding="2">
        ///         <TextBox Text="{Binding Path=(des:TreeViewItemAdv.Header), Mode=TwoWay, RelativeSource={RelativeSource AncestorType={x:Type des:TreeViewItemAdv}}}"/>
        ///     </Border>
        /// </DataTemplate>
        /// </Window.Resources>
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview"
        ///     EditedItemTemplate="{StaticResource CustomEditedItemTemplate}">
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="DataTemplate"/>
        public DataTemplate EditedItemTemplate
        {
            get
            {
                return (DataTemplate)base.GetValue(TreeViewAdv.EditedItemTemplateProperty);
            }

            set
            {
                base.SetValue(TreeViewAdv.EditedItemTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets indicator for drag-and-drop.
        /// </summary>
        /// <value>
        /// Type: <see cref="FrameworkElement"/>
        /// Specifies the desired drag indicator for displaying drag-and-drop process.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set DragIndicator property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <Window.Resources>
        /// <Style x:Key="Drag_Marker" TargetType="{x:Type utilsOuter:TemplatedAdornerInternalControl}">
        ///     <Setter Property="HorizontalAlignment" Value="Left"/>
        ///     <Setter Property="VerticalAlignment" Value="Top"/>
        ///     <Setter Property="SnapsToDevicePixels" Value="False"/>
        ///     <Setter Property="Template">
        ///     <Setter.Value>
        ///         <ControlTemplate TargetType="{x:Type utilsOuter:TemplatedAdornerInternalControl}">
        ///             <Grid>
        ///                 <Grid.ColumnDefinitions>
        ///                     <ColumnDefinition Width="*"/>
        ///                 </Grid.ColumnDefinitions>
        ///                 <Rectangle Grid.Column="0"
        ///                             Height="4"
        ///                             Fill="Red"/>
        ///                 </Grid>
        ///         </ControlTemplate>
        ///     </Setter.Value>
        ///     </Setter>
        /// </Style>
        /// </Window.Resources>
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview"
        ///     DragIndicator="{StaticResource Drag_Marker}">
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="FrameworkElement"/>
        public Style DragIndicatorStyle
        {
            get
            {
                return (Style)GetValue(DragIndicatorStyleProperty);
            }

            set
            {
                SetValue(DragIndicatorStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether uses fake drag indicator.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// true if using fake drag indicator for displaying drag-and-drop process;
        /// otherwise, false. The default value is false.
        /// </value>
        /// <remarks>
        /// If you set ShowRootLines to True, for displaying drag-and-drop process
        /// is using fake drag indicator.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set IsFakeDragIndicator property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             myTreeView.IsFakeDragIndicator = true;
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set ShowRootLines property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview" IsFakeDragIndicator="true">
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="bool"/>
        public bool IsFakeDragIndicator
        {
            get
            {
                return (bool)GetValue(IsFakeDragIndicatorProperty);
            }

            set
            {
                SetValue(IsFakeDragIndicatorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether uses Select On Right MouseClick.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// true if using select an item for Right Mouse Click;
        /// otherwise, false. The default value is false.
        /// </value>

        public bool IsSelectOnRightMouseClick
        {
            get
            {
                return (bool)GetValue(IsSelectOnRightMouseClickProperty);
            }

            set
            {
                SetValue(IsSelectOnRightMouseClickProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that represents the degree
        /// to which the corners of a Border are rounded.
        /// </summary>
        /// <value>
        /// Type: <see cref="CornerRadius"/>
        /// Provides CornerRadius value for the <see cref="TreeViewAdv"/>.
        /// The default value of the CornerRadius property is 0.
        /// </value>
        /// <remarks>
        /// CornerRadius dependency property defines corner radius of the <see cref="TreeViewAdv"/>.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set CornerRadius property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             myTreeView.CornerRadius = new CornerRadius( 10 );
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set CornerRadius property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel" HorizontalAlignment="Center">
        /// <local:TreeViewAdv Name="myTreeview" CornerRadius="10">
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="CornerRadius"/>
        public CornerRadius CornerRadius
        {
            get
            {
                return (CornerRadius)base.GetValue(CornerRadiusProperty);
            }

            set
            {
                base.SetValue(CornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Brush used to fill the background of the item when mouse over.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// The Brush used to fill the background of the item when mouse over.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set MouseOverBackground property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             myTreeView.MouseOverBackground = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set MouseOverBackground property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview" MouseOverBackground="Red">
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="Brush"/>
        public Brush MouseOverBackground
        {
            get
            {
                return (Brush)GetValue(MouseOverBackgroundProperty);
            }

            set
            {
                SetValue(MouseOverBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Brush used to fill the border of the item when mouse over.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// The Brush used to fill the border of the item when mouse over.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set MouseOverBackground property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             myTreeView.MouseOverBorderBrush = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set MouseOverBorderBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview" MouseOverBorderBrush="Red">
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="Brush"/>
        public Brush MouseOverBorderBrush
        {
            get
            {
                return (Brush)GetValue(MouseOverBorderBrushProperty);
            }

            set
            {
                SetValue(MouseOverBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the dragging container with custom items.
        /// </summary>
        /// <value>The dragging container[ItemsControl].</value>
        public ItemsControl DraggingContainer
        {
            get
            {
                return (ItemsControl)GetValue(DraggingContainerProperty);
            }

            set
            {
                SetValue(DraggingContainerProperty, value);
            }
        }

        public double DraggingContainerOpacity
        {
            get { return (double)GetValue(DraggingContainerOpacityProperty); }
            set { SetValue(DraggingContainerOpacityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DraggingContainerOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DraggingContainerOpacityProperty =
            DependencyProperty.Register("DraggingContainerOpacity", typeof(double), typeof(TreeViewAdv), new FrameworkPropertyMetadata(0.5));

        /// <summary>
        /// Gets or sets the Brush to apply to the text contents
        /// of the <see cref="TreeViewItemAdv"/> when mouse over.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// The Brush to apply to the text contents of the <see cref="TreeViewItemAdv"/>
        /// when mouse over.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set MouseOverForeground property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             myTreeView.MouseOverForeground = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set MouseOverForeground property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview" MouseOverForeground="Red">
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="Brush"/>
        public Brush MouseOverForeground
        {
            get
            {
                return (Brush)GetValue(MouseOverForegroundProperty);
            }

            set
            {
                SetValue(MouseOverForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Brush used to fill the background of the selected items.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// The Brush used to fill the background of the selected items.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set SelectedBackground property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             myTreeView.SelectedBackground = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set SelectedBackground property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview" SelectedBackground="Red">
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="Brush"/>
        public Brush SelectedBackground
        {
            get
            {
                return (Brush)GetValue(SelectedBackgroundProperty);
            }

            set
            {
                SetValue(SelectedBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Brush used to fill the border of the selected items.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// The Brush used to fill the border of the selected items.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set SelectedBorderBrush property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             myTreeView.SelectedBorderBrush = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set SelectedBorderBrush property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview" SelectedBorderBrush="Red">
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="Brush"/>
        public Brush SelectedBorderBrush
        {
            get
            {
                return (Brush)GetValue(SelectedBorderBrushProperty);
            }

            set
            {
                SetValue(SelectedBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Brush used to fill the background of the selected items
        /// when it lost focus.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// The Brush used to fill the background of the selected items.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set SelectionUnfocussedBackcolor property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             myTreeView.SelectionUnfocussedBackcolor = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set SelectionUnfocussedBackcolor property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview" SelectionUnfocussedBackcolor="Red">
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="Brush"/>
        public Brush SelectionUnfocussedBackcolor
        {
            get
            {
                return (Brush)GetValue(SelectionUnfocussedBackcolorProperty);
            }

            set
            {
                SetValue(SelectionUnfocussedBackcolorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Brush to apply to the text contents of the selected items.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// The Brush to apply to the text contents of the selected items.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set SelectedForeground property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             myTreeView.SelectedForeground = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set SelectedForeground property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview" SelectedForeground="Red">
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="Brush"/>
        public Brush SelectedForeground
        {
            get
            {
                return (Brush)GetValue(SelectedForegroundProperty);
            }

            set
            {
                SetValue(SelectedForegroundProperty, value);
            }
        }

        public Brush UniversalForeground
        {
            get
            {
                return (Brush)GetValue(UniversalForegroundProperty);
            }

            set
            {
                SetValue(UniversalForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the speed of the animation for collapse and expand.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// The speed of the animation for collapse and expand. The default value is 1.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set AnimationSpeed property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             myTreeView.AnimationSpeed = 2d;
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set AnimationSpeed property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview" AnimationSpeed="2">
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="double"/>
        public double AnimationSpeed
        {
            get
            {
                return (double)GetValue(AnimationSpeedProperty);
            }

            set
            {
                SetValue(AnimationSpeedProperty, value);
            }
        }

        /// <summary>
        /// Gets path to selected item.
        /// </summary>
        /// <value>
        /// Type: <see cref="string"/>
        /// A string that contains the path to <see cref="SelectedItem"/>.
        /// </value>
        /// <remarks>
        /// The value of the SelectedPath is used to determine path to <see cref="SelectedItem"/>.
        /// </remarks>
        /// <example>
        /// <para/>The following example shows how to access the SelectedPath property.
        /// <code language="C#">
        /// string path = (TreeViewItemAdv)myTreeView.SelectedPath;
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="SelectedItem"/>
        public string SelectedPath
        {
            get
            {
                return (string)GetValue(SelectedPathProperty);
            }
        }

        public bool SelectParentContainer
        {
            get { return (bool)GetValue(SelectParentContainerProperty); }
            set { SetValue(SelectParentContainerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectParentContainer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectParentContainerProperty =
            DependencyProperty.Register("SelectParentContainer", typeof(bool), typeof(TreeViewAdv), new UIPropertyMetadata(true));

        /// <summary>
        /// Gets whether the TreeViewAdv can scroll
        /// </summary>
        /// <value></value>
        /// <returns>true if the control has a <see cref="T:System.Windows.Controls.ScrollViewer"/> in its style and has a custom keyboard scrolling behavior; otherwise, false.
        /// </returns>
        protected override bool HandlesScrolling
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the Brush to apply to the text contents of the dragging items.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// The Brush to apply to the text contents of the dragging items.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set FakeItemForeground property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             myTreeView.FakeItemForeground = Brushes.Red;
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set FakeItemForeground property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview" FakeItemForeground="Red">
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="Brush"/>
        public Brush FakeItemForeground
        {
            get
            {
                return (Brush)GetValue(FakeItemForegroundProperty);
            }

            set
            {
                SetValue(FakeItemForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether item is in MultiColumn mode.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// true if the item displayed in MultiColumn mode; otherwise, false. The default value is false.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set MultiColumnEnable property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             myTreeView.MultiColumnEnable = True;
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set MultiColumnEnable property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview" MultiColumnEnable="True">
        ///     <local:TreeViewItemAdv Header="2222">
        ///         <local:TreeViewItemAdv Header="3333"/>
        ///         <local:TreeViewItemAdv Header="3333"/>
        ///         <local:TreeViewItemAdv Header="3333"/>
        ///         <local:TreeViewItemAdv Header="3333"/>
        ///         </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="2222"/>
        ///     <local:TreeViewItemAdv Header="2222"/>
        ///     <local:TreeViewItemAdv Header="2222"/>
        ///     <local:TreeViewItemAdv Header="2222"/>
        /// <snc:TreeViewAdv.Columns>
        ///     <local:TreeViewColumnCollection>
        ///         <local:TreeViewColumn Width="100" Header="1"
        ///             DisplayMemberBinding="{Binding Path=Header, RelativeSource={RelativeSource AncestorType={x:Type snc:TreeViewItemAdv}}}"/>
        ///         <local:TreeViewColumn Width="50" Header="BBB"
        ///             DisplayMemberBinding="{Binding Path=Header, RelativeSource={RelativeSource AncestorType={x:Type snc:TreeViewItemAdv}}}"/>
        ///         <local:TreeViewColumn Width="50" Header="CCC"
        ///             DisplayMemberBinding="{Binding Path=Header, RelativeSource={RelativeSource AncestorType={x:Type snc:TreeViewItemAdv}}}"/>
        ///         <local:TreeViewColumn Width="50" Header="DDD">
        ///             <local:TreeViewColumn.CellTemplate>
        ///                 <DataTemplate>
        ///                     <Border Margin="1" BorderBrush="Red" BorderThickness="1">
        ///                         <TextBlock Text="{Binding Path=Header, RelativeSource={RelativeSource AncestorType={x:Type snc:TreeViewItemAdv}}}"/>
        ///                     </Border>
        ///                 </DataTemplate>
        ///             </local:TreeViewColumn.CellTemplate>
        ///         </local:TreeViewColumn>
        ///     </local:TreeViewColumnCollection>
        /// </local:TreeViewAdv.Columns>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="TreeViewItemAdv"/>
        /// <seealso cref="bool"/>
        public bool MultiColumnEnable
        {
            get
            {
                return (bool)GetValue(MultiColumnEnableProperty);
            }

            set
            {
                SetValue(MultiColumnEnableProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets columns for MultiColumn mode.
        /// </summary>
        /// <value>
        /// Type: <see cref="TreeViewColumnCollection"/>
        /// The collection of the <see cref="TreeViewColumn"/>.
        /// The default value is a null.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set Columns property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview" MultiColumnEnable="True">
        ///     <local:TreeViewItemAdv Header="2222">
        ///         <local:TreeViewItemAdv Header="3333"/>
        ///         <local:TreeViewItemAdv Header="3333"/>
        ///         <local:TreeViewItemAdv Header="3333"/>
        ///         <local:TreeViewItemAdv Header="3333"/>
        ///         </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="2222"/>
        ///     <local:TreeViewItemAdv Header="2222"/>
        ///     <local:TreeViewItemAdv Header="2222"/>
        ///     <local:TreeViewItemAdv Header="2222"/>
        /// <snc:TreeViewAdv.Columns>
        ///     <local:TreeViewColumnCollection>
        ///         <local:TreeViewColumn Width="100" Header="1"
        ///             DisplayMemberBinding="{Binding Path=Header, RelativeSource={RelativeSource AncestorType={x:Type snc:TreeViewItemAdv}}}"/>
        ///         <local:TreeViewColumn Width="50" Header="BBB"
        ///             DisplayMemberBinding="{Binding Path=Header, RelativeSource={RelativeSource AncestorType={x:Type snc:TreeViewItemAdv}}}"/>
        ///         <local:TreeViewColumn Width="50" Header="CCC"
        ///             DisplayMemberBinding="{Binding Path=Header, RelativeSource={RelativeSource AncestorType={x:Type snc:TreeViewItemAdv}}}"/>
        ///         <local:TreeViewColumn Width="50" Header="DDD">
        ///             <local:TreeViewColumn.CellTemplate>
        ///                 <DataTemplate>
        ///                     <Border Margin="1" BorderBrush="Red" BorderThickness="1">
        ///                         <TextBlock Text="{Binding Path=Header, RelativeSource={RelativeSource AncestorType={x:Type snc:TreeViewItemAdv}}}"/>
        ///                     </Border>
        ///                 </DataTemplate>
        ///             </local:TreeViewColumn.CellTemplate>
        ///         </local:TreeViewColumn>
        ///     </local:TreeViewColumnCollection>
        /// </local:TreeViewAdv.Columns>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="TreeViewItemAdv"/>
        /// <seealso cref="TreeViewColumnCollection"/>
        public TreeViewColumnCollection Columns
        {
            get
            {
                return (TreeViewColumnCollection)GetValue(ColumnsProperty);
            }

            set
            {
                SetValue(ColumnsProperty, value);
            }
        }

        /// <summary>
        /// Gets wrapped columns for MultiColumn mode.
        /// </summary>
        /// <value>
        /// Type: <see cref="TreeViewColumnCollection"/>
        /// The collection of the <see cref="TreeViewColumn"/>.
        /// The default value is a null.
        /// </value>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="TreeViewItemAdv"/>
        /// <seealso cref="TreeViewColumnCollection"/>
        public TreeViewColumnCollection WrappedColumns
        {
            get
            {
                return (TreeViewColumnCollection)base.GetValue(WrappedColumnsProperty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether
        /// should be able multiselect of the nodes.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// true if the multiselect is allow; otherwise, false. The default value is true.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set AllowMultiSelect property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             myTreeView.AllowMultiSelect = false;
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set AllowMultiSelect property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview" AllowMultiSelect="false">
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="bool"/>
        public bool AllowMultiSelect
        {
            get
            {
                return (bool)GetValue(AllowMultiSelectProperty);
            }

            set
            {
                SetValue(AllowMultiSelectProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether
        /// should be able drag-and-drop of the nodes.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// true if the drag-and-drop is allow; otherwise, false. The default value is true.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set AllowDragDrop property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             myTreeView.AllowDragDrop = false;
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set AllowDragDrop property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview" AllowDragDrop="false">
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="bool"/>
        public bool AllowDragDrop
        {
            get
            {
                return (bool)GetValue(AllowDragDropProperty);
            }

            set
            {
                SetValue(AllowDragDropProperty, value);
            }
        }

        //public bool AllowDragOnRightClick
        //{
        //    get
        //    {
        //        return (bool)GetValue(AllowDragOnRightClickProperty);
        //    }

        //    set
        //    {
        //        SetValue(AllowDragOnRightClickProperty, value);
        //    }
        //}

        /// <summary>
        /// Gets or sets the IsColumnHeaderAutoWidthEnabled property.
        /// </summary>
        /// <value>The ColumnHeaderAutoWidthEnabled (Target Type is bool).</value>
        ///
        public bool IsColumnHeaderAutoWidthEnabled
        {
            get
            {
                return (bool)GetValue(IsColumnHeaderAutoWidthEnabledProperty);
            }

            set
            {
                SetValue(IsColumnHeaderAutoWidthEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether
        /// should be able drag-and-drop of the columns for multicolumn.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// true if the drag-and-drop is allow; otherwise, false. The default value is true.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set AllowsColumnReorder property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             myTreeView.AllowsColumnReorder = false;
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set AllowsColumnReorder property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview" AllowsColumnReorder="false">
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="bool"/>
        public bool AllowsColumnReorder
        {
            get
            {
                return (bool)GetValue(AllowsColumnReorderProperty);
            }

            set
            {
                SetValue(AllowsColumnReorderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether that is using virtualizing.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// true if the virtualizing is using; otherwise, false. The default value is true.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set IsVirtualizing property in C#.
        /// <code language="C#">
        /// using System.Windows;
        /// using Syncfusion.Windows.Tools.Controls;
        /// namespace Sample1
        /// {
        ///     public partial class Window1 : Window
        ///     {
        ///         public Window1()
        ///         {
        ///             InitializeComponent();
        ///             myTreeView.IsVirtualizing = false;
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set IsVirtualizing property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview" IsVirtualizing="False">
        ///     <local:TreeViewItemAdv Header="Employee1">
        ///         <local:TreeViewItemAdv Header="Jesper"/>
        ///         <local:TreeViewItemAdv Header="Aaberg"/>
        ///         <local:TreeViewItemAdv Header="12345"/>
        ///     </local:TreeViewItemAdv>
        ///     <local:TreeViewItemAdv Header="Employee2">
        ///         <local:TreeViewItemAdv Header="Dominik"/>
        ///         <local:TreeViewItemAdv Header="Paiha"/>
        ///         <local:TreeViewItemAdv Header="98765"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="bool"/>
        public bool IsVirtualizing
        {
            get
            {
                return (bool)GetValue(IsVirtualizingProperty);
            }

            set
            {
                SetValue(IsVirtualizingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the sorting direction arrow style.
        /// </summary>
        /// <value>The sorting direction arrow style(Target Type is Path).</value>
        ///
        public Style SortingDirectionArrowStyle
        {
            get
            {
                return (Style)GetValue(SortingDirectionArrowStyleProperty);
            }

            set
            {
                SetValue(SortingDirectionArrowStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets of the simulate selected item.
        /// </summary>
        internal TreeViewItemAdv SelectedFalseItem
        {
            get
            {
                return m_selectedFalseItem;
            }

            set
            {
                if (value != m_selectedFalseItem)
                {
                    if (value != null)
                    {
                        value.IsSelectedFalse = true;
                    }

                    if (m_selectedFalseItem != null)
                    {
                        m_selectedFalseItem.IsSelectedFalse = false;
                    }

                    m_selectedFalseItem = value;
                    UpdateIsSelectedSelectedItems();
                }
            }
        }

        /// <summary>
        /// Gets scroll host of the TreeViewAdv.
        /// </summary>
        public ScrollViewer ScrollHost
        {
            get
            {
                return (ScrollViewer)GetTemplateChild(C_scrollViewName);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether allows users to select multiple nodes.
        /// </summary>
        internal bool IsMultiselection
        {
            get
            {
                return m_bIsMultiselection;
            }

            set
            {
                if (value != m_bIsMultiselection)
                {
                    m_bIsMultiselection = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is selected container hooked up.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selected container hooked up; otherwise, <c>false</c>.
        /// </value>
        internal bool IsSelectedContainerHookedUp
        {
            get
            {
                bool bHookUp = false;

                if (m_selectedContainer != null)
                {
                    bHookUp = m_selectedContainer.ParentTreeView == this;
                }

                return bHookUp;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether selected item(s) is changing.
        /// </summary>
        internal bool IsSelectionChangeActive
        {
            get
            {
                return m_bIsSelectionChangeActive;
            }

            set
            {
                m_bIsSelectionChangeActive = value;
            }
        }

        /// <summary>
        /// Gets container of the selected item.
        /// </summary>
        public TreeViewItemAdv SelectedContainer
        {
            get
            {
                return m_selectedContainer;
            }
        }

        /// <summary>
        /// Gets containers of the selected items.
        /// </summary>
        public TreeViewItemAdvCollection SelectedContainers
        {
            get
            {
                return m_selectedContainers;
            }
        }

        /// <summary>
        /// Gets or sets last visible item in visible items collection.
        /// </summary>
        internal TreeViewItemAdv LastVisibleItem
        {
            get
            {
                return m_lastVisibleItem;
            }

            set
            {
                m_lastVisibleItem = value;
            }
        }

        /// <summary>
        /// Gets or sets first visible item in visible items collection.
        /// </summary>
        internal TreeViewItemAdv FirstVisibleItem
        {
            get
            {
                return m_firstVisibleItem;
            }

            set
            {
                m_firstVisibleItem = value;
            }
        }

        /// <summary>
        /// Gets items that dragging.
        /// </summary>
        internal TreeObjectCollection DraggingItmes
        {
            get
            {
                return m_draggingItmes;
            }
        }

        /// <summary>
        /// Gets containers for items that dragging.
        /// </summary>
        internal TreeViewItemAdvCollection DraggingItmesContiners
        {
            get
            {
                return m_draggingItmesContiners;
            }
        }

        /// <summary>
        /// Gets parents for dragging items.
        /// </summary>
        internal TreeItemsControlCollection DraggingParentItmes
        {
            get
            {
                return m_draggingParentItmes;
            }
        }

        /// <summary>
        /// Gets a value indicating whether drag process started.
        /// </summary>
        internal static bool IsDragging
        {
            get
            {
                return m_bIsDragging;
            }
        }

        /// <summary>
        /// Gets or sets last over item.
        /// </summary>
        internal static TreeViewItemAdv LastDragOverItem
        {
            get
            {
                return m_lastDragOverItem;
            }

            set
            {
                m_lastDragOverItem = value;
            }
        }

        /// <summary>
        /// Gets or sets storage for drag data.
        /// </summary>
        internal static TreeObjectCollection DragData
        {
            get
            {
                return m_dragData;
            }

            set
            {
                if (value != m_dragData)
                {
                    m_dragData = value;

                    if (DragOverTreeView.IsFakeDragIndicator)
                    {
                        IItemsPanelRef panel = DragOverTreeView as IItemsPanelRef;

                        if (panel != null && panel.ItemsPanel != null)
                        {
                            TreeViewAdvItemsPanel.RemoveFakeItems();
                            panel.ItemsPanel.AddFakeItems(m_dragData);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets target ItemsControl for drop when drag-and-drop.
        /// </summary>
        internal static ItemsControl DragOverControl
        {
            get
            {
                return m_dragOverControl;
            }

            set
            {
                if (value != m_dragOverControl)
                {
                    HideFakeItem(m_dragOverControl);
                    m_dragOverControl = value;
                    ShowFakeItem(DragOverControl);
                }
                else if (m_dragOverControl != null && m_dragOverControl == value
                    && m_dragOverControl is TreeViewItemAdv)
                {
                    TreeViewItemAdv item = (TreeViewItemAdv)m_dragOverControl;

                    if (!item.IsFakeItem && m_lastTopPosition != item.IsShowTopMarker)
                    {
                        ShowFakeItem(DragOverControl);
                        m_lastTopPosition = item.IsShowTopMarker;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets TreeViewAdv under mouse when drag-and-drop in progress.
        /// </summary>
        internal static TreeViewAdv DragOverTreeView
        {
            get
            {
                return m_dragOverTreeView;
            }

            set
            {
                if (value != m_dragOverTreeView)
                {
                    DragOverControl = null;
                    HideAllFakeItems(m_dragOverTreeView);
                    m_dragOverTreeView = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets TreeViewAdv of the started drag-and-drop.
        /// </summary>
        internal static TreeViewAdv DragStartTreeView
        {
            get
            {
                return m_dragStartTreeView;
            }

            set
            {
                if (value != m_dragStartTreeView)
                {
                    m_dragStartTreeView = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets Control under mouse when drag-and-drop in progress.
        /// </summary>
        internal static FrameworkElement DragOverElement
        {
            get
            {
                return m_dragOverElement;
            }

            set
            {
                if (value != m_dragOverElement)
                {
                    m_dragOverElement = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether possible drop.
        /// </summary>
        internal static bool IsPossipleDrop
        {
            get
            {
                return GetIsPossipleDrop(TreeViewAdv.DragOverControl);
            }
        }

        /// <summary>
        /// Gets or sets item which editing now.
        /// </summary>
        internal TreeViewItemAdv EditingItem
        {
            get
            {
                return m_editingItem;
            }

            set
            {
                if (value != m_editingItem)
                {
                    m_editingItem = value;
                }
            }
        }

        /// <summary>
        /// Gets items host from template.
        /// </summary>
        internal ItemsPresenter ItemsHost
        {
            get
            {
                return m_itemsHost;
            }
        }

        /// <summary>
        /// Gets a value indicating whether shift key is down.
        /// </summary>
        private static bool IsShiftKeyDown
        {
            get
            {
                return (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
            }
        }

        /// <summary>
        /// Gets a value indicating whether control key is down.
        /// </summary>
        private static bool IsControlKeyDown
        {
            get
            {
                return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
            }
        }

        /// <summary>
        /// Gets the internal rect.
        /// </summary>
        /// <value>The internal rect.</value>
        internal virtual Rect InternalRect
        {
            get
            {
                Rect rect = CompleteRect;
                if (!rect.IsEmpty)
                {
                    rect.Inflate(-AutoScrollStartDistance, -AutoScrollStartDistance);
                }
                return rect;
            }
        }

        /// <summary>
        /// Gets or sets the complete rect.
        /// </summary>
        /// <value>The complete rect.</value>
        internal Rect CompleteRect
        {
            get
            {
                if (scrollBounds.IsEmpty)
                {
                    if (ScrollHost != null && ScrollHost.Content != null)
                    {
                        return new Rect(new Point(0, 0), (ScrollHost.Content as FrameworkElement).RenderSize);
                    }
                }
                return scrollBounds;
            }
            set
            {
                scrollBounds = value;
            }
        }

        /// <summary>
        /// Gets or sets the space between two rect.
        /// </summary>
        /// <value>The space between two rect.</value>
        public double AutoScrollStartDistance
        {
            get
            {
                return spaceBetweenTwoRect;
            }
            set
            {
                spaceBetweenTwoRect = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether expand item on drop.
        /// </summary>
        public bool IsExpandOnDrop
        {
            get
            {
                return (bool)GetValue(IsExpandOnDropProperty);
            }

            set
            {
                SetValue(IsExpandOnDropProperty, value);
            }
        }

        #endregion Properties

        #region IVirtualTree

        /// <summary>
        /// Gets or sets a value indicating whether Virtualization logic uses IVirtualTree interface.
        /// </summary>
        /// <value>
        /// 	<c>Extended</c> if Virtualization logic uses IVirtualTree interface; otherwise, <c>Normal</c>.
        /// </value>
        public VirtualizationMode VirtualizationMode
        {
            get { return (VirtualizationMode)GetValue(VirtualizationModeProperty); }
            set { SetValue(VirtualizationModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VirtualizationMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VirtualizationModeProperty =
            DependencyProperty.Register("VirtualizationMode", typeof(VirtualizationMode), typeof(TreeViewAdv), new PropertyMetadata(VirtualizationMode.Normal));

        /// <summary>
        /// flag indication that the TreeViewItemAdv already in expanded state.
        /// </summary>
        internal bool isAlreadyExpanded = false;

        /// <summary>
        /// ExtentHeight for the Viewport.
        /// </summary>
        private double extentHeight = 0.0;

        /// <summary>
        /// Gets or sets the ExtentHeight for the Viewport.
        /// </summary>
        internal double ExtentHeight
        {
            get
            {
                if (extentHeight == 0.0)
                {
                    extentHeight = Items.Count * treeHeight;
                }
                return extentHeight;
            }

            set
            {
                extentHeight = value;
                if (m_fakeItemsPanel != null)
                    m_fakeItemsPanel.extent.Height = value;
            }
        }

        /// <summary>
        /// internal variable to store single TreeViewItemAdv height.
        /// </summary>
        internal double treeHeight = 0.0;

        internal int expandedItemsCount = 0;

        #endregion IVirtualTree

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="TreeViewAdv"/> class.
        /// </summary>
        static TreeViewAdv()
        {
            List<double> values = new List<double>();
            values.Add(2);
            values.Add(2);
            dcollection = new DoubleCollection(values);

            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeViewAdv), new FrameworkPropertyMetadata(typeof(TreeViewAdv)));
            EventManager.RegisterClassHandler(typeof(TreeViewAdv), ItemsControl.MouseMoveEvent, new MouseEventHandler(TreeViewAdv.OnMouseMoveHandler), true);
            EventManager.RegisterClassHandler(typeof(TreeViewAdv), ItemsControl.KeyDownEvent, new KeyEventHandler(TreeViewAdv.OnKeyDownHandler), true);
            EventManager.RegisterClassHandler(typeof(TreeViewAdv), ItemsControl.KeyUpEvent, new KeyEventHandler(TreeViewAdv.OnKeyUpHandler), true);
            EventManager.RegisterClassHandler(typeof(TreeViewAdv), ItemsControl.MouseDownEvent, new MouseButtonEventHandler(TreeViewAdv.OnMouseDownHandler), true);
            EventManager.RegisterClassHandler(typeof(TreeViewAdv), ItemsControl.MouseUpEvent, new MouseButtonEventHandler(TreeViewAdv.OnMouseUpHandler), true);
            EventManager.RegisterClassHandler(typeof(TreeViewAdv), TreeViewAdv.ContextMenuClosingEvent, new ContextMenuEventHandler(TreeViewAdv.OnContextMenuClosingHandler), true);
            EventManager.RegisterClassHandler(typeof(TreeViewAdv), TreeViewAdv.ContextMenuOpeningEvent, new ContextMenuEventHandler(TreeViewAdv.OnContextMenuOpeningHandler), true);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewAdv"/> class.
        /// </summary>
        public TreeViewAdv()
        {
            if (EnvironmentTestTools.IsSecurityGranted)
            {
                EnvironmentTestTools.StartValidateLicense(typeof(TreeViewAdv));
            }
            m_loaded = true;
            SetValue(SelectedItemsPropertyKey, new TreeObjectCollection());
            AddHandler(Expander.ExpandedEvent, new RoutedEventHandler(ExpandedCollapsedHandler));
            AddHandler(Expander.CollapsedEvent, new RoutedEventHandler(ExpandedCollapsedHandler));
            this.LayoutUpdated += new EventHandler(TreeViewAdv_LayoutUpdated);
            this.Loaded += new RoutedEventHandler(TreeViewAdv_Loaded);
            this.Unloaded += new RoutedEventHandler(TreeViewAdv_Unloaded);
           
        }

        private void TreeViewAdv_Unloaded(object sender, RoutedEventArgs e)
        {
            this.m_loaded = false;
            if (this.Items != null)
            {
                LinearItems.Clear();
            }
            if (this.m_fakeItemsPanel != null)
                this.m_fakeItemsPanel = null;

            this.Loaded -= new RoutedEventHandler(TreeViewAdv_Loaded);
            this.Unloaded -= new RoutedEventHandler(TreeViewAdv_Unloaded);
        }

        private void TreeViewAdv_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.IsVirtualizing && this.VirtualizationMode == Tools.VirtualizationMode.Extended && m_treeviewadvVirtualizingPanel != null)
                this.m_treeviewadvVirtualizingPanel.InvalidateMeasure();
            modelItems = new List<TreeModel>();
            if (this.m_scrollinfo != null)
                tempViewPortHeight = this.m_scrollinfo.ViewportHeight;
            foreach (object treeitem in Items)
            {
                modelItems.Add(new TreeModel() { iTree = treeitem as IVirtualTree });
            }
        }

        /// <summary>
        /// Handles the LayoutUpdated event of the TreeViewAdv control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void TreeViewAdv_LayoutUpdated(object sender, EventArgs e)
        {
            if (this.Visibility == Visibility.Visible && IsInitialized)
            {
                if (SortDirection.None != Sorting)
                {
                    SortingTreeView(this, Sorting, SortDirection.None);
                }
                this.LayoutUpdated -= new EventHandler(TreeViewAdv_LayoutUpdated);
            }
        }

        #endregion Initialization

        #region Public methods

        /// <summary>
        /// Raises the <see cref="E:ItemGenerated"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        internal virtual void OnItemGenerated(RoutedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code
        /// or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            m_itemsHost = GetTemplateChild(C_itemsPresenterName) as ItemsPresenter;
            if (ScrollHost != null)
                ScrollHost.KeyDown += new KeyEventHandler(ScrollViewer_KeyDown);
            this.SizeChanged += new SizeChangedEventHandler(TreeViewAdv_SizeChanged);
            SetWrappedColumns();
            InitializeDefaultColumn();
            if (AllowDynamicResizing && MultiColumnEnable)
            {
                if (cell_width.Count <= 0)
                {
                    if (Columns != null)
                    {
                        foreach (TreeViewColumn column in Columns)
                        {
                            cell_width.Add(column.ActualIndex, 0);
                        }
                    }
                }
            }
        }

        private void TreeViewAdv_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.allowArrange = true;
        }

        /// <summary>
        /// Brings the into view.
        /// </summary>
        /// <param name="item">The item.</param>
        public void BringIntoView(TreeViewItemAdv item)
        {
            if (item != null)
            {
                bringintoviewstatus = true;
                m_viewableitem = item;
                item.isMakeVisibleCalled = true;
                item.BringIntoView(new Rect(0, 0, item.DesiredSize.Width, item.DesiredSize.Height));
            }
        }

        /// <summary>
        /// internal variable which has tree view item
        /// </summary>
        internal TreeViewItemAdv m_MovedItem = null;

        /// <summary>
        /// internal variable which has dictinoary for moved items
        /// </summary>
        internal System.Collections.Generic.Dictionary<object, object> m_MovedItems = new System.Collections.Generic.Dictionary<object, object>();

        //internal List<object> m_MovedItems = new List<object>();
        /// <summary>
        /// Method gets ExpanderStyleProperty.
        /// </summary>
        /// <param name="obj">DependencyObject property.</param>
        /// <returns>
        /// ExpanderStyleProperty value.
        /// </returns>
        public static Style GetExpanderStyle(DependencyObject obj)
        {
            return (Style)obj.GetValue(ExpanderStyleProperty);
        }

        /// <summary>
        /// Method sets ExpanderStyleProperty.
        /// </summary>
        /// <param name="obj">Dependency object property.</param>
        /// <param name="value">Set value.</param>
        public static void SetExpanderStyle(DependencyObject obj, Style value)
        {
            obj.SetValue(ExpanderStyleProperty, value);
        }

        /// <summary>
        /// Method gets SortingProperty.
        /// </summary>
        /// <param name="obj">DependencyObject property.</param>
        /// <returns>
        /// SortingProperty value.
        /// </returns>
        public static SortDirection GetSorting(DependencyObject obj)
        {
            return (SortDirection)obj.GetValue(SortingProperty);
        }

        /// <summary>
        /// Method sets SortingProperty.
        /// </summary>
        /// <param name="obj">Dependency object property.</param>
        /// <param name="value">Set value.</param>
        public static void SetSorting(DependencyObject obj, SortDirection value)
        {
            obj.SetValue(SortingProperty, value);
        }

        /// <summary>
        /// Method gets SortingFieldProperty.
        /// </summary>
        /// <param name="obj">DependencyObject property.</param>
        /// <returns>
        /// SortingFieldProperty value.
        /// </returns>
        public static string GetSortingField(DependencyObject obj)
        {
            return (string)obj.GetValue(SortingFieldProperty);
        }

        /// <summary>
        /// Method sets SortingFieldProperty.
        /// </summary>
        /// <param name="obj">Dependency object property.</param>
        /// <param name="value">Set value.</param>
        public static void SetSortingField(DependencyObject obj, string value)
        {
            obj.SetValue(SortingFieldProperty, value);
        }

        /// <summary>
        /// Method gets LineBrushProperty.
        /// </summary>
        /// <param name="obj">DependencyObject property.</param>
        /// <returns>
        /// LineBrushProperty value.
        /// </returns>
        public static Brush GetLineBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(LineBrushProperty);
        }

        /// <summary>
        /// Method sets LineBrushProperty.
        /// </summary>
        /// <param name="obj">Dependency object property.</param>
        /// <param name="value">Set value.</param>
        public static void SetLineBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(LineBrushProperty, value);
        }

        /// <summary>
        /// Method gets IsVirtualizingProperty.
        /// </summary>
        /// <param name="o">DependencyObject property.</param>
        /// <returns>
        /// IsVirtualizingProperty value.
        /// </returns>
        public static bool GetIsVirtualizing(DependencyObject o)
        {
            return (bool)o.GetValue(IsVirtualizingProperty);
        }

        /// <summary>
        /// Method sets IsVirtualizingProperty.
        /// </summary>
        /// <param name="element">Dependency object property.</param>
        /// <param name="value">Set value.</param>
        public static void SetIsVirtualizing(DependencyObject element, bool value)
        {
            element.SetValue(IsVirtualizingProperty, value);
        }

        /// <summary>
        /// Method gets ItemsSizeProperty.
        /// </summary>
        /// <param name="obj">DependencyObject property.</param>
        /// <returns>
        /// ItemsSizeProperty value.
        /// </returns>
        public static Size GetItemsSize(DependencyObject obj)
        {
            return (Size)obj.GetValue(TreeViewAdv.ItemsSizeProperty);
        }

        /// <summary>
        /// Method sets ItemsSizeProperty.
        /// </summary>
        /// <param name="obj">Dependency object property.</param>
        /// <param name="value">Set value.</param>
        public static void SetItemsSize(DependencyObject obj, Size value)
        {
            obj.SetValue(TreeViewAdv.ItemsSizeProperty, value);
        }

        /// <summary>
        /// Method gets ShowRootLinesProperty.
        /// </summary>
        /// <param name="obj">DependencyObject property.</param>
        /// <returns>
        /// ShowRootLinesProperty value.
        /// </returns>
        public static bool GetShowRootLines(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowRootLinesProperty);
        }

        /// <summary>
        /// Method sets ShowRootLinesProperty.
        /// </summary>
        /// <param name="obj">Dependency object property.</param>
        /// <param name="value">Set value.</param>
        public static void SetShowRootLines(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowRootLinesProperty, value);
        }

        #endregion Public methods

        #region Implementation

        private void SelectedItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (AllowMultiSelect)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        {
                            int selectedtreeCount = SelectedTreeViewItems.Count;
                            for (int i = selectedtreeCount - 1; i >= 0; i--)
                            {
                                TreeViewItemAdv treeitem = SelectedTreeViewItems[i];
                                if (!this.SelectedItems.Contains(treeitem.m_treeviewitemactualobject))
                                {
                                    this.SelectedTreeViewItems.Remove(treeitem);
                                }
                            }
                            foreach (var item in e.NewItems)
                            {
                                if (item is TreeViewItemAdv)
                                {
                                    m_selectedInCollectionChanged = true;
                                    (item as TreeViewItemAdv).IsSelected = true;
                                    SelectedTreeViewItems.Add(item as TreeViewItemAdv);
                                }
                                else
                                {
                                    TreeViewItemAdv tvItemAdv = this.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItemAdv;
                                    if (tvItemAdv == null && this.m_treeviewadv != null && this.m_treeviewadv.Count > 0 && this.SelectedItems.Count > 0)
                                    {
                                        foreach (TreeViewItemAdv treeitem in this.m_treeviewadv)
                                        {
                                            if (this.SelectedItems.Contains(treeitem.m_treeviewitemactualobject))
                                                tvItemAdv = treeitem;
                                            if (tvItemAdv != null && !tvItemAdv.IsSelected)
                                            {
                                                m_selectedInCollectionChanged = true;
                                                SetSelectedItem(SelectedItems[SelectedItems.Count - 1]);
                                                m_selectedContainer = tvItemAdv;
                                                if (!SelectedTreeViewItems.Contains(tvItemAdv))
                                                    SelectedTreeViewItems.Add(tvItemAdv);
                                                tvItemAdv.IsSelected = true;
                                            }
                                        }
                                    }
                                    else if (tvItemAdv != null)
                                    {
                                        m_selectedInCollectionChanged = true;
                                        SetSelectedItem(SelectedItems[SelectedItems.Count - 1]);
                                        m_selectedContainer = tvItemAdv;
                                        SelectedTreeViewItems.Add(tvItemAdv);
                                        tvItemAdv.IsSelected = true;
                                    }
                                    else
                                    {
                                        if (this.LinearItems.ContainsKey(item))
                                        {
                                            TreeViewItemAdv tvi = this.LinearItems[item] as TreeViewItemAdv;
                                            if (tvi != null)
                                            {
                                                m_selectedInCollectionChanged = true;
                                                tvi.IsSelected = true;
                                                SelectedTreeViewItems.Add(tvi);
                                            }
                                        }
                                    }
                                }
                            }
                            m_selectedInCollectionChanged = false;
                        }
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        {
                            if (e.OldItems != null)
                            {
                                foreach (var item in e.OldItems)
                                {
                                    if (item is TreeViewItemAdv)
                                    {
                                        (item as TreeViewItemAdv).IsSelected = false;
                                        SelectedTreeViewItems.Remove(item as TreeViewItemAdv);
                                    }
                                    else
                                    {
                                        TreeViewItemAdv tvItemAdv = this.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItemAdv;
                                        if (tvItemAdv != null)
                                        {
                                            tvItemAdv.IsSelected = false;
                                            SelectedTreeViewItems.Remove(tvItemAdv);
                                        }
                                        else
                                        {
                                            if (this.LinearItems.ContainsKey(item))
                                            {
                                                TreeViewItemAdv tvi = this.LinearItems[item] as TreeViewItemAdv;
                                                if (tvi != null)
                                                {
                                                    tvi.IsSelected = false;
                                                    SelectedTreeViewItems.Remove(tvi);
                                                    if (tvi.ParentTreeView == null || tvi.ParentItemsControl == null)
                                                    {
                                                        IList addedlist = new List<object>();
                                                        SelectionChangedEventArgs args = new SelectionChangedEventArgs(
                                                                SelectionChangedEvent, e.OldItems, addedlist);
                                                        if (!UnSubscribeSelectionChangedEvent)
                                                            OnSelectionChanged(args);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (SelectedItems.Count == 0 && SelectedItem != null)
                            {
                                TreeViewItemAdv item = this.ItemContainerGenerator.ContainerFromItem(SelectedItem) as TreeViewItemAdv;
                                if (item == null)
                                {
                                    foreach (TreeViewItemAdv treeitem in SelectedTreeViewItems)
                                    {
                                        if (treeitem.m_treeviewitemactualobject.Equals(SelectedItem))
                                        {
                                            item = treeitem;
                                            break;
                                        }
                                    }
                                }
                                if (item != null)
                                    item.IsSelected = false;
                            }
                        }
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        if (SelectedTreeViewItems != null)
                        {
                            foreach (TreeViewItemAdv tvi in SelectedTreeViewItems.ToArray())
                            {
                                tvi.IsSelected = false;
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Called when [is virtualizing changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsVirtualizingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when [drag drop effect changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDragDropEffectChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when [drag adorner width changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDragAdornerWidthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when [AllowDynmaicResizing property changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAllowDynamicResizingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TreeViewAdv tree = sender as TreeViewAdv;
            if (tree != null)
            {
                tree.OnAllowDynamicResizingChanged(e);
            }
        }

        protected void OnAllowDynamicResizingChanged(DependencyPropertyChangedEventArgs args)
        {
            if (AllowDynamicResizingChanged != null)
            {
                AllowDynamicResizingChanged(this, args);
            }
            if (AllowDynamicResizing && MultiColumnEnable)
            {
                if (cell_width.Count <= 0)
                {
                    if (Columns != null)
                    {
                        foreach (TreeViewColumn column in Columns)
                        {
                            cell_width.Add(column.ActualIndex, 0);
                        }
                    }
                }
            }
        }

       
        /// <summary>
        /// Called when [is scroll on expand changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsScrollOnExpandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Invoked just before the IsKeyboardFocusWithinChanged event is raised by this element.
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">A DependencyPropertyChangedEventArgs that contains the event data.</param>
        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsKeyboardFocusWithinChanged(e);
            bool flag = false;
            bool isKeyboardFocusWithin = base.IsKeyboardFocusWithin;

            if (isKeyboardFocusWithin)
            {
                flag = true;
            }
            else
            {
                DependencyObject focusedElement = Keyboard.FocusedElement as DependencyObject;

                if (focusedElement != null)
                {
                    if (focusedElement is ContextMenu)
                    {
                        flag = true;
                    }
                    else if (EnvironmentTest.IsSecurityGranted)
                    {
                        UIElement visualRoot = TreeViewAdv.GetVisualRoot(this) as UIElement;

                        if (((visualRoot != null) && visualRoot.IsKeyboardFocusWithin)
                            && (FocusManager.GetFocusScope(focusedElement) != visualRoot))
                        {
                            flag = true;
                        }
                    }
                }
            }

            if (((bool)base.GetValue(TreeViewItemAdv.IsSelectionActiveProperty)) != flag)
            {
                base.SetValue(TreeViewItemAdv.IsSelectionActivePropertyKey, BooleanBoxes.Box(flag));
            }

            if ((isKeyboardFocusWithin && base.IsKeyboardFocused)
                && ((m_selectedContainer != null) && !m_selectedContainer.IsKeyboardFocusWithin))
            {
                m_selectedContainer.Focus();
            }
        }

        /// <summary>
        /// Called when Items of the control is changed.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            m_itemschanged = true;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (IsVirtualizing)
                        {
                            foreach (object item in e.NewItems)
                            {
                                if (item is TreeViewItemAdv)
                                {
                                    if (!LinearList.ContainsKey(item))
                                    {
                                        if ((item as TreeViewItemAdv).CompleteHeaderElement != null)
                                        {
                                            LinearList.Add(item, (item as TreeViewItemAdv).CompleteHeaderElement.DesiredSize.Height);
                                        }
                                        else
                                        {
                                            LinearList.Add(item, 0);
                                        }
                                    }
                                }
                                else
                                {
                                    TreeViewItemAdv treeitem = ItemContainerGenerator.ContainerFromItem(item) as TreeViewItemAdv;

                                    if (treeitem != null && !LinearList.ContainsKey(treeitem))
                                    {
                                        if ((treeitem as TreeViewItemAdv).CompleteHeaderElement != null)
                                        {
                                            LinearList.Add(treeitem, (treeitem as TreeViewItemAdv).CompleteHeaderElement.DesiredSize.Height);
                                        }
                                        else
                                        {
                                            LinearList.Add(treeitem, 0);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }

                case NotifyCollectionChangedAction.Remove:
                    {
                        int count = this.SelectedItems.Count;
                        if (!SelectParentContainer)
                        {
                            for (int i = count - 1; i >= 0; i--)
                            {
                                if (!this.Items.Contains(this.SelectedItems[i]))
                                {
                                    this.oldselectedItems.Add(this.SelectedItems[i]);
                                    this.SelectedItems.RemoveAt(i);
                                }
                            }
                            if (this.SelectedItems.Count == 0)
                            {
                                this.SelectedTreeItem = null;
                                this.SelectedTreeItemObject = null;
                            }
                        }
                        if (IsVirtualizing)
                        {
                            foreach (object item in e.OldItems)
                            {
                                if (item is TreeViewItemAdv)
                                {
                                    LinearList.Remove(item);
                                }
                                else
                                {
                                    TreeViewItemAdv item1 = ItemContainerGenerator.ContainerFromItem(item) as TreeViewItemAdv;
                                    if (item1 != null)
                                        LinearList.Remove(item1);
                                }
                            }

                            this.m_scrollinfo.SetVerticalOffset(this.m_scrollinfo.VerticalOffset - this.treeHeight);
                            if (ExtentHeight != 0.0)
                                ExtentHeight = ExtentHeight - this.treeHeight;
                        }

                        if ((SelectedItem != null) && !UnSubscribeSelectionChangedEvent && !IsSelectedContainerHookedUp && SelectParentContainer)
                        {
                            SelectFirstItem();
                        }

                        break;
                    }

                case NotifyCollectionChangedAction.Reset:
                    {
                        int count = this.SelectedItems.Count;
                        for (int i = count - 1; i >= 0; i--)
                        {
                            if (!this.Items.Contains(this.SelectedItems[i]))
                                this.SelectedItems.RemoveAt(i);
                        }
                        
                        if (IsVirtualizing)
                        {
                            ExtentHeight = 0d;
                            if (this.m_treeviewadvVirtualizingPanel != null)
                            {
                                this.m_treeviewadvVirtualizingPanel.UpdateScrollInfo(this.RenderSize);
                            }
                            if (m_fakeItemsPanel != null)
                            {
                                m_fakeItemsPanel.UpdateLayout();
                            }
                            foreach (TreeViewItemAdv item in ExpandedTreeViewAdvItems.Values)
                            {
                                if (item != null && this.Items.Contains(item.m_treeviewitemactualobject) && (item.m_treeviewitemactualobject is IVirtualTree))
                                {
                                    ExtentHeight += ((IVirtualTree)(item.m_treeviewitemactualobject)).ItemsCount * treeHeight;
                                }
                            }

                            if (this.m_scrollinfo != null)
                                this.m_scrollinfo.SetVerticalOffset(this.m_scrollinfo.VerticalOffset);
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Move:
                    {
                        foreach (object item in e.OldItems)
                        {
                            m_MovedItems.Add(item, (this as IItemsPanelRef).ItemsPanel.GetInternalItems());
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Called when the ItemsSource property changes.
        /// </summary>
        /// <param name="oldValue">Old value of the ItemsSource property.</param>
        /// <param name="newValue">New value of the ItemsSource property.</param>
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            ExpandStateLinearList.Clear();
            AlreadyExpandedLinearList.Clear();
            SelectedStateLinearList.Clear();
            m_itemschanged = true;
            base.OnItemsSourceChanged(oldValue, newValue);
            LinearItems.Clear();
            if (this.IsLoaded)
                isSourceChanged = true;
            if (IsVirtualizing)
            {
                if (newValue != null)
                {
                    foreach (object item in newValue)
                    {
                        if (item is TreeViewItemAdv)
                        {
                            if (!LinearList.ContainsKey(item))
                            {
                                if ((item as TreeViewItemAdv).CompleteHeaderElement != null)
                                {
                                    LinearList.Add(item, (item as TreeViewItemAdv).CompleteHeaderElement.DesiredSize.Height);
                                }
                                else
                                {
                                    LinearList.Add(item, 0);
                                }
                            }
                        }
                        else
                        {
                            TreeViewItemAdv treeitem = ItemContainerGenerator.ContainerFromItem(item) as TreeViewItemAdv;

                            if (treeitem != null && !LinearList.ContainsKey(treeitem))
                            {
                                if ((treeitem as TreeViewItemAdv).CompleteHeaderElement != null)
                                {
                                    LinearList.Add(treeitem, (treeitem as TreeViewItemAdv).CompleteHeaderElement.DesiredSize.Height);
                                }
                                else
                                {
                                    LinearList.Add(treeitem, 0);
                                }
                            }
                        }
                    }
                }
                if (oldValue != null)
                {
                    foreach (object item in oldValue)
                    {
                        LinearList.Remove(item);
                    }
                }

                if (this.DataContext == null && this.ExpandedItemsIndexCollection != null)
                {
                    this.ExpandedItemsIndexCollection.Clear();
                }
            }
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>Returns true if the item is (or is eligible to be) its own container;
        /// otherwise, false.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeViewItemAdv;
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeViewItemAdv();
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">Element used to display the specified item.</param>
        /// <param name="item">Specified item.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            TreeViewItemAdv treeViewItem = element as TreeViewItemAdv;
            TreeViewAdvVirtualizingPanel.m_scrollmovemanually = false;
            if (treeViewItem != null && treeViewItem.ParentTreeView != null)
            {
                if (item != null && this.LinearItems.ContainsKey(item))
                {
                    TreeViewItemAdv tvi = this.LinearItems[item] as TreeViewItemAdv;
                    if (tvi != null && tvi.DataContext != null && this.ItemsSource != null && !this.Items.Contains(tvi.DataContext))
                    {
                        this.LinearItems.Remove(item);
                        this.LinearItems.Add(item, treeViewItem);
                        isLinearItemsChanged = true;
                    }
                }
                if (treeViewItem.ParentTreeViewItem == null)
                {
                    if (IsVirtualizing)
                    {
                        treeViewItem.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    }
                    if (treeViewItem.DesiredSize.Height != 0)
                    {
                        if (treeViewItem.ItemTemplate != null)
                        {
                            if ((treeViewItem.ItemTemplate.LoadContent() as FrameworkElement).Margin.Top != 0 || (treeViewItem.ItemTemplate.LoadContent() as FrameworkElement).Margin.Bottom != 0)
                            {
                                treeViewItem.ParentTreeView.m_treeviewitemadvHeight = treeViewItem.DesiredSize.Height + (treeViewItem.ItemTemplate.LoadContent() as FrameworkElement).Margin.Bottom + (treeViewItem.ItemTemplate.LoadContent() as FrameworkElement).Margin.Top;
                            }
                        }
                        else
                        {
                            if (treeitemheight == 0.0 && treeViewItem.m_fakeItemsPanel == null)
                            {
                                treeitemheight = treeViewItem.DesiredSize.Height;
                                treeViewItem.ParentTreeView.m_treeviewitemadvHeight = treeitemheight;
                            }
                            else if (treeitemheight != 0.0)
                                treeViewItem.ParentTreeView.m_treeviewitemadvHeight = treeitemheight;
                            else
                                treeViewItem.ParentTreeView.m_treeviewitemadvHeight = treeViewItem.DesiredSize.Height;
                        }
                    }
                    else if (treeViewItem.CompleteHeaderElement != null)
                    {
                        if (treeViewItem.CompleteHeaderElement.ActualHeight != 0)
                        {
                            treeViewItem.ParentTreeView.m_treeviewitemadvHeight = treeViewItem.CompleteHeaderElement.ActualHeight;
                        }
                    }
                    if (treeViewItem.ParentTreeView.m_virtualizingpanel != null)
                    {
                        (treeViewItem.ParentTreeView.m_virtualizingpanel as TreeViewAdvVirtualizingPanel).c_scrollOffset = treeViewItem.ParentTreeView.AutoScrollStartDistance;
                    }
                    IList list = (treeViewItem.ParentTreeView.ItemsSource as IList);
                    if (list != null)
                    {
                        treeViewItem.m_ItemIndex = list.IndexOf(treeViewItem.Header);
                    }
                }
                else
                {
                    if (IsVirtualizing)
                    {
                        treeViewItem.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    }
                    if (treeViewItem.DesiredSize.Height != 0)
                    {
                        if (treeViewItem.ItemTemplate != null)
                        {
                            if ((treeViewItem.ItemTemplate.LoadContent() as FrameworkElement).Margin.Top != 0 || (treeViewItem.ItemTemplate.LoadContent() as FrameworkElement).Margin.Bottom != 0)
                            {
                                treeViewItem.ParentTreeViewItem.m_treeviewitemadvHeight = treeViewItem.DesiredSize.Height + (treeViewItem.ItemTemplate.LoadContent() as FrameworkElement).Margin.Bottom + (treeViewItem.ItemTemplate.LoadContent() as FrameworkElement).Margin.Top;
                            }
                        }
                        else
                        {
                            treeViewItem.ParentTreeViewItem.m_treeviewitemadvHeight = treeViewItem.DesiredSize.Height;
                        }
                    }
                    else if (treeViewItem.CompleteHeaderElement != null)
                    {
                        if (treeViewItem.CompleteHeaderElement.ActualHeight != 0)
                        {
                            treeViewItem.ParentTreeViewItem.m_treeviewitemadvHeight = treeViewItem.CompleteHeaderElement.ActualHeight;
                        }
                    }
                    if (treeViewItem.ParentTreeViewItem.m_virtualizingpanel != null)
                    {
                        (treeViewItem.ParentTreeViewItem.m_virtualizingpanel as TreeViewAdvVirtualizingPanel).c_scrollOffset = treeViewItem.ParentTreeViewItem.m_treeviewitemadvHeight;
                    }
                    IList list = (treeViewItem.ParentTreeViewItem.ItemsSource as IList);
                    if (list != null)
                    {
                        treeViewItem.m_ItemIndex = list.IndexOf(treeViewItem.Header);
                    }
                }
            }

            if (treeViewItem != null)
            {
                treeViewItem.m_treeviewitemactualobject = item;
                if (SelectedItems != null && SelectedItems.Contains(item) || (item is IVirtualTree && (item as IVirtualTree).IsSelected))
                {
                    TreeViewItemAdv selItem = this.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItemAdv;
                    if (!selItem.IsSelected)
                    {
                        AllowChange = false;
                        if (selItem != null)
                        {
                            m_selectedInCollectionChanged = true;
                            UnSubscribeSelectionChangedEvent = true;
                            if (SelectedItems.Count > 0)
                                SetSelectedItem(SelectedItems[SelectedItems.Count - 1]);
                            m_selectedContainer = selItem;
                            if (SelectedTreeViewItems != null)
                                SelectedTreeViewItems.Add(selItem);
                            selItem.IsSelected = true;
                            UnSubscribeSelectionChangedEvent = false;
                            AllowChange = true;
                            m_selectedInCollectionChanged = false;
                        }
                    }
                }
                if (this.IsVirtualizing)
                {
                    if (!LinearList.ContainsKey(item))
                    {
                        LinearList.Add(item, 0);
                    }
                }
                else
                {
                    if (!LinearList.ContainsKey(treeViewItem))
                    {
                        LinearList.Add(treeViewItem, 0);
                    }
                }
                treeViewItem.MultiColumnEnable = MultiColumnEnable;
            }

            if (treeViewItem != null)
            {
                if (treeViewItem.ParentTreeView != null && treeViewItem.ParentTreeView.IsColumnHeaderAutoWidthEnabled)
                {
                    if (treeViewItem.ParentTreeViewItem == null)
                    {
                        treeViewItem.internalindex = 1;
                    }
                    else
                    {
                        treeViewItem.internalindex = treeViewItem.ParentTreeViewItem.internalindex + 1;
                    }

                    double calculatedwidth = 0;

                    if (treeViewItem.LeftImageSource != null || treeViewItem.RightImageSource != null || treeViewItem.ExpandedImageSource != null || treeViewItem.CollapsedImageSource != null)
                    {
                        calculatedwidth = treeViewItem.internalindex * treeViewItem.GetWidthUnheader(true);
                    }
                    else
                    {
                        calculatedwidth = treeViewItem.internalindex * treeViewItem.GetWidthUnheader(false);
                    }

                    if (calculatedwidth != 0)
                    {
                        treeViewItem.internalwidth = calculatedwidth;
                        if (this.Columns != null)
                        {
                            for (int i = 0; i < this.Columns.Count; i++)
                            {
                                PropertyInfo p = item.GetType().GetProperty(((Binding)this.Columns[i].DisplayMemberBinding).Path.Path.ToString());
                                if (p != null)
                                {
                                    if (p.GetValue(item, null) != null)
                                    {
                                        this.Columns[i].Width = new GridLength(Convert.ToDouble(p.GetValue(item, null).ToString().Length * 5 + calculatedwidth));
                                    }
                                }
                            }
                        }
                    }
                }
                if (ExpandStateLinearList.Contains(item) && IsVirtualizing)
                {
                    if (VirtualizationMode == Tools.VirtualizationMode.Extended)
                    {
                        isAlreadyExpanded = true;
                        treeViewItem.IsExpanded = true;
                        isAlreadyExpanded = false;
                    }
                    else
                        treeViewItem.IsExpanded = true;
                }

                if (AlreadyExpandedLinearList.Contains(item) && !IsVirtualizing)
                {
                    treeViewItem.IsExpanded = true;
                }

                if (SelectedStateLinearList.Contains(item) && IsVirtualizing && !droppedobjects.Contains(item) && !DraggingItmes.Contains(item))
                {
                    treeViewItem.ParentTreeView.m_itemsselectedfromlinearlist = true;
                    isSelectedFromPrepareContainer = true;
                    if (SelectedItems.Contains(treeViewItem))
                        UnSubscribeSelectionChangedEvent = true;
                    treeViewItem.IsSelected = true;
                    isSelectedFromPrepareContainer = false;
                    UnSubscribeSelectionChangedEvent = false;
                }
                else
                {
                    treeViewItem.ParentTreeView.m_itemsselectedfromlinearlist = false;
                }
            }

            if (treeViewItem != null)
            {
                if (treeViewItem.IsSelected)
                {
                    if (!m_selectedContainers.Contains(treeViewItem))
                    {
                        if (!AllowMultiSelect)
                        {
                            this.ChangeSelection(item, treeViewItem, treeViewItem.IsSelected, false);
                            m_selectedContainers.Clear();
                            m_selectedContainers.Add(treeViewItem);
                        }
                        else
                        {
                            m_selectedContainers.Add(treeViewItem);
                        }
                    }
                }
                else
                {
                    if (m_selectedContainers.Contains(treeViewItem))
                    {
                        m_selectedContainers.Remove(treeViewItem);
                    }
                }
               
            }

            object source = null;
            bool isSelected = false;
            if (treeViewItem.ParentTreeView != null && treeViewItem.ParentTreeView.IsInitialized && treeViewItem.ParentTreeView.m_MovedItems != null && treeViewItem.ParentTreeView.m_MovedItems.Count > 0 && AllowToMoveItems(ref source, ref isSelected, treeViewItem))
            {
                if (source != null)
                {
                    if (treeViewItem.ParentTreeView.m_MovedItems[source] is TreeViewItemAdv)
                    {
                        treeViewItem.ParentTreeView.RemoveItemFromContainer(treeViewItem.ParentTreeView.m_MovedItems[source] as TreeViewItemAdv);
                        (treeViewItem.ParentTreeView.m_MovedItems[source] as TreeViewItemAdv).ReleaseMemoryForOldItem();
                    }
                    treeViewItem.IsSelected = true;
                    treeViewItem.ParentTreeView.BringIntoView(treeViewItem);
                    treeViewItem.ParentTreeView.m_MovedItems.Remove(source);
                }
            }
            else if (source != null && !isSelected)
            {
                treeViewItem.ParentTreeView.m_MovedItems.Remove(source);
            }
        }

        /// <summary>
        /// Allows to move items.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="isSelected">if set to <c>true</c> [is selected].</param>
        /// <param name="treeViewItem">The tree view item.</param>
        /// <returns></returns>
        internal bool AllowToMoveItems(ref object source, ref bool isSelected, TreeViewItemAdv treeViewItem)
        {
            bool allowMe = false;
            foreach (object item in treeViewItem.ParentTreeView.m_MovedItems.Keys)
            {
                if (treeViewItem.Header.Equals(item) && treeViewItem.ParentTreeView.m_MovedItems[item] is TreeViewItemAdv)
                {
                    if ((treeViewItem.ParentTreeView.m_MovedItems[item] as TreeViewItemAdv).IsSelected)
                    {
                        isSelected = true;
                        allowMe = true;
                    }
                    source = item;
                    break;
                }
            }
            return allowMe;
        }

        

        /// <summary>
        /// Returns class-specific AutomationPeer implementations
        /// for the Windows Presentation Foundation (WPF) infrastructure.
        /// </summary>
        /// <returns>The type-specific AutomationPeer implementation.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new TreeViewAdvAutomationPeer(this);
        }

        /// <summary>
        /// Called when [mouse move handler].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private static void OnMouseMoveHandler(object sender, MouseEventArgs e)
        {
            TreeViewAdv tree = sender as TreeViewAdv;

            if (tree != null && tree.IsLoaded && EnvironmentTest.IsSecurityGranted &&
                tree.AllowDragDrop && !m_bIsDragging &&
                (e.LeftButton == MouseButtonState.Pressed || (e.RightButton == MouseButtonState.Pressed)) && tree.m_clickOnHeader && !tree.IsDragInEditingState)
            {
                Point currentPos = MouseUtils.GetMousePosition(null);
                double horizontalDragDistance = Math.Abs(currentPos.X - tree.m_dragStart.X);
                double verticalDragDistance = Math.Abs(currentPos.Y - tree.m_dragStart.Y);

                if (horizontalDragDistance > SystemParameters.MinimumHorizontalDragDistance
                    || verticalDragDistance > SystemParameters.MinimumVerticalDragDistance)
                {
                    TreeViewItemAdv item = e.Source as TreeViewItemAdv;

                    if (item == null)
                    {
                        item = TreeViewAdv.GetTreeViewItemFromChildren(e.OriginalSource as FrameworkElement);
                    }
                    item = item == null ? TreeViewAdv.GetTreeViewItemFromChildrenInDescendantMode(e.OriginalSource as FrameworkElement) : item;

                    if ((tree.m_bIsAvailableStartDrag && item != null)
                        && ((tree.ContextMenu != null && !tree.ContextMenu.IsVisible) || tree.ContextMenu == null))
                    {
                        if (item != null)
                        {
                            var treeViewAdv = GetTreeViewFromChildren(item);
                            if (treeViewAdv != null && treeViewAdv != tree)
                            {
                                if(treeViewAdv.m_IsDraggedItemIsTreeViewItemAdv)
                                {
                                    ParentContainer = item.ParentItemsControl as TreeViewItemAdv;
                                    treeViewAdv.DoAutoScroll();
                                    treeViewAdv.DragStarted();
                                }
                            }
                            else
                            {
                                if (tree.m_IsDraggedItemIsTreeViewItemAdv)
                                {
                                    ParentContainer = item.ParentItemsControl as TreeViewItemAdv;
                                    tree.DoAutoScroll();
                                    tree.DragStarted();
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when [key down handler].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyboardEventArgs"/> instance containing the event data.</param>
        private static void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                dragdropeffects = TreeViewItemAdvDragDropEffects.Copy;
            }
            else if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                dragdropeffects = TreeViewItemAdvDragDropEffects.Move;
            }
        }

        private static void OnKeyUpHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl || e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                dragdropeffects = TreeViewItemAdvDragDropEffects.None;
            }
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        public void SelectAll()
        {
            foreach (object obj in this.Items)
            {
                TreeViewItemAdv item = this.ItemContainerGenerator.ContainerFromItem(obj) as TreeViewItemAdv;
                if (item != null)
                {
                    if (!item.IsExpanded)
                    {
                        item.IsExpanded = true;
                    }
                    if (item.HasItems)
                    {
                        PassTree(item, false);
                    }
                }
            }
            HandleVisualConnector();
        }

        /// <summary>
        /// Passes the tree.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="select">if set to <c>true</c> [select].</param>
        private void PassTree(TreeViewItemAdv tree, bool select)
        {
            foreach (object obj in tree.Items)
            {
                TreeViewItemAdv item = tree.ItemContainerGenerator.ContainerFromItem(obj) as TreeViewItemAdv;

                if (item != null)
                {
                    if (select)
                    {
                        if (!item.IsSelected)
                        {
                            item.IsSelected = true;
                        }
                    }
                    else
                    {
                        if (!item.IsExpanded)
                        {
                            item.IsExpanded = true;
                        }
                    }

                    if (item.HasItems)
                    {
                        PassTree(item, select);
                    }
                }
            }
        }

        /// <summary>
        /// Iterates the items using parent treeview.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="parentTreeview">The parent treeview.</param>
        internal static void IterateItems(TreeViewItemAdv tree, TreeViewAdv parentTreeview)
        {
            foreach (object obj in tree.Items)
            {
                TreeViewItemAdv item = tree.ItemContainerGenerator.ContainerFromItem(obj) as TreeViewItemAdv;

                if (item != null)
                {
                    parentTreeview.LinearList.Remove(item);

                    if (item.HasItems)
                    {
                        IterateItems(item, parentTreeview);
                    }
                }
            }
        }

        /// <summary>
        /// Stops the auto scroll.
        /// </summary>
        private void StopHandleVisualConnector()
        {
            if (handleVisualConnector != null)
            {
                handleVisualConnector.Tick -= new EventHandler(handleVisualConnector_Tick);
                handleVisualConnector.Stop();
                handleVisualConnector = null;
            }
        }

        /// <summary>
        /// Does the auto scroll.
        /// </summary>
        private void HandleVisualConnector()
        {
            if (handleVisualConnector == null)
            {
                handleVisualConnector = new DispatcherTimer();
                handleVisualConnector.Interval = new TimeSpan(0, 0, 0, 0, 20);
                handleVisualConnector.Tick += new EventHandler(handleVisualConnector_Tick);
                handleVisualConnector.Start();
            }
        }

        /// <summary>
        /// Handles the Tick event of the handleVisualConnector control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void handleVisualConnector_Tick(object sender, EventArgs e)
        {
            foreach (object obj in this.Items)
            {
                TreeViewItemAdv item = this.ItemContainerGenerator.ContainerFromItem(obj) as TreeViewItemAdv;
                if (item != null)
                {
                    if (!item.IsSelected)
                    {
                        item.IsSelected = true;
                    }
                    if (item.HasItems)
                    {
                        PassTree(item, true);
                    }
                }
            }

            StopHandleVisualConnector();
        }

        /// <summary>
        /// Stops the auto scroll.
        /// </summary>
        private void StopAutoScroll()
        {
            if (autoScrollerTimer != null)
            {
                autoScrollerTimer.Tick -= new EventHandler(autoScrollerTimer_Tick);
                autoScrollerTimer.Stop();
                autoScrollerTimer = null;
            }
        }

        /// <summary>
        /// Does the auto scroll.
        /// </summary>
        private void DoAutoScroll()
        {
            if (autoScrollerTimer == null)
            {
                autoScrollerTimer = new DispatcherTimer();
                autoScrollerTimer.Interval = new TimeSpan(0, 0, 0, 0, 20);
                autoScrollerTimer.Tick += new EventHandler(autoScrollerTimer_Tick);
                autoScrollerTimer.Start();
            }
        }

        /// <summary>
        /// Handles the Tick event of the auto scroll.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void autoScrollerTimer_Tick(object sender, EventArgs e)
        {
            if (latestMousePossition.X > 0 || latestMousePossition.Y > 0)
            {
                DoAutoScroll(latestMousePossition);
            }
        }

        /// <summary>
        /// Does the auto scroll.
        /// </summary>
        /// <param name="mousePoint">The mouse point.</param>
        public void DoAutoScroll(Point mousePoint)
        {
            Rect innerRect = InternalRect;
            if (mousePoint.Y < innerRect.Top && m_bIsDragging && ScrollHost != null)
            {
                ScrollHost.LineUp();
            }
            else if (mousePoint.Y > innerRect.Bottom && m_bIsDragging && ScrollHost != null)
            {
                ScrollHost.LineDown();
            }
        }

        /// <summary>
        /// Called on drop event.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            if (this.AllowDragDrop)
            {
                if (MultiColumnEnable)
                    dragOverItem = TreeViewAdv.GetTreeViewItemFromChildren(e.OriginalSource as FrameworkElement);

                DragEnded(e);
                if (IsVirtualizing && VirtualizationMode == VirtualizationMode.Extended)
                    drag_count = this.SelectedItems.Count;
                else
                    drag_count = this.DraggingItmes.Count;
            }
        }

        /// <summary>
        /// Called on query continue drag event.
        /// </summary
        /// <param name="e">Event arguments</param>
        protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e)
        {
            base.OnQueryContinueDrag(e);
            if (this.AllowDragDrop)
            {
                DragOverItemsUpdate();
                LayoutPopupContainer();
            }
        }

        /// <summary>
        /// Drags the over items update.
        /// </summary>
        private void DragOverItemsUpdate()
        {
            Point p;
            if (DragOverTreeView != null)
            {
                p = MouseUtils.GetMousePosition(DragOverTreeView.ScrollHost);

                if (DragOverTreeView.FlowDirection == FlowDirection.RightToLeft)
                {
                    p.X = Math.Abs(p.X);
                }
                ScaleTransform scaleTransform = this.LayoutTransform as ScaleTransform;

                if (scaleTransform != null)
                {
                    scalefractionx = scaleTransform.ScaleX;
                    scalefractiony = scaleTransform.ScaleY;
                }
                if (p.X < 0 || p.X > (DragOverTreeView.ActualWidth * MouseUtils.GetDPIFraction() * scalefractionx)
                    || p.Y < 0 || p.Y > (DragOverTreeView.ActualHeight * MouseUtils.GetDPIFraction() * scalefractiony))
                {
                    DragOverElement = null;
                }
            }

            DragOverTreeView = TreeViewAdv.GetTreeViewFromChildren(DragOverElement);

            if (DragOverTreeView != null && (DragOverTreeView.AllowDragDrop || DragOverTreeView.AllowDrop) && !DragOverTreeView.IsDragInEditingState)
            {
                p = MouseUtils.GetMousePosition(DragOverTreeView.ScrollHost);
                latestMousePossition = MouseUtils.GetMousePosition(DragOverTreeView.ScrollHost);
                TreeViewItemAdv item = DragOverTreeView.FindItemByPoint(p);

                if (item != null && !item.IsFakeItem)
                {
                    DragTreeViewItemAdvEventArgs args = new DragTreeViewItemAdvEventArgs(DragOverEvent);
                    if (DragOverTreeView.GetAdornerLayer().Visibility == Visibility.Visible)
                    {
                        args.TargetOverItem = null;
                    }
                    else
                        args.TargetOverItem = item as ItemsControl;

                    args.DropIndex = item.GetDropIndex();
                    args.DraggingItems = DraggingItmesContiners;
                    RaiseEvent(args);
                    argsdragdropeffetschanged = args.Effects;
                    if (!args.Handled && !args.Cancel)
                    {
                        DropTreeView = item.ParentTreeView;
                        if (this.tempDraggedItem != null && this.tempDraggedItem != item)
                        {
                            tempDraggedItem.IsDragOver = false;
                        }
                        if (DraggedTreeView != null && DropTreeView != null)
                        {
                            if (!DraggedTreeView.Equals(DropTreeView))
                                tempDraggedItem = item;
                        }

                        if (DraggedTreeView != null)
                        {
                            if (!DraggedTreeView.Equals(DropTreeView) && DraggedTreeView != null)
                            {
                                if (tempDraggedItem != null)
                                    tempDraggedItem.IsDragOver = true;
                            }
                        }

                        DragOverControl = item;

                        //if ((item as TreeViewItemAdv) != null && item.m_expander.Visibility == Visibility.Visible)
                        //{
                        //    if (!(item as TreeViewItemAdv).IsExpanded)
                        //    {
                        //        (item as TreeViewItemAdv).IsExpanded = true;
                        //    }
                        //}

                        bool isDragOver = item.IsDragOver;
                        ItemsControl parent = item.ParentItemsControl;
                        int dropIndex = item.GetDropIndex();

                        bool bInternalDrop = (parent != null && !isDragOver && dropIndex != -1) ?
                            false : true;

                        if (bInternalDrop)
                        {
                            if (IsWindowsXP)
                            {
                                item.m_topdragLine.Visibility = Visibility.Collapsed;
                                item.m_bottomdragLine.Visibility = Visibility.Collapsed;
                            }
                            else
                            {
                                DragOverTreeView.GetAdornerLayer().Visibility = Visibility.Collapsed;
                            }
                        }
                        else
                        {
                            if (!IsWindowsXP)
                            {
                                DragOverTreeView.GetAdornerLayer().Visibility = Visibility.Visible;
                            }
                        }
                        if (item.ParentTreeView != this && (latestMousePossition.X > 0 || latestMousePossition.Y > 0))
                        {
                            if (item.ParentTreeView != null)
                            {
                                item.ParentTreeView.DoAutoScroll(latestMousePossition);
                                if (autoScrollerTimer != null)
                                {
                                    autoScrollerTimer.Stop();
                                }
                            }
                        }
                        else
                        {
                            if (autoScrollerTimer != null)
                            {
                                if (!autoScrollerTimer.IsEnabled)
                                {
                                    autoScrollerTimer.Start();
                                }
                            }
                        }
                    }
                    else
                    {
                        double height = 0;
                        height += item.Margin.Top + item.Margin.Bottom;

                        if (item.CompleteHeaderElement != null)
                        {
                            if (item.DesiredSize.Height > item.CompleteHeaderElement.ActualHeight)
                            {
                                if (!item.IsExpanded)
                                {
                                    height += item.DesiredSize.Height;
                                }
                                else
                                {
                                    height += item.CompleteHeaderElement.ActualHeight;
                                }
                            }
                            else
                            {
                                height += item.CompleteHeaderElement.ActualHeight;
                            }
                        }
                        else
                        {
                            if (item.DesiredSize.Height > item.ActualHeight)
                            {
                                if (!item.IsExpanded)
                                {
                                    height += item.DesiredSize.Height;
                                }
                                else
                                {
                                    height += item.ActualHeight;
                                }
                            }
                            else
                            {
                                height += item.ActualHeight;
                            }
                        }

                        if (item.ParentItemsControl != null && (item.ParentItemsControl as TreeViewAdv) != null && m_pointovercontroldiff > height - 2)
                        {
                            DragOverControl = item;
                            bool isDragOver = item.IsDragOver;
                            ItemsControl parent = item.ParentItemsControl;
                            int dropIndex = item.GetDropIndex();

                            bool bInternalDrop = (parent != null && !isDragOver && dropIndex != -1) ?
                                false : true;

                            if (bInternalDrop)
                            {
                                if (IsWindowsXP)
                                {
                                    item.m_topdragLine.Visibility = Visibility.Collapsed;
                                    item.m_bottomdragLine.Visibility = Visibility.Collapsed;
                                }
                                else
                                {
                                    DragOverTreeView.GetAdornerLayer().Visibility = Visibility.Collapsed;
                                }
                            }
                            else
                            {
                                if (!IsWindowsXP)
                                {
                                    DragOverTreeView.GetAdornerLayer().Visibility = Visibility.Visible;
                                }
                            }
                            if (item.ParentTreeView != this && (latestMousePossition.X > 0 || latestMousePossition.Y > 0))
                            {
                                if (item.ParentTreeView != null)
                                {
                                    item.ParentTreeView.DoAutoScroll(latestMousePossition);
                                    if (autoScrollerTimer != null)
                                    {
                                        autoScrollerTimer.Stop();
                                    }
                                }
                            }
                            else
                            {
                                if (autoScrollerTimer != null)
                                {
                                    if (!autoScrollerTimer.IsEnabled)
                                    {
                                        autoScrollerTimer.Start();
                                    }
                                }
                            }
                        }
                        else
                        {
                            changeddragdropeffects = TreeViewItemAdvDragDropEffects.None;
                        }
                    }
                }
                else if (item == null)
                {
                    double absX = Math.Abs(p.X);

                    if (absX > 0 && absX < (DragOverTreeView.ActualWidth * MouseUtils.GetDPIFraction())
                        && p.Y > C_hittestOffset && p.Y < (DragOverTreeView.ActualHeight * MouseUtils.GetDPIFraction()))
                    {
                        DragOverControl = DragOverTreeView;
                    }
                    else
                    {
                        DragOverControl = null;
                    }
                    if (DragOverTreeView != this && (latestMousePossition.X > 0 || latestMousePossition.Y > 0))
                    {
                        DragOverTreeView.DoAutoScroll(latestMousePossition);
                        autoScrollerTimer.Stop();
                    }
                    else
                    {
                        if (autoScrollerTimer != null)
                        {
                            if (!autoScrollerTimer.IsEnabled)
                            {
                                autoScrollerTimer.Start();
                            }
                        }
                    }
                }
            }
            else
            {
                DragOverControl = null;
            }
        }

        /// <summary>
        /// Raises the SelectedItemChanged event when the SelectedItem property value changes.
        /// </summary>
        /// <param name="e">Provides the item that was previously selected and
        /// the item that is currently selected for the SelectedItemChanged event.</param>
        protected virtual void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            if (SelectedItem != null)
            {
                string path = GetPathToItem(SelectedItem as TreeViewItemAdv);
                SetSelectedPath(path);
                if (SelectedItems.Count == 1 && SelectedItem != null && SelectedItems.Contains(SelectedItem))
                {
                    TreeViewItemAdv item = this.ItemContainerGenerator.ContainerFromItem(SelectedItem) as TreeViewItemAdv;
                    if (item != null && !item.IsSelected)
                        item.IsSelected = true;
                }
            }

            if (!(Keyboard.Modifiers == ModifierKeys.Control && m_bUpdownNavigation))
            {
                base.RaiseEvent(e);
            }
        }

        /// <summary>
        /// Raises the SelectedItemChanged event when the SelectedItem property value changes.
        /// </summary>
        /// <param name="e">Provides the item that was previously selected and
        /// the item that is currently selected for the SelectedItemChanged event.</param>
        protected virtual void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (SelectedItem != null)
            {
                allowSelectedTreeItem = false;
                if (SelectedTreeItem == null)
                    SelectedTreeItem = this.ItemContainerGenerator.ContainerFromItem(SelectedItem);
                allowSelectedTreeItem = true;
                if ((SelectedItem as TreeViewItemAdv) != null)
                {
                    if ((SelectedItem as TreeViewItemAdv).m_treeviewitemactualobject != null)
                    {
                        SelectedTreeItemObject = (SelectedItem as TreeViewItemAdv).m_treeviewitemactualobject;
                    }
                }
                string path = GetPathToItem(SelectedItem as TreeViewItemAdv);
                SetSelectedPath(path);
            }

            if (!(Keyboard.Modifiers == ModifierKeys.Control && m_bUpdownNavigation))
            {
                base.RaiseEvent(e);
            }
            this.oldselectedItems.Clear();
        }

        /// <summary>
        /// Raises the PreviewSelectedItemChanged event when the SelectedItem property value changes.
        /// </summary>
        /// <param name="e">Provides the item that was previously selected and
        /// the item that is currently selected for the SelectedItemChanged event.</param>
        protected virtual void OnPreviewSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            if (!(Keyboard.Modifiers == ModifierKeys.Control && m_bUpdownNavigation))
            {
                base.RaiseEvent(e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:BeforeItemSort"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnBeforeItemSort(RoutedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Raises the <see cref="E:AfterItemSort"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnAfterItemSort(RoutedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Raises the <see cref="E:ExpandingEvent"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnExpanding(RoutedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Raises the <see cref="E:CollapsingEvent"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCollapsing(RoutedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Raises the <see cref="E:ExpandedEvent"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnExpanded(RoutedEventArgs e)
        {
            base.RaiseEvent(e);

            if (modelItems != null)
            {
                for (int i = 0; i < modelItems.Count; i++)
                {
                    if (modelItems[i].iTree == (e.Source as TreeViewItemAdv).Header && modelItems[i].treeItems.Count == 0)
                    {
                        foreach (object internalitem in (e.Source as TreeViewItemAdv).Items)
                        {
                            modelItems[i].treeItems.Add(internalitem as IVirtualTree);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:CollapsedEvent"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCollapsed(RoutedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Expandings the specified e.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        internal void AfterExpand(RoutedEventArgs e)
        {
            this.OnExpanded(e);
        }

        /// <summary>
        /// Collapses the specified e.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        internal void AfterCollapse(RoutedEventArgs e)
        {
            this.OnCollapsed(e);
        }

        /// <summary>
        /// Expandings the specified e.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        internal void BeforeExpand(RoutedEventArgs e)
        {
            this.OnExpanding(e);
        }

        /// <summary>
        /// Collapses the specified e.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        internal void BeforeCollapse(RoutedEventArgs e)
        {
            this.OnCollapsing(e);
        }

        /// <summary>
        /// Invoked when an unhandled PreviewMouseRightButtonUp routed event reaches
        /// an element in its route that is derived from this class.
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data.
        /// The event data reports that the Right mouse button was released.</param>
        protected override void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e)
        {
            isRightMouseButtonDown = false;
            if (this.DragMode == DragMode.LeftButton)
            {
                Focus();
            }
            else
            {
                base.OnPreviewMouseRightButtonUp(e);
                TreeViewItemAdv treeitem = GetTreeViewItemFromChildren(e.OriginalSource as FrameworkElement);
                if (treeitem != null && treeitem.Focusable)
                {
                    SelectedFalseItem = null;
                    m_bIsAvailableStartDrag = false;
                }
            }
        }

        /// <summary>
        /// Invoked when an unhandled PreviewMouseLeftButtonDown routed event reaches
        /// an element in its route that is derived from this class.
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data.
        /// The event data reports that the left mouse button was pressed.</param>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            if (this.DragMode == DragMode.LeftButton || this.DragMode == DragMode.Both)
            {
                TreeViewItemAdv treeitem;
                treeitem = GetTreeViewItemFromChildren(e.OriginalSource as FrameworkElement);
                if (treeitem == null && (e.OriginalSource as FrameworkContentElement) != null)
                    treeitem = GetTreeViewItemFromChildren((((e.OriginalSource as FrameworkContentElement).TemplatedParent) as ContentPresenter) as FrameworkElement);

                if (treeitem != null && treeitem.Focusable)
                {
                    if (bringintoviewstatus)
                    {
                        bringintoviewstatus = false;
                    }
                    TreeViewItemAdv item;
                    item = GetTreeViewItemFromChildren(e.OriginalSource as FrameworkElement);
                    if (item == null && (e.OriginalSource as FrameworkContentElement) != null)
                        item = GetTreeViewItemFromChildren((((e.OriginalSource as FrameworkContentElement).TemplatedParent) as ContentPresenter) as FrameworkElement);
                    Expander expander = GetExpanderChildren(e.OriginalSource as FrameworkElement);
                    if (item != null && expander != null
                        && item.Expander != null && item.Expander == expander)
                    {
                        m_expandeditem = item;
                        return;
                    }
                    else
                    {
                        m_expandeditem = null;
                    }

                    if (!e.Handled && item != null && item.IsEnabled)
                    {
                        if ((e.ClickCount % 2) == 0 && item.IsSelected)
                        {
                            isdoubleclick = true;
                            if (item.Items.Count > 0 || item.IsLoadOnDemand)
                                item.IsExpanded = !item.IsExpanded;
                        }

                        m_bIsAvailableStartDrag = true;

                        if (EnvironmentTest.IsSecurityGranted)
                        {
                            m_dragStart = MouseUtils.GetMousePosition(null);
                            m_dragOffset = MouseUtils.GetMousePosition(item.CompleteHeaderElement);
                        }

                        m_clickOnHeader = item.IsMouseOverComplateHeader;
                    }

                    SelectedFalseItem = null;
                }
                
                SelectedFalseItem = null;
            }
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            isRightMouseButtonDown = true;
            base.OnPreviewMouseRightButtonDown(e);
            if (this.DragMode == DragMode.RightButton || this.DragMode == DragMode.Both)
            {
                TreeViewItemAdv treeitem = GetTreeViewItemFromChildren(e.OriginalSource as FrameworkElement);
                if (treeitem != null && treeitem.Focusable)
                {
                    if (bringintoviewstatus)
                    {
                        bringintoviewstatus = false;
                    }
                    TreeViewItemAdv item = GetTreeViewItemFromChildren(e.OriginalSource as FrameworkElement);
                    Expander expander = GetExpanderChildren(e.OriginalSource as FrameworkElement);
                    if (item != null && expander != null
                        && item.Expander != null && item.Expander == expander)
                    {
                        m_expandeditem = item;
                        return;
                    }
                    else
                    {
                        m_expandeditem = null;
                    }

                    if (!e.Handled && item != null && item.IsEnabled)
                    {
                        if ((e.ClickCount % 2) == 0 && item.IsSelected)
                        {
                            item.IsExpanded = !item.IsExpanded;
                        }

                        m_bIsAvailableStartDrag = true;

                        if (EnvironmentTest.IsSecurityGranted)
                        {
                            m_dragStart = MouseUtils.GetMousePosition(null);
                            m_dragOffset = MouseUtils.GetMousePosition(item.CompleteHeaderElement);
                        }

                        m_clickOnHeader = item.IsMouseOverComplateHeader;
                    }

                    SelectedFalseItem = null;
                }
            }
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);
            if (this.DragMode == DragMode.RightButton || this.DragMode == DragMode.Both)
            {
                DragDrop.AddQueryContinueDragHandler(this, QueryContinueDragHandler);
            }
        }

        /// <summary>
        /// Invoked when an unhandled PreviewMouseLeftButtonUp routed event reaches
        /// an element in its route that is derived from this class.
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data.
        /// The event data reports that the left mouse button was released.</param>
        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);
            if (this.DragMode == DragMode.LeftButton || this.DragMode == DragMode.Both)
            {
                TreeViewItemAdv treeitem = GetTreeViewItemFromChildren(e.OriginalSource as FrameworkElement);
                if (treeitem != null && treeitem.Focusable)
                {
                    SelectedFalseItem = null;
                    m_bIsAvailableStartDrag = false;
                    if (MultiColumnEnable)
                    {
                        if (treeitem.IsEditable)
                        {
                            if (!treeitem.IsInEditMode && treeitem.IsSelected)
                                Focus();
                        }
                    }
                }
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
            TreeViewAdv owner = d as TreeViewAdv;

            if (owner != null)
            {
                owner.UpdateVisualStyle(owner.VisualStyle);
            }
        }

        /// <summary>
        /// Calls OnAnimationSpeedChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnAnimationSpeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// This method corrects AnimationSpeed.
        /// </summary>
        /// <param name="d">Object to which this property belongs.</param>
        /// <param name="baseValue">The property whish should be corrected.</param>
        /// <returns>
        /// Corrected value.
        /// </returns>
        private static object CoerceOnAnimationSpeed(DependencyObject d, object baseValue)
        {
            TreeViewItemAdv item = d as TreeViewItemAdv;
            double coerceValue = (double)baseValue;

            if (coerceValue <= 0)
            {
                coerceValue = 1d;
            }

            return coerceValue;
        }

        //Flag for Called when [double click event]
        private bool _isdoubleclick = false;

        private bool _ismdoubleclick = false;

        /// <summary>
        /// Called when [mouse down handler].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private static void OnMouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            TreeViewAdv droptree = sender as TreeViewAdv;
            droptree.droppedobjects.Clear();
            TreeViewItemAdv treeitem;
            treeitem = GetTreeViewItemFromChildren(e.OriginalSource as FrameworkElement);
            if (treeitem == null && (e.OriginalSource as FrameworkContentElement) != null)
                treeitem = GetTreeViewItemFromChildren((((e.OriginalSource as FrameworkContentElement).TemplatedParent) as ContentPresenter) as FrameworkElement);
            if (droptree.m_wasSelectedByMouseClick)
                droptree.m_wasSelectedByMouseClick = false;
            if (treeitem != null && treeitem.Focusable)
            {
                if ((!e.Handled || (e.Source is ICommandSource)) && ((e.ChangedButton == MouseButton.Left) || (e.ChangedButton == MouseButton.Right && sender is TreeViewAdv && (sender as TreeViewAdv).IsSelectOnRightMouseClick))) //(!e.Handled || (e.Source is ICommandSource)) &&
                {
                    TreeViewAdv tree = sender as TreeViewAdv;
                    TreeViewItemAdv item;
                    item = GetTreeViewItemFromChildren(e.OriginalSource as FrameworkElement);
                    if (item == null && (e.OriginalSource as FrameworkContentElement) != null)
                        item = TreeViewAdv.GetTreeViewItemFromChildren((((e.OriginalSource as FrameworkContentElement).TemplatedParent) as ContentPresenter) as FrameworkElement);
                    if (tree != null && item != null)
                    {
                        tree.m_ismouseSelection = true;

                        Expander expander = TreeViewAdv.GetExpanderChildren(e.OriginalSource as FrameworkElement);

                        if (expander != null && item.Expander == expander)
                        {
                            return;
                        }

                        if (!item.IsMouseOverComplateHeader)
                        {
                            e.Handled = false;
                        }

                        if (!item.IsMouseOverExpander)
                        {
                           
                            if (item == null)
                            {
                                item = GetTreeViewItemFromChildren(e.OriginalSource as FrameworkElement);
                            }

                            if (item != null)
                            {
                                if (e.ClickCount == 2)
                                {
                                    tree._isdoubleclick = true;
                                    tree._ismdoubleclick = true;
                                    tree.SetSelectAndFocus(item);
                                }
                                else
                                {
                                    tree._isMouseDownInActive = true;
                                    tree.SetSelectAndFocus(item);
                                }

                                tree._isMouseDownInActive = false;
                                tree.m_wasSelectedByMouseClick = true;
                            }
                        }

                        tree.SelectedFalseItem = null;
                    }
                }
            }
        }

        /// <summary>
        /// Called when [mouse up handler].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private static void OnMouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            TreeViewItemAdv treeitem = GetTreeViewItemFromChildren(e.OriginalSource as FrameworkElement);
            if (treeitem != null && treeitem.Focusable)
            {
                TreeViewAdv tree = sender as TreeViewAdv;
                if (tree != null)
                {
                    if (tree.m_previoustargetitem != null)
                    {
                        tree.m_previoustargetitem.m_topdragLine.Visibility = Visibility.Collapsed;
                        tree.m_previoustargetitem.m_bottomdragLine.Visibility = Visibility.Collapsed;
                        tree.m_previoustargetitem = null;
                    }
                }

                if (e.ChangedButton == MouseButton.Left || (e.ChangedButton == MouseButton.Right && sender is TreeViewAdv && (sender as TreeViewAdv).IsSelectOnRightMouseClick))
                {
                    if (tree != null)
                    {
                        if (!e.Handled && !tree.m_wasSelectedByMouseClick && !((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control))
                        {
                            Point p = Mouse.GetPosition(tree.ScrollHost);
                            TreeViewItemAdv item = GetTreeViewItemFromChildren(e.OriginalSource as FrameworkElement);

                            if (item == null)
                            {
                                item = tree.FindItemByPoint(p);
                            }

                            if (item != null)
                            {
                                tree.SetSelectAndFocus(item);
                            }
                        }
                        if (!e.Handled)
                        {
                            Point p = Mouse.GetPosition(tree.ScrollHost);
                            TreeViewItemAdv item = GetTreeViewItemFromChildren(e.OriginalSource as FrameworkElement);

                            if (item == null)
                            {
                                item = tree.FindItemByPoint(p);
                            }

                            if (!((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) && !((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift))
                            {
                                if (tree.SelectedItems.Count > 1)
                                {
                                    if (item != null)
                                    {
                                        if (!(e.ChangedButton == MouseButton.Right && (tree.ContextMenu != null || item.ContextMenu != null)))
                                            item.Select(true);
                                        tree.m_ismouseSelection = false;
                                        tree.m_wasSelectedByMouseClick = false;
                                    }
                                }
                            }
                            else
                            {
                                if (item != null && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                                {
                                    if (!tree._ismdoubleclick)
                                    {
                                        tree.SetSelectAndFocus(item);
                                    }
                                    else
                                    {
                                        tree._ismdoubleclick = false;
                                    }
                                    tree.m_ismouseSelection = false;
                                    tree.m_wasSelectedByMouseClick = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when [context menu closing handler].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.ContextMenuEventArgs"/> instance containing the event data.</param>
        private static void OnContextMenuClosingHandler(object sender, ContextMenuEventArgs e)
        {
            TreeViewAdv tree = sender as TreeViewAdv;
            tree.SelectedFalseItem = null;
        }

        /// <summary>
        /// Invoked when an unhandled ContextMenuOpening routed event reaches
        /// an element in its route that is derived from this class.
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.ContextMenuEventArgs"/> instance containing the event data.</param>
        private static void OnContextMenuOpeningHandler(object sender, ContextMenuEventArgs e)
        {
            TreeViewAdv tree = sender as TreeViewAdv;
            TreeViewItemAdv item = TreeViewAdv.GetTreeViewItemFromChildren(e.OriginalSource as FrameworkElement);

            if (tree != null && item != null && tree.ContextMenu != null)
            {
                tree.SelectedFalseItem = item;
            }
        }

        /// <summary>
        /// Invoked when an unhandled ExpandedEvent and CollapsedEvent routed event reaches
        /// an element in its route that is derived from this class.
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ExpandedCollapsedHandler(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Invoked when to the KeyDown event is received.
        /// </summary>
        /// <param name="e">Information about the event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (!e.Handled)
            {
                switch (e.Key)
                {
                    case Key.Up:
                    case Key.Down:
                        {
                            m_bUpdownNavigation = true;
                            IInputElement focusedElement = FocusManager.GetFocusedElement(this);

                            if (focusedElement == null && SelectedItem == null && SelectedItems.Count == 0)
                            {
                                FocusFirstItem();
                                e.Handled = true;
                            }

                            break;
                        }

                    default:
                        {
                            m_bUpdownNavigation = false;
                            count = 0;
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Returns a String that represents the current Object.
        /// </summary>
        /// <returns>A String that represents the current Object.</returns>
        public override string ToString()
        {
            string value = base.ToString();
            value = "Root";
            return value;
        }

        /// <summary>
        /// Gets type of the items panel.
        /// </summary>
        /// <returns>Type value </returns>
        protected virtual Type GetItemsPanelType()
        {
            return typeof(TreeViewAdvItemsPanel);
        }

        /// <summary>
        /// Gets visual root.
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <returns>Visual value</returns>
        internal static Visual GetVisualRoot(DependencyObject d)
        {
            if ((d is Visual) || (d is Visual3D))
            {
                PresentationSource source = PresentationSource.FromVisual(d as Visual);

                if (source != null)
                {
                    return source.RootVisual;
                }
            }
            else
            {
                FrameworkContentElement element = d as FrameworkContentElement;

                if (element != null)
                {
                    return GetVisualRoot(element.Parent);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets items panel for owner.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="panelType">Type of the panel.</param>
        /// <returns>Panel value </returns>
        internal static Panel GetItemsPanel(ItemsControl owner, Type panelType)
        {
            Panel panel = null;

            if (owner != null && panelType != null)
            {
                Panel currentPanel = null;

                foreach (Visual visual in VisualUtils.EnumChildrenOfType(owner, panelType))
                {
                    currentPanel = visual as Panel;

                    if (currentPanel != null
                        && TreeViewAdv.GetItemsControlFromChildren(currentPanel) == owner)
                    {
                        panel = currentPanel;
                        break;
                    }
                }
            }

            return panel;
        }

        /// <summary>
        /// Scrolls TreeViewAdv page to bottom and selects last item.
        /// </summary>
        /// <param name="e">KeyEvent Args</param>
        internal void ScrollToEndHandle(KeyEventArgs e)
        {
            ScrollTo_StartOrEnd(false);

            if (FocusLastItem())
            {
                e.Handled = true;
            }
            else
            {
                object item = GetLastItem();
                TreeViewItemAdv lastItem = null;

                foreach (object obj in Items)
                {
                    TreeViewItemAdv parent = (obj is TreeViewItemAdv) ? ((TreeViewItemAdv)obj)
                        : ItemContainerGenerator.ContainerFromItem(obj) as TreeViewItemAdv;

                    if (parent != null)
                    {
                        lastItem = parent.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItemAdv;

                        if (lastItem != null)
                        {
                            break;
                        }
                    }
                }

                if (lastItem != null)
                {
                    lastItem.Select(true);
                    this.SelectedContainer.Focus();
                }
            }
        }

        /// <summary>
        /// Scrolls TreeViewAdv page to top and selects first item.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        internal void ScrollToHomeHandle(KeyEventArgs e)
        {
            ScrollTo_StartOrEnd(true);

            if (FocusFirstItem())
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Scrolls the to_ start or end.
        /// </summary>
        /// <param name="start">if set to <c>true</c> [start].</param>
        private void ScrollTo_StartOrEnd(bool start)
        {
            ScrollViewer host = ScrollHost;

            if (start)
            {
                host.ScrollToHome();
                scrollToHome = true;
            }
            else
            {
                host.ScrollToEnd();
                scrollToEnd = true;
            }
        }

        /// <summary>
        /// Returns the first adorner layer in the visual tree above a specified Visual
        /// except for ScrollContentPresenter.
        /// </summary>
        /// The visual element for which to find an adorner layer.
        /// <returns>An adorner layer for the specified visual,
        /// or null if no adorner layer can be found.</returns>
        internal AdornerLayer GetAdornerLayer()
        {
            AdornerLayer layer = null;

            if (m_itemsHost != null)
            {
                layer = AdornerLayer.GetAdornerLayer(m_itemsHost);
            }

            return layer;
        }

        /// <summary>
        /// Gets container with fake items that dragging.
        /// </summary>
        /// <returns>ItemsControl value </returns>
        internal ItemsControl GetDraggingContainer()
        {
            ItemsControl container = null;
            if (DraggingContainer == null)
            {
                if (m_draggingItmes != null && m_draggingItmes.Count > 0 || SelectedItems.Count > 0)
                {
                    container = new ItemsControl();
                    container.Opacity = this.DraggingContainerOpacity;
                    Rectangle rect = null;
                    TreeViewItemAdv item = null;
                    Size conteinerSize = new Size(0, 0);
                    if (IsVirtualizing && VirtualizationMode == VirtualizationMode.Extended)
                    {
                        for (int i = 0; i < SelectedItems.Count; i++)
                        {
                            {
                                try
                                {
                                    DataTemplate draggingtemplate = this.FindResource("draggingContainerTemplate") as DataTemplate;
                                    if (draggingtemplate != null)
                                    {
                                        rect = new Rectangle();
                                        Grid panel = draggingtemplate.LoadContent() as Grid;
                                        TextBlock tblock = VisualUtils.FindDescendant(panel, typeof(TextBlock)) as TextBlock;
                                        tblock.Text = this.SelectedItems.Count.ToString();
                                        rect.Height = 50;
                                        rect.Width = 50;
                                        rect.Fill = new VisualBrush(panel);
                                        container.Items.Add(rect);
                                        break;
                                    }
                                }
                                catch
                                { }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < m_draggingItmes.Count; i++)
                        {
                            item = m_draggingItmesContiners[i];
                            if (item != null && item.CompleteHeaderElement != null)
                            {
                                rect = new Rectangle();
                                rect.Height = item.CompleteHeaderElement.ActualHeight;
                                rect.Fill = GetItemBrush(item);
                                rect.Width = 0;
                                if (MultiColumnEnable)
                                {
                                    if (dragvisualborder.Width != 0)
                                    {
                                        rect.Width = dragvisualborder.Width;
                                        if (DragAdornerWidth != 0)
                                        {
                                            if (rect.Width != 0 && rect.Width > DragAdornerWidth)
                                            {
                                                rect.Width = DragAdornerWidth;
                                            }
                                        }
                                        visualtext.Width = rect.Width;
                                        dragvisualborder.Child = visualtext;
                                    }

                                    if (dragvisualborder.Width == 0 && rect.Width == 0)
                                    {
                                        rect.Width = item.CompleteHeaderElement.ActualWidth;
                                    }
                                }
                                else
                                {
                                    rect.Width = item.CompleteHeaderElement.ActualWidth;
                                }

                                container.Items.Add(rect);
                                conteinerSize.Height += rect.Height;

                                if (conteinerSize.Width < rect.Width)
                                {
                                    conteinerSize.Width = rect.Width;
                                }
                            }
                        }
                    }
                    if (VirtualizationMode == VirtualizationMode.Extended)
                        return container;

                    container.Width = conteinerSize.Width;
                    container.Height = conteinerSize.Height;
                }
            }
            else
            {
                container = DraggingContainer;
                container.Opacity = this.DraggingContainerOpacity;
            }

            return container;
        }

        internal void RemoveItemFromContainer(TreeViewItemAdv movedElement)
        {
            if (m_selectedContainers.Contains(movedElement))
            {
                m_selectedContainers.Remove(movedElement);
            }
        }

        /// <summary>
        /// Changes selection.
        /// </summary>
        /// <param name="data">The data selection.</param>
        /// <param name="container">The container value.</param>
        /// <param name="selected">if set to <c>true</c> [selected].</param>
        /// <param name="bRandom">if set to <c>true</c> [b random].</param>
        internal void ChangeSelection(object data, TreeViewItemAdv container, bool selected, bool bRandom)
        {
            if (!this.IsSelectionChangeActive
                && !(IsDragging && Keyboard.Modifiers == ModifierKeys.Shift))
            {
                this.IsSelectionChangeActive = true;
                object oldValue = SelectedItem;
                IList oldvalues = new List<object>();

                if (SelectedTreeViewItems != null)
                foreach (object obj in SelectedTreeViewItems)
                {
                    TreeViewItemAdv newobj = obj as TreeViewItemAdv;
                    if (isLinearItemsChanged)
                    {
                        if (newobj.DataContext != null && newobj.m_treeviewitemactualobject != null && this.ItemsSource != null && this.Items.Contains(newobj.DataContext))
                        {
                            if (!oldvalues.Contains(newobj.m_treeviewitemactualobject))
                                oldvalues.Add(newobj.m_treeviewitemactualobject);
                        }
                        else
                        {
                            if (newobj.ParentTreeViewItem != null)
                            {
                                if (newobj.DataContext != null && newobj.m_treeviewitemactualobject != null && this.ItemsSource != null)
                                {
                                    if (!oldvalues.Contains(newobj.m_treeviewitemactualobject))
                                        oldvalues.Add(newobj.m_treeviewitemactualobject);
                                }
                            }
                        }
                    }
                    else if (newobj.DataContext != null && newobj.m_treeviewitemactualobject != null && this.ItemsSource != null && !this.Items.Contains(newobj.DataContext))
                    {
                        if (!oldvalues.Contains(newobj.m_treeviewitemactualobject))
                            oldvalues.Add(newobj.m_treeviewitemactualobject);
                    }
                    else
                    {
                        if (this.ItemsSource != null && newobj.m_treeviewitemactualobject != null)
                        {
                            if (!oldvalues.Contains(newobj.m_treeviewitemactualobject))
                                oldvalues.Add(newobj.m_treeviewitemactualobject);
                        }
                        else
                        {
                            if (!oldvalues.Contains(newobj))
                                oldvalues.Add(newobj);
                        }
                    }
                }
                if (SelectedTreeViewItems != null && (SelectedTreeViewItems.Count <= 0 || SelectedTreeViewItems.Count < this.oldselectedItems.Count))
                {
                    oldvalues.Clear();
                    if (this.oldselectedItems.Count > 0)
                        oldvalues = this.oldselectedItems;
                }
                else if (oldvalues.Count <= oldselectedItems.Count && oldselectedItems.Count > 0)
                {
                    oldvalues.Clear();
                    if (this.oldselectedItems.Count > 0)
                        oldvalues = this.oldselectedItems;
                }
                if (!AllowMultiSelect && oldValue != null)
                {
                    oldvalues.Clear();
                    oldvalues.Add(oldValue);
                }
                object newValue = data;

                if (oldValue == newValue)
                {
                    if (oldvalues.Contains(oldValue) && SelectedItems.Count > 0 && Keyboard.Modifiers != ModifierKeys.Control && Keyboard.Modifiers != ModifierKeys.Shift && !issingleselection)
                        oldvalues.Clear();
                    if (SelectedItems.Count > 0)
                        oldValue = SelectedItems[0];
                    if (issingleselection)
                        issingleselection = false;
                }

                RoutedPropertyChangedEventArgs<object> e = new RoutedPropertyChangedEventArgs<object>(
                    oldValue, newValue, PreviewSelectedItemChangedEvent);
                OnPreviewSelectedItemChanged(e);

                try
                {
                    if (!AllowMultiSelect || ForceSingleSelection)
                    {
                        SingleSelection(data, container, selected);
                    }
                    else if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    {
                        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                        {
                            if (m_startSelectContainer == null || (!m_startSelectContainer.IsSelected && selected))
                            {
                                if (!container.IsSelected)
                                {
                                    selected = false;
                                }
                                else
                                {
                                    m_startSelectContainer = container;
                                    if (container != null)
                                        m_startIndex = this.ItemContainerGenerator.IndexFromContainer(container);
                                }
                                SegmentSelection(data, container, selected);
                            }
                            externalSelect = false;
                            canExecuteshift = true;
                            SequentialSelection(data, container, selected);
                        }
                        else
                        {
                            if (m_startSelectContainer == null)
                            {
                                m_startSelectContainer = GetItem(0);
                            }
                            if (!externalSelect && !isRightMouseButtonDown)
                                SequentialSelection(data, container, selected);
                            else if (selected)
                            {
                                externalSelect = false;
                                SegmentSelection(data, container, selected);
                            }
                        }
                    }
                    else if (!(SelectedItem == null && SelectedItems.Count == 0)
                        && (((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control &&
                        !Keyboard.IsKeyDown(Key.Home) && !Keyboard.IsKeyDown(Key.End)) || bRandom))
                    {
                        if (!m_bUpdownNavigation || bRandom)
                        {
                            RandomSelection(data, container, selected);
                        }
                    }
                    else if (container != null && AllowMultiSelect && !m_ismouseSelection && droppedobjects != null && !droppedobjects.Contains(data))
                    {
                        if (!m_selectedInCollectionChanged && !isSelectedFromPrepareContainer && isSelectionbymouseclick)
                            SingleSelection(data, container, selected);
                        else
                            RandomSelection(data, container, selected);
                    }
                    else
                    {
                        if (!m_selectedInCollectionChanged && !isSelectedFromPrepareContainer && droppedobjects != null && !droppedobjects.Contains(data))
                            SingleSelection(data, container, selected);
                        if (m_selectedInCollectionChanged)
                            m_selectedInCollectionChanged = false;
                    }
                }
                finally
                {
                    this.IsSelectionChangeActive = false;
                }

                if (selected)
                {
                    newValue = container.Header;
                    if (oldvalues.Contains(newValue))
                        oldvalues.Remove(newValue);
                    if (externalSelect && !IsMultiselection)
                        RandomSelection(data, container, selected);
                }
                else
                {
                    if (SelectedItems.Count > 0)
                    {
                        newValue = SelectedItems[0];
                    }
                    else
                    {
                        newValue = null;
                    }
                }

                allowSelectedTreeItem = false;
                this.SelectedTreeItem = data;
                allowSelectedTreeItem = true;

                if (!UnSubscribeSelectionChangedEvent)
                {
                    RoutedPropertyChangedEventArgs<object> eargs = new RoutedPropertyChangedEventArgs<object>(
               oldValue, newValue, SelectedItemChangedEvent);
                    OnSelectedItemChanged(eargs);
                }

                SelectionChangedEventArgs args = new SelectionChangedEventArgs(
                    SelectionChangedEvent, RemovedItemsList(oldvalues, SelectedItems), AddedItemsList(oldvalues, SelectedItems));

                if (addeditemscount == 0 && removeditemscount == 0)
                    UnSubscribeSelectionChangedEvent = true;
                if (!UnSubscribeSelectionChangedEvent)
                    OnSelectionChanged(args);
                if (UnSubscribeSelectionChangedEvent)
                    UnSubscribeSelectionChangedEvent = false;
                oldvalues.Clear();
                oldvalues = null;
            }
        }

        /// <summary>
        /// Removeds the items list.
        /// </summary>
        /// <param name="oldvalues">The oldvalues.</param>
        /// <param name="Selecteditems">The selecteditems.</param>
        /// <returns></returns>
        private IList RemovedItemsList(IList oldvalues, IList Selecteditems)
        {
            IList removedlist = new List<object>();
            foreach (object olditem in oldvalues)
            {
                if (!SelectedItems.Contains(olditem))
                    removedlist.Add(olditem);
            }
            removeditemscount = removedlist.Count;
            return removedlist;
        }

        /// <summary>
        /// Addeds the items list.
        /// </summary>
        /// <param name="oldvalues">The oldvalues.</param>
        /// <param name="Selecteditems">The selecteditems.</param>
        /// <returns></returns>
        private IList AddedItemsList(IList oldvalues, IList Selecteditems)
        {
            IList addedlist = new List<object>();
            foreach (object newitem in Selecteditems)
            {
                if (!addedlist.Contains(newitem) && !oldvalues.Contains(newitem))
                    addedlist.Add(newitem);
                else if (addedlist.Count == 0 && !oldvalues.Contains(newitem) && Keyboard.Modifiers != ModifierKeys.Control && Keyboard.Modifiers != ModifierKeys.Shift)
                    addedlist.Add(newitem);
            }
            addeditemscount = addedlist.Count;
            return addedlist;
        }

        //// Un Use Code-->It got through N-Cover;

        /// <summary>
        /// Clears the selection.
        /// </summary>
        public void ClearSelection()
        {
            this.IsSelectionChangeActive = true;
            for (int i = m_selectedContainers.Count - 1; i >= 0; i--)
            {
                m_selectedContainers[i].IsSelected = false;
            }
            m_selectedContainers.Clear();
            m_startSelectContainer = null;
            SelectedItems.Clear();
            SelectedTreeItem = null;
            SetSelectedItem(null);
            UpdateSelectedValue(null);
            SelectedTreeItemObject = null;

            this.IsSelectionChangeActive = false;
        }

        /// <summary>
        /// Handles IsExpanded.
        /// </summary>
        /// <param name="collapsed">The collapsed.</param>
        internal void HandleSelectionAndCollapsed(TreeViewItemAdv collapsed)
        {
            TreeViewItemAdv parentTreeViewItem = m_selectedContainer;

            for (int i = m_selectedContainers.Count - 1; i >= 0; i--)
            {
                TreeViewItemAdv item = m_selectedContainers[i];

                if (item != null)
                {
                    if (ItemsSource != null ? IsInChildren(collapsed, item, true) : IsInChildren(collapsed, item, false))
                    {
                        if (collapsed != item)
                        {
                            IsSelectionChangeActive = true;
                            if (item.ParentItemsControl != null)
                            {
                                RandomSelection(item.ParentItemsControl.ItemContainerGenerator.ItemFromContainer(item), item, false);
                                item.UpdateLayout();
                            }
                            IsSelectionChangeActive = false;
                        }
                    }
                }
            }

            if (parentTreeViewItem != null && collapsed != null && !collapsed.IsSelected)
            {
                do
                {
                    parentTreeViewItem = parentTreeViewItem.ParentTreeViewItem;

                    if (parentTreeViewItem == collapsed)
                    {
                        ChangeSelection(collapsed.ParentItemsControl.ItemContainerGenerator.ItemFromContainer(collapsed), collapsed, true, false);
                        return;
                    }
                }
                while (parentTreeViewItem != null);
            }
        }

        /// <summary>
        /// Gets true if top, bottom, right or left key was down.
        /// </summary>
        /// <returns>bool value </returns>
        internal bool WasNavigationKeyDown()
        {
            return Keyboard.IsKeyDown(Key.Up) || Keyboard.IsKeyDown(Key.Down)
                || Keyboard.IsKeyDown(Key.Left) || Keyboard.IsKeyDown(Key.Right);
        }

        /// <summary>
        /// Finds parent TreeViewAdv for given element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>TreeViewAdv value </returns>
        internal static TreeViewAdv GetTreeViewFromChildren(FrameworkElement element)
        {
            TreeViewAdv item = null;

            if (element != null)
            {
                if (element is TreeViewItemAdv)
                {
                    item = ((TreeViewItemAdv)element).ParentTreeView;
                }
                else
                {
                    item = element as TreeViewAdv;
                }

                if (item == null)
                {
                    item = VisualUtils.FindAncestor(element, typeof(TreeViewAdv)) as TreeViewAdv;
                }
            }

            return item;
        }

        private bool m_IsDraggedItemIsTreeViewItemAdv = true;
        /// <summary>
        /// Finds parent TreeViewItemAdv for given element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>TreeviewAdv value </returns>
        public static TreeViewItemAdv GetTreeViewItemFromChildren(FrameworkElement element)
        {
            TreeViewItemAdv item = null;
            bool isItemScrollViewer = false;

            if (element != null)
            {
                item = element as TreeViewItemAdv;

                if (item == null)
                {
                    while (element != null)
                    {
                        element = VisualTreeHelper.GetParent(element) as FrameworkElement;

                        if (element is TreeViewItemAdv)
                        {
                            TreeViewAdvVirtualizingPanel.m_scrollmovemanually = false;
                            item = (TreeViewItemAdv)element;
                            break;
                        }
                        if (element is ScrollBar)
                        {
                            TreeViewAdvVirtualizingPanel.m_scrollmovemanually = true;
                        }
                        if (element is ScrollViewer)
                        {
                            isItemScrollViewer = true;
                        }
                    }
                }
            }

            if (item != null)
            {
                var treeViewAdv = GetTreeViewFromChildren(item);
                if (treeViewAdv != null)
                {
                    if (isItemScrollViewer)
                    {
                        treeViewAdv.m_IsDraggedItemIsTreeViewItemAdv = false;
                    }
                    else
                    {
                        treeViewAdv.m_IsDraggedItemIsTreeViewItemAdv = true;
                    }
                }
            }
            return item;
        }

        internal static TreeViewItemAdv GetTreeViewItemFromChildrenInDescendantMode(FrameworkElement element)
        {
            TreeViewItemAdv treeViewItemAdv;
            treeViewItemAdv = VisualUtils.FindDescendant(element as Visual, typeof(TreeViewItemAdv)) as TreeViewItemAdv;
            if(treeViewItemAdv !=null)
            {
                HitTestResult testresult = VisualTreeHelper.HitTest(treeViewItemAdv, Mouse.GetPosition(treeViewItemAdv));
                if(testresult!=null)
                {
                    return treeViewItemAdv;
                }
                else
                {
                    GetTreeViewItemFromChildrenInDescendantMode(treeViewItemAdv);
                }
            }
            return treeViewItemAdv;
        }

        /// <summary>
        /// Finds parent TreeViewItemAdv for given element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>TreeviewAdv value </returns>
        public static ContentPresenter GetPresenterFromChildren(FrameworkElement element)
        {
            ContentPresenter item = null;

            if (element != null)
            {
                item = element as ContentPresenter;

                if (item == null)
                {
                    while (element != null)
                    {
                        element = VisualTreeHelper.GetParent(element) as FrameworkElement;

                        if (element is ContentPresenter)
                        {
                            item = (ContentPresenter)element;
                            break;
                        }
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Finds expander for given element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>Expander value </returns>
        internal static Expander GetExpanderChildren(FrameworkElement element)
        {
            Expander expander = null;

            if (element != null)
            {
                expander = element as Expander;

                if (expander == null)
                {
                    while (element != null)
                    {
                        element = VisualTreeHelper.GetParent(element) as FrameworkElement;

                        if (element is Expander)
                        {
                            expander = (Expander)element;
                            break;
                        }
                    }
                }
            }

            return expander;
        }

        /// <summary>
        /// Finds parent TreeViewItemAdv for given element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>Items control value </returns>
        internal static ItemsControl GetItemsControlFromChildren(FrameworkElement element)
        {
            ItemsControl item = null;

            if (element != null)
            {
                item = element as ItemsControl;

                if (item == null)
                {
                    while (element != null)
                    {
                        element = VisualTreeHelper.GetParent(element) as FrameworkElement;

                        if (element is ItemsControl)
                        {
                            item = (ItemsControl)element;
                            break;
                        }
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Finds parent page for given element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>Page value </returns>
        internal static Page GetPageFromChildren(FrameworkElement element)
        {
            Page page = null;

            if (element != null)
            {
                page = element as Page;

                if (page == null)
                {
                    while (element != null)
                    {
                        element = VisualTreeHelper.GetParent(element) as FrameworkElement;

                        if (element is Page)
                        {
                            page = (Page)element;
                            break;
                        }
                    }
                }
            }

            return page;
        }

        /// <summary>
        /// Finds parent window for given element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>Window value </returns>
        internal static Window GetWindowFromChildren(FrameworkElement element)
        {
            Window window = null;

            if (element != null)
            {
                window = element as Window;

                if (window == null)
                {
                    while (element != null)
                    {
                        element = VisualTreeHelper.GetParent(element) as FrameworkElement;

                        if (element is Window)
                        {
                            window = (Window)element;
                            break;
                        }
                    }
                }
            }

            return window;
        }

        /// <summary>
        /// Hides all fake items for specified ItemsControl.
        /// </summary>
        /// <param name="items">The items.</param>
        internal static void HideAllFakeItems(ItemsControl items)
        {
            if (items != null && items.IsVisible)
            {
                IItemsPanelRef panel = items as IItemsPanelRef;

                if (panel != null && panel.ItemsPanel != null)
                {
                    panel.ItemsPanel.HideFakeItems();

                    if (panel.ItemsPanel.CountInternalItems > 0)
                    {
                        ItemsControl item = null;

                        for (int i = 0; i < panel.ItemsPanel.CountInternalItems; i++)
                        {
                            item = panel.ItemsPanel.GetInternalItem(i) as ItemsControl;

                            if (item != null)
                            {
                                HideAllFakeItems(item);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Releases drag-and-drop data.
        /// </summary>
        /// <param name="tree">The tree drag drop.</param>
        internal static void ReleaseDragDropData(TreeViewAdv tree)
        {
            if (tree != null)
            {
                TreeViewAdv.HideAllFakeItems(tree);
                tree.InvalidatePanelRender(tree);
            }

            if (TreeViewAdv.DragOverTreeView != null)
            {
                TreeViewAdv dragOverTree = TreeViewAdv.DragOverTreeView;
                TreeViewAdv.HideAllFakeItems(dragOverTree);
                TreeViewAdvItemsPanel.RemoveFakeItems();
                TreeViewAdv.DragOverTreeView = null;
            }
        }

        /// <summary>
        /// Invalidates measure of the items panel.
        /// </summary>
        /// <param name="items">The items.</param>
        internal void InvalidatePanelMeasure(ItemsControl items)
        {
            if (items != null && items.Visibility != Visibility.Collapsed)
            {
                IItemsPanelRef panel = items as IItemsPanelRef;
                TreeViewAdvItemsPanel itemsPanel = (panel != null) ? panel.ItemsPanel : null;

                if (panel != null && itemsPanel != null)
                {
                    if (itemsPanel.Children.Count > 0)
                    {
                        TreeViewItemAdv item = null;

                        for (int i = 0; i < itemsPanel.Children.Count; i++)
                        {
                            item = itemsPanel.GetInternalItem(i) as TreeViewItemAdv;

                            if (item != null)
                            {
                                if (!item.IsLoaded)
                                {
                                    item.ApplyTemplate();
                                }

                                InvalidatePanelMeasure(item);
                            }
                        }
                    }

                    itemsPanel.InvalidateMeasure();
                    itemsPanel.UpdateLayout();
                }
            }
        }

        /// <summary>
        /// Retrive item from the container
        /// </summary>
        /// <param name="items">The items.</param>
        public TreeViewItemAdv GetContainerFromItem(object item)
        {
            TreeViewItemAdv treeviewitemcontainer = ItemContainerGenerator.ContainerFromItem(item) as TreeViewItemAdv;
            if (treeviewitemcontainer != null)
            {
                return treeviewitemcontainer;
            }
            else
            {
                if (m_treeviewadv != null)
                {
                    if (m_treeviewadv.Count > 0)
                    {
                        for (int i = 0; i < m_itemObject.Count; i++)
                        {
                            if (m_itemObject[i].Equals(item))
                            {
                                return m_treeviewadv[i];
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Invalidates render of the items panel.
        /// </summary>
        /// <param name="items">The items.</param>
        internal void InvalidatePanelRender(ItemsControl items)
        {
            if (items != null && items.IsVisible)
            {
                IItemsPanelRef panel = items as IItemsPanelRef;

                if (panel != null && panel.ItemsPanel != null)
                {
                    panel.ItemsPanel.InvalidateRender();

                    if (panel.ItemsPanel.Children.Count > 0)
                    {
                        TreeViewItemAdv item = null;

                        for (int i = 0; i < panel.ItemsPanel.Children.Count; i++)
                        {
                            item = panel.ItemsPanel.GetInternalItem(i) as TreeViewItemAdv;

                            if (item != null)
                            {
                                InvalidatePanelRender(item);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the item by index.
        /// </summary>
        /// <param name="index">Index of the item.</param>
        /// <returns>Tree View Item Adv</returns>
        internal TreeViewItemAdv GetItem(int index)
        {
            TreeViewItemAdv item = null;
            IItemContainer itemContainer = this as IItemContainer;

            if (itemContainer != null && index > -1)
            {
                item = itemContainer.GetItem(index);
            }

            return item;
        }

        /// <summary>
        /// Clear all item for internal panel.
        /// </summary>
        /// <param name="items">The items.</param>
        private static void InternalPanelClearAllItems(ItemsControl items)
        {
            if (items != null && items.IsVisible)
            {
                IItemsPanelRef panel = items as IItemsPanelRef;

                if (panel != null && panel.ItemsPanel != null && items.ItemsSource != null)
                {
                    panel.ItemsPanel.ClearAllItems();
                }
            }
        }

        internal static Visibility m_dragbordervisibility = Visibility.Visible;

        internal static bool IsWindowsXP = false;

        /// <summary>
        /// Sets the properties.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        private static void SetProperties(object source, object target)
        {
            if (!double.IsNaN((source as Border).Height))
            {
                (target as Border).Height = (source as Border).Height;
            }
            if (!double.IsNaN((source as Border).Width))
            {
                (target as Border).Width = (source as Border).Width;
            }
            if (!double.IsNaN((source as Border).MaxHeight))
            {
                (target as Border).MaxHeight = (source as Border).MaxHeight;
            }
            if (!double.IsNaN((source as Border).MaxWidth))
            {
                (target as Border).MaxWidth = (source as Border).MaxWidth;
            }
            if (!double.IsNaN((source as Border).MinHeight))
            {
                (target as Border).MinHeight = (source as Border).MinHeight;
            }
            if (!double.IsNaN((source as Border).MinWidth))
            {
                (target as Border).MinWidth = (source as Border).MinWidth;
            }
            (target as Border).AllowDrop = (source as Border).AllowDrop;
            (target as Border).Background = (source as Border).Background;
            (target as Border).BorderBrush = (source as Border).BorderBrush;
            (target as Border).BorderThickness = (source as Border).BorderThickness;
            (target as Border).Clip = (source as Border).Clip;
            (target as Border).ClipToBounds = (source as Border).ClipToBounds;
            (target as Border).ContextMenu = (source as Border).ContextMenu;
            (target as Border).CornerRadius = (source as Border).CornerRadius;
            (target as Border).FlowDirection = (source as Border).FlowDirection;
            (target as Border).Focusable = (source as Border).Focusable;
            (target as Border).IsEnabled = (source as Border).IsEnabled;
            (target as Border).IsHitTestVisible = (source as Border).IsHitTestVisible;
            (target as Border).SnapsToDevicePixels = (source as Border).SnapsToDevicePixels;
            if ((source as Border).Visibility == Visibility.Collapsed)
            {
                m_dragbordervisibility = Visibility.Collapsed;
            }
            else
            {
                m_dragbordervisibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Shows fake items for specified ItemsControl.
        /// </summary>
        /// <param name="items">The items.</param>
        private static void ShowFakeItem(ItemsControl items)
        {
            if (items != null && TreeViewAdv.DragOverTreeView != null)
            {
                TreeViewItemAdv targetItem = items as TreeViewItemAdv;
                ItemsControl parentItemsControl = (targetItem == null) ?
                items : targetItem.ParentItemsControl;
                Border bottomline, topline = null;
                bool IsLastItem = false;

                System.OperatingSystem osInfo = System.Environment.OSVersion;

                switch (osInfo.Platform)
                {
                    case System.PlatformID.Win32NT:

                        switch (osInfo.Version.Major)
                        {
                            case 5:
                                if (osInfo.Version.Minor == 0)
                                    IsWindowsXP = false;
                                else
                                    IsWindowsXP = true;
                                break;
                        }

                        break;
                }

                if (targetItem == null)
                {
                    targetItem = items.ItemContainerGenerator.ContainerFromIndex(items.Items.Count - 1) as TreeViewItemAdv;

                    if (targetItem == null && items.Items.Count > 0)
                    {
                        targetItem = items.Items[items.Items.Count - 1] as TreeViewItemAdv;
                    }
                    if (IsWindowsXP)
                    {
                        bottomline = targetItem.m_bottomdragLine as Border;
                        if (bottomline != null)
                        {
                            IsLastItem = true;
                            bottomline.Visibility = Visibility.Visible;
                            topline = targetItem.m_topdragLine as Border;
                            if (topline != null)
                            {
                                topline.Visibility = Visibility.Collapsed;
                            }
                        }
                    }
                }

                if (TreeViewAdv.DragOverTreeView.IsFakeDragIndicator)
                {
                    if (parentItemsControl != null)
                    {
                        IItemsPanelRef panel = parentItemsControl as IItemsPanelRef;

                        if (panel != null && panel.ItemsPanel != null)
                        {
                            bool bShowTop = targetItem != null && targetItem.IsShowTopMarker;
                            panel.ItemsPanel.ShowFakeItems(targetItem, bShowTop);
                        }
                    }
                }
                else if (targetItem != null)
                {
                    if (IsWindowsXP)
                    {
                        Border b = new Border();
                        bool m_borderstyleapply = true;
                        try
                        {
                            b.Style = targetItem.ParentTreeView.DragIndicatorStyle;
                        }
                        //SU I78477
                        //catch (Exception e)
                        catch (Exception)
                        //EU I78477
                        {
                            m_borderstyleapply = false;
                        }
                        topline = targetItem.m_topdragLine as Border;
                        if (m_borderstyleapply)
                        {
                            SetProperties(b, topline);
                        }
                        if (topline != null && m_dragbordervisibility == Visibility.Visible)
                        {
                            if (targetItem.m_bottomdragLine != null)
                            {
                                try
                                {
                                    b.Style = targetItem.ParentTreeView.DragIndicatorStyle;
                                    SetProperties(b, targetItem.m_bottomdragLine);
                                }
                                //SU I78477
                                //catch (Exception e)
                                catch (Exception)
                                //EU I78477
                                {
                                }
                                if (!IsLastItem && targetItem.m_bottomdragLine.Visibility.Equals(Visibility.Visible))
                                {
                                    targetItem.m_bottomdragLine.Visibility = Visibility.Collapsed;
                                }
                                if (targetItem.m_bottomdragLine.Visibility.Equals(Visibility.Collapsed))
                                {
                                    topline.Visibility = Visibility.Visible;
                                }
                            }
                        }

                        if (targetItem.ParentTreeView != null)
                        {
                            if (targetItem.ParentTreeView.m_previoustargetitem == null)
                            {
                                targetItem.ParentTreeView.m_previoustargetitem = targetItem;
                            }
                            else
                            {
                                if (targetItem.ParentTreeView.m_previoustargetitem != targetItem)
                                {
                                    if (targetItem.ParentTreeView.m_previoustargetitem.m_topdragLine != null)
                                    {
                                        targetItem.ParentTreeView.m_previoustargetitem.m_topdragLine.Visibility = Visibility.Collapsed;
                                    }

                                    if (targetItem.ParentTreeView.m_previoustargetitem.m_bottomdragLine != null)
                                    {
                                        targetItem.ParentTreeView.m_previoustargetitem.m_bottomdragLine.Visibility = Visibility.Collapsed;
                                    }

                                    targetItem.ParentTreeView.m_previoustargetitem = targetItem;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (targetItem.ParentTreeView != null && targetItem.ParentTreeView.DragIndicatorStyle != null)
                        {
                            try
                            {
                                targetItem.AdornerMarker.SetStyle(targetItem.ParentTreeView.DragIndicatorStyle);
                            }
                            //SU I78477
                            //catch (Exception e)
                            catch (Exception)
                            //EU I78477
                            {
                            }
                        }
                        targetItem.AdornerMarker.OffsetY = targetItem.IsShowTopMarker ?
                            0 : targetItem.ActualHeight;
                        targetItem.TryAddAdorner(targetItem.AdornerMarker);
                    }
                }

                TreeViewAdv.DragOverTreeView.InvalidatePanelRender(TreeViewAdv.DragOverTreeView);
            }
        }

        /// <summary>
        /// internal variable which has drop index
        /// </summary>
        internal static int dropIndex = -1;

        /// <summary>
        /// Hide fake items for specified ItemsControl.
        /// </summary>
        /// <param name="items">The items.</param>
        private static void HideFakeItem(ItemsControl items)
        {
            if (items != null)
            {
                TreeViewItemAdv targetItem = items as TreeViewItemAdv;
                ItemsControl parentItemsControl = (targetItem == null) ?
                items : targetItem.ParentItemsControl;

                if (parentItemsControl != null)
                {
                    IItemsPanelRef panel = parentItemsControl as IItemsPanelRef;

                    if (panel != null && panel.ItemsPanel != null)
                    {
                        panel.ItemsPanel.HideFakeItems();
                    }
                }

                if (targetItem != null && !targetItem.IsFakeItem)
                {
                    targetItem.TryRemoveAdorner(targetItem.AdornerMarker);
                }
                else if (items is TreeViewAdv)
                {
                    ((TreeViewAdv)items).TryRemoveLastItemAdorner();
                }
            }
        }

        /// <summary>
        /// internal variable which has mouse active flag
        /// </summary>
        private bool _isMouseDownInActive = false;

        private bool isSelectionbymouseclick = false;
        private TreeViewItemAdv olditem;
        private TreeViewItemAdv lastSelectedItem = null;

        /// <summary>
        /// Sets select state and focus for item.
        /// </summary>
        /// <param name="item">The item TreeView.</param>
        internal void SetSelectAndFocus(TreeViewItemAdv item, bool bCheckMouse = true)
        {
            if (item != null)
            {
                if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    if (!_isMouseDownInActive || _isdoubleclick)
                    {
                        item.Select(!item.IsSelected);
                        lastSelectedItem = item;
                        if (_isdoubleclick)
                        {
                            if (olditem != item || olditem == null)
                                item.IsSelected = !item.IsSelected;
                            _isdoubleclick = false;
                            olditem = item;
                        }
                        if (item.IsSelected)
                            FocusManager.SetFocusedElement(this, item);
                    }
                }
                else if (AllowMultiSelect && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    if (!m_ismouseSelection)
                        FocusManager.SetFocusedElement(this, item);
                    item.Select(true);
                    lastSelectedItem = item;
                }
                else
                {
                    FocusManager.SetFocusedElement(this, item);

                    if (item.IsSelected == false)
                    {
                        if (bCheckMouse)
                        {
                            Point p = Mouse.GetPosition(item);
                            isSelectionbymouseclick = true;
                            if (p.X < item.ActualWidth && (item.ItemTemplate != null && p.Y > 1 && p.Y < item.ActualHeight - 2))
                            {
                                item.Select(true);
                                lastSelectedItem = item;
                            }
                            else if (item.ItemTemplate == null && p.Y > 0 && p.X < item.ActualWidth)
                            {
                                item.Select(true);
                                lastSelectedItem = item;
                            }

                            if (p.X > 0 && p.X < item.ActualWidth && !item.IsSelected)
                            {
                                item.Select(true);
                                lastSelectedItem = item;
                            }
                        }
                        else
                        {
                            item.Select(true);
                            lastSelectedItem = item;
                        }
                    }
                    else if (!((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) && !((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift))
                    {
                        isSelectionbymouseclick = true;
                        if (SelectedItems.Count > 1 && !item.IsSelected)
                        {
                            item.Select(true);
                            lastSelectedItem = item;
                        }
                        else if (SelectedItems.Count > 1 && !_isMouseDownInActive)
                        {
                            item.Select(true);
                            lastSelectedItem = item;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles key down event in scroll viewer.
        /// </summary>
        /// <param name="sender">scroll viewer object</param>
        /// <param name="e">KeyEvent Args</param>
        private void ScrollViewer_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.End:
                    {
                        ScrollToEndHandle(e);
                        break;
                    }

                case Key.Home:
                    {
                        ScrollToHomeHandle(e);
                        break;
                    }
            }
        }

        /// <summary>
        /// used for setting the sorting Task in progress
        /// </summary>
        internal static bool IsSortingTaskInProgress = false;

        /// <summary>
        /// Sortings the tree view.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="OldValue">The old value.</param>
        internal void SortingTreeView(DependencyObject d, SortDirection newValue, SortDirection OldValue)
        {
            IsSortingTaskInProgress = true;

            ItemsControl itemsControl = d as ItemsControl;

            if (itemsControl != null &&
                (itemsControl is TreeViewAdv || itemsControl is TreeViewItemAdv))
            {
                SortDirection sortDirection = (SortDirection)newValue;

                if (SortDirection.None != sortDirection)
                {
                    ListSortDirection listSortDirection = SortDirection.Ascending == sortDirection
                        ? ListSortDirection.Ascending : ListSortDirection.Descending;
                    string sortingField = TreeViewAdv.GetSortingField(itemsControl);

                    if (!itemsControl.Items.SortDescriptions.Contains(new SortDescription(sortingField, listSortDirection)))
                    {
                        itemsControl.Items.SortDescriptions.Clear();
                        try
                        {
                            ItemsControl treeviewitemsControl = null;
                            SortModeChangeEventArgs args = new SortModeChangeEventArgs(TreeViewAdv.BeforeItemSortEvent, itemsControl, OldValue, newValue);
                            if (itemsControl is TreeViewItemAdv)
                            {
                                if ((itemsControl as TreeViewItemAdv).ParentTreeView != null)
                                {
                                    treeviewitemsControl = (itemsControl as TreeViewItemAdv).ParentTreeView as ItemsControl;
                                }
                                if (treeviewitemsControl != null)
                                {
                                    (treeviewitemsControl as TreeViewAdv).OnBeforeItemSort(args);
                                }
                            }
                            else
                            {
                                (d as TreeViewAdv).OnBeforeItemSort(args);
                            }
                            if (!args.Cancel)
                            {
                                if (itemsControl.Items.Count > 0)
                                {
                                    itemsControl.Items.SortDescriptions.Add(new SortDescription(sortingField, listSortDirection));
                                }
                            }
                            SortModeChangeEventArgs afterargs = new SortModeChangeEventArgs(TreeViewAdv.AfterItemSortEvent, itemsControl, OldValue, newValue);
                            if (itemsControl is TreeViewItemAdv)
                            {
                                if ((itemsControl as TreeViewItemAdv).ParentTreeView != null)
                                {
                                    treeviewitemsControl = (itemsControl as TreeViewItemAdv).ParentTreeView as ItemsControl;
                                }
                                if (treeviewitemsControl != null)
                                {
                                    (treeviewitemsControl as TreeViewAdv).OnAfterItemSort(afterargs);
                                }
                            }
                            else
                            {
                                (d as TreeViewAdv).OnAfterItemSort(afterargs);
                            }
                            if (afterargs.Cancel)
                            {
                                itemsControl.Items.SortDescriptions.Clear();
                            }
                        }
                        catch (InvalidOperationException ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                    }

                    TreeViewAdv tree = TreeViewAdv.GetTreeViewFromChildren(itemsControl);

                    if (tree != null && m_scrollinfo != null && tree.Items != null && tree.IsInitialized && tree.IsVirtualizing && tree.VirtualizationMode == VirtualizationMode.Normal)
                    {
                        //tree.RefreshItems();
                        int itemcount = 0;

                        if (tree.Items.Count > 1)
                            itemcount = tree.Items.Count - 1;
                        else
                            itemcount = 1;

                        this.m_scrollinfo.SetVerticalOffset(itemcount * 20);
                    }
                }
            }

            IsSortingTaskInProgress = false;
        }

        /// <summary>
        /// Calls OnSortingChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSortingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewAdv instance = d as TreeViewAdv;
            if (instance != null)
            {
                instance.newvalue = (SortDirection)e.NewValue;
                instance.oldvalue = (SortDirection)e.OldValue;
                instance.SortTreeView();
                instance.SortingChangedCount++;
            }
            else
            {
                TreeViewItemAdv tree = d as TreeViewItemAdv;
                if (tree != null)
                {
                    tree.Sorting = (SortDirection)e.NewValue;
                }
            }
        }

        /// <summary>
        /// Called when [is recursive sorting enabled changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnEnabledRecursiveSortingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Searches the item by header path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="pathseperator">The pathseperator.</param>
        /// <param name="IsSelected">if set to <c>true</c> [is selected].</param>
        /// <returns></returns>
        internal TreeViewItemAdv SearchItemByHeaderPath(string path, char pathseperator, bool IsSelected)
        {
            string[] hierarchylist = path.Split(pathseperator);
            int elementindex = 0;
            if (this != null)
            {
                foreach (object obj in (IEnumerable)base.Items)
                {
                    TreeViewItemAdv tree = (TreeViewItemAdv)base.ItemContainerGenerator.ContainerFromItem(obj);
                    if (tree == null)
                    {
                        tree = (obj as TreeViewItemAdv);
                    }
                    if (((tree != null) && (elementindex < hierarchylist.Length)) && tree.Header.Equals(hierarchylist[elementindex]))
                    {
                        if (!tree.HasItems)
                        {
                            if (IsSelected)
                            {
                                tree.IsSelected = true;
                            }
                            return tree;
                        }
                        if (elementindex == hierarchylist.Length - 1)
                        {
                            if (IsSelected)
                            {
                                tree.IsSelected = true;
                            }
                            return tree;
                        }
                        if (!tree.IsExpanded)
                        {
                            tree.IsExpanded = true;
                            tree.UpdateLayout();
                        }
                        if (tree.IsExpanded)
                        {
                            elementindex++;
                            return this.IterateHeaderPath(tree, elementindex, hierarchylist, IsSelected);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Iterates the header path.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="elementindex">The elementindex.</param>
        /// <param name="hierarchylist">The hierarchylist.</param>
        /// <param name="IsSelected">if set to <c>true</c> [is selected].</param>
        /// <returns></returns>
        private TreeViewItemAdv IterateHeaderPath(TreeViewItemAdv tree, int elementindex, string[] hierarchylist, bool IsSelected)
        {
            if (tree != null)
            {
                foreach (object obj in (IEnumerable)tree.Items)
                {
                    TreeViewItemAdv treeviewitem = (TreeViewItemAdv)tree.ItemContainerGenerator.ContainerFromItem(obj);
                    if (treeviewitem == null)
                    {
                        treeviewitem = (obj as TreeViewItemAdv);
                    }

                    if (((treeviewitem != null) && (elementindex < hierarchylist.Length)) && treeviewitem.Header.Equals(hierarchylist[elementindex]))
                    {
                        if (!treeviewitem.HasItems)
                        {
                            if (IsSelected)
                            {
                                treeviewitem.IsSelected = true;
                            }
                            return treeviewitem;
                        }
                        if (elementindex == hierarchylist.Length - 1)
                        {
                            if (IsSelected)
                            {
                                treeviewitem.IsSelected = true;
                            }
                            return treeviewitem;
                        }
                        if (!treeviewitem.IsExpanded)
                        {
                            treeviewitem.IsExpanded = true;
                            treeviewitem.UpdateLayout();
                        }
                        if (treeviewitem.IsExpanded)
                        {
                            elementindex++;
                            return this.IterateHeaderPath(treeviewitem, elementindex, hierarchylist, IsSelected);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Iterates the object path.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="elementindex">The elementindex.</param>
        /// <param name="hierarchylist">The hierarchylist.</param>
        /// <param name="IsSelected">if set to <c>true</c> [is selected].</param>
        /// <returns></returns>
        private TreeViewItemAdv IterateObjectPath(TreeViewItemAdv tree, int elementindex, string[] hierarchylist, string propertyname, bool IsSelected)
        {
            if (tree != null)
            {
                foreach (object obj2 in (IEnumerable)tree.Items)
                {
                    PropertyInfo p = obj2.GetType().GetProperty(propertyname);
                    if (p != null)
                    {
                        if (p.GetValue(obj2, null) != null)
                        {
                            if (p.GetValue(obj2, null).ToString().Equals(hierarchylist[elementindex]))
                            {
                                TreeViewItemAdv treeviewitem = (TreeViewItemAdv)tree.ItemContainerGenerator.ContainerFromItem(obj2);

                                if (((treeviewitem != null) && (elementindex < hierarchylist.Length)))
                                {
                                    if (!treeviewitem.HasItems)
                                    {
                                        if (IsSelected)
                                        {
                                            treeviewitem.IsSelected = true;
                                        }
                                        return treeviewitem;
                                    }
                                    if (elementindex == hierarchylist.Length - 1)
                                    {
                                        if (IsSelected)
                                        {
                                            treeviewitem.IsSelected = true;
                                        }
                                        return treeviewitem;
                                    }
                                    if (!treeviewitem.IsExpanded)
                                    {
                                        treeviewitem.IsExpanded = true;
                                        treeviewitem.UpdateLayout();
                                    }
                                    if (treeviewitem.IsExpanded)
                                    {
                                        elementindex++;
                                        return this.IterateObjectPath(treeviewitem, elementindex, hierarchylist, propertyname, IsSelected);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Searches the item by path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="pathseperator">The pathseperator.</param>
        /// <param name="propertyname">The propertyname.</param>
        /// <param name="IsSelected">if set to <c>true</c> [is selected].</param>
        /// <returns></returns>
        public TreeViewItemAdv SearchItemByPath(string path, char pathseperator, string propertyname, bool IsSelected)
        {
            return SearchItemByObjectPath(path, pathseperator, propertyname, IsSelected);
        }

        /// <summary>
        /// Searches the item by path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="pathseperator">The pathseperator.</param>
        /// <param name="propertyname">The propertyname.</param>
        /// <returns></returns>
        public TreeViewItemAdv SearchItemByPath(string path, char pathseperator, string propertyname)
        {
            return SearchItemByObjectPath(path, pathseperator, propertyname, false);
        }

        /// <summary>
        /// Searches the item by path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="pathseperator">The pathseperator.</param>
        /// <returns></returns>
        public TreeViewItemAdv SearchItemByPath(string path, char pathseperator)
        {
            return SearchItemByHeaderPath(path, pathseperator, false);
        }

        /// <summary>
        /// Searches the item by path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="pathseperator">The pathseperator.</param>
        /// <param name="IsSelected">if set to <c>true</c> [is selected].</param>
        /// <returns></returns>
        public TreeViewItemAdv SearchItemByPath(string path, char pathseperator, bool IsSelected)
        {
            return SearchItemByHeaderPath(path, pathseperator, IsSelected);
        }

        /// <summary>
        /// Searches the item by object path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="pathseperator">The pathseperator.</param>
        /// <param name="propertyname">The propertyname.</param>
        /// <param name="IsSelected">if set to <c>true</c> [is selected].</param>
        /// <returns></returns>
        internal TreeViewItemAdv SearchItemByObjectPath(string path, char pathseperator, string propertyname, bool IsSelected)
        {
            string[] hierarchylist = path.Split(pathseperator);
            int elementindex = 0, count = 0;
            if (this != null)
            {
                foreach (object obj2 in (IEnumerable)base.Items)
                {
                    count++;
                    TreeViewItemAdv tree = (TreeViewItemAdv)base.ItemContainerGenerator.ContainerFromItem(obj2);
                    if (tree == null)
                    {
                        if (IsVirtualizing)
                        {
                            if (this.m_scrollinfo != null)
                            {
                                IScrollInfo m_scrollhost = this.m_scrollinfo;
                                if (m_scrollhost != null)
                                {
                                    m_scrollhost.SetVerticalOffset((count - 1) * 20);
                                    m_scrollhost.ScrollOwner.UpdateLayout();
                                }
                            }
                        }
                    }
                    PropertyInfo p = obj2.GetType().GetProperty(propertyname);
                    if (p != null)
                    {
                        if (p.GetValue(obj2, null) != null)
                        {
                            if (p.GetValue(obj2, null).ToString().Equals(hierarchylist[elementindex]))
                            {
                                tree = (TreeViewItemAdv)base.ItemContainerGenerator.ContainerFromItem(obj2);
                                if (((tree != null) && (elementindex < hierarchylist.Length)))
                                {
                                    if (!tree.HasItems)
                                    {
                                        if (IsSelected)
                                        {
                                            tree.IsSelected = true;
                                        }
                                        return tree;
                                    }
                                    if (elementindex == hierarchylist.Length - 1)
                                    {
                                        if (IsSelected)
                                        {
                                            tree.IsSelected = true;
                                        }
                                        return tree;
                                    }
                                    if (!tree.IsExpanded)
                                    {
                                        tree.IsExpanded = true;
                                        tree.UpdateLayout();
                                    }
                                    if (tree.IsExpanded)
                                    {
                                        elementindex++;
                                        return this.IterateObjectPath(tree, elementindex, hierarchylist, propertyname, IsSelected);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Sorts the tree view.
        /// </summary>
        public void SortTreeView()
        {
            TreeViewAdv tree = null;
            if ((this as TreeViewAdv) != null)
            {
                tree = this as TreeViewAdv;
                (tree as ItemsControl).Items.SortDescriptions.Clear();
                SortingTreeView((DependencyObject)tree, tree.newvalue, tree.oldvalue);
                if (!EnabledRecursiveSorting)
                {
                    foreach (object obj in tree.Items)
                    {
                        TreeViewItemAdv titem = obj as TreeViewItemAdv;

                        if (titem != null)
                        {
                            if (titem.HasItems)
                            {
                                (titem as ItemsControl).Items.SortDescriptions.Clear();
                                SortingTreeView((DependencyObject)titem, tree.newvalue, tree.oldvalue);
                                IterateItems(titem, tree, tree.newvalue, tree.oldvalue);
                            }
                        }
                    }
                }
                else
                {
                    foreach (object obj in tree.Items)
                    {
                        TreeViewItemAdv titem = obj as TreeViewItemAdv;

                        if (titem != null)
                        {
                            if (titem.HasItems && titem.IsExpanded)
                            {
                                (titem as ItemsControl).Items.SortDescriptions.Clear();
                                SortingTreeView((DependencyObject)titem, tree.newvalue, tree.oldvalue);
                                IterateItems(titem, tree, tree.newvalue, tree.oldvalue);
                            }
                        }
                    }
                }
            }
        }

        ///Iterates the items
        /// <summary>
        /// Iterates the items.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="item">The item.</param>
        internal void IterateItems(TreeViewItemAdv tree, TreeViewAdv treeview, SortDirection newvalue, SortDirection oldvalue)
        {
            foreach (object obj in tree.Items)
            {
                TreeViewItemAdv titem = obj as TreeViewItemAdv;

                if (titem != null)
                {
                    if (!EnabledRecursiveSorting)
                    {
                        if (titem.HasItems)
                        {
                            (titem as ItemsControl).Items.SortDescriptions.Clear();
                            SortingTreeView((DependencyObject)titem, newvalue, oldvalue);
                            IterateItems(titem, treeview, newvalue, oldvalue);
                        }
                    }
                    else
                    {
                        if (titem.HasItems && titem.IsExpanded)
                        {
                            (titem as ItemsControl).Items.SortDescriptions.Clear();
                            SortingTreeView((DependencyObject)titem, newvalue, oldvalue);
                            IterateItems(titem, treeview, newvalue, oldvalue);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calls OnSortingFieldChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSortingFieldChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemsControl itemsControl = d as ItemsControl;

            try
            {
                if (itemsControl != null && (itemsControl is TreeViewAdv
                    || itemsControl is TreeViewItemAdv))
                {
                    itemsControl.Items.SortDescriptions.Clear();
                    string sortingField = (string)e.NewValue;
                    SortDirection sortDirection = TreeViewAdv.GetSorting(itemsControl);

                    if (itemsControl is TreeViewItemAdv)
                    {
                        if ((itemsControl as TreeViewItemAdv).Sorting != SortDirection.None)
                        {
                            sortDirection = (itemsControl as TreeViewItemAdv).Sorting;
                        }
                    }

                    if (SortDirection.None != sortDirection)
                    {
                        ListSortDirection listSortDirection = SortDirection.Ascending == sortDirection
                            ? ListSortDirection.Ascending : ListSortDirection.Descending;
                        itemsControl.Items.SortDescriptions.Add(new SortDescription(sortingField, listSortDirection));
                    }

                    TreeViewAdv tree = TreeViewAdv.GetTreeViewFromChildren(itemsControl);
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Called when [expander style changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnExpanderStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when [selected tree item changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedTreeItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Object item = null;
            object olditem = null;
            TreeViewAdv tree = d as TreeViewAdv;
            if (tree != null && tree.allowSelectedTreeItem)
            {
                if (e.NewValue != null)
                {
                    if (e.NewValue is TreeViewItemAdv)
                    {
                        item = e.NewValue;
                        olditem = e.OldValue;
                    }
                    else
                    {
                        item = (tree.ItemContainerGenerator.ContainerFromItem(e.NewValue) as TreeViewItemAdv);
                        if (item == null)
                        {
                            if (tree.Items.Contains(e.NewValue))
                            {
                                item = (tree.ItemContainerGenerator.ContainerFromItem(e.NewValue) as TreeViewItemAdv);
                                if (item == null)
                                {
                                    tree.UpdateLayout();
                                    item = (tree.ItemContainerGenerator.ContainerFromItem(e.NewValue) as TreeViewItemAdv);
                                }
                            }
                        }
                        if (item == null)
                        {
                            foreach (object subItem in tree.Items)
                            {
                                TreeViewItemAdv treeItem = (tree.ItemContainerGenerator.ContainerFromItem(subItem) as TreeViewItemAdv);
                                if (treeItem != null)
                                {
                                    if (treeItem.Items.Contains(e.NewValue))
                                    {
                                        item = treeItem.ItemContainerGenerator.ContainerFromItem(e.NewValue) as TreeViewItemAdv;
                                        if (item == null)
                                        {
                                            treeItem.UpdateLayout();
                                            item = treeItem.ItemContainerGenerator.ContainerFromItem(e.NewValue) as TreeViewItemAdv;
                                        }
                                    }
                                    else
                                        item = (object)GetModelContainer(treeItem, e.NewValue);
                                }
                                if (item != null)
                                {
                                    if (item is TreeViewItemAdv && (item as TreeViewItemAdv).ParentTreeViewItem != null)
                                        (item as TreeViewItemAdv).ParentTreeView.BringIntoView(item as TreeViewItemAdv);
                                    break;
                                }
                            }
                        }
                        if (e.OldValue != null)
                            olditem = (tree.ItemContainerGenerator.ContainerFromItem(e.OldValue) as TreeViewItemAdv);
                    }
                    if ((item as TreeViewItemAdv) != null)
                    {
                        if (!(item as TreeViewItemAdv).IsSelected)
                            (item as TreeViewItemAdv).IsSelected = true;
                        if (tree != null)
                        {
                            tree.SelectedTreeItemObject = (item as TreeViewItemAdv).m_treeviewitemactualobject;
                        }
                        if ((item as TreeViewItemAdv) != null && (item as TreeViewItemAdv).m_treeviewitemactualobject != null && !(item as TreeViewItemAdv).m_treeviewitemactualobject.Equals((d as TreeViewAdv).SelectedItem))
                        {
                            if (!(item as TreeViewItemAdv).IsSelected)
                            {
                                (tree).SetSelectAndFocus((item as TreeViewItemAdv));
                            }
                        }
                    }
                }
                if (tree.SelectedItems.Count == 0 && (olditem as TreeViewItemAdv) != null)
                {
                    if ((olditem as TreeViewItemAdv).ParentTreeView == null)
                        (olditem as TreeViewItemAdv).m_treeViewAdv = tree;
                    (olditem as TreeViewItemAdv).Select(false);
                }
            }

            if (tree != null && (tree.SelectedItem as TreeViewItemAdv) != null)
            {
                if ((tree.SelectedItem as TreeViewItemAdv).m_treeviewitemactualobject != null)
                {
                    tree.SelectedTreeItemObject = (tree.SelectedItem as TreeViewItemAdv).m_treeviewitemactualobject;
                }
            }
        }

        /// <summary>
        /// Iterate and get the container of the given object in the tree in Model based
        /// </summary>
        /// <param name="tree">The tree</param>
        /// <param name="Item">target object whose container has to find <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> return TreeViewItemAdv</param>
        internal static TreeViewItemAdv GetModelContainer(TreeViewItemAdv tree, object item)
        {
            TreeViewItemAdv container = null;
            if (tree != null)
            {
                container = (tree.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItemAdv);
                if (container != null)
                    return container;
                else if (tree.Items.Count > 0)
                {
                    foreach (object subItem in tree.Items)
                    {
                        TreeViewItemAdv treeItem = (tree.ItemContainerGenerator.ContainerFromItem(subItem) as TreeViewItemAdv);
                        if (treeItem != null)
                        {
                            if (treeItem.Items.Contains(item))
                                container = treeItem.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItemAdv;
                            else
                                container = GetModelContainer(treeItem, item);
                        }
                        if (container != null)
                            return container;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Called when [selected tree item object changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedTreeItemObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Calls OnSelectedValuePathChanged method of the instance, notifies of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property change details, such as old value and new value.</param>
        private static void OnSelectedValuePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewAdv instance = (TreeViewAdv)d;
            instance.UpdateSelectedValue(instance.SelectedItem);
        }

        /// <summary>
        /// Makes one item or group of items selected ( when user minimizes and maximizes the window )
        /// </summary>
        /// <param name="newFocused">focused item</param>
        private void StartSelection(TreeViewItemAdv newFocused)
        {
            if (newFocused != null && ModifierKeyNotPressed())
            {
                TreeViewItemAdv focusedElement = FocusManager.GetFocusedElement(this) as TreeViewItemAdv;
                bool isInSelectedItems = SelectedItems.Contains(newFocused) || SelectedItems.Contains(newFocused.DataContext);

                if (isInSelectedItems || newFocused == focusedElement)
                {
                    if (!newFocused.IsSelected)
                    {
                        SelectedItems.Remove(newFocused);
                    }

                    ////NOTE: SelectedItems collection may be changed while for is executing.
                    for (int i = 0; i < SelectedItems.Count; i++)
                    {
                        TreeViewItemAdv item = SelectedItems[i] as TreeViewItemAdv;

                        if (item != null && !item.IsSelected)
                        {
                            item.Select(true);
                        }
                    }
                }
                else
                {
                    SelectItem(newFocused);
                }
            }
            else if (WasNavigationKeyDown() || Keyboard.Modifiers == ModifierKeys.Control || SelectedItems.Count > 1)
            {
                SelectItem(newFocused);
            }
        }

        /// <summary>
        /// Is used to define if Control or Shift key was pressed.
        /// </summary>
        /// <returns>Returns true if Control and Shift key was not pressed</returns>
        private bool ModifierKeyNotPressed()
        {
            return Keyboard.Modifiers != ModifierKeys.Control
                && Keyboard.Modifiers != ModifierKeys.Shift;
        }

        /// <summary>
        /// Gets Last item of the tree view.
        /// </summary>
        /// <returns>last item.</returns>
        private object GetLastItem()
        {
            object lastItem = null;
            TreeViewItemAdv item = GetItemContainer(Items.Count - 1);
            object tempItem = null;

            while (item != null)
            {
                int count = item.Items.Count;

                if (item.IsExpanded && count > 0)
                {
                    tempItem = item.Items[count - 1];

                    if (tempItem == null)
                    {
                        tempItem = GetItemContainer(count - 1);
                    }

                    item = tempItem as TreeViewItemAdv;
                }
                else
                {
                    break;
                }
            }

            lastItem = (item == null) ? tempItem : item;
            return lastItem;
        }

        /// <summary>
        /// Gets first node.
        /// </summary>
        /// <param name="item">The item TreeView.</param>
        /// <param name="container">The container TreeView.</param>
        /// <returns>bool type value </returns>
        private bool GetFirstItem(out object item, out TreeViewItemAdv container)
        {
            bool result = false;

            if (base.HasItems)
            {
                item = Items[0];
                container = GetItemContainer(0);

                if (item != null)
                {
                    if (droppedobjects.Count == 0)
                    {
                        result = true;
                    }
                }
            }
            else
            {
                item = null;
                container = null;
            }

            return result;
        }

        private bool targetnodeselected = false;

        /// <summary>
        /// Gets the item by index.
        /// </summary>
        /// <param name="index">Index of the item.</param>
        /// <returns>TreeViewitemAdv value </returns>
        private TreeViewItemAdv GetItemContainer(int index)
        {
            TreeViewItemAdv item = null;

            if (index > -1 && Items.Count > index)
            {
                item = Items[index] as TreeViewItemAdv;

                if (item == null)
                {
                    item = ItemContainerGenerator.ContainerFromIndex(index) as TreeViewItemAdv;
                    if (isSelectItemsChanged)
                    {
                        for (int i = 0; i < modelItems.Count; i++)
                        {
                            if (modelItems[i].iTree == Items[index])
                            {
                                if (tempCurrentNode != null && tempTargetNode != null && (modelItems[i].iTree == tempTargetNode.Header || modelItems[i].iTree == tempCurrentNode.Header))
                                {
                                    modelItems[i].iTree.IsSelected = true;
                                    if (modelItems[i].iTree.IsExpanded)
                                    {
                                        for (int j = 0; j < modelItems[i].treeItems.Count; j++)
                                        {
                                            TreeViewItemAdv tempitem = null;
                                            if (item != null)
                                                tempitem = item.ItemContainerGenerator.ContainerFromIndex(j) as TreeViewItemAdv;
                                            if (tempCurrentNode != null && tempCurrentNode.ParentItemsControl == null)
                                            {
                                                modelItems[i].treeItems[j].IsSelected = true;
                                                startindex = j;
                                                if (internaltargetnode != null && internaltargetindex == j)
                                                {
                                                    targetnodeselected = true;
                                                }
                                                isInternalItem = true;
                                            }
                                            else if (startindex >= 0 && j > startindex && (!(modelItems[i].treeItems[j].IsSelected) && !targetnodeselected))
                                                modelItems[i].treeItems[j].IsSelected = true;
                                        }
                                        startindex = -1;
                                    }
                                }
                                else if (modelItems[i].iTree.IsExpanded)
                                {
                                    for (int j = 0; j < modelItems[i].treeItems.Count; j++)
                                    {
                                        TreeViewItemAdv tempitem = null;
                                        if (item != null)
                                            tempitem = item.ItemContainerGenerator.ContainerFromIndex(j) as TreeViewItemAdv;
                                        if (tempCurrentNode != null && modelItems[i].treeItems[j] == tempCurrentNode.Header || (tempitem != null && tempTargetNode.Header == tempitem.Header))
                                        {
                                            modelItems[i].treeItems[j].IsSelected = true;
                                            startindex = j;
                                            if (internaltargetnode != null && internaltargetindex == j && (((internaltargetnode.ParentItemsControl is TreeViewItemAdv) && (internaltargetnode.ParentItemsControl as TreeViewItemAdv).Header == modelItems[i].iTree)) || (internaltargetnode.ParentItemsControl is TreeViewAdv))
                                            {
                                                targetnodeselected = true;
                                            }
                                            isInternalItem = true;
                                        }
                                        else if (startindex >= 0 && j > startindex && (!(modelItems[i].treeItems[j].IsSelected) && !targetnodeselected))
                                            modelItems[i].treeItems[j].IsSelected = true;
                                    }
                                    startindex = -1;
                                }
                            }
                        }
                        isSelectItemsChanged = false;
                    }
                    else if (tempCurrentNode != null && tempTargetNode != null && isTargetNodeParentNotVisible)
                    {
                        for (int i = 0; i < modelItems.Count; i++)
                        {
                            if (modelItems[i].iTree == Items[index] && !(modelItems[i].iTree == tempTargetNode.Header))
                            {
                                if (modelItems[i].iTree == tempCurrentNode.Header || modelItems[i].iTree == tempTargetNode.Header)
                                {
                                    modelItems[i].iTree.IsSelected = true;
                                }
                                else if (modelItems[i].iTree.IsExpanded)
                                {
                                    for (int j = 0; j < modelItems[i].treeItems.Count; j++)
                                    {
                                        if (!isInternalItem)
                                        {
                                            if (modelItems[i].treeItems[j] == tempTargetNode.Header)
                                                startindex = j;
                                            else if (startindex >= 0 && j > startindex && !(modelItems[i].treeItems[j].IsSelected))
                                                modelItems[i].treeItems[j].IsSelected = true;
                                            isParentItem = true;
                                        }
                                        else
                                        {
                                            modelItems[i].treeItems[j].IsSelected = true;
                                            if (j == 0 && !modelItems[i].iTree.IsSelected)
                                                modelItems[i].iTree.IsSelected = true;
                                            if (modelItems[i].treeItems[j] == tempTargetNode.Header || modelItems[i].treeItems[j] == tempCurrentNode.Header)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    startindex = -1;
                                }
                            }
                            else
                            {
                                if (isParentItem)
                                {
                                    modelItems[i].iTree.IsSelected = true;
                                    isInternalItem = false;
                                    if (modelItems[i].iTree == tempCurrentNode.Header)
                                        break;
                                }
                            }
                        }
                        isInternalItem = false;
                        isParentItem = false;
                        isTargetNodeParentNotVisible = false;
                    }
                    else if (tempCurrentNode == null)
                    {
                        for (int i = 0; i < modelItems.Count; i++)
                        {
                            if (modelItems[i].iTree == Items[index])
                            {
                                if (tempTargetNode != null && !modelItems[i].iTree.IsExpanded)
                                {
                                    if (modelItems[i].iTree == tempTargetNode.Header)
                                    {
                                        modelItems[i].iTree.IsSelected = true;
                                        for (int j = 0; j < i; j++)
                                        {
                                            if (!modelItems[j].iTree.IsExpanded && !modelItems[j].iTree.IsSelected)
                                                modelItems[j].iTree.IsSelected = true;
                                        }
                                    }
                                }
                                else if (modelItems[i].iTree.IsExpanded)
                                {
                                    if (internalcurrentindex > -1)
                                    {
                                        modelItems[i].iTree.IsSelected = true;
                                    }

                                    for (int j = 0; j < modelItems[i].treeItems.Count; j++)
                                    {
                                        if (!isInternalItem)
                                        {
                                            if (internaltargetindex == 0 || (internalcurrentnode != null && !(internaltargetnode.ParentItemsControl.Equals(internalcurrentnode.ParentItemsControl))))
                                            {
                                                if (!(modelItems[i].treeItems[j].IsSelected))
                                                    modelItems[i].treeItems[j].IsSelected = true;
                                            }

                                            if (tempTargetNode != null && modelItems[i].treeItems[j] == tempTargetNode.Header)
                                            {
                                                break;
                                            }
                                            else if (internaltargetnode != null && internalcurrentnode != null && internaltargetnode.ParentItemsControl.Equals(internalcurrentnode.ParentItemsControl) && modelItems[i].treeItems[j] == internaltargetnode.Header)
                                            {
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            modelItems[i].treeItems[j].IsSelected = true;
                                            if (j == 0 && !modelItems[i].iTree.IsSelected)
                                                modelItems[i].iTree.IsSelected = true;
                                            if (modelItems[i].treeItems[j] == tempTargetNode.Header || modelItems[i].treeItems[j] == tempCurrentNode.Header)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        isInternalItem = false;
                    }
                }
            }
            if (targetnodeselected)
                targetnodeselected = false;
            return item;
        }

        /// <summary>
        /// Selects first node.
        /// </summary>
        private void SelectFirstItem()
        {
            ThreadStart thread = delegate
            {
                object selectedItem = null;
                TreeViewItemAdv container = null;

                bool selected = GetFirstItem(out selectedItem, out container);

                if (!selected && Items.Count > 0)
                {
                    selectedItem = SelectedItem;
                    container = m_selectedContainer;
                }
                if (!SelectedItems.Contains(selectedItem) && container != null)
                {
                    ChangeSelection(selectedItem, container, selected, false);
                }
            };

            Dispatcher.BeginInvoke(DispatcherPriority.Background, thread);
        }

        /// <summary>
        /// Sets value of the SelectedItem item.
        /// </summary>
        /// <param name="data">The data TreeView.</param>
        internal void SetSelectedItem(object data)
        {
            if (this.SelectedItem != data)
            {
                base.SetValue(SelectedItemPropertyKey, data);
            }
        }

        /// <summary>
        /// Update binding for SelectedValue.
        /// </summary>
        /// <param name="selectedItem">The selected item.</param>
        internal void UpdateSelectedValue(object selectedItem)
        {
            if (selectedItem == null)
            {
                BindingOperations.ClearBinding(this, TreeViewAdv.SelectedValueProperty);
            }
            else
            {
                bool flag = XmlHelper.IsXmlNode(selectedItem);
                BindingExpression bindingExpr = this.GetBindingExpression(SelectedValueProperty);

                if (bindingExpr != null)
                {
                    bool flag2 = bindingExpr.ParentBinding.XPath != null;

                    if (flag2 != flag)
                    {
                        bindingExpr = null;
                    }
                }

                Binding binding = new Binding();
                binding.Source = selectedItem;

                if (flag)
                {
                    binding.XPath = this.SelectedValuePath;
                    binding.Path = new PropertyPath("/InnerText", new object[0]);
                }
                else
                {
                    binding.Path = new PropertyPath(this.SelectedValuePath, new object[0]);
                }
                SetBinding(TreeViewAdv.SelectedValueProperty, binding);
            }
        }

        /// <summary>
        /// Selects one item.
        /// </summary>
        /// <param name="data">The data TreeView.</param>
        /// <param name="container">The container.</param>
        /// <param name="selected">if set to <c>true</c> [selected].</param>
        private void SingleSelection(object data, TreeViewItemAdv container, bool selected)
        {
            try
            {
                if (AllowMultiSelect && ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control || ForceSingleSelection) && container != null)
                {
                    if (selected)
                    {
                        container.IsSelected = selected;

                        if (!SelectedItems.Contains(data))
                        {
                            SelectedItems.Add(data);
                            SetSelectedItem(data);
                            UpdateSelectedValue(data);
                        }
                        if (!m_selectedContainers.Contains(container))
                        {
                            m_selectedContainers.Add(container);
                            m_selectedContainer = container;
                        }
                    }
                    else
                    {
                        container.IsSelected = selected;

                        if (SelectedItems.Contains(data))
                        {
                            SelectedItems.Remove(data);
                            if (SelectedItem == data && SelectedItems.Count >= 1)
                            {
                                SetSelectedItem(SelectedItems[0]);
                                UpdateSelectedValue(SelectedItems[0]);
                            }
                            else if (SelectedItem == data && SelectedItems.Count <= 0)
                            {
                                SetSelectedItem(null);
                                UpdateSelectedValue(null);
                            }
                        }
                        if (m_selectedContainers.Contains(container))
                        {
                            m_selectedContainers.Remove(container);
                        }
                    }
                }
                else if (AllowChange)
                {
                    m_startSelectContainer = container;
                    TempParentItemsControl = container.ParentItemsControl;
                    if (container.ParentTreeViewItem != null)
                        tempCurrentIndex = this.ItemContainerGenerator.IndexFromContainer(container.ParentTreeViewItem);
                    else
                        tempCurrentIndex = this.ItemContainerGenerator.IndexFromContainer(container);
                    if (container != null)
                        m_startIndex = this.ItemContainerGenerator.IndexFromContainer(container);
                    if (selected)
                    {
                        if (container != null)
                        {
                            foreach (TreeViewItemAdv item in m_selectedContainers)
                            {
                                if (item != null)
                                    item.IsSelected = false;
                            }
                            for (int i = 0; i < m_treeviewadv.Count; i++)
                            {
                                if (m_treeviewadv[i] is TreeViewItemAdv && (m_treeviewadv[i] as TreeViewItemAdv) != null && (m_treeviewadv[i] as TreeViewItemAdv).IsSelected)
                                {
                                    (m_treeviewadv[i] as TreeViewItemAdv).IsSelected = false;
                                }
                            }
                            if (modelItems != null)
                            {
                                for (int i = 0; i < modelItems.Count; i++)
                                {
                                    if (modelItems[i].iTree != null)
                                    {
                                        modelItems[i].iTree.IsSelected = false;
                                        if (modelItems[i].treeItems.Count > 0)
                                        {
                                            for (int j = 0; j < modelItems[i].treeItems.Count; j++)
                                            {
                                                modelItems[i].treeItems[j].IsSelected = false;
                                            }
                                        }
                                    }
                                }
                            }

                            container.IsSelected = true;
                            m_selectedContainers.Clear();
                            if (isSelectionbymouseclick)
                            {
                                SelectedItems.Clear();
                            }
                            isSelectionbymouseclick = false;
                            m_draggingParentItmes.Clear();
                            SelectedItems.Add(data);
                            m_selectedContainers.Add(container);
                            m_draggingParentItmes.Add(container.ParentItemsControl);
                            SetSelectedItem(data);
                            UpdateSelectedValue(data);
                            m_selectedContainer = container;
                            if (Keyboard.Modifiers != ModifierKeys.Shift && Keyboard.Modifiers != ModifierKeys.Control)
                                issingleselection = true;
                            m_selectedContainer.UpdateContainsSelection(true);
                        }
                    }
                    else
                    {
                        if (m_selectedContainers.Contains(container))
                        {
                            foreach (TreeViewItemAdv item in m_selectedContainers)
                            {
                                if (item != null)
                                {
                                    item.IsSelected = false;
                                    SelectedItems.Remove(item.Header);
                                }
                            }
                        }
                        m_selectedContainers.Clear();
                        SetSelectedItem(null);
                        UpdateSelectedValue(null);
                        if (this.SelectedItem == null && this.SelectedItems.Count > 0 && m_selectedContainers.Count > 0)
                        {
                            SetSelectedItem(this.SelectedItems[0]);
                            UpdateSelectedValue(this.SelectedItems[0]);
                        }

                        if (container != null)
                        {
                            container.IsSelected = false;
                            if (SelectedItems.Contains(container.Header))
                            {
                                SelectedItems.Remove(container.Header);
                            }
                            if (m_selectedContainer != null)
                            {
                                m_selectedContainer.UpdateContainsSelection(false);
                                m_selectedContainer = null;
                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Selects any item and add it to SelectedItems collection.
        /// </summary>
        /// <param name="data">The data TreeView.</param>
        /// <param name="container">The container.</param>
        /// <param name="selected">if set to <c>true</c> [selected].</param>
        private void RandomSelection(object data, TreeViewItemAdv container, bool selected)
        {
            m_startSelectContainer = container;
            if (container != null)
                m_startIndex = this.ItemContainerGenerator.IndexFromContainer(container);
            if (selected)
            {
                if (!(container == m_selectedContainer && !IsMultiselection))
                {
                    if (!IsMultiselection)
                    {
                        if (SelectedItem != null)
                        {
                            if (!m_selectedContainers.Contains(container))
                            {
                                m_selectedContainers.Add(container);
                                m_draggingParentItmes.Add(container.ParentItemsControl);
                            }
                            if (!SelectedItems.Contains(data) && !droppedobjects.Contains(data) && !m_selectedContainers.Contains(container))
                            {
                                SelectedItems.Add(data);
                            }
                            if (m_selectedContainer == null)
                                m_selectedContainer = container;
                            if (!m_selectedContainer.IsSelected && m_selectedContainer == container)
                            {
                                m_selectedContainer.IsSelected = true;
                            }
                        }
                    }

                    if (!m_selectedContainers.Contains(container))
                    {
                        m_selectedContainers.Add(container);
                    }

                    if (!SelectedItems.Contains(data) && !droppedobjects.Contains(data))
                    {
                        SelectedItems.Add(data);
                    }

                    container.IsSelected = true;

                    if (SelectedItems.Count > 0)
                    {
                        if (this.SelectedItem == null)
                        {
                            m_selectedContainer = m_selectedContainers[0];
                            SetSelectedItem(SelectedItems[0]);
                            UpdateSelectedValue(SelectedItems[0]);
                        }
                    }
                    else
                    {
                        m_selectedContainer = m_startSelectContainer;
                        SetSelectedItem(data);
                        UpdateSelectedValue(data);
                    }
                }
            }
            else if (m_selectedContainers.IndexOf(container) != -1)
            {
                container.IsSelected = false;

                if (!SelectedItems.Remove(data))
                {
                    SelectedItems.Remove(data);
                }
                if (!_isdoubleclick)
                    m_selectedContainers.Remove(container);

                if (!selected)
                {
                    if (SelectedItems.Count > 0)
                    {
                        SetSelectedItem(SelectedItems[0]);
                        UpdateSelectedValue(SelectedItems[0]);
                    }
                    else
                    {
                        SetSelectedItem(null);
                        UpdateSelectedValue(null);
                    }
                }
            }
        }

        /// <summary>
        /// Implements sequential selection of the items.
        /// </summary>
        /// <param name="data">The data TreeView.</param>
        /// <param name="container">The container.</param>
        /// <param name="selected">if set to <c>true</c> [selected].</param>
        private void SequentialSelection(object data, TreeViewItemAdv container, bool selected)
        {
            if (data != null && container != null)
            {
                IsMultiselection = true;
                SetSelectedItem(data);
                if (selected)
                {
                    if (IsDirectlySelection(m_startSelectContainer, container))
                    {
                        if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                        {
                            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                            {
                                if (lastSelectedItem == null || lastSelectedItem.Header.ToString() == "{DisconnectedItem}")
                                    SelectDiapason(m_startSelectContainer, container, true, true);
                                else
                                    SelectDiapason(lastSelectedItem, container, true, true);
                            }
                            else
                                SelectDiapason(m_startSelectContainer, container, true, true);
                        }
                        else
                            SelectDiapason(m_startSelectContainer, container, true, true);
                    }
                    else
                    {
                        SelectDiapason(container, m_startSelectContainer, true, true);
                    }
                }
                else if (m_selectedContainers.IndexOf(container) != -1)
                {
                    container.IsSelected = false;

                    if (!SelectedItems.Remove(data))
                    {
                        SelectedItems.Remove(data);
                    }
                    if (!_isdoubleclick)
                        m_selectedContainers.Remove(container);
                }
            }
        }

        /// <summary>
        /// Implements segment selection of the items (Using Shift+Ctrl).
        /// </summary>
        /// <param name="data">The data TreeView.</param>
        /// <param name="container">The container.</param>
        /// <param name="selected">if set to <c>true</c> [selected].</param>
        private void SegmentSelection(object data, TreeViewItemAdv container, bool selected)
        {
            if (data != null && container != null)
            {
                IsMultiselection = true;
                SetSelectedItem(data);

                if (selected)
                {
                    if (IsDirectlySelection(m_startSelectContainer, container))
                    {
                        SelectDiapason(m_startSelectContainer, container, false, true);
                    }
                    else
                    {
                        SelectDiapason(container, m_startSelectContainer, false, true);
                    }
                }
                else
                {
                    if (IsDirectlySelection(m_startSelectContainer, container))
                    {
                        SelectDiapason(m_startSelectContainer, container.GetPreviousVisibleItem(true), false, false);
                        if (!IsMultiselection)
                        {
                            IsMultiselection = true;
                        }
                        AddNodeToSelectedItems(container);
                    }
                    else
                    {
                        SelectDiapason(container.GetNextVisibleItem(true), m_startSelectContainer, false, false);
                        if (!IsMultiselection)
                        {
                            IsMultiselection = true;
                        }
                        AddNodeToSelectedItems(container);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value indicates whether selecting is top-down.
        /// </summary>
        /// <param name="currentNode">The current node.</param>
        /// <param name="targerNode">The targer node.</param>
        /// <returns>
        /// <c>true</c> if [is directly selection] [the specified current node]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsDirectlySelection(TreeViewItemAdv currentNode, TreeViewItemAdv targerNode)
        {
            bool findedTarger = true;

            if (currentNode != null && targerNode != null)
            {
                int currentNodeLevel = currentNode.GetNodeLevel();
                int targetNodeLevel = targerNode.GetNodeLevel();
                TreeViewItemAdv currentItem = currentNode;
                TreeViewItemAdv targetItem = targerNode;

                if (currentNodeLevel > targetNodeLevel)
                {
                    while (currentNodeLevel > targetNodeLevel && currentItem.ParentTreeViewItem != null)
                    {
                        currentItem = currentItem.ParentTreeViewItem;
                        currentNodeLevel = currentItem.GetNodeLevel();
                    }
                }
                else
                {
                    while (currentNodeLevel < targetNodeLevel && targetItem.ParentTreeViewItem != null)
                    {
                        targetItem = targetItem.ParentTreeViewItem;
                        targetNodeLevel = targetItem.GetNodeLevel();
                    }
                }

                if (currentNodeLevel == targetNodeLevel)
                {
                    while (currentItem.ParentItemsControl != targetItem.ParentItemsControl
                        && currentItem.ParentItemsControl != null
                        && targetItem.ParentItemsControl != null)
                    {
                        currentItem = currentItem.ParentTreeViewItem;
                        targetItem = targetItem.ParentTreeViewItem;
                        currentNodeLevel = currentItem.GetNodeLevel();
                        targetNodeLevel = targetItem.GetNodeLevel();
                    }

                    if (currentItem.ParentItemsControl == targetItem.ParentItemsControl)
                    {
                        currentNodeLevel = currentItem.ParentItemsControl.ItemContainerGenerator.IndexFromContainer(currentItem);
                        targetNodeLevel = targetItem.ParentItemsControl.ItemContainerGenerator.IndexFromContainer(targetItem);

                        if (currentItem == targerNode || currentNodeLevel > targetNodeLevel)
                        {
                            findedTarger = false;
                        }
                    }
                }
            }

            return findedTarger;
        }

        /// <summary>
        /// Selects diapason of the items and add it to SelectedItems collection.
        /// </summary>
        /// <param name="currentNode">The current node.</param>
        /// <param name="targetNode">The target node.</param>
        /// <param name="bReset">if set to <c>true</c> [b reset].</param>
        /// <param name="bSelect">if set to <c>true</c> [b select].</param>
        /// <returns> bool  value </returns>
        private bool SelectDiapason(TreeViewItemAdv currentNode, TreeViewItemAdv targetNode, bool bReset, bool bSelect)
        {
            bool findedTarget = false;
            m_itemsStack.Clear();

            if (bReset && externalSelect)
            {
                ClearItemContainers();
            }

            if (currentNode != null && targetNode != null)
            {
                if (currentNode == targetNode && Keyboard.Modifiers != ModifierKeys.Shift)
                {
                    if (bSelect)
                    {
                        AddNodeToSelectedItems(currentNode);
                    }
                    else
                    {
                        RemoveNodeFromSelectedItems(currentNode);
                    }
                }
                else if (!externalSelect)
                {
                    FindSelectItems(currentNode, targetNode, ref findedTarget, bSelect);
                }
            }
            else if (IsVirtualizing && VirtualizationMode == VirtualizationMode.Extended && !externalSelect)
            {
                FindSelectItems(currentNode, targetNode, ref findedTarget, bSelect);
            }

            return findedTarget;
        }

        /// <summary>
        /// Clears selected items, selected containers and items stack.
        /// </summary>
        private void ClearItemContainers()
        {
            m_selectedContainers.Clear();
            SelectedItems.Clear();
        }

        /// <summary>
        /// Gets tree view item from stack or from tree view.
        /// </summary>
        /// <param name="counter">number of item from TreeViewAdv to get</param>
        /// <returns>next tree view item in the tree</returns>
        private TreeViewItemAdv GetNextTreeViewItemAdv(ref int counter)
        {
            TreeViewItemAdv node = null;

            if (m_itemsStack.Count != 0)
            {
                node = m_itemsStack.Pop();
            }
            else
            {
                if (IsVirtualizing && VirtualizationMode == VirtualizationMode.Extended)
                {
                    node = GetItemContainer(counter);
                    counter++;
                }
                else
                    node = GetItemContainer(++counter);
            }
            return node;
        }

        /// <summary>
        /// Inserts node to the end of selected items list.
        /// </summary>
        /// <param name="node">The node TreeView.</param>
        public void AddNodeToSelectedItems(TreeViewItemAdv node)
        {
            if (SelectedItems.Count >= 1 && !AllowMultiSelect)
            {
            }
            else
            {
                if (node != null && !m_selectedContainers.Contains(node) && !node.IsReadOnly)
                {
                    m_ismouseSelection = false;
                    if (!m_selectedContainers.Contains(node))
                    {
                        m_selectedContainers.Add(node);
                    }

                    if (!node.IsSelected)
                    {
                        node.IsSelected = true;
                    }
                    object obj = null;
                    if (node.ParentItemsControl != null)
                        obj = node.ParentItemsControl.ItemContainerGenerator.ItemFromContainer(node);

                    if (obj != null && obj != DependencyProperty.UnsetValue)
                    {
                        if (!SelectedItems.Contains(obj))
                        {
                            SelectedItems.Add(obj);
                        }
                    }
                    else
                    {
                        if (!SelectedItems.Contains(node))
                        {
                            SelectedItems.Add(node);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes node from the end of selected items list.
        /// </summary>
        /// <param name="node">The node TreeView.</param>
        public void RemoveNodeFromSelectedItems(TreeViewItemAdv node)
        {
            if (node != null && m_selectedContainers.Contains(node) && !node.IsReadOnly)
            {
                node.IsSelected = false;
                object obj = null;
                if (node.ParentItemsControl != null)
                    obj = node.ParentItemsControl.ItemContainerGenerator.ItemFromContainer(node);

                m_selectedContainers.Remove(node);

                if (obj != null && obj != DependencyProperty.UnsetValue)
                {
                    SelectedItems.Remove(obj);
                }
                else
                {
                    SelectedItems.Remove(node);
                }
            }
        }

        /// <summary>
        /// Puts inner item of a node to stack.
        /// </summary>
        /// <param name="node">TreeViewItemAdv to put</param>
        /// <param name="i">index of the item in TreeViewAdv parent</param>
        private void PushInnerItemToStack(TreeViewItemAdv node, int i)
        {
            TreeViewItemAdv innerItem = node.Items[i] as TreeViewItemAdv;

            if (innerItem == null)
            {
                innerItem = node.ItemContainerGenerator.ContainerFromItem(node.Items[i]) as TreeViewItemAdv;
            }

            if (innerItem != null)
            {
                m_itemsStack.Push(innerItem);
            }
        }

        /// <summary>
        /// Unselects all selected items.
        /// </summary>
        private void ResetSelectedItems()
        {
            for (int i = m_selectedContainers.Count - 1; i >= 0; i--)
            {
                if (m_selectedContainers.Count > 0)
                {
                    TreeViewItemAdv item = m_selectedContainers[i];
                    if (item != null)
                        item.IsSelected = false;
                }
            }

            m_selectedContainers.Clear();
            SelectedItems.Clear();
        }

        /// <summary>
        /// Called when ShowRootLines is changed.
        /// </summary>
        /// <param name="d">TreeViewAdv object.</param>
        /// <param name="e">The instance containing the event data.</param>
        private static void OnShowRootLinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewAdv tree = d as TreeViewAdv;

            if (tree != null)
            {
                tree.UpdateShowRootLines();
            }
        }

        /// <summary>
        /// Check if item is dropped on his child.
        /// </summary>
        /// <param name="parent">Item that dropped</param>
        /// <param name="child">Drop target item</param>
        /// <param name="isItemsSource">if set to <c>true</c> [is items source].</param>
        /// <returns>
        /// True if parent contains child, else false
        /// </returns>
        internal static bool IsInChildren(TreeViewItemAdv parent, TreeViewItemAdv child, bool isItemsSource)
        {
            bool result = false;

            if (!isItemsSource)
            {
                if (parent == null || child == null)
                {
                    return result;
                }
                else
                {
                    if (parent.Items.Contains(child))
                    {
                        result = true;
                    }
                    else
                    {
                        TreeViewItemAdv item = null;

                        for (int i = 0; i < parent.Items.Count; i++)
                        {
                            item = (parent.Items[i] is TreeViewItemAdv) ?
                                (TreeViewItemAdv)parent.Items[i] :
                                parent.ItemContainerGenerator.ContainerFromItem(parent.Items[i]) as TreeViewItemAdv;

                            if (item != null)
                            {
                                if (item == child)
                                {
                                    result = true;
                                    break;
                                }
                                TreeViewAdv.IsInChildren(item, child, isItemsSource);
                            }
                        }
                    }
                }
            }
            else
            {
                for (TreeViewItemAdv parentTreeViewItem = child.ParentTreeViewItem; parentTreeViewItem != null; parentTreeViewItem = parentTreeViewItem.ParentTreeViewItem)
                {
                    if (parentTreeViewItem == parent)
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets user item that can be dragged from TreeViewItemAdv container.
        /// </summary>
        /// <param name="item">The item TreeView.</param>
        /// <param name="originalSource">original item mouse clicked on</param>
        /// <returns>
        /// dragged user item or null if no user items were dragged
        /// </returns>
        private TreeViewItemAdv GetDraggedUserItem(TreeViewItemAdv item, FrameworkElement originalSource)
        {
            TreeViewItemAdv possibleDragUserItem = null;

            if (item != null && originalSource != null)
            {
                FrameworkElement parent = originalSource.TemplatedParent as FrameworkElement;
                possibleDragUserItem = item.ItemContainerGenerator.ContainerFromItem(parent.DataContext) as TreeViewItemAdv;
            }

            return possibleDragUserItem;
        }

        /// <summary>
        /// internal variable which has dictionary for linear list
        /// </summary>
        internal System.Collections.Generic.Dictionary<object, double> LinearList = new System.Collections.Generic.Dictionary<object, double>();

        internal System.Collections.Generic.Dictionary<object, TreeViewItemAdv> LinearItems = new Dictionary<object, TreeViewItemAdv>();

        /// <summary>
        /// Contains item drop index
        /// </summary>
        private int finalindex = 0;

        /// <summary>
        /// Handles drag starting routines.
        /// </summary>
        internal static TreeViewItemAdv startitem;

        internal TreeViewItemAdv start_treeitem;

        /// <summary>
        /// Drag start the treeview item
        /// </summary>
        private void DragStarted()
        {
            finalindex = 0;
            if (droppedobjects != null) droppedobjects.Clear();

            Debug.WriteLine("Drag Started Called");
            if (m_draggingItmes != null && m_draggingItmesContiners != null)
            {
                TreeViewAdv.DragStartTreeView = this;
                if (m_selectedContainers != null && m_selectedContainers.Count > 0)
                    UpdateDraggingItems();
                DragTreeViewItemAdvEventArgs args = new DragTreeViewItemAdvEventArgs(DragStartEvent);
                args.Data = GetDragDataObject();

                if (m_draggingItmesContiners.Count > 0)
                {
                    args.DraggingItems = m_draggingItmesContiners;
                    if (args.DraggingItems.Count != 0)
                    {
                        startitem = (TreeViewItemAdv)args.DraggingItems[0];
                    }
                    if (startitem != null)
                    {
                        int dragindex = this.ItemContainerGenerator.IndexFromContainer(startitem);
                        int selectindex = 0;
                        if (dragindex > 0)
                            selectindex = dragindex - 1;
                        else
                        {
                            if (startitem.ParentItemsControl != null)
                            {
                                dragindex = (startitem.ParentItemsControl).ItemContainerGenerator.IndexFromContainer(startitem);
                                if (dragindex > 0)
                                    selectindex = dragindex - 1;
                                else
                                    selectindex = 0;
                            }
                            else
                                dragindex = 0;
                        }
                        if (startitem.ParentItemsControl != null)
                        {
                            if (startitem.ParentItemsControl.ItemContainerGenerator.ContainerFromIndex(selectindex) != null)
                                dropSelectContainer = startitem.ParentItemsControl.ItemContainerGenerator.ContainerFromIndex(selectindex) as TreeViewItemAdv;
                        }
                        else
                        {
                            if (this.ItemContainerGenerator.ContainerFromIndex(selectindex) != null)
                                dropSelectContainer = this.ItemContainerGenerator.ContainerFromIndex(selectindex) as TreeViewItemAdv;
                        }

                        DraggedTreeView = startitem.ParentTreeView;
                    }
                }
                TreeViewItemAdvDragDropEffects localEffects;
                if (dragdropeffects == TreeViewItemAdvDragDropEffects.None)
                {
                    localEffects = DragDropEffect;
                    dragdropeffects = localEffects;
                }
                else
                {
                    localEffects = dragdropeffects;
                }
                args.Effects = localEffects;
                args.Source = TreeViewAdv.DragStartTreeView;
                RaiseEvent(args);
                TreeViewAdv.DragOverTreeView = this;

                if (args.Data != null)
                {
                    TreeViewAdv.DragData = args.Data.GetData(typeof(TreeObjectCollection)) as TreeObjectCollection;
                }

                m_bIsDragging = true;

                if (args.Data != null && args.AllowDragDrop)
                {
                    DragDropEffects effects = DragDropEffects.None;
                    if (dragdropeffects == TreeViewItemAdvDragDropEffects.Move)
                    {
                        effects = DragDropEffects.Move;
                    }
                    else if (dragdropeffects == TreeViewItemAdvDragDropEffects.Copy)
                    {
                        effects = DragDropEffects.Copy;
                    }
                    else if (dragdropeffects == TreeViewItemAdvDragDropEffects.MoveOnly)
                    {
                        effects = DragDropEffects.Move;
                    }
                    else if (dragdropeffects == TreeViewItemAdvDragDropEffects.CopyOnly)
                    {
                        effects = DragDropEffects.Copy;
                    }
                    if (localEffects != args.Effects)
                    {
                        effects = ConvertToDragDropEffects(args.Effects);
                    }

                    DragDropEffects de;
                    if (dragdropeffects == TreeViewItemAdvDragDropEffects.MoveOnly)
                        de = DragDrop.DoDragDrop(this, args.Data, DragDropEffects.Move);
                    else if (dragdropeffects == TreeViewItemAdvDragDropEffects.CopyOnly)
                        de = DragDrop.DoDragDrop(this, args.Data, DragDropEffects.Copy);
                    else if (dragdropeffects == TreeViewItemAdvDragDropEffects.None)
                        de = DragDrop.DoDragDrop(this, args.Data, DragDropEffects.None);
                    else
                        de = DragDrop.DoDragDrop(this, args.Data, DragDropEffects.Copy | DragDropEffects.Move);

                    TreeViewAdv.DragStartTreeView = null;

                    if (!dropcancelstatus && de == DragDropEffects.Move)
                    {
                        if (dragOverItem != null && m_draggingItmesContiners != null &&
                            m_draggingItmesContiners.Count > 0 &&
                            !(dragOverItem as TreeViewItemAdv).Equals((m_draggingItmesContiners[0] as
                            TreeViewItemAdv)))
                            RemoveDragItem();
                        else if (dragOverItem == null && args.DraggingItems != null && (args.DraggingItems[0].ParentTreeViewItem != null && !args.DraggingItems[0].ParentTreeViewItem.Equals(temptargetItem)))
                            RemoveDragItem();
                        else if (dragOverItem == null && args.DraggingItems != null && args.DraggingItems[0].ParentTreeViewItem == null)
                            RemoveDragItem();
                    }
                    else if (!dropcancelstatus && de == DragDropEffects.None)
                    {
                        if (TreeViewAdv.DragOverTreeView != null)
                            TreeViewAdv.DragOverTreeView.DraggingItmesContiners.Clear();
                    }
                }

                SetIsDragingProperty(false);
                m_dragPopup.IsOpen = false;
                m_bIsDragging = false;
                m_bIsAvailableStartDrag = false;
                TreeViewAdv.ReleaseDragDropData(this);
                if (this != null)
                {
                    if (this.m_previoustargetitem != null)
                    {
                        if (this.m_previoustargetitem.m_topdragLine != null)
                        {
                            this.m_previoustargetitem.m_topdragLine.Visibility = Visibility.Collapsed;
                        }

                        if (this.m_previoustargetitem.m_bottomdragLine != null)
                        {
                            this.m_previoustargetitem.m_bottomdragLine.Visibility = Visibility.Collapsed;
                        }

                        this.m_previoustargetitem = null;
                    }
                }
            }
        }

        /// <summary>
        /// Gets data for drag.
        /// </summary>
        /// <returns>Data object value </returns>
        private DataObject GetDragDataObject()
        {
            DataObject data = null;
            if (IsVirtualizing && VirtualizationMode == Tools.VirtualizationMode.Extended && m_draggingItmesContiners.Count == 0)
            {
                TreeObjectCollection sitems = new TreeObjectCollection();
                foreach (object item in SelectedItems)
                {
                    sitems.Add(item);
                }
                data = new DataObject(sitems);
            }
            else
            {
                if (m_draggingItmesContiners != null && m_draggingItmesContiners.Count > 0)
                {
                    TreeObjectCollection cloneData = new TreeObjectCollection();
                    ICloneable clone = null;

                    for (int i = 0; i < m_draggingItmesContiners.Count; i++)
                    {
                        clone = m_draggingItmesContiners[i] as ICloneable;

                        if (clone != null)
                        {
                            try
                            {
                                cloneData.Add(clone.Clone());
                            }
                            catch { }
                        }
                    }

                    data = new DataObject(cloneData);
                }
            }
            return data;
        }

        /// <summary>
        /// Gets the node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private TreeViewItemAdv GetNode(ItemsControl node, string path)
        {
            string name;
            int index = path.IndexOf(PathSeparator);
            if (index == -1)
                name = path;
            else
                name = path.Substring(0, index);

            for (int i = 0; i < node.Items.Count; i++)
            {
                TreeViewItemAdv item = node.Items[i] as TreeViewItemAdv;

                if (item != null)
                {
                    if (item.Header.ToString() == name)
                    {
                        if ((path == name))
                        {
                            return item;
                        }

                        string newPath = path.Substring(index + 1, path.Length - index - 1);
                        TreeViewItemAdv ret = GetNode(item, newPath);

                        if (ret != null)
                        {
                            return ret;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a node from the specified path. Make sure that the path does not end with a separator when calling this.
        /// This method works only in non-databinding treeview. TreeviewItem should be added in treeview using TreeViewAdv.Items.Add(new TreeViewItemAdv("Test")).
        /// </summary>
        /// <param name="path">The path of the node.</param>
        /// <returns>The node that has the specified path.</returns>
        /// <remarks>This method works only in non-databinding treeview. TreeviewItem should be added in treeview using TreeViewAdv.Items.Add(new TreeViewItemAdv("Test")).</remarks>
        public TreeViewItemAdv GetNodeFromPath(string path)
        {
            return GetNode(this, path);
        }

        /// <summary>
        /// Removes drag items.
        /// </summary>
        private void RemoveDragItem()
        {
            if (m_draggingItmes != null && m_draggingItmesContiners != null
                && m_draggingItmes.Count > 0 && m_draggingItmesContiners.Count > 0 || SelectedItems.Count > 0)
            {
                ItemsControl parent = null;
                ArrayList draggingItems = new ArrayList();
                TreeObjectCollection m_draggingItems = new TreeObjectCollection();
                TreeViewItemAdvCollection m_draggingItemsContainers = new TreeViewItemAdvCollection();
                if (IsVirtualizing && VirtualizationMode == VirtualizationMode.Extended && ParentContainer != null)
                {
                    IList list = (ParentContainer.ItemsSource == null) ?
        ParentContainer.Items as IList : ParentContainer.ItemsSource as IList;
                    if (list != null)
                    {
                        for (int i = 0; i < SelectedItems.Count; i++)
                        {
                            list.Remove(SelectedItems[i]);
                        }
                    }
                }
                else if (m_draggingItmes.Count > 0)
                {
                    for (int j = 0; j < m_draggingItmesContiners.Count; j++)
                    {
                        draggingItems.Add((m_draggingItmesContiners[j] as TreeViewItemAdv).m_ItemIndex);
                        draggingItems.Sort();
                    }
                    for (int j = 0; j < draggingItems.Count; j++)
                    {
                        for (int k = 0; k < m_draggingItmesContiners.Count; k++)
                        {
                            if (Convert.ToInt32(draggingItems[j]).Equals((m_draggingItmesContiners[k] as TreeViewItemAdv).m_ItemIndex))
                            {
                                m_draggingItems.Add(m_draggingItmes[k]);
                                m_draggingItemsContainers.Add(m_draggingItmesContiners[k]);
                                break;
                            }
                        }
                    }
                }

                for (int i = 0; i < m_draggingItems.Count; i++)
                {
                    parent = m_draggingItemsContainers[i].ParentItemsControl;

                    if (parent == null && m_draggingParentItmes.Count > i)
                    {
                        parent = m_draggingParentItmes[i];
                    }

                    if (parent != null)
                    {
                        if (parent.ItemsSource == null)
                        {
                            if (parent.Items.Count > 0)
                            {
                                parent.Items.Remove(m_draggingItmesContiners[i]);
                                parent.Items.Remove(m_draggingItems[i]);
                                parent.Items.Refresh();
                            }
                            TreeViewAdv.IterateItems(m_draggingItemsContainers[i], this);
                        }
                        else
                        {
                            IList list = (parent.ItemsSource == null) ?
    parent.Items as IList : parent.ItemsSource as IList;

                            if (list != null)
                            {
                                int index = (parent.ItemsSource == null) ?
                                    parent.ItemContainerGenerator.IndexFromContainer(m_draggingItemsContainers[i]) :
                                    list.IndexOf(m_draggingItems[i]);

                                int actualindex = (m_draggingItemsContainers[i] as TreeViewItemAdv).m_ItemIndex;

                                if (finalindex != -1)
                                {
                                    if (finalindex < actualindex)
                                    {
                                        if (actualindex != -1)
                                        {
                                            if (i >= 1)
                                            {
                                                if (InternalDrop)
                                                {
                                                    list.RemoveAt(actualindex - i);
                                                }
                                                else
                                                {
                                                    if (m_draggingItems.Count > i + 1)
                                                    {
                                                        if (list.Count > (actualindex + m_draggingItems.Count - i))
                                                            list.RemoveAt(actualindex + m_draggingItems.Count - i);
                                                    }
                                                    else
                                                    {
                                                        if (list.Count > (actualindex + 1))
                                                            list.RemoveAt(actualindex + 1);
                                                        else if (list.Count > actualindex)
                                                            list.RemoveAt(actualindex);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (InternalDrop)
                                                    list.RemoveAt(actualindex);
                                                else
                                                {
                                                    if (list.Count > (actualindex + m_draggingItems.Count - i) && !list.IsFixedSize)
                                                    {
                                                        if (list.IndexOf(m_draggingItems[0]) == actualindex)
                                                            list.RemoveAt(actualindex);
                                                        else
                                                            list.RemoveAt(actualindex + m_draggingItems.Count - i);
                                                    }
                                                    else if (list.Count > actualindex && !list.IsFixedSize)
                                                    {
                                                        list.RemoveAt(actualindex);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            list.RemoveAt(index);
                                        }
                                    }
                                    else if (isDragSelectedItem)
                                    {
                                        list.RemoveAt(index);
                                        isDragSelectedItem = false;
                                    }
                                    else if (actualindex >= i)
                                    {
                                        if (m_draggingItemsContainers[i].Header.Equals(list[actualindex - i]) || m_draggingItemsContainers[i].Header.ToString() == "{DisconnectedItem}")
                                        {
                                            list.RemoveAt(actualindex - i);
                                        }
                                        else
                                            list.Remove(m_draggingItemsContainers[i].Header);
                                    }
                                    parent.Items.Refresh();
                                    TreeViewAdv.IterateItems(m_draggingItemsContainers[i], this);

                                    droppedobjects.Add(m_draggingItemsContainers[i].m_treeviewitemactualobject);

                                    for (int k = 0; k < m_selectedContainers.Count; k++)
                                    {
                                        TreeViewItemAdv tree = m_selectedContainers[k] as TreeViewItemAdv;
                                        if (tree != null)
                                        {
                                            if (tree.m_treeviewitemactualobject != null)
                                            {
                                                if (tree.m_treeviewitemactualobject.Equals(m_draggingItmesContiners[i].m_treeviewitemactualobject))
                                                {
                                                    m_selectedContainers.Remove(tree);
                                                }
                                            }
                                            else
                                            {
                                                m_selectedContainers.Remove(tree);
                                            }
                                        }
                                    }
                                    for (int j = 0; j < SelectedItems.Count; j++)
                                    {
                                        TreeViewItemAdv tree = SelectedItems[j] as TreeViewItemAdv;
                                        if (tree != null)
                                        {
                                            if (tree != null)
                                            {
                                                if (tree.m_treeviewitemactualobject != null)
                                                {
                                                    if (tree.m_treeviewitemactualobject.Equals(m_draggingItmesContiners[i].m_treeviewitemactualobject))
                                                    {
                                                        SelectedItems.Remove(tree);
                                                    }
                                                }
                                                else
                                                {
                                                    SelectedItems.Remove(tree);
                                                }
                                            }
                                        }
                                    }

                                 
                                }
                            }
                        }
                    }

                    InternalPanelClearAllItems(parent);
                }

                if (this.DragDropEffect != TreeViewItemAdvDragDropEffects.None)
                {
                    UpdateDragDropSelection(ParentContainer, SelectedItems, true);
                }
                SetIsDragingProperty(false);
                m_draggingItmes.Clear();
                m_draggingParentItmes.Clear();
                m_draggingItmesContiners.Clear();
            }
        }

        /// <summary>
        /// Updates collection items that dragging.
        /// </summary>
        private void UpdateDraggingItems()
        {
            SetIsDragingProperty(false);
            m_draggingItmes.Clear();
            m_draggingItmesContiners.Clear();
            if (SelectedItems != null && SelectedItems.Count > 0
                && m_selectedContainers != null && m_selectedContainers.Count > 0
                && SelectedItems.Count == m_selectedContainers.Count)
            {
                object objItem = null;
                TreeViewItemAdv item = null;
                bool bNeedAdd = true;
                ItemsControl parentControl = null;

                for (int i = 0; i < SelectedItems.Count; i++)
                {
                    objItem = SelectedItems[i];
                    item = m_selectedContainers[i];
                    bNeedAdd = true;

                    for (int j = 0; j < m_selectedContainers.Count; j++)
                    {
                        if (m_selectedContainers[j].ParentItemsControl != null
                            && m_selectedContainers[j] == item.ParentItemsControl)
                        {
                            bNeedAdd = false;
                            break;
                        }
                    }

                    if (bNeedAdd)
                    {
                        m_draggingItmes.Add(objItem);
                        if (item != null && item.Header != null && item.Header.ToString() == "{DisconnectedItem}")
                            item.Header = item.m_treeviewitemactualobject;
                        m_draggingItmesContiners.Add(item);
                        if (item.ParentItemsControl == null)
                        {
                            if (parentControl == null)
                            {
                                int index = 0;
                                foreach (TreeViewItemAdv titem in m_selectedContainers)
                                {
                                    index++;
                                    if (titem.ParentItemsControl != null)
                                    {
                                        parentControl = titem.ParentItemsControl;
                                        if (m_draggingParentItmes.Count > index)
                                        {
                                            m_draggingParentItmes.RemoveAt(index);
                                            m_draggingParentItmes.Insert(index, parentControl);
                                            break;
                                        }
                                    }
                                }
                                if (m_draggingParentItmes.Count == 0 && DragTreeItem != null)
                                {
                                    m_draggingParentItmes.Add(DragTreeItem.ParentItemsControl);
                                    isDragSelectedItem = true;
                                }
                            }
                        }
                        else
                        {
                            if ((item.ParentItemsControl is TreeViewItemAdv) && !(m_draggingParentItmes.Contains(item.ParentItemsControl as TreeViewItemAdv)))
                                m_draggingParentItmes.Add(item.ParentItemsControl);
                        }
                    }
                }
            }
            else if (SelectedItem != null)
            {
                m_draggingItmes.Add(SelectedItem);
                m_draggingItmesContiners.Add(SelectedContainer);
                m_draggingParentItmes.Add(SelectedContainer.ParentItemsControl);
            }

            SetIsDragingProperty(true);
        }

        /// <summary>
        /// Sets Isdraging property for draging items.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        private void SetIsDragingProperty(bool value)
        {
            if (DraggingItmesContiners != null && DraggingItmesContiners.Count > 0)
            {
                for (int i = 0; i < DraggingItmesContiners.Count; i++)
                {
                    DraggingItmesContiners[i].IsDraging = value;
                }
            }
        }

        /// <summary>
        /// Converts DragDropEffects to TreeViewItemAdvDragDropEffects.
        /// </summary>
        /// <param name="effects">The effects.</param>
        /// <returns>tree view value </returns>
        private TreeViewItemAdvDragDropEffects ConvertFromDragDropEffects(DragDropEffects effects)
        {
            TreeViewItemAdvDragDropEffects treeEffects = TreeViewItemAdvDragDropEffects.None;

            if (changeddragdropeffects == TreeViewItemAdvDragDropEffects.Copy)
            {
                treeEffects = TreeViewItemAdvDragDropEffects.Copy;
            }
            else if (changeddragdropeffects == TreeViewItemAdvDragDropEffects.Move)
            {
                treeEffects = TreeViewItemAdvDragDropEffects.Move;
            }

            return treeEffects;
        }

        /// <summary>
        /// Converts TreeViewItemAdvDragDropEffects to DragDropEffects.
        /// </summary>
        /// <param name="effects">The effects.</param>
        /// <returns>Dragdropeffects value </returns>
        private DragDropEffects ConvertToDragDropEffects(TreeViewItemAdvDragDropEffects effects)
        {
            DragDropEffects treeEffects = DragDropEffects.None;

            if (effects == TreeViewItemAdvDragDropEffects.Copy)
            {
                treeEffects = DragDropEffects.Copy;
            }
            else if (effects == TreeViewItemAdvDragDropEffects.Move)
            {
                treeEffects = DragDropEffects.Move;
            }
            else if (effects == (TreeViewItemAdvDragDropEffects.Move | TreeViewItemAdvDragDropEffects.Copy))
            {
                treeEffects = DragDropEffects.Move | DragDropEffects.Copy;
            }
            else
            {
                treeEffects = DragDropEffects.None;
            }

            return treeEffects;
        }

        /// <summary>
        /// Handles drag ending routines.
        /// </summary>
        /// <param name="e">Drag end event arguments</param>
        private void DragEnded(DragEventArgs e)
        {
            startitem = null;
            m_ismouseSelection = false;
            if(m_dragPopup != null)
                m_dragPopup.IsOpen = false;
            TreeObjectCollection data = null;
            if (e.Data.GetDataPresent(typeof(TreeObjectCollection)))
            {
                try
                {
                    data = e.Data.GetData(typeof(TreeObjectCollection)) as TreeObjectCollection;
                }
                //SU I78477
                //catch (Exception ee)
                catch (Exception)
                //EU I78477
                {
                }
            }
            if (MultiColumnEnable)
                TreeViewAdv.DragOverControl = dragOverItem;
            ItemsControl targetItem = TreeViewAdv.DragOverControl;
            TreeViewAdv targetTree = null;
            bool bInternalDrop = true;

            if (data == null && e.Data != null)
            {
                DragTreeViewItemAdvEventArgs args = new DragTreeViewItemAdvEventArgs(DragEndEvent);
                args.Data = new DataObject(e.Data);
                if (targetItem != null && targetItem is TreeViewItemAdv && !(targetItem as TreeViewItemAdv).IsDragOver)
                {
                    args.TargetDropItem = (targetItem as TreeViewItemAdv).ParentItemsControl;
                    args.DropIndex = (targetItem as TreeViewItemAdv).GetDropIndex();
                }
                else
                {
                    args.TargetDropItem = targetItem;
                    args.DropIndex = TreeViewAdv.dropIndex;
                }
                args.DraggingItems = null;

                TreeViewItemAdvDragDropEffects localEffect = ConvertFromDragDropEffects(e.Effects);
                args.Effects = localEffect;
                args.Source = TreeViewAdv.DragStartTreeView;
                RaiseEvent(args);
            }
            else if (data != null)
            {
                DragTreeViewItemAdvEventArgs args = new DragTreeViewItemAdvEventArgs(DragEndEvent);
                args.Data = new DataObject(data);
                if (targetItem != null && targetItem is TreeViewItemAdv && !(targetItem as TreeViewItemAdv).IsDragOver)
                {
                    args.TargetDropItem = (targetItem as TreeViewItemAdv).ParentItemsControl;
                    args.DropIndex = (targetItem as TreeViewItemAdv).GetDropIndex();
                }
                else
                {
                    args.TargetDropItem = targetItem;
                    args.DropIndex = TreeViewAdv.dropIndex;
                }
                args.DraggingItems = TreeViewAdv.DragStartTreeView.m_draggingItmesContiners;
                TreeViewItemAdvDragDropEffects localEffect = ConvertFromDragDropEffects(e.Effects);
                args.Effects = localEffect;
                args.Source = TreeViewAdv.DragStartTreeView;
                RaiseEvent(args);

                TreeViewAdv.DragStartTreeView.dropcancelstatus = args.Cancel;
                if (!args.Cancel && args.AllowDragDrop)
                {
                    if (targetItem != null && targetItem is TreeViewItemAdv)
                    {
                        temptargetItem = (targetItem as TreeViewItemAdv);
                        if (data.Count > 0)
                        {
                            bool isDragOver = temptargetItem.IsDragOver;
                            ItemsControl parent = temptargetItem.ParentItemsControl;
                            int dropIndex = temptargetItem.GetDropIndex();
                            bInternalDrop = (parent != null && !isDragOver && dropIndex != -1) ?
                                false : true;
                            InternalDrop = bInternalDrop;
                        
                            for (int i = 0; i < data.Count; i++)
                            {
                                if (bInternalDrop)
                                {
                                    if (temptargetItem.ItemsSource != null)
                                    {
                                        IList list = temptargetItem.ItemsSource as IList;
                                        bool isListSource = false;
                                        if (list == null)
                                        {
                                            list = (temptargetItem.ItemsSource as IListSource).GetList();
                                            if (list != null)
                                                isListSource = true;
                                        }

                                        if (list != null)
                                        {
                                            if (args.DraggingItems[0].ParentTreeViewItem == null || (args.DraggingItems[0].ParentTreeViewItem != null && !args.DraggingItems[0].ParentTreeViewItem.Equals(temptargetItem)))
                                            {
                                                try
                                                {
                                                    if (isListSource)
                                                    {
                                                        if (data[i] is TreeViewItemAdv)
                                                        {
                                                            TreeViewItemAdv tvAdv = (TreeViewItemAdv)data[i];
                                                            if (tvAdv.DataContext != null)
                                                                list.Add(tvAdv.DataContext);
                                                            else
                                                                list.Add(tvAdv.Header);
                                                        }
                                                        else
                                                            list.Add(data[i]);
                                                    }
                                                    else
                                                        list.Add(data[i]);
                                                }
                                                catch
                                                {
                                                    TreeViewItemAdv item = data[i] as TreeViewItemAdv;

                                                    if (item != null)
                                                    {
                                                        try
                                                        {
                                                            if (!temptargetItem.Header.Equals(item.Header))
                                                            {
                                                                if (args.Effects == TreeViewItemAdvDragDropEffects.Move)
                                                                {
                                                                    list.Add(item.Header);
                                                                }
                                                                else if (args.Effects == TreeViewItemAdvDragDropEffects.Copy)
                                                                {
                                                                    list.Add(item.Header);
                                                                }
                                                            }
                                                        }
                                                        catch
                                                        {
                                                            args.Effects = TreeViewItemAdvDragDropEffects.None;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        TreeViewItemAdv item = data[i] as TreeViewItemAdv;
                                        if (args.DraggingItems[0].ParentTreeViewItem == null || (args.DraggingItems[0].ParentTreeViewItem != null && !args.DraggingItems[0].ParentTreeViewItem.Equals(temptargetItem)))
                                        {
                                            try
                                            {
                                                temptargetItem.Items.Add(data[i]);
                                            }
                                            catch
                                            {
                                                if (item != null)
                                                {
                                                    temptargetItem.Items.Add(item);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    InternalDrop = false;
                                    int reverseIndex = data.Count - i - 1;

                                    if (parent.ItemsSource != null)
                                    {
                                        IList list = parent.ItemsSource as IList;

                                        if (list == null && (parent.ItemsSource as IListSource) != null)
                                            list = (parent.ItemsSource as IListSource).GetList();

                                        if (list != null)
                                        {
                                            TreeViewItemAdv item = data[reverseIndex] as TreeViewItemAdv;

                                            int index;
                                            if (IsVirtualizing && VirtualizationMode == VirtualizationMode.Extended)
                                                index = list.IndexOf(data[reverseIndex]);
                                            else
                                                index = list.IndexOf(item.Header);

                                            if (dropIndex < index)
                                            {
                                                index++;
                                            }

                                            finalindex = dropIndex;

                                            try
                                            {
                                                if (IsVirtualizing && VirtualizationMode == VirtualizationMode.Extended)
                                                {
                                                    if (SelectedItems.Count > 0)
                                                    {
                                                        list.Insert(dropIndex, this.SelectedItems[drag_count - 1]);
                                                        drag_count--;
                                                    }
                                                }
                                                else if (this.DraggingItmes.Count > 0)
                                                {
                                                    list.Insert(dropIndex, this.DraggingItmes[drag_count - 1]);
                                                    drag_count--;
                                                }
                                            }
                                            catch
                                            {
                                                if (IsVirtualizing && VirtualizationMode == VirtualizationMode.Extended)
                                                {
                                                    if (SelectedItems.Count > 0)
                                                    {
                                                        list.Insert(dropIndex, this.SelectedItems[drag_count - 1]);
                                                        drag_count--;
                                                    }
                                                }
                                                else if (this.DraggingItmes.Count > 0)
                                                {
                                                    try
                                                    {
                                                        list.Insert(dropIndex, this.DraggingItmes[drag_count - 1]);
                                                        drag_count--;
                                                    }
                                                    catch
                                                    { }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        parent.Items.Insert(dropIndex, data[reverseIndex]);
                                    }
                                }
                            }
                            
                            if (!(args.Effects == TreeViewItemAdvDragDropEffects.Move || args.Effects == TreeViewItemAdvDragDropEffects.MoveOnly) || temptargetItem.Equals(args.DraggingItems[0].ParentTreeViewItem) || temptargetItem.Header.Equals(args.DraggingItems[0].Header))
                                InternalPanelClearAllItems(parent);
                        }

                        temptargetItem.IsDragOver = false;
                        temptargetItem.StopDragTimer();
                    }
                    else if (TreeViewAdv.DragOverControl is TreeViewAdv)
                    {
                        targetTree = (TreeViewAdv)TreeViewAdv.DragOverControl;
                        int addedIndex = -1;

                        for (int i = 0; i < data.Count; i++)
                        {
                            if (targetTree.ItemsSource != null)
                            {
                                IList list = targetTree.ItemsSource as IList;
                                bool isListSource = false;
                                if (list == null)
                                {
                                    list = (targetTree.ItemsSource as IListSource).GetList();
                                    if (list != null)
                                        isListSource = true;
                                }
                                if (list != null)
                                {
                                    TreeViewItemAdv item = data[i] as TreeViewItemAdv;

                                    int index1 = list.IndexOf(item.Header);

                                    if (list.Count < index1)
                                    {
                                        index1++;
                                    }

                                    if (i == 0)
                                    {
                                        finalindex = list.Count;
                                    }

                                    try
                                    {
                                        if (isListSource)
                                        {
                                            if (data[i] is TreeViewItemAdv)
                                            {
                                                TreeViewItemAdv tvAdv = (TreeViewItemAdv)data[i];
                                                if (tvAdv.DataContext != null)
                                                    addedIndex = list.Add(tvAdv.DataContext);
                                                else
                                                    addedIndex = list.Add(tvAdv.Header);
                                            }
                                            else
                                                addedIndex = list.Add(data[i]);
                                        }
                                        else
                                            addedIndex = list.Add(data[i]);
                                    }
                                    catch
                                    {
                                        if (item != null)
                                        {
                                            addedIndex = list.Add(item.Header);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                addedIndex = targetTree.Items.Add(data[i]);
                            }

                            
                        }
                    }

                    e.Effects = ConvertToDragDropEffects(args.Effects);
                    
                }

                ItemsControl targetItemsControl = (targetTree == null) ? targetItem as ItemsControl : targetTree as ItemsControl;

                if (targetItem != null && targetItem is TreeViewItemAdv && !(targetItem as TreeViewItemAdv).IsDragOver && (targetItem as TreeViewItemAdv).ParentItemsControl != null)
                {
                    targetItemsControl = (targetItem as TreeViewItemAdv).ParentItemsControl;
                }
                else
                {
                    targetItemsControl = targetItem;
                }
                if (this.DragDropEffect != TreeViewItemAdvDragDropEffects.None && ParentContainer == null && !(targetItemsControl is TreeViewAdv))
                {
                    UpdateDragDropSelection(targetItemsControl, data, bInternalDrop);
                }
            }

            if (targetItem != null && targetItem is TreeViewItemAdv)
            {
                (targetItem as TreeViewItemAdv).IsDragOver = false;
            }

            m_dragPopup.IsOpen = false;
            TreeViewAdv.ReleaseDragDropData(this);
            TreeViewAdv.dropIndex = -1;
            dragdropeffects = TreeViewItemAdvDragDropEffects.None;
            StopAutoScroll();
            if (this != null)
            {
                if (this.m_previoustargetitem != null)
                {
                    if (this.m_previoustargetitem.m_topdragLine != null)
                    {
                        this.m_previoustargetitem.m_topdragLine.Visibility = Visibility.Collapsed;
                    }

                    if (this.m_previoustargetitem.m_bottomdragLine != null)
                    {
                        this.m_previoustargetitem.m_bottomdragLine.Visibility = Visibility.Collapsed;
                    }

                    this.m_previoustargetitem = null;
                }
            }
        }

        /// <summary>
        /// Invoked when an unhandled DragDrop.DragOver attached event reaches an element
        /// in its route that is derived from this class. Implement this method to
        /// add class handling for this event.
        /// </summary>
        /// <param name="e">The DragEventArgs that contains the event data.</param>
        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
            if (this.AllowDragDrop)
            {
                TreeViewAdv.DragOverElement = e.Source as FrameworkElement;

                if (e.Source is TreeViewAdv)
                {
                    TreeViewAdv.DragOverTreeView = e.Source as TreeViewAdv;
                }
                else if (e.Source is TreeViewItemAdv)
                {
                    TreeViewAdv.DragOverTreeView = (e.Source as TreeViewItemAdv).ParentTreeView;
                }

                if (dragdropeffects == TreeViewItemAdvDragDropEffects.Move)
                {
                    if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                    {
                        e.Effects = DragDropEffects.Move;
                        changeddragdropeffects = TreeViewItemAdvDragDropEffects.Move;
                        m_dragshortcutkeysenabled = true;
                        e.Handled = true;
                    }
                    else if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        e.Effects = DragDropEffects.Copy;
                        changeddragdropeffects = TreeViewItemAdvDragDropEffects.Copy;
                        m_dragshortcutkeysenabled = true;
                        e.Handled = true;
                    }

                    else if (m_dragshortcutkeysenabled && (Keyboard.IsKeyUp(Key.LeftCtrl) && Keyboard.IsKeyUp(Key.RightCtrl) && Keyboard.IsKeyUp(Key.LeftShift) && Keyboard.IsKeyUp(Key.RightShift)))
                    {
                        e.Effects = DragDropEffects.Move;
                        changeddragdropeffects = TreeViewItemAdvDragDropEffects.Move;
                        e.Handled = true;
                    }
                    else if (!m_dragshortcutkeysenabled)
                    {
                        e.Effects = DragDropEffects.Move;
                        changeddragdropeffects = TreeViewItemAdvDragDropEffects.Move;
                        e.Handled = true;
                    }
                }
                else if (dragdropeffects == TreeViewItemAdvDragDropEffects.Copy)
                {
                    if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                    {
                        e.Effects = DragDropEffects.Move;
                        changeddragdropeffects = TreeViewItemAdvDragDropEffects.Move;
                        m_dragshortcutkeysenabled = true;
                        e.Handled = true;
                    }
                    else if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        e.Effects = DragDropEffects.Copy;
                        changeddragdropeffects = TreeViewItemAdvDragDropEffects.Copy;
                        m_dragshortcutkeysenabled = true;
                        e.Handled = true;
                    }

                    else if (m_dragshortcutkeysenabled && (Keyboard.IsKeyUp(Key.LeftCtrl) || Keyboard.IsKeyUp(Key.RightCtrl) || Keyboard.IsKeyUp(Key.LeftShift) && Keyboard.IsKeyUp(Key.RightShift)))
                    {
                        e.Effects = DragDropEffects.Copy;
                        changeddragdropeffects = TreeViewItemAdvDragDropEffects.Copy;
                        e.Handled = true;
                    }
                    else if (!m_dragshortcutkeysenabled)
                    {
                        e.Effects = DragDropEffects.Copy;
                        changeddragdropeffects = TreeViewItemAdvDragDropEffects.Copy;
                        e.Handled = true;
                    }
                }
                else if (dragdropeffects == TreeViewItemAdvDragDropEffects.CopyOnly)
                {
                    e.Effects = DragDropEffects.Copy;
                    changeddragdropeffects = TreeViewItemAdvDragDropEffects.Copy;
                    e.Handled = true;
                }
                else if (dragdropeffects == TreeViewItemAdvDragDropEffects.MoveOnly)
                {
                    e.Effects = DragDropEffects.Move;
                    changeddragdropeffects = TreeViewItemAdvDragDropEffects.Move;
                    e.Handled = true;
                }

                DragOverItemsUpdate();

                if (changeddragdropeffects == TreeViewItemAdvDragDropEffects.None)
                {
                    e.Effects = DragDropEffects.None;
                    TreeViewAdv.DragOverControl = null;
                    TreeViewAdv.DragOverElement = null;
                }

                if (argsdragdropeffetschanged == TreeViewItemAdvDragDropEffects.Copy)
                {
                    e.Effects = DragDropEffects.Copy;
                    changeddragdropeffects = TreeViewItemAdvDragDropEffects.Copy;
                }
                else if (argsdragdropeffetschanged == TreeViewItemAdvDragDropEffects.Move)
                {
                    e.Effects = DragDropEffects.Move;
                    changeddragdropeffects = TreeViewItemAdvDragDropEffects.Move;
                }

                if (!TreeViewAdv.IsPossipleDrop)
                {
                    e.Effects = DragDropEffects.None;
                    changeddragdropeffects = TreeViewItemAdvDragDropEffects.None;
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Invoked when an unhandled DragDrop.DragEnter attached event reaches an element
        /// in its route that is derived from this class. Implement this method to
        /// add class handling for this event.
        /// </summary>
        /// <param name="e">The DragEventArgs that contains the event data.</param>
        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
            if (IsVirtualizing && VirtualizationMode == VirtualizationMode.Extended)
                drag_count = SelectedItems.Count;
            else
                drag_count = this.DraggingItmes.Count;
            if (!TreeViewAdv.IsPossipleDrop)
            {
                e.Effects = DragDropEffects.None;
                changeddragdropeffects = TreeViewItemAdvDragDropEffects.None;
                if (e.KeyStates == DragDropKeyStates.RightMouseButton)
                {
                    DragDrop.AddQueryContinueDragHandler(this, QueryContinueDragHandler);
                }
            }
        }

        private void QueryContinueDragHandler(Object source, QueryContinueDragEventArgs e)
        {
            if (e.KeyStates == DragDropKeyStates.RightMouseButton)
            {
                e.Handled = true;
                e.Action = DragAction.Continue;
            }
        }

        /// <summary>
        /// Tries to remove adorner from adorner layer of
        /// logical parent for last item.
        /// </summary>
        private void TryRemoveLastItemAdorner()
        {
            if (Items.Count > 0)
            {
                TreeViewItemAdv lastItem = null;

                for (int i = Items.Count - 1; i >= 0; i--)
                {
                    lastItem = ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv;

                    if (lastItem == null)
                    {
                        lastItem = Items[i] as TreeViewItemAdv;
                    }

                    if (lastItem != null && lastItem.TryRemoveAdorner(lastItem.AdornerMarker))
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets visual brush of the item.
        /// </summary>
        /// <param name="item">The item TreeView.</param>
        /// <returns>Visual brush value </returns>
        public VisualBrush GetItemBrush(TreeViewItemAdv item)
        {
            VisualBrush brush = null;
            dragvisualborder.Width = 0;
            if (MultiColumnEnable)
            {
                ArrayList treeviewcolumns = new ArrayList();
                foreach (TreeViewColumn column in Columns)
                {
                    if (column.IsColumnDragDataEnabled)
                    {
                        treeviewcolumns.Add(column);
                    }
                }
                if ((treeviewcolumns.Count > 0 && !treeviewcolumns.Count.Equals(Columns.Count)) || DragAdornerWidth > 0)
                {
                    String finalheader = "    ";
                    foreach (TreeViewColumn column in treeviewcolumns)
                    {
                        string propertyname = ((Binding)column.DisplayMemberBinding).Path.Path.ToString();
                        object obj = item.m_treeviewitemactualobject as object;
                        if (obj != null)
                        {
                            PropertyInfo p = obj.GetType().GetProperty(propertyname);
                            if (p != null)
                            {
                                finalheader = finalheader + p.GetValue(obj, null) + "   ";
                            }
                        }
                    }

                    if (item != null)
                    {
                        FrameworkTemplate treeitemtemplate = item.Template as FrameworkTemplate;
                        Border itemborder = treeitemtemplate.FindName("Bd", item) as Border;
                        visualtext.TextTrimming = TextTrimming.CharacterEllipsis;
                        visualtext.Text = finalheader.ToString();
                        visualtext.Background = itemborder.Background;
                        visualtext.Foreground = item.Foreground;
                        dragvisualborder.BorderBrush = itemborder.BorderBrush;
                        dragvisualborder.CornerRadius = itemborder.CornerRadius;
                        dragvisualborder.BorderThickness = itemborder.BorderThickness;
                        dragvisualborder.MinHeight = itemborder.MinHeight;
                        dragvisualborder.Height = item.CompleteHeaderElement.Height;
                        dragvisualborder.Width = visualtext.Text.Length * 5;
                        brush = new VisualBrush(dragvisualborder);
                        brush.Stretch = Stretch.None;
                    }
                }
                else
                {
                    if (item != null)
                    {
                        brush = new VisualBrush(item.CompleteHeaderElement);
                        brush.Stretch = Stretch.None;
                    }
                }
            }
            else
            {
                if (item != null)
                {
                    brush = new VisualBrush(item.CompleteHeaderElement);
                    brush.Stretch = Stretch.None;
                }
            }
            return brush;
        }

        /// <summary>
        /// Finds focused element from KeyboardFocusChangedEventArgs.
        /// </summary>
        /// <param name="e">KeyboardFocusChanged EventArgs</param>
        /// <returns>new focused element</returns>
        private TreeViewItemAdv FindNewFocusedElement(KeyboardFocusChangedEventArgs e)
        {
            TreeViewItemAdv focusedItem = null;
            FrameworkElement newFocus = e.NewFocus as FrameworkElement;
            TreeViewItemAdv source = e.Source as TreeViewItemAdv;

            if (source != null && newFocus != null)
            {
                focusedItem = source.ItemContainerGenerator.ContainerFromItem(newFocus.DataContext) as TreeViewItemAdv;

                if (focusedItem == null)
                {
                    focusedItem = source;
                }
            }

            return focusedItem;
        }

        /// <summary>
        /// Selects given item.
        /// </summary>
        /// <param name="newFocused">item to be selected</param>
        private void SelectItem(TreeViewItemAdv newFocused)
        {
            if (newFocused != null)
            {
                if (!newFocused.IsSelected)
                {
                    newFocused.Select(true);
                }
                else
                {
                    newFocused.Select(false);
                }
            }
        }

        /// <summary>
        /// Sets focus on the first node.
        /// </summary>
        /// <returns> bool value </returns>
        private bool FocusFirstItem()
        {
            bool focused = false;
            object obj = Items[0];
            TreeViewItemAdv item = obj is TreeViewItemAdv ? (TreeViewItemAdv)obj :
                ItemContainerGenerator.ContainerFromItem(obj) as TreeViewItemAdv;

            if (item != null && item.IsEnabled)
            {
                item.Focus();
                m_ismouseSelection = true;
                item.Select(true);
                this.SelectedContainer.Focus();
                m_ismouseSelection = false;
                focused = item.IsFocused;
            }

            return focused;
        }

        /// <summary>
        /// Focuses the last item.
        /// </summary>
        /// <returns>bool  value </returns>
        private bool FocusLastItem()
        {
            bool focused = false;

            object obj = Items[Items.Count - 1];
            TreeViewItemAdv item = obj is TreeViewItemAdv ? (TreeViewItemAdv)obj :
                ItemContainerGenerator.ContainerFromItem(obj) as TreeViewItemAdv;

            while (item != null)
            {
                int count = item.Items.Count;

                if (item.IsExpanded && count > 0)
                {
                    obj = item.Items[count - 1];
                    item = obj is TreeViewItemAdv ? (TreeViewItemAdv)obj :
                                ItemContainerGenerator.ContainerFromItem(obj) as TreeViewItemAdv;
                }
                else
                {
                    break;
                }
            }

            if (item != null && item.IsEnabled)
            {
                item.Focus();
                m_ismouseSelection = true;
                item.Select(true);
                this.SelectedContainer.Focus();
                m_ismouseSelection = false;
                focused = item.IsFocused;
            }

            return focused;
        }

        internal int internaltargetindex = 0;
        private TreeViewItemAdv internaltargetnode = null;

        /// <summary>
        /// Finds nodes and selects or unselects diapason between them.
        /// </summary>
        /// <param name="currentNode">first node from diapason</param>
        /// <param name="targetNode">second node from diapason</param>
        /// <param name="findedTarget">true if target item was found</param>
        /// <param name="select">true if need select item</param>
        private void FindSelectItems(TreeViewItemAdv currentNode, TreeViewItemAdv targetNode, ref bool findedTarget, bool select)
        {
            bool findSelectionBorder = false;
            int counter = 0;
            int itemsCnt = Items.Count;
            TreeViewItemAdv firstItem;
            object firstObject;
            if (targetNode != null && targetNode.ParentItemsControl != null)
            {
                internaltargetindex = targetNode.ParentItemsControl.ItemContainerGenerator.IndexFromContainer(targetNode);
                internaltargetnode = targetNode;
            }
            if (currentNode != null && currentNode.ParentItemsControl != null)
            {
                internalcurrentindex = currentNode.ParentItemsControl.ItemContainerGenerator.IndexFromContainer(currentNode);
                internalcurrentnode = currentNode;
            }
            GetFirstItem(out firstObject, out firstItem);

            if (firstItem != null && (!IsVirtualizing || VirtualizationMode == VirtualizationMode.Normal))
            {
                m_itemsStack.Push(firstItem);
            }
            int currentIndex = 0;
            int targetIndex = 0;
            if (currentNode != null)
            {
                if (currentNode.ParentItemsControl == null && (targetNode.ParentItemsControl as TreeViewItemAdv) != null)
                {
                    currentIndex = (targetNode.ParentItemsControl as TreeViewItemAdv).Items.IndexOf(currentNode.Header);
                }
                else if (currentNode.ParentItemsControl != null)
                    currentIndex = currentNode.ParentItemsControl.ItemContainerGenerator.IndexFromContainer(currentNode);
                else
                    currentIndex = this.ItemContainerGenerator.IndexFromContainer(currentNode);

                tempCurrentNode = currentNode;
                if (currentNode.ParentItemsControl == null)
                    isSelectItemsChanged = true;
            }
            if (targetNode != null)
            {
                if (targetNode.ParentItemsControl != null)
                    targetIndex = targetNode.ParentItemsControl.ItemContainerGenerator.IndexFromContainer(targetNode);
                else
                    targetIndex = this.ItemContainerGenerator.IndexFromContainer(targetNode);

                if (targetNode.ParentTreeViewItem != null)
                    tempTargetIndex = this.ItemContainerGenerator.IndexFromContainer(targetNode.ParentTreeViewItem);
                else
                    tempTargetIndex = this.ItemContainerGenerator.IndexFromContainer(targetNode);

                tempTargetNode = targetNode;
                if (targetNode.IsVisible == true && (currentNode == null || currentNode.ParentItemsControl == null))
                    isTargetNodeParentNotVisible = true;
            }
            if (IsVirtualizing && VirtualizationMode == Tools.VirtualizationMode.Extended)
            {
                if (currentNode != null && currentNode.ParentItemsControl != null && targetNode.ParentItemsControl != null && currentNode.ParentItemsControl != targetNode.ParentItemsControl && (currentIndex != 0 || targetIndex != 0))
                {
                    do
                    {
                        TreeViewItemAdv node = GetNextTreeViewItemAdv(ref counter);

                        if (node == currentNode || node == targetNode)
                        {
                            if (!findSelectionBorder)
                            {
                                findSelectionBorder = true;
                            }
                            else
                            {
                                findSelectionBorder = false;
                                findedTarget = true;

                                if (select)
                                {
                                    if (!IsMultiselection)
                                    {
                                        IsMultiselection = true;
                                    }
                                    AddNodeToSelectedItems(node);
                                }
                                else
                                {
                                    RemoveNodeFromSelectedItems(node);
                                }

                                break;
                            }
                        }

                        if (currentNode.ParentItemsControl != null)
                            TempParentItemsControl = null;
                        if (findSelectionBorder)
                        {
                            if (select)
                            {
                                if (!IsMultiselection)
                                {
                                    IsMultiselection = true;
                                }
                                AddNodeToSelectedItems(node);
                            }
                            else
                            {
                                RemoveNodeFromSelectedItems(node);
                            }
                        }

                        if (node != null && node.IsExpanded)
                        {
                            for (int i = node.Items.Count - 1; i >= 0; i--)
                            {
                                PushInnerItemToStack(node, i);
                            }
                        }
                    }
                    while (m_itemsStack.Count != 0 || ((itemsCnt > counter + 1) && Items[counter + 1] != null));
                }
                else if ((startindex == -1 && currentIndex == -1 && TempParentItemsControl != targetNode.ParentItemsControl))
                {
                    if (currentNode != null && currentNode.ParentItemsControl == null && targetNode.ParentItemsControl != null && (currentIndex != 0 || targetIndex != 0))
                    {
                        do
                        {
                            TreeViewItemAdv node = GetNextTreeViewItemAdv(ref counter);

                            if ((node == currentNode) || (node == targetNode && isSet))
                            {
                                if (select)
                                {
                                    if (!IsMultiselection)
                                    {
                                        IsMultiselection = true;
                                    }
                                    AddNodeToSelectedItems(node);
                                }
                                else
                                {
                                    RemoveNodeFromSelectedItems(node);
                                }
                                isSet = false;
                                break;
                            }

                            else if (node == null && isSet)
                            {
                                isSet = false;
                                break;
                            }

                            if (tempTargetIndex <= tempCurrentIndex)
                            {
                                if (node != null && targetNode == node)
                                {
                                    AddNodeToSelectedItems(node);
                                    isSet = true;
                                }

                                else if (isSet)
                                {
                                    AddNodeToSelectedItems(node);
                                }
                            }

                            else if (TempParentItemsControl != node && node != null && tempTargetIndex > tempCurrentIndex)
                            {
                                if (node.Header != modelItems[0].iTree && !node.IsSelected)
                                {
                                    AddNodeToSelectedItems(node);
                                    isSet = true;
                                }
                            }

                            if (node != null && node.IsExpanded)
                            {
                                for (int i = node.Items.Count - 1; i >= 0; i--)
                                {
                                    PushInnerItemToStack(node, i);
                                }
                            }
                        }
                        while (m_itemsStack.Count != 0 || ((itemsCnt > counter + 1) && Items[counter + 1] != null));
                    }
                }
                else if (currentNode == null && currentIndex == 0 && TempParentItemsControl != targetNode.ParentItemsControl)
                {
                    do
                    {
                        TreeViewItemAdv node = GetNextTreeViewItemAdv(ref counter);

                        if (node == targetNode && isSet)
                        {
                            if (select)
                            {
                                if (!IsMultiselection)
                                {
                                    IsMultiselection = true;
                                }
                                AddNodeToSelectedItems(node);
                            }
                            else
                            {
                                RemoveNodeFromSelectedItems(node);
                            }
                            isSet = false;
                            break;
                        }

                        else if (node == null && isSet)
                        {
                            isSet = false;
                            break;
                        }

                        if (tempTargetIndex <= tempCurrentIndex)
                        {
                            if (node != null && targetNode == node)
                            {
                                AddNodeToSelectedItems(node);
                                isSet = true;
                            }

                            else if (isSet)
                            {
                                AddNodeToSelectedItems(node);
                            }
                        }

                        else if (TempParentItemsControl != node && node != null && tempTargetIndex > tempCurrentIndex)
                        {
                            AddNodeToSelectedItems(node);
                            isSet = true;
                        }

                        if (node != null && node.IsExpanded)
                        {
                            for (int i = node.Items.Count - 1; i >= 0; i--)
                            {
                                PushInnerItemToStack(node, i);
                            }
                        }
                    }
                    while (m_itemsStack.Count != 0 || ((itemsCnt > counter + 1) && Items[counter + 1] != null));
                }
                else
                {
                    if (currentIndex >= 0 && currentIndex != m_startIndex)
                        m_startIndex = currentIndex;
                    counter = m_startIndex;
                    if (TempParentItemsControl != null)
                        TempParentItemsControl = null;
                    if (!canExecuteshift)
                    {
                        foreach (object x in m_selectedContainers)
                        {
                            TreeViewItemAdv ti = x as TreeViewItemAdv;
                            ti.IsSelected = false;
                        }
                        SelectedItems.Clear();
                        m_selectedContainers.Clear();
                        DraggingParentItmes.Clear();
                    }
                    else
                    {
                        canExecuteshift = false;
                    }
                    TreeViewItemAdv selecteditem = null;
                    if (m_startIndex >= 0 && targetIndex >= 0)
                    {
                        if (m_startIndex > targetIndex)
                        {
                            for (int i = targetIndex; i <= m_startIndex; i++)
                            {
                                if ((currentNode == null || currentNode.ParentItemsControl == null) && (targetNode.ParentItemsControl as TreeViewItemAdv) != null)
                                {
                                    selecteditem = (targetNode.ParentItemsControl.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv);
                                    SelectedItems.Add(targetNode.ParentItemsControl.Items[i]);
                                }
                                if (currentNode.ParentItemsControl != null)
                                {
                                    selecteditem = currentNode.ParentItemsControl.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv;
                                    SelectedItems.Add(currentNode.ParentItemsControl.Items[i]);
                                }
                                else if (selecteditem == null && targetNode.ParentItemsControl != null)
                                {
                                    selecteditem = targetNode.ParentItemsControl.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv;

                                    SelectedItems.Add(targetNode.ParentItemsControl.Items[i]);
                                }
                                else
                                {
                                    selecteditem = this.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv;
                                    SelectedItems.Add(this.Items[i]);
                                }
                                if (selecteditem == null)
                                {
                                    if (currentNode.ParentItemsControl == null && (targetNode.ParentItemsControl as TreeViewItemAdv) != null)
                                    {
                                        try
                                        {
                                            selecteditem = (targetNode.ParentTreeView.LinearItems[this.Items[i]]) as TreeViewItemAdv;
                                        }
                                        catch
                                        { }
                                    }
                                    else
                                        if (this.LinearItems.ContainsKey(this.Items[i]))
                                            selecteditem = (this.LinearItems[this.Items[i]]) as TreeViewItemAdv;
                                }
                                if (selecteditem != null)
                                {
                                    m_selectedContainers.Add(selecteditem);
                                    DraggingParentItmes.Add((selecteditem as TreeViewItemAdv).ParentItemsControl);
                                }
                            }
                        }
                        else
                        {
                            for (int i = m_startIndex; i <= targetIndex; i++)
                            {
                                if ((currentNode == null || currentNode.ParentItemsControl == null) && (targetNode.ParentItemsControl as TreeViewItemAdv) != null)
                                {
                                    selecteditem = targetNode.ParentItemsControl.ItemContainerGenerator.ContainerFromItem(targetNode.ParentItemsControl.Items[i]) as TreeViewItemAdv;
                                    SelectedItems.Add(targetNode.ParentItemsControl.Items[i]);
                                }
                                else if (currentNode.ParentItemsControl != null)
                                {
                                    selecteditem = currentNode.ParentItemsControl.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv;
                                    SelectedItems.Add(currentNode.ParentItemsControl.Items[i]);
                                }
                                else if (selecteditem == null && targetNode.ParentItemsControl != null)
                                {
                                    selecteditem = targetNode.ParentItemsControl.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv;

                                    SelectedItems.Add(targetNode.ParentItemsControl.Items[i]);
                                }
                                else
                                {
                                    selecteditem = this.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv;
                                    SelectedItems.Add(this.Items[i]);
                                }
                                if (selecteditem == null && this.Items.Count > i)
                                {
                                    if (this.LinearItems.ContainsKey(this.Items[i]))
                                        selecteditem = (this.LinearItems[this.Items[i]]) as TreeViewItemAdv;
                                }
                                if (selecteditem != null)
                                {
                                    m_selectedContainers.Add(selecteditem);
                                    DraggingParentItmes.Add((selecteditem as TreeViewItemAdv).ParentItemsControl);
                                }
                            }
                        }
                    }
                }
            }
            do
            {
                TreeViewItemAdv node = GetNextTreeViewItemAdv(ref counter);
                if (IsVirtualizing && VirtualizationMode == Tools.VirtualizationMode.Extended && node != null)
                {
                    if (!findSelectionBorder)
                    {
                        findSelectionBorder = true;
                    }
                    else
                        break;
                }
                else
                {
                    if (node == currentNode || node == targetNode)
                    {
                        if (!findSelectionBorder)
                        {
                            findSelectionBorder = true;
                        }
                        else
                        {
                            findSelectionBorder = false;
                            findedTarget = true;

                            if (select)
                            {
                                if (!IsMultiselection)
                                {
                                    IsMultiselection = true;
                                }
                                AddNodeToSelectedItems(node);
                            }
                            else
                            {
                                RemoveNodeFromSelectedItems(node);
                            }

                            break;
                        }
                    }
                }
                if (findSelectionBorder)
                {
                    if (select)
                    {
                        if (!IsMultiselection)
                        {
                            IsMultiselection = true;
                        }
                        if (VirtualizationMode != VirtualizationMode.Extended || !IsVirtualizing)
                            AddNodeToSelectedItems(node);
                    }
                    else
                    {
                        RemoveNodeFromSelectedItems(node);
                    }
                }
                if (findSelectionBorder && IsVirtualizing && VirtualizationMode == Tools.VirtualizationMode.Extended)
                    findSelectionBorder = false;
                if (IsVirtualizing && VirtualizationMode == Tools.VirtualizationMode.Extended && node != null && targetNode != null && node == targetNode)
                {
                    findSelectionBorder = true;
                }
                if (node != null && node.IsExpanded)
                {
                    for (int i = node.Items.Count - 1; i >= 0; i--)
                    {
                        PushInnerItemToStack(node, i);
                    }
                }
            }
            while (m_itemsStack.Count != 0 || ((itemsCnt > counter + 1) && Items[counter + 1] != null));
        }

        /// <summary>
        /// Updates ShowRootLines property.
        /// </summary>
        private void UpdateShowRootLines()
        {
            InvalidatePanelRender(this);
        }

        /// <summary>
        /// Sets value of the SelectedPath item.
        /// </summary>
        /// <param name="value">The value.</param>
        private void SetSelectedPath(string value)
        {
            base.SetValue(SelectedPathPropertyKey, value);
        }

        /// <summary>
        /// Gets path to item.
        /// </summary>
        /// <param name="item">The item TreeView.</param>
        /// <returns>string  value </returns>
        private string GetPathToItem(TreeViewItemAdv item)
        {
            string path = String.Empty;

            if (item != null)
            {
                TreeViewItemAdv currentItem = item;

                do
                {
                    path = PathSeparator + currentItem.ToString() + path;
                    currentItem = currentItem.ParentTreeViewItem;
                }
                while (currentItem != null);

                if (item.ParentTreeView != null)
                {
                    path = item.ParentTreeView.ToString() + path;
                }
            }

            return path;
        }

        /// <summary>
        /// Gets HeaderRowPresenter.
        /// </summary>
        /// <value>The header row presenter.</value>
        internal TreeViewHeaderRowPresenter HeaderRowPresenter
        {
            get
            {
                if (m_headerRowPresenter == null)
                {
                    m_headerRowPresenter = ScrollHost.Template.FindName(C_headerRowPresenter, this.ScrollHost) as TreeViewHeaderRowPresenter;
                }

                return m_headerRowPresenter;
            }
        }

        /// <summary>
        /// Gets first visible item.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns>Treeviewadv  value </returns>
        private TreeViewItemAdv GetFirstVisibleItem(out double offset)
        {
            offset = 0;
            TreeViewItemAdv item = null;
            int countPoint = Convert.ToInt32(ActualWidth);
            double yOffset = C_hittestOffset;

            if (MultiColumnEnable && HeaderRowPresenter != null)
            {
                yOffset += HeaderRowPresenter.ActualHeight;
                countPoint = GetLenghtColumns();
            }

            for (int i = countPoint - 1; i > 0; i--)
            {
                IInputElement element = this.InputHitTest(new Point(i, yOffset));

                if (element != null)
                {
                    item = TreeViewAdv.GetTreeViewItemFromChildren(element as FrameworkElement);

                    if (item != null)
                    {
                        break;
                    }
                }
            }

            if (item != null)
            {
                Point p = this.ScrollHost.TranslatePoint(new Point(0, 0), item);
                System.Drawing.Graphics graphics = System.Drawing.Graphics.FromHwnd(IntPtr.Zero);
                offset = p.Y * MouseUtils.GetDPIFraction();
            }

            return item;
        }

        /// <summary>
        /// Gets length columns.
        /// </summary>
        /// <returns>int  value  type</returns>
        private int GetLenghtColumns()
        {
            int width = 0;

            if (WrappedColumns != null)
            {
                for (int i = 0; i < WrappedColumns.Count; i++)
                {
                    width += Convert.ToInt32(WrappedColumns[i].ActualWidth);
                }
            }

            return width;
        }

        /// <summary>
        /// Finds item by point.
        /// </summary>
        /// <param name="p">The p TreeView.</param>
        /// <returns>Treeviewadv  value </returns>
        public TreeViewItemAdv FindItemByPoint(Point p)
        {
            TreeViewItemAdv item = null;
           
            if (p.Y > 0 && p.X < ActualWidth * MouseUtils.GetDPIFraction() * scalefractionx)
            {
                double height = 0;
                double offset = 0;
                TreeViewItemAdv currentItem = GetFirstVisibleItem(out offset);

                if (currentItem != null)
                {
                    height = 0;

                    do
                    {
                        height += currentItem.Margin.Top + currentItem.Margin.Bottom;

                        if (currentItem.CompleteHeaderElement != null)
                        {
                            if (currentItem.DesiredSize.Height > currentItem.CompleteHeaderElement.ActualHeight)
                            {
                                if (!currentItem.IsExpanded)
                                {
                                    height += currentItem.DesiredSize.Height;
                                }
                                else
                                {
                                    height += currentItem.CompleteHeaderElement.ActualHeight;
                                }
                            }
                            else
                            {
                                height += currentItem.CompleteHeaderElement.ActualHeight;
                            }
                        }
                        else
                        {
                            if (currentItem.DesiredSize.Height > currentItem.ActualHeight)
                            {
                                if (!currentItem.IsExpanded)
                                {
                                    height += currentItem.DesiredSize.Height;
                                }
                                else
                                {
                                    height += currentItem.ActualHeight;
                                }
                            }
                            else
                            {
                                height += currentItem.ActualHeight;
                            }
                        }

                        if ((height - offset) * scalefractiony * MouseUtils.GetDPIFraction() >= p.Y && (height - currentItem.ActualHeight - offset) * scalefractiony * MouseUtils.GetDPIFraction() < p.Y)
                        {
                            m_pointovercontroldiff = height * MouseUtils.GetDPIFraction() - p.Y;
                            item = currentItem;
                            break;
                        }

                        currentItem = currentItem.GetNextVisibleItem(true);
                    }
                    while (currentItem != null);
                }
            }
          
            return item;
        }

        /// <summary>
        /// Layouts popup container when drag-and-drop.
        /// </summary>
        private void LayoutPopupContainer()
        {
            Rect placementRectangle;

            if (m_dragPopup.IsOpen)
            {
                Point mousePosinion;

                if (BrowserInteropHelper.IsBrowserHosted)
                {
                    mousePosinion = MouseUtils.GetMousePosition(this);
                }
                else
                {
                    mousePosinion = MouseUtils.GetMousePosition(null);
                }

                placementRectangle = m_dragPopup.PlacementRectangle;
                placementRectangle.X = mousePosinion.X - m_dragOffset.X;
                placementRectangle.Y = mousePosinion.Y - m_dragOffset.Y;
                if (this.m_loaded)
                    m_dragPopup.PlacementRectangle = placementRectangle;
            }
            else
            {
                ItemsControl container = GetDraggingContainer();

                if (container != null && m_dragPopup != null)
                {
                    m_dragPopup.Child = container;
                    m_dragPopup.LayoutTransform = this.LayoutTransform;
                    m_dragPopup.PlacementTarget = this;

                    double x = m_dragStart.X - m_dragOffset.X;
                    double y = m_dragStart.Y - m_dragOffset.Y;
                    placementRectangle = new Rect(x, y, container.Width, container.Height);
                    m_dragPopup.PlacementRectangle = placementRectangle;
                   
                    if (MultiColumnEnable && (dragvisualborder.Width > 0 || DragAdornerWidth > 0))
                    {
                        m_dragPopup.HorizontalOffset = m_dragOffset.X * scalefractionx;
                        m_dragPopup.VerticalOffset = m_dragOffset.Y * scalefractiony;
                    }
                    m_dragPopup.IsOpen = true;
                }
            }
        }

        /// <summary>
        /// Gets value indicating whether possible drop.
        /// </summary>
        /// <param name="targetItem">The target item.</param>
        /// <returns>bool  value </returns>
        private static bool GetIsPossipleDrop(ItemsControl targetItem)
        {
            bool bIsPossible = false;

            if (targetItem != null)
            {
                if (TreeViewAdv.DragOverControl != null
                    && TreeViewAdv.DragOverTreeView != null)
                {
                    if (TreeViewAdv.DragOverTreeView.DraggingItmesContiners.Contains(targetItem as TreeViewItemAdv))
                    {
                        bIsPossible = ((TreeViewItemAdv)targetItem).IsNeedShowDragMarker;
                        if (bIsPossible == false && (((TreeViewItemAdv)startitem)!=null && ((TreeViewItemAdv)startitem).IsDraging) && startitem != (targetItem as TreeViewItemAdv))
                        {
                            bIsPossible = true;
                        }
                    }
                    else
                    {
                        bIsPossible = true;
                        TreeViewItemAdv parent = targetItem as TreeViewItemAdv;

                        if (parent != null && parent.ParentTreeViewItem != null)
                        {
                            do
                            {
                                if (!bIsPossible)
                                {
                                    break;
                                }

                                parent = parent.ParentTreeViewItem;
                                TreeViewItemAdvCollection items = TreeViewAdv.DragOverTreeView.DraggingItmesContiners;

                                for (int i = 0; i < items.Count; i++)
                                {
                                    if (parent == items[i] || (targetItem as TreeViewItemAdv).Header.Equals(items[i].Header))
                                    {
                                        bIsPossible = false;
                                        break;
                                    }
                                }
                            }
                            while (parent != null);
                        }
                    }
                }
            }
            return bIsPossible;
        }

        /// <summary>
        /// Unselect all items.
        /// </summary>
        /// <param name="tree">The tree TreeView.</param>
        private static void UnselectAllItems(TreeViewAdv tree)
        {
            if (tree != null)
            {
                if (tree.m_selectedContainers != null && tree.m_selectedContainers.Count > 0)
                {
                    for (int j = tree.m_selectedContainers.Count - 1; j >= 0; j--)
                    {
                        TreeViewItemAdv item = tree.m_selectedContainers[j];

                        if (item != null)
                        {
                            tree.IsSelectionChangeActive = true;
                            item.IsSelected = false;
                            tree.IsSelectionChangeActive = false;
                        }
                    }
                }

                tree.IsSelectionChangeActive = true;
                tree.SingleSelection(null, null, false);
                tree.IsSelectionChangeActive = false;
            }
        }

        /// <summary>
        /// Update selection after drop.
        /// </summary>
        /// <param name="targetItemsControl">The target items control.</param>
        /// <param name="data">The data TreeView.</param>
        /// <param name="bIsInternal">if set to <c>true</c> [b is internal].</param>
        private void UpdateDragDropSelection(ItemsControl targetItemsControl, TreeObjectCollection data, bool bIsInternal)
        {
            if (dropSelectContainer != null)
            {
                if (ItemsSource != null)
                {
                    if (dropSelectContainer.DataContext != null && !this.Items.Contains(dropSelectContainer.DataContext))
                    {
                        dropSelectContainer.Header = dropSelectContainer.m_treeviewitemactualobject;
                    }
                    SelectedItems.Clear();
                    m_selectedContainers.Clear();
                    SelectedItems.Add(dropSelectContainer.Header);
                    m_selectedContainers.Add(dropSelectContainer);
                }
                else
                {
                    SelectedItems.Clear();
                    m_selectedContainers.Clear();
                    SelectedItems.Add(dropSelectContainer);
                    m_selectedContainers.Add(dropSelectContainer);
                }
            }
            
        }

        /// <summary>
        /// Update coerce for IsSelected property of the selected items.
        /// </summary>
        private void UpdateIsSelectedSelectedItems()
        {
            if (m_selectedContainers.Count > 0)
            {
                for (int i = 0; i < m_selectedContainers.Count; i++)
                {
                    m_selectedContainers[i].m_bUpdatingCoerce = true;
                    m_selectedContainers[i].CoerceValue(TreeViewItemAdv.IsSelectedProperty);
                    m_selectedContainers[i].m_bUpdatingCoerce = false;
                }
            }
            else if (SelectedContainer != null)
            {
                SelectedContainer.m_bUpdatingCoerce = true;
                SelectedContainer.CoerceValue(TreeViewItemAdv.IsSelectedProperty);
                SelectedContainer.m_bUpdatingCoerce = false;
            }
        }

        /// <summary>
        /// Called when Columns of the control is changed.
        /// </summary>
        /// <param name="d">The d DependencyObject.</param>
        /// <param name="e">The instance containing the event data.</param>
        private static void OnColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TreeViewAdv)d).OnColumnsChanged();
        }

        /// <summary>
        /// Called when Columns of the control is changed.
        /// </summary>
        private void OnColumnsChanged()
        {
            if (Columns != null)
            {
                Columns.CollectionChanged += new NotifyCollectionChangedEventHandler(ColumnCollectionChanged);
            }
            SetWrappedColumns();
        }

        /// <summary>
        /// Called when ColumnCollection of the control is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void ColumnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SetWrappedColumns();
        }

        /// <summary>
        /// Sets wrapped columns.
        /// </summary>
        private void SetWrappedColumns()
        {
            if (MultiColumnEnable && Columns != null && Template != null)
            {
                TreeViewColumnCollection columns = TreeViewColumn.GetWrapperColumnCollection(Columns);
                base.SetValue(WrappedColumnsPropertyKey, columns);
            }
        }

        /// <summary>
        /// Initialize columns by default.
        /// </summary>
        private void InitializeDefaultColumn()
        {
            if (MultiColumnEnable && (Columns == null || Columns.Count == 0))
            {
                TreeViewColumn column = new TreeViewColumn();
                column.Width = new GridLength(100);
                column.Header = "Header";
                Binding binding = new Binding("Header");
                binding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(TreeViewItemAdv), 1);
                column.DisplayMemberBinding = binding;

                if (Columns == null)
                {
                    Columns = new TreeViewColumnCollection();
                }

                Columns.Add(column);
            }
        }

        /// <summary>
        /// Updates visual style for TreeViewAdv.
        /// </summary>
        /// <param name="style">The style.</param>
        private void UpdateVisualStyle(TreeViewAdvVisualStyle style)
        {
            if (!m_bVisualStyleChanging)
            {
                m_bVisualStyleChanging = true;

                switch (style)
                {
                    case TreeViewAdvVisualStyle.Office2007Blue:
                        {
                            SkinStorage.SetVisualStyle(this, C_nameOffice2007BlueVisualStyle);
                            break;
                        }

                    case TreeViewAdvVisualStyle.Office2007Silver:
                        {
                            SkinStorage.SetVisualStyle(this, C_nameOffice2007SilverVisualStyle);
                            break;
                        }

                    case TreeViewAdvVisualStyle.Office2007Black:
                        {
                            SkinStorage.SetVisualStyle(this, C_nameOffice2007BlackVisualStyle);
                            break;
                        }

                    case TreeViewAdvVisualStyle.Office2003:
                        {
                            SkinStorage.SetVisualStyle(this, C_nameOffice2003VisualStyle);
                            break;
                        }

                    case TreeViewAdvVisualStyle.Blend:
                        {
                            SkinStorage.SetVisualStyle(this, C_nameBlendVisualStyle);
                            break;
                        }
                    case TreeViewAdvVisualStyle.Office2010Black:
                        {
                            SkinStorage.SetVisualStyle(this, C_nameOffice2010BlackVisualStyle);
                            break;
                        }
                    case TreeViewAdvVisualStyle.Office2010Silver:
                        {
                            SkinStorage.SetVisualStyle(this, C_nameOffice2010SilverVisualStyle);
                            break;
                        }
                    case TreeViewAdvVisualStyle.Office2010Blue:
                        {
                            SkinStorage.SetVisualStyle(this, C_nameOffice2010BlueVisualStyle);
                            break;
                        }
                    case TreeViewAdvVisualStyle.Metro:
                        {
                            SkinStorage.SetVisualStyle(this, C_nameMetroVisualStyle);
                            break;
                        }
                    case TreeViewAdvVisualStyle.Transparent:
                        {
                            SkinStorage.SetVisualStyle(this, C_nameTransparentVisualStyle);
                            break;
                        }

                    case TreeViewAdvVisualStyle.ShinyBlue:
                        {
                            SkinStorage.SetVisualStyle(this, C_nameShinyBlueVisualStyle);
                            break;
                        }
                    case TreeViewAdvVisualStyle.ShinyRed:
                        {
                            SkinStorage.SetVisualStyle(this, C_nameShinyRedVisualStyle);
                            break;
                        }
                    default:
                        {
                            SkinStorage.SetVisualStyle(this, C_nameDefaultVisualStyle);
                            break;
                        }
                }

                UpdateLayout();
                m_bVisualStyleChanging = false;
            }
        }

        /// <summary>
        /// Gets the value indicating whether visual style registered for TreeViewAdv.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <returns>
        /// <c>true</c> if [is registered visual style] [the specified style]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsRegisteredVisualStyle(string style)
        {
            bool registered = false;

            if (TreeViewAdvVisualStyle.Blend.ToString() == style
                || TreeViewAdvVisualStyle.Default.ToString() == style
                || TreeViewAdvVisualStyle.Office2003.ToString() == style
                || TreeViewAdvVisualStyle.Office2007Black.ToString() == style
                || TreeViewAdvVisualStyle.Office2007Blue.ToString() == style
                || TreeViewAdvVisualStyle.Office2007Silver.ToString() == style
                || TreeViewAdvVisualStyle.Office2010Silver.ToString() == style
                || TreeViewAdvVisualStyle.Office2010Blue.ToString() == style
                || TreeViewAdvVisualStyle.Office2010Black.ToString() == style
                || TreeViewAdvVisualStyle.Metro.ToString() == style
                || TreeViewAdvVisualStyle.Transparent.ToString() == style
                || TreeViewAdvVisualStyle.ShinyBlue.ToString() == style
                || TreeViewAdvVisualStyle.ShinyRed.ToString() == style
                || "Luna.NormalColor" == style
                || "Luna.Metallic" == style
                || "Luna.Homestead" == style
                || "Royale.NormalColor" == style
                || "Zune.NormalColor" == style
                || "Aero.NormalColor" == style)
            {
                registered = true;
            }

            return registered;
        }

        /// <summary>
        /// Gets the value converted to TreeViewAdvVisualStyle.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <returns>Treeviewadv  value </returns>
        private TreeViewAdvVisualStyle GetConvertedVisualStyle(string style)
        {
            TreeViewAdvVisualStyle converted = TreeViewAdvVisualStyle.Default;

            if (style == TreeViewAdvVisualStyle.Blend.ToString())
            {
                converted = TreeViewAdvVisualStyle.Blend;
            }
            else if (style == TreeViewAdvVisualStyle.Office2003.ToString())
            {
                converted = TreeViewAdvVisualStyle.Office2003;
            }
            else if (style == TreeViewAdvVisualStyle.Office2007Black.ToString())
            {
                converted = TreeViewAdvVisualStyle.Office2007Black;
            }
            else if (style == TreeViewAdvVisualStyle.Office2007Blue.ToString())
            {
                converted = TreeViewAdvVisualStyle.Office2007Blue;
            }
            else if (style == TreeViewAdvVisualStyle.Office2007Silver.ToString())
            {
                converted = TreeViewAdvVisualStyle.Office2007Silver;
            }
            else if (style == TreeViewAdvVisualStyle.Office2010Blue.ToString())
            {
                converted = TreeViewAdvVisualStyle.Office2010Blue;
            }
            else if (style == TreeViewAdvVisualStyle.Office2010Black.ToString())
            {
                converted = TreeViewAdvVisualStyle.Office2010Black;
            }
            else if (style == TreeViewAdvVisualStyle.Office2010Silver.ToString())
            {
                converted = TreeViewAdvVisualStyle.Office2010Silver;
            }
            else if (style == TreeViewAdvVisualStyle.Metro.ToString())
            {
                converted = TreeViewAdvVisualStyle.Metro;
            }
            else if (style == TreeViewAdvVisualStyle.Transparent.ToString())
            {
                converted = TreeViewAdvVisualStyle.Transparent;
            }
            else if (style == TreeViewAdvVisualStyle.ShinyBlue.ToString())
            {
                converted = TreeViewAdvVisualStyle.ShinyBlue;
            }
            else if (style == TreeViewAdvVisualStyle.ShinyRed.ToString())
            {
                converted = TreeViewAdvVisualStyle.ShinyRed;
            }

            return converted;
        }

        #endregion Implementation

        #region LoadOnDemand

        /// <summary>
        /// Event gets fired when expanding the TreeViewItemAdv in OnDemandLoading.
        /// </summary>
        public event LoadOnDemandEventHandler LoadOnDemand;

        /// <summary>
        /// Expanding the treeview item
        /// </summary>
        internal void ExpandingTreeViewItem(object sender)
        {
            TreeViewItemAdv treeItem = sender as TreeViewItemAdv;
            if (treeItem != null && treeItem.IsLoadOnDemand)
            {
                treeItem.IsLoading = true;
                if (LoadOnDemand != null)
                {
                    LoadOnDemand(this, new LoadonDemandEventArgs() { TreeViewItem = treeItem });
                }
            }
        }

        #endregion LoadOnDemand

        #region Support IItemsPanelRef

        /// <summary>
        /// Gets the template that defines the panel that controls the layout of items. This is a dependency property.
        /// </summary>
        /// <value></value>
        /// <returns>An <see cref="T:System.Windows.Controls.ItemsPanelTemplate"/> that defines the panel to use for the layout of the items. The default value for the <see cref="T:System.Windows.Controls.ItemsControl"/> is an <see cref="T:System.Windows.Controls.ItemsPanelTemplate"/> that specifies a <see cref="T:System.Windows.Controls.StackPanel"/>.</returns>
        TreeViewAdvItemsPanel IItemsPanelRef.ItemsPanel
        {
            get
            {
                return TreeViewAdv.GetItemsPanel(this, GetItemsPanelType()) as TreeViewAdvItemsPanel;
            }
        }

        #endregion Support IItemsPanelRef

        #region Support IItemContainer

        /// <summary>
        /// Gets the item by index.
        /// </summary>
        /// <param name="index">Index of the item.</param>
        /// <returns>Tree View Item Adv</returns>
        TreeViewItemAdv IItemContainer.GetItem(int index)
        {
            TreeViewItemAdv item = null;

            if (index > -1 && Items.Count > 0)
            {
                item = Items[index] as TreeViewItemAdv;

                if (item == null)
                {
                    item = ItemContainerGenerator.ContainerFromIndex(index) as TreeViewItemAdv;
                }
            }

            return item;
        }

        /// <summary>
        /// Gets the index by item.
        /// </summary>
        /// <param name="obj">Index of the item.</param>
        /// <returns>int type for index</returns>
        int IItemContainer.GetIndex(object obj)
        {
            int index = -1;

            if (obj != null)
            {
                if (obj is TreeViewItemAdv)
                {
                    index = ItemContainerGenerator.IndexFromContainer(obj as TreeViewItemAdv);
                }
                else
                {
                    TreeViewItemAdv item = ItemContainerGenerator.ContainerFromItem(obj) as TreeViewItemAdv;

                    if (item != null)
                    {
                        index = ItemContainerGenerator.IndexFromContainer(item);
                    }
                }
            }

            return index;
        }

        /// <summary>
        /// Gets first visible item.
        /// </summary>
        /// <returns>Treeviewadv  value </returns>
        TreeViewItemAdv IItemContainer.GetFirstVisibleItem()
        {
            TreeViewItemAdv itemFound = null;

            for (int i = 0; i < Items.Count; i++)
            {
                TreeViewItemAdv item = Items[i] as TreeViewItemAdv;

                if (item == null)
                {
                    item = ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv;
                }

                if (item != null && item.Visibility == Visibility.Visible)
                {
                    itemFound = item;
                    break;
                }
            }

            return itemFound;
        }

        /// <summary>
        /// Gets last visible item.
        /// </summary>
        /// <returns>Treeviewadv  value </returns>
        TreeViewItemAdv IItemContainer.GetLastVisibleItem()
        {
            TreeViewItemAdv itemFound = null;

            for (int i = Items.Count - 1; i >= 0; i--)
            {
                TreeViewItemAdv item = Items[i] as TreeViewItemAdv;

                if (item == null)
                {
                    item = ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv;
                }

                if (item != null && item.Visibility == Visibility.Visible)
                {
                    itemFound = item;
                    break;
                }
            }

            return itemFound;
        }

        #endregion Support IItemContainer
    }

    /// <summary>
    /// TreeModel class is used maintain the ivirtualtree items
    /// </summary>
    public class TreeModel
    {
        public IVirtualTree iTree;
        public List<IVirtualTree> treeItems = new List<IVirtualTree>();
    }
}