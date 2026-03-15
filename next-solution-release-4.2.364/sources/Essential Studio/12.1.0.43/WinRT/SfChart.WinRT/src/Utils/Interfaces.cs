#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
#if WINDOWS_PHONE
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// Inteface implementation for IRangeAxis
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRangeAxis<T> where T : IComparable
    {
        /// <summary>
        /// Get or Set Minimum property
        /// </summary>
        T Minimum
        {
            get;
            set;
        }

        /// <summary>
        /// Get or Set Maximum property
        /// </summary>
        T Maximum
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Interface implementation for IRangeAxis
    /// </summary>
    public interface IRangeAxis
    {
        /// <summary>
        /// Get Range property
        /// </summary>
        DoubleRange Range { get; }
    }

    /// <summary>
    /// Interface implementation for IChartAxis
    /// </summary>
    public interface IChartAxis
    {
        /// <summary>
        /// Get or Set VisibleLabels property
        /// </summary>
        ChartAxisLabelCollection VisibleLabels
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Interface implementation for IChartSeries
    /// </summary>
    public interface IChartSeries
    {
        /// <summary>
        /// Get or Set ItemsSource property
        /// </summary>
        IEnumerable ItemsSource
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Interface implementation for ISupportAxes
    /// </summary>
    public interface ISupportAxes
    {
        /// <summary>
        /// Get XRange property
        /// </summary>
        DoubleRange XRange
        {
            get;
        }

        /// <summary>
        /// Get YRange property
        /// </summary>
        DoubleRange YRange
        {
            get;
        }

        /// <summary>
        /// Get ActualXAxis property
        /// </summary>
        ChartAxis ActualXAxis { get; }
        /// <summary>
        /// Get ActualYAxis property
        /// </summary>
        ChartAxis ActualYAxis { get; }
    }

    public interface ISupportAxes2D : ISupportAxes
    {
        ///<summary>
        /// Get or Set YAxis property
        /// </summary>
        RangeAxisBase YAxis
        {
            get;
            set;
        }

        /// <summary>
        /// Get or Set XAxis property
        /// </summary>
        ChartAxisBase2D XAxis
        {
            get;
            set;
        }
    }

    public interface ISupportAxes3D : ISupportAxes
    {
        ///<summary>
        /// Get or Set YAxis property
        /// </summary>
        RangeAxisBase3D YAxis
        {
            get;
            set;
        }

        /// <summary>
        /// Get or Set XAxis property
        /// </summary>
        ChartAxisBase3D XAxis
        {
            get;
            set;
        }
    }


    /// <summary>
    /// Interface implementation for ICloneable
    /// </summary>
    public interface ICloneable
    {
        DependencyObject Clone();
    }

}
