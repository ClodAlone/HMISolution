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
using System.Collections;

using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using Syncfusion.XlsIO.Parser;
using System.Collections.Generic;
#endregion

namespace Syncfusion.XlsIO.Implementation.Charts
{
	/// <summary>
	/// Class used for Chart Legend entries implementation.
	/// </summary>
	public class ChartLegendEntryImpl
    : CommonObject
    , IChartLegendEntry
	{
    #region Class members
    /// <summary>
    /// Represents LegendXN record.
    /// </summary>
    private ChartLegendxnRecord m_legendXN;
    /// <summary>
    /// Represents text of legend entry.
    /// </summary>
    private ChartTextAreaImpl m_text;
    /// <summary>
    /// Represents parent chart legend entry collection.
    /// </summary>
    private ChartLegendEntriesColl m_legendEnties;
    /// <summary>
    /// Represents index of legend entry in collection.
    /// </summary>
    private int m_index;
    #endregion

    #region Class constructors
    /// <summary>
    /// Creates new instance of legend entry.
    /// </summary>
    /// <param name="application">Application object.</param>
    /// <param name="parent">Parent object.</param>
    /// <param name="iIndex">Represents index in collection</param>
		public ChartLegendEntryImpl( IApplication application, object parent, int iIndex )
      : base( application, parent )
		{
      m_legendXN = ( ChartLegendxnRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ChartLegendxn );
			m_text = new ChartTextAreaImpl( application, this );
      m_index = iIndex;

      SetParents();
		}
    /// <summary>
    /// Creates new instance of legend entry by parsing from stream.
    /// </summary>
    /// <param name="application">Application object.</param>
    /// <param name="parent">Parent object.</param>
    /// <param name="iIndex">Represents index in collection.</param>
    /// <param name="data">Represents record holder.</param>
    /// <param name="iPos">Represents position in stream.</param>
    public ChartLegendEntryImpl( IApplication application, object parent
      , int iIndex, IList<BiffRecordRaw> data, ref int iPos )
      : base( application, parent )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      m_index = iIndex;
      SetParents();

      Parse( data, ref iPos );
    }
    #endregion

    #region Class parse methods
    /// <summary>
    /// Parses legend entry.
    /// </summary>
    /// <param name="data">Record storage.</param>
    /// <param name="iPos">Position in storage.</param>
    public void Parse( IList<BiffRecordRaw> data, ref int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      BiffRecordRaw record = ( BiffRecordRaw )data[ iPos ];
      record.CheckTypeCode( TBIFFRecord.ChartLegendxn );

      m_legendXN = ( ChartLegendxnRecord )data[ iPos ];
      iPos++;

      record = ( BiffRecordRaw )data[ iPos ];
      
      if( record.TypeCode != TBIFFRecord.Begin )
        return;

      iPos++;
      int count = 1;

      while( count != 0 )
      {
        record = ( BiffRecordRaw )data[ iPos ];

        switch( record.TypeCode )
        {
          case TBIFFRecord.Begin:
            iPos = BiffRecordRaw.SkipBeginEndBlock( data, iPos );
            break;

          case TBIFFRecord.End:
            count--;
            break;

          case TBIFFRecord.ChartText:
            m_text = new ChartTextAreaImpl( Application, this );
            iPos = m_text.Parse( data, iPos ) - 1;
            break;

          default:
            //throw new ApplicationException( "Unknown record." );
            // We should skip this record since MS Excel 2007 adds some additional record to this structure.
            break;
        }

        iPos++;
      }
    }
    /// <summary>
    /// Finds parent object for collection.
    /// </summary>
    public void SetParents()
    {
      m_legendEnties = ( ChartLegendEntriesColl )FindParent( typeof( ChartLegendEntriesColl ) );

      if( m_legendEnties == null )
        throw new ArgumentNullException( "cannot find parent object" );
    }
    #endregion

    #region Class serialize methods
    /// <summary>
    /// Serialize legend entry object.
    /// </summary>
    /// <param name="records">Record storage.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( IList<IBiffStorage> records )
    {
      if( records == null )
        throw new ArgumentException( "records" );

      if( !IsFormatted && !IsDeleted )
        return;

      records.Add( ( BiffRecordRaw )m_legendXN.Clone() );

      if( m_text != null )
      {
        records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.Begin ) );
        m_text.Serialize( records, true );
        records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.End ) );
      }
    }
    #endregion

    #region Class properties
    /// <summary>
    /// If true then this entry deleted. otherwise false.
    /// </summary>
    public bool IsDeleted
    {
      get
      {
        return m_legendXN.IsDeleted;
      }
      set
      {
        if( IsDeleted != value )
        {
          if( value )
          {
            if( !m_legendEnties.CanDelete( m_index ) && !m_text.ParentWorkbook.Loading )
              throw new ApplicationException( "cannot delete last legend entry in chart" );

            IsFormatted = !value;
          }

          m_legendXN.IsDeleted = value;
        }
      }
    }
    /// <summary>
    /// True if the legend entry has been formatted.
    /// </summary>
    public bool IsFormatted
    {
      get
      {
        return m_legendXN.IsFormatted;
      }
      set
      {
        if( value != IsFormatted )
        {
          if( value )
          {
            m_text = new ChartTextAreaImpl( Application, this );
            m_legendXN.IsDeleted = false;
          }

          m_legendXN.IsFormatted = value;
        }
      }
    }
    /// <summary>
    /// Returns text area. Read-only.
    /// </summary>
    public IChartTextArea TextArea
    {
      get
      {
        m_legendXN.IsDeleted = false;
        m_legendXN.IsFormatted = true;

        return m_text;
      }
    }
    /// <summary>
    /// Legend-entry index.
    /// </summary>
    public int LegendEntityIndex
    {
      get
      {
        return m_legendXN.LegendEntityIndex;
      }
      set
      {
        m_legendXN.LegendEntityIndex = ( ushort )value;
      }
    }
    /// <summary>
    /// Represents index in collection.
    /// </summary>
    public int Index
    {
      get
      {
        return m_index;
      }
      set
      {
        m_index = value;
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Clears current data point
    /// </summary>
    public void Clear()
    {
      IsFormatted = false;
    }
    /// <summary>
    /// Deletes current legend entry.
    /// </summary>
    public void Delete()
    {
      IsDeleted = true;
    }
    /// <summary>
    /// Clones current object.
    /// </summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="dicIndexes">Dictionary with new font indexes.</param>
    /// <param name="dicNewSheetNames">Dictionary with new worksheet names.</param>
    /// <returns>Returns cloned object.</returns>
    public ChartLegendEntryImpl Clone( object parent, Dictionary<int, int> dicIndexes, Dictionary<string, string> dicNewSheetNames )
    {
      ChartLegendEntryImpl result = ( ChartLegendEntryImpl )MemberwiseClone();
      result.SetParent( parent );
      result.SetParents();

      if( m_text != null )
        result.m_text = ( ChartTextAreaImpl )m_text.Clone( result, dicIndexes, dicNewSheetNames );

      result.m_legendXN = ( ChartLegendxnRecord )CloneUtils.CloneCloneable( m_legendXN );

      return result;
    }
    #endregion
	}
}
