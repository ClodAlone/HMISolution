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

namespace Syncfusion.JavaScript.DataVisualization
{
    // Button Size Enumerations
    [DataContract]
    public enum BarcodeSymbolType
    {
        [EnumMember(Value = "code39")]
        Code39,
        [EnumMember(Value = "code39extended")]
        Code39Extended,
        [EnumMember(Value = "code11")]
        Code11,
        [EnumMember(Value = "codabar")]
        Codabar,
        [EnumMember(Value = "code32")]
        Code32,
        [EnumMember(Value = "code93")]
        Code93,
        [EnumMember(Value = "code93extended")]
        Code93Extended,
        [EnumMember(Value = "code128a")]
        Code128A,
        [EnumMember(Value = "code128b")]
        Code128B,
        [EnumMember(Value = "code128c")]
        Code128C,
        [EnumMember(Value = "datamatrix")]
        DataMatrix,
        [EnumMember(Value = "qrbarcode")]
        QRBarcode
    }
}
