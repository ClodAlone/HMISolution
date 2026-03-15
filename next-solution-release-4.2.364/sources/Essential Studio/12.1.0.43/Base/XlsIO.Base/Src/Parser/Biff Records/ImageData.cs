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

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
  /// <summary>
  /// This record contains the complete description of a bitmapped graphic object,
  /// such as a drawing created by a graphic tool.
  /// </summary>
  [ Biff( TBIFFRecord.ImageData ) ]
  class ImageDataRecord : BiffRecordWithContinue
  {
    #region Overrides
    /// <summary>
    /// Parse structure of record. Convert Data buffer to special
    /// values according to record specification.
    /// </summary>
    public override void ParseStructure()
    {
    }
    /// <summary>
    /// In this method, the class must pack all of its properties into
    /// an internal Data array: m_data. This method is called by
    /// FillStream, when the record must be serialized into stream.
    /// </summary>
    public override void InfillInternalData( ExcelVersion version )
    {
    }
    #endregion
  }
}
