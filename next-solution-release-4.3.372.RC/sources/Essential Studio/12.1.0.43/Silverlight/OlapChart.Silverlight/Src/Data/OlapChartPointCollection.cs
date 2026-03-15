#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.ObjectModel;
using System.ComponentModel;
using Syncfusion.Silverlight.Chart;
using Syncfusion.Windows.Chart;

namespace Syncfusion.Silverlight.Chart.Olap
{
    [TypeConverter(typeof(ChartListDataConveter))]
    public class OlapChartPointCollection : ObservableCollection<OlapChartPoint>
    {
    }
}
