#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using System.ComponentModel;
using Syncfusion.Windows.Shared;
using System.Collections.Specialized;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the GroupBar UI element.
    /// </summary>
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
        Type = typeof(GroupBar), XamlResource = "/Syncfusion.Theming.Blend;component/GroupBar.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(GroupBar), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/GroupBar.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(GroupBar), XamlResource = "/Syncfusion.Theming.Office2007Black;component/GroupBar.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(GroupBar), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/GroupBar.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
       Type = typeof(GroupBar), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/GroupBar.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(GroupBar), XamlResource = "/Syncfusion.Theming.Office2010Black;component/GroupBar.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(GroupBar), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/GroupBar.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(GroupBar), XamlResource = "/Syncfusion.Theming.Default;component/GroupBar.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2003,
        Type = typeof(GroupBar), XamlResource = "/Syncfusion.Theming.Office2003;component/GroupBar.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
        Type = typeof(GroupBar), XamlResource = "/Syncfusion.Theming.Windows7;component/GroupBar.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
        Type = typeof(GroupBar), XamlResource = "/Syncfusion.Theming.VS2010;component/GroupBar.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
    Type = typeof(GroupBar), XamlResource = "/Syncfusion.Theming.Metro;component/GroupBar.xaml")]
 [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent ,
Type = typeof(GroupBar), XamlResource = "/Syncfusion.Theming.Transparent;component/GroupBar.xaml")]
    public class GroupBar : ItemsControl
    {
        #region Constants
        private const string StackModeHeaderGridElement = "HeaderItemGrid";
        private const string NonStackModeGridElement = "ScrollViewer";
        private const string NonStackGridElement = "NonStackGrid";
        private const string StackGridElement = "StackGrid";
        private const string MainHostElement = "MainHost";
        private const string ScrollViewerElement = "ScrollViewer";
        private const string DraggingPopup = "DraggingPopup";       
        #endregion

        #region Private members
        
        /// <summary>
        /// List of indices of the hidden items.
        /// </summary>
        /// 
        private readonly List<int> hiddenIndices = new List<int>();

        internal FrameworkElement stackModeHeaderGridElement;
        internal FrameworkElement nonStackModeGridElement;
        internal GroupBarItem stackModeItem;
        internal NonStackGrid nonStackGridElement;
        internal StackGrid stackGridElement;
        internal FrameworkElement stackModeItemsGridElement;
        internal Popup draggingPopup;
        internal GroupBarItem dragItem;
        internal GroupBarItem overItem;
        internal StackGrid stackGrid;
        internal ScrollViewer scrollViewerElement;
        internal bool isNavPaneMode = false;
        internal double itemGridHeight, itemGridWidth;
        #endregion

        #region event

        /// <summary>
        /// 
        /// </summary>
        public event GroupBarSelectionChangedEventHandler SelectionChanged;

        #endregion

        #region Dependency properties


        /// <summary>
        /// 
        /// </summary>
        public bool FitContent
        {
            get { return (bool)GetValue(FitContentProperty); }
            set { SetValue(FitContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FitContent.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty FitContentProperty =
            DependencyProperty.Register("FitContent", typeof(bool), typeof(GroupBar), new PropertyMetadata(false));
        


        /// <summary>
        /// Gets or Sets a value indicating Gripper visibility when GroupBar's VisualMode is StackMode
        /// </summary>
        public bool ShowGripper
        {
            get { return (bool)GetValue(ShowGripperProperty); }
            set { SetValue(ShowGripperProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowGripper.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ShowGripperProperty =
            DependencyProperty.Register("ShowGripper", typeof(bool), typeof(GroupBar), new PropertyMetadata(true, OnShowGripperChanged));

        /// <summary>
        /// 
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(GroupBar), new PropertyMetadata(null));



        /// <summary>
        /// Identifies <see cref="VisualMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VisualModeProperty =
            DependencyProperty.Register("VisualMode", typeof(VisualMode), typeof(GroupBar), new PropertyMetadata(VisualMode.SingleExpansion, new PropertyChangedCallback(IsVisualModeChanged)));

        /// <summary>
        /// Identifies <see cref="Orientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(GroupBar), new PropertyMetadata(Orientation.Vertical, new PropertyChangedCallback(IsOrientationChanged)));

        /// <summary>
        /// Identifies <see cref="EnableItemsDraggingProperty">EnableItemsDragging</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty EnableItemsDraggingProperty =
            DependencyProperty.Register("EnableItemsDragging", typeof(bool), typeof(GroupBar), new PropertyMetadata(true, null));

        /// <summary>
        /// Identifies <see cref="AllowCollapseProperty">AllowCollapse</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty AllowCollapseProperty =
            DependencyProperty.Register("AllowCollapse", typeof(bool), typeof(GroupBar), new PropertyMetadata(new PropertyChangedCallback(AllowCollapseChanged)));

        /// <summary>
        /// Identifies <see cref="ShowNavigationPaneTextProperty">ShowNavigationPaneText</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty ShowNavigationPaneTextProperty =
            DependencyProperty.Register("ShowNavigationPaneText", typeof(bool), typeof(GroupBar), new PropertyMetadata(true, new PropertyChangedCallback(ShowNavigationPaneTextChanged)));


        ///<summary>
        ///</summary>
        // Identifies NavigationPaneText DependencyProperty
        public static readonly DependencyProperty NavigationPaneTextProperty =
            DependencyProperty.Register("NavigationPaneText", typeof(string), typeof(GroupBar), new PropertyMetadata("Navigation Panel"));

        /// <summary>
        /// 
        /// </summary>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(GroupBar), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedItemChanged)));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets visual mode of the control according to
        /// the <see cref="VisualMode"/> enumeration.
        /// This is a dependency property.
        /// </summary>
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
        /// Gets or sets a value indicates the Orientation.
        /// </summary>
        /// <value>The orientation.</value>
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
        /// Gets or sets the stack mode item.
        /// </summary>
        /// <value>The stack mode item.</value>
        internal GroupBarItem StackModeItem
        {
            get
            {
                return this.stackModeItem;
            }

            set
            {
                this.stackModeItem = value;
            }
        }

        /// <summary>
        /// Gets or sets the stack mode header grid.
        /// </summary>
        /// <value>The stack mode header grid.</value>
        internal FrameworkElement StackModeHeaderGrid
        {
            get
            {
                return this.stackModeHeaderGridElement;
            }

            set
            {
                this.stackModeHeaderGridElement = value;
            }
        }

        /// <summary>
        /// Gets or sets the stack mode items grid element.
        /// </summary>
        /// <value>The stack mode items grid element.</value>
        internal FrameworkElement StackModeItemsGridElement
        {
            get
            {
                return this.stackModeItemsGridElement;
            }

            set
            {
                this.stackModeItemsGridElement = value;
            }
        }

        /// <summary>
        /// Gets or sets the drag item.
        /// </summary>
        /// <value>The drag item.</value>
        internal GroupBarItem DragItem
        {
            get
            {
                return this.dragItem;
            }

            set
            {
                this.dragItem = value;
            }
        }

        /// <summary>
        /// Gets or sets the over item.
        /// </summary>
        /// <value>The over item.</value>
        internal GroupBarItem OverItem
        {
            get
            {
                return this.overItem;
            }

            set
            {
                this.overItem = value;
            }
        }

        /// <summary>
        /// Gets or sets the stack grid.
        /// </summary>
        /// <value>The stack grid.</value>
        internal StackGrid StackGrid
        {
            get
            {
                return this.stackGrid;
            }

            set
            {
                this.stackGrid = value;
            }
        }

        /// <summary>
        /// Gets the scroll viewer.
        /// </summary>
        /// <value>The scroll viewer.</value>
        internal ScrollViewer ScrollViewer
        {
            get
            {
                return this.scrollViewerElement;
            }
        }

        /// <summary>
        /// Gets the height of the item grid.
        /// </summary>
        /// <value>The height of the item grid.</value>
        internal double ItemGridHeight
        {
            get
            {
                return itemGridHeight;
            }
        }

        /// <summary>
        /// Gets the width of the item grid.
        /// </summary>
        /// <value>The width of the item grid.</value>
        internal double ItemGridWidth
        {
            get
            {
                return itemGridWidth;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether enable items dragging.
        /// </summary>
        /// <value><c>true</c> if [enable items dragging]; otherwise, <c>false</c>.</value>
        public bool EnableItemsDragging
        {
            get
            {
                return (bool)GetValue(EnableItemsDraggingProperty);
            }

            set
            {
                SetValue(EnableItemsDraggingProperty, value);
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
        /// Gets or sets a value indicating whether [show navigation pane text].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show navigation pane text]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowNavigationPaneText
        {
            get
            {
                return (bool)GetValue(ShowNavigationPaneTextProperty);
            }

            set
            {
                SetValue(ShowNavigationPaneTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is nav pane mode.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is nav pane mode; otherwise, <c>false</c>.
        /// </value>
        internal bool IsNavPaneMode
        {
            get
            {
                return isNavPaneMode;
            }

            set
            {
                this.isNavPaneMode = value;
            }
        }

        /// <summary>
        /// Gets or sets a value to be displayed in the Navigation pane when in collapsed state.
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
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes new instance of the <see cref="GroupBar"/> class.
        /// </summary>
        public GroupBar()
        {
            this.DefaultStyleKey = typeof(GroupBar);           
        }

        
        /// <summary>
        /// Static constructor.
        /// </summary>
        static GroupBar()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                LoadDependentAssemblies load = new LoadDependentAssemblies();
                load = null;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.LoadTemplateChildren();
        }

        /// <summary>
        /// Loads the template children.
        /// </summary>
        internal void LoadTemplateChildren()
        {
            if (this.nonStackGridElement is NonStackGrid)
            {
                (this.nonStackGridElement as NonStackGrid).ItemsSource = null;
            }

            if (this.stackGridElement is StackGrid)
            {
                (this.stackGridElement as StackGrid).ItemsSource = null;
            }

            this.stackModeHeaderGridElement = GetTemplateChild(StackModeHeaderGridElement) as FrameworkElement;
            this.nonStackModeGridElement = GetTemplateChild(NonStackModeGridElement) as FrameworkElement;
            this.scrollViewerElement = GetTemplateChild(ScrollViewerElement) as ScrollViewer;
            this.nonStackGridElement = GetTemplateChild(NonStackGridElement) as NonStackGrid;

            this.stackGridElement = GetTemplateChild(StackGridElement) as StackGrid;
            this.draggingPopup = GetTemplateChild(DraggingPopup) as Popup;
            
          

            FrameworkElement stackPanelHost = GetTemplateChild(MainHostElement) as FrameworkElement;

            if (this.VisualMode != VisualMode.StackMode)
            {
                if (this.nonStackGridElement != null && this.stackGridElement != null)
                {
                    this.nonStackGridElement.Visibility = Visibility.Visible;
                    this.stackGridElement.Visibility = Visibility.Collapsed;
                    this.nonStackGridElement.groupBar = this;
                    (this.nonStackGridElement as NonStackGrid).ItemsSource = this.Items;
                    nonStackGridElement.ItemTemplate = ItemTemplate;

                }
            }
            else
            {
                if (this.stackGridElement != null && this.nonStackGridElement != null)
                {
                    this.StackModeHeaderGrid = this.stackGridElement;
                    this.StackGrid = this.stackGridElement as StackGrid;
                    this.stackGrid.groupBar = this;
                    this.stackGridElement.Visibility = Visibility.Visible;
                    this.nonStackGridElement.Visibility = Visibility.Collapsed;
                    (this.stackGridElement as StackGrid).Orientation = this.Orientation;
                    (this.stackGridElement as StackGrid).ItemsSource = this.Items;
                    this.stackGridElement.ItemTemplate = ItemTemplate;
                    //if((this.stackGridElement as StackGrid).Items.Count>0)
                    //{
                    //(this.stackGridElement.ItemContainerGenerator.ContainerFromIndex(0) as GroupBarItem).IsExpanded = true;
                    //(this.stackGridElement.ItemContainerGenerator.ContainerFromIndex(0) as GroupBarItem).IsPressed = true;
                    //(this.stackGridElement.ItemContainerGenerator.ContainerFromIndex(0) as GroupBarItem).IsSelected = true;
                    //}
                    (this.stackGridElement as StackGrid).StackModeHeaderGridElementMinHeight = this.Height / 1.5;

                    if (this.AllowCollapse)
                    {
                        (this.stackGridElement as StackGrid).AllowCollapseVisibility = Visibility.Visible;
                    }
                    else
                    {
                        (this.stackGridElement as StackGrid).AllowCollapseVisibility = Visibility.Collapsed;
                    }

                    if (this.ShowNavigationPaneText)
                    {
                        (this.stackGridElement as StackGrid).ShowNavigationPaneTextVisibility = Visibility.Visible;
                    }
                    else
                    {
                        (this.stackGridElement as StackGrid).ShowNavigationPaneTextVisibility = Visibility.Collapsed;
                    }

                    (this.stackGridElement as StackGrid).LocalParent = this;
                }
            }
        }

        internal bool orientationFlag = false;
        internal double locheight;
        internal double locwidth;

        /// <summary>
        /// Rotates the group bar.
        /// </summary>
        internal void RotateGroupBar()
        {
            if (this.Orientation == Orientation.Horizontal)
            {
                RotateTransform rtw = new RotateTransform();
                rtw.Angle = -90;
                this.RenderTransformOrigin = new Point(0.5, 0.5);
                this.RenderTransform = rtw;
                if (this.VisualMode == VisualMode.StackMode && (StackGrid)this.stackGridElement != null)
                {
                    (this.stackGridElement as StackGrid).RotatePopup();

                    if (itemGridHeight == 0)
                    {
                        itemGridHeight = (((StackGrid)(this.stackGridElement)).ItemGrid as FrameworkElement).ActualHeight;
                        itemGridWidth = (((StackGrid)(this.stackGridElement)).ItemGrid as FrameworkElement).ActualWidth;
                    }
                }

                if (this.IsNavPaneMode)
                {
                    (this.stackGridElement as StackGrid).CollapseNavPane();

                    RotateTransform transf = new RotateTransform();
                    transf.Angle = 180;
                    (this.stackGridElement as StackGrid).NavPaneText.RenderTransformOrigin = new Point(0.5, 0.5);
                    (this.stackGridElement as StackGrid).NavPaneText.RenderTransform = transf;
                }
                if (this.VisualMode != VisualMode.StackMode)
                {
                    if (this.Height != double.NaN && this.Width != double.NaN)
                    {
                        locheight = this.Height;
                        locwidth = this.Width;
                    }
                    else if (this.ActualHeight != 0 && this.ActualWidth != 0)
                    {
                        locwidth = this.ActualWidth;
                        locheight = this.ActualHeight;
                    }

                    this.Height = locwidth;
                    this.Width = locheight;
                    orientationFlag = true;
                }

                foreach (GroupBarItem item in this.Items)
                {
                    if (item.Content != null)
                    {
                        item.RotateGroupBarItem(this);
                    }
                }
                
            }
            else
            {
                RotateTransform rtw = new RotateTransform();
                rtw.Angle = 0;
                this.RenderTransformOrigin = new Point(0.5, 0.5);
                this.RenderTransform = rtw;

                if (this.VisualMode == VisualMode.StackMode && (this.stackGridElement as StackGrid) != null)
                {
                    (this.stackGridElement as StackGrid).RotatePopup();
                }

                if (this.IsNavPaneMode)
                {
                    (this.stackGridElement as StackGrid).CollapseNavPane();

                    RotateTransform transf = new RotateTransform();
                    transf.Angle = 0;
                    (this.stackGridElement as StackGrid).NavPaneText.RenderTransformOrigin = new Point(0.5, 0.5);
                    (this.stackGridElement as StackGrid).NavPaneText.RenderTransform = transf;
                }
                if (orientationFlag && this.VisualMode != VisualMode.StackMode)
                {
                    this.Height = locheight;
                    this.Width = locwidth;
                    locwidth = 0;
                    locheight = 0;
                    orientationFlag = false;
                }
                if (this.ItemsSource == null)
                {
                    foreach (GroupBarItem item in this.Items)
                    {
                        if (item.Content != null)
                        {
                            item.RotateGroupBarItem(this);
                        }
                    }
                }
               
            }
        }

        /// <summary>
        /// Replaces the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="i">The i.</param>
        internal void ReplaceItem(GroupBarItem item, int i)
        {
            //this.Items.Remove(item);
            //this.Items.Insert(i, item);
            //this.LoadTemplateChildren();
            item.CollapseContent();

            if (this.IsNavPaneMode)
            {
                this.StackGrid.CollapseNavPane();
            }

            item.ChangeItemExpandMode(this);
        }

        /// <summary>
        /// Saves the state persisted for the current GroupBar location.
        /// </summary>
        public void SaveState()
        {
            GroupBarParams gbParamsList = new GroupBarParams(this);

            string content = this.SerializeElement(gbParamsList);
            this.SaveIsolatedState(content);
            gbParamsList = null;
        }

        /// <summary>
        /// Serialze element for storing
        /// </summary>
        /// <param name="obj">Object to serialize.</param>     
        /// <returns>
        /// Serialize string
        /// </returns>
        private string SerializeElement(object obj)
        {
            XmlSerializer xs = new XmlSerializer(typeof(GroupBarParams));

            StringBuilder sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb);
            xs.Serialize(writer, obj);
            writer.Close();

            return sb.ToString();
        }

        /// <summary>
        /// Saves data in the Isolated Storage
        /// </summary>
        /// <param name="strData">string data.</param>
        private void SaveIsolatedState(string strData)
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream("GroupBarParams.txt", FileMode.Create, isoStore))
                {
                    using (StreamWriter writer = new StreamWriter(isoStream))
                    {
                        writer.Write(strData);
                    }
                }
            }
        }

        /// <summary>
        /// Loads the state persisted.
        /// </summary>
        public void LoadState()
        {
            string savedState = this.ReadIsolatedState();
            if (savedState != string.Empty)
            {
                GroupBarParams gbParamsList = this.DeSerializeElement(savedState);

                for (int i = 0; i < gbParamsList.GroupBarItems.Count; i++)
                {
                    GroupBarItem it = null;

                    for (int j = 0; j < this.Items.Count; j++)
                    {
                        if (gbParamsList.GroupBarItems[i].ItemName == ((GroupBarItem)this.Items[j]).Name)
                        {
                            it = (GroupBarItem)this.Items[j];
                        }
                    }

                    int pos = 0;

                    foreach (GroupBarItemParams param in gbParamsList.GroupBarItems)
                    {
                        if (param.ItemName == it.Name)
                        {
                            pos = param.ItemIndex;
                        }
                    }

                    ReplaceItem(it, pos);
                }

                for (int i = 0; i < gbParamsList.GroupBarItems.Count; i++)
                {
                    if (gbParamsList.GroupBarItems[i].IsSelected || gbParamsList.GroupBarItems[i].IsExpanded)
                    {
                        (this.Items[i] as GroupBarItem).CollapseContent();

                        if (gbParamsList.GroupBarItems[i].IsExpanded)
                        {
                            (this.Items[i] as GroupBarItem).IsExpanded = false;
                            (this.Items[i] as GroupBarItem).ChangeItemExpandMode(this);
                        }

                        (this.Items[i] as GroupBarItem).IsSelected = true;
                        (this.Items[i] as GroupBarItem).UnSelectItem();
                        (this.Items[i] as GroupBarItem).UpdateVisualState();
                    }
                    else
                    {
                        (this.Items[i] as GroupBarItem).CollapseContent();
                        (this.Items[i] as GroupBarItem).UnSelectItem();
                    }
                }
            }
        }

        /// <summary>
        /// De-serialize string reader for showign the existing data
        /// </summary>
        /// <param name="strSerilizeObject">object to serialize.</param>
        /// <returns>
        /// GroupBar params
        /// </returns>
        private GroupBarParams DeSerializeElement(string strSerilizeObject)
        {
            XmlSerializer xs = new XmlSerializer(typeof(GroupBarParams));

            StringReader sr = new StringReader(strSerilizeObject);

            GroupBarParams gbParamsList;
            gbParamsList = (GroupBarParams)xs.Deserialize(sr);

            return gbParamsList;
        }

        /// <summary>
        /// Reads data from isolated Storage
        /// </summary>
        /// <returns>
        /// Deserialize string
        /// </returns>
        private string ReadIsolatedState()
        {
            string content = string.Empty;

            using (IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isoStream =
                    new IsolatedStorageFileStream("GroupBarParams.txt", FileMode.OpenOrCreate, isoFile))
                {
                    using (StreamReader sr = new StreamReader(isoStream))
                    {
                        content = sr.ReadToEnd();
                    }
                }
            }

            return content;
        }

        #endregion

        /// <summary>
        /// Determines whether [is orientation changed] [the specified obj].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void IsOrientationChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            GroupBar groupbar = (GroupBar)obj;
            StackGrid stackGrid = null;

            if (groupbar.stackGridElement != null)
            {
                stackGrid = (StackGrid)groupbar.stackGridElement;
                stackGrid.Orientation = groupbar.Orientation;
                stackGrid.SetCursor();
            }

            groupbar.RotateGroupBar();
        }

        /// <summary>
        /// Determines whether [is visual mode changed] [the specified obj].
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void IsVisualModeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            GroupBar groupbar = (GroupBar)obj;

            if (groupbar.StackModeHeaderGrid != null)
            {
                ((groupbar.StackModeHeaderGrid as StackGrid).ItemGrid as Grid).Children.Clear();

                if (groupbar.IsNavPaneMode)
                {
                    groupbar.StackGrid.CollapseNavPaneMode();
                }
            }

            groupbar.LoadTemplateChildren();
            GroupBarItem lastexpitem = null;

            foreach (GroupBarItem item in groupbar.Items)
            {
                if (item.IsExpanded || item.IsPressed)
                {
                    lastexpitem = item;
                    if (groupbar.StackGrid != null)
                    {
                        groupbar.StackGrid.LastExpItem = item;
                    }

                    if (groupbar.VisualMode == VisualMode.MultipleExpansion)
                    {
                        lastexpitem.IsExpanded = false;
                        item.CollapseContent();
                    }
                }
            }

            foreach (GroupBarItem item in groupbar.Items)
            {
                item.CollapseContent();
                item.LoadTemplateChildren();
            }

            if (lastexpitem != null)
            {
                lastexpitem.IsExpanded = !lastexpitem.IsExpanded;
                if (lastexpitem.HighlightOuterBorder != null)
                {
                    lastexpitem.HighlightOuterBorder.Visibility = Visibility.Visible;
                }
                if (lastexpitem.OuterBorder != null)
                {
                    lastexpitem.OuterBorder.Visibility = Visibility.Collapsed;
                }

                if (groupbar.VisualMode != VisualMode.MultipleExpansion)
                {
                    lastexpitem.ChangeItemExpandMode(groupbar);
                }
            }
            else
            {
                if (groupbar.VisualMode == VisualMode.SingleExpansion)
                {
                    lastexpitem = (GroupBarItem)groupbar.Items[0];
                    lastexpitem.ChangeItemExpandMode(groupbar);
                }
            }
        }

        /// <summary>
        /// Allows the collapse changed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void AllowCollapseChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            GroupBar groupbar = (GroupBar)obj;

            if (groupbar.StackGrid != null)
            {
                if (groupbar.AllowCollapse)
                {
                    groupbar.StackGrid.AllowCollapseVisibility = Visibility.Visible;
                }
                else
                {
                    groupbar.StackGrid.AllowCollapseVisibility = Visibility.Collapsed;
                }
            }
        }

        private static void OnShowGripperChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            GroupBar groupBar = obj as GroupBar;

            if (groupBar.stackGrid != null && groupBar.stackGrid.splitterElement != null)
            {
                groupBar.stackGrid.ValidateSplitterVisibility();
            }
        }

        private static void OnSelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            GroupBar groupbar = (GroupBar)obj;
            GroupBarItem newitem = e.NewValue as GroupBarItem;

            if (newitem != null)
            {
                if (groupbar.VisualMode != VisualMode.MultipleExpansion)
                {
                    newitem.IsExpanded = true;
                    newitem.ChangeItemExpandMode(groupbar);
                    double unexpandedItemHeight = 0d;
                    for (int i = 0; i < groupbar.Items.Count; i++)
                    {
                        GroupBarItem gitem = groupbar.Items[i] as GroupBarItem;
                        if (gitem == null)
                            gitem = groupbar.ItemContainerGenerator.ContainerFromIndex(0) as GroupBarItem;
                        if (gitem != null && !gitem.IsExpanded && gitem.ActualHeight > 0d)
                        {
                            unexpandedItemHeight += gitem.ActualHeight;
                        }
                    }
                    if (unexpandedItemHeight > 0d)
                    {
                        double availableHeight = groupbar.ActualHeight - unexpandedItemHeight;
                        if (availableHeight > 0d)
                            newitem.MaxHeight = availableHeight;
                    }
                }
            }

            groupbar.OnSelectedItemChanged(e);
        }

        private void OnSelectedItemChanged(DependencyPropertyChangedEventArgs e)
        {
            GroupBarItem newitem = e.NewValue as GroupBarItem;

            GroupBarItem olditem = e.OldValue as GroupBarItem;

            if (olditem != null && VisualMode == VisualMode.StackMode)
            {
                olditem.IsExpanded = false;
            }
            else if (olditem == null && VisualMode == VisualMode.StackMode)
            {
                if (StackGrid != null)
                {
                    GroupBarItem gBarItem = this.StackGrid.ItemContainerGenerator.ContainerFromItem(e.OldValue) as GroupBarItem;
                    if (gBarItem != null)
                    {
                        gBarItem.IsExpanded = false;
                    }
                }
            }
                
            if (newitem != null && SelectionChanged != null)
            {
                int selectedIndex = -1;

                if (VisualMode == VisualMode.StackMode)
                {
                    selectedIndex = this.stackGrid.ItemContainerGenerator.IndexFromContainer(newitem);
                }
                else
                {
                    selectedIndex = this.nonStackGridElement.ItemContainerGenerator.IndexFromContainer(newitem);
                }

                if(selectedIndex != -1)
                    this.SelectionChanged(this, new GroupBarSelectionChangedEventArgs(selectedIndex));
            }
            else if (newitem == null && VisualMode == VisualMode.StackMode)
            {
                if (StackGrid != null)
                {
                    GroupBarItem gBarItem = this.StackGrid.ItemContainerGenerator.ContainerFromItem(e.NewValue) as GroupBarItem;
                    if (gBarItem != null)
                    {
                        gBarItem.IsSelected = true;
                        gBarItem.IsExpanded = true;
                        VisualStateManager.GoToState(gBarItem, "Selected", true);
                        int selectedIndex = -1;
                        selectedIndex = this.StackGrid.Items.IndexOf(e.NewValue);
                        if (selectedIndex != -1)
                        {
                            this.SelectionChanged(this, new GroupBarSelectionChangedEventArgs(selectedIndex));
                        }
                    }
                    else
                    {
                        int indx = this.StackGrid.Items.IndexOf(e.NewValue);
                        if (indx >= 0)
                        {
                            this.SelectionChanged(this, new GroupBarSelectionChangedEventArgs(indx));
                        }
                    }
                }
            }
                
        }

        /// <summary>
        /// Shows the navigation pane text changed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ShowNavigationPaneTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            GroupBar groupbar = (GroupBar)obj;

            if (groupbar.StackGrid != null)
            {
                if (groupbar.IsNavPaneMode && groupbar.ShowNavigationPaneText)
                {
                    groupbar.StackGrid.NavPaneText.Visibility = Visibility.Visible;
                    groupbar.ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    groupbar.ScrollViewer.ScrollToHorizontalOffset(0);
                }
                else
                {
                    groupbar.StackGrid.NavPaneText.Visibility = Visibility.Collapsed;
                    groupbar.ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if(this.StackGrid != null && this.StackGrid.Toolbar != null)
                this.StackGrid.Toolbar.Items.Clear();
                if (this.StackGrid != null && this.StackGrid.navToolBarStackPanel != null)
                    this.StackGrid.navToolBarStackPanel.Children.Clear();
            }
            base.OnItemsChanged(e);
        }
    }

    /// <summary>
    /// Initialize a new instance of <see cref="NonStackGrid"/>
    /// </summary>
    public class NonStackGrid : ItemsControl
    {
        #region Constructors
        /// <summary>
        /// Initializes new instance of the <see cref="GroupBar"/> class.
        /// </summary>
        public NonStackGrid()
        {
            this.DefaultStyleKey = typeof(NonStackGrid);
        }
        #endregion

        #region Implementation

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        internal GroupBar groupBar;

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">The element used to display the specified item.</param>
        /// <param name="item">The item to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            //ContentPresenter contPresenter = (element as ContentPresenter);

            //if (contPresenter != null)
            //{
            //    contPresenter.Content = item;
            //}

            GroupBarItem con_element = element as GroupBarItem;
            con_element._groupBar = groupBar;
            if (item is GroupBarItem)
            {
                base.PrepareContainerForItemOverride(con_element, item);
            }
            else
            {
                con_element.Content = item;
                con_element.DataContext = item;
                con_element._element = item;
                con_element.ContentTemplate = ItemTemplate;
                con_element.HeaderTemplate = con_element._groupBar.HeaderTemplate;
                base.PrepareContainerForItemOverride(con_element, con_element);
            }

        }

        /// <summary>
        /// Undoes the effects of the <see cref="M:System.Windows.Controls.ItemsControl.PrepareContainerForItemOverride(System.Windows.DependencyObject,System.Object)"/> method.
        /// </summary>
        /// <param name="element">The container element.</param>
        /// <param name="item">The item.</param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            //ContentPresenter contPresenter = (element as ContentPresenter);

            //if (contPresenter != null)
            //{
            //    contPresenter.Content = null;
            //}
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            GroupBarItem con_element = new GroupBarItem();
            return con_element;
            //return new ContentPresenter();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>
        /// true if the item is (or is eligible to be) its own container; otherwise, false.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is GroupBarItem;
            //return false;
        }
        #endregion
    }

    /// <summary>
    /// Initialize a new instance of <see cref="NonStackGrid"/>
    /// </summary>
    public class StackGrid : ItemsControl
    {
        #region Constants
        private const string StackModeHeaderGridElement = "ItemGrid";
        private const string MainHostGridElement = "MainHostGrid";
        private const string StackModeHeaderElement = "HeaderGrid";
        private const string NonStackModeGridElement = "ScrollViewer";
        private const string ItemsElement = "ItemsGrid";
        private const string NavigationToolbarElement = "NavT";
        private const string SplitterElement = "GroupBarSplitter";
        private const string ItemHeaderTextElement = "ItemHeaderText";
        private const string PopupMenuElement = "PopupMenu";
        private const string PopupBorderElement = "PopupBorder";
        private const string ItemBorderElement = "ItemBorder";
        private const string NavigationToolBarElement = "NavigationToolBar";
        private const string ItemHostElement = "ItemHost";
        private const string CollapseButtonElement = "CollapseButton";
        private const string ExpandButtonElement = "ExpandButton";
        private const string NavPanePopupElement = "NavPanePopup";
        private const string NavPaneContentElement = "NavPanelContent";
        private const string NavPaneGridElement = "NavPaneGrid";
        private const string NavPanelElement = "NavPanel";
        private const string NavPaneTextElement = "NavPaneText";
        internal FrameworkElement stackModeGrid;
        private const double DefPopupImageWidth = 16;
        private const double DefPopupImageHeight = 16;

        private const double NavPaneWidth = 25;

        private const double NavPaneMinSize = 15;
        private const double NavPaneContentMinSize = 30;
        #endregion

        #region Private members
        
        private Grid mainhostGrid = null;
        private Border groupbarSplitter = null;
        private Grid itemsGrid = null;

        /// <summary>
        /// List of indices of the hidden items.
        /// </summary>
        private readonly List<int> hiddenIndices = new List<int>();

        private NavigationToolbar toolbar;

        /// <summary>
        /// Direction of the dragging.
        /// </summary>
        private DragDirection dragDirection = DragDirection.None;

        /// <summary>
        /// Point on the splitter that stores mouse cursor coordinates
        /// when left mouse button is pressed.
        /// </summary>
        private Point mouseDownPoint;

        private bool isDragging = false;

        private const double ItemHeight = 26;

        internal FrameworkElement stackModeHeaderGridElement;
        internal FrameworkElement itemsElement;
        internal FrameworkElement itemGrid;
        internal FrameworkElement headerGrid;
        internal FrameworkElement splitterElement;
        internal FrameworkElement stackHeaderElement;
        internal FrameworkElement itemHeaderTextElement;
        internal FrameworkElement popupBorderElement;
        internal FrameworkElement mainHostGridElement;
        internal FrameworkElement itemBorderElement;
        internal FrameworkElement itemHostElement;
        internal FrameworkElement collapseButtonElement;
        internal FrameworkElement expandButtonElement;
        //private string navigationPaneText;

        private int itemContentIndex;
        private double stackModeHeaderGridElementMinHeight;
        private object itemContent;
        private GroupBarItem stackItem;
        private GroupBarItem lastexpItem;
        private Orientation orientation;
        private PopupMenu popupmenuElement;
        private PopupMenu navpanepopupmenuElement;
        private TextBlock navPaneText;
        private GroupBar localParent;
        private bool popupInit = false;
        private string stackHeaderText;
        //private GroupBarItem selectedItem;
        private double localParentNormalWidth;
        private bool navPaneElementIsOpen = false;
        private Point navPaneMouseDownPoint;
        private bool isMouseOverNavPane = false;
        private bool isMouseOverPopGridPressed = false;
        private bool isMouseOverMainHost = false;
        private bool isNavPaneHorResizing = false;
        private bool isNavPaneVerResizing = false;

        #endregion

        internal GroupBar groupBar;

        /// <summary>
        ///  Idenfies NavPaneText DependencyProperty
        /// </summary>
        public static readonly DependencyProperty NavigationPaneTextProperty =
            DependencyProperty.Register("NavigationPaneText", typeof(string), typeof(StackGrid), new PropertyMetadata("Navigation Pane"));

        #region Properties

        /// <summary>
        /// Gets or sets the stack mode header.
        /// </summary>
        /// <value>The stack mode header.</value>
        internal FrameworkElement StackModeHeader
        {
            get
            {
                return this.stackHeaderElement;
            }

            set
            {
                this.stackHeaderElement = value;
            }
        }

        /// <summary>
        /// Gets the header text element.
        /// </summary>
        /// <value>The header text element.</value>
        internal FrameworkElement HeaderTextElement
        {
            get
            {
                return this.itemHeaderTextElement;
            }
        }

        /// <summary>
        /// Gets or sets the height of the stack mode header grid element min.
        /// </summary>
        /// <value>The height of the stack mode header grid element min.</value>
        public double StackModeHeaderGridElementMinHeight
        {
            get
            {
                return this.stackModeHeaderGridElementMinHeight;
            }

            set
            {
                this.stackModeHeaderGridElementMinHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the stack header text.
        /// </summary>
        /// <value>The stack header text.</value>
        internal string StackHeaderText
        {
            get
            {
                return this.stackHeaderText;
            }

            set
            {
                this.stackHeaderText = value;
            }
        }

        /// <summary>
        /// Gets or sets the index of the item content.
        /// </summary>
        /// <value>The index of the item content.</value>
        internal int ItemContentIndex
        {
            get
            {
                return this.itemContentIndex;
            }

            set
            {
                this.itemContentIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the content of the item.
        /// </summary>
        /// <value>The content of the item.</value>
        internal object ItemContent
        {
            get
            {
                return this.itemContent;
            }

            set
            {
                this.itemContent = value;
            }
        }

        /// <summary>
        /// Gets or sets the item grid.
        /// </summary>
        /// <value>The item grid.</value>
        internal FrameworkElement ItemGrid
        {
            get
            {
                return this.itemGrid;
            }

            set
            {
                this.itemGrid = value;
            }
        }

        /// <summary>
        /// Gets or sets the stack item.
        /// </summary>
        /// <value>The stack item.</value>
        internal GroupBarItem StackItem
        {
            get
            {
                return this.stackItem;
            }

            set
            {
                this.stackItem = value;
            }
        }

        /// <summary>
        /// Gets or sets the header grid.
        /// </summary>
        /// <value>The header grid.</value>
        internal FrameworkElement HeaderGrid
        {
            get
            {
                return this.headerGrid;
            }

            set
            {
                this.headerGrid = value;
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
        /// Gets the number of items in stack.
        /// </summary>
        /// <value>The stack items count.</value>
        private int StackItemsCount
        {
            get
            {
                return Items.Count;
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
                return this.toolbar;
            }
        }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
        internal Orientation Orientation
        {
            get
            {
                return this.orientation;
            }

            set
            {
                this.orientation = value;
            }
        }

        /// <summary>
        /// Gets or sets the local parent.
        /// </summary>
        /// <value>The local parent.</value>
        internal GroupBar LocalParent
        {
            get
            {
                return this.localParent;
            }

            set
            {
                this.localParent = value;
            }
        }

        /// <summary>
        /// Gets the main host grid.
        /// </summary>
        /// <value>The main host grid.</value>
        internal FrameworkElement MainHostGrid
        {
            get
            {
                return this.mainHostGridElement;
            }
        }

        /// <summary>
        /// Gets the item border.
        /// </summary>
        /// <value>The item border.</value>
        internal FrameworkElement ItemBorder
        {
            get
            {
                return this.itemBorderElement;
            }
        }

        /// <summary>
        /// Gets or sets the header background.
        /// </summary>
        /// <value>The header background.</value>
        public Brush HeaderBackground
        {
            get
            {
                return (Brush)GetValue(HeaderBackgroundProperty);
            }

            set
            {
                SetValue(HeaderBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the last exp item.
        /// </summary>
        /// <value>The last exp item.</value>
        internal GroupBarItem LastExpItem
        {
            get
            {
                return lastexpItem;
            }

            set
            {
                lastexpItem = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the local parent normal.
        /// </summary>s
        /// <value>The width of the local parent normal.</value>
        internal double LocalParentNormalWidth
        {
            get
            {
                return this.localParentNormalWidth;
            }

            set
            {
                this.localParentNormalWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [nav pane element is open].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [nav pane element is open]; otherwise, <c>false</c>.
        /// </value>
        internal bool NavPaneElementIsOpen
        {
            get
            {
                return this.navPaneElementIsOpen;
            }

            set
            {
                this.navPaneElementIsOpen = value;
            }
        }

        /// <summary>
        /// Gets the nav pane text.
        /// </summary>
        /// <value>The nav pane text.</value>
        internal TextBlock NavPaneText
        {
            get
            {
                return this.navPaneText;
            }
        }

        /// <summary>
        /// Gets or sets the allow collapse visibility.
        /// </summary>
        /// <value>The allow collapse visibility.</value>
        public Visibility AllowCollapseVisibility
        {
            get
            {
                return (Visibility)GetValue(AllowCollapseVisibilityProperty);
            }

            set
            {
                SetValue(AllowCollapseVisibilityProperty, value);
            }
        }

      

        /// <summary>
        /// Gets or sets the show navigation pane text visibility.
        /// </summary>
        /// <value>The show navigation pane text visibility.</value>
        public Visibility ShowNavigationPaneTextVisibility
        {
            get
            {
                return (Visibility)GetValue(ShowNavigationPaneTextVisibilityProperty);
            }

            set
            {
                SetValue(ShowNavigationPaneTextVisibilityProperty, value);
            }
        }

        #endregion

        #region Dependency properties

        /// <summary>
        /// Identifies <see cref="HeaderBackgroundProperty">HeaderBackground</see>
        /// dependency Property
        /// </summary>
        internal static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(StackGrid), null);

        /// <summary>
        /// Identifies <see cref="AllowCollapseVisibilityProperty">AllowCollapseVisibility</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty AllowCollapseVisibilityProperty =
            DependencyProperty.Register("AllowCollapseVisibility", typeof(Visibility), typeof(StackGrid), new PropertyMetadata(AllowCollapseVisibilityChanged));

        /// <summary>
        /// Identifies <see cref="ShowNavigationPaneTextVisibilityProperty">ShowNavigationPaneTextVisibility</see>
        /// dependency Property
        /// </summary>
        public static readonly DependencyProperty ShowNavigationPaneTextVisibilityProperty =
            DependencyProperty.Register("ShowNavigationPaneTextVisibility", typeof(Visibility), typeof(StackGrid), new PropertyMetadata(ShowNavigationPaneTextVisibilityChanged));

        internal double ContentPanelHeight = 350;

        #endregion

        #region Initialization
        /// <summary>
        /// Initializes new instance of the <see cref="GroupBar"/> class.
        /// </summary>
        public StackGrid()
        {
            this.DefaultStyleKey = typeof(StackGrid);
            this.Loaded -= new RoutedEventHandler(StackGrid_Loaded);
            this.Loaded += new RoutedEventHandler(StackGrid_Loaded);
        }

        void StackGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.groupBar != null)
            {
                GroupBarItem gBaritem = this.ItemContainerGenerator.ContainerFromItem(this.groupBar.SelectedItem) as GroupBarItem;
                if (gBaritem != null)
                {
                    gBaritem.IsExpanded = true;
                    gBaritem.IsSelected = true;
                    gBaritem.Expand();
                }
            }
        }

      
        #endregion

        #region Implementation
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.LoadTemplateChildren();
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">The element used to display the specified item.</param>
        /// <param name="item">The item to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            //ContentPresenter contPresenter = (element as ContentPresenter);

            //if (contPresenter != null)
            //{
            //    contPresenter.Content = item;
            //}

            GroupBarItem con_element = element as GroupBarItem;
            con_element._groupBar = groupBar;
            if (item is GroupBarItem)
            {
                base.PrepareContainerForItemOverride(con_element, item);
            }
            else
            {
                con_element.Content = item;
                con_element._element = item;
                con_element.ContentTemplate = ItemTemplate;
                con_element.HeaderTemplate = groupBar.HeaderTemplate;
                base.PrepareContainerForItemOverride(con_element, con_element);
            }

        }

        /// <summary>
        /// Undoes the effects of the <see cref="M:System.Windows.Controls.ItemsControl.PrepareContainerForItemOverride(System.Windows.DependencyObject,System.Object)"/> method.
        /// </summary>
        /// <param name="element">The container element.</param>
        /// <param name="item">The item.</param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            //ContentPresenter contPresenter = (element as ContentPresenter);

            //if (contPresenter != null)
            //{
            //    contPresenter.Content = null;
            //}
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            //return new ContentPresenter();
            GroupBarItem con_element = new GroupBarItem();
            return con_element;
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>
        /// true if the item is (or is eligible to be) its own container; otherwise, false.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is GroupBarItem;
            //return false;
        }
        ContentControl itemHeaderImage = null;
        internal StackPanel navToolBarStackPanel=null;
        internal StackPanel navToolBarPanel = null;
        /// <summary>
        /// Loads the template children.
        /// </summary>
        internal void LoadTemplateChildren()
        {
            this.stackModeHeaderGridElement = GetTemplateChild(StackModeHeaderGridElement) as FrameworkElement;
            this.mainHostGridElement = GetTemplateChild(MainHostGridElement) as FrameworkElement;
            this.itemBorderElement = GetTemplateChild(ItemBorderElement) as FrameworkElement;
            this.stackHeaderElement = GetTemplateChild(StackModeHeaderElement) as FrameworkElement;
            this.itemHostElement = GetTemplateChild(ItemHostElement) as FrameworkElement;
            this.collapseButtonElement = GetTemplateChild(CollapseButtonElement) as FrameworkElement;
            this.expandButtonElement = GetTemplateChild(ExpandButtonElement) as FrameworkElement;
            this.navpanepopupmenuElement = GetTemplateChild(NavPanePopupElement) as PopupMenu;
            this.StackModeHeader = this.stackHeaderElement;
            this.stackModeGrid = GetTemplateChild("StackModeGrid") as FrameworkElement;
            this.itemsElement = GetTemplateChild(ItemsElement) as FrameworkElement;
            this.splitterElement = GetTemplateChild(SplitterElement) as FrameworkElement;
            this.itemHeaderTextElement = GetTemplateChild(ItemHeaderTextElement) as FrameworkElement;
            this.itemHeaderImage = GetTemplateChild("ItemHeaderImage") as ContentControl;
            this.popupBorderElement = GetTemplateChild(PopupBorderElement) as FrameworkElement;
            this.navToolBarStackPanel = GetTemplateChild("NavigationToolBar") as StackPanel;
            this.navToolBarPanel = GetTemplateChild("NavPanel") as StackPanel;
            this.itemGrid = this.stackModeHeaderGridElement;
            this.HeaderGrid = this.stackModeHeaderGridElement;

            ValidateSplitterVisibility();

            if (this.LastExpItem != null)
            {
                this.LastExpItem.IsPressed = true;
                this.LastExpItem.IsExpanded = true;
                this.LastExpItem.CollapseContent();
                this.LastExpItem.SetExpanded(this.LocalParent);
                this.LastExpItem.UpdateVisualState();
            }

            if (this.collapseButtonElement != null)
            {
                this.collapseButtonElement.MouseLeftButtonUp += new MouseButtonEventHandler(CollapseButtonElement_MouseLeftButtonUp);
            }

            if (this.expandButtonElement != null)
            {
                this.expandButtonElement.MouseLeftButtonUp += new MouseButtonEventHandler(ExpandButtonElement_MouseLeftButtonUp);
            }
            mainhostGrid = GetTemplateChild("MainHostGrid") as Grid;
            groupbarSplitter = GetTemplateChild("GroupBarSplitter") as Border;
            itemsGrid = GetTemplateChild("ItemsGrid") as Grid;
        }
              

        internal void ValidateSplitterVisibility()
        {
            //if (this.Items.Count > 0 && this.splitterElement != null)
            if (this.splitterElement != null && this.groupBar.ShowGripper)
            {
                this.splitterElement.MouseMove -= new MouseEventHandler(this.SplitterElement_MouseMove);
                this.splitterElement.MouseLeftButtonDown -= new MouseButtonEventHandler(this.SplitterElement_MouseLeftButtonDown);
                this.splitterElement.MouseLeftButtonUp -= new MouseButtonEventHandler(this.SplitterElement_MouseLeftButtonUp);

                if (this.popupBorderElement != null)
                {
                    this.popupBorderElement.MouseLeftButtonDown -= new MouseButtonEventHandler(this.PopupBorderElement_MouseLeftButtonDown);
                    this.popupBorderElement.MouseEnter -= new MouseEventHandler(this.PopupBorderElement_MouseEnter);
                    this.popupBorderElement.MouseLeave -= new MouseEventHandler(this.PopupBorderElement_MouseLeave);
                    this.popupBorderElement.MouseLeftButtonUp -= new MouseButtonEventHandler(this.PopupBorderElement_MouseLeftButtonUp);
                }

                this.splitterElement.Visibility = Visibility.Visible;
                this.SetCursor();

                this.splitterElement.MouseMove += new MouseEventHandler(this.SplitterElement_MouseMove);
                this.splitterElement.MouseLeftButtonDown += new MouseButtonEventHandler(this.SplitterElement_MouseLeftButtonDown);
                this.splitterElement.MouseLeftButtonUp += new MouseButtonEventHandler(this.SplitterElement_MouseLeftButtonUp);

                this.toolbar = new NavigationToolbar();

                this.popupmenuElement = GetTemplateChild(PopupMenuElement) as PopupMenu;

                this.popupBorderElement.MouseLeftButtonDown += new MouseButtonEventHandler(this.PopupBorderElement_MouseLeftButtonDown);
                this.popupBorderElement.MouseEnter += new MouseEventHandler(this.PopupBorderElement_MouseEnter);
                this.popupBorderElement.MouseLeave += new MouseEventHandler(this.PopupBorderElement_MouseLeave);
                this.popupBorderElement.MouseLeftButtonUp += new MouseButtonEventHandler(this.PopupBorderElement_MouseLeftButtonUp);

                Border popBorder = GetTemplateChild(PopupBorderElement) as Border;

                this.popupmenuElement.ParentStackGrid = this;
                this.popupmenuElement.PopupFrom = popBorder;

                this.RotatePopup();
            }
            else if (this.splitterElement != null)
            {
                this.splitterElement.Visibility = Visibility.Collapsed;
            }

        }

        /// <summary>
        /// Handles the MouseLeave event of the MainHostGridElement control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void MainHostGridElement_MouseLeave(object sender, MouseEventArgs e)
        {
            this.isMouseOverMainHost = false;

            if (this.LocalParent.IsNavPaneMode && !this.NavPaneElementIsOpen)
            {
                VisualStateManager.GoToState(this, "NavPaneMouseOut", true);
            }
        }

        /// <summary>
        /// Handles the MouseEnter event of the MainHostGridElement control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void MainHostGridElement_MouseEnter(object sender, MouseEventArgs e)
        {
            this.isMouseOverMainHost = true;

            if (this.LocalParent.IsNavPaneMode && !this.NavPaneElementIsOpen)
            {
                VisualStateManager.GoToState(this, "NavPaneMouseOver", true);
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the MainHostGridElement control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void MainHostGridElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Grid navPaneElement = GetTemplateChild(NavPaneGridElement) as Grid;
            ContentControl navPanelContentElement = GetTemplateChild(NavPaneContentElement) as ContentControl;

            if (navPaneElement != null)
            {
                if (this.NavPaneElementIsOpen)
                {
                    CollapseNavPane();
                }
                else
                {
                    ExpandNavPane();
                }
            }

            this.navpanepopupmenuElement.PopGrid.LostFocus += new RoutedEventHandler(navPaneElement_LostFocus);
        }

        /// <summary>
        /// Expands the nav pane.
        /// </summary>
        internal void ExpandNavPane()
        {
            this.navpanepopupmenuElement.Margin = new Thickness(24, 0, 0, 0);
            this.navpanepopupmenuElement.IsNavPaneMode = true;
            this.navpanepopupmenuElement.ParentStackGrid = this;
            this.navpanepopupmenuElement.PopupFrom = this;
            this.navpanepopupmenuElement.PopGrid.Children.Clear();

            if (LocalParent.SelectedItem != null)
            {
                this.navpanepopupmenuElement.Background = this.Background;
                FrameworkElement content = null;
                if(this.LocalParent.SelectedItem as GroupBarItem != null)
                content = (this.LocalParent.SelectedItem as GroupBarItem).Content as FrameworkElement;
                (this.itemGrid as Grid).Children.Clear();
                this.navpanepopupmenuElement.PopGrid.Children.Clear();
                this.navpanepopupmenuElement.PopGrid.Background = this.Background;
                ((Border)this.navpanepopupmenuElement.PopGrid.Parent).BorderBrush = this.LocalParent.BorderBrush;
                ((Border)this.navpanepopupmenuElement.PopGrid.Parent).BorderThickness = new Thickness(1);
                Border contentBorder = new Border();
                contentBorder.Child = content;

                if (this.LocalParent.Orientation == Orientation.Horizontal)
                {
                    if (Double.IsNaN(this.navpanepopupmenuElement.PopGrid.Width)
                        || Double.IsNaN(this.navpanepopupmenuElement.PopGrid.Height))
                    {
                        this.navpanepopupmenuElement.PopGrid.Width = this.LocalParent.ItemGridWidth + 20;
                        this.navpanepopupmenuElement.PopGrid.Height = this.LocalParent.ItemGridWidth + 20;
                    }
                    else
                    {
                        content.Height = this.navpanepopupmenuElement.PopGrid.Height - 20;
                        content.Width = this.navpanepopupmenuElement.PopGrid.Width - 20;
                    }
                }

                contentBorder.Margin = new Thickness(5);
                contentBorder.BorderThickness = new Thickness(1);
                contentBorder.BorderBrush = this.localParent.BorderBrush;

                this.navpanepopupmenuElement.HasContentBorder = true;
                this.navpanepopupmenuElement.PopGrid.Children.Add(contentBorder);
                this.navpanepopupmenuElement.PopGrid.Visibility = Visibility.Visible;
                this.navpanepopupmenuElement.PopMenu.IsOpen = true;

                this.navpanepopupmenuElement.PopGrid.MouseLeftButtonDown += new MouseButtonEventHandler(PopGrid_MouseLeftButtonDown);
                this.navpanepopupmenuElement.PopGrid.MouseLeftButtonUp += new MouseButtonEventHandler(PopGrid_MouseLeftButtonUp);
                this.navpanepopupmenuElement.PopGrid.MouseEnter += new MouseEventHandler(PopGrid_MouseEnter);
                this.navpanepopupmenuElement.PopGrid.MouseMove += new MouseEventHandler(PopGrid_MouseMove);
                this.navpanepopupmenuElement.PopGrid.MouseLeave += new MouseEventHandler(PopGrid_MouseLeave);
            }

            this.NavPaneElementIsOpen = true;
            VisualStateManager.GoToState(this, "NavPaneSelected", true);
        }

        /// <summary>
        /// Collapses the nav pane.
        /// </summary>
        internal void CollapseNavPane()
        {
            this.navpanepopupmenuElement.PopMenu.IsOpen = false;
            if (this.navpanepopupmenuElement.PopGrid.Children.Count > 0)
            {
                ((Border)this.navpanepopupmenuElement.PopGrid.Parent).BorderBrush = new SolidColorBrush(Colors.Transparent);
                ((Border)this.navpanepopupmenuElement.PopGrid.Parent).BorderThickness = new Thickness(0);
                this.navpanepopupmenuElement.PopGrid.Visibility = Visibility.Collapsed;
                (this.navpanepopupmenuElement.PopGrid.Children[0] as Border).Child = null;
                this.navpanepopupmenuElement.HasContentBorder = false;
            }

            this.navpanepopupmenuElement.PopGrid.Children.Clear();
            this.NavPaneElementIsOpen = false;

            VisualStateManager.GoToState(this, "NavPaneUnselected", true);
        }

        /// <summary>
        /// Handles the MouseMove event of the PopGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void PopGrid_MouseMove(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition(this.navpanepopupmenuElement);

            bool horResize = point.X > this.navpanepopupmenuElement.PopGrid.ActualWidth - 5 && point.X <= this.navpanepopupmenuElement.PopGrid.ActualWidth;

            bool verResize = point.Y > this.navpanepopupmenuElement.PopGrid.ActualHeight - 5 && point.Y <= this.navpanepopupmenuElement.PopGrid.ActualHeight;

            if (horResize)
            {
                if (this.LocalParent.Orientation == Orientation.Vertical)
                {
                    this.navpanepopupmenuElement.PopGrid.Cursor = Cursors.SizeWE;
                }
                else
                {
                    this.navpanepopupmenuElement.PopGrid.Cursor = Cursors.SizeNS;
                }

                this.isNavPaneHorResizing = true;
                this.isNavPaneVerResizing = false;
            }
            else if (verResize)
            {
                if (this.LocalParent.Orientation == Orientation.Vertical)
                {
                    this.navpanepopupmenuElement.PopGrid.Cursor = Cursors.SizeNS;
                }
                else
                {
                    this.navpanepopupmenuElement.PopGrid.Cursor = Cursors.SizeWE;
                }

                this.isNavPaneVerResizing = true;
                this.isNavPaneHorResizing = false;
            }
            else
            {
                this.navpanepopupmenuElement.PopGrid.Cursor = Cursors.Arrow;
            }
        }

        /// <summary>
        /// Handles the MouseLeave event of the PopGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void PopGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            this.isMouseOverNavPane = false;
        }

        /// <summary>
        /// Handles the MouseEnter event of the PopGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void PopGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            this.isMouseOverNavPane = true;

            Point point = e.GetPosition(this.navpanepopupmenuElement);

            if (this.LocalParent.Orientation == Orientation.Vertical)
            {
                this.navpanepopupmenuElement.PopGrid.Cursor = Cursors.SizeWE;
            }
            else
            {
                this.navpanepopupmenuElement.PopGrid.Cursor = Cursors.SizeNS;
            }
        }

        /// <summary>
        /// Handles the LostFocus event of the navPanelContentElement control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void navPanelContentElement_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!this.isMouseOverNavPane && this.NavPaneElementIsOpen && !this.isMouseOverMainHost)
            {
                this.navpanepopupmenuElement.PopMenu.IsOpen = false;
            }
        }

        /// <summary>
        /// Handles the LostFocus event of the navPaneElement control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void navPaneElement_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!this.isMouseOverNavPane && this.NavPaneElementIsOpen && !this.isMouseOverMainHost)
            {
                this.navpanepopupmenuElement.PopMenu.IsOpen = false;
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the PopGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void PopGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.navPaneMouseDownPoint = e.GetPosition(this.navpanepopupmenuElement);
            this.isMouseOverPopGridPressed = true;

            if ((this.navPaneMouseDownPoint.X >= (sender as Grid).ActualWidth - 5 || this.navPaneMouseDownPoint.Y >= (sender as Grid).ActualHeight - 5) && this.isMouseOverPopGridPressed)
            {
                (sender as Grid).CaptureMouse();

                (sender as Grid).MouseMove += new MouseEventHandler(PopGridBorder_MouseMove);
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the PopGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void PopGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (sender as Grid).ReleaseMouseCapture();

            this.isMouseOverPopGridPressed = false;

            (sender as Grid).MouseMove -= new MouseEventHandler(PopGridBorder_MouseMove);
        }

        /// <summary>
        /// Handles the MouseMove event of the PopGridBorder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void PopGridBorder_MouseMove(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition(this.navpanepopupmenuElement);

            if (point.X > NavPaneMinSize && point.Y > NavPaneMinSize && this.isMouseOverPopGridPressed)
            {
                if (this.isNavPaneHorResizing)
                {
                    if (this.localParent.Orientation == Orientation.Vertical)
                    {
                        this.navpanepopupmenuElement.PopGrid.Width = point.X;
                        this.navpanepopupmenuElement.PopGrid.Height = point.X;
                    }
                    else
                    {
                        if (point.X > NavPaneContentMinSize * 2)
                        {
                            ((this.navpanepopupmenuElement.PopGrid.Children[0] as Border).Child as FrameworkElement).Height = point.X - 20;
                            ((this.navpanepopupmenuElement.PopGrid.Children[0] as Border).Child as FrameworkElement).Width = point.X - 20;
                            this.navpanepopupmenuElement.PopGrid.Width = point.X;
                            this.navpanepopupmenuElement.PopGrid.Height = point.X;
                        }
                    }
                }

                if (this.isNavPaneVerResizing)
                {
                    if (this.localParent.Orientation == Orientation.Vertical)
                    {
                        this.navpanepopupmenuElement.PopGrid.Height = point.Y;
                        this.navpanepopupmenuElement.PopGrid.Width = point.Y;
                    }
                    else
                    {
                        if (point.Y > NavPaneContentMinSize * 2)
                        {
                            ((this.navpanepopupmenuElement.PopGrid.Children[0] as Border).Child as FrameworkElement).Width = point.Y - 20;
                            ((this.navpanepopupmenuElement.PopGrid.Children[0] as Border).Child as FrameworkElement).Height = point.Y - 20;
                            this.navpanepopupmenuElement.PopGrid.Width = point.Y;
                            this.navpanepopupmenuElement.PopGrid.Height = point.Y;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the ExpandButtonElement control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void ExpandButtonElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.LocalParent != null)
            {
                CollapseNavPaneMode();
            }
        }

        /// <summary>
        /// Collapses the nav pane mode.
        /// </summary>
        internal void CollapseNavPaneMode()
        {
            if (this.mainHostGridElement != null)
            {
                this.mainHostGridElement.MouseLeftButtonUp -= new MouseButtonEventHandler(MainHostGridElement_MouseLeftButtonUp);
            }

            this.LocalParent.Width = this.LocalParentNormalWidth;

            this.HeaderTextElement.Visibility = Visibility.Visible;

            VisualStateManager.GoToState(this, "NavPaneUnselected", true);

            StackPanel nbTextElement = GetTemplateChild(NavigationToolBarElement) as StackPanel;
            nbTextElement.Visibility = Visibility.Visible;
            StackPanel nbPaneltElement = GetTemplateChild(NavPanelElement) as StackPanel;
            nbPaneltElement.HorizontalAlignment = HorizontalAlignment.Right;
            this.collapseButtonElement.Visibility = Visibility.Visible;
            this.expandButtonElement.Visibility = Visibility.Collapsed;
            if (this.itemHeaderImage != null)
                itemHeaderImage.Visibility = System.Windows.Visibility.Visible;
            this.expandButtonElement.HorizontalAlignment = HorizontalAlignment.Right;
            this.splitterElement.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.itemGrid.Visibility = Visibility.Visible;
            Grid navPaneElement = GetTemplateChild("NavPaneGrid") as Grid;
            navPaneElement.Visibility = Visibility.Collapsed;
            this.navpanepopupmenuElement.PopMenu.IsOpen = false;
            ((Border)this.navpanepopupmenuElement.PopGrid.Parent).BorderBrush = new SolidColorBrush(Colors.Transparent);
            ((Border)this.navpanepopupmenuElement.PopGrid.Parent).BorderThickness = new Thickness(0);
            this.navpanepopupmenuElement.PopGrid.Visibility = Visibility.Collapsed;

            if (this.navpanepopupmenuElement.PopGrid.Children.Count > 0)
            {
                (this.navpanepopupmenuElement.PopGrid.Children[0] as Border).Child = null;
                this.navpanepopupmenuElement.HasContentBorder = false;
            }

            this.LocalParent.IsNavPaneMode = false;
            this.navpanepopupmenuElement.PopGrid.Children.Clear();
            this.navPaneElementIsOpen = false;
            this.LocalParent.ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            if((this.LocalParent.SelectedItem as GroupBarItem) != null)
            (this.LocalParent.SelectedItem as GroupBarItem).SetExpanded(this.LocalParent);

            this.LocalParent.RotateGroupBar();
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the CollapseButtonElement control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void CollapseButtonElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.LocalParent != null)
            {
                ShowNavPaneMode();
            }
        }

        /// <summary>
        /// Shows the nav pane mode.
        /// </summary>
        internal void ShowNavPaneMode()
        {
            this.LocalParentNormalWidth = this.LocalParent.Width;
            this.LocalParent.Width = NavPaneWidth;
            ((Border)this.ItemBorder).BorderThickness = new Thickness(0);
            ((Border)this.ItemBorder).BorderBrush = new SolidColorBrush(Colors.Transparent);

            this.HeaderTextElement.Visibility = Visibility.Collapsed;

            if (this.mainHostGridElement != null)
            {
                this.mainHostGridElement.MouseEnter += new MouseEventHandler(MainHostGridElement_MouseEnter);
                this.mainHostGridElement.MouseLeave += new MouseEventHandler(MainHostGridElement_MouseLeave);
                this.mainHostGridElement.MouseLeftButtonUp += new MouseButtonEventHandler(MainHostGridElement_MouseLeftButtonUp);
            }

            StackPanel nbTextElement = GetTemplateChild(NavigationToolBarElement) as StackPanel;
            nbTextElement.Visibility = Visibility.Collapsed;
            StackPanel nbPaneltElement = GetTemplateChild(NavPanelElement) as StackPanel;
            this.navPaneText = GetTemplateChild(NavPaneTextElement) as TextBlock;
            nbPaneltElement.HorizontalAlignment = HorizontalAlignment.Left;
            this.collapseButtonElement.Visibility = Visibility.Collapsed;
            this.expandButtonElement.Visibility = Visibility.Visible;
            this.expandButtonElement.HorizontalAlignment = HorizontalAlignment.Left;
            if (this.itemHeaderImage != null)
                itemHeaderImage.Visibility = System.Windows.Visibility.Collapsed;
            this.splitterElement.HorizontalAlignment = HorizontalAlignment.Left;
            this.itemGrid.Visibility = Visibility.Collapsed;
            Grid navPaneElement = GetTemplateChild(NavPaneGridElement) as Grid;
            navPaneElement.Visibility = Visibility.Visible;
            this.LocalParent.ScrollViewer.ScrollToHorizontalOffset(0);

            if (this.LocalParent.Orientation == Orientation.Horizontal)
            {
                RotateTransform transf = new RotateTransform();
                transf.Angle = 180;
                this.NavPaneText.RenderTransformOrigin = new Point(0.5, 0.5);
                this.NavPaneText.RenderTransform = transf;
            }

            if (!this.LocalParent.ShowNavigationPaneText)
            {
                this.LocalParent.ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                this.NavPaneText.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.LocalParent.ScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }

            this.LocalParent.IsNavPaneMode = true;
        }

        /// <summary>
        /// Sets the cursor.
        /// </summary>
        internal void SetCursor()
        {
            if (this.splitterElement != null && this.Orientation == Orientation.Vertical)
            {
                this.splitterElement.Cursor = Cursors.SizeNS;
            }
            else if (this.splitterElement != null && this.Orientation == Orientation.Horizontal)
            {
                this.splitterElement.Cursor = Cursors.SizeWE;
            }
        }

        /// <summary>
        /// Rotate PopupMenu
        /// </summary>
        internal void RotatePopup()
        {
            if (this.Orientation == Orientation.Horizontal)
            {
                TransformGroup transformGroup = new TransformGroup();
                TranslateTransform translateTransform = new TranslateTransform();
                translateTransform.X = 12;
                translateTransform.Y = 12;

                RotateTransform rotateTransform = new RotateTransform();
                rotateTransform.Angle = 90;

                transformGroup.Children.Add(translateTransform);
                transformGroup.Children.Add(rotateTransform);

                this.popupmenuElement.RenderTransformOrigin = new Point(0.5, 0.5);
                this.popupmenuElement.RenderTransform = transformGroup;
            }
            else
            {
                RotateTransform rtw = new RotateTransform();
                this.RenderTransformOrigin = new Point(0.5, 0.5);
                rtw.Angle = 0;
                if(this.popupmenuElement!= null)
                this.popupmenuElement.RenderTransform = rtw;
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the PopupBorderElement control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void PopupBorderElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            VisualStateManager.GoToState(this, "UnSelect", true);
        }

        /// <summary>
        /// Handles the MouseLeave event of the PopupBorderElement control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void PopupBorderElement_MouseLeave(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "MouseOut", true);
        }

        /// <summary>
        /// Handles the MouseEnter event of the PopupBorderElement control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void PopupBorderElement_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "MouseOver", true);
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the PopupBorderElement control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void PopupBorderElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.popupmenuElement = GetTemplateChild(PopupMenuElement) as PopupMenu;

            this.popupBorderElement.MouseLeftButtonDown += new MouseButtonEventHandler(this.PopupBorderElement_MouseLeftButtonDown);
            this.popupBorderElement.MouseEnter += new MouseEventHandler(this.PopupBorderElement_MouseEnter);
            this.popupBorderElement.MouseLeave += new MouseEventHandler(this.PopupBorderElement_MouseLeave);
            this.popupBorderElement.MouseLeftButtonUp += new MouseButtonEventHandler(this.PopupBorderElement_MouseLeftButtonUp);

            if (!popupInit)
            {
                InitPopup();
                popupInit = true;
            }

            this.RotatePopup();
        }

        /// <summary>
        /// Inits the popup.
        /// </summary>
        private void InitPopup()
        {
            Border popBorder = GetTemplateChild(PopupBorderElement) as Border;
            this.popupmenuElement.ParentStackGrid = this;
            this.popupmenuElement.PopupFrom = popBorder;
            this.SetPopupItems();
        }

        /// <summary>
        /// Sets the popup items.
        /// </summary>
        private void SetPopupItems()
        {
            List<PopupMenuItem> itemsCollection = new List<PopupMenuItem>();

            for (int i = 0; i < this.Items.Count; i++)
            {

                GroupBarItem item = this.ItemContainerGenerator.ContainerFromIndex(i) as GroupBarItem;

                Image im = new Image();
                im.Source = item.HeaderImageSource;
                im.Margin = new Thickness(5);
                im.Height = DefPopupImageHeight;
                im.Width = DefPopupImageWidth;
                im.HorizontalAlignment = HorizontalAlignment.Left;
                im.VerticalAlignment = VerticalAlignment.Center;

                Image imchk = new Image();
                imchk.Margin = new Thickness(5);
                imchk.Height = DefPopupImageHeight;
                imchk.Width = DefPopupImageWidth;
                imchk.HorizontalAlignment = HorizontalAlignment.Left;
                imchk.VerticalAlignment = VerticalAlignment.Center;

                Uri uri = new Uri(@"/Syncfusion.Tools.Silverlight;component/Controls/GroupBar/Resources/checked.png", UriKind.Relative);
                ImageSource imgSource = new BitmapImage(uri);
                imchk.Source = imgSource;

                if (item.Hidden)
                {
                    imchk.Visibility = Visibility.Collapsed;
                    im.Visibility = Visibility.Visible;
                }
                else
                {
                    imchk.Visibility = Visibility.Visible;
                    im.Visibility = Visibility.Collapsed;
                }

                if (item._element == null)
                {
                    ContentControl con = new ContentControl();
                    con.Content = item.HeaderText;
                    itemsCollection.Add(new PopupMenuItem() { HeaderText = con, HeaderImage = im, CheckedImage = imchk, Id = i, ParentId = null });
                }
                else
                {
                    ContentControl con = new ContentControl();
                    con.Content = item._element;
                    con.ContentTemplate = item.HeaderTemplate;
                    itemsCollection.Add(new PopupMenuItem() { HeaderText = con, HeaderImage = im, CheckedImage = imchk, Id = i, ParentId = null });
                }
            }

            this.popupmenuElement.SetMenuItems(itemsCollection);

            VisualStateManager.GoToState(this, "Select", true);
        }

        /// <summary>
        /// Hides the item.
        /// </summary>
        /// <param name="index">The index.</param>
        internal void HideItem(int index)
        {
            GroupBarItem gbItem = this.ItemContainerGenerator.ContainerFromIndex(index) as GroupBarItem;

            if (!gbItem.Hidden)
            {
                if (gbItem.ShowInGroupBar)
                {
                    gbItem.Visibility = Visibility.Collapsed;
                }
                else
                {
                    StackPanel nbTextElement = GetTemplateChild(NavigationToolBarElement) as StackPanel;

                    foreach (UIElement item in nbTextElement.Children)
                    {
                        if ((item as NavigationToolbarItem).GroupBarItem == gbItem)
                        {
                            item.Visibility = Visibility.Collapsed;
                        }
                    }
                }

                gbItem.Hidden = true;
            }
            else
            {
                if (gbItem.ShowInGroupBar)
                {
                    gbItem.Visibility = Visibility.Visible;
                }
                else
                {
                    StackPanel nbTextElement = GetTemplateChild(NavigationToolBarElement) as StackPanel;

                    foreach (UIElement item in nbTextElement.Children)
                    {
                        if ((item as NavigationToolbarItem).GroupBarItem == gbItem)
                        {
                            item.Visibility = Visibility.Visible;
                        }
                    }
                }

                gbItem.Hidden = false;
            }

            this.SetPopupItems();

            if (this.LocalParent.ScrollViewer.ComputedHorizontalScrollBarVisibility == Visibility.Visible || this.LocalParent.ScrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible)
            {
                this.LocalParent.ScrollViewer.ScrollToVerticalOffset(this.LocalParent.ScrollViewer.ExtentHeight);
            }
        }

        /// <summary>
        /// Performs the splitting of stack host.
        /// </summary>
        /// <param name="direction">Drag direction.</param>
        private void DoSplitting(DragDirection direction)
        {
            if (direction == DragDirection.Down && this.StackItemsCount > 0 && this.toolbar.Items.Count < this.Items.Count)
            {
                GroupBarItem lastStackItem = this.GetLastStackItem();
                bool isPossibleToSplitDown = true;
                if (this.groupBar != null && navToolBarStackPanel != null)
                {
                    if (this.toolbar != null && toolbar.Items.Count > 0 && navToolBarStackPanel.Children.Count > 0)
                    {
                        NavigationToolbarItem nitem = navToolBarStackPanel.Children[0] as NavigationToolbarItem;
                        if (nitem != null && this.groupBar != null)
                        {
                            double itemsWidth = this.toolbar.Items.Count * nitem.ActualWidth;
                            if (itemsWidth >= groupBar.ActualWidth)
                            {
                                isPossibleToSplitDown = false;
                            }
                        }
                    }
                }
                if (lastStackItem.ShowInGroupBar && !lastStackItem.Hidden && isPossibleToSplitDown)
                {
                    lastStackItem.Visibility = Visibility.Collapsed;
                    lastStackItem.ShowInGroupBar = false;

                    StackPanel nbElement = GetTemplateChild(NavigationToolBarElement) as StackPanel;

                    NavigationToolbarItem navItem = new NavigationToolbarItem(lastStackItem);

                    navItem.MouseLeftButtonDown += new MouseButtonEventHandler(this.NavItem_MouseLeftButtonDown);

                    NavigationToolbarItem navItem1 = new NavigationToolbarItem(lastStackItem);

                    nbElement.Children.Insert(0, navItem);
                    this.toolbar.Items.Insert(0, navItem1);
                }
            }
            else if (direction == DragDirection.Up && this.toolbar.Items.Count > 0)
            {
                GroupBarItem item = this.GetLastToolbarItem();

                if (!item.ShowInGroupBar && !item.Hidden)
                {
                    StackPanel nbTextElement = GetTemplateChild(NavigationToolBarElement) as StackPanel;

                    for (int i = 0; i < nbTextElement.Children.Count; i++)
                    {
                        if ((nbTextElement.Children[i] as NavigationToolbarItem).GroupBarItem == item)
                        {
                            nbTextElement.Children.RemoveAt(i);
                            this.toolbar.Items.RemoveAt(i);
                        }
                    }

                    item.ShowInGroupBar = true;
                    item.Visibility = Visibility.Visible;
                    item.UpdateVisualState();

                    if (item._groupBar != null)
                    {
                        foreach (var gbItem in item._groupBar.Items)
                        {
                            GroupBarItem it = null;

                            it = this.ItemContainerGenerator.ContainerFromItem(gbItem) as GroupBarItem;

                            if (it != null && !it.IsPressed)
                            {
                                it.UpdateVisualState();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the NavItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void NavItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            GroupBarItem item = (sender as NavigationToolbarItem).GroupBarItem;

            (this.ItemGrid as Grid).Children.Clear();

            if (item.Content != null)
            {
                if (item.Content is UIElement)
                    (this.ItemGrid as Grid).Children.Add(item.Content as UIElement);
                else
                {
                    ContentControl con = new ContentControl();
                    con.Content = item.Content;
                    con.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                    con.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    con.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
                    con.VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch;
                    con.ContentTemplate = item.ContentTemplate;
                    (this.ItemGrid as Grid).Children.Add(con);
                }
            }

            //(this.HeaderTextElement as TextBlock).Text = item.HeaderText;
            if (item._element == null)
            {
                (this.HeaderTextElement as ContentControl).Content = item.HeaderText;
            }
            else
            {
                (this.HeaderTextElement as ContentControl).Content = item._element;
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the SplitterElement control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void SplitterElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.isDragging = false;
            (sender as Border).ReleaseMouseCapture();
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the SplitterElement control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void SplitterElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.isDragging = true;
            (sender as Border).CaptureMouse();
            this.mouseDownPoint = e.GetPosition((sender as Border));
        }

        /// <summary>
        /// Handles the MouseMove event of the SplitterElement control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void SplitterElement_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.isDragging)
            {
                Point scrollbarpoint = e.GetPosition((sender as Border));
                Point stackpoint = e.GetPosition(this);

                double deltaY = scrollbarpoint.Y - this.mouseDownPoint.Y;
                bool suitable = false;

                if (stackpoint.Y <= this.ActualHeight)
                {
                    suitable = true;
                }

                this.dragDirection = deltaY > 0 ? DragDirection.Down : DragDirection.Up;

                double absDeltaY = Math.Abs(deltaY);

                if (absDeltaY > ItemHeight && suitable)
                {
                    this.DoSplitting(this.dragDirection);
                }
            }
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

            for (int i = Items.Count - 1; i >= 0; i--)
            {
                //item = Items[i] as GroupBarItem;
                item = ItemContainerGenerator.ContainerFromIndex(i) as GroupBarItem;

                if (!this.IsItemInToolbar(item) && !item.Hidden)
                {
                    break;
                }
            }

            return item;
        }

        /// <summary>
        /// Gets the last visible item in toolbar.
        /// </summary>
        /// <returns>
        /// The last visible item in toolbar.
        /// </returns>
        private GroupBarItem GetLastToolbarItem()
        {
            GroupBarItem item = null;

            for (int i = 0; i < Items.Count; i++)
            {
                item = ItemContainerGenerator.ContainerFromIndex(i) as GroupBarItem;
                //item = Items[i] as GroupBarItem;

                if (item != null && this.IsItemInToolbar(item) && !item.Hidden && !item.ShowInGroupBar)
                {
                    break;
                }
            }

            return item;
        }

        /// <summary>
        /// Checks whether the item with the given index is in the toolbar.
        /// </summary>
        /// <param name="item">GroupBar item.</param>
        /// <returns>
        /// Returns true if item is in toolbar
        /// </returns>
        private bool IsItemInToolbar(GroupBarItem item)
        {
            bool contains = false;
            foreach (NavigationToolbarItem toolBarItem in this.Toolbar.Items)
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
        /// Allows the collapse visibility changed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void AllowCollapseVisibilityChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            StackGrid stackgrid = (StackGrid)obj;

            if (stackgrid.collapseButtonElement != null)
            {
                if (stackgrid.AllowCollapseVisibility == Visibility.Visible)
                {
                    stackgrid.collapseButtonElement.Visibility = Visibility.Visible;
                }
                else
                {
                    if (stackgrid.LocalParent != null)
                    {
                        if (stackgrid.LocalParent.IsNavPaneMode)
                        {
                            stackgrid.CollapseNavPaneMode();
                        }
                    }
                    stackgrid.collapseButtonElement.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Shows the navigation pane text visibility changed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void ShowNavigationPaneTextVisibilityChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            StackGrid stackgrid = (StackGrid)obj;

            if (stackgrid.NavPaneText != null)
            {
                if (stackgrid.ShowNavigationPaneTextVisibility == Visibility.Visible)
                {
                    stackgrid.NavPaneText.Visibility = Visibility.Visible;
                }
                else
                {
                    stackgrid.NavPaneText.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (this.stackModeHeaderGridElement != null)
                {
                    Grid commongrid = this.ItemGrid as Grid;
                    if (Items.Count == 0)
                    {
                        if (commongrid.Children.Count > 0)
                        {
                            commongrid.Children.Clear();
                            (this.HeaderTextElement as ContentControl).Content = null;
                        }
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                for (int i = 0; i < e.OldItems.Count; i++)
                {
                    GroupBarItem item = e.OldItems[i] as GroupBarItem;
                    if (item.Content != null)
                    {
                        if ((this.stackModeHeaderGridElement as Grid).Children.Contains(item.Content as UIElement))
                        {
                            (this.stackModeHeaderGridElement as Grid).Children.Remove(item.Content as UIElement);
                            (this.HeaderTextElement as ContentControl).Content = null;
                        }

                    }
                    int index = this.Items.IndexOf(item);
                    GroupBarItem itemtobeselected = null;
                    (this.stackModeGrid as Grid).Children.Remove(item as UIElement);
                    if (index != Items.Count - 1)
                    {
                        itemtobeselected = this.Items[index + 1] as GroupBarItem;
                    }
                    if (itemtobeselected != null)
                    {
                        if (LocalParent.VisualMode == VisualMode.StackMode)
                        {
                            itemtobeselected.IsExpanded = false;
                        }
                        LocalParent.SelectedItem = itemtobeselected;
                    }
                }
            }

        #endregion
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dpo"></param>
    /// <param name="args"></param>
    public delegate void GroupBarSelectionChangedEventHandler(DependencyObject dpo, GroupBarSelectionChangedEventArgs args);

    /// <summary>
    /// 
    /// </summary>
    public class GroupBarSelectionChangedEventArgs: EventArgs
    {
        private int selectedIndex;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public GroupBarSelectionChangedEventArgs(int index)
        {
            selectedIndex = index;
        }

        /// <summary>
        /// Gets the Selected GroupBarItem's index
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
        }
    }
}

