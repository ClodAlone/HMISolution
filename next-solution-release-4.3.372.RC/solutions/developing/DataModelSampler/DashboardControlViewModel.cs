using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DataModelSampler
{
    public class DashboardControlViewModel : INotifyPropertyChanged
    {
        public DashboardControlViewModel()
        {
            NumberOfDays = 90;
        }

        /* Start Relevant Section */
        /* The ViewModel for the Demo - LoadData will populate the DataPoints collection and
         * set the StartTime, EndTime, SelectedStartTime and SelectedEndTime. The properties
         * are updated without knowledge of the controls they are feeding data to.
         */
        public void LoadData(int days)
        {
            DataPoints = new DemoDataValues();
            Random r = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < days; i++)
            {
                DataPoints.Add(new DemoDataValue
                {
                    Date = DateTime.Now.AddDays(i),
                    Y = r.NextDouble() * 100
                });
            }
            StartTime = DataPoints.First().Date;
            EndTime = DataPoints.Last().Date;

            SelectedStartTime = DataPoints[days / 2 - 5].Date.StartOfDay();
            SelectedEndTime = DataPoints[days / 2 + 5].Date.EndOfDay();
        }
        /* End Relevant Section */

        #region AverageValue (INotifyPropertyChanged Property)
        private double _averageValue;

        /* Start Relevant Section */
        public double AverageValue
        {
            get
            {
                if (DataPoints == null)
                    return 0;

                double total = 0;
                int count = 0;
                // loop through the selected range of DataPoints and calculate the average value of Y.
                foreach (DemoDataValue t in DataPoints.Where(t => t.Date >= SelectedStartTime && t.Date < SelectedEndTime))
                {
                    total += t.Y;
                    count++;
                }
                // trigger the PropertyChanged event.
                AverageValue = (count > 0) ? total / count : _averageValue;
                return _averageValue;
            }
            set
            {
                if (_averageValue != value)
                {
                    _averageValue = value;
                    RaisePropertyChanged("AverageValue");
                }
            }
        }
        /* End Relevant Section */
        #endregion

        #region SelectedStartTime (INotifyPropertyChanged Property)
        private DateTime _selectedStartTime = DateTime.Now;

        public DateTime SelectedStartTime
        {
            get { return _selectedStartTime; }
            set
            {
                if (_selectedStartTime != value)
                {
                    _selectedStartTime = value;
                    RaisePropertyChanged("SelectedStartTime");
                    double a = AverageValue; // cause AverageValue update.
                }
            }
        }
        #endregion

        #region SelectedEndTime (INotifyPropertyChanged Property)
        private DateTime _selectedEndTime = DateTime.Now;

        public DateTime SelectedEndTime
        {
            get { return _selectedEndTime; }
            set
            {
                if (_selectedEndTime != value)
                {
                    _selectedEndTime = ((DateTime)value).AddSeconds(-1);
                    RaisePropertyChanged("SelectedEndTime");
                    double a = AverageValue; // cause AverageValue update.
                }
            }
        }
        #endregion

        #region StartTime (INotifyPropertyChanged Property)
        private DateTime _startTime = DateTime.Now;

        public DateTime StartTime
        {
            get { return _startTime; }
            set
            {
                if (_startTime != value)
                {
                    _startTime = value;
                    RaisePropertyChanged("StartTime");
                }
            }
        }
        #endregion

        #region EndTime (INotifyPropertyChanged Property)
        private DateTime _endTime = DateTime.Now;

        public DateTime EndTime
        {
            get { return _endTime; }
            set
            {
                if (_endTime != value)
                {
                    _endTime = value;
                    RaisePropertyChanged("EndTime");
                }
            }
        }
        #endregion

        #region DataPoints (INotifyPropertyChanged Property)
        private DemoDataValues _dataPoints;

        public DemoDataValues DataPoints
        {
            get { return _dataPoints; }
            set
            {
                if (_dataPoints != value)
                {
                    _dataPoints = value;
                    RaisePropertyChanged("DataPoints");
                }
            }
        }


        private int _NumberOfDays;
        public int NumberOfDays
        {
            get { return _NumberOfDays; }
            set
            {
                if (_NumberOfDays == value)
                    return;
                _NumberOfDays = value;
                RaisePropertyChanged("NumberOfDays");
                LoadData(value);
            }
        }
        
        #endregion

        #region INotifyPropertyChanged values

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

    }
}
