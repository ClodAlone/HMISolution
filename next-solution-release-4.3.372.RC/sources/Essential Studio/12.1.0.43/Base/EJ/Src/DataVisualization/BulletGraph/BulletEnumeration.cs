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
using System.Text;
using System.Threading.Tasks;
using Syncfusion.JavaScript;

using System.Runtime.Serialization;

namespace Syncfusion.JavaScript.DataVisualization
{
    [DataContract]
    public enum FlowDirection
    {
        [EnumMember(Value = "forward")]
        Forward,
        [EnumMember(Value = "backward")]
        Backward
    }

    [DataContract]
    public enum TickPosition
    {
        [EnumMember(Value = "below")]
        Below,
        [EnumMember(Value = "above")]
        Above,
        [EnumMember(Value = "cross")]
        Cross
    }

    [DataContract]
    public enum LabelPosition
    {
        [EnumMember(Value = "below")]
        Below,
        [EnumMember(Value = "above")]
        Above
    }

    [DataContract]
    public enum BulletFontStyle
    {
        [EnumMember(Value = "Normal")]
        Normal,
        [EnumMember(Value = "Italic")]
        Italic,
        [EnumMember(Value = "Oblique")]
        Oblique
    }
}