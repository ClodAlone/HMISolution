using System;
using System.ComponentModel;
using OPCUAViewModel;
using Opc.Ua;
using UFUAModel.Extensions;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using WPFUtilities.HistoricalHelpers;

namespace WPFPenHelpers
{
    public enum NotifyValueType
    {
        Always,
        OnlyOnQualityGood,
        Never
    }

    public class PenItemHelper : IDisposable
    {
        #region Declarations
        readonly string key;
        readonly int arrayIndex;
        readonly NotifyValueType notifyValueType;

        MonitoredItemViewModel monitoredItemViewModel;
        CancellationTokenSource cts;
        #endregion

        #region Constructors
        public PenItemHelper(string key, int arrayIndex)
            : this(key, arrayIndex, NotifyValueType.Always)
        { }

        public PenItemHelper(string key, NotifyValueType notifyValueType) 
            : this(key, -1, notifyValueType)
        { }

        public PenItemHelper(string key, int arrayIndex = -1, NotifyValueType notifyValueType = NotifyValueType.Always)
        {
            this.key = key;
            this.arrayIndex = arrayIndex;
            this.notifyValueType = notifyValueType;
        }
        #endregion

        #region Public Events
        public event EventHandler<ModelChangedEventArgs> ModelChanged;
        void OnModelChanged(OPCUAEntityReference entityReference)
        {
            ModelChanged?.Invoke(this, new ModelChangedEventArgs(entityReference.HumanReadable, entityReference.NodeIdViewModel));
        }

        public event EventHandler<Utilities.ErrorEventArgs> Error;
        protected void OnError(string error)
        {
            Error?.Invoke(this, new Utilities.ErrorEventArgs() { ErrorMessage = error });
        }

        public event EventHandler<ValueChangedEventArgs> ValueChanged;
        public void RefreshValue()
        {
            OnValueChanged();
        }
        protected void OnValueChanged()
        {
            if (monitoredItemViewModel == null)
                return;

            var dataValue = monitoredItemViewModel.DataValue;
            if (dataValue == null || notifyValueType == NotifyValueType.Never || 
                (notifyValueType == NotifyValueType.OnlyOnQualityGood && !StatusCode.IsGood(dataValue.StatusCode)))
                return;

            if (dataValue.Value is Array)
            {
                if (ArrayIndex != -1)
                {
                    DataValue newDataValue = null;
                    if (monitoredItemViewModel.DataValueCollectionDouble.Count > ArrayIndex)
                    {
                        newDataValue = new DataValue(dataValue)
                        {
                            Value = new Variant(monitoredItemViewModel.DataValueCollectionDouble[ArrayIndex].Value)
                        };
                    }
                    else
                    {
                        newDataValue = new DataValue(dataValue)
                        {
                            Value = 0.0,
                            StatusCode = StatusCodes.BadIndexRangeNoData
                        };
                    }

                    ValueChanged?.Invoke(this, new ValueChangedEventArgs(newDataValue));
                }
                else if (ArrayIndex == -1)
                    ValueChanged?.Invoke(this, new ValueChangedEventArgs(monitoredItemViewModel.DataValueCollectionDouble));
            }
            else
                ValueChanged?.Invoke(this, new ValueChangedEventArgs(dataValue));
        }

        public event EventHandler<MinMaxRangeChangedEventArgs> MinMaxRangeChanged;
        protected void OnMinMaxRangeChanged()
        {
            TaskScheduler ts = null;
            try
            {
                ts = TaskScheduler.FromCurrentSynchronizationContext();
            }
            catch
            {

            }

            if (ts == null)
            {
                if (monitoredItemViewModel.HasRange && monitoredItemViewModel.Range != null)
                {
                    MinMaxRangeChanged?.Invoke(this, new MinMaxRangeChangedEventArgs(
                        monitoredItemViewModel.Range.Low, monitoredItemViewModel.Range.High));
                }
            }
            else
            {
                if (cts == null)
                    cts = new CancellationTokenSource();
                var token = cts.Token;
                var task1 = Task.Factory.StartNew(() =>
                {
                    if (token.IsCancellationRequested)
                        return;

                    var b = monitoredItemViewModel.HasRange;
                }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                var task2 = task1.ContinueWith(ret =>
                {
                    if (token.IsCancellationRequested)
                        return;

                    if (monitoredItemViewModel.HasRange && monitoredItemViewModel.Range != null)
                    {
                        MinMaxRangeChanged?.Invoke(this, new MinMaxRangeChangedEventArgs(
                            monitoredItemViewModel.Range.Low, monitoredItemViewModel.Range.High));
                    }
                }, ts);
            }
        }
        #endregion

        #region Public Methods
        public void opcuaEntityReference_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IsDisposed)
                return;

            OPCUAEntityReference n = (OPCUAEntityReference)sender;
            OnPropertyChanged(n, e);
        }

        public void monitoredItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IsDisposed)
                return;

            MonitoredItemViewModel m = (MonitoredItemViewModel)sender;
            OnPropertyChanged(m, e);
        }
        #endregion

        #region Properties
        public string Key
        {
            get
            {
                return key;
            }
        }

        public int ArrayIndex
        {
            get
            {
                return arrayIndex;
            }
        }

        public PenTypeEnum PenType
        {
            get
            {
                int penType;
                int.TryParse(Key.Split('_').Last(), out penType);

                return (PenTypeEnum)penType;
            }
        }

        protected bool IsDisposed
        {
            get
            {
                return bDisposed;
            }
        }
        #endregion

        #region Virtual Methods
        protected virtual void OnPropertyChanged(OPCUAEntityReference n, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MonitoredItemViewModel")
            {
                if (monitoredItemViewModel != null)
                    monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;

                monitoredItemViewModel = n.MonitoredItemViewModel;
                if (monitoredItemViewModel != null)
                {
                    OnMinMaxRangeChanged();

                    if (notifyValueType != NotifyValueType.Never)
                    {
                        monitoredItemViewModel.PropertyChanged += monitoredItemViewModel_PropertyChanged;
                        monitoredItemViewModel_PropertyChanged(monitoredItemViewModel, new PropertyChangedEventArgs("DataValue"));
                    }
                }
            }
            else if (e.PropertyName == "NodeIdViewModel")
            {
                if (n.NodeIdViewModel != null)
                {
                    OnModelChanged(n);
                }
            }
        }

        protected virtual void OnPropertyChanged(MonitoredItemViewModel m, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LastMessage")
            {
                OnError(m.LastMessage);
            }
            else if (e.PropertyName == "DataValue")
            {
                OnValueChanged();
            }
        }

        bool bDisposed;
        protected virtual void OnDispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            if (monitoredItemViewModel != null)
                monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;

            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
            }
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
