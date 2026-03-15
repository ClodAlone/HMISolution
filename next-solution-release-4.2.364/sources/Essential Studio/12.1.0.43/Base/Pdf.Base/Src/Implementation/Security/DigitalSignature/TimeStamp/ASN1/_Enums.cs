#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.Pdf.Security
{
    /// <summary>
    /// List of Asn Tags.
    /// http://www.obj-sys.com/asn1tutorial/node124.html
    /// </summary>
    [Flags()]
    internal enum ASN1Tags
    {
        ReservedBER = 0,
        Boolean = 1,
        Integer = 2,
        BitString = 3,
        OctetString = 4,
        Null = 5,
        ObjectIdentifier = 6,
        ObjectDescriptor = 7,
        External = 8,
        Real = 9,
        Enumerated = 10,
        EmbeddedPDV = 11,
        UTF8String = 12,
        RelativeOid = 13,
        Sequence = 16,
        Set = 17,
        NumericString = 18,
        PrintableString = 19,
        TeletexString = 20,
        VideotexString = 21,
        IA5String = 22,
        UTFTime = 23,
        GeneralizedTime = 24,
        GraphicsString = 25,
        VisibleString = 26,
        GeneralString = 27,
        UniversalString = 28,
        CharacterString = 29,
        BMPString = 30,
        Constructed = 32,
        Application = 64,
        Tagged= 128,       
    }
}
