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

namespace Syncfusion.XlsIO.Parser.Biff_Records.ObjRecords
{
  /// <summary>
  /// To store an OBJ record in BIFF8, Microsoft Excel writes a collection
  /// of sub-records. The structure of a sub-record is identical to the
  /// structure of a BIFF record. Each sub-record begins with a 2-byte
  /// ID number ft (see the following table). Next a 2-byte length field,
  /// cb, specifies the length of the sub-record data field. The sub-record
  /// data field follows the length field. The first sub-record is always
  /// ftCmo (common object data) and the last sub-record is always ftEnd.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public enum TObjSubRecordType
  {
    /// <summary>
    /// Represents the ftEnd subrecord type.
    /// </summary>
    ftEnd = 0x00,
    /// <summary>
    /// Represents the Reserved0 subrecord type.
    /// </summary>
    Reserved0 = 0x01,
    /// <summary>
    /// Represents the Reserved1 subrecord type.
    /// </summary>
    Reserved1 = 0x02,
    /// <summary>
    /// Represents the Reserved2 subrecord type.
    /// </summary>
    Reserved2 = 0x03,
    /// <summary>
    /// Represents the ftMacro subrecord type.
    /// </summary>
    ftMacro = 0x04,
    /// <summary>
    /// Represents the ftButton subrecord type.
    /// </summary>
    ftButton = 0x05,
    /// <summary>
    /// Represents the ftGmo subrecord type.
    /// </summary>
    ftGmo = 0x06,
    /// <summary>
    /// Represents the ftCf subrecord type.
    /// </summary>
    ftCf = 0x07,
    /// <summary>
    /// Represents the ftPioGrbit subrecord type.
    /// </summary>
    ftPioGrbit = 0x08,
    /// <summary>
    /// Represents the ftPictFmla subrecord type.
    /// </summary>
    ftPictFmla = 0x09,
    /// <summary>
    /// Represents the ftCbls subrecord type.
    /// </summary>
    ftCbls = 0x0A,
    /// <summary>
    /// Represents the ftRbo subrecord type.
    /// </summary>
    ftRbo = 0x0B,
    /// <summary>
    /// Represents the ftSbs subrecord type.
    /// </summary>
    ftSbs = 0x0C,
    /// <summary>
    /// Represents the ftNts subrecord type.
    /// </summary>
    ftNts = 0x0D,
    /// <summary>
    /// Represents the ftSbsFmla subrecord type.
    /// </summary>
    ftSbsFormula = 0x0E,
    /// <summary>
    /// Represents the ftGboData subrecord type.
    /// </summary>
    ftGboData = 0x0F,
    /// <summary>
    /// Represents the ftEdoData subrecord type.
    /// </summary>
    ftEdoData = 0x10,
    /// <summary>
    /// Represents the ftRboData subrecord type.
    /// </summary>
    ftRboData = 0x11,
    /// <summary>
    /// Represents the ftCblsData subrecord type.
    /// </summary>
    ftCblsData = 0x12,
    /// <summary>
    /// Represents the ftLbsData subrecord type.
    /// </summary>
    ftLbsData = 0x13,
    /// <summary>
    /// Represents the ftCblsFmla subrecord type.
    /// </summary>
    ftCblsFmla = 0x14,
    /// <summary>
    /// Represents the ftCmo subrecord type.
    /// </summary>
    ftCmo = 0x15,
  }
  /// <summary>
  /// Possible object types:
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public enum TObjType
  {
    /// <summary>
    /// Represents the otGroup object type.
    /// </summary>
    otGroup = 0x00,
    /// <summary>
    /// Represents the otLine object type.
    /// </summary>
    otLine = 0x01,
    /// <summary>
    /// Represents the otRectangle object type.
    /// </summary>
    otRectangle = 0x02,
    /// <summary>
    /// Represents the otOval object type.
    /// </summary>
    otOval = 0x03,
    /// <summary>
    /// Represents the otArc object type.
    /// </summary>
    otArc = 0x04,
    /// <summary>
    /// Represents the otChart object type.
    /// </summary>
    otChart = 0x05,
    /// <summary>
    /// Represents the otText object type.
    /// </summary>
    otText = 0x06,
    /// <summary>
    /// Represents the otButton object type.
    /// </summary>
    otButton = 0x07,
    /// <summary>
    /// Represents the otPicture object type.
    /// </summary>
    otPicture = 0x08,
    /// <summary>
    /// Represents the otPolygon object type.
    /// </summary>
    otPolygon = 0x09,
    /// <summary>
    /// Represents the otReserved0 object type.
    /// </summary>
    otReserved0 = 0x0A,
    /// <summary>
    /// Represents the otCheckBox object type.
    /// </summary>
    otCheckBox = 0x0B,
    /// <summary>
    /// Represents the otOptionBtn object type.
    /// </summary>
    otOptionBtn = 0x0C,
    /// <summary>
    /// Represents the otEditBox object type.
    /// </summary>
    otEditBox = 0x0D,
    /// <summary>
    /// Represents the otLabel object type.
    /// </summary>
    otLabel = 0x0E,
    /// <summary>
    /// Represents the otDialogBox object type.
    /// </summary>
    otDialogBox = 0x0F,
    /// <summary>
    /// Represents the otSpinner object type.
    /// </summary>
    otSpinner = 0x10,
    /// <summary>
    /// Represents the otScrollBar object type.
    /// </summary>
    otScrollBar = 0x11,
    /// <summary>
    /// Represents the otGroupBox object type.
    /// </summary>
    otListBox = 0x12,
    /// <summary>
    /// Represents the otGroupBox object type.
    /// </summary>
    otGroupBox = 0x13,
    /// <summary>
    /// Represents the otComboBox object type.
    /// </summary>
    otComboBox = 0x14,
    /// <summary>
    /// Represents the otReserved1 object type.
    /// </summary>
    otReserved1 = 0x15,
    /// <summary>
    /// Represents the otReserved2 object type.
    /// </summary>
    otReserved2 = 0x16,
    /// <summary>
    /// Represents the otReserved3 object type.
    /// </summary>
    otReserved3 = 0x17,
    /// <summary>
    /// Represents the otReserved4 object type.
    /// </summary>
    otReserved4 = 0x18,
    /// <summary>
    /// Represents the otComment object type.
    /// </summary>
    otComment = 0x19,
    /// <summary>
    /// Represents the otReserved5 object type.
    /// </summary>
    otReserved5 = 0x1A,
    /// <summary>
    /// Represents the otReserved6 object type.
    /// </summary>
    otReserved6 = 0x1B,
    /// <summary>
    /// Represents the otReserved7 object type.
    /// </summary>
    otReserved7 = 0x1C,
    /// <summary>
    /// Represents the otReserved8 object type.
    /// </summary>
    otReserved8 = 0x1D,
    /// <summary>
    /// Represents the otMSODrawing object type.
    /// </summary>
    otMSODrawing = 0x1E,
  }
}
