#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Windows.Controls.Map;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Maps.MVVM
{
    public class ShapeFileLayerCommandBehaviorBase<TReturn, TEventArgs> : BuilderCommandBehaviorBase<ShapeFileLayer, TEventArgs, TReturn>
    { }
    public class MapControlCommandBehaviorBase<TReturn, TEventArgs> : BuilderCommandBehaviorBase<MapControl, TEventArgs, TReturn>
    { }
    public class ImageryLayerCommandBehaviorBase<TReturn, TEventArgs> : BuilderCommandBehaviorBase<ImageryLayer, TEventArgs, TReturn>
    {}
}