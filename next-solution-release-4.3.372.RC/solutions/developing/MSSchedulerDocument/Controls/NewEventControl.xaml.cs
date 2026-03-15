using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MSSchedulerSettings.Document;
using Utilities;
using Utilities.WPF;
using OPCUAViewModel;
using DevExpress.XtraScheduler;
using DevExpress.Xpf.Scheduler;
using System.ComponentModel;
using WPFUtilities.PropertyDataTemplate;
using System.Windows.Threading;
using TranslationHelpers;
using System.IO.IsolatedStorage;
using System.Xml;
using System.Runtime.Serialization;
using System.Reflection;
using System.IO;
using DevExpress.Xpf.Core;
using DocumentManager.ComponentService;
using Nager.Date;
using DevExpress.Mvvm.Native;
using System.Globalization;
using static WPFUtilities.WebConfirmationDialog;
using WPFUtilities;
using DevExpress.Xpf.Editors;
using CommandManager;
using CommandManager.ComponentService;
using System.Windows.Data;
using WPFUtilities.Converters;
using WPFUtilities.Extensions;
using System.Xml.Serialization;
using UIMsgBoxAlertService.ComponentService;

namespace MSSchedulerSettings.Controls
{
    /// <summary>
    /// Interaction logic for NewEventControl.xaml
    /// </summary>
    public partial class NewEventControl : UserControl, IDisposable
    {
        #region DP
        #region DP Foreground

        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(NewEventControl));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);
            OnForegroundChanged();
        }
        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(NewEventControl));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
        }


        private void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as NewEventControl;
            if (control != null)
            {
                control.OnForegroundChanged();
            }
        }
        protected virtual void OnForegroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            (from c in (MainGrid as UIElement).GetVisualChildrenOfType<TextBox>()
             select c).ToList().ForEach(child =>
             {
                 (child as TextBox).Foreground = Foreground;
             });
            (from c in (MainGrid as UIElement).GetVisualChildrenOfType<TextBlock>()
             select c).ToList().ForEach(child =>
             {
                 (child as TextBlock).Foreground = Foreground;
             });
            (from c in (MainGrid as UIElement).GetVisualChildrenOfType<DXTabItem>()
             select c).ToList().ForEach(child =>
             {
                 (child as DXTabItem).Foreground = Foreground;
             });
        }
        #endregion

        #region WeeklyEventBrush
        public static readonly DependencyProperty WeeklyEventBrushProperty = DependencyProperty.Register("WeeklyEventBrush", typeof(Brush), typeof(NewEventControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnWeeklyEventBrushChanged), new CoerceValueCallback(OnCoerceWeeklyEventBrush)));

        private static object OnCoerceWeeklyEventBrush(DependencyObject o, object value)
        {
            NewEventControl control = o as NewEventControl;
            if (control != null)
                return control.OnCoerceWeeklyEventBrush((Brush)value);
            else
                return value;
        }

        private static void OnWeeklyEventBrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            NewEventControl control = o as NewEventControl;
            if (control != null)
                control.OnWeeklyEventBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }

        protected virtual Brush OnCoerceWeeklyEventBrush(Brush value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnWeeklyEventBrushChanged(Brush oldValue, Brush newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public Brush WeeklyEventBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(WeeklyEventBrushProperty);
            }
            set
            {
                SetValue(WeeklyEventBrushProperty, value);
            }
        }

        #endregion


        #region TimeScale
        public static readonly DependencyProperty TimeScaleProperty = DependencyProperty.Register("TimeScale", typeof(TimeSpan), typeof(NewEventControl), new UIPropertyMetadata(new TimeSpan(0, 1, 0, 0)));
        [Browsable(false)]
        [XmlIgnore]
        public TimeSpan TimeScale
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (TimeSpan)GetValue(TimeScaleProperty);
            }
            set
            {
                SetValue(TimeScaleProperty, value);
            }
        }

        #endregion


        #region WorkViewStartTime
        public static readonly DependencyProperty WorkViewStartTimeProperty = DependencyProperty.Register("WorkViewStartTime", typeof(TimeSpan), typeof(NewEventControl), new UIPropertyMetadata(new TimeSpan(0, 8, 0, 0), new PropertyChangedCallback(OnWorkViewStartTimeChanged), new CoerceValueCallback(OnCoerceWorkViewStartTime)));

        private static object OnCoerceWorkViewStartTime(DependencyObject o, object value)
        {
            NewEventControl control = o as NewEventControl;
            if (control != null)
                return control.OnCoerceWorkViewStartTime((TimeSpan)value);
            else
                return value;
        }

        private static void OnWorkViewStartTimeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            NewEventControl control = o as NewEventControl;
            if (control != null)
                control.OnWorkViewStartTimeChanged((TimeSpan)e.OldValue, (TimeSpan)e.NewValue);
        }

        protected virtual TimeSpan OnCoerceWorkViewStartTime(TimeSpan value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnWorkViewStartTimeChanged(TimeSpan oldValue, TimeSpan newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            schedulerControl1.WorkViewStartTime = newValue;
        }

        public TimeSpan WorkViewStartTime
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (TimeSpan)GetValue(WorkViewStartTimeProperty);
            }
            set
            {
                SetValue(WorkViewStartTimeProperty, value);
            }
        }

        #endregion

        #region WorkViewEndTime
        public static readonly DependencyProperty WorkViewEndTimeProperty = DependencyProperty.Register("WorkViewEndTime", typeof(TimeSpan), typeof(NewEventControl), new UIPropertyMetadata(new TimeSpan(0, 18, 0, 0), new PropertyChangedCallback(OnWorkViewEndTimeChanged), new CoerceValueCallback(OnCoerceWorkViewEndTime)));

        private static object OnCoerceWorkViewEndTime(DependencyObject o, object value)
        {
            NewEventControl control = o as NewEventControl;
            if (control != null)
                return control.OnCoerceWorkViewEndTime((TimeSpan)value);
            else
                return value;
        }

        private static void OnWorkViewEndTimeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            NewEventControl control = o as NewEventControl;
            if (control != null)
                control.OnWorkViewEndTimeChanged((TimeSpan)e.OldValue, (TimeSpan)e.NewValue);
        }

        protected virtual TimeSpan OnCoerceWorkViewEndTime(TimeSpan value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnWorkViewEndTimeChanged(TimeSpan oldValue, TimeSpan newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            schedulerControl1.WorkViewEndTime = newValue;
        }

        public TimeSpan WorkViewEndTime
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (TimeSpan)GetValue(WorkViewEndTimeProperty);
            }
            set
            {
                SetValue(WorkViewEndTimeProperty, value);
            }
        }

        #endregion

        #region ShowWorkTimeOnly
        public static readonly DependencyProperty ShowWorkTimeOnlyProperty = DependencyProperty.Register("ShowWorkTimeOnly", typeof(bool), typeof(NewEventControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowWorkTimeOnlyChanged), new CoerceValueCallback(OnCoerceShowWorkTimeOnly)));

        private static object OnCoerceShowWorkTimeOnly(DependencyObject o, object value)
        {
            NewEventControl control = o as NewEventControl;
            if (control != null)
                return control.OnCoerceShowWorkTimeOnly((bool)value);
            else
                return value;
        }

        private static void OnShowWorkTimeOnlyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            NewEventControl control = o as NewEventControl;
            if (control != null)
                control.OnShowWorkTimeOnlyChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowWorkTimeOnly(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowWorkTimeOnlyChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            schedulerControl1.ShowWorkTimeOnly = newValue;
        }

        public bool ShowWorkTimeOnly
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowWorkTimeOnlyProperty);
            }
            set
            {
                SetValue(ShowWorkTimeOnlyProperty, value);
            }
        }

        #endregion

        #region SelectionColor
        public static readonly DependencyProperty SelectionColorProperty = DependencyProperty.Register("SelectionColor", typeof(Brush), typeof(NewEventControl), new UIPropertyMetadata(null));
        public Brush SelectionColor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(SelectionColorProperty);
            }
            set
            {
                SetValue(SelectionColorProperty, value);
            }
        }
        #endregion

        #region MultiSelectionColor
        public static readonly DependencyProperty MultiSelectionColorProperty = DependencyProperty.Register("MultiSelectionColor", typeof(Brush), typeof(NewEventControl), new UIPropertyMetadata(null));
        public Brush MultiSelectionColor
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(MultiSelectionColorProperty);
            }
            set
            {
                SetValue(MultiSelectionColorProperty, value);
            }
        }
        #endregion
        #endregion
        #region Declarations
        SchedulerEditorDocument Document;
        MSModel.MSScheduledAction schedEvent;
        MSModel.MSScheduledAction SchedEvent
        {
            get
            {
                return schedEvent;
            }
            set
            {
                if (bRuntime && schedEvent != null && schedEvent != value)
                    SaveRuntimeLayout(GetStorageName());
                schedEvent = value;
            }
        }
        CultureInfo currentCulture = CultureInfo.CurrentUICulture;
        public CultureInfo CurrentCulture
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return currentCulture;
            }
            set
            {
                if (currentCulture != value)
                {
                    currentCulture = value;
                    if (newcal != null)
                        newcal.CurrentCulture = currentCulture;
                }
            }
        }

        bool runningOnServer;
        IUIMsgBoxAlertService iUIMsgBoxAlertService;
        ContentControl webPopupHost = null;
        NewCalendarItem newcal;
        public WebConfirmationDialog WebDialogUC
        {
            get;
            set;
        }
        #endregion

        bool alreadyLoaded;
        string currentStyle = ApplicationPropertiesHelper.GetProperty<String>("CurrentSkin");
        readonly bool bRuntime;
        readonly bool bHideGeneralSettings;
        ComboBox comboTypeUsed;
        ComboBox comboAccessRoleUsed;

        public NewEventControl(SchedulerEditorDocument doc, bool runningOnServer = false, bool bRuntime = false, string styleName = null, bool openExceptions = false, bool bEditing = false, bool bHideGeneralSettings = false)
        {
            InitializeComponent();

            this.bRuntime = bRuntime;
            this.bHideGeneralSettings = bHideGeneralSettings;

            textEditEnableItem.DesignMode = textScheduleItemName.DesignMode = bEditing;
            textEditEnableItem.Skin = textScheduleItemName.Skin = styleName;
            textEditEnableItem.Document = textScheduleItemName.Document = doc;
#if !NET_STANDARD
            ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, doc, !bEditing);
            bitmaskImage.SetResourceReference(Image.SourceProperty, "EditGeneralSmall");
            commandsOffImage.SetResourceReference(Image.SourceProperty, "EditGeneralSmall");
            commandsOnImage.SetResourceReference(Image.SourceProperty, "EditGeneralSmall");
#endif


            if (bEditing)
                lblCommandsOff.Visibility = commandsOff.Visibility = lblCommandsOn.Visibility = commandsOn.Visibility = Visibility.Visible;

            this.runningOnServer = runningOnServer;

            comboTypeUsed = comboType;
            comboAccessRoleUsed = comboAccessRole;
            if (runningOnServer)
            {
                comboType.Visibility = comboAccessRole.Visibility = Visibility.Collapsed;
                comboType_web.Visibility = comboAccessRole_web.Visibility = Visibility.Visible;
                comboTypeUsed = comboType_web;
                comboAccessRoleUsed = comboAccessRole_web;
            }

            Document = doc;
            schedulerControl1.Document = Document;
            currentStyle = styleName;
            iUIMsgBoxAlertService = Document?.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;

            Loaded += (o, e) =>
            {
                if (alreadyLoaded)
                    return;

                SchedEvent = DataContext as MSModel.MSScheduledAction;
                comboTypeUsed.SelectionChanged -= comboType_SelectionChanged;
                foreach (var item in Enum.GetValues(typeof(MSModel.ScheduleType)))
                {
                    comboTypeUsed.Items.Add(new ComboBoxItem() { Tag = item });
                }

                TranslateCombo(_stringlist, stringPlaceolder);
                //schedulerControl1.Stringlist = _stringlist;
                //schedulerControl1.StringPlaceolder = stringPlaceolder;
                if (SchedEvent != null)
                    comboTypeUsed.SelectedItem = (from ComboBoxItem c in comboTypeUsed.Items where (MSModel.ScheduleType)c.Tag == SchedEvent.Type select c).FirstOrDefault();

                comboTypeUsed.SelectionChanged += comboType_SelectionChanged;

                alreadyLoaded = true;

                if (string.IsNullOrEmpty(currentStyle))
                    currentStyle = ApplicationPropertiesHelper.GetProperty<String>("CurrentSkin");

                if (bEditing && !string.IsNullOrEmpty(currentStyle))
                {
                    Brush brush = WPFUtilities.ThemeHelper.GetHilightingThemeBrush(currentStyle);
                    SelectionColor = this.GetUnsetPropertyValue<Brush>(SelectionColorProperty, SelectionColor, brush, false);
                    brush = WPFUtilities.ThemeHelper.GetHilightingThemeBrush(currentStyle, true);
                    MultiSelectionColor = this.GetUnsetPropertyValue<Brush>(MultiSelectionColorProperty, MultiSelectionColor, brush, false);
                }


                //WPFUtilities.ThemeHelper.SetTheme(this, currentStyle);
                schedulerControl1.Style = currentStyle;

                if (SchedEvent != null)
                {
                    OPCUAEntityReferenceModel ScheduleItem = new OPCUAEntityReferenceModel() { Value = SchedEvent.ScheduleItem != null ? SchedEvent.ScheduleItem.FromXml<OPCUAEntityReference>() : null };
                    OPCUAEntityReferenceModel EnableVariable = new OPCUAEntityReferenceModel() { Value = SchedEvent.EnableVariable != null ? SchedEvent.EnableVariable.FromXml<OPCUAEntityReference>() : null };

                    textScheduleItemName.DataContext = ScheduleItem;
                    textEditEnableItem.DataContext = EnableVariable;

                    ScheduleItem.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            if (ScheduleItem.Value != null)
                                SchedEvent.ScheduleItem = ScheduleItem.Value.ToXml();
                            else
                                SchedEvent.ScheduleItem = null;
                        }
                    };
                    EnableVariable.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            if (EnableVariable.Value != null)
                                SchedEvent.EnableVariable = EnableVariable.Value.ToXml();
                            else
                                SchedEvent.EnableVariable = null;
                        }
                    };
                }

                if (Document != null)
                {
                    var rList = Document.GetRolesNames(bRuntime);
                    List<string> tmp = new List<string>();
                    tmp.Add(string.Empty);
                    if (rList != null && rList.Count > 0)
                        tmp.AddRange(rList);
                    comboAccessRoleUsed.ItemsSource = tmp;
                }

                if (SchedEvent != null)
                {
                    UpdateCalendarList();
                    schedulerControl1.DataContext = SchedEvent;
                }

                if (bRuntime)
                    LoadRuntimeLayout(GetStorageName());

                OverrideBaseProperties();
                ShowAvailableTabs();
                if (runningOnServer)
                {
                    var schedulerHostingGrid = this.FindFirstAncestor<ContentControl>()?.Parent as Grid;
                    if (schedulerHostingGrid != null)
                    {
                        webPopupHost = schedulerHostingGrid.FindName("webPopupHost") as ContentControl;
                        if (WebDialogUC != null)
                            WebDialogUC.WebDialogYesClicked += WebDialogYesClick;
                    }
                    var mbStart = new Binding("Time")
                    {
                        ValidatesOnDataErrors = true,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Mode = BindingMode.TwoWay,
                        Converter = new TimeSpanToStringConverter(),
                        NotifyOnSourceUpdated = true
                    };
                    textEditTime_web.SetBinding(TextBox.TextProperty, mbStart);
                    var mbEnd = new Binding("TimeOff")
                    {
                        ValidatesOnDataErrors = true,
                        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                        Mode = BindingMode.TwoWay,
                        Converter = new TimeSpanToStringConverter(),
                        NotifyOnSourceUpdated = true
                    };
                    textEditTimeOff_web.SetBinding(TextBox.TextProperty, mbEnd);

                    textEditTime.Visibility = textEditTimeOff.Visibility = Visibility.Collapsed;
                    textEditTime_web.Visibility = textEditTimeOff_web.Visibility = Visibility.Visible;
                    cpStartTime_web.Visibility = cpEndTime_web.Visibility = Visibility.Visible;
                    cpStartTime.Visibility = cpEndTime.Visibility = Visibility.Collapsed;
                }
                var exceptionsList = new List<MSModel.ExceptionsCalendarItem>();
                if (SchedEvent != null)
                    exceptionsList = (from c in SchedEvent.ExceptionsCalendar orderby c.StartDate select c).ToList();
                CalendarItemsGrid2.ItemsSource = exceptionsList;
                if (openExceptions && (!bRuntime || (SchedEvent != null && !SchedEvent.HideRuntimeExceptionTab)))
                    ExceptionCalendarSettings.IsSelected = true;
            };

            DataContextChanged += (o, e) =>
            {
                SchedEvent = DataContext as MSModel.MSScheduledAction;
                if (bRuntime)
                {
                    LoadRuntimeLayout(GetStorageName());
                    if (SchedEvent != null)
                    {
                        if (SchedEvent.HideRuntimeSettingsTab)
                            HideTab(GeneralSettings);
                        else
                            ShowTab(GeneralSettings);

                        if (SchedEvent.HideRuntimeExceptionTab)
                            HideTab(ExceptionCalendarSettings);
                        else
                            ShowTab(ExceptionCalendarSettings);
                    }
                }
            };
        }

        private void CommandsBtn_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            if (CommandManagerComponent.commandExplorer == null)
                return;

            var commands = button.Tag as String;
            CommandManagerList commandList;
            try
            {
                if (!String.IsNullOrEmpty(commands))
                    commandList = commands.FromXml<CommandManagerList>();
                else
                    commandList = new CommandManagerList();
            }
            catch
            {
                commandList = new CommandManagerList();
            }

            var explorerControl = CommandManagerComponent.commandExplorer.control;
            CommandManagerComponent.commandExplorer.SetSync(explorerControl, true);
            explorerControl.ClearValue(FrameworkElement.WidthProperty);
            explorerControl.ClearValue(FrameworkElement.HeightProperty);
            explorerControl.DataContext = new CommandManager.Hepers.CommandsEditObject(commandList);

            var Dialog = new GeneralDialogContent(explorerControl)
            {
                Owner = this.FindParent<Window>(),
                DialogKeepContent = true,
                HelpLink = "CommandsEditor"
            };
            if (Dialog.ShowDialog() == true)
            {
                CommandManagerComponent.commandExplorer.PropagateChanges(explorerControl);

                if (commandList.Count > 0)
                    commands = commandList.ToXml();
                else
                    commands = null;

                button.Tag = commands;
            }
        }

        private void WebDialogYesClick(object sender, EventArgs e)
        {
            switch (WebDialogUC.ActionType)
            {
                case DialogActionType.AddHolidays:
                    AddMissingYearHolidays();
                    break;
                case DialogActionType.DeleteItems:
                    var items = ((ConfirmationEventArgs)e).ActionItems;
                    DeleteItems(items as List<DevExpress.Xpo.XPObject>);
                    break;
            }
        }

        #region IsolatedStorage
        internal String GetStorageName()
        {
            string name = Name;
            if (SchedEvent != null)
                name = SchedEvent.Name;
            if (Document != null)
            {
                var parent = Document.Parent;
                var parentName = String.Empty;
                if (parent != null)
                    parentName = parent.Title;
                return $"{parentName}_{Document.Title}_{name}";
            }
            return name;
        }

        static String GetStoreFileName(String title)
        {
            return String.Format("{0}.{1}Weekly.dat", title, System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
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

        void SaveRuntimeLayout(String title)
        {
            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage || string.IsNullOrEmpty(title))
                    return;

                using (var stream = new IsolatedStorageFileStream(GetStoreFileName(title), FileMode.Create, isoStorage))
                {
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Encoding = System.Text.Encoding.UTF8,
                        Indent = true,
                        CloseOutput = true
                    };

                    using (XmlWriter writer = XmlDictionaryWriter.Create(stream, settings))
                    {
                        bool bRet = false;
                        try
                        {
                            DataContractSerializer serializer = new DataContractSerializer(typeof(TimeSpan));
                            //serializer.WriteObject(writer, workView.TimeScale);
                            bRet = true;
                        }
                        finally
                        {
                            writer.Close();
                        }
                        return;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        void LoadRuntimeLayout(String title)
        {
            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage || string.IsNullOrEmpty(title))
                    return;

                using (var stream = new IsolatedStorageFileStream(GetStoreFileName(title), FileMode.OpenOrCreate, isoStorage))
                {
                    System.Xml.XmlReaderSettings settings = new XmlReaderSettings
                    {
                        CloseInput = true
                    };

                    using (XmlReader reader = XmlDictionaryReader.Create(stream, settings))
                    {
                        bool bRet = false;
                        try
                        {
                            DataContractSerializer formatter = new DataContractSerializer(typeof(TimeSpan));
                            //workView.TimeScale = (TimeSpan)formatter.ReadObject(reader);
                            bRet = true;
                        }
                        finally
                        {
                            reader.Close();
                        }
                        return;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        IDictionary<String, String> _stringlist;
        internal static string stringPlaceolder;
        public void TranlslateText(IDictionary<String, String> stringlist, string stringPlaceolder)
        {
            _stringlist = stringlist;
            NewEventControl.stringPlaceolder = stringPlaceolder;
            schedulerControl1.TranslateText(stringlist, stringPlaceolder);
            GeneralSettings.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_NewEventTitle", stringlist, Properties.Resources.NewEventTitle);
            lblName.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_NewEventName", stringlist, Properties.Resources.NewEventName);
            lblEnable.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EnableEvent", stringlist, Properties.Resources.EnableEvent);
            lblType.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_NewEventType", stringlist, Properties.Resources.NewEventType);
            lblTime.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Time", stringlist, Properties.Resources.Time);
            lblDate.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Date", stringlist, Properties.Resources.Date);
            lblTimeOff.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_TimeOff", stringlist, Properties.Resources.TimeOff);
            //lblDateOff.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DateOff", stringlist, Properties.Resources.DateOff);
            lblTag.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_NewEventTag", stringlist, Properties.Resources.NewEventTag);
            lblOn.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_NewEventValueOn", stringlist, Properties.Resources.NewEventValueOn);
            lblOff.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_NewEventValueOff", stringlist, Properties.Resources.NewEventValueOff);
            lblEnableItem.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_NewEventEnableTag", stringlist, Properties.Resources.NewEventEnableTag);
            lblStartup.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ExecOnAtStartup", stringlist, Properties.Resources.ExecOnAtStartup);
            lblOffStartup.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ExecOffAtStartup", stringlist, Properties.Resources.ExecOffAtStartup);
            lblRuntime.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_SelectableRuntime", stringlist, Properties.Resources.SelectableRuntime);
            lblRole.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_NewEventAccessRole", stringlist, Properties.Resources.NewEventAccessRole);
            lblLevel.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_NewEventAccessLevel", stringlist, Properties.Resources.NewEventAccessLevel);
            lblMask.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_NewEventAccessMask", stringlist, Properties.Resources.NewEventAccessMask);

            CalendarSettings.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_NewEventCalendar", stringlist, Properties.Resources.NewEventCalendar);
            ExceptionCalendarSettings.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_NewEventExceptions", stringlist, Properties.Resources.NewEventExceptions);
            btnAddCalText.Text = btnAddCal2Text.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_AddCalendarItem", stringlist, Properties.Resources.AddCalendarItem);
            btnEditCalText.Text = btnEditCal2Text.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EditCalendarItem", stringlist, Properties.Resources.EditCalendarItem);
            btnDelCalText.Text = btnDelCal2Text.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DeleteCalendarItem", stringlist, Properties.Resources.DeleteCalendarItem);
            btnDelCalMulti2Text.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DeleteCalendarItems", stringlist, Properties.Resources.DeleteCalendarItems);
            btnAddHolidaysText.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_AddDefaultHolidays", stringlist, Properties.Resources.AddDefaultHolidays);
            lblCommandsOff.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_CommandsOff", stringlist, Properties.Resources.CommandsOff);
            lblCommandsOn.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_CommandsOn", stringlist, Properties.Resources.CommandsOn);

            textEditName.Text = SchedEvent?.Name;
            TranslationHelper.TranlslateViewColumns((CalendarItemsGrid.View as GridView).Columns, stringlist, stringPlaceolder);
            TranslationHelper.TranlslateViewColumns((CalendarItemsGrid2.View as GridView).Columns, stringlist, stringPlaceolder);

            TranslateCombo(stringlist, stringPlaceolder);

            DevWeeklyPlan.Header = TranslationHelper.TranlslateText($"_{stringPlaceolder}_NewEventWeeklyPlan", stringlist, Properties.Resources.NewEventWeeklyPlan);
            //AddScheduleTime.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_AddScheduleTime", stringlist, Properties.Resources.AddScheduleTime);
            //NewApp.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_AddScheduleTime", stringlist, Properties.Resources.AddScheduleTime);
            //AddSchedule.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_AddScheduleTime", stringlist, Properties.Resources.AddScheduleTime);

            CalendarItemsGrid.ItemsSource = null;
            CalendarItemsGrid2.ItemsSource = null;
            if (SchedEvent != null)
            {
                UpdateCalendarList(UpdateOption.All);
            }
        }

        private void TranslateCombo(IDictionary<String, String> stringlist, string stringPlaceolder)
        {
            foreach (ComboBoxItem item in comboTypeUsed.Items)
            {
                switch (item.Tag.ToString())
                {
                    case "everyMinute":
                        item.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EveryMinute", stringlist, Properties.Resources.EveryMinute);
                        break;
                    case "everyHour":
                        item.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EveryHour", stringlist, Properties.Resources.EveryHour);
                        break;
                    case "everyDay":
                        item.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EveryDay", stringlist, Properties.Resources.EveryDay);
                        break;
                    case "everySunday":
                        item.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EverySunday", stringlist, Properties.Resources.EverySunday);
                        break;
                    case "everyMonday":
                        item.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EveryMonday", stringlist, Properties.Resources.EveryMonday);
                        break;
                    case "everyTuesday":
                        item.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EveryTuesday", stringlist, Properties.Resources.EveryTuesday);
                        break;
                    case "everyWednesday":
                        item.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EveryWednesday", stringlist, Properties.Resources.EveryWednesday);
                        break;
                    case "everyThursday":
                        item.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EveryThursday", stringlist, Properties.Resources.EveryThursday);
                        break;
                    case "everyFriday":
                        item.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EveryFriday", stringlist, Properties.Resources.EveryFriday);
                        break;
                    case "everySaturday":
                        item.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_EverySaturday", stringlist, Properties.Resources.EverySaturday);
                        break;
                    case "weeklyPlan":
                        item.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_WeeklyPlan", stringlist, Properties.Resources.WeeklyPlan);
                        break;
                    case "calendar":
                        item.Content = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Calendar", stringlist, Properties.Resources.Calendar);
                        break;
                }
            }
        }

        public void SetDimensions(double h, double w)
        {
            Height = h;
            Width = w;
            tabGeneral.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

            schedulerControl1.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            schedulerControl1.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            DevWeeklyPlan.VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch;
            DevWeeklyPlan.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
        }

        public void DisableCoreSettings()
        {
            //TagGrid.IsEnabled = EnableGrid.IsEnabled = false;

            /*lblName*/
            lblEnable.Visibility = chkEnable.Visibility = System.Windows.Visibility.Collapsed;
            /*lblType*/
            comboTypeUsed.IsEnabled = false;
            /*lblTime textEditTime.IsEnabled = false;*/
            /*lblDate textEditDate.IsEnabled = false;*/
            /*lblTag textEditItem*/
            textScheduleItemName.IsEnabled = false; //btnItem.Visibility = btnDelItem .Visibility = System.Windows.Visibility.Collapsed;
            /*lblOn*/
            textEditValueOn.IsReadOnly = true; textEditValueOn.IsEnabled = false;
            /*lblOff*/
            textEditValueOff.IsReadOnly = true; textEditValueOff.IsEnabled = false;
            /*lblEnableItem textEditEnableItem*/
            textEditEnableItem.IsEnabled = false; // btnEnableItem.Visibility = btnDelEnableItem.Visibility = System.Windows.Visibility.Collapsed;
            lblRuntime.Visibility = chkSelectable.Visibility = System.Windows.Visibility.Collapsed;
            /*lblRole*/ /*textEditAccessRole*///comboAccessRoleUsed.IsReadOnly = true;
            comboAccessRoleUsed.IsEnabled = false;
            /*lblLevel*/
            textEditAccessLevel.IsReadOnly = true; textEditAccessLevel.IsEnabled = false;
            /*lblMask*/
            bitmask.Visibility = System.Windows.Visibility.Collapsed; text.IsReadOnly = true; text.IsEnabled = false;
        }
        bool bIsUpdating;
        public void UpdateData()
        {
            if (SchedEvent != null)
            {
                bIsUpdating = true;
                textEditName.Text = SchedEvent.Name;
                comboTypeUsed.SelectedItem = (from ComboBoxItem c in comboTypeUsed.Items where (MSModel.ScheduleType)c.Tag == SchedEvent.Type select c).FirstOrDefault();
                bIsUpdating = false;
                CalendarItemsGrid.ItemsSource = null;
                CalendarItemsGrid.ItemsSource = SchedEvent.Calendar;
                CalendarItemsGrid2.ItemsSource = null;
                CalendarItemsGrid2.ItemsSource = SchedEvent.ExceptionsCalendar;

                OPCUAEntityReferenceModel ScheduleItem = new OPCUAEntityReferenceModel() { Value = SchedEvent.ScheduleItem != null ? SchedEvent.ScheduleItem.FromXml<OPCUAEntityReference>() : null };
                OPCUAEntityReferenceModel EnableVariable = new OPCUAEntityReferenceModel() { Value = SchedEvent.EnableVariable != null ? SchedEvent.EnableVariable.FromXml<OPCUAEntityReference>() : null };

                textScheduleItemName.DataContext = ScheduleItem;
                textEditEnableItem.DataContext = EnableVariable;

                schedulerControl1.DataContext = null;
                schedulerControl1.DataContext = SchedEvent;
            }
            else
            {
                CalendarItemsGrid.ItemsSource = null;
                CalendarItemsGrid2.ItemsSource = null;
                //schedulerControl1.Storage.AppointmentStorage.DataSource = null;
            }
            ShowAvailableTabs();
            ListViewBestFit(CalendarItemsGrid);
            ListViewBestFit(CalendarItemsGrid2);
        }

        private void OnTabSelectionChanged(object sender, TabControlSelectionChangedEventArgs e)
        {
            if (!runningOnServer || SchedEvent == null)
                return;

            if (e.NewSelectedItem == ExceptionCalendarSettings)
            {
                CalendarItemsGrid2.ItemsSource = null;
                CalendarItemsGrid2.ItemsSource = SchedEvent.ExceptionsCalendar;
            }
            else if (e.NewSelectedItem == CalendarSettings)
            {
                CalendarItemsGrid.ItemsSource = null;
                CalendarItemsGrid.ItemsSource = SchedEvent.Calendar;
            }
        }

        private void CalendarItemAdd_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            AddCalendarItem();
        }

        private void CalendarItemEdit_Click(object sender, RoutedEventArgs e)
        {
            EditCalendarItem();
        }

        private void CalendarItemDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteCalendarItem();
        }

        private void CalendarExceptionsItemAdd_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            AddCalendarItem(true);
        }

        private void CalendarExceptionsItemEdit_Click(object sender, RoutedEventArgs e)
        {
            EditExceptionsCalendarItem();
        }

        private void CalendarExceptionsItemDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteCalendarItem(true);
        }

        private void CalendarExceptionsItemsDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteCalendarItem(true, true);
        }

        private void AddCalendarItem(bool bFromExceptionsTab = false)
        {
            if (SchedEvent != null)
            {
                var uow = SchedEvent.Session.BeginNestedUnitOfWork();
                try
                {
                    DevExpress.Xpo.XPObject calItem;
                    if (!bFromExceptionsTab)
                        calItem = new MSModel.CalendarItem(uow)
                        {
                            MSScheduledAction = uow.GetNestedObject(SchedEvent),
                            StartDate = DateTime.Now,
                            EndDate = DateTime.Now
                        };
                    else
                        calItem = new MSModel.ExceptionsCalendarItem(uow)
                        {
                            MSScheduledAction = uow.GetNestedObject(SchedEvent),
                            StartDate = DateTime.Now,
                            EndDate = DateTime.Now
                        };
                    newcal = new NewCalendarItem(_stringlist, stringPlaceolder, runningOnServer, SchedEvent != null && SchedEvent.Type == MSModel.ScheduleType.weeklyPlan, bFromExceptionsTab) { DataContext = calItem, CurrentCulture = CurrentCulture };
                    if (!runningOnServer)
                    {
                        GeneralDialogContent Dialog = new GeneralDialogContent(newcal)
                        {
                            Owner = this.FindParent<Window>(),
                            HelpLink = "CalendarItemEditor"
                        };
                        if (Dialog.ShowDialog() == true)
                        {
                            uow.CommitChanges();
                            UpdateCalendarList(bFromExceptionsTab ? UpdateOption.Exceptions : UpdateOption.Calendar);
                        }
                    }
                    else if (webPopupHost != null)
                    {
                        newcal.VerticalAlignment = VerticalAlignment.Stretch;
                        newcal.HorizontalAlignment = HorizontalAlignment.Stretch;
                        newcal.DialogConfirmed += (o, e) =>
                        {
                            var sender = o as NewCalendarItem;
                            var senderUow = (sender.DataContext as DevExpress.Xpo.XPObject).Session as DevExpress.Xpo.NestedUnitOfWork;
                            senderUow.CommitChanges();
                            UpdateCalendarList(bFromExceptionsTab ? UpdateOption.Exceptions : UpdateOption.Calendar);
                            senderUow.Dispose();
                            WebPopupSetModal(false);
                            webPopupHost.Content = null;
                        };
                        newcal.DialogCanceled += (o, e) =>
                        {
                            var sender = o as NewCalendarItem;
                            var senderUow = (sender.DataContext as DevExpress.Xpo.XPObject).Session as DevExpress.Xpo.NestedUnitOfWork;
                            senderUow.Dispose();
                            WebPopupSetModal(false);
                            webPopupHost.Content = null;
                        };
                        WebPopupSetModal(true);
                        webPopupHost.Content = newcal;
                    }
                }
                finally
                {
                    if (!runningOnServer)
                        uow.Dispose();
                }
            }
        }

        void WebPopupSetModal(bool bSet)
        {
            if (bSet)
            {
                (webPopupHost.Parent as Grid).GetVisualChildrenOfType<FrameworkElement>().ForEach(el => el.IsHitTestVisible = false);
                webPopupHost.IsHitTestVisible = true;
                webPopupHost.GetVisualChildrenOfType<FrameworkElement>().ForEach(el => el.IsHitTestVisible = true);
            }
            else
            {
                (webPopupHost.Parent as Grid).GetVisualChildrenOfType<FrameworkElement>().ForEach(el => el.IsHitTestVisible = true);
            }
        }

        private void EditExceptionsCalendarItem()
        {
            if (CalendarItemsGrid2.SelectedItem != null)
            {
                var cal = CalendarItemsGrid2.SelectedItem as MSModel.ExceptionsCalendarItem;
                var uow = cal.Session.BeginNestedUnitOfWork();
                try
                {
                    var tempdate = uow.GetNestedObject(cal);
                    newcal = new NewCalendarItem(_stringlist, stringPlaceolder, runningOnServer, SchedEvent != null && SchedEvent.Type == MSModel.ScheduleType.weeklyPlan, true) { DataContext = tempdate, CurrentCulture = CurrentCulture };
                    if (!runningOnServer)
                    {
                        GeneralDialogContent Dialog = new GeneralDialogContent(newcal)
                        {
                            Owner = this.FindParent<Window>(),
                            HelpLink = "CalendarItemEditor"
                        };
                        if (Dialog.ShowDialog() == true)
                        {
                            uow.CommitChanges();
                            CalendarItemsGrid2.ItemsSource = null;
                            if (SchedEvent != null)
                                UpdateCalendarList(UpdateOption.Exceptions);
                        }
                    }
                    else if (webPopupHost != null)
                    {
                        newcal.VerticalAlignment = VerticalAlignment.Stretch;
                        newcal.HorizontalAlignment = HorizontalAlignment.Stretch;
                        newcal.DialogConfirmed += (o, e) =>
                        {
                            var item = (o as NewCalendarItem).DataContext;
                            var senderUow = (item as DevExpress.Xpo.XPObject).Session as DevExpress.Xpo.NestedUnitOfWork;
                            senderUow.CommitChanges();
                            CalendarItemsGrid2.ItemsSource = null;
                            if (SchedEvent != null)
                                UpdateCalendarList(UpdateOption.Exceptions);
                            senderUow.Dispose();
                            WebPopupSetModal(false);
                            webPopupHost.Content = null;
                        };
                        newcal.DialogCanceled += (o, e) =>
                        {
                            var item = (o as NewCalendarItem).DataContext;
                            var senderUow = (item as DevExpress.Xpo.XPObject).Session as DevExpress.Xpo.NestedUnitOfWork;
                            senderUow.Dispose();
                            WebPopupSetModal(false);
                            webPopupHost.Content = null;
                        };
                        WebPopupSetModal(true);
                        webPopupHost.Content = newcal;
                    }
                }
                finally
                {
                    if (!runningOnServer)
                        uow.Dispose();
                }
            }
        }

        private void EditCalendarItem()
        {
            if (CalendarItemsGrid.SelectedItem != null)
            {
                var cal = CalendarItemsGrid.SelectedItem as MSModel.CalendarItem;
                var uow = cal.Session.BeginNestedUnitOfWork();
                try
                {
                    var tempdate = uow.GetNestedObject(cal);
                    newcal = new NewCalendarItem(_stringlist, stringPlaceolder, runningOnServer, SchedEvent != null && SchedEvent.Type == MSModel.ScheduleType.weeklyPlan) { DataContext = tempdate, CurrentCulture = CurrentCulture };
                    if (!runningOnServer)
                    {
                        GeneralDialogContent Dialog = new GeneralDialogContent(newcal)
                        {
                            Owner = this.FindParent<Window>(),
                            HelpLink = "CalendarItemEditor"
                        };
                        if (Dialog.ShowDialog() == true)
                        {
                            uow.CommitChanges();
                            CalendarItemsGrid.ItemsSource = null;
                            if (SchedEvent != null)
                                UpdateCalendarList();
                        }
                    }
                    else if (webPopupHost != null)
                    {
                        newcal.VerticalAlignment = VerticalAlignment.Stretch;
                        newcal.HorizontalAlignment = HorizontalAlignment.Stretch;
                        newcal.DialogConfirmed += (o, e) =>
                        {
                            var item = (o as NewCalendarItem).DataContext;
                            var senderUow = (item as DevExpress.Xpo.XPObject).Session as DevExpress.Xpo.NestedUnitOfWork;
                            senderUow.CommitChanges();
                            CalendarItemsGrid.ItemsSource = null;
                            if (SchedEvent != null)
                                UpdateCalendarList();
                            senderUow.Dispose();
                            WebPopupSetModal(false);
                            webPopupHost.Content = null;
                        };
                        newcal.DialogCanceled += (o, e) =>
                        {
                            var item = (o as NewCalendarItem).DataContext;
                            var senderUow = (item as DevExpress.Xpo.XPObject).Session as DevExpress.Xpo.NestedUnitOfWork;
                            senderUow.Dispose();
                            WebPopupSetModal(false);
                            webPopupHost.Content = null;
                        };
                        WebPopupSetModal(true);
                        webPopupHost.Content = newcal;
                    }
                }
                finally
                {
                    if (!runningOnServer)
                        uow.Dispose();
                }
            }
        }

        private void DeleteCalendarItem(bool bFromExceptionsTab = false, bool bDeleteAll = false)
        {
            var grid = bFromExceptionsTab ? CalendarItemsGrid2 : CalendarItemsGrid;
            List<DevExpress.Xpo.XPObject> itemsToDelete = null;
            string message = String.Empty;
            if (bDeleteAll)
            {
                itemsToDelete = grid.ItemsSource.Cast<DevExpress.Xpo.XPObject>().ToList();
                message = Properties.Resources.AskDeleteAllCalendarItems;
                if (itemsToDelete.Count == 0)
                    return;
            }
            else if (grid.SelectedItem != null)
            {
                MSModel.CalendarItemType? type = null;
                DateTime? start = null; DateTime? end = null;
                if (bFromExceptionsTab)
                {
                    type = (grid.SelectedItem as MSModel.ExceptionsCalendarItem).Type;
                    start = (grid.SelectedItem as MSModel.ExceptionsCalendarItem).StartDate;
                    end = (grid.SelectedItem as MSModel.ExceptionsCalendarItem).EndDate;
                }
                else
                {
                    type = (grid.SelectedItem as MSModel.CalendarItem).Type;
                    start = (grid.SelectedItem as MSModel.CalendarItem).StartDate;
                    end = (grid.SelectedItem as MSModel.CalendarItem).EndDate;
                }
                var msg = string.Format(Properties.Resources.AskDeleteCalendarItem, type, start, end);
                if (SchedEvent.Type == MSModel.ScheduleType.weeklyPlan && start != null && end != null)
                    msg = string.Format(Properties.Resources.AskDeleteCalendarItem, type, ((DateTime)start).ToString("HH:mm:ss"), ((DateTime)end).ToString("HH:mm:ss"));
                itemsToDelete = grid.SelectedItems.Cast<DevExpress.Xpo.XPObject>().ToList();
                message = grid.SelectedItems.Count == 1 ? msg : Properties.Resources.AskDeleteCalendarItems;
            }
            if (message == String.Empty)
                return;
            if (!runningOnServer)
            {
                if (GetAnswer(message, Properties.Resources.CaptionDeleteCalendarItem))
                    DeleteItems(itemsToDelete);
            }
            else
                WebDialogUC?.Show(message, itemsToDelete, DialogActionType.DeleteItems);
        }

        bool GetAnswer(string message, string caption)
        {
            if (iUIMsgBoxAlertService != null)
                return iUIMsgBoxAlertService.ShowYesNo(message, CustomDialogIcons.Question) == CustomDialogResults.Yes;
            else
                return MessageBox.Show(message, caption, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }

        private void DeleteItems(List<DevExpress.Xpo.XPObject> itemsToDelete)
        {
            if (SchedEvent != null && itemsToDelete != null && itemsToDelete.Count > 0)
            {
                bool bFromExceptionsTab = itemsToDelete[0] as MSModel.ExceptionsCalendarItem != null;
                foreach (var item in itemsToDelete)
                {
                    if (bFromExceptionsTab)
                        SchedEvent.ExceptionsCalendar.Remove(item as MSModel.ExceptionsCalendarItem);
                    else
                        SchedEvent.Calendar.Remove(item as MSModel.CalendarItem);
                    item.Delete();
                }
                UpdateCalendarList(bFromExceptionsTab ? UpdateOption.Exceptions : UpdateOption.Calendar);
            }
        }

        enum UpdateOption
        {
            Calendar,
            Exceptions,
            All
        }

        private void UpdateCalendarList(UpdateOption option = UpdateOption.Calendar)
        {
            var grid = option == UpdateOption.Exceptions ? CalendarItemsGrid2 : CalendarItemsGrid;
            switch (option)
            {
                case UpdateOption.Calendar:
                    CalendarItemsGrid.ItemsSource = (from c in SchedEvent.Calendar orderby c.StartDate select c).ToList();
                    ListViewBestFit(CalendarItemsGrid);
                    break;
                case UpdateOption.Exceptions:
                    CalendarItemsGrid2.ItemsSource = (from c in SchedEvent.ExceptionsCalendar orderby c.StartDate select c).ToList();
                    ListViewBestFit(CalendarItemsGrid2);
                    break;
                case UpdateOption.All:
                    CalendarItemsGrid.ItemsSource = (from c in SchedEvent.Calendar orderby c.StartDate select c).ToList();
                    CalendarItemsGrid2.ItemsSource = (from c in SchedEvent.ExceptionsCalendar orderby c.StartDate select c).ToList();
                    ListViewBestFit(CalendarItemsGrid2);
                    ListViewBestFit(CalendarItemsGrid);
                    break;
                default:
                    break;
            }
        }

        private void ListViewBestFit(ListView gv)
        {
            foreach (var col in (gv.View as GridView).Columns)
            {
                if (double.IsNaN(col.Width))
                    col.Width = col.ActualWidth;
                col.Width = double.NaN;
            }
        }

        bool bDispose;
        public void Dispose()
        {
            bDispose = true;
            DetachOverrideBaseProperties();
            if (bRuntime && schedEvent != null)
                SaveRuntimeLayout(GetStorageName());
            //foreach (TabItemExt titem in tabGeneral.Items)
            //{
            //    if (titem.Content is IDisposable)
            //        (titem.Content as IDisposable).Dispose();
            //}
            tabGeneral.Items.Clear();
            if (WebDialogUC != null)
            {
                WebDialogUC.WebDialogYesClicked -= WebDialogYesClick;
                WebDialogUC.Dispose();
            }
            //tabGeneral.Dispose();
            schedulerControl1.Dispose();
        }

        private void AMButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            int value = 0;
            if (button.Tag is int)
            {
                try
                {
                    value = (int)button.Tag;
                }
                catch (Exception ex)
                {

                }
            }

            var mdl = new model() { Value = value };
            var editor = new BitMaskEditor() { DataContext = mdl, Document = Document };
            var ColorDialog = new GeneralDialogContent(editor)
            {
                Owner = button.FindParent<Window>(),
                Title = Properties.Resources.AccessMaskEditor,
                HelpLink = "AccessMaskEditor"
            };
            if (ColorDialog.ShowDialog() != true)
            {
                return;
            }
            button.Tag = mdl.Value;
        }

        private void AddMissingHolidays_Click(object sender, RoutedEventArgs e)
        {
            if (SchedEvent != null)
            {
                if (!runningOnServer && GetAnswer(Properties.Resources.AskAddHolidays, Properties.Resources.CaptionDeleteCalendarItem))
                    AddMissingYearHolidays();
                else if (runningOnServer)
                    WebDialogUC?.Show(Properties.Resources.AskAddHolidays, null, DialogActionType.AddHolidays);
            }
        }

        private void AddMissingYearHolidays()
        {
            var publicHolidays = DateSystem.GetPublicHoliday(System.Globalization.RegionInfo.CurrentRegion.TwoLetterISORegionName, DateTime.Now.Year);
            var existingExceptions = (CalendarItemsGrid2.ItemsSource as ICollection<MSModel.ExceptionsCalendarItem>)?.ToList();

            foreach (var h in publicHolidays)
            {
                MSModel.ExceptionsCalendarItem calItem = null;
                var endDate = new DateTime(h.Date.Year, h.Date.Month, h.Date.Day, 23, 59, 59);
                MSModel.ExceptionsCalendarItem stillPresent = null;
                if (!h.Fixed)
                {
                    if (existingExceptions != null)
                        stillPresent = (from MSModel.ExceptionsCalendarItem item in existingExceptions.AsParallel()
                                        where
                                        item.MSScheduledAction == SchedEvent &&
                                        item.Type == MSModel.CalendarItemType.DateRange &&
                                        item.StartDate == h.Date &&
                                        item.EndDate == endDate
                                        select item
                        ).FirstOrDefault();
                    if (stillPresent == null)
                    {
                        calItem = new MSModel.ExceptionsCalendarItem(SchedEvent.Session)
                        {
                            MSScheduledAction = SchedEvent,
                            Type = MSModel.CalendarItemType.DateRange,
                            StartDate = h.Date,
                            EndDate = endDate
                        };
                    }
                }
                else
                {
                    if (existingExceptions != null)
                        stillPresent = (from MSModel.ExceptionsCalendarItem item in existingExceptions.AsParallel()
                                        where
                                        item.MSScheduledAction == SchedEvent &&
                                        item.Type == MSModel.CalendarItemType.WeekNDate &&
                                        item.DayOfMonth == h.Date.Day + 2 &&
                                        item.Month == (MSModel.CalendarMonthType)h.Date.Month &&
                                        item.StartDate == h.Date &&
                                        item.EndDate == endDate
                                        select item
                        ).FirstOrDefault();
                    if (stillPresent == null)
                    {
                        calItem = new MSModel.ExceptionsCalendarItem(SchedEvent.Session)
                        {
                            MSScheduledAction = SchedEvent,
                            Type = MSModel.CalendarItemType.WeekNDate,
                            DayOfMonth = h.Date.Day + 2,
                            Month = (MSModel.CalendarMonthType)h.Date.Month,
                            StartDate = h.Date,
                            EndDate = endDate
                        };
                    }
                }
                if (calItem != null)
                {
                    var newcal = new NewCalendarItem(_stringlist, stringPlaceolder, runningOnServer, SchedEvent != null && SchedEvent.Type == MSModel.ScheduleType.weeklyPlan, true) { DataContext = calItem, CurrentCulture = CurrentCulture };
                }
            }
            UpdateCalendarList(UpdateOption.Exceptions);
        }

        private void ResetDisplays()
        {
            comboTypeUsed.ItemsSource = null;
            textScheduleItemName.DataContext = null;
            textEditEnableItem.DataContext = null;
        }

        private void HideTab(DXTabItem tab)
        {
            if (runningOnServer)
            {
                tab.Width = 0;
            }
            else
                tab.Visibility = Visibility.Collapsed;
        }

        private void ShowTab(DXTabItem tab)
        {
            if (runningOnServer)
            {
                tab.Width = double.NaN;
            }
            else
                tab.Visibility = Visibility.Visible;
        }

        private void ShowAvailableTabs()
        {
            if (SchedEvent == null)
            {
                ShowTab(GeneralSettings);
                ShowTab(ExceptionCalendarSettings);
                ShowTab(CalendarSettings);
                ShowTab(DevWeeklyPlan);

                lblTime.Visibility = System.Windows.Visibility.Visible;
                lblTimeOff.Visibility = System.Windows.Visibility.Visible;
                if (!runningOnServer)
                {
                    textEditTime.Visibility = System.Windows.Visibility.Visible;
                    textEditTimeOff.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    textEditTime_web.Visibility = Visibility.Visible;
                    textEditTimeOff_web.Visibility = Visibility.Visible;
                }

                lblStartup.Visibility = Visibility.Hidden;
                chkStartup.Visibility = Visibility.Hidden;
                cpStartup.Visibility = Visibility.Hidden;
                lblOffStartup.Visibility = Visibility.Hidden;
                chkOffStartup.Visibility = Visibility.Hidden;
                cpOffStartup.Visibility = Visibility.Hidden;

                DevWeeklyPlan.IsSelected = true;
                ResetDisplays();
                //WPFUtilities.ThemeHelper.SetTheme(this, currentStyle);
                return;
            }
            else
            {
                if (!bRuntime)
                {
                    if (bHideGeneralSettings)
                        HideTab(GeneralSettings);
                    else
                        ShowTab(GeneralSettings);
                    ShowTab(ExceptionCalendarSettings);
                }
                else
                {
                    if (SchedEvent.HideRuntimeSettingsTab)
                        HideTab(GeneralSettings);
                    else
                        ShowTab(GeneralSettings);

                    if (SchedEvent.HideRuntimeExceptionTab)
                        HideTab(ExceptionCalendarSettings);
                    else
                        ShowTab(ExceptionCalendarSettings);
                }
            }


            if (SchedEvent.Type == MSModel.ScheduleType.weeklyPlan && !runningOnServer)
            {
                HideTab(CalendarSettings);
                ShowTab(DevWeeklyPlan);

                if (CalendarSettings.IsSelected && (!bRuntime || (SchedEvent != null && !SchedEvent.HideRuntimeSettingsTab)))
                    GeneralSettings.IsSelected = true;
                lblTime.Visibility = System.Windows.Visibility.Hidden;
                textEditTime.Visibility = System.Windows.Visibility.Hidden;
                lblTimeOff.Visibility = System.Windows.Visibility.Hidden;
                textEditTimeOff.Visibility = System.Windows.Visibility.Hidden;
                textEditTimeOff_web.Visibility = Visibility.Hidden;
                textEditTime_web.Visibility = Visibility.Hidden;
                lblStartup.Visibility = Visibility.Visible;
                chkStartup.Visibility = Visibility.Visible;
                cpStartup.Visibility = Visibility.Visible;
                lblOffStartup.Visibility = Visibility.Visible;
                chkOffStartup.Visibility = Visibility.Visible;
                cpOffStartup.Visibility = Visibility.Visible;

                DevWeeklyPlan.IsSelected = true;
            }
            else if (SchedEvent.Type == MSModel.ScheduleType.calendar || (SchedEvent.Type == MSModel.ScheduleType.weeklyPlan && runningOnServer))
            {
                ShowTab(CalendarSettings);
                HideTab(DevWeeklyPlan);

                if (DevWeeklyPlan.IsSelected && (!bRuntime || (SchedEvent != null && !SchedEvent.HideRuntimeSettingsTab)))
                    GeneralSettings.IsSelected = true;
                lblTime.Visibility = System.Windows.Visibility.Hidden;
                textEditTime.Visibility = System.Windows.Visibility.Hidden;
                lblTimeOff.Visibility = System.Windows.Visibility.Hidden;
                textEditTimeOff.Visibility = System.Windows.Visibility.Hidden;
                textEditTimeOff_web.Visibility = Visibility.Hidden;
                textEditTime_web.Visibility = Visibility.Hidden;
                lblStartup.Visibility = Visibility.Visible;
                chkStartup.Visibility = Visibility.Visible;
                cpStartup.Visibility = Visibility.Visible;
                lblOffStartup.Visibility = Visibility.Visible;
                chkOffStartup.Visibility = Visibility.Visible;
                cpOffStartup.Visibility = Visibility.Visible;

                CalendarSettings.IsSelected = true;
            }
            else
            {
                HideTab(CalendarSettings);
                HideTab(DevWeeklyPlan);

                if ((DevWeeklyPlan.IsSelected || CalendarSettings.IsSelected) && (!bRuntime || (SchedEvent != null && !SchedEvent.HideRuntimeSettingsTab)))
                    GeneralSettings.IsSelected = true;
                else if (!SchedEvent.HideRuntimeExceptionTab)
                    ExceptionCalendarSettings.IsSelected = true;

                lblTime.Visibility = System.Windows.Visibility.Visible;
                textEditTime.Visibility = System.Windows.Visibility.Visible;
                lblTimeOff.Visibility = System.Windows.Visibility.Visible;
                textEditTimeOff.Visibility = System.Windows.Visibility.Visible;
                if (runningOnServer)
                {
                    textEditTime_web.Visibility = Visibility.Visible;
                    textEditTimeOff_web.Visibility = Visibility.Visible;
                }

                lblStartup.Visibility = Visibility.Hidden;
                chkStartup.Visibility = Visibility.Hidden;
                cpStartup.Visibility = Visibility.Hidden;
                lblOffStartup.Visibility = Visibility.Hidden;
                chkOffStartup.Visibility = Visibility.Hidden;
                cpOffStartup.Visibility = Visibility.Hidden;
                if (!bRuntime && !bHideGeneralSettings || bRuntime && (SchedEvent != null && !SchedEvent.HideRuntimeSettingsTab))
                    GeneralSettings.IsSelected = true;
            }
            //WPFUtilities.ThemeHelper.SetTheme(this, currentStyle);
            OnForegroundChanged();
        }
        private void comboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bIsUpdating)
                return;

            try
            {
                if (SchedEvent != null)
                    SchedEvent.Type = (MSModel.ScheduleType)(comboTypeUsed.SelectedItem as ComboBoxItem).Tag;
            }
            catch (Exception)
            { }

            ShowAvailableTabs();
        }
    }
}
