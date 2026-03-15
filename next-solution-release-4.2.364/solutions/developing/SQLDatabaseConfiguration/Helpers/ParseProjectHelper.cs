using DataReader;
using DataReader.Helpers;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UFInterfaces.CoreHostComponents;
using Utilities;

namespace SQLDatabaseConfiguration.Helpers
{
    public class ParseProjectHelper : IDisposable
    {
        #region Declarations
        readonly string filePath;
        readonly string password;
        readonly bool silent;

        readonly static UFProjectManager.ComponentService.UFProjectManagerComponent projectManagerComponent = new UFProjectManager.ComponentService.UFProjectManagerComponent();
        readonly static UFUAEditor.ComponentService.UFUAEditorManagerComponent serverEditorComponent = new UFUAEditor.ComponentService.UFUAEditorManagerComponent();
        readonly static UriResolver.ComponentService.UriResolverComponent uriRisolver = new UriResolver.ComponentService.UriResolverComponent();
        readonly static UIMsgBoxAlertService.ComponentService.UIMsgBoxAlertServiceComponent uiMsgBoxAlertService = new UIMsgBoxAlertService.ComponentService.UIMsgBoxAlertServiceComponent();

        readonly ComponentHost componentHost;

        UFProjectManager.UFProjectDocument projectDocument;
        UFUAEditor.Document.UFUAServerDocument serverDocument;
        #endregion

        #region Constructors
        public ParseProjectHelper(string filePath) : 
            this (filePath, null, silent: false)
        { }

        public ParseProjectHelper(string filePath, string password, bool silent)
        {
            this.filePath = filePath;
            this.password = password;
            this.componentHost = new ComponentHost();
            this.silent = silent;

            AddComponentHost();
            LoadProjectDocuments();
        }
        #endregion

        #region Private Methods
        void AddComponentHost()
        {
            componentHost.Components.Add(projectManagerComponent);
            componentHost.Components.Add(serverEditorComponent);
            if (!silent)
                componentHost.Components.Add(uiMsgBoxAlertService);
        }

        void LoadProjectDocuments()
        {
            projectDocument = UFProjectManager.UFProjectDocument.FromFile(filePath, projectManagerComponent);
            if (projectDocument == null)
                throw new NullReferenceException(Properties.Resources.ErrorOnOpeningProject);

            if (projectDocument.Protected && (password == null || !projectDocument.CheckPassword(password)))
                throw new AccessViolationException(Properties.Resources.InvalidProjectPassword);

            var uri = new System.Uri(projectDocument.rootBase, UriKind.RelativeOrAbsolute);
            serverDocument = UFUAEditor.Document.UFUAServerDocument.FromFile(uri.GetPathString(), serverEditorComponent, projectDocument, bCreateNew: false, bCheckEmpty: false);
            if (serverDocument == null)
                throw new NullReferenceException(Properties.Resources.ErrorOnOpeningServerDocument);
        }
        #endregion

        #region Public Properties
        IList<UFUAModel.UFUAHistorianSettings> historianSettings;
        public IList<UFUAModel.UFUAHistorianSettings> HistorianSettings
        {
            get
            {
                if (serverDocument != null && historianSettings == null)
                {
                    historianSettings = (from hs in new XPQuery<UFUAModel.UFUAHistorianSettings>(serverDocument.GetSession()).AsParallel()
                                         select hs).ToList();
                }
                return historianSettings;
            }
        }

        IList<DataLoggerModel.Helpers.DataLoggerSettingsHelper> dataLoggerSettings;
        public IList<DataLoggerModel.Helpers.DataLoggerSettingsHelper> DataLoggerSettings
        {
            get
            {
                if (serverDocument != null && dataLoggerSettings == null)
                {
                    dataLoggerSettings = new List<DataLoggerModel.Helpers.DataLoggerSettingsHelper>();

                    var dataloggers = (from p in new XPQuery<DataLoggerModel.DataLoggerSettings>(serverDocument.GetSession()).AsParallel()
                                       select p).ToList();

                    foreach (var datalogger in dataloggers)
                    {
                        var helper = new DataLoggerModel.Helpers.DataLoggerSettingsHelper(datalogger, serverDocument.rootBase);
                        foreach (var col in helper.DataLoggerSettings.Columns)
                        {
                            if (col.ColumnTag != null && !col.ColumnTag.IsEmpty())
                            {
                                var tag = (from t in new XPQuery<UFUAModel.UFUATag>(serverDocument.GetSession()).AsParallel()
                                           where t.NodeId == col.ColumnTag.Guid && t.PrototypeReference == null
                                           select t).FirstOrDefault();
                                col.UFUATagReference = tag;
                            }
                        }

                        lock (dataLoggerSettings)
                        {
                            dataLoggerSettings.Add(helper);
                        }
                    }
                }
                return dataLoggerSettings;
            }
        }

        DataReaderModel defaultConnection;
        public DataReaderModel DefaultConnection
        {
            get
            {
                if (serverDocument != null && defaultConnection == null)
                {
                    String connectionString = XpoHelpers.XpoHelper.NormalizeConnectionString(serverDocument.GetConfiguration().HistorianDefaultConnection, serverDocument.rootBase);
                    defaultConnection = new DataReader.DataReaderModel()
                    {
                        DataProvider = XpoConversionHelper.GetDataProviderFromXpoConnection(connectionString),
                        Connection = XpoConversionHelper.GetConnectionStringFromXpoConnection(connectionString)
                    };
                }
                return defaultConnection;
            }
        }
        #endregion

        #region Public Methods
        public static string SelectProjectPath()
        {
            var filter = uriRisolver.GetOpenFileFilter();
            var fileType = new CommonControls.SelectFileType(true, filter, null, uiMsgBoxAlertService);
            var newDialog = new GeneralDialogContent(fileType)
            {
                Owner = Application.Current.MainWindow,
                HelpLink = "OpenProject",
            };

            if (newDialog.ShowDialog() == true)
                return fileType.currentUri;
            else
                return null;
        }
        #endregion

        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            if (serverDocument != null)
            {
                serverDocument.Dispose();
                serverDocument = null;
            }

            if (projectDocument != null)
            {
                projectDocument.Dispose();
                projectDocument = null;
            }
        }
        #endregion
    }
}
