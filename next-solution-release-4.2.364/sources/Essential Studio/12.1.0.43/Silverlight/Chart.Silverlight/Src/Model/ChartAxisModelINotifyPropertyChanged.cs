#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
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

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Class implementation for ChartAxisModel
    /// </summary>
    public class ChartAxisModel : INotifyPropertyChanged
    {
        Size m_totalRenderSize = new Size();
        /// <summary>
        /// Get or Set totalrenderSize
        /// </summary>
        public Size TotalRenderSize
        {
            get
            {
                return m_totalRenderSize;
            }

            set
            {
                m_totalRenderSize = value;
                RaisePropertyChangedEvent("TotalRenderSize");
            }
        }


        DoubleRange m_Range = new DoubleRange();
        /// <summary>
        /// Get or Set range Property
        /// </summary>
        public DoubleRange Range
        {
            get
            {
                return m_Range;
            }

            set
            {
                m_Range = value;
                RaisePropertyChangedEvent("Range");
            }
        }

        double m_Interval = 1d;
        /// <summary>
        /// Get or Set m_visibleinterval property
        /// </summary>
        public double m_visibleInterval
        {
            get
            {
                return m_Interval;
            }

            set
            {
                m_Interval = value;
                RaisePropertyChangedEvent("m_visibleInterval");
            }
        }

        bool m_OppsedPosition = false;
        /// <summary>
        /// Get or Set OpposedPosition property
        /// </summary>
        public bool OpposedPosition
        {
            get
            {
                return m_OppsedPosition;
            }

            set
            {
                m_OppsedPosition = value;
                RaisePropertyChangedEvent("OpposedPosition");
            }
        }

        bool m_IsAutoSetRange = true;
        /// <summary>
        /// Get or Set IsAutoSetRangeProperty
        /// </summary>
        public bool IsAutoSetRange
        {
            get
            {
                return m_IsAutoSetRange;
            }

            set
            {
                m_IsAutoSetRange = value;
                RaisePropertyChangedEvent("IsAutoSetRange");
            }
        }

        Orientation m_Orientation = Orientation.Horizontal;
        /// <summary>
        /// Get or Set Orientation property
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return m_Orientation;
            }

            set
            {
                m_Orientation = value;
                RaisePropertyChangedEvent("Orientation");
            }
        }

        object m_Header = string.Empty;
        /// <summary>
        /// Get or Set Header Property
        /// </summary>
        public object Header
        {
            get
            {
                return m_Header;
            }

            set
            {
                m_Header = value;
                RaisePropertyChangedEvent("Header");
            }
        }

        ChartLabelIntersectAction m_IntersectAction = ChartLabelIntersectAction.None;
        /// <summary>
        /// Get or Set intersectActionProperty
        /// </summary>
        public ChartLabelIntersectAction IntersectAction
        {
            get
            {
                return m_IntersectAction;
            }

            set
            {
                m_IntersectAction = value;
                RaisePropertyChangedEvent("IntersectAction");
            }
        }


        #region INotifyPropertyChanged Members

        /// <summary>
        /// Called when Property changed in ChartAxisModel class
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChangedEvent(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
