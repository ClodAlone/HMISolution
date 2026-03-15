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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using UFRecipeEditor.ComponentService;
using DocumentManager.ComponentService;
using System.ComponentModel;
using UFRecipeSettings.Documents;
using Utilities;
using UFRecipeLayout.Helpers;
using OPCUAViewModelService.ComponentService;
using System.Threading.Tasks;
using WPFUtilities;
using WPFUtilities.Extensions;
using ScreenSettings;
using UFRecipeSettings.Helpers;
using System.Windows.Threading;
using UFInterfaces.PropertyControl;
using UFUAEditor.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using Utilities.WPF;
using System.Threading;
using UFInterfaces;

namespace RecipeViewerControl
{
    /// <summary>
    /// Interaction logic for RecipeGridView.xaml
    /// </summary>
    public partial class RecipeViewer : UserControl, IContainPropertyEditors, IDisposable
    {
        #region Dependency Properties

        /// <summary>
        /// relative Uri for recipe document
        /// </summary>
        public static readonly DependencyProperty RecipeNameProperty = DependencyProperty.Register("RecipeName", typeof(Uri), typeof(RecipeViewer), new UIPropertyMetadata(null, new PropertyChangedCallback(OnRecipeNameChanged), new CoerceValueCallback(OnCoerceRecipeName)));

        private static object OnCoerceRecipeName(DependencyObject o, object value)
        {
            RecipeViewer recipeControl = o as RecipeViewer;
            if (recipeControl != null)
                return recipeControl.OnCoerceRecipeName((Uri)value);
            else
                return value;
        }

        private static void OnRecipeNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeViewer recipeControl = o as RecipeViewer;
            if (recipeControl != null)
                recipeControl.OnRecipeNameChanged((Uri)e.OldValue, (Uri)e.NewValue);
        }

        protected virtual Uri OnCoerceRecipeName(Uri value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRecipeNameChanged(Uri oldValue, Uri newValue)
        {
            if (bLoaded && oldValue != newValue)
            {
                LoadLayoutControl();
            }
        }

        //public Uri testUri;
        public Uri RecipeName
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                //if (testUri == null)
                //    testUri = new Uri(@"file:///C:/Users/Public/Documents/Progea/Movicon.NExT/TestRecipe1/Recipe/Recipe1.UFRecipe");
                //return testUri;

                return (Uri)GetValue(RecipeNameProperty);
            }
            set
            {
                SetValue(RecipeNameProperty, value);
            }
        }

        /// <summary>
        /// allow edit values of the recipes
        /// </summary>
        public static readonly DependencyProperty AllowEditProperty = DependencyProperty.Register("AllowEdit", typeof(bool), typeof(RecipeViewer), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAllowEditChanged), new CoerceValueCallback(OnCoerceAllowEdit)));

        private static object OnCoerceAllowEdit(DependencyObject o, object value)
        {
            RecipeViewer recipeViewer = o as RecipeViewer;
            if (recipeViewer != null)
                return recipeViewer.OnCoerceAllowEdit((bool)value);
            else
                return value;
        }

        private static void OnAllowEditChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeViewer recipeViewer = o as RecipeViewer;
            if (recipeViewer != null)
                recipeViewer.OnAllowEditChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceAllowEdit(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAllowEditChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool AllowEdit
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(AllowEditProperty);
            }
            set
            {
                SetValue(AllowEditProperty, value);
            }
        }

        /// <summary>
        /// enable or disable allow to add new recipes
        /// </summary>
        public static readonly DependencyProperty AllowAddNewProperty = DependencyProperty.Register("AllowAddNew", typeof(bool), typeof(RecipeViewer), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAllowAddNewChanged), new CoerceValueCallback(OnCoerceAllowAddNew)));

        private static object OnCoerceAllowAddNew(DependencyObject o, object value)
        {
            RecipeViewer recipeViewer = o as RecipeViewer;
            if (recipeViewer != null)
                return recipeViewer.OnCoerceAllowAddNew((bool)value);
            else
                return value;
        }

        private static void OnAllowAddNewChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeViewer recipeViewer = o as RecipeViewer;
            if (recipeViewer != null)
                recipeViewer.OnAllowAddNewChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceAllowAddNew(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAllowAddNewChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool AllowAddNew
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(AllowAddNewProperty);
            }
            set
            {
                SetValue(AllowAddNewProperty, value);
            }
        }

        /// <summary>
        /// enable or disable allow to remove recipes
        /// </summary>
        public static readonly DependencyProperty AllowRemoveProperty = DependencyProperty.Register("AllowRemove", typeof(bool), typeof(RecipeViewer), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAllowRemoveChanged), new CoerceValueCallback(OnCoerceAllowRemove)));

        private static object OnCoerceAllowRemove(DependencyObject o, object value)
        {
            RecipeViewer recipeViewer = o as RecipeViewer;
            if (recipeViewer != null)
                return recipeViewer.OnCoerceAllowRemove((bool)value);
            else
                return value;
        }

        private static void OnAllowRemoveChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeViewer recipeViewer = o as RecipeViewer;
            if (recipeViewer != null)
                recipeViewer.OnAllowRemoveChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceAllowRemove(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAllowRemoveChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool AllowRemove
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(AllowRemoveProperty);
            }
            set
            {
                SetValue(AllowRemoveProperty, value);
            }
        }

        /// <summary>
        /// enable or disable allow to import recipes
        /// </summary>
        public static readonly DependencyProperty AllowImportProperty = DependencyProperty.Register("AllowImport", typeof(bool), typeof(RecipeViewer), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAllowImportChanged), new CoerceValueCallback(OnCoerceAllowImport)));

        private static object OnCoerceAllowImport(DependencyObject o, object value)
        {
            RecipeViewer recipeViewer = o as RecipeViewer;
            if (recipeViewer != null)
                return recipeViewer.OnCoerceAllowImport((bool)value);
            else
                return value;
        }

        private static void OnAllowImportChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeViewer recipeViewer = o as RecipeViewer;
            if (recipeViewer != null)
                recipeViewer.OnAllowImportChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceAllowImport(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAllowImportChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool AllowImport
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(AllowImportProperty);
            }
            set
            {
                SetValue(AllowImportProperty, value);
            }
        }

        /// <summary>
        /// enable or disable allow to export recipes
        /// </summary>
        public static readonly DependencyProperty AllowExportProperty = DependencyProperty.Register("AllowExport", typeof(bool), typeof(RecipeViewer), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAllowExportChanged), new CoerceValueCallback(OnCoerceAllowExport)));

        private static object OnCoerceAllowExport(DependencyObject o, object value)
        {
            RecipeViewer recipeViewer = o as RecipeViewer;
            if (recipeViewer != null)
                return recipeViewer.OnCoerceAllowExport((bool)value);
            else
                return value;
        }

        private static void OnAllowExportChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeViewer recipeViewer = o as RecipeViewer;
            if (recipeViewer != null)
                recipeViewer.OnAllowExportChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceAllowExport(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAllowExportChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool AllowExport
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(AllowExportProperty);
            }
            set
            {
                SetValue(AllowExportProperty, value);
            }
        }
        
        
        /// <summary>
        /// enable or disable allow to read value from plc
        /// </summary>
        public static readonly DependencyProperty AllowReadProperty = DependencyProperty.Register("AllowRead", typeof(bool), typeof(RecipeViewer), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAllowReadChanged), new CoerceValueCallback(OnCoerceAllowRead)));

        private static object OnCoerceAllowRead(DependencyObject o, object value)
        {
            RecipeViewer recipeViewer = o as RecipeViewer;
            if (recipeViewer != null)
                return recipeViewer.OnCoerceAllowRead((bool)value);
            else
                return value;
        }

        private static void OnAllowReadChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeViewer recipeViewer = o as RecipeViewer;
            if (recipeViewer != null)
                recipeViewer.OnAllowReadChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceAllowRead(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAllowReadChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool AllowRead
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(AllowReadProperty);
            }
            set
            {
                SetValue(AllowReadProperty, value);
            }
        }

        /// <summary>
        /// enable or disable allow to write value to plc
        /// </summary>
        public static readonly DependencyProperty AllowWriteProperty = DependencyProperty.Register("AllowWrite", typeof(bool), typeof(RecipeViewer), new UIPropertyMetadata(true, new PropertyChangedCallback(OnAllowWriteChanged), new CoerceValueCallback(OnCoerceAllowWrite)));

        private static object OnCoerceAllowWrite(DependencyObject o, object value)
        {
            RecipeViewer recipeViewer = o as RecipeViewer;
            if (recipeViewer != null)
                return recipeViewer.OnCoerceAllowWrite((bool)value);
            else
                return value;
        }

        private static void OnAllowWriteChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeViewer recipeViewer = o as RecipeViewer;
            if (recipeViewer != null)
                recipeViewer.OnAllowWriteChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceAllowWrite(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAllowWriteChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool AllowWrite
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(AllowWriteProperty);
            }
            set
            {
                SetValue(AllowWriteProperty, value);
            }
        }

        /// <summary>
        /// enable or disable the layout customization in runtime
        /// </summary>
        public static readonly DependencyProperty AllowLayoutCustomizationProperty = DependencyProperty.Register("AllowLayoutCustomization", typeof(bool), typeof(RecipeViewer), new UIPropertyMetadata(false, new PropertyChangedCallback(OnAllowLayoutCustomizationChanged), new CoerceValueCallback(OnCoerceAllowLayoutCustomization)));

        private static object OnCoerceAllowLayoutCustomization(DependencyObject o, object value)
        {
            RecipeViewer recipeViewer = o as RecipeViewer;
            if (recipeViewer != null)
                return recipeViewer.OnCoerceAllowLayoutCustomization((bool)value);
            else
                return value;
        }

        private static void OnAllowLayoutCustomizationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeViewer recipeViewer = o as RecipeViewer;
            if (recipeViewer != null)
                recipeViewer.OnAllowLayoutCustomizationChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceAllowLayoutCustomization(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnAllowLayoutCustomizationChanged(bool oldValue, bool newValue)
        {
            btnShow.Visibility = newValue ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        public bool AllowLayoutCustomization
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(AllowLayoutCustomizationProperty);
            }
            set
            {
                SetValue(AllowLayoutCustomizationProperty, value);
            }
        }

        #endregion

        #region Ovveride Dependency Properties

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            // Add foreground property.
            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(RecipeViewer));
            dpd.AddValueChangedSafe(this, OnForegroundColorChanged);
            
            // Add background property.
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(RecipeViewer));
            dpd.AddValueChangedSafe(this, OnBackgroundColorChanged);
        }
        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            // Remove foreground property.
            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(RecipeViewer));
            dpd.RemoveValueChangedSafe(this, OnForegroundColorChanged);

            // Remove background property.
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(RecipeViewer));
            dpd.RemoveValueChangedSafe(this, OnBackgroundColorChanged);
        }

        private void OnForegroundColorChanged(object sender, EventArgs e)
        {
            var control = sender as RecipeViewer;
            if (control != null)
            {
                control.OnForegroundColorChanged();
            }
        }

        private void OnBackgroundColorChanged(object sender, EventArgs e)
        {
            var control = sender as RecipeViewer;
            if (control != null)
            {
                control.OnBackgroundColorChanged();
            }
        }

        protected virtual void OnForegroundColorChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                UpdateForeColor(); 
        }

        protected virtual void OnBackgroundColorChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bInit)
                UpdateBackColor();
        }

        private void UpdateBackColor()
        {
            var control = recipeLayout.Content as Control;
            if (control != null)
                control.Background = this.Background;
        }

        #endregion

        #region Declarations

        bool bUriLoaded;
        bool bUriLoading;
        string lastLoadingError;
        IRecipeEditorManager recipeEditorManager;
        IUFUAEditorManager ufuaEditorService;
        IUIMsgBoxAlertService uiInterfaceService;
        UFRecipeExecuter.LayoutRecipeExecuter LayoutExecuter;
        UFRecipeExecuter.UFRecipeExecuter recipeExecuter;
        UFRecipeDocument recipeDocument;
        DataSet dataSet;

        CancellationTokenSource ctsLoading;

        #endregion

        #region Constructors

        bool bLoaded;
        bool bInit;
        public RecipeViewer()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            Loaded += (ob, ev) =>
                {
                    if (!bLoaded && !bDisposed)
                    {
                        bLoaded = true;
                        var document = ScreenDocument.GetScreenDocument(this);
                        ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, document);
                        OverrideBaseProperties();
                        LoadLayoutControl();
                        UpdateBackColor();
                        UpdateForeColor();

                        bInit = true;
                    }
                };
        }

        private void UpdateForeColor()
        {
            var control = recipeLayout.Content as Control;
            if (control != null)
                control.Foreground = this.Foreground;
        }

        #endregion

        #region Methods

        void LoadLayoutControl()
        {
            if (bUriLoading)
                return;

            UnloadLayout();

            var document = ScreenDocument.GetScreenDocument(this);
            if (document != null)
            {
                if (recipeEditorManager == null)
                    recipeEditorManager = document.GetService(typeof(IRecipeEditorManager)) as IRecipeEditorManager;
                if (recipeEditorManager == null)
                    throw new NotImplementedException("Expecting the missing IRecipeEditorManager Interface");

                if (ufuaEditorService == null)
                    ufuaEditorService = document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                if (ufuaEditorService == null)
                    throw new NotImplementedException("Expecting the missing IUFUAEditorManager Interface");

                if (uiInterfaceService == null)
                    uiInterfaceService = document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                
                if (RecipeName != null)
                {
                    Uri uri = document.MakeAbosoluteUri(RecipeName);
                    recipeDocument = UFRecipeDocument.FromFile(uri.GetPathString(), document);
                    if (recipeDocument != null)
                    {
                        SetBusy(true);

                        bUriLoading = true;
                        recipeDocument.Parent = document;

                        if (ctsLoading == null)
                            ctsLoading = new CancellationTokenSource();
                        var token = ctsLoading.Token;

                        bool designMode = DesignerProperties.GetIsInDesignMode(this);
                        recipeExecuter = new UFRecipeExecuter.UFRecipeExecuter(recipeDocument, UFRecipeExecuter.OPCUA.ConnectorType.None);
                        var executer = recipeExecuter;

                        var task = Task.Factory.StartNew(() =>
                        {
                            if (!designMode)
                            {
                                if (executer.CheckState == UFRecipeExecuter.RecipeCheckingStateEnum.Error ||
                                    executer.CheckState == UFRecipeExecuter.RecipeCheckingStateEnum.None)
                                {
                                    executer.CheckAndVerifyDatabase(token);

                                    if (token.IsCancellationRequested)
                                        return null;
                                }
                            }

                            return executer.CreateDataSet(fill: !designMode);
                        }, token);

                        task.ContinueWith(ret =>
                        {
                            try
                            {
                                if (token.IsCancellationRequested)
                                    return;

                                if (ret.Exception != null)
                                {
                                    lastLoadingError = ret.Exception.InnerException == null ?
                                            ret.Exception.Message : ret.Exception.InnerException.Message;
                                }
                                else if (ret.Result != null)
                                {
                                    dataSet = ret.Result;
                                    var runningOnServer = ScreenSettings.ScreenDocument.GetRunningOnServer(this);

                                    LayoutExecuter = new UFRecipeExecuter.LayoutRecipeExecuter(recipeExecuter, dataSet, document.Parent, !designMode, runningOnServer);

                                    LayoutExecuter.LayoutControl.IsEditRecipeEnabled = AllowEdit;
                                    LayoutExecuter.LayoutControl.IsInsertRecipeEnabled = AllowAddNew;
                                    LayoutExecuter.LayoutControl.IsRemoveRecipeEnabled = AllowRemove;
                                    LayoutExecuter.LayoutControl.IsReadRecipeEnabled = AllowRead;
                                    LayoutExecuter.LayoutControl.IsWriteRecipeEnabled = AllowWrite;
                                    LayoutExecuter.LayoutControl.IsImportRecipeEnabled = !runningOnServer && AllowImport;
                                    LayoutExecuter.LayoutControl.IsExportRecipeEnabled = !runningOnServer && AllowExport;

                                    // Handle the button reject changes enabled visualization.
                                    btnRejectChanges.IsEnabled = LayoutExecuter.LayoutControl.LayoutItems.IsCustomization;
                                    LayoutExecuter.LayoutControl.LayoutItems.IsCustomizationChanged += (s2, e2) =>
                                    {
                                        btnRejectChanges.IsEnabled = LayoutExecuter.LayoutControl.LayoutItems.IsCustomization;
                                    };

                                    if (!designMode)
                                    {
                                        Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)(() =>
                                        {
                                            if (bDisposed)
                                                return;

                                            if (runningOnServer)
                                            {
                                                var elements = LayoutExecuter.LayoutControl.LayoutItems.GetVisualChildrenOfType<FrameworkElement>().ToList();
                                                elements.ForEach(fe =>
                                                {
                                                    fe.IsHitTestVisible = false;
                                                    if (fe is DevExpress.Xpf.LayoutControl.GroupBox)
                                                    {
                                                        (fe as DevExpress.Xpf.LayoutControl.GroupBox).IsEnabled = false;
                                                    }
                                                    else if (fe is DevExpress.Xpf.LayoutControl.LayoutGroup)
                                                    {
                                                        (fe as DevExpress.Xpf.LayoutControl.LayoutGroup).IsCollapsible = false;
                                                        (fe as DevExpress.Xpf.LayoutControl.LayoutGroup).IsCollapsed = false;
                                                    }
                                                });
                                            }

                                            LayoutExecuter.PrepareExecution();
                                            LayoutExecuter.LayoutControl.ExecutedCommand += (s, e) =>
                                            {
                                                if (dataSet == null)
                                                    return;

                                                if (e.CommandType == UFRecipeLayout.EditCommandType.Reload)
                                                {
                                                    try
                                                    {
                                                        dataSet.Clear();
                                                        recipeExecuter.FillDataSet(dataSet, e.CancellationToken);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        e.exception = ex;
                                                    }
                                                }
                                            };
                                        }));
                                    }

                                    bUriLoaded = LayoutExecuter.LayoutControl.LayoutItems.GetChildren(true).Count > 0;
                                }
                            }
                            finally
                            {
                                bUriLoading = false;
                                SetBusy(false);
                                CheckIfLoaded();
                            }
                        }, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                }
            }

            CheckIfLoaded();
        }

        void UnloadLayout()
        {
            if (ctsLoading != null)
            {
                ctsLoading.Cancel();
                ctsLoading.Dispose();
                ctsLoading = null;
            }

            bUriLoaded = false;
            bUriLoading = false;
            lastLoadingError = null;

            SetBusy(false);
            SetError(false);

            if (LayoutExecuter != null)
                LayoutExecuter.Dispose();
            LayoutExecuter = null;

            if (recipeExecuter != null)
                recipeExecuter.Dispose();
            recipeExecuter = null;

            recipeLayout.Content = null;
        }

        void SetBusy(bool bSet)
        {
            if (bSet)
            {
                uriError.Visibility = Visibility.Collapsed;
            }
            
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                uriLoading.Visibility = bSet ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        void SetError(bool bSet)
        {
            if (bSet)
            {
                recipeLayout.Content = null;
                uriLoading.Visibility = Visibility.Collapsed;
            }

            uriError.Visibility = bSet ? Visibility.Visible : Visibility.Collapsed;
        }

        void CheckIfLoaded()
        {
            if (bUriLoaded)
            {
                var document = ScreenDocument.GetScreenDocument(this);
                if (document != null)
                    ThemeHelper.SetTheme(LayoutExecuter.LayoutControl, document.Theme);

                var runningOnServer = ScreenDocument.GetRunningOnServer(this);
                if (runningOnServer)
                {
                    LayoutExecuter.LayoutControl.Visibility = Visibility.Collapsed;
                    Dispatcher.BeginInvokeAsynchronouslyInBackground(() => 
                    {
                        if (bUriLoaded)
                        {
                            SetBusy(false);
                            SetError(false);

                            LayoutExecuter.LayoutControl.Visibility = Visibility.Visible;
                        }
                    });
                }
                else
                {
                    SetBusy(false);
                    SetError(false);
                }

                recipeLayout.Content = LayoutExecuter.LayoutControl;

                OnForegroundColorChanged();
                OnBackgroundColorChanged();
            }
            else if (!bUriLoading)
            {
                if (RecipeName == null)
                {
                    txtUriError.Text = Properties.Resources.EmptyRecipeUri;
                    uriRefresh.Visibility = Visibility.Collapsed;
                }
                else if (lastLoadingError != null)
                {
                    txtUriError.Text = lastLoadingError;
                    uriRefresh.Visibility = DesignerProperties.GetIsInDesignMode(this) ? Visibility.Collapsed : Visibility.Visible;
                }
                else if (LayoutExecuter != null && LayoutExecuter.LayoutControl != null)
                {
                    txtUriError.Text = String.Format(Properties.Resources.EmptyRecipeLayout, RecipeName.OriginalString);
                    uriRefresh.Visibility = Visibility.Collapsed;
                }
                else
                {
                    txtUriError.Text = String.Format(Properties.Resources.InvalidRecipeUri, RecipeName.OriginalString);
                    uriRefresh.Visibility = Visibility.Collapsed;
                }

                SetBusy(false);
                SetError(true);
            }
        }

        void AcceptLayoutChanges()
        {
            if (recipeDocument == null)
                return;

            LayoutControlHelper.SaveLayout(recipeDocument, LayoutExecuter.LayoutControl.LayoutItems, true);
        }

        void RejectLayoutChanges()
        {
            if (recipeDocument == null)
                return;

            LayoutControlHelper.LoadLayout(recipeDocument, LayoutExecuter.LayoutControl.LayoutItems, true);
        }

        void RestoreLayout()
        {
            if (recipeDocument == null)
                return;

            LayoutControlHelper.LoadLayout(recipeDocument, LayoutExecuter.LayoutControl.LayoutItems, false);
        }

        #endregion

        #region Commands

        private void EditLayout_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (LayoutExecuter.LayoutControl.LayoutItems.IsCustomization)
                AcceptLayoutChanges();

            LayoutExecuter.LayoutControl.LayoutItems.IsCustomization = !LayoutExecuter.LayoutControl.LayoutItems.IsCustomization; 
        }

        private void RejectChanges_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (uiInterfaceService == null || uiInterfaceService.ShowYesNo(String.Format(Properties.Resources.AskRejectLayout, Environment.NewLine),
                UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Question) == UIMsgBoxAlertService.ComponentService.CustomDialogResults.Yes)
            {
                RejectLayoutChanges();
            }
        }

        private void RestoreLayout_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (uiInterfaceService == null || uiInterfaceService.ShowYesNo(String.Format(Properties.Resources.AskRestoreLayout, Environment.NewLine),
                UIMsgBoxAlertService.ComponentService.CustomDialogIcons.Question) == UIMsgBoxAlertService.ComponentService.CustomDialogResults.Yes)
            {
                LayoutExecuter.LayoutControl.LayoutItems.IsCustomization = false;
                RestoreLayout();
                AcceptLayoutChanges();
            }
        }

        private void RefreshLayout_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            LoadLayoutControl();
        }

        #endregion

        #region IContainPropertyEditors Members

        [Browsable(false)]
        public Type ObjectType
        {
            get
            {
                return this.GetType();
            }
        }

        [Browsable(false)]
        public IDictionary<DependencyProperty, DataTemplate> GetListDataTemplates
        {
            get
            {
                var mapDataTemplates = new Dictionary<DependencyProperty, DataTemplate>();

                var document = ScreenSettings.ScreenDocument.GetScreenDocument(this);
                var workspace = document?.GetService(typeof(IWorkspace)) as IWorkspace;

                // Defines Data Template for 'RecipeNameProperty' dependency property.
                var dt = new DataTemplate();
                var factory = new FrameworkElementFactory(typeof(WPFUtilities.PropertyDataTemplate.RecipeUriPropertyEditor));
                factory.SetValue(WPFUtilities.PropertyDataTemplate.RecipeUriPropertyEditor.WorkspaceProperty, workspace);
                dt.DataType = typeof(Uri);
                dt.VisualTree = factory;
                mapDataTemplates.Add(RecipeNameProperty, dt);

                return mapDataTemplates;
            }
        }

        #endregion

        public bool DropManager(Object o)
        {
            if (o is Uri)
            {
                var dataUri = o as Uri;

                if (dataUri.GetPathString().Contains(".UFRecipe"))
                {
                    RecipeName = dataUri;
                    return true;
                }
            }

            return false;
        }

        #region IDisposable Members

        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            DetachOverrideBaseProperties();

            if (dataSet != null)
                dataSet.Dispose();
            dataSet = null;

            UnloadLayout();
        }

        #endregion
    }
}
