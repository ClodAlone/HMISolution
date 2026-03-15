#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region file using directives
using System;

using Syncfusion.XlsIO.Parser.Biff_Records;
#endregion

namespace Syncfusion.XlsIO.Implementation
{
	/// <summary>
	/// Summary description for CellDataImpl.
	/// </summary>
	public class CellDataImpl
	{
    #region Class members
    /// <summary>
    /// Cell range.
    /// </summary>
    private RangeImpl m_range;
    /// <summary>
    /// Cell record.
    /// </summary>
    private ICellPositionFormat m_record;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public CellDataImpl()
    {
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Cell range.
    /// </summary>
    public RangeImpl Range
    {
      get
      {
        return m_range;
      }
      set
      {
        m_range = value;
      }
    }
    /// <summary>
    /// Cell record.
    /// </summary>
    [ CLSCompliant( false ) ]
    public ICellPositionFormat Record
    {
      get
      {
        return m_record;
      }
      set
      {
        m_record = value;
      }
    }
    #endregion
  }
}
