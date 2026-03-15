#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Syncfusion.JavaScript.DataVisualization.Models
{

    [DataContract]
    public enum DockPosition
    {
        [EnumMember(Value = "none")]
        None,
        [EnumMember(Value = "topleft")]
        Topleft,
        [EnumMember(Value = "topcenter")]
        TopCenter,
        [EnumMember(Value = "topright")]
        TopRight,
        [EnumMember(Value = "centerleft")]
        CenterLeft,
        [EnumMember(Value = "center")]
        Center,
        [EnumMember(Value = "centerright")]
        CenterRight,
        [EnumMember(Value = "bottomleft")]
        BottomLeft,
        [EnumMember(Value = "bottomcenter")]
        BottomCenter,
        [EnumMember(Value = "bottomright")]
        BottomRight
    }
    [DataContract]
    public enum LegendIcons
    {
        [EnumMember(Value = "rectangle")]
        Rectangle,
        [EnumMember(Value = "circle")]
        Cirle
    }

    public enum LabelSize
    {
        [EnumMember(Value = "fixed")]
        Fixed,
        [EnumMember(Value = "default")]
        Default
    }

    public enum LegendMode
    {
        [EnumMember(Value = "default")]
        Default,
        [EnumMember(Value = "interactive")]
        Interactive
    }

    public enum MapType
    {
        [EnumMember(Value = "geomtery")]
        Geomtery,
        [EnumMember(Value = "osm")]
        OSM
        //[EnumMember(Value = "bing")]
        //Bing,
        //[EnumMember(Value = "custom")]
        //Custom,
    }

    [DataContract]
    public enum ColorPalette
    {
        [EnumMember(Value = "palette1")]
        Palette1,
        [EnumMember(Value = "palette2")]
        Palette2,
        [EnumMember(Value = "palette3")]
        Palette3,
        [EnumMember(Value = "custompalette")]
        CustomPalette
    }
    
}
