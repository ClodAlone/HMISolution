using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel;
using Opc.Ua;
#if NETFRAMEWORK
using DevExpress.Xpf.Charts;
using System.Windows.Media;
using System.Windows.Threading;
#endif

namespace WPFPenHelpers
{
    public class LastClickedPoint : INotifyPropertyChanged
    {
        private string date = "";
        private double value;
        public string Date {
            get { return date; }
            set
            {
                if (value == null)
                    value = "";
                if (value == date) return;
                date = value;
                OnPropertyChanged("Date");
            }
        }
        public double Value {
            get { return value; }
            set
            {
                if (value == this.value) return;
                this.value = value;
                OnPropertyChanged("Value");
            }
        }
        public LastClickedPoint(string date, double value)
        {
            Date = date;
            Value = value;
        }
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

    [DataContract(Name = "SerieSettings")]
    public class SerieSettings : INotifyPropertyChanged
    {
        #region Constructor
        public SerieSettings()
        {
            Initialize();
        }

        void Initialize()
        {
#if NETFRAMEWORK
            Color = Colors.Transparent;
#endif
            _IsStatisticEnabled = false;
            if (!_IsSet)
            {
                _IsVisible = true;
                _IsSet = true;
            }
        }
        #endregion

        #region Deserializing
        [OnDeserializing]
        void Deserializing(StreamingContext context)
        {
            Initialize();
        }
        #endregion

        #region Members persistance
        [DataMember]
        public String Name { get; set; }
        [DataMember]
        public String DName { get; set; }
        [DataMember]
        public String TagName { get; set; }
        [DataMember]
        public String HistoricalName { get; set; }
        [DataMember]
        public bool UseTableAggregation { get; set; }
        [DataMember]
        public bool _MinAggregation;
        [DataMember]
        public bool _MaxAggregation;
        [DataMember]
        public bool _AvgAggregation;
        [DataMember]
        public bool DLRSorce { get; set; }
        [DataMember]
        public bool UseSourceTimeStamp { get; set; }
        [DataMember]
        public bool LocalizeSourceTimeStamp { get; set; }
        [DataMember]
        public String SourcetimestampColumnName { get; set; }
        [DataMember]
        public string NodeID { get; set; }
        [DataMember]
        public string SGuid { get; set; }
        [DataMember]
        public String TagreferenceXml { get; set; }
        [DataMember]
        public String ConditionalTagXml { get; set; }
        [DataMember]
        public bool ShowAxis { get; set; }
        [DataMember]
        public bool AuthomaticScale { get; set; }
        [DataMember]
        string eUnit;
        //[DataMember]
        public String EUnit {
            get
            {
                return eUnit;
            }
            set
            {
                if (eUnit != value)
                {
                    eUnit = value;
                    OnPropertyChanged("EUnit");
                }
            }
        }
        [DataMember]
        public double AbsoluteMin { get; set; }
        [DataMember]
        public double AbsoluteMax { get; set; }
        [DataMember]
        public double Min { get; set; }
        [DataMember]
        public double Max { get; set; }
        [DataMember]
        public bool LogarithmicYScale { get; set; }
        [DataMember]
        public double LogarithmicBaseYScale { get; set; }
#if NETFRAMEWORK
        [DataMember]
        public Color Color { get; set; }
#endif
        [DataMember]
        public List<MyDataValue> listValues;
        [DataMember]
        public double minValue { get; set; }
        [DataMember]
        public double maxValue { get; set; }
        [DataMember]
        public double averageValue { get; set; }
        [DataMember]
        public double medianValue { get; set; }
        [DataMember]
        public double varianceValue { get; set; }
        [DataMember]
        public double standardDeviationValue { get; set; }
        [DataMember]
        public double populationVarianceValue { get; set; }
        [DataMember]
        public double populationStandardDeviationValue { get; set; }
        [DataMember]
        public double rangeValue { get; set; }
        [DataMember]
        public String serieTypeLine;
        [DataMember]
        public int thickness;
        [DataMember]
        private DataValue dvalue;


        [DataMember]
        public int numPoints { get; set; }
        [DataMember]
        public int numCompressPoint { get; set; }
        [DataMember]
        public int numCompressRation { get; set; }
        [DataMember]
        private bool _IsVisible;
        [DataMember]
        private bool _IsStatisticEnabled = true;
        [DataMember]
        private bool _IsSet;
        [DataMember]
        public bool UseEUMinMax { get; set; }
        [DataMember]
        public int arrayindex { get; set; }
        [DataMember]
        public bool AddVirtualPoints { get; set; }

        #endregion

        #region Data
#if NETFRAMEWORK
        public Series lineSerie;
        public Series lineSerieCompare;
        public Series lineSerieMin;
        public Series lineSerieMax;
        public Series lineSerieAvg;
        int selectedPenThickness = 5;
        public int SelectedPenThickness
        {
            get
            {
                return selectedPenThickness;
            }
            set
            {
                if (value == selectedPenThickness)
                    return;
                selectedPenThickness = value;
                if (IsHighlighted)
                    UpdatePenThickness();
            }
        }
        public bool IsHighlighted {
            get;
            private set;
        }
#endif
#endregion

#region Properties
#if NETFRAMEWORK
        bool showAxisDynamic;
        public bool ShowAxisDynamic
        {
            get
            {
                return showAxisDynamic;
            }
            set
            {
                if (value != showAxisDynamic)
                {
                    showAxisDynamic = value;
                    if (lineSerie != null && XYDiagram2D.GetSeriesAxisY((XYSeries)lineSerie) != null)
                        XYDiagram2D.GetSeriesAxisY((XYSeries)lineSerie).Visible = value;
                }
            }
        }
#endif
        public bool MinAggregation
        {
            get
            {
                return _MinAggregation;
            }
            set
            {
                _MinAggregation = value;
#if NETFRAMEWORK
                if (lineSerieMin != null)
                    lineSerieMin.Visible = value && _IsVisible;
#endif
            }
        }
        public bool MaxAggregation
        {
            get
            {
                return _MaxAggregation;
            }
            set
            {
                _MaxAggregation = value;
#if NETFRAMEWORK
                if (lineSerieMax != null)
                    lineSerieMax.Visible = value && _IsVisible;
#endif
            }
        }
        public bool AvgAggregation
        {
            get
            {
                return _AvgAggregation;
            }
            set
            {
                _AvgAggregation = value;
#if NETFRAMEWORK
                if (lineSerieAvg != null)
                    lineSerieAvg.Visible = value && _IsVisible;
#endif
            }
        }

#if NETFRAMEWORK
        public Brush RealColor
        {
            get
            {
                return new SolidColorBrush(Color);
            }
        }
#endif

        public bool IsStatisticEnabled
        {
            get
            {
                return _IsStatisticEnabled;
            }
            set
            {
                _IsStatisticEnabled = value;
            }
        }

        public bool IsSet
        {
            get
            {
                return _IsSet;
            }
            set
            {
                _IsSet = value;
            }
        }

#if NETFRAMEWORK
        public bool IsVisible
        {
            get {
                return _IsVisible; 
            }
            set
            {
                ShowAxisDynamic = value;

                if (value == _IsVisible)
                    return;
                _IsVisible = value;
                if (lineSerie != null)
                    lineSerie.Visible = value;
                if (lineSerieCompare != null)
                    lineSerieCompare.Visible = value;
                if (lineSerieMin != null)
                    lineSerieMin.Visible = value && MinAggregation;
                if (lineSerieMax != null)
                    lineSerieMax.Visible = value && MaxAggregation;
                if (lineSerieAvg != null)
                    lineSerieAvg.Visible = value && AvgAggregation;

                if (value)
                    SelectUnselectPen(true);
                OnPropertyChanged("IsVisible");
            }
        }
#endif
        public DataValue DValue
        {
            get { return dvalue; }
            set
            {
                if (dvalue == value)
                    return;
                dvalue = value;
                OnPropertyChanged("DValue");
                OnPropertyChanged("DValueValue");
            }
        }
        public double DValueValue
        {
            get {
                double val = 0d;
                if (DValue == null)
                    return val;
                if (DValue.Value is Boolean)
                    val = (bool) DValue.Value ? 1.0 : 0.0;
                else
                    try { val = Convert.ToDouble(DValue.Value, System.Globalization.CultureInfo.InvariantCulture); } catch { }
                return val;
            }
        }
        public LastClickedPoint LastClickedPoint { get; set; } = new LastClickedPoint("", 0);
#endregion
#region Methods
#if NETFRAMEWORK
        public void SelectUnselectPen(bool? bForceSelect = null)
        {
            bool bSelect = (bForceSelect == null && !IsHighlighted) || (bForceSelect != null && (bool)bForceSelect);
            IsHighlighted = bSelect;
            UpdatePenThickness();
        }
        void UpdatePenThickness()
        {
            int unselectedPenThickness = thickness > 0 ? thickness : 1;
            if (lineSerie is LineSeries2D)
                (lineSerie as LineSeries2D).LineStyle.Thickness = IsHighlighted ? selectedPenThickness : unselectedPenThickness;
            else if (lineSerie is AreaSeries2D)
                (lineSerie as AreaSeries2D).Border.LineStyle.Thickness = IsHighlighted ? selectedPenThickness : unselectedPenThickness;
        }
#endif
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
            // VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
#if NETFRAMEWORK
                DispatcherObject dispatcherObject = handler.Target as DispatcherObject;

                // If the subscriber is a DispatcherObject and different thread
                if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                {
                    // Invoke handler in the target dispatcher's thread
                    dispatcherObject.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, e);
                }
                else // Execute handler as is
#endif
                    handler(this, e);
            }
        }
#endregion
    }
}
