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
using System.Reflection;
using System.Text;
#if WINRT_USING
using System.Threading.Tasks;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
#else
using System.Windows.Shapes; 
using System.Windows.Controls;
using System.Windows;
#endif
using Syncfusion.UI.Xaml.Diagram.Panels;
using Syncfusion.UI.Xaml.Diagram.Serializer;
using Syncfusion.UI.Xaml.Diagram.Controls;
using System.ComponentModel;
using ScrollViewer = Syncfusion.UI.Xaml.Diagram.Controls.ScrollViewer;

namespace Syncfusion.UI.Xaml.Diagram.Controller
{
    internal class SharedData
    {
        public Rectangle selectionRectangle;
        public ContentPresenter _mPreview;
        public NullSourceTarget NullSourceTarget = NullSourceTarget.None;
        internal bool _unitchanging = false;
        public SharedData(IGraphInternal graph)
        {
            NeededEvents = new NeededEvents();
            EventAggregator = new EventAggregartor();
            Selected = EventAggregator.GetEvent<SelectedEvent<IInternalGroupable>>();
            UnSelected = EventAggregator.GetEvent<UnSelectedEvent<IInternalGroupable>>();
            PropertyMappingDictionary = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
            Graph = graph;

            Commands = new DiagramCommands();
            Commands.Init(this);
            Graph.SetCommands(Commands);
            VirtualizingController = new VirtualizingController();
            VirtualizingController.Init(this);

            ViewDictionary = new DataTemplateDictionary();
            ViewManager = new DataTemplateViewController<IView>(ViewDictionary);
            ViewManager.Init(this);

            RelationshipController = new RelationshipController();
            RelationshipController.Init(this);

            Serializer = new GraphSerializerController();
            Serializer.Init(this);

            SpatialSearch = new SpatialSearching();
            SpatialSearch.Init(this);

            Routing = new LineRouting();
            Routing.Init(this);


#if SyncfusionFramework4_5_1 && WINRT
            PrintandExportController = new ExportController();
            PrintandExportController.Init(this);
#endif

            GlobalNodes = new Dictionary<object, IInternalNode>();
            GlobalConnectors = new Dictionary<object, IInternalConnector>();
            GlobalGroup = new Dictionary<object, IInternalGroup>();
            GlobalPorts = new Dictionary<IPort, IWrapper>();
            GlobalAnnotation = new Dictionary<IAnnotation, IWrapper>();

            InvalidateState = new InvalidateState();

            Adorner = new Adorner();
            this.Adorner.Init(this);
        }

        public Dictionary<object, IInternalNode> GlobalNodes { get; set; }
        public Dictionary<object, IInternalConnector> GlobalConnectors { get; set; }
        public Dictionary<object, IInternalGroup> GlobalGroup { get; set; }
        public Dictionary<IPort, IWrapper> GlobalPorts { get; set; }
        public Dictionary<IAnnotation, IWrapper> GlobalAnnotation { get; set; }

        public IGraphInternal Graph { get; private set; }
        public VirtualizingController VirtualizingController { get; private set; }
        public DataTemplateDictionary ViewDictionary { get; private set; }
        public DataTemplateViewController<IView> ViewManager { get; private set; }
        public RelationshipController RelationshipController { get; private set; }
        public UndoRedoController UndoRedoController { get; private set; }
        public GraphSerializerController Serializer { get; private set; }
        public SpatialSearching SpatialSearch { get; private set; }
        public LineRouting Routing { get; private set; }
        public SnapSettingsWrapper SnapSettingsController { get; private set; }
        
        public DiagramCommands Commands { get; private set; }

        public void SetUndoRedoController(UndoRedoController controller)
        {
            if (UndoRedoController != null)
            {
                UndoRedoController.Dispose();
            }
            if (controller != null)
            {
                controller.Init(this);
            }
            UndoRedoController = controller;
        }

        public EventAggregartor EventAggregator { get; private set; }
        public Dictionary<Type, Dictionary<string, PropertyInfo>> PropertyMappingDictionary { get; private set; }
        public DiagramPage Page { get; set; }

        public SelectedEvent<IInternalGroupable> Selected { get; private set; }
        public UnSelectedEvent<IInternalGroupable> UnSelected { get; private set; }
        public NeededEvents NeededEvents { get; private set; }
        public InvalidateState InvalidateState { get; private set; }
        public ScrollViewer ScrollViewer { get; private set; }
        public PageSettingsWrapper PageSettingsController { get; private set; }
#if SyncfusionFramework4_5_1 && WINRT
        public ExportController PrintandExportController { get; private set; }
#endif
        public int AutoIncrement = 0;

        public void SetScrollViewer(ScrollViewer sv)
        {
            ScrollViewer = sv;
            Graph.SetScrollInfo(sv);
            if (sv != null)
            {
                if (Graph.HorizontalRuler != null)
                {
                    Graph.HorizontalRuler.PrepareRuler(sv);
                }
                if (Graph.VerticalRuler != null)
                {
                    Graph.VerticalRuler.PrepareRuler(sv);
                }
            }
            Adorner.PrepareAdorner();
        }

        public Adorner Adorner { get; private set; }

        public GridLinePanel GridLinePanel { get; set; }
         
        private readonly MeasurementUnit _nullUnit = new LengthUnit{ Unit = LengthUnits.Pixels };
        public MeasurementUnit Unit {
            get
            {
                if (Graph.PageSettings != null && Graph.PageSettings.Unit != null)
                {
                    return Graph.PageSettings.Unit;
                }
                return _nullUnit;
            } 
        }

        internal void SetSnapSettings(SnapSettings snapSettings)
        {
            if (SnapSettingsController != null)
            {
                (SnapSettingsController as IDisposable).Dispose();
            }

            if (SnapSettingsController != null)
            {
                SnapSettingsController.OnSnapSettingsChanged();
            }
            else if (Graph.SnapSettings != null)
            {
                SnapSettingsController = new SnapSettingsWrapper(Graph.SnapSettings, this);
            }

        }
        internal void SetDragDropPreview(ContentPresenter Preview)
        {
            _mPreview.Content = Preview;
            _mPreview.Width = Preview.Width;
            _mPreview.Height = Preview.Height;
        }
        internal void ClearDragDropPreview()
        {
            _mPreview.Content = null;
        }
        internal void SetPageSetting()
        {
            if (PageSettingsController != null)
            {
                (PageSettingsController as IDisposable).Dispose();
            }
            if (PageSettingsController != null)
            {
                PageSettingsController.OnPageSettingsChanged();
            }
            else if(Graph.PageSettings != null)
            {
                PageSettingsController = new PageSettingsWrapper(Graph.PageSettings, this);
                SnapSettingsController.PrepareUnit();
            }
            else
            {
                PageSettingsController = null;
            }
        }
    }

}
