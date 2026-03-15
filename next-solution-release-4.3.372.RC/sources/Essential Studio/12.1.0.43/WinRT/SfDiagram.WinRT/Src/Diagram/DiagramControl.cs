#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.IO;
using System.Windows;
using Syncfusion.UI.Xaml.Diagram.Layout;
using Syncfusion.UI.Xaml.Diagram.Layout.Base;
using Syncfusion.UI.Xaml.Diagram.Panels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Syncfusion.UI.Xaml.Diagram.Serializer;
using Syncfusion.UI.Xaml.Diagram.Utility;
#if WINRT_USING
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Core;
using System.Threading.Tasks;
#else
using System.Windows.Controls; 
using System.Windows.Data;
using PressedEventArgs = System.Windows.Input.MouseButtonEventArgs;
using CoreDispatcher = System.Windows.Threading.Dispatcher;
using PointerRoutedEventArgs = System.Windows.Input.MouseButtonEventArgs;
#endif
using System.Reflection;
using Syncfusion.UI.Xaml.Diagram.Controller;
using Syncfusion.UI.Xaml.Diagram.Controls;
using System.ComponentModel;


namespace Syncfusion.UI.Xaml.Diagram
{
    public delegate IEnumerable<Type> GetTypes();

    public partial class SfDiagram : Control, IGraph, INotifyPropertyChanged
    {
        //private readonly SharedData _mSharedData;
        private IGraphInternal _graph;
        private NodePort _dragOverPort;
        private Node _dragOverNode;
        internal static CoreDispatcher Disp;

#if WPF
        static SfDiagram()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SfDiagram), new FrameworkPropertyMetadata(typeof(SfDiagram)));
        } 
#endif

        public SfDiagram()
        {
            if (!this.IsDesignMode())
            {
                Disp = this.Dispatcher;
            }
#if !WPF
            this.DefaultStyleKey = typeof(SfDiagram); 
#endif
            this.Page = new DiagramPage();
            //this.ViewDictionary = new DataTemplateDictionary();
            _graph = new SfDiagramWrapper(null, this);
            string str= "<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" +
                                     "<Rectangle Stroke=\"#FFD20C0C\" StrokeThickness=\"1\" ></Rectangle>" +
                                      "</DataTemplate>";
            _mconnectionIndicatorTemplate = str.LoadXaml() as DataTemplate;           
            SetBinding(DCProperty, new Binding());

#if SyncfusionFramework4_5_1 && WINRT
            PrintingService = new PrintingService();
#endif
            //this.Page = _graph.Page;

            /*SharedData shared = new SharedData(new SfDiagramWrapper(null),new DataTemplateViewController<IView>(this.ViewDictionary));
            shared.Graph.SetSharedData(shared);

            _mSharedData.Graph.CanRelate = true;
            _mSharedData.Graph.CanVirtualize = false;
            ViewportChangedEvent mViewportChanged = _mSharedData.EventAggregator.GetEvent<ViewportChangedEvent>();
            SnappingPropertyChanged<SnapSettings> _mSnapSettingsChangedEvent = _mSharedData.EventAggregator.GetEvent<SnappingPropertyChanged<SnapSettings>>();
            _mSnapSettingsChangedEvent.Subscribe(SnapSettingsChanged, root: true);
            mViewportChanged.Subscribe(ViewportChanged, root: true);

            var selector = new SelectorViewModel() {MinWidth = 10, MinHeight = 10};
            SelectedItems = selector;
            selector.Nodes = new ObservableCollection<object>();
            selector.Connectors = new ObservableCollection<object>();
            selector.Groups = new ObservableCollection<object>();

            (this.ViewDictionary as ISharedData).Init(_mSharedData);
            (this.Page as DiagramPage).SetSharedData(_mSharedData);

            this.SnapSettings = new SnapSettings();
            //this.SnapSettings.PropertyChanged += SnapSettings_PropertyChanged;
            this.SnapSettings.HorizontalGridlines.PropertyChanged += Gridlines_PropertyChanged;
            this.SnapSettings.VerticalGridlines.PropertyChanged += Gridlines_PropertyChanged;
            
            _mSharedData.EventAggregator.GetEvent<DrawStartedEvent>().Subscribe(StartDraw);
            _mSharedData.Graph.CanBridge = false;
            */
            this.Loaded += DiagramLoadedEvent;
#if WINRT
            this.PointerPressed += SfDiagram_PointerPressed; 
#else
            this.MouseLeftButtonDown += SfDiagram_PointerPressed;
#endif
            PageSettings = new PageSettings();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        void SfDiagram_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            IGraphInternal graph = _graph;
            if (graph.CurrentEditor != null)
            {
                DependencyObject source = e.OriginalSource as DependencyObject;
                if (source != null && source.FindVisualParent<AnnotationEditor>() == null)
                {
                    graph.CurrentEditor = null;
                }
            }
        }

        public object Info
        {
            get { return _info; }
            set
            {
                if (_info != value)
                {
                    _info = value;
                    OnPropertyChanged("Info");
                }
            }
        }

        private static readonly DependencyProperty DCProperty =
            DependencyProperty.Register("DC", typeof(object), typeof(SfDiagram), new PropertyMetadata(null, OnDataContextChanged));

        internal static void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IGraph g = e.NewValue as IGraph;
            if (g != null)
            {
                g.Info = (d as SfDiagram)._graph;
            }
        }
        
        public void Save(Stream stream)
        {
            _graph.SharedData.Serializer.DataContractSerializer(stream);
        }

        public void Load(Stream stream)
        {
            _graph.SharedData.Serializer.DataContractDeSerializer(stream);
        }

#if SyncfusionFramework4_5_1 && WINRT
        /// <summary>
        /// Used to Export the Sfdiagram as Image -Multiple Pages
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="rowCount"></param>
        /// <param name="columnCount"></param>
        /// <returns></returns>
        public async Task Export(int rowCount, int columnCount)
        {
            await _graph.Export(rowCount, columnCount);
        }

        /// <summary>
        /// Used to Export the SfDiagram as Image-Single Page
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public async Task Export()
        {
            await _graph.Export();
        }
#endif

#if !SILVERLIGHT
        public void Upgrade(Stream stream)
        {
            _graph.SharedData.Serializer.Upgrade(stream);
        } 
#endif

        private void DiagramLoadedEvent(object sender, RoutedEventArgs e)
        {
            if (LayoutManager != null)
            {
                LayoutManager.Layout.UpdateLayout();
            }
            _graph.SharedData.SpatialSearch.UpdatePageBounds();
        }

        private void DoApplyTemplate()
        {
            base.OnApplyTemplate();

#if SyncfusionFramework4_5_1 && WINRT
            _graph.SharedData.PrintandExportController.PrintContainer = GetTemplateChild("InVisibleCanvas") as Canvas;
         
#endif
        }

        public Panel Page
        {
            get { return (Panel)GetValue(PageProperty); }
            private set { SetValue(PageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Page.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageProperty =
            DependencyProperty.Register("Page", typeof(Panel), typeof(SfDiagram), new PropertyMetadata(null));

        internal object getNewItem(ElementType itemType, Type desiredType)
        {
            switch (itemType)
            {
                case ElementType.Group:
                    return GetNewGroup(desiredType);
                case ElementType.Node:
                    return GetNewNode(desiredType);
                case ElementType.Connector:
                    return GetNewConnector(desiredType);
            }
            return null;
        }
        internal void prepareElementForItemOverride(IView element, object item)
        {
            object tempITem = item;
            if (element is ISelectorView)
            {
                PrepareSelectorForItemOverride(element as Selector, tempITem);
            }
            else if (element is IGroupView)
            {
                PrepareGroupForItemOverride(element as Group, tempITem);
            }
            else if (element is IConnectorView)
            {
                PrepareConnectorForItemOverride(element as Connector, tempITem);
            }
            else if (element is INodeView)
            {
                PrepareNodeForItemOverride(element as Node, tempITem);
            }
        }
        internal void clearElementForItemOverride(IView element, object item)
        {
            object tempITem = item;
            if (element is ISelectorView)
            {
                ClearSelectorForItemOverride(element as Selector, tempITem);
            }
            else if (element is IGroupView)
            {
                ClearGroupForItemOverride(element as Group, tempITem);
            }
            else if (element is INodeView)
            {
                ClearNodeForItemOverride(element as Node, tempITem);
            }
            else if (element is IConnectorView)
            {
                ClearConnectorForItemOverride(element as Connector, tempITem);
            }
        }
        internal IView getViewForItemOverride(IInternalGroupable item)
        {
            IView element = null;
            object tempSrc = item.Source;
            if (item is IInternalSelector)
            {
                element = GetSelectorForItemOverride(tempSrc);
            }
            else if (item is IInternalGroup)
            {
                element = GetGroupForItemOverride(tempSrc);
            }
            else if (item is IInternalConnector)
            {
                element = GetConnectorForItemOverride(tempSrc);
            }
            else if (item is IInternalNode)
            {
                element = GetNodeForItemOverride(tempSrc);
            }
            return element;
        }

        protected virtual object GetNewNode(Type desiredType)
        {
            var ctor = desiredType.GetTypeInfo().GetConstructors().Any(c => !c.GetParameters().Any());
            if (!ctor)
            {
                if (desiredType.GetTypeInfo().IsAssignableFrom(typeof(INode).GetTypeInfo()))
                {
                    return new NodeViewModel();
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            else
            {
                return Activator.CreateInstance(desiredType);
            }
        }

        protected virtual object GetNewConnector(Type desiredType)
        {
            var ctor = desiredType.GetTypeInfo().GetConstructors().Any(c => !c.GetParameters().Any());
            if (!ctor)
            {
                if (desiredType.GetTypeInfo().IsAssignableFrom(typeof(IConnector).GetTypeInfo()))
                {
                    return new ConnectorViewModel();
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            else
            {
                return Activator.CreateInstance(desiredType);
            }
        }

        protected virtual object GetNewGroup(Type desiredType)
        {
            var ctor = desiredType.GetTypeInfo().GetConstructors().Any(c => !c.GetParameters().Any());
            if (!ctor)
            {
                if (desiredType.GetTypeInfo().IsAssignableFrom(typeof(IGroup).GetTypeInfo()))
                {
                    return new GroupViewModel();
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            else
            {
                return Activator.CreateInstance(desiredType);
            }
        }

        protected virtual Node GetNodeForItemOverride(object item)
        {
            return new Node();
        }

        protected virtual Connector GetConnectorForItemOverride(object item)
        {
            return new Connector();
        }

        protected virtual Group GetGroupForItemOverride(object item)
        {
            return new Group();
        }

        protected virtual Selector GetSelectorForItemOverride(object item)
        {
            Selector selector = new Selector();
            selector.Visibility = Visibility.Collapsed;
            Canvas.SetZIndex(selector, 1000000);
            return selector;
        }
        
        protected virtual void PrepareNodeForItemOverride(Node element, object item)
        {
            element.DataContext = item;
        }

        protected virtual void ClearNodeForItemOverride(Node element, object item)
        {
            element.DataContext = null;
        }

        protected virtual void PrepareConnectorForItemOverride(Connector element, object item)
        {
            element.DataContext = item;
        }

        protected virtual void ClearConnectorForItemOverride(Connector element, object item)
        {
            element.DataContext = null;
        }

        protected virtual void PrepareGroupForItemOverride(Group element, object item)
        {
            element.DataContext = item;
        }

        protected virtual void ClearGroupForItemOverride(Group element, object item)
        {
            element.DataContext = null;
        }

        protected virtual void PrepareSelectorForItemOverride(Selector element, object item)
        {
            element.DataContext = item;
        }

        protected virtual void ClearSelectorForItemOverride(Selector element, object item)
        {
            element.DataContext = null;
        }
        
        #region DependencyPropertyChanged
        
        //private void OnScrollInfoChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    OnPropertyChanged(SfDiagramConstants.ScrollInfo);
        //}

        //private void OnPageSettingsChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    OnPropertyChanged(SfDiagramConstants.PageSettings);
        //}

        //private void OnCommandsChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    OnPropertyChanged(SfDiagramConstants.Commands);
        //}

        private void OnLayoutManagerChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(SfDiagramConstants.LayoutManager);
        }

        private void OnNodesChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(SfDiagramConstants.Nodes);
        }

        private void OnConnectorsChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(SfDiagramConstants.Connectors);
        }

        private void OnGroupsChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(SfDiagramConstants.Groups);
        }
        
        private void OnSelectedItemsChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(SfDiagramConstants.SelectedItems);
        }
        
        private void OnViewDictionaryChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(SfDiagramConstants.ViewDictionary);
        }

        private void OnDefaultConnectorTypeChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(SfDiagramConstants.DefaultConnectorType);
        }

        private void OnSnapSettingsChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(SfDiagramConstants.SnapSettings);
        }

        private void OnPageSettingsChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(SfDiagramConstants.PageSettings);
        }

#if SyncfusionFramework4_5_1 && WINRT
        private void OnExportSettingsChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(SfDiagramConstants.ExportSettings);            
        }

        private void OnPrintingServiceChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(SfDiagramConstants.PrintingService);     
        }
#endif
        private void OnConstraintsChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(SfDiagramConstants.Constraints);
        }

        private void OnBezierSmoothnessChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(SfDiagramConstants.Constraints);
        }

        private void OnMultipleSelectionModeChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(SfDiagramConstants.MultipleSelectionMode);
        }

        private void OnToolChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(SfDiagramConstants.Tool);
        }

        private DataTemplate _mconnectionIndicatorTemplate = null;
        private object _info;

        private void OnDrawingToolChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(SfDiagramConstants.DrawingTool);
        }

        private void OnKnownTypesChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(SfDiagramConstants.KnownTypes);
        }

        private void OnHorizontalRulerChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(SfDiagramConstants.HorizontalRuler);

            Ruler r = (Ruler)e.NewValue;
            if (r != null)
            {
                if (_graph.SharedData.PageSettingsController != null)
                {
                    r.PrepareUnit(_graph.SharedData.PageSettingsController.Unit);
                }
                if (_graph.SharedData.ScrollViewer != null)
                {
                    r.PrepareRuler(_graph.SharedData.ScrollViewer);
                }
            }
        }

        private void OnVerticalRulerChanged(DependencyPropertyChangedEventArgs e)
        {
            OnPropertyChanged(SfDiagramConstants.VerticalRuler);

            Ruler r = (Ruler)e.NewValue;
            if (r != null)
            {
                if (_graph.SharedData.PageSettingsController != null)
                {
                    r.PrepareUnit(_graph.SharedData.PageSettingsController.Unit);
                }
                if (_graph.SharedData.ScrollViewer != null)
                {
                    r.PrepareRuler(_graph.SharedData.ScrollViewer);
                }
            }
        }
        #endregion
        
        public NodePort DragOverPort
        {
            get { return _dragOverPort; }
            set
            {
                if (_dragOverPort != null)
                {
                    _dragOverPort.IsConnecting = false;
                }
                _dragOverPort = value;
                if (_dragOverPort != null)
                {
                    _dragOverPort.IsConnecting = true;
                }
            }
        }

        public Node DragOverNode
        {
            get { return _dragOverNode; }
            set
            {
                if (_dragOverNode != null)
                {
                    _dragOverNode.IsConnecting = false;
                }
                _dragOverNode = value;
                if (_dragOverNode != null)
                {
                    _dragOverNode.IsConnecting = true;
                }
            }
        }

        protected internal virtual DataTemplate GetConnectionIndicator(object target)
        {
            return _mconnectionIndicatorTemplate;
        }
        
    }
}
