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

using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Interfaces;
using System.Collections.Generic;

#if ( WINRT )
using Windows.UI;
using Rectangle=Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.Silverlight.Rectangle;
#elif WP
using System.Windows.Media;
using Rectangle = Syncfusion.XlsIO.Implementation.WP.Rectangle;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif
#endregion

namespace Syncfusion.XlsIO.Implementation.Collections
{
	/// <summary>
	/// Summary description for WorksheetConditionalFormats.
	/// </summary>
	public class WorksheetConditionalFormats
    : CollectionBaseEx<ConditionalFormats>
    , ICloneParent
	{
    #region Class members
    /// <summary>
    /// Dictionary with all ConditionalFormats. Used to check whether format is unique.
    /// </summary>
    private Dictionary<ConditionalFormats, ConditionalFormats> m_hash =
      new Dictionary<ConditionalFormats, ConditionalFormats>();
    /// <summary>
    /// List of CFEx Records.
    /// </summary>
    internal Dictionary<int, CFExRecord> m_arrCFExRecords = new Dictionary<int, CFExRecord>();
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes new instance of the collection.
    /// </summary>
    /// <param name="application">Application object for the new collection.</param>
    /// <param name="parent">Parent object for the new collection.</param>
    public WorksheetConditionalFormats( IApplication application, object parent )
      : base( application, parent )
    {
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Search for collection that contains all specified ranges.
    /// </summary>
    /// <param name="arrRanges"></param>
    /// <returns></returns>
    public ConditionalFormats Find( Rectangle[] arrRanges )
    {
      ConditionalFormats resultFormats = new ConditionalFormats(this.Application, this);
      if( arrRanges == null ) return null;

      int iCount = arrRanges.Length;

      if( iCount == 0 ) return null;

      for( int i = 0, len = Count; i < len; i++ )
      {
        ConditionalFormats formats = this[ i ];

        if( formats.Contains( arrRanges ) )
        {
            resultFormats = AddCF(resultFormats, formats);
        }
      }
      if (resultFormats.Count == 0)
          return null;
      else
          return resultFormats;
    }
    /// <summary>
    /// Add the CF to output CF
    /// </summary>
    /// <param name="outputCF"></param>
    /// <param name="AddCf"></param>
    /// <returns></returns>
    private ConditionalFormats AddCF(ConditionalFormats outputCF, ConditionalFormats AddCf)
    {
        ConditionalFormats result = outputCF;
        for (int i = 0; i < AddCf.Count; i++)
        {
            outputCF.Add(AddCf[i]);
        }
        return result;
    }
    /// <summary>
    /// Defines whether collection contains conditional format.
    /// </summary>
    /// <param name="formats">Conditional formats.</param>
    /// <returns>Conditional formats if exists or null.</returns>
    public ConditionalFormats Contains( ConditionalFormats formats )
    {
      if( formats == null )
        throw new ArgumentNullException( "formats" );

      ConditionalFormats curFormats;
      m_hash.TryGetValue( formats, out curFormats );

      return curFormats;
    }
    /// <summary>
    /// Adds new item to the collection.
    /// </summary>
    /// <param name="formats">Item to add.</param>
    /// <returns>Object that contains required cells.</returns>
    public ConditionalFormats Add( ConditionalFormats formats )
    {
      if( formats == null )
        throw new ArgumentNullException( "formats" );

      ConditionalFormats curFormats;

      if( m_hash.TryGetValue( formats, out curFormats ) )
      {
          if(curFormats.CondFMTRecord!=null)
              curFormats.AddCells( formats );
          else
              curFormats.AddCellsCondFMT12(formats);
      }
      else
      {
        base.Add( formats );
        m_hash.Add( formats, formats );
      }

      return ( curFormats != null ) ? curFormats : formats;
    }
    /// <summary>
    /// Removes range from the collection of conditional formats.
    /// </summary>
    /// <param name="arrRanges">Array of ranges to remove.</param>
    public void Remove( Rectangle[] arrRanges )
    {
      if( arrRanges == null || arrRanges.Length == 0 ) return;

      int iRemoveCount = 0;
      List<ConditionalFormats> arrList = InnerList;

      for( int i = 0, len = Count; i < len; i++ )
      {
        ConditionalFormats formats = this[ i ];
        formats.Remove( arrRanges );

        if( formats.IsEmpty )
        {
          int iNewIndex = len - 1;

          if( iNewIndex != i )
          {
            ConditionalFormats oldValue = arrList[ iNewIndex ];
            arrList[ iNewIndex ] = formats;
            arrList[ i ] = oldValue;
          }

          iRemoveCount++;
          len--;
          i--;
          m_hash.Remove( formats );
        }
      }

      if( iRemoveCount > 0 )
        arrList.RemoveRange( Count - iRemoveCount, iRemoveCount );
    }
    /// <summary>
    /// Copies conditional formats from another formats collection.
    /// </summary>
    /// <param name="arrSourceFormats">Source collection of conditional formats to copy.</param>
    public void CopyFrom( WorksheetConditionalFormats arrSourceFormats )
    {
      if( arrSourceFormats == null )
        throw new ArgumentNullException( "arrSourceFormats" );

      for( int i = 0, len = arrSourceFormats.Count; i < len; i++ )
      {
        ConditionalFormats sourceFormats = ( ConditionalFormats )arrSourceFormats[ i ];
        ConditionalFormats formats = new ConditionalFormats(Application, this, sourceFormats, true);
        Add( formats );
      }
    }
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <param name="parent">Parent object for a copy of this instance.</param>
    /// <returns>A new object that is a copy of this instance.</returns>
    public override object Clone( object parent )
    {
      WorksheetConditionalFormats result = ( WorksheetConditionalFormats )base.Clone( parent );
      result.m_hash = CloneUtils.CloneHash( m_hash, result );
      return result;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="formats"></param>
    public void RemoveItem( ConditionalFormats formats )
    {
      base.Remove( formats );
      m_hash.Remove( formats );
    }
    /// <summary>
    /// Sets items with used reference indexes to true.
    /// </summary>
    /// <param name="usedItems">Array to mark used references in.</param>
    public void MarkUsedReferences( bool[] usedItems )
    {
      List<ConditionalFormats> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        ConditionalFormats formats = list[ i ];
        formats.MarkUsedReferences( usedItems );
      }
    }
    /// <summary>
    /// Updates reference indexes.
    /// </summary>
    /// <param name="arrUpdatedIndexes">Array with updated indexes.</param>
    public void UpdateReferenceIndexes( int[] arrUpdatedIndexes )
    {
      List<ConditionalFormats> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        ConditionalFormats formats = list[ i ];
        formats.UpdateReferenceIndexes( arrUpdatedIndexes );
      }
    }
    #endregion

    #region Class parse / serialization methods
    /// <summary>
    /// Serializes all conditional formats as Biff records.
    /// </summary>
    /// <param name="records">OffsetArrayList to serialize records into.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      int priority = 1;
      ushort index = 1;

      for( int i = 0, len = Count; i < len; i++ )
      {
          priority = this[i].Serialize(records, index, priority);
          m_arrCFExRecords = this[i].sheet.m_dictCFExRecords;
          index++;
      }

      for (int i =0;i< m_arrCFExRecords.Count; i++)
      {
          if (m_arrCFExRecords[i].IsCF12Extends != 1)
              records.Add(m_arrCFExRecords[i]);
          else
          {
              CFExRecord cfex = m_arrCFExRecords[i];
              records.Add(cfex);
              records.Add(cfex.CF12RecordIfExtends);
          }
      }

      m_arrCFExRecords.Clear();
    }
    /// <summary>
    /// Updates conditional format formulas.
    /// </summary>
    /// <param name="iCurIndex">Current worksheet index.</param>
    /// <param name="iSourceIndex">Source worksheet index.</param>
    /// <param name="sourceRect">Source rectangle.</param>
    /// <param name="iDestIndex">Destination worksheet index.</param>
    /// <param name="destRect">Destination rectangle.</param>
    public void UpdateFormula( int iCurIndex, int iSourceIndex, Rectangle sourceRect,
      int iDestIndex, Rectangle destRect )
    {
      for( int i = 0, len = Count; i < len; i++ )
      {
        this[ i ].UpdateFormula( iCurIndex, iSourceIndex, sourceRect, iDestIndex, destRect );
      }
    }
    #endregion
  }
}
