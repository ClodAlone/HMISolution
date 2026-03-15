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
using Windows.UI.Input.Inking;
using Windows.UI.Input;
using System.Threading.Tasks;
#else
using System.Windows.Controls; 
using PressedEventArgs = System.Windows.Input.MouseButtonEventArgs;
using CoreDispatcher = System.Windows.Threading.Dispatcher;
using PointerRoutedEventArgs = System.Windows.Input.MouseButtonEventArgs;
#endif
using System.Reflection;
using Syncfusion.UI.Xaml.Diagram.Controller;
using Syncfusion.UI.Xaml.Diagram.Controls;


namespace Syncfusion.UI.Xaml.Diagram
{
    internal partial class SfDiagramWrapper : DiagramElementWrapper, IGraphInternal
    {
        private readonly SfDiagram _graphView;
        public new SharedData SharedData
        {
            get { return base.SharedData; }
        }
        //internal static CoreDispatcher Disp;
        object _sourceNode = null;
# if WINRT
        InkManager _inkManager = null;
        Windows.UI.Xaml.Shapes.Path _child = null;
        uint _penID;
        INodePort _sourcePort = null;
        Point? _prevPoint = null;
        Point? _SourcePoint = null;
#endif
        public SfDiagramWrapper(SharedData shared, SfDiagram view) : base(shared)
        {
            _graphView = view;
            this.Page = _graphView.Page;
            base.SharedData = new SharedData(this);
            Source = view;
            Info = this;
            _graphView.ViewDictionary = SharedData.ViewDictionary; 

            SharedData.Graph.CanRelate = true;
            SharedData.Graph.CanVirtualize = false;
            ViewportChangedEvent mViewportChanged = SharedData.EventAggregator.GetEvent<ViewportChangedEvent>();
            mViewportChanged.Subscribe(ViewportChanged, root: true);

            SelectedItems = new SelectorViewModel() {MinWidth = 10, MinHeight = 10};

            (_graphView.ViewDictionary as ISharedData).Init(SharedData);
            (this.Page as DiagramPage).SetSharedData(SharedData);

            this.SnapSettings = new SnapSettings();
            
            SharedData.EventAggregator.GetEvent<DrawStartedEvent>().Subscribe(StartDraw);
#if WINRT
            SharedData.EventAggregator.GetEvent<DrawingCompletedEvent>().Subscribe(EndDraw);
# endif
            SharedData.Graph.CanBridge = false;
        }
        
        public object Info
        {
            get { throw new NotImplementedException(); }
            set
            {
                if (Source is IGraph)
                {
                    (Source as IGraph).Info = value;
                }
            }
        }

        public IScrollInfo ScrollInfo { get; private set; }
        public IDiagramCommands Commands { get; private set; }

        protected override void OnPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case SfDiagramConstants.Connectors:
                    OnConnectorsChanged();
                    break;
                case SfDiagramConstants.Constraints:
                    OnConstraintsChanged();
                    break;
                case SfDiagramConstants.DefaultConnectorType:
                    break;
                case SfDiagramConstants.DrawingTool:
                    break;
                case SfDiagramConstants.Groups:
                    OnGroupsChanged();
                    break;
                case SfDiagramConstants.InternalSelectedItems:
                    break;
                case SfDiagramConstants.KnownTypes:
                    break;
                case SfDiagramConstants.LayoutManager:
                    OnLayoutManagerChanged();
                    break;
                case SfDiagramConstants.MultipleSelectionMode:
                    break;
                case SfDiagramConstants.Nodes:
                    OnNodesChanged();
                    break;
                //case SfDiagramConstants.Commands:
                //    break;
                //case SfDiagramConstants.PageSettings:
                //    break;
                //case SfDiagramConstants.ScrollInfo:
                //    break;
                case SfDiagramConstants.SelectedItems:
                    OnSelectedItemsChanged();
                    break;
                case SfDiagramConstants.SnapSettings:
                    OnSnapSettingsChanged();
                    break;
                case SfDiagramConstants.Tool:
                    break;
                case SfDiagramConstants.ViewDictionary:
                    break;
                case SfDiagramConstants.Viewport:
                    OnViewportChanged();
                    break;
                case SfDiagramConstants.PageSettings:
                    OnPageSettingsChanged();
                    break;
#if SyncfusionFramework4_5_1 && WINRT
                case SfDiagramConstants.PrintingService:
                    OnPrintTicketChanged();
                    break;

                case SfDiagramConstants.ExportSettings:
                    OnExportSettingsChanged();
                    break;
#endif
            }
            //throw new NotImplementedException();
        }

        protected override void SharedDataInitialized()
        {
        }

        protected override void SourceChanged()
        {
        }
      
        void SfDiagram_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            IGraphInternal graph = this;
            if (graph.CurrentEditor != null)
            {
                DependencyObject source = e.OriginalSource as DependencyObject;
                if (source != null && source.FindVisualParent<AnnotationEditor>() == null)
                {
                    graph.CurrentEditor = null;
                }
            }
        }

        public void Save(Stream stream)
        {
            SharedData.Serializer.DataContractSerializer(stream);
        }

        public void Load(Stream stream)
        {
            SharedData.Serializer.DataContractDeSerializer(stream);
        }

#if !SILVERLIGHT
        private void Upgrade(Stream stream)
        {
            SharedData.Serializer.Upgrade(stream);
        } 
#endif

        //public void StartDraw(IDrawCommandParameter param)
        //{
        //    //DrawParameter startArgs = new DrawParameter(param.Tool,
        //    //    param.PressedEventArgs,
        //    //    param.Point, 
        //    //    param.Node, 
        //    //    param.Port,
        //    //    param.NullSourceTarget);
        //    StartDraw(param);
        //}

        public void StartDraw(IDrawParameter args)
        {
            object sourceNode = args.Node;
            if (sourceNode == null &&
                (args.NullSourceTarget & NullSourceTarget.SelectionAsSource) == NullSourceTarget.SelectionAsSource)
            {
                if (InternalSelectedItems.InternalNodes != null && InternalSelectedItems.InternalNodes.Any())
                {
                    sourceNode = InternalSelectedItems.InternalNodes.First().Source;
                }
            }
            _sourceNode = sourceNode;
            //object obj = new List<DrawParameter>();
            if (!Tool.Contains(Tool.DrawOnce) && !Tool.Contains(Tool.ContinuesDraw))
            {
                Tool |= Tool.DrawOnce;
            }
            if (SharedData.UndoRedoController != null) SharedData.UndoRedoController.BeginComposite();
            Selector selector =
                      (this as IGraphInternal).InternalSelectedItems.View as
                      Selector;
            (selector.Wrapper as IInternalSelector).ClearSelection();
#if WINRT
            if (!(DefaultConnectorType == ConnectorType.PolyCubicBezier))
#endif
            //if (Connectors is ICollection<TConnector> && !(Connectors as ICollection<TConnector>).IsReadOnly)
            {
                //(Nodes as ICollection<TNode>).Add((TNode)(object)node);
                if (_mInternalConnectors != null)
                {
                    object connector = GetNewItem(ElementType.Connector, _mInternalConnectors.ItemType);

                    IInternalConnector internalConnector = _mInternalConnectors.GetNewWrapper(connector, true);
                    internalConnector.BezierSmoothness = BezierSmoothness.SymmetricAngle;
                    _mInternalConnectors.Add(internalConnector, ItemSource.DrawingTool);
                    internalConnector.SourcePoint = args.Point ??
                                                    args.PressedEventArgs.GetCurrentPoint(SharedData.Page).Position;
                    internalConnector.TargetPoint = args.PressedEventArgs.GetCurrentPoint(SharedData.Page).Position;

                    if (sourceNode != null)
                    {
                        IInternalNode internalNode = (this as IGraphInternal).GetNodeWrapper(sourceNode, false);
                        if (internalNode.Constraints.Contains(NodeConstraints.OutConnect))
                        {
                            internalConnector.KnownSourceNode = internalNode;
                        }
                    }
                    if (args.Port != null)
                    {
                        IInternalNodePort internalPort =
                            (this as IGraphInternal).GetNodePortWrapper((INodePort)args.Port, false);
                        if (
                                (internalPort.Constraints.Contains(PortConstraints.Inherit) &&
                                internalPort.KnownNode.Constraints.Contains(NodeConstraints.OutConnect))
                                ||
                                (!(internalPort.Constraints.Contains(PortConstraints.Inherit) &&
                                internalPort.Constraints.Contains(PortConstraints.OutConnect)))
                            )
                        {
                            internalConnector.KnownSourcePort = internalPort;
                        }
                    }
                  
                    internalConnector.IsSelected = true;
                    //SharedData.VirtualizingController.Realize(internalConnector);
                    selector.UpdateLayout();
                    (selector.Wrapper as IInternalSelector).ManualPressTargetThumb(args.PressedEventArgs);
                }
            }
#if WINRT
            else
            {
                _inkManager = new InkManager();
                _sourcePort = args.Port as INodePort;
                //_sourceNode = args.Node as INode;
                _prevPoint = args.PressedEventArgs.GetCurrentPoint(SharedData.Page).Position;
                _SourcePoint = _prevPoint;
                PointerPoint p = args.PressedEventArgs.GetCurrentPoint(SharedData.Page);
                _penID = p.PointerId;
                _inkManager.ProcessPointerDown(p);
                if (Page.Children.Contains(_child))
                {
                    Page.Children.Remove(_child);
                }
                _child = new Windows.UI.Xaml.Shapes.Path();
                _child.Data = new PathGeometry() { Figures = new PathFigureCollection() };
                _child.Stroke = new SolidColorBrush(Windows.UI.Colors.Black);
                _child.StrokeThickness = 1;
                PathFigure fig = new PathFigure();
                fig.StartPoint = p.Position;
                (_child.Data as PathGeometry).Figures.Add(fig);
                Page.Children.Add(_child);
            }
# endif
        }
#if WINRT
        private object RenderStroke(InkStroke stroke, Windows.UI.Color color, double width, double opacity = 1)
        {
            object connector = GetNewItem(ElementType.Connector, _mInternalConnectors.ItemType);
            IInternalConnector internalConnector = _mInternalConnectors.GetNewWrapper(connector, true);
            _mInternalConnectors.Add(internalConnector, ItemSource.DrawingTool);
            internalConnector.BezierSmoothness = BezierSmoothness.SymmetricAngle;
            internalConnector.Constraints = internalConnector.Constraints & ~ConnectorConstraints.InheritSmoothness;
            var renderingStrokes = stroke.GetRenderingSegments();
            internalConnector.Segments = new ConnectorSegments();
            int index = 0;
            foreach (var renderStroke in renderingStrokes)
            {
                if (Distance(_SourcePoint.Value.X, _SourcePoint.Value.Y, renderStroke.Position.X, renderStroke.Position.Y) <= 2 && index == 0 && renderingStrokes.Count() > 1)
                {
                }
                else if (index == renderingStrokes.Count() - 1)
                {
                    internalConnector.Segments.Add(new CubicCurveSegment()
                    {
                        Point1 = renderStroke.BezierControlPoint1,
                        Point2 = renderStroke.BezierControlPoint2
                    });
                }
                else
                {
                    internalConnector.Segments.Add(new CubicCurveSegment()
                    {
                        Point1 = renderStroke.BezierControlPoint1,
                        Point2 = renderStroke.BezierControlPoint2,
                        Point3 = renderStroke.Position
                    });
                }
                index++;
            }        
  
            return connector;
        }


        private void EndDraw(DrawParameter args)
        {
            if (DefaultConnectorType == ConnectorType.PolyCubicBezier )
            {
                PointerPoint p = args.PressedEventArgs.GetCurrentPoint(SharedData.Page);
                if (p.PointerId == _penID)
                {
                    SharedData.Page.Children.Remove(_child);

                    if(args.Point==null)
                        return;
                    _inkManager.ProcessPointerUp(p);
                    var strokes = _inkManager.GetStrokes();
                    object connector = null;
                    foreach (var stroke in strokes)
                    {
                        connector = RenderStroke(stroke, stroke.DrawingAttributes.Color, stroke.DrawingAttributes.Size.Width);
                    }

                    if (connector != null)
                    {
                        IInternalConnector _internalcon = this.GetConnectorWrapper(connector, false);
                        _internalcon.TargetPoint = p.Position;
                        if (args.Node != null)
                        {
                            _internalcon.KnownTargetNode = this.GetNodeWrapper(args.Node, false);
                        }
                        if (args.Port != null)
                        {
                            _internalcon.KnownTargetPort = this.GetNodePortWrapper((INodePort)args.Port, false);
                        }
                        if (_sourceNode != null)
                            _internalcon.KnownSourceNode = this.GetNodeWrapper(_sourceNode, false);
                        if (_sourcePort != null)
                            _internalcon.KnownSourcePort = this.GetNodePortWrapper(_sourcePort, false);
                        _internalcon.SourcePoint = _SourcePoint.Value;

                        _internalcon.IsSelected = true;

                    }
                }
            }
            //_mSharedData.UndoRedoController.EndComposite();
        }

        double diffx, diffy;

        internal void UpdateDrawingPath(PointerRoutedEventArgs e)
        {
            PointerPoint pt = e.GetCurrentPoint(Page);
            if (pt.PointerId == _penID)
            {
                double dist = Distance(_prevPoint.Value.X, _prevPoint.Value.Y, pt.Position.X, pt.Position.Y);

                if (dist > 2)
                {
                    (_child.Data as PathGeometry).Figures[0].Segments.Add(new LineSegment() { Point = pt.Position });
                    if (Math.Abs(_prevPoint.Value.X - pt.Position.X - diffx) > 20 || Math.Abs(_prevPoint.Value.Y - pt.Position.Y - diffy) > 20)
                    {
                        _inkManager.ProcessPointerUpdate(pt);
                        diffx = _prevPoint.Value.X - pt.Position.X;
                        diffy = _prevPoint.Value.Y - pt.Position.Y;
                        _prevPoint = pt.Position;
                    }
                }
            }
        }

        private double Distance(double x1, double y1, double x2, double y2)
        {
            double d = 0;
            d = Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
            return d;
        }

# endif
        private void ViewportChanged(ChangeArgs<Rect> element)
        {
            Viewport = element.NewValue;
        }

        private void DiagramLoadedEvent(object sender, RoutedEventArgs e)
        {
            if (LayoutManager != null)
            {
                LayoutManager.Layout.UpdateLayout();
            }
        }

        public Panel Page
        {
            get; private set;
        }

        // Using a DependencyProperty as the backing store for Page.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageProperty =
            DependencyProperty.Register("Page", typeof(Panel), typeof(SfDiagram), new PropertyMetadata(null));

        public object GetKey(IInternalGroupable item, ItemType itemType)
        {
            object key = null;
            if (item.Source is IDiagramElement)
            {
                IDiagramElement node = item.Source as IDiagramElement;
                if (node.Key != null && _graphView.ViewDictionary.Contains(node.Key, itemType))
                {
                    key = node.Key;
                }
            }
            if (item.Source == null)
            {
                return null;
            }
            Type type = item.Source.GetType();
            while (key == null && type != null)
            {
                object tempCast = type;
                if (_graphView.ViewDictionary.Contains(tempCast, itemType))
                {
                    key = tempCast;
                }
                else
                {
                    type = type.GetTypeInfo().BaseType;
                }
            }
            return key;
        }
        
        public IView InternalGetViewForItemOverride(IInternalGroupable item, out bool isNew)
        {
            isNew = false;
            IView element = null;
            if (item.View == null)
            {
                ItemType type = ItemType.None;
                if (item is IGroup)
                {
                    type = ItemType.Group;
                }
                else if (item is INode)
                {
                    type = ItemType.Node;
                }
                else if (item is IConnector)
                {
                    type = ItemType.Connector;
                }

                object key = (this as IGraphInternal).GetKey(item, type);
                if (key != null)
                {
                    element = SharedData.ViewManager.Realize(key, type, out isNew);
                    //element = SharedData.ViewManager.Realize(key, out isNew);
                }

                if (element == null)
                {
                    element = _graphView.getViewForItemOverride(item);
                    isNew = true;
                }
                return element;
            }
            else
            {
                return item.View;
            }
        }

        //public bool CanVirtualize { get; set; }
        public bool CanRelate { get; set; }
        public bool CanBridge { get; set; }

        public object GetNewItem(ElementType itemType, Type desiredType)
        {
            return _graphView.getNewItem(itemType, desiredType);
        }
        
        public void PrepareElementForItemOverride(IView element, object item)
        {
            _graphView.prepareElementForItemOverride(element, item);
        }

        public void ClearElementForItemOverride(IView element, object item)
        {
            _graphView.clearElementForItemOverride(element, item);
        }
        
        #region PropertyChanged

        private ObservableElements<object, IInternalNode> _mInternalNodes;
        private ObservableElements<object, IInternalConnector> _mInternalConnectors;
        private ObservableElements<object, IInternalGroup> _mInternalGroups;

        public ObservableElements<object, IInternalNode> InternalNodes
        {
            get { return _mInternalNodes; }
        }

        public ObservableElements<object, IInternalConnector> InternalConnectors
        {
            get { return _mInternalConnectors; }
        }
        public ObservableElements<object, IInternalGroup> InternalGroups
        {
            get { return _mInternalGroups; }
        }

        private void OnPageSettingsChanged()
        {
            if (SharedData != null)
            {
                SharedData.SetPageSetting();
            }
        }

#if SyncfusionFramework4_5_1 && WINRT
        private void OnExportSettingsChanged()
        {
            SharedData.PageSettingsController.UpdateExportSettings();  
        }

        private void OnPrintTicketChanged()
        {
            if (SharedData != null)
            {
                ISharedData service = SharedData.Graph.PrintingService;
                if(service != null)
                {
                    service.Init(SharedData);
                }
            }
            //(SharedData.Graph.PrintingService as PrintingService).RegisterForPrinting();
        }
#endif

        private void OnLayoutManagerChanged()
        {
            if (LayoutManager != null)
            {
                LayoutManager.Graph = this;
            }
            //if (LayoutManager != null && LayoutManager.Layout != null)
            //{
            //    IInternalLayout layout = (e.NewValue as LayoutManager).Layout;
            //    layout.Graph = this;
            //}
            //if (e.OldValue != null)
            //{
            //    IInternalLayout layout = (e.OldValue as LayoutManager).Layout;
            //    layout.Graph = null;
            //}
        }

        private void OnNodesChanged()
        {
            if (!SharedData.NeededEvents.NeedItemAddedEvent &&
                !SharedData.NeededEvents.NeedUndoableEvent)
            {
                SharedData.EventAggregator.GetEvent<AddedEvent<CollectionArgs<IInternalNode>>>().IsSuspended = true;
            }
            IGraphInternal internalGraph = this;
            if (_mInternalNodes != null)
            {
                _mInternalNodes.Clear();
                _mInternalNodes = null;
            }
            if (Nodes != null)
            {
                _mInternalNodes =
                         new ObservableElements<object, IInternalNode>
                             (Nodes,
                              ElementType.Node,
                              SourceType.Graph,
                              SharedData.EventAggregator,
                              internalGraph.GetNodeWrapper);
            }
            SharedData.EventAggregator.GetEvent<AddedEvent<CollectionArgs<IInternalNode>>>().IsSuspended = false;
            SharedData.Page.InvalidateMeasure();
        }

        private void OnConnectorsChanged()
        {
            if (!SharedData.NeededEvents.NeedItemAddedEvent &&
                !SharedData.NeededEvents.NeedUndoableEvent)
            {
                SharedData.EventAggregator.GetEvent<AddedEvent<CollectionArgs<IInternalConnector>>>().IsSuspended =
                    true;
            }
            IGraphInternal internalGraph = this;
            if (_mInternalConnectors != null)
            {
                _mInternalConnectors.Clear();
                _mInternalConnectors = null;
            }
            if (Connectors != null)
            {
                _mInternalConnectors = 
                         new ObservableElements<object, IInternalConnector>(
                             Connectors,
                             ElementType.Connector,
                             SourceType.Graph,
                             SharedData.EventAggregator,
                             internalGraph.GetConnectorWrapper);
            }
            SharedData.EventAggregator.GetEvent<AddedEvent<CollectionArgs<IInternalConnector>>>().IsSuspended = false;
            SharedData.Page.InvalidateMeasure();
        }

        private void OnGroupsChanged()
        {
            if (!SharedData.NeededEvents.NeedItemAddedEvent &&
                !SharedData.NeededEvents.NeedUndoableEvent)
            {
                SharedData.EventAggregator.GetEvent<AddedEvent<CollectionArgs<IInternalGroup>>>().IsSuspended =
                    true;
            }
            IGraphInternal internalGraph = this;
            if (_mInternalGroups != null)
            {
                _mInternalGroups.Clear();
                _mInternalGroups = null;
            }
            if (Groups != null)
            {
                _mInternalGroups =
                    new ObservableElements<object, IInternalGroup>(
                        Groups, 
                        ElementType.Group,
                        SourceType.Graph,
                        SharedData.EventAggregator,
                        internalGraph.GetGroupWrapper);
            }
            SharedData.EventAggregator.GetEvent<AddedEvent<CollectionArgs<IInternalGroup>>>().IsSuspended = false;
            SharedData.Page.InvalidateMeasure();
        }
        
        private void OnSelectedItemsChanged()
        {
            if (InternalSelectedItems != null)
            {
                SharedData.Adorner.Children.Remove(InternalSelectedItems.View as UIElement);
                InternalSelectedItems.View = null;
                InternalSelectedItems.Dispose();
                InternalSelectedItems = null;
            }
            if (SelectedItems != null)
            {
                var wrap = new SelectorWrapper(SharedData);
                wrap.Init(SelectedItems);
                InternalSelectedItems = wrap;
                SharedData.VirtualizingController.Realize(InternalSelectedItems, SharedData.Adorner,0);
            }
            //if (newValue != null)
            //{
            //    SetValue(InternalSelectedItemsProperty,
            //        new ObservableElements<TGroup, IInternalSelector>(
            //            newValue,
            //            ElementType.Group,
            //            SourceType.SelectionList,
            //            SharedData.EventAggregartor,
            //            internalGraph.GetGroupWrapper));
            //}
        }

        private void OnViewportChanged()
        {
            ScrollChanged current = SharedData.ScrollViewer._mCurrentState;
            current.ViewPort = Viewport;
            current.CurrentZoom = SharedData.ScrollViewer.CurrentZoom;
            SharedData.ScrollViewer.InvokeViewportChangedEvent(current);
            SharedData.InvalidateState.RecalculateVirtualization = true;
            if (SharedData.Graph.SnapSettings.SnapConstraints.Contains(SnapConstraints.HorizontalLines) ||
               SharedData.Graph.SnapSettings.SnapConstraints.Contains(SnapConstraints.VerticalLines))
            {
                SharedData.SnapSettingsController.UpdateGridlines(Viewport);
            }
            if (SharedData.PageSettingsController != null)
                SharedData.PageSettingsController.UpdateBackground(Viewport);
        }
        
        private void OnInternalSelectedItemsChanged()
        {
            //if (newValue != null)
            {
                //bool isNew;
                //IView view = newValue.Source as IView;
                //if (view == null)
                //{
                //    view = SharedData.Graph.InternalGetViewForItemOverride(newValue, out isNew);
                //    SharedData.Graph.PrepareElementForItemOverride(view, newValue.Source);
                //    view.SetBusinessObject(newValue.Source);
                //    newValue.View = view;
                //    (view as UIElement).InvalidateMeasure();
                //    (view as UIElement).InvalidateArrange();
                //}
                //else
                //{
                //    newValue.View = view;
                //    isNew = true;
                //}
                //if (isNew)
                //{
                //    SharedData.Adorner.Children.Add(view as UIElement);
                //}
            }
        }

        private void OnSnapSettingsChanged()
        {
            SharedData.SetSnapSettings(SnapSettings);
           
        }

        #endregion
        
        private object lockCreation = new object();
        private AnnotationEditor _currentEditor;

        public void SetScrollInfo(IScrollInfo info)
        {
            ScrollInfo = info;
        }

        public void SetPageSettings(IPageSettings settings)
        {
            PageSettings = settings;
        }

        public void SetCommands(IDiagramCommands commands)
        {
            Commands = commands;
        }

        public IInternalNode GetNodeWrapper(object source, bool canCreate)
        {
            //lock (lockCreation)
            {
                IInternalNode wrapper = null;
                if (SharedData != null)
                {
                    //if (source is IGroupable)
                    //{
                    //    wrapper = (source as IGroupable).PrivateData as IInternalNode;
                    //}
                    //if (wrapper == null)
                    //{
                        SharedData.GlobalNodes.TryGetValue(source, out wrapper);
                    //}
                }
                if (wrapper == null && canCreate)
                {
                    var t = new NodeWrapper(SharedData);
                    t.Init(source);
                    wrapper = t;
                    SharedData.GlobalNodes.Add(source, wrapper);
                }
                return wrapper;
            }
        }

        public IInternalConnector GetConnectorWrapper(object source, bool canCreate)
        {
            IInternalConnector wrapper = null;
            if (SharedData != null)
            {
                //    if (source is IGroupable)
                //    {
                //        wrapper = (source as IGroupable).PrivateData as IInternalConnector;
                //    }
                //if (wrapper == null)
                //{
                    SharedData.GlobalConnectors.TryGetValue(source, out wrapper);
                //}
            }
            if (wrapper == null && canCreate)
            {
                var k = new ConnectorWrapper(SharedData);
                k.Init(source);
                wrapper = k;
                SharedData.GlobalConnectors.Add(source, wrapper);
            }
            return wrapper;
        }

        public IInternalGroup GetGroupWrapper(object source, bool canCreate)
        {
            IInternalGroup wrapper = null;
            if (SharedData != null)
            {
                //    if (source is IGroupable)
                //    {
                //        wrapper = (source as IGroupable).PrivateData as IInternalGroup;
                //    }
                //if (wrapper == null)
                //{
                    SharedData.GlobalGroup.TryGetValue(source, out wrapper);
                //}
            }
            if (wrapper == null && canCreate)
            {
                var k = new GroupWrapper(SharedData);
                k.Init(source);
                wrapper = k;
                SharedData.GlobalGroup.Add(source, wrapper);
            }
            return wrapper;
        }

        public IInternalNodePort GetNodePortWrapper(INodePort source, bool canCreate)
        {
            IWrapper wrapper = null;
            if (SharedData != null)
            {
                SharedData.GlobalPorts.TryGetValue(source, out wrapper);
            }
            if (wrapper == null && canCreate)
            {
                wrapper = new NodePortWrapper
                    (
                    source,
                    SharedData
                    );
                SharedData.GlobalPorts.Add(source, wrapper);
            }
            return wrapper as IInternalNodePort;
        }

        public AnnotationEditorWrapper GetAnnotationWrapper(IAnnotation source, bool canCreate)
        {
            IWrapper wrapper = null;
            if (SharedData != null)
            {
                SharedData.GlobalAnnotation.TryGetValue(source, out wrapper);
            }
            if (wrapper == null && canCreate)
            {
                wrapper = new AnnotationEditorWrapper
                    (
                    source,
                    SharedData
                    );
                SharedData.GlobalAnnotation.Add(source, wrapper);
            }
            return wrapper as AnnotationEditorWrapper;
        }

        internal DataTemplate GetConnectionIndicator(object source)
        {
            return _graphView.GetConnectionIndicator(source);
        }
       
        public AnnotationEditor CurrentEditor
        {
            get { return _currentEditor; }
            set
            {
                if (_currentEditor != null)
                    _currentEditor.Mode = ContentEditorMode.View;
                _currentEditor = value;
            }
        }
        
        private void OnConstraintsChanged()
        {
            GraphConstraints newValue = Constraints;
            if (newValue.Contains(GraphConstraints.Undoable))
            {
                if (SharedData.UndoRedoController == null)
                {
                    SharedData.SetUndoRedoController(new UndoRedoController());
                }
            }
            else
            {
                if (SharedData.UndoRedoController != null)
                {
                    SharedData.SetUndoRedoController(null);
                }
            }

            if (newValue.Contains(GraphConstraints.Virtualize))
            {
                (this as IGraphInternal).CanVirtualize = true;
            }
            else
            {
                (this as IGraphInternal).CanVirtualize = false;
            }

            if (newValue.Contains(GraphConstraints.Relationship))
            {
                (this as IGraphInternal).CanRelate = true;
            }
            else
            {
                (this as IGraphInternal).CanRelate = false;
            }


            if (newValue.Contains(GraphConstraints.Bridging))
            {
                (this as IGraphInternal).CanBridge = true;
            }
            else
            {
                (this as IGraphInternal).CanBridge = false;
            }
        }

#if SyncfusionFramework4_5_1 && WINRT
        public async Task Export(int RowCount, int ColumnCount)
        {
            await SharedData.PrintandExportController.Export(ExportSettings, RowCount, ColumnCount);
        }

        public async Task Export()
        {
            await SharedData.PrintandExportController.Export(ExportSettings);
        }
#endif
    }
}
