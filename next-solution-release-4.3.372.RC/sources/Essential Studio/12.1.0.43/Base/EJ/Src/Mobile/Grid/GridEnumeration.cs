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
using Syncfusion.JavaScript.Shared;
using System.Runtime.Serialization;

namespace Syncfusion.JavaScript
{
    [DataContract]
    public enum PagerDisplay
    {
        [EnumMember(Value = "Normal")]
        Normal,

        [EnumMember(Value = "Fixed")]
        Fixed
    }
    [DataContract]
    public enum PagerType
    {
        [EnumMember(Value = "Scrollbale")]
        Scrollbale,

        [EnumMember(Value = "Normal")]
        Normal
    }
    [DataContract]
    public enum SortType
    {
        [EnumMember(Value = "Ascending")]
        Ascending,

        [EnumMember(Value = "Descending")]
        Descending,

        [EnumMember(Value = "ClearSorting")]
        ClearSorting
    }
}
