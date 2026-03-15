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
using System.Runtime.Serialization;

namespace Syncfusion.JavaScript.DataVisualization
{
    // Chart Series type
    [DataContract]
    public enum NavigatorType
    {
        [EnumMember(Value = "line")]
        Line,
        [EnumMember(Value = "spline")]
        Spline,
        [EnumMember(Value = "area")]
        Area,
        [EnumMember(Value = "splinearea")]
        SplineArea,
        [EnumMember(Value = "StepArea")]
        StepArea,
        [EnumMember(Value = "StepLine")]
        StepLine,
        
    }
     
    //Legend position
    [DataContract]
    public enum NavigatorPosition
    {
        [EnumMember(Value = "top")]
        Top,
        [EnumMember(Value = "bottom")]
        Bottom,
        [EnumMember(Value = "left")]
        Left,
        [EnumMember(Value = "right")]
        Right,
        [EnumMember(Value = "custom")]
        Custom
    }
    //Legend alignment
    [DataContract]
    public enum HorizontalAlignment
    {
        [EnumMember(Value = "center")]
        Center,
        [EnumMember(Value = "left")]
        Left,
        [EnumMember(Value = "right")]
        Right,
    }
     
     
     
    //Interval type
    [DataContract]
    public enum NavigatorIntervalType
    {
        [EnumMember(Value = "years")]
        Years,
        [EnumMember(Value = "quarters")]
        Quarters,
        [EnumMember(Value = "months")]
        Seconds,
        [EnumMember(Value = "days")]
        Days,
        [EnumMember(Value = "hours")]
        Hours       
    }
    //Font style
    [DataContract]
    public enum RangeNavigatorFontStyle
    {
        [EnumMember(Value = "normal")]
        Normal,
        [EnumMember(Value = "bold")]
        Bold,
        [EnumMember(Value = "italic")]
        Italic
    }
	//Font weight
	[DataContract]
    public enum RangeNavigatorFontWeight
    {
        [EnumMember(Value = "regular")]
        Regular,
        [EnumMember(Value = "lighter")]
        Lighter
    }
    
   
    
}