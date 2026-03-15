#region Copyright

//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.

#endregion Copyright

using System;
#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
using System.Drawing;
using System.IO;
#elif ( WINRT )
using Syncfusion.XlsIO;
using System.IO;
#else
using System.Drawing;
using System.IO;

#endif

namespace Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing
{
  /// <summary>
  /// Common interface for all records that contains picture.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public interface IPictureRecord
  {
    /// <summary>
    /// Picture that is contained by the record.
    /// </summary>
    Image Picture { get; set; }

    Stream PictureStream { get; set; }
    /// <summary>
    /// Picture id.
    /// </summary>
    byte[] RgbUid { get; }
    }
}
