// <copyright file="ChartDataPoint.cs" company="Syncfusion">
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
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Represents Chart point class. The chart point that WPF chart system uses to build series.
    /// </summary>
    /// <seealso cref="ChartPoint"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [Browsable(false)]
    public class ChartPoint : DependencyObject, IChartDataPoint, INotifyPropertyChanged 
    {
        #region dependency properties
        /// <summary>
        /// Identifies the Visible dependency property.
        /// </summary>
        public static readonly DependencyProperty VisibleProperty =
                DependencyProperty.Register("Visible", typeof(bool), typeof(ChartPoint), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnVisibleChanged)));
        #endregion

        #region Properties
        /// <summary>
        /// Gets object initially wrapped by this instance.
        /// </summary>
        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets parent segment for point.
        /// </summary>
        public ChartSegment ParentSegment
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets visibility of point. Affects Pie, Doughnut, Pyramid and Funel chart types only. This is dependency property.
        /// </summary>
        public bool Visible
        {
            get { return (bool)GetValue(VisibleProperty); }
            set { SetValue(VisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the X.
        /// </summary>
        /// <value>The X value.</value>
        public double X
        {
            [DebuggerStepThrough]
            get
            {
                return m_x;
            }

            [DebuggerStepThrough]
            set
            {
                m_x = value;
                OnPropertyChanged("X");
            }
        }

        /// <summary>
        /// Gets or sets the Y.
        /// </summary>
        /// <value>The Y value.</value>
        public double Y
        {
            [DebuggerStepThrough]
            get
            {
                return m_values[0];
            }

            set
            {
                if (m_values == null)
                {
                    m_values = new double[] { value };
                }
                else
                {
                    m_values[0] = value;
                }

                if (m_item == null)
                {
                    m_item = value;
                }

                OnPropertyChanged("Y");
            }
        }

        /// <summary>
        /// Gets or sets the values array that correspond to X.
        /// </summary>
        /// <value>The values.</value>
        [TypeConverter(typeof(DoubleArrayConverter))]
        public double[] Values
        {
            get
            {
                return m_values;
            }

            set
            {
                m_values = value;
                OnPropertyChanged("Values");
            }
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>The item value.</value>
        public object Item
        {
            get
            {
                return m_item;
            }

            set
            {
                m_item = value;
            }
        }

        /// <summary>
        /// Gets or sets the item that points represents string values.
        /// </summary>
        public object StringItem
        {
            get
            {
                return m_StringItem;
            }
            set
            {
                m_StringItem = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this point is empty.
        /// </summary>
        /// <value><c>true</c> if this point is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty
        {
            get { return m_values == null; }
        }

        /// <summary>
        /// Gets or sets the Label
        /// </summary>
        public string Label
        {
            get
            {
                if (m_label == String.Empty)
                {
                    return this.Values[0].ToString();
                }
                else
                {
                    return m_label;
                }
            }

            set
            {
                m_label = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether empty point.
        /// </summary>
        /// <value><c>true</c> if empty point otherwise, <c>false</c>.</value>
        public bool EmptyPoint
        {
            get
            {
                return m_emptyPoint;
            }

            set
            {
                m_emptyPoint = value;
            }
        }

        #endregion

        #region Members
        /// <summary>
        /// Initializes m_StringItem
        /// </summary>
        private object m_StringItem;

        /// <summary>
        /// Initializes m_label
        /// </summary>
        private string m_label = String.Empty;

        /// <summary>
        /// Initializes m_x
        /// </summary>
        private double m_x;

        /// <summary>
        /// Initializes m_values
        /// </summary>
        private double[] m_values;

        /// <summary>
        /// Initializes m_item
        /// </summary>
        private object m_item = new object();

        /// <summary>
        /// Declared m_emptyPoint
        /// </summary>
        private bool m_emptyPoint;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPoint"/> class.
        /// </summary>
        public ChartPoint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPoint"/> class.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="values">The values.</param>
        /// <param name="source">The source.</param>
        public ChartPoint(double x, double[] values, object source)
        {
            m_x = x;
            m_values = values;
            m_item = source;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPoint"/> class.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <param name="source">The source.</param>
        public ChartPoint(double x, double y, object source)
        {
            m_x = x;
            m_values = new double[] { y };
            m_item = source;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPoint"/> class.
        /// </summary>
        /// <param name="x">The X value.</param>
        /// <param name="values">Array of values that correspond to X.</param>
        public ChartPoint(double x, double[] values)
        {
            m_x = x;
            m_values = values;
            m_item = values[0];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPoint"/> class.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        public ChartPoint(double x, double y)
        {
            m_x = x;
            m_values = new double[] { y };
            m_item = y;
        }

        /// <summary>
        /// Called when instance created for ChartPoint
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="values"></param>
        public ChartPoint(double x, double y, double[] values)
        {
            m_x = x;
            m_values = values;
            m_item = y;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            ChartPoint cp = new ChartPoint(m_x, m_values, m_item);
            cp.ParentSegment = ParentSegment;
            cp.Visible = Visible;
            return cp;
        }

        /// <summary>
        /// Remove All elements from the ChartPoint
        /// </summary>
        public void DisposePoint()
        {
            //m_item = null;
            //m_values = null;
            ////Item = null;
            //Tag = null;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        public void Dispose()
        {
           //this.ParentSegment = null;
           // this.Item = null;
        }

        /// <summary>
        /// Invoked when Visible property changes. Invalidates parent series.
        /// </summary>
        /// <param name="dObj">The DependencyObject dObj</param>
        /// <param name="args">The DependencyPropertyChangedEventArgs args</param>
        private static void OnVisibleChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs args)
        {
            ChartPoint point = dObj as ChartPoint;
            if (point != null && point.ParentSegment != null)
            {
                ////Invalidates parent series.
                point.ParentSegment.Series.Invalidate();
            }
        }
        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }


    #region struct ChartIndexedDataPoint
    /// <summary>
    /// Represents indexed chart point
    /// </summary>
    /// <exclude/>
    public struct ChartIndexedDataPoint
    {
        #region Members
        /// <summary>
        /// Initializes m_index
        /// </summary>
        private int m_index;

        /// <summary>
        /// Initializes m_dataPoint
        /// </summary>
        private IChartDataPoint m_dataPoint;
        #endregion

        #region Properties
        /// <summary>
        /// Gets index of point.
        /// </summary>
        public int Index
        {
            get
            {
                return m_index;
            }
        }

        /// <summary>
        /// Gets chart point.
        /// </summary>
        /// <value>The data point.</value>
        public IChartDataPoint DataPoint
        {
            get
            {
                return m_dataPoint;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Chart.ChartIndexedDataPoint">ChartIndexedDataPoint</see> structure. 
        /// </summary>
        /// <param name="chartPoint">The chart point.</param>
        /// <param name="index">Index of point.</param>
        internal ChartIndexedDataPoint(IChartDataPoint chartPoint, int index)
        {
            m_dataPoint = chartPoint;
            m_index = index;
        }
        #endregion
    }
    #endregion

    
}
