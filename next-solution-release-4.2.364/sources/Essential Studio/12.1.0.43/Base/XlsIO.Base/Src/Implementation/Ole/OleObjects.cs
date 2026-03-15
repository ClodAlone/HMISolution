#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;


using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Implementation.XmlSerialization;
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using Syncfusion.XlsIO.Parser.Biff_Records;
using System.IO;
using Syncfusion.CompoundFile.XlsIO;

namespace Syncfusion.XlsIO.Implementation
{
  public class OleObjects :
    List<IOleObject>,
    IOleObjects
  {
    #region Members
    /// <summary>
    /// Parent worksheet.
    /// </summary>
    private WorksheetImpl m_sheet;
    #endregion

    #region Methods
    /// <summary>
    /// Initializes new instance of the collection.
    /// </summary>
    /// <param name="sheet">Parent worksheet.</param>
    public OleObjects( WorksheetImpl sheet )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      m_sheet = sheet;
    }
    /// <summary>
    /// Adds the specified OLE.
    /// </summary>
    /// <param name="ole">The OLE.</param>
    public new void Add( OleObject ole )
    {
      if( ( ole.FileNativeData == null ) && ( ole.OleType == OleLinkType.Embed ) )
      {
        ole.StorageName = OleTypeConvertor.GetOleFileName();
        ole.SetOleFile( ole.FileName, ole.StorageName );
        SetOleObject( ole );
      }
      else if( ( ole.OleType == OleLinkType.Link ) || ( ole.IsStream == true ) )
      {
        SetOleObject( ole );
      }

      base.Add( ole );
    }
    /// <summary>
    /// Adds new ole object to the collection.
    /// </summary>
    /// <param name="fileName">File name.</param>
    /// <param name="image">File image.</param>
    /// <param name="linkType">Link type.</param>
    public IOleObject Add( string fileName, Image image, OleLinkType linkType )
    {
      // 1. Create Image shape
      IPictureShape shape = m_sheet.Pictures.AddPicture( 1, 1, image );

      if( linkType == OleLinkType.Link )
      {
        fileName = Path.GetFullPath( fileName );
        ExternWorkbookImpl book = CreateExternalLink( fileName );
      }

      // 2. Create Ole object
      OleObject oleObject = new OleObject( fileName, shape, linkType );
      // 3. Add ole object.
      Add( oleObject );
      //oleObject.ObjectType = "Word.Document.12";

      return oleObject;
    }

    private ExternWorkbookImpl CreateExternalLink( string fileName )
    {
      WorkbookImpl book = m_sheet.ParentWorkbook;

      string strFileName = Path.GetFileName( fileName );
      string strFilePath = fileName.Substring( 0, fileName.Length - strFileName.Length );

      int index = book.ExternWorkbooks.Add( strFilePath, strFileName, null, new string[]{ "'" } );
      ExternWorkbookImpl result = book.ExternWorkbooks[ index ];
      //result.ProgramId = "Word.Document.12";
      result.ProgramId = "Package";

      ExternNameRecord name = result.ExternNames[ 0 ].Record;
      name.OleLink = true;
      name.Ole = false;
      name.WantPicture = true;
      name.WantAdvise = true;
      name.BuiltIn = false;

      return result;
    }
    /// <summary>
    /// Sets the OLE object.
    /// </summary>
    /// <param name="ole">The OLE.</param>
    internal static void SetOleObject( OleObject ole )
    {
      //if( ole.Location == null )
      //  ole.SetLocation( 0, 0 );

      //if( ole.Size.IsEmpty )
      //  ole.Size = new SizeF( ole.Picture.Width * 72 / 96, ole.Picture.Height * 72 / 96 );

      //ole.GetRangeValues( ole.Location );
    }

    #endregion
  }

  #region OleTypeConvertor Class

  /// <summary>
  /// Class performs converting string to OleObjectType enum and vice versa.
  /// </summary>
  internal class OleTypeConvertor
  {
    #region Constants
    internal const string OleType = "Package";
    #endregion

    #region Members
    static int m_fileid = 1;
    internal static List<string> files = new List<string>();
    #endregion

    #region Methods
    /// <summary>
    /// Converts the string to "OleObjectType"
    /// </summary>
    /// <param name="oleTypeStr">The OLE type STR.</param>
    /// <returns></returns>
    internal static OleObjectType ToOleType(string oleTypeStr)
    {
        OleObjectType oleType = OleObjectType.Undefined;
        if (oleTypeStr.StartsWith("Acrobat Document") || oleTypeStr.StartsWith("AcroExch.Document.7"))
            oleType = OleObjectType.AdobeAcrobatDocument;
        else if (oleTypeStr.StartsWith("Package"))
            oleType = OleObjectType.Package;
        else if (oleTypeStr.StartsWith("PBrush"))
            oleType = OleObjectType.BitmapImage;
        else if (oleTypeStr.StartsWith("Media Clip") || oleTypeStr.StartsWith("MPlayer"))
            oleType = OleObjectType.MediaClip;
        else if (oleTypeStr.StartsWith("Microsoft Equation 3.0") || oleTypeStr.StartsWith("Equation.3"))
            oleType = OleObjectType.Equation;
        else if (oleTypeStr.StartsWith("Microsoft Graph Chart") || oleTypeStr.StartsWith("MSGraph.Chart.8"))
            oleType = OleObjectType.GraphChart;
        else if (oleTypeStr.StartsWith("Microsoft Office Excel 2003 Worksheet") || oleTypeStr.StartsWith("Excel.Sheet.8"))
            oleType = OleObjectType.Excel_97_2003_Worksheet;
        else if (oleTypeStr.StartsWith("Microsoft Office Excel Binary Worksheet") || oleTypeStr.StartsWith("Excel.SheetBinaryMacroEnabled.12"))
            oleType = OleObjectType.ExcelBinaryWorksheet;
        else if (oleTypeStr.StartsWith("Microsoft Office Excel Chart") || oleTypeStr.StartsWith("Excel.Chart.8"))
            oleType = OleObjectType.ExcelChart;
        else if (oleTypeStr.StartsWith("Microsoft Office Excel Worksheet (code)") || oleTypeStr.StartsWith("Excel.SheetMacroEnabled.12"))
            oleType = OleObjectType.ExcelMacroWorksheet;
        else if (oleTypeStr.StartsWith("Microsoft Office Excel Worksheet") || oleTypeStr.StartsWith("Excel.Sheet.12"))
            oleType = OleObjectType.ExcelWorksheet;
        else if (oleTypeStr.StartsWith("Microsoft Office PowerPoint 97-2003 Presentation") || oleTypeStr.StartsWith("PowerPoint.Show.8"))
            oleType = OleObjectType.PowerPoint_97_2003_Presentation;
        else if (oleTypeStr.StartsWith("Microsoft Office PowerPoint 97-2003 Slide") || oleTypeStr.StartsWith("PowerPoint.Slide.8"))
            oleType = OleObjectType.PowerPoint_97_2003_Slide;
        else if (oleTypeStr.StartsWith("Microsoft Office PowerPoint Macro-Enabled Presentation") || oleTypeStr.StartsWith("PowerPoint.ShowMacroEnabled.12"))
            oleType = OleObjectType.PowerPointMacroPresentation;
        else if (oleTypeStr.StartsWith("Microsoft Office PowerPoint Macro-Enabled Slide") || oleTypeStr.StartsWith("PowerPoint.SlideMacroEnabled.12"))
            oleType = OleObjectType.PowerPointMacroSlide;
        else if (oleTypeStr.StartsWith("Microsoft Office PowerPoint Presentation") || oleTypeStr.StartsWith("PowerPoint.Show.12"))
            oleType = OleObjectType.PowerPointPresentation;
        else if (oleTypeStr.StartsWith("Microsoft Office PowerPoint Slide") || oleTypeStr.StartsWith("PowerPoint.Slide.12"))
            oleType = OleObjectType.PowerPointSlide;
        else if (oleTypeStr.StartsWith("Microsoft Office Word 97-2003 Document") || oleTypeStr.StartsWith("Word.Document.8"))
            oleType = OleObjectType.Word_97_2003_Document;
        else if (oleTypeStr.StartsWith("Microsoft Office Word Document") || oleTypeStr.StartsWith("Word.Document.12"))
            oleType = OleObjectType.WordDocument;
        else if (oleTypeStr.StartsWith("Microsoft Office Word Macro-Enabled Document") || oleTypeStr.StartsWith("Word.DocumentMacroEnabled.12"))
            oleType = OleObjectType.WordMacroDocument;
        else if (oleTypeStr.StartsWith("Microsoft Visio Drawing") || oleTypeStr.StartsWith("Visio.Drawing.11"))
            oleType = OleObjectType.VisioDrawing;
        else if (oleTypeStr.StartsWith("OpenDocument Presentation") || oleTypeStr.StartsWith("PowerPoint.OpenDocumentPresentation.12"))
            oleType = OleObjectType.OpenDocumentPresentation;
        else if (oleTypeStr.StartsWith("OpenDocument Spreadsheet") || oleTypeStr.StartsWith("Excel.OpenDocumentSpreadsheet.12"))
            oleType = OleObjectType.OpenDocumentSpreadsheet;
        else if (oleTypeStr.StartsWith("opendocument.CalcDocument.1"))
            oleType = OleObjectType.OpenOfficeSpreadsheet;
        else if (oleTypeStr.StartsWith("opendocument.WriterDocument.1"))
            oleType = OleObjectType.OpenOfficeText;
        else if (oleTypeStr.StartsWith("soffice.StarCalcDocument.6"))
            oleType = OleObjectType.OpenOfficeSpreadsheet1_1;
        else if (oleTypeStr.StartsWith("soffice.StarWriterDocument.6"))
            oleType = OleObjectType.OpenOfficeText_1_1;
        else if (oleTypeStr.StartsWith("Video Clip") || oleTypeStr.StartsWith("AVIFile"))
            oleType = OleObjectType.VideoClip;
        else if (oleTypeStr.StartsWith("WaveSound") || oleTypeStr.StartsWith("SoundRec"))
            oleType = OleObjectType.WaveSound;
        else //if ( oleTypeStr.StartsWith("WordPad Document") || oleTypeStr.StartsWith("WordPad.Document.1"))
            oleType = OleObjectType.WordPadDocument;

        return oleType;
    }
    /// <summary>
    /// Converts the string to "OleObjectType"
    /// </summary>
    /// <param name="oleTypeStr">The OLE type STR.</param>
    /// <returns></returns>
    internal static string ToOleString(OleObjectType oleType)
    {
        string strOleType = string.Empty;
        switch (oleType)
        {
            case OleObjectType.AdobeAcrobatDocument:
                strOleType =  "AcroExch.Document.7";
                break;
            case OleObjectType.Package:
                strOleType = "Package";
                break;
            case OleObjectType.BitmapImage:
                strOleType = "PBrush";
                break;
            case OleObjectType.MediaClip:
                strOleType =  "MPlayer";
                break;
            case OleObjectType.Equation:
                strOleType = "Equation.3";
                break;
            case OleObjectType.GraphChart:
                strOleType = "MSGraph.Chart.8";
                break;
            case OleObjectType.Excel_97_2003_Worksheet:
                strOleType =  "Excel.Sheet.8";
                break;
            case OleObjectType.ExcelBinaryWorksheet:
                strOleType = "Excel.SheetBinaryMacroEnabled.12";
                break;
            case OleObjectType.ExcelChart:
                strOleType =  "Excel.Chart.8";
                break;
            case OleObjectType.ExcelMacroWorksheet:
                strOleType =  "Excel.SheetMacroEnabled.12";
                break;
            case OleObjectType.ExcelWorksheet:
                strOleType =  "Excel.Sheet.12";
                break;
            case OleObjectType.PowerPoint_97_2003_Presentation:
                strOleType = "PowerPoint.Show.8";
                break;
            case OleObjectType.PowerPoint_97_2003_Slide:
                strOleType =  "PowerPoint.Slide.8";
                break;
            case OleObjectType.PowerPointMacroPresentation:
                strOleType =  "PowerPoint.ShowMacroEnabled.12";
                break;
            case OleObjectType.PowerPointMacroSlide:
                strOleType =  "PowerPoint.SlideMacroEnabled.12";
                break;
            case OleObjectType.PowerPointPresentation:
                strOleType = "PowerPoint.Show.12";
                break;
            case OleObjectType.PowerPointSlide:
                strOleType = "PowerPoint.Slide.12";
                break;
            case OleObjectType.Word_97_2003_Document:
                strOleType = "Word.Document.8";
                break;
            case OleObjectType.WordDocument:
                strOleType =  "Word.Document.12";
                break;
            case OleObjectType.WordMacroDocument:
                strOleType = "Word.DocumentMacroEnabled.12";
                break;
            case OleObjectType.VisioDrawing:
                strOleType =  "Visio.Drawing.11";
                break;
            case OleObjectType.OpenDocumentPresentation:
                strOleType = "PowerPoint.OpenDocumentPresentation.12";
                break;
            case OleObjectType.OpenDocumentSpreadsheet:
                strOleType = "Excel.OpenDocumentSpreadsheet.12";
                break;
            case OleObjectType.OpenOfficeSpreadsheet:
                strOleType = "opendocument.CalcDocument.1";
                break;
            case OleObjectType.OpenOfficeText:
                strOleType = "opendocument.WriterDocument.1";
                break;
            case OleObjectType.OpenOfficeSpreadsheet1_1:
                strOleType = "soffice.StarCalcDocument.6";
                break;
            case OleObjectType.OpenOfficeText_1_1:
                strOleType = "soffice.StarWriterDocument.6";
                break;
            case OleObjectType.VideoClip:
                strOleType ="AVIFile";
                break;
            case OleObjectType.WaveSound:
                strOleType = "SoundRec";
                break;
            case OleObjectType.WordPadDocument:
                strOleType =  "WordPad.Document.1";
                break;
            case OleObjectType.MIDISequence:
                strOleType = "MIDI Sequence";
                break;
        }
        return strOleType;
    }
    /// <summary>
    /// Gets the GUID for specified type of object.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    internal static Guid GetGUID()
    {
      Guid guid = Guid.NewGuid();
      string strGuid = "0003000c-0000-0000-c000-000000000046";

      if( strGuid != null )
      {
        guid = new Guid( strGuid );
      }
      return guid;
    }
    /// <summary>
    /// Gets the name of the OLE file.
    /// </summary>
    /// <returns></returns>
    internal static string GetOleFileName()
    {
      string name = "oleObject" + GetNextFileId().ToString() + ".bin";
      return name;
    }
    /// <summary>
    /// Gets the next file id.
    /// </summary>
    /// <returns></returns>
    internal static int GetNextFileId()
    {
      return m_fileid++;
    }
    /// <summary>
    /// Adds the specified file name.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    internal static void Add( string fileName )
    {
      files.Add( fileName );
    }
    #endregion
  }

  #endregion

  //TODO:- Remove this below class once implemented the complete OLEOBJECT parsing and serialization.
  /// <summary>
  /// Used to preserve OleObjects for add copy method
  /// </summary>
  public class OleStorageCollection
  {
      string[] OleConstants = { "_VBA_PROJECT_CUR", "_SX_DB_CUR", "MsoDataStore" };

      Dictionary<string, OleStorage> m_OleStorage = new Dictionary<string, OleStorage>();
      Dictionary<string, MemoryStream> m_arrayStream = new Dictionary<string, MemoryStream>();

      private WorkbookImpl m_workbook;

      private List<string> m_OleStoragesNames = new List<string>();
      private List<string> m_arrayStreamNames = new List<string>();

      internal void Add(OleStorage storage)
      {
          m_OleStorage.Add(storage.StorageName, storage);
          m_OleStoragesNames.Add(storage.StorageName);
      }
      internal void Add(string streamName, CompoundStream stream)
      {
          MemoryStream m_controlsStream = new MemoryStream((int)stream.Length);
          UtilityMethods.CopyStreamTo(stream, m_controlsStream);

          stream.Position = 0;
          m_arrayStream.Add(streamName, m_controlsStream);
          m_arrayStreamNames.Add(streamName);
      }
      internal void Add(string streamName, MemoryStream stream)
      {
          stream.Position = 0;
          m_arrayStream.Add(streamName, stream);
          m_arrayStreamNames.Add(streamName);
      }
      public OleStorageCollection()
      {
          
      }

      public List<string> OleStoragesNames
      {
          get { return m_OleStoragesNames; }
      }
      public List<string> ArrayStreamNames
      {
          get { return m_arrayStreamNames; }
      }

      internal void ParseStorage(ICompoundStorage CompoundStorage)
      {
          string[] streams = CompoundStorage.Streams;
          OleStorage storage = new OleStorage(CompoundStorage.Name);

          if (Array.IndexOf(streams, "CONTENTS") != -1)
          {
              using (CompoundStream oleStream = CompoundStorage.OpenStream("CONTENTS"))
              {
                  storage.ParseStream(oleStream);

              }
          }
          else
          {
              foreach (string stream in streams)
              {
                  using (CompoundStream oleStream = CompoundStorage.OpenStream(stream))
                  {
                      storage.ParseStream(oleStream);

                  }
              }
          }

          Add(storage);
      }

      internal OleStorage OpenStorage(string StorageName)
      {
          return this.m_OleStorage[StorageName];
      }

      internal MemoryStream OpenStream(string streamName)
      {
          return this.m_arrayStream[streamName];
      }

            
  }
}
