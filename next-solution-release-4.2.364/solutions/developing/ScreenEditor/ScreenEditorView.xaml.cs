using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Threading;
using OPCUAViewModel;
using ScreenManager.Adorners;
using ScreenManager.ComponentService;
using ScreenSettings;
using ScreenSettings.Entities;
using ScreenManager.UndoRedoManager;
using UFInterfaces;
using Utilities;
using Utilities.Animations;
using Utilities.WPF;
using System.Threading;
using Utilities.ProgressDialog;
using CommandManager;
using AnimationManager;
using System.IO;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Text;
using Toolbox.ComponentService;
using WPFUtilities;
using DocumentManager.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using Converters;
using UFInterfaces.Scriptable;
using DevExpress.Xpf.Printing;
using WPFUtilities.PropertyDataTemplate;
using UFInterfaces.Converters;
using ScreenSettings.Documents;
using StringManager.ComponentService;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using Utilities.Commands;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Layout.Core;
using System.IO.IsolatedStorage;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.Docking.VisualElements;
using ViewModelLib;
using log4net;

namespace ScreenManager
{
    public class ChangeScreenWatcher : IDisposable
    {
        readonly ScreenEditorView screenEditorView;
        readonly bool Undo;
        readonly ScreenManager.UndoRedoManager.UndoAction Action;

        public ChangeScreenWatcher(ScreenEditorView view, bool undo = true,
                                 ScreenManager.UndoRedoManager.UndoAction action = ScreenManager.UndoRedoManager.UndoAction.Changed)
        {
            screenEditorView = view;
            Undo = undo;
            Action = action;
            if (Undo)
            {
                if (Action == ScreenManager.UndoRedoManager.UndoAction.Changed)
                    screenEditorView.undoredo.SetStateForUndoRedo(Action);
            }
        }

        public void Dispose()
        {
            if (Undo)
            {
                if (Action != ScreenManager.UndoRedoManager.UndoAction.Changed)
                    screenEditorView.undoredo.SetStateForUndoRedo(Action);
            }
            screenEditorView.MainSurface_SetModified(true);
        }
    }

    //public class SelectedObject
    //{
    //    private SelectedObject() {}
    //    public SelectedObject(UIElement u, ScreenEntity s)
    //    {
    //        Element = u;
    //        Entity = s;
    //    }

    //    public UIElement Element { get; set; }
    //    public ScreenEntity Entity { get; set; }
    //}

    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ScreenEditorView : UserControl, IEditableObject, IGridViewInfoService, INotifyPropertyChanged, IDisposable
    {
        #region Members

        readonly int nAnimationTime = 250;
        bool isUpdatingEntityPosition = false;
        bool isMovingEntity = false;
        MessageAdorner _gestureResultAdorner;
        GridAdorner _gridAdorner;
        TreeListControl objectBrowserTree;
        TreeListControl referenceListTree;
        TagListSearchOperator searchOperator;
        Dictionary<UIElement, BasicAdorner> SelectedElementsMap = new Dictionary<UIElement, BasicAdorner>();
#if CanvasEnabled
        InkAnalyzer analyzer;
#endif
        readonly ScreenManagerComponent EditorComponent;
        internal ScreenManagerComponent ScreenEditorComponent
        {
            get
            {
                return EditorComponent;
            }
        }

        readonly Canvas ActiveLayer;
        internal readonly UndoRedo undoredo;
        TreeListControl activeTree = null;
        string dockingFileName;
        
        // readonly DocumentObject documentObject;

        DelayedSingleActionInvoker SizeChangedInvoker;

        int currentVisibilityLevel = Int32.MaxValue;
        public int CurrentVisibilityLevel {
            get
            {
                return currentVisibilityLevel;
            }
            set
            {
                if (value != currentVisibilityLevel)
                {
                    currentVisibilityLevel = value;
                    OnPropertyChanged("CurrentVisibilityLevel");
                }
            }
        }

        IStringEditorManager StringEditor;
        ObservableCollection<string> activeLanguages = new ObservableCollection<string>() { Properties.Resources.None };

        public ObservableCollection<string> ActiveLanguages {
            get
            {
                return activeLanguages;
            }
            set
            {
                activeLanguages = value;
                OnPropertyChanged("ActiveLanguages");
            }
        }
        string lastCulture;

        FrameworkElement MainControl
        {
            get
            {
#if !WINDOWS_UWP
                return Document.LayoutView ? layoutItems as FrameworkElement : MainSurface as FrameworkElement;
#else
                return MainSurface as FrameworkElement;
#endif
            }
        }

        #endregion Members

        #region Constructors
        public ScreenEditorView(ScreenManagerComponent editorComponent, ScreenDocument doc)
        {
            InitializeComponent();

            dockingFileName = String.Format("{0}{1}", GetType().Name, "_Popups");
            EditorComponent = editorComponent;
            Document = doc;
            List<double> zoomList = new List<double>();
            (Properties.Settings.Default.ZoomLevelList).Split('|').ToList().ForEach(x =>
            {
                double value;
                if (double.TryParse(x, out value))
                    zoomList.Add(value);
            });
            comboZoom.ItemsSource = zoomList;
            //editorComponent.menuControl.DataContext = this;

            DesignerProperties.SetIsInDesignMode(MainSurface, true);
            ScreenDocument.SetScreenDocument(MainSurface, Document);

            Document.GetTagList += (o, e) =>
                {
                    if (Document != null)
                        e.list = EditorComponent.UFUAEditor.GetFlatListTags(Document);
                };
            Document.GetPrototypeList += (o, ev) =>
            {
                if (Document != null)
                {
                    ev.mapDefinitions = EditorComponent.UFUAEditor.GetFlatListPrototypes(Document);
                    ev.mapPrototypes = EditorComponent.UFUAEditor.GetFlatListPrototypeInstances(Document);
                }
            };

            Document.SubscribeToDocumentChangeEntries();

            Document.PropertyChanged += (o, e) =>
                {
                    if (e.PropertyName == "Width")
                    {
                        if (!bRequestResize)
                        {
                            bRequestResize = true;
                            previousHeight = Document.PreviewHeight;
                            previousWidth = Document.PreviewWidth;
                        }
                        if (Document.Width > 0 && Document.Width <= ScreenSettings.Properties.Settings.Default.ScreenMaxWidth)
                        {
                            cvsBackground.Width = borderMainSurface.Width = Document.Width;

                            UpdateScrollerSize();
                        }
                        //else
                        //    borderMainSurface.Width = MainSurface.Width = Double.NaN;
                    }
                    else if (e.PropertyName == "Height")
                    {
                        if (!bRequestResize)
                        {
                            bRequestResize = true;
                            previousHeight = Document.PreviewHeight;
                            previousWidth = Document.PreviewWidth;
                        }
                        if (Document.Height > 0 && Document.Height <= ScreenSettings.Properties.Settings.Default.ScreenMaxHeight)
                        {
                            cvsBackground.Height = borderMainSurface.Height = Document.Height;
                            UpdateScrollerSize();
                        }
                        //else
                        //    borderMainSurface.Height = MainSurface.Height = Double.NaN;
                    }
                    else if (e.PropertyName == "Background")
                        cvsBackground.Background = Document.Background;
                };

            Document.RepositoryItemLoaded += (o, e) => {
                if (StringEditor != null)
                {
                    Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                    {
                        if (!bDisposed)
                        {
                            ChangeLanguage(StringEditor.GetActiveCulture(Document), o as FrameworkElement);
                        }
                    });
                }
                UpdateHMIControl(o as UIElement);
            };
            // Document.ScreenEditorView = this;
            // Create a name scope for the stackpanel.
            NameScope.SetNameScope(MainSurface, new NameScope());
            NameScope.SetNameScope(layoutItems, new NameScope());

            ActiveLayer = MainSurface;
            // MainSurface.Strokes.StrokesChanged += Strokes_StrokesChanged;
#if CanvasEnabled
            analyzer = new InkAnalyzer(Dispatcher);
            analyzer.ResultsUpdated += analyzer_ResultsUpdated;
#endif // CanvasEnabled

            // documentObject = new DocumentObject(MainSurface, Document);
            EditorComponent.Workspace.ContextObject = Document;
            EditorComponent.Workspace.DockItemRestored += Workspace_DockItemRestored;

            undoredo = new UndoRedo(this);
        }

        internal void ProjectTypeChanged()
        {
            UpdateHMIControls(); 
        }

        private void UpdateHMIControls()
        {
            (from key in Document.MapScreenEntities.Keys
             where Document.MapScreenEntities[key].Entity != null
             select key).ToList().ForEach(name =>
             {
                 var uie = Document.MapScreenEntities[name].Entity;
                 if (Document.IsInnerEntity(name))
                 {
                     UpdateHMIControl(uie, Document.FindParentInnerControl(MainSurface, name));
                 }
                 else
                     UpdateHMIControl(uie);
             });
        }

        private void UpdateHMIControl(UIElement uielement, UIElement parent = null)
        {
            UIElement _uielement = uielement;
            var entity = GetSelectedEntity((FrameworkElement)uielement);
            if (entity.SourceSymbolLinked && uielement is ContentControl && !(uielement is UserControl) && (uielement as ContentControl).Content is FrameworkElement)
                _uielement = (uielement as ContentControl).Content as FrameworkElement;

            string uiType = _uielement.GetType().Name;
            if (entity.Contains3DElement())
                uiType = typeof(Viewport3D).Name;

            if (WebHMIDesignHelper.WebHMIHelper.VisibleHMIScreenControls.Contains(uiType))
                return;

            uielement = parent != null ? parent : uielement;

            if (!ActiveLayer.Children.Contains(uielement))
                return;
            CleanWebHMIState(uielement);
            if (Document.ProjectType == ProjectType.WebHMI.ToString())
            {
                var ad = new WarningAdorner(uielement);
                ad.AllowDrop = false;

                ad.DataContext = entity;

                var adorner = AdornerLayer.GetAdornerLayer(MainSurface);
                adorner.Add(ad);
                ad.PreviewMouseDown += (o, ev) =>
                {
                    MainSurface_CleanCurrentSelectedion();
                    var toSelect = new List<UIElement>() { uielement };
                    ReadOnlyCollection<UIElement> roList = new ReadOnlyCollection<UIElement>(toSelect);
                    MainSurface_SetCurrentSelection(roList);
                };
            }
        }


        private void Workspace_DockItemRestored(object sender, EventArgs e)
        {
            if (bIsActive && layoutItems.Visibility == Visibility.Visible)
                layoutItems.IsCustomization = true;
        }

        #endregion Constructors

        #region Properties

        ScreenDocument _Document;

        [Browsable(false)]
        public ScreenDocument Document
        {
            get
            {
                return _Document;
            }
            private set
            {
                _Document = value;
            }
        }

        double canvasHeight = Double.NaN;
        double canvasWidth = Double.NaN;

        [Browsable(false)]
        public Canvas Surface
        {
            get
            {
                Canvas cv = new Canvas
                {
                    Background = cvsBackground.Background,
                    Resources = MainSurface.Resources
                };

                if (!Double.IsNaN(canvasHeight) && canvasHeight > 0)
                    cv.Height = canvasHeight;
                if (!Double.IsNaN(canvasWidth) && canvasWidth > 0)
                    cv.Width = canvasWidth;

                //var size = new Size(MainSurface.ActualWidth, MainSurface.ActualHeight);
                //cv.Measure(size);
                //cv.Arrange(new Rect(0, 0, size.Width, size.Height));

                //foreach (var style in Document.GetListResourceUsed())
                //    cv.Resources.Add(style, new Style()); // add empty style just to make xamlread happy, real style are loaded from real resources

                NameScope.SetNameScope(cv, new NameScope());

                Document.ResolveProblematicXamlOnCanvas(MainSurface);

                foreach (FrameworkElement uie in MainSurface.Children)
                {
                    String xamlData = uie.XamlWriterFormatted();
                    FrameworkElement element = null;
                    try
                    {
                        element = xamlData.ReadUIElement() as FrameworkElement;
                    }
                    catch (Exception ex)
                    {
                        EditorComponent.UIInterface.ShowError(String.Format("{0} : {1}", uie.Name, ex.Message));
                    }
                    if (element == null)
                        continue;
                    //element.SetValue(SkinStorage.VisualStyleProperty, DependencyProperty.UnsetValue);
                    //foreach (var el in element.GetChildrenOfType<FrameworkElement>())
                    //    el.SetValue(SkinStorage.VisualStyleProperty, DependencyProperty.UnsetValue);

                    element.SetValue(InkCanvas.TopProperty, DependencyProperty.UnsetValue);
                    element.SetValue(InkCanvas.LeftProperty, DependencyProperty.UnsetValue);

                    //just add the element, it wasn't another Canvas that we
                    //added to the dataobject as a container
                    //if (!(element is Polygon) && !(element is Polyline) && !(element is Line))
                    //{
                    //    element.Width = uie.ActualWidth;
                    //    element.Height = uie.ActualHeight;
                    //}
                    element.Name = uie.Name;

                    /*
                    if (MainSurface is Canvas)
                    {
                        Canvas.SetLeft(element, Canvas.GetLeft(uie));
                        Canvas.SetTop(element, Canvas.GetTop(uie));
                        element.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                        element.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                    }
                    */
                    try
                    {
                        cv.Children.Add(element);
                    }
                    catch (Exception ex)
                    {
                        EditorComponent.UIInterface.ShowError(String.Format("{0} : {1}", element.Name, ex.Message));
                    }

                    if (element is FrameworkElement)
                        Utilities.WPF.DependencyObjectExtensions.RegisterName(cv, element as FrameworkElement, false, false);
                }

                Document.RestoreProblematicXamlWriter(MainSurface);
                Document.CleanProblematicXamlBags();

                Document.CleanRepositoryItemBindings(cv);
                return cv;
            }

            set
            {
                var visibilityMain = MainSurface.Visibility;
                MainSurface.Visibility = Visibility.Collapsed;
                var visibilityLayout = layoutItems.Visibility;
                layoutItems.Visibility = Visibility.Collapsed;

                CleanSurface();

                if (value != null)
                {
                    canvasHeight = MainSurface.Height = value.Height;
                    canvasWidth = MainSurface.Width = value.Width;
                    //MainSurface.Background = value.Background;
                    Document.Background = value.Background;
                    MainSurface.Resources = value.Resources;

                    if (!Double.IsNaN(Document.Width) && Document.Width > 0 && Document.Width <= ScreenSettings.Properties.Settings.Default.ScreenMaxWidth)
                        cvsBackground.Width = borderMainSurface.Width = MainSurface.Width = Document.Width;
                    if (!Double.IsNaN(Document.Height) && Document.Height > 0 && Document.Height <= ScreenSettings.Properties.Settings.Default.ScreenMaxHeight)
                        cvsBackground.Height = borderMainSurface.Height = MainSurface.Height = Document.Height;

                    while (value.Children.Count > 0)
                    {
                        UIElement child = value.Children[0] as UIElement;

                        double left = Canvas.GetLeft(child);
                        double top = Canvas.GetTop(child);

                        left = double.IsNaN(left) ? 0 : left;
                        top = double.IsNaN(top) ? 0 : top;

                        //if (MainSurface is Canvas)
                        //{
                        Canvas.SetLeft(child, left);
                        Canvas.SetTop(child, top);
                        //}
                        //else
                        //{
                        //    Canvas.SetLeft(child, left);
                        //    Canvas.SetTop(child, top);
                        //}

                        value.Children.Remove(child);
                        MainSurface.Children.Add(child);

                        if (child is FrameworkElement)
                            Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, child as FrameworkElement, false, false);
                    }
                }

                MainSurface.Visibility = visibilityMain;
                layoutItems.Visibility = visibilityLayout;

                //Dispatcher.BeginInvoke(
                //(Action)delegate
                //{
                //    using (new WaitCursor())
                //    {
                //        undoredo.SetStateForUndoRedo();
                //        // ApplyDynamicElements();
                //    }
                //}, DispatcherPriority.ApplicationIdle);
            }
        }

        public double ZoomLevel
        {
            get
            {
                return (ZoomLevelX + ZoomLevelY) / 2;
            }
            set
            {
                if (Object.Equals((ZoomLevelX + _ZoomLevelY) / 2, value))
                    return;

                ZoomLevelX = value;
                ZoomLevelY = value;
            }
        }

        double _selWidth;
        public double SelectionWidth
        {
            get
            {
                return _selWidth;
            }
            set
            {
                if (_selWidth != value)
                    _selWidth = value;
                ChangeW();
                OnPropertyChanged("SelectionWidth");
            }
        }

        double _selHeight;
        public double SelectionHeight
        {
            get
            {
                return _selHeight;
            }
            set
            {
                if (_selHeight != value)
                    _selHeight = value;
                ChangeH();
                OnPropertyChanged("SelectionHeight");
            }
        }

        double _selXPos;
        public double SelectionXPos
        {
            get
            {
                return _selXPos;
            }
            set
            {
                if (_selXPos != value)
                    _selXPos = value;
                MoveX();
                OnPropertyChanged("SelectionXPos");
            }
        }

        double _selYPos;
        public double SelectionYPos
        {
            get
            {
                return _selYPos;
            }
            set
            {
                if(_selYPos != value)
                    _selYPos = value;
                MoveY();
                OnPropertyChanged("SelectionYPos");
            }
        }
        private void MoveX()
        {
            if (isUpdatingEntityPosition)
                return;
            isMovingEntity = true;
            var listSelected = MainSurface_GetCurrentSelection();
            if (listSelected?.Count > 0)
            {
                MainSurface_CleanCurrentSelectedion();

                var elementsToSelect = new List<UIElement>();
                using (new ChangeScreenWatcher(this))
                {
                    double dNewValue = SelectionXPos;

                    listSelected.ForEach(uie =>
                    {
                        Canvas.SetLeft(uie, dNewValue);
                        elementsToSelect.Add(uie);

                        if (listSelected.Last() == uie)
                        {
                            var readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                            MainSurface_SetCurrentSelection(readonlyList, bSkipFocus: true);
                            isMovingEntity = false;
                        }
                    });
                }
            }
        }
        private void MoveY()
        {
            if (isUpdatingEntityPosition)
                return;

            isMovingEntity = true;
            var listSelected = MainSurface_GetCurrentSelection();
            if (listSelected?.Count > 0)
            {
                var elementsToSelect = new List<UIElement>();
                using (new ChangeScreenWatcher(this))
                {
                    MainSurface_CleanCurrentSelectedion();

                    double dNewValue = SelectionYPos;

                    listSelected.ForEach(uie =>
                    {
                        Canvas.SetTop(uie, dNewValue);
                        elementsToSelect.Add(uie);

                        if (listSelected.Last() == uie)
                        {
                            var readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                            MainSurface_SetCurrentSelection(readonlyList, bSkipFocus: true);
                            isMovingEntity = false;
                        }
                    });
                }
            }
        }
        private void ChangeW()
        {
            if (isUpdatingEntityPosition)
                return;

            isMovingEntity = true;
            var listSelected = MainSurface_GetCurrentSelection();
            if (listSelected?.Count > 0)
            {
                var elementsToSelect = new List<UIElement>();
                using (new ChangeScreenWatcher(this))
                {
                    //MainSurface_CleanCurrentSelectedion();

                    double dNewValue = SelectionWidth;

                    listSelected.ForEach(uie =>
                    {
                        var factor = 1.0;
                        if ((uie as FrameworkElement).ActualWidth > 0)
                            factor = dNewValue / (uie as FrameworkElement).ActualWidth;
                        (uie as FrameworkElement).Width = dNewValue;
                        TransformsPoints(uie, factor, 1, false, false);
                        elementsToSelect.Add(uie);

                        if (listSelected.Last() == uie)
                        {
                            MainSurface_CleanCurrentSelectedion();
                            var readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                            MainSurface_SetCurrentSelection(readonlyList, bSkipFocus: true);
                            isMovingEntity = false;
                        }
                    });
                }
            }
        }
        private void ChangeH()
        {
            if (isUpdatingEntityPosition)
                return;

            isMovingEntity = true;

            var listSelected = MainSurface_GetCurrentSelection();
            if (listSelected?.Count > 0)
            {
                var elementsToSelect = new List<UIElement>();
                using (new ChangeScreenWatcher(this))
                {
                    //MainSurface_CleanCurrentSelectedion();

                    double dNewValue = SelectionHeight;

                    listSelected.ForEach(uie =>
                    {
                        var factor = 1.0;
                        if ((uie as FrameworkElement).ActualHeight > 0)
                            factor = dNewValue / (uie as FrameworkElement).ActualHeight;
                        (uie as FrameworkElement).Height = dNewValue;
                        TransformsPoints(uie, factor, 1, false, false);
                        elementsToSelect.Add(uie);

                        if (listSelected.Last() == uie)
                        {
                            MainSurface_CleanCurrentSelectedion();
                            var readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                            MainSurface_SetCurrentSelection(readonlyList, bSkipFocus: true);
                            isMovingEntity = false;
                        }
                    });
                }
            }
        }

        void UpdateScrollerSize()
        {
            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    if (!bDisposed)
                    {
                        //if (_ZoomLevelX < 1)
                        //    MainSurface.Width = gridLayout.ActualWidth / _ZoomLevelX + SystemParameters.VerticalScrollBarWidth;
                        //else
                        {
                            if (Document.Width > Scroller.ViewportWidth)
                                MainSurface.Width = Scroller.ExtentWidth / _ZoomLevelX;
                            else
                                MainSurface.Width = Scroller.ViewportWidth;

                            if (Document.Height > Scroller.ViewportHeight)
                                MainSurface.Height = Scroller.ExtentHeight / _ZoomLevelY;
                            else
                                MainSurface.Height = Scroller.ViewportHeight;
                        }

                        PART_ZoomTransformBackground.ScaleX = PART_ZoomTransformRect.ScaleX = PART_ZoomTransform.ScaleX = _ZoomLevelX;

                        //if (_ZoomLevelY < 1)
                        //    MainSurface.Height = gridLayout.ActualHeight / _ZoomLevelY + SystemParameters.HorizontalScrollBarHeight;
                        //else
                        {
                            if (Document.Width > Scroller.ViewportWidth)
                                MainSurface.Width = Scroller.ExtentWidth / _ZoomLevelX;
                            else
                                MainSurface.Width = Scroller.ViewportWidth;

                            if (Document.Height > Scroller.ViewportHeight)
                                MainSurface.Height = Scroller.ExtentHeight / _ZoomLevelY;
                            else
                                MainSurface.Height = Scroller.ViewportHeight;
                        }

                        PART_ZoomTransformBackground.ScaleY = PART_ZoomTransformRect.ScaleY = PART_ZoomTransform.ScaleY = _ZoomLevelY;
                    }
                });
        }

        private double _ZoomLevelX = 1.0;
        public double ZoomLevelX
        {
            get
            {
                return _ZoomLevelX;
            }
            set
            {
                if (Object.Equals(_ZoomLevelX, value))
                    return;

                if (value <= 0)
                    value = 0.0000000001;

                _ZoomLevelX = value;
                OnPropertyChanged("ZoomLevelX");
                OnPropertyChanged("ZoomLevel");

                UpdateScrollerSize();
            }
        }

        private double _ZoomLevelY = 1.0;
        public double ZoomLevelY
        {
            get
            {
                return _ZoomLevelY;
            }
            set
            {
                if (Object.Equals(_ZoomLevelY, value))
                    return;

                if (value <= 0)
                    value = 0.0000000001;

                _ZoomLevelY = value;
                OnPropertyChanged("ZoomLevelY");
                OnPropertyChanged("ZoomLevel");

                UpdateScrollerSize();
            }
        }

        private double _OffsetLevelX = 0;
        public double OffsetLevelX
        {
            get
            {
                return _OffsetLevelX;
            }
            set
            {
                if (Object.Equals(_OffsetLevelX, value))
                    return;

                _OffsetLevelX = value;
                OnPropertyChanged("OffsetLevelX");

                PART_PanTransformRect.X = PART_PanTransform.X = _OffsetLevelX;
            }
        }

        private double _OffsetLevelY = 0;

        public double OffsetLevelY
        {
            get
            {
                return _OffsetLevelY;
            }
            set
            {
                if (Object.Equals(_OffsetLevelY, value))
                    return;

                _OffsetLevelY = value;
                OnPropertyChanged("OffsetLevelY");

                PART_PanTransformRect.Y = PART_PanTransform.Y = _OffsetLevelY;
            }
        }

        /// <summary>
        /// An Adorner which is used for displaying the gesture feeback.
        /// </summary>
        [Browsable(false)]
        private MessageAdorner GestureResultAdorner
        {
            get
            {
                // Initialize the MessageAdorner if it isn't created yet.
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(MainSurface);
                if (_gestureResultAdorner != null)
                    adornerLayer.Remove(_gestureResultAdorner);

                _gestureResultAdorner = new MessageAdorner(MainSurface);

                // The MessageAdorner only needs to be rendered. Disable the HitTest on it.
                _gestureResultAdorner.IsHitTestVisible = false;
                if (adornerLayer != null)
                    adornerLayer.Add(_gestureResultAdorner);
                return _gestureResultAdorner;
            }
        }

        bool isSearchCaseSensitive = true;
        [Browsable(false)]
        public bool IsSearchCaseSensitive
        {
            get
            {
                return isSearchCaseSensitive;
            }
            set
            {
                if (isSearchCaseSensitive != value)
                {
                    isSearchCaseSensitive = value;
                    UpdateSearchFilterCriteria();
                    OnPropertyChanged("IsSearchCaseSensitive");
                }
            }
        }
        bool isFullSearchAllowed = false;
        [Browsable(false)]
        public bool IsFullSearchAllowed
        {
            get
            {
                return isFullSearchAllowed;
            }
            set
            {
                if (isFullSearchAllowed != value)
                {
                    isFullSearchAllowed = value;
                    UpdateSearchFilterCriteria();
                    OnPropertyChanged("IsFullSearchAllowed");
                }
            }
        }
        string searchResultsCount;
        [Browsable(false)]
        public string SearchResultsCount
        {
            get
            {
                if (referenceListTree != null)
                    return !string.IsNullOrEmpty(searchResultsCount) ? searchResultsCount : String.Format(Properties.Resources.SearchItemsFound, referenceListTree.VisibleItems.Count);
                else
                    return !string.IsNullOrEmpty(searchResultsCount) ? searchResultsCount : String.Empty;
            }
            set
            {
                if (searchResultsCount != value)
                {
                    searchResultsCount = value;
                    OnPropertyChanged("SearchResultsCount");
                }
            }
        }

        #endregion Properties

        #region Drag and Drop

        /// <summary>
        /// Called during the drag operation, we set e.Effects both to update
        /// the cursor and to inform the OnPreviewMouseDown method if it should
        /// remove the strokes.
        /// </summary>
        private void MainSurface_DragOver(object sender, DragEventArgs e)
        {
            if (bSetZOrderInProgress ||
                !(e.Data.GetDataPresent(StrokeCollection.InkSerializedFormat) ||
                  e.Data.GetDataPresent(DataFormats.Xaml) ||
                  e.Data.GetDataPresent("Item") ||
                  e.Data.GetDataPresent(typeof(ObservableCollection<MonitoredItemViewModel>)) ||
                  e.Data.GetDataPresent(typeof(Uri)) ||
                  e.Data.GetDataPresent(typeof(RecordDragDropData))))
                return;

            SetDragDropEffects(e);

            // e.Handled = true;
        }

        /// <summary>
        /// OnQueryContinueDrag is called to see if we should continue the drag and drop
        /// operation.  If the escape key is pressed, we cancel it.
        /// </summary>
        private void MainSurface_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (!e.EscapePressed)
            {
                SelectedElementsMap.Values.ToList().ForEach(ad =>
                {
                    ad.SetDraggable(true, true);
                });
                return;
            }

            currentCursor = null;
            e.Action = DragAction.Cancel;
            e.Handled = true;
        }

        /// <summary>
        /// SetDragDropEffects changes the cursor based on key and mouse state
        /// as well as what effects are allowed
        /// </summary>
        private void SetDragDropEffects(DragEventArgs e)
        {
            if (EditorComponent.Workspace.ActiveWindow != this)
                EditorComponent.Workspace.ActiveWindow = this;

            //if the CTRL key is down, treat this as a copy
            e.Effects = (e.KeyStates & DragDropKeyStates.ControlKey) != 0 && (e.AllowedEffects & DragDropEffects.Copy) != 0 ? DragDropEffects.Copy : (e.AllowedEffects & DragDropEffects.Move) != 0 ? DragDropEffects.Move : DragDropEffects.None;

            if (EditorComponent.ToolBox != null)
            {
                var data = e.Data.GetData(typeof(RecordDragDropData)) as RecordDragDropData;
                if (data != null)
                {
                    foreach (var v in data.Records)
                    {
                        var subitem = v as TreeItemControl;
                        if ((subitem == null || subitem.TreeItemInnerObject == null) && !(v is IDocumentManager))
                            continue;
                        if (v is IDocumentManager)
                        {
                            if (EditorComponent.ToolBox.IsToolboxDragging(v))
                                return;
                        }
                        else
                        {
                            if (EditorComponent.ToolBox.IsToolboxDragging(subitem.TreeItemInnerObject))
                                return;
                        }
                    }
                }
            }

            Point endPoint = e.GetPosition(ActiveLayer);
            UIElement uie = null;
            System.Diagnostics.Debug.WriteLine("Dropping Hitting Test on {0}", endPoint);
            var hitTestResult = VisualTreeHelper.HitTest(ActiveLayer, endPoint);
            System.Diagnostics.Debug.WriteLine("Dropping Hitting Test redult {0}", hitTestResult != null ?
                hitTestResult.VisualHit.GetType().ToString() : "*none*");
            if (hitTestResult != null && hitTestResult.VisualHit != null)
            {
                var parent = hitTestResult.VisualHit as DependencyObject;
                if (parent != null)
                {
                    do
                    {
                        System.Diagnostics.Debug.WriteLine("Dropping loop on Element {0}", parent.GetType());
                        if (ActiveLayer.Children.Contains(parent as UIElement))
                        {
                            System.Diagnostics.Debug.WriteLine("Found Element {0}", parent.GetType());
                            uie = parent as UIElement;
                            break;
                        }
                        parent = VisualTreeHelper.GetParent(parent) as DependencyObject;
                    } while (parent != null && parent != ActiveLayer);
                }
            }

            var elementsToSelect = MainSurface_GetCurrentSelection();
            //bool bChanged = false;
            if (uie != null)
            {
                if (elementsToSelect.Contains(uie) && elementsToSelect.Count == 1)
                    return;
                elementsToSelect.Clear();
                elementsToSelect.Add(uie);
            }
            else
                elementsToSelect.Clear();
            ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
            MainSurface_SetCurrentSelection(readonlyList);
            SelectedElementsMap.Values.ToList().ForEach(ad =>
            {
                ad.SetDraggable(false, true);
            });
        }

        private void CreateObjectFromSelectedType(Point dropPosition, ReferenceDescriptionViewModel model)
        {
            //String dataType = model.TypeDefinitionString;
            //if (model.IsMethod)
            //    dataType = model.NodeClass.ToString();
            //else if (model.IsEventNotifier)
            //    dataType = "EventNotifier";
            //FrameworkElement elementSource = EditorComponent.ToolBox.GetItemFromDataType(this,
            //                        model.TypeDefinitionString, dataType);
            //if (elementSource != null)
            //{
            //    List<UIElement> elementsToSelect = new List<UIElement>();
            //    String xamlData = elementSource.XamlWriterFormatted();

            //    FrameworkElement element = xamlData.ReadUIElement() as FrameworkElement;
            //    element.IsHitTestVisible = true;

            //    //just add the element, it wasn't another Canvas that we
            //    //added to the dataobject as a container
            //    element.Width = elementSource.ActualWidth;
            //    element.Height = elementSource.ActualHeight;

            //    if (MainSurface is Canvas)
            //    {
            //        Canvas.SetLeft(element, dropPosition.X);
            //        Canvas.SetTop(element, dropPosition.Y);
            //        element.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
            //        element.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
            //    }
            //    else
            //    {
            //        Canvas.SetLeft(element, dropPosition.X);
            //        Canvas.SetTop(element, dropPosition.Y);
            //        element.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
            //        element.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
            //    }

            //    ActiveLayer.Children.Add(element);
            //    DesignerProperties.SetIsInDesignMode(element, true);
            //    ScreenDocument.SetScreenDocument(element, Document);

            //    if (element is FrameworkElement)
            //        Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, element as FrameworkElement);

            //    elementsToSelect.Add(element);

            //    //now that we're done, we select
            //    ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
            //    MainSurface_SetCurrentSelection(readonlyList);

            //}
        }

        void MainSurface_Drop_New(object sender, DragEventArgs e) //Dropped item comes from a DevExpress TreeListControl
        {
            FrameworkElement targetElement = sender as FrameworkElement;
            Point dropPosition = e.GetPosition(ActiveLayer);
            AdjustPointToGrid(ref dropPosition);

            EditorComponent.Workspace.ActivateDockedElement(this);
            Focusable = true;
            Focus();

                if (e.Data.GetDataPresent(typeof(Uri))) //droppedType == typeof(Uri)
                {
                    Dispatcher.InvokeIfRequired(
                    (Action)delegate
                    {
                        Uri data = e.Data.GetData(typeof(Uri)) as Uri;

                        // var projectUri = new Uri(Document.Parent.rootBase, UriKind.RelativeOrAbsolute);
                        data = Document.Parent.MakeRelativeUri(data);

                        bool bCancelEdit = true;
                        BeginEdit();

                        List<UIElement> listSelected = MainSurface_GetCurrentSelection();
                        if (listSelected.Count == 0)
                        {
                        // CreateObjectFromSelectedType(dropPosition, data);
                        listSelected = MainSurface_GetCurrentSelection();
                        }

                        for(int i = listSelected.Count - 1; i >= 0; --i)
                        {
                            var uie = listSelected[i];
                            var bDropManagerHandled = false;
                            try
                            {
                                var method = uie.GetType().GetMethod("DropManager");
                                if (method != null)
                                {
                                    bDropManagerHandled = (bool)(method.Invoke(uie, new object[] { data }));
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                            if (bDropManagerHandled)
                                bCancelEdit = false;
                        //if (uie is ScreenManager.SpecialObjects.EmbeddedTabScreen && 
                        //    data.GetPathString().Contains(".xaml"))
                        //{
                        //    var screen = data.GetPathString();
                        //    var embedded = uie as ScreenManager.SpecialObjects.EmbeddedTabScreen;
                        //    if (embedded.Screens == null || !embedded.Screens.Contains(screen))
                        //    {
                        //        var list = new ScreenManager.SpecialObjects.ScreenList();
                        //        if (embedded.Screens != null)
                        //            list.AddRange(embedded.Screens);
                        //        list.Add(screen);
                        //        embedded.Screens = list;

                        //        bCancelEdit = false;
                        //    }
                        //    else
                        //    {
                        //        bCancelEdit = true;
                        //        break;
                        //    }
                        //}
                        else
                            {
                                var entity = GetSelectedEntity(uie as FrameworkElement);
                                if (!entity.OnDropUri(data))
                                {
                                    bCancelEdit = true;
                                    break;
                                }
                                else
                                    UpdateStringIdAndTranslate(uie);
                            }

                            bCancelEdit = false;
                        }

                        if (bCancelEdit)
                            CancelEdit();
                        else
                            EndEdit();
                    }, DispatcherPriority.ApplicationIdle);

                    e.Handled = true;
                }
                else if (e.Data.GetDataPresent(typeof(ObservableCollection<MonitoredItemViewModel>))) //droppedType == typeof(ObservableCollection<MonitoredItemViewModel>)
                {
                    Dispatcher.InvokeIfRequired(
                    (Action)delegate
                    {
                        var data = e.Data.GetData(typeof(ObservableCollection<MonitoredItemViewModel>)) as ObservableCollection<MonitoredItemViewModel>;

                        List<UIElement> listSelected = MainSurface_GetCurrentSelection();
                        if (listSelected.Count == 0 &&
                            (EditorComponent.ToolBox == null || EditorComponent.ToolBox.ActiveToolCode == null))
                            EditorComponent.UIInterface.ShowError(Properties.Resources.ToolboxSelectionNeeded);
                        else if (EditorComponent.ToolBox != null && EditorComponent.ToolBox.ActiveToolCode != null)
                        {
                            var listToSelect = new List<UIElement>();
                            var toolselected = EditorComponent.ToolBox.ActiveToolCode;
                            using (new ChangeScreenWatcher(this, true, UndoRedoManager.UndoAction.Added))
                            {
                                MainSurface.BeginInit();

                                EditorComponent.Workspace.IsBusy = true;
                                using (var cursor = new WaitCursor())
                                {
                                    try
                                    {
                                        int i = 0;
                                        foreach (var v in data)
                                        {
                                            EditorComponent.Workspace.BusyContent = String.Format(Properties.Resources.CreatingObjectFromToolbox, ++i, data.Count);
                                            EditorComponent.ToolBox.ActiveToolCode = toolselected;
                                            AddToolBoxDataObject(dropPosition);
                                            var element = MainSurface_GetCurrentSelection();
                                            if (element.Count > 0)
                                            {
                                                var entity = GetSelectedEntity(element[0] as FrameworkElement);
                                                entity.OnDropReference(v.referenceItem);

                                                listToSelect.AddRange(element);
                                            }
                                        }
                                    }
                                    finally
                                    {
                                        EditorComponent.Workspace.IsBusy = false;
                                        MainSurface.EndInit();
                                    }
                                }
                            }
                            ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(listToSelect);
                            MainSurface_SetCurrentSelection(readonlyList);
                            if (listToSelect.Count > 1)
                            {
                            // OnDistributeSpace(this, null);
                            Dispatcher.BeginInvokeAsynchronouslyInRender(() =>
                                OnDistributeSpace(this, null));
                            }
                        }
                        else if (listSelected.Count > 0)
                        {
                            bool bCancelEdit = true;
                            BeginEdit();
                            foreach (var v in data)
                            {
                                var model = v as MonitoredItemViewModel;
                                for (int i = listSelected.Count - 1; i >= 0; --i)
                                {
                                    var uie = listSelected[i];
                                    var entity = GetSelectedEntity(uie as FrameworkElement);
                                    if (!entity.OnDropReference(model.referenceItem))
                                    {
                                        bCancelEdit = true;
                                        break;
                                    }

                                    bCancelEdit = false;
                                }
                            }

                            if (bCancelEdit)
                                CancelEdit();
                            else
                                EndEdit();
                        }

                    }, DispatcherPriority.ApplicationIdle);
                    e.Handled = true;
                }
                //else if (droppedObject is IDocumentManager)
                //{
                //    Dispatcher.InvokeIfRequired(
                //    (Action)delegate
                //    {
                //        bool bCancelEdit = true;
                //        BeginEdit();
                //        var model = droppedObject as IDocumentManager;
                //        var reference = model.DragContent as OPCUAEntityReference;
                //        if (reference != null)
                //        {
                //            List<UIElement> listSelected = MainSurface_GetCurrentSelection();
                //            foreach (var uie in listSelected)
                //            {
                //                var entity = GetSelectedEntity(uie as FrameworkElement);
                //                if (!entity.OnDropReference(reference))
                //                {
                //                    bCancelEdit = true;
                //                    break;
                //                }
                //                else
                //                    UpdateStringIdAndTranslate(uie);

                //                bCancelEdit = false;
                //            }
                //        }
                //        if (bCancelEdit)
                //            CancelEdit();
                //        else
                //            EndEdit();
                //    }, DispatcherPriority.ApplicationIdle);
                //    e.Handled = true;
                //}
                else if (e.Data.GetDataPresent(typeof(DevExpress.Xpf.Core.RecordDragDropData)))
                {
                    Dispatcher.InvokeIfRequired(
                    (Action)delegate
                    {
                        bool bToolboxDragging = false;
                        if (EditorComponent.ToolBox != null)
                        {
                            var data = e.Data.GetData(typeof(RecordDragDropData)) as RecordDragDropData;
                            if (data != null)
                            {
                                foreach (var v in data.Records)
                                {
                                    var subitem = v as TreeItemControl;
                                    if ((subitem == null || subitem.TreeItemInnerObject == null) && !(v is IDocumentManager))
                                        continue;

                                    if (v is IDocumentManager)
                                    {
                                        if (EditorComponent.ToolBox.IsToolboxDragging(v))
                                        {
                                            bToolboxDragging = true;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (EditorComponent.ToolBox.IsToolboxDragging(subitem.TreeItemInnerObject))
                                        {
                                            bToolboxDragging = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        List<UIElement> listSelected = MainSurface_GetCurrentSelection();
                        if ((listSelected.Count == 0 || bToolboxDragging) && 
                            (EditorComponent.ToolBox == null || EditorComponent.ToolBox.ActiveToolCode == null))
                            EditorComponent.UIInterface.ShowError(Properties.Resources.ToolboxSelectionNeeded);
                        else if (EditorComponent.ToolBox != null && EditorComponent.ToolBox.ActiveToolCode != null)                     
                        {
                            var listToSelect = new List<UIElement>();
                            var toolselected = EditorComponent.ToolBox.ActiveToolCode;
                            using (new ChangeScreenWatcher(this, true, UndoRedoManager.UndoAction.Added))
                            {
                                MainSurface.BeginInit();
                                EditorComponent.Workspace.IsBusy = true;
                                using (var cursor = new WaitCursor())
                                {
                                    try
                                    {
                                        int i = 0;
                                        //foreach (var v in data)
                                        //{
                                        //TreeViewItemAdv subitem = v as TreeViewItemAdv;
                                        //if (subitem == null || subitem.Tag == null)
                                        //    continue;

                                        var data = e.Data.GetData(typeof(RecordDragDropData)) as RecordDragDropData;
                                        if (data != null)
                                        {
                                            foreach (var v in data.Records)
                                            {
                                                var subitem = v as TreeItemControl;
                                                if ((subitem == null || subitem.TreeItemInnerObject == null) && !(v is IDocumentManager))
                                                    continue;

                                                EditorComponent.Workspace.BusyContent = String.Format(Properties.Resources.CreatingObjectFromToolbox, ++i, 1/*data.Count*/);
                                                EditorComponent.ToolBox.ActiveToolCode = toolselected;
                                                AddToolBoxDataObject(dropPosition);
                                                var element = MainSurface_GetCurrentSelection();
                                                if (element.Count > 0)
                                                {
                                                    var entity = GetSelectedEntity(element[0] as FrameworkElement);

                                                    if (v is IDocumentManager)
                                                    {
                                                        var reference = (v as IDocumentManager).DragContent as OPCUAEntityReference;
                                                        if (reference != null)
                                                            entity.OnDropReference(reference);
                                                    }
                                                    else if (subitem.TreeItemInnerObject is ReferenceDescriptionViewModel)
                                                    {
                                                        var model = subitem.TreeItemInnerObject as ReferenceDescriptionViewModel;
                                                        entity.OnDropReference(model);
                                                    }
                                                    else if (subitem.TreeItemInnerObject is IDocumentManager)
                                                    {
                                                        var model = subitem.TreeItemInnerObject as IDocumentManager;
                                                        var reference = model.DragContent as OPCUAEntityReference;
                                                        if (reference != null)
                                                            entity.OnDropReference(reference);
                                                    }
                                                    else if (subitem.TreeItemInnerObject is OPCUAEntityReference)
                                                    {
                                                        var model = subitem.TreeItemInnerObject as OPCUAEntityReference;
                                                        entity.OnDropReference(model);
                                                    }

                                                    listToSelect.AddRange(element);
                                                }
                                            }
                                        }
                                    }
                                    finally
                                    {
                                        EditorComponent.Workspace.IsBusy = false;
                                        MainSurface.EndInit();
                                    }
                                }
                            }
                            ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(listToSelect);
                            MainSurface_SetCurrentSelection(readonlyList);
                            if (listToSelect.Count > 1)
                            {
                                // OnDistributeSpace(this, null);
                                Dispatcher.BeginInvokeAsynchronouslyInRender(() =>
                                    OnDistributeSpace(this, null));
                            }
                        }
                        else if (listSelected.Count > 0)
                        {
                            bool bCancelEdit = true;
                            BeginEdit();

                            var records = (e.Data.GetData(typeof(RecordDragDropData)) as RecordDragDropData).Records;
                            foreach (var record in records)
                            {
                                object droppedObject = null;
                                if (record is IDocumentManager)
                                {
                                    droppedObject = record;
                                }
                                else if (record is TreeItemControl)
                                {
                                    droppedObject = (record as TreeItemControl).TreeItemInnerObject;
                                }
                                Type droppedType = droppedObject.GetType();
                                //foreach (var v in data)
                                //{
                                //    TreeViewItemAdv subitem = v as TreeViewItemAdv;
                                //    if (subitem == null || subitem.Tag == null)
                                //        continue;

                                if (droppedObject is ReferenceDescriptionViewModel)
                                {
                                    var model = droppedObject as ReferenceDescriptionViewModel;

                                    for (int i = listSelected.Count - 1; i >= 0; --i)
                                    {
                                        var uie = listSelected[i];
                                        var entity = GetSelectedEntity(uie as FrameworkElement);
                                        if (!entity.OnDropReference(model))
                                        {
                                            bCancelEdit = true;
                                            break;
                                        }
                                        else
                                            UpdateStringIdAndTranslate(uie);

                                        bCancelEdit = false;
                                    }
                                }
                                else if (droppedObject is IDocumentManager)
                                {
                                    var model = droppedObject as IDocumentManager;
                                    var reference = model.DragContent as OPCUAEntityReference;
                                    if (reference != null || model.CanBeDragged)
                                    {
                                        for (int i = listSelected.Count - 1; i >= 0; --i)
                                        {
                                            var uie = listSelected[i];
                                            var entity = GetSelectedEntity(uie as FrameworkElement);
                                            if (reference != null)
                                            {
                                                if (!entity.OnDropReference(reference))
                                                {
                                                    bCancelEdit = true;
                                                    break;
                                                }
                                                else
                                                    UpdateStringIdAndTranslate(uie);
                                            } 
                                            else  if (entity.Element is IDropRecordAware && !(entity.Element as IDropRecordAware).OnDropRecord(model.DragContent))
                                            {
                                                bCancelEdit = true;
                                                break;
                                            }

                                            bCancelEdit = false;
                                        }
                                    }
                                }
                                else if (droppedObject is Uri)
                                {
                                    BeginEdit();

                                    Uri dataUri = droppedObject as Uri;
                                    // var projectUri = new Uri(Document.Parent.rootBase, UriKind.RelativeOrAbsolute);
                                    dataUri = Document.Parent.MakeRelativeUri(dataUri);

                                    for (int i = listSelected.Count - 1; i >= 0; --i)
                                    {
                                        var uie = listSelected[i];
                                        var bDropManagerHandled = false;
                                        try
                                        {
                                            var method = uie.GetType().GetMethod("DropManager");
                                            if (method != null)
                                            {
                                                bDropManagerHandled = (bool)(method.Invoke(uie, new object[] { dataUri }));
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                        }
                                        if (bDropManagerHandled)
                                            bCancelEdit = false;
                                        //if (uie is ScreenManager.SpecialObjects.EmbeddedTabScreen &&
                                        //    dataUri.GetPathString().Contains(".xaml"))
                                        //{
                                        //    var screen = dataUri.GetPathString();
                                        //    var embedded = uie as ScreenManager.SpecialObjects.EmbeddedTabScreen;
                                        //    if (embedded.Screens == null || !embedded.Screens.Contains(screen))
                                        //    {
                                        //        var list = new ScreenManager.SpecialObjects.ScreenList();
                                        //        if (embedded.Screens != null)
                                        //            list.AddRange(embedded.Screens);
                                        //        list.Add(screen);
                                        //        embedded.Screens = list;

                                        //        bCancelEdit = false;
                                        //    }
                                        //    else
                                        //    {
                                        //        bCancelEdit = true;
                                        //        break;
                                        //    }
                                        //}
                                        else
                                        {
                                            var entity = GetSelectedEntity(uie as FrameworkElement);
                                            if (!entity.OnDropUri(dataUri))
                                            {
                                                bCancelEdit = true;
                                                break;
                                            }
                                            else
                                                UpdateStringIdAndTranslate(uie);

                                            bCancelEdit = false;
                                        }
                                    }
                                }
                                else if (droppedObject is OPCUAEntityReference)
                                {
                                    var model = droppedObject as OPCUAEntityReference;

                                    for (int i = listSelected.Count - 1; i >= 0; --i)
                                    {
                                        var uie = listSelected[i];
                                        var entity = GetSelectedEntity(uie as FrameworkElement);
                                        if (!entity.OnDropReference(model))
                                        {
                                            bCancelEdit = true;
                                            break;
                                        }
                                        else
                                            UpdateStringIdAndTranslate(uie);

                                        bCancelEdit = false;
                                    }
                                }
                                //}

                                if (bCancelEdit)
                                    CancelEdit();
                                else
                                    EndEdit();
                            }
                        }
                    }, DispatcherPriority.ApplicationIdle);

                    e.Handled = true;
                }
                else if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    String[] filenames = e.Data.GetData(DataFormats.FileDrop) as String[];
                    Visual mp = null;
                    Uri uriFound = null;
                    if (filenames.Length > 0)
                    {
                        var dropHelper = new DropFileHelper(Document, uriToUriAbsoluteImageConverter, EditorComponent.UIInterface, SourceFileCopyOption.Ask);
                        foreach (var file in filenames)
                        {
                            try
                            {
                                mp = dropHelper.DropFile(file, out uriFound);
                                break;
                            }
                            catch (OperationCanceledException)
                            {
                                return;
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                    }

                    if (mp != null)
                    {
                        VisualBrush db = new VisualBrush(mp);

                        List<UIElement> listSelected = MainSurface_GetCurrentSelection();

                        using (new ChangeScreenWatcher(this, true, UndoRedoManager.UndoAction.Changed))
                        {
                            // var result = VisualTreeHelper.HitTest(ActiveLayer, dropPosition);
                            SetSelectedBrush(db, null, false);
                        }
                    }

                    e.Handled = true;
                }
                else if (e.Data.GetDataPresent(DataFormats.Xaml) ||
                    e.Data.GetDataPresent(DataFormats.Text))
                {
                    List<UIElement> elementsToSelect = new List<UIElement>();
                    SetDragDropEffects(e);

                    if (e.Effects == DragDropEffects.Move || e.Effects == DragDropEffects.Copy)
                    {
                        using (new ChangeScreenWatcher(this, true, UndoRedoManager.UndoAction.Added))
                        {
                            //after we drop, we need to select the elements and strokes that were
                            //copied into the Canvas
                            if (e.Data.GetDataPresent(DataFormats.Xaml) ||
                                e.Data.GetDataPresent(DataFormats.Text))
                            {
                                //paste Xaml
                                string xamlData = "";
                                if (e.Data.GetDataPresent(DataFormats.Xaml))
                                    xamlData = e.Data.GetData(DataFormats.Xaml) as String;
                                else if (e.Data.GetDataPresent(DataFormats.Text))
                                    xamlData = e.Data.GetData(DataFormats.Text) as String;

                                if (!String.IsNullOrEmpty(xamlData))
                                {
                                    UIElement element = null;
                                    try
                                    {
                                        element = xamlData.ReadUIElement();
                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                    if (element != null)
                                    {
                                        element.IsHitTestVisible = true;
                                        //just add the element, it wasn't another Canvas that we
                                        //added to the dataobject as a container
                                        //if (MainSurface is Canvas)
                                        //{
                                        Canvas.SetLeft(element, dropPosition.X);
                                        Canvas.SetTop(element, dropPosition.Y);
                                        //    element.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                                        //    element.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                                        //}
                                        //else
                                        //{
                                        //    Canvas.SetLeft(element, dropPosition.X);
                                        //    Canvas.SetTop(element, dropPosition.Y);
                                        //    element.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                                        //    element.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                                        //}

                                        ActiveLayer.Children.Add(element);

                                        var settings = EditorComponent.SymbolGallery.GetCurrentDropSettings();
                                        var provider = EditorComponent.SymbolGallery.GetCurrentSourceSymbolProvider();
                                        var path = EditorComponent.SymbolGallery.GetCurrentSourceSymbolPath();

                                        if (element is FrameworkElement)
                                        {
                                            var fe = element as FrameworkElement;
                                            bool bForceName = !String.IsNullOrEmpty(settings) ||
                                                              !String.IsNullOrEmpty(provider) ||
                                                              !String.IsNullOrEmpty(path);
                                            var map = Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, fe, bForceName, true);
                                            var renamed = new Dictionary<string, string>();
                                            var ret = Document.MergeDocument(settings, map, renamed: renamed);

                                            Document.LoadResources(this);
                                            // Document.ResetSourceProviderPath(fe);
                                            Document.SetSourceProviderPath(fe, provider, path, true);
                                            try
                                            {
                                                Document.LoadRepositoryItem(this, MainSurface, fe.Name, bSetSize: false);
                                            }
                                            catch (Exception ex)
                                            {
                                                EditorComponent.UIInterface.ShowError(ex.Message);
                                            }
                                            // Document.UpdateRepositoryItems(this, MainSurface, true); // case 10593

                                            if (Utilities.WPF.XmlHelper.CheckProblematicXamlWriter(fe, xamlData))
                                            {
                                                Document.SetProblematicXaml(fe, xamlData);
                                            }

                                            if (element is Panel)
                                                Document.RestoreProblematicXamlWriter(MainSurface, (element as Panel).Name, bUpdate: true);
                                            else
                                            {
                                                element.GetChildrenOfType<Panel>().ToList()
                                                    .ForEach(panel => Document.RestoreProblematicXamlWriter(MainSurface,
                                                        (element as FrameworkElement).Name, bUpdate: true));
                                            }
                                            Document.CleanProblematicXamlBags();

                                            if (bForceName)
                                                Document.UpdateRepositoryItems(this, MainSurface, true); // case 11656

                                            foreach (var name in ret)
                                            {
                                                Document.GetListInners(name).ForEach(I =>
                                                {
                                                    UIElement uie = Document.FindInnerControl(MainSurface, I);
                                                    if (uie == null)
                                                        return;

                                                    var entity = Document.MapScreenEntities[I];
                                                    entity.Entity = uie;
                                                    entity.Document = Document;
                                                });
                                            }

                                            if (ret != null)
                                            {
                                                ret.ForEach(elName =>
                                                {
                                                    Document.MapScreenEntities[elName].ReplaceEntityReferences(renamed, elName);
                                                });
                                            }
                                        }

                                        elementsToSelect.Add(element);
                                    }
                                }
                                else
                                {
                                    var element = new ContentControl();
                                    //if (MainSurface is Canvas)
                                    //{
                                    Canvas.SetLeft(element, dropPosition.X);
                                    Canvas.SetTop(element, dropPosition.Y);
                                    //    element.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                                    //    element.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                                    //}
                                    //else
                                    //{
                                    //    Canvas.SetLeft(element, dropPosition.X);
                                    //    Canvas.SetTop(element, dropPosition.Y);
                                    //    element.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                                    //    element.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                                    //}

                                    ActiveLayer.Children.Add(element);

                                    var settings = EditorComponent.SymbolGallery.GetCurrentDropSettings();
                                    var provider = EditorComponent.SymbolGallery.GetCurrentSourceSymbolProvider();
                                    var path = EditorComponent.SymbolGallery.GetCurrentSourceSymbolPath();
                                    var code = EditorComponent.SymbolGallery.GetCurrentSourceSymbolCode();

                                    bool bForceName = !String.IsNullOrEmpty(settings) ||
                                                        !String.IsNullOrEmpty(provider) ||
                                                        !String.IsNullOrEmpty(path);
                                    // element.Name = path;
                                    var fe = element as FrameworkElement;
                                    var map = Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, element, bForceName, false);
                                    var renamed = new Dictionary<string, string>();
                                    var ret = Document.MergeDocument(settings, map, fe.Name, code, renamed);

                                    Document.LoadResources(this);
                                    Document.SetSourceProviderPath(element, provider, path);
                                    try
                                    {
                                        Document.LoadRepositoryItem(this, MainSurface, element.Name, bSetSize: true);
                                    }
                                    catch (Exception ex)
                                    {
                                        EditorComponent.UIInterface.ShowError(ex.Message);
                                    }

                                    if (bForceName)
                                        Document.UpdateRepositoryItems(this, MainSurface, true); // case 11656

                                // element.SetResourceReference(ContentControl.ContentProperty, element.Name);

                                //var bindingWidth = new Binding()
                                //{
                                //    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UIElement), 1),
                                //    Path = new PropertyPath("ActualWidth")
                                //};
                                //var bindingHeight = new Binding()
                                //{
                                //    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UIElement), 1),
                                //    Path = new PropertyPath("ActualHeight")
                                //};

                                //var fe = (element.Content as FrameworkElement);
                                //element.Width = fe.Width;
                                //element.Height = fe.Height;
                                //fe.SetBinding(FrameworkElement.WidthProperty, bindingWidth);
                                //fe.SetBinding(FrameworkElement.HeightProperty, bindingHeight);

                                //element.SetBinding(ContentControl.ContentProperty,
                                //    new Binding()
                                //    {
                                //        Source = new StaticResourceExtension(element.Name)
                                //    });

                                elementsToSelect.Add(element);

                                    Document.GetListElementsUsingProviderPath(provider, path).ForEach(name =>
                                    {
                                        if (name != element.Name)
                                        {
                                            var el = Document.FindInnerControl(MainSurface, name);
                                            // var el = MainSurface.FindName(name) as FrameworkElement;
                                            if (el != null)
                                                Document.ResolveEntityBrushAndPen(MainSurface, el);
                                        }
                                    });
                                    foreach (var name in ret)
                                    {
                                        Document.GetListInners(name).ForEach(I =>
                                        {
                                            UIElement uie = Document.FindInnerControl(MainSurface, I);
                                            if (uie == null)
                                                return;

                                            var entity = Document.MapScreenEntities[I];
                                            entity.Entity = uie;
                                            entity.Document = Document;
                                            UpdateHMIControl(uie, element);
                                        });
                                    }

                                    if (ret != null)
                                    {
                                        ret.ForEach(elName =>
                                        {
                                            Document.MapScreenEntities[elName].ReplaceEntityReferences(renamed, elName);
                                        });
                                    }
                            }
                        }

                        //now that we're done, we select
                        ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                        MainSurface_SetCurrentSelection(readonlyList);

                        OnExecuteDroppingCode(this, null);
                        if (bDroppingCodeCancelled)
                            OnDeleteEvent(this, null);
                    }

                    e.Handled = true;
                    }

            }
        }

        private void MainSurface_Drop(object sender, DragEventArgs e)
        {
            if (bSetZOrderInProgress)
                return;

            using (var Cursor = new WaitCursor())
            {
                SelectedElementsMap.Values.ToList().ForEach(ad =>
                {
                    ad.SetDraggable(true, true);
                });

                if (e.Data.GetDataPresent(typeof(RecordDragDropData)))
                {
                    MainSurface_Drop_New(sender, e);
                    return;
                }

                FrameworkElement targetElement = sender as FrameworkElement;
                Point dropPosition = e.GetPosition(ActiveLayer);
                AdjustPointToGrid(ref dropPosition);

                EditorComponent.Workspace.ActivateDockedElement(this);
                Focusable = true;
                Focus();

                //see if ISF is present in the drag and drop IDataObject
                if (e.Data.GetDataPresent(typeof(Uri)))
                {
                    Dispatcher.InvokeIfRequired(
                    (Action)delegate
                    {
                        Uri data = e.Data.GetData(typeof(Uri)) as Uri;

                        // var projectUri = new Uri(Document.Parent.rootBase, UriKind.RelativeOrAbsolute);
                        data = Document.Parent.MakeRelativeUri(data);

                        bool bCancelEdit = true;
                        BeginEdit();

                        List<UIElement> listSelected = MainSurface_GetCurrentSelection();
                        if (listSelected.Count == 0)
                        {
                            // CreateObjectFromSelectedType(dropPosition, data);
                            listSelected = MainSurface_GetCurrentSelection();
                        }

                        for (int i = listSelected.Count - 1; i >= 0; --i)
                        {
                            var uie = listSelected[i];
                            var bDropManagerHandled = false;
                            try
                            {
                                var method = uie.GetType().GetMethod("DropManager");
                                if (method != null)
                                {
                                    bDropManagerHandled = (bool)(method.Invoke(uie, new object[] { data }));
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                            if (bDropManagerHandled)
                                bCancelEdit = false;
                            //if (uie is ScreenManager.SpecialObjects.EmbeddedTabScreen && 
                            //    data.GetPathString().Contains(".xaml"))
                            //{
                            //    var screen = data.GetPathString();
                            //    var embedded = uie as ScreenManager.SpecialObjects.EmbeddedTabScreen;
                            //    if (embedded.Screens == null || !embedded.Screens.Contains(screen))
                            //    {
                            //        var list = new ScreenManager.SpecialObjects.ScreenList();
                            //        if (embedded.Screens != null)
                            //            list.AddRange(embedded.Screens);
                            //        list.Add(screen);
                            //        embedded.Screens = list;

                            //        bCancelEdit = false;
                            //    }
                            //    else
                            //    {
                            //        bCancelEdit = true;
                            //        break;
                            //    }
                            //}
                            else
                            {
                                var entity = GetSelectedEntity(uie as FrameworkElement);
                                if (!entity.OnDropUri(data))
                                {
                                    bCancelEdit = true;
                                    break;
                                }
                                else
                                    UpdateStringIdAndTranslate(uie);
                            }

                            bCancelEdit = false;
                        }

                        if (bCancelEdit)
                            CancelEdit();
                        else
                            EndEdit();
                    }, DispatcherPriority.ApplicationIdle);

                    e.Handled = true;
                }
                else if (e.Data.GetDataPresent(typeof(ObservableCollection<MonitoredItemViewModel>)))
                {
                    Dispatcher.InvokeIfRequired(
                    (Action)delegate
                    {
                        var data = e.Data.GetData(typeof(ObservableCollection<MonitoredItemViewModel>)) as ObservableCollection<MonitoredItemViewModel>;

                        List<UIElement> listSelected = MainSurface_GetCurrentSelection();
                        if (listSelected.Count == 0)
                        {
                            if (EditorComponent.ToolBox == null || EditorComponent.ToolBox.ActiveToolCode == null)
                            {
                                EditorComponent.UIInterface.ShowError(Properties.Resources.ToolboxSelectionNeeded);
                            }
                            else
                            {
                                var listToSelect = new List<UIElement>();
                                var toolselected = EditorComponent.ToolBox.ActiveToolCode;
                                using (new ChangeScreenWatcher(this, true, UndoRedoManager.UndoAction.Added))
                                {
                                    MainSurface.BeginInit();

                                    EditorComponent.Workspace.IsBusy = true;
                                    using (var cursor = new WaitCursor())
                                    {
                                        try
                                        {
                                            int i = 0;
                                            foreach (var v in data)
                                            {
                                                EditorComponent.Workspace.BusyContent = String.Format(Properties.Resources.CreatingObjectFromToolbox, ++i, data.Count);
                                                EditorComponent.ToolBox.ActiveToolCode = toolselected;
                                                AddToolBoxDataObject(dropPosition);
                                                var element = MainSurface_GetCurrentSelection();
                                                if (element.Count > 0)
                                                {
                                                    var entity = GetSelectedEntity(element[0] as FrameworkElement);
                                                    entity.OnDropReference(v.referenceItem);

                                                    listToSelect.AddRange(element);
                                                }
                                            }
                                        }
                                        finally
                                        {
                                            EditorComponent.Workspace.IsBusy = false;
                                            MainSurface.EndInit();
                                        }
                                    }
                                }
                                ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(listToSelect);
                                MainSurface_SetCurrentSelection(readonlyList);
                                if (listToSelect.Count > 1)
                                {
                                    // OnDistributeSpace(this, null);
                                    Dispatcher.BeginInvokeAsynchronouslyInRender(() =>
                                            OnDistributeSpace(this, null));
                                }
                            }
                        }
                        else
                        {
                            bool bCancelEdit = true;
                            BeginEdit();
                            foreach (var v in data)
                            {
                                var model = v as MonitoredItemViewModel;
                                for (int i = listSelected.Count - 1; i >= 0; --i)
                                {
                                    var uie = listSelected[i];
                                    var entity = GetSelectedEntity(uie as FrameworkElement);
                                    if (!entity.OnDropReference(model.referenceItem))
                                    {
                                        bCancelEdit = true;
                                        break;
                                    }

                                    bCancelEdit = false;
                                }
                            }

                            if (bCancelEdit)
                                CancelEdit();
                            else
                                EndEdit();
                        }

                    }, DispatcherPriority.ApplicationIdle);
                    e.Handled = true;
                }
                else if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    String[] filenames = e.Data.GetData(DataFormats.FileDrop) as String[];
                    Visual mp = null;
                    Uri uriFound = null;
                    if (filenames.Length > 0)
                    {
                        var dropHelper = new DropFileHelper(Document, uriToUriAbsoluteImageConverter, EditorComponent.UIInterface, SourceFileCopyOption.Ask);
                        foreach (var file in filenames)
                        {
                            try
                            {
                                mp = dropHelper.DropFile(file, out uriFound);
                                break;
                            }
                            catch (OperationCanceledException)
                            {
                                return;
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                    }

                    if (mp != null)
                    {
                        VisualBrush db = new VisualBrush(mp);

                        List<UIElement> listSelected = MainSurface_GetCurrentSelection();

                        using (new ChangeScreenWatcher(this, true, UndoRedoManager.UndoAction.Changed))
                        {
                            // var result = VisualTreeHelper.HitTest(ActiveLayer, dropPosition);
                            SetSelectedBrush(db, null, false);
                        }
                    }

                    e.Handled = true;
                }
                else if (e.Data.GetDataPresent(DataFormats.Xaml) ||
                    e.Data.GetDataPresent(DataFormats.Text))
                {
                    List<UIElement> elementsToSelect = new List<UIElement>();
                    SetDragDropEffects(e);

                    if (e.Effects == DragDropEffects.Move || e.Effects == DragDropEffects.Copy)
                    {
                        using (new ChangeScreenWatcher(this, true, UndoRedoManager.UndoAction.Added))
                        {
                            //after we drop, we need to select the elements and strokes that were
                            //copied into the Canvas
                            if (e.Data.GetDataPresent(DataFormats.Xaml) ||
                                e.Data.GetDataPresent(DataFormats.Text))
                            {
                                //paste Xaml
                                string xamlData = "";
                                if (e.Data.GetDataPresent(DataFormats.Xaml))
                                    xamlData = e.Data.GetData(DataFormats.Xaml) as String;
                                else if (e.Data.GetDataPresent(DataFormats.Text))
                                    xamlData = e.Data.GetData(DataFormats.Text) as String;

                                if (!String.IsNullOrEmpty(xamlData))
                                {
                                    UIElement element = null;
                                    try
                                    {
                                        element = xamlData.ReadUIElement();
                                    }
                                    catch (Exception ex)
                                    {

                                    }

                                    if (element != null)
                                    {
                                        element.IsHitTestVisible = true;
                                        //just add the element, it wasn't another Canvas that we
                                        //added to the dataobject as a container
                                        //if (MainSurface is Canvas)
                                        //{
                                        Canvas.SetLeft(element, dropPosition.X);
                                        Canvas.SetTop(element, dropPosition.Y);
                                        //    element.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                                        //    element.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                                        //}
                                        //else
                                        //{
                                        //    Canvas.SetLeft(element, dropPosition.X);
                                        //    Canvas.SetTop(element, dropPosition.Y);
                                        //    element.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                                        //    element.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                                        //}

                                        ActiveLayer.Children.Add(element);

                                        var settings = EditorComponent.SymbolGallery.GetCurrentDropSettings();
                                        var provider = EditorComponent.SymbolGallery.GetCurrentSourceSymbolProvider();
                                        var path = EditorComponent.SymbolGallery.GetCurrentSourceSymbolPath();

                                        if (element is FrameworkElement)
                                        {
                                            var fe = element as FrameworkElement;
                                            bool bForceName = !String.IsNullOrEmpty(settings) ||
                                                              !String.IsNullOrEmpty(provider) ||
                                                              !String.IsNullOrEmpty(path);
                                            var map = Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, fe, bForceName, true);
                                            var renamed = new Dictionary<string, string>();
                                            var ret = Document.MergeDocument(settings, map, renamed: renamed);

                                            Document.LoadResources(this);
                                            // Document.ResetSourceProviderPath(fe);
                                            Document.SetSourceProviderPath(fe, provider, path, true);
                                            try
                                            {
                                                Document.LoadRepositoryItem(this, MainSurface, fe.Name, bSetSize: false);
                                            }
                                            catch (Exception ex)
                                            {
                                                EditorComponent.UIInterface.ShowError(ex.Message);
                                            }
                                            // Document.UpdateRepositoryItems(this, MainSurface, true); // case 10593

                                            if (Utilities.WPF.XmlHelper.CheckProblematicXamlWriter(fe, xamlData))
                                            {
                                                Document.SetProblematicXaml(fe, xamlData);
                                            }

                                            if (element is Panel)
                                                Document.RestoreProblematicXamlWriter(MainSurface, (element as Panel).Name, bUpdate: true);
                                            else
                                            {
                                                element.GetChildrenOfType<Panel>().ToList()
                                                    .ForEach(panel => Document.RestoreProblematicXamlWriter(MainSurface,
                                                        (element as FrameworkElement).Name, bUpdate: true));
                                            }
                                            Document.CleanProblematicXamlBags();

                                            if (bForceName)
                                                Document.UpdateRepositoryItems(this, MainSurface, true); // case 11656

                                            foreach (var name in ret)
                                            {
                                                Document.GetListInners(name).ForEach(I =>
                                                {
                                                    UIElement uie = Document.FindInnerControl(MainSurface, I);
                                                    if (uie == null)
                                                        return;

                                                    var entity = Document.MapScreenEntities[I];
                                                    entity.Entity = uie;
                                                    entity.Document = Document;
                                                });
                                            }

                                            if (ret != null)
                                            {
                                                ret.ForEach(elName =>
                                                {
                                                    Document.MapScreenEntities[elName].ReplaceEntityReferences(renamed, elName);
                                                });
                                            }
                                        }

                                        elementsToSelect.Add(element);
                                    }
                                }
                                else
                                {
                                    var element = new ContentControl();
                                    //if (MainSurface is Canvas)
                                    //{
                                    Canvas.SetLeft(element, dropPosition.X);
                                    Canvas.SetTop(element, dropPosition.Y);
                                    //    element.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                                    //    element.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                                    //}
                                    //else
                                    //{
                                    //    Canvas.SetLeft(element, dropPosition.X);
                                    //    Canvas.SetTop(element, dropPosition.Y);
                                    //    element.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                                    //    element.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                                    //}

                                    ActiveLayer.Children.Add(element);

                                    var settings = EditorComponent.SymbolGallery.GetCurrentDropSettings();
                                    var provider = EditorComponent.SymbolGallery.GetCurrentSourceSymbolProvider();
                                    var path = EditorComponent.SymbolGallery.GetCurrentSourceSymbolPath();
                                    var code = EditorComponent.SymbolGallery.GetCurrentSourceSymbolCode();

                                    bool bForceName = !String.IsNullOrEmpty(settings) ||
                                                        !String.IsNullOrEmpty(provider) ||
                                                        !String.IsNullOrEmpty(path);
                                    // element.Name = path;
                                    var fe = element as FrameworkElement;
                                    var map = Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, element, bForceName, false);
                                    var renamed = new Dictionary<string, string>();
                                    var ret = Document.MergeDocument(settings, map, fe.Name, code, renamed);

                                    Document.LoadResources(this);
                                    Document.SetSourceProviderPath(element, provider, path);
                                    try
                                    {
                                        Document.LoadRepositoryItem(this, MainSurface, element.Name, bSetSize: true);
                                    }
                                    catch (Exception ex)
                                    {
                                        EditorComponent.UIInterface.ShowError(ex.Message);
                                    }

                                    if (bForceName)
                                        Document.UpdateRepositoryItems(this, MainSurface, true); // case 11656

                                    //Document.SubscribePropertyChangeXamlWriterProperties(element, element.Name);
                                    (element.Content as UIElement).IsHitTestVisible = false;
                                    //Document.UnsubscribePropertyChangeXamlWriterProperties(element);
#if !WINDOWS_UWP
                                    Document.UpdateXamlWriterProperty(element, element.Name, UIElement.IsHitTestVisibleProperty);
#endif

                                    // element.SetResourceReference(ContentControl.ContentProperty, element.Name);

                                    //var bindingWidth = new Binding()
                                    //{
                                    //    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UIElement), 1),
                                    //    Path = new PropertyPath("ActualWidth")
                                    //};
                                    //var bindingHeight = new Binding()
                                    //{
                                    //    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UIElement), 1),
                                    //    Path = new PropertyPath("ActualHeight")
                                    //};

                                    //var fe = (element.Content as FrameworkElement);
                                    //element.Width = fe.Width;
                                    //element.Height = fe.Height;
                                    //fe.SetBinding(FrameworkElement.WidthProperty, bindingWidth);
                                    //fe.SetBinding(FrameworkElement.HeightProperty, bindingHeight);

                                    //element.SetBinding(ContentControl.ContentProperty,
                                    //    new Binding()
                                    //    {
                                    //        Source = new StaticResourceExtension(element.Name)
                                    //    });

                                    elementsToSelect.Add(element);

                                    Document.GetListElementsUsingProviderPath(provider, path).ForEach(name =>
                                    {
                                        if (name != element.Name)
                                        {
                                            var el = Document.FindInnerControl(MainSurface, name);
                                            // var el = MainSurface.FindName(name) as FrameworkElement;
                                            if (el != null)
                                                Document.ResolveEntityBrushAndPen(MainSurface, el);
                                        }
                                    });
                                    foreach (var name in ret)
                                    {
                                        Document.GetListInners(name).ForEach(I =>
                                        {
                                            UIElement uie = Document.FindInnerControl(MainSurface, I);
                                            if (uie == null)
                                                return;

                                            var entity = Document.MapScreenEntities[I];
                                            entity.Entity = uie;
                                            entity.Document = Document;
                                            UpdateHMIControl(uie, element);
                                        });
                                    }

                                    if (ret != null)
                                    {
                                        ret.ForEach(elName =>
                                        {
                                            Document.MapScreenEntities[elName].ReplaceEntityReferences(renamed, elName);
                                        });
                                    }
                                }
                            }

                            //now that we're done, we select
                            ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                            MainSurface_SetCurrentSelection(readonlyList);

                            OnExecuteDroppingCode(this, null);
                            if (bDroppingCodeCancelled)
                                OnDeleteEvent(this, null);
                        }

                        e.Handled = true;
                    }
                }
            }
        }

        #endregion Drag and Drop

        bool bRequestResize = false;
        double previousWidth = Double.NaN;
        double previousHeight = Double.NaN;
        string searchString;

        private void OnSearchStringChanged(object sender, SearchStringToFilterCriteriaEventArgs e)
        {
            if (searchOperator == null)
                UpdateSearchFilterCriteria();
            if (searchOperator != null)
            {
                bool bRefresh = searchOperator.SearchString != e.SearchString;
                searchOperator.SearchString = e.SearchString;
                e.Filter = searchOperator.FilterCriteria;
                if (bRefresh)
                    referenceListTree?.RefreshData();
            }
        }

        private void OnFilterChanged(object sender, RoutedEventArgs e)
        {
            UpdateSearchResult();
        }

        void UpdateSearchFilterCriteria()
        {
            if (referenceListTree != null)
            {
                if (IsSearchCaseSensitive)
                    searchOperator = TagListSearchOperatorCaseSensitive.Register();
                else
                    searchOperator = TagListSearchOperatorCaseInsensitive.Register();

                if (searchOperator != null)
                {
                    var columnNames = new List<String>();
                    if (IsFullSearchAllowed)
                    {
                        foreach (var column in referenceListTree.Columns)
                            columnNames.Add(column.FieldName);
                    }
                    else
                        columnNames.Add("StringRepresentation");
                    searchOperator.SetCriteriaOperator(columnNames.ToArray());
                    
                    if (referenceListTree.View.SearchControl != null)
                        referenceListTree.View.SearchControl.FilterCriteria = searchOperator.FilterCriteria;
                }
            }
        }

        void UpdateSearchResult()
        {
            //if (referenceListTree == null || String.IsNullOrEmpty(referenceListTree.View.SearchString?.Trim()))
            //    SearchResultsCount = String.Empty;
            //else
            if (referenceListTree != null)
                SearchResultsCount = String.Format(Properties.Resources.SearchItemsFound, referenceListTree.VisibleItems.Count);

            searchString = SearchResultsCount;
        }

        void ResizeContent()
        {
            if (!bRequestResize || MainSurface.Children.Count == 0 || Double.IsNaN(Document.Width) || Double.IsNaN(Document.Height) ||
                Double.IsNaN(previousWidth) || Double.IsNaN(previousHeight) ||
                previousWidth == Document.Width && previousHeight == Document.Height)
            {
                bRequestResize = false;
                return;
            }
            bRequestResize = false;
            if (EditorComponent.UIInterface.ShowYesNo(Properties.Resources.AskResizeChildren, CustomDialogIcons.Question) == CustomDialogResults.No)
                return;

            var widthRatio = previousWidth / Document.Width;
            var heightRatio = previousHeight / Document.Height;

            var widthRatioInverse = Document.Width / previousWidth;
            var heightRatioInverse = Document.Height / previousHeight;
            using (new ChangeScreenWatcher(this))
            {
                foreach (FrameworkElement element in MainSurface.Children)
                {
                    var bSub = Document.IsSubscribePropertyChangeXamlWriterProperties(element);
                    if (!bSub)
                        Document.SubscribePropertyChangeXamlWriterProperties(element, GetSelectedEntityName(element));

                    var factor = Canvas.GetTop(element) / heightRatio;
                    Canvas.SetTop(element, factor);
                    factor = Canvas.GetLeft(element) / widthRatio;
                    Canvas.SetLeft(element, factor);

                    factor = element.Height / heightRatio;
                    element.Height = factor;
                    factor = element.Width / widthRatio;
                    element.Width = factor;

                    TransformsPoints(element, widthRatioInverse, heightRatioInverse);

                    if (!bSub)
                        Document.UnsubscribePropertyChangeXamlWriterProperties(element);
                }
            }

            UpdateScrollerSize();
        }

        private void OnEnableManipulation(object sender, ExecutedRoutedEventArgs e)
        {
            SelectedElementsMap.Keys.ToList().ForEach(key =>
            {
                var element = key as FrameworkElement;
                var entity = GetSelectedEntity(element);
                entity.EnableManipulation = !entity.EnableManipulation;
            });
        }

        private void OnEnable3D(object sender, ExecutedRoutedEventArgs e)
        {
            enable3DCameraBI.IsVisible = (bool)enable3DBI.IsChecked;
            SelectedElementsMap.Values.ToList().ForEach(ab =>
            {
                ab.Update3DragControl((bool)enable3DBI.IsChecked);
            });
        }

        private void OnEnable3DCamera(object sender, ExecutedRoutedEventArgs e)
        {
            edit3DCameraPositionsBI.IsVisible = (bool)enable3DCameraBI.IsChecked;
            RestoreBeginEdit3DModels();
            SelectedElementsMap.Values.ToList().ForEach(ab =>
            {
                ab.Update3DCamera((bool)enable3DCameraBI.IsChecked);
            });
            RestoreAfterBeginEdit3DModels();
        }

        private void OnEdit3DCameraPositions(object sender, ExecutedRoutedEventArgs e)
        {
            SelectedElementsMap.Values.ToList().ForEach(ab =>
            {
                ab.Update3DCameraPosition();
            });
        }
        
        void CanEnable3D(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedElementsMap.Count > 0;
        }

        void CanEnable3DCamera(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedElementsMap.Count > 0;
        }

        void CanEdit3DCameraPositions(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedElementsMap.Count > 0;
        }

        private void cm_Opened(object sender, EventArgs e)
        {
            var menu = sender as DevExpress.Xpf.Bars.PopupMenu;
            if (sender == null)
                return;

            var listSelected = MainSurface_GetCurrentSelection();
            bool canShowRotatemenu = (from c in listSelected
                                      where c is Pipeline.Pipeline ||
                                            c is PolyBezier.PolyBezier ||
                                            c is System.Windows.Shapes.Line ||
                                            c is System.Windows.Shapes.Polygon ||
                                            c is System.Windows.Shapes.Polyline
                                      select c).ToList().Count == 0;
            rotationMenu.IsVisible = canShowRotatemenu;
            //menuExpression.IsVisible = listSelected.Count == 1 ? true : false;
            if (listSelected.Count == 1)
            {
                var fe = listSelected[0] as FrameworkElement;
                var entity = GetSelectedEntity(fe);
                bool bSet = fe.CacheMode != null;
                menu.DataContext = entity;
                //chacheMode.IsChecked = bSet;
                lockMode.IsChecked = entity.LockMovement;
                entityManipulationBI.IsChecked = entity.EnableManipulation;
                entityNameBI.Content = entity.EntityName.Replace("_", "__");
            }
            else if (listSelected.Count == 0)
                menu.DataContext = null;

            UpdateBarButtonLinkVisibility(listSelected);
        }

        private void Click_ClearScreen(object sender, RoutedEventArgs e)
        {
            BeginEdit();
            (sender as Button).Tag = null;
            EndEdit();
            MainSurface_SetModified(true);
        }

        private void Click_BrowseScreen(object sender, RoutedEventArgs e)
        {
            BeginEdit();
            var uri = ShowScreenSelector();
            if (uri != null)
            {
                (sender as Button).Tag = uri;
                EndEdit();
                MainSurface_SetModified(true);
            }
            else
                CancelEdit();
        }

        Uri ShowScreenSelector(String resourceType = "ScreenManager")
        {
            var doc = EditorComponent.Workspace.ContextDocument;
            if (doc == null)
                return null;
            var control = EditorComponent.ProjectManager.GetResourcePickerUserControl(doc, resourceType);
            var Dialog = new GeneralDialogContent(control)
            {
                Owner = this.FindParent<Window>(),
                HelpLink = "ScreenSelector"
            };
            Dialog.DialogKeepContent = true;
            if (Dialog.ShowDialog() == true)
            {
                var uri = EditorComponent.ProjectManager.GetResourcePickerUserControlUri(control);
                var urirelative = doc.MakeRelativeUri(uri);
                return urirelative;
            }

            return null;
        }

        private void Click_ClearParameter(object sender, RoutedEventArgs e)
        {
            BeginEdit();
            (sender as Button).Tag = null;
            EndEdit();
            MainSurface_SetModified(true);
        }

        private void Click_BrowseParameter(object sender, RoutedEventArgs e)
        {
            BeginEdit();
            var uri = ShowScreenSelector("ScreenParametersEditor");
            if (uri != null)
            {
                (sender as Button).Tag = uri.ToString();
                EndEdit();
                MainSurface_SetModified(true);
            }
            else
                CancelEdit();
        }

        private void UpdateBarButtonLinkVisibility(List<UIElement> listselected)
        {
            bool bShowItem = listselected?.Count == 1;
            //entityCommandsBI.IsVisible = entityAnimationsBI.IsVisible = bShowItem;
            entityTagBI.IsVisible = entityNameBI.IsVisible = bShowItem;
            edit3dInnerBI.IsVisible = false;
            if (!bShowItem)
            {
                //entityItemSourceBI.IsVisible = false;
                edit3dBI.IsVisible = false;
                enable3DBI.IsVisible = false;
                enable3DCameraBI.IsVisible = false;
                edit3DCameraPositionsBI.IsVisible = false;
                executeCode.IsVisible = false;
            }
            else
            {
                UIElement subElement = listselected[0];
                var entity = GetSelectedEntity((FrameworkElement)subElement);
                if (entity.SourceSymbolLinked &&
                    subElement is ContentControl && !(subElement is UserControl) && (subElement as ContentControl).Content is FrameworkElement)
                {
                    subElement = (subElement as ContentControl).Content as FrameworkElement;
                }
                //Type t = subElement.GetType();
                //if (t.GetProperty("ItemsSource") != null || t.GetProperty("DataSource") != null)
                //    entityItemSourceBI.IsVisible = bShowItem;
                //else
                //    entityItemSourceBI.IsVisible = !bShowItem;

                var adornedElement = listselected[0];
                var listModel3Ds = subElement.GetChildrenOfType<Viewport3D>().ToList();
                if (listModel3Ds.Count == 0 && adornedElement is Viewport3D)
                    listModel3Ds.Add(adornedElement as Viewport3D);
                bool isVisible3D = listModel3Ds.Count > 0;
                edit3dBI.IsVisible = isVisible3D;
                enable3DBI.IsVisible = isVisible3D;
                enable3DCameraBI.IsVisible = isVisible3D && (bool)enable3DBI.IsChecked;
                executeCode.IsVisible = blastSelectedCanExecuteDrop;
                edit3DCameraPositionsBI.IsVisible = isVisible3D && (bool)enable3DBI.IsChecked && (bool)enable3DCameraBI.IsChecked;


                if (listActive3dModels.Count > 0)
                {
                    var model = listActive3dModels.First();
                    if(model != null)
                    {
                        var hash = Utilities.WPF.DependencyObjectExtensions.RegisterModel3D(MainSurface, subElement as FrameworkElement, model);
                        entity = entity.AddOrFind3DInnerModel(model, hash);
                        entity.Entity3D = model;
                        inner3dParameterBI.DataContext = inner3dUriBI.DataContext = entity;
                        edit3dInnerBI.IsVisible = model != null;
                    }
                }
            }
        }

        private void OnRename(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();
            var fe = listSelected[0] as FrameworkElement;
            var name = fe.Name;

            var renameUserControl = new ScreenManager.Popups.RenameUserControl();
            renameUserControl.txtName.Text = name;
            while (true)
            {
                var Dialog = new GeneralDialogContent(renameUserControl)
                {
                    Owner = this.FindParent<Window>(),
                    Title = Properties.Resources.RenameTitle,
                    HelpLink = "RenameControl",
                };
                if (Dialog.ShowDialog() != true)
                    break;

                name = renameUserControl.txtName.Text;
                if (String.IsNullOrEmpty(name))
                    continue;
                if (name == fe.Name)
                    break;
                if (MainSurface.FindName(name) != null)
                    EditorComponent.UIInterface.ShowError(Properties.Resources.NameAlreadyInUse);
                else
                {
                    bool bInvalid = false;
                    if (name.Length > 0 && Char.IsDigit(name[0]))
                        bInvalid = true;
                    else
                    {
                        for (int index = 0; index < name.Length; index++)
                        {
                            // search for any non letter character from input string
                            if (!Char.IsLetter(name[index]) && !Char.IsDigit(name[index]) && name[index] != '_')
                            {
                                bInvalid = true;
                                break;
                            }
                        }
                    }

                    if (bInvalid)
                    {
                        EditorComponent.UIInterface.ShowError(Properties.Resources.NameInvalid);
                        continue;
                    }

                    Document.RenameControl(MainSurface, fe.Name, name);
                    Utilities.WPF.DependencyObjectExtensions.UnregisterName(MainSurface, fe);
                    fe.Name = name;
                    Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, fe, true, false);
                    MainSurface_SetModified(true);
                    break;
                }
            }
        }

        private void CanExecuteRename(object sender, CanExecuteRoutedEventArgs e)
        {
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();
            e.CanExecute = listSelected.Count == 1 && MainSurface.Children.Contains(listSelected[0]);
        }

        private void CanExecuteZoomIn(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnZoomIn(object sender, ExecutedRoutedEventArgs e)
        {
            ZoomLevelX = ZoomLevelX + 0.1;
            ZoomLevelY = ZoomLevelY + 0.1;
        }

        private void CanExecuteZoomOut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnZoomOut(object sender, ExecutedRoutedEventArgs e)
        {
            ZoomLevelX = ZoomLevelX - 0.1;
            ZoomLevelY = ZoomLevelY - 0.1;
        }

        void TranslateShapePoints()
        {
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();
            var list = (from c in listSelected.OfType<Shape>()
                        where c is Polygon || c is Line || c is Polyline || c is Pipeline.Pipeline || c is PolyBezier.PolyBezier
                        select c).ToList();
            var mapTransform = new Dictionary<Shape, Transform>();
            list.ForEach(shape => mapTransform.Add(shape, shape.RenderTransform));
            MainSurface_CleanCurrentSelectedion();
            list.ForEach(shape =>
                {
                    shape.RenderTransform = mapTransform[shape];

                    var points = new PointCollection();
                    if (shape is Pipeline.Pipeline)
                    {
                        var pipeline = shape as Pipeline.Pipeline;
                        points = pipeline.Points;
                    }
                    else if (shape is PolyBezier.PolyBezier)
                    {
                        var polybezier = shape as PolyBezier.PolyBezier;
                        points = polybezier.Points;
                    }
                    else if (shape is Polyline)
                    {
                        var polyline = shape as Polyline;
                        points = polyline.Points;
                    }
                    else if (shape is Polygon)
                    {
                        var polygon = shape as Polygon;
                        points = polygon.Points;
                    }
                    else if (shape is Line)
                    {
                        var line = shape as Line;

                        points.Add(new Point(line.X1, line.Y1));
                        points.Add(new Point(line.X2, line.Y2));
                    }

                    var top = Canvas.GetTop(shape);
                    var left = Canvas.GetLeft(shape);
                    Canvas.SetTop(shape, 0);
                    Canvas.SetLeft(shape, 0);
                    shape.UpdateLayout();

                    var pointsTranslated = new PointCollection();
                    var transform = shape.TransformToVisual(MainSurface);
                    foreach (var point in points)
                    {
                        var pt = transform.Transform(point);
                        pointsTranslated.Add(pt);
                    }

                    shape.RenderTransform = null;

                    if (shape is Pipeline.Pipeline)
                    {
                        var pipeline = shape as Pipeline.Pipeline;
                        pipeline.Points = pointsTranslated;
                    }
                    else if (shape is PolyBezier.PolyBezier)
                    {
                        var polybezier = shape as PolyBezier.PolyBezier;
                        polybezier.Points = pointsTranslated;
                    }
                    else if (shape is Polyline)
                    {
                        var polyline = shape as Polyline;
                        polyline.Points = pointsTranslated;
                    }
                    else if (shape is Polygon)
                    {
                        var polygon = shape as Polygon;
                        polygon.Points = pointsTranslated;
                    }
                    else if (shape is Line)
                    {
                        var line = shape as Line;

                        line.X1 = pointsTranslated[0].X;
                        line.Y1 = pointsTranslated[0].Y;
                        line.X2 = pointsTranslated[1].X;
                        line.Y2 = pointsTranslated[1].Y;
                    }
                    Canvas.SetTop(shape, top);
                    Canvas.SetLeft(shape, left);
                });
            ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(listSelected);
            MainSurface_SetCurrentSelection(readonlyList);
        }

        void OnRotate(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (e.Parameter != null)
                UpdateRotation(Convert.ToDouble(e.Parameter));
            else
                UpdateRotation();
        }
        void UpdateRotation(double? value = null)
        {
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();

            using (new ChangeScreenWatcher(this))
            {
                var mapTransforms = new Dictionary<UIElement, Transform>();
                var elementsToSelect = new List<UIElement>();
                listSelected.ForEach(ue =>
                {
                    RotateTransform t = Utilities.Animations.Animations.SetTransform<RotateTransform>(ue, true, true);
                    double tTargetAngle = t.Angle;
                    if (value == null)
                        tTargetAngle = t.Angle + 90;
                    else
                        tTargetAngle = Convert.ToDouble(value);

                    DoubleAnimation da = new DoubleAnimation()
                    {
                        To = tTargetAngle,
                        Duration = new Duration(TimeSpan.FromMilliseconds(nAnimationTime)),
                        EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut }
                    };

                    da.Completed += (o, u) =>
                    {
                        t.BeginAnimation(RotateTransform.AngleProperty, null);
                        t.Angle = tTargetAngle;
                        mapTransforms.Add(ue, ue.RenderTransform);

                        elementsToSelect.Add(ue);

                        System.Windows.Input.CommandManager.InvalidateRequerySuggested();

                        if (elementsToSelect.Count == listSelected.Count)
                        {
                            MainSurface_CleanCurrentSelectedion();
                            ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                            MainSurface_SetCurrentSelection(readonlyList);
                            mapTransforms.Keys.ToList().ForEach(uie => uie.RenderTransform = mapTransforms[uie]);
                            TranslateShapePoints();
                        }
                    };
                    t.BeginAnimation(RotateTransform.AngleProperty, da);
                });
            }
        }
        bool bUpdatingRotation;
        private void MenuRotateValue_Changed(decimal value)
        {
            if (!bLoaded)
                return;
            if (bUpdatingRotation)
                return;

            bUpdatingRotation = true;
            using (new ChangeScreenWatcher(this))
            {
                List<UIElement> listSelected = MainSurface_GetCurrentSelection();
                listSelected.ForEach(ue =>
                {
                    RotateTransform t = Utilities.Animations.Animations.SetTransform<RotateTransform>(ue, true, true);
                    t.Angle = Convert.ToDouble(value);
                });
                TranslateShapePoints();
                bUpdatingRotation = false;
            }
        }

        private void textRot_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (!bChanging)
                //UpdateRotation(Convert.ToDouble(textRot.Value));
                MenuRotateValue_Changed((sender as DevExpress.Xpf.Editors.SpinEdit).Value);
        }
        bool bChanging;
        private void RotationValue_LostFocus(object sender, RoutedEventArgs e)
        {
            bChanging = false;
        }
        private void textRot_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Key.Enter != e.Key)
                bChanging = true;
            else
            {
                bChanging = false;
                //UpdateRotation(Convert.ToDouble(textRot.Value));
                MenuRotateValue_Changed((sender as DevExpress.Xpf.Editors.SpinEdit).Value);
            }
        }

        private void OnRotationControlLoaded(object sender, RoutedEventArgs e)
        {
            if (SelectedElementsMap.Count > 0)
            {
                var uie = SelectedElementsMap.Keys.First();
                var transform = uie.RenderTransform;
                double angle = 0;
                if (transform is TransformGroup)
                {
                    var angles = (from t in ((TransformGroup)uie.RenderTransform).Children where t is RotateTransform select ((RotateTransform)t).Angle).ToList();
                    foreach (var a in angles)
                        angle += a;
                }
                else if (transform is RotateTransform)
                    angle = ((RotateTransform)transform).Angle;
                bChanging = true;
                (sender as SpinEdit).Value = (decimal)AngleMod360(angle);
                bChanging = false;
            }
        }

        RelayCommand replaceTextCommand;
        public ICommand ReplaceTextCommand
        {
            get
            {
                if (replaceTextCommand == null)
                {
                    replaceTextCommand = new RelayCommand(
                        param =>
                        {
#if !WINDOWS_UWP
                            using (new AsyncWaitCursor())
                            {
                                Dictionary<string, string> tagNewNameMap = new Dictionary<string, string>();
                                var listRejected = new List<string>();
                                var map = Document.GetDynamicMapForElementAndChilds();
                                var opcRefMap = map.ToDictionary(k => k.Key, v => (from o in v.Value select o.ToXml()).ToList());
                                List<String> toUpdate = new List<String>();
                                var list = referenceListTree.VisibleItems;
                                int tagCount = 0;
                                for (int i = 0; i < list.Count; i++)
                                {
                                    string preChange = ((OPCUAEntityReference)list[i]).ToXml();
                                    OPCUAEntityReference tag = preChange.FromXml<OPCUAEntityReference>();
                                    var keyList = (from key in opcRefMap.Keys where opcRefMap[key].Contains(preChange) select key).ToList();
                                    keyList.ForEach(k => { if (!toUpdate.Contains(k)) toUpdate.Add(k); });
                                    string result = null;
                                    string searchString = referenceListTree.View.SearchString;
                                    string stringToChange = tag.StringRepresentation;
                                    bool isManyCharSearch = searchString.Contains(Properties.Settings.Default.MultipleCharPlaceHolder);
                                    int manyCharSearchCount = searchString.Split(Properties.Settings.Default.MultipleCharPlaceHolder).Length - 1;
                                    bool isSingleCharSearch = searchString.Contains(Properties.Settings.Default.OneCharPlaceHolder);
                                    int singleCharSearchCount = searchString.Split(Properties.Settings.Default.OneCharPlaceHolder).Length - 1;
                                    if (isManyCharSearch && manyCharSearchCount == 1)
                                    {
                                        result = ReplaceSearchString(searchString, stringToChange.Replace('\\', '/'), Properties.Settings.Default.MultipleCharPlaceHolder, param as String);
                                    }
                                    else if (isSingleCharSearch && singleCharSearchCount == 1)
                                    {
                                        result = ReplaceSearchString(searchString, stringToChange.Replace('\\', '/'), Properties.Settings.Default.OneCharPlaceHolder, param as String, true);
                                    }
                                    else
                                    {
                                        searchString = searchString.Replace('\\', '/');
                                        if (IsSearchCaseSensitive)
                                            result = System.Text.RegularExpressions.Regex.Replace(stringToChange.Replace('\\', '/'), searchString, param as String);
                                        else
                                            result = System.Text.RegularExpressions.Regex.Replace(stringToChange.Replace('\\', '/'), searchString, param as String, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                                    }

                                    if (tag.StringRepresentation != result)
                                    {
                                        if (!UpdateReference(result, tag, tagNewNameMap))
                                            listRejected.Add(result);
                                        else
                                            tagCount++;
                                    }
                                }

                                if (tagNewNameMap.Count > 0)
                                {
                                    toUpdate.ForEach(key =>
                                    {
                                        Document.MapScreenEntities[key].ReplaceEntityReferences(tagNewNameMap, key);
                                    });
                                    MainSurface_SetModified(true);
                                    FillVisualTree_References(false);
                                }

                                var message = String.Format(Properties.Resources.ReplacedItemsCountMessage, tagCount);
                                if (listRejected.Count > 0)
                                {
                                    listRejected.ForEach(f =>
                                    {
                                        var syslog = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Document.Parent.Title);
                                        syslog.ErrorFormat(Properties.Resources.ErrorReplacingTagNotFound, Document.Title, f);
                                    });
                                    message = String.Format("{0}{1}{2}", message, Environment.NewLine,
                                        String.Format(Properties.Resources.RejectedItemsCountMessage, listRejected.Count));
                                }

                                var uiMsgBox = ScreenManagerComponent.screenManagerComponent.UIInterface;
                                if (uiMsgBox != null)
                                {
                                    if (listRejected.Count > 0)
                                    {
                                        message = String.Format("{0}{1}{2}", message, Environment.NewLine, Properties.Resources.NeedToOpenSysLogQuestion);
                                        if (uiMsgBox.ShowYesNo(message, CustomDialogIcons.Warning) == CustomDialogResults.Yes)
                                        {
                                            Dispatcher.BeginInvokeInBackgroundIfRequired(() => ScreenManagerComponent.screenManagerComponent.Workspace.ShowSystemLog());
                                        }
                                    }
                                    else
                                        uiMsgBox.ShowInformation(message);
                                }
                            }
#endif
                        },
                        param => !String.IsNullOrEmpty(param as String) && referenceListTree != null && !String.IsNullOrEmpty(referenceListTree.View.SearchString?.Trim()) && referenceListTree.VisibleItems?.Count > 0 && !(bool)IsFullSearchAllowed
                        );
                }
                return replaceTextCommand;
            }
        }

        string ReplaceSearchString(string searchString, string stringToChange, char splitter, string param, bool checkOneChar = false)
        {
            string result = stringToChange;

            if (string.IsNullOrEmpty(searchString) || string.IsNullOrEmpty(stringToChange))
                return result;

            int manyCharCount = searchString.Split(splitter).Length - 1;
            if (manyCharCount == 1)
            {
                var searchingStrings = searchString.Split(splitter);
                var initString = searchingStrings[0];
                var endString = searchingStrings[1];
                if (string.IsNullOrEmpty(endString))
                {
                    initString = initString.Replace('\\', '/');
                    if (IsSearchCaseSensitive)
                        result = System.Text.RegularExpressions.Regex.Replace(stringToChange, initString, param);
                    else
                        result = System.Text.RegularExpressions.Regex.Replace(stringToChange, initString, param as String, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                }
                else
                {
                    var value = stringToChange;
                    if (!IsSearchCaseSensitive)
                    {
                        initString = initString.ToLower();
                        endString = endString.ToLower();
                        value = value.ToLower();
                    }
                    if (value.Contains(initString))
                    {
                        if ((!checkOneChar &&
                            value.Length > value.IndexOf(initString) + initString.Length &&
                            value.Substring(value.IndexOf(initString) + initString.Length).Contains(endString))
                            ||
                            (checkOneChar &&
                            value.Length > value.IndexOf(initString) + initString.Length + 1 &&
                            value.Substring(value.IndexOf(initString) + initString.Length + 1).StartsWith(endString)))
                        {
                            var _searchString = value.Substring(value.IndexOf(initString));
                            _searchString = _searchString.Substring(0, _searchString.IndexOf(endString) + endString.Length);

                            _searchString = _searchString.Replace('\\', '/');
                            if (IsSearchCaseSensitive)
                                result = System.Text.RegularExpressions.Regex.Replace(stringToChange, _searchString, param);
                            else
                                result = System.Text.RegularExpressions.Regex.Replace(stringToChange, _searchString, param as String, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        }
                    }
                }
            }
            else
            {
                searchString = searchString.Replace('\\', '/');
                if (IsSearchCaseSensitive)
                    result = System.Text.RegularExpressions.Regex.Replace(stringToChange, searchString, param);
                else
                    result = System.Text.RegularExpressions.Regex.Replace(stringToChange, searchString, param as String, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }

            return result;
        }


        private bool UpdateReference(string path, OPCUAEntityReference original, Dictionary<string, string> tagNewNameMap)
        {
            if (string.IsNullOrEmpty(path) || original == null)
                return false;

            if (tagNewNameMap == null)
                tagNewNameMap = new Dictionary<string, string>();
            string preChange = original.ToXml();
            if(tagNewNameMap.ContainsKey(preChange))
            {
                original.UpdateValue(tagNewNameMap[preChange].FromXml<OPCUAEntityReference>());
                return true;
            }

            var editor = EditorComponent.UFUAEditor;
            if (editor == null)
                return false;

            var split = path.Split(':');
            var instance = split[0]?.Replace('/', '\\');
            var tagName = split[0];
            if (split.Length > 1)
                tagName = split[1];
            else
                instance = null;

            var xml = editor.GetTagEntityReference(Document, tagName, instance);
            if (xml == null)
            {
                var datasync = OPCUAEntityReference.GetDataSinkInterface("TemporaryVariables");
                if (datasync != null)
                {
                    var relPath = tagName.Replace('\\', '&').Replace('/', '&');
                    datasync.GetVariables();
                    var tagRef = datasync.GetReference(relPath);
                    if (!String.IsNullOrEmpty(tagRef.ReadablePath))
                        xml = tagRef.ToXml();
                }
            }
            if (!String.IsNullOrEmpty(xml))
            {
                var tag = xml.FromXml<OPCUAEntityReference>();
                original.UpdateValue(tag);
                tagNewNameMap.Add(preChange, xml);
                return true;
            }
            return false;
        }

        double AngleMod360(double angle)
        {
            var a = angle % 360;
            return a < 0 ? a + 360 : a;
        }
        void OnCommandStop(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            CancelCurrentSelectedInsertObject();
            CancelSetZOrder();
        }

        void OnCommandSave(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            SaveCurrentDocument();
        }

        void CanCommandSave(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document != null && Document.NeedsSave || bLayoutChanged;
        }

        internal void MainSurface_SetModified(bool bSet)
        {
            if (Document == null)
                return;

            if (bSet ^ Document.NeedsSave)
            {
                Document.NeedsSave = bSet;
            }
        }

        /*
        private void MainSurface_SelectionChanged(object sender, EventArgs e)
        {
            if (MainSurface.EditingMode == CanvasEditingMode.Select)
                MainSurface_SetCurrentSelection();
        }
        */

        internal void MainSurface_CleanCurrentSelectedion()
        {
            RestoreSelected3DModels();

            foreach (KeyValuePair<UIElement, BasicAdorner> data in SelectedElementsMap)
            {
                data.Value.Deactivate();
                data.Value.changed -= adorner_changed;
                data.Value.changing -= adorner_changing;
                //var adorner = AdornerLayer.GetAdornerLayer(data.Key);
                //if (adorner == null)
                var adorner = AdornerLayer.GetAdornerLayer(MainSurface);
                if (adorner != null)
                    adorner.Remove(data.Value);

                Document.UnsubscribePropertyChangeXamlWriterProperties(data.Key);
                RestoreUntranslated(data.Key as FrameworkElement, false);
                ElementToCurrentLanguage(data.Key as FrameworkElement);
            }

            SelectedElementsMap.Clear();

            // Document.Mediator.NotifyColleagues<ScreenEditorView>("SelectionChanged", this);
            UpdateSelectionMeasureBar();
            if (smartPropertiesControl != null)
                smartPropertiesControl.DataContext = null;

            OnSelectionChanged(new EventArgs());
        }

        private void MainSurface_CleanWebHMIState()
        {
            foreach (UIElement element in ActiveLayer.Children)
            {
                CleanWebHMIState(element);
            }
        }

        private void CleanWebHMIState(UIElement element)
        {
            var adorner = AdornerLayer.GetAdornerLayer(MainSurface);
            if (adorner != null)
            {
                var adoreners = adorner.GetAdorners(element);
                if (adoreners != null)
                    (from a in adorner.GetAdorners(element) where a is WarningAdorner select a).
                        ToList().ForEach(a => adorner.Remove(a));
            }
        }

        private List<UIElement> MainSurface_GetCurrentSelection()
        {
            List<UIElement> selectedItems = new List<UIElement>();
            if (layoutItems.Visibility == Visibility.Collapsed)
            {
                foreach (KeyValuePair<UIElement, BasicAdorner> data in SelectedElementsMap)
                    selectedItems.Add(data.Key);
            }
            return selectedItems;
        }

        void UpdateSelectionMeasureBar()
        {
            if (isMovingEntity)
                return;

            var rect = new Rect(0, 0, 0, 0);
            var list = MainSurface_GetCurrentSelection();
            bool bFirst = true;
            list.OfType<FrameworkElement>().ToList().ForEach(el =>
                {
                    var rc = new Rect(Canvas.GetLeft(el), Canvas.GetTop(el), el.ActualWidth, el.ActualHeight);

                    var propertyPoints = el.GetType().GetProperty("Points");
                    if (propertyPoints != null || el is Line)
                    {
                        var points = new PointCollection();
                        if (el is Line)
                        {
                            var line = el as Line;
                            points.Add(new Point(line.X1, line.Y1));
                            points.Add(new Point(line.X2, line.Y2));
                        }
                        else
                            points = propertyPoints.GetValue(el) as PointCollection;
                        if (points != null)
                        {
                            Rect b = new Rect();
                            b.Y = b.X = Double.MaxValue;
                            b.Width = b.Height = 0;
                            foreach (Point p in points)
                            {
                                if (p.X < b.X)
                                    b.X = p.X;
                                if (p.Y < b.Y)
                                    b.Y = p.Y;
                                if (p.X > b.Width)
                                    b.Width = p.X;
                                if (p.Y > b.Height)
                                    b.Height = p.Y;
                            }
                            b.Width -= b.X;
                            b.Height -= b.Y;
                            rc = b;
                        }
                    }

                    if (bFirst)
                    {
                        bFirst = false;
                        rect = rc;
                    }
                    else
                        rect.Union(rc);
                });
            
            bool canShowPositionOptions = (from c in list
                                           where c is Pipeline.Pipeline ||
                                                 c is PolyBezier.PolyBezier ||
                                                 c is System.Windows.Shapes.Line ||
                                                 c is System.Windows.Shapes.Polygon ||
                                                 c is System.Windows.Shapes.Polyline
                                           select c).ToList().Count == 0;

            if (list.Count == 0 || !canShowPositionOptions)
            {
                stackEntityOptions.Visibility = Visibility.Collapsed;
                isUpdatingEntityPosition = true;
                SelectionHeight = SelectionWidth = SelectionXPos = SelectionYPos = 0d;
                isUpdatingEntityPosition = false;
            }
            else
            {
                stackEntityOptions.Visibility = Visibility.Visible;
                isUpdatingEntityPosition = true;
                SelectionXPos = rect.X;
                SelectionYPos = rect.Y;
                SelectionWidth = rect.Width;
                SelectionHeight = rect.Height;
                isUpdatingEntityPosition = false;
            }
        }

        public bool SaveCurrentDocument()
        {
            var doc = Document;
            if (doc == null)
                return true;
            var mainCtrl = MainControl;

            // Document.SaveAllProblematicXamlOnCanvas(MainSurface);
            if (EditorComponent != null && EditorComponent.Workspace != null)
            {
                EditorComponent.Workspace.BusyContent = Properties.Resources.Saving;
                EditorComponent.Workspace.IsBusy = true;
            }
            using (var cursor = new WaitCursor())
            {
                try
                {
                    ResetVisibilityLevel();
                    if (layoutItems.Visibility == Visibility.Visible)
                    {
                        SaveLayout();
                        bLayoutChanged = false;
                        Document.LayoutChanged = false;
                        return true;
                    }
                    else
                    {
                        if (doc.NeedsSave)
                            bHasBeenSaved = true;
                        if (lastCulture != null && ActiveLanguages.Contains(lastCulture) && langCombo.SelectedIndex != 0)
                            RestoreUntranslated(mainCtrl);
                        var ret = doc.SaveCurrentDocument(Surface);
                        ElementToCurrentLanguage(mainCtrl);
                        if (lastCulture != null && ActiveLanguages.Contains(lastCulture) && langCombo.SelectedIndex != 0)
                            StringEditor.SetActiveCulture(doc, lastCulture, true);
                        if (bIsActive) 
                            RefreshCurrentSelection();
                        return ret;
                    }
                }
                finally
                {
                    RestoreVisibilityLevel();
                    if (EditorComponent != null && EditorComponent.Workspace != null)
                        EditorComponent.Workspace.IsBusy = false;
                }
            }
        }

        public void ElementToCurrentLanguage(FrameworkElement control)
        {
            var bSub = Document.IsSubscribePropertyChangeXamlWriterProperties(control);
            if (bSub)
                Document.UnsubscribePropertyChangeXamlWriterProperties(control);
            if (StringEditor != null && control != null && lastCulture != null && ActiveLanguages.Contains(lastCulture) && langCombo.SelectedIndex != 0)
            {
                var map = StringEditor.GetListStringForCulture(Document, lastCulture);
                if (map != null && map.Count != 0)
                    Document.ChangeLanguage(lastCulture, control, map, true);
            }
            if (bSub)
                Document.SubscribePropertyChangeXamlWriterProperties(control, GetSelectedEntityName(control));
        }

        bool bHasBeenSaved;
        public void SaveImage()
        {
            if (!bHasBeenSaved)
                return;
            bHasBeenSaved = false;
            using (new WaitCursor())
            {
#if DEBUG
                using (var stopwatcher = new StopWatcher("Saving screen to PNG took : {0}"))
#endif
                {
                    Document.SaveToPng(MainSurface);
                }
            }
        }

        public List<String> MainSurface_GetCurrentSelectionNames()
        {
            List<String> selectedItems = new List<String>();
            foreach (KeyValuePair<UIElement, BasicAdorner> data in SelectedElementsMap)
            {
                if (data.Key is FrameworkElement)
                {
                    FrameworkElement element = data.Key as FrameworkElement;
                    String name = element.Name;
                    if (String.IsNullOrEmpty(name))
                        name = element.DependencyObjectType.Name;
                    selectedItems.Add(name);
                }
            }
            return selectedItems;
        }

        /*
        internal void MainSurface_ActivateDeactivateLineBaseAdorners(bool bActivate)
        {
            var list = (from c in SelectedElementsMap 
                        where c.Key is Line || c.Key is Polygon || c.Key is Polyline || c.Key is Pipeline.Pipeline select c.Value).ToList();
            list.ForEach(adorner => 
                {
                    adorner.RestorePosition(!bActivate);
                });
        }
        */

        public void MainSurface_SetCurrentSelectionNames(List<String> list)
        {
            List<UIElement> elementToSelect = new List<UIElement>();

            foreach (var uie in ActiveLayer.Children)
            {
                if (!(uie is FrameworkElement))
                    continue;

                FrameworkElement element = uie as FrameworkElement;
                String name = element.Name;
                if (String.IsNullOrEmpty(name))
                    name = element.DependencyObjectType.Name;
                if (list.Contains(name))
                    elementToSelect.Add(element);
            }

            MainSurface_SetCurrentSelection(elementToSelect.AsReadOnly());
        }

        void UpdateCurrentSelectionBindingProperties(FrameworkElement fe)
        {
            var listAlarm = new List<FrameworkElement>();
            var listConnectionString = new List<FrameworkElement>();
            var listEventConnectionString = new List<FrameworkElement>();
            var listScheduler = new List<FrameworkElement>();

            ScreenDocument.CheckPreBinding(listAlarm,
                    listConnectionString,
                    listEventConnectionString,
                    listScheduler,
                    fe);

            if (listConnectionString.Count > 0)
            {
                var settings = EditorComponent.UFUAEditor.GetHistorianDefaultConnection(Document);
                Document.PreBindConnectionStringSource(listConnectionString, settings, true);
            }
            if (listEventConnectionString.Count > 0)
            {
                var settings = EditorComponent.UFUAEditor.GetEventDefaultConnection(Document);
                Document.PreBindConnectionStringSource(listEventConnectionString, settings, true);
            }
        }

        /*
        private void MainSurface_SetCurrentSelection()
        {
            ReadOnlyCollection<UIElement> selectedElements = ActiveLayer.GetSelectedElements();
            if (selectedElements.Count <= 0)
                return;

            ActiveLayer.Select(null, null);
            MainSurface_SetCurrentSelection(selectedElements);
        }
        */

        public event EventHandler SelectionChanged;
        #region OnSelectionChanged
        /// <summary>
        /// Triggers the SelectionChanged event.
        /// </summary>
        public virtual void OnSelectionChanged(EventArgs ea)
        {   
            enable3DBI.IsChecked = false;
            enable3DCameraBI.IsChecked = false;
            var e = SelectionChanged;
            if (e != null)
                e(this, ea);
        }
        #endregion

        private void MainSurface_UpdateCurrentSelection(ReadOnlyCollection<UIElement> selectedElements)
        {
            var listToSelect = (from c in selectedElements where !SelectedElementsMap.ContainsKey(c) select c).ToList();
            var listToUnselect = (from c in SelectedElementsMap.Keys where !selectedElements.Contains(c) select c).ToList();

            foreach (UIElement uie in listToUnselect)
            {
                SelectedElementsMap[uie].Deactivate();
                SelectedElementsMap[uie].changed -= adorner_changed;
                SelectedElementsMap[uie].changing -= adorner_changing;
                //var adorner = AdornerLayer.GetAdornerLayer(data.Key);
                //if (adorner == null)
                var adorner = AdornerLayer.GetAdornerLayer(MainSurface);
                if (adorner != null)
                    adorner.Remove(SelectedElementsMap[uie]);

                SelectedElementsMap.Remove(uie);

                Document.UnsubscribePropertyChangeXamlWriterProperties(uie);
            }

            var selected = new List<ScreenEntity>();
            foreach (UIElement uie in listToSelect)
            {
                //if (!uie.IsVisible)
                //    continue;
                // Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, uie as FrameworkElement, true, true);
                var element = uie as FrameworkElement;
                var entity = GetSelectedEntity(element);
                var ad = BasicAdorner.CreateAdorner(MainSurface, uie, ActiveLayer.Children.Contains(uie), this, SelectedElementsMap.Count != 0);
                ad.changed += adorner_changed;
                ad.changing += adorner_changing;

                ad.DataContext = entity;
                selected.Add(entity);
                //var adorner = AdornerLayer.GetAdornerLayer(uie);
                //if (adorner == null)
                var adorner = AdornerLayer.GetAdornerLayer(MainSurface);
                adorner.Add(ad);
                SelectedElementsMap.Add(uie, ad);

                ad.ShowExpander(Document.IsShowAdornerExpander);

                if (lastKeyDown != Key.None)
                    ad.SetDraggable(false, (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift &&
                                           (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control);

                ad.SetLockMovement(entity.LockMovement);
                ad.SetCacheMode(element.CacheMode != null);

                Document.SubscribePropertyChangeXamlWriterProperties(element, GetSelectedEntityName(element));
            }

            bool bMultipleSelection = SelectedElementsMap.Count > 1;
            selectedElements.ToList().ForEach(el =>
                {
                    SelectedElementsMap[el].SetIsActive(false);

                    var propertyPoints = el.GetType().GetProperty("Points");
                    if (propertyPoints != null)
                        SelectedElementsMap.Values.ToList().ForEach(ad => ad.SetDraggable(bMultipleSelection, bForce: true));
                });
            if (selectedElements.Count > 0)
                SelectedElementsMap[selectedElements.First()].SetIsActive(true, SelectedElementsMap.Count > 0);

            //if (listToSelect.Count > 0)
            //    EditorComponent.Workspace.ContextObjects = selected;
            //else
            //    EditorComponent.Workspace.ContextObject = Document;

            UpdateSelectionMeasureBar();

            OnSelectionChanged(new EventArgs());
        }

        ScreenEntity GetSelectedEntity(FrameworkElement fe, bool bTestOnly = false)
        {
            var arg = new GetFriendObjectsEventArgs();
            if (ActiveLayer.Children.Contains(fe))
            {
                if (String.IsNullOrEmpty(fe.Name) || MainSurface.FindName(fe.Name) != fe)
                {
                    if (!bTestOnly)
                        Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, fe, true);
                }
            }
            else
            {
                var listContainer = new List<String>();
                var element = fe;
                while ((element = LogicalTreeHelper.GetParent(element) as FrameworkElement) != null)
                {
                    var name = element.Uid as String;
                    if (String.IsNullOrEmpty(name))
                        name = element.Name;
                    if (!String.IsNullOrEmpty(name))
                        listContainer.Insert(0, name);

                    if (ActiveLayer.Children.Contains(element))
                    {
                        break;
                    }
                }
                if (element == null)
                    element = fe;

                if (String.IsNullOrEmpty(element.Name) || MainSurface.FindName(element.Name) != element)
                {
                    if (!bTestOnly)
                        Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, element, true);
                }

                ContentControl contentControl = null;
                if  (objectBrowserTree == null || 
                    objectBrowserTree.GetSelectedNodes().Length == 0 ||
                    objectBrowserTree.GetSelectedNodes()[0].Tag != fe)
                    contentControl = LogicalTreeHelper.GetParent(fe) as ContentControl;
                if (contentControl != null)
                {
                    arg.Name = contentControl.Name;
                    fe = contentControl;
                }
                else
                {
                    arg.Name = fe.Uid as String;
                    if (String.IsNullOrEmpty(arg.Name))
                        arg.Name = fe.Name;
                }
                arg.Container = Document.FormatContainerNames(listContainer);
            }
            Document.GetFriendObjects(fe, arg, bTestOnly);

            if (arg.friendList.Count > 0)
            {
                var ret = arg.friendList[0] as ScreenEntity;
                if (ret != null)
                {
                    ret.Entity = fe;
                    ret.Document = Document;
                }

                return ret;
            }

            return null;
        }

        String GetSelectedEntityName(FrameworkElement fe)
        {
            var arg = new GetFriendObjectsEventArgs();
            if (ActiveLayer.Children.Contains(fe))
            {
                if (String.IsNullOrEmpty(fe.Name) || MainSurface.FindName(fe.Name) != fe)
                    Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, fe, true);
            }
            else
            {
                var listContainer = new List<String>();
                var element = fe;
                while ((element = LogicalTreeHelper.GetParent(element) as FrameworkElement) != null)
                {
                    var name = element.Uid as String;
                    if (String.IsNullOrEmpty(name))
                        name = element.Name;
                    if (!String.IsNullOrEmpty(name))
                        listContainer.Insert(0, name);

                    if (ActiveLayer.Children.Contains(element))
                    {
                        break;
                    }
                }
                if (element == null)
                    element = fe;

                if (ActiveLayer.Children.Contains(element))
                {
                    if (String.IsNullOrEmpty(element.Name) || MainSurface.FindName(element.Name) != element)
                        Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, element, true);
                }

                var contentControl = LogicalTreeHelper.GetParent(fe) as ContentControl;
                if (contentControl != null)
                {
                    arg.Name = contentControl.Name;
                    fe = contentControl;
                }
                else
                {
                    arg.Name = fe.Uid as String;
                    if (String.IsNullOrEmpty(arg.Name))
                        arg.Name = fe.Name;
                }
                arg.Container = Document.FormatContainerNames(listContainer);
            }
            try
            {
                Document.GetFriendObjects(fe, arg);
            }
            catch
            {
                return String.Empty;
            }

            return arg.Name;
        }

        private void MainSurface_SetCurrentSelection(ReadOnlyCollection<UIElement> selectedElements, 
            bool bScroll = false, bool bSkipFocus = false, bool bForceRefresh = false)
        {
            var listSelected = MainSurface_GetCurrentSelection();
            if (!bForceRefresh && listSelected.Count == selectedElements.Count)
            {
                bool bEqual = true;
                for (int i = 0; i < selectedElements.Count; ++i)
                {
                    if (!Object.ReferenceEquals(selectedElements[i], listSelected[i]))
                    {
                        bEqual = false;
                        break;
                    }
                }
                if (bEqual)
                    return;
            }
            MainSurface_CleanCurrentSelectedion();

            var selecteItemProp = new List<UIElement>();
            var selected = new List<ScreenEntity>();

            int nCounter = 0;
            Rect rc = Rect.Empty;
            foreach (UIElement uie in selectedElements)
            {
                if (SelectedElementsMap.ContainsKey(uie))
                    continue;

                //if (!uie.IsVisible)
                //    continue;

                bool multiselect = nCounter != 0;
                nCounter++;

                // Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, uie as FrameworkElement, true, true);
                if (!ActiveLayer.Children.Contains(uie))
                    bScroll = false;

                var element = uie as FrameworkElement;
                var entity = GetSelectedEntity(element);
                var ad = BasicAdorner.CreateAdorner(MainSurface, uie, ActiveLayer.Children.Contains(uie), this, multiselect);
                ad.AllowDrop = true;
                ad.changed += adorner_changed;
                ad.changing += adorner_changing;

                ad.DataContext = entity;
                selected.Add(entity);

                selecteItemProp.Add(uie);

                //var adorner = AdornerLayer.GetAdornerLayer(uie);
                //if (adorner == null)
                var adorner = AdornerLayer.GetAdornerLayer(MainSurface);
                adorner.Add(ad);
                SelectedElementsMap.Add(uie, ad);

                ad.ShowExpander(Document.IsShowAdornerExpander);

                if (lastKeyDown != Key.None)
                    ad.SetDraggable(false, (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift &&
                                            (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control);
                ad.SetLockMovement(entity.LockMovement);
                ad.SetCacheMode(element.CacheMode != null);

                UpdateCurrentSelectionBindingProperties(uie as FrameworkElement);

                var rect = DependencyObjectExtensions.CalculateBoundRect(new List<UIElement>() { uie },
                                                                MainSurface);
                rc.Union(rect);

                Document.SubscribePropertyChangeXamlWriterProperties(element, GetSelectedEntityName(element));
            }

            if (bScroll && !Double.IsNaN(rc.Left) && !Double.IsNaN(rc.Top))
            {
                Scroller.ScrollToHorizontalOffset(rc.Left * ZoomLevelX);
                Scroller.ScrollToVerticalOffset(rc.Top * ZoomLevelY);
            }

            bool bMultipleSelection = SelectedElementsMap.Count > 1;
            if (bMultipleSelection)
            {
                selectedElements.ToList().ForEach(el =>
                {
                    var propertyPoints = el.GetType().GetProperty("Points");
                    if (propertyPoints != null)
                        SelectedElementsMap.Values.ToList().ForEach(ad => ad.SetDraggable(bMultipleSelection, bForce: true));
                });
            }

            if (selectedElements.Count > 0)
            {
                SelectedElementsMap[selectedElements.First()].SetIsActive(true, selectedElements.Count > 0);
            }

            if (listActive3dModels.Count > 0 && selectedElements.Count == 1 && selected.Count == 1 && selectedElements[0] is FrameworkElement)
            {
                var model = listActive3dModels.First();
                var hash = Utilities.WPF.DependencyObjectExtensions.RegisterModel3D(MainSurface, selectedElements[0] as FrameworkElement, model);
                var entity = selected[0].AddOrFind3DInnerModel(model, hash);
                entity.Entity3D = model;
                selected.Clear();
                selected.Add(entity);
            }

            if (selecteItemProp.Count > 0)
                EditorComponent.Workspace.ContextObjects = selected;
            else
                EditorComponent.Workspace.ContextObject = Document;

            OnSelectionChanged(new EventArgs());

            // Document.Mediator.NotifyColleagues<ScreenEditorView>("SelectionChanged", this);
            if (!bSkipFocus)
            {
                UpdateSelectionMeasureBar();

                Focusable = true;
                Focus();
            }
            System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        }

        // DispatcherOperation ScrollChangedOperation;
        private void Scroller_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            //if (ScrollChangedOperation != null || e.HorizontalChange == 0 && e.VerticalChange == 0)
            //    return;
            //ScrollChangedOperation = Dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
            //{
            //    var listSelected = MainSurface_GetCurrentSelection();
            //    MainSurface_CleanCurrentSelectedion();
            //    var readonlyList = new ReadOnlyCollection<UIElement>(listSelected);
            //    MainSurface_SetCurrentSelection(readonlyList);
            //    ScrollChangedOperation = null;
            //});
        }

        void adorner_changing(object sender, EventArgs e)
        {
            undoredo.SetStateForUndoRedo(ScreenManager.UndoRedoManager.UndoAction.Changed);

            var listSelected = MainSurface_GetCurrentSelection();
            if (listSelected.Count > 0)
                PrepareControlRectangleForSnapLines(listSelected[0]);
        }

        void adorner_changed(object sender, EventArgs e)
        {
            var listSelected = MainSurface_GetCurrentSelection();
            listSelected.ForEach(el =>
                {
                    var fe = listSelected[0] as FrameworkElement;
                    var entity = GetSelectedEntity(fe);
                    entity.PreservedWidth = fe.Width;
                    entity.PreservedHeight = fe.Height;
                });

            if (e is AdornerOperationEventArgs && (e as AdornerOperationEventArgs).Operation == AdornerOperationEventArgs.AdornerOperation.TextChanged && listSelected.Count > 0 && (e as AdornerOperationEventArgs).uiElement != null)
                UpdateStringIdAndTranslate((e as AdornerOperationEventArgs).uiElement);

            MainSurface_SetModified(true);
        }

        public void UpdateStringIdAndTranslate(UIElement el, bool bAlreadyUntranslated = true, bool bClearTexts = true, bool bClearTooltips = true)
        {
            if (Document == null)
                return;

            if (bAlreadyUntranslated)
                Document.ClearTranslationMaps(el, bClearTexts, bClearTooltips);
            if (lastCulture != null && ActiveLanguages.Contains(lastCulture) && langCombo.SelectedIndex != 0)
                ChangeLanguage(lastCulture, el as FrameworkElement);
        }

        public void UnGroupItem()
        {
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();
            if (listSelected == null || listSelected.Count != 1)
                return;

            regroupType = listSelected[0] as FrameworkElement;
            regroupName = regroupType.Name;
            if (Document.MapScreenEntities.ContainsKey(regroupName))
                entityRegroup = Document.MapScreenEntities[regroupName];
            else
                entityRegroup = null;

            //if (!CheckSelectedProblematicXaml())
            //{
            //    EditorComponent.UIInterface.ShowError(Properties.Resources.SelectionContainsProblematicXaml);
            //    return;
            //}

            if (listSelected[0] is Panel)
            {
                UnGroupItem(listSelected[0] as Panel);
                return;
            }
            else if (listSelected[0] is Expander || listSelected[0] is GroupBox)
            {
                UnGroupItem(listSelected[0] as ContentControl);
                return;
            }
            else if (listSelected[0] is Decorator && !(listSelected[0] is Viewbox))
            {
                UnGroupItem(listSelected[0] as Decorator);
                return;
            }

            Viewbox g = listSelected[0] as Viewbox;
            var gTop = Canvas.GetTop(g);
            var gLeft = Canvas.GetLeft(g);

            // Canvas parent = VisualTreeHelper.GetParent(g) as Canvas;
            // Canvas Inkparent = VisualTreeHelper.GetParent(g) as Canvas;

            List<UIElement> listItemToSelect = new List<UIElement>();
            try
            {
                Canvas gc = g.Child as Canvas;

                var renamed = new Dictionary<String, String>();

                while (gc.Children.Count > 0)
                {
                    FrameworkElement child = gc.Children[0] as FrameworkElement;

                    bool bResubscribe = false;
                    if (Document.IsSubscribePropertyChangeXamlWriterProperties(child))
                    {
                        bResubscribe = true;
                        Document.UnsubscribePropertyChangeXamlWriterProperties(child);
                    }

                    child.SetValue(Panel.MarginProperty, DependencyProperty.UnsetValue);
                    Rect b = VisualTreeHelper.GetDescendantBounds(child);

                    double left = Canvas.GetLeft(child);
                    double top = Canvas.GetTop(child);

                    left = double.IsNaN(left) ? 0 : left;
                    top = double.IsNaN(top) ? 0 : top;

                    //if ((child is Shape) && child.ReadLocalValue(Shape.StretchProperty).Equals(DependencyProperty.UnsetValue))
                    //{
                    //    child.Width = b.Width;
                    //    child.Height = b.Height;

                    //    if (MainSurface is Canvas)
                    //    {
                    //        Canvas.SetLeft(child, left + b.X + gLeft);
                    //        Canvas.SetTop(child, top + b.Y + gTop);
                    //        child.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                    //        child.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                    //    }
                    //    else
                    //    {
                    //        Canvas.SetLeft(child, left + b.X + gLeft);
                    //        Canvas.SetTop(child, top + b.Y + gTop);
                    //        child.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                    //        child.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                    //    }

                    //    child.SetValue(Shape.StretchProperty, Stretch.Fill);
                    //    gc.UpdateLayout();
                    //}
                    //else
                    // {
                    // excluding rotate from common matrix
                    // need to separate scale*skew matrix on scale and transform transformations
                    //Matrix matrGtr = ((Transform)child.TransformToVisual(ActiveLayer)).Value;

                    //double x = matrGtr.OffsetX;
                    //double y = matrGtr.OffsetY;

                    //double angleY = Math.Atan(matrGtr.M12 / matrGtr.M11) * 180 / Math.PI;
                    //matrGtr.OffsetY = 0; matrGtr.OffsetX = 0;

                    //RotateTransform rt = new RotateTransform(angleY);
                    //matrGtr.Rotate(-angleY);
                    //TransformGroup gtr = new TransformGroup();
                    //gtr.Children.Add(new MatrixTransform(matrGtr));
                    //gtr.Children.Add(rt);
                    //// child.RenderTransform = gtr;

                    //// child.RenderTransformOrigin = new Point(0.5, 0.5);

                    //Point pO = new Point(child.ActualWidth * child.RenderTransformOrigin.X, child.ActualHeight * child.RenderTransformOrigin.Y);
                    //Point p = gtr.Transform(pO);

                    var tranform = child.RenderTransform;
                    child.RenderTransform = null;

                    child.Refresh();
                    var transformOffset = LayoutHelper.GetRelativeElementRect(child, MainSurface);

                    var name = GetSelectedEntityName(child);

                    gc.Children.Remove(child);
                    ActiveLayer.Children.Add(child);

                    if (!String.IsNullOrEmpty(name))
                        Document.SubscribePropertyChangeXamlWriterProperties(child, name);

                    //if (MainSurface is Canvas)
                    //{
                    Canvas.SetLeft(child, transformOffset.X);
                    Canvas.SetTop(child, transformOffset.Y);
                    //    child.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                    //    child.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                    //}
                    //else
                    //{
                    //    Canvas.SetLeft(child, transformOffset.X);
                    //    Canvas.SetTop(child, transformOffset.Y);
                    //    child.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                    //    child.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                    //}


                    var factorX = transformOffset.Width / child.ActualWidth;
                    var factorY = transformOffset.Height / child.ActualHeight;

                    TransformsPoints(child, factorX, factorY);

                    child.Width = transformOffset.Width;
                    child.Height = transformOffset.Height;

                    child.RenderTransform = tranform;

                    if (Document.IsSubscribePropertyChangeXamlWriterProperties(child))
                        Document.UnsubscribePropertyChangeXamlWriterProperties(child);

                    if (child.Uid is String && !String.IsNullOrEmpty(child.Uid as String))
                    {
                        child.Name = child.Uid as String;
                        child.ClearValue(FrameworkElement.UidProperty);
                    }

                    var ret = Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, child, true, false);
                    ret.Keys.ToList().ForEach(key =>
                    {
                        if (renamed.ContainsKey(key))
                            renamed.Remove(key);
                        renamed.Add(key, ret[key]);

                        //if (Document.MapScreenEntities.ContainsKey(key))
                        //{
                        //    var entity = Document.MapScreenEntities[key];
                        //    Document.MapScreenEntities.Remove(key);
                        //    if (Document.MapScreenEntities.ContainsKey(ret[key]))
                        //        Document.MapScreenEntities.Remove(ret[key]);
                        //    Document.MapScreenEntities.Add(ret[key], entity);
                        //}
                    });
                    listItemToSelect.Add(child);

                    //if (bResubscribe)
                    //    Document.SubscribePropertyChangeXamlWriterProperties(child, child.Name);
                }

                Document.TransformFromInners(MainSurface, g.Name, renamed);
                Document.RemoveEntity(g);
                Utilities.WPF.DependencyObjectExtensions.UnregisterName(MainSurface, g);

                listItemToSelect.OfType<FrameworkElement>().ToList().ForEach(fe =>
                    {
                        if (Document.MapScreenEntities.ContainsKey(fe.Name) &&
                            !String.IsNullOrEmpty(Document.MapScreenEntities[fe.Name].ProblematicXaml))
                            Utilities.WPF.XmlHelper.CheckProblematicXamlWriter(fe, Document.MapScreenEntities[fe.Name].ProblematicXaml);
                        // Document.UpdateProblematicXamlWriterProperties(fe, fe.Name);
                    });

                g.Child = null;

                MainSurface_CleanCurrentSelectedion();
                ActiveLayer.Children.Remove(g);

                Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    Document.ResolveEntityBrushAndPen(MainSurface);
                    Document.ResolveEntityTextDecorators(MainSurface);
                });

                // Utilities.WPF.DependencyObjectExtensions.UnregisterName(MainSurface, g);

                listItemToSelect.OfType<FrameworkElement>().ToList().ForEach(fe =>
                    {
                        Document.LoadRepositoryItem(this, MainSurface, fe.Name);
                    });
                ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(listItemToSelect);
                MainSurface_SetCurrentSelection(readonlyList);
                MainSurface_SetModified(true);
            }
            catch (Exception)
            {
            }
        }

        public void UnGroupItem(ContentControl panel)
        {
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();
            if (listSelected == null || listSelected.Count != 1)
                return;

            ContentControl g = panel as ContentControl;
            var gTop = Canvas.GetTop(g);
            var gLeft = Canvas.GetLeft(g);

            List<UIElement> listItemToSelect = new List<UIElement>();
            try
            {
                var renamed = new Dictionary<String, String>();

                // while (g.Children.Count > 0)
                {
                    FrameworkElement child = g.Content as FrameworkElement;
                    if (child == null)
                        return;

                    bool bResubscribe = false;
                    if (Document.IsSubscribePropertyChangeXamlWriterProperties(child))
                    {
                        bResubscribe = true;
                        Document.UnsubscribePropertyChangeXamlWriterProperties(child);
                    }

                    child.SetValue(Panel.MarginProperty, DependencyProperty.UnsetValue);
                    Rect b = VisualTreeHelper.GetDescendantBounds(child);

                    double left = Canvas.GetLeft(child);
                    double top = Canvas.GetTop(child);

                    left = double.IsNaN(left) ? 0 : left;
                    top = double.IsNaN(top) ? 0 : top;

                    //if ((child is Shape) && child.ReadLocalValue(Shape.StretchProperty).Equals(DependencyProperty.UnsetValue))
                    //{
                    //    child.Width = b.Width;
                    //    child.Height = b.Height;

                    //    if (MainSurface is Canvas)
                    //    {
                    //        Canvas.SetLeft(child, left + b.X + gLeft);
                    //        Canvas.SetTop(child, top + b.Y + gTop);
                    //        child.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                    //        child.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                    //    }
                    //    else
                    //    {
                    //        Canvas.SetLeft(child, left + b.X + gLeft);
                    //        Canvas.SetTop(child, top + b.Y + gTop);
                    //        child.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                    //        child.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                    //    }

                    //    child.SetValue(Shape.StretchProperty, Stretch.Fill);
                    //    g.UpdateLayout();
                    //}
                    //else
                    //{
                    //    // excluding rotate from common matrix
                    //    // need to separate scale*skew matrix on scale and transform transformations
                    //    Matrix matrGtr = ((Transform)child.TransformToVisual(ActiveLayer)).Value;

                    //    double x = matrGtr.OffsetX;
                    //    double y = matrGtr.OffsetY;

                    //    double angleY = Math.Atan(matrGtr.M12 / matrGtr.M11) * 180 / Math.PI;
                    //    matrGtr.OffsetY = 0; matrGtr.OffsetX = 0;

                    //    RotateTransform rt = new RotateTransform(angleY);
                    //    matrGtr.Rotate(-angleY);
                    //    TransformGroup gtr = new TransformGroup();
                    //    gtr.Children.Add(new MatrixTransform(matrGtr));
                    //    gtr.Children.Add(rt);
                    // child.RenderTransform = gtr;

                    // child.RenderTransformOrigin = new Point(0.5, 0.5);

                    var tranform = child.RenderTransform;
                    child.RenderTransform = null;

                    child.Refresh();
                    Rect transformOffset = Rect.Empty;
                    try
                    {
                        transformOffset = LayoutHelper.GetRelativeElementRect(child, MainSurface);
                    }
                    catch
                    {
                    }

                    g.Content = null;
                    ActiveLayer.Children.Add(child);

                    var name = GetSelectedEntityName(child);
                    if (!String.IsNullOrEmpty(name))
                        Document.SubscribePropertyChangeXamlWriterProperties(child, name);

                    if (transformOffset.IsEmpty)
                    {
                        Canvas.SetLeft(child, gLeft);
                        Canvas.SetTop(child, gTop);
                    }
                    else
                    {
                        //if (MainSurface is Canvas)
                        //{
                        Canvas.SetLeft(child, transformOffset.X);
                        Canvas.SetTop(child, transformOffset.Y);
                        //    child.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                        //    child.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                        //}
                        //else
                        //{
                        //    Canvas.SetLeft(child, transformOffset.X);
                        //    Canvas.SetTop(child, transformOffset.Y);
                        //    child.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                        //    child.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                        //}

                        child.Width = transformOffset.Width;
                        child.Height = transformOffset.Height;
                    }

                    child.RenderTransform = tranform;
                    // }

                    if (Document.IsSubscribePropertyChangeXamlWriterProperties(child))
                        Document.UnsubscribePropertyChangeXamlWriterProperties(child);

                    if (child.Uid is String && !String.IsNullOrEmpty(child.Uid as String))
                    {
                        child.Name = child.Uid as String;
                        child.ClearValue(FrameworkElement.UidProperty);
                    }

                    var ret = Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, child, true, false);
                    ret.Keys.ToList().ForEach(key =>
                    {
                        if (renamed.ContainsKey(key))
                            renamed.Remove(key);
                        renamed.Add(key, ret[key]);

                        //if (Document.MapScreenEntities.ContainsKey(key))
                        //{
                        //    var entity = Document.MapScreenEntities[key];
                        //    Document.MapScreenEntities.Remove(key);
                        //    if (Document.MapScreenEntities.ContainsKey(ret[key]))
                        //        Document.MapScreenEntities.Remove(ret[key]);
                        //    Document.MapScreenEntities.Add(ret[key], entity);
                        //}
                    });
                    listItemToSelect.Add(child);

                    //if (bResubscribe)
                    //    Document.SubscribePropertyChangeXamlWriterProperties(child, child.Name);
                }

                Document.TransformFromInners(MainSurface, g.Name, renamed);
                Document.RemoveEntity(g);
                Utilities.WPF.DependencyObjectExtensions.UnregisterName(MainSurface, g);

                listItemToSelect.OfType<FrameworkElement>().ToList().ForEach(fe =>
                {
                    if (Document.MapScreenEntities.ContainsKey(fe.Name) &&
                        !String.IsNullOrEmpty(Document.MapScreenEntities[fe.Name].ProblematicXaml))
                        Utilities.WPF.XmlHelper.CheckProblematicXamlWriter(fe, Document.MapScreenEntities[fe.Name].ProblematicXaml);
                    // Document.UpdateProblematicXamlWriterProperties(fe, fe.Name);
                });

                MainSurface_CleanCurrentSelectedion();
                ActiveLayer.Children.Remove(g);

                Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    Document.ResolveEntityBrushAndPen(MainSurface);
                    Document.ResolveEntityTextDecorators(MainSurface);
                });

                // Utilities.WPF.DependencyObjectExtensions.UnregisterName(MainSurface, g);

                listItemToSelect.OfType<FrameworkElement>().ToList().ForEach(fe =>
                {
                    Document.LoadRepositoryItem(this, MainSurface, fe.Name);
                });
                ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(listItemToSelect);
                MainSurface_SetCurrentSelection(readonlyList);

                MainSurface_SetModified(true);
            }
            catch (Exception)
            {
            }
        }

        public void UnGroupItem(Decorator panel)
        {
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();
            if (listSelected == null || listSelected.Count != 1)
                return;

            Decorator g = panel as Decorator;
            var gTop = Canvas.GetTop(g);
            var gLeft = Canvas.GetLeft(g);

            List<UIElement> listItemToSelect = new List<UIElement>();
            try
            {
                var renamed = new Dictionary<String, String>();
                // while (g.Children.Count > 0)
                {
                    FrameworkElement child = g.Child as FrameworkElement;
                    if (child == null)
                        return;

                    bool bResubscribe = false;
                    if (Document.IsSubscribePropertyChangeXamlWriterProperties(child))
                    {
                        bResubscribe = true;
                        Document.UnsubscribePropertyChangeXamlWriterProperties(child);
                    }

                    child.SetValue(Panel.MarginProperty, DependencyProperty.UnsetValue);
                    Rect b = VisualTreeHelper.GetDescendantBounds(child);

                    double left = Canvas.GetLeft(child);
                    double top = Canvas.GetTop(child);

                    left = double.IsNaN(left) ? 0 : left;
                    top = double.IsNaN(top) ? 0 : top;


                    //if ((child is Shape) && child.ReadLocalValue(Shape.StretchProperty).Equals(DependencyProperty.UnsetValue))
                    //{
                    //    child.Width = b.Width;
                    //    child.Height = b.Height;

                    //    if (MainSurface is Canvas)
                    //    {
                    //        Canvas.SetLeft(child, left + b.X + gLeft);
                    //        Canvas.SetTop(child, top + b.Y + gTop);
                    //        child.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                    //        child.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                    //    }
                    //    else
                    //    {
                    //        Canvas.SetLeft(child, left + b.X + gLeft);
                    //        Canvas.SetTop(child, top + b.Y + gTop);
                    //        child.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                    //        child.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                    //    }

                    //    child.SetValue(Shape.StretchProperty, Stretch.Fill);
                    //    g.UpdateLayout();
                    //}
                    //else
                    //{
                    //    // excluding rotate from common matrix
                    //    // need to separate scale*skew matrix on scale and transform transformations
                    //    Matrix matrGtr = ((Transform)child.TransformToVisual(ActiveLayer)).Value;

                    //    double x = matrGtr.OffsetX;
                    //    double y = matrGtr.OffsetY;

                    //    double angleY = Math.Atan(matrGtr.M12 / matrGtr.M11) * 180 / Math.PI;
                    //    matrGtr.OffsetY = 0; matrGtr.OffsetX = 0;

                    //    RotateTransform rt = new RotateTransform(angleY);
                    //    matrGtr.Rotate(-angleY);
                    //    TransformGroup gtr = new TransformGroup();
                    //    gtr.Children.Add(new MatrixTransform(matrGtr));
                    //    gtr.Children.Add(rt);
                    // child.RenderTransform = gtr;

                    // child.RenderTransformOrigin = new Point(0.5, 0.5);

                    var transformOffset = LayoutHelper.GetRelativeElementRect(child, MainSurface);

                    g.Child = null;
                    ActiveLayer.Children.Add(child);

                    var name = GetSelectedEntityName(child);
                    if (!String.IsNullOrEmpty(name))
                        Document.SubscribePropertyChangeXamlWriterProperties(child, name);

                    //if (MainSurface is Canvas)
                    //{
                    Canvas.SetLeft(child, transformOffset.X);
                    Canvas.SetTop(child, transformOffset.Y);
                    //    child.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                    //    child.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                    //}
                    //else
                    //{
                    //    Canvas.SetLeft(child, transformOffset.X);
                    //    Canvas.SetTop(child, transformOffset.Y);
                    //    child.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                    //    child.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                    //}

                    child.Width = transformOffset.Width;
                    child.Height = transformOffset.Height;
                    // }

                    if (Document.IsSubscribePropertyChangeXamlWriterProperties(child))
                        Document.UnsubscribePropertyChangeXamlWriterProperties(child);

                    if (child.Uid is String && !String.IsNullOrEmpty(child.Uid as String))
                    {
                        child.Name = child.Uid as String;
                        child.ClearValue(FrameworkElement.UidProperty);
                    }

                    var ret = Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, child, true, false);
                    ret.Keys.ToList().ForEach(key =>
                    {
                        if (renamed.ContainsKey(key))
                            renamed.Remove(key);
                        renamed.Add(key, ret[key]);

                        //if (Document.MapScreenEntities.ContainsKey(key))
                        //{
                        //    var entity = Document.MapScreenEntities[key];
                        //    Document.MapScreenEntities.Remove(key);
                        //    if (Document.MapScreenEntities.ContainsKey(ret[key]))
                        //        Document.MapScreenEntities.Remove(ret[key]);
                        //    Document.MapScreenEntities.Add(ret[key], entity);
                        //}
                    });
                    listItemToSelect.Add(child);

                    //if (bResubscribe)
                    //    Document.SubscribePropertyChangeXamlWriterProperties(child, child.Name);
                }

                Document.TransformFromInners(MainSurface, g.Name, renamed);
                Document.RemoveEntity(g);
                Utilities.WPF.DependencyObjectExtensions.UnregisterName(MainSurface, g);

                listItemToSelect.OfType<FrameworkElement>().ToList().ForEach(fe =>
                {
                    if (Document.MapScreenEntities.ContainsKey(fe.Name) &&
                        !String.IsNullOrEmpty(Document.MapScreenEntities[fe.Name].ProblematicXaml))
                        Utilities.WPF.XmlHelper.CheckProblematicXamlWriter(fe, Document.MapScreenEntities[fe.Name].ProblematicXaml);
                    // Document.UpdateProblematicXamlWriterProperties(fe, fe.Name);
                });

                MainSurface_CleanCurrentSelectedion();
                ActiveLayer.Children.Remove(g);

                Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    Document.ResolveEntityBrushAndPen(MainSurface);
                    Document.ResolveEntityTextDecorators(MainSurface);
                });

                // Utilities.WPF.DependencyObjectExtensions.UnregisterName(MainSurface, g);

                listItemToSelect.OfType<FrameworkElement>().ToList().ForEach(fe =>
                {
                    Document.LoadRepositoryItem(this, MainSurface, fe.Name);
                });
                ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(listItemToSelect);
                MainSurface_SetCurrentSelection(readonlyList);

                MainSurface_SetModified(true);
            }
            catch (Exception)
            {
            }
        }

        public void UnGroupItem(Panel panel)
        {
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();
            if (listSelected == null || listSelected.Count != 1)
                return;

            Panel g = listSelected[0] as Panel;
            var gTop = Canvas.GetTop(g);
            var gLeft = Canvas.GetLeft(g);
            // Canvas parent = VisualTreeHelper.GetParent(g) as Canvas;
            // Canvas Inkparent = VisualTreeHelper.GetParent(g) as Canvas;

            List<UIElement> listItemToSelect = new List<UIElement>();
            try
            {
                var renamed = new Dictionary<String, String>();

                while (g.Children.Count > 0)
                {
                    FrameworkElement child = g.Children[0] as FrameworkElement;

                    bool bResubscribe = false;
                    if (Document.IsSubscribePropertyChangeXamlWriterProperties(child))
                    {
                        bResubscribe = true;
                        Document.UnsubscribePropertyChangeXamlWriterProperties(child);
                    }

                    child.SetValue(Panel.MarginProperty, DependencyProperty.UnsetValue);
                    Rect b = VisualTreeHelper.GetDescendantBounds(child);

                    double left = Canvas.GetLeft(child);
                    double top = Canvas.GetTop(child);

                    left = double.IsNaN(left) ? 0 : left;
                    top = double.IsNaN(top) ? 0 : top;

                    //if ((child is Shape) && child.ReadLocalValue(Shape.StretchProperty).Equals(DependencyProperty.UnsetValue))
                    //{
                    //    child.Width = b.Width;
                    //    child.Height = b.Height;

                    //    if (MainSurface is Canvas)
                    //    {
                    //        Canvas.SetLeft(child, left + b.X + gLeft);
                    //        Canvas.SetTop(child, top + b.Y + gTop);
                    //        child.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                    //        child.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                    //    }
                    //    else
                    //    {
                    //        Canvas.SetLeft(child, left + b.X + gLeft);
                    //        Canvas.SetTop(child, top + b.Y + gTop);
                    //        child.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                    //        child.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                    //    }

                    //    child.SetValue(Shape.StretchProperty, Stretch.Fill);
                    //    g.UpdateLayout();
                    //}
                    //else
                    //{
                    //    // excluding rotate from common matrix
                    //    // need to separate scale*skew matrix on scale and transform transformations
                    //    Matrix matrGtr = ((Transform)child.TransformToVisual(ActiveLayer)).Value;

                    //    double x = matrGtr.OffsetX;
                    //    double y = matrGtr.OffsetY;

                    //    double angleY = Math.Atan(matrGtr.M12 / matrGtr.M11) * 180 / Math.PI;
                    //    matrGtr.OffsetY = 0; matrGtr.OffsetX = 0;

                    //    RotateTransform rt = new RotateTransform(angleY);
                    //    matrGtr.Rotate(-angleY);
                    //    TransformGroup gtr = new TransformGroup();
                    //    gtr.Children.Add(new MatrixTransform(matrGtr));
                    //    gtr.Children.Add(rt);
                    // child.RenderTransform = gtr;

                    // child.RenderTransformOrigin = new Point(0.5, 0.5);

                    var tranform = child.RenderTransform;
                    child.RenderTransform = null;

                    child.Refresh();
                    var transformOffset = LayoutHelper.GetRelativeElementRect(child, MainSurface);

                    g.Children.Remove(child);
                    ActiveLayer.Children.Add(child);

                    var name = GetSelectedEntityName(child);
                    if (!String.IsNullOrEmpty(name))
                        Document.SubscribePropertyChangeXamlWriterProperties(child, name);

                    //if (MainSurface is Canvas)
                    //{
                    Canvas.SetLeft(child, transformOffset.X);
                    Canvas.SetTop(child, transformOffset.Y);
                    //    child.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                    //    child.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                    //}
                    //else
                    //{
                    //    Canvas.SetLeft(child, transformOffset.X);
                    //    Canvas.SetTop(child, transformOffset.Y);
                    //    child.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                    //    child.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                    //}

                    child.Width = transformOffset.Width;
                    child.Height = transformOffset.Height;

                    child.RenderTransform = tranform;
                    // }


                    if (Document.IsSubscribePropertyChangeXamlWriterProperties(child))
                        Document.UnsubscribePropertyChangeXamlWriterProperties(child);

                    if (child.Uid is String && !String.IsNullOrEmpty(child.Uid as String))
                    {
                        child.Name = child.Uid as String;
                        child.ClearValue(FrameworkElement.UidProperty);
                    }

                    var ret = Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, child, true, false);
                    ret.Keys.ToList().ForEach(key =>
                    {
                        if (renamed.ContainsKey(key))
                            renamed.Remove(key);
                        renamed.Add(key, ret[key]);

                        //if (Document.MapScreenEntities.ContainsKey(key))
                        //{
                        //    var entity = Document.MapScreenEntities[key];
                        //    Document.MapScreenEntities.Remove(key);
                        //    if (Document.MapScreenEntities.ContainsKey(ret[key]))
                        //        Document.MapScreenEntities.Remove(ret[key]);
                        //    Document.MapScreenEntities.Add(ret[key], entity);
                        //}
                    });
                    listItemToSelect.Add(child);

                    //if (bResubscribe)
                    //    Document.SubscribePropertyChangeXamlWriterProperties(child, child.Name);
                }

                Document.TransformFromInners(MainSurface, g.Name, renamed);
                Document.RemoveEntity(g);
                Utilities.WPF.DependencyObjectExtensions.UnregisterName(MainSurface, g);

                listItemToSelect.OfType<FrameworkElement>().ToList().ForEach(fe =>
                {
                    if (Document.MapScreenEntities.ContainsKey(fe.Name) &&
                        !String.IsNullOrEmpty(Document.MapScreenEntities[fe.Name].ProblematicXaml))
                        Utilities.WPF.XmlHelper.CheckProblematicXamlWriter(fe, Document.MapScreenEntities[fe.Name].ProblematicXaml);
                    // Document.UpdateProblematicXamlWriterProperties(fe, fe.Name);
                });

                MainSurface_CleanCurrentSelectedion();
                ActiveLayer.Children.Remove(g);

                Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    Document.ResolveEntityBrushAndPen(MainSurface);
                    Document.ResolveEntityTextDecorators(MainSurface);
                });

                // Utilities.WPF.DependencyObjectExtensions.UnregisterName(MainSurface, g);

                listItemToSelect.OfType<FrameworkElement>().ToList().ForEach(fe =>
                {
                    Document.LoadRepositoryItem(this, MainSurface, fe.Name);
                });
                ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(listItemToSelect);
                MainSurface_SetCurrentSelection(readonlyList);

                MainSurface_SetModified(true);
            }
            catch (Exception)
            {
            }
        }

        public bool CheckSelectedProblematicXaml()
        {
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();
            foreach (var el in listSelected)
            {
                if (Utilities.WPF.XmlHelper.IsProblematicXamlWriter(el))
                    return false;
            }

            return true;
        }

        public void CreateGroup()
        {
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();
            if (listSelected == null || listSelected.Count < 1)
                return;

            MainSurface_CleanCurrentSelectedion();
            MainSurface_CleanWebHMIState();
            //if (!CheckSelectedProblematicXaml())
            //{
            //    EditorComponent.UIInterface.ShowError(Properties.Resources.SelectionContainsProblematicXaml);
            //    return;
            //}

            Viewbox Group = new Viewbox();
            Group.SetValue(Viewbox.StretchProperty, Stretch.Fill);
            Canvas g = new Canvas();
            Rect r = Utilities.WPF.DependencyObjectExtensions.CalculateBoundRect(listSelected, ActiveLayer);

            //if (MainSurface is Canvas)
            //{
            Canvas.SetLeft(Group, r.X);
            Canvas.SetTop(Group, r.Y);
            //    Group.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
            //    Group.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
            //}
            //else
            //{
            //    Canvas.SetLeft(Group, r.X);
            //    Canvas.SetTop(Group, r.Y);
            //    Group.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
            //    Group.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
            //}

            Group.Width = g.Width = r.Width;
            Group.Height = g.Height = r.Height;

            if (bRegrouping && !String.IsNullOrEmpty(regroupName))
                Group.Name = regroupName;
            Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, Group, true, false);

            var mapOrdered = new SortedDictionary<int, UIElement>();
            listSelected.ForEach(ch =>
                {
                    int n = ActiveLayer.Children.IndexOf(ch);
                    mapOrdered.Add(n, ch);
                });

            var listNames = new List<String>();
            mapOrdered.Keys.ToList().ForEach(index =>
            {
                var ch = mapOrdered[index];
                listNames.Add((ch as FrameworkElement).Name);
            });
            Document.TransformToInners(MainSurface, Group.Name, listNames);

            mapOrdered.Keys.ToList().ForEach(index =>
            {
                var ch = mapOrdered[index];
                Utilities.WPF.DependencyObjectExtensions.UnregisterName(MainSurface, (ch as FrameworkElement).Name);
                (ch as FrameworkElement).Uid = (ch as FrameworkElement).Name;
                (ch as FrameworkElement).ClearValue(FrameworkElement.NameProperty);
            });

            mapOrdered.Keys.ToList().ForEach(index =>
            {
                var ch = mapOrdered[index];

                Vector off = VisualTreeHelper.GetOffset(ch);
                //if (MainSurface is Canvas)
                //{
                Canvas.SetLeft(ch, Canvas.GetLeft(ch) - r.X);
                Canvas.SetTop(ch, Canvas.GetTop(ch) - r.Y);
                //    ch.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                //    ch.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                //}
                //else
                //{
                //    Canvas.SetLeft(ch, Canvas.GetLeft(ch) - r.X);
                //    Canvas.SetTop(ch, Canvas.GetTop(ch) - r.Y);
                //    ch.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                //    ch.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                //}
                ActiveLayer.Children.Remove(ch);
                g.Children.Add(ch);
            });
            Group.Child = g;
            // Group.CacheMode = new BitmapCache() { EnableClearType = true };

            Canvas.SetTop(g, 0); Canvas.SetLeft(g, 0);
            ActiveLayer.Children.Add(Group);

            // Document.LoadRepositoryItem(this, MainSurface, Group.Name);

            if (bRegrouping && entityRegroup != null)
            {
                if (Document.MapScreenEntities.ContainsKey(Group.Name))
                    Document.MapScreenEntities.Remove(Group.Name);
                Document.MapScreenEntities.Add(Group.Name, entityRegroup);
            }

            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                Document.ResolveEntityBrushAndPen(MainSurface);
                Document.ResolveEntityTextDecorators(MainSurface);
            });

            List<UIElement> listItemToSelect = new List<UIElement>();
            listItemToSelect.Add(Group);
            ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(listItemToSelect);
            MainSurface_SetCurrentSelection(readonlyList);
            readonlyList.ToList().ForEach(uie => UpdateHMIControl(uie));

            MainSurface_SetModified(true);
        }

        public void CreatePanel(Panel Group)
        {
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();
            if (listSelected == null || listSelected.Count < 1)
                return;

            MainSurface_CleanCurrentSelectedion();

            //if (!CheckSelectedProblematicXaml())
            //{
            //    EditorComponent.UIInterface.ShowError(Properties.Resources.SelectionContainsProblematicXaml);
            //    return;
            //}

            Rect r = Utilities.WPF.DependencyObjectExtensions.CalculateBoundRect(listSelected, ActiveLayer);

            //if (MainSurface is Canvas)
            //{
            Canvas.SetLeft(Group, r.X);
            Canvas.SetTop(Group, r.Y);
            //    Group.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
            //    Group.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
            //}
            //else
            //{
            //    Canvas.SetLeft(Group, r.X);
            //    Canvas.SetTop(Group, r.Y);
            //    Group.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
            //    Group.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
            //}

            Group.Width = r.Width;
            Group.Height = r.Height;

            if (bRegrouping && !String.IsNullOrEmpty(regroupName))
                Group.Name = regroupName;
            Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, Group, true, false);

            var mapOrdered = new SortedDictionary<int, UIElement>();
            listSelected.ForEach(ch =>
                {
                    int n = ActiveLayer.Children.IndexOf(ch);
                    mapOrdered.Add(n, ch);
                });

            var listNames = new List<String>();
            mapOrdered.Keys.ToList().ForEach(index =>
            {
                var ch = mapOrdered[index];
                listNames.Add((ch as FrameworkElement).Name);
            });
            Document.TransformToInners(MainSurface, Group.Name, listNames);

            mapOrdered.Keys.ToList().ForEach(index =>
            {
                var ch = mapOrdered[index];
                Utilities.WPF.DependencyObjectExtensions.UnregisterName(MainSurface, (ch as FrameworkElement).Name);
                (ch as FrameworkElement).Uid = (ch as FrameworkElement).Name;
                (ch as FrameworkElement).ClearValue(FrameworkElement.NameProperty);
            });

            mapOrdered.Keys.ToList().ForEach(index =>
            {
                var ch = mapOrdered[index];

                ActiveLayer.Children.Remove(ch);
                Group.Children.Add(ch);

                ch.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                ch.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                ch.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                ch.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                ch.SetValue(FrameworkElement.WidthProperty, DependencyProperty.UnsetValue);
                ch.SetValue(FrameworkElement.HeightProperty, DependencyProperty.UnsetValue);
                ch.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Stretch);
                ch.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            });

            ActiveLayer.Children.Add(Group);

            // Document.LoadRepositoryItem(this, MainSurface, Group.Name);

            if (bRegrouping && entityRegroup != null)
            {
                if (Document.MapScreenEntities.ContainsKey(Group.Name))
                    Document.MapScreenEntities.Remove(Group.Name);
                Document.MapScreenEntities.Add(Group.Name, entityRegroup);
            }

            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                Document.ResolveEntityBrushAndPen(MainSurface);
                Document.ResolveEntityTextDecorators(MainSurface);
            });

            List<UIElement> listItemToSelect = new List<UIElement>();
            listItemToSelect.Add(Group);
            ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(listItemToSelect);
            MainSurface_SetCurrentSelection(readonlyList);

            MainSurface_SetModified(true);
        }

        public void CreateContentControl(ContentControl Group)
        {
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();
            if (listSelected == null || listSelected.Count != 1)
                return;

            MainSurface_CleanCurrentSelectedion();

            //if (!CheckSelectedProblematicXaml())
            //{
            //    EditorComponent.UIInterface.ShowError(Properties.Resources.SelectionContainsProblematicXaml);
            //    return;
            //}

            Rect r = Utilities.WPF.DependencyObjectExtensions.CalculateBoundRect(listSelected, ActiveLayer);

            //if (MainSurface is Canvas)
            //{
            Canvas.SetLeft(Group, r.X);
            Canvas.SetTop(Group, r.Y);
            //    Group.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
            //    Group.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
            //}
            //else
            //{
            //    Canvas.SetLeft(Group, r.X);
            //    Canvas.SetTop(Group, r.Y);
            //    Group.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
            //    Group.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
            //}

            Group.Width = r.Width;
            Group.Height = r.Height;

            Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, Group, true, false);

            var ch = listSelected[0];

            var listNames = new List<String>();
            listNames.Add((ch as FrameworkElement).Name);
            Document.TransformToInners(MainSurface, Group.Name, listNames);

            Utilities.WPF.DependencyObjectExtensions.UnregisterName(MainSurface, (ch as FrameworkElement).Name);
            (ch as FrameworkElement).Uid = (ch as FrameworkElement).Name;
            (ch as FrameworkElement).ClearValue(FrameworkElement.NameProperty);

            ActiveLayer.Children.Remove(ch);
            Group.Content = ch;

            ch.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
            ch.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
            ch.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
            ch.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
            ch.SetValue(FrameworkElement.WidthProperty, DependencyProperty.UnsetValue);
            ch.SetValue(FrameworkElement.HeightProperty, DependencyProperty.UnsetValue);
            ch.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Stretch);
            ch.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);

            Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, Group, true, false);

            ActiveLayer.Children.Add(Group);

            // Document.LoadRepositoryItem(this, MainSurface, Group.Name);

            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                Document.ResolveEntityBrushAndPen(MainSurface);
                Document.ResolveEntityTextDecorators(MainSurface);
            });

            List<UIElement> listItemToSelect = new List<UIElement>();
            listItemToSelect.Add(Group);
            ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(listItemToSelect);
            MainSurface_SetCurrentSelection(readonlyList);

            MainSurface_SetModified(true);
        }

        public void CreateDecorator(Decorator Group)
        {
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();
            if (listSelected == null || listSelected.Count != 1)
                return;

            MainSurface_CleanCurrentSelectedion();

            //if (!CheckSelectedProblematicXaml())
            //{
            //    EditorComponent.UIInterface.ShowError(Properties.Resources.SelectionContainsProblematicXaml);
            //    return;
            //}

            Rect r = Utilities.WPF.DependencyObjectExtensions.CalculateBoundRect(listSelected, ActiveLayer);

            //if (MainSurface is Canvas)
            //{
            Canvas.SetLeft(Group, r.X);
            Canvas.SetTop(Group, r.Y);
            //    Group.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
            //    Group.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
            //}
            //else
            //{
            //    Canvas.SetLeft(Group, r.X);
            //    Canvas.SetTop(Group, r.Y);
            //    Group.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
            //    Group.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
            //}

            Group.Width = r.Width;
            Group.Height = r.Height;

            if (bRegrouping && !String.IsNullOrEmpty(regroupName))
                Group.Name = regroupName;
            Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, Group, true, false);

            var ch = listSelected[0];

            var listNames = new List<String>();
            listNames.Add((ch as FrameworkElement).Name);
            Document.TransformToInners(MainSurface, Group.Name, listNames);

            Utilities.WPF.DependencyObjectExtensions.UnregisterName(MainSurface, (ch as FrameworkElement).Name);
            (ch as FrameworkElement).Uid = (ch as FrameworkElement).Name;
            (ch as FrameworkElement).ClearValue(FrameworkElement.NameProperty);

            ActiveLayer.Children.Remove(ch);
            Group.Child = ch;

            ch.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
            ch.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
            ch.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
            ch.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
            ch.SetValue(FrameworkElement.WidthProperty, DependencyProperty.UnsetValue);
            ch.SetValue(FrameworkElement.HeightProperty, DependencyProperty.UnsetValue);
            ch.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Stretch);
            ch.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);

            ActiveLayer.Children.Add(Group);

            // Document.LoadRepositoryItem(this, MainSurface, Group.Name);

            if (bRegrouping && entityRegroup != null)
            {
                if (Document.MapScreenEntities.ContainsKey(Group.Name))
                    Document.MapScreenEntities.Remove(Group.Name);
                Document.MapScreenEntities.Add(Group.Name, entityRegroup);
            }

            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                Document.ResolveEntityBrushAndPen(MainSurface);
                Document.ResolveEntityTextDecorators(MainSurface);
            });

            List<UIElement> listItemToSelect = new List<UIElement>();
            listItemToSelect.Add(Group);
            ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(listItemToSelect);
            MainSurface_SetCurrentSelection(readonlyList);

            MainSurface_SetModified(true);
        }

        #region Effects

        static Dictionary<Type, UserControl> mapTypeLibraries = new Dictionary<Type, UserControl>();
        void OnStyleEditor(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            if (e.Parameter != null && e.Parameter is UIElement)
            {
                var elementsToSelect = new List<UIElement>();
                elementsToSelect.Add(e.Parameter as UIElement);
                var readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                MainSurface_SetCurrentSelection(readonlyList);
            }

            List<UIElement> listSelected = MainSurface_GetCurrentSelection();

            var uel = (listSelected[0]);
            var name = (uel as FrameworkElement).Name;
            if (String.IsNullOrEmpty(name))
                name = uel.Uid as String;

            var entity = GetSelectedEntity(listSelected[0] as FrameworkElement);
            if (entity.SourceSymbolLinked)
            {
                var styleType = (listSelected[0] as ContentControl).Content.GetType();
                if (!mapTypeLibraries.ContainsKey(styleType))
                {
                    var userControl = EditorComponent.SymbolGallery.GetStyleLibraryControl(styleType.Name, String.Empty); 
                    mapTypeLibraries.Add(styleType, userControl);
                    ResourceDictionaryExtensions.AddCommonResources(mapTypeLibraries[styleType]);
                }

                if (!String.IsNullOrEmpty(name))
                    name = styleType.Name;

                var Dialog = new GeneralDialogContent(mapTypeLibraries[styleType])
                {
                    Owner = this.FindParent<Window>(),
                    Title = String.Format("{0} - {1}", Properties.Resources.StyleEditor, name),
                    DialogKeepContent = true,
                    HelpLink = "StyleEditor"
                };
                BeginEdit();
                if (Dialog.ShowDialog() != true)
                {
                    CancelEdit();

                    return;
                }

                var provider = EditorComponent.SymbolGallery.GetCurrentSourceSymbolProvider(mapTypeLibraries[styleType]);
                string sourceSymbolPath = WPFUtilities.CryptString.CryptString.DecryptString(entity.SourceSymbolPath);
                var path = EditorComponent.SymbolGallery.GetCurrentSourceSymbolPath(mapTypeLibraries[styleType], sourceSymbolPath);

                // var element = listSelected[0] as FrameworkElement;
                var elements = (from c in listSelected.OfType<ContentControl>() where c.Content.GetType() == styleType select c).ToList();
                elements.ForEach(el =>
                    {
                        Document.SetSourceProviderPath(el, provider, path);
                        try
                        {
                            Document.LoadRepositoryItem(this, MainSurface, el.Name, bRecreateResources: true);
                        }
                        catch (Exception ex)
                        {
                            EditorComponent.UIInterface.ShowError(ex.Message);
                        }
                    });
                EndEdit();
            }
            else
            {
                Type styleType = listSelected[0].GetType();
                if (listSelected[0] is ContentControl && (listSelected[0] as ContentControl).Content is FrameworkElement)
                    styleType = (listSelected[0] as ContentControl).Content.GetType();
                StyleEditor styleEditor = new StyleEditor(styleType, EditorComponent.UIInterface);
                styleEditor.StyleChanged += (o, ev) =>
                {
                    listSelected.ForEach(ue =>
                    {
                        if (!String.IsNullOrEmpty(styleEditor.ResourceName))
                        {
                            if (MainSurface.Resources.Contains(styleEditor.ResourceName))
                                MainSurface.Resources.Remove(styleEditor.ResourceName);
                            MainSurface.Resources.Add(styleEditor.ResourceName, styleEditor.StyleEdit);

                            if (!Document.ListResources.Contains(styleEditor.ResourceFileName))
                                Document.ListResources.Add(styleEditor.ResourceFileName);

                            Document.SetDynamicEntityStyle(ue as FrameworkElement, styleEditor.ResourceName);
                        }

                        if (ue is ContentControl && (ue as ContentControl).Content is FrameworkElement)
                            ((ue as ContentControl).Content as FrameworkElement).Style = styleEditor.StyleEdit;
                        else
                            (ue as FrameworkElement).Style = styleEditor.StyleEdit;
                    });
                };

                if (!String.IsNullOrEmpty(name))
                    name = styleType.Name;

                var Dialog = new GeneralDialogContent(styleEditor)
                {
                    Owner = this.FindParent<Window>(),
                    Title = String.Format("{0} - {1}", Properties.Resources.StyleEditor, name),
                    HelpLink = "StyleEditor"
                };
                BeginEdit();
                if (Dialog.ShowDialog() != true)
                {
                    CancelEdit();

                    return;
                }

                EndEdit();
            }
        }

        internal Brush GetSelectedBrush(bool editbackbrush, bool needforeground, ref PropertyDataTemplate.BrushEditorOptions options)
        {
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();

            Brush brush = null;

            if (editbackbrush && listSelected.Count == 0)
            {
                brush = cvsBackground.Background;
                options.ItemName = Properties.Resources.Screen;
            }
            else if (listSelected.Count == 1)
            {
                var ue = listSelected[0];
                options.ItemName = ue.GetType().Name;
                var entity = GetSelectedEntity(ue as FrameworkElement);

                var selected = (entity as IEntityReference).ContainedObject;
                var name = (ue as FrameworkElement).Name;
                if (selected is ContentControl && (selected as ContentControl).Content is UIElement &&
                    !(selected is UserControl))
                {
                    selected = (selected as ContentControl).Content;
                    ue = selected as FrameworkElement;
                    if (!string.IsNullOrEmpty((ue as FrameworkElement).Name))
                        name = (ue as FrameworkElement).Name;
                    options.ItemIsSymbol = true;
                }

                if (String.IsNullOrEmpty(name) && !string.IsNullOrEmpty(ue.Uid))
                    name = ue.Uid as String;
                if (!String.IsNullOrEmpty(name))
                    options.ItemName = name;

                if (editbackbrush)
                {
                    if (ue is Panel)
                        brush = (ue as Panel).Background;
                    else if (ue is Microsoft.Expression.Media.IShape)
                        brush = (ue as Microsoft.Expression.Media.IShape).Fill;
                    else if (ue is Control)
                        brush = (ue as Control).Background;
                    else if (ue is Border)
                        brush = (ue as Border).Background;
                    else if (ue is Shape)
                        brush = (ue as Shape).Fill;

                    var plist = (from c in ue.GetVisualChildrenOfType<Panel>()
                                 where (c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0
                                 select c).ToList();
                    if (plist.Count >= 1 && options.ItemIsSymbol)
                        brush = plist[0].Background;
                    if (plist.Count > 1)
                        options.ItemIsComposed = true;

                    var clist = (from c in ue.GetVisualChildrenOfType<Control>()
                                 where (c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0
                                 select c).ToList();
                    if (clist.Count >= 1 && options.ItemIsSymbol)
                        brush = clist[0].Background;
                    if (clist.Count > 1)
                        options.ItemIsComposed = true;

                    var blist = (from c in ue.GetVisualChildrenOfType<Border>()
                                 where (c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0
                                 select c).ToList();
                    if (blist.Count >= 1 && options.ItemIsSymbol)
                        brush = blist[0].Background;
                    if (blist.Count > 1)
                        options.ItemIsComposed = true;

                    var slist = (from c in ue.GetVisualChildrenOfType<Shape>()
                                 where (c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0
                                 select c).ToList();
                    if (slist.Count >= 1 && options.ItemIsSymbol)
                        brush = slist[0].Fill;
                    if (slist.Count > 1)
                        options.ItemIsComposed = true;

                }
                else
                {
                    if (needforeground)
                    {
                        if (ue is Control)
                            brush = (ue as Control).Foreground;

                        var cflist = (from c in ue.GetVisualChildrenOfType<Control>()
                                      where (c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0
                                      select c).ToList();
                        if (cflist.Count >= 1 && options.ItemIsSymbol)
                            brush = cflist[0].Foreground;
                        if (cflist.Count > 1)
                            options.ItemIsComposed = true;
                    }
                    else
                    {
                        if (ue is Microsoft.Expression.Media.IShape)
                            brush = (ue as Microsoft.Expression.Media.IShape).Stroke;
                        else if (ue is Control)
                            brush = (ue as Control).BorderBrush;
                        else if (ue is Border)
                            brush = (ue as Border).BorderBrush;
                        else if (ue is Shape)
                            brush = (ue as Shape).Stroke;

                        var clist = (from c in ue.GetVisualChildrenOfType<Control>()
                                     where (c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0
                                     select c).ToList();
                        if (clist.Count >= 1 && options.ItemIsSymbol)
                            brush = clist[0].BorderBrush;
                        if (clist.Count > 1)
                            options.ItemIsComposed = true;

                        var blist = (from c in ue.GetVisualChildrenOfType<Border>()
                                     where (c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0
                                     select c).ToList();
                        if (blist.Count >= 1 && options.ItemIsSymbol)
                            brush = blist[0].BorderBrush;
                        if (blist.Count > 1)
                            options.ItemIsComposed = true;

                        var slist = (from c in ue.GetVisualChildrenOfType<Shape>()
                                     where (c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0
                                     select c).ToList();
                        if (slist.Count >= 1 && options.ItemIsSymbol)
                            brush = slist[0].Stroke;
                        if (slist.Count > 1)
                            options.ItemIsComposed = true;

                    }
                }
            }

            bool _bSpread = false;
            listSelected.ForEach(ue =>
            {
                var entity = GetSelectedEntity(ue as FrameworkElement);
                bool _bSymbol = false;
                var selected = (entity as IEntityReference).ContainedObject;
                if (selected is ContentControl && (selected as ContentControl).Content is UIElement &&
                    !(selected is UserControl))
                {
                    selected = (selected as ContentControl).Content;
                    ue = selected as FrameworkElement;
                    _bSymbol = true;
                }

                if (!(ue is Microsoft.Expression.Media.IShape) &&
                    !(ue is Control) &&
                    (!_bSymbol))
                {
                    _bSpread = true;
                    return;
                }
            });
            options.SpreadOnChild = _bSpread;


            return brush;
        }

        internal void SetSelectedBrush(Brush brush, List<UIElement> listSelected = null, bool bSpreadColor = false)
        {
            bool bBeginEdit = false;
            if (listSelected == null)
            {
                listSelected = MainSurface_GetCurrentSelection();
                if (listSelected.Count > 0)
                {
                    bBeginEdit = true;
                    BeginEdit();
                    listSelected.ForEach(element =>
                    {
                        var fe = element as FrameworkElement;
                        if (fe != null && !(fe is UserControl))
                        {
                            var list = fe.GetChildrenOfType<FrameworkElement>();
                            foreach (var el in list)
                            {
                                if (!Utilities.WPF.XmlHelper.IsProblematicXamlWriter(el))
                                    continue;

                                var name = GetSelectedEntityName(el);
                                if (!String.IsNullOrEmpty(name))
                                    Document.SubscribePropertyChangeXamlWriterProperties(el, name);
                            }
                        }
                    });
                }
            }
            if (listSelected.Count > 0)
            {
                listSelected.ForEach(ue =>
                {
                    //if (!String.IsNullOrEmpty(brushEditor.ResourceName))
                    //{
                    //    if (MainSurface.Resources.Contains(brushEditor.ResourceName))
                    //        MainSurface.Resources.Remove(brushEditor.ResourceName);
                    //    MainSurface.Resources.Add(brushEditor.ResourceName, brushEditor.Brush);

                    //    Document.SetDynamicEntityBrush(ue as FrameworkElement, brushEditor.ResourceName);
                    //}
                    bool bStyledSymbol = false;
                    var entity = GetSelectedEntity(ue as FrameworkElement);
                    Document.SetEntityPreservedBrush(entity, brush);
                    Document.SetDynamicEntityTagBrush(entity, brush);
                    var selected = (entity as IEntityReference).ContainedObject;
                    var contained = (selected as ContentControl);
                    if (selected is ContentControl && (selected as ContentControl).Content is UIElement &&
                        !(selected is UserControl))
                    {
                        selected = (selected as ContentControl).Content;
                        ue = selected as FrameworkElement;
                        bStyledSymbol = true;
                    }

                    if (brush != null)
                    {
                        SpreadBackgroundOnChilds(entity, ue, brush?.Clone(), bSpreadColor, bStyledSymbol);
                    }
                    else if (bStyledSymbol)
                    {
                        SpreadBackgroundOnChilds(entity, ue, brush?.Clone(), bSpreadColor, bStyledSymbol, bClearValue: true);

                        if (contained != null)
                        {
                            if (ue is Control && !(selected is UserControl))
                                (ue as Control).ClearValue(Control.BackgroundProperty);

                            if (!(selected is UserControl))
                            {
                                //Document.RestoreProblematicXamlWriter(MainSurface);
                                //Document.CleanProblematicXamlBags();
                                Document.RefreshEntityStyleBinding(MainSurface, true, contained.Name);
                                Document.UpdateRepositoryItems(this, MainSurface, true, UpdateOnlyThoseWithStyle: false, bRecreateResources: true, entityname: contained.Name); // case 10871
                            }
                        }
                    }

                });
            }
            else
            {
                //MainSurface.Background = brushEditor.Brush;
                Document.Background = brush;
            }

            if (bBeginEdit)
            {
                listSelected.ForEach(uie =>
                {
                    var fe = uie as FrameworkElement;
                    if (fe != null && !(fe is UserControl))
                    {
                        try
                        {
                            var list = fe.GetChildrenOfType<FrameworkElement>();
                            foreach (var el in list)
                            {
                                if (!Utilities.WPF.XmlHelper.IsProblematicXamlWriter(el))
                                    continue;

                                Document.UnsubscribePropertyChangeXamlWriterProperties(el);
                            }
                        }
                        catch (Exception ex)
                        {
                            EditorComponent.UIInterface.ShowError(ex.Message);
                        }
                    }
                });

                EndEdit();
            }
        }

        internal void SetSelectedPen(Brush brush, List<UIElement> listSelected = null, bool bForeground = false, bool bSpreadColor = false)
        {
            bool bBeginEdit = false;
            if (listSelected == null)
            {
                listSelected = MainSurface_GetCurrentSelection();
                if (listSelected.Count > 0)
                {
                    bBeginEdit = true;
                    BeginEdit();
                    listSelected.ForEach(element =>
                    {
                        var fe = element as FrameworkElement;
                        if (fe != null && !(fe is UserControl))
                        {
                            var list = fe.GetChildrenOfType<FrameworkElement>();
                            foreach (var el in list)
                            {
                                if (!Utilities.WPF.XmlHelper.IsProblematicXamlWriter(el))
                                    continue;

                                var name = GetSelectedEntityName(el);
                                if (!String.IsNullOrEmpty(name))
                                    Document.SubscribePropertyChangeXamlWriterProperties(el, name);
                            }
                        }
                    });
                }
            }
            if (listSelected.Count > 0)
            {
                listSelected.ForEach(ue =>
                {
                    //if (!String.IsNullOrEmpty(brushEditor.ResourceName))
                    //{
                    //    if (MainSurface.Resources.Contains(brushEditor.ResourceName))
                    //        MainSurface.Resources.Remove(brushEditor.ResourceName);
                    //    MainSurface.Resources.Add(brushEditor.ResourceName, brushEditor.Brush);

                    //    Document.SetDynamicEntityPen(ue as FrameworkElement, brushEditor.ResourceName);
                    //}
                    bool bStyledSymbol = false;
                    var entity = GetSelectedEntity(ue as FrameworkElement);
                    Document.SetEntityPreservedPen(entity, brush);
                    Document.SetDynamicEntityTagPen(entity, brush);
                    var selected = (entity as IEntityReference).ContainedObject;
                    var contained = (selected as ContentControl);
                    if (selected is ContentControl && (selected as ContentControl).Content is UIElement &&
                        !(selected is UserControl))
                    {
                        selected = (selected as ContentControl).Content;
                        ue = selected as FrameworkElement;
                        bStyledSymbol = true;
                    }

                    if (brush != null)
                    {
                        SpreadStrokeOnChilds(entity, ue, brush?.Clone(), bSpreadColor, bStyledSymbol, bForeground: bForeground);
                    }
                    else if (bStyledSymbol)
                    {
                        SpreadStrokeOnChilds(entity, ue, brush?.Clone(), bSpreadColor, bStyledSymbol, bClearValue: true, bForeground: bForeground);

                        if (contained != null)
                        {
                            if (ue is Control && !(selected is UserControl))
                            {
                                (ue as Control).ClearValue(Control.BorderBrushProperty);
                                (ue as Control).ClearValue(Control.ForegroundProperty);
                            }

                            if (!(selected is UserControl))
                            {
                                //Document.RestoreProblematicXamlWriter(MainSurface);
                                //Document.CleanProblematicXamlBags();
                                Document.RefreshEntityStyleBinding(MainSurface, true, contained.Name);
                                Document.UpdateRepositoryItems(this, MainSurface, true, UpdateOnlyThoseWithStyle: false, bRecreateResources: true, entityname: contained.Name); // case 10871
                            }
                        }
                    }
                });
            }
            if (bBeginEdit)
            {
                listSelected.ForEach(uie =>
                {
                    var fe = uie as FrameworkElement;
                    if (fe != null && !(fe is UserControl))
                    {
                        try
                        {
                            var list = fe.GetChildrenOfType<FrameworkElement>();
                            foreach (var el in list)
                            {
                                if (!Utilities.WPF.XmlHelper.IsProblematicXamlWriter(el))
                                    continue;

                                var name = GetSelectedEntityName(el);
                                Document.UnsubscribePropertyChangeXamlWriterProperties(el);

                                if (!String.IsNullOrEmpty(name))
                                    Document.UpdateProblematicXamlWriterProperties(el, name);
                            }
                        }
                        catch (Exception ex)
                        {
                            EditorComponent.UIInterface.ShowError(ex.Message);
                        }
                    }
                });

                EndEdit();
            }
        }

        internal void OnBrushEditor(object sender, ExecutedRoutedEventArgs e)
        {
            if (e != null)
                e.Handled = true;
            OpenBrushEditor();
        }
        internal void OpenBrushEditor()
        { 
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();
            Dictionary<ScreenEntity, Brush> mapEntityToBrush = new Dictionary<ScreenEntity, Brush>();
            Dictionary<UIElement, Brush> mapUeToBrush = new Dictionary<UIElement, Brush>();
            String type = String.Empty;
            Brush brush = null;
            var options = new PropertyDataTemplate.BrushEditorOptions();

            brush = GetSelectedBrush(true, false, ref options);

            listSelected.ForEach(ue =>
            {
                bool bStyledSymbol = false;
                var entity = GetSelectedEntity(ue as FrameworkElement);
                var selected = (entity as IEntityReference).ContainedObject;
                if (selected is ContentControl && (selected as ContentControl).Content is UIElement &&
                                               !(selected is UserControl))
                {
                    selected = (selected as ContentControl).Content;
                    ue = selected as FrameworkElement;
                    bStyledSymbol = true;
                }
                InitBackgroundMapOnChilds(mapUeToBrush, mapEntityToBrush, entity, ue, bStyledSymbol);
            });

            BrushEditor brushEditor = new BrushEditor(Document, EditorComponent.UIInterface, null, options.ItemIsSymbol, options.SpreadOnChild);

            if (brush != null && !options.ItemIsComposed)
                brushEditor.Brush = brush;

            brushEditor.BrushChanged += (o, ev) =>
            {
                SetSelectedBrush(brushEditor.Brush, listSelected, brushEditor.SpreadColor);
            };

            brushEditor.UndoChanges += (o, ev) =>
            {
                if (listSelected.Count > 0)
                    listSelected.ForEach(ue =>
                    {
                        bool bStyledSymbol = false;
                        var entity = GetSelectedEntity(ue as FrameworkElement);
                        var selected = (entity as IEntityReference).ContainedObject;
                        if (selected is ContentControl && (selected as ContentControl).Content is UIElement &&
                                                       !(selected is UserControl))
                        {
                            selected = (selected as ContentControl).Content;
                            ue = selected as FrameworkElement;
                            bStyledSymbol = true;
                        }
                        RestoreBackgroundMapOnChilds(mapUeToBrush, mapEntityToBrush, entity, ue, bStyledSymbol, brushEditor.SpreadColor);
                    });
                else
                    Document.Background = brush;
            };

            var Dialog = new GeneralDialogContent(brushEditor)
            {
                Owner = this.FindParent<Window>(),
                Title = String.Format("{0} - {1}", Properties.Resources.BrushEditor, options.ItemName),
                HelpLink = "BrushEditor"
            };

            brushEditor.SelectionCompleted += (o, ev) =>
            {
                Dialog.DialogResult = true;
            };

            if (listSelected.Count > 0)
            {
                BeginEdit();

                listSelected.ForEach(element =>
                {
                    var fe = element as FrameworkElement;
                    if (fe != null && !(fe is UserControl))
                    {
                        var list = fe.GetChildrenOfType<FrameworkElement>();
                        foreach (var el in list)
                        {
                            if (!Utilities.WPF.XmlHelper.IsProblematicXamlWriter(el))
                                continue;

                            var name = GetSelectedEntityName(el);
                            if (!String.IsNullOrEmpty(name))
                                Document.SubscribePropertyChangeXamlWriterProperties(el, name);
                        }
                    }
                });
            }

            var ret = Dialog.ShowDialog();
            if (ret != true)
                CancelEdit();
            if (listSelected.Count > 0)
                listSelected.ForEach(uie =>
                {
                    var fe = uie as FrameworkElement;
                    if (fe != null && !(fe is UserControl))
                    {
                        try
                        {
                            var list = fe.GetChildrenOfType<FrameworkElement>();
                            foreach (var el in list)
                            {
                                if (!Utilities.WPF.XmlHelper.IsProblematicXamlWriter(el))
                                    continue;

                                Document.UnsubscribePropertyChangeXamlWriterProperties(el);
                            }
                            if (ret != true)
                                Document.LoadRepositoryItem(this, MainSurface, fe.Name, bSetSize: false);
                        }
                        catch (Exception ex)
                        {
                            EditorComponent.UIInterface.ShowError(ex.Message);
                        }
                    }
                });
            else if (ret != true)
            {
                Document.Background = brush;
            }

            if (ret == true)
                EndEdit();
        }

        private void OnPenEditor(object sender, ExecutedRoutedEventArgs e)
        {
            if (e != null)
                e.Handled = true;
            OpenPenEditor(true);
        }
        private void OnLineEditor(object sender, ExecutedRoutedEventArgs e)
        {
            if (e != null)
                e.Handled = true;
            OpenPenEditor(false);
        }
        internal void OpenPenEditor(bool bForeground = false)
        {
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();
            Dictionary<ScreenEntity, Brush> mapEntityToBrush = new Dictionary<ScreenEntity, Brush>();
            Dictionary<UIElement, Brush> mapUeToBrush = new Dictionary<UIElement, Brush>();
            Dictionary<UIElement, Brush> mapUeToFore = new Dictionary<UIElement, Brush>();

            String type = String.Empty;
            Brush brush = null;

            var options = new PropertyDataTemplate.BrushEditorOptions();
            brush = GetSelectedBrush(false, bForeground, ref options);

            listSelected.ForEach(ue =>
            {
                bool bStyledSymbol = false;
                var entity = GetSelectedEntity(ue as FrameworkElement);
                var selected = (entity as IEntityReference).ContainedObject;
                if (selected is ContentControl && (selected as ContentControl).Content is UIElement &&
                                               !(selected is UserControl))
                {
                    selected = (selected as ContentControl).Content;
                    ue = selected as FrameworkElement;
                    bStyledSymbol = true;
                }
                InitStrokeMapOnChilds(mapUeToFore, mapUeToBrush, mapEntityToBrush, entity, ue, bStyledSymbol);
            });

            BrushEditor brushEditor = new BrushEditor(Document, EditorComponent.UIInterface, null, options.ItemIsSymbol, options.SpreadOnChild);

            if (brush != null && !options.ItemIsComposed)
                brushEditor.Brush = brush;

            brushEditor.BrushChanged += (o, ev) =>
            {
                SetSelectedPen(brushEditor.Brush, listSelected, bForeground, brushEditor.SpreadColor);
            };

            brushEditor.UndoChanges += (o, ev) =>
            {
                listSelected.ForEach(ue =>
                {
                    bool bStyledSymbol = false;
                    var entity = GetSelectedEntity(ue as FrameworkElement);
                    var selected = (entity as IEntityReference).ContainedObject;
                    if (selected is ContentControl && (selected as ContentControl).Content is UIElement &&
                                                   !(selected is UserControl))
                    {
                        selected = (selected as ContentControl).Content;
                        ue = selected as FrameworkElement;
                        bStyledSymbol = true;
                    }
                    RestoreStrokeMapOnChilds(mapUeToFore, mapUeToBrush, mapEntityToBrush, entity, ue, bStyledSymbol, brushEditor.SpreadColor);
                });
            };

            var Dialog = new GeneralDialogContent(brushEditor)
            {
                Owner = this.FindParent<Window>(),
                Title = String.Format("{0} - {1}", Properties.Resources.PenEditor, options.ItemName),
                HelpLink = "PenEditor"
            };
            brushEditor.SelectionCompleted += (o, ev) =>
            {
                Dialog.DialogResult = true;
            };

            if (listSelected.Count > 0)
            {
                BeginEdit();
                listSelected.ForEach(element =>
                {
                    var fe = element as FrameworkElement;
                    if (fe != null && !(fe is UserControl))
                    {
                        var list = fe.GetChildrenOfType<FrameworkElement>();
                        foreach (var el in list)
                        {
                            if (!Utilities.WPF.XmlHelper.IsProblematicXamlWriter(el))
                                continue;

                            var name = GetSelectedEntityName(el);
                            if (!String.IsNullOrEmpty(name))
                                Document.SubscribePropertyChangeXamlWriterProperties(el, name);
                        }
                    }
                });
            }

            var ret = Dialog.ShowDialog();
            if (ret != true)
                CancelEdit();

            listSelected.ForEach(uie =>
            {
                var fe = uie as FrameworkElement;
                if (fe != null && !(fe is UserControl))
                {
                    try
                    {
                        var list = fe.GetChildrenOfType<FrameworkElement>();
                        foreach (var el in list)
                        {
                            if (!Utilities.WPF.XmlHelper.IsProblematicXamlWriter(el))
                                continue;

                            var name = GetSelectedEntityName(el);
                            Document.UnsubscribePropertyChangeXamlWriterProperties(el);

                            if (!String.IsNullOrEmpty(name))
                                Document.UpdateProblematicXamlWriterProperties(el, name);
                        }
                        if (ret != true)
                            Document.LoadRepositoryItem(this, MainSurface, fe.Name, bSetSize: false);
                    }
                    catch (Exception ex)
                    {
                        EditorComponent.UIInterface.ShowError(ex.Message);
                    }
                }
            });

            if (ret == true)
                EndEdit();
        }
        private void InitBackgroundMapOnChilds(Dictionary<UIElement, Brush> uemap, Dictionary<ScreenEntity, Brush> map, ScreenEntity entity, UIElement ue, bool bStyledSymbol)
        {
            try
            {
                if (!map.ContainsKey(entity))
                    map.Add(entity, Document.GetEntityPreservedBrush(entity));

                if (bStyledSymbol)
                {
                    if (ue is Panel && !uemap.ContainsKey(ue))
                        uemap.Add(ue, (ue as Panel).Background);
                    else if (ue is Microsoft.Expression.Media.IShape && !uemap.ContainsKey(ue))
                        uemap.Add(ue, (ue as Microsoft.Expression.Media.IShape).Fill);
                    else if (ue is Control && !uemap.ContainsKey(ue))
                        uemap.Add(ue, (ue as Control).Background);
                    else if (ue is Border && !uemap.ContainsKey(ue))
                        uemap.Add(ue, (ue as Border).Background);
                    else if (ue is Shape && !uemap.ContainsKey(ue))
                        uemap.Add(ue, (ue as Shape).Fill);

                    (from c in ue.GetVisualChildrenOfType<Panel>()
                     where ((c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0)
                     select c).ToList().ForEach(child =>
                     {
                         if (!uemap.ContainsKey(child))
                             uemap.Add(child, child.Background);
                     });

                    (from c in ue.GetVisualChildrenOfType<Control>()
                     where ((c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0)
                     select c).ToList().ForEach(child =>
                     {
                         if (!uemap.ContainsKey(child))
                             uemap.Add(child, child.Background);
                     });

                    (from c in ue.GetVisualChildrenOfType<Border>()
                     where ((c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0)
                     select c).ToList().ForEach(child =>
                     {
                         if (!uemap.ContainsKey(child))
                             uemap.Add(child, child.Background);
                     });

                    (from c in ue.GetVisualChildrenOfType<Shape>()
                     where ((c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0)
                     select c).ToList().ForEach(child =>
                     {
                         if (!uemap.ContainsKey(child))
                             uemap.Add(child, child.Fill);
                     });
                }
                else
                {
                    if (ue is Panel && !uemap.ContainsKey(ue))
                        uemap.Add(ue, (ue as Panel).Background);
                    else if (ue is Microsoft.Expression.Media.IShape && !uemap.ContainsKey(ue))
                        uemap.Add(ue, (ue as Microsoft.Expression.Media.IShape).Fill);
                    else if (ue is Control && !uemap.ContainsKey(ue))
                        uemap.Add(ue, (ue as Control).Background);
                    else if (ue is Border && !uemap.ContainsKey(ue))
                        uemap.Add(ue, (ue as Border).Background);
                    else if (ue is Shape && !uemap.ContainsKey(ue))
                        uemap.Add(ue, (ue as Shape).Fill);

                    var selected = (entity as IEntityReference).ContainedObject;
                    if (selected is ContentControl && selected is ButtonBase)
                        return;

                    if (!(selected is UserControl))
                    {
                        var rootchildren = (from c in ue.GetVisualChildrenOfType<UIElement>()
                                            select c).FirstOrDefault();

                        if (rootchildren != null && !(ue is UserControl))
                            ue = rootchildren;

                        (from c in ue.GetChildrenOfTypeBreadthFirst<UIElement>()
                         select c).ToList().ForEach(child =>
                         {
                             var _entity = GetSelectedEntity(child as FrameworkElement);
                             var _selected = (_entity as IEntityReference).ContainedObject;
                             bool bLStyledSymbol = false;
                             if (_selected is ContentControl && (_selected as ContentControl).Content is UIElement &&
                                                             !(_selected is UserControl))
                             {
                                 _selected = (_selected as ContentControl).Content;
                                 child = _selected as FrameworkElement;
                                 bLStyledSymbol = true;
                             }
                             InitBackgroundMapOnChilds(uemap, map, _entity, child, bLStyledSymbol);
                         });

                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void InitStrokeMapOnChilds(Dictionary<UIElement, Brush> mapUeToFore, Dictionary<UIElement, Brush> uemap, Dictionary<ScreenEntity, Brush> map, ScreenEntity entity, UIElement ue, bool bStyledSymbol)
        {
            try
            {
                if (!map.ContainsKey(entity))
                    map.Add(entity, Document.GetEntityPreservedPen(entity));

                if (bStyledSymbol)
                {
                    if (ue is Microsoft.Expression.Media.IShape && !uemap.ContainsKey(ue))
                        uemap.Add(ue, (ue as Microsoft.Expression.Media.IShape).Stroke);
                    else if (ue is Control && !uemap.ContainsKey(ue))
                        uemap.Add(ue, (ue as Control).BorderBrush);
                    else if (ue is Border && !uemap.ContainsKey(ue))
                        uemap.Add(ue, (ue as Border).BorderBrush);
                    else if (ue is Shape && !uemap.ContainsKey(ue))
                        uemap.Add(ue, (ue as Shape).Stroke);

                    if (ue is Control && !mapUeToFore.ContainsKey(ue))
                        mapUeToFore.Add(ue, (ue as Control).Foreground);

                    (from c in ue.GetVisualChildrenOfType<Control>()
                     where ((c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0)
                     select c).ToList().ForEach(child =>
                     {
                         if (!uemap.ContainsKey(child))
                             uemap.Add(child, child.BorderBrush);
                         if (!mapUeToFore.ContainsKey(child))
                             mapUeToFore.Add(child, child.Foreground);
                     });

                    (from c in ue.GetVisualChildrenOfType<Border>()
                     where ((c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0)
                     select c).ToList().ForEach(child =>
                     {
                         if (!uemap.ContainsKey(child))
                             uemap.Add(child, child.BorderBrush);
                     });

                    (from c in ue.GetVisualChildrenOfType<Shape>()
                     where ((c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0)
                     select c).ToList().ForEach(child =>
                     {
                         if (!uemap.ContainsKey(child))
                             uemap.Add(child, child.Stroke);
                     });
                }
                else
                {
                    if (ue is Microsoft.Expression.Media.IShape && !uemap.ContainsKey(ue))
                        uemap.Add(ue, (ue as Microsoft.Expression.Media.IShape).Stroke);
                    else if (ue is Control && !uemap.ContainsKey(ue))
                        uemap.Add(ue, (ue as Control).BorderBrush);
                    else if (ue is Border && !uemap.ContainsKey(ue))
                        uemap.Add(ue, (ue as Border).BorderBrush);
                    else if (ue is Shape && !uemap.ContainsKey(ue))
                        uemap.Add(ue, (ue as Shape).Stroke);

                    if (ue is Control && !mapUeToFore.ContainsKey(ue))
                        mapUeToFore.Add(ue, (ue as Control).Foreground);

                    var selected = (entity as IEntityReference).ContainedObject;
                    if (selected is ContentControl && selected is ButtonBase)
                        return;

                    if (!(selected is UserControl))
                    {
                        var rootchildren = (from c in ue.GetVisualChildrenOfType<UIElement>()
                                            select c).FirstOrDefault();

                        if (rootchildren != null && !(ue is UserControl))
                            ue = rootchildren;

                        (from c in ue.GetChildrenOfTypeBreadthFirst<UIElement>()
                         select c).ToList().ForEach(child =>
                         {
                             var _entity = GetSelectedEntity(child as FrameworkElement);
                             var _selected = (_entity as IEntityReference).ContainedObject;
                             bool bLStyledSymbol = false;
                             if (_selected is ContentControl && (_selected as ContentControl).Content is UIElement &&
                                                             !(_selected is UserControl))
                             {
                                 _selected = (_selected as ContentControl).Content;
                                 child = _selected as FrameworkElement;
                                 bLStyledSymbol = true;
                             }
                             InitStrokeMapOnChilds(mapUeToFore, uemap, map, _entity, child, bLStyledSymbol);
                         });

                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void RestoreStrokeMapOnChilds(Dictionary<UIElement, Brush> mapUeToFore, Dictionary<UIElement, Brush> uemap, Dictionary<ScreenEntity, Brush> map, ScreenEntity entity, UIElement ue, bool bStyledSymbol, bool bSpreadColor = false)
        {
            try
            {
                if (bSpreadColor && map.ContainsKey(entity))
                {
                    Document.SetEntityPreservedPen(entity, map[entity]);
                    Document.SetDynamicEntityTagPen(entity, map[entity]);
                }

                if (bStyledSymbol)
                {
                    if (uemap.ContainsKey(ue))
                    {
                        Brush ubrush = uemap[ue];
                        if (ue is Microsoft.Expression.Media.IShape)
                            (ue as Microsoft.Expression.Media.IShape).Stroke = ubrush;
                        else if (ue is Control)
                            (ue as Control).BorderBrush = ubrush;
                        else if (ue is Border)
                            (ue as Border).BorderBrush = ubrush;
                        else if (ue is Shape)
                            (ue as Shape).Stroke = ubrush;

                    }
                    if (mapUeToFore.ContainsKey(ue) && ue is Control)
                        (ue as Control).Foreground = mapUeToFore[ue];

                    (from c in ue.GetVisualChildrenOfType<Control>()
                     where ((c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0)
                     select c).ToList().ForEach(child =>
                     {
                         if (uemap.ContainsKey(child))
                             child.BorderBrush = uemap[child];
                         if (mapUeToFore.ContainsKey(child))
                             child.Foreground = mapUeToFore[child];
                     });

                    (from c in ue.GetVisualChildrenOfType<Border>()
                     where ((c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0)
                     select c).ToList().ForEach(child =>
                     {
                         if (uemap.ContainsKey(child))
                             child.BorderBrush = uemap[child];
                     });

                    (from c in ue.GetVisualChildrenOfType<Shape>()
                     where ((c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0)
                     select c).ToList().ForEach(child =>
                     {
                         if (uemap.ContainsKey(child))
                             child.Stroke = uemap[child];
                     });

                }
                else
                {
                    if (uemap.ContainsKey(ue))
                    {
                        Brush ubrush = uemap[ue];
                        if (ue is Microsoft.Expression.Media.IShape)
                            (ue as Microsoft.Expression.Media.IShape).Stroke = ubrush;
                        else if (ue is Control)
                            (ue as Control).BorderBrush = ubrush;
                        else if (ue is Border)
                            (ue as Border).BorderBrush = ubrush;
                        else if (ue is Shape)
                            (ue as Shape).Stroke = ubrush;

                    }
                    if (mapUeToFore.ContainsKey(ue) && ue is Control)
                        (ue as Control).Foreground = mapUeToFore[ue];

                    if (bSpreadColor)
                    {
                        var selected = (entity as IEntityReference).ContainedObject;
                        if (selected is ContentControl && selected is ButtonBase)
                            return;

                        if (!(selected is UserControl))
                        {
                            var rootchildren = (from c in ue.GetVisualChildrenOfType<UIElement>()
                                                select c).FirstOrDefault();

                            if (rootchildren != null && !(ue is UserControl))
                                ue = rootchildren;

                            (from c in ue.GetChildrenOfTypeBreadthFirst<UIElement>()
                             select c).ToList().ForEach(child =>
                             {
                                 var _entity = GetSelectedEntity(child as FrameworkElement);
                                 var _selected = (_entity as IEntityReference).ContainedObject;
                                 bool bLStyledSymbol = false;
                                 if (_selected is ContentControl && (_selected as ContentControl).Content is UIElement &&
                                                                 !(_selected is UserControl))
                                 {
                                     _selected = (_selected as ContentControl).Content;
                                     child = _selected as FrameworkElement;
                                     bLStyledSymbol = true;
                                 }
                                 RestoreStrokeMapOnChilds(mapUeToFore, uemap, map, _entity, child, bLStyledSymbol, bSpreadColor);
                             });

                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void RestoreBackgroundMapOnChilds(Dictionary<UIElement, Brush> uemap, Dictionary<ScreenEntity, Brush> map, ScreenEntity entity, UIElement ue, bool bStyledSymbol, bool bSpreadColor = false)
        {
            try
            {
                if (bSpreadColor && map.ContainsKey(entity))
                {
                    Document.SetEntityPreservedBrush(entity, map[entity]);
                    Document.SetDynamicEntityTagBrush(entity, map[entity]);
                }

                if (bStyledSymbol)
                {
                    if (uemap.ContainsKey(ue))
                    {
                        Brush ubrush = uemap[ue];
                        if (ue is Panel)
                            (ue as Panel).Background = ubrush;
                        else if (ue is Microsoft.Expression.Media.IShape)
                            (ue as Microsoft.Expression.Media.IShape).Fill = ubrush;
                        else if (ue is Control)
                            (ue as Control).Background = ubrush;
                        else if (ue is Border)
                            (ue as Border).Background = ubrush;
                        else if (ue is Shape)
                            (ue as Shape).Fill = ubrush;

                        if (ubrush is VisualBrush)
                            SetBaseUri(ue as FrameworkElement);

                    }
                    (from c in ue.GetVisualChildrenOfType<Panel>()
                     where ((c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0)
                     select c).ToList().ForEach(child =>
                     {
                         if (!uemap.ContainsKey(child))
                             return;
                         Brush brush = uemap[child];
                         child.Background = brush;
                         if (brush is VisualBrush)
                             SetBaseUri(child as FrameworkElement);
                     });

                    (from c in ue.GetVisualChildrenOfType<Control>()
                     where ((c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0)
                     select c).ToList().ForEach(child =>
                     {
                         if (!uemap.ContainsKey(child))
                             return;
                         Brush brush = uemap[child];
                         child.Background = brush;
                         if (brush is VisualBrush)
                             SetBaseUri(child as FrameworkElement);
                     });

                    (from c in ue.GetVisualChildrenOfType<Border>()
                     where ((c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0)
                     select c).ToList().ForEach(child =>
                     {
                         if (!uemap.ContainsKey(child))
                             return;
                         Brush brush = uemap[child];
                         child.Background = brush;
                         if (brush is VisualBrush)
                             SetBaseUri(child as FrameworkElement);
                     });

                    (from c in ue.GetVisualChildrenOfType<Shape>()
                     where ((c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0)
                     select c).ToList().ForEach(child =>
                     {
                         if (!uemap.ContainsKey(child))
                             return;
                         Brush brush = uemap[child];
                         child.Fill = brush;
                         if (brush is VisualBrush)
                             SetBaseUri(child as FrameworkElement);
                     });
                }
                else
                {
                    if (uemap.ContainsKey(ue))
                    {
                        Brush ubrush = uemap[ue];
                        if (ue is Panel)
                            (ue as Panel).Background = ubrush;
                        else if (ue is Microsoft.Expression.Media.IShape)
                            (ue as Microsoft.Expression.Media.IShape).Fill = ubrush;
                        else if (ue is Control)
                            (ue as Control).Background = ubrush;
                        else if (ue is Border)
                            (ue as Border).Background = ubrush;
                        else if (ue is Shape)
                            (ue as Shape).Fill = ubrush;

                        if (ubrush is VisualBrush)
                            SetBaseUri(ue as FrameworkElement);
                    }

                    if (bSpreadColor)
                    {
                        var selected = (entity as IEntityReference).ContainedObject;
                        if (selected is ContentControl && selected is ButtonBase)
                            return;

                        if (!(selected is UserControl))
                        {
                            var rootchildren = (from c in ue.GetVisualChildrenOfType<UIElement>()
                                                select c).FirstOrDefault();

                            if (rootchildren != null && !(ue is UserControl))
                                ue = rootchildren;

                            (from c in ue.GetChildrenOfTypeBreadthFirst<UIElement>()
                             select c).ToList().ForEach(child =>
                             {
                                 var _entity = GetSelectedEntity(child as FrameworkElement);
                                 var _selected = (_entity as IEntityReference).ContainedObject;
                                 bool bLStyledSymbol = false;
                                 if (_selected is ContentControl && (_selected as ContentControl).Content is UIElement &&
                                                                 !(_selected is UserControl))
                                 {
                                     _selected = (_selected as ContentControl).Content;
                                     child = _selected as FrameworkElement;
                                     bLStyledSymbol = true;
                                 }
                                 RestoreBackgroundMapOnChilds(uemap, map, _entity, child, bLStyledSymbol, bSpreadColor);
                             });

                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void SpreadBackgroundOnChilds(ScreenEntity entity, UIElement ue, Brush brush = null, bool bSpreadColor = false, bool bStyledSymbol = false, bool bClearValue = false)
        {
            try
            {
                if (bSpreadColor)
                {
                    Document.SetEntityPreservedBrush(entity, brush);
                    Document.SetDynamicEntityTagBrush(entity, brush);
                }

                if (bStyledSymbol)
                {
                    if (bClearValue)
                    {
                        (from c in ue.GetVisualChildrenOfType<Panel>()
                         where ((c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0)
                         select c).ToList().ForEach(child =>
                         {
                             child.ClearValue(Panel.BackgroundProperty);
                         });

                        (from c in ue.GetVisualChildrenOfType<Control>()
                         where ((c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0)
                         select c).ToList().ForEach(child =>
                         {
                             child.ClearValue(Control.BackgroundProperty);
                         });

                        (from c in ue.GetVisualChildrenOfType<Border>()
                         where ((c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0)
                         select c).ToList().ForEach(child =>
                         {
                             child.ClearValue(Border.BackgroundProperty);
                         });

                        (from c in ue.GetVisualChildrenOfType<Shape>()
                         where ((c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0)
                         select c).ToList().ForEach(child =>
                         {
                             child.ClearValue(Shape.FillProperty);
                         });
                    }
                    else
                    {
                        if (ue is Panel)
                            (ue as Panel).Background = brush;
                        else if (ue is Microsoft.Expression.Media.IShape)
                            (ue as Microsoft.Expression.Media.IShape).Fill = brush;
                        else if (ue is Control)
                            (ue as Control).Background = brush;
                        else if (ue is Border)
                            (ue as Border).Background = brush;
                        else if (ue is Shape)
                            (ue as Shape).Fill = brush;

                        if (brush is VisualBrush)
                            SetBaseUri(ue as FrameworkElement);

                        (from c in ue.GetVisualChildrenOfType<Panel>()
                         where ((c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0)
                         select c).ToList().ForEach(child =>
                         {
                             child.Background = brush;
                             if (brush is VisualBrush)
                                 SetBaseUri(child as FrameworkElement);
                         });

                        (from c in ue.GetVisualChildrenOfType<Control>()
                         where ((c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0)
                         select c).ToList().ForEach(child =>
                         {
                             child.Background = brush;
                             if (brush is VisualBrush)
                                 SetBaseUri(child as FrameworkElement);
                         });

                        (from c in ue.GetVisualChildrenOfType<Border>()
                         where ((c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0)
                         select c).ToList().ForEach(child =>
                         {
                             child.Background = brush;
                             if (brush is VisualBrush)
                                 SetBaseUri(child as FrameworkElement);
                         });

                        (from c in ue.GetVisualChildrenOfType<Shape>()
                         where ((c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0)
                         select c).ToList().ForEach(child =>
                         {
                             child.Fill = brush;
                             if (brush is VisualBrush)
                                 SetBaseUri(child as FrameworkElement);
                         });
                    }
                }
                else
                {
                    if (ue is Panel)
                        (ue as Panel).Background = brush;
                    else if (ue is Microsoft.Expression.Media.IShape)
                        (ue as Microsoft.Expression.Media.IShape).Fill = brush;
                    else if (ue is Control)
                        (ue as Control).Background = brush;
                    else if (ue is Border)
                        (ue as Border).Background = brush;
                    else if (ue is Shape)
                        (ue as Shape).Fill = brush;

                    if (brush is VisualBrush)
                        SetBaseUri(ue as FrameworkElement);

                    if (bSpreadColor)
                    {
                        var selected = (entity as IEntityReference).ContainedObject;
                        if (selected is ContentControl && selected is ButtonBase)
                            return;

                        if (!(selected is UserControl))
                        {
                            var rootchildren = (from c in ue.GetVisualChildrenOfType<UIElement>()
                                                select c).FirstOrDefault();

                            if (rootchildren != null && !(ue is UserControl))
                                ue = rootchildren;

                            (from c in ue.GetChildrenOfTypeBreadthFirst<UIElement>()
                             select c).ToList().ForEach(child =>
                             {
                                 var _entity = GetSelectedEntity(child as FrameworkElement);
                                 var _selected = (_entity as IEntityReference).ContainedObject;
                                 bool bLStyledSymbol = false;
                                 if (_selected is ContentControl && (_selected as ContentControl).Content is UIElement &&
                                                                 !(_selected is UserControl))
                                 {
                                     _selected = (_selected as ContentControl).Content;
                                     child = _selected as FrameworkElement;
                                     bLStyledSymbol = true;
                                 }
                                 SpreadBackgroundOnChilds(_entity, child, brush, bSpreadColor, bLStyledSymbol, false);
                             });

                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void SpreadStrokeOnChilds(ScreenEntity entity, UIElement ue, Brush brush = null, bool bSpreadColor = false, bool bStyledSymbol = false, bool bClearValue = false, bool bForeground = false)
        {
            try
            {
                if (bSpreadColor)
                {
                    Document.SetEntityPreservedPen(entity, brush);
                    Document.SetDynamicEntityTagPen(entity, brush);
                }

                if (bStyledSymbol)
                {
                    if (bClearValue)
                    {
                        if (!bForeground)
                        {
                            (from c in ue.GetVisualChildrenOfType<Control>()
                             where ((c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0)
                             select c).ToList().ForEach(child =>
                             {
                                 child.ClearValue(Control.BorderBrushProperty);
                             });

                            (from c in ue.GetVisualChildrenOfType<Border>()
                             where ((c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0)
                             select c).ToList().ForEach(child =>
                             {
                                 child.ClearValue(Border.BorderBrushProperty);
                             });

                            (from c in ue.GetVisualChildrenOfType<Shape>()
                             where ((c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0)
                             select c).ToList().ForEach(child =>
                             {
                                 child.ClearValue(Shape.StrokeProperty);
                             });
                        }
                        else
                        {
                            (from c in ue.GetVisualChildrenOfType<Control>()
                             where ((c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0)
                             select c).ToList().ForEach(child =>
                             {
                                 child.ClearValue(Control.ForegroundProperty);
                             });
                        }
                    }
                    else
                    {
                        if (bForeground)
                        {
                            if (ue is Control)
                            {
                                (ue as Control).Foreground = brush;
                            }

                            (from c in ue.GetVisualChildrenOfType<Control>()
                             where ((c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0)
                             select c).ToList().ForEach(child =>
                             {
                                 child.Foreground = brush;
                             });
                        }
                        else
                        {
                            if (ue is Microsoft.Expression.Media.IShape)
                                (ue as Microsoft.Expression.Media.IShape).Stroke = brush;
                            else if (ue is Control)
                            {
                                (ue as Control).BorderBrush = brush;
                            }
                            else if (ue is Border)
                                (ue as Border).BorderBrush = brush;
                            else if (ue is Shape)
                                (ue as Shape).Stroke = brush;

                            (from c in ue.GetVisualChildrenOfType<Control>()
                             where ((c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0)
                             select c).ToList().ForEach(child =>
                             {
                                 child.BorderBrush = brush;
                             });
                            (from c in ue.GetVisualChildrenOfType<Border>()
                             where ((c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0)
                             select c).ToList().ForEach(child =>
                             {
                                 child.BorderBrush = brush;
                             });
                            (from c in ue.GetVisualChildrenOfType<Shape>()
                             where ((c.Tag as String) == ScreenSettings.Properties.Settings.Default.BackgroundTag && c.Opacity != 0)
                             select c).ToList().ForEach(child =>
                             {
                                 child.Stroke = brush;
                             });
                        }
                    }
                }
                else
                {
                    if (bForeground)
                    {
                        if (ue is Control)
                        {
                            (ue as Control).Foreground = brush;
                        }
                    }
                    else
                    {
                        if (ue is Microsoft.Expression.Media.IShape)
                            (ue as Microsoft.Expression.Media.IShape).Stroke = brush;
                        else if (ue is Control)
                        {
                            (ue as Control).BorderBrush = brush;
                        }
                        else if (ue is Border)
                            (ue as Border).BorderBrush = brush;
                        else if (ue is Shape)
                            (ue as Shape).Stroke = brush;
                    }

                    if (bSpreadColor)
                    {
                        var selected = (entity as IEntityReference).ContainedObject;
                        if (selected is ContentControl && selected is ButtonBase)
                            return;

                        if (!(selected is UserControl))
                        {
                            var rootchildren = (from c in ue.GetVisualChildrenOfType<UIElement>()
                                                select c).FirstOrDefault();

                            if (rootchildren != null && !(ue is UserControl))
                                ue = rootchildren;

                            (from c in ue.GetChildrenOfTypeBreadthFirst<UIElement>()
                             select c).ToList().ForEach(child =>
                             {
                                 var _entity = GetSelectedEntity(child as FrameworkElement, true);
                                 var _selected = (_entity as IEntityReference).ContainedObject;
                                 bool bLStyledSymbol = false;
                                 if (_selected is ContentControl && (_selected as ContentControl).Content is UIElement &&
                                                                 !(_selected is UserControl))
                                 {
                                     _selected = (_selected as ContentControl).Content;
                                     child = _selected as FrameworkElement;
                                     bLStyledSymbol = true;
                                 }
                                 SpreadStrokeOnChilds(_entity, child, brush, bSpreadColor, bLStyledSymbol, false, bForeground: bForeground);
                             });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        void OnShadowEffect(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();

            using (new ChangeScreenWatcher(this))
            {
                listSelected.ForEach(ue =>
                {
                    if (e.Parameter == null)
                        ue.Effect = null;
                    else
                    {
                        var effect = new DropShadowEffect();
                        var str = e.Parameter as String;
                        var values = str.Split(',');
                        effect.ShadowDepth = Convert.ToInt32(values[0]);
                        if (values.Length > 1)
                            effect.Direction = Convert.ToInt32(values[1]);
                        ue.Effect = effect;
                        ue.ClipToBounds = false;
                    }
                });
            }
        }

        void OnOuterGlowEffect(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();

            using (new ChangeScreenWatcher(this))
            {
                listSelected.ForEach(ue =>
                {
                    if (e.Parameter == null)
                        ue.Effect = null;
                    else
                    {
                        var effect = new DropShadowEffect();
                        effect.ShadowDepth = 0;
                        effect.BlurRadius = Convert.ToInt32(e.Parameter);
                        ue.Effect = effect;
                        ue.ClipToBounds = false;
                    }
                });
            }
        }

        void OnBlurEffect(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();

            using (new ChangeScreenWatcher(this))
            {
                listSelected.ForEach(ue =>
                {
                    if (e.Parameter == null)
                        ue.Effect = null;
                    else
                    {
                        var effect = new BlurEffect();
                        effect.Radius = Convert.ToInt32(e.Parameter);
                        effect.KernelType = KernelType.Gaussian;
                        ue.Effect = effect;
                        ue.ClipToBounds = false;
                    }
                });
            }
        }

        void OnReflectionEffect(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();

            using (new ChangeScreenWatcher(this))
            {
                listSelected.ForEach(ue =>
                {
                    if (ue.Effect != null)
                        ue.Effect = null;
                    else
                    {
                        var fe = ue as FrameworkElement;

                        var effect = new ShaderEffects.ReflectionEffect();
                        effect.CenterX = 0.5;
                        effect.LeftAngle = 0;
                        effect.RightAngle = 0;
                        effect.Height = 1;
                        ue.Effect = effect;
                        ue.ClipToBounds = false;

                        // Bind the height of the rectangle to the border height.
                        Binding heightBinding = new Binding
                        {
                            Path = new PropertyPath("ActualHeight"),
                            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(FrameworkElement), 1)
                        };
                        BindingOperations.SetBinding(effect, ShaderEffects.ReflectionEffect.InputHeightProperty, heightBinding);
                    }
                });
            }
        }

        void OnStrokeThickness(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();
            using (new ChangeScreenWatcher(this))
            {
                listSelected.ForEach(uie =>
                    {
                        if (uie is Shape)
                        {
                            Shape shape = uie as Shape;
                            double thickness = 0;
                            if (e.Parameter != null)
                                thickness = Convert.ToDouble(e.Parameter);

                            if (thickness == 0 && (shape is Polygon || shape is Rectangle || shape is Ellipse) || thickness > 0)
                                shape.StrokeThickness = thickness;
                        }
                    });
            }
        }

        void OnStrokeDashArray(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();
            using (new ChangeScreenWatcher(this))
            {
                listSelected.ForEach(uie =>
                {
                    if (uie is Shape)
                    {
                        Shape shape = uie as Shape;
                        DoubleCollection dc = new DoubleCollection();
                        if (e.Parameter != null)
                            dc = DoubleCollection.Parse(e.Parameter as String);

                        shape.StrokeDashArray = dc;
                    }
                });
            }
        }

        void CanApplyLineStyle(object sender, CanExecuteRoutedEventArgs e)
        {
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();
            foreach (var uie in listSelected)
                if (uie is Shape)
                {
                    e.CanExecute = true;
                    break;
                }
        }

        void CanApplyPenEditor(object sender, CanExecuteRoutedEventArgs e)
        {
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();
            foreach (var uie in listSelected)
                if (uie is Shape || uie is Control || uie is Microsoft.Expression.Media.IShape)
                {
                    e.CanExecute = true;
                    break;
                }
        }

        #endregion Effects

        private void MainSurface_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Delta > 0)
                    UIGeneralCommands.ZoomIn.Execute(null, this);
                else
                    UIGeneralCommands.ZoomOut.Execute(null, this);
                e.Handled = true;
            }
        }

        /*
        private void Strokes_StrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            if (MainSurface.EditingMode != CanvasEditingMode.InkAndGesture ||
                EditorComponent.ToolBox != null && EditorComponent.ToolBox.ActiveToolCode == null)
                return;

#if CanvasEnabled
            if (e.Added.Count > 0)
            {
                analyzer.AddStrokes(e.Added);
                analyzer.SetStrokesType(e.Added, StrokeType.Drawing);
            }
            if (e.Removed.Count > 0)
            {
                analyzer.RemoveStrokes(e.Removed);
            }
            analyzer.BackgroundAnalyze();

            GestureResultAdorner.ShowMessage("Analyzing Gesture", e.Added.GetBounds().TopLeft);
#endif // CanvasEnabled
        }

#if CanvasEnabled
        void analyzer_ResultsUpdated(object sender, ResultsUpdatedEventArgs e)
        {
            AddShapes(analyzer.RootNode);
        }

        private void AddShapes(ContextNode node)
        {
            if (EditorComponent.ToolBox == null || EditorComponent.ToolBox.ActiveToolCode == null)
                return;

            if (node is InkDrawingNode)
            {
                InkDrawingNode drawingNode = (InkDrawingNode)node;
                string name = drawingNode.GetShapeName();
                List<UIElement> elementsToSelect = new List<UIElement>();
                if (name == "Rectangle" || name == "Square")
                {
                    String xamlData = EditorComponent.ToolBox.ActiveToolCode;

                    FrameworkElement element = xamlData.ReadUIElement() as FrameworkElement;
                    element.IsHitTestVisible = true;

                    Rect rect = drawingNode.Strokes.GetBounds();

                    Canvas.SetTop(element, rect.Top);
                    Canvas.SetLeft(element, rect.Left);
                    element.Width = rect.Width;
                    element.Height = rect.Height;

                    ActiveLayer.Children.Add(element);

                    elementsToSelect.Add(element);
                }
                else if (name == "Circle")
                {
                    /*
                    StringReader strreader = new StringReader(WPFtextEdit.Text);
                    XmlTextReader xmlreader = new XmlTextReader(strreader);
                    try
                    {
                        object obj = XamlReader.Load(xmlreader);

                        if (obj is Window)
                        {
                            Window win = obj as Window;

                            ActiveLayer.Content = win.Content;
                            // statusParse.Content = "Press F7 to display Window";
                        }
                        else
                        {
                            win = null;
                            try
                            {
                                WPFelementHost.Child = obj as UIElement;
                            }
                            catch (System.Exception e)
                            {
                            }
                            // statusParse.Content = "OK";
                        }

                        DependencyObject dependencyObj = obj as DependencyObject;
                        TreeListNode node = WPFVisualTree.AppendNode(new object[] { dependencyObj.DependencyObjectType.Name }, null, dependencyObj);
                        node.StateImageIndex = 0;
                        // int nChildren = LogicalTreeHelper.GetChildrenCount(dependencyObj);
                        node.HasChildren = HasChildren(dependencyObj);
                        // if (node.HasChildren)
                        //     node.Tag = true;
                        // FillTreeList(obj, null);
                    }
                    catch (Exception exc)
                    {
                        WPFtextEdit.ForeColor = System.Drawing.Color.Red;
                        // statusParse.Content = exc.Message;
                    }

                    /*
                    CircularGauge gauge = new CircularGauge();

                    Rect rect = drawingNode.Strokes.GetBounds();

                    Canvas.SetTop(gauge, rect.Top);
                    Canvas.SetLeft(gauge, rect.Left);
                    gauge.Width = rect.Width;
                    gauge.Height = rect.Height;

                    gauge.HorizontalAlignment = HorizontalAlignment.Center;
                    gauge.Radius = 170;
                    gauge.FrameType = GaugeFrameType.FullCircle;

                    CircularScale scale = new CircularScale();
                    scale.ShadowOffset = 1;
                    scale.Minimum = 0;
                    scale.Maximum = 100;
                    scale.MinorIntervalValue = 2;
                    scale.MajorIntervalValue = 10;
                    scale.StartAngle = 120;
                    scale.GapSweepAngle = 300;
                    scale.ScaleBarSize = 5;
                    scale.Radius = 130;
                    scale.BorderWidth = 1.2;
                    scale.BorderBrush = Brushes.PeachPuff;
                    scale.ShadowOffset = 2.5;
                    gauge.Scales.Add(scale);

                    CircularPointer pointer = new CircularPointer();
                    pointer.BorderWidth = 0.3;
                    pointer.PointerWidth = 10;
                    pointer.PointerLength = 10;
                    pointer.HorizontalAlignment = HorizontalAlignment.Right;
                    pointer.PointerPlacement = ScalePlacement.Outside;
                    scale.Pointers.Add(pointer);

                    ActiveLayer.Children.Add(gauge);

                    elementsToSelect.Add(gauge);
                     *//*
                }

                ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                MainSurface_SetCurrentSelection(readonlyList);
                MainSurface.Strokes.Remove(drawingNode.Strokes);
            }
            foreach (ContextNode subNode in node.SubNodes)
            {
                AddShapes(subNode);
            }
            MainSurface_SetModified(true);
        }
#endif // CanvasEnabled

        private void MainSurface_Gesture(object sender, CanvasGestureEventArgs e)
        {
            ReadOnlyCollection<GestureRecognitionResult> results = e.GetGestureRecognitionResults();

            if (results.Count != 0 && results[0].RecognitionConfidence == RecognitionConfidence.Strong)
            {
                if (ActiveLayer.EditingMode == CanvasEditingMode.GestureOnly)
                {
                    // Show gesture feedback in the GestureOnly mode
                    GestureResultAdorner.ShowMessage(results[0].ApplicationGesture.ToString(), e.Strokes.GetBounds().TopLeft);
                }
                else
                {
                    // In InkAndGesture mode, and if not using highlighter, if a ScratchOut
                    // gesture is detected then remove the underlying strokes
                    if (results[0].ApplicationGesture == ApplicationGesture.ScratchOut
                        && !e.Strokes[0].DrawingAttributes.IsHighlighter)
                    {
                        StrokeCollection strokesToRemove = ActiveLayer.Strokes.HitTest(e.Strokes.GetBounds(), 10);
                        if (strokesToRemove.Count != 0)
                        {
                            ActiveLayer.Strokes.Remove(strokesToRemove);
                        }
                    }
                    else
                    {
                        // Otherwise cancel the gesture.
                        e.Cancel = true;
                    }
                }
            }
            else
            {
                // Cancel the gesture.
                e.Cancel = true;
            }
        }
        */
        private bool bLoaded;
        public new bool IsLoaded
        {
            get
            {
                return bLoaded;
            }
        }

        IUriToUriAbsoluteImageConverter uriToUriAbsoluteImageConverter;
        private void ScreenEditorControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (bLoaded)
            {
                CheckCurrentLanguage();
                return;
            }

            bLoaded = true;

            var adornerLayer = AdornerLayer.GetAdornerLayer(MainSurface);
            if (adornerLayer != null)
            {
                _gridAdorner = new GridAdorner(MainSurface, magicSnapNumber);
                _gridAdorner.Visibility = Visibility.Hidden;
                adornerLayer.Add(_gridAdorner);
            }

            var tabHeader = screenChildDocumentGroup.GetVisualChildrenOfType<DockingSplitLayoutPanel>().FirstOrDefault(x => x.Name == "HeadersPanel");
            if (tabHeader != null)
                tabHeader.Visibility = Visibility.Collapsed;

            var tabContainerBorder = screenChildDocumentGroup.GetVisualChildrenOfType<DXBorder>().FirstOrDefault(x => x.Name == "TabbedLine");
            if (tabContainerBorder != null)
                tabContainerBorder.Visibility = Visibility.Collapsed;

            MainSurface.BeginInit();
            ThemeHelper.SetTheme(MainSurface, Document.Theme);
            uriToUriAbsoluteImageConverter = Resources["uriToUriAbsoluteImageConverter"] as IUriToUriAbsoluteImageConverter;
            string dest = System.IO.Path.GetDirectoryName(Document.FilePath) + "\\";
            var absolutePath = new Uri(dest, UriKind.RelativeOrAbsolute);
            var absolutePath2 = Document.GetSpecialFolder(SpecialFolders.Images);
            uriToUriAbsoluteImageConverter.FileSystemProviderBase = Document.fileSystemProviderBase;
            uriToUriAbsoluteImageConverter.AbsolutePath = absolutePath;
            uriToUriAbsoluteImageConverter.AbsolutePath2 = absolutePath2;

            try
            {
                Surface = Document.GetCurrentXamlDocument();
            }
            catch (Exception ex)
            {
                GestureResultAdorner.ShowMessage(ex.Message, Mouse.GetPosition(ActiveLayer));
            }

            //MainSurface.ManipulationStarting += Window_ManipulationStarting;
            //MainSurface.ManipulationDelta += Window_ManipulationDelta;
            //MainSurface.ManipulationInertiaStarting += Window_InertiaStarting;
            //MainSurface.ManipulationCompleted += Window_ManipulationCompleted;

            Document.LoadResources(this);
            Document.RefreshEntityStyleBinding(MainSurface);
            Document.RemoveDeadEntities(MainSurface);

            Document.NeedsSave = false;

            Document.UpdateRepositoryItems(this, MainSurface, bSync: true);

            // Document.SubscribeAllChangeXamlWriterProperties(MainSurface);
            // Document.Mediator.NotifyColleagues<ScreenEditorView>("ScreenLoaded", this);

            UpdateHMIControls();

            SetBaseUri(MainSurface);

            if (cvsBackground.Background is VisualBrush &&
                                                  (cvsBackground.Background as VisualBrush).Visual is MediaElement)
            {
                var oldme = (cvsBackground.Background as VisualBrush).Visual as MediaElement;
                var me = new MediaElement();
                me.Source = oldme.Source;
                var list = new List<MediaElement>() { me };
                Document.SetBaseUri(list, absolutePath, absolutePath2);
                Utilities.ResourceDictionaryExtensions.SetMediaElementAutoStart(list);

                cvsBackground.Background = new VisualBrush(me);
            }
            else if (cvsBackground.Background is VisualBrush &&
                                                  (cvsBackground.Background as VisualBrush).Visual is Image)
            {
                var image = (cvsBackground.Background as VisualBrush).Visual as Image;
                var map = image.GetAllValueEntries();
                var listExpression = (from c in map.Keys
                                      where (c.Value as BindingExpression).ParentBinding.Converter is IUriToUriAbsoluteImageConverter
                                      select (c.Value as BindingExpression)).ToList();
                listExpression.ForEach(binding =>
                {
                    (binding.ParentBinding.Converter as IUriToUriAbsoluteImageConverter).FileSystemProviderBase = Document.fileSystemProviderBase;
                    (binding.ParentBinding.Converter as IUriToUriAbsoluteImageConverter).AbsolutePath = absolutePath;
                    (binding.ParentBinding.Converter as IUriToUriAbsoluteImageConverter).AbsolutePath2 = absolutePath2;
                    binding.UpdateTarget();
                });
            }

            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    RestoreGridSettings();
                    if (Document != null)
                    {
                        Document.IsShowAdornerExpander = ApplicationPropertiesHelper.GetProperty<bool>("ShowAdornerExpander");
                        Document.NeedsSave = false;
                    }
                });

            //ThemeHelper.SetTheme(this, Document.Theme);
            MainSurface.EndInit();

            Action actionDelaySizeChange = () =>
            {
                UpdateScrollerSize();
            };

            SizeChanged += (ob, ev) =>
            {
                if (SizeChangedInvoker == null)
                    SizeChangedInvoker = new DelayedSingleActionInvoker(actionDelaySizeChange, TimeSpan.FromMilliseconds(50));

                if (SizeChangedInvoker != null)
                    SizeChangedInvoker.BeginInvoke();
            };

            actionDelaySizeChange();

            if (StringEditor == null)
                StringEditor = EditorComponent.StringEditor;

            if (StringEditor != null)
            {
                StringEditor.LocalesChanged += OnLocalesChanged;
                OnLocalesChanged(null, null);
            }
        }

        void RestoreDockSettings()
        {
            using (new WaitCursor())
            {
                var storage = GetStorage();

                var name = Assembly.GetExecutingAssembly().GetName().Name;
                using (var Mutex = new Mutex(false, name))
                {
                    Mutex.WaitOne();

                    try
                    {
                        if (storage.FileExists(dockingFileName))
                        {
                            using (var fileStream = storage.OpenFile(dockingFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            {
                                dockingManagerScreenEditor.RestoreLayoutFromStream(fileStream);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        EditorComponent.UIInterface.ShowError(Properties.Resources.FailedToLoadDockState);
                    }
                    finally
                    {
                        Mutex.ReleaseMutex();
                    }
                }
            }
        }

        void SaveDockSettings()
        {
            using (new WaitCursor())
            {
                var storage = GetStorage();

                var name = Assembly.GetExecutingAssembly().GetName().Name;
                using (var Mutex = new Mutex(false, name))
                {
                    try
                    {
                        Mutex.WaitOne();
                    }
                    catch (AbandonedMutexException ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }

                    try
                    {
                        if (storage.FileExists(dockingFileName))
                            storage.DeleteFile(dockingFileName);
                        using (var fileStream = storage.OpenFile(dockingFileName, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite))
                        {
                            dockingManagerScreenEditor.SaveLayoutToStream(fileStream);
                        }
                    }
                    catch (Exception ex)
                    {
                        EditorComponent.UIInterface.ShowError(Properties.Resources.FailedToSaveDockState);
                    }
                    finally
                    {
                        Mutex.ReleaseMutex();
                    }
                }
            }
        }

        static IsolatedStorageFile GetStorage()
        {
            try
            {
                return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            }
            catch
            { }

            return null;
        }

        void OnLocalesChanged(object sender, EventArgs e)
        {
            if (StringEditor == null)
                return;

            var cultures = StringEditor.GetListAvailableCultures(Document);
            if (cultures != null)
            {                
                ActiveLanguages = new ObservableCollection<string>(new string[] { Properties.Resources.None }.Concat(cultures.ToList()));
                CheckCurrentLanguage();
            }
        }

        void CheckCurrentLanguage()
        {
            if (Document != null)
            {
                var designLang = (string)ApplicationPropertiesHelper.GetProperty("DesignLanguage");
                if (designLang != null && ActiveLanguages.Contains(designLang))
                {
                    langCombo.SelectedItem = designLang;
                    if (ActiveLanguages.IndexOf(designLang) != 0)
                        lastCulture = designLang;
                }
                Document.bIsUntranslatedMode = langCombo.SelectedIndex == 0;
            }
        }

        void ComboChangeLanguage(object sender, SelectionChangedEventArgs args)
        {
            if (!bLoaded)
                return;

            var cmb = (ComboBox)sender;
            string newCulture = (string)cmb.SelectedItem;
            if (newCulture == null)
                newCulture = Properties.Resources.None;
            ApplicationPropertiesHelper.SetProperty("DesignLanguage", newCulture);
            Document.bIsUntranslatedMode = cmb.SelectedIndex == 0;
            ChangeLanguage(newCulture, MainControl);
        }

        void ChangeLanguage(String culture, FrameworkElement mainControl)
        {
            if (StringEditor == null || Document == null || (lastCulture == null && culture == null))
                return;
            if (!Document.bIsUntranslatedMode || lastCulture == null)
                lastCulture = culture;
            using (var cursor = new WaitCursor())
            {
                var map = StringEditor.GetListStringForCulture(Document, lastCulture);
                if (map == null || map.Count == 0)
                    return;

                RestoreUntranslated(mainControl);
                if (!Document.bIsUntranslatedMode)
                {
                    var bSub = Document.IsSubscribePropertyChangeXamlWriterProperties(mainControl);
                    if (bSub)
                        Document.UnsubscribePropertyChangeXamlWriterProperties(mainControl);
                    Document.ChangeLanguage(lastCulture, mainControl, map, true);
                    if (bSub)
                        Document.SubscribePropertyChangeXamlWriterProperties(mainControl, GetSelectedEntityName(mainControl));
                }

                StringEditor.SetActiveCulture(Document, culture, true);
            }
        }

        public void RestoreUntranslated(FrameworkElement mainControl, bool bSubscribeEntity = true)
        {
            if (mainControl == null || Document == null)
                return;
            using (var cursor = new WaitCursor())
            {
                var listControlCulture = Document.GetDynControlsList(mainControl);
                if (listControlCulture.Count > 0)
                    DependencyObjectExtensions.CleanChildrenOfTypeCache();
                listControlCulture.ForEach(_control =>
                {
                    bool bUnsubscribe = false;
                    var fe = _control as FrameworkElement;
                    if (fe != null && !(fe is UserControl) && Utilities.WPF.XmlHelper.IsProblematicXamlWriter(fe))
                    {
                        var name = GetSelectedEntityName(fe);
                        if (bSubscribeEntity && !String.IsNullOrEmpty(name) && !Document.IsSubscribePropertyChangeXamlWriterProperties(fe))
                        {
                            Document.SubscribePropertyChangeXamlWriterProperties(fe, name);
                            bUnsubscribe = true;
                        }
                    }
                    Document.TranslateElement(_control, new Dictionary<string, string>(), true);
                    if (bUnsubscribe)
                        Document.UnsubscribePropertyChangeXamlWriterProperties(fe);
                });
            }
        }

        void SetBaseUri(FrameworkElement element)
        {
            string dest = System.IO.Path.GetDirectoryName(Document.FilePath) + "\\";
            var absolutePath = new Uri(dest, UriKind.RelativeOrAbsolute);
            var absolutePath2 = Document.GetSpecialFolder(SpecialFolders.Images);

            var listPanels = element.GetChildrenOfType<Panel>().ToList();
            if (element is Panel)
                listPanels.Add(element as Panel);
            var listPanelsWithMediaBrush = (from c in listPanels
                                            where c.Background is VisualBrush &&
                                                  (c.Background as VisualBrush).Visual is MediaElement
                                            select ((c.Background as VisualBrush).Visual as MediaElement)).ToList();
            Document.SetBaseUri(listPanelsWithMediaBrush, absolutePath, absolutePath2);
            Utilities.ResourceDictionaryExtensions.SetMediaElementAutoStart(listPanelsWithMediaBrush);

            var listPanelsWithImageBrush = (from c in listPanels
                                            where c.Background is VisualBrush &&
                                                  (c.Background as VisualBrush).Visual is Image
                                            select ((c.Background as VisualBrush).Visual as Image)).ToList();
            listPanelsWithImageBrush.ForEach(image =>
            {
                var map = image.GetAllValueEntries();
                var list = (from c in map.Keys
                            where (c.Value as BindingExpression).ParentBinding.Converter is IUriToUriAbsoluteImageConverter
                            select (c.Value as BindingExpression)).ToList();
                list.ForEach(binding =>
                {
                    (binding.ParentBinding.Converter as IUriToUriAbsoluteImageConverter).FileSystemProviderBase = Document.fileSystemProviderBase;
                    (binding.ParentBinding.Converter as IUriToUriAbsoluteImageConverter).AbsolutePath = absolutePath;
                    (binding.ParentBinding.Converter as IUriToUriAbsoluteImageConverter).AbsolutePath2 = absolutePath2;
                    binding.UpdateTarget();
                });
            });

            var listControls = element.GetChildrenOfType<Control>().ToList();
            if (element is Control)
                listControls.Add(element as Control);
            var listControlsWithMediaBrush = (from c in listControls
                                              where c.Background is VisualBrush &&
                                                    (c.Background as VisualBrush).Visual is MediaElement
                                              select ((c.Background as VisualBrush).Visual as MediaElement)).ToList();
            Document.SetBaseUri(listControlsWithMediaBrush, absolutePath, absolutePath2);
            Utilities.ResourceDictionaryExtensions.SetMediaElementAutoStart(listControlsWithMediaBrush);

            var listControlsWithImageBrush = (from c in listControls
                                              where c.Background is VisualBrush &&
                                                    (c.Background as VisualBrush).Visual is Image
                                              select ((c.Background as VisualBrush).Visual as Image)).ToList();
            listControlsWithImageBrush.ForEach(image =>
            {
                var map = image.GetAllValueEntries();
                var list = (from c in map.Keys
                            where (c.Value as BindingExpression).ParentBinding.Converter is IUriToUriAbsoluteImageConverter
                            select (c.Value as BindingExpression)).ToList();
                list.ForEach(binding =>
                {
                    (binding.ParentBinding.Converter as IUriToUriAbsoluteImageConverter).FileSystemProviderBase = Document.fileSystemProviderBase;
                    (binding.ParentBinding.Converter as IUriToUriAbsoluteImageConverter).AbsolutePath = absolutePath;
                    (binding.ParentBinding.Converter as IUriToUriAbsoluteImageConverter).AbsolutePath2 = absolutePath2;
                    binding.UpdateTarget();
                });
            });

            var listBorders = element.GetChildrenOfType<Border>().ToList();
            if (element is Border)
                listBorders.Add(element as Border);
            var listBordersWithMediaBrush = (from c in listBorders
                                             where c.Background is VisualBrush &&
                                                  (c.Background as VisualBrush).Visual is MediaElement
                                            select ((c.Background as VisualBrush).Visual as MediaElement)).ToList();
            Document.SetBaseUri(listBordersWithMediaBrush, absolutePath, absolutePath2);
            Utilities.ResourceDictionaryExtensions.SetMediaElementAutoStart(listBordersWithMediaBrush);

            var listBordersWithImageBrush = (from c in listBorders
                                             where c.Background is VisualBrush &&
                                                  (c.Background as VisualBrush).Visual is Image
                                            select ((c.Background as VisualBrush).Visual as Image)).ToList();
            listBordersWithImageBrush.ForEach(image =>
            {
                var map = image.GetAllValueEntries();
                var list = (from c in map.Keys
                            where (c.Value as BindingExpression).ParentBinding.Converter is IUriToUriAbsoluteImageConverter
                            select (c.Value as BindingExpression)).ToList();
                list.ForEach(binding =>
                {
                    (binding.ParentBinding.Converter as IUriToUriAbsoluteImageConverter).FileSystemProviderBase = Document.fileSystemProviderBase;
                    (binding.ParentBinding.Converter as IUriToUriAbsoluteImageConverter).AbsolutePath = absolutePath;
                    (binding.ParentBinding.Converter as IUriToUriAbsoluteImageConverter).AbsolutePath2 = absolutePath2;
                    binding.UpdateTarget();
                });
            });

            var listShapes = element.GetChildrenOfType<Shape>().ToList();
            if (element is Shape)
                listShapes.Add(element as Shape);
            var listShapesWithMediaBrush = (from c in listShapes
                                            where c.Fill is VisualBrush &&
                                                  (c.Fill as VisualBrush).Visual is MediaElement
                                            select ((c.Fill as VisualBrush).Visual as MediaElement)).ToList();
            Document.SetBaseUri(listShapesWithMediaBrush, absolutePath, absolutePath2);
            Utilities.ResourceDictionaryExtensions.SetMediaElementAutoStart(listShapesWithMediaBrush);

            var listShapesWithImageBrush = (from c in listShapes
                                            where c.Fill is VisualBrush &&
                                                  (c.Fill as VisualBrush).Visual is Image
                                            select ((c.Fill as VisualBrush).Visual as Image)).ToList();
            listShapesWithImageBrush.ForEach(image =>
            {
                var map = image.GetAllValueEntries();
                var list = (from c in map.Keys
                            where (c.Value as BindingExpression).ParentBinding.Converter is IUriToUriAbsoluteImageConverter
                            select (c.Value as BindingExpression)).ToList();
                list.ForEach(binding =>
                {
                    (binding.ParentBinding.Converter as IUriToUriAbsoluteImageConverter).FileSystemProviderBase = Document.fileSystemProviderBase;
                    (binding.ParentBinding.Converter as IUriToUriAbsoluteImageConverter).AbsolutePath = absolutePath;
                    (binding.ParentBinding.Converter as IUriToUriAbsoluteImageConverter).AbsolutePath2 = absolutePath2;
                    binding.UpdateTarget();
                });
            });

            var listImages = element.GetChildrenOfType<Image>().ToList();
            if (element is Shape)
                listImages.Add(element as Image);
            var listImagesToSet = new List<BitmapImage>();
            foreach (var c in listImages)
            {
                try
                {
                    if (c.Source != null && c.Source is BitmapImage &&
                    (c.Source as BitmapImage).UriSource != null &&
                    !(c.Source as BitmapImage).UriSource.IsAbsoluteUri &&
                    (c.Source as BitmapImage).UriSource.IsFile &&
                    !File.Exists((c.Source as BitmapImage).UriSource.GetPathString()))
                    {
                        listImagesToSet.Add(c.Source as BitmapImage);
                    }
                }
                catch { }
            }
            //var listImagesToSet = (from c in listImages
            //                       where
            //                           c.Source != null && c.Source is BitmapImage &&
            //                           (c.Source as BitmapImage).UriSource != null &&
            //                           !(c.Source as BitmapImage).UriSource.IsAbsoluteUri && 
            //                           (c.Source as BitmapImage).UriSource.IsFile &&
            //                           !File.Exists((c.Source as BitmapImage).UriSource.GetPathString())
            //                       select c.Source as BitmapImage).ToList();
            Document.SetBaseUri(listImagesToSet);
            listImagesToSet.ForEach(bmp =>
            {
                var name = System.IO.Path.GetFileName(bmp.UriSource.GetPathString());
                var uriFound = new Uri(name, UriKind.RelativeOrAbsolute);
                bmp.UriSource = uriFound;
            });
        }

        private void CleanSurface()
        {
            foreach (var uie in MainSurface.Children)
            {
                if (uie is FrameworkElement)
                    Utilities.WPF.DependencyObjectExtensions.UnregisterName(MainSurface, uie as FrameworkElement);
            }

            MainSurface.Children.Clear();
        }

        bool bUnloaded;
        private void ScreenEditorControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (bLoaded && !bUnloaded)
            {
                Document.UnloadResources(this);
                // Document.Mediator.NotifyColleagues<ScreenEditorView>("ScreenUnloaded", this);

                bUnloaded = true;
            }
        }

        #region VisualTree

        private object yourDummyNode = new Object(); // this is needed for the delay load tree filling
        bool bTreeSelectedOnLoaded;
        private void FillVisualTree(TreeListNode itemRoot, bool expandAll = false)
        {
            //if (btnExplorer.IsChecked == null || btnExplorer.IsChecked.Value == false)
            //    return;

            var listSelected = MainSurface_GetCurrentSelection();

            using (var cursor = new WaitCursor())
            {
                objectBrowserTree.BeginDataUpdate();
                objectBrowserTree.View.Nodes.Clear();

                foreach (UIElement uie in MainSurface.Children)
                {
                    bool bEditable = true;
                    var element = uie as UIElement;
                    var name = element.Uid as String;
                    if (String.IsNullOrEmpty(name))
                    {
                        bEditable = false;
                        var fe = uie as FrameworkElement;
                        if (fe != null && !String.IsNullOrEmpty(fe.Name))
                        {
                            name = fe.Name;
                            bEditable = true;
                        }
                    }
                    String title = !String.IsNullOrEmpty(name) ? name : uie.DependencyObjectType.Name;

                    ScreenEntity entity = null;
                    if (Document.MapScreenEntities.ContainsKey(name))
                        entity = Document.MapScreenEntities[name];

                    bool containsCode = false;
                    bool sourceSymboLinked = false;
                    bool constainsDynamic = false;
                    string typeIcon = "SMEntities";
                    if (entity != null)
                    {
                        containsCode = entity.ContainsCode();
                        sourceSymboLinked = entity.SourceSymbolLinked;
                        constainsDynamic = entity.ContainsDynamic();
                        typeIcon = constainsDynamic || containsCode ? "SMDynamicEntity" : sourceSymboLinked ? "SMLink" : "SMEmbedded";
                    }
                    var obti = new ObjectBrowseTreeItem(title, bEditable)
                    {
                        DataContext = uie,
                        Title = title,
                        ZLayer = MainSurface.Children.IndexOf(uie) + 1,
                        VisualCount = uie.GetVisualChildrenOfType<DependencyObject>().ToList().Count,
                        LogicCount = uie.GetChildrenOfType<DependencyObject>().ToList().Count,
                        ContainsCode = containsCode,
                        IsDynamic = constainsDynamic
                    };

                    var item = objectBrowserTree.AddNode(new ObjectBrowserTreeItemControl(obti, GetDynamicImageSource(typeIcon)), null, uie);

                    foreach (object sub in LogicalTreeHelper.GetChildren(uie))
                    {
                        if (!(sub is UIElement))
                            continue;

                        objectBrowserTree.AddNode(null, item, TreeListControlHelper.DummyNode);
                        break;
                    }

                    if (listSelected.Contains(uie))
                    {
                        Dispatcher.BeginInvokeAsynchronouslyInBackground(() => 
                            {
                                bTreeSelectedOnLoaded = true;
                                try
                                {
                                    objectBrowserTree.SelectNode(item);
                                    if (expandAll)
                                        item.ExpandAll();
                                }
                                catch
                                {

                                }
                                finally
                                {
                                    bTreeSelectedOnLoaded = false;
                                }
                            });
                    }
                }
                objectBrowserTree.EndDataUpdate();
            }
        }

        private void FillVisualTree_Objects()
        {
            var objectBrowserTree = ObjectsPanelTreeContainer.Content as TreeListControl;
            if (objectBrowserTree == null)
                return;

            objectBrowserTree.View.Nodes.Clear();
            var itemRoot = objectBrowserTree.AddNode(new ObjectBrowserTreeItemControl(new ObjectBrowseTreeItem(Document.Title, false) { Title = Document.Title }, null), Tag as TreeListNode, Document);

            FillVisualTree(itemRoot, true);
        }

        private void FillVisualTree_References(bool bRenameReferences)
        {
            using (var cursor = new WaitCursor())
            {
                referenceListTree.BeginDataUpdate();
                referenceListTree.View.Nodes.Clear();
                var map = Document.GetDynamicMapForElementAndChilds();
                var list = new List<OPCUAEntityReference>();
                foreach (var el in map.Keys)
                {
                    Parallel.ForEach(map[el], item => { item.Name = el; });
                    list.AddRange(map[el]);
                }

                if (bRenameReferences && ScreenEditorComponent.UFUAEditor != null)
                {
                    var nodelist = (from c in list
                                    where c.ResolvedNodeId != null
                                    select c.ResolvedNodeId.ToString()).Distinct().ToList();
                    var mapNodes = ScreenEditorComponent.UFUAEditor.GetListNodeNames(Document, nodelist);
                    if (mapNodes == null)
                        return;
                    bool bDirty = false;
                    bool clearCache = true;
                    var mapTagChanged = new Dictionary<String, String>();
                    foreach (var node in mapNodes.Keys)
                    {
                        var found = (from c in list
                                        where c.ResolvedNodeId != null &&
                                        c.ResolvedNodeId.ToString() == node
                                        select c).ToList();
                        found.ForEach(item =>
                        {
                            string tagFound;
                            ScreenEditorComponent.UFUAEditor.CheckVariable(Document, mapNodes[node], item.EndpointUrl, null, out tagFound, true, clearCache);
                            clearCache = false;
                            OPCUAEntityReference tag = tagFound?.FromXml<OPCUAEntityReference>();
                            if (tag != null)
                            {
                                var preChanged = item.ToXml();
                                var newName = tag.HumanReadable;
                                if (item.HumanReadable != newName)
                                {
                                    item.HumanReadable = newName;
                                    item.ReadablePath = tag.ReadablePath;
                                    item.RelativePath = tag.RelativePath;
                                    //bDirty = true;
                                }
                                var postChanged = item.ToXml();

                                if (!postChanged.Equals(preChanged) && !mapTagChanged.ContainsKey(preChanged))
                                    mapTagChanged.Add(preChanged, postChanged);
                            }
                        });
                    }
                    if (mapTagChanged.Count > 0)
                    {
                        map.Keys.ToList().ForEach(key =>
                        {
                            Document.MapScreenEntities[key].ReplaceEntityReferences(mapTagChanged, key);
                        });
                        MainSurface_SetModified(true);
                    }
                }
                referenceListTree.ItemsSource = null;
                referenceListTree.ItemsSource = list;
                referenceListTree.EndDataUpdate();
                referenceListTree.View.BestFitColumns();
            }
        }

        private void Expander_Selected(object sender, RoutedEventArgs e)
        {
            RestoreSelected3DModels();

            var item = sender as TreeListNode;
            if (item != null && item.Tag != null && item.Tag is UIElement)
            {
                bool bEditable = true;
                var element = item.Tag as UIElement;
                var name = element.Uid as String;
                if (String.IsNullOrEmpty(name))
                {
                    bEditable = false;
                    var fe = element as FrameworkElement;
                    if (fe != null && !String.IsNullOrEmpty(fe.Name))
                        bEditable = true;
                }

                if (bEditable)
                    SelectElement(element);
            }
            else if (item != null && item.Tag != null && item.Tag is Model3D)
            {
                CreateSelectionBrush3d();

                var model = item.Tag as Model3D;

                var parent = (item.Content as ObjectBrowserTreeItemControl).ParentTag;
                var toSelect = parent;
                while(!MainSurface.Children.Contains(toSelect))
                {
                    var control = LogicalTreeHelper.GetParent(toSelect) as FrameworkElement;
                    if (control == null)
                        break;
                    toSelect = control;
                }
                SelectElement(toSelect);

                if (model is GeometryModel3D)
                {
                    var geometryModel3D = model as GeometryModel3D;
                    mapActive3dModels.Add(geometryModel3D, geometryModel3D.Material);
                    mapBackActive3dModels.Add(geometryModel3D, geometryModel3D.BackMaterial);
                    listActive3dModels.Add(geometryModel3D);
                    geometryModel3D.Material = new DiffuseMaterial()
                    {
                        Brush = currentBrush3D
                    };
                    geometryModel3D.BackMaterial = new DiffuseMaterial()
                    {
                        Brush = currentBrush3D
                    };
                    model.Scale3D(1.5, 500, false, true, new SineEase() { EasingMode = EasingMode.EaseInOut });
                }
                else if (model is Model3DGroup)
                {
                    var model3DGroup = model as Model3DGroup;
                    listActive3dModels.Add(model3DGroup);
                    model3DGroup.Scale3D(1.5, 500, false, true, new SineEase() { EasingMode = EasingMode.EaseInOut });

                    var geometries = Utilities.WPF.DependencyObjectExtensions.GetAllGeometries(model3DGroup);
                    foreach (var geometry in geometries)
                    {
                        mapActive3dModels.Add(geometry, geometry.Material);
                        mapBackActive3dModels.Add(geometry, geometry.BackMaterial);
                        // listActive3dModels.Add(geometry);
                        geometry.Material = new DiffuseMaterial()
                        {
                            Brush = currentBrush3D
                        };
                        geometry.BackMaterial = new DiffuseMaterial()
                        {
                            Brush = currentBrush3D
                        };
                    }
                }
            }
        }

        public void SelectElement(String name)
        {
            UIElement fe = null;
            var names = name.Split(new string[] { ScreenDocument.innerEntityNameFormat }, StringSplitOptions.None);
            if (names.Length == 1)
                fe = MainSurface.FindName(names[0]) as UIElement;
            else if (Document.MapScreenEntities.ContainsKey(name))
                fe = Document.MapScreenEntities[name].Element;
            SelectElement(fe);
        }

        public void SelectElement(UIElement element)
        {
            if (bTreeSelectedOnLoaded)
                return;

            if (element == null)
            {
                MainSurface_CleanCurrentSelectedion();
                return;
            }

            var selected = MainSurface_GetCurrentSelection();
            if (selected.Count == 1 && selected[0] == element)
                return;

            var listItemToSelect = new List<UIElement>();

            listItemToSelect.Add(element);
            element.Scale(1.2, 250, false, true, new BackEase() { EasingMode = EasingMode.EaseOut }, false, (o, i) =>
            {
                var readonlyList = new ReadOnlyCollection<UIElement>(listItemToSelect);
                MainSurface_SetCurrentSelection(readonlyList, true);
            });
        }

        void Add3DModelTreeItem(TreeListControl tree, TreeListNode item, Model3D sub, FrameworkElement parent)
        {
            var name = Utilities.WPF.DependencyObjectExtensions.RegisterModel3D(MainSurface, parent, sub, false);
            if (String.IsNullOrEmpty(name))
                name = Properties.Resources.UnnamedModel3DName;

            var obti = new ObjectBrowseTreeItem(name, false/*, listModelVisibility, mapModelBrushOpacity*/) 
            { 
                DataContext = sub,
                Title = name
            };

            var newitem = tree.AddNode(new ObjectBrowserTreeItemControl(obti, Document.IsDynamicEntity(parent) ? GetDynamicImageSource("SMDynamicEntity") : GetDynamicImageSource("SMEntities"), parent), item, sub);
            if (sub is Model3DGroup)
            {
                tree.AddNode(null, newitem, TreeListControlHelper.DummyNode);
                // newitem.Expanded += Expander_Expanded;
            }
            //newitem.Selected += Expander_Selected;
        }

        static readonly String parentTag = "parentTag";
        private void Expander_Expanded(TreeListControl tree, TreeListNode item)
        {
            if (item == null || tree == null || item.Tag == null || (!(item.Tag is UIElement) && !(item.Tag is Model3DGroup)))
                return;

            using (var cursor = new WaitCursor())
            {
                if (item.Tag is Model3DGroup)
                {
                    var parent = (item.Content as ObjectBrowserTreeItemControl).ParentTag;

                    var _3dGroup = item.Tag as Model3DGroup;
                    foreach (Model3D sub in _3dGroup.Children)
                    {
                        if (sub is GeometryModel3D || sub is Model3DGroup)
                            Add3DModelTreeItem(tree, item, sub, parent);
                    }
                    return;
                }

                UIElement uie = item.Tag as UIElement;

                try
                {
                    foreach (object child in LogicalTreeHelper.GetChildren(uie))
                    // for (int i = 0; i < LogicalTreeHelper.GetChildrenCount(obj); i++)
                    {
                        if (!(child is UIElement)) continue;

                        DependencyObject dpendencychild = child as DependencyObject;

                        try
                        {
                            bool bEditable = true;
                            var element = child as UIElement;
                            var name = element.Uid as String;
                            if (String.IsNullOrEmpty(name))
                            {
                                bEditable = false;
                                var fe = child as FrameworkElement;
                                if (fe != null && !String.IsNullOrEmpty(fe.Name))
                                {
                                    name = fe.Name;
                                    bEditable = true;
                                }
                            }

                            String title = !String.IsNullOrEmpty(name) ? name : dpendencychild.DependencyObjectType.Name;

                            ScreenEntity entity = null;
                            if (Document.MapScreenEntities.ContainsKey(name))
                                entity = Document.MapScreenEntities[name];

                            bool containsCode = false;
                            bool sourceSymboLinked = false;
                            bool constainsDynamic = false;
                            string typeIcon = "SMEntities";
                            if (entity != null)
                            {
                                containsCode = entity.ContainsCode();
                                sourceSymboLinked = entity.SourceSymbolLinked;
                                constainsDynamic = entity.ContainsDynamic();
                                typeIcon = constainsDynamic || containsCode ? "SMDynamicEntity" : sourceSymboLinked ? "SMLink" : "SMEmbedded";
                            }
                            var obti = new ObjectBrowseTreeItem(title, bEditable)
                            {
                                DataContext = dpendencychild,
                                Title = title,
                                ZLayer = MainSurface.Children.IndexOf(uie) + 1,
                                VisualCount = uie.GetVisualChildrenOfType<DependencyObject>().ToList().Count,
                                LogicCount = uie.GetChildrenOfType<DependencyObject>().ToList().Count,
                                ContainsCode = containsCode,
                                IsDynamic = constainsDynamic
                            };

                            var newitem = tree.AddNode(new ObjectBrowserTreeItemControl(obti, GetDynamicImageSource(typeIcon)), item, dpendencychild);

                            foreach (object sub in LogicalTreeHelper.GetChildren(dpendencychild))
                            {
                                if (!(sub is UIElement)) continue;
                                tree.AddNode(null, newitem, TreeListControlHelper.DummyNode);
                                break;
                            }

                            // newitem.Expanded += Expander_Expanded;

                            //if (bEditable)
                            //    newitem.Selected += Expander_Selected;


                            if (child is Viewport3D)
                            {
                                var viewport3D = child as Viewport3D;
                                if (viewport3D.Children.Count > 0 && newitem.Nodes.Count == 0)
                                    tree.AddNode(null, newitem, TreeListControlHelper.DummyNode);
                            }
                        }
                        catch { }
                    }

                    if (uie is Viewport3D)
                    {
                        var viewport3D = uie as Viewport3D;
                        foreach (var child in viewport3D.Children)
                        {
                            var modelVisual = child as ModelVisual3D;
                            if (modelVisual != null)
                            {
                                var model = modelVisual.Content;
                                Add3DModelTreeItem(tree, item, model, viewport3D);
                            }
                        }
                    }
                }
                catch { }
            }
        }

        BitmapImage GetDynamicImageSource(string source)
        {
            if (string.IsNullOrEmpty(source))
                return null;
            return ScreenManagerComponent.GetBitmapImage(source);
        }
        #endregion VisualTree

        #region Commands

        String currentObjectTypeXaml;
        ToolBoxData currentObjectTypeFile;
        Cursor currentCursor;

        void CancelCurrentSelectedInsertObject()
        {
            if (EditorComponent.ToolBox != null && EditorComponent.ToolBox.ActiveToolCode != null)
                EditorComponent.ToolBox.ActiveToolCode = null;

            currentObjectTypeXaml = null;
            currentObjectTypeFile = null;
            if (currentCursor != null)
            {
                currentCursor.Dispose();
                currentCursor = null;
            }
            MainSurface.Cursor = Cursors.Arrow;
        }
        string lastToolboxHash;
        void CreateCurrentToolCursor(ToolBoxData data)
        {
            try
            {
                // MainSurface_CleanCurrentSelectedion();

                //Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                //{
                //    Focusable = true;
                //    Focus();
                //});

                if (data.image != null)
                {
                    var border = new Border()
                    {
                        BorderThickness = new Thickness(3),
                        BorderBrush = Brushes.Gray,
                        Child = new Image { Source = data.image }
                    };
                    currentCursor = Utilities.CursorHelper.CreateCursor(border, 0, 0);
                    border.Child = null;
                }
                else
                {
                    var code = EditorComponent.ToolBox.GetCodeFromHash(data.Hash);
                    UIElement element = code.ReadUIElement();

                    var border = new Border() { BorderThickness = new Thickness(3), BorderBrush = Brushes.Gray, Child = element };
                    currentCursor = Utilities.CursorHelper.CreateCursor(border, 0, 0);
                    border.Child = null;
                }
                lastToolboxHash = data.Hash;
            }
            catch (Exception ex)
            {
                currentCursor = Cursors.Cross;
                lastToolboxHash = string.Empty;
            }

            //UIElement element = currentObjectTypeXaml.ReadUIElement();
            //var border = new Border() { BorderThickness = new Thickness(3), BorderBrush = Brushes.Gray, Child = element };
            //currentCursor = Utilities.CursorHelper.CreateCursor(border, 0, 0);
            //border.Child = null;
            MainSurface.Cursor = currentCursor;
        }

        private void OnInsertObjectType(object sender, ExecutedRoutedEventArgs e)
        {
            if (bSetZOrderInProgress)
                return;

            e.Handled = true;
            CancelCurrentSelectedInsertObject();

            currentObjectTypeFile = e.Parameter as ToolBoxData;
            currentObjectTypeXaml = EditorComponent.ToolBox.GetCodeFromHash(currentObjectTypeFile.Hash);
            EditorComponent.ToolBox.ActiveToolCode = currentObjectTypeFile;
                // var folder = System.IO.Path.GetDirectoryName(currentObjectTypeFile);
            CreateCurrentToolCursor(currentObjectTypeFile);
        }

        private void OnCreateGroup(object sender, ExecutedRoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                e.Handled = true;
                undoredo.SetStateForUndoRedo(ScreenManager.UndoRedoManager.UndoAction.TotalState);
                CreateGroup();
            }
        }

        private void OnCreateExpander(object sender, ExecutedRoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                e.Handled = true;
                undoredo.SetStateForUndoRedo(ScreenManager.UndoRedoManager.UndoAction.TotalState);
                List<UIElement> listSelected = MainSurface_GetCurrentSelection();
                if (listSelected != null && listSelected.Count > 1)
                    CreateGroup();

                ExpandDirection expand = ExpandDirection.Down;
                try
                {
                    String s = e.Parameter as String;
                    switch (s)
                    {
                        case "Up": expand = ExpandDirection.Up; break;
                        case "Right": expand = ExpandDirection.Right; break;
                        case "Left": expand = ExpandDirection.Left; break;
                    }
                }
                catch (Exception)
                {
                }

                CreateContentControl(new Expander()
                                                {
                                                    Header = Properties.Resources.ExpanderTitle,
                                                    ExpandDirection = expand,
                                                    Background = Brushes.Transparent,
                                                    IsExpanded = false,
                                                    BorderBrush = Brushes.Transparent
                                                });
            }
        }

        private void OnCreateGroupBox(object sender, ExecutedRoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                e.Handled = true;
                undoredo.SetStateForUndoRedo(ScreenManager.UndoRedoManager.UndoAction.TotalState);
                List<UIElement> listSelected = MainSurface_GetCurrentSelection();
                if (listSelected != null && listSelected.Count > 1)
                    CreateGroup();

                CreateContentControl(new GroupBox() { Header = Properties.Resources.GroupBoxTitle, Background = Brushes.Transparent });
            }
        }

        private void OnCreateBorder(object sender, ExecutedRoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                e.Handled = true;
                undoredo.SetStateForUndoRedo(ScreenManager.UndoRedoManager.UndoAction.TotalState);
                List<UIElement> listSelected = MainSurface_GetCurrentSelection();
                if (listSelected != null && listSelected.Count > 1)
                    CreateGroup();

                double borderThickness = 3;
                double cornerRadius = 10;

                try
                {
                    String s = e.Parameter as String;
                    DoubleCollection dc = new DoubleCollection();
                    dc = DoubleCollection.Parse(e.Parameter as String);
                    borderThickness = dc[0];
                    cornerRadius = dc[1];
                }
                catch (Exception)
                {
                }

                CreateDecorator(new Border()
                                    {
                                        BorderThickness = new Thickness(borderThickness),
                                        CornerRadius = new CornerRadius(cornerRadius),
                                        Background = Brushes.Transparent
                                    }
                                );
            }
        }

        private void OnCreateVerticalStackPanel(object sender, ExecutedRoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                e.Handled = true;
                undoredo.SetStateForUndoRedo(ScreenManager.UndoRedoManager.UndoAction.TotalState);
                CreatePanel(new StackPanel() { Orientation = Orientation.Vertical });
            }
        }

        private void OnCreateHorizontalStackPanel(object sender, ExecutedRoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                e.Handled = true;
                undoredo.SetStateForUndoRedo(ScreenManager.UndoRedoManager.UndoAction.TotalState);
                CreatePanel(new StackPanel() { Orientation = Orientation.Horizontal });
            }
        }

        private void OnCreateVerticalWrapPanel(object sender, ExecutedRoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                e.Handled = true;
                undoredo.SetStateForUndoRedo(ScreenManager.UndoRedoManager.UndoAction.TotalState);
                CreatePanel(new WrapPanel() { Orientation = Orientation.Vertical });
            }
        }

        private void OnCreateHorizontalWrapPanel(object sender, ExecutedRoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                e.Handled = true;
                undoredo.SetStateForUndoRedo(ScreenManager.UndoRedoManager.UndoAction.TotalState);
                CreatePanel(new WrapPanel() { Orientation = Orientation.Horizontal });
            }
        }

        private void OnCreateDockPanel(object sender, ExecutedRoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                e.Handled = true;
                undoredo.SetStateForUndoRedo(ScreenManager.UndoRedoManager.UndoAction.TotalState);
                var dockPanel = new DockPanel();
                CreatePanel(dockPanel);

                int nDockLevel = 4;
                String s = e.Parameter as String;
                if (s != null)
                    Int32.TryParse(s, out nDockLevel);

                for (int i = 0; i < dockPanel.Children.Count && i < nDockLevel; ++i)
                {
                    switch (i)
                    {
                        case 0: dockPanel.Children[i].SetValue(DockPanel.DockProperty, Dock.Top); break;
                        case 2: dockPanel.Children[i].SetValue(DockPanel.DockProperty, Dock.Bottom); break;
                        case 1: dockPanel.Children[i].SetValue(DockPanel.DockProperty, Dock.Left); break;
                        case 3: dockPanel.Children[i].SetValue(DockPanel.DockProperty, Dock.Right); break;
                        default: i = dockPanel.Children.Count; break;
                    }
                }
            }
        }

        private void OnGeneratePowerTemplate(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            if (EditorComponent.UIInterface.ShowOkCancel(Properties.Resources.NeedServersRunning, CustomDialogIcons.Question) == CustomDialogResults.Cancel)
                return;

            using (var cursor = new WaitCursor())
            {
                List<UIElement> listSelected = MainSurface_GetCurrentSelection();
                using (new ChangeScreenWatcher(this))
                {
                    listSelected.ForEach(el =>
                        {
                            var fe = el as FrameworkElement;
                            Document.CreateSmartTemplate(fe);

                            var control = new Popups.TypeDefinitionSummary(fe);
                            var list = new List<OPCUAEntityReference>();
                            var map = Document.GetMapItemsToBeResolved(fe, fe.Name, true);
                            foreach (var element in map.Keys)
                            {
                                map[element].ToList().ForEach(item => { item.Name = element; });
                                list.AddRange(map[element]);
                            }
                            control.gridDataControl.ItemsSource = list;
                            cursor.Release();
                            OnShowPopupEditor(fe, control, fe, title: Properties.Resources.DataTypeEditor, bShowOk:false, helplink:"GeneratePowerTemplate");
                            cursor.Aquire();
                        });
                }
            }
        }

        private void OnResolvePowerTemplate(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            if (EditorComponent.UIInterface.ShowOkCancel(Properties.Resources.NeedServersRunning, CustomDialogIcons.Question) == CustomDialogResults.Cancel)
                return;

            using (new WaitCursor())
            {
                List<UIElement> listSelected = MainSurface_GetCurrentSelection();
                using (new ChangeScreenWatcher(this))
                {
                    var fe = listSelected[0] as FrameworkElement;
                    var tobeResolved = Document.GetMapItemsToBeResolved(fe, fe.Name, true);
                    Dictionary<string, List<OPCUAEntityReference>> mapPreChanged = new Dictionary<string, List<OPCUAEntityReference>>();

                    tobeResolved.Keys.ToList().ForEach(key =>
                    {
                        List<OPCUAEntityReference> list = new List<OPCUAEntityReference>();
                        tobeResolved[key].ForEach(entity =>
                        {
                            var xml = entity.ToXml();
                            var en = xml.FromXml<OPCUAEntityReference>();
                            list.Add(en);
                        });
                        mapPreChanged.Add(key, list);
                    });

                    Document.ResolveRelativeItems(fe, fe.Name, MainSurface);

                    


                    //start processing and submit the start value
                    Exception exception = null;
                    bool result = false;

                    var screenentity = GetSelectedEntity(fe);
                    var mapTagChanged = new Dictionary<String, String>();

                    
                    {
                        //using (new WaitCursor())
                        {
                            try
                            {
                                var map = new Dictionary<OPCUAEntityReference, OPCUAEntityReference>();
                                tobeResolved.Values.ToList().ForEach(list =>
                                    {
                                        list.ForEach(entity =>
                                            {
                                                var xml = entity.ToXml();
                                                var en = xml.FromXml<OPCUAEntityReference>();
                                                if (!map.ContainsKey(entity))
                                                {
                                                    map.Add(entity, en);
                                                    en.Resolve(Document.SessionString);
                                                    en.SetInUse(screenentity, true);
                                                }
                                            });
                                    });

                                var reachTime = DateTime.Now + TimeSpan.FromSeconds(10);
                                while (DateTime.Now < reachTime)
                                {
                                    bool bOk = true;
                                    map.Values.ToList().ForEach(entity =>
                                    {
                                        bOk &= !Opc.Ua.NodeId.IsNull(entity.ResolvedNodeId);
                                    });
                                    if (bOk)
                                    {
                                        result = true;
                                        break;
                                    }
                                    Thread.Sleep(100);
                                }

                                Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
                                UInt16 ns = (UInt16)(n.Count + 2 - 1);
                                string oldChars = string.Format("{0}:", ns);

                                map.Values.ToList().ForEach(entity =>
                                {
                                    entity.SetInUse(screenentity, false);

                                    //entity.HumanReadable = string.Format("{0}/{1} ({2})", entity.StartingAddress, entity.RelativePath, entity.AppName);
                                    string human = entity.HumanReadable;
                                    if (entity.IsRelative)
                                    {
                                        human = $"{entity.StartingAddress.Replace(oldChars, "").Replace('&', '/')}:{entity.RelativePath.Replace(oldChars, "").Replace('&', '/')} ({entity.AppName})";
                                        entity.RelativePath = $"{entity.StartingAddress}/{entity.RelativePath}";
                                        entity.ReadablePath = entity.RelativePath;
                                    }
                                    else
                                    {
                                        entity.RelativePath = $"{entity.RelativePath}";
                                        entity.ReadablePath = $"{entity.ReadablePath}";
                                    }

                                    if (human.StartsWith("Tags/"))
                                        human = human.Remove(0, 5);

                                    entity.HumanReadable = human;
                                    entity.StartingAddress = null;
                                });

                                tobeResolved.Values.ToList().ForEach(list =>
                                {
                                    list.ForEach(entity =>
                                    {
                                        if (map.ContainsKey(entity))
                                        {
                                            entity.StartingAddress = map[entity].StartingAddress;
                                            entity.ResolvedItem = map[entity].ResolvedItem;
                                            entity.HumanReadable = map[entity].HumanReadable;
                                            entity.RelativePath = map[entity].RelativePath;
                                            entity.ReadablePath = map[entity].ReadablePath;
                                            entity.ResolvedNodeId = map[entity].ResolvedNodeId;
                                            entity.ResolvedStartingNodeId = map[entity].ResolvedStartingNodeId;
                                            entity.HasResolvedNodeId = map[entity].HasResolvedNodeId;
                                        }
                                    });
                                });

                                mapPreChanged.Keys.ToList().ForEach(key =>
                                {
                                    for (int i = 0; i < mapPreChanged[key].Count; i++)
                                    {
                                        var preChanged = mapPreChanged[key][i].ToXml();
                                        var postChanged = tobeResolved[key][i].ToXml();
                                        if (!mapTagChanged.ContainsKey(preChanged))
                                            mapTagChanged.Add(preChanged, postChanged);
                                    }
                                });

                            }
                            catch (Exception ex)
                            {
                                exception = ex;
                            }
                        }
                    }
                    {
                        RealTimeConnectionManagerViewModel.CleanDeadConnections(true);

                        if (result)
                        {
                            if (mapTagChanged.Count > 0)
                            {
                                tobeResolved.Keys.ToList().ForEach(key =>
                                {
                                    //if (Document.MapScreenEntities[key].Element == null)
                                    {
                                        var uie = Document.FindInnerControl(MainSurface, key);
                                        if (uie != null)
                                        {
                                            Document.MapScreenEntities[key].Entity = uie;
                                            Document.MapScreenEntities[key].Document = Document;
                                        }
                                    }

                                    if (Document.MapScreenEntities[key].Element is IDynamicTagAware)
                                    {
                                        var dyamicAware = Document.MapScreenEntities[key].Element as IDynamicTagAware;

                                        bool bUnsubscribe = false;
                                        if (!Document.IsSubscribePropertyChangeXamlWriterProperties(Document.MapScreenEntities[key].Element))
                                        {
                                            bUnsubscribe = true;

                                            Document.SubscribePropertyChangeXamlWriterProperties(Document.MapScreenEntities[key].Element, key);
                                        }
                                        dyamicAware.UpdateMapDynamics(mapTagChanged);
                                        if (bUnsubscribe)
                                            Document.UnsubscribePropertyChangeXamlWriterProperties(Document.MapScreenEntities[key].Element);
                                    }
                                });
                            }

                            EditorComponent.UIInterface.ShowInformation(Properties.Resources.PowerTemplateResolved);
                        }
                        else
                        {
                            /*
                            tobeResolved.Values.ToList().ForEach(list =>
                            {
                                list.ForEach(entity =>
                                {
                                    entity.StartingAddress = null;
                                    entity.ResolvedItem = false;
                                });
                            });
                            */
                            EditorComponent.UIInterface.ShowError(Properties.Resources.PowerTemplateResolvedError);
                        }
                    }
                }
            }
        }

        FrameworkElement lastSelectedResolvePowerTemplate;
        bool lastCanExecuteResolvePowerTemplate;

        
        private void CanExecuteGeneratePowerTemplate(object sender, CanExecuteRoutedEventArgs e)
        {
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();
            if (listSelected.Count == 1 && MainSurface.Children.Contains(listSelected[0]))
                e.CanExecute = !IsPowerTemplateResolve();
        }

        private void CanExecuteResolvePowerTemplate(object sender, CanExecuteRoutedEventArgs e)
        {
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();
            if (listSelected.Count == 1 && MainSurface.Children.Contains(listSelected[0]))
                e.CanExecute = IsPowerTemplateResolve();
        }

        private bool IsPowerTemplateResolve()
        {
            var ret = false;
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();
            if (listSelected.Count == 1 && MainSurface.Children.Contains(listSelected[0]))
            {
                var fe = listSelected[0] as FrameworkElement;
                if (fe == lastSelectedResolvePowerTemplate)
                    ret = lastCanExecuteResolvePowerTemplate;
                else
                {
                    var tobeResolved = Document.GetMapItemsToBeResolved(fe, fe.Name);
                    ret = (tobeResolved.Count > 0);
                    lastCanExecuteResolvePowerTemplate = ret;
                    lastSelectedResolvePowerTemplate = fe;
                }
            }
            else
                lastSelectedResolvePowerTemplate = null;

            return ret;
        }

        private void OnCreateScreenTemplate(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (Double.IsNaN(MainSurface.Width))
            {
                MainSurface.Width = MainSurface.ActualWidth;
                Document.NeedsSave = true;
            }
            if (Double.IsNaN(MainSurface.Height))
            {
                MainSurface.Height = MainSurface.ActualHeight;
                Document.NeedsSave = true;
            }

            if (Document.NeedsSave)
            {
                SaveCurrentDocument();
            //    Document.SaveToPng(MainSurface);
            }

            Document.SaveToPng(MainSurface);

            FolderBrowser folderBrowser = new FolderBrowser(Document.Parent.Title);
            string startingFolder = string.Empty;

            if (!(folderBrowser == null))
            {
                var wnd = new GeneralDialogContent(folderBrowser)
                {
                    Owner = Application.Current.MainWindow,
                    Title = " ",
                    DialogKeepContent = true,
                    HelpLink = "FolderBrwoser"
                };
                if (wnd.ShowDialog() == true)
                {
                    startingFolder = folderBrowser.FolderPath;
                    try
                    {
                        Document.CreateScreenTemplate(startingFolder);
                        EditorComponent.UIInterface.ShowInformation(Properties.Resources.ScreenTemplateCreated);
                    }
                    catch (Exception ex)
                    {
                        EditorComponent.UIInterface.ShowError(ex.ToString());
                    }
                }
            }

        }

        private void OnCreateGridPanel(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new ChangeScreenWatcher(this, true, UndoRedoManager.UndoAction.TotalState))
            {
                var grid = new Grid();
                CreatePanel(grid);

                int Col = 0;
                if (e != null)
                {
                    String s = e.Parameter as String;
                    if (s != null)
                        Int32.TryParse(s, out Col);
                }

                if (Col < 1)
                    Col = Convert.ToInt32(Math.Sqrt(grid.Children.Count));

                int Row = grid.Children.Count / Col;

                var rowDef = grid.RowDefinitions;
                rowDef.Clear();
                for (int i = 0; i < Row; ++i)
                {
                    rowDef.Add(new System.Windows.Controls.RowDefinition
                    {
                        Height = new GridLength(1, GridUnitType.Star)
                    });
                }

                var colDef = grid.ColumnDefinitions;
                colDef.Clear();
                for (int i = 0; i < Col; ++i)
                {
                    colDef.Add(new System.Windows.Controls.ColumnDefinition
                    {
                        Width = new GridLength(1, GridUnitType.Star)
                    });
                }

                int actrow = 0, actcol = 0;
                for (int i = 0; i < grid.Children.Count; ++i)
                {
                    grid.Children[i].SetValue(Grid.RowProperty, actrow);
                    grid.Children[i].SetValue(Grid.ColumnProperty, actcol);

                    if (++actrow < Row)
                        continue;

                    actrow = 0;
                    ++actcol;
                }
            }
        }

        private void OnUnCreateGroup(object sender, ExecutedRoutedEventArgs e)
        {
            using (new WaitCursor())
            {
                e.Handled = true;
                undoredo.SetStateForUndoRedo(ScreenManager.UndoRedoManager.UndoAction.TotalState);
                UnGroupItem();

                listRegroup = MainSurface_GetCurrentSelection();
            }
        }

        void CanExecuteUngroupItem(object sender, CanExecuteRoutedEventArgs e)
        {
            CanExecuteSelectedFirstLevel(sender, e);
            if (!e.CanExecute)
                return;

            foreach (KeyValuePair<UIElement, BasicAdorner> data in SelectedElementsMap)
            {
                e.CanExecute = data.Key is Viewbox || data.Key is Panel || 
                    data.Key is Expander || data.Key is GroupBox || data.Key is Border;
                break;
            }
        }

        internal bool CanClose()
        {
            CancelSetZOrder();

            if (!String.IsNullOrEmpty(regroupName))
            {
                var res = EditorComponent.UIInterface.ShowOkCancel(String.Format(Properties.Resources.LooseRegroupData,
                    Document.Title), CustomDialogIcons.Question);
                if (res == CustomDialogResults.Cancel)
                    return false;
            }

            return true;
        }

        List<UIElement> listRegroup;
        ScreenEntity entityRegroup;
        FrameworkElement regroupType;
        String regroupName;
        bool bRegrouping;
        private void Regroup(object sender, ExecutedRoutedEventArgs e)
        {
            using (new WaitCursor())
            {
                e.Handled = true;
                undoredo.SetStateForUndoRedo(ScreenManager.UndoRedoManager.UndoAction.TotalState);

                bRegrouping = true;

                try
                {
                    var currentlist = MainSurface_GetCurrentSelection();
                    listRegroup.RemoveAll(c => !MainSurface.Children.Contains(c));
                    currentlist.ForEach(c =>
                        {
                            if (!listRegroup.Contains(c))
                                listRegroup.Add(c);
                        });
                    var readonlyList = new ReadOnlyCollection<UIElement>(listRegroup);
                    MainSurface_SetCurrentSelection(readonlyList);

                    Utilities.WPF.DependencyObjectExtensions.UnregisterName(MainSurface, regroupType);

                    if (regroupType is Panel)
                        CreatePanel(regroupType as Panel);
                    else if (regroupType is Viewbox)
                        CreateGroup();
                    else if (regroupType is Decorator)
                        CreateDecorator(regroupType as Decorator);
                    else
                        CreateGroup();

                    regroupName = null;
                    listRegroup = null;
                }
                finally
                {
                    bRegrouping = false;                    
                }
            }
        }

        private void CanExecuteRegroup(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !String.IsNullOrEmpty(regroupName);
        }

        void CanExecuteSelected(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedElementsMap.Count > 0;
        }
       
        void CanExecuteSelectedFirstLevel(object sender, CanExecuteRoutedEventArgs e)
        {
            if (SelectedElementsMap.Count == 0)
                e.CanExecute = false;
            else
            {
                foreach (var element in SelectedElementsMap.Keys)
                {
                    if (!MainSurface.Children.Contains(element))
                    {
                        e.CanExecute = false;
                        return;
                    }
                }
                e.CanExecute = true;
            }
        }
        

        void CanExecuteIsControl(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            foreach (KeyValuePair<UIElement, BasicAdorner> data in SelectedElementsMap)
            {
                e.CanExecute = data.Key is Control;
                break;
            }
        }

        void CanExecuteSelectedOne(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document != null && SelectedElementsMap.Count == 1;
        }

        void CanExecuteSelectedOneOnFirstLevel(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (Document != null && SelectedElementsMap.Count == 1)
            {
                if (MainSurface.Children.Contains(SelectedElementsMap.Keys.First()))
                {
                    e.CanExecute = true;
                }
            }
        }

        void CanExecuteSelectedAny(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document != null && SelectedElementsMap.Count > 0;
        }

        void CanExecuteSelectedAnyOnFirstLevel(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (Document != null && SelectedElementsMap.Count > 0)
            {
                foreach (var element in SelectedElementsMap.Keys)
                {
                    if (!MainSurface.Children.Contains(element))
                    {
                        e.CanExecute = false;
                        return;
                    }
                }
                e.CanExecute = true;
            }
        }

        void CanExecuteSelectedMany(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document != null && SelectedElementsMap.Count > 1;
        }

        void CanExecuteSelectedManyOnFirstLevel(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (Document != null && SelectedElementsMap.Count > 1)
            {
                foreach (var element in SelectedElementsMap.Keys)
                {
                    if (!MainSurface.Children.Contains(element))
                    {
                        e.CanExecute = false;
                        return;
                    }
                }
                e.CanExecute = true;
            }
        }

        void CanAlwaysExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document != null;
        }

        private void OnRunTest(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Test();
        }

        private void OnCopyEvent(object sender, ExecutedRoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                e.Handled = true;
                List<UIElement> listSelected = MainSurface_GetCurrentSelection();
                var mapEntity = new Dictionary<String, ScreenEntity>();
                var listNames = new Dictionary<String, String>();

                PasteCounter = 1;
                if (listSelected.Count == 1)
                {
                    var dataObject = new DataObject();

                    var listAssemblies = new Queue<String>();
                    Document.ListAssemblies.ForEach(c => listAssemblies.Enqueue(c));
                    dataObject.SetData(listAssemblies.GetType(), listAssemblies.ToXml());

                    if (listSelected[0] is Panel)
                        Document.ResolveProblematicXamlOnCanvas(MainSurface, (listSelected[0] as Panel).Name);
                    else
                    {
                        listSelected[0].GetChildrenOfType<Panel>().ToList()
                            .ForEach(panel => Document.ResolveProblematicXamlOnCanvas(MainSurface, (listSelected[0] as FrameworkElement).Name));
                    }

                    RestoreUntranslated(listSelected[0] as FrameworkElement);
                    string xaml = listSelected[0].XamlWriterFormatted();

                    if (listSelected[0] is Panel)
                        Document.RestoreProblematicXamlWriter(MainSurface, (listSelected[0] as Panel).Name);
                    else
                    {
                        listSelected[0].GetChildrenOfType<Panel>().ToList()
                            .ForEach(panel => Document.RestoreProblematicXamlWriter(MainSurface, (listSelected[0] as FrameworkElement).Name));
                    }
                    Document.CleanProblematicXamlBags();

                    dataObject.SetData(DataFormats.Xaml, xaml);
                    dataObject.SetData(DataFormats.Text, xaml);
                    // Clipboard.SetText(xaml, TextDataFormat.Xaml);

                    var fe = listSelected[0] as FrameworkElement;
                    listNames.Add(fe.Name, fe.Name);

                    //var list = fe.GetChildrenOfType<FrameworkElement>();
                    //foreach (var el in list)
                    //{
                    //    if (String.IsNullOrEmpty(el.Name))
                    //        continue;
                    var list = Document.GetListInners(fe.Name);
                    foreach(var name in list)
                    {
                        var control = Document.FindInnerControl(MainSurface, name);
                        if (control != null && Document.MapScreenEntities.ContainsKey(name) &&
                            !String.IsNullOrEmpty(Document.MapScreenEntities[name].ProblematicXaml))
                        {
                            Document.SaveProblematicXamlWriterProperties(control, name);

                            if (Document.MapScreenEntities[name].MapProblematicXamlWriterProperties == null)
                                Document.MapScreenEntities[name].MapProblematicXamlWriterProperties = new ProblematicXamlWriterProperties();

                            try
                            {
                                var swtop = System.Windows.Markup.XamlWriter.Save(Canvas.GetTop(control));

                                if (Document.MapScreenEntities[name].MapProblematicXamlWriterProperties.ContainsKey("Canvas.Top"))
                                    Document.MapScreenEntities[name].MapProblematicXamlWriterProperties.Remove("Canvas.Top");
                                Document.MapScreenEntities[name].MapProblematicXamlWriterProperties.Add("Canvas.Top", swtop);
                            }
                            catch { }

                            try
                            {
                                var swleft = System.Windows.Markup.XamlWriter.Save(Canvas.GetLeft(control));

                                if (Document.MapScreenEntities[name].MapProblematicXamlWriterProperties.ContainsKey("Canvas.Left"))
                                    Document.MapScreenEntities[name].MapProblematicXamlWriterProperties.Remove("Canvas.Left");
                                Document.MapScreenEntities[name].MapProblematicXamlWriterProperties.Add("Canvas.Left", swleft);
                            }
                            catch { }

                            try
                            {
                                var swwidth = System.Windows.Markup.XamlWriter.Save(control.Width);

                                if (Document.MapScreenEntities[name].MapProblematicXamlWriterProperties.ContainsKey("Width"))
                                    Document.MapScreenEntities[name].MapProblematicXamlWriterProperties.Remove("Width");
                                Document.MapScreenEntities[name].MapProblematicXamlWriterProperties.Add("Width", swwidth);
                            }
                            catch { }

                            try
                            {
                                var swheight = System.Windows.Markup.XamlWriter.Save(control.Height);

                                if (Document.MapScreenEntities[name].MapProblematicXamlWriterProperties.ContainsKey("Height"))
                                    Document.MapScreenEntities[name].MapProblematicXamlWriterProperties.Remove("Height");
                                Document.MapScreenEntities[name].MapProblematicXamlWriterProperties.Add("Height", swheight);
                            }
                            catch { }

                        }

                        mapEntity.Add(name, Document.MapScreenEntities[name]);
                    }

                    //now lets do the image representation 
                    double width = fe.ActualWidth;
                    double height = fe.ActualHeight;
                    try
                    {
                        var bmpCopied = new RenderTargetBitmap((int)Math.Round(width), (int)Math.Round(height), 96, 96, PixelFormats.Default);
                        DrawingVisual dv = new DrawingVisual();
                        using (DrawingContext dc = dv.RenderOpen())
                        {
                            VisualBrush vb = new VisualBrush(fe);
                            dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(width, height)));
                        }
                        bmpCopied.Render(dv);
                        dataObject.SetData(DataFormats.Bitmap, bmpCopied, true);
                    }
                    catch
                    {}

                    if (Document.MapScreenEntities.ContainsKey(fe.Name) &&
                        !String.IsNullOrEmpty(Document.MapScreenEntities[fe.Name].ProblematicXaml))
                    {
                        Document.SaveProblematicXamlWriterProperties(fe, fe.Name);

                        if (Document.MapScreenEntities[fe.Name].MapProblematicXamlWriterProperties == null)
                            Document.MapScreenEntities[fe.Name].MapProblematicXamlWriterProperties = new ProblematicXamlWriterProperties();

                        try
                        {
                            var swtop = System.Windows.Markup.XamlWriter.Save(Canvas.GetTop(fe));

                            if (Document.MapScreenEntities[fe.Name].MapProblematicXamlWriterProperties.ContainsKey("Canvas.Top"))
                                Document.MapScreenEntities[fe.Name].MapProblematicXamlWriterProperties.Remove("Canvas.Top");
                            Document.MapScreenEntities[fe.Name].MapProblematicXamlWriterProperties.Add("Canvas.Top", swtop);
                        }
                        catch { }

                        try
                        {
                            var swleft = System.Windows.Markup.XamlWriter.Save(Canvas.GetLeft(fe));

                            if (Document.MapScreenEntities[fe.Name].MapProblematicXamlWriterProperties.ContainsKey("Canvas.Left"))
                                Document.MapScreenEntities[fe.Name].MapProblematicXamlWriterProperties.Remove("Canvas.Left");
                            Document.MapScreenEntities[fe.Name].MapProblematicXamlWriterProperties.Add("Canvas.Left", swleft);
                        }
                        catch { }

                        try
                        {
                            var swwidth = System.Windows.Markup.XamlWriter.Save(fe.Width);

                            if (Document.MapScreenEntities[fe.Name].MapProblematicXamlWriterProperties.ContainsKey("Width"))
                                Document.MapScreenEntities[fe.Name].MapProblematicXamlWriterProperties.Remove("Width");
                            Document.MapScreenEntities[fe.Name].MapProblematicXamlWriterProperties.Add("Width", swwidth);
                        }
                        catch { }

                        try
                        {
                            var swheight = System.Windows.Markup.XamlWriter.Save(fe.Height);

                            if (Document.MapScreenEntities[fe.Name].MapProblematicXamlWriterProperties.ContainsKey("Height"))
                                Document.MapScreenEntities[fe.Name].MapProblematicXamlWriterProperties.Remove("Height");
                            Document.MapScreenEntities[fe.Name].MapProblematicXamlWriterProperties.Add("Height", swheight);
                        }
                        catch { }

                        // Clipboard.SetDataObject(Document.MapScreenEntities[fe.Name]);
                    }
                    if (Document.MapScreenEntities.ContainsKey(fe.Name))
                        mapEntity.Add(fe.Name, Document.MapScreenEntities[fe.Name]);

                    dataObject.SetData(mapEntity.GetType(), mapEntity.ToXml());
                    dataObject.SetData(listNames.GetType(), listNames.ToXml());

                    ElementToCurrentLanguage(fe);

                    Clipboard.SetDataObject(dataObject, true);
                    e.Handled = true;
                }
                else
                {
                    var listXaml = new List<String>();
                    var dataObject = new DataObject();

                    var listAssemblies = new Queue<String>();
                    Document.ListAssemblies.ForEach(c => listAssemblies.Enqueue(c));
                    dataObject.SetData(listAssemblies.GetType(), listAssemblies.ToXml());

                    var mapOrdered = new SortedDictionary<int, UIElement>();
                    listSelected.ForEach(ch =>
                        {
                            int n = ActiveLayer.Children.IndexOf(ch);
                            mapOrdered.Add(n, ch);
                        });

                    mapOrdered.Keys.ToList().ForEach(index =>
                    {
                        var uie = mapOrdered[index];

                        if (uie is Panel)
                            Document.ResolveProblematicXamlOnCanvas(MainSurface, (uie as FrameworkElement).Name);
                        else
                        {
                            uie.GetChildrenOfType<Panel>().ToList()
                                .ForEach(panel => Document.ResolveProblematicXamlOnCanvas(MainSurface, (uie as FrameworkElement).Name));
                        }

                        RestoreUntranslated(uie as FrameworkElement);
                        string xaml = uie.XamlWriterFormatted();

                        if (uie is Panel)
                            Document.RestoreProblematicXamlWriter(MainSurface, (uie as Panel).Name);
                        else
                        {
                            uie.GetChildrenOfType<Panel>().ToList()
                                .ForEach(panel => Document.RestoreProblematicXamlWriter(MainSurface, (uie as FrameworkElement).Name));
                        }
                        Document.CleanProblematicXamlBags();

                        listXaml.Add(xaml);

                        var fe = uie as FrameworkElement;
                        listNames.Add(fe.Name, fe.Name);

                        //var list = fe.GetChildrenOfType<FrameworkElement>();
                        //foreach (var el in list)
                        //{
                        //    if (String.IsNullOrEmpty(el.Name))
                        //        continue;
                        var list = Document.GetListInners(fe.Name);
                        foreach(var name in list)
                        {
                            var control = Document.FindInnerControl(MainSurface, name);
                            if (control != null && Document.MapScreenEntities.ContainsKey(name) &&
                                !String.IsNullOrEmpty(Document.MapScreenEntities[name].ProblematicXaml))
                            {
                                Document.SaveProblematicXamlWriterProperties(control, name);

                                if (Document.MapScreenEntities[name].MapProblematicXamlWriterProperties == null)
                                    Document.MapScreenEntities[name].MapProblematicXamlWriterProperties = new ProblematicXamlWriterProperties();

                                try
                                {
                                    var swtop = System.Windows.Markup.XamlWriter.Save(Canvas.GetTop(control));

                                    if (Document.MapScreenEntities[name].MapProblematicXamlWriterProperties.ContainsKey("Canvas.Top"))
                                        Document.MapScreenEntities[name].MapProblematicXamlWriterProperties.Remove("Canvas.Top");
                                    Document.MapScreenEntities[name].MapProblematicXamlWriterProperties.Add("Canvas.Top", swtop);
                                }
                                catch { }

                                try
                                {
                                    var swleft = System.Windows.Markup.XamlWriter.Save(Canvas.GetLeft(control));

                                    if (Document.MapScreenEntities[name].MapProblematicXamlWriterProperties.ContainsKey("Canvas.Left"))
                                        Document.MapScreenEntities[name].MapProblematicXamlWriterProperties.Remove("Canvas.Left");
                                    Document.MapScreenEntities[name].MapProblematicXamlWriterProperties.Add("Canvas.Left", swleft);
                                }
                                catch { }

                                try
                                {
                                    var swwidth = System.Windows.Markup.XamlWriter.Save(control.Width);

                                    if (Document.MapScreenEntities[name].MapProblematicXamlWriterProperties.ContainsKey("Width"))
                                        Document.MapScreenEntities[name].MapProblematicXamlWriterProperties.Remove("Width");
                                    Document.MapScreenEntities[name].MapProblematicXamlWriterProperties.Add("Width", swwidth);
                                }
                                catch { }

                                try
                                {
                                    var swheight = System.Windows.Markup.XamlWriter.Save(control.Height);

                                    if (Document.MapScreenEntities[name].MapProblematicXamlWriterProperties.ContainsKey("Height"))
                                        Document.MapScreenEntities[name].MapProblematicXamlWriterProperties.Remove("Height");
                                    Document.MapScreenEntities[name].MapProblematicXamlWriterProperties.Add("Height", swheight);
                                }
                                catch { }
                            }

                            if (!mapEntity.ContainsKey(name))
                                mapEntity.Add(name, Document.MapScreenEntities[name]);
                        }

                        if (Document.MapScreenEntities.ContainsKey(fe.Name) &&
                            !String.IsNullOrEmpty(Document.MapScreenEntities[fe.Name].ProblematicXaml))
                        {
                            Document.SaveProblematicXamlWriterProperties(fe, fe.Name);

                            if (Document.MapScreenEntities[fe.Name].MapProblematicXamlWriterProperties == null)
                                Document.MapScreenEntities[fe.Name].MapProblematicXamlWriterProperties = new ProblematicXamlWriterProperties();

                            try
                            {
                                var swtop = System.Windows.Markup.XamlWriter.Save(Canvas.GetTop(fe));

                                if (Document.MapScreenEntities[fe.Name].MapProblematicXamlWriterProperties.ContainsKey("Canvas.Top"))
                                    Document.MapScreenEntities[fe.Name].MapProblematicXamlWriterProperties.Remove("Canvas.Top");
                                Document.MapScreenEntities[fe.Name].MapProblematicXamlWriterProperties.Add("Canvas.Top", swtop);
                            }
                            catch { }

                            try
                            {
                                var swleft = System.Windows.Markup.XamlWriter.Save(Canvas.GetLeft(fe));

                                if (Document.MapScreenEntities[fe.Name].MapProblematicXamlWriterProperties.ContainsKey("Canvas.Left"))
                                    Document.MapScreenEntities[fe.Name].MapProblematicXamlWriterProperties.Remove("Canvas.Left");
                                Document.MapScreenEntities[fe.Name].MapProblematicXamlWriterProperties.Add("Canvas.Left", swleft);
                            }
                            catch { }

                            try
                            {
                                var swwidth = System.Windows.Markup.XamlWriter.Save(fe.Width);

                                if (Document.MapScreenEntities[fe.Name].MapProblematicXamlWriterProperties.ContainsKey("Width"))
                                    Document.MapScreenEntities[fe.Name].MapProblematicXamlWriterProperties.Remove("Width");
                                Document.MapScreenEntities[fe.Name].MapProblematicXamlWriterProperties.Add("Width", swwidth);
                            }
                            catch { }

                            try
                            {
                                var swheight = System.Windows.Markup.XamlWriter.Save(fe.Height);

                                if (Document.MapScreenEntities[fe.Name].MapProblematicXamlWriterProperties.ContainsKey("Height"))
                                    Document.MapScreenEntities[fe.Name].MapProblematicXamlWriterProperties.Remove("Height");
                                Document.MapScreenEntities[fe.Name].MapProblematicXamlWriterProperties.Add("Height", swheight);
                            }
                            catch { }
                        }

                        if (!mapEntity.ContainsKey(fe.Name) && Document.MapScreenEntities.ContainsKey(fe.Name))
                            mapEntity.Add(fe.Name, Document.MapScreenEntities[fe.Name]);
                    });

                    dataObject.SetData(listXaml.GetType(), listXaml.ToXml());
                    dataObject.SetData(mapEntity.GetType(), mapEntity.ToXml());
                    dataObject.SetData(listNames.GetType(), listNames.ToXml());

                    /*
                    Canvas canvas = new Canvas();
                    Rect r = Utilities.WPF.DependencyObjectExtensions.CalculateBoundRect(listSelected, ActiveLayer);
                    canvas.Height = r.Height;
                    canvas.Width = r.Width;

                    listSelected.ForEach(uie =>
                    {
                        UIElement clone = Cloners.CloneUsingXaml(uie) as UIElement;
                        if (MainSurface is Canvas)
                        {
                            Canvas.SetLeft(clone, Canvas.GetLeft(uie) - r.Left);
                            Canvas.SetTop(clone, Canvas.GetTop(uie) - r.Top);
                            clone.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                            clone.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                        }
                        else
                        {
                            Canvas.SetLeft(clone, Canvas.GetLeft(uie) - r.Left);
                            Canvas.SetTop(clone, Canvas.GetTop(uie) - r.Top);
                        }
                        canvas.Children.Add(clone);
                    });

                    string xaml = canvas.XamlWriterFormatted();
                    Clipboard.SetText(xaml, TextDataFormat.Xaml);
                    */
                    mapOrdered.Keys.ToList().ForEach(index =>
                    {
                        ElementToCurrentLanguage(mapOrdered[index] as FrameworkElement);
                    });

                    Clipboard.SetDataObject(dataObject, true);
                    e.Handled = true;
                }
            }
        }

        private void OnDeleteEvent(object sender, ExecutedRoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                if (e != null)
                    e.Handled = true;
                List<UIElement> listSelected = MainSurface_GetCurrentSelection();
                using (new ChangeScreenWatcher(this, true, UndoRedoManager.UndoAction.Removed))
                {
                }
                // MainSurface_CleanCurrentSelectedion();
                var listEmpty = new List<UIElement>();
                var readonlyList = new ReadOnlyCollection<UIElement>(listEmpty);
                MainSurface_SetCurrentSelection(readonlyList);

                listSelected.ForEach(element =>
                    {
                        if (Document.ProjectType == ProjectType.WebHMI.ToString())
                            CleanWebHMIState(element);
                        var fe = element as FrameworkElement;
                        if (fe != null)
                        {
                            var list = fe.GetChildrenOfType<FrameworkElement>();
                            foreach (var el in list)
                            {
                                Document.UnsubscribePropertyChangeXamlWriterProperties(el);
                                if (String.IsNullOrEmpty(el.Name))
                                    continue;
                                // Document.RemoveEntity(el);
                                HidePopupEditor(el);
                            }
                            Document.RemoveEntity(fe);
                            Utilities.WPF.DependencyObjectExtensions.UnregisterName(MainSurface, fe, false);

                            HidePopupEditor(fe);
                        }

                        Document.UpdateResources();
                        ActiveLayer.Children.Remove(element);
                    });
            }
            // listSelected.ForEach(MainSurface.Children.Remove);
        }

        private void OnCutEvent(object sender, ExecutedRoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                e.Handled = true;
                OnCopyEvent(sender, e);
                OnDeleteEvent(sender, e);
            }
        }

        double PasteCounter = 0;

        private void OnPasteEvent(object sender, ExecutedRoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                e.Handled = true;
                using (new ChangeScreenWatcher(this, true, UndoRedoManager.UndoAction.Added))
                {
                    MainSurface_CleanCurrentSelectedion(); // case 18063

                    var dataObject = Clipboard.GetDataObject() as DataObject;
                    string xaml = dataObject.GetText(TextDataFormat.Xaml);
                    if (String.IsNullOrEmpty(xaml))
                        xaml = dataObject.GetText(TextDataFormat.Text);

                    var listXaml = new List<String>();
                    var listAssemblies = new Queue<String>();
                    var listNames = new Dictionary<String, String>();
                    var mapEntity = new Dictionary<String, ScreenEntity>();
                    IDictionary<String, String> renamed = new Dictionary<String, String>();
                    List<String> renamedMulti = new List<String>();
                    var elementsToSelect = new List<UIElement>();

                    var listEntityString = dataObject.GetData(mapEntity.GetType()) as String;
                    if (listEntityString != null)
                    {
                        mapEntity = listEntityString.FromXml<Dictionary<String, ScreenEntity>>();
                    }

                    var listNamesString = dataObject.GetData(listNames.GetType()) as String;
                    if (listNamesString != null)
                    {
                        listNames = listNamesString.FromXml<Dictionary<String, String>>();
                    }

                    var listAssembliesString = dataObject.GetData(listAssemblies.GetType()) as String;
                    if (listAssembliesString != null)
                    {
                        var listAssembliesList = listAssembliesString.FromXml<Queue<String>>().ToList();
                        Document.AddListAssembly(listAssembliesList);
                    }

                    var listXamlString = dataObject.GetData(listXaml.GetType()) as String;
                    if (listXamlString != null)
                    {
                        listXaml = listXamlString.FromXml<List<String>>();

                        var listXamlToArray = listXaml.ToArray();
                        var listNamesToArray = listNames.ToArray();
                        if (listXamlToArray.Length != listNamesToArray.Length)
                            return;

                        for (int i = 0; i < listXamlToArray.Length; ++i)
                        {
                            renamedMulti.AddRange(renamed.Values.Where(x => !renamedMulti.Contains(x)));
                            try
                            {
                                var uie = listXamlToArray[i].ReadUIElement();
                                if (uie != null)
                                {
                                    ActiveLayer.Children.Add(uie);

                                    if (Utilities.WPF.XmlHelper.CheckProblematicXamlWriter(uie, listXamlToArray[i]))
                                    {
                                        var fe = uie as FrameworkElement;
                                        fe.Name = listNamesToArray[i].Key;
                                        renamed = Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, fe, true, false);
                                        if (renamed.Count == 0)
                                            renamed.Add(fe.Name, fe.Name);
                                        else if (!renamedMulti.Contains(fe.Name))
                                            renamedMulti.Add(fe.Name);
                                        Document.SetProblematicXaml(uie as FrameworkElement, xaml);
                                        if (mapEntity != null)
                                        {
                                            var mapInners = ScreenDocument.CleanMapInnners(mapEntity, listNamesToArray[i].Key);

                                            foreach (var key in mapEntity.Keys)
                                            {
                                                FrameworkElement element = null;
                                                if (renamed.ContainsKey(key))
                                                    element = MainSurface.FindName(renamed[key]) as FrameworkElement;
                                                else
                                                {
                                                    if (renamed.Count > 0)
                                                        continue;
                                                    element = MainSurface.FindName(key) as FrameworkElement;
                                                }

                                                if (element != null)
                                                {
                                                    Document.AddInnersEntity(key, element.Name, mapInners);

                                                    Document.AddDynamicEntity(element);
                                                    Document.MapScreenEntities[element.Name].CopyAll(mapEntity[key]);
                                                    Document.MapScreenEntities[element.Name].Entity = element;

                                                    // if (mapEntity[key].SourceSymbolLinked)
                                                    {
                                                        try
                                                        {
                                                            Document.LoadRepositoryItem(this, MainSurface, element.Name, bAnimate: false);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            EditorComponent.UIInterface.ShowError(ex.Message);
                                                        }
                                                        //element.SetResourceReference(ContentControl.ContentProperty, element.Name);

                                                        //var bindingWidth = new Binding()
                                                        //{
                                                        //    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UIElement), 1),
                                                        //    Path = new PropertyPath("ActualWidth")
                                                        //};
                                                        //var bindingHeight = new Binding()
                                                        //{
                                                        //    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UIElement), 1),
                                                        //    Path = new PropertyPath("ActualHeight")
                                                        //};

                                                        //var fe2 = ((element as ContentControl).Content as FrameworkElement);
                                                        ////fe.Width = fe2.Width;
                                                        ////fe.Height = fe2.Height;
                                                        //fe2.SetBinding(FrameworkElement.WidthProperty, bindingWidth);
                                                        //fe2.SetBinding(FrameworkElement.HeightProperty, bindingHeight);
                                                    }

                                                    Document.UpdateProblematicXamlWriterProperties(element, element.Name);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var fe = uie as FrameworkElement;
                                        fe.Name = listNamesToArray[i].Key;
                                        renamed = Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, fe, true, false);
                                        if (renamed.Count == 0)
                                            renamed.Add(fe.Name, fe.Name);
                                        else if (!renamedMulti.Contains(fe.Name))
                                            renamedMulti.Add(fe.Name);
                                        if (mapEntity != null)
                                        {
                                            var mapInners = ScreenDocument.CleanMapInnners(mapEntity, listNamesToArray[i].Key);

                                            foreach (var key in mapEntity.Keys)
                                            {
                                                FrameworkElement element = null;
                                                if (renamed.ContainsKey(key))
                                                    element = MainSurface.FindName(renamed[key]) as FrameworkElement;
                                                else
                                                {
                                                    if (renamed.Count > 0)
                                                        continue;
                                                    element = MainSurface.FindName(key) as FrameworkElement;
                                                }

                                                if (element != null)
                                                {
                                                    Document.AddInnersEntity(key, element.Name, mapInners);

                                                    Document.AddDynamicEntity(element);
                                                    Document.MapScreenEntities[element.Name].CopyAll(mapEntity[key]);
                                                    Document.MapScreenEntities[element.Name].Entity = element;
                                                    // Document.UpdateProblematicXamlWriterProperties(element, element.Name);

                                                    // if (mapEntity[key].SourceSymbolLinked)
                                                    {
                                                        try
                                                        {
                                                            Document.LoadRepositoryItem(this, MainSurface, element.Name, bAnimate: false);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            EditorComponent.UIInterface.ShowError(ex.Message);
                                                        }
                                                        //element.SetResourceReference(ContentControl.ContentProperty, element.Name);

                                                        //var bindingWidth = new Binding()
                                                        //{
                                                        //    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UIElement), 1),
                                                        //    Path = new PropertyPath("ActualWidth")
                                                        //};
                                                        //var bindingHeight = new Binding()
                                                        //{
                                                        //    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UIElement), 1),
                                                        //    Path = new PropertyPath("ActualHeight")
                                                        //};

                                                        //var fe2 = ((element as ContentControl).Content as FrameworkElement);
                                                        ////fe.Width = fe2.Width;
                                                        ////fe.Height = fe2.Height;
                                                        //fe2.SetBinding(FrameworkElement.WidthProperty, bindingWidth);
                                                        //fe2.SetBinding(FrameworkElement.HeightProperty, bindingHeight);
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (uie is Panel)
                                        Document.RestoreProblematicXamlWriter(MainSurface, (uie as Panel).Name);
                                    else
                                    {
                                        uie.GetChildrenOfType<Panel>().ToList()
                                            .ForEach(panel => Document.RestoreProblematicXamlWriter(MainSurface, (uie as FrameworkElement).Name));
                                    }
                                    Document.CleanProblematicXamlBags();

                                    // Point pt = Mouse.GetPosition(MainSurface);
                                    /*
                                    double left = Canvas.GetLeft(uie);
                                    double top = Canvas.GetTop(uie);

                                    left = double.IsNaN(left) ? Canvas.GetLeft(uie) : left;
                                    top = double.IsNaN(top) ? Canvas.GetTop(uie) : top;

                                    left = double.IsNaN(left) ? 0 : left;
                                    top = double.IsNaN(top) ? 0 : top;
                                    */
                                    double left = Canvas.GetLeft(uie);
                                    double top = Canvas.GetTop(uie);

                                    left = double.IsNaN(left) ? Canvas.GetLeft(uie) : left;
                                    top = double.IsNaN(top) ? Canvas.GetTop(uie) : top;

                                    //left = double.IsNaN(left) ? 0 : left;
                                    //top = double.IsNaN(top) ? 0 : top;

                                    var feinserted = uie as FrameworkElement;
                                    if (Double.IsNaN(top))
                                    {
                                        if (Document.MapScreenEntities.ContainsKey(feinserted.Name) &&
                                            Document.MapScreenEntities[feinserted.Name].MapProblematicXamlWriterProperties != null &&
                                            Document.MapScreenEntities[feinserted.Name].MapProblematicXamlWriterProperties.ContainsKey("Canvas.Top"))
                                        {
                                            var value = Document.MapScreenEntities[feinserted.Name].MapProblematicXamlWriterProperties["Canvas.Top"];
                                            try
                                            {
                                                using (var reader = new StringReader(value))
                                                {
                                                    // object obj = s.Deserialize(reader);
                                                    using (var textReader = new XmlTextReader(reader))
                                                    {
                                                        var obj = System.Windows.Markup.XamlReader.Load(textReader);
                                                        top = Convert.ToDouble(obj);
                                                    }
                                                }
                                            }
                                            catch
                                            {

                                            }
                                        }
                                        else
                                            top = 0;
                                    }

                                    if (Double.IsNaN(left))
                                    {
                                        if (Document.MapScreenEntities.ContainsKey(feinserted.Name) &&
                                            Document.MapScreenEntities[feinserted.Name].MapProblematicXamlWriterProperties != null &&
                                            Document.MapScreenEntities[feinserted.Name].MapProblematicXamlWriterProperties.ContainsKey("Canvas.Left"))
                                        {
                                            var value = Document.MapScreenEntities[feinserted.Name].MapProblematicXamlWriterProperties["Canvas.Left"];
                                            try
                                            {
                                                using (var reader = new StringReader(value))
                                                {
                                                    // object obj = s.Deserialize(reader);
                                                    using (var textReader = new XmlTextReader(reader))
                                                    {
                                                        var obj = System.Windows.Markup.XamlReader.Load(textReader);
                                                        left = Convert.ToDouble(obj);
                                                    }
                                                }
                                            }
                                            catch
                                            {

                                            }
                                        }
                                        else
                                            left = 0;
                                    }

                                    if (Document.MapScreenEntities.ContainsKey(feinserted.Name) &&
                                        Document.MapScreenEntities[feinserted.Name].MapProblematicXamlWriterProperties != null &&
                                        Document.MapScreenEntities[feinserted.Name].MapProblematicXamlWriterProperties.ContainsKey("Width"))
                                    {
                                        var value = Document.MapScreenEntities[feinserted.Name].MapProblematicXamlWriterProperties["Width"];
                                        try
                                        {
                                            using (var reader = new StringReader(value))
                                            {
                                                // object obj = s.Deserialize(reader);
                                                using (var textReader = new XmlTextReader(reader))
                                                {
                                                    var obj = System.Windows.Markup.XamlReader.Load(textReader);
                                                    (uie as FrameworkElement).Width = Convert.ToDouble(obj);
                                                }
                                            }
                                        }
                                        catch
                                        {

                                        }
                                    }

                                    if (Document.MapScreenEntities.ContainsKey(feinserted.Name) &&
                                        Document.MapScreenEntities[feinserted.Name].MapProblematicXamlWriterProperties != null &&
                                        Document.MapScreenEntities[feinserted.Name].MapProblematicXamlWriterProperties.ContainsKey("Height"))
                                    {
                                        var value = Document.MapScreenEntities[feinserted.Name].MapProblematicXamlWriterProperties["Height"];
                                        try
                                        {
                                            using (var reader = new StringReader(value))
                                            {
                                                // object obj = s.Deserialize(reader);
                                                using (var textReader = new XmlTextReader(reader))
                                                {
                                                    var obj = System.Windows.Markup.XamlReader.Load(textReader);
                                                    (uie as FrameworkElement).Height = Convert.ToDouble(obj);
                                                }
                                            }
                                        }
                                        catch
                                        {

                                        }
                                    }

                                    left += GridSnapNumber * PasteCounter;
                                    top += GridSnapNumber * PasteCounter;

                                    //if (MainSurface is Canvas)
                                    //{
                                    Canvas.SetLeft(uie, left);
                                    Canvas.SetTop(uie, top);
                                    //}
                                    //else
                                    //{
                                    //    Canvas.SetLeft(uie, left);
                                    //    Canvas.SetTop(uie, top);
                                    //}
                                   
                                    SetBaseUri(feinserted);
                                    elementsToSelect.Add(uie);
                                }
                            }
                            catch (Exception ex)
                            {
                                GestureResultAdorner.ShowMessage(String.Format("{0} : {1}", Properties.Resources.CannotPasteContent, ex.Message), Mouse.GetPosition(ActiveLayer));
                            }
                        }

                    }
                    else if (!String.IsNullOrEmpty(xaml))
                    {
                        var listNamesToArray = listNames.ToArray();
                        if (listNamesToArray.Length < 1)
                            return;

                        try
                        {
                            Object content = xaml.ReadUIElement();

                            /*
                            if (content is Canvas)
                            {
                                Canvas canvas = content as Canvas;

                                while (canvas.Children.Count > 0)
                                {
                                    UIElement child = canvas.Children[0] as UIElement;

                                    double left = Canvas.GetLeft(child);
                                    double top = Canvas.GetTop(child);

                                    left = double.IsNaN(left) ? 0 : left;
                                    top = double.IsNaN(top) ? 0 : top;

                                    if (MainSurface is Canvas)
                                    {
                                        Canvas.SetLeft(child, left);
                                        Canvas.SetTop(child, top);
                                    }
                                    else
                                    {
                                        Canvas.SetLeft(child, left);
                                        Canvas.SetTop(child, top);
                                    }

                                    canvas.Children.Remove(child);
                                    ActiveLayer.Children.Add(child);
                                    DesignerProperties.SetIsInDesignMode(child, true);
                                    ScreenDocument.SetScreenDocument(child, Document);

                                    if (Utilities.WPF.XmlHelper.CheckProblematicXamlWriter(child, xaml))
                                    {
                                        Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, child as FrameworkElement, true, true);
                                        Document.SetProblematicXaml(child as FrameworkElement, xaml);
                                    }
                                    else
                                        Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, child as FrameworkElement);

                                    elementsToSelect.Add(child);
                                }
                            }
                            else
                            */
                            {
                                UIElement uie = content as UIElement;
                                if (uie != null)
                                {
                                    ActiveLayer.Children.Add(uie);

                                    if (Utilities.WPF.XmlHelper.CheckProblematicXamlWriter(uie, xaml))
                                    {
                                        var fe = uie as FrameworkElement;
                                        fe.Name = listNamesToArray[0].Key;
                                        renamed = Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, fe, true, false);
                                        Document.SetProblematicXaml(uie as FrameworkElement, xaml);
                                        if (mapEntity != null)
                                        {
                                            var mapInners = ScreenDocument.CleanMapInnners(mapEntity, listNamesToArray[0].Key);

                                            foreach (var key in mapEntity.Keys)
                                            {
                                                FrameworkElement element = null;
                                                if (renamed.ContainsKey(key))
                                                    element = MainSurface.FindName(renamed[key]) as FrameworkElement;
                                                else
                                                {
                                                    //if (renamed.Count > 0)
                                                    //    continue;
                                                    element = MainSurface.FindName(key) as FrameworkElement;
                                                }

                                                if (element != null)
                                                {
                                                    Document.AddInnersEntity(key, element.Name, mapInners);

                                                    Document.AddDynamicEntity(element);
                                                    Document.MapScreenEntities[element.Name].CopyAll(mapEntity[key]);
                                                    Document.MapScreenEntities[element.Name].Entity = element;

                                                    // if (mapEntity[key].SourceSymbolLinked)
                                                    {
                                                        try
                                                        {
                                                            Document.LoadRepositoryItem(this, MainSurface, element.Name, bAnimate: false);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            EditorComponent.UIInterface.ShowError(ex.Message);
                                                        }
                                                        //element.SetResourceReference(ContentControl.ContentProperty, element.Name);

                                                        //var bindingWidth = new Binding()
                                                        //{
                                                        //    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UIElement), 1),
                                                        //    Path = new PropertyPath("ActualWidth")
                                                        //};
                                                        //var bindingHeight = new Binding()
                                                        //{
                                                        //    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UIElement), 1),
                                                        //    Path = new PropertyPath("ActualHeight")
                                                        //};

                                                        //var fe2 = ((element as ContentControl).Content as FrameworkElement);
                                                        ////fe.Width = fe2.Width;
                                                        ////fe.Height = fe2.Height;
                                                        //fe2.SetBinding(FrameworkElement.WidthProperty, bindingWidth);
                                                        //fe2.SetBinding(FrameworkElement.HeightProperty, bindingHeight);
                                                    }
                                                    
                                                    Document.UpdateProblematicXamlWriterProperties(element, element.Name);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var fe = uie as FrameworkElement;
                                        fe.Name = listNamesToArray[0].Key;
                                        renamed = Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, fe, true, false);
                                        if (mapEntity != null)
                                        {
                                            var mapInners = ScreenDocument.CleanMapInnners(mapEntity, listNamesToArray[0].Key);

                                            foreach (var key in mapEntity.Keys)
                                            {
                                                FrameworkElement element = null;
                                                if (renamed.ContainsKey(key))
                                                    element = MainSurface.FindName(renamed[key]) as FrameworkElement;
                                                else
                                                {
                                                    //if (renamed.Count > 0)
                                                    //    continue;
                                                    element = MainSurface.FindName(key) as FrameworkElement;
                                                }

                                                if (element != null)
                                                {
                                                    Document.AddInnersEntity(key, element.Name, mapInners, MainSurface);

                                                    Document.AddDynamicEntity(element);
                                                    Document.MapScreenEntities[element.Name].CopyAll(mapEntity[key]);
                                                    Document.MapScreenEntities[element.Name].Entity = element;

                                                    if (!String.IsNullOrEmpty(Document.MapScreenEntities[element.Name].ProblematicXaml))
                                                        Utilities.WPF.XmlHelper.SetProblematicXamlWriter(element, Document.MapScreenEntities[element.Name].ProblematicXaml);

                                                    // Document.UpdateProblematicXamlWriterProperties(element, element.Name);

                                                    // if (mapEntity[key].SourceSymbolLinked)
                                                    {
                                                        try
                                                        {
                                                            Document.LoadRepositoryItem(this, MainSurface, element.Name, bAnimate: false);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            EditorComponent.UIInterface.ShowError(ex.Message);
                                                        }
                                                        //element.SetResourceReference(ContentControl.ContentProperty, element.Name);

                                                        //var bindingWidth = new Binding()
                                                        //{
                                                        //    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UIElement), 1),
                                                        //    Path = new PropertyPath("ActualWidth")
                                                        //};
                                                        //var bindingHeight = new Binding()
                                                        //{
                                                        //    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UIElement), 1),
                                                        //    Path = new PropertyPath("ActualHeight")
                                                        //};

                                                        //var fe2 = ((element as ContentControl).Content as FrameworkElement);
                                                        ////fe.Width = fe2.Width;
                                                        ////fe.Height = fe2.Height;
                                                        //fe2.SetBinding(FrameworkElement.WidthProperty, bindingWidth);
                                                        //fe2.SetBinding(FrameworkElement.HeightProperty, bindingHeight);
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    // Point pt = Mouse.GetPosition(MainSurface);
                                    double left = Canvas.GetLeft(uie);
                                    double top = Canvas.GetTop(uie);

                                    left = double.IsNaN(left) ? Canvas.GetLeft(uie) : left;
                                    top = double.IsNaN(top) ? Canvas.GetTop(uie) : top;

                                    //left = double.IsNaN(left) ? 0 : left;
                                    //top = double.IsNaN(top) ? 0 : top;

                                    var feinserted = uie as FrameworkElement;
                                    if (Double.IsNaN(top))
                                    {
                                        if (Document.MapScreenEntities.ContainsKey(feinserted.Name) &&
                                            Document.MapScreenEntities[feinserted.Name].MapProblematicXamlWriterProperties != null &&
                                            Document.MapScreenEntities[feinserted.Name].MapProblematicXamlWriterProperties.ContainsKey("Canvas.Top"))
                                        {
                                            var value = Document.MapScreenEntities[feinserted.Name].MapProblematicXamlWriterProperties["Canvas.Top"];
                                            try
                                            {
                                                using (var reader = new StringReader(value))
                                                {
                                                    // object obj = s.Deserialize(reader);
                                                    using (var textReader = new XmlTextReader(reader))
                                                    {
                                                        var obj = System.Windows.Markup.XamlReader.Load(textReader);
                                                        top = Convert.ToDouble(obj);
                                                    }
                                                }
                                            }
                                            catch
                                            {

                                            }
                                        }
                                        else
                                            top = 0;
                                    }

                                    if (Double.IsNaN(left))
                                    {
                                        if (Document.MapScreenEntities.ContainsKey(feinserted.Name) &&
                                            Document.MapScreenEntities[feinserted.Name].MapProblematicXamlWriterProperties != null &&
                                            Document.MapScreenEntities[feinserted.Name].MapProblematicXamlWriterProperties.ContainsKey("Canvas.Left"))
                                        {
                                            var value = Document.MapScreenEntities[feinserted.Name].MapProblematicXamlWriterProperties["Canvas.Left"];
                                            try
                                            {
                                                using (var reader = new StringReader(value))
                                                {
                                                    // object obj = s.Deserialize(reader);
                                                    using (var textReader = new XmlTextReader(reader))
                                                    {
                                                        var obj = System.Windows.Markup.XamlReader.Load(textReader);
                                                        left = Convert.ToDouble(obj);
                                                    }
                                                }
                                            }
                                            catch
                                            {

                                            }
                                        }
                                        else
                                            left = 0;
                                    }

                                    if (Document.MapScreenEntities.ContainsKey(feinserted.Name) &&
                                        Document.MapScreenEntities[feinserted.Name].MapProblematicXamlWriterProperties != null &&
                                        Document.MapScreenEntities[feinserted.Name].MapProblematicXamlWriterProperties.ContainsKey("Width"))
                                    {
                                        var value = Document.MapScreenEntities[feinserted.Name].MapProblematicXamlWriterProperties["Width"];
                                        try
                                        {
                                            using (var reader = new StringReader(value))
                                            {
                                                // object obj = s.Deserialize(reader);
                                                using (var textReader = new XmlTextReader(reader))
                                                {
                                                    var obj = System.Windows.Markup.XamlReader.Load(textReader);
                                                    (uie as FrameworkElement).Width = Convert.ToDouble(obj);
                                                }
                                            }
                                        }
                                        catch
                                        {

                                        }
                                    }

                                    if (Document.MapScreenEntities.ContainsKey(feinserted.Name) &&
                                        Document.MapScreenEntities[feinserted.Name].MapProblematicXamlWriterProperties != null &&
                                        Document.MapScreenEntities[feinserted.Name].MapProblematicXamlWriterProperties.ContainsKey("Height"))
                                    {
                                        var value = Document.MapScreenEntities[feinserted.Name].MapProblematicXamlWriterProperties["Height"];
                                        try
                                        {
                                            using (var reader = new StringReader(value))
                                            {
                                                // object obj = s.Deserialize(reader);
                                                using (var textReader = new XmlTextReader(reader))
                                                {
                                                    var obj = System.Windows.Markup.XamlReader.Load(textReader);
                                                    (uie as FrameworkElement).Height = Convert.ToDouble(obj);
                                                }
                                            }
                                        }
                                        catch
                                        {

                                        }
                                    }

                                    left += GridSnapNumber * PasteCounter;
                                    top += GridSnapNumber * PasteCounter;

                                    //if (MainSurface is Canvas)
                                    //{
                                        Canvas.SetLeft(uie, left);
                                        Canvas.SetTop(uie, top);
                                    //}
                                    //else
                                    //{
                                    //    Canvas.SetLeft(uie, left);
                                    //    Canvas.SetTop(uie, top);
                                    //}

                                    if (uie is Panel)
                                        Document.RestoreProblematicXamlWriter(MainSurface, (uie as Panel).Name);
                                    else
                                    {
                                        uie.GetChildrenOfType<Panel>().ToList()
                                            .ForEach(panel => Document.RestoreProblematicXamlWriter(MainSurface, (uie as FrameworkElement).Name));
                                    }
                                    Document.CleanProblematicXamlBags();

                                    SetBaseUri(feinserted);
                                    elementsToSelect.Add(uie);
                                }
                                else
                                    GestureResultAdorner.ShowMessage(Properties.Resources.CannotPasteContent, new Point(0, 0));
                            }
                        }
                        catch (Exception ex)
                        {
                            GestureResultAdorner.ShowMessage(String.Format("{0} : {1}", Properties.Resources.CannotPasteContent, ex.Message), new Point(0, 0));
                            return;
                        }
                        renamedMulti.AddRange(renamed.Values);
                    }

                    foreach (var name in renamedMulti)
                    {
                        Document.UpdateRepositoryItems(this, MainSurface, true, UpdateOnlyThoseWithStyle: false, bRecreateResources: true, entityname: name); // case 18063
                        Document.GetListInners(name).ForEach(I => 
                        {
                            Document.UpdateRepositoryItems(this, MainSurface, true, UpdateOnlyThoseWithStyle: false, bRecreateResources: true, entityname: I); // case 18063

                            UIElement uie = Document.FindInnerControl(MainSurface, I);
                            if (uie == null)
                                return;

                            var entity = Document.MapScreenEntities[I];
                            entity.Entity = uie;
                            entity.Document = Document;
                        });
                    }

                    PasteCounter++;
                    //now that we're done, we select
                    ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                    foreach (var uie in elementsToSelect)
                    {
                        UpdateHMIControl(uie);
                        ChangeLanguage(lastCulture, uie as FrameworkElement);
                    }
                    MainSurface_SetCurrentSelection(readonlyList);
                }
            }

            //Document.UpdateRepositoryItems(this, MainSurface, true, UpdateOnlyThoseWithStyle: false, bRecreateResources: true); // case 10871

            Focusable = true;
            Focus();
        }

        private void OnCopyReference(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var listSelected = MainSurface_GetCurrentSelection();

            if (listSelected.Count != 1)
                return;

            var entity = GetSelectedEntity(listSelected[0] as FrameworkElement);
            if (entity.OpcuaEntityReference == null)
                return;

            var str = entity.OpcuaEntityReference.ToXml();
            Clipboard.SetText(str, TextDataFormat.UnicodeText);
        }

        private void OnCopyCommands(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var listSelected = MainSurface_GetCurrentSelection();

            if (listSelected.Count != 1)
                return;

            var entity = GetSelectedEntity(listSelected[0] as FrameworkElement);
            if (!entity.HasCommands)
                return;

            var str = entity.CommandList.ToXml();
            Clipboard.SetText(str, TextDataFormat.UnicodeText);
        }

        private void OnCopyFontSettingList(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var listSelected = MainSurface_GetCurrentSelection();

            if (listSelected.Count != 1)
                return;

            var entity = GetSelectedEntity(listSelected[0] as FrameworkElement);
            var fsl = entity.FontSettingList?.ToXml();
            Clipboard.SetText(fsl, TextDataFormat.UnicodeText);
        }

        private void OnPasteFontSettingList(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var fsl = Clipboard.GetText(TextDataFormat.UnicodeText);
            if (fsl == null || fsl.Count() == 0)
                return;
            var listSelected = MainSurface_GetCurrentSelection();
            if (listSelected.Count > 0)
                using (new ChangeScreenWatcher(this))
                {
                    listSelected.ForEach(fe =>
                    {
                        var entity = GetSelectedEntity(fe as FrameworkElement);
                        entity.FontSettingList = fsl.FromXml<Dictionary<string, FontSettings>>();
                    });
                }
        }

        private void OnCopyAnimations(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var listSelected = MainSurface_GetCurrentSelection();

            if (listSelected.Count != 1)
                return;

            var entity = GetSelectedEntity(listSelected[0] as FrameworkElement);
            if (!entity.HasAnimations)
                return;

            var str = entity.ListAnimations.ToXml();
            Clipboard.SetText(str, TextDataFormat.UnicodeText);
        }

        private void OnPasteReference(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var str = Clipboard.GetText(TextDataFormat.UnicodeText);
            var listSelected = MainSurface_GetCurrentSelection();
            if (listSelected.Count != 1)
                return;

            using (new ChangeScreenWatcher(this))
            {
                var entity = GetSelectedEntity(listSelected[0] as FrameworkElement);
                entity.OpcuaEntityReference = str.FromXml<OPCUAEntityReference>();
            }
        }

        private void OnPasteCommands(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var str = Clipboard.GetText(TextDataFormat.UnicodeText);
            var listSelected = MainSurface_GetCurrentSelection();
            if (listSelected.Count != 1)
                return;

            using (new ChangeScreenWatcher(this))
            {
                var entity = GetSelectedEntity(listSelected[0] as FrameworkElement);
                entity.CommandList = str.FromXml<CommandManagerList>();
            }
        }

        private void OnPasteAnimations(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var str = Clipboard.GetText(TextDataFormat.UnicodeText);
            var listSelected = MainSurface_GetCurrentSelection();
            if (listSelected.Count != 1)
                return;

            using (new ChangeScreenWatcher(this))
            {
                var entity = GetSelectedEntity(listSelected[0] as FrameworkElement);
                var list = str.FromXml<AnimationManagerList>();
                var element = listSelected[0];
                var animationList = (from am in list where !am.NotSupportedControl(element) select am).ToList();
                entity.AnimationList = animationList;
                if (animationList.Count < list.Count)
                    MessageBox.Show(Properties.Resources.NotAllAnimationPasted);
            }
        }

        void CanExecuteCopyReference(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (Document == null)
                return;
            var listSelected = MainSurface_GetCurrentSelection();

            if (listSelected.Count != 1)
                return;

            var entity = GetSelectedEntity(listSelected[0] as FrameworkElement);
            e.CanExecute = entity.OpcuaEntityReference != null;
        }

        void CanExecuteCopyCommands(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (Document == null)
                return;
            var listSelected = MainSurface_GetCurrentSelection();

            if (listSelected.Count != 1)
                return;

            var entity = GetSelectedEntity(listSelected[0] as FrameworkElement);
            e.CanExecute = entity.HasCommands;
        }

        void CanExecuteCopyAnimations(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (Document == null)
                return;
            var listSelected = MainSurface_GetCurrentSelection();

            if (listSelected.Count != 1)
                return;

            var entity = GetSelectedEntity(listSelected[0] as FrameworkElement);
            e.CanExecute = entity.HasAnimations;
        }

        void CanExecuteCopyFontSettingList(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (Document == null)
                return;
            var listSelected = MainSurface_GetCurrentSelection();

            if (listSelected.Count != 1)
                return;

            var entity = GetSelectedEntity(listSelected[0] as FrameworkElement);
            e.CanExecute = entity.FontSettingList?.Count() > 0;
        }

        void CanExecutePasteFontSettingList(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (Document == null)
                return;
            var listSelected = MainSurface_GetCurrentSelection();

            try
            {
                string str = Clipboard.GetText(TextDataFormat.UnicodeText);
                e.CanExecute = listSelected.Count > 0 && str.FromXml<Dictionary<string, FontSettings>>() != null;
            }
            catch (Exception ex)
            {
                e.CanExecute = false;
            }
        }

        void CanExecutePasteReference(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                if (Document == null)
                {
                    e.CanExecute = false;
                    return;
                }
                string str = Clipboard.GetText(TextDataFormat.UnicodeText);
                e.CanExecute = str.FromXml<OPCUAEntityReference>() != null;
            }
            catch (Exception ex)
            {
                e.CanExecute = false;
            }
        }

        void CanExecutePasteCommands(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                if (Document == null)
                {
                    e.CanExecute = false;
                    return;
                }
                string str = Clipboard.GetText(TextDataFormat.UnicodeText);
                e.CanExecute = str.FromXml<CommandManagerList>() != null;
            }
            catch (Exception ex)
            {
                e.CanExecute = false;
            }
        }

        void CanExecutePasteAnimations(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                if (Document == null)
                {
                    e.CanExecute = false;
                    return;
                }
                string str = Clipboard.GetText(TextDataFormat.UnicodeText);
                e.CanExecute = str.FromXml<AnimationManagerList>() != null;
            }
            catch (Exception ex)
            {
                e.CanExecute = false;
            }
        }

        void CanExecutePaste(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (Document == null)
                return;
            try
            {
                var dataObject = Clipboard.GetDataObject() as DataObject;
                if (dataObject != null)
                {
                    if (dataObject.ContainsText(TextDataFormat.Xaml) || dataObject.ContainsText(TextDataFormat.Text))
                        e.CanExecute = true;
                    else
                    {
                        var listXaml = new List<String>();
                        var listXamlString = dataObject.GetData(listXaml.GetType()) as String;
                        e.CanExecute = listXamlString != null;
                    }
                }
            }
            catch (Exception ex)
            {
                e.CanExecute = false;
            }
        }

        bool bDroppingCodeCancelled;
        private void OnExecuteDroppingCode(object sender, ExecutedRoutedEventArgs e)
        {
            bDroppingCodeCancelled = false;
            using (var cursor = new WaitCursor())
            {
                if (e != null)
                    e.Handled = true;
                var listSelected = MainSurface_GetCurrentSelection();

                if (listSelected.Count != 1)
                    return;

                var element = listSelected[0] as FrameworkElement;
                if (element != null && !(element is UserControl))
                {
                    var list = element.GetChildrenOfType<FrameworkElement>();
                    foreach (var el in list)
                    {
                        if (!Utilities.WPF.XmlHelper.IsProblematicXamlWriter(el))
                            continue;

                        var name = GetSelectedEntityName(el);
                        if (!String.IsNullOrEmpty(name))
                            Document.SubscribePropertyChangeXamlWriterProperties(el, name);
                    }
                }

                var entity = GetSelectedEntity(element);

                var code = EditorComponent.SymbolGallery.GetSymbolCode(entity.SourceSymbolProvider,
                                                                        entity.SourceSymbolPath, Document.rootBase);
                using (var droppingCode = new DroppingCode(code, entity.EntityName, listSelected[0], entity))
                {
                    var arg = new CancelEventArgs();
                    droppingCode.ExecuteScriptCode(arg);
                    bDroppingCodeCancelled = arg.Cancel;
                }

                if (element != null && !(element is UserControl))
                {
                    var list = element.GetChildrenOfType<FrameworkElement>();
                    foreach (var el in list)
                    {
                        if (!Utilities.WPF.XmlHelper.IsProblematicXamlWriter(el))
                            continue;

                        Document.UnsubscribePropertyChangeXamlWriterProperties(el);
                    }
                }
            }
        }

        UIElement lastSelectedCanExecuteDrop;
        bool blastSelectedCanExecuteDrop;
        void CanExecuteDroppingCode(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (Document == null)
                return;

            var listSelected = MainSurface_GetCurrentSelection();
            if (listSelected.Count != 1)
            {
                executeCode.IsVisible = false;
                return;
            }

            if (lastSelectedCanExecuteDrop == listSelected[0])
            {
                executeCode.IsVisible = e.CanExecute = blastSelectedCanExecuteDrop;
                return;
            }

            lastSelectedCanExecuteDrop = listSelected[0];
            var entity = GetSelectedEntity(listSelected[0] as FrameworkElement);
            if (!String.IsNullOrWhiteSpace(entity.SourceSymbolPath))
            {
                var task = Task.Factory.StartNew(() =>
                {
                    var code = EditorComponent.SymbolGallery.GetSymbolCode(entity.SourceSymbolProvider,
                                                                            entity.SourceSymbolPath, Document.rootBase);
                    return code;
                });

                task.ContinueWith(t =>
                {
                    blastSelectedCanExecuteDrop = e.CanExecute = !String.IsNullOrWhiteSpace(t.Result);
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();

                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
                blastSelectedCanExecuteDrop = false;

            executeCode.IsVisible = blastSelectedCanExecuteDrop;
        }

        private void OnRefreshRepositoryItems(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var list = (from entry in Document.MapScreenEntities.AsParallel()
                        where entry.Value.SourceSymbolLinked
                        select entry.Value).ToList();
            RefreshRepositoryItems(list);
        }
        private void RefreshRepositoryItems(List<ScreenEntity> list)
        {
            list.ForEach(entity =>
            {
                RefreshRepositoryItem(entity);
            });
        }

        private void RefreshRepositoryItem(ScreenEntity entity)
        {
            try
            {
                var selected = (entity as IEntityReference).ContainedObject;
                var contained = (selected as ContentControl);
                if (contained != null)
                {
                    Document.RefreshEntityStyleBinding(MainSurface, true, contained.Name);
                    Document.UpdateRepositoryItems(this, MainSurface, true, true, UpdateOnlyThoseWithStyle: false, bRecreateResources: true, entityname: contained.Name);
                }
            }
            catch (Exception ex)
            {
                EditorComponent.UIInterface.ShowError(ex.Message);
            }
        }

        void CanRefreshRepositoryItems(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnSelectNext(object sender, ExecutedRoutedEventArgs e)
        {
            var elementsToSelect = MainSurface_GetCurrentSelection();
            if (elementsToSelect.Count == 0)
            {
                if (ActiveLayer.Children.Count > 0)
                    elementsToSelect.Add(ActiveLayer.Children[0] as UIElement);
            }
            else
            {
                int nFound = ActiveLayer.Children.IndexOf(elementsToSelect[0]);
                elementsToSelect.Clear();
                if (nFound >= 0 && ++nFound < ActiveLayer.Children.Count)
                    elementsToSelect.Add(ActiveLayer.Children[nFound] as UIElement);
            }

            ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
            MainSurface_SetCurrentSelection(readonlyList);

            e.Handled = true;
        }

        private void OnSelectPrev(object sender, ExecutedRoutedEventArgs e)
        {
            var elementsToSelect = MainSurface_GetCurrentSelection();
            if (elementsToSelect.Count == 0)
            {
                if (ActiveLayer.Children.Count > 0)
                    elementsToSelect.Add(ActiveLayer.Children[ActiveLayer.Children.Count - 1] as UIElement);
            }
            else
            {
                int nFound = ActiveLayer.Children.IndexOf(elementsToSelect[0]);
                elementsToSelect.Clear();
                if (nFound >= 0 && --nFound >= 0)
                    elementsToSelect.Add(ActiveLayer.Children[nFound] as UIElement);
            }

            ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
            MainSurface_SetCurrentSelection(readonlyList);

            e.Handled = true;
        }

        /*
        private void OnChangeEditMode(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            switch (MainSurface.EditingMode)
            {
                case CanvasEditingMode.None: MainSurface.EditingMode = MainSurface.EditingMode = CanvasEditingMode.Ink; break;
                case CanvasEditingMode.Ink: MainSurface.EditingMode = MainSurface.EditingMode = CanvasEditingMode.GestureOnly; break;
                case CanvasEditingMode.GestureOnly: MainSurface.EditingMode = MainSurface.EditingMode = CanvasEditingMode.InkAndGesture; break;
                case CanvasEditingMode.InkAndGesture: MainSurface.EditingMode = MainSurface.EditingMode = CanvasEditingMode.Select; break;
                case CanvasEditingMode.Select: MainSurface.EditingMode = MainSurface.EditingMode = CanvasEditingMode.EraseByPoint; break;
                case CanvasEditingMode.EraseByPoint: MainSurface.EditingMode = MainSurface.EditingMode = CanvasEditingMode.EraseByStroke; break;
                case CanvasEditingMode.EraseByStroke: MainSurface.EditingMode = MainSurface.EditingMode = CanvasEditingMode.None; break;
            }

            GestureResultAdorner.ShowMessage(MainSurface.EditingMode.ToString(), Mouse.GetPosition(MainSurface));
        }
        */

        private void OnMoveLast(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();
            if (listSelected.Count != 1)
                return;

            using (new ChangeScreenWatcher(this))
            {
                ActiveLayer.Children.Remove(listSelected[0]);
                ActiveLayer.Children.Insert(0, listSelected[0]);

                var fe = listSelected[0] as FrameworkElement;
                if (fe != null && Document != null)
                {
                    String name = fe.Name;
                    if (String.IsNullOrEmpty(name))
                        name = fe.DependencyObjectType.Name;
                    try
                    {
                        Document.LoadRepositoryItem(this, MainSurface, name, bAnimate: false);
                    }
                    catch (Exception ex)
                    {
                        EditorComponent.UIInterface.ShowError(ex.Message);
                    }
                }

                var top = Canvas.GetTop(listSelected[0]) + 10;
                var left = Canvas.GetLeft(listSelected[0]) + 10;
                GestureResultAdorner.ShowMessage(ActiveLayer.Children.IndexOf(listSelected[0]).ToString(),
                                                 new Point(left, top));
            }
        }

        private void OnMoveFirst(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();
            if (listSelected.Count != 1)
                return;

            using (new ChangeScreenWatcher(this))
            {
                ActiveLayer.Children.Remove(listSelected[0]);
                ActiveLayer.Children.Add(listSelected[0]);

                var fe = listSelected[0] as FrameworkElement;
                if (fe != null && Document != null)
                {
                    String name = fe.Name;
                    if (String.IsNullOrEmpty(name))
                        name = fe.DependencyObjectType.Name;
                    try
                    {
                        Document.LoadRepositoryItem(this, MainSurface, name, bAnimate: false);
                    }
                    catch (Exception ex)
                    {
                        EditorComponent.UIInterface.ShowError(ex.Message);
                    }
                }

                var top = Canvas.GetTop(listSelected[0]) + 10;
                var left = Canvas.GetLeft(listSelected[0]) + 10;
                GestureResultAdorner.ShowMessage(ActiveLayer.Children.IndexOf(listSelected[0]).ToString(),
                                                 new Point(left, top));
            }
        }

        private void OnMovePrev(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();
            if (listSelected.Count == 1)
            {
                int index = MainSurface.Children.IndexOf(listSelected[0]);
                if (index > 0)
                {
                    using (new ChangeScreenWatcher(this))
                    {
                        ActiveLayer.Children.Remove(listSelected[0]);
                        ActiveLayer.Children.Insert(index - 1, listSelected[0]);

                        var fe = listSelected[0] as FrameworkElement;
                        if (fe != null && Document != null)
                        {
                            String name = fe.Name;
                            if (String.IsNullOrEmpty(name))
                                name = fe.DependencyObjectType.Name;
                            try
                            {
                                Document.LoadRepositoryItem(this, MainSurface, name, bAnimate: false);
                            }
                            catch (Exception ex)
                            {
                                EditorComponent.UIInterface.ShowError(ex.Message);
                            }
                        }

                        var top = Canvas.GetTop(listSelected[0]) + 10;
                        var left = Canvas.GetLeft(listSelected[0]) + 10;
                        GestureResultAdorner.ShowMessage(ActiveLayer.Children.IndexOf(listSelected[0]).ToString(),
                                                         new Point(left, top));
                    }
                }
            }
        }

        private void OnMoveNext(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();
            if (listSelected.Count == 1)
            {
                int index = ActiveLayer.Children.IndexOf(listSelected[0]);
                if (index < ActiveLayer.Children.Count - 1)
                {
                    using (new ChangeScreenWatcher(this))
                    {
                        ActiveLayer.Children.Remove(listSelected[0]);
                        if (index + 1 >= ActiveLayer.Children.Count)
                            ActiveLayer.Children.Add(listSelected[0]);
                        else
                            ActiveLayer.Children.Insert(index + 1, listSelected[0]);

                        var fe = listSelected[0] as FrameworkElement;
                        if (fe != null && Document != null)
                        {
                            String name = fe.Name;
                            if (String.IsNullOrEmpty(name))
                                name = fe.DependencyObjectType.Name;
                            try
                            {
                                Document.LoadRepositoryItem(this, MainSurface, name, bAnimate:false);
                            }
                            catch (Exception ex)
                            {
                                EditorComponent.UIInterface.ShowError(ex.Message);
                            }
                        }

                        var top = Canvas.GetTop(listSelected[0]) + 10;
                        var left = Canvas.GetLeft(listSelected[0]) + 10;
                        GestureResultAdorner.ShowMessage(ActiveLayer.Children.IndexOf(listSelected[0]).ToString(),
                                                         new Point(left, top));
                    }
                }
            }
        }

        bool bSetZOrderInProgress;
        bool zOrderChanged;
        int currentTabIndex;
        readonly Dictionary<UIElement, SetZOrderAdorner> mapZOrderAdorners = new Dictionary<UIElement, SetZOrderAdorner>();

        void CancelSetZOrder()
        {
            if (bSetZOrderInProgress)
            {
                bSetZOrderInProgress = false;
                foreach (KeyValuePair<UIElement, SetZOrderAdorner> data in mapZOrderAdorners)
                {
                    //var adorner = AdornerLayer.GetAdornerLayer(data.Key);
                    //if (adorner == null)
                    var adorner = AdornerLayer.GetAdornerLayer(MainSurface);
                    if (adorner != null)
                        adorner.Remove(data.Value);
                }
                mapZOrderAdorners.Clear();

                if (zOrderChanged)
                    MainSurface_SetModified(true);
            }
        }

        private void OnSetZOrder(object sender, ExecutedRoutedEventArgs e)
        {
            if (bSetZOrderInProgress)
            {
                CancelSetZOrder();
            }
            else
            {
                MainSurface_CleanCurrentSelectedion();

                bSetZOrderInProgress = true;
                zOrderChanged = false;
                currentTabIndex = 0;

                var adorner = AdornerLayer.GetAdornerLayer(MainSurface);

                foreach (UIElement element in ActiveLayer.Children)
                {
                    var ad = new SetZOrderAdorner(element);
                    adorner.Add(ad);
                    mapZOrderAdorners.Add(element, ad);

                    ad.PreviewMouseDown += (o, ev) =>
                        {
                            ActiveLayer.Children.Remove(element);
                            ActiveLayer.Children.Insert(currentTabIndex, element);

                            var fe = element as FrameworkElement;
                            if (fe != null && Document != null)
                            {
                                String name = fe.Name;
                                if (String.IsNullOrEmpty(name))
                                    name = fe.DependencyObjectType.Name;
                                try
                                {
                                    Document.LoadRepositoryItem(this, MainSurface, name, bAnimate: false);
                                }
                                catch (Exception ex)
                                {
                                    EditorComponent.UIInterface.ShowError(ex.Message);
                                }
                            }

                            if (++currentTabIndex >= ActiveLayer.Children.Count)
                                currentTabIndex = 0;

                            zOrderChanged = true;
                            mapZOrderAdorners.Values.ToList().ForEach(value =>
                                {
                                    value.SetIndex();
                                });
                        };
                }
            }
        }

        private void CanSetZOrder(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (ActiveLayer.Children.Count > 1 || bSetZOrderInProgress) && layoutItems.Visibility == Visibility.Collapsed;
        }

        private void OnAddSymolLibrary(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();
            List<UIElement> listItemToSelect = new List<UIElement>();
            if (listSelected == null || listSelected.Count < 1)
                return;

            MainSurface_CleanCurrentSelectedion();

            listSelected.ForEach(uie =>
                {
                    var fe = uie as FrameworkElement;
                    String name = fe.Name;
                    if (String.IsNullOrEmpty(name))
                        name = fe.DependencyObjectType.Name;

                    if(uie is Viewbox)
                        listItemToSelect.Add(uie);
                    else
                    {
                        var res = EditorComponent.UIInterface.ShowYesNo(string.Format(Properties.Resources.AddSymbolToLibraryWarning, name), UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Question);
                        if (res == CustomDialogResults.No)
                        {
                            listItemToSelect.Add(uie);
                            return;
                        }
                        List<UIElement> tempListItemToSelect = new List<UIElement>() { uie };
                        ReadOnlyCollection<UIElement> tempReadonlyList = new ReadOnlyCollection<UIElement>(tempListItemToSelect);
                        MainSurface_SetCurrentSelection(tempReadonlyList);
                        CreateGroup();
                        uie = MainSurface_GetCurrentSelection().FirstOrDefault();
                        MainSurface_CleanCurrentSelectedion();
                        if (uie == null || !(uie is Viewbox))
                        {
                            EditorComponent.UIInterface.ShowError(string.Format(Properties.Resources.ErrorGroupingElement, name));
                            return;
                        }

                        listItemToSelect.Add(uie);
                        fe = uie as FrameworkElement;
                        name = fe.Name;
                        if (String.IsNullOrEmpty(name))
                            name = fe.DependencyObjectType.Name;
                    }


                    var doc = Document.CreateSmartTemplateDocument(fe);
                    if (doc.IsEmpty)
                        doc = null;

                    Document.ResolveProblematicXamlOnCanvas(MainSurface);
                    Document.ClearRepositoryItem(MainSurface, name);
                    RestoreUntranslated(fe);
                    var xaml = uie.XamlWriterFormatted();
                    try
                    {
                        Document.LoadRepositoryItem(this, MainSurface, name);
                    }
                    catch (Exception ex)
                    {
                        EditorComponent.UIInterface.ShowError(ex.Message);
                    }
                    Document.RestoreProblematicXamlWriter(MainSurface);
                    Document.CleanProblematicXamlBags();

                    EditorComponent.SymbolGallery.AddSymbolToLibrary(Document.Title, name, xaml, doc != null ? doc.ToXml() : null);
                    ElementToCurrentLanguage(fe);
                });

            ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(listItemToSelect);
            MainSurface_SetCurrentSelection(readonlyList);
        }

        private void OnUpdateSymolLibrary(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            List<UIElement> listSelected = MainSurface_GetCurrentSelection();
            
            listSelected.ForEach(uie =>
            {
                var fe = uie as FrameworkElement;
                String name = fe.Name;
                if (String.IsNullOrEmpty(name))
                    name = fe.DependencyObjectType.Name;

                var doc = Document.CreateSmartTemplateDocument(fe);
                if (doc.IsEmpty)
                    doc = null;

                Document.ResolveProblematicXamlOnCanvas(MainSurface);
                var xaml = uie.XamlWriterFormatted();
                Document.RestoreProblematicXamlWriter(MainSurface);
                Document.CleanProblematicXamlBags();

                var entity = GetSelectedEntity(uie as FrameworkElement);

                var ret = EditorComponent.SymbolGallery.UpdateSymbolToLibrary(xaml, doc != null ? doc.ToXml() : null,
                    entity.SourceSymbolProvider, entity.SourceSymbolPath, Document.rootBase);
                Document.ForceUnload();

                var list = (from entry in Document.MapScreenEntities.AsParallel()
                            where entry.Value.SourceSymbolLinked &&
                                  entry.Value.SourceSymbolProvider == entity.SourceSymbolProvider &&
                                  entry.Value.SourceSymbolPath == entity.SourceSymbolPath
                            select entry.Value).ToList();
                RefreshRepositoryItems(list);

                if (!ret)
                    EditorComponent.UIInterface.ShowError(Properties.Resources.ErrorUpdatingSymbol);
                else
                {
                    EditorComponent.UIInterface.ShowInformation(Properties.Resources.SymbolUpdated);
                    if (Document.Parent != null)
                    {
                        try
                        {
                            var screensRoot = String.Format("{0}\\{1}", Document.Parent.rootBase, ScreenManagerComponent.screenManagerComponent.TypeLabel);
                            if (Directory.Exists(screensRoot))
                            {
                                using (var cursor = new WaitCursor())
                                {
                                    var di = new DirectoryInfo(screensRoot);
                                    foreach (var file in di.EnumerateFiles(String.Format("*{0}*", ScreenCompilerManager.ScreenCompilerManager.GetCompiledExtension()), SearchOption.AllDirectories).ToList())
                                        file.Delete();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            EditorComponent.UIInterface.ShowError(String.Format(Properties.Resources.CompiledFilesDeletionFailed, ex.Message));
                        }
                    }
                }
            });
        }

        private void CanExecuteUpdateSymbolLibrary(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            var listToSelect = MainSurface_GetCurrentSelection();
            if (listToSelect.Count == 1)
            {
                var entity = GetSelectedEntity(listToSelect[0] as FrameworkElement);
                e.CanExecute = entity.SourceSymbolLinkedPassive;
            }
        }

        private void OnAlignCenterHorizontal(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var listSelected = MainSurface_GetCurrentSelection();
            if (listSelected.Count > 0)
            {
                var elementsToSelect = new List<UIElement>();
                using (new ChangeScreenWatcher(this))
                {
                    MainSurface_CleanCurrentSelectedion();
                    double refPoint = Double.MaxValue;
                    refPoint = borderMainSurface.ActualWidth / 2;
                    bool bMoveGroup = listSelected.Count > 1;
                    double MainGroupWidth;
                    double MainGroupHeight;
                    UIElement minLefObject = listSelected[0];
                    UIElement minTopObject = listSelected[0];
                    UIElement maxRightObject = listSelected[0];
                    UIElement maxBottomObject = listSelected[0];

                    //if (MainSurface is Canvas)
                    //{
                        minLefObject = listSelected.MinElement(item => Canvas.GetLeft(item));
                        minTopObject = listSelected.MinElement(item => Canvas.GetTop(item));
                        maxRightObject = listSelected.MaxElement(item => Canvas.GetLeft(item) + (item as FrameworkElement).ActualWidth);
                        maxBottomObject = listSelected.MaxElement(item => Canvas.GetTop(item) + (item as FrameworkElement).ActualHeight);
                        MainGroupWidth = Canvas.GetLeft(maxRightObject) + (maxRightObject as FrameworkElement).ActualWidth - Canvas.GetLeft(minLefObject);
                        MainGroupHeight = Canvas.GetTop(maxBottomObject) + (maxBottomObject as FrameworkElement).ActualHeight - Canvas.GetTop(minTopObject);
                    //}
                    //else
                    //{
                    //    minLefObject = listSelected.MinElement(item => Canvas.GetLeft(item));
                    //    minTopObject = listSelected.MinElement(item => Canvas.GetTop(item));
                    //    maxRightObject = listSelected.MaxElement(item => Canvas.GetLeft(item) + (item as FrameworkElement).ActualWidth);
                    //    maxBottomObject = listSelected.MaxElement(item => Canvas.GetTop(item) + (item as FrameworkElement).ActualHeight);
                    //    MainGroupWidth = Canvas.GetLeft(maxRightObject) + (maxRightObject as FrameworkElement).ActualWidth - Canvas.GetLeft(minLefObject);
                    //    MainGroupHeight = Canvas.GetTop(maxBottomObject) + (maxBottomObject as FrameworkElement).ActualHeight - Canvas.GetTop(minTopObject);
                    //}


                    listSelected.ForEach(uie =>
                        {
                            double factor = !bMoveGroup && (uie as FrameworkElement).RenderTransformOrigin != null ? (uie as FrameworkElement).RenderTransformOrigin.X : 0.5;
                            //double dWidth = (uie as FrameworkElement).ActualWidth * factor;
                            double dWidth = MainGroupWidth * factor;
                            double dTargetLeft = refPoint - dWidth;
                            
                            //if (MainSurface is Canvas)
                            //{
                                dTargetLeft = Canvas.GetLeft(uie) - Canvas.GetLeft(minLefObject) + dTargetLeft;
                            //}
                            //else
                            //{
                            //    dTargetLeft = Canvas.GetLeft(uie) - Canvas.GetLeft(minLefObject) + dTargetLeft;
                            //}

                            DoubleAnimation daLeft = new DoubleAnimation()
                            {
                                To = dTargetLeft,
                                Duration = new Duration(TimeSpan.FromMilliseconds(nAnimationTime)), 
                                EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut }
                            };

                            daLeft.Completed += (o, u) =>
                            {
                                elementsToSelect.Add(uie);

                                //if (MainSurface is Canvas)
                                //{
                                    uie.BeginAnimation(Canvas.LeftProperty, null);
                                    Canvas.SetLeft(uie, dTargetLeft);
                                //}
                                //else
                                //{
                                //    uie.BeginAnimation(Canvas.LeftProperty, null);
                                //    Canvas.SetLeft(uie, dTargetLeft);
                                //}

                                if (listSelected.Count == elementsToSelect.Count)
                                {
                                    var readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                                    MainSurface_SetCurrentSelection(readonlyList);
                                }
                            };


                            //if (MainSurface is Canvas)
                                uie.BeginAnimation(Canvas.LeftProperty, daLeft);
                            //else
                            //    uie.BeginAnimation(Canvas.LeftProperty, daLeft);
                        });
                }
            }
        }

        private void OnAlignCenterVertical(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var listSelected = MainSurface_GetCurrentSelection();
            if (listSelected.Count > 0)
            {
                var elementsToSelect = new List<UIElement>();
                using (new ChangeScreenWatcher(this))
                {
                    MainSurface_CleanCurrentSelectedion();
                    double refPoint = Double.MaxValue;
                    refPoint = borderMainSurface.ActualHeight / 2;
                    bool bMoveGroup = listSelected.Count > 1;
                    double MainGroupWidth;
                    double MainGroupHeight;
                    UIElement minLefObject = listSelected[0];
                    UIElement minTopObject = listSelected[0];
                    UIElement maxRightObject = listSelected[0];
                    UIElement maxBottomObject = listSelected[0];

                    //if (MainSurface is Canvas)
                    //{
                        minLefObject = listSelected.MinElement(item => Canvas.GetLeft(item));
                        minTopObject = listSelected.MinElement(item => Canvas.GetTop(item));
                        maxRightObject = listSelected.MaxElement(item => Canvas.GetLeft(item) + (item as FrameworkElement).ActualWidth);
                        maxBottomObject = listSelected.MaxElement(item => Canvas.GetTop(item) + (item as FrameworkElement).ActualHeight);
                        MainGroupWidth = Canvas.GetLeft(maxRightObject) + (maxRightObject as FrameworkElement).ActualWidth - Canvas.GetLeft(minLefObject);
                        MainGroupHeight = Canvas.GetTop(maxBottomObject) + (maxBottomObject as FrameworkElement).ActualHeight - Canvas.GetTop(minTopObject);
                    //}
                    //else
                    //{
                    //    minLefObject = listSelected.MinElement(item => Canvas.GetLeft(item));
                    //    minTopObject = listSelected.MinElement(item => Canvas.GetTop(item));
                    //    maxRightObject = listSelected.MaxElement(item => Canvas.GetLeft(item) + (item as FrameworkElement).ActualWidth);
                    //    maxBottomObject = listSelected.MaxElement(item => Canvas.GetTop(item) + (item as FrameworkElement).ActualHeight);
                    //    MainGroupWidth = Canvas.GetLeft(maxRightObject) + (maxRightObject as FrameworkElement).ActualWidth - Canvas.GetLeft(minLefObject);
                    //    MainGroupHeight = Canvas.GetTop(maxBottomObject) + (maxBottomObject as FrameworkElement).ActualHeight - Canvas.GetTop(minTopObject);
                    //}

                    listSelected.ForEach(uie =>
                    {
                        double factor = !bMoveGroup && (uie as FrameworkElement).RenderTransformOrigin != null && !bMoveGroup ? (uie as FrameworkElement).RenderTransformOrigin.Y : 0.5;
                        //double dHeight = (uie as FrameworkElement).ActualHeight * factor;
                        double dHeight = MainGroupHeight * factor;
                        double dTargetTop = refPoint - dHeight;

                        //if (MainSurface is Canvas)
                        //{
                            dTargetTop = Canvas.GetTop(uie) - Canvas.GetTop(minTopObject) + dTargetTop;
                        //}
                        //else
                        //{
                        //    dTargetTop = Canvas.GetTop(uie) - Canvas.GetTop(minTopObject) + dTargetTop;
                        //}


                        DoubleAnimation daTop = new DoubleAnimation()
                        {
                            To = dTargetTop,
                            Duration = new Duration(TimeSpan.FromMilliseconds(nAnimationTime)),
                            EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut }
                        };

                        daTop.Completed += (o, u) =>
                        {
                            elementsToSelect.Add(uie);

                            //if (MainSurface is Canvas)
                            //{
                                uie.BeginAnimation(Canvas.TopProperty, null);
                                Canvas.SetTop(uie, dTargetTop);
                            //}
                            //else
                            //{
                            //    uie.BeginAnimation(Canvas.TopProperty, null);
                            //    Canvas.SetTop(uie, dTargetTop);
                            //}

                            if (listSelected.Count == elementsToSelect.Count)
                            {
                                var readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                                MainSurface_SetCurrentSelection(readonlyList);
                            }
                        };

                        //if (MainSurface is Canvas)
                            uie.BeginAnimation(Canvas.TopProperty, daTop);
                        //else
                        //    uie.BeginAnimation(Canvas.TopProperty, daTop);
                    });
                }
            }
        }
        private void OnAlignLeft(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var listSelected = MainSurface_GetCurrentSelection();
            if (listSelected.Count > 1)
            {
                var elementsToSelect = new List<UIElement>();
                using (new ChangeScreenWatcher(this))
                {
                    MainSurface_CleanCurrentSelectedion();
                    double refPoint = Double.MaxValue;
                    bool emulShift = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) || e.Parameter?.ToString() == "emulShift";
                    double Xfactor = 0;
                    FrameworkElement baseElement = (listSelected[0] as FrameworkElement);
                    if (emulShift)
                        Xfactor = baseElement.RenderTransformOrigin != null && 
                                  baseElement.ReadLocalValue(FrameworkElement.RenderTransformOriginProperty) != DependencyProperty.UnsetValue ? 
                                  (listSelected[0] as FrameworkElement).RenderTransformOrigin.X : 0.5; 

                    //if (MainSurface is Canvas)
                        refPoint = Canvas.GetLeft(listSelected[0]) + listSelected[0].RenderSize.Width * Xfactor;
                    //else
                    //    refPoint = Canvas.GetLeft(listSelected[0]) + listSelected[0].RenderSize.Width * Xfactor;

                    listSelected.ForEach(uie =>
                        {
                            if (emulShift && listSelected[0] == uie)
                            {
                                elementsToSelect.Add(uie);
                            }
                            else
                            {
                                double dWidth = 0;
                                double factor = (uie as FrameworkElement).RenderTransformOrigin != null &&
                                              (uie as FrameworkElement).ReadLocalValue(FrameworkElement.RenderTransformOriginProperty) != DependencyProperty.UnsetValue ? 
                                              (uie as FrameworkElement).RenderTransformOrigin.X : 0.5;
                                if (listSelected[0] != uie && emulShift)
                                    dWidth = (uie as FrameworkElement).ActualWidth * factor;
                                double dTargetLeft = refPoint - dWidth;

                                DoubleAnimation da = new DoubleAnimation()
                                {
                                    To = dTargetLeft,
                                    Duration = new Duration(TimeSpan.FromMilliseconds(nAnimationTime)),
                                    EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut }
                                };

                                da.Completed += (o, u) =>
                                {
                                    {
                                        elementsToSelect.Add(uie);

                                        //if (MainSurface is Canvas)
                                        //{
                                            uie.BeginAnimation(Canvas.LeftProperty, null);
                                            Canvas.SetLeft(uie, dTargetLeft);
                                        //}
                                        //else
                                        //{
                                        //    uie.BeginAnimation(Canvas.LeftProperty, null);
                                        //    Canvas.SetLeft(uie, dTargetLeft);
                                        //}

                                        if (listSelected.Count == elementsToSelect.Count)
                                        {
                                            var readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                                            MainSurface_SetCurrentSelection(readonlyList);
                                        }
                                    }
                                };

                                //if (MainSurface is Canvas)
                                    uie.BeginAnimation(Canvas.LeftProperty, da);
                                //else
                                //    uie.BeginAnimation(Canvas.LeftProperty, da);
                            }
                        });
                }
            }
        }

        private void OnAlignRight(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var listSelected = MainSurface_GetCurrentSelection();
            if (listSelected.Count > 1)
            {
                var elementsToSelect = new List<UIElement>();
                using (new ChangeScreenWatcher(this))
                {
                    MainSurface_CleanCurrentSelectedion();
                    double refPoint = Double.MaxValue;
                    double Xfactor = 1;
                    if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                        Xfactor = (listSelected[0] as FrameworkElement).RenderTransformOrigin != null ? (listSelected[0] as FrameworkElement).RenderTransformOrigin.X : 0.5;

                    //if (MainSurface is Canvas)
                        refPoint = Canvas.GetLeft(listSelected[0]) + listSelected[0].RenderSize.Width * Xfactor;
                    //else
                    //    refPoint = Canvas.GetLeft(listSelected[0]) + listSelected[0].RenderSize.Width * Xfactor;

                    listSelected.ForEach(uie =>
                    {
                        if ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) && listSelected[0] == uie)
                        {
                            elementsToSelect.Add(uie);
                        }
                        else
                        {
                            double factor = (uie as FrameworkElement).RenderTransformOrigin != null ? (uie as FrameworkElement).RenderTransformOrigin.X : 0.5;
                            double dWidth = 0;
                            if (listSelected[0] != uie && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
                                dWidth = (uie as FrameworkElement).ActualWidth - (uie as FrameworkElement).ActualWidth * factor;
                            double dTargetLeft = refPoint + dWidth;

                            DoubleAnimation da = new DoubleAnimation()
                            {
                                To = dTargetLeft - uie.RenderSize.Width,
                                Duration = new Duration(TimeSpan.FromMilliseconds(nAnimationTime)),
                                EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut }
                            };

                            da.Completed += (o, u) =>
                            {
                                {
                                    elementsToSelect.Add(uie);

                                    //if (MainSurface is Canvas)
                                    //{
                                        uie.BeginAnimation(Canvas.LeftProperty, null);
                                        Canvas.SetLeft(uie, dTargetLeft - uie.RenderSize.Width);
                                    //}
                                    //else
                                    //{
                                    //    uie.BeginAnimation(Canvas.LeftProperty, null);
                                    //    Canvas.SetLeft(uie, dTargetLeft - uie.RenderSize.Width);
                                    //}

                                    if (listSelected.Count == elementsToSelect.Count)
                                    {
                                        var readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                                        MainSurface_SetCurrentSelection(readonlyList);
                                    }
                                }
                            };

                            //if (MainSurface is Canvas)
                                uie.BeginAnimation(Canvas.LeftProperty, da);
                            //else
                            //    uie.BeginAnimation(Canvas.LeftProperty, da);
                        }
                    });
                }
            }
        }

        private void OnAlignTop(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var listSelected = MainSurface_GetCurrentSelection();
            if (listSelected.Count > 1)
            {
                var elementsToSelect = new List<UIElement>();
                using (new ChangeScreenWatcher(this))
                {
                    MainSurface_CleanCurrentSelectedion();
                    double refPoint = Double.MaxValue;
                    double Yfactor = 0;
                    bool emulShift = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift) || e.Parameter?.ToString() == "emulShift";
                    FrameworkElement baseElement = (listSelected[0] as FrameworkElement);
                    if (emulShift)
                        Yfactor = baseElement.RenderTransformOrigin != null &&
                                  baseElement.ReadLocalValue(FrameworkElement.RenderTransformOriginProperty) != DependencyProperty.UnsetValue ?
                                  (listSelected[0] as FrameworkElement).RenderTransformOrigin.Y : 0.5;

                    //if (MainSurface is Canvas)
                        refPoint = Canvas.GetTop(listSelected[0]) + listSelected[0].RenderSize.Height * Yfactor;
                    //else
                    //    refPoint = Canvas.GetTop(listSelected[0]) + listSelected[0].RenderSize.Height * Yfactor;

                    listSelected.ForEach(uie =>
                    {
                        if (emulShift && listSelected[0] == uie)
                        {
                            elementsToSelect.Add(uie);
                        }
                        else
                        {
                            double factor = (uie as FrameworkElement).RenderTransformOrigin != null &&
                                              (uie as FrameworkElement).ReadLocalValue(FrameworkElement.RenderTransformOriginProperty) != DependencyProperty.UnsetValue ?
                                               (uie as FrameworkElement).RenderTransformOrigin.Y : 0.5;
                            double dHeight = 0;
                            if (listSelected[0] != uie && emulShift)
                                dHeight = (uie as FrameworkElement).ActualHeight * factor;
                            double dTargetTop = refPoint - dHeight;

                            DoubleAnimation da = new DoubleAnimation()
                            {
                                To = dTargetTop,
                                Duration = new Duration(TimeSpan.FromMilliseconds(nAnimationTime)),
                                EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut }
                            };

                            da.Completed += (o, u) =>
                            {
                                {
                                    elementsToSelect.Add(uie);

                                    //if (MainSurface is Canvas)
                                    //{
                                        uie.BeginAnimation(Canvas.TopProperty, null);
                                        Canvas.SetTop(uie, dTargetTop);
                                    //}
                                    //else
                                    //{
                                    //    uie.BeginAnimation(Canvas.TopProperty, null);
                                    //    Canvas.SetTop(uie, dTargetTop);
                                    //}

                                    if (listSelected.Last() == uie)
                                    {
                                        var readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                                        MainSurface_SetCurrentSelection(readonlyList);
                                    }
                                }
                            };

                            //if (MainSurface is Canvas)
                                uie.BeginAnimation(Canvas.TopProperty, da);
                            //else
                            //    uie.BeginAnimation(Canvas.TopProperty, da);
                    }
                    });
                }
            }
        }

        private void OnAlignBottom(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var listSelected = MainSurface_GetCurrentSelection();
            if (listSelected.Count > 1)
            {
                var elementsToSelect = new List<UIElement>();
                using (new ChangeScreenWatcher(this))
                {
                    MainSurface_CleanCurrentSelectedion();
                    double refPoint = Double.MaxValue;
                    double Yfactor = 1;
                    if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                        Yfactor = (listSelected[0] as FrameworkElement).RenderTransformOrigin != null ? (listSelected[0] as FrameworkElement).RenderTransformOrigin.Y : 0.5;

                    //if (MainSurface is Canvas)
                        refPoint = Canvas.GetTop(listSelected[0]) + listSelected[0].RenderSize.Height * Yfactor;
                    //else
                    //    refPoint = Canvas.GetTop(listSelected[0]) + listSelected[0].RenderSize.Height * Yfactor;

                    listSelected.ForEach(uie =>
                    {
                        if ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) && listSelected[0] == uie)
                        {
                            elementsToSelect.Add(uie);
                        }
                        else
                        {
                            double factor = (uie as FrameworkElement).RenderTransformOrigin != null ? (uie as FrameworkElement).RenderTransformOrigin.Y : 0.5;
                            double dHeight = 0;
                            if (listSelected[0] != uie && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
                                dHeight = (uie as FrameworkElement).ActualHeight - (uie as FrameworkElement).ActualHeight * factor;
                            double dTargetTop = refPoint + dHeight;

                            DoubleAnimation da = new DoubleAnimation()
                            {
                                To = dTargetTop - uie.RenderSize.Height,
                                Duration = new Duration(TimeSpan.FromMilliseconds(nAnimationTime)),
                                EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut }
                            };

                            da.Completed += (o, u) =>
                            {
                                {
                                    elementsToSelect.Add(uie);

                                    //if (MainSurface is Canvas)
                                    //{
                                        uie.BeginAnimation(Canvas.TopProperty, null);
                                        Canvas.SetTop(uie, dTargetTop - uie.RenderSize.Height);
                                    //}
                                    //else
                                    //{
                                    //    uie.BeginAnimation(Canvas.TopProperty, null);
                                    //    Canvas.SetTop(uie, dTargetTop - uie.RenderSize.Height);
                                    //}

                                    if (listSelected.Last() == uie)
                                    {
                                        var readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                                        MainSurface_SetCurrentSelection(readonlyList);
                                    }
                                }
                            };

                            //if (MainSurface is Canvas)
                                uie.BeginAnimation(Canvas.TopProperty, da);
                            //else
                            //    uie.BeginAnimation(Canvas.TopProperty, da);
                        }
                    });
                }
            }
        }

        void TransformsPoints(UIElement control, double factorX, double factorY, bool bAlsoFont = true, bool bAlsoThikness = true)
        {
            if (control is Line)
            {
                var line = control as Line;
                var points = new PointCollection();
                var newPoints = new PointCollection();
                points.Add(new Point(line.X1, line.Y1));
                points.Add(new Point(line.X2, line.Y2));
                foreach (var point in points)
                {
                    newPoints.Add(new Point(point.X * factorX, point.Y * factorY));
                }
                line.X1 = newPoints[0].X;
                line.Y1 = newPoints[0].Y;
                line.X2 = newPoints[1].X;
                line.Y2 = newPoints[1].Y;
            }
            else
            {
                var propertyPoints = control.GetType().GetProperty("Points");
                if (propertyPoints != null)
                {
                    var points = propertyPoints.GetValue(control) as PointCollection;
                    var newPoints = new PointCollection();
                    foreach (var point in points)
                    {
                        newPoints.Add(new Point(point.X * factorX, point.Y * factorY));
                    }
                    propertyPoints.SetValue(control, newPoints);
                }
            }

            if(bAlsoThikness)
            {
                var strokeThickness = control.GetType().GetProperty("StrokeThickness");
                if (strokeThickness != null)
                {
                    var thickness = (double)strokeThickness.GetValue(control);
                    strokeThickness.SetValue(control, thickness * Math.Max(factorX, factorY));
                }
            }

            if (bAlsoFont)
            {
                var p = control.GetType().GetProperty("FontSize");
                if (p != null)
                {
                    var val = (double)p.GetValue(control);
                    p.SetValue(control, Math.Max(val * factorY, 1));
                }
            }
        }

        private void OnSetSameWidth(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var listSelected = MainSurface_GetCurrentSelection();
            if (listSelected.Count > 1)
            {
                var elementsToSelect = new List<UIElement>();
                using (new ChangeScreenWatcher(this))
                {
                    double dValue = (listSelected[0] as FrameworkElement).Width;
                    if (Double.IsNaN(dValue))
                        dValue = (listSelected[0] as FrameworkElement).Width = (listSelected[0] as FrameworkElement).ActualWidth;
                    elementsToSelect.Add(listSelected[0]);
                    listSelected.RemoveAt(0);

                    listSelected.ForEach(uie =>
                    {
                        var factorx = 1.0;
                        if ((uie as FrameworkElement).ActualWidth > 0)
                            factorx = dValue / (uie as FrameworkElement).ActualWidth;

                        DoubleAnimation da = new DoubleAnimation()
                        {
                            To = dValue,
                            Duration = new Duration(TimeSpan.FromMilliseconds(nAnimationTime)),
                            EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut }
                        };

                        da.Completed += (o, u) =>
                        {
                            {
                                uie.BeginAnimation(FrameworkElement.WidthProperty, null);
                                (uie as FrameworkElement).Width = dValue;

                                TransformsPoints(uie, factorx, 1, false);
                                elementsToSelect.Add(uie);

                                if (listSelected.Last() == uie)
                                {
                                    MainSurface_CleanCurrentSelectedion();
                                    var readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                                    MainSurface_SetCurrentSelection(readonlyList);
                                }
                            }
                        };

                        (uie as FrameworkElement).Width = (uie as FrameworkElement).ActualWidth;

                        // (uie as FrameworkElement).Width = dValue;
                        uie.BeginAnimation(FrameworkElement.WidthProperty, da);
                    });
                }
            }
        }

        private void OnSetSameHeight(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var listSelected = MainSurface_GetCurrentSelection();
            if (listSelected.Count > 1)
            {
                var elementsToSelect = new List<UIElement>();
                using (new ChangeScreenWatcher(this))
                {
                    double dValue = (listSelected[0] as FrameworkElement).Height;
                    if (Double.IsNaN(dValue))
                        dValue = (listSelected[0] as FrameworkElement).Height = (listSelected[0] as FrameworkElement).ActualHeight;
                    elementsToSelect.Add(listSelected[0]);
                    listSelected.RemoveAt(0);

                    listSelected.ForEach(uie =>
                    {
                        var factory = 1.0;
                        if ((uie as FrameworkElement).ActualHeight > 0)
                            factory = dValue / (uie as FrameworkElement).ActualHeight;

                        DoubleAnimation da = new DoubleAnimation()
                        {
                            To = dValue,
                            Duration = new Duration(TimeSpan.FromMilliseconds(nAnimationTime)),
                            EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut }
                        };

                        // (uie as FrameworkElement).Height = dValue;
                        da.Completed += (o, u) =>
                        {
                            {
                                uie.BeginAnimation(FrameworkElement.HeightProperty, null);
                                (uie as FrameworkElement).Height = dValue;

                                TransformsPoints(uie, 1, factory, false);
                                elementsToSelect.Add(uie);

                                if (listSelected.Last() == uie)
                                {
                                    MainSurface_CleanCurrentSelectedion();
                                    var readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                                    MainSurface_SetCurrentSelection(readonlyList);
                                }
                            }
                        };

                        (uie as FrameworkElement).Height = (uie as FrameworkElement).ActualHeight;

                        uie.BeginAnimation(FrameworkElement.HeightProperty, da);
                    });
                }
            }
        }

        private void OnSetSameBoth(object sender, ExecutedRoutedEventArgs e)
        {
            if (e != null)
                e.Handled = true;
            var listSelected = MainSurface_GetCurrentSelection();
            if (listSelected.Count > 1)
            {
                var elementsToSelect = new List<UIElement>();
                using (new ChangeScreenWatcher(this))
                {
                    double dWidth = (listSelected[0] as FrameworkElement).Width;
                    if (Double.IsNaN(dWidth))
                        dWidth = (listSelected[0] as FrameworkElement).Width = (listSelected[0] as FrameworkElement).ActualWidth;
                    double dHeight = (listSelected[0] as FrameworkElement).Height;
                    if (Double.IsNaN(dHeight))
                        dHeight = (listSelected[0] as FrameworkElement).Height = (listSelected[0] as FrameworkElement).ActualHeight;
                    elementsToSelect.Add(listSelected[0]);
                    listSelected.RemoveAt(0);
                    listSelected.ForEach(uie =>
                    {
                        var factorx = 1.0;
                        if ((uie as FrameworkElement).ActualWidth > 0)
                            factorx = dWidth / (uie as FrameworkElement).ActualWidth;
                        var factory = 1.0;
                        if ((uie as FrameworkElement).ActualHeight > 0)
                            factory = dHeight / (uie as FrameworkElement).ActualHeight;

                        DoubleAnimation daw = new DoubleAnimation()
                        {
                            To = dWidth,
                            Duration = new Duration(TimeSpan.FromMilliseconds(nAnimationTime)),
                            EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut }
                        };
                        DoubleAnimation dah = new DoubleAnimation()
                        {
                            To = dHeight,
                            Duration = new Duration(TimeSpan.FromMilliseconds(nAnimationTime)),
                            EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut }
                        };

                        dah.Completed += (o, u) =>
                        {
                            {
                                uie.BeginAnimation(FrameworkElement.WidthProperty, null);
                                (uie as FrameworkElement).Width = dWidth;
                                uie.BeginAnimation(FrameworkElement.HeightProperty, null);
                                (uie as FrameworkElement).Height = dHeight;

                                TransformsPoints(uie, factorx, factory, false);
                                elementsToSelect.Add(uie);

                                if (sender != null && listSelected.Last() == uie)
                                {
                                    MainSurface_CleanCurrentSelectedion();
                                    var readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                                    MainSurface_SetCurrentSelection(readonlyList);
                                }
                            }
                        };

                        (uie as FrameworkElement).Width = (uie as FrameworkElement).ActualWidth;
                        (uie as FrameworkElement).Height = (uie as FrameworkElement).ActualHeight;

                        uie.BeginAnimation(FrameworkElement.WidthProperty, daw);
                        uie.BeginAnimation(FrameworkElement.HeightProperty, dah);
                        // (uie as FrameworkElement).Height = dHeight;
                        // (uie as FrameworkElement).Width = Width;
                    });
                }
            }
        }

        private void OnPropertyMapper(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var listSelected = MainSurface_GetCurrentSelection();
            if (listSelected == null || listSelected.Count <= 1)
                return;

            using (new ChangeScreenWatcher(this))
            {
                MainSurface_CleanCurrentSelectedion();

                using (var cursor = new WaitCursor())
                {
                    for (int i = 1; i < listSelected.Count; ++i)
                    {
                        listSelected[i].copyPropertiesFrom(listSelected[0], new String[] { "Name",
                            "ControlTemplate", "ContentTemplate", "Content", "Tag", "Style", "Resources",
                        "ContentTemplateSelector", "Template", "OverridesDefaultStyle", "Triggers",
                        "TemplatedParent", "DataContext", "BindingGroup", "FocusVisualStyle", "RenderTransform",
                        "LayoutTransform", "Parent", "InputBindings", "CommandBindings", "Uid", "PersistId",
                        "Dispatcher", "RenderTransformOrigin" });
                    }
                }
            }

            ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(listSelected);
            MainSurface_SetCurrentSelection(readonlyList);
        }

        private void OnDynamicPropertyMapper(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var listSelected = MainSurface_GetCurrentSelection();
            if (listSelected == null || listSelected.Count <= 1)
                return;

            using (var cursor = new WaitCursor())
            {
                using (new ChangeScreenWatcher(this))
                {
                    for (int i = 1; i < listSelected.Count; ++i)
                    {
                        Document.CopyDynamic(listSelected[0] as FrameworkElement,
                                             listSelected[i] as FrameworkElement, true);
                    }
                }
            }
        }

        void CanExecuteDynamicPropertyMapper(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (Document != null && SelectedElementsMap.Count > 1)
            {
                var listSelected = MainSurface_GetCurrentSelection();
                if (listSelected.Count > 0)
                    e.CanExecute = Document.IsDynamicEntity(listSelected[0] as FrameworkElement);
            }
        }



        private void OnDistributeSpace(object sender, ExecutedRoutedEventArgs e)
        {
            if (e != null)
                e.Handled = true;

            bool applied = false;
            var listSelected = MainSurface_GetCurrentSelection();

            var distributeSpaceDialog = new Popups.SpaceDistribution();
            distributeSpaceDialog.btnApply.Click += (ob, ev) =>
                {
                    if (!applied && listSelected.Count > 0)
                        BeginEdit();

                    DistributeSelection(Convert.ToInt32(distributeSpaceDialog.upDownColumns.Value),
                                        Convert.ToInt32(distributeSpaceDialog.upDownColumnsGap.Value),
                                        Convert.ToInt32(distributeSpaceDialog.upDownRowsGap.Value));
                    // EndEdit();
                    applied = true;
                };

            distributeSpaceDialog.upDownColumns.EditValueChanged += (d, f) => {
                if (applied && listSelected.Count > 0)
                    BeginEdit();
                applied = false; 
            };
            distributeSpaceDialog.upDownColumnsGap.EditValueChanged += (d, f) =>
            {
                if (applied && listSelected.Count > 0)
                    BeginEdit();
                applied = false;
            };
            distributeSpaceDialog.upDownRowsGap.EditValueChanged += (d, f) =>
            {
                if (applied && listSelected.Count > 0)
                    BeginEdit();
                applied = false;
            };
            
            var Dialog = new GeneralDialogContent(distributeSpaceDialog)
            {
                Owner = this.FindParent<Window>(),
                Title = Properties.Resources.DistributeSpace,
                HelpLink = "DistributeSpace"
            };
            if (listSelected.Count > 0)
                BeginEdit();
            if (Dialog.ShowDialog() != true)
            {
                CancelEdit();

                return;
            }
            if (!applied)
                DistributeSelection(Convert.ToInt32(distributeSpaceDialog.upDownColumns.Value),
                    Convert.ToInt32(distributeSpaceDialog.upDownColumnsGap.Value),
                    Convert.ToInt32(distributeSpaceDialog.upDownRowsGap.Value));
            else
                EndEdit();
        }

        void DistributeSelection(int columns, int ColumnsGap, int RowsGap)
        {
            var listSelected = MainSurface_GetCurrentSelection();
            if (listSelected == null || listSelected.Count <= 1)
                return;
            var elementsToSelect = new List<UIElement>();
            MainSurface_CleanCurrentSelectedion();
            // OnSetSameBoth(null, null);

            double dWidth = (listSelected[0] as FrameworkElement).ActualWidth;
            double dHeight = (listSelected[0] as FrameworkElement).ActualHeight;
            double dTop, dLeft, dTargetTop, dTargetLeft, startTop, startLeft;

            startTop = dTargetTop = dTop = Canvas.GetTop(listSelected[0]);
            startLeft = dTargetLeft = dLeft = Canvas.GetLeft(listSelected[0]);

            int actCol = 0;
            int actRow = 0;

            var mapOrdered = new SortedDictionary<int, UIElement>();
            listSelected.ForEach(ch =>
                {
                    int n = ActiveLayer.Children.IndexOf(ch);
                    mapOrdered.Add(n, ch);
                });

            var list = mapOrdered.Keys.ToList();
            double prevWidth = 0;
            double prevHeight = 0;
            double maxHeight = 0;

            list.ForEach(index =>
            {
                var uie = mapOrdered[index];

                DoubleAnimation daLeft = new DoubleAnimation()
                {
                    To = dTargetLeft,
                    Duration = new Duration(TimeSpan.FromMilliseconds(nAnimationTime)),
                    EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut }
                };

                daLeft.Completed += (o, u) =>
                {
                    uie.BeginAnimation(Canvas.LeftProperty, null);
                    Canvas.SetLeft(uie, (double)((o as AnimationClock).Timeline as DoubleAnimation).To);
                };

                DoubleAnimation daTop = new DoubleAnimation()
                {
                    To = dTargetTop,
                    Duration = new Duration(TimeSpan.FromMilliseconds(nAnimationTime)),
                    EasingFunction = new SineEase() { EasingMode = EasingMode.EaseOut }
                };

                daTop.Completed += (o, u) =>
                {
                    elementsToSelect.Add(uie);

                    uie.BeginAnimation(Canvas.TopProperty, null);
                    Canvas.SetTop(uie, (double)((o as AnimationClock).Timeline as DoubleAnimation).To);

                    if (list.Count == elementsToSelect.Count)
                    {
                        var readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                        MainSurface_SetCurrentSelection(readonlyList);

                        EndEdit();
                    }
                };

                uie.BeginAnimation(Canvas.LeftProperty, daLeft);
                uie.BeginAnimation(Canvas.TopProperty, daTop);

                prevWidth = (uie as FrameworkElement).ActualWidth;
                prevHeight = (uie as FrameworkElement).ActualHeight;
                dTargetLeft = dTargetLeft + prevWidth + ColumnsGap;
                
                if (prevHeight > maxHeight)
                    maxHeight = prevHeight;

                if (++actCol >= columns)
                {
                    dTargetLeft = startLeft;
                    ++actRow;
                    dTargetTop += maxHeight + RowsGap;
                    maxHeight = actCol = 0;
                }
            });
        }


        void OnUndoEvent(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                EditorComponent.Workspace.ContextObject = Document;
                undoredo.Undo(1);
                MainSurface_SetModified(true);
            }
        }

        void OnRedoEvent(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (new WaitCursor())
            {
                EditorComponent.Workspace.ContextObject = Document;
                undoredo.Redo(1);
                MainSurface_SetModified(true);
            }
        }

        void CanExecuteUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = undoredo.IsUndoPossible();
        }

        void CanExecuteRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = undoredo.IsRedoPossible();
        }

        private void OnEnableUndoRedo(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            undoredo.IsEnabled = !undoredo.IsEnabled;
            if (!undoredo.IsEnabled)
                GestureResultAdorner.ShowMessage("Undo-Redo is disabled", Mouse.GetPosition(ActiveLayer));
        }

        private List<DependencyPropertyDescriptor> GetCurrentPropertyList(Type t)
        {
            try
            {
                var listP = PropertyChangeNotifier.GetPropertyList(t);
                var listPD = (from c in listP.OfType<PropertyDescriptor>() select DependencyPropertyDescriptor.FromProperty(c)).Where(x => x != null && !x.IsReadOnly && x.PropertyType == typeof(FontSettings)).ToList();
                return listPD;
            }
            catch
            {
                return new List<DependencyPropertyDescriptor>();
            }
        }

        internal void OnFontNameSelectionChanged(FontFamily fontFamily)
        {
            if (fontFamily == null)
                return;

            var listSelected = MainSurface_GetCurrentSelection();
            List<Control> listSControl = (from c in listSelected.OfType<Control>() select c).ToList();
            List<Control> listControl = new List<Control>();
            listSControl.ForEach(item =>
            {
                if (item is ContentControl)
                {
                    if ((item as ContentControl).Content is String)
                        listControl.Add(item);
                    else
                    {
                        if ((item as ContentControl).Content is ContentControl)
                            listControl.Add((item as ContentControl).Content as Control);
                        else
                            listControl.Add(item as Control);
                    }
                }
                else
                    listControl.Add(item);
            });
            if (listControl.Count <= 0)
                return;

            using (new ChangeScreenWatcher(this))
            {
                listControl.ForEach(control =>
                {
                    var value = control.ReadLocalValue(Control.FontWeightProperty);
                    if (!(value is BindingExpression))
                        control.FontFamily = fontFamily;
                });
            }
        }
        
        internal String GetSelectedFontFamilyName()
        {
            var listSelected = MainSurface_GetCurrentSelection();
            List<Control> listSControl = (from c in listSelected.OfType<Control>() select c).ToList();
            string selectedFontFamily = null;
            bool useFont = true;
            listSControl.ForEach(item => 
                {
                    string fontFamilySource = null;
                    if (item is ContentControl)
                    {
                        if ((item as ContentControl).Content is String)
                            fontFamilySource = item.FontFamily.Source;
                        else
                        {
                            if ((item as ContentControl).Content is ContentControl)
                                fontFamilySource = ((item as ContentControl).Content as Control).FontFamily.Source;
                            else
                                fontFamilySource = (item as Control).FontFamily.Source;
                        }
                    }
                    else
                        fontFamilySource = item.FontFamily.Source;

                    if (selectedFontFamily != null && fontFamilySource != selectedFontFamily)
                    {
                        useFont = false;
                        return;
                    }
                    selectedFontFamily = fontFamilySource;
                });
            if (!useFont)
                return String.Empty;

            return selectedFontFamily;
        }

        internal void OnFontSizeSelectionChanged(int fontSize)
        {
            var listSelected = MainSurface_GetCurrentSelection();
            List<Control> listSControl = (from c in listSelected.OfType<Control>() select c).ToList();
            List<Control> listControl = new List<Control>();
            listSControl.ForEach(item =>
            {
                if (item is ContentControl)
                {
                    if ((item as ContentControl).Content is String)
                        listControl.Add(item);
                    else
                    {
                        if ((item as ContentControl).Content is ContentControl)
                            listControl.Add((item as ContentControl).Content as Control);
                        else
                            listControl.Add(item as Control);
                    }
                }
                else
                    listControl.Add(item);
            });
            if (listControl.Count <= 0)
                return;
            
            using (new ChangeScreenWatcher(this))
            {
                listControl.ForEach(control =>
                {
                    var value = control.ReadLocalValue(Control.FontSizeProperty);
                    if (!(value is BindingExpression))
                        control.FontSize = fontSize;
                });
            }
        }

        internal String GetSelectedFontSize()
        {
            var listSelected = MainSurface_GetCurrentSelection();
            List<Control> listSControl = (from c in listSelected.OfType<Control>() select c).ToList();
            string selectedFontSize = null;
            bool useFont = true;
            listSControl.ForEach(item =>
            {
                string fontSize = null;
                if (item is ContentControl)
                {
                    if ((item as ContentControl).Content is String)
                        fontSize = item.FontSize.ToString();
                    else
                    {
                        if ((item as ContentControl).Content is ContentControl)
                            fontSize = ((item as ContentControl).Content as Control).FontSize.ToString();
                        else
                            fontSize = (item as Control).FontSize.ToString();
                    }
                }
                else
                    fontSize = item.FontSize.ToString();

                if (selectedFontSize != null && fontSize != selectedFontSize)
                {
                    useFont = false;
                    return;
                }
                selectedFontSize = fontSize;
            });
            if (!useFont)
                return String.Empty;

            return selectedFontSize;
        }
        PropertyInfo p;
        // FontSettings fs;
        private void OnToggleBold(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var listSelected = MainSurface_GetCurrentSelection();
            var listControl = (from c in listSelected.OfType<Control>() select c).ToList();
            listSelected.ForEach(item => listControl.AddRange(item.GetChildrenOfType<Control>()));
            if (listControl.Count <= 0)
                return;
            using (new ChangeScreenWatcher(this))
            {
                var map = new Dictionary<Control, FontWeight>();
                listControl.ForEach(control =>
                {
                    var value = control.ReadLocalValue(Control.FontWeightProperty);
                    if (!(value is BindingExpression))
                        map.Add(control, control.FontWeight);
                });

                foreach (var control in map.Keys)
                    control.FontWeight = map[control] != FontWeights.Normal ? FontWeights.Normal : FontWeights.Bold;
            }
        }
        void CanToggleBold(object sender, CanExecuteRoutedEventArgs e)
        {
            var listSelected = MainSurface_GetCurrentSelection();
            var listControl = (from c in listSelected.OfType<Control>() select c).ToList();
            listSelected.ForEach(item => listControl.AddRange(item.GetChildrenOfType<Control>()));
            e.CanExecute = listControl.Count > 0;
            if (e.CanExecute)
            { 
                //var parameter = e.Parameter as RibbonButton;
                //if (parameter != null)
                //{
                //    parameter.IsSelected = listControl[0].FontWeight == FontWeights.Bold;
                //}
            }
        }
        private void OnToggleItalic(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var listSelected = MainSurface_GetCurrentSelection();
            var listControl = (from c in listSelected.OfType<Control>() select c).ToList();
            listSelected.ForEach(item => listControl.AddRange(item.GetChildrenOfType<Control>()));
            if (listControl.Count <= 0)
                return;
            using (new ChangeScreenWatcher(this))
            {
                var map = new Dictionary<Control, FontStyle>();
                listControl.ForEach(control =>
                {
                    var value = control.ReadLocalValue(Control.FontStyleProperty);
                    if (!(value is BindingExpression))
                        map.Add(control, control.FontStyle);
                });

                foreach (var control in map.Keys)
                    control.FontStyle = map[control] != FontStyles.Italic ? FontStyles.Italic : FontStyles.Normal;
            }
        }
        void CanToggleItalic(object sender, CanExecuteRoutedEventArgs e)
        {
            var listSelected = MainSurface_GetCurrentSelection();
            var listControl = (from c in listSelected.OfType<Control>() select c).ToList();
            listSelected.ForEach(item => listControl.AddRange(item.GetChildrenOfType<Control>()));
            e.CanExecute = listControl.Count > 0;
            if (e.CanExecute)
            {
                //var parameter = e.Parameter as RibbonButton;
                //if (parameter != null)
                //{
                //    parameter.IsSelected = listControl[0].FontStyle == FontStyles.Italic;
                //}
            }
        }
        private void OnToggleUnderline(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var listSelected = MainSurface_GetCurrentSelection();
            var listTextBlocks = new List<TextBlock>();
            listSelected.ForEach(item => listTextBlocks.AddRange(item.GetVisualChildrenOfType<TextBlock>()));
            listSelected.ForEach(item =>
            {
                if (item is TextBlock && !listTextBlocks.Contains(item))
                    listTextBlocks.Add(item as TextBlock);
            });
            var listTextBoxes = new List<TextBox>();
            listSelected.ForEach(item => listTextBoxes.AddRange(item.GetVisualChildrenOfType<TextBox>()));
            listSelected.ForEach(item =>
            {
                if (item is TextBox && !listTextBoxes.Contains(item))
                    listTextBoxes.Add(item as TextBox);
            });
            if (listTextBlocks.Count <= 0 && listTextBoxes.Count <= 0)
                return;
            using (new ChangeScreenWatcher(this))
            {
                listTextBlocks.ForEach(control =>
                {
                    bool bSetUnderline = false;
                    var parent = control as DependencyObject;
                    while (parent != null && !listSelected.Contains(parent))
                        parent = VisualTreeHelper.GetParent(parent);
                    if (parent is FrameworkElement)
                    {
                        var entity = GetSelectedEntity(parent as FrameworkElement);
                        bSetUnderline = !BitOperations.CheckBitValue((byte)TextDecorators.Underline, entity.TagDecorators);
                    }

                    if (control.TextDecorations != null)
                    {
                        var textDecorations = new TextDecorationCollection(control.TextDecorations);
                        foreach (var item in textDecorations)
                        {
                            if (item.Location == TextDecorationLocation.Underline)
                                control.TextDecorations.Remove(item);
                        }

                        if (bSetUnderline)
                        {
                            foreach (var item in TextDecorations.Underline)
                                control.TextDecorations.Add(item);
                        }
                    }
                });

                listTextBoxes.ForEach(control =>
                {
                    if (control.TextDecorations != null)
                    {
                        bool bSetUnderline = false;
                        var parent = control as DependencyObject;
                        while (parent != null && !listSelected.Contains(parent))
                            parent = VisualTreeHelper.GetParent(parent);
                        if (parent is FrameworkElement)
                        {
                            var entity = GetSelectedEntity(parent as FrameworkElement);
                            bSetUnderline = !BitOperations.CheckBitValue((byte)TextDecorators.Underline, entity.TagDecorators);
                        }

                        if (control.TextDecorations != null)
                        {
                            var textDecorations = new TextDecorationCollection(control.TextDecorations);
                            foreach (var item in textDecorations)
                            {
                                if (item.Location == TextDecorationLocation.Underline)
                                    control.TextDecorations.Remove(item);
                            }

                            if (bSetUnderline)
                            {
                                foreach (var item in TextDecorations.Underline)
                                    control.TextDecorations.Add(item);
                            }
                        }
                    }
                });

                listSelected.ForEach(item =>
                {
                    var entity = GetSelectedEntity(item as FrameworkElement);
                    bool isUnderline = BitOperations.CheckBitValue((byte)TextDecorators.Underline, entity.TagDecorators);
                    entity.TagDecorators = BitOperations.SetBitValue((byte)TextDecorators.Underline, entity.TagDecorators, !isUnderline);
                });
            }

            listSelected.ForEach(item =>
                {
                    item.InvalidateArrange();
                    item.InvalidateMeasure();
                });
        }
        void CanToggleUnderline(object sender, CanExecuteRoutedEventArgs e)
        {
            var listSelected = MainSurface_GetCurrentSelection();
            var listTextBlocks = new List<TextBlock>();
            listSelected.ForEach(item => listTextBlocks.AddRange(item.GetVisualChildrenOfType<TextBlock>()));
            listSelected.ForEach(item =>
            {
                if (item is TextBlock && !listTextBlocks.Contains(item))
                    listTextBlocks.Add(item as TextBlock);
            });

            var listTextBoxes = new List<TextBox>();
            listSelected.ForEach(item => listTextBoxes.AddRange(item.GetVisualChildrenOfType<TextBox>()));
            listSelected.ForEach(item =>
            {
                if (item is TextBox && !listTextBoxes.Contains(item))
                    listTextBoxes.Add(item as TextBox);
            });

            e.CanExecute = listTextBlocks.Count > 0 || listTextBoxes.Count > 0;
            if (e.CanExecute)
            {
                //var parameter = e.Parameter as RibbonButton;
                //if (parameter != null)
                //{
                //    parameter.IsSelected = false;
                //    if (listTextBlocks.Count > 0)
                //    {
                //        foreach (var item in TextDecorations.Underline)
                //        {
                //            if (listTextBlocks[0].TextDecorations.Contains(item))
                //            {
                //                parameter.IsSelected = true;
                //                break;
                //            }
                //        }
                //    }
                //    else if (listTextBoxes.Count > 0)
                //    {
                //        foreach (var item in TextDecorations.Underline)
                //        {
                //            if (listTextBoxes[0].TextDecorations.Contains(item))
                //            {
                //                parameter.IsSelected = true;
                //                break;
                //            }
                //        }
                //    }
                //}
            }
        }
        private void OnIncreaseFontSize(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var listSelected = MainSurface_GetCurrentSelection();
            var listControl = (from c in listSelected.OfType<Control>() select c).ToList();
            listSelected.ForEach(item => listControl.AddRange(item.GetChildrenOfType<Control>()));
            if (listControl.Count <= 0)
                return;
            using (new ChangeScreenWatcher(this))
            {
                listControl.ForEach(control =>
                {
                    var value = control.ReadLocalValue(Control.FontWeightProperty);
                    if (!(value is BindingExpression))
                        control.FontSize += 1;

                    //if (!(control is HeaderedContentControl) &&
                    //    control is ContentControl && !((control as ContentControl).Content is String))
                    //{
                    //    var t = control.GetType();
                    //    var listProperty = GetCurrentPropertyList(t);
                    //    if (listProperty.Count > 0)
                    //    {
                    //        listProperty.ForEach(X =>
                    //        {
                    //            p = t.GetProperty(X.Name);
                    //            fs = (p.GetValue((UIElement)control) as FontSettings).Clone();
                    //            fs.FontSize += 1;
                    //            p.SetValue((UIElement)control, fs);
                    //        });
                    //    }

                    //}
                });
            }
        }
        void CanTexHorAlign(object sender, CanExecuteRoutedEventArgs e)
        {
            var listSelected = MainSurface_GetCurrentSelection();
            var listControl = (from c in listSelected.OfType<Control>() select c).ToList();
            listSelected.ForEach(item => listControl.AddRange(item.GetChildrenOfType<Control>()));
            e.CanExecute = listControl.Count > 0;
        }

        void CanIncreaseFontSize(object sender, CanExecuteRoutedEventArgs e)
        {
            var listSelected = MainSurface_GetCurrentSelection();
            var listControl = (from c in listSelected.OfType<Control>() select c).ToList();
            listSelected.ForEach(item => listControl.AddRange(item.GetChildrenOfType<Control>()));
            e.CanExecute = listControl.Count > 0;
        }
        private void OnTexHorAlignLeft(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var listSelected = MainSurface_GetCurrentSelection();
            var listControl = (from c in listSelected.OfType<Control>() select c).ToList();
            listSelected.ForEach(item => listControl.AddRange(item.GetChildrenOfType<Control>()));
            if (listControl.Count <= 0)
                return;
            using (new ChangeScreenWatcher(this))
            {
                foreach (var selectedcontrol in listControl)
                {
                    Control control = selectedcontrol;
                    if (selectedcontrol is ContentControl && (selectedcontrol as ContentControl).Content is Control && !(selectedcontrol is UserControl))
                        control = (selectedcontrol as ContentControl).Content as Control;
                    var descriptor = DependencyPropertyDescriptor.FromName("TextAlignment", control.GetType(), control.GetType());
                    if (descriptor != null)
                    {
                        descriptor.SetValue(control, TextAlignment.Left);
                        //continue;
                    }

                    var value = control.ReadLocalValue(Control.HorizontalContentAlignmentProperty);
                    if (!(value is BindingExpression))
                    {
                        control.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left;
                        //UIElement ui = control as UIElement;
                        //if (ui is TextBox)
                        //    (ui as TextBox).TextAlignment = TextAlignment.Left;
                        //if (ui is TextBlock)
                        //    (ui as TextBlock).TextAlignment = TextAlignment.Left;
                    }
                }
            }
        }

        private void OnTexHorAlignJustify(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var listSelected = MainSurface_GetCurrentSelection();
            var listControl = (from c in listSelected.OfType<Control>() select c).ToList();
            listSelected.ForEach(item => listControl.AddRange(item.GetChildrenOfType<Control>()));
            if (listControl.Count <= 0)
                return;
            using (new ChangeScreenWatcher(this))
            {
                foreach (var selectedcontrol in listControl)
                {
                    Control control = selectedcontrol;
                    if (selectedcontrol is ContentControl && (selectedcontrol as ContentControl).Content is Control && !(selectedcontrol is UserControl))
                        control = (selectedcontrol as ContentControl).Content as Control;
                    var descriptor = DependencyPropertyDescriptor.FromName("TextAlignment", control.GetType(), control.GetType());
                    if (descriptor != null)
                    {
                        descriptor.SetValue(control, TextAlignment.Justify);
                        //continue;
                    }

                    var value = control.ReadLocalValue(Control.HorizontalContentAlignmentProperty);
                    if (!(value is BindingExpression))
                    {
                        control.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
                        //UIElement ui = control as UIElement;
                        //if (ui is TextBox)
                        //    (ui as TextBox).TextAlignment = TextAlignment.Justify;
                        //if (ui is TextBlock)
                        //    (ui as TextBlock).TextAlignment = TextAlignment.Justify;
                    }
                }
            }
        }
        private void OnTexHorAlignCenter(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var listSelected = MainSurface_GetCurrentSelection();
            var listControl = (from c in listSelected.OfType<Control>() select c).ToList();
            listSelected.ForEach(item => listControl.AddRange(item.GetChildrenOfType<Control>()));
            if (listControl.Count <= 0)
                return;
            using (new ChangeScreenWatcher(this))
            {
                foreach (var selectedcontrol in listControl)
                {
                    Control control = selectedcontrol;
                    if (selectedcontrol is ContentControl && (selectedcontrol as ContentControl).Content is Control && !(selectedcontrol is UserControl))
                        control = (selectedcontrol as ContentControl).Content as Control;
                    var descriptor = DependencyPropertyDescriptor.FromName("TextAlignment", control.GetType(), control.GetType());
                    if (descriptor != null)
                    {
                        descriptor.SetValue(control, TextAlignment.Center);
                        //continue;
                    }

                    var value = control.ReadLocalValue(Control.HorizontalContentAlignmentProperty);
                    if (!(value is BindingExpression))
                    {
                        control.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                        //UIElement ui = control as UIElement;
                        //if (ui is TextBox)
                        //    (ui as TextBox).TextAlignment = TextAlignment.Center;
                        //if (ui is TextBlock)
                        //    (ui as TextBlock).TextAlignment = TextAlignment.Center;
                    }
                }
            }
        }
        private void OnTexHorAlignRight(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var listSelected = MainSurface_GetCurrentSelection();
            var listControl = (from c in listSelected.OfType<Control>() select c).ToList();
            listSelected.ForEach(item => listControl.AddRange(item.GetChildrenOfType<Control>()));
            if (listControl.Count <= 0)
                return;
            using (new ChangeScreenWatcher(this))
            {
                foreach (var selectedcontrol in listControl)
                {
                    Control control = selectedcontrol;
                    if (selectedcontrol is ContentControl && (selectedcontrol as ContentControl).Content is Control && !(selectedcontrol is UserControl))
                        control = (selectedcontrol as ContentControl).Content as Control;
                    var descriptor = DependencyPropertyDescriptor.FromName("TextAlignment", control.GetType(), control.GetType());
                    if (descriptor != null)
                    {
                        descriptor.SetValue(control, TextAlignment.Right);
                        //continue;
                    }

                    var value = control.ReadLocalValue(Control.HorizontalContentAlignmentProperty);
                    if (!(value is BindingExpression))
                    {
                        
                        control.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
                        //UIElement ui = control as UIElement;
                        //if (ui is TextBox)
                        //    (ui as TextBox).TextAlignment = TextAlignment.Right;
                        //if (ui is TextBlock)
                        //    (ui as TextBlock).TextAlignment = TextAlignment.Right;
                    }
                }
            }
        }

        private void OnDecreaseFontSize(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var listSelected = MainSurface_GetCurrentSelection();
            var listControl = (from c in listSelected.OfType<Control>() select c).ToList();
            listSelected.ForEach(item => listControl.AddRange(item.GetChildrenOfType<Control>()));
            if (listControl.Count <= 0)
                return;
            using (new ChangeScreenWatcher(this))
            {
                listControl.ForEach(control =>
                {
                    var value = control.ReadLocalValue(Control.FontWeightProperty);
                    if (!(value is BindingExpression))
                        control.FontSize -= 1;

                    //if (!(control is HeaderedContentControl) &&
                    //    control is ContentControl && !((control as ContentControl).Content is String))
                    //{
                    //    var t = control.GetType();
                    //    var listProperty = GetCurrentPropertyList(t);
                    //    if (listProperty.Count > 0)
                    //    {
                    //        listProperty.ForEach(X =>
                    //        {
                    //            p = t.GetProperty(X.Name);
                    //            fs = (p.GetValue((UIElement)control) as FontSettings).Clone();
                    //            fs.FontSize -= 1;
                    //            p.SetValue((UIElement)control, fs);
                    //        });
                    //    }

                    //}
                });
            }
        }
        void CanDecreaseFontSize(object sender, CanExecuteRoutedEventArgs e)
        {
            var listSelected = MainSurface_GetCurrentSelection();
            var listControl = (from c in listSelected.OfType<Control>() select c).ToList();
            listSelected.ForEach(item => listControl.AddRange(item.GetChildrenOfType<Control>()));
            e.CanExecute = listControl.Count > 0;
        }

        private void OnMoveUp(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            MoveSelection(-1, 0);
        }

        private void OnMoveDown(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            MoveSelection(1, 0);
        }

        private void OnMoveLeft(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            MoveSelection(0, -1);
        }

        private void OnMoveRight(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            MoveSelection(0, 1);
        }

        void MoveSelection(double top, double left)
        {
            using (new ChangeScreenWatcher(this))
            {
                var listSelected = MainSurface_GetCurrentSelection();
                var listUpdated = new List<UIElement>(listSelected);

                foreach (KeyValuePair<UIElement, BasicAdorner> data in SelectedElementsMap)
                {
                    if (data.Value is BasicPointAdorner)
                        listUpdated.Remove(data.Key);
                }

                var readonlyList = new ReadOnlyCollection<UIElement>(listUpdated);
                MainSurface_UpdateCurrentSelection(readonlyList);
                // MainSurface_CleanCurrentSelectedion();

                listSelected.ForEach(uie =>
                {
                    //if (MainSurface is Canvas)
                    //{
                        Canvas.SetTop(uie, Canvas.GetTop(uie) + top);
                        Canvas.SetLeft(uie, Canvas.GetLeft(uie) + left);
                    //}
                    //else
                    //{
                    //    Canvas.SetTop(uie, Canvas.GetTop(uie) + top);
                    //    Canvas.SetLeft(uie, Canvas.GetLeft(uie) + left);
                    //}
                });

                SelectedElementsMap.Values.ToList().ForEach(adorner =>
                    {
                        adorner.InvalidateMeasure();
                        adorner.InvalidateArrange();
                    });
                var readonlyListtot = new ReadOnlyCollection<UIElement>(listSelected);
                MainSurface_UpdateCurrentSelection(readonlyListtot);
                //ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(listSelected);
                //MainSurface_SetCurrentSelection(readonlyList);
            }
        }

        //Dictionary<FrameworkElement, Popup> parentPopupMap = new Dictionary<FrameworkElement, Popup>();
        //bool bCancelPopupEditor;

        void HidePopupEditor(FrameworkElement owner)
        {
            //if (parentPopupMap.ContainsKey(owner))
            //{
            //    parentPopupMap[owner].IsOpen = false;
            //}
        }

        void OnShowPopupEditor(UIElement parent, UserControl control, FrameworkElement owner, bool bKeepContent = false, 
            String title = null, bool bShowOk = true, string helplink = null)
        {
            var listSelected = new List<UIElement>();
            listSelected.Add(owner);
            var readonlyList = new ReadOnlyCollection<UIElement>(listSelected);
            MainSurface_SetCurrentSelection(readonlyList);

            var name = owner.Name;
            if (String.IsNullOrEmpty(name))
            {
                name = owner.Uid as String;
            }

            var Dialog = new GeneralDialogContent(control, bShowOk ? GeneralDialogButtons.OkCancelHelpButtons : GeneralDialogButtons.CloseHelpButtons)
            {
                Owner = this.FindParent<Window>(),
                DialogKeepContent = bKeepContent,
                Title = String.Format("{0} - {1}", title, name),
                HelpLink = helplink
            };
            // BeginEdit();
            bool bCanceled = false;
            if (Dialog.ShowDialog() != true)
            {
                CancelEdit();
                return;
            }

            //if (EditorComponent.PropertyControl != null)
            //    EditorComponent.PropertyControl.UpdateControlSelection();

            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    EndEdit();
                });
            //if (parentPopupMap.ContainsKey(owner))
            //{
            //    parentPopupMap[owner].IsOpen = false;
            //    return;
            //}

            //List<UIElement> elementsToSelect = MainSurface_GetCurrentSelection();
            //if (elementsToSelect.Count == 0)
            //    return;

            //Popup parentPopup = new Popup();
            //var effect = new DropShadowEffect
            //{
            //    ShadowDepth = 10,
            //    Opacity = 0.5
            //};
            //control.Effect = effect;
            //Popup.CreateRootPopup(parentPopup, control);

            //control.ClipToBounds = false;
            //parentPopup.ClipToBounds = false;
            //parentPopup.AllowsTransparency = true;
            //parentPopup.Placement = PlacementMode.Left;
            //parentPopup.PopupAnimation = PopupAnimation.Slide;
            //parentPopup.StaysOpen = true;
            //parentPopup.Focusable = true;
            //parentPopup.PlacementTarget = owner;
            //parentPopup.Opened += (o, i) =>
            //    {
            //        // BeginEdit();
            //    };
            //parentPopup.Closed += (o, i) =>
            //    {
            //        parent.Rotate(0, 100, false, false, new BackEase() { EasingMode = EasingMode.EaseOut }, false,
            //            (u, a) =>
            //            {
            //                parentPopupMap.Remove(owner);
            //            });

            //        //if (bCancelPopupEditor)
            //        //{
            //        //    bCancelPopupEditor = false;
            //        //    CancelEdit();
            //        //}
            //        //else
            //        //    EndEdit();
            //    };
            //parent.Rotate(180, 100, false, false, new BackEase() { EasingMode = EasingMode.EaseOut }, false,
            //    (o, i) =>
            //    {
            //        parentPopupMap.Add(owner, parentPopup);
            //        parentPopup.IsOpen = true;
            //    });
        }

        ScreenManager.Popups.SmartProperties smartPropertiesControl;
        void OnShowInlineProperties(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            BeginEdit();

            if (e.Parameter != null && e.Parameter is UIElement)
            {
                var elementsToSelect = new List<UIElement>();
                elementsToSelect.Add(e.Parameter as UIElement);
                var readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                MainSurface_SetCurrentSelection(readonlyList);
            }

            // var control = EditorComponent.PropertyControl.control;
            var parent = e.OriginalSource as UIElement;
            // Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, parent as FrameworkElement, true, false);
            List<UIElement> selectedElements = MainSurface_GetCurrentSelection();
            var selectedel = selectedElements[0] as FrameworkElement;
            RestoreUntranslated(selectedel as FrameworkElement, false);
            var entity = GetSelectedEntity(selectedel);

            if (listActive3dModels.Count > 0)
            {
                var model = listActive3dModels.First();
                var hash = Utilities.WPF.DependencyObjectExtensions.RegisterModel3D(MainSurface, selectedel, model);
                entity = entity.AddOrFind3DInnerModel(model, hash);
                entity.Entity3D = model;
            }
            if (smartPropertiesControl == null)
                smartPropertiesControl = new ScreenManager.Popups.SmartProperties(EditorComponent, entity,
                                    selectedel, Document);

            int selectedTab;
            int.TryParse(e.Parameter?.ToString(), out selectedTab);

            bool bForceChanged = !Object.ReferenceEquals(smartPropertiesControl.Entity, entity) || smartPropertiesControl.SelectedTab != selectedTab;
            smartPropertiesControl.Entity = entity;
            smartPropertiesControl.SelectedTab = selectedTab;
            smartPropertiesControl.Element = selectedel;
            smartPropertiesControl.DataContext = selectedel;
            if (Object.ReferenceEquals(smartPropertiesControl.DataContext, selectedel) && bForceChanged ||
                !Object.ReferenceEquals(smartPropertiesControl.DataContext, selectedel))
            {
                smartPropertiesControl.DataContext = null;
                smartPropertiesControl.DataContext = selectedel;
            }
            // bCancelPopupEditor = e.Parameter != null;
            var title = Properties.Resources.PropertiesEditor;
            if (entity.OpcuaEntityReference != null && entity.OpcuaEntityReference.IsValid)
                title = String.Format(Properties.Resources.PropertiesEditorEntity, entity.OpcuaEntityReference.HumanReadable);
            OnShowPopupEditor(parent, smartPropertiesControl, selectedel, true, title, helplink:"ShowInlineProperties");

            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                EditorComponent.Workspace.ForceRefreshCurrentContents();
            });
        }

        void OnShowInlineCommands(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            ActivateCommandExplorer();
        }

        private void CanExecuteShowInlineCommands(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = EditorComponent.CommandExplorer != null && Document != null && SelectedElementsMap.Count > 0;
        }

        void OnShowInlineAnimations(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            ActivateAnimationExplorer();
        }

        private void CanExecuteShowInlineAnimations(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = EditorComponent.AnimationExplorer != null && Document != null && SelectedElementsMap.Count > 0;
        }


        void OnEditDataContextItem(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;

            if (e.Parameter != null && e.Parameter is UIElement)
            {
                var elementsToSelect = new List<UIElement>();
                elementsToSelect.Add(e.Parameter as UIElement);
                var readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                MainSurface_SetCurrentSelection(readonlyList);
            }

            // var control = EditorComponent.PropertyControl.control;
            var parent = e.OriginalSource as UIElement;
            // Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, parent as FrameworkElement);
            List<UIElement> selectedElements = MainSurface_GetCurrentSelection();
            var selectedel = selectedElements[0] as FrameworkElement;
            var entity = GetSelectedEntity(selectedel);

            var title = selectedel.GetType().Name;

            var name = (selectedel as FrameworkElement).Name;
            if (String.IsNullOrEmpty(name))
                name = selectedel.Uid as String;
            if (!String.IsNullOrEmpty(name))
                title = name;

            if (!Keyboard.IsKeyDown(Key.LeftAlt) && !Keyboard.IsKeyDown(Key.RightAlt))
            {
                if (entity.OpcuaEntityReference == null)
                    entity.OpcuaEntityReference = new OPCUAEntityReference();
                BeginEdit();
                if (entity.OpcuaEntityReference.Edit(title: title, sync: true))
                {
                    EndEdit();
                    MainSurface_SetModified(true);
                }
                else
                    CancelEdit();
            }
            else
            {
                if (entity.OpcuaEntityReference != null)
                {
                    using (new ChangeScreenWatcher(this))
                    {
                        entity.OpcuaEntityReference = null;
                    }
                }
            }
        }

        void OnEditDataTypeItem(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            // var control = EditorComponent.PropertyControl.control;
            //var parent = e.OriginalSource as UIElement;
            //Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, parent as FrameworkElement);
            //var arg = new GetFriendObjectsEventArgs();
            //List<UIElement> selectedElements = MainSurface_GetCurrentSelection();
            //var selectedel = selectedElements[0] as FrameworkElement;
            //Document.GetFriendObjects(selectedel, arg);

            //var entity = arg.friendList[0] as ScreenEntity;
            //if (entity.OpcuaDataTypeEntityReference == null)
            //{
            //    entity.OpcuaDataTypeEntityReference = new OPCUAEntityReference();
            //    entity.OpcuaDataTypeEntityReference.IsTypeDefinition = true;
            //}
            //BeginEdit();
            //if (entity.OpcuaDataTypeEntityReference.Edit())
            //    EndEdit();
            //else
            //    CancelEdit();

            if (e.Parameter != null && e.Parameter is UIElement)
            {
                var elementsToSelect = new List<UIElement>();
                elementsToSelect.Add(e.Parameter as UIElement);
                var readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                MainSurface_SetCurrentSelection(readonlyList);
            }

            var parent = e.OriginalSource as UIElement;
            // Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, parent as FrameworkElement, true, true);
            List<UIElement> selectedElements = MainSurface_GetCurrentSelection();
            var selectedel = selectedElements[0] as FrameworkElement;
            var control = new Popups.TypeDefinitionSummary(selectedel);
            var list = new List<OPCUAEntityReference>();
            Document.GetListInners(selectedel.Name).ForEach(I =>
            {
                UIElement uie = Document.FindInnerControl(MainSurface, I);
                if (uie == null)
                    return;

                var entity = Document.MapScreenEntities[I];
                entity.Entity = uie;
                entity.Document = Document;
            });

            var map = Document.GetMapItemsToBeResolved(selectedel, selectedel.Name, true);
            foreach (var el in map.Keys)
            {
                Parallel.ForEach(map[el], item => { item.Name = el; });
                list.AddRange(map[el]);
            }
            control.gridDataControl.ItemsSource = list;
            OnShowPopupEditor(parent, control, selectedel, title:Properties.Resources.DataTypeEditor, helplink:"EditDataType");
        }

        private void OnAddStringId(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            var list = new List<String>();
            var selectedElements = MainSurface_GetCurrentSelection();
            if (selectedElements.Count > 0)
                list = GetSelectedTranslatableList(selectedElements);
            else
                list = GetTranslatableList();
            if (list.Count > 0)
                EditorComponent.StringEditor.AddListStringId(Document, list);
        }

        List<string> GetSelectedTranslatableList(List<UIElement> selectedElements)
        {
            var list = new List<string>();
            var listSingleElements = new List<FrameworkElement>();
            var listMultipleElements = new List<ItemsControl>();

            selectedElements.ForEach(element =>
            {
                var fe = element as FrameworkElement;
                string name = string.IsNullOrEmpty(fe.Name) ? fe.Uid : fe.Name;
                if(!string.IsNullOrEmpty(name))
                {
                    var innerslist = Document.GetListInners(name);
                    if(innerslist.Count > 0)
                    {
                       var inners = (from control in fe.GetChildrenOfType<Control>()
                                     let contentControl = (control as ContentControl)
                                     where control is TextBox || control is ComboBox || control is GroupBox || control is ListBox ||
                                           contentControl != null && (contentControl.Content is string ||
                                           (!(contentControl.Content is string) && !(control is UserControl))) ||
                                           control.ToolTip is string ||
                                           control is IStringIDAware
                                     select control as UIElement).ToList();
                       list.AddRange(GetSelectedTranslatableList(inners));
                    }
                }
                if (element is ContentControl)
                {
                    var control = element as ContentControl;
                    if (control.Content is string || control.ToolTip is string || control is IStringIDAware)
                        listSingleElements.Add(element as FrameworkElement);
                    //else
                    if(!(control.Content is string) && !(element is UserControl))
                    {
                        var listControls = element.GetChildrenOfType<ContentControl>();
                        listSingleElements.AddRange(listControls);
                    }
                }
                else if (element is TextBox || element is GroupBox)
                    listSingleElements.Add(element as FrameworkElement);
                else if (element is ListBox || element is ComboBox)
                {
                    var entity = GetSelectedEntity(fe);
                    if (entity != null && entity.IsItemSourceEntity())
                    {
                        (element as ItemsControl).ItemsSource = entity.GetItemSources();
                    }

                    listMultipleElements.Add(element as ItemsControl);
                }
                else if ((fe.ToolTip is string && 
                        !string.IsNullOrEmpty(fe.ToolTip?.ToString()))
                        || (element is Control && (element as Control) is IStringIDAware))
                {
                    listSingleElements.Add(element as FrameworkElement);
                }
            });
            foreach (var control in listSingleElements)
                GetTranslatableUntranslatedText(ref list, control);
            foreach (var control in listMultipleElements)
                for (var i = 0; i < (control as ItemsControl).Items.Count; i++)
                    GetTranslatableUntranslatedText(ref list, control, i);

            return list;
        }


        void GetTranslatableUntranslatedText(ref List<string> list, FrameworkElement control, int itemIndex = 0)
        {
            List<string> textList = new List<string>();
            bool bUntranslated = false;
            if (langCombo.SelectedIndex != 0)
            {
                bUntranslated = true;
                RestoreUntranslated(control);
            }
            var typeSwitch = new Dictionary<Type, Func<FrameworkElement, int, List<string>>> {
                { typeof(GroupBox), (fe, index) => 
                {
                    GroupBox fecontrol = (GroupBox)fe;
                    List<string> tlist = new List<string>();
                    if(!string.IsNullOrEmpty(fecontrol.Header as String ))
                        tlist.Add(fecontrol.Header as String);
                    if(!string.IsNullOrEmpty(fecontrol.ToolTip as String ))
                        tlist.Add(fecontrol.ToolTip as String);
                    return tlist; 
                } },
                { typeof(ComboBox), (fe, index) => 
                { 
                    ComboBox fecontrol = (ComboBox)fe;
                    var item = fecontrol.Items[index];
                    string txt = null;
                    var propertyInfo = item.GetType().GetProperty("OptionContent");
                    if (propertyInfo != null)
                        txt = propertyInfo.GetValue(item, null).ToString();
                    else
                        txt = item as String;
                    List<string> tlist = new List<string>();
                    if(!string.IsNullOrEmpty(txt))
                        tlist.Add(txt);
                    if(!string.IsNullOrEmpty(fecontrol.ToolTip as String ))
                        tlist.Add(fecontrol.ToolTip as String);
                    return tlist;
                } },
                { typeof(TextBox), (fe, index) =>
                {
                    TextBox fecontrol = (TextBox)fe;
                    List<string> tlist = new List<string>();
                    if(!string.IsNullOrEmpty(fecontrol.Text))
                        tlist.Add(fecontrol.Text);
                    if(!string.IsNullOrEmpty(fecontrol.ToolTip as String ))
                        tlist.Add(fecontrol.ToolTip as String);
                    return tlist;
                } },
                { typeof(ListBox), (fe, index) => {
                    var listBox = (ListBox)fe;
                    var item = listBox.Items[index];
                    string txt = null;
                    var propertyInfo = item.GetType().GetProperty("OptionContent");
                    if (propertyInfo != null)
                        txt = propertyInfo.GetValue(item, null).ToString();
                    else
                        txt = item as String;

                    List<string> tlist = new List<string>();
                    if(!string.IsNullOrEmpty(txt))
                        tlist.Add(txt);
                    if(!string.IsNullOrEmpty(listBox.ToolTip as String ))
                        tlist.Add(listBox.ToolTip as String);
                    return tlist;
                } },
                { typeof(ContentControl), (fe, index) =>
                {
                    ContentControl fecontrol = (ContentControl)fe;
                    List<string> tlist = new List<string>();
                    if(!string.IsNullOrEmpty(fecontrol.Content as String ))
                        tlist.Add(fecontrol.Content as String);
                    if(!string.IsNullOrEmpty(fecontrol.ToolTip as String ))
                        tlist.Add(fecontrol.ToolTip as String);
                    if(fecontrol is IStringIDAware)
                        tlist.AddRange((fecontrol as IStringIDAware).GetStringIDs());
                    return tlist;
                } },
                { typeof(Control), (fe, index) =>
                {
                    Control fecontrol = (Control)fe;
                    List<string> tlist = new List<string>();
                    if(!string.IsNullOrEmpty(fecontrol.ToolTip as String ))
                        tlist.Add(fecontrol.ToolTip as String);
                    if(fecontrol is IStringIDAware)
                        tlist.AddRange((fecontrol as IStringIDAware).GetStringIDs());
                    return tlist;
                } }
            };

            foreach (var type in typeSwitch.Keys)
                if (type.IsAssignableFrom(control.GetType()))
                {
                    textList = typeSwitch[type](control, itemIndex);
                    break;
                }
                else if(control.ToolTip is string && !string.IsNullOrEmpty(control.ToolTip?.ToString()))
                {
                    textList = new List<string>() { control.ToolTip.ToString() };
                }

            if (bUntranslated)
                ElementToCurrentLanguage(control);

            list.AddRange(textList);
        }

        List<string> GetTranslatableList()
        {
            List<UIElement> elements = new List<UIElement>();
            foreach (FrameworkElement child in ActiveLayer.Children)
            {
                string key = string.IsNullOrEmpty(child?.Name) ? child.Uid : child.Name;
                if (Document.MapScreenEntities.ContainsKey(key))
                    elements.Add(child);
            }
            var list = GetSelectedTranslatableList(elements);
            return list;
        }

        private void CanAddStringId(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = bIsActive && EditorComponent.StringEditor != null;
        }

        bool ManageImport3DImageBaseUri(String xmlString, string path, out string text)
        {
            text = xmlString;
            Dictionary<string, string> imageCopy = new Dictionary<string, string>();
            XmlDocument document = new XmlDocument();
            using (XmlReader reader = XmlReader.Create(new StringReader(xmlString), new XmlReaderSettings()
            { DtdProcessing = System.Xml.DtdProcessing.Prohibit, ValidationType = ValidationType.None }))
            {
                var dropHelper = new DropFileHelper(Document, uriToUriAbsoluteImageConverter, EditorComponent.UIInterface, SourceFileCopyOption.Ask);
                document.Load(reader);
                var listImageBrush = document.GetElementsByTagName("ImageBrush");
                foreach(var imageBrush in listImageBrush)
                {
                    var imageBrushNode = imageBrush as XmlNode;
                    string imagePath = imageBrushNode.Attributes.GetNamedItem("ImageSource")?.Value;
                    string newPath = string.Empty;
                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        if (imageCopy.ContainsKey(imagePath))
                            continue;
                        else
                        {
                            try
                            {
                                newPath = dropHelper.DropFile(imagePath, path);
                            }
                            catch (Exception)
                            {
                            }
                            if (!string.IsNullOrEmpty(newPath) && !imageCopy.ContainsKey(imagePath))
                                imageCopy.Add(imagePath, newPath);
                        }
                    }
                }
            }

            foreach (var key in imageCopy.Keys)
            {
                text = text.Replace(key, imageCopy[key]);
            }

            return imageCopy.Count > 0;
        }

        private void OnImportXaml(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            String fileName = EditorComponent.UIInterface.ShowOpenFileDialog(Properties.Settings.Default.ImportFileFilter);
            if (String.IsNullOrEmpty(fileName))
                return;

            String text = null;
            String problematicXaml = null;
            List<String> listAssemblies = null;
            FrameworkElement control = null;
            string fileExtension = System.IO.Path.GetExtension(fileName);
            if (String.Compare(fileExtension, ".dll", true) == 0)
            {
                using (var cursor = new WaitCursor())
                {
                    try
                    {
                        var types = Assembly.LoadFile(fileName).GetTypes();
                        var controls = (from t in types
                                        where !t.IsAbstract && typeof(FrameworkElement).IsAssignableFrom(t)
                                        select t).ToList();
                        if (controls.Count == 0)
                            EditorComponent.UIInterface.ShowError(Properties.Resources.NoUserControlFound);
                        else if (controls.Count > 1)
                        {
                            var listBox = new ListBox() { ItemsSource = controls, Width = 300, Height = 300 };

                            var Dialog = new GeneralDialogContent(listBox)
                            {
                                Owner = this.FindParent<Window>(),
                                Title = Properties.Resources.SelectUserControl,
                                HelpLink = "ImportXaml"
                            };
                            cursor.Release();
                            if (Dialog.ShowDialog() != true || listBox.SelectedItem == null)
                            {
                                return;
                            }
                            cursor.Aquire();

                            control = (FrameworkElement)Activator.CreateInstance(listBox.SelectedItem as Type);
                        }
                        else
                        {
                            control = (FrameworkElement)Activator.CreateInstance(controls[0]);
                        }

                        problematicXaml = control.XamlWriterFormatted();
                        listAssemblies = Utilities.WPF.XmlHelper.GetAssemblyListInXaml(problematicXaml).ToList();
                        Utilities.WPF.XmlHelper.SetProblematicXamlWriter(control, problematicXaml);
                    }
                    catch (Exception ex)
                    {
                        EditorComponent.UIInterface.ShowError(ex.Message);
                        control = null;
                    }
                }
            }
            else if (String.Compare(fileExtension, ".xaml", true) == 0)
            {
                try
                {
                    text = File.ReadAllText(fileName);
                    control = text.ReadUIElement(System.IO.Path.GetDirectoryName(fileName)) as FrameworkElement;
                    string changedtext;
                    if (ManageImport3DImageBaseUri(text, System.IO.Path.GetDirectoryName(fileName), out changedtext))
                        text = changedtext;

                }
                catch (Exception ex)
                {
                    EditorComponent.UIInterface.ShowError(ex.Message);
                    control = null;
                }
            }
            else if ( /*String.Compare(fileExtension, ".3ds", true) == 0 || */
                     String.Compare(fileExtension, ".lwo", true) == 0 ||
                     String.Compare(fileExtension, ".obj", true) == 0 ||
                     String.Compare(fileExtension, ".objz", true) == 0 ||
                     String.Compare(fileExtension, ".stl", true) == 0 ||
                     String.Compare(fileExtension, ".off", true) == 0)
            {
                try
                {
                    var group = HelixToolkit.Wpf.ModelImporter.Load(fileName);
                    var viewport3D = new Viewport3D();
                    var perspectiveCamera = new PerspectiveCamera();
                    HelixToolkit.Wpf.CameraHelper.Reset(perspectiveCamera);
                    viewport3D.Camera = perspectiveCamera;

                    var viual3D = new ModelVisual3D()
                    {
                        Content = group
                    };
                    viewport3D.Children.Add(viual3D);
                    control = viewport3D;

                    if (control.ReadLocalValue(FrameworkElement.WidthProperty) == DependencyProperty.UnsetValue)
                        control.Width = 200;
                    if (control.ReadLocalValue(FrameworkElement.HeightProperty) == DependencyProperty.UnsetValue)
                        control.Height = 200;
                    control.Measure(new Size(control.Width, control.Height));
                    control.Arrange(new Rect(new Point(0, 0), control.DesiredSize));
                    control.ApplyTemplate();
                    control.UpdateLayout();

                    HelixToolkit.Wpf.CameraHelper.ZoomExtents(perspectiveCamera, viewport3D, 0);
                }
                catch (Exception ex)
                {
                    EditorComponent.UIInterface.ShowError(ex.Message);
                    control = null;
                }
            }
            else
            {
                EditorComponent.UIInterface.ShowError(String.Format(Properties.Resources.DunnoHowToImportFile, fileName));
                control = null;
            }

            if (control == null)
                return;

            if (control.ReadLocalValue(FrameworkElement.WidthProperty) == DependencyProperty.UnsetValue)
                control.Width = 200;
            if (control.ReadLocalValue(FrameworkElement.HeightProperty) == DependencyProperty.UnsetValue)
                control.Height = 200;

            using (new ChangeScreenWatcher(this, true, UndoRedoManager.UndoAction.Added))
            {
                try
                {
                    Canvas.SetTop(control, 10);
                    Canvas.SetLeft(control, 10);

                    ActiveLayer.Children.Add(control);

                    if (!String.IsNullOrEmpty(text))
                    {
                        if (Utilities.WPF.XmlHelper.CheckProblematicXamlWriter(control, text))
                        {
                            Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, control, true, true);
                            Document.SetProblematicXaml(control, text);
                        }
                        else
                            Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, control, true);
                    }

                    List<UIElement> elementsToSelect = new List<UIElement>();
                    elementsToSelect.Add(control);
                    ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                    MainSurface_SetCurrentSelection(readonlyList);
                    if (Document.ProjectType == ProjectType.WebHMI.ToString())
                        readonlyList.ToList().ForEach(uie =>
                        {
                            if (WebHMIDesignHelper.WebHMIHelper.VisibleHMIScreenControls.Contains(uie.GetType().Name))
                                return;
                            UpdateHMIControl(uie);
                        });
                    if (!String.IsNullOrEmpty(problematicXaml))
                    {
                        Document.SetProblematicXaml(control, problematicXaml);
                        Document.AddListAssembly(listAssemblies);
                    }
                }
                catch(Exception ex)
                {
                    try
                    {
                        ActiveLayer.Children.Remove(control);
                    }
                    catch
                    {

                    }
                    EditorComponent.UIInterface.ShowError(ex.Message);
                }
            }
        }

        #endregion Commands

        void Test()
        {
            if (Document == null)
                return;

            String filename = Document.FullPath;
            // ScreenDocument doc = ScreenDocument.FromFile(filename);

            var screenViewer = new ScreenTransitionViewer(EditorComponent, Document.Parent, null, true);
            // doc.DocumentParent = Document.DocumentParent;
            // doc.ActiveView = screenViewer;

            //EditorComponent.Workspace.SetDesiredHeightAndWidthInDockedMode(screenViewer, screenViewer.Height, screenViewer.Width);
            //screenViewer.ClearValue(FrameworkElement.WidthProperty);
            //screenViewer.ClearValue(FrameworkElement.HeightProperty);

            screenViewer.ListScreens.Add(filename);
            //EditorComponent.Workspace.AddDockingChildren(screenViewer, "[Runtime]", DockState.Document, DockSide.Left);
            //EditorComponent.Workspace.ActiveWindow = screenViewer;

            //Canvas cv = Surface;
            //ScrollViewer grid = new ScrollViewer
            //{
            //    Content = cv,
            //    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            //    VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            //};

            Window wnd = null;
            if (Properties.Settings.Default.UseThemedWindow)
                wnd = new ThemedWindow();
            else
                wnd = new DXWindow();
            wnd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            wnd.ShowActivated = true;
            wnd.WindowState = WindowState.Normal;
            wnd.SizeToContent = SizeToContent.WidthAndHeight;
            wnd.WindowStyle = WindowStyle.ThreeDBorderWindow;
            wnd.Content = screenViewer;
            wnd.Owner = this.FindParent<Window>();

            ResourceDictionaryExtensions.AddCommonResources(wnd);

            //// Document.LoadResources(wnd);

            ThemeHelper.SetTheme(wnd, Document.Theme);

            //wnd.Loaded += (o, e) =>
            //    {
            //        Document.PrepareExecution(cv);
            //    };
            //wnd.Closing += (o, e) =>
            //    {
            //        Document.TerminateExecution(cv);
            //    };
            wnd.Show();
            wnd.Closed += (ob, eve) =>
            {
                screenViewer.Dispose();

                RealTimeConnectionManagerViewModel.CleanDeadConnections(true);
            };
        }

        #region Selection management

        internal void OnDeactivate()
        {
            bIsActive = false;
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                SaveDockSettings();

            AdornerLayer adorner = AdornerLayer.GetAdornerLayer(MainSurface);
            if (adorner != null)
                adorner.IsHitTestVisible = false;
        }
        
        bool bIsActive;
        internal void OnActivate(bool bForceRefresh = false)
        {
            AdornerLayer adorner = AdornerLayer.GetAdornerLayer(MainSurface);
            if (adorner != null)
                adorner.IsHitTestVisible = true;

            ResizeContent();
            RefreshCurrentSelection(bForceRefresh);
            PasteCounter = 0;
            bIsActive = true;
            if (layoutItems.Visibility == Visibility.Visible)
                layoutItems.IsCustomization = true;
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                RestoreDockSettings();
        }

        void RefreshCurrentSelection(bool bForce = false)
        {
            var listSelected = MainSurface_GetCurrentSelection();
            MainSurface_CleanCurrentSelectedion();
            MainSurface_SetCurrentSelection(listSelected.AsReadOnly(), bForceRefresh: bForce);
        }

        internal void UpdateCurrentSelectionContext(Point? startposition = null, Point? endposition = null)
        {
            if (/*e.Source == ActiveLayer && */(EditorComponent.ToolBox != null && EditorComponent.ToolBox.ActiveToolCode != null
                || currentObjectTypeFile != null) && startposition != null)
            {
                if (/*e.Source == ActiveLayer && */(EditorComponent.ToolBox != null && EditorComponent.ToolBox.ActiveToolCode != null
                    || currentObjectTypeFile != null))
                {
                    using (new ChangeScreenWatcher(this, true, UndoRedoManager.UndoAction.Added))
                        AddToolBoxDataObject(startposition.Value, endposition);
                    return;
                }
            }

            var listToSelect = MainSurface_GetCurrentSelection();

            var selected = new List<ScreenEntity>();
            foreach (UIElement uie in listToSelect)
            {
                var entity = GetSelectedEntity(uie as FrameworkElement);
                selected.Add(entity);
            }

            if (listActive3dModels.Count > 0 && listToSelect.Count == 1 && selected.Count == 1 && listToSelect[0] is FrameworkElement)
            {
                var model = listActive3dModels.First();
                var hash = Utilities.WPF.DependencyObjectExtensions.RegisterModel3D(MainSurface, listToSelect[0] as FrameworkElement, model);
                var entity = selected[0].AddOrFind3DInnerModel(model, hash);
                entity.Entity3D = model;
                selected.Clear();
                selected.Add(entity);
            }

            if (listToSelect.Count > 0)
                EditorComponent.Workspace.ContextObjects = selected;
            else
                EditorComponent.Workspace.ContextObject = Document;
        }

        internal void InvalidateSelection(Point? startposition, Point? endposition)
        {
            if (/*e.Source == ActiveLayer && */(EditorComponent.ToolBox != null && EditorComponent.ToolBox.ActiveToolCode != null
                || currentObjectTypeFile != null))
                return;

            using (var cursor = new WaitCursor())
            {
                List<UIElement> elementsToSelect = MainSurface_GetCurrentSelection();

                Rect selectedArea = new Rect(startposition.Value, endposition.Value);
                foreach (UIElement item in MainSurface.Children)
                {
                    Rect itemRect = VisualTreeHelper.GetDescendantBounds(item);
                    Rect itemBounds = item.TransformToAncestor(ActiveLayer).TransformBounds(itemRect);

                    if (selectedArea.Contains(itemBounds) && item.Visibility != Visibility.Collapsed)
                    {
                        if (!elementsToSelect.Contains(item))
                            elementsToSelect.Add(item);
                    }
                    else
                    {
                        if (elementsToSelect.Contains(item))
                            elementsToSelect.Remove(item);
                    }
                }

                ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                MainSurface_UpdateCurrentSelection(readonlyList);
            }
        }

        Point? startPoint;
        bool dragControl;
        readonly List<Model3D> listActive3dModels = new List<Model3D>();
        readonly Dictionary<GeometryModel3D, Material> mapActive3dModels = new Dictionary<GeometryModel3D, Material>();
        readonly Dictionary<GeometryModel3D, Material> mapBackActive3dModels = new Dictionary<GeometryModel3D, Material>();
        readonly List<GeometryModel3D> listToRecover3DGeometries = new List<GeometryModel3D>();
        readonly List<Model3D> listToRecoverActive3dModels = new List<Model3D>();
        SolidColorBrush currentBrush3D;
        void RestoreSelected3DModels()
        {
            foreach (var model in mapActive3dModels.Keys)
                model.Material = mapActive3dModels[model];
            mapActive3dModels.Clear();
            foreach (var model in mapBackActive3dModels.Keys)
                model.BackMaterial = mapBackActive3dModels[model];
            mapBackActive3dModels.Clear();
            listActive3dModels.ForEach(model =>
                {
                    model.Scale3D(0.0, -1, false, false, null);
                });
            listActive3dModels.Clear();
        }
        void RestoreBeginEdit3DModels()
        {
            listToRecoverActive3dModels.Clear();
            listToRecover3DGeometries.Clear();
            listToRecoverActive3dModels.AddRange(listActive3dModels);
            listToRecover3DGeometries.AddRange(mapActive3dModels.Keys);
            RestoreSelected3DModels();
        }
        void RestoreAfterBeginEdit3DModels()
        {
            listActive3dModels.AddRange(listToRecoverActive3dModels);
            listToRecover3DGeometries.ForEach(geometry =>
                {
                    mapActive3dModels.Add(geometry, geometry.Material);
                    mapBackActive3dModels.Add(geometry, geometry.BackMaterial);
                    listActive3dModels.Add(geometry);
                    geometry.Material = new DiffuseMaterial()
                    {
                        Brush = currentBrush3D
                    };
                    geometry.BackMaterial = new DiffuseMaterial()
                    {
                        Brush = currentBrush3D
                    };
                });
        }

        void CreateSelectionBrush3d()
        {
            if (currentBrush3D != null)
                return;
            currentBrush3D = new SolidColorBrush() { Color = Colors.White };
            currentBrush3D.SolidBrushColorAnimation(Colors.White, Colors.Black, 1000, true, 
                true, new SineEase() { EasingMode = EasingMode.EaseOut });
        }

        public bool CanDragStart(FrameworkElement item, DragStartedEventArgs e)
        {
            var currentselection = MainSurface_GetCurrentSelection();
            MainSurface_MouseDown(item, new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left) 
                                            {
                                                RoutedEvent = Mouse.MouseDownEvent,
                                                Source = ActiveLayer 
                                            });
            var newselection = MainSurface_GetCurrentSelection();

            if (newselection.Count == currentselection.Count && newselection.Count > 0)
            {
                for(int i = 0; i < newselection.Count; ++i)
                    if (newselection[i] != currentselection[i])
                        return false;
            }

            return (newselection.Count == currentselection.Count);
        }

        private void MainSurface_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (EditorComponent.Workspace.ActiveWindow != this)
                return;

            RestoreSelected3DModels();

            if (bSetZOrderInProgress)
                return;

            if (EditorComponent.ToolBox != null && EditorComponent.ToolBox.ActiveToolCode != null)
            {
                startPoint = new Point?(e.GetPosition(ActiveLayer));
                e.Handled = true;
                return;
            }

            Focusable = true;
            Focus();

            UIElement element = e.Source as UIElement;
            if (e.ChangedButton == MouseButton.Right)
            {
                var currentSelection = MainSurface_GetCurrentSelection();
                if (currentSelection.Contains(element))
                    return;
            }

            if (element != ActiveLayer && !ActiveLayer.Children.Contains(element))
            {
                if (element != null &&
                    (Keyboard.IsKeyDown(Key.LeftShift) && Keyboard.IsKeyDown(Key.LeftCtrl) ||
                    (Keyboard.IsKeyDown(Key.RightShift) && Keyboard.IsKeyDown(Key.RightCtrl))))
                {
                    var name = element.Uid as String;
                    if (!String.IsNullOrEmpty(name))
                    {
                        var elementsToSelect = new List<UIElement>();
                        elementsToSelect.Add(element);

                        element.Scale(1.2, 250, false, true, new BackEase() { EasingMode = EasingMode.EaseOut }, false, (o, i) =>
                        {
                            ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                            MainSurface_SetCurrentSelection(readonlyList);
                        });
                    }
                    else
                    {
                        var contentControl = LogicalTreeHelper.GetParent(element) as ContentControl;
                        if (contentControl != null)
                        {
                            name = contentControl.Uid as String;
                            if (!String.IsNullOrEmpty(name))
                            {
                                var elementsToSelect = new List<UIElement>();
                                elementsToSelect.Add(element);

                                element.Scale(1.2, 250, false, true, new BackEase() { EasingMode = EasingMode.EaseOut }, false, (o, i) =>
                                {
                                    ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                                    MainSurface_SetCurrentSelection(readonlyList);
                                });
                            }
                        }
                    }
                    e.Handled = true;
                    return;
                }
                else if (element != null)
                {
                    while ((element = LogicalTreeHelper.GetParent(element) as UIElement) != null)
                    {
                        if (ActiveLayer.Children.Contains(element))
                            break;
                    }
                }

                if (element == null)
                    element = e.Source as UIElement;
            }

            if (element == ActiveLayer || ActiveLayer.Children.Contains(element))
            {
                BasicAdorner adorner = null;
                if (e.OriginalSource is Visual || e.OriginalSource is Visual3D)
                    adorner = (e.OriginalSource as DependencyObject).FindParent<BasicAdorner>();
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) || !(e.OriginalSource is Thumb) && adorner == null)
                {
                    Point endPoint = e.GetPosition(ActiveLayer);

                    UIElement uie = null;
                    var mapRestoreInvisibles = new Dictionary<UIElement, int>();
                    while (true)
                    {
                        var hitTestResult = VisualTreeHelper.HitTest(ActiveLayer, endPoint);
                        if (hitTestResult != null && hitTestResult.VisualHit != null)
                        {
                            var parent = hitTestResult.VisualHit as DependencyObject;
                            if (parent != null)
                            {
                                ContentControl parentControl = null;
                                do
                                {
                                    if (parentControl == null && parent is ContentControl &&
                                        (parent as ContentControl).Content is UIElement && !(parent is UserControl) &&
                                        (Keyboard.IsKeyDown(Key.LeftShift) && Keyboard.IsKeyDown(Key.LeftCtrl) ||
                                        (Keyboard.IsKeyDown(Key.RightShift) && Keyboard.IsKeyDown(Key.RightCtrl))) &&
                                        !((parent as ContentControl).Content as UIElement).IsHitTestVisible)
                                        parentControl = parent as ContentControl;

                                    if (ActiveLayer.Children.Contains(parent as UIElement))
                                    {
                                        uie = parent as UIElement;
                                        break;
                                    }
                                    parent = VisualTreeHelper.GetParent(parent) as DependencyObject;

                                } while (parent != null && parent != ActiveLayer);

                                if (parentControl != null)
                                {
                                    UIElement subElement = hitTestResult.VisualHit as UIElement;
                                    var name = subElement?.Uid as String;
                                    if (element != subElement && !ActiveLayer.Children.Contains(subElement) &&
                                        !string.IsNullOrEmpty(name))
                                    {
                                        var elementsToSelect = new List<UIElement>();
                                        elementsToSelect.Add(subElement);

                                        subElement.Scale(1.2, 250, false, true, new BackEase() { EasingMode = EasingMode.EaseOut }, false, (o, i) =>
                                        {
                                            ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                                            MainSurface_SetCurrentSelection(readonlyList);
                                        });


                                        e.Handled = true;
                                        return;
                                    }
                                }
                                
                                if (uie != null)
                                {
                                    if (uie.Visibility != Visibility.Visible)
                                    {
                                        var index = ActiveLayer.Children.IndexOf(uie);
                                        if (index != -1)
                                        {
                                            ActiveLayer.Children.Remove(uie);
                                            mapRestoreInvisibles.Add(uie, index);
                                            uie = null;
                                        }
                                        else
                                            break;
                                    }
                                    else
                                        break;
                                }
                                else
                                    break;
                            }
                        }
                        else
                            break;
                    }

                    foreach (var pair in mapRestoreInvisibles.Reverse())
                        ActiveLayer.Children.Insert(pair.Value, pair.Key);

                    if (uie == null)
                    {
                        for (int i = ActiveLayer.Children.Count - 1; i >= 0; --i)
                        {
                            var uielooper = ActiveLayer.Children[i];

                            Rect itemBounds = Rect.Empty;
                            Rect itemRect = VisualTreeHelper.GetDescendantBounds(uielooper);
                            if (itemRect.IsEmpty || (itemRect.Width == 0 && itemRect.Height == 0))
                            {
                                var fe = uielooper as FrameworkElement;
                                if (fe != null)
                                {
                                    itemRect = new Rect(Canvas.GetLeft(uielooper), Canvas.GetTop(uielooper),
                                        fe.ActualWidth, fe.ActualHeight);
                                    itemBounds = itemRect;
                                }
                            }
                            else
                                itemBounds = uielooper.TransformToAncestor(ActiveLayer).TransformBounds(itemRect);

                            itemBounds.Inflate(3, 3);
                            if (!itemBounds.Contains(endPoint))
                                continue;

                            if (uielooper.InputHitTest(endPoint) != null)
                                uie = uielooper;
                        }
                    }

                    if (uie == null)
                    {
                        if (e.ChangedButton != MouseButton.Right)
                        {
                            startPoint = new Point?(e.GetPosition(ActiveLayer));
                            e.Handled = true;
                        }

                        List<UIElement> elementsToSelect = MainSurface_GetCurrentSelection();
                        if (elementsToSelect.Count == 1)
                        {
                            var entity = GetSelectedEntity(elementsToSelect[0] as FrameworkElement);

                            if (elementsToSelect.Count > 0)
                                EditorComponent.Workspace.ContextObjects = new List<ScreenEntity>() { entity };
                            else
                                EditorComponent.Workspace.ContextObject = Document;
                        }
                        return;
                    }

                    if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        List<UIElement> elementsToSelect = MainSurface_GetCurrentSelection();
                        if (!elementsToSelect.Contains(uie))
                        {
                            elementsToSelect.Clear();
                            elementsToSelect.Add(uie);
                            ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                            MainSurface_SetCurrentSelection(readonlyList);

                            e.Handled = true;
                        }
                        else if(!(bool)enable3DCameraBI.IsChecked)
                        {
                            CreateSelectionBrush3d();

                            var listViewport3Ds = uie.GetChildrenOfType<Viewport3D>().ToList();
                            if (listViewport3Ds.Count == 0 && uie is Viewport3D)
                                listViewport3Ds.Add(uie as Viewport3D);
                            foreach (var viewport3D in listViewport3Ds)
                            {
                                var position = e.GetPosition(viewport3D);
                                var rayMeshResult = VisualTreeHelper.HitTest(viewport3D, position) as RayMeshGeometry3DHitTestResult;

                                //VisualTreeHelper.HitTest(viewport3D, null, (res) =>
                                //    {
                                //        if (res.VisualHit is RayMeshGeometry3DHitTestResult)
                                //        {
                                //        }
                                //        return HitTestResultBehavior.Stop;
                                //    }, new PointHitTestParameters(position));

                                if (rayMeshResult != null && rayMeshResult.ModelHit != null)
                                {
                                    if (rayMeshResult.ModelHit is GeometryModel3D)
                                    {
                                        var model = rayMeshResult.ModelHit as GeometryModel3D;
                                        bool bFound = false;
                                        if (Keyboard.IsKeyDown(Key.LeftShift) && 
                                            rayMeshResult.VisualHit is ModelVisual3D)
                                        {
                                            var visual3D = rayMeshResult.VisualHit as ModelVisual3D;
                                            var father = Utilities.WPF.DependencyObjectExtensions.FindParentGroup(visual3D, model);
                                            if (father != null)
                                            {
                                                listActive3dModels.Add(father);
                                                father.Scale3D(1.5, 500, false, true, new SineEase() { EasingMode = EasingMode.EaseInOut });

                                                var geometries = Utilities.WPF.DependencyObjectExtensions.GetAllGeometries(father);
                                                foreach (var geometry in geometries)
                                                {
                                                    mapActive3dModels.Add(geometry, geometry.Material);
                                                    mapBackActive3dModels.Add(geometry, geometry.BackMaterial);
                                                    // listActive3dModels.Add(geometry);
                                                    geometry.Material = new DiffuseMaterial()
                                                    {
                                                        Brush = currentBrush3D
                                                    };
                                                    geometry.BackMaterial = new DiffuseMaterial()
                                                    {
                                                        Brush = currentBrush3D
                                                    };
                                                }

                                                bFound = true;
                                            }
                                            else if (visual3D.Content is Model3D)
                                            {
                                                var modelgroup = visual3D.Content as Model3D;
                                                listActive3dModels.Add(modelgroup);
                                                modelgroup.Scale3D(1.5, 500, false, true, new SineEase() { EasingMode = EasingMode.EaseInOut });

                                                var geometries = Utilities.WPF.DependencyObjectExtensions.GetAllGeometries(visual3D);
                                                if (geometries != null)
                                                {
                                                    foreach (var geometry in geometries)
                                                    {
                                                        mapActive3dModels.Add(geometry, geometry.Material);
                                                        mapBackActive3dModels.Add(geometry, geometry.BackMaterial);
                                                        // listActive3dModels.Add(geometry);
                                                        geometry.Material = new DiffuseMaterial()
                                                        {
                                                            Brush = currentBrush3D
                                                        };
                                                        geometry.BackMaterial = new DiffuseMaterial()
                                                        {
                                                            Brush = currentBrush3D
                                                        };
                                                    }

                                                    bFound = true;
                                                }
                                            }
                                        }
                                            
                                        if (!bFound)
                                        {
                                            mapActive3dModels.Add(model, model.Material);
                                            mapBackActive3dModels.Add(model, model.BackMaterial);
                                            listActive3dModels.Add(model);
                                            model.Material = new DiffuseMaterial()
                                            {
                                                Brush = currentBrush3D
                                            };
                                            model.BackMaterial = new DiffuseMaterial()
                                            {
                                                Brush = currentBrush3D
                                            };
                                            model.Scale3D(1.5, 500, false, true, new SineEase() { EasingMode = EasingMode.EaseInOut });
                                        }
                                    }
                                    //else
                                    //{
                                    //    if (rayMeshResult.ModelHit is Model3DGroup && Keyboard.IsKeyDown(Key.LeftShift))
                                    //    {
                                    //        listActive3dModels.Add(rayMeshResult.ModelHit);
                                    //        rayMeshResult.ModelHit.Scale3D(1.5, 500, false, true, new SineEase() { EasingMode = EasingMode.EaseInOut });
                                    //    }
                                    //    else
                                    //    {
                                    //        var geometries = rayMeshResult.ModelHit.GetChildrenOfType<GeometryModel3D>();
                                    //        foreach (var geometry in geometries)
                                    //        {
                                    //            geometry.Scale3D(1.5, 500, false, true, new SineEase() { EasingMode = EasingMode.EaseInOut });
                                    //            mapActive3dModels.Add(geometry, geometry.Material);
                                    //            mapBackActive3dModels.Add(geometry, geometry.BackMaterial);
                                    //            listActive3dModels.Add(geometry);
                                    //            geometry.Material = new DiffuseMaterial()
                                    //            {
                                    //                Brush = currentBrush3D
                                    //            };
                                    //            geometry.BackMaterial = new DiffuseMaterial()
                                    //            {
                                    //                Brush = currentBrush3D
                                    //            };
                                    //        }
                                    //    }
                                    //}
                                    if (listActive3dModels.Count > 0 && elementsToSelect.Count == 1 && elementsToSelect[0] is FrameworkElement)
                                    {
                                        var entity = GetSelectedEntity(elementsToSelect[0] as FrameworkElement);

                                        var model = listActive3dModels.First();
                                        var hash = Utilities.WPF.DependencyObjectExtensions.RegisterModel3D(MainSurface, elementsToSelect[0] as FrameworkElement, model);
                                        entity = entity.AddOrFind3DInnerModel(model, hash);
                                        entity.Entity3D = model;

                                        if (elementsToSelect.Count > 0)
                                            EditorComponent.Workspace.ContextObjects = new List<ScreenEntity>() { entity };
                                        else
                                            EditorComponent.Workspace.ContextObject = Document;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        List<UIElement> elementsToSelect = MainSurface_GetCurrentSelection();
                        if (!elementsToSelect.Contains(uie))
                        {
                            elementsToSelect.Insert(0, uie);

                            ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                            MainSurface_SetCurrentSelection(readonlyList);
                            e.Handled = true;
                        }
                        else if (elementsToSelect.Count > 1 && elementsToSelect[0] != uie)
                        {
                            elementsToSelect.Remove(uie);

                            ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                            MainSurface_SetCurrentSelection(readonlyList);
                            e.Handled = true;
                        }
                    }
                }
            }
            else
                EditorComponent.Workspace.ContextObject = Document;
        }

        void AddToolBoxDataObject(Point dropPosition, Point? endposition = null)
        {
            AdjustPointToGrid(ref dropPosition);

            try
            {
                {
                    //String folder = null;
                    //if (!String.IsNullOrEmpty(currentObjectTypeFile))
                    //    folder = System.IO.Path.GetDirectoryName(currentObjectTypeFile);
                    String xamlData = String.Empty;
                    ToolBoxData data = null;
                    if (currentObjectTypeFile != null)
                    {
                        xamlData = EditorComponent.ToolBox.GetCodeFromHash(currentObjectTypeFile.Hash);
                        data = currentObjectTypeFile;
                    }
                    else if (EditorComponent.ToolBox.ActiveToolCode != null)
                    {
                        xamlData = EditorComponent.ToolBox.GetCodeFromHash(EditorComponent.ToolBox.ActiveToolCode.Hash);
                        data = EditorComponent.ToolBox.ActiveToolCode;
                    }

                    var path = EditorComponent.ToolBox.GetCurrentSourceSymbolPath(data.Hash);

                    List<UIElement> elementsToSelect = new List<UIElement>();
                    if (!String.IsNullOrEmpty(path))
                    {
                        var element = new ContentControl();
                        //TextBoxProperties.SetUnderlineTextSource(element, false);

                        //if (MainSurface is Canvas)
                        //{
                            Canvas.SetLeft(element, dropPosition.X);
                            Canvas.SetTop(element, dropPosition.Y);
                        //    element.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                        //    element.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                        //}
                        //else
                        //{
                        //    Canvas.SetLeft(element, dropPosition.X);
                        //    Canvas.SetTop(element, dropPosition.Y);
                        //    element.SetValue(Canvas.TopProperty, DependencyProperty.UnsetValue);
                        //    element.SetValue(Canvas.LeftProperty, DependencyProperty.UnsetValue);
                        //}

                        ActiveLayer.Children.Add(element);

                        var settings = EditorComponent.ToolBox.GetCurrentDropSettings(data.Hash);
                        var provider = EditorComponent.ToolBox.GetCurrentSourceSymbolProvider(data.Hash);

                        bool bForceName = !String.IsNullOrEmpty(settings) ||
                                            !String.IsNullOrEmpty(provider) ||
                                            !String.IsNullOrEmpty(path);
                        // element.Name = path;
                        var map = Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, element, bForceName);
                        //Document.MergeDocument(settings, map);
                        //Document.LoadResources(this);
                        Document.SetSourceProviderPath(element, provider, path);
                        try
                        {
                            Document.LoadRepositoryItem(this, MainSurface, element.Name, bSetSize: true);
                        }
                        catch (Exception ex)
                        {
                            EditorComponent.UIInterface.ShowError(ex.Message);
                        }
                        //element.SetResourceReference(ContentControl.ContentProperty, element.Name);

                        //var bindingWidth = new Binding()
                        //{
                        //    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UIElement), 1),
                        //    Path = new PropertyPath("ActualWidth")
                        //};
                        //var bindingHeight = new Binding()
                        //{
                        //    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UIElement), 1),
                        //    Path = new PropertyPath("ActualHeight")
                        //};

                        //var fe = (element.Content as FrameworkElement);
                        //element.Width = fe.Width;
                        //element.Height = fe.Height;
                        //fe.SetBinding(FrameworkElement.WidthProperty, bindingWidth);
                        //fe.SetBinding(FrameworkElement.HeightProperty, bindingHeight);

                        //element.SetBinding(ContentControl.ContentProperty,
                        //    new Binding()
                        //    {
                        //        Source = new StaticResourceExtension(element.Name)
                        //    });

                        elementsToSelect.Add(element);

                        Document.GetListElementsUsingProviderPath(provider, path).ForEach(name =>
                            {
                                if (name != element.Name)
                                {
                                    var el = Document.FindInnerControl(MainSurface, name);
                                    // var el = MainSurface.FindName(name) as FrameworkElement;
                                    if (el != null)
                                        Document.ResolveEntityBrushAndPen(MainSurface, el);
                                }
                            });

                        //if (Utilities.WPF.XmlHelper.CheckProblematicXamlWriter(element, xamlData))
                        //    Document.SubscribePropertyChangeXamlWriterProperties(element, element.Name);

                        if (endposition != null)
                        {
                            var w = Math.Abs(endposition.Value.X - dropPosition.X);
                            var h = Math.Abs(endposition.Value.Y - dropPosition.Y);
                            if (w > 10 && h > 10)
                            {
                                element.Width = w;
                                element.Height = h;
                            }
                        }

                        //if (bForceName)
                        //    Document.UpdateRepositoryItems(this, MainSurface, true); // case 11656
                        Document.AddDynamicEntity(element);
                    }
                    else
                    {
                        FrameworkElement element = xamlData.ReadUIElement() as FrameworkElement;

                        element.IsHitTestVisible = true;

                        Canvas.SetTop(element, dropPosition.Y);
                        Canvas.SetLeft(element, dropPosition.X);

                        ActiveLayer.Children.Add(element);

                        // element.ApplyStoryBoard("ViewLoadingStoryboard", false, false, (o, i) =>
                        // {
                        if (element is FrameworkElement)
                        {
                            if (Utilities.WPF.XmlHelper.CheckProblematicXamlWriter(element, xamlData))
                            {
                                Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, element as FrameworkElement, true, true);
                                Document.SetProblematicXaml(element, xamlData);
                            }
                            else
                                Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, element as FrameworkElement, true);
                        }

                        var listAssemblies = Utilities.WPF.XmlHelper.GetAssemblyListInXaml(xamlData);
                        Document.AddListAssembly(listAssemblies);

                        elementsToSelect.Add(element);

                        if (endposition != null)
                        {
                            var w = Math.Abs(endposition.Value.X - dropPosition.X);
                            var h = Math.Abs(endposition.Value.Y - dropPosition.Y);
                            if (w > 10 && h > 10)
                            {
                                element.Width = w;
                                element.Height = h;
                            }
                        }
                        Document.AddDynamicEntity(element);
                    }

                    ReadOnlyCollection<UIElement> readonlyList = new ReadOnlyCollection<UIElement>(elementsToSelect);
                    MainSurface_SetCurrentSelection(readonlyList);
                    elementsToSelect.ForEach(uie => UpdateHMIControl(uie));
                }
            }
            catch (Exception ex)
            {
                EditorComponent.UIInterface.ShowError(String.Format(
                    Properties.Resources.ErrorToolbox, ex.Message));
            }

            CancelCurrentSelectedInsertObject();
        }

        private void MainSurface_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (bSetZOrderInProgress)
                return;

            Focusable = true;
            Focus();

            if (dragControl)
            {
                dragControl = false;
                var listSelected = MainSurface_GetCurrentSelection();
                if (listSelected.Count > 0)
                {
                    ResizeConrtol(listSelected[0], startPoint.Value, e.GetPosition(MainSurface), false);
                }
                return;
            } 

            if (/*e.Source == ActiveLayer && */(EditorComponent.ToolBox != null && EditorComponent.ToolBox.ActiveToolCode != null
                || currentObjectTypeFile != null))
            {
                e.Handled = true;
                var dropPosition = e.GetPosition(ActiveLayer);
                using (new ChangeScreenWatcher(this, true, UndoRedoManager.UndoAction.Added))
                    AddToolBoxDataObject(dropPosition);
                return;
            }

            {
                BasicAdorner adorner = null;
                if (e.OriginalSource is Visual || e.OriginalSource is Visual3D)
                    adorner = (e.OriginalSource as DependencyObject).FindParent<BasicAdorner>();

                if (adorner != null ||
                    !LayoutHelper.IsChildElement(ActiveLayer, e.OriginalSource as DependencyObject))
                    return;

                if (e.Source == ActiveLayer || ActiveLayer.Children.Contains(e.Source as UIElement))
                {
                    if (e.Source != ActiveLayer || !startPoint.HasValue)
                        return;

                    MainSurface_CleanCurrentSelectedion();
                    EditorComponent.Workspace.ContextObject = Document;

                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();

                    e.Handled = true;
                }
            }

            //FocusManager.SetFocusedElement(this, this);
            //MainSurface.Focus();
        }

        private void MainSurface_MouseMove(object sender, MouseEventArgs e)
        {
            if (bSetZOrderInProgress)
                return;

            ShowToolTip();

            if (/*e.Source == MainSurface && */EditorComponent.ToolBox != null)
            {
                if (EditorComponent.ToolBox.ActiveToolCode != null)
                {
                    if (currentCursor == null || lastToolboxHash != EditorComponent.ToolBox.ActiveToolCode.Hash)
                    {
                        CreateCurrentToolCursor(EditorComponent.ToolBox.ActiveToolCode);
                    }
                }
                else
                    CancelCurrentSelectedInsertObject();
            }
            //else if (currentCursor != null)
            //{
            //    MainSurface.Cursor = currentCursor;
            //}

            if (e.LeftButton != MouseButtonState.Pressed)
            {
                if(dragControl)
                {
                    dragControl = false;
                    var listSelected = MainSurface_GetCurrentSelection();
                    if (listSelected.Count > 0 && startPoint != null)
                    {
                        ResizeConrtol(listSelected[0], startPoint.Value, e.GetPosition(MainSurface), false);
                    }
                }
                startPoint = null;
            }

            if (startPoint.HasValue)
            {
                if ((EditorComponent.ToolBox != null && EditorComponent.ToolBox.ActiveToolCode != null
                                || currentObjectTypeFile != null) && startPoint != null)
                {
                    ////Get the current position as end position.
                    UpdateCurrentSelectionContext(startPoint, e.GetPosition(MainSurface));
                    var listSelected = MainSurface_GetCurrentSelection();
                    if (listSelected.Count > 0)
                    {
                        dragControl = true;
                    }
                }
                else if (dragControl)
                {
                    var listSelected = MainSurface_GetCurrentSelection();
                    if (listSelected.Count > 0)
                    {
                        ResizeConrtol(listSelected[0], startPoint.Value, e.GetPosition(MainSurface));
                    } 
                }
                else
                {
                    AdornerLayer adorner = AdornerLayer.GetAdornerLayer(MainSurface);
                    if (adorner != null)
                    {
                        SelectionAdorner nodeadorner = new SelectionAdorner(this, startPoint);
                        if (adorner != null)
                        {
                            adorner.Add(nodeadorner);
                        }
                    }

                }

                e.Handled = true;
            }
        }

        private void ResizeConrtol(UIElement uie, Point dropPosition, Point endPosition, bool lockMovement = true)
        {
            if (uie != null)
            {
                try
                {
                    BasicAdorner ba = (from key in SelectedElementsMap.Keys where key == uie select SelectedElementsMap[key]).FirstOrDefault();
                        
                    if (ba != null)
                        ba.SetLockMovement(lockMovement);

                    if (endPosition != null)
                    {
                        var w = (endPosition.X - dropPosition.X);
                        var h = (endPosition.Y - dropPosition.Y);
                        if(Math.Abs(w) > 10 && Math.Abs(h) > 10)
                        {
                            (uie as FrameworkElement).Width = w;
                            (uie as FrameworkElement).Height = h;

                            if (ba != null)
                            {
                                ba.Width = w;
                                ba.Height = h;
                            }
                        }
                    }                
                }
                catch (Exception)
                {
                }
            }
        }

        #endregion Selection management

        #region IEditableObject Members

        Dictionary<UIElement, String> mapEditingElements = new Dictionary<UIElement, string>();
        List<PropertyChangeNotifier> propertyChangeNotifierList;
        public void BeginEdit()
        {
            RestoreBeginEdit3DModels();
            undoredo.BeginEdit();
            RestoreAfterBeginEdit3DModels();

            mapEditingElements.Clear();

            var listSelected = MainSurface_GetCurrentSelection();
            if (listSelected.Count == 0)
                throw new NotSupportedException("Undo not available for document properties");

            using (var cursor = new WaitCursor())
            {
                foreach (var el in listSelected)
                {
                    var fe = el as FrameworkElement;

                    if (Document.MapScreenEntities.ContainsKey(fe.Name) && 
                        !Document.MapScreenEntities[fe.Name].SourceSymbolLinked)
                    {
                        var list = Document.GetListInners(fe.Name);
                        if (list.Count == 0 || Utilities.WPF.XmlHelper.IsProblematicXamlWriter(fe))
                        {
                            if (!mapEditingElements.ContainsKey(el))
                            {
                                String xamlData = el.XamlWriterFormatted();
                                mapEditingElements.Add(el, xamlData);
                            }
                        }
                        else
                        {
                            foreach (var name in list)
                            {
                                if (Document.MapScreenEntities.ContainsKey(name) &&
                                    !Document.MapScreenEntities[name].SourceSymbolLinked)
                                {
                                    var control = Document.FindInnerControl(MainSurface, name) as UIElement;
                                    if (control != null && !Utilities.WPF.XmlHelper.IsProblematicXamlWriter(control))
                                    {
                                        try
                                        {
                                            if (!mapEditingElements.ContainsKey(control))
                                            {
                                                String xamlData = control.XamlWriterFormatted();
                                                mapEditingElements.Add(control, xamlData);
                                            }
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                    }

                    var sizeChangedInvoker = new DelayedSingleActionInvoker(
                        new Action(() => 
                        {
                            if (SelectedElementsMap.ContainsKey(fe))
                            {
                                //SelectedElementsMap[fe].Deactivate();
                                //SelectedElementsMap[fe].Activate();
                                SelectedElementsMap[fe].InvalidateMeasure();
                                SelectedElementsMap[fe].InvalidateArrange();
                            }

                            var entity = GetSelectedEntity(fe);
                            if (entity != null)
                            {
                                entity.PreservedWidth = fe.Height;
                                entity.PreservedWidth = fe.Width;
                                MainSurface_SetModified(true);
                            }
                        }), TimeSpan.FromMilliseconds(50));

                    if (propertyChangeNotifierList == null)
                        propertyChangeNotifierList = new List<PropertyChangeNotifier>();
                    var notifier = new PropertyChangeNotifier(fe, FrameworkElement.WidthProperty);
                    notifier.ValueChanged += (o, e) =>
                    {
                        sizeChangedInvoker.BeginInvoke();
                    };
                    propertyChangeNotifierList.Add(notifier);
                    notifier = new PropertyChangeNotifier(fe, FrameworkElement.HeightProperty);
                    notifier.ValueChanged += (o, e) =>
                    {
                        sizeChangedInvoker.BeginInvoke();
                    };
                    propertyChangeNotifierList.Add(notifier);
                    notifier = new PropertyChangeNotifier(fe, Canvas.LeftProperty);
                    notifier.ValueChanged += (o, e) =>
                    {
                        sizeChangedInvoker.BeginInvoke();
                    };
                    propertyChangeNotifierList.Add(notifier);
                    notifier = new PropertyChangeNotifier(fe, Canvas.TopProperty);
                    notifier.ValueChanged += (o, e) =>
                    {
                        sizeChangedInvoker.BeginInvoke();
                    };
                    propertyChangeNotifierList.Add(notifier);
                }
            }
        }

        public void CancelEdit()
        {
            MainSurface_CleanCurrentSelectedion();
            undoredo.CancelEdit();
            if (propertyChangeNotifierList != null)
            {
                propertyChangeNotifierList.ForEach(c => c.Dispose());
                propertyChangeNotifierList.Clear();
            }
        }

        public void EndEdit()
        {
            if (undoredo == null || Document == null || bDisposed)
                return;

            undoredo.EndEdit();
            if (propertyChangeNotifierList != null)
            {
                propertyChangeNotifierList.ForEach(c => c.Dispose());
                propertyChangeNotifierList.Clear();
            }

            var listSelected = MainSurface_GetCurrentSelection();
            listSelected.ForEach(el =>
            {
                var fe = listSelected[0] as FrameworkElement;
                var entity = GetSelectedEntity(fe);

                Type t = fe.GetType();
                p = t.GetProperty("Text");
                if (p != null && !(fe is ComboBox) && !(fe is ListBox))
                {
                    RestoreUntranslated(fe);
                    entity.PreservedText = p.GetValue(fe) as String;
                    ElementToCurrentLanguage(fe);
                }
                else
                {
                    var contentControl = fe as ContentControl;
                    if (contentControl != null)
                    {
                        if (contentControl.Content is String)
                        {
                            RestoreUntranslated(fe);
                            entity.PreservedText = contentControl.Content as String;
                            ElementToCurrentLanguage(fe);
                        }
                        else if(!(fe is HeaderedContentControl) && !(contentControl.Content is String))
                        {
                            UIElement ue = (UIElement)fe;

                            var currentList = (from c in ue.GetVisualChildrenOfType<ContentControl>()
                                           where c.Content is String && c.Visibility == System.Windows.Visibility.Visible
                                           select c).FirstOrDefault();
                            if (currentList != null)
                            {
                                RestoreUntranslated(fe);
                                entity.PreservedText = currentList.Content as String;
                                ElementToCurrentLanguage(fe);
                            }
                        }
                        else if(fe is HeaderedContentControl &&
                                (fe as HeaderedContentControl).Header is String ||
                                (fe as HeaderedContentControl).Header == null) 
                        {
                            RestoreUntranslated(fe);
                            entity.PreservedText = (fe as HeaderedContentControl).Header as String;
                            ElementToCurrentLanguage(fe);
                        }
                    }
                }

                if (fe is Panel)
                    Document.SetEntityPreservedBrush(entity, (fe as Panel).Background);
                else if (fe is Microsoft.Expression.Media.IShape)
                    Document.SetEntityPreservedBrush(entity, (fe as Microsoft.Expression.Media.IShape).Fill);
                else if (fe is Control)
                    Document.SetEntityPreservedBrush(entity, (fe as Control).Background);
                else if (fe is Border)
                    Document.SetEntityPreservedBrush(entity, (fe as Border).Background);
                else if (fe is Shape)
                    Document.SetEntityPreservedBrush(entity, (fe as Shape).Fill);

                if (fe is Microsoft.Expression.Media.IShape)
                    Document.SetEntityPreservedPen(entity, (fe as Microsoft.Expression.Media.IShape).Stroke);
                else if (fe is Control)
                    Document.SetEntityPreservedPen(entity, (fe as Control).Foreground);
                else if (fe is Shape)
                    Document.SetEntityPreservedPen(entity, (fe as Shape).Stroke);

                RestoreUntranslated(fe, false);
                ElementToCurrentLanguage(fe);
            });

            if (!Document.NeedsSave)
            {
                using (var cursor = new WaitCursor())
                {
                    foreach (var el in listSelected)
                    {
                        FrameworkElement fe = el as FrameworkElement;
                        var list = Document.GetListInners(fe.Name);
                        if (!mapEditingElements.ContainsKey(el) && list.Count == 0)
                            continue;

                        if (list.Count == 0)
                        {
                            if (Document.MapScreenEntities.ContainsKey(fe.Name) &&
                                !Document.MapScreenEntities[fe.Name].SourceSymbolLinked)
                            {
                                String xamlData = el.XamlWriterFormatted();
                                if (mapEditingElements[el] != xamlData)
                                {
                                    MainSurface_SetModified(true);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            foreach (var name in list)
                            {
                                if (Document.MapScreenEntities.ContainsKey(name) &&
                                    !Document.MapScreenEntities[name].SourceSymbolLinked)
                                {
                                    var control = Document.FindInnerControl(MainSurface, name) as UIElement;
                                    if (control == null || !mapEditingElements.ContainsKey(control))
                                        continue;

                                    String xamlData = control.XamlWriterFormatted();
                                    if (mapEditingElements[control] != xamlData)
                                    {
                                        MainSurface_SetModified(true);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            mapEditingElements.Clear();
        }

        #endregion IEditableObject Members

        #region Grid Manager

        void ShowGrid(bool bshow)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(MainSurface);
            if (adornerLayer == null)
                return;

            if (bshow)
            {
                //if (_gridAdorner != null)
                //    adornerLayer.Remove(_gridAdorner);
                //_gridAdorner = new GridAdorner(MainSurface, magicSnapNumber);
                //adornerLayer.Add(_gridAdorner);
                if (_gridAdorner != null)
                {
                    _gridAdorner.Visibility = Visibility.Hidden;
                    _gridAdorner.MagicSnapNumber = magicSnapNumber;
                    _gridAdorner.RefreshGridBrush();
                    _gridAdorner.Visibility = Visibility.Visible;
                }
                Document.IsGridMode = true;
            }
            else 
            {
                //if (_gridAdorner != null)
                //{
                //    adornerLayer.Remove(_gridAdorner);
                //    _gridAdorner = null;
                //}
                if (_gridAdorner != null)
                    _gridAdorner.Visibility = Visibility.Hidden;
                Document.IsGridMode = false;
            }

            Document.IsGridMode5 = magicSnapNumber == 5;
            Document.IsGridMode10 = magicSnapNumber == 10;
            Document.IsGridMode20 = magicSnapNumber == 20;
            Document.IsGridMode40 = magicSnapNumber == 40;
            Document.IsGridMode100 = magicSnapNumber == 100;

            //Document.IsGridMode5 = bshow && magicSnapNumber == 5;
            //Document.IsGridMode10 = bshow && magicSnapNumber == 10;
            //Document.IsGridMode20 = bshow && magicSnapNumber == 20;
            //Document.IsGridMode40 = bshow && magicSnapNumber == 40;
            //Document.IsGridMode100 = bshow && magicSnapNumber == 100;

            ApplicationPropertiesHelper.SetProperty("ScreenShowGrid", bshow);
            ApplicationPropertiesHelper.SetProperty("ScreenShowGridSnap", magicSnapNumber);
        }

        bool IsGridDislayed()
        {
            return _gridAdorner != null;
        }

        List<FrameworkElement> listLayoutItems;
        private void OnLayoutEdit(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            using (var cursor = new WaitCursor())
            {
                if (layoutItems.Visibility == Visibility.Collapsed)
                {
                    if (Document.NeedsSave)
                    {
                        if (EditorComponent.UIInterface != null)
                        {
                            var res = EditorComponent.UIInterface.ShowYesNoCancel(String.Format(Properties.Resources.SaveDoc,
                                Document.Title), UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Question);
                            if (res == CustomDialogResults.Cancel)
                                return;
                            if (res == CustomDialogResults.Yes)
                            {
                                SaveCurrentDocument();
                            }
                        }
                    }

                    CancelCurrentSelectedInsertObject();
                    CancelSetZOrder();

                    if (listLayoutItems == null)
                        listLayoutItems = new List<FrameworkElement>();

                    foreach (FrameworkElement uie in MainSurface.Children)
                    {
                        listLayoutItems.Add(uie);
                    }
                    listLayoutItems.ForEach(uie =>
                        {
                            MainSurface.Children.Remove(uie);
                            Utilities.WPF.DependencyObjectExtensions.UnregisterName(MainSurface, uie);
                            layoutItems.AvailableItems.Add(uie);
                            Utilities.WPF.DependencyObjectExtensions.RegisterName(layoutItems, uie);
                        });

                    LoadLayout();
                    layoutItems.Visibility = Visibility.Visible;
                    gridEditor.Visibility = System.Windows.Visibility.Collapsed;

                    // layoutItems.LayoutUpdated += layoutItems_LayoutUpdated;
                    layoutItems.IsCustomizationChanged += layoutItems_IsCustomizationChanged;
                    layoutItems.Controller.ModelChanged += Controller_ModelChanged;
                    layoutItems.IsCustomization = true;
                }
                else
                {
                    if (Document.LayoutChanged)
                    {
                        if (EditorComponent.UIInterface != null)
                        {
                            var res = EditorComponent.UIInterface.ShowYesNoCancel(String.Format(Properties.Resources.SaveDoc,
                                Document.Title), UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Question);
                            if (res == CustomDialogResults.Cancel)
                                return;
                            if (res == CustomDialogResults.Yes)
                            {
                                SaveCurrentDocument();
                            }
                        }
                    }

                    layoutItems.IsCustomization = false;
                    // layoutItems.LayoutUpdated -= layoutItems_LayoutUpdated;
                    layoutItems.IsCustomizationChanged -= layoutItems_IsCustomizationChanged;
                    layoutItems.Controller.ModelChanged -= Controller_ModelChanged;
                    layoutItems.Visibility = Visibility.Collapsed;
                    if (listLayoutItems != null)
                    {
                        listLayoutItems.ForEach(uie =>
                        {
                            if (layoutItems.AvailableItems.Contains(uie))
                                layoutItems.AvailableItems.Remove(uie);
                            else
                            {
                                var parent = VisualTreeHelper.GetParent(uie);
                                if (parent is Panel)
                                    (parent as Panel).Children.Remove(uie);
                                else if (parent is ContentControl)
                                    (parent as ContentControl).Content = null;
                                else if (parent is ItemsControl)
                                    (parent as ItemsControl).Items.Remove(uie);
                                else if (parent is Border)
                                    (parent as Border).Child = null;
                            }

                            Utilities.WPF.DependencyObjectExtensions.UnregisterName(layoutItems, uie);
                            MainSurface.Children.Add(uie);
                            Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, uie);
                        });
                        layoutItems.Children.Clear();
                        listLayoutItems.Clear();
                    }
                    gridEditor.Visibility = System.Windows.Visibility.Visible;
                }

                Document.IsLayouEditMode = layoutItems.Visibility == Visibility.Visible;
            }
        }

        /*
        private void layoutItems_LayoutUpdated(object sender, EventArgs e)
        {
            // force IsCustomization because DevExpress when executing OnLayoutUpdate set IsCustomization to 'false'.
            // see https://support.progea.com/Products/default.asp?11207
            layoutItems.IsCustomization = true;
            Document.LayoutChanged = true;
        }
        */

        bool bLayoutChanged;
        void Controller_ModelChanged(object sender, DevExpress.Xpf.LayoutControl.LayoutControlModelChangedEventArgs e)
        {
            bLayoutChanged = true;
            Document.LayoutChanged = true;
        }

        private void layoutItems_IsCustomizationChanged(object sender, EventArgs e)
        {
            if (layoutItems.IsCustomization)
            {
                layoutItems.Controller.CustomizationController.SelectionChanged += layoutItems_SelectionChanged;
            }
            else
            {
                layoutItems.Controller.CustomizationController.SelectionChanged -= layoutItems_SelectionChanged;
            }
        }

        void layoutItems_SelectionChanged(object sender, DevExpress.Xpf.LayoutControl.LayoutControlSelectionChangedEventArgs e)
        {
            foreach (var element in e.SelectedElements)
            {
                if (element.ReadLocalValue(DevExpress.Xpf.LayoutControl.LayoutControl.AllowHorizontalSizingProperty) == DependencyProperty.UnsetValue)
                    DevExpress.Xpf.LayoutControl.LayoutControl.SetAllowHorizontalSizing(element, false);
                if (element.ReadLocalValue(DevExpress.Xpf.LayoutControl.LayoutControl.AllowVerticalSizingProperty) == DependencyProperty.UnsetValue)
                    DevExpress.Xpf.LayoutControl.LayoutControl.SetAllowVerticalSizing(element, false);
            }

            if (e.SelectedElements.Count == 1 && !ReferenceEquals(e.SelectedElements[0], layoutItems))
                EditorComponent.Workspace.ContextObject = e.SelectedElements[0];
            else
                EditorComponent.Workspace.ContextObjects = e.SelectedElements;
        }

        private void OnToggleGrid(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            ShowGrid(!Document.IsGridMode);
        }

        void CanToggleGrid(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnSetGrid(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            if (e.Parameter == null)
                magicSnapNumber = 2;

            double number; 
            Double.TryParse(e.Parameter as String, out number);
            number = Math.Max(number, 2);
            magicSnapNumber = number;
            OnPropertyChanged("GridSize");

            ShowGrid(Document.IsGridMode);
        }

        void CanSetGrid(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnSnapToGrid(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            bSnapToGrid = !bSnapToGrid;
            Document.IsSnapToGrid = bSnapToGrid;

            ApplicationPropertiesHelper.SetProperty("ScreenSnapToGrid", bSnapToGrid);
        }

        void CanSnapToGrid(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnSmartSnap(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            bSmartSnapToObjects = !bSmartSnapToObjects;
            Document.IsSmartSnap = bSmartSnapToObjects;

            ApplicationPropertiesHelper.SetProperty("ScreenSmartSnap", bSmartSnapToObjects);
        }

        void CanSmartSnap(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        void RestoreGridSettings()
        {
            if (Document == null)
                return;

            bSmartSnapToObjects = Document.IsSmartSnap = ApplicationPropertiesHelper.GetProperty<bool>("ScreenSmartSnap", true);
            bSnapToGrid = Document.IsSnapToGrid = ApplicationPropertiesHelper.GetProperty<bool>("ScreenSnapToGrid", true);

            var show = ApplicationPropertiesHelper.GetProperty<bool>("ScreenShowGrid");
            magicSnapNumber = ApplicationPropertiesHelper.GetProperty<double>("ScreenShowGridSnap");
            magicSnapNumber = Math.Max(magicSnapNumber, 1);
            OnPropertyChanged("GridSize");
            Document.IsGridMode = show;
            ShowGrid(show);
        }

        bool bSmartSnapToObjects = false;
        bool bSnapToGrid = false;
        public void AdjustPointToGrid(ref Point point)
        {
            if (!bSnapToGrid)
                return;
            point.X = Math.Round(point.X / magicSnapNumber, 0) * magicSnapNumber;
            point.Y = Math.Round(point.Y / magicSnapNumber, 0) * magicSnapNumber;
        }

        #endregion Grid Manager

        #region IGridViewInfoService Members

        public void ActivateProperty()
        {
            if (EditorComponent.PropertyControl == null)
                return;

            EditorComponent.PropertyControl.Activate();
        }

        public void ActivateAnimationExplorer()
        {
            if (EditorComponent.AnimationExplorer == null)
                return;

            EditorComponent.AnimationExplorer.Activate();
        }

        public void ActivateCommandExplorer()
        {
            if (EditorComponent.CommandExplorer == null)
                return;

            EditorComponent.CommandExplorer.Activate();
        }

        public void DragMultipleSelection(double h, double v, UIElement current)
        {
            List<UIElement> selectedElements = MainSurface_GetCurrentSelection();
            if (selectedElements.Contains(current))
                selectedElements.Remove(current);
            selectedElements.ForEach(item =>
                {
                    if (SelectedElementsMap[item] is BasicPointAdorner)
                        SelectedElementsMap[item].Deactivate();

                    double left = Canvas.GetLeft(item) + h;
                    double top = Canvas.GetTop(item) + v;

                    Canvas.SetLeft(item, left);
                    Canvas.SetTop(item, top);

                    if (SelectedElementsMap[item] is BasicPointAdorner)
                        SelectedElementsMap[item].Activate();
                });

            MoveDraggingPoints(listStartingPoint.Keys.ToList(), h, v, true);
            MoveDraggingPoints(listEndingPoint.Keys.ToList(), h, v, false);

            UpdateSelectionMeasureBar();
        }

        void MoveDraggingPoints(List<UIElement> elements, double h, double v, bool bStartingPoint = true)
        {
            if (elements == null)
                return;

            if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
                return;

            elements.ForEach(el =>
            {
                var propertyPoints = el.GetType().GetProperty("Points");
                if (propertyPoints != null || el is Line)
                {
                    if (el is Line)
                    {
                        var line = el as Line;
                        if (bStartingPoint)
                        {
                            line.X1 += h;
                            line.Y1 += v;
                        }
                        else
                        {
                            line.X2 += h;
                            line.Y2 += v;
                        }
                    }
                    else
                    {
                        var points = propertyPoints.GetValue(el) as PointCollection;
                        if (bStartingPoint)
                        {
                            var pt = new Point(points[0].X + h, points[0].Y + v);
                            points.RemoveAt(0);
                            points.Insert(0, pt);
                        }
                        else
                        {
                            var pt = new Point(points[points.Count - 1].X + h, points[points.Count - 1].Y + v);
                            points.RemoveAt(points.Count - 1);
                            points.Add(pt);
                        }

                        propertyPoints.SetValue(el, points);
                    }
                }
            });
        }

        void MoveDraggingPoints(Dictionary<UIElement, Point> elements, bool bStartingPoint = true)
        {
            if (elements == null || referencePointElement == null)
                return;

            //if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
            //    return;

            FrameworkElement fe = referencePointElement as FrameworkElement;
            var bound = new Rect(Canvas.GetLeft(referencePointElement), Canvas.GetTop(referencePointElement), fe.ActualWidth, fe.ActualHeight);

            elements.Keys.ToList().ForEach(el =>
            {
                var propertyPoints = el.GetType().GetProperty("Points");
                if (propertyPoints != null || el is Line)
                {
                    var x = bound.Left + elements[el].X - Canvas.GetLeft(el);
                    var y = bound.Top + elements[el].Y - Canvas.GetTop(el);

                    if (el is Line)
                    {
                        var line = el as Line;
                        if (bStartingPoint)
                        {
                            line.X1 = x;
                            line.Y1 = y;
                        }
                        else
                        {
                            line.X2 = x;
                            line.Y2 = y;
                        }
                    }
                    else
                    {
                        var points = propertyPoints.GetValue(el) as PointCollection;
                        if (bStartingPoint)
                        {
                            points.RemoveAt(0);
                            points.Insert(0, new Point(x, y));
                        }
                        else
                        {
                            points.RemoveAt(points.Count - 1);
                            points.Add(new Point(x, y));
                        }

                        propertyPoints.SetValue(el, points);
                    }
                }
            });
        }

        Dictionary<UIElement, Point> listStartingPoint;
        Dictionary<UIElement, Point> listEndingPoint;
        UIElement referencePointElement;
        void PrepareDraggingPoints(UIElement element)
        {
            if (listStartingPoint == null)
                listStartingPoint = new Dictionary<UIElement, Point>();
            else
                listStartingPoint.Clear();

            if (listEndingPoint == null)
                listEndingPoint = new Dictionary<UIElement, Point>();
            else
                listEndingPoint.Clear();

            if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
                return;

            referencePointElement = element;
            FrameworkElement fe = element as FrameworkElement;
            var bound = new Rect(Canvas.GetLeft(element), Canvas.GetTop(element), fe.ActualWidth, fe.ActualHeight);

            foreach (UIElement el in MainSurface.Children)
            {
                var propertyPoints = el.GetType().GetProperty("Points");
                if (propertyPoints != null || el is Line)
                {
                    var points = new PointCollection();
                    if (el is Line)
                    {
                        var line = el as Line;
                        points.Add(new Point(line.X1, line.Y1));
                        points.Add(new Point(line.X2, line.Y2));
                    }
                    else
                        points = propertyPoints.GetValue(el) as PointCollection;

                    if (points != null && points.Count > 1)
                    {
                        var ptOffsetStarting = new Point(Canvas.GetLeft(el) + points[0].X, Canvas.GetTop(el) + points[0].Y);
                        var ptOffsetEnding = new Point(Canvas.GetLeft(el) + points[points.Count - 1].X, 
                                                        Canvas.GetTop(el) + points[points.Count - 1].Y);

                        if (bound.Contains(ptOffsetStarting))
                            listStartingPoint.Add(el, new Point(ptOffsetStarting.X - bound.Left, ptOffsetStarting.Y - bound.Top));
                        if (bound.Contains(ptOffsetEnding))
                            listEndingPoint.Add(el, new Point(ptOffsetEnding.X - bound.Left, ptOffsetEnding.Y - bound.Top));
                    }
                }
            }
        }

        public void GridServiceObjectResized()
        {
            MoveDraggingPoints(listStartingPoint, true);
            MoveDraggingPoints(listEndingPoint, false);
        }

        List<Rect> listSnapLineRects = new List<Rect>();
        public void PrepareControlRectangleForSnapLines(UIElement exludeme)
        {
            List<UIElement> selectedElements = MainSurface_GetCurrentSelection();

            listSnapLineRects.Clear();
            foreach (UIElement uie in MainSurface.Children)
            {
                if (selectedElements.Contains(uie) || uie == exludeme)
                    continue;

                FrameworkElement fe = uie as FrameworkElement;
                listSnapLineRects.Add(
                    new Rect(Canvas.GetLeft(uie), Canvas.GetTop(uie), fe.ActualWidth, fe.ActualHeight));
            }

            PrepareDraggingPoints(exludeme);
        }

        double magicSnapNumber = 10;

        public bool IsPointOnSnapLineHorizontal(double h)
        {
            foreach (var rect in listSnapLineRects)
            {
                if (Math.Abs(h - rect.Left) <= magicSnapNumber)
                    return true;

                if (Math.Abs(h - rect.Right) <= magicSnapNumber)
                    return true;
            }

            return false;
        }

        public bool IsPointOnSnapLineVertical(double v)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                var selectedElements = MainSurface_GetCurrentSelection();
                if (selectedElements.Count == 1)
                {
                    var fe = selectedElements[0] as FrameworkElement;
                    var entity = GetSelectedEntity(fe);
                    if (!Double.IsNaN(entity.LinkedAspectRatio) && entity.LinkedAspectRatio > 0)
                    {
                        fe.Width = fe.Height * entity.LinkedAspectRatio;
                    }
                }
            }

            foreach (var rect in listSnapLineRects)
            {
                if (Math.Abs(v - rect.Top) <= magicSnapNumber)
                    return true;

                if (Math.Abs(v - rect.Bottom) <= magicSnapNumber)
                    return true;
            }

            return false;
        }

        public double SnapPointToGridSizeHorizontal(double h)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                var selectedElements = MainSurface_GetCurrentSelection();
                if (selectedElements.Count == 1)
                {
                    var fe = selectedElements[0] as FrameworkElement;
                    var entity = GetSelectedEntity(fe);
                    if (!Double.IsNaN(entity.LinkedAspectRatio) && entity.LinkedAspectRatio > 0)
                    {
                        fe.Height = fe.Width / entity.LinkedAspectRatio;
                    }
                }
            }

            if (bSmartSnapToObjects)
            {
                foreach (var rect in listSnapLineRects)
                {
                    if (Math.Abs(h - rect.Left) <= magicSnapNumber)
                        return rect.Left;

                    if (Math.Abs(h - rect.Right) <= magicSnapNumber)
                        return rect.Right;
                }
            }

            if (bSnapToGrid)
                h = Math.Round(h / magicSnapNumber, 0) * magicSnapNumber;

            UpdateSelectionMeasureBar();
            return h;
        }

        public double SnapPointToGridSizeVertical(double v)
        {
            if (bSmartSnapToObjects)
            {
                foreach (var rect in listSnapLineRects)
                {
                    if (Math.Abs(v - rect.Top) <= magicSnapNumber)
                        return rect.Top;

                    if (Math.Abs(v - rect.Bottom) <= magicSnapNumber)
                        return rect.Bottom;
                }
            }

            if (bSnapToGrid)
                v = Math.Round(v / magicSnapNumber, 0) * magicSnapNumber;

            UpdateSelectionMeasureBar();
            return v;
        }

        public Point SnapPointToGridSize(Point pt)
        {
            AdjustPointToGrid(ref pt);
            return pt;
        }

        public Point GetMousePosition()
        {
            Point pos = Mouse.GetPosition(MainSurface);
            AdjustPointToGrid(ref pos);
            return pos;
        }

        public Double GridSnapNumber
        {
            get
            {
                return magicSnapNumber;
            }
        }

        public Double GridSize
        {
            get
            {
                return magicSnapNumber;
            }
            set
            {
                if (value == magicSnapNumber || value < 0)
                    return;
                if (value < 0)
                    value = 0;
                magicSnapNumber = value;
                Document.IsGridMode = magicSnapNumber > 1;
                ShowGrid(Document.IsGridMode);

                OnPropertyChanged("GridSize");
            }
        }

        #endregion IGridViewInfoService Members

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                var e = new PropertyChangedEventArgs(propertyName);
                // If the subscriber is a DispatcherObject and different thread
                if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                {
                    // Invoke handler in the target dispatcher's thread
                    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                }
                else // Execute handler as is
                    handler(this, e);
            }
        }

        protected void NotifyPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion INotifyPropertyChanged Members

        #region Layout Persistance

        void SaveLayout()
        {
            try
            {
                using (var stream = new FileStream(ScreenDocument.GetLayoutFileName(Document.FullPath), FileMode.Create))
                {
                    var settings = new XmlWriterSettings
                    {
                        Indent = true,
                        OmitXmlDeclaration = false,
                        Encoding = Encoding.UTF8
                    };

                    using (var writer = XmlWriter.Create(stream, settings))
                    {
                        layoutItems.WriteToXML(writer);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        void LoadLayout()
        {
            try
            {
                using (var stream = new FileStream(ScreenDocument.GetLayoutFileName(Document.FullPath), FileMode.OpenOrCreate))
                {
                    XmlReaderSettings settings = new XmlReaderSettings
                    {
                        ConformanceLevel = ConformanceLevel.Document,
                        CloseInput = true
                    };

                    using (var reader = XmlReader.Create(stream, settings))
                    {
                        layoutItems.ReadFromXML(reader);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        #endregion


        //readonly String StoreFileName = String.Format("{0}.dat", System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));

        //static IsolatedStorageFile GetStorage()
        //{
        //    return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
        //}

        #region IDisposable Members

        bool bDisposed = false;

        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            EditorComponent.Workspace.ContextObjects = null;
            EditorComponent.Workspace.ContextObject = null;
            EditorComponent.Workspace.DockItemRestored -= Workspace_DockItemRestored;

            Document.UnsubscribeToDocumentChangeEntries();

            ScreenEditorControl_Unloaded(null, null);

            MainSurface_CleanCurrentSelectedion();
            MainSurface_CleanWebHMIState();

            MainSurface.Dispose();

            undoredo.Dispose();
            
            if (propertyChangeNotifierList != null)
            {
                propertyChangeNotifierList.ForEach(c => c.Dispose());
                propertyChangeNotifierList.Clear();
                propertyChangeNotifierList = null;
            }

            //MainSurface.SelectionChanged -= MainSurface_SelectionChanged;
            //MainSurface.Strokes.StrokesChanged -= Strokes_StrokesChanged;

            if (smartPropertiesControl != null)
            {
                //btnExplorer.IsChecked = false;
                smartPropertiesControl.Dispose();
            }

            if(StringEditor != null)
                StringEditor.LocalesChanged -= OnLocalesChanged;
            // btnOverview.IsChecked = false;

            //MainSurface.ManipulationStarting -= Window_ManipulationStarting;
            //MainSurface.ManipulationDelta -= Window_ManipulationDelta;
            //MainSurface.ManipulationInertiaStarting -= Window_InertiaStarting;
            //MainSurface.ManipulationCompleted -= Window_ManipulationCompleted;

            _Document = null;

            //EditorComponent.Workspace.ContextObjects = null;
            //EditorComponent.Workspace.ContextObject = null;
        }

        #endregion IDisposable Members

        //void ApplyDynamicElements()
        //{
        //    foreach (var element in Document.DynamicElements())
        //    {
        //        if (element == null)
        //            continue;
        //        FrameworkElement fe = MainSurface.FindName(element) as FrameworkElement;
        //        if (fe == null)
        //            continue;

        //        ApplyDynamicElement(fe);
        //    }
        //}

        //Dictionary<UIElement, DynamicAdorner> DynamicElementsMap = new Dictionary<UIElement, DynamicAdorner>();
        //void ApplyDynamicElement(FrameworkElement element)
        //{
        //    if (Document.IsDynamicEntity(element))
        //    {
        //        if (!DynamicElementsMap.ContainsKey(element))
        //        {
        //            var ad = new DynamicAdorner(element);
        //            AdornerLayer.GetAdornerLayer(element).Add(ad);
        //            DynamicElementsMap.Add(element, ad);
        //        }
        //    }
        //    else if (DynamicElementsMap.ContainsKey(element))
        //    {
        //        AdornerLayer.GetAdornerLayer(element).Remove(DynamicElementsMap[element]);
        //        DynamicElementsMap.Remove(element);
        //    }
        //}

        /*
        static bool IsLayerElement(Canvas uie)
        {
            return uie.Tag is String && (uie.Tag as String).Contains("Layer");
        }

        void SetActiveLayer(Canvas layer)
        {
            ActiveLayer = layer;
        }

        void FillLayerCollection()
        {
            Layers.Clear();
            var list = (from c in MainSurface.Children.OfType<Canvas>() where IsLayerElement(c) select c).ToList();
            list.ForEach(layer =>
                {
                    Layers.Add(layer);
                });
        }

        ObservableCollection<Canvas> Layers = new ObservableCollection<Canvas>();
        void CreateNewLayer()
        {
            var newLayer = new Canvas()
                                        {
                                            Name = "Layer",
                                            Background = Brushes.Transparent,
                                            Focusable = false,
                                        };
            var bindingWidth = new Binding()
                                {
                                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UIElement), 1),
                                    Path = new PropertyPath("ActualWidth")
                                };
            var bindingHeight = new Binding()
                                {
                                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(UIElement), 1),
                                    Path = new PropertyPath("ActualHeight")
                                };
            newLayer.SetBinding(FrameworkElement.WidthProperty, bindingWidth);
            newLayer.SetBinding(FrameworkElement.HeightProperty, bindingHeight);

            MainSurface.Children.Add(newLayer);
            Utilities.WPF.DependencyObjectExtensions.RegisterName(MainSurface, newLayer);
            newLayer.Tag = newLayer.Name;
            SetActiveLayer(newLayer);

            MainSurface_SetModified(true);
        }

        private void btnAddLayer_Click(object sender, RoutedEventArgs e)
        {
            CreateNewLayer();
            FillLayerCollection();
        }
        */

        #region Manipulation management

        Effect savedEffect;
        int savedZIndex;
        void Window_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = MainSurface;
            // e.Handled = true;
            if (savedEffect != null)
                return;

            var uie = e.OriginalSource as UIElement;
            if (uie != null)
            {
                uie.RenderTransformOrigin = new Point(0.5, 0.5);
                savedZIndex = Canvas.GetZIndex(uie);

                Canvas.SetZIndex(uie, MainSurface.Children.Count - 1);

                var effect = new DropShadowEffect
                {
                    ShadowDepth = 50,
                    Opacity = 0.5
                };
                savedEffect = uie.Effect;
                uie.Effect = effect;
            }
        }

        private void Window_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            var uie = e.OriginalSource as UIElement;
            if (uie != null)
            {
                Canvas.SetZIndex(uie, savedZIndex);
                uie.Effect = savedEffect;
            }
            savedEffect = null;
            // e.Handled = true;
        }

        void Window_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            var element = e.OriginalSource as FrameworkElement;
            if (element == null)
                return;

            RotateTransform rotation = Utilities.Animations.Animations.SetTransform<RotateTransform>(element, true, true);
            ScaleTransform scale = Utilities.Animations.Animations.SetTransform<ScaleTransform>(element, true, true);
            TranslateTransform translation = Utilities.Animations.Animations.SetTransform<TranslateTransform>(element, true, true);

            var container = (FrameworkElement)e.ManipulationContainer;
            var transformGroup = element.RenderTransform as TransformGroup;
            Rect containerBounds = new Rect(container.RenderSize);
            // Rect objectBounds = transformGroup.TransformBounds(new Rect(element.RenderSize));
            List<UIElement> list = new List<UIElement>();
            list.Add(element);
            Rect objectBounds = Utilities.WPF.DependencyObjectExtensions.CalculateBoundRect(list, MainSurface);

            e.Handled = true;
            if (e.IsInertial && !containerBounds.Contains(objectBounds))
            {
                e.ReportBoundaryFeedback(e.DeltaManipulation);
                e.Complete();
            }
            else
            {
                // the center never changes in this sample, although we always compute it.
                Point center = new Point(element.ActualWidth / 2.0, element.ActualHeight / 2.0);

                // apply the rotation at the center of the rectangle if it has changed
                rotation.CenterX = center.X;
                rotation.CenterY = center.Y;
                rotation.Angle += e.DeltaManipulation.Rotation;

                // Scale is always uniform, by definition, so the x and y will always have the same magnitude if it has changed

                scale.CenterX = center.X;
                scale.CenterY = center.Y;
                scale.ScaleX *= e.DeltaManipulation.Scale.X;
                scale.ScaleY *= e.DeltaManipulation.Scale.Y;

                translation.X += e.DeltaManipulation.Translation.X;
                translation.Y += e.DeltaManipulation.Translation.Y;
            }
        }

        void Window_InertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            // Decrease the velocity of the Rectangle's movement by
            // 10 inches per second every second.
            // (10 inches * 96 DIPS per inch / 1000ms^2)
            e.TranslationBehavior = new InertiaTranslationBehavior()
            {
                InitialVelocity = e.InitialVelocities.LinearVelocity,
                DesiredDeceleration = 10.0 * 96.0 / (1000.0 * 1000.0)
            };

            // Decrease the velocity of the Rectangle's resizing by
            // 0.1 inches per second every second.
            // (0.1 inches * 96 DIPS per inch / (1000ms^2)
            e.ExpansionBehavior = new InertiaExpansionBehavior()
            {
                InitialVelocity = e.InitialVelocities.ExpansionVelocity,
                DesiredDeceleration = 0.1 * 96 / 1000.0 * 1000.0
            };

            // Decrease the velocity of the Rectangle's rotation rate by
            // 2 rotations per second every second.
            // (2 * 360 degrees / (1000ms^2)
            e.RotationBehavior = new InertiaRotationBehavior()
            {
                InitialVelocity = e.InitialVelocities.AngularVelocity,
                DesiredDeceleration = 720 / (1000.0 * 1000.0)
            };
            // e.Handled = true;
        }

        #endregion Manipulation management

        Key lastKeyDown = Key.None;
        private void MainSurface_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.LeftShift || 
                e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift &&
                    (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control || 
                    (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    lastKeyDown = e.Key;
                    SelectedElementsMap.Values.ToList().ForEach(ad =>
                        {
                            bool bSet = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift &&
                                        (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
                            ad.SetDraggable(!bSet, !bSet);
                        });
                }
            }
            if (e.Key == Key.OemPlus || e.Key == Key.OemMinus)
            {
                if (!((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt) &&
                    (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    if (e.Key == Key.OemPlus)
                        UIGeneralCommands.ZoomIn.Execute(null, this);
                    else
                        UIGeneralCommands.ZoomOut.Execute(null, this);
                    e.Handled = true;
                }
            }
        }

        private void MainSurface_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == lastKeyDown)
            {
                lastKeyDown = Key.None;
                SelectedElementsMap.Values.ToList().ForEach(ad => ad.SetDraggable(true));
                if (SelectedElementsMap.Count > 0)
                    SelectedElementsMap[SelectedElementsMap.First().Key].SetIsActive(true, SelectedElementsMap.Count > 0);
            }
        }

        private void OnEditExpression(object sender, ExecutedRoutedEventArgs e)
        {
            var listSelected = MainSurface_GetCurrentSelection();
            //menuExpression.IsVisible = listSelected.Count >= 1 ? true : false;
            if (listSelected.Count == 1)
            {
                var fe = listSelected[0] as FrameworkElement;
                var entity = GetSelectedEntity(fe);
                var title = String.Format("{0} - {1}", Properties.Resources.ExpressionEditor, fe.Name);

                BeginEdit();
                var expression = entity.Expression;
                if (EditExpression(ref expression, title))
                {
                    entity.Expression = expression;
                    EndEdit();
                }
                else
                    CancelEdit();
            }
        }

        private void OnEditReverseExpression(object sender, ExecutedRoutedEventArgs e)
        {
            var listSelected = MainSurface_GetCurrentSelection();
            //menuExpression.IsVisible = listSelected.Count >= 1 ? true : false;
            if (listSelected.Count == 1)
            {
                var fe = listSelected[0] as FrameworkElement;
                var entity = GetSelectedEntity(fe);
                var title = String.Format("{0} - {1}", Properties.Resources.ExpressionEditor, fe.Name);
                
                BeginEdit();
                var expression = entity.ReverseExpression;
                if (EditExpression(ref expression, title))
                {
                    entity.ReverseExpression = expression;
                    EndEdit();
                }
                else
                    CancelEdit();
            }
        }

        bool EditExpression(ref String Expression, String title)
        {
            //var map = Document.GetDynamicMapForElementAndChilds();
            //var list = new List<OPCUAEntityReference>();
            //foreach (var el in map.Keys)
            //{
            //    Parallel.ForEach(map[el], item => { item.Name = el; });
            //    list.AddRange(map[el]);
            //}

            //var variables = (from c in list
            //                 orderby c.HumanReadable ascending
            //                 select (System.Text.RegularExpressions.Regex.Split(c.HumanReadable, @" \("))[0]).Distinct().ToList();
            // var variables = (from c in EditorComponent.UFUAEditor.GetFlatListTags(Document).AsParallel() orderby c select c).ToList();
            var varEditor = EditorComponent.UFUAEditor.GetRuntimeAddressSpaceControl(Document);
            if (varEditor == null)
                return false;

            WPFUtilities.PropertyDataTemplate.ExpressonEditor editor = new WPFUtilities.PropertyDataTemplate.ExpressonEditor(varEditor, Expression);
            var Dialog = new GeneralDialogContent(editor)
            {
                Owner = this.FindParent<Window>(),
                Title = title,
                HelpLink = "ExpressionEditor"
            };
            
            Dialog.DialogKeepContent = true;
            if (Dialog.ShowDialog() == true)
            {
                Expression = editor.model.Expression;
                return true;
            } 
            
            return false;
        }

        private void OnFlipVertical(object sender, ExecutedRoutedEventArgs e)
        {
            using (new ChangeScreenWatcher(this, true, UndoRedoManager.UndoAction.Changed))
            {
                var listSelected = MainSurface_GetCurrentSelection();
                listSelected.ForEach(element =>
                    {
                        var t = element.SetTransform<ScaleTransform>(true, true);
                        if (t.ScaleY == -1)
                            t.ScaleY = 1;
                        else
                            t.ScaleY = -1;
                    });
                TranslateShapePoints();
            }
        }

        private void OnFlipHorizontal(object sender, ExecutedRoutedEventArgs e)
        {
            using (new ChangeScreenWatcher(this, true, UndoRedoManager.UndoAction.Changed))
            {
                var listSelected = MainSurface_GetCurrentSelection();
                listSelected.ForEach(element =>
                {
                    var t = element.SetTransform<ScaleTransform>(true, true);
                    if (t.ScaleX == -1)
                        t.ScaleX = 1;
                    else
                        t.ScaleX = -1;
                });
                TranslateShapePoints();
            }
        }

        private void OnDynamicPropertyInspector(object sender, ExecutedRoutedEventArgs e)
        {
            var listSelected = MainSurface_GetCurrentSelection();
            var entity = GetSelectedEntity(listSelected[0] as FrameworkElement);
            var found = (from c in Document.MapScreenEntities where c.Value == entity select c.Key).ToList();
            var ret = Document.GetDynamicEntriesAndInners(found[0]);

            var listScriptables = new List<IScriptable>();
            ret.ForEach(item =>
                {
                    if (!String.IsNullOrEmpty(Document.MapScreenEntities[item].Code))
                    {
                        listScriptables.Add(Document.MapScreenEntities[item]);
                    }
                });
            IDictionary<IScriptable, Grid> scriptControls = null;
            if (listScriptables.Count > 0 && EditorComponent.ScriptExplorer != null)
            {
                scriptControls = EditorComponent.ScriptExplorer.CreateEditControls(listScriptables);
            }

            var userDialog = new ScreenManager.Popups.DynamicPropertyInspector(Document, ret,
                EditorComponent.PropertyControl, scriptControls);
            var Dialog = new GeneralDialogContent(userDialog, bhandleEnterKey: false)
            {
                Owner = this.FindParent<Window>(),
                Title = Properties.Resources.DynamicPropertyInspector,
                HelpLink = "DynamicPropertyInspector"
            };
            BeginEdit();
            listSelected.ForEach(element =>
            {
                var fe = element as FrameworkElement;
                if (fe != null && !(fe is UserControl))
                {
                    var list = fe.GetChildrenOfType<FrameworkElement>();
                    foreach (var el in list)
                    {
                        if (!Utilities.WPF.XmlHelper.IsProblematicXamlWriter(el))
                            continue;

                        var name = GetSelectedEntityName(el);
                        if (!String.IsNullOrEmpty(name))
                            Document.SubscribePropertyChangeXamlWriterProperties(el, name);
                    }
                }
            });
            Dialog.Closing += (o, ev) =>
            {
                if (listScriptables.Count > 0 && EditorComponent.ScriptExplorer != null)
                {
                    EditorComponent.ScriptExplorer.CheckForChanges(listScriptables);
                    EditorComponent.ScriptExplorer.DestroyEditControls(listScriptables);
                }
            };

            var dret = Dialog.ShowDialog();
            if (dret != true)
                CancelEdit();

            listSelected.ForEach(uie =>
            {
                var fe = uie as FrameworkElement;
                if (fe != null && !(fe is UserControl))
                {
                    try
                    {
                        var list = fe.GetChildrenOfType<FrameworkElement>();
                        foreach (var el in list)
                        {
                            if (!Utilities.WPF.XmlHelper.IsProblematicXamlWriter(el))
                                continue;

                            Document.UnsubscribePropertyChangeXamlWriterProperties(el);
                        }
                        if (dret != true)
                            Document.LoadRepositoryItem(this, MainSurface, fe.Name, bSetSize: false, bRecreateResources: true);
                    }
                    catch (Exception ex)
                    {
                        EditorComponent.UIInterface.ShowError(ex.Message);
                    }
                }
            });

            if (dret == true)
                EndEdit();
        }

        private void CanDynamicPropertyInspector(object sender, CanExecuteRoutedEventArgs e)
        {
            var listSelected = MainSurface_GetCurrentSelection();
            if (listSelected.Count == 1 && Document != null && EditorComponent.PropertyControl != null)
            {
                var entity = GetSelectedEntity(listSelected[0] as FrameworkElement);
                var found = (from c in Document.MapScreenEntities where c.Value == entity select c.Key).ToList();
                if (found.Count > 0)
                {
                    var ret = Document.GetDynamicEntriesAndInners(found[0]);
                    e.CanExecute = ret.Count > 0;
                }
            }
        }

        private void OnCommandSelectAll(object sender, ExecutedRoutedEventArgs e)
        {
            using (var cursor = new WaitCursor())
            {
                var listSelected = MainSurface_GetCurrentSelection();
                MainSurface_CleanCurrentSelectedion();
                if (listSelected.Count > 0)
                    return;
                var list = (from c in ActiveLayer.Children.OfType<UIElement>() where c.Visibility != Visibility.Collapsed select c).ToList();
                var readonlyList = new ReadOnlyCollection<UIElement>(list);
                MainSurface_SetCurrentSelection(readonlyList);
            }
        }

        Dictionary<UIElement, Popup> mapElementTreePopup = new Dictionary<UIElement, Popup>();
        private void OnExploreObjectTree(object sender, ExecutedRoutedEventArgs e)
        {
            dockingManagerScreenEditor.ActivateDockItem(ObjectsPanel);
            SelectNode();
        }

        void SelectNode()
        {
            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                var objectBrowserTree = ObjectsPanelTreeContainer.Content as TreeListControl;
                if (objectBrowserTree == null)
                    return;
                var listSelected = MainSurface_GetCurrentSelection();
                if (listSelected.Count == 0)
                    return;
                var item = (from node in objectBrowserTree.View.Nodes
                            where node.Tag == listSelected[0]
                            select node).FirstOrDefault();
                if (item != null)
                    item.ExpandAll();
                else
                    FillVisualTree_Objects();
            });
        }

        void OnTreeNodeChecked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            var rowHandle = (checkbox.TemplatedParent as LightweightCellEditor).RowData.RowHandle.Value;
            var node = activeTree.View.GetNodeByRowHandle(rowHandle);

            var uie = node.Tag as UIElement;
            if (uie != null)
                uie.Visibility = checkbox.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
        }

        void OnCheckboxLoaded(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            var rowHandle = (checkbox.TemplatedParent as LightweightCellEditor).RowData.RowHandle.Value;
            var node = activeTree.View.GetNodeByRowHandle(rowHandle);

            var uie = node.Tag as UIElement;
            if (uie != null)
                checkbox.IsChecked = uie.Visibility == Visibility.Visible ? true : false;
        }

        void OnTreeNodeCollapsing(object sender, DevExpress.Xpf.Grid.TreeList.TreeListNodeAllowEventArgs e)
        {
            //if (!(e.Node.Content as TreeItemControl).IsNodeExpanding)
            //    UpdateFolderIcon(e.Node, false);
        }

        void OnTreeNodeExpanding(object sender, TreeListNodeAllowEventArgs e)
        {
            TreeListNode item = e.Node;

            var tree = (sender as TreeListView).DataControl as TreeListControl;
            if (tree == null)
                return;

            try
            {
                if (item == null || TreeListControlHelper.WasExpanded(item)) //Node's subtree already populated
                    return;

                (item.Content as TreeItemControl).IsNodeExpanding = true;
                tree.BeginDataUpdate();
                item.Nodes.Clear();

                using (var cursor = new WaitCursor())
                {
                    Expander_Expanded(tree, item);
                }
                tree.EndDataUpdate();
            }
            finally
            {
                (item.Content as TreeItemControl).IsNodeExpanding = false;
            }
        }

        class ObjectBrowserTreeItemControl : TreeItemControl
        {
            readonly ObjectBrowseTreeItem obj;
            public bool IsEditable { get; private set; }
            public FrameworkElement ParentTag { get; set; }
            public int ZLayer { get { return obj.ZLayer; } }
            public int VisualCount { get { return obj.VisualCount; } }
            public int LogicCount { get { return obj.LogicCount; } }
            public bool ContainsCode { get { return obj.ContainsCode; } }
            public bool IsDynamic { get { return obj.IsDynamic; } }


            public ObjectBrowserTreeItemControl(object header, ImageSource icon = null, FrameworkElement parentTag = null) : base(header, icon)
            {
                obj = header as ObjectBrowseTreeItem;
                if (obj == null)
                    return;

                ItemHeader = obj.Title;
                IsEditable = obj.IsEditable;
                ParentTag = parentTag;
            }
        }

        void OnRowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            var view = sender as TreeListView;
            var node = view.GetNodeByRowHandle(e.HitInfo.RowHandle);
            if (node == null)
                return;

            ((TreeListControl)view.DataControl).SelectNode(node);

            if (node?.Content is ObjectBrowserTreeItemControl && ((ObjectBrowserTreeItemControl)node.Content).IsEditable)
                Expander_Selected(node, null);
            else if (node?.Content is OPCUAEntityReference)
                SelectElement(((OPCUAEntityReference)node.Content).Name);
        }

        private void CanExploreObjectTree(object sender, CanExecuteRoutedEventArgs e)
        {
            var listSelected = MainSurface_GetCurrentSelection();
            if (listSelected.Count == 1)
            {
                foreach (object sub in LogicalTreeHelper.GetChildren(listSelected[0]))
                {
                    if (!(sub is UIElement))
                        continue;

                    e.CanExecute = true;
                    break;
                }
            }
        }

        public class CustomLinkPreviewModel : LinkPreviewModel
        {
            public CustomLinkPreviewModel(LinkBase link)
            {
                Link = link;
            }
            protected override bool CanPrint(object parameter)
            {
                return false;
            }
        }

        private void OnCommandPrint(object sender, ExecutedRoutedEventArgs e)
        {
            var sl = new SimpleLink();
            sl.DetailCount = 1;
            sl.DetailTemplate = (DataTemplate)Resources["Data"];
            sl.CreateDetail += (o, ev) =>
            {
                var brush = new VisualBrush(MainSurface);
                var visual = new DrawingVisual();
                var context = visual.RenderOpen();

                context.DrawRectangle(brush, null,
                    new Rect(0, 0, MainSurface.ActualWidth, MainSurface.ActualHeight));
                context.Close();

                var bmp = new RenderTargetBitmap((int)MainSurface.ActualWidth,
                    (int)MainSurface.ActualHeight, 96, 96, PixelFormats.Pbgra32);

                bmp.Render(visual);
                ev.Data = bmp;
            };
            sl.CreateDocument(true);
            // using (CustomLinkPreviewModel PreviewModel = new CustomLinkPreviewModel(sl))
            {
                DocumentPreviewWindow preview = new DocumentPreviewWindow() { /*Model = PreviewModel, */Owner = this.FindParent<Window>() };
                preview.PreviewControl.DocumentSource = sl;
                ThemeHelper.SetTheme(preview);
                preview.ShowDialog();
            }
            // sl.ShowPrintPreviewDialog(this.FindParent<Window>());
        }

        private void OnCommandProperties(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            ActivateProperty();
        }

        private void OnShowAdornerExpander(object sender, ExecutedRoutedEventArgs e)
        {
            Document.IsShowAdornerExpander = !Document.IsShowAdornerExpander;
            ApplicationPropertiesHelper.SetProperty("ShowAdornerExpander", Document.IsShowAdornerExpander);
            SelectedElementsMap.Values.ToList().ForEach(ad =>
                {
                    ad.ShowExpander(Document.IsShowAdornerExpander);
                });
        }

        private void OnEditAlias(object sender, ExecutedRoutedEventArgs e)
        {
            var map = Document.MapAlias;
            var listSelected = MainSurface_GetCurrentSelection();
            ScreenEntity entity = null;
            if (listSelected.Count == 1)
            {
                using (var cursor = new WaitCursor())
                {
                    entity = GetSelectedEntity(listSelected[0] as FrameworkElement);
                    map = entity.MapAlias;
                    if (map == null)
                        map = new Dictionary<string, string>();
                    // if (map.Count == 0)
                    {
                        var list = (from c in entity.GetAllSourceEntityReferences()
                                    where AliasHelper.ContainsAlias(c.RelativePath)
                                    select c).ToList();
                        foreach (var opcentity in list)
                        {
                            var ret = AliasHelper.ListAliasFromString(opcentity.RelativePath);
                            ret.ForEach(val =>
                            {
                                if (!map.ContainsKey(val))
                                    map.Add(val, "");
                            });
                        }

                        var listExpressions = (from c in entity.GetAllSourceExpressions()
                                    where AliasHelper.ContainsAlias(c)
                                    select c).ToList();
                        foreach (var expression in listExpressions)
                        {
                            var ret = AliasHelper.ListAliasFromString(expression);
                            ret.ForEach(val =>
                            {
                                if (!map.ContainsKey(val))
                                    map.Add(val, "");
                            });
                        }

                        Document.GetListInners(entity.EntityName).ForEach(I =>
                        {
                            var en = Document.MapScreenEntities[I];
                            list = (from c in en.GetAllSourceEntityReferences()
                                    where AliasHelper.ContainsAlias(c.RelativePath)
                                    select c).ToList();
                            foreach (var opcentity in list)
                            {
                                var ret = AliasHelper.ListAliasFromString(opcentity.RelativePath);
                                ret.ForEach(val =>
                                {
                                    if (!map.ContainsKey(val))
                                        map.Add(val, "");
                                });
                            }

                            listExpressions = (from c in en.GetAllSourceExpressions()
                                                   where AliasHelper.ContainsAlias(c)
                                                   select c).ToList();
                            foreach (var expression in listExpressions)
                            {
                                var ret = AliasHelper.ListAliasFromString(expression);
                                ret.ForEach(val =>
                                {
                                    if (!map.ContainsKey(val))
                                        map.Add(val, "");
                                });
                            }
                        });
                    }
                }
            }
            if (map == null)
                map = new Dictionary<string, string>();

            var editor = new Popups.AliasEditor(map);
            var dlg = new GeneralDialogContent(editor)
            {
                Owner = this.FindParent<Window>(),
                Title = Properties.Resources.AliasEditor,
                HelpLink = "AliasEditor"
            };
            if (dlg.ShowDialog() != true)
            {
                return;
            }
            if (entity != null)
                entity.MapAlias = editor.GetMap();
            else
                Document.MapAlias = editor.GetMap();
        }

        private void CanExecuteEditAlias(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Document != null && SelectedElementsMap.Count <= 1;
        }

        int savecurrentVisibilityLevel;
        private void ResetVisibilityLevel()
        {
            savecurrentVisibilityLevel = CurrentVisibilityLevel;
            CurrentVisibilityLevel = Int32.MaxValue;
            UpdateVisibilityLevel();
        }

        private void RestoreVisibilityLevel()
        {
            CurrentVisibilityLevel = savecurrentVisibilityLevel;
            UpdateVisibilityLevel();
        }

        private void OnEditVisibilityLevel(object sender, ExecutedRoutedEventArgs e)
        {
            if (CurrentVisibilityLevel == -1)
                CurrentVisibilityLevel = 0;
            var mdl = new model() { Value = CurrentVisibilityLevel };
            var editor = new BitMaskEditor() { DataContext = mdl, Type = BitMaskEditor.BitMaskType.Level };
            var dlg = new GeneralDialogContent(editor)
            {
                Owner = this.FindParent<Window>(),
                Title = Properties.Resources.VisibilityLevelEditor,
                HelpLink = "VisibilityLevelEditor"
            };
            if (dlg.ShowDialog() != true)
            {
                return;
            }
            if (mdl.Value.HasValue)
                CurrentVisibilityLevel = mdl.Value.Value;

            MainSurface_CleanCurrentSelectedion();

            UpdateVisibilityLevel();
        }

        private void OnToolbarEditVisibilityLevel(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter != null && e.Parameter.ToString() == Properties.Settings.Default.SplitToolbarButton)
                return;

            if (CurrentVisibilityLevel == -1)
                CurrentVisibilityLevel = 0;

            var chk = (DevExpress.Xpf.Bars.BarCheckItem)e.Parameter;
            var target = (BitMask)Enum.ToObject(typeof(BitMask), CurrentVisibilityLevel);

            CurrentVisibilityLevel = (int)WPFUtilities.Converters.BitMaskConverter.GetNewVisibilityLevel((BitMask)chk.Content, target, chk.IsChecked == true);

            MainSurface_CleanCurrentSelectedion();

            UpdateVisibilityLevel();
        }

        private void UpdateVisibilityLevel()
        {
            var list = (from c in Document.MapScreenEntities where c.Value.VisibilityLevel != Int32.MaxValue && c.Value.VisibilityLevel != 0 select c).ToList();
            list.ForEach(c =>
            {
                var match = CurrentVisibilityLevel & c.Value.VisibilityLevel;
                var control = Document.FindInnerControl(MainSurface, c.Key);
                if (control != null)
                    control.Visibility = match != 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            });
        }

        private void OnClearExpression(object sender, ExecutedRoutedEventArgs e)
        {
            var listSelected = MainSurface_GetCurrentSelection();
            //menuExpression.IsVisible = listSelected.Count >= 1 ? true : false;
            if (listSelected.Count == 1)
            {
                var fe = listSelected[0] as FrameworkElement;
                var entity = GetSelectedEntity(fe);

                BeginEdit();
                entity.Expression = null;
                EndEdit();
            }
        }

        private void OnClearReverseExpression(object sender, ExecutedRoutedEventArgs e)
        {
            var listSelected = MainSurface_GetCurrentSelection();
            //menuExpression.IsVisible = listSelected.Count >= 1 ? true : false;
            if (listSelected.Count == 1)
            {
                var fe = listSelected[0] as FrameworkElement;
                var entity = GetSelectedEntity(fe);

                BeginEdit();
                entity.ReverseExpression = null;
                EndEdit();
            }
        }

        private void OnResetTransormOriginSettings(object sender, ExecutedRoutedEventArgs e)
        {
            SelectedElementsMap.Values.ToList().ForEach(ad =>
            {
                ad.ResetTransormOriginSettings();
            });
        }

        void CanResetTransormOriginSettings(object sender, CanExecuteRoutedEventArgs e)
        {
            CanExecuteSelectedOne(sender, e);
            if(e.CanExecute)
            {
                BasicAdorner ad = SelectedElementsMap.Values.FirstOrDefault();
                if (ad != null)
                    e.CanExecute = ad.IsTransformOriginChanged();
            }
        }

        private void OnShowHideRotationThumb(object sender, ExecutedRoutedEventArgs e)
        {
            SelectedElementsMap.Values.ToList().ForEach(ad =>
            {
                ad.ShowHideRotationThumbs();
            });
        }

        private void OnLockPosition(object sender, ExecutedRoutedEventArgs e)
        {
            SelectedElementsMap.Keys.ToList().ForEach(key =>
            {
                var element = key as FrameworkElement;
                var entity = GetSelectedEntity(element);
                entity.LockMovement = !entity.LockMovement;
                SelectedElementsMap[key].SetLockMovement(entity.LockMovement);
            });
        }

        private void OnEnCacheMode(object sender, ExecutedRoutedEventArgs e)
        {
            SelectedElementsMap.Keys.ToList().ForEach(key =>
            {
                var element = key as FrameworkElement;
                var entity = GetSelectedEntity(element);
                bool bSet = element.CacheMode != null;
                SelectedElementsMap[key].SetCacheMode(!bSet);
                if (sender is MenuItem)
                    (sender as MenuItem).IsChecked = !bSet;
            });
        }

        DispatcherTimer tooltipTimer;
        DispatcherTimer flyoutTimer;
        ScreenEntity lastTooltiped;
        private void ShowToolTip()
        {
            Point endPoint = Mouse.GetPosition(MainSurface);

            FrameworkElement uie = null;
            var hitTestResult = VisualTreeHelper.HitTest(ActiveLayer, endPoint);
            if (hitTestResult != null && hitTestResult.VisualHit != null)
            {
                var parent = hitTestResult.VisualHit as DependencyObject;
                if (parent != null)
                {
                    do
                    {
                        if (ActiveLayer.Children.Contains(parent as FrameworkElement))
                        {
                            uie = parent as FrameworkElement;
                            break;
                        }
                        parent = VisualTreeHelper.GetParent(parent) as DependencyObject;
                    } while (parent != null && parent != ActiveLayer);
                }
            }

            if (uie == null)
            {
                if (tooltipTimer != null)
                    tooltipTimer.Stop();
                tooltipTag.Visibility = Visibility.Collapsed;
                tooltipTag.IsOpen = false;
                tooltipTag.Content = null;
                lastTooltiped = null;
                return;
            }
            var entity = GetSelectedEntity(uie, true);
            if (entity != null && entity.OpcuaEntityReference != null)
            {
                if (lastTooltiped != entity)
                {
                    lastTooltiped = entity;
                    tooltipTag.IsOpen = false;
                    tooltipTag.Content = entity.OpcuaEntityReference.StringRepresentationWithProject;
                    tooltipTag.IsOpen = true;
                    tooltipTag.Visibility = Visibility.Visible;
                    if (tooltipTimer == null)
                    {
                        tooltipTimer = new DispatcherTimer();
                        tooltipTimer.Interval = TimeSpan.FromSeconds(2);
                        tooltipTimer.Tick += (o, ev) =>
                        {
                            tooltipTimer.Stop();
                            tooltipTag.IsOpen = false;
                        };
                        tooltipTimer.Start();
                    }
                    else
                    {
                        tooltipTimer.Stop();
                        tooltipTimer.Start();
                    }
                }
            }
            else
            {
                if (tooltipTimer != null)
                    tooltipTimer.Stop();
                tooltipTag.Visibility = Visibility.Collapsed;
                tooltipTag.IsOpen = false;
                tooltipTag.Content = null;
                lastTooltiped = null;
            }
        }

        private void ObjectsRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (ObjectsPanelTreeContainer.Content != null && !bDisposed)
                FillVisualTree_Objects();
        }

        private void ReferenceRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (entityReferenceList.Content != null && !bDisposed)
                FillVisualTree_References(true);
        }

        private void OnReferencesPanelVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    referenceListTree = TryFindResource("referenceListTree") as TreeListControl;
                    if (referenceListTree == null || entityReferenceList.Content != null)
                        return;

                    referenceListTree.View.Nodes.Clear();
                    //var itemRoot = referenceListTree.AddNode(new ObjectBrowserTreeItemControl(new ObjectBrowseTreeItem(Document.Title, false) { Title = Document.Title }, null), Tag as TreeListNode, Document);

                    UpdateSearchFilterCriteria();
                    FillVisualTree_References(false);

                    entityReferenceList.Content = referenceListTree;
                });
            }
            else
            {
                if (referenceListTree == null)
                    return;
                entityReferenceList.Content = null;

                referenceListTree = null;
            }
        }

        private void OnObjectsPanelVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                objectBrowserTree = TryFindResource("objectBrowserTree") as TreeListControl;
                if (objectBrowserTree == null || ObjectsPanelTreeContainer.Content != null)
                    return;

                objectBrowserTree.View.Nodes.Clear();
                var itemRoot = objectBrowserTree.AddNode(new ObjectBrowserTreeItemControl(new ObjectBrowseTreeItem(Document.Title, false) { Title = Document.Title }, null), Tag as TreeListNode, Document);

                FillVisualTree(itemRoot);

                ObjectsPanelTreeContainer.Content = objectBrowserTree;
            }
            else
            {
                if (objectBrowserTree == null)
                    return;
                ObjectsPanelTreeContainer.Content = null;

                objectBrowserTree = null;
            }
        }

        LayoutGroup PopupsLayoutGroup;
        bool bCustomDocking;
        private void OnDockOperationStarting(object sender, DevExpress.Xpf.Docking.Base.DockOperationStartingEventArgs e)
        {
            if (e.Item is AutoHideGroup && (e.Item as AutoHideGroup).Items.Count > 0 && (PopupsLayoutGroup == null || (!PopupsLayoutGroup.Items.Contains(ReferencesPanel) || !PopupsLayoutGroup.Items.Contains(ObjectsPanel))))
            {
                var innerPanel = (e.Item as AutoHideGroup).Items[0] as DevExpress.Xpf.Docking.LayoutPanel;
                if (innerPanel != null && (innerPanel == ReferencesPanel || innerPanel == ObjectsPanel))
                {
                    if (innerPanel == ReferencesPanel && ObjectsPanel.DockItemState == DockItemState.Docked)
                    {
                        e.Cancel = true;
                        PopupsSideToSide(ReferencesPanel, ObjectsPanel);
                    }
                    else if (innerPanel == ObjectsPanel && ReferencesPanel.DockItemState == DockItemState.Docked)
                    {
                        e.Cancel = true;
                        PopupsSideToSide(ObjectsPanel, ReferencesPanel);
                    }
                }
            }
            if (e.DockOperation == DockOperation.Hide && !bCustomDocking && e.Item.Parent.IsVisible)
            {
                if (e.Item == ObjectsPanel)
                {
                    e.Cancel = true;
                    bCustomDocking = true;
                    dockingManagerScreenEditor.DockController.Hide(ObjectsPanel, ScreenEditorAutoHideGroup_ObjectsPanel);
                    bCustomDocking = false;
                }
                else if (e.Item == ReferencesPanel)
                {
                    e.Cancel = true;
                    bCustomDocking = true;
                    dockingManagerScreenEditor.DockController.Hide(ReferencesPanel, ScreenEditorAutoHideGroup_ReferencesPanel);
                    bCustomDocking = false;
                }
            }
        }

        private void OnDockItemEndDocking(object sender, DevExpress.Xpf.Docking.Base.DockItemDockingEventArgs e)
        {
            if (bCustomDocking || !Enum.TryParse(e.DockType.ToString(), out Dock dock))
                return;

            
            if (e.Item == ObjectsPanel)
                ScreenEditorAutoHideGroup_ObjectsPanel.DockType = dock;
            else if (e.Item == ReferencesPanel)
                ScreenEditorAutoHideGroup_ReferencesPanel.DockType = dock;
        }

        void PopupsSideToSide(DevExpress.Xpf.Docking.LayoutPanel dockingPanel, DevExpress.Xpf.Docking.LayoutPanel dockedPanel)
        {
            bCustomDocking = true;
            try
            {
                if (PopupsLayoutGroup == null)
                {
                    if (rootScreenLayoutGroup.Items.Count > 0)
                        PopupsLayoutGroup = (from BaseLayoutItem g in rootScreenLayoutGroup.Items where g is LayoutGroup && g.Name == "PopupsGroup" select g as LayoutGroup).FirstOrDefault();
                    if (PopupsLayoutGroup == null)
                    {
                        PopupsLayoutGroup = new LayoutGroup() { Name = "PopupsGroup", DestroyOnClosingChildren = false };
                        dockingManagerScreenEditor.LayoutRoot.Add(PopupsLayoutGroup);
                    }
                }

                //if (dockingPanel.Parent is AutoHideGroup && (dockingPanel.Parent as AutoHideGroup).Items.Count == 1)
                //    dockingManagerScreenEditor.AutoHideGroups.Remove((AutoHideGroup)dockingPanel.Parent);
                PopupsLayoutGroup.Items.Add(dockingPanel);

                if (!PopupsLayoutGroup.Items.Contains(dockedPanel))
                {
                    dockingManagerScreenEditor.DockController.RemovePanel(dockedPanel);
                    PopupsLayoutGroup.Items.Add(dockedPanel);
                }
                dockingPanel.AutoHidden = dockedPanel.AutoHidden = true;
                dockingPanel.AutoHidden = dockedPanel.AutoHidden = false;
            }
            finally
            {
                bCustomDocking = false;
            }
        }

        void FlyoutAutoClose()
        {
            if (flyoutTimer == null)
            {
                flyoutTimer = new DispatcherTimer();
                flyoutTimer.Interval = TimeSpan.FromSeconds(2);
                flyoutTimer.Tick += (o, ev) =>
                {
                    flyoutTimer.Stop();
                    referenceFlyoutControl.IsOpen = false;
                    objectsPanelFlyoutControl.IsOpen = false;
                };
                flyoutTimer.Start();
            }
            else
            {
                flyoutTimer.Stop();
                flyoutTimer.Start();
            }
        }

        private void OnDockItemExpanded(object sender, DevExpress.Xpf.Docking.Base.DockItemExpandedEventArgs e)
        {
            if (!(e.Item is AutoHideGroup) || (e.Item as AutoHideGroup).Items.Count < 1)
                return;

            var panel = (e.Item as AutoHideGroup)?.Items[0];
            if (panel == ReferencesPanel && !referenceFlyoutControl.IsOpen)
            {
                referenceFlyoutControl.IsOpen = true;
                FlyoutAutoClose();
            }
            else if (panel == ObjectsPanel && !objectsPanelFlyoutControl.IsOpen)
            {
                objectsPanelFlyoutControl.IsOpen = true;
                FlyoutAutoClose();
            }
        }

        private void OnDockItemCollapsed(object sender, DevExpress.Xpf.Docking.Base.DockItemCollapsedEventArgs e)
        {
            if (!(e.Item is AutoHideGroup) || (e.Item as AutoHideGroup).Items.Count < 1)
                return;

            var panel = (e.Item as AutoHideGroup)?.Items[0];
            if (panel == ReferencesPanel && referenceFlyoutControl.IsOpen)
            {
                flyoutTimer?.Stop();
                referenceFlyoutControl.IsOpen = false;
            }
            else if (panel == ObjectsPanel && objectsPanelFlyoutControl.IsOpen)
            {
                flyoutTimer?.Stop();
                objectsPanelFlyoutControl.IsOpen = false;
            }
        }

        private void OnDockItemClosing(object sender, DevExpress.Xpf.Docking.Base.ItemCancelEventArgs e)
        {
            if (e.Item is FloatGroup)
            {
                e.Cancel = true;
                return;
            }
        }

        void OnAllowPropertyHandler(object sender, AllowPropertyEventArgs e)
        {
            if (e.DependencyProperty == BaseLayoutItem.CaptionProperty)
                e.Allow = false;
        }
    }
    /*
    public class DocumentObject
    {
        public DocumentObject(Canvas surface, ScreenDocument settings)
        {
            _settings = settings;
            _surface = surface;
        }

        ScreenDocument _settings;
        public ScreenDocument Settings
        {
            get
            {
                return _settings;
            }
            set
            {
                if (value == _settings)
                    return;
                _settings = value;
            }
        }

        Canvas _surface;
        public Canvas Surface
        {
            get
            {
                return _surface;
            }
            set
            {
                if (value == _surface)
                    return;
                _surface = value;
            }
        }
    }
    */
    static class EnumerableExtensions
    {
        public static T MaxElement<T, R>(this IEnumerable<T> container, Func<T, R> valuingFoo) where R : IComparable
        {
            var enumerator = container.GetEnumerator();
            if (!enumerator.MoveNext())
                throw new ArgumentException("Container is empty!");

            var maxElem = enumerator.Current;
            var maxVal = valuingFoo(maxElem);

            while (enumerator.MoveNext())
            {
                var currVal = valuingFoo(enumerator.Current);

                if (currVal.CompareTo(maxVal) > 0)
                {
                    maxVal = currVal;
                    maxElem = enumerator.Current;
                }
            }

            return maxElem;
        }
        public static T MinElement<T, R>(this IEnumerable<T> container, Func<T, R> valuingFoo) where R : IComparable
        {
            var enumerator = container.GetEnumerator();
            if (!enumerator.MoveNext())
                throw new ArgumentException("Container is empty!");

            var minElem = enumerator.Current;
            var minVal = valuingFoo(minElem);

            while (enumerator.MoveNext())
            {
                var currVal = valuingFoo(enumerator.Current);

                if (currVal.CompareTo(minVal) < 0)
                {
                    minVal = currVal;
                    minElem = enumerator.Current;
                }
            }

            return minElem;
        }
    }
}
