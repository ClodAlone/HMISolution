#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
#if WINDOWS_PHONE
using System.Windows.Media;
using System.Collections.ObjectModel;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    ///ChartPointInfo contains information about the displaying series data points.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class ChartPointInfo: INotifyPropertyChanged
    {
        #region fields

        private ChartSeriesBase series;
        private string labelX;
        private string labelY;
        private string valueX;
        private string valueY;
        private Brush interior;
        private ObservableCollection<string> seriesvalues;

        #endregion

        #region properties

        /// <summary>
        /// Get or Set SeriesValues Property
        /// </summary>
        public ObservableCollection<string> SeriesValues
        {
            get
            {
                if(seriesvalues==null)
                {
                    seriesvalues=new ObservableCollection<string>();
                }
                return seriesvalues;
            }
            set
            {
                value = seriesvalues;
            }
        }
        /// <summary>
        /// Gets or Sets the owning series.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public ChartSeriesBase Series
        {
            get
            {
                return series;
            }
            set
            {
                if (value != series)
                {
                    series = value;
                    OnPropertyChanged("Series");
                    Interior = series.GetInteriorColor(1);
                }
            }
        }

        /// <summary>
        /// Gets or Sets the interior color of this data point.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public Brush Interior
        {
            get
            {
                return interior;
            }
            set
            {
                interior = value;
                OnPropertyChanged("Interior");
            }
        }

        /// <summary>
        /// Gets or Sets the x label.
        /// </summary>
        internal string LabelX 
        {
            get
            {
                return labelX;
            }
            set
            {
                if (value != labelX)
                {
                    labelX = value;
                    OnPropertyChanged("LabelX");
                }
            }
        }

        /// <summary>
        /// Gets or Sets the y label.
        /// </summary>
        internal string LabelY
        {
            get
            {
                return labelY;
            }
            set
            {
                if (value != labelY)
                {
                    labelY = value;
                    OnPropertyChanged("LabelY");
                }
            }
        }

        /// <summary>
        /// Gets or Sets the x value
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public string ValueX
        {
            get
            {
                return valueX;
            }
            set
            {
                if (value != valueX)
                {
                    valueX = value;
                    OnPropertyChanged("ValueX");
                }
            }
        }

        /// <summary>
        /// Gets or Sets the y value.
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public string ValueY
        {
            get
            {
                return valueY;
            }
            set
            {
                if (value != valueY)
                {
                    valueY = value;
                    OnPropertyChanged("ValueY");
                }
            }
        }

        /// <summary>
        /// Gets or sets the x coordinate
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double X
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the y coordinate
        /// </summary>
        [ClassReference(IsReviewed = false)]
        public double Y
        {
            get;
            set;
        }

        #endregion

        #region ctor

        #endregion

        #region events

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when property changed
        /// </summary>
        /// <param name="propertyName"></param>
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
