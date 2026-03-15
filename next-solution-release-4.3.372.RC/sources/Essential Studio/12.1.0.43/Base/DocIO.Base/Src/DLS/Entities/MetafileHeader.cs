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
#if !WINRT && !WP
using System.Drawing;
#endif
//using System.Windows.Controls;
//using System.Windows.Documents;
//using System.Windows.Ink;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Animation;
//using System.Windows.Shapes;

namespace Syncfusion.DocIO.DLS.Entities
{
  public sealed class MetafileHeader
    {
        // Summary:
        //     Gets a System.Drawing.Rectangle that bounds the associated System.Drawing.Imaging.Metafile.
        //
        // Returns:
        //     A System.Drawing.Rectangle that bounds the associated System.Drawing.Imaging.Metafile.
    public Rectangle Bounds
    {
      get
      {
        return new Rectangle();
      }
    }
        ////
        //// Summary:
        ////     Gets the horizontal resolution, in dots per inch, of the associated System.Drawing.Imaging.Metafile.
        ////
        //// Returns:
        ////     The horizontal resolution, in dots per inch, of the associated System.Drawing.Imaging.Metafile.
        //public float DpiX
        //{
        //    get;
        //}
        ////
        //// Summary:
        ////     Gets the vertical resolution, in dots per inch, of the associated System.Drawing.Imaging.Metafile.
        ////
        //// Returns:
        ////     The vertical resolution, in dots per inch, of the associated System.Drawing.Imaging.Metafile.
        //public float DpiY
        //{
        //    get;
        //}
        ////
        //// Summary:
        ////     Gets the size, in bytes, of the enhanced metafile plus header file.
        ////
        //// Returns:
        ////     The size, in bytes, of the enhanced metafile plus header file.
        //public int EmfPlusHeaderSize
        //{
        //    get;
        //}
        ////
        //// Summary:
        ////     Gets the logical horizontal resolution, in dots per inch, of the associated
        ////     System.Drawing.Imaging.Metafile.
        ////
        //// Returns:
        ////     The logical horizontal resolution, in dots per inch, of the associated System.Drawing.Imaging.Metafile.
        //public int LogicalDpiX
        //{
        //    get;
        //}
        ////
        //// Summary:
        ////     Gets the logical vertical resolution, in dots per inch, of the associated
        ////     System.Drawing.Imaging.Metafile.
        ////
        //// Returns:
        ////     The logical vertical resolution, in dots per inch, of the associated System.Drawing.Imaging.Metafile.
        //public int LogicalDpiY
        //{
        //    get;
        //}
        ////
        //// Summary:
        ////     Gets the size, in bytes, of the associated System.Drawing.Imaging.Metafile.
        ////
        //// Returns:
        ////     The size, in bytes, of the associated System.Drawing.Imaging.Metafile.
        //public int MetafileSize
        //{
        //    get;
        //}
        ////
        //// Summary:
        ////     Gets the type of the associated System.Drawing.Imaging.Metafile.
        ////
        //// Returns:
        ////     A System.Drawing.Imaging.MetafileType enumeration that represents the type
        ////     of the associated System.Drawing.Imaging.Metafile.
        //public MetafileType Type
        //{
        //    get;
        //}
        ////
        //// Summary:
        ////     Gets the version number of the associated System.Drawing.Imaging.Metafile.
        ////
        //// Returns:
        ////     The version number of the associated System.Drawing.Imaging.Metafile.
        //public int Version
        //{
        //    get;
        //}
        ////
        //// Summary:
        ////     Gets the Windows metafile (WMF) header file for the associated System.Drawing.Imaging.Metafile.
        ////
        //// Returns:
        ////     A System.Drawing.Imaging.MetaHeader that contains the WMF header file for
        ////     the associated System.Drawing.Imaging.Metafile.
        //public MetaHeader WmfHeader
        //{
        //    get;
        //}

        //// Summary:
        ////     Returns a value that indicates whether the associated System.Drawing.Imaging.Metafile
        ////     is device dependent.
        ////
        //// Returns:
        ////     true if the associated System.Drawing.Imaging.Metafile is device dependent;
        ////     otherwise, false.
        //public bool IsDisplay();
        ////
        //// Summary:
        ////     Returns a value that indicates whether the associated System.Drawing.Imaging.Metafile
        ////     is in the Windows enhanced metafile format.
        ////
        //// Returns:
        ////     true if the associated System.Drawing.Imaging.Metafile is in the Windows
        ////     enhanced metafile format; otherwise, false.
        //public bool IsEmf();
        ////
        //// Summary:
        ////     Returns a value that indicates whether the associated System.Drawing.Imaging.Metafile
        ////     is in the Windows enhanced metafile format or the Windows enhanced metafile
        ////     plus format.
        ////
        //// Returns:
        ////     true if the associated System.Drawing.Imaging.Metafile is in the Windows
        ////     enhanced metafile format or the Windows enhanced metafile plus format; otherwise,
        ////     false.
        //public bool IsEmfOrEmfPlus();
        ////
        //// Summary:
        ////     Returns a value that indicates whether the associated System.Drawing.Imaging.Metafile
        ////     is in the Windows enhanced metafile plus format.
        ////
        //// Returns:
        ////     true if the associated System.Drawing.Imaging.Metafile is in the Windows
        ////     enhanced metafile plus format; otherwise, false.
        //public bool IsEmfPlus();
        ////
        //// Summary:
        ////     Returns a value that indicates whether the associated System.Drawing.Imaging.Metafile
        ////     is in the Dual enhanced metafile format. This format supports both the enhanced
        ////     and the enhanced plus format.
        ////
        //// Returns:
        ////     true if the associated System.Drawing.Imaging.Metafile is in the Dual enhanced
        ////     metafile format; otherwise, false.
        //public bool IsEmfPlusDual();
        ////
        //// Summary:
        ////     Returns a value that indicates whether the associated System.Drawing.Imaging.Metafile
        ////     supports only the Windows enhanced metafile plus format.
        ////
        //// Returns:
        ////     true if the associated System.Drawing.Imaging.Metafile supports only the
        ////     Windows enhanced metafile plus format; otherwise, false.
        //public bool IsEmfPlusOnly();
        ////
        //// Summary:
        ////     Returns a value that indicates whether the associated System.Drawing.Imaging.Metafile
        ////     is in the Windows metafile format.
        ////
        //// Returns:
        ////     true if the associated System.Drawing.Imaging.Metafile is in the Windows
        ////     metafile format; otherwise, false.
        //public bool IsWmf();
        ////
        //// Summary:
        ////     Returns a value that indicates whether the associated System.Drawing.Imaging.Metafile
        ////     is in the Windows placeable metafile format.
        ////
        //// Returns:
        ////     true if the associated System.Drawing.Imaging.Metafile is in the Windows
        ////     placeable metafile format; otherwise, false.
        //public bool IsWmfPlaceable();
    }
}
