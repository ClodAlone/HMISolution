#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.UI.Xaml.Maps
{
    using System;
    using System.Net;
    using System.Windows;
   


    internal class ShapeFileDBFValues
    {
        public const byte EndOfData = 0x1A; //^Z End of File
        public const byte EndOfField = 0x0D; //End of Field
        public const byte False = 0x46; //F in Ascci
        public const byte Space = 0x20; //Space in ascii
        public const byte True = 0x54; //T in ascii
        public const string Unknown = "?"; //Unknown value
    }
}
