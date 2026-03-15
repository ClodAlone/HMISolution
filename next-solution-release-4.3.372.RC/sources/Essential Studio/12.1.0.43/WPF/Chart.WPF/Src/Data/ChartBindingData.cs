// <copyright file="ChartBindingData.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;
    using System.Xml;

    /// <summary>
    /// Represents chart binding data class.
    /// </summary>
     public class ChartBindingData : IChartData, ISupportInitialize, IDisposable
    {
        #region Members
        /// <summary>
        /// Initializes m_source
        /// </summary>
        private IEnumerable m_source;

        /// <summary>
        /// Initializes m_xPath
        /// </summary>
        private string m_xPath;

        /// <summary>
        /// Initializes m_valuesPath
        /// </summary>
        private string[] m_valuesPath;

        /// <summary>
        /// Declares m_xValueType
        /// </summary>
        private ChartValueType m_xValueType = ChartValueType.Double;

        /// <summary>
        /// Initializes m_isInitializeEnable
        /// </summary>
        private bool m_isInitializeEnable;

        /// <summary>
        /// Initializes m_points
        /// </summary>
        private ObservableCollection<IChartDataPoint> m_points = new ObservableCollection<IChartDataPoint>();


        internal ChartSeries series = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        [DefaultValue(null)]
        public IEnumerable Source
        {
            get
            {
                return m_source;
            }

            set
            {
                if (m_source != value)
                {
                    if (m_source != null)
                    {
                        UnwireSource(m_source);
                    }

                    m_source = value;
                    WireSource(m_source);
                    this.RefreshPoints();
                }
            }
        }

        /// <summary>
        /// Gets or sets the X path.
        /// </summary>
        /// <value>The X path.</value>
        [DefaultValue(null)]
        public string XPath
        {
            get
            {
                return m_xPath;
            }

            set
            {
                if (m_xPath != value)
                {
                    m_xPath = value;
                    this.RefreshPoints();
                }
            }
        }

        /// <summary>
        /// Gets or sets the Y paths.
        /// </summary>
        /// <value>The Y paths.</value>
        [DefaultValue(null), TypeConverter(typeof(ChartPathsConverter))]
        public string[] YPaths
        {
            get
            {
                return m_valuesPath;
            }

            set
            {
                if (m_valuesPath != value)
                {
                    m_valuesPath = value;
                    this.RefreshPoints();
                }
            }
        }

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        /// <value>The type of the value.</value>
        public ChartValueType XValueType
        {
            get
            {
                return m_xValueType;
            }
        }

        /// <summary>
        /// Get and Set ChartXValueTypeProperty
        /// </summary>
        public ChartValueType ChartXValueType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the chart points count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return m_points.Count;
            }
        }

        /// <summary>
        /// Gets the <see cref="Syncfusion.Windows.Chart.IChartDataPoint"/> at the specified index.
        /// </summary>
        /// <param name="index">The index value</param>
        /// <returns>The Chart Data point</returns>
        public IChartDataPoint this[int index]
        {
            get
            {
                if (index < m_points.Count)
                    return m_points[index];
                else
                    return null;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when data is changed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                m_points.CollectionChanged += value;
            }

            remove
            {
                m_points.CollectionChanged -= value;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Signals the object that initialization is starting.
        /// </summary>
        public void BeginInit()
        {
            m_isInitializeEnable = true;
        }

        /// <summary>
        /// Signals the object that initialization is complete.
        /// </summary>
        public void EndInit()
        {
            m_isInitializeEnable = false;
            this.ResetPoints();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        public void Dispose()
        {
            if (m_points != null)
            {
                for (int i = 0; i < m_points.Count; i++)
                {
                    if (m_points[i] != null)
                    {
                        m_points[i].Item = null;
                        if (m_points[i].ParentSegment != null)
                        {
                            m_points[i].ParentSegment.Dispose();
                        }
                        m_points[i].StringItem = null;
                        m_points[i].Values = null;
                        m_points[i].Dispose();
                        m_points[i] = null;
                    }
                }
                m_points.Clear();
            }
            if (Source != null)
                ((INotifyCollectionChanged)Source).CollectionChanged -= new NotifyCollectionChangedEventHandler(OnChartSeriesDataCollectionChanged);
            Source = null;

        }

        /// <summary>
        /// Wires the source.
        /// </summary>
        /// <param name="source">The source.</param>
        private void WireSource(IEnumerable source)
        {
            if (source is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)source).CollectionChanged += new NotifyCollectionChangedEventHandler(OnChartSeriesDataCollectionChanged);
            }
        }

        /// <summary>
        /// The DataObjectPropertyChanged method
        /// </summary>
        /// <param name="sender">The object sender</param>
        /// <param name="e">The PropertyChangedEvent Argument</param>
        /// <remarks></remarks>
        private void DataObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            int position = 0;
            foreach (object o in m_source)
            {
                if (o.Equals(sender))
                {
                    break;
                }

                position++;
            }

            m_points[position] = this.GetPoint(position, sender);
        }

        /// <summary>
        /// Unwires the source.
        /// </summary>
        /// <param name="source">The source.</param>
        private void UnwireSource(IEnumerable source)
        {
            if (source is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)source).CollectionChanged -= new NotifyCollectionChangedEventHandler(OnChartSeriesDataCollectionChanged);
            }

            foreach (object o in source)
            {
                if (o is INotifyPropertyChanged)
                {
                    ((INotifyPropertyChanged)o).PropertyChanged -= new PropertyChangedEventHandler(DataObjectPropertyChanged);
                }
            }
        }

        /// <summary>
        /// Called when data source is changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnChartSeriesDataCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            if (series != null)
            {
                if (series.Area != null)
                {

                    series.Area.BeginInit();
                    if (e.Action == NotifyCollectionChangedAction.Remove)
                    {
                        foreach (object obj in e.OldItems)
                        {
                            if (obj is INotifyPropertyChanged)
                            {
                                (obj as INotifyPropertyChanged).PropertyChanged -= new PropertyChangedEventHandler(DataObjectPropertyChanged);
                            }

                            m_points[e.OldStartingIndex].Dispose();
                            m_points.RemoveAt(e.OldStartingIndex);
                        }
                    }

                    if (e.Action == NotifyCollectionChangedAction.Add)
                    {
                        foreach (object obj in e.NewItems)
                        {
                            if (obj is INotifyPropertyChanged)
                            {
                                (obj as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(DataObjectPropertyChanged);
                            }

                            if (e.NewStartingIndex != m_points.Count)
                            {
                                m_points.Insert(e.NewStartingIndex, this.GetPoint(0, obj));
                            }
                            else
                            {
                                m_points.Add(this.GetPoint(0, obj));
                            }
                        }

                        // return;
                    }



                    if (e.Action == NotifyCollectionChangedAction.Reset)
                    {

                        this.RefreshPoints();
                        //  return;

                    }
                }
                if (series.Area != null)
                {
                    series.Area.EndInit();
                }


            }
            ////foreach (object obj in e.NewItems)
            ////{
            ////  if (obj is INotifyPropertyChanged)
            ////  {
            ////    (obj as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(DataObjectPropertyChanged);
            ////  }
            ////  m_points.Add(this.GetPoint(m_points.Count, obj));
            ////}
        }

        /// <summary>
        /// Resets the points.
        /// </summary>
        private void ResetPoints()
        {
            if (!m_isInitializeEnable)
            {
                //foreach (IChartDataPoint point in m_points)
                //{
                //    point.Dispose();
                //}

                for (int i = 0; i < m_points.Count; i++)
                {
                    (m_points[i] as ChartPoint).DisposePoint();
                    //m_points[i] = null;
                }

                m_points.Clear();

                if (m_source != null)
                {
                    m_xValueType = ChartValueType.Double;
                    foreach (object obj in m_source)
                    {
                        if (obj is INotifyPropertyChanged)
                        {
                            (obj as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(DataObjectPropertyChanged);
                        }
                        m_points.Add(this.GetPoint(m_points.Count, obj));
                    }
                }
            }
        }

        /// <summary>
        /// Refreshes the points.
        /// </summary>
        private void RefreshPoints()
        {
            this.ResetPoints();
        }

        /// <summary>
        /// Gets the point.
        /// </summary>
        /// <param name="index">The index value.</param>
        /// <param name="obj">The obj value.</param>
        /// <returns>Returns the ChartDataPoint</returns>
        private IChartDataPoint GetPoint(int index, object obj)
        {
            IChartDataPoint result = null;
            object xValue = null;
            if (obj is IChartDataPoint)
            {
                result = obj as IChartDataPoint;
            }
            else
            {
                double xPosition = index;

                if (!string.IsNullOrEmpty(m_xPath))
                {
                    xValue = ChartDataUtils.GetObjectByPath(obj, m_xPath);

                    if (xValue is DateTime)
                    {
                        m_xValueType = ChartValueType.DateTime;
                    }
                    if (xValue is TimeSpan)
                    {
                        TimeSpan ts = (TimeSpan)xValue;
                        xValue = ts;
                        m_xValueType = ChartValueType.TimeSpan;
                    }
                    xPosition = ChartDataUtils.ConvertToDouble(xValue);
                }

                if (m_valuesPath == null)
                {
                    result = new ChartPoint(xPosition, new double[] { ChartDataUtils.ConvertToDouble(obj) }) { Tag = obj };
                }
                else
                {
                    double[] yValues = new double[m_valuesPath.Length];

                    for (int i = 0; i < m_valuesPath.Length; i++)
                    {
                        yValues[i] = ChartDataUtils.ConvertToDouble(ChartDataUtils.GetDoubleByPath(obj, m_valuesPath[i]));
                    }

                    if (obj is XmlElement)
                    {
                        result = new ChartPoint(xPosition, yValues) { Item = xValue };
                    }
                    else
                    {
                        if (double.IsNaN(xPosition))
                        {
                            result = new ChartPoint(xPosition, yValues, obj) { Tag = obj, StringItem = xValue };
                            this.m_xValueType = ChartValueType.String;
                        }
                        else
                        {
                            result = new ChartPoint(xPosition, yValues, obj) { Tag = obj };
                        }
                    }
                }
            }

            return result;
        }
        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (m_points != null)
            {
                foreach (ChartPoint item in m_points)
                {
                    item.DisposePoint();
                    item.Dispose();
                }
                m_points.Clear();
            }

            if (Source != null)
            {
                if (Source is INotifyCollectionChanged)
                    ((INotifyCollectionChanged)Source).CollectionChanged -= new NotifyCollectionChangedEventHandler(OnChartSeriesDataCollectionChanged);
            }
            Source = null;

            this.m_valuesPath = null;

        }

        #endregion
    }

    /// <summary>
    /// Represents ChartBindingDataExtension
    /// </summary>
    [MarkupExtensionReturnType(typeof(ChartBindingData))]
    public class ChartBindingDataExtension : MarkupExtension
    {
        #region Constants
        /// <summary>
        /// Initializes m_splitter
        /// </summary>
        private const string M_splitter = " ";
        #endregion

        #region Members
        /// <summary>
        /// Initializes m_source
        /// </summary>
        private IEnumerable m_source;

        /// <summary>
        /// Initializes m_xPath
        /// </summary>
        private string m_xPath;

        /// <summary>
        /// Initializes m_yPaths
        /// </summary>
        private string[] m_yPaths;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        public IEnumerable Source
        {
            get { return m_source; }
            set { m_source = value; }
        }

        /// <summary>
        /// Gets or sets the X path.
        /// </summary>
        /// <value>The X path.</value>
        public string XPath
        {
            get { return m_xPath; }
            set { m_xPath = value; }
        }

        /// <summary>
        /// Gets or sets the Y paths.
        /// </summary>
        /// <value>The Y paths.</value>
        [DefaultValue(null), TypeConverter(typeof(ChartPathsConverter))]
        public string[] YPaths
        {
            get { return m_yPaths; }
            set { m_yPaths = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartBindingDataExtension"/> class.
        /// </summary>
        public ChartBindingDataExtension()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartBindingDataExtension"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public ChartBindingDataExtension(IEnumerable source)
        {
            m_source = source;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// When implemented in a derived class, returns an object that is set as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
        /// <returns>
        /// The object value to set on the property where the extension is applied.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            ChartBindingData bindingData = new ChartBindingData();

            bindingData.BeginInit();
            bindingData.XPath = m_xPath;
            bindingData.YPaths = m_yPaths;
            bindingData.Source = m_source;
            bindingData.EndInit();

            return bindingData;
        }
        #endregion
    }

   
}
