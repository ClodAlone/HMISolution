#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !WITHOUTOWNTYPES
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;

namespace XlsIO.Silverlight.Implementation.Exceptions
{
    public class ArgumentOutOfRangeException : Exception
    {
        public ArgumentOutOfRangeException( string name, object value, string message ) :
            base( string.Format( "{0}. Real value of {1} was {2}", message, name, value ) )
        {
        }
    }
}
#endif