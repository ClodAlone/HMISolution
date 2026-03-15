using Opc.Ua;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Utilities;
using WPFPenHelpers;

namespace DataAnalisysRTControl.Helpers
{
    class SeriesDataHelpers : IDisposable
    {
        internal DataAnalisysRT control;
        internal MonitoredItemViewModel monitoredItemViewModel;
        CancellationTokenSource cts;
        internal string Key { get; set; }
        bool isRealTime;
        public int arrayIndex;

        public SeriesDataHelpers(int arrayindex, bool bRealTime)
        {
            arrayIndex = arrayindex;
            isRealTime = bRealTime;
        }

        internal void opcuaEntityReference_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (bDisposed)
                return;

            OPCUAEntityReference n = (OPCUAEntityReference)sender;
            OnPropertyChanged(n, e);
        }

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
                    monitoredItemViewModel.PropertyChanged += monitoredItemViewModel_PropertyChanged;

                    if (isRealTime || (monitoredItemViewModel.NodeIdModel != null && monitoredItemViewModel.NodeIdModel.IsVariable))
                        monitoredItemViewModel_PropertyChanged(monitoredItemViewModel, new PropertyChangedEventArgs("DataValue"));
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

        internal static bool CheckQuality(StatusCode status, bool onlygood)
        {
            if (!onlygood)
                return true;

            return StatusCode.IsGood(status);
        }

        internal void monitoredItemViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (bDisposed)
                return;

            MonitoredItemViewModel m = (MonitoredItemViewModel)sender;
            if (isRealTime)
            {
                OnPropertyChanged(m, e);
                return;
            }

            if (m == null)
                return;
            m.PropertyChanged -= monitoredItemViewModel_PropertyChanged;

            var fe = control as FrameworkElement;
            fe.Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
            {
                try
                {
                    if (control.OpcuaEntityReference.ContainsKey(Key) && control.OpcuaEntityReference[Key] != null)
                        control.UpdateReferences(Key, m.NodeIdModel.nodeId.ToString());
                }
                catch (Exception)
                {
                }
            });
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

        #region Public Events
        public event EventHandler<ModelChangedEventArgs> ModelChanged;
        void OnModelChanged(OPCUAEntityReference entityReference)
        {
            ModelChanged?.Invoke(this, new ModelChangedEventArgs(entityReference.HumanReadable, entityReference.NodeIdViewModel));
        }
        public event EventHandler<ErrorEventArgs> Error;
        protected void OnError(string error)
        {
            Error?.Invoke(this, new ErrorEventArgs() { ErrorMessage = error });
        }

        public event EventHandler<ValueChangedEventArgs> ValueChanged;
        public void RefreshValue()
        {
            OnValueChanged();
        }
        protected void OnValueChanged()
        {
            var dataValue = monitoredItemViewModel.DataValue;
            if (dataValue == null)
                return;

            if (dataValue.Value is Array)
            {
                if (arrayIndex != -1)
                {
                    DataValue newDataValue = null;
                    if (monitoredItemViewModel.DataValueCollectionDouble.Count > arrayIndex)
                    {
                        newDataValue = new DataValue(dataValue)
                        {
                            Value = new Variant(monitoredItemViewModel.DataValueCollectionDouble[arrayIndex].Value)
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
                else if (arrayIndex == -1)
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

        bool bDisposed;
        public void Dispose()
        {
            bDisposed = true;
            if (monitoredItemViewModel != null)
                monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;

            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
            }

            control = null;
        }
    }

}
