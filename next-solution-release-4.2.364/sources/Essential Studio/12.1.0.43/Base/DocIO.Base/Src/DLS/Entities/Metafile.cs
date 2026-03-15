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
//using System.Windows.Controls;
//using System.Windows.Documents;
//using System.Windows.Ink;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Animation;
//using System.Windows.Shapes;
using Syncfusion.DocIO;
using System.IO;

namespace Syncfusion.DocIO.DLS.Entities
{
  internal sealed class Metafile : Image
  {
    #region Constructors
    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="stream"></param>
    ///// <param name="referenceHdc"></param>
    ///// <param name="type"></param>
    //public Metafile( Stream stream, IntPtr referenceHdc, EmfType type )
    //{
    //}
    /// <summary>
    /// 
    /// </summary>
    /// <param name="stream"></param>
    public Metafile( Stream stream )
    {
    }
    #endregion

    #region Methods
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public MetafileHeader GetMetafileHeader()
    {
      return null;
    }
    #endregion
  }
}
