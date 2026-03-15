#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region file using directives
using System;

using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser;
#endregion

namespace Syncfusion.XlsIO.Implementation.Charts
{
	/// <summary>
	/// Summary description for ChartAxisImpl.
	/// </summary>
	public class ChartAxisImpl : CommonObject
	{
    #region Class members
    private ChartAxisRecord m_chartAxis;
    #endregion

    #region Class constructors
    /// <summary>
    /// 
    /// </summary>
		public ChartAxisImpl( IApplication application, object parent )
      : base( application, parent )
		{
		}
    /// <summary>
    /// 
    /// </summary>
    public ChartAxisImpl( IApplication application, object parent, 
      BiffRecordRaw[] data, ref int iPos )
      : this( application, parent )
    {
      Parse( data, ref iPos );
    }
    #endregion

    #region Parse methods
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="iPos"></param>
    public void Parse( BiffRecordRaw[] data, ref int iPos )
    {
      if( data[ iPos ].TypeCode != TBIFFRecord.ChartAxis )
        throw new ArgumentOutOfRangeException( "ChartAxisRecord was expected" );

      m_chartAxis = (ChartAxisRecord) data[ iPos ].Clone();
      iPos++;

      if( data[ iPos ].TypeCode != TBIFFRecord.Begin )
        throw new ArgumentOutOfRangeException( "BeginRecord was expected" );

      iPos++;

      while( data[ iPos ].TypeCode != TBIFFRecord.End )
      {
        iPos++;
      }
    }
    #endregion
	}
}
