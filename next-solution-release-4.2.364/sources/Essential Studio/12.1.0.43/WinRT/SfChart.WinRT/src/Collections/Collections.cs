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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
#if !WINDOWS_PHONE
using System.Threading.Tasks;
#endif

namespace Syncfusion.UI.Xaml.Charts
{
    /// <summary>
    /// A collection class which holds chart legend
    /// </summary>
    public class ChartLegendCollection : ObservableCollection<ChartLegend>
    { 
    
    }

    /// <summary>
    /// A collection class which holds ChartStripLine
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class ChartStripLines : ObservableCollection<ChartStripLine>
    {
    }

    /// <summary>
    /// A collection class which holds ChartAxis.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class ChartAxisCollection : ObservableCollection<ChartAxis>
    {
        /// <summary>
        /// return ChartAxis value from the given string
        /// </summary>
        /// <param name="name"></param>
        public ChartAxis this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                    return null;

                foreach (ChartAxis axis in this)
                {
                    if (axis.Name == name)
                    {
                        return axis;
                    }
                }

                return null;
            }
        }
    }

    /// <summary>
    /// A collection class which holds ChartTrendLine.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class ChartTrendLineCollection : ObservableCollection<Trendline>
    {
        public ChartTrendLineCollection()
        {
            
        }
        /// <summary>
        /// return ChartTrendLine from the given string
        /// </summary>
        /// <param name="name"></param>
        public TrendlineBase this[string name]
        {
            get
            {
                foreach (var trend in this)
                {
                    if (trend.Name == name)
                    {
                        return trend;
                    }
                }

                return null;
            }
        }
    }

    /// <summary>
    /// A collection class which holds ChartSeries.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class ChartSeriesCollection : ObservableCollection<ChartSeries>
    {
        /// <summary>
        /// return ChartSeries from the given string
        /// </summary>
        /// <param name="name"></param>
        public ChartSeries this[string name]
        {
            get
            {
                foreach (ChartSeries series in this)
                {
                    if (series.Name == name)
                    {
                        return series;
                    }
                }

                return null;
            }
        }
    }

    /// <summary>
    /// A collection class which holds ChartSeries 2D.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class ChartVisibleSeriesCollection : ObservableCollection<ChartSeriesBase>
    {
        /// <summary>
        /// return ChartSeries from the given string
        /// </summary>
        /// <param name="name"></param>
        public ChartSeriesBase this[string name]
        {
            get
            {
                foreach (ChartSeriesBase series in this)
                {
                    if (series.Name == name)
                    {
                        return series;
                    }
                }

                return null;
            }
        }
    }

    /// <summary>
    /// A collection class which holds ChartSeries 3D.
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class ChartSeries3DCollection : ObservableCollection<ChartSeries3D>
    {
        /// <summary>
        /// return ChartSeries from the given string
        /// </summary>
        /// <param name="name"></param>
        public ChartSeries3D this[string name]
        {
            get
            {
                foreach (ChartSeries3D series in this)
                {
                    if (series.Name == name)
                    {
                        return series;
                    }
                }

                return null;
            }
        }
    }

    /// <summary>
    /// A collection class which holds ChartRowDefinitions
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class ChartRowDefinitions : ObservableCollection<ChartRowDefinition> 
    {
    }

    /// <summary>
    /// A collection class which holds ChartColumnDefinitions
    /// </summary>
    [ClassReference(IsReviewed = false)]
    public class ChartColumnDefinitions : ObservableCollection<ChartColumnDefinition> 
    {
    }
}
