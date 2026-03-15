using Opc.Ua;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Utilities;

namespace DataAnalisysControl.Helpers
{
    class SeriesDataHelpers : IDisposable
    {
        internal DataAnalisys control;
        internal MonitoredItemViewModel monitoredItemViewModel;
        internal string Key { get; set; }

        internal void opcuaEntityReference_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (bDisposed)
                return;

            OPCUAEntityReference n = (OPCUAEntityReference)sender;
            if (e.PropertyName == "MonitoredItemViewModel")
            {
                if (monitoredItemViewModel != null)
                    monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;

                if (bDisposed)
                    return;

                monitoredItemViewModel = n.MonitoredItemViewModel;

                if (monitoredItemViewModel != null)
                {
                    monitoredItemViewModel.PropertyChanged += monitoredItemViewModel_PropertyChanged;

                    if (monitoredItemViewModel.NodeIdModel != null &&
                        monitoredItemViewModel.NodeIdModel.IsVariable)
                        monitoredItemViewModel_PropertyChanged(monitoredItemViewModel, new PropertyChangedEventArgs("DataValue"));
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

        bool bDisposed;
        public void Dispose()
        {
            bDisposed = true;
            if (monitoredItemViewModel != null)
                monitoredItemViewModel.PropertyChanged -= monitoredItemViewModel_PropertyChanged;
            control = null;
        }
    }
}
