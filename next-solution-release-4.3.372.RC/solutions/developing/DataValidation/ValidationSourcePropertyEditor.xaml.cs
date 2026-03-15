using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using UFUAEditor.ComponentService;
using DocumentManager.ComponentService;
using Utilities;
using DataLoggerModel.Helpers;
using OPCUAViewModelService.ComponentService;
using System.ComponentModel;
using System.Xml.Serialization;

namespace DataValidation
{
    /// <summary>
    /// Interaction logic for ValidationSourcePropertyEditor.xaml
    /// </summary>
    public partial class ValidationSourcePropertyEditor : UserControl, INotifyPropertyChanged
    {
        #region Ctor
        public ValidationSourcePropertyEditor()
        {
            InitializeComponent();
        }
        #endregion
        bool sourcesListLoading = false;
        #region Public Props
        #region SourcesListLoading
        [Browsable(false)]
        [XmlIgnore]
        public bool SourcesListLoading
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return sourcesListLoading;
            }
            set
            {
                if (sourcesListLoading != value)
                {
                    sourcesListLoading = value;
                    OnPropertyChanged("SourcesListLoading");
                }
            }
        }
        #endregion
        #endregion
        #region Methods
        CancellationTokenSource cts;
        void OnDropDownOpened(object sender, System.EventArgs e)
        {
            if (sourcesCombo.ItemsSource == null)
            {
                IDocument document = OPCUAViewModelComponent.workspaceService.ContextDocument;
                if (document == null)
                    return;
                cts = new CancellationTokenSource();
                var token = cts.Token;
                SourcesListLoading = true;
                var task1 = Task.Factory.StartNew(() =>
                {
                    var dataValidationItems = new List<string>() { string.Empty };
                    var service = document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                    if (service != null)
                    {
                        token.ThrowIfCancellationRequested();

                        if (service.IsEventDataProtectionEnabled(document))
                        {
                            dataValidationItems.Add(Properties.Settings.Default.EventDataSourceName);

                            if (service.IsAtLeastOneAuditTraceEnabled(document))
                                dataValidationItems.Add(Properties.Settings.Default.AuditTraceSourceName);
                        }

                        var historicalNames = service.GetHistoricalSettingsNameList(document, inExecution: true);
                        if (historicalNames != null)
                        {
                            foreach (var historicalName in historicalNames)
                            {
                                token.ThrowIfCancellationRequested();

                                if (service.IsHistorianDataProtectionEnabled(document, historicalName))
                                {
                                    dataValidationItems.Add(historicalName);
                                }
                            }
                        }

                        var dataloggerNames = service.GetDataLoggerSettingsNameList(document, inExecution: true);
                        if (dataloggerNames != null)
                        {
                            foreach (var dataloggerName in dataloggerNames)
                            {
                                token.ThrowIfCancellationRequested();

                                var settings = service.GetDataLoggerDataTable(document, dataloggerName, inExecution: true);
                                if (settings == null)
                                    continue;
                                var dataLoggerTable = settings.FromXml<DataLoggerTable>();
                                if (dataLoggerTable.IsDataProtectionEnabled)
                                    dataValidationItems.Add(dataLoggerTable.Name);
                            }
                        }
                    }

                    return dataValidationItems;
                }, token);
                task1.ContinueWith(ret =>
                {
                    cts.Dispose();
                    cts = null;

                    if (token.IsCancellationRequested)
                        return;

                    sourcesCombo.ItemsSource = ret.Result;
                    SourcesListLoading = false;
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }
        #endregion
        #region INotifyPropertyChanged Members
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                //DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                var e = new PropertyChangedEventArgs(propertyName);
                //// If the subscriber is a DispatcherObject and different thread
                //if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                //{
                //    // Invoke handler in the target dispatcher's thread
                //    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                //}
                //else // Execute handler as is
                handler(this, e);
            }
        }
        #endregion
    }
}
