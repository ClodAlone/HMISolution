#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
#if WINRT_USING
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using System.Threading.Tasks; 
#else
using System.Windows.Controls; 
#endif
using Syncfusion.UI.Xaml.Diagram;
using Syncfusion.UI.Xaml.Diagram.Layout;
using Syncfusion.UI.Xaml.Diagram.Controller;
using Syncfusion.UI.Xaml.Diagram.Controls;
using System.ComponentModel;
using System.IO;

namespace Syncfusion.UI.Xaml.Diagram
{
    public interface IGraph : INotifyPropertyChanged
    {
        GraphConstraints Constraints { get; set; }
        MultipleSelectionMode MultipleSelectionMode { get; set; }
        Tool Tool { get; set; }
        DrawingTool DrawingTool { get; set; }
        object Nodes { get; set; }
        object Connectors { get; set; }
        object Groups { get; set; }
        ConnectorType DefaultConnectorType { get; set; }
        object SelectedItems { get; set; }
        LayoutManager LayoutManager { get; set; }
        GetTypes KnownTypes { get; set; }
        SnapSettings SnapSettings { get; set; }
        object Info { get; set; }
        IPageSettings PageSettings { get; set; }
        BezierSmoothness BezierSmoothness { get; set; }
        Ruler HorizontalRuler { get; set; }
        Ruler VerticalRuler { get; set; }
#if SyncfusionFramework4_5_1 && WINRT
        PrintingService PrintingService { get; set; }
        ExportSettings ExportSettings { get; set; }
#endif
    }

    public interface IGraphInfo
    {
        IScrollInfo ScrollInfo { get; }
        IDiagramCommands Commands { get; }
        Rect Viewport { get; }
        void Save(Stream stream);
        void Load(Stream stream);
#if SyncfusionFramework4_5_1 && WINRT
        Task Export(int RowCount, int ColumnCount);
        Task Export();
#endif
        #region Node
        event SelectedEventHandler ItemSelectedEvent;
        event UnSelectedEventHandler ItemUnSelectedEvent;
        event ItemTappedEventHandler ItemTappedEvent;
        event ItemDoubleTappedEventHandler ItemDoubleTappedEvent;
        event NodeChangedEventHandler NodeChangedEvent;
        //event TransformStartingEventHandler NodeTransformStartingEvent;
        //event TransformStartedEventHandler NodeTransformStartedEvent;
        //event TransformDeltaEventHandler NodeTransformDeltaEvent;
        //event TransformCompletedEventHandler NodeTransformCompletedEvent;
        //event TransformCanceledEventHandler NodeTransformCanceledEvent;

        event ItemAddedEventHandler ItemAdded;
        event ItemDeletedEventHandler ItemDeleted;

        event SelectingEventHandler ItemSelectingEvent;
        event UnSelectingEventHandler ItemUnSelectingEvent;
        event ItemDeletingEventHandler ItemDeletingEvent;
        #endregion

        #region Connector
        event ConnectorSourceChangedEventHandler ConnectorSourceChangedEvent;
        event ConnectorTargetChangedEventHandler ConnectorTargetChangedEvent;
        #endregion

        #region ScrollViewer
        event ViewPortChangedEventHandler ViewPortChangedEvent;
        event SymbolDroppingEventHandler SymbolDroppingEvent;
        #endregion 
    }

    internal interface IGraphInternal : IGraph, IGraphInfo, IWrapper
    {
       
        Panel Page { get; }
        AnnotationEditor CurrentEditor { get; set; }
        bool CanVirtualize { get; set; }
        bool CanRelate { get; set; }
        bool CanBridge { get; set; }

        ObservableElements<object, IInternalNode> InternalNodes { get; }
        ObservableElements<object, IInternalConnector> InternalConnectors { get; }
        ObservableElements<object, IInternalGroup> InternalGroups { get; }
        IInternalSelector InternalSelectedItems { get; }
        
        IInternalNode GetNodeWrapper(object source, bool canCreate);
        IInternalConnector GetConnectorWrapper(object source, bool canCreate);
        IInternalGroup GetGroupWrapper(object source, bool canCreate);
        IInternalNodePort GetNodePortWrapper(INodePort source, bool canCreate);
        AnnotationEditorWrapper GetAnnotationWrapper(IAnnotation source, bool canCreate);
        void StartDraw(IDrawParameter param);

        object GetKey(IInternalGroupable item, ItemType itemType);
        IView InternalGetViewForItemOverride(IInternalGroupable item, out bool isNew);
        void PrepareElementForItemOverride(IView element, object item);
        void ClearElementForItemOverride(IView element, object item);

        object GetNewItem(ElementType itemType, Type desiredType);

        void SetScrollInfo(IScrollInfo info);
        void SetCommands(IDiagramCommands commands);

        #region Node Events
        void OnItemSelectedEvent(DiagramEventArgs args);
        void OnItemUnSelectedEvent(DiagramEventArgs args);
        void OnItemTappedEvent(DiagramEventArgs args);
        void OnItemDoubleTappedEvent(DiagramEventArgs args);
        void OnItemAdded(ItemAddedEventArgs args);
        void OnItemDeleted(DiagramEventArgs args);

        void OnItemSelectingEvent(DiagramPreviewEventArgs args);
        void OnItemUnSelectingEvent(DiagramPreviewEventArgs args);
        void OnItemDeletingEvent(DiagramPreviewEventArgs args);
        //void OnNodeTransformStartingEvent(TransformStartingEventArgs args);
        //void OnNodeTransformStartedEvent(TransformStartedEventArgs args);
        //void OnNodeTransformDeltaEvent(TransformDeltaEventArgs args);
        //void OnNodeTransformCompletedEvent(TransformCompletedEventArgs args);
        //void OnNodeTransformCanceledEvent(TransformCanceledEventArgs args);
        void OnNodeChangedEvent(ChangeEventArgs<object, NodeChangedEventArgs> args);
        #endregion

        #region Connector Event
        void OnConnectorSourceChangedEvent(ChangeEventArgs<object, ConnectorChangedEventArgs> args);
        void OnConnectorTargetChangedEvent(ChangeEventArgs<object, ConnectorChangedEventArgs> args);
        //void OnConnectorSelectedEvent(DiagramEventArgs args);
        //void OnConnectorUnSelectedEvent(DiagramEventArgs args);
        //void OnConnectorTappedEvent(DiagramEventArgs args);
        //void OnConnectorDoubleTappedEvent(DiagramEventArgs args);
        //void OnConnectorAdded(DiagramEventArgs args);
        //void OnConnectorDeleted(DiagramEventArgs args); 
        #endregion

        #region ScrollViewer
        void OnViewPortChangedEvent(ChangeEventArgs<object, ScrollChanged> args);
        #endregion
        //#region Group Event
        //void OnGroupSelectedEvent(DiagramEventArgs args);
        //void OnGroupUnSelectedEvent(DiagramEventArgs args);
        //void OnGroupTappedEvent(DiagramEventArgs args);
        //void OnGroupDoubleTappedEvent(DiagramEventArgs args);
        //void OnGroupAdded(DiagramEventArgs args);
        //void OnGroupDeleted(DiagramEventArgs args);

        //void OnGroupTransformStartingEvent(TransformStartingEventArgs args);
        //void OnGroupTransformStartedEvent(TransformStartedEventArgs args);
        //void OnGroupTransformDeltaEvent(TransformDeltaEventArgs args);
        //void OnGroupTransformCompletedEvent(TransformCompletedEventArgs args);
        //void OnGroupTransformCanceledEvent(TransformCanceledEventArgs args);
        //void OnGroupTransformedEvent(TransformEventArgs args);
        //#endregion

        //void PrepareConnectorForItemOverride(Connector element, TConnector item);

        //void ClearConnectorForItemOverride(Connector element, TConnector item);
    }

    internal class NeededEvents
    {
        public bool NeedSourceChangedEvent = false;
        public bool NeedTargetChangedEvent = false;
        public bool NeedNodeChangedEvent = false;
        public bool NeedItemAddedEvent = false;
        public bool NeedItemDeletedEvent = false;
        public bool NeedUndoableEvent = false;
        public bool NeedViewportChangedEvent = false;
    }

    internal class InvalidateState
    {
        public bool RecalculateVirtualization = true;
    }
}
