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

//#define MEASURE_PERFORMANCE

#region file using directives
using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;

using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.Collections.Grouping;
using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Implementation.XmlSerialization;
using Syncfusion.XlsIO.Implementation.XmlReaders;
using Syncfusion.XlsIO.Implementation.Security;

using Syncfusion.XlsIO;
using Syncfusion.XlsIO.Implementation.Charts;
using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Interfaces.XmlSerialization;

using FOPTE = Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing.MsofbtOPT.FOPTE;
using TRuns = Syncfusion.XlsIO.Parser.Biff_Records.Charts.ChartAlrunsRecord.TRuns;
using System.Collections.Generic;
using Syncfusion.CompoundFile.XlsIO;
using Syncfusion.CompoundFile.XlsIO.Net;
using Syncfusion.XlsIO.Implementation.PivotTables;
using Syncfusion.XlsIO.Implementation.Sorting;

#if ( WINRT || WP)
using System.Windows;
using System.Threading.Tasks;
using Windows.Storage;
#endif

#if WINRT
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
#endif


#if  SILVERLIGHT || WP
using System.Windows;
using System.Windows.Media;
#if SILVERLIGHT
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using Syncfusion.XlsIO.Implementation.WP;
#endif
#endif

#if !(SILVERLIGHT) && !(WINRT) && !(WP)
using System.Drawing;
using System.Data;
using Syncfusion.CompoundFile.XlsIO.Native;
using System.Windows.Forms;
using System.Web;
using Syncfusion.XlsIO.Implementation.Clipboard;
#endif

#endregion

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// This class represents an Excel Workbook.
  /// </summary>
  public partial class WorkbookImpl
    : CommonObject
    , IWorkbook
  {
    #region Skipped
#if SKIPPED
    /// <summary>
    /// Returns or sets colors in the palette for the workbook. The palette has 
    /// 56 entries, each represented by an RGB value. Read/write Variant.
    /// </summary>
    public object       Colors
    {
      get
      {
        // TODO: Add WorkbookImpl.Colors getter implementation.
        throw new NotImplementedException();
      }
      set
      {
        // TODO: Add WorkbookImpl.Colors setter implementation.
        throw new NotImplementedException();
      }
    }
   
    /// <summary>
    /// Returns a Comments collection that represents all the comments for the 
    /// specified worksheet. Read-only.
    /// </summary>
    public IComments    Comments
    {
      get
      {
        // TODO: Add WorkbookImpl.Comments getter implementation.
        throw new NotImplementedException();
      }
      set
      {
        // TODO: Add WorkbookImpl.Comments setter implementation.
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Returns the name of the object, including its path on disk, as a 
    /// string. Read-only String.
    /// </summary>
    public string       FullName
    {
      get
      {
        // TODO: Add WorkbookImpl.FullName getter implementation.
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Returns the complete path to the application, excluding the final 
    /// separator and name of the application. Read-only String.
    /// </summary>
    public string       Path
    {
      get
      {
        // TODO: Add WorkbookImpl.Path getter implementation.
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// True if personal information can be removed from the specified 
    /// workbook. The default value is False. Read/write Boolean.
    /// </summary>
    public bool         RemovePersonalInformation
    {
      get
      {
        // TODO: Add WorkbookImpl.RemovePersonalInformation getter implementation.
        throw new NotImplementedException();
      }
      set
      {
        // TODO: Add WorkbookImpl.RemovePersonalInformation setter implementation.
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// True if Microsoft Excel saves external link values with the 
    /// workbook. Read/write Boolean.
    /// </summary>
    public bool         SaveLinkValues
    {
      get
      {
        // TODO: Add WorkbookImpl.SaveLinkValues getter implementation.
        throw new NotImplementedException();
      }
      set
      {
        // TODO: Add WorkbookImpl.SaveLinkValues setter implementation.
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Deletes a custom number format from the workbook.
    /// </summary>
    /// <param name="NumberFormat"></param>
    public void DeleteNumberFormat( string NumberFormat )
    {
      // TODO: Add WorkbookImpl.DeleteNumberFormat implementation.
      throw new NotImplementedException();
    }

    /// <summary>
    /// Refreshes all external data ranges and PivotTable reports in the 
    /// specified workbook.
    /// </summary>
    public void RefreshAll()
    {
      // TODO: Add WorkbookImpl.RefreshAll implementation.
      throw new NotImplementedException();
    }

    /// <summary>
    /// Resets the color palette to the default colors.
    /// </summary>
    public void ResetColors()
    {
      // TODO: Add WorkbookImpl.ResetColors implementation.
      throw new NotImplementedException();
    }

    /// <summary>
    /// Saves a copy of the workbook to a file but doesn't modify the 
    /// open workbook in memory.
    /// </summary>
    /// <param name="Filename"></param>
    public void SaveCopyAs( string Filename )
    {
      // TODO: Add WorkbookImpl.SaveCopyAs implementation.
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    public void UpdateFromFile()
    {
      // TODO: Add WorkbookImpl.UpdateFromFile implementation.
      throw new NotImplementedException();
    }
    /// <summary>
    /// Saves changes to the workbook in a different file.
    /// </summary>
    /// <param name="Filename"></param>
    /// <param name="FileFormat"></param>
    /// <param name="Password"></param>
    /// <param name="WriteResPassword"></param>
    /// <param name="ReadOnlyRecommended"></param>
    /// <param name="CreateBackup"></param>
    /// <param name="AccessMode"></param>
    /// <param name="ConflictResolution"></param>
    /// <param name="AddToMru"></param>
    /// <param name="TextCodepage"></param>
    /// <param name="TextVisualLayout"></param>
    /// <param name="Local"></param>
    public void SaveAs( string Filename, object FileFormat, 
      object Password, object WriteResPassword, 
      object ReadOnlyRecommended, object CreateBackup, 
      XlSaveAsAccessMode AccessMode, object ConflictResolution, 
      object AddToMru, object TextCodepage, 
      object TextVisualLayout, object Local )
    {
      SaveAs( Filename );
    }

#endif

    #endregion

    #region Class constants
    /// <summary>
    /// Stream name that represent summary name.
    /// </summary>
    private const string DEF_SUMMARY_INFO = "\x5SummaryInformation";
    /// <summary>
    /// Stream name that represent document summary name.
    /// </summary>
    private const string DEF_DOCUMENT_SUMMARY_INFO = "\x0005DocumentSummaryInformation";
    /// <summary>
    /// Stream name used by new versions of Excel.
    /// </summary>
    internal const string DEF_STREAM_NAME1 = "Workbook";
    /// <summary>
    /// Old styled name of stream in Excel binary file.
    /// </summary>
    private const string DEF_STREAM_NAME2 = "Book";
    /// <summary>
    /// Name of sub-storage in input file which contains macros of opened file.
    /// </summary>
    private const string DEF_VBA_MACROS = "_VBA_PROJECT_CUR";
    /// <summary>
    /// Sub-storage of Macros storage. Used for checks is file format correct 
    /// or not.
    /// </summary>
    private const string DEF_VBA_SUB_STORAGE = "VBA";
    /// <summary>
    /// Self-referential external reference.
    /// </summary>
    private const char DEF_CHAR_SELF = '\x02';
    /// <summary>
    /// File name has been excoded.
    /// </summary>
    private const char DEF_CHAR_CODED = '\x01';
    /// <summary>
    /// Reference to an empty workbook name.
    /// </summary>
    private const char DEF_CHAR_EMPTY = '\x00';
    /// <summary>
    /// Represents an MS-DOS drive letter. It is followed by the drive letter.
    /// For example, the formula ='D:\SALES.XLS'!A1 generates this character key
    /// when the dependent workbook is not on the D drive. UNC file names, such as
    /// \\server\share\myfile.xls, generate an @ character after this character key;
    /// this replaces the initial double backslash (\\).
    /// </summary>
    private const char DEF_CHAR_VOLUME        = '\x01';
    /// <summary>
    /// Indicates that the source workbook is on the same drive as the 
    /// dependent workbook (the drive letter is omitted). For example
    /// the formula ='\SALES.XLS'!A1 generates this key when
    /// the dependent workbook is not in the root directory.
    /// </summary>
    private const char DEF_CHAR_SAMEVOLUME    = '\x02';
    /// <summary>
    /// Indicates that the source workbook is in a subdirectory of the current directory.
    /// For example, the formula ='XL\SALES.XLS'!A1 generates the DEF_CHAR_DOWNDIR key.
    /// The subdirectory name precedes the DEF_CHAR_DOWNDIR key, and the file
    /// name follows it.
    /// </summary>
    private const char DEF_CHAR_DOWNDIR       = '\x03';
    /// <summary>
    /// Indicates that the source workbook is in the parent directory of the current directory.
    /// For example, the formula ='..\SALES.XLS'!A1 generates the DEF_CHAR_UPDIR key.
    /// </summary>
    private const char DEF_CHAR_UPDIR         = '\x04';
    /// <summary>
    /// Not used.
    /// </summary>
    private const char DEF_CHAR_LONGVOLUME    = '\x05';
    /// <summary>
    /// Indicates that the source workbook is in the startup directory
    /// (the Xlstart subdirectory of the directory that contains Excel.exe).
    /// </summary>
    private const char DEF_CHAR_STARTUPDIR    = '\x06';
    /// <summary>
    /// Indicates that the source workbook is in the alternate startup directory.
    /// </summary>
    private const char DEF_CHAR_ALTSTARTUPDIR = '\x07';
    /// <summary>
    /// Indicates that the source workbook is in the Library directory.
    /// </summary>
    private const char DEF_CHAR_LIBDIR        = '\x08';
    /// <summary>
    /// Indicates that path is UNC file name (is replaced by \\).
    /// </summary>
    private const char DEF_CHAR_NETWORKPATH   = '@';
    /// <summary>
    /// Start of the UNC network path.
    /// </summary>
    private const string DEF_NETWORKPATH_START = @"\\";
    /// <summary>
    /// Indicates that current workbook is not protected.
    /// </summary>
    private const int DEF_NOT_PASSWORD_PROTECTION = 0;
    /// <summary>
    /// Index of the removed sheet.
    /// </summary>
    internal const int DEF_REMOVED_SHEET_INDEX = 65535;
    /// <summary>
    /// Start of the http url string.
    /// </summary>
    private const string HttpStart = "http:";
    /// <summary>
    /// Default Palette colors. 
    /// </summary>
    internal static readonly Color[] DEF_PALETTE = new Color[]
    {
      #region Colors
      ColorExtension.Black,
      ColorExtension.White,
      ColorExtension.Red,
      Color.FromArgb( 255, 0, 255, 0 ),
      //Color.LightGreen,
      ColorExtension.Blue,
      ColorExtension.Yellow,
      ColorExtension.Magenta,
      ColorExtension.Cyan,

      Color.FromArgb( 255, 0, 0, 0 ),
      Color.FromArgb( 255, 255, 255, 255 ),
      Color.FromArgb( 255, 255, 0, 0 ),
      Color.FromArgb( 255, 0, 255, 0 ),
      Color.FromArgb( 255, 0, 0, 255 ),
      Color.FromArgb( 255, 255, 255, 0 ),
      Color.FromArgb( 255, 255, 0, 255 ),
      Color.FromArgb( 255, 0, 255, 255 ),
      Color.FromArgb( 255, 128, 0, 0 ),
      Color.FromArgb( 255, 0, 128, 0 ),
      Color.FromArgb( 255, 0, 0, 128 ),
      Color.FromArgb( 255, 128, 128, 0 ),
      Color.FromArgb( 255, 128, 0, 128 ),
      Color.FromArgb( 255, 0, 128, 128 ),
      Color.FromArgb( 255, 192, 192, 192 ),
      Color.FromArgb( 255, 128, 128, 128 ),
      Color.FromArgb( 255, 153, 153, 255 ),
      Color.FromArgb( 255, 153, 51, 102 ),
      Color.FromArgb( 255, 255, 255, 204 ),
      Color.FromArgb( 255, 204, 255, 255 ), // Custom19
      Color.FromArgb( 255, 102, 0, 102 ),
      Color.FromArgb( 255, 255, 128, 128 ),
      Color.FromArgb( 255, 0, 102, 204 ),
      Color.FromArgb( 255, 204, 204, 255 ),
      Color.FromArgb( 255, 0, 0, 128 ),
      Color.FromArgb( 255, 255, 0, 255 ),
      Color.FromArgb( 255, 255, 255, 0 ),
      Color.FromArgb( 255, 0, 255, 255 ),
      Color.FromArgb( 255, 128, 0, 128 ),
      Color.FromArgb( 255, 128, 0, 0 ),
      Color.FromArgb( 255, 0, 128, 128 ),
      Color.FromArgb( 255, 0, 0, 255 ),
      Color.FromArgb( 255, 0, 204, 255 ),
      Color.FromArgb( 255, 204, 255, 255 ),
      Color.FromArgb( 255, 204, 255, 204 ),
      Color.FromArgb( 255, 255, 255, 153 ),
      Color.FromArgb( 255, 153, 204, 255 ),// Custom36
      Color.FromArgb( 255, 255, 153, 204 ),
      Color.FromArgb( 255, 204, 153, 255 ),
      Color.FromArgb( 255, 255, 204, 153 ),
      Color.FromArgb( 255, 51, 102, 255 ),
      Color.FromArgb( 255, 51, 204, 204 ),
      Color.FromArgb( 255, 153, 204, 0 ),
      Color.FromArgb( 255, 255, 204, 0 ),
      Color.FromArgb( 255, 255, 153, 0 ),
      Color.FromArgb( 255, 255, 102, 0 ),
      Color.FromArgb( 255, 102, 102, 153 ),
      Color.FromArgb( 255, 150, 150, 150 ),
      Color.FromArgb( 255, 0, 51, 102 ),
      Color.FromArgb( 255, 51, 153, 102 ),
      Color.FromArgb( 255, 0, 51, 0 ),
      Color.FromArgb( 255, 51, 51, 0 ),
      Color.FromArgb( 255, 153, 51, 0 ),
      Color.FromArgb( 255, 153, 51, 102 ),
      Color.FromArgb( 255, 51, 51, 153 ),
      Color.FromArgb( 255, 51, 51, 51 ),
      #endregion
    };
    internal static readonly double[] DefaultTints = new double[]
      {
            -4.9989318521683403E-2,
            -0.249977111117893,
            -0.14999847407452621,
            -0.34998626667073579,
            -0.499984740745262,
            0.34998626667073579,
            0.499984740745262,
            0.249977111117893,
            0.14999847407452621,
            4.9989318521683403E-2,
            0.79998168889431442,
            0.59999389629810485,
            0.39997558519241921,
            -0.0999786370433668,
            -0.749992370372631,
            -0.89999084444715716
      };
    internal static readonly Color[][] ThemeColorPalette = new Color[][]
      {
          new Color[] {
              Color.FromArgb(255,242,242,242),
              Color.FromArgb(255,191,191,191),
              Color.FromArgb(255,217,217,217),
              Color.FromArgb(255,166,166,166),
              Color.FromArgb(255,128,128,128),
          },
          //Swapped 2nd and 3rd
          new Color[]
          {
             Color.FromArgb(0,0,0,0),
             Color.FromArgb(0,0,0,0),
             Color.FromArgb(0,0,0,0),
             Color.FromArgb(0,0,0,0),
             Color.FromArgb(0,0,0,0),
             Color.FromArgb(255,89,89,89),
             Color.FromArgb(255,128,128,128),
             Color.FromArgb(255,64,64,64),
             Color.FromArgb(255,38,38,38),
             Color.FromArgb(255,13,13,13),
          },
           new Color[]
          {
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(255,196,189,151),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(255,148,138,84),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(255,221,217,196),
            Color.FromArgb(255,73,69,41),
            Color.FromArgb(255,29,27,16),
                             
          },
          new Color[]
          {
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(255,22,54,92),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(255,15,36,62),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(255,197,217,241),
            Color.FromArgb(255,141,180,226),
            Color.FromArgb(255,83,141,213),
          },
          new Color[]
          {
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(255,54,96,146),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(255,36,64,98),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(255,220,230,241),
            Color.FromArgb(255,184,204,228),
            Color.FromArgb(255,149,179,215),
          }, 
           new Color[]
          {
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(255,150,54,52),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(255,99,37,35),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(255,242,220,219),
            Color.FromArgb(255,230,184,183),
            Color.FromArgb(255,218,150,148),
          },
          new Color[]
          {
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(255,118,147,60),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(255,79,98,40),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(255,235,241,222),
            Color.FromArgb(255,216,228,188),
            Color.FromArgb(255,196,215,155),
           },
          new Color[]
          {
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(255,96,73,122),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(255,64,49,81),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(255,228,223,236),
            Color.FromArgb(255,204,192,218),
            Color.FromArgb(255,177,160,199),
          },
        new Color[]
        {
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(255,49,134,155),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(255,33,89,103),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(255,218,238,243),
            Color.FromArgb(255,183,222,232),
            Color.FromArgb(255,146,205,220),
          },
        new Color[]
        {
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(255,226,107,10),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(255,151,71,6),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(0,0,0,0),
            Color.FromArgb(255,253,233,217),
            Color.FromArgb(255,252,213,180),
            Color.FromArgb(255,250,191,143),
          }
      };
    private  float[] DEF_FONT_HEIGHT_SINGLE_INCR = new float[] { 6, 8, 9, 12, 14, 15, 18, 21, 23, 24, 26, 27 };
    private  float[] DEF_FONT_HEIGHT_DOUBLE_INCR = new float[] { 5, 17, 20 };
    private float[] DEF_FONT_WIDTH_SINGLE_INCR = new float[] { 11 };
    /// <summary>
    /// First user-defined color.
    /// </summary>
    public const int DEF_FIRST_USER_COLOR = 8;
    /// <summary>
    /// 
    /// </summary>
    public const string DEF_BAD_SHEET_NAME = "#REF";
    /// <summary>
    /// 
    /// </summary>
    private const int DEF_FIRST_DEFINED_FONT = 10;
    /// <summary>
    /// 
    /// </summary>
    private const string DEF_RESPONSE_OPEN = "inline";
    /// <summary>
    /// 
    /// </summary>
    private const string DEF_RESPONSE_DIALOG = "attachment";
    /// <summary>
    /// Index of the removed sheet.
    /// </summary>
    private const ushort DEF_REMOVED_INDEX = ushort.MaxValue;
    /// <summary>
    /// Content type for Excel 97.
    /// </summary>
    private const string DEF_EXCEL97_CONTENT_TYPE = "Application/x-msexcel";
    /// <summary>
    /// Content type for Excel 2000.
    /// </summary>
    private const string DEF_EXCEL2000_CONTENT_TYPE = "Application/vnd.ms-excel";
    /// <summary>
    /// Content type for Excel 2007.
    /// </summary>
    private const string DEF_EXCEL2007_CONTENT_TYPE = "Application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    /// <summary>
    /// Content type for CSV.
    /// </summary>
    private const string DEF_CSV_CONTENT_TYPE = "text/csv";
    /// <summary>
    /// Standard password.
    /// </summary>
    internal const string StandardPassword = "VelvetSweatshop";
    /// <summary>
    /// TextQualifier.
    /// </summary>
    internal const char TextQualifier = '"';
    /// <summary>
    /// Records that should be stored in m_arrPivotRecords.
    /// </summary>
    private static readonly TBIFFRecord[] DEF_PIVOTRECORDS = new TBIFFRecord[]
    {
      TBIFFRecord.StreamId,
      TBIFFRecord.PivotViewSource,
      TBIFFRecord.DCONRef,
      TBIFFRecord.DCONBIN,
      TBIFFRecord.DCONNAME,
      TBIFFRecord.DCON,
      TBIFFRecord.PivotViewAdditionalInfo,
      TBIFFRecord.ExternalSourceInfo,

      //( TBIFFRecord )2148,
    };
    /// <summary>
    /// Default regular expression options.
    /// </summary>
    private const RegexOptions DEF_REGEX = 
#if !SILVERLIGHT && !WINRT && !WP
        RegexOptions.Compiled;
#else
        RegexOptions.None;
#endif
    /// <summary>
    /// Regular expression for workbook-worksheet pair.
    /// </summary>
    private static readonly Regex  ExternSheetRegEx = new Regex(
      @"(?<BookName>\[[\S^ ']+\])?(?<SheetName>[\S ]+)",
      DEF_REGEX );
    /// <summary>
    /// Name of the book's group in regular expressions.
    /// </summary>
    private const string DEF_BOOK_GROUP = "BookName";
    /// <summary>
    /// Name of the sheet's group in regular expressions.
    /// </summary>
    private const string DEF_SHEET_GROUP = "SheetName";
    /// <summary>
    /// Index of worksheet used for workbook references.
    /// </summary>
    internal const int DEF_BOOK_SHEET_INDEX = 65534;
    /// <summary>
    /// Name prefix for styles in ignore style mode.
    /// </summary>
    private const string DEF_FORMAT_STYLE_NAME_START = "Format_";
    /// <summary>
    /// Array with streams that shouldn't be copied.
    /// </summary>
    private static readonly string[] DEF_STREAM_SKIP_COPYING = new string[]
    {
      DEF_SUMMARY_INFO,
      DEF_DOCUMENT_SUMMARY_INFO,
    };
    /// <summary>
    /// Array with characters that are reserved.
    /// </summary>
    private static readonly char[] DEF_RESERVED_BOOK_CHARS = new char[]
    {
      DEF_CHAR_VOLUME,
      DEF_CHAR_SAMEVOLUME,
      DEF_CHAR_DOWNDIR,
      DEF_CHAR_UPDIR,
      DEF_CHAR_LONGVOLUME,
      DEF_CHAR_STARTUPDIR,
      DEF_CHAR_ALTSTARTUPDIR,
      DEF_CHAR_LIBDIR,
      '|',
    };
    /// <summary>
    /// 
    /// </summary>
    private static readonly int[] PredefinedStyleOutlines = new int[] { 0, 3, 4, 5, 6, 7 };
    /// <summary>
    /// 
    /// </summary>
    private static readonly int[] PredefinedXFs = new int[] { 0, 16, 18, 20, 17, 19 };
    /// <summary>
    /// Text for evaluation warning.
    /// </summary>
    private const string EvaluationWarning = "This file was created using the evaluation version of Syncfusion Essential XlsIO.";
    /// <summary>
    /// Name of the expired evaluation sheet.
    /// </summary>
    private const string EvaluationSheetName = "Evaluation expired";
    /// <summary>
    /// Default theme colors.
    /// </summary>
    internal static readonly Color[] DefaultThemeColors = new Color[]
    {
#if !SILVERLIGHT && !WINRT && !WP
      SystemColors.Window,        // 1 and 2 are swapped
      SystemColors.WindowText,
#else
      Color.FromArgb(255,255,255,255),
      Color.FromArgb(255,0,0,0),
#endif
      ColorExtension.FromArgb( 0xEEECE1 ), // 3 and 4 are swapped
      ColorExtension.FromArgb( 0x1F497D ),
      ColorExtension.FromArgb( 0x4F81BD ),
      ColorExtension.FromArgb( 0xC0504D ),
      ColorExtension.FromArgb( 0x9BBB59 ),
      ColorExtension.FromArgb( 0x8064A2 ),
      ColorExtension.FromArgb( 0x4BACC6 ),
      ColorExtension.FromArgb( 0xF79646 ),
      ColorExtension.FromArgb( 0x0000FF ),
      ColorExtension.FromArgb( 0x800080 ),
    };
    /// <summary>
    /// First chart color index.
    /// </summary>
    private const int FirstChartColor = 77;
    /// <summary>
    /// Last chart color index.
    /// </summary>
    private const int LastChartColor = 79;
    private static readonly Color[] m_chartColors = new Color[]
    {
      ColorExtension.ChartForeground,
      ColorExtension.ChartBackground,
      ColorExtension.ChartNeutral,
    };
    /// <summary>
    /// Separator between worksheets in the cross-worksheet formula (Sheet1:Sheet3!A1).
    /// </summary>
    private const char SheetRangeSeparator = ':';
    /// <summary>
    /// Represents the Difference value of 1904 and 1899 date system format.
    /// </summary>
    internal const int Date1904SystemDifference = 1462;
    #endregion

    #region Class members
    /// <summary>
    /// Indicates whether all the formula in the workbook is evaluated.
    /// </summary>
    private bool m_enabledCalcEngine;
    /// <summary>
    /// Used to store records which belong to the Workbook part. 
    /// Temporary array which must be cleaned after read/parse operations.
    /// </summary>
    private List<BiffRecordRaw> m_records;
    /// <summary>
    /// Active sheet in the current workbook.
    /// </summary>
    private WorksheetBaseImpl m_ActiveSheet;
    /// <summary>
    /// Returns a Sheets collection that represents all the worksheets 
    /// in the specified workbook. Read-only Sheets object.
    /// </summary>
    private WorksheetsCollection  m_worksheets;
    ///// <summary>
    ///// Returns a Sheets collection stream that represents all the worksheets
    ///// in the specified workbook. Read-Only sheets stream object.
    ///// </summary>
    //private List<Stream> m_worksheetsStream; 
    /// <summary>
    /// Styles collection that represents all the styles 
    /// in the specified workbook.
    /// </summary>
    private StylesCollection m_styles;
    /// <summary>
    /// Storage of all Fonts created in a workbook.
    /// </summary>
    private FontsCollection m_fonts;
    /// <summary>
    /// Storage of all ExtendedFormats created in a workbook.
    /// </summary>
    private ExtendedFormatsCollection m_extFormats;
    /// <summary>
    /// Storage of NameRecords.
    /// </summary>
    private List<NameRecord> m_arrNames;
    /// <summary>
    /// New value changes for the format Record
    /// </summary>
    private Dictionary<int, int> m_modifiedFormatRecord = new Dictionary<int, int>();
    /// <summary>
    /// Collection of workbook's formats.
    /// </summary>
    private FormatsCollection m_rawFormats;
    /// <summary>
    /// Storage of BoundSheet records. Is a temporary storage which
    /// after read / parse operations must be cleaned.
    /// </summary>
    private List<BoundSheetRecord> m_arrBound;
    /// <summary>
    /// Dictionary which is used for storage of SST strings.
    /// </summary>
    private SSTDictionary  m_SSTDictionary;
    /// <summary>
    /// Collection contains all SupBookRecords.
    /// </summary>
    private ExternSheetRecord m_externSheet
      = ( ExternSheetRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ExternSheet );
    /// <summary>
    /// Represents the continuity of ExternSheet record
    /// </summary>
    private List<ContinueRecord> m_continue;
    /// <summary>
    /// List of the ranges that should be reparsed.
    /// </summary>
    private List<IReparse> m_arrReparse;
    /// <summary>
    /// Full name of the file where workbook was saved or loaded.
    /// </summary>
    private string  m_strFullName;
    /// <summary>
    /// True if the workbook uses the 1904 date system.
    /// </summary>
    private bool    m_bDate1904;
    /// <summary>
    /// True if workbook uses Precision
    /// </summary>
    private bool m_bPrecisionAsDisplayed;
    /// <summary>
    /// True if the workbook has been opened as Read-only.
    /// </summary>
    private bool    m_bReadOnly;
    /// <summary>
    /// True if no changes have been made to the specified workbook since 
    /// it was last saved. Read / write Boolean.
    /// </summary>
    private bool    m_bSaved;
    /// <summary>
    /// 
    /// </summary>
    private bool    m_bSelFSUsed;
    /// <summary>
    /// Indicates whether workbook is in loading state.
    /// </summary>
    private bool    m_bLoading;
    /// <summary>
    /// Indicates whether workbook is in saving state.
    /// </summary>
    private bool    m_bSaving;
    /// <summary>
    /// True if cells are protected.
    /// </summary>
    private bool    m_bCellProtect;
    /// <summary>
    /// True if window is protected.
    /// </summary>
    private bool    m_bWindowProtect;
    /// <summary>
    /// Default name of workbook used by macros.
    /// </summary>
    private string  m_strCodeName = "ThisWorkbook";
      /// <summary>
      /// True if pivot table Fields lists are hidden
      /// </summary>
    private bool m_bHidePivotFieldList = true;
    /// <summary>
    /// Indicates default theme version for wokbook.
    /// </summary>
    private string m_defaultThemeVersion;
    /// <summary>
    /// True - indicates that current workbook contains macros sub-storage.
    /// </summary>
    private bool    m_bHasMacros;
    /// <summary>
    /// True - indicates that current workbook contains summary information.
    /// </summary>
    private bool    m_bHasSummaryInformation;
    /// <summary>
    /// True - indicates that current workbook contains document summary information.
    /// </summary>
    private bool    m_bHasDocumentSummaryInformation;
    /// <summary>
    /// True - indicate that macros exists in document and they
    /// are disabled, otherwise False.
    /// </summary>
    private bool    m_bMacrosDisable;
    /// <summary>
    /// Stores current workbook's palette.
    /// </summary>
    private List<Color> m_colors;
	private bool m_hasStandardFont;
    /// <summary>
    /// True- indicates that user used custom palette instead of the default.
    /// </summary>
    private bool    m_bOwnPalette;
    /// <summary>
    /// Window one record for the workbook.
    /// </summary>
    private WindowOneRecord m_windowOne;
    /// <summary>
    /// Collection of all Name objects defined in the workbook
    /// </summary>
    private WorkbookNamesCollection m_names;
    /// <summary>
    /// Collection of all Chart objects of the workbook.
    /// </summary>
    private ChartsCollection m_charts;
    /// <summary>
    /// Collection of Charts and Worksheets objects.
    /// </summary>
    private WorkbookObjectsCollection m_arrObjects;
    /// <summary>
    /// One of protection records.
    /// </summary>
    private PasswordRecord m_password;
    /// <summary>
    /// One of protection records.
    /// </summary>
    private PasswordRev4Record m_passwordRev4;
    /// <summary>
    /// One of protection records.
    /// </summary>
    private ProtectionRev4Record m_protectionRev4;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bThrowInFormula = true;
    /// <summary>
    /// Data needed by workbook shapes (unique images and other things).
    /// </summary>
    private WorkbookShapeDataImpl m_shapesData;
    /// <summary>
    /// First unused color (color that wasn't redefined yet).
    /// </summary>
    private int m_iFirstUnusedColor = DEF_FIRST_USER_COLOR;
    /// <summary>
    /// Current object id.
    /// </summary>
    private int m_iCurrentObjectId;
    /// <summary>
    /// First free shape id.
    /// </summary>
    private int m_iCurrentHeaderId;
    /// <summary>
    /// 
    /// </summary>
    private List<ExtendedFormatRecord> m_arrExtFormatRecords;
    /// <summary>
    /// List of ExtendedXF Format record
    /// </summary>
    private List<ExtendedXFRecord> m_arrXFExtRecords;
    /// <summary>
    /// Stream from which workbook was read.
    /// </summary>
    private ICompoundFile m_workbookFile;
    /// <summary>
    /// 
    /// </summary>
    private bool m_bOptimization;
    /// <summary>
    /// Indicates whether to allow usage of 3D ranges in DataValidation
    /// list property (MS Excel doesn't allow).
    /// </summary>
    private bool m_b3dRangesInDV;
    /// <summary>
    /// Collection of extern workbooks.
    /// </summary>
    private ExternBookCollection m_externBooks;
    /// <summary>
    /// Collection of all add-in functions used in this workbook.
    /// </summary>
    private AddInFunctionsCollection m_addinFunctions;
    /// <summary>
    /// Collection of header / footer pictures.
    /// </summary>
    private WorkbookShapeDataImpl m_headerFooterPictures;
    /// <summary>
    /// Calculation options.
    /// </summary>
    private CalculationOptionsImpl m_calcution;
    /// <summary>
    /// Collection of pivot caches.
    /// </summary>
    private PivotCacheCollection m_pivotCaches;
    int[] pivotCacheIndexes = null;
    /// <summary>
    /// Workbook level Conditional Priority count.
    /// </summary>
    private int m_dxfPriority = 0;
#if !(SILVERLIGHT) && !(WINRT) && !(WP)
    /// <summary>
    /// Graphics used for string measurement.
    /// </summary>
    private Graphics m_graphics;
    /// <summary>
    /// Graphics used for string measurement.
    /// </summary>
    private OleStorageCollection m_OleStorageCollection;
#endif
    /// <summary>
    /// Formula parser.
    /// </summary>
    private FormulaUtil m_formulaUtil;
    /// <summary>
    /// Represents group of selected worksheets.
    /// </summary>
    private WorksheetGroup m_sheetGroup;
    /// <summary>
    /// Indicates whether original worksheet contains duplicated names.
    /// </summary>
    private bool m_bDuplicatedNames;
    /// <summary>
    /// Collection of built-in document properties.
    /// </summary>
    private BuiltInDocumentProperties m_builtInDocumentProperties;
    /// <summary>
    /// Collection of custom document properties.
    /// </summary>
    private CustomDocumentProperties m_customDocumentProperties;
    /// <summary>
    /// Collection of Content Type properties.
    /// </summary>
    private MetaPropertiesImpl m_contentTypeProperties;
    /// <summary>
    /// Collection of CustomXmlParts.
    /// </summary>
    private CustomXmlPartCollection m_customXmlPartCollection;
    ///<summary>
    /// Indicates whether workbook is write protected.
    /// </summary>
    internal bool m_bWriteProtection;
    /// <summary>
    /// File sharing record.
    /// </summary>
    private FileSharingRecord m_fileSharing;
    /// <summary>
    /// Indicates whether library should try to detect string value passed to Value (and Value2)
    /// property as DateTime. Setting this property to false can increase performance greatly for
    /// such operations especially on .Net Framework 1.0 and 1.1. Default value is true.
    /// </summary>
    private bool m_bDetectDateTimeInValue = true;
    /// <summary>
    /// Size of the default character measured using first function.
    /// </summary>
    private int m_iFirstCharSize = -1;
    /// <summary>
    /// Size of the default character measured using second function.
    /// </summary>
    private int m_iSecondCharSize = -1;
    /// <summary>
    /// Password used to encrypt document.
    /// </summary>
    private string m_strEncryptionPassword;
    /// <summary>
    /// Encryption type.
    /// </summary>
    internal ExcelEncryptionType m_encryptionType;
    /// <summary>
    /// Document id.
    /// </summary>
    private byte[] m_arrDocId;
    /// <summary>
    /// Maximum row count for each worksheet in this workbook.
    /// </summary>
    private int m_iMaxRowCount = /*1 << 20;//*/65536;
    /// <summary>
    /// Maximum column count for each worksheet in this workbook.
    /// </summary>
    private int m_iMaxColumnCount = /*1 << 14;//*/256;
    /// <summary>
    /// Maximum possible number of extended formats.
    /// </summary>
    private int m_iMaxXFCount = 4095;
    /// <summary>
    /// Maximum possible indent value.
    /// </summary>
    private int m_iMaxIndent = 250;
    private int m_maxImportColumns;
    /// <summary>
    /// Current excel version.
    /// </summary>
    private ExcelVersion m_version = ExcelVersion.Excel97to2003;
    /// <summary>
    /// Default XF index.
    /// </summary>
    private int m_iDefaultXFIndex = 15;
    /// <summary>
    /// File data holder, used to store data for Excel 2007 format.
    /// </summary>
    private FileDataHolder m_fileDataHolder;
    /// <summary>
    /// Maximum digit width (used to evaluate different column width)
    /// </summary>
    private double m_dMaxDigitWidth;
    /// <summary>
    /// Workbook's heap handle.
    /// </summary>
    private IntPtr m_ptrHeapHandle;
    /// <summary>
    /// This field is used to preserve BookExt record.
    /// </summary>
    private BiffRecordRaw m_bookExt;
    /// <summary>
    /// Contains list of theme colors.
    /// </summary>
    private List<Color> m_themeColors = new List<Color>( DefaultThemeColors );
    /// <summary>
    /// Unparsed controls stream.
    /// </summary>
    private Stream m_controlsStream;
    /// <summary>
    /// Maximum used table index.
    /// </summary>
    private int m_iMaxTableIndex = 1;
    /// <summary>
    /// Country code.
    /// </summary>
    private int m_iCountry = 1;
    /// <summary>
    /// Stream contains custom table styles
    /// </summary>
    private Stream m_CustomTableStylesStream;
    /// <summary>
    /// Indicates whether workbook was loaded or it is created.
    /// </summary>
    private bool m_bIsLoaded;
	private bool m_bIsCreated;
    private Dictionary<string, FontImpl> m_majorFonts;
    private Dictionary<string, FontImpl> m_minorFonts;
    private bool isEqualColor = false;
    internal bool m_hasApostrophe;
    private bool? m_isStartsOrEndsWith;
    private bool m_isOleObjectCopied;
    private bool m_hasOleObjects;
    private MSODrawingGroupRecord m_drawGroup; 
    private bool m_checkFirst;
    private int m_versioncheck = 0;
/// <summary>
    /// Represents the Compatibility record
    /// </summary>
    private CompatibilityRecord m_compatibility;
    /// <summary>
    /// Represents the Shared string stream
    /// </summary>
    private Stream m_sstStream;
    /// <summary>
    /// Represents whether workbook has Inlinestring.
    /// </summary>
    private bool m_hasInlineString;
    /// <summary>
    /// Represents the Data sorter.
    /// </summary>
    private DataSorter m_dataSorter;
    private bool m_isConverted;
    /// <summary>
    /// Preserves the pivot cache.
    /// </summary>
    private List<Stream> m_preservesPivotCache;
    /// <summary>
    /// Represents the Excel Parse Options.
    /// </summary>
    private ExcelParseOptions m_options;
    /// <summary>
    /// Preserves the unique styles cells count.
    /// </summary>
    internal Dictionary<int, int> m_xfCellCount = new Dictionary<int, int>();
    /// <summary>
    /// Boolean that represent the extended format CRC value status.
    /// </summary>
    internal bool IsCRCSucceed = false;
    /// <summary>
    /// Extended format CRC value.
    /// </summary>
    internal uint crcValue;
    /// <summary>
    /// Find the beginning version of excel
    /// </summary>
    private int beginversion = 0;
    /// <summary>
    /// 
    /// </summary>
    private int m_iLastPivotTableIndex;
      
    /// <summary>
    /// Preserves the child element in the document management properties.
    /// </summary>
    internal Dictionary<string, List<Stream>> m_childElements = new Dictionary<string, List<Stream>>();
    /// <summary>
    /// Illegal xml character defined names count.
    /// </summary>
    internal int XmlInvalidCharCount = 1;
    private bool m_IsDisposed = false;
    /// <summary>
    /// Connections collections
    /// </summary>
    private ExternalConnectionCollection m_connections;
    private ExternalConnectionCollection m_deletedConnections;
    /// <summary>
    /// Connection Support for Excel2003
    /// </summary>
    private List<BiffRecordRaw> m_externalConnection;
    /// <summary>
    /// Represents to parse sheet on demand
    /// </summary>
    private bool m_bParseOnDemand;
    /// <summary>
    /// ReCalculation Identifier.
    /// </summary>
    private RecalcIdRecord m_reCalcId = new RecalcIdRecord();
    /// <summary>
    /// Calc Identifier.
    /// </summary>
    private uint m_uCalcIdentifier = 152511;
    private bool m_isCellModified;
    internal ExcelVersion originalVersion;
    /// <summary>
    /// Preserves the DDE type link in workbook
    /// </summary>
    private List<string> m_preservedExternalLinks;
    #endregion

    #region Class static members
    /// <summary>
    /// ExcelSheetType-to-Name.
    /// </summary>
    private static Dictionary<ExcelSheetType, string> SheetTypeToName = new Dictionary<ExcelSheetType, string>( 5 );
    #endregion

    #region IWorkbook Members
    /// <summary>
    /// Returns an object that represents the active sheet (the sheet 
    /// on top) in the active workbook or in the specified window or 
    /// workbook. Returns Nothing if no sheet is active. Read-only.
    /// </summary>
    public IWorksheet   ActiveSheet
    {
      get
      {
          if (m_ActiveSheet == null)
              CheckParseOnDemand();
          return m_ActiveSheet as IWorksheet;
      }
    }
    /// <summary>
    /// Preserves the DDE type links in workbook
    /// </summary>
    internal List<string> PreservedExternalLinks
    {
        get
        {
            if(m_preservedExternalLinks == null )
                m_preservedExternalLinks = new List<string>(10);
            return m_preservedExternalLinks;
        }
    }
    /// <summary>
    /// Gets / sets index of the active sheet.
    /// </summary>
    public int          ActiveSheetIndex
    {
      get
      {
        if( m_ActiveSheet == null ) return -1;

        return m_ActiveSheet.RealIndex;
      }
      set
      {
        if( value < 0 || value >= this.ObjectCount )
          throw new ArgumentOutOfRangeException( "ActiveSheetIndex" );

        WorksheetBaseImpl oldSheet = m_ActiveSheet;

        m_ActiveSheet = this.Objects[ value ] as WorksheetBaseImpl;
        ISerializableNamedObject sheet = ( ISerializableNamedObject )m_ActiveSheet;
        WindowOne.SelectedTab = ( ushort )sheet.RealIndex;

        if( oldSheet != null )
        {
          oldSheet.Unselect( false );
        }
      }
    }

    /// <summary>
    /// Returns or sets the author of the comment. Read / write String.
    /// </summary>
    public string       Author
    {
      get
      {
        return m_builtInDocumentProperties[ ExcelBuiltInProperty.Author ].Text;
      }
      set
      {
        m_builtInDocumentProperties[ ExcelBuiltInProperty.Author ].Text = value;
      }
    }

    /// <summary>
    /// Returns collection that represents all the built-in document properties
    /// for the specified workbook. Read-only.
    /// </summary>
    public IBuiltInDocumentProperties BuiltInDocumentProperties
    {
      get
      {
        return m_builtInDocumentProperties;
      }
    }
    /// <summary>
    /// Name which used by macros to access to workbook items.
    /// </summary>
    public string       CodeName
    {
      get
      {
        return m_strCodeName;
      }
      set
      {
        m_strCodeName = value;
      }
    }
    /// <summary>
    /// Indicates whether pivot table fields option is hidden or not.
    /// </summary>
    public bool HidePivotFieldList
    {
        get
        {
            return m_bHidePivotFieldList;
        }
        set
        {
            m_bHidePivotFieldList= value;
        }
    }
    /// <summary>
    /// Indicates default theme version for wokbook
    /// </summary>
    public string DefaultThemeVersion
    {
        get
        {
            return m_defaultThemeVersion;
        }
        set
        {
            m_defaultThemeVersion = value;
        }
    }
    /// <summary>
    /// Returns collection that represents all the custom document properties
    /// for the specified workbook. Read-only.
    /// </summary>
    public ICustomDocumentProperties CustomDocumentProperties
    {
      get
      {
        return m_customDocumentProperties;
      }
    }
    /// <summary>
    /// Returns collection that represents all the ContentType  properties
    /// for the specified workbook. Read-only.
    /// </summary>
    public IMetaProperties ContentTypeProperties
    {
        get
        {
            return m_contentTypeProperties;
        }
    }
    /// <summary>
    /// Returns collection that represents all the CustomXmlParts collection
    /// for the specified workbook. Read-only.
    /// </summary>
    public ICustomXmlPartCollection CustomXmlparts
    {
        get { return m_customXmlPartCollection; }
    }
    /// <summary>
    /// True if the workbook uses the 1904 date system. Read / write Boolean.
    /// </summary>
    public bool         Date1904
    {
      get
      {
        return m_bDate1904;
      }
      set
      {
        m_bDate1904 = value;
      }
    }
    /// <summary>
    /// True if the workbook uses precision.
    /// </summary>
    public bool PrecisionAsDisplayed
    {
      get
      {
        return m_bPrecisionAsDisplayed;
      }
      set
      {
        m_bPrecisionAsDisplayed = value;
      }
    }
    /// <summary>
    /// True if cells are protected.
    /// </summary>
    public bool         IsCellProtection
    {
      get
      {
        return m_bCellProtect;
      }
    }
    /// <summary>
    /// True if window is protected.
    /// </summary>
    public bool         IsWindowProtection
    {
      get
      {
        return m_bWindowProtect;
      }
    }
    /// <summary>
    /// For an Application object, it returns a Names collection that represents
    /// all the names in the active workbook. For a Workbook object, it returns
    /// a Names collection that represents all the names in the specified
    /// workbook (including all worksheet-specific names).
    /// </summary>
    public INames       Names
    {
      [ DebuggerStepThrough() ]
      get
      {
        return m_names;
      }
    }
    internal List<BiffRecordRaw> PreserveExternalConnectionDetails
    {
        get
        {
            if (m_externalConnection == null)
                m_externalConnection = new List<BiffRecordRaw>();
            return m_externalConnection;
        }
    }
    /// <summary>
    /// True if the workbook has been opened as Read-only. Read-only Boolean.
    /// </summary>
    public bool         ReadOnly
    {
      get
      {
        return m_bReadOnly;
      }
        internal set
        {
            m_bReadOnly = value;
        }
    }

    /// <summary>
    /// True if no changes have been made to the specified workbook since 
    /// it was last saved. If current value is false then setting it to true cause Save() method call.
    /// Read/write Boolean.
    /// </summary>
    public bool         Saved
    {
      get
      {
        return m_bSaved;
      }
      set
      {
#if !(WINRT || WP)
        // On property change save changes.
        if( m_bSaved == false && value != m_bSaved )
        {
          Save();
        }
#endif
        m_bSaved = value;
      }
    }

    /// <summary>
    /// Returns a Styles collection that represents all the styles 
    /// in the specified workbook. Read-only.
    /// </summary>
    public IStyles      Styles
    {
      [ System.Diagnostics.DebuggerStepThrough ]
      get
      {
        return m_styles;
      }
    }

    /// <summary>
    /// Returns a Sheets collection that represents all the worksheets 
    /// in the specified workbook. Read-only Sheets object.
    /// </summary>
    public IWorksheets  Worksheets
    {
      [ System.Diagnostics.DebuggerStepThrough ]
      get
      {
        return m_worksheets;
      }
    }
    public IConnections Connections
    {
        [System.Diagnostics.DebuggerStepThrough]
        get
        {
            return m_connections;
        }
    }
    /// <summary>
    /// True indicates that opened workbook contains VBA macros.
    /// </summary>
    public bool         HasMacros
    {
      get
      {
        return m_bHasMacros;
      }
      internal set
      {
        m_bHasMacros = value;
      }
    }
    public IConnections DeletedConnections
    {
        get
        {
            if (m_deletedConnections == null)
                m_deletedConnections = new ExternalConnectionCollection(this.Application, this.Parent);
            return m_deletedConnections;
        }
    }
    /// <summary>
    /// Gets a Palette of colors the Excel document can have. 
    /// Here is a Table of color indexes their places in the color tool box 
    /// provided by XlsIO application:
    /// --------------------------------------------
    /// |  | 0  | 1  | 2  | 3  | 4  | 5  | 6  | 7  |
    /// ---+----------------------------------------
    /// |0 | 00 | 51 | 50 | 49 | 47 | 10 | 53 | 54 |
    /// |1 | 08 | 45 | 11 | 09 | 13 | 04 | 46 | 15 |
    /// |2 | 02 | 44 | 42 | 48 | 41 | 40 | 12 | 55 |
    /// |3 | 06 | 43 | 05 | 03 | 07 | 32 | 52 | 14 |
    /// |4 | 37 | 39 | 35 | 34 | 33 | 36 | 38 | 01 |
    /// ---+----------------------------------------
    /// |5 | 16 | 17 | 18 | 19 | 20 | 21 | 22 | 23 |
    /// |6 | 24 | 25 | 26 | 27 | 28 | 29 | 30 | 31 |
    /// --------------------------------------------
    /// </summary>
    public Color[]      Palettte
    {
      get
      {
        return m_colors.ToArray();
      }
    }
    /// <summary>
    /// Gets a Palette of colors the Excel document can have. 
    /// Here is a Table of color indexes their places in the color tool box 
    /// provided by XlsIO application:
    /// --------------------------------------------
    /// |  | 0  | 1  | 2  | 3  | 4  | 5  | 6  | 7  |
    /// ---+----------------------------------------
    /// |0 | 00 | 51 | 50 | 49 | 47 | 10 | 53 | 54 |
    /// |1 | 08 | 45 | 11 | 09 | 13 | 04 | 46 | 15 |
    /// |2 | 02 | 44 | 42 | 48 | 41 | 40 | 12 | 55 |
    /// |3 | 06 | 43 | 05 | 03 | 07 | 32 | 52 | 14 |
    /// |4 | 37 | 39 | 35 | 34 | 33 | 36 | 38 | 01 |
    /// ---+----------------------------------------
    /// |5 | 16 | 17 | 18 | 19 | 20 | 21 | 22 | 23 |
    /// |6 | 24 | 25 | 26 | 27 | 28 | 29 | 30 | 31 |
    /// --------------------------------------------
    /// </summary>
    public Color[]      Palette
    {
      get
      {
        return m_colors.ToArray();
      }
    }
    /// <summary>
    /// Index of the tab which will be displayed on document open.
    /// </summary>
    public int          DisplayedTab
    {
      get
      {
        return ( int ) WindowOne.DisplayedTab;
      }
      set
      {
        if(( value < 0 || value > m_arrObjects.Count ) && this.IsCreated)//m_worksheets.Count )
          throw new ArgumentOutOfRangeException( "DisplayedTab", "Displayed tab must be greater than zero and less than Worksheets count" );

        WindowOne.DisplayedTab = ( ushort ) value;
        WindowOne.SelectedTab  = ( ushort ) value;
      }
    }
    /// <summary>
    /// Collection of the chart objects.
    /// </summary>
    public ICharts      Charts
    {
      get
      {
        CheckParseOnDemand();
        return m_charts;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool         ThrowOnUnknownNames
    {
      get
      {
        return m_bThrowInFormula;
      }
      set
      {
        m_bThrowInFormula = value;
      }
    }
    /// <summary>
    /// Gets/Sets value to display horizontal scrollbar
    /// </summary>
    public bool IsHScrollBarVisible
    {
      get
      {
        return WindowOne.IsHScroll;
      }
      set
      {
        WindowOne.IsHScroll = value;
      }
    }
    /// <summary>
    /// Gets/Sets value to display vertical scrollbar
    /// </summary>
    public bool IsVScrollBarVisible
    {
      get
      {
        return WindowOne.IsVScroll;
      }
      set
      {
        WindowOne.IsVScroll = value;
      }
    }
    /// <summary>
    /// This Property allows users to disable load of macros from 
    /// document. Excel on file open will simply skip macros and will
    /// work like document does not contains them. This options works
    /// only when file contains macros (HasMacros property is True ).
    /// </summary>
    public bool         DisableMacrosStart
    {
      get
      {
        return m_bMacrosDisable;
      }
      set
      {
        if( value != m_bMacrosDisable )
        {
          m_bMacrosDisable = value;
          this.Saved = false;
        }
      }
    }
    /// <summary>
    /// Returns or sets the standard font size. Read/write.
    /// </summary>
    public double       StandardFontSize 
    {
      get
      {
        return (( FontImpl )m_fonts[ 0 ]).Size;
      }
      set
      {
          if (value != StandardFontSize)
          {
              m_hasStandardFont = true;
              //for( int i=0; i < 4; i++ )
              //{
              ((FontImpl)m_fonts[0]).Size = (int)value;
              //}

              FontWrapper font = (Styles[RangeImpl.DEF_DEFAULT_STYLE].Font as FontWrapper);
              m_dMaxDigitWidth = -1;

              if (font.Index < 4)
              {
                  font.InvokeAfterChange();
              }
          }
        //NormalStyle
      }
    }
	internal bool HasStandardFont
    {
        get
        {
           return m_hasStandardFont;
       }
    }
    /// <summary>
    /// Returns or sets the name of the standard font. Read/write String.
    /// </summary>
    public string       StandardFont 
    {
      get
      {
        return (( FontImpl )m_fonts[ 0 ]).FontName;
      }
      set
      {
		m_hasStandardFont = true;
        for( int i=0; i < 4; i++ )
        {
          (( FontImpl )m_fonts[ 0 ]).FontName = value;
        }
      }
    }

    /// <summary>
    /// Indicates whether to allow usage of 3D ranges in DataValidation
    /// list property (MS Excel doesn't allow).
    /// </summary>
    public bool         Allow3DRangesInDataValidation
    {
      get
      {
        return m_b3dRangesInDV;
      }
      set
      {
        m_b3dRangesInDV = value;
      }
    }
    /// <summary>
    /// Returns collection of add-in functions. Read-only.
    /// </summary>
    public IAddInFunctions AddInFunctions
    {
      get
      {
        return m_addinFunctions;
      }
    }
    //    /// <summary>
    //    /// Indicates whether to preserve header / footer pictures.
    //    /// </summary>
    //    public bool         PreserveHeaderFooterPictures
    //    {
    //      get
    //      {
    //        return m_bPreserveHeaderFooterPictures;
    //      }
    //      set
    //      {
    //        m_bPreserveHeaderFooterPictures = value;
    //      }
    //    }
    /// <summary>
    /// Returns calculation options. Read-only.
    /// </summary>
    public ICalculationOptions CalculationOptions
    {
      get
      {
        return m_calcution;
      }
    }
    /// <summary>
    /// Gets / sets row separator for array parsing.
    /// </summary>
    public string       RowSeparator
    {
      get
      {
        return FormulaUtil.ArrayRowSeparator;
      }
    }
    /// <summary>
    /// Formula arguments separator.
    /// </summary>
    public string       ArgumentsSeparator
    {
      get
      {
        return FormulaUtil.OperandsSeparator;
      }
    }

    /// <summary>
    /// Returns grouped worksheets. Read-only.
    /// </summary>
    public IWorksheetGroup WorksheetGroup
    {
      get
      {
        return m_sheetGroup;
      }
    }
    /// <summary>
    /// Indicates whether worksheet is displayed right to left.
    /// </summary>
    public bool IsRightToLeft
    {
      get
      {
        return m_worksheets.IsRightToLeft;
      }
      set
      {
        m_worksheets.IsRightToLeft = value;
      }
    }
    /// <summary>
    /// Indicates whether workbook tabs are visible.
    /// </summary>
    public bool DisplayWorkbookTabs
    {
      get
      {
        return WindowOne.IsTabs;
      }
      set
      {
        WindowOne.IsTabs = value;
      }
    }
    /// <summary>
    /// Returns collection with all tabsheets in the workbook. Read-only.
    /// </summary>
    public ITabSheets TabSheets
    {
      get
      {
        CheckParseOnDemand();
        return m_arrObjects;
      }
    }
    /// <summary>
    /// Indicates whether library should try to detect string value passed to Value (and Value2)
    /// property as DateTime. Setting this property to false can increase performance greatly for
    /// such operations especially on Framework 1.0 and 1.1. Default value is true.
    /// </summary>
    public bool DetectDateTimeInValue
    {
      get
      {
        return m_bDetectDateTimeInValue;
      }
      set
      {
        m_bDetectDateTimeInValue = value;
      }
    }
    /// <summary>
    /// Toggles string searching algorithm. If true then Dictionary will be used
    /// to locate string inside strings dictionary. This mode is faster but uses
    /// more memory. If false then each time string is added to strings dictionary
    /// we will have to iterate through it and compare new strings with existing ones.
    /// Default value is TRUE.
    /// </summary>
    public bool UseFastStringSearching
    {
      get
      {
        return m_SSTDictionary.UseHashForSearching;
      }
      set
      {
        m_SSTDictionary.UseHashForSearching = value;
      }
    }
    /// <summary>
    /// True to display a message when the file is opened, recommending that the file be opened as read-only.
    /// </summary>
    public bool ReadOnlyRecommended
    {
      get
      {
        return ( m_fileSharing == null )
          ? false
          : m_fileSharing.RecommendReadOnly != 0;
      }
      set
      {
        if( value )
        {
          if( m_fileSharing == null )
            m_fileSharing = ( FileSharingRecord )BiffRecordFactory.GetRecord( TBIFFRecord.FileSharing );

          m_fileSharing.RecommendReadOnly = 1;
        }
        else
        {
          if( m_fileSharing != null )
            m_fileSharing.RecommendReadOnly = 0;
        }
      }
    }
    /// <summary>
    /// Gets / sets password to encrypt document.
    /// </summary>
    public string PasswordToOpen
    {
      get
      {
        return m_strEncryptionPassword;
      }
      set
      {
        m_strEncryptionPassword = value;

        if( value == null || value.Length == 0 )
        {
          m_encryptionType = ExcelEncryptionType.None;
        }
        else
        {
          m_encryptionType = ExcelEncryptionType.Standard;
        }
      }
    }
    /// <summary>
    /// Returns maximum row count for each worksheet in this workbook. Read-only.
    /// </summary>
    public int MaxRowCount
    {
      get
      {
        return m_iMaxRowCount;
      }
    }
    /// <summary>
    /// Returns maximum column count for each worksheet in this workbook. Read-only.
    /// </summary>
    public int MaxColumnCount
    {
      get
      {
        return m_iMaxColumnCount;
      }
    }
    /// <summary>
    /// Returns maximum possible number of extended formats. Read-only.
    /// </summary>
    public int MaxXFCount
    {
      get
      {
        return m_iMaxXFCount;
      }
    }
    /// <summary>
    /// Gets maximum possible indent value. Read-only.
    /// </summary>
    public int MaxIndent
    {
      get
      {
        return m_iMaxIndent;
      }
    }
    public int MaxImportColumns
    {
      get
      {
        return m_maxImportColumns;
      }
      set
      {
        m_maxImportColumns = value;
      }
    }
    #endregion

    #region Implementation properties
    /// <summary>
    /// Workbook level Conditional Priority count.
    /// </summary>
    internal int BookCFPriorityCount
    {
        get
        {
            return m_dxfPriority;
        }
        set
        {
            m_dxfPriority = value;
        }
    }
    /// <summary>
    /// Indicates whether all the formula in the workbook is evaluated.
    /// </summary>
    internal bool EnabledCalcEngine
    {
        get
        {
            return m_enabledCalcEngine;
        }
        set
        {
            m_enabledCalcEngine = value;
        }
    }
    /// <summary>
    /// Represents the Excel Parse Options.
    /// </summary>
    internal ExcelParseOptions Options
    {
        get
        {
            return m_options;
        }
        set
        {
            m_options = value;
        }
    }

      /// <summary>
      /// gets/ Sets the pivot table last index
      /// </summary>
    internal int LastPivotTableIndex
    {
        get
        {
            return m_iLastPivotTableIndex;
        }
        set
        {
            m_iLastPivotTableIndex = value;
        }
    }
    /// <summary>
    /// Preserves the pivot cache.
    /// </summary>
    internal List<Stream> PreservesPivotCache
    {
        get
        {
            if (m_preservesPivotCache == null)
                m_preservesPivotCache = new List<Stream>();
            return m_preservesPivotCache;
        }
    }
    /// <summary>
    /// /Return file data holder, used to store data for Excel 2007 format.
    /// </summary>
    public FileDataHolder DataHolder
    {
      get
      {
        return m_fileDataHolder;
      }
    }
    /// <summary>
    /// Return WorkbookNamesColection from parent WorkBook.
    /// </summary>
    public WorkbookNamesCollection InnerNamesColection
    {
      [ DebuggerStepThrough ]
      get
      {
        return m_names;
      }
    }
    /// <summary>
    /// Return ContententType Properties from the workbook
    /// </summary>
    public MetaPropertiesImpl InnerContentTypeProperties
    {
        [DebuggerStepThrough]
        get
        {
            return m_contentTypeProperties;
        }
    }
    /// <summary>
    /// Return CustomXmlParts from the workbook
    /// </summary>
    public CustomXmlPartCollection InnerCustomXmlParts
    {
        [DebuggerStepThrough]
        get
        {
            return m_customXmlPartCollection;
        }
    }
    /// <summary>
    /// Returns collection of add-in functions. Read-only.
    /// </summary>
    public AddInFunctionsCollection InnerAddInFunctions
    {
      [ DebuggerStepThrough ]
      get
      {
        return m_addinFunctions;
      }
    }
    /// <summary>
    /// Returns name of the file the workbook was saved
    /// in last time or loaded from.
    /// </summary>
    public string FullFileName
    {
      [ DebuggerStepThrough ]
      get
      {
        return m_strFullName;
      }
      [DebuggerStepThrough]
      internal set
      {
        m_strFullName = value;
      }
    }
    /// <summary>
    /// Collection of all fonts used in the workbook. Read-only.
    /// </summary>
    public FontsCollection InnerFonts
    {
      [ DebuggerStepThrough ]
      get
      {
        return m_fonts;
      }
    }
    /// <summary>
    /// Collection of all ExtendedFormats used in the workbook.
    /// </summary>
    public ExtendedFormatsCollection InnerExtFormats
    {
      [ DebuggerStepThrough ]
      get
      {
        return m_extFormats;
      }
    }
    /// <summary>
    /// Collection of all formats used in the workbook. Read-only.
    /// </summary>
    public FormatsCollection InnerFormats
    {
      [ DebuggerStepThrough ]
      get
      {
        return m_rawFormats;
      }
    }
    /// <summary>
    /// SSTDictionary that contains all strings used in the workbook. Read-only.
    /// </summary>
    public SSTDictionary InnerSST
    {
      [ DebuggerStepThrough ]
      get
      {
        return m_SSTDictionary;
      }
    }
    /// <summary>
    /// Indicates whether workbook is loading. Read-only.
    /// </summary>
    public bool Loading
    {
      [ DebuggerStepThrough ]
      get
      {
        return m_bLoading;
      }
      [DebuggerStepThrough]
      /*internal*/ set
      {
        m_bLoading = value;
      }
    }
    /// <summary>
    /// Indicates whether workbook is in saving process. Read-only.
    /// </summary>
    public bool Saving
    {
      [DebuggerStepThrough]
      get
      {
        return m_bSaving;
      }
      [DebuggerStepThrough]
      internal set
      {
        m_bSaving = value;
      }
    }
    /// <summary>
    /// Stores the attributes of the workbook window. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public WindowOneRecord WindowOne
    {
      get
      {
        if( m_windowOne == null )
        {
          m_windowOne = ( WindowOneRecord )
            BiffRecordFactory.GetRecord( TBIFFRecord.WindowOne );
        }

        return m_windowOne;
      }
    }
    /// <summary>
    /// Returns count of charts and worksheets in the workbook.
    /// </summary>
    public int ObjectCount
    {
      get
      {
        return m_arrObjects.Count;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public double MaxDigitWidth
    {
      get
      {
        if( m_dMaxDigitWidth <= 0 )
        {
          m_dMaxDigitWidth = GetMaxDigitWidth();
        }

        return m_dMaxDigitWidth;
      }
    }
    /// <summary>
    /// Gets or sets the SST stream.
    /// </summary>
    /// <value>The SST stream.</value>
    internal Stream SSTStream
    {
        get
        {
            return m_sstStream;
        }
        set
        {
            m_sstStream = value;
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance has inline strings.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance has inline strings; otherwise, <c>false</c>.
    /// </value>
    internal bool HasInlineStrings
    {
        get
        {
            return m_hasInlineString;
        }
        set
        {
            m_hasInlineString = value;
        }
    }
    /// <summary>
    /// Gets / sets PasswordRecord.
    /// </summary>
    internal PasswordRecord  Password
    {
      get
      {
        if( m_password == null )
        {
          m_password = ( PasswordRecord )
            BiffRecordFactory.GetRecord( TBIFFRecord.Password );
        }

        return m_password;
      }
      set
      {
        m_password = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    [ CLSCompliant( false ) ]
    protected PasswordRev4Record PasswordRev4
    {
      get
      {
        if( m_passwordRev4 == null )
        {
          m_passwordRev4 = ( PasswordRev4Record )
            BiffRecordFactory.GetRecord( TBIFFRecord.PasswordRev4 );
        }

        return m_passwordRev4;
      }
      set
      {
        m_passwordRev4 = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    [ CLSCompliant( false ) ]
    protected ProtectionRev4Record ProtectionRev4
    {
      get
      {
        if( m_protectionRev4 == null )
        {
          m_protectionRev4 = ( ProtectionRev4Record )
            BiffRecordFactory.GetRecord( TBIFFRecord.ProtectionRev4 );
        }

        return m_protectionRev4;
      }
      set
      {
        m_protectionRev4 = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int CurrentObjectId
    {
      get
      {
        return m_iCurrentObjectId;
      }
      set
      {
        if( value > 0 ) m_iCurrentObjectId = value;
      }
    }
    /// <summary>
    /// First free shape id.
    /// </summary>
    public int CurrentHeaderId
    {
      get
      {
        return m_iCurrentHeaderId;
      }
      set
      {
        if( value < 0 )
          throw new ArgumentOutOfRangeException( "shape id" );

        m_iCurrentHeaderId = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    internal protected List<ExtendedFormatRecord> InnerExtFormatRecords
    {
      get
      {
        return m_arrExtFormatRecords;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    internal protected List<ExtendedXFRecord> InnerXFExtRecords
    {
        get
        {
            return m_arrXFExtRecords;
        }
     }
    /// <summary>
    /// Returns collection of named objects owned by the workbook
    /// (worksheet and charts). Read-only.
    /// </summary>
    internal protected WorkbookObjectsCollection Objects
    {
      get
      {
        return m_arrObjects;
      }
      internal set
      {
        m_arrObjects = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    protected internal StylesCollection InnerStyles
    {
      get
      {
        return m_styles;
      }
    }
    /// <summary>
    /// Worksheets collection.
    /// </summary>
    protected internal WorksheetsCollection InnerWorksheets
    {
      get
      {
        return m_worksheets;
      }
    }
    /// <summary>
    /// Charts collection.
    /// </summary>
    protected internal ChartsCollection InnerCharts
    {
      get
      {
        return m_charts;
      }
    }
    /// <summary>
    /// Returns collection of external workbooks.
    /// </summary>
    public ExternBookCollection ExternWorkbooks
    {
      get
      {
        return m_externBooks;
      }
    }
    /// <summary>
    /// Returns calculation options. Read-only.
    /// </summary>
    public CalculationOptionsImpl InnerCalculation
    {
      get
      {
        return m_calcution;
      }
    }
#if !(SILVERLIGHT) && !(WINRT) && !(WP)
    /// <summary>
    /// Returns workbook graphics.
    /// </summary>
    public Graphics InnerGraphics
    {
      get
      {
        return m_graphics;
      }
    }
#endif
    /// <summary>
    /// Returns class for formula parsing. Read-only.
    /// </summary>
    [ CLSCompliant( false ) ]
    public FormulaUtil FormulaUtil
    {
      get
      {
        if( m_formulaUtil == null )
        {
          m_formulaUtil = new FormulaUtil( Application, this );
        }

        return m_formulaUtil;
      }
    }
    /// <summary>
    /// Returns grouped worksheets. Read-only.
    /// </summary>
    public WorksheetGroup InnerWorksheetGroup
    {
      get
      {
        return m_sheetGroup;
      }
    }
    internal bool? IsStartsOrEndsWith
    {
        get
        {
            return m_isStartsOrEndsWith;
        }
        set
        {
            m_isStartsOrEndsWith = value;
        }
    }
    /// <summary>
    /// Indicates whether original file contains duplicated external names.
    /// </summary>
    public bool HasDuplicatedNames
    {
      get
      {
        return m_bDuplicatedNames;
      }
      set
      {
        m_bDuplicatedNames = value;
      }
    }
    /// <summary>
    /// Returns data that is shared by all shapes (global options, unique pictures, etc. ).
    /// </summary>
    public WorkbookShapeDataImpl ShapesData
    {
      get
      {
        return m_shapesData;
      }
    }
    /// <summary>
    /// Returns data that is shared by all header/footers.
    /// </summary>
    public WorkbookShapeDataImpl HeaderFooterData
    {
      get
      {
        return m_headerFooterPictures;
      }
    }
    /// <summary>
    /// Gets externSheet record. Read - only.
    /// </summary>
    internal ExternSheetRecord ExternSheet
    {
      get
      {
        return m_externSheet;
      }
    }
    /// <summary>
    /// Gets / sets internal flag that indicates whether workbook was saved or not.
    /// </summary>
    protected internal bool InternalSaved
    {
      get
      {
        return m_bSaved;
      }
      set
      {
        m_bSaved = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int FirstCharSize
    {
      get
      {
        return m_iFirstCharSize;
      }
      set
      {
        m_iFirstCharSize = value;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public int SecondCharSize
    {
      get
      {
        return m_iSecondCharSize;
      }
      set
      {
        m_iSecondCharSize = value;
      }
    }
    /// <summary>
    /// Gets a value indicating whether this Workbook is converted.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is converted; otherwise, <c>false</c>.
    /// </value>
    internal bool IsConverted
    {
        get
        {
            return m_isConverted;
        }
    }
    public int BeginVersion
    {
        get
        {
            return beginversion; 
        }
        set
        {
            beginversion = value;
        }
    }

    /// <summary>
    /// Gets / sets excel version.
    /// </summary>
    public ExcelVersion Version
    {
      get
      {
        //return ExcelVersion.Excel2007;
        //return ExcelVersion.Excel97to2003;
        return m_version;
      }
      set
      {
          if (m_version != value)
              CheckParseOnDemand();

          bool clearPivotTables = false;
          if (m_checkFirst && m_version == ExcelVersion.Excel97to2003 && (value !=ExcelVersion.Excel97to2003))
              m_isConverted = true;
          else if (!m_checkFirst)
              m_checkFirst = true;
          if ((value == ExcelVersion.Excel97to2003 && m_versioncheck == 0))
          {
              beginversion = 1;
              m_versioncheck++;
          }
          else if ((m_versioncheck == 0 || beginversion == 2) && !(value == ExcelVersion.Excel97to2003))
          {
              beginversion = 0;
              m_versioncheck++;
          }
          if (m_version >= ExcelVersion.Excel2007 && value >= ExcelVersion.Excel2007)
          {
              m_version = value;
          }
          else if (m_version != value)
          {
              originalVersion = m_version;
              m_version = value;
              // We don't support macro conversion.
              m_bHasMacros = false;

              switch (value)
              {
                  case ExcelVersion.Excel97to2003:
                      m_iMaxRowCount = 65536;
                      m_iMaxColumnCount = 256;
                      m_extFormats.SetMaxCount(4095 - 20);
                      m_iMaxXFCount = 4075;
                      m_extFormats.SetMaxCount(4095);
                      m_iMaxXFCount = 4095;
                      m_iMaxIndent = 15;
                      ChangeStylesTo97();
                      ClearPivotCaches();
                      break;

                  case ExcelVersion.Excel2007:
                  case ExcelVersion.Excel2010:
                  case ExcelVersion.Excel2013:

                      m_iMaxRowCount = 1 << 20;
                      m_iMaxColumnCount = 1 << 14;
                      // Please refere WF-8182 bug regarding the Max count changes.
                      m_extFormats.SetMaxCount(65000);
                      m_iMaxXFCount = 65000;
                      m_iMaxIndent = 250;

                      if (originalVersion == ExcelVersion.Excel97to2003)
                      {
                          ClearPivotCaches();
                          clearPivotTables = true;
                      }
                      m_names.Validate();
                      break;
              }

              for (int i = 0, len = m_worksheets.Count; i < len; i++)
              {
                  WorksheetImpl sheet = (WorksheetImpl)m_worksheets[i];
                  PivotTableCollection pivotTables = sheet.PivotTables as PivotTableCollection;
                  if (clearPivotTables)
                      pivotTables.ClearWithoutCheck();
                  sheet.Version = value;
              }

              //// NOTE: we don't support chart sheets in Excel 2007, so after version change
              //// we won't be able to serialize it neither in Excel97-2003 format, nor in Excel2007 format
              //for( int i = m_charts.Count - 1; i >= 0; i-- )
              //{
              //  //m_charts.RemoveAt( i );
              //  ITabSheet sheet = ( ITabSheet )m_charts[ i ];
              //  m_arrObjects.RemoveAt( sheet.TabIndex );
              //}

              if (value == ExcelVersion.Excel97to2003)
              {
                  m_SSTDictionary.RemoveUnnecessaryStrings();                  
              }

              WorkbookNamesCollection names = InnerNamesColection;

              if (names != null)
              {
                  names.ConvertFullRowColumnNames(value);
              }
          }
      }
    }
    /// <summary>
    /// Clears all pivot caches.
    /// </summary>
    private void ClearPivotCaches()
    {
      if( m_pivotCaches != null )
        m_pivotCaches.Clear();
    }
    /// <summary>
    /// Returns index to the default extended format.
    /// </summary>
    public int DefaultXFIndex
    {
      get
      {
        return m_iDefaultXFIndex;
      }
      set
      {
        if( value < 0 )
          throw new ArgumentOutOfRangeException( "DefaultXFIndex" );

        m_iDefaultXFIndex = value;
      }
    }
    /// <summary>
    /// Returns internal array with palette colors. Read-only.
    /// </summary>
    public List<Color> InnerPalette
    {
      get
      {
        return m_colors;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    public IntPtr HeapHandle
    {
      get
      {
#if AllowUnsafeCode
        if( m_ptrHeapHandle == IntPtr.Zero )
        {
          m_ptrHeapHandle = Heap.HeapCreate( 0, 1024 * 128, 0 );
        }
#endif

        return m_ptrHeapHandle;
      }
    }
    /// <summary>
    /// Returns collection of workbook pivot caches. Read-only.
    /// </summary>
    public PivotCacheCollection PivotCaches
    {
      get
      {
          if (m_pivotCaches == null)
              m_pivotCaches = new PivotCacheCollection(Application, this);
        return m_pivotCaches;
      }
    }
    /// <summary>
    /// Returns collection of workbook pivot caches. Read-only.
    /// </summary>
    IPivotCaches IWorkbook.PivotCaches
    {
      get
      {
        if( m_pivotCaches == null )
          m_pivotCaches = new PivotCacheCollection( Application, this );

        return m_pivotCaches;
      }
    }
    /// <summary>
    /// Gets value indicating whether workbook controls stream or not. Read-only.
    /// </summary>
    public Stream ControlsStream
    {
      get
      {
        return m_controlsStream;
      }
      internal set
      {
        m_controlsStream = value;
      }
    }
    /// <summary>
    /// Gets value indicating whether workbook controls stream or not. Read-only.
    /// </summary>
    internal Stream CustomTableStylesStream
    {
        get
        {
            return m_CustomTableStylesStream;
        }
        set
        {
            m_CustomTableStylesStream = value;
        }
    }
    /// <summary>
    /// Gets or sets maximum used table (list object) index.
    /// </summary>
    public int MaxTableIndex
    {
      get
      {
        return m_iMaxTableIndex;
      }
      set
      {
        m_iMaxTableIndex = value;
      }
    }
	 internal bool IsCreated
    {
        get
        {
            return m_bIsCreated;
        }
    }
    /// <summary>
    /// Gets value indicating whether workbook was loaded from file or stream.
    /// </summary>
    public bool IsLoaded
    {
      get
      {
        return m_bIsLoaded;
      }
    }
    internal Dictionary<string, FontImpl> MajorFonts
    {
        get
        {
            return m_majorFonts;
        }
        set
        {
            m_majorFonts = value;
        }
    }
    internal Dictionary<string, FontImpl> MinorFonts
    {
        get
        {
            return m_minorFonts;
        }
        set
        {
            m_minorFonts = value;
        }
    }
/// <summary>
    /// Specifies wheather the workbook checks the Compability of earlier version
    /// </summary>
    public bool CheckCompability
    {
        get
        {
            if (m_compatibility == null)
                return false;
            return !(m_compatibility.NoComptabilityCheck == 0);
        }
        set
        {
            if (m_compatibility == null)
                m_compatibility = new CompatibilityRecord();
            m_compatibility.NoComptabilityCheck=(uint) ((!value)?1:0);
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether the name ranges has apostrophe.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance has apostrophe; otherwise, <c>false</c>.
    /// </value>
    internal bool HasApostrophe
    {
        get
        {
            return m_hasApostrophe;
        }
        set
        {
            m_hasApostrophe = value;
        }
    }
#if !(SILVERLIGHT) && !(WINRT) && !(WP)
    /// <summary>
    /// Indicates whether book has oleObjects
    /// </summary>
    public bool HasOleObjects
    {
        get
        {
            return m_hasOleObjects;
        }
        set
        {
            m_hasOleObjects = value;
        }
    }
    /// <summary>
    /// Indicates whether Ole Objects are copied
    /// </summary>
    public bool IsOleObjectCopied
    {
        get
        {
            return m_isOleObjectCopied;
        }
        set
        {
            m_isOleObjectCopied = value;
        }
    }
    public OleStorageCollection OleStorageCollection
    {
        get
        {
            if (m_OleStorageCollection == null)
                m_OleStorageCollection = new OleStorageCollection();
            return m_OleStorageCollection;
        }
    }
    internal uint CalcIdentifier
    {
        get
        {
            return m_uCalcIdentifier;
        }
    }
#endif
      /// <summary>
      /// Gets or sets boolean value to parse worksheets on demand
      /// </summary>
      internal bool ParseOnDemand
      {
          get 
          { 
              return m_bParseOnDemand;
          }
          set
          {
              m_bParseOnDemand = value;
          }
      }
      internal bool IsCellModified
      {
          get
          {
              return m_isCellModified;
          }
          set
          {
              m_isCellModified = value;
          }
      }
    #endregion

    #region Implementation helper methods
    /// <summary>
    /// Returns theme color by its index.
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public Color GetThemeColor( int color )
    {
      return m_themeColors[ color ];
    }
    /// <summary>
    /// Creates extended format record and registers it in workbook.
    /// </summary>
    /// <param name="bForceAdd">Indicates whether to force add to collection.</param>
    /// <returns>Newly created extended format.</returns>
    protected internal IExtendedFormat CreateExtFormat( bool bForceAdd )
    {
      ExtendedFormatImpl extFormat = new ExtendedFormatImpl( Application, this );
      extFormat.Index = ( ushort )m_extFormats.Count;

      if( bForceAdd )
      {
        m_extFormats.ForceAdd( extFormat );
      }
      else
      {
        m_extFormats.Add( extFormat );
      }
      
      return extFormat;
    }
    /// <summary>
    /// Creates extended format based on baseFormat and registers it in workbook. 
    /// </summary>
    /// <param name="baseFormat">Base format for the new format.</param>
    /// <param name="bForceAdd">Indicates whether to force add.</param>
    /// <returns>Newly created format.</returns>
    protected internal IExtendedFormat CreateExtFormat( IExtendedFormat baseFormat, bool bForceAdd )
    {
      if( baseFormat == null )
        throw new ArgumentNullException( "baseFormat" );

      ExtendedFormatImpl extFormat = CreateExtFormatWithoutRegister( baseFormat );

      if( bForceAdd )
      {
        m_extFormats.ForceAdd( extFormat );
      }
      else
      {
        extFormat = m_extFormats.Add( extFormat );
      }

      return extFormat;
    }
    internal bool IsEqualColor
    {
        get
        {
            return isEqualColor;
        }
    }
    /// <summary>
    /// Creates extended format based on baseFormat without registering it in the workbook.
    /// </summary>
    /// <param name="baseFormat">Base format for the new format.</param>
    /// <returns>Newly created format.</returns>
    protected internal ExtendedFormatImpl CreateExtFormatWithoutRegister( IExtendedFormat baseFormat )
    {
      ExtendedFormatImpl extFormat;
      ExtendedFormatRecord rc;
      ExtendedXFRecord xf;
      ShapeFillImpl gradient = null;

      if( baseFormat == null )
        throw new ArgumentNullException( "baseFormat" );

      if( baseFormat is ExtendedFormatImpl )
      {
        extFormat = ( ExtendedFormatImpl )baseFormat;
        rc = ( ExtendedFormatRecord )extFormat.Record.Clone();
        xf = (ExtendedXFRecord)extFormat.XFRecord.Clone();
      }
      else if( baseFormat is ExtendedFormatWrapper )
      {
        extFormat = ( ( ExtendedFormatWrapper )baseFormat ).Wrapped;
        rc = ( ExtendedFormatRecord )extFormat.Record.Clone();
        xf = (ExtendedXFRecord)extFormat.XFRecord.Clone();
      }
      else
        throw new ArgumentException( "baseFormat can be only ExtendedFormatImpl or ExtendedFormatImplWrapper classes" );

      if( extFormat.Gradient != null )
        gradient = ( ( ShapeFillImpl )extFormat.Gradient ).Clone( extFormat );

      ExtendedFormatImpl baseFormatImpl = extFormat;
      extFormat = extFormat = new ExtendedFormatImpl(Application, this, rc, xf);
      extFormat.ColorObject.CopyFrom( baseFormatImpl.ColorObject, false );
      extFormat.PatternColorObject.CopyFrom( baseFormatImpl.PatternColorObject, false );
      extFormat.BottomBorderColor.CopyFrom(baseFormatImpl.BottomBorderColor, false);
      extFormat.TopBorderColor.CopyFrom(baseFormatImpl.TopBorderColor, false);
      extFormat.LeftBorderColor.CopyFrom(baseFormatImpl.LeftBorderColor, false);
      extFormat.RightBorderColor.CopyFrom(baseFormatImpl.RightBorderColor, false);
      //extFormat.Index = ( ushort )m_extFormats.Count;
      extFormat.Gradient = gradient;
      return extFormat;
    }
    /// <summary>
    /// Registers extended format.
    /// </summary>
    /// <param name="format">Format to register.</param>
    /// <returns>
    /// Format from the collection if there were such format;
    /// otherwise returns format that was added.
    /// </returns>
    protected internal ExtendedFormatImpl RegisterExtFormat( ExtendedFormatImpl format )
    {
      return RegisterExtFormat( format, false );
    }
    /// <summary>
    /// Registers extended format.
    /// </summary>
    /// <param name="format">Format to register.</param>
    /// <param name="forceAdd">Indicates whether to force format object registration in the collection.</param>
    /// <returns>
    /// Format from the collection if there were such format;
    /// otherwise returns format that was added.
    /// </returns>
    protected internal ExtendedFormatImpl RegisterExtFormat( ExtendedFormatImpl format, bool forceAdd )
    {
      if( format == null )
        throw new ArgumentNullException( "format" );

      // TODO: Here we should check whether such format is in collection.

      format = forceAdd ?
        m_extFormats.ForceAdd( format ) :
        m_extFormats.Add( format );

      return format;
    }
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Copies to clipboard the selected worksheet or all worksheets if sheet is NULL.
    /// </summary>
    /// <param name="sheet">Worksheet that would be copied into the clipboard.</param>
    protected internal void CopyToClipboard( WorksheetImpl sheet )
    {
      if( sheet == null ) sheet = m_ActiveSheet as WorksheetImpl;

      IWorksheet sheetInp = ( sheet == null ) ? this.Worksheets[ 0 ] : sheet;

      ClipboardProvider provider = AppImplementation.CreateClipboardProvider( sheetInp );

      provider.SetClipboard();

      //#if DEBUG
      //      IDataObject objClipTest = System.Windows.Forms.Clipboard.GetDataObject();
      //      string[] outp = objClipTest.GetFormats();
      //      object data = objClipTest.GetData( "Biff8" );
      //      //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, string.Join( "\n", outp ), "Clipboard" );
      //#endif

    }
    /// <summary>
    /// Copies workbook and all its worksheets to the clipboard.
    /// </summary>
    protected internal void Paste()
    {
      IDataObject obj = System.Windows.Forms.Clipboard.GetDataObject();

      if( obj != null && obj.GetDataPresent( "Biff8", true ) )
      {
        object objData = obj.GetData( "Biff8" );
        
        if( objData != null )
        {
          Trace.WriteLine( "type: " + objData.GetType().FullName, "Data storage" );

          BiffReader m_reader = new BiffReader( ( Stream )objData );
          m_reader.SeekOnBOFRecord();
          Parse( m_reader );
        }
      }
    }
#endif
    /// <summary>
    /// Inserts SupbookRecord describing this workbook.
    /// </summary>
    /// <returns>
    /// Index to the SupBookRecord that describes current workbook.
    /// </returns>
    protected int InsertSelfSupbook()
    {
      return m_externBooks.InsertSelfSupbook();

      //      for( int i = 0, len = m_arrSupBooks.Count; i < len; i++ )
      //      {
      //        // Use 'as' to increase performance.
      //        SupBookRecord supBook = m_arrSupBooks[ i ] as SupBookRecord;
      //
      //        if( supBook.IsInternalReference )
      //        {
      //          supBook.SheetNumber = ( ushort ) ( m_worksheets.Count + m_charts.Count );
      //          return i;
      //        }
      //        i++;
      //      }
      //
      //      SupBookRecord selfBook = ( SupBookRecord )BiffRecordFactory.GetRecord( 
      //        TBIFFRecord.SupBook );
      //      selfBook.IsInternalReference = true;
      //      selfBook.URL = null;
      //      selfBook.SheetNumber = ( ushort ) ( m_worksheets.Count + m_charts.Count );
      //      
      //      m_arrSupBooks.Add( selfBook );
      //      m_arrExternRef.Add( selfBook );
      //      
      //      return m_arrSupBooks.Count - 1;
    }
    /// <summary>
    /// Adds internal sheet reference.
    /// </summary>
    /// <param name="sheetName">Name of the sheet that should be referenced.</param>
    /// <returns>Index to the sheet in ExternSheetRecord.</returns>
    /// <exception cref="System.ArgumentException">
    /// When can't find specified worksheet in this workbook.
    /// </exception>
    protected internal int AddSheetReference( string sheetName )
    {
      string[] arrSheetNames = sheetName.Split( SheetRangeSeparator );

      if( arrSheetNames.Length > 2 )
        throw new ArgumentException( "sheetName" );

      sheetName = arrSheetNames[ 0 ];
      string lastSheetName = arrSheetNames[ arrSheetNames.Length - 1 ];
      IWorksheet sheet = Worksheets[ sheetName ];
      IWorksheet lastSheet = Worksheets[ lastSheetName ];

      if( sheet != null && lastSheet != null )
      {
        return AddSheetReference( sheet, lastSheet );
      }

      Match result = ExternSheetRegEx.Match( sheetName );

      if( result.Success && result.Value == sheetName )
      {
        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled,
        //  result.Groups[ DEF_BOOK_GROUP ], "Workbook name" );

        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled,
        //  result.Groups[ DEF_SHEET_GROUP ], "Worksheet name" );

        string strBookName = result.Groups[ DEF_BOOK_GROUP ].Value;
        int iLength = strBookName.Length;
        int iReferenceIndex = 0;

        if( iLength == 0 )
        {
          iReferenceIndex = AddBrokenSheetReference();
        }
        else
        {
          if( iLength >= 2 )
          {
            strBookName = strBookName.Substring( 1, iLength - 2 );
          }

          string strSheetName = result.Groups[ DEF_SHEET_GROUP ].Value;

          iReferenceIndex = AddExternSheetReference( strBookName, strSheetName );
        }

        return iReferenceIndex;
      }

      return 0;
    }
    /// <summary>
    /// Adds reference to the extern worksheet.
    /// </summary>
    /// <param name="strBookName">
    /// Workbook name (can be null or empty than sheet name is treated as book name.
    /// </param>
    /// <param name="strSheetName">Worksheet name.</param>
    /// <returns>Index in the ExternSheet record.</returns>
    private int AddExternSheetReference( string strBookName, string strSheetName )
    {
      if( strBookName == null )
        throw new ArgumentNullException( "strBookName" );

      if( strSheetName == null )
        throw new ArgumentNullException( "strSheetName" );

      int iBookIndex = -1;
      int iSheetIndex = DEF_BOOK_SHEET_INDEX;
      ExternWorkbookImpl externBook;

      if( strBookName == null || strBookName.Length == 0 )
      {
        // [...] expression was not found, this means that worksheet name is book name
        strBookName = strSheetName;
        strSheetName = null;
      }

      externBook = m_externBooks[ strBookName ];

      if( externBook == null )
      {
        int iExternIndex;

        if( Loading && ( Version !=ExcelVersion.Excel97to2003 ) &&
          int.TryParse( strBookName, out iExternIndex ) )
        {
          iBookIndex = iExternIndex - 1;
          externBook = m_externBooks[ iBookIndex ];
        }
        else
        {
          throw new ArgumentNullException( "Can't find extern workbook" );
        }
      }
      else
      {
        iBookIndex = externBook.Index;
      }

      if( strSheetName != null )
      {
        // We have book name and sheet name thats why it should be easy to locate range.
        //iSheetIndex = externBook.IndexOf( strSheetName );
        iSheetIndex = externBook.IndexOf( strSheetName );
      }

      return AddSheetReference( iBookIndex, iSheetIndex, iSheetIndex );
    }
    /// <summary>
    /// Adds internal sheet reference.
    /// </summary>
    /// <param name="sheet">Name of the sheet that should be referenced.</param>
    /// <returns>Worksheet to be referenced.</returns>
    /// <exception cref="System.ArgumentException">
    /// When can't find specified worksheet in this workbook.
    /// </exception>
    protected internal int AddSheetReference( IWorksheet sheet )
    {
      return AddSheetReference( sheet, sheet );
    }
    /// <summary>
    /// Adds internal sheet reference.
    /// </summary>
    /// <param name="sheet">Name of the sheet that should be referenced.</param>
    /// <returns>Worksheet to be referenced.</returns>
    /// <exception cref="System.ArgumentException">
    /// When can't find specified worksheet in this workbook.
    /// </exception>
    protected internal int AddSheetReference( IWorksheet sheet, IWorksheet lastSheet )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( sheet.Workbook != this )
        throw new ArgumentException( "Can't refer to external worksheets" );

      int supIndex = InsertSelfSupbook();
      int sheetIndex = ( ( ISerializableNamedObject )sheet ).RealIndex;
      int lastSheetIndex = ( ( ISerializableNamedObject )lastSheet ).RealIndex;

      return m_externSheet.AddReference( supIndex, sheetIndex, lastSheetIndex );
    }
    /// <summary>
    /// Adds internal sheet reference.
    /// </summary>
    /// <param name="sheet">Name of the sheet that should be referenced.</param>
    /// <returns>Worksheet to be referenced.</returns>
    /// <exception cref="System.ArgumentException">
    /// When can't find specified worksheet in this workbook.
    /// </exception>
    protected internal int AddSheetReference( ITabSheet sheet )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( sheet.Workbook != this )
        throw new ArgumentException( "Can't refer to external worksheets" );

      if( !( sheet is IWorksheet ) ) return -1;

      int supIndex = InsertSelfSupbook();
      int sheetIndex = ( ( ISerializableNamedObject )sheet ).RealIndex;

      return m_externSheet.AddReference( supIndex, sheetIndex, sheetIndex );
    }
    /// <summary>
    /// This method adds one TREF structure to the list.
    /// </summary>
    /// <param name="supIndex">SUPBOOK index.</param>
    /// <param name="firstSheetIndex">Index to first SUPBOOK sheet.</param>
    /// <param name="lastSheetIndex">Index to last SUPBOOK sheet.</param>
    /// <returns>
    /// Index of the old REF structure (if there was one)
    /// or new REF structure.
    /// </returns>
    protected internal int AddSheetReference( int supIndex, int firstSheetIndex, int lastSheetIndex )
    {
      return m_externSheet.AddReference( supIndex, firstSheetIndex, lastSheetIndex );
    }
    /// <summary>
    /// Adds incorrect sheet reference.
    /// </summary>
    /// <returns>Worksheet to be referenced.</returns>
    /// <exception cref="System.ArgumentException">
    /// When can't find specified worksheet in this workbook.
    /// </exception>
    protected internal int AddBrokenSheetReference()
    {
      int supIndex = InsertSelfSupbook();

      return m_externSheet.AddReference( supIndex, DEF_REMOVED_SHEET_INDEX, DEF_REMOVED_SHEET_INDEX );
    }
    /// <summary>
    /// Decreases index (in ExternSheet record) of all worksheets with index
    /// that is smaller than specified index.
    /// </summary>
    /// <param name="index"></param>
    protected internal void DecreaseSheetIndex( int index )
    {
      if( m_externSheet == null || m_externSheet.Refs == null ) return;

      int iInternalBookIndex = m_externBooks.GetFirstInternalIndex();

      ExternSheetRecord.TREF[] arrRefs = m_externSheet.Refs;
      for( int i = 0, len = arrRefs.Length; i < len; i++ )
      {
        ExternSheetRecord.TREF tref = arrRefs[ i ];

        if( tref.SupBookIndex != iInternalBookIndex )
          continue;

        int iSheetIndex = tref.FirstSheet;

        if( iSheetIndex > index && iSheetIndex != DEF_REMOVED_INDEX && iSheetIndex != DEF_BOOK_SHEET_INDEX )
        {
          tref.FirstSheet--;
        }
        else if( iSheetIndex == index )
        {
          tref.FirstSheet = DEF_REMOVED_INDEX;
        }

        iSheetIndex = tref.LastSheet;

        if( iSheetIndex > index && iSheetIndex != DEF_REMOVED_INDEX && iSheetIndex != DEF_BOOK_SHEET_INDEX )
        {
          tref.LastSheet--;
        }
        else if( iSheetIndex == index )
        {
          tref.LastSheet = DEF_REMOVED_INDEX;
        }
      }
    }
    /// <summary>
    /// Increases index (in ExternSheet record) of all worksheets with index
    /// that is larger or equal than specified index.
    /// </summary>
    /// <param name="index"></param>
    protected internal void IncreaseSheetIndex( int index )
    {
      if( m_externSheet == null || m_externSheet.Refs == null ) return;

      ExternSheetRecord.TREF[] arrRefs = m_externSheet.Refs;
      for( int i = 0, len = arrRefs.Length; i < len; i++ )
      {
        ExternSheetRecord.TREF tref = arrRefs[ i ];

        if( tref.FirstSheet >= index ) tref.FirstSheet++;
        if( tref.LastSheet  >= index ) tref.LastSheet++;
      }
    }
    /// <summary>
    /// This method updates external sheet table when a worksheet was moved.
    /// </summary>
    /// <param name="iOldIndex">Old index of the worksheet.</param>
    /// <param name="iNewIndex">New index of the worksheet.</param>
    protected internal void MoveSheetIndex( int iOldIndex, int iNewIndex )
    {
      if( m_externSheet == null || m_externSheet.Refs == null ) return;

      if( iOldIndex == iNewIndex ) return;
      
      ExternSheetRecord.TREF[] arrRefs = m_externSheet.Refs;

      for( int i = 0, len = arrRefs.Length; i < len; i++ )
      {
        if( IsLocalReference( i ) )
        {
          ExternSheetRecord.TREF curRef = arrRefs[ i ];

          curRef.FirstSheet = ( ushort )GetMovedSheetIndex( curRef.FirstSheet,
            iOldIndex, iNewIndex );

          curRef.LastSheet = ( ushort )GetMovedSheetIndex( curRef.LastSheet,
            iOldIndex, iNewIndex );
        }
      }
    }
    /// <summary>
    /// Updates active sheet index after move operation.
    /// </summary>
    /// <param name="iOldIndex">Old sheet index.</param>
    /// <param name="iNewIndex">New sheet index.</param>
    protected internal void UpdateActiveSheetAfterMove( int iOldIndex, int iNewIndex )
    {
      int iActiveSheet = ActiveSheetIndex;

      if( iOldIndex == iActiveSheet )
      {
        iActiveSheet = iNewIndex;
      }
      else if( iOldIndex < iNewIndex )
      {
        if( iActiveSheet < iOldIndex && iActiveSheet >= iNewIndex )
        {
          iActiveSheet++;
        }
      }
      else if( iActiveSheet <= iNewIndex && iActiveSheet > iOldIndex )
      {
        iActiveSheet--;
      }

      ActiveSheetIndex = iActiveSheet;
    }

    /// <summary>
    /// Gets sheet index after move operation.
    /// </summary>
    /// <param name="iCurIndex">Current sheet index.</param>
    /// <param name="iOldIndex">Old index of the sheet that was moved.</param>
    /// <param name="iNewIndex">New index of the sheet that was moved.</param>
    /// <returns>New index for current sheet index.</returns>
    private int GetMovedSheetIndex( int iCurIndex, int iOldIndex, int iNewIndex )
    {
      if( iOldIndex == iNewIndex ) return iCurIndex;
      if( iCurIndex == iOldIndex ) return iNewIndex;

      int iMin = Math.Min( iOldIndex, iNewIndex );
      int iMax = Math.Max( iOldIndex, iNewIndex );
      
      if( iCurIndex < iMin || iCurIndex > iMax ) return iCurIndex;

      if( iOldIndex > iNewIndex )
      {
        return iCurIndex + 1;
      }
      else
      {
        return iCurIndex - 1;
      }
    }
    /// <summary>
    /// Returns worksheet name.
    /// </summary>
    /// <param name="reference">Reference to worksheet.</param>
    /// <returns>Returns sheet name.</returns>
    protected internal string GetSheetNameByReference( int reference )
    {
      return GetSheetNameByReference( reference, false );
    }
    /// <summary>
    /// Returns worksheet name.
    /// </summary>
    /// <param name="reference">Reference to worksheet.</param>
    /// <returns>Returns sheet name.</returns>
    /// <param name="throwArgumentOutOfRange">Indicates whether to throw exception if reference index is out of range.</param>
    protected internal string GetSheetNameByReference( int reference, bool throwArgumentOutOfRange )
    {
      string strResult = null;

      if( m_externSheet.RefCount <= reference || reference < 0 )
      {
        if( throwArgumentOutOfRange )
          throw new ArgumentOutOfRangeException( "reference" );

        return null;
      }

      ExternSheetRecord.TREF tref = m_externSheet.Refs[ reference ];

      int iSupBook = tref.SupBookIndex;

      if( iSupBook > m_externBooks.Count )
      {
        throw new ParseException();
      }
      else
      {

        ExternWorkbookImpl book = m_externBooks[ iSupBook ];

        try
        {
            strResult = (!book.IsInternalReference) ?
              GetExternalSheetNameByReference(book, tref, iSupBook) :
              GetInternalSheetNameByReference(tref);
        }
        catch(Exception e)
        {

        }
      }

      return strResult;
    }
    /// <summary>
    /// Get name of the external worksheet by reference.
    /// </summary>
    private string GetExternalSheetNameByReference( ExternWorkbookImpl book, ExternSheetRecord.TREF reference,
      int iSupBook )
    {
      int sheetIndex = reference.FirstSheet;
      string strPath = GetDirectoryName( book.URL );
      string strResult = null;

      //if( strPath != null && strPath.Length > 0 && strPath[ strPath.Length - 1 ] != '\\' )
      //  strPath += '\\';

      string strFileName = Path.GetFileName( book.URL );

      if( m_bSaving )
      {
        int iCount = 0;
        // TODO: optimize this
        for( int i = iSupBook - 1; i >= 0; i-- )
        {
          ExternWorkbookImpl curBook = m_externBooks[ i ];

          if( curBook.IsInternalReference || string.IsNullOrEmpty( curBook.URL ) )
            iCount++;
        }

        strResult = string.Format( "[{0}]", iSupBook - iCount + 1 );
      }
      else
      {
        strResult = strPath + '[' + strFileName + ']';
      }

      strResult += book.GetSheetName( sheetIndex );

      return strResult;
    }
    /// <summary>
    /// Get name of the internal worksheet by reference.
    /// </summary>
    private string GetInternalSheetNameByReference( ExternSheetRecord.TREF reference )
    {
      string strResult = null;
      int sheetIndex = reference.FirstSheet;
      //int realIndex = m_hashSheetIndexes[ sheetIndex ];

      if( sheetIndex == DEF_REMOVED_SHEET_INDEX )
      {
        strResult = DEF_BAD_SHEET_NAME;
      }
      else if( ObjectCount <= sheetIndex || sheetIndex < 0 )
      {
        throw new ParseException();
      }
      else
      {
        object result = Objects[ sheetIndex ];

        if( result is IWorksheet )
        {
          strResult = ( ( IWorksheet )result ).Name;
        }

        if( reference.FirstSheet != reference.LastSheet )
        {
          result = Objects[ reference.LastSheet ];

          if( result is IWorksheet )
          {
            strResult += ":" + ( ( IWorksheet )result ).Name;
          }
        }
      }

      return strResult;
    }

    private string GetDirectoryName( string url )
    {
      if( url == null )
        return null;

      string strResult = null;

      if( url.StartsWith( "http://" ) )
      {
        int index = url.LastIndexOf( '/' );
        strResult = url.Substring( 0, index + 1 );
      }
      else
      {
        strResult = Path.GetDirectoryName( url );

        if( strResult != null && strResult.Length > 0 && strResult[ strResult.Length - 1 ] != '\\' )
          strResult += '\\';
      }

      return strResult;
    }
    /// <summary>
    /// Returns worksheet by its reference index.
    /// </summary>
    /// <param name="reference">Reference index of the sheet.</param>
    /// <returns>Found worksheet.</returns>
    /// <exception cref="Syncfusion.XlsIO.Implementation.Exceptions.ParseException">
    /// When can't find referenced worksheet.
    /// </exception>
    protected internal IWorksheet GetSheetByReference( int reference )
    {
      return GetSheetByReference( reference, true );
    }
    /// <summary>
    /// Returns worksheet by its reference index.
    /// </summary>
    /// <param name="reference">Reference index of the sheet.</param>
    /// <param name="bThrowExceptions">
    /// Indicates whether to throw exception when can't
    /// find worksheet with specified index.
    /// </param>
    /// <returns>Found worksheet.</returns>
    /// <exception cref="Syncfusion.XlsIO.Implementation.Exceptions.ParseException">
    /// When can't find referenced worksheet.
    /// </exception>
    protected internal IWorksheet GetSheetByReference( int reference, bool bThrowExceptions )
    {
      if( m_externSheet.RefCount <= reference || reference < 0 )
      {
        if( bThrowExceptions )
          throw new ArgumentOutOfRangeException( "reference" );
      
        return null;
      }

      ExternSheetRecord.TREF tref = m_externSheet.Refs[ reference ];

      int iSupBook = tref.SupBookIndex;

      if( iSupBook > m_externBooks.Count ) //m_arrSupBooks.Count )
      {
        if( bThrowExceptions ) throw new ParseException();

        return null;
      }

      //SupBookRecord supBook = ( SupBookRecord )m_arrSupBooks[ iSupBook ];
      ExternWorkbookImpl book = m_externBooks[ iSupBook ];

      if( !book.IsInternalReference )
      {
        if( bThrowExceptions )
          throw new ParseException();

        return null;
      }
      else
      {
        int sheetIndex = tref.FirstSheet;
        //int realIndex = m_hashSheetIndexes[ sheetIndex ];

        if( ObjectCount <= sheetIndex || sheetIndex < 0 )
        {
          if( bThrowExceptions )
            throw new ParseException();

          return null;
        }

        object result = Objects[ sheetIndex ];

        if( result is IWorksheet )
          return ( IWorksheet )result;

        if( bThrowExceptions )
        {
          throw new ArgumentOutOfRangeException( "Can't find worksheet at the specified index" );
        }
        else
        {
          return null;
        }
      }
    }
    /// <summary>
    /// Check for internal Reference; If external - rise NotSupported exception.
    /// </summary>
    /// <param name="iRef">Ref index.</param>
    protected internal void CheckForInternalReference( int iRef )
    {
      if( m_externSheet.RefCount <= iRef || iRef < 0 )
        throw new ArgumentOutOfRangeException( "iRef" );
      
      int supIndex = m_externSheet.Refs[ iRef ].SupBookIndex;
      ExternSheetRecord.TREF tref = m_externSheet.Refs[ iRef ];
      
      if( supIndex > m_externBooks.Count )
        throw new ParseException();
      
      ExternWorkbookImpl book = m_externBooks[ supIndex ];
      
      if( !book.IsInternalReference )
        throw new NotSupportedException( "External indexes are not supported in current version." );
    }
    /// <summary>
    /// Indicates whether reference is reference to local worksheet.
    /// </summary>
    /// <param name="reference">Reference index.</param>
    /// <returns>Value that indicates whether reference is reference to local worksheet.</returns>
    protected internal bool IsLocalReference( int reference )
    {
      if( m_externSheet.RefCount <= reference || reference < 0 )
        return false;

      //int supIndex = m_externSheet.Refs[ reference ].SupBookIndex;
      ExternSheetRecord.TREF tref = m_externSheet.Refs[ reference ];

      int iSupBook = tref.SupBookIndex;

      if( iSupBook > m_externBooks.Count )
      {
        return false;
      }

      return m_externBooks[ iSupBook ].IsInternalReference;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reference"></param>
    /// <returns></returns>
    public bool IsExternalReference( int reference )
    {
      if( reference == DEF_REMOVED_SHEET_INDEX )
        return false;

      if (m_externSheet.RefCount < 0 || m_externSheet.RefCount <= reference)
          return false;

      ExternSheetRecord.TREF referenceSheet = m_externSheet.Refs[ reference ];

      if( referenceSheet.FirstSheet == DEF_REMOVED_SHEET_INDEX )
        return false;

      int supbookIndex = referenceSheet.SupBookIndex;
      
      if( supbookIndex < 0 || supbookIndex >= m_externBooks.Count )
        throw new ArgumentOutOfRangeException( "supbookIndex" );

      return !m_externBooks[ supbookIndex ].IsInternalReference;
    }
    /// <summary>
    /// Inserts reparse into array of object that should be reparsed
    /// when loading will be complete.
    /// </summary>
    /// <param name="reparse">Object that will be reparsed later.</param>
    internal void AddForReparse( IReparse reparse )
    {
      m_arrReparse.Add( reparse );
    }
    /// <summary>
    /// Returns number from the style name, i.e. Normal_1 result is 1.
    /// </summary>
    /// <param name="pre">Style name.</param>
    /// <returns>Parsed number.</returns>
    protected internal int CurrentStyleNumber( string pre )
    {
      int number = 0;

      IStyles styles = Styles;
      for( int i = 0, len = styles.Count; i < len; i++ )
      {
        IStyle style = styles[ i ];
        string name = style.Name;
        int pos = name.IndexOf( pre );
        if( pos >= 0 )
        {
          string numberSub = name.Substring( pos + pre.Length, name.Length - pre.Length - pos );
            
          double doubleValue;
            
          if( double.TryParse( numberSub, NumberStyles.Integer, null, out doubleValue ) )
          {
            int tmp = ( int )doubleValue;
            if( tmp > number ) number = tmp;
          }
        }
      }
      
      return number;
    }
    /// <summary>
    /// Raises argument to the second power.
    /// </summary>
    /// <param name="value">Value to be squared.</param>
    /// <returns>Squared value.</returns>
    protected double Sqr( double value )
    {
      return value * value;
    }
    /// <summary>
    /// Calculates distance between two colors.
    /// </summary>
    /// <param name="color1">First color.</param>
    /// <param name="color2">Second color.</param>
    /// <returns>Distance between two colors.</returns>
    protected internal double ColorDistance( Color color1, Color color2 )
    {
      return Math.Sqrt( Sqr( color1.R - color2.R ) + Sqr( color1.B - color2.B )
        + Sqr( color1.G - color2.G ) );
    }
    /// <summary>
    /// Clears collection of references.
    /// </summary>
    public void ClearInternalReferences()
    {
      m_externSheet.Refs = new ExternSheetRecord.TREF[ 0 ]{};
    }
    /// <summary>
    /// Raises FileSaved event.
    /// </summary>
    private void RaiseSavedEvent()
    {
      if( OnFileSaved != null )
      {
        OnFileSaved( this, EventArgs.Empty );
      }
    }
#if !(WINRT || WP)
    /// <summary>
    /// Raises OnReadOnlyFile event.
    /// </summary>
    private void RaiseReadOnlyFileEvent( string strFullPath )
    {
      if( OnReadOnlyFile != null )
      {
        ReadOnlyFileEventArgs args = new ReadOnlyFileEventArgs();
        OnReadOnlyFile( this, args );

        if( args.ShouldRewrite )
        {
          FileAttributes attrib = 
#if !SILVERLIGHT && !WINRT && !WP
            File.GetAttributes( strFullPath );
#else
            FileAttributes.Normal;
#endif
          attrib = attrib & ~FileAttributes.ReadOnly;
          File.SetAttributes( strFullPath, attrib );
        }
      }
      else
      {
        throw new ApplicationException( "File " + strFullPath + " is read-only" );
      }
    }
#endif
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public IExtendedFormat GetExtFormat( int index )
    {
      return ( IExtendedFormat )m_extFormats[ index ];
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sourceRange"></param>
    /// <param name="destRange"></param>
    public void UpdateFormula( IRange sourceRange, IRange destRange )
    {
      if( sourceRange == null )
        throw new ArgumentNullException( "sourceRange" );

      if( destRange == null )
        throw new ArgumentNullException( "destRange" );

      RangeImpl source = ( RangeImpl )sourceRange;
      RangeImpl dest = ( RangeImpl )destRange;

      WorksheetImpl sheetSource = source.InnerWorksheet;
      WorksheetImpl sheetDest = dest.InnerWorksheet;

      int iSourceIndex = AddSheetReference( sheetSource );
      int iDestIndex = AddSheetReference( sheetDest );

      Rectangle rectSource = Rectangle.FromLTRB( source.FirstColumn - 1, source.FirstRow - 1,
        source.LastColumn - 1, source.LastRow - 1 );

      Rectangle rectDest = Rectangle.FromLTRB( dest.FirstColumn - 1, dest.FirstRow - 1,
        dest.LastColumn - 1, dest.LastRow - 1 );

      UpdateFormula( iSourceIndex, rectSource, iDestIndex, rectDest );
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="iSourceIndex"></param>
    /// <param name="rectSource"></param>
    /// <param name="iDestIndex"></param>
    /// <param name="rectDest"></param>
    public void UpdateFormula( int iSourceIndex, Rectangle rectSource,
      int iDestIndex, Rectangle rectDest )
    {
      for( int i = 0, len = m_arrObjects.Count; i < len; i++ )
      {
        WorksheetBaseImpl tabSheet = m_arrObjects[ i ] as WorksheetBaseImpl;
        int iCurIndex = AddSheetReference( tabSheet );

        tabSheet.UpdateFormula( iCurIndex, iSourceIndex, rectSource, iDestIndex, rectDest );
      }
    }
    /// <summary>
    /// Returns reference index by extern workbook index.
    /// </summary>
    /// <param name="iNameBookIndex">Index of the extern workbook.</param>
    /// <returns>Reference index if extern workbook was found; otherwise returns -1.</returns>
    public int GetReferenceIndex( int iNameBookIndex )
    {
      return m_externSheet.GetBookReference( iNameBookIndex );
    }
    /// <summary>
    /// Returns extern workbook index by reference index.
    /// </summary>
    /// <param name="iReferenceIndex">Reference index.</param>
    /// <returns>Extern workbook index.</returns>
    public int GetBookIndex( int iReferenceIndex )
    {
      ExternSheetRecord.TREF[] arrRefs = m_externSheet.Refs;

      if( iReferenceIndex < 0 || iReferenceIndex > arrRefs.Length - 1 )
        throw new ArgumentOutOfRangeException( "iReferenceIndex", "Value cannot be less than 0 and greater than arrRefs.Count - 1" );

      return m_externSheet.Refs[ iReferenceIndex ].SupBookIndex;
    }
    /// <summary>
    /// Returns external sheet object by reference index.
    /// </summary>
    /// <param name="referenceIndex">Reference index.</param>
    /// <returns>External worksheet that corresponds to the specified reference index.</returns>
    public ExternWorksheetImpl GetExternSheet( int referenceIndex )
    {
      ExternSheetRecord.TREF[] arrRefs = m_externSheet.Refs;

      if( referenceIndex < 0 || referenceIndex > arrRefs.Length - 1 )
        throw new ArgumentOutOfRangeException( "referenceIndex", "Value cannot be less than 0 and greater than arrRefs.Count - 1" );

      ExternWorksheetImpl sheet = null;
      ExternSheetRecord.TREF reference = m_externSheet.Refs[ referenceIndex ];
      int iSheetIndex;

      if( ( iSheetIndex = reference.FirstSheet ) == reference.LastSheet && iSheetIndex != DEF_REMOVED_SHEET_INDEX )
      {
        int iBookIndex = reference.SupBookIndex;
        sheet = m_externBooks[ iBookIndex ].Worksheets.Values[ iSheetIndex ];
      }

      return sheet;
    }
    /// <summary>
    /// Decodes name encoded in supbook.
    /// </summary>
    /// <param name="strName">Name to decode.</param>
    /// <returns>Decoded name.</returns>
    public string DecodeName( string strName )
    {
      if( strName == null )
        throw new ArgumentNullException( "strName" );

      if( strName.Length == 0 )
        throw new ArgumentException( "strName - string cannot be empty" );

      char chFirst = strName[ 0 ];

      if( chFirst == DEF_CHAR_SELF ) return strName;

      if( chFirst == DEF_CHAR_EMPTY ) return string.Empty;

      if( chFirst != DEF_CHAR_CODED ) return strName.Replace( '\x3', '|' );

      StringBuilder builder = new StringBuilder();
      int iLen = strName.Length;
      int i = 1;

      //      string strFullName = Path.GetFullPath( strName );
      char chDrive = ( m_strFullName != null ) ? m_strFullName[ 0 ] : GetDriveName();
      //bool bUrl = false;
      char chSeparator =
#if ( WINRT )
 '/';
#else
          Path.DirectorySeparatorChar;
#endif
      while( i < iLen )
      {
        chFirst = strName[ i ];

        if( builder.Length == 5 && builder.ToString() == HttpStart )
          chSeparator = '/';

        switch( chFirst )
        {
          case DEF_CHAR_VOLUME:
            i++;

            if( strName[ i ] != DEF_CHAR_NETWORKPATH )
            {
              builder.Append( strName[ i ] );
              builder.Append(
#if ( WINRT )
':'
#else
                Path.VolumeSeparatorChar 
#endif
);
              builder.Append( chSeparator );
            }
            else
            {
              builder.Append( DEF_NETWORKPATH_START );
            }

            break;

          case DEF_CHAR_SAMEVOLUME:
            builder.Append( chDrive );
            builder.Append( 
#if ( WINRT )
                  ':'
#else
                Path.VolumeSeparatorChar 
#endif
                  );
            builder.Append( chSeparator );
            break;

          case DEF_CHAR_DOWNDIR:
            builder.Append( chSeparator );
            break;

          case DEF_CHAR_UPDIR:
            throw new NotImplementedException();

          case DEF_CHAR_STARTUPDIR:
            builder.Append( '\\' );//DEF_CHAR_STARTUPDIR );
            break;

          case DEF_CHAR_ALTSTARTUPDIR:
            builder.Append( strName[ i ] );
            break;

          case DEF_CHAR_LIBDIR:
            builder.Append( strName[ i ] );
            break;

          case DEF_CHAR_LONGVOLUME:
            int iLength = ( int )strName[ i + 1 ];
            builder.Append( strName.Substring( i + 2, iLength ) );
            i += iLength;
            break;

          default:
            builder.Append( strName[ i ] );
            break;
        }

        i++;
      }

      return builder.ToString();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private static char GetDriveName()
    {
#if AllowUnsafeCode
      return Environment.CurrentDirectory[ 0 ];
#elif  (SILVERLIGHT || WP)
      return 'C';
#elif ( WINRT )
        return Windows.Storage.ApplicationData.Current.LocalFolder.Path[0];
#else
      return HttpContext.Current.Server.MapPath( "t" )[ 0 ];
#endif
    }
    /// <summary>
    /// Decodes name encoded in supbook.
    /// </summary>
    /// <param name="strName">Name to decode.</param>
    /// <returns>Decoded name.</returns>
    public string EncodeName( string strName )
    {
      if( strName == null || strName.Length == 0 )
        return strName;

      if( strName.IndexOfAny( DEF_RESERVED_BOOK_CHARS ) != -1
        || strName == ExternBookCollection.DEF_WRONG_URL_NAME )
      {
        return strName.Replace( '|', '\x3' );
      }

      StringBuilder builder = new StringBuilder();
      builder.Append( DEF_CHAR_CODED );
      bool bHttpUrl = strName.StartsWith( HttpStart );

      char chFirst;
      int iStartChar = 0;
      int iLength = ( strName != null ) ? strName.Length : 0;

      //      try
      //      {
      //        strName = Path.GetFullPath( strName );
      //      }
      //      catch( ArgumentException )
      //      {
      //        return strName;
      //      }

      if( strName.StartsWith( DEF_NETWORKPATH_START ) )
      {
        builder.Append( DEF_CHAR_VOLUME );
        builder.Append( DEF_CHAR_NETWORKPATH );
        iStartChar = DEF_NETWORKPATH_START.Length;
      }
      else if( bHttpUrl )
      {
        builder.Append( DEF_CHAR_LONGVOLUME );
        builder.Append( ( char )strName.Length );
        builder.Append( strName );
      }
      else if( iLength > 2 && strName[ 2 ] == '\\' )
      {
        builder.Append( DEF_CHAR_VOLUME );
        chFirst = strName[ 0 ];

        builder.Append( chFirst );
        iStartChar = 3; // drive letter + ':' + '\'
      }
      else if( strName[ 0 ] == '\\' )
      {
        builder.Append( DEF_CHAR_STARTUPDIR );
        strName = UtilityMethods.RemoveFirstCharUnsafe( strName );
      }
      //      else
      //      {
      //        builder.Length = 0;
      //      }

      if( !bHttpUrl )
      {
        int iLen = strName.Length;
        strName = strName.Substring( iStartChar );
        string[] arrParts = 
#if ( WINRT )
            strName.Split( new char[]{ '\\','/'} );
#else
            strName.Split( new char[]{ Path.AltDirectorySeparatorChar,
                                                     Path.DirectorySeparatorChar } );
#endif
        iLen = arrParts.Length;

        for( int i = 0; i < iLen; i++ )
        {
          builder.Append( arrParts[ i ] );

          if( i != iLen - 1 )
          {
            builder.Append( DEF_CHAR_DOWNDIR );
          }
        }
      }

      return builder.ToString();
    }
    /// <summary>
    /// Modifies record in skip styles mode.
    /// </summary>
    /// <param name="record">Record to modify.</param>
    /// <returns>Boolean value indicating that record should be added to the array.</returns>
    [ CLSCompliant( false ) ]
    public bool    ModifyRecordToSkipStyle( BiffRecordRaw record )
    {
      if( record == null )
        throw new ArgumentNullException( "record" );

      FontImpl fontImpl = ( FontImpl )InnerFonts[ 0 ];
      FontRecord font = fontImpl.Record;

      switch( record.TypeCode )
      {
        case TBIFFRecord.ChartFbi:
          ChartFbiRecord fbi = ( ChartFbiRecord )record;
          fbi.FontIndex = 0;
          fbi.AppliedFontHeight = font.FontHeight;
          return false;

        case TBIFFRecord.ChartAlruns:
          ChartAlrunsRecord chartRuns = ( ChartAlrunsRecord )record;
          TRuns[] arrRuns = chartRuns.Runs;

          for( int i = 0, len = arrRuns.Length; i < len; i++ )
          {
            arrRuns[ i ].FontIndex = 0;
          }
          break;

        case TBIFFRecord.ChartFontx:
          ( ( ChartFontxRecord )record ).FontIndex = 0;
          break;
      }

      return true;
    }
    /// <summary>
    /// Modifies record in skip styles mode.
    /// </summary>
    /// <param name="arrRecords">Records to modify.</param>
    [ CLSCompliant( false ) ]
    public void    ModifyRecordToSkipStyle( BiffRecordRaw[] arrRecords )
    {
      if( arrRecords == null )
        throw new ArgumentNullException( "arrRecords" );

      FontImpl fontImpl = ( FontImpl )m_fonts[ 0 ];
      FontRecord font = fontImpl.Record;

      for( int i = 0, len = arrRecords.Length; i < len; i++ )
      {
        BiffRecordRaw record = arrRecords[ i ];

        switch( record.TypeCode )
        {
          case TBIFFRecord.ChartFbi:
            ChartFbiRecord fbi = ( ChartFbiRecord )record;
            fbi.FontIndex = 0;
            fbi.AppliedFontHeight = font.FontHeight;
            break;

          case TBIFFRecord.ChartAlruns:
            ChartAlrunsRecord chartRuns = ( ChartAlrunsRecord )record;
            TRuns[] arrRuns = chartRuns.Runs;

            for( int j = 0, lenJ = arrRuns.Length; j < lenJ; j++ )
            {
              arrRuns[ j ].FontIndex = 0;
            }
            break;

          case TBIFFRecord.ChartFontx:
            ( ( ChartFontxRecord )record ).FontIndex = 0;
            break;

            //            case TBIFFRecord.ChartPlotGrowth:
            //              ChartPlotGrowthRecord plotGrowth = ( ChartPlotGrowthRecord )record;
            //              plotGrowth.VertGrowth = 65536;
            //              plotGrowth.HorzGrowth = 65536;
            //              break;
        }
      }
    }
    /// <summary>
    /// Compares two colors.
    /// </summary>
    /// <param name="color1">First color to compare.</param>
    /// <param name="color2">Second color to compare.</param>
    /// <returns>True if colors are equal.</returns>
    private bool CompareColors( Color color1, Color color2 )
    {
      return color1.R == color2.R && color1.G == color2.G && color1.B == color2.B;
    }
    /// <summary>
    /// Removes extended format by its index.
    /// </summary>
    /// <param name="xfIndex">Index to the extended format to remove.</param>
    public void RemoveExtenededFormatIndex( int xfIndex )
    {
      Dictionary<int, int> dictFormats = m_extFormats.RemoveAt( xfIndex );

      for( int i = 0, len = m_arrObjects.Count; i < len; i++ )
      {
        WorksheetBaseImpl tabSheet = m_arrObjects[ i ] as WorksheetBaseImpl;
        tabSheet.UpdateExtendedFormatIndex( dictFormats );
      }

      m_styles.UpdateStyleRecords();
    }
    /// <summary>
    /// Adds licensing worksheet if necessary.
    /// </summary>
    private void AddLicenseWorksheet()
    {
      // NOTE: maybe we have to check whether such worksheet exists.
      if( AppImplementation.EvalExpired )
      {
        IWorksheet sheet = Worksheets.Create( EvaluationSheetName );
        sheet.TabColorRGB = ColorExtension.Red;
        sheet[ "A1" ].Text = EvaluationWarning;
        IFont font = sheet[ "A1" ].CellStyle.Font;
        font.Size = 14;
        font.Bold = true;
        font.RGBColor = ColorExtension.Red;

        string password = string.Empty;
        Random rnd = new Random( ( int )DateTime.Now.Ticks );

        for( int i = 0; i < 10; i++ )
        {
          byte value = ( byte )( rnd.Next( 26 ) + ( byte )'A' );
          password += ( char )value;
        }

        sheet.Protect( password, ExcelSheetProtection.All );
        sheet.Activate();
      }
    }
    /// <summary>
    /// Checks whether workbook contains licensing worksheet and removes it.
    /// </summary>
    private void CheckLicensingSheet()
    {
      if( AppImplementation.EvalExpired )
      {
        IWorksheet sheet = m_worksheets[ EvaluationSheetName ];

        if( sheet != null )
          sheet.Remove();
        //for( int i = 0, len = m_worksheets.Count; i < len; i++ )
        //{
        //  IWorksheet sheet = m_worksheets[ i ];

        //  if( sheet.Name.StartsWith( EvaluationSheetName ) && CheckProtectionContent( sheet ) )
        //  {
        //    sheet.Remove();
        //    break;
        //  }
        //}
      }
    }
    /// <summary>
    /// Checks whether protected content is correct.
    /// </summary>
    /// <param name="sheet">Worksheet to check.</param>
    /// <returns>True if content is correct.</returns>
    private bool CheckProtectionContent( IWorksheet sheet )
    {
      IRange range = sheet[ "A1" ];
      return range.Text == EvaluationWarning && range.ColumnWidth > 8 &&
        range.RowHeight > 10 &&
        range.CellStyle.Font.Size > 10 &&
        sheet.TopVisibleRow == 1 &&
        sheet.LeftVisibleColumn == 1 &&
        sheet.Visibility == WorksheetVisibility.Visible;
    }
    /// <summary>
    /// Optimizes internal references by removing unused ones.
    /// </summary>
    private void OptimizeReferences()
    {
      // first of all we have to find used references
      // a. worksheet cells
      // b. named ranges
      // c. data validations
      // d. conditional formats
      // e. charts
      int iCount = m_externSheet.RefCount;
      bool[] arrUsedItems = new bool[ iCount ];

      for( int i = 0, len = m_arrObjects.Count; i < len; i++ )
      {
        WorksheetBaseImpl sheet = m_arrObjects[ i ] as WorksheetBaseImpl;
        sheet.MarkUsedReferences( arrUsedItems );
      }

      m_names.MarkUsedReferences( arrUsedItems );

      int[] arrUpdatedIndexes = new int[ iCount ];
      int iFreeCount = 0;

      for( int i = 0; i < iCount; i++ )
      {
        if( arrUsedItems[ i ] )
        {
          arrUpdatedIndexes[ i ] = i - iFreeCount;
        }
        else
        {
          arrUpdatedIndexes[ i ] = -1;
          iFreeCount++;
        }
      }

      UpdateReferenceIndexes( arrUpdatedIndexes );
    }
    /// <summary>
    /// Updates reference indexes.
    /// </summary>
    /// <param name="arrUpdatedIndexes">Array with updated indexes.</param>
    private void UpdateReferenceIndexes( int[] arrUpdatedIndexes )
    {
      for( int i = 0, len = m_arrObjects.Count; i < len; i++ )
      {
        WorksheetBaseImpl sheet = m_arrObjects[ i ] as WorksheetBaseImpl;
        sheet.UpdateReferenceIndexes( arrUpdatedIndexes );
      }

      m_names.UpdateReferenceIndexes( arrUpdatedIndexes );

      ExternSheetRecord.TREF[] arrRefs = m_externSheet.Refs;
      List<ExternSheetRecord.TREF> arrNewRefs = new List<ExternSheetRecord.TREF>();

      for( int i = 0, len = arrRefs.Length; i < len; i++ )
      {
        if( arrUpdatedIndexes[ i ] >= 0 )
          arrNewRefs.Add( arrRefs[ i ] );
      }

      m_externSheet.Refs = arrNewRefs.ToArray();
    }
    /// <summary>
    /// Updates pivot caches after insert.
    /// </summary>
    /// <param name="worksheet">Worksheet where insert operation took place.</param>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <param name="isRow"></param>
    /// <param name="isRemove">Indicates whether this is remove operation.</param>
    public void UpdatePivotCachesAfterInsertRemove( WorksheetImpl worksheet, int index,
      int count, bool isRow, bool isRemove )
    {
      if( m_pivotCaches != null && m_pivotCaches.Count > 0 )
      {
        foreach( PivotCacheImpl cache in m_pivotCaches )
        {
          cache.UpdateAfterInsertRemove( worksheet, index, count, isRow, isRemove );
        }
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Static constructor.
    /// </summary>
    static WorkbookImpl()
    {
      //string[] names = Enum.GetNames( typeof( ExcelSheetType ) );
      //Type t = typeof( ExcelSheetType );
      
      //for( int i = 0; i < names.Length; i++ )
      //{
      //  MemberInfo[] memberInfo = t.GetMember( names[ i ] );
        
      //  DescriptionAttribute attr = ( DescriptionAttribute )
      //    Attribute.GetCustomAttribute( memberInfo[ 0 ], typeof( DescriptionAttribute ) );
        
      //  ExcelSheetType value = ( ExcelSheetType )
      //    Enum.Parse( typeof( ExcelSheetType ), names[ i ], true );

      //  if( attr != null )
      //  {
      //    SheetTypeToName.Add( value, attr.Description );
      //  }
      //}
      SheetTypeToName.Add( ExcelSheetType.Chart, "Charts" );
      SheetTypeToName.Add( ExcelSheetType.DialogSheet, "Dialogs" );
      SheetTypeToName.Add( ExcelSheetType.Excel4IntlMacroSheet, "Excel 4.0 Intl Marcos" );
      SheetTypeToName.Add( ExcelSheetType.Excel4MacroSheet, "Excel 4.0 Macros" );
      SheetTypeToName.Add( ExcelSheetType.Worksheet, "Worksheets" );
   
    }
    /// <summary>
    /// Base constructor which must be used when workbook is created 
    /// from scratch (maybe clipboard data)and  not from file. 
    /// </summary>
    /// <param name="application">Application object for the workbook.</param>
    /// <param name="parent">Parent object for the workbook.</param>
    /// <param name="version">Excel version.</param>
    public WorkbookImpl( IApplication application, object parent, ExcelVersion version )
      : this( application, parent, application.SheetsInNewWorkbook, version )
    {
    }
    /// <summary>
    /// Creates workbook with specific number of worksheets.
    /// </summary>
    /// <param name="application">Application object for the workbook.</param>
    /// <param name="parent">Parent object for the workbook.</param>
    /// <param name="sheetQuantity">Quantity of empty worksheets to create.</param>
    /// <param name="version">Excel version.</param>
    public WorkbookImpl( IApplication application, object parent, int sheetQuantity, ExcelVersion version )
      : base( application, parent )
    {
      InitializeCollections();
      Version = version;
      AppImplementation.CheckDefaultFont(this);
      InsertDefaultFonts();
      InsertDefaultValues();
      m_bReadOnly = false;
	  m_bIsCreated = true;
      int iCount = sheetQuantity;
      m_worksheets.EnsureCapacity( iCount );

      for( int i = 0; i < iCount; i++ )
      {
        m_worksheets.Add( string.Format( "Sheet{0}", i + 1 ) );
      }

      m_worksheets[ 0 ].Activate();
    }
#if !(WINRT )
    /// <summary>
    /// Create Workbook from file.
    /// </summary>
    /// <param name="application">Application object for the workbook.</param>
    /// <param name="parent">Parent object for the workbook.</param>
    /// <param name="FileName">Name of the file with workbook.</param>
    /// <param name="version">Excel version.</param>
    /// <exception cref="System.ApplicationException">
    /// When can't find workbook stream in the file.
    /// </exception>
    public WorkbookImpl( IApplication application, object parent, string FileName, ExcelVersion version )
      : this( application, parent, FileName, ExcelParseOptions.Default, version )
    {
    }
    /// <summary>
    /// Create Workbook from file.
    /// </summary>
    /// <param name="application">Application object for the workbook.</param>
    /// <param name="parent">Parent object for the workbook.</param>
    /// <param name="strFileName">Name of the file with workbook.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="version">Excel version.</param>
    /// <exception cref="System.ApplicationException">
    /// When can't find workbook stream in the file.
    /// </exception>
    public WorkbookImpl( IApplication application, object parent, string strFileName
      , ExcelParseOptions options, ExcelVersion version )
      : this( application, parent, strFileName, options, false, null, version )
    {
    }
    /// <summary>
    /// Create Workbook from file.
    /// </summary>
    /// <param name="application">Application object for the workbook.</param>
    /// <param name="parent">Parent object for the workbook.</param>
    /// <param name="strFileName">Name of the file with workbook.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="bReadOnly">Indicates whether to open workbook in read-only mode.</param>
    /// <param name="password">Password to decrypt workbook stream.</param>
    /// <param name="version">Excel version.</param>
    /// <exception cref="System.ApplicationException">
    /// When can't find workbook stream in the file.
    /// </exception>
    public WorkbookImpl( IApplication application, object parent, string strFileName
      , ExcelParseOptions options, bool bReadOnly, string password, ExcelVersion version )
      : base( application, parent )
    {
      m_bOptimization = application.OptimizeFonts;
      InitializeCollections();
      Version = version;
      ParseFile(strFileName, password, version, options, bReadOnly);

      //if( version == ExcelVersion.Excel97to2003 )
      //{
      //  STGM storageOptions = bReadOnly
      //    ? StgStream.DEF_STORAGE_READONLY
      //    : StgStream.DEF_STORE_READONLY;

      //  // Open compound file / storage.
      //  m_workbookFile = AppImplementation.CreateCompoundFile( strFileName, storageOptions );
      //  ICompoundStorage storage = m_workbookFile.RootStorage;
      //  ParseStgStream( storage, options, password );
      //}
      //else if( version == ExcelVersion.Excel2007 || version == ExcelVersion.Excel2010 )
      //{
      //  using( FileStream stream = new FileStream( strFileName, FileMode.Open, FileAccess.Read ) )
      //  {
      //    ParseExcel2007Stream( stream, password );
      //  }
      //}
      //else
      //{
      //  throw new ArgumentOutOfRangeException( "version" );
      //}

      // Set full name.
      m_strFullName = System.IO.Path.GetFullPath( strFileName );
    }
#endif
         /// <summary>
    /// Initializes a new instance of the <see cref="WorkbookImpl"/> class.
    /// </summary>
    /// <param name="application">Application object for the workbook.</param>
    /// <param name="parent">Parent object for the workbook.</param>
    /// <param name="strFileName">Name of the file with workbook.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="bReadOnly">Indicates whether to open workbook in read-only mode.</param>
    /// <param name="password">Password to decrypt workbook stream.</param>
    /// <param name="version">Excel version.</param>
    /// <exception cref="System.ApplicationException">
    /// When can't find workbook stream in the file.
    /// </exception>
    public WorkbookImpl(IApplication application, object parent, Stream stream
    , ExcelParseOptions options, bool bReadOnly, string password, ExcelVersion version)
        : base(application, parent)
    {
        m_bOptimization = application.OptimizeFonts;
        InitializeCollections();
        Version = version;
        ParseStream(stream, password, version, options);
    }
    /// <summary>
    /// Create WorkBook from file.
    /// </summary>
    /// <param name="application">Application object for the workbook.</param>
    /// <param name="parent">Parent object for the workbook.</param>
    /// <param name="stream">Stream to open.</param>
    /// <param name="separator">Current separator.</param>
    /// <param name="row">Number of first row to write.</param>
    /// <param name="column">Number of first column to write.</param>
    /// <param name="version">Excel version.</param>
    /// <param name="fileName">Filename is used to generate worksheet name</param>
    public WorkbookImpl( IApplication application, object parent, Stream stream
      , string separator, int row, int column, ExcelVersion version, string fileName, Encoding encoding )
      : this( application, parent, 1, version )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      if( separator == null )
        throw new ArgumentNullException( "separator" );

      if( separator.Length == 0 )
        throw new ArgumentException( "separator" );

      if( encoding == null )
        encoding =
#if !SILVERLIGHT && !WINRT && !WP
        Encoding.Default;
#else
        Encoding.UTF8;
#endif

      m_bIsLoaded = true;
      StreamReader streamToRead = new StreamReader( stream, encoding );
      m_bLoading = true;

      if( m_ActiveSheet != null )
      {
          bool isValid = IsValidDocument(stream, encoding,separator);

        ( ( WorksheetImpl )m_ActiveSheet ).Parse( streamToRead, separator, row, column,isValid );

        if( fileName != null && fileName.Length > 0 )
          m_ActiveSheet.Name = Path.GetFileNameWithoutExtension( fileName );
      }

      m_bLoading = false;
    }
    /// <summary>
    /// Create Workbook from stream.
    /// </summary>
    /// <param name="application">Application object for the workbook.</param>
    /// <param name="parent">Parent object for the workbook.</param>
    /// <param name="stream">Stream that contains workbook's data.</param>
    /// <param name="version">Excel version.</param>
    /// <exception cref="ArgumentNullException">
    /// When specified stream is NULL.
    /// </exception>
    public WorkbookImpl( IApplication application, object parent, Stream stream, ExcelVersion version )
      : this( application, parent, stream, ExcelParseOptions.Default, version )
    {
    }
    /// <summary>
    /// Create Workbook from stream.
    /// </summary>
    /// <param name="application">Application object for the workbook.</param>
    /// <param name="parent">Parent object for the workbook.</param>
    /// <param name="stream">Stream that contains workbook's data.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="version">Excel version.</param>
    /// <exception cref="ArgumentNullException">
    /// When specified stream is NULL.
    /// </exception>
    public WorkbookImpl( IApplication application, object parent, Stream stream,
      ExcelParseOptions options, ExcelVersion version )
      : base( application, parent )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      m_bOptimization = application.OptimizeFonts;
      m_options = options;
      InitializeCollections();
      Version = version;

      ParseStream( stream, null, version, options );
      //if( version == ExcelVersion.Excel97to2003 )
      //{
      //  m_workbookFile = AppImplementation.CreateCompoundFile( stream );
      //  ICompoundStorage storage = m_workbookFile.RootStorage;
      //  ParseStgStream( storage, options, null );
      //  //ParseExcel97Stream( stream, options );
      //}
      //else if( version == ExcelVersion.Excel2007 || version == ExcelVersion.Excel2010 )
      //{
      //  ParseExcel2007Stream( stream, null );
      //}
      //else
      //{
      //  throw new ArgumentOutOfRangeException( "version" );
      //}
    }
    /// <summary>
    /// Parses new workbook from xml stream.
    /// </summary>
    /// <param name="application">Current application</param>
    /// <param name="parent">Parent object.</param>
    /// <param name="reader">Xml reader.</param>
    /// <param name="openType">Xml open type.</param>
    public WorkbookImpl( IApplication application, object parent, XmlReader reader
      , ExcelXmlOpenType openType )
      : base( application, parent )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      InitializeCollections();

      // This can be only Excel97to2003, since MS Excel 2007 files cannot be saved
      // into single xml (at least we don't support such operation).
      // Update - MS Excel 2007 can copy data in such format into clipboard,
      // so this can be any version, in this case rely on users DefaultVersion selection.
      Version = application.DefaultVersion;// ExcelVersion.Excel97to2003;

      InsertDefaultFonts();
      InsertDefaultValues();

      m_bReadOnly = false;

      switch( openType )
      {
        case ExcelXmlOpenType.MSExcel:
          m_bLoading = true;
          MSXmlReader msReader = new MSXmlReader( Application, this );
          bool bOldValue = m_bThrowInFormula;
          m_bThrowInFormula = false;
          msReader.FillWorkbook( reader, this );
          m_bThrowInFormula = bOldValue;
          m_bLoading = false;
          break;

        default:
          throw new ArgumentOutOfRangeException( "cannot specified xml open type." );
      }
    }

    /// <summary>
    /// Initializes all internal collections.
    /// </summary>
    protected void InitializeCollections()
    {
#if !SILVERLIGHT && !WINRT && !WP
      CreateGraphics();
#endif

      // m_arrObjects must be created before worksheets collection.
      m_arrObjects = new WorkbookObjectsCollection( Application, this );
      m_worksheets = new WorksheetsCollection( Application, this );
      m_styles = new StylesCollection( Application, this );
      m_colors = new List<Color>( DEF_PALETTE );
      m_names = new WorkbookNamesCollection( Application, this );
      m_charts = new ChartsCollection( Application, this );
      m_SSTDictionary = new SSTDictionary( this );
      m_fonts = new FontsCollection( Application, this );
      m_externBooks = new ExternBookCollection( Application, this );
      m_addinFunctions = new AddInFunctionsCollection( Application, this );
      m_calcution = new CalculationOptionsImpl( Application, this );
      m_extFormats = new ExtendedFormatsCollection( Application, this );
      m_shapesData = new WorkbookShapeDataImpl( Application, this, /*new ShapesGetter()*/GetWorksheetShapes );
      m_customDocumentProperties = new CustomDocumentProperties( Application, this );
      m_builtInDocumentProperties = new BuiltInDocumentProperties( Application, this );
      m_contentTypeProperties = new MetaPropertiesImpl(Application, this);
      m_customXmlPartCollection = new CustomXmlPartCollection(Application, this);
      m_connections = new ExternalConnectionCollection(Application, this);
      m_builtInDocumentProperties[ ExcelBuiltInProperty.Author ].Text =
#if !SILVERLIGHT && !WINRT && !WP
        ( ExcelEngine.IsSecurityGranted ) ?
        Environment.UserName :
#endif
        string.Empty;

      m_arrNames = new List<NameRecord>();
      m_rawFormats = new FormatsCollection( Application, this );
      m_arrBound = new List<BoundSheetRecord>();
      m_arrReparse = new List<IReparse>();
      m_arrExtFormatRecords = new List<ExtendedFormatRecord>();
      m_arrXFExtRecords = new List<ExtendedXFRecord>();
      m_headerFooterPictures = new WorkbookShapeDataImpl( Application, this, /*new HeaderImageGetter()*/GetHeaderFooterShapes );

      m_sheetGroup = new WorksheetGroup( Application, this );
      WindowOne.SelectedTab = ushort.MaxValue;
    }
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Creates graphics object.
    /// </summary>
    private void CreateGraphics()
    {
        using (Image image = new Bitmap(1, 1))
        {
            m_graphics = Graphics.FromImage(image);
        }
    }
#endif
    /// <summary>
    /// Fills collection with default formats, extended format, and styles.
    /// </summary>
    internal void InsertDefaultValues()
    {
      m_rawFormats.InsertDefaultFormats();
      InsertDefaultExtFormats();
      InsertDefaultStyles();
    }
    /// <summary>
    /// Inserts all default extended formats into special list. Excel has 21 
    /// default formats for each workbook.
    /// </summary>
    protected void InsertDefaultExtFormats()
    {
      int iCount = m_extFormats.Count;
      ExtendedFormatRecord ext;
      ExtendedXFRecord xfExt = GetDefaultXFExt();

      if( iCount <= 0 )
      {
        ext = GetDefaultXF( 0 );
        m_extFormats.ForceAdd( new ExtendedFormatImpl( Application, this, ext,xfExt ) );
      }

      if( iCount <= 1 )
      {
        ext = GetDefaultXF( 1 );
        m_extFormats.ForceAdd(new ExtendedFormatImpl(Application, this, ext, xfExt));     
      }

      if (iCount <= 2)
      {
          ext = GetDefaultXF(2);
          m_extFormats.ForceAdd(new ExtendedFormatImpl(Application, this, ext, xfExt));
      }

      if( iCount <= 3 )
      {
        ext = GetDefaultXF( 3 );
        m_extFormats.ForceAdd(new ExtendedFormatImpl(Application, this, ext, xfExt));
      }

      if (iCount <= 4)
      {
          ext = GetDefaultXF(4);
          m_extFormats.ForceAdd(new ExtendedFormatImpl(Application, this, ext, xfExt));
      }

      if( iCount <= 5 )
      {
        ext = GetDefaultXF( 5 );
        m_extFormats.ForceAdd(new ExtendedFormatImpl(Application, this, ext, xfExt));
      }

      if (iCount <= 6)
      {
          ext = GetDefaultXF(6);
          m_extFormats.ForceAdd(new ExtendedFormatImpl(Application, this, ext, xfExt));
      }

      if (iCount <= 7)
      {
          ext = GetDefaultXF(7);
          m_extFormats.ForceAdd(new ExtendedFormatImpl(Application, this, ext, xfExt));
      }

      if (iCount <= 8)
      {
          ext = GetDefaultXF(8);
          m_extFormats.ForceAdd(new ExtendedFormatImpl(Application, this, ext, xfExt));
      }

      if (iCount <= 9)
      {
          ext = GetDefaultXF(9);
          m_extFormats.ForceAdd(new ExtendedFormatImpl(Application, this, ext, xfExt));
      }

      if (iCount <= 10)
      {
          ext = GetDefaultXF(10);
          m_extFormats.ForceAdd(new ExtendedFormatImpl(Application, this, ext, xfExt));
      }

      if (iCount <= 11)
      {
          ext = GetDefaultXF(11);
          m_extFormats.ForceAdd(new ExtendedFormatImpl(Application, this, ext, xfExt));
      }

      if (iCount <= 12)
      {
          ext = GetDefaultXF(12);
          m_extFormats.ForceAdd(new ExtendedFormatImpl(Application, this, ext, xfExt));
      }

      if (iCount <= 13)
      {
          ext = GetDefaultXF(13);
          m_extFormats.ForceAdd(new ExtendedFormatImpl(Application, this, ext, xfExt));
      }

      if (iCount <= 14)
      {
          ext = GetDefaultXF(14);
          m_extFormats.ForceAdd(new ExtendedFormatImpl(Application, this, ext, xfExt));
      }

      if( iCount <= 15 )
      {
        ext = GetDefaultXF( 15 );
        m_extFormats.ForceAdd(new ExtendedFormatImpl(Application, this, ext, xfExt));
      }

      if( iCount <= 16 )
      {
        ext = GetDefaultXF( 16 );
        m_extFormats.ForceAdd(new ExtendedFormatImpl(Application, this, ext, xfExt));
      }

      if( iCount <= 17 )
      {
        ext = GetDefaultXF( 17 );
        m_extFormats.ForceAdd(new ExtendedFormatImpl(Application, this, ext, xfExt));
      }

      if( iCount <= 18 )
      {
        ext = GetDefaultXF( 18 );
        m_extFormats.ForceAdd(new ExtendedFormatImpl(Application, this, ext, xfExt));
      }

      if( iCount <= 19 )
      {
        ext = GetDefaultXF( 19 );
        m_extFormats.ForceAdd(new ExtendedFormatImpl(Application, this, ext, xfExt));
      }

      if( iCount <= 20 )
      {
        ext = GetDefaultXF( 20 );
        m_extFormats.ForceAdd(new ExtendedFormatImpl(Application, this, ext, xfExt));
      }
    }
    /// <summary>
    /// Inserts all default styles into special list.
    /// </summary>
    protected void InsertDefaultStyles()
    {
      InsertDefaultStyles( null );
    }
    /// <summary>
    /// Inserts all default styles into special list.
    /// </summary>
    /// <param name="arrStyles">Styles that were read from file.</param>
    protected void InsertDefaultStyles( List<StyleRecord> arrStyles )
    {
      bool bLoaded = ( arrStyles != null );
      StyleImpl stout;

      StyleRecord style = ( StyleRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Style );
      style.ExtendedFormatIndex = 0;
      style.BuildInOrNameLen = 0;
      style = FindStyle( arrStyles, style );
      StyleImpl newStyle = AppImplementation.CreateStyle( this, style );
      if( m_styles.ContainsName( newStyle.Name ) == false ) m_styles.Add( newStyle, true );

      style = ( StyleRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Style );
      style.ExtendedFormatIndex = 16;
      style.BuildInOrNameLen = 3;
      style = FindStyle( arrStyles, style );
      stout = AppImplementation.CreateStyle( this, style );
      AddDefaultStyle( stout );
      
      style = ( StyleRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Style );
      style.ExtendedFormatIndex = 17;
      style.BuildInOrNameLen = 6;
      style = FindStyle( arrStyles, style );
      stout = AppImplementation.CreateStyle( this, style );
      AddDefaultStyle( stout );
      
      style = ( StyleRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Style );
      style.ExtendedFormatIndex = 18;
      style.BuildInOrNameLen = 4;
      style = FindStyle( arrStyles, style );
      stout = AppImplementation.CreateStyle( this, style );
      AddDefaultStyle( stout );
      
      style = ( StyleRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Style );
      style.ExtendedFormatIndex = 19;
      style.BuildInOrNameLen = 7;
      style = FindStyle( arrStyles, style );
      stout = AppImplementation.CreateStyle( this, style );
      AddDefaultStyle( stout );
      
      style = ( StyleRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Style );
      style = FindStyle( arrStyles, style );
      stout = AppImplementation.CreateStyle( this, style );
      AddDefaultStyle( stout );
      
      style = ( StyleRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Style );
      style.ExtendedFormatIndex = 20;
      style.BuildInOrNameLen = 5;
      style = FindStyle( arrStyles, style );
      stout = AppImplementation.CreateStyle( this, style );
      AddDefaultStyle( stout );

      //m_styles.BuildStylesHash();
      ( Styles[ "Normal" ].Font as FontWrapper ).AfterChangeEvent += new EventHandler(WorkbookImpl_AfterChangeEvent);
    }
    /// <summary>
    /// Registers default style inside styles collection.
    /// </summary>
    /// <param name="stout">Style to register.</param>
    private void AddDefaultStyle( StyleImpl stout )
    {
      if( stout == null )
        throw new ArgumentNullException( "stout" );

      int xfIndex = stout.XFormatIndex;

      if( InnerExtFormats[ xfIndex ].HasParent )
      {
        ExtendedFormatImpl newFormat = CreateExtFormatWithoutRegister( InnerExtFormats[ 0 ] );
        newFormat = RegisterExtFormat( newFormat, true );
        stout.SetFormatIndex( newFormat.Index );
      }

      if( m_styles.ContainsName( stout.Name ) == false )
        m_styles.Add( stout );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    [ CLSCompliant( false ) ]
    protected ExtendedFormatRecord GetDefaultXF( int index )
    {
      ExtendedFormatRecord result = null;
      switch( index )
      {
        case 0:
          result = ( ExtendedFormatRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ExtendedFormat );
          result.IsLocked = true;
          result.ParentIndex = ( ushort )MaxXFCount;//ExtendedFormatRecord.DEF_XF_MAX_INDEX;
          result.XFType = ExtendedFormatRecord.TXFType.XF_CELL;
          result.FillBackground = ExtendedFormatRecord.DEF_DEFAULT_COLOR_INDEX;
          result.FillForeground = ExtendedFormatRecord.DEF_DEFAULT_PATTERN_COLOR_INDEX;
          break;

        case 1:
        case 2:
          result = ( ExtendedFormatRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ExtendedFormat );
          result.FontIndex = 1;
          result.IsLocked = true;
          result.XFType = ExtendedFormatRecord.TXFType.XF_CELL;
          result.ParentIndex = ( ushort )MaxXFCount;//ExtendedFormatRecord.DEF_XF_MAX_INDEX;
          result.VAlignmentType = ExcelVAlign.VAlignBottom;
          result.IsNotParentFormat = true;
          result.IsNotParentAlignment = true;
          result.IsNotParentBorder = true;
          result.IsNotParentPattern = true;
          result.IsNotParentCellOptions = true;
          break;

        case 3:
        case 4:
          result = ( ExtendedFormatRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ExtendedFormat );
          if(this.InnerFonts.Count > 2)
              result.FontIndex = 2;
          result.IsLocked = true;
          result.XFType = ExtendedFormatRecord.TXFType.XF_CELL;
          result.ParentIndex = ( ushort )MaxXFCount;//ExtendedFormatRecord.DEF_XF_MAX_INDEX;
          result.VAlignmentType = ExcelVAlign.VAlignBottom;
          result.IsNotParentFormat = true;
          result.IsNotParentAlignment = true;
          result.IsNotParentBorder = true;
          result.IsNotParentPattern = true;
          result.IsNotParentCellOptions = true;
          break;

        case 5:
        case 6:
        case 7:
        case 8:
        case 9:
        case 10:
        case 11:
        case 12:
        case 13:
        case 14:
          result = ( ExtendedFormatRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ExtendedFormat );
          result.IsLocked = true;
          result.XFType = ExtendedFormatRecord.TXFType.XF_CELL;
          result.ParentIndex = ( ushort )MaxXFCount;//ExtendedFormatRecord.DEF_XF_MAX_INDEX;
          result.VAlignmentType = ExcelVAlign.VAlignBottom;
          result.IsNotParentFormat = true;
          result.IsNotParentAlignment = true;
          result.IsNotParentBorder = true;
          result.IsNotParentPattern = true;
          result.IsNotParentCellOptions = true;
          break;

        case 15:
          result = ( ExtendedFormatRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ExtendedFormat );
          result.IsLocked = true;
          result.HAlignmentType = ExcelHAlign.HAlignGeneral;
          result.VAlignmentType = ExcelVAlign.VAlignBottom;
          result.FillBackground = ExtendedFormatRecord.DEF_DEFAULT_COLOR_INDEX;
          result.FillForeground = ExtendedFormatRecord.DEF_DEFAULT_PATTERN_COLOR_INDEX;
          break;

        case 16:
          result = ( ExtendedFormatRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ExtendedFormat );
          result.IsNotParentFont = true;
          result.IsNotParentAlignment = true;
          result.IsNotParentBorder = true;
          result.IsNotParentPattern = true;
          result.IsNotParentCellOptions = true;
          result.FontIndex = 1;
          result.FormatIndex = 43;
          result.XFType = ExtendedFormatRecord.TXFType.XF_CELL;
          result.ParentIndex = ( ushort )MaxXFCount;//ExtendedFormatRecord.DEF_XF_MAX_INDEX;
          break;

        case 17:
          result = ( ExtendedFormatRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ExtendedFormat );
          result.IsNotParentFont = true;
          result.IsNotParentAlignment = true;
          result.IsNotParentBorder = true;
          result.IsNotParentPattern = true;
          result.IsNotParentCellOptions = true;
          result.FontIndex = 1;
          result.FormatIndex = 41;
          result.ParentIndex = ( ushort )MaxXFCount;//ExtendedFormatRecord.DEF_XF_MAX_INDEX;
          result.XFType = ExtendedFormatRecord.TXFType.XF_CELL;
          break;

        case 18:
          result = ( ExtendedFormatRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ExtendedFormat );
          result.IsLocked = true;
          result.ParentIndex = ( ushort )MaxXFCount;//ExtendedFormatRecord.DEF_XF_MAX_INDEX;
          result.XFType = ExtendedFormatRecord.TXFType.XF_CELL;
          result.IsNotParentFont = true;
          result.IsNotParentAlignment = true;
          result.IsNotParentBorder = true;
          result.IsNotParentPattern = true;
          result.IsNotParentCellOptions = true;
          result.FontIndex = 1;
          result.FormatIndex = 44;
          break;

        case 19:
          result = ( ExtendedFormatRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ExtendedFormat );
          result.IsLocked = true;
          result.ParentIndex = ( ushort )MaxXFCount;//ExtendedFormatRecord.DEF_XF_MAX_INDEX;
          result.XFType = ExtendedFormatRecord.TXFType.XF_CELL;
          result.IsNotParentFont = true;
          result.IsNotParentAlignment = true;
          result.IsNotParentBorder = true;
          result.IsNotParentPattern = true;
          result.IsNotParentCellOptions = true;
          result.FontIndex = 1;
          result.FormatIndex = 42;
          break;

        case 20:
          result = ( ExtendedFormatRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ExtendedFormat );
          result.IsLocked = true;
          result.ParentIndex = ( ushort )MaxXFCount;//ExtendedFormatRecord.DEF_XF_MAX_INDEX;
          result.XFType = ExtendedFormatRecord.TXFType.XF_CELL;
          result.IsNotParentFont = true;
          result.IsNotParentAlignment = true;
          result.IsNotParentBorder = true;
          result.IsNotParentPattern = true;
          result.IsNotParentCellOptions = true;
          result.FontIndex = 1;
          result.FormatIndex = 9;
          break;
      }

      return result;
    }
    ///<summary></summary>
    /// <returns></returns>
    [CLSCompliant(false)]
    protected ExtendedXFRecord GetDefaultXFExt()
    {
        ExtendedXFRecord result = null;
        result = (ExtendedXFRecord)BiffRecordFactory.GetRecord(TBIFFRecord.ExtendedXFRecord);
        return result;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="arrStyles"></param>
    /// <param name="style"></param>
    /// <returns></returns>
    private StyleRecord FindStyle( List<StyleRecord> arrStyles, StyleRecord style )
    {
      if( arrStyles == null )
        return style;

      for( int i = 0, len = arrStyles.Count; i < len; i++ )
      {
        StyleRecord record = arrStyles[ i ];

        if( CompareDefaultStyleRecords( record, style ) )
        {
          return record;
        }
      }

      return style;
    }
    /// <summary>
    /// Compares two style records.
    /// </summary>
    /// <param name="style1">First style record to compare.</param>
    /// <param name="style2">Second style record to compare.</param>
    /// <returns>True if records describe same default style.</returns>
    private bool CompareDefaultStyleRecords( StyleRecord style1, StyleRecord style2 )
    {
      if( style1.IsBuildInStyle && style2.IsBuildInStyle )
      {
        return style1.BuildInOrNameLen == style2.BuildInOrNameLen;
      }

      return false;
    }
    /// <summary>
    /// Inserts default fonts into special list.
    /// </summary>
    internal void InsertDefaultFonts()
    {
      m_fonts.InsertDefaultFonts();
    }
    /// <summary>
    /// Disposes internal collections.
    /// </summary>
    internal void DisposeAll()
    {
        if (!this.m_IsDisposed)
        {
            //TODO: Dispose internal data
            //m_arrObjects.DisposeInternalData();
            m_arrObjects.Clear();
            m_arrObjects = null;
            m_externBooks.Dispose();
            ClearAll();
            if (m_drawGroup != null)
            {
                m_drawGroup.m_data = null;
                m_drawGroup.Dispose();
            }
            if (m_shapesData != null)
            {

                m_shapesData.Dispose();
            }
            if (m_SSTDictionary != null)
            {
                m_SSTDictionary.Dispose();
                m_SSTDictionary = null;
            }

#if AllowUnsafeCode
            if (m_ptrHeapHandle != IntPtr.Zero)
            {
                //Console.WriteLine( "HeapDestroy {0}", m_ptrHeapHandle );
                Heap.HeapDestroy(m_ptrHeapHandle);
                m_ptrHeapHandle = IntPtr.Zero;
            }
#endif

            if (m_workbookFile != null)
                m_workbookFile.Dispose();

#if !SILVERLIGHT && !WINRT && !WP
        if (m_graphics != null)
            m_graphics.Dispose();
#endif
            this.m_IsDisposed = true;
    }
    }
    /// <summary>
    /// Clears all internal collections.
    /// </summary>
    protected void ClearAll()
    {
        if (!m_bIsDisposed)
        {
        //m_styles.Clear();
            
            //TODO: Need to dispose m_styles
            //m_styles.Dispose();
            //m_styles.Clear();
        if (m_ActiveSheet != null)
        {
            WorksheetImpl worksheet = m_ActiveSheet as WorksheetImpl;
            if (worksheet != null)
            {
                (m_ActiveSheet as WorksheetImpl).ClearAllData();
                m_ActiveSheet = null;
            }

        }
        foreach (IWorksheet sheet in m_worksheets)
        {
            (sheet as WorksheetImpl).ClearAllData();
        }

            if (m_worksheets != null)
            {
      m_worksheets.Clear();
                //m_worksheets = null;
            }
            if (m_arrBound != null)
            {
      m_arrBound.Clear();
                m_arrBound = null;
            }
            if (m_arrNames != null)
            {
      m_arrNames.Clear();
                m_arrNames = null;
            }
            if (m_arrReparse != null)
            {
      m_arrReparse.Clear();
                m_arrReparse = null;
            }
      //m_arrSupBooks.Clear();
      //m_arrExternRef.Clear();
            if (m_colors != null)
            {
      m_colors.Clear();
                m_colors = null;
            }
      if (m_extFormats != null)
      {
          m_extFormats.Dispose();
          m_extFormats.Clear();
                m_extFormats = null;
            }
            if (m_styles != null)
            {
                m_styles.Clear();
                m_styles = null;
            }
            if (m_fonts != null)
      {
          m_fonts.Dispose();
          m_fonts.Clear();
                m_fonts = null;
      }
            if (m_rawFormats != null)
            {
      m_rawFormats.Clear();
                m_rawFormats = null;
            }
            if (m_shapesData != null)
            {
      m_shapesData.Clear();
                m_shapesData = null;
            }
            if (m_SSTDictionary != null)
            {
                m_SSTDictionary.Clear();
            }
            if (m_sstStream != null)
            {
                m_sstStream.Dispose();
                m_sstStream = null;
            }

            if (m_arrExtFormatRecords != null)
            {
                m_arrExtFormatRecords.Clear();
                m_arrExtFormatRecords = null;
            }
            if (m_arrXFExtRecords != null)
            {
                m_arrXFExtRecords.Clear();
                m_arrXFExtRecords = null;
            }
            if (m_styles != null)
            {
                m_styles.Clear();
                m_styles = null;
            }
            if (m_fonts != null)
            {
                m_fonts.Clear();
                m_fonts = null;
            }
            if (m_externBooks != null)
            {
                m_externBooks.Clear();
                m_externBooks = null;
            }

            if (m_addinFunctions != null)
            {
                m_addinFunctions.Clear();
                m_addinFunctions = null;
            }

            if (m_builtInDocumentProperties != null)
            {
                m_builtInDocumentProperties.Clear();
                m_builtInDocumentProperties = null;
            }
            if (m_customDocumentProperties != null)
            {
                m_customDocumentProperties.Clear();
                m_customDocumentProperties = null;
            }
            if (m_majorFonts != null)
            {
                m_majorFonts.Clear();
                m_majorFonts = null;
            }
            if (m_minorFonts != null)
            {
                m_minorFonts.Clear();
                m_minorFonts = null;
            }
      if (m_fileDataHolder != null)
      {
          m_fileDataHolder.Dispose();
          m_fileDataHolder = null;
      }
            if (m_arrObjects != null)
            {
                m_arrObjects.Clear();
                m_arrObjects = null;
            }
      m_controlsStream = null;
      m_sstStream = null;
            if (m_names != null)
            {
      foreach (NameImpl name in m_names)
      {
          name.ClearAll();
      }
      m_names.Clear();
                m_names = null;
            }
        if (m_connections != null)
      {
          m_connections.Dispose();
          m_connections.Clear();
          m_connections = null;
      }
      if (m_deletedConnections != null)
      {
          m_deletedConnections.Clear();
          m_deletedConnections.Dispose();
          m_deletedConnections = null;
      }
#if !SILVERLIGHT && !WINRT && !WP
      if (m_graphics != null)
            {
          m_graphics.Dispose();
                m_graphics = null;

            }
#endif

            if (m_calcution != null)
            {
                m_calcution.Dispose();
                m_calcution = null;
    }

            if (m_bookExt != null)
            {
                m_bookExt.ClearData();
                m_bookExt = null;
            }


            if (m_charts != null)
            {
                m_charts.Clear();
                m_charts = null;
            }
            if (m_childElements != null)
            {
                m_childElements.Clear();
                m_childElements = null;
            }
            if (m_compatibility != null)
            {
                m_compatibility.ClearData();
                m_compatibility = null;
            }

            if (m_contentTypeProperties != null)
            {
                m_contentTypeProperties.Clear();
                m_contentTypeProperties = null;
            }
            if (m_customXmlPartCollection != null)
            
            {
                foreach (CustomXmlPart xmlPar in m_customXmlPartCollection)
                    xmlPar.Dispose();
                m_customXmlPartCollection.Clear();
                m_CustomTableStylesStream = null;
            }


            if (m_drawGroup != null)
            {
                m_drawGroup.Dispose();
                m_drawGroup = null;
            }
            if (m_externSheet != null)
            {
                m_externSheet.Dispose();
                m_externSheet = null;
            }
            if (m_formulaUtil != null)
            {
                m_formulaUtil.Dispose();
                m_formulaUtil = null;
            }
            if (m_headerFooterPictures != null)
            {
                m_headerFooterPictures.Clear();
                m_headerFooterPictures = null;
            }
            if (m_modifiedFormatRecord != null)
            {
                m_modifiedFormatRecord.Clear();
                m_modifiedFormatRecord = null;
            }
            if (m_password != null)
            {
                m_password.ClearData();
                m_password = null;
            }
            if (m_passwordRev4 != null)
            {
                m_passwordRev4.ClearData();
                m_passwordRev4 = null;
            } GC.SuppressFinalize(this);
            m_bIsDisposed = true;
        }
    
    }
    /// <summary>
    /// Clears the extended formats.
    /// </summary>
    internal void ClearExtendedFormats()
    {
        if (m_extFormats != null)
        {
            foreach (ExtendedFormatImpl format in m_extFormats)
            {
                format.Clear();
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="storage">Storage object to extract pivot cache from.</param>
    /// <param name="decryptor">Object used to decrypt encrypted records.</param>
    private void CreatePivotCache( ICompoundStorage storage, IDecryptor decryptor )
    {
      if( storage == null )
        throw new ArgumentNullException( "storage" );

      m_pivotCaches = new PivotCacheCollection( Application, this, storage, decryptor );
    }
    /// <summary>
    /// Parse StgStream.
    /// </summary>
    /// <param name="storage">Storage object to extract data from.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="password">Password to decrypt if necessary.</param>
    private void ParseStgStream( ICompoundStorage storage, ExcelParseOptions options, string password )
    {
      // Workbook can be stored by two differ names: "Workbook" and "Book".
      string strStreamName = FindStreamCaseInsensitive( storage, DEF_STREAM_NAME1 );

      if( strStreamName == null )
        throw new ApplicationException( "File does not contain workbook stream" );

      ReadControlsData( storage );
      ReadDocumentProperties( storage );

      // Check for macros availability in source document.
      bool bMacros = storage.ContainsStorage( DEF_VBA_MACROS );

      m_bHasSummaryInformation = storage.ContainsStream( DEF_SUMMARY_INFO );
      m_bHasDocumentSummaryInformation = storage.ContainsStream( DEF_DOCUMENT_SUMMARY_INFO );
      IDecryptor decryptor = null;
      string[] StorageNames = storage.Storages;

      if( storage.ContainsStorage( PivotCacheCollection.DEF_PIVOT_CACHE_STORAGE ) )
      {
        CreatePivotCache( storage, decryptor );
      }
#if !SILVERLIGHT && !WINRT && !WP
      this.HasOleObjects = CreateOleCache(storage);
#endif

      // Initialize workbook.
      using( CompoundStream workbookStream = storage.OpenStream( strStreamName ) )
      {
        using( Parser.BiffReader reader = new Parser.BiffReader( workbookStream ) )
        {
          decryptor = Parse( reader, options, password );
        }
      }
      if (storage.ContainsStorage("MsoDataStore"))
      {
          ICompoundStorage msoDataStore=storage.OpenStorage("MsoDataStore");

          MsoDataStore custom = new MsoDataStore(msoDataStore, this);
          custom.ParseMsoDataStore();
          //storage.DeleteStorage("MsoDataStore");
      }

      //ExtractControlProperties( storage );
      // If after parse flag HasMacros set to true and storage contains
      // macros substorages, then we allow Macros to attach.
      m_bHasMacros &= bMacros;
    }
#if !SILVERLIGHT && !WINRT && !WP
    private bool CreateOleCache(ICompoundStorage storage)
    {
        string[] StorageNames = storage.Storages;
        string[] OleConstants = { "_VBA_PROJECT_CUR", "_SX_DB_CUR", "MsoDataStore" };
        bool containsOleObject = false;
        List<string> OleStorgeName = new List<string>();

        foreach (string storageName in StorageNames)
        {
            if (Array.IndexOf(OleConstants, storageName) == -1)
            {
                OleStorgeName.Add(storageName);
                containsOleObject = true;
            }
        }
        if (containsOleObject)
        {
            this.m_OleStorageCollection = new OleStorageCollection();

            foreach (string storageName in OleStorgeName)
            {
                using (ICompoundStorage CompoundStorage = storage.OpenStorage(storageName))
                {
                    m_OleStorageCollection.ParseStorage(CompoundStorage);
                }
            }

            //using (CompoundStream compObjectStream = storage.OpenStream(storage.Streams[0]))
            //{
            //    m_OleStorageCollection.Add(compObjectStream.Name, compObjectStream);
            //}
        }
        return containsOleObject;
    }
#endif

    private void ReadControlsData( ICompoundStorage storage )
    {
      const string ControlsStreamName = "Ctls";

      if( storage.ContainsStream( ControlsStreamName ) )
      {
        CompoundStream stream = storage.OpenStream( ControlsStreamName );

        m_controlsStream = new MemoryStream( ( int )stream.Length );
        UtilityMethods.CopyStreamTo( stream, m_controlsStream );

        stream.Position = 0;
#if ( WINRT || WP)
          stream.Dispose();
#else
        stream.Close();
#endif
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="storage"></param>
    private void ExtractControlProperties( ICompoundStorage storage )
    {
      //if( storage.ContainsStream( ControlPropertiesList.StreamName ) )
      //{
      //  using( CompoundStream stream = storage.OpenStream( ControlPropertiesList.StreamName ) )
      //  {
      //    for( int i = 0, len = m_worksheets.Count; i < len; i++ )
      //    {
      //      WorksheetImpl sheet = ( WorksheetImpl )m_worksheets[ i ];
      //      TextBoxCollection textBoxes = sheet.InnerTextBoxes;

      //      if( textBoxes != null )
      //      {
      //        for( int j = 0, lenJ = textBoxes.Count; j < lenJ; j++ )
      //        {
      //          TextBoxShapeImpl textBox = ( TextBoxShapeImpl )textBoxes[ j ];
      //          textBox.Properties.Parse( stream );
      //        }
      //      }
      //    }
      //  }
      //}
    }
    /// <summary>
    /// Searches for the stream name, case-insensitive.
    /// </summary>
    /// <param name="storage">Storage object to search inside of.</param>
    /// <param name="streamName">Name of the stream to locate.</param>
    /// <returns>Case-sensitive stream name if stream was found; null otherwise.</returns>
    private static string FindStreamCaseInsensitive( ICompoundStorage storage, string streamName )
    {
      if( streamName == null )
        throw new ArgumentNullException( "strStreamName" );

      if( streamName.Length == 0 )
        throw new ArgumentException( "strStreamName - string cannot be empty." );

      string[] arrStreams = storage.Streams;
      string strResult = null;

      for( int i = 0, len = arrStreams.Length; i < len; i++ )
      {
        string strStream = arrStreams[ i ];

        if( string.Compare( strStream, streamName, StringComparison.CurrentCultureIgnoreCase ) == 0 )
        {
          strResult = strStream;
          break;
        }
      }

      return strResult;
    }
    /// <summary>
    /// This method parses stream that holds data in Excel 2007 format (Open XML).
    /// </summary>
    /// <param name="stream">Stream to parse.</param>
    /// <param name="password">Password to use during for decryption.</param>
    private void ParseExcel2007Stream( Stream stream, string password, bool parseOnDemand )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      m_rawFormats.InsertDefaultFormats();
      m_fileDataHolder = new FileDataHolder( this, stream, password );
      m_fileDataHolder.ParseDocument( ref m_themeColors, parseOnDemand );
    }
#if !(WINRT )
    /// <summary>
    /// Parses specified file.
    /// </summary>
    /// <param name="fileName">File to parse.</param>
    /// <param name="password">Password to use for decryption (null if file is not encrypted).</param>
    /// <param name="version">Excel version.</param>
    /// <param name="options">Parsing options.</param>
    private void ParseFile( string fileName, string password, ExcelVersion version, ExcelParseOptions options,bool isReadOnly )
    {
        if (isReadOnly)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                ParseStream(stream, password, version, options);
            }
            this.ReadOnly = true;
        }
        else
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ParseStream(stream, password, version, options);
            }
        }
    }
#endif
    /// <summary>
    /// Parses specified stream.
    /// </summary>
    /// <param name="stream">Stream to parse.</param>
    /// <param name="password">Password to use for decryption (null if file is not encrypted).</param>
    /// <param name="version">Excel version.</param>
    /// <param name="options">Parsing options.</param>
    private void ParseStream( Stream stream, string password, ExcelVersion version, ExcelParseOptions options )
    {
      m_bIsLoaded = true;

      if( version == ExcelVersion.Excel97to2003 )
      {
        m_workbookFile = AppImplementation.CreateCompoundFile( stream );
        ICompoundStorage storage = m_workbookFile.RootStorage;
        ParseStgStream( storage, options, password );
      }
      else if( version !=ExcelVersion.Excel97to2003 )
      {
        ParseExcel2007Stream( stream, password, (options == ExcelParseOptions.ParseWorksheetsOnDemand) );
        this.Activate();
      }
      else
      {
        throw new ArgumentOutOfRangeException( "version" );
      }
    }
    /// <summary>
    /// Class finalizer.
    /// </summary>
    ~WorkbookImpl()
    {
#if !(WINRT )
        //TODO: ( WINRT ): need to implement.
      Close();
#endif
    }
    #endregion

    #region Recover from stream
    /// <summary>
    /// Reads workbook from the specified reader.
    /// </summary>
    /// <param name="reader">BiffReader that contains workbook.</param>
    private void  Parse( BiffReader reader )
    {
      Parse( reader, ExcelParseOptions.Default, null );
    }
    /// <summary>
    /// Reads workbook from the specified reader.
    /// </summary>
    /// <param name="reader">BiffReader that contains workbook.</param>
    /// <param name="options">Parsing options.</param>
    /// <param name="password">
    /// Password - used for decryption.Otherwise this argument is ignored.
    /// </param>
    /// <returns>Decryptor used for parsing, if it was created; null otherwise.</returns>
    private IDecryptor  Parse( BiffReader reader, ExcelParseOptions options, string password )
    {
      m_bLoading = true;
      bool bContinueExtraction = true;

      int newValue = 1;
      List<StyleRecord> arrStyles = new List<StyleRecord>();
      IDecryptor decryptor = null;

      // Clear internal storage of records.
      m_records = new List<BiffRecordRaw>( 128 );
      m_arrBound.Clear();
      //m_arrSupBooks.Clear();
      //m_arrExternRef.Clear();
      m_externBooks.Clear();
      m_headerFooterPictures.Clear();
      m_shapesData.Clear();
      
      // Erase default styles, fonts, extFormats.
      //m_arrFontWrappers.Clear();
      m_fonts.Clear();
      m_extFormats.Clear();

      // If workbook extracted from stream, we must first find startup record.
      reader.SeekOnBOFRecord();
      bool bIgnoreStyle = false;//( ( options & ExcelParseOptions.SkipStyles ) != 0 );
      bool bStylesReady = false;
      //bool bIncrease = true;
      long lCurPos;
      Dictionary<int, int> hashNewXFormatIndexes = bIgnoreStyle ? new Dictionary<int, int>() : null;
      List<BiffRecordRaw> arrPivotRecords = new List<BiffRecordRaw>();

      uint[] crcCache = InitCRC();
      uint m_tempCRCValue = 0;
      uint m_parsedCRCValue = 0;

      // First we must extract main / first workbook BOF / EOF part
      // which hold settings, style, formatting, etc. Data
      // needed by Excel for good data showing.
      while( !reader.IsEOF && bContinueExtraction )
      {
        lCurPos = reader.BaseStream.Position;
        BiffRecordRaw raw = reader.GetRecord( decryptor );
        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, raw.TypeCode, "Current record" );

        // Store records into internal storage.
        m_records.Add( raw );

        if( Array.IndexOf( DEF_PIVOTRECORDS, raw.TypeCode ) != -1 )
        {
          arrPivotRecords.Add( raw );
        }

        // On first EOF, we must start parsing of WorkSheets.
        switch( raw.TypeCode )
        {
          case TBIFFRecord.BOF:
            if( ( ( BOFRecord )raw ).Type != BOFRecord.TType.TYPE_WORKBOOK )
              throw new WrongBiffStreamPartException();
            break;

          case TBIFFRecord.ExtSST:
            if( ( ( ExtSSTRecord )raw ).IsEnd )
              goto case TBIFFRecord.EOF;
            break;

          case TBIFFRecord.EOF:
            //reader.GetRecord();

            //InsertDefaultValues();
            if( !bStylesReady )
            {
              PrepareStyles( bIgnoreStyle, arrStyles, hashNewXFormatIndexes );
            }

#if MEASURE_PERFORMANCE
            DateTime start = DateTime.Now;
#endif
            ExtractWorksheetsFromStream(reader, options, -1, -1, hashNewXFormatIndexes, decryptor);
            bContinueExtraction = false;
#if MEASURE_PERFORMANCE
            TimeSpan extractSheetsTime = DateTime.Now - start;
            Console.WriteLine( "Extract worksheets time: {0}", extractSheetsTime );
#endif
            continue;
          
          case TBIFFRecord.DateWindow1904:
            m_bDate1904 = (( DateWindow1904Record )raw).Is1904Windowing;
            break;

          case TBIFFRecord.Precision:
            PrecisionRecord precision = ( PrecisionRecord )raw;
            PrecisionAsDisplayed = precision.IsPrecision == 0;
            break;

          case TBIFFRecord.WindowOne:
            m_windowOne = ( WindowOneRecord ) raw;
            break;

          case TBIFFRecord.Font:
            if( !bIgnoreStyle )
            {
              m_fonts.ForceAdd( AppImplementation.CreateFont( this, ( FontRecord )raw ) );
            }
            break;

          case TBIFFRecord.Format:
            FormatRecord format = ( FormatRecord )raw;
            m_rawFormats.Add(RecheckFormatRecord(format, ref newValue));
            break;

          case TBIFFRecord.ExtendedFormat:
            ExtendedFormatRecord xf = ( ExtendedFormatRecord )raw;
            m_arrExtFormatRecords.Add(RecheckExtendedFormatRecord(xf));
            m_tempCRCValue = CalculateCRC(m_tempCRCValue, xf.Data, crcCache);
            break;

          case TBIFFRecord.ExtendedFormatCRC:
            ExtendedFormatCRC m_CRCRecords = (ExtendedFormatCRC)raw;
            m_parsedCRCValue = m_CRCRecords.CRCChecksum;
            break;

          case TBIFFRecord.ExtendedXFRecord:
            ExtendedXFRecord xfExt = (ExtendedXFRecord)raw;
            m_arrXFExtRecords.Add(xfExt);
            break;

          case TBIFFRecord.Style:
            StyleRecord style = ( StyleRecord ) raw;
            arrStyles.Add( style );
            break;

          case TBIFFRecord.Name:
            m_arrNames.Add( ( NameRecord )raw );
            break;

          case TBIFFRecord.SST:
            PrepareStyles( bIgnoreStyle, arrStyles, hashNewXFormatIndexes );
            bStylesReady = true;

            ParseSSTRecord( ( SSTRecord )raw, options );
            break;

          case TBIFFRecord.BoundSheet:
            m_arrBound.Add( ( BoundSheetRecord )raw );
            break;

          case TBIFFRecord.SupBook:
            reader.BaseStream.Position = lCurPos;
            m_externBooks.Parse( reader, decryptor );
            continue;

          case TBIFFRecord.ExternSheet:
            ExternSheetRecord externSheet = ( ExternSheetRecord )raw;

            if( m_externSheet == null || m_externSheet.RefCount == 0 )
            {
              m_externSheet = externSheet;
            }
            else
            {
              m_externSheet.PrependReferences( externSheet.RefList );
            }
            break;

          case TBIFFRecord.Continue:
            ContinueRecord cont = (ContinueRecord) raw;
            if(m_continue == null)
                m_continue = new List<ContinueRecord>();
            m_continue.Add(cont);
            break;

          case TBIFFRecord.UseSelFS:
            UseSelFSRecord selfs = ( UseSelFSRecord )raw;
            m_bSelFSUsed = selfs.Flags;
            break;

          case TBIFFRecord.WriteAccess:
            //WriteAccessRecord wrAcs = ( WriteAccessRecord )raw;
            //Author = wrAcs.UserName;
            break;

          case TBIFFRecord.Protect:
            m_bCellProtect = ( ( ProtectRecord ) raw ).IsProtected;
            break;

          case TBIFFRecord.WindowProtect:
            m_bWindowProtect = ( ( WindowProtectRecord ) raw ).IsProtected;
            break;

          case TBIFFRecord.MSODrawingGroup:
            MSODrawingGroupRecord drawRecord=  m_drawGroup = ( MSODrawingGroupRecord ) raw;
            m_shapesData.ParseDrawGroup( drawRecord);
            break;

          case TBIFFRecord.CodeName:
            m_strCodeName = ( ( CodeNameRecord )raw ).CodeName;
            break;

          case TBIFFRecord.HasBasic:
            m_bHasMacros = true;
            break;

          case TBIFFRecord.UnkMacrosDisable:
            m_bMacrosDisable = true;
            break;

          case TBIFFRecord.Palette:
            PaletteRecord.TColor[] arrColors = ( ( PaletteRecord )raw ).Colors;
            int iColorsLength = ( arrColors != null ) ? arrColors.Length : 0;

            if( iColorsLength > 0 )
            {
              int i = DEF_FIRST_USER_COLOR;

              for( int j = 0; j < iColorsLength; j++ )
              {
                // Use 'as' to increase performance.
                PaletteRecord.TColor clr = arrColors[ j ];
                SetPaletteColor( i, Color.FromArgb( clr.A, clr.R, clr.G, clr.B ) );
                i++;
              }
            }
            break;

          case TBIFFRecord.HeaderFooterImage:
            HeaderFooterImageRecord headerFooterPictures = ( HeaderFooterImageRecord )raw;
            m_headerFooterPictures.ParseDrawGroup( headerFooterPictures );
            break;

          case TBIFFRecord.FilePass:
            FilePassRecord filePass = ( FilePassRecord )raw;
            decryptor = CreateDecryptor( password, filePass );
            break;

          case TBIFFRecord.WriteProtection:
            m_bWriteProtection = true;
            break;

          case TBIFFRecord.FileSharing:
            m_fileSharing = ( FileSharingRecord )raw;
            break;

          case TBIFFRecord.Password:
            m_password = ( PasswordRecord )raw;
            break;

          case TBIFFRecord.BookExt:
            m_bookExt = raw;
            break;
         case TBIFFRecord.DConn:
            PreserveExternalConnectionDetails.Add(raw);
            break;

          case TBIFFRecord.Country:
            CountryRecord country = ( CountryRecord )raw;
            m_iCountry = country.CurrentCountry;
            m_rawFormats.AddDefaultFormats( m_iCountry );
            break;

          case TBIFFRecord.RecalcId:
            m_reCalcId = (RecalcIdRecord)raw;
            break;

          case TBIFFRecord.Compatibility:
            m_compatibility = (CompatibilityRecord)raw;
                break;
        }
      }

      if (m_parsedCRCValue == m_tempCRCValue)
          IsCRCSucceed = true;

      // After extraction of workbook records free storage.
      m_records = null;//.Clear();
      m_arrBound.Clear();

      (( ApplicationImpl )this.Application).SetActiveWorkbook( this );

      m_bLoading = false;
      //ParseNames();
      Reparse();
      ParseAutoFilters();
      ParsePivotRecords( arrPivotRecords );

      return decryptor;
    }
    /// <summary>
    /// Extracts pivot caches information from the workbook pivot records.
    /// </summary>
    private void ParsePivotRecords( List<BiffRecordRaw> arrPivotRecords )
    {
      List<PivotCacheInfo> arrPivotCacheInfo = CreatePivotCacheInfos( arrPivotRecords );

      for( int i = 0, len = arrPivotCacheInfo.Count; i < len; i++ )
      {
        PivotCacheInfo info = arrPivotCacheInfo[ i ];
        int id = info.StreamId;
        PivotCacheImpl cache = m_pivotCaches[ id ] as PivotCacheImpl;
        cache.Info = info;
        m_pivotCaches.Order.Add( id );
      }
    }
    // <summary>
    /// Recheck the Extended format Record
    /// </summary>
    private ExtendedFormatRecord RecheckExtendedFormatRecord(ExtendedFormatRecord xf)
    {
        if (xf == null)
            throw new ArgumentNullException("ExtendedFormatRecord");

        int value;
        if (m_modifiedFormatRecord.TryGetValue(xf.FormatIndex, out value))
        {
            xf.FormatIndex = Convert.ToUInt16(value);
        }
        return xf;
    }
    /// <summary>
    /// Recheck the Format Record
    /// </summary>
    private FormatRecord RecheckFormatRecord(FormatRecord format,ref int m_newValue)
    {
        if (format == null)
            throw new ArgumentNullException("FormatRecord");

        int index = format.Index;
        
        //Constant Ranges for the Format

        //if (!((index >= 5 && index <= 8) || (index >= 23 && index <= 26) || (index >= 41 && index <= 44) || (index >= 63 && index <= 66) || (index >= 164 && index <= 382)))

        //wrong index
        if (((index>=50 && index<=52)))
        {
            int value = FormatsCollection.DEF_FIRST_CUSTOM_INDEX + (m_newValue++);
            m_modifiedFormatRecord.Add(index,value);
            format.Index = value;
        }
        return format;
    }
    /// <summary>
    /// <summary>
    /// Extracts pivot caches information from the workbook pivot records.
    /// </summary>
    private List<PivotCacheInfo> CreatePivotCacheInfos( List<BiffRecordRaw> arrPivotRecords )
    {
      int iCurrentIndex = 0;
      int iCount = arrPivotRecords.Count;
      List<PivotCacheInfo> cacheInfoList = new List<PivotCacheInfo>();

      while( iCurrentIndex < iCount )
      {
        if( arrPivotRecords[ iCurrentIndex ].TypeCode == TBIFFRecord.StreamId )
        {
          PivotCacheInfo info = new PivotCacheInfo();
          iCurrentIndex = info.Parse( arrPivotRecords, iCurrentIndex );
          cacheInfoList.Add( info );
        }
        else
        {
          iCurrentIndex++;
        }
      }

      return cacheInfoList;
    }
    /// <summary>
    /// Normalizes border settings for specified extended format record in some incorrect files.
    /// </summary>
    /// <param name="xf">ExtendedFormatRecord to process.</param>
    private void NormalizeBorders( ExtendedFormatRecord xf )
    {
      if( xf.XFType == ExtendedFormatRecord.TXFType.XF_STYLE && !xf.IsNotParentBorder && xf.ParentIndex != MaxXFCount )
      {
        ExtendedFormatRecord parent = m_arrExtFormatRecords[ xf.ParentIndex ];

        if( xf.BorderBottom != parent.BorderBottom ||
          xf.BorderLeft != parent.BorderLeft ||
          xf.BorderRight != parent.BorderRight ||
          xf.BorderTop != parent.BorderTop )
        {
          xf.IsNotParentBorder = true;
        }
      }
    }
    /// <summary>
    /// Creates decryptor.
    /// </summary>
    /// <param name="password">Password that should be used for decryption.</param>
    /// <param name="filePass">Contains encryption information.</param>
    private IDecryptor CreateDecryptor( string password, FilePassRecord filePass )
    {
      if( filePass == null )
        throw new ArgumentNullException( "filePass" );

      if( filePass.IsWeakEncryption )
        throw new NotSupportedException( "Weak encryption algorithm is not supported." );

      FilePassStandardBlock standardBlock = filePass.StandardBlock;

      if( standardBlock == null )
        throw new NotSupportedException( "Strong encryption algorithms are not supported." );

      PasswordRequiredEventArgs args = null;
      IDecryptor result = null;

      CheckPasswordFirstTime( ref password, ref result, standardBlock );

      if( result == null )
      {
        // TODO: Later when we will support more than one decryption algorithm
        // we should use some kind of collection or factory to create decryptor instances.
        //return CreateDecryptor( filePass, password );

        result = new MD5Decryptor();

        byte[] arrDocId = standardBlock.DocumentID;
        m_arrDocId = arrDocId;
        byte[] arrEncryptDocId = standardBlock.EncyptedDocumentID;
        byte[] arrDigest = standardBlock.Digest;

        while( !result.SetDecryptionInfo( arrDocId, arrEncryptDocId, arrDigest, password ) )
        {
          // Here we have to raise second event to re-ask for password
          // and if no event is specified or user decided not to continue asking
          // for password then we have to throw an exception.
          args = new PasswordRequiredEventArgs();

          if( AppImplementation.RaiseOnWrongPassword( this, args ) )
          {
            if( !args.StopParsing )
            {
              password = args.NewPassword;
              result = new MD5Decryptor();
              continue;
            }
          }

          throw new ArgumentOutOfRangeException( "password", "Wrong password." );
        }
      }

      m_strEncryptionPassword = password;
      m_encryptionType = ExcelEncryptionType.Standard;

      return result;
    }
    /// <summary>
    /// Checks password provided password or asks to provide a password for the first time.
    /// </summary>
    /// <param name="password">Provided password; null if there was no password provided.</param>
    /// <param name="decryptor">Resulting decryptor object.</param>
    /// <param name="standardBlock">Part of the decryption structure used to check password.</param>
    private void CheckPasswordFirstTime( ref string password, ref IDecryptor decryptor,
      FilePassStandardBlock standardBlock )
    {
      if( password == null )
      {
        // Here we have to try standard password
        CheckStandardPassword( ref decryptor, standardBlock );

        if( decryptor == null )
        {
          PasswordRequiredEventArgs args = new PasswordRequiredEventArgs();

          if( AppImplementation.RaiseOnPasswordRequired( this, args ) )
          {
            password = args.NewPassword;
          }
          else
          {
            args = null;
          }

          if( args == null || args.StopParsing || password == null )
            throw new ArgumentException( "Workbook is protected and password wasn't specified." );
        }
      }
    }
    /// <summary>
    /// Checks whether document was encrypted with standard password and creates decryptor object if necessary.
    /// </summary>
    /// <param name="result">Decryptor to create.</param>
    /// <param name="standardBlock">Part of the decryption structure used to check password.</param>
    /// <returns>True if file was encrypted with standard password.</returns>
    private bool CheckStandardPassword( ref IDecryptor result, FilePassStandardBlock standardBlock )
    {
      result = new MD5Decryptor();

      byte[] arrDocId = standardBlock.DocumentID;
      m_arrDocId = arrDocId;
      byte[] arrEncryptDocId = standardBlock.EncyptedDocumentID;
      byte[] arrDigest = standardBlock.Digest;

      if( !result.SetDecryptionInfo( arrDocId, arrEncryptDocId, arrDigest, StandardPassword ) )
      {
        result = null;
      }

      return ( result != null );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEncryptor CreateEncryptor()
    {
      if( m_encryptionType == ExcelEncryptionType.Standard )
      {
        IEncryptor encryptor = new MD5Decryptor();

        if( m_arrDocId == null )
        {
          Guid docId = Guid.NewGuid();
          m_arrDocId = docId.ToByteArray();
        }

        encryptor.SetEncryptionInfo( m_arrDocId, m_strEncryptionPassword );
        return encryptor;
      }
      else
      {
        throw new NotSupportedException( "Not supported encryption type." );
      }
    }
    /// <summary>
    /// Parses internal sst record.
    /// </summary>
    /// <param name="sst">SSTRecord to parse.</param>
    /// <param name="options">Parse options.</param>
    private void ParseSSTRecord( SSTRecord sst, ExcelParseOptions options )
    {
      m_SSTDictionary.OriginalSST = sst;
      //TextWithFormat[] arrStrings = sst.Strings;

      //for( int i = 0; i < arrStrings.Length; i++ )
      //{
      //  int index = m_SSTDictionary.AddIncrease( arrStrings[ i ], options, false );
      //  m_arrNewSSTIndexes.Add( index );
              
      //  // To keep references count equal to zero on till cells extracting.
      //  //m_SSTDictionary.DecreaseOnly( index );
      //}

      ////Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, m_SSTDictionary.Count, "Strings in dictionary" );
      ////Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, arrStrings.Length, "Strings in file" );
    }
    /// <summary>
    /// Prepares fonts, formats and styles collections.
    /// </summary>
    /// <param name="bIgnoreStyles">Indicates whether parsing is made in ignore styles mode.</param>
    /// <param name="arrStyles">List with all styles.</param>
    /// <param name="hashNewXFormatIndexes">Dictionary with new extended format indexes for ignore styles mode.</param>
    internal void PrepareStyles( bool bIgnoreStyles, List<StyleRecord> arrStyles,
      Dictionary<int, int> hashNewXFormatIndexes )
    {
      PrepareExtendedFormats( bIgnoreStyles, arrStyles );

      m_rawFormats.InsertDefaultFormats();

      if( !bIgnoreStyles )
      {
        CreateAllStyles( arrStyles );
        // Insertion of default styles into collection.
        InsertDefaultExtFormats();
        InsertDefaultStyles( arrStyles );
      }
      else
      {
        InsertDefaultFonts();
        InsertDefaultExtFormats();
        InsertDefaultStyles();
        CreateStyleForEachFormat( hashNewXFormatIndexes );
      }
    }
    /// <summary>
    /// Prepares extended format records.
    /// </summary>
    private void PrepareExtendedFormats( bool bIgnoreStyle, List<StyleRecord> arrStyles )
    {
      if( !bIgnoreStyle )
      {
        int iCount = m_arrExtFormatRecords.Count;

        for( int i = 0; i < iCount; i++ )
        {
          // Here we should fix records if they are broken.
          ExtendedFormatRecord xfRecord = m_arrExtFormatRecords[ i ];
          NormalizeBorders( xfRecord );

          ExtendedXFRecord xfExtRecord = (ExtendedXFRecord)BiffRecordFactory.GetRecord(TBIFFRecord.ExtendedXFRecord);
 
          for (int j = 0; j <m_arrXFExtRecords.Count; j++)
          {
              if (m_arrXFExtRecords[j].XFIndex == i)
              {
                  xfExtRecord = m_arrXFExtRecords[j];
                  break;
              }
          }

          if (xfRecord.XFType == ExtendedFormatRecord.TXFType.XF_STYLE && xfRecord.ParentIndex >= MaxXFCount)
          {
              xfRecord.ParentIndex = 0;
          }
          int iParentIndex = xfRecord.ParentIndex;

          if( iParentIndex != MaxXFCount )
          {
            ExtendedFormatRecord xfParent = ( ExtendedFormatRecord )m_arrExtFormatRecords[ iParentIndex ];

            if( !xfRecord.IsNotParentFont && xfRecord.FontIndex != xfParent.FontIndex )
              xfRecord.IsNotParentFont = true;

            if( !xfRecord.IsNotParentAlignment &&
              xfRecord.AlignmentOptions != xfParent.AlignmentOptions )
            {
              xfRecord.IsNotParentAlignment = true;
            }

            if( !xfRecord.IsNotParentFormat
              && xfRecord.FormatIndex != xfParent.FormatIndex )
            {
              xfRecord.IsNotParentFormat = true;
            }
          }

          ExtendedFormatImpl extF = new ExtendedFormatImpl(Application, this, xfRecord, xfExtRecord, true);
          m_extFormats.ForceAdd( extF );
        }

        for( int i = 0; i < iCount; i++ )
        {
          m_extFormats[ i ].UpdateFromParent();
        }
      }
    }
    /// <summary>
    /// Parses autofilters.
    /// </summary>
    private void ParseAutoFilters()
    {
      for( int i = 0, len = m_worksheets.Count; i < len; i++ )
      {
        WorksheetImpl sheet = ( WorksheetImpl )m_worksheets[ i ];
        sheet.ParseAutoFilters();
      }
    }

    /// <summary>
    /// Method to extract all worksheets from stream.
    /// </summary>
    /// <param name="reader">BiffReader that contains worksheets.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="iFirstSheet">First worksheet to parse.</param>
    /// <param name="iLastSheet">Last worksheet to parse.</param>
    /// <param name="hashNewXFormatIndexes">
    /// Dictionary with new extended format indexes for ignore styles mode.
    /// </param>
    /// <param name="decryptor">Object used to decrypt encrypted records.</param>
    private void ExtractWorksheetsFromStream( BiffReader reader, ExcelParseOptions options
      , int iFirstSheet, int iLastSheet, Dictionary<int, int> hashNewXFormatIndexes, IDecryptor decryptor )
    {
      //      m_styles.BuildStylesHash();

#if MEASURE_PERFORMANCE
      DateTime start = DateTime.Now;
#endif
      
      ReadWorksheetsData( reader, options, iFirstSheet, iLastSheet, hashNewXFormatIndexes, decryptor );
#if MEASURE_PERFORMANCE
      TimeSpan readSheetDataTime = DateTime.Now - start;
      Console.WriteLine( "Read worksheets data: {0}", readSheetDataTime );
#endif
      PrepareNames();
      ParseNames();
      
      if(options!=ExcelParseOptions.ParseWorksheetsOnDemand)
          ParseWorksheets();
      //m_styles.ClearStylesHash();
    }
    /// <summary>
    /// Reads worksheet data from the reader without parsing worksheets.
    /// </summary>
    /// <param name="reader">Reader to read data from.</param>
    /// <param name="options">Parse options.</param>
    /// <param name="iFirstSheet">The first worksheet to parse.</param>
    /// <param name="iLastSheet">The last worksheet to parse.</param>
    /// <param name="hashNewXFormatIndexes">Dictionary with new extended format indexes for ignore styles mode.</param>
    /// <param name="decryptor">Object used to decrypt encrypted records.</param>
    private void ReadWorksheetsData( BiffReader reader, ExcelParseOptions options
      , int iFirstSheet, int iLastSheet, Dictionary<int, int> hashNewXFormatIndexes, IDecryptor decryptor )
    {
      long iFullSize = reader.BaseStream.Length;
      int iPos = -1;

      if( iLastSheet == -1 ) iLastSheet = int.MaxValue;
      bool bSkipCharts = ( ( options & ExcelParseOptions.DoNotParseCharts ) != 0 );
      BOFRecord bofRecord = null;
      int iCount = m_arrBound.Count;
      m_arrObjects.EnsureCapacity( iCount );
      m_worksheets.EnsureCapacity( iCount );

      do
      {
        // Check if there is a real worksheet from that point,
        // otherwise skip all not needed data and continue from
        // worksheet part of stream.
        if( !reader.IsEOF )
        {
          BiffRecordRaw bof = reader.PeekRecord();
          
          // After EOF, always must be BOF or end of stream.
          if( bof.TypeCode != TBIFFRecord.BOF )
            throw new WrongBiffStreamFormatException();

          bofRecord = ( BOFRecord ) bof;
        }

        iPos++;
        bool bSkip = ( iPos < iFirstSheet || iPos > iLastSheet );
        BoundSheetRecord bound = ( BoundSheetRecord )m_arrBound[ iPos ];

        ITabSheet newSheet = null;

        switch( bofRecord.Type )
        {
          case BOFRecord.TType.TYPE_CHART:
            IChart chart = m_charts.Add( reader, options, bSkip, hashNewXFormatIndexes, decryptor );
            newSheet = chart;
            break;

          case BOFRecord.TType.TYPE_WORKSHEET:
            IWorksheet sheet = m_worksheets.Add( reader, options, bSkip, hashNewXFormatIndexes, decryptor );
            newSheet = sheet;
            ( ( WorksheetImpl )sheet ).Type = ( ExcelSheetType )bound.BoundSheetType;            
            break;

          default:
            WorksheetImpl unknownSheet = AppImplementation.CreateWorksheet( this, reader, options, bSkip,
              hashNewXFormatIndexes, decryptor );
            unknownSheet.Type = ( ExcelSheetType )bound.BoundSheetType;
            newSheet = unknownSheet;

            if( unknownSheet.Type == ExcelSheetType.Worksheet )
            {
              m_worksheets.Add( unknownSheet );
            }
            else
            {
              m_arrObjects.Add( unknownSheet );
            }
            break;
        }

        newSheet.Name = bound.SheetName;
        newSheet.Visibility = bound.Visibility;

        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, newSheet.Name, "Parsed worksheet" );
        
        AppImplementation.RaiseProgressEvent( reader.BaseStream.Position, iFullSize );
      }
      while( !reader.IsEOF && iPos < m_arrBound.Count - 1 );
    }

    /// <summary>
    /// Prepares named ranges (just create them without parsing).
    /// </summary>
    private void PrepareNames()
    {
      for( int i = 0, len = m_arrNames.Count; i < len; i++ )
      {
        NameRecord name = m_arrNames[ i ];

        if( name.IndexOrGlobal == 0 )
        {
          m_names.Add( name );
        }
        else
        {
          IWorksheet sheet = ( IWorksheet )m_arrObjects[ name.IndexOrGlobal - 1 ];//m_worksheets[ name.IndexOrGlobal - 1 ];
          WorksheetNamesCollection names = ( WorksheetNamesCollection )sheet.Names;
          names.Add( name );
        }
      }
    }
    /// <summary>
    /// Parses set of Name records.
    /// </summary>
    private void ParseNames()
    {
      m_names.ParseNames();
    }
    /// <summary>
    /// Parses all worksheets, charts, etc.
    /// </summary>
    private void ParseWorksheets()
    {
      for( int i = 0, len = m_arrObjects.Count; i < len; i++ )
      {
        IParseable toParse = ( IParseable )m_arrObjects[ i ];

#if DEBUG
        INamedObject name = toParse as INamedObject;
        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, name.Name, "Parsing object" );
#endif
        toParse.Parse();

        WorksheetBaseImpl sheet = ( WorksheetBaseImpl )toParse;

        //if( ActiveSheetIndex < 0 || sheet.WindowTwo.IsSelected )
//        if( sheet.WindowTwo.IsPaged )
//        {
//          //ActiveSheetIndex = sheet.RealIndex;
//          m_ActiveSheet = sheet;
//        }
      }

      if( m_windowOne.SelectedTab != ushort.MaxValue )
      {
        m_ActiveSheet = ( WorksheetBaseImpl )m_arrObjects[ m_windowOne.SelectedTab ];
      }
      else
      {
        ( ( WorksheetBaseImpl )m_arrObjects[ 0 ] ).Activate();
      }
    }
    /// <summary>
    /// Parses all worksheets, charts, etc. on demand
    /// </summary>
    internal void ParseWorksheetsOnDemand()
    {
        if (m_windowOne.SelectedTab != ushort.MaxValue)
        {
            m_ActiveSheet = (WorksheetBaseImpl)m_arrObjects[m_windowOne.SelectedTab];
        }
        else
        {
            ((WorksheetBaseImpl)m_arrObjects[0]).Activate();
        }
    }
    /// <summary>
    /// Parse worksheets if worksheets were loaded on demand
    /// </summary>
    internal void CheckParseOnDemand()
    {
        if (this.IsLoaded || !this.m_bLoading)
        {

            for (int i = 0; i < m_arrObjects.Count; i++)
            {
                IParseable toParse = (IParseable)m_arrObjects[i];

                if (toParse != null && toParse is WorksheetImpl)
                {
                    if ((toParse as WorksheetImpl).ParseOnDemand)
                    {
                        this.m_bIsLoaded = false;
                        this.m_bLoading = true;

                        toParse.Parse();

                        this.m_bIsLoaded = true;
                        this.m_bLoading = false;
                    }
                }
                else if (toParse != null && toParse is ChartImpl)
                {
                    if ((toParse as ChartImpl).ParseOnDemand)
                    {
                        this.m_bIsLoaded = false;
                        this.m_bLoading = true;

                        toParse.Parse();

                        (toParse as ChartImpl).ParseOnDemand = false;
                        this.m_bIsLoaded = true;
                        this.m_bLoading = false;
                    }
                }
            }


            

            //foreach (WorksheetImpl sheet in this.Worksheets)
            //{
            //    if (sheet.ParseOnDemand)
            //        sheet.ParseData();
            //}
        }
    }
    /// <summary>
    /// Reparses all ranges that were not parsed because of 
    /// insufficient data (that weren't loaded when parsing).
    /// </summary>
    private void Reparse()
    {
      if( !m_bLoading )
      {
        for( int i = 0, len = m_arrReparse.Count; i < len; i++ )
        {
          IReparse reparse = m_arrReparse[ i ];
          reparse.Reparse();
        }

        m_arrReparse.Clear();
      }
    }
    /// <summary>
    /// Creates all necessary styles for the workbook.
    /// </summary>
    /// <param name="arrStyles">Array of all read StyleRecords.</param>
    private void CreateAllStyles( List<StyleRecord> arrStyles )
    {
      for( int i = 0, len = arrStyles.Count; i < len; i++ )
      {
        StyleRecord style = arrStyles[ i ];
        int xfIndex = style.ExtendedFormatIndex;

        ExtendedFormatImpl format = InnerExtFormats[ xfIndex ];

        if( format.HasParent )
        {
          ExtendedFormatImpl styleFormat = ( ExtendedFormatImpl )format.Clone();
          styleFormat.ParentIndex = MaxXFCount;
          styleFormat.Record.XFType = ExtendedFormatRecord.TXFType.XF_CELL;
          styleFormat = m_extFormats.ForceAdd( styleFormat );
          format.ParentIndex = styleFormat.Index;
          style.ExtendedFormatIndex = ( ushort )styleFormat.Index;
          format = styleFormat;
          //format.ParentIndex = MaxXFCount;
        }

        if( style.IsBuildInStyle )
        {
          StyleImpl builtInStyle = AppImplementation.CreateStyle( this, style );

          if( !m_styles.ContainsName( builtInStyle.Name ) )
            m_styles.Add( builtInStyle, true );
        }
        else
        {
          if( style.Name == null || style.Name.Length == 0 )
          {
            style.StyleName = CollectionBaseEx<WorksheetImpl>.GenerateDefaultName( arrStyles, "UNKNOWNSTYLE_" );
          }

          m_styles.Add( style );
        }
      }
    }
    /// <summary>
    /// Searches StyleRecord for the specified index of the Extended 
    /// Format record in the list.
    /// </summary>
    /// <param name="arrStyles">List that contains StyleRecord.</param>
    /// <param name="formatIndex">Index of the Extended Format.</param>
    /// <param name="iStyleIndex">
    /// Found style index (if it is greater or equal to zero than it is index
    /// from arrStyles array; otherwise it is evaluated as index in arrDefaultStyles
    /// increased by 1 and multiplied by -1.
    /// .</param>
    private StyleRecord FindStyleRecord( List<StyleRecord> arrStyles, int formatIndex,
      out int iStyleIndex )
    {
      iStyleIndex = -1;

      for( int i = 0, len = arrStyles.Count; i < len; i++ )
      {
        StyleRecord style = arrStyles[ i ];

        if( style.ExtendedFormatIndex == formatIndex )
        {
          iStyleIndex = i;
          return style;
        }
      }

      // Let's try to search in default styles (maybe they where not included).
      StyleRecord[] arrDefaultStyles = GetDefaultStyles();

      for( int i = 0, len = arrDefaultStyles.Length; i < len; i++ )
      {
        StyleRecord style = arrDefaultStyles[ i ];

        if( style.ExtendedFormatIndex == formatIndex )
        {
          iStyleIndex = -i - 1;
          return style;
        }
      }

      return null;
    }

    /// <summary>
    /// Returns array of default styles.
    /// </summary>
    /// <returns>Array of StyleRecords with default styles.</returns>
    private StyleRecord[] GetDefaultStyles()
    {
      List<StyleRecord> result = new List<StyleRecord>( 7 );
      
      StyleRecord style = ( StyleRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Style );
      style.ExtendedFormatIndex = 16;
      style.BuildInOrNameLen = 3;
      result.Add( style );
      
      style = ( StyleRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Style );
      style.ExtendedFormatIndex = 17;
      style.BuildInOrNameLen = 6;
      result.Add( style );
      
      style = ( StyleRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Style );
      style.ExtendedFormatIndex = 18;
      style.BuildInOrNameLen = 4;
      result.Add( style );
      
      style = ( StyleRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Style );
      style.ExtendedFormatIndex = 19;
      style.BuildInOrNameLen = 7;
      result.Add( style );
      
      style = ( StyleRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Style );
      result.Add( style );
      
      style = ( StyleRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Style );
      style.ExtendedFormatIndex = 20;
      style.BuildInOrNameLen = 5;
      result.Add( style );

      return result.ToArray();
    }

    /// <summary>
    /// Creates style each number format.
    /// </summary>
    /// <param name="hashNewXFormatIndexes">
    /// Dictionary with new extended format indexes.
    /// </param>
    private void CreateStyleForEachFormat( Dictionary<int, int> hashNewXFormatIndexes )
    {
      foreach( KeyValuePair<int, FormatImpl> entry in m_rawFormats )
      {
        int iFormatIndex = entry.Key;
        string strStyleName = DEF_FORMAT_STYLE_NAME_START + iFormatIndex;

        if( !m_styles.ContainsName( strStyleName ) )
        {
          StyleImpl style = ( StyleImpl )m_styles.Add( strStyleName, "Normal" );
          style.NumberFormat = entry.Value.FormatString;

          ExtendedFormatImpl format = style.Wrapped;
          format = format.CreateChildFormat();
          int iNewFormatIndex = format.Index;
          hashNewXFormatIndexes.Add( iFormatIndex, iNewFormatIndex );
        }
      }
    }

    /// <summary>
    /// Read document properties from StgStream.
    /// </summary>
    /// <param name="storage">Storage to read properties from.</param>
    private void ReadDocumentProperties( ICompoundStorage storage )
    {
      try
      {
      bool bDocumentPropertiesParsed = false;

#if !SILVERLIGHT && !WINRT && !WP
      Storage typedStorage = storage as Syncfusion.CompoundFile.XlsIO.Native.Storage;

      if( typedStorage != null )
      {
        ReadDocumentPropertiesNative( typedStorage );
        bDocumentPropertiesParsed = true;
      }
#endif

      if( !bDocumentPropertiesParsed )
        ReadDocumentPropertiesManaged( storage );
      }
      catch( Exception ex )
      {
        Debug.WriteLine( "Problems reading document properties: " + ex.Message );
      }
    }
#if !(SILVERLIGHT) && !(WINRT) && !(WP)
    /// <summary>
    /// Reads document properties using native objects.
    /// </summary>
    /// <param name="storage">Storage to get document properties from.</param>
    private void ReadDocumentPropertiesNative( Syncfusion.CompoundFile.XlsIO.Native.Storage storage )
    {
      IPropertySetStorage setProp;
      int error = Syncfusion.CompoundFile.XlsIO.Native.API.StgCreatePropSetStg( storage.COMStorage, 0, out setProp );

      if( error != 0 )
      {
        //throw new ExternalException( "cannot create Storage properties stream", error );
        Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, string.Format(
          "cannot create property set storage. Error code = {0:X}", error ), "Error" );

        return;
      }

#if MEASURE_PERFORMANCE
      WorksheetPerfCounter counter = new WorksheetPerfCounter();
      counter.Start();
#endif

      m_builtInDocumentProperties.Parse( setProp );
      m_customDocumentProperties.Parse( setProp );

#if MEASURE_PERFORMANCE
      float result = counter.Finish();
      Console.WriteLine( "Parsing time {0}", result );
#endif

      Marshal.FinalReleaseComObject( setProp );
    }
#endif
    /// <summary>
    /// Read document properties using managed classes.
    /// </summary>
    /// <param name="storage">Storage to get document properites from.</param>
    private void ReadDocumentPropertiesManaged( ICompoundStorage storage )
    {
      if( storage.ContainsStream( DEF_SUMMARY_INFO ) )
      {
        using( Stream propertiesStream = storage.OpenStream( DEF_SUMMARY_INFO ) )
        {
          DocumentPropertyCollection properties = new DocumentPropertyCollection( propertiesStream );
          m_builtInDocumentProperties.Parse( properties );
        }
      }

      if( storage.ContainsStream( DEF_DOCUMENT_SUMMARY_INFO ) )
      {
        using( Stream propertiesStream = storage.OpenStream( DEF_DOCUMENT_SUMMARY_INFO ) )
        {
          DocumentPropertyCollection properties = new DocumentPropertyCollection( propertiesStream );
          m_builtInDocumentProperties.Parse( properties );
          m_customDocumentProperties.Parse( properties );
        }
      }
    }
    #endregion

    #region IWorkbook methods
    /// <summary>
    /// Creates the Data sorter to sort the data..
    /// </summary>
    /// <returns>Data Sorter.</returns>
    public IDataSort CreateDataSorter()
    {
        if (m_dataSorter == null)
            m_dataSorter = new DataSorter(this);
        return m_dataSorter;
    }
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Copies to the clipboard whole workbook.
    /// </summary>
    public void CopyToClipboard()
    {
      CopyToClipboard( null );
    }
#endif
    /// <summary>
    /// Activates the first window associated with the workbook.
    /// </summary>
    public void Activate()
    {
      (( ApplicationImpl )this.Application).SetActiveWorkbook( this );
    }
#if !(WINRT )
    /// <summary>
    /// Closes the object and saves changes into specified file.
    /// </summary>
    /// <param name="Filename">
    /// File name in which workbook will be saved if SaveChanges is true.
    /// </param>
    public void Close( string Filename )
    {
      Close( Filename != null && Filename.Length > 0, Filename );
    }
#endif
    /// <summary>
    /// Closes the object.
    /// </summary>
    /// <param name="SaveChanges">If True, all changes will be saved.</param>
    /// <param name="Filename">
    /// File name in which workbook will be saved if SaveChanges is true.
    /// </param>
    public void Close( bool SaveChanges, string Filename )
    {
#if !(WINRT || WP)
      if( true == SaveChanges )
      {
        if( Filename != null )
        {
          SaveAs( Filename );
        }
        else if( m_strFullName != null )
        {
          Save();
        }
      }
#endif
      if( Parent is IList )
      {
        IList list = ( IList )Parent;
        int index = list.IndexOf( this );
        
        if( index >= 0 )
        {
          list.RemoveAt( index );
        }
      }
      
      DisposeAll();
      //ClearAll();
      //      Debug.WriteLine( ValueChangedEventArgs.m_iInstancesCount, "Instances" );
      //      Debug.WriteLine( ValueChangedEventArgs.m_iNameReferenced, "Name referenced" );
      //      Debug.WriteLine( ValueChangedEventArgs.m_iNewReferenced, "New referenced" );
      //      Debug.WriteLine( ValueChangedEventArgs.m_iOldReferenced, "Old referenced" );
      // Force GC to release COM objects interfaces.
      //GC.Collect( GC.MaxGeneration );
      //GC.WaitForPendingFinalizers();
     if(AppImplementation.Workbooks!=null )
         ((AppImplementation.Workbooks) as WorkbooksCollection).Remove(this);
      GC.SuppressFinalize( this );
    }

    /// <summary>
    /// Closes the object.
    /// </summary>
    /// <param name="saveChanges">If TRUE all changes will be saved</param>
    public void Close( bool saveChanges )
    {
      Close( saveChanges, null );
    }

    /// <summary>
    /// Closes the object without saving.
    /// </summary>
    public void Close()
    {
      Close( false );
    }
    /// <summary>
    /// Creates object that can be used for template markers processing.
    /// </summary>
    /// <returns>Object that can be used for template markers processing.</returns>
    public ITemplateMarkersProcessor CreateTemplateMarkersProcessor()
    {
      return AppImplementation.CreateTemplateMarkers( this );
    }
    /// <summary>     
    /// Marks workbook as final and Read-Only.   
    /// </summary>   
    public void MarkAsFinal()
    {
      DocumentPropertyImpl customProperty = null;
      customProperty = ( DocumentPropertyImpl )m_customDocumentProperties.Add( "_MarkAsFinal" );
      customProperty.Boolean = true;
    }
#if !(WINRT || WP)
    /// <summary>
    /// Saves changes to the specified workbook.
    /// </summary>
    /// <exception cref="System.ApplicationException">
    /// If file name was not specified before.
    /// </exception>
    public void Save()
    {
      if( m_strFullName == null || m_strFullName.Length == 0 )
        throw new ApplicationException( "Workbook was not created from file." + 
          " That is why woorkbook file not specified." + 
          " You must use SaveAs method instead." );

      SaveAs(m_strFullName,((m_fileDataHolder!=null)? m_fileDataHolder.GetWorkbookPartType(): ExcelSaveType.SaveAsXLS));
    }
    /// <summary>
    /// Short variant of SaveAs method.
    /// </summary>
    /// <param name="FileName">
    /// Name of the file into which workbook will be saved.
    /// </param>
    /// <exception cref="System.ArgumentNullException">
    /// When FileName is NULL.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// When FileName is empty.
    /// </exception>
    public void SaveAs( string FileName )
    {
      SaveAs( FileName, ExcelSaveType.SaveAsXLS, Version );
    }
    /// <summary>
    /// Short variant of SaveAs method.
    /// </summary>
    /// <param name="FileName">
    /// Name of the file into which workbook will be saved.
    /// </param>
    /// <param name="saveType">Options for save.</param>
    /// <exception cref="System.ArgumentNullException">
    /// When FileName is NULL.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// When FileName is empty.
    /// </exception>
    public void SaveAs( string FileName, ExcelSaveType saveType )
    {
      SaveAs( FileName, saveType, Version );
    }
    /// <summary>
    /// Short variant of SaveAs method.
    /// </summary>
    /// <param name="FileName">
    /// Name of the file into which workbook will be saved.
    /// </param>
    /// <param name="saveType">Options for save.</param>
    /// <param name="version">Excel version that should be used.</param>
    /// <exception cref="System.ArgumentNullException">
    /// When FileName is NULL.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    /// When FileName is empty.
    /// </exception>
    public void SaveAs( string FileName, ExcelSaveType saveType, ExcelVersion version )
    {
      if( FileName == null )
        throw new ArgumentNullException( "Filename" );

      if( FileName.Length == 0 )
        throw new ArgumentException( "FileName cannot be empty." );


      // NOTE: Potentially from that point if we detect that file
      // must be overridden, we must ask user. Workbook_BeforeSave
      // event not supported by us at the current moment.

      string tmpFullPath = Path.GetFullPath( FileName );
      string dir = Path.GetDirectoryName( tmpFullPath );
      m_bSaving = true;
      
      if( File.Exists( tmpFullPath ) )
      {
        FileAttributes attrib = 
#if !(SILVERLIGHT || WP)
          File.GetAttributes( tmpFullPath );
#else
          FileAttributes.Normal;
#endif

        if( ( attrib & FileAttributes.ReadOnly ) != 0 )
        {
          RaiseReadOnlyFileEvent( tmpFullPath );
        }

        if( Application.DeleteDestinationFile && tmpFullPath != m_strFullName )
        {
          File.Delete( tmpFullPath );
        }
      }

      if( dir.Length != 0 )
      {
        // Create directory if it does not exist.
        if( dir != null && dir.Length > 0 && !System.IO.Directory.Exists( dir ) )
        {
          System.IO.Directory.CreateDirectory( dir );
        }
      }
      if (Styles.Count > 0)
      {
          FontImpl normalFont = (Styles["Normal"].Font as FontWrapper).Wrapped;
          m_iFirstCharSize = (int)Math.Round(normalFont.MeasureString(WorksheetImpl.DEF_STANDARD_CHAR.ToString()).Width);
          m_iSecondCharSize = (int)Math.Round(normalFont.MeasureCharacter(WorksheetImpl.DEF_STANDARD_CHAR).Width);
      }
      //PrepareShapes();

      //WorkbookExcel97Serializator serializator = new WorkbookExcel97Serializator();
      IdReserver shapeIds = PrepareShapes( GetWorksheetShapes );
      IdReserver headerFooterIds = PrepareShapes( GetHeaderFooterShapes );

      IWorkbookSerializator serializator = CreateSerializator( version, shapeIds );
      serializator.Serialize( tmpFullPath, this, saveType );

      // Update FullName property value.
      m_strFullName = tmpFullPath;

      // After save, mark workbook as saved.
      m_bSaving = false;
      m_bSaved = true;
      
      RaiseSavedEvent();
      
      // Force GC to release COM objects interfaces.
      // GC.Collect( GC.MaxGeneration );
      // GC.WaitForPendingFinalizers();
#if DEBUG
//      //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, m_arrFontWrappers.Count, "FontsCount in " + m_strFullName );
//      //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, string.Format(
//        "FontsCount in {0} is {1}. Referenced fonts {2}. Read-only fonts {3}. Null wrappers {4}."
//        , Path.GetFileName( m_strFullName ) , m_arrFontWrappers.Count, GetReferencedFonts()
//        , GetReadOnlyFonts(), GetNullReferenced() ) );
#endif
    }
#endif
#if !(SILVERLIGHT) && !(WINRT) && !(WP)
    /// <summary>
    /// Saves the html files.
    /// </summary>
    /// <param name="fileName">The filename</param>
    /// <param name="saveOption">The saveoption</param>
    public void SaveAsHtml(string fileName, HtmlSaveOptions saveOption)
    {
        if (fileName == null)
            throw new ArgumentNullException("Filename");

        if (fileName.Length == 0)
            throw new ArgumentException("FileName cannot be empty.");

        string fullPath = Path.GetFullPath(fileName);

        if (File.Exists(fullPath))
        {
            FileAttributes attrib = File.GetAttributes(fullPath);

            if ((attrib & FileAttributes.ReadOnly) != 0)
                RaiseReadOnlyFileEvent(fullPath);

            if (Application.DeleteDestinationFile && fullPath != m_strFullName)
                File.Delete(fullPath);

        }


        string outputDirectoryPath = string.Format("{0}_files", Path.Combine(Path.GetDirectoryName(fullPath), Path.GetFileNameWithoutExtension(fullPath)));


        if (System.IO.Directory.Exists(outputDirectoryPath))
        {
            System.IO.Directory.Delete(outputDirectoryPath, true);
            System.IO.Directory.CreateDirectory(outputDirectoryPath);
        }
        else
            System.IO.Directory.CreateDirectory(outputDirectoryPath);

        using (FileStream stream = new FileStream(fileName, FileMode.CreateNew))
        {

            ExcelToHtmlConverter converter = new ExcelToHtmlConverter();
            converter.ConvertToHtml(stream, this, outputDirectoryPath, saveOption);

            stream.Close();
        }
    }
    /// <summary>
    /// Saves as HTML.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="saveOption">The save option.</param>
   public void SaveAsHtml(Stream stream, HtmlSaveOptions saveOption)
    {
        ExcelToHtmlConverter converter = new ExcelToHtmlConverter();
        converter.ConvertToHtml(stream, this, saveOption);
        stream.Flush();     
    }
    /// <summary>
    /// Saves as HTML.
    /// </summary>
    /// <param name="stream">The stream.</param>
    public void SaveAsHtml(Stream stream)
    {
        SaveAsHtml(stream, HtmlSaveOptions.Default);
    }
#endif
    #region Shapes Id
    public delegate ShapeCollectionBase ShapesGetterMethod( ITabSheet sheet );
    /// <summary>
    /// Prepares shapes for serialization.
    /// </summary>
    private IdReserver PrepareShapes( ShapesGetterMethod shapesGetter )
    {
      // Step 1. Re-index shape collections if necessary
      bool bShapesPresent = ReIndexShapeCollections( shapesGetter );
      // Step 2. Fill IdReserver object
      bool bChanged;
      IdReserver shapeIdReserver = FillReserverFromShapes( shapesGetter, out bChanged );

      if( bShapesPresent )
      {
        // Step 3. Add new shapes
        if( m_shapesData != null && bChanged )
          m_shapesData.ClearPreservedClusters();

        RegisterNewShapes( shapeIdReserver, shapesGetter );
      }

      return shapeIdReserver;
    }
    /// <summary>
    /// Registers new shapes (that have no shapeId yet) inside shapeIdReserver.
    /// </summary>
    /// <param name="shapeIdReserver">IdReserver that helps in the id generation process.</param>
    /// <param name="shapesGetter">Delegate used to get necessary shapes collection from a TabSheet.</param>
    private void RegisterNewShapes( IdReserver shapeIdReserver, ShapesGetterMethod shapesGetter )
    {
      if( shapeIdReserver == null )
        throw new ArgumentNullException( "shapeIdReserver" );

      // 1. Try to update shape collections that already has some shape id's reserved.
      UpdateAddedShapes( shapeIdReserver, shapesGetter );
      // 2. Update new ones.
      RegisterNewShapeCollections( shapeIdReserver, shapesGetter );
    }
    /// <summary>
    /// Updates shapes that were added after last save operation.
    /// </summary>
    /// <param name="shapeIdReserver">IdReserver that helps in the id generation process.</param>
    /// <param name="shapesGetter">Delegate used to get necessary shapes collection from a TabSheet.</param>
    private void UpdateAddedShapes( IdReserver shapeIdReserver, ShapesGetterMethod shapesGetter )
    {
      //for( int i = 0, len = m_arrObjects.Count; i < len; i++ )
      foreach( ShapeCollectionBase shapes in EnumerateShapes( shapesGetter ) )
      {
        //ITabSheet tabSheet = TabSheets[ i ];
        //ShapeCollectionBase shapes = shapesGetter( tabSheet );

        if( shapes.StartId != 0 )
        {
          // 1. Find out number of shapes to update.
          int iShapesToUpdate = GetShapesWithoutId( shapes );

          if( iShapesToUpdate > 0 )
          {
            // 2. Find out number of free shape indexes (maybe even all free indexes too).
            int iFreeIndexes = GetShapesFreeIndexes( shapeIdReserver, shapes );

            // 3. Re-allocate whole collection if necessary or
            if( iShapesToUpdate > iFreeIndexes )
            {
              shapeIdReserver.FreeSequence( shapes.CollectionIndex );
              AssignIndexes( shapeIdReserver, shapes );
            }
            // 4. assign indexes to new shapes.
            else
            {
              AssignNewIndexes( shapeIdReserver, shapes );
            }
          }
        }
      }
    }
    /// <summary>
    /// Assigns indexes to the new shapes.
    /// </summary>
    /// <param name="shapeIdReserver">IdReserver that helps in the id generation process.</param>
    /// <param name="shapes">Shape collection to process.</param>
    private void AssignNewIndexes( IdReserver shapeIdReserver, ShapeCollectionBase shapes )
    {
      int iLastId = shapes.LastId;
      for( int i = 0, iShapesCount = shapes.Count; i < iShapesCount; i++ )
      {
        ShapeImpl shape = ( shapes[ i ] as ShapeImpl );

        if( shape.ShapeId == 0 )
          shape.ShapeId = ++iLastId;
      }

      shapes.LastId = iLastId;
    }
    /// <summary>
    /// Gets number of free indexes inside currently allocated/reserved ids.
    /// </summary>
    /// <param name="shapeIdReserver">IdReserver that helps in the id generation process.</param>
    /// <param name="shapes">Shape collection to process.</param>
    /// <returns>Number of free indexes inside currently allocated/reserved ids.</returns>
    private int GetShapesFreeIndexes( IdReserver shapeIdReserver, ShapeCollectionBase shapes )
    {
      if( shapeIdReserver == null )
        throw new ArgumentNullException( "shapeIdReserver" );

      if( shapes == null )
        throw new ArgumentNullException( "shapes" );

      int iReservedCount = shapeIdReserver.GetReservedCount( shapes.CollectionIndex );
      return iReservedCount + shapes.StartId - shapes.LastId;
    }
    /// <summary>
    /// Evaluates number of shapes without assigned id.
    /// </summary>
    /// <param name="shapes">Shape collection to check.</param>
    /// <returns>Number of shapes without assigned id.</returns>
    private int GetShapesWithoutId( ShapeCollectionBase shapes )
    {
      if( shapes == null )
        throw new ArgumentNullException( "shapes" );

      int result = 0;

      for( int i = 0, len = shapes.Count; i < len; i++ )
      {
        ShapeImpl shape = shapes[ i ] as ShapeImpl;

        if( shape.ShapeId <= 0 )
          result++;
      }

      return result;
    }
    /// <summary>
    /// Registers absolutely new shape collections using IdReserver.
    /// </summary>
    /// <param name="shapeIdReserver">IdReserver that helps in the id generation process.</param>
    /// <param name="shapesGetter">Delegate used to get necessary shapes collection from a TabSheet.</param>
    private void RegisterNewShapeCollections( IdReserver shapeIdReserver,
      ShapesGetterMethod shapesGetter )
    {
      if( shapeIdReserver == null )
        throw new ArgumentNullException( "shapeIdReserver" );

      //for( int i = 0, len = m_arrObjects.Count; i < len; i++ )
      foreach( ShapeCollectionBase shapes in EnumerateShapes( shapesGetter ) )
      {
        //ITabSheet tabSheet = TabSheets[ i ];
        //ShapeCollectionBase shapes = shapesGetter( tabSheet );

        if( shapes.StartId == 0 )
        {
          AssignIndexes( shapeIdReserver, shapes );
          shapeIdReserver.AddAdditionalShapes( shapes.CollectionIndex, shapes.Count );
        }
      }
    }
    /// <summary>
    /// Allocates and assigns indexes for all shapes inside shape collection.
    /// </summary>
    /// <param name="shapeIdReserver">IdReserver that helps in the id generation process.</param>
    /// <param name="shapes">Shape collection to process.</param>
    private void AssignIndexes( IdReserver shapeIdReserver, ShapeCollectionBase shapes )
    {
      if( shapeIdReserver == null )
        throw new ArgumentNullException( "shapeIdReserver" );

      if( shapes == null )
        throw new ArgumentNullException( "shapes" );

      int iShapesCount = shapes.Count;
      int iStartId = shapeIdReserver.Allocate( iShapesCount + 1, shapes.CollectionIndex );
      int iLastId = iStartId + shapes.Count;

      shapes.StartId = iStartId;
      shapes.LastId = iLastId;

      iStartId++;
      for( int j = 0; j < iShapesCount; j++ )
      {
        if(( shapes[ j ] as ShapeImpl ).ShapeId==0)
        {
            (shapes[j] as ShapeImpl).ShapeId = iStartId + j;
        }
      }
    }
    /// <summary>
    /// Creates IdReserver based on the current shapes.
    /// </summary>
    /// <param name="shapesGetter">Delegate used to get necessary shapes collection from a TabSheet.</param>
    /// <returns>IdReserver filled with current shape id's data.</returns>
    private IdReserver FillReserverFromShapes( ShapesGetterMethod shapesGetter, out bool bChanged )
    {
      IdReserver result = new IdReserver();
      bChanged = false;

      //for( int i = 0, len = m_arrObjects.Count; i < len; i++ )
      foreach( ShapeCollectionBase shapes in EnumerateShapes( shapesGetter ) )
      {
        //ITabSheet tabSheet = m_arrObjects[ i ] as ITabSheet;
        //WorksheetImpl worksheet = tabSheet as WorksheetImpl;
        //ShapeCollectionBase shapes = shapesGetter( tabSheet );//( ShapesCollection )tabSheet.Shapes;

        if( shapes != null )
        {
          int iStartId = shapes.StartId;
          int iLastId = shapes.LastId;
          int iCollectionIndex = shapes.CollectionIndex;

          if( iStartId > 0 )
          {
            for( int j = 0, lenJ = shapes.Count; j < lenJ; j++ )
            {
              int iShapeId = ( shapes[ j ] as ShapeImpl ).ShapeId;

              if( iShapeId > 0 && !result.TryReserve( iShapeId, iShapeId, iCollectionIndex ) )
              {
                // TODO: add this situation handling.
                //throw new NotSupportedException();
                shapes.StartId = 0;
                shapes.LastId = 0;
              }
              else if( iShapeId <= 0 && shapes[ j ].ShapeType != ExcelShapeType.Unknown )
              {
                bChanged = true;
              }
            }
          }

          if( shapes != null && shapes.Count > 0 )
          {
            WorksheetImpl worksheet = shapes.Worksheet;

            if( worksheet != null && worksheet.InnerDVTable != null )
              result.AddAdditionalShapes( iCollectionIndex, worksheet.InnerDVTable.ShapesCount + 1 );

            result.AddAdditionalShapes( iCollectionIndex, shapes.Count );
          }
        }
      }

      return result;
    }
    /// <summary>
    /// Re-indexes shape collections if necessary.
    /// </summary>
    /// <param name="shapesGetter">Delegate used to get necessary shapes collection from a TabSheet.</param>
    /// <returns>True if there are shapes inside the workbook.</returns>
    private bool ReIndexShapeCollections( ShapesGetterMethod shapesGetter )
    {
      Dictionary<int, int> dictShapeCollections = new Dictionary<int, int>();
      int iMaxCollectionIndex = -1;

      // Find out max collection index.
      iMaxCollectionIndex = GetMaxCollectionIndex( shapesGetter );

      // Re-index if necessary.
      //for( int i = 0, len = m_arrObjects.Count; i < len; i++ )
      foreach( ShapeCollectionBase shapes in EnumerateShapes( shapesGetter ) )
      {
        //ITabSheet tabSheet = m_arrObjects[ i ] as ITabSheet;
        //ShapeCollectionBase shapes = shapesGetter( tabSheet );

        if( shapes != null && shapes.Count > 0 )
        {
          int iCollectionIndex = shapes.CollectionIndex;

          if( dictShapeCollections.ContainsKey( iCollectionIndex ) )
            shapes.CollectionIndex = iCollectionIndex = ++iMaxCollectionIndex;

          dictShapeCollections.Add( iCollectionIndex, iCollectionIndex );
        }
      }

      return iMaxCollectionIndex >= 0;
    }

    private int GetMaxCollectionIndex( ShapesGetterMethod shapesGetter )
    {
      int iMaxCollectionIndex = -1;

      //for( int i = 0, len = m_arrObjects.Count; i < len; i++ )
      //{
      //  ITabSheet tabSheet = m_arrObjects[ i ] as ITabSheet;
      //  ShapeCollectionBase shapes = shapesGetter( tabSheet );
      //  int iCollectionIndex = ( shapes != null ) ?
      //    shapes.CollectionIndex :
      //    -1;

      //  if( iCollectionIndex > iMaxCollectionIndex )
      //    iMaxCollectionIndex = iCollectionIndex;
      //}
      foreach( ShapeCollectionBase shapes in EnumerateShapes( shapesGetter ) )
      {
        int iCollectionIndex = ( shapes != null ) ?
          shapes.CollectionIndex :
          -1;

        if( iCollectionIndex > iMaxCollectionIndex )
          iMaxCollectionIndex = iCollectionIndex;
      }

      return iMaxCollectionIndex;
    }

    internal IEnumerable<ShapeCollectionBase> EnumerateShapes( ShapesGetterMethod shapesGetter )
    {
      for( int i = 0, len = m_arrObjects.Count; i < len; i++ )
      {
        ITabSheet tabSheet = m_arrObjects[ i ] as ITabSheet;

        ShapeCollectionBase shapes = shapesGetter( tabSheet );

        if( shapes != null && shapes.Count > 0 )
        {
          yield return shapes;
        }

        shapes = tabSheet.Shapes as ShapeCollectionBase;

        for( int j = 0, lenJ = shapes.Count; j < lenJ; j++ )
        {
          tabSheet = shapes[ j ] as ITabSheet;

          if( tabSheet != null )
          {
            ShapeCollectionBase shapesResult = shapesGetter( tabSheet );

            if( shapesResult != null && shapesResult.Count > 0 )
              yield return shapesResult;
          }
        }
      }
    }
    #endregion

    /// <summary>
    /// Returns sheet's shapes collection.
    /// </summary>
    /// <param name="sheet">TabSheet to get collection from.</param>
    /// <returns>Extracted collection.</returns>
    private ShapeCollectionBase GetWorksheetShapes( ITabSheet sheet )
    {
      return sheet.Shapes as ShapeCollectionBase;
    }
    /// <summary>
    /// Returns header/footer shapes collection.
    /// </summary>
    /// <param name="sheet">TabSheet to get collection from.</param>
    /// <returns>Extracted collection.</returns>
    private ShapeCollectionBase GetHeaderFooterShapes( ITabSheet sheet )
    {
      ChartShapeImpl chartShape = sheet as ChartShapeImpl;

      if( chartShape != null )
      {
        sheet = ( WorksheetBaseImpl )chartShape;
      }

      return ( ( WorksheetBaseImpl )sheet ).HeaderFooterShapes;
    }
    private void PrepareShapes()
    {
      //for( int i = 0, len = m_worksheets.Count; i < len; i++ )
      //{
      //  WorksheetImpl sheet = m_worksheets[ i ] as WorksheetImpl;
      //  TextBoxCollection arrTextBoxes = sheet.InnerTextBoxes;

      //  if( arrTextBoxes != null )
      //  {
      //    for( int j = 0, lenJ = arrTextBoxes.Count; j < lenJ; j++ )
      //    {
      //      TextBoxShapeImpl textBox = arrTextBoxes[ j ] as TextBoxShapeImpl;
      //      textBox.SerializeImage();
      //    }
      //  }
      //}
    }
#if !(SILVERLIGHT) && !(WINRT) && !(WP)
#if !ClientProfile
    /// <summary>
    /// Saves changes to the specified HttpResponse.
    /// </summary>
    /// <param name="fileName">Name of the file in HttpResponse.</param>
    /// <param name="response">HttpResponse to save in.</param>
    public void SaveAs( string fileName, HttpResponse response )
    {
      SaveAs( fileName, ExcelSaveType.SaveAsXLS, response );
    }
#endif
#endif
#if !(WINRT || WP)
    /// <summary>
    /// Save active WorkSheet using separator.
    /// </summary>
    /// <param name="fileName">Path to save.</param>
    /// <param name="separator">Current separator.</param>
    public void SaveAs( string fileName, string separator )
    {
      if( fileName == null )
        throw new ArgumentNullException( "Filename" );

      if( fileName.Length == 0 )
        throw new ArgumentException( "FileName cannot be empty." );

      if( separator == null || separator.Length == 0 )
        throw new ArgumentNullException( "separator" );

      if( m_ActiveSheet == null )
        throw new ArgumentNullException( "Active worksheet." );

      WorksheetImpl sheet = m_ActiveSheet as WorksheetImpl;

      if( sheet != null )
      {
        sheet.SaveAs( fileName, separator );
      }
      else
      {
        throw new ArgumentNullException( "ActiveSheet" );
      }
    }
#endif
    /// <summary>
    /// Saves active Worksheet using separator.
    /// </summary>
    /// <param name="stream">Stream to save int.</param>
    /// <param name="separator">Separator to use.</param>
    public void SaveAs( Stream stream, string separator )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      if( separator == null || separator.Length == 0 )
        throw new ArgumentNullException( "separator" );
      SaveAsInternal(stream, separator);
    }
   
    /// <summary>
    /// Saves active Worksheet using separator.
    /// </summary>
    /// <param name="stream">Stream to save int.</param>
    /// <param name="separator">Separator to use.</param>
    private void SaveAsInternal(Stream stream, string separator)
    {
        WorksheetImpl sheet = ActiveSheet as WorksheetImpl;

        if (sheet != null)
        {
            sheet.SaveAs(stream, separator);
        }
    }
#if !(SILVERLIGHT) && !(WINRT) && !(WP)
#if !ClientProfile
    /// <summary>
    /// Saves changes to the specified HttpResponse.
    /// </summary>
    /// <param name="fileName">Name of the file in HttpResponse.</param>
    /// <param name="response">HttpResponse to save in.</param>
    /// <param name="contentType">Content type to use.</param>
    public void SaveAs( string fileName, HttpResponse response, ExcelHttpContentType contentType )
    {
      SaveAs( fileName, ExcelSaveType.SaveAsXLS, response, contentType );
    }
    /// <summary>
    /// Saves changes to the specified HttpResponse.
    /// </summary>
    /// <param name="fileName">Name of the file in HttpResponse.</param>
    /// <param name="saveType">Type of the Excel file.</param>
    /// <param name="response">HttpResponse that will receive workbook's data.</param>
    public void SaveAs( string fileName, ExcelSaveType saveType, HttpResponse response )
    {
      SaveAs( fileName, saveType, response, ExcelDownloadType.PromptDialog );
    }
    /// <summary>
    /// Saves changes to the specified HttpResponse.
    /// </summary>
    /// <param name="fileName">Name of the file in HttpResponse.</param>
    /// <param name="saveType">Type of the Excel file.</param>
    /// <param name="response">HttpResponse that will receive workbook's data.</param>
    /// <param name="contentType">Content type to use.</param>
    public void SaveAs( string fileName, ExcelSaveType saveType, HttpResponse response,
      ExcelHttpContentType contentType )
    {
      SaveAs( fileName, saveType, response, ExcelDownloadType.PromptDialog, contentType );
    }
    /// <summary>
    /// Saves changes to the specified HttpResponse.
    /// </summary>
    /// <param name="fileName">Name of the file in HttpResponse.</param>
    /// <param name="response">HttpResponse that will receive workbook's data.</param>
    /// <param name="downloadType">Download type.</param>
    public void SaveAs( string fileName, HttpResponse response
      , ExcelDownloadType downloadType )
    {
      SaveAs( fileName, ExcelSaveType.SaveAsXLS, response, downloadType );
    }
    /// <summary>
    /// Saves changes to the specified HttpResponse.
    /// </summary>
    /// <param name="fileName">Name of the file in HttpResponse.</param>
    /// <param name="response">HttpResponse that will receive workbook's data.</param>
    /// <param name="downloadType">Download type.</param>
    /// <param name="contentType">Content type to use.</param>
    public void SaveAs( string fileName, HttpResponse response
      , ExcelDownloadType downloadType, ExcelHttpContentType contentType )
    {
      SaveAs( fileName, ExcelSaveType.SaveAsXLS, response, downloadType, contentType );
    }
    /// <summary>
    /// Saves changes to the specified HttpResponse.
    /// </summary>
    /// <param name="fileName">Name of the file in HttpResponse.</param>
    /// <param name="saveType">Type of the Excel file.</param>
    /// <param name="response">HttpResponse that will receive workbook's data.</param>
    /// <param name="downloadType">Download type.</param>
    public void SaveAs( string fileName, ExcelSaveType saveType, HttpResponse response
      , ExcelDownloadType downloadType )
    {
      ExcelHttpContentType contentType = ( Version == ExcelVersion.Excel97to2003 )?
        ExcelHttpContentType.Excel97 :
        ExcelHttpContentType.Excel2000;

      SaveAs( fileName, saveType, response, downloadType, contentType );
    }
    /// <summary>
    /// Saves changes to the specified HttpResponse.
    /// </summary>
    /// <param name="fileName">Name of the file in HttpResponse.</param>
    /// <param name="separator">string separator.</param>
    /// <param name="response">HttpResponse that will receive workbook's data.</param>
    /// <param name="downloadType">Download type.</param>
    /// <param name="contentType">Http content type.</param>
    public void SaveAs( string fileName, string separator, HttpResponse response,
      ExcelDownloadType downloadType, ExcelHttpContentType contentType )
    {
      PrepareResponse( fileName, response, downloadType, contentType );
      SaveAs( response.OutputStream, separator );
      response.End();
    }

    /// <summary>
    /// Saves changes to the specified HttpResponse.
    /// </summary>
    /// <param name="fileName">Name of the file in HttpResponse.</param>
    /// <param name="saveType">Type of the Excel file.</param>
    /// <param name="response">HttpResponse that will receive workbook's data.</param>
    /// <param name="downloadType">Download type.</param>
    /// <param name="contentType">Content type to use.</param>
    public void SaveAs( string fileName, ExcelSaveType saveType, HttpResponse response
      , ExcelDownloadType downloadType, ExcelHttpContentType contentType )
    {
      PrepareResponse( fileName, response, downloadType, contentType );
      SaveAs( response.OutputStream, saveType );

      if( Version !=ExcelVersion.Excel97to2003)
      {
        // Zip archive cannot contain any additional information, so we have to close response
        // this can cause ThreadAbortedException if placed inside try...catch block.
        response.End();
      }
      else
      {
        // This is done because structured storage is not sensitive to information written after
        // actual file, so there is no need in ending response, and we just call Flush to enable
        // calling SaveAs method from try...catch block without exception.
#if AllowUnsafeCode
        response.Flush();
#else
        response.End();
#endif
      }
      //HttpContext.Current.ApplicationInstance.CompleteRequest();
      //response.End();
    }
#endif
#if MVC
    /// <summary>
    /// Save as ActionResult
    /// </summary>
    /// <param name="filename">Name of the file.</param>
    /// <param name="response">The Response.</param>
    /// <param name="contentType">Http content type.</param>
    /// <returns></returns>
    public ExcelResult SaveAsActionResult(string filename, HttpResponse response, ExcelHttpContentType contentType)
    {
        return new ExcelResult(this, filename, response, ExcelDownloadType.PromptDialog, contentType);
    }
    /// <summary>
    /// Save as ActionResult
    /// </summary>
    /// <param name="filename">Name of the file.</param>
    /// <param name="response">The Response.</param>
    /// <param name="DownloadType">Download type.</param>
    /// <param name="contentType">Http content type.</param>
    /// <returns></returns>
    public ExcelResult SaveAsActionResult(string filename, HttpResponse response, ExcelDownloadType DownloadType, ExcelHttpContentType contentType)
    {
        return new ExcelResult(this, filename, response, DownloadType, contentType);
    }
    /// <summary>
    /// Save as ActionResult
    /// </summary>
    /// <param name="filename">Name of the file.</param>
    /// <param name="saveType">Type of the Excel file.</param>
    /// <param name="response">The Response.</param>
    /// <param name="DownloadType">Download type.</param>
    /// <param name="contentType">Http content type.</param>
    /// <returns></returns>
    public ExcelResult SaveAsActionResult(string filename, ExcelSaveType saveType, HttpResponse response, ExcelDownloadType DownloadType, ExcelHttpContentType contentType)
    {
        return new ExcelResult(this, filename, response, DownloadType, contentType);
    }
    /// <summary>
    /// Save as ActionResult
    /// </summary>
    /// <param name="filename">Name of the file.</param>
    /// <param name="response">The Response.</param>
    /// <param name="DownloadType">Download type.</param>
    /// <returns></returns>
    public ExcelResult SaveAsActionResult(string filename, HttpResponse response, ExcelDownloadType DownloadType)
    {
        return new ExcelResult(this, filename, response, DownloadType);
    }
    /// <summary>
    /// Save as ActionResult
    /// </summary>
    /// <param name="filename">Name of the file.</param>
    /// <param name="response">The Response.</param>
    /// <param name="DownloadType">Download type.</param>
    /// <param name="contentType">Http content type.</param>
    /// <returns></returns>
    public ExcelResult SaveAsActionResult(string filename, string separator, HttpResponse response, ExcelDownloadType DownloadType, ExcelHttpContentType contentType)
    {
        return new ExcelResult(this, filename, separator, response, DownloadType, contentType);
    }
#endif
#if !ClientProfile
    /// <summary>
    /// Prepares response before saving.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="response"></param>
    /// <param name="downloadType"></param>
    /// <param name="contentType"></param>
    private void PrepareResponse( string fileName, HttpResponse response
      , ExcelDownloadType downloadType, ExcelHttpContentType contentType )
    {
      if( fileName == null )
        throw new ArgumentNullException( "fileName" );

      if( fileName.Length == 0 )
        throw new ArgumentOutOfRangeException( "fileName" );

      if( response == null )
        throw new ArgumentNullException( "response" );

      string strOpenType = string.Empty;

      switch( downloadType )
      {
        case ExcelDownloadType.Open:
          strOpenType = DEF_RESPONSE_OPEN;
          break;

        case ExcelDownloadType.PromptDialog:
          strOpenType = DEF_RESPONSE_DIALOG;
          break;

        default:
          throw new ArgumentOutOfRangeException( "downloadType" );
      }

      string strContentType = GetContentTypeString( contentType );

      fileName = Path.GetFileName( fileName );

      response.Clear();
      //response.ClearHeaders();
      response.ContentType = strContentType;

      response.AddHeader( "Content-Disposition", string.Format( "{0}; filename={1};"
        , strOpenType, fileName ) );
    }
#endif
#endif
    /// <summary>
    /// Saves workbook in xml format.
    /// </summary>
    /// <param name="writer">XmlWriter to save into.</param>
    /// <param name="saveType">Xml save type.</param>
    public void SaveAsXmlInternal( XmlWriter writer, ExcelXmlSaveType saveType )
    {
      if( writer == null )
        throw new ArgumentNullException( "writer" );

      m_bSaving = true;
      IXmlSerializator serializator = XmlSerializatorFactory.GetSerializator( saveType );
      serializator.Serialize( writer, this );
      m_bSaving = false;
    }

    /// <summary>
    /// Saves workbook in xml format.
    /// </summary>
    /// <param name="writer">XmlWriter to save into.</param>
    /// <param name="saveType">Xml save type.</param>
    public void SaveAsXml(XmlWriter writer, ExcelXmlSaveType saveType)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");
        SaveAsXmlInternal(writer, saveType);
    }
#if !(WINRT || WP)
    /// <summary>
    /// Saves workbook in xml format.
    /// </summary>
    /// <param name="strFileName">File name to save into.</param>
    /// <param name="saveType">Xml save type.</param>
    public void SaveAsXml( string strFileName, ExcelXmlSaveType saveType )
    {
      if( strFileName == null )
        throw new ArgumentNullException( "strFileName" );

      if( strFileName.Length == 0 )
        throw new ArgumentException( "strFileName - string cannot be empty." );

      m_bSaving = true;
#if !SILVERLIGHT && !WINRT && !WP
      Encoding encoding = new UTF8Encoding( false );
      XmlTextWriter writer = new XmlTextWriter( strFileName, encoding );
      writer.Formatting = Formatting.Indented;
#else
      XmlWriter writer = XmlWriter.Create( new StreamWriter( strFileName ) );
#endif
      SaveAsXml( writer, saveType );
      writer.Close();
      m_bSaving = false;
    }
    /// <summary>
    /// Saves workbook in xml format.
    /// </summary>
    /// <param name="stream">Stream to save into.</param>
    /// <param name="saveType">Xml save type.</param>
    public void SaveAsXml(Stream stream, ExcelXmlSaveType saveType)
    {
        if (stream == null)
            throw new ArgumentNullException("stream");

        m_bSaving = true;
#if !SILVERLIGHT && !WINRT && !WP
      Encoding encoding = new UTF8Encoding( false );
      XmlTextWriter writer = new XmlTextWriter( stream, encoding );
      writer.Formatting = Formatting.Indented;
#else
        XmlWriter writer = XmlWriter.Create(stream);
#endif
        SaveAsXml(writer, saveType);
        writer.Flush();
        m_bSaving = false;
    }    
#endif

#if ( WINRT || WP )
#if (!WP)
    /// <summary>
    /// Saves workbook in xml format.
    /// </summary>
    /// <param name="storageFile">storage File to save into.</param>
    /// <param name="saveType">Xml save type.</param>
    public async void SaveAsXml(StorageFile storageFile, ExcelXmlSaveType saveType)
    {
        if (storageFile == null)
            throw new ArgumentNullException("storageFile");

        Stream stream = await storageFile.OpenStreamForWriteAsync();

        m_bSaving = true;
#if !SILVERLIGHT && !WINRT && !WP
      Encoding encoding = new UTF8Encoding( false );
      XmlTextWriter writer = new XmlTextWriter( stream, encoding );
      writer.Formatting = Formatting.Indented;
#else
        XmlWriter writer = XmlWriter.Create(stream);
#endif
        SaveAsXml(writer, saveType);
        writer.Flush();
        m_bSaving = false;
        stream.Flush();
        stream.Dispose();
    }
#endif
    /// <summary>
    /// Saves active Worksheet.
    /// </summary>
    /// <param name="stream">Stream to save int.</param>
    /// <param name="separator">Separator to use.</param>
    public Task<bool> SaveAsAsync(Stream stream, string separator)
    {
        if (stream == null)
            throw new ArgumentNullException("stream");

        if (separator == null || separator.Length == 0)
            throw new ArgumentNullException("separator");
        return SaveAsAsyncInternal(stream, separator);
    }
    /// <summary>
    /// Saves active Worksheet.
    /// </summary>
    /// <param name="stream">Stream to save int.</param>
    /// <param name="separator">Separator to use.</param>
    private async Task<bool> SaveAsAsyncInternal(Stream stream, string separator)
    {
        TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
        await Task.Run(() =>
        {
            try
            {
                SaveAsInternal(stream, separator);
                taskCompletionSource.SetResult(true);
            }
            catch (Exception exception)
            {
                taskCompletionSource.SetException(exception);
            }
        });
        return await taskCompletionSource.Task;

    }

    /// <summary>
    /// Saves workbook in xml format.
    /// </summary>
    /// <param name="writer">XmlWriter to save into.</param>
    /// <param name="saveType">Xml save type.</param>
    /// <returns></returns>
    public Task<bool> SaveAsXmlAsync(XmlWriter writer, ExcelXmlSaveType saveType)
    {
        if (writer == null)
            throw new ArgumentNullException("writer");
        return SaveAsXmlAsyncInternal(writer, saveType);
    }

    /// <summary>
    /// Saves workbook in xml format.
    /// </summary>
    /// <param name="writer">XmlWriter to save into.</param>
    /// <param name="saveType">Xml save type.</param>
    /// <returns></returns>
    private async Task<bool> SaveAsXmlAsyncInternal(XmlWriter writer, ExcelXmlSaveType saveType)
    {
        TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
        await Task.Run(() =>
        {
            try
            {
                SaveAsXmlInternal(writer, saveType);
                taskCompletionSource.SetResult(true);
            }
            catch (Exception exception)
            {
                taskCompletionSource.SetException(exception);
            }
        });
        return await taskCompletionSource.Task;
    }

    /// <summary>
    /// Saves changes to the specified stream.
    /// </summary>
    /// <param name="stream">Stream that will receive workbook data.</param>
    /// <param name="saveType">Save type.</param>
    public Task<bool> SaveAsAsync(Stream stream, ExcelSaveType saveType)
    {
        if (stream == null)
            throw new ArgumentNullException("stream");
        return SaveAsAsyncInternal(stream, saveType);
    }

    /// <summary>
    /// Saves changes to the specified stream.
    /// </summary>
    /// <param name="stream">Stream that will receive workbook data.</param>
    /// <param name="saveType">Save type.</param>
    /// <returns></returns>
    private async Task<bool> SaveAsAsyncInternal(Stream stream, ExcelSaveType saveType)
    {
        TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
        await Task.Run(() =>
        {
            try
            {
                SaveAsInternal(stream, saveType);
                taskCompletionSource.SetResult(true);
            }
            catch (Exception exception)
            {
                taskCompletionSource.SetException(exception);
            }
        });
        return await taskCompletionSource.Task;
    }
    /// <summary>
    /// Saves changes to the specified stream.
    /// </summary>
    /// <param name="stream">Stream that will receive workbook data.</param>
    public Task<bool> SaveAsAsync(Stream stream)
    {
        if (stream == null)
            throw new ArgumentNullException("stream");

        return SaveAsAsync(stream, ExcelSaveType.SaveAsXLS);
    }
    /// <summary>
    /// Saves workbook in xml format.
    /// </summary>
    /// <param name="stream">Stream to save into.</param>
    /// <param name="saveType">Xml save type.</param>
    public Task<bool> SaveAsXmlAsync(Stream stream, ExcelXmlSaveType saveType)
    {
        if (stream == null)
            throw new ArgumentNullException("stream");
        return SaveAsXmlAsyncInternal(stream, saveType);
    }
    /// <summary>
    /// Saves workbook in xml format.
    /// </summary>
    /// <param name="stream">Stream to save into.</param>
    /// <param name="saveType">Xml save type.</param>
    private async Task<bool> SaveAsXmlAsyncInternal(Stream stream, ExcelXmlSaveType saveType)
    {
        TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
        await Task.Run(async () =>
        {
            try
            {
                m_bSaving = true;
                XmlWriter writer = XmlWriter.Create(stream);
                bool saveResult = await SaveAsXmlAsync(writer, saveType);

                writer.Flush();
                m_bSaving = false;
                taskCompletionSource.SetResult(saveResult);
            }
            catch (Exception exception)
            {
                taskCompletionSource.SetException(exception);
            }
        });
        return await taskCompletionSource.Task;
    }

    /// <summary>
    /// Saves active Worksheet.
    /// </summary>
    /// <param name="storageFile">Storage file to save in it.</param>
    /// <param name="separator">Separator to use.</param>
    public async Task<bool> SaveAsAsync(StorageFile storageFile, string separator)
    {
        if (storageFile == null)
            throw new ArgumentNullException("storageFile");

        if (separator == null || separator.Length == 0)
            throw new ArgumentNullException("separator");

        Stream stream = await storageFile.OpenStreamForWriteAsync();
        stream.Position = 0;
        stream.SetLength(0);
        bool task=await SaveAsAsyncInternal(stream, separator);
        stream.Flush();
        stream.Dispose();
        return task;
    }

    /// <summary>
    /// Saves changes to the specified storage file.
    /// </summary>
    /// <param name="storageFile">Storage File that will receive workbook data.</param>
    /// <param name="saveType">Save type.</param>
    public async Task<bool> SaveAsAsync(StorageFile storageFile, ExcelSaveType saveType)
    {
        if (storageFile == null)
            throw new ArgumentNullException("storageFile");
        Stream stream = await storageFile.OpenStreamForWriteAsync();
        stream.Position = 0;
        stream.SetLength(0);
        bool task= await SaveAsAsyncInternal(stream, saveType);
        stream.Flush();
        stream.Dispose();
        return task;
    }

    /// <summary>
    /// Saves changes to the specified storage.
    /// </summary>
    /// <param name="storageFile">Storage that will receive workbook data.</param>
    public async Task<bool> SaveAsAsync(StorageFile storageFile)
    {
        if (storageFile == null)
            throw new ArgumentNullException("storageFile");
        
        Stream stream = await storageFile.OpenStreamForWriteAsync();
        stream.Position = 0;
        stream.SetLength(0);
        bool task = await SaveAsAsync(stream, ExcelSaveType.SaveAsXLS);
        stream.Flush();
        stream.Dispose();
        return task;
    }

    /// <summary>
    /// Saves workbook in xml format.
    /// </summary>
    /// <param name="storageFile">Storage file to save into.</param>
    /// <param name="saveType">Xml save type.</param>
    public async Task<bool> SaveAsXmlAsync(StorageFile storageFile, ExcelXmlSaveType saveType)
    {
        if (storageFile == null)
            throw new ArgumentNullException("storageFile");

        Stream stream = await storageFile.OpenStreamForWriteAsync();
        stream.Position = 0;
        stream.SetLength(0);
        bool task= await SaveAsXmlAsyncInternal(stream, saveType);
        stream.Flush();
        stream.Dispose();
        return task;
    }

#endif
    /// <summary>
    /// Returns string that corresponds to contentType.
    /// </summary>
    /// <param name="contentType">Content type for browser.</param>
    /// <returns>String that corresponds to contentType.</returns>
    private string GetContentTypeString( ExcelHttpContentType contentType )
    {
      switch( contentType )
      {
        case ExcelHttpContentType.Excel97:
          return DEF_EXCEL97_CONTENT_TYPE;

        case ExcelHttpContentType.Excel2000:
          return DEF_EXCEL2000_CONTENT_TYPE;

        case ExcelHttpContentType.Excel2007:
        case ExcelHttpContentType.Excel2010:
        case ExcelHttpContentType.Excel2013:
          return DEF_EXCEL2007_CONTENT_TYPE;

        case ExcelHttpContentType.CSV:
          return DEF_CSV_CONTENT_TYPE;

        default:
          throw new ArgumentOutOfRangeException( "contentType" );
      }
    }
    /// <summary>
    /// Saves changes to the specified stream.
    /// </summary>
    /// <param name="stream">Stream that will receive workbook data.</param>
    public void SaveAs( Stream stream )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );

      SaveAs( stream, ExcelSaveType.SaveAsXLS );
    }
    /// <summary>
    /// Saves changes to the specified stream.
    /// </summary>
    /// <param name="stream">Stream that will receive workbook data.</param>
    /// <param name="saveType">Save type.</param>
    public void SaveAs( Stream stream, ExcelSaveType saveType )
    {
      if( stream == null )
        throw new ArgumentNullException( "stream" );
       SaveAsInternal(stream, saveType);
    }
    private void SaveAsInternal(Stream stream, ExcelSaveType saveType)
    {

        //FontImpl normalFont = ( Styles[ "Normal" ].Font as FontWrapper ).Wrapped;
        //int firstSize = ( int )Math.Round( normalFont.MeasureString( WorksheetImpl.DEF_STANDARD_CHAR.ToString() ).Width );
        //int secondSize = ( int )Math.Round( normalFont.MeasureCharacter( WorksheetImpl.DEF_STANDARD_CHAR ).Width );

        m_bSaving = true;
        IdReserver shapeIds = PrepareShapes(GetWorksheetShapes);
        IWorkbookSerializator serializator = CreateSerializator(m_version, shapeIds);
        serializator.Serialize(stream, this, saveType);
        stream.Flush();
        m_bSaving = false;
        m_bSaved = true;
        RaiseSavedEvent();
    }

    /// <summary>
    /// Gets names of all worksheets.
    /// </summary>
    /// <returns>Names of all worksheets.</returns>
    private string[] GetDocParts()
    {
      string[] arrDocParts = new string[ Worksheets.Count ];
          
      IWorksheets sheets = Worksheets;
      for( int i = 0, len = sheets.Count; i < len; i++ )
      {
        arrDocParts[ i ] = sheets[ i ].Name;
      }

      return arrDocParts;
    }
    /// <summary>
    /// Returns array odd elements (sheets, charts, etc. ) of which 
    /// are workbook's part name and even number of such elements 
    /// in the workbook.
    /// </summary>
    /// <returns>
    /// Array odd elements (sheets, charts, etc. ) of which 
    /// are workbook's part name and even number of such elements 
    /// in the workbook.
    /// </returns>
    private object[] GetHeadingPairs()
    {
      List<object> result = new List<object>();
      Dictionary<ExcelSheetType, int> hashPairs = GetHashHeadingPairs();

      foreach( KeyValuePair<ExcelSheetType, int> pair in hashPairs )
      {
        ExcelSheetType key = pair.Key;
        int value = pair.Value;
        
        if( SheetTypeToName.ContainsKey( key ) )
        {
          result.Add( SheetTypeToName[ key ] );
          result.Add( value );
        }
      }

      return result.ToArray();
    }
    /// <summary>
    /// Returns Dictionary ExcelSheetType - to - number of such
    /// elements in the workbook.
    /// </summary>
    /// <returns>
    /// Dictionary ExcelSheetType - to - number of such
    /// elements in the workbook.
    /// </returns>
    private Dictionary<ExcelSheetType, int> GetHashHeadingPairs()
    {
      Dictionary<ExcelSheetType, int> hashPartsCount = new Dictionary<ExcelSheetType, int>();

      IWorksheets sheets = Worksheets;
      for( int i = 0, len = sheets.Count; i < len; i++ )
      {
        // Use 'as' to increase performance.
        WorksheetImpl sheet = sheets[ i ] as WorksheetImpl;
        ExcelSheetType sheetType = sheet.Type;
        
        if( hashPartsCount.ContainsKey( sheetType ) )
        {
          int temp = hashPartsCount[ sheetType ];
          temp++;
          hashPartsCount[ sheetType ] = temp;
        }
        else
        {
          hashPartsCount.Add( sheetType, 1 );
        }
      }

      return hashPartsCount;
    }

    /// <summary>
    /// Set user color for specified element in Color table.
    /// </summary>
    /// <param name="index">Index of Color in array.</param>
    /// <param name="color">New color which must be set.</param>
    public void SetPaletteColor( int index, Color color )
    {
      if( !m_bLoading && index < DEF_FIRST_USER_COLOR || index >= m_colors.Count )
        throw new ArgumentOutOfRangeException( "index", "Index cannot be less than 0 and larger than Palette colors array size." );

      if( m_colors[ index ] != color )
      {
        m_bOwnPalette = true;
        m_colors[ index ] = Color.FromArgb( color.A, color.R, color.G, color.B );
      }
    }
      /// <summary>
      /// Copies palette colors to workbook/
      /// </summary>
      /// <param name="destinationWorkbook">Workbook to copy palette into.</param>
    public void CopyPaletteColorTo(WorkbookImpl destinationWorkbook)
    {
        destinationWorkbook.InnerPalette.Clear();
        destinationWorkbook.InnerPalette.AddRange( InnerPalette );
        destinationWorkbook.m_bOwnPalette = m_bOwnPalette;
    }
    /// <summary>
    /// Recover palette to default values.
    /// </summary>
    public void ResetPalette()
    {
      m_bOwnPalette = false;
      m_colors = new List<Color>( DEF_PALETTE );
    }

    /// <summary>
    /// Method return Color object from workbook palette by its index.
    /// </summary>
    /// <param name="color">Index from palette array.</param>
    /// <returns>RGB Color.</returns>
    public Color GetPaletteColor( ExcelKnownColors color )
    {
      int iColor = ( int )color;
      Color result;

      if( iColor >= FirstChartColor && iColor <= LastChartColor )
      {
        result = m_chartColors[ iColor - FirstChartColor ];
      }
      else if( iColor == ShapeFillImpl.DEF_COMMENT_COLOR_INDEX )
      {
        result = ShapeFillImpl.DEF_COMENT_PARSE_COLOR;
      }
      else if (iColor == 32767 && m_colors.Count > 0)
          result = m_colors[0];
      else
      {
        iColor %= m_colors.Count;
        result = m_colors[ iColor ];
      }

      return result;
    }

    /// <summary>
    /// Gets the nearest color to the specified Color structure
    /// from Workbook palette.
    /// </summary>
    /// <param name="color">Color to look for.</param>
    /// <returns>Color index from workbook palette.</returns>
    public ExcelKnownColors GetNearestColor( Color color )
    {
      return GetNearestColor( color, 0 );
    }
    /// <summary>
    /// Gets the nearest color to the specified Color structure
    /// from Workbook palette.
    /// </summary>
    /// <param name="color">Color to look for.</param>
    /// <param name="iStartIndex">Start index.</param>
    /// <returns>Color index from workbook palette.</returns>
    public ExcelKnownColors GetNearestColor( Color color, int iStartIndex )
    {
      if( iStartIndex < 0 || iStartIndex > m_colors.Count )
        throw new ArgumentOutOfRangeException( "iStartIndex" );

      int minIndex = iStartIndex;
      double minDist = ColorDistance( m_colors[ iStartIndex ], color );
      double dist2;
      
      for( int i = iStartIndex + 1; i < m_colors.Count; i++ )
      {
        dist2 = ColorDistance( m_colors[ i ], color );
        
        if( dist2 < minDist )
        {
          minDist = dist2;
          minIndex = i;

          if( dist2 == 0 ) break;
        }
      }

      return ( ExcelKnownColors )minIndex;
    }
    /// <summary>
    /// Gets the nearest color to the specified by red, green, and blue 
    /// values color from Workbook palette.
    /// </summary>
    /// <param name="r">Red component of the color.</param>
    /// <param name="g">Green component of the color.</param>
    /// <param name="b">Blue component of the color.</param>
    /// <returns>Color index from workbook palette.</returns>
    public ExcelKnownColors GetNearestColor( int r, int g, int b )
    {
      Color color = Color.FromArgb( 255, ( byte )r, ( byte )g, ( byte )b );
      int minIndex = 0;
      double minDist = ColorDistance( m_colors[ 0 ], color );
      double dist2;
      
      for( int i = 1; i < m_colors.Count; i++ )
      {
        dist2 = ColorDistance( m_colors[ i ], color );
        
        if( dist2 < minDist )
        {
          minDist = dist2;
          minIndex = i;
        }
      }

      return ( ExcelKnownColors )minIndex;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public ExcelKnownColors SetColorOrGetNearest( Color color )
    {
      ExcelKnownColors result = GetNearestColor( color );
      bool bEqual = isEqualColor= CompareColors( m_colors[ ( int )result ], color );
      
      if( !bEqual && m_iFirstUnusedColor < m_colors.Count )
      {
        SetPaletteColor( m_iFirstUnusedColor, color );

        ExcelKnownColors colorIndex = ( ExcelKnownColors )m_iFirstUnusedColor;

        m_iFirstUnusedColor++;

        return colorIndex;
      }

      return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public ExcelKnownColors SetColorOrGetNearest( int r, int g, int b )
    {
      Color color = Color.FromArgb( 255, ( byte )r, ( byte )g, ( byte )b );
      return SetColorOrGetNearest( color );
    }
    /// <summary>
    /// Replaces specified string by specified value.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValue">New value for the range with specified string.</param>
    public void Replace( string oldValue, string newValue )
    {
      for( int i = 0, len = m_worksheets.Count; i < len; i++ )
      {
        m_worksheets[ i ].Replace( oldValue, newValue );
      }
    }
    /// <summary>
    /// Replaces specified string by specified value.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValue">New value for the range with specified string.</param>
    public void Replace( string oldValue, DateTime newValue )
    {
      for( int i = 0, len = m_worksheets.Count; i < len; i++ )
      {
        m_worksheets[ i ].Replace( oldValue, newValue );
      }
    }
    /// <summary>
    /// Replaces specified string by specified value.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValue">New value for the range with specified string.</param>
    public void Replace( string oldValue, double newValue )
    {
      for( int i = 0, len = m_worksheets.Count; i < len; i++ )
      {
        m_worksheets[ i ].Replace( oldValue, newValue );
      }
    }

    /// <summary>
    /// Replaces specified string by data from array.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValues">Array of new values.</param>
    /// <param name="isVertical">
    /// Indicates whether array should be inserted vertically.
    /// </param>
    public void Replace( string oldValue, string[] newValues, bool isVertical )
    {
      for( int i = 0, len = m_worksheets.Count; i < len; i++ )
      {
        m_worksheets[ i ].Replace( oldValue, newValues, isVertical );
      }
    }
    /// <summary>
    /// Replaces specified string by data from array.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValues">Array of new values.</param>
    /// <param name="isVertical">
    /// Indicates whether array should be inserted vertically.
    /// </param>
    public void Replace( string oldValue, int[] newValues, bool isVertical )
    {
      for( int i = 0, len = m_worksheets.Count; i < len; i++ )
      {
        m_worksheets[ i ].Replace( oldValue, newValues, isVertical );
      }
    }
    /// <summary>
    /// Replaces specified string by data from array.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValues">Array of new values.</param>
    /// <param name="isVertical">
    /// Indicates whether array should be inserted vertically.
    /// </param>
    public void Replace( string oldValue, double[] newValues, bool isVertical )
    {
      for( int i = 0, len = m_worksheets.Count; i < len; i++ )
      {
        m_worksheets[ i ].Replace( oldValue, newValues, isVertical );
      }
    }
#if !(SILVERLIGHT) && !(WINRT) && !(WP)
    /// <summary>
    /// Replaces specified string by data table values.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValues">Data table with new data.</param>
    /// <param name="isFieldNamesShown">Indicates whether field name must be shown.</param>
    public void Replace( string oldValue, DataTable newValues, bool isFieldNamesShown )
    {
      for( int i = 0, len = m_worksheets.Count; i < len; i++ )
      {
        m_worksheets[ i ].Replace( oldValue, newValues, isFieldNamesShown );
      }
    }
    /// <summary>
    /// Replaces specified string by data column values.
    /// </summary>
    /// <param name="oldValue">String value to replace.</param>
    /// <param name="newValues">Data table with new data.</param>
    /// <param name="isFieldNamesShown">Indicates whether field name must be shown.</param>
    public void Replace( string oldValue, DataColumn newValues, bool isFieldNamesShown )
    {
      for( int i = 0, len = m_worksheets.Count; i < len; i++ )
      {
        m_worksheets[ i ].Replace( oldValue, newValues, isFieldNamesShown );
      }
    }
#endif
    /// <summary>
    /// Method to create a font object and register it in the workbook.
    /// </summary>]
    /// <returns>Newly created font.</returns>
    public IFont CreateFont()
    {
      FontImpl font = ( FontImpl )AppImplementation.CreateFont( this );
      //font.Index = m_fonts.Count;
      //m_fonts.Add( font );
      //m_fonts.ForceAdd( font );
      //return new FontWrapper( Application, this, font, false, false );
      return new FontWrapper( font, false, false );
    }
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Method creates a font object based on native font and register it in the workbook.
    /// </summary>]
    /// <param name="nativeFont">Native font to get settings from.</param>
    /// <returns>Newly created font.</returns>
    public IFont CreateFont( Font nativeFont )
    {
      FontImpl font = ( FontImpl )AppImplementation.CreateFont( this, nativeFont );
      return new FontWrapper( font, false, false );
    }
#endif
    /// <summary>
    /// Adds font into collection.
    /// </summary>
    /// <param name="fontToAdd">Font to add.</param>
    /// <returns>
    /// Current font with correct font index, or same font from the collection if was added before.
    /// </returns>
    public IFont AddFont( IFont fontToAdd )
    {
      bool bWrapper = fontToAdd is FontWrapper;
      FontImpl font;
      FontWrapper wrapper = null;

      if( bWrapper )
      {
        wrapper = fontToAdd as FontWrapper;

        if( wrapper == null )
          throw new ArgumentNullException( "fontToAdd" );

        font = wrapper.Wrapped;
      }
      else
      {
        font = fontToAdd as FontImpl;
      }

      //font.Index = m_fonts.Count;
      font = m_fonts.Add( font ) as FontImpl;

      if( bWrapper )
      {
        wrapper.Wrapped = font;
        wrapper.IsReadOnly = true;

        return wrapper;
      }

      return font;
    }
    /// <summary>
    /// Method that creates font object based on another font object
    /// and registers it in the workbook.
    /// </summary>
    /// <param name="baseFont">Base font for the new one.</param>
    /// <returns>Newly created font.</returns>
    public IFont CreateFont( IFont baseFont )
    {
      return CreateFont( baseFont, true );
    }
    /// <summary>
    /// Method that creates font object based on another font object
    /// and registers it in the workbook.
    /// </summary>
    /// <param name="baseFont">Base font for the new one.</param>
    /// <param name="bAddToCollection">Indicates whether font should be added to the collection.</param>
    /// <returns>Newly created font.</returns>
    public IFont CreateFont( IFont baseFont, bool bAddToCollection )
    {
      IFont newFont = ( baseFont != null ) ?
        AppImplementation.CreateFont( baseFont ) :
        AppImplementation.CreateFont( this );
      
      if( bAddToCollection )
      {
        (( FontImpl )newFont).Index = m_fonts.Count;
        m_fonts.Add( newFont );
      }

      return newFont;
    }
    /// <summary>
    /// This method searches for the first cell with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( string findValue, ExcelFindType flags )
    {
        return FindFirst(findValue, flags, ExcelFindOptions.None);
    }
    /// <summary>
    /// This method searches for the first cell that starts with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>
    /// First found cell, or Null if value was not found.
    /// </returns>
    public IRange FindStringStartsWith(string findValue, ExcelFindType flags)
    {
        return FindStringStartsWith(findValue, flags, false);
    }
    /// <summary>
    /// This method searches for the first cell that starts with specified string value which igonres the case.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <param name="ignoreCase">true to ignore case wen comparing this string to the value;otherwise,false</param>
    /// <returns>
    /// First found cell, or Null if value was not found.
    /// </returns>
    public IRange FindStringStartsWith(string findValue, ExcelFindType flags, bool ignoreCase)
    {
        m_isStartsOrEndsWith = true;
        ExcelFindOptions findOptions = ignoreCase ?
            ExcelFindOptions.None :
            ExcelFindOptions.MatchCase;
        return FindFirst(findValue, flags, findOptions);
    }
    /// <summary>
    /// This method searches for the first cell that ends  with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>
    /// First found cell, or Null if value was not found.
    /// </returns>
    public IRange FindStringEndsWith(string findValue, ExcelFindType flags)
    {
        return FindStringEndsWith(findValue, flags, false);
    }
    /// <summary>
    /// This method searches for the first cell that ends with specified string value which igonres the case.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <param name="ignoreCase">true to ignore case wen comparing this string to the value;otherwise,false</param>
    /// <returns>
    /// First found cell, or Null if value was not found.
    /// </returns>
    public IRange FindStringEndsWith(string findValue, ExcelFindType flags, bool ignoreCase)
    {
        m_isStartsOrEndsWith = false;
        ExcelFindOptions findOptions = ignoreCase ?
            ExcelFindOptions.None :
            ExcelFindOptions.MatchCase;
        return FindFirst(findValue, flags, findOptions);
    }
    /// <summary>
    /// This method searches for the first cell with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <param name="findOptions">Way to search the value.</param>
    /// <returns>
    /// First found cell, or Null if value was not found.
    /// </returns>
    public IRange FindFirst(string findValue, ExcelFindType flags, ExcelFindOptions findOptions)
    {
        if (findValue == null) return null;

        bool bIsFormula = ((flags & ExcelFindType.Formula) == ExcelFindType.Formula);
        bool bIsText = ((flags & ExcelFindType.Text) == ExcelFindType.Text);
        bool bIsFormulaStringValue = ((flags & ExcelFindType.FormulaStringValue) == ExcelFindType.FormulaStringValue);
        bool bIsError = ((flags & ExcelFindType.Error) == ExcelFindType.Error);

        if (!(bIsFormula || bIsText || bIsFormulaStringValue || bIsError))
            throw new ArgumentException("Parameter flags is not valid.", "flags");

        return Worksheets.FindFirst(findValue, flags,findOptions);
    }
    /// <summary>
    /// This method searches for the first cell with specified double value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( double findValue, ExcelFindType flags )
    {
      bool bIsFormulaValue = ( ( flags & ExcelFindType.FormulaValue ) == ExcelFindType.FormulaValue );
      bool bIsNumber = ( ( flags & ExcelFindType.Number ) == ExcelFindType.Number );

      if( !( bIsFormulaValue || bIsNumber ) )
        throw new ArgumentException( "Parameter flags is not valid.", "flags" );

      return Worksheets.FindFirst( findValue, flags );
    }
    /// <summary>
    /// This method searches for the first cell with specified bool value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( bool findValue )
    {
      return Worksheets.FindFirst( findValue );
    }
    /// <summary>
    /// This method searches for the first cell with specified DateTime value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( DateTime findValue )
    {
      return Worksheets.FindFirst( findValue );
    }
    /// <summary>
    /// This method searches for the first cell with specified TimeSpan value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>First found cell, or Null if value was not found.</returns>
    public IRange FindFirst( TimeSpan findValue )
    {
      return Worksheets.FindFirst( findValue );
    }
    /// <summary>
    /// This method searches for the all cells with specified string value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( string findValue, ExcelFindType flags )
    {
        return FindAll(findValue, flags, ExcelFindOptions.None);
    }
    /// <summary>
    /// This method searches for the all cells with specified string value based on the Excel find options.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <param name="findOptions">Way to search.</param>
    /// <returns>
    /// All found cells, or Null if value was not found.
    /// </returns>
    public IRange[] FindAll(string findValue, ExcelFindType flags, ExcelFindOptions findOptions)
    {
        if (findValue == null) return null;

        bool bIsFormula = ((flags & ExcelFindType.Formula) == ExcelFindType.Formula);
        bool bIsText = ((flags & ExcelFindType.Text) == ExcelFindType.Text);
        bool bIsFormulaStringValue = ((flags & ExcelFindType.FormulaStringValue) == ExcelFindType.FormulaStringValue);
        bool bIsError = ((flags & ExcelFindType.Error) == ExcelFindType.Error);

        if (!(bIsFormula || bIsText || bIsFormulaStringValue || bIsError))
            throw new ArgumentException("Parameter flags is not valid.", "flags");

        return Worksheets.FindAll(findValue, flags,findOptions);
    }
    ///<summary>
    /// This method searches for the all cells with specified double value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <param name="flags">Type of value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( double findValue, ExcelFindType flags )
    {
      bool bIsFormulaValue = ( ( flags & ExcelFindType.FormulaValue ) == ExcelFindType.FormulaValue );
      bool bIsNumber = ( ( flags & ExcelFindType.Number ) == ExcelFindType.Number );

      if( !( bIsFormulaValue || bIsNumber ) )
        throw new ArgumentException( "Parameter flags is not valid.", "flags" );

      return Worksheets.FindAll( findValue, flags );
    }
    /// <summary>
    /// This method searches for the all cells with specified bool value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found</returns>
    public IRange[] FindAll( bool findValue )
    {
      return Worksheets.FindAll( findValue );
    }
    /// <summary>
    /// This method searches for the all cells with specified DateTime value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( DateTime findValue )
    {
      return Worksheets.FindAll( findValue );
    }
    /// <summary>
    /// This method searches for the all cells with specified TimeSpan value.
    /// </summary>
    /// <param name="findValue">Value to search.</param>
    /// <returns>All found cells, or Null if value was not found.</returns>
    public IRange[] FindAll( TimeSpan findValue )
    {
      return Worksheets.FindAll( findValue );
    }
    /// <summary>
    /// Sets separators for formula parsing.
    /// </summary>
    /// <param name="argumentsSeparator">Arguments separator to set.</param>
    /// <param name="arrayRowsSeparator">Array rows separator to set.</param>
    public void SetSeparators( char argumentsSeparator, char arrayRowsSeparator )
    {
      Syncfusion.Calculate.CalcEngine.ParseArgumentSeparator = argumentsSeparator;
      FormulaUtil.SetSeparators( argumentsSeparator, arrayRowsSeparator );
    }
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Creates header/footer engine.
    /// </summary>
    /// <returns>New instance of header/footer engine.</returns>
    public IHFEngine CreateHFEngine()
    {
      return new HFEngine( Application, this );
    }
#endif
    /// <summary>
    /// Sets protection for workbook.
    /// </summary>
    /// <param name="bIsProtectWindow">Indicates if protect workbook window.</param>
    /// <param name="bIsProtectContent">Indicates if protect workbook content.</param>
    public void Protect( bool bIsProtectWindow, bool bIsProtectContent )
    {
      Protect( bIsProtectWindow, bIsProtectContent, null );
    }
    /// <summary>
    /// Sets protection for workbook.
    /// </summary>
    /// <param name="bIsProtectWindow">Indicates if protect workbook window.</param>
    /// <param name="bIsProtectContent">Indicates if protect workbook content.</param>
    /// <param name="password">Password to protect with.</param>
    public void Protect( bool bIsProtectWindow, bool bIsProtectContent, string password )
    {
      if( !bIsProtectWindow && !bIsProtectContent )
        throw new ArgumentOutOfRangeException( "One of params must be TRUE." );

      if( m_bCellProtect || m_bWindowProtect )
        throw new NotSupportedException( "Workbook is already protected. Use Unprotect before calling method." );

      m_bCellProtect = bIsProtectContent;
      m_bWindowProtect = bIsProtectWindow;
      m_encryptionType = ExcelEncryptionType.Standard;

      if( password != null )
      {
        PasswordRecord pass = Password;
        pass.IsPassword = ( password.Length > 0 )
          ? WorksheetBaseImpl.GetPasswordHash( password )
          : ( ushort )0;
      }
    }
    /// <summary>
    /// Unprotect workbook.
    /// </summary>
    public void Unprotect()
    {
      Unprotect( null );
    }
    /// <summary>
    /// Unprotects workbook. Throws ArgumentOutOfRangeException when password is wrong.
    /// </summary>
    /// <param name="password">Password to unprotect workbook.</param>
    public void Unprotect( string password )
    {
      if( password == null && m_password != null && m_password.IsPassword != 0 ||
        WorksheetBaseImpl.GetPasswordHash( password ) != m_password.IsPassword )
      {
        throw new ArgumentOutOfRangeException( "Please, provide correct password to unprotect this workbook." );
      }

      m_bCellProtect = false;
      m_bWindowProtect = false;

      if( m_strEncryptionPassword == null )
      {
        m_password.IsPassword = 0;
        m_encryptionType = ExcelEncryptionType.None;
      }
    }
    /// <summary>
    /// Creates copy of the current instance.
    /// </summary>
    /// <returns>Copy of the current instance.</returns>
    public IWorkbook Clone()
    {
      WorkbookImpl result = ( WorkbookImpl )MemberwiseClone();

      if( m_fileDataHolder != null )
        result.m_fileDataHolder = m_fileDataHolder.Clone( result );

      result.m_ptrHeapHandle = IntPtr.Zero;

      result.m_fonts = m_fonts.Clone( result );
      result.m_rawFormats = m_rawFormats.Clone( result );
      result.m_extFormats = ( ExtendedFormatsCollection )m_extFormats.Clone( result );
      result.m_styles = ( StylesCollection )m_styles.Clone( result );
      result.m_colors = ClonePalette();

      #region Clonning Stream
      //Preserved Custom Table clonning
      if (m_CustomTableStylesStream != null)
      {
          m_CustomTableStylesStream.Position = 0;
          byte[] data = new byte[m_CustomTableStylesStream.Length];
          m_CustomTableStylesStream.Read(data, 0, data.Length);
          m_CustomTableStylesStream.Position = 0;
          result.m_CustomTableStylesStream = new MemoryStream(data);
      }

      //Preserved ControlStream clonning
      if (m_controlsStream != null)
      {
          m_controlsStream.Position = 0;
          byte[] data = new byte[m_controlsStream.Length];
          m_controlsStream.Read(data, 0, data.Length);
          m_controlsStream.Position = 0;
          result.m_controlsStream = new MemoryStream(data);
      }
      #endregion

      result.m_worksheets = new WorksheetsCollection( Application, result );
      result.m_charts = new ChartsCollection( Application, result );
      result.m_addinFunctions = new AddInFunctionsCollection( Application, result );

      //( WorksheetsCollection )m_worksheets.Clone( result );

      result.m_externSheet = ( ExternSheetRecord )Syncfusion.XlsIO.Parser.Biff_Records.CloneUtils.CloneCloneable( m_externSheet );
      // Extern workbooks must be cloned before objects, since object could reference them
      result.m_externBooks = ( ExternBookCollection )m_externBooks.Clone( result );
      result.m_arrObjects = ( WorkbookObjectsCollection )m_arrObjects.Clone( result );
      result.m_SSTDictionary = ( SSTDictionary )m_SSTDictionary.Clone( result );
      result.m_calcution = ( CalculationOptionsImpl )m_calcution.Clone( result );
      result.m_builtInDocumentProperties = ( BuiltInDocumentProperties )m_builtInDocumentProperties.Clone( result );
      result.m_customDocumentProperties = ( CustomDocumentProperties )m_customDocumentProperties.Clone( result );
      result.m_names = ( WorkbookNamesCollection )m_names.Clone( result );
      result.m_shapesData = ( WorkbookShapeDataImpl )m_shapesData.Clone( result );
      result.m_headerFooterPictures = ( WorkbookShapeDataImpl )m_headerFooterPictures.Clone( result );
      result.m_pivotCaches = ( PivotCacheCollection )Syncfusion.XlsIO.Parser.Biff_Records.CloneUtils.CloneCloneable( ( ICloneParent )m_pivotCaches, result );
      result.m_sheetGroup = ( WorksheetGroup )m_sheetGroup.Clone( result );
      result.m_addinFunctions.CopyFrom( m_addinFunctions );

      // Double clone to fix worksheet indexes update after insert operation.
      result.m_externSheet = ( ExternSheetRecord )Syncfusion.XlsIO.Parser.Biff_Records.CloneUtils.CloneCloneable( m_externSheet );
      result.m_windowOne = ( WindowOneRecord )Syncfusion.XlsIO.Parser.Biff_Records.CloneUtils.CloneCloneable( m_windowOne );
      result.m_password = ( PasswordRecord )Syncfusion.XlsIO.Parser.Biff_Records.CloneUtils.CloneCloneable( m_password );
      result.m_passwordRev4 = ( PasswordRev4Record )Syncfusion.XlsIO.Parser.Biff_Records.CloneUtils.CloneCloneable( m_passwordRev4 );
      result.m_protectionRev4 = ( ProtectionRev4Record )Syncfusion.XlsIO.Parser.Biff_Records.CloneUtils.CloneCloneable( m_protectionRev4 );
      result.m_fileSharing = ( FileSharingRecord )Syncfusion.XlsIO.Parser.Biff_Records.CloneUtils.CloneCloneable( m_fileSharing );

      result.m_arrNames = Syncfusion.XlsIO.Parser.Biff_Records.CloneUtils.CloneCloneable( m_arrNames );
      result.m_arrBound = Syncfusion.XlsIO.Parser.Biff_Records.CloneUtils.CloneCloneable( m_arrBound );
      result.m_arrReparse = new List<IReparse>();
      result.m_arrExtFormatRecords = Syncfusion.XlsIO.Parser.Biff_Records.CloneUtils.CloneCloneable( m_arrExtFormatRecords );
      result.m_arrXFExtRecords = Syncfusion.XlsIO.Parser.Biff_Records.CloneUtils.CloneCloneable(m_arrXFExtRecords);

      result.m_ActiveSheet = null;
      result.ActiveSheetIndex = ActiveSheetIndex;

#if !SILVERLIGHT && !WINRT && !WP
      result.CreateGraphics();
#endif

      result.m_formulaUtil = null;
      int sheetCount = 0;
      foreach (IWorksheet sheet in this.Worksheets)
      {

          if (((sheet) as WorksheetImpl).m_hasSheetCalculation)
          {
              result.Worksheets[sheetCount].CalcEngine = null;
              result.Worksheets[sheetCount].EnableSheetCalculations();
          }
          sheetCount++;
      }
      return result;
    }
    /// <summary>
    /// This method sets write protection password.
    /// </summary>
    /// <param name="password">Password to set.</param>
    public void SetWriteProtectionPassword( string password )
    {
      if( password == null || password.Length == 0 )
      {
        if( m_fileSharing != null )
        {
          m_fileSharing.HashPassword = 0;
          m_fileSharing.CreatorName = null;
        }
      }
      else
      {
        if( m_fileSharing == null )
          m_fileSharing = ( FileSharingRecord )BiffRecordFactory.GetRecord( TBIFFRecord.FileSharing );

        m_fileSharing.HashPassword = WorksheetBaseImpl.GetPasswordHash( password );
        m_fileSharing.CreatorName = Author;
      }
    }
    /// <summary>
    /// Creates copy of the palette.
    /// </summary>
    /// <returns>Copy of the palette.</returns>
    private List<Color> ClonePalette()
    {
      List<Color> arrResult = new List<Color>( m_colors );
      return arrResult;
    }
    /// <summary>
    /// Saves workbook data in Excel 2007 format into specified stream.
    /// </summary>
    /// <param name="stream">Stream to save into.</param>
    /// <param name="saveType">Represents type of the saving document (on the current
    /// moment supported values are template or ordinary document).</param>
    private void SaveInExcel2007( Stream stream, ExcelSaveType saveType )
    {
      if( m_fileDataHolder == null )
      {
        m_fileDataHolder = new FileDataHolder( this );
      }

      m_fileDataHolder.SaveDocument( stream, saveType );
    }
    /// <summary>
    /// Creates serializator that can be used to serialize workbook into file or stream.
    /// </summary>
    /// <param name="version">Version that must be used to serialize workbook.</param>
    /// <returns>Created serializator.</returns>
    private IWorkbookSerializator CreateSerializator( ExcelVersion version, IdReserver shapeIds )
    {
      CheckLicensingSheet();
      AddLicenseWorksheet();

      if( m_externSheet.RefCount > ExternSheetRecord.MaximumRefsCount )
        OptimizeReferences();

      switch( version )
      {
        case ExcelVersion.Excel97to2003:
          return new WorkbookExcel97Serializator( shapeIds );

        case ExcelVersion.Excel2007:
        case ExcelVersion.Excel2010:
        case ExcelVersion.Excel2013:
          if( m_fileDataHolder == null )
            m_fileDataHolder = new FileDataHolder( this );

          return m_fileDataHolder;

        default:
          throw new ArgumentOutOfRangeException( "version" );
      }
    }
    /// <summary>
    /// This method changes internal styles structure for Excel 97.
    /// </summary>
    private void ChangeStylesTo97()
    {
      int[] result = null;
      if( CheckIfStyleChangeNeeded() )
      {
        List<int> arrDefaultStyles = PredefidedStylesPositions();
        result = FixStyles97( arrDefaultStyles );
        UpdateStyleIndexes( result );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="styleIndexes"></param>
    private void UpdateStyleIndexes( int[] styleIndexes )
    {
      if( styleIndexes == null )
        throw new ArgumentNullException( "styleIndexes" );

      for( int i = 0, len = m_arrObjects.Count; i < len; i++ )
      {
        WorksheetBaseImpl sheet = ( WorksheetBaseImpl )m_arrObjects[ i ];
        sheet.UpdateStyleIndexes( styleIndexes );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private List<int> PredefidedStylesPositions()
    {
      List<int> arrResult = new List<int>();

      for( int i = 0, len = PredefinedStyleOutlines.Length; i < len; i++ )
      {
        int iStyleOultine = PredefinedStyleOutlines[ i ];
        string strStyleName = this.AppImplementation.DefaultStyleNames[ iStyleOultine ];

        StyleImpl style = ( m_styles.Contains( strStyleName ) ) ?
          ( StyleImpl )m_styles[ strStyleName ]
          : null;

        int iXFIndex = ( style != null ) ? style.XFormatIndex : -1;
        arrResult.Add( iXFIndex );
      }

      return arrResult;
    }
    /// <summary>
    /// This method checks whether we must change styles model from 2007 into 97 format
    /// (if document (or file) was created by XlsIO we don't need to change styles).
    /// </summary>
    /// <returns>True if additional styles must be added for Excel 97.</returns>
    private bool CheckIfStyleChangeNeeded()
    {
      bool bResult = false;

      if( !bResult )
        bResult = DefaultXFIndex != 15;

      if( !bResult )
      {
        // TODO: finish detection algorithm.
        //throw new 
        ////Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, D
      }

      return bResult;
      //throw new NotImplementedException();
      // 1. Default XF must be 15
      // 2. style[ 1 ] == style[ 2 ]
      // 3. style[ 3 ] == style[ 4 ]
      // 4. style[ 5 ] == style[ 6 ] == ... == style[ 14 ]

    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="defaultStyleIndexes"></param>
    /// <returns></returns>
    private int[] FixStyles97( List<int> defaultStyleIndexes )
    {
      if( defaultStyleIndexes == null )
        throw new ArgumentNullException( "defaultStyleIndexes" );

      int iCount = m_extFormats.Count;
      int[] arrResult = new int[ iCount ];

      for( int i = 0; i < iCount; i++ )
      {
        arrResult[ i ] = -1;
      }

      List<int> arrFontIndexes = ConvertFonts();

      ExtendedFormatsCollection oldXFormats = m_extFormats;
      m_extFormats = new ExtendedFormatsCollection( Application, this );
      InsertDefaultExtFormats();

      for( int i = 0, len = defaultStyleIndexes.Count; i < len; i++ )
      {
        int iXFIndex = defaultStyleIndexes[ i ];

        if( iXFIndex >= 0 )
        {
          int iNewXFIndex = PredefinedXFs[ i ];
          arrResult[ iXFIndex ] = iNewXFIndex;
          m_extFormats.SetXF( iNewXFIndex, oldXFormats[ iXFIndex ] );
        }
      }

      for( int i = 1, len = oldXFormats.Count; i < len; i++ )
      {
        ExtendedFormatImpl format = oldXFormats[ i ];
        ConvertColors( format, arrFontIndexes );
        format.IndentLevel = Math.Min( format.IndentLevel, m_iMaxIndent );

        if( format.FillPattern == ExcelPattern.Gradient )
        {
          format.FillPattern = ExcelPattern.Solid;
          ExcelKnownColors currentColor = format.Gradient.BackColorObject.GetIndexed( this );
          format.ColorObject.SetIndexed( currentColor );
        }

        if( !format.HasParent && arrResult[ i ] < 0 )
        {
          format = m_extFormats.ForceAdd( format );
          arrResult[ i ] = format.Index;
        }
      }

      for( int i = 1, len = oldXFormats.Count; i < len; i++ )
      {
        ExtendedFormatImpl format = oldXFormats[ i ];

        if( format.HasParent && format.Index != m_iDefaultXFIndex )
        {
          format.ParentIndex = arrResult[ format.ParentIndex ];
          format = m_extFormats.Add( format );
          arrResult[ i ] = format.Index;
        }
      }

      m_extFormats.SetXF( 15, oldXFormats[ DefaultXFIndex ] );
      arrResult[ m_iDefaultXFIndex ] = 15;
      m_iDefaultXFIndex = 15;

      return arrResult;
    }
    /// <summary>
    /// Converts extended format colors.
    /// </summary>
    /// <param name="format"></param>
    /// <param name="fontIndexes"></param>
    private void ConvertColors( ExtendedFormatImpl format, List<int> fontIndexes )
    {
      format.ColorObject.ConvertToIndexed( this );
      format.PatternColorObject.ConvertToIndexed( this );
      format.TopBorderColor.ConvertToIndexed( this );
      format.BottomBorderColor.ConvertToIndexed( this );
      format.LeftBorderColor.ConvertToIndexed( this );
      format.RightBorderColor.ConvertToIndexed( this );
      format.DiagonalBorderColor.ConvertToIndexed( this );

      int iOldFontIndex = format.FontIndex;
      format.FontIndex = fontIndexes[ iOldFontIndex ];
    }
    /// <summary>
    /// Reduces font count based by switching all colors into indexed color mode.
    /// </summary>
    /// <returns>List with new font indexes.</returns>
    private List<int> ConvertFonts()
    {
      int iFontCount = m_fonts.Count;
      FontsCollection newFonts = new FontsCollection( Application, this );
      //Dictionary<int, int> dictResult = new Dictionary<int, int>();
      List<int> result = new List<int>( iFontCount );

      // Check whether we have same fonts
      for( int i = 0; i < iFontCount; i++ )
      {
        FontImpl font = ( FontImpl )m_fonts[ i ];
        ColorObject color = font.ColorObject;
        color.ConvertToIndexed( this );
        font = ( FontImpl )newFonts.Add( font );
        result.Add( font.Index );
      }

      iFontCount = newFonts.Count;
      if( iFontCount < 5 )
      {
        // Add minimum fonts count.
        FontImpl font = ( FontImpl )newFonts[ 0 ];

        for( int i = iFontCount; i <= 5; i++ )
        {
          font = ( FontImpl )font.Clone();
          newFonts.ForceAdd( font );
        }
      }

      m_fonts = newFonts;
      return result;
    }

    /// <summary>
    /// Reads the complete stream and retruns false if the doument is notvalid.
    /// </summary>
    /// <param name="reader">Read to get data from.</param>
    /// <param name="separator">Separator between cell values.</param>
    /// <param name="encoding">Encoding scheme for the StreamReader.</param>
    /// <returns>boolean value.</returns>
    private bool IsValidDocument(Stream stream,Encoding encoding,string separator)
    {
        StreamReader streamToRead = new StreamReader(stream, encoding);

        string content = streamToRead.ReadToEnd();
        int index = 0;
        int count = 0;
        int local = 1;
        bool isValue = true;
        int length = separator.Length;
        double totalIndex = content.Length;
        
        while (isValue && local != 0 && index < totalIndex) 
        {
            index = content.IndexOf(TextQualifier, index);
            index += 1;
            local = index;
            count++;
            if ( ((index+length)<=totalIndex) &&content.Substring(index, length) == separator && count % 2 != 0 )
            {
                isValue = false;
            }
        }

        stream.Position = 0;
        
        return isValue;
    }
    #endregion

    #region Class Biff Serialization
    /// <summary>
    /// Serialize workbook for the clipboard.
    /// </summary>
    /// <param name="records">Record's list to serialize into.</param>
    /// <param name="sheet">Worksheet to serialize.</param>
    [CLSCompliant( false )]
    protected internal void SerializeForClipboard( OffsetArrayList records, WorksheetImpl sheet )
    {
      WorkbookExcel97Serializator serializator = new WorkbookExcel97Serializator( null );
      serializator.Serialize( records, ExcelSaveType.SaveAsXLS, null, this, sheet, true );
    }
    #endregion

    #region Class public methods
    /// <summary>
    /// Sets active worksheet.
    /// </summary>
    /// <param name="sheet">Worksheet that will be activated.</param>
    public void SetActiveWorksheet( WorksheetBaseImpl sheet )
    {
      m_ActiveSheet = sheet;
      int realIndex = sheet.RealIndex;
      WindowOneRecord windowOne = WindowOne;
      windowOne.SelectedTab = ( ushort )realIndex;

      if( windowOne.DisplayedTab > ( ushort )realIndex )
        windowOne.DisplayedTab = ( ushort )realIndex;
    }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="wrapper"></param>
    //    public int AddWrapper( FontWrapper wrapper )
    //    {
    //      if( m_bOptimization )
    //      {
    //        return m_arrFontWrappers.Add( wrapper );
    //      }
    //      else
    //      {
    //        return -1;
    //      }
    //    }
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="wrapper"></param>
    //    public void RemoveWrapper( FontWrapper wrapper )
    //    {
    //      if( m_bOptimization )
    //      {
    //        m_arrFontWrappers.Remove( wrapper );
    //        m_iRemovesCount++;
    //      }
    //    }
    //
    //    /// <summary>
    //    /// Removed wrapper by index.
    //    /// </summary>
    //    /// <param name="index">Index to remove.</param>
    //    public void RemoveWrapperAt( int index )
    //    {
    //      if( m_bOptimization )
    //      {
    //        m_arrFontWrappers.RemoveAt( index );
    //        m_iRemovesCount++;
    //      }
    //    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="font"></param>
    /// <returns></returns>
    public bool ContainsFont( FontImpl font )
    {
      return m_fonts.Contains( font );
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="arrNewIndex"></param>
    public void UpdateNamedRangeIndexes( int[] arrNewIndex )
    {
      if( arrNewIndex == null )
        throw new ArgumentNullException( "arrNewIndex" );

      for( int i = 0, len = m_worksheets.Count; i < len; i++ )
      {
        ( ( WorksheetImpl )m_worksheets[ i ] ).UpdateNamedRangeIndexes( arrNewIndex );
      }
    }
    /// <summary>
    /// Updates index of all named ranges.
    /// </summary>
    /// <param name="dicNewIndex">Key - old index, value - new index.</param>
    public void UpdateNamedRangeIndexes( IDictionary<int, int> dicNewIndex )
    {
      if( dicNewIndex == null )
        throw new ArgumentNullException( "dicNewIndex" );

      for( int i = 0, len = m_worksheets.Count; i < len; i++ )
      {
        ( ( WorksheetImpl )m_worksheets[ i ] ).UpdateNamedRangeIndexes( dicNewIndex );
      }
    }
    /// <summary>
    /// Sets Saved flag to the False state.
    /// </summary>
    public void SetChanged()
    {
      Saved = false;
    }
    /// <summary>
    /// Updates string indexes.
    /// </summary>
    /// <param name="arrNewIndexes">List with new indexes.</param>
    public void UpdateStringIndexes( List<int> arrNewIndexes )
    {
      if( arrNewIndexes == null )
        throw new ArgumentNullException( "arrNewIndexes" );

      m_worksheets.UpdateStringIndexes( arrNewIndexes );
    }
    /// <summary>
    /// Copies externsheets to to another workbook.
    /// </summary>
    /// <param name="externSheet">Represents base extern sheets.</param>
    /// <param name="hashSubBooks">Represents dictionary with new sub books indexes.</param>
    /// <returns>Dictioanry with new indexes. Key - old index; value - new index.</returns>
    [ CLSCompliant( false ) ]
    public Dictionary<int, int> CopyExternSheets( ExternSheetRecord externSheet, Dictionary<int, int> hashSubBooks )
    {
      if( externSheet == null )
        throw new ArgumentNullException( "externSheet" );

      if( hashSubBooks == null )
        throw new ArgumentNullException( "hashSubBooks" );

      ExternSheetRecord baseExtern = ExternSheet;
      Dictionary<int, int> result = new Dictionary<int, int>();

      for( int i = 0, iLen = externSheet.RefCount; i < iLen; i++ )
      {
        ExternSheetRecord.TREF reference = externSheet.Refs[ i ];
        int iSubIndex = reference.SupBookIndex;

        if( hashSubBooks.ContainsKey( iSubIndex ) )
          iSubIndex = hashSubBooks[ iSubIndex ];

        int iIndex = baseExtern.AddReference( iSubIndex, reference.FirstSheet, reference.LastSheet );
        result.Add( i, iIndex );
      }

      return result;
    }
    /// <summary>
    /// Looks through all records and calls AddIncrease for each LabelSST record.
    /// </summary>
    public void ReAddAllStrings()
    {
      for( int i = 0, len = m_worksheets.Count; i < len; i++ )
      {
        WorksheetImpl sheet = ( WorksheetImpl )m_worksheets[ i ];
        sheet.ReAddAllStrings();
      }
    }
    /// <summary>
    /// Updates indexes in all records accordingly to the new maximum count property.
    /// </summary>
    /// <param name="maxCount">New value of maximum possible XF index.</param>
    public void UpdateXFIndexes( int maxCount )
    {
      if( maxCount <= 0 )
        throw new ArgumentOutOfRangeException( "maxCount" );

      for( int i = 0, len = m_worksheets.Count; i < len; i++ )
      {
        WorksheetImpl sheet = ( WorksheetImpl )m_worksheets[ i ];
        sheet.UpdateExtendedFormatIndex( maxCount );
      }
    }
    /// <summary>
    /// Indicates whether specified xf index differs from the default one.
    /// </summary>
    /// <param name="xfIndex">XFIndex to check.</param>
    /// <returns>True if there is no difference.</returns>
    public bool IsFormatted( int xfIndex )
    {
      return xfIndex != DefaultXFIndex;
    }
    /// <summary>
    /// Evaluates maximum digit width of the font for Normal style.
    /// </summary>
    /// <returns>Maximum digit width of the font for Normal style.</returns>
    public double GetMaxDigitWidth()
    {
#if !SILVERLIGHT && !WINRT && !WP
      Font font = m_styles[ "Normal" ].Font.GenerateNativeFont();
      if (this.HasStandardFont)
      {
          if (Array.IndexOf(DEF_FONT_WIDTH_SINGLE_INCR, font.Size) >= 0)
              return GetMaxDigitWidth(font) + 1;
          else
              return GetMaxDigitWidth(font);
      }
      else
          return GetMaxDigitWidth(font);
#else
        double dMaxWidth = 0;
        using (AutoFitManager autoFitManager = new AutoFitManager())
        {
            FontImpl font = ((IInternalFont)m_styles["Normal"].Font).Font;
           
            
            for (char ch = '0'; ch <= '9'; ch++)
            {
#if WP
                SizeF size = autoFitManager.CheckThreadMeasureText(new string(ch, 1), font.FontName, font.Bold, font.Italic, (float)font.Size);
#else
                SizeF size = autoFitManager.Measure(new string(ch, 1), font.FontName, font.Bold, font.Italic, (float)font.Size);
#endif

                if (size.Width > dMaxWidth)
                    dMaxWidth = size.Width;
            }

        }
     
      return dMaxWidth;
#endif
    }
    /// <summary>
    /// Evaluates maximum digit width of the font for Normal style.
    /// </summary>
    /// <returns>Maximum digit width of the font for Normal style.</returns>
    public double GetMaxDigitHeight()
    {
#if !SILVERLIGHT && !WINRT && !WP
      Font font = m_styles[ "Normal" ].Font.GenerateNativeFont();
      if (this.HasStandardFont)
      {
          return GetActualValue(font);
      }
      else
      {
          return (int)Math.Ceiling(m_graphics.MeasureString("pP", font).Height);
      }
      return GetMaxDigitHeight( font );
#else
      FontImpl font = ( ( IInternalFont )m_styles[ "Normal" ].Font ).Font;
      double dMaxHeight = 0;

      for (char ch = '0'; ch <= '9'; ch++)
      {
        SizeF size = font.MeasureString( new string ( ch, 1 ) );

        if( size.Height > dMaxHeight )
          dMaxHeight = size.Height;
      }

      return dMaxHeight;
#endif
    }
#if !(SILVERLIGHT) && !(WINRT) && !(WP)
private int GetActualValue(Font font)
{
    if(Array.IndexOf(DEF_FONT_HEIGHT_SINGLE_INCR, font.Size) >=0)
    {
        return (int)Math.Ceiling(m_graphics.MeasureString("pP", font).Height) + 1;
    }
    else if (Array.IndexOf(DEF_FONT_HEIGHT_DOUBLE_INCR,font.Size)>=0)
    {
        return (int)Math.Ceiling(m_graphics.MeasureString("pP", font).Height)+2;
    }
    else
    {
        return (int)Math.Ceiling(m_graphics.MeasureString("pP", font).Height);
    }
}
      
    /// <summary>
    /// Delegate used for digit size evaluation.
    /// </summary>
    /// <param name="rect">Rectangle containing current digit size.</param>
    /// <param name="currentMax">Current maximum value.</param>
    public delegate void DigitSizeCallback( RectangleF rect, ref double currentMax );
    /// <summary>
    /// Evaluates maximum digit width of the specified font.
    /// </summary>
    /// <param name="font">Font to measure.</param>
    /// <returns>Maximum digit width of the specified font.</returns>
    public double GetMaxDigitWidth( Font font )
    {
      return EnumerateDigits( font, GetDigitWidth );
    }
    /// <summary>
    /// Gets maximum digit height.
    /// </summary>
    /// <param name="font">Font to get digit height for.</param>
    /// <returns>Maximum digit height.</returns>
    public double GetMaxDigitHeight( Font font )
    {
      return EnumerateChars( font, GetDigitHeight, new char[]{ 'p', 'P' } );
    }
    /// <summary>
    /// Updates width from the rectangle.
    /// </summary>
    /// <param name="rect">Current rectangle.</param>
    /// <param name="maxValue">Current maximum value.</param>
    private void GetDigitWidth( RectangleF rect, ref double maxValue )
    {
      if( rect.Width > maxValue )
        maxValue = rect.Width;
    }
    /// <summary>
    /// Updates height from the rectangle.
    /// </summary>
    /// <param name="rect">Current rectangle.</param>
    /// <param name="maxValue">Current maximum value.</param>
    private void GetDigitHeight( RectangleF rect, ref double maxValue )
    {
      if( rect.Height > maxValue )
        maxValue = rect.Height;
    }
    /// <summary>
    /// Measures all digits using specified font and calls digitProcessor passing measurement results.
    /// </summary>
    /// <param name="font">Font to use.</param>
    /// <param name="digitProcessor">DigitProcessor to call.</param>
    /// <returns>Value returned by digitProcessor after processing all digits.</returns>
    private double EnumerateDigits( Font font, DigitSizeCallback digitProcessor )
    {
      return EnumerateChars( font, digitProcessor, new char[]{ '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' } );
    }
    private double EnumerateChars( Font font, DigitSizeCallback digitProcessor, char[] chars )
    {
      StringFormat stringFormat = new StringFormat( StringFormat.GenericTypographic );
      stringFormat.Alignment = StringAlignment.Near;
      stringFormat.SetMeasurableCharacterRanges( new CharacterRange[] { new CharacterRange( 1, 1 ) } );
      RectangleF rect = new RectangleF( 0, 0, 1000, 1000 );
      double dMaxValue = 0;

      for( int i = 0, len = chars.Length; i < len; i++ )
      {
        char ch = chars[ i ];
        Region[] arrRegions = m_graphics.MeasureCharacterRanges( "0" + ch, font, rect, stringFormat );
        Region region = arrRegions[ 0 ];
        rect = region.GetBounds( m_graphics );

        digitProcessor( rect, ref dMaxValue );
	  }
	  if (this.HasStandardFont)
      {
          	return
               font.Size <= 10 ?
               font.Size < 10 ?
               dMaxValue :
               dMaxValue - 2 :
               font.Size %2==0?
               dMaxValue:
               dMaxValue-1;
	  }
      else
      {
        	return dMaxValue;
      }
    }
#endif
    /// <summary>
    /// Converts column width in characters into column width in file.
    /// </summary>
    /// <param name="width">Column width in characters.</param>
    /// <returns>Column width in file.</returns>
    public double WidthToFileWidth( double width )
    {
      double dDigitWidth = MaxDigitWidth;
      return ( width > 1 ) ?
        /*Math.Truncate*/( ( width * dDigitWidth + 5 ) / dDigitWidth * 256.0 ) / 256.0 :
        /*Math.Truncate*/( width * ( dDigitWidth + 5 ) / dDigitWidth * 256.0 ) / 256.0;
      //return Math.Truncate( ( width * dDigitWidth + 5 ) / dDigitWidth * 256 ) / 256;
    }
    /// <summary>
    /// Convert column width that is stored in file into pixels.
    /// </summary>
    /// <param name="fileWidth">Column width in file.</param>
    /// <returns>Column width in pixels.</returns>
    public double FileWidthToPixels( double fileWidth )
    {
      double dDigitWidth = MaxDigitWidth;
      return MathGeneral.Truncate( ( ( 256 * fileWidth + MathGeneral.Truncate( 128 / dDigitWidth ) ) / 256 ) * dDigitWidth );
    }
    static double Truncate(double d)
    {
        return d > 0 ? Math.Floor(d) : -Math.Floor(-d);
    }
    /// <summary>
    /// Converts column width in pixels into column width in characters.
    /// </summary>
    /// <param name="pixels">Column width in pixels.</param>
    /// <returns>Column width in characters.</returns>
    public double PixelsToWidth( double pixels )
    {
      double dDigitWidth = MaxDigitWidth;
#if  (SILVERLIGHT) || (WINRT) || (WP)
      return (pixels > dDigitWidth + 5)?
       Truncate((pixels - 5) / dDigitWidth * 100 + 0.5) / 100 :
       pixels / (dDigitWidth + 5);
#else
        
      return
        ( pixels > dDigitWidth + 5 ) ?
        Math.Truncate( ( pixels - 5 ) / dDigitWidth * 100 + 0.5 ) / 100 :
        pixels / ( dDigitWidth + 5 );
#endif
      //return ( pixels > dDigitWidth ) ? 
      //  Math.Truncate( ( pixels - 5 ) / dDigitWidth * 100 + 0.5 ) / 100 :
      //  Math.Truncate( pixels / ( dDigitWidth + 5 ) * 100 ) / 100;
    }
    internal void RemoveUnusedCaches()
    {
        if(m_pivotCaches !=null)
        {
        int []cacheIndexes= m_pivotCaches.GetIndexes();
        pivotCacheIndexes = cacheIndexes;
        foreach (int index in cacheIndexes)
            RemoveCache(index);
        }
    }
    /// <summary>
    /// Checks cache refer by pivot table in the workbook.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="pivotCaches"></param>
    /// <param name="book"></param>
    internal void RemoveCache(int index)
    {
        bool has = false;
        IWorksheets sheets = Worksheets;
        foreach (IWorksheet sheet in sheets)
        {
            IPivotTables tables = sheet.PivotTables;
            for (int i = 0; i < tables.Count; i++)
            {
                IPivotTable table = tables[i];
                if (this.Version != ExcelVersion.Excel97to2003)
                {
                    if (table.CacheIndex == index)
                    {
                        has = true;
                        break;
                    }
                }
                else
                {
                    int cacheIndex = Array.IndexOf(pivotCacheIndexes, index);
                    if (cacheIndex == table .CacheIndex )
                    {
                        has = true ;
                        break;
                    }
                }
            }
            if (has)
                break;
        }
        if (!has)
        {
            m_pivotCaches.RemoveAt(index);
            m_pivotCaches.Order.Remove(index);
        }
    }
    public void DeleteConnection(IConnection Connection)
    {
       // m_deletedConnections.Add(Connection);
        m_connections.Remove(Connection);
    }
    #endregion

    #region Events
    /// <summary>
    /// This event is fired after workbook is successfully saved.
    /// </summary>
    public event EventHandler OnFileSaved;
    /// <summary>
    /// This event is fired when user tries to save into read-only file.
    /// </summary>
    public event ReadOnlyFileEventHandler OnReadOnlyFile;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void WorkbookImpl_AfterChangeEvent(object sender, EventArgs e)
    {
      m_iFirstCharSize = -1;
      m_iSecondCharSize = -1;
      m_dMaxDigitWidth = GetMaxDigitWidth();
      StandardRowHeightInPixels = ( int )GetMaxDigitHeight();
    }
    /// <summary>
    /// Gets or sets standard ( default ) row height of all the worksheets.
    /// in points. Double.
    /// </summary>
    public double StandardRowHeight
    {
      get
      {
        double result;

        if( m_worksheets.Count > 0 )
        {
          result = m_worksheets[ 0 ].StandardHeight;
        }
        else
        {
          result = GetMaxDigitHeight();
        }

        return result;
      }
      set
      {
        if( value != StandardRowHeight )
        {
          for( int i = 0, len = m_worksheets.Count; i < len; i++ )
          {
            m_worksheets[ i ].StandardHeight = value;
          }
        }
      }
    }
    public int StandardRowHeightInPixels
    {
      get
      {
        return ( int )ApplicationImpl.ConvertToPixels( StandardRowHeight, MeasureUnits.Point );
      }
      set
      {
        double heightInPoints = ApplicationImpl.ConvertFromPixel( value, MeasureUnits.Point );
        StandardRowHeight = heightInPoints;
        
      }
    }
    #endregion

    internal void ExtractControlProperties()
    {
      throw new Exception( "The method or operation is not implemented." );
    }

    #region Extended format CRC checksum calculation
    ///<summary>
    /// CRC checksum calculation
    ///</summary>
    internal uint CalculateCRC(uint crcValue, byte[] arrData, uint[] crcCache)
    {
        uint index = 0;
        foreach (byte data in arrData)
        {
            index = crcValue;
            index >>= 24;
            index ^= data;
            crcValue <<= 8;
            crcValue ^= (uint)crcCache[index];
        }
        return crcValue;
    }
    internal static uint[] InitCRC()
    {
        uint[] crcCache = new uint[256];
        uint value = 0;
        uint status = (uint)1 << 31;
        for (uint index = 0; index <= 255; index++)
        {
            value = index;
            value <<= 24;
            for (int bit = 0; bit <= 7; bit++)
            {
                if ((value & status) == status)
                {
                    value <<= 1;
                    value ^= 0xAF;
                }
                else
                {
                    value <<= 1;
                }
            }

            value &= 0xFFFF;
            crcCache[index] = value;
        }

        return crcCache;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Add list of extended properties into XF extended record.
    /// </summary>
    internal ExtendedFormatImpl AddExtendedProperties(ExtendedFormatImpl m_xFormat)
    {
        if (m_xFormat.Index != DefaultXFIndex)
        {
            if (m_xFormat.Properties.Count > 0)
                m_xFormat.Properties.Clear();

            if (m_xFormat.FillPattern == ExcelPattern.Solid)
            {
                if (m_xFormat.ColorObject.ColorType == ColorType.RGB || m_xFormat.ColorObject.ColorType == ColorType.Theme)
                    AddExtendedProperty(CellPropertyExtensionType.ForeColor, m_xFormat.Color, m_xFormat);

                if (m_xFormat.PatternColorObject.ColorType == ColorType.RGB || m_xFormat.PatternColorObject.ColorType == ColorType.Theme)
                    AddExtendedProperty(CellPropertyExtensionType.BackColor, m_xFormat.PatternColor, m_xFormat);
            }
            else
            {
                if (m_xFormat.ColorObject.ColorType == ColorType.RGB || m_xFormat.ColorObject.ColorType == ColorType.Theme)
                    AddExtendedProperty(CellPropertyExtensionType.BackColor, m_xFormat.Color, m_xFormat);

                if (m_xFormat.PatternColorObject.ColorType == ColorType.RGB || m_xFormat.PatternColorObject.ColorType == ColorType.Theme)
                    AddExtendedProperty(CellPropertyExtensionType.ForeColor, m_xFormat.PatternColor, m_xFormat);
            }            

            if ((m_xFormat.TopBorderColor.ColorType == ColorType.RGB || m_xFormat.TopBorderColor.ColorType == ColorType.Theme) && m_xFormat.TopBorderLineStyle != ExcelLineStyle.None)
                AddExtendedProperty(CellPropertyExtensionType.TopBorderColor, m_xFormat.Borders[ExcelBordersIndex.EdgeTop].ColorRGB, m_xFormat);

            if ((m_xFormat.BottomBorderColor.ColorType == ColorType.RGB || m_xFormat.BottomBorderColor.ColorType == ColorType.Theme) && m_xFormat.BottomBorderLineStyle != ExcelLineStyle.None)
                AddExtendedProperty(CellPropertyExtensionType.BottomBorderColor, m_xFormat.Borders[ExcelBordersIndex.EdgeBottom].ColorRGB, m_xFormat);

            if ((m_xFormat.LeftBorderColor.ColorType == ColorType.RGB || m_xFormat.LeftBorderColor.ColorType == ColorType.Theme) && m_xFormat.LeftBorderLineStyle != ExcelLineStyle.None)
                AddExtendedProperty(CellPropertyExtensionType.LeftBorderColor, m_xFormat.Borders[ExcelBordersIndex.EdgeLeft].ColorRGB, m_xFormat);

            if ((m_xFormat.RightBorderColor.ColorType == ColorType.RGB || m_xFormat.RightBorderColor.ColorType == ColorType.Theme) && m_xFormat.RightBorderLineStyle != ExcelLineStyle.None)
                AddExtendedProperty(CellPropertyExtensionType.RightBorderColor, m_xFormat.Borders[ExcelBordersIndex.EdgeRight].ColorRGB, m_xFormat);

            if ((m_xFormat.DiagonalBorderColor.ColorType == ColorType.RGB || m_xFormat.DiagonalBorderColor.ColorType == ColorType.Theme) && m_xFormat.DiagonalDownBorderLineStyle != ExcelLineStyle.None)
                AddExtendedProperty(CellPropertyExtensionType.DiagonalCellBorder, m_xFormat.Borders[ExcelBordersIndex.DiagonalDown].ColorRGB, m_xFormat);

            if (m_xFormat.IndentLevel > 15)
            {
                ExtendedProperty property = new ExtendedProperty();
                property.Type = CellPropertyExtensionType.TextIndentationLevel;
                property.Size = 6;
                property.Indent = (ushort)m_xFormat.IndentLevel;
                m_xFormat.Properties.Add(property);
            }

            FontImpl fontImp = this.InnerFonts[m_xFormat.FontIndex] as FontImpl;
            if (fontImp.ColorObject.ColorType == ColorType.RGB || fontImp.ColorObject.ColorType == ColorType.Theme)
            {
                AddExtendedProperty(CellPropertyExtensionType.TextColor, m_xFormat.Font.RGBColor, m_xFormat);
            }
        }

        return m_xFormat;
    }
    /// <summary>
    /// Add extended property into list.
    /// </summary>
    internal void AddExtendedProperty(CellPropertyExtensionType type, Color ColorValue,ExtendedFormatImpl m_xFormat)
    {
        ColorValue = ConvertARGBToRGBA(ColorValue);

        //add extended property
        ExtendedProperty property = new ExtendedProperty();

        property.Type = GetPropertyType(type);
        property.Size = 20;
        property.ColorValue = ColorToUInt(ColorValue);
        m_xFormat.Properties.Add(property);
    }
    /// <summary>
    /// Get extended property type.
    /// </summary>
    internal CellPropertyExtensionType GetPropertyType(CellPropertyExtensionType type)
    {
        switch (type)
        {
            case CellPropertyExtensionType.ForeColor:
                type = CellPropertyExtensionType.ForeColor;
                break;

            case CellPropertyExtensionType.BackColor:
                type = CellPropertyExtensionType.BackColor;
                break;

            case CellPropertyExtensionType.GradientFill:
                type = CellPropertyExtensionType.GradientFill;
                break;

            case CellPropertyExtensionType.TopBorderColor:
                type = CellPropertyExtensionType.TopBorderColor;
                break;

            case CellPropertyExtensionType.BottomBorderColor:
                type = CellPropertyExtensionType.BottomBorderColor;
                break;

            case CellPropertyExtensionType.LeftBorderColor:
                type = CellPropertyExtensionType.LeftBorderColor;
                break;

            case CellPropertyExtensionType.RightBorderColor:
                type = CellPropertyExtensionType.RightBorderColor;
                break;

            case CellPropertyExtensionType.DiagonalCellBorder:
                type = CellPropertyExtensionType.DiagonalCellBorder;
                break;

            case CellPropertyExtensionType.TextColor:
                type = CellPropertyExtensionType.TextColor;
                break;

            case CellPropertyExtensionType.FontScheme:
                type = CellPropertyExtensionType.FontScheme;
                break;

            case CellPropertyExtensionType.TextIndentationLevel:
                type = CellPropertyExtensionType.TextIndentationLevel;
                break;
        }

        return type;
    }
    /// <summary>
    /// Convert ARGB to RGBA.
    /// </summary>
    internal Color ConvertARGBToRGBA(Color colorValue)
    {
        //convert ARGB to RGBA        
        byte New_R = colorValue.B;
        byte New_G = colorValue.G;
        byte New_B = colorValue.R;
        byte New_A = colorValue.A;
#if  (SILVERLIGHT || WP)
        colorValue = Color.FromArgb(New_A, New_R, New_G, New_B);
#elif ( WINRT )
        colorValue = Color.FromArgb(New_A, New_R, New_G, New_B);
#else
        colorValue = System.Drawing.Color.FromArgb(New_A, New_R, New_G, New_B);
#endif

        return colorValue;
    }
    /// <summary>
    /// Convert ARGB to RGBA.
    /// </summary>
    internal Color ConvertRGBAToARGB(Color colorValue)
    {
        //convert ARGB to RGB  
        byte New_A = colorValue.A;
        byte New_R = colorValue.B;
        byte New_G = colorValue.G;
        byte New_B = colorValue.R;
#if (SILVERLIGHT || WP)
        colorValue=System.Windows.Media.Color.FromArgb(New_A, New_R, New_G, New_B);
#elif ( WINRT )
        colorValue = Color.FromArgb(New_A, New_R, New_G, New_B);
#else
        colorValue = System.Drawing.Color.FromArgb(New_A, New_R, New_G, New_B);
#endif

        return colorValue;
    }
    /// <summary>
    /// Convert Color object to unsigned integer.
    /// </summary>
    internal uint ColorToUInt(Color color)
    {
        return (uint)((color.A << 24) | (color.R << 16) | (color.G << 8) | (color.B << 0));
    }
    /// <summary>
    /// Convert unsigned integer to Color object.
    /// </summary>
    internal Color UIntToColor(uint color)
    {
        byte a = (byte)(color >> 24);
        byte r = (byte)(color >> 16);
        byte g = (byte)(color >> 8);
        byte b = (byte)(color >> 0);
        return Color.FromArgb(a, r, g, b);
    }
    /// <summary>
    /// Whether a given character is allowed by XML 1.0.
    /// </summary>
    private bool IsLegalXmlChar(int character)
    {
        return (character == 0x9 /* == '\t' == 9   */
            || character == 0xA /* == '\n' == 10  */
            || character == 0xD /* == '\r' == 13  */
            || (character >= 0x20 && character <= 0xD7FF)
            || (character >= 0xE000 && character <= 0xFFFD)
            || (character >= 0x10000 && character <= 0x10FFFF));
    }
    /// <summary>
    /// Remove illegal xml character which is not allowed by XML 1.0.
    /// </summary>
    internal string RemoveInvalidXmlCharacters(string nameValue)
    {
        byte[] bytNameValue = Encoding.Unicode.GetBytes(nameValue);
        bool isInvalidChar = false;

        for (int i = 0; i < bytNameValue.Length; i++)
        {
            if ((!IsLegalXmlChar(bytNameValue[i])) && bytNameValue[i]!=0)
            {
                bytNameValue[i] = 0;
                isInvalidChar = true;
            }
        }

        string definedName = Encoding.Unicode.GetString(bytNameValue, 0, bytNameValue.Length);
        definedName = definedName.Replace("\0", string.Empty);
        if (isInvalidChar)
        {
            definedName = "_" + XmlInvalidCharCount + definedName;
            XmlInvalidCharCount++;
        }

        return definedName;
    }
    #endregion
  }
}
