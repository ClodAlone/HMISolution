#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.XlsIO.Implementation
{
  public class MathGeneral
  {
    public static double Truncate( double value )
    {
#if SILVERLIGHT
      double result = Math.Round( value );

      if( result > value )
        result -= 1;

      return result;
#else
      return Math.Truncate( value );
#endif
    }
  }
}
