#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;

using Syncfusion.CompoundFile.XlsIO;
using Syncfusion.CompoundFile.XlsIO.Native;

using Syncfusion.XlsIO.Interfaces;

using Syncfusion.XlsIO.Implementation.Security;
using Syncfusion.XlsIO.Implementation.Charts;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Implementation.PivotTables;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.Exceptions;
#endif

#if  (SILVERLIGHT || WP)
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.CompoundFile.XlsIO.Net;
#elif ( WINRT )
using Syncfusion.XlsIO;
using Syncfusion.CompoundFile.XlsIO.Net;
#else
using System.Drawing;
using Syncfusion.CompoundFile.XlsIO.Net;
#endif

namespace Syncfusion.XlsIO.Implementation
{
  public partial class WorkbookImpl
  {
    #region Internal classes
    /// <summary>
    /// Summary description for WorkbookExcel97Serializator.
    /// </summary>
    public class WorkbookExcel97Serializator : IWorkbookSerializator
    {
      #region Constants
      /// <summary>
      /// Maximum password length.
      /// </summary>
      private const int MaximumPassordLength = 15;
      #endregion

      #region Members
      /// <summary>
      /// Reserved shape ids.
      /// </summary>
      private IdReserver m_shapeIds;
      #endregion

      #region Constructors
      /// <summary>
      /// Default constructor.
      /// </summary>
      public WorkbookExcel97Serializator( IdReserver shapeIds )
      {
        m_shapeIds = shapeIds;
      }
      #endregion

      #region Methods
#if !(WINRT )
      /// <summary>
      /// Saves workbook into specified file.
      /// </summary>
      /// <param name="fullName">Destination file name.</param>
      /// <param name="book">Workbook to save.</param>
      /// <param name="saveType">Save type.</param>
      public void Serialize( string fullName, WorkbookImpl book, ExcelSaveType saveType )
      {
        if( fullName == null || fullName.Length == 0 )
          throw new ArgumentOutOfRangeException( "fullName" );

        if( book == null )
          throw new ArgumentNullException( "book" );

        IEncryptor encryptor = null;

        if( book.m_encryptionType != ExcelEncryptionType.None )
        {
          encryptor = CreateEncryptor( book.m_encryptionType, book );
        }

        OffsetArrayList records = new OffsetArrayList();
        Serialize( records, saveType, encryptor, book, null, false );
        records.UpdateBiffRecordsOffsets();

        // Force GC to release COM objects interfaces.
        //GC.Collect( GC.MaxGeneration );
        //GC.WaitForPendingFinalizers();

        // Create stream.
        //using( ICompoundFile compoundFile = CreateOrOpenStorage( fullName, book ) )
        //using( ICompoundFile compoundFile = book.AppImplementation.CreateCompoundFile() )
        using( ICompoundFile compoundFile = book.AppImplementation.CreateCompoundFile( fullName,
          STGM.STGM_CREATE | STGM.STGM_READWRITE | STGM.STGM_SHARE_EXCLUSIVE ) )
        {
          // TODO: this construction must be removed
          //if( book.m_workbookFile != null && compoundFile is Syncfusion.CompoundFile.XlsIO.Net.CompoundFile )
          //{
          //  ( compoundFile as Syncfusion.CompoundFile.XlsIO.Net.CompoundFile ).SetMiniStreams(
          //    book.m_workbookFile as Syncfusion.CompoundFile.XlsIO.Net.CompoundFile );
          //}

          SaveToStgStream( compoundFile.RootStorage, false, records, encryptor, book );
          compoundFile.Flush();
          //compoundFile.Save( fullName );
          //Debug.Fail( "Reassign variables" );
        }
      }
#endif
      /// <summary>
      /// Saves workbook into stream.
      /// </summary>
      /// <param name="stream">Stream to save into.</param>
      /// <param name="book">Workbook to save.</param>
      /// <param name="saveType">Save type (template or ordinary xls).</param>
      public void Serialize( Stream stream, WorkbookImpl book, ExcelSaveType saveType )
      {
        if( stream == null )
          throw new ArgumentNullException( "stream" );

        if( book == null )
          throw new ArgumentNullException( "book" );

        IEncryptor encryptor = null;

        if( book.m_encryptionType != ExcelEncryptionType.None )
        {
          encryptor = CreateEncryptor( book.m_encryptionType, book );
        }

        OffsetArrayList records = new OffsetArrayList();
        Serialize( records, saveType, encryptor, book, null, false );
        records.UpdateBiffRecordsOffsets();

        using( ICompoundFile file = book.AppImplementation.CreateCompoundFile() )
        {
          SaveToStgStream( file.RootStorage, false, records, encryptor, book );
          file.Save( stream );
        }
        //using( StgStream stgStream = StgStream.CreateStorageOnILockBytes() )
        //{
        //  stgStream.CreateStream( DEF_STREAM_NAME1 );
        //  SaveToStgStream( stgStream, false, records, encryptor, book );
        //  stgStream.Flush();
        //  stgStream.SaveILockBytesIntoStream( stream );
        //}
      }
      /// <summary>
      /// Saves all workbook records into specified array.
      /// </summary>
      /// <param name="records">Array that will receive all workbook records.</param>
      /// <param name="saveType">Format in which worksheet should be saved.</param>
      /// <param name="encryptor">Object that is used to encrypt data.</param>
      /// <param name="book">Workbook to serialize.</param>
      /// <param name="sheet">Worksheet that must be serialized when we are
      /// serializing data for clipboard.</param>
      /// <param name="forClipboard">Indicates whether we are serializing data
      /// for saving into file or into clipboard.</param>
      [CLSCompliant( false )]
      public void Serialize( OffsetArrayList records, ExcelSaveType saveType,
        IEncryptor encryptor, WorkbookImpl book, WorksheetImpl sheet, bool forClipboard )
      {
        OptimizeStyles( book );
        OptimizeExtFormats();
        OptimizeFonts();

        records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.BOF ) );

        if( !forClipboard )
        {
          if( saveType == ExcelSaveType.SaveAsTemplate )
          {
            records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.Template ) );
          }

          if( book.m_bWriteProtection )
            records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.WriteProtection ) );

          if( encryptor != null )
          {
            records.Add( encryptor.GetFilePassRecord() );
          }

          records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.InterfaceHdr ) );
          records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.MMS ) );
          records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.InterfaceEnd ) );

          WriteAccessRecord wrAcs = ( WriteAccessRecord )BiffRecordFactory.GetRecord(
            TBIFFRecord.WriteAccess );
          wrAcs.UserName = book.Author;
          records.Add( wrAcs );

          if( book.m_fileSharing != null )
            records.Add( book.m_fileSharing );
        }

        records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.Codepage ) );
        records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.DSF ) );
        records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.UnkBegin ) );

        #region Insert TableID information
        if( !forClipboard )
        {
          // Fill position of sheets in workbook.
          if( book.m_worksheets.Count == 0 )
            throw new ApplicationException( "Workbook must contains at least one worksheet" );

          TabIdRecord tabid = ( TabIdRecord )BiffRecordFactory.GetRecord( TBIFFRecord.TabId );
          tabid.TabIds = new ushort[ book.m_worksheets.Count + book.m_charts.Count ];

          for( int i = 0, len = book.m_worksheets.Count; i < len; i++ )
          {
            tabid.TabIds[ i ] = ( ushort )( book.m_worksheets[ i ].Index + 1 );
          }

          for( int j = book.m_worksheets.Count, len = tabid.TabIds.Length; j < len; j++ )
          {
            tabid.TabIds[ j ] = ( ushort )( j + 1 );
          }

          records.Add( tabid );
        }
        #endregion

        if( !forClipboard && ( book.Application.SkipOnSave & SkipExtRecords.Macros ) != SkipExtRecords.Macros )
        {
          // If macros are available in source file and user does not skip them,
          // set flag that output compound file contains macros sub-storage.
          if( book.m_bHasMacros )
          {
            records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.HasBasic ) );

            if( book.m_bMacrosDisable )
            {
              records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.UnkMacrosDisable ) );
            }

            CodeNameRecord code = ( CodeNameRecord )BiffRecordFactory.GetRecord( TBIFFRecord.CodeName );
            code.CodeName = book.CodeName;
            records.Add( code );
          }
        }

        records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.FnGroupCount ) );

        #region Workbook protection block
        if( !forClipboard )
        {
          WindowProtectRecord windowProtect = ( WindowProtectRecord )
            BiffRecordFactory.GetRecord( TBIFFRecord.WindowProtect );
          windowProtect.IsProtected = book.IsWindowProtection;

          records.Add( windowProtect );

          ProtectRecord protect = ( ProtectRecord )
            BiffRecordFactory.GetRecord( TBIFFRecord.Protect );
          protect.IsProtected = book.IsCellProtection;

          records.Add( protect );

          records.Add( book.Password );       //BiffRecordFactory.GetRecord( TBIFFRecord.Password ) );
          records.Add( book.ProtectionRev4 ); //BiffRecordFactory.GetRecord( TBIFFRecord.ProtectionRev4 ) );
          records.Add( book.PasswordRev4 );   //BiffRecordFactory.GetRecord( TBIFFRecord.PasswordRev4 ) );
        }
        else if( sheet != null )
        {
          OleSizeRecord oleSize = ( OleSizeRecord )BiffRecordFactory.GetRecord( TBIFFRecord.OleSize );
          oleSize.FirstRow = ( ushort )( sheet.FirstRow - 1 );
          oleSize.FirstColumn = ( byte )( sheet.FirstColumn - 1 );
          oleSize.LastRow = ( ushort )( sheet.LastRow - 1 );
          oleSize.LastColumn = ( byte )( sheet.LastColumn - 1 );
          records.Add( oleSize );
        }
        #endregion

        #region Insert Window One Setting
        if( !forClipboard )
        {
          records.Add( book.WindowOne );
        }
        else
        {
          WindowOneRecord windowOne = ( WindowOneRecord )book.WindowOne.Clone();
          windowOne.SelectedTab = 0;
          windowOne.DisplayedTab = 0;
          records.Add( windowOne );
        }
        #endregion

        if( !forClipboard )
        {
          records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.Backup ) );
          records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.HideObj ) );

          DateWindow1904Record date = ( DateWindow1904Record )
            BiffRecordFactory.GetRecord( TBIFFRecord.DateWindow1904 );
          date.Is1904Windowing = book.Date1904;

          records.Add( date );

          PrecisionRecord precision = ( PrecisionRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Precision );
          precision.IsPrecision = ( ushort )( book.PrecisionAsDisplayed ? 0 : 1 );
          records.Add( precision );

          records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.RefreshAll ) );
          records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.BookBool ) );
        }

        #region Compatibility Record
        if (book.m_compatibility == null)
            book.m_compatibility = new CompatibilityRecord();
        records.Add(book.m_compatibility);
        #endregion

        #region Insert Fonts records
        book.m_fonts.Serialize( records );
        #endregion

        #region Insert Format records
        book.m_rawFormats.Serialize( records );
        #endregion

        #region Insert ExtFormat records
        uint[] crcCache = InitCRC();
        for( int i = 0, len = book.m_extFormats.Count; i < len; i++ )
        {
          // Use 'as' to increase performance.
          ExtendedFormatImpl format = book.m_extFormats[ i ] as ExtendedFormatImpl;
          format.Serialize(records, crcCache);
          if(format.XFRecord!=null)
            format.XFRecord.XFIndex = (ushort)format.Index;
        }
        #endregion

        #region Insert XFCRC records
        ExtendedFormatCRC m_crcRecord=new ExtendedFormatCRC();
        m_crcRecord.XFCount = (ushort)book.m_extFormats.Count;
        m_crcRecord.CRCChecksum = book.crcValue;
        records.Add(m_crcRecord);
        #endregion

        #region Insert Extended X Format records
        for (int i = 0, len = book.m_extFormats.Count; i < len; i++)
        {
            // Use 'as' to increase performance.
            ExtendedFormatImpl format = book.m_extFormats[i] as ExtendedFormatImpl;
            format.SerializeXFormat(records);
        }
        #endregion

        #region Insert Style records
        for( int i = 0, len = book.m_styles.Count; i < len; i++ )
        {
          // Use 'as' to increase performance.
          StyleImpl style = book.m_styles[ i ] as StyleImpl;
          style.Serialize( records );
        }
        #endregion

        #region Insert Palette
        if( book.m_bOwnPalette )
        {
          PaletteRecord palette = ( PaletteRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Palette );

          PaletteRecord.TColor[] newPal = new PaletteRecord.TColor[ book.m_colors.Count - DEF_FIRST_USER_COLOR ];
          for( int i = 0; i < newPal.Length; i++ )
          {
            newPal[ i ].A = ( ( Color )book.m_colors[ i + DEF_FIRST_USER_COLOR ] ).A;
            newPal[ i ].R = ( ( Color )book.m_colors[ i + DEF_FIRST_USER_COLOR ] ).R;
            newPal[ i ].G = ( ( Color )book.m_colors[ i + DEF_FIRST_USER_COLOR ] ).G;
            newPal[ i ].B = ( ( Color )book.m_colors[ i + DEF_FIRST_USER_COLOR ] ).B;
          }

          palette.Colors = newPal;
          records.Add( palette );
        }

        #endregion

        SerializePivotCachesInfo( records, book );

        #region Insert UseSelFSRecord
        if( !forClipboard )
        {
          UseSelFSRecord selfs = ( UseSelFSRecord )BiffRecordFactory.GetRecord( TBIFFRecord.UseSelFS );
          selfs.Flags = book.m_bSelFSUsed;
          records.Add( selfs );
        }
        #endregion

        #region Insert BoundSheets
        if( !forClipboard || sheet == null )
        {
          for( int i = 0, len = book.m_arrObjects.Count; i < len; i++ )
          {
            INamedObject toSave = book.m_arrObjects[ i ];
            records.Add( CreateBoundSheet( toSave ) );
          }
        }
        else
        {
          BoundSheetRecord bound = CreateBoundSheet( ( INamedObject )sheet );
          bound.SheetIndex = 0;
          records.Add( bound );
        }
        #endregion

        CountryRecord country = ( CountryRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Country );
        country.DefaultCountry = ( ushort )book.m_iCountry;
        country.CurrentCountry = ( ushort )book.m_iCountry;
        records.Add( country );

        #region SuppBook && ExtSheetRec
        book.m_externBooks.Serialize( records );

        if( book.m_externSheet.RefList != null && book.m_externSheet.RefList.Count != 0 )
          records.Add( book.m_externSheet );
        #endregion

        if (book.m_continue != null)
        {
            foreach (ContinueRecord cont in book.m_continue)
            {
                records.Add(cont);
            }
        }

        #region Insert Name records
        SerializeNames( records, book );
        #endregion

#if !SILVERLIGHT && !WINRT && !WP

        if (book.EnabledCalcEngine == true)
        {
            book.m_reCalcId.CalcIdentifier = book.CalcIdentifier;
            records.Add(book.m_reCalcId);
        }
        else
        {
            records.Add(book.m_reCalcId);
        }
#else
          records.Add(book.m_reCalcId);
#endif

        if (book.m_drawGroup != null 
            && book.m_shapesData.Pictures.Count==0 
            && book.m_headerFooterPictures.PreservedClusters == null 
            && book.m_headerFooterPictures.Pictures.Count==0)
        {
            records.Add(book.m_drawGroup);
        }
        else if(!forClipboard)
        {
            SerializeMsoDrawings(records, book);
        }

        #region Fill SST Records
        // Extract strings and sort them by index.

        book.m_SSTDictionary.Serialize( records );
        #endregion

        if( book.m_bookExt != null )
          records.Add( book.m_bookExt );

        records.AddList(book.PreserveExternalConnectionDetails);

        records.Add( BiffRecordFactory.GetRecord( TBIFFRecord.EOF ) );

        // Save each worksheet into array.
        if( !forClipboard || sheet == null )
        {
          for( int i = 0, len = book.m_arrObjects.Count; i < len; i++ )
          {
            //ISerializableNamedObject obj = m_arrObjects[ i ];
            WorksheetBaseImpl toSerialize = ( WorksheetBaseImpl )book.m_arrObjects[ i ];
            toSerialize.Serialize( records );
          }
        }
        else
        {
          sheet.SerializeForClipboard( records );
        }
      }
      /// <summary>
      /// Serializes pivot caches info.
      /// </summary>
      /// <param name="records">Records to serialize into.</param>
      /// <param name="book">Worbook to get data from.</param>
      private void SerializePivotCachesInfo( OffsetArrayList records, WorkbookImpl book )
      {
        if( records == null )
          throw new ArgumentNullException( "records" );

        if( book == null )
          throw new ArgumentNullException( "book" );

        if( book.m_pivotCaches == null )
          return;

        //for( int i = 0, len = book.m_pivotCaches.Count; i < len; i++ )
        foreach( int index in book.m_pivotCaches.Order )
        {
          PivotCacheImpl cache = book.m_pivotCaches[ index ];
          PivotCacheInfo info = cache.Info;

          // Some caches could have no description inside workbook part of the document.
          if( info != null )
          {
            //listInfo.Insert( 0, info );
            info.Serialize( records );
          }
        }
      }
      /// <summary>
      /// Serializes all mso drawings (picture, header/footer images, etc.) if necessary.
      /// </summary>
      /// <param name="records">Record's list to serialize into.</param>
      /// <param name="book">Workbook to serialize.</param>
      private void SerializeMsoDrawings( OffsetArrayList records, WorkbookImpl book )
      {
        if( records == null )
          throw new ArgumentNullException( "records" );

        // TODO: add this property
        //if( NeedHeaders )
        if( book.m_headerFooterPictures != null )
        {
          book.m_headerFooterPictures.SerializeMsoDrawingGroup( records, TBIFFRecord.HeaderFooterImage, null );
        }

        if( book.m_shapesData != null && ( book.Application.SkipOnSave
          & SkipExtRecords.Drawings ) != SkipExtRecords.Drawings )
        {
          book.m_shapesData.SerializeMsoDrawingGroup( records, TBIFFRecord.MSODrawingGroup, m_shapeIds );
        }
      }
      /// <summary>
      /// Creates encryptor.
      /// </summary>
      /// <param name="encryptionType">Encryption type to use.</param>
      /// <param name="book">Workbook to serialize.</param>
      /// <returns>Created encryptor.</returns>
      private IEncryptor CreateEncryptor( ExcelEncryptionType encryptionType, WorkbookImpl book )
      {
        if( encryptionType == ExcelEncryptionType.Standard )
        {
          IEncryptor encryptor = new MD5Decryptor();

          if( book.m_arrDocId == null )
          {
            Guid docId = Guid.NewGuid();
            book.m_arrDocId = docId.ToByteArray();
          }

          string strPassword = ( book.m_strEncryptionPassword != null ) ?
            book.m_strEncryptionPassword :
            StandardPassword;

          if( strPassword != null && strPassword.Length > MaximumPassordLength )
            throw new ArgumentOutOfRangeException( "PasswordToOpen", "Password too long. Maximum password length is 15 characters." );

          encryptor.SetEncryptionInfo( book.m_arrDocId, strPassword );
          return encryptor;
        }
        else
        {
          throw new NotSupportedException( "Not supported encryption type." );
        }
      }
      /// <summary>
      /// This function is looking for unused formats and for formats
      /// that have equal data.
      /// </summary>
      private void OptimizeExtFormats()
      {
      }
      /// <summary>
      /// Removes unused or equal fonts from internal lists. Also will be updated styles
      /// and formats which have references on unused or optimized fonts.
      /// </summary>
      private void OptimizeFonts()
      {
      }
      /// <summary>
      /// Method remove from list equal styles.
      /// </summary>
      /// <param name="book">Workbook to serialize.</param>
      private void OptimizeStyles( WorkbookImpl book )
      {
      }
      /// <summary>
      /// Creates BoundSheetRecord that corresponds to the specified worksheet.
      /// </summary>
      /// <param name="namedObject">INamedObject the BoundSheetRecord is created for.</param>
      /// <returns>Created BoundSheetRecord.</returns>
      private BoundSheetRecord CreateBoundSheet( INamedObject namedObject )
      {
        BoundSheetRecord bound = ( BoundSheetRecord )BiffRecordFactory.GetRecord(
          TBIFFRecord.BoundSheet );

        bound.SheetName = namedObject.Name;

        if( namedObject is IWorksheet )
        {
          FillBoundSheet( bound, ( WorksheetImpl ) namedObject );
        }
        else if( namedObject is IChart )
        {
          FillBoundSheet( bound, ( ChartImpl ) namedObject );
        }
        else throw new ArgumentOutOfRangeException( "namedObject has wrong type" );

        return bound;
      }
      /// <summary>
      /// Saves workbook into compound storage.
      /// </summary>
      /// <param name="storage">Storage to save workbook into.</param>
      /// <param name="bDisposeAfterSave">Indicates whether </param>
      /// <param name="records">Record collection that should be filled during serialization</param>
      /// <param name="encryptor">Object that is used to encrypt data.</param>
      /// <param name="book">Workbook to serialize.</param>
      private void SaveToStgStream( ICompoundStorage storage, bool bDisposeAfterSave,
        OffsetArrayList records, IEncryptor encryptor, WorkbookImpl book )
      {
        if( storage == null )
          throw new ArgumentNullException( "storage" );
#if MEASURE_PERFORMANCE
      DateTime start = DateTime.Now;
#endif
        if(book.CalculationOptions.CalculationMode == ExcelCalculationMode.Automatic ||
          book.CalculationOptions.CalculationMode == ExcelCalculationMode.AutomaticExceptTables )
        {
          for( int i = 0, len = book.Worksheets.Count; i < len; i++ )
          {
            WorksheetImpl sheet = book.Worksheets[ i ] as WorksheetImpl;
            if (!sheet.ParseOnDemand)
                sheet.CellRecords.Table.UpdateFormulaFlags();
          }
        }

        CompoundStream stream = null;

        try
        {
          stream = storage.CreateStream( DEF_STREAM_NAME1 );
          using( BiffWriter writer = new BiffWriter( stream, true ) )
          {
            writer.WriteRecord( records, encryptor );
          }

#if MEASURE_PERFORMANCE
        TimeSpan writeRecordTime = DateTime.Now - start;
        Debug.WriteLine( writeRecordTime, "Time to write all records" );
        Console.WriteLine( "Time to write all records: {0}", writeRecordTime );
        start = DateTime.Now;
#endif
          WritePivotCaches( storage, book, encryptor );
          CopySourceData( storage, book );
#if !SILVERLIGHT && !WINRT && !WP
          CopyOleData(storage, book);
#endif

          //if( storage.ContainsStorage( "MsoDataStore" ) )
          //{
          //  storage.DeleteStorage( "MsoDataStore" );
          //}

          //SerializeControlProperties( storage, book );
          //if( !book.m_bReadFromStream )
          //{
          //  CopySourceFileData( stream.FileName, stream, book );
          //}
          //else
          //{
          //  if( !( book.m_workbookStorage is StgStream ) )
          //    book.m_workbookStorage.Position = book.m_lStreamPos;

          //  CopySourceStreamData( book.m_workbookStorage, stream, false, book );
          //}

#if MEASURE_PERFORMANCE
        TimeSpan sourceDataCopy = DateTime.Now - start;
        Debug.WriteLine( sourceDataCopy, "Time to copy source data" );
        Console.WriteLine( "Time to copy source data: {0}", sourceDataCopy );
#endif
          
          // NOTE: there is some problem with pivot records serialization (issue 9637)
          //if( book.m_pivotCaches != null )
          //{
          //  book.m_pivotCaches.Serialize( storage, encryptor );
          //}

          bool bDocumentPropertiesSerialized = false;

#if MEASURE_PERFORMANCE
        start = DateTime.Now;
#endif
#if !SILVERLIGHT && !WINRT && !WP
          //throw new NotImplementedException();
          //Debug.Fail( "Document properties" );
          Storage nativeStorage = storage as Storage;
          // Create Summary information for document.
#endif

#if AllowUnsafeCode
          if( nativeStorage != null )
          {
            SerializeDocumentPropertiesNative( book, nativeStorage );
            bDocumentPropertiesSerialized = true;

            if( book.m_controlsStream != null )
            {
              CompoundStream controlsStream = /*storage.ContainsStream( "Ctls" ) ?
                storage.OpenStream( "Ctls",  ) :*/
                storage.CreateStream( "Ctls" );

              book.m_controlsStream.Position = 0;
              controlsStream.Position = 0;
              UtilityMethods.CopyStreamTo( book.m_controlsStream, controlsStream );
              controlsStream.Flush();
              controlsStream.Close();
            }
          }
#endif

          if( !bDocumentPropertiesSerialized )
            SerializeDocumentPropertiesManaged( book, storage );

          if (book.CustomXmlparts != null && book.CustomXmlparts.Count > 0)
          {
              MsoDataStore store = new MsoDataStore(storage, book);
              store.SerializeMetaStore();
          }

#if MEASURE_PERFORMANCE
        TimeSpan propertiesTime = DateTime.Now - start;
        Debug.WriteLine( propertiesTime, "Properties serialization time" );
        Console.WriteLine( "Properties serialization time: {0}", propertiesTime );
#endif
        }
        catch( Exception ex )
        {
          Debug.WriteLine( "Exception occured" );
          Debug.WriteLine( ex.Message, "Message" );
          Debug.WriteLine( ex.StackTrace, "Stack trace" );
          throw;
        }
        finally
        {
          //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "SaveToStgStream completed." );

          //stream.Flush();
          //stream.Dispose();
          storage.Flush();

          if( bDisposeAfterSave )
          {
            storage.Dispose();
            //stream = null;
          }
        }
      }

#if !(SILVERLIGHT) && !(WINRT) && !(WP)
      private void SerializeDocumentPropertiesNative( WorkbookImpl book, Storage nativeStorage )
      {
        if( ( book.Application.SkipOnSave & SkipExtRecords.SummaryInfo ) != SkipExtRecords.SummaryInfo )
        {
          IPropertySetStorage setProp = null;
          Guid guidPropSet = new Guid( "0000013a-0000-0000-c000-000000000046" );

          int error = Syncfusion.CompoundFile.XlsIO.Native.API.StgCreatePropSetStg( nativeStorage.COMStorage, 0, out setProp );

          if( error != 0 )
            throw new ExternalException( "Cannot create Storage properties stream", error );

          //#if MEASURE_PERFORMANCE
          //          WorksheetPerfCounter counter = new WorksheetPerfCounter();
          //          counter.Start();
          //#endif

          book.m_builtInDocumentProperties.Serialize( setProp );
          book.m_customDocumentProperties.Serialize( setProp );

          //#if MEASURE_PERFORMANCE
          //          float result = counter.Finish();
          //          Console.WriteLine( "Serialization time {0}", result );
          //#endif

          Marshal.FinalReleaseComObject( setProp );
        }
      }
      private void CopyOleData(ICompoundStorage storage, WorkbookImpl book)
      {
          if (book.HasOleObjects && book.IsOleObjectCopied)
          {
              foreach (string storageName in book.OleStorageCollection.OleStoragesNames)
              {
                  OleStorage OleStorage = book.OleStorageCollection.OpenStorage(storageName);

                  ICompoundStorage compoundStorage = storage.CreateStorage(storageName);

                  foreach (string streamName in OleStorage.StreamNames)
                  {
                      MemoryStream memoryStream = OleStorage.OpenStream(streamName);

                      CompoundStream controlsStream = compoundStorage.CreateStream(streamName);

                      controlsStream.Position = 0;
                      memoryStream.Position = 0;

                      UtilityMethods.CopyStreamTo(memoryStream, controlsStream);
                      controlsStream.Flush();
                      controlsStream.Close();
                  }

              }
              foreach (string streamName in book.OleStorageCollection.ArrayStreamNames)
              {
                  MemoryStream memoryStream = book.OleStorageCollection.OpenStream(streamName);

                  CompoundStream controlsStream = storage.CreateStream(streamName);

                  controlsStream.Position = 0;
                  memoryStream.Position = 0;

                  UtilityMethods.CopyStreamTo(memoryStream, controlsStream);
                  controlsStream.Flush();
                  controlsStream.Close();
              }
          }
      }
#endif
      private void SerializeDocumentPropertiesManaged( WorkbookImpl book, ICompoundStorage storage )
      {
        // Step 1.
        // Create collections and sections
        DocumentPropertyCollection summary = new DocumentPropertyCollection();
        PropertySection summarySection = new PropertySection( Collections.BuiltInDocumentProperties.GuidSummary, -1 );
        summary.Sections.Add( summarySection );

        DocumentPropertyCollection documentSummary = new DocumentPropertyCollection();
        PropertySection documentSummarySection = new PropertySection( Collections.BuiltInDocumentProperties.GuidDocument, -1 );
        PropertySection customSection = new PropertySection( Collections.CustomDocumentProperties.GuidCustom, -1 );
        documentSummary.Sections.Add( documentSummarySection );
        documentSummary.Sections.Add( customSection );

        // Step 2.
        // Fill collections and sections
        book.m_builtInDocumentProperties.Serialize( summarySection, documentSummarySection );
        book.m_customDocumentProperties.Serialize( customSection );

        // Step 3.
        // Save them into corresponding streams.
        if( storage.ContainsStream( WorkbookImpl.DEF_SUMMARY_INFO ) )
          storage.DeleteStream( WorkbookImpl.DEF_SUMMARY_INFO );

        Stream stream = storage.CreateStream( WorkbookImpl.DEF_SUMMARY_INFO );

        stream.Position = 0;
        summary.Serialize( stream );
        stream.Close();

        if( storage.ContainsStream( WorkbookImpl.DEF_DOCUMENT_SUMMARY_INFO ) )
          storage.DeleteStream( WorkbookImpl.DEF_DOCUMENT_SUMMARY_INFO );

        stream = storage.CreateStream( WorkbookImpl.DEF_DOCUMENT_SUMMARY_INFO );

        stream.Position = 0;
        documentSummary.Serialize( stream );
        stream.Close();
      }

      private void SerializeControlProperties( ICompoundStorage storage, WorkbookImpl book )
      {
        //IWorksheets sheets = book.Worksheets;
        //CompoundStream stream = null;

        //for( int i = 0, len = sheets.Count; i < len; i++ )
        //{
        //  WorksheetImpl sheet = ( WorksheetImpl )sheets[ i ];
        //  TextBoxCollection textBoxes = sheet.InnerTextBoxes;

        //  if( textBoxes != null && textBoxes.Count > 0 )
        //  {
        //    for( int j = 0, lenJ = textBoxes.Count; j < lenJ; j++ )
        //    {
        //      TextBoxShapeImpl textBox = ( TextBoxShapeImpl )textBoxes[ j ];

        //      if( stream == null )
        //      {
        //        stream = storage.CreateStream( ControlPropertiesList.StreamName );
        //      }

        //      textBox.Properties.Serialize( stream );
        //    }
        //  }
        //}

        //if( stream != null )
        //  stream.Dispose();
      }
      private void WritePivotCaches( ICompoundStorage storage, WorkbookImpl book, IEncryptor encryptor )
      {
        PivotCacheCollection caches = book.PivotCaches;

        if( caches != null && caches.Count > 0 )
        {
          using( ICompoundStorage pivotStorage = storage.CreateStorage( PivotCacheCollection.DEF_PIVOT_CACHE_STORAGE ) )
          {
            foreach( PivotCacheImpl cache in caches )
            {
              string streamName = ( cache.StreamId ).ToString( "X4" );

              using( CompoundStream stream = pivotStorage.CreateStream( streamName ) )
              {
                cache.Serialize( stream, encryptor );
              }
            }
          }
        }
      }
      /// <summary>
      /// Copies storage subitems.
      /// </summary>
      /// <param name="storage">Storage to copy into.</param>
      /// <param name="book">Parent workbook.</param>
      private void CopySourceData( ICompoundStorage storage, WorkbookImpl book )
      {
        if( storage == null )
          throw new ArgumentNullException( "storage" );

        if( book == null )
          throw new ArgumentNullException( "book" );

        if( book.m_workbookFile != null )
        {
          ICompoundStorage root = book.m_workbookFile.RootStorage;
          CopySourceSubstorages( storage, root );
          CopySourceSubstreams( storage, root );
        }
      }
      /// <summary>
      /// Copies substorages.
      /// </summary>
      /// <param name="storage">Storage to copy into.</param>
      /// <param name="sourceStorage">Storage to copy from.</param>
      private void CopySourceSubstorages( ICompoundStorage storage, ICompoundStorage sourceStorage )
      {
        if( storage == null )
          throw new ArgumentNullException( "storage" );

        if( sourceStorage == null )
          throw new ArgumentNullException( "sourceStorage" );

        string[] arrStorages = sourceStorage.Storages;

        for( int i = 0, len = arrStorages.Length; i < len; i++ )
        {
          //storage.Co
          if( arrStorages[ i ] != PivotCacheCollection.DEF_PIVOT_CACHE_STORAGE && arrStorages[ i ] !="MsoDataStore" )
          {
            using( ICompoundStorage storageToCopy = sourceStorage.OpenStorage( arrStorages[ i ] ) )
            {
              storage.InsertCopy( storageToCopy );
            }
          }
        }
      }
      /// <summary>
      /// Copies substreams.
      /// </summary>
      /// <param name="storage">Storage to copy into.</param>
      /// <param name="sourceStorage">Storage to copy from.</param>
      private void CopySourceSubstreams( ICompoundStorage storage, ICompoundStorage sourceStorage )
      {
        if( storage == null )
          throw new ArgumentNullException( "storage" );

        if( sourceStorage == null )
          throw new ArgumentNullException( "sourceStorage" );

        string[] arrStreams = sourceStorage.Streams;

        for( int i = 0, len = arrStreams.Length; i < len; i++ )
        {
          string streamName = arrStreams[ i ];

          //if( streamName != WorkbookImpl.DEF_STREAM_NAME1 )
          if( FindStreamCaseInsensitive( storage, streamName ) == null )
          {
            using( CompoundStream compoundStream = sourceStorage.OpenStream( streamName ) )
            {
              storage.InsertCopy( compoundStream );
            }
          }
        }
      }
      /// <summary>
      /// Serializes all names defined in the workbook.
      /// </summary>
      /// <param name="records">OffsetArrayList that will receive all records.</param>
      /// <param name="book">Workbook to serialize names from.</param>
      /// <exception cref="System.ArgumentNullException">
      /// When specified OffsetArrayList is NULL.
      /// </exception>
      private void SerializeNames( OffsetArrayList records, WorkbookImpl book )
      {
        if( records == null )
          throw new ArgumentNullException( "records" );

        book.m_names.Serialize( records );
      }
      /// <summary>
      /// Fill BoundSheetRecord that corresponds to the specified worksheet.
      /// </summary>
      /// <param name="bound">Record to fill.</param>
      /// <param name="worksheet">Worksheet the BoundSheetRecord is created for.</param>
      private void FillBoundSheet( BoundSheetRecord bound, WorksheetImpl worksheet )
      {
        bound.SheetIndex = worksheet.RealIndex;
        bound.Visibility = worksheet.Visibility;
        bound.BoundSheetType = ( BoundSheetRecord.SheetType )worksheet.Type;
        bound.BOF = worksheet.BOF;
      }
      /// <summary>
      /// Fills BoundSheetRecord that corresponds to the specified worksheet.
      /// </summary>
      /// <param name="bound">Record to fill.</param>
      /// <param name="chart">Chart the BoundSheetRecord is created for.</param>
      /// <returns>Created BoundSheetRecord.</returns>
      private void FillBoundSheet( BoundSheetRecord bound, ChartImpl chart )
      {
        bound.SheetIndex = chart.RealIndex;
        bound.Visibility = chart.Visibility;//WorksheetVisibility.Visible;
        bound.BoundSheetType = BoundSheetRecord.SheetType.Chart;
        bound.BOF = chart.BOF;
      }
      #endregion
    }
    #endregion
  }
}
