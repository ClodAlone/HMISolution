using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UFInterfaces;
using UFInterfaces.PropertyControl;
using DocumentManager.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using UFRecipeSettings.Documents;
using Utilities;
using UFRecipeEditor.ComponentService;
using RecipeViewerControl.ViewModel;
using Utilities.WPF;
using ViewModelLib;
using UFRecipeLayout.LayoutItemControls;
using UFRecipeLayout;
using System.Data;
using UFRecipeSettings.Helpers;
using DataReader.Extensions;
using Opc.Ua;
using UFUAEditor.ComponentService;
using System.Globalization;
using System.Xml.Serialization;
using System.IO;
using StringManager.ComponentService;
using TranslationHelpers;
using System.Windows.Media;
using Converters;
using DevExpress.Xpf.Grid;
using System.Windows.Media.Imaging;
using UFRecipeSettings.UFRecipeModel;
using WPFUtilities.Extensions;
using ScreenSettings;
using WPFUtilities;
using RecipeViewerControl.Converters;
using System.Windows.Data;
using Utilities.Commands;

namespace RecipeViewerControl
{
    /// <summary>
    /// Interaction logic for RecipeGrid.xaml
    /// </summary>
    public partial class RecipeGrid : UserControl, IContainPropertyEditors, ICheckUserCallable, IDisposable
    {
        #region Dependency Properties
        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), HasOverrideBrushProperties = true, RequiredKey = true)]
        [Browsable(false)]
        [XmlIgnore]
        public bool FooProperty { get { return false; } }
        #region RecipeName
        /// <summary>
        /// relative Uri for recipe document
        /// </summary>
        public static readonly DependencyProperty RecipeNameProperty = DependencyProperty.Register("RecipeName", typeof(Uri), typeof(RecipeGrid), new UIPropertyMetadata(null, new PropertyChangedCallback(OnRecipeNameChanged), new CoerceValueCallback(OnCoerceRecipeName)));

        private static object OnCoerceRecipeName(DependencyObject o, object value)
        {
            RecipeGrid recipeControl = o as RecipeGrid;
            if (recipeControl != null)
                return recipeControl.OnCoerceRecipeName((Uri)value);
            else
                return value;
        }

        private static void OnRecipeNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeGrid recipeControl = o as RecipeGrid;
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
                //LoadLayoutControl();
            }
        }

        [Category("Execution")]
        public Uri RecipeName
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Uri)GetValue(RecipeNameProperty);
            }
            set
            {
                SetValue(RecipeNameProperty, value);
            }
        }
        #endregion

        #region DataValues
        public static readonly DependencyProperty DataValuesProperty = DependencyProperty.Register("DataValues", typeof(UFDataValueEntityList), typeof(RecipeGrid), new UIPropertyMetadata(null));
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [XmlIgnore]
        [SvgValueConverter(typeof(ConvertUFDataValueEntityList))]
        public object DataValues
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (UFDataValueEntityList)GetValue(DataValuesProperty);
            }
            set
            {
                SetValue(DataValuesProperty, value);
            }
        }
        #endregion

        #region GroupByName
        public static readonly DependencyProperty GroupByNameProperty = DependencyProperty.Register("GroupByName", typeof(Boolean), typeof(RecipeGrid), new UIPropertyMetadata(false, new PropertyChangedCallback(OnGroupByNameChanged), new CoerceValueCallback(OnCoerceGroupByName)));

        private static object OnCoerceGroupByName(DependencyObject o, object value)
        {
            RecipeGrid control = o as RecipeGrid;
            if (control != null)
                return control.OnCoerceGroupByName((Boolean)value);
            else
                return value;
        }

        private static void OnGroupByNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeGrid control = o as RecipeGrid;
            if (control != null)
                control.OnGroupByNameChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceGroupByName(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnGroupByNameChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("Execution")]
        public Boolean GroupByName
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(GroupByNameProperty);
            }
            set
            {
                SetValue(GroupByNameProperty, value);
            }
        }
        #endregion

        #region UseIcon
        public static readonly DependencyProperty UseIconProperty = DependencyProperty.Register("UseIcon", typeof(Boolean), typeof(RecipeGrid), new UIPropertyMetadata(true, new PropertyChangedCallback(OnUseIconChanged), new CoerceValueCallback(OnCoerceUseIcon)));

        private static object OnCoerceUseIcon(DependencyObject o, object value)
        {
            RecipeGrid control = o as RecipeGrid;
            if (control != null)
                return control.OnCoerceUseIcon((Boolean)value);
            else
                return value;
        }

        private static void OnUseIconChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeGrid control = o as RecipeGrid;
            if (control != null)
                control.OnUseIconChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceUseIcon(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnUseIconChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("Style")]
        public Boolean UseIcon
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(UseIconProperty);
            }
            set
            {
                SetValue(UseIconProperty, value);
            }
        }
        #endregion

        #region PromptPad
        public static readonly DependencyProperty PromptPadProperty = DependencyProperty.Register("PromptPad", typeof(Boolean), typeof(RecipeGrid), new UIPropertyMetadata(false, new PropertyChangedCallback(OnPromptPadChanged), new CoerceValueCallback(OnCoercePromptPad)));

        private static object OnCoercePromptPad(DependencyObject o, object value)
        {
            RecipeGrid control = o as RecipeGrid;
            if (control != null)
                return control.OnCoercePromptPad((Boolean)value);
            else
                return value;
        }

        private static void OnPromptPadChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeGrid control = o as RecipeGrid;
            if (control != null)
                control.OnPromptPadChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoercePromptPad(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnPromptPadChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            viewModel.PromptPad = newValue;
        }

        [Category("Execution")]
        public Boolean PromptPad
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(PromptPadProperty);
            }
            set
            {
                SetValue(PromptPadProperty, value);
            }
        }
        #endregion

        #region ShowSearchPanel
        public static readonly DependencyProperty ShowSearchPanelProperty = DependencyProperty.Register("ShowSearchPanel", typeof(bool), typeof(RecipeGrid), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowSearchPanelChanged), new CoerceValueCallback(OnCoerceShowSearchPanel)));

        private static object OnCoerceShowSearchPanel(DependencyObject o, object value)
        {
            RecipeGrid control = o as RecipeGrid;
            if (control != null)
                return control.OnCoerceShowSearchPanel((bool)value);
            else
                return value;
        }

        private static void OnShowSearchPanelChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeGrid control = o as RecipeGrid;
            if (control != null)
                control.OnShowSearchPanelChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowSearchPanel(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowSearchPanelChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue)
            {
                if (tableView.SearchControl != null)
                {
                    tableView.SearchControl.PreviewTouchUp -= SearchControl_PreviewTouchUp;
                    tableView.SearchControl.PreviewMouseDown -= SearchControl_PreviewMouseDown;
                }
                tableView.ShowSearchPanelMode = newValue && !RunningOnServer ? ShowSearchPanelMode.Always : ShowSearchPanelMode.Never;
                if (tableView.ShowSearchPanelMode != ShowSearchPanelMode.Never)
                {
                    if (tableView.SearchControl != null)
                    {
                        tableView.SearchControl.PreviewTouchUp += SearchControl_PreviewTouchUp;
                        tableView.SearchControl.PreviewMouseDown += SearchControl_PreviewMouseDown;
                    }
                }
            }
        }

        private void SearchControl_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            if (!viewModel.PromptPad)
                return;

            var typeName = e.OriginalSource.GetType().FullName;
            if (typeName != TextBoxViewControlFullname)
                return;

            viewModel.ExecuteAsync(Dispatcher, new Action(() => EditSearchTextByPad()));
        }

        private void SearchControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!viewModel.PromptPad)
                return;

            var typeName = e.OriginalSource.GetType().FullName;
            if (typeName != TextBoxViewControlFullname)
                return;

            viewModel.ExecuteAsync(Dispatcher, new Action(() => EditSearchTextByPad()));
        }

        void EditSearchTextByPad()
        {
            var owner = this.FindParent<Window>();
            if (owner == null)
            {
                var ie = Keyboard.FocusedElement as DependencyObject;
                if (ie != null)
                    owner = Window.GetWindow(ie);
            }

            if (owner != null)
            {
                var ret = Pads.Pads.ShowAlphaNumericPad(tableView.SearchControl.SearchText, owner);
                if (ret != null)
                    tableView.SearchControl.SearchText = ret;
            }
        }

        [Category("Execution")]
        public bool ShowSearchPanel
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowSearchPanelProperty);
            }
            set
            {
                SetValue(ShowSearchPanelProperty, value);
            }
        }

        #endregion

        #region ShowReloadButton
        public static readonly DependencyProperty ShowReloadButtonProperty = DependencyProperty.Register("ShowReloadButton", typeof(Boolean), typeof(RecipeGrid), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowReloadButtonChanged), new CoerceValueCallback(OnCoerceShowReloadButton)));

        private static object OnCoerceShowReloadButton(DependencyObject o, object value)
        {
            RecipeGrid control = o as RecipeGrid;
            if (control != null)
                return control.OnCoerceShowReloadButton((Boolean)value);
            else
                return value;
        }

        private static void OnShowReloadButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeGrid control = o as RecipeGrid;
            if (control != null)
                control.OnShowReloadButtonChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceShowReloadButton(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowReloadButtonChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("Commands")]
        public Boolean ShowReloadButton
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(ShowReloadButtonProperty);
            }
            set
            {
                SetValue(ShowReloadButtonProperty, value);
            }
        }
        #endregion

        #region ShowSaveButton
        public static readonly DependencyProperty ShowSaveButtonProperty = DependencyProperty.Register("ShowSaveButton", typeof(Boolean), typeof(RecipeGrid), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowSaveButtonChanged), new CoerceValueCallback(OnCoerceShowSaveButton)));

        private static object OnCoerceShowSaveButton(DependencyObject o, object value)
        {
            RecipeGrid control = o as RecipeGrid;
            if (control != null)
                return control.OnCoerceShowSaveButton((Boolean)value);
            else
                return value;
        }

        private static void OnShowSaveButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeGrid control = o as RecipeGrid;
            if (control != null)
                control.OnShowSaveButtonChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceShowSaveButton(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowSaveButtonChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("Commands")]
        public Boolean ShowSaveButton
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(ShowSaveButtonProperty);
            }
            set
            {
                SetValue(ShowSaveButtonProperty, value);
            }
        }
        #endregion

        #region ShowAddButton
        public static readonly DependencyProperty ShowAddButtonProperty = DependencyProperty.Register("ShowAddButton", typeof(Boolean), typeof(RecipeGrid), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowAddButtonChanged), new CoerceValueCallback(OnCoerceShowAddButton)));

        private static object OnCoerceShowAddButton(DependencyObject o, object value)
        {
            RecipeGrid control = o as RecipeGrid;
            if (control != null)
                return control.OnCoerceShowAddButton((Boolean)value);
            else
                return value;
        }

        private static void OnShowAddButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeGrid control = o as RecipeGrid;
            if (control != null)
                control.OnShowAddButtonChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceShowAddButton(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowAddButtonChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("Commands")]
        public Boolean ShowAddButton
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(ShowAddButtonProperty);
            }
            set
            {
                SetValue(ShowAddButtonProperty, value);
            }
        }
        #endregion

        #region ShowRemoveButton
        public static readonly DependencyProperty ShowRemoveButtonProperty = DependencyProperty.Register("ShowRemoveButton", typeof(Boolean), typeof(RecipeGrid), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowRemoveButtonChanged), new CoerceValueCallback(OnCoerceShowRemoveButton)));

        private static object OnCoerceShowRemoveButton(DependencyObject o, object value)
        {
            RecipeGrid control = o as RecipeGrid;
            if (control != null)
                return control.OnCoerceShowRemoveButton((Boolean)value);
            else
                return value;
        }

        private static void OnShowRemoveButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeGrid control = o as RecipeGrid;
            if (control != null)
                control.OnShowRemoveButtonChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceShowRemoveButton(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowRemoveButtonChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("Commands")]
        public Boolean ShowRemoveButton
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(ShowRemoveButtonProperty);
            }
            set
            {
                SetValue(ShowRemoveButtonProperty, value);
            }
        }
        #endregion

        #region ShowImportButton
        public static readonly DependencyProperty ShowImportButtonProperty = DependencyProperty.Register("ShowImportButton", typeof(Boolean), typeof(RecipeGrid), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowImportButtonChanged), new CoerceValueCallback(OnCoerceShowImportButton)));

        private static object OnCoerceShowImportButton(DependencyObject o, object value)
        {
            RecipeGrid control = o as RecipeGrid;
            if (control != null)
                return control.OnCoerceShowImportButton((Boolean)value);
            else
                return value;
        }

        private static void OnShowImportButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeGrid control = o as RecipeGrid;
            if (control != null)
                control.OnShowImportButtonChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceShowImportButton(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowImportButtonChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("Commands")]
        public Boolean ShowImportButton
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(ShowImportButtonProperty);
            }
            set
            {
                SetValue(ShowImportButtonProperty, value);
            }
        }
        #endregion

        #region ShowExportButton
        public static readonly DependencyProperty ShowExportButtonProperty = DependencyProperty.Register("ShowExportButton", typeof(Boolean), typeof(RecipeGrid), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowExportButtonChanged), new CoerceValueCallback(OnCoerceShowExportButton)));

        private static object OnCoerceShowExportButton(DependencyObject o, object value)
        {
            RecipeGrid control = o as RecipeGrid;
            if (control != null)
                return control.OnCoerceShowExportButton((Boolean)value);
            else
                return value;
        }

        private static void OnShowExportButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeGrid control = o as RecipeGrid;
            if (control != null)
                control.OnShowExportButtonChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceShowExportButton(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowExportButtonChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("Commands")]
        public Boolean ShowExportButton
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(ShowExportButtonProperty);
            }
            set
            {
                SetValue(ShowExportButtonProperty, value);
            }
        }
        #endregion

        #region ShowReadButton
        public static readonly DependencyProperty ShowReadButtonProperty = DependencyProperty.Register("ShowReadButton", typeof(Boolean), typeof(RecipeGrid), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowReadButtonChanged), new CoerceValueCallback(OnCoerceShowReadButton)));

        private static object OnCoerceShowReadButton(DependencyObject o, object value)
        {
            RecipeGrid control = o as RecipeGrid;
            if (control != null)
                return control.OnCoerceShowReadButton((Boolean)value);
            else
                return value;
        }

        private static void OnShowReadButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeGrid control = o as RecipeGrid;
            if (control != null)
                control.OnShowReadButtonChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceShowReadButton(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowReadButtonChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("Commands")]
        public Boolean ShowReadButton
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(ShowReadButtonProperty);
            }
            set
            {
                SetValue(ShowReadButtonProperty, value);
            }
        }
        #endregion

        #region ShowWriteButton
        public static readonly DependencyProperty ShowWriteButtonProperty = DependencyProperty.Register("ShowWriteButton", typeof(Boolean), typeof(RecipeGrid), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowWriteButtonChanged), new CoerceValueCallback(OnCoerceShowWriteButton)));

        private static object OnCoerceShowWriteButton(DependencyObject o, object value)
        {
            RecipeGrid control = o as RecipeGrid;
            if (control != null)
                return control.OnCoerceShowWriteButton((Boolean)value);
            else
                return value;
        }

        private static void OnShowWriteButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeGrid control = o as RecipeGrid;
            if (control != null)
                control.OnShowWriteButtonChanged((Boolean)e.OldValue, (Boolean)e.NewValue);
        }

        protected virtual Boolean OnCoerceShowWriteButton(Boolean value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowWriteButtonChanged(Boolean oldValue, Boolean newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("Commands")]
        public Boolean ShowWriteButton
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Boolean)GetValue(ShowWriteButtonProperty);
            }
            set
            {
                SetValue(ShowWriteButtonProperty, value);
            }
        }
        #endregion

        #region ReadWriteTimeOut
        public static readonly DependencyProperty ReadWriteTimeOutProperty = DependencyProperty.Register("ReadWriteTimeOut", typeof(int), typeof(RecipeGrid), new UIPropertyMetadata(5000, new PropertyChangedCallback(OnReadWriteTimeOutChanged), new CoerceValueCallback(OnCoerceReadWriteTimeOut)));

        private static object OnCoerceReadWriteTimeOut(DependencyObject o, object value)
        {
            RecipeGrid control = o as RecipeGrid;
            if (control != null)
                return control.OnCoerceReadWriteTimeOut((int)value);
            else
                return value;
        }

        private static void OnReadWriteTimeOutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeGrid control = o as RecipeGrid;
            if (control != null)
                control.OnReadWriteTimeOutChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceReadWriteTimeOut(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnReadWriteTimeOutChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("Execution")]
        public int ReadWriteTimeOut
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(ReadWriteTimeOutProperty);
            }
            set
            {
                SetValue(ReadWriteTimeOutProperty, value);
            }
        }
        #endregion

        #region ClientSessionName
        public static readonly DependencyProperty ClientSessionNameProperty = DependencyProperty.Register("ClientSessionName", typeof(string), typeof(RecipeGrid), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnClientSessionNameChanged), new CoerceValueCallback(OnCoerceClientSessionName)));

        private static object OnCoerceClientSessionName(DependencyObject o, object value)
        {
            RecipeGrid RecipeGrid = o as RecipeGrid;
            if (RecipeGrid != null)
                return RecipeGrid.OnCoerceClientSessionName((string)value);
            else
                return value;
        }

        private static void OnClientSessionNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeGrid RecipeGrid = o as RecipeGrid;
            if (RecipeGrid != null)
                RecipeGrid.OnClientSessionNameChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceClientSessionName(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnClientSessionNameChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [XmlIgnore]
        [Browsable(false)]
        [MergablePropertyAttribute(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ClientSessionName
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(ClientSessionNameProperty);
            }
            set
            {
                SetValue(ClientSessionNameProperty, value);
            }
        }
        #endregion

        #region GridLayout
        public static readonly DependencyProperty GridLayoutProperty = DependencyProperty.Register("GridLayout", typeof(String), typeof(RecipeGrid), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnGridLayoutChanged), new CoerceValueCallback(OnCoerceGridLayout)));

        private static object OnCoerceGridLayout(DependencyObject o, object value)
        {
            RecipeGrid recipeGrid = o as RecipeGrid;
            if (recipeGrid != null)
                return recipeGrid.OnCoerceGridLayout((String)value);
            else
                return value;
        }

        private static void OnGridLayoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeGrid recipeGrid = o as RecipeGrid;
            if (recipeGrid != null)
                recipeGrid.OnGridLayoutChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceGridLayout(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnGridLayoutChanged(String oldValue, String newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            LoadDesignGridLayout();
        }

        internal void ResetGridLayout()
        {
            if (!string.IsNullOrEmpty(resetGridLayout))
                GridLayout = resetGridLayout;
        }

        void SaveResetGridLayout()
        {
            using (MemoryStream output = new MemoryStream())
            {
                Encoding utf8noBOM = new UTF8Encoding(true);
                gridControl.SaveLayoutToStream(output);
                resetGridLayout = utf8noBOM.GetString(output.ToArray());
            }
        }

        internal void SaveDesignGridLayout()
        {
            try
            {
                using (MemoryStream output = new MemoryStream())
                {
                    Encoding utf8noBOM = new UTF8Encoding(true);
                    gridControl.SaveLayoutToStream(output);
                    GridLayout = utf8noBOM.GetString(output.ToArray());
                }
            }
            catch (Exception)
            {
            }
        }

        void LoadDesignGridLayout()
        {
            if (string.IsNullOrEmpty(GridLayout))
                return;

            if (string.IsNullOrEmpty(resetGridLayout))
                SaveResetGridLayout();

            var dim = GridLayout.Length;
            string _mid = string.Empty;

            if (GridLayout.IndexOf('?') == 0)
            {
                _mid = GridLayout.Substring(1, dim - 1);
                SetValue(GridLayoutProperty, _mid);
                return;
            }
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl) || (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && Keyboard.IsKeyDown(Key.V))
            {
                if (!string.IsNullOrEmpty(GridLayout))
                {
                    Encoding utf8noBOM = new UTF8Encoding(true);
                    using (MemoryStream output = new MemoryStream(utf8noBOM.GetBytes(GridLayout)))
                    {
                        try
                        {
                            gridControl.RestoreLayoutFromStream(output);
                        }
                        catch
                        {
                            GridLayout = string.Empty;
                        }
                    }
                }
                else
                {
                    GridLayout = string.Empty;
                }
            }
            else
            {
                GridLayout = string.Empty;
            }
        }

        [Browsable(false)]
        [Category("Style")]
        [SvgValueConverter(typeof(ConvertGridLayout))]
        public String GridLayout
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(GridLayoutProperty);
            }
            set
            {
                SetValue(GridLayoutProperty, value);
            }
        }
        #endregion

        #region  GridControl Settings
        #region HeaderBackground
        public static readonly DependencyProperty HeaderBackgroundProperty = DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(RecipeGrid), new UIPropertyMetadata(null, new PropertyChangedCallback(OnHeaderBackgroundChanged), new CoerceValueCallback(OnCoerceHeaderBackground)));

        private static object OnCoerceHeaderBackground(DependencyObject o, object value)
        {
            RecipeGrid RecipeGrid = o as RecipeGrid;
            if (RecipeGrid != null)
                return RecipeGrid.OnCoerceHeaderBackground((Brush)value);
            else
                return value;
        }

        private static void OnHeaderBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeGrid RecipeGrid = o as RecipeGrid;
            if (RecipeGrid != null)
                RecipeGrid.OnHeaderBackgroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceHeaderBackground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnHeaderBackgroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("Style")]
        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), RequiredKey = true)]
        public Brush HeaderBackground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(HeaderBackgroundProperty);
            }
            set
            {
                SetValue(HeaderBackgroundProperty, value);
            }
        }
        #endregion

        #region HeaderForeground
        public static readonly DependencyProperty HeaderForegroundProperty = DependencyProperty.Register("HeaderForeground", typeof(Brush), typeof(RecipeGrid), new UIPropertyMetadata(null, new PropertyChangedCallback(OnHeaderForegroundChanged), new CoerceValueCallback(OnCoerceHeaderForeground)));

        private static object OnCoerceHeaderForeground(DependencyObject o, object value)
        {
            RecipeGrid RecipeGrid = o as RecipeGrid;
            if (RecipeGrid != null)
                return RecipeGrid.OnCoerceHeaderForeground((Brush)value);
            else
                return value;
        }

        private static void OnHeaderForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeGrid RecipeGrid = o as RecipeGrid;
            if (RecipeGrid != null)
                RecipeGrid.OnHeaderForegroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceHeaderForeground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnHeaderForegroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            //if (bLoaded)
            //{
            //    UpdateControlLayout();
            //}
        }

        [Category("Style")]
        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), RequiredKey = true)]
        public Brush HeaderForeground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(HeaderForegroundProperty);
            }
            set
            {
                SetValue(HeaderForegroundProperty, value);
            }
        }
        #endregion

        #region RowAreaForeground
        public static readonly DependencyProperty RowAreaForegroundProperty = DependencyProperty.Register("RowAreaForeground", typeof(Brush), typeof(RecipeGrid), new UIPropertyMetadata(null, new PropertyChangedCallback(OnRowAreaForegroundChanged), new CoerceValueCallback(OnCoerceRowAreaForeground)));

        private static object OnCoerceRowAreaForeground(DependencyObject o, object value)
        {
            RecipeGrid RecipeGrid = o as RecipeGrid;
            if (RecipeGrid != null)
                return RecipeGrid.OnCoerceRowAreaForeground((Brush)value);
            else
                return value;
        }

        private static void OnRowAreaForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeGrid RecipeGrid = o as RecipeGrid;
            if (RecipeGrid != null)
                RecipeGrid.OnRowAreaForegroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceRowAreaForeground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRowAreaForegroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("Style")]
        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), RequiredKey = true)]
        public Brush RowAreaForeground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(RowAreaForegroundProperty);
            }
            set
            {
                SetValue(RowAreaForegroundProperty, value);
            }
        }
        #endregion

        #region ShowCellsBorder
        public static readonly DependencyProperty ShowCellsBorderProperty = DependencyProperty.Register("ShowCellsBorder", typeof(bool), typeof(RecipeGrid), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowCellsBorderChanged), new CoerceValueCallback(OnCoerceShowCellsBorder)));

        private static object OnCoerceShowCellsBorder(DependencyObject o, object value)
        {
            RecipeGrid RecipeGrid = o as RecipeGrid;
            if (RecipeGrid != null)
                return RecipeGrid.OnCoerceShowCellsBorder((bool)value);
            else
                return value;
        }

        private static void OnShowCellsBorderChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeGrid RecipeGrid = o as RecipeGrid;
            if (RecipeGrid != null)
                RecipeGrid.OnShowCellsBorderChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowCellsBorder(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowCellsBorderChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("Style")]
        public bool ShowCellsBorder
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowCellsBorderProperty);
            }
            set
            {
                SetValue(ShowCellsBorderProperty, value);
            }
        }
        #endregion

        #region FocusedRowBackground
        public static readonly DependencyProperty FocusedRowBackgroundProperty = DependencyProperty.Register("FocusedRowBackground", typeof(Brush), typeof(RecipeGrid), new UIPropertyMetadata(Brushes.LightGray, new PropertyChangedCallback(OnFocusedRowBackgroundChanged), new CoerceValueCallback(OnCoerceFocusedRowBackground)));

        private static object OnCoerceFocusedRowBackground(DependencyObject o, object value)
        {
            RecipeGrid RecipeGrid = o as RecipeGrid;
            if (RecipeGrid != null)
                return RecipeGrid.OnCoerceFocusedRowBackground((Brush)value);
            else
                return value;
        }

        private static void OnFocusedRowBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeGrid RecipeGrid = o as RecipeGrid;
            if (RecipeGrid != null)
                RecipeGrid.OnFocusedRowBackgroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceFocusedRowBackground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnFocusedRowBackgroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("Style")]
        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), RequiredKey = true)]
        public Brush FocusedRowBackground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(FocusedRowBackgroundProperty);
            }
            set
            {
                SetValue(FocusedRowBackgroundProperty, value);
            }
        }
        #endregion

        #region FocusedRowForeground
        public static readonly DependencyProperty FocusedRowForegroundProperty = DependencyProperty.Register("FocusedRowForeground", typeof(Brush), typeof(RecipeGrid), new UIPropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnFocusedRowForegroundChanged), new CoerceValueCallback(OnCoerceFocusedRowForeground)));

        private static object OnCoerceFocusedRowForeground(DependencyObject o, object value)
        {
            RecipeGrid RecipeGrid = o as RecipeGrid;
            if (RecipeGrid != null)
                return RecipeGrid.OnCoerceFocusedRowForeground((Brush)value);
            else
                return value;
        }

        private static void OnFocusedRowForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeGrid RecipeGrid = o as RecipeGrid;
            if (RecipeGrid != null)
                RecipeGrid.OnFocusedRowForegroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceFocusedRowForeground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnFocusedRowForegroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("Style")]
        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), RequiredKey = true)]
        public Brush FocusedRowForeground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(FocusedRowForegroundProperty);
            }
            set
            {
                SetValue(FocusedRowForegroundProperty, value);
            }
        }
        #endregion

        #region CellBorderColor
        public static readonly DependencyProperty CellBorderColorProperty = DependencyProperty.Register("CellBorderColor", typeof(Brush), typeof(RecipeGrid), new UIPropertyMetadata(null, new PropertyChangedCallback(OnCellBorderColorChanged), new CoerceValueCallback(OnCoerceCellBorderColor)));

        private static object OnCoerceCellBorderColor(DependencyObject o, object value)
        {
            RecipeGrid RecipeGrid = o as RecipeGrid;
            if (RecipeGrid != null)
                return RecipeGrid.OnCoerceCellBorderColor((Brush)value);
            else
                return value;
        }

        private static void OnCellBorderColorChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeGrid RecipeGrid = o as RecipeGrid;
            if (RecipeGrid != null)
                RecipeGrid.OnCellBorderColorChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceCellBorderColor(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnCellBorderColorChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("Style")]
        public Brush CellBorderColor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(CellBorderColorProperty);
            }
            set
            {
                SetValue(CellBorderColorProperty, value);
            }
        }
        #endregion

        #region FocusedCellBackground
        public static readonly DependencyProperty FocusedCellBackgroundProperty = DependencyProperty.Register("FocusedCellBackground", typeof(Brush), typeof(RecipeGrid), new UIPropertyMetadata(null, new PropertyChangedCallback(OnFocusedCellBackgroundChanged), new CoerceValueCallback(OnCoerceFocusedCellBackground)));

        private static object OnCoerceFocusedCellBackground(DependencyObject o, object value)
        {
            RecipeGrid RecipeGrid = o as RecipeGrid;
            if (RecipeGrid != null)
                return RecipeGrid.OnCoerceFocusedCellBackground((Brush)value);
            else
                return value;
        }

        private static void OnFocusedCellBackgroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeGrid RecipeGrid = o as RecipeGrid;
            if (RecipeGrid != null)
                RecipeGrid.OnFocusedCellBackgroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceFocusedCellBackground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnFocusedCellBackgroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("Style")]
        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), RequiredKey = true)]
        public Brush FocusedCellBackground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(FocusedCellBackgroundProperty);
            }
            set
            {
                SetValue(FocusedCellBackgroundProperty, value);
            }
        }
        #endregion

        #region FocusedCellForeground
        public static readonly DependencyProperty FocusedCellForegroundProperty = DependencyProperty.Register("FocusedCellForeground", typeof(Brush), typeof(RecipeGrid), new UIPropertyMetadata(null, new PropertyChangedCallback(OnFocusedCellForegroundChanged), new CoerceValueCallback(OnCoerceFocusedCellForeground)));

        private static object OnCoerceFocusedCellForeground(DependencyObject o, object value)
        {
            RecipeGrid RecipeGrid = o as RecipeGrid;
            if (RecipeGrid != null)
                return RecipeGrid.OnCoerceFocusedCellForeground((Brush)value);
            else
                return value;
        }

        private static void OnFocusedCellForegroundChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeGrid RecipeGrid = o as RecipeGrid;
            if (RecipeGrid != null)
                RecipeGrid.OnFocusedCellForegroundChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceFocusedCellForeground(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnFocusedCellForegroundChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        [Category("Style")]
        [SvgValueConverter(ConverterType = typeof(ConvertDefaultBrushValue), RequiredKey = true)]
        public Brush FocusedCellForeground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(FocusedCellForegroundProperty);
            }
            set
            {
                SetValue(FocusedCellForegroundProperty, value);
            }
        }
        #endregion
        #region RowAreaFontSettings
        public static readonly DependencyProperty RowAreaFontSettingsProperty = DependencyProperty.Register("RowAreaFontSettings", typeof(FontSettings), typeof(RecipeGrid), new UIPropertyMetadata(new FontSettings(), new PropertyChangedCallback(OnRowAreaFontSettingsChanged), new CoerceValueCallback(OnCoerceRowAreaFontSettings)));

        private static object OnCoerceRowAreaFontSettings(DependencyObject o, object value)
        {
            RecipeGrid historicalEvents = o as RecipeGrid;
            if (historicalEvents != null)
                return historicalEvents.OnCoerceRowAreaFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnRowAreaFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeGrid historicalEvents = o as RecipeGrid;
            if (historicalEvents != null)
                historicalEvents.OnRowAreaFontSettingsChanged((FontSettings)e.OldValue, (FontSettings)e.NewValue);
        }

        protected virtual FontSettings OnCoerceRowAreaFontSettings(FontSettings value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnRowAreaFontSettingsChanged(FontSettings oldValue, FontSettings newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            UpdateValueFont(newValue);

            //{

            //}
        }

        [Category("Style")]
        [SvgValueConverter(typeof(ConvertFontSettings))]
        public FontSettings RowAreaFontSettings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (FontSettings)GetValue(RowAreaFontSettingsProperty);
            }
            set
            {
                SetValue(RowAreaFontSettingsProperty, value);
            }
        }
        bool bOverride;
        void UpdateValueFont(FontSettings newValue)
        {
            bOverride = true;
            FontStyle = newValue.FontStyle;
            FontWeight = newValue.FontWeight;
            FontSize = (double)newValue.FontSize;
            FontFamily = newValue.FontFamily;
            bOverride = false;
        }

        #endregion

        #region HeaderFontSettings
        public static readonly DependencyProperty HeaderFontSettingsProperty = DependencyProperty.Register("HeaderFontSettings", typeof(FontSettings), typeof(RecipeGrid), new UIPropertyMetadata(new FontSettings(), new PropertyChangedCallback(OnHeaderFontSettingsChanged), new CoerceValueCallback(OnCoerceHeaderFontSettings)));

        private static object OnCoerceHeaderFontSettings(DependencyObject o, object value)
        {
            RecipeGrid historicalEvents = o as RecipeGrid;
            if (historicalEvents != null)
                return historicalEvents.OnCoerceHeaderFontSettings((FontSettings)value);
            else
                return value;
        }

        private static void OnHeaderFontSettingsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeGrid historicalEvents = o as RecipeGrid;
            if (historicalEvents != null)
                historicalEvents.OnHeaderFontSettingsChanged((FontSettings)e.OldValue, (FontSettings)e.NewValue);
        }

        protected virtual FontSettings OnCoerceHeaderFontSettings(FontSettings value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnHeaderFontSettingsChanged(FontSettings oldValue, FontSettings newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                tableView.BestFitColumns();
            });
        }

        [Category("Style")]
        [SvgValueConverter(typeof(ConvertFontSettings))]
        public FontSettings HeaderFontSettings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (FontSettings)GetValue(HeaderFontSettingsProperty);
            }
            set
            {
                SetValue(HeaderFontSettingsProperty, value);
            }
        }

        #endregion
        #endregion
        #endregion

        #region Overrides
        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(RecipeGrid));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(RecipeGrid));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(RecipeGrid));
            dpd.AddValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(RecipeGrid));
            dpd.AddValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(RecipeGrid));
            dpd.AddValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(RecipeGrid));
            dpd.AddValueChangedSafe(this, OnFontSizeChanged);
        }
        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(RecipeGrid));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(RecipeGrid));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontStyleProperty, typeof(RecipeGrid));
            dpd.RemoveValueChangedSafe(this, OnFontStyleChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontFamilyProperty, typeof(RecipeGrid));
            dpd.RemoveValueChangedSafe(this, OnFontFamilyChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontWeightProperty, typeof(RecipeGrid));
            dpd.RemoveValueChangedSafe(this, OnFontWeightChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(FontSizeProperty, typeof(RecipeGrid));
            dpd.RemoveValueChangedSafe(this, OnFontSizeChanged);
        }
        private void OnFontFamilyChanged(object sender, EventArgs e)
        {
            var control = sender as RecipeGrid;
            if (control != null)
            {
                control.OnFontFamilyChanged();
            }
        }

        protected virtual void OnFontFamilyChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && !bOverride)
            {
                var value = RowAreaFontSettings.Clone();
                var measure = HeaderFontSettings.Clone();

                value.FontFamily = FontFamily;
                measure.FontFamily = FontFamily;

                RowAreaFontSettings = value;
                HeaderFontSettings = measure;
            }
        }

        private void OnFontWeightChanged(object sender, EventArgs e)
        {
            var control = sender as RecipeGrid;
            if (control != null)
            {
                control.OnFontWeightChanged();
            }
        }

        protected virtual void OnFontWeightChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && !bOverride)
            {
                var value = RowAreaFontSettings.Clone();
                var measure = HeaderFontSettings.Clone();

                value.FontWeight = FontWeight;
                measure.FontWeight = FontWeight;

                RowAreaFontSettings = value;
                HeaderFontSettings = measure;
            }
        }

        private void OnFontStyleChanged(object sender, EventArgs e)
        {
            var control = sender as RecipeGrid;
            if (control != null)
            {
                control.OnFontStyleChanged();
            }
        }

        protected virtual void OnFontStyleChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && !bOverride)
            {
                var value = RowAreaFontSettings.Clone();
                var measure = HeaderFontSettings.Clone();

                value.FontStyle = FontStyle;
                measure.FontStyle = FontStyle;

                RowAreaFontSettings = value;
                HeaderFontSettings = measure;
            }
        }

        private void OnFontSizeChanged(object sender, EventArgs e)
        {
            var control = sender as RecipeGrid;
            if (control != null)
            {
                control.OnFontSizeChanged();
            }
        }

        protected virtual void OnFontSizeChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (bLoaded && !bOverride)
            {
                var value = RowAreaFontSettings.Clone();
                var measure = HeaderFontSettings.Clone();

                value.FontSize = (int)FontSize;
                measure.FontSize = (int)FontSize;

                RowAreaFontSettings = value;
                HeaderFontSettings = measure;
            }
        }
        private void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as RecipeGrid;
            if (control != null)
            {
                control.OnForegroundChanged();
            }
        }
        protected virtual void OnForegroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            //if (bLoaded)
            //    ControlForeground = Foreground;
        }
        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            var control = sender as RecipeGrid;
            if (control != null)
            {
                control.OnBackgroundChanged();
            }
        }
        protected virtual void OnBackgroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            //if (bLoaded)
            //    UpdateControlLayout();
        }

        //private void UpdateControlLayout()
        //{
        //    if (this.ReadLocalValue(BackgroundProperty) != DependencyProperty.UnsetValue)
        //    {
        //        (from c in tableView.GetVisualChildrenOfType<Grid>()
        //         where c.Name == "rowPresenterGrid"
        //         select c).ToList().ForEach(o =>
        //         {
        //             (from d in o.GetVisualChildrenOfType<DevExpress.Xpf.Core.DXBorder>()
        //              select d).ToList().ForEach(x =>
        //              {
        //                  x.Background = Background;
        //              });
        //         });
        //    }

        //    if (this.ReadLocalValue(ControlForegroundProperty) != DependencyProperty.UnsetValue)
        //    {
        //        Dispatcher.BeginInvokeIfRequired(() =>
        //        {
        //            (from c in (this as UIElement).GetVisualChildrenOfType<TextBox>()
        //             select c).ToList().ForEach(child =>
        //             {
        //                 child.Foreground = ControlForeground;
        //             });
        //            (from c in (this as UIElement).GetVisualChildrenOfType<TextBlock>()
        //             select c).ToList().ForEach(child =>
        //             {
        //                 child.Foreground = ControlForeground;
        //             });
        //            (from c in (this as UIElement).GetVisualChildrenOfType<Label>()
        //             select c).ToList().ForEach(child =>
        //             {
        //                 child.Foreground = ControlForeground;
        //             });

        //            (from c in (this as UIElement).GetVisualChildrenOfType<ComboBox>()
        //             select c).ToList().ForEach(child =>
        //             {
        //                 child.Foreground = ControlForeground;
        //             });
        //        });
        //    }

        //    //if (this.ReadLocalValue(ToolbarBackgroundProperty) != DependencyProperty.UnsetValue)
        //    //{
        //    //    toolbar.Background = ToolbarBackground;
        //    //    toolbarSettings.Background = ToolbarBackground;
        //    //    bestFitbar.Background = ToolbarBackground;
        //    //}

        //    //if (this.ReadLocalValue(ToolbarForegroundProperty) != DependencyProperty.UnsetValue)
        //    //{
        //    //    actualSettings.Foreground = ToolbarForeground;
        //    //    configMemory.Foreground = ToolbarForeground;
        //    //    bestFit.Foreground = ToolbarForeground;
        //    //}
        //}

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (readIcon == null)
                readIcon = FindResource("UFRLRead") as BitmapImage;
            if (readIconOk == null)
                readIconOk = FindResource("UFRLReadOk") as BitmapImage;
            if (readIconError == null)
                readIconError = FindResource("UFRLReadError") as BitmapImage;
            if (writeIcon == null)
                writeIcon = FindResource("UFRLWrite") as BitmapImage;
            if (writeIconOk == null)
                writeIconOk = FindResource("UFRLWriteOk") as BitmapImage;
            if (writeIconError == null)
                writeIconError = FindResource("UFRLWriteError") as BitmapImage;
        }
        #endregion

        #region Declarations
        UFRecipeExecuter.UFRecipeExecuter recipeExecuter;
        UFRecipeDocument recipeDocument;
        DataSet dataSet;
        IDocument document;

        readonly List<Task> pendingTask = new List<Task>();
        CancellationTokenSource ctsPendingTask;
        CancellationTokenSource ctsLoading;
        bool isNewRecipeEntering;
        bool isAuditTraceSupported;

        string resetGridLayout;
        IDictionary<String, String> stringlist;

        BitmapImage readIcon;
        BitmapImage readIconOk;
        BitmapImage readIconError;
        BitmapImage writeIcon;
        BitmapImage writeIconOk;
        BitmapImage writeIconError;

        bool bCanGetInDataServerValues;
        bool bCanGetOutDataServerValues;
        bool bCanReadXmlDataSet;
        bool bCanWriteXmlDataSet;

        bool bLoaded;
        bool bDesign;
        internal bool bSmartSettingsEditing;

        const string TextBoxViewControlFullname = "System.Windows.Controls.TextBoxView";
        #endregion

        #region Constructors
        public RecipeGrid() : 
            this(null)
        { }

        public RecipeGrid(IDocument parent)
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);
            OverrideBaseProperties();

            Loaded += (ob, ev) =>
            {
                if (bLoaded || bDisposed)
                    return;
                bLoaded = true;

                bDesign = bSmartSettingsEditing || DesignerProperties.GetIsInDesignMode(this);
                document = parent ?? ScreenSettings.ScreenDocument.GetScreenDocument(this);

                ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, document);
                //LoadDesignGridLayout();
                OnShowSearchPanelChanged(!ShowSearchPanel, ShowSearchPanel);

                if (StringManager != null)
                {
                    StringManager_CultureChanged(document, null);
                    StringManager.CultureChanged += StringManager_CultureChanged;
                }

                if (bDesign)
                    busyControl.Visibility = Visibility.Collapsed;
                else
                {
                    isAuditTraceSupported = !RunningOnServer;
                    if (!RunningOnServer)
                    {
                        MouseEnter += GridControl_MouseEnter;
                    }

                    var task = Initialize();
                    if (task != null)
                        task.ContinueWith((o) => OnControlLoaded(), 
                            TaskScheduler.FromCurrentSynchronizationContext());
                    else
                        OnControlLoaded();
                }
            };
        }

        #endregion

        #region Methods
        Task Initialize()
        {
            if (RecipeEditorManager == null || RecipeName == null)
                return null;

            var recipeName = RecipeName;
            var clientSessionName = ClientSessionName;
            String fillError = null;
            ctsLoading = new CancellationTokenSource();
            var token = ctsLoading.Token;
            var task = Task.Factory.StartNew(() =>
            {
                var recipeUri = document.MakeAbosoluteUri(recipeName);
                recipeDocument = UFRecipeDocument.FromFile(recipeUri.GetPathString(), document);
                if (recipeDocument == null)
                    throw new System.IO.FileNotFoundException(String.Format(Properties.Resources.InvalidRecipeUri, recipeUri));

                recipeDocument.Parent = document;
                recipeExecuter = new UFRecipeExecuter.UFRecipeExecuter(recipeDocument, UFRecipeExecuter.OPCUA.ConnectorType.UseExtendedMethods, clientSessionName);
                recipeExecuter.Initialize();
                dataSet = recipeExecuter.CreateDataSet(token, fill: true, out fillError, bContinueOnError: true);
                return new RecipeGridViewModel(recipeDocument, dataSet);
            });

            return task.ContinueWith(ret =>
            {
                if (token.IsCancellationRequested)
                    return;

                RecipeGridViewModel taskResult = null;
                if (ret.Exception != null)
                {
                    try
                    {
                        dataSet = recipeExecuter.CreateDataSet(token);
                        taskResult = new RecipeGridViewModel(recipeDocument, dataSet);
                        taskResult.LastError = ret.Exception.InnerException == null ? ret.Exception.Message : ret.Exception.InnerException.Message;
                    }
                    catch (Exception ex)
                    {
                        lastErrorText.Text = ex.Message;
                    }
                }
                else
                    taskResult = ret.Result;

                if (taskResult != null)
                {
                    thisControl.DataContext = taskResult;
                    viewModel.PromptPad = PromptPad;
                    viewModel.IsReady = ret.Exception == null;
                    recipeIndexUI.SetBinding(DataSetHelper.TableName(recipeDocument.RecipeEntity),
                                            DataSetHelper.ColumnName(recipeDocument.RecipeEntity),
                                            DataSetHelper.PrimaryKeyName(recipeDocument.RecipeEntity));
                    if (recipeIndexUI.textEditBox != null)
                    {
                        recipeIndexUI.textEditBox.PreviewTouchUp += (s, e) =>
                        {
                            if (!viewModel.PromptPad)
                                return;

                            //e.Handled = true;
                            viewModel.ExecuteAsync(Dispatcher, new Action(() => recipeIndexUI.ShowPad()));
                        };

                        recipeIndexUI.textEditBox.PreviewMouseDown += (s, e) =>
                        {
                            if (!viewModel.PromptPad)
                                return;

                            //e.Handled = true;
                            viewModel.ExecuteAsync(Dispatcher, new Action(() => recipeIndexUI.ShowPad()));
                        };
                    }

                    viewModel.PropertyChanged += (s, e) =>
                    {
                        if (viewModel.IsBusy)
                        {
                            if (readIcon != null)
                                readBtnImage.Source = readIcon;
                            if (writeIcon != null)
                                writeBtnImage.Source = writeIcon;
                        }
                    };

                    if (!String.IsNullOrEmpty(fillError))
                        viewModel.LastError = String.Format(Properties.Resources.RecipeReadingError, fillError);

                    if (recipeExecuter != null && recipeExecuter.RecipeUAViewModel != null)
                    {
                        recipeExecuter.RecipeUAViewModel.PropertyChanged += RecipeUAViewModel_PropertyChanged;
                        Utilities.Commands.CheckUserCallable.GetInstance().Add(this);
                    }
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        const string stringPlaceolder = "RecipeGrid";
        void StringManager_CultureChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
            {
                if (bDisposed)
                    return;

                bool bUntranslated = bDesign && StringManager.GetActiveCulture(document, false) == String.Empty;
                if (!bUntranslated)
                    stringlist = stringManager.GetListStringForCulture(document, stringManager.GetActiveCulture(document));
                else
                    stringlist = null;

                StringValueConverter stringValueConverter = TryFindResource("StringValueConverter") as StringValueConverter;
                if(stringValueConverter != null)
                {
                    stringValueConverter.stringlist = stringlist;
                    stringValueConverter.stringPlaceolder = stringPlaceolder;
                    gridControl.RefreshData();
                }

                reloadBtnText.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ReloadButton", stringlist, UFRecipeLayout.Properties.Resources.LayoutRecipeReloadCommand);
                saveBtnText.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SaveButton", stringlist, UFRecipeLayout.Properties.Resources.LayoutRecipeSaveCommand);
                newBtnText.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_AddButton", stringlist, UFRecipeLayout.Properties.Resources.LayoutRecipeNewCommand);
                removeBtnText.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_RemoveButton", stringlist, UFRecipeLayout.Properties.Resources.LayoutRecipeRemoveCommand);
                importBtnText.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ImportButton", stringlist, UFRecipeLayout.Properties.Resources.LayoutRecipeImportCommand);
                exportBtnText.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ExportButton", stringlist, UFRecipeLayout.Properties.Resources.LayoutRecipeExportCommand);
                readBtnText.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ReadButton", stringlist, UFRecipeLayout.Properties.Resources.LayoutRecipeReadCommand);
                writeBtnText.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_WriteButton", stringlist, UFRecipeLayout.Properties.Resources.LayoutRecipeWriteCommand);

                recipeNameLabel.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_RecipeNameLabel", stringlist, Properties.Resources.RecipeNameLabel);
                newRecipeWindowTitle.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_NewRecipeWindowTitle", stringlist, UFRecipeLayout.Properties.Resources.EnterNewRecipeNameWindowTitle);
                viewModel.WaitText = TranslationHelper.TranlslateText($"_{stringPlaceolder}_WaitText", stringlist, UFRecipeLayout.Properties.Resources.WaitText);

                reloadButton.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_LayoutRecipeReloadCommandToolTip", stringlist, UFRecipeLayout.Properties.Resources.LayoutRecipeReloadCommandToolTip);
                saveButton.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_LayoutRecipeSaveCommandToolTip", stringlist, UFRecipeLayout.Properties.Resources.LayoutRecipeSaveCommandToolTip);
                newButton.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_LayoutRecipeNewCommandToolTip", stringlist, UFRecipeLayout.Properties.Resources.LayoutRecipeNewCommandToolTip);
                removeButton.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_LayoutRecipeRemoveCommandToolTip", stringlist, UFRecipeLayout.Properties.Resources.LayoutRecipeRemoveCommandToolTip);
                importButton.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_LayoutRecipeImportCommandTooltip", stringlist, UFRecipeLayout.Properties.Resources.LayoutRecipeImportCommandTooltip);
                exportButton.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_LayoutRecipeExportCommandTooltip", stringlist, UFRecipeLayout.Properties.Resources.LayoutRecipeExportCommandTooltip);
                readButton.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_LayoutRecipeReadCommandToolTip", stringlist, UFRecipeLayout.Properties.Resources.LayoutRecipeReadCommandToolTip);
                writeButton.ToolTip = TranslationHelper.TranlslateText($"_{stringPlaceolder}_LayoutRecipeWriteCommandToolTip", stringlist, UFRecipeLayout.Properties.Resources.LayoutRecipeWriteCommandToolTip);

                TranslationHelper.TranlslateColumns(gridControl.Columns, stringlist, stringPlaceolder);
            });
        }

        void RecipeUAViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UserIdentity")
            {
                Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    if (bDisposed)
                        return;

                    ReloadCommand.Execute(null);
                });
            }
        }

        void RecipeIndexUI_RecipeTextChanged(object sender, RecipeTextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.RecipeName))
                return;

            Guid newguid = Guid.NewGuid();
            var recipes = new Dictionary<String, String>();

            using (var viewRecipes = new DataView(dataSet.Tables[DataSetHelper.TableName(recipeDocument.RecipeEntity)]))
            {
                if (viewRecipes.Count > 0)
                {
                    int count = viewRecipes.Count;
                    for (int ii = 0; ii < viewRecipes.Count; ii++)
                    {
                        var name = viewRecipes[ii].Row[DataSetHelper.ColumnName(recipeDocument.RecipeEntity)].ToString().Trim();
                        recipes[name] = viewRecipes[ii].Row[DataSetHelper.PrimaryKeyName(recipeDocument.RecipeEntity)].ToString();
                    }
                }
            }

            if (recipes.ContainsKey(e.RecipeName) && Guid.TryParse(recipes[e.RecipeName], out newguid))
                recipeIndexUI.SelectedRecipeId = newguid;
            else if (!recipes.ContainsKey(e.RecipeName))
                AddNewRecipe(e.RecipeName, copyValues: true);
        }

        void RecipeIndexUI_RecipeSelectionChanged(object sender, RecipeSelectionEventArgs e)
        {
            Task task = null;
            try
            {
                if (!isNewRecipeEntering)
                    task = CheckAndAskSavePendingChanges(e.RecipeID);
            }
            catch (OperationCanceledException)
            {
                e.Cancel = true;
                return;
            }

            var action = new Action(() =>
            {
                try
                {
                    DataRow parentRow = null;
                    for (int cc = 0; cc < dataSet.Tables.Count; cc++)
                    {
                        if (dataSet.Tables[cc].DefaultView.RowStateFilter != DataViewRowState.CurrentRows)
                            dataSet.Tables[cc].DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
                        dataSet.Tables[cc].DefaultView.RowFilter = String.Format("[{0}]='{1}'", 
                            dataSet.Tables[cc].PrimaryKey[0].ColumnName, e.RecipeID);

                        // ensure a valid row in the table
                        if (dataSet.Tables[cc].DefaultView.Count == 0)
                        {
                            DataRowView rowView = dataSet.Tables[cc].DefaultView.AddNew();
                            rowView.BeginEdit();
                            if (parentRow != null)
                                rowView.Row.SetParentRow(parentRow);
                            rowView.Row[dataSet.Tables[cc].PrimaryKey[0]] = e.RecipeID;
                            rowView.EndEdit();
                        }
                        else if (dataSet.Tables[cc].TableName == DataSetHelper.TableName(recipeDocument.RecipeEntity))
                            parentRow = dataSet.Tables[cc].DefaultView[0].Row;
                    }
                }
                catch (Exception ex)
                {
                    viewModel.LastError = String.Format(UFRecipeLayout.Properties.Resources.AddNewRecipeFailed, e.RecipeID, ex.Message);
                }

                if (!isNewRecipeEntering)
                    recipeIndexUI.SelectedRecipeId = e.RecipeID;
            });

            if (task != null)
            {
                task.ContinueWith((T) =>
                {
                    action();
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
                action();
        }

        Task CheckAndAskSavePendingChanges()
        {
            return CheckAndAskSavePendingChanges(Guid.NewGuid());
        }

        Task CheckAndAskSavePendingChanges(Guid newRecipeID)
        {
            if (recipeIndexUI.SelectedRecipeId != Guid.Empty &&
                recipeIndexUI.SelectedRecipeId != newRecipeID &&
                (dataSet.HasChanges(DataRowState.Modified) || dataSet.HasChanges(DataRowState.Added)))
            {
                if (gridControl.ValidateBindings())
                {
                    CustomDialogResults result = CustomDialogResults.No;
                    if (UIInterface != null)
                    {
                        result = UIInterface.ShowYesNoCancel(UFRecipeLayout.Properties.Resources.AskSaveChanges, CustomDialogIcons.Question);
                    }

                    if (result == CustomDialogResults.Cancel)
                        throw new OperationCanceledException();
                    else if (result == CustomDialogResults.Yes)
                    {
                        if (isAuditTraceSupported &&
                            recipeExecuter.RecipeUAViewModel != null &&
                            recipeExecuter.RecipeUAViewModel.IsAuditTraceEnabled &&
                            recipeExecuter.RecipeUAViewModel.IsCommentRequired)
                        {
                            var auditTraceViewModel = new RecipeAuditTrace.RecipeAuditTraceViewModel(recipeExecuter.RecipeUAViewModel)
                            {
                                Control = saveButton,
                                RecipeIndex = recipeIndexUI.TextValue,
                                RecipeCommand = UFRecipeExecutionContext.RecipeCommandType.Save
                            };
                            auditTraceViewModel.Execute += (s, e) =>
                            {
                                recipeExecuter.AcceptUpdateData(dataSet, recipeIndexUI.SelectedRecipeId, userComment: auditTraceViewModel.AuditComment);
                                recipeExecuter.ForceUpdateRecipeTags();
                            };
                            if (!auditTraceViewModel.AskUserComment())
                                throw new OperationCanceledException();
                        }
                        else
                        {
                            if (ctsPendingTask == null)
                                ctsPendingTask = new CancellationTokenSource();
                            CancellationToken token = ctsPendingTask.Token;

                            var task = Task.Factory.StartNew(() =>
                            {
                                recipeExecuter.AcceptUpdateData(dataSet, recipeIndexUI.SelectedRecipeId);
                                recipeExecuter.ForceUpdateRecipeTags();
                            }, token);

                            task.ContinueWith(ret =>
                            {
                                pendingTask.Remove(task);
                                if (pendingTask.Count == 0)
                                    viewModel.IsBusy = false;

                                if (token.IsCancellationRequested)
                                    return;

                                if (ret.Exception != null)
                                    viewModel.LastError = ret.Exception.InnerException.Message;
                            }, TaskScheduler.FromCurrentSynchronizationContext());

                            pendingTask.Add(task);
                            viewModel.IsBusy = false;

                            return task;
                        }
                    }
                }

                if (ctsPendingTask == null)
                    ctsPendingTask = new CancellationTokenSource();
                CancellationToken token2 = ctsPendingTask.Token;

                var task2 = Task.Factory.StartNew(() =>
                {
                    recipeExecuter.RejectUpdateData(dataSet, recipeIndexUI.SelectedRecipeId);
                }, token2);

                task2.ContinueWith(ret =>
                {
                    pendingTask.Remove(task2);
                    if (pendingTask.Count == 0)
                        viewModel.IsBusy = false;

                    if (token2.IsCancellationRequested)
                        return;

                    if (ret.Exception != null)
                        viewModel.LastError = ret.Exception.InnerException.Message;
                }, TaskScheduler.FromCurrentSynchronizationContext());

                pendingTask.Add(task2);
                viewModel.IsBusy = true;

                return task2;
            }

            return null;
        }

        void AddNewRecipe(String recipeName, bool copyValues = false)
        {
            isNewRecipeEntering = true;
            if (ctsPendingTask == null)
                ctsPendingTask = new CancellationTokenSource();
            CancellationToken token = ctsPendingTask.Token;

            Guid newguid = Guid.NewGuid();
            var task = Task.Factory.StartNew(() =>
            {
                DataRow parentRow = null;
                for (int cc = 0; cc < dataSet.Tables.Count; cc++)
                {
                    DataView view = new DataView(dataSet.Tables[cc]);
                    DataRowView rowView = view.AddNew();
                    rowView.BeginEdit();
                    if (dataSet.Tables[cc].TableName == DataSetHelper.TableName(recipeDocument.RecipeEntity))
                    {
                        parentRow = rowView.Row;
                        rowView.Row[dataSet.Tables[cc].Columns[DataSetHelper.ColumnName(recipeDocument.RecipeEntity)]] = recipeName;
                        rowView.Row[dataSet.Tables[cc].Columns[DataSetHelper.CreationDateTimeColumnName(recipeDocument.RecipeEntity)]] = DateTime.UtcNow;
                    }
                    else if (parentRow != null)
                        rowView.Row.SetParentRow(parentRow);
                    rowView.Row[dataSet.Tables[cc].PrimaryKey[0]] = newguid;

                    // copy all column values
                    if (copyValues)
                    {
                        for (int ii = 0; ii < rowView.Row.Table.Columns.Count; ii++)
                        {
                            token.ThrowIfCancellationRequested();

                            if (dataSet.Tables[cc].DefaultView.Count == 0 ||
                                rowView.Row.Table.PrimaryKey.Contains(rowView.Row.Table.Columns[ii]) ||
                                (rowView.Row.Table.TableName == DataSetHelper.TableName(recipeDocument.RecipeEntity) &&
                                rowView.Row.Table.Columns[ii].ColumnName == DataSetHelper.ColumnName(recipeDocument.RecipeEntity)) ||
                                rowView.Row.Table.Columns[ii].ColumnName == DataSetHelper.CreationDateTimeColumnName(recipeDocument.RecipeEntity) ||
                                rowView.Row.Table.Columns[ii].ColumnName == DataSetHelper.ActivationDateTimeColumnName(recipeDocument.RecipeEntity))
                                continue;

                            rowView.Row[ii] = dataSet.Tables[cc].DefaultView[0].Row[ii];
                        }
                    }

                    rowView.EndEdit();
                }
            }, token);

            task.ContinueWith(ret =>
            {
                try
                {
                    pendingTask.Remove(task);
                    if (pendingTask.Count == 0)
                        viewModel.IsBusy = false;

                    if (token.IsCancellationRequested)
                        return;

                    if (ret.Exception != null)
                        viewModel.LastError = ret.Exception.InnerException.Message;
                    else
                        recipeIndexUI.SelectedRecipeId = newguid;
                }
                finally
                {
                    isNewRecipeEntering = false;
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());

            pendingTask.Add(task);
            viewModel.IsBusy = true;
        }

        void newRecipe_Ok(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            CloseNewRecipeForm(true);
        }

        void newRecipe_Cancel(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            CloseNewRecipeForm(false);
        }

        private void lastError_Ok(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            viewModel.LastError = null;
        }

        bool bHandledLastKeyPressed;
        void RecipeGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (newRecipeWindow.Visibility == System.Windows.Visibility.Visible)
            {
                if (e.Key == Key.Enter)
                {
                    e.Handled = true;
                    CloseNewRecipeForm(true);
                }
                else if (e.Key == Key.Escape)
                {
                    e.Handled = true;
                    CloseNewRecipeForm(false);
                }
            }

            bHandledLastKeyPressed = e.Handled || 
                busyControl.Visibility == Visibility.Visible ||
                lastErrorWindow.Visibility == Visibility.Visible ||
                (e.Key == Key.Enter || e.Key == Key.Escape) && e.OriginalSource is TextBox;
        }

        void ShowNewRecipeForm(string[] recipenames)
        {
            var uie = new UFRecipeLayout.UserControlNewRecipeName(recipenames, recipeDocument.RecipeEntity.MaxLength);
            newRecipeControl.Content = uie;
            if (viewModel.PromptPad)
            {
                uie.PreviewTouchUp += (s, e) =>
                {
                    //e.Handled = true;
                    viewModel.ExecuteAsync(Dispatcher, new Action(() => uie.ShowPad()));
                };

                uie.PreviewMouseDown += (s, e) =>
                {
                    //e.Handled = true;
                    viewModel.ExecuteAsync(Dispatcher, new Action(() => uie.ShowPad()));
                };
            }
            newRecipeWindow.Visibility = System.Windows.Visibility.Visible;
        }

        void CloseNewRecipeForm(bool bOK)
        {
            if (bOK)
            {
                var usercontrol = newRecipeControl.Content as UFRecipeLayout.UserControlNewRecipeName;
                if (usercontrol == null || !usercontrol.ValidateBindings())
                    return;

                Task task = null;
                try
                {
                    task = CheckAndAskSavePendingChanges();
                }
                catch (OperationCanceledException)
                {
                    return; 
                }

                var action = new Action(() =>
                {
                    AddNewRecipe(usercontrol.RecipeName, true);
                });

                if (task != null)
                {
                    task.ContinueWith((T) =>
                    {
                        action();
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
                else
                    action();
            }

            newRecipeWindow.Visibility = System.Windows.Visibility.Collapsed;
            newRecipeControl.Content = null;
        }

        public bool CanClose(bool bCheckPendingChanges)
        {
            if (bHandledLastKeyPressed)
            {
                bHandledLastKeyPressed = false;
                return false;
            }

            if (bCheckPendingChanges && recipeExecuter != null)
            {
                CustomDialogResults dlgResult = CustomDialogResults.No;
                if (dataSet != null && dataSet.HasChanges() && UIInterface != null)
                {
                    dlgResult = UIInterface.ShowYesNoCancel(UFRecipeLayout.Properties.Resources.AskSaveChanges, CustomDialogIcons.Question);
                }

                if (dlgResult == CustomDialogResults.Cancel)
                    return false;
                else if (dlgResult == CustomDialogResults.Yes)
                {
                    if (isAuditTraceSupported && 
                        recipeExecuter.RecipeUAViewModel != null &&
                        recipeExecuter.RecipeUAViewModel.IsAuditTraceEnabled &&
                        recipeExecuter.RecipeUAViewModel.IsCommentRequired)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            var auditTraceViewModel = new RecipeAuditTrace.RecipeAuditTraceViewModel(recipeExecuter.RecipeUAViewModel)
                            {
                                Control = saveButton,
                                RecipeIndex = recipeIndexUI.TextValue,
                                RecipeCommand = UFRecipeExecutionContext.RecipeCommandType.Save
                            };
                            auditTraceViewModel.Execute += (s, e) =>
                            {
                                recipeExecuter.AcceptUpdateData(dataSet, Guid.Empty, userComment: auditTraceViewModel.AuditComment);
                                recipeExecuter.ForceUpdateRecipeTags();
                            };
                            auditTraceViewModel.AskUserComment();
                        });
                    }
                    else
                    {
                        recipeExecuter.AcceptUpdateData(dataSet);
                        recipeExecuter.ForceUpdateRecipeTags();
                    }
                }
            }

            return true;
        }

        private void GridControl_MouseEnter(object sender, MouseEventArgs e)
        {
            MouseEnter -= GridControl_MouseEnter;
            MouseDown += GridControl_MouseDown;
        }

        void GridControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = e.LeftButton == MouseButtonState.Pressed;
        }
        #endregion

        #region Commands
        RelayCommand reloadCommand;
        [Browsable(false)]
        public ICommand ReloadCommand
        {
            get
            {
                if (reloadCommand == null)
                {
                    reloadCommand = new RelayCommand(param =>
                    {
                        Task task = null;
                        try
                        {
                            // reload recipe
                            task = CheckAndAskSavePendingChanges();
                        }
                        catch (OperationCanceledException)
                        {
                            return;
                        }

                        if (ctsPendingTask == null)
                            ctsPendingTask = new CancellationTokenSource();
                        CancellationToken token = ctsPendingTask.Token;

                        var recipeId = recipeIndexUI.SelectedRecipeId;
                        String fillError = null;
                        var action = new Action(() =>
                        {
                            dataSet.Clear();
                            fillError = recipeExecuter.FillDataSet(dataSet, token, bContinueOnError: true);
                        });

                        if (task != null)
                        {
                            task = task.ContinueWith((T) =>
                            {
                                action();
                            }, token);
                        }
                        else
                        {
                            task = Task.Factory.StartNew(() =>
                            {
                                action();
                            }, token);
                        }

                        task.ContinueWith(ret =>
                        {
                            pendingTask.Remove(task);
                            if (pendingTask.Count == 0)
                                viewModel.IsBusy = false;

                            if (token.IsCancellationRequested)
                                return;

                            viewModel.IsReady = ret.Exception == null;
                            recipeIndexUI.SelectedRecipeId = recipeId;
                            if (ret.Exception != null)
                                viewModel.LastError = ret.Exception.InnerException.Message;
                            else if (!String.IsNullOrEmpty(fillError))
                                viewModel.LastError = String.Format(Properties.Resources.RecipeReadingError, fillError);
                        }, TaskScheduler.FromCurrentSynchronizationContext());

                        pendingTask.Add(task);
                        viewModel.IsBusy = true;
                    }, param =>
                    {
                        return viewModel != null && pendingTask.Count == 0 && bCanReadXmlDataSet;
                    });
                }

                return reloadCommand;
            }
        }

        RelayCommand saveCommand;
        [Browsable(false)]
        public ICommand SaveCommand
        {
            get
            {
                if (saveCommand == null)
                {
                    saveCommand = new RelayCommand(param =>
                    {
                        // save recipe
                        if (!gridControl.ValidateBindings())
                        {
                            viewModel.LastError = UFRecipeLayout.Properties.Resources.InvalidBindingWarning;
                            return;
                        }

                        if (ctsPendingTask == null)
                            ctsPendingTask = new CancellationTokenSource();
                        CancellationToken token = ctsPendingTask.Token;

                        var task = Task.Factory.StartNew(() =>
                        {
                            if (isAuditTraceSupported && 
                                recipeExecuter.RecipeUAViewModel != null &&
                                recipeExecuter.RecipeUAViewModel.IsAuditTraceEnabled &&
                                recipeExecuter.RecipeUAViewModel.IsCommentRequired)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    var auditTraceViewModel = new RecipeAuditTrace.RecipeAuditTraceViewModel(recipeExecuter.RecipeUAViewModel)
                                    {
                                        Control = saveButton,
                                        RecipeIndex = recipeIndexUI.TextValue,
                                        RecipeCommand = UFRecipeExecutionContext.RecipeCommandType.Save
                                    };
                                    auditTraceViewModel.Execute += (s, e) =>
                                    {
                                        recipeExecuter.AcceptUpdateData(dataSet, recipeIndexUI.SelectedRecipeId, userComment: auditTraceViewModel.AuditComment);
                                        recipeExecuter.ForceUpdateRecipeTags();
                                    };
                                    auditTraceViewModel.AskUserComment();
                                });
                            }
                            else
                            {
                                recipeExecuter.AcceptUpdateData(dataSet, recipeIndexUI.SelectedRecipeId);
                                recipeExecuter.ForceUpdateRecipeTags();
                            }
                        }, token);

                        task.ContinueWith(ret =>
                        {
                            pendingTask.Remove(task);
                            if (pendingTask.Count == 0)
                                viewModel.IsBusy = false;

                            if (token.IsCancellationRequested)
                                return;

                            if (ret.Exception != null)
                                viewModel.LastError = ret.Exception.InnerException.Message;
                        }, TaskScheduler.FromCurrentSynchronizationContext());

                        pendingTask.Add(task);
                        viewModel.IsBusy = true;
                    }, param =>
                    {
                        return viewModel != null && viewModel.IsReady && viewModel.DataSet.HasChanges() && pendingTask.Count == 0 && bCanWriteXmlDataSet;
                    });
                }

                return saveCommand;
            }
        }

        RelayCommand addCommand;
        [Browsable(false)]
        public ICommand AddCommand
        {
            get
            {
                if (addCommand == null)
                {
                    addCommand = new RelayCommand(param =>
                    {
                        if (ctsPendingTask == null)
                            ctsPendingTask = new CancellationTokenSource();
                        CancellationToken token = ctsPendingTask.Token;

                        String[] recipenames = null;
                        var task = Task.Factory.StartNew(() =>
                        {
                            // new recipe
                            DataView viewRecipes = new DataView(dataSet.Tables[DataSetHelper.TableName(recipeDocument.RecipeEntity)]);
                            if (viewRecipes.Count > 0)
                            {
                                int count = viewRecipes.Count;
                                recipenames = new String[count];
                                for (int ii = 0; ii < viewRecipes.Count; ii++)
                                {
                                    token.ThrowIfCancellationRequested();

                                    recipenames[ii] = viewRecipes[ii].Row[DataSetHelper.ColumnName(recipeDocument.RecipeEntity)].ToString().Trim();
                                }
                            }
                        }, token);

                        task.ContinueWith(ret =>
                        {
                            pendingTask.Remove(task);
                            if (pendingTask.Count == 0)
                                viewModel.IsBusy = false;

                            if (token.IsCancellationRequested)
                                return;

                            if (ret.Exception != null)
                                viewModel.LastError = ret.Exception.InnerException.Message;
                            else
                                ShowNewRecipeForm(recipenames);
                        }, TaskScheduler.FromCurrentSynchronizationContext());

                        pendingTask.Add(task);
                        viewModel.IsBusy = true;
                    }, param =>
                    {
                        return viewModel != null && viewModel.IsReady && pendingTask.Count == 0;
                    });
                }

                return addCommand;
            }
        }

        RelayCommand removeCommand;
        [Browsable(false)]
        public ICommand RemoveCommand
        {
            get
            {
                if (removeCommand == null)
                {
                    removeCommand = new RelayCommand(param =>
                    {
                        // remove recipe
                        if (dataSet.Tables[DataSetHelper.TableName(recipeDocument.RecipeEntity)].DefaultView.Count == 0)
                            return;

                        if (UIInterface == null ||
                            UIInterface.ShowYesNo(UFRecipeLayout.Properties.Resources.AskDeleteRecipe,
                            CustomDialogIcons.Question) == CustomDialogResults.Yes)
                        {
                            if (ctsPendingTask == null)
                                ctsPendingTask = new CancellationTokenSource();
                            CancellationToken token = ctsPendingTask.Token;

                            var recipeId = dataSet.Tables[DataSetHelper.TableName(recipeDocument.RecipeEntity)].DefaultView[0].Row[DataSetHelper.PrimaryKeyName(recipeDocument.RecipeEntity)].ToString();
                            var recipeIndex = dataSet.Tables[DataSetHelper.TableName(recipeDocument.RecipeEntity)].DefaultView[0].Row[DataSetHelper.ColumnName(recipeDocument.RecipeEntity)].ToString();
                            var guid = Guid.Empty;
                            var task = Task.Factory.StartNew(() =>
                            {
                                // notify command execution
                                if (Guid.TryParse(recipeId, out guid))
                                {
                                    bool bRejectChanges = false;
                                    if (isAuditTraceSupported &&
                                        recipeExecuter.RecipeUAViewModel != null &&
                                        recipeExecuter.RecipeUAViewModel.IsAuditTraceEnabled &&
                                        recipeExecuter.RecipeUAViewModel.IsCommentRequired)
                                    {
                                        Dispatcher.Invoke(() =>
                                        {
                                            var auditTraceViewModel = new RecipeAuditTrace.RecipeAuditTraceViewModel(recipeExecuter.RecipeUAViewModel)
                                            {
                                                Control = removeButton,
                                                RecipeIndex = recipeIndex,
                                                RecipeCommand = UFRecipeExecutionContext.RecipeCommandType.Remove
                                            };
                                            auditTraceViewModel.Execute += (s, e) =>
                                            {
                                                // delete row in the main table and in all parent tables
                                                if (dataSet.Tables[DataSetHelper.TableName(recipeDocument.RecipeEntity)].DefaultView.Count > 0)
                                                    dataSet.Tables[DataSetHelper.TableName(recipeDocument.RecipeEntity)].DefaultView[0].Row.Delete();
                                                recipeExecuter.AcceptUpdateData(dataSet, guid, userComment: auditTraceViewModel.AuditComment);
                                                recipeExecuter.ForceUpdateRecipeTags();
                                            };
                                            bRejectChanges = !auditTraceViewModel.AskUserComment();
                                        });
                                    }
                                    else
                                    {
                                        // delete row in the main table and in all parent tables
                                        dataSet.Tables[DataSetHelper.TableName(recipeDocument.RecipeEntity)].DefaultView[0].Row.Delete();
                                        recipeExecuter.AcceptUpdateData(dataSet, guid);
                                        recipeExecuter.ForceUpdateRecipeTags();
                                    }

                                    if (bRejectChanges)
                                    {
                                        dataSet.RejectChanges();
                                        for (int cc = 0; cc < dataSet.Tables.Count; cc++)
                                        {
                                            token.ThrowIfCancellationRequested();

                                            if (dataSet.Tables[cc].DefaultView.RowStateFilter != DataViewRowState.CurrentRows)
                                                dataSet.Tables[cc].DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
                                            dataSet.Tables[cc].DefaultView.RowFilter = String.Empty;
                                            dataSet.Tables[cc].DefaultView.RowFilter = String.Format("[{0}]='{1}'",
                                                                                        dataSet.Tables[cc].PrimaryKey[0].ColumnName,
                                                                                        guid);
                                        }
                                    }
                                    else
                                    {
                                        using (var viewRecipes = new DataView(dataSet.Tables[DataSetHelper.TableName(recipeDocument.RecipeEntity)]))
                                        {
                                            viewRecipes.Sort = String.Format("[{0}] ASC", DataSetHelper.ColumnName(recipeDocument.RecipeEntity));
                                            if (viewRecipes.Count > 0)
                                            {
                                                if (Guid.TryParse(viewRecipes[0].Row[viewRecipes.Table.PrimaryKey[0].ColumnName].ToString(), out guid))
                                                {
                                                    for (int cc = 0; cc < dataSet.Tables.Count; cc++)
                                                    {
                                                        token.ThrowIfCancellationRequested();

                                                        if (dataSet.Tables[cc].DefaultView.RowStateFilter != DataViewRowState.CurrentRows)
                                                            dataSet.Tables[cc].DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
                                                        dataSet.Tables[cc].DefaultView.RowFilter = String.Format("{0}='{1}'",
                                                                                                    dataSet.Tables[cc].PrimaryKey[0].ColumnName,
                                                                                                    guid);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }, token);

                            task.ContinueWith(ret =>
                            {
                                pendingTask.Remove(task);
                                if (pendingTask.Count == 0)
                                    viewModel.IsBusy = false;

                                if (token.IsCancellationRequested)
                                    return;

                                if (ret.Exception != null)
                                    viewModel.LastError = ret.Exception.InnerException.Message;
                                else
                                    recipeIndexUI.SelectedRecipeId = guid;
                            }, TaskScheduler.FromCurrentSynchronizationContext());

                            pendingTask.Add(task);
                            viewModel.IsBusy = true;
                        }
                    }, param =>
                    {
                        return viewModel != null && viewModel.IsReady && pendingTask.Count == 0 && bCanWriteXmlDataSet;
                    });
                }

                return removeCommand;
            }
        }

        RelayCommand importCommand;
        [Browsable(false)]
        public ICommand ImportCommand
        {
            get
            {
                if (importCommand == null)
                {
                    importCommand = new RelayCommand(param =>
                    {
                        // import recipe
                        if (UIInterface != null)
                        {
                            var file = UIInterface.ShowOpenFileDialog(UFRecipeLayout.Properties.Resources.CSVFilter);
                            if (String.IsNullOrEmpty(file) || !System.IO.File.Exists(file))
                                return;

                            if (ctsPendingTask == null)
                                ctsPendingTask = new CancellationTokenSource();
                            CancellationToken token = ctsPendingTask.Token;

                            var task = Task.Factory.StartNew(() =>
                            {
                                using (System.IO.StreamReader readFile = new System.IO.StreamReader(file))
                                {
                                    var recipeEntity = recipeDocument.RecipeEntity;
                                    var recipeId = dataSet.Tables[DataSetHelper.TableName(recipeEntity)].DefaultView[0].Row[DataSetHelper.PrimaryKeyName(recipeEntity)].ToString();
                                    var recipePrimaryKeyName = DataSetHelper.PrimaryKeyName(recipeDocument.RecipeEntity);
                                    var recipeName = recipeDocument.RecipeEntity.Name;

                                    readFile.ReadLine(); //writeFile.WriteLine(UFRecipeLayout.Properties.Resources.ExportWhiteSpace);
                                    var line = readFile.ReadLine(); //writeFile.WriteLine(string.Format(UFRecipeLayout.Properties.Resources.ExportItemSelected, recipeName));
                                    if (!line.Contains(string.Format("{{{0}}}", recipeName)))
                                        throw new WarningException(UFRecipeLayout.Properties.Resources.BadImportRecipeName);
                                    readFile.ReadLine(); //writeFile.WriteLine(UFRecipeLayout.Properties.Resources.ExportWhiteSpace);

                                    StringBuilder retColumns = new StringBuilder();
                                    StringBuilder retValues = new StringBuilder();

                                    var guid = Guid.Empty;
                                    if (Guid.TryParse(recipeId, out guid))
                                    {
                                        for (int cc = 0; cc < dataSet.Tables.Count; cc++)
                                        {
                                            token.ThrowIfCancellationRequested();

                                            DataView dataView = new DataView(dataSet.Tables[cc]);

                                            String rowFilter = null;
                                            rowFilter = String.Format("[{0}]='{1}'", dataView.Table.PrimaryKey[0].ColumnName, guid);

                                            dataView.RowFilter = rowFilter;
                                            dataView.RowStateFilter = DataViewRowState.CurrentRows;

                                            readFile.ReadLine(); //writeFile.WriteLine();
                                            line = readFile.ReadLine(); //writeFile.WriteLine(string.Format(Properties.Resources.ExportGroup, dataView.Table.TableName));
                                            if (!line.Contains(string.Format("{{{0}}}", dataView.Table.TableName)))
                                            {
                                                throw new WarningException(UFRecipeLayout.Properties.Resources.BadImportRecipeFormat);
                                            }
                                            readFile.ReadLine(); //writeFile.WriteLine();


                                            retColumns.Append(readFile.ReadLine()); //writeFile.WriteLine(retColumns.ToString());
                                            retValues.Append(readFile.ReadLine()); //writeFile.WriteLine(retValues.ToString());

                                            var columnlist = retColumns.ToString().Split(';');
                                            var valuelist = retValues.ToString().Split(';');
                                            foreach (DataRowView rowView in dataView)
                                            {
                                                token.ThrowIfCancellationRequested();

                                                foreach (DataColumn column in rowView.Row.Table.Columns)
                                                {
                                                    token.ThrowIfCancellationRequested();

                                                    if (column.DataType != typeof(System.DateTime) && column.ColumnName != recipePrimaryKeyName && column.ColumnName != recipeName)
                                                    {
                                                        if (columnlist.Contains(column.ColumnName))
                                                        {
                                                            try
                                                            {
                                                                string v = valuelist.ElementAt(Array.IndexOf(columnlist, column.ColumnName));
                                                                rowView[column.ColumnName] = TypeExtensions.ChangeType(v, column.DataType, force: true);
                                                            }
                                                            catch
                                                            { }
                                                        }
                                                    }
                                                }

                                            }

                                            retColumns.Clear();
                                            retValues.Clear();
                                        }

                                        for (int cc = 0; cc < dataSet.Tables.Count; cc++)
                                        {
                                            token.ThrowIfCancellationRequested();

                                            if (dataSet.Tables[cc].DefaultView.RowStateFilter != DataViewRowState.CurrentRows)
                                                dataSet.Tables[cc].DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
                                            dataSet.Tables[cc].DefaultView.RowFilter = String.Empty;
                                            dataSet.Tables[cc].DefaultView.RowFilter = String.Format("[{0}]='{1}'",
                                                                                        dataSet.Tables[cc].PrimaryKey[0].ColumnName,
                                                                                        guid);
                                        }
                                    }
                                    else
                                    {
                                        throw new WarningException(UFRecipeLayout.Properties.Resources.ImportActionWarning);
                                    }
                                }
                            }, token);

                            task.ContinueWith(ret =>
                            {
                                pendingTask.Remove(task);
                                if (pendingTask.Count == 0)
                                    viewModel.IsBusy = false;

                                if (token.IsCancellationRequested)
                                    return;
                                else if (ret.Exception != null)
                                    viewModel.LastError = ret.Exception.InnerException.Message;
                            }, TaskScheduler.FromCurrentSynchronizationContext());

                            pendingTask.Add(task);
                            viewModel.IsBusy = true;
                        }
                    }, param =>
                    {
                        return viewModel != null && viewModel.IsReady && pendingTask.Count == 0 && UIInterface != null;
                    });
                }

                return importCommand;
            }
        }

        RelayCommand exportCommand;
        [Browsable(false)]
        public ICommand ExportCommand
        {
            get
            {
                if (exportCommand == null)
                {
                    exportCommand = new RelayCommand(param =>
                    {
                        // export recipe
                        if (UIInterface != null)
                        {
                            if (!gridControl.ValidateBindings())
                            {
                                viewModel.LastError = UFRecipeLayout.Properties.Resources.InvalidBindingWarning;
                                return;
                            }

                            var file = UIInterface.ShowSaveFileDialog(UFRecipeLayout.Properties.Resources.CSVFilter, new FileSaveOptions() { AddExtension = true, OverwritePrompt = false });
                            if (String.IsNullOrEmpty(file))
                                return;

                            if (System.IO.File.Exists(file))
                            {
                                var ret = UIInterface.ShowYesNoCancel(UFRecipeLayout.Properties.Resources.ExportFileWarning, CustomDialogIcons.Question);
                                if (ret == CustomDialogResults.Cancel)
                                    return;
                                else if (ret == CustomDialogResults.No)
                                {
                                    int i = 1;
                                    while (System.IO.File.Exists(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), string.Format("{0}{1}{2}", System.IO.Path.GetFileNameWithoutExtension(file), i.ToString(), System.IO.Path.GetExtension(file).ToLower()))))
                                    {
                                        i++;
                                    }
                                    file = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), string.Format("{0}{1}{2}", System.IO.Path.GetFileNameWithoutExtension(file), i.ToString(), System.IO.Path.GetExtension(file).ToLower()));
                                }
                            }

                            if (ctsPendingTask == null)
                                ctsPendingTask = new CancellationTokenSource();
                            CancellationToken token = ctsPendingTask.Token;

                            var task = Task.Factory.StartNew(() =>
                            {
                                using (System.IO.StreamWriter writeFile = new System.IO.StreamWriter(file))
                                {
                                    var recipeEntity = recipeDocument.RecipeEntity;
                                    var recipeId = dataSet.Tables[DataSetHelper.TableName(recipeEntity)].DefaultView[0].Row[DataSetHelper.PrimaryKeyName(recipeEntity)].ToString();
                                    var recipePrimaryKeyName = DataSetHelper.PrimaryKeyName(recipeDocument.RecipeEntity);
                                    var recipeName = recipeDocument.RecipeEntity.Name;

                                    writeFile.WriteLine(UFRecipeLayout.Properties.Resources.ExportWhiteSpace);
                                    writeFile.WriteLine(string.Format(UFRecipeLayout.Properties.Resources.ExportRecipeHeader, recipeName));
                                    writeFile.WriteLine(UFRecipeLayout.Properties.Resources.ExportWhiteSpace);

                                    StringBuilder retColumns = new StringBuilder();
                                    StringBuilder retValues = new StringBuilder();

                                    var guid = Guid.Empty;
                                    if (Guid.TryParse(recipeId, out guid))
                                    {
                                        for (int cc = 0; cc < dataSet.Tables.Count; cc++)
                                        {
                                            token.ThrowIfCancellationRequested();

                                            DataView dataView = new DataView(dataSet.Tables[cc]);

                                            String rowFilter = null;
                                            rowFilter = String.Format("[{0}]='{1}'", dataView.Table.PrimaryKey[0].ColumnName, guid);

                                            dataView.RowFilter = rowFilter;
                                            dataView.RowStateFilter = DataViewRowState.CurrentRows;

                                            writeFile.WriteLine();
                                            writeFile.WriteLine(string.Format(UFRecipeLayout.Properties.Resources.ExportGroupHeader, dataView.Table.TableName));
                                            writeFile.WriteLine();

                                            foreach (DataRowView rowView in dataView)
                                            {
                                                token.ThrowIfCancellationRequested();

                                                foreach (DataColumn column in rowView.Row.Table.Columns)
                                                {
                                                    token.ThrowIfCancellationRequested();

                                                    if (column.DataType != typeof(System.DateTime) && column.ColumnName != recipePrimaryKeyName && column.ColumnName != recipeName)
                                                    {
                                                        if (retColumns.Length > 0)
                                                            retColumns.Append(';');
                                                        retColumns.Append(column.ColumnName);
                                                        string v = Convert.ToString(rowView[column.ColumnName], CultureInfo.InvariantCulture);
                                                        if (retValues.Length > 0)
                                                            retValues.Append(';');
                                                        retValues.Append(v);
                                                    }
                                                }

                                            }

                                            writeFile.WriteLine(retColumns.ToString());
                                            writeFile.WriteLine(retValues.ToString());

                                            retColumns.Clear();
                                            retValues.Clear();
                                        }

                                        writeFile.WriteLine();
                                        writeFile.WriteLine(UFRecipeLayout.Properties.Resources.ExportWhiteSpace);
                                    }
                                }
                            }, token);

                            task.ContinueWith(ret =>
                            {
                                pendingTask.Remove(task);
                                if (pendingTask.Count == 0)
                                    viewModel.IsBusy = false;

                                if (token.IsCancellationRequested)
                                    return;
                                else if (ret.Exception != null)
                                    viewModel.LastError = ret.Exception.InnerException.Message;
                            }, TaskScheduler.FromCurrentSynchronizationContext());

                            pendingTask.Add(task);
                            viewModel.IsBusy = true;
                        }
                    }, param =>
                    {
                        return viewModel != null && viewModel.IsReady && pendingTask.Count == 0 && UIInterface != null;
                    });
                }

                return exportCommand;
            }
        }

        RelayCommand readCommand;
        [Browsable(false)]
        public ICommand ReadCommand
        {
            get
            {
                if (readCommand == null)
                {
                    readCommand = new RelayCommand(param =>
                    {
                        // read recipe
                        if (dataSet.Tables[DataSetHelper.TableName(recipeDocument.RecipeEntity)].DefaultView.Count == 0)
                            return;

                        var recipeId = dataSet.Tables[DataSetHelper.TableName(recipeDocument.RecipeEntity)].DefaultView[0].Row[DataSetHelper.PrimaryKeyName(recipeDocument.RecipeEntity)].ToString();
                        int readTimeout = ReadWriteTimeOut;

                        if (ctsPendingTask == null)
                            ctsPendingTask = new CancellationTokenSource();
                        CancellationToken token = ctsPendingTask.Token;

                        var task = Task.Factory.StartNew(() =>
                        {
                            // notify command execution
                            var guid = Guid.Empty;
                            if (Guid.TryParse(recipeId, out guid))
                            {
                                recipeExecuter.GetInDataServerValues(dataSet, guid, readTimeout, token);
                            }
                            else
                            {
                                throw new WarningException(UFRecipeLayout.Properties.Resources.ReadCommandExecutionResultWarning);
                            }
                        }, token);

                        task.ContinueWith(ret =>
                        {
                            pendingTask.Remove(task);
                            if (pendingTask.Count == 0)
                                viewModel.IsBusy = false;

                            if (token.IsCancellationRequested)
                                return;
                            else if (ret.Exception != null)
                            {
                                if (readIconError != null)
                                    readBtnImage.Source = readIconError;
                                viewModel.LastError = String.Format(UFRecipeLayout.Properties.Resources.ReadCommandExecutionResultError, ret.Exception.InnerException.Message);
                            }
                            else if(readIconOk != null)
                                    readBtnImage.Source = readIconOk;
                        }, TaskScheduler.FromCurrentSynchronizationContext());

                        pendingTask.Add(task);
                        viewModel.IsBusy = true;
                    }, param =>
                    {
                        return viewModel != null && viewModel.IsReady && pendingTask.Count == 0 && bCanGetInDataServerValues;
                    });
                }

                return readCommand;
            }
        }

        RelayCommand writeCommand;
        [Browsable(false)]
        public ICommand WriteCommand
        {
            get
            {
                if (writeCommand == null)
                {
                    writeCommand = new RelayCommand(param =>
                    {
                        // write recipe
                        if (dataSet.Tables[DataSetHelper.TableName(recipeDocument.RecipeEntity)].DefaultView.Count == 0)
                            return;

                        if (!gridControl.ValidateBindings())
                        {
                            viewModel.LastError = UFRecipeLayout.Properties.Resources.InvalidBindingWarning;
                            return;
                        }

                        var recipeId = dataSet.Tables[DataSetHelper.TableName(recipeDocument.RecipeEntity)].DefaultView[0].Row[DataSetHelper.PrimaryKeyName(recipeDocument.RecipeEntity)].ToString();
                        var recipeIndex = dataSet.Tables[DataSetHelper.TableName(recipeDocument.RecipeEntity)].DefaultView[0].Row[DataSetHelper.ColumnName(recipeDocument.RecipeEntity)].ToString();
                        int writeTimeout = ReadWriteTimeOut;

                        if (ctsPendingTask == null)
                            ctsPendingTask = new CancellationTokenSource();
                        CancellationToken token = ctsPendingTask.Token;

                        var task = Task.Factory.StartNew(() =>
                        {
                            // notify command execution
                            var guid = Guid.Empty;
                            if (Guid.TryParse(recipeId, out guid))
                            {
                                if (isAuditTraceSupported &&
                                    recipeExecuter.RecipeUAViewModel != null &&
                                    recipeExecuter.RecipeUAViewModel.IsAuditTraceEnabled &&
                                    recipeExecuter.RecipeUAViewModel.IsCommentRequired)
                                {
                                    Dispatcher.Invoke(() =>
                                    {
                                        var auditTraceViewModel = new RecipeAuditTrace.RecipeAuditTraceViewModel(recipeExecuter.RecipeUAViewModel)
                                        {
                                            Control = writeButton,
                                            RecipeIndex = recipeIndex,
                                            RecipeCommand = UFRecipeExecutionContext.RecipeCommandType.Activate
                                        };
                                        auditTraceViewModel.Execute += (s, e) =>
                                        {
                                            recipeExecuter.GetOutDataServerValues(dataSet, guid, writeTimeout, token, userComment: auditTraceViewModel.AuditComment);
                                        };
                                        auditTraceViewModel.AskUserComment();
                                    });
                                }
                                else
                                    recipeExecuter.GetOutDataServerValues(dataSet, guid, writeTimeout, token);
                            }
                            else
                            {
                                throw new WarningException(UFRecipeLayout.Properties.Resources.WriteCommandExecutionResultWarning);
                            }
                        }, token);

                        task.ContinueWith(ret =>
                        {
                            pendingTask.Remove(task);
                            if (pendingTask.Count == 0)
                                viewModel.IsBusy = false;

                            if (token.IsCancellationRequested)
                                return;
                            else if (ret.Exception != null)
                            {
                                if (writeIconError != null)
                                    writeBtnImage.Source = writeIconError;
                                viewModel.LastError = String.Format(UFRecipeLayout.Properties.Resources.WriteCommandExecutionResultError, ret.Exception.InnerException.Message);
                            }
                            else if (writeIconOk != null)
                                writeBtnImage.Source = writeIconOk;
                        }, TaskScheduler.FromCurrentSynchronizationContext());

                        pendingTask.Add(task);
                        viewModel.IsBusy = true;
                    }, param =>
                    {
                        return viewModel != null && viewModel.IsReady && pendingTask.Count == 0 && bCanGetOutDataServerValues;
                    });
                }

                return writeCommand;
            }
        }
        #endregion

        #region Properties
        IRecipeEditorManager recipeEditorManager;
        [Browsable(false)]
        IRecipeEditorManager RecipeEditorManager
        {
            get
            {
                if (recipeEditorManager == null && document != null)
                    recipeEditorManager = document.GetService(typeof(IRecipeEditorManager)) as IRecipeEditorManager;
                return recipeEditorManager;
            }
        }

        IUFUAEditorManager ufuaEditorService;
        IUFUAEditorManager UfuaEditorService
        {
            get
            {
                if (ufuaEditorService == null && document != null)
                    ufuaEditorService = document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                return ufuaEditorService;
            }
        }

        IStringEditorManager stringManager;
        internal IStringEditorManager StringManager
        {
            get
            {
                if (stringManager == null && document != null)
                    stringManager = document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                return stringManager;
            }
        }

        IUIMsgBoxAlertService uiInterface;
        [Browsable(false)]
        internal IUIMsgBoxAlertService UIInterface
        {
            get
            {
                if (uiInterface == null && document != null)
                    uiInterface = document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                return uiInterface;
            }
        }

        //[Browsable(false)]
        RecipeGridViewModel viewModel
        {
            get
            {
                return thisControl.DataContext as RecipeGridViewModel;
            }
        }

        public static readonly DependencyProperty EditLayoutProperty = DependencyProperty.Register("EditLayout", typeof(bool), typeof(RecipeGrid), new UIPropertyMetadata(false));

        [XmlIgnore]
        [MergablePropertyAttribute(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EditLayout
        {
            get
            {
                return (bool)GetValue(EditLayoutProperty);
            }
        }

        //[Browsable(false)]
        internal bool RunningOnServer
        {
            get
            {
                return ScreenSettings.ScreenDocument.GetRunningOnServer(this);
            }
        }
        #endregion

        #region Events
        public event EventHandler ControlLoaded;
        void OnControlLoaded()
        {
            ControlLoaded?.Invoke(this, EventArgs.Empty);
        }

        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(e.NewValue is RecipeGridViewModel))
                DataContext = e.OldValue;
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

                dt = new DataTemplate();
                factory = new FrameworkElementFactory(typeof(Controls.EditLayoutPropertyEditor));
                factory.SetValue(Controls.EditLayoutPropertyEditor.DocumentProperty, document);
                dt.DataType = typeof(bool);
                dt.VisualTree = factory;
                mapDataTemplates.Add(EditLayoutProperty, dt);

                return mapDataTemplates;
            }
        }

        #endregion

        #region ICheckUserCallable
        public void CheckUserCallable()
        {
            bool bInvalidateRequerySuggested = false;
            bool bCan = recipeExecuter.CanGetInDataServerValues();
            if (bCan != bCanGetInDataServerValues)
            {
                bCanGetInDataServerValues = bCan;
                bInvalidateRequerySuggested = true;
            }
            
            bCan = recipeExecuter.CanGetOutDataServerValues();
            if (bCan != bCanGetOutDataServerValues)
            {
                bCanGetOutDataServerValues = bCan;
                bInvalidateRequerySuggested = true;
            }

            bCan = recipeExecuter.CanReadXmlDataSet();
            if (bCan != bCanReadXmlDataSet)
            {
                bCanReadXmlDataSet = bCan;
                bInvalidateRequerySuggested = true;
            }

            bCan = recipeExecuter.CanWriteXmlDataSet();
            if (bCan != bCanWriteXmlDataSet)
            {
                bCanWriteXmlDataSet = bCan;
                bInvalidateRequerySuggested = true;
            }

            if (bInvalidateRequerySuggested)
            {
                Dispatcher.BeginInvokeIfRequired(() =>
                {
                    System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                });
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

            MouseEnter -= GridControl_MouseEnter;
            MouseDown -= GridControl_MouseDown;

            DetachOverrideBaseProperties();

            if (recipeExecuter != null && recipeExecuter.RecipeUAViewModel != null)
            {
                recipeExecuter.RecipeUAViewModel.PropertyChanged -= RecipeUAViewModel_PropertyChanged;
                Utilities.Commands.CheckUserCallable.GetInstance().Remove(this);
            }

            if (StringManager != null)
                StringManager.CultureChanged -= StringManager_CultureChanged;

            if (ctsLoading != null)
            {
                ctsLoading.Cancel();
                ctsLoading.Dispose();
            }

            if (ctsPendingTask != null)
                ctsPendingTask.Cancel();

            if (pendingTask.Count > 0)
                Task.WaitAll(pendingTask.ToArray());

            if (ctsPendingTask != null)
                ctsPendingTask.Dispose();

            if (recipeExecuter != null)
                recipeExecuter.Dispose();
            if (recipeDocument != null)
                recipeDocument.Dispose();
            if (dataSet != null)
                dataSet.Dispose();

            recipeIndexUI.Dispose();
            foreach (var valueUI in gridControl.GetVisualChildrenOfType<IDisposable>())
                valueUI.Dispose();

            if (viewModel != null)
                viewModel.Dispose();
        }

        #endregion
        [Browsable(false)]
        [XmlIgnore]
        [SvgValueConverter(ConverterType = typeof(ConvertDefaultToolbarBackground), RequiredKey = true)]
        public Brush ToolbarBackground { get { return Background; } }
        [Browsable(false)]
        [XmlIgnore]
        [SvgValueConverter(ConverterType = typeof(ConvertDefaultToolbarForeground), RequiredKey = true)]
        public Brush ToolbarForeground { get { return Foreground; } }
    }

    [SvgValueConverter(typeof(ConvertUFDataValueEntityList))]
    public class UFDataValueEntityList : List<UFDataValueEntity>
    {
        #region Constructors
        public UFDataValueEntityList()
        { }

        public UFDataValueEntityList(List<UFDataValueEntity> instance)
        {
            if (instance == null)
                return;

            foreach (var item in instance)
                Add(new UFDataValueEntity(item));
        }
        #endregion

        public List<object> ToDictionary()
        {
            List<object> res = new List<object>();
            this.ToList().ForEach(p => res.Add(p.ToDictionary()));
            return res;
        }
    }

    public class ConvertUFDataValueEntityList : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            if ((document as IDocument) == null)
                return (new UFDataValueEntityList()).ToDictionary();
            RecipeGrid recipeGrid = sender as RecipeGrid;
            var recipeName = recipeGrid.RecipeName;
            var recipeUri = (document as IDocument).MakeAbosoluteUri(recipeName);
            var recipeDocument = UFRecipeDocument.FromFile(recipeUri.GetPathString(), (document as IDocument));
            if (recipeDocument == null || recipeDocument.RecipeEntity == null)
                return (new UFDataValueEntityList()).ToDictionary();
            UFDataValueEntityList dataValues = new UFDataValueEntityList();
            if(recipeDocument.RecipeEntity.DataValues.Count() > 0 || recipeDocument.RecipeEntity.Groups.Count > 0)
                dataValues.AddRange(recipeDocument.RecipeEntity.GetFlatDataValuesCollection());
            return dataValues.ToDictionary();
        }
        public override Type StorageType
        {
            get
            {
                return typeof(List<object>);
            }
        }
    }

    public class ConvertGridLayout : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender = null)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            RecipeGrid recipeGrid = sender as RecipeGrid;
            var gridLayout = value as string;
            List<object> res = new List<object>();
            var columnlist = (from column in recipeGrid.gridControl.Columns where column.Visible == true orderby column.VisibleIndex select column);
            columnlist.ToList().ForEach(c =>
            {
                res.Add(new Dictionary<string, object>() {
                            { "FieldName", c.FieldName },
                            {"ActualWidth", c.ActualWidth }});
            });

            return res;
        }
        public override Type StorageType
        {
            get
            {
                return typeof(Dictionary<string, object>);
            }
        }
    }

    internal class ConvertDefaultBrushValue : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertFromStorageType(object value, object sender, object document)
        {
            Dictionary<string, Brush> ret = new Dictionary<string, Brush>();
            if (sender is RecipeGrid)
            {
                RecipeGrid control = sender as RecipeGrid;
                Brush foreground = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.BorderBrush; ;
                Brush background = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.Background;
                if (control.ReadLocalValue(RecipeGrid.ForegroundProperty) != DependencyProperty.UnsetValue)
                    ret.Add("Foreground", control.Foreground);
                else
                    ret.Add("Foreground", foreground);

                if (control.ReadLocalValue(RecipeGrid.BackgroundProperty) != DependencyProperty.UnsetValue && control.ReadLocalValue(RecipeGrid.BackgroundProperty) != null)
                    ret.Add("Background", control.Background);
                else
                    ret.Add("Background", background);
            }
            return ret;
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            if (sender == null)
                return null;
            var cvalue = (Brush)value;
            string prop = ((DependencyProperty)property).Name;
            RecipeGrid gridControl = sender as RecipeGrid;
            Brush defColor = (sender as RecipeGrid).Foreground;
            Brush foreground = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.BorderBrush; ;
            Brush background = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.Background;
            bool useDefColor = true;
            try
            {
                if (prop.Equals(RecipeGrid.RowAreaForegroundProperty.Name))
                    defColor = ThemeHelper.GetLightWeightThemeBrush((document as ScreenDocument).Theme);
                else if (prop.Equals(RecipeGrid.FocusedRowBackgroundProperty.Name))
                    defColor = ThemeHelper.GetGridHilightingThemeBrush((document as ScreenDocument).Theme, false);
                else if (prop.Equals(RecipeGrid.FocusedRowForegroundProperty.Name))
                    defColor = ThemeHelper.GetGridHilightingThemeBrush((document as ScreenDocument).Theme, true);
                else if (prop.Equals(RecipeGrid.FocusedCellBackgroundProperty.Name))
                    defColor = ThemeHelper.GetGridHilightingThemeBrush((document as ScreenDocument).Theme, false);
                else if (prop.Equals(RecipeGrid.FocusedCellForegroundProperty.Name))
                    defColor = ThemeHelper.GetGridHilightingThemeBrush((document as ScreenDocument).Theme, true);
                else if (prop.Equals(RecipeGrid.HeaderForegroundProperty.Name))
                    defColor = foreground;
                else if (prop.Equals(RecipeGrid.HeaderBackgroundProperty.Name))
                    defColor = background;
                else
                    useDefColor = false;
            }
            catch (Exception)
            {
                return value;
            }

            Brush res = (sender as GridControl).GetUnsetPropertyValue<Brush>((DependencyProperty)property, cvalue, useDefColor ? defColor : null, false);
            return res;
        }

        public override Type StorageType
        {
            get
            {
                return typeof(string);
            }
        }
    }

    internal class ConvertDefaultToolbarBackground : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertFromStorageType(object value, object sender, object document)
        {
            Dictionary<string, Brush> ret = new Dictionary<string, Brush>();
            return ret;
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            if (sender == null)
                return null;

            RecipeGrid control = sender as RecipeGrid;
            Brush defColor = control.Foreground;
            Brush background = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.Background;

            if (background is SolidColorBrush)
            {
                SolidColorBrush solidColorBrush = (background as SolidColorBrush);

                return new SolidColorBrush(WPFUtilities.DeployHelper.GetColorInContrast(solidColorBrush.Color, (document as ScreenDocument).Theme));
            }
            else if (background is LinearGradientBrush)
            {
                LinearGradientBrush linearGradientBrush = background.Clone() as LinearGradientBrush;
                if (linearGradientBrush.GradientStops.Count > 0)
                {
                    linearGradientBrush.GradientStops.ToList().ForEach(gradient =>
                    {
                        gradient.Color = WPFUtilities.DeployHelper.GetColorInContrast(gradient.Color, (document as ScreenDocument).Theme);
                    });
                }
                return linearGradientBrush;
            }

            return background;
        }

        public override Type StorageType
        {
            get
            {
                return typeof(string);
            }
        }
    }

    internal class ConvertDefaultToolbarForeground : CustomValueConverter
    {
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertFromStorageType(object value, object sender, object document)
        {
            Dictionary<string, Brush> ret = new Dictionary<string, Brush>();
            return ret;
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property)
        {
            if (sender == null)
                return null;

            RecipeGrid control = sender as RecipeGrid;
            Brush defColor = control.Foreground;
            Brush foreground = WPFUtilities.DeployHelper.GetBorderResources<Border>(sender as FrameworkElement, "editArea", (document as ScreenDocument).Theme)?.BorderBrush; ;

            return foreground;
        }

        public override Type StorageType
        {
            get
            {
                return typeof(string);
            }
        }
    }
}
