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

using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Interfaces;
#endregion

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// Represents hyperlink object.
  /// </summary>
  public class HyperLinkImpl
    : CommonObject
    , IHyperLink
    , ICloneParent
  {
    #region Class constants
    /// <summary>
    /// Style for hyperlink.
    /// </summary>
    public const string DEF_STYLE_NAME = "Hyperlink";
    #endregion

    #region Class members
    /// <summary>
    /// Hyper link record.
    /// </summary>
    private HLinkRecord m_link = ( HLinkRecord )BiffRecordFactory.GetRecord( TBIFFRecord.HLink );
    /// <summary>
    /// Tool tip.
    /// </summary>
    private string m_strToolTip;
    /// <summary>
    /// Parent worksheet.
    /// </summary>
    private WorksheetImpl m_sheet;
    /// <summary>
    /// Range object that represents the range the specified hyperlink is attached to.
    /// </summary>
    private IRange m_range;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates an object and sets its Application and Parent 
    /// properties to specified values.
    /// </summary>
    /// <param name="application">Application object for the hyperlink.</param>
    /// <param name="parent">Parent object for the hyperlink.</param>
    public HyperLinkImpl( IApplication application, object parent )
      : base( application, parent )
    {
      SetParents();
    }
    /// <summary>
    /// Recover hyperlink from Biff records.
    /// </summary>
    /// <param name="application">Application object for the hyperlink.</param>
    /// <param name="parent">Parent object for the hyperlink.</param>
    /// <param name="data">Array of BiffRecordRaws that contains hyperlink records.</param>
    /// <param name="iPos">Position of the first hyperlink record.</param>
    public HyperLinkImpl( IApplication application, object parent, IList data, ref int iPos )
      : this( application, parent )
    {
      iPos = Parse( data, iPos );
    }
    /// <summary>
    /// Creates an object and sets its Application, Parent and Range 
    /// properties to specified values.
    /// </summary>
    /// <param name="application">Application object for the hyperlink.</param>
    /// <param name="parent">Parent object for the hyperlink.</param>
    /// <param name="range">
    /// Range object that represents the range the specified hyperlink is attached to.
    /// </param>
    public HyperLinkImpl( IApplication application, object parent, IRange range )
      : this( application, parent )
    {
      if( range == null )
        throw new ArgumentNullException( "range" );

      //range.CellStyleName = DEF_STYLE_NAME;

      m_link.FirstRow = ( uint )( range.Row - 1 );        // make it zero based.
      m_link.FirstColumn = ( uint )( range.Column - 1 );  // make it zero based.
      m_link.LastRow = ( uint )( range.LastRow - 1 );        // make it zero based.
      m_link.LastColumn = ( uint )( range.LastColumn - 1 );  // make it zero based.
    }
    /// <summary>
    /// Searches for all necessary parent objects.
    /// </summary>
    private void SetParents()
    {
      m_sheet = FindParent( typeof( WorksheetImpl ) ) as WorksheetImpl;

      if( m_sheet == null )
        throw new ArgumentNullException( "Can't find parent worksheet" );
    }
    #endregion

    #region IHyperLink Members
    /// <summary>
    /// Returns or sets the address of the target document.
    /// </summary>
    public string Address
    {
      get
      {
        switch( m_link.LinkType )
        {
          case ExcelHyperLinkType.File:
            return m_link.FileName;

          case ExcelHyperLinkType.Unc:
            return m_link.UncPath;

          case ExcelHyperLinkType.Url:
            return m_link.Url;

          case ExcelHyperLinkType.Workbook:
            return m_link.TextMark;

          case ExcelHyperLinkType.None:
            return null;

          default:
            throw new ArgumentOutOfRangeException( "LinkType" );
        }
      }
      set
      {
        SetAddress( value, true );
      }
    }

    /// <summary>
    /// Returns or sets the name of the object.
    /// </summary>
    public string Name
    {
      get
      {
        return m_link.Description;
      }
    }

    /// <summary>
    /// Returns/sets a Range object that represents the range the specified hyperlink is attached to.
    /// </summary>
    public IRange Range
    {
      get
      {
          if (m_range == null)
          {
              m_range = m_sheet.Range[(int)m_link.FirstRow + 1, (int)m_link.FirstColumn + 1,
                (int)m_link.LastRow + 1, (int)m_link.LastColumn + 1];
          }
        return m_range;
      }
      set
      {
        m_link.FirstRow = ( ushort )( value.Row - 1 );
        m_link.FirstColumn = ( ushort )( value.Column - 1 );
        m_link.LastRow = ( ushort )( value.LastRow - 1 );
        m_link.LastColumn = ( ushort )( value.LastColumn - 1 );
        m_range = m_sheet.Range[(int) m_link.FirstRow +1, (int)m_link.FirstColumn+1,(int) m_link.LastRow+1, (int)m_link.LastColumn+1];
      }
    }

    /// <summary>
    /// Returns or sets the ScreenTip text for the specified hyperlink.
    /// </summary>
    public string ScreenTip
    {
      get
      {
        return m_strToolTip;
      }
      set
      {
        m_strToolTip = value;
      }
    }

    /// <summary>
    /// Returns or sets the location within the document associated with the hyperlink.
    /// </summary>
    public string SubAddress
    {
      get
      {
        return m_link.TextMark;
      }
      set
      {
        if( Range.CellStyleName != DEF_STYLE_NAME )
          Range.CellStyleName = DEF_STYLE_NAME;

        m_link.TextMark = value;
      }
    }

    /// <summary>
    /// Returns or sets the text to be displayed for the specified hyperlink.
    /// The default value is the address of the hyperlink.
    /// </summary>
    public string TextToDisplay
    {
      get
      {
        if (m_link.Description== string.Empty || Range.Value == string.Empty || Range.Value== null)
             return m_link.Description;
          else
              return Range.Value;
      }
      set
      {
        if( value != TextToDisplay )
        {
          m_link.Description = value;

          if( !m_sheet.ParentWorkbook.Loading )
             TopLeftCell.Value2 = m_link.Description;
        }
      }
    }

    /// <summary>
    /// Returns or sets the object type.
    /// </summary>
    public ExcelHyperLinkType Type
    {
      get
      {
        return m_link.LinkType;
      }
      set
      {
        m_link.LinkType = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public int FirstRow
    {
      get
      {
        return (int)m_link.FirstRow;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int FirstColumn
    {
      get
      {
        return (int)m_link.FirstColumn;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int LastRow
    {
      get
      {
        return (int)m_link.LastRow;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int LastColumn
    {
      get
      {
        return (int)m_link.LastColumn;
      }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Recovers hyperlink from array of Biff Records and position in it.
    /// </summary>
    /// <param name="data">Array of BiffRecordRaws that contains hyperlink records.</param>
    /// <param name="iPos">Position of the first hyperlink record.</param>
    /// <returns>Position after the last hyperlink record.</returns>
    private int Parse( IList data, int iPos )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      if( iPos < 0 || iPos > data.Count - 1 )
        throw new ArgumentOutOfRangeException( "iPos", "Value cannot be less than 0 and greater than data.Length - 1" );

      BiffRecordRaw record = ( BiffRecordRaw )data[ iPos ];
      record.CheckTypeCode( TBIFFRecord.HLink );

      m_link = ( HLinkRecord )record;
      iPos++;

      record = ( BiffRecordRaw )data[ iPos ];

      if( record.TypeCode == TBIFFRecord.QuickTip )
      {
        QuickTipRecord quickTip = ( QuickTipRecord )record;
        m_strToolTip = quickTip.ToolTip;
        iPos++;
      }

      return iPos;
    }
    /// <summary>
    /// Saves hyperlink into OffsetArrayList.
    /// </summary>
    /// <param name="records">
    /// OffsetArrayList that will receive all records of the range.
    /// </param>
    [ CLSCompliant( false ) ]
    public void Serialize( OffsetArrayList records )
    {
      if( records == null )
        throw new ArgumentNullException( "records" );

      records.Add( m_link );

      if( m_strToolTip != null && m_strToolTip.Length > 0 )
      {
        QuickTipRecord tip = ( QuickTipRecord )BiffRecordFactory.GetRecord( TBIFFRecord.QuickTip );
        tip.ToolTip = m_strToolTip;
        tip.CellRange = new TAddr( (int)m_link.FirstRow, (int)m_link.FirstColumn, (int)m_link.LastRow, (int)m_link.LastColumn );
        records.Add( tip );
      }
    }
    /// <summary>
    /// Sets sub address property.
    /// </summary>
    /// <param name="strSubAddress">Sub address to set.</param>
    public void SetSubAddress( string strSubAddress )
    {
      m_link.TextMark = strSubAddress;
    }
    /// <summary>
    /// Sets address property.
    /// </summary>
    /// <param name="strAddress">Address to set.</param>
    /// <param name="bSetText">Indicates whether we should set Text property of
    /// the TopLeft cell with this hyperlink.</param>
    public void SetAddress( string strAddress, bool bSetText )
    {
      if( bSetText )
      {
        Range.CellStyleName = DEF_STYLE_NAME;
        
        if( m_link.Description == null || m_link.Description.Length == 0 )
        {
          TopLeftCell.Text = strAddress;
        }
      }

      if( strAddress.IndexOf( '#' ) == 0 )
        strAddress = strAddress.Remove( 0, 1 );

      switch( m_link.LinkType )
      {
        case ExcelHyperLinkType.File:
          m_link.FileName = strAddress;
          //m_link.XFilePath = value;
          m_link.IsAbsolutePathOrUrl = Path.IsPathRooted( strAddress );
          break;

        case ExcelHyperLinkType.Unc:
          m_link.UncPath = strAddress;
          break;

        case ExcelHyperLinkType.Url:
          m_link.Url = strAddress;
          break;

        case ExcelHyperLinkType.Workbook:
          m_link.TextMark = strAddress;
          break;

        default:
          throw new ArgumentOutOfRangeException( "LinkType" );
      }
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns top left cell of the range. Read-only.
    /// </summary>
    protected IRange TopLeftCell
    {
      get
      {
        return m_sheet.Range[ (int)m_link.FirstRow + 1, (int)m_link.FirstColumn + 1 ];
      }
    }
    #endregion

    #region ICloneParent Members
    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <param name="parent">Parent object for a copy of this instance.</param>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone( object parent )
    {
      if( parent == null )
        throw new ArgumentNullException( "parent" );

      HyperLinkImpl result = ( HyperLinkImpl )MemberwiseClone();
      result.SetParent( parent );
      result.SetParents();

      result.m_link = ( HLinkRecord )CloneUtils.CloneCloneable( m_link );
      result.m_range = null;

      return result;
    }

    #endregion
  }
}
