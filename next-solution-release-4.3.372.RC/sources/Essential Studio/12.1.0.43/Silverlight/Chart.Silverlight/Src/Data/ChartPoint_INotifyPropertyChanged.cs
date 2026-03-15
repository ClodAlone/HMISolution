#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.ComponentModel;

    /// <summary>
    /// Represents Chart point class. The chart point that WPF chart series uses to build series.
    /// </summary>
    public class ChartPoint : INotifyPropertyChanged
    {
        #region Members
        private double m_x = 0d;
        private double m_y = 0d;
        private double[] m_values;
        private bool m_emptypoint;
        private string m_stringItem;
        private object m_axisContent;
        private double m_positionValue;
        #endregion

        #region Constructor
        /// <summary>
        /// Create the default initialize value for ChartPoint
        /// </summary>
        public ChartPoint()
        {
            this.Values = new double[1];
            this.X = 0;
            this.Y = 0;
            this.Visible = true;
        }

        /// <summary>
        /// Create Chart point by passing the both X and collection of Y values of Chart.
        /// </summary>
        /// <param name="x">double x value</param>
        /// <param name="y">list of double y value</param>
        public ChartPoint(double x, double[] y)
        {
            this.Values = y;
            this.X = x;
            this.Y = y[0];
            this.Visible = true;
        }

        /// <summary>
        /// Create Instance for the chart point by passing the X and Y values.
        /// </summary>
        /// <param name="x">double x value</param>
        /// <param name="y">double y value</param>
        public ChartPoint(double x, double y)
        {
            this.Values = new double[1];
            this.X = x;
            this.Y = y;
            this.Values[0] = y;
            this.Visible = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get or Set Stringitem property
        /// </summary>
        public string StringItem
        {
            get { return m_stringItem; }
            set { m_stringItem = value; }
        }
        /// <summary>
        /// Get or Set Axiscontent property
        /// </summary>
        public object AxisContent
        {
            get { return m_axisContent; }
            set { m_axisContent = value; }
        }
        /// <summary>
        /// Get or Set AxisPosition property
        /// </summary>
        public double AxisPosition
        {
            get { return m_positionValue; }
            set { m_positionValue = value; }
        }

        object m_Tag = null;
        /// <summary>
        /// Get or Set Tag property
        /// </summary>
        public object Tag
        {
            get { return m_Tag; }
            set { m_Tag = value; }
        }
        /// <summary>
        /// Get or Set emptyPoint property
        /// </summary>
        public bool EmptyPoint
        {
            get { return m_emptypoint; }
            set { m_emptypoint = value; }
        }

        /// <summary>
        /// Gets or sets the X value of Chart
        /// </summary>
        public double X
        {
            get
            {
                return m_x;
            }

            set
            {
                m_x = value;
                OnPropertyChanged("X");
            }
        }

        /// <summary>
        /// Gets or sets the Y value of Chart
        /// </summary>
        public double Y
        {
            get
            {
                return m_y;
            }

            set
            {
                m_y = value;
                this.Values[0] = value;
                //if (this.Y <= 0)
                //{
                //    //this.Visible = false;
                //}
                OnPropertyChanged("Y");
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Visibility state of Chart Segment.
        /// </summary>
        public bool Visible {
            get; 
            internal set; }

        /// <summary>
        /// Gets or sets the double array y values for Chart.
        /// </summary>
        [TypeConverter(typeof(ChartYPointsConveter))]
        public double[] Values 
        {
            get
            {
                return m_values;
            }

            set
            {
                m_values = value;
                this.m_y = m_values[0];
                OnPropertyChanged("Values");
            }
        }
        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Evant creation for PropertyChanged in chartPoint class
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
}
