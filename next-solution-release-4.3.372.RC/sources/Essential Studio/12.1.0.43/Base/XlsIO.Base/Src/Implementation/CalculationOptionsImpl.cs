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
using System.IO;
using System.Collections;

using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Interfaces;
#endregion

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Worksheet calculation options block.
  /// </summary>
  public class CalculationOptionsImpl
    : CommonObject
    , ICalculationOptions
    , ICloneParent
  {
    #region Class constants
    /// <summary>
    /// Correct records.
    /// </summary>
    public static readonly TBIFFRecord[] DEF_CORRECT_CODES = new TBIFFRecord[]
    {
      TBIFFRecord.CalcMode,
      TBIFFRecord.CalCount,
      TBIFFRecord.RefMode,
      TBIFFRecord.Iteration,
      TBIFFRecord.Delta,
      TBIFFRecord.SaveRecalc,
    };
    #endregion

    #region Class members
    /// <summary>
    /// Specifies whether to calculate formulas manually, automatically, or automatically
    /// except for multiple table operations.
    /// </summary>
    private CalcModeRecord m_calcMode = ( CalcModeRecord )
      BiffRecordFactory.GetRecord( TBIFFRecord.CalcMode );
    /// <summary>
    /// Specifies the maximum number of times the formulas should be iteratively calculated.
    /// This is a fail-safe against mutually recursive formulas locking up
    /// a spreadsheet application.
    /// </summary>
    private CalcCountRecord m_calcCount = ( CalcCountRecord )
      BiffRecordFactory.GetRecord( TBIFFRecord.CalCount );
    /// <summary>
    /// Describes which reference mode to use.
    /// </summary>
    private RefModeRecord m_refMode = ( RefModeRecord )
      BiffRecordFactory.GetRecord( TBIFFRecord.RefMode );
    /// <summary>
    /// Tells whether to iterate over formula calculations or not
    /// (if a formula is dependent upon another formula's result)
    /// (odd feature for something that can only have 32 elements in a formula).
    /// </summary>
    private IterationRecord m_iteration = ( IterationRecord )
      BiffRecordFactory.GetRecord( TBIFFRecord.Iteration );
    /// <summary>
    /// Stores the maximum change of the result to the exit of an iteration.
    /// </summary>
    private DeltaRecord m_delta = ( DeltaRecord )
      BiffRecordFactory.GetRecord( TBIFFRecord.Delta );
    /// <summary>
    /// Defines whether to recalculate before saving (set to 1).
    /// </summary>
    private SaveRecalcRecord m_saveRecalc = ( SaveRecalcRecord )
      BiffRecordFactory.GetRecord( TBIFFRecord.SaveRecalc );
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes new instance and sets application and parent fields.
    /// </summary>
    /// <param name="application">Application object for the page setup.</param>
    /// <param name="parent">Parent object for the page setup.</param>
    public CalculationOptionsImpl( IApplication application, object parent )
      : base( application, parent )
    {
    }
    /// <summary>
    /// Recovers Page setup from the Biff Records array starting from position
    /// </summary>
    /// <param name="application">Application object for the page setup.</param>
    /// <param name="parent">Parent object for the page setup.</param>
    /// <param name="data">Array of Biff Records that contains all needed records.</param>
    /// <param name="iPos">Position of PrintHeadersRecord in the array.</param>
    [ CLSCompliant( false ) ]
    public CalculationOptionsImpl( IApplication application, object parent,
      BiffRecordRaw[] data, int iPos )
      : this( application, parent )
    {
      Parse( data, iPos );
    }
    #endregion

    #region Class parse / serialize methods
    /// <summary>
    /// Recovers Page setup from the Biff Records array starting from the position specified.
    /// </summary>
    /// <param name="data">Biff Records data.</param>
    /// <param name="iPos">Position of first PageSetup record - PrintHeadersRecord.</param>
    /// <returns>Position after extracting calculation options.</returns>
    public int Parse( IList data, int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      int iCount = data.Count;

      if( iPos < 0 || iPos >= iCount )
        throw new ArgumentOutOfRangeException( "iPos", "Value cannot be less than 0 and greater than data.Length" );

      while( iPos < iCount )
      {
        BiffRecordRaw record = ( BiffRecordRaw )data[ iPos ];

        switch( record.TypeCode )
        {
          case TBIFFRecord.CalcMode:
            m_calcMode = ( CalcModeRecord )record;
            break;

          case TBIFFRecord.CalCount:
            m_calcCount = ( CalcCountRecord )record;
            break;

          case TBIFFRecord.RefMode:
            m_refMode = ( RefModeRecord )record;
            break;

          case TBIFFRecord.Iteration:
            m_iteration = ( IterationRecord )record;
            break;

          case TBIFFRecord.Delta:
            m_delta = ( DeltaRecord )record;
            break;

          case TBIFFRecord.SaveRecalc:
            m_saveRecalc = ( SaveRecalcRecord )record;
            break;

          default:
            return iPos;
        }

        iPos++;
      }

      return iPos;
    }

    /// <summary>
    /// Adds all records to OffsetArrayList.
    /// </summary>
    /// <param name="records">OffsetArrayList which will get all records.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      records.Add( m_calcMode.Clone() );
      records.Add( m_calcCount.Clone() );
      records.Add( m_refMode.Clone() );
      records.Add( m_iteration.Clone() );
      records.Add( m_delta.Clone() );
      records.Add( m_saveRecalc.Clone() );
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Specifies the maximum number of times the formulas should be iteratively calculated.
    /// This is a fail-safe against mutually recursive formulas locking up
    /// a spreadsheet application.
    /// </summary>
    public int MaximumIteration
    {
      get
      {
        return m_calcCount.Iterations;
      }
      set
      {
        m_calcCount.Iterations = ( ushort )value;
      }
    }
    /// <summary>
    /// Specifies the mode of formula value calculations.
    /// </summary>
    public ExcelCalculationMode CalculationMode
    {
      get
      {
        return m_calcMode.CalculationMode;
      }
      set
      {
        m_calcMode.CalculationMode = value;
      }
    }
    /// <summary>
    /// Defines whether to recalculate before saving.
    /// </summary>
    public bool RecalcOnSave
    {
      get
      {
        return m_saveRecalc.RecalcOnSave == 1;
      }
      set
      {
        m_saveRecalc.RecalcOnSave = ( ushort )( value ? 1 : 0 );
      }
    }
    /// <summary>
    /// Gets / sets maximum change of the result to the exit of an iteration.
    /// </summary>
    public double MaximumChange
    {
      get
      {
        return m_delta.MaxChange;
      }
      set
      {
        m_delta.MaxChange = value;
      }
    }
    /// <summary>
    /// Indicates whether iterations are turned on.
    /// </summary>
    public bool IsIterationEnabled
    {
      get
      {
        return m_iteration.IsIteration == 1;
      }
      set
      {
        m_iteration.IsIteration = ( ushort )( value ? 1 : 0 );
      }
    }
    /// <summary>
    /// Indicates whether R1C1 reference mode is turned on.
    /// </summary>
    public bool R1C1ReferenceMode
    {
      get
      {
        return m_refMode.IsA1ReferenceMode == 0;
      }
      set
      {
        m_refMode.IsA1ReferenceMode = ( ushort )( value ? 0 : 1 );
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <param name="parent">Parent object for a copy of this instance.</param>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone( object parent )
    {
      CalculationOptionsImpl result = ( CalculationOptionsImpl )MemberwiseClone();
      result.SetParent( parent );

      result.m_calcMode = ( CalcModeRecord )CloneUtils.CloneCloneable( m_calcMode );
      result.m_calcCount = ( CalcCountRecord )CloneUtils.CloneCloneable( m_calcCount );
      result.m_refMode = ( RefModeRecord )CloneUtils.CloneCloneable( m_refMode );
      result.m_iteration = ( IterationRecord )CloneUtils.CloneCloneable( m_iteration );
      result.m_delta = ( DeltaRecord )CloneUtils.CloneCloneable( m_delta );
      result.m_saveRecalc = ( SaveRecalcRecord )CloneUtils.CloneCloneable( m_saveRecalc );

      return result;
    }
    #endregion
  }
}
