using System;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using Utilities;

namespace OPCUAViewModel.UserControls
{
    /// <summary>
    /// Interaction logic for SubscriptionViewModel.xaml
    /// </summary>
    public partial class SubscriptionViewModel : UserControl, IDisposable
    {

        #region Declarations

        ObservableCollection<MeasureValue> measurements = new ObservableCollection<MeasureValue>();
        OPCUAViewModel.SubscriptionViewModel model;

        #endregion
      
        public SubscriptionViewModel()
        {
            InitializeComponent();

            DataContextChanged += SubscriptionViewModel_DataContextChanged;
        }

        void SubscriptionViewModel_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            model = DataContext as OPCUAViewModel.SubscriptionViewModel;
            if (model == null)
                return;

            model.Children.CollectionChanged += Children_CollectionChanged;
            measurements.Add(new MeasureValue(DateTime.Now.ToOADate(), model.Children.Count));
        }

        void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //Removes points if the points count is greater than 30
            Dispatcher.BeginInvokeIfRequired(() =>
                {
                    if (measurements.Count > 100)
                        measurements.RemoveAt(0);
                    measurements.Add(new MeasureValue(DateTime.Now.ToOADate(), model.Children.Count));
                });
        }


        #region Measure
        /// <summary>
        /// Data collection with Time and Stock of Measurement
        /// </summary>
        /// <returns></returns>

        public class MeasureValue
        {
            public MeasureValue(double d, int v)
            {
                Time = d;
                Value = v;
            }
            public double Time
            {
                get;
                private set;
            }
            public int Value
            {
                get;
                private set;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            model.Children.CollectionChanged -= Children_CollectionChanged;
            DataContextChanged -= SubscriptionViewModel_DataContextChanged;
        }

        #endregion
    }
}
