#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

#region file using directives
using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

using Syncfusion.XlsIO.Implementation;
using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.CompoundFile.XlsIO;
//using Syncfusion.XlsIO.IO;
//using Syncfusion.XlsIO.IO.Stream;
#endregion

namespace Syncfusion.XlsIO.Implementation.Clipboard
{
  /// <summary>
  /// Creates Clipboard provider for the Biff8 format.
  /// </summary>
  public class Biff8ClipboardProvider : ClipboardProvider
  {
    #region Class constants
    /// <summary>
    /// Default name of the format.
    /// </summary>
    public const string DEF_BIFF8_FORMAT = "Biff8";
    #endregion

    #region Class constructors
    /// <summary>
    /// Default constructor.
    /// </summary>
    public Biff8ClipboardProvider()
      : this( null )
    {
    }
    /// <summary>
    /// Creates provider and sets its Next property to the specified value.
    /// </summary>
    /// <param name="next">Next clipboard provider in the provider's list.</param>
    public Biff8ClipboardProvider( ClipboardProvider next )
      : base( ( IWorkbook )null, next )
    {
      FormatName = DEF_BIFF8_FORMAT;
    }
    /// <summary>
    /// Creates provider and sets its Next property to the specified value.
    /// </summary>
    /// <param name="sheet">Worksheet to copy to the clipboard.</param>
    /// <param name="next">Next clipboard provider in the provider's list.</param>
    public Biff8ClipboardProvider( IWorksheet sheet, ClipboardProvider next )
      : base( sheet, next )
    {
      FormatName = DEF_BIFF8_FORMAT;
    }
    #endregion

    #region Not Public Class Methods
    /// <summary>
    /// Extracts workbook from the data object.
    /// </summary>
    /// <param name="dataObject">Data object that contains workbook data.</param>
    /// <param name="workbooks">Workbooks collection to add workbook to.</param>
    /// <returns>Extracted workbook.</returns>
    protected override IWorkbook ExtractWorkbook( IDataObject dataObject, IWorkbooks workbooks )
    {
      if( dataObject != null )
      {
        if( dataObject.GetDataPresent( "Biff8", true ) )
        {
          // Get Biff8 format with conversion if needed.
          object objData = dataObject.GetData( "Biff8", true );
          
          if( objData != null )
          {
            Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "type: " + objData.GetType().FullName, "Data storage" );
            Stream stream = ( Stream )objData;
            return workbooks.Open( stream, ExcelParseOptions.Default );
          }
        }
      }
      
      return null;
    }
    /// <summary>
    /// Fills the data object.
    /// </summary>
    /// <param name="dataObject">DataObject to filling.</param>
    protected override void FillDataObject( IDataObject dataObject )
    {
      if( dataObject == null )
        throw new ArgumentNullException( "dataObject" );

      dataObject.SetData( FormatName, GetData() );
    }
    /// <summary>
    /// Extracts the workbook.
    /// </summary>
    /// <param name="dataObject">Data object to extract.</param>
    /// <param name="range">Range to copy into data object.</param>
    protected override void FillDataObject(IDataObject dataObject, IRange range)
    {
    }

    /// <summary>
    /// Returns data for copying into clipboard.
    /// </summary>
    /// <returns>MemoryStream with data to copy into clipboard.</returns>
    private MemoryStream GetData()
    {
      WorkbookImpl book = ( WorkbookImpl )Workbook;
      OffsetArrayList records = new OffsetArrayList();
      book.SerializeForClipboard( records, ( WorksheetImpl )Worksheet );
      records.UpdateBiffRecordsOffsets();

      MemoryStream memStream = new MemoryStream();

      //using( StgStream stream = StgStream.CreateStorageOnILockBytes() )
      using( ICompoundFile compoundFile = book.AppImplementation.CreateCompoundFile() )
      {
        ICompoundStorage storage = compoundFile.RootStorage;
        using( CompoundStream stream = storage.CreateStream( WorkbookImpl.DEF_STREAM_NAME1 ) )
        {

          using( BiffWriter writer = new BiffWriter( stream, false ) )
          {
            // No encryption is needed when we write into clipboard, so encryptor is always null.
            writer.WriteRecord( records, null );
          }

          stream.Flush();
        }

        //stream.SaveILockBytesIntoStream( memStream );
        compoundFile.Save( memStream );
      }

      return memStream;
    }
    #endregion
    
    #region Class methods
    /// <summary>
    /// Returns IDataObject (for copying to the clipboard)
    /// that contains data from workbook or worksheet.
    /// </summary>
    /// <returns>IDataObject for copying to the clipboard.</returns>
    /// <exception cref="System.NotImplementedException">
    /// This method is not implemented yet.
    /// </exception>
    public override System.Windows.Forms.IDataObject GetForClipboard()
    {
      MemoryStream stream = GetData();
      return new DataObject( FormatName, stream );
    }
    /// <summary>
    /// Returns IDataObject (for copying to the clipboard)
    /// that contains data from workbook or worksheet.
    /// </summary>
    /// <param name="range">Range to copy to the clipboard.</param>
    /// <returns>IDataObject for copying to the clipboard.</returns>
    /// <exception cref="System.NotImplementedException">
    /// This method is not implemented yet.
    /// </exception>
    public override System.Windows.Forms.IDataObject GetForClipboard( IRange range )
    {
      //throw new NotImplementedException( "GetForClipboard" );
      Debug.Fail( "GetForClipboard in Biff8 format is not supported for ranges" );
      return new DataObject();
    }
    #endregion
  }
}