#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Windows.Forms;

using Syncfusion.XlsIO.Implementation;

namespace Syncfusion.XlsIO.Implementation.Clipboard
{
  /// <summary>
  /// ClipboardProvider is an abstract class that facilitates reading from
  /// or writing to the clipboard workbook object.
  /// </summary>
  public abstract class ClipboardProvider
  {
    #region Class members
    /// <summary>
    /// Next element in the clipboard providers list.
    /// </summary>
    private ClipboardProvider m_clpProviderNext;
    /// <summary>
    /// Workbook that should be copied to the clipboard.
    /// </summary>
    private IWorkbook m_book;
    /// <summary>
    /// Worksheet that should be copied to the clipboard.
    /// </summary>
    private IWorksheet m_sheet;
    /// <summary>
    /// Name of the format that this ClipboardProvider can 
    /// read from or write to.
    /// </summary>
    private string m_strFormatName = string.Empty;
    #endregion

    #region Class constructor
    /// <summary>
    /// Default constructor.
    /// </summary>
    protected ClipboardProvider()
    {
    }

    /// <summary>
    /// Creates provider for the specified worksheet.
    /// </summary>
    /// <param name="worksheet">Worksheet that should be copied to the clipboard.</param>
    protected ClipboardProvider( IWorksheet worksheet )
      : this( worksheet, null )
    {
    }

    /// <summary>
    /// Creates provider for the specified workbook.
    /// </summary>
    /// <param name="workbook">Workbook that should be copied to the clipboard.</param>
    protected ClipboardProvider( IWorkbook workbook )
      : this( workbook, null )
    {
    }

    /// <summary>
    /// Creates provider for the specified workbook and sets the Next property 
    /// to the specified value.
    /// </summary>
    /// <param name="workbook">Workbook that should be copied to the clipboard.</param>
    /// <param name="next">Next clipboard provider.</param>
    protected ClipboardProvider( IWorkbook workbook, ClipboardProvider next )
    {
      m_clpProviderNext = next;
      Initialize( workbook );
    }
    /// <summary>
    /// Creates provider for the specified worksheet and sets the Next property 
    /// to the specified value.
    /// </summary>
    /// <param name="worksheet">Worksheet that should be copied to the clipboard.</param>
    /// <param name="next">Next ClipboardProvider.</param>
    protected ClipboardProvider( IWorksheet worksheet, ClipboardProvider next )
    {
      m_clpProviderNext = next;
      Initialize( worksheet );
    }
    #endregion
    
    #region Class properties
    /// <summary>
    /// Returns next clipboard provider.
    /// </summary>
    public ClipboardProvider Next
    {
      get
      {
        return m_clpProviderNext;
      }
      set
      {
        m_clpProviderNext = value;
      }
    }

    /// <summary>
    /// Gets / sets name of the clipboard format.
    /// </summary>
    public virtual string FormatName
    {
      get
      {
        return m_strFormatName;
      }
      set
      {
        m_strFormatName = value;
      }
    }
    /// <summary>
    /// Workbook that can be copied to the clipboard.
    /// </summary>
    public IWorkbook Workbook
    {
      get
      {
        return m_book;
      }
      set
      {
        Initialize( value );
      }
    }
    /// <summary>
    /// Worksheet that can be copied to the clipboard.
    /// </summary>
    public IWorksheet Worksheet
    {
      get
      {
        return m_sheet;
      }
      set
      {
        Initialize( value );
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Initializes clipboard provider for the specified workbook.
    /// </summary>
    /// <param name="workbook">Workbook that can be copied to the clipboard.</param>
    public virtual void Initialize( IWorkbook workbook )
    {
      m_book = workbook;
    }
    /// <summary>
    /// Initializes clipboard provider for the specified worksheet.
    /// </summary>
    /// <param name="worksheet">Worksheet that can be copied to the clipboard.</param>
    public virtual void Initialize( IWorksheet worksheet )
    {
      m_sheet = worksheet;

      if( worksheet != null )
        m_book = worksheet.Workbook;
    }
    /// <summary>
    /// Sets clipboard's data object to the value returned 
    /// by GetForClipboard method.
    /// </summary>
    public virtual void SetClipboard()
    {
      IDataObject dataObject = GetForClipboard();

      ClipboardProvider curProvider = Next;

      while( curProvider != null )
      {
        curProvider.FillDataObject( dataObject );
        curProvider = curProvider.Next;
      }

      System.Windows.Forms.Clipboard.SetDataObject( dataObject, true );
    }

    /// <summary>
    /// Sets clipboard's data object to the value returned 
    /// by GetForClipboard method.
    /// </summary>
    /// <param name="range">Range to copy to the clipboard.</param>
    public virtual void SetClipboard( IRange range )
    {
      IDataObject dataObject = GetForClipboard( range );

      ClipboardProvider curProvider = Next;

      while( curProvider != null )
      {
        curProvider.FillDataObject( dataObject, range );
        curProvider = curProvider.Next;
      }

      System.Windows.Forms.Clipboard.SetDataObject( dataObject, true );
    }

    /// <summary>
    /// Reads workbook from the clipboard.
    /// </summary>
    /// <param name="workbooks">Workbooks collection to add workbook to.</param>
    /// <returns>Workbook that was read if success in reading; otherwise NULL.</returns>
    /// <exception cref="System.ArgumentNullException">
    /// When application is NULL.
    /// </exception>
    public virtual IWorkbook  GetBookFromClipboard( IWorkbooks workbooks )
    {
      if( workbooks == null )
        throw new ArgumentNullException( "workbooks" );

      IDataObject dataObject = System.Windows.Forms.Clipboard.GetDataObject();
      if( dataObject == null ) return null;
      
      if( dataObject.GetDataPresent( FormatName, true ) )
      {
        return ExtractWorkbook( dataObject, workbooks );
      }
      else if( Next != null )
      {
        return Next.GetBookFromClipboard( workbooks );
      }

      return null;
    }
    #endregion

    #region Abstract methods
    /// <summary>
    /// Returns IDataObject (for copying to the clipboard)
    /// that contains data from workbook or worksheet.
    /// </summary>
    /// <returns>IDataObject for copying to the clipboard.</returns>
    public abstract IDataObject GetForClipboard();
    /// <summary>
    /// Returns IDataObject (for copying to the clipboard)
    /// that contains data from workbook or worksheet.
    /// </summary>
    /// <param name="range">Range to copy to the clipboard.</param>
    /// <returns>IDataObject for copying to the clipboard.</returns>
    public abstract IDataObject GetForClipboard( IRange range );
    /// <summary>
    /// Extracts workbook from the data object.
    /// </summary>
    /// <param name="dataObject">Data object that contains workbook data.</param>
    /// <param name="workbooks">Workbooks collection to add workbook to.</param>
    /// <returns>Extracted workbook.</returns>
    protected abstract IWorkbook ExtractWorkbook( IDataObject dataObject, 
      IWorkbooks workbooks );
    /// <summary>
    /// Extracts the workbook.
    /// </summary>
    /// <param name="dataObject">Data object to extract.</param>
    protected abstract void FillDataObject( IDataObject dataObject );
    /// <summary>
    /// Extracts the workbook.
    /// </summary>
    /// <param name="dataObject">Data object to extract.</param>
    /// <param name="range">Range to copy into data object.</param>
    protected abstract void FillDataObject( IDataObject dataObject, IRange range );
    #endregion
  }
}
