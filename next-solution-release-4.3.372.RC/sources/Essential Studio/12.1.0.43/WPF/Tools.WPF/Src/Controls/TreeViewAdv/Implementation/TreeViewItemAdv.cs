// <copyright file="TreeViewItemAdv.cs" company="Syncfusion">
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
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Xml;
using Syncfusion.Licensing;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls.Resources;

#endregion file using

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Implements a selectable item in a <see cref="TreeViewAdv"/> control.
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
    /// <example><code>public class TreeViewItemAdv : <see cref="HeaderedItemsControl"/></code></example>
    /// </list>
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <description>XAML Object Element Usage</description>
    /// </listheader>
    /// <example>
    /// <code language="XAML">
    /// <![CDATA[
    /// <local:TreeViewItemAdv>
    /// <see cref="ItemsControl.Items" >
    /// <local:TreeViewItemAdv/>
    /// ]]>
    /// </code>
    /// </example>
    /// </list>
    /// </example>
    /// </list>
    /// <remarks>
    /// Content Model: The TreeViewItemAdv control is a <see cref="HeaderedItemsControl"/>
    /// and has three content properties: <see cref="System.Windows.Controls.HeaderedItemsControl.HeaderProperty"/>, <see cref="System.Windows.Controls.ItemsControl.Items"/>,
    /// and <see cref="System.Windows.Controls.ItemsControl.ItemsSourceProperty"/>.
    /// <para/> TreeViewItemAdv controls can be embedded inside other TreeViewItemAdv controls
    /// to create a hierarchy of nodes inside a TreeViewAdv control.
    /// </remarks>
    /// <example>
    /// <para/>The following example shows how to create a hierarchy of TreeViewItemAdv controls
    /// in a TreeViewAdv control in XAML.
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
    /// <seealso cref="TreeViewAdv"/>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [TemplatePart(Name = "PART_Header", Type = typeof(FrameworkElement)), StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(TreeViewItemAdv))]
    [Serializable]
    public class TreeViewItemAdv : HeaderedItemsControl, ICloneable, IItemsPanelRef, IItemContainer
    {
        #region Constants

        Brush ForegroundBrush = (Brush)new BrushConverter().ConvertFromString("#FF393939");
        Brush ForegroundBrush1 = (Brush)new BrushConverter().ConvertFromString("#FFDCDCDC");
        
        /// <summary>
        /// Name of the header from template.
        /// </summary>
        private const string C_nameHeaderPart = "PART_Header";

        /// <summary>
        /// Name of the complete header from template.
        /// </summary>
        private const string C_nameCompleteHeaderPart = "PART_CompleteHeader";

        /// <summary>
        /// Message for header if doesn't find.
        /// </summary>
        private const string C_errorHeader = "Header not found.";

        /// <summary>
        /// Name of the edit header from template.
        /// </summary>
        private const string C_nameEditHeaderPart = "PART_EditHeader";

        /// <summary>
        /// Message for edit header if doesn't find.
        /// </summary>
        private const string C_errorEditHeader = "Edit header not found.";

        /// <summary>
        /// Name of the expander from template.
        /// </summary>
        private const string C_nameExpander = "PART_Expander";

        /// <summary>
        /// Message for expander if doesn't find.
        /// </summary>
        private const string C_errorExpander = "Expander not found.";

        /// <summary>
        /// Name of the vertical line from template.
        /// </summary>
        private const string C_nameVerticalLine = "PART_VerticalLine";

        /// <summary>
        /// Message for vertical line if doesn't find.
        /// </summary>
        private const string C_errorVerticalLine = "VerticalLine not found.";

        /// <summary>
        /// Name of the items host from template.
        /// </summary>
        private const string C_nameItemsHost = "PART_ItemsHost";

        /// <summary>
        /// Message for items host if doesn't find.
        /// </summary>
        private const string C_errorItemsHost = "ItemsHost not found.";

        /// <summary>
        /// Name of the ItemContent from template.
        /// </summary>
        private const string C_nameItemContent = "PART_ItemContent";

        /// <summary>
        /// Message for ItemContent if doesn't find.
        /// </summary>
        private const string C_errorItemContent = "ItemContent fake area not found.";

        /// <summary>
        /// Name of the panel for images from template.
        /// </summary>
        private const string C_nameImagePanel = "PART_ImagePanel";

        /// <summary>
        /// Name of the HorizontalLine from template.
        /// </summary>
        private const string C_nameHorizontalLine = "PART_HorizontalLine";

        /// <summary>
        /// Name of the TopDragLine from template.
        /// </summary>
        private const string C_nameTopDragBorder = "Part_TopDragLine";

        /// <summary>
        /// Name of the BottomDragLine from template.
        /// </summary>
        private const string C_nameBottomDragBorder = "Part_BottomDragLine";

        /// <summary>
        /// Margin for fake area.
        /// </summary>
        private const int C_marginFakeArea = 10;

        /// <summary>
        /// Offset for show tooltip.
        /// </summary>
        private const int C_offsetToolTip = 2;

        /// <summary>
        /// Offset for drag marker with percent.
        /// </summary>
        private const double C_dragMarkerOffset = 20d;

        /// <summary>
        /// Delay for turn into edit mode.
        /// </summary>
        private const double C_editDelay = 500;

        /// <summary>
        /// Delay for drag-and-drop.
        /// </summary>
        private const double C_dragDelay = 500;

        /// <summary>
        /// Default height for item.
        /// </summary>
        private const double C_defaultItemHeight = 13;

        /// <summary>
        /// Default items offset. Represents X-Axis offset of the items host.
        /// </summary>
        private const double C_defaultItemsOffset = 19;

        /// <summary>
        /// stores the treeviewitems
        /// </summary>
        internal static List<TreeViewItemAdv> m_treeviewadv = new List<TreeViewItemAdv>();

        /// <summary>
        /// stores the itemsource object
        /// </summary>
        internal static List<Object> m_itemObject = new List<object>();

        /// <summary>
        /// indicates whether item is expanded change occured internally or not.
        /// </summary>
        private static bool expandchangeinternally = false;

        private bool isExpandedAlready = false;

        /// <summary>
        /// indicates TreeViewItemAdv actual object bounded
        /// </summary>
        internal object m_treeviewitemactualobject = null;

        internal TreeViewAdv m_treeViewAdv = null;

        internal int ExpandedCount = 0;

        #endregion Constants

        #region Members

        /// <summary>
        /// stores the sortdirection new value
        /// </summary>
        internal SortDirection newvalue = SortDirection.None;

        /// <summary>
        /// stores the sortdirection old value
        /// </summary>
        internal SortDirection oldvalue = SortDirection.None;

        /// <summary>
        /// Value indicates whether item is selected.
        /// </summary>
        private bool m_bContainsSelection = false;

        /// <summary>
        /// Value indicates the index specific to TreeViewItemAdv which is used to calculate auto width for columns.
        /// </summary>
        internal int internalindex = 0;

        /// <summary>
        /// stores the first TreeViewItemAdvHeight
        /// </summary>
        internal double m_treeviewitemadvHeight = 20.0;

        /// <summary>
        /// stores the first TreeViewItemAdv offset Height
        /// </summary>
        internal double m_treeviewitemadvoffsetHeight = 0.0;

        /// <summary>
        /// stores the panel from TreeViewItemAdv
        /// </summary>
        internal VirtualizingPanel m_virtualizingpanel = null;

        internal FakeItemsPanel m_fakeItemsPanel = null;

        internal TreeViewAdvVirtualizingPanel m_treeviewadvVirtualizingPanel = null;

        /// <summary>
        /// Value indicates the width specific to TreeViewItemAdv which is used to calculate auto width for columns.
        /// </summary>
        internal double internalwidth = 0;

        /// <summary>
        /// Value indicates the collection for the expanded items.
        /// </summary>
        internal ObservableCollection<TreeViewItemAdv> ExpandedItems = new ObservableCollection<TreeViewItemAdv>();

        internal ObservableCollection<int> ExpandedItemsIndexCollection = new ObservableCollection<int>();

        internal Dictionary<int, TreeViewItemAdv> ExpandedTreeViewAdvItems = new Dictionary<int, TreeViewItemAdv>();

        internal double availableTopOffset = 0.0;

        private double DesiredItemHeight = 0.0;

        internal double topOffset = 0.0;
        internal double tempHeight = 0.0;
        private bool isRightClick = false;
        internal string tempText = "";
        private bool isTextEnter = false;

        /// <summary>
        /// Catches a copy of the header string before edit.
        /// </summary>
        internal string headerBeforeEdit = string.Empty;

        /// <summary>
        /// Header of the node.
        /// </summary>
        private FrameworkElement m_headerElement = null;

        /// <summary>
        /// Header of the node.
        /// </summary>
        private FrameworkElement m_completeHeaderElement = null;

        /// <summary>
        /// Edit header of the node.
        /// </summary>
        private FrameworkElement m_editHeaderElement = null;

        /// <summary>
        /// Expander for node.
        /// </summary>
        internal Expander m_expander = null;

        /// <summary>
        /// Panel with images.
        /// </summary>
        private FrameworkElement m_imagePanel = null;

        /// <summary>
        /// Items host for item.
        /// </summary>
        private ItemsPresenter m_itemsHost = null;

        /// <summary>
        /// VerticalLine for item.
        /// </summary>
        private TreeRootLine m_verticalLinePartOne = null;

        /// <summary>
        /// VerticalLine for item.
        /// </summary>
        private TreeRootLine m_verticalLinePartTwo = null;

        /// <summary>
        /// HorizontalLine for item.
        /// </summary>
        private TreeRootLine m_horiLine = null;

        /// <summary>
        /// DragLine for item.
        /// </summary>
        internal Border m_topdragLine = null;

        /// <summary>
        /// DragLine for item.
        /// </summary>
        internal Border m_bottomdragLine = null;

        /// <summary>
        /// Item Index
        /// </summary>
        internal int m_ItemIndex = 0;

        /// <summary>
        /// Temporary container for visible items of the TreeViewAdv.
        /// </summary>
        private Stack<TreeViewItemAdv> m_itemsStack = new Stack<TreeViewItemAdv>();

        /// <summary>
        /// Value indicates whether coerce will update without animation.
        /// </summary>
        private bool m_bUpdateCoerce = false;

        /// <summary>
        /// Value indicates whether animation is expanded or collapsed.
        /// </summary>
        private bool m_bAnimatinExpand = true;

        /// <summary>
        /// Animation applied to children items when control is expanded or collapsed.
        /// </summary>
        private DoubleAnimation m_expandAnimation = null;

        /// <summary>
        /// Animation applied to children items when control is expanded or collapsed.
        /// </summary>
        private DoubleAnimation m_fadeAnimation = null;

        /// <summary>
        /// Indicates that collapsed or expanded animation playing.
        /// </summary>
        internal bool m_bAnimating = false;

        /// <summary>
        /// Adorner shown marker when current item is dragged.
        /// </summary>
        private TreeViewItemAdvDragMarkerAdorner m_dragMarkerAdorner = null;

        /// <summary>
        /// Value indicates whether one may turn into edit mode by timer.
        /// </summary>
        private bool m_bIsEditByTimer = false;

        /// <summary>
        /// Value indicates whether one may drag by timer.
        /// </summary>
        private bool m_bIsDragByTimer = false;

        /// <summary>
        /// Point of the item for turn into edit mode by timer.
        /// </summary>
        private Point m_editByTimerPoint;

        /// <summary>
        /// Point of the item for drag by timer.
        /// </summary>
        private Point m_dragByTimerPoint;

        /// <summary>
        /// Timer for turn edit mode.
        /// </summary>
        private DispatcherTimer m_editTimer = new DispatcherTimer();

        /// <summary>
        /// Timer for drag-and-drop.
        /// </summary>
        private DispatcherTimer m_dragTimer = new DispatcherTimer();

        /// <summary>
        /// Cache item size.
        /// </summary>
        private Size m_cacheSize = new Size(0, 0);

        /// <summary>
        /// A value indicates whether the item is last item in collection.
        /// </summary>
        private bool m_bIsLastItem = false;

        /// <summary>
        /// A value indicates whether the item is first item in collection.
        /// </summary>
        private bool m_bIsFirstItem = false;

        /// <summary>
        /// Value indicates whether item is dragging item.
        /// </summary>
        private bool m_bIsFakeItem = false;

        /// <summary>
        /// Value indicates whether the item just switch to edit mode.
        /// </summary>
        private bool m_justStartEdit = false;

        /// <summary>
        /// Old value of the editing item.
        /// </summary>
        private TextBlock m_oldEditValue = null;

        /// <summary>
        /// Value indicates whether need edit item by mouse click.
        /// </summary>
        private bool m_needEditByClick = false;

        /// <summary>
        /// Indicates whether mouse double click on the item
        /// </summary>
        private bool isDoubleClick = false;

        /// <summary>
        /// Save copy for editing item.
        /// </summary>
        //private object m_saveCopyEditingItem = null;

        /// <summary>
        /// Value indicates whether need refresh items after changed theme.
        /// </summary>
        private bool m_needUpdateAfterChangeTheme = false;

        /// <summary>
        /// Value indicates whether updating coerce.
        /// </summary>
        internal bool m_bUpdatingCoerce = false;

        #endregion Members

        #region Events

        /// <summary>
        /// Identifies the <see cref="Collapsed"/> routed_event. This field is read-only.
        /// </summary>
        public static readonly RoutedEvent CollapsedEvent = EventManager.RegisterRoutedEvent(
            "Collapsed",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(TreeViewItemAdv));

        /// <summary>
        /// Occurs when the <see cref="IsExpanded"/> property changes from true to false.
        /// </summary>
        /// <value>
        /// Type: <see cref="RoutedEventHandler"/>
        /// This event occurs when a <see cref="TreeViewItemAdv"/> is collapsed
        /// so that child elements are hidden.
        /// </value>
        /// <example>
        /// <para/>The following example shows how to assign an event handler for
        /// the Collapsed event to a TreeViewItemAdv, and how to define the event handler in code.
        /// <code language="XAML">
        /// <![CDATA[
        /// <TreeViewItemAdv Header="Employee1"
        ///                  Name="Employee1Data"
        ///                  IsExpanded="True"
        ///                  Collapsed="OnCollapsed"
        ///                  Expanded="OnExpanded">
        ///     <TreeViewItemAdv Header="Work Days"
        ///                      Name="EmployeeWorkDays"
        ///                      IsSelected="True">
        ///         <TreeViewItemAdv Header="Tuesday" />
        ///         <TreeViewItemAdv Header="Friday"/>
        ///     </TreeViewItemAdv>
        /// </TreeViewItemAdv>
        /// <TreeViewItemAdv Header="Employee2"
        ///                  Name="Employee2Data">
        ///     <TreeViewItemAdv Header="Work Days"
        ///                      Name="emp2WorkDays"
        ///                      Selected="GetSchedule"
        ///                      Unselected="SetSchedule">
        ///         <TreeViewItemAdv Header="Monday" />
        ///         <TreeViewItemAdv Header="Wednesday"/>
        ///     </TreeViewItemAdv>
        /// </TreeViewItemAdv>
        /// ]]>
        /// </code>
        /// <code language="C#">
        /// public void OnCollapsed(object sender, RoutedEventArgs e)
        /// {
        ///     //Perform actions when the TreeViewItem is collapsed
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="RoutedEventHandler"/>
        public event RoutedEventHandler Collapsed
        {
            add
            {
                AddHandler(TreeViewItemAdv.CollapsedEvent, value);
            }

            remove
            {
                RemoveHandler(TreeViewItemAdv.CollapsedEvent, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="Expanded"/> routed_event.
        /// </summary>
        public static readonly RoutedEvent ExpandedEvent = EventManager.RegisterRoutedEvent(
            "Expanded",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(TreeViewItemAdv));

        /// <summary>
        /// Occurs when the <see cref="IsExpanded"/> property changes from true to false.
        /// </summary>
        /// <value>
        /// Type: <see cref="RoutedEventHandler"/>
        /// This event occurs when a <see cref="TreeViewItemAdv"/> is collapsed
        /// so that child elements are hidden.
        /// </value>
        /// <example>
        /// <para/>The following example shows how to assign an event handler
        /// for the Expanded event to a TreeViewItemAdv and how
        /// to define the event handler in the code-behind.
        /// <code language="XAML">
        /// <![CDATA[
        /// <TreeViewItemAdv Header="Employee1"
        ///                  Name="Employee1Data"
        ///                  IsExpanded="True"
        ///                  Collapsed="OnCollapsed"
        ///                  Expanded="OnExpanded">
        ///     <TreeViewItemAdv Header="Work Days"
        ///                      Name="EmployeeWorkDays"
        ///                      IsSelected="True">
        ///         <TreeViewItemAdv Header="Tuesday" />
        ///         <TreeViewItemAdv Header="Friday"/>
        ///     </TreeViewItemAdv>
        /// </TreeViewItemAdv>
        /// <TreeViewItemAdv Header="Employee2"
        ///                  Name="Employee2Data">
        ///     <TreeViewItemAdv Header="Work Days"
        ///                      Name="emp2WorkDays"
        ///                      Selected="GetSchedule"
        ///                      Unselected="SetSchedule">
        ///         <TreeViewItemAdv Header="Monday" />
        ///         <TreeViewItemAdv Header="Wednesday"/>
        ///     </TreeViewItemAdv>
        /// </TreeViewItemAdv>
        /// ]]>
        /// </code>
        /// <code language="C#">
        /// public void OnExpanded(object sender, RoutedEventArgs e)
        /// {
        ///     //Perform actions when the TreeViewItem is collapsed
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="RoutedEventHandler"/>
        public event RoutedEventHandler Expanded
        {
            add
            {
                AddHandler(TreeViewItemAdv.ExpandedEvent, value);
            }

            remove
            {
                RemoveHandler(TreeViewItemAdv.ExpandedEvent, value);
            }
        }

        /// <summary>
        /// Identified by the <see cref="Selected"/> routed_event. This field is read-only.
        /// </summary>
        public static readonly RoutedEvent SelectedEvent = EventManager.RegisterRoutedEvent(
            "Selected",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(TreeViewItemAdv));

        /// <summary>
        /// Occurs when the <see cref="IsExpanded"/> property changes from true to false.
        /// </summary>
        /// <value>
        /// Type: <see cref="RoutedEventHandler"/>
        /// The event occurs when a TreeViewItemAdv becomes the selected
        /// TreeViewItemAdv in a <see cref="TreeView"/> control.
        /// This event is related to the <see cref="System.Windows.Controls.TreeView.SelectedItemChangedEvent"/> event
        /// that occurs when there is a change in the <see cref="System.Windows.Controls.TreeView.SelectedItemProperty"/> property
        /// of a <see cref="TreeViewAdv"/> control.
        /// </value>
        /// <example>
        /// <para/>The following example shows how to assign an event handler
        /// for the Selected event to a TreeViewItemAdv,
        /// and how to define the event handler in the code-behind.
        /// <code language="XAML">
        /// <![CDATA[
        /// <TreeViewItem Header="Employee2"
        ///              Name="Employee2Data">
        ///     <TreeViewItem Header="Work Days"
        ///                   Name="emp2WorkDays"
        ///                   Selected="GetSchedule"
        ///                   Unselected="SetSchedule">
        ///         <TreeViewItem Header="Monday" />
        ///         <TreeViewItem Header="Wednesday"/>
        ///     </TreeViewItem>
        /// </TreeViewItem>
        /// ]]>
        /// </code>
        /// <code language="C#">
        /// public void GetSchedule(object sender, RoutedEventArgs e)
        /// {
        ///     //Perform actions when a TreeViewItem
        ///     //controls is selected
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="RoutedEventHandler"/>
        public event RoutedEventHandler Selected
        {
            add
            {
                AddHandler(TreeViewItemAdv.SelectedEvent, value);
            }

            remove
            {
                RemoveHandler(TreeViewItemAdv.SelectedEvent, value);
            }
        }

        /// <summary>
        /// Identified by the <see cref="Unselected"/> routed_event.
        /// </summary>
        public static readonly RoutedEvent UnselectedEvent = EventManager.RegisterRoutedEvent(
            "Unselected",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(TreeViewItemAdv));

        /// <summary>
        /// Occurs when the <see cref="IsExpanded"/> property changes from true to false.
        /// </summary>
        /// <value>
        /// Type: <see cref="RoutedEventHandler"/>
        /// The event occurs when the selection changes from this TreeViewItemAdv
        /// to another TreeViewItemAdv in a <see cref="TreeViewAdv"/> control.
        /// This event is related to the <see cref="System.Windows.Controls.TreeView.SelectedItemChangedEvent"/> event
        /// that occurs when there is a change in the <see cref="System.Windows.Controls.TreeView.SelectedItemProperty"/> property
        /// of a <see cref="TreeViewAdv"/> control.
        /// </value>
        /// <example>
        /// <para/>The following example shows how to assign an event handler
        /// for the <see cref="Selected"/> event to a TreeViewItemAdv,
        /// and how to define the event handler in the code-behind.
        /// <code language="XAML">
        /// <![CDATA[
        /// <TreeViewItem Header="Employee2"
        ///              Name="Employee2Data">
        ///     <TreeViewItem Header="Work Days"
        ///                   Name="emp2WorkDays"
        ///                   Selected="GetSchedule"
        ///                   Unselected="SetSchedule">
        ///         <TreeViewItem Header="Monday" />
        ///         <TreeViewItem Header="Wednesday"/>
        ///     </TreeViewItem>
        /// </TreeViewItem>
        /// ]]>
        /// </code>
        /// <code language="C#">
        /// public void SetSchedule(object sender, RoutedEventArgs e)
        /// {
        ///     //Perform actions when a TreeViewItem
        ///     //control becomes unselected
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="RoutedEventHandler"/>
        public event RoutedEventHandler Unselected
        {
            add
            {
                AddHandler(TreeViewItemAdv.UnselectedEvent, value);
            }

            remove
            {
                RemoveHandler(TreeViewItemAdv.UnselectedEvent, value);
            }
        }

        /// <summary>
        /// Identified by the <see cref="BeforeItemEdit"/> routed_event.
        /// </summary>
        public static readonly RoutedEvent BeforeItemEditEvent = EventManager.RegisterRoutedEvent(
            "BeforeItemEdit",
            RoutingStrategy.Bubble,
            typeof(EditModeChangeHandler),
            typeof(TreeViewItemAdv));

        /// <summary>
        /// Occurs when the <see cref="IsInEditMode"/> property changes.
        /// </summary>
        /// <value>
        /// Type: <see cref="EditModeChangeHandler"/>
        /// This event raise before the <see cref="TreeViewItemAdv"/> enters the edit mode.
        /// </value>
        /// <example>
        /// <para/>The following example shows how to assign an event handler
        /// for the <see cref="BeforeItemEdit"/> event to a <see cref="TreeViewItemAdv"/>,
        /// and how to define the event handler in the code-behind.
        /// <code language="XAML">
        /// <![CDATA[
        /// <TreeViewItem Header="Employee2"
        ///              Name="Employee2Data">
        ///     <TreeViewItem Header="Work Days"
        ///                   Name="emp2WorkDays"
        ///                   AfterItemEdit="TreeViewItemAdv_AfterItemEdit"
        ///                   BeforeItemEdit="TreeViewItemAdv_BeforeItemEdit">
        ///         <TreeViewItem Header="Monday" />
        ///         <TreeViewItem Header="Wednesday"/>
        ///     </TreeViewItem>
        /// </TreeViewItem>
        /// ]]>
        /// </code>
        /// <code language="C#">
        /// private void TreeViewItemAdv_AfterItemEdit( object sender, EditModeChangeEventArgs e )
        /// {
        ///     Debug.WriteLine( "AfterItemEdit: old( " + e.OldValue + "), new( " + e.NewValue + ")"  );
        /// }
        /// private void TreeViewItemAdv_BeforeItemEdit( object sender, EditModeChangeEventArgs e )
        /// {
        ///     Debug.WriteLine( "BeforeItemEdit: old( " + e.OldValue + "), new( " + e.NewValue + ")" );
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="EditModeChangeHandler"/>
        public event EditModeChangeHandler BeforeItemEdit
        {
            add
            {
                AddHandler(TreeViewItemAdv.BeforeItemEditEvent, value);
            }

            remove
            {
                RemoveHandler(TreeViewItemAdv.BeforeItemEditEvent, value);
            }
        }

        /// <summary>
        /// Identified by the <see cref="AfterItemEdit"/> routed_event.
        /// </summary>
        public static readonly RoutedEvent AfterItemEditEvent = EventManager.RegisterRoutedEvent(
            "AfterItemEdit",
            RoutingStrategy.Bubble,
            typeof(EditModeChangeHandler),
            typeof(TreeViewItemAdv));

        /// <summary>
        /// Occurs when the <see cref="IsInEditMode"/> property changes.
        /// </summary>
        /// <value>
        /// Type: <see cref="EditModeChangeHandler"/>
        /// This event raise after the edit operations are completed.
        /// </value>
        /// <example>
        /// <para/>The following example shows how to assign an event handler
        /// for the <see cref="AfterItemEdit"/> event to a <see cref="TreeViewItemAdv"/>,
        /// and how to define the event handler in the code-behind.
        /// <code language="XAML">
        /// <![CDATA[
        /// <TreeViewItem Header="Employee2"
        ///              Name="Employee2Data">
        ///     <TreeViewItem Header="Work Days"
        ///                   Name="emp2WorkDays"
        ///                   AfterItemEdit="TreeViewItemAdv_AfterItemEdit"
        ///                   BeforeItemEdit="TreeViewItemAdv_BeforeItemEdit">
        ///         <TreeViewItem Header="Monday" />
        ///         <TreeViewItem Header="Wednesday"/>
        ///     </TreeViewItem>
        /// </TreeViewItem>
        /// ]]>
        /// </code>
        /// <code language="C#">
        /// private void TreeViewItemAdv_AfterItemEdit( object sender, EditModeChangeEventArgs e )
        /// {
        ///     Debug.WriteLine( "AfterItemEdit: old( " + e.OldValue + "), new( " + e.NewValue + ")"  );
        /// }
        /// private void TreeViewItemAdv_BeforeItemEdit( object sender, EditModeChangeEventArgs e )
        /// {
        ///     Debug.WriteLine( "BeforeItemEdit: old( " + e.OldValue + "), new( " + e.NewValue + ")" );
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="EditModeChangeHandler"/>
        public event EditModeChangeHandler AfterItemEdit
        {
            add
            {
                AddHandler(TreeViewItemAdv.AfterItemEditEvent, value);
            }

            remove
            {
                RemoveHandler(TreeViewItemAdv.AfterItemEditEvent, value);
            }
        }

        /// <summary>
        /// Identified by the <see cref="EditKeyUp"/> routed_event.
        /// </summary>
        public static readonly RoutedEvent EditKeyUpEvent = EventManager.RegisterRoutedEvent(
            "EditKeyUp",
            RoutingStrategy.Bubble,
            typeof(KeyEventHandler),
            typeof(TreeViewItemAdv));

        /// <summary>
        /// Occurs when a key is released when the item is in edit mode.
        /// </summary>
        /// <value>
        /// Type: <see cref="KeyEventHandler"/>
        /// This event raise when the item is in edit mode.
        /// </value>
        /// <example>
        /// <para/>The following example shows how to assign an event handler
        /// for the <see cref="EditKeyUp"/> event to a TreeViewItemAdv,
        /// and how to define the event handler in the code-behind.
        /// <code language="XAML">
        /// <![CDATA[
        /// <TreeViewItem Header="Employee2"
        ///              Name="Employee2Data">
        ///     <TreeViewItem Header="Work Days"
        ///                   Name="emp2WorkDays"
        ///                   EditKeyDown="TreeViewItemAdv_EditKeyDown"
        ///                   EditKeyUp="TreeViewItemAdv_EditKeyUp">
        ///         <TreeViewItem Header="Monday" />
        ///         <TreeViewItem Header="Wednesday"/>
        ///     </TreeViewItem>
        /// </TreeViewItem>
        /// ]]>
        /// </code>
        /// <code language="C#">
        /// private void TreeViewItemAdv_EditKeyDown( object sender, KeyEventArgs e )
        /// {
        ///     Debug.WriteLine( "Down: " + e.Key );
        /// }
        /// private void TreeViewItemAdv_EditKeyUp( object sender, KeyEventArgs e )
        /// {
        ///     Debug.WriteLine( "Up: " + e.Key );
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="KeyEventHandler"/>
        public event KeyEventHandler EditKeyUp
        {
            add
            {
                AddHandler(TreeViewItemAdv.EditKeyUpEvent, value);
            }

            remove
            {
                RemoveHandler(TreeViewItemAdv.EditKeyUpEvent, value);
            }
        }

        /// <summary>
        /// Identified by the <see cref="EditKeyDown"/> routed_event.
        /// </summary>
        public static readonly RoutedEvent EditKeyDownEvent = EventManager.RegisterRoutedEvent(
            "EditKeyDown",
            RoutingStrategy.Bubble,
            typeof(KeyEventHandler),
            typeof(TreeViewItemAdv));

        /// <summary>
        /// Occurs when a key is pressed when the item is in edit mode.
        /// </summary>
        /// <value>
        /// Type: <see cref="KeyEventHandler"/>
        /// This event raise when the item is in edit mode.
        /// </value>
        /// <example>
        /// <para/>The following example shows how to assign an event handler
        /// for the <see cref="EditKeyDown"/> event to a TreeViewItemAdv,
        /// and how to define the event handler in the code-behind.
        /// <code language="XAML">
        /// <![CDATA[
        /// <TreeViewItem Header="Employee2"
        ///              Name="Employee2Data">
        ///     <TreeViewItem Header="Work Days"
        ///                   Name="emp2WorkDays"
        ///                   EditKeyDown="TreeViewItemAdv_EditKeyDown"
        ///                   EditKeyUp="TreeViewItemAdv_EditKeyUp">
        ///         <TreeViewItem Header="Monday" />
        ///         <TreeViewItem Header="Wednesday"/>
        ///     </TreeViewItem>
        /// </TreeViewItem>
        /// ]]>
        /// </code>
        /// <code language="C#">
        /// private void TreeViewItemAdv_EditKeyDown( object sender, KeyEventArgs e )
        /// {
        ///     Debug.WriteLine( "Down: " + e.Key );
        /// }
        /// private void TreeViewItemAdv_EditKeyUp( object sender, KeyEventArgs e )
        /// {
        ///     Debug.WriteLine( "Up: " + e.Key );
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="KeyEventHandler"/>
        public event KeyEventHandler EditKeyDown
        {
            add
            {
                AddHandler(TreeViewItemAdv.EditKeyDownEvent, value);
            }

            remove
            {
                RemoveHandler(TreeViewItemAdv.EditKeyDownEvent, value);
            }
        }

        #endregion Events

        #region Commands

        /// <summary>
        /// Command responsible for editing TreeViewItemAdv in TreeViewAdv.
        /// </summary>
        public static RoutedCommand Edit = new RoutedCommand("Edit", typeof(TreeViewAdv), new InputGestureCollection(new KeyGesture[1] { new KeyGesture(Key.F2) }));

        #endregion Commands

        #region Dependency property

        /// <summary>
        /// Identifies TreeViewItemAdv. IsExpanded dependency property.
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(TreeViewItemAdv), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(TreeViewItemAdv.OnIsExpandedChanged), new CoerceValueCallback(CoerceOnIsExpanded)));

        /// <summary>
        /// Identifies TreeViewItemAdv. IsExpandedInternal dependency property.
        /// </summary>
        private static readonly DependencyProperty IsExpandedInternalProperty =
            DependencyProperty.Register("IsExpandedInternal", typeof(bool), typeof(TreeViewItemAdv), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(TreeViewItemAdv.OnIsExpandedChangedInternal), new CoerceValueCallback(CoerceOnIsExpandedInternal)));

        /// <summary>
        /// Identifies TreeViewItemAdv. Sorting dependency property.
        /// </summary>
        public static readonly DependencyProperty SortingProperty =
            DependencyProperty.RegisterAttached("Sorting", typeof(SortDirection), typeof(TreeViewItemAdv), new FrameworkPropertyMetadata(SortDirection.None, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnSortingChanged)));

        /// <summary>
        /// Identifies TreeViewItemAdv. IsSelected dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(TreeViewItemAdv), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(TreeViewItemAdv.OnIsSelectedChanged), new CoerceValueCallback(CoerceOnIsSelected)));

        /// <summary>
        /// Identifies TreeViewItemAdv. IsSelected dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedFalseProperty =
            DependencyProperty.Register("IsSelectedFalse", typeof(bool), typeof(TreeViewItemAdv), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Identifies TreeViewItemAdv. IsSelectionActive dependency property.
        /// </summary>
        public static readonly DependencyPropertyKey IsSelectionActivePropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("IsSelectionActive", typeof(bool), typeof(TreeViewItemAdv), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Identifies TreeViewItemAdv.IsSelectionActiveProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectionActiveProperty = IsSelectionActivePropertyKey.DependencyProperty;

        /// <summary>
        /// Identifies TreeViewItemAdv. LeftImageSource dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftImageSourceProperty =
            DependencyProperty.Register("LeftImageSource", typeof(ImageSource), typeof(TreeViewItemAdv), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Identifies TreeViewItemAdv. RightImageSource dependency property.
        /// </summary>
        public static readonly DependencyProperty RightImageSourceProperty =
            DependencyProperty.Register("RightImageSource", typeof(ImageSource), typeof(TreeViewItemAdv), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Identifies TreeViewItemAdv. ExpandedImageSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpandedImageSourceProperty =
            DependencyProperty.Register("ExpandedImageSource", typeof(ImageSource), typeof(TreeViewItemAdv), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Identifies TreeViewItemAdv. CollapsedImageSource dependency property.
        /// </summary>
        public static readonly DependencyProperty CollapsedImageSourceProperty =
            DependencyProperty.Register("CollapsedImageSource", typeof(ImageSource), typeof(TreeViewItemAdv), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Identifies TreeViewItemAdv. ImageWidth dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(double), typeof(TreeViewItemAdv), new UIPropertyMetadata(double.NaN));

        /// <summary>
        /// Identifies TreeViewItemAdv. ImageHeight dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(double), typeof(TreeViewItemAdv), new UIPropertyMetadata(double.NaN));

        /// <summary>
        /// Identifies TreeViewItemAdv. IsReadOnly dependency property.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(TreeViewItemAdv), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies TreeViewItemAdv. ImageStretch dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageStretchProperty =
            DependencyProperty.Register("ImageStretch", typeof(Stretch), typeof(TreeViewItemAdv), new UIPropertyMetadata(Stretch.Uniform));

        /// <summary>
        /// Identifies TreeViewItemAdv. IsShowToolTip dependency property.
        /// </summary>
        public static readonly DependencyProperty IsShowToolTipProperty =
            DependencyProperty.Register("IsShowToolTip", typeof(bool), typeof(TreeViewItemAdv), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies TreeViewItemAdv. IsCustomToolTipEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCustomToolTipEnabledProperty =
            DependencyProperty.Register("IsCustomToolTipEnabled", typeof(bool), typeof(TreeViewItemAdv), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies TreeViewItemAdv. IsEditable dependency property.
        /// </summary>
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof(bool), typeof(TreeViewItemAdv), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Identifies TreeViewItemAdv. IsEdited dependency property.
        /// </summary>
        public static readonly DependencyProperty IsInEditModeProperty =
            DependencyProperty.Register("IsInEditMode", typeof(bool), typeof(TreeViewItemAdv), new FrameworkPropertyMetadata(false, OnIsInEditModeChanged, new CoerceValueCallback(CoerceOnIsInEditMode)));

        /// <summary>
        /// Identifies TreeViewItemAdv. ExpandAnimation dependency property.
        /// </summary>
        public static DependencyProperty ExpandAnimationProperty =
            DependencyProperty.Register("ExpandAnimation", typeof(DoubleAnimation), typeof(TreeViewItemAdv), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies TreeViewItemAdv. FadeAnimation dependency property.
        /// </summary>
        public static DependencyProperty FadeAnimationProperty =
            DependencyProperty.Register("FadeAnimation", typeof(DoubleAnimation), typeof(TreeViewItemAdv), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies TreeViewAdv. IsDragOver dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDragOverProperty =
            DependencyProperty.Register("IsDragOver", typeof(bool), typeof(TreeViewItemAdv), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies TreeViewAdv. IsDragOver dependency property.
        /// </summary>
        public static readonly DependencyPropertyKey HasFakeItemsPropertyKey =
            DependencyProperty.RegisterReadOnly("HasFakeItems", typeof(bool), typeof(TreeViewItemAdv), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Identifies TreeViewAdv.HasFakeItemsProperty dependency property.
        /// </summary>
        public static readonly DependencyProperty HasFakeItemsProperty = HasFakeItemsPropertyKey.DependencyProperty;

        /// <summary>
        /// Identifies TreeViewItemAdv. HeaderTextDecorations dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderTextDecorationsProperty =
            DependencyProperty.Register("HeaderTextDecorations", typeof(TextDecorationCollection), typeof(TreeViewItemAdv), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies TreeViewItemAdv. IsDraging dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDragingProperty =
            DependencyProperty.Register("IsDraging", typeof(bool), typeof(TreeViewItemAdv), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies TreeViewItemAdv. MultiColumnEnable dependency property.
        /// </summary>
        public static readonly DependencyProperty MutiColumnEnableProperty =
            DependencyProperty.Register("MultiColumnEnable", typeof(bool), typeof(TreeViewItemAdv), new FrameworkPropertyMetadata(false));

        #endregion Dependency property

        #region LoadOnDemand

        /// <summary>
        /// header for loading content
        /// </summary>
        public object LoadingHeader
        {
            get { return (object)GetValue(LoadingHeaderProperty); }
            set { SetValue(LoadingHeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LoadingHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoadingHeaderProperty =
            DependencyProperty.Register("LoadingHeader", typeof(object), typeof(TreeViewItemAdv), new PropertyMetadata(new ResourceWrapper().LoadOnDemandHeader));

        /// <summary>
        /// header template for loading content
        /// </summary>
        public DataTemplate LoadingHeaderTemplate
        {
            get { return (DataTemplate)GetValue(LoadingHeaderTemplateProperty); }
            set { SetValue(LoadingHeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LoadingHeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoadingHeaderTemplateProperty =
            DependencyProperty.Register("LoadingHeaderTemplate", typeof(DataTemplate), typeof(TreeViewItemAdv), new PropertyMetadata(null));

        /// <summary>
        /// Get or Set the item is loading
        /// </summary>
        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            internal set { SetValue(IsLoadingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLoading.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(TreeViewItemAdv), new PropertyMetadata(false));

        /// <summary>
        /// Get or Set Load on Demand
        /// </summary>
        public bool IsLoadOnDemand
        {
            get { return (bool)GetValue(IsLoadOnDemandProperty); }
            set { SetValue(IsLoadOnDemandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsLoadOnDemand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsLoadOnDemandProperty =
            DependencyProperty.Register("IsLoadOnDemand", typeof(bool), typeof(TreeViewItemAdv), new PropertyMetadata(false, new PropertyChangedCallback(OnIsLoadOnDemandChanged)));

        /// <summary>
        /// IsLoadOnDemand changed callback event will triggered
        /// when the IsLoadOnDemand property changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OnIsLoadOnDemandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            TreeViewItemAdv treeItem = sender as TreeViewItemAdv;
            if (treeItem != null)
            {
                if (!(bool)args.NewValue)
                {
                    treeItem.IsLoading = false;
                }
            }
        }

        #endregion LoadOnDemand

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether will can edit header of the item.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// true if the header of the item can be edit; otherwise, false. The default value is true.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set IsEditable property in C#.
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
        ///             myTreeViewItem.IsEditable = false;
        ///         }
        ///     }
        /// }
        /// </code>
        /// <para/>This example shows how to set IsEditable property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview">
        ///     <local:TreeViewItemAdv Header="Employee1" IsEditable="false">
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
        /// <seealso cref="TreeViewItemAdv"/>
        /// <seealso cref="bool"/>
        public bool IsEditable
        {
            get
            {
                return (bool)base.GetValue(IsEditableProperty);
            }

            set
            {
                base.SetValue(IsEditableProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the sorting.
        /// </summary>
        /// <value>The sorting.</value>
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
        /// Gets or sets a value indicating whether item is in edit mode.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// true if the item is in edit mode edit; otherwise, false. The default value is false.
        /// </value>
        /// <example>
        /// <para/>The following example shows how to get the value of the IsInEditMode property in C#.
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
        ///             bool isInEditMode = myTreeViewItem.IsInEditMode;
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="TreeViewItemAdv"/>
        /// <seealso cref="bool"/>
        public bool IsInEditMode
        {
            get
            {
                return (bool)base.GetValue(IsInEditModeProperty);
            }

            set
            {
                base.SetValue(IsInEditModeProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether show root line for items.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// true if root line is visible; otherwise, false.
        /// The default value is true.
        /// </value>
        /// <remarks>
        /// Specifies whether lines should be shown near the top-level nodes.
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
        ///             myTreeViewItem.ShowRootLines = false;
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="TreeViewItemAdv"/>
        /// <seealso cref="bool"/>
        public bool ShowRootLines
        {
            get
            {
                return (bool)TreeViewAdv.GetShowRootLines(this);
            }

            private set
            {
                TreeViewAdv.SetShowRootLines(this, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether show tooltip.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// true if need shows the tooltip; otherwise, false.
        /// The default value is true.
        /// </value>
        /// <remarks>
        /// In case when there is not enough space to display the entire item,
        /// it�s content displays help text in a tooltip above the item.
        /// </remarks>
        /// <example>
        /// <para/>The following example shows how to get the value of the IsShowToolTip property in C#.
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
        ///             bool show = myTreeViewItem.IsShowToolTip;
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="TreeViewItemAdv"/>
        /// <seealso cref="bool"/>
        public bool IsShowToolTip
        {
            get
            {
                return (bool)GetValue(IsShowToolTipProperty);
            }

            private set
            {
                SetValue(IsShowToolTipProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is custom tool tip enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is custom tool tip enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsCustomToolTipEnabled
        {
            get
            {
                return (bool)GetValue(IsCustomToolTipEnabledProperty);
            }

            set
            {
                SetValue(IsCustomToolTipEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets ImageSource to left image.
        /// </summary>
        /// <value>
        /// Type: <see cref="ImageSource"/>
        /// The source of the image. The default value is null.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set LeftImageSource property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview">
        ///     <local:TreeViewItemAdv Header="Employee1" LeftImageSource="images/image1.png">
        ///         <local:TreeViewItemAdv Header="Jesper" LeftImageSource="images/image2.png"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="TreeViewItemAdv"/>
        /// <seealso cref="ImageSource"/>
        public ImageSource LeftImageSource
        {
            get
            {
                return (ImageSource)GetValue(LeftImageSourceProperty);
            }

            set
            {
                SetValue(LeftImageSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets ImageSource to right image.
        /// </summary>
        /// <value>
        /// Type: <see cref="ImageSource"/>
        /// The source of the image. The default value is null.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set RightImageSource property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview">
        ///     <local:TreeViewItemAdv Header="Employee1" RightImageSource="images/image1.png">
        ///         <local:TreeViewItemAdv Header="Jesper" RightImageSource="images/image2.png"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="TreeViewItemAdv"/>
        /// <seealso cref="ImageSource"/>
        public ImageSource RightImageSource
        {
            get
            {
                return (ImageSource)GetValue(RightImageSourceProperty);
            }

            set
            {
                SetValue(RightImageSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets ImageSource to expanded image.
        /// </summary>
        /// <value>
        /// Type: <see cref="ImageSource"/>
        /// The source of the image. The default value is null.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set ExpandedImageSource property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview">
        ///     <local:TreeViewItemAdv Header="Employee1" ExpandedImageSource="images/image1.png">
        ///         <local:TreeViewItemAdv Header="Jesper" ExpandedImageSource="images/image2.png"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="TreeViewItemAdv"/>
        /// <seealso cref="ImageSource"/>
        public ImageSource ExpandedImageSource
        {
            get
            {
                return (ImageSource)GetValue(ExpandedImageSourceProperty);
            }

            set
            {
                SetValue(ExpandedImageSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets ImageSource to collapsed image.
        /// </summary>
        /// <value>
        /// Type: <see cref="ImageSource"/>
        /// The source of the image. The default value is null.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set CollapsedImageSource property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview">
        ///     <local:TreeViewItemAdv Header="Employee1" CollapsedImageSource="images/image1.png">
        ///         <local:TreeViewItemAdv Header="Jesper" CollapsedImageSource="images/image2.png"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="TreeViewItemAdv"/>
        /// <seealso cref="ImageSource"/>
        public ImageSource CollapsedImageSource
        {
            get
            {
                return (ImageSource)GetValue(CollapsedImageSourceProperty);
            }

            set
            {
                SetValue(CollapsedImageSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets width to all images.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// The width of the image. The default value is NaN.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set ImageWidth property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview">
        ///     <local:TreeViewItemAdv Header="Employee1" ImageWidth="10">
        ///         <local:TreeViewItemAdv Header="Jesper" ImageWidth="10"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="TreeViewItemAdv"/>
        /// <seealso cref="double"/>
        public double ImageWidth
        {
            get
            {
                return (double)GetValue(ImageWidthProperty);
            }

            set
            {
                SetValue(ImageWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets height to all images.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// The height of the image. The default value is NaN.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set ImageHeight property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview">
        ///     <local:TreeViewItemAdv Header="Employee1" ImageHeight="10">
        ///         <local:TreeViewItemAdv Header="Jesper" ImageHeight="10"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="TreeViewItemAdv"/>
        /// <seealso cref="double"/>
        public double ImageHeight
        {
            get
            {
                return (double)GetValue(ImageHeightProperty);
            }

            set
            {
                SetValue(ImageHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether treeviewitem is read only.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly
        {
            get
            {
                return (bool)GetValue(IsReadOnlyProperty);
            }

            set
            {
                SetValue(IsReadOnlyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets stretch to all images.
        /// </summary>
        /// <value>
        /// Type: <see cref="double"/>
        /// The stretch of the image. The default value is Uniform.
        /// </value>
        /// <example>
        /// <para/>This example shows how to set ImageStretch property in XAML.
        /// <code language="XAML">
        /// <![CDATA[
        /// <Window x:Class="Sample1.Window1"
        /// xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        /// xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        /// xmlns:local="clr-namespace:Syncfusion.Windows.Tools.Controls;assembly=Syncfusion.Tools.WPF"
        /// Title="Window1" Height="300" Width="300">
        /// <StackPanel Name="stackPanel">
        /// <local:TreeViewAdv Name="myTreeview">
        ///     <local:TreeViewItemAdv Header="Employee1" ImageStretch="Fill">
        ///         <local:TreeViewItemAdv Header="Jesper" ImageStretch="Fill"/>
        ///     </local:TreeViewItemAdv>
        /// </local:TreeViewAdv>
        /// </StackPanel>
        /// </Window>
        /// ]]>
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="TreeViewItemAdv"/>
        /// <seealso cref="Stretch"/>
        public Stretch ImageStretch
        {
            get
            {
                return (Stretch)GetValue(ImageStretchProperty);
            }

            set
            {
                SetValue(ImageStretchProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the edited item template selector.
        /// </summary>
        /// <value>The edited item template selector.</value>
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
        /// ClickMode="Press"
        /// IsChecked="{Binding Path=IsExpanded, RelativeSource={RelativeSource TemplatedParent}}">
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
        /// <local:TreeViewAdv Name="myTreeview">
        ///     <local:TreeViewItemAdv Header="Employee1" ExpanderStyle="{DynamicResource MyEStyle}>
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
        /// <seealso cref="TreeViewItemAdv"/>
        /// <seealso cref="Style"/>
        public Style ExpanderStyle
        {
            get
            {
                return (Style)GetValue(TreeViewAdv.ExpanderStyleProperty);
            }

            set
            {
                SetValue(TreeViewAdv.ExpanderStyleProperty, value);
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
        /// <local:TreeViewAdv Name="myTreeview">
        ///     <local:TreeViewItemAdv Header="Employee1" EditedItemTemplate="{StaticResource CustomEditedItemTemplate}">
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
        /// <seealso cref="TreeViewItemAdv"/>
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
        /// Gets or sets a value indicating whether the nested items in a <see cref="TreeViewItemAdv"/> are expanded or collapsed.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// true if the nested items of a TreeViewItemAdv are visible; otherwise, false.
        /// The default value is false.
        /// </value>
        /// <remarks>
        /// The appearance of the button that expands and collapses the TreeViewItemAdv changes
        /// when the TreeViewItemAdv is expanded or collapsed. When the IsExpanded property
        /// value changes from true to false, the <see cref="Collapsed"/> event occurs.
        /// Similarly, the <see cref="Expanded"/> event occurs when the
        /// IsExpanded property value changes from false to true.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set IsExpanded property in C#.
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
        ///             myTreeViewItem.IsExpanded = true;
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="TreeViewItemAdv"/>
        /// <seealso cref="bool"/>
        public bool IsExpanded
        {
            get
            {
                return (bool)base.GetValue(IsExpandedProperty);
            }

            set
            {
                base.SetValue(IsExpandedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a <see cref="TreeViewItemAdv"/> control is selected.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// true if the TreeViewItemAdv is selected; otherwise, false. The default value is false.
        /// </value>
        /// <remarks>
        /// When the IsSelected property value changes, the <see cref="Selected"/> event occurs.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set IsSelected property in C#.
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
        ///             myTreeViewItem.IsSelected = true;
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="TreeViewItemAdv"/>
        /// <seealso cref="bool"/>
        public bool IsSelected
        {
            get
            {
                return (bool)base.GetValue(IsSelectedProperty);
            }

            set
            {
                base.SetValue(IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="TreeViewItemAdv"/> has keyboard focus.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// true if the TreeViewItemAdv has keyboard focus; otherwise, false.
        /// The default value is false.
        /// </value>
        /// <remarks>
        /// If the keyboard changes focus from a TreeViewItemAdv
        /// to a <see cref="Menu"/> or a <see cref="ToolBar"/>, the value of this property remains true.
        /// </remarks>
        /// <example>
        /// <para/>The following example shows how to get the value of the IsSelectionActive property in C#.
        /// <code language="C#">
        /// bool isEmployee1Active = Employee1Data.IsSelectionActive;
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="TreeViewItemAdv"/>
        /// <seealso cref="bool"/>
        public bool IsSelectionActive
        {
            get
            {
                return (bool)base.GetValue(IsSelectionActiveProperty);
            }
        }

        

        /// <summary>
        /// Gets or sets the expand animation.
        /// </summary>
        /// <value>The expand animation.</value>
        public DoubleAnimation ExpandAnimation
        {
            get
            {
                return (DoubleAnimation)GetValue(ExpandAnimationProperty);
            }

            set
            {
                SetValue(ExpandAnimationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the fade animation.
        /// </summary>
        /// <value>The fade animation.</value>
        public DoubleAnimation FadeAnimation
        {
            get
            {
                return (DoubleAnimation)GetValue(FadeAnimationProperty);
            }

            set
            {
                SetValue(FadeAnimationProperty, value);
            }
        }

        /// <summary>
        /// Gets the parent items control.
        /// </summary>
        /// <value>The parent items control.</value>
        public ItemsControl ParentItemsControl
        {
            get
            {
                return ItemsControl.ItemsControlFromItemContainer(this);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the dragging item is located over this element.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// true if the dragging item is located over this element; otherwise, false.
        /// A value indicating whether the dragging item is located over this element.
        /// </value>
        /// <example>
        /// <para/>The following example shows how to access the IsDragOver property in C#.
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
        ///             bool bIsDragOver = myTreeViewItem.IsDragOver;
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="TreeViewItemAdv"/>
        /// <seealso cref="bool"/>
        public bool IsDragOver
        {
            get
            {
                return (bool)GetValue(IsDragOverProperty);
            }

            set
            {
                SetValue(IsDragOverProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected false.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is selected false; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelectedFalse
        {
            get
            {
                return (bool)base.GetValue(IsSelectedFalseProperty);
            }

            set
            {
                base.SetValue(IsSelectedFalseProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header text decorations.
        /// </summary>
        /// <value>The header text decorations.</value>
        public TextDecorationCollection HeaderTextDecorations
        {
            get
            {
                return (TextDecorationCollection)GetValue(HeaderTextDecorationsProperty);
            }

            set
            {
                SetValue(HeaderTextDecorationsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a <see cref="TreeViewItemAdv"/> control is dragging
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// true if the TreeViewItemAdv is dragging otherwise, false. The default value is false.
        /// </value>
        /// <remarks>
        /// When the IsDraging property value changes, the <see cref="Selected"/> event occurs.
        /// </remarks>
        /// <example>
        /// <para/>This example shows how to set IsDraging property in C#.
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
        ///             myTreeViewItem.IsDraging = true;
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="TreeViewAdv"/>
        /// <seealso cref="TreeViewItemAdv"/>
        /// <seealso cref="bool"/>
        public bool IsDraging
        {
            get
            {
                return (bool)GetValue(IsDragingProperty);
            }

            set
            {
                SetValue(IsDragingProperty, value);
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
                return (bool)GetValue(MutiColumnEnableProperty);
            }

            set
            {
                SetValue(MutiColumnEnableProperty, value);
            }
        }

        /// <summary>
        /// Gets expander for node.
        /// </summary>
        internal Expander Expander
        {
            get
            {
                return m_expander;
            }
        }

        /// <summary>
        /// Gets or sets the vertical line part one.
        /// </summary>
        /// <value>The vertical line part one.</value>
        internal TreeRootLine VerticalLinePartOne
        {
            get
            {
                return m_verticalLinePartOne;
            }

            set
            {
                m_verticalLinePartOne = value;
            }
        }

        /// <summary>
        /// Gets or sets the vertical line part two.
        /// </summary>
        /// <value>The vertical line part two.</value>
        internal TreeRootLine VerticalLinePartTwo
        {
            get
            {
                return m_verticalLinePartTwo;
            }

            set
            {
                m_verticalLinePartTwo = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is last item.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is last item; otherwise, <c>false</c>.
        /// </value>
        internal bool IsLastItem
        {
            get
            {
                return m_bIsLastItem;
            }

            set
            {
                if (value != m_bIsLastItem)
                {
                    m_bIsLastItem = value;
                    OnIsLastItemChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is first item.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is first item; otherwise, <c>false</c>.
        /// </value>
        internal bool IsFirstItem
        {
            get
            {
                return m_bIsFirstItem;
            }

            set
            {
                if (value != m_bIsFirstItem)
                {
                    m_bIsFirstItem = value;
                }
            }
        }

        /// <summary>
        /// Gets parent as TreeViewAdv.
        /// </summary>
        ///
        internal TreeViewAdv ParentTreeView
        {
            get
            {
                TreeViewAdv treeViewAdv = null;

                for (ItemsControl container = this.ParentItemsControl; container != null; container = ItemsControl.ItemsControlFromItemContainer(container))
                {
                    treeViewAdv = container as TreeViewAdv;

                    if (treeViewAdv != null)
                    {
                        break;
                    }
                }

                return treeViewAdv;
            }
        }

        /// <summary>
        /// Gets parent as TreeViewItemAdv.
        /// </summary>
        internal TreeViewItemAdv ParentTreeViewItem
        {
            get
            {
                return this.ParentItemsControl as TreeViewItemAdv;
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
        /// Gets header of the node.
        /// </summary>
        internal FrameworkElement HeaderElement
        {
            get
            {
                return m_headerElement;
            }
        }

        /// <summary>
        /// Gets complete header of the node.
        /// </summary>
        internal FrameworkElement CompleteHeaderElement
        {
            get
            {
                return m_completeHeaderElement;
            }
        }

        /// <summary>
        /// Gets a value indicating whether need shows drag indicator on top.
        /// </summary>
        internal bool IsShowTopMarker
        {
            get
            {
                bool result = true;
                Point position = MouseUtils.GetMousePosition(CompleteHeaderElement);

                if (CompleteHeaderElement != null && position.Y > CompleteHeaderElement.ActualHeight / 2
                    && (!IsExpanded || TreeViewAdv.DragOverControl == ParentTreeView))
                {
                    result = false;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is fake item.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is fake item; otherwise, <c>false</c>.
        /// </value>
        internal bool IsFakeItem
        {
            get
            {
                return m_bIsFakeItem;
            }

            set
            {
                m_bIsFakeItem = value;
            }
        }

        /// <summary>
        /// Gets drag marker adorner. Adorner is created on first call.
        /// </summary>
        internal TreeViewItemAdvDragMarkerAdorner AdornerMarker
        {
            get
            {
                if (m_dragMarkerAdorner == null)
                {
                    m_dragMarkerAdorner = new TreeViewItemAdvDragMarkerAdorner(this);
                }

                return m_dragMarkerAdorner;
            }
        }

        /// <summary>
        /// Gets a value indicating whether need shows drag indicator.
        /// </summary>
        internal bool IsNeedShowDragMarker
        {
            get
            {
                bool result = true;

                if (ParentItemsControl != null)
                {
                    Point position = MouseUtils.GetMousePosition(this);

                    double dxtop = CompleteHeaderElement.ActualHeight * ParentTreeView.scalefractiony * C_dragMarkerOffset / 100;
                    double dxbottom = CompleteHeaderElement.ActualHeight * ParentTreeView.scalefractiony - dxtop;

                    if ((position.Y > dxtop && position.Y < dxbottom)
                        || (position.Y < CompleteHeaderElement.ActualHeight && IsExpanded))
                    {
                        result = false;
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether item can expand.
        /// </summary>
        private bool CanExpand
        {
            get
            {
                return base.HasItems;
            }
        }

        /// <summary>
        /// Gets a value indicating whether item can expand.
        /// </summary>
        private bool CanExpandOnInput
        {
            get
            {
                bool bCan = false;

                if (this.CanExpand)
                {
                    bCan = base.IsEnabled;
                }

                return bCan;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [contains selection].
        /// </summary>
        /// <value><c>true</c> if [contains selection]; otherwise, <c>false</c>.</value>
        private bool ContainsSelection
        {
            get
            {
                return m_bContainsSelection;
            }

            set
            {
                m_bContainsSelection = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether control key is pressed.
        /// </summary>
        private static bool IsControlKeyDown
        {
            get
            {
                return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
            }
        }

        /// <summary>
        /// Gets header editor of the node.
        /// </summary>
        private FrameworkElement EditHeaderElement
        {
            get
            {
                return m_editHeaderElement;
            }
        }

        #endregion Properties

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="TreeViewItemAdv"/> class.
        /// </summary>
        static TreeViewItemAdv()
        {
            EnvironmentTest.ValidateLicense(typeof(TreeViewItemAdv));
            VisibilityProperty.OverrideMetadata(typeof(TreeViewItemAdv), new PropertyMetadata(OnVisibilityChanged));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeViewItemAdv), new FrameworkPropertyMetadata(typeof(TreeViewItemAdv)));
            EventManager.RegisterClassHandler(typeof(TreeViewItemAdv), FrameworkElement.RequestBringIntoViewEvent, new RequestBringIntoViewEventHandler(TreeViewItemAdv.OnRequestBringIntoView));
        }

        internal bool visiblityFlag = false;

        private static void OnVisibilityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ((TreeViewItemAdv)sender).visiblityFlag = true;
            ((TreeViewItemAdv)sender).InvalidatePanelRender();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewItemAdv"/> class.
        /// </summary>
        public TreeViewItemAdv()
        {
            CommandBinding editCommand = new CommandBinding(Edit, EditExecute, EditCanExecute);
            CommandBindings.Add(editCommand);

            TreeViewAdv parentTV = (TreeViewAdv)this.ParentItemsControl;
         
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewItemAdv"/> class.
        /// </summary>
        /// <param name="header">The header.</param>
        public TreeViewItemAdv(string header)
            : this()
        {
            Header = header;
        }

        #endregion Initialization

        #region Implementation

        /// <summary>
        /// Returns a String that represents the current Object.
        /// </summary>
        /// <returns>A String that represents the current Object.</returns>
        public override string ToString()
        {
            string value = base.ToString();

            if (Header != null && Header is String)
            {
                value = Header as String;
            }
            else if (HeaderElement != null)
            {
                value = HeaderElement.ToString();
            }

            return value;
        }

        /// <summary>
        /// Builds the current template's visual tree if necessary.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Loaded += new RoutedEventHandler(TreeViewItemAdv_Loaded);
            Unloaded += new RoutedEventHandler(TreeViewItemAdv_Unloaded);
            UpdateMeasureData(false);
            m_itemsHost = Template.FindName(C_nameItemsHost, this) as ItemsPresenter;
           
        }

        void TreeViewItemAdv_Unloaded(object sender, RoutedEventArgs e)
        {
            if (m_verticalLinePartOne != null)
                m_verticalLinePartOne = null;
            if (m_verticalLinePartTwo != null)
                m_verticalLinePartTwo = null;
            if (m_horiLine != null)
                m_horiLine = null;

            Loaded -= new RoutedEventHandler(TreeViewItemAdv_Loaded);
            Unloaded -= new RoutedEventHandler(TreeViewItemAdv_Unloaded);
        }

       
        /// <summary>
        /// Updates measure data by DesiredSize or RenderedSize.
        /// If updateByDesiredSize == true - updates based on DesiredSize, else - by RendeSize.
        /// </summary>
        /// <param name="updateByDesiredSize">bool updateByDesiredSize</param>
        private void UpdateMeasureData(bool updateByDesiredSize)
        {
            if (ParentTreeView != null)
            {
                if (CompleteHeaderElement == null)
                {
                    if (Template != null)
                    {
                        m_completeHeaderElement = Template.FindName(C_nameCompleteHeaderPart, this) as FrameworkElement;
                    }
                }

                if (VerticalLinePartOne == null)
                {
                    m_verticalLinePartOne = this.Template.FindName("PART_VerticalLinePartOne", this) as TreeRootLine;
                }

                if (VerticalLinePartTwo == null)
                {
                    m_verticalLinePartTwo = this.Template.FindName("PART_VerticalLinePartTwo", this) as TreeRootLine;
                }
            }
        }

        /// <summary>
        /// Gets the size of the item header.
        /// </summary>
        /// <param name="updateByDesiredSize">if set to <c>true</c> [update by desired size].</param>
        /// <returns>Return Header Size.</returns>
        private Size GetItemHeaderSize(bool updateByDesiredSize)
        {
            Size size = new Size();

            ControlTemplate template = Template;
            FrameworkElement header = CompleteHeaderElement;
            if (header == null && template != null)
            {
                header = template.FindName(C_nameCompleteHeaderPart, this) as FrameworkElement;
            }

            if (header != null)
            {
                size = updateByDesiredSize ? header.DesiredSize : header.RenderSize;
            }

            size.Width += GetItemsPanelOffset(updateByDesiredSize);

            return size;
        }

        /// <summary>
        /// Gets the items panel offset.
        /// </summary>
        /// <param name="updateByDesiredSize">if set to <c>true</c> [update by desired size].</param>
        /// <returns>Return Panel Offset</returns>
        internal double GetItemsPanelOffset(bool updateByDesiredSize)
        {
            double offset = 0.0;

            ControlTemplate template = Template;
            FrameworkElement expander = Expander;
            if (expander == null && template != null)
            {
                expander = template.FindName(C_nameExpander, this) as FrameworkElement;
            }

            if (expander != null)
            {
                offset += updateByDesiredSize ? expander.DesiredSize.Width : expander.RenderSize.Width;
            }

            return offset;
        }

        /// <summary>
        /// Handles the Loaded event of the TreeViewItemAdv control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void TreeViewItemAdv_Loaded(object sender, RoutedEventArgs e)
        {
            TreeViewItemAdv treeViewItem = (sender as TreeViewItemAdv);
            if (this.m_treeviewadvVirtualizingPanel != null && ParentTreeView != null && ParentTreeView.IsVirtualizing && ParentTreeView.VirtualizationMode == VirtualizationMode.Extended)
                this.m_treeviewadvVirtualizingPanel.InvalidateMeasure();
            if (!IsFakeItem & ParentTreeView != null)
            {
               
                Initialize(Template);
                InitializeTextDecorations();

                if (m_needUpdateAfterChangeTheme && IsVisible && ParentTreeView != null)
                {
                    m_needUpdateAfterChangeTheme = false;
                }
            }
            if (ParentTreeView != null && ParentTreeView.IsImageSourceFreeze)
            {
                if (this.LeftImageSource != null) this.LeftImageSource.Freeze();
                if (this.RightImageSource != null) this.RightImageSource.Freeze();
                if (this.CollapsedImageSource != null) this.CollapsedImageSource.Freeze();
                if (this.ExpandedImageSource != null) this.ExpandedImageSource.Freeze();
            }

            if (this.ParentTreeView != null)
            {
                if (m_treeviewitemactualobject != null)
                {
                    if (!this.ParentTreeView.LinearItems.ContainsKey(this.m_treeviewitemactualobject))
                    {
                        this.ParentTreeView.LinearItems.Add(this.m_treeviewitemactualobject, this);
                    }
                }
                ObservableCollection<object> selItems = new ObservableCollection<object>();

                if (ParentTreeView.SelectedItems.Contains(this.DataContext) || ParentTreeView.SelectedItems.Contains(this))
                {
                    this.IsSelected = true;
                }

                ParentTreeView.isLinearItemsChanged = false;
                foreach (Object item in ParentTreeView.m_selectedContainers)
                {
                    selItems.Add(item);
                }
                if (treeViewItem != null)
                {
                    if (ParentTreeView.m_selectedContainers.Contains(treeViewItem))
                    {
                        ParentTreeView.m_selectedContainers.Clear();
                        foreach (TreeViewItemAdv item in selItems)
                        {
                            ParentTreeView.m_selectedContainers.Add(item);
                        }
                    }
                }
            }

            Loaded -= new RoutedEventHandler(TreeViewItemAdv_Loaded);
            MouseDoubleClick += new MouseButtonEventHandler(TreeViewItemAdv_MouseDoubleClick);
        }

        /// <summary>
        /// treeviewitemadv mouse double click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItemAdv_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            isDoubleClick = true;
        }

        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.FrameworkElement"/> has been updated. The specific dependency property that changed is reported in the arguments parameter. Overrides <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)"/>.
        /// </summary>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == SkinStorage.VisualStyleProperty)
            {
                m_needUpdateAfterChangeTheme = true;
            }
            else if (e.Property == VisibilityProperty)
            {
                if (ParentTreeView != null && ParentTreeView.IsVirtualizing)
                {
                    if (e.NewValue.Equals(Visibility.Collapsed) && e.OldValue.Equals(Visibility.Visible))
                    {
                        ParentTreeView.LinearList.Remove(this);
                    }
                    else if (e.NewValue.Equals(Visibility.Visible) && e.OldValue.Equals(Visibility.Collapsed))
                    {
                        if (!ParentTreeView.LinearList.ContainsKey(this))
                        {
                            ParentTreeView.LinearList.Add(this, 0);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when the parent of this element in the visual tree is changed. Overrides <see cref="M:System.Windows.UIElement.OnVisualParentChanged(System.Windows.DependencyObject)"/>.
        /// </summary>
        /// <param name="oldParent">The old parent element. May be null to indicate that the element did not have a visual parent previously.</param>
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            if (oldParent == null && VisualParent != null)
            {
                m_needUpdateAfterChangeTheme = true;
            }
        }

        /// <summary>
        /// Initializes TextDecoration for Header.
        /// </summary>
        private void InitializeTextDecorations()
        {
            if (!MultiColumnEnable)
            {
                if (HeaderElement != null)
                {
                    HeaderElement.ApplyTemplate();
                    List<TextBlock> arr = GetTextBlockForHeader();

                    for (int i = 0; i < arr.Count; i++)
                    {
                        arr[i].SetBinding(TextBlock.TextDecorationsProperty, Binder.Bind(this, "HeaderTextDecorations"));
                    }
                }
            }
        }

        /// <summary>
        /// Gets the text block for header.
        /// </summary>
        /// <returns>List TextBlock </returns>
        private List<TextBlock> GetTextBlockForHeader()
        {
            List<TextBlock> list = new List<TextBlock>();
            int count = VisualTreeHelper.GetChildrenCount(HeaderElement);
            TextBlock textBlock = null;

            for (int i = 0; i < count; i++)
            {
                textBlock = VisualTreeHelper.GetChild(HeaderElement, i) as TextBlock;

                if (textBlock != null)
                {
                    list.Add(textBlock);
                }
            }

            return list;
        }

        /// <summary>
        /// Selects the specified selected.
        /// </summary>
        /// <param name="selected">if set to <c>true</c> [selected].</param>
        internal void Select(bool selected)
        {
            object data = null;
            if (((ParentTreeView != null) && (ParentItemsControl != null))
                && !ParentTreeView.IsSelectionChangeActive)
            {
                data = ParentItemsControl.ItemContainerGenerator.ItemFromContainer(this);

                if (data == null || data == DependencyProperty.UnsetValue)
                {
                    data = this;
                }
                if (data is TreeViewItemAdv && ParentTreeView.SelectedItem != null && (data as TreeViewItemAdv).m_treeviewitemactualobject != null && ParentTreeView.SelectedItem.Equals((data as TreeViewItemAdv).m_treeviewitemactualobject))
                {
                    ParentTreeView.allowSelectedTreeItem = false;
                    ParentTreeView.SelectedTreeItem = (data as TreeViewItemAdv);
                    ParentTreeView.allowSelectedTreeItem = true;
                    ParentTreeView.SelectedTreeItemObject = (data as TreeViewItemAdv).m_treeviewitemactualobject;
                }

                ParentTreeView.ChangeSelection(data, this, selected, false);
            }
            else if (m_treeViewAdv != null && (ParentItemsControl != null))
            {
                data = m_treeViewAdv.ItemContainerGenerator.ItemFromContainer(this);
                if (data == null || data == DependencyProperty.UnsetValue)
                {
                    data = this;
                }

                m_treeViewAdv.ChangeSelection(data, this, selected, false);
            }
        }

        /// <summary>
        /// Focuses down.
        /// </summary>
        /// <returns>bool value type</returns>
        internal bool FocusDown()
        {
            ItemsControl parentItemsControl = this.ParentItemsControl;

            if (parentItemsControl != null)
            {
                TreeViewItemAdv item;
                int index = parentItemsControl.ItemContainerGenerator.IndexFromContainer(this);
                int count = parentItemsControl.Items.Count;

                while (index < count)
                {
                    index++;
                    item = parentItemsControl.ItemContainerGenerator.ContainerFromIndex(index) as TreeViewItemAdv;

                    if (((item != null) && item.IsEnabled) && item.Focus())
                    {
                        return true;
                    }
                }

                item = parentItemsControl as TreeViewItemAdv;

                if (item != null)
                {
                    return item.FocusDown();
                }
            }

            return false;
        }

        /// <summary>
        /// Handles down key.
        /// </summary>
        /// <returns>bool value type</returns>
        internal bool HandleDownKey()
        {
            if (this.AllowHandleKeyEvent(FocusNavigationDirection.Down))
            {
                TreeViewItemAdv item = ItemContainerGenerator.ContainerFromIndex(0) as TreeViewItemAdv;

                if (item != null)
                {
                    if (item.IsEnabled)
                    {
                      return item.Focus();
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Updates the contains selection.
        /// </summary>
        /// <param name="selected">if set to <c>true</c> [selected].</param>
        internal void UpdateContainsSelection(bool selected)
        {
            for (TreeViewItemAdv parentTreeViewItemAdv = this.ParentTreeViewItem; parentTreeViewItemAdv != null; parentTreeViewItemAdv = parentTreeViewItemAdv.ParentTreeViewItem)
            {
                parentTreeViewItemAdv.ContainsSelection = selected;
            }
        }

        /// <summary>
        /// Gets the node level.
        /// </summary>
        /// <returns>bool value type</returns>
        internal int GetNodeLevel()
        {
            int level = 0;
            ItemsControl itemsControl = ParentItemsControl;
            TreeViewItemAdv item = this;

            while (itemsControl != null)
            {
                level++;

                item = item.ParentTreeViewItem;

                if (item != null)
                {
                    itemsControl = item;
                }
                else
                {
                    itemsControl = null;
                }
            }

            return level;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is mouse over complete header.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is mouse over complete header; otherwise, <c>false</c>.
        /// </value>
        internal bool IsMouseOverComplateHeader
        {
            get
            {
                bool result = false;
                Point p = Mouse.GetPosition(CompleteHeaderElement);

                if (p.X > 0 && p.X < CompleteHeaderElement.ActualWidth
                    && p.Y > 0 && p.Y < CompleteHeaderElement.ActualHeight)
                {
                    result = true;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is mouse over expander.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is mouse over expander; otherwise, <c>false</c>.
        /// </value>
        internal bool IsMouseOverExpander
        {
            get
            {
                bool result = false;
                if (Expander != null)
                {
                    Point p = Mouse.GetPosition(Expander);

                    if (p.X > 0 && p.X < Expander.ActualWidth
                        && p.Y > 0 && p.Y < Expander.ActualHeight)
                    {
                        result = true;
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Gets the next visible item.
        /// </summary>
        /// <param name="bInternal">if set to <c>true</c> [b internal].</param>
        /// <returns>TreeView ItemAdv</returns>
        internal TreeViewItemAdv GetNextVisibleItem(bool bInternal)
        {
            TreeViewItemAdv item = null;
            IItemsPanelRef panel = null;

            if (Items.Count > 0 && IsExpanded && bInternal)
            {
                panel = this as IItemsPanelRef;

                if (panel != null && panel.ItemsPanel != null)
                {
                    item = panel.ItemsPanel.GetInternalItem(0) as TreeViewItemAdv;
                    if (item != null && item.Visibility == Visibility.Collapsed)
                    {
                        for (int i = 1; i < panel.ItemsPanel.CountInternalItems - 1; i++)
                        {
                            item = panel.ItemsPanel.GetInternalItem(i) as TreeViewItemAdv;
                            if (item != null && item.Visibility != Visibility.Collapsed)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            else if (ParentItemsControl != null)
            {
                panel = ParentItemsControl as IItemsPanelRef;

                if (panel != null && panel.ItemsPanel != null)
                {
                    int index = panel.ItemsPanel.GetInternalIndex(this);

                    if (index > -1)
                    {
                        if (index < panel.ItemsPanel.CountInternalItems - 1)
                        {
                            item = panel.ItemsPanel.GetInternalItem(index + 1) as TreeViewItemAdv;
                            if (item != null && item.Visibility == Visibility.Collapsed)
                            {
                                item = (item).GetNextVisibleItem(false);
                            }
                        }
                        else if (index == panel.ItemsPanel.CountInternalItems - 1 && ParentItemsControl is TreeViewItemAdv)
                        {
                            item = (ParentItemsControl as TreeViewItemAdv).GetNextVisibleItem(false);
                            if (item != null && item.Visibility == Visibility.Collapsed)
                            {
                                item = (ParentItemsControl as TreeViewItemAdv).GetNextVisibleItem(false);
                            }
                        }
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Gets the previous visible item.
        /// </summary>
        /// <param name="bInternal">if set to <c>true</c> [b internal].</param>
        /// <returns>TreeView ItemAdv</returns>
        internal TreeViewItemAdv GetPreviousVisibleItem(bool bInternal)
        {
            TreeViewItemAdv item = null;
            IItemsPanelRef panel = null;

            if (Items.Count > 0 && IsExpanded && bInternal)
            {
                TreeViewItemAdv currItem = this.GetPreviousVisibleItem(false);

                if (currItem != null)
                {
                    while (currItem.GetNextVisibleItem(true) != this)
                    {
                        currItem = currItem.GetNextVisibleItem(true);
                    }

                    item = currItem;
                }
            }
            else if (ParentItemsControl != null)
            {
                panel = ParentItemsControl as IItemsPanelRef;

                if (panel != null && panel.ItemsPanel != null)
                {
                    int index = panel.ItemsPanel.GetInternalIndex(this);

                    if (index > -1)
                    {
                        if (index > 0)
                        {
                            item = panel.ItemsPanel.GetInternalItem(index - 1) as TreeViewItemAdv;

                            while (item.GetNextVisibleItem(true) != this)
                            {
                                item = item.GetNextVisibleItem(true);
                            }
                        }
                        else if (index == 0 && ParentItemsControl is TreeViewItemAdv)
                        {
                            item = ParentItemsControl as TreeViewItemAdv;
                        }
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Gets the type of the items panel.
        /// </summary>
        /// <returns>Type of tree view items panel</returns>
        protected virtual Type GetItemsPanelType()
        {
            return typeof(TreeViewAdvItemsPanel);
        }

        /// <summary>
        /// Raises the <see cref="E:Collapsed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCollapsed(RoutedEventArgs e)
        {
            if (ParentTreeView != null && ParentTreeView.IsVirtualizing)
            {
                foreach (object obj in Items)
                {
                    TreeViewItemAdv currentItem = obj as TreeViewItemAdv;
                    if (currentItem == null)
                    {
                        currentItem = ItemContainerGenerator.ContainerFromItem(obj) as TreeViewItemAdv;
                    }

                    if (currentItem != null)
                    {
                        ParentTreeView.LinearList.Remove(currentItem);
                    }
                    if (currentItem != null && currentItem.IsExpanded)
                    {
                        AddInCache(currentItem, false);
                    }
                }
            }

            base.RaiseEvent(e);
            this.IsInEditMode = false;
        }

        /// <summary>
        /// Adds the in cache.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="needToAdd">if set to <c>true</c> [need to add].</param>
        protected void AddInCache(TreeViewItemAdv item, bool needToAdd)
        {
            if (item != null)
            {
                for (int i = 0; i < item.Items.Count; i++)
                {
                    TreeViewItemAdv currentItem = item.Items[i] as TreeViewItemAdv;
                    if (currentItem == null)
                    {
                        currentItem = item.ItemContainerGenerator.ContainerFromItem(item.Items[i]) as TreeViewItemAdv;
                    }
                    if (needToAdd)
                    {
                        if (currentItem != null && !ParentTreeView.LinearList.ContainsKey(currentItem))
                        {
                            ParentTreeView.LinearList.Add(currentItem, 0);
                        }
                    }
                    else
                    {
                        if (currentItem != null && ParentTreeView != null)
                        {
                            ParentTreeView.LinearList.Remove(currentItem);
                        }
                    }
                    if (currentItem != null && currentItem.IsExpanded)
                    {
                        if (currentItem.HasItems)
                        {
                            AddInCache(currentItem, needToAdd);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:Expanded"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnExpanded(RoutedEventArgs e)
        {
            if (ParentTreeView != null && ParentTreeView.IsVirtualizing && ParentTreeView.VirtualizationMode == VirtualizationMode.Normal)
            {
                foreach (object obj in Items)
                {
                    TreeViewItemAdv currentItem = obj as TreeViewItemAdv;
                    if (currentItem == null)
                    {
                        currentItem = ItemContainerGenerator.ContainerFromItem(obj) as TreeViewItemAdv;
                    }

                    if (currentItem != null && ParentTreeView != null)
                    {
                        if (!ParentTreeView.LinearList.ContainsKey(currentItem))
                        {
                            ParentTreeView.LinearList.Add(currentItem, 0);
                        }

                        if (currentItem.IsExpanded)
                        {
                            AddInCache(currentItem, true);
                        }
                    }
                }
                InvalidatePanelRender(this);
            }

            base.RaiseEvent(e);
            this.IsInEditMode = false;
        }

        /// <summary>
        /// Invalidates the panel render.
        /// </summary>
        /// <param name="items">The items.</param>
        internal void InvalidatePanelRender(ItemsControl items)
        {
            if (items != null && items.Visibility != Visibility.Collapsed)
            {
                IItemsPanelRef panel = items as IItemsPanelRef;
                TreeViewAdvItemsPanel itemsPanel = (panel != null) ? panel.ItemsPanel : null;

                if (panel != null && itemsPanel != null)
                {
                    if (itemsPanel.Children.Count > 0)
                    {
                        itemsPanel.InvalidateRender();
                    }
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:Selected"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSelected(RoutedEventArgs e)
        {
            base.RaiseEvent(e);
            if (this.Template != null)
            {
                ContentPresenter contentPresenter = this.Template.FindName("PART_Header", this) as ContentPresenter;
                TreeViewRowPresenter treeViewRowPresenter = this.Template.FindName("PART_RowPresenter", this) as TreeViewRowPresenter;
                ControlTemplate template = Template;
                FrameworkElement frameWorkElement;
                FrameworkElement multicolumnElement;
                if (template != null)
                {
                    frameWorkElement = template.FindName("PART_CompleteHeader", this) as StackPanel;
                    multicolumnElement = template.FindName("PART_SelectRectangle", this) as SelectRectangle;
                    if (frameWorkElement is StackPanel)
                    {
                        if ((contentPresenter is ContentPresenter) && (contentPresenter.Content != null) && !(contentPresenter.Content is TreeViewAdv) && contentPresenter.Content.GetType() != typeof(string))
                        {
                            foreach (var item in VisualUtils.EnumChildrenOfType(contentPresenter, typeof(TextBlock)))
                            {
                                if (((VisualUtils.FindAncestor(item, typeof(TreeViewItemAdv)) as TreeViewItemAdv) != null) && (VisualUtils.FindAncestor(item, typeof(TreeViewItemAdv)) as TreeViewItemAdv).Header == (this as TreeViewItemAdv).Header)
                                {
                                    if (ParentTreeView != null && ParentTreeView.ReadLocalValue(TreeViewAdv.UniversalForegroundProperty) != DependencyProperty.UnsetValue)
                                        (item as TextBlock).Foreground = ParentTreeView.UniversalForeground;
                                    else if (SkinStorage.GetVisualStyle(this) == "Blend")
                                        (item as TextBlock).Foreground = ForegroundBrush;
                                    else
                                    {
                                        (item as TextBlock).ClearValue(ForegroundProperty);
                                        SkinStorage.SetVisualStyle(item, SkinStorage.GetVisualStyle(this));
                                    }
                                }
                            }
                        }
                    }
                    if (multicolumnElement is SelectRectangle)
                    {
                        if ((treeViewRowPresenter is TreeViewRowPresenter) && (treeViewRowPresenter.Content != null) && !(treeViewRowPresenter.Content is TreeViewAdv) && treeViewRowPresenter.Content.GetType() != typeof(string))
                        {
                            foreach (var item in VisualUtils.EnumChildrenOfType(treeViewRowPresenter, typeof(TextBlock)))
                            {
                                if (((VisualUtils.FindAncestor(item, typeof(TreeViewItemAdv)) as TreeViewItemAdv) != null) && (VisualUtils.FindAncestor(item, typeof(TreeViewItemAdv)) as TreeViewItemAdv).Header == (this as TreeViewItemAdv).Header)
                                {
                                    if (ParentTreeView != null && ParentTreeView.ReadLocalValue(TreeViewAdv.UniversalForegroundProperty) != DependencyProperty.UnsetValue)
                                        (item as TextBlock).Foreground = ParentTreeView.UniversalForeground;
                                    else if (SkinStorage.GetVisualStyle(this) == "Blend")
                                        (item as TextBlock).Foreground = ForegroundBrush;
                                    else
                                    {
                                        (item as TextBlock).ClearValue(ForegroundProperty);
                                        SkinStorage.SetVisualStyle(item, SkinStorage.GetVisualStyle(this));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:Unselected"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnUnselected(RoutedEventArgs e)
        {
            base.RaiseEvent(e);
            this.IsInEditMode = false;
            ContentPresenter contentPresenter = this.Template.FindName("PART_Header", this) as ContentPresenter;
            TreeViewRowPresenter treeViewRowPresenter = this.Template.FindName("PART_RowPresenter", this) as TreeViewRowPresenter;
            ControlTemplate template = Template;
            FrameworkElement frameWorkElement;
            FrameworkElement multicolumnElement;
            if (template != null)
            {
                frameWorkElement = template.FindName("PART_CompleteHeader", this) as StackPanel;
                multicolumnElement = template.FindName("PART_SelectRectangle", this) as SelectRectangle;
                if (frameWorkElement is StackPanel)
                {
                    if ((contentPresenter is ContentPresenter) && (contentPresenter.Content != null) && !(contentPresenter.Content is TreeViewAdv) && contentPresenter.Content.GetType() != typeof(string))
                    {
                        foreach (var item in VisualUtils.EnumChildrenOfType(contentPresenter, typeof(TextBlock)))
                        {
                            if (((VisualUtils.FindAncestor(item, typeof(TreeViewItemAdv)) as TreeViewItemAdv) != null) && (VisualUtils.FindAncestor(item, typeof(TreeViewItemAdv)) as TreeViewItemAdv).Header == (this as TreeViewItemAdv).Header)
                            {
                                if (ParentTreeView != null && ParentTreeView.ReadLocalValue(TreeViewAdv.UniversalForegroundProperty) != DependencyProperty.UnsetValue)
                                    (item as TextBlock).Foreground = ParentTreeView.UniversalForeground;
                                else if (SkinStorage.GetVisualStyle(this) == "Blend")
                                    (item as TextBlock).Foreground = ForegroundBrush1;
                                else
                                {
                                    (item as TextBlock).ClearValue(ForegroundProperty);
                                    SkinStorage.SetVisualStyle(item, SkinStorage.GetVisualStyle(this));
                                }
                            }
                        }
                    }
                }
                if (multicolumnElement is SelectRectangle)
                {
                    if ((treeViewRowPresenter is TreeViewRowPresenter) && (treeViewRowPresenter.Content != null) && !(treeViewRowPresenter.Content is TreeViewAdv) && treeViewRowPresenter.Content.GetType() != typeof(string))
                    {
                        foreach (var item in VisualUtils.EnumChildrenOfType(treeViewRowPresenter, typeof(TextBlock)))
                        {
                            if (((VisualUtils.FindAncestor(item, typeof(TreeViewItemAdv)) as TreeViewItemAdv) != null) && (VisualUtils.FindAncestor(item, typeof(TreeViewItemAdv)) as TreeViewItemAdv).Header == (this as TreeViewItemAdv).Header)
                            {
                                if (ParentTreeView != null && ParentTreeView.ReadLocalValue(TreeViewAdv.UniversalForegroundProperty) != DependencyProperty.UnsetValue)
                                    (item as TextBlock).Foreground = ParentTreeView.UniversalForeground;
                                else if (SkinStorage.GetVisualStyle(this) == "Blend")
                                    (item as TextBlock).Foreground = ForegroundBrush1;
                                else
                                {
                                    (item as TextBlock).ClearValue(ForegroundProperty);
                                    SkinStorage.SetVisualStyle(item, SkinStorage.GetVisualStyle(this));
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:BeforeItemEdit"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnBeforeItemEdit(RoutedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Raises the <see cref="E:AfterItemEdit"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnAfterItemEdit(RoutedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Raises the <see cref="E:EditKeyUp"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnEditKeyUp(RoutedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Raises the <see cref="E:EditKeyDown"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnEditKeyDown(RoutedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Raises the <see cref="E:EditKeyPress"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnEditKeyPress(RoutedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        /// <summary>
        /// Invoked when an unhandled Mouse. MouseEnter attached event is raised on this element.
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The MouseEventArgs that contains the event data.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (ParentTreeView != null && EnvironmentTest.IsSecurityGranted)
            {
                ScrollViewer parentScroll = ParentTreeView.ScrollHost;

                if (HeaderElement != null && parentScroll != null && IsShowToolTip == false)
                {
                    try
                    {
                        double xElement = HeaderElement.PointToScreen(new Point(HeaderElement.ActualWidth, 0)).X;
                        double xParent = parentScroll.PointToScreen(new Point(parentScroll.ActualWidth, 0)).X;
                        xParent -= parentScroll.ActualWidth - parentScroll.ViewportWidth - C_offsetToolTip;

                        if ((!Double.IsInfinity(xParent) && xParent < xElement && ParentTreeView.FlowDirection == FlowDirection.LeftToRight)
                            || (!Double.IsInfinity(xParent) && xParent > xElement && ParentTreeView.FlowDirection == FlowDirection.RightToLeft))
                        {
                            if (IsCustomToolTipEnabled)
                            {
                                ToolTipService.SetToolTip(this.CompleteHeaderElement, this.ToolTip);
                                ToolTipService.SetIsEnabled(this.CompleteHeaderElement, true);
                                ToolTipService.SetPlacementTarget(this, this.CompleteHeaderElement);
                                ToolTipService.SetPlacement(this.CompleteHeaderElement, PlacementMode.RelativePoint);
                                ToolTipService.SetHorizontalOffset(this.CompleteHeaderElement, 0);
                                ToolTipService.SetVerticalOffset(this.CompleteHeaderElement, 0);
                                ToolTipService.SetPlacementRectangle(this.CompleteHeaderElement, new Rect(0, 0, HeaderElement.ActualWidth, HeaderElement.ActualHeight));
                            }
                            else
                            {
                                if ((this.Header as Visual) != null)
                                {
                                    TextBlock text = VisualUtils.FindDescendant(this.Header as Visual, typeof(TextBlock)) as TextBlock;
                                    if (text != null)
                                    {
                                        ToolTipService.SetToolTip(this.CompleteHeaderElement, text.Text);
                                        ToolTipService.SetIsEnabled(this.CompleteHeaderElement, true);
                                        ToolTipService.SetPlacementTarget(this, this.CompleteHeaderElement);
                                        ToolTipService.SetPlacement(this.CompleteHeaderElement, PlacementMode.RelativePoint);
                                        ToolTipService.SetHorizontalOffset(this.CompleteHeaderElement, 0);
                                        ToolTipService.SetVerticalOffset(this.CompleteHeaderElement, 0);
                                        ToolTipService.SetPlacementRectangle(this.CompleteHeaderElement, new Rect(0, 0, HeaderElement.ActualWidth, HeaderElement.ActualHeight));
                                    }
                                }
                            }
                        }
                        else if (!Double.IsInfinity(xParent))
                        {
                            xElement = HeaderElement.PointToScreen(new Point(0, 0)).X;
                            xParent = parentScroll.PointToScreen(new Point(0, 0)).X;

                            if ((xParent > xElement && ParentTreeView.FlowDirection == FlowDirection.LeftToRight)
                            || (xParent < xElement && ParentTreeView.FlowDirection == FlowDirection.RightToLeft))
                            {
                                if (IsCustomToolTipEnabled)
                                {
                                    ToolTipService.SetToolTip(this.CompleteHeaderElement, this.ToolTip);
                                    ToolTipService.SetIsEnabled(this.CompleteHeaderElement, true);
                                    ToolTipService.SetPlacementTarget(this, this.CompleteHeaderElement);
                                    ToolTipService.SetPlacement(this.CompleteHeaderElement, PlacementMode.RelativePoint);
                                    ToolTipService.SetHorizontalOffset(this.CompleteHeaderElement, 0);
                                    ToolTipService.SetVerticalOffset(this.CompleteHeaderElement, 0);
                                    ToolTipService.SetPlacementRectangle(this.CompleteHeaderElement, new Rect(0, 0, HeaderElement.ActualWidth, HeaderElement.ActualHeight));
                                }
                                else
                                {
                                    if ((this.Header as Visual) != null)
                                    {
                                        TextBlock text = VisualUtils.FindDescendant(this.Header as Visual, typeof(TextBlock)) as TextBlock;
                                        if (text != null)
                                        {
                                            ToolTipService.SetToolTip(this.CompleteHeaderElement, text.Text);
                                            ToolTipService.SetIsEnabled(this.CompleteHeaderElement, true);
                                            ToolTipService.SetPlacementTarget(this, this.CompleteHeaderElement);
                                            ToolTipService.SetPlacement(this.CompleteHeaderElement, PlacementMode.RelativePoint);
                                            ToolTipService.SetHorizontalOffset(this.CompleteHeaderElement, 0);
                                            ToolTipService.SetVerticalOffset(this.CompleteHeaderElement, 0);
                                            ToolTipService.SetPlacementRectangle(this.CompleteHeaderElement, new Rect(0, 0, HeaderElement.ActualWidth, HeaderElement.ActualHeight));
                                        }
                                    }
                                }
                            }
                            
                        }
                    }
                    catch
                    { }
                }
            }
        }

        /// <summary>
        /// Invoked when an unhandled Mouse.MouseLeave attached event is raised on this element.
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The MouseEventArgs that contains the event data.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.IsShowToolTip = false;
            ContentPresenter contentPresenter = this.Template.FindName("PART_Header", this) as ContentPresenter;
            TreeViewRowPresenter treeViewRowPresenter = this.Template.FindName("PART_RowPresenter", this) as TreeViewRowPresenter;
            ControlTemplate template = Template;
            FrameworkElement frameWorkElement;
            FrameworkElement multicolumnElement;
            if (template != null && SkinStorage.GetVisualStyle(this) == "Blend")
            {
                frameWorkElement = template.FindName("PART_CompleteHeader", this) as StackPanel;
                multicolumnElement = template.FindName("PART_SelectRectangle", this) as SelectRectangle;
                if (frameWorkElement is StackPanel)
                {
                    if ((contentPresenter is ContentPresenter) && (contentPresenter.Content != null) && !(contentPresenter.Content is TreeViewAdv) && contentPresenter.Content.GetType() != typeof(string))
                    {
                        if (this.IsSelected == false)
                        {

                            foreach (var item in VisualUtils.EnumChildrenOfType(contentPresenter, typeof(TextBlock)))
                            {
                                if (((VisualUtils.FindAncestor(item, typeof(TreeViewItemAdv)) as TreeViewItemAdv) != null) && (VisualUtils.FindAncestor(item, typeof(TreeViewItemAdv)) as TreeViewItemAdv).Header == (this as TreeViewItemAdv).Header)
                                {
                                    if (ParentTreeView != null && ParentTreeView.ReadLocalValue(TreeViewAdv.UniversalForegroundProperty) != DependencyProperty.UnsetValue)
                                        (item as TextBlock).Foreground = ParentTreeView.UniversalForeground;
                                    else if (SkinStorage.GetVisualStyle(this) == "Blend")
                                        (item as TextBlock).Foreground = ForegroundBrush1;
                                    else
                                    {
                                        (item as TextBlock).ClearValue(ForegroundProperty);
                                        SkinStorage.SetVisualStyle(item, SkinStorage.GetVisualStyle(this));
                                    }
                                }
                            }
                        }
                    }
                }
                if (multicolumnElement is SelectRectangle)
                {
                    if ((treeViewRowPresenter is TreeViewRowPresenter) && (treeViewRowPresenter.Content != null) && !(treeViewRowPresenter.Content is TreeViewAdv) && treeViewRowPresenter.Content.GetType() != typeof(string) && (this as TreeViewItemAdv).IsSelected == false)
                    {
                        foreach (var item in VisualUtils.EnumChildrenOfType(treeViewRowPresenter, typeof(TextBlock)))
                        {
                            if (((VisualUtils.FindAncestor(item, typeof(TreeViewItemAdv)) as TreeViewItemAdv) != null) && (VisualUtils.FindAncestor(item, typeof(TreeViewItemAdv)) as TreeViewItemAdv).Header == (this as TreeViewItemAdv).Header)
                            {
                                if (ParentTreeView != null && ParentTreeView.ReadLocalValue(TreeViewAdv.UniversalForegroundProperty) != DependencyProperty.UnsetValue)
                                    (item as TextBlock).Foreground = ParentTreeView.UniversalForeground;
                                else if (SkinStorage.GetVisualStyle(this) == "Blend")
                                    (item as TextBlock).Foreground = ForegroundBrush1;
                                else
                                {
                                    (item as TextBlock).ClearValue(ForegroundProperty);
                                    SkinStorage.SetVisualStyle(item, SkinStorage.GetVisualStyle(this));
                                }
                            }
                        }
                    }
                }
            }
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
            int count = 0;
            TreeViewItemAdv treeViewItem = element as TreeViewItemAdv;
            TreeViewAdvVirtualizingPanel.m_scrollmovemanually = false;
            if (treeViewItem != null)
            {
                if (treeViewItem.ParentTreeView != null)
                    treeViewItem.m_treeViewAdv = treeViewItem.ParentTreeView;
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
                        calculatedwidth = treeViewItem.internalindex * treeViewItem.GetWidthUnheader(false) * 2;
                    }

                    if (calculatedwidth != 0)
                    {
                        treeViewItem.internalwidth = calculatedwidth;
                        if (treeViewItem.ParentTreeView != null && treeViewItem.ParentTreeView.Columns != null)
                        {
                            for (int i = 0; i < treeViewItem.ParentTreeView.Columns.Count; i++)
                            {
                                PropertyInfo p =
                                    item.GetType().GetProperty(
                                        ((Binding)treeViewItem.ParentTreeView.Columns[i].DisplayMemberBinding).Path.
                                            Path.ToString());
                                if (p != null)
                                {
                                    if (p.GetValue(item, null) != null)
                                    {
                                        treeViewItem.ParentTreeView.Columns[i].Width = new GridLength(
                                            Convert.ToDouble(p.GetValue(item, null).ToString().Length * 5 +
                                                             calculatedwidth));
                                    }
                                }
                            }
                        }
                    }
                }
                treeViewItem.m_treeviewitemactualobject = item;
                if (treeViewItem != null && treeViewItem.ParentTreeView != null)
                {
                    if (treeViewItem.ParentTreeViewItem == null)
                    {
                        treeViewItem.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

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
                            (treeViewItem.ParentTreeView.m_virtualizingpanel as TreeViewAdvVirtualizingPanel).c_scrollOffset = treeViewItem.ParentTreeView.m_treeviewitemadvHeight;
                        }
                        IList list = (treeViewItem.ParentTreeView.ItemsSource as IList);
                        if (list != null)
                        {
                            treeViewItem.m_ItemIndex = list.IndexOf(treeViewItem.Header);
                        }
                    }
                    else
                    {
                        if (!(ParentTreeView != null && ParentTreeView.IsVirtualizing && ParentTreeView.VirtualizationMode == VirtualizationMode.Extended))
                            treeViewItem.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
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
                            (treeViewItem.ParentTreeViewItem.m_virtualizingpanel as TreeViewAdvVirtualizingPanel).c_scrollOffset = treeViewItem.ParentTreeView.AutoScrollStartDistance;
                        }
                        IList list = (treeViewItem.ParentTreeViewItem.ItemsSource as IList);
                        if (list != null)
                        {
                            treeViewItem.m_ItemIndex = list.IndexOf(treeViewItem.Header);
                        }
                    }
                    if (ParentTreeView.ExpandStateLinearList.Contains(item) && ParentTreeView.IsVirtualizing)
                    {
                        if (ParentTreeView.VirtualizationMode == VirtualizationMode.Extended)
                        {
                            ParentTreeView.isAlreadyExpanded = true;
                            treeViewItem.IsExpanded = true;
                            ParentTreeView.isAlreadyExpanded = false;
                        }
                        else
                            treeViewItem.IsExpanded = true;
                    }

                    if (ParentTreeView.SelectedStateLinearList.Contains(item) && ParentTreeView.IsVirtualizing && ParentTreeView.droppedobjects != null && !ParentTreeView.droppedobjects.Contains(item) && !treeViewItem.ParentTreeView.SelectedItems.Contains(item))
                    {
                        treeViewItem.ParentTreeView.m_itemsselectedfromlinearlist = true;
                        treeViewItem.IsSelected = true;
                    }
                    else
                    {
                        treeViewItem.ParentTreeView.m_itemsselectedfromlinearlist = false;
                    }
                }
            }

            if (ParentTreeView != null)
            {
                if (treeViewItem != null)
                {
                    if (treeViewItem.IsSelected)
                    {
                        if (!ParentTreeView.m_selectedContainers.Contains(treeViewItem))
                        {
                            if (!ParentTreeView.m_selectedContainers.Contains(treeViewItem))
                            {
                                if (!ParentTreeView.AllowMultiSelect)
                                {
                                    ParentTreeView.ChangeSelection(item, treeViewItem, treeViewItem.IsSelected, false);
                                    ParentTreeView.m_selectedContainers.Clear();
                                    ParentTreeView.m_selectedContainers.Add(treeViewItem);
                                }
                                else
                                {
                                    ParentTreeView.m_selectedContainers.Add(treeViewItem);
                                }
                            }
                        }
                    }

                    else
                    {
                        if (ParentTreeView.m_selectedContainers.Contains(treeViewItem))
                        {
                            ParentTreeView.m_selectedContainers.Remove(treeViewItem);
                        }
                    }
                }
            }

            if (ParentTreeView != null)
            {
                if (ParentTreeView.m_treeviewadv != null)
                {
                    if (ParentTreeView.m_itemObject != null)
                    {
                        count = 0;
                        if (ParentTreeView.m_itemObject.Count > 0)
                        {
                            if (ParentTreeView.m_itemObject.Contains(item))
                            {
                                count++;
                            }
                            if (count > 0)
                            {
                                for (int i = 0; i < ParentTreeView.m_itemObject.Count; i++)
                                {
                                    if (item.Equals(ParentTreeView.m_itemObject[i]))
                                    {
                                        ParentTreeView.m_treeviewadv.RemoveAt(i);
                                        ParentTreeView.m_treeviewadv.Insert(i, element as TreeViewItemAdv);
                                    }
                                }
                            }
                        }
                        if (count == 0)
                        {
                            ParentTreeView.m_treeviewadv.Add(element as TreeViewItemAdv);
                            ParentTreeView.m_itemObject.Add(item);
                        }
                    }
                    else
                    {
                        count = 0;
                        if (ParentTreeView.m_treeviewadv.Count > 0)
                        {
                            if (ParentTreeView.m_treeviewadv.Contains(element as TreeViewItemAdv))
                            {
                                count++;
                            }
                            if (count > 0)
                            {
                                for (int i = 0; i < ParentTreeView.m_treeviewadv.Count; i++)
                                {
                                    if (item.Equals(ParentTreeView.m_itemObject[i]))
                                    {
                                        ParentTreeView.m_treeviewadv.RemoveAt(i);
                                        ParentTreeView.m_treeviewadv.Insert(i, element as TreeViewItemAdv);
                                    }
                                }
                            }
                        }
                        if (count == 0)
                        {
                            ParentTreeView.m_treeviewadv.Add(element as TreeViewItemAdv);
                        }
                    }
                }
            }
            if (treeViewItem != null && ParentTreeView != null)
            {
                if (ParentTreeView.IsVirtualizing && !ParentTreeView.LinearList.ContainsKey(item))
                {
                    ParentTreeView.LinearList.Add(item, 0);
                    ParentTreeView.isLinearItemsChanged = true;
                    
                    if (((IVirtualTree)this.DataContext).ExtentHeight <= 0d)
                    {
                        if (((IVirtualTree)this.DataContext).ItemsCount == 0d)
                            ((IVirtualTree)this.DataContext).ItemsCount = this.Items.Count;
                        if (ParentTreeView.treeHeight == 0d)
                            ParentTreeView.treeHeight = 19.96d;
                        if (ParentTreeView.m_treeviewadvVirtualizingPanel != null)
                            ParentTreeView.m_treeviewadvVirtualizingPanel.m_extentSize.Height += (((IVirtualTree)this.DataContext).ItemsCount * ParentTreeView.treeHeight);
                        ParentTreeView.ExtentHeight += (((IVirtualTree)this.DataContext).ItemsCount * ParentTreeView.treeHeight);
                        ParentTreeView.expandedItemsCount++;

                        ((IVirtualTree)this.DataContext).ExtentHeight = (((IVirtualTree)this.DataContext).ItemsCount * ParentTreeView.treeHeight);
                        IVirtualTree parent = ((IVirtualTree)this.DataContext).Parent;
                        if (parent == null && ParentTreeViewItem != null && ParentTreeViewItem.DataContext != null)
                        {
                            ((IVirtualTree)this.DataContext).Parent = ParentTreeViewItem.DataContext as IVirtualTree;
                            parent = ((IVirtualTree)this.DataContext).Parent;
                        }
                        do
                        {
                            if (parent != null)
                            {
                                parent.ExtentHeight += (((IVirtualTree)this.DataContext).ItemsCount * ParentTreeView.treeHeight);
                                parent = ((IVirtualTree)parent).Parent;
                            }
                        } while (parent != null);

                        ((IVirtualTree)this.DataContext).IsExpanded = IsExpanded = true;
                        ParentTreeView.ScrollHost.InvalidateScrollInfo();
                    }
                }

                treeViewItem.MultiColumnEnable = ParentTreeView.MultiColumnEnable;
           
            }
            if (treeViewItem != null && ParentTreeView != null && ParentTreeView.IsVirtualizing)
            {
                if (ParentTreeView.SelectedItems.Contains(treeViewItem.Header) || (item is IVirtualTree && (item as IVirtualTree).IsSelected))
                {
                    TreeViewItemAdv selItem = this.ItemContainerGenerator.ContainerFromItem(treeViewItem.Header) as TreeViewItemAdv;
                    if (!selItem.IsSelected || (selItem.IsSelected && ParentTreeView.internaltargetindex != 0))
                    {
                        if (treeViewItem.m_treeviewitemactualobject != treeViewItem.Header)
                            treeViewItem.m_treeviewitemactualobject = treeViewItem.Header;
                        if (selItem != null)
                        {
                            ParentTreeView.AllowChange = false;
                            ParentTreeView.m_selectedInCollectionChanged = true;
                            ParentTreeView.UnSubscribeSelectionChangedEvent = true;
                            if (ParentTreeView.SelectedItems.Count > 0)
                                ParentTreeView.SetSelectedItem(ParentTreeView.SelectedItems[ParentTreeView.SelectedItems.Count - 1]);
                            if (ParentTreeView.SelectedTreeViewItems != null)
                                ParentTreeView.SelectedTreeViewItems.Add(selItem);
                            selItem.IsSelected = true;
                            ParentTreeView.UnSubscribeSelectionChangedEvent = false;
                            ParentTreeView.m_selectedInCollectionChanged = false;
                            ParentTreeView.AllowChange = true;
                        }
                    }
                    if ((item as IVirtualTree) != null && (item as IVirtualTree).IsSelected && (!selItem.IsSelected && ParentTreeView.internaltargetindex == 0))
                        (item as IVirtualTree).IsSelected = false;
                }
            }
        }

        /// <summary>
        /// When overridden in a derived class, undoes the effects of the <see cref="M:System.Windows.Controls.ItemsControl.PrepareContainerForItemOverride(System.Windows.DependencyObject,System.Object)"/> method.
        /// </summary>
        /// <param name="element">The container element.</param>
        /// <param name="item">The item.</param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
            if (ParentTreeView != null && ParentTreeView.IsVirtualizing)
            {
                ParentTreeView.LinearList.Remove(element);
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
        /// Returns class-specific AutomationPeer implementations
        /// for the Windows Presentation Foundation (WPF) infrastructure.
        /// </summary>
        /// <returns>The type-specific AutomationPeer implementation.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new TreeViewItemAdvAutomationPeer(this);
        }

        /// <summary>
        /// Called when Items of the control is changed.
        /// </summary>
        /// <param name="e">The instance containing the event data.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            if (ParentTreeView != null && !ParentTreeView.IsVirtualizing)
            {
                ParentTreeView.SelectedFalseItem = null;

                if (!m_bAnimating && IsExpanded && ParentTreeView.AnimationType != AnimationType.None
                    && IsNeedAnimation(ParentTreeView.DraggingItmesContiners))
                {
                    UpdateItemChangedAnimation(e);

                    if (e.NewItems != null || e.OldItems != null)
                    {
                        if (e.Action == NotifyCollectionChangedAction.Add)
                        {
                            BeginExpandAnimation();
                        }
                        else if (e.Action == NotifyCollectionChangedAction.Remove && !TreeViewAdv.IsDragging)
                        {
                            BeginCollapseAnimation();
                        }
                    }
                }
            }
            if (ParentTreeView != null && ParentTreeView.IsLoaded && ((e.Action == NotifyCollectionChangedAction.Remove) && (ParentTreeView.DraggingItmes!=null && ParentTreeView.DraggingItmes.Count == 0) && (ParentTreeView.DraggingParentItmes!=null && ParentTreeView.DraggingParentItmes.Count == 0)))
            {
                if (ParentTreeView != null)
                {
                    foreach (TreeViewItemAdv item in e.OldItems)
                    {
                        if (item != null)
                        {
                            if (ParentTreeView.m_treeviewadv != null && ParentTreeView.m_treeviewadv.Contains(item))
                                ParentTreeView.m_treeviewadv.Remove(item);
                            if (ParentTreeView.m_itemObject != null && ParentTreeView.m_itemObject.Contains(item))
                                ParentTreeView.m_itemObject.Remove(item);
                            item.m_treeviewadvVirtualizingPanel = null;
                            item.m_virtualizingpanel = null;
                            if (m_fakeItemsPanel != null && m_fakeItemsPanel.m_cashedMeasureSize != null)
                                m_fakeItemsPanel.m_cashedMeasureSize.Remove(item);
                            item.m_fakeItemsPanel = null;
                            item.m_itemsHost = null;
                            if (ParentTreeView.LinearItems != null)
                                ParentTreeView.LinearItems.Remove(item);
                        }
                    }
                }
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        break;
                    }

                case NotifyCollectionChangedAction.Remove:
                    {
                        if (ParentTreeView != null)
                        {
                            int count = ParentTreeView.SelectedItems.Count;
                            for (int i = count - 1; i >= 0; i--)
                            {
                                if (i >= 0)
                                {
                                    if (!this.Items.Contains(ParentTreeView.SelectedItems[i]))
                                    {
                                        if (this.ParentTreeView.SelectedItem == ParentTreeView.SelectedItems[i] && 
                                            ParentTreeView.SelectedItems.Count > 0)
                                        {
                                            this.ParentTreeView.SetSelectedItem(ParentTreeView.SelectedItems[0]);
                                            this.ParentTreeView.UpdateSelectedValue(ParentTreeView.SelectedItems[0]);
                                        }
                                        ParentTreeView.oldselectedItems.Add(ParentTreeView.SelectedItems[i]);
                                        ParentTreeView.SelectedItems.RemoveAt(i);
                                    }
                                }
                            }
                            if (ParentTreeView.SelectedItems.Count == 0)
                            {
#if !SyncfusionFramework3_5
                                this.ParentTreeView.SetCurrentValue(TreeViewAdv.SelectedTreeItemProperty, null);
                                this.ParentTreeView.SetCurrentValue(TreeViewAdv.SelectedTreeItemObjectProperty, null);
#else
                                this.ParentTreeView.SetValue(TreeViewAdv.SelectedTreeItemProperty, null);
                                this.ParentTreeView.SetValue(TreeViewAdv.SelectedTreeItemObjectProperty, null);
#endif
                                this.ParentTreeView.SetSelectedItem(null);
                            }
                        }
                        if (ParentTreeView != null && ParentTreeView.IsVirtualizing)
                        {
                            foreach (object item in e.OldItems)
                            {
                                if (item is TreeViewItemAdv)
                                {
                                    ParentTreeView.LinearList.Remove(item);
                                    TreeViewAdv.IterateItems(item as TreeViewItemAdv, ParentTreeView);
                                }
                                else
                                {
                                    TreeViewItemAdv item1 = ItemContainerGenerator.ContainerFromItem(item) as TreeViewItemAdv;
                                    if (item1 != null)
                                    {
                                        ParentTreeView.LinearList.Remove(item1);
                                        TreeViewAdv.IterateItems(item1, ParentTreeView);
                                    }
                                }
                            }
                        }

                        if (this.ContainsSelection)
                        {
                            TreeViewAdv parentFxTreeView = this.ParentTreeView;

                            if ((parentFxTreeView == null) || parentFxTreeView.IsSelectedContainerHookedUp)
                            {
                                break;
                            }

                            if (ParentTreeView != null && !TreeViewAdv.IsDragging)
                            {
                                this.ContainsSelection = false;
                                if (ParentTreeView.SelectParentContainer)
                                    this.Select(true);
                                else
                                    this.Select(false);
                            }
                        }

                        InvalidatePanelRender();

                        break;
                    }

                case NotifyCollectionChangedAction.Reset:
                    {
                        if (this.ContainsSelection)
                        {
                            TreeViewAdv parentFxTreeView = this.ParentTreeView;

                            if ((parentFxTreeView == null) || parentFxTreeView.IsSelectedContainerHookedUp)
                            {
                                break;
                            }

                            if (ParentTreeView != null && !TreeViewAdv.IsDragging)
                            {
                                this.ContainsSelection = false;
                            }
                        }

                        if (ParentTreeView != null)
                        {
                            InvalidatePanelRender();
                        }

                        break;
                    }
                case NotifyCollectionChangedAction.Replace:
                    {
                        foreach (object item in e.OldItems)
                        {
                            var treeItem = item as TreeViewItemAdv;

                            if (!(treeItem is TreeViewItemAdv))
                                treeItem = this.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItemAdv;

                            if (treeItem != null)
                            {
                                this.ParentTreeView.UnSubscribeSelectionChangedEvent = true;
                                treeItem.IsSelected = false;
                                this.ParentTreeView.UnSubscribeSelectionChangedEvent = false;

                                if (this.ParentTreeView.SelectedItems.Contains(treeItem.Header))
                                {
                                    this.ParentTreeView.SelectedItems.Remove(treeItem.Header);
                                    if (treeItem.Header == this.ParentTreeView.SelectedTreeItem)
                                    {
#if !SyncfusionFramework3_5
                                        this.ParentTreeView.SetCurrentValue(TreeViewAdv.SelectedTreeItemProperty, null);
                                        this.ParentTreeView.SetCurrentValue(TreeViewAdv.SelectedTreeItemObjectProperty, null);
#else
                                        this.ParentTreeView.SetValue(TreeViewAdv.SelectedTreeItemProperty, null);
                                        this.ParentTreeView.SetValue(TreeViewAdv.SelectedTreeItemObjectProperty, null);
#endif
                                        this.ParentTreeView.SetSelectedItem(null);
                                    }
                                }
                            }
                        }

                        break;
                    }

                default:
                    {
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
            base.OnItemsSourceChanged(oldValue, newValue);

            if (ParentTreeView != null && ParentTreeView.IsVirtualizing)
            {
                if (oldValue != null)
                {
                    foreach (object item in oldValue)
                    {
                        TreeViewItemAdv tempitem = this.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItemAdv;
                        if (tempitem != null)
                        {
                            ParentTreeView.LinearList.Remove(tempitem);
                        }
                    }
                }
            }

            InvalidatePanelRender();
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

            if (ParentTreeView.AllowDragDrop && !ParentTreeView.IsDragInEditingState && !IsFakeItem && IsNeedShowDragMarker)
            {
                StartDragTimer(e);
            }
            else if (!ParentTreeView.AllowDragDrop)
            {
                StartDragTimer(e);
            }
        }

        /// <summary>
        /// Invoked when an unhandled DragDrop.DragLeave attached event reaches an element
        /// in its route that is derived from this class. Implement this method to
        /// add class handling for this event.
        /// </summary>
        /// <param name="e">The DragEventArgs that contains the event data.</param>
        protected override void OnDragLeave(DragEventArgs e)
        {
            base.OnDragLeave(e);
            if (ParentTreeView != null)
            {
                if (!IsFakeItem && ParentTreeView.AllowDragDrop && !ParentTreeView.IsDragInEditingState)
                {
                    IsDragOver = false;
                    StopDragTimer();
                }
                else if (!ParentTreeView.AllowDragDrop)
                {
                    IsDragOver = false;
                    StopDragTimer();
                }
            }
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            if (!ParentTreeView.AllowDragDrop)
            {
                IsDragOver = false;
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
            TreeViewAdv.DragOverElement = e.Source as FrameworkElement;

            if (!IsFakeItem && (ParentTreeView.AllowDragDrop || ParentTreeView.AllowDrop) && !ParentTreeView.IsDragInEditingState)
            {
                IsDragOver = !IsNeedShowDragMarker;

                if (IsNeedShowDragMarker && m_bIsDragByTimer)
                {
                    Point currentPoint = MouseUtils.GetMousePosition(this);

                    if (!currentPoint.Equals(m_dragByTimerPoint))
                    {
                        StopDragTimer();
                        StartDragTimer(e);
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when to the KeyDown event is received.
        /// </summary>
        /// <param name="e">Information about the event.</param>
        /// 
        static bool bSettingFocusOnElement;
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (!e.Handled)
            {
                if (ParentTreeView != null && ParentTreeView.WasNavigationKeyDown())
                {
                    TreeViewItemAdv focusedItem = TreeViewAdv.GetTreeViewItemFromChildren(Keyboard.FocusedElement as FrameworkElement);

                    if (focusedItem != null && focusedItem == this)
                    {
                        bool bShift = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
                        if (bShift && (e.Key == Key.Up || e.Key == Key.Down))
                        {
                            if (e.Key == Key.Up)
                            {
                                var toSelect = focusedItem.GetPreviousVisibleItem(false);
                                if (toSelect != null)
                                {
                                    if (!toSelect.IsSelected)
                                        toSelect.IsSelected = true;
                                    else
                                        focusedItem.IsSelected = false;
                                    bSettingFocusOnElement = true;
                                    toSelect.Focus();
                                    bSettingFocusOnElement = false;
                                    UpKeyHandle();
                                    e.Handled = true;
                                    return;
                                }
                            }
                            else if (e.Key == Key.Down)
                            {
                                var toSelect = focusedItem.GetNextVisibleItem(false);
                                if (toSelect != null)
                                {
                                    if (!toSelect.IsSelected)
                                        toSelect.IsSelected = true;
                                    else
                                        focusedItem.IsSelected = false;
                                    bSettingFocusOnElement = true;
                                    toSelect.Focus();
                                    bSettingFocusOnElement = false;
                                    DownKeyHandle();
                                    e.Handled = true;
                                    return;
                                }
                            }
                        }
                        else
                        {
                            ParentTreeView.externalSelect = false;
                            ParentTreeView.m_ismouseSelection = true;
                            ParentTreeView.SetSelectAndFocus(focusedItem, false);
                            ParentTreeView.m_ismouseSelection = false;
                            if (!(e.OriginalSource is TreeViewItemAdv))
                            {
                                e.Handled = true;
                                return;
                            }
                        }
                    }
                }

                switch (e.Key)
                {
                    case Key.Left:
                        {
                            HandleLeftKey(e);
                            break;
                        }

                    case Key.Right:
                        {
                            HandleRightKey(e);
                            break;
                        }

                    case Key.Add:
                        {
                            HandleAddKey(e);
                            break;
                        }

                    case Key.Subtract:
                        {
                            HandleSubtractKey(e);
                            break;
                        }

                    case Key.Up:
                        {
                            UpKeyHandle();
                            return;
                        }

                    case Key.Down:
                        {
                            DownKeyHandle();
                            e.Handled = HandleDownKey();
                            return;
                        }

                    case Key.Home:
                        {
                            HandleHomeKey(e);
                            break;
                        }

                    case Key.End:
                        {
                            HandleEndKey(e);
                            break;
                        }

                    case Key.Prior:
                        {
                            HandlePriorKey(e);
                            break;
                        }

                    case Key.Next:
                        {
                            HandleNextKey(e);
                            break;
                        }

                    case Key.Enter:
                        {
                            HandleEnterKey(e);
                            break;
                        }

                    case Key.Escape:
                        {
                            HandleEscapeKey(e);
                            break;
                        }

                    case Key.Space:
                        {
                            HandleSpaceKey(e);
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Invoked when the KeyboardFocus event is received.
        /// </summary>
        /// <param name="e">Information about the event.</param>
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);

            if (!bSettingFocusOnElement && ParentTreeView != null && ParentTreeView.WasNavigationKeyDown())
            {
                TreeViewItemAdv focusedItem = TreeViewAdv.GetTreeViewItemFromChildren(Keyboard.FocusedElement as FrameworkElement);

                if (focusedItem != null && focusedItem == this)
                {
                    ParentTreeView.externalSelect = false;
                    ParentTreeView.m_ismouseSelection = true;
                    ParentTreeView.SetSelectAndFocus(focusedItem);
                    ParentTreeView.m_ismouseSelection = false;
                    if (!(e.OriginalSource is TreeViewItemAdv))
                        e.Handled = true;
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
            if (ParentTreeView != null && (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift &&
                ParentTreeView.IsMultiselection && !IsExpanded)
                ParentTreeView.IsMultiselection = false;
            base.OnPreviewMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled PreviewMouseRightButtonDown routed event reaches
        /// an element in its route that is derived from this class.
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data.
        /// The event data reports that the right mouse button was pressed.</param>
        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (ParentTreeView != null && (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift && ParentTreeView.IsMultiselection && !IsExpanded)
                ParentTreeView.IsMultiselection = false;
            Focus();
            base.OnPreviewMouseRightButtonDown(e);
         
        }

        /// <summary>
        /// Invoked when an unhandled PreviewMouseRightButtonUp routed event reaches
        /// an element in its route that is derived from this class.
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data.
        /// The event data reports that the left mouse button was pressed.</param>
        protected override void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e)
        {
            if (ParentTreeView != null)
                ParentTreeView.externalSelect = true;
            Focus();
            base.OnPreviewMouseRightButtonUp(e);
        }

        /// <summary>
        /// Invoked when an unhandled PreviewMouseLeftButtonUp routed event reaches
        /// an element in its route that is derived from this class.
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data.
        /// The event data reports that the left mouse button was pressed.</param>
        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (ParentTreeView != null)
                ParentTreeView.externalSelect = true;
            base.OnPreviewMouseLeftButtonUp(e);
            if (Focusable)
            {
                if (ParentTreeView != null && !ParentTreeView.IsMultiselection
                    && !IsInEditMode)
                {
                    //if ((e.ClickCount == 1) && IsMouseOverComplateHeader
                    //    && m_needEditByClick && IsSelected && IsSelectionActive && !IsReadOnly && !isDoubleClick)
                    //{
                    //    m_editTimer.Tag = this;
                    //    m_editTimer.Tick += new EventHandler(Edit_Tick);
                    //    m_editTimer.Interval = TimeSpan.FromMilliseconds(C_editDelay);
                    //    m_editTimer.Start();
                    //    m_bIsEditByTimer = true;

                    //    m_editByTimerPoint = MouseUtils.GetMousePosition(this);
                    //}
                    //else
                    //{
                    //    m_bIsEditByTimer = false;
                    //    m_editTimer.Stop();
                    //    isDoubleClick = false;
                    //    m_needEditByClick = true;
                    //}
                    if (ParentTreeView.AllowDynamicResizing && ParentTreeView.MultiColumnEnable)
                    {
                        ParentTreeView.Flag_Width = false;
                        if (this.Items.Count > 0)
                        {
                            TreeViewRowPresenter rowPresenter = null;
                            for (int i = 0; i < this.Items.Count; i++)
                            {
                                if (this.Items[i] as TreeViewItemAdv == null)
                                {
                                    if ((this.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv) != null &&
                                        (this.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv).HeaderElement != null &&
                                        ((this.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv).HeaderElement as ContentControl) != null &&
                                        ((this.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv).HeaderElement as ContentControl).Content != null)
                                        rowPresenter = (((this.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv).HeaderElement as ContentControl).Content) as TreeViewRowPresenter;
                                }
                                else
                                {
                                    if ((this.Items[i] as TreeViewItemAdv) != null &&
                                        (this.Items[i] as TreeViewItemAdv).HeaderElement != null &&
                                        ((this.Items[i] as TreeViewItemAdv).HeaderElement as ContentControl) != null &&
                                        ((this.Items[i] as TreeViewItemAdv).HeaderElement as ContentControl).Content != null)
                                        rowPresenter = (((this.Items[i] as TreeViewItemAdv).HeaderElement as ContentControl).Content) as TreeViewRowPresenter;
                                }
                                if (rowPresenter != null)
                                {
                                    rowPresenter.InvalidateArrange();
                                }
                            }
                        }
                    }
                }
                if (ParentTreeView != null && (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift && ParentTreeView.IsMultiselection && !IsExpanded)
                    ParentTreeView.IsMultiselection = false;
            }
        }

        /// <summary>
        /// Invoked when an unhandled Mouse.MouseMove attached event reaches
        /// an element in its route that is derived from this class.
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The MouseEventArgs that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (m_bIsEditByTimer)
            {
                Point currentPoint = MouseUtils.GetMousePosition(this);

                if (!currentPoint.Equals(m_editByTimerPoint))
                {
                    m_bIsEditByTimer = false;
                }
            }

            if ((this is TreeViewItemAdv) && (this as TreeViewItemAdv).IsSelected)
            {
                ParentTreeView.DragTreeItem = null;
                ParentTreeView.DragTreeItem = this;
            }
            /*
            ContentPresenter contentPresenter = this.Template.FindName("PART_Header", this) as ContentPresenter;
            TreeViewRowPresenter treeViewRowPresenter = this.Template.FindName("PART_RowPresenter", this) as TreeViewRowPresenter;
            ControlTemplate template = Template;
            FrameworkElement frameWorkElement;
            FrameworkElement multicolumnElement;
            if (template != null)
            {
                frameWorkElement = template.FindName("PART_CompleteHeader", this) as StackPanel;
                multicolumnElement = template.FindName("PART_SelectRectangle", this) as SelectRectangle;
                if (frameWorkElement is StackPanel)
                {
                    if ((contentPresenter is ContentPresenter) && (contentPresenter.Content != null) && !(contentPresenter.Content is TreeViewAdv) && contentPresenter.Content.GetType() != typeof(string) && (this as TreeViewItemAdv).IsSelected == false)
                    {
                        if ((frameWorkElement as StackPanel).IsMouseOver == true)
                        {
                            foreach (var item in VisualUtils.EnumChildrenOfType(contentPresenter, typeof(TextBlock)))
                            {
                                if (((VisualUtils.FindAncestor(item, typeof(TreeViewItemAdv)) as TreeViewItemAdv) != null) && (VisualUtils.FindAncestor(item, typeof(TreeViewItemAdv)) as TreeViewItemAdv).Header == (this as TreeViewItemAdv).Header)
                                {
                                    if (SkinStorage.GetVisualStyle(this) == "Blend")
                                        (item as TextBlock).Foreground = ForegroundBrush;
                                    else
                                    {
                                        (item as TextBlock).ClearValue(ForegroundProperty);
                                        SkinStorage.SetVisualStyle(item, SkinStorage.GetVisualStyle(this));
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var item in VisualUtils.EnumChildrenOfType(contentPresenter, typeof(TextBlock)))
                            {
                                if ((VisualUtils.FindAncestor(item, typeof(TreeViewItemAdv)) as TreeViewItemAdv).Header == (this as TreeViewItemAdv).Header)
                                {
                                    if (SkinStorage.GetVisualStyle(this) == "Blend")
                                        (item as TextBlock).Foreground = ForegroundBrush1;
                                    else
                                    {
                                        (item as TextBlock).ClearValue(ForegroundProperty);
                                        SkinStorage.SetVisualStyle(item, SkinStorage.GetVisualStyle(this));
                                    }
                                }
                            }
                        }
                    }
                }
                if (multicolumnElement is SelectRectangle)
                {
                    if ((treeViewRowPresenter is TreeViewRowPresenter) && (treeViewRowPresenter.Content != null) && !(treeViewRowPresenter.Content is TreeViewAdv) && treeViewRowPresenter.Content.GetType() != typeof(string) && (this as TreeViewItemAdv).IsSelected == false)
                    {
                        foreach (var item in VisualUtils.EnumChildrenOfType(treeViewRowPresenter, typeof(TextBlock)))
                        {
                            if (SkinStorage.GetVisualStyle(this) == "Blend")
                                (item as TextBlock).Foreground = ForegroundBrush;
                            else
                            {
                                (item as TextBlock).ClearValue(ForegroundProperty);
                                SkinStorage.SetVisualStyle(item, SkinStorage.GetVisualStyle(this));
                            }
                        }
                    }
                }
            }
            */
        }

        /// <summary>
        /// Called on mouse left button down event.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (ParentTreeView != null)
                ParentTreeView.externalSelect = false;
            if (Focusable)
            {
                if (!e.Handled && base.IsEnabled)
                {
                    if ((e.ClickCount % 2) == 0)
                    {
                        m_bIsEditByTimer = false;
                    }

                    if (!IsSelectionActive && !IsKeyboardFocusWithin && HeaderElement != null)
                    {
                        Focus();
                    }
                }
            }
            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Called on mouse right button down event.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (ParentTreeView != null)
                ParentTreeView.externalSelect = false;
            if (Focusable)
            {
                if (!e.Handled && base.IsEnabled)
                {
                    if ((e.ClickCount % 2) == 0)
                    {
                        m_bIsEditByTimer = false;
                    }

                    if (!IsSelectionActive && !IsKeyboardFocusWithin && HeaderElement != null)
                    {
                        Focus();
                    }
                }
            }
            base.OnMouseRightButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (ParentTreeView != null)
                ParentTreeView.externalSelect = true;
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            if (ParentTreeView != null)
                ParentTreeView.externalSelect = true;
            base.OnMouseRightButtonUp(e);
        }

        /// <summary>
        /// Invalidates render of the items panel.
        /// </summary>
        internal void InvalidatePanelRender()
        {
            IItemsPanelRef panel = this as IItemsPanelRef;

            if (panel != null && panel.ItemsPanel != null)
            {
                panel.ItemsPanel.InvalidateRender();
            }
        }

        /// <summary>
        /// Determines whether [is need animation] [the specified drag items].
        /// </summary>
        /// <param name="dragItems">The drag items.</param>
        /// <returns>
        /// <c>true</c> if [is need animation] [the specified drag items]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsNeedAnimation(TreeViewItemAdvCollection dragItems)
        {
            bool bNeed = true;

            if (dragItems != null && dragItems.Count > 0)
            {
                for (int i = 0; i < dragItems.Count; i++)
                {
                    if (TreeViewAdv.IsInChildren(this, dragItems[i], false))
                    {
                        bNeed = false;
                        break;
                    }
                }
            }

            return bNeed;
        }

        /// <summary>
        /// Calls when timer counting ends.
        /// </summary>
        /// <param name="sender">Object target.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void Edit_Tick(object sender, EventArgs e)
        {
            DispatcherTimer timer = sender as DispatcherTimer;

            if (timer != null)
            {
                IsInEditMode = m_bIsEditByTimer;
                timer.Tick -= new EventHandler(Edit_Tick);
            }
        }

        /// <summary>
        /// Calls when timer counting ends.
        /// </summary>
        /// <param name="sender">Object target.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void DragTimer_Tick(object sender, EventArgs e)
        {
            DispatcherTimer timer = sender as DispatcherTimer;

            if (timer != null)
            {
                timer.Tick -= new EventHandler(DragTimer_Tick);

                if (timer.Tag != null && timer.IsEnabled)
                {
                    DragEventArgs dragArgs = timer.Tag as DragEventArgs;

                    if (Items.Count > 0 && dragArgs != null
                        && m_bIsDragByTimer && this is IItemsPanelRef && !IsFakeItem
                        && !IsNeedShowDragMarker && TreeViewAdv.IsPossipleDrop && ParentTreeView.IsExpandOnDrop)
                    {
                        IsExpanded = true;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the default size of the item.
        /// </summary>
        /// <returns>Returns the Size</returns>
        internal static Size GetDefaultItemSize()
        {
            return new Size(C_defaultItemHeight, C_defaultItemHeight);
        }

        /// <summary>
        /// Gets the default items panel offset.
        /// </summary>
        /// <returns>Return the Panel offset</returns>
        internal static double GetDefaultItemsPanelOffset()
        {
            return C_defaultItemsOffset;
        }

        /// <summary>
        /// Gets the hash key.
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <returns>object type value </returns>
        internal static object GetHashKey(object obj)
        {
            object key = null;

            if (obj is TreeViewItemAdv)
            {
                TreeViewItemAdv item = (TreeViewItemAdv)obj;

                if (item.ParentItemsControl != null)
                {
                    object keyItem = item.ParentItemsControl.ItemContainerGenerator.ItemFromContainer(item);

                    if (keyItem == DependencyProperty.UnsetValue)
                    {
                        key = item.GetHashCode();
                    }
                    else if (keyItem != null)
                    {
                        key = keyItem.GetHashCode();
                    }
                }

                if (key == null)
                {
                    key = item.GetHashCode();
                }
            }
            else if (!(obj is TreeViewAdv))
            {
                key = obj.GetHashCode();
            }

            return key;
        }

        /// <summary>
        /// This method corrects IsExpanded.
        /// </summary>
        /// <param name="d">Object to which this property belongs.</param>
        /// <param name="baseValue">The property whish should be corrected.</param>
        /// <returns>
        /// Corrected value.
        /// </returns>
        private static object CoerceOnIsExpanded(DependencyObject d, object baseValue)
        {
            TreeViewItemAdv item = d as TreeViewItemAdv;
            bool coerceValue = (bool)baseValue;

            if (item != null && item.ParentTreeView != null && item.Items.Count > 0)
            {
                if (item.m_bUpdateCoerce)
                {
                    item.m_bUpdateCoerce = false;
                }
                else if (item.m_bAnimating || item.IsExpanded != coerceValue)
                {
                    if (!coerceValue)
                    {
                        ExpandingCollapsingEventArgs args = new ExpandingCollapsingEventArgs(TreeViewAdv.CollapsingEvent, item);
                        if (item.ParentTreeView.isdoubleclick)
                        {
                            args.IsExpandingOnDoubleClick = true;
                            item.ParentTreeView.isdoubleclick = false;
                        }
                        else
                        {
                            args.IsExpandingOnDoubleClick = false;
                        }
                        item.ParentTreeView.BeforeCollapse(args);
                        if (!args.Cancel)
                        {
                            if (item.ParentTreeView.AnimationType != AnimationType.None)
                            {
                                coerceValue = true;
                                item.StartAnimation(false);
                            }
                        }
                        else
                        {
                            coerceValue = true;
                        }
                    }
                    else
                    {
                        ExpandingCollapsingEventArgs args = new ExpandingCollapsingEventArgs(TreeViewAdv.ExpandingEvent, item);
                        if (item.ParentTreeView.isdoubleclick)
                        {
                            args.IsExpandingOnDoubleClick = true;
                            item.ParentTreeView.isdoubleclick = false;
                        }
                        else
                        {
                            args.IsExpandingOnDoubleClick = false;
                        }
                        item.ParentTreeView.BeforeExpand(args);
                        if (!args.Cancel)
                        {
                            if (item.ParentTreeView.AnimationType != AnimationType.None && !item.isExpandedAlready)
                            {
                                item.DesiredItemHeight = item.DesiredSize.Height;
                                item.StartAnimation(true);
                            }
                        }
                        else
                        {
                            coerceValue = false;
                        }
                    }
                }
            }

            if (item != null && item.ParentTreeView != null && item.ParentTreeView.EditingItem != null)
            {
                item.ParentTreeView.EditingItem.IsInEditMode = false;
            }

            return coerceValue;
        }

        /// <summary>
        /// Sorts the tree view.
        /// </summary>
        public void SortTreeViewItem()
        {
            TreeViewItemAdv tree = null;
            if ((this as TreeViewItemAdv) != null)
            {
                tree = this as TreeViewItemAdv;
                if (tree.ParentTreeView != null)
                {
                    (tree as ItemsControl).Items.SortDescriptions.Clear();
                    tree.ParentTreeView.SortingTreeView((DependencyObject)tree, tree.newvalue, tree.oldvalue);
                    if (!tree.ParentTreeView.EnabledRecursiveSorting)
                    {
                        foreach (object obj in tree.Items)
                        {
                            TreeViewItemAdv titem = obj as TreeViewItemAdv;

                            if (titem != null)
                            {
                                if (titem.HasItems)
                                {
                                    (titem as ItemsControl).Items.SortDescriptions.Clear();
                                    titem.ParentTreeView.SortingTreeView((DependencyObject)titem, tree.newvalue, tree.oldvalue);
                                    titem.ParentTreeView.IterateItems(titem, titem.ParentTreeView, tree.newvalue, tree.oldvalue);
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
                                    titem.ParentTreeView.SortingTreeView((DependencyObject)titem, tree.newvalue, tree.oldvalue);
                                    titem.ParentTreeView.IterateItems(titem, titem.ParentTreeView, tree.newvalue, tree.oldvalue);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when [sorting changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSortingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv instance1 = d as TreeViewItemAdv;
            if (instance1 != null)
            {
                instance1.newvalue = (SortDirection)e.NewValue;
                instance1.oldvalue = (SortDirection)e.OldValue;
                instance1.SortTreeViewItem();
            }
        }

        /// <summary>
        /// This method corrects IsExpandedInternal.
        /// </summary>
        /// <param name="d">Object to which this property belongs.</param>
        /// <param name="baseValue">The property whish should be corrected.</param>
        /// <returns>
        /// Corrected value.
        /// </returns>
        private static object CoerceOnIsExpandedInternal(DependencyObject d, object baseValue)
        {
            TreeViewItemAdv item = d as TreeViewItemAdv;
            bool coerceValue = (bool)baseValue;

            if (item != null && item.ParentTreeView != null && item.Items.Count > 0
                && item.ParentTreeView.AnimationType != AnimationType.None)
            {
                if (item.m_bUpdateCoerce)
                {
                    item.m_bUpdateCoerce = false;
                }
                else if (item.m_bAnimating || item.IsExpanded != coerceValue)
                {
                    coerceValue = !coerceValue;
                }
            }

            return coerceValue;
        }

        /// <summary>
        /// This method corrects IsSelected.
        /// </summary>
        /// <param name="d">Object to which this property belongs.</param>
        /// <param name="baseValue">The property whish should be corrected.</param>
        /// <returns>
        /// Corrected value.
        /// </returns>
        private static object CoerceOnIsSelected(DependencyObject d, object baseValue)
        {
            TreeViewItemAdv item = d as TreeViewItemAdv;
            TreeViewAdv tree = item.ParentTreeView;
            bool coerceValue = (bool)baseValue;
            if (!item.IsReadOnly)
            {
                if (item != null && tree != null && item.CompleteHeaderElement != null
                    && tree.SelectedFalseItem != null && item.m_bUpdatingCoerce)
                {
                    item.CompleteHeaderElement.Opacity = 0.5d;
                }
                else if (item.CompleteHeaderElement != null)
                {
                    item.CompleteHeaderElement.Opacity = 1d;
                }

                return coerceValue;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This method corrects IsInEditMode.
        /// </summary>
        /// <param name="d">Object to which this property belongs.</param>
        /// <param name="baseValue">The property whish should be corrected.</param>
        /// <returns>
        /// Corrected value.
        /// </returns>
        private static object CoerceOnIsInEditMode(DependencyObject d, object baseValue)
        {
            TreeViewItemAdv item = d as TreeViewItemAdv;
            bool coerceValue = (bool)baseValue;

            if (item.IsEditable == false || !item.IsLoaded)
            {
                coerceValue = false;
            }

            return coerceValue;
        }

        /// <summary>
        /// Starts the animation.
        /// </summary>
        /// <param name="bExpand">if set to <c>true</c> [b expand].</param>
        private void StartAnimation(bool bExpand)
        {
            m_bAnimatinExpand = bExpand;
            UpdateAnimation(bExpand);

            if (bExpand)
            {
                BeginExpandAnimation();
            }
            else
            {
                BeginCollapseAnimation();
            }
        }

        /// <summary>
        /// Called when [Expanded changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        internal void OnIsExpandedChanged(DependencyPropertyChangedEventArgs e)
        {
            #region IVirtualTree

            if (ParentTreeView != null && ParentTreeView.IsVirtualizing && ParentTreeView.VirtualizationMode == VirtualizationMode.Extended && !ParentTreeView.isAlreadyExpanded && this.DataContext is IVirtualTree)
            {
                if (this.IsExpanded)
                {
                    if (((IVirtualTree)this.DataContext).ExtentHeight <= 0d)
                    {
                        if (((IVirtualTree)this.DataContext).ItemsCount == 0d)
                            ((IVirtualTree)this.DataContext).ItemsCount = this.Items.Count;
                        if (ParentTreeView.treeHeight == 0d)
                            ParentTreeView.treeHeight = 19.96d;
                        if (ParentTreeView.m_treeviewadvVirtualizingPanel != null)
                            ParentTreeView.m_treeviewadvVirtualizingPanel.m_extentSize.Height += (((IVirtualTree)this.DataContext).ItemsCount * ParentTreeView.treeHeight);
                        ParentTreeView.ExtentHeight += (((IVirtualTree)this.DataContext).ItemsCount * ParentTreeView.treeHeight);
                        ParentTreeView.expandedItemsCount++;

                        ((IVirtualTree)this.DataContext).ExtentHeight = (((IVirtualTree)this.DataContext).ItemsCount * ParentTreeView.treeHeight);
                        IVirtualTree parent = ((IVirtualTree)this.DataContext).Parent;
                        if (parent == null && ParentTreeViewItem != null && ParentTreeViewItem.DataContext != null)
                        {
                            ((IVirtualTree)this.DataContext).Parent = ParentTreeViewItem.DataContext as IVirtualTree;
                            parent = ((IVirtualTree)this.DataContext).Parent;
                        }
                        do
                        {
                            if (parent != null)
                            {
                                parent.ExtentHeight += (((IVirtualTree)this.DataContext).ItemsCount * ParentTreeView.treeHeight);
                                parent = ((IVirtualTree)parent).Parent;
                            }
                        } while (parent != null);

                        ((IVirtualTree)this.DataContext).IsExpanded = IsExpanded = true;
                        ParentTreeView.ScrollHost.InvalidateScrollInfo();
                    }
                }
                else
                {
                    if (ParentTreeView.m_treeviewadvVirtualizingPanel != null && (ParentTreeView.m_treeviewadvVirtualizingPanel.m_extentSize.Height >= (this.Items.Count * ParentTreeView.treeHeight)))
                        ParentTreeView.m_treeviewadvVirtualizingPanel.m_extentSize.Height -= (this.Items.Count * ParentTreeView.treeHeight);
                    if (ParentTreeView.ExtentHeight >= (this.Items.Count * ParentTreeView.treeHeight))
                        ParentTreeView.ExtentHeight -= (this.Items.Count * ParentTreeView.treeHeight);
                    ((IVirtualTree)this.DataContext).ExtentHeight = 0;
                    ParentTreeView.expandedItemsCount--;
                    IVirtualTree parent = ((IVirtualTree)this.DataContext).Parent;
                    do
                    {
                        if (parent != null)
                        {
                            parent.ExtentHeight -= (this.Items.Count * ParentTreeView.treeHeight);
                            parent = ((IVirtualTree)parent).Parent;
                        }
                    } while (parent != null);
                    ((IVirtualTree)this.DataContext).ItemsCount = 0;
                    ((IVirtualTree)this.DataContext).IsExpanded = IsExpanded = false;
                    ParentTreeView.ScrollHost.InvalidateScrollInfo();
                }
            }

            #endregion IVirtualTree

            if (this.ParentTreeView != null)
            {
                #region LoadOnDemand

                this.ParentTreeView.ExpandingTreeViewItem(this);

                #endregion LoadOnDemand

                if (this.ParentTreeViewItem == null)
                {
                    if (this.IsExpanded)
                    {
                        int indx = this.ParentTreeView.ItemContainerGenerator.IndexFromContainer(this);
                        if (indx != -1 && !this.ParentTreeView.ExpandedItemsIndexCollection.Contains(indx))
                        {
                            this.ParentTreeView.ExpandedItemsIndexCollection.Add(indx);
                            this.InvalidateMeasure();
                        }
                        else
                        {
                            indx = this.ParentTreeView.Items.IndexOf(this);
                            if (indx != -1 && !this.ParentTreeView.ExpandedItemsIndexCollection.Contains(indx))
                            {
                                this.ParentTreeView.ExpandedItemsIndexCollection.Add(indx);
                                this.InvalidateMeasure();
                            }
                        }

                        isExpandedAlready = true;
                    }
                    else
                    {
                        int indx = this.ParentTreeView.ItemContainerGenerator.IndexFromContainer(this);
                        if (indx != -1 && this.ParentTreeView.ExpandedItemsIndexCollection.Contains(indx))
                        {
                            this.ParentTreeView.ExpandedItemsIndexCollection.Remove(indx);
                        }
                        else
                        {
                            indx = this.ParentTreeView.Items.IndexOf(this);
                            if (indx != -1 && !this.ParentTreeView.ExpandedItemsIndexCollection.Contains(indx))
                            {
                                this.ParentTreeView.ExpandedItemsIndexCollection.Remove(indx);
                            }
                        }
                        if (ParentTreeView.IsVirtualizing && ParentTreeView.VirtualizationMode == VirtualizationMode.Extended)
                            ParentTreeView.m_treeviewadvVirtualizingPanel.ScrollInfo.SetVerticalOffset(this.ParentTreeView.m_treeviewadvVirtualizingPanel.ScrollInfo.VerticalOffset + 0.1);
                        isExpandedAlready = false;
                    }
                }
            }
            if (this.ParentTreeViewItem != null)
            {
                if (this.IsExpanded)
                {
                    int indx = this.ParentTreeViewItem.ItemContainerGenerator.IndexFromContainer(this);
                    if (indx != -1 && !this.ParentTreeViewItem.ExpandedItemsIndexCollection.Contains(indx))
                    {
                        this.ParentTreeViewItem.ExpandedItemsIndexCollection.Add(indx);
                        this.InvalidateMeasure();
                    }
                    else
                    {
                        indx = this.ParentTreeViewItem.Items.IndexOf(this);
                        if (indx != -1 && !this.ParentTreeViewItem.ExpandedItemsIndexCollection.Contains(indx))
                        {
                            this.ParentTreeViewItem.ExpandedItemsIndexCollection.Add(indx);
                            this.InvalidateMeasure();
                        }
                    }
                }
                else
                {
                    int indx = this.ParentTreeViewItem.ItemContainerGenerator.IndexFromContainer(this);
                    if (indx != -1 && this.ParentTreeViewItem.ExpandedItemsIndexCollection.Contains(indx))
                    {
                        this.ParentTreeViewItem.ExpandedItemsIndexCollection.Remove(indx);
                    }
                    else
                    {
                        indx = this.ParentTreeViewItem.Items.IndexOf(this);
                        if (indx != -1 && !this.ParentTreeViewItem.ExpandedItemsIndexCollection.Contains(indx))
                        {
                            this.ParentTreeViewItem.ExpandedItemsIndexCollection.Remove(indx);
                        }
                    }
                }
            }

            if (ParentTreeView != null && (ParentTreeView.VirtualizationMode == VirtualizationMode.Extended || ParentTreeView.VirtualizationMode == VirtualizationMode.Normal))
                TreeViewAdvVirtualizingPanel.IsHorizontalScroll = false;
        }

        /// <summary>
        /// Calls OnIsExpandedChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv collapsed = (TreeViewItemAdv)d;
            collapsed.OnIsExpandedChanged(e);

            if (collapsed.ParentTreeView != null && collapsed.ParentTreeView.IsColumnHeaderAutoWidthEnabled)
            {
                double calculatedwidth = 0;

                if (collapsed.LeftImageSource != null || collapsed.RightImageSource != null || collapsed.ExpandedImageSource != null || collapsed.CollapsedImageSource != null)
                {
                    calculatedwidth = collapsed.internalindex * collapsed.GetWidthUnheader(true);
                }
                else
                {
                    calculatedwidth = collapsed.internalindex * collapsed.GetWidthUnheader(false);
                }

                if (calculatedwidth != 0)
                {
                    if (calculatedwidth < collapsed.internalwidth)
                    {
                        calculatedwidth = calculatedwidth * 2;
                        if (calculatedwidth > collapsed.internalwidth)
                        {
                            collapsed.internalwidth = calculatedwidth;
                        }
                    }
                    if (collapsed.internalwidth < calculatedwidth)
                    {
                        collapsed.internalwidth = calculatedwidth;
                    }
                    if (collapsed.ParentTreeViewItem == null)
                    {
                        calculatedwidth = (collapsed.internalindex) * calculatedwidth * 2;
                        collapsed.internalwidth = calculatedwidth;
                    }

                    if (collapsed.ParentTreeView != null && collapsed.ParentTreeView.Columns != null)
                    {
                        for (int i = 0; i < collapsed.ParentTreeView.Columns.Count; i++)
                        {
                            PropertyInfo p;
                            if (collapsed.m_treeviewitemactualobject != null)
                            {
                                if (collapsed.ParentTreeView.Columns[i].DisplayMemberBinding != null)
                                    p =
                                        collapsed.m_treeviewitemactualobject.GetType().GetProperty(
                                            ((Binding)collapsed.ParentTreeView.Columns[i].DisplayMemberBinding).Path.Path.
                                                ToString());
                                else
                                    p =
                                   collapsed.m_treeviewitemactualobject.GetType().GetProperty(collapsed.ParentTreeView.Columns[i].SortBy);
                                if (p != null)
                                {
                                    if (p.GetValue(collapsed.m_treeviewitemactualobject, null) != null)
                                    {
                                        collapsed.ParentTreeView.Columns[i].Width = new GridLength(
                                            Convert.ToDouble(
                                                p.GetValue(collapsed.m_treeviewitemactualobject, null).ToString().Length *
                                                5 +
                                                calculatedwidth));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (collapsed.IsExpanded && collapsed.ParentTreeView != null && collapsed.ParentTreeView.IsVirtualizing)
            {
                if (!collapsed.ParentTreeView.ExpandStateLinearList.Contains(collapsed.m_treeviewitemactualobject))
                {
                    collapsed.ParentTreeView.ExpandStateLinearList.Add(collapsed.m_treeviewitemactualobject);
                }
            }
            else if (!collapsed.IsExpanded && collapsed.ParentTreeView != null && collapsed.ParentTreeView.IsVirtualizing)
            {
                if (collapsed.ParentTreeView.ExpandStateLinearList.Contains(collapsed.m_treeviewitemactualobject))
                {
                    collapsed.ParentTreeView.ExpandStateLinearList.Remove(collapsed.m_treeviewitemactualobject);
                }
            }

            if (collapsed.IsExpanded && collapsed.ParentTreeView != null && !collapsed.ParentTreeView.IsVirtualizing)
            {
                if (!collapsed.ParentTreeView.AlreadyExpandedLinearList.Contains(collapsed.m_treeviewitemactualobject))
                {
                    collapsed.ParentTreeView.AlreadyExpandedLinearList.Add(collapsed.m_treeviewitemactualobject);
                }
            }
            else if (!collapsed.IsExpanded && collapsed.ParentTreeView != null && !collapsed.ParentTreeView.IsVirtualizing)
            {
                if (collapsed.ParentTreeView.AlreadyExpandedLinearList.Contains(collapsed.m_treeviewitemactualobject))
                {
                    collapsed.ParentTreeView.AlreadyExpandedLinearList.Remove(collapsed.m_treeviewitemactualobject);
                }
            }

            if (collapsed.IsExpanded == true && collapsed.ParentTreeView != null)
            {
                ExpandingCollapsingEventArgs args = new ExpandingCollapsingEventArgs(TreeViewAdv.ExpandingEvent, collapsed);


                if (!args.Cancel)
                {
                    if (!expandchangeinternally)
                    {
                        collapsed.ParentTreeView.bringintoviewstatus = false;
                    }
                    if (collapsed.ParentTreeView.bringintoviewstatus)
                    {
                        collapsed.ParentTreeView.m_expandeditem = null;
                    }
                    else
                    {
                        collapsed.ParentTreeView.m_expandeditem = collapsed;
                    }
                }
                else
                {
                    collapsed.IsExpanded = !collapsed.IsExpanded;
                }
            }
            else if (!collapsed.IsExpanded && collapsed.ParentTreeView != null)
            {
                ExpandingCollapsingEventArgs args = new ExpandingCollapsingEventArgs(TreeViewAdv.CollapsingEvent, collapsed);


                if (!args.Cancel)
                {
                    collapsed.ParentTreeView.m_expandeditem = null;
                }
                else
                {
                    collapsed.IsExpanded = !collapsed.IsExpanded;
                }
            }

            if (collapsed.ParentTreeView != null)
            {
                if (collapsed.ParentTreeView.EnabledRecursiveSorting && collapsed.IsExpanded)
                {
                    collapsed.ParentTreeView.SortingTreeView((DependencyObject)collapsed, collapsed.ParentTreeView.newvalue, collapsed.ParentTreeView.oldvalue);
                    collapsed.ParentTreeView.IterateItems(collapsed, collapsed.ParentTreeView, collapsed.ParentTreeView.newvalue, collapsed.ParentTreeView.oldvalue);
                }
            }

            if (collapsed.IsExpanded && collapsed.ParentTreeView != null)
            {
                ExpandedCollapsedEventArgs args = new ExpandedCollapsedEventArgs(TreeViewAdv.ExpandedEvent, collapsed);

                collapsed.ParentTreeView.AfterExpand(args);

                collapsed.ParentTreeView.m_itemsexpanded = true;

                if (collapsed.ParentTreeViewItem == null)
                {
                    int count = 0;
                    TreeViewItemAdv removabletree = null;
                    foreach (TreeViewItemAdv tree in collapsed.ParentTreeView.ExpandedItems)
                    {
                        if (collapsed.ParentTreeView.ItemContainerGenerator.IndexFromContainer(tree) == -1 && collapsed.ParentTreeView.Items.IndexOf(tree) == -1)
                        {
                            removabletree = tree;
                        }
                        if (collapsed.m_treeviewitemactualobject != null && tree.m_treeviewitemactualobject != null)
                        {
                            if (collapsed.m_treeviewitemactualobject.Equals(tree.m_treeviewitemactualobject))
                            {
                                count++;
                            }
                        }
                    }
                    if (removabletree != null)
                    {
                        int indx = collapsed.ParentTreeView.ItemContainerGenerator.IndexFromContainer(removabletree);
                        if (indx != -1 && collapsed.ParentTreeView.ExpandedTreeViewAdvItems.ContainsKey(indx))
                        {
                            collapsed.ParentTreeView.ExpandedTreeViewAdvItems.Remove(indx);
                            collapsed.ParentTreeView.ExpandedItems.Remove(removabletree);
                        }
                        else
                        {
                            indx = collapsed.ParentTreeView.Items.IndexOf(removabletree);
                            if (indx != -1 && collapsed.ParentTreeView.ExpandedTreeViewAdvItems.ContainsKey(indx))
                            {
                                collapsed.ParentTreeView.ExpandedTreeViewAdvItems.Remove(indx);
                                collapsed.ParentTreeView.ExpandedItems.Remove(removabletree);
                            }
                        }
                    }
                    if (count == 0)
                    {
                        int indx = collapsed.ParentTreeView.ItemContainerGenerator.IndexFromContainer(collapsed);
                        if (indx != -1 && !collapsed.ParentTreeView.ExpandedTreeViewAdvItems.ContainsKey(indx))
                        {
                            collapsed.ParentTreeView.ExpandedTreeViewAdvItems.Add(indx, collapsed);
                            collapsed.ParentTreeView.ExpandedItems.Add(collapsed);
                        }
                        else
                        {
                            indx = collapsed.ParentTreeView.Items.IndexOf(collapsed);
                            if (indx != -1 && !collapsed.ParentTreeView.ExpandedTreeViewAdvItems.ContainsKey(indx))
                            {
                                collapsed.ParentTreeView.ExpandedTreeViewAdvItems.Add(indx, collapsed);
                                collapsed.ParentTreeView.ExpandedItems.Add(collapsed);
                            }
                        }
                    }
                }
                else
                {
                    int count = 0;
                    TreeViewItemAdv removabletree = null;
                    foreach (TreeViewItemAdv tree in collapsed.ParentTreeViewItem.ExpandedItems)
                    {
                        if (collapsed.ParentTreeViewItem.ItemContainerGenerator.IndexFromContainer(tree) == -1)
                        {
                            removabletree = tree;
                        }
                        if (collapsed.m_treeviewitemactualobject != null && tree.m_treeviewitemactualobject != null)
                        {
                            if (collapsed.m_treeviewitemactualobject.Equals(tree.m_treeviewitemactualobject))
                            {
                                count++;
                            }
                        }
                    }
                    if (removabletree != null)
                    {
                        int indx = collapsed.ParentTreeViewItem.ItemContainerGenerator.IndexFromContainer(removabletree);
                        if (indx != -1 && collapsed.ParentTreeViewItem.ExpandedTreeViewAdvItems.ContainsKey(indx))
                        {
                            collapsed.ParentTreeViewItem.ExpandedTreeViewAdvItems.Remove(indx);
                            collapsed.ParentTreeViewItem.ExpandedItems.Remove(removabletree);
                        }
                        else
                        {
                            indx = collapsed.ParentTreeViewItem.Items.IndexOf(removabletree);
                            if (indx != -1 && collapsed.ParentTreeViewItem.ExpandedTreeViewAdvItems.ContainsKey(indx))
                            {
                                collapsed.ParentTreeViewItem.ExpandedTreeViewAdvItems.Remove(indx);
                                collapsed.ParentTreeViewItem.ExpandedItems.Remove(removabletree);
                            }
                        }
                    }
                    if (count == 0)
                    {
                        int indx = collapsed.ParentTreeViewItem.ItemContainerGenerator.IndexFromContainer(collapsed);
                        if (indx != -1 && !collapsed.ParentTreeViewItem.ExpandedTreeViewAdvItems.ContainsKey(indx))
                        {
                            collapsed.ParentTreeViewItem.ExpandedTreeViewAdvItems.Add(indx, collapsed);
                            collapsed.ParentTreeViewItem.ExpandedItems.Add(collapsed);
                        }
                        else
                        {
                            indx = collapsed.ParentTreeViewItem.Items.IndexOf(collapsed);
                            if (indx != -1 && !collapsed.ParentTreeViewItem.ExpandedTreeViewAdvItems.ContainsKey(indx))
                            {
                                collapsed.ParentTreeViewItem.ExpandedTreeViewAdvItems.Add(indx, collapsed);
                                collapsed.ParentTreeViewItem.ExpandedItems.Add(collapsed);
                            }
                        }
                    }
                }
            }
            else if (!collapsed.IsExpanded && collapsed.ParentTreeView != null)
            {
                ExpandedCollapsedEventArgs args = new ExpandedCollapsedEventArgs(TreeViewAdv.CollapsedEvent, collapsed);

                collapsed.ParentTreeView.AfterCollapse(args);

                collapsed.ParentTreeView.m_itemsexpanded = false;

                TreeViewItemAdv removabletree = null;
                if (collapsed.ParentTreeViewItem == null)
                {
                    foreach (TreeViewItemAdv tree in collapsed.ParentTreeView.ExpandedItems)
                    {
                        if (collapsed.m_treeviewitemactualobject != null && tree.m_treeviewitemactualobject != null)
                        {
                            if (tree.m_treeviewitemactualobject.Equals(collapsed.m_treeviewitemactualobject))
                            {
                                removabletree = tree;
                            }
                        }
                    }
                    if (removabletree != null)
                    {
                        int indx = collapsed.ParentTreeView.ItemContainerGenerator.IndexFromContainer(removabletree);
                        collapsed.ParentTreeView.ExpandedTreeViewAdvItems.Remove(indx);
                        collapsed.ParentTreeView.ExpandedItems.Remove(removabletree);
                    }
                }
                else
                {
                    foreach (TreeViewItemAdv tree in collapsed.ParentTreeViewItem.ExpandedItems)
                    {
                        if (collapsed.m_treeviewitemactualobject != null && tree.m_treeviewitemactualobject != null)
                        {
                            if (tree.m_treeviewitemactualobject.Equals(collapsed.m_treeviewitemactualobject))
                            {
                                removabletree = tree;
                            }
                        }
                    }
                    if (removabletree != null)
                    {
                        int indx = collapsed.ParentTreeViewItem.ItemContainerGenerator.IndexFromContainer(removabletree);
                        collapsed.ParentTreeViewItem.ExpandedTreeViewAdvItems.Remove(indx);
                        collapsed.ParentTreeViewItem.ExpandedItems.Remove(removabletree);
                    }
                }
            }
        }

        public bool IsImageSourceFreeze { get; set; }

        /// <summary>
        /// Calls OnIsExpandedChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnIsExpandedChangedInternal(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv collapsed = (TreeViewItemAdv)d;
            ExpandingCollapsingEventArgs expandargs = new ExpandingCollapsingEventArgs(TreeViewAdv.ExpandingEvent, collapsed);
            ExpandingCollapsingEventArgs collapseargs = new ExpandingCollapsingEventArgs(TreeViewAdv.CollapsingEvent, collapsed);
            if (collapsed.IsExpanded == true && collapsed.ParentTreeView != null)
            {

                if (!expandargs.Cancel)
                {
                    if (!expandchangeinternally)
                    {
                        collapsed.ParentTreeView.bringintoviewstatus = false;
                    }
                    if (collapsed.ParentTreeView.bringintoviewstatus)
                    {
                        collapsed.ParentTreeView.m_expandeditem = null;
                    }
                    else
                    {
                        collapsed.ParentTreeView.m_expandeditem = collapsed;
                    }
                    expandchangeinternally = false;
                }
                else
                {
                    collapsed.IsExpanded = !collapsed.IsExpanded;
                }
            }
            else if (!collapsed.IsExpanded && collapsed.ParentTreeView != null)
            {

                if (!collapseargs.Cancel)
                {
                    collapsed.ParentTreeView.m_expandeditem = null;
                }
                else
                {
                    collapsed.IsExpanded = !collapsed.IsExpanded;
                }
            }
            bool newValue = (bool)e.NewValue;

            if (!newValue && !collapsed.IsExpanded)
            {
                if (!collapseargs.Cancel && collapsed.ParentTreeView != null)
                {
                    TreeViewAdv parentFxTreeView = collapsed.ParentTreeView;

                    if (parentFxTreeView != null)
                    {
                        parentFxTreeView.HandleSelectionAndCollapsed(collapsed);
                    }
                }
            }

            collapsed.RaiseCollaseExpandInternal();

            if (collapsed.ParentTreeView != null)
            {
                if (collapsed.ParentTreeView.EnabledRecursiveSorting && collapsed.IsExpanded)
                {
                    collapsed.ParentTreeView.IterateItems(collapsed, collapsed.ParentTreeView, collapsed.ParentTreeView.newvalue, collapsed.ParentTreeView.oldvalue);
                }
            }

            if (collapsed.IsExpanded && collapsed.ParentTreeView != null)
            {
                ExpandedCollapsedEventArgs args = new ExpandedCollapsedEventArgs(TreeViewAdv.ExpandedEvent, collapsed);

            }
            else if (!collapsed.IsExpanded && collapsed.ParentTreeView != null)
            {
                ExpandedCollapsedEventArgs args = new ExpandedCollapsedEventArgs(TreeViewAdv.CollapsedEvent, collapsed);

            }
        }

        /// <summary>
        /// Raises the collapse expand internal.
        /// </summary>
        private void RaiseCollaseExpandInternal()
        {
            bool bIsExpanded = IsExpanded;
            TreeViewItemAdvAutomationPeer peer = UIElementAutomationPeer.FromElement(this) as TreeViewItemAdvAutomationPeer;

            if (peer != null)
            {
                peer.RaiseExpandCollapseAutomationEvent(!bIsExpanded, bIsExpanded);
            }

            if (bIsExpanded)
            {
                OnExpanded(new RoutedEventArgs(ExpandedEvent, this));
            }
            else
            {
                OnCollapsed(new RoutedEventArgs(CollapsedEvent, this));
            }
        }

        /// <summary>
        /// Selects item and saves it to TreeViewAdv visible items.
        /// </summary>
        /// <param name="item">item to select</param>
        private static void SelectAndSave(TreeViewItemAdv item)
        {
            if (item != null && item.ParentTreeView != null)
            {
                item.Select(true);
                item.ParentTreeView.FirstVisibleItem = item;
                item.ParentTreeView.LastVisibleItem = item;
            }
        }

        internal bool isMakeVisibleCalled = false;

        /// <summary>
        /// Called when [request bring into view].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RequestBringIntoViewEventArgs"/> instance containing the event data.</param>
        private static void OnRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            if (e.TargetObject == sender)
            {
                if (sender is TreeViewItemAdv)
                {
                    if (((TreeViewItemAdv)sender).isMakeVisibleCalled)
                    {
                        ((TreeViewItemAdv)sender).HandleBringIntoView(e);
                    }
                }
            }
        }

        /// <summary>
        /// Allows the handle key event.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <returns>bool value type</returns>
        private bool AllowHandleKeyEvent(FocusNavigationDirection direction)
        {
            if (!this.IsSelected)
            {
                return false;
            }

            UIElement element = Keyboard.FocusedElement as UIElement;

            if (element != null)
            {
                Visual reference = element.PredictFocus(direction) as Visual;

                if (reference != element)
                {
                    while (reference != null)
                    {
                        TreeViewItemAdv item = reference as TreeViewItemAdv;

                        if (item == this)
                        {
                            return false;
                        }

                        if ((item != null) || (reference is TreeViewAdv))
                        {
                            return true;
                        }

                        reference = VisualTreeHelper.GetParent(reference) as Visual;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Handles the bring into view.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.RequestBringIntoViewEventArgs"/> instance containing the event data.</param>
        private void HandleBringIntoView(RequestBringIntoViewEventArgs e)
        {
            expandchangeinternally = false;
            for (TreeViewItemAdv parentTreeViewItem = this.ParentTreeViewItem; parentTreeViewItem != null; parentTreeViewItem = parentTreeViewItem.ParentTreeViewItem)
            {
                if (!parentTreeViewItem.IsExpanded)
                {
                    expandchangeinternally = true;
                    parentTreeViewItem.IsExpanded = true;
                }
            }

            if (this.ParentItemsControl != null && this.ParentItemsControl.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                this.LayoutUpdated += new EventHandler(bringIntoViewItem_LayoutUpdated);
            }
            else if (this.ParentItemsControl != null)
            {
                this.ParentItemsControl.ItemContainerGenerator.StatusChanged += new EventHandler(ItemContainerGenerator_StatusChanged);
            }

            if (e.TargetRect.IsEmpty)
            {
                FrameworkElement headerElement = this.HeaderElement;
                if (headerElement == null)
                {
                    m_headerElement = this.Template.FindName(C_nameHeaderPart, this) as FrameworkElement;
                    headerElement = this.HeaderElement;
                }
                if (headerElement != null)
                {
                    e.Handled = true;
                    headerElement.BringIntoView();
                }
            }
        }

        /// <summary>
        /// Handles the StatusChanged event of the ItemContainerGenerator control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            this.LayoutUpdated += new EventHandler(bringIntoViewItem_LayoutUpdated);

            if (ParentItemsControl != null)
            {
                this.ParentItemsControl.ItemContainerGenerator.StatusChanged -= new EventHandler(ItemContainerGenerator_StatusChanged);
            }
        }

        /// <summary>
        /// Handles the LayoutUpdated event of the bringIntoViewItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void bringIntoViewItem_LayoutUpdated(object sender, EventArgs e)
        {
            if (isMakeVisibleCalled)
            {
                this.BringIntoView(new Rect(0, 0, this.DesiredSize.Width, this.DesiredSize.Height));
            }

            this.LayoutUpdated -= new EventHandler(bringIntoViewItem_LayoutUpdated);
        }

        /// <summary>
        /// Clears selected items when no key modifiers were pressed.
        /// </summary>
        private void ClearSelectedItems()
        {
            if (Keyboard.Modifiers == ModifierKeys.None && ParentTreeView.SelectedItems.Count > 0)
            {
                TreeViewItemAdv item = ParentTreeView.SelectedItems[ParentTreeView.SelectedItems.Count - 1] as TreeViewItemAdv;
                ParentTreeView.SelectedTreeViewItems.Clear();
                ParentTreeView.SelectedItems.Clear();
                FocusManager.SetFocusedElement(ParentTreeView, item);
            }
        }

        /// <summary>
        /// Is used to handle up key when scroll bar is visible.
        /// </summary>
        private void UpKeyHandle()
        {
            ScrollViewer scrollViewer = ParentTreeView.ScrollHost;

            if (scrollViewer.ScrollableHeight > 0)
            {
                Point scrollViewerPoint = scrollViewer.PointToScreen(new Point(0, 0));
                TreeViewItemAdv selectedItem = GetSelectedItem();

                if (selectedItem != null)
                {
                    Point point = selectedItem.PointToScreen(new Point(0, 0));
                    Rect rect = new Rect(scrollViewerPoint, new Size(scrollViewer.ActualWidth, (m_completeHeaderElement.ActualHeight * 2)));

                    if (rect.Contains(point))
                    {
                        scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - m_completeHeaderElement.ActualHeight);
                    }
                }
            }
        }

        /// <summary>
        /// Is used to handle down key when scroll bar is visible.
        /// </summary>
        private void DownKeyHandle()
        {
            try
            {
                ScrollViewer scrollViewer = ParentTreeView.ScrollHost;

                if (scrollViewer.ScrollableHeight > 0)
                {
                    Point scrollViewerPoint = scrollViewer.PointToScreen(new Point(0, 0));
                    TreeViewItemAdv selectedItem = GetSelectedItem();

                    if (selectedItem != null)
                    {
                        Point point = selectedItem.PointToScreen(new Point(0, 0));

                        int offset = 40;
                        Rect rect = new Rect(scrollViewerPoint, new Size(scrollViewer.ViewportWidth - offset, scrollViewer.ViewportHeight - offset));

                        if (!rect.Contains(new Point(Math.Abs(scrollViewerPoint.X), Math.Abs(point.Y))))
                        {
                            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + m_completeHeaderElement.ActualHeight);
                        }
                    }
                }
            }
            catch
            { }
        }

        /// <summary>
        /// Gets last selected item from TreeViewAdv.
        /// </summary>
        /// <returns>selected item.</returns>
        private TreeViewItemAdv GetSelectedItem()
        {
            TreeViewItemAdv selectedItem = ParentTreeView.SelectedItem as TreeViewItemAdv;

            if (selectedItem == null)
            {
                if (ParentTreeViewItem != null)
                {
                    selectedItem = ParentTreeViewItem.ItemContainerGenerator.ContainerFromItem(ParentTreeView.SelectedItem) as TreeViewItemAdv;
                }
                else if (ParentTreeView != null)
                {
                    selectedItem = ParentTreeView.ItemContainerGenerator.ContainerFromItem(ParentTreeView.SelectedItem) as TreeViewItemAdv;
                }
            }

            if (selectedItem == null)
            {
                TreeObjectCollection selectedItems = ParentTreeView.SelectedItems;

                if (selectedItems.Count > 0)
                {
                    selectedItem = selectedItems[selectedItems.Count - 1] as TreeViewItemAdv;
                }
            }

            return selectedItem;
        }

        /// <summary>
        /// Scrolls up TreeViewAdv page.
        /// </summary>
        /// <param name="scrollViewer">scrollViewer object</param>
        /// <param name="e">KeyEvent Args</param>
        private void ScrollUpPage(ScrollViewer scrollViewer, KeyEventArgs e)
        {
            double scrollableHeight = scrollViewer.ScrollableHeight;
            double verticalOffset = scrollViewer.VerticalOffset;
            TreeViewItemAdv firstVisibleItem = ParentTreeView.FirstVisibleItem;
            TreeViewItemAdv lastVisibleItem = ParentTreeView.LastVisibleItem;

            if (scrollableHeight > 0)
            {
                if (verticalOffset == 0)
                {
                    ParentTreeView.ScrollToHomeHandle(e);
                }
                else
                {
                    bool selectedByUser = lastVisibleItem == firstVisibleItem && firstVisibleItem != null;
                    if (firstVisibleItem != null || selectedByUser)
                    {
                        double upOffset = verticalOffset - scrollViewer.ViewportHeight + CompleteHeaderElement.ActualHeight * 2;
                        scrollViewer.ScrollToVerticalOffset(upOffset);
                    }

                    HandleScrollUp(scrollViewer);
                }

                ParentTreeView.LastVisibleItem = null;
            }
            else
            {
                ParentTreeView.ScrollToHomeHandle(e);
            }
        }

        /// <summary>
        /// Scrolls down TreeViewAdv page.
        /// </summary>
        /// <param name="scrollViewer">scrollViewer object</param>
        /// <param name="e">KeyEvent Args</param>
        private void ScrollDownPage(ScrollViewer scrollViewer, KeyEventArgs e)
        {
            double scrollableHeight = scrollViewer.ScrollableHeight;
            double verticalOffset = scrollViewer.VerticalOffset;
            TreeViewItemAdv firstVisibleItem = ParentTreeView.FirstVisibleItem;
            TreeViewItemAdv lastVisibleItem = ParentTreeView.LastVisibleItem;

            if (scrollableHeight > 0)
            {
                if (verticalOffset == scrollableHeight)
                {
                    ParentTreeView.ScrollToEndHandle(e);
                }
                else
                {
                    bool selectedByUser = lastVisibleItem == firstVisibleItem && lastVisibleItem != null;
                    if (lastVisibleItem != null || selectedByUser)
                    {
                        double downOffset = verticalOffset + scrollViewer.ViewportHeight - CompleteHeaderElement.ActualHeight * 2;
                        scrollViewer.ScrollToVerticalOffset(downOffset);
                    }

                    HandleScrollDown(scrollViewer);
                }

                ParentTreeView.FirstVisibleItem = null;
            }
            else
            {
                ParentTreeView.ScrollToEndHandle(e);
            }
        }

        /// <summary>
        /// Looks through items tree and finds last visible item.
        /// </summary>
        /// <param name="scrollViewer">ScrollViewer scrollViewer</param>
        private void HandleScrollDown(ScrollViewer scrollViewer)
        {
            Rect rect = GetScrollViewerRect(scrollViewer);
            TreeViewItemAdv preLastItem = null;

            for (int i = ParentTreeView.Items.Count - 1; i >= 0; i--)
            {
                m_itemsStack.Push(ParentTreeView.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv);
            }

            do
            {
                TreeViewItemAdv node = m_itemsStack.Pop();
                if (node != null)
                {
                    if (scrollViewer.IsAncestorOf(node))
                    {
                        Point point = node.PointToScreen(new Point(0, 0));
                        if (rect.Contains(point))
                        {
                            preLastItem = ParentTreeView.LastVisibleItem;
                            ParentTreeView.LastVisibleItem = node;
                        }
                    }

                    if (node.IsExpanded)
                    {
                        for (int i = node.Items.Count - 1; i >= 0; i--)
                        {
                            PushInnerItemToStack(node, i);
                        }
                    }
                }
            }
            while (m_itemsStack.Count != 0);

            if (preLastItem != null)
            {
                ParentTreeView.m_ismouseSelection = true;
                ParentTreeView.LastVisibleItem.Select(true);
                ParentTreeView.SelectedContainer.Focus();
                ParentTreeView.m_ismouseSelection = false;
                ParentTreeView.LastVisibleItem = preLastItem;
            }
        }

        /// <summary>
        /// Looks through items tree and finds first visible item.
        /// </summary>
        /// <param name="scrollViewer">ScrollViewer scrollViewer</param>
        private void HandleScrollUp(ScrollViewer scrollViewer)
        {
            Rect rect = GetScrollViewerRect(scrollViewer);

            for (int i = 0, cnt = ParentTreeView.Items.Count; i < cnt; i++)
            {
                m_itemsStack.Push(ParentTreeView.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv);
            }

            do
            {
                TreeViewItemAdv node = m_itemsStack.Pop();

                if (node != null)
                {
                    if (scrollViewer.IsAncestorOf(node))
                    {
                        Point point = node.PointToScreen(new Point(0, 0));
                        if (rect.Contains(point))
                        {
                            ParentTreeView.FirstVisibleItem = node;
                        }
                    }

                    if (node.IsExpanded)
                    {
                        for (int i = 0, cnt = node.Items.Count; i < cnt; i++)
                        {
                            PushInnerItemToStack(node, i);
                        }
                    }
                }
            }
            while (m_itemsStack.Count != 0);

            if (ParentTreeView.FirstVisibleItem != null)
            {
                ParentTreeView.m_ismouseSelection = true;
                ParentTreeView.FirstVisibleItem.Select(true);
                ParentTreeView.SelectedContainer.Focus();
                ParentTreeView.m_ismouseSelection = false;
            }
        }

        /// <summary>
        /// Handles the enter key.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void HandleEnterKey(KeyEventArgs e)
        {
            if (e != null && IsInEditMode)
            {
                TextBox textBox = e.OriginalSource as TextBox;
                IsInEditMode = false;
                ParentTreeView.SortTreeView();
                e.Handled = true;
            }
        }

        private static bool _wasEscKeypreseed = false;

        /// <summary>
        /// Handles the escape key.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void HandleEscapeKey(KeyEventArgs e)
        {
            if (e != null)
            {
                TextBox textBox = e.OriginalSource as TextBox;

                if (textBox != null && IsInEditMode)
                {
                    textBox.Clear();

                    _wasEscKeypreseed = true;
                    IsInEditMode = false;
                }

                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles the left key.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void HandleLeftKey(KeyEventArgs e)
        {
            if (e != null)
            {
                if (IsExpanded)
                {
                    ParentTreeView.count++;

                    if (ParentTreeView.count == 1)
                        IsExpanded = false;
                    else
                    {
                        e.Handled = false;
                    }
                }
                else if (ParentTreeViewItem != null)
                {
                    ParentTreeViewItem.Focus();
                }
                e.Handled = false;
            }
        }

        /// <summary>
        /// Handles the right key.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void HandleRightKey(KeyEventArgs e)
        {
            if (e != null && !IsControlKeyDown && CanExpandOnInput)
            {
                if (!IsExpanded)
                {
                    IsExpanded = true;
                    //e.Handled = true;
                }
                else if (HandleDownKey())
                {
                    DownKeyHandle();
                    //e.Handled = true;
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles the add key.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void HandleAddKey(KeyEventArgs e)
        {
            if (e != null && CanExpandOnInput && !IsExpanded)
            {
                IsExpanded = true;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles the subtract key.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void HandleSubtractKey(KeyEventArgs e)
        {
            if (e != null && CanExpandOnInput && IsExpanded)
            {
                IsExpanded = false;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles the prior key.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void HandlePriorKey(KeyEventArgs e)
        {
            if (e != null && ParentTreeView != null && ParentTreeView.ScrollHost != null)
            {
                ScrollUpPage(ParentTreeView.ScrollHost, e);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles the home key.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void HandleHomeKey(KeyEventArgs e)
        {
            if (e != null && ParentTreeView != null && ParentTreeView.ScrollHost != null)
            {
                ParentTreeView.ScrollToHomeHandle(e);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles the end key.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void HandleEndKey(KeyEventArgs e)
        {
            if (e != null && ParentTreeView != null && ParentTreeView.ScrollHost != null)
            {
                ParentTreeView.ScrollToEndHandle(e);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles the next key.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void HandleNextKey(KeyEventArgs e)
        {
            if (e != null && ParentTreeView != null && ParentTreeView.ScrollHost != null)
            {
                ScrollDownPage(ParentTreeView.ScrollHost, e);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles the space key.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void HandleSpaceKey(KeyEventArgs e)
        {
            if (e != null && Keyboard.Modifiers == ModifierKeys.None)
            {
                if (((ParentTreeView != null) && (ParentItemsControl != null))
                && !ParentTreeView.IsSelectionChangeActive)
                {
                    object data = ParentItemsControl.ItemContainerGenerator.ItemFromContainer(this);
                    ParentTreeView.ChangeSelection(data, this, !IsSelected, true);
                }

                e.Handled = true;
            }
            else if (Keyboard.Modifiers != ModifierKeys.None)
            {
                e.Handled = false;
            }
        }

        /// <summary>
        /// Gets ScrollViewer TreeRootLine.
        /// </summary>
        /// <param name="scrollViewer">ScrollViewer object</param>
        /// <returns>TreeRootLine of the ScrollViewer</returns>
        private Rect GetScrollViewerRect(ScrollViewer scrollViewer)
        {
            if (ParentTreeView != null)
            {
                // ParentTreeView.UpdateLayout();
                ParentTreeView.ApplyTemplate();
            }
            Point scrollViewerPoint = scrollViewer.PointToScreen(new Point(0, 0));
            Rect rect = new Rect(scrollViewerPoint, new Size(scrollViewer.ViewportWidth, scrollViewer.ViewportHeight));

            return rect;
        }

        /// <summary>
        /// Puts inner item of a node to stack.
        /// </summary>
        /// <param name="node">TreeViewItemAdv to put</param>
        /// <param name="i">index of the item in TreeViewAdv parent</param>
        private void PushInnerItemToStack(TreeViewItemAdv node, int i)
        {
            TreeViewItemAdv innerItem = node.Items[i] as TreeViewItemAdv;

            if (innerItem != null)
            {
                m_itemsStack.Push(innerItem);
            }
        }

        /// <summary>
        /// DeleteTabCommand command handler.
        /// </summary>
        /// <param name="sender">Object value .</param>
        /// <param name="e">The instance containing the event data.</param>
        private void EditExecute(object sender, ExecutedRoutedEventArgs e)
        {
            TreeViewItemAdv selectedItem = ItemContainerGenerator.ContainerFromItem(ParentTreeView.SelectedItem) as TreeViewItemAdv;

            if (selectedItem != null && !IsSelected)
            {
                selectedItem.IsInEditMode = !selectedItem.IsInEditMode;

                if (selectedItem.ParentTreeView != null)
                {
                    selectedItem.ParentTreeView.EditingItem = selectedItem;
                }
            }
            else
            {
                IsInEditMode = !IsInEditMode;

                if (ParentTreeView != null)
                {
                    ParentTreeView.EditingItem = this;
                }
            }
        }

        /// <summary>
        /// Determines whether DeleteTabCommand can execute in its current state.
        /// </summary>
        /// <param name="sender">Object value.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void EditCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;

            if (IsSelected && IsEditable && !IsInEditMode && !IsReadOnly)
            {
                e.CanExecute = true;
            }
        }

        internal void ReleaseMemoryForOldItem()
        {
            Loaded -= new RoutedEventHandler(TreeViewItemAdv_Loaded);
            if (m_expandAnimation != null)
                m_expandAnimation.Completed += new EventHandler(Animation_Completed);
        }

        /// <summary>
        /// Initializes control in the given template.
        /// </summary>
        /// <param name="template">Template to initialize control in.</param>
        internal void Initialize(FrameworkTemplate template)
        {
            m_expander = template.FindName(C_nameExpander, this) as Expander;

            if (m_expander != null)
            {
                m_expander.Height = double.NaN;
            }

            m_headerElement = template.FindName(C_nameHeaderPart, this) as FrameworkElement;
            m_completeHeaderElement = template.FindName(C_nameCompleteHeaderPart, this) as FrameworkElement;

            m_editHeaderElement = template.FindName(C_nameEditHeaderPart, this) as FrameworkElement;
            m_itemsHost = template.FindName(C_nameItemsHost, this) as ItemsPresenter;
            m_verticalLinePartOne = template.FindName("PART_VerticalLinePartOne", this) as TreeRootLine;
            m_verticalLinePartTwo = template.FindName("PART_VerticalLinePartTwo", this) as TreeRootLine;
            if (m_verticalLinePartTwo != null)
            {
                if (this.ParentTreeViewItem != null)
                {
                    int index = this.ParentTreeViewItem.Items.IndexOf(this);
                    if (index != -1 && index < this.ParentTreeViewItem.Items.Count - 1)
                    {
                        FrameworkElement treeitem = this.ParentTreeViewItem.Items[index + 1] as FrameworkElement;
                        if (treeitem != null && treeitem.Visibility == Visibility.Collapsed)
                        {
                            m_verticalLinePartTwo.Visibility = Visibility.Collapsed;
                        }

                        for (int i = index; i < ParentTreeViewItem.Items.Count - 1; i++)
                        {
                            treeitem = this.ParentTreeViewItem.Items[i + 1] as FrameworkElement;
                            if (treeitem != null && treeitem.Visibility == Visibility.Collapsed)
                            {
                                m_verticalLinePartTwo.Visibility = Visibility.Collapsed;
                            }
                            else
                            {
                                m_verticalLinePartTwo.Visibility = Visibility.Visible;
                            }
                        }
                    }
                    else if (index == this.ParentTreeViewItem.Items.Count - 1)
                    {
                        m_verticalLinePartTwo.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    int index = this.ParentTreeView.Items.IndexOf(this);
                    if (index != -1 && index < this.ParentTreeView.Items.Count - 1)
                    {
                        FrameworkElement treeitem = this.ParentTreeView.Items[index + 1] as FrameworkElement;
                        if (treeitem != null && treeitem.Visibility == Visibility.Collapsed)
                        {
                            m_verticalLinePartTwo.Visibility = Visibility.Collapsed;
                        }
                        for (int i = index; i < ParentTreeView.Items.Count - 1; i++)
                        {
                            treeitem = this.ParentTreeView.Items[i + 1] as FrameworkElement;
                            if (treeitem != null && treeitem.Visibility == Visibility.Collapsed)
                            {
                                m_verticalLinePartTwo.Visibility = Visibility.Collapsed;
                            }
                            else
                            {
                                m_verticalLinePartTwo.Visibility = Visibility.Visible;
                            }
                        }
                    }
                    else if (index == this.ParentTreeView.Items.Count - 1)
                    {
                        m_verticalLinePartTwo.Visibility = Visibility.Collapsed;
                    }
                }
            }
            m_horiLine = template.FindName(C_nameHorizontalLine, this) as TreeRootLine;
            m_topdragLine = template.FindName(C_nameTopDragBorder, this) as Border;
            m_bottomdragLine = template.FindName(C_nameBottomDragBorder, this) as Border;

            CoerceValue(IsInEditModeProperty);
            m_expandAnimation = ExpandAnimation.Clone();
            m_fadeAnimation = FadeAnimation.Clone();

            ClearValue(IsExpandedInternalProperty);
            Binding binding = new Binding(IsExpandedProperty.Name);
            binding.Source = this;
            SetBinding(IsExpandedInternalProperty, binding);
            m_expandAnimation.Completed += new EventHandler(Animation_Completed);

            if (m_verticalLinePartOne != null && m_verticalLinePartTwo != null && m_horiLine != null)
            {
                Binding lineBinding_v1 = new Binding("LineBrush");
                lineBinding_v1.Source = ParentTreeView;
                m_verticalLinePartOne.SetBinding(TreeRootLine.LineBrushProperty, lineBinding_v1);

                Binding lineBinding_v2 = new Binding("LineBrush");
                lineBinding_v2.Source = ParentTreeView;
                m_verticalLinePartTwo.SetBinding(TreeRootLine.LineBrushProperty, lineBinding_v2);

                Binding lineBinding_h = new Binding("LineBrush");
                lineBinding_h.Source = ParentTreeView;
                m_horiLine.SetBinding(TreeRootLine.LineBrushProperty, lineBinding_h);

                Binding penBinding_v1 = new Binding("LinePen");
                penBinding_v1.Source = ParentTreeView;
                m_verticalLinePartOne.SetBinding(TreeRootLine.LinePenProperty, penBinding_v1);

                Binding penBinding_v2 = new Binding("LinePen");
                penBinding_v2.Source = ParentTreeView;
                m_verticalLinePartTwo.SetBinding(TreeRootLine.LinePenProperty, penBinding_v2);

                Binding penBinding_h = new Binding("LinePen");
                penBinding_h.Source = ParentTreeView;
                m_horiLine.SetBinding(TreeRootLine.LinePenProperty, penBinding_h);
            }

        }

        /// <summary>
        /// Called when the value of IsInEditMode property is changed.
        /// </summary>
        /// <param name="d">TreeViewItemAdv object.</param>
        /// <param name="e">The instance containing the event data.</param>

        private static void OnIsInEditModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv item = d as TreeViewItemAdv;
            if (item.ParentTreeView != null)
            {
                if (item.ParentTreeView.EditedItemTemplateSelector != null)
                {
                    ContentPresenter cPresenter = item.GetTemplateChild("PART_EditHeader") as ContentPresenter;
                    if (cPresenter != null)
                    {
                        cPresenter.ContentTemplate = null;
                        cPresenter.ContentTemplateSelector = item.ParentTreeView.EditedItemTemplateSelector;
                    }
                }
            }

            FrameworkElement element = null;
            if (item != null && !item.IsFakeItem)
            {
                TextBlock originalText = TreeViewItemAdv.GetEditableText(item);
                TextBlock old = null;
                if (originalText != null && originalText.Text != string.Empty)
                    old = originalText;

                item.headerBeforeEdit = (old == null) ? string.Empty : old.Text;

                if (item != null && item.IsEditable)
                {
                    element = item.GetFirstFocusableFrameworkElement();
                }
                if (item != null && item.IsEditable && item.IsInEditMode && item.EditHeaderElement != null)
                {
                    item.EditHeaderElement.ApplyTemplate();
                    foreach (TextBox tBox in VisualUtils.EnumChildrenOfType(item.EditHeaderElement, typeof(TextBox)))
                    {
                        if (tBox.Visibility == Visibility.Visible)
                        {
                            element = tBox;
                            item.tempText = tBox.Text;
                        }
                    }
                    if (element == null)
                        element = item.GetFirstFocusableFrameworkElement();

                    if (element != null)
                    {
                        item.m_oldEditValue = originalText;
                        item.m_justStartEdit = true;
                        element.IsVisibleChanged += new DependencyPropertyChangedEventHandler(EditElement_IsVisibleChanged);
                        element.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(EditElement_LostKeyboardFocus);
                        element.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(element_GotKeyboardFocus);
                        element.PreviewMouseRightButtonDown += new MouseButtonEventHandler(element_PreviewMouseRightButtonDown);
                        element.KeyUp += new KeyEventHandler(TextBox_KeyUp);
                        element.KeyDown += new KeyEventHandler(TextBox_KeyDown);
                    }
                }
            }
            if (item != null)
            {
                TextBlock originalText = TreeViewItemAdv.GetEditableText(item);
                if (originalText != null)
                {
                    if (item.IsInEditMode)
                    {
                        TextBlock oldValue = originalText;
                        TextBlock newValue = originalText;
                        EditModeChangeEventArgs args = new EditModeChangeEventArgs(TreeViewItemAdv.BeforeItemEditEvent, item, oldValue.Text, newValue.Text);
                        item.OnBeforeItemEdit(args);
                        if (newValue != TreeViewItemAdv.GetEditableText(item))
                        {
                            newValue = TreeViewItemAdv.GetEditableText(item);
                            item.m_oldEditValue = newValue;
                            AssignHeader(item, newValue);
                        }
                        item.SetIsSelectionActiveInEdit(true);
                    }
                    else
                    {
                        TextBlock oldValue = item.m_oldEditValue;
                        TextBlock newValue = null;
                        if (!_wasEscKeypreseed)
                        {
                            string val = (element as TextBox) == null ? string.Empty : (element as TextBox).Text;
                            newValue = new TextBlock() { Text = val };
                        }
                        else
                        {
                            newValue = item.m_oldEditValue;
                        }
                        string temp = TreeViewItemAdv.GetEditableText(item).Text;
                        if (oldValue != null)
                        {
                            EditModeChangeEventArgs args = new EditModeChangeEventArgs(TreeViewItemAdv.AfterItemEditEvent, item, oldValue.Text, newValue.Text);
                            item.OnAfterItemEdit(args);

                            if (args.Cancel)
                            {
                                if (temp != TreeViewItemAdv.GetEditableText(item).Text)
                                {
                                    AssignHeader(item, TreeViewItemAdv.GetEditableText(item));
                                }
                                else
                                {
                                    AssignHeader(item, oldValue);
                                }
                            }
                            else
                            {
                                if (temp != TreeViewItemAdv.GetEditableText(item).Text)
                                {
                                    AssignHeader(item, TreeViewItemAdv.GetEditableText(item));
                                }
                                else
                                {
                                    if (newValue.Text == string.Empty)
                                    {
                                        AssignHeader(item, oldValue);
                                    }
                                    else
                                        AssignHeader(item, newValue);
                                }
                            }
                        }
                        item.SetIsSelectionActiveInEdit(true);
                    }

                    if (item.ParentTreeView != null && item.IsInEditMode)
                    {
                        item.ParentTreeView.EditingItem = item;
                    }
                    else if (item.ParentTreeView != null && !item.IsInEditMode)
                    {
                        item.ParentTreeView.EditingItem = null;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the PreviewMouseRightButtonDown event of the element.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyboardFocusChangedEventArgs"/> instance containing the event data.</param>
        private static void element_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItemAdv item = TreeViewAdv.GetTreeViewItemFromChildren(sender as FrameworkElement);
            if (item != null)
            {
                item.isRightClick = true;
            }
        }

        

        /// <summary>
        /// Handles the LostKeyboardFocus event of the EditElement control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyboardFocusChangedEventArgs"/> instance containing the event data.</param>
        private static void EditElement_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TreeViewItemAdv item = TreeViewAdv.GetTreeViewItemFromChildren(sender as FrameworkElement);
            if (sender is TextBox)
            {
                if (item != null)
                {
                    AssignHeader(item, TreeViewItemAdv.GetEditableText(item));
                    if (item.IsInEditMode && !item.isRightClick)
                    {
                        item.IsInEditMode = false;
                    }
                }
                _wasEscKeypreseed = false;

                (sender as TextBox).LostKeyboardFocus -= new KeyboardFocusChangedEventHandler(EditElement_LostKeyboardFocus);
            }
            else if (sender is ComboBox)
            {
                if (item != null && !(sender as ComboBox).IsDropDownOpen)
                {
                    AssignHeader(item, TreeViewItemAdv.GetEditableText(item));
                    if (item.IsInEditMode && !item.isRightClick)
                    {
                        item.IsInEditMode = false;
                    }
                }
                _wasEscKeypreseed = false;
            }
            else if (sender is ComboBoxAdv)
            {
                if (item != null && !(sender as ComboBoxAdv).IsDropDownOpen)
                {
                    AssignHeader(item, TreeViewItemAdv.GetEditableText(item));
                    if (item.IsInEditMode && !item.isRightClick)
                    {
                        item.IsInEditMode = false;
                    }
                }
                _wasEscKeypreseed = false;
            }
        }

        /// <summary>
        /// Assign header used to set the edited text to treeviewitem
        /// </summary>
        /// <param name="item">treeviewitem</param>
        /// <param name="text">textblock text</param>
        private static void AssignHeader(TreeViewItemAdv item, TextBlock text)
        {
            TextBlock headerTextBlock = TreeViewItemAdv.GetEditableTextBlock(item);

            if (headerTextBlock != null && item != null && !_wasEscKeypreseed && item.headerBeforeEdit != text.Text)
            {
                headerTextBlock = text;
                if (item.HeaderTemplate != null)
                {
                    if (TreeViewItemAdv.GetEditableTextBlock(item) != null)
                    {
                        if (headerTextBlock.Text != TreeViewItemAdv.GetEditableTextBlock(item).Text)
                        {
                            headerTextBlock = TreeViewItemAdv.GetEditableTextBlock(item);
                            headerTextBlock.Text = text.Text;
                        }
                    }
                }
                ContentControl contentpresen = item.Template.FindName("PART_Header", item) as ContentControl;
                TreeViewRowPresenter presenter = null;
                if (contentpresen != null)
                    presenter = contentpresen.Content as TreeViewRowPresenter;

                if (presenter != null)
                {
                    (presenter.InternalChildren[0] as ContentPresenter).Content = headerTextBlock.Text;
                }

                if (item.Header is string)
                {
                    item.Header = headerTextBlock.Text;
                }
                else
                {
                    if (TreeViewItemAdv.GetEditableTextBlock(item) != null)
                    {
                        if (headerTextBlock.Text != TreeViewItemAdv.GetEditableTextBlock(item).Text && item.tempText == TreeViewItemAdv.GetEditableTextBlock(item).Text)
                        {
                            headerTextBlock = TreeViewItemAdv.GetEditableTextBlock(item);
                            headerTextBlock.Text = text.Text;
                        }
                    }
                }
            }
        }

       
        /// <summary>
        /// Handles the GotKeyboardFocus event of the element control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyboardFocusChangedEventArgs"/> instance containing the event data.</param>
        private static void element_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TreeViewItemAdv item = TreeViewAdv.GetTreeViewItemFromChildren(sender as FrameworkElement);

            TextBlock headerTextBlock = TreeViewItemAdv.GetEditableTextBlock(item);

            if (headerTextBlock != null && item != null && (sender is TextBox))
            {
                if ((sender as TextBox).Text == string.Empty)
                {
                    (sender as TextBox).Text = headerTextBlock.Text;
                    (sender as TextBox).SelectionStart = 0;
                    (sender as TextBox).SelectionLength = headerTextBlock.Text.ToString().Length;
                }
                else
                {
                    if (BindingOperations.GetBinding((sender as TextBox), TextBox.TextProperty) == null && item.isTextEnter && item.ParentTreeView.EditedItemTemplateSelector == null)
                    {
                        (sender as TextBox).Text = headerTextBlock.Text;
                        item.isTextEnter = false;
                    }
                    (sender as TextBox).SelectionStart = 0;
                    (sender as TextBox).SelectionLength = (sender as TextBox).Text.ToString().Length;
                }
            }

            if (sender is TextBox)
                (sender as TextBox).GotKeyboardFocus -= new KeyboardFocusChangedEventHandler(element_GotKeyboardFocus);
        }

        /// <summary>
        /// Gets the editable text.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private static TextBlock GetEditableText(TreeViewItemAdv item)
        {
            TextBlock originalText = null;

            if (item != null)
            {
                if (item.Header is string && originalText != null)
                {
                    originalText.Text = item.Header.ToString();
                    return originalText;
                }
                TextBlock oobj = null;
                if (!item.MultiColumnEnable)
                {
                    foreach (TextBlock tBlock in VisualUtils.EnumChildrenOfType(item.Template.FindName("PART_Header", item) as ContentPresenter, typeof(TextBlock)))
                    {
                        if (tBlock.Visibility == Visibility.Visible && tBlock.Text == item.tempText)
                            oobj = tBlock;
                    }
                    if (oobj == null)
                        oobj = VisualUtils.FindDescendant(item.Template.FindName("PART_Header", item) as ContentPresenter, typeof(TextBlock)) as TextBlock;
                }
                else
                {
                    oobj = VisualUtils.FindDescendant(item.Template.FindName("PART_Header", item) as ContentControl, typeof(TextBlock)) as TextBlock;
                }
                if (oobj != null)
                {
                    originalText = oobj;
                }
            }
            return originalText;
        }

        /// <summary>
        /// Gets the editable text block.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private static TextBlock GetEditableTextBlock(TreeViewItemAdv item)
        {
            if (item != null)
            {
                TextBlock oobj = null;
                if (!item.MultiColumnEnable)
                {
                    foreach (TextBlock tBlock in VisualUtils.EnumChildrenOfType(item.Template.FindName("PART_Header", item) as ContentPresenter, typeof(TextBlock)))
                    {
                        if (tBlock.Visibility == Visibility.Visible && tBlock.Text == item.tempText)
                            oobj = tBlock;
                    }
                    if (oobj == null)
                        oobj = VisualUtils.FindDescendant(item.Template.FindName("PART_Header", item) as ContentPresenter, typeof(TextBlock)) as TextBlock;
                }
                else
                {
                    oobj = VisualUtils.FindDescendant(item.Template.FindName("PART_Header", item) as ContentControl, typeof(TextBlock)) as TextBlock;
                }
                return oobj;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether item need set edited value to directly.
        /// </summary>
        private bool IsDirectlySetEditValue
        {
            get
            {
                bool bValue = true;

                if (ParentItemsControl != null && ParentItemsControl.ItemsSource != null
                    && !(Header is String))
                {
                    bValue = false;
                }

                return bValue;
            }
        }

        /// <summary>
        /// Sets the is selection active in edit.
        /// </summary>
        /// <param name="bIsSelectionActive">if set to <c>true</c> [b is selection active].</param>
        private void SetIsSelectionActiveInEdit(bool bIsSelectionActive)
        {
            if (ParentTreeView != null && ParentTreeView.IsMultiselection)
            {
                foreach (TreeViewItemAdv item in ParentTreeView.SelectedContainers)
                {
                    if (item != this)
                    {
                        item.SetValue(TreeViewItemAdv.IsSelectionActivePropertyKey, bIsSelectionActive);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the first focusable framework element.
        /// </summary>
        /// <returns>FrameworkElement element</returns>
        private FrameworkElement GetFirstFocusableFrameworkElement()
        {
            FrameworkElement element = null;
            DependencyObject obj = TreeViewItemAdv.FindFirstFocusableElement(EditHeaderElement);

            if (obj != null)
            {
                element = obj as FrameworkElement;
            }

            return element;
        }

        /// <summary>
        /// Finds the first focusable element.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns>Dependency Object</returns>
        private static DependencyObject FindFirstFocusableElement(DependencyObject parent)
        {
            DependencyObject element = null;

            if (parent != null)
            {
                int count = VisualTreeHelper.GetChildrenCount(parent);

                if (count > 0)
                {
                    DependencyObject currentElement = null;
                    IInputElement iInputElement = null;
                    bool isElementVisible = false;
                    for (int i = 0; i < count; i++)
                    {
                        currentElement = VisualTreeHelper.GetChild(parent, i) as FrameworkElement;
                        if (currentElement is FrameworkElement)
                        {
                            if (((FrameworkElement)currentElement).Visibility == Visibility.Visible)
                                isElementVisible = true;
                        }
                        else
                            isElementVisible = true;
                        if (currentElement != null && isElementVisible)
                        {
                            iInputElement = currentElement as IInputElement;

                            if (iInputElement != null && iInputElement.Focusable)
                            {
                                element = currentElement;
                                break;
                            }
                            else
                            {
                                currentElement = FindFirstFocusableElement(currentElement);
                                if (currentElement != null)
                                {
                                    element = currentElement;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return element;
        }

        /// <summary>
        /// Handles the IsVisibleChanged event of the EditElement control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void EditElement_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ComboBox)
            {
                ComboBox element = sender as ComboBox;
                TreeViewItemAdv item = TreeViewAdv.GetTreeViewItemFromChildren(sender as FrameworkElement);
                item.EditComboBoxChanged(sender, element, item);
            }
            else if (sender is ComboBoxAdv)
            {
                ComboBoxAdv element = sender as ComboBoxAdv;
                TreeViewItemAdv item = TreeViewAdv.GetTreeViewItemFromChildren(sender as FrameworkElement);
                item.EditComboBoxAdvChanged(sender, element, item);
            }
            else
            {
                TextBox element = sender as TextBox;
                TreeViewItemAdv item = TreeViewAdv.GetTreeViewItemFromChildren(sender as FrameworkElement);
                item.EditTextBoxChanged(sender, element, item);
            }
        }

        private void EditTextBoxChanged(object sender, TextBox element, TreeViewItemAdv item)
        {
            if (element != null)
            {
                if (element.IsVisible)
                {
                    element.Focus();
                    if (item != null && item.ParentTreeView != null)
                    {
                        item.ParentTreeView.IsDragInEditingState = true;
                    }
                }
                else
                {
                    element.IsVisibleChanged -= new DependencyPropertyChangedEventHandler(EditElement_IsVisibleChanged);

                    element.KeyUp -= new KeyEventHandler(TextBox_KeyUp);
                    element.KeyDown -= new KeyEventHandler(TextBox_KeyDown);
                    if (item != null && item.ParentTreeView != null)
                    {
                        item.ParentTreeView.IsDragInEditingState = false;
                    }
                }
            }
        }

        private void EditComboBoxChanged(object sender, ComboBox element, TreeViewItemAdv item)
        {
            if (element != null)
            {
                if (element.IsVisible)
                {
                    element.Focus();
                    if (item != null && item.ParentTreeView != null)
                    {
                        item.ParentTreeView.IsDragInEditingState = true;
                    }
                }
                else
                {
                    element.IsVisibleChanged -= new DependencyPropertyChangedEventHandler(EditElement_IsVisibleChanged);
                    item.isTextEnter = true;
                    if (element != null && item != null && element.SelectionBoxItem.ToString() != string.Empty)
                    {
                        item.Header = element.SelectionBoxItem;
                    }
                    if (item != null && item.ParentTreeView != null)
                    {
                        item.ParentTreeView.IsDragInEditingState = false;
                    }
                }
            }
        }

        private void EditComboBoxAdvChanged(object sender, ComboBoxAdv element, TreeViewItemAdv item)
        {
            if (element != null)
            {
                if (element.IsVisible)
                {
                    element.Focus();
                    if (item != null && item.ParentTreeView != null)
                    {
                        item.ParentTreeView.IsDragInEditingState = true;
                    }
                }
                else
                {
                    element.IsVisibleChanged -= new DependencyPropertyChangedEventHandler(EditElement_IsVisibleChanged);

                    item.isTextEnter = true;
                    if (element != null && item != null && element.SelectionBoxItem.ToString() != string.Empty)
                    {
                        item.Header = element.SelectionBoxItem;
                    }
                    if (item != null && item.ParentTreeView != null)
                    {
                        item.ParentTreeView.IsDragInEditingState = false;
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when to the KeyUp event is received.
        /// </summary>
        /// <param name="sender">Text Box value</param>
        /// <param name="e">Information about the event.</param>
        private static void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox textBox = e.Source as TextBox;

            if (textBox != null)
            {
                switch (e.Key)
                {
                    case Key.Up:
                        {
                            textBox.CaretIndex++;
                            e.Handled = true;
                            break;
                        }

                    case Key.Down:
                        {
                            if (textBox.CaretIndex > 0)
                            {
                                textBox.CaretIndex--;
                            }

                            e.Handled = true;
                            break;
                        }
                }

                TreeViewItemAdv item = TreeViewAdv.GetTreeViewItemFromChildren(sender as FrameworkElement);

                if (item != null)
                {
                    if (item.m_justStartEdit)
                    {
                        item.m_justStartEdit = false;
                    }
                    else
                    {
                        KeyEventArgs args = new KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, e.Key);
                        args.RoutedEvent = TreeViewItemAdv.EditKeyUpEvent;
                        args.Source = textBox;
                        item.OnEditKeyUp(args);
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when to the KeyDown event is received.
        /// </summary>
        /// <param name="sender">Text Box value</param>
        /// <param name="e">Information about the event.</param>
        private static void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = e.Source as TextBox;
            TreeViewItemAdv item = TreeViewAdv.GetTreeViewItemFromChildren(sender as FrameworkElement);
            item.isTextEnter = true;
            if (textBox != null && item != null && !item.m_justStartEdit)
            {
                if (e.Key != Key.DeadCharProcessed)
                {
                    KeyEventArgs args = new KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, e.Key);
                    args.RoutedEvent = TreeViewItemAdv.EditKeyDownEvent;
                    args.Source = textBox;
                    item.OnEditKeyDown(args);
                }
            }
        }

        /// <summary>
        /// Detaches animation, if any, from the given property of the
        /// given element. Detaching animation unfreezes property
        /// allowing thus changing its value.
        /// </summary>
        private void DetachAnimation()
        {
            if (ItemsHost != null)
            {
                double height = 0;

                if (m_bAnimatinExpand)
                {
                    height = m_expandAnimation.To.Value;

                    if (ItemsHost.Height >= height)
                    {
                        ResetAnimation();
                        m_bAnimating = false;
                        UpdateIsExpandedCoerce();
                    }
                }
                else if (ItemsHost.Height <= height)
                {
                    ResetAnimation();
                    m_bAnimating = false;
                    UpdateIsExpandedCoerce();
                }
            }
        }

        /// <summary>
        /// Begins expand animation.
        /// </summary>
        private void BeginExpandAnimation()
        {
            if (Items.Count > 0 && ItemsHost != null && m_expandAnimation != null
                && m_expandAnimation.To != m_expandAnimation.From)
            {
                ItemsHost.BeginAnimation(TreeViewItemAdv.HeightProperty, m_expandAnimation);
                if (ParentTreeView.AnimationType == AnimationType.Fade)
                {
                    ItemsHost.BeginAnimation(TreeViewItemAdv.OpacityProperty, m_fadeAnimation);
                }

                m_bAnimating = true;
            }
        }

        /// <summary>
        /// Begins collapse animation.
        /// </summary>
        private void BeginCollapseAnimation()
        {
            if (Items.Count > 0 && ItemsHost != null && m_expandAnimation != null)
            {
                ItemsHost.BeginAnimation(ItemsPresenter.HeightProperty, m_expandAnimation);

                if (ParentTreeView.AnimationType == AnimationType.Fade)
                {
                    ItemsHost.BeginAnimation(TreeViewItemAdv.OpacityProperty, m_fadeAnimation);
                }

                m_bAnimating = true;
            }
        }

        /// <summary>
        /// Updates the animation.
        /// </summary>
        /// <param name="bExpand">if set to <c>true</c> [b expand].</param>
        private void UpdateAnimation(bool bExpand)
        {
            if (Items.Count > 0 && ItemsHost != null && m_expandAnimation != null)
            {
                double height = GetAnimationHeight();

                if (bExpand)
                {
                    m_expandAnimation.From = (m_bAnimating && !ItemsHost.Height.Equals(double.NaN)) ?
                        ItemsHost.Height : 0;
                    m_expandAnimation.To = height;
                    m_fadeAnimation.From = 0;
                    m_fadeAnimation.To = 1;
                }
                else
                {
                    m_expandAnimation.From = (m_bAnimating && !ItemsHost.Height.Equals(double.NaN)) ?
                        ItemsHost.Height : height;
                    m_expandAnimation.To = 0;
                    m_fadeAnimation.From = 1;
                    m_fadeAnimation.To = 0;
                }

                if (ParentTreeView != null)
                {
                    m_expandAnimation.SpeedRatio = ParentTreeView.AnimationSpeed;
                    m_fadeAnimation.SpeedRatio = ParentTreeView.AnimationSpeed;
                }
            }
        }

        /// <summary>
        /// Resets animation.
        /// </summary>
        private void ResetAnimation()
        {
            if (ItemsHost != null)
            {
                ItemsHost.BeginAnimation(TreeViewItemAdv.OpacityProperty, null);
                ItemsHost.BeginAnimation(TreeViewItemAdv.HeightProperty, null);
            }
        }

        /// <summary>
        /// Updates the item changed animation.
        /// </summary>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void UpdateItemChangedAnimation(NotifyCollectionChangedEventArgs e)
        {
            if (e != null && ItemsHost != null && m_expandAnimation != null)
            {
                double height = GetAnimationHeight();

                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        {
                            m_expandAnimation.From = height - GetItemsHeight(e.NewItems);
                            m_expandAnimation.To = height;
                            break;
                        }

                    case NotifyCollectionChangedAction.Remove:
                        {
                            m_expandAnimation.From = height + GetItemsHeight(e.OldItems);
                            m_expandAnimation.To = height;
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Updates coerce for IsExpanded property.
        /// </summary>
        private void UpdateIsExpandedCoerce()
        {
            m_bUpdateCoerce = true;
            CoerceValue(TreeViewItemAdv.IsExpandedProperty);
            CoerceValue(TreeViewItemAdv.IsExpandedInternalProperty);
        }

        /// <summary>
        /// Gets the height of the items.
        /// </summary>
        /// <param name="list">The list value.</param>
        /// <returns>double value type</returns>
        private double GetItemsHeight(IList list)
        {
            double height = 0;

            if (list != null)
            {
                TreeViewItemAdv item = null;

                for (int i = 0; i < list.Count; i++)
                {
                    item = list[i] as TreeViewItemAdv;

                    if (item != null)
                    {
                        height += item.DesiredSize.Height;
                    }
                }
            }

            return height;
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Tree View Item Adv</returns>
        private TreeViewItemAdv GetItem(int index)
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
        /// Gets the height of the animation.
        /// </summary>
        /// <returns>double value type </returns>
        private double GetAnimationHeight()
        {
            double height = 0;

            if (Items.Count > 0)
            {
                TreeViewItemAdv item = null;

                for (int i = 0; i < Items.Count; i++)
                {
                    item = GetItem(i);
                    if (item == null && this.ParentItemsControl != null)
                        item = this.ParentItemsControl.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv;

                    if (item != null && item.Visibility == Visibility.Visible)
                    {
                        if (item.DesiredSize.Height == 0d)
                        {
                            item.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        }
                        if (item.DesiredSize.Height > this.DesiredItemHeight)
                            height += this.DesiredItemHeight;
                        else
                            height += item.DesiredSize.Height;
                    }
                }
            }

            return height;
        }

        /// <summary>
        /// Handles the Completed event of the Animation control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Animation_Completed(object sender, EventArgs e)
        {
            DetachAnimation();
        }

        /// <summary>
        /// Gets the size fake items.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        /// <returns>Size items size</returns>
        private Size GetSizeFakeItems(ItemsControl itemsControl)
        {
            Size size = new Size(0, 0);

            if (itemsControl != null)
            {
                FrameworkElement element = null;

                for (int i = 0; i < itemsControl.Items.Count; i++)
                {
                    element = itemsControl.Items[i] as FrameworkElement;

                    if (element != null)
                    {
                        size.Height += element.Height;

                        if (element.Width > size.Width)
                        {
                            size.Width = element.Width;
                        }
                    }
                }
            }

            return size;
        }

        /// <summary>
        /// Gets the index of the drop.
        /// </summary>
        /// <returns>int value type</returns>
        internal int GetDropIndex()
        {
            int index = -1;

            if (ParentItemsControl != null)
            {
                if (ParentTreeView != null && ParentTreeView.IsFakeDragIndicator
                    && IsFakeItem)
                {
                    IItemsPanelRef panel = ParentItemsControl as IItemsPanelRef;

                    if (panel != null && panel.ItemsPanel != null)
                    {
                        index = panel.ItemsPanel.GetIndexNoFakeItems(this);
                    }
                }
                else
                {
                    index = ParentItemsControl.ItemContainerGenerator.IndexFromContainer(this);
                }

                if (index > -1)
                {
                    TreeViewItemAdv item = ParentItemsControl.ItemContainerGenerator.ContainerFromIndex(index) as TreeViewItemAdv;

                    if (item != null && !item.IsShowTopMarker)
                    {
                        ++index;
                    }
                }
            }

            return index;
        }

        /// <summary>
        /// Tries to add the given adorner to adorner layer of logical
        /// parent.
        /// </summary>
        /// <param name="adorner">Adorner to be added.</param>
        /// <returns>
        /// Value indicating whether adorner has been successfully added
        /// to adorner layer.
        /// </returns>
        internal bool TryAddAdorner(Adorner adorner)
        {
            bool ret = false;

            if (adorner != null && ParentTreeView != null)
            {
                AdornerLayer layer = ParentTreeView.GetAdornerLayer();

                if (layer != null && layer.GetAdorners(this) == null)
                {
                    layer.Add(adorner);
                    ret = true;
                }
            }

            return ret;
        }

        /// <summary>
        /// Tries to remove the given adorner from adorner layer of
        /// logical parent.
        /// </summary>
        /// <param name="adorner">Adorner to be removed.</param>
        /// <returns>
        /// Value indicating whether adorner has been successfully
        /// removed from adorner layer.
        /// </returns>
        internal bool TryRemoveAdorner(Adorner adorner)
        {
            bool ret = false;

            if (adorner != null && ParentTreeView != null)
            {
                AdornerLayer adornerLayer = ParentTreeView.GetAdornerLayer();

                if (adornerLayer != null)
                {
                    if (adorner.IsVisible)
                    {
                        ret = true;
                    }

                    adornerLayer.Remove(adorner);
                }
            }

            return ret;
        }

        /// <summary>
        /// Gets the width unheader.
        /// </summary>
        /// <param name="bIncludImage">if set to <c>true</c> [b include image].</param>
        /// <returns>double value type</returns>
        internal double GetWidthUnheader(bool bIncludImage)
        {
            double width = 0;
            Expander expander = GetExpander();

            if (expander != null)
            {
                width += expander.DesiredSize.Width;
            }

            if (bIncludImage)
            {
                FrameworkElement imagePanel = GetImagePanel();

                if (imagePanel != null)
                {
                    width += imagePanel.DesiredSize.Width;
                }
            }

            if (ParentTreeViewItem != null)
            {
                width += ParentTreeViewItem.GetWidthUnheader(false);
            }

            return width;
        }

        /// <summary>
        /// Gets the expander.
        /// </summary>
        /// <returns>Expander m_expander</returns>
        private Expander GetExpander()
        {
            if (m_expander == null && Template != null)
            {
                m_expander = Template.FindName(C_nameExpander, this) as Expander;
            }

            return m_expander;
        }

        /// <summary>
        /// Gets the image panel.
        /// </summary>
        /// <returns>Framework Element</returns>
        private FrameworkElement GetImagePanel()
        {
            if (m_imagePanel == null && Template != null)
            {
                m_imagePanel = Template.FindName(C_nameImagePanel, this) as FrameworkElement;
            }

            return m_imagePanel;
        }

        /// <summary>
        /// Calls OnIsSelectedChanged method of the instance, notifies
        /// of the dependency property value changes.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs on.</param>
        /// <param name="e">Property changes details, such as old value
        /// and new value.</param>
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItemAdv element = (TreeViewItemAdv)d;

            if (element.IsSelected && element.ParentTreeView != null && element.ParentTreeView.IsVirtualizing)
            {
                if (element.ParentTreeView.AllowMultiSelect && element.ParentTreeView.m_itemsselectedfromlinearlist)
                {
                    if (!element.ParentTreeView.SelectedStateLinearList.Contains(element.m_treeviewitemactualobject))
                    {
                        element.ParentTreeView.SelectedStateLinearList.Add(element.m_treeviewitemactualobject);
                    }
                }
                else
                {
                    element.ParentTreeView.SelectedStateLinearList.Clear();
                    element.ParentTreeView.SelectedStateLinearList.Add(element.m_treeviewitemactualobject);
                }
            }
            else if (!element.IsSelected && element.ParentTreeView != null && element.ParentTreeView.IsVirtualizing)
            {
                if (element.ParentTreeView.SelectedStateLinearList.Contains(element.m_treeviewitemactualobject))
                {
                    element.ParentTreeView.SelectedStateLinearList.Remove(element.m_treeviewitemactualobject);
                }
            }

            bool selected = (bool)e.NewValue;

            if (element != null && !element.m_bUpdatingCoerce)
            {
                if (element.ParentTreeView != null)
                {
                    element.ParentTreeView.SelectedTreeItemObject = element.m_treeviewitemactualobject;
                }
                element.Select(selected);
                TreeViewItemAdvAutomationPeer peer = UIElementAutomationPeer.FromElement(element) as TreeViewItemAdvAutomationPeer;

                if (peer != null)
                {
                    peer.RaiseAutomationIsSelectedChanged(selected);
                }

                if (selected)
                {
                    element.OnSelected(new RoutedEventArgs(SelectedEvent, element));
                    element.m_needEditByClick = false;
                }
                else
                {
                    element.OnUnselected(new RoutedEventArgs(UnselectedEvent, element));
                    element.m_needEditByClick = false;
                }

                element.m_bIsEditByTimer = false;
            }
        }

        /// <summary>
        /// Calls OnIsLastItemChanged method of the instance, notifies
        /// of the property value changes.
        /// </summary>
        private void OnIsLastItemChanged()
        {
            TreeViewItemAdv element = this;

            if (element.ParentItemsControl != null)
            {
                IItemsPanelRef panel = element.ParentItemsControl as IItemsPanelRef;

                if (panel != null && panel.ItemsPanel != null)
                {
                    panel.ItemsPanel.InvalidateRender();
                }
            }
        }

        /// <summary>
        /// Starts the drag timer.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        private void StartDragTimer(DragEventArgs e)
        {
            if (e != null)
            {
                m_dragTimer.Tag = e;
                m_dragTimer.Tick += new EventHandler(DragTimer_Tick);
                m_dragTimer.Interval = TimeSpan.FromMilliseconds(C_dragDelay);
                m_dragTimer.Start();
                m_bIsDragByTimer = true;
                m_dragByTimerPoint = MouseUtils.GetMousePosition(this);
            }
        }

        /// <summary>
        /// Stops drag timer.
        /// </summary>
        internal void StopDragTimer()
        {
            m_bIsDragByTimer = false;
            m_dragTimer.Stop();
        }

        #endregion Implementation

        #region Support ICloneable

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        object ICloneable.Clone()
        {
            TreeViewItemAdv clone = new TreeViewItemAdv();
            if (this.Header is UserControl)
            {
                clone.Header = (Header is FrameworkElement) ? CloneFrameWorkElement(this.Header as FrameworkElement) : this.Header is ICloneable ? (this.Header as ICloneable).Clone() : this.Header;
            }
            else
            {
                clone.Header = (Header is UIElement) ? CloneUIElement(this.Header as UIElement) : this.Header is ICloneable ? (this.Header as ICloneable).Clone() : this.Header;
            }
            clone.Tag = this.Tag;
            clone.Background = this.Background;
            clone.BorderBrush = this.BorderBrush;
            clone.BorderThickness = this.BorderThickness;
            clone.FontSize = this.FontSize;
            clone.FontStyle = this.FontStyle;
            clone.FontStretch = this.FontStretch;
            clone.FontWeight = this.FontWeight;
            clone.HorizontalAlignment = this.HorizontalAlignment;
            clone.HorizontalContentAlignment = this.HorizontalContentAlignment;
            clone.Sorting = this.Sorting;
            clone.ItemsSource = this.ItemsSource;
            clone.ImageWidth = this.ImageWidth;
            clone.ImageHeight = this.ImageHeight;
            clone.ImageStretch = this.ImageStretch;
            clone.Visibility = this.Visibility;
            clone.IsReadOnly = this.IsReadOnly;
            clone.IsEditable = this.IsEditable;

            if (LeftImageSource != null)
            {
                clone.LeftImageSource = this.LeftImageSource;
            }

            if (RightImageSource != null)
            {
                clone.RightImageSource = this.RightImageSource;
            }

            if (CollapsedImageSource != null)
            {
                clone.CollapsedImageSource = this.CollapsedImageSource;
            }

            if (ExpandedImageSource != null)
            {
                clone.ExpandedImageSource = this.ExpandedImageSource;
            }

            if (ItemsSource == null && Items.Count > 0)
            {
                ICloneable childClone = null;

                for (int i = 0; i < Items.Count; i++)
                {
                    childClone = Items[i] as ICloneable;

                    if (childClone != null)
                    {
                        clone.Items.Add(childClone.Clone());
                    }
                }
            }

            return clone;
        }

        /// <summary>
        /// Clones the UI element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>UIElement value type</returns>
        private UIElement CloneUIElement(UIElement element)
        {
            UIElement clone = null;

            if (element != null)
            {
                StringReader strReader = new StringReader(XamlWriter.Save(element));
                XmlReader xmlReader = XmlReader.Create(strReader);
                if ((element as FrameworkElement).Tag != null)
                {
                    if ((element as FrameworkElement).Tag.Equals(String.Empty))
                    {
                        clone = XamlReader.Load(xmlReader) as UIElement;
                    }
                }
                else
                {
                    clone = XamlReader.Load(xmlReader) as UIElement;
                }
            }

            return clone;
        }

        private FrameworkElement CloneFrameWorkElement(FrameworkElement element)
        {
            FrameworkElement clone = null;

            if (element != null)
            {
                clone = TreeViewAdvCloneManager.Clone(element) as FrameworkElement;
                clone.DataContext = element.DataContext;
            }

            return clone;
        }

        /// <summary>
        /// XMLs the clone.
        /// </summary>
        /// <param name="obj">The obj value.</param>
        /// <returns>object value type</returns>
        private object XmlClone(object obj)
        {
            object clone = null;

            if (obj != null)
            {
                try
                {
                    StringReader strReader = new StringReader(XamlWriter.Save(obj));
                    XmlReader xmlReader = XmlReader.Create(strReader);
                    clone = XamlReader.Load(xmlReader) as object;
                }
                catch (System.Windows.Markup.XamlParseException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return clone;
        }

        #endregion Support ICloneable

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
        /// Gets the item.
        /// </summary>
        /// <param name="index">The index.</param>
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
        /// Gets the index.
        /// </summary>
        /// <param name="obj">The obj of index.</param>
        /// <returns>int type for index</returns>
        int IItemContainer.GetIndex(object obj)
        {
            int index = -1;

            if (obj != null)
            {
                if (obj is TreeViewItemAdv)
                {
                    index = ItemContainerGenerator.IndexFromContainer(obj as DependencyObject);

                    if (index == -1)
                    {
                        index = Items.IndexOf(obj);
                    }
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
        /// Gets the first visible item.
        /// </summary>
        /// <returns>TreeViewItemAdv first visibility</returns>
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
        /// Gets the last visible item.
        /// </summary>
        /// <returns>Last Visible Item</returns>
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
}