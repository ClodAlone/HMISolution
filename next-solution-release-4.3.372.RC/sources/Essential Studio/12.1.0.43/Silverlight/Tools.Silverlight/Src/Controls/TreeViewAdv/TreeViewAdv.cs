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
using System.Collections.Generic;
using System.Collections;
using System.Windows.Data;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Browser;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Controls;
using System.ComponentModel;
using System.Reflection;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a TreeViewAdv Control, used for populating items in
    /// the form of Tree.
    /// </summary>
    /// <example>
    /// The control can be added to the application in the following ways. 
    /// <para></para>
    /// <para></para>
    /// <list type="table">
    /// <listheader>
    /// <term>Xaml</term></listheader>
    /// <item>
    /// <description>&lt;syncfusion:TreeViewAdv Height=&quot;150&quot;
    /// Width=&quot;300&quot;  Name=&quot;treeview'/&gt;
    /// <para>                                </para></description></item></list>
    /// <list type="table">
    /// <listheader>
    /// <term>C#</term></listheader>
    /// <item>
    /// <description>TreeViewAdv treeview=new TreeViewAdv; </description>
    /// </item>
    /// </list>
    /// </example>
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
      Type = typeof(TreeViewAdv), XamlResource = "/Syncfusion.Theming.Blend;component/TreeViewAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(TreeViewAdv), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/TreeViewAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(TreeViewAdv), XamlResource = "/Syncfusion.Theming.Office2007Black;component/TreeViewAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(TreeViewAdv), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/TreeViewAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
        Type = typeof(TreeViewAdv), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/TreeViewAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(TreeViewAdv), XamlResource = "/Syncfusion.Theming.Office2010Black;component/TreeViewAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(TreeViewAdv), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/TreeViewAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(TreeViewAdv), XamlResource = "/Syncfusion.Theming.Default;component/TreeViewAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
        Type = typeof(TreeViewAdv), XamlResource = "/Syncfusion.Theming.Office2003;component/TreeViewAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
        Type = typeof(TreeViewAdv), XamlResource = "/Syncfusion.Theming.Windows7;component/TreeViewAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
        Type = typeof(TreeViewAdv), XamlResource = "/Syncfusion.Theming.VS2010;component/TreeViewAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
        Type = typeof(TreeViewAdv), XamlResource = "/Syncfusion.Theming.Metro;component/TreeViewAdv.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent,
        Type = typeof(TreeViewAdv), XamlResource = "/Syncfusion.Theming.Transparent;component/TreeViewAdv.xaml")]
    [TemplateVisualState(Name = "Focused", GroupName = "FocusStates")]
    [TemplateVisualState(Name = "Unfocused", GroupName = "FocusStates")]
    public class TreeViewAdv : ItemsControl
    {

        internal int itemCount = 0;
        /// <summary>
        /// 
        /// </summary>
        public event ExpandCollapseEventHandler Expanding;
        /// <summary>
        /// 
        /// </summary>
        public event ExpandCollapseEventHandler Collapsing;

        /// <summary>
        /// Occurs when [node editing].
        /// </summary>
        public event NodeCancellableEditEventHandler NodeEditing;
        /// <summary>
        /// Occurs when [node edited].
        /// </summary>
        public event NodeEditEventHandler NodeEdited;
        /// <summary>
        /// Occurs when [node edit cancelled].
        /// </summary>
        public event NodeEditEventHandler NodeEditCancelled;

        /// <summary>
        /// Occurs when [node editor validate string].
        /// </summary>
        public event NodeEditorCancellableEventHandler NodeEditorValidateString;
        /// <summary>
        /// Occurs when [node editor validating].
        /// </summary>
        public event NodeEditorCancellableEventHandler NodeEditorValidating;
        /// <summary>
        /// Occurs when [node editor validated].
        /// </summary>
        public event NodeEditorEventHandler NodeEditorValidated;

		/// <summary>
        /// Occurs when [expanding the TreeViewItemAdv in OnDemandLoading].
        /// </summary>
        public event LoadOnDemandEventHandler LoadOnDemand;
        //used to change the theme states
        /// <summary>
        /// Gets or sets a value indicating whether this instance is focused.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is focused; otherwise, <c>false</c>.
        /// </value>
        internal bool IsFocused   
        {
            get { return (bool)GetValue(IsFocusedProperty); }
            set { SetValue(IsFocusedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsFocused.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        internal static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.Register("IsFocused", typeof(bool), typeof(TreeViewAdv), new PropertyMetadata(true,OnIsFocusedChanged));

        /// <summary>
        /// Called when [is focused changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsFocusedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewAdv treeView = d as TreeViewAdv;
            if (treeView.IsFocused == false)
            {
                TreeViewItemAdv tree = treeView.SelectedItem as TreeViewItemAdv;
                if(tree != null)
                {
                    tree.UpdateUnfocusedState();
                    treeView.IsFocused = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        /// <value>The theme.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [BrowsableAttribute(false)]
        public string Theme
        {
            get { return (string)GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        internal static readonly DependencyProperty ThemeProperty =
            DependencyProperty.Register("Theme", typeof(string), typeof(TreeViewAdv), new PropertyMetadata(OnThemeChanged));

        internal bool _mouseEnter = true;

        private static void OnThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewAdv treeView = d as TreeViewAdv;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected void OnThemeChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.ThemeChanged != null)
            {
                this.ThemeChanged(this, args);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [enable mouse over effect].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable mouse over effect]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableMouseOverEffect
        {
            get { return (bool)GetValue(EnableMouseOverEffectProperty); }
            set { SetValue(EnableMouseOverEffectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableMouseOverEffect.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty EnableMouseOverEffectProperty =
            DependencyProperty.Register("EnableMouseOverEffect", typeof(bool), typeof(TreeViewAdv), new PropertyMetadata(true));


        #region Drag and Drop
        ///// <summary>
        ///// Even that is raised when the Dragging of nodes begins. 
        ///// </summary>
        //public event TreeViewAdvDragEventHandler DragStarted;

        ///// <summary>
        ///// Event that is raised when the node is being dragged. 
        ///// </summary>
        //public event TreeViewAdvDragEventHandler DragMoved;//drag query

        ///// <summary>
        ///// Evemt that is raised when the Dragging of nodes ends. 
        ///// </summary>
        //public event TreeViewAdvDragEventHandler DragFinished;

        ///// <summary>
        ///// Occurs when the item is dragged above the particular item
        ///// </summary>
        //public event DragMoveHandler DragAbove;

        ///// <summary>
        ///// Occurs when the item is dragged below the particualar item.
        ///// </summary>
        //public event DragMoveHandler DragBelow;

        ///// <summary>
        ///// Occurs when the item is dragged over the particular item.
        ///// </summary>
        //public event DragMoveHandler DragOver;

        /// <summary>
        /// 
        /// </summary>
        public event TreeViewAdvDragStartEventHandler DragStarted;
        /// <summary>
        /// 
        /// </summary>
        public event TreeViewAdvDragEventHandler DragQuery;
        /// <summary>
        /// 
        /// </summary>
        public event TreeViewAdvDragEventHandler DragDrop;

        #endregion

        #region Dependency Properties
        /// <summary>
        /// This Property determines whether the item should be selected when the expansion changed
        /// </summary>
        public static readonly DependencyProperty SelectOnExpandChangeProperty = DependencyProperty.Register("SelectOnExpandChange", typeof(bool), typeof(TreeViewAdv), new PropertyMetadata(true,new PropertyChangedCallback(OnSelectOnExpandChangeChanged)));

        /// <summary>
        /// This Property contains selected TreeViewAdv Node, which is of type Object
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(TreeViewAdv), new PropertyMetadata(new PropertyChangedCallback(OnSelectedItemChanged)));

        /// <summary>
        /// This property contains selected TreeViewAdv Node, Which is of Type TreeViewItemAdv
        /// </summary>
        public static readonly DependencyProperty SelectedNodeProperty = DependencyProperty.Register("SelectedNode", typeof(TreeViewItemAdv), typeof(TreeViewAdv), new PropertyMetadata(new PropertyChangedCallback(OnSelectedNodeChanged)));

        /// <summary>
        /// This Property indicates whether the TreeViewAdv Node is in edit mode or not
        /// </summary>
        public static readonly DependencyProperty IsInEditModeProperty = DependencyProperty.Register("IsInEditMode", typeof(bool), typeof(TreeViewAdv), new PropertyMetadata(true, new PropertyChangedCallback(OnIsInEditModeChanged)));

        /// <summary>
        /// This Property indicates the Type of sorting, Ascending or Descending
        /// </summary>
        public static readonly DependencyProperty SortModeProperty = DependencyProperty.Register("SortMode", typeof(SortMode), typeof(TreeViewAdv), new PropertyMetadata(SortMode.Asc, new PropertyChangedCallback(OnSortModeChanged)));

        /// <summary>
        /// This Property determines whether the dragging of nodes of TreeViewAdv is enabled or not
        /// </summary>
        public static readonly DependencyProperty DraggingEnabledProperty = DependencyProperty.Register("DraggingEnabled", typeof(bool), typeof(TreeViewAdv), new PropertyMetadata(true, new PropertyChangedCallback(OnDraggingEnabledChanged)));

        /// <summary>
        /// This Property determines whether multiple selection of nodes is allowed or not
        /// </summary>
        public static readonly DependencyProperty IsMultiSelectProperty = DependencyProperty.Register("IsMultiSelect", typeof(bool), typeof(TreeViewAdv), new PropertyMetadata(false, new PropertyChangedCallback(OnIsMultiSelectChanged)));

        /// <summary>
        /// This property contains the selected items. This property will handles the selection of items only when the IsMultiSelect Property is set to true;
        /// </summary>
        public static readonly DependencyProperty SelectedNodesProperty = DependencyProperty.Register("SelectedNodes", typeof(List<TreeViewItemAdv>), typeof(TreeViewAdv), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedNodesChanged)));
        /// <summary>
        /// This property contains the selected items. This property will handles the selection of items only when the IsMultiSelect Property is set to true;
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register("SelectedItems", typeof(List<object>), typeof(TreeViewAdv), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedItemsChanged)));
        
        /// <summary>
        /// This Property indicates the StrokeDashArray for the TreeViewAdv Lines.
        /// </summary>
        public static readonly DependencyProperty LineStrokeArrayProperty = DependencyProperty.Register("LineStrokeArray", typeof(DoubleCollection), typeof(TreeViewAdv), new PropertyMetadata(null, new PropertyChangedCallback(OnLineStrokeArrayChanged)));

        /// <summary>
        /// This Property indicates the Stroke for the TreeViewAdv Lines
        /// </summary>
        public static readonly DependencyProperty RootLineStrokeProperty = DependencyProperty.Register("RootLineStroke", typeof(Brush), typeof(TreeViewAdv), new PropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnRootLineStrokeChanged)));

        /// <summary>
        /// This Property Determines whether the TreeViewAdv Lines should be Visible or not
        /// </summary>
        public static readonly DependencyProperty RootLineVisibilityProperty = DependencyProperty.Register("RootLineVisibility", typeof(Visibility), typeof(TreeViewAdv), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(OnRootLineVisibilityChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ExpanderVisibilityProperty = DependencyProperty.Register("ExpanderVisibility", typeof(Visibility), typeof(TreeViewAdv), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(OnExpanderVisibilityChanged)));

        /// <summary>
        /// This Property Determines whether the TreeView Drag Lines should be Visible or not
        /// </summary>
        public static readonly DependencyProperty DragLineVisibilityProperty = DependencyProperty.Register("DragLineVisibility", typeof(Visibility), typeof(TreeViewAdv), new PropertyMetadata(Visibility.Visible, new PropertyChangedCallback(OnDragLineVisibilityChanged)));

        /// <summary>
        /// This Property Determines whether the TreeViewAdv Lines should be Visible or not
        /// </summary>
        public static readonly DependencyProperty DragLineColorProperty = DependencyProperty.Register("DragLineColor", typeof(Brush), typeof(TreeViewAdv), new PropertyMetadata(new SolidColorBrush(Colors.Black), new PropertyChangedCallback(OnDragLineColorChanged)));

        /// <summary>
        /// This Property determines the Scrolling speed for the TreeViewAdv Control.
        /// </summary>
        public static readonly DependencyProperty ScrollingSpeedProperty = DependencyProperty.Register("ScrollingSpeed", typeof(double), typeof(TreeViewAdv), new PropertyMetadata(5d, new PropertyChangedCallback(OnScrollingSpeedChanged)));

        /// <summary>
        /// This property indicates the template for the Expander
        /// </summary>
        public static readonly DependencyProperty ExpanderTemplateProperty = DependencyProperty.Register("ExpanderTemplate", typeof(ControlTemplate), typeof(TreeViewAdv), new PropertyMetadata(null, new PropertyChangedCallback(OnExpanderTemplateChanged)));

        /// <summary>
        /// This Property determines the style for container
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(TreeViewAdv), new PropertyMetadata(null, OnItemContainerStyleChanged));

        #endregion

        #region DP Events

        /// <summary>
        /// Event that is raised when the SelectOnExpandChange Property is Changed.
        /// </summary>
        public event PropertyChangedCallback SelectOnExpandChangeChanged;

        /// <summary>
        /// Event that is raised when the SelectedItem Property is changed.
        /// </summary>
        public event PropertyChangedCallback SelectedItemChanged;

        /// <summary>
        /// Event that is raised when the SelectedNode Property is changed.
        /// </summary>
        public event PropertyChangedCallback SelectedNodeChanged;

        /// <summary>
        /// Event that is raised when the IsInEditMode Property is changed.
        /// </summary>
        public event PropertyChangedCallback IsInEditModeChanged;

        /// <summary>
        /// Event that is raised when the SortMode Property is changed.
        /// </summary>
        public event PropertyChangedCallback SortModeChanged;

        /// <summary>
        /// Event that is raised when the DraggingEnables Property is changed.
        /// </summary>
        public event PropertyChangedCallback DraggingEnabledChanged;

        /// <summary>
        /// Event that is raised when the IsMultiSelect Property is changed.
        /// </summary>
        public event PropertyChangedCallback IsMultiSelectChanged;

        /// <summary>
        /// Event that is raised when the SelectedItems Property is changed.
        /// </summary>
        public event PropertyChangedCallback SelectedNodesChanged;

        /// <summary>
        /// Event that is raised when the SelectedItems Property is changed.
        /// </summary>
        public event PropertyChangedCallback SelectedItemsChanged;
        
        /// <summary>
        /// Event that is raised when the LineStrokeArray Property is changed.
        /// </summary>
        public event PropertyChangedCallback LineStrokeArrayChanged;

        /// <summary>
        /// Event that is raised when the LineStroke Property is changed.
        /// </summary>
        public event PropertyChangedCallback RootLineStrokeChanged;

        /// <summary>
        /// Occurs when [root line visibility changed].
        /// </summary>
        public event PropertyChangedCallback RootLineVisibilityChanged;

        /// <summary>
        /// Occurs when [drag line visibility changed].
        /// </summary>
        public event PropertyChangedCallback DragLineVisibilityChanged;

        /// <summary>
        /// Occurs when [drag line color changed].
        /// </summary>
        public event PropertyChangedCallback DragLineColorChanged;

        /// <summary>
        /// Event that is raised when the Theme Property is changed.
        /// </summary>
        public event PropertyChangedCallback ThemeChanged;

        /// <summary>
        /// Event that is raised when the Text Property is changed.
        /// </summary>         
        public event PropertyChangedCallback ScrollingSpeedChanged;

        /// <summary>
        /// Occurs when [toggle button template changed].
        /// </summary>
        public event PropertyChangedCallback ExpanderTemplateChanged;
        
   
        #endregion

        #region Fields

        private const string ElementScrollViewerName = "ScrollViewer";
        internal ScrollViewer elementScrollViewer;
        internal Canvas treeviewcanvas;
        internal Grid treeviewgrid;
        private bool isselectionChangeActive;
        private TreeViewItemAdv lastContainerCheck;
        bool scrollFlag = false;
        
        private Popup popup;
        private TextBlock txtPopupDet;
        internal TreeViewItemAdv Previousmouseoveritem1 = null;
        internal TreeViewItemAdv Previousmouseoveritem = null;
        internal bool IsPressed { get; private set; }
        internal int ClickCount { get; private set; }
        private DateTime LastClickTime { get; set; }
        private Point LastClickPosition { get; set; }
        private const double Milliseconds = 500.0;
        private const double PixelsSquared = 3.0 * 3.0;
        internal DispatcherTimer timer;
        Dictionary<string, TreeViewItemAdv> treeViewItemsCollection;
        internal bool lostFocus = false;
        
        bool searchflag = false;
        
        private Point lastDragPosition;
        private Point lastmouseposition;
        Point pos;
        internal TreeViewItemAdv mouseoveritem = null;
        internal Visibility ItemExpanderVisibility = Visibility.Collapsed;
        

        #region Collections

        bool dragging = false;
        DropMode dropmode;
        TreeViewItemAdv trackTreeviewitem = null;
        bool mouseLButtonDown = false;
        List<TreeViewItemAdv> targetListcollection = new List<TreeViewItemAdv>();

        List<TreeViewItemAdv> source_Selected_TVItems_Parent;
        List<TreeViewItemAdv> source_Selected_TVItems;
        List<TreeViewAdv> source_TreeView;

        TreeViewItemAdv target_Selected_TVItem;
        TreeViewItemAdv target_Selected_TVItem_Parent;
        TreeViewAdv target_TreeView;
        #endregion


        /// <summary>
        /// 
        /// </summary>
        public bool FullRowSelect
        {
            get { return (bool)GetValue(FullRowSelectProperty); }
            set { SetValue(FullRowSelectProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for FullRowSelect.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty FullRowSelectProperty =
            DependencyProperty.Register("FullRowSelect", typeof(bool), typeof(TreeViewAdv), new PropertyMetadata(false, new PropertyChangedCallback(OnFullRowSelectChanged)));//,new PropertyChangedCallback(OnFullRowSelectChanged)));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnFullRowSelectChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((TreeViewAdv)obj != null)
            {
                ((TreeViewAdv)obj).OnFullRowSelectChanged(args);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected void OnFullRowSelectChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.SelectedItems.Count > 1)
            {
                foreach (var item in this.SelectedNodes)
                {
                    if (item != null)
                        item.RefreshFullRowSelect();
                }
            }
            else
            {
                if (this.SelectedNode != null)
                    this.SelectedNode.RefreshFullRowSelect();
            }
        }

        /// <summary>
        /// Gets the root.
        /// </summary>
        /// <value>The root.</value>
        internal TreeViewItemAdv root
        {
            get
            {
                if (this.ItemsSource == null)
                {
                    if (this.Items.Count > 0)
                    {
                        return (TreeViewItemAdv)this.Items[0];
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return (TreeViewItemAdv)this.ItemContainerGenerator.ContainerFromItem(this.Items[0] as object);
                }
            }
        }

        #endregion Fields

        #region Constructors
        /// <summary>
        /// Initializes the TreeViewAdv Control
        /// </summary>
        public TreeViewAdv()
        {
            this.TabNavigation = KeyboardNavigationMode.Once;
            DefaultStyleKey = typeof(TreeViewAdv);
            this.Nodes = new List<TreeViewItemAdv>();
            this.IsTabStop = false;
            this.SelectedNodes = new List<TreeViewItemAdv>();
            this.SelectedItems = new List<object>();
            ItemContGenerator = new ItemContainerGeneratorAdv(this);
            
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer.Tick += new EventHandler(timer_Tick);

            AutoSearchTimer= new DispatcherTimer();
            AutoSearchTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            AutoSearchTimer.Tick += new EventHandler(AutoSearchTimer_Tick);

          
            this.Loaded += new RoutedEventHandler(TreeViewAdv_Loaded);
            this.Unloaded += new RoutedEventHandler(TreeViewAdv_Unloaded);
        }

        void TreeViewAdv_Loaded(object sender, RoutedEventArgs e)
        {
            if (!applyHTML_Dom_Event)
            {
                if (HtmlPage.IsEnabled)
                {
                    HtmlPage.Window.AttachEvent("DOMMouseScroll", OnScroll);
                    HtmlPage.Window.AttachEvent("onmousewheel", OnScroll);
                    HtmlPage.Document.AttachEvent("onmousewheel", OnScroll);
                    applyHTML_Dom_Event = true;
                }
            }
            Application.Current.RootVisual.MouseLeave += new MouseEventHandler(RootVisual_MouseLeave);
            if (layoutRoot != null)
            {
                layoutRoot.MouseLeftButtonUp += new MouseButtonEventHandler(treeviewcanvas_MouseLeftButtonUp);
                layoutRoot.MouseMove += new MouseEventHandler(treeviewcanvas_MouseMove);
            }
            this.MouseEnter += new MouseEventHandler(TreeViewAdv_MouseEnter);
            this.MouseLeave += new MouseEventHandler(TreeViewAdv_MouseLeave);
            this.LayoutUpdated += new EventHandler(TreeViewAdv_LayoutUpdated);
        }

        void TreeViewAdv_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this != null && !m_layOutUpdated)
            {
                m_layOutUpdated = true;
                if (this.RootLineVisibility == System.Windows.Visibility.Visible)
                    RefreshRootLines(this);
            }
            if (HtmlPage.IsEnabled)
            {
                HtmlPage.Window.DetachEvent("DOMMouseScroll", OnScroll);
                HtmlPage.Window.DetachEvent("onmousewheel", OnScroll);
                HtmlPage.Document.DetachEvent("onmousewheel", OnScroll);
            }

            Nodes.Clear();
           
            ItemContGenerator.ClearItems();
            this.MouseEnter -= new MouseEventHandler(TreeViewAdv_MouseEnter);
            this.MouseLeave -= new MouseEventHandler(TreeViewAdv_MouseLeave);
            this.LayoutUpdated -= new EventHandler(TreeViewAdv_LayoutUpdated);          

            if(Application.Current.RootVisual != null)
            Application.Current.RootVisual.MouseLeave -= new MouseEventHandler(RootVisual_MouseLeave);

            if (layoutRoot != null)
            {
                layoutRoot.MouseLeftButtonUp -= new MouseButtonEventHandler(treeviewcanvas_MouseLeftButtonUp);
                layoutRoot.MouseMove -= new MouseEventHandler(treeviewcanvas_MouseMove);
            }
           
             timer.Tick += new EventHandler(timer_Tick);
             AutoSearchTimer.Tick += new EventHandler(AutoSearchTimer_Tick);
        }
        void AutoSearchTimer_Tick(object sender, EventArgs e)
        {
            AutoSearchTimer.Stop();
        }


        void TreeViewAdv_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!this.ShouldAlwaysVisibleExpander)
            {
                ItemExpanderVisibility = Visibility.Collapsed;
                RefreshToogleButton();
            }
        }

        void TreeViewAdv_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!ShouldAlwaysVisibleExpander)
            {
                ItemExpanderVisibility = Visibility.Visible; ;
                RefreshToogleButton();
            }
           
         
        }

        /// <summary>
        /// Handles the LostFocus event of the TreeViewAdv control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void TreeViewAdv_LostFocus(object sender, RoutedEventArgs e)
        {
            //dragging = false;
            //if (popup != null)
            //{
            //    popup.IsOpen = false;
            //    trackTreeviewitem = null;
            //    mouseLButtonDown = false;
            //}
        }
        private bool m_layOutUpdated = false;
        private bool applyHTML_Dom_Event = false;
        /// <summary>
        /// Handles the LayoutUpdated event of the TreeViewAdv control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void TreeViewAdv_LayoutUpdated(object sender, EventArgs e)
        {
            if (this != null && !m_layOutUpdated)
            {
                m_layOutUpdated = true;
                if (this.RootLineVisibility == System.Windows.Visibility.Visible)
                    RefreshRootLines(this);
            }
        }

        void RootVisual_MouseLeave(object sender, MouseEventArgs e)
        {
            dragging = false;
            if (popup != null)
            {
                popup.IsOpen = false;
                trackTreeviewitem = null;
                mouseLButtonDown = false;
            }    
        }

        /// <summary>
        /// Initializes the <see cref="TreeViewAdv"/> class.
        /// </summary>
        static TreeViewAdv()
        {
            if (System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                Syncfusion.Windows.Shared.LoadDependentAssemblies load = new Syncfusion.Windows.Shared.LoadDependentAssemblies();
                load = null;
            }

        } 
        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [drag on text].
        /// </summary>
        /// <value><c>true</c> if [drag on text]; otherwise, <c>false</c>.</value>
        public bool DragOnText
        {
            get { return (bool)GetValue(DragOnTextProperty); }
            set { SetValue(DragOnTextProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty DragOnTextProperty =
            DependencyProperty.Register("DragOnText", typeof(bool), typeof(TreeViewAdv), new PropertyMetadata(true));


        /// <summary>
        /// Gets or sets a value indicating whether [should always visible expander].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [should always visible expander]; otherwise, <c>false</c>.
        /// </value>
        public bool ShouldAlwaysVisibleExpander
        {
            get { return (bool)GetValue(ShowAlwaysProperty); }
            set { SetValue(ShowAlwaysProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ShowAlwaysProperty =
            DependencyProperty.Register("ShouldAlwaysVisibleExpander", typeof(bool), typeof(TreeViewAdv), new PropertyMetadata(false,OnShowAlwaysChanged));

        /// <summary>
        /// Called when [show always changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnShowAlwaysChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (((TreeViewAdv)obj) != null)
            {
                ((TreeViewAdv)obj).RefreshToogleButton();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is control key down.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is control key down; otherwise, <c>false</c>.
        /// </value>
        private static bool IsControlKeyDown
        {
            get
            {
                return (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is shift key down.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is shift key down; otherwise, <c>false</c>.
        /// </value>
        private static bool IsShiftKeyDown
        {
            get
            {
                return (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is drag.
        /// </summary>
        /// <value><c>true</c> if this instance is drag; otherwise, <c>false</c>.</value>
        bool IsDrag
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the SelectOnExpandChange dependency property.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [select on expand change]; otherwise, <c>false</c>.
        /// </value>
        public bool SelectOnExpandChange
        {
            get
            {
                return (bool)GetValue(SelectOnExpandChangeProperty);
            }

            set
            {
                SetValue(SelectOnExpandChangeProperty, value);
            }
        }

        /// <summary>
        /// Gets the value of the Nodes property.
        /// </summary>
        /// <value>The nodes.</value>
        public IList<TreeViewItemAdv> Nodes
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the value of the SelectedItem dependency property.
        /// </summary>
        public object SelectedItem
        {
            get
            {
                return (object)GetValue(SelectedItemProperty);
            }

            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the SelectedNode dependency property.
        /// </summary>
        /// <value>The selected node.</value>
        public TreeViewItemAdv SelectedNode
        {
            get
            {
                return (TreeViewItemAdv)GetValue(SelectedNodeProperty);
            }

            set
            {
                SetValue(SelectedNodeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the IsInEditMode dependency property.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is in edit mode; otherwise, <c>false</c>.
        /// </value>
        public bool IsInEditMode
        {
            get
            {
                return (bool)GetValue(IsInEditModeProperty);
            }

            set
            {
                SetValue(IsInEditModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the SortMode dependency property.
        /// </summary>
        public SortMode SortMode
        {
            get
            {
                return (SortMode)GetValue(SortModeProperty);
            }

            set
            {
                SetValue(SortModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the DraggingEnabled dependency property.
        /// </summary>
        public bool DraggingEnabled
        {
            get
            {
                return (bool)GetValue(DraggingEnabledProperty);
            }

            set
            {
                SetValue(DraggingEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the IsMultiSelect dependency property.
        /// </summary>
        [Obsolete("Please use SelectionMode Property")]
        public bool IsMultiSelect
        {
            get
            {
                return (bool)GetValue(IsMultiSelectProperty);
            }

            set
            {
                SetValue(IsMultiSelectProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the mouse hovered item.
        /// </summary>
        /// <value>The mouse hovered item.</value>
        public TreeViewItemAdv MouseHoveredItem
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the selected items.
        /// </summary>
        /// <value>The selected items.</value>
        public List<object> SelectedItems
        {
            get
            {
                return (List<object>)GetValue(SelectedItemsProperty);
            }

            set
            {
                SetValue(SelectedItemsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the SelectedItems dependency property.
        /// </summary>
        public List<TreeViewItemAdv> SelectedNodes
        {
            get
            {
                return (List<TreeViewItemAdv>)GetValue(SelectedNodesProperty);
            }

            set
            {
                SetValue(SelectedNodesProperty, value);
            }
        } 

        /// <summary>
        /// Gets or sets the value of the LineStrokeArray dependency property.
        /// </summary>
        public DoubleCollection LineStrokeArray
        {
            get
            {
                return (DoubleCollection)GetValue(LineStrokeArrayProperty);
            }

            set
            {
                SetValue(LineStrokeArrayProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value of the LineStroke dependency property.
        /// </summary>
        public Brush RootLineStroke
        {
            get
            {
                return (Brush)GetValue(RootLineStrokeProperty);
            }

            set
            {
                SetValue(RootLineStrokeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the root line visibility.
        /// </summary>
        /// <value>The root line visibility.</value>
        public Visibility RootLineVisibility
        {
            get
            {
                return (Visibility)GetValue(RootLineVisibilityProperty);
            }

            set
            {
                SetValue(RootLineVisibilityProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Visibility ExpanderVisibility
        {
            get { return (Visibility)GetValue(ExpanderVisibilityProperty); }
            set { SetValue(ExpanderVisibilityProperty, value); }
        }

        
        /// <summary>
        /// Gets or sets the drag line visibility.
        /// </summary>
        /// <value>The drag line visibility.</value>
        public Visibility DragLineVisibility
        {
            get
            {
                return (Visibility)GetValue(DragLineVisibilityProperty);
            }

            set
            {
                SetValue(DragLineVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the drag line.
        /// </summary>
        /// <value>The color of the drag line.</value>
        public Brush DragLineColor
        {
            get
            {
                return (Brush)GetValue(DragLineColorProperty);
            }

            set
            {
                SetValue(DragLineColorProperty, value);
            }
        }

        

        /// <summary>
        /// Gets or sets the scrolling speed.
        /// </summary>
        /// <value>The scrolling speed.</value>
        public double ScrollingSpeed
        {
            get
            {
                return (double)GetValue(ScrollingSpeedProperty);
            }

            set
            {
                SetValue(ScrollingSpeedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the ExpanderTemplate Dependency Property
        /// </summary>
        public ControlTemplate ExpanderTemplate
        {
            get
            {
                return (ControlTemplate)base.GetValue(ExpanderTemplateProperty);
            }

            set
            {
                base.SetValue(ExpanderTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicates the style for the container(TreeViewItemAdv)
        /// </summary>
        public Style ItemContainerStyle
        {
            get
            {
                return (Style)GetValue(ItemContainerStyleProperty);
            }

            set
            {
                SetValue(ItemContainerStyleProperty, value);
            }
        }
        #endregion Properties

        #region DP event implementation
        /// <summary>
        /// Calls OnIsInEditModeChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnIsInEditModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewAdv obj = (TreeViewAdv)d;
            obj.OnIsInEditModeChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="IsInEditModeChanged"/> event.
        /// </summary>
        protected virtual void OnIsInEditModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsInEditModeChanged != null)
            {
                IsInEditModeChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnSelectOnExpandChangeChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnSelectOnExpandChangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewAdv obj = (TreeViewAdv)d;
            obj.OnSelectOnExpandChangeChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="SelectOnExpandChangeChanged"/> event.
        /// </summary>
        private void OnSelectOnExpandChangeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectOnExpandChangeChanged != null)
            {
                SelectOnExpandChangeChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnSelectedItemChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewAdv obj = (TreeViewAdv)d;
            obj.OnSelectedItemChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="SelectedItemChanged"/> event.
        /// </summary>
        private void OnSelectedItemChanged(DependencyPropertyChangedEventArgs e)
        {
            this.CheckSelection();            
            if (SelectedItemChanged != null)
            {
                SelectedItemChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnSelectedNodeChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnSelectedNodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewAdv obj = (TreeViewAdv)d;
            obj.OnSelectedNodeChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="SelectedNodeChanged"/> event.
        /// </summary>
        private void OnSelectedNodeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                TreeViewItemAdv item = e.OldValue as TreeViewItemAdv;
                if (null != item && e.NewValue != null)
                {
                    if (!item.IsMultiSelect)
                    {
                        item.IsMouseOver = false;
                        item.IsSelected = false;
                    }
                }
            }
            if (e.NewValue == null)
            {
                ((TreeViewItemAdv)e.OldValue).IsSelected = false;
            }
            else
            {
                ((TreeViewItemAdv)e.NewValue).IsSelected = true;
            }

            if (SelectedNodeChanged != null)
            {
                SelectedNodeChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnSortModeChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnSortModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewAdv obj = (TreeViewAdv)d;
            obj.OnSortModeChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="SortModeChanged"/> event.
        /// </summary>
        private void OnSortModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SortModeChanged != null)
            {
                SortModeChanged(this, e);
            }

            Sort();
        }

        /// <summary>
        /// Calls OnDraggingEnabledChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnDraggingEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewAdv obj = (TreeViewAdv)d;
            obj.OnDraggingEnabledChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="DraggingEnabledChanged"/> event.
        /// </summary>
        private void OnDraggingEnabledChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DraggingEnabledChanged != null)
            {
                DraggingEnabledChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnIsMultiSelectChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnIsMultiSelectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewAdv obj = (TreeViewAdv)d;
            obj.OnIsMultiSelectChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="IsMultiSelectChanged"/> event.
        /// </summary>
        private void OnIsMultiSelectChanged(DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false)
            {
                List<TreeViewItemAdv> temp = new List<TreeViewItemAdv>();
                for (int i = 0; i < this.SelectedNodes.Count; i++)
                {
                    this.SelectedNodes[i].IsMultiSelect = false;
                    if (i != 0)
                    {
                        this.SelectedNodes[i].IsSelected = false;
                    }
                    else
                    {
                        this.SelectedItem = this.SelectedItems[i];
                        this.SelectedNode = this.SelectedNodes[i];
                    }
                }
                this.SelectedItems.Clear();
                this.SelectedNodes.Clear();
                //this.SelectedNodes = new List<TreeViewItemAdv>();
                //this.SelectedItems = new List<object>();
            }

            if (IsMultiSelectChanged != null)
            {
                IsMultiSelectChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnSelectedItemsChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnSelectedNodesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewAdv obj = (TreeViewAdv)d;
            obj.OnSelectedNodesChanged(e);
        }

         /// <summary>
        /// Calls OnSelectedItemsChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewAdv obj = (TreeViewAdv)d;
            obj.OnSelectedItemsChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="SelectedItemsChanged"/> event.
        /// </summary>
        private void OnSelectedItemsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectedItemsChanged != null)
            {
                SelectedItemsChanged(this, e);
            }
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="SelectedItemsChanged"/> event.
        /// </summary>
        private void OnSelectedNodesChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectedNodesChanged != null)
            {
                SelectedNodesChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnLineStrokeArrayChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnLineStrokeArrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewAdv obj = (TreeViewAdv)d;
            obj.OnLineStrokeArrayChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="LineStrokeArrayChanged"/> event.
        /// </summary>
        protected virtual void OnLineStrokeArrayChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LineStrokeArrayChanged != null)
            {
                LineStrokeArrayChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnLineStrokeChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnRootLineStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewAdv obj = (TreeViewAdv)d;
            obj.OnRootLineStrokeChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="RootLineStroke"/> event.
        /// </summary>
        protected virtual void OnRootLineStrokeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (RootLineStrokeChanged != null)
            {
                RootLineStrokeChanged(this, e);
            }

            RefreshRootLines(this);
        }

        /// <summary>
        /// Called when [root line visibility changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnRootLineVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewAdv obj = (TreeViewAdv)d;
            //obj.RootLineVisibility = Visibility.Collapsed;
            obj.OnRootLineVisibilityChanged(e);
        }

        private static void OnExpanderVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewAdv obj = (TreeViewAdv)d;
            obj.RefreshToogleButton();
        }

        /// <summary>
        /// Raises the <see cref="E:RootLineVisibilityChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnRootLineVisibilityChanged(DependencyPropertyChangedEventArgs e)
        {
            if (RootLineVisibilityChanged != null)
            {
                RootLineVisibilityChanged(this, e);
            }

            RefreshRootLines(this);
        }

        /// <summary>
        /// Called when [drag line visibility changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDragLineVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewAdv obj = (TreeViewAdv)d;
            obj.OnDragLineVisibilityChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:DragLineVisibilityChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnDragLineVisibilityChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DragLineVisibilityChanged != null)
            {
                DragLineVisibilityChanged(this, e);
            }

            RefreshRootLines(this);
        }

        /// <summary>
        /// Called when [drag line color changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDragLineColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewAdv obj = (TreeViewAdv)d;
            obj.OnDragLineColorChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:DragLineColorChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnDragLineColorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DragLineColorChanged != null)
            {
                DragLineColorChanged(this, e);
            }

            RefreshRootLines(this);
        }

        /// <summary>
        /// Called when [scrolling speed changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnScrollingSpeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewAdv obj = (TreeViewAdv)d;
            obj.OnScrollingSpeedChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ScrollingSpeedChanged"/> event.
        /// </summary>
        protected virtual void OnScrollingSpeedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ScrollingSpeedChanged != null)
            {
                ScrollingSpeedChanged(this, e);
            }
        }

        /// <summary>
        /// Calls OnExpanderTemplateChanged method of the instance,
        /// notifies of the dependency property value changes.
        /// </summary>
        private static void OnExpanderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewAdv source = (TreeViewAdv)d;
            source.OnExpanderTemplateChanged(e);
        }

        /// <summary>
        /// Updates property value cache and raises <see cref="ExpanderTemplateChanged"/> event.
        /// </summary>
        protected virtual void OnExpanderTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ExpanderTemplateChanged != null)
            {
                ExpanderTemplateChanged(this, e);
            }
        }

        /// <summary>
        /// Called when [item container style changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnItemContainerStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TreeViewAdv source = d as TreeViewAdv;
            Style value = e.NewValue as Style;
            source.ItemContGenerator.UpdateItemContainerStyle(value);
        }

        /// <summary>
        /// Gets or sets the item cont generator.
        /// </summary>
        /// <value>The item cont generator.</value>
        public ItemContainerGeneratorAdv ItemContGenerator
        {
            get;
            private set;
        }
        #endregion

        #region Search
        TreeViewItemAdv gblTreeNode;
        ////search through content

        TreeViewItemAdv tempsearchnode = null;
        internal bool searchfocus = false;
        /// <summary>
        /// Searches the specified STR node.
        /// </summary>
        /// <param name="strNode">The STR node.</param>
        public void Search(string strNode)
        {
            searchflag = true;
            searchfocus = true;
            if (this.Items.Count > 0)
            {
                object temp1 = this.Items[0];
                if (temp1 is TreeViewItemAdv)
                {
                    ClearSelectedNodes();
                    ItemCollection tncoll = this.Items;
                    FindInTreeView(tncoll, strNode);
                    if (tempsearchnode != null)
                    {
                        ClearSelectedNodes();
                        ChangeSelectedState(tempsearchnode);
                        tempsearchnode.Select(true);
                        tempsearchnode.IsSelected = true;
                        this.SelectedItem = tempsearchnode;
                    }
                }
            }
        }

        /// <summary>
        /// Finds the in tree view.
        /// </summary>
        /// <param name="tncoll">The tncoll.</param>
        /// <param name="strNode">The STR node.</param>
        private void FindInTreeView(ItemCollection tncoll, string strNode)
        {
            if (tncoll == null && strNode == null)
            {
                return;
            }

            foreach (TreeViewItemAdv tnode in tncoll)
            {
                if (tnode.Items.Count > 0)
                {
                    tnode.IsExpanded = true;
                    if (tnode.expander != null)
                    {
                        tnode.expander.IsChecked = true;
                    }
                }

                tnode.IsSelected = false;

                if (!(tnode.Header is string))
                {
                    return;
                }

                if (tnode.Header.ToString().ToLower() == strNode.ToLower())
                {
                    tempsearchnode = tnode;
                }
                else
                {
                }

                if (tnode.Items.Count > 0)
                {
                    FindInTreeView(tnode.Items, strNode);
                }
            }

            return;
        }

        /// <summary>
        /// Method used for searching particular node  through the TreeViewAdv control. 
        /// </summary>
        /// <param name="strNode">Indicates Node to be searched</param>
        public void Search(object strNode)
        {
            if (strNode is TreeViewItemAdv)
            {
                ClearSelectedNodes();
                searchfocus = true;
                ItemCollection tncoll = this.Items;
                FindInTreeView(tncoll, (TreeViewItemAdv)strNode);
                if (tempsearchnode != null)
                {
                    ClearSelectedNodes();
                    ChangeSelectedState(tempsearchnode);
                    tempsearchnode.Select(true);
                    tempsearchnode.IsSelected = true;
                    this.SelectedItem = tempsearchnode;
                }
            }
        }

        /// <summary>
        /// Finds the in tree view.
        /// </summary>
        /// <param name="tncoll">The tncoll.</param>
        /// <param name="strNode">The STR node.</param>
        private void FindInTreeView(ItemCollection tncoll, TreeViewItemAdv strNode)
        {
            if (tncoll == null || strNode == null)
            {
                return;
            }

            foreach (TreeViewItemAdv tnode in tncoll)
            {
                if (tnode.Items.Count > 0)
                {
                    tnode.IsExpanded = true;
                    if (tnode.expander != null)
                    {
                        tnode.expander.IsChecked = true;
                    }
                }

                tnode.IsSelected = false;
                //if ( tnode == strNode)
                if (object.ReferenceEquals(tnode, strNode))
                {
                    tempsearchnode = tnode;
                }
                else
                {
                    Deselect(tnode);
                }

                if (tnode.Items.Count > 0)
                {
                    FindInTreeView(tnode.Items, strNode);
                }
            }

            return;
        }

        // for searching multiple occurences of items
        /// <summary>
        /// Recursives the traversal.
        /// </summary>
        /// <param name="currentNode">The current node.</param>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        TreeViewItemAdv RecursiveTraversal(TreeViewItemAdv currentNode, string text)
        {
            TreeViewItemAdv returnNode = null;
            if (currentNode.Header.ToString() == text)
            {
                gblTreeNode = currentNode;
                returnNode = currentNode;
            }
            else
            {
                foreach (TreeViewItemAdv traversedNode in currentNode.Items)
                {
                    if (!(traversedNode.Header is string))
                    {
                        return null;
                    }

                    if (traversedNode.Header.ToString() == text)
                    {
                        returnNode = traversedNode;
                        gblTreeNode = traversedNode;
                        break;
                    }

                    if (traversedNode.Items.Count > 0)
                    {
                        RecursiveTraversal(traversedNode, text);
                    }
                }
            }

            return returnNode;
        }

        /// <summary>
        /// Searches the node.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        TreeViewItemAdv SearchNode(string text)
        {
            TreeViewItemAdv returnNode = null;
            gblTreeNode = null;
            foreach (TreeViewItemAdv tnode in this.Items)
            {
                returnNode = RecursiveTraversal(tnode, text);
                if (gblTreeNode != null)
                {
                    break;
                }
            }

            return gblTreeNode;
        }
        #endregion

        #region sorting
        /// <summary>
        /// Sortnodes the specified tncoll.
        /// </summary>
        /// <param name="tncoll">The tncoll.</param>
        private void sortnode(ItemCollection tncoll)
        {
            foreach (TreeViewItemAdv tnode in tncoll)
            {
                this.Sort(tnode, false);
                sortnode(tnode.Items);
            }
        }
       

        /// <summary>
        /// Method used for sorting the nodes in the TreeViewAdv control
        /// </summary>
        public void Sort()
        {
            if (this.ItemsSource == null)
            {
                if (this.Items.Count > 0)
                {
                    TreeViewItemAdv selection = this.SelectedNode;
                    List<TreeViewItemAdv> multiselection = new List<TreeViewItemAdv>();
                    foreach (TreeViewItemAdv pselection in this.SelectedNodes)
                    {
                        multiselection.Add(pselection);
                    }

                    SortTreeView();
                    ItemCollection tncoll = this.Items;
                    sortnode(tncoll);
                    this.ClearSelectedNodes();

                    if (multiselection.Count == 0)
                    {
                        if (this.SelectedNode != null)
                        {
                            this.SelectedItem = selection;
                            this.SelectedNode = selection;
                            this.SelectedNode.IsSelected = true;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < multiselection.Count; i++)
                        {
                            this.SelectedNodes.Add(multiselection[i]);
                            this.SelectedItems.Add(multiselection[i].DataContext ?? multiselection[i]);
                            this.SelectedNode = multiselection[i];
                            this.SelectedNode.IsSelected = true;
                            this.SelectedNode.IsMultiSelect = true;
                        }
                    }
                }
            }

            //    PagedCollectionView coll = new PagedCollectionView(this.ItemsSource);
            //    coll.SortDescriptions.Add(new SortDescription
            //    {
            //        Direction = ListSortDirection.Descending,
            //        PropertyName = "myString"
            //    });
            //    this.ItemsSource = coll;

        }

        /// <summary>
        /// Sorts the tree view.
        /// </summary>
        private void SortTreeView()
        {
            if (this.SortMode == SortMode.Asc)
            {
                SortTreeViewAsc(this);
            }
            else
            {
                SortTreeViewDesc(this);
            }
        }

        /// <summary>
        /// Method used for sorting the Children of particular Node
        /// </summary>
        /// <param name="treeNode">Indicates the Node whose children will be sorted</param>
        /// <param name="includeSubTrees">Indicates that sub nodes are also sorted</param>
        public void Sort(object treeNode, bool includeSubTrees)
        {
            if (treeNode is TreeViewItemAdv)
            {
                if (this.SortMode == SortMode.Asc&& !includeSubTrees)
                {
                    SortAsc((TreeViewItemAdv)treeNode);
                }
                else if (this.SortMode == SortMode.Asc && includeSubTrees)
                {
                    SortAsc((TreeViewItemAdv)treeNode, true);
                }
                else if (this.SortMode == SortMode.Desc && !includeSubTrees)
                {
                    SortDesc((TreeViewItemAdv)treeNode);
                }
                else
                {
                    SortDesc((TreeViewItemAdv)treeNode, true);
                }

                this.ToggleNode(treeNode as TreeViewItemAdv, true);
                this.SelectedItem = treeNode as TreeViewItemAdv;
                this.SelectedNode = treeNode as TreeViewItemAdv;
                this.SelectedNode.IsSelected = true;
                RefreshRootLines(this);
            }
        }
        ////Sorting TreeViewitems with SubTrees
        /// <summary>
        /// Sorts the asc.
        /// </summary>
        /// <param name="treeNode">The tree node.</param>
        /// <param name="subTrees">if set to <c>true</c> [sub trees].</param>
        private void SortAsc(TreeViewItemAdv treeNode, bool subTrees)
        {
            if (subTrees)
            {
                for (int i = 0; i < treeNode.Items.Count; i++)
                {
                    TreeViewItemAdv temp=treeNode.Items[i] as TreeViewItemAdv;
                    if (temp.Items.Count>0)
                    {
                        SortAsc(temp);
                        SortAsc(temp, true);
                    }
                }

                SortAsc(treeNode);
            }
        }

        ////Sorting TreeViewitems with SubTrees
        /// <summary>
        /// Sorts the desc.
        /// </summary>
        /// <param name="treeNode">The tree node.</param>
        /// <param name="subTrees">if set to <c>true</c> [sub trees].</param>
        private void SortDesc(TreeViewItemAdv treeNode, bool subTrees)
        {
            if (subTrees)
            {
                for (int i = 0; i < treeNode.Items.Count; i++)
                {
                    TreeViewItemAdv temp = treeNode.Items[i] as TreeViewItemAdv;
                    if (temp.Items.Count > 0)
                    {
                        SortDesc(temp);
                        SortDesc(temp, true);
                    }
                }

                SortDesc(treeNode);
            }
        }
        
        ////Sorting TreeViewitems without including SubTrees
        /// <summary>
        /// Sorts the asc.
        /// </summary>
        /// <param name="treeNode">The tree node.</param>
        private void SortAsc(TreeViewItemAdv treeNode)
        {
            bool mExpand=false;
            if (treeNode != null)
            {
                mExpand = treeNode.IsExpanded;

                List<TreeViewItemAdv> mTreeNode = new List<TreeViewItemAdv>();

                mTreeNode = treeNode.Items.Cast<TreeViewItemAdv>().ToList();
               
                var mTreeCollection = from c in mTreeNode orderby c.Header select c;

                treeNode.Items.Clear();

                foreach (TreeViewItemAdv pTreeNode in mTreeCollection)
                {
                    treeNode.Items.Add(pTreeNode);
                }

                if (mExpand)
                {
                    if (treeNode.Items.Count > 0)
                    {
                        treeNode.IsExpanded = true;
                        if (treeNode.expander != null)
                        {
                            treeNode.expander.IsChecked = true;
                        }
                    }
                }
                else
                {
                    if (treeNode.Items.Count > 0)
                    {
                        treeNode.IsExpanded = false;
                        if (treeNode.expander != null)
                        {
                            treeNode.expander.IsChecked = false;
                        }
                    }
                }            
            }
        }

        /// <summary>
        /// Sorts the desc.
        /// </summary>
        /// <param name="treeNode">The tree node.</param>
        private void SortDesc(TreeViewItemAdv treeNode)
        {
            bool mExpand = false;
            if (treeNode != null)
            {
                mExpand = treeNode.IsExpanded;
                List<TreeViewItemAdv> mTreeNode = new List<TreeViewItemAdv>();

                mTreeNode = treeNode.Items.Cast<TreeViewItemAdv>().ToList();

                var mTreeCollection = from c in mTreeNode orderby c.Header descending select c;

                treeNode.Items.Clear();

                foreach (TreeViewItemAdv pTreeNode in mTreeCollection)
                {
                    treeNode.Items.Add(pTreeNode);
                }

                if (mExpand)
                {
                    if (treeNode.Items.Count > 0)
                    {
                        treeNode.IsExpanded = true;
                        if (treeNode.expander != null)
                        {
                            treeNode.expander.IsChecked = true;
                        }
                    }
                }
                else
                {
                    if (treeNode.Items.Count > 0)
                    {
                        treeNode.IsExpanded = false;
                        if (treeNode.expander != null)
                        {
                            treeNode.expander.IsChecked = false;
                        }
                    }
                }             
            }
        }

        ////Sorting TreeViewAdv
        /// <summary>
        /// Sorts the tree view asc.
        /// </summary>
        /// <param name="treeNode">The tree node.</param>
        private void SortTreeViewAsc(TreeViewAdv treeNode)
        {
            if (treeNode != null)
            {
                List<TreeViewItemAdv> mTreeNode = new List<TreeViewItemAdv>();

                for (int i = 0; i < treeNode.Items.Count; i++)
                {
                    mTreeNode.Add(treeNode.Items[i] as TreeViewItemAdv);
                }

                var mTreeCollection = from c in mTreeNode orderby c.Header select c;

                treeNode.Items.Clear();

                foreach (TreeViewItemAdv pTreeNode in mTreeCollection)
                {
                    treeNode.Items.Add(pTreeNode);
                }
            }
        }

        /// <summary>
        /// Sorts the tree view desc.
        /// </summary>
        /// <param name="treeNode">The tree node.</param>
        private void SortTreeViewDesc(TreeViewAdv treeNode)
        {
            if (treeNode != null)
            {
                List<TreeViewItemAdv> mTreeNode = new List<TreeViewItemAdv>();

                for (int i = 0; i < treeNode.Items.Count; i++)
                {
                    mTreeNode.Add(treeNode.Items[i] as TreeViewItemAdv);
                }

                var mTreeCollection = from c in mTreeNode orderby c.Header descending select c;

                treeNode.Items.Clear();
                foreach (TreeViewItemAdv pTreeNode in mTreeCollection)
                {
                    treeNode.Items.Add(pTreeNode);
                }
            }
        }
        #endregion

        #region expandcollapse
        /// <summary>
        /// Method that will expand the whole TreeViewAdv control
        /// </summary>
        public void ExpandAll()
        {
            if (this.Items.Count > 0)
            {
                object temp1 = this.Items[0];
                if (temp1 is TreeViewItemAdv)
                {
                    ItemCollection tncoll = this.Items;
                    Exp(tncoll);
                }
                else
                {
                    for (int i = 0; i < this.Items.Count; i++)
                    {
                        TreeViewItemAdv titem = this.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv;
                        UpdateExpandAll(titem);
                    }
                    
                    IDictionary<DependencyObject, object> tempdic = this.ItemContGenerator.ChildrenToItems;
                    Exp1(tempdic);
                }
            }
        }

        private void UpdateExpandAll(TreeViewItemAdv treeitem)
        {
            TreeViewItemAdv item = null;
            for (int i = 0; i < treeitem.Items.Count; i++)
            {
                item = treeitem.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv;
                if (item == null && !treeitem.IsExpanded)
                {
                    treeitem.IsExpanded = true;
                    UpdateLayout();
                }
                item = treeitem.ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv;
                if (item != null && !item.IsExpanded && item.Items.Count > 0)
                {
                    item.IsExpanded = true;
                    for (int j = 0; j < item.Items.Count; j++)
                    {
                        TreeViewItemAdv subItem = item.ItemContainerGenerator.ContainerFromIndex(j) as TreeViewItemAdv;
                        if (subItem == null)
                        {
                            UpdateLayout();
                        }
                        subItem = item.ItemContainerGenerator.ContainerFromIndex(j) as TreeViewItemAdv;
                        if(subItem != null)
                        UpdateExpandAll(subItem);
                    }
                }
            }         
        }

        /// <summary>
        /// Method that will expand the particular Node of the TreeViewAdv Control.
        /// </summary>
        /// <param name="item">Indicates the Node, which is to be expanded</param>
        public void Expand(object item)
        {
            if (item is TreeViewItemAdv)
            {
                Exp(((TreeViewItemAdv)item).Items);
            }
            else
            {
                IDictionary<DependencyObject, object> tempdic = ((TreeViewItemAdv)item).ContainersToItems;
                Exp1(tempdic);
            }
        }

        /// <summary>
        /// Exps the specified tncoll.
        /// </summary>
        /// <param name="tncoll">The tncoll.</param>
        private void Exp(ItemCollection tncoll)
        {
            foreach (TreeViewItemAdv tnode in tncoll)
            {
                if (tnode.Items.Count > 0)
                {
                    tnode.IsExpanded = true;
                    if (tnode.expander != null)
                    {
                        tnode.expander.IsChecked = true;
                    }
                }

                Exp(tnode.Items);
            }
        }

        /// <summary>
        /// Exp1s the specified tncoll.
        /// </summary>
        /// <param name="tncoll">The tncoll.</param>
        private void Exp1(IDictionary<DependencyObject, object> tncoll)
        {
            foreach (KeyValuePair<DependencyObject, object> keyvaluepair in tncoll)
            {
                if (((TreeViewItemAdv)keyvaluepair.Key).Items.Count > 0)
                {
                    ((TreeViewItemAdv)keyvaluepair.Key).IsExpanded = false;
                    ((TreeViewItemAdv)keyvaluepair.Key).IsExpanded = true;
                    if (((TreeViewItemAdv)keyvaluepair.Key).expander != null)
                    {
                        ((TreeViewItemAdv)keyvaluepair.Key).expander.IsChecked = true;
                        ((TreeViewItemAdv)keyvaluepair.Key).HasItems = true;
                        ((TreeViewItemAdv)keyvaluepair.Key).itemsHost.Visibility = Visibility.Visible;
                    }
                }

                TreeViewItemAdv itm = (TreeViewItemAdv)keyvaluepair.Key;
                Exp1(((TreeViewItemAdv)keyvaluepair.Key).ContainersToItems);
            }
        }

        private void Exp1(ItemCollection tncoll)
        {
            foreach (TreeViewItemAdv tnode in tncoll)
            {
                if (tnode.Items.Count > 0)
                {
                    tnode.IsExpanded = true;
                    tnode.expander.IsChecked = true;
                }

                Exp1(tnode.Items);
            }
        }


        /// <summary>
        /// Method that will Collapse the whole TreeViewAdv control
        /// </summary>
        public void CollapseAll()
        {
            if (this.Items.Count > 0)
            {
                object temp1 = this.Items[0];
                if (temp1 is TreeViewItemAdv)
                {
                    ItemCollection tncoll = this.Items;
                    Col(tncoll);
                }
                else
                {
                    IDictionary<DependencyObject, object> tempdic = this.ItemContGenerator.ChildrenToItems;
                    Col1(tempdic);
                }
            }
        }

        /// <summary>
        /// Method that will Collapse the particular Node of the TreeViewAdv Control.
        /// </summary>
        /// <param name="item">Indicates the Node, which is to be Collapsed</param>
        public void Collapse(object item)
        {
            if (item is TreeViewItemAdv)
            {
                Col(((TreeViewItemAdv)item).Items);
            }
            else
            {
                IDictionary<DependencyObject, object> tempdic = this.ItemContGenerator.ChildrenToItems;
                Col1(tempdic);
            }
        }

        /// <summary>
        /// Cols the specified tncoll.
        /// </summary>
        /// <param name="tncoll">The tncoll.</param>
        private void Col(ItemCollection tncoll)
        {
            foreach (TreeViewItemAdv tnode in tncoll)
            {
                if (tnode.Items.Count > 0)
                {
                    tnode.IsExpanded = false;
                    if (tnode.expander != null)
                    {
                        tnode.expander.IsChecked = false;
                    }
                }

                Col(tnode.Items);
            }
        }

        /// <summary>
        /// Col1s the specified tncoll.
        /// </summary>
        /// <param name="tncoll">The tncoll.</param>
        private void Col1(IDictionary<DependencyObject, object> tncoll)
        {
            foreach (KeyValuePair<DependencyObject, object> keyvaluepair in tncoll)
            {
                if (((TreeViewItemAdv)keyvaluepair.Key).Items.Count > 0)
                {
                    ((TreeViewItemAdv)keyvaluepair.Key).IsExpanded = false;
                    if (((TreeViewItemAdv)keyvaluepair.Key).expander != null)
                    {
                        ((TreeViewItemAdv)keyvaluepair.Key).expander.IsChecked = false;
                    }
                }

                TreeViewItemAdv itm = (TreeViewItemAdv)keyvaluepair.Key;
                Col1(((TreeViewItemAdv)keyvaluepair.Key).ContainersToItems);
            }
        }

        #endregion

        #region Removing items
        TreeViewItemAdv rglobalitem = null;

        /// <summary>
        /// Method removes the selected node from the treeview control.
        /// </summary>
        /// <returns>return the boolean values indicates the successful removal of node from the TreeViewAdv</returns>
        public bool Remove()
        {
            TreeViewItemAdv rparenttreeviewitem = null;
            TreeViewAdv rparenttreeview = null;
            object item = this.SelectedItem;
            if (!(item is TreeViewItemAdv))
            {
                return false;
            }

            if (item == null)
            {
                return false;
            }
            else
            {
                rparenttreeviewitem = ((TreeViewItemAdv)item).ParentNode;
                rparenttreeview = ((TreeViewItemAdv)item).ParentTreeview;
                if (rparenttreeviewitem != null)
                {
                    bool flag = rparenttreeviewitem.Items.Remove(item);
                    if (flag == true)
                    {
                        if (rparenttreeviewitem.Items.Count > 0)
                        {
                            rparenttreeviewitem.HasItems = true;
                            RefreshRootLines(this);
                            rparenttreeviewitem.expander.IsChecked = true;
                        }
                    }
                }
                else if (rparenttreeview != null)
                {
                    bool flag = rparenttreeview.Items.Remove(item);
                }
            }

            return false;
        }

        /// <summary>
        /// Method Removes the node from the Treeview, where the node to be removed should be passed as a parameter.
        /// </summary>
        /// <param name="item">Indicates the Node to be removed</param>
        /// <returns>return the boolean values indicates the successful removal of node from the TreeViewAdv</returns>
        public bool Remove(object item)
        {
            if (!(item is TreeViewItemAdv))
            {
                return false;
            }

            rglobalitem = null;
            TreeViewItemAdv rparenttreeviewitem = null;
            TreeViewAdv rparenttreeview = null;
            if (item == null)
            {
                return false;
            }

            ItemExists(this.Items, (TreeViewItemAdv)item);
            if (rglobalitem == null)
            {
                return false;
            }

            rparenttreeviewitem = ((TreeViewItemAdv)item).ParentNode;
            rparenttreeview = ((TreeViewItemAdv)item).ParentTreeview;

            if (rparenttreeviewitem != null)
            {
                bool flag = rparenttreeviewitem.Items.Remove(item);
                if (flag == true)
                {
                    if (rparenttreeviewitem.Items.Count > 0)
                    {
                        rparenttreeviewitem.HasItems = true;
                        RefreshRootLines(this);
                        rparenttreeviewitem.expander.IsChecked = true;
                    }
                }
            }
            else if (rparenttreeview != null)
            {
                bool flag = rparenttreeview.Items.Remove(item);
            }
            RefreshRootLines(this);
            return false;
        }

        /// <summary>
        /// Method removes the node from the TreeViewAdv by specifying the index value of the node
        /// </summary>
        /// <param name="index">Indicates the index value of the node to be removed</param>
        /// <returns>Returns the boolean values indicates the successful removal of node from the TreeViewAdv</returns>
        public bool RemoveAt(int index)
        {
            TreeViewAdv treeview = this as TreeViewAdv;
            if (treeview != null)
            {
                if (treeview.Items.Count > 0)
                {
                    if (index > treeview.Items.Count - 1)
                    {
                        return false;
                    }
                    else
                    {
                        treeview.Items.RemoveAt(index);
                        RefreshRootLines(this);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Method removes the node from the TreeViewAdv control.
        /// </summary>
        /// <param name="index">Indicates the index value of the node to be removed</param>
        /// <param name="container">Indicates the Container from which the specified node will be removed</param>
        /// <returns>Returns the boolean value indicates the successful completion of removal</returns>
        public bool RemoveAt(int index, object container)
        {
            if (container == null)
            {
                return false;
            }

            if (!(container is TreeViewItemAdv))
            {
                return false;
            }

            if (((TreeViewItemAdv)container).Items.Count < 0)
            {
                return false;
            }

            if (index > ((TreeViewItemAdv)container).Items.Count - 1)
            {
                return false;
            }

            ((TreeViewItemAdv)container).Items.RemoveAt(index);
            RefreshRootLines(this);
            return true;
        }

        /// <summary>
        /// Method removes the Whole children from the TreeViewAdv control.
        /// </summary>
        /// <returns>Returns the boolean value indicates the successful completion of removal</returns>
        public bool RemoveChildren()
        {
            TreeViewAdv treeview = this as TreeViewAdv;
            if (treeview == null)
            {
                return false;
            }

            if (treeview.Items.Count < 0)
            {
                return false;
            }

            treeview.Items.Clear();
            RefreshRootLines(this);
            return true;
        }

        /// <summary>
        ///  Method removes the Entire children of the specified container.
        /// </summary>
        /// <param name="container">container is used to check and whether null or minimum count or whether exist or not.</param>
        /// <returns>Returns the boolean value indicates the successful completion of removal</returns>
        public bool RemoveChildren(object container)
        {
            if (!(container is TreeViewItemAdv))
            {
                return false;
            }

            rglobalitem = null;
            if (container == null)
            {
                return false;
            }

            if (((TreeViewItemAdv)container).Items.Count < 0)
            {
                return false;
            }

            ItemExists(this.Items, (TreeViewItemAdv)container);
            if (rglobalitem == null)
            {
                return false;
            }

            ((TreeViewItemAdv)container).Items.Clear();
            RefreshRootLines(this);
            return true;
        }

        /// <summary>
        /// Items the exists.
        /// </summary>
        /// <param name="tncoll">The tncoll.</param>
        /// <param name="strNode">The STR node.</param>
        private void ItemExists(ItemCollection tncoll, TreeViewItemAdv strNode)
        {
            foreach (TreeViewItemAdv tnode in tncoll)
            {
                if (object.ReferenceEquals(tnode, strNode))//if (tnode == strNode)
                {
                    rglobalitem = tnode;
                }
                else
                {
                }

                ItemExists(tnode.Items, strNode);
            }
        }
        #endregion

        #region Adding items
        int nodename = 0;

        /// <summary>
        /// Method adds the new node at the end TreeViewAdv control dynamically.
        /// </summary>
        public void Add()
        {
            TreeViewAdv treeview = (TreeViewAdv)this;
            if (this.Items.Count > 0)
            {
                object temp1 = this.Items[0];
                if (!(temp1 is TreeViewItemAdv))
                {
                    return;
                }
            }

            TreeViewItemAdv item = (TreeViewItemAdv)this.SelectedItem;
            TreeViewItemAdv newitem = CreateItem();
            if (item != null)
            {
                item.Items.Add(newitem);
                RefreshRootLines(this);
                item.expander.IsChecked = true;
            }
            else if (treeview != null)
            {
                treeview.Items.Add(newitem);
                RefreshRootLines(this);
            }
        }

        /// <summary>
        /// Method adds the Specified new node at the end TreeViewAdv control dynamically.
        /// </summary>
        /// <param name="newitem">Indicates the new node to be added to the control.</param>
        public void Add(object newitem)
        {
            if (!(newitem is TreeViewItemAdv))
            {
                return;
            }

            TreeViewAdv treeview = (TreeViewAdv)this;
            TreeViewItemAdv item = (TreeViewItemAdv)this.SelectedItem;
            if (item != null)
            {
                item.Items.Add(newitem);
                RefreshRootLines(this);
                item.expander.IsChecked = true;
            }
            else if (treeview != null)
            {
                treeview.Items.Add(newitem);
                RefreshRootLines(this);
            }
        }

        /// <summary>
        /// Method adds the Specified new node to the specified container dynamically.
        /// </summary>
        /// <param name="newitem">Indicates the new node to be added</param>
        /// <param name="container">Indicates the container for adding the new nodes</param>
        public void Add(object newitem, object container)
        {
            if (newitem == null)
            {
                return;
            }

            if (container == null)
            {
                return;
            }

            if (!(newitem is TreeViewItemAdv))
            {
                return;
            }

            if (!(container is TreeViewItemAdv))
            {
                return;
            }

            ((TreeViewItemAdv)container).Items.Add(newitem);
            RefreshRootLines(this);
        }

        /// <summary>
        /// Creates the item.
        /// </summary>
        /// <returns></returns>
        private TreeViewItemAdv CreateItem()
        {
            TreeViewItemAdv newItem = new TreeViewItemAdv();
            newItem.Header = "Node" + nodename.ToString();
            newItem.Select(true);
            nodename = nodename + 1;
            return newItem;
        }
        #endregion

        #region PreviousNext items
        /// <summary>
        /// Method used to get the previous node of selected node.
        /// </summary>
        /// <returns>Returns the previous node</returns>
        public TreeViewItemAdv GetPreviousNode()
        {
            object temp = this.SelectedNode;
            if (!(temp is TreeViewItemAdv))
            {
                return null;
            }

            TreeViewItemAdv item = this.SelectedNode;
            TreeViewAdv parentTreeView = item.ParentTreeview;
            TreeViewItemAdv parentTreeViewItem = item.ParentNode;
            TreeViewItemAdv previousTreeViewItem = null;
            int currentIndex = -1;
            int previousIndex = -1;
            if (parentTreeViewItem != null)
            {
                currentIndex = parentTreeViewItem.Items.IndexOf(item);
                previousIndex = currentIndex - 1;
                if (previousIndex > -1)
                {
                    previousTreeViewItem = parentTreeViewItem.Items[previousIndex] as TreeViewItemAdv;
                    if (previousTreeViewItem.IsExpanded && previousTreeViewItem.Items.Count > 0)
                    {
                        previousTreeViewItem = previousTreeViewItem.Items[previousTreeViewItem.Items.Count - 1] as TreeViewItemAdv;
                    }

                    return previousTreeViewItem;
                }
                else
                {
                    return parentTreeViewItem;
                }
            }
            else
            {
                currentIndex = parentTreeView.Items.IndexOf(item);
                previousIndex = currentIndex - 1;
                if (previousIndex > -1)
                {
                    previousTreeViewItem = parentTreeView.Items[previousIndex] as TreeViewItemAdv;
                    if (previousTreeViewItem.IsExpanded && previousTreeViewItem.Items.Count > 0)
                    {
                        previousTreeViewItem = previousTreeViewItem.Items[previousTreeViewItem.Items.Count - 1] as TreeViewItemAdv;
                    }

                    return previousTreeViewItem;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Method used to get the Next node of selected node.
        /// </summary>
        /// <returns>Returns the previous node</returns>
        public TreeViewItemAdv GetNextNode()
        {
            object temp = this.SelectedNode;
            if (!(temp is TreeViewItemAdv))
            {
                return null;
            }

            TreeViewItemAdv item = this.SelectedNode;
            TreeViewAdv parentTreeView = item.ParentTreeview;
            TreeViewItemAdv parentTreeViewItem = item.ParentNode;
            TreeViewItemAdv nextTreeViewItem = null;
            int currentIndex = -1;
            int nextIndex = -1;
            if (parentTreeViewItem != null)
            {
                if (item.IsExpanded && this.Items.Count > 0)
                {
                    nextTreeViewItem = this.Items[0] as TreeViewItemAdv;
                    return nextTreeViewItem;
                }
                else
                {
                    currentIndex = parentTreeViewItem.Items.IndexOf(item);
                    nextIndex = currentIndex + 1;
                    if (nextIndex < parentTreeViewItem.Items.Count)
                    {
                        nextTreeViewItem = parentTreeViewItem.Items[nextIndex] as TreeViewItemAdv;
                        return nextTreeViewItem;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                if (item.IsExpanded && this.Items.Count > 0)
                {
                    nextTreeViewItem = this.Items[0] as TreeViewItemAdv;
                    return nextTreeViewItem;
                }
                else
                {
                    currentIndex = parentTreeView.Items.IndexOf(this);
                    nextIndex = currentIndex + 1;
                    if (nextIndex < parentTreeView.Items.Count)
                    {
                        nextTreeViewItem = parentTreeView.Items[nextIndex] as TreeViewItemAdv;
                        return nextTreeViewItem;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// Method used to get the previous node of selected node.
        /// </summary>
        /// <returns>Returns the previous node</returns>
        public TreeViewItemAdv GetPreviousNode(object currentitem)
        {
            if (!(currentitem is TreeViewItemAdv))
            {
                return null;
            }

            TreeViewItemAdv item = (TreeViewItemAdv)currentitem;
            TreeViewAdv parentTreeView = item.ParentTreeview;
            TreeViewItemAdv parentTreeViewItem = item.ParentNode;
            TreeViewItemAdv previousTreeViewItem = null;
            int currentIndex = -1;
            int previousIndex = -1;
            if (parentTreeViewItem != null)
            {
                currentIndex = parentTreeViewItem.Items.IndexOf(item);
                previousIndex = currentIndex - 1;
                if (previousIndex > -1)
                {
                    previousTreeViewItem = parentTreeViewItem.Items[previousIndex] as TreeViewItemAdv;
                    if (previousTreeViewItem.IsExpanded && previousTreeViewItem.Items.Count > 0)
                    {
                        previousTreeViewItem = previousTreeViewItem.Items[previousTreeViewItem.Items.Count - 1] as TreeViewItemAdv;
                    }

                    return previousTreeViewItem;
                }
                else
                {
                    return parentTreeViewItem;
                }
            }
            else
            {
                currentIndex = parentTreeView.Items.IndexOf(item);
                previousIndex = currentIndex - 1;
                if (previousIndex > -1)
                {
                    previousTreeViewItem = parentTreeView.Items[previousIndex] as TreeViewItemAdv;
                    if (previousTreeViewItem.IsExpanded && previousTreeViewItem.Items.Count > 0)
                    {
                        previousTreeViewItem = previousTreeViewItem.Items[previousTreeViewItem.Items.Count - 1] as TreeViewItemAdv;
                    }

                    return previousTreeViewItem;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Method used to get the Next node of selected node.
        /// </summary>
        /// <returns>Returns the previous node</returns>
        public TreeViewItemAdv GetNextNode(TreeViewItemAdv currentitem)
        {
            if (!(currentitem is TreeViewItemAdv))
            {
                return null;
            }

            TreeViewItemAdv item = currentitem;
            TreeViewAdv parentTreeView = item.ParentTreeview;
            TreeViewItemAdv parentTreeViewItem = item.ParentNode;
            TreeViewItemAdv nextTreeViewItem = null;
            int currentIndex = -1;
            int nextIndex = -1;
            if (parentTreeViewItem != null)
            {
                if (item.IsExpanded && this.Items.Count > 0)
                {
                    nextTreeViewItem = this.Items[0] as TreeViewItemAdv;
                    return nextTreeViewItem;
                }
                else
                {
                    currentIndex = parentTreeViewItem.Items.IndexOf(item);
                    nextIndex = currentIndex + 1;
                    if (nextIndex < parentTreeViewItem.Items.Count)
                    {
                        nextTreeViewItem = parentTreeViewItem.Items[nextIndex] as TreeViewItemAdv;
                        return nextTreeViewItem;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                if (item.IsExpanded && this.Items.Count > 0)
                {
                    nextTreeViewItem = this.Items[0] as TreeViewItemAdv;
                    return nextTreeViewItem;
                }
                else
                {
                    currentIndex = parentTreeView.Items.IndexOf(this);
                    nextIndex = currentIndex + 1;
                    if (nextIndex < parentTreeView.Items.Count)
                    {
                        nextTreeViewItem = parentTreeView.Items[nextIndex] as TreeViewItemAdv;
                        return nextTreeViewItem;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
        #endregion

        #region Cloning the treeviewitems
        TreeViewItemAdv newtreeviewitem;
        /// <summary>
        /// Trees the view item clone.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        TreeViewItemAdv TreeViewItemClone(TreeViewItemAdv source)
        {
            if (source.Items.Count == 0)
            {
                TreeViewItemAdv item1 = null;
                item1 = new TreeViewItemAdv();

                item1.LeftImageSource = source.LeftImageSource;
                item1.LeftImageHeight = source.LeftImageHeight;
                item1.LeftImageWidth = source.LeftImageWidth;
                item1.LineStrokeArray = source.LineStrokeArray;
                item1.RightImageSource = source.RightImageSource;
                item1.RightImageHeight = source.RightImageHeight;
                item1.RightImageWidth = source.RightImageWidth;
                item1.ImageMargin = source.ImageMargin;
                item1.TextHorizontalAlignment = source.TextHorizontalAlignment;
                item1.TextMargin = source.TextMargin;
                item1.TextVerticalAlignment = source.TextVerticalAlignment;
                item1.ExpanderTemplate = source.ExpanderTemplate;
                item1.ExpandImageSource = source.ExpandImageSource;
                item1.ExpandImageHeight = source.ExpandImageHeight;
                item1.ExpandImageWidth = source.ExpandImageWidth;
                item1.CollapseImageSource = source.CollapseImageSource;
                item1.CollapseImageHeight = source.CollapseImageHeight;
                item1.CollapseImageWidth = source.CollapseImageWidth;
                item1.Header = source.Header;

                return item1;
            }
            else
            {
                newtreeviewitem = new TreeViewItemAdv();
                newtreeviewitem.LeftImageSource = source.LeftImageSource;
                newtreeviewitem.LeftImageHeight = source.LeftImageHeight;
                newtreeviewitem.LeftImageWidth = source.LeftImageWidth;
                newtreeviewitem.LineStrokeArray = source.LineStrokeArray;
                newtreeviewitem.RightImageSource = source.RightImageSource;
                newtreeviewitem.RightImageHeight = source.RightImageHeight;
                newtreeviewitem.RightImageWidth = source.RightImageWidth;
                newtreeviewitem.ImageMargin = source.ImageMargin;
                newtreeviewitem.TextHorizontalAlignment = source.TextHorizontalAlignment;
                newtreeviewitem.TextMargin = source.TextMargin;
                newtreeviewitem.TextVerticalAlignment = source.TextVerticalAlignment;
                newtreeviewitem.ExpanderTemplate = source.ExpanderTemplate;
                newtreeviewitem.ExpandImageSource = source.ExpandImageSource;
                newtreeviewitem.ExpandImageHeight = source.ExpandImageHeight;
                newtreeviewitem.ExpandImageWidth = source.ExpandImageWidth;
                newtreeviewitem.CollapseImageSource = source.CollapseImageSource;
                newtreeviewitem.CollapseImageHeight = source.CollapseImageHeight;
                newtreeviewitem.CollapseImageWidth = source.CollapseImageWidth;
                newtreeviewitem.Header = source.Header;
                CloningTreeViewItems(source.Items, newtreeviewitem);
                return newtreeviewitem;
            }
        }

        /// <summary>
        /// Clonings the tree view items.
        /// </summary>
        /// <param name="tncoll">The tncoll.</param>
        /// <param name="container">The container.</param>
        private void CloningTreeViewItems(ItemCollection tncoll, TreeViewItemAdv container)
        {
            if (tncoll == null && container == null)
            {
                return;
            }

            foreach (TreeViewItemAdv tnode in tncoll)
            {
                TreeViewItemAdv temp = new TreeViewItemAdv();
                temp.LeftImageSource = tnode.LeftImageSource;
                temp.LeftImageHeight = tnode.LeftImageHeight;
                temp.LeftImageWidth = tnode.LeftImageWidth;
                temp.LineStrokeArray = tnode.LineStrokeArray;
                temp.RightImageSource = tnode.RightImageSource;
                temp.RightImageHeight = tnode.RightImageHeight;
                temp.RightImageWidth = tnode.RightImageWidth;
                temp.ImageMargin = tnode.ImageMargin;
                temp.TextHorizontalAlignment = tnode.TextHorizontalAlignment;
                temp.TextMargin = tnode.TextMargin;
                temp.TextVerticalAlignment = tnode.TextVerticalAlignment;
                temp.ExpanderTemplate = tnode.ExpanderTemplate;
                temp.ExpandImageSource = tnode.ExpandImageSource;
                temp.ExpandImageHeight = tnode.ExpandImageHeight;
                temp.ExpandImageWidth = tnode.ExpandImageWidth;
                temp.CollapseImageSource = tnode.CollapseImageSource;
                temp.CollapseImageHeight = tnode.CollapseImageHeight;
                temp.CollapseImageWidth = tnode.CollapseImageWidth;
                temp.Header = tnode.Header;
                container.Items.Add(temp);
                CloningTreeViewItems(tnode.Items, temp);
            }
        }
        #endregion

        #region Scroll Method
        double down = 0;
        double lineChange = 16.0;
        /// <summary>
        /// Called when [scroll].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.Browser.HtmlEventArgs"/> instance containing the event data.</param>
        private void OnScroll(object sender, HtmlEventArgs args)
        {
            if (scrollFlag)
            {
                double delta = 0;
                down = this.elementScrollViewer.VerticalOffset;
                ScriptObject e = args.EventObject;
                if (e.GetProperty("wheelDelta") != null) 
                {
                    delta = (double)e.GetProperty("wheelDelta");
                    if (HtmlPage.Window.GetProperty("opera") != null)
                    {
                        delta = -delta;
                    }
                }
                else if (e.GetProperty("detail") != null)
                {
                    delta = -((double)e.GetProperty("detail"));
                }

                delta = Math.Sign(delta);

                ////scrolling outwards
                if (delta > 0)
                {
                    down = down - this.ScrollingSpeed;
                    this.elementScrollViewer.ScrollToVerticalOffset(down);
                }

                ////Scrolling inwards
                if (delta < 0)
                {
                    down = down + this.ScrollingSpeed;
                    this.elementScrollViewer.ScrollToVerticalOffset(down);
                }
            }
        }

        /// <summary>
        /// Ups this instance.
        /// </summary>
        public static void Up()
        {
            object obj = FocusManager.GetFocusedElement();
            if (obj is TreeViewAdv && obj != null)
            {
                TreeViewAdv tview = (TreeViewAdv)obj;
                tview.elementScrollViewer.ScrollToVerticalOffset(-tview.lineChange);
            }
        }

        /// <summary>
        /// Downs this instance.
        /// </summary>
        public static void Down()
        {
            object obj = FocusManager.GetFocusedElement();
            if (obj is TreeViewAdv && obj != null)
            {
                TreeViewAdv tview = (TreeViewAdv)obj;
                tview.elementScrollViewer.ScrollToVerticalOffset(tview.lineChange);
            }
        }

        /// <summary>
        /// Rights this instance.
        /// </summary>
        public static void Right()
        {
            object obj = FocusManager.GetFocusedElement();
            if (obj is TreeViewAdv && obj != null)
            {
                TreeViewAdv tview = (TreeViewAdv)obj;
                tview.elementScrollViewer.ScrollToHorizontalOffset(tview.lineChange);
            }
        }

        /// <summary>
        /// Lefts this instance.
        /// </summary>
        public static void Left()
        {
            object obj = FocusManager.GetFocusedElement();
            if (obj is TreeViewAdv && obj != null)
            {
                TreeViewAdv tview = (TreeViewAdv)obj;
                tview.elementScrollViewer.ScrollToVerticalOffset(-tview.lineChange);
            }
        }

        /// <summary>
        /// Pages up.
        /// </summary>
        public static void PageUp()
        {
            object obj = FocusManager.GetFocusedElement();
            //if (obj is TreeViewAdv && obj != null)
            //{
            //    TreeViewAdv tview = (TreeViewAdv)obj;
            //    tview.elementScrollViewer.ScrollToVerticalOffset(-tview.elementScrollViewer.ViewportHeight);
            //}
            if (obj is TreeViewItemAdv & obj != null)
            {
                //((TreeViewItemAdv)obj).ParentTreeview.elementScrollViewer.ScrollToVerticalOffset(-((TreeViewItemAdv)obj).ParentTreeview.elementScrollViewer.ViewportHeight);
                ((TreeViewItemAdv)obj).ParentTreeview.elementScrollViewer.ScrollToVerticalOffset(((TreeViewItemAdv)obj).ParentTreeview.elementScrollViewer.VerticalOffset -
                    ((TreeViewItemAdv)obj).ParentTreeview.elementScrollViewer.ViewportHeight);
            }
        }

        /// <summary>
        /// Pages down.
        /// </summary>
        public static void PageDown()
        {
            object obj = FocusManager.GetFocusedElement();
            //if (obj is TreeViewAdv && obj != null)
            //{
            //    TreeViewAdv tview = (TreeViewAdv)obj;
            //    tview.elementScrollViewer.ScrollToVerticalOffset(tview.elementScrollViewer.ViewportHeight);
            //}
            if (obj is TreeViewItemAdv & obj != null)
            {
                ((TreeViewItemAdv)obj).ParentTreeview.elementScrollViewer.ScrollToVerticalOffset(((TreeViewItemAdv)obj).ParentTreeview.elementScrollViewer.VerticalOffset +
                    ((TreeViewItemAdv)obj).ParentTreeview.elementScrollViewer.ViewportHeight);
            }
        }

        /// <summary>
        /// Tops this instance.
        /// </summary>
        public static void Top()
        {
            object obj = FocusManager.GetFocusedElement();
            if (obj is TreeViewAdv && obj != null)
            {
                TreeViewAdv tview = (TreeViewAdv)obj;
                tview.elementScrollViewer.ScrollToVerticalOffset(0);
            }
        }

        /// <summary>
        /// Bottoms this instance.
        /// </summary>
        public static void Bottom()
        {
            object obj = FocusManager.GetFocusedElement();
            if (obj is TreeViewAdv && obj != null)
            {
                TreeViewAdv tview = (TreeViewAdv)obj;
                tview.elementScrollViewer.ScrollToVerticalOffset(tview.elementScrollViewer.ExtentHeight);
            }
        }
        #endregion

        #region Methods

        Grid layoutRoot = null;

        //LayoutRoot
        /// <summary>
        /// Called when an internal process or application calls
        /// ApplyTemplate, which is used to build the current template's
        /// visual tree. 
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (layoutRoot != null)
            {
                layoutRoot.MouseLeftButtonUp -= new MouseButtonEventHandler(treeviewcanvas_MouseLeftButtonUp);
                layoutRoot.MouseMove -= new MouseEventHandler(treeviewcanvas_MouseMove);
            }

            if (this.treeviewgrid != null)
            {
                this.treeviewgrid.MouseLeftButtonDown -= new MouseButtonEventHandler(treeviewgrid_MouseLeftButtonDown);
                this.treeviewgrid.MouseMove -= new MouseEventHandler(treeviewgrid_MouseMove);
                this.treeviewgrid.MouseLeftButtonUp -= new MouseButtonEventHandler(treeviewgrid_MouseLeftButtonUp);
            }

            layoutRoot = this.GetTemplateChild("LayoutRoot") as Grid;
            this.elementScrollViewer = this.GetTemplateChild(TreeViewAdv.ElementScrollViewerName) as ScrollViewer;
            this.treeviewcanvas = this.GetTemplateChild("TreeViewCanvas") as Canvas;
            this.treeviewgrid = this.GetTemplateChild("TreeViewGrid") as Grid;

            if (layoutRoot != null)
            {
                layoutRoot.MouseLeftButtonUp += new MouseButtonEventHandler(treeviewcanvas_MouseLeftButtonUp);
                layoutRoot.MouseMove += new MouseEventHandler(treeviewcanvas_MouseMove);
            }
            
            if (this.treeviewgrid != null)
            {
                this.treeviewgrid.MouseLeftButtonDown += new MouseButtonEventHandler(treeviewgrid_MouseLeftButtonDown);
                this.treeviewgrid.MouseMove += new MouseEventHandler(treeviewgrid_MouseMove);
                this.treeviewgrid.MouseLeftButtonUp += new MouseButtonEventHandler(treeviewgrid_MouseLeftButtonUp);
            }

            this.popup = this.GetTemplateChild("PopUp") as Popup;
            this.txtPopupDet = this.GetTemplateChild("TxtPopupDet") as TextBlock;
            if (this.ExpanderTemplate == null)
            {
                if (this.treeviewgrid != null)
                {
                    ExpanderTemplate = this.treeviewgrid.Resources["ExpanderTemplate"] as ControlTemplate;
                }
            }

            for (int i = 0; i < this.Items.Count; i++)
            {
                if (this.ItemsSource != null)
                {
                    TreeViewItemAdv m_treeViewItem = this.ItemContainerGenerator.ContainerFromItem(this.Items[i]) as TreeViewItemAdv;
                    if (m_treeViewItem != null)
                        m_treeViewItem.Style = this.ItemContainerStyle;
                }
                else
                {
                    if (this.Items[i] is TreeViewItemAdv && this.ItemContainerStyle != null)
                        (this.Items[i] as TreeViewItemAdv).Style = this.ItemContainerStyle;
                }
            }
        }

        private Panel _itemsHost;

        internal Panel ItemsHost
        {
            get
            {
                // Lookup the ItemsHost if we haven't already cached it.
                if (_itemsHost == null && this != null && this.ItemContainerGenerator != null)
                {
                    // Get any live container
                    DependencyObject container = this.ItemContainerGenerator.ContainerFromIndex(0);
                    if (container != null)
                    {
                        // Get the parent of the container
                        _itemsHost = VisualTreeHelper.GetParent(container) as Panel;
                    }
                }

                return _itemsHost;
            }
        }

        internal void UpdateItemContainerStyle(Style itemContainerStyle)
        {
            if (itemContainerStyle == null)
            {
                return;
            }

            Panel itemsHost = ItemsHost;
            if (itemsHost == null || itemsHost.Children == null)
            {
                return;
            }

            foreach (UIElement element in itemsHost.Children)
            {
                FrameworkElement obj = element as FrameworkElement;
                if (obj.Style == null)
                {
                    obj.Style = itemContainerStyle;
                }
            }
        }

        /// <summary>
        /// Method used to scroll the TreeViewAdv control to reach the particular Node
        /// </summary>
        /// <param name="item">Indicates the Node to be Viewed</param>
        public void ScrollIntoView(object item)
        {
            if (!(item is TreeViewItemAdv))
            {
                return;
            }

            if (null != this.elementScrollViewer)
            {
                Rect itemsHostRect;
                Rect listBoxItemRect;
                if (!this.CTPage((TreeViewItemAdv)item, out itemsHostRect, out listBoxItemRect))
                {
                    // Scroll into view vertically (first make the right bound visible, then the left)
                    double verticalOffset = this.elementScrollViewer.VerticalOffset;
                    double verticalDelta = 0;
                    if (itemsHostRect.Bottom < listBoxItemRect.Bottom)
                    {
                        verticalDelta = listBoxItemRect.Bottom - itemsHostRect.Bottom;
                        verticalOffset += verticalDelta;
                    }

                    if (listBoxItemRect.Top - verticalDelta < itemsHostRect.Top)
                    {
                        verticalOffset -= itemsHostRect.Top - (listBoxItemRect.Top - verticalDelta);
                    }

                    this.elementScrollViewer.ScrollToVerticalOffset(verticalOffset);
                }
            }
        }

        /// <summary>
        /// Selections the CHGD.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="selected">if set to <c>true</c> [selected].</param>
        internal void SelectionChgd(object container, bool selected)
        {
            if (!(container is TreeViewItemAdv))
            {
                return;
            }

            if (!this.isselectionChangeActive)
            {
                this.isselectionChangeActive = true;

                try
                {
                    if (!selected)
                    {
                        if (container == this.SelectedNode)
                        {
                            this.SelectedNode = null;
                            this.SelectedItem = null;
                        }
                    }

                    if (((TreeViewItemAdv)container).IsSelected != selected)
                    {
                        ((TreeViewItemAdv)container).IsSelected = selected;
                    }

                    if (selected)
                    {
                        if (this.SelectedNode != null && this.SelectedNode != container)
                        {
                            if (this.SelectionMode == SelectionMode.Single)
                            {
                                for (int i = 0; i < SelectedNodes.Count; i++)
                                    this.SelectedNodes[i].IsSelected = false;
                                this.SelectedNode.IsSelected = false;
                            }
                            //if (!this.IsMultiSelect)
                            //{
                            //    this.SelectedNode.IsSelected = false;
                            //}
                        }

                        this.SelectedNode = (TreeViewItemAdv)container;

                        this.SelectedItem = ((TreeViewItemAdv)container).DataContext ?? container;
                    }
                }
                finally
                {
                    this.isselectionChangeActive = false;
                }
            }
        }

        /// <summary>
        /// Checks the selection.
        /// </summary>
        internal void CheckSelection()
        {
            object selectedItem = this.SelectedItem;

            if (selectedItem == null)
            {
                if (this.SelectedNode != null)
                {
                    this.SelectedNode.IsMouseOver = false;
                    this.SelectedNode.IsSelected = false;                    
                }
            }
            //else if (this.SelectedNode == null || this.SelectedNode.DataContext != selectedItem)
            //{
            //    TreeViewItemAdv node = this.FindChildNode(selectedItem);
            //    if (node != null)
            //    {
            //        node.IsSelected = true;
            //        TreeViewItemAdv parentNode = node.ParentNode;
            //        while (parentNode != null)
            //        {
            //            parentNode.IsExpanded = true;
            //            parentNode = parentNode.ParentNode;
            //        }

            //        this.ScrollIntoView(node);
            //    }
            //}
        }

        /// <summary>
        /// Method clears the container 
        /// </summary>
        /// <param name="element">The element is a dependency object to clear container for item override</param>
        /// <param name="item">Indicates the Node to be Removed</param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            this.Nodes.Remove(item as TreeViewItemAdv);
            base.ClearContainerForItemOverride(element, item);
            this.RefreshRootLines(this);
        }

        /// <summary>
        /// Method returns the container
        /// </summary>
        /// <returns>Type : TreeViewItemAdv</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            if (this.lastContainerCheck != null)
            {
                return this.lastContainerCheck;
            }

            TreeViewItemAdv itm = new TreeViewItemAdv();
            return itm;
        }

        internal void ExpandingTreeViewItem(object sender, ExpandCollapseEventArgs args)
        {
            if (this.Expanding != null)
            {
                this.Expanding(sender, args);
            }
            TreeViewItemAdv treeItem = sender as TreeViewItemAdv;
            if (treeItem != null && treeItem.IsLoadOnDemand)
            {
                treeItem.IsLoading = true;
                treeItem.UpdateVisualState(false);
                if (LoadOnDemand != null)
                {
                    LoadOnDemand(this, new LoadonDemandEventArgs() { TreeViewItem = sender });
                }
            }
        }

        internal void CollapsingTreeViewItem(object sender, ExpandCollapseEventArgs args)
        {
            if (this.Collapsing != null)
            {
                this.Collapsing(sender, args);
            }
        }
        /// <summary>
        /// Method Determines whether the item and container is same or not
        /// </summary>
        /// <param name="item">Indicates the item</param>
        /// <returns>Type : bool</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            this.lastContainerCheck = null;
            if (item is TreeViewItemAdv)
            {
                return true;
            }

            DataTemplate template = this.ItemTemplate;
            if (template != null)
            {
                DependencyObject container = template.LoadContent();
                if (container is TreeViewItemAdv)
                {
                    this.lastContainerCheck = (TreeViewItemAdv)container;
                }
            }

            return false;
        }
        
        #region MultiSelection
        void Deselect(object item)
        {
            if (!(item is TreeViewItemAdv))
            {
                return;
            }

            TreeViewItemAdv treeViewItem = (TreeViewItemAdv)item;
            List<TreeViewItemAdv> templist = new List<TreeViewItemAdv>();
            for (int i = 0; i < this.SelectedNodes.Count; i++)
            {
                templist.Add(this.SelectedNodes[i]);
            }

            treeViewItem.IsSelected = false;
            treeViewItem.IsMultiSelect = false;
            templist.Remove(treeViewItem);
            this.SelectedNodes = templist;
        }

        void ChangeSelectedState(TreeViewItemAdv item)
        {
            if (!(item is TreeViewItemAdv))
            {
                return;
            }

            TreeViewItemAdv treeViewItem = (TreeViewItemAdv)item;
            if (SelectedNodes.IndexOf(treeViewItem) == -1)
            {
                List<TreeViewItemAdv> templist = new List<TreeViewItemAdv>();
                for (int i = 0; i < this.SelectedNodes.Count; i++)
                {
                    templist.Add(this.SelectedNodes[i]);
                }

                treeViewItem.IsSelected = false;
                treeViewItem.IsMultiSelect = true;
                templist.Add(treeViewItem); // add the item to selected items
                this.SelectedNodes = templist;
            }
            else
            {
                Deselect(treeViewItem);
            }
        }

        internal void ClearSelectedNodes()
        {
            try
            {
                foreach (TreeViewItemAdv node in this.SelectedNodes)
                {
                    node.IsMouseOver = false;
                    node.IsSelected = false;
                    node.IsMultiSelect = false;  
                }
            }
            finally
            {

                if (this.SelectedNodes.Count > 1)
                {
                    this.SelectedNode = null;
                }
                this.SelectedNodes.Clear();
                this.SelectedItems.Clear();
            }
        }

        private void SelectSingleNode(TreeViewItemAdv node)
        {
            if (node == null)
            {
                return;
            }

            ClearSelectedNodes();
            ToggleNode(node, true);
            ////node.EnsureVisible();
        }

        private void ToggleNode(TreeViewItemAdv node, bool bSelectNode)
        {
            if (bSelectNode)
            {
                if (this.SelectedNode != null)
                {
                    if (!SelectedNodes.Contains(this.SelectedNode))
                    {
                        SelectedNodes.Add(this.SelectedNode);                       
                        this.SelectedItems.Add(node.DataContext ?? node);
                        this.SelectedNode.IsSelected = true;
                        this.SelectedNode.IsMultiSelect = true;
                    }
                }

                if (!SelectedNodes.Contains(node))
                {
                    SelectedNodes.Add(node);
                    SelectedItems.Add(node.DataContext ?? node);
                }

                node.IsSelected = true;
                node.IsMouseOver = false;
                node.IsMultiSelect = true;                
                
            }
            else
            {
                SelectedNodes.Remove(node);
                SelectedItems.Remove(node.DataContext ?? node);
                node.IsMouseOver = false;
                node.IsMultiSelect = false;
                node.IsSelected = false;
            }

            List<TreeViewItemAdv> templist = new List<TreeViewItemAdv>();
            for (int i = 0; i < this.SelectedNodes.Count; i++)
            {
                templist.Add(this.SelectedNodes[i]);
            }

            this.SelectedNodes = templist;
        }

        private void HandleException(Exception ex)
        {
            // Perform some error handling here.
            // We don't want to bubble errors to the CLR. 
            MessageBox.Show(ex.Message);
        }

        private int ItemIndex(TreeViewItemAdv item)
        {
            if (item.ParentNode != null)
            {
                if (item.ParentNode.ItemsSource == null)
                    return item.ParentNode.Items.IndexOf(item);
                else
                {
                    IList list = item.ParentNode.ItemsSource as IList;
                    return list.IndexOf(item.Header);
                }
            }
            else
            {
                if (item.ParentTreeview.ItemsSource == null)
                    return item.ParentTreeview.Items.IndexOf(item);
                else
                {
                    IList list = item.ParentTreeview.ItemsSource as IList;
                    return list.IndexOf(item.Header);
                }
            }
        }

        #endregion
        /// <summary>
        /// Method handles when the mouse left button is pressed
        /// </summary>
        /// <param name="e">Contains information about the cursor position</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            TreeViewItemAdv item = GetTreeViewItem(e.OriginalSource);
        }

        TreeViewItemAdv node = null;
        bool clickDragflag = false;
        /// <summary>
        /// Handles the MouseMove event of the treeviewgrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void treeviewgrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.mouseLButtonDown)
            {
                Point position = e.GetPosition(this.treeviewcanvas);
                if (!this.dragging && (((((this.initialmouseposition.X - position.X) > 5.0) || ((this.initialmouseposition.X - position.X) < -5.0)) || ((this.initialmouseposition.Y - position.Y) > 5.0)) || ((this.initialmouseposition.Y - position.Y) < -5.0)))
                {
                    this.dragging = true;
                }
            }
            if (this.dragging && this.clickDragflag)
            {
                this.PopuUpCreation(this.source_Selected_TVItems, e);
                if (this.popup != null)
                {
                    this.popup.CaptureMouse();
                    Point point2 = e.GetPosition(this.treeviewcanvas);
                    this.popup.VerticalOffset = point2.Y;
                    this.popup.HorizontalOffset = point2.X;
                }
                if (this.target_Selected_TVItem != null)
                {
                    this.destinationData = this.target_Selected_TVItem.Header;
                }
                if (this.DragStarted != null)
                {
                    this.DragStarted(this, new TreeViewAdvDragStartEventArgs(source_Selected_TVItems, source_Selected_TVItems_Parent, source_TreeView));
                    //this.DragStarted(this, new TreeViewAdvDragStartEventArgs(source_Selected_TVItems,source_Selected_TVItems_Parent, source_TreeView));
                    //this.DragStarted(this, new TreeViewAdvDragEventArgs(source_Selected_TVItems, source_Selected_TVItems_Parent, source_TreeView, target_Selected_TVItem, target_Selected_TVItem_Parent, target_TreeView, data, destinationData));
                }
                this.clickDragflag = false;
            }
            base.OnMouseMove(e);
        }

        internal double m_Height = 0.0;
        //double m_Width = 0.0;
        internal TreeViewItemAdv exactItemFromPoint = null;
        TreeViewItemAdv m_PreviouNodeFromPoint = null;
        /// <summary>
        /// Gets the width of the previous item from height and.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="tnCollection">The tn collection.</param>
        /// <returns></returns>
        public TreeViewItemAdv GetPreviousItemFromHeightAndWidth(object obj, ItemCollection tnCollection)
        {
            double scrollHeight = elementScrollViewer.VerticalOffset;

            TreeViewItemAdv item = null;
            if (m_PreviouNodeFromPoint == null)
            {
                for (int i = 0; i < tnCollection.Count; i++)
                {
                    if (m_PreviouNodeFromPoint == null)
                    {
                        item = tnCollection[i] as TreeViewItemAdv;

                        if (item == null)
                        {
                            if (obj is TreeViewAdv)
                            {
                                item = ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv;
                            }
                            else
                            {
                                item = (obj as TreeViewItemAdv).ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv;
                            }
                        }
                        if (item != null)
                        {
                            if (item.Items.Count > 0)
                            {
                                if (exactItemFromPoint == item)
                                    {
                                        m_PreviouNodeFromPoint = obj as TreeViewItemAdv;
                                        break;
                                    }
                                    if (item.IsExpanded)
                                    {
                                        GetPreviousItemFromHeightAndWidth(item, item.Items);
                                    } 
                            }
                            else
                            {
                                if (exactItemFromPoint == item)
                                {
                                    m_PreviouNodeFromPoint = obj as TreeViewItemAdv;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return m_PreviouNodeFromPoint;
        }


        /// <summary>
        /// Gets the width of the item height and.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="tnCollection">The tn collection.</param>
        /// <param name="treeViewAdv">The tree view adv.</param>
        /// <returns></returns>
        public double GetItemHeightAndWidth(object obj , ItemCollection tnCollection, TreeViewAdv treeViewAdv)
        {
            double scrollHeight = 0.0;
            if (treeViewAdv.elementScrollViewer != null)
            {
                scrollHeight = treeViewAdv.elementScrollViewer.VerticalOffset;
            }
            TreeViewItemAdv item = null;
            if (exactItemFromPoint == null)
            {
                for (int i = 0; i < tnCollection.Count; i++)
                {
                    if (exactItemFromPoint == null)
                    {
                        item = tnCollection[i] as TreeViewItemAdv;

                        if (item == null)
                        {
                            if (obj is TreeViewAdv)
                            {
                                item = ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv;
                            }
                            else
                            {
                                item = (obj as TreeViewItemAdv).ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv;
                            }
                        }
                        if (item != null)
                        {
                            if (item.Items.Count > 0)
                            {
                                if (item.HeaderBorder != null)
                                {
                                    m_Height = m_Height + item.HeaderBorder.ActualHeight;
                                    if ((m_Height - item.HeaderBorder.ActualHeight - scrollHeight) <= pos1 && m_Height - scrollHeight >= pos1 && pos1 != -1)
                                    {
                                        if (pos1 < m_Height)
                                        {
                                            exactItemFromPoint = item;
                                        }
                                        break;
                                    }
                                    if (item.IsExpanded)
                                    {
                                        GetItemHeightAndWidth(item, item.Items, treeViewAdv);
                                    }
                                }
                            }
                            else
                            {
                                if (item.HeaderBorder != null)
                                {
                                    m_Height = m_Height + item.HeaderBorder.ActualHeight;
                                    if ((m_Height - item.HeaderBorder.ActualHeight - scrollHeight) <= pos1 && m_Height - scrollHeight >= pos1 && pos1 != -1)
                                    {
                                        if (pos1 < m_Height)
                                        {
                                            exactItemFromPoint = item;
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return m_Height;
        }

        internal TreeViewItemAdv firstlastViewItem;
        internal double GetItemFromHeight(object obj, ItemCollection tnCollection, TreeViewAdv treeViewAdv,double position)
        {
            double scrollHeight = 0.0;
            if (treeViewAdv.elementScrollViewer != null)
            {
                scrollHeight = treeViewAdv.elementScrollViewer.VerticalOffset;
            }
            TreeViewItemAdv item = null;
            if (firstlastViewItem == null)
            {
                for (int i = 0; i < tnCollection.Count; i++)
                {
                    if (exactItemFromPoint == null)
                    {
                        item = tnCollection[i] as TreeViewItemAdv;

                        if (item == null)
                        {
                            if (obj is TreeViewAdv)
                            {
                                item = ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv;
                            }
                            else
                            {
                                item = (obj as TreeViewItemAdv).ItemContainerGenerator.ContainerFromIndex(i) as TreeViewItemAdv;
                            }
                        }
                        if (item != null)
                        {
                            if (item.Items.Count > 0)
                            {
                                if (item.HeaderBorder != null)
                                {
                                    m_Height = m_Height + item.HeaderBorder.ActualHeight;
                                    if ((m_Height - item.HeaderBorder.ActualHeight - scrollHeight) <= position && m_Height - scrollHeight >= position && position != -1)
                                    {
                                        firstlastViewItem = item;
                                        break;
                                    }
                                    if (item.IsExpanded)
                                    {
                                        GetItemFromHeight(item, item.Items, treeViewAdv, position);
                                    }
                                }
                            }
                            else
                            {
                                if (item.HeaderBorder != null)
                                {
                                    m_Height = m_Height + item.HeaderBorder.ActualHeight;
                                    if ((m_Height - item.HeaderBorder.ActualHeight - scrollHeight) <= position && m_Height - scrollHeight >= position && position != -1)
                                    {
                                        firstlastViewItem = item;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return m_Height;
        }
        
        
        bool isItemHeaderEdit = false;
        Point initialmouseposition;
        object data = null;
        object destinationData = null;
        double pos1 = -1.0;

        internal void HandleEditMode(bool m_editFlag)
        {
            if (this.currentItemAdv.textbox != null)
            {
                this.currentItemAdv.textbox.Height = this.currentItemAdv.HeaderBorder.ActualHeight;
            }
            if (m_editFlag == true)
            {
                if (currentItemAdv != null)
                {
                    if (currentItemAdv.ParentTreeview.IsInEditMode == true)
                    {
                        mouseLButtonDown = false;
                        dragging = false;
                        if (this.IsInEditMode == true)
                        {
                            if (!(currentItemAdv.Header is string))
                                return;

                            NodeCancellableEditEventArgs nodeEditArgs = new NodeCancellableEditEventArgs(currentItemAdv)
                            {
                                Text = currentItemAdv.textbox.ToString(),
                                Cancel = false
                            };

                            if (this.NodeEditing != null)
                            {
                                this.NodeEditing(this, nodeEditArgs);
                            }
                            if (!nodeEditArgs.Cancel)
                            {
                                currentItemAdv.IsInEditMode = true;
                                currentItemAdv.contentpresenter.Visibility = Visibility.Collapsed;
                                currentItemAdv.textbox.Visibility = Visibility.Visible;
                                currentItemAdv.textbox.Focus();
                                currentItemAdv.textbox.SelectionStart = 0;
                                currentItemAdv.textbox.Foreground = new SolidColorBrush(Colors.Black);
                              
                                currentItemAdv.textbox.Text = currentItemAdv.Header.ToString();
                                
                                if (this.Theme == "Metro")
                                {
                                    currentItemAdv.textbox.Foreground = this.Foreground;
                                }
                                else if (this.Theme == "Blend")
                                {
                                    currentItemAdv.textbox.Foreground = new SolidColorBrush(Colors.White);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                NodeEditorCancellableEventArgs nodeEditorArgs = new NodeEditorCancellableEventArgs(currentItemAdv)
                {
                    Cancel=false,
                    ContinueEditing=false,
                    Text=currentItemAdv.textbox.Text
                };

                if (NodeEditorValidating != null)
                {
                    NodeEditorValidating(this, nodeEditorArgs);
                }

                if (nodeEditorArgs.ContinueEditing==true && nodeEditorArgs.Cancel==true)
                {
                    currentItemAdv.textbox.Text = currentItemAdv.Header.ToString();
                }

                if (nodeEditorArgs.ContinueEditing == true && nodeEditorArgs.Cancel == false)
                {
                    currentItemAdv.Header = currentItemAdv.textbox.Text;
                }

                if (nodeEditorArgs.ContinueEditing == false && nodeEditorArgs.Cancel == true)
                {
                    this.FireNodeEditCancelledEvent();
                    currentItemAdv.IsInEditMode = false;
                    currentItemAdv.contentpresenter.Visibility = Visibility.Visible;
                    currentItemAdv.textbox.Visibility = Visibility.Collapsed;
                    currentItemAdv.Focus();
                    currentItemAdv.textbox.SelectionStart = 0;
                }

                if (nodeEditorArgs.ContinueEditing == false && nodeEditorArgs.Cancel == false)
                {
                    if (this.NodeEditorValidated != null)
                        this.NodeEditorValidated(this, new NodeEditorEventArgs(currentItemAdv) { Text = this.currentItemAdv.textbox.Text });

                    currentItemAdv.IsInEditMode = false;
                    currentItemAdv.Header = currentItemAdv.textbox.Text;
                    currentItemAdv.contentpresenter.Visibility = Visibility.Visible;
                    currentItemAdv.textbox.Visibility = Visibility.Collapsed;
                    currentItemAdv.Focus();
                    currentItemAdv.textbox.SelectionStart = 0;
                    
                    if (this.NodeEdited != null)
                        this.NodeEdited(this, new NodeEditEventArgs(currentItemAdv) { Text = currentItemAdv.Header.ToString() });
                }
            }
        }

        internal void FireNodeEditedEvent()
        {
            if (this.NodeEdited != null)
                this.NodeEdited(this, new NodeEditEventArgs(currentItemAdv) { Text = currentItemAdv.Header.ToString() });
        }

        internal void FireNodeEditCancelledEvent()
        {
            if (this.NodeEditCancelled != null)
                this.NodeEditCancelled(this, new NodeEditEventArgs(currentItemAdv) { Text = currentItemAdv.Header.ToString() });
        }

        internal NodeEditorCancellableEventArgs FireNodeEditorValidateStringEvent(NodeEditorCancellableEventArgs args)
        {
            if (this.NodeEditorValidateString != null)
            {
                this.NodeEditorValidateString(this, args);
            }
            return args;
        }
        /// <summary>
        /// Handles the MouseLeftButtonDown event of the treeviewgrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        internal void treeviewgrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        { 
            Point p = e.GetPosition(this);
            pos1 = p.Y;
            Point p1 = e.GetPosition(null);
            p1.X = p1.X + 25;
            //exactItemFromPoint = null; 
            m_Height = 0.0;
          //  double height = GetItemHeightAndWidth(this, this.Items, this);

            IEnumerable<UIElement> collection = VisualTreeHelper.FindElementsInHostCoordinates(p1, this);
            data = null;
            destinationData = null; 
            timer.Stop();
            initialmouseposition = e.GetPosition(treeviewcanvas);
            searchfocus = false;
            if (searchflag)
            {
                ClearSelectedNodes();
                searchflag = false;
            }

            //if (this.ItemsSource == null)
            //{
            if (this.DraggingEnabled)
            {
                mouseLButtonDown = true;

                lastmouseposition = e.GetPosition(Application.Current.RootVisual);

                if (((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None))
                {
                    dragging = false;
                }
                else
                {
                    #region Dragging
                    source_Selected_TVItems = new List<TreeViewItemAdv>();
                    source_Selected_TVItems_Parent = new List<TreeViewItemAdv>();
                    source_TreeView = new List<TreeViewAdv>();
                    TreeViewItemAdv item = exactItemFromPoint;// GetTreeViewItem(e.OriginalSource);
                    if (item != exactItemFromPoint)
                    {
                        if (exactItemFromPoint != null)
                        {
                            if (!(e.OriginalSource is TreeViewItemAdv))
                            {
                                item = exactItemFromPoint;
                            }
                        }
                    }
                    if (item != null)
                    {
                        data = item.Header as object;

                        if ((this.SelectionMode == SelectionMode.MultiSelectAll || this.SelectionMode == SelectionMode.MultiSelectSameLevel) && this.SelectedNodes.Count > 0)
                        {
                            for (int i = 0; i < SelectedNodes.Count; i++)
                            {
                                source_Selected_TVItems.Add(SelectedNodes[i]);
                                if (SelectedNodes[i].ParentNode != null)
                                {
                                    source_Selected_TVItems_Parent.Add(SelectedNodes[i].ParentNode);
                                }
                                else
                                {
                                    source_Selected_TVItems_Parent.Add(null);
                                }

                                source_TreeView.Add(SelectedNodes[i].ParentTreeview);
                            }
                        }
                        else
                        {
                            source_Selected_TVItems.Add(item);
                            if (item.ParentNode != null)
                            {
                                source_Selected_TVItems_Parent.Add(item.ParentNode);
                            }
                            else
                            {
                                source_Selected_TVItems_Parent.Add(null);
                            }

                            source_TreeView.Add(item.ParentTreeview);
                        }

                        clickDragflag = true;
                    }
                    #endregion
                }
            }
            //}

            if (!e.Handled)
            {
                TreeViewItemAdv item = exactItemFromPoint;//GetTreeViewItem(e.OriginalSource);
                if (item != exactItemFromPoint)
                {
                    if (exactItemFromPoint != null)
                    {
                        if (!(e.OriginalSource is TreeViewItemAdv))
                        {
                            item = exactItemFromPoint;
                        }
                    }
                }
                if (item != null)
                {
                    if (!((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None) && !((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None))
                    {
                        if (this.SelectedNodes.Count <= 1)
                        {
                            ClearSelectedNodes();
                            item.Select(true);
                        }
                    }

                    if (e == null)
                    {
                        throw new ArgumentNullException("e");
                    }

                    if (this.IsEnabled)
                    {
                        DateTime now = DateTime.UtcNow;
                        Point position = e.GetPosition(this);
                        double timeDelta = (now - LastClickTime).TotalMilliseconds;
                        Point lastPosition = LastClickPosition;
                        double dx = position.X - lastPosition.X;
                        double dy = position.Y - lastPosition.Y;
                        double distance = (dx * dx) + (dy * dy);
                        if (timeDelta < Milliseconds && distance < PixelsSquared)
                        {
                            ClickCount++;
                        }
                        else
                        {
                            ClickCount = 1;
                        }

                        LastClickTime = now;
                        LastClickPosition = position;
                        IsPressed = true;
                    }
                    else
                    {
                        ClickCount = 1;
                    }

                    if ((ClickCount % 2 == 1) && item.oddClickMade)
                    {
                        isItemHeaderEdit = true;
                    }
                    else if (ClickCount % 2 == 0)
                    {
                        if (item.Items.Count > 0)
                        {
                            if (!item.IsExpanded)
                            {
                                item.IsExpanded = true;
                                item.expander.IsChecked = true;
                            }
                            else
                            {
                                item.IsExpanded = false;
                                item.expander.IsChecked = false;
                            }
                        }
                    }

                    item.oddClickMade = !item.oddClickMade;
                }
            }
            
            e.Handled = true;
        }

        /// <summary>
        /// Handles the Tick event of the timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void timer_Tick(object sender, EventArgs e)
        {
            this.HandleEditMode(true);
            timer.Stop();
        }

        /// <summary>
        /// Gets the thumb.
        /// </summary>
        /// <param name="elem">The elem.</param>
        /// <returns></returns>
        private static Thumb GetThumb(object elem)
        {
            if (elem is UIElement)
            {
                UIElement parent = (UIElement)VisualTreeHelper.GetParent((UIElement)elem);
                if (parent != null)
                {
                    if (parent is Thumb)
                    {
                        return (Thumb)parent;
                    }
                    else
                    {
                        return GetThumb(parent);
                    }
                }
            }

            return null;
        }

        internal TreeViewItemAdv currentItemAdv;
        /// <summary>
        /// Handles the MouseLeftButtonUp event of the treeviewgrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void treeviewgrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isItemHeaderEdit && this.SelectedNodes.Count == 0)
            {
                isItemHeaderEdit = false;
                timer.Start();
                currentItemAdv = GetTreeViewItem(e.OriginalSource);
                //if (currentItemAdv == null)
                //{
                //    currentItemAdv = GetTreeViewItem(e.OriginalSource);
                //}

                
                if (currentItemAdv != exactItemFromPoint)
                {
                    if (exactItemFromPoint != null)
                    {
                        if (!(e.OriginalSource is TreeViewItemAdv))
                        {
                            currentItemAdv = exactItemFromPoint;
                        }
                    }
                }
            }

            Thumb thumb = GetThumb(e.OriginalSource);
            if (thumb != null)
            {
                return;
            }

            if (!((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None))
            {
                node =  GetTreeViewItem(e.OriginalSource);
                //if (node == null)
                //{
                //    node = GetTreeViewItem(e.OriginalSource);
                //}
                if (node != exactItemFromPoint)
                {
                    if (exactItemFromPoint != null)
                    {
                        if (!(e.OriginalSource is TreeViewItemAdv))
                        {
                            node = exactItemFromPoint;
                        }
                    }
                }
            }

            if (this.SelectionMode == SelectionMode.MultiSelectAll || this.SelectionMode == SelectionMode.MultiSelectSameLevel)
            {
                if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    bool bIsSelected = SelectedNodes.Contains(node);

                    if (this.SelectionMode == SelectionMode.MultiSelectSameLevel)
                    {
                        if (this.SelectedNode != null)
                        {
                            if (node.ParentNode == this.SelectedNode.ParentNode)
                            {
                                ToggleNode(node, !bIsSelected);
                            }
                        }
                        else
                        {
                            ToggleNode(node, !bIsSelected);
                        }
                    }
                    else
                    {
                        ToggleNode(node, !bIsSelected);
                    }
                }

                #region Shift Select
                else if (((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None))
                {
                    node = this.SelectedNode;
                    if (this.SelectedNode == null)
                    {
                    }

                    ////Changed for Volume 4 Release
                    //TreeViewItemAdv ndStart = node;

                    TreeViewItemAdv ndStart = this.SelectedNode;

                    ClearSelectedNodes();
                    SelectSingleNode(ndStart);

                    TreeViewItemAdv ndEnd = GetTreeViewItem(e.OriginalSource);

                    if (ndStart.ParentNode == ndEnd.ParentNode)
                    {
                        int i = ItemIndex(ndStart);
                        int j = ItemIndex(ndEnd);
                        if (ItemIndex(ndStart) < ItemIndex(ndEnd))
                        {
                            while (ndStart != ndEnd)
                            {
                                ndStart = ndStart.GetNextNode(true);
                                if (ndStart == null)
                                {
                                    break;
                                }
                                if (this.SelectionMode == SelectionMode.MultiSelectSameLevel)
                                {
                                    if (node.ParentNode == ndStart.ParentNode)
                                    {
                                        ToggleNode(ndStart, true);
                                    }
                                }
                                else
                                {
                                    if (node.ParentNode == ndStart.ParentNode)
                                    {
                                        ToggleNode(ndStart, true);
                                    }
                                }
                            }
                        }

                        if (ItemIndex(ndStart) == ItemIndex(ndEnd))
                        {
                        }
                        else
                        {
                            if (this.SelectionMode == SelectionMode.MultiSelectSameLevel)
                            {
                                if (node.ParentNode == ndStart.ParentNode)
                                {
                                    ToggleNode(ndStart, true);
                                }
                            }
                            else
                            {
                                if (node.ParentNode == ndStart.ParentNode)
                                {
                                    ToggleNode(ndStart, true);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (this.SelectionMode == SelectionMode.MultiSelectAll)
                        {
                            TreeViewItemAdv ndStartP = ndStart;
                            TreeViewItemAdv ndEndP = ndEnd;
                            int startDepth = Math.Min(ndStartP.Level, ndEndP.Level);
                            while (ndStartP.Level > startDepth)
                            {
                                ndStartP = ndStartP.ParentNode;
                            }

                            while (ndEndP.Level > startDepth)
                            {
                                ndEndP = ndEndP.ParentNode;
                            }

                            while (ndStartP.ParentNode != ndEndP.ParentNode)
                            {
                                ndStartP = ndStartP.ParentNode;
                                ndEndP = ndEndP.ParentNode;
                            }

                            if (ItemIndex(ndStartP) < ItemIndex(ndEndP))
                            {
                                while (ndStart != ndEnd)
                                {
                                    ndStart = ndStart.GetNextNode(true);
                                    if (ndStart == null)
                                    {
                                        break;
                                    }

                                    ToggleNode(ndStart, true);
                                }
                            }
                            else if (ItemIndex(ndStartP) == ItemIndex(ndEndP))
                            {
                                if (ndStart.Level < ndEnd.Level)
                                {
                                    while (ndStart != ndEnd)
                                    {
                                        ndStart = ndStart.GetNextNode(true);
                                        if (ndStart == null)
                                        {
                                            break;
                                        }

                                        ToggleNode(ndStart, true);
                                    }
                                }
                                else
                                {
                                    while (ndStart != ndEnd)
                                    {
                                        ndStart = ndStart.GetPreviousNode(true);
                                        if (ndStart == null)
                                        {
                                            break;
                                        }

                                        ToggleNode(ndStart, true);
                                    }
                                }
                            }
                            else
                            {
                                while (ndStart != ndEnd)
                                {
                                    ndStart = ndStart.GetPreviousNode(true);
                                    if (ndStart == null)
                                    {
                                        break;
                                    }

                                    ToggleNode(ndStart, true);
                                }
                            }
                        }
                    }
                }
                #endregion

                else
                {
                    if (!this.DraggingEnabled)
                    {
                        ClearSelectedNodes();
                        SelectSingleNode(node);
                    }
                    else
                    {
                        ClearSelectedNodes();
                    }
                }
            }

            mouseLButtonDown = false;
            TreeViewAdv currentTreeView = GetTreeView(e.OriginalSource);

            try
            {
                if (!((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None) && !((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None))
                {
                    TreeViewItemAdv item1 =  GetTreeViewItem(e.OriginalSource);
                    //if (item1 == null)
                    //{
                    //    item1 = GetTreeViewItem(e.OriginalSource);
                    //}
                    if (item1 != exactItemFromPoint)
                    {
                        if (exactItemFromPoint != null)
                        {
                            if (!(e.OriginalSource is TreeViewItemAdv))
                            {
                                item1 = exactItemFromPoint;
                            }
                        }
                    }
                    if (item1 != null) exactItemFromPoint = null;
                    {
                        item1.Select(true);
                    }
                }

                if (currentTreeView == null)
                {
                    RefreshRootLines(this);
                    return;
                }
            }
            catch
            {
            }
            finally
            {
                popup.Child = null;
                popup.IsOpen = false;
                this.dragging = false;
                this.Cursor = Cursors.Arrow;
                dragging = false;
                e.Handled = false;
            }
            exactItemFromPoint = null;
        }

        /// <summary>
        /// Handles the MouseMove event of the treeviewcanvas control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void treeviewcanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.dragging)
            {
                TreeViewAdv treeView = null;
                IEnumerable<UIElement> treeViewAdvCollection = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), Application.Current.RootVisual);
                foreach (TreeViewAdv treeViewAdv in treeViewAdvCollection.OfType<TreeViewAdv>())
                {
                    if (treeViewAdv != this)
                    {
                        treeView = treeViewAdv;
                        break;
                    }
                }//GetTreeView(e.OriginalSource);
                if (treeView == null)
                {
                    treeView = this;
                }
                if ((treeView != null) && treeView.DraggingEnabled)
                {
                    TreeViewItemAdv previousNode;
                    Point position = e.GetPosition(this.treeviewcanvas);
                    IEnumerable<UIElement> enumerable = VisualTreeHelper.FindElementsInHostCoordinates(position, this.treeviewcanvas);
                    this.popup.VerticalOffset = position.Y - 5.0;
                    this.popup.HorizontalOffset = position.X - 10.0;
                    if (this.trackTreeviewitem != null)
                    {
                        previousNode = this.trackTreeviewitem.GetPreviousNode(true);
                        if (treeView == null)
                        {
                        }
                        if (previousNode != null)
                        {
                            previousNode.IsMouseOver = false;
                            previousNode.DragLineVisibility = Visibility.Collapsed;
                            this.trackTreeviewitem.IsMouseOver = false;
                            this.trackTreeviewitem.DragLineVisibility = Visibility.Collapsed;
                        }
                    }
                    Point point2 = e.GetPosition(sender as UIElement);
                    this.pos1 = point2.Y;
                    this.exactItemFromPoint = null;
                    this.m_Height = 0.0;
                    //double itemHeightAndWidth = this.GetItemHeightAndWidth(this, treeView.Items,treeView);
                    IList uilist = (VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(null), this) as IList);
                    List<TreeViewItemAdv> tiadvcol = new List<TreeViewItemAdv>();
                    foreach (var item in uilist)
                    {
                        if (item as TreeViewItemAdv != null)
                        {
                            tiadvcol.Add(item as TreeViewItemAdv);                          
                        }
                    }
                    
                    if (tiadvcol != null && tiadvcol.Count > 0)
                    {
                        if (tiadvcol.Count == 1)
                            exactItemFromPoint = tiadvcol[0] as TreeViewItemAdv;
                        else
                        {
                            exactItemFromPoint = tiadvcol[1] as TreeViewItemAdv;
                        }            
                    }
                    this.trackTreeviewitem = this.exactItemFromPoint;
                    if ((this.trackTreeviewitem != null))
                    {
                        if (this.trackTreeviewitem.ParentTreeview != null)
                        {
                            //if (this.trackTreeviewitem.ParentTreeview.DragMoved != null)
                            //{
                            //    this.trackTreeviewitem.ParentTreeview.DragMoved(this.trackTreeviewitem.ParentTreeview, new TreeViewAdvDragEventArgs(this.source_Selected_TVItems, this.source_Selected_TVItems_Parent, this.source_TreeView, this.target_Selected_TVItem, this.target_Selected_TVItem_Parent, this.target_TreeView, this.data, this.destinationData));
                            //}
                            if ((this.trackTreeviewitem != null) && (this.trackTreeviewitem.ParentTreeview != null))
                            {
                                this.trackTreeviewitem.ParentTreeview.Focus();
                            }
                            if ((this.trackTreeviewitem != null) && (this.popup != null))
                            {
                                double actualHeight;
                                DragMoveEventArgs args;
                                if (this.trackTreeviewitem.HasItems)
                                {
                                    actualHeight = this.trackTreeviewitem.HeaderBorder.ActualHeight;
                                }
                                else
                                {
                                    actualHeight = this.trackTreeviewitem.ActualHeight;
                                }
                                double num3 = actualHeight / 3.0;
                                Point point3 = e.GetPosition(this.trackTreeviewitem);
                                Point point4 = e.GetPosition(this);
                                bool flag = false;
                                //if ((point3.Y >= 0.0) && (point3.Y <= (num3 - 2.0)))
                                if((point3.Y>=0.0) && (point3.Y<=(num3-2.0)))
                                {
                                    this.dropmode = DropMode.DropUp;
                                    args = new DragMoveEventArgs(this.trackTreeviewitem);
                                    if(this.DragQuery!=null)//if (this.DragAbove != null)
                                    {
                                        this.DragQuery(this, new TreeViewAdvDragEventArgs(source_Selected_TVItems, source_Selected_TVItems_Parent, source_TreeView,
                                                                                        target_Selected_TVItem, target_Selected_TVItem_Parent, target_TreeView, this.dropmode));
                                        //this.DragAbove(this.trackTreeviewitem.ParentTreeview, args);
                                    }
                                    previousNode = this.trackTreeviewitem.GetPreviousNode(true);
                                    if ((previousNode != null) && (this.trackTreeviewitem.ParentTreeview.DragLineVisibility == Visibility.Visible))
                                    {
                                        previousNode.DragLineVisibility = Visibility.Visible;
                                        previousNode.DragLineColor = this.trackTreeviewitem.ParentTreeview.DragLineColor;
                                    }
                                    this.trackTreeviewitem.IsMouseOver = false;
                                }
                                else if ((point3.Y>=(num3-2.0))&&(point3.Y<=((2*num3)+2.0)))//((point3.Y >= (num3 + 1.0)) && (point3.Y <= ((num3 + num3) + 2.0)))
                                {
                                    //this.dropmode = DropMode.DropMiddle;
                                    this.dropmode = DropMode.DropOver;
                                    args = new DragMoveEventArgs(this.trackTreeviewitem);
                                    if(this.DragQuery!=null)//if (this.DragOver != null)
                                    {
                                        this.DragQuery(this, new TreeViewAdvDragEventArgs(source_Selected_TVItems, source_Selected_TVItems_Parent, source_TreeView,
                                                                                        target_Selected_TVItem, target_Selected_TVItem_Parent, target_TreeView, this.dropmode));
                                        //this.DragOver(this, args);
                                    }
                                    this.trackTreeviewitem.DragLineVisibility = Visibility.Collapsed;
                                    previousNode = this.trackTreeviewitem.GetPreviousNode(true);
                                    if (previousNode != null)
                                    {
                                        previousNode.DragLineVisibility = Visibility.Collapsed;
                                    }
                                    if (this.trackTreeviewitem.ParentTreeview != null)
                                    {
                                        this.RefreshRootLines(this.trackTreeviewitem.ParentTreeview);
                                    }
                                    this.trackTreeviewitem.IsMouseOver = true;
                                }
                                else if((point3.Y>=((num3*2)+2.0)) && (point3.Y<=actualHeight))//((point3.Y >= (((num3 + num3) + 2.0) + 1.0)) && (point3.Y <= actualHeight))
                                {
                                    this.dropmode = DropMode.DropDown;
                                    args = new DragMoveEventArgs(this.trackTreeviewitem);
                                    if(this.DragQuery!=null)//if (this.DragBelow != null)
                                    {
                                        this.DragQuery(this, new TreeViewAdvDragEventArgs(source_Selected_TVItems, source_Selected_TVItems_Parent, source_TreeView,
                                                                                        target_Selected_TVItem, target_Selected_TVItem_Parent, target_TreeView, this.dropmode));
                                        //this.DragBelow(this, args);
                                    }
                                    if (this.DragLineVisibility == Visibility.Visible)
                                    {
                                        this.trackTreeviewitem.DragLineVisibility = Visibility.Visible;
                                        this.trackTreeviewitem.DragLineColor = this.trackTreeviewitem.ParentTreeview.DragLineColor;
                                    }
                                    this.trackTreeviewitem.IsMouseOver = false;
                                }
                                if ((point4.X >= (treeView.ActualWidth - 20.0)) || (point4.X < (((double)treeView.GetValue(Canvas.LeftProperty)) + 20.0)))
                                {
                                    flag = true;
                                    treeView.DragLineVisibility = Visibility.Collapsed;
                                }
                                else if ((point4.X < treeView.ActualWidth) && ((point4.X + 20.0) > ((double)treeView.GetValue(Canvas.LeftProperty))))
                                {
                                    flag = false;
                                    treeView.DragLineVisibility = Visibility.Visible;
                                }
                                TreeViewAdv adv3 = this;
                                if (adv3 != null)
                                {
                                    adv3.CaptureMouse();
                                    adv3.Focus();
                                    if (flag)
                                    {
                                        adv3.DragLineVisibility = Visibility.Collapsed;
                                    }
                                    else
                                    {
                                        adv3.DragLineVisibility = Visibility.Visible;
                                    }
                                }
                            }
                            else
                            {
                                this.Previousmouseoveritem = this.Previousmouseoveritem1;
                            }
                        }
                    }
                }
            }
        }

        bool allowtopmost = false;
        /// <summary>
        /// Handles the MouseLeftButtonUp event of the treeviewcanvas control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void treeviewcanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (this.dragging)
                {

                    this.mouseLButtonDown = false;

                    #region Finds the TreeView to Drag Items
                    IEnumerable<UIElement> treeViewAdvCollection = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(Application.Current.RootVisual), Application.Current.RootVisual);
                    TreeViewAdv treeView = null;
                    foreach (TreeViewAdv treeViewAdv in treeViewAdvCollection.OfType<TreeViewAdv>())
                    {
                        if (treeViewAdv != this)
                        {
                            treeView = treeViewAdv;
                            break;
                        }
                    }

                    if (treeView == null)
                    {
                        treeView = this;
                    }
                    #endregion

                    #region Collapse the DragLines
                    if (this.trackTreeviewitem != null)
                    {
                        TreeViewItemAdv previousNode = this.trackTreeviewitem.GetPreviousNode(true);
                        if (treeView != null)
                        {
                            treeView.ReleaseMouseCapture();
                            treeView.DragLineVisibility = Visibility.Collapsed;
                        }
                        if (previousNode != null)
                        {
                            previousNode.IsMouseOver = false;
                            previousNode.DragLineVisibility = Visibility.Collapsed;
                            this.trackTreeviewitem.IsMouseOver = false;
                            this.trackTreeviewitem.DragLineVisibility = Visibility.Collapsed;
                        }
                    }
                    #endregion

                    if (treeView == null)
                    {
                        this.RefreshRootLines(this);
                        treeView.RefreshRootLines(treeView);
                    }
                    else
                    {
                        if (this.DraggingEnabled)
                        {
                            this.destinationData = null;
                            this.dragging = false;
                            TreeViewItemAdv treeviewItem;
                            Application.Current.RootVisual.ReleaseMouseCapture();
                            TreeViewItemAdv selectedNode = this.SelectedNode;
                            Point position = e.GetPosition(this);
                            this.pos1 = position.Y;
                            this.exactItemFromPoint = null;
                            this.m_Height = 0.0;
                        //    double itemHeightAndWidth = this.GetItemHeightAndWidth(this, treeView.Items, treeView);
                            IList uilist = (VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(null), this) as IList);
                            List<TreeViewItemAdv> tiadvcol = new List<TreeViewItemAdv>();
                            foreach (var item in uilist)
                            {
                                if (item as TreeViewItemAdv != null)
                                    tiadvcol.Add(item as TreeViewItemAdv);
                            }
                            if (tiadvcol != null && tiadvcol.Count > 0)
                            {
                                if (tiadvcol.Count == 1)
                                    this.exactItemFromPoint = tiadvcol[0] as TreeViewItemAdv;
                                else 
                                {
                                   this.exactItemFromPoint = tiadvcol[1] as TreeViewItemAdv;
                                }                             
                            }
                            TreeViewItemAdv exactItemFromPoint = this.exactItemFromPoint;
                            allowtopmost = false;
                            if (position.Y < 5)
                            {
                                exactItemFromPoint = null;
                                allowtopmost = true;
                            }
                             TreeViewAdvDragEventArgs args= null;
                            if (exactItemFromPoint != null)
                            {

                                this.target_TreeView = exactItemFromPoint.ParentTreeview;
                                this.target_Selected_TVItem_Parent = exactItemFromPoint.ParentNode;
                                this.target_Selected_TVItem = exactItemFromPoint;

                               args = new TreeViewAdvDragEventArgs(source_Selected_TVItems, source_Selected_TVItems_Parent, source_TreeView,
                                                        target_Selected_TVItem, target_Selected_TVItem_Parent, target_TreeView, this.dropmode);
                                if (this.DragDrop != null)
                                {
                                    this.DragDrop(this, args);
                                }
                            }
                            #region DragInside Tree View
                            if (exactItemFromPoint != null && position.Y > 5)
                            {
                                this.targetListcollection = new List<TreeViewItemAdv>();
                                treeviewItem = exactItemFromPoint;
                                this.target_TreeView = exactItemFromPoint.ParentTreeview;
                                this.target_Selected_TVItem_Parent = exactItemFromPoint.ParentNode;
                                this.target_Selected_TVItem = exactItemFromPoint;
                                this.treeViewItemsCollection = new Dictionary<string, TreeViewItemAdv>();

                                if (this.target_Selected_TVItem != null)
                                {
                                    //this.destinationData = this.target_Selected_TVItem.Header;
                                    if (this.dropmode == DropMode.DropDown || this.dropmode == DropMode.DropUp)
                                    {
                                        this.destinationData = (target_Selected_TVItem.ParentItemsControl as TreeViewItemAdv) != null ? (target_Selected_TVItem.ParentItemsControl as TreeViewItemAdv).Header : this.target_Selected_TVItem.Header;
                                    }
                                }

                                #region DragFinished Event
                                //TreeViewAdvDragEventArgs args = null;
                                //if (this.DragFinished != null)
                                //{
                                //    args = new TreeViewAdvDragEventArgs(this.source_Selected_TVItems, this.source_Selected_TVItems_Parent, this.source_TreeView, this.target_Selected_TVItem, this.target_Selected_TVItem_Parent, this.target_TreeView, this.data, this.destinationData);
                                //    this.DragFinished(this, args);
                                //}
                                #endregion

                                #region WithOut ItemSource
                                if (this.ItemsSource == null && !args.Handled)//if (this.ItemsSource == null && !e.Handled)
                                {
                                    if (this.dropmode == DropMode.DropOver)//if (this.dropmode == DropMode.DropMiddle)
                                    {
                                        for (int i = 0; i < this.source_Selected_TVItems.Count; i++)
                                        {
                                            this.MouseDropOver(this.source_Selected_TVItems[i], this.source_Selected_TVItems_Parent[i], this.source_TreeView[i]);
                                            this.source_Selected_TVItems[i].Select(false);
                                            this.RefreshRootLines(this.target_TreeView);
                                            this.RefreshRootLines(this);
                                        }
                                    }
                                    else if (this.dropmode == DropMode.DropDown)
                                    {
                                        for (int i = 0; i < this.source_Selected_TVItems.Count; i++)
                                        {
                                            this.MouseDropAfter(this.source_Selected_TVItems[i], this.source_Selected_TVItems_Parent[i], this.source_TreeView[i]);
                                            this.source_Selected_TVItems[i].Select(false);
                                            this.RefreshRootLines(this.target_TreeView);
                                            this.RefreshRootLines(this.source_TreeView[i]);
                                        }
                                    }

                                    else if (this.dropmode == DropMode.DropUp)
                                    {
                                        for (int i = 0; i < this.source_Selected_TVItems.Count; i++)
                                        {
                                            this.MouseDropBefore(this.source_Selected_TVItems[i], this.source_Selected_TVItems_Parent[i], this.source_TreeView[i]);
                                            this.source_Selected_TVItems[i].Select(false);
                                            this.RefreshRootLines(this.target_TreeView);
                                            this.RefreshRootLines(this.source_TreeView[i]);
                                        }
                                    }
                                }
                                #endregion

                                #region WithItemSource
                                else if (!e.Handled)
                                {
                                    //TreeViewItemAdv destination = GetPreviousItemFromHeightAndWidth(this, base.Items);
                                    //if (this.dropmode == DropMode.DropMiddle)
                                    //{
                                    //    for (int i = 0; i < this.source_Selected_TVItems.Count; i++)
                                    //    {
                                    //        int oldIndex = this.source_Selected_TVItems[i].ParentItemsControl.ItemContainerGenerator.IndexFromContainer(this.source_Selected_TVItems[i]);
                                    //        //int newIndex = (exactItemFromPoint as TreeViewItemAdv).ItemContainerGenerator.IndexFromContainer(exactItemFromPoint);
                                    //        if (this.source_Selected_TVItems[i].ItemsSource != null)
                                    //        {
                                    //            IList collview = this.source_Selected_TVItems[i].ParentItemsControl.ItemsSource as IList;
                                    //            object o = collview[oldIndex];
                                    //            collview.RemoveAt(oldIndex);
                                    //            collview = (exactItemFromPoint as TreeViewItemAdv).ItemsSource as IList;
                                    //            collview.Add(o);
                                    //        }
                                    //    }
                                    //}
                                    //else if (this.dropmode == DropMode.DropDown || this.dropmode == DropMode.DropUp)
                                    //{
                                    //    for (int i = 0; i < this.source_Selected_TVItems.Count; i++)
                                    //    {
                                    //        int oldIndex = this.source_Selected_TVItems[i].ParentItemsControl.ItemContainerGenerator.IndexFromContainer(this.source_Selected_TVItems[i]);
                                    //        int newIndex = (exactItemFromPoint.ParentItemsControl as TreeViewItemAdv).ItemContainerGenerator.IndexFromContainer(exactItemFromPoint);
                                    //        if (this.source_Selected_TVItems[i].ItemsSource != null)
                                    //        {
                                    //            IList collview = this.source_Selected_TVItems[i].ParentItemsControl.ItemsSource as IList;
                                    //            object o = collview[oldIndex];
                                    //            collview.RemoveAt(oldIndex);
                                    //            collview = (exactItemFromPoint.ParentItemsControl as TreeViewItemAdv).ItemsSource as IList;
                                    //            if (this.dropmode == DropMode.DropUp)
                                    //                collview.Insert(newIndex, o);
                                    //            else
                                    //                collview.Add(o);
                                    //        }
                                    //    }
                                    //}
                                    //RefreshRootLinesWithBinding(this);

                                    /*
                                    TreeViewItemAdv destination = GetPreviousItemFromHeightAndWidth(this, base.Items);
                                    if (this.dropmode == DropMode.DropMiddle)
                                    {
                                        for (int i = 0; i < this.source_Selected_TVItems.Count; i++)
                                        {
                                            int oldIndex = this.source_Selected_TVItems[i].ParentItemsControl.ItemContainerGenerator.IndexFromContainer(this.source_Selected_TVItems[i]);
                                            //int newIndex = (exactItemFromPoint as TreeViewItemAdv).ItemContainerGenerator.IndexFromContainer(exactItemFromPoint);
                                            if (this.source_Selected_TVItems[i].ItemsSource != null)
                                            {
                                                IList collview_source = this.source_Selected_TVItems[i].ParentItemsControl.ItemsSource as IList;

                                                bool flag = CheckForTargetUnderSource(this.source_Selected_TVItems[i].Items, exactItemFromPoint);

                                                object o = collview_source[oldIndex];
                                                IList collview_target = (exactItemFromPoint as TreeViewItemAdv).ItemsSource as IList;
                                                collview_source.RemoveAt(oldIndex);
                                                collview_target.Add(o);
                                            }
                                        }
                                    }
                                    else if (this.dropmode == DropMode.DropDown || this.dropmode == DropMode.DropUp)
                                    {
                                        for (int i = 0; i < this.source_Selected_TVItems.Count; i++)
                                        {
                                            int oldIndex = this.source_Selected_TVItems[i].ParentItemsControl.ItemContainerGenerator.IndexFromContainer(this.source_Selected_TVItems[i]);
                                            int newIndex = (exactItemFromPoint.ParentItemsControl as TreeViewItemAdv).ItemContainerGenerator.IndexFromContainer(exactItemFromPoint);
                                            if (this.source_Selected_TVItems[i].ItemsSource != null)
                                            {
                                                IList collview = this.source_Selected_TVItems[i].ParentItemsControl.ItemsSource as IList;
                                                object o = collview[oldIndex];
                                                collview.RemoveAt(oldIndex);
                                                collview = (exactItemFromPoint.ParentItemsControl as TreeViewItemAdv).ItemsSource as IList;
                                                if (this.dropmode == DropMode.DropUp)
                                                    collview.Insert(newIndex, o);
                                                else
                                                    collview.Add(o);
                                            }
                                        }
                                    }
                                    RefreshRootLinesWithBinding(this);*/
                                }
                                #endregion
                            }
                            #endregion

                            else
                            {
                                TreeViewAdv targetTreeView = treeView;
                                if (targetTreeView.ItemsSource == null && args != null && !args.Handled)
                                {
                                    //if (this.DragFinished != null)
                                    //{
                                    //    this.DragFinished(this, new TreeViewAdvDragEventArgs(this.source_Selected_TVItems, this.source_Selected_TVItems_Parent, this.source_TreeView, null, null, targetTreeView, this.data, this.destinationData));
                                    //}
                                    for (int i = 0; i < this.source_Selected_TVItems.Count; i++)
                                    {
                                        this.MouseDropOnTreeView(this.source_Selected_TVItems[i], this.source_Selected_TVItems_Parent[i], this.source_TreeView[i], targetTreeView);
                                        this.RefreshRootLines(this);
                                        this.RefreshRootLines(targetTreeView);
                                        source_Selected_TVItems[i].ParentTreeview = targetTreeView;
                                        this.source_Selected_TVItems[i].Select(true);
                                    }
                                }
                                else
                                {

                                }
                            }
                        }
                        //else if (this.source_Selected_TVItems[0] != null)
                        //{
                        //    this.RefreshRootLines(this.source_TreeView[0]);
                        //}
                        //else
                        //{
                        //    for (num2 = 0; num2 < this.source_Selected_TVItems.Count; num2++)
                        //    {
                        //        this.RefreshRootLines(this.sourcetreeviews[num2]);
                        //    }
                        //}
                    }

                }
            }
            catch
            {
            }
            this.popup.Child = null;
            this.popup.IsOpen = false;
            this.dragging = false;
            base.Cursor = Cursors.Arrow;
            this.dragging = false;
            e.Handled = false;
        }


        
     

        /// <summary>
        /// Mouses the drop on tree view.
        /// </summary>
        /// <param name="sourcetreeviewitem">The sourcetreeviewitem.</param>
        /// <param name="sourceparenttreeviewitem">The sourceparenttreeviewitem.</param>
        /// <param name="sourcetreeview">The sourcetreeview.</param>
        /// <param name="targettreeview">The target_TreeView.</param>
        private void MouseDropOnTreeView(TreeViewItemAdv sourcetreeviewitem, TreeViewItemAdv sourceparenttreeviewitem, TreeViewAdv sourcetreeview, TreeViewAdv targettreeview)
        {
            if (sourcetreeviewitem != target_Selected_TVItem)
            {
                if (sourceparenttreeviewitem != null)
                {
                    if (sourcetreeviewitem is TreeViewItemAdv)
                    {
                        if (((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None) != true)
                        {
                            sourceparenttreeviewitem.Items.Remove(sourcetreeviewitem);
                            if (sourceparenttreeviewitem.Items.Count > 0)
                            {
                                sourceparenttreeviewitem.HasItems = true;
                            }
                        }

                        if (((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None) == true && !(this.SelectionMode == SelectionMode.MultiSelectAll || this.SelectionMode == SelectionMode.MultiSelectSameLevel))
                        {
                            TreeViewItemAdv item1 = new TreeViewItemAdv();
                            item1 = TreeViewItemClone(sourcetreeviewitem);
                            if (item1 == null)
                            {
                                return;
                            }

                            if (targettreeview.Items.Count > 0 && allowtopmost)
                            {
                                targettreeview.Items.Insert(0, item1);
                            }
                            else
                            {
                                targettreeview.Items.Add(item1);
                            }
                        }
                        else
                        {
                            if (targettreeview.Items.Count > 0 && allowtopmost)
                            {
                                targettreeview.Items.Insert(0, sourcetreeviewitem);
                            }
                            else
                            {
                                targettreeview.Items.Add(sourcetreeviewitem);
                            }
                        }
                    }
                }
                else
                {
                    if (sourcetreeviewitem is TreeViewItemAdv)
                    {
                        if (((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None) != true)
                        {
                            flag1 = false;
                            bool t = CheckForTargetUnderSource(sourcetreeviewitem.Items, target_Selected_TVItem);
                            if (CheckForTargetUnderSource(sourcetreeviewitem.Items, target_Selected_TVItem))
                            {
                                return;
                            }

                            sourcetreeview.Items.Remove(sourcetreeviewitem);
                        }

                        if (((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None) == true)
                        {
                            TreeViewItemAdv item1 = new TreeViewItemAdv();
                            item1 = TreeViewItemClone(sourcetreeviewitem);
                            if (item1 == null)
                            {
                                return;
                            }
                            if (targettreeview.Items.Count > 0 && allowtopmost)
                            {
                                targettreeview.Items.Insert(0, item1);
                            }
                            else
                            {
                                targettreeview.Items.Add(item1);
                            }
                        }
                        else
                        {
                            if (targettreeview.Items.Count > 0 && allowtopmost)
                            {
                                targettreeview.Items.Insert(0, sourcetreeviewitem);
                            }
                            else
                            {
                                targettreeview.Items.Add(sourcetreeviewitem);
                            } 
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the tree view drop before.
        /// </summary>
        /// <param name="item">The item.</param>
        private void HandleTreeViewDropBefore(TreeViewItemAdv item)
        {
            if (target_TreeView != null)
            {
                int targetindex = target_TreeView.Items.IndexOf(target_Selected_TVItem);
                if (item.ParentNode == null)
                {
                }
                else
                {
                    item.ParentNode = null;
                }

                if (targetindex <= 0)
                {
                    target_TreeView.Items.Insert(0, item);
                }
                else
                {
                    target_TreeView.Items.Insert(targetindex, item);
                }
            }
        }

        /// <summary>
        /// Inserts the before.
        /// </summary>
        /// <param name="targetindex">The targetindex.</param>
        /// <param name="item">The item.</param>
        private void InsertBefore(int targetindex, TreeViewItemAdv item)
        {
            if (targetindex <= 0)
            {
                target_Selected_TVItem_Parent.Items.Insert(0, item);
            }
            else
            {
                target_Selected_TVItem_Parent.Items.Insert(targetindex, item);
            }
        }

        /// <summary>
        /// Mouses the drop before.
        /// </summary>
        /// <param name="sourcetreeviewitem">The sourcetreeviewitem.</param>
        /// <param name="sourceparenttreeviewitem">The sourceparenttreeviewitem.</param>
        /// <param name="sourcetreeview">The sourcetreeview.</param>
        private void MouseDropBefore(TreeViewItemAdv sourcetreeviewitem, TreeViewItemAdv sourceparenttreeviewitem, TreeViewAdv sourcetreeview)
        {
            if (sourcetreeviewitem != target_Selected_TVItem)
            {
                if (sourceparenttreeviewitem != null)
                {
                    //// Within the particular TreeViewItemAdv
                    if (sourcetreeviewitem is TreeViewItemAdv && target_Selected_TVItem is TreeViewItemAdv)
                    {
                        if (((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None) != true)
                        {
                            flag1 = false;
                            bool t = CheckForTargetUnderSource(sourcetreeviewitem.Items, target_Selected_TVItem);
                            if (CheckForTargetUnderSource(sourcetreeviewitem.Items, target_Selected_TVItem))
                            {
                                return;
                            }

                            sourceparenttreeviewitem.Items.Remove(sourcetreeviewitem);
                            if (sourceparenttreeviewitem.Items.Count > 0)
                            {
                                sourceparenttreeviewitem.HasItems = true;
                            }
                        }

                        if (((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None) == true && !(this.SelectionMode == SelectionMode.MultiSelectAll || this.SelectionMode == SelectionMode.MultiSelectSameLevel))
                        {
                            TreeViewItemAdv item1 = new TreeViewItemAdv();
                            item1 = TreeViewItemClone(sourcetreeviewitem);
                            if (item1 == null)
                            {
                                return;
                            }

                            if (target_Selected_TVItem_Parent == null)
                            {
                                HandleTreeViewDropBefore(item1);
                                return;
                            }

                            int targetindex = target_Selected_TVItem_Parent.Items.IndexOf(target_Selected_TVItem);
                            InsertBefore(targetindex, item1);
                        }
                        else
                        {
                            if (target_Selected_TVItem_Parent == null)
                            {
                                HandleTreeViewDropBefore(sourcetreeviewitem);
                                return;
                            }

                            int targetindex = target_Selected_TVItem_Parent.Items.IndexOf(target_Selected_TVItem);
                            InsertBefore(targetindex, sourcetreeviewitem);
                        }

                        target_Selected_TVItem_Parent.expander.IsChecked = true;
                        sourceparenttreeviewitem.expander.IsChecked = true;
                    }
                }
                else
                {
                    //// Dragging takes place in TreeViewAdv
                    if (sourcetreeviewitem != target_Selected_TVItem_Parent)
                    {
                        if (((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None) != true)
                        {
                            flag1 = false;
                            bool t = CheckForTargetUnderSource(sourcetreeviewitem.Items, target_Selected_TVItem);
                            if (CheckForTargetUnderSource(sourcetreeviewitem.Items, target_Selected_TVItem))
                            {
                                return;
                            }

                            sourcetreeview.Items.Remove(sourcetreeviewitem);
                        }

                        if (((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None) == true && !(this.SelectionMode == SelectionMode.MultiSelectAll || this.SelectionMode == SelectionMode.MultiSelectSameLevel))
                        {
                            TreeViewItemAdv item1 = new TreeViewItemAdv();
                            item1 = TreeViewItemClone(sourcetreeviewitem);
                            if (item1 == null)
                            {
                                return;
                            }

                            if (target_Selected_TVItem_Parent == null)
                            {
                                HandleTreeViewDropBefore(item1);
                                return;
                            }

                            int targetindex = target_Selected_TVItem_Parent.Items.IndexOf(target_Selected_TVItem);
                            InsertBefore(targetindex, item1);
                        }
                        else
                        {
                            if (target_Selected_TVItem_Parent == null)
                            {
                                HandleTreeViewDropBefore(sourcetreeviewitem);
                                return;
                            }

                            int targetindex = target_Selected_TVItem_Parent.Items.IndexOf(target_Selected_TVItem);
                            InsertBefore(targetindex, sourcetreeviewitem);
                        }

                        target_Selected_TVItem.IsSelected = true;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the tree view drop after.
        /// </summary>
        /// <param name="item">The item.</param>
        private void HandleTreeViewDropAfter(TreeViewItemAdv item)
        {
            if (target_TreeView != null)
            {
                int targetindex = target_TreeView.Items.IndexOf(target_Selected_TVItem);

                if (item.ParentNode == null)
                {
                }
                else
                {
                    item.ParentNode = null;
                }

                if (targetindex + 1 > (target_TreeView.Items.Count - 1))
                {
                    target_TreeView.Items.Add(item);
                }
                else
                {
                    target_TreeView.Items.Insert(targetindex + 1, item);
                }
            }
        }

        /// <summary>
        /// Inserts the after.
        /// </summary>
        /// <param name="targetindex">The targetindex.</param>
        /// <param name="item">The item.</param>
        private void InsertAfter(int targetindex, TreeViewItemAdv item)
        {
            if (targetindex + 1 > (target_Selected_TVItem_Parent.Items.Count - 1))
            {
                target_Selected_TVItem_Parent.Items.Add(item);
            }
            else
            {
                target_Selected_TVItem_Parent.Items.Insert(targetindex + 1, item);
            }
        }

        /// <summary>
        /// Mouses the drop after.
        /// </summary>
        /// <param name="sourcetreeviewitem">The sourcetreeviewitem.</param>
        /// <param name="sourceparenttreeviewitem">The sourceparenttreeviewitem.</param>
        /// <param name="sourcetreeview">The sourcetreeview.</param>
        private void MouseDropAfter(TreeViewItemAdv sourcetreeviewitem, TreeViewItemAdv sourceparenttreeviewitem, TreeViewAdv sourcetreeview)
        {
            if (sourcetreeviewitem != target_Selected_TVItem)
            {
                if (sourceparenttreeviewitem != null)
                {
                    if (sourcetreeviewitem is TreeViewItemAdv && target_Selected_TVItem is TreeViewItemAdv)
                    {
                        if (((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None) != true)
                        {
                            flag1 = false;
                            bool t = CheckForTargetUnderSource(sourcetreeviewitem.Items, target_Selected_TVItem);
                            if (CheckForTargetUnderSource(sourcetreeviewitem.Items, target_Selected_TVItem))
                            {
                                return;
                            }

                            sourceparenttreeviewitem.Items.Remove(sourcetreeviewitem);
                            if (sourceparenttreeviewitem.Items.Count > 0)
                            {
                                sourceparenttreeviewitem.HasItems = true;
                            }
                        }

                        if (((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None) == true)
                        {
                            TreeViewItemAdv item1 = new TreeViewItemAdv();
                            item1 = TreeViewItemClone(sourcetreeviewitem);
                            if (item1 == null)
                            {
                                return;
                            }

                            if (target_Selected_TVItem_Parent == null)
                            {
                                HandleTreeViewDropAfter(item1);
                                return;
                            }

                            int targetindex = target_Selected_TVItem_Parent.Items.IndexOf(target_Selected_TVItem);

                            InsertAfter(targetindex, item1);
                        }
                        else
                        {
                            if (target_Selected_TVItem_Parent == null)
                            {
                                HandleTreeViewDropAfter(sourcetreeviewitem);
                                return;
                            }

                            int targetindex = target_Selected_TVItem_Parent.Items.IndexOf(target_Selected_TVItem);
                            InsertAfter(targetindex, sourcetreeviewitem);
                        }

                        target_Selected_TVItem_Parent.expander.IsChecked = true;
                        sourceparenttreeviewitem.expander.IsChecked = true;
                    }
                }
                else
                {
                    if (sourcetreeviewitem != target_Selected_TVItem_Parent)
                    {
                        if (((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None) != true)
                        {
                            flag1 = false;
                            bool t = CheckForTargetUnderSource(sourcetreeviewitem.Items, target_Selected_TVItem);
                            if (CheckForTargetUnderSource(sourcetreeviewitem.Items, target_Selected_TVItem))
                            {
                                return;
                            }

                            sourcetreeview.Items.Remove(sourcetreeviewitem);
                        }

                        if (((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None) == true)
                        {
                            TreeViewItemAdv item1 = new TreeViewItemAdv();
                            item1 = TreeViewItemClone(sourcetreeviewitem);
                            if (item1 == null)
                            {
                                return;
                            }

                            if (target_Selected_TVItem_Parent == null)
                            {
                                HandleTreeViewDropAfter(item1);
                                return;
                            }

                            int targetindex = target_Selected_TVItem_Parent.Items.IndexOf(target_Selected_TVItem);
                            InsertAfter(targetindex, item1);
                        }
                        else
                        {
                            if (target_Selected_TVItem_Parent == null)
                            {
                                HandleTreeViewDropAfter(sourcetreeviewitem);
                                return;
                            }

                            int targetindex = target_Selected_TVItem_Parent.Items.IndexOf(target_Selected_TVItem);
                            InsertAfter(targetindex, sourcetreeviewitem);
                        }

                        target_Selected_TVItem.IsSelected = true;
                    }
                }
            }
        }

        private bool flag1 = false;
        /// <summary>
        /// Checks for target under source.
        /// </summary>
        /// <param name="tncoll">The tncoll.</param>
        /// <param name="strNode">The STR node.</param>
        /// <returns></returns>
        private bool CheckForTargetUnderSource(ItemCollection tncoll, TreeViewItemAdv strNode)
        {
            if (tncoll == null || strNode == null)
            {
                return false;
            }

            for (int i = 0; i < tncoll.Count; i++)
            {
                if (ItemsSource == null)
                {
                    if (tncoll[i] == strNode)
                    {
                        flag1 = true;
                        break;
                    }

                    if (((TreeViewItemAdv)tncoll[i]).Items.Count > 0)
                    {
                        CheckForTargetUnderSource(((TreeViewItemAdv)tncoll[i]).Items, strNode);
                    }
                }
                else
                {
                    TreeViewItemAdv item = GetItem(i,tncoll);
                    if (item == strNode)
                    {
                        flag1 = true;
                        break;
                    }
                    
                    if (item.Items.Count > 0)
                    {
                        CheckForTargetUnderSource(item.Items, strNode);
                    }
                }
            }

            if (flag1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Method invoked when the item is dropped.
        /// </summary>
        /// <param name="sourcetreeviewitem">represents source treeview item</param>
        /// <param name="sourceparenttreeviewitem">represents source parent treeview item</param>
        /// <param name="sourcetreeview">represents source treeview</param>
        private void MouseDropOver(TreeViewItemAdv sourcetreeviewitem, TreeViewItemAdv sourceparenttreeviewitem, TreeViewAdv sourcetreeview)
        {
            if (sourcetreeviewitem != target_Selected_TVItem)
            {
                if (sourceparenttreeviewitem != null && target_TreeView != null)
                {
                    if (sourcetreeviewitem is TreeViewItemAdv && target_Selected_TVItem is TreeViewItemAdv)
                    {
                        if (target_Selected_TVItem_Parent != null)
                        {
                            if (sourcetreeviewitem == target_Selected_TVItem_Parent)
                            {
                                return;
                            }

                            int targetindex = target_Selected_TVItem_Parent.Items.IndexOf(target_Selected_TVItem);
                        }
                        else
                        {
                            int targetindex = target_TreeView.Items.IndexOf(target_Selected_TVItem);
                        }

                        if (((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None) != true)
                        {
                            flag1 = false;
                            bool t = CheckForTargetUnderSource(sourcetreeviewitem.Items, target_Selected_TVItem);
                            if (CheckForTargetUnderSource(sourcetreeviewitem.Items, target_Selected_TVItem))
                            {
                                return;
                            }

                            sourceparenttreeviewitem.Items.Remove(sourcetreeviewitem);
                            if (sourceparenttreeviewitem.Items.Count > 0)
                            {
                                sourceparenttreeviewitem.HasItems = true;
                            }
                        }

                        if (((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None) == true)
                        {
                            TreeViewItemAdv item1 = new TreeViewItemAdv();
                            item1 = TreeViewItemClone(sourcetreeviewitem);
                            if (item1 == null)
                            {
                                return;
                            }

                            target_Selected_TVItem.Items.Add(item1);
                        }
                        else
                        {
                            target_Selected_TVItem.Items.Add(sourcetreeviewitem);
                        }

                        target_Selected_TVItem.expander.IsChecked = true;
                        sourceparenttreeviewitem.expander.IsChecked = true;
                    }
                }
                else
                {
                    if (sourcetreeviewitem != target_Selected_TVItem_Parent)
                    {
                        if (((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None) != true)
                        {
                            flag1 = false;
                            bool t = CheckForTargetUnderSource(sourcetreeviewitem.Items, target_Selected_TVItem);
                            if (CheckForTargetUnderSource(sourcetreeviewitem.Items, target_Selected_TVItem))
                            {
                                return;
                            }

                            sourcetreeview.Items.Remove(sourcetreeviewitem);
                        }

                        if (((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None) == true)
                        {
                            TreeViewItemAdv item1 = new TreeViewItemAdv();
                            item1 = TreeViewItemClone(sourcetreeviewitem);
                            if (item1 == null)
                            {
                                return;
                            }

                            target_Selected_TVItem.Items.Add(item1);
                        }
                        else
                        {
                            target_Selected_TVItem.Items.Add(sourcetreeviewitem);
                        }
                        if (sourceparenttreeviewitem != null)
                        {
                            if (sourceparenttreeviewitem.Items.Count > 0)
                                sourceparenttreeviewitem.HasItems = true;
                        }
                        target_Selected_TVItem.IsSelected = true;
                    }
                }
            }
        }

        ///// <summary>
        ///// Populatings the collections.
        ///// </summary>
        //private void PopulatingCollections()
        //{
        //    if (targetparenttreeviewitem != null)
        //    {
        //        for (int i = 0; i < targetparenttreeviewitem.Items.Count; i++)
        //        {
        //            targetListcollection.Add((TreeViewItemAdv)targetparenttreeviewitem.Items[i]);
        //        }
        //    }
        //    else if (targettreeview != null)
        //    {
        //        for (int i = 0; i < targettreeview.Items.Count; i++)
        //        {
        //            targetListcollection.Add((TreeViewItemAdv)targettreeview.Items[i]);
        //        }
        //    }
        //}

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.LostFocus"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            this.DragLineVisibility = Visibility.Collapsed;
            if (_mouseEnter == false)
            {
                this.IsFocused = false;
            }
        }

        #region Cloning for popup
        private TreeViewItemAdv popupItemRoot;
        /// <summary>
        /// Popups the items clone.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        private TreeViewItemAdv PopupItemsClone(TreeViewItemAdv source)
        {
            if (source.Items.Count == 0)
            {
                TreeViewItemAdv item1 = null;
                item1 = new TreeViewItemAdv();
                item1.LeftImageSource = source.LeftImageSource;
                item1.RightImageSource = source.RightImageSource;
                item1.Header = source.Header;
                return item1;
            }
            else
            {
                popupItemRoot = new TreeViewItemAdv();
                popupItemRoot.LeftImageSource = source.LeftImageSource;
                popupItemRoot.RightImageSource = source.RightImageSource;
                popupItemRoot.Header = source.Header;
                CloningPopupTreeViewItems(source.Items, popupItemRoot);
                return popupItemRoot;
            }
        }

        /// <summary>
        /// Clonings the popup tree view items.
        /// </summary>
        /// <param name="tncoll">The tncoll.</param>
        /// <param name="container">The container.</param>
        private void CloningPopupTreeViewItems(ItemCollection tncoll, TreeViewItemAdv container)
        {
            if (tncoll == null && container == null)
            {
                return;
            }

            foreach (TreeViewItemAdv tnode in tncoll)
            {
                TreeViewItemAdv temp = new TreeViewItemAdv();
                temp.Header = tnode.Header;
                temp.LeftImageSource = tnode.LeftImageSource;
                temp.RightImageSource = tnode.RightImageSource;
                container.Items.Add(temp);
                if (container.HasItems)
                {
                    container.expander.IsChecked = true;
                }

                CloningPopupTreeViewItems(tnode.Items, temp);
            }
        }
        #endregion

        /// <summary>
        /// Popups the template.
        /// </summary>
        /// <param name="rect">The rect.</param>
        void PopupTemplate(ref Rectangle rect)
        {
            
        }

        /// <summary>
        /// Popus up creation.
        /// </summary>
        /// <param name="sourcetreeviewitem">The sourcetreeviewitem.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void PopuUpCreation(List<TreeViewItemAdv> sourcetreeviewitem, MouseEventArgs e)
        {
            if (popup != null && sourcetreeviewitem != null)
            {
                StackPanel stackpanel = new StackPanel();
                for (int i = 0; i < sourcetreeviewitem.Count; i++)
                {
                    TreeViewItemAdv temp = new TreeViewItemAdv();
                    temp.LeftImageSource = sourcetreeviewitem[i].LeftImageSource;
                    temp.LeftImageHeight = sourcetreeviewitem[i].LeftImageHeight;
                    temp.LeftImageWidth = sourcetreeviewitem[i].LeftImageWidth;
                    temp.LineStrokeArray = sourcetreeviewitem[i].LineStrokeArray;
                    temp.VerticalRootLine1Visibility = Visibility.Collapsed;
                    temp.VerticalRootLine2Visibility = Visibility.Collapsed;
                    temp.VerticalRootLine3Visibility = Visibility.Collapsed;
                    temp.HorizontalLineVisibility = Visibility.Collapsed;
                    temp.RightImageSource = sourcetreeviewitem[i].RightImageSource;
                    temp.RightImageHeight = sourcetreeviewitem[i].RightImageHeight;
                    temp.RightImageWidth = sourcetreeviewitem[i].RightImageWidth;
                    temp.ImageMargin = sourcetreeviewitem[i].ImageMargin;
                    temp.TextHorizontalAlignment = sourcetreeviewitem[i].TextHorizontalAlignment;
                    temp.TextMargin = sourcetreeviewitem[i].TextMargin;
                    temp.TextVerticalAlignment = sourcetreeviewitem[i].TextVerticalAlignment;
                    temp.ExpanderTemplate = sourcetreeviewitem[i].ExpanderTemplate;
                    if (this.ItemsSource == null)
                    {
                        temp.Header = sourcetreeviewitem[i].Header;
                    }
                    else
                    {
                        object item = sourcetreeviewitem[i].Header;
                        Style baseStyle = sourcetreeviewitem[i].Style;

                        bool setContent = true;

                        if (setContent)
                        {
                            HeaderedItemsControl child = temp as HeaderedItemsControl;
                            Style parentItemContainerStyle = ItemContainerStyle;
                            if (this.ItemContainerStyle != null)
                            {
                                //    if (child != item)
                                //    {


                                //        DataTemplate parentItemTemplate = ItemContGenerator.parent.ItemTemplate;
                                //        if (parentItemTemplate != null)
                                //        {
                                //            child.SetValue(HeaderedItemsControl.ItemTemplateProperty, parentItemTemplate);
                                //        }


                                //        if (parentItemContainerStyle != null)
                                //        {
                                //            child.SetValue(HeaderedItemsControl.ItemContainerStyleProperty, parentItemContainerStyle);
                                //        }


                                //        child.Header = item;



                                //        if (parentItemTemplate != null)
                                //        {
                                //            child.SetValue(HeaderedItemsControl.HeaderTemplateProperty, parentItemTemplate);
                                //        }


                                //        if (parentItemContainerStyle != null && child.Style == null)
                                //        {
                                //            child.SetValue(HeaderedItemsControl.StyleProperty, parentItemContainerStyle);
                                //        }
                                //    }
                                temp.Header = item;
                                temp.ItemContainerStyle = this.ItemContainerStyle;
                            }
                            else
                            {
                                this.Style = baseStyle;
                            }
                            DataTemplate ownTemplate = sourcetreeviewitem[i].HeaderTemplate;
                            temp.HeaderTemplate = ownTemplate;
                            temp.Header = item;

                        }

                    }

                sourcetreeviewitem[i].IsMultiSelect = false;
                sourcetreeviewitem[i].DragLineVisibility = Visibility.Collapsed;
                temp.DragLineVisibility = Visibility.Collapsed;
                Rectangle rect = new Rectangle();
                PopupTemplate(ref rect);
                Grid gd = new Grid();
                gd.Children.Add(rect);
                gd.Children.Add(temp);
                //sourcetreeviewitem[i].IsSelected = false;
                stackpanel.Children.Add(gd);
            }

            popup.Child = stackpanel;
            pos = e.GetPosition(this);
            popup.IsOpen = true;
            this.lastDragPosition = e.GetPosition(popup);
            this.dragging = true;
         
            this.Cursor = Cursors.Hand;
        }
    }


        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.MouseEnter"/> event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            scrollFlag = true;
            _mouseEnter = true;
        }

        /// <summary>
        /// Method that Handles when Mouse leaves the TreeViewAdv control
        /// </summary>
        /// <param name="e">Contains information about the cursor information</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            scrollFlag = false;
            _mouseEnter = false;
            Popup popup = GetPopup(e.OriginalSource);
            if (popup != null)
            {
            }
        }

        /// <summary>
        /// Gets the popup.
        /// </summary>
        /// <param name="elem">The elem.</param>
        /// <returns></returns>
        private static Popup GetPopup(object elem)
        {
            if (elem is UIElement)
            {
                UIElement parent = (UIElement)VisualTreeHelper.GetParent((UIElement)elem);
                if (parent != null)
                {
                    if (parent is Popup)
                    {
                        return (Popup)parent;
                    }
                    else
                    {
                        return GetPopup(parent);
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// Refreshes the root lines.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="treeview">The treeview.</param>
        internal void RefreshRootLines(List<object> collection, TreeViewAdv treeview)
        {
            if (collection.Count == 1)
            {
                object m_element = collection[0];
                TreeViewItemAdv m_treeViewItem = (TreeViewItemAdv)treeview.ItemContainerGenerator.ContainerFromItem(m_element);

                if (m_treeViewItem != null)
                {
                    m_treeViewItem.HorizontalLineVisibility = this.RootLineVisibility;
                    m_treeViewItem.VerticalRootLine1Visibility = Visibility.Collapsed;
                    m_treeViewItem.VerticalRootLine2Visibility = Visibility.Collapsed;
                    m_treeViewItem.VerticalRootLine3Visibility = Visibility.Collapsed;
                    m_treeViewItem.RootLineStroke = treeview.RootLineStroke;
                    m_treeViewItem.DragLineVisibility = Visibility.Collapsed;
                    m_treeViewItem.IsMouseOver = false;
                  
                    if (m_treeViewItem.Items.Count > 0)
                    {
                        m_treeViewItem.RefreshRootLines(m_treeViewItem.Items.ToList<object>());
                    }
                }
            }
            else
            {
                for (int i = 0; i < collection.Count; i++)
                {
                    object m_element = collection[i];
                    TreeViewItemAdv m_treeViewItem = (TreeViewItemAdv)treeview.ItemContainerGenerator.ContainerFromItem(m_element);
                    if (m_treeViewItem == null)
                    {
                        m_layOutUpdated = false;
                    }
                    else
                    {
                        m_treeViewItem.HorizontalLineVisibility = this.RootLineVisibility;
                        m_treeViewItem.RootLineStroke = treeview.RootLineStroke;
                        m_treeViewItem.VerticalRootLine1Visibility = this.RootLineVisibility;
                        m_treeViewItem.VerticalRootLine2Visibility = this.RootLineVisibility;
                        m_treeViewItem.VerticalRootLine3Visibility = this.RootLineVisibility;
                        m_treeViewItem.DragLineVisibility = Visibility.Collapsed;
                        m_treeViewItem.IsMouseOver = false;

                        if (i == 0)
                        {
                            if (!m_treeViewItem.HasItems)
                            {
                                //m_treeViewItem.VerticalRootLine1Visibility = Visibility.Collapsed;
                                m_treeViewItem.VerticalRootLine1Visibility = Visibility.Collapsed;
                            }
                        }

                        //string temp = m_treeViewItem.Header.ToString();

                        if (i == (collection.Count - 1))
                        {
                            m_treeViewItem.VerticalRootLine3Visibility = Visibility.Collapsed;
                            m_treeViewItem.VerticalRootLine2Visibility = Visibility.Collapsed;
                            //if (m_treeViewItem.HasItems)
                            //{
                            //    m_treeViewItem.VerticalRootLine1Visibility = Visibility.Collapsed;
                            //}
                        }

                        if (m_treeViewItem.Items.Count > 0)
                        {
                            m_treeViewItem.RefreshRootLines(m_treeViewItem.Items.ToList<object>());
                        }
                    }
                }
            }
            //if (collection.Count > 0)
            //{
            //    object m_element = collection[0];
            //    TreeViewItemAdv m_treeViewItem = (TreeViewItemAdv)treeview.ItemContainerGenerator.ContainerFromItem(m_element);

            //    if (m_treeViewItem != null)
            //    {
            //        if (m_treeViewItem.VerticalLine1 != null)
            //        {
            //            m_treeViewItem.Vert1LineVisibility = Visibility.Collapsed;
            //            m_treeViewItem.VerticalLine1.Margin = new Thickness(m_treeViewItem.VerticalLine1.Margin.Left, 12, m_treeViewItem.VerticalLine1.Margin.Right, m_treeViewItem.VerticalLine1.Margin.Bottom);
            //        }
            //    }
            //} 
        }

        /// <summary>
        /// Refreshes the root lines.
        /// </summary>
        /// <param name="treeview">The treeview.</param>
        internal void RefreshRootLines(TreeViewAdv treeview)
        {
            if (treeview != null)
            {
                if (treeview.ItemsSource != null)
                {
                    List<object> collection = ItemsSource.OfType<object>().ToList();

                    if (collection.Count > 0)
                    {
                        object mooElement = collection[0];
                        TreeViewItemAdv mootreeViewItem = (TreeViewItemAdv)this.ItemContainerGenerator.ContainerFromItem(mooElement);
                        if (mootreeViewItem != null)
                            RefreshRootLines(collection, treeview);
                    }
                }
                else
                {
                    List<object> collection = treeview.Items.ToList();
                    if (treeview.Items.Count == 1)
                    {
                        if (treeview.Items[0] is TreeViewItemAdv)
                        {
                            ((TreeViewItemAdv)treeview.Items[0]).HorizontalLineVisibility = treeview.RootLineVisibility;
                            ((TreeViewItemAdv)treeview.Items[0]).VerticalRootLine1Visibility = Visibility.Collapsed;
                            ((TreeViewItemAdv)treeview.Items[0]).VerticalRootLine2Visibility = Visibility.Collapsed;
                            ((TreeViewItemAdv)treeview.Items[0]).VerticalRootLine3Visibility = Visibility.Collapsed;
                            ((TreeViewItemAdv)treeview.Items[0]).RootLineStroke = treeview.RootLineStroke;
                            ((TreeViewItemAdv)treeview.Items[0]).DragLineVisibility = Visibility.Collapsed;
                            ((TreeViewItemAdv)treeview.Items[0]).IsMouseOver = false;

                            if (((TreeViewItemAdv)treeview.Items[0]).Items.Count > 0)
                            {
                                ((TreeViewItemAdv)treeview.Items[0]).RefreshRootLines(((TreeViewItemAdv)treeview.Items[0]).Items.ToList<object>());
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < treeview.Items.Count; i++)
                        {
                            if (treeview.Items[i] is TreeViewItemAdv)
                            {
                                ((TreeViewItemAdv)treeview.Items[i]).HorizontalLineVisibility = treeview.RootLineVisibility;
                                ((TreeViewItemAdv)treeview.Items[i]).VerticalRootLine1Visibility = treeview.RootLineVisibility;
                                ((TreeViewItemAdv)treeview.Items[i]).VerticalRootLine2Visibility = treeview.RootLineVisibility;
                                ((TreeViewItemAdv)treeview.Items[i]).VerticalRootLine3Visibility = treeview.RootLineVisibility;
                                ((TreeViewItemAdv)treeview.Items[i]).RootLineStroke = treeview.RootLineStroke;

                                ((TreeViewItemAdv)treeview.Items[i]).RootLineStroke = treeview.RootLineStroke;
                                ((TreeViewItemAdv)treeview.Items[i]).DragLineVisibility = Visibility.Collapsed;
                                ((TreeViewItemAdv)treeview.Items[i]).IsMouseOver = false;

                                if (i == 0)
                                {
                                    //if (!((TreeViewItemAdv)treeview.Items[i]).HasItems)
                                    //{
                                    ((TreeViewItemAdv)treeview.Items[i]).VerticalRootLine1Visibility = Visibility.Collapsed;
                                    //}
                                }

                                if (i == (treeview.Items.Count - 1))
                                {
                                    //((TreeViewItemAdv)ItemContGenerator.childfromindex(i)).VerticalRootLine3Visibility = Visibility.Collapsed;
                                    ((TreeViewItemAdv)treeview.Items[i]).VerticalRootLine3Visibility = Visibility.Collapsed;
                                    ((TreeViewItemAdv)treeview.Items[i]).VerticalRootLine2Visibility = Visibility.Collapsed;
                                }

                                if (((TreeViewItemAdv)treeview.Items[i]).Items.Count > 0)
                                {
                                    ((TreeViewItemAdv)treeview.Items[i]).RefreshRootLines(((TreeViewItemAdv)treeview.Items[i]).Items.ToList<object>());
                                }
                            }
                        }
                    }
                    //if (this.Items.Count > 0)
                    //{
                    //    TreeViewItemAdv treeItem = this.Items[0] as TreeViewItemAdv;
                    //    if (treeItem != null)
                    //    {
                    //        if (treeItem.VerticalLine1 != null)
                    //        {
                    //            treeItem.VerticalLine1.Margin = new Thickness(treeItem.VerticalLine1.Margin.Left, 12, treeItem.VerticalLine1.Margin.Right, treeItem.VerticalLine1.Margin.Bottom);
                    //        }
                    //    }
                    //}
                }
            }
       }

        internal void RefreshToogleButton()
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                if ((this.ItemContainerGenerator.ContainerFromItem(this.Items[i])) != null)
                {
                    if (ShouldAlwaysVisibleExpander)
                    (this.ItemContainerGenerator.ContainerFromItem(this.Items[i]) as TreeViewItemAdv).ExpanderVisibility = this.ExpanderVisibility;//Visibility.Collapsed;
                    if (!ShouldAlwaysVisibleExpander && ExpanderVisibility==Visibility.Visible )
                        (this.ItemContainerGenerator.ContainerFromItem(this.Items[i]) as TreeViewItemAdv).ExpanderVisibility = ItemExpanderVisibility;
                }
                   
            }
        }

        /// <summary>
        /// Refreshes the root lines with binding.
        /// </summary>
        /// <param name="treeview">The treeview.</param>
        internal void RefreshRootLinesWithBinding(TreeViewAdv treeview)
        {
            if (this.Items.Count == 1)
            {
                if (((TreeViewItemAdv)ItemContGenerator.childfromindex(0)) != null)
                {
                    //TreeViewItemAdv itm = (TreeViewItemAdv)ItemContGenerator.childfromindex(0);
                    ((TreeViewItemAdv)ItemContGenerator.childfromindex(0)).HorizontalLineVisibility = this.RootLineVisibility;
                    ((TreeViewItemAdv)ItemContGenerator.childfromindex(0)).VerticalRootLine1Visibility = Visibility.Collapsed;
                    ((TreeViewItemAdv)ItemContGenerator.childfromindex(0)).VerticalRootLine2Visibility = Visibility.Collapsed;
                    ((TreeViewItemAdv)ItemContGenerator.childfromindex(0)).VerticalRootLine3Visibility = Visibility.Collapsed;
                    ((TreeViewItemAdv)ItemContGenerator.childfromindex(0)).RootLineStroke = this.RootLineStroke;
                    ((TreeViewItemAdv)ItemContGenerator.childfromindex(0)).DragLineVisibility = Visibility.Collapsed;
                    ((TreeViewItemAdv)ItemContGenerator.childfromindex(0)).IsMouseOver = false;             
                }
            }
            else
            {
                for (int i = 0; i < this.ItemContGenerator.ChildrenToItems.Count; i++)
                {
                    if (((TreeViewItemAdv)ItemContGenerator.childfromindex(i)) != null)
                    {
                        ((TreeViewItemAdv)ItemContGenerator.childfromindex(i)).HorizontalLineVisibility = this.RootLineVisibility;
                        if (ItemContGenerator.childfromindex(i) != null)
                        {
                            ((TreeViewItemAdv)ItemContGenerator.childfromindex(i)).VerticalRootLine1Visibility = this.RootLineVisibility;
                            ((TreeViewItemAdv)ItemContGenerator.childfromindex(i)).VerticalRootLine2Visibility = this.RootLineVisibility;
                            ((TreeViewItemAdv)ItemContGenerator.childfromindex(i)).VerticalRootLine3Visibility = this.RootLineVisibility;
                            ((TreeViewItemAdv)ItemContGenerator.childfromindex(i)).RootLineStroke = this.RootLineStroke;
                        }
                        if (this.RootLineVisibility == Visibility.Collapsed)
                        {
                            ((TreeViewItemAdv)ItemContGenerator.childfromindex(i)).IsMouseOver = false;
                            ((TreeViewItemAdv)ItemContGenerator.childfromindex(i)).DragLineVisibility = Visibility.Collapsed;
                        }

                        if (i == 0)
                        {
                            //if (!((TreeViewItemAdv)ItemContGenerator.childfromindex(i)).HasItems)
                            //{
                            ((TreeViewItemAdv)ItemContGenerator.childfromindex(i)).VerticalRootLine1Visibility = Visibility.Collapsed;
                            ((TreeViewItemAdv)ItemContGenerator.childfromindex(i)).RootLineMargin = new Thickness(0);
                            //}
                        }

                        if (i == (this.ItemContGenerator.ChildrenToItems.Count - 1))
                        {
                            ((TreeViewItemAdv)ItemContGenerator.childfromindex(i)).VerticalRootLine2Visibility = Visibility.Collapsed;
                            ((TreeViewItemAdv)ItemContGenerator.childfromindex(i)).VerticalRootLine3Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method  Prepares the container for the item
        /// </summary>
        /// <param name="element">The element is a dependency object is used to prepare container for item override</param>
        /// <param name="item">The item is a object is used to prepare container</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is TreeViewItemAdv)
            {
                ((TreeViewItemAdv)element).HeaderMargin = new Thickness(0);
            }
            base.PrepareContainerForItemOverride(element, item);
            TreeViewItemAdv tvitem = element as TreeViewItemAdv;
            tvitem.ExpanderTemplate = this.ExpanderTemplate;
            tvitem.ParentTreeview = this;
            this.Nodes.Add(tvitem); 
            tvitem.LineStrokeArray = this.LineStrokeArray;
            int cnt = this.Items.Count;
            tvitem.ParentNode = null;

            ItemContGenerator.ApplyPropertiesTochild(element, item, ItemContainerStyle);
            base.PrepareContainerForItemOverride(element, item);

            if (this.ItemsSource == null)
            {
                if (this.Items.Count > 0)
                {
                    object temp1 = this.Items[0];
                    if (temp1 is TreeViewItemAdv && this.RootLineVisibility == System.Windows.Visibility.Visible)
                    {
                        RefreshRootLines(this);
                    }
                }
            }
            else if(this.RootLineVisibility == System.Windows.Visibility.Visible)
            {
                RefreshRootLinesWithBinding(this);
                this.RefreshRootLines(this);
            }

            if (tvitem.ItemTemplate == null)
            {
                tvitem.ItemTemplate = this.ItemTemplate;
            }

            if (this.lastContainerCheck != null)
            {
                return;
            }

            DataTemplate template = this.ItemTemplate;
            bool setContent = true;
            if (tvitem != item)
            {
                if (null != template)
                {
                    tvitem.HeaderTemplate = template;
                }
                else if (!string.IsNullOrEmpty(this.DisplayMemberPath))
                {
                    //this.DataContext = this.ItemsSource;

                    // Create a binding for displaying the DisplayMemberPath (which always renders as a string)

                    //Binding binding = new Binding(this.DisplayMemberPath);
                    //binding.Converter = new MemberValueConverter();
                    //binding.Source = this.ItemsSource;
                    //binding.Path = new PropertyPath(this.DisplayMemberPath.ToString());
                    //BindingOperations.SetBinding(tvitem, TreeViewItemAdv.HeaderProperty, binding);
                    //setContent = false;


                    //Binding binding = new Binding(DisplayMemberPath);
                    //binding.Converter = new MemberValueConverter();
                    //tvitem.SetBinding(ContentPresenter.ContentProperty, binding);
                    //setContent = false;

                    Binding binding = new Binding();
                    binding.Source = this;

                    binding.Path = new PropertyPath(this.DisplayMemberPath, new object[0]);
                    

                    DataTemplate data = new DataTemplate();
                    TextBlock text = new TextBlock();
                    text.SetBinding(TextBlock.TextProperty, binding);
                    tvitem.Header = text;
                    setContent = false; 
                }
                
                if (setContent)
                {
                    if (tvitem.ItemTemplate != null && tvitem.HeaderTemplate == null)
                    {
                        DataTemplate ownTemplate = tvitem.ItemTemplate;
                        tvitem.HeaderTemplate = ownTemplate;
                    }
                    tvitem.Header = item;
                }
            }
        }
 
        #region Support GetElement

        /// <summary>
        /// Finds the item by point.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public TreeViewItemAdv FindItemByPoint(Point p)
        {
            TreeViewItemAdv item = null;

            if (p.Y > 0 && p.X < ActualWidth)
            {
                //double height = 0;
                //double offset = 0;
                if (elementScrollViewer != null)
                {
                    IEnumerable<UIElement> elements = VisualTreeHelper.FindElementsInHostCoordinates(p, elementScrollViewer);
                    List<TreeViewItemAdv> collection = elements.OfType<TreeViewItemAdv>().ToList();
                    if (collection.Count > 0)
                    {
                        item = collection[0] as TreeViewItemAdv;

                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return item;
        }


        /// <summary>
        /// Inserts node to the end of selected items list.
        /// </summary>
        /// <param name="node">The node TreeView.</param>
        public void AddNodeToSelectedItems(TreeViewItemAdv node)
        {
            if (node != null && !SelectedNodes.Contains(node))
            {
                if (!SelectedNodes.Contains(node))
                {
                    SelectedNodes.Add(node);
                    this.SelectedItems.Add(node.DataContext ?? node);
                }

                if (!node.IsSelected)
                {
                    node.IsSelected = true;
                }
                 
                object obj = node.ParentItemsControl.ItemContainerGenerator.ItemFromContainer(node);

                if (obj != null && obj != DependencyProperty.UnsetValue && obj is TreeViewItemAdv)
                {
                    if (!SelectedNodes.Contains(obj as TreeViewItemAdv))
                    {
                        SelectedNodes.Add(obj as TreeViewItemAdv);
                        this.SelectedItems.Add(node.DataContext ?? node);
                    }
                }
                else
                {
                    if (!SelectedNodes.Contains(node))
                    {
                        SelectedNodes.Add(node);
                        this.SelectedItems.Add(node.DataContext ?? node);
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
            if (node != null && SelectedNodes.Contains(node))
            {
                node.IsSelected = false;
                node.IsMultiSelect = false; 
                object obj = node.ParentItemsControl.ItemContainerGenerator.ItemFromContainer(node);

                SelectedNodes.Remove(node);
                SelectedItems.Remove(node.DataContext ?? node);

                if (obj != null && obj != DependencyProperty.UnsetValue && obj is TreeViewItemAdv)
                {
                    SelectedNodes.Remove(obj as TreeViewItemAdv);
                    SelectedItems.Remove((obj as TreeViewItemAdv).DataContext ?? (obj as TreeViewItemAdv));
                }
                else
                {
                    SelectedNodes.Remove(node as TreeViewItemAdv);
                    SelectedItems.Remove(node.DataContext ?? node);
                    //(node as TreeViewItemAdv).UpdateVisualStateForSelectedItem(true);
                }
            }
        }


        /// <summary>
        /// Gets the item by index.
        /// </summary>
        /// <param name="index">Index of the item.</param>
        /// <returns>Tree View Item Adv</returns>
        public TreeViewItemAdv GetItem(int index)
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
        /// Gets the item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="tnCollection">The tn collection.</param>
        /// <returns></returns>
        public TreeViewItemAdv GetItem(int index,ItemCollection tnCollection)
        {
            TreeViewItemAdv item = null;

            if (index > -1 && tnCollection.Count > 0)
            {
                item = tnCollection[index] as TreeViewItemAdv;

                if (item == null)
                {
                    item = item.ItemContainerGenerator.ContainerFromIndex(index) as TreeViewItemAdv;
                }
            }

            return item;
        }

        /// <summary>
        /// Gets the index by item.
        /// </summary>
        /// <param name="obj">Index of the item.</param>
        /// <returns>int type for index</returns>
        public int GetIndex(object obj)
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
        public TreeViewItemAdv GetFirstVisibleItem()
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
        public TreeViewItemAdv GetLastVisibleItem()
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TreeViewItemAdv GetLastItem()
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
                    if (item.Items.Count > 0)
                    {
                        if (item.IsExpanded == true)
                            itemFound = item.GetLastSubItem();
                        else
                            itemFound = item;
                    }
                    else
                        itemFound = item;
                    break;
                }
            }
            return itemFound; 
        }

      
        #endregion


        /// <summary>
        /// Gets the tree view.
        /// </summary>
        /// <param name="elem">The elem.</param>
        /// <returns></returns>
        private static TreeViewAdv GetTreeView(object elem)
        {
            if (elem is UIElement)
            {
                UIElement parent = (UIElement)VisualTreeHelper.GetParent((UIElement)elem);
                if (parent != null)
                {
                    if (parent is TreeViewAdv)
                    {
                        return (TreeViewAdv)parent;
                    }
                    else
                    {
                        return GetTreeView(parent);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the tree view item.
        /// </summary>
        /// <param name="elem">The elem.</param>
        /// <returns></returns>
        private static TreeViewItemAdv GetTreeViewItem(object elem)
        {
            if (elem is UIElement)
            {
                UIElement parent = (UIElement)VisualTreeHelper.GetParent((UIElement)elem);
                if (parent != null)
                {
                    if (parent is TreeViewItemAdv)
                    {
                        return (TreeViewItemAdv)parent;
                    }
                    else
                    {
                        return GetTreeViewItem(parent);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the child node.
        /// </summary>
        /// <param name="dataItem">The data item.</param>
        /// <returns></returns>
        private TreeViewItemAdv FindChildNode(object dataItem)
        {
            var item = (from n in Nodes where n.DataContext == dataItem select n).FirstOrDefault();

            if (item == null)
            {
                foreach (var childNode in this.Nodes)
                {
                    item = childNode.FindChildNode(dataItem);
                    if (item != null)
                    {
                        return item;
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// CTs the page.
        /// </summary>
        /// <param name="treeViewItem">The tree view item.</param>
        /// <param name="itemsHostRect">The items host rect.</param>
        /// <param name="treeViewItemRect">The tree view item rect.</param>
        /// <returns></returns>
        private bool CTPage(TreeViewItemAdv treeViewItem, out Rect itemsHostRect, out Rect treeViewItemRect)
        {
            FrameworkElement itemsHost = this.elementScrollViewer;
            itemsHostRect = new Rect(new Point(), new Point(itemsHost.RenderSize.Width, itemsHost.RenderSize.Height));
            Control itemsHostControl = itemsHost as Control;
            if (null != itemsHostControl)
            {
                Thickness padding = itemsHostControl.Padding;
                itemsHostRect = new Rect(
                    itemsHostRect.Left + padding.Left,
                    itemsHostRect.Top + padding.Top,
                    Math.Max(0, itemsHostRect.Width - padding.Left - padding.Right),
                    Math.Max(0, itemsHostRect.Height - padding.Top - padding.Bottom));
            }

            GeneralTransform generalTransform = treeViewItem.TransformToVisual(itemsHost);
            treeViewItemRect = new Rect(generalTransform.Transform(new Point()), generalTransform.Transform(new Point(treeViewItem.RenderSize.Width, treeViewItem.RenderSize.Height)));
            return (itemsHostRect.Top <= treeViewItemRect.Top) && (treeViewItemRect.Bottom <= itemsHostRect.Bottom);
        }
        #endregion Methods

        #region ContextMenu

        /// <summary>
        /// Occurs when [context menu opening].
        /// </summary>
        public event ContextMenuEventHandler ContextMenuOpening;
        ///// <summary>
        ///// Occurs when [context menu opened].
        ///// To remove warnings
        ///// </summary>
        //public event ContextMenuEventHandler ContextMenuOpened;

        /// <summary>
        /// Gets or sets the context menu.
        /// </summary>
        /// <value>The context menu.</value>
        internal ContextMenuAdv ContextMenu
        {
            get { return (ContextMenuAdv)GetValue(ContextMenuProperty); }
            set { SetValue(ContextMenuProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ContextMenuProperty =
            DependencyProperty.Register("ContextMenu", typeof(ContextMenuAdv), typeof(TreeViewAdv), new PropertyMetadata(null, OnContextMenuPropertyChanged));

        /// <summary>
        /// Called when [context menu property changed].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnContextMenuPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((TreeViewAdv)obj).OnContextMenuPropertyChanged(args);
        }

        /// <summary>
        /// Raises the <see cref="E:ContextMenuPropertyChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnContextMenuPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.ContextMenu != null)
            {
                ContextMenuAdvService.SetContextMenuAdv(this, this.ContextMenu);
                this.ContextMenu.Opened += new RoutedEventHandler(ContextMenu_Opened);
            }
        }

        /// <summary>
        /// Handles the Opened event of the ContextMenu control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenuEventArgs args = new ContextMenuEventArgs() { Node = firstlastViewItem, Handled = false };
            if (this.ContextMenuOpening != null)
            {
                this.ContextMenuOpening(this, args);
            }
            if (args.Handled)
            {
                this.ContextMenu.IsOpen = false;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.UIElement.MouseRightButtonDown"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data.</param>
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);
            if (this.ContextMenu != null)
            {
                Point p = new Point();
                p = e.GetPosition(this);
                this.m_Height = 0.0;
                this.firstlastViewItem = null;
                this.GetItemFromHeight(this, this.Items, this, p.Y);
                if (this.firstlastViewItem != null)
                {
                    this.ClearSelectedNodes();
                    this.firstlastViewItem.Select(true);
                    this.firstlastViewItem.Focus();
                }
            }
        }
        #endregion

        #region SearchItemByPath

        /// <summary>
        /// Searches the item by header path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="pathseperator">The pathseperator.</param>
        /// <param name="IsSelected">if set to <c>true</c> [is selected].</param>
        /// <returns></returns>
        private TreeViewItemAdv SearchItemByHeaderPath(string path, char pathseperator, bool IsSelected)
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
        /// <param name="propertyname">The property Name</param>
        /// <param name="IsSelected">if set to <c>true</c> [is selected].</param>
        /// <returns></returns>
        private TreeViewItemAdv IterateObjectPath(TreeViewItemAdv tree, int elementindex, string[] hierarchylist,string propertyname,bool IsSelected)
        {
            if (tree != null)
            {
                foreach (object obj2 in (IEnumerable)tree.Items)
                {
                    PropertyInfo p = obj2.GetType().GetProperty(propertyname);
                    if(p!=null)
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
                                        return this.IterateObjectPath(treeviewitem, elementindex, hierarchylist,propertyname, IsSelected);
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
            int elementindex = 0;
            if (this != null)
            {
                foreach (object obj2 in (IEnumerable)base.Items)
                {
                    PropertyInfo p = obj2.GetType().GetProperty(propertyname);
                    if(p!=null)
                    {
                        if(p.GetValue(obj2, null)!=null)
                        {
                            if (p.GetValue(obj2, null).ToString().Equals(hierarchylist[elementindex]))
                            {
                                TreeViewItemAdv tree = (TreeViewItemAdv)base.ItemContainerGenerator.ContainerFromItem(obj2);
                                if (tree == null)
                                {
                                }
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
                                        return this.IterateObjectPath(tree, elementindex, hierarchylist,propertyname,IsSelected);
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
        /// Gets the path.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="pathseparator">The pathseparator.</param>
        /// <returns></returns>
        public string GetPath(TreeViewItemAdv node, char pathseparator)
        {
            return GetStringPath(node, pathseparator, string.Empty);
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="pathseparator">The pathseparator.</param>
        /// <param name="propertyname">The propertyname.</param>
        /// <returns></returns>
        public string GetPath(TreeViewItemAdv node, char pathseparator, string propertyname)
        {
            return GetStringPath(node, pathseparator, propertyname);
        }

        /// <summary>
        /// Gets the string path.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="pathseperator">The pathseperator.</param>
        /// <param name="propertyname">The propertyname.</param>
        /// <returns></returns>
        internal string GetStringPath(TreeViewItemAdv node, char pathseperator, string propertyname)
        {
            string TreeViewItemPath = string.Empty;
            if (node != null)
            {
                if (this.ItemsSource == null)
                {
                    TreeViewItemPath = node.Header.ToString();
                    while (node != node.ParentTreeview.root)
                    {
                        if (node.ParentNode != null)
                        {
                            node = node.ParentNode;
                            //if (node == node.ParentTreeview.root)
                            //{
                            //    TreeViewItemPath = node.Header.ToString() + pathseperator + TreeViewItemPath;
                            //}
                            if (node.ParentNode == null)
                            {
                                TreeViewItemPath = node.Header.ToString() + pathseperator + TreeViewItemPath;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    string pathstring;
                    Type t;
                    if (propertyname != string.Empty)
                    {
                        t = node.Header.GetType();
                        var prop = t.GetProperty(propertyname);
                        pathstring = prop.GetValue(node.Header, null).ToString();
                    }
                    else
                    {
                        pathstring = node.Header.ToString();
                    }
                    TreeViewItemPath = pathstring;
                    //while (node != node.ParentTreeview.root)
                    while (node.ParentNode != null)
                    {
                        if (node.ParentNode != null)
                        {
                            node = node.ParentNode;
                            //if (node == node.ParentTreeview.root)
                            if (node.ParentNode == null)
                            {
                                if (propertyname != string.Empty)
                                {
                                    t = node.Header.GetType();
                                    var prop = t.GetProperty(propertyname);
                                    pathstring = prop.GetValue(node.Header, null).ToString();
                                }
                                else
                                {
                                    pathstring = node.Header.ToString();
                                }
                                TreeViewItemPath = pathstring + pathseperator + TreeViewItemPath;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return TreeViewItemPath;
        }


        #endregion

        //protected override void OnKeyDown(KeyEventArgs e)
        //{
        //    base.OnKeyDown(e);
        //}

        //public bool AutoSearch(char c)
        //{
        //    return false;
        //}

        /// <summary>
        /// Autoes the search.
        /// </summary>
        /// <param name="StartFrom">The start from.</param>
        /// <returns></returns>
        internal bool AutoSearch(int StartFrom)
        {
            for (int i = StartFrom; i < this.Items.Count; i++)
            {
                TreeViewItemAdv treeviewitem = (TreeViewItemAdv)this.ItemContainerGenerator.ContainerFromIndex(i);
                if (treeviewitem.AutoSearchStopFlag)
                    return false;

                #region AutoSearchHeader
                if (this.ItemsSource == null)
                {
                    if (treeviewitem.Header.ToString().StartsWith(this.AutoSearchString))
                    {
                        treeviewitem.IsSelected = true;
                        treeviewitem.Focus();
                        this.ScrollIntoView(treeviewitem);
                        return true;
                    }
                    else if (treeviewitem.IsExpanded)
                    {
                        //if (!treeviewitem.AutoSearchStartNode)
                        //    return treeviewitem.AutoSearch(0, c);
                        //else
                        //{
                        //    if (treeviewitem.AutoSearch(0, c))
                        //        return true;
                        //}
                        if (treeviewitem.AutoSearch(0))
                            return true;
                    }
                }
                #endregion

                #region AutoSerachPath
                else
                {
                    if (!this.DisplayMemberPath.Equals(""))
                    {
                        Type t = this.Items[i].GetType();
                        var prop = t.GetProperty(this.DisplayMemberPath);
                        object pathstring = prop.GetValue(this.Items[i], null);

                        if (pathstring.ToString().StartsWith(this.AutoSearchString))
                        {
                            treeviewitem.IsSelected = true;
                            treeviewitem.Focus();
                            this.ScrollIntoView(treeviewitem);
                            return true;
                        }
                        else if (treeviewitem.IsExpanded)
                        {
                            if (treeviewitem.AutoSearch(0))
                                return true;
                            //if (!treeviewitem.AutoSearchStartNode)
                            //    return treeviewitem.AutoSearch(0, c);
                            //else
                            //{
                            //    if (treeviewitem.AutoSearch(0, c))
                            //        return true;
                            //}
                        }
                       
                    }
                    else
                        return false;
                }
                #endregion
            }
            return false;
        }

        /// <summary>
        /// Gets or sets the name or path of the property that is displayed for each data item.
        /// </summary>
        /// <value></value>
        /// <returns>The name or path of the property that is displayed for each the data item in the control.  The default is an empty string ("").</returns>
        public new string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public new static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(TreeViewAdv), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets a value indicating whether [auto search enabled].
        /// </summary>
        /// <value><c>true</c> if [auto search enabled]; otherwise, <c>false</c>.</value>
        public bool AutoSearchEnabled
        {
            get { return (bool)GetValue(AutoSearchEnabledProperty); }
            set { SetValue(AutoSearchEnabledProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty AutoSearchEnabledProperty =
            DependencyProperty.Register("AutoSearchEnabled", typeof(bool), typeof(TreeViewAdv), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the selection mode.
        /// </summary>
        /// <value>The selection mode.</value>
        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register("SelectionMode", typeof(SelectionMode), typeof(TreeViewAdv), new PropertyMetadata(SelectionMode.Single));

        internal string AutoSearchString = string.Empty;

        //protected override void OnKeyDown(KeyEventArgs e)
        //{
        //    if (this.SelectedItem == null && this.Items.Count > 0)
        //    {
        //    }

        //    switch (e.Key)
        //    {
        //        case Key.Up:
        //            Up();
        //            break;
        //        case Key.Down:
        //            Down();
        //            break;
        //        case Key.PageUp:
        //            PageUp();
        //            break;
        //        case Key.PageDown:
        //            PageDown();
        //            break;
        //        case Key.Right:
        //            Right();
        //            break;
        //        case Key.Left:
        //            Left();
        //            break;
        //        case Key.Home:
        //            Top();
        //            break;
        //        case Key.End:
        //            Bottom();
        //            break;
        //        default: return;
        //    }
        //}
        internal DispatcherTimer AutoSearchTimer;
    }

    /// <summary>
    /// 
    /// </summary>
    public enum SelectionMode
    {
        /// <summary>
        /// 
        /// </summary>
        Single,
        /// <summary>
        /// 
        /// </summary>
        MultiSelectAll,
        /// <summary>
        /// 
        /// </summary>
        MultiSelectSameLevel
    }
}