#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Gauge
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Windows;

    /// <summary>
    /// Observable collection of scales.
    /// </summary>
    public class ScaleCollection : ObservableCollection<CircularScale>
    {
    }

    /// <summary>
    /// Observable collection of state indicators.
    /// </summary>
    public class StateIndicatorsCollection : ObservableCollection<StateIndicator>
    {
    }

    /// <summary>
    /// Observable collection of state ranges.
    /// </summary>
    public class StateRangeCollection : ObservableCollection<StateRange>
    {
    }

    /// <summary>
    /// Observable collection of custom labels.
    /// </summary>
    public class CustomLabelsCollection : ObservableCollection<GaugeLabel>
    {
    }

    /// <summary>
    /// Observable collection of images.
    /// </summary>
    public class ImagesCollection : ObservableCollection<GaugeImage>
    {
    }

    /// <summary>
    /// Observable collection of linear scales.
    /// </summary>
    public class LinearScaleCollection : ObservableCollection<LinearScale>
    {
    }

    /// <summary>
    /// Observable collection of characters.
    /// </summary>
    internal class CharacterCollection : ObservableCollection<CharacterBase>
    {
    }

    /// <summary>
    /// Observable collection of Rolling characters.
    /// </summary>
    public class SegmentCollection : ObservableCollection<RollingCharacter>
    {
    }
}
