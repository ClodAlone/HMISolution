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
using System.Runtime.InteropServices;

#if DOCIO
namespace Syncfusion.CompoundFile.DocIO
#else
namespace Syncfusion.CompoundFile.XlsIO
#endif
{
  // Summary:
  //     Supports cloning, which creates a new instance of a class with the same value
  //     as an existing instance.
  [ComVisible( true )]
  public interface IDocIOCloneable
  {
    // Summary:
    //     Creates a new object that is a copy of the current instance.
    //
    // Returns:
    //     A new object that is a copy of this instance.
    object Clone();
  }
}
