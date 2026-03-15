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

namespace Syncfusion.XlsIO.Implementation
{
  class ExtendedFormatStandAlone : ExtendedFormatImpl
  {
    #region Members
    /// <summary>
    /// Font settings.
    /// </summary>
    private FontImpl font;
    #endregion

    #region Members
    /// <summary>
    /// Initializes new instance of the format.
    /// </summary>
    /// <param name="format">Object to copy settings from.</param>
    public ExtendedFormatStandAlone( ExtendedFormatImpl format )
      : base( format.AppImplementation, format.Parent )
    {
      format.CopyTo( this );
      font = AppImplementation.CreateFont( format.Font );
      FontIndex = -1;

      InitializeColors();
      CopyColorsFrom( format );
    }
    #endregion

    #region Properties
    /// <summary>
    /// Returns object that stores font settings. Read-only.
    /// </summary>
    public override IFont Font
    {
      get
      {
        return font;
      }
    }
    #endregion
  }
}
