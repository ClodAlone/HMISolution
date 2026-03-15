#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Map
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;
    using Syncfusion.Maps.Imagery.Common;


    internal class CustomBackgroundWorker : BackgroundWorker
    {
        #region Public Fields
        internal Tile Tile;
#if WPF
        internal string TileImageUri;
#endif
        internal int X;
        internal int Y;
        internal int ZoomLevel;
        internal MapStyle MapStyle;
        internal string BingMapKey;

        #endregion
        internal CustomBackgroundWorker(Tile _tile, int _x, int _y, int _zoomlevel, MapStyle _mapStyle, string _bingMapKey)
        {
            this.Tile = _tile;
            this.X = _x;
            this.Y = _y;
            this.ZoomLevel = _zoomlevel;
            this.MapStyle = _mapStyle;
            this.BingMapKey = _bingMapKey;
        }
    }
}
