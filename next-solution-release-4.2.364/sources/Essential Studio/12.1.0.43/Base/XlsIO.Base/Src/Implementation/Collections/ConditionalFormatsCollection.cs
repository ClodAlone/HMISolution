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
using System.Collections.Generic;

using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser;

#if ( WINRT )
using Windows.UI;
using Rectangle = Syncfusion.XlsIO.Implementation.WINRT.Rectangle;
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
  /// Collection of conditional formats for the single-cell range.
  /// </summary>
  public class ConditionalFormats
    : CollectionBaseEx<IConditionalFormat>
    , IConditionalFormats
  {
    #region Class constants
    /// <summary>
    /// Maximum number of conditional formats in the collection.
    /// </summary>
    public const int MAXIMUM_CF_NUMBER = 3;
    #endregion

    #region Class members
    /// <summary>
    /// Record that contains collection data.
    /// </summary>
    private CondFMTRecord m_format;
    /// CondFmt12Record that contains collection data.
    /// </summary>
    private CondFmt12Record m_condFMT12;
    /// <summary>
    /// <summary>
    /// Parent worksheet.
    /// </summary>
    private WorksheetImpl m_sheet;
    /// <summary>
    /// Ranges operations.
    /// </summary>
    private RangesOperations m_rangesOperations;
    /// <summary>
    /// Normal rule count.
    /// </summary>
    private int m_cfCount = 0;
    /// <summary>
    /// Advanced conditional format rule count.
    /// </summary>
    private int m_acfCount = 0;
    /// <summary>
    /// Total rule count.
    /// </summary>
    private int m_totalCFCount = 0;
    /// <summary>
    /// Index of the rule.
    /// </summary>
    private int m_cfIndex = 0;
    /// <summary>
    /// Specifies the future record.
    /// </summary>
    private bool m_futureRecord = false;
    #endregion

    /// <summary>
    /// Get worksheet. Read only.
    /// </summary>
    public WorksheetImpl sheet
    {
        get
        {
            return m_sheet;
        }
    }

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new instance.
    /// </summary>
    /// <param name="application">Base application.</param>
    /// <param name="parent">Parent object.</param>
    public ConditionalFormats( IApplication application, object parent )
      : base( application, parent )
    {
      m_format = ( CondFMTRecord )BiffRecordFactory.GetRecord( TBIFFRecord.CondFMT );
      m_condFMT12 = (CondFmt12Record)BiffRecordFactory.GetRecord(TBIFFRecord.CondFMT12);

      FindParent();

      m_rangesOperations = new RangesOperations( m_format.CellList );
    }
    /// <summary>
    /// Creates new instance.
    /// </summary>
    /// <param name="application">Base application.</param>
    /// <param name="parent">Parent object.</param>
    /// <param name="toClone">Collection to clone.</param>
    public ConditionalFormats( IApplication application, object parent
      , ConditionalFormats toClone )
      : base( application, parent )
    {
      if( toClone == null )
        return;

      if (toClone.m_format != null)
      {
        m_format = ( CondFMTRecord )toClone.m_format.Clone();
        m_rangesOperations = new RangesOperations(m_format.CellList);
      }

      if (toClone.m_condFMT12 != null)
      {
          m_condFMT12 = (CondFmt12Record)toClone.m_condFMT12.Clone();
          m_rangesOperations = new RangesOperations(m_condFMT12.CellList);
      }

      for( int i = 0, iLen = toClone.Count; i < iLen; i++ )
      {
        ConditionalFormatImpl entryToClone = ( ConditionalFormatImpl )toClone.List[ i ];

        if( entryToClone != null )
        {
          object o = entryToClone.Clone( this );
          base.Add( o as IConditionalFormat );
        }
      }

      FindParent();      
    }
    /// <summary>
    /// Copy Conditional format.
    /// </summary>
    /// <param name="application">Base application.</param>
    /// <param name="parent">Parent object.</param>
    /// <param name="toClone">Collection to clone.</param>
    /// <param name="toClone">boolean value to copy conditional format</param>
    public ConditionalFormats(IApplication application, object parent
      , ConditionalFormats toClone,bool bCopy)
        : base(application, parent)
    {
        if (toClone == null)
            return;

        if (bCopy)
        {
            if (toClone.m_format != null && toClone.m_format.CellList.Count > 0)
            {
                m_format = (CondFMTRecord)toClone.m_format.Clone();
                m_rangesOperations = new RangesOperations(m_format.CellList);
            }

            if (toClone.m_condFMT12 != null && toClone.m_condFMT12.CellList.Count > 0)
            {
                m_condFMT12 = (CondFmt12Record)toClone.m_condFMT12.Clone();
                m_rangesOperations = new RangesOperations(m_condFMT12.CellList);
            }

            for (int i = 0, iLen = toClone.Count; i < iLen; i++)
            {
                ConditionalFormatImpl entryToClone = (ConditionalFormatImpl)toClone.List[i];

                if (entryToClone != null)
                {
                    object o = entryToClone.Clone(this);
                    base.Add(o as IConditionalFormat);
                }
            }

            FindParent();
        }
    }
    /// <summary>
    /// Creates ConditionalFormats collection from array of BiffRecords.
    /// </summary>
    /// <param name="application">Application object for the collection.</param>
    /// <param name="parent">Parent object for the collection.</param>
    /// <param name="format">Main conditional format record.</param>
    /// <param name="formats">Array of CFRecords with conditional formats.</param>
    [CLSCompliant( false )]
    public ConditionalFormats(IApplication application, object parent, CondFMTRecord format, IList formats, IList CFExRecords)
      : base( application, parent )
    {
      if( format == null )
        throw new ArgumentNullException( "format" );

      if( formats == null )
        throw new ArgumentNullException( "formats" );

      m_format = format;

      if (formats.Count > 0)
      {
          for (int i = 0, len = formats.Count; i < len; i++)
          {
              // Use 'as' to increase performance.
              CFRecord cf = formats[i] as CFRecord;
              AddFromRecord(cf);
          }
      }

      if (CFExRecords.Count > 0)
      {
          for (int i = 0, len = CFExRecords.Count; i < len; i++)
          {
              // Use 'as' to increase performance.
              CFExRecord cfEx = CFExRecords[i] as CFExRecord;
              AddFromCFEXRecord(cfEx);
          }
      }

      FindParent();

      m_rangesOperations = new RangesOperations( m_format.CellList );
    }
    /// <summary>
    /// Creates ConditionalFormats collection from array of BiffRecords.
    /// </summary>
    /// <param name="application">Application object for the collection.</param>
    /// <param name="parent">Parent object for the collection.</param>
    /// <param name="format">Main conditional format12 record.</param>
    /// <param name="formats">Array of CF12Records with conditional formats.</param>
    [CLSCompliant(false)]
    public ConditionalFormats(IApplication application, object parent, CondFmt12Record format, IList formats)
        : base(application, parent)
    {
        if (format == null)
            throw new ArgumentNullException("format");

        if (formats == null)
            throw new ArgumentNullException("formats");

        m_condFMT12 = format;

        for (int i = 0, len = formats.Count; i < len; i++)
        {
            // Use 'as' to increase performance.
            CF12Record cf = formats[i] as CF12Record;
            AddFromCF12Record(cf);
        }

        FindParent();
        m_rangesOperations= new RangesOperations(m_condFMT12.CellList);
    }
    /// <summary>
    /// Finds parent worksheet.
    /// </summary>
    private void FindParent()
    {
      m_sheet = FindParent( typeof( WorksheetImpl ) ) as WorksheetImpl;

      if( m_sheet == null )
        throw new ArgumentNullException( "Parent worksheet." );
    }
    #endregion

    #region IConditionalFormats Members
    /// <summary>
    /// Returns maximum conditional formats number. Depends on current Excel version.
    /// </summary>
    private int MaxCFNumber
    {
      get
      {
        //return ( m_sheet.Version == ExcelVersion.Excel97to2003 ) ? MAXIMUM_CF_NUMBER : int.MaxValue;
          return int.MaxValue;
      }
    }
    /// <summary>
    /// Adds new condition to the collection.
    /// </summary>
    /// <returns></returns>
    public IConditionalFormat AddCondition()
    {
      if( Count >= MaxCFNumber )
        throw new ArgumentOutOfRangeException( "Too many conditional formats." );

      ConditionalFormatImpl result = new ConditionalFormatImpl( Application, this );
      base.Add( result );

      return result;
    }
    /// <summary>
    /// Removes the Condtional Format at the specified range
    /// </summary>
    public void Remove()
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Removes the Condtional Format at the Specified Index
    /// </summary>
    public void RemoveAt()
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Adds condition from CFRecord.
    /// </summary>
    private void CopyAdvancedConditionalFormatting(ConditionalFormatImpl format)
    {
        format.CF12Record.FormatType = format.FormatType;            
        format.CF12Record.Criteria = format.ColorScale;
        format.CF12Record.DataBarImpl = format.DataBar;
        format.CF12Record.IconSetImpl = format.IconSet;
        format.CF12Record.StopIfTrue = format.StopIfTrue;
    }
    #endregion

    #region Class Serialize methods
    /// <summary>
    /// Serializes collection data into OffsetArrayList.
    /// </summary>
    /// <param name="records">OffsetArrayList that will get all data.</param>
    [CLSCompliant( false )]
    public int Serialize(OffsetArrayList records, ushort index, int priority)
    {
      int iCount = Count;

      if( iCount <= 0 || m_rangesOperations.CellList.Count == 0 )
        return iCount;

      m_cfCount = 0;
      m_acfCount = 0;      

      for (int i = 0; i < iCount; i++)
      {
          ConditionalFormatImpl formatimpl = (ConditionalFormatImpl)InnerList[i];

          if ((formatimpl.CF12Record.IsParsed) || ((!formatimpl.CF12Record.IsParsed) && (formatimpl.FormatType == ExcelCFType.ColorScale || formatimpl.FormatType == ExcelCFType.DataBar || formatimpl.FormatType == ExcelCFType.IconSet)))
              m_acfCount++;
          else
              m_cfCount++;
      }
      m_totalCFCount = m_cfCount + m_acfCount;

      CondFmt12Record temp_condfmt12 = null;
      CondFMTRecord temp_condfmt = null;

      if (iCount <= 3)
      {
          for (int i = 0; i < iCount; i++)
          {
              ConditionalFormatImpl condFormat = (ConditionalFormatImpl)InnerList[i];
              
              if ((condFormat.CF12Record.IsParsed) || ((!condFormat.CF12Record.IsParsed) && (condFormat.FormatType == ExcelCFType.ColorScale || condFormat.FormatType == ExcelCFType.DataBar || condFormat.FormatType == ExcelCFType.IconSet)))
                  temp_condfmt12 = SerializeACF(condFormat, temp_condfmt12, priority, records,index,m_acfCount);
              else
                  temp_condfmt = SerializeCF(condFormat, temp_condfmt, priority, records, index,i);

              priority++;              
          }
      }
      else
      {
          for (int i = 0; i < iCount; i++)
          {
              ConditionalFormatImpl condFormat = (ConditionalFormatImpl)InnerList[i];
              if (m_acfCount == m_totalCFCount)
              {
                  temp_condfmt12 = SerializeACF(condFormat, temp_condfmt12, priority, records, index, m_acfCount);
              }
              else if (m_cfCount == m_totalCFCount)
              {
                  temp_condfmt = SerializeCF(condFormat, temp_condfmt, priority, records, index, i);
              }
              else
              {
                  if (condFormat.CF12Record.IsParsed || ((!condFormat.CF12Record.IsParsed) && (condFormat.FormatType == ExcelCFType.ColorScale || condFormat.FormatType == ExcelCFType.DataBar || condFormat.FormatType == ExcelCFType.IconSet)))
                  {
                      temp_condfmt12 = SerializeACF(condFormat, temp_condfmt12, priority, records, index, m_totalCFCount);
                  }
                  else
                  {
                      temp_condfmt = SerializeCF(condFormat, temp_condfmt, priority, records, index, i);
                  }
              }

              priority++;
          }
      }

      foreach (CFExRecord cfex in m_sheet.m_dictCFExRecords.Values)
      {
          if (!cfex.IsCFExParsed)
          {
              foreach (CondFMTRecord formatRecord in m_sheet.m_dictCondFMT.Values)
              {
                  Rectangle rect = cfex.EncloseRange.GetRectangle();
                  if (formatRecord.CellList.Contains(rect))
                  {
                      cfex.CondFmtIndex = formatRecord.Index;
                  }
              }
          }
       }

      return priority;
    }
    /// <summary>
    /// Serializes normal conditional formats.
    /// </summary>
    private CondFmt12Record SerializeACF(ConditionalFormatImpl condFormat,CondFmt12Record temp_condfmt12,int priority,OffsetArrayList records,ushort index,int ruleCount)
    {
        if (m_totalCFCount == m_acfCount)
        {
            if ((!condFormat.CF12Record.IsParsed) && (condFormat.FormatType == ExcelCFType.ColorScale || condFormat.FormatType == ExcelCFType.DataBar || condFormat.FormatType == ExcelCFType.IconSet))
            {
                if (temp_condfmt12 == null)
                {
                    m_condFMT12.CellList = CellRectangles;
                    m_condFMT12.CellsCount = (ushort)CellRectangles.Count;
                    m_condFMT12.EncloseRange = EnclosedRange;
                    m_condFMT12.Index = index;
                    m_condFMT12.CF12RecordCount = (ushort)ruleCount;

                    records.Add(m_condFMT12);
                    temp_condfmt12 = m_condFMT12;
                }

                condFormat.CF12Record.FormatType = condFormat.FormatType;
                condFormat.CF12Record.ComparisonOperator = ExcelComparisonOperator.None;
                condFormat.CF12Record.StopIfTrue = condFormat.StopIfTrue;
                condFormat.CF12Record.Priority = (ushort)priority;

                condFormat.CF12Record.Criteria = condFormat.ColorScale;
                condFormat.CF12Record.DataBarImpl = condFormat.DataBar;
                condFormat.CF12Record.IconSetImpl = condFormat.IconSet;

                condFormat.SerializeCF12(records);
            }
            else
            {
                if (temp_condfmt12 == null)
                {
                    records.Add(m_condFMT12);
                    temp_condfmt12 = m_condFMT12;
                }

                condFormat.CF12Record.Priority = (ushort)priority;
                condFormat.SerializeCF12(records);
            }
        }
        else
        {
            if ((!condFormat.CF12Record.IsParsed) && condFormat.CFExRecord.IsCF12Extends!=1)
            {
                //Fill CFEx record
                condFormat.CFExRecord.CondFmtIndex = index;
                condFormat.CFExRecord.IsCF12Extends = 1;

                //Fill CF12Record if it extends
                condFormat.CFExRecord.CF12RecordIfExtends = condFormat.CF12Record;
                condFormat.CFExRecord.CF12RecordIfExtends.FormatType = condFormat.FormatType;
                condFormat.CFExRecord.CF12RecordIfExtends.ComparisonOperator = condFormat.Operator;
                condFormat.CFExRecord.CF12RecordIfExtends.StopIfTrue = condFormat.StopIfTrue;
                condFormat.CFExRecord.CF12RecordIfExtends.Priority = (ushort)priority;

                condFormat.CFExRecord.CF12RecordIfExtends.Criteria = condFormat.ColorScale;
                condFormat.CFExRecord.CF12RecordIfExtends.DataBarImpl = condFormat.DataBar;
                condFormat.CFExRecord.CF12RecordIfExtends.IconSetImpl = condFormat.IconSet;

                m_sheet.m_dictCFExRecords.Add(m_sheet.m_dictCFExRecords.Count, condFormat.CFExRecord);
            }
            else
            {
                m_sheet.m_dictCFExRecords.Add(m_sheet.m_dictCFExRecords.Count, condFormat.CFExRecord);
            }
        }

        return temp_condfmt12;
    }
    /// <summary>
    /// Serializes advanced conditional formats.
    /// </summary>
    private CondFMTRecord SerializeCF(ConditionalFormatImpl condFormat, CondFMTRecord temp_condfmt, int priority, OffsetArrayList records,ushort index,int CFIndex)
    {
        if ((m_totalCFCount==m_cfCount && CFIndex < 3) || (m_totalCFCount!=m_cfCount && m_cfCount<=3))
        {
            if (!m_format.IsParsed)
            {
                if (temp_condfmt == null)
                {
                    m_format.CellList = CellRectangles;
                    m_format.CellsCount = (ushort)CellRectangles.Count;
                    m_format.EncloseRange = EnclosedRange;
                    m_format.Index = index;
                    if (m_cfCount > 3)
                        m_format.CFNumber = 3;
                    else
                        m_format.CFNumber = (ushort)m_cfCount;

                    records.Add(m_format);
                    temp_condfmt = m_format;
                    m_sheet.m_dictCondFMT.Add(temp_condfmt.Index, temp_condfmt);
                }
                
                condFormat.CFExRecord.StopIfTrue = condFormat.StopIfTrue;
                condFormat.CFExRecord.Priority = (ushort)priority;
                condFormat.CFExRecord.CondFmtIndex = m_format.Index;
                condFormat.CFExRecord.CFIndex = (ushort)m_cfIndex;
                m_cfIndex++;

                condFormat.Serialize(records);
                m_sheet.m_dictCFExRecords.Add(m_sheet.m_dictCFExRecords.Count, condFormat.CFExRecord);
            }
            else
            {
                if (temp_condfmt == null && !(m_sheet.m_dictCondFMT.ContainsKey(m_format.Index)))
                {
                    records.Add(m_format);
                    temp_condfmt = m_format;
                    m_sheet.m_dictCondFMT.Add(temp_condfmt.Index, m_format);
                }

                if (condFormat.CFExRecord.CondFmtIndex == 0)
                    condFormat.Serialize(records);
                else
                    m_sheet.m_dictCFExRecords.Add(m_sheet.m_dictCFExRecords.Count, condFormat.CFExRecord);
            }
        }
        else
        {
            if (!m_format.IsParsed)
            {
                //Fill CFEx record
                condFormat.CFExRecord.CellList = CellRectangles;
                condFormat.CFExRecord.CellsCount = (ushort)CellRectangles.Count;
                condFormat.CFExRecord.EncloseRange = EnclosedRange;                
                condFormat.CFExRecord.IsCF12Extends = 1;

                //Fill CF12Record if it extends
                condFormat.CFExRecord.CF12RecordIfExtends = condFormat.CF12Record;
                condFormat.CFExRecord.CF12RecordIfExtends.FormatType = condFormat.FormatType;
                condFormat.CFExRecord.CF12RecordIfExtends.ComparisonOperator = condFormat.Operator;
                condFormat.CFExRecord.CF12RecordIfExtends.StopIfTrue = condFormat.StopIfTrue;
                condFormat.CFExRecord.CF12RecordIfExtends.Priority = (ushort)priority;

                m_sheet.m_dictCFExRecords.Add(m_sheet.m_dictCFExRecords.Count, condFormat.CFExRecord);
            }
            else
            {
                m_sheet.m_dictCFExRecords.Add(m_sheet.m_dictCFExRecords.Count, condFormat.CFExRecord);
            }
        }

        return temp_condfmt;
    }
    #endregion

    #region Class Helper Methods
    /// <summary>
    /// Adds condition from CFRecord.
    /// </summary>
    /// <param name="cf">CFRecord that contains condition data.</param>
    [CLSCompliant( false )]
    public void AddFromRecord( CFRecord cf )
    {
      ConditionalFormatImpl format = new ConditionalFormatImpl( Application, this, cf );
      base.Add( format );
    }
    /// <summary>
    /// Adds condition from CF12Record.
    /// </summary>
    /// <param name="cf12">CF12Record that contains condition data.</param>
    [CLSCompliant(false)]
    public void AddFromCF12Record(CF12Record cf12)
    {
        ConditionalFormatImpl format = new ConditionalFormatImpl(Application, this, cf12);
        format.FormatType = cf12.FormatType;
        base.Add(format);
    }
    /// <summary>
    /// Adds condition from CFExRecord.
    /// </summary>
    /// <param name="cf">CFRecord that contains condition data.</param>
    [CLSCompliant(false)]
    public void AddFromCFEXRecord(CFExRecord cfEx)
    {
        ConditionalFormatImpl format = new ConditionalFormatImpl(Application, this, cfEx);
        if (cfEx.IsCF12Extends == 1)
            format.FormatType = cfEx.CF12RecordIfExtends.FormatType;
        base.Add(format);
    }
    /// <summary>
    /// Compares this collection to another.
    /// </summary>
    /// <param name="formats">Collection to compare.</param>
    /// <returns>True if collections are equal.</returns>
    public bool CompareTo( ConditionalFormats formats )
    {
      if( formats.Count != Count )
        return false;

      for( int i = 0; i < Count; i++ )
      {
        if( !CompareFormats( this[ i ], formats[ i ] ) )
          return false;
      }

      return true;
    }
    /// <summary>
    /// Compares two ConditionalFormats.
    /// </summary>
    /// <param name="firstFormat">First conditional format to compare.</param>
    /// <param name="secondFormat">Second conditional format to compare.</param>
    /// <returns>True if they are equal.</returns>
    public bool CompareFormats( IConditionalFormat firstFormat, IConditionalFormat secondFormat )
    {
      ConditionalFormatImpl format1 = ( ConditionalFormatImpl )firstFormat;
      ConditionalFormatImpl format2 = ( ConditionalFormatImpl )secondFormat;

      CFRecord record1 = format1.Record;
      CFRecord record2 = format2.Record;

      CF12Record record12_1 = format1.CF12Record;
      CF12Record record12_2 = format2.CF12Record;

      CFExRecord recordEx_1 = format1.CFExRecord;
      CFExRecord recordEx_2 = format2.CFExRecord;

      if (record1.Data.Length != record2.Data.Length || record12_1.Data.Length != record12_2.Data.Length || recordEx_1.Data.Length != recordEx_2.Data.Length)
        return false;

      return ((BiffRecordRaw.CompareArrays( record1.Data, 0, record2.Data, 0, record1.Length)) &&
          (BiffRecordRaw.CompareArrays( record12_1.Data, 0, record12_2.Data, 0, record12_1.Length)) &&
          (BiffRecordRaw.CompareArrays( recordEx_1.Data, 0, recordEx_2.Data, 0, recordEx_1.Length)));
    }
    /// <summary>
    /// Adds cells from the collection.
    /// </summary>
    /// <param name="formats">Formats collection to get cells from.</param>
    public void AddCells( ConditionalFormats formats )
    {
      if( formats == null )
        return;

      CondFMTRecord condFMT = formats.m_format;
      List<Rectangle> arrCells = condFMT.CellList;
      AddCells( arrCells );
    }
    /// <summary>
    /// Adds cells from the collection.
    /// </summary>
    /// <param name="formats">Formats collection to get cells from.</param>
    public void AddCellsCondFMT12(ConditionalFormats formats)
    {
        if (formats == null)
            return;

        List<Rectangle> arrCells = new List<Rectangle>();

        CondFmt12Record condFMT12 = formats.m_condFMT12;
        arrCells = condFMT12.CellList;

        AddCells(arrCells);
    }
    /// <summary>
    /// Indicates whether collection contains all specified ranges.
    /// </summary>
    /// <param name="arrRanges">Ranges to check.</param>
    /// <returns>True if collection contains all specified ranges.</returns>
    public bool Contains( Rectangle[] arrRanges )
    {
      return m_rangesOperations.Contains( arrRanges );
    }
    /// <summary>
    /// Returns contains count for specified range.
    /// </summary>
    /// <param name="range">Range to check.</param>
    /// <returns>Contains count</returns>
    public int ContainsCount( Rectangle range )
    {
      return m_rangesOperations.ContainsCount( range );
    }
    /// <summary>
    /// Adds cells from the collection.
    /// </summary>
    /// <param name="arrCells">Cells to add to the collection.</param>
    public void AddCells( IList<Rectangle> arrCells )
    {
      //m_rangesOperations.AddCells( arrCells );
      if( arrCells == null )
        return;

      for( int i = 0, len = arrCells.Count; i < len; i++ )
      {
        Rectangle range = arrCells[ i ];
        AddRange( range );
      }
    }
    /// <summary>
    /// Adds range to the collection.
    /// </summary>
    /// <param name="range">Range to add.</param>
    public void AddRange( IRange range )
    {
      m_rangesOperations.AddRange( range );
    }
    /// <summary>
    /// Adds new cell range to the collection.
    /// </summary>
    /// <param name="rect">Range to add.</param>
    public void AddRange( Rectangle rect )
    {
      m_rangesOperations.AddRange( rect );

      TAddr enclosed=new TAddr();

      if (m_format != null)
          enclosed = m_format.EncloseRange;
      if(m_condFMT12!=null)
          enclosed = m_condFMT12.EncloseRange;

      enclosed.FirstCol = Math.Min( rect.Left, enclosed.FirstCol );
      enclosed.FirstRow = Math.Min( rect.Top, enclosed.FirstRow );

      enclosed.LastCol = Math.Max( rect.Right, enclosed.LastCol );
      enclosed.LastRow = Math.Max( rect.Bottom, enclosed.LastRow );

      if (m_format != null)
          m_format.EncloseRange = enclosed;
      if(m_condFMT12!=null)
          m_condFMT12.EncloseRange = enclosed;
    }
    /// <summary>
    /// Removes range from the collection of conditional formats.
    /// </summary>
    /// <param name="arrRanges">Array of ranges to remove.</param>
    public void Remove( Rectangle[] arrRanges )
    {
      m_rangesOperations.Remove( arrRanges );
    }
    /// <summary>
    /// Removes all cells from the collection.
    /// </summary>
    public void ClearCells()
    {
        if (m_format != null && m_format.CFNumber>0)
        {
            m_format.CellList.Clear();
            m_format.CellsCount = 0;
        }

        if (m_condFMT12 != null)
        {
            m_condFMT12.CellList.Clear();
            m_condFMT12.CellsCount = 0;
        }
    }
    /// <summary>
    /// Converts collection to Excel97to03 version (reduces collection capacity to maximum three conditions).
    /// </summary>
    public void ConvertToExcel97to03Version()
    {
      int iCount = Count;

      if( iCount > MAXIMUM_CF_NUMBER )
      {
        int iNumberToRemove = Count - MAXIMUM_CF_NUMBER;
        InnerList.RemoveRange( MAXIMUM_CF_NUMBER, iNumberToRemove );
      }
    }
    /// <summary>
    /// Creates ConditionaFormats collection containing part of conditional formats.
    /// </summary>
    /// <param name="row">Index of the first row to get.</param>
    /// <param name="column">Index of the first column to get.</param>
    /// <param name="rowCount">Number of rows to get.</param>
    /// <param name="columnCount">Number of columns to get.</param>
    /// <param name="remove">Indicates whether we should remove original items from the collection.</param>
    /// <param name="rowIncrement">This value should be added to row index of every CF region.</param>
    /// <param name="columnIncrement">This value should be added to column index of every CF region.</param>
    /// <param name="newParent">New parent object.</param>
    /// <returns>Created CF collection.</returns>
    public ConditionalFormats GetPart( int row, int column, int rowCount, int columnCount,
      bool remove, int rowIncrement, int columnIncrement, object newParent )
    {
        if (columnCount - 1 >= 0)
            columnCount -= 1;
        if (rowCount - 1 >= 0)
            rowCount -= 1;

      Rectangle rect = new Rectangle( column - 1, row - 1, columnCount , rowCount );
      RangesOperations resultRanges = m_rangesOperations.GetPart( rect, remove, rowIncrement, columnIncrement );
      ConditionalFormats result = null;

      if( resultRanges != null )
      {
        result = ( ConditionalFormats )Clone( newParent );
        result.FindParent();
        result.m_rangesOperations = resultRanges;
        if (m_format != null && result.m_format.CFNumber > 0)
        {
            result.m_format = (CondFMTRecord)CloneUtils.CloneCloneable(m_format);
            result.m_format.CellList = resultRanges.CellList;
        }
        if(m_condFMT12!=null)
        {
            result.m_condFMT12 = (CondFmt12Record)CloneUtils.CloneCloneable(m_condFMT12);
            result.m_condFMT12.CellList = resultRanges.CellList;
        }
      }

      return result;
    }
    /// <summary>
    /// Sets items with used reference indexes to true.
    /// </summary>
    /// <param name="usedItems">Array to mark used references in.</param>
    public void MarkUsedReferences( bool[] usedItems )
    {
      List<IConditionalFormat> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        ConditionalFormatImpl condFormat = ( ConditionalFormatImpl )list[ i ];
        condFormat.MarkUsedReferences( usedItems );
      }
    }
    /// <summary>
    /// Updates reference indexes.
    /// </summary>
    /// <param name="arrUpdatedIndexes">Array with updated indexes.</param>
    public void UpdateReferenceIndexes( int[] arrUpdatedIndexes )
    {
      List<IConditionalFormat> list = InnerList;

      for( int i = 0, len = list.Count; i < len; i++ )
      {
        ConditionalFormatImpl condFormat = ( ConditionalFormatImpl )list[ i ];
        condFormat.UpdateReferenceIndexes( arrUpdatedIndexes );
      }
    }
    /// <summary>
    /// Updates conditional format formulas.
    /// </summary>
    /// <param name="iCurIndex">Current worksheet index.</param>
    /// <param name="iSourceIndex">Source worksheet index.</param>
    /// <param name="sourceRect">Source rectangle.</param>
    /// <param name="iDestIndex">Destination worksheet index.</param>
    /// <param name="destRect">Destination rectangle.</param>
    public void UpdateFormula( int iCurIndex, int iSourceIndex, Rectangle sourceRect, int iDestIndex, Rectangle destRect )
    {
      if( m_rangesOperations == null || m_rangesOperations.CellList.Count == 0 )
        return;

      Rectangle rect = m_rangesOperations.CellList[ 0 ];
      int row = rect.Top;
      int column = rect.Left;

      for( int i = 0, len = Count; i < len; i++ )
      {
        ( this[ i ] as ConditionalFormatImpl ).UpdateFormula( iCurIndex, iSourceIndex,
          sourceRect, iDestIndex, destRect, row, column );
      }
    }
    #endregion

    #region IOptimizedUpdate methods
    /// <summary>
    /// This method should be called before several updates to the object will take place.
    /// </summary>
    public void BeginUpdate()
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// This method should be called after several updates to the object took place.
    /// </summary>
    public void EndUpdate()
    {
      throw new NotImplementedException();
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Serves as a hash function for a particular type, suitable for use in
    /// hashing algorithms and data structures like a hash table.
    /// </summary>
    /// <returns>A hash code for the current Object.</returns>
    public override int GetHashCode()
    {
      // TODO: here we can optimize a little bit by caching hashcode.
      int iCount = Count;
      int iHashCode = iCount;

      for( int i = 0; i < iCount; i++ )
      {
        iHashCode |= this[ i ].GetHashCode();
      }

      return iHashCode;
    }
    /// <summary>
    /// A hash code for the current Object without taking cell list into account.
    /// </summary>
    /// <param name="obj">The Object to compare with the current Object.</param>
    /// <returns></returns>
    public override bool Equals( object obj )
    {
      ConditionalFormats toCompare = obj as ConditionalFormats;

      if( toCompare == null )
        return false;

      int iCount = Count;

      if( iCount != toCompare.Count )
        return false;

      for( int i = 0; i < iCount; i++ )
      {
        //if( !( this[ i ] == toCompare[ i ] ) ) return false;
        if( !this[ i ].Equals( toCompare[ i ] ) )
          return false;
      }

      return true;
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <param name="parent">Parent object for a copy of this instance.</param>
    /// <returns>A new object that is a copy of this instance.</returns>
    public override object Clone( object parent )
    {
      ConditionalFormats result = ( ConditionalFormats )base.Clone( parent );

      List<Rectangle> arrCells=new List<Rectangle>();
      if (m_format != null)
      {
          result.m_format = (CondFMTRecord)CloneUtils.CloneCloneable(m_format);
          arrCells = result.m_format.CellList;
      }
      if(m_condFMT12!=null)
      {
          result.m_condFMT12 = (CondFmt12Record)CloneUtils.CloneCloneable(m_condFMT12);
          arrCells = result.m_condFMT12.CellList;
      }

      result.m_rangesOperations = new RangesOperations(arrCells);

      return result;
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Indicates whether collection is empty. Read-only.
    /// </summary>
    public bool IsEmpty
    {
      get
      {
          if (m_format != null && m_format.CFNumber>0)
              return m_format.CellList.Count == 0;
          else
              return m_condFMT12.CellList.Count == 0;
      }
    }
    /// <summary>
    /// Represents address of cf ranges.
    /// </summary>
    public string Address
    {
      get
      {
        TAddr addr=new TAddr();
 
          if (m_format != null)
              addr = m_format.EncloseRange;
          else
              addr = m_condFMT12.EncloseRange;

        return RangeImpl.GetAddressLocal( addr.FirstRow + 1, addr.FirstCol + 1, addr.LastRow + 1, addr.LastCol + 1 );
      }
    }
    /// <summary>
    /// Represents address of cf ranges in R1C1 notation.
    /// </summary>
    public string AddressR1C1
    {
      get
      {
          TAddr addr = new TAddr();

          if (m_format != null)
              addr = m_format.EncloseRange;
          else
              addr = m_condFMT12.EncloseRange;

        return string.Format( "R{0}C{1}:R{2}C{3}", addr.FirstRow + 1, addr.FirstCol + 1
          , addr.LastRow + 1, addr.LastCol + 1 );
      }
    }
    /// <summary>
    /// Cell range address of the range enclosing all
    /// conditionally formatted ranges.
    /// </summary>
    [CLSCompliant( false )]
    public TAddr EnclosedRange
    {
      get
      {
          TAddr addr = new TAddr();
          if (m_format != null)
              addr = m_format.EncloseRange;
          if(m_condFMT12!=null)
              addr =  m_condFMT12.EncloseRange;
          return addr;
      }
      set
      {
         if (m_format != null)
              m_format.EncloseRange = value;
         if(m_condFMT12!=null)
              m_condFMT12.EncloseRange = value;
      }
    }
    /// <summary>
    /// Returns all string ranges for this CF.
    /// </summary>
    public string[] CellsList
    {
      get
      {
        List<Rectangle> lstCells=new List<Rectangle>();

        if (m_format != null && m_format.CFNumber>0)
            lstCells = m_format.CellList;
        else
            lstCells=m_condFMT12.CellList;

        int iCount = lstCells.Count;
        string[] arrResult = new string[ iCount ];

        for( int i = 0; i < iCount; i++ )
        {
          Rectangle rect = lstCells[ i ];
          arrResult[ i ] = RangeImpl.GetAddressLocal( rect.Y + 1, rect.X + 1, rect.Bottom + 1, rect.Right + 1 );
        }

        return arrResult;
      }
    }
    /// <summary>
    /// Gets list of rectangles describing cells with conditional formatting.
    /// </summary>
    public List<Rectangle> CellRectangles
    {
      get
      {
        return m_rangesOperations.CellList;
      }
    }
    /// <summary>
    /// CondFMT record.
    /// </summary>
    public CondFMTRecord CondFMTRecord
    {
        get
        {
            return m_format;
        }
        set
        {
            m_format = value;
        }
    }
    /// <summary>
    /// CondFMT record.
    /// </summary>
    public CondFmt12Record CondFMT12Record
    {
        get
        {
            return m_condFMT12;
        }
        set
        {
            m_condFMT12 = value;
        }
    }
    /// <summary>
    /// Specify whether the record is future record.
    /// </summary>
    public bool IsFutureRecord
    {
        get
        {
            return m_futureRecord;
        }
        set
        {
            m_futureRecord = value;
        }
    }
    #endregion
  }
}
