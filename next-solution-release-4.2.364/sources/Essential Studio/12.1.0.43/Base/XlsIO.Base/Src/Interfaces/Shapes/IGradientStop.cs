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
#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;

#endif

namespace Syncfusion.XlsIO.Interfaces.Shapes
{
  /// <summary>
  /// Interface that represents single gradient stop of the gradient fill.
  /// </summary>
  public interface IGradientStop
  {
    /// <summary>
    /// Gets / sets color.
    /// </summary>
    Color Color { get; set; }
    /// <summary>
    /// Gets / sets position of the gradient stop.
    /// </summary>
    int Position { get; set; }
    /// <summary>
    /// Gets / sets transparency value.
    /// </summary>
    int Transparency { get; set; }
  }
}
