#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT
namespace Syncfusion.Windows.Grid.Olap
#else
using Syncfusion.Silverlight.Grid.Olap;
namespace Syncfusion.Silverlight.Grid.Olap
#endif
{
    public delegate void LinkLabelClickEventHander(object sender, LinkLabelEventArgs e);

    public delegate void OlapGridDrillDownEventHander(object sender, OlapGridDrillDownEventArgs e);

    public delegate void SelectionChanged(object sender, OlapGridSelectionChangedEventArgs e);
}
