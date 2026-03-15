using DevExpress.Xpf.Docking;
using DocumentManager.ComponentService;
using ScreenSettings;
using StringManager.ComponentService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TranslationHelpers;
using UIMsgBoxAlertService.ComponentService;
using Utilities;
using WPFPenHelpers;

namespace StatesChartControl
{
    /// <summary>
    /// Interaction logic for DynamicDockManager.xaml
    /// </summary>
    public partial class DynamicDockManager : UserControl, IDisposable
    {
        #region Declarations
        SettingsStorage settings;
        MemoryStream saveAutoHiddenStream;
        IDocument Document;
        IStringEditorManager stringManager;
        IUIMsgBoxAlertService UIMsgBoxAlertService;
        IDictionary<String, String> stringlist;
        string ownerControlName;

        bool bPanelsHidden = false;
        bool bLoaded = false;
        bool bDesign = false;
        #endregion


        #region LegendAreaForeground
        public static readonly DependencyProperty LegendAreaForegroundProperty = DependencyProperty.Register("LegendAreaForeground", typeof(Brush), typeof(DynamicDockManager), new UIPropertyMetadata(new SolidColorBrush(Colors.White)));
        public Brush LegendAreaForeground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(LegendAreaForegroundProperty);
            }
            set
            {
                SetValue(LegendAreaForegroundProperty, value);
            }
        }

        #endregion

        #region Ctor
        public DynamicDockManager(IDocument document, SettingsStorage settings, string ownerControlName, UIElement dockedObject)
        {
            this.settings = settings;
            this.ownerControlName = ownerControlName;
            Document = document;
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (bLoaded || bDisposed)
                    return;
                bLoaded = true;

                chartPanel.Content = dockedObject;
                timeRangePanel.DataContext = settings;

                if (DesignerProperties.GetIsInDesignMode(this))
                    bDesign = true;
                else if (Document != null)
                    UIMsgBoxAlertService = Document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;

                ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, Document);
                if (Document != null)
                {
                    if (stringManager == null)
                        stringManager = Document.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                    if (stringManager != null)
                    {
                        StringManager_CultureChanged(null, null);
                        stringManager.CultureChanged += StringManager_CultureChanged;
                    }
                };
            };
        }
        #endregion
        #region Methods
        public object GetContent()
        {
            return chartPanel.Content;
        }
        public void SetLegendSource(TagPenList list)
        {
            legendListBox.ItemsSource = list;
        }
        internal void StringManager_CultureChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
            {
                if (bDisposed)
                    return;

                bool bUntranslated = bDesign && stringManager.GetActiveCulture(Document, false) == String.Empty;
                if (!bUntranslated)
                    stringlist = stringManager.GetListStringForCulture(Document, stringManager.GetActiveCulture(Document));
                else
                    stringlist = null;

                btnFetchData.ToolTip = btnFetchData.Content = TranslationHelper.TranlslateText($"_{ownerControlName}_FetchTitle", stringlist, Properties.Resources.FetchTitle);
                btnClearRecent.ToolTip = btnClearRecent.Content = TranslationHelper.TranlslateText($"_{ownerControlName}_ClearRecentTitle", stringlist, Properties.Resources.ClearRecent);
                recentTimeRanges.Text = TranslationHelper.TranlslateText($"_{ownerControlName}_RecentTimeRanges", stringlist, Properties.Resources.RecentTimeRanges);
                legendPanel.Caption = TranslationHelper.TranlslateText($"_{ownerControlName}_LegendTitle", stringlist, Properties.Resources.LegendTitle);
                timeRangePanel.Caption = TranslationHelper.TranlslateText($"_{ownerControlName}_TimeRange", stringlist, Properties.Resources.TimeRange);
                docked_startdate.Text = TranslationHelper.TranlslateText($"_{ownerControlName}_StartDate", stringlist, Properties.Resources.StartDate);
                docked_enddate.Text = TranslationHelper.TranlslateText($"_{ownerControlName}_EndDate", stringlist, Properties.Resources.EndDate);

                todayButton.ToolTip = TranslationHelper.TranlslateText($"_{ownerControlName}_TodayBtnTooltip", stringlist, Properties.Resources.TodayBtnTooltip);
                tomorrowButton.ToolTip = TranslationHelper.TranlslateText($"_{ownerControlName}_TomorrowBtnTooltip", stringlist, Properties.Resources.TomorrowBtnTooltip);

                penTitle.Header = TranslationHelper.TranlslateText($"_{ownerControlName}_Name", stringlist, Properties.Resources.Title);
                tagNameTitle.Header = TranslationHelper.TranlslateText($"_{ownerControlName}_TagNameColumn", stringlist, Properties.Resources.TagName);
                historianTitle.Header = TranslationHelper.TranlslateText($"_{ownerControlName}_HistoricalNameColumn", stringlist, Properties.Resources.HistoricalDlrName);
            });
        }

        public void DisableTimeGrid()
        {
            timeRangeGrid.IsEnabled = false;
        }

        void RefreshRecentRanges()
        {
            listBoxRecent.ItemsSource = null;
            listBoxRecent.ItemsSource = settings.ListRanges;
        }

        public bool ExpandCollapseDocking()
        {
            if (!bPanelsHidden)
                HideAllHidden();
            else
                RestoreAllHidden();
            return bPanelsHidden;
        }

        internal void HideAllHidden()
        {
            using (var cursor = new WaitCursor())
            {
                if (!bPanelsHidden)
                {
                    bPanelsHidden = true;

                    if (saveAutoHiddenStream != null)
                        saveAutoHiddenStream.Dispose();
                    saveAutoHiddenStream = new MemoryStream();
                    dockManager.SaveLayoutToStream(saveAutoHiddenStream);

                    var listToHide = new List<BaseLayoutItem>();
                    dockManager.FloatGroups.ToList().ForEach(group =>
                    {
                        if (!listToHide.Contains(group))
                            listToHide.Add(group);
                    });
                    dockManager.AutoHideGroups.ToList().ForEach(group =>
                    {
                        dockManager.DockController.Dock(group);
                    });
                    GetChildPanels(groupGeneral).ForEach(group =>
                    {
                        if (!listToHide.Contains(group))
                            listToHide.Add(group);
                    });

                    if (listToHide.Contains(chartPanel))
                        listToHide.Remove(chartPanel);
                    listToHide.ForEach(group => dockManager.DockController.Close(group));

                    dockManager.ClosedPanelsBarVisibility = DevExpress.Xpf.Docking.Base.ClosedPanelsBarVisibility.Never;
                }
            }
        }

        void RestoreAllHidden()
        {
            using (var cursor = new WaitCursor())
            {
                if (bPanelsHidden)
                {
                    bPanelsHidden = false;
                    if (saveAutoHiddenStream != null)
                    {
                        try
                        {
                            saveAutoHiddenStream.Seek(0, SeekOrigin.Begin);
                            dockManager.RestoreLayoutFromStream(saveAutoHiddenStream);
                            saveAutoHiddenStream.Dispose();
                            saveAutoHiddenStream = null;
                            dockManager.ClosedPanelsBarVisibility = DevExpress.Xpf.Docking.Base.ClosedPanelsBarVisibility.Auto;
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
        }

        List<LayoutPanel> GetChildPanels(LayoutGroup root)
        {
            List<LayoutPanel> panels = new List<LayoutPanel>();
            foreach (BaseLayoutItem item in root.Items)
            {
                if (item is LayoutPanel)
                {
                    panels.Add((LayoutPanel)item);
                }

                if (item is LayoutGroup)
                {
                    panels.AddRange(GetChildPanels((LayoutGroup)item));
                }
            }
            return panels;
        }
        #endregion
        #region EventHandlers
        void FromToday_Click(object sender, RoutedEventArgs e)
        {
            var now = DateTime.UtcNow;
            DateTime today = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            settings.DateTimeStart = today;
        }

        void TillTomorrow_Click(object sender, RoutedEventArgs e)
        {
            var now = DateTime.UtcNow;
            DateTime today = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0) + TimeSpan.FromDays(1);
            settings.DateTimeEnd = today;
        }

        void Button_ClearRecent(object sender, RoutedEventArgs e)
        {
            settings.ListRanges.Clear();
            listBoxRecent.ItemsSource = null;
            listBoxRecent.ItemsSource = settings.ListRanges;
        }

        public event EventHandler DataFetchRequest;
        void OnDataFetchRequest(object sender, RoutedEventArgs e)
        {
            DataFetchRequest?.Invoke(this, e);
            RefreshRecentRanges();
        }

        public event EventHandler LoadRangeRequest;
        void OnLoadRangeRequest(object sender, RoutedEventArgs e)
        {
            var newTimeRange = listBoxRecent.SelectedItem as WPFUtilities.TimeRange;
            if (newTimeRange == null)
                return;
            settings.DateTimeStart = newTimeRange.DateTimeStart;
            settings.DateTimeEnd = newTimeRange.DateTimeEnd;
            LoadRangeRequest?.Invoke(this, e);
        }
        private void OnDockItemHidden(object sender, DevExpress.Xpf.Docking.Base.ItemEventArgs e)
        {
            if (e.Item as LayoutPanel != null && e.Item.IsActive)
                dockManager.ActiveDockItem = chartPanel;
        }
        #endregion
        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            chartPanel.Content = null;
            DataContext = null;
            if (stringManager != null)
                stringManager.CultureChanged -= StringManager_CultureChanged;
            if (saveAutoHiddenStream != null)
                saveAutoHiddenStream.Dispose();
            if (dockManager.DockController != null && dockManager.DockController is IDisposable)
                (dockManager.DockController as IDisposable).Dispose();
            if (dockManager is IDisposable)
                (dockManager as IDisposable).Dispose();
        }
        #endregion
    }
}
