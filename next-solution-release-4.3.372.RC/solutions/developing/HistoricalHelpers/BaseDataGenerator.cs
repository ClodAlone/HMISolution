using DataReader.Helpers;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using Opc.Ua;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UFUAHistorianModel;
using UFUAModel.Extensions;
using WPFUtilities.HistoricalHelpers;
using ErrorEventArgs = Utilities.ErrorEventArgs;

namespace WPFPenHelpers
{
    public abstract class BaseDataGenerator : IDisposable
    {
        #region Declarations
        readonly string penId;
        readonly protected DataGeneratorSettings settings;
        readonly int commandTimeout;
        protected bool isArray;
        protected bool isBool;
        protected string absolutenodeid;

        IDataLayer dl;
        protected UnitOfWork ufw { get; private set; }

        protected List<Task> pendingTask = new List<Task>();

        protected readonly static DateTime DeadBandValue = new DateTime(2000, 1, 1);
        #endregion

        #region Constructors
        protected BaseDataGenerator(string penId, DataGeneratorSettings settings, int commandTimeout = 0)
        {
            if (String.IsNullOrEmpty(penId))
                throw new ArgumentNullException("penId");

            if (settings == null)
                throw new ArgumentNullException("settings");

            this.commandTimeout = commandTimeout;
            this.penId = penId;
            this.settings = settings;

            Initialize();
        }
        #endregion

        #region Properties
        public int CommandTimeout
        {
            get
            {
                return commandTimeout;
            }
        }

        public String PenId
        {
            get
            {
                return penId;
            }
        }

        public DataGeneratorSettings Settings
        {
            get
            {
                return settings;
            }
        }

        MyDataValue lastValue;
        public MyDataValue LastValue
        {
            get
            {
                return lastValue;
            }
            protected set
            {
                lastValue = value;
            }
        }

        protected bool IsDisposed
        {
            get
            {
                return bDisposed;
            }
        }

        protected bool IsPendingTask
        {
            get
            {
                return pendingTask.Count > 0;
            }
        }
        #endregion

#if !NET_STANDARD
        #region Events
        public event EventHandler<ErrorEventArgs> Error;
        protected void OnError(string error)
        {
            Error?.Invoke(this, new ErrorEventArgs() { ErrorMessage = error });
        }

        public event EventHandler HistoryLoaded;
        protected void OnHistoryLoaded()
        {
            HistoryLoaded?.Invoke(this, EventArgs.Empty);
        }
        #endregion
#endif

        #region Public Methods
        public void UpdateReferences(NodeIdViewModel nodeIdViewModel)
        {
            ParseNodeIdViewModel(nodeIdViewModel);

            var resolved_nodeid = nodeIdViewModel.nodeId.ToString();
            if (resolved_nodeid.Equals(absolutenodeid))
                return;

            absolutenodeid = resolved_nodeid;
            if (string.IsNullOrEmpty(absolutenodeid))
                return;

            LoadBuffer();
        }

        public void UpdateConnection(string newconnection)
        {
            if (newconnection.Equals(Settings.ConnectionString))
                return;

            Settings.ConnectionString = newconnection;
            LoadBuffer();
        }
        #endregion

        #region Abstract Methods
        protected abstract void SetDataSources(System.Threading.CancellationToken ct);
        #endregion

        #region Protected Methods
        protected List<MyDataValue> GetInitialBufferValues(int count)
        {
            List<MyDataValue> list = new List<MyDataValue>();
            var startTime = Settings.ClientTimezoneOffset != TimeSpan.Zero ? DateTime.UtcNow + Settings.ClientTimezoneOffset : DateTime.Now;
            for (int ii = count - 1; ii >= 0; ii--)
            {
                var sourceTimestamp = startTime.AddMilliseconds(-Settings.DeadBandInterval.TotalMilliseconds * ii);
                list.Add(new MyDataValue()
                {
                    Value = 0,
                    SourceTimestamp = sourceTimestamp //DeadBandValue
                });
            }
            return list;
        }

        protected bool ParseNodeIdViewModel(NodeIdViewModel nodeIdViewModel)
        {
            isArray = false;
            isBool = false;

            if (nodeIdViewModel == null)
                return false;

            UFUAModel.DataType dataType;
            if (nodeIdViewModel.DataType == Opc.Ua.BuiltInType.Null)
                return false;

            dataType = nodeIdViewModel.DataType.ToDataType();
            if (dataType == UFUAModel.DataType.String)
                return false;

            isBool = dataType == UFUAModel.DataType.Boolean;
            isArray = nodeIdViewModel.IsOneDimension;

            return true;
        }

        protected void LoadBuffer()
        {
            InitHistoricalData();
        }

        protected void CreateConnectionStringDataLayer()
        {
            if (ufw != null)
                return;
#if !NET_STANDARD
            try
#endif
            {
                dl = UFUAHistorianModel.Helpers.HistorianHelper.CreateDataLayer<UFUAAuditDataItem>(Settings.ConnectionString, CommandTimeout);
                ufw = new UnitOfWork(dl);
            }
#if !NET_STANDARD
            catch (Exception ex)
            {
                OnError(ex.Message);
            }
#endif
        }

        protected bool GetConnectionAndDataprovider(out string defaultDataProvider, out string defaultConnectionString)
        {
            defaultDataProvider = null;
            defaultConnectionString = null;

            if (string.IsNullOrEmpty(Settings.ConnectionString) ||
                    string.IsNullOrEmpty(Settings.DlrName) ||
                    string.IsNullOrEmpty(Settings.ColName))
                return false;

#if !NET_STANDARD
            try
            {
#endif
                var helper = new ConnectionStringParser(Settings.ConnectionString);
                string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
                if (!string.IsNullOrEmpty(providerType))
                {
                    defaultDataProvider = XpoConversionHelper.GetDataProviderFromXpoConnection(Settings.ConnectionString);
                    defaultConnectionString = XpoConversionHelper.GetConnectionStringFromXpoConnection(Settings.ConnectionString);
                }
                else
                {
                    defaultDataProvider = helper.GetPartByName("DataProvider");
                    helper.RemovePartByName("DataProvider");
                    defaultConnectionString = helper.GetConnectionString();
                }
#if !NET_STANDARD
            }
            catch (Exception ex)
            {
                OnError(ex.Message);
                return false;
            }
#endif
            return true;
        }
        #endregion

        #region Private Methods
        void InitHistoricalData()
        {
            if (IsPendingTask || !Settings.IsValid)
                return;

            SetDataSources(System.Threading.CancellationToken.None);
        }

        void UnloadDataLayer()
        {
            if (ufw != null)
            {
                ufw.Disconnect();
                ufw.Dispose();
                ufw = null;
            }
            if (dl != null)
            {
                dl.Dispose();
                dl = null;
            }
        }
        #endregion

        #region Virtual Methods
        protected virtual void Initialize()
        {
            lastValue = new MyDataValue()
            {
                Value = 0.0,
                SourceTimestamp = DateTime.MaxValue,
                SourcePicoseconds = 0
            };
        }

        bool bDisposed;
        protected virtual void OnDispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            if (pendingTask.Count > 0)
            {
                try
                {
                    Task.WaitAll(pendingTask.ToArray());
                }
                catch (AggregateException) { }
            }


            UnloadDataLayer();
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            if (IsDisposed)
                return;

            OnDispose();
        }
        #endregion
    }
}
