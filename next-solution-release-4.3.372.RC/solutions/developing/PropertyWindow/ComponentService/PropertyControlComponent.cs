using System;
using UFInterfaces.CoreHostComponents;
using UFInterfaces;
using System.Windows.Threading;
using System.Collections;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.IO;
using System.Windows.Media;
using System.Collections.Generic;
using System.ComponentModel;
using Utilities;
using Utilities.WPF;
using System.Windows.Controls;
using Mindscape.WpfElements.WpfPropertyGrid;
using PropertyControl.PropertyDataTemplate;
using Mindscape.WpfElements.PropertyEditing;
using UriResolver.ComponentService;
using DocumentManager.ComponentService;
using DataReader;
using Converters;
using UIMsgBoxAlertService.ComponentService;
using HelpProvider.ComponentService;
using WPFUtilities.PropertyDataTemplate;
using PropertyControl.Localization;
using StringManager.ComponentService;
using UFInterfaces.PropertyControl;
using CommonControls.PropertyDataTemplate;
using System.Windows.Input;
using UFProjectManager.ComponentService;
using System.Linq;

namespace PropertyControl.ComponentService
{
    public class PropertyControlComponent : ComponentBase<IPropertyControl>, IPropertyControl, IDisposable
    {
        #region Declaration
        Object lockObject = new Object();
        internal PropertyControlUI propertyControlUI;
        Object PromoteSelectingObject;
        IList PromoteSelectingObjects;
        DispatcherOperation IdleExecutionPending;
        bool bLoaded;
        bool bEnableIdleCode;
        bool bPendingIdleCode;
        bool projHasChildren;

        public static EditorCollection propertyEditors = new EditorCollection();

        public static IWorkspace workspace { get; protected set; }
        public static bool workspaceAvailable { get { return workspace != null; } }

        public static PropertyControlComponent propertyControlComponent { get; protected set; }

        public static void QueryInterfaces()
        {
            if (workspace == null)
                workspace = propertyControlComponent.GetService(typeof(IWorkspace)) as IWorkspace;
        }

        IUIMsgBoxAlertService uiInterface;
        public IUIMsgBoxAlertService UIInterface
        {
            get
            {
                if (uiInterface == null)
                    uiInterface = GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                return uiInterface;
            }
        }

        #endregion

        #region IUFInterfaceBase Members

        void IUFInterfaceBase.Initialize()
        {
            if (propertyControlComponent == null)
                propertyControlComponent = this;

            QueryInterfaces();

            if (!workspaceAvailable)
                throw new NotImplementedException("Expecting the missing IWorkpsace Interface");

            workspace.WorkspaceLoading += workspace_WorkspaceLoading;

            CreatePropertyDataTemplates();
        }

        #endregion

        void workspace_WorkspaceLoading(object sender, EventArgs e)
        {
            CreatePropertyControl();
        }

        private static BitmapImage GetControlImage()
        {
            var bm = SharedResources.Helpers.ResourceManager.GetCommonImage("PropertyControl", "PCEditor");
            return bm;
        }

        ContentControl emptyControl;
        public void CreatePropertyControl()
        {
            lock (lockObject)
            {
                if (propertyControlUI != null)
                    return;

                if (emptyControl == null)
                {
                    emptyControl = new ContentControl();
                    if (workspace.HasContentRendered())
                        bEnableIdleCode = true;

                    BitmapImage bm = GetControlImage();

                    // workspace.SetDesiredSideMode(propertyControlUI, "SmartTagsControl");
                    workspace.AddDockingChildren(emptyControl, Properties.Resources.Property_Title, UFInterfaces.DockState.AutoHidden, UFInterfaces.DockSide.Right, false, itemID: nameof(PropertyControlComponent));
                    workspace.SetDockedElementIcon(emptyControl, new ImageBrush(bm));

                    lastDockSide = workspace.GetElementDockSide(emptyControl);

                    workspace.AutoHideAnimationStop += workspace_AutoHideAnimationStop;
                    workspace.AutoHideAnimationStart += workspace_AutoHideAnimationStart;
                    workspace.ContextContentChanging += workspace_ContextContentChanging;
                    workspace.ContextContentChanged += workspace_ContextContentChanged;
                    workspace.ActiveWindowChanged += workspace_ActiveWindowChanged;
                    workspace.DockStateChanged += workspace_DockStateChanged;
                    workspace.ContentRendered += workspace_ContentRendered;

                    var wnd = emptyControl.FindAncestor<Window>();
                    if (wnd != null)
                    {
                        wnd.PreviewKeyDown += emptyControl_PreviewKeyDown;
                        wnd.KeyDown += emptyControl_KeyDown;
                    }
                }
                else
                {
                    using (var cursor = new WaitCursor())
                    {
                        propertyControlUI = new PropertyControlUI();

                        propertyControlUI.Loaded += (o, e) =>
                        {
                            if (bEnableIdleCode)
                            {
                                if (!bLoaded && propertyControlUI != null && propertyControlUI.IsVisible)
                                {
                                    bLoaded = true;
                                    if (workspace.GetElementDockState(emptyControl) == UFInterfaces.DockState.Document)
                                        PromoteCodeToIdle(true);
                                }
                            }
                        };
                        propertyControlUI.Unloaded += (o, e) =>
                        {
                            if (bEnableIdleCode)
                                bLoaded = false;
                        };

                        propertyControlUI.btnFriend.ItemClick += (o, e) =>
                            {
                                UpdateCurrentSelection(propertyControlUI);
                            };

                        workspace.SetDesiredHeightAndWidthInDockedMode(emptyControl, propertyControlUI.Height, propertyControlUI.Width);
                        propertyControlUI.ClearValue(FrameworkElement.WidthProperty);
                        propertyControlUI.ClearValue(FrameworkElement.HeightProperty);

                        emptyControl.Content = propertyControlUI;
                    }
                }
            }
        }

        void emptyControl_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            var treeItem = (e.OriginalSource as DevExpress.Xpf.Grid.TreeListView)?.DataControl.SelectedItem as WPFUtilities.TreeItemControl;

            if (treeItem != null && treeItem.IsOpenable)
                return;

            var focused = Keyboard.FocusedElement;
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift ||
                (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control ||
                (Keyboard.Modifiers & ModifierKeys.Windows) == ModifierKeys.Windows ||
                (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt ||
                focused == null ||
                (focused as System.Windows.Controls.TextBox) != null)
                return;

            workspace.FlashDockedElement(emptyControl);
            e.Handled = true;
        }

        void emptyControl_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (!LayoutHelper.IsChildElementLogical(propertyControlUI, e.OriginalSource as DependencyObject) ||
                e.Key != Key.Enter)
                return;

            if (propertyControlUI != null && propertyControlUI.btnCancel.IsEnabled == true)
            {
                if (CheckErrorBindings())
                {
                    e.Handled = true;
                    ForcePropertyControlUIFocus();
                    workspace.FlashDockedElement(emptyControl);
                }
                else
                {
                    ForcePropertyControlUIFocus();
                    workspace.EndEdit();
                    OnAcceptChanges();
                    workspace.ActivatePreviousActiveElement();
                }
            }
        }

        void CreatePropertyDataTemplates()
        {
            var dt = new DataTemplate();
            var factory = new FrameworkElementFactory(typeof(SourceFilePropertyEditor));
            factory.SetValue(SourceFilePropertyEditor.WorkspaceProperty, workspace);
            factory.SetValue(SourceFilePropertyEditor.CopyOptionProperty, SourceFileCopyOption.Never);
            dt.DataType = typeof(Uri);
            dt.VisualTree = factory;
            AddPropertyEditor(typeof(Uri), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(DateTimePropertyEditor));
            dt.DataType = typeof(DateTime);
            dt.VisualTree = factory;
            AddPropertyEditor(typeof(DateTime), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(FontFamilyPropertyEditor));
            dt.DataType = typeof(FontFamily);
            dt.VisualTree = factory;
            AddPropertyEditor(typeof(FontFamily), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(FontStylePropertyEditor));
            dt.DataType = typeof(FontStyle);
            dt.VisualTree = factory;
            AddPropertyEditor(typeof(FontStyle), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(FontWeightPropertyEditor));
            dt.DataType = typeof(FontWeight);
            dt.VisualTree = factory;
            AddPropertyEditor(typeof(FontWeight), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(Converters.PropertyDataTemplate.FontSettingsPropertyEditor));
            dt.DataType = typeof(FontSettings);
            dt.VisualTree = factory;
            AddPropertyEditor(typeof(FontSettings), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
            factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, 0d);
            factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, (double)Int16.MaxValue);
            dt.DataType = typeof(double);
            dt.VisualTree = factory;
            AddPropertyEditor("FontSize", typeof(double), typeof(System.Windows.Controls.Control), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(BrushPropertyEditor));
            dt.DataType = typeof(Brush);
            dt.VisualTree = factory;
            AddPropertyEditor(typeof(Brush), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(ColorPropertyEditor));
            dt.DataType = typeof(Color);
            dt.VisualTree = factory;
            AddPropertyEditor(typeof(Color), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
            factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, (double)Byte.MinValue);
            factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, (double)Byte.MaxValue);
            dt.DataType = typeof(Int32);
            dt.VisualTree = factory;
            AddPropertyEditor("Shades", typeof(Int32), typeof(System.Windows.Shapes.Shape), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
            factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, 0.0);
            factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, Double.MaxValue);
            dt.DataType = typeof(Double);
            dt.VisualTree = factory;
            AddPropertyEditor("StrokeThickness", typeof(Double), typeof(System.Windows.Shapes.Shape), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(StrokeDashArrayPropertyEditor));
            dt.DataType = typeof(DoubleCollection);
            dt.VisualTree = factory;
            AddPropertyEditor("StrokeDashArray", typeof(DoubleCollection), typeof(System.Windows.Shapes.Shape), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(TextDefaultPropertyEditor));
            dt.DataType = typeof(String);
            dt.VisualTree = factory;
            AddPropertyEditor(typeof(String), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(TextPropertyEditor));
            factory.SetValue(TextPropertyEditor.WorkspaceProperty, workspace);
            dt.DataType = typeof(String);
            dt.VisualTree = factory;
            AddPropertyEditor("ToolTip", typeof(Object), typeof(System.Windows.FrameworkElement), null, dt, true);
            AddPropertyEditor("Content", typeof(Object), typeof(ContentControl), null, dt, true);

            //dt = new DataTemplate();
            //factory = new FrameworkElementFactory(typeof(BrushPropertyEditor));
            //dt.DataType = typeof(Color);
            //dt.VisualTree = factory;
            //AddPropertyEditor(typeof(Color), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(TimeSpanPropertyEditor));
            factory.SetValue(TimeSpanPropertyEditor.TimeSpanFormatProperty, String.Format("d '({0})' hh:mm:ss", Properties.Resources.TimeSpanFormatDaysPart));
            dt.DataType = typeof(TimeSpan);
            dt.VisualTree = factory;
            AddPropertyEditor(typeof(TimeSpan), dt);
            AddPropertyEditor(typeof(Nullable<TimeSpan>), dt);

            // Standard Numeric data templates.
            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
            factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, Double.MinValue);
            factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, Double.MaxValue);
            factory.SetValue(NumericUpDownPropertyEditor.SpinValueProperty, 0.5);
            dt.DataType = typeof(Double);
            dt.VisualTree = factory;
            AddPropertyEditor(typeof(Double), dt);
            AddPropertyEditor(typeof(Nullable<Double>), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
            factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, (double)Decimal.MinValue);
            factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, (double)Decimal.MaxValue);
            factory.SetValue(NumericUpDownPropertyEditor.SpinValueProperty, 0.5);
            dt.DataType = typeof(Decimal);
            dt.VisualTree = factory;
            AddPropertyEditor(typeof(Decimal), dt);
            AddPropertyEditor(typeof(Nullable<Decimal>), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
            factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, (double)Single.MinValue);
            factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, (double)Single.MaxValue);
            factory.SetValue(NumericUpDownPropertyEditor.SpinValueProperty, 0.5);
            dt.DataType = typeof(Single);
            dt.VisualTree = factory;
            AddPropertyEditor(typeof(Single), dt);
            AddPropertyEditor(typeof(Nullable<Single>), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
            factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, (double)Int16.MinValue);
            factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, (double)Int16.MaxValue);
            dt.DataType = typeof(Int16);
            dt.VisualTree = factory;
            AddPropertyEditor(typeof(Int16), dt);
            AddPropertyEditor(typeof(Nullable<Int16>), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
            factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, (double)Int32.MinValue);
            factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, (double)Int32.MaxValue);
            dt.DataType = typeof(Int32);
            dt.VisualTree = factory;
            AddPropertyEditor(typeof(Int32), dt);
            AddPropertyEditor(typeof(Nullable<Int32>), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
            factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, (double)Int64.MinValue);
            factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, (double)Int64.MaxValue);
            dt.DataType = typeof(Int64);
            dt.VisualTree = factory;
            AddPropertyEditor(typeof(Int64), dt);
            AddPropertyEditor(typeof(Nullable<Int64>), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
            factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, (double)Byte.MinValue);
            factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, (double)Byte.MaxValue);
            dt.DataType = typeof(Byte);
            dt.VisualTree = factory;
            AddPropertyEditor(typeof(Byte), dt);
            AddPropertyEditor(typeof(Nullable<Byte>), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
            factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, (double)SByte.MinValue);
            factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, (double)SByte.MaxValue);
            dt.DataType = typeof(SByte);
            dt.VisualTree = factory;
            AddPropertyEditor(typeof(SByte), dt);
            AddPropertyEditor(typeof(Nullable<SByte>), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
            factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, (double)UInt16.MinValue);
            factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, (double)UInt16.MaxValue);
            dt.DataType = typeof(UInt16);
            dt.VisualTree = factory;
            AddPropertyEditor(typeof(UInt16), dt);
            AddPropertyEditor(typeof(Nullable<UInt16>), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
            factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, (double)UInt32.MinValue);
            factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, (double)UInt32.MaxValue);
            dt.DataType = typeof(UInt32);
            dt.VisualTree = factory;
            AddPropertyEditor(typeof(UInt32), dt);
            AddPropertyEditor(typeof(Nullable<UInt32>), dt);

            dt = new DataTemplate();
            factory = new FrameworkElementFactory(typeof(NumericUpDownPropertyEditor));
            factory.SetValue(NumericUpDownPropertyEditor.MinValueProperty, (double)UInt64.MinValue);
            factory.SetValue(NumericUpDownPropertyEditor.MaxValueProperty, (double)UInt64.MaxValue);
            dt.DataType = typeof(UInt64);
            dt.VisualTree = factory;
            AddPropertyEditor(typeof(UInt64), dt);
            AddPropertyEditor(typeof(Nullable<UInt64>), dt);
        }

        List<Type> listContainPropertyEditors;
        internal void AddToolBoxPropertyEditors(IContainPropertyEditors obj)
        {
            if (obj == null)
                return;

            if (listContainPropertyEditors == null)
                listContainPropertyEditors = new List<Type>();
            if (!listContainPropertyEditors.Contains(obj.GetType()))
            {
                listContainPropertyEditors.Add(obj.GetType());

                var datatempaltes = obj.GetListDataTemplates;
                foreach (var key in datatempaltes.Keys)
                {
                    AddPropertyEditor(key.Name, (Type)datatempaltes[key].DataType, key.OwnerType, datatempaltes[key]);
                }
            }
        }

        internal bool IsSelectedPropertyReadOnly(String propertyName)
        {
            if (workspace == null)
                return false;

            var selectedObject = workspace.ContextObject;
            var selectedObjects = workspace.ContextObjects;

            var toCheck = new List<INotifyPropertyReadOnlyChanged>();
            if (selectedObject != null && selectedObject is INotifyPropertyReadOnlyChanged)
                toCheck.Add(selectedObject as INotifyPropertyReadOnlyChanged);
            else if (selectedObjects != null)
            {
                foreach (var selected in selectedObjects)
                {
                    if (selected is INotifyPropertyReadOnlyChanged)
                        toCheck.Add(selected as INotifyPropertyReadOnlyChanged);
                }
            }

            foreach (var selected in toCheck)
            {
                if (selected[propertyName])
                    return true;
            }

            return false;
        }

        bool CheckErrorBindings(bool showerror = true)
        {
            if (bActionWasOnButton)
                return false;

            var valid = propertyControlUI.propertyGrid.ValidateBindings();
            if (!valid && showerror && UIInterface != null)
            {
                UIInterface.ShowWarning(Properties.Resources.InvalidValues);
            }

            return !valid;
        }

        void workspace_ContentRendered(object sender, EventArgs e)
        {
            bEnableIdleCode = true;
            if (bPendingIdleCode)
            {
                bPendingIdleCode = false;
                PromoteCodeToIdle(false);
            }
        }

        void workspace_DockStateChanged(FrameworkElement sender, UFInterfaces.DockStateEventArgs e)
        {
            if (sender == emptyControl && e.OldState == UFInterfaces.DockState.AutoHidden)
            {
                if (emptyControl != null)
                    emptyControl.Visibility = Visibility.Visible;
                PromoteCodeToIdle(true);
            }
            else if (sender == emptyControl && (e.NewState == UFInterfaces.DockState.Hidden || e.NewState == UFInterfaces.DockState.AutoHidden))
            {
                if (emptyControl != null)
                {
                    emptyControl.Visibility = Visibility.Collapsed;
                    bAutoHideVisible = false;
                }
            }
        }

        void workspace_ActiveWindowChanging(object sender, CancelEventArgs e)
        {
            IInputElement focusedElement = null;
            if (propertyControlUI != null)
                focusedElement = FocusManager.GetFocusedElement(Window.GetWindow(propertyControlUI));

            ForcePropertyControlUIFocus();
            e.Cancel = CheckErrorBindings();
            if (e.Cancel)
            {
                ForcePropertyControlUIFocus();
                workspace.FlashDockedElement(emptyControl);
                var dockState = workspace.GetElementDockState(emptyControl);
                if (dockState == UFInterfaces.DockState.AutoHidden)
                    workspace.SetElementDockState(emptyControl, UFInterfaces.DockState.Dock);
            }
            else if (!bActionWasOnButton)
            {
                //ForcePropertyControlUIFocus();
                workspace.EndEdit();
                OnAcceptChanges();
            }

            if (!e.Cancel && propertyControlUI != null && focusedElement != null && 
                focusedElement != FocusManager.GetFocusedElement(Window.GetWindow(propertyControlUI)))
            {
                focusedElement.Focusable = true;
                focusedElement.Focus();
                FocusManager.SetFocusedElement(Window.GetWindow(propertyControlUI), focusedElement);
            }
        }

        void workspace_ContextDocumentChanging(object sender, CancelEventArgs e)
        {
            ForcePropertyControlUIFocus();
            e.Cancel = CheckErrorBindings();
            if (e.Cancel)
            {
                ForcePropertyControlUIFocus();
                workspace.FlashDockedElement(emptyControl);
                var dockState = workspace.GetElementDockState(emptyControl);
                if (dockState == UFInterfaces.DockState.AutoHidden)
                    workspace.SetElementDockState(emptyControl, UFInterfaces.DockState.Dock);
            }
        }

        UFInterfaces.DockSide lastDockSide;
        bool bActionWasOnButton;
        void workspace_ActiveWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue == emptyControl)
            {
                if (propertyControlUI != null)
                {
                    propertyControlUI.btnCancel.ItemClick -= btnCancel_Click;
                    propertyControlUI.btnOK.ItemClick -= btnOK_Click;
                    propertyControlUI.btnCancel.IsEnabled = false;
                    propertyControlUI.btnOK.IsEnabled = false;
                    workspace.ActiveWindowChanging -= workspace_ActiveWindowChanging;
                    workspace.ContextDocumentChanging -= workspace_ContextDocumentChanging;
                }

                lastDockSide = workspace.GetElementDockSide(emptyControl);
                if (lastDockSide == UFInterfaces.DockSide.Tabbed && !workspace.GetElementIsSelectedTab(emptyControl) &&
                    workspace.GetElementDockState(emptyControl) != DockState.Float)
                {
                    if (emptyControl != null)
                        emptyControl.Visibility = Visibility.Collapsed;
                }
            }
            if (e.NewValue == emptyControl)
            {
                if (lastDockSide == UFInterfaces.DockSide.Tabbed && workspace.GetElementIsSelectedTab(emptyControl))
                {
                    if (emptyControl != null)
                        emptyControl.Visibility = Visibility.Visible;
                    PromoteCodeToIdle(true);
                }

                bActionWasOnButton = false;

                CreatePropertyControl();
                if (propertyControlUI != null)
                {
                    try
                    {
                        workspace.BeginEdit();
                        propertyControlUI.btnCancel.IsEnabled = true;
                        propertyControlUI.btnOK.IsEnabled = true;
                    }
                    catch
                    {
                        propertyControlUI.btnCancel.IsEnabled = false;
                        if (propertyControlUI.propertyGrid.SelectedObject == null &&
                            propertyControlUI.propertyGrid.SelectedObjects == null)
                            propertyControlUI.btnOK.IsEnabled = false;
                        else
                            propertyControlUI.btnOK.IsEnabled = true;
                    }

                    propertyControlUI.btnCancel.ItemClick += btnCancel_Click;
                    propertyControlUI.btnOK.ItemClick += btnOK_Click;

                    workspace.ActiveWindowChanging += workspace_ActiveWindowChanging;
                    workspace.ContextDocumentChanging += workspace_ContextDocumentChanging;
                }
            }

            if (emptyControl != null && (e.OldValue == emptyControl || e.NewValue == emptyControl))
            {
                var dockstate = workspace.GetElementDockState(emptyControl);
                if (dockstate == UFInterfaces.DockState.Document)
                {
                    if (e.NewValue == emptyControl && bLoaded)
                        emptyControl.Visibility = Visibility.Visible;
                    //else if (e.OldValue == emptyControl)
                    //    emptyControl.Visibility = Visibility.Collapsed;
                }
            }
        }

        void btnOK_Click(object sender, RoutedEventArgs e)
        {
            ForcePropertyControlUIFocus();
            if (CheckErrorBindings())
                return;

            bActionWasOnButton = true;
            workspace.EndEdit();
            OnAcceptChanges();
            workspace.ActivatePreviousActiveElement();
        }

        void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (workspace.ContextDocument == null)
                return;

            bActionWasOnButton = true;
            workspace.CancelEdit();
            workspace.ActivatePreviousActiveElement();
        }

        bool bContextObjectChanged;
        void workspace_ContextContentChanged(object sender, EventArgs e)
        {
            if (workspace.ContextObject != null)
            {
                SelectObjects = null;
                SelectObject = workspace.ContextObject;
            }
            else
            {
                SelectObject = null;
                SelectObjects = workspace.ContextObjects;
            }
            bContextObjectChanged = true;
        }

        void workspace_ContextContentChanging(object sender, CancelEventArgs e)
        {
            if (propertyControlUI != null)
                OnAcceptChanges();
        }

        bool bAutoHideVisible = false;
        bool bWasAutoHideVisible = false;
        void workspace_AutoHideAnimationStart(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource == emptyControl)
            {
                CreatePropertyControl();

                bAutoHideVisible = true;
                if (idleOperation != null)
                {
                    idleOperation.Abort();
                    idleOperation = null;
                }

                // if (bAutoHideVisible)
                {
                    if (emptyControl != null)
                    {
                        emptyControl.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        DispatcherOperation idleOperation;
        void workspace_AutoHideAnimationStop(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource == emptyControl)
            {
                if (emptyControl != null/* && workspace.ActiveWindow != emptyControl*/)
                {
                    //var dockstate = workspace.GetElementDockState(emptyControl);
                    //if (dockstate == DockState.AutoHidden)
                    {
                        if (bWasAutoHideVisible)
                        {
                            bWasAutoHideVisible = false;
                            if (idleOperation == null)
                            {
                                idleOperation = emptyControl.Dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
                                {
                                    if (!DockingHelper.GetLayoutItemVisible(emptyControl))
                                        emptyControl.Visibility = Visibility.Collapsed;
                                    else
                                        bAutoHideVisible = bWasAutoHideVisible = true;
                                });

                                idleOperation.Completed += (o, ev) =>
                                {
                                    idleOperation = null;
                                };
                            }
                            bAutoHideVisible = false;
                        }
                        else
                        {
                            bWasAutoHideVisible = true;
                            PromoteCodeToIdle(false);
                        }
                    }
                }
                else
                    PromoteCodeToIdle(true);
            }
        }

        private void PromoteSelectionObject(Object ob)
        {
            lock (lockObject)
            {
                PromoteSelectingObject = ob;
                PromoteSelectingObjects = null;
                PromoteCodeToIdle(false);
            }
        }

        private void PromoteSelectionObject(IList obs)
        {
            lock (lockObject)
            {
                PromoteSelectingObject = null;
                PromoteSelectingObjects = obs;
                PromoteCodeToIdle(false);
            }
        }

        private void PromoteCodeToIdle(bool bSynchro)
        {
            if (!bEnableIdleCode)
            {
                bPendingIdleCode = true;
                return;
            }

            var dockstate = workspace.GetElementDockState(emptyControl);
            bool bVisible = emptyControl.Visibility == Visibility.Visible && 
                (dockstate == UFInterfaces.DockState.Document || dockstate != UFInterfaces.DockState.Document && dockstate != UFInterfaces.DockState.AutoHidden ||
                 bAutoHideVisible && dockstate == UFInterfaces.DockState.AutoHidden);

            if (IdleExecutionPending == null && (bSynchro || bVisible))
            {
                Action action = () => IdleExecution();
                Dispatcher disp = emptyControl != null ? emptyControl.Dispatcher : Dispatcher.CurrentDispatcher;
                IdleExecutionPending = disp.BeginInvoke(action, bSynchro ? DispatcherPriority.Send : DispatcherPriority.Background);
                IdleExecutionPending.Completed += (sender, e) => IdleExecutionPending = null;
            }
        }

        bool ProjectHasChildren()
        {
            if (workspace.ContextDocument != null)
            {
                var rootDocument = DocumentManager.ComponentService.Helpers.DocumentHelper.GetRootParent(workspace.ContextDocument, traverse: true);
                if (rootDocument != null)
                {
                    var uFProjectManager = rootDocument.GetService(typeof(IUFProjectManager)) as IUFProjectManager;
                    if (uFProjectManager != null)
                    {
                        var _projectsDocument = uFProjectManager.GetAllProjectsDocuments(rootDocument);
                        var _projectsTitleList = _projectsDocument.ToDictionary(x => x.Key, y => y.Value.Title);
                        return _projectsTitleList.Count > 1 ? true : false;
                    }
                }
                else
                    return false;
            }
            return false;
            
        }

        void UpdateCurrentSelection(PropertyControlUI propertyControlUI, Object select = null)
        {
            if (propertyControlUI.IsDisposed)
                return;
            // PropertyGridState.RecordGridStatus(propertyControlUI.propertyGrid);

            propertyControlUI.propertyGrid.BeginInit();

            projHasChildren = ProjectHasChildren();

            var listextended = new Dictionary<Object, List<Object>>();
            var toSelect = select;
            if (toSelect == null)
                toSelect = PromoteSelectingObject;
            if (toSelect != null)
            {
                if (toSelect is IEntityReference &&
                    (toSelect as IEntityReference).ContainedObject != null &&
                    propertyControlUI.propertyGrid.SelectedObject != (toSelect as IEntityReference).ContainedObject)
                {
                    object parent = null;
                    var selected = (toSelect as IEntityReference).ContainedObject;
                    if (selected is ContentControl && (selected as ContentControl).Content is UIElement &&
                        !(selected is UserControl))
                    {
                        parent = selected;
                        selected = (selected as ContentControl).Content;
                    }

                    var listFriends = workspace.GetFriendObjects(parent ?? selected, null);
                    if (listFriends != null)
                    {
                        if (!listextended.ContainsKey(selected))
                            listextended.Add(selected, new List<Object>());
                        foreach (var v in listFriends)
                            listextended[selected].Add(v);
                    }

                    propertyControlUI.Title = selected.GetType().Name;
                    if (workspace.ContextDocument != null)
                        propertyControlUI.ProjectType = workspace.ContextDocument.ProjectType;

                    List<Object> extendeds = null;
                    if (listextended.ContainsKey(selected))
                        extendeds = listextended[selected];
                    var aggregated = new Aggregated(propertyControlUI, selected, projHasChildren, extendeds);
                    CleanCurrentSelection(propertyControlUI);
                    propertyControlUI.propertyGrid.SelectedObject = aggregated;
                }
                else if (propertyControlUI.propertyGrid.SelectedObject != toSelect)
                {
                    object parent = null;
                    var selected = toSelect;
                    if (selected is ContentControl && (selected as ContentControl).Content is UIElement &&
                        !(selected is UserControl))
                    {
                        parent = selected;
                        selected = (selected as ContentControl).Content;
                    }

                    var listFriends = workspace.GetFriendObjects(parent ?? selected, null);
                    if (listFriends != null)
                    {
                        if (!listextended.ContainsKey(selected))
                            listextended.Add(selected, new List<Object>());
                        foreach (var v in listFriends)
                            listextended[selected].Add(v);
                    }

                    propertyControlUI.Title = selected.GetType().Name;
                    if (workspace.ContextDocument != null)
                        propertyControlUI.ProjectType = workspace.ContextDocument.ProjectType;
                    if (selected is IDictionary)
                    {
                        propertyControlUI.Title = String.Empty;
                        CleanCurrentSelection(propertyControlUI);
                        propertyControlUI.propertyGrid.SelectedObject = selected;
                    }
                    else
                    {
                        List<Object> extendeds = null;
                        if (listextended.ContainsKey(selected))
                            extendeds = listextended[selected];
                        var aggregated = new Aggregated(propertyControlUI, selected, projHasChildren, extendeds);
                        CleanCurrentSelection(propertyControlUI);
                        propertyControlUI.propertyGrid.SelectedObject = aggregated;
                    }
                }
            }
            else if (PromoteSelectingObjects != null)
            {
                var listparent = new Dictionary<Object, List<Object>>();
                var list = new List<Object>();
                foreach (Object o in PromoteSelectingObjects)
                {
                    if (o is IEntityReference &&
                        (o as IEntityReference).ContainedObject != null)
                    {
                        object parent = null;
                        var selected = (o as IEntityReference).ContainedObject;
                        if (selected is ContentControl && (selected as ContentControl).Content is UIElement &&
                            !(selected is UserControl))
                        {
                            parent = selected;
                            selected = (selected as ContentControl).Content;
                            if (!listparent.ContainsKey(selected))
                                listparent.Add(selected, new List<Object>());
                            listparent[selected].Add(parent);
                        }

                        var sel = selected;
                        if (parent != null)
                            sel = parent;
                        var listFriends = workspace.GetFriendObjects(sel, null);
                        if (listFriends != null)
                        {
                            if (!listextended.ContainsKey(selected))
                                listextended.Add(selected, new List<Object>());
                            foreach (var v in listFriends)
                                listextended[selected].Add(v);
                        }

                        list.Add(selected);
                    }
                    else if (o != null)
                    {
                        object parent = null;
                        var selected = o;
                        if (selected is ContentControl && (selected as ContentControl).Content is UIElement &&
                            !(selected is UserControl))
                        {
                            parent = selected;
                            selected = (selected as ContentControl).Content;
                            if (!listparent.ContainsKey(selected))
                                listparent.Add(selected, new List<Object>());
                            listparent[selected].Add(parent);
                        }

                        var sel = selected;
                        if (parent != null)
                            sel = parent;
                        var listFriends = workspace.GetFriendObjects(sel, null);
                        if (listFriends != null)
                        {
                            if (!listextended.ContainsKey(selected))
                                listextended.Add(selected, new List<Object>());
                            foreach (var v in listFriends)
                                listextended[selected].Add(v);
                        }

                        list.Add(selected);
                    }
                }

                bool bChanged = true;
                if (propertyControlUI.propertyGrid.SelectedObjects != null)
                {
                    bChanged = false;
                    foreach (Object o in list)
                    {
                        if (!propertyControlUI.propertyGrid.SelectedObjects.Contains(o))
                        {
                            bChanged = true;
                            break;
                        }
                    }
                }

                if (bChanged)
                {
                    if (list.Count == 0)
                    {
                        propertyControlUI.Title = String.Empty;
                        CleanCurrentSelection(propertyControlUI);
                        propertyControlUI.propertyGrid.SelectedObject = null;
                        propertyControlUI.propertyGrid.SelectedObjects = null;
                    }
                    else if (list.Count == 1)
                    {
                        propertyControlUI.Title = list[0].GetType().Name;
                        if (workspace.ContextDocument != null)
                            propertyControlUI.ProjectType = workspace.ContextDocument.ProjectType;
                        List<Object> parents = null;
                        if (listparent.ContainsKey(list[0]))
                            parents = listparent[list[0]];
                        List<Object> extendeds = null;
                        if (listextended.ContainsKey(list[0]))
                            extendeds = listextended[list[0]];
                        var aggregated = new Aggregated(propertyControlUI, list[0], projHasChildren, extendeds, parents);
                        CleanCurrentSelection(propertyControlUI);
                        propertyControlUI.propertyGrid.SelectedObject = aggregated;
                    }
                    else
                    {
                        propertyControlUI.Title = list[0].GetType().Name;
                        if (workspace.ContextDocument != null)
                            propertyControlUI.ProjectType = workspace.ContextDocument.ProjectType;
                        var listaggregated = new List<Aggregated>();
                        for (int i = 0; i < list.Count; ++i)
                        {
                            List<Object> parents = null;
                            if (listparent.ContainsKey(list[i]))
                                parents = listparent[list[i]];
                            List<Object> extendeds = null;
                            if (listextended.ContainsKey(list[i]))
                                extendeds = listextended[list[i]];
                            listaggregated.Add(new Aggregated(propertyControlUI, list[i], projHasChildren, extendeds, parents));
                        }
                        CleanCurrentSelection(propertyControlUI);
                        propertyControlUI.propertyGrid.SelectedObjects = listaggregated;
                    }
                }
            }
            else
            {
                propertyControlUI.Title = String.Empty;
                CleanCurrentSelection(propertyControlUI);
                propertyControlUI.propertyGrid.SelectedObject = null;
                propertyControlUI.propertyGrid.SelectedObjects = null;
            }

            if (propertyControlUI.propertyGrid.SelectedObject == null &&
                propertyControlUI.propertyGrid.SelectedObjects == null)
            {
                propertyControlUI.btnCancel.IsEnabled = false;
                propertyControlUI.btnOK.IsEnabled = false;
            }

            propertyControlUI.RefreshGrid();

            //if (propertyControlUI.propertyGrid.SelectedObjects != null &&
            //    propertyControlUI.propertyGrid.SelectedObjects.Count > 1 ||
            //    propertyControlUI.propertyGrid.SelectedObject == null &&
            //    propertyControlUI.propertyGrid.SelectedObjects == null)
            //    propertyControlUI.ShowCustomExpander(null);
            //else if (propertyControlUI.propertyGrid.SelectedObject != null)
            //{
            //    var obj = propertyControlUI.propertyGrid.SelectedObject;
            //    if (obj is Aggregated)
            //        obj = (obj as Aggregated).Main;
            //    Type t = obj.GetType();
            //    var cust = t.GetProperty("SmartControl");
            //    if (cust != null)
            //    {
            //        var control = cust.GetValue(obj) as UserControl;
            //        propertyControlUI.ShowCustomExpander(control);
            //    }
            //    else
            //        propertyControlUI.ShowCustomExpander(null);
            //}

            propertyControlUI.propertyGrid.EndInit();
        }

        void CleanCurrentSelection(PropertyControlUI propertyControlUI)
        {
            if (propertyControlUI.propertyGrid.SelectedObject is Aggregated)
                (propertyControlUI.propertyGrid.SelectedObject as Aggregated).Dispose();

            var selectedObjects = propertyControlUI.propertyGrid.SelectedObjects as List<Aggregated>;
            if (selectedObjects != null)
            {
                foreach (var aggregated in selectedObjects)
                    aggregated.Dispose();
            }
        }

        private void IdleExecution()
        {
            using (new WaitCursor())
            {
                CreatePropertyControl();
                if (!bContextObjectChanged)
                    return;
                bContextObjectChanged = false;

                OnSelecting();

                UpdateCurrentSelection(propertyControlUI);

                OnSelected();
            }
        }

        private void ForcePropertyControlUIFocus()
        {
            if (propertyControlUI != null)
            {
                propertyControlUI.Focusable = true;
                propertyControlUI.Focus();
                FocusManager.SetFocusedElement(Window.GetWindow(propertyControlUI), propertyControlUI);
            }
        }

        IHelpProvider helpProvider;
        public IHelpProvider HelpProvider
        {
            get
            {
                if (helpProvider == null)
                    helpProvider = GetService(typeof(IHelpProvider)) as IHelpProvider;
                return helpProvider;
            }
        }

        #region IPropertyControl Members

        public event EventHandler PrepareChanges;
        virtual public void OnPrepareChanges()
        {
            EventHandler temp = PrepareChanges;
            if (temp != null)
                temp(this, EventArgs.Empty);
        }
        
        public event EventHandler Selecting;
        virtual public void OnSelecting()
        {
            EventHandler temp = Selecting;
            if (temp != null)
                temp(this, EventArgs.Empty);
        }

        public event EventHandler Selected;
        virtual public void OnSelected()
        {
            EventHandler temp = Selected;
            if (temp != null)
                temp(this, EventArgs.Empty);
        }

        public event EventHandler AcceptChanges;
        virtual public void OnAcceptChanges()
        {
            EventHandler temp = AcceptChanges;
            if (temp != null)
                temp(this, EventArgs.Empty);
        }

        public event EventHandler CancelChanges;
        virtual public void OnCancelChanges()
        {
            EventHandler temp = CancelChanges;
            if (temp != null)
                temp(this, EventArgs.Empty);
        }

        public object SelectObject
        {
            get
            {
                return PromoteSelectingObject != null ? PromoteSelectingObject : propertyControlUI != null ? propertyControlUI.propertyGrid.SelectedObject : null;
            }
            set
            {
                PromoteSelectionObject(value);
            }
        }

        public IList SelectObjects
        {
            get
            {
                return PromoteSelectingObjects != null ? PromoteSelectingObjects : propertyControlUI != null ? propertyControlUI.propertyGrid.SelectedObjects : null;
            }
            set
            {
                PromoteSelectionObject(value);
            }
        }

        public UserControl control
        {
            get
            {
                using (new WaitCursor())
                {
                    PopupControl popupControl = new PopupControl();
                    UpdateCurrentSelection(popupControl.propertyControl);
                    return popupControl;
                }
            }
        }

        public UserControl controlNoSelection
        {
            get
            {
                using (new WaitCursor())
                {
                    PopupControl popupControl = new PopupControl();
                    popupControl.propertyControl.IsPropertySetEasyMode = false;
                    return popupControl;
                }
            }
        }

        public UserControl controlNoSelectionPriority
        {
            get
            {
                using (new WaitCursor())
                {
                    PopupControl popupControl = new PopupControl();
                    popupControl.propertyControl.IsPropertySetEasyMode = false;
                    popupControl.propertyControl.PropertyFilterState = PropertyFilterEnum.SortByPriority;
                    return popupControl;
                }
            }
        }
        public void SetControlSelection(UserControl control, Object select)
        {
            using (new WaitCursor())
            {
                var propertycontrol = control as PopupControl;
                if (propertycontrol == null || select == null)
                    return;
                UpdateCurrentSelection(propertycontrol.propertyControl, select);
            }
        }

        public void UpdateControlSelection()
        {
            if (propertyControlUI != null)
                UpdateCurrentSelection(propertyControlUI);
        }

        public void AddPropertyEditor(Type editedType, DataTemplate editorTemplate)
        {
            var editor = new PropertyTypeEditor()
            {
                EditedType = editedType,
                EditorTemplate = editorTemplate,
                TemplateBindingMode = TypeEditorTemplateBindingMode.WrappedValue
            };

            propertyEditors.Add(editor);
        }

        public void AddPropertyEditor(Type editedType, Type declaringType, DataTemplate editorTemplate)
        {
            var editor = new PropertyNameContainsEditor()
            {
                DeclaringType = declaringType,
                EditType = editedType,
                EditorTemplate = editorTemplate
            };

            propertyEditors.Insert(0, editor); // need to insert type name to avoid priority on type only
        }

        public void AddPropertyEditor(String propertyName, Type editedType, Type declaringType, DataTemplate editorTemplate)
        {
            var editor = new PropertyNameContainsEditor()
            {
                PropertyName = propertyName,
                DeclaringType = declaringType,
                EditType = editedType,
                EditorTemplate = editorTemplate
            };

            propertyEditors.Insert(0, editor); // need to insert type name to avoid priority on type only
        }

        public void AddPropertyEditor(String propertyName, Type editedType, Type declaringType, Type documentType, DataTemplate editorTemplate, bool useBaseClass = false)
        {
            var editor = new PropertyNameContainsEditor()
            {
                PropertyName = propertyName,
                DeclaringType = declaringType,
                EditType = editedType,
                DocumentType = documentType, 
                EditorTemplate = editorTemplate,
                UseBaseClass = useBaseClass
            };

            propertyEditors.Insert(0, editor); // need to insert type name to avoid priority on type only
        }

        public PropertyDescriptor GetLocalizedPropertyDescriptor(DependencyProperty dependencyProperty, Type targetType)
        {
            DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(dependencyProperty, targetType);
            return new LocalizablePropertyDescriptor(dpd);
        }

        public IList PropertyEditors
        {
            get
            {
                return propertyEditors;
            }
        }

        DispatcherOperation pendingActivation;
        public void Activate()
        {
            if (emptyControl == null || pendingActivation != null)
                return;

            // workspace.SetDesiredSideMode(propertyControlUI, "SmartTagsControl");
            pendingActivation = Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
            {
                workspace.FlashDockedElement(emptyControl);
                pendingActivation = null;
            });
        }

        #endregion
        
        #region IDisposable Members

        void IDisposable.Dispose()
        {
            PromoteSelectingObject = null;
            PromoteSelectingObjects = null;
            propertyControlUI = null;

            if (workspace != null)
            {
                if (emptyControl != null)
                {
                    var wnd = emptyControl.FindParent<Window>();
                    if (wnd != null)
                    {
                        wnd.PreviewKeyDown -= emptyControl_PreviewKeyDown;
                        wnd.KeyDown -= emptyControl_KeyDown;
                    }
                }

                workspace.AutoHideAnimationStop -= workspace_AutoHideAnimationStop;
                workspace.AutoHideAnimationStart -= workspace_AutoHideAnimationStart;
                workspace.WorkspaceLoading -= workspace_WorkspaceLoading;
                workspace.ContextContentChanging -= workspace_ContextContentChanging;
                workspace.ContextContentChanged -= workspace_ContextContentChanged;
                workspace.DockStateChanged -= workspace_DockStateChanged;
                workspace.ActiveWindowChanged -= workspace_ActiveWindowChanged;
                workspace.ContentRendered -= workspace_ContentRendered;
            }

            if (IdleExecutionPending != null)
            {
                IdleExecutionPending.Abort();
                IdleExecutionPending = null;
            }

            lockObject = null;
        }

        #endregion

    }
}