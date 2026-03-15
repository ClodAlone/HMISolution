using System;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using Utilities;
using System.ComponentModel;
using Opc.Ua;
using System.Threading;
using ViewModelLib;
using System.Collections.Generic;
using System.Windows.Data;

namespace OPCUAViewModel.UserControls
{
    /// <summary>
    /// Interaction logic for MonitoredItemViewModel.xaml
    /// </summary>
    public partial class MonitoredItemViewModel : UserControl, IDisposable
    {
        #region Declarations

        List<MeasureValue> measurements = new List<MeasureValue>();
        OPCUAViewModel.MonitoredItemViewModel model;
        NodeIdViewModel nodeId;
        bool bIsArray;

        PropertyObserver<OPCUAViewModel.MonitoredItemViewModel> _observer;

        #endregion

        public MonitoredItemViewModel()
        {
            InitializeComponent();

            DataContextChanged += SubscriptionViewModel_DataContextChanged;
        }

        void SubscriptionViewModel_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            model = DataContext as OPCUAViewModel.MonitoredItemViewModel;
            if (model == null)
                return;

            grid.DataContext = DataContext;
            grid2.DataContext = DataContext;

            nodeId = new NodeIdViewModel(model.monitoredItem.ResolvedNodeId, model.GetSubscriptionViewModelParent().GetSessionViewModelParent());

            try
            {
                bIsArray = nodeId.IsOneDimension || nodeId.IsTwoDimensions || nodeId.IsByteStringDataType;
                // GaugeGroup.Visibility = bIsArray ? Visibility.Collapsed : Visibility.Visible;
            }
            catch (Exception ex)
            {
                Utils.Trace(ex, "MonitoredItemVieWModel DataContextChanged");
            }

            _observer = new PropertyObserver<OPCUAViewModel.MonitoredItemViewModel>(model);
            _observer.RegisterHandler(n => n.DataValue, DataValueChanged);
            // model.PropertyChanged += model_PropertyChanged;

        }

        // void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        void DataValueChanged(OPCUAViewModel.MonitoredItemViewModel model)
        {
            // if (e.PropertyName == "DataValue")
            {
                if (bIsArray)
                {
                    Array array = model.DataValue.GetValue(null) as Array;

                    measurements.Clear();

                    if (array.Rank == 1)
                    {
                        int nIndex = 1;
                        foreach (var v in array)
                        {
                            double value = Convert.ToDouble(v);
                            measurements.Add(new MeasureValue(nIndex++, value));
                        }
                    }
                    else
                    {
                        int n = array.GetLowerBound(0);
                        int x = array.GetLowerBound(1);
                        while(n < array.GetUpperBound(0) && x < array.GetUpperBound(1))
                            measurements.Add(new MeasureValue(Convert.ToDouble(array.GetValue(n++)), 
                                                                Convert.ToDouble(array.GetValue(x++))));
                    }
                }
                else
                {
                    object modelDataValueGetValue = model.DataValue.GetValue(null);
                    if (modelDataValueGetValue != null)
                    {
                        double value = 0.0;
                        try
                        {
                            value = Convert.ToDouble(modelDataValueGetValue);
                        }
                        catch (Exception ex)
                        {
                            return;
                        }
                        System.Diagnostics.Debug.Assert(!Double.IsInfinity(value) && !Double.IsNaN(value));
                        measurements.Add(new MeasureValue(model.DataValue.SourceTimestamp.ToLocalTime().ToOADate(), value));
                    }
                }
            }
        }

        #region Measure
        /// <summary>
        /// Data collection with Time and Stock of Measurement
        /// </summary>
        /// <returns></returns>

        public class MeasureValue
        {
            public MeasureValue(double d, double v)
            {
                Time = d;
                Value = v;
            }
            public double Time
            {
                get;
                private set;
            }
            public double Value
            {
                get;
                private set;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            // model.PropertyChanged -= model_PropertyChanged;
            DataContextChanged -= SubscriptionViewModel_DataContextChanged;
            nodeId.Dispose();

            if (_observer != null)
            {
                _observer.Dispose();
                _observer = null;
            }
        }

        #endregion
    }
}
