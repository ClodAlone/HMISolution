#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
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
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;
using Syncfusion.PivotAnalysis.Base;
using System.Collections;
using System.Windows.Controls.Primitives;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Syncfusion.Windows.Controls.PivotSchemaDesigner;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Controls.PivotGrid
{
    /// <summary>
    /// Interaction logic for PivotGridFieldList.xaml
    /// </summary>
#if SyncfusionFramework4_0 
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public partial class PivotGridFieldList : Window
    {
        #region [ Private Members ]

        private Point m_StartPoint;

        private bool m_CanDrop;

        private IntPtr retInt = IntPtr.Zero;

        private const int WM_SYSCOMMAND = 0x112;

        private HwndSource hwndSource;

        #endregion

        #region [ Private Properties ]

        internal bool IsDrag { get; set; }

        #endregion

        #region [Initialize / Finalize ]

        /// <summary>
        /// Initializes a new instance of the <see cref="PivotGridFieldList"/> class.
        /// </summary>
        /// <param name="GridControl">The grid control.</param>
        /// <param name="DragSource">The drag source.</param>
        public PivotGridFieldList(PivotGridControl GridControl, ListBox DragSource)
        {
            InitializeComponent();
            this.GridControl = GridControl;
            this.DragSource = DragSource;
            this.PivotFields = GridControl.PivotFields;
            this.PivotItemPanel.DataContext = this.PivotFields;
            this.PivotItemPanel.ItemsSource = this.PivotFields;
            this.WireEvents();
        }
             
        #endregion
        
        #region [ Public Properties ]

        internal bool IsFieldList { get; set; }

        internal Point PositionPoint { get; set; }

        internal ListBox DragSource { get; set; }
        /// <summary>
        /// Gets or sets the PivotItem collection for PivotFields
        /// </summary>
        public ObservableCollection<PivotItem> PivotFields { get; set; }

        internal string FormatText { get; set; }

        internal PivotItem DragPivotItem { get; set; }

        internal object DroppedItem { get; set; }

        internal PivotGridControl GridControl
        {
            get
            {
                return (PivotGridControl)GetValue(GridControlProperty);
            }

            set
            {
                SetValue(GridControlProperty, value);
            }
        }

        internal event RoutedEventHandler GenericDragOverEvent
        {
            add { AddHandler(GenericDragOver, value); }
            remove { RemoveHandler(GenericDragOver, value); }
        }

        internal event RoutedEventHandler GenericDropEvent
        {
            add { AddHandler(GenericDrop, value); }
            remove { RemoveHandler(GenericDrop, value); }
        }

        #endregion

        #region [ Dependency Property Implementation ]
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridFieldList.GridControl"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridFieldList.GridControl"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty GridControlProperty =
           DependencyProperty.Register("GridControl", typeof(PivotGridControl), typeof(PivotGridFieldList), new UIPropertyMetadata(null));
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridFieldList.GenericDragOverEvent"/> routed event.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridFieldList.GenericDragOverEvent"/> routed event.
        /// </returns>
   
        public static readonly RoutedEvent GenericDragOver = EventManager.RegisterRoutedEvent("GenericDragOverEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler),
            typeof(PivotGridFieldList));
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridFieldList.GenericDropEvent"/> routed event.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotGridFieldList.GenericDropEvent"/> routed event.
        /// </returns>
   
        public static readonly RoutedEvent GenericDrop = EventManager.RegisterRoutedEvent("GenericDropEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler),
         typeof(PivotGridFieldList));    

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Creates the adorner.
        /// </summary>
        internal void CreateAdorner()
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(this.PivotItemPanel);
            this.RemoveAdorner(layer);
            layer.Opacity = 0.6;
            Point point = PositionPoint;
            if (this.DragPivotItem != null)
            {
                this.FormatText = DragPivotItem.FieldHeader;
            }
            layer.Add(new FieldListAdorner(this.PivotItemPanel, point.X - 25, point.Y - 42, FormatText , this,DragPivotItem));
        }

        /// <summary>
        /// Removes the adorner.
        /// </summary>
        internal void RemoveAdorner(AdornerLayer layer)
        {
            Adorner[] adorners = layer.GetAdorners(this.PivotItemPanel);
            if (adorners != null)
            {
                foreach (var item in adorners)
                {
                    layer.Remove(item);
                }
            }
        }

        #endregion

        #region [ Events ]

        /// <summary>
        /// Handles the QueryContinueDrag event of the PivotGridFieldList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.QueryContinueDragEventArgs"/> instance containing the event data.</param>
        void PivotGridFieldList_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (!this.IsDrag)
            {
                this.RemoveAdorner(AdornerLayer.GetAdornerLayer(this.PivotItemPanel));
            }
        }

        /// <summary>
        /// Handles the PreviewDragOver event of the PivotGridFieldList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        void PivotGridFieldList_PreviewDragOver(object sender, DragEventArgs e)
        {           
            PositionPoint = e.GetPosition(this);
            this.DragPivotItem = this.GetPivotItem(e.Data) as PivotItem;  
            RaiseEvent(new RoutedEventArgs(PivotGridFieldList.GenericDragOver));           
        }

        /// <summary>
        /// Handles the PreviewMouseMove event of the PivotItemPanel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void PivotItemPanel_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            ListBox source = (ListBox)sender;
            PivotGroupingItemsControl control = (PivotGroupingItemsControl)sender;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point position = e.GetPosition((IInputElement)source);
                if (Math.Abs(position.X - m_StartPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - m_StartPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    ToggleButton btn = e.OriginalSource as ToggleButton;
                    object _draggedData = PivotSchemaDesigner.PivotSchemaDesigner.GetDataFromListBox(this.DragSource, position);
                    this.GridControl.GroupingBar.DraggedItem = _draggedData;
                    if (btn != null)
                    {
                        if (m_CanDrop)
                        {
                            m_CanDrop = false;
                            this.GridControl.GroupingBar.shouldShowBouncingArrows = true;
                            if (_draggedData is PivotItem)
                            {
                                PivotItem item = _draggedData as PivotItem;
                                if (!item.AllowRunTimeGroupByField)
                                {
                                    this.GridControl.GroupingBar.shouldShowBouncingArrows = false;
                                }
                            }
                            DragDrop.DoDragDrop(source, btn.Tag, DragDropEffects.Move);
                        }
                    }
                }
            }
        }        

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event of the PivotItemPanel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void PivotItemPanel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.GridControl.GroupingBar.shouldShowBouncingArrows = true;
            m_StartPoint = e.GetPosition(DragSource);
            m_CanDrop = true;
            this.GridControl.GroupingBar.DragSource = this.PivotItemPanel;
            this.IsFieldList = true;
        }

        /// <summary>
        /// Handles the PreviewDrop event of the PivotItemPanel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DragEventArgs"/> instance containing the event data.</param>
        void PivotItemPanel_PreviewDrop(object sender, DragEventArgs e)
        {
            if (this.GridControl.GroupingBar.shouldShowBouncingArrows)
            {
                this.GridControl.GroupingBar.shouldShowBouncingArrows = true;
                this.DroppedItem = this.GetPivotItem(e.Data);
                RaiseEvent(new RoutedEventArgs(PivotGridFieldList.GenericDrop));
                this.DisableControls();
            }
        }

        /// <summary>
        /// Gets the pivot item.
        /// </summary>
        /// <param name="iDataObject">The i data object.</param>
        /// <returns></returns>
        public object GetPivotItem(IDataObject iDataObject)
        {
            object pivotItem = null;
            if (iDataObject.GetDataPresent(typeof(PivotItem)))
            {
               return pivotItem = iDataObject.GetData(typeof(PivotItem)) as PivotItem;
            }
            else if (iDataObject.GetDataPresent(typeof(FilterItemsCollection)))
            {
                pivotItem = iDataObject.GetData(typeof(FilterItemsCollection)) as FilterItemsCollection;
                this.GridControl.GroupingBar.RemoveFilterItem(pivotItem as FilterItemsCollection);
                return pivotItem;
            }
            else if (iDataObject.GetDataPresent(typeof(PivotComputationInfo)))
            {
                return pivotItem = iDataObject.GetData(typeof(PivotComputationInfo)) as PivotComputationInfo;                
            }

            return null;
        }

        /// <summary>
        /// Handles the PreviewMouseDown event of the Border control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        /// <summary>
        /// Handles the GenericDragOverEvent event of the PivotGridFieldList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void PivotGridFieldList_GenericDragOverEvent(object sender, RoutedEventArgs e)
        {
            ListBox dropTarget = sender as ListBox;          
            this.CreateAdorner();       
        }

        void PivotGridFieldList_GenericDropEvent(object sender, RoutedEventArgs e)
        {
            if (this.DroppedItem != null)
            {
                if (!this.PivotFields.Contains(this.DroppedItem as PivotItem))
                {
                    this.GridControl.PivotFields.Add(this.GetPivotItem(this.DroppedItem));
                    //this.PivotFields.Add(DroppedItem);
                    if (this.DragSource != null && this.DragSource.ItemsSource != null)
                    {
                        ((IList)this.DragSource.ItemsSource).Remove(DroppedItem);
                    }

                    if (this.PivotItemPanel.ItemTemplate == null)
                    {
                        this.PivotItemPanel.ItemTemplate = this.Resources["PivotItemTemplate"] as DataTemplate;
                    }

                    this.PivotItemPanel.ItemsSource = null;//.Clear();
                    this.PivotItemPanel.DataContext = this.PivotFields;
                    this.PivotItemPanel.ItemsSource = this.PivotFields;
                }
            }

            this.GridControl.GroupingBar.AddEmptyItem(this.DragSource);

            this.RemoveAdorner(AdornerLayer.GetAdornerLayer(this.PivotItemPanel));
        }

        /// <summary>
        /// Handles the Loaded event of the PivotGridFieldList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void PivotGridFieldList_Loaded(object sender, RoutedEventArgs e)
        {
            this.GridControl.ShowFieldList = true;
        }

        /// <summary>
        /// Handles the Closed event of the PivotGridFieldList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void PivotGridFieldList_Closed(object sender, EventArgs e)
        {
            this.GridControl.ShowFieldList = false;
        }

        /// <summary>
        /// Resets the cursor.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void ResetCursor(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Initializes the window source.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void InitializeWindowSource(object sender, EventArgs e)
        {
            hwndSource = PresentationSource.FromVisual((Visual)this) as HwndSource;
            hwndSource.AddHook(new HwndSourceHook(WndProc));
        }

        /// <summary>
        /// Displays the resize cursor.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void DisplayResizeCursor(object sender, MouseEventArgs e)
        {
            Rectangle rectangle = sender as Rectangle;

            if (rectangle != null)
            {
                switch (rectangle.Name)
                {
                    case "top":
                        this.Cursor = Cursors.SizeNS;
                        break;
                    case "topleft":
                        this.Cursor = Cursors.SizeNWSE;
                        break;
                    case "topright":
                        this.Cursor = Cursors.SizeNESW;
                        break;
                    case "bottomleft":
                        this.Cursor = Cursors.SizeNESW;
                        break;
                    case "bottomright":
                        this.Cursor = Cursors.SizeNWSE;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Border border = sender as Border;
                switch (border.Name)
                {

                    case "bottom":
                        this.Cursor = Cursors.SizeNS;
                        break;
                    case "left":
                        this.Cursor = Cursors.SizeWE;
                        break;
                    case "right":
                        this.Cursor = Cursors.SizeWE;
                        break;
                }
            }
        }

        /// <summary>
        /// Resizes the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void Resize(object sender, MouseButtonEventArgs e)
        {
            Rectangle clickedRectangle = sender as Rectangle;

            if (clickedRectangle != null)
            {
                switch (clickedRectangle.Name)
                {
                    case "top":
                        this.Cursor = Cursors.SizeNS;
                        ResizeWindow(ResizeDirection.Top);
                        break;
                    case "topleft":
                        this.Cursor = Cursors.SizeNWSE;
                        ResizeWindow(ResizeDirection.TopLeft);
                        break;
                    case "topright":
                        this.Cursor = Cursors.SizeNESW;
                        ResizeWindow(ResizeDirection.TopRight);
                        break;
                    case "bottomleft":
                        this.Cursor = Cursors.SizeNESW;
                        ResizeWindow(ResizeDirection.BottomLeft);
                        break;
                    case "bottomright":
                        this.Cursor = Cursors.SizeNWSE;
                        ResizeWindow(ResizeDirection.BottomRight);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Border border = sender as Border;
                if (border != null)
                {
                    switch (border.Name)
                    {
                        case "left":
                            this.Cursor = Cursors.SizeWE;
                            ResizeWindow(ResizeDirection.Left);
                            break;
                        case "right":
                            this.Cursor = Cursors.SizeWE;
                            ResizeWindow(ResizeDirection.Right);
                            break;

                        case "bottom":
                            this.Cursor = Cursors.SizeNS;
                            ResizeWindow(ResizeDirection.Bottom);
                            break;
                    }
                }
            }
        }

#endregion

        #region [ Private Methods ]

        private void WireEvents()
        {
            this.PreviewDragOver += new DragEventHandler(PivotGridFieldList_PreviewDragOver);

            this.QueryContinueDrag += new QueryContinueDragEventHandler(PivotGridFieldList_QueryContinueDrag);

            this.PivotItemPanel.PreviewMouseMove += new MouseEventHandler(PivotItemPanel_PreviewMouseMove);
            this.PivotItemPanel.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(PivotItemPanel_PreviewMouseLeftButtonDown);
            this.PivotItemPanel.PreviewDrop += new DragEventHandler(PivotItemPanel_PreviewDrop);

            this.SourceInitialized += new EventHandler(InitializeWindowSource);
            this.GenericDragOverEvent += new RoutedEventHandler(PivotGridFieldList_GenericDragOverEvent);
            this.GenericDropEvent += new RoutedEventHandler(PivotGridFieldList_GenericDropEvent);
            this.Loaded += new RoutedEventHandler(PivotGridFieldList_Loaded);
            this.Closed += new EventHandler(PivotGridFieldList_Closed);
            this.GridControl.ShowDisabledGroupBackgroundPropertyChanged += new EventHandler(GridControl_ShowDisabledGroupBackgroundPropertyChanged);
        }

        void GridControl_ShowDisabledGroupBackgroundPropertyChanged(object sender, EventArgs e)
        {
            this.DisableControls();
        }
   
        private PivotItem GetPivotItem(object item)
        {
            if (item is PivotItem)
            {
                return item as PivotItem;
            }
            else if (item is FilterItemsCollection)
            {
                PivotItem pivotitem = this.GridControl.GroupingBar.GetPivotItem(item as FilterItemsCollection);
                return pivotitem;
            }
            else if (item is PivotComputationInfo)
            {
                PivotComputationInfo compInfo = item as PivotComputationInfo;
                //((IList)this.FieldList.DragSource.ItemsSource).Remove(compInfo);
                PivotItem pivotitem = new PivotItem() { FieldHeader = compInfo.FieldHeader, Summary=compInfo.Summary, SummaryType=compInfo.SummaryType, FieldMappingName = compInfo.FieldName, Format=compInfo.Format, TotalHeader = "Total", AllowRunTimeGroupByField = compInfo.AllowRunTimeGroupByField };
                return pivotitem;
            }

            return null;
        }
                     
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            return IntPtr.Zero;
        }      

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Resizes the window.
        /// </summary>
        /// <param name="direction">The direction.</param>
        private void ResizeWindow(ResizeDirection direction)
        {
            SendMessage(hwndSource.Handle, WM_SYSCOMMAND, (IntPtr)(61440 + direction), IntPtr.Zero);
        }   

        #endregion

        #region [ Public Properties ]

        internal enum ResizeDirection
        {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8,
        }

        #endregion
        /// <summary>
        /// An overridden method to arrange and size the content of a <see cref="T:Syncfusion.Windows.Controls.PivotGrid.PivotGridFieldList"/> object.
        /// </summary>
        /// <param name="arrangeBounds">computed size that is used to arrange the content.</param>
        /// <returns>Size of the control.</returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            this.DisableControls();
            return base.ArrangeOverride(arrangeBounds);
        }
        private void DisableControls()
        {
            for (int i = 0; i < this.PivotItemPanel.Items.Count; i++)
            {
                ListBoxItem item = this.PivotItemPanel.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                PivotGridGroupingBar.SetDisabled(item,this.GridControl);
            }
        }
    }
}
