#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Text;

using Syncfusion.XlsIO.Implementation.XmlSerialization;
using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Syncfusion.XlsIO.Parser;
using Syncfusion.XlsIO.Implementation.Exceptions;
using Syncfusion.XlsIO.Implementation.XmlReaders.Shapes;
using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Shapes;
using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using Syncfusion.Compression.Zip;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Constants;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation.Charts;
using Syncfusion.XlsIO.Implementation.XmlSerialization.Charts;
using Syncfusion.CompoundFile.XlsIO;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using Syncfusion.XlsIO.Drawing;

#if ( WINRT )
using Windows.UI;
using Syncfusion.XlsIO.Implementation.WINRT;
#endif

#if  (SILVERLIGHT)
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using System.Windows.Media;
using Syncfusion.XlsIO.Implementation.WP;
#elif ( WINRT )
using Syncfusion.XlsIO;
#else
using System.Drawing;
#endif

namespace Syncfusion.XlsIO.Implementation.XmlReaders
{
  /// <summary>
  /// Class used for parsing Excel 2007 workbooks.
  /// </summary>
  public class Excel2007Parser
  {
    #region Constants
    /// <summary>
    /// Represents HLS max value.
    /// </summary>
    internal const byte HLSMax = 255;
    /// <summary>
    /// Represents RGB max value.
    /// </summary>
    private const byte RGBMax = 255;
    /// <summary>
    /// Represents undefined HLS value.
    /// </summary>
    private const double Undefined = ( HLSMax * 2 ) / 3;
    public const int AdditionalProgressItems = 4;
    /// <summary>
    /// CarriageReturn ControlCharacters
    /// </summary>
    private const string CarriageReturn = "_x000d_";
    /// <summary>
    /// LineFeed ControlCharacters
    /// </summary>
    private const string LineFeed = "_x000a_";
    /// <summary>
    /// NullChar ControlCharacters
    /// </summary>
    private const string NullChar = "_x0000_";
    /// <summary>
    /// BackSpace ControlCharacters
    /// </summary>
    private const string BackSpace = "_x0008_";
    /// <summary>
    /// Tab ControlCharacters
    /// </summary>
    private const string Tab = "_x0009_";
    /// <summary>
    /// Content TypeSchema
    /// </summary>
    private const string ContentTypeSchema = "contentTypeSchema";
    /// <summary>
    /// Content Type NameSpace
    /// </summary>
    private const string ContentTypeNameSpace = "http://schemas.microsoft.com/office/2006/metadata/contentType";
    /// <summary>
    /// Xml Schema NameSpace
    /// </summary>
    private const string XmlSchemaNameSpace = "http://www.w3.org/2001/XMLSchema";
    /// <summary>
    /// Element Name
    /// </summary>
    private const string ElementName = "element";
    /// <summary>
    /// Name Attribute
    /// </summary>
    private const string Name = "name";
    /// <summary>
    ///Display Name Attribute
    /// </summary>
    private const string DisplayName = "ma:displayName";
    /// <summary>
    ///Internal Name Attribute
    /// </summary>
    private const string InternalName = "ma:internalName";
    /// <summary>
    ///Ref attribute
    /// </summary>
    private const string Reference = "ref";
    /// <summary>
    /// Complex content
    /// </summary>
    private const string ComplexContent = "complexContent";

    #endregion

    #region Members
    /// <summary>
    /// Parent workbook.
    /// </summary>
    private WorkbookImpl m_book;
    /// <summary>
    /// Formula utils.
    /// </summary>
    private FormulaUtil m_formulaUtil;
    /// <summary>
    /// Dictionary with all available shape parsers. Key - shape instance value,
    /// or o:spt value; Value - shape parser.
    /// </summary>
    private Dictionary<int, ShapeParser> m_dictShapeParsers = new Dictionary<int, ShapeParser>();
    /// <summary>
    /// Theme colors list.
    /// </summary>
    private List<Color> m_lstThemeColors;
    /// <summary>
    /// Dictionary of theme colors: key - theme color name, value - corresponding color.
    /// </summary>
    private Dictionary<string, Color> m_dicThemeColors;
    private Dictionary<string, FontImpl> m_dicMajorFonts;
    private Dictionary<string, FontImpl> m_dicMinorFonts;
    private List<string> m_values = new List<string>();
    /// <summary>
    /// Parent element name.
    /// </summary>
    private string parentElement = string.Empty;

    private bool m_enableAlternateContent = false;
    /// <summary>
    /// Represents the current worksheet.
    /// </summary>
    private WorksheetImpl m_workSheet;
    private DrawingParser m_drawingParser;
    /// <summary>
    /// Represens the Range impl objects
    /// </summary>
    //private RangeImpl m_rangeImpl;
    /// <summary>
    /// Collecton of Outline levels 
    /// </summary>
    private Dictionary<int, List<Point>> m_outlineLevels = new Dictionary<int, List<Point>>();
    private Dictionary<int, int> m_indexAndLevels = new Dictionary<int, int>();
    /// <summary>
    /// Represent the OutlineWrapperUtility object.
    /// </summary>
    private OutlineWrapperUtility m_outlineWrapperUtility;
    private int dpiX;
    private int dpiY;
    #endregion

    #region Properties
    /// <summary>
    /// Gets FformulaUtil that corresponds to invariant culture.
    /// </summary>
    public FormulaUtil FormulaUtil
    {
      get
      {
        return m_formulaUtil;
      }
    }
    /// <summary>
    /// Gets the worksheet.
    /// </summary>
    /// <value>The worksheet.</value>
    internal WorksheetImpl Worksheet
    {
        get
        {
            return m_workSheet;
        }
    }
    #endregion

    #region Constructors
    /// <summary>
    /// Initializes new instance of the parser.
    /// </summary>
    /// <param name="book">Parent workbook.</param>
    public Excel2007Parser( WorkbookImpl book )
    {
      if( book == null )
        throw new ArgumentNullException( "book" );

      m_book = book;
      m_formulaUtil = new FormulaUtil( m_book.Application, m_book, NumberFormatInfo.InvariantInfo,
        ApplicationImpl.DEF_ARGUMENT_SEPARATOR, ApplicationImpl.DEF_ROW_SEPARATOR );

      this.dpiX = book.AppImplementation.GetdpiX();
      this.dpiY = book.AppImplementation.GetdpiY();
      m_dictShapeParsers.Add( CommentShapeImpl.ShapeInstance, new CommentShapeParser() );
      m_dictShapeParsers.Add( CheckBoxShapeImpl.ShapeInstance, new VmlFormControlParser() );
      m_dictShapeParsers.Add( BitmapShapeImpl.ShapeInstance, new HFImageParser() );
    }
    #endregion

    #region Methods
    /// <summary>
    /// Converts theme color name into rgb color value.
    /// </summary>
    /// <param name="colorName">Color name to get rgb color value for.</param>
    /// <returns>Rgb color object that corresponds to the theme color.</returns>
    public Color GetThemeColor( string colorName )
    {
      return GetThemeColor( colorName, m_dicThemeColors );
    }
    /// <summary>
    /// Converts theme color name into rgb color value.
    /// </summary>
    /// <param name="colorName">Color name to get rgb color value for.</param>
    /// <param name="themeColors">Dictionary that holds theme color name and color.</param>
    /// <returns>Rgb color object that corresponds to the theme color.</returns>
    public static Color GetThemeColor( string colorName, Dictionary<string, Color> themeColors )
    {
      if( colorName == null || colorName.Length == 0 )
        throw new ArgumentOutOfRangeException( "colorName" );

      Color color = ColorExtension.Empty;
      bool bResult = themeColors.TryGetValue( colorName, out color );

      if( !bResult && colorName == "bg1" )
      {
        colorName = "lt1";
        color = GetThemeColor( colorName, themeColors );
      }
      else if (!bResult && colorName == "bg2")
      {
          colorName = "lt2";
          color = GetThemeColor(colorName, themeColors);
      }
      else
      {
          //Debug.Assert( bResult, "Color wasn't found " + colorName );
      }

      return color;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="contentDefaults"></param>
    /// <param name="contentOverrides"></param>
    public void ParseContentTypes( XmlReader reader, IDictionary<string, string> contentDefaults,
      IDictionary<string, string> contentOverrides )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( contentDefaults == null )
        throw new ArgumentNullException( "contentDefaults" );

      if( contentOverrides == null )
        throw new ArgumentNullException( "contentOverrides" );

      //reader.MoveToContent();
      //reader.Read();

      while( reader.NodeType != XmlNodeType.Element && !reader.EOF )
        reader.Read();

      if( reader.EOF )
        throw new XmlException( "Cannot locate appropriate xml tag" );

      if( reader.LocalName == Excel2007Serializator.TypesTagName &&
        reader.NamespaceURI == Excel2007Serializator.ContentTypesNamespace )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case Excel2007Serializator.DefaultTagName:
                ParseDictionaryEntry( reader, contentDefaults, Excel2007Serializator.ExtensionAttributeName,
                  Excel2007Serializator.ContentTypeAttributeName );
                break;

              case Excel2007Serializator.OverrideTagName:
                ParseDictionaryEntry( reader, contentOverrides, Excel2007Serializator.PartNameAttributeName,
                  Excel2007Serializator.ContentTypeAttributeName );
                break;

              default:
                throw new NotImplementedException( reader.LocalName );
            }

            reader.Skip();
          }
          else
          {
            reader.Read();
          }
        }
      }
      else
      {
        throw new XmlException( "Cannot locate appropriate xml tag" );
      }
    }
    /// <summary>
    /// Parses workbook part.
    /// </summary>
    /// <param name="reader">XmlReader to extract workbook part data from.</param>
    /// <param name="relations">Workbook relations.</param>
    /// <param name="holder">Object that stores document data.</param>
    /// <param name="bookPath">Absolute path in zip archive to the parent workbook.</param>
    /// <param name="streamStart">Stream that will get all xml tags before worksheets.</param>
    /// <param name="streamEnd">Stream that will get all xml tags after named ranges section.</param>
    /// <param name="lstBookViews">Workbook views collection.</param>
    public void ParseWorkbook( XmlReader reader, RelationCollection relations,
      FileDataHolder holder, string bookPath, Stream streamStart, Stream streamEnd,
      ref List<Dictionary<string, string>> lstBookViews, Stream functionGroups   )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( streamStart == null )
        throw new ArgumentNullException( "streamStart" );

      if( streamEnd == null )
        throw new ArgumentNullException( "streamEnd" );

      while( reader.NodeType != XmlNodeType.Element )
        reader.Read();

      if( reader.LocalName == Excel2007Serializator.WorkbookTagName )
      {
        bool bAdd = false;
        StreamWriter textWriter = new StreamWriter( streamStart );
        XmlWriter writer = UtilityMethods.CreateWriter( textWriter );
        writer.WriteStartElement( Excel2007Serializator.TemporaryRoot, Excel2007Serializator.XmlNamespaceMain );

        reader.Read();

        int iActiveSheetIndex = 0;
        int iDisplayedTab = 0;

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case Excel2007Serializator.DefinedNamesXmlTagName:
                m_book.ActiveSheetIndex = iActiveSheetIndex;
                ParseNamedRanges( reader );
                SwitchStreams( ref bAdd, ref writer, ref textWriter, streamEnd );
                break;

              case Excel2007Serializator.SheetsTagName:
                ParseSheetsOptions( reader, relations, holder, bookPath );
                SwitchStreams( ref bAdd, ref writer, ref textWriter, streamEnd );
                break;

              case Excel2007Serializator.WorkbookViewsTagName:
                lstBookViews = ParseBookViews( reader, out iActiveSheetIndex, out iDisplayedTab );
                SwitchStreams( ref bAdd, ref writer, ref textWriter, streamEnd );
                break;

              case Excel2007Serializator.CalcProperties:
                ParseCalcProperties( reader );
                break;

              case ExternalLinks.ExternalReferencesTag:
                ParseExternalLinksWorkbookPart( reader );
                break;

              case Protection.WorkbookProtectionTag:
                ParseWorkbookProtection( reader );
                break;

              case Excel2007Serializator.FileVersionTag:
                ParseFileVersion( reader, m_book.DataHolder.FileVersion );
                break;

              case Excel2007Serializator.FunctionGroups:
                XmlWriter writerFunctionGroups = UtilityMethods.CreateWriter( functionGroups, Encoding.UTF8 );
                writerFunctionGroups.WriteNode( reader, false );
                break;

              case Excel2007Serializator.WorkbookPr:
                ParseWorkbookPr( reader );
                break;

              case PivotTable.PivotCachesTag:
                ParsePivotCaches( reader );
                break;
                
              default:
                writer.WriteNode( reader, false );
                break;
            }
          }
          else
          {
            reader.Read();
          }
        }

        m_book.ActiveSheetIndex = iActiveSheetIndex;
        m_book.DisplayedTab = iDisplayedTab;

        writer.WriteEndElement();
        writer.Flush();
      }
      else
      {
        throw new XmlException( "Unexpected xml tag: " + reader.LocalName );
      }
    }
    /// <summary>
    /// Parses Meta Properties
    /// </summary>
    ///<param name="fileDataHolder">File Data Holder</param>
    ///<param name="reader">XML Reader</param>
    ///<param name="stream">DataStream</param>
    ///<param name="itemName">Item Name</param>
    internal void ParseMetaProperties(XmlReader reader, FileDataHolder fileDataHolder, Stream stream,string itemName)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        if (fileDataHolder == null)
            throw new ArgumentNullException("fileDataHolder");

        if (stream == null)
            throw new ArgumentNullException("stream");

        while (reader.NodeType != XmlNodeType.Element)
            reader.Read();

        switch (reader.LocalName)
        {
            case ContentTypeSchema:
                {
                    if (reader.LocalName == ContentTypeSchema && reader.NamespaceURI == ContentTypeNameSpace)
                    {
                        while (reader.Read())
                        {
                            if (reader.LocalName == ElementName)
                                parentElement = reader.GetAttribute(Name);

                            if (reader.LocalName == ComplexContent)
                            {
                                ParseChildElements(reader);
                            }
                            else
                            {
                                switch (reader.NodeType)
                                {
                                    case XmlNodeType.Element:
                                        {
                                            if (reader.LocalName == ElementName)
                                            {
                                                string name = reader.GetAttribute(Name);

                                                if (name == Excel2007Serializator.DocumentManagement)
                                                {
                                                    ParseDocumentManagmentSchema(reader, ref m_values);
                                                    break;
                                                }
                                                if (name != null && reader.NamespaceURI == XmlSchemaNameSpace && m_values.IndexOf(name) >= 0)
                                                {
                                                    string display = reader.GetAttribute(DisplayName);
                                                    string internalName = reader.GetAttribute(InternalName);

                                                    if(display!=null && internalName!=null)
                                                    {
                                                    MetaPropertyImpl proper = m_book.InnerContentTypeProperties.GetItemByInternalName(internalName) as MetaPropertyImpl;
                                                    if (proper != null)
                                                    {
                                                        proper.Name = display;
                                                        proper.ElementName = name;
                                                    }
                                                    else
                                                    {
                                                        MetaPropertyImpl property = new MetaPropertyImpl();
                                                        property.InternalName = internalName;
                                                        property.Name = display;
                                                        property.ElementName = name;

                                                        m_book.InnerContentTypeProperties.Add(property);
                                                    }
                                                    }
                                                }

                                            }
                                            break;
                                        }
                                    default:
                                        break;

                                }
                            }                            
                        }


                    }

                    byte[] bytes = new byte[stream.Length];

                    stream.Position = 0;

                    stream.Read(bytes, 0, (int)stream.Length);
#if !SILVERLIGHT && !WINRT && !WP
                    m_book.InnerContentTypeProperties.SchemaXml = Encoding.UTF8.GetString(bytes);
#else
                    m_book.InnerContentTypeProperties.SchemaXml = Encoding.Unicode.GetString(bytes, 0, bytes.Length);
#endif

                    break;

                }
            case Excel2007Serializator.Properties:
                {
                    reader.Read();
                    if (reader.NodeType == XmlNodeType.Whitespace)
                        reader.Read();
                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            switch (reader.LocalName)
                            {
                                case Excel2007Serializator.DocumentManagement:
                                    {
                                        m_book.InnerContentTypeProperties.ItemName = itemName;
                                        ParseDocumentManagementPropties(reader);
                                        break;
                                    }
                            }
                        }
                    }
                    break;
                }
        }

        
    }
    /// <summary>
    /// Parses DocumentManagement Schema Elements
    /// </summary>
    /// <param name="m_values">List of elements in DocumentManagement</param>
    /// <param name="reader">Xml Reader</param> 
    private static void ParseDocumentManagmentSchema(XmlReader reader, ref List<string> m_values)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        while (reader.NodeType != XmlNodeType.EndElement)
        {
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    {
                        if (reader.LocalName == ElementName)
                        {
                            string elementname = reader.GetAttribute(Reference);

                            if (elementname != null && elementname.Length > 0)
                                m_values.Add(elementname.Split(':')[1]);
                        }
                        break;
                    }

            }
            reader.Read();
        }
    }
    /// <summary>
    /// Parses DocumentManagement child Elements
    /// </summary>    
    /// <param name="reader">Xml Reader</param> 
    private void ParseChildElements(XmlReader reader)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        List<Stream> listChild = new List<Stream>();

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.NodeType == XmlNodeType.Whitespace)
                    reader.Read();
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        if (reader.LocalName == ElementName)
                        {
                            string name = reader.GetAttribute(Name);
                            if (name != null)
                            {
                                listChild.Add(ShapeParser.ReadNodeAsStream(reader));
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            reader.Read();
        }

        if(!m_book.m_childElements.ContainsKey(parentElement) && listChild.Count>0)
            m_book.m_childElements.Add(parentElement, listChild);
    }
    /// <summary>
    /// Parses workbook part.
    /// </summary>
    /// <param name="reader">Xml Reader</param>
    private void ParseDocumentManagementPropties(XmlReader reader)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        reader.Read();

        
        List<Stream> listChild = new List<Stream>();
        int childCount = 0;

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        {

                            if (m_values.IndexOf(reader.LocalName) >= 0)
                            {
                                string internalName = reader.LocalName;
                                string nameSpaceURI = reader.NamespaceURI;
                                string value = "";
                                try
                                {
                                    value = reader.ReadElementContentAsString();
                                }
                                catch (Exception exception)
                                {
                                    //m_book.InnerContentTypeProperties.IsValid = false;
                                    listChild = new List<Stream>();
                                    if (m_book.m_childElements.ContainsKey(internalName))
                                    {
                                        m_book.m_childElements.TryGetValue(internalName, out listChild);
                                        childCount = listChild.Count;
                                        listChild.Clear();

                                        m_book.m_childElements.Remove(internalName);
                                        for (int i = 0; i < childCount; i++)
                                        {
                                            listChild.Add(ShapeParser.ReadNodeAsStream(reader));
                                        }
                                        m_book.m_childElements.Add(internalName, listChild);
                                    }
                                    reader.Skip();
                                }

                                MetaPropertyImpl property = (MetaPropertyImpl)m_book.ContentTypeProperties.GetItemByInternalName(internalName);
                                if (property != null)
                                {
                                    property.Value = value;
                                    property.NameSpaceURI = nameSpaceURI;
                                }
                            }
                            else
                            {
                                string internalName = reader.LocalName;
                                string nameSpaceURI = reader.NamespaceURI;
                                string value = "";
                                try
                                {
                                    value = reader.ReadElementContentAsString();
                                }
                                catch (Exception exception)
                                {
                                    //m_book.InnerContentTypeProperties.IsValid = false;
                                    listChild = new List<Stream>();
                                    if (m_book.m_childElements.ContainsKey(internalName))
                                    {
                                        m_book.m_childElements.TryGetValue(internalName, out listChild);
                                        childCount = listChild.Count;
                                        listChild.Clear();

                                        m_book.m_childElements.Remove(internalName);
                                        for (int i = 0; i < childCount; i++)
                                        {
                                            listChild.Add(ShapeParser.ReadNodeAsStream(reader));
                                        }
                                        m_book.m_childElements.Add(internalName, listChild);
                                    }
                                    reader.Skip();
                                }

                                MetaPropertyImpl property = new MetaPropertyImpl();
                                property.Value = value;
                                property.NameSpaceURI = nameSpaceURI;
                                property.InternalName =internalName;
                                m_book.InnerContentTypeProperties.Add(property);
                                
                            }
                            break;
                        }

                        reader.Read();
                }
        }

    }
    public void ParsePivotTables()
    {
        WorksheetsCollection sheets = m_book.Worksheets as WorksheetsCollection;
        foreach (WorksheetImpl sheet in sheets)
        {
            
                sheet.DataHolder.ParsePivotTable(sheet);
        }
    }
    public void ParseWorksheets( Dictionary<int, int> dictUpdatedSSTIndexes, bool parseOnDemand )
    {
      ITabSheets arrSheets = m_book.Objects;
      ApplicationImpl app = m_book.AppImplementation;
      int maxProgressEvent = arrSheets.Count + AdditionalProgressItems;
      app.RaiseProgressEvent( AdditionalProgressItems, maxProgressEvent );

      app.IsFormulaParsed = false;
      for( int i = 0, len = arrSheets.Count; i < len; i++ )
      {
        WorksheetBaseImpl sheet = ( WorksheetBaseImpl )arrSheets[ i ];

        if ((sheet as WorksheetImpl) != null)
            (sheet as WorksheetImpl).ParseDataOnDemand = parseOnDemand;
        else if ((sheet as ChartImpl) != null)
            (sheet as ChartImpl).ParseDataOnDemand = parseOnDemand;

        sheet.ParseData( dictUpdatedSSTIndexes);
        sheet.IsSaved = false;
        app.RaiseProgressEvent( i + AdditionalProgressItems + 1, maxProgressEvent );
      }
      app.IsFormulaParsed = true;
    }
    /// <summary>
    /// Extracts pivot caches from the reader.
    /// </summary>
    /// <param name="reader">Xml reader to get pivot caches data from.</param>
    private void ParsePivotCaches( XmlReader reader )
    {
      /*
       <pivotCaches>
  <pivotCache cacheId="1" r:id="rId8" /> 
  </pivotCaches
       */

      if( reader.LocalName != PivotTable.PivotCachesTag )
        throw new XmlException();

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case PivotTable.PivotCacheTag:
                ParsePivotCache( reader );
                break;

              default:
                reader.Skip();
                break;
            }
          }
          else
          {
            reader.Skip();
          }
        }
      }

      reader.Read();
    }
    /// <summary>
    /// Extracts pivot cache from the reader.
    /// </summary>
    /// <param name="reader">Xml reader to get data from.</param>
    private void ParsePivotCache( XmlReader reader )
    {
      // <pivotCache cacheId="1" r:id="rId8" /> 

      if( reader.LocalName != PivotTable.PivotCacheTag )
        throw new XmlException();

      string cacheId = null;
      string relationId = null;

      if( reader.MoveToAttribute( PivotTable.PivotCacheId ) )
        cacheId = reader.Value;

      if( reader.MoveToAttribute( Excel2007Serializator.RelationAttribute, Excel2007Serializator.RelationNamespace ) )
        relationId = reader.Value;

      m_book.DataHolder.RegisterCache( cacheId, relationId );
      reader.MoveToElement();
      reader.Skip();
    }

    private void ParseFileVersion( XmlReader reader, FileVersion fileVersion )
    {
      if( reader.LocalName != Excel2007Serializator.FileVersionTag )
        throw new XmlException();

      fileVersion.ApplicationName = ( reader.MoveToAttribute( Excel2007Serializator.ApplicationNameAttribute ) ) ?
        fileVersion.ApplicationName = reader.Value :
        null;

      fileVersion.BuildVersion = ( reader.MoveToAttribute( Excel2007Serializator.RupBuild ) ) ?
        reader.Value :
        null;

      fileVersion.LowestEdited = ( reader.MoveToAttribute( Excel2007Serializator.LowestEdited ) ) ?
        reader.Value :
        null;

      fileVersion.LastEdited = ( reader.MoveToAttribute( Excel2007Serializator.LastEdited ) ) ?
        reader.Value :
        null;

      fileVersion.CodeName = ( reader.MoveToAttribute( Excel2007Serializator.CodeName ) ) ?
        reader.Value :
        null;

      reader.MoveToElement();
      reader.Skip();
    }
    /// <summary>
    /// Parses workbook Pr tag and parses Date1904 value
    /// </summary>
    /// <param name="reader">XmlReader to extract workbook part data from.</param>
    private void ParseWorkbookPr( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentException( "reader" );

      if( reader.LocalName != Excel2007Serializator.WorkbookPr )
        throw new XmlException();

      if( reader.MoveToAttribute( Excel2007Serializator.WorkbookDate1904 ) )
        m_book.Date1904 = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( Excel2007Serializator.CodeName ) )
        m_book.CodeName = reader.Value;
      if (reader.MoveToAttribute(Excel2007Serializator.HidePivotFieldList))
          m_book.HidePivotFieldList = !XmlConvert.ToBoolean(reader.Value);
      if (reader.MoveToAttribute(Excel2007Serializator.DefaultThemeVersion))
          m_book.DefaultThemeVersion = reader.Value;

      reader.Skip();
    }
    /// <summary>
    /// Parses calculation tag and reads Precision
    /// </summary>
    /// <param name="reader">XmlReader to extract workbook part data from.</param>
    private void ParseCalcProperties( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentException( "reader" );

      if( reader.LocalName != Excel2007Serializator.CalcProperties )
        throw new XmlException();

      if( reader.MoveToAttribute( Excel2007Serializator.WorkbookPrecision ) )
        m_book.PrecisionAsDisplayed = !XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( Excel2007Serializator.CalculationId ) )
        m_book.DataHolder.CalculationId = reader.Value;

      reader.Skip();
    }
    /// <summary>
    /// Extracts workbook protection options.
    /// </summary>
    /// <param name="reader">XmlReader to extract protection options from.</param>
    private void ParseWorkbookProtection( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.LocalName != Protection.WorkbookProtectionTag )
        throw new XmlException();

      bool bStructure = false;
      bool bWindows = false;
      ushort usPassword = 0;

      if( reader.MoveToAttribute( Protection.WorkbookPassword ) )
        usPassword = ushort.Parse( reader.Value, NumberStyles.HexNumber, CultureInfo.CurrentCulture );

      if( reader.MoveToAttribute( Protection.LockStructureTag ) )
        bStructure = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( Protection.LockWindowsTag ) )
        bWindows = XmlConvert.ToBoolean( reader.Value );

      reader.Read();

      if( bStructure || bWindows )
        m_book.Protect( bWindows, bStructure );

      m_book.Password.IsPassword = usPassword;
    }
    /// <summary>
    /// Parses workbook views.
    /// </summary>
    /// <param name="reader">Reader to get data from.</param>
    /// <param name="iActiveSheetIndex">Active sheet index.</param>
    /// <param name="iDisplayedTab">Display tab index.</param>
    /// <returns>Workbook views collection.</returns>
    private List<Dictionary<string, string>> ParseBookViews( XmlReader reader,
      out int iActiveSheetIndex, out int iDisplayedTab )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      reader.Read();

      iActiveSheetIndex = 0;
      iDisplayedTab = 0;

      List<Dictionary<string, string>> lstBookViews = new List<Dictionary<string, string>>();
      Dictionary<string, string> dicBookView;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.LocalName == Excel2007Serializator.WorkbookViewTagName )
        {
          dicBookView = ParseWorkbookView( reader );
          lstBookViews.Add( dicBookView );
        }

        reader.Skip();
      }

      dicBookView = lstBookViews[ 0 ];
      string strValue;

      if( dicBookView.TryGetValue( Excel2007Serializator.ActiveSheetIndexAttributeName, out strValue ) )
        iActiveSheetIndex = XmlConvert.ToInt32( strValue );

      if( dicBookView.TryGetValue( Excel2007Serializator.FirstSheetAttributeName, out strValue ) )
        iDisplayedTab = XmlConvert.ToInt32( strValue );

      reader.Skip();

      return lstBookViews;
    }
    /// <summary>
    /// Parses workbook view.
    /// </summary>
    /// <param name="reader">Reader to get data from.</param>
    /// <returns>Dictionary where key - attribute name, value - attribute value.</returns>
    private Dictionary<string, string> ParseWorkbookView( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      Dictionary<string, string> dicWorkbookView = new Dictionary<string, string>();

      for( int iIndex = 0, iCount = reader.AttributeCount; iIndex < iCount; iIndex++ )
      {
        reader.MoveToAttribute( iIndex );
        dicWorkbookView.Add( reader.Name, reader.Value );
      }

      return dicWorkbookView;
    }
    /// <summary>
    /// Parses sheet (chart of worksheet).
    /// </summary>
    /// <param name="reader">Reader to get data from.</param>
    /// <param name="sheet">Sheet to parse.</param>
    /// <param name="strParentPath">Absolute path to the parent worksheet item.</param>
    /// <param name="streamStart">This stream will receive xml text starting just
    /// after "worksheet" tag to "col" or "sheetData" tag.</param>
    /// <param name="streamCF">This stream contains conditional formatting.</param>
    /// <param name="arrStyles">List with new style indexes (index - old
    /// style index, value - new one).</param>
    /// <param name="dictItemsToRemove">Dictionary with archive items to remove after parsing.</param>
    public void ParseSheet( XmlReader reader, WorksheetImpl sheet, string strParentPath, ref MemoryStream streamStart,
      ref MemoryStream streamCF, List<int> arrStyles, Dictionary<string, object> dictItemsToRemove, Dictionary<int, int> dictUpdatedSSTIndexes )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( streamStart == null )
        throw new ArgumentNullException( "streamStart" );

      while( reader.NodeType != XmlNodeType.Element )
        reader.Read();

      if( reader.LocalName != Excel2007Serializator.WorksheetTagName )
        throw new XmlException( Excel2007Serializator.WorksheetTagName + " tag was not found." );

      reader.Read();
      m_workSheet = sheet;
      ParseSheetBeforeData( reader, sheet, streamStart, arrStyles );

      if( reader.LocalName == Excel2007Serializator.SheetDataTagName )
      {
        ParseSheetData( reader, sheet, arrStyles, Excel2007Serializator.CellTagName );
      }

      if( dictUpdatedSSTIndexes != null )
        sheet.UpdateLabelSSTIndexes( dictUpdatedSSTIndexes, sheet.ParentWorkbook.InnerSST.AddIncrease );

      ParseAfterSheetData( reader, sheet, ref streamCF, strParentPath, dictItemsToRemove );
    }
    /// <summary>
    /// Parses worksheet before sheetData tag.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="sheet"></param>
    /// <param name="streamStart"></param>
    /// <param name="arrStyles"></param>
    private void ParseSheetBeforeData( XmlReader reader, WorksheetImpl sheet, Stream streamStart, List<int> arrStyles )
    {
      XmlWriter writer = UtilityMethods.CreateWriter( streamStart, Encoding.UTF8 );
      writer.WriteStartElement( Excel2007Serializator.TemporaryRoot, Excel2007Serializator.XmlNamespaceMain );

      while( !reader.EOF && reader.NodeType != XmlNodeType.EndElement &&
        reader.LocalName != Excel2007Serializator.SheetDataTagName )
      {
        switch( reader.LocalName )
        {
          case Excel2007Serializator.ColsTagName:
            ParseColumns( reader, sheet, arrStyles );
            reader.Read();
            break;

          case Excel2007Serializator.SheetLevelPropertiesTagName:
            ParseSheetLevelProperties( reader, sheet );
            break;

          case Excel2007Serializator.SheetViewsTag:
            ParseSheetViews( reader, sheet );
            break;

          case Excel2007Serializator.DimensionTagName:
            reader.Skip();
            break;

          case Excel2007Serializator.SheetFormatPropertiesTag:
            ExtractDefaultRowHeight( reader, sheet );
            ExtractZeroHeight(reader, sheet);
            reader.MoveToElement();
            goto default;

          default:
            writer.WriteNode( reader, false );
            break;
        }
      }

      writer.WriteEndElement();
      writer.Flush();
      streamStart.Position = 0;
    }
    /// <summary>
    /// Parses sheet views.
    /// </summary>
    /// <param name="reader">Reader to get data from.</param>
    /// <param name="sheet">Worksheet to place extracted data into.</param>
    private void ParseSheetViews( XmlReader reader, WorksheetBaseImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( reader.LocalName != Excel2007Serializator.SheetViewsTag )
        throw new XmlException( "Wrong xml tag" );

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          switch( reader.LocalName )
          {
            case Excel2007Serializator.SheetViewTag:
              ParseSheetView( reader, sheet );
              break;

            default:
              reader.Skip();
              break;
          }
        }
      }

      reader.Read();
    }
    /// <summary>
    /// Parses single sheet view item.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="sheet">Worksheet to put extracted data into.</param>
    private void ParseSheetView( XmlReader reader, WorksheetBaseImpl sheetBase )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheetBase == null )
        throw new ArgumentNullException( "sheetBase" );

      if( reader.LocalName != Excel2007Serializator.SheetViewTag )
        throw new XmlException( "Wrong xml tag" );

      WorksheetImpl sheet = sheetBase as WorksheetImpl;

      if( reader.MoveToAttribute( Excel2007Serializator.ShowGridLines ) )
      {
        sheet.IsGridLinesVisible = XmlConvert.ToBoolean( reader.Value );
      }

      if( reader.MoveToAttribute( Pane.TopLeftCell ) )
      {
        sheet.TopLeftCell = sheet[ reader.Value ];
      }

      if( reader.MoveToAttribute( Excel2007Serializator.ShowZeros ) )
      {
        sheet.IsDisplayZeros = XmlConvert.ToBoolean( reader.Value );
      }

      if( reader.MoveToAttribute( Excel2007Serializator.ShowRowColHeaders ) )
      {
        sheet.IsRowColumnHeadersVisible = XmlConvert.ToBoolean( reader.Value );
      }

      if( reader.MoveToAttribute( Excel2007Serializator.RightToLeft ) )
      {
        sheet.IsRightToLeft = XmlConvert.ToBoolean( reader.Value );
      }

      if( reader.MoveToAttribute( Excel2007Serializator.SheetZoomScale ) )
      {
        sheetBase.Zoom = XmlConvert.ToInt32( reader.Value );
      }

      if( reader.MoveToAttribute( ChartConstants.ZoomToFit ) )
      {
        ( sheetBase as ChartImpl ).ZoomToFit = XmlConvert.ToBoolean( reader.Value );
      }

      if( reader.MoveToAttribute( Excel2007Serializator.SheetGridColor ) )
      {
        sheet.WindowTwo.IsDefaultHeader = XmlConvert.ToBoolean( reader.Value );
      }

      if( reader.MoveToAttribute( Excel2007Serializator.ColorID ) )
      {
        sheet.GridLineColor = ( ExcelKnownColors )XmlConvert.ToInt32( reader.Value );
      }

       if (reader.MoveToAttribute(Excel2007Serializator.ViewTag))
      {         
          
          switch(reader.Value)
          {
              case Excel2007Serializator.Layout:
                  sheet.View = SheetView.PageLayout;
                  break;
              case Excel2007Serializator.PageBreakPreview:
                  sheet.View = SheetView.PageBreakPreview;
                  sheet.WindowTwo.IsSavedInPageBreakPreview = true;
                  break;
              case Excel2007Serializator.Normal:
                  sheet.View = SheetView.Normal;
                  break;
           }         
      }

      if( reader.MoveToAttribute( Excel2007Serializator.TabSelected ) && reader.Value != "0" )
        sheetBase.Select();

      reader.MoveToElement();

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          switch( reader.LocalName )
          {
            case Pane.TagName:
              if (reader.IsEmptyElement && !reader.HasAttributes)
                  reader.Read();
              else
                  ParsePane(reader, sheet);
              break;

            case Pane.Selection:
              ParseSelection( reader, sheet );
              break;

            default:
              reader.Skip();
              break;
          }
        }
      }

      reader.Read();
    }
    /// <summary>
    /// Extracts selection data.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="sheet">Worksheet to put extracted data into.</param>
    private void ParseSelection( XmlReader reader, WorksheetImpl sheet )
    {
      //IRange activeRange = sheet.GetActiveCell();

      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.LocalName != Pane.Selection )
        throw new XmlException();

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( reader.MoveToAttribute( Pane.TagName ) )
      {
        sheet.Pane.ActivePane = ( ushort )GetPaneType( reader.Value );
      }

      if( reader.MoveToAttribute( Pane.ActiveCell ) )
      {
        string address = reader.Value;
        sheet.SetActiveCell( sheet.Range[ address ], false );
      }

      reader.MoveToElement();
      reader.Skip();
    }
    private Pane.ActivePane GetPaneType( string value )
    {
      return Pane.PaneStrings[ value ];
    }
    /// <summary>
    /// Extracts pane objects.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="sheet">Worksheet to put extracted data into.</param>
    private void ParsePane( XmlReader reader, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( reader.LocalName != Pane.TagName )
        throw new XmlException( "Wrong xml tag" );

      if( reader.MoveToAttribute( Pane.XSplit ) )
        sheet.VerticalSplit = XmlConvert.ToInt32( reader.Value );

      if( reader.MoveToAttribute( Pane.YSplit ) )
        sheet.HorizontalSplit = XmlConvert.ToInt32( reader.Value );

      if( reader.MoveToAttribute( Pane.TopLeftCell ) )
      {
        string cellName = reader.Value;
        sheet.PaneFirstVisible = sheet[ cellName ];
        //int iRow;
        //int iColumn;
        //RangeImpl.CellNameToRowColumn( cellName, out iRow, out iColumn );
        //pane.FirstRow = iRow - 1;
        //pane.FirstColumn = iColumn - 1;
      }

      if( reader.MoveToAttribute( Pane.Active ) )
      {
        Pane.ActivePane activePane = ( Pane.ActivePane )Enum.Parse( typeof( Pane.ActivePane ),
          reader.Value, false );
        sheet.ActivePane = ( int )activePane;
      }

      if( reader.MoveToAttribute( Pane.State ) )
      {
        WindowTwoRecord windowTwo = sheet.WindowTwo;
        ParsePaneState( windowTwo, reader.Value );
      }
    }
    /// <summary>
    /// Parses pane state.
    /// </summary>
    /// <param name="windowTwo">WindowTwo record that stores pane state flags.</param>
    /// <param name="state">State value to parse</param>
    private void ParsePaneState( WindowTwoRecord windowTwo, string state )
    {
      if( windowTwo == null )
        throw new ArgumentNullException( "windowTwo" );

      if( state == null )
        throw new ArgumentNullException( "state" );

      switch( state )
      {
        case Pane.StateFrozen:
          windowTwo.IsFreezePanes = true;
          windowTwo.IsFreezePanesNoSplit = true;
          break;

        case Pane.StateFrozenSplit:
          windowTwo.IsFreezePanes = true;
          windowTwo.IsFreezePanesNoSplit = false;
          break;

        case Pane.StateSplit:
          windowTwo.IsFreezePanes = false;
          windowTwo.IsFreezePanesNoSplit = false;
          break;

        default:
          throw new XmlException();
      }
    }
    /// <summary>
    /// Extracts chart sheet from XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to extract chart sheet data from.</param>
    /// <param name="chart">Chart toi fill with data.</param>
    public void ParseChartsheet( XmlReader reader, ChartImpl chart )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      while( reader.NodeType != XmlNodeType.Element )
        reader.Read();

      if( reader.LocalName != ChartConstants.ChartsheetTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();
      PageSetupBaseImpl pageSetup = chart.PageSetupBase;
      pageSetup.IsNotValidSettings = true;
      bool bTempValue;
      //chart.PageSetupBase.Orientation = ExcelPageOrientation.Landscape;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case Excel2007Serializator.SheetLevelPropertiesTagName:
              ParseSheetLevelProperties( reader, chart );
              break;

            case Excel2007Serializator.SheetViewsTag:
              ParseSheetViews( reader, chart );
              break;

            case PageSetup.PageMarginsTag:
              //ParsePageSetup( reader, chart.PageSetup );
              bTempValue = pageSetup.IsNotValidSettings;
              ParsePageMargins( reader, chart.PageSetup, new WorksheetPageSetupConstants() );
              pageSetup.IsNotValidSettings = bTempValue;
              break;

            case PageSetup.PageSetupTag:
              ParsePageSetup( reader, chart.PageSetupBase );
              break;

            case PageSetup.HeaderFooterTag:
              bTempValue = pageSetup.IsNotValidSettings;
              ParseHeaderFooter( reader, chart.PageSetupBase );
              pageSetup.IsNotValidSettings = bTempValue;
              break;

            case Drawings.DrawingTagName:
              ParseChartDrawing( reader, chart );
              break;

            case Vml.LegacyDrawing:
              ParseLegacyDrawing( reader, chart );
              break;

            case Vml.LegacyDrawingHF:
              ParseLegacyDrawingHF( reader, chart, null );
              break;

            case Protection.SheetProtectionTag:
              ParseSheetProtection( reader, chart, Protection.ContentAttribute );
              break;

            default:
              reader.Skip();
              break;
          }
        }
        else
        {
          reader.Skip();
        }
      }
    }
    /// <summary>
    /// Parses drawing associated with chartsheet.
    /// </summary>
    /// <param name="reader">XmlReader that contains drawing data.</param>
    /// <param name="chart">Chart to put extracted data into.</param>
    private void ParseChartDrawing( XmlReader reader, ChartImpl chart )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( chart == null )
        throw new ArgumentNullException( "chart" );

      if( reader.LocalName != Drawings.DrawingTagName )
        throw new XmlException( "Unexpected xml tag." );

      if( !reader.MoveToAttribute( Excel2007Serializator.RelationAttribute, Excel2007Serializator.RelationNamespace ) )
        throw new XmlException();

      string strRelationId = reader.Value;
      Relation relation = chart.m_dataHolder.Relations[ strRelationId ];
      chart.m_dataHolder.Relations.Remove( strRelationId );

      if( relation == null )
        throw new XmlException();

      string strTarget = relation.Target;
      FileDataHolder dataHolder = chart.m_dataHolder.ParentHolder;
      string strPath;
      string strItemName = chart.m_dataHolder.ArchiveItem.ItemName;
      FileDataHolder.SeparateItemName( strItemName, out strPath );
      XmlReader chartReader = dataHolder.CreateReader( relation, strPath, out strItemName );
      string strRelations = FileDataHolder.GetCorrespondingRelations( strItemName );
      RelationCollection relations = dataHolder.ParseRelations( strRelations );

      while( chartReader.LocalName != ChartConstants.ChartTag )
      {
        if( chartReader.NodeType == XmlNodeType.Element && chartReader.LocalName == Drawings.AbsoluteAnchorTag )
        {
          Size extent = ParseAbsoluteAnchorExtent( chartReader );
          chart.Width = extent.Width;
          chart.Height = extent.Height;
        }
        else
        {
          chartReader.Read();
        }
      }

      ParseChartTag( chartReader, chart, relations, dataHolder, strItemName );
    }

    private Size ParseAbsoluteAnchorExtent( XmlReader reader )
    {
      while( reader.LocalName != Drawings.Extents )
      {
        reader.Read();
      }

      Size extent = ParseExtent( reader );
      return extent;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="chart"></param>
    /// <param name="relations"></param>
    /// <param name="dataHolder"></param>
    /// <param name="itemName"></param>
    private void ParseChartTag( XmlReader reader, ChartImpl chart,
      RelationCollection relations, FileDataHolder dataHolder, string itemName )
    {
      if( !reader.MoveToAttribute( Excel2007Serializator.RelationAttribute, Excel2007Serializator.RelationNamespace ) )
        throw new XmlException();

      string strRelationId = reader.Value;
      string strPath;
      Relation relation = relations[ strRelationId ];
      FileDataHolder.SeparateItemName( itemName, out strPath );

      if (relation != null)
      {
          string strItemPath;
          XmlReader chartReader = dataHolder.CreateReader(relation, strPath, out strItemPath);

          string strRelations = FileDataHolder.GetCorrespondingRelations(strItemPath);
          RelationCollection chartRelations = dataHolder.ParseRelations(strRelations);

          if (chartRelations != null)
          {
              IEnumerator enumerator = chartRelations.GetEnumerator();
              while (enumerator.MoveNext())
              {
                  object obj = enumerator.Current;
                  KeyValuePair<string, Relation> keyValue = (KeyValuePair<string, Relation>)obj;
                  chart.Relations[keyValue.Key] = keyValue.Value;
              }
              chart.Relations.ItemPath = chartRelations.ItemPath;
          }
          ChartParser parser = new ChartParser(m_book);
          parser.ParseChart(chartReader, chart, chartRelations);
          dataHolder.Archive.RemoveItem(itemName);
          relations.Remove(strRelationId);
      }
    }
    /// <summary>
    /// Extracts default row height from sheet format properties tag.
    /// </summary>
    /// <param name="reader">XmlReader to get info from.</param>
    /// <param name="sheet">Worksheet to put settings into.</param>
    private void ExtractDefaultRowHeight( XmlReader reader, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if (reader.MoveToAttribute(Excel2007Serializator.DefaultColumWidthAttribute))
      {
          double value= XmlConvert.ToDouble(reader.Value);

          int iWidth = (int)Math.Round(value * 256);
          iWidth = sheet.EvaluateRealColumnWidth( iWidth );
          value = (iWidth / 256.0);
          sheet.StandardWidth = value > 0 ? value : sheet.StandardWidth;

          double defaultColumnWidth = Helper.ParseDouble(reader.Value);
          this.SetDefaultColumnWidth(defaultColumnWidth, false, sheet);
      }
      if( reader.MoveToAttribute( Excel2007Serializator.DefaultRowHeightAttribute ) )
      {
        sheet.StandardHeight = XmlConvert.ToDouble( reader.Value );
      }
      sheet.CustomHeight = false;
      if (reader.MoveToAttribute(Excel2007Serializator.RowCustomHeightAttributeName))
      {
          sheet.CustomHeight = XmlConvert.ToBoolean(reader.Value);
      }
        if (reader .MoveToAttribute (Excel2007Serializator .OutlineLevelColAttribute ))
            sheet .OutlineLevelColumn = XmlConvert .ToByte (reader .Value );
        if (reader.MoveToAttribute(Excel2007Serializator.OutlineLevelRowAttribute))
            sheet.OutlineLevelRow = XmlConvert.ToByte(reader.Value);
      if (reader.MoveToAttribute(Excel2007Serializator.BaseColWidthAttribute))
          sheet.BaseColumnWidth = XmlConvert.ToInt16(reader.Value);
      if (reader.MoveToAttribute(Excel2007Serializator.ThickBottomAttribute))
          sheet.IsThickBottom = XmlConvert.ToBoolean(reader.Value);
      if (reader.MoveToAttribute(Excel2007Serializator.ThickTopAttribute))
          sheet.IsThickTop = XmlConvert.ToBoolean(reader.Value);
      reader.MoveToElement();
    }
    private void SetDefaultColumnWidth(double defaultWidth, bool bool_2, WorksheetImpl worksheet)
    {
        if (Math.Abs((double)(defaultWidth - 0.0)) < 0.0001)
        {
            worksheet.Columnss.Width = defaultWidth;
        }
        else
        {
            int num = worksheet.GetAppImpl().GetFontCalc2();
            double num2 = defaultWidth * num;
            if (bool_2)
            {
                num2 += 10.0;
            }
            if (num2 > 5.0)
            {
                worksheet.Columnss.Width = ((num2 - 5.0) / ((double)num));
            }
            else
            {
                worksheet.Columnss.Width = 0.0;
                ColumnCollection columns = worksheet.Columnss;
                if (columns.column == null)
                {
                    columns.GetOrCreateColumn().SetWidth((int)(num2 + 0.5));
                }
            }
        }
    }
    /// <summary>
    /// Extracts Zero row height from sheet format properties tag.
    /// </summary>
    /// <param name="reader">XmlReader to get info from.</param>
    /// <param name="sheet">Worksheet to put settings into.</param>
    private void ExtractZeroHeight(XmlReader reader, WorksheetImpl sheet)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        if (sheet == null)
            throw new ArgumentNullException("sheet");

        if (reader.MoveToAttribute(Excel2007Serializator.ZeroHeightAttribute))
        {
            sheet.IsZeroHeight = XmlConvert.ToBoolean(reader.Value);
            (sheet.PageSetup as PageSetupImpl).DefaultRowHeightFlag = false;
        }

        reader.MoveToElement();
    }
    /// <summary>
    /// This methods extracts merged cells information from XmlReader.
    /// </summary>
    /// <param name="reader">Reader to get merge information from.</param>
    /// <param name="sheet">Parent worksheet.</param>
    public void ParseMergedCells( XmlReader reader, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( reader.NodeType == XmlNodeType.Element )
      {
        if( reader.LocalName == Excel2007Serializator.MergeCellsXmlTagName )
        {
          reader.Read();
          while( reader.NodeType != XmlNodeType.EndElement )
          {
            if( reader.NodeType == XmlNodeType.Element )
            {
              ParseMergeRegion( reader, sheet );
            }
            else
            {
              reader.Read();
            }
          }

          reader.Read();
        }
      }
    }
    /// <summary>
    /// This methods extracts named ranges from XmlReader.
    /// </summary>
    /// <param name="reader">Reader to get information from.</param>
    public void ParseNamedRanges( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.NodeType == XmlNodeType.Element )
      {
        if( reader.LocalName == Excel2007Serializator.DefinedNamesXmlTagName )
        {
          if( !reader.IsEmptyElement )
          {
            reader.Read();

            List<string> arrValues = new List<string>();

            while( reader.NodeType != XmlNodeType.EndElement )
            {
              if( reader.NodeType == XmlNodeType.Element )
              {
                string value = ParseNamedRange( reader );
                arrValues.Add( value );
              }

              reader.Read();
            }

            INames names = m_book.Names;
            m_book.AppImplementation.IsFormulaParsed = false;
            for( int i = 0, len = arrValues.Count; i < len; i++ )
            {
              NameImpl nameImpl = ( NameImpl )names[ i ];
              int iExclamationIndex = arrValues[i].LastIndexOf('!');
              if (iExclamationIndex == 0)
              {
                  nameImpl.IsCommon = true;
                  if (names[i].Scope == "Workbook" && m_book.ActiveSheet != null)
                  {
                      arrValues[i] = m_book.ActiveSheet.Name + arrValues[i];                      
                  }
                  else
                  {
                      arrValues[i] = names[i].Scope + arrValues[i];
                  }
              }
              nameImpl.SetValue( m_formulaUtil.ParseString( arrValues[ i ] ) );
              //names[ i ].Value = arrValues[ i ];
            }
          }
          m_book.AppImplementation.IsFormulaParsed = true;

          reader.Read();
        }
      }
    }
    /// <summary>
    /// Parses styles of the workbook.
    /// </summary>
    /// <param name="reader">XmlReader to read styles data from.</param>
    /// <param name="streamDxfs">Stream that will get Dxfs formatting.</param>
    public List<int> ParseStyles( XmlReader reader, ref Stream streamDxfs )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      while( reader.NodeType != XmlNodeType.Element )
        reader.Read();

      if( reader.LocalName != Excel2007Serializator.StyleSheetTagName )
        throw new XmlException( "Unexpected xml tag " + reader.LocalName );

      List<int> arrNewFontIndexes = null;
      List<BordersCollection> arrBorders = null;
      List<FillImpl> arrFills = null;
      List<int> arrNewCellStyles = null;
      List<int> arrNewCellFormats = null;

      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        bool bReadNext = true;

        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case Excel2007Serializator.NumberFormatsTagName:
              ParseNumberFormats( reader );
              break;

            case Excel2007Serializator.FontsTagName:
              arrNewFontIndexes = ParseFonts( reader );
              break;

            case Excel2007Serializator.FillsTagName:
              arrFills = ParseFills( reader );
              break;

            case Excel2007Serializator.BordersTagName:
              arrBorders = ParseBorders( reader );
              break;

            case Excel2007Serializator.NamedStyleXFsTagName:
              arrNewCellStyles = ParseNamedStyles( reader, arrNewFontIndexes, arrFills, arrBorders );
              break;

            case Excel2007Serializator.CellFormatXFsTagName:
              arrNewCellFormats = ParseCellFormats( reader, arrNewFontIndexes, arrFills,
                arrBorders, arrNewCellStyles );
              break;

            case Excel2007Serializator.CellStylesTagName:
              ParseStyles( reader, arrNewCellStyles );
              break;

            case Excel2007Serializator.DiffXFsTagName:
              // Here we only are saving conditional formatting without parsing.
              streamDxfs = new MemoryStream();
              StreamWriter streamWriter = new StreamWriter( streamDxfs );
              XmlWriter writer = UtilityMethods.CreateWriter( streamWriter );
              writer.WriteNode( reader, false );
              writer.Flush();
              bReadNext = false;
              break;

            case Excel2007Serializator.TableStylesTagName:
              // Here, we are saving the custom table styles without parsing.
              //TODO: Need to have parsed support for custom table styling.
              m_book.CustomTableStylesStream = ShapeParser.ReadNodeAsStream(reader);
              bReadNext = false;
              break;

            case Excel2007Serializator.ColorsTagName:
              ParseColors( reader );
              break;

            case Excel2007Serializator.Extensionlist:
              ParseBookExtensions( reader );
              bReadNext = false;
              break;

            default:
              throw new NotImplementedException(reader.LocalName);
            //break;
          }
        }

        if( bReadNext )
          reader.Read();
      }

      if (m_book.InnerStyles.Count == 0)
      {
          List<StyleRecord> arrStyles = new List<StyleRecord>();
          m_book.PrepareStyles(false, arrStyles, null);
      }

      ExtendedFormatWrapper wrap = ( ExtendedFormatWrapper )m_book.InnerStyles[ "Normal" ];
      ExtendedFormatImpl defaultFormat = wrap.Wrapped.CreateChildFormat();
      defaultFormat = m_book.InnerExtFormats.Add( defaultFormat );
      m_book.DefaultXFIndex = defaultFormat.Index;

      return arrNewCellFormats;
    }

    private void ParseBookExtensions( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.LocalName != Excel2007Serializator.Extensionlist )
        throw new XmlException();

      Stream extensions = ShapeParser.ReadNodeAsStream( reader, true );
      m_book.DataHolder.ExtensionStream = extensions;
    }
    /// <summary>
    /// Parses shared strings table.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <returns>Dictionary with updated string indexes.</returns>
    public Dictionary<int, int> ParseSST( XmlReader reader, bool parseOnDemand )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      while( reader.NodeType != XmlNodeType.Element )
        reader.Read();

      if( reader.LocalName != Excel2007Serializator.SharedStringTableTagName )
        throw new XmlException( "reader" );

      if( reader.IsEmptyElement ) return null;
      m_book.SSTStream = ShapeParser.ReadNodeAsStream(reader);

      if (parseOnDemand)
          m_book.ParseOnDemand = parseOnDemand;

      m_book.SSTStream.Position = 0;
      XmlReader sstReader = UtilityMethods.CreateReader(m_book.SSTStream);

      sstReader.Read();
      int iFileStringIndex = 0;
      Dictionary<int, int> result = new Dictionary<int, int>();
      SSTDictionary sst = m_book.InnerSST;

      while (sstReader.NodeType != XmlNodeType.EndElement && sstReader.NodeType != XmlNodeType.None)
      {
          if (sstReader.LocalName == Excel2007Serializator.StringItemTagName)
        {
          int currentIndex = ParseStringItem( sstReader );

          if( iFileStringIndex != currentIndex )
            result[ iFileStringIndex ] = currentIndex;

          iFileStringIndex++;
        }

          sstReader.Skip();
      }

      sst.UpdateRefCounts();
      return result;
    }
    /// <summary>
    /// Parses string item.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <returns>Added string index.</returns>
    public int ParseStringItem( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.LocalName != Excel2007Serializator.StringItemTagName &&
        reader.LocalName != Excel2007Serializator.RichTextInlineTagName )
      {
        throw new XmlException( "reader" );
      }

      bool bSetCount = ( reader.LocalName == Excel2007Serializator.RichTextInlineTagName );

      int result = -1;
      reader.Read();

      if( reader.IsEmptyElement )
      {
        result = m_book.InnerSST.AddIncrease( string.Empty, false );
        reader.Skip();
      }
      else
      {
        while( reader.NodeType != XmlNodeType.EndElement )
        {
          switch( reader.LocalName )
          {
            case Excel2007Serializator.TextTagName:
              result = ParseText( reader, bSetCount );
              break;

            case Excel2007Serializator.RichTextRunTagName:
              result = ParseRichTextRun( reader );
              break;

            default:
              //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, reader.LocalName, "Not supported xml tag" );
              //throw new NotSupportedException( "Not supported tag." );
              reader.Skip();
              break;
          }
        }
      }

      return result;
    }
    /// <summary>
    /// Parses string item.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <returns>Added string index.</returns>
    public int ParseStringItem(XmlReader reader,out string text)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        if (reader.LocalName != Excel2007Serializator.StringItemTagName &&
          reader.LocalName != Excel2007Serializator.RichTextInlineTagName)
        {
            throw new XmlException("reader");
        }

        bool bSetCount = (reader.LocalName == Excel2007Serializator.RichTextInlineTagName);

        int result = -1;
        reader.Read();
        text = string.Empty;
        if (reader.IsEmptyElement)
        {
            result = m_book.InnerSST.AddIncrease(string.Empty, false);
            reader.Skip();
        }
        else
        {
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                switch (reader.LocalName)
                {
                    case Excel2007Serializator.TextTagName:
                        result = ParseText(reader, bSetCount,out text);
                        break;

                    case Excel2007Serializator.RichTextRunTagName:
                        result = ParseRichTextRun(reader);
                        break;

                    default:
                        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, reader.LocalName, "Not supported xml tag" );
                        //throw new NotSupportedException( "Not supported tag." );
                        reader.Skip();
                        break;
                }
            }
        }

        return result;
    }
    /// <summary>
    /// Parses Vml shapes.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="sheet">Worksheet to put extracted shapes into.</param>
    /// <param name="relations">Corresponding relations collection.</param>
    /// <param name="parentItemPath">Path to the parent item.</param>
    public void ParseVmlShapes( XmlReader reader, ShapeCollectionBase shapes,
      RelationCollection relations, string parentItemPath )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( shapes == null )
        throw new ArgumentNullException( "shapes" );

      //reader.Read();

      while( reader.NodeType != XmlNodeType.Element )
      {
        reader.Read();

        if( reader.NodeType == XmlNodeType.None )
          break;
      }

      if( reader.LocalName != Vml.XmlTagName )
        throw new XmlException( "Unexpected tag" );

      reader.Read();

      Dictionary<string, ShapeImpl> dictShapeIdToShape = new Dictionary<string, ShapeImpl>();
      Stream shapeLayoutStream = null;
      while( reader.NodeType != XmlNodeType.EndElement && !reader.EOF )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case Vml.ShapeTypeTagName:
              ParseShapeType( reader, shapes, dictShapeIdToShape,shapeLayoutStream );
              break;

            case Vml.ShapeTagName:
              if (reader.MoveToAttribute(Vml.TypeAttributeName))
                  ParseShape(reader, dictShapeIdToShape, relations, parentItemPath);
              else
                  ParseShapeWithoutType(reader, shapes, relations, parentItemPath);
              break;
            case Vml.ShapeLayoutTagName:
              shapeLayoutStream = ShapeParser.ReadNodeAsStream(reader);
              shapeLayoutStream.Position = 0;                  
              break;
            default:
              reader.Skip();
              break;
            //throw new NotSupportedException();
          }
        }
        else
        {
          reader.Read();
        }
      }
    }
    /// <summary>
    /// Extracts relations collection from XmlReader.
    /// </summary>
    /// <param name="reader">Reader to extract data from.</param>
    /// <returns>Extracted relations collection.</returns>
    public RelationCollection ParseRelations( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      RelationCollection result = new RelationCollection();

      while( reader.NodeType != XmlNodeType.Element )
        reader.Read();

      if( reader.LocalName != Excel2007Serializator.RelationsTagName )
        throw new XmlException( "Unexpected tag " + reader.LocalName );

      reader.Read();
      if (reader.NodeType != XmlNodeType.None)
      {
          while (reader.NodeType != XmlNodeType.EndElement)
          {
              if (reader.NodeType == XmlNodeType.Element)
              {
                  if (reader.LocalName == Excel2007Serializator.RelationTagName)
                  {
                      ParseRelation(reader, result);
                  }
                  else
                  {
                      throw new XmlException("Unexpected tag " + reader.Value);
                  }
              }

              reader.Read();
          }
      }
      return result;
    }
    /// <summary>
    /// Extracts sheet data from reader and insets it into worksheet.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="sheet">Worksheet to put data into.</param>
    /// <param name="arrStyles">List with new style indexes (index - old style index, value - new one).</param>
    /// <param name="cellTag">Tag used for cell definition.</param>
    /// <returns>Dictionary with all attributes of sheetData tag.</returns>
    public Dictionary<string, string> ParseSheetData( XmlReader reader, IInternalWorksheet sheet,
      List<int> arrStyles, string cellTag )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( reader.LocalName != Excel2007Serializator.SheetDataTagName )
        throw new XmlException( "reader" );

      reader.MoveToElement();
      Dictionary<string, string> dicAttributes = null;

      if(m_outlineLevels == null)
          m_outlineLevels = new Dictionary<int, List<Point>>();
 
      m_outlineWrapperUtility = new OutlineWrapperUtility(m_outlineLevels);

      if ((sheet as WorksheetImpl )!= null && (sheet as WorksheetImpl).RowOutlineLevels == null)
          (sheet as WorksheetImpl).RowOutlineLevels = new Dictionary<int, List<Point>>();

      if( reader.MoveToFirstAttribute() )
      {
        dicAttributes = new Dictionary<string, string>();
        dicAttributes.Add( reader.LocalName, reader.Value );

        while( reader.MoveToNextAttribute() )
        {
          dicAttributes.Add( reader.LocalName, reader.Value );
        }

        reader.MoveToElement();
      }

      if( !reader.IsEmptyElement )
      {
        reader.Read();
        int generateRowIndex = 1;

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.LocalName == Excel2007Serializator.RowTagName )
          {
            generateRowIndex = ParseRow( reader, sheet, arrStyles, cellTag, generateRowIndex );
            generateRowIndex++;
          }

          reader.Skip();
        }
      }

      reader.Read();

      if (((sheet as WorksheetImpl )!= null) && m_indexAndLevels!=null && m_indexAndLevels.Count>0)
      {
          m_outlineWrapperUtility.UpdateOutlineRowStorage(sheet as WorksheetImpl, m_indexAndLevels);
          m_indexAndLevels = null;
          m_outlineLevels = null;
      }
      return dicAttributes;
    }
    /// <summary>
    /// Parse xml document with comments data (author, text).
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="sheet">Worksheet to put extracted comments data into.</param>
    public void ParseComments( XmlReader reader, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      do
      {
        reader.Read();
      }
      while( reader.NodeType != XmlNodeType.Element );

      if( reader.LocalName == Excel2007Serializator.CommentsTagName )
        reader.Read();

      List<string> arrAuthors = null;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case Excel2007Serializator.CommentAuthorsTagName:
              arrAuthors = ParseAuthors( reader );
              break;

            case Excel2007Serializator.CommentListTagName:
              ParseCommentList( reader, arrAuthors, sheet );
              break;

            default:
              throw new XmlException( "Unexpected xml tag." );
          }
        }
        else
        {
          reader.Skip();
        }
      }
    }
    /// <summary>
    /// This method extracts drawings from specified XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="sheet">Worksheet to place extracted shapes into.</param>
    /// <param name="drawingsPath">Absolute path to the drawings.</param>
    /// <param name="lstRelationIds">List that will get relation id of the picture
    /// (used to remove parsed relations after parsing).</param>
    /// <param name="dictItemsToRemove">Dictionary with archive items to remove after parsing.</param>
    public void ParseDrawings( XmlReader reader, WorksheetBaseImpl sheet,
      string drawingsPath, List<string> lstRelationIds, Dictionary<string, object> dictItemsToRemove )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( lstRelationIds == null )
        throw new ArgumentNullException( "lstRelationdIds" );

      while( reader.NodeType != XmlNodeType.Element )
        reader.Read();

      if( reader.LocalName != Drawings.WorksheetDrawings && reader.LocalName != ChartConstants.UserShapesTag )
        throw new XmlException( "Unexpected xml tag " + reader.LocalName );

      reader.Read();

      if( reader.NodeType != XmlNodeType.None )
      {
        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
              string localName = reader.LocalName;
              m_drawingParser = new DrawingParser();
              m_drawingParser.anchorName = localName;
              switch (localName)
            {
              case Drawings.TwoCellAnchorTagName:
              case Drawings.OneCellAnchorTagName:
              case ChartConstants.RelativeSizeAnchorTag:
                ParseTwoCellAnchor( reader, sheet, drawingsPath, lstRelationIds, dictItemsToRemove );
                break;

            case Drawings.AlternateContentTag:
              ParseAlternateContent( reader, sheet, drawingsPath, lstRelationIds, dictItemsToRemove );
              break;
            
            case Drawings.AbsoluteAnchorTag:                    
              Rectangle extent = new  Rectangle();
              double xPos=0;
              double yPos=0;
              double width = 0;
              double height=0;
              while (reader.LocalName != Drawings.GraphicDataTag)
              {
                  if (reader.NodeType == XmlNodeType.Element && reader.LocalName == Drawings.AbsoluteAnchorTag)
                  {                     

                      while (reader.LocalName != Drawings.Extents)
                      {
                          if (reader.LocalName == Drawings.PositionTag)
                          {
                              if (reader.MoveToAttribute(Drawings.XAttributeName))
                                  xPos = reader.ReadContentAsInt();
                              if (reader.MoveToAttribute(Drawings.YAttributeName))
                                  yPos = reader.ReadContentAsInt();

                              m_drawingParser.posX = Helper.ConvertEmuToOffset((int)xPos, this.dpiX);
                              m_drawingParser.posY = Helper.ConvertEmuToOffset((int)yPos, this.dpiX);
                          }
                          reader.Read();
                      }  
                     
                      if( reader.MoveToAttribute( Drawings.CXAttributeName ) )
                          width = int.Parse( reader.Value );

                      if( reader.MoveToAttribute( Drawings.CYAttributeName ) )
                           height = int.Parse( reader.Value );

                      m_drawingParser.cx = Helper.ConvertEmuToOffset((int)width, this.dpiX);
                      m_drawingParser.cy = Helper.ConvertEmuToOffset((int)height, this.dpiX);
                  }
                  else
                  {
                      reader.Read();
                  }
              }
              MemoryStream  data = ReadSingleNodeIntoStream( reader );
              ShapeImpl shape = TryParseChart(data, sheet, drawingsPath);
              shape.IsAbsoluteAnchor = true ;
              if (shape == null)
              {
                  shape = new ShapeImpl(sheet.Application, sheet.InnerShapes);
                  sheet.InnerShapes.AddShape(shape);
              }
              else
              {
                  data = null;
                  (shape as ChartShapeImpl).ChartObject.EMUWidth = width;
                  (shape as ChartShapeImpl).ChartObject.EMUHeight = height;
                  (shape as  IChart).XPos= xPos;
                  (shape as IChart).YPos = yPos;
              }
              while (reader.LocalName != Drawings.AbsoluteAnchorTag)// && reader.NodeType != XmlNodeType.EndElement)
              {
                  reader.Read();
              }
              reader.Read();
              break;
                    

              default:
                //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, reader.LocalName, "Not supported xml tag." );
                reader.Skip();
                break;
              //throw new XmlException( "Notsupported xml tag " + reader.LocalName );
            }
          }
          else
          {
            reader.Skip();
          }
        }
      }
    }

    private void ParseAlternateContent(XmlReader reader, WorksheetBaseImpl sheet, string drawingsPath,
      List<string> lstRelationIds, Dictionary<string, object> dictItemsToRemove)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        if (sheet == null)
            throw new ArgumentNullException("sheet");

        if (lstRelationIds == null)
            throw new ArgumentNullException("lstRelationIds");

        reader.Read();
        m_enableAlternateContent = true;

        while (reader.NodeType != XmlNodeType.EndElement)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.LocalName)
                {
                    case Drawings.ChoiceTag:
                        ParseChoice(reader, sheet, drawingsPath, lstRelationIds, dictItemsToRemove);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            else
            {
                reader.Skip();
            }
        }
        reader.Read();
    }

    private void ParseAlternateContent(XmlReader reader, WorksheetImpl sheet)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        if (sheet == null)
            throw new ArgumentNullException("sheet");

        sheet.HasAlternateContent = true;
        reader.Read();
        while (reader.NodeType != XmlNodeType.EndElement)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.LocalName)
                {
                    case Drawings.ChoiceTag:
                        ParseChoice(reader, sheet);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            else
            {
                reader.Skip();
            }
        }
        reader.Read();
    }

    private void ParseChoice(XmlReader reader, WorksheetBaseImpl sheet, string drawingsPath,
      List<string> lstRelationIds, Dictionary<string, object> dictItemsToRemove)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        if (sheet == null)
            throw new ArgumentNullException("sheet");

        if (lstRelationIds == null)
            throw new ArgumentNullException("lstRelationIds");

        reader.Read();
        while (reader.NodeType != XmlNodeType.EndElement)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.LocalName)
                {
                    case Drawings.TwoCellAnchorTagName:
                    case Drawings.OneCellAnchorTagName:
                    case ChartConstants.RelativeSizeAnchorTag:
                        ParseTwoCellAnchor(reader, sheet, drawingsPath, lstRelationIds, dictItemsToRemove);
                        break;

                    default:
                        reader.Skip();
                        break;
                }
            }

            else
            {
                reader.Skip();
            }
        }
        reader.Read();
    }

    private void ParseChoice(XmlReader reader, WorksheetImpl sheet)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        if (sheet == null)
            throw new ArgumentNullException("sheet");

        reader.Read();
        while (reader.NodeType != XmlNodeType.EndElement)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.LocalName)
                {
                    case Drawings.ControlsTag:
                        ParseControls(reader, sheet);
                        break;
#if !SILVERLIGHT && !WINRT && !WP
                    case Vml.OleObject:
                        OleObjects lstOleObjects = ( OleObjects )sheet.OleObjects;
                        OleObject newOle = ParseOleObject(reader, sheet);
                        lstOleObjects.Add(newOle);
                        break;
#endif
                    default:
                        reader.Skip();
                        break;
                }
            }

            else
            {
                reader.Skip();
            }
        }
        reader.Read();
    }
    /// <summary>
    /// This method extracts all xml tags after SheetData tag.
    /// </summary>
    /// <param name="reader">Reader to get data from.</param>
    /// <param name="sheet">Sheet to parse.</param>
    /// <param name="streamCF">This stream contains conditional formatting.</param>
    /// <param name="strParentPath">Absolute path to the parent worksheet item.</param>
    /// <param name="dictItemsToRemove">Dictionary with archive items to remove after parsing.</param>
    private void ParseAfterSheetData( XmlReader reader, WorksheetImpl sheet,
      ref MemoryStream streamCF, string strParentPath, Dictionary<string, object> dictItemsToRemove )
    {
      //Stream cfStream = streamCF;
      streamCF = new MemoryStream();
      XmlWriter writerCf = UtilityMethods.CreateWriter( streamCF, Encoding.UTF8 );
      writerCf.WriteStartElement( Excel2007Serializator.TemporaryRoot, Excel2007Serializator.XmlNamespaceMain );

      while( !reader.EOF && reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case Excel2007Serializator.MergeCellsXmlTagName:
              ParseMergedCells( reader, sheet );
              break;

            case Excel2007Serializator.PhoneticPr:
              // We don't support this tag yet.
              reader.Skip();
              break;

            case Vml.LegacyDrawing:
              ParseLegacyDrawing( reader, sheet );
              break;
# if !SILVERLIGHT && !WINRT && !WP
            case Vml.OleObjects:
              ParseOleObjects( reader, sheet );
              break;
# endif
            case Vml.LegacyDrawingHF:
              ParseLegacyDrawingHF( reader, sheet, null );
              break;

            case Drawings.DrawingTagName:
              ParseDrawings( reader, sheet, dictItemsToRemove );
              break;

            case CF.ConditionalFormattingTagName:
              writerCf.WriteNode( reader, false );
              break;

            case Excel2007Serializator.BackgroundImageTagName:
              ParseBackgroundImage( reader, sheet, strParentPath );
              break;

            case DV.DataValidationsTagName:
              ParseDataValidations( reader, sheet );
              break;

            case AF.AutoFilterSettingsTagName:
              ParseAutoFilters( reader, sheet );
              break;

            case Excel2007Serializator.HyperlinksTagName:
              ParseHyperlinks( reader, sheet );
              break;

            case PageSetup.PrintOptionsTag:
              ParsePrintOptions( reader, sheet.PageSetup );
              break;

            case PageSetup.PageMarginsTag:
              ParsePageMargins( reader, sheet.PageSetup, new WorksheetPageSetupConstants() );
              break;

            case PageSetup.PageSetupTag:
              ParsePageSetup( reader, ( PageSetupImpl )sheet.PageSetup );
              break;

            case PageSetup.HeaderFooterTag:
              ParseHeaderFooter( reader, ( PageSetupBaseImpl )sheet.PageSetup );
              break;

            case Excel2007Serializator.HorizontalPageBreaksTagName:
              ParseHorizontalPagebreaks( reader, sheet );
              break;

            case Excel2007Serializator.VerticalPageBreaksTagName:
              ParseVerticalPagebreaks( reader, sheet );
              break;

            case Excel2007Serializator.CustomPropertiesTagName:
              ParseCustomWorksheetProperties( reader, sheet );
              break;

            case Excel2007Serializator.IgnoredErrorsTag:
              ParseIgnoreError( reader, sheet );
              break;

            case Protection.SheetProtectionTag:
              ParseSheetProtection( reader, sheet, Protection.SheetAttribute );
              break;

            case Drawings.AlternateContentTag:
              ParseAlternateContent( reader, sheet );
              break;

            case Drawings.ControlsTag:
              ParseControls( reader, sheet );
              break;

            case ListObjects.TableParts:
              ParseTableParts( reader, sheet, strParentPath );
              break;

            case Excel2007Serializator.Extensionlist:
              ParseExtensionlist( reader, sheet );
              break;

            default:
              reader.Skip();
              break;
          }
        }
        else
        {
          reader.Skip();
        }
      }

      writerCf.WriteEndElement();
      writerCf.Flush();
      streamCF.Position = 0;
    }

    /// <summary>
    /// Parses the extensionlist.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="sheet">The sheet.</param>
    private void ParseExtensionlist( XmlReader reader, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( reader.LocalName != Excel2007Serializator.Extensionlist )
        throw new XmlException();

      if( !reader.IsEmptyElement )
      {
        reader.Read();
        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case Vml.Ext:
                ParseExt( sheet, reader );
                break;

              default:
                reader.Skip();
                break;
            }
          }
          else
          {
            reader.Skip();
          }
        }
      }

      reader.Read();

    }

    /// <summary>
    /// Parses the ext.
    /// </summary>
    /// <param name="sheet">The sheet.</param>
    /// <param name="reader">The reader.</param>
    private void ParseExt( WorksheetImpl sheet, XmlReader reader )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.LocalName != Vml.Ext )
        throw new XmlException();

      if( !reader.IsEmptyElement )
      {
        reader.Read();
        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case SparkConstants.SparklineGroupsTag:
                ParseSparklineGroups( sheet, reader );
                break;
              case Excel2007Serializator.SlicerList:
                Stream stream = ShapeParser.ReadNodeAsStream(reader);
                sheet.WorksheetSlicerStream = stream;
                break;
              
              case CF.ConditionalFormattingsTagName:
                (sheet.DataHolder as WorksheetDataHolder).m_cfsStream = new MemoryStream();
                (sheet.DataHolder as WorksheetDataHolder).m_cfsStream = ShapeParser.ReadNodeAsStream(reader);
                break;

              default:
                reader.Skip();
                break;
            }
          }
          else
          {
            reader.Skip();
          }
        }
      }

      reader.Read();

    }

    /// <summary>
    /// Parses the sparkline groups.
    /// </summary>
    /// <param name="sheet">The sheet.</param>
    /// <param name="reader">The reader.</param>
    private void ParseSparklineGroups( WorksheetImpl sheet, XmlReader reader )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.LocalName != SparkConstants.SparklineGroupsTag )
        throw new XmlException();

      if( !reader.IsEmptyElement )
      {
        reader.Read();
        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case SparkConstants.SparklineGroupTag:
                sheet.SparklineGroups.Add( ParseSparklineGroup( reader, sheet ) );
                reader.Read();
                break;

              default:
                reader.Skip();
                break;
            }
          }
          else
          {
            reader.Skip();
          }
        }
      }
    }

    /// <summary>
    /// Parses the sparkline group.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="sheet">The sheet.</param>
    private SparklineGroup ParseSparklineGroup( XmlReader reader, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( reader.LocalName != SparkConstants.SparklineGroupTag )
        throw new XmlException();

      SparklineGroup sparklineGroup = new SparklineGroup( sheet.ParentWorkbook );
      ColorObject colorObject;

      if( reader.MoveToAttribute( SparkConstants.LineWeightAttribute ) )
        sparklineGroup.LineWeight = XmlConvert.ToDouble( reader.Value );

      if( reader.MoveToAttribute( SparkConstants.SparklineTypeAttribute ) )
      {
        switch( reader.Value )
        {
          case "line":
            sparklineGroup.SparklineType = SparklineType.Line;
            break;
          case "column":
            sparklineGroup.SparklineType = SparklineType.Column;
            break;
          case "stacked":
            sparklineGroup.SparklineType = SparklineType.ColumnStacked100;
            break;
        }
      }

      if( reader.MoveToAttribute( SparkConstants.DateAxisAttribute ) )
        sparklineGroup.HorizontalDateAxis = true;

      if( reader.MoveToAttribute( SparkConstants.DisplayEmptyCellsAttribute ) )
      {
        switch( reader.Value )
        {
          case "span":
            sparklineGroup.DisplayEmptyCellsAs = SparklineEmptyCells.Line;
            break;
          case "gap":
            sparklineGroup.DisplayEmptyCellsAs = SparklineEmptyCells.Gaps;
            break;
          case "zero":
            sparklineGroup.DisplayEmptyCellsAs = SparklineEmptyCells.Zero;
            break;
        }
      }

      if( reader.MoveToAttribute( SparkConstants.MarkersAttribute ) )
        sparklineGroup.ShowMarkers = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( SparkConstants.HighAttribute ) )
        sparklineGroup.ShowHighPoint = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( SparkConstants.LowAttribute ) )
        sparklineGroup.ShowLowPoint = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( SparkConstants.FirstAttribute ) )
        sparklineGroup.ShowFirstPoint = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( SparkConstants.LastAttribute ) )
        sparklineGroup.ShowLastPoint = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( SparkConstants.NegativeAttribute ) )
        sparklineGroup.ShowNegativePoint = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( SparkConstants.DisplayAxisAttribute ) )
        sparklineGroup.DisplayAxis = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( SparkConstants.DisplayHiddenAttribute ) )
        sparklineGroup.DisplayHiddenRC = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( SparkConstants.VerticalMaxAxisTypeAttr ) )
      {
        switch( reader.Value )
        {
          case "individual":
            sparklineGroup.VerticalAxisMaximum.VerticalAxisOptions = SparklineVerticalAxisOptions.Automatic;
            break;
          case "group":
            sparklineGroup.VerticalAxisMaximum.VerticalAxisOptions = SparklineVerticalAxisOptions.Same;
            break;
          case "custom":
            sparklineGroup.VerticalAxisMaximum.VerticalAxisOptions = SparklineVerticalAxisOptions.Custom;
            break;
        }
      }

      if( reader.MoveToAttribute( SparkConstants.VerticalMinAxisTypeAttr ) )
      {
        switch( reader.Value )
        {
          case "individual":
            sparklineGroup.VerticalAxisMinimum.VerticalAxisOptions = SparklineVerticalAxisOptions.Automatic;
            break;
          case "group":
            sparklineGroup.VerticalAxisMinimum.VerticalAxisOptions = SparklineVerticalAxisOptions.Same;
            break;
          case "custom":
            sparklineGroup.VerticalAxisMinimum.VerticalAxisOptions = SparklineVerticalAxisOptions.Custom;
            break;
        }
      }
      if (reader.MoveToAttribute(SparkConstants.VerticalMaxAttribute))
          sparklineGroup.VerticalAxisMaximum.CustomValue = Convert.ToDouble(reader.Value);

      if (reader.MoveToAttribute(SparkConstants.VerticalMinAttribute))
          sparklineGroup.VerticalAxisMinimum.CustomValue = Convert.ToDouble(reader.Value);

      if( reader.MoveToAttribute( SparkConstants.PlotRighttoLeftAttribute ) )
        sparklineGroup.PlotRightToLeft = true;

      if( !reader.IsEmptyElement )
      {
        reader.Read();
        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case SparkConstants.ColorSeriesTag:
                sparklineGroup.SparklineColor = ParseColor( reader ).GetRGB( ( IWorkbook )m_book );
                break;
              case SparkConstants.ColorNegativeTag:
                sparklineGroup.NegativePointColor = ParseColor( reader ).GetRGB( ( IWorkbook )m_book );
                break;
              case SparkConstants.ColorAxisTag:
                sparklineGroup.AxisColor = ParseColor( reader ).GetRGB( ( IWorkbook )m_book );
                break;
              case SparkConstants.ColorMarkersTag:
                sparklineGroup.MarkersColor = ParseColor( reader ).GetRGB( ( IWorkbook )m_book );
                break;
              case SparkConstants.ColorFirstTag:
                sparklineGroup.FirstPointColor = ParseColor( reader ).GetRGB( ( IWorkbook )m_book );
                break;
              case SparkConstants.ColorLastTag:
                sparklineGroup.LastPointColor = ParseColor( reader ).GetRGB( ( IWorkbook )m_book );
                break;
              case SparkConstants.ColorHighTag:
                sparklineGroup.HighPointColor = ParseColor( reader ).GetRGB( ( IWorkbook )m_book );
                break;
              case SparkConstants.ColorLowTag:
                sparklineGroup.LowPointColor = ParseColor( reader ).GetRGB( ( IWorkbook )m_book );
                break;
              case Excel2007Serializator.FormulaTagName:
                bool bStringEmpty = reader.IsEmptyElement;
                if( !bStringEmpty )
                  reader.Read();

                sparklineGroup.HorizontalDateAxisRange = sheet.Range[ reader.Value ];

                if( !bStringEmpty )
                  reader.Skip();

                reader.Skip();

                break;
              case SparkConstants.SparklinesTag:
                sparklineGroup.Add( ParseSparklines( reader, sheet ) );
                break;
              default:
                reader.Read();
                break;
            }
          }
          else
          {
            reader.Read();
          }
        }
      }
      reader.Read();
      return sparklineGroup;

    }

    /// <summary>
    /// Parses the sparklines.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="sheet">The sheet.</param>
    /// <returns></returns>
    private Sparklines ParseSparklines( XmlReader reader, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( reader.LocalName != SparkConstants.SparklinesTag )
        throw new XmlException();

      Sparklines sparklines = new Sparklines();
      if( !reader.IsEmptyElement )
      {
        reader.Read();
        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case SparkConstants.SparklineTag:
                sparklines.Add( ParseSparkline( reader, sheet ) );
                reader.Read();
                break;
              default:
                reader.Skip();
                break;
            }
          }
          else
          {
            reader.Skip();
          }
        }
      }
      return sparklines;
    }

    /// <summary>
    /// Parses the sparkline.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="sheet">The sheet.</param>
    /// <returns></returns>
    private Sparkline ParseSparkline( XmlReader reader, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( reader.LocalName != SparkConstants.SparklineTag )
        throw new XmlException();

      Sparkline sparkline = new Sparkline();
      if( !reader.IsEmptyElement )
      {
        reader.Read();
        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case Excel2007Serializator.FormulaTagName:
                bool bStringEmpty = reader.IsEmptyElement;
                if( !bStringEmpty )
                  reader.Read();
                sparkline.DataRange = sheet.Range[ reader.Value ];

                if( !bStringEmpty )
                  reader.Skip();

                reader.Skip();

                break;
              case Excel2007Serializator.RangeReferenceAttribute:
                bStringEmpty = reader.IsEmptyElement;
                if( !bStringEmpty )
                  reader.Read();

                sparkline.ReferenceRange = sheet.Range[ reader.Value ];

                if( !bStringEmpty )
                  reader.Skip();

                reader.Read();
                break;
              default:
                reader.Skip();
                break;
            }
          }
          else
          {
            reader.Skip();
          }
        }
      }
      return sparkline;
    }
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Parses the OLE objects.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="sheet">The sheet.</param>
    private void ParseOleObjects( XmlReader reader, WorksheetBaseImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( reader.LocalName != Vml.OleObjects )
        throw new XmlException();

      WorksheetImpl worksheet = sheet as WorksheetImpl;

      if( worksheet == null )
        return;

      OleObjects lstOleObjects = ( OleObjects )worksheet.OleObjects;

      if( !reader.IsEmptyElement )
      {
        reader.Read();
        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case Vml.OleObject:
                OleObject newOle = ParseOleObject( reader, worksheet );
                lstOleObjects.Add( newOle );
                break;
              case Drawings.AlternateContentTag:
                ParseAlternateContent(reader, worksheet);
                break;
                    
              default:
                reader.Skip();
                break;
            }
          }
          else
          {
            reader.Skip();
          }
        }
      }
    }

    /// <summary>
    /// Parses the OLE object.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="sheet">The sheet.</param>
    /// <param name="oleObject">The OLE object.</param>
    private OleObject ParseOleObject( XmlReader reader, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( reader.LocalName != Vml.OleObject )
        throw new XmlException();

      OleObject oleObject = new OleObject( sheet );

      if( reader.MoveToAttribute( Vml.ProgramID ) )
      {          
          oleObject.OleObjectType = OleTypeConvertor.ToOleType(reader.Value);
      }

      if( reader.MoveToAttribute( Vml.DevAspect ) && reader.Value == DVAspect.DVASPECT_ICON.ToString() )
      {
        oleObject.DisplayAsIcon = true;
      }

      if( reader.MoveToAttribute( Excel2007Serializator.RelationAttribute, Excel2007Serializator.RelationNamespace ) )
      {
        string relationId = reader.Value;
        WorksheetDataHolder holder = sheet.DataHolder;
        holder.ParseOleData( sheet, relationId, oleObject );
        oleObject.ShapeRId = relationId;
        oleObject.OleType = OleLinkType.Embed;
      }
      else if( reader.MoveToAttribute( Vml.LinkAttribute ) )
      {
        oleObject.OleType = OleLinkType.Link;
        string fileName = ParseExternName( reader.Value, oleObject );
        //oleObject.SetFile( fileName );
        oleObject.FileName = fileName;
      }
      else
      {
        throw new XmlException();
      }

      if( reader.MoveToAttribute( "shapeId" ) )
      {
        oleObject.ShapeID = XmlConvert.ToInt32( reader.Value );
      }
      reader.Read();
      if (reader.LocalName != Vml.OleObject)
      {
          while (reader.NodeType != XmlNodeType.EndElement)
          {
              if (reader.NodeType == XmlNodeType.Element)
              {
                  switch (reader.LocalName)
                  {
                      case "objectPr":
                          reader.Skip();
                          break;
                  }
              }
          }
          reader.Read();
      }
      return oleObject;
    }

    private string ParseExternName( string link, OleObject oleObject )
    {
      // [linkid+1]!'name'
      int iDelimIndex = link.IndexOf( '!' );

      if( iDelimIndex < 0 )
        throw new XmlException();

      string bookPart = link.Substring( 0, iDelimIndex );
      string namePart = link.Substring( iDelimIndex + 1, link.Length - iDelimIndex - 1 );

      if( bookPart[ 0 ] != '[' || bookPart[ bookPart.Length - 1 ] != ']' )
        throw new XmlException();

      bookPart = bookPart.Substring( 1, bookPart.Length - 2 );

      if( namePart[ 0 ] == '\'' && namePart[ namePart.Length - 1 ] == '\'' )
        namePart = namePart.Substring( 1, namePart.Length - 2 );

      int bookIndex = int.Parse( bookPart );
      namePart = namePart.Replace( "''", "'" );

      ExternWorkbookImpl book = m_book.ExternWorkbooks[ bookIndex - 1 ];
      //ExternNameImpl name = book.ExternNames[ namePart ];
      //return name.
      return book.URL;
    }
# endif
    /// <summary>
    /// Extracts all table parts for the specified worksheet.
    /// </summary>
    /// <param name="reader">Reader to get table parts from.</param>
    /// <param name="sheet">Worksheet to put extracted table parts into.</param>
    /// <param name="sheetPath">Path to the parent sheet object.</param>
    private void ParseTableParts( XmlReader reader, WorksheetImpl sheet, string sheetPath )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( reader == null )
        throw new ArgumentNullException( "reader" );

      List<string> result = new List<string>();

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case ListObjects.TablePart:
                ParseTablePart( reader, sheet, sheetPath );
                break;

              default:
                reader.Skip();
                break;
            }
          }
          else
          {
            reader.Skip();
          }
        }
      }

      reader.Read();
    }
    /// <summary>
    /// Extracts table part.
    /// </summary>
    /// <param name="reader">Reader to get table part from.</param>
    /// <param name="sheet">Worksheet to put extracted table part into.</param>
    /// <param name="sheetPath">Path to the parent sheet object.</param>
    private string ParseTablePart( XmlReader reader, WorksheetImpl sheet, string sheetPath )
    {
      if( reader == null )
        throw new ArgumentException( "reader" );

      if( sheet == null )
        throw new ArgumentException( "sheet" );

      if( reader.LocalName != ListObjects.TablePart )
        throw new XmlException();

      if( !reader.MoveToAttribute( ListObjects.IdAttribute, Excel2007Serializator.RelationNamespace ) )
        throw new XmlException();

      string strRelation = reader.Value;
      sheet.DataHolder.ParseTablePart( sheet, strRelation, sheetPath );

      reader.MoveToElement();
      reader.Skip();
      return strRelation;
    }
    /// <summary>
    /// Extracts controls tag from a reader.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="sheet"></param>
    private void ParseControls( XmlReader reader, WorksheetImpl sheet )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.LocalName != Drawings.ControlsTag )
        throw new XmlException();

      WorksheetDataHolder holder = sheet.DataHolder;
      MemoryStream stream = new MemoryStream();
      XmlWriter writer = UtilityMethods.CreateWriter( stream, Encoding.UTF8 );
      writer.WriteNode( reader, false );
      writer.Flush();
      holder.ControlsStream = stream;
    }
    /// <summary>
    /// Extracts protection options.
    /// </summary>
    /// <param name="reader">XmlReader to extract from.</param>
    /// <param name="sheet">Worksheet to put extracted protection options into.</param>
    private void ParseSheetProtection( XmlReader reader, WorksheetBaseImpl sheet, string protectContentTag )
    {
      if( reader == null )
        throw new ArgumentNullException( "writer" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( reader.LocalName != Protection.SheetProtectionTag )
        throw new XmlException();

      ExcelSheetProtection protection = ExcelSheetProtection.None;
      ushort usPassword = 0;

      if( reader.MoveToAttribute( Protection.PasswordAttribute ) )
      {
        string password = reader.Value;
        usPassword = ushort.Parse( password, NumberStyles.AllowHexSpecifier );
      }

      //SerializeAttribute( writer, "sheet", value, false );
      bool bProtectContent = false;

      if( reader.MoveToAttribute( protectContentTag ) )
      {
        bProtectContent = XmlConvert.ToBoolean( reader.Value );
        if (usPassword == 0)
            usPassword = 1;
      }

      string[] attributes = Protection.ProtectionAttributes;
      ChecProtectionDelegate protectionChecker = CheckProtectionAttribute;


      if( sheet is ChartImpl )
      {
        attributes = Protection.ChartProtectionAttributes;
        protectionChecker = CheckChartProtectionAttribute;
      }

      for( int i = 0, len = attributes.Length; i < len; i++ )
      {
        protection = protectionChecker( reader,
          attributes[ i ],
          Protection.ProtectionFlags[ i ],
          Protection.DefaultValues[ i ],
          protection );
      }

      //if( bProtectContent )
      {
        sheet.Protect( usPassword, protection );
        sheet.ProtectContents = bProtectContent;
      }

      reader.Read();
    }
    delegate ExcelSheetProtection ChecProtectionDelegate( XmlReader reader, string attributeName,
      ExcelSheetProtection flag, bool defaultValue, ExcelSheetProtection protection );
    /// <summary>
    /// Checks single protection attribute and updates protection flags as necessary.
    /// </summary>
    /// <param name="reader">XmlReader to get attribute value from.</param>
    /// <param name="attributeName">Attribute name to check.</param>
    /// <param name="flag">Flag value that corresponds to the protection attribute.</param>
    /// <param name="defaultValue">Default value of the attribute.</param>
    /// <param name="protection">Current protection settings.</param>
    /// <returns>Updated protection settings.</returns>
    private ExcelSheetProtection CheckChartProtectionAttribute( XmlReader reader, string attributeName,
      ExcelSheetProtection flag, bool defaultValue, ExcelSheetProtection protection )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( attributeName == null || attributeName.Length == 0 )
        throw new ArgumentOutOfRangeException( "attributeName" );

      bool flagValue = defaultValue;

      if( reader.MoveToAttribute( attributeName ) )
      {
        flagValue = XmlConvert.ToBoolean( reader.Value );
      }

      if( flagValue )
      {
        protection |= flag;
      }
      else
      {
        protection &= ~flag;
      }

      return protection;
    }
    /// <summary>
    /// Checks single protection attribute and updates protection flags as necessary.
    /// </summary>
    /// <param name="reader">XmlReader to get attribute value from.</param>
    /// <param name="attributeName">Attribute name to check.</param>
    /// <param name="flag">Flag value that corresponds to the protection attribute.</param>
    /// <param name="defaultValue">Default value of the attribute.</param>
    /// <param name="protection">Current protection settings.</param>
    /// <returns>Updated protection settings.</returns>
    private ExcelSheetProtection CheckProtectionAttribute( XmlReader reader, string attributeName,
      ExcelSheetProtection flag, bool defaultValue, ExcelSheetProtection protection )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( attributeName == null || attributeName.Length == 0 )
        throw new ArgumentOutOfRangeException( "attributeName" );

      bool flagValue = defaultValue;

      if( reader.MoveToAttribute( attributeName ) )
      {
        flagValue = XmlConvert.ToBoolean( reader.Value );
      }

      if( !flagValue )
      {
        protection |= flag;
      }
      else
      {
        protection &= ~flag;
      }

      return protection;
    }
    /// <summary>
    /// Extracts ignore error options from XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="sheet">Worksheet to put extracted data into.</param>
    private void ParseIgnoreError( XmlReader reader, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( reader.LocalName != Excel2007Serializator.IgnoredErrorsTag )
        throw new XmlException();

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element && reader.LocalName == Excel2007Serializator.IgnoredErrorTag )
          {
            ExtractIgnoredError( reader, sheet );
          }
          else
          {
            reader.Read();
          }
        }
      }

      reader.Read();
    }
    /// <summary>
    /// Extracts ignore error option from a reader.
    /// </summary>
    /// <param name="reader">XmlReader to extract ignored error settings.</param>
    /// <param name="sheet">Worksheet to put extracted data into.</param>
    private void ExtractIgnoredError( XmlReader reader, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( reader.LocalName != Excel2007Serializator.IgnoredErrorTag )
        throw new XmlException();

      ExcelIgnoreError options = ExcelIgnoreError.None;
      string strRange = null;

      for( int i = 0, len = reader.AttributeCount; i < len; i++ )
      {
        reader.MoveToAttribute( i );

        if( reader.LocalName == Excel2007Serializator.RangeReferenceAttribute )
        {
          strRange = reader.Value;
        }
        else if( XmlConvert.ToBoolean( reader.Value ) )
        {
          int index = Array.IndexOf( Excel2007Serializator.ErrorTagsSequence, reader.LocalName );

          if( index >= 0 )
          {
            options |= Excel2007Serializator.ErrorsSequence[ index ];
          }
        }
      }

      if( strRange == null )
        throw new XmlException();

      AddErrorIndicator( strRange, options, sheet );

      reader.MoveToElement();
      reader.Read();
    }
    /// <summary>
    /// Adds error indicator to the corresponding worksheet's collection.
    /// </summary>
    /// <param name="strRange">String representation of the cells with such options.</param>
    /// <param name="options">Ignore error options.</param>
    /// <param name="sheet">Parent worksheet.</param>
    private void AddErrorIndicator( string strRange, ExcelIgnoreError options, WorksheetImpl sheet )
    {
      ErrorIndicatorImpl errorIndicator = new ErrorIndicatorImpl( options );
      string[] arrCells = strRange.Split( ' ' );
      IWorkbook book = sheet.Workbook;

      int firstRow;
      int firstCol;
      int lastRow;
      int lastCol;

      for( int i = 0, len = arrCells.Length; i < len; i++ )
      {
        RangeImpl.ParseRangeString( arrCells[ i ], book, out firstRow, out firstCol, out lastRow, out lastCol );
        // Rectangle is zero-based.
        Rectangle rect = Rectangle.FromLTRB( firstCol - 1, firstRow - 1, lastCol - 1, lastRow - 1 );
        errorIndicator.AddRange( rect );
      }

      sheet.ErrorIndicators.Add( errorIndicator );
    }
    /// <summary>
    /// Extracts custom worksheet properties.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="sheet">Worksheet to put extracted properties into.</param>
    private void ParseCustomWorksheetProperties( XmlReader reader, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( reader.LocalName != Excel2007Serializator.CustomPropertiesTagName )
        throw new XmlException();

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element && reader.LocalName == Excel2007Serializator.CustomPropertyTagName )
          {
            ParseCustomProperty( reader, sheet );
          }
          else
          {
            reader.Skip();
          }
        }
      }

      reader.Read();
    }
    /// <summary>
    /// Extracts single custom property from specified reader.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="sheet">Worksheet to put extracted data into.</param>
    private void ParseCustomProperty( XmlReader reader, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( reader.LocalName != Excel2007Serializator.CustomPropertyTagName )
        throw new XmlException();

      if( !reader.MoveToAttribute( Excel2007Serializator.NameAttributeName ) )
        throw new XmlException();

      string propertyName = reader.Value;

      if( !reader.MoveToAttribute( Excel2007Serializator.RelationAttribute, Excel2007Serializator.RelationNamespace ) )
        throw new XmlException();

      string id = reader.Value;

      IWorksheetCustomProperties properties = sheet.CustomProperties;
      ICustomProperty property = properties.Add( propertyName );
      property.Value = GetPropertyData( id, sheet.DataHolder );

      reader.MoveToElement();
      reader.Skip();
    }
    /// <summary>
    /// Gets the parsed XML value.
    /// </summary>
    /// <param name="xmlValue">The XML value.</param>
    /// <returns></returns>
    private static int GetParsedXmlValue(string xmlValue)
    {
        return xmlValue.StartsWith("-") ?
                                  (int)XmlConvert.ToInt32(xmlValue)
                                 : (int)XmlConvert.ToUInt32(xmlValue);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dataHolder"></param>
    /// <returns></returns>
    private string GetPropertyData( string id, WorksheetDataHolder dataHolder )
    {
      RelationCollection relations = dataHolder.Relations;
      Relation relation = relations[ id ];
      relations.Remove( id );
      string itemPath = dataHolder.ArchiveItem.ItemName;
      itemPath = Path.GetDirectoryName( itemPath );
      itemPath = itemPath.Replace( '\\', '/' );

      byte[] arrData = dataHolder.ParentHolder.GetData( relation, itemPath, true );
      return Encoding.Unicode.GetString( arrData, 0, arrData.Length );
    }
    /// <summary>
    /// Parses header/footer drawings.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="sheet">Worksheet to put data into.</param>
    public static void ParseLegacyDrawingHF( XmlReader reader, WorksheetBaseImpl sheet,
      RelationCollection relations )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( reader.LocalName != Vml.LegacyDrawingHF )
        throw new XmlException( "Unexpected xml tag." );

      if( reader.MoveToAttribute( Excel2007Serializator.RelationAttribute, Excel2007Serializator.RelationNamespace ) )
      {
        string strRelationId = reader.Value;
        WorksheetDataHolder holder = sheet.DataHolder;
        holder.VmlHFDrawingsId = strRelationId;
        holder.ParseVmlShapes( sheet.HeaderFooterShapes, strRelationId, relations );
        reader.MoveToElement();
        reader.Skip();
      }
      else
      {
        throw new XmlException( "Wrong xml format" );
      }
    }
    /// <summary>
    /// This method parses Drawings xml tag (shapes).
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="sheet">Worksheet to put extracted data into.</param>
    /// <param name="dictItemsToRemove">Dictionary with archive items to remove after parsing.</param>
    private void ParseDrawings( XmlReader reader, WorksheetBaseImpl sheet, Dictionary<string, object> dictItemsToRemove )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( reader.LocalName != Drawings.DrawingTagName )
        throw new XmlException( "Unexpected xml tag." );

      if( reader.MoveToAttribute( Excel2007Serializator.RelationAttribute, Excel2007Serializator.RelationNamespace ) )
      {
        string strRelationId = reader.Value;
        WorksheetDataHolder holder = sheet.DataHolder;
        holder.ParseDrawings( sheet, strRelationId, dictItemsToRemove );
        holder.DrawingsId = strRelationId;
        reader.MoveToElement();
        reader.Skip();
      }
      else
      {
        throw new XmlException( "Wrong xml format" );
      }
    }
    /// <summary>
    /// This method parses LegacyDrawings xml tag (vml shapes).
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="sheet">Worksheet to put extracted data into.</param>
    private void ParseLegacyDrawing( XmlReader reader, WorksheetBaseImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( reader.LocalName != Vml.LegacyDrawing )
        throw new XmlException( "Unexpected xml tag." );

      if( reader.MoveToAttribute( Excel2007Serializator.RelationAttribute, Excel2007Serializator.RelationNamespace ) )
      {
        string strRelationId = reader.Value;
        WorksheetDataHolder holder = sheet.DataHolder;
        holder.ParseVmlShapes( sheet.InnerShapes, strRelationId, null );
        holder.VmlDrawingsId = strRelationId;
        reader.MoveToElement();
        reader.Skip();
      }
      else
      {
        throw new XmlException( "Wrong xml format" );
      }
    }
    /// <summary>
    /// Extracts single drawing defined by TwoCellAnchor tag name from the specified XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to get shape data from.</param>
    /// <param name="sheet">Worksheet to put extracted shape into.</param>
    /// <param name="drawingsPath">Absolute path to the drawings.</param>
    /// <param name="lstRelationIds">Relation ids that were parsed (to remove them from the collection later).</param>
    /// <param name="dictItemsToRemove">Dictionary with archive items to remove after parsing.</param>
    private void ParseTwoCellAnchor( XmlReader reader, WorksheetBaseImpl sheet,
      string drawingsPath, List<string> lstRelationIds, Dictionary<string, object> dictItemsToRemove )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( lstRelationIds == null )
        throw new ArgumentNullException( "lstRelationIds" );

      //if( reader.LocalName != Drawings.TwoCellAnchorTagName )
      //  throw new XmlException( "Unexpected xml tag." );

      bool bRelative = reader.LocalName == ChartConstants.RelativeSizeAnchorTag;

      string strEditAs = null;

      if (reader.MoveToAttribute(Drawings.EditAsAttribute))
      {
          strEditAs = reader.Value;
          m_drawingParser.placement = strEditAs;
      }

      reader.Read();

      Rectangle fromRect = new Rectangle();
      Rectangle toRect = new Rectangle();
      ShapeImpl shape = null;
      MemoryStream data = null;
      Size shapeExtent = new Size( -1, -1 );

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case Drawings.FromTagName:
              fromRect = ParseAnchorPoint( reader );
              m_drawingParser.leftColumn = fromRect.X;
              m_drawingParser.leftColumnOffset = Helper.ConvertEmuToOffset(fromRect.Width, this.dpiX);
              m_drawingParser.topRow = fromRect.Y;
              m_drawingParser.topRowOffset = Helper.ConvertEmuToOffset(fromRect.Height, this.dpiY);
              break;

            case Drawings.ToTagName:
              toRect = ParseAnchorPoint( reader );
              m_drawingParser.rightColumn = toRect.X;
              m_drawingParser.rightColumnOffset = Helper.ConvertEmuToOffset(toRect.Width, this.dpiX);
              m_drawingParser.bottomRow = toRect.Y;
              m_drawingParser.bottomRowOffset = Helper.ConvertEmuToOffset(toRect.Height, this.dpiY);
              break;

            case Drawings.Extents:
              shapeExtent = ParseExtent( reader );
              break;

            case Drawings.PictureTagName:
              shape = ParsePicture( reader, sheet, drawingsPath, lstRelationIds, dictItemsToRemove );
              break;

            case Drawings.ClientDataTagName:
              // This tag can be present but we don't support it.
              reader.Skip();
              break;

            case Drawings.Shape:
              case Drawings.ConnectionShape:
              m_drawingParser.preFix = reader.Prefix;
              m_drawingParser.shapeType = reader.LocalName;
              shape = CreateShape( reader, sheet, ref data );

              if (m_enableAlternateContent)
                  shape.EnableAlternateContent = m_enableAlternateContent;                 
              break;
              case Drawings.AlternateContentTag:
              shape = CreateShape(reader, sheet, ref data);
              shape.XmlDataStream = data;
              shape.IsEquationShape = true;

              break;

            //case Drawings.ConnectionShape:
            //  //TODO: We don't support this shape type, so we have to preserve it.
            //  Stream cnxnShapeStream = ReadSingleNodeIntoStream(reader);
            //  if (shape == null)
            //  {
            //      shape = new ShapeImpl(sheet.Application, sheet.InnerShapes);
            //      sheet.InnerShapes.AddShape(shape);
            //  }
            //  if (shape.preservedCnxnShapeStreams == null)
            //      shape.preservedCnxnShapeStreams = new List<Stream>();
            //  shape.preservedCnxnShapeStreams.Add(cnxnShapeStream);
            //  break;

            case Drawings.GroupShape:
            default:
              //TODO: Need to provide parsing support for shapes other than chart
              data = ReadSingleNodeIntoStream(reader);
              shape = TryParseShape(data, sheet, drawingsPath);

              if (shape == null)
              {
                  shape = new ShapeImpl(sheet.Application, sheet.InnerShapes);
                  sheet.InnerShapes.AddShape(shape);
              }
              else
              {
                  data = null;
              }
              break;

            case Drawings.GraphicFrame:
              //throw new XmlException( "Not supported xml tag." );
              data = ReadSingleNodeIntoStream( reader );
              shape = TryParseChart(data, sheet, drawingsPath);

              if (shape == null)
              {
                  shape = new ShapeImpl(sheet.Application, sheet.InnerShapes);
                  sheet.InnerShapes.AddShape(shape);
              }
              else
              {
                  data = null;
              }
              break;
          }
        }
        else
        {
          reader.Skip();
        }
      }

      reader.Read();

      if (shape != null)
      {
          SetAnchor(shape, fromRect, toRect, shapeExtent, bRelative);
          shape.XmlDataStream = data;
          ParseEditAsValue(shape, strEditAs);
      }
      m_enableAlternateContent = false;
    }
    /// <summary>
    /// Extracts shape specified by "sp" tag and registers it in required collections.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="sheet">Parent worksheet.</param>
    /// <param name="data">Stream that will contains xml representation of the shape if necessary.</param>
    /// <returns>Created shape object.</returns>
    private ShapeImpl CreateShape( XmlReader reader, WorksheetBaseImpl sheet, ref MemoryStream data )
    {
      // Read shape data into stream.
      data = ReadSingleNodeIntoStream( reader );
      data.Position = 0;

      reader = UtilityMethods.CreateReader( data );
      //reader.Read();
      ExcelShapeType shapeType = ExcelShapeType.Unknown;
      ShapeImpl result = null;
      string shapeName = null;
      int? shapeId = null;
      string textlink = null;
      if (reader.MoveToAttribute("textlink"))
      {
          textlink = reader.Value;
      }
      // 1. Detect whether this is TextBox shape
      // we have to find cNvSpPr tag for this purpose.
      while( reader.NodeType != XmlNodeType.None )
      {
        reader.Read();

        if( reader.NodeType == XmlNodeType.Element )
        {
          if( reader.LocalName == Drawings.NonVisualDrawingProperties )
          {
              if (reader.MoveToAttribute(Drawings.TextBoxAttribute) && XmlConvert.ToBoolean(reader.Value))
              {
                  shapeType = ExcelShapeType.TextBox;
                  break;
              }
          }
          else if (reader.LocalName == Drawings.NVCanvasPropertiesTag)
          {
              if (reader.MoveToAttribute(Drawings.IdAttributeName))
              {
                  int id;

                  if (int.TryParse(reader.Value, out id))
                  {
                      shapeId = id;
                      this.m_drawingParser.id = id;
                  }
              }
              if (reader.MoveToAttribute(Drawings.NameAttributeName))
              {
                  this.m_drawingParser.name = reader.Value;
                  shapeName = reader.Value;
              }
              if (reader.MoveToAttribute(Drawings.DescriptionAttributeName))
                  this.m_drawingParser.descr = reader.Value;
              if (reader.MoveToAttribute(Drawings.HiddenAttribute))
                  this.m_drawingParser.IsHidden = XmlConvert.ToBoolean(reader.Value);
              if (reader.MoveToAttribute("title"))
                  this.m_drawingParser.tittle = reader.Value;
          }
          if (reader.LocalName == "xfrm")
          {
              if (reader.MoveToAttribute(Drawings.RotationAttribute))
              {
                  string str3 = reader.Value;
                  if ((str3 != null) && (str3.Length > 0))
                  {
                      double num = double.Parse(str3, CultureInfo.InvariantCulture) / 60000.0;
                      this.m_drawingParser.shapeRotation = num;
                  }
              }
              if (reader.MoveToAttribute("flipH"))
                  this.m_drawingParser.FlipHorizontal = XmlConvert.ToBoolean(reader.Value);
              if (reader.MoveToAttribute("flipV"))
                  this.m_drawingParser.FlipVertical = XmlConvert.ToBoolean(reader.Value);

              ParseForm(reader);

          }
          if (reader.LocalName == "prstGeom" || reader.LocalName == "custGeom")
          {
              if (!m_enableAlternateContent && m_drawingParser.preFix != Drawings.CdrPreffix)
              {
                  if (reader.LocalName == "prstGeom")
                  {
                      string autoShapeName = "";
                      if (reader.MoveToAttribute("prst"))
                          autoShapeName = reader.Value;
                      AutoShapeConstant autoConst = AutoShapeHelper.GetAutoShapeConstant(autoShapeName);
                      AutoShapeType type = AutoShapeHelper.GetAutoShapeType(autoConst);

                      if (type != AutoShapeType.Unknown)
                      {
                          shapeType = ExcelShapeType.AutoShape;
                          this.m_drawingParser.autoShapeType = type;
                          reader.MoveToElement();
                          reader.Read();
                      }
                  }
                  if (reader.LocalName == "avLst")
                      this.m_drawingParser.CustGeomStream = ShapeParser.ReadNodeAsStream(reader);

              }
              break;
          }
          if (reader.LocalName == "Fallback")
              reader.Skip();
        }
      }

      data.Position = 0;
      reader = UtilityMethods.CreateReader( data );
      //reader.Read();
      //reader.Read();

      switch( shapeType )
      {
        case ExcelShapeType.TextBox:
          ITextBoxShapeEx textBox = sheet.Shapes.AddTextBox();
          if (textlink != null && textlink.Length > 0)
              textBox.TextLink = string.Format("={0}", textlink);
          result = ( ShapeImpl )textBox;
          TextBoxShapeParser.ParseTextBox( textBox, reader, this );
		  if (shapeId != null)
             result.ShapeId = (int)shapeId;
          break;

        case ExcelShapeType.Unknown:
          result = new ShapeImpl( sheet.Application, sheet.InnerShapes );

          if( shapeId != null )
            result.ShapeId = ( int )shapeId;
          if (shapeName != null)
          {
              result.Name = shapeName;
          }
          sheet.InnerShapes.AddShape( result );
          break;

        case ExcelShapeType.AutoShape:
          AutoShapeImpl result1 = new AutoShapeImpl(sheet.Application, sheet.InnerShapes);
          m_drawingParser.AddShape(result1, m_workSheet);
          ParseAutoShape(result1, reader);
          sheet.InnerShapes.Add(result1);
          result1.ShapeExt.Logger.ResetFlag();
          break;
      }

      return result;
    }
    public void ParseAutoShape(AutoShapeImpl autoShape, XmlReader reader)
    {
        if (autoShape == null)
            throw new ArgumentNullException("autoShape");

        if (reader == null)
            throw new ArgumentNullException("reader");

        reader.Read();

        while (reader.NodeType != XmlNodeType.None)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.LocalName)
                {
                    case Drawings.TextBody:
                        // On the current moment we don't support
                        Stream txtBody = ShapeParser.ReadNodeAsStream(reader);
                        txtBody.Position = 0;
                        autoShape.ShapeExt.PreservedElements.Add("TextBody", txtBody);
                        XmlReader txtReader = UtilityMethods.CreateReader(txtBody);
                        ParseRichText(txtReader, autoShape,this);
                        break;

                    case Drawings.ShapePropertiesTag:
                        ParseProperties(reader, autoShape);
                        break;

                    case "style":
                        autoShape.ShapeExt.PreservedElements.Add("Style", ShapeParser.ReadNodeAsStream(reader));
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
            else
            {
                reader.Read();
            }
        }
    }

    private  void ParseProperties(XmlReader reader, AutoShapeImpl autoShape)
    {
        reader.Read();
        while (reader.NodeType != XmlNodeType.EndElement)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.LocalName)
                {
                    case "noFill":
                        autoShape.ShapeExt.Fill.Visible = false;
                        reader.Skip();
                        break;

                    case Drawings.SolidFillTag:
                        Stream soildFill = ShapeParser.ReadNodeAsStream(reader);
                        soildFill.Position = 0;
                        autoShape.ShapeExt.PreservedElements.Add("Fill", soildFill);
                        XmlReader fillReader = UtilityMethods.CreateReader(soildFill);
                        IInternalFill fill = autoShape.ShapeExt.Fill as IInternalFill;
                        ChartParserCommon.ParseSolidFill(fillReader, this, fill.ForeColorObject);
                        break;

                    case Drawings.LineTag:
                        Stream lineFill = ShapeParser.ReadNodeAsStream(reader);
                        lineFill.Position = 0;
                        autoShape.ShapeExt.PreservedElements.Add("Line", lineFill);
                        XmlReader lineFillReader = UtilityMethods.CreateReader(lineFill);
                        ShapeLineFormatImpl line = autoShape.ShapeExt.Line;
                        TextBoxShapeParser.ParseLineProperties(lineFillReader, line, false, this);
                        break;

                    case "gradFill":
                        Stream gradientFill = ShapeParser.ReadNodeAsStream(reader);
                        gradientFill.Position = 0;
                        autoShape.ShapeExt.PreservedElements.Add("Fill", gradientFill);
                        XmlReader gfillReader = UtilityMethods.CreateReader(gradientFill);
                        IInternalFill gfill = autoShape.ShapeExt.Fill as IInternalFill;
                        gfill.FillType = ExcelFillType.Gradient;
                        gfill.PreservedGradient = ChartParserCommon.ParseGradientFill(gfillReader, this);
                        break;

                    case "blipFill":
                        autoShape.ShapeExt.PreservedElements.Add("Fill", ShapeParser.ReadNodeAsStream(reader));
                        break;

                    case "pattFill":
                        Stream patternFill = ShapeParser.ReadNodeAsStream(reader);
                        patternFill.Position = 0;
                        autoShape.ShapeExt.PreservedElements.Add("Fill", patternFill);
                        XmlReader patternfillReader = UtilityMethods.CreateReader(patternFill);
                        IInternalFill patternfill = autoShape.ShapeExt.Fill as IInternalFill;
                        patternfill.FillType = ExcelFillType.Pattern;
                        ChartParserCommon.ParsePatternFill(patternfillReader, patternfill, this);
                        break;

                    case "grpFill":
                        autoShape.ShapeExt.PreservedElements.Add("Fill", ShapeParser.ReadNodeAsStream(reader));
                        break;

                    case "effectLst":
                        autoShape.ShapeExt.PreservedElements.Add("Effect", ShapeParser.ReadNodeAsStream(reader));
                        break;

                    case "effectDag":
                        autoShape.ShapeExt.PreservedElements.Add("Effect", ShapeParser.ReadNodeAsStream(reader));
                        break;

                    case "scene3d":
                        autoShape.ShapeExt.PreservedElements.Add("Scene3d", ShapeParser.ReadNodeAsStream(reader));
                        break;

                    case "sp3d":
                        autoShape.ShapeExt.PreservedElements.Add("Sp3d", ShapeParser.ReadNodeAsStream(reader));
                        break;

                    default:
                        reader.Skip();
                        break;
                }
            }
            else
                reader.Read();
        }
    }
    private static void ParseRichText(XmlReader reader, AutoShapeImpl autoShape,Excel2007Parser parser)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        if (autoShape == null)
            throw new ArgumentNullException("textBox");

        reader.Read();

        while (reader.NodeType != XmlNodeType.EndElement)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.LocalName)
                {
                    case Drawings.TextBodyPropertiesTag:
                        ParseBodyProperties(reader, autoShape.TextFrameInternal);  //Parse Body Properties
                        break;

                    case Drawings.Paragraphs:
                        ParseParagraphs(reader, autoShape, parser);
                        break;

                    default:
                        reader.Skip();
                        break;
                }
            }
            else
            {
                reader.Skip();
            }
        }

        reader.Read();
    }

    private static void ParseBodyProperties(XmlReader reader, TextFrame textFrame)
    {

        if (reader.MoveToAttribute("vertOverflow"))
        {
            textFrame.TextVertOverflowType = Helper.GetVerticalFlowType(reader.Value);
        }
        if (reader.MoveToAttribute("horzOverflow"))
        {
            textFrame.TextHorzOverflowType = Helper.GetHorizontalFlowType(reader.Value);
        }
        if (reader.MoveToAttribute("vert"))
        {
            textFrame.TextDirection = Helper.SetTextDirection(reader.Value);
        }
        if (reader.MoveToAttribute("wrap"))
        {
            textFrame.WrapTextInShape = (reader.Value != "none");
        }

        if (reader.MoveToAttribute("lIns"))
        {
            textFrame.SetLeftMargin(Helper.ParseInt(reader.Value));
            textFrame.IsAutoMargins = false;
        }
        if (reader.MoveToAttribute("tIns"))
        {
            textFrame.SetTopMargin(Helper.ParseInt(reader.Value));
            textFrame.IsAutoMargins = false;
        }
        if (reader.MoveToAttribute("rIns"))
        {
            textFrame.SetRightMargin(Helper.ParseInt(reader.Value));
            textFrame.IsAutoMargins = false;
        }
        if (reader.MoveToAttribute("bIns"))
        {
            textFrame.SetBottomMargin(Helper.ParseInt(reader.Value));
            textFrame.IsAutoMargins = false;
        }
        if (reader.MoveToAttribute("numCol"))
        {
            textFrame.Columns.Number = Helper.ParseInt(reader.Value);
        }
        if (reader.MoveToAttribute("spcCol"))
        {
            textFrame.Columns.SpacingPt = (int)(Helper.ParseInt(reader.Value) / 12700.0);
        }
        string anchor = "t";
        bool anchorCtr = false;
        if (reader.MoveToAttribute("anchor"))
        {
            anchor = reader.Value;
        }
        if (reader.MoveToAttribute("anchorCtr"))
        {
            anchorCtr = XmlConvert.ToBoolean(reader.Value);
        }
        Helper.SetAnchorPosition(textFrame, anchor, anchorCtr);
        reader.Read();
    }


    private static void ParseParagraphs(XmlReader reader, AutoShapeImpl autoShape,Excel2007Parser parser)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        if (autoShape == null)
            throw new ArgumentNullException("textBox");

        if (reader.LocalName != Drawings.Paragraphs)
            throw new XmlException("Unexpected xml tag.");

        ITextRange range=autoShape.ShapeExt.TextFrame.TextRange;
        RichTextString textArea = range.RichText as RichTextString;
        string strCurrentText = textArea.Text;

        if (strCurrentText != null && strCurrentText.Length != 0 && !strCurrentText.EndsWith("\n"))
            textArea.AddText("\n", textArea.GetFont(strCurrentText.Length - 1));

        reader.Read();

        while (reader.NodeType != XmlNodeType.EndElement)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.LocalName)
                {
                    case Drawings.ParagraphRun:
                        TextBoxShapeParser.ParseParagraphRun(reader, textArea, parser);
                        break;

                    case Drawings.ParagraphEndProperties:
                        TextBoxShapeParser.ParseParagraphEnd(reader, textArea, parser);
                        break;

                    default:
                        reader.Skip();
                        break;
                }
            }
            else
            {
                reader.Skip();
            }
        }

        reader.Read();
    }
    private static void ParseParagraphRun(XmlReader reader, AutoShapeImpl autoShape)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        if (autoShape == null)
            throw new ArgumentNullException("textArea");

        if (reader.LocalName != Drawings.ParagraphRun)
            throw new XmlException("Unexpected xml tag.");

        reader.Read();
        string text = null;
        IFont font = null;

        while (reader.NodeType != XmlNodeType.EndElement)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.LocalName)
                {

                    case Drawings.ParagraphText:
                        string textPart = reader.ReadElementContentAsString();//ReadElementString();
                        text = textPart;
                        break;

                    default:
                        reader.Skip();
                        break;
                }
            }
            else
            {
                reader.Skip();
            }
        }

        if (text == null || text.Length == 0)
            text = "\n";
        string value = autoShape.ShapeExt.TextFrame.TextRange.Text;
        autoShape.ShapeExt.TextFrame.TextRange.Text = value + text;
        reader.Read();
    }
    private void ParseForm(XmlReader reader)
    {
        if (!reader.IsEmptyElement)
        {
            reader.Read();
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case Drawings.Offset:
                            if (reader.MoveToAttribute(Drawings.XAttributeName))
                                this.m_drawingParser.posX = XmlConvert.ToInt32(reader.Value);

                            if (reader.MoveToAttribute(Drawings.YAttributeName))
                                this.m_drawingParser.posY = XmlConvert.ToInt32(reader.Value);
                            break;

                        case Drawings.Extents:
                            if (reader.MoveToAttribute(Drawings.CXAttributeName))
                                this.m_drawingParser.extCX = XmlConvert.ToInt32(reader.Value);

                            if (reader.MoveToAttribute(Drawings.CYAttributeName))
                                this.m_drawingParser.extCY = XmlConvert.ToInt32(reader.Value);
                            break;

                        default:
                            reader.Skip();
                            break;
                    }
                }
                else
                {
                    reader.Skip();
                }
            }
        }
    }
    /// <summary>
    /// Tries to extract chart object from specified stream with GraphicFrame tag.
    /// </summary>
    /// <param name="data">Stream with GraphicFrame tag.</param>
    /// <param name="sheet">Parent worksheet.</param>
    /// <param name="drawingPath">Path to the drawing item.</param>
    /// <returns>Extracted chart shape; or null if chart wasn't located.</returns>
    private ShapeImpl TryParseChart( MemoryStream data, WorksheetBaseImpl sheet, string drawingPath )
    {
      if( data == null )
        throw new ArgumentNullException( "data" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      data.Position = 0;
      XmlReader reader = UtilityMethods.CreateReader( data );

      //while( reader.NodeType != XmlNodeType.Element )
      //  reader.Read();

      // skipping GraphicFrame tag.
      reader.Read();
      string shapeName = null;
      string shapeID="0";
      // locating chart tag
      while( reader.LocalName != ChartConstants.ChartTag && reader.NodeType != XmlNodeType.None )
      {
        if( reader.LocalName == Drawings.NVCanvasPropertiesTag &&
          reader.MoveToAttribute( Drawings.NameAttributeName ) )
        {
          shapeName = reader.Value;

          if (reader.MoveToAttribute(Drawings.IdAttributeName))
              shapeID = reader.Value;
        }
        

        reader.Read();
      }

      ChartShapeImpl chartShape = null;

      if( reader.LocalName == ChartConstants.ChartTag )
      {
        chartShape = ( ChartShapeImpl )sheet.Charts.Add();
        ChartImpl chart = chartShape.ChartObject;
        WorksheetDataHolder sheetDataHolder = sheet.DataHolder;
        FileDataHolder fileDataHolder = sheetDataHolder.ParentHolder;
        RelationCollection relations = sheetDataHolder.DrawingsRelations;
        chart.DataHolder = sheetDataHolder;
        ParseChartTag( reader, chart, relations, fileDataHolder, drawingPath );
        chart.DataHolder = null;
        if (sheetDataHolder.DrawingsRelations.Count == 0 && relations.Count!=0)
        {
            sheetDataHolder.AssignDrawingrelation(relations);
        }
        if( shapeName != null )
          chartShape.Name = shapeName;
      }
        if(chartShape !=null)
      ((ShapeImpl)chartShape).ShapeId = Int32.Parse(shapeID);
      return chartShape;
    }
    /// <summary>
    /// Tries to extract chart object from specified stream with GraphicFrame tag.
    /// </summary>
    /// <param name="data">Stream with GraphicFrame tag.</param>
    /// <param name="sheet">Parent worksheet.</param>
    /// <param name="drawingPath">Path to the drawing item.</param>
    /// <returns>Extracted chart shape; or null if chart wasn't located.</returns>
    private ShapeImpl TryParseShape(MemoryStream data, WorksheetBaseImpl sheet, string drawingPath)
    {
        if (data == null)
            throw new ArgumentNullException("data");

        if (sheet == null)
            throw new ArgumentNullException("sheet");

        data.Position = 0;
        XmlReader reader = UtilityMethods.CreateReader(data);

        // skipping group shape tag.
        reader.Read();

        string shapeName = null;
        string shapeID = "0";
        ChartShapeImpl chartShape = null;

        // locating chart tag
        ShapeImpl shape = sheet.InnerShapes.AddShape(new ShapeImpl(sheet.Application, sheet.InnerShapes));
        bool skip = false;
        while (reader.LocalName != Drawings.GroupShape && reader.NodeType != XmlNodeType.None)
        {
            switch (reader.LocalName)
            {
                case Drawings.NVGroupShapePropertiesTag:
                    Stream nvGroupShapeStream = ReadSingleNodeIntoStream(reader);
                    if (((WorksheetImpl)sheet).preservedStreams == null)
                        ((WorksheetImpl)sheet).preservedStreams = new List<Stream>();
                    ((WorksheetImpl)sheet).preservedStreams.Add(nvGroupShapeStream);
                    skip = true;
                    break;
                case Drawings.Slicer:
                    if (chartShape == null)
                            chartShape = new ChartShapeImpl(sheet.Application, shape);
                     if (shapeName != null)
                            chartShape.Name = shapeName;
                    if (chartShape != null)
                        ((ShapeImpl)chartShape).ShapeId = Int32.Parse(shapeID);
                    shape.GraphicFrameStream = ShapeParser.ReadNodeAsStream(reader);
                    chartShape.GraphicFrameStream = shape.GraphicFrameStream;
                    shape.ChildShapes.Add(chartShape);
                    break;
                case Drawings.GroupShapePropertiesTag:
                    Stream groupShapeStream = ReadSingleNodeIntoStream(reader);
                    if (((WorksheetImpl)sheet).preservedStreams == null)
                        ((WorksheetImpl)sheet).preservedStreams = new List<Stream>();
                    ((WorksheetImpl)sheet).preservedStreams.Add(groupShapeStream);
                    skip = true;
                    break;

                case Drawings.NVCanvasPropertiesTag:
                    if (reader.LocalName == Drawings.NVCanvasPropertiesTag &&
                        reader.MoveToAttribute(Drawings.NameAttributeName))
                    {
                        shapeName = reader.Value;

                        if (reader.MoveToAttribute(Drawings.IdAttributeName))
                            shapeID = reader.Value;
                    }
                    break;

                case Drawings.PictureTagName:
                    Stream pictureStream = ReadSingleNodeIntoStream(reader);
                    if (shape.preservedPictureStreams == null)
                        shape.preservedPictureStreams = new List<Stream>();
                    shape.preservedPictureStreams.Add(pictureStream);
                    skip = true;
                    break;

                case Drawings.Shape:
                    Stream shapeStream = ReadSingleNodeIntoStream(reader);
                    if (shape.preservedShapeStreams == null)
                        shape.preservedShapeStreams = new List<Stream>();
                    shape.preservedShapeStreams.Add(shapeStream);
                    skip = true;
                    break;

                case ChartConstants.ChartTag:
                    if (reader.LocalName == ChartConstants.ChartTag)
                    {
                        if (chartShape == null)
                            chartShape = new ChartShapeImpl(sheet.Application, shape);
                        shape.ChildShapes.Add(chartShape);
                        ChartImpl chart = chartShape.ChartObject;
                        WorksheetDataHolder sheetDataHolder = sheet.DataHolder;
                        FileDataHolder fileDataHolder = sheetDataHolder.ParentHolder;
                        RelationCollection relations = sheetDataHolder.DrawingsRelations;
                        chart.DataHolder = sheetDataHolder;
                        ParseChartTag(reader, chart, relations, fileDataHolder, drawingPath);
                        chart.DataHolder = null;

                        if (shapeName != null)
                            chartShape.Name = shapeName;
                    }
                    if (chartShape != null)
                        ((ShapeImpl)chartShape).ShapeId = Int32.Parse(shapeID);

                    break;

                case Drawings.Offset:
                    chartShape = new ChartShapeImpl(sheet.Application, shape);

                    if (reader.LocalName == Drawings.Offset)
                    {
                        if (reader.MoveToAttribute(Drawings.XAttributeName))
                            chartShape.OffsetX = XmlConvert.ToInt32(reader.Value);

                        if (reader.MoveToAttribute(Drawings.YAttributeName))
                            chartShape.OffsetY = XmlConvert.ToInt32(reader.Value);
                    }
                    break;

                case Drawings.Extents:
                    if (chartShape == null)
                        chartShape = new ChartShapeImpl(sheet.Application, shape);

                    if (reader.LocalName == Drawings.Extents)
                    {
                        if (reader.MoveToAttribute(Drawings.CXAttributeName))
                            chartShape.ExtentsX = XmlConvert.ToInt32(reader.Value);

                        if (reader.MoveToAttribute(Drawings.CYAttributeName))
                            chartShape.ExtentsY = XmlConvert.ToInt32(reader.Value);
                    }
                    break;

                case Drawings.ConnectionShape:
                    Stream cxnStream = ReadSingleNodeIntoStream(reader);
                    if (shape.preservedInnerCnxnShapeStreams == null)
                        shape.preservedInnerCnxnShapeStreams = new List<Stream>();
                    shape.preservedInnerCnxnShapeStreams.Add(cxnStream);
                    skip = true;
                    break;
                    
                default:
                    //reader.Skip();
                    break;
            }

            if (!skip)
             reader.Read();
            skip = false;
        }

        return shape;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    private MemoryStream ReadSingleNodeIntoStream( XmlReader reader )
    {
      MemoryStream result = new MemoryStream();
      XmlWriter writer = UtilityMethods.CreateWriter( result, Encoding.UTF8 );
      writer.WriteNode( reader, false );
      writer.Flush();
      return result;
    }
    /// <summary>
    /// Extracts extent settings from reader and converts them into pixels.
    /// </summary>
    /// <param name="reader">Reader to get extent data from.</param>
    /// <returns>Size of the shape in pixels.</returns>
    private Size ParseExtent( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      int iWidth = -1;
      int iHeight = -1;

      if( reader.MoveToAttribute( Drawings.CXAttributeName ) )
        iWidth = int.Parse( reader.Value );

      if( reader.MoveToAttribute( Drawings.CYAttributeName ) )
        iHeight = int.Parse( reader.Value );

      if (m_drawingParser != null)
      {
          m_drawingParser.cx = Helper.ConvertEmuToOffset(iWidth, this.dpiX);
          m_drawingParser.cy = Helper.ConvertEmuToOffset(iHeight, this.dpiX);
      }

      iWidth = ( int )Math.Round( ApplicationImpl.ConvertToPixels( iWidth, MeasureUnits.EMU ) );
      iHeight = ( int )Math.Round( ApplicationImpl.ConvertToPixels( iHeight, MeasureUnits.EMU ) );

      return new Size( iWidth, iHeight );
    }
    /// <summary>
    /// Parses EditAs attribute.
    /// </summary>
    /// <param name="shape">Shape to set properties for.</param>
    /// <param name="editAs">Value to parse</param>
    private void ParseEditAsValue( ShapeImpl shape, string editAs )
    {
      if( editAs == null )
        return;

      if( shape == null )
        throw new ArgumentNullException( "shape" );

      switch( editAs )
      {
        case Drawings.PositionSizeRelative:
          shape.IsMoveWithCell = true;
          shape.IsSizeWithCell = true;
          break;

        case Drawings.PositionRelative:
          shape.IsMoveWithCell = true;
          shape.IsSizeWithCell = false;
          break;

        case Drawings.PositionSizeAbsolute:
          shape.IsMoveWithCell = false;
          shape.IsSizeWithCell = false;
          break;

        default:
          throw new XmlException();
      }
    }
    /// <summary>
    /// Sets shape anchor.
    /// </summary>
    /// <param name="shape">Shape to set anchor for.</param>
    /// <param name="fromRect">Rectangle that defines top-left shape's position in Excel 2007 units.</param>
    /// <param name="toRect">Rectangle that defines bottom-right shape's position in Excel 2007 units.</param>
    /// <param name="shapeExtent">Width and height of the shape if extent token was present, -1 otherwise.</param>
    private void SetAnchor( ShapeImpl shape, Rectangle fromRect, Rectangle toRect, Size shapeExtent, bool bRelative )
    {
      if( shape == null )
        throw new ArgumentNullException( "shape" );

      IWorksheet sheet = shape.Worksheet as WorksheetImpl;
      fromRect = NormalizeAnchor( fromRect, sheet );
      toRect = NormalizeAnchor( toRect, sheet );

      MsofbtClientAnchor anchor = shape.ClientAnchor;
      anchor.LeftColumn = fromRect.Left;
      anchor.LeftOffset = fromRect.Width;

      anchor.TopRow = fromRect.Top;
      anchor.TopOffset = fromRect.Height;

      anchor.RightColumn = toRect.Left;
      anchor.RightOffset = toRect.Width;

      anchor.BottomRow = toRect.Top;
      anchor.BottomOffset = toRect.Height;

      //shape.ParseClientAnchor( shape.ClientAnchor );
      shape.EvaluateTopLeftPosition();

      if( shapeExtent.Width < 0 )
      {
        shape.UpdateHeight();
        shape.UpdateWidth();
      }
      else
      {
        shape.Width = shapeExtent.Width;
        shape.Height = shapeExtent.Height;
        anchor.OneCellAnchor = true;
      }
    }
    /// <summary>
    /// Converts anchor offsets from Excel 2007 units into Excel 97-2003 units (used by XlsIO).
    /// </summary>
    /// <param name="anchorPoint">Rectangle with shape coordinates (left-top or bottom-right corner).</param>
    /// <param name="sheet">Parent worksheet.</param>
    /// <returns>Rectangle with converted values from Excel 2007 coordinates space into Excel 97-2003 coordinates.</returns>
    private Rectangle NormalizeAnchor( Rectangle anchorPoint, IWorksheet sheet )
    {
      // if there is no parent worksheet (shape is placed inside chart)
      // we don't know how to convert coordinates, so we simply keep original ones.
      if( sheet == null )
        return anchorPoint;

      int iColumnWidth = 0;
      int iRowHeight = 0;

      int iColumnIndex = anchorPoint.Left + 1;
      int iRowIndex = anchorPoint.Top + 1;

      if( iRowIndex > sheet.Workbook.MaxRowCount )
      {
        anchorPoint.Y = sheet.Workbook.MaxRowCount - 1;
        iRowHeight = ShapeImpl.DEF_FULL_ROW_OFFSET;
      }
      else
      {
        double dRowOffset = anchorPoint.Height;
        dRowOffset = ApplicationImpl.ConvertToPixels( dRowOffset, MeasureUnits.EMU );
        iRowHeight = sheet.GetRowHeightInPixels( iRowIndex );
        iRowHeight = ( iRowHeight != 0 ) ?
          ( int )Math.Round( dRowOffset * ShapeImpl.DEF_FULL_ROW_OFFSET / ( double )iRowHeight ) :
          0;
      }

      if( iColumnIndex > sheet.Workbook.MaxColumnCount )
      {
        anchorPoint.X = sheet.Workbook.MaxColumnCount - 1;
        iColumnWidth = ShapeImpl.DEF_FULL_COLUMN_OFFSET;
      }
      else
      {
        double dColumnOffset = anchorPoint.Width;
        dColumnOffset = ApplicationImpl.ConvertToPixels( dColumnOffset, MeasureUnits.EMU );
        iColumnWidth = sheet.GetColumnWidthInPixels( iColumnIndex );
        iColumnWidth = ( iColumnWidth != 0 ) ?
          ( int )Math.Round( dColumnOffset * ShapeImpl.DEF_FULL_COLUMN_OFFSET / ( double )iColumnWidth ) :
          0;
      }

      anchorPoint.Width = iColumnWidth;
      anchorPoint.Height = iRowHeight;

      return anchorPoint;
    }
    /// <summary>
    /// Extracts anchor point from the specified XmlReader.
    /// </summary>
    /// <param name="reader">Reader to get data from.</param>
    /// <returns>Rectangle with extracted data.</returns>
    private Rectangle ParseAnchorPoint( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      Rectangle result = new Rectangle();

      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
          if (reader.NodeType == XmlNodeType.Element)
          {
              string strName = reader.LocalName;
              //int iValue = reader.ReadElementContentAsInt();
              string strValue = reader.ReadElementContentAsString();

              switch (strName)
              {
                  case Drawings.ColumnTagName:
                      result.X = XmlConvert.ToInt32(strValue);
                      break;

                  case Drawings.ColumnOffsetTagName:
                      result.Width = XmlConvert.ToInt32(strValue);
                      break;

                  case Drawings.RowTagName:
                      result.Y = XmlConvert.ToInt32(strValue);
                      break;

                  case Drawings.RowOffsetTagName:
                      result.Height = XmlConvert.ToInt32(strValue);
                      break;

                  case ChartConstants.XTagName:
                      result.X = (int)(XmlConvert.ToDouble(strValue) * ChartConstants.CoordinatesMultiplyer);
                      break;

                  case ChartConstants.YTagName:
                      result.Y = (int)(XmlConvert.ToDouble(strValue) * ChartConstants.CoordinatesMultiplyer);
                      break;

                  default:
                      throw new XmlException("Unexpected xml tag.");
              }
          }
          else
          {
              reader.Skip();
          }
      }

      reader.Read();
      return result;
    }
    /// <summary>
    /// Parse picture shape.
    /// </summary>
    /// <param name="reader">XmlReader to get picture data from.</param>
    /// <param name="sheet">Worksheet to place extracted shape into.</param>
    /// <param name="drawingsPath">Absolute path to the drawings.</param>
    /// <param name="lstRelationIds">List that will get relation id of the picture
    /// (used to remove parsed relations after parsing).</param>
    /// <param name="dictItemsToRemove">Dictionary with archive items to remove after parsing.</param>
    private ShapeImpl ParsePicture( XmlReader reader, WorksheetBaseImpl sheet,
      string drawingsPath, List<string> lstRelationIds, Dictionary<string, object> dictItemsToRemove )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( lstRelationIds == null )
        throw new ArgumentNullException( "lstRelationIds" );

      if( reader.LocalName != Drawings.PictureTagName )
        throw new XmlException( "Unexpected xml tag." );

      BitmapShapeImpl shape = new BitmapShapeImpl( sheet.Application, sheet.InnerShapes );
      WorksheetDataHolder worksheetHolder = sheet.DataHolder;
      RelationCollection relations = worksheetHolder.DrawingsRelations;
      FileDataHolder holder = worksheetHolder.ParentHolder;

      if( reader.MoveToAttribute( Drawings.MacroAttribute ) )
      {
        shape.Macro = reader.Value;
        reader.MoveToElement();
      }

      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case Drawings.NVPicturePropertiesTag:
                  ParsePictureProperties(reader, shape, relations, drawingsPath, holder, lstRelationIds, dictItemsToRemove);
              break;

            case Drawings.BlipFillTagName:
              ParseBlipFill( reader, shape, relations, drawingsPath, holder, lstRelationIds, dictItemsToRemove );
              break;

            case Drawings.ShapePropertiesTag:
              ParseShapeProperties( reader, shape );
              break;

            default:
              reader.Skip();
              break;
          }
        }
        else
        {
          reader.Skip();
        }
      }

      reader.Read();
      sheet.InnerShapes.AddPicture( shape );
      return shape;
    }
    /// <summary>
    /// This method extracts shape properties from the specified XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="shape">Shape to put extracted data into.</param>
    private void ParseShapeProperties( XmlReader reader, ShapeImpl shape )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( shape == null )
        throw new ArgumentNullException( "shape" );

      if( reader.LocalName != Drawings.ShapePropertiesTag )
        throw new XmlException( "Unexpected xml tag." );

      BitmapShapeImpl bitmap = shape as BitmapShapeImpl;

      if( bitmap != null )
      {
        Stream stream = new MemoryStream();
        XmlWriter writer = UtilityMethods.CreateWriter( stream, Encoding.UTF8 );
        writer.WriteNode( reader, false );
        writer.Flush();

        bitmap.ShapePropertiesStream = stream;
      }
      else
      {
        // NOTE: we don't support these properties, so we simply skip them.
        //throw new Exception( "The method or operation is not implemented." );
        reader.Skip();
      }
    }
    /// <summary>
    /// Extracts blip data from the specified XmlReader.
    /// </summary>
    /// <param name="reader">Reader to get blip data from.</param>
    /// <param name="shape">Shape to put extracted data into.</param>
    /// <param name="relations">Worksheet drawings relations collection.</param>
    /// <param name="parentPath">Path to the parent item.</param>
    /// <param name="holder">Parent file data holder.</param>
    /// <param name="lstRelationIds">List that will get relation id of the picture
    /// (used to remove parsed relations after parsing).</param>
    /// <param name="dictItemsToRemove">Dictionary with archive items to remove after parsing.</param>
    private void ParseBlipFill( XmlReader reader, BitmapShapeImpl shape,
      RelationCollection relations, string parentPath, FileDataHolder holder,
      List<string> lstRelationIds, Dictionary<string, object> dictItemsToRemove )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( shape == null )
        throw new ArgumentNullException( "shape" );

      if( lstRelationIds == null )
        throw new ArgumentNullException( "lstRelationIds" );

      if( reader.LocalName != Drawings.BlipFillTagName )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case Drawings.BlipTagName:
              ParseBlipTag( reader, shape, relations, parentPath, holder, lstRelationIds,
                dictItemsToRemove );
              break;

            case Drawings.SourceRectangleTagName:
              MemoryStream stream = new MemoryStream();
              XmlWriter writer = UtilityMethods.CreateWriter( stream, Encoding.UTF8 );
              writer.WriteNode( reader, false );
              writer.Flush();
              shape.SourceRectStream = stream;
              stream.Position = 0;
              XmlReader sourceRectReader = UtilityMethods.CreateReader(stream);
              if (sourceRectReader.MoveToAttribute(Drawings.LeftAttribute))
                  shape.CropLeftOffset = Convert.ToInt32(sourceRectReader.Value);
              if (sourceRectReader.MoveToAttribute(Drawings.TopAttribute))
                  shape.CropTopOffset = Convert.ToInt32(sourceRectReader.Value);
              if (sourceRectReader.MoveToAttribute(Drawings.RightAttribute))
                  shape.CropRightOffset = Convert.ToInt32(sourceRectReader.Value);
              if (sourceRectReader.MoveToAttribute(Drawings.BottomAttribute))
                  shape.CropBottomOffset = Convert.ToInt32(sourceRectReader.Value);
       
              break;

            case Drawings.StretchTagName:
            case Drawings.TileTagName:
              reader.Skip();
              break;

            default:
              throw new XmlException( "Unexpected xml tag." );
          }
        }
        else
        {
          reader.Skip();
        }
      }

      reader.Read();
    }
    /// <summary>
    /// Parses blip tag and sets appropriate image to the specified shape.
    /// </summary>
    /// <param name="reader">Reader to get data from.</param>
    /// <param name="shape">Shape to put image into.</param>
    /// <param name="relations">Collection with all drawings relations.</param>
    /// <param name="strParentPath">Path to the parent item (used to resolve related relation path).</param>
    /// <param name="holder">Parent file data holder.</param>
    /// <param name="lstRelationIds">List that will get relation id of the picture
    /// (used to remove parsed relations after parsing).</param>
    /// <param name="dictItemsToRemove">Dictionary with archive items to remove after parsing.</param>
    private void ParseBlipTag( XmlReader reader, BitmapShapeImpl shape, RelationCollection relations,
      string strParentPath, FileDataHolder holder, List<string> lstRelationIds,
      Dictionary<string, object> dictItemsToRemove )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( shape == null )
        throw new ArgumentNullException( "shape" );

      if( relations == null )
        throw new ArgumentNullException( "relations" );

      if( strParentPath == null )
        throw new ArgumentNullException( "strParentPath" );

      if( holder == null )
        throw new ArgumentNullException( "holder" );

      if( lstRelationIds == null )
        throw new ArgumentNullException( "lstRelationIds" );

      if( reader.MoveToAttribute( Drawings.EmbeddedPicture, Excel2007Serializator.RelationNamespace ) )
      {
        string strRelationId = reader.Value;
        Relation relation = relations[ strRelationId ];
        lstRelationIds.Add( strRelationId );

        if( relation == null )
          throw new XmlException( "Cannot find required relation" );

        ZipArchiveItem imageItem = holder[ relation, strParentPath ];
#if (SILVERLIGHT || WP)
        System.Drawing.Image picture = holder.GetImage(imageItem.ItemName);
#else
                  Image picture = holder.GetImage( imageItem.ItemName );
#endif
        shape.Picture = picture;

        // We have to store image item name insize bse, to serialize images correctly next time.
        WorkbookImpl book = ( WorkbookImpl )shape.Workbook;
        MsofbtBSE bse = ( MsofbtBSE )book.ShapesData.Pictures[ ( int )( shape.BlipId - 1 ) ];
        bse.PicturePath = imageItem.ItemName;
        dictItemsToRemove[ imageItem.ItemName ] = null;
      }

      reader.MoveToElement();

      if( !reader.IsEmptyElement )
      {
        MemoryStream streamBlipData = new MemoryStream();
        XmlWriter writer = UtilityMethods.CreateWriter( streamBlipData, Encoding.UTF8 );
        writer.WriteStartElement( Excel2007Serializator.TemporaryRoot );

        reader.Read();
        while( reader.NodeType != XmlNodeType.EndElement )
        {
            if (reader.LocalName == Drawings.ColorChangeTag)
                shape.HasTransparency = true;
          writer.WriteNode( reader, false );
        }

        writer.WriteEndElement();
        writer.Flush();

        shape.BlipSubNodesStream = streamBlipData;
      }

      reader.Skip();
    }
    /// <summary>
    /// This method parses picture properties.
    /// </summary>
    /// <param name="reader">Reader to extract data from.</param>
    /// <param name="shape">Shape to put properties into.</param>
    private void ParsePictureProperties(XmlReader reader, ShapeImpl shape, RelationCollection relations,
      string strParentPath, FileDataHolder holder, List<string> lstRelationIds,
      Dictionary<string, object> dictItemsToRemove)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        if (shape == null)
            throw new ArgumentNullException("shape");

        if (relations == null)
            throw new ArgumentNullException("relations");

        if (strParentPath == null)
            throw new ArgumentNullException("strParentPath");

        if (holder == null)
            throw new ArgumentNullException("holder");

        if (lstRelationIds == null)
            throw new ArgumentNullException("lstRelationIds");

      if( reader.LocalName != Drawings.NVPicturePropertiesTag )
        throw new XmlException( "Unexcpected xml tag" );

      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case Drawings.NVCanvasPropertiesTag:
              ParseNVCanvasProperties( reader, shape );
              break;

            case Drawings.NVPictureCanvasPropertiesTag:
              ParseNVPictureCanvas( reader, shape );
              break;
            case Drawings.ClickHyperlinkTag:
              ParseClickHyperlink(reader, shape, relations, strParentPath, holder, lstRelationIds, dictItemsToRemove);
              break;
            default:
              throw new XmlException( "Unexpected xml tag." );
          }
        }
        else
        {
          reader.Skip();
        }
      }

      reader.Read();
    }
    /// <summary>
    /// Parses non visual picture canvas properties.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="shape">Shape to place extracted data into.</param>
    private void ParseNVPictureCanvas( XmlReader reader, ShapeImpl shape )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( shape == null )
        throw new ArgumentNullException( "shape" );

      if( reader.LocalName != Drawings.NVPictureCanvasPropertiesTag )
        throw new XmlException( "Unexpected xml tag." );

      // NOTE: we don't support any of those properties, so we simply skip them.
      //throw new Exception( "The method or operation is not implemented." );
      reader.Skip();
    }
    /// <summary>
    /// Parses non visual canvas properties.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="shape">Shape to store extracted data.</param>
    public static void ParseNVCanvasProperties( XmlReader reader, IShape shape )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( shape == null )
        throw new ArgumentNullException( "shape" );

      if( reader.LocalName != Drawings.NVCanvasPropertiesTag )
        throw new XmlException( "Unexpected xml tag." );

      shape.IsShapeVisible = true;
      
        if (reader.MoveToAttribute(Drawings.IdAttributeName))
          //if (shape.GetType() == typeof(BitmapShapeImpl) || shape.GetType() == typeof(TextBoxShapeImpl))
              ((ShapeImpl)shape).ShapeId = XmlConvert.ToInt32(reader.Value);
          

      if( reader.MoveToAttribute( Drawings.NameAttributeName ) )
        shape.Name = reader.Value;

      if( reader.MoveToAttribute( Drawings.DescriptionAttributeName ) )
        shape.AlternativeText = reader.Value;

      if( reader.MoveToAttribute( Drawings.HiddenAttribute ) )
          shape.IsShapeVisible = !XmlConvert.ToBoolean(reader.Value);

      reader.MoveToElement();
      reader.Read ();
      if (reader.NodeType == XmlNodeType.EndElement)
          reader.Read();
    }
    /// <summary>
    /// Parses Click Hyper link properties.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="shape">Shape to store extracted data.</param>
    private void ParseClickHyperlink(XmlReader reader, ShapeImpl shape, RelationCollection relations,
      string strParentPath, FileDataHolder holder, List<string> lstRelationIds,
      Dictionary<string, object> dictItemsToRemove)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        if (shape == null)
            throw new ArgumentNullException("shape");

        if (relations == null)
            throw new ArgumentNullException("relations");

        if (strParentPath == null)
            throw new ArgumentNullException("strParentPath");

        if (holder == null)
            throw new ArgumentNullException("holder");

        if (lstRelationIds == null)
            throw new ArgumentNullException("lstRelationIds");
              
        
        if (reader.MoveToAttribute(Drawings.IdAttributeName,Excel2007Serializator.RelationNamespace))
        {
            string strRelationId = reader.Value;
            Relation relation = relations[strRelationId];
            lstRelationIds.Add(strRelationId);

            //if (relation == null)
            //    throw new XmlException("Cannot find required relation");

            shape.ImageRelation = relation;
            shape.IsHyperlink = true;
            reader.Skip();
        }        
        reader.Skip ();
    }
    /// <summary>
    /// Parses commentList tag.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="arrAuthors">List with comment authors.</param>
    /// <param name="sheet">Worksheet</param>
    private void ParseCommentList( XmlReader reader, List<string> arrAuthors, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( arrAuthors == null )
        throw new ArgumentNullException( "arrAuthors" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( reader.LocalName != Excel2007Serializator.CommentListTagName )
        throw new XmlException( "Unexpected tag" );

      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element
          && reader.LocalName == Excel2007Serializator.CommentTagName )
        {
          ParseComment( reader, arrAuthors, sheet );
        }
        else
        {
          reader.Skip();
        }
      }
    }
    /// <summary>
    /// This method extracts single comment from the specified reader.
    /// </summary>
    /// <param name="reader">XmlReader to get comment from.</param>
    /// <param name="authors">List of comment authors.</param>
    /// <param name="sheet">Parent worksheet for the comment.</param>
    private void ParseComment( XmlReader reader, IList<string> authors, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( authors == null )
        throw new ArgumentNullException( "authors" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( reader.LocalName != Excel2007Serializator.CommentTagName )
        throw new XmlException( "Unexpected xml tag" );

      if( !reader.MoveToAttribute( Excel2007Serializator.RefAttributeName ) )
        throw new XmlException();

      string strCellAddress = reader.Value;

      if( !reader.MoveToAttribute( Excel2007Serializator.AuthorIdAttributeName ) )
        throw new XmlException();

      int iAuthorId = int.Parse( reader.Value );

      CommentShapeImpl comment = ( CommentShapeImpl )sheet[ strCellAddress ].AddComment();
      comment.Author = authors[ iAuthorId ];

      if( !reader.IsEmptyElement )
      {
        do
        {
          reader.Read();
        }
        while( reader.NodeType != XmlNodeType.Element );

        if( reader.LocalName != Excel2007Serializator.CommentTextTagName )
          throw new XmlException( "Unexpected xml tag" );

        reader.Read();

        TextWithFormat commentText = ParseTextWithFormat( reader, Excel2007Serializator.CommentTextTagName );
        reader.Read();
        comment.SetText( commentText );
        reader.Read();
      }
    }
    /// <summary>
    /// Extracts authors from the specified reader.
    /// </summary>
    /// <param name="reader">XmlReader to get list of comment authors from.</param>
    /// <returns>List with comment authors.</returns>
    private List<string> ParseAuthors( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.NodeType != XmlNodeType.Element || reader.LocalName !=
        Excel2007Serializator.CommentAuthorsTagName )
      {
        throw new XmlException();
      }

      List<string> result = new List<string>();

      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element && reader.LocalName ==
          Excel2007Serializator.CommentAuthorTagName )
        {
          result.Add( reader.ReadElementContentAsString() );
          //reader.Read();
        }
        else
        {
          reader.Read();
        }
      }

      reader.Read();

      return result;
    }
    /// <summary>
    /// Extracts shape from the reader.
    /// </summary>
    /// <param name="reader">XmlReader to get shape settings from.</param>
    /// <param name="dictShapeIdToShape">Dictionary with default shape settings.
    /// Key - shape type, Value - default shape</param>
    /// <param name="relations">Corresponding relations collection.</param>
    /// <param name="parentItemPath">Path to the parent item.</param>
    private void ParseShape( XmlReader reader, Dictionary<string, ShapeImpl> dictShapeIdToShape,
      RelationCollection relations, string parentItemPath )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( dictShapeIdToShape == null )
        throw new ArgumentNullException( "dictShapeIdToShape" );

      if( parentItemPath == null || parentItemPath.Length == 0 )
        throw new ArgumentOutOfRangeException( "parentItemPath" );

      if( !reader.MoveToAttribute( Vml.TypeAttributeName ) )
        throw new XmlException();

      string strShapeType = UtilityMethods.RemoveFirstCharUnsafe( reader.Value );
      ShapeImpl defaultShape;

      if( !dictShapeIdToShape.TryGetValue( strShapeType, out defaultShape ) )
      {
        reader.Skip();
      }
      else
      {
        int iShapeInstance = -1;

        if( reader.MoveToAttribute( Vml.SptAttriubteName ) )
        {
          iShapeInstance = int.Parse( reader.Value );
        }
        else
        {
          iShapeInstance = defaultShape.InnerSpRecord.Instance;
        }

        ShapeParser parser;

        if( !m_dictShapeParsers.TryGetValue( iShapeInstance, out parser ) )
          throw new XmlException();

        reader.MoveToElement();
        // Save current shape
        MemoryStream shapeStream = new MemoryStream();
        XmlWriter writer = UtilityMethods.CreateWriter( shapeStream, Encoding.UTF8 );
        writer.WriteNode( reader, false );
        writer.Flush();

        shapeStream.Position = 0;
        reader = UtilityMethods.CreateReader( shapeStream );

        //while( reader.NodeType != XmlNodeType.Element )
        //  reader.Read();

        if( !parser.ParseShape( reader, defaultShape, relations, parentItemPath ) )
        {
          parser = new UnknownVmlShapeParser();
          shapeStream.Position = 0;
          reader = UtilityMethods.CreateReader( shapeStream );
          //reader.Read();
          int instance = defaultShape.Instance;
          Stream typeStream = defaultShape.XmlTypeStream;

          defaultShape = new ShapeImpl( defaultShape.Application, defaultShape.Parent );
          defaultShape.VmlShape = true;
          defaultShape.XmlTypeStream = typeStream;
          defaultShape.SetInstance( instance );
          parser.ParseShape( reader, defaultShape, relations, parentItemPath );
        }
        //reader.Read();
      }
    }
    /// <summary>
    /// Extracts shape type from the specified reader.
    /// </summary>
    /// <param name="reader">XmlReader to get shape type from.</param>
    /// <param name="sheet">Worksheet that is being currently parsed.</param>
    /// <param name="dictShapeIdToShape">Dictionary that will get default shape for the current shape type.</param>
    private void ParseShapeType( XmlReader reader, ShapeCollectionBase shapes,
      Dictionary<string, ShapeImpl> dictShapeIdToShape ,Stream layoutStream)
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( shapes == null )
        throw new ArgumentNullException( "shapes" );

      if( dictShapeIdToShape == null )
        throw new ArgumentNullException( "dictShapeIdToShape" );

      string strShapeId = null;
      string strShapeSpt = null;

      if( reader.MoveToAttribute( Vml.ShapeIdAttributeName ) )
        strShapeId = reader.Value;

      if( reader.MoveToAttribute( Vml.SptAttriubteName, Vml.ONamespace ) )
        strShapeSpt = reader.Value;
      else
          return;

      if( strShapeId == null || strShapeSpt == null )
        throw new XmlException();

      int iShapeInstance = int.Parse( strShapeSpt );
      ShapeParser parser;

      reader.MoveToElement();

      if( !m_dictShapeParsers.TryGetValue( iShapeInstance, out parser ) )
      {
        parser = new UnknownVmlShapeParser();
        m_dictShapeParsers[ iShapeInstance ] = parser;
        //reader.MoveToElement();
        //reader.Skip();
        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, iShapeInstance, "Unsupported shape type" );
        //throw new XmlException( "Unknown shape type" );
      }
      if (layoutStream != null)
          shapes.ShapeLayoutStream = layoutStream;
      ShapeImpl defaultShape = parser.ParseShapeType( reader, shapes );
      defaultShape.SetInstance( iShapeInstance );
      defaultShape.VmlShape = true;

      if( !dictShapeIdToShape.ContainsKey( strShapeId ) )
        dictShapeIdToShape.Add( strShapeId, defaultShape );
    }
    /// <summary>
    /// Extracts shape from the reader if shape type does not exist.
    /// </summary>
    /// <param name="reader">XmlReader to get shape settings from.</param>   
    /// <param name="shapes">Shape collection to process</param>
    /// <param name="relations">Corresponding relations collection.</param>
    /// <param name="parentItemPath">Path to the parent item.</param>
    private void ParseShapeWithoutType(XmlReader reader, ShapeCollectionBase shapes,
      RelationCollection relations, string parentItemPath)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        if (parentItemPath == null || parentItemPath.Length == 0)
            throw new ArgumentOutOfRangeException("parentItemPath");

        if (reader.MoveToAttribute(Vml.TypeAttributeName))
            throw new XmlException("shape type exists");

        ShapeParser parser;
        int iShapeInstance = -1;

        if (reader.MoveToAttribute(Vml.SptAttriubteName))
        {
            iShapeInstance = int.Parse(reader.Value);
        }

        if (!m_dictShapeParsers.TryGetValue(iShapeInstance, out parser))
        {
            parser = new UnknownVmlShapeParser();
            m_dictShapeParsers[iShapeInstance] = parser;
        }

        ShapeImpl defaultShape = new ShapeImpl(shapes.Application, shapes);
        defaultShape.SetInstance(iShapeInstance);
        defaultShape.VmlShape = true;

        reader.MoveToElement();
        // Save current shape
        MemoryStream shapeStream = new MemoryStream();
        XmlWriter writer = UtilityMethods.CreateWriter(shapeStream, Encoding.UTF8);
        writer.WriteNode(reader, false);
        writer.Flush();

        shapeStream.Position = 0;
        reader = UtilityMethods.CreateReader(shapeStream);

        if (!parser.ParseShape(reader, defaultShape, relations, parentItemPath))
        {
            parser = new UnknownVmlShapeParser();
            shapeStream.Position = 0;
            reader = UtilityMethods.CreateReader(shapeStream);
            //reader.Read();
            int instance = defaultShape.Instance;
            Stream typeStream = defaultShape.XmlTypeStream;

            defaultShape = new ShapeImpl(defaultShape.Application, defaultShape.Parent);
            defaultShape.VmlShape = true;
            defaultShape.XmlTypeStream = typeStream;
            defaultShape.SetInstance(instance);
            parser.ParseShape(reader, defaultShape, relations, parentItemPath);
        }
    }
    /// <summary>
    /// Parses rich text run.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <returns>Index of the added string.</returns>
    private int ParseRichTextRun( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.LocalName != Excel2007Serializator.RichTextRunTagName )
        throw new XmlException( "reader" );

      SSTDictionary dictionarySST = m_book.InnerSST;
      TextWithFormat textWithFormat = ParseTextWithFormat( reader, Excel2007Serializator.StringItemTagName );
      return dictionarySST.AddIncrease( textWithFormat, false );
    }
    /// <summary>
    /// Extracts TextWithFormat from the specified XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="closingTagName">Tag that means that we have to stop parsing.</param>
    /// <returns>Parsed text with format.</returns>
    private TextWithFormat ParseTextWithFormat( XmlReader reader, string closingTagName )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( closingTagName == null || closingTagName.Length == 0 )
        throw new ArgumentOutOfRangeException( "closingTagName" );

      TextWithFormat textWithFormat = new TextWithFormat();

      while( reader.LocalName != closingTagName && reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element
          && reader.LocalName == Excel2007Serializator.RichTextRunTagName )
        {
          ParseFormattingRun( reader, textWithFormat );
        }
        else
        {
          reader.Skip();
        }
      }

      return textWithFormat;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="textWithFormat"></param>
    private void ParseFormattingRun( XmlReader reader, TextWithFormat textWithFormat )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( textWithFormat == null )
        throw new ArgumentNullException( "textWithFormat" );

      int iFontIndex = -1;
      int iStartPos = -1;

      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement
        || reader.LocalName != Excel2007Serializator.RichTextRunTagName )
      {
        if( reader.LocalName == Excel2007Serializator.RichTextRunPropertiesTagName )
        {
          iFontIndex = ParseFont( reader, null );
          reader.Skip();
        }
        else if( reader.LocalName == Excel2007Serializator.TextTagName )
        {
          if( !reader.IsEmptyElement )
          {
            iStartPos = textWithFormat.Text.Length;
            reader.Read();
            string strText = reader.Value;
            strText = strText.Replace( "\r", string.Empty );
            textWithFormat.Text += strText;
            reader.Skip();

            if( reader.NodeType == XmlNodeType.EndElement &&
              reader.LocalName == Excel2007Serializator.TextTagName )
            {
              reader.Skip();
            }
          }
          else
          {
            reader.Skip();
          }
        }
        else
        {
          reader.Skip();
        }
      }

      reader.Skip();

      if( iFontIndex >= 0 && iStartPos >= 0 )
      {
        textWithFormat.SetTextFontIndex( iStartPos, textWithFormat.Text.Length - 1, iFontIndex );
      }
    }
    /// <summary>
    /// Parses text content.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <returns>Parsed string index.</returns>
    private int ParseText( XmlReader reader, bool setCount )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.LocalName != Excel2007Serializator.TextTagName )
        throw new XmlException( "reader" );

      SSTDictionary dictionarySST = m_book.InnerSST;
      bool isPreserved = false;
      if (reader.XmlSpace==XmlSpace.Preserve)
      {
          isPreserved = true;
      }
      reader.Read();
      string strText = XmlConvert.DecodeName( reader.Value );
      strText = strText.Replace( "\r", string.Empty );
      int iResult;
      if (isPreserved)
      {
          TextWithFormat textWithFormat = new TextWithFormat();
          textWithFormat.IsPreserved = true;
          textWithFormat.Text = strText;
          reader.Skip();
          reader.Skip();
          return dictionarySST.AddIncrease(textWithFormat, false);
      }
      else
      {
          iResult = dictionarySST.AddIncrease(strText, setCount);
      }
      reader.Skip();
      if(!string.IsNullOrEmpty(strText))
       reader.Skip();
      return iResult;
    }
    /// <summary>
    /// Parses text content.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <returns>Parsed string index.</returns>
    private int ParseText(XmlReader reader, bool setCount,out string text)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        if (reader.LocalName != Excel2007Serializator.TextTagName)
            throw new XmlException("reader");

        SSTDictionary dictionarySST = m_book.InnerSST;

        reader.Read();
        string strText =text= XmlConvert.DecodeName(reader.Value);
        strText = strText.Replace("\r", string.Empty);
        int iResult = dictionarySST.AddIncrease( strText, setCount );
        reader.Skip();

        reader.Skip();
        return iResult;
    }
    /// <summary>
    /// Extracts named styles from specified XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="arrFontIndexes">List with updated font indexes, index - font index
    /// in the file, value - font index in our document (we can change indexes during parsing).</param>
    /// <param name="arrFills">List with extracted fill objects.</param>
    /// <param name="arrBorders">List with extracted borders.</param>
    /// <returns>List with indexes of created extended formats.</returns>
    private List<int> ParseNamedStyles( XmlReader reader, List<int> arrFontIndexes,
      List<FillImpl> arrFills, List<BordersCollection> arrBorders )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( arrFontIndexes == null )
        throw new ArgumentNullException( "arrFontIndexes" );

      if( arrFills == null )
        throw new ArgumentNullException( "arrFills" );

      if( arrBorders == null )
        throw new ArgumentNullException( "arrBorders" );

      if( reader.NodeType != XmlNodeType.Element ||
        reader.LocalName != Excel2007Serializator.NamedStyleXFsTagName )
      {
        throw new XmlException( "Unexpected xml tag " + reader.LocalName );
      }

      if( reader.IsEmptyElement )
        return null;

      reader.Read();

      List<int> arrResult = new List<int>();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          ExtendedFormatImpl xf = ParseExtendedFormat( reader, arrFontIndexes, arrFills, arrBorders, null, null );
          xf.Record.ParentIndex = (ushort)m_book.MaxXFCount;
          // TODO: maybe we have to call ForceAdd method here.
          xf = ( ExtendedFormatImpl )m_book.InnerExtFormats.ForceAdd( xf );
          arrResult.Add( ( int )xf.Index );
        }

        reader.Read();
      }

      return arrResult;
    }
    /// <summary>
    /// Extracts cell formats from specified XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="arrNewFontIndexes">List with updated font indexes, index - font index
    /// in the file, value - font index in our document (we can change indexes during parsing).</param>
    /// <param name="arrFills">List with extracted fill objects.</param>
    /// <param name="arrBorders">List with extracted borders.</param>
    /// <param name="namedStyleIndexes">List with updated parent indexes, index - xfId in
    /// the xml document, value - xf index in our internal collection.</param>
    /// <returns>List with indexes of created extended formats.</returns>
    private List<int> ParseCellFormats( XmlReader reader, List<int> arrNewFontIndexes,
      List<FillImpl> arrFills, List<BordersCollection> arrBorders, List<int> namedStyleIndexes )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if (namedStyleIndexes == null)
      {
          m_book.InsertDefaultFonts();
          m_book.InsertDefaultValues();
      }

      if( arrNewFontIndexes == null )
        throw new ArgumentNullException( "arrNewFontIndexes" );

      if( arrFills == null )
        throw new ArgumentNullException( "arrFills" );

      if( arrBorders == null )
        throw new ArgumentNullException( "arrBorders" );

      if( reader.NodeType != XmlNodeType.Element ||
        reader.LocalName != Excel2007Serializator.CellFormatXFsTagName )
      {
        throw new XmlException( "Unexpected xml tag " + reader.LocalName );
      }

      if( reader.IsEmptyElement ) return null;

      reader.Read();

      List<int> arrResult = new List<int>();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          ExtendedFormatImpl xf = ParseExtendedFormat( reader, arrNewFontIndexes, arrFills, arrBorders, namedStyleIndexes, false );
          xf = ( ExtendedFormatImpl )m_book.InnerExtFormats.Add( xf );
          arrResult.Add( xf.Index );
        }

        reader.Read();
      }

      return arrResult;
    }
    /// <summary>
    /// Parses named style settings (name, etc.).
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="arrNamedStyleIndexes">List with modified indexes to named
    /// style's extended formats.</param>
    private void ParseStyles( XmlReader reader, List<int> arrNamedStyleIndexes )
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        if (arrNamedStyleIndexes == null)
        {
            m_book.InsertDefaultFonts();
            m_book.InsertDefaultValues();
        }

        if (reader.NodeType != XmlNodeType.Element ||
          reader.LocalName != Excel2007Serializator.CellStylesTagName)
        {
            throw new XmlException("Unexpected xml element " + reader.LocalName);
        }

        reader.Read();

        m_book.InnerStyles.Clear();

        List<int> validate = new List<int>();

        while (reader.NodeType != XmlNodeType.EndElement || reader.LocalName != Excel2007Serializator.CellStylesTagName)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                if (reader.LocalName == Excel2007Serializator.CellStyleTagName)
                {
                    ParseStyle(reader, arrNamedStyleIndexes, ref validate);
                }
                else
                {
                    throw new XmlException("Unexpected xml tag " + reader.LocalName);
                }
            }

            reader.Read();
        }
    }
    /// <summary>
    /// Parses single style object.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="arrNamedStyleIndexes">List with modified indexes to named
    /// style's extended formats.</param>
    private void ParseStyle(XmlReader reader, List<int> arrNamedStyleIndexes, ref List<int> validate)
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( arrNamedStyleIndexes == null )
        throw new ArgumentNullException( "arrNamedStyleIndexes" );

      if( reader.NodeType != XmlNodeType.Element ||
        reader.LocalName != Excel2007Serializator.CellStyleTagName )
      {
        throw new XmlException( "Unexpected xml item " + reader.LocalName );
      }

      StyleRecord style = ( StyleRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Style );

      if( reader.MoveToAttribute( Excel2007Serializator.NameAttributeName ) )
      {
        style.StyleName = reader.Value;
      }

      if( reader.MoveToAttribute( Excel2007Serializator.XFIdAttributeName ) )
      {
        int iXFIndex = XmlConvert.ToInt32( reader.Value );
        style.ExtendedFormatIndex = ( ushort )arrNamedStyleIndexes[ iXFIndex ];

          if ( m_book.Version == ExcelVersion.Excel97to2003 )
              style.DefXFIndex = 0;
          else
              style.DefXFIndex = 65535;
      }

      if( reader.MoveToAttribute( Excel2007Serializator.StyleBuiltinIdAttributeName ) )
      {
        int iBuiltinId = XmlConvert.ToInt32( reader.Value );
        style.BuildInOrNameLen = ( byte )iBuiltinId;
        style.IsBuildInStyle = true;
      }
      if (reader.MoveToAttribute(Excel2007Serializator.StyleCustomizedAttributeName))
      {          
          style.IsBuiltIncustomized = ParseBoolean(reader, Excel2007Serializator.StyleCustomizedAttributeName, false);
      }
      if( reader.MoveToAttribute( Excel2007Serializator.OutlineLevelAttribute ) )
      {
        style.OutlineStyleLevel = XmlConvert.ToByte( reader.Value );
      }

      if (!validate.Contains(style.ExtendedFormatIndex) || style.IsBuildInStyle)
      {
          validate.Add(style.ExtendedFormatIndex);
          m_book.InnerStyles.Add(style);
      }
    }
    /// <summary>
    /// Extracts single extended format from the XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="arrFontIndexes">List with updated font indexes, index - font index
    /// in the file, value - font index in our document (we can change indexes during parsing).</param>
    /// <param name="arrFills">List with extracted fill objects.</param>
    /// <param name="arrBorders">List with extracted borders.</param>
    /// <param name="namedStyleIndexes">List with updated parent indexes.</param>
    /// <returns>Created ExtendedFormat object.</returns>
    private ExtendedFormatImpl ParseExtendedFormat( XmlReader reader, List<int> arrFontIndexes,
      List<FillImpl> arrFills, List<BordersCollection> arrBorders, List<int> namedStyleIndexes, bool? includeDefault )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( arrFontIndexes == null )
        throw new ArgumentNullException( "arrFontIndexes" );

      if( arrFills == null )
        throw new ArgumentNullException( "arrFills" );

      if( arrBorders == null )
        throw new ArgumentNullException( "arrBorders" );

      if( reader.NodeType != XmlNodeType.Element ||
        reader.LocalName != Excel2007Serializator.ExtendedFormatTagName )
      {
        throw new XmlException( "Unexpected xml tag " + reader.LocalName );
      }
      bool hasAlignment;
      ExtendedFormatImpl format = new ExtendedFormatImpl( m_book.Application, m_book );
      ExtendedFormatRecord record = format.Record;
      ExtendedXFRecord xfExtRecord = format.XFRecord;

      if( reader.MoveToAttribute( Excel2007Serializator.XFIdAttributeName ) )
      {
        int iParent = XmlConvert.ToUInt16( reader.Value );
          if(namedStyleIndexes !=null)
        record.ParentIndex = ( ushort )namedStyleIndexes[ iParent ];
        record.XFType = ExtendedFormatRecord.TXFType.XF_STYLE;
      }
      else if (namedStyleIndexes != null)
      {
          record.ParentIndex = (ushort)namedStyleIndexes[0];
          record.XFType = ExtendedFormatRecord.TXFType.XF_STYLE;
      }
      else
      {
        record.ParentIndex = ( ushort )m_book.MaxXFCount;
        record.XFType = ExtendedFormatRecord.TXFType.XF_CELL;
      }

      ParseFontFillBorder( reader, format, arrFontIndexes, arrFills, arrBorders );

      if( reader.MoveToAttribute( Excel2007Serializator.NumberFormatIdAttributeName ) )
        record.FormatIndex = XmlConvert.ToUInt16( reader.Value );

      ParseIncludeAttributes( reader, format, includeDefault ,out hasAlignment);      
      format.HorizontalAlignment = ExcelHAlign.HAlignGeneral;
      format.VerticalAlignment = ExcelVAlign.VAlignBottom;
      ParseAlignmentAndProtection(reader, format, hasAlignment);

      format = m_book.AddExtendedProperties(format);
      return format;
    }
    /// <summary>
    /// Parse alignment and protection properties for extended format
    /// if they are present in the document.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="record">ExtendedFormatRecord to put data into.</param>
    private void ParseAlignmentAndProtection( XmlReader reader, ExtendedFormatImpl format ,bool hasAlignment )
    {
      ExtendedFormatRecord record = format.Record;
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( record == null )
        throw new ArgumentNullException( "record" );

      reader.MoveToElement();

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while (reader.NodeType != XmlNodeType.EndElement || reader.LocalName != Excel2007Serializator.ExtendedFormatTagName)
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case Excel2007Serializator.AlignmentTagName:
                ParseAlignment( reader, record );
                if(!hasAlignment)
                format.IncludeAlignment = true;
                break;

              case Excel2007Serializator.ProtectionTagName:
                ParseProtection( reader, record );
                break;

              default:
                throw new NotImplementedException( reader.LocalName );
            }
          }

          reader.Read();
        }
      }
    }
    /// <summary>
    /// Parses alignment settings.
    /// </summary>
    /// <param name="reader">XmlReader to get alignment data from.</param>
    /// <param name="record">Record to write alignment data into.</param>
    private void ParseAlignment( XmlReader reader, ExtendedFormatRecord record )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( record == null )
        throw new ArgumentNullException( "record" );

      if( reader.NodeType != XmlNodeType.Element ||
        reader.LocalName != Excel2007Serializator.AlignmentTagName )
      {
        throw new XmlException();
      }

      if( reader.MoveToAttribute( Excel2007Serializator.HAlignAttributeName ) )
      {
        string strHAlign = reader.Value;
        record.HAlignmentType = ( ExcelHAlign )Enum.Parse( typeof( Excel2007HAlign ), strHAlign, true );
      }

      if( reader.MoveToAttribute( Excel2007Serializator.IndentAttributeName ) )
        record.Indent = XmlConvert.ToByte( reader.Value );

      if( reader.MoveToAttribute( Excel2007Serializator.JustifyLastLineAttributeName ) )
        record.JustifyLast = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( Excel2007Serializator.ReadingOrderAttributeName ) )
        record.ReadingOrder = XmlConvert.ToUInt16( reader.Value );

      if( reader.MoveToAttribute( Excel2007Serializator.ShrinkToFitAttributeName ) )
        record.ShrinkToFit = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( Excel2007Serializator.TextRotationAttributeName ) )
        record.Rotation = XmlConvert.ToUInt16( reader.Value );

      if( reader.MoveToAttribute( Excel2007Serializator.WrapTextAttributeName ) )
        record.WrapText = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( Excel2007Serializator.VerticalAttributeName ) )
      {
        string strVAlign = reader.Value;
        record.VAlignmentType = ( ExcelVAlign )Enum.Parse( typeof( Excel2007VAlign ), strVAlign, true );
      }
    }
    /// <summary>
    /// Parses protection settings.
    /// </summary>
    /// <param name="reader">XmlReader to get protection data from.</param>
    /// <param name="record">Record to write protection data into.</param>
    private void ParseProtection( XmlReader reader, ExtendedFormatRecord record )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( record == null )
        throw new ArgumentNullException( "record" );

      if( reader.NodeType != XmlNodeType.Element || reader.LocalName != Excel2007Serializator.ProtectionTagName )
        throw new XmlException( "Unable to locate necessary xml tag" );

      if( reader.MoveToAttribute( Excel2007Serializator.HiddenAttributeName ) )
        record.IsHidden = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( Excel2007Serializator.LockedAttributeName ) )
        record.IsLocked = XmlConvert.ToBoolean( reader.Value );
    }
    /// <summary>
    /// Extracts Include (IncludeAlignment, IncludeFont, etc.) attributes from XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="format">ExtendedFormat to put data into.</param>
    private void ParseIncludeAttributes( XmlReader reader, ExtendedFormatImpl format, bool? defaultValue ,out bool hasAlignment)
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( format == null )
        throw new ArgumentNullException( "format" );
      hasAlignment = false;
      ExtendedFormatRecord record = format.Record;
     // if (defaultValue!=null)
      //defaultValue = true;
      //bool bHasParent = record.ParentIndex != m_book.MaxXFCount;
      // TODO: 1.1 It is possible that option value will depend on HasParent
      // TODO: 1.2 Maybe we should perform some additional work in every case
      // (named style and cell format).

      if( reader.MoveToAttribute( Excel2007Serializator.IncludeAlignmentAttributeName ) )
      {
        format.IncludeAlignment = XmlConvert.ToBoolean( reader.Value );
        hasAlignment = true;
      }
      else if( defaultValue != null )
      {
        record.IsNotParentAlignment = ( bool )defaultValue;
      }

      if( reader.MoveToAttribute( Excel2007Serializator.IncludeBorderAttributeName ) )
      {
        format.IncludeBorder = XmlConvert.ToBoolean( reader.Value );
      }
      else if( defaultValue != null && record.BorderIndex < 1)
      {
        record.IsNotParentBorder = ( bool )defaultValue;
      }
      else if (record.BorderIndex > 0)
      {
          format.IncludeBorder = true;
      }


      if( reader.MoveToAttribute( Excel2007Serializator.IncludeFontAttributeName ) )
      {
        format.IncludeFont = XmlConvert.ToBoolean( reader.Value );
      }
      else if( defaultValue != null && record.FontIndex < 1)
      {
        record.IsNotParentFont = ( bool )defaultValue;
      }
      else if (record.FontIndex > 0)
      {
          format.IncludeFont = true;
      }

      if( reader.MoveToAttribute( Excel2007Serializator.IncludeNumberFormatAttributeName ) )
      {
        format.IncludeNumberFormat = XmlConvert.ToBoolean( reader.Value );
      }
      else if( defaultValue != null )
      {
        record.IsNotParentFormat = ( bool )defaultValue;
        if (record.FormatIndex > 0)
            format.IncludeNumberFormat = true;
      }

      if( reader.MoveToAttribute( Excel2007Serializator.IncludePatternsAttributeName ) )
      {
        format.IncludePatterns = XmlConvert.ToBoolean( reader.Value );
      }
      else if( defaultValue != null && record.FillIndex < 1)
      {
        record.IsNotParentPattern = ( bool )defaultValue;
      }
      else if (record.FillIndex > 0)
      {
          format.IncludePatterns = true;
      }

      if( reader.MoveToAttribute( Excel2007Serializator.IncludeProtectionAttributeName ) )
      {
        format.IncludeProtection = XmlConvert.ToBoolean( reader.Value );
      }
      else if( defaultValue != null )
      {
        record.IsNotParentCellOptions = ( bool )defaultValue;
      }
      defaultValue = false;
      if (reader.MoveToAttribute(Excel2007Serializator.QuotePreffixAttributeName))
      {
          format.IsFirstSymbolApostrophe = XmlConvert.ToBoolean(reader.Value);
      }
      else if (defaultValue != null)
      {
          record._123Prefix = (bool)defaultValue;
      }
    }
    /// <summary>
    /// This method extracts font, fill and border settings from specified
    /// XmlReader and sets inside specified ExtendedFormat.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="extendedFormat">Extended format to put data into.</param>
    /// <param name="arrFontIndexes">List with updated font indexes, index - font index
    /// in the file, value - font index in our document (we can change indexes during parsing).</param>
    /// <param name="arrFills">List with extracted fill objects.</param>
    /// <param name="arrBorders">List with extracted borders.</param>
    private void ParseFontFillBorder( XmlReader reader, ExtendedFormatImpl extendedFormat,
      List<int> arrFontIndexes, List<FillImpl> arrFills, List<BordersCollection> arrBorders )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( extendedFormat == null )
        throw new ArgumentNullException( "extendedFormat" );

      if( arrFontIndexes == null )
        throw new ArgumentNullException( "arrFontIndexes" );

      if( arrFills == null )
        throw new ArgumentNullException( "arrFills" );

      if( arrBorders == null )
        throw new ArgumentNullException( "arrBorders" );

      ExtendedFormatRecord record = extendedFormat.Record;

      if( reader.MoveToAttribute( Excel2007Serializator.FontIdAttributeName ) )
      {
        int iFontIndex = XmlConvert.ToInt32( reader.Value );
        record.FontIndex = ( ushort )arrFontIndexes[ iFontIndex ];
      }

      if( reader.MoveToAttribute( Excel2007Serializator.FillIdAttributeName ) )
      {
        int iFillIndex = XmlConvert.ToInt32( reader.Value );
        record.FillIndex = (ushort)iFillIndex;
        FillImpl fill = arrFills[ iFillIndex ];
        CopyFillSettings( fill, extendedFormat );
      }

      if( reader.MoveToAttribute( Excel2007Serializator.BorderIdAttributeName ) )
      {
        int iBorderIndex = XmlConvert.ToInt32( reader.Value );
        record.BorderIndex = (ushort)iBorderIndex;
        if (iBorderIndex > 0)
            extendedFormat.HasBorder = true;
        if (iBorderIndex == arrBorders.Count)
            iBorderIndex = arrBorders.Count - 1;
        BordersCollection borders = arrBorders[ iBorderIndex ];
        CopyBorderSettings( borders, extendedFormat );
      }
    }
    /// <summary>
    /// Copies border settings from specified borders collection into ExtendedFormatImpl.
    /// </summary>
    /// <param name="borders">BordersCollection to copy settings from.</param>
    /// <param name="format">Format object to copy into.</param>
    private void CopyBorderSettings( BordersCollection borders, ExtendedFormatImpl format )
    {
      if( borders == null )
        throw new ArgumentNullException( "borders" );

      if( format == null )
        throw new ArgumentNullException( "record" );
		
		ExtendedFormatRecord record = format.Record;		

      IBorder border = borders[ ExcelBordersIndex.EdgeLeft ];

      if( border != null && ( format.IncludeBorder || BordersDifferent( border, format.LeftBorderColor, format.LeftBorderLineStyle ) ) )
      {
        format.IncludeBorder = true;
        format.LeftBorderColor.CopyFrom( border.ColorObject, true );
        format.LeftBorderLineStyle = border.LineStyle;
      }
      else if(border != null)
      {
          record.BorderLeft = border.LineStyle;
      }

      border = borders[ ExcelBordersIndex.EdgeRight ];

      if( border != null && ( format.IncludeBorder || BordersDifferent( border, format.RightBorderColor, format.RightBorderLineStyle ) ) )
      {
        format.IncludeBorder = true;
        format.RightBorderColor.CopyFrom( border.ColorObject, true );
        format.RightBorderLineStyle = border.LineStyle;
      }
      else if(border != null)
      {
          record.BorderRight = border.LineStyle;
      }

      border = borders[ ExcelBordersIndex.EdgeTop ];

      if( border != null && ( format.IncludeBorder || BordersDifferent( border, format.TopBorderColor, format.TopBorderLineStyle ) ) )
      {
        format.IncludeBorder = true;
        format.TopBorderColor.CopyFrom( border.ColorObject, true );
        format.TopBorderLineStyle = border.LineStyle;
      }
      else if (border != null)
      {
          record.BorderTop = border.LineStyle;
      }

      border = borders[ ExcelBordersIndex.EdgeBottom ];

      if( border != null && ( format.IncludeBorder || BordersDifferent( border, format.BottomBorderColor, format.BottomBorderLineStyle ) ) )
      {
        format.IncludeBorder = true;
        format.BottomBorderColor.CopyFrom( border.ColorObject, true );
        format.BottomBorderLineStyle = border.LineStyle;
      }
      else if (border != null)
      {
          record.BorderBottom = border.LineStyle;
      }

      border = borders[ ExcelBordersIndex.DiagonalDown ];

      if( border != null )
      {
        if( format.IncludeBorder || BordersDifferent( border, format.DiagonalBorderColor, format.DiagonalDownBorderLineStyle ) )
        {
          format.IncludeBorder = true;
          format.DiagonalBorderColor.CopyFrom( border.ColorObject, true );
          format.DiagonalDownBorderLineStyle = border.LineStyle;
        }
        else
        {
            record.DiagonalLineStyle = (ushort)border.LineStyle;
            record.DiagonalFromTopLeft = border.ShowDiagonalLine;
        }      
        format.DiagonalDownVisible = border.ShowDiagonalLine;
      }

      border = borders[ ExcelBordersIndex.DiagonalUp ];

      if( border != null )
      {
        if( format.IncludeBorder || BordersDifferent( border, format.DiagonalBorderColor, format.DiagonalUpBorderLineStyle ) )
        {
          format.IncludeBorder = true;
          format.DiagonalBorderColor.CopyFrom( border.ColorObject, true );
          format.DiagonalUpBorderLineStyle = border.LineStyle;
        }
        else
        {
            record.DiagonalLineStyle = (ushort)border.LineStyle;
            record.DiagonalFromBottomLeft = border.ShowDiagonalLine;
        }        
        format.DiagonalUpVisible = border.ShowDiagonalLine;
      }
    }
    private static bool BordersDifferent( IBorder border, ColorObject color, ExcelLineStyle lineStyle )
    {
      ColorObject borderColor = border.ColorObject;
      return ( borderColor.ColorType == color.ColorType || borderColor.Value != color.Value || border.LineStyle != lineStyle );
    }
    /// <summary>
    /// Extracts single relation item from the reader and puts it into relations collection.
    /// </summary>
    /// <param name="reader">XmlReader to extract relation from.</param>
    /// <param name="relations">Relations collection that should get extracted value.</param>
    private static void ParseRelation( XmlReader reader, RelationCollection relations )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( relations == null )
        throw new ArgumentNullException( "relations" );

      string strId = null;
      string strType = null;
      string strTarget = null;
      bool bExternal = false;
      string styleDoc = "styles.xml";

      if( reader.MoveToAttribute( Excel2007Serializator.RelationIdAttribute ) )
        strId = reader.Value;

      if( reader.MoveToAttribute( Excel2007Serializator.RelationTypeAttribute ) )
        strType = reader.Value;

      if (reader.MoveToAttribute(Excel2007Serializator.RelationTargetAttribute))
      {
          strTarget = reader.Value;
          if (strTarget.ToLower().Contains(styleDoc))
              strTarget = styleDoc;
      }

      if( reader.MoveToAttribute( Excel2007Serializator.RelationTargetModeAttribute ) )
        bExternal = reader.Value == Excel2007Serializator.RelationExternalTargetMode;

      Relation relation = new Relation( strTarget, strType, bExternal );
      relations[ strId ] = relation;
    }
    /// <summary>
    /// Parses sheet options.
    /// </summary>
    /// <param name="reader">XmlReader to extract sheet options from (name, visibility, relation, etc.).</param>
    /// <param name="relations">Workbook relations.</param>
    /// <param name="holder">Object that holds document data.</param>
    /// <param name="bookPath">Absolute path in zip archive to the parent workbook.</param>
    private void ParseSheetsOptions( XmlReader reader, RelationCollection relations,
      FileDataHolder holder, string bookPath )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( relations == null )
        throw new ArgumentNullException( "relations" );

      if( reader.LocalName != Excel2007Serializator.SheetsTagName )
        throw new XmlException( "Unexpected tag name " + reader.LocalName );

      m_book.Objects.Clear();
      m_book.InnerWorksheets.Clear();
      m_book.InnerCharts.Clear();

      reader.Read();
      int sheetRelationCount = 0;
      while( reader.NodeType != XmlNodeType.EndElement || reader.LocalName != Excel2007Serializator.SheetsTagName )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          if( reader.LocalName == Excel2007Serializator.SheetTagName )
          {
            ParseWorkbookSheetEntry( reader, relations, holder, bookPath ,++sheetRelationCount );
          }
          else
          {
            reader.Skip();
          }
        }
        else
        {
          reader.Read();
        }
      }

      reader.Read();
    }
    /// <summary>
    /// Parses sheet entry from workbook item.
    /// </summary>
    /// <param name="reader">Reader to extract data from.</param>
    /// <param name="relations">Workook's relations collection.</param>
    /// <param name="holder">FileDataHolder that stores document data.</param>
    /// <param name="bookPath">Absolute path in zip archive to the parent workbook.</param>
    private void ParseWorkbookSheetEntry( XmlReader reader, RelationCollection relations,
      FileDataHolder holder, string bookPath,int sheetRelationIdCount )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( relations == null )
        throw new ArgumentNullException( "relations" );
      
      string strSheetName = null;
      string strState = null;
      string strRelation = null;
      string strSheetId = null;

      if( reader.MoveToAttribute( Excel2007Serializator.SheetNameAttribute ) )
        strSheetName = reader.Value;

      if( reader.MoveToAttribute( Excel2007Serializator.SheetStateAttributeName ) )
        strState = reader.Value;

      if( reader.MoveToAttribute( Excel2007Serializator.RelationAttribute,
        Excel2007Serializator.RelationNamespace ) )
      {
        strRelation = reader.Value;
      }

      if( reader.MoveToAttribute( Excel2007Serializator.SheetIdAttribute ) )
        strSheetId = reader.Value;

      Relation relation = relations[ strRelation ];

      if (relation == null)
      {
          string target = string.Format(Excel2007Serializator.DefaultWorksheetPathFormat, sheetRelationIdCount);
          relation = new Relation(target , Syncfusion.XlsIO.Implementation.XmlSerialization.Excel2007Serializator.WorksheetPartType);
          string relationId = string.Format(Excel2007Serializator.RelationIdFormat, relations.Count);
          relations[relationId] = relation;
          strRelation = relationId;
      }
      WorksheetBaseImpl sheet = null;

      switch( relation.Type )
      {
        case Excel2007Serializator.WorksheetPartType:
          sheet = ( WorksheetBaseImpl )m_book.InnerWorksheets.Add( strSheetName );
          // TODO: finish worksheet parsing.
          break;

        case Excel2007Serializator.ChartSheetPartType:
          sheet = ( WorksheetBaseImpl )m_book.InnerCharts.Add( strSheetName );
          //Default page orientation for chart is landscape
          sheet.PageSetupBase.Orientation = ExcelPageOrientation.Landscape;
          // TODO: finish worksheet parsing.
          break;

        default:
          throw new XmlException( "Unknown part type: " + relation.Type );
      }

      sheet.DataHolder = new WorksheetDataHolder( holder, relation, bookPath );
      sheet.m_dataHolder.RelationId = strRelation;
      sheet.m_dataHolder.SheetId = strSheetId;
      sheet.IsSaved = true;

      SetVisibilityState( sheet, strState );
      relations.Remove( strRelation );
    }
    /// <summary>
    /// Sets sheet visibility.
    /// </summary>
    /// <param name="sheet">Worksheet to set visibility into.</param>
    /// <param name="strVisibility">Visibility string value.</param>
    private void SetVisibilityState( WorksheetBaseImpl sheet, string strVisibility )
    {
      if( strVisibility == null )
        return;

      switch( strVisibility )
      {
        case Excel2007Serializator.StateHidden:
          sheet.Visibility = WorksheetVisibility.Hidden;
          break;

        case Excel2007Serializator.StateVeryHidden:
          sheet.Visibility = WorksheetVisibility.StrongHidden;
          break;

        case Excel2007Serializator.StateVisible:
          sheet.Visibility = WorksheetVisibility.Visible;
          break;

        default:
          throw new ArgumentException( "Unknown visibility state type" );
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="dictionary"></param>
    /// <param name="keyAttribute"></param>
    /// <param name="valueAttribute"></param>
    private void ParseDictionaryEntry( XmlReader reader, IDictionary<string, string> dictionary,
      string keyAttribute, string valueAttribute )
    {
      string strKey = null;
      string strValue = null;

      if( reader.MoveToAttribute( keyAttribute ) )
        strKey = reader.Value;

      if( reader.MoveToAttribute( valueAttribute ) )
        strValue = reader.Value;

      if( strKey == null || strValue == null )
        throw new XmlReadingException( "Unable to parse dictionary entry item from Content type" );

      dictionary.Add( strKey, strValue );
    }
    /// <summary>
    /// Extracts single MergeRegion from reader and inserts it into collection inside worksheet.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="sheet">Worksheet to put data into.</param>
    private void ParseMergeRegion( XmlReader reader, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( reader.LocalName == Excel2007Serializator.MergeCellXmlTagName )
      {
        if( reader.MoveToAttribute( Excel2007Serializator.RefAttributeName ) )
        {
          string strReferenceValue = reader.Value;
          RangeImpl range = sheet.Range[strReferenceValue] as RangeImpl;
          range.MergeWithoutCheck();
          reader.Skip();
        }
        else
        {
          throw new InvalidDataException();// "Unsupported Merged cells format" );
        }
      }
    }
    /// <summary>
    /// Extracts single MergeRegion from reader and inserts it into collection inside worksheet.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <returns>Named range value.</returns>
    private string ParseNamedRange( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      string strName = null;
      string strValue = null;
      bool bLocal = false;
      int iSheetId = -1;
      bool bHidden = false;

      if( reader.LocalName == Excel2007Serializator.DefinedNameXmlTagName )
      {
        if( reader.MoveToAttribute( Excel2007Serializator.NameAttributeName ) )
        {
          strName = reader.Value;
        }
        else
        {
          throw new ApplicationException( "Cannot find name for named range" );
        }

        if( reader.MoveToAttribute( Excel2007Serializator.NameSheetIdAttribute ) )
        {
          bLocal = true;
          iSheetId = int.Parse( reader.Value );
        }

        if( reader.MoveToAttribute( Excel2007Serializator.HiddenAttributeName ) )
        {
          bHidden = XmlConvert.ToBoolean( reader.Value );
        }

        WorksheetImpl sheet = ( bLocal ) ?
          m_book.Objects[ iSheetId ] as WorksheetImpl
          : null;

        bool bAddLocal = bLocal || ( sheet != null );

        IName name;

        if( bAddLocal )
        {
          name = sheet.Names.Add( strName );
        }
        else
        {
          name = m_book.Names.Add( strName );
        }


        reader.Read();
        strValue = reader.Value;

        m_book.HasApostrophe = strValue.Contains("'") ? true : false;

        NameImpl nameImpl = ( NameImpl )name;
        //nameImpl.SetValue( m_formulaUtil.ParseString( strValue ) );
        nameImpl.Visible = !bHidden;

        if( bLocal )
        {
          nameImpl.Record.IndexOrGlobal = ( ushort )( iSheetId + 1 );
        }

        reader.Skip();
      }

      return strValue;
    }
    /// <summary>
    /// Extracts collection of number formats from specified reader.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
   private FormatImpl ParseDxfNumberFormat(XmlReader reader)
    {
        if( reader == null )
        throw new ArgumentNullException( "reader" );

        if( reader.NodeType != XmlNodeType.Element || reader.LocalName != Excel2007Serializator.NumberFormatTagName )
        {
          throw new XmlException( "Unexpected tag " + reader.LocalName );
        }

        int iFormatId = -1;
        string strNumberFormat = null;        

        if (reader.MoveToAttribute(Excel2007Serializator.NumberFormatIdAttributeName))
        {
            iFormatId = Convert.ToInt32(reader.Value);
        }
        else
        {
            throw new XmlException(Excel2007Serializator.NumberFormatIdAttributeName + " wasn't found");
        }

        if (reader.MoveToAttribute(Excel2007Serializator.NumberFormatStringAttributeName))
        {
            strNumberFormat = reader.Value;
        }
        else
        {
            throw new XmlException(Excel2007Serializator.NumberFormatStringAttributeName + " wasn't found");
        }

        m_book.InnerFormats.Add(iFormatId, strNumberFormat);
        return m_book.InnerFormats[iFormatId];
    }
    /// <summary>
    /// Extracts fonts from XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <returns>List with new font indexes.</returns>
    private List<int> ParseFonts( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      List<int> result = new List<int>();

      if( reader.NodeType == XmlNodeType.Element )
      {
        if( reader.LocalName == Excel2007Serializator.FontsTagName )
        {
          reader.Read();

          while( reader.NodeType != XmlNodeType.EndElement )
          {
            if( reader.NodeType == XmlNodeType.Element )
              ParseFont( reader, result );

            reader.Read();
          }
        }
      }

      return result;
    }
    /// <summary>
    /// Extracts font object from specified XmlReader.
    /// </summary>
    /// <param name="reader">Reader to read font data from.</param>
    /// <param name="fontIndexes">List to add new font index into.</param>
    private int ParseFont( XmlReader reader, List<int> fontIndexes )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );
      m_book.AppImplementation.CheckDefaultFont(m_book);
      FontImpl font = ( FontImpl )m_book.CreateFont( null, false );

      if (!reader.IsEmptyElement)
      {
          reader.Read();

          while (reader.NodeType == XmlNodeType.Whitespace)
              reader.Read();

          ParseFontSettings(reader, font);
      }

      font = ( FontImpl )m_book.InnerFonts.Add( font );

      if( fontIndexes != null )
      {
        fontIndexes.Add( font.Index );
      }

      return font.Index;
    }
    /// <summary>
    /// Extracts font object from specified XmlReader.
    /// </summary>
    /// <param name="reader">Reader to read font data from.</param>
    /// <returns>Extracted font.</returns>
    private FontImpl ParseFont( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      FontImpl font = new FontImpl( m_book.Application, m_book );

     if (!reader.IsEmptyElement)
      {
          reader.Read();
          ParseFontSettings(reader, font);
      }

      return font;
    }
    /// <summary>
    /// Extracts font settings from specified XmlReader.
    /// </summary>
    /// <param name="reader">Reader to read font data from.</param>
    /// <param name="font">Font to extract data into.</param>
    private void ParseFontSettings(XmlReader reader, FontImpl font)
    {
        while (reader.NodeType != XmlNodeType.EndElement || (reader.LocalName != Excel2007Serializator.FontTagName && reader.LocalName != Excel2007Serializator.RichTextRunPropertiesTagName))
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.LocalName)
                {
                    case Excel2007Serializator.FontBoldTagName:
                        font.Bold = ParseBoolean(reader, Excel2007Serializator.ValueAttributeName, true);
                        break;

                    case Excel2007Serializator.FontItalicTagName:
                        font.Italic = ParseBoolean(reader, Excel2007Serializator.ValueAttributeName, true);
                        break;

                    case Excel2007Serializator.FontNameTagName:
                    case Excel2007Serializator.RichTextRunFontTagName:
                        font.FontName = ParseValue(reader, Excel2007Serializator.ValueAttributeName);
                        break;

                    case Excel2007Serializator.FontSizeTagName:
                        string strSize = ParseValue(reader, Excel2007Serializator.ValueAttributeName);
                        font.Size = double.Parse(strSize, CultureInfo.InvariantCulture);
                        break;

                    case Excel2007Serializator.FontStrikeTagName:
                        font.Strikethrough = ParseBoolean(reader, Excel2007Serializator.ValueAttributeName, true);
                        break;

                    case Excel2007Serializator.FontUnderlineTagName:
                        string strUnderline = ParseValue(reader, Excel2007Serializator.ValueAttributeName);

                        font.Underline = (strUnderline != null) ?
                          (ExcelUnderline)Enum.Parse(typeof(ExcelUnderline), strUnderline, true) :
                          font.Underline = ExcelUnderline.Single;
                        break;

                    case Excel2007Serializator.FontVerticalAlignmentTagName:
                        string strAlign = ParseValue(reader, Excel2007Serializator.ValueAttributeName);
                        font.VerticalAlignment = (ExcelFontVertialAlignment)Enum.Parse(
                          typeof(ExcelFontVertialAlignment), strAlign, true);
                        break;

                    case Excel2007Serializator.MacOSShadowTagName:
                        font.MacOSShadow = ParseBoolean(reader, Excel2007Serializator.ValueAttributeName, true);
                        break;

                    case Excel2007Serializator.ColorTagName:
                        font.ColorObject.CopyFrom(ParseColor(reader), true);
                        break;

                    case Excel2007Serializator.FontCharsetTagName:
                        font.CharSet = ParseCharSet(reader);
                        break;

                    case Excel2007Serializator.FontFamilyTagName:
                        font.Family = ParseFamily(reader);
                        break;
                    default:
                        //throw new NotImplementedException( reader.LocalName );

                        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, reader.Name, "Xml tag is not supported" );
                        break;
                }
                reader.Read();
            }
            else
                reader.Skip();
        }
    }

    /// <summary>
    /// Parses the family.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns></returns>
    private byte ParseFamily(XmlReader reader)
    {
        byte iResult = 0;

        if (reader.MoveToAttribute(Excel2007Serializator.ValueAttributeName))
        {            
            iResult = byte.Parse(reader.Value);
        }

        //reader.Read();
        return iResult;
    }
    /// <summary>
    /// Extracts
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    private byte ParseCharSet( XmlReader reader )
    {
      byte iResult = 1;

      if( reader.MoveToAttribute( Excel2007Serializator.ValueAttributeName ) )
      {
        iResult = byte.Parse( reader.Value );
      }

      //reader.Read();
      return iResult;
    }
    /// <summary>
    /// Extracts collection of number formats from specified reader.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    private void ParseNumberFormats( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.NodeType != XmlNodeType.Element ||
        reader.LocalName != Excel2007Serializator.NumberFormatsTagName )
      {
        throw new XmlException( "Unexpected tag " + reader.LocalName );
      }

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement || reader.LocalName!=Excel2007Serializator.NumberFormatsTagName)
        {
          if( reader.NodeType == XmlNodeType.Element )
            ParseNumberFormat( reader );            
          reader.Read();
        }
      }
    }
    /// <summary>
    /// Extracts single number format entry from specified reader.
    /// </summary>
    /// <param name="reader">Reader to extract from.</param>
    private void ParseNumberFormat( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.NodeType != XmlNodeType.Element ||
        reader.LocalName != Excel2007Serializator.NumberFormatTagName )
      {
        throw new XmlException( "Unexpected tag " + reader.LocalName );
      }

      int iFormatId = -1;
      string strNumberFormat = null;

      if( reader.MoveToAttribute( Excel2007Serializator.NumberFormatIdAttributeName ) )
      {
        iFormatId = Convert.ToInt32( reader.Value );
      }
      else
      {
        throw new XmlException( Excel2007Serializator.NumberFormatIdAttributeName + " wasn't found" );
      }

      if( reader.MoveToAttribute( Excel2007Serializator.NumberFormatStringAttributeName ) )
      {
        strNumberFormat = reader.Value;
      }
      else
      {
        throw new XmlException( Excel2007Serializator.NumberFormatStringAttributeName + " wasn't found" );
      }

      m_book.InnerFormats.Add( iFormatId, strNumberFormat );
      m_book.InnerFormats.HasNumberFormats = true;
    }
    /// <summary>
    /// Extracts color from XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <returns>Extracted color index (on the current moment we support only indexed colors).</returns>
    private ColorObject ParseColor( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      ColorObject color = new ColorObject( ExcelKnownColors.BlackCustom );
      ParseColor( reader, color );
      return color;
    }
    /// <summary>
    /// Extracts color from XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="color">Color object to put extracted values into.</param>
    private void ParseColor( XmlReader reader, ColorObject color )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.MoveToAttribute( Excel2007Serializator.ColorIndexedAttributeName ) )
      {
        ExcelKnownColors index = ( ExcelKnownColors )Convert.ToInt32( reader.Value );
        color.SetIndexed( index,true,m_book);
      }
      else
      {
        // TODO: add tint/shade support
        Color rgb = ColorExtension.Empty;
        double dTintValue = 0;

        if( reader.MoveToAttribute( Excel2007Serializator.ColorTintAttributeName ) )
        {
          dTintValue = XmlConvert.ToDouble( reader.Value );
          //rgb = ConvertColorByTint( rgb, dTintValue );
        }

        if( reader.MoveToAttribute( Excel2007Serializator.ColorRgbAttribute ) )
        {
          rgb = ColorExtension.FromArgb( int.Parse( reader.Value, NumberStyles.HexNumber ) );
          color.SetRGB( rgb, m_book, dTintValue );
          color.ColorType = ColorType.RGB;
        }
        else if( reader.MoveToAttribute( Excel2007Serializator.ColorThemeAttributeName ) )
        {
          //if( m_lstThemeColors == null || m_lstThemeColors.Count == 0 )
          //throw new ArgumentNullException( "Theme colors were not parsed" );

          //rgb = m_lstThemeColors[ Convert.ToInt32( reader.Value ) ];
          int iThemeIndex = Convert.ToInt32( reader.Value );
          color.SetTheme( iThemeIndex, m_book, dTintValue );
        }
      }
    }
    /// <summary>
    /// Applies tint to the specified color.
    /// </summary>
    /// <param name="color">Color to apply tint value to.</param>
    /// <param name="dTint">Tint value to apply.</param>
    /// <returns>Color after applying tint value.</returns>
    public static Color ConvertColorByTint( Color color, double dTint )
    {
      double dHue;
      double dLuminance;
      double dSaturation;

      ConvertRGBtoHLS( color, out dHue, out dLuminance, out dSaturation );

      // Here we have to change luminance value according to tint value.
      // If tint < 0, then luminance = luminance * ( 1.0 + tint ),
      // if tint > 0, then luminance = luminance * ( 1.0 - tint ) + ( 255 � 255 * ( 1.0 - tint ) )

      if( dTint < 0 )
        dLuminance = dLuminance * ( 1.0 + dTint );

      if( dTint > 0 )
        dLuminance = dLuminance * ( 1.0 - dTint ) + ( HLSMax - HLSMax * ( 1.0 - dTint ) );

      Color color1 = ConvertHLSToRGB( dHue, dLuminance, dSaturation );
      return ConvertHLSToRGB( dHue, dLuminance, dSaturation );
    }
    /// <summary>
    /// Converts color to HLS values.
    /// </summary>
    /// <param name="color">Color to convert.</param>
    /// <param name="dHue">Hue value.</param>
    /// <param name="dLuminance">Luminance value.</param>
    /// <param name="dSaturation">Saturation value.</param>
    public static void ConvertRGBtoHLS( Color color, out double dHue, out double dLuminance, out double dSaturation )
    {
      dHue = 0;
      dLuminance = 0;
      dSaturation = 0;

      byte bRed = color.R;
      byte bGreen = color.G;
      byte bBlue = color.B;

      byte bMinVal = Math.Min( bRed, Math.Min( bGreen, bBlue ) );
      byte bMaxVal = Math.Max( bRed, Math.Max( bGreen, bBlue ) );

      double fMinMaxDiff = ( double )( bMaxVal - bMinVal );
      double fMinMaxSum = ( double )( bMaxVal + bMinVal );
      
      dLuminance = ( ( fMinMaxSum * HLSMax ) + RGBMax ) / ( 2 * RGBMax );

      if( bMaxVal == bMinVal )
      {
        dSaturation = 0;
        dHue = Undefined;
      }
      else
      {
        if( dLuminance <= ( HLSMax / 2 ) )
        {
          dSaturation = ( ( fMinMaxDiff * HLSMax ) +  ( fMinMaxSum / 2 ) ) / fMinMaxSum;
        }
        else
        {
          dSaturation = ( ( fMinMaxDiff * HLSMax ) + ( ( 2 * RGBMax - fMinMaxSum ) / 2 ) ) /
            ( 2 * RGBMax - fMinMaxSum );
        }

        double dRedDelta = ( ( ( bMaxVal - bRed ) * ( HLSMax / 6 ) ) + fMinMaxDiff / 2 ) / fMinMaxDiff;
        double dGreenDelta = ( ( ( bMaxVal - bGreen ) * ( HLSMax / 6 ) ) + fMinMaxDiff / 2 ) / fMinMaxDiff;
        double dBlueDelta = ( ( ( bMaxVal - bBlue ) * ( HLSMax / 6 ) ) + fMinMaxDiff / 2 ) / fMinMaxDiff;

        if( bRed == bMaxVal )
        {
          dHue = dBlueDelta - dGreenDelta;
        }
        else if( bGreen == bMaxVal )
        {
          dHue = ( HLSMax / 3 ) + dRedDelta - dBlueDelta;
        }
        else
        {
          dHue = ( ( 2 * HLSMax ) / 3 ) + dGreenDelta - dRedDelta;
        }

        if( dHue < 0 )
          dHue += HLSMax;

        if( dHue > HLSMax )
          dHue -= HLSMax;
      }

      if( dSaturation < 0 )
        dSaturation = 0;

      if( dSaturation > HLSMax )
        dSaturation = HLSMax;

      if( dLuminance < 0 )
        dLuminance = 0;

      if( dLuminance > HLSMax )
        dLuminance = HLSMax;
    }
    /// <summary>
    /// Converts HLS components to RGB color values and returns Color, according to this values.
    /// </summary>
    /// <param name="dHue">Hue value.</param>
    /// <param name="dLuminance">Luminance value.</param>
    /// <param name="dSaturation">Saturation value.</param>
    /// <returns></returns>
    public static Color ConvertHLSToRGB( double dHue, double dLuminance, double dSaturation )
    {
      int iRed = 0;
      int iGreen = 0;
      int iBlue = 0;

      if( dSaturation == 0 )
      {
        iBlue = ( int )( ( dLuminance * RGBMax ) / HLSMax );
        iRed = iBlue;
        iGreen = iBlue;
      }
      else
      {
        double dMagic1;
        double dMagic2;

        if( dLuminance <= ( HLSMax / 2 ) )
        {
          dMagic2 = ( dLuminance * ( HLSMax + dSaturation ) + ( HLSMax / 2 ) ) / HLSMax;
        }
        else
        {
          dMagic2 = dLuminance + dSaturation - ( ( dLuminance * dSaturation ) + ( HLSMax / 2 ) ) / HLSMax;
        }

        dMagic1 = 2 * dLuminance - dMagic2;
        iRed = ( int )( ( HueToRGB( dMagic1, dMagic2, dHue + ( HLSMax / 3 ) ) * RGBMax + ( HLSMax / 2 ) ) / HLSMax );
        iGreen = ( int )( ( HueToRGB( dMagic1, dMagic2, dHue ) * RGBMax + ( HLSMax / 2 ) ) / HLSMax );
        iBlue = ( int )( ( HueToRGB( dMagic1, dMagic2, dHue - ( HLSMax / 3 ) ) * RGBMax + ( HLSMax / 2 ) ) / HLSMax );
      }

      if( iRed < 0 )
        iRed = 0;

      if( iGreen < 0 )
        iGreen = 0;

      if( iBlue < 0 )
        iBlue = 0;

      if( iGreen > RGBMax )
        iRed = RGBMax;

      if( iGreen > RGBMax )
        iGreen = RGBMax;

      if( iBlue > RGBMax )
        iBlue = RGBMax;

      return Color.FromArgb( 0, ( byte )iRed, ( byte )iGreen, ( byte )iBlue );
    }
    /// <summary>
    /// Converts Hue value to RGB single component.
    /// </summary>
    /// <param name="dN1">Magic number 1.</param>
    /// <param name="dN2">Magic number 2.</param>
    /// <param name="dHue">Hue value.</param>
    /// <returns>RGB component value.</returns>
    public static double HueToRGB( double dN1, double dN2, double dHue )
    {
      double dResult;

      if( dHue < 0 )
        dHue += HLSMax;

      if( dHue > HLSMax )
        dHue -= HLSMax;

      if( dHue < ( HLSMax / 6 ) )
      {
        dResult = dN1 + ( ( dN2 - dN1 ) * dHue + ( HLSMax / 12 ) ) / ( HLSMax / 6 );
      }
      else
      {
        if( dHue < ( HLSMax / 2 ) )
        {
          dResult = dN2;
        }
        else if( dHue < ( ( HLSMax * 2 ) / 3 ) )
        {
          dResult = dN1 + ( ( ( dN2 - dN1 ) * ( ( ( HLSMax * 2 ) / 3 ) - dHue ) + ( HLSMax / 12 ) ) / ( HLSMax / 6 ) );
        }
        else
        {
          dResult = dN1;
        }
      }

      return dResult;
    }
    /// <summary>
    /// Extracts boolean value from the current xml tag.
    /// </summary>
    /// <param name="reader">XmlReader to get value from.</param>
    /// <param name="valueAttribute">Name of the attribute where value is stored.</param>
    /// <param name="defaultValue">Default value (when there is no attribute specified).</param>
    /// <returns>Extracted value.</returns>
    private bool ParseBoolean( XmlReader reader, string valueAttribute, bool defaultValue )
    {
      bool bResult = defaultValue;

      if( reader.MoveToAttribute( valueAttribute ) )
      {
        //bResult = bool.Parse( reader.Value );
        bResult = XmlConvert.ToBoolean( reader.Value );
      }

      return bResult;
    }
    /// <summary>
    /// Extract value of the attribute from reader.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="valueAttribute">Name of the attribute to extract.</param>
    /// <returns>Extracted value; or null if there were no attribute with such name.</returns>
    private string ParseValue( XmlReader reader, string valueAttribute )
    {
      string strResult = null;

      if( reader.MoveToAttribute( valueAttribute ) )
      {
        strResult = reader.Value;
      }

      return strResult;
    }
    /// <summary>
    /// Extracts fill objects from XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <returns>List with all extracted fills.</returns>
    private List<FillImpl> ParseFills( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      List<FillImpl> result = new List<FillImpl>();

      if( reader.NodeType == XmlNodeType.Element )
      {
        if( reader.LocalName == Excel2007Serializator.FillsTagName )
        {
          reader.Read();
          if (reader.NodeType == XmlNodeType.Whitespace)
              reader.Read();
          while( reader.NodeType != XmlNodeType.EndElement )
          {
            if( reader.NodeType == XmlNodeType.Element )
            {
              FillImpl fill = ParseFill( reader, true );
              result.Add( fill );
            }

            reader.Read();
          }
        }
      }

      return result;
    }
    /// <summary>
    /// Extracts single fill object from specified XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to read fill data from.</param>
    /// <param name="swapColors">Indicates whether fore and back colors should be swapped.</param>
    /// <returns>Extracted Fill object.</returns>
    private FillImpl ParseFill( XmlReader reader, bool swapColors )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.LocalName != Excel2007Serializator.FillTagName )
        throw new XmlException( "Wrong tag name " + reader.LocalName );

      reader.Read();
      if (reader.NodeType == XmlNodeType.Whitespace)
          reader.Read();
      FillImpl result = null;

      switch( reader.LocalName )
      {
        case Excel2007Serializator.PatternFillTagName:
          result = ParsePatternFill( reader, swapColors );
          break;

        case Excel2007Serializator.GradientFillTagName:
          result = ParseGradientFill( reader );
          break;

        default:
          throw new ArgumentException( "Unexpected tag  " + reader.LocalName );
      }
      if (reader.NodeType == XmlNodeType.Whitespace)
          reader.Read();
      return result;
    }
    /// <summary>
    /// Extracts single gradient fill object from specified XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to read gradient fill from.</param>
    /// <returns>Extracted gradient fill object.</returns>
    private FillImpl ParseGradientFill( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.LocalName != Excel2007Serializator.GradientFillTagName )
        throw new XmlException( "Unexpected tag " + reader.LocalName );

      FillImpl result = null;

      if( reader.MoveToAttribute( Excel2007Serializator.GradientFillTypeAttributeName )
        && reader.Value == Excel2007Serializator.GradientFillTypePath )
      {
        result = ParsePathGradientType( reader );
      }
      else
      {
        result = ParseLinearGradientType( reader );
      }

      reader.Skip();

      return result;
    }
    /// <summary>
    /// Extracts single path gradient fill object from specified XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to read gradient fill from.</param>
    /// <returns>Extracted path gradient fill object.</returns>
    private FillImpl ParsePathGradientType( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      FillImpl result = new FillImpl();
      result.FillType = ExcelFillType.Gradient;

      double dTop = ParseAttributeValue( reader, Excel2007Serializator.TopConvergenceAttributeName );
      double dBottom = ParseAttributeValue( reader, Excel2007Serializator.BottomConvergenceAttributeName );
      double dLeft = ParseAttributeValue( reader, Excel2007Serializator.LeftConvergenceAttributeName );
      double dRight = ParseAttributeValue( reader, Excel2007Serializator.RightConvergenceAttributeName );

      if( dTop == 0.5 && dBottom == 0.5 && dLeft == 0.5 && dRight == 0.5 )
      {
        result.GradientStyle = ExcelGradientStyle.From_Center;
        result.GradientVariant = ExcelGradientVariants.ShadingVariants_1;
      }
      else if( dTop == 1 && dBottom == 1 && dLeft == 1 && dRight == 1 )
      {
        result.GradientStyle = ExcelGradientStyle.From_Corner;
        result.GradientVariant = ExcelGradientVariants.ShadingVariants_4;
      }
      else if( dTop == 1 && dBottom == 1 )
      {
        result.GradientStyle = ExcelGradientStyle.From_Corner;
        result.GradientVariant = ExcelGradientVariants.ShadingVariants_3;
      }
      else if( dLeft == 1 && dRight == 1 )
      {
        result.GradientStyle = ExcelGradientStyle.From_Corner;
        result.GradientVariant = ExcelGradientVariants.ShadingVariants_2;
      }
      else if( double.IsNaN( dTop ) && double.IsNaN( dBottom ) && double.IsNaN( dLeft ) && double.IsNaN( dRight ) )
      {
        result.GradientStyle = ExcelGradientStyle.From_Corner;
        result.GradientVariant = ExcelGradientVariants.ShadingVariants_1;
      }

      reader.Read();

      List<ColorObject> arrColors = ParseStopColors( reader );
      result.PatternColorObject.CopyFrom( arrColors[ 0 ], true );
      result.ColorObject.CopyFrom( arrColors[ 1 ], true );
      return result;
    }
    /// <summary>
    /// Extracts stop colors from specified XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to read stop colors from.</param>
    /// <returns>Colors list.</returns>
    private List<ColorObject> ParseStopColors( XmlReader reader )
    {
      List<ColorObject> arrColors = new List<ColorObject>();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.LocalName == Excel2007Serializator.GradientStopTagName )
        {
          reader.Read();
          arrColors.Add( ParseColor( reader ) );
          reader.Skip();
        }

        reader.Skip();
      }

      return arrColors;
    }
    /// <summary>
    /// Extracts single linear gradient fill object from specified XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to read linear gradient fill from.</param>
    /// <returns>Extracted linear gradient fill object.</returns>
    private FillImpl ParseLinearGradientType( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      FillImpl result = new FillImpl();
      result.FillType = ExcelFillType.Gradient;

      double dDegree = ParseAttributeValue( reader, Excel2007Serializator.LinearGradientDegreeAttributeName );

      reader.Read();

      List<ColorObject> arrColors = ParseStopColors( reader );
      result.PatternColorObject.CopyFrom( arrColors[ 0 ], true );
      result.ColorObject.CopyFrom( arrColors[ 1 ], true );

      if( arrColors.Count == 3 )
      {
        result.GradientVariant = ExcelGradientVariants.ShadingVariants_3;
        int iDegree = ( double.IsNaN( dDegree ) ) ? 0 : ( int )dDegree;

        switch( iDegree )
        {
          case 90:
            result.GradientStyle = ExcelGradientStyle.Horizontal;
            break;

          case 0:
            result.GradientStyle = ExcelGradientStyle.Vertical;
            break;

          case 45:
            result.GradientStyle = ExcelGradientStyle.Diagonl_Up;
            break;

          case 135:
            result.GradientStyle = ExcelGradientStyle.Diagonl_Down;
            break;

          default:
            throw new ArgumentException( "Unsupported degree value" );
        }
      }
      else
      {
        SetGradientStyleVariant( result, dDegree );
      }

      return result;
    }
    /// <summary>
    /// Sets gradients fill style and variant values according to degree value.
    /// </summary>
    /// <param name="fill">Fill to set data into.</param>
    /// <param name="dDegree">Degree value.</param>
    private void SetGradientStyleVariant( FillImpl fill, double dDegree )
    {
      int iDegree = ( double.IsNaN( dDegree ) ) ? 0 : ( int )dDegree;

      switch( iDegree )
      {
        case 90:
          fill.GradientStyle = ExcelGradientStyle.Horizontal;
          fill.GradientVariant = ExcelGradientVariants.ShadingVariants_1;
          break;

        case 270:
          fill.GradientStyle = ExcelGradientStyle.Horizontal;
          fill.GradientVariant = ExcelGradientVariants.ShadingVariants_2;
          break;

        case 0:
          fill.GradientStyle = ExcelGradientStyle.Vertical;
          fill.GradientVariant = ExcelGradientVariants.ShadingVariants_1;
          break;

        case 180:
          fill.GradientStyle = ExcelGradientStyle.Vertical;
          fill.GradientVariant = ExcelGradientVariants.ShadingVariants_2;
          break;

        case 45:
          fill.GradientStyle = ExcelGradientStyle.Diagonl_Up;
          fill.GradientVariant = ExcelGradientVariants.ShadingVariants_1;
          break;

        case 225:
          fill.GradientStyle = ExcelGradientStyle.Diagonl_Up;
          fill.GradientVariant = ExcelGradientVariants.ShadingVariants_2;
          break;

        case 135:
          fill.GradientStyle = ExcelGradientStyle.Diagonl_Down;
          fill.GradientVariant = ExcelGradientVariants.ShadingVariants_1;
          break;

        case 315:
          fill.GradientStyle = ExcelGradientStyle.Diagonl_Down;
          fill.GradientVariant = ExcelGradientVariants.ShadingVariants_2;
          break;

        default:
          throw new ArgumentException( "Unsupported degree value" );
      }
    }
    /// <summary>
    /// Extracts attribute value.
    /// </summary>
    /// <param name="reader">XmlReader to extract attribute value from.</param>
    /// <param name="strAttributeName">Attribute name.</param>
    /// <returns>Attribute double value.</returns>
    private double ParseAttributeValue( XmlReader reader, string strAttributeName )
    {
      return reader.MoveToAttribute( strAttributeName ) ? XmlConvert.ToDouble( reader.Value ) : double.NaN;
    }

    /// <summary>
    /// Extracts pattern fill from specified XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="swapColors">Indicates whether fore and back colors should be swapped.</param>
    /// <returns>Extracted fill object.</returns>
    private FillImpl ParsePatternFill( XmlReader reader, bool swapColors )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.LocalName != Excel2007Serializator.PatternFillTagName )
        throw new XmlException( "Unexpected tag " + reader.LocalName );

      FillImpl result = new FillImpl();

      if( reader.MoveToAttribute( Excel2007Serializator.PatternAttributeName ) )
        result.Pattern = ConvertStringToPattern( reader.Value );
      if (reader.NodeType == XmlNodeType.Whitespace)
          reader.Read();
      reader.Read();
      

      ColorObject backColor = null;
      ColorObject foreColor = null;

      while (reader.NodeType != XmlNodeType.EndElement || reader.LocalName != Excel2007Serializator.FillTagName)
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case Excel2007Serializator.BackgroundColorTagName:
              backColor = new ColorObject( ( ExcelKnownColors )ExtendedFormatRecord.DEF_DEFAULT_PATTERN_COLOR_INDEX );
              ParseColor( reader, backColor );
              break;

            case Excel2007Serializator.ForegroundColorTagName:
              foreColor = new ColorObject( ( ExcelKnownColors )ExtendedFormatRecord.DEF_DEFAULT_COLOR_INDEX );
              ParseColor( reader, foreColor );
              break;
          }
        }

        reader.Read();
      }

      if( reader.LocalName == Excel2007Serializator.PatternFillTagName )
      {
        reader.Read();
      }

      if( swapColors && result.Pattern != ExcelPattern.Solid )
      {
        ColorObject tempColor = foreColor;
        foreColor = backColor;
        backColor = tempColor;
      }


      if( foreColor != null )
      {
        result.ColorObject.CopyFrom( foreColor, true );
      }
      else if ((backColor == null) || result.Pattern != ExcelPattern.Solid)
      {
        result.ColorObject.SetIndexed((ExcelKnownColors)ExtendedFormatRecord.DEF_DEFAULT_COLOR_INDEX);
      }

      if( backColor != null )
      {
        result.PatternColorObject.CopyFrom( backColor, true );
      }
      else if ((foreColor == null) || result.Pattern != ExcelPattern.Solid)
      {
        result.PatternColorObject.SetIndexed((ExcelKnownColors)ExtendedFormatRecord.DEF_DEFAULT_PATTERN_COLOR_INDEX);
      }

      return result;
    }
    /// <summary>
    /// Converts specified string in MS Excel 2007 pattern format into pattern value.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <returns>Converted value.</returns>
    private static ExcelPattern ConvertStringToPattern( string value )
    {
      Excel2007Pattern pattern = ( Excel2007Pattern )Enum.Parse( typeof( Excel2007Pattern ), value, true );
      return ( ExcelPattern )pattern;
    }
    /// <summary>
    /// Extracts border objects from XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <returns>List with new borders indexes.</returns>
    private List<BordersCollection> ParseBorders( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      List<BordersCollection> result = new List<BordersCollection>();

      //throw new NotImplementedException();
      if( reader.NodeType != XmlNodeType.Element ||
        reader.LocalName != Excel2007Serializator.BordersTagName )
      {
        throw new XmlException( "Unexpected xml tag " + reader.LocalName );
      }

      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          if( reader.LocalName == Excel2007Serializator.BordersCollectionTagName )
          {
            BordersCollection borders = ParseBordersCollection( reader );
            result.Add( borders );
          }
          else
          {
            throw new XmlException( "Unexpected xml tag " + reader.LocalName );
          }
        }
        else
        {
          reader.Read();
        }
      }

        if (!result[0].IsEmptyBorder)
          result.RemoveAt(0);
      
      return result;
    }
    /// <summary>
    /// Extracts border collection from specified XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <returns>Extracted borders collection.</returns>
    private BordersCollection ParseBordersCollection( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.NodeType != XmlNodeType.Element ||
        reader.LocalName != Excel2007Serializator.BordersCollectionTagName )
      {
        throw new XmlException( "Unexpected xml tag " + reader.LocalName );
      }

      BordersCollection borders = new BordersCollection( m_book.Application, m_book, true );

      bool bDiagonalUp = false;
      bool bDiagonalDown = false;

      if( reader.MoveToAttribute( Excel2007Serializator.DiagonalUpAttributeName ) )
        bDiagonalUp = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( Excel2007Serializator.DiagonalDownAttributeName ) )
        bDiagonalDown = XmlConvert.ToBoolean( reader.Value );

      if( reader.IsEmptyElement )
      {
        reader.Read();
      }
      else
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            Excel2007BorderIndex borderIndex;
            BorderSettingsHolder border = ParseBorder( reader, out borderIndex );

            if( borderIndex == Excel2007BorderIndex.diagonal )
            {
              BorderSettingsHolder border2 = ( BorderSettingsHolder )border.Clone();

              border.ShowDiagonalLine = bDiagonalUp;
              border2.ShowDiagonalLine = bDiagonalDown;
              borders.SetBorder( ExcelBordersIndex.DiagonalUp, border );
              borders.SetBorder( ExcelBordersIndex.DiagonalDown, border2 );
            }
            else
            {
              ExcelBordersIndex oldIndex = ( ExcelBordersIndex )borderIndex;
              borders.SetBorder( oldIndex, border );
            }
            if (borders.IsEmptyBorder && border != null && !border.IsEmptyBorder)
                borders.IsEmptyBorder = false;
          }

          reader.Read();
        }

        reader.Read();
      }

      return borders;
    }
    /// <summary>
    /// Extracts single border from the XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="borderIndex">Output - index of the extracted border (left, top, etc.).</param>
    /// <returns>Extracted border.</returns>
    private BorderSettingsHolder ParseBorder( XmlReader reader, out Excel2007BorderIndex borderIndex )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.NodeType != XmlNodeType.Element )
        throw new XmlException( "Unexpected node type " + reader.NodeType );

      borderIndex = ( Excel2007BorderIndex )Enum.Parse( typeof( Excel2007BorderIndex ),
        reader.LocalName, true );

      BorderSettingsHolder result = new BorderSettingsHolder();

      bool emptyElement = false;
      if (reader.IsEmptyElement) emptyElement = true;

      if( reader.MoveToAttribute( Excel2007Serializator.BorderStyleAttributeName ) )
      {
        Excel2007BorderLineStyle lineStyle = ( Excel2007BorderLineStyle )Enum.Parse(
          typeof( Excel2007BorderLineStyle ), reader.Value, true );

        result.LineStyle = ( ExcelLineStyle )lineStyle;
        result.IsEmptyBorder = false;
      }

      if (!emptyElement && !reader.IsEmptyElement)
      {
        reader.Read();

        while (reader.NodeType != XmlNodeType.EndElement || reader.LocalName == Excel2007Serializator.ColorTagName)
        {
          if( reader.NodeType == XmlNodeType.Element && reader.LocalName == Excel2007Serializator.ColorTagName )
          {
            ParseColor( reader, result.ColorObject );
          }
          reader.Read();
        }
      }

      return result;
    }
    /// <summary>
    /// Extracts row data from reader and inserts it into worksheet.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="sheet">Worksheet to put data into.</param>
    /// <param name="arrStyles">Style hashtable. Key - recovered from xml extended format id,
    /// value - extended format index in workbook collection.</param>
    /// <returns>Current row id.</returns>
    private int ParseRow(XmlReader reader, IInternalWorksheet sheet, List<int> arrStyles, string cellTag, int generatedRowIndex)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");

        if (sheet == null)
            throw new ArgumentNullException("sheet");

        if (reader.LocalName != Excel2007Serializator.RowTagName)
            throw new XmlException("reader");

        //bool bHidden = false;
        //bool bCustomFormat = false;
        //bool bCustomHeight = false;
        //bool bCollapsed = false;
        //bool bThickBottom = false;
        //bool bThickTop = false;
        //ushort usOutlineLevel = 0;
        int iRowIndex = 0;
        int iStyleIndex = m_book.DefaultXFIndex;
        int iRowHeight = sheet.DefaultRowHeight;
        bool isFormatted = false;
        bool isWrapText = false;
        if (reader.MoveToAttribute(Excel2007Serializator.RowIndexAttributeName))
        {
            generatedRowIndex = iRowIndex = XmlConvert.ToInt32(reader.Value);
        }
        else
        {
            iRowIndex = generatedRowIndex;
        }

        RowStorage rowStorage = WorksheetHelper.GetOrCreateRow(sheet, iRowIndex - 1, true);
        List<int> cellstyleIndex = new List<int>();
        if (reader.MoveToAttribute(Excel2007Serializator.RowColumnCollapsedAttribute))
            rowStorage.IsCollapsed = XmlConvert.ToBoolean(reader.Value);

        if (reader.MoveToAttribute(Excel2007Serializator.RowCustomFormatAttributeName))
            isFormatted = XmlConvert.ToBoolean(reader.Value);
        else
            isFormatted = false;

        if (reader.MoveToAttribute(Excel2007Serializator.RowCustomHeightAttributeName))
            rowStorage.IsBadFontHeight = XmlConvert.ToBoolean(reader.Value);

        if (reader.MoveToAttribute(Excel2007Serializator.RowColumnOutlineLevelAttribute))
        {
            rowStorage.OutlineLevel = XmlConvert.ToUInt16(reader.Value);
            m_indexAndLevels.Add(generatedRowIndex, rowStorage.OutlineLevel);
        }

        if (reader.MoveToAttribute(Excel2007Serializator.RowHeightAttributeName))
        {
            double dRowHeight = XmlConvert.ToDouble(reader.Value);

            if (dRowHeight > RowRecord.DEF_MAX_HEIGHT)
                dRowHeight = RowRecord.DEF_MAX_HEIGHT;

            iRowHeight = (int)(dRowHeight * WorkbookXmlSerializator.DEF_ROW_DIV);
            rowStorage.HasRowHeight = true;
        }

      if (reader.MoveToAttribute(Excel2007Serializator.StyleIndexAttributeName))
      {
          if (arrStyles.Count > XmlConvert.ToInt32(reader.Value))
              iStyleIndex = arrStyles[XmlConvert.ToInt32(reader.Value)];
      }

      if (reader.MoveToAttribute(Excel2007Serializator.RowHiddenAttributeName))
      {
          rowStorage.IsHidden = XmlConvert.ToBoolean(reader.Value);
          rowStorage.IsCollapsed = XmlConvert.ToBoolean(reader.Value);
      }
      else
      {
          rowStorage.IsCollapsed = false;
      }

      if( reader.MoveToAttribute( Excel2007Serializator.RowThickBottomAttributeName ) )
        rowStorage.IsSpaceBelowRow = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( Excel2007Serializator.RowThickTopAttributeName ) )
        rowStorage.IsSpaceAboveRow = XmlConvert.ToBoolean( reader.Value );

      rowStorage.ExtendedFormatIndex = ( ushort )iStyleIndex;
      rowStorage.Height = ( ushort )iRowHeight;
      //rowStorage.IsHidden = bHidden;
      //rowStorage.OutlineLevel = usOutlineLevel;
      //rowStorage.IsCollapsed = bCollapsed;
      //rowStorage.IsFormatted = bCustomFormat;
      //rowStorage.IsBadFontHeight = bCustomHeight;
      //rowStorage.IsSpaceAboveRow = bThickTop;
      //rowStorage.IsSpaceBelowRow = bThickBottom;

      if( sheet.FirstRow < 0 || sheet.FirstRow > iRowIndex ) sheet.FirstRow = iRowIndex;

      if( sheet.LastRow < iRowIndex ) sheet.LastRow = iRowIndex;

      reader.MoveToElement();
      int generatedColumnIndex = 1;
      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.LocalName == cellTag )
          {
            generatedColumnIndex = ParseCell(reader, sheet, arrStyles, iRowIndex, generatedColumnIndex, cellstyleIndex);
            generatedColumnIndex++;
          }

              reader.Skip();
          }
      }
      else if (rowStorage.IsHidden)
      {
          Excel2007Serializator.CellType cellType = Excel2007Serializator.CellType.e;
          CellRecordCollection cells = sheet.CellRecords;
          cells.SetBlank(iRowIndex, generatedColumnIndex,m_book.DefaultXFIndex);          
      }
     foreach (int cellindex in cellstyleIndex)
       {
           if (!isWrapText)
               isWrapText = m_book.GetExtFormat(cellindex).WrapText;
           else
               break;
       }
 
       rowStorage.IsFormatted = isFormatted;
       rowStorage.IsWrapText = isWrapText;
      return generatedRowIndex;
    }
    /// <summary>
    /// Extracts cell from reader and inserts it into worksheet.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="sheet">Worksheet to put data into.</param>
    /// <param name="arrStyles">Style hashtable. Key - recovered from xml extended format id,
    /// value - extended format index in workbook collection.</param>
    private int ParseCell( XmlReader reader, IInternalWorksheet sheet, List<int> arrStyles, int rowIndex, int columnIndex,List<int> cellStyleIndex )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      bool bSetCell = false;
      int iRowIndex = rowIndex;
      int iColumnIndex = columnIndex;
      int iStyleIndex = m_book.DefaultXFIndex;
      Excel2007Serializator.CellType cellType = Excel2007Serializator.CellType.n;
      CellRecordCollection cells = sheet.CellRecords;

      if( reader.MoveToAttribute( Excel2007Serializator.ReferenceAttributeName ) )
        RangeImpl.CellNameToRowColumn( reader.Value, out iRowIndex, out iColumnIndex );

        if (reader.MoveToAttribute(Excel2007Serializator.StyleIndexAttributeName))
       {
           iStyleIndex = arrStyles[XmlConvert.ToInt32(reader.Value)];
 
           if(!cellStyleIndex.Contains(iStyleIndex))
           cellStyleIndex.Add(iStyleIndex);
       }
 
      if( reader.MoveToAttribute( Excel2007Serializator.CellDataTypeAttributeName ) )
        cellType = GetCellType( reader.Value );

      reader.MoveToElement();

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.LocalName == Excel2007Serializator.RichTextInlineTagName )
          {
            string text = string.Empty;
            int result = ParseStringItem( reader ,out text);
            SSTDictionary sst = m_book.InnerSST;
            //sst.UpdateRefCounts();
            cells.SetSingleStringValue( iRowIndex, iColumnIndex, iStyleIndex, result);
            
            if (text != null)
            {
                string cellName = RangeImpl.GetCellName(iColumnIndex, iRowIndex);
                this.Worksheet.InlineStrings.Add(cellName, text);
                if (!m_book.HasInlineStrings)
                    m_book.HasInlineStrings = true;
                    
            }
            // 1 is set in parsing, 2 is set in SetSingleStringValue
            // Here we have to remove one of them
            sst.RemoveDecrease( result );

            //SetCellRecord( cellType, result, cells, iRowIndex, iColumnIndex, iStyleIndex );
            bSetCell = true;
          }

          if( reader.LocalName == Excel2007Serializator.FormulaTagName )
          {
            //Console.WriteLine( "Sheet: {0}, row: {1}, column: {2}", sheet.Name, iRowIndex, iColumnIndex );
            ParseFormula( reader, sheet, iRowIndex, iColumnIndex, iStyleIndex, cellType );
            bSetCell = true;
          }

          if (reader.LocalName == Excel2007Serializator.RichTextInlineTagName)
              reader.Skip();
           
         if(reader.NodeType!=XmlNodeType.EndElement)
         {
          if( reader.LocalName == Excel2007Serializator.CellValueTagName )
          {
            bool bStringEmpty = reader.IsEmptyElement;

            if( !bStringEmpty )
              reader.Read();

            if( WorksheetHelper.HasFormulaRecord( sheet, iRowIndex, iColumnIndex ) )
            {
              SetFormulaValue( sheet, cellType, reader.Value, iRowIndex, iColumnIndex );
            }
            else
            {
              SetCellRecord( cellType, reader.Value, cells, iRowIndex, iColumnIndex, iStyleIndex );
            }

            bSetCell = true;

            if( !bStringEmpty )
              reader.Skip();
          }

          reader.Skip();
        }
      }
      }

      if (!bSetCell)
      {
          if (cells.Sheet is ExternWorksheetImpl)
              SetCellRecord(Excel2007Serializator.CellType.n, "0", cells, iRowIndex, iColumnIndex, iStyleIndex);
          else
              cells.SetBlank(iRowIndex, iColumnIndex, iStyleIndex);
      }

      return iColumnIndex;
    }
    /// <summary>
    /// Returns type of the cell.
    /// </summary>
    /// <param name="cellType">String representation of the cell type.</param>
    /// <returns></returns>
    private static Excel2007Serializator.CellType GetCellType( string cellType )
    {
      Excel2007Serializator.CellType result;

      switch( cellType )
      {
        case "b":
          result = Excel2007Serializator.CellType.b;
          break;

        case "e":
          result = Excel2007Serializator.CellType.e;
          break;

        case "inlineStr":
          result = Excel2007Serializator.CellType.inlineStr;
          break;

        case "n":
          result = Excel2007Serializator.CellType.n;
          break;

        case "s":
          result = Excel2007Serializator.CellType.s;
          break;

        case "str":
          result = Excel2007Serializator.CellType.str;
          break;

        default:
          throw new ArgumentOutOfRangeException();
      }

      return result;
    }
    /// <summary>
    /// Extracts formula from reader and inserts it into worksheet.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="sheet">Worksheet to put data into.</param>
    /// <param name="iRow">Current row index.</param>
    /// <param name="iCol">Current column index.</param>
    /// <param name="iXFIndex">Extended format index.</param>
    private void ParseFormula( XmlReader reader, IInternalWorksheet sheet, int iRow, int iCol, int iXFIndex, Excel2007Serializator.CellType cellType )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( reader.LocalName != Excel2007Serializator.FormulaTagName )
        throw new XmlException( "reader" );

      string strCellsRange = null;
      string strFormulaString = "=";
      uint uiSharedGroupIndex = 0;
      Excel2007Serializator.FormulaType formulaType = Excel2007Serializator.FormulaType.normal;
      CellRecordCollection cells = sheet.CellRecords;
      bool bCalculateOnOpen = false;

      if( reader.MoveToAttribute( Excel2007Serializator.FormulaTypeAttributeName ) )
        formulaType = ( Excel2007Serializator.FormulaType )Enum.Parse(
          typeof( Excel2007Serializator.FormulaType ), reader.Value, false );

      if( reader.MoveToAttribute( Excel2007Serializator.SharedGroupIndexAttributeName ) )
        uiSharedGroupIndex = XmlConvert.ToUInt32( reader.Value );

      if( reader.MoveToAttribute( Excel2007Serializator.RangeOfCellsAttributeName ) )
        strCellsRange = reader.Value;

      if( reader.MoveToAttribute( Excel2007Serializator.CalculateOnOpen ) )
        bCalculateOnOpen = XmlConvert.ToBoolean( reader.Value );

      reader.MoveToElement();

      if( !reader.IsEmptyElement )
      {
        reader.Read();
        if (reader.NodeType != XmlNodeType.EndElement)
        {
            strFormulaString += reader.Value;
            reader.Skip();
        }
      }

      switch( formulaType )
      {
        case Excel2007Serializator.FormulaType.normal:

          strFormulaString = UtilityMethods.RemoveFirstCharUnsafe( strFormulaString );

          if( strFormulaString.Length > 0 )
          {
            FormulaRecord formulaRecord = ( FormulaRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Formula );
            if (!strFormulaString.StartsWith("[") || cellType != Excel2007Serializator.CellType.e)
                formulaRecord.ParsedExpression = m_formulaUtil.ParseString(strFormulaString, sheet, null);
            else
            {
                (sheet as WorksheetImpl).m_formulaString = strFormulaString;
            }
            formulaRecord.Row = iRow - 1;
            formulaRecord.Column = iCol - 1;
            formulaRecord.ExtendedFormatIndex = ( ushort )iXFIndex;
            formulaRecord.CalculateOnOpen = bCalculateOnOpen;
            cells.SetCellRecord( iRow, iCol, formulaRecord );
          }
          break;

        case Excel2007Serializator.FormulaType.array:
          // We don't support formulas for external worksheets on the current moment
          SetArrayFormula( sheet as WorksheetImpl, strFormulaString, strCellsRange, iXFIndex );
          break;

        case Excel2007Serializator.FormulaType.shared:
          // We don't support formulas for external worksheets on the current moment
          SetSharedFormula( sheet as WorksheetImpl, strFormulaString, strCellsRange, uiSharedGroupIndex,
            iRow, iCol, iXFIndex, bCalculateOnOpen );
          break;

        default:
          break;
      }

      reader.Skip();
    }
    /// <summary>
    /// Extracts and fills palette settings from specified XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to get palette from.</param>
    private void ParsePalette( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.NodeType != XmlNodeType.Element ||
        reader.LocalName != Excel2007Serializator.IndexedColorsTagName )
      {
        throw new XmlException( "Cannot locate tag " + Excel2007Serializator.IndexedColorsTagName );
      }

      reader.Read();
      int iCurrentIndex = 0;

      while (reader.NodeType != XmlNodeType.EndElement || reader.LocalName != Excel2007Serializator.IndexedColorsTagName)
      {
        if( reader.NodeType == XmlNodeType.Element &&
          reader.LocalName == Excel2007Serializator.RgbColorTagName )
        {
          reader.MoveToAttribute( Excel2007Serializator.ColorRgbAttribute );
          int iValue = int.Parse( reader.Value, NumberStyles.HexNumber );
          Color color = ColorExtension.FromArgb( iValue );
          m_book.SetPaletteColor( iCurrentIndex, color );
          iCurrentIndex++;
        }

        reader.Read();
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    private void ParseColors( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.NodeType != XmlNodeType.Element ||
        reader.LocalName != Excel2007Serializator.ColorsTagName )
      {
        throw new XmlException( "Cannot locate tag " + Excel2007Serializator.ColorsTagName );
      }

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element &&
            reader.LocalName == Excel2007Serializator.IndexedColorsTagName )
          {
            ParsePalette( reader );
          }

          reader.Read();
        }
      }
    }
    /// <summary>
    /// Parses columns collection.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="sheet">Worksheet that will store extracted data.</param>
    /// <param name="arrStyles">List with new style indexes.</param>
    private void ParseColumns( XmlReader reader, WorksheetImpl sheet, List<int> arrStyles )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( arrStyles == null )
        throw new ArgumentNullException( "arrStyles" );

      if( reader.NodeType != XmlNodeType.Element ||
        reader.LocalName != Excel2007Serializator.ColsTagName )
      {
        throw new XmlException( "Unable to locate xml tag " + Excel2007Serializator.ColsTagName );
      }

      if (m_outlineLevels == null)
          m_outlineLevels = new Dictionary<int, List<Point>>();

      if (m_indexAndLevels == null)
          m_indexAndLevels = new Dictionary<int, int>();

      m_outlineWrapperUtility = new OutlineWrapperUtility(m_outlineLevels);
         
      if ( sheet != null && sheet.ColumnOutlineLevels == null)
          sheet.ColumnOutlineLevels = new Dictionary<int, List<Point>>();        
    
      reader.Read();

      while (reader.NodeType != XmlNodeType.EndElement || reader.LocalName != Excel2007Serializator.ColsTagName)
      {
        if( reader.NodeType == XmlNodeType.Element &&
          reader.LocalName == Excel2007Serializator.ColTagName )
        {
          ParseColumn( reader, sheet, arrStyles );
        }

        reader.Read();
      }

      if (sheet != null && m_indexAndLevels != null && m_indexAndLevels.Count > 0)
      {
          m_outlineWrapperUtility.UpdateOutlineColumn(sheet, m_indexAndLevels);
          m_indexAndLevels.Clear();
          m_outlineLevels = null;
      }

    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="sheet"></param>
    /// <param name="arrStyles"></param>
    private void ParseColumn( XmlReader reader, WorksheetImpl sheet, List<int> arrStyles )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( arrStyles == null )
        throw new ArgumentNullException( "arrStyles" );

      if( reader.NodeType != XmlNodeType.Element ||
        reader.LocalName != Excel2007Serializator.ColTagName )
      {
        throw new XmlException( "Unable to locate xml tag " + Excel2007Serializator.ColTagName );
      }

      ColumnInfoRecord columnInfo = ( ColumnInfoRecord )BiffRecordFactory.GetRecord( TBIFFRecord.ColumnInfo );

      if( reader.MoveToAttribute( Excel2007Serializator.ColumnMinAttribute ) )
        columnInfo.FirstColumn = ( ushort )( XmlConvert.ToUInt16( reader.Value ) - 1 );

      if( reader.MoveToAttribute( Excel2007Serializator.ColumnMaxAttribute ) )
        columnInfo.LastColumn = ( ushort )( XmlConvert.ToUInt16( reader.Value ) - 1 );

      if( reader.MoveToAttribute( Excel2007Serializator.ColumnWidthAttribute ) )
      {
        double dValue = XmlConvert.ToDouble( reader.Value );
        int iWidth = ( int )Math.Round( dValue * 256 );
        //iWidth = sheet.EvaluateRealColumnWidth( iWidth );
        columnInfo.ColumnWidth = ( ushort )iWidth;
      }

      if( reader.MoveToAttribute( Excel2007Serializator.ColumnStyleAttribute ) )
      {
        int iStyle = int.Parse( reader.Value );
        iStyle = arrStyles[ iStyle ];
        columnInfo.ExtendedFormatIndex = ( ushort )iStyle;
      }
      else
      {
        columnInfo.ExtendedFormatIndex = ( ushort )m_book.DefaultXFIndex;
      }
      if (reader.MoveToAttribute(Excel2007Serializator.BestFitAttribute))
          columnInfo.IsBestFit = XmlConvert.ToBoolean(reader.Value);

      if (reader.MoveToAttribute(Excel2007Serializator.Phonetic))
          columnInfo.IsPhenotic = XmlConvert.ToBoolean(reader.Value);

      if (reader.MoveToAttribute(Excel2007Serializator.ColumnCustomWidthAttribute))
          columnInfo.IsUserSet = XmlConvert.ToBoolean(reader.Value);

      if( reader.MoveToAttribute( Excel2007Serializator.RowColumnCollapsedAttribute ) )
        columnInfo.IsCollapsed = XmlConvert.ToBoolean( reader.Value );

      if (reader.MoveToAttribute(Excel2007Serializator.RowColumnOutlineLevelAttribute))
      {
          columnInfo.OutlineLevel = XmlConvert.ToUInt16(reader.Value);
          for (int i = columnInfo.FirstColumn; i <= columnInfo.LastColumn; i++)
          {
              m_indexAndLevels.Add(i+1,columnInfo.OutlineLevel);
          }
      }
      if (reader.MoveToAttribute(Excel2007Serializator.HiddenAttributeName))
      {
          columnInfo.IsHidden = XmlConvert.ToBoolean(reader.Value);
          columnInfo.IsCollapsed = XmlConvert.ToBoolean(reader.Value);
      }
      else
          columnInfo.IsCollapsed = false;

      sheet.ParseColumnInfo( columnInfo, false );

      ParseColumnInfoRecord(reader, sheet, -1);
    }
    private int ParseColumnInfoRecord(XmlReader xmlTextReader, WorksheetImpl worksheet, int index)
    {
        if (!xmlTextReader.HasAttributes)
        {
            xmlTextReader.Skip();
            return index;
        }
        int min = -1;
        int max = -1;
        int styleIndex = -1;
        short outLineLevel = -1;
        double width = -1.0;
        bool containsWidth = false;
        bool isHidden = false;
        bool isCollapsed = false;
        bool isBestFit = false;
        xmlTextReader.MoveToElement();
        while (xmlTextReader.MoveToNextAttribute())
        {
            string localName = xmlTextReader.LocalName;
            if (localName != null)
            {
                int value;
                if (Helper.columnAttributes == null)
                {
                    Dictionary<string, int> dictionary1 = new Dictionary<string, int>(9);
                    dictionary1.Add("min", 0);
                    dictionary1.Add("max", 1);
                    dictionary1.Add("width", 2);
                    dictionary1.Add("style", 3);
                    dictionary1.Add("hidden", 4);
                    dictionary1.Add("customWidth", 5);
                    dictionary1.Add("outlineLevel", 6);
                    dictionary1.Add("collapsed", 7);
                    dictionary1.Add("bestFit", 8);
                    Helper.columnAttributes = dictionary1;
                }
                if (Helper.columnAttributes.TryGetValue(localName, out value))
                {
                    switch (value)
                    {
                        case 0:
                            min = Helper.ParseInt(xmlTextReader.Value) - 1;
                            max = min;
                            break;

                        case 1:
                            max = Helper.ParseInt(xmlTextReader.Value) - 1;
                            break;

                        case 2:
                            width = Helper.ParseDouble(xmlTextReader.Value);
                            containsWidth = true;
                            break;

                        case 3:
                            styleIndex = Helper.ParseInt(xmlTextReader.Value);
                            break;

                        case 4:
                            isHidden = Helper.ParseBoolen(xmlTextReader.Value);
                            break;

                        case 6:
                            outLineLevel = Helper.ParseShort(xmlTextReader.Value);
                            break;

                        case 7:
                            isCollapsed = Helper.ParseBoolen(xmlTextReader.Value);
                            break;

                        case 8:
                            isBestFit = Helper.ParseBoolen(xmlTextReader.Value);
                            break;
                    }
                }
            }
        }
        xmlTextReader.MoveToElement();
        ColumnCollection columns = worksheet.Columnss;
        double columnWidth = columns.Width;
        double num7 = containsWidth ? worksheet.CharacterWidth(width) : columnWidth;
        int defaultStyleIndex = 15;
        if (styleIndex != -1)
        {
            object obj2 = null; //[Check]
            if (obj2 != null)
            {
                defaultStyleIndex = (int)obj2;
            }
        }
        for (int i = min; i <= max; i++)
        {
            Column column = null;
            if (max >= 0x3fff)
            {
                column = columns.GetOrCreateColumn();
                column.SetMinColumnIndex(min);
            }
            else if (min < index)
            {
                //column = columns[i]; //[Check]
            }
            else
            {
                column = columns.AddColumn(i);
            }
            if (containsWidth)
            {
                column.Width = num7;
            }
            column.SetStyleIndex(defaultStyleIndex);
            if (outLineLevel != -1)
            {
                column.SetOutLineLevel((byte)outLineLevel);
            }
            column.IsHidden = isHidden;
            column.SetCollapsedInfo(isCollapsed);
            column.SetBestFitInfo(isBestFit);
            if (max >= 0x3fff)
            {
                break;
            }
        }
        if (index <= max)
        {
            return max;
        }
        return index;
    }
    /// <summary>
    /// 
    /// </summary>
    private void SwitchStreams( ref bool bAdd, ref XmlWriter writer,
      ref StreamWriter textWriter, Stream streamEnd )
    {
      if( !bAdd )
      {
        bAdd = true;
        writer.WriteEndElement();
        writer.Flush();
        textWriter = new StreamWriter( streamEnd );
        writer = UtilityMethods.CreateWriter( textWriter );
        writer.WriteStartElement( Excel2007Serializator.TemporaryRoot, Excel2007Serializator.XmlNamespaceMain );
      }
    }
    /// <summary>
    /// Extracts data validation from reader and inserts it into worksheet.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="sheet">Worksheet to put data into.</param>
    public void ParseDataValidations( XmlReader reader, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( reader.LocalName != DV.DataValidationsTagName )
        throw new XmlException( "reader" );

      DValRecord dValRecord = ( DValRecord )BiffRecordFactory.GetRecord( TBIFFRecord.DVal );

      if( reader.MoveToAttribute( DV.DisablePromptsAttributeName ) )
        dValRecord.IsPromtBoxVisible = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( DV.XCoodrinateAttributeName ) )
        dValRecord.PromtBoxHPos = XmlConvert.ToInt32( reader.Value );

      if( reader.MoveToAttribute( DV.YCoodrinateAttributeName ) )
        dValRecord.PromtBoxVPos = XmlConvert.ToInt32( reader.Value );

      DataValidationCollection dvCollection = sheet.DVTable.Add( dValRecord );

      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element && reader.LocalName == DV.DataValidationTagName )
        {
          ParseDataValidation( reader, dvCollection );
        }
        else
        {
          reader.Skip();
        }
      }

      reader.Skip();
      //dvCollection.RemoveAt( 0 );
    }
    /// <summary>
    /// Extracts data validation from reader and inserts it into data validation collection.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="dvCollection">Data validation collection.</param>
    private void ParseDataValidation( XmlReader reader, DataValidationCollection dvCollection )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( dvCollection == null )
        throw new ArgumentNullException( "dvCollection" );

      if( reader.LocalName != DV.DataValidationTagName )
        throw new XmlException( "reader" );

      DataValidationImpl dataValidation = new DataValidationImpl( dvCollection );

      if( reader.MoveToAttribute( DV.CFSequenceOfReferencesAttributeName ) )
      {
        TAddr[] ranges = GetRangesForDataValidation( reader.Value );

        foreach( TAddr tAddr in ranges )
        {
          dataValidation.AddRange( tAddr );
        }
      }

      if( reader.MoveToAttribute( DV.TypeAttributeName ) )
        dataValidation.AllowType = ConvertDataValidationType( reader.Value );

      if (reader.MoveToAttribute(DV.AllowBlankAttributeName))
          dataValidation.IsEmptyCellAllowed = XmlConvert.ToBoolean(reader.Value);
      else
          dataValidation.IsEmptyCellAllowed = false;

      if( reader.MoveToAttribute( DV.ErrorMessageAttributeName ) )
        dataValidation.ErrorBoxText = ConvertToASCII(reader.Value);

      if( reader.MoveToAttribute( DV.ErrorStyleAttributeName ) )
        dataValidation.ErrorStyle = ConvertDataValidationErrorStyle( reader.Value );

      if( reader.MoveToAttribute( DV.ErrorAlertTextAttributeName ) )
        dataValidation.ErrorBoxTitle = reader.Value;

      if( reader.MoveToAttribute( DV.OperatorAttributeName ) )
        dataValidation.CompareOperator = ConvertDataValidationOperator( reader.Value );

      if( reader.MoveToAttribute( DV.InputPromptAttributeName ) )
        dataValidation.PromptBoxText = ConvertToASCII(reader.Value);

      if( reader.MoveToAttribute( DV.PromptTitleAttributeName ) )
        dataValidation.PromptBoxTitle = reader.Value;

      if( reader.MoveToAttribute( DV.ShowDropDownAttributeName ) )
        dataValidation.IsSuppressDropDownArrow = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( DV.ShowErrorMessageAttributeName ) )
        dataValidation.ShowErrorBox = XmlConvert.ToBoolean( reader.Value );
      else
	    dataValidation.ShowErrorBox =false;

      if (reader.MoveToAttribute(DV.ShowInputMessageAttributeName))
          dataValidation.ShowPromptBox = XmlConvert.ToBoolean(reader.Value);
      else
          dataValidation.ShowPromptBox = false;

      reader.MoveToElement();

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.LocalName == DV.FormulaOneTagName ||
            reader.LocalName == DV.FormulaTwoTagName )
          {
            ParseFormulaOneTwoValues( reader, dataValidation );
          }

          reader.Skip();
        }
      }

      DetectIsStringList( dataValidation );

      reader.Read();
      dvCollection.Add( dataValidation );
    }
    /// <summary>
    /// Detext value of IsStrListExplicit property.
    /// </summary>
    private void DetectIsStringList( DataValidationImpl dataValidation )
    {
      DVRecord record = dataValidation.DVRecord;

      Ptg[] tokens = record.FirstFormulaTokens;

      record.IsStrListExplicit = ( record.DataType == ExcelDataType.User &&
        tokens != null && record.SecondFormulaTokens == null &&
        tokens.Length == 1 && tokens[ 0 ].TokenCode == FormulaToken.tStringConstant );
    }
    /// <summary>
    /// Extracts data validation formula values from reader and inserts it into data validation.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="dataValidation">Data validation implementation.</param>
    private void ParseFormulaOneTwoValues( XmlReader reader, DataValidationImpl dataValidation )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( dataValidation == null )
        throw new ArgumentNullException( "dataValidation" );

      string strFormulaType = reader.LocalName;

      reader.Read();

      if( strFormulaType == DV.FormulaOneTagName )
      {
        dataValidation.SetFormulaOneTwoValue( reader.Value, m_formulaUtil, true );
      }
      else
      {
        dataValidation.SetFormulaOneTwoValue( reader.Value, m_formulaUtil, false );
      }

      reader.Skip();
    }

    /// <summary>
    /// Extracts auto filters from reader and inserts it into worksheet.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="sheet">Worksheet to put data into.</param>
    public void ParseAutoFilters( XmlReader reader, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      bool bIsEmpty = reader.IsEmptyElement;

      AutoFiltersCollection autoFilters = ( AutoFiltersCollection )sheet.AutoFilters;

      if( reader.MoveToAttribute( AF.CellOrRangeReferenceAttributeName ) )
      {
        TAddr tAddr = GetRangeForDVOrAF( reader.Value );
        autoFilters.FilterRange = sheet[ tAddr.FirstRow + 1, tAddr.FirstCol + 1, tAddr.LastRow + 1, tAddr.LastCol + 1 ];
      }

      if( reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.LocalName == AF.AutoFilterColumnTagName )
          {
            ParseFilterColumn( reader, autoFilters );
          }

          reader.Skip();
        }
      }

      reader.Skip();
    }
    /// <summary>
    /// Extracts filter column from reader and inserts it into auto filter collection.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="autoFilters">Auto filter collection to put data into.</param>
    private void ParseFilterColumn( XmlReader reader, AutoFiltersCollection autoFilters )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( autoFilters == null )
        throw new ArgumentNullException( "autoFilters" );

      if( !reader.MoveToAttribute( AF.FilterColumnDataAttributeName ) )
        throw new XmlReadingException( "Required attribute wan not present." );

      int iIndex = XmlConvert.ToInt32( reader.Value );
      AutoFilterImpl autoFilter = ( AutoFilterImpl )autoFilters[ iIndex ];
      autoFilter.Index = iIndex;

      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.LocalName == AF.CustomFiltersCriteriaTagName )
          ParseCustomFilters( reader, autoFilter );

        if( reader.LocalName == AF.AutoFilterTopTenTagName )
          ParseAutoFilterTopTen( reader, autoFilter );

        if( reader.LocalName == AF.FilterCriteriaTagName )
          ParseFilters( reader, autoFilter );

        reader.Skip();
      }

      reader.Skip();
    }
    /// <summary>
    /// Extracts filters from reader and inserts it into auto filter.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="autoFilter">Auto filter to put data into.</param>
    private void ParseFilters( XmlReader reader, AutoFilterImpl autoFilter )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( autoFilter == null )
        throw new ArgumentNullException( "autoFilter" );

      reader.Read();

      //In current implementation we parse only the first filter condition,
      //though in MS Excel 2007 the can be more than one filter condition.
      if( reader.LocalName == AF.FilterTagName )
      {
        if( reader.MoveToAttribute( AF.FilterValueAttributeName ) )
        {
          autoFilter.IsSimple1 = true;
          AutoFilterConditionImpl autoFilterCondition = ( AutoFilterConditionImpl )autoFilter.FirstCondition;
          autoFilterCondition.DataType = ExcelFilterDataType.String;
          autoFilterCondition.ConditionOperator = ExcelFilterCondition.Equal;
          autoFilterCondition.String = reader.Value;

          reader.Skip();
        }
      }
    }
    /// <summary>
    /// Extracts auto filter top ten from reader and inserts it into auto filter.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="autoFilter">Auto filter to put data into.</param>
    private void ParseAutoFilterTopTen( XmlReader reader, AutoFilterImpl autoFilter )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( autoFilter == null )
        throw new ArgumentNullException( "autoFilter" );

      autoFilter.IsTop10 = true;
      autoFilter.IsTop = true;

      if( reader.MoveToAttribute( AF.TopAttributeAttributeName ) )
        autoFilter.IsTop = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( AF.FilterByPercentAttributeName ) )
        autoFilter.IsPercent = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( AF.TopOrBottomValueAttributeName ) )
        autoFilter.Top10Number = XmlConvert.ToInt32( reader.Value );

      if( reader.MoveToAttribute( AF.FilterValAttributeName ) )
      {
        AutoFilterConditionImpl autoFilterCondition = ( AutoFilterConditionImpl )autoFilter.FirstCondition;
        autoFilterCondition.ConditionOperator = ExcelFilterCondition.GreaterOrEqual;
        autoFilterCondition.DataType = ExcelFilterDataType.FloatingPoint;
        autoFilterCondition.Double = XmlConvert.ToDouble( reader.Value );
      }
    }
    /// <summary>
    /// Extracts custom filters from reader and inserts it into auto filter.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="autoFilter">Auto filter to put data into.</param>
    private void ParseCustomFilters( XmlReader reader, AutoFilterImpl autoFilter )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( autoFilter == null )
        throw new ArgumentNullException( "autoFilter" );

      if( reader.MoveToAttribute( AF.AndCriteriaAttributeName ) )
        autoFilter.IsAnd = XmlConvert.ToBoolean( reader.Value );

      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.LocalName == AF.CustomFilterCriteriaTagName )
          ParseCustomFilter( reader, autoFilter );

        reader.Skip();
      }
    }
    /// <summary>
    /// Extracts custom filter from reader and inserts it into auto filter.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="autoFilter">Auto filter to put data into.</param>
    private void ParseCustomFilter( XmlReader reader, AutoFilterImpl autoFilter )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( autoFilter == null )
        throw new ArgumentNullException( "autoFilter" );

      AutoFilterConditionImpl autoFilterCondition;

      autoFilterCondition = ( autoFilter.IsFirstCondition ) ? ( AutoFilterConditionImpl )autoFilter.FirstCondition :
        ( AutoFilterConditionImpl )autoFilter.SecondCondition;

      autoFilterCondition.ConditionOperator = ExcelFilterCondition.Equal;

      if( reader.MoveToAttribute( AF.FilterComparisonOperatorAttributeName ) )
        autoFilterCondition.ConditionOperator = ConvertAutoFormatFilterCondition( reader.Value );

      if( reader.MoveToAttribute( AF.FilterValueAttributeName ) )
      {
        autoFilterCondition.String = reader.Value;
        autoFilterCondition.DataType = ExcelFilterDataType.String;
      }
    }
    /// <summary>
    /// Extracts themes.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    public List<Color> ParseThemes( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      while( reader.NodeType != XmlNodeType.Element )
        reader.Read();

      if( reader.LocalName != Excel2007Serializator.ThemeTagName )
        throw new XmlException( "reader" );

      if( reader.IsEmptyElement )
        return null;

      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.LocalName == Excel2007Serializator.ThemeElementsTagName )
          m_lstThemeColors = ParseThemeElements( reader);

        reader.Skip();
      }
      m_book.MajorFonts = m_dicMajorFonts;
      m_book.MinorFonts = m_dicMinorFonts;
      return m_lstThemeColors;
    }
   public List<Color> ParseThemeElements(XmlReader reader)
   {
       if( reader == null )
        throw new ArgumentNullException( "reader" );
         
       if( reader.IsEmptyElement )
        return null;

       List<Color> lstColors=new List<Color>();

       reader.Read();
         while (reader.NodeType != XmlNodeType.EndElement)
         {
             if (reader.NodeType == XmlNodeType.Element)
             {
                 switch (reader.LocalName)
                 {
                     case "clrScheme":
                         lstColors = ParseThemeColors(reader, out m_dicThemeColors);
                         break;

                     case "fontScheme":
                         ParseFontScheme(reader);
                         break;
                 }
             }
             else
             {
                 reader.Skip();
             }
         }
         return lstColors;
   }

   private void ParseFontScheme(XmlReader reader)
   {
      if(reader ==null)
          throw new ArgumentNullException("reader");
      reader.Read();
      while (reader.NodeType != XmlNodeType.EndElement)
      {
          if (reader.NodeType == XmlNodeType.Element)
          {
              switch (reader.LocalName)
              {
                  case "majorFont":
                      ParseMajorFont(reader, out m_dicMajorFonts);
                      break;
                  case "minorFont":
                      ParseMinorFont(reader, out m_dicMinorFonts);
                      break;
                  default:
                      reader.Skip();
                      break;
              }
          }
          else
          {
              reader.Skip();
          }
      }   

   }

   private void ParseMinorFont(XmlReader reader, out Dictionary<string, FontImpl> dicMinorFonts)
   {
       if (reader == null)
           throw new ArgumentNullException("reader");

       reader.Read();
       FontImpl font = null;
       dicMinorFonts = new Dictionary<string, FontImpl>();
       while (reader.NodeType != XmlNodeType.EndElement)
       {
           if (reader.NodeType == XmlNodeType.Element)
           {
               switch (reader.LocalName)
               {
                   case Drawings.LatinTag:
                       font = GetFont(reader);
                       dicMinorFonts.Add(Drawings.LatinTag, font);
                       break;
                   case Drawings.EaTag:
                       font = GetFont(reader);
                       dicMinorFonts.Add(Drawings.EaTag, font);
                       break;
                   case Drawings.CsTag:
                       font = GetFont(reader);
                       dicMinorFonts.Add(Drawings.CsTag, font);
                       break;
                   default:
                       reader.Skip();
                       break;
               }
           }
           else
           {
               reader.Skip();
           }
       }
       reader.Read();
   }

   private void ParseMajorFont(XmlReader reader, out Dictionary<string, FontImpl> dicMajorFonts)
   {
       if(reader == null)
       throw new ArgumentNullException("reader");

       reader.Read();
       FontImpl font=null;
       dicMajorFonts = new Dictionary<string, FontImpl>();
       while (reader.NodeType != XmlNodeType.EndElement)
       {
           if (reader.NodeType == XmlNodeType.Element)
           {
               switch (reader.LocalName)
               {
                   case Drawings.LatinTag:
                       font = GetFont(reader);
                       dicMajorFonts.Add("lt", font);                       
                       break;

                   case Drawings.EaTag:
                       font = GetFont(reader);
                       dicMajorFonts.Add(Drawings.EaTag, font);
                       
                       break;
                   case Drawings.CsTag:
                       font = GetFont(reader);
                       dicMajorFonts.Add(Drawings.CsTag, font);
                       
                       break;
                   default:
                       reader.Skip();
                       break;
               }               
           }
           else
           {
               reader.Skip();
           }
       }
       reader.Read();
   }
   public FontImpl GetFont(XmlReader reader)
   {
       FontImpl font = null;
       if (reader.MoveToAttribute(Drawings.TypefaceTag))
       {
           font = (FontImpl)m_book.CreateFont(null, false);
           font.FontName = reader.Value;
       }
       return font;
   }
   /// <summary>
   /// Skips the white spaces in the XML.
   /// </summary>
   /// <param name="reader"></param>
   public static void SkipWhiteSpaces(XmlReader reader)
   {
       while (reader.NodeType == XmlNodeType.Whitespace)
           reader.Read();
   }
    /// <summary>
    /// Extracts theme colors.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="dicThemeColors">Dictionary that will be filled with theme colors,
    /// key - color name, value - color value.</param>
    /// <returns>Returns theme colors list.</returns>
    public List<Color> ParseThemeColors( XmlReader reader, out Dictionary<string, Color> dicThemeColors )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      dicThemeColors = null;

      if( reader.IsEmptyElement )
        return null;

      reader.Read();

      List<Color> lstColors = new List<Color>();
      dicThemeColors = new Dictionary<string, Color>();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          string strColorName = reader.LocalName;
          reader.Read();
          SkipWhiteSpaces(reader);
          Color color = ColorExtension.Black;

          if( reader.LocalName == Drawings.SRGBColorTag )
          {
            if( reader.MoveToAttribute( Excel2007Serializator.RGBHexColorValueAttributeName ) )
            {
              color = ColorExtension.FromArgb( int.Parse( reader.Value, NumberStyles.HexNumber ) );
            }

            reader.Skip();
            SkipWhiteSpaces(reader);
          }

          else if( reader.LocalName == Excel2007Serializator.SystemColorTagName )
          {
            if( reader.MoveToAttribute( Excel2007Serializator.SystemColorValueAttributeName ) )
            {
              color = ColorExtension.FromName( reader.Value );
            }

            reader.Skip();
            SkipWhiteSpaces(reader);
          }

          lstColors.Add( color );
          m_dicThemeColors.Add( strColorName, color );

          reader.Skip();
        }
        else
        {
          reader.Skip();
        }
      }

      lstColors.Reverse( 0, 2 );
      lstColors.Reverse( 2, 2 );

      m_dicThemeColors.Add( "tx1", lstColors[ 1 ] );
      m_dicThemeColors.Add( "tx2", lstColors[ 3 ] );

      reader.Read();
      return lstColors;
    }

    /// <summary>
    /// Extracts Dxfs collection from XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <returns>List of Dxf styles.</returns>
    public List<DxfImpl> ParseDxfCollection( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.LocalName != Excel2007Serializator.DiffXFsTagName )
        throw new XmlException( "reader" );

      List<DxfImpl> lstDfxs = new List<DxfImpl>();

      reader.Read();

      if( reader.NodeType != XmlNodeType.None )
      {

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.LocalName == Excel2007Serializator.DxfFormattingTagName )
          {
            lstDfxs.Add( ParseDxfStyle( reader ) );
          }
          else
          {
            reader.Skip();
          }
        }
      }

      return lstDfxs;
    }
    /// <summary>
    /// Extracts single Dxf style record from XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <returns>Dxf style object.</returns>
    private DxfImpl ParseDxfStyle( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      DxfImpl dxfStyle = new DxfImpl();

      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        switch( reader.LocalName )
        {
          case Excel2007Serializator.NumberFormatTagName:
            dxfStyle.FormatRecord = ParseDxfNumberFormat(reader);
            reader.Skip();
            break;

          case Excel2007Serializator.FillTagName:
            dxfStyle.Fill = ParseFill( reader, /*true*/ false );
            reader.Skip();
            break;

          case Excel2007Serializator.FontTagName:
            dxfStyle.Font = ParseFont( reader );
            reader.Skip();
            break;

          case Excel2007Serializator.BordersCollectionTagName:
            dxfStyle.Borders = ParseBordersCollection( reader );
            //reader.Skip();
            break;

          default:
            reader.Skip();
            break;
        }
      }

      reader.Read();

      return dxfStyle;
    }

    /// <summary>
    /// Extracts worksheet conditional formats from XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="sheetConditionalFormats">Worksheet conditional formats.</param>
    /// <param name="lstDxfs">Dxf styles collection.</param>
    public void ParseSheetConditionalFormatting( XmlReader reader, WorksheetConditionalFormats sheetConditionalFormats,
      List<DxfImpl> lstDxfs )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.NodeType == XmlNodeType.None )
        return;

      if( sheetConditionalFormats == null )
        throw new ArgumentNullException( "sheetConditionalFormats" );

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.LocalName == CF.ConditionalFormattingTagName )
        {
          ConditionalFormats conditionalFormats = new ConditionalFormats( m_book.Application, sheetConditionalFormats );
          bool isAdd=ParseConditionalFormatting( reader, conditionalFormats, lstDxfs );

          if (conditionalFormats.Count != 0)
          {
              if (conditionalFormats.CondFMTRecord != null && conditionalFormats.CondFMTRecord.CellList.Count > 0 && conditionalFormats.CondFMTRecord.CFNumber==0)
              {
                  conditionalFormats.CondFMTRecord.CFNumber = (ushort)conditionalFormats.InnerList.Count;
              }
              if (conditionalFormats.CondFMT12Record != null && conditionalFormats.CondFMT12Record.CellList.Count > 0 && conditionalFormats.CondFMT12Record.CF12RecordCount == 0)
              {
                  conditionalFormats.CondFMT12Record.CF12RecordCount = (ushort)conditionalFormats.InnerList.Count;
              }
              sheetConditionalFormats.Add(conditionalFormats);
          }
        }
        else
        {
          reader.Skip();
        }
      }
    }
    /// <summary>
    /// Extracts conditional formats from XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="conditionalFormats">Conditional formats.</param>
    /// <param name="lstDxfs">Dxf styles collection.</param>
    public bool ParseConditionalFormatting( XmlReader reader, ConditionalFormats conditionalFormats, List<DxfImpl> lstDxfs )
    {
      if( reader == null || reader.LocalName != CF.ConditionalFormattingTagName )
        throw new ArgumentException( "reader" );

      if( conditionalFormats == null )
        throw new ArgumentNullException( "conditionalFormats" );
      bool add = true;
      if( reader.MoveToAttribute( DV.CFSequenceOfReferencesAttributeName ) )
      {
        TAddr[] tAddrRanges = GetRangesForDataValidation( reader.Value );
        foreach( TAddr tAddr in tAddrRanges )
        {
          Rectangle rectangle = tAddr.GetRectangle();
          //rectangle.X--;
          //rectangle.Y--;
          conditionalFormats.AddRange( rectangle );
          conditionalFormats.EnclosedRange = tAddr;
        }
      }

      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case CF.RuleTagName:
              ParseCFRuleTag( reader, conditionalFormats, lstDxfs );
              break;

            default:
              reader.Read();
              break;
          }
        }
        else
        {
          reader.Skip();
        }
      }

      reader.Read();
      return add;
    }
      /// <summary>
      /// Parses the Range Reference for Conditional formatting
      /// </summary>
    /// <param name="reader">XmlReader to extract data from</param>
      /// <param name="format">Conditional format value</param>
  private void ParseRangeReference(XmlReader reader, IConditionalFormat format)
  {
      ConditionalFormatImpl cFormat = format as ConditionalFormatImpl;
      if (reader == null)
          throw new ArgumentNullException("reader");

      if (format == null)
          throw new ArgumentNullException("format");

      if (reader.LocalName == Excel2007Serializator.RangeReferenceAttribute)
        {
            reader.Read();
          cFormat .RangeRefernce = reader.Value;
          reader.Read();
        }
  }
    /// <summary>
    /// Parses conditional formatting rule tag.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="conditionalFormats">Condition to extract data into.</param>
    /// <param name="lstDxfs">Dxf styles collection.</param>
    public void ParseCFRuleTag( XmlReader reader, ConditionalFormats conditionalFormats, List<DxfImpl> lstDxfs )
    {
      bool bIsValidType = false;
      bool bIsValidOperator = false;
      bool stopIfTrue = false;
      bool bIsValidTimePeriod = false;
      int iPriority = 1;
      string text = string.Empty;
      ExcelCFType cfType = ExcelCFType.CellValue;
      ExcelComparisonOperator cfOperator = ExcelComparisonOperator.Between;
      CFTimePeriods cfTimePeriod = CFTimePeriods.Yesterday; 

      if( reader.MoveToAttribute( CF.TypeAttributeName ) )
      {
        cfType = ConvertCFType( reader.Value, out bIsValidType );
      }

      if (reader.MoveToAttribute(CF.StopIfTrueAttributeName))
      {
          stopIfTrue = XmlConvert.ToBoolean(reader.Value);
      }

      if (reader.MoveToAttribute(CF.PriorityAttributeName))
      {
          iPriority = int.Parse(reader.Value);
      }

      if( reader.MoveToAttribute( CF.OperatorAttributeName ) )
      {
          if (cfType == ExcelCFType.TimePeriod)
          {
              cfTimePeriod = ConvertCFTimePeriods(reader.Value, out bIsValidTimePeriod);
          }
          else
          {
              cfOperator = ConvertCFOperator(reader.Value, out bIsValidOperator);
          }
      }
      if (reader.MoveToAttribute(CF.TextAttributeName))
      {
          text = reader.Value;
      }

      if( bIsValidType )
      {
        IInternalConditionalFormat conFormat = conditionalFormats.AddCondition() as IInternalConditionalFormat;
        conFormat.FormatType = cfType;
        conFormat.StopIfTrue = stopIfTrue;
        ConditionalFormatImpl formatImp = conFormat as ConditionalFormatImpl;
        if (formatImp != null)
            formatImp.Priority = iPriority;

        if (bIsValidOperator)
            conFormat.Operator = cfOperator;
        else if (bIsValidTimePeriod)
            conFormat.TimePeriodType = cfTimePeriod;

        if (cfType == ExcelCFType.SpecificText)
            conFormat.Text = text;

        ParseConditionFormatRule( reader, conFormat, lstDxfs );

          if(reader .LocalName == Excel2007Serializator.RangeReferenceAttribute)
              ParseRangeReference(reader, conFormat);

      }
    }
    /// <summary>
    /// Extracts conditional formats single condition from XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="conFormat">Condition to extract data into.</param>
    /// <param name="lstDxfs">Dxf styles collection.</param>
    private void ParseConditionFormatRule( XmlReader reader, IInternalConditionalFormat conFormat, List<DxfImpl> lstDxfs )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( conFormat == null )
        throw new ArgumentNullException( "conFormat" );

      if( reader.MoveToAttribute( CF.DifferentialFormattingIdAttributeName ) )
      {
        int iDxfIndex = XmlConvert.ToInt32( reader.Value );
        DxfImpl dxfImpl = lstDxfs[ iDxfIndex ];
        dxfImpl.FillCondition( conFormat );
      }

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            ConditionalFormatImpl cFormat = ( ConditionalFormatImpl )conFormat;

            switch( reader.LocalName )
            {
              case Excel2007Serializator.FormulaTagName:
              case CF.FormulaTagName:
                ParseCFFormulas( reader, cFormat );
                break;
              case Excel2007Serializator.DxfFormattingTagName:
                lstDxfs.Add(ParseDxfStyle(reader));
                break;
              case CF.DataBarTag:
                ParseDataBar( reader, cFormat.InnerDataBar, cFormat.Workbook );
                break;

              case CF.IconSetTag:
                ParseIconSet( reader, cFormat.IconSet, cFormat.Workbook );
                break;

              case CF.ColorScaleTag:
                ParseColorScale( reader, cFormat.ColorScale, cFormat.Workbook );
                break;

              default:
                reader.Read();
                //Debug.Fail( "Not supported" );
                break;
            }
          }
          else
          {
            reader.Read();
          }
        }
      }

      reader.Read();
    }
    /// <summary>
    /// Extracts color scale from the reader.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="colorScale">Color scale to fill.</param>
    /// <param name="book">Parent workbook.</param>
    private void ParseColorScale( XmlReader reader, IColorScale colorScale, WorkbookImpl book )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( colorScale == null )
        throw new ArgumentNullException( "colorScale" );

      if( reader.LocalName != CF.ColorScaleTag )
        throw new XmlException();

      IList<IColorConditionValue> valueObjects = colorScale.Criteria;
      valueObjects.Clear();

      if( !reader.IsEmptyElement )
      {
        reader.Read();
        int iValueIndex = 0;

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case CF.ValueObjectTag:
                ColorConditionValue condValue = new ColorConditionValue();
                ParseCFValueObject( reader, book, condValue );
                valueObjects.Add( condValue );
                break;

              case Excel2007Serializator.ColorTagName:
                valueObjects[ iValueIndex ].FormatColorRGB = ParseColor( reader ).GetRGB( book );
                iValueIndex++;
                break;

              default:
                reader.Skip();
                break;
            }
          }
          else
          {
            reader.Read();
          }
        }
      }

      reader.Read();
    }
    /// <summary>
    /// Extracts data bar object from the reader.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="dataBar">Data bar to put extracted data into.</param>
    /// <param name="book">Parent workbook.</param>
    private void ParseDataBar( XmlReader reader, DataBarImpl dataBar, IWorkbook book )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( dataBar == null )
        throw new ArgumentNullException( "dataBar" );

      if( reader.LocalName != CF.DataBarTag )
        throw new XmlException();

      if( reader.MoveToAttribute( CF.MinLengthTag ) )
        dataBar.PercentMin = XmlConvert.ToInt32( reader.Value );

      if( reader.MoveToAttribute( CF.MaxLengthTag ) )
        dataBar.PercentMax = XmlConvert.ToInt32( reader.Value );

      if( reader.MoveToAttribute( CF.ShowValueAttribute ) )
        dataBar.ShowValue = XmlConvert.ToBoolean( reader.Value );

      reader.MoveToElement();

      if( !reader.IsEmptyElement )
      {
        reader.Read();
        int iValueIndex = 0;

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case CF.ValueObjectTag:
                ConditionValue condValue = new ConditionValue();
                ParseCFValueObject( reader, book, condValue );

                if( iValueIndex == 0 )
                {
                  dataBar.MinPoint = condValue;
                }
                else if( iValueIndex == 1 )
                {
                  dataBar.MaxPoint = condValue;
                }
                else
                {
                  throw new XmlException();
                }

                iValueIndex++;
                break;

              case Excel2007Serializator.ColorTagName:
                dataBar.BarColor = ParseColor( reader ).GetRGB( book );
                break;

              default:
                reader.Skip();
                break;
            }
          }
          else
          {
            reader.Read();
          }
        }
      }

      reader.Read();
    }
    /// <summary>
    /// Extracts icon set object from the reader.
    /// </summary>
    /// <param name="reader">XmlReader to get icon set from.</param>
    /// <param name="iconSet">Icon set object to put extracted data into.</param>
    /// <param name="book">Parent workbook.</param>
    private void ParseIconSet( XmlReader reader, IIconSet iconSet, IWorkbook book )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( iconSet == null )
        throw new ArgumentNullException( "iconSet" );

      if( reader.LocalName != CF.IconSetTag )
        throw new XmlException();

      if (reader.MoveToAttribute(CF.IconSetAttribute))
          iconSet.IconSet = (ExcelIconSetType)Array.IndexOf(CF.IconSetTypeNames, reader.Value);

      if( reader.MoveToAttribute( CF.PercentAttribute ) )
        iconSet.PercentileValues = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( CF.ReverseAttribute ) )
        iconSet.ReverseOrder = !XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( CF.ShowValueAttribute ) )
        iconSet.ShowIconOnly = !XmlConvert.ToBoolean( reader.Value );

      reader.MoveToElement();
      IList<IConditionValue> valueObjects = iconSet.IconCriteria;

      if( !reader.IsEmptyElement )
      {
        reader.Read();
        int iValueIndex = 0;

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case CF.ValueObjectTag:
                ConditionValue condValue = new ConditionValue();
                ParseCFValueObject( reader, book, condValue );
                valueObjects[ iValueIndex ] = condValue;
                iValueIndex++;
                break;

              default:
                reader.Skip();
                break;
            }
          }
          else
          {
            reader.Read();
          }
        }
      }

      reader.Read();
    }
    /// <summary>
    /// Extracts conditional format value object from reader.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="book">Parent workbook.</param>
    /// <param name="condition">Condition object to put extracted data into.</param>
    /// <returns>Extracted object.</returns>
    private void ParseCFValueObject( XmlReader reader, IWorkbook book, ConditionValue condition )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( book == null )
        throw new ArgumentNullException( "book" );

      if( reader.LocalName != CF.ValueObjectTag )
        throw new XmlException();


      string strType = null;
      string strValue = null;
      bool strConditon = true;

      if( reader.MoveToAttribute( CF.TypeAttributeName ) )
        strType = reader.Value;

      if( reader.MoveToAttribute( Excel2007Serializator.ValueAttributeName ) )
        strValue = reader.Value;

      if (reader.MoveToAttribute(CF.GreaterAttribute))
          strConditon = XmlConvert.ToBoolean(reader.Value);

      condition.Operator = strConditon ?
          ConditionalFormatOperator.GreaterThanorEqualTo : ConditionalFormatOperator.GreaterThan;
      ConditionValueType valueType = GetValueType( strType );
      condition.Type = valueType;
      condition.Value = strValue;
    }
    /// <summary>
    /// Converts string representation of conditional format value object type to corresponding enumeration.
    /// </summary>
    /// <param name="strType">String value to convert.</param>
    /// <returns>Converted enumeration object.</returns>
    private ConditionValueType GetValueType( string strType )
    {
      return ( ConditionValueType )Array.IndexOf( CF.ValueTypes, strType );
    }
    /// <summary>
    /// Extract conditional format formulas.
    /// </summary>
    /// <param name="reader">Reader to get values from.</param>
    /// <param name="cFormat">Conditional format to put extracted formulas into.</param>
    private void ParseCFFormulas( XmlReader reader, ConditionalFormatImpl cFormat )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( cFormat == null )
        throw new ArgumentNullException( "cFormat" );
      if (reader.LocalName == Excel2007Serializator.FormulaTagName)
          cFormat.CFHasExtensionList = true;
      if( reader.LocalName == CF.FormulaTagName || reader .LocalName == Excel2007Serializator .FormulaTagName)
      {
        reader.Read();
          if(!cFormat .CFHasExtensionList )
        cFormat.SetFirstSecondFormula( m_formulaUtil, reader.Value, true );
          else
        cFormat.FirstFormula = reader.Value;
        reader.Skip();
        reader.Skip();
      }

      if (reader.LocalName == CF.FormulaTagName || reader.LocalName == Excel2007Serializator.FormulaTagName)
      {
        reader.Read();
        if (!cFormat.CFHasExtensionList)
            cFormat.SetFirstSecondFormula(m_formulaUtil, reader.Value, false);
        else if (cFormat.FormatType == ExcelCFType.SpecificText)
            cFormat.Range = (cFormat.Parent as ConditionalFormats).sheet.Range[reader.Value.Replace("$", string.Empty)] as RangeImpl;
        else
            cFormat.AsteriskRange = reader.Value;
        reader.Skip();
        reader.Skip();
      }
    }
    ///// <summary>
    ///// Parse type of conditional format.
    ///// </summary>
    ///// <param name="type">Type to parse.</param>
    ///// <param name="conFormat">Conditional format to set type for.</param>
    //private void ParseCFType( string type, IInternalConditionalFormat conFormat )
    //{
    //  if( type == null || type.Length == 0 )
    //    throw new ArgumentOutOfRangeException( "type" );

    //  if( conFormat == null )
    //    throw new ArgumentNullException( "conFormat" );
    //  ExcelCFType cfType = ExcelCFType.CellValue;

    //  // TODO: create array.
    //  switch( type )
    //  {
    //    case CF.TypeCellIs:
    //      cfType = ExcelCFType.CellValue;
    //      break;

    //    case CF.TypeExpression:
    //      cfType = ExcelCFType.Formula;
    //      break;

    //    case CF.TypeDataBar:
    //      cfType = ExcelCFType.DataBar;
    //      break;

    //    default:
    //      throw new NotSupportedException();
    //  }

    //  conFormat.FormatType = cfType;
    //}

    /// <summary>
    /// Parses print options.
    /// </summary>
    /// <param name="reader">XmlReader to parse print options from.</param>
    /// <param name="pageSetup">PageSetup object to put print options into.</param>
    public static void ParsePrintOptions( XmlReader reader, IPageSetupBase pageSetup )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( pageSetup == null )
        throw new ArgumentNullException( "pageSetup" );

      if( reader.LocalName != PageSetup.PrintOptionsTag )
        throw new XmlException( "Unexpected xml tag." );
      // TODO: find out when we have to parse GridLinesSet property and whether we have any corresponding value.

      PageSetupImpl sheetSetup = pageSetup as PageSetupImpl;

      if( sheetSetup != null )
      {
        sheetSetup.PrintGridlines = ( reader.MoveToAttribute( PageSetup.GridLines ) ) ?
          XmlConvert.ToBoolean( reader.Value ) :
          false;

        sheetSetup.PrintGridlines |= reader.MoveToAttribute( PageSetup.GridLinesSet ) ?
          XmlConvert.ToBoolean( reader.Value ) :
          false;

        sheetSetup.PrintHeadings = reader.MoveToAttribute( PageSetup.Headings ) ?
          XmlConvert.ToBoolean( reader.Value ) :
          false;
      }

      pageSetup.CenterHorizontally = reader.MoveToAttribute( PageSetup.HorizontalCentered ) ?
        XmlConvert.ToBoolean( reader.Value ) :
        false;

      pageSetup.CenterVertically = reader.MoveToAttribute( PageSetup.VerticalCentered ) ?
        XmlConvert.ToBoolean( reader.Value ) :
        false;

      reader.Read();
    }
    /// <summary>
    /// Parses page margins.
    /// </summary>
    /// <param name="reader">XmlReader to get values from.</param>
    /// <param name="pageSetup">Object to put extracted values into.</param>
    public static void ParsePageMargins( XmlReader reader, IPageSetupBase pageSetup,
      IPageSetupConstantsProvider constants )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( pageSetup == null )
        throw new ArgumentNullException( "pageSetup" );

      if( reader.LocalName != constants.PageMarginsTag )
        throw new XmlException( "Unexpected xml tag." );

      if( reader.MoveToAttribute( constants.LeftMargin ) )
        pageSetup.LeftMargin = XmlConvert.ToDouble( reader.Value );

      if( reader.MoveToAttribute( constants.RightMargin ) )
        pageSetup.RightMargin = XmlConvert.ToDouble( reader.Value );

      if( reader.MoveToAttribute( constants.TopMargin ) )
        pageSetup.TopMargin = XmlConvert.ToDouble( reader.Value );

      if( reader.MoveToAttribute( constants.BottomMargin ) )
        pageSetup.BottomMargin = XmlConvert.ToDouble( reader.Value );

      if( reader.MoveToAttribute( constants.HeaderMargin ) )
        pageSetup.HeaderMargin = XmlConvert.ToDouble( reader.Value );

      if( reader.MoveToAttribute( constants.FooterMargin ) )
        pageSetup.FooterMargin = XmlConvert.ToDouble( reader.Value );

      reader.MoveToElement();
      reader.Skip();
    }
    /// <summary>
    /// Serialize PageSetup tag.
    /// </summary>
    /// <param name="reader">XmlReader to get page setup values from.</param>
    /// <param name="pageSetup">Object to put extracted values into.</param>
    public static void ParsePageSetup( XmlReader reader, PageSetupBaseImpl pageSetup )
    {
      if( reader == null )
        throw new ArgumentNullException( "writer" );

      if( pageSetup == null )
        throw new ArgumentNullException( "pageSetup" );

      if( reader.LocalName != PageSetup.PageSetupTag )
        throw new XmlException( "Unexpected xml tag." );

      if( reader.MoveToAttribute( PageSetup.PaperSize ) )
      {
        pageSetup.PaperSize = ( ExcelPaperSize )XmlConvert.ToInt32( reader.Value );
      }
      else
      {
        pageSetup.PaperSize = ExcelPaperSize.PaperLetter;
      }

      if( reader.MoveToAttribute( PageSetup.Scale ) )
      {
        int iZoom = XmlConvert.ToInt32( reader.Value );

        iZoom = ( iZoom > 400 ) ? 400 : iZoom;
        iZoom = ( iZoom < 10 ) ? 10 : iZoom;

        pageSetup.Zoom = iZoom;
      }
      else
      {
        pageSetup.Zoom = 100;
      }

      if (reader.MoveToAttribute(PageSetup.FirstPageNumber))
      {
          uint firstPageNumber = XmlConvert.ToUInt32(reader.Value);
          pageSetup.FirstPageNumber = (short)firstPageNumber;
      }

      if( reader.MoveToAttribute( PageSetup.FitToWidth ) )
        pageSetup.FitToPagesWide = XmlConvert.ToInt32( reader.Value );

      if( reader.MoveToAttribute( PageSetup.FitToHeight ) )
        pageSetup.FitToPagesTall = XmlConvert.ToInt32( reader.Value );

      if( reader.MoveToAttribute( PageSetup.PageOrder ) )
        pageSetup.Order = ( ExcelOrder )Enum.Parse( typeof( ExcelOrder ), reader.Value, true );

      if (reader.MoveToAttribute(PageSetup.Orientation))
        if((reader.Value.ToUpper() == "LANDSCAPE")||(reader.Value.ToUpper() == "PORTRAIT"))
        pageSetup.Orientation = ( ExcelPageOrientation )Enum.Parse( typeof( ExcelPageOrientation ), reader.Value, true );

      //SerializeAttribute( writer, PageSetup.UsePrinterDefaults, pageSetup.IsNotValidSettings, true );

      if( reader.MoveToAttribute( PageSetup.BlackAndWhite ) )
        pageSetup.BlackAndWhite = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( PageSetup.Draft ) )
        pageSetup.Draft = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( PageSetup.CellComments ) )
        pageSetup.PrintComments = StringToPrintComments( reader.Value );

      if( reader.MoveToAttribute( PageSetup.UseFirstPageNumber ) )
        pageSetup.AutoFirstPageNumber = !XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( PageSetup.Errors ) )
        pageSetup.PrintErrors = StringToPrintErrors( reader.Value );

      if (reader.MoveToAttribute(PageSetup.HorizontalDpi))
          pageSetup.HResolution = GetParsedXmlValue(reader.Value);         
     
      if( reader.MoveToAttribute( PageSetup.VerticalDpi ) )
        pageSetup.VResolution = GetParsedXmlValue(reader.Value);

      if( reader.MoveToAttribute( PageSetup.Copies ) )
        pageSetup.Copies = XmlConvert.ToInt32( reader.Value );

      PageSetupImpl sheetSetup = pageSetup as PageSetupImpl;

      if( sheetSetup != null && reader.MoveToAttribute( PageSetup.Id ) )
        sheetSetup.RelationId = reader.Value;

      reader.MoveToElement();
      reader.Skip();
    }
   
    /// <summary>
    /// Parse header footer tags.
    /// </summary>
    /// <param name="reader">XmlReader to get all necessary data from.</param>
    /// <param name="pageSetup">Object that would store all extracted settings.</param>
    public static void ParseHeaderFooter( XmlReader reader, PageSetupBaseImpl pageSetup )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( pageSetup == null )
        throw new ArgumentNullException( "pageSetup" );

      if( reader.LocalName != PageSetup.HeaderFooterTag )
        throw new XmlException( "Unexpected xml tag." );
      // Excel version 2010 default value.
      pageSetup.AlignHFWithPageMargins = true;

      if (reader.MoveToAttribute(PageSetup.ScaleWithDocTag))
          pageSetup.HFScaleWithDoc = XmlConvert.ToBoolean(reader.Value);
      if (reader.MoveToAttribute(PageSetup.AlignWithMarginsTag))
          pageSetup.AlignHFWithPageMargins = XmlConvert.ToBoolean(reader.Value);
      if (reader.MoveToAttribute(PageSetup.DifferentFirst))
          pageSetup.DifferentFirstPageHF = XmlConvert.ToBoolean(reader.Value);
      if (reader.MoveToAttribute(PageSetup.DifferentOddEvenTag))
          pageSetup.DifferentOddAndEvenPagesHF = XmlConvert.ToBoolean(reader.Value);

        reader.MoveToElement();
      if( !reader.IsEmptyElement)
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case PageSetup.OddHeaderTag:
                string strHeaderString = reader.ReadElementContentAsString();
                pageSetup.FullHeaderString = strHeaderString;
                break;

              case PageSetup.OddFooterTag:
                string strFooterString = reader.ReadElementContentAsString();
                pageSetup.FullFooterString = strFooterString;
                break;

              default:
                // We don't support other header/footers, so we simply skip them.
                reader.Skip();
                break;
            }
          }
          else
          {
            reader.Skip();
          }
        }
      }

      reader.Read();
    }
    /// <summary>
    /// Converts string into ExcelPrintLocation.
    /// </summary>
    /// <param name="printLocation">Value to convert.</param>
    /// <returns>Converted value.</returns>
    private static ExcelPrintLocation StringToPrintComments( string printLocation )
    {
      ExcelPrintLocation result;

      switch( printLocation )
      {
        case PageSetup.CommentAsDisplayed:
          result = ExcelPrintLocation.PrintInPlace;
          break;

        case PageSetup.CommentNone:
          result = ExcelPrintLocation.PrintNoComments;
          break;

        case PageSetup.CommentAtEnd:
          result = ExcelPrintLocation.PrintSheetEnd;
          break;

        default:
          throw new ArgumentOutOfRangeException( "printLocation" );
      }

      return result;
    }
    /// <summary>
    /// Converts string value to ExcelPrintErrors.
    /// </summary>
    /// <param name="printErrors">Value to convert.</param>
    /// <returns>Converted value.</returns>
    private static ExcelPrintErrors StringToPrintErrors( string printErrors )
    {
      ExcelPrintErrors result;

      switch( printErrors )
      {
        case PageSetup.ErrorsBlank:
          result = ExcelPrintErrors.PrintErrorsBlank;
          break;

        case PageSetup.ErrorsDash:
          result = ExcelPrintErrors.PrintErrorsDash;
          break;

        case PageSetup.ErrorsDisplayed:
          result = ExcelPrintErrors.PrintErrorsDisplayed;
          break;

        case PageSetup.ErrorsNA:
          result = ExcelPrintErrors.PrintErrorsNA;
          break;

        default:
          throw new ArgumentOutOfRangeException( "printLocation" );
      }

      return result;
    }

    /// <summary>
    /// Extracts hyperlinks from reader and inserts them into worksheet.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="sheet">Worksheet to put data into.</param>
    private void ParseHyperlinks( XmlReader reader, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      HyperLinksCollection hyperlinks = sheet.InnerHyperLinks;
      RelationCollection relations = sheet.DataHolder.Relations;

      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.LocalName == Excel2007Serializator.HyperlinkTagName )
        {
          ParseHyperlink( reader, sheet, hyperlinks, relations );
        }

        reader.Skip();
      }

      reader.Skip();
    }
    /// <summary>
    /// Extracts hyperlink from reader and inserts it into worksheet.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="sheet">Worksheet to put data into.</param>
    /// <param name="hyperlinks">Hyperlinks collection to insert link into.</param>
    /// <param name="relations">Relations collection.</param>
    private void ParseHyperlink( XmlReader reader, WorksheetImpl sheet, HyperLinksCollection hyperlinks, RelationCollection relations )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "hyperlinks" );

      HyperLinkImpl hyperlink = new HyperLinkImpl( m_book.Application, hyperlinks );
      string strLocationValue = string.Empty;

      if( reader.MoveToAttribute( Excel2007Serializator.HyperlinkReferenceAttributeName ) )
      {
        TAddr tAddr = GetRangeForDVOrAF( reader.Value );
        IRange range = sheet[ tAddr.FirstRow + 1, tAddr.FirstCol + 1, tAddr.LastRow + 1, tAddr.LastCol + 1 ];
        hyperlink.Range = range;
        range = sheet[ tAddr.FirstRow + 1, tAddr.FirstCol + 1 ];
        hyperlink.TextToDisplay = range.HasFormula ? range.FormulaStringValue : range.Text;
      }

      if( reader.MoveToAttribute( Excel2007Serializator.ToolTipAttributeName ) )
      {
        hyperlink.ScreenTip = reader.Value;
      }

      if( reader.MoveToAttribute( Excel2007Serializator.LocationAttributeName ) )
      {
        strLocationValue = reader.Value;
      }

      if( reader.MoveToAttribute( Excel2007Serializator.RelationshipIdAttributeName, Excel2007Serializator.RelationNamespace ) )
      {
        string strRelationId = reader.Value;
        Relation relation = relations[ strRelationId ];
        string strTarget;
        if (relation.Target.StartsWith("mailto"))
            strTarget = relation.Target;
        else
            strTarget = Uri.UnescapeDataString(relation.Target);

        if( strTarget.StartsWith( Excel2007Serializator.FileHyperlinkStartString ) )
        {
          strTarget = strTarget.Remove( 0, Excel2007Serializator.FileHyperlinkStartString.Length );
        }

        if( strTarget.StartsWith( @"\\" ) )
        {
          hyperlink.Type = ExcelHyperLinkType.Unc;
        }
        else if( ( strTarget.StartsWith( "mailto" ) ) || ( strTarget.IndexOf( @"://" ) != -1 ) )
        {
          hyperlink.Type = ExcelHyperLinkType.Url;
        }
        else
        {
          hyperlink.Type = ExcelHyperLinkType.File;
        }

        hyperlink.SetAddress( strTarget, false );
        //hyperlink.SubAddress = strLocationValue;
        hyperlink.SetSubAddress( strLocationValue );
        relations.Remove( strRelationId );
      }
      else
      {
        hyperlink.Type = ExcelHyperLinkType.Workbook;
        hyperlink.SetAddress( strLocationValue, false );
      }

      if( hyperlink.TextToDisplay == string.Empty && reader.MoveToAttribute( Excel2007Serializator.DisplayStringAttributeName ) )
      {
        hyperlink.TextToDisplay = reader.Value;
      }

      hyperlinks.Add( hyperlink );
      hyperlinks.AddToHash( hyperlink );
    }
    /// <summary>
    /// Extracts sheet-level properties from reader and inserts it into worksheet.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="sheet">Worksheet to put data into.</param>
    private void ParseSheetLevelProperties( XmlReader reader, WorksheetBaseImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( reader.MoveToAttribute( Excel2007Serializator.CodeName ) )
        sheet.CodeName = reader.Value;

      if( reader.MoveToAttribute( Excel2007Serializator.TransitionEvaluation ) )
        sheet.IsTransitionEvaluation = XmlConvert.ToBoolean( reader.Value );

      reader.MoveToElement();

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case Excel2007Serializator.SheetTabColorTagName:
                ParseColor( reader, sheet.TabColorObject );
                break;

              case Excel2007Serializator.SheetOutlinePropertiesTagName:
                ParseOutlineProperites( reader, sheet.PageSetupBase as IPageSetup );
                break;

              case Excel2007Serializator.PageSetupPropertiesTag:
                ParsePageSetupProperties( reader, sheet.PageSetupBase as IPageSetup );
                break;

              default:
                reader.Skip();
                break;
            }
          }
          else
          {
            reader.Skip();
          }
        }
      }

      reader.Read();
    }
    /// <summary>
    /// Parses worksheet page setup properties.
    /// </summary>
    /// <param name="reader">Reader to extract data from.</param>
    /// <param name="pageSetup">Object to put extracted data into.</param>
    private void ParsePageSetupProperties( XmlReader reader, IPageSetup pageSetup )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( pageSetup == null )
        throw new ArgumentNullException( "pageSetup" );

      if( reader.LocalName != Excel2007Serializator.PageSetupPropertiesTag )
        throw new XmlException();

      if( reader.MoveToAttribute( Excel2007Serializator.FitToPageAttribute ) )
        pageSetup.IsFitToPage = XmlConvert.ToBoolean( reader.Value );

      reader.Read();
    }
    /// <summary>
    /// Parses worksheet outline properties.
    /// </summary>
    /// <param name="reader">Reader to get data from.</param>
    /// <param name="pageSetup">Page setup to put data into.</param>
    private void ParseOutlineProperites( XmlReader reader, IPageSetup pageSetup )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( pageSetup == null )
        throw new ArgumentNullException( "pageSetup" );

      if( reader.MoveToAttribute( Excel2007Serializator.SummaryRowBelow ) )
        pageSetup.IsSummaryRowBelow = XmlConvert.ToBoolean( reader.Value );

      if( reader.MoveToAttribute( Excel2007Serializator.SummaryColumnRight ) )
        pageSetup.IsSummaryColumnRight = XmlConvert.ToBoolean( reader.Value );

      reader.MoveToElement();
      reader.Skip();
    }
    /// <summary>
    /// Extracts background image from reader and inserts it into worksheet.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="sheet">Worksheet to put data into.</param>
    /// <param name="strParentPath">Worksheet parent path.</param>
    private void ParseBackgroundImage( XmlReader reader, WorksheetImpl sheet, string strParentPath )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( strParentPath == null )
        throw new ArgumentNullException( "strParentPath" );

#if !SILVERLIGHT && !WINRT && !WP
      if( reader.MoveToAttribute( Excel2007Serializator.RelationshipIdAttributeName, Excel2007Serializator.RelationNamespace ) )
      {
        string strRelation = reader.Value;
        WorksheetDataHolder sheetDataHolder = sheet.DataHolder;
        RelationCollection relations = sheetDataHolder.Relations;
        Relation targetRelation = relations[ strRelation ];
        FileDataHolder fileDataHolder = sheetDataHolder.ParentHolder;
        ZipArchiveItem zipArchiveItem = fileDataHolder[ targetRelation, strParentPath ];
        Stream stream = new MemoryStream();
        MemoryStream dataStream = ( MemoryStream )zipArchiveItem.DataStream;
        dataStream.WriteTo( stream );
        sheet.PageSetup.BackgoundImage = ( Bitmap )Bitmap.FromStream( stream );
        relations.Remove( strRelation );
        fileDataHolder.Archive.RemoveItem( zipArchiveItem.ItemName );
      }
#endif

      reader.Skip();
    }
    /// <summary>
    /// Parses the CustomXmlParts
    /// </summary>
    /// <param name="reader">XmlReader</param>
    /// <param name="schemas">Schema Reference</param>
    public string ParseItemProperties(XmlReader reader,ref List<string> schemas)
    {
        string value = null;

        if (reader == null)
            throw new ArgumentNullException("reader");

        List<string> schemaValues = new List<string>();

        while (reader.NodeType != XmlNodeType.Element)
            reader.Read();
        while (reader.NodeType != XmlNodeType.EndElement && reader.NodeType!=XmlNodeType.None)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.LocalName)
                {
                    case Excel2007Serializator.DataStoreItem:
                        if (reader.MoveToAttribute(Excel2007Serializator.ItemIdAttribute))
                            value = reader.Value;
                        reader.Read();
                        break;

                    case Excel2007Serializator.CustomXmlSchemaReferences:
                        ParseschemaReference(reader,ref schemas);
                        reader.Read();
                        break;
                }
            }
        }
        return value;
    }
    /// <summary>
    /// Parses the CustomXmlParts
    /// </summary>
    /// <param name="reader">XmlReader</param>
    /// <param name="schemas">Schema Reference</param>
    private void ParseschemaReference(XmlReader reader,ref List<string> schemas)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");


        if (reader.LocalName != Excel2007Serializator.CustomXmlSchemaReferences)
            throw new XmlException("Wrong xml tag");

        schemas = new List<string>();

        if (!reader.IsEmptyElement)
        {
            reader.Read();

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                switch (reader.LocalName)
                {
                    case Excel2007Serializator.CustomXmlSchemaReference:
                        ParseSchemaRef(reader,ref schemas);
                        break;

                    default:
                        reader.Skip();
                        break;
                }
            }
        }


    }
    /// <summary>
    /// Parses the Each Schemas and added to Schema collections
    /// </summary>
    /// <param name="reader">XmlReader</param>
    /// <param name="schemas">Schema Reference</param>
    private void ParseSchemaRef(XmlReader reader,ref List<string>schemas)
    {
        string uri;

        if (reader == null)
            throw new ArgumentNullException("reader");

        if (reader.LocalName != Excel2007Serializator.CustomXmlSchemaReference)
            throw new XmlException("Wrong xml tag");

        if (reader.MoveToAttribute(Excel2007Serializator.CustomXmlUriAttribute))
        {
            uri = reader.Value;
            schemas.Add(uri);
        }

        reader.Read();
    }
    /// <summary>
    /// Extracts core properties from reader and inserts it workbook.
    /// </summary>
    /// <param name="reader">XmlReader to extract core properties from.</param>
    public void ParseDocumentCoreProperties( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      while( reader.NodeType != XmlNodeType.Element )
        reader.Read();

      if( reader.LocalName != DocProp.CorePropertiesTagName )
        throw new XmlException( "Unexpected xml tag " + reader.LocalName );

      if( reader.IsEmptyElement )
        return;

      IBuiltInDocumentProperties docProperties = m_book.BuiltInDocumentProperties;

      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case DocProp.CategoryTagName:
              docProperties.Category = GetReaderElementValue( reader );
              break;

            case DocProp.CreatedTagName:
              docProperties.CreationDate = DateTime.Parse( GetReaderElementValue( reader ) );
              break;

            case DocProp.CreatorTagName:
              docProperties.Author = GetReaderElementValue( reader );
              break;

            case DocProp.DescriptionTagName:
              docProperties.Comments = GetReaderElementValue( reader );
              break;

            case DocProp.KeywordsTagName:
              docProperties.Keywords = GetReaderElementValue( reader );
              break;

            case DocProp.LastModifiedByTagName:
              docProperties.LastAuthor = GetReaderElementValue( reader );
              break;

            case DocProp.LastPrintedTagName:
              docProperties.LastPrinted = DateTime.Parse( GetReaderElementValue( reader ) );
              break;

            case DocProp.ModifiedTagName:
              docProperties.LastSaveDate = DateTime.Parse( GetReaderElementValue( reader ) );
              break;

            case DocProp.SubjectTagName:
              docProperties.Subject = GetReaderElementValue( reader );
              break;

            case DocProp.TitleTagName:
              docProperties.Title = GetReaderElementValue( reader );
              break;

            default:
              reader.Skip();
              break;
          }
        }
        else
        {
          reader.Skip();
        }
      }
    }
    /// <summary>
    /// Extracts extended properties from reader and inserts it workbook.
    /// </summary>
    /// <param name="reader">XmlReader to extract extended properties from.</param>
    public void ParseExtendedProperties( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      while( reader.NodeType != XmlNodeType.Element )
        reader.Read();

      if( reader.LocalName != DocProp.ApplicationSpecificFilePropertiesTagName )
        throw new XmlException( "Unexpected xml tag " + reader.LocalName );

      if( reader.IsEmptyElement )
        return;

      IBuiltInDocumentProperties docProperties = m_book.BuiltInDocumentProperties;

      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          switch( reader.LocalName )
          {
            case DocProp.ApplicationNameTagName:
              docProperties.ApplicationName = GetReaderElementValue( reader );
              break;

            case DocProp.TotalNumberOfCharacters:
              docProperties.CharCount = XmlConvert.ToInt32( GetReaderElementValue( reader ) );
              break;

            case DocProp.NameOfCompanyTagName:
              docProperties.Company = GetReaderElementValue( reader );
              break;

            case DocProp.NumberOfLinesTagName:
              docProperties.LineCount = XmlConvert.ToInt32( GetReaderElementValue( reader ) );
              break;

            case DocProp.HeadingPairsTagName:
              ((BuiltInDocumentProperties)docProperties).HasHeadingPair = true;
              reader.Skip();
              break;

            case DocProp.NameOfManagerTagName:
              docProperties.Manager = GetReaderElementValue( reader );
              break;

            case DocProp.TotalNumberOfMultimediaClipsTagName:
              docProperties.MultimediaClipCount = XmlConvert.ToInt32( GetReaderElementValue( reader ) );
              break;

            case DocProp.NumberOfSlidesContainingNotesTagName:
              docProperties.SlideCount = XmlConvert.ToInt32( GetReaderElementValue( reader ) );
              break;

            case DocProp.TotalNumberOfPagesTagName:
              docProperties.PageCount = XmlConvert.ToInt32( GetReaderElementValue( reader ) );
              break;

            case DocProp.TotalNumberOfParagraphsTagName:
              docProperties.ParagraphCount = XmlConvert.ToInt32( GetReaderElementValue( reader ) );
              break;

            case DocProp.IntendedFormatOfPresentationTagName:
              docProperties.PresentationTarget = GetReaderElementValue( reader );
              break;

            case DocProp.NameOfDocumentTemplateTagName:
              docProperties.Template = GetReaderElementValue( reader );
              break;

            case DocProp.TotalEditTimeMetadataElementTagName:
              docProperties.EditTime = TimeSpan.FromMinutes( XmlConvert.ToInt32( GetReaderElementValue( reader ) ) );
              break;

            case DocProp.WordCountTagName:
              docProperties.WordCount = XmlConvert.ToInt32( GetReaderElementValue( reader ) );
              break;

            case DocProp.RelativeHyperlinkBaseTagName:
              CustomDocumentProperties customProperties = ( CustomDocumentProperties )m_book.CustomDocumentProperties;
              DocumentPropertyImpl customProperty =
                ( DocumentPropertyImpl )customProperties.Add( DocProp.RelativeHyperlinkExcel97Name );
              customProperty.Blob = Encoding.Unicode.GetBytes( GetReaderElementValue( reader ) + "\0" );
              break;

            case DocProp.AppVersion:
              double versionValue = reader.ReadElementContentAsDouble();

              if (versionValue > 15)
              {
                m_book.Version = ExcelVersion.Excel2013;
              }
              else if (versionValue > 14)
              {
                  m_book.Version = ExcelVersion.Excel2010;
              }
              break;

            default:
              reader.Skip();
              break;
          }
        }
        else
        {
          reader.Skip();
        }
      }
      if (!((BuiltInDocumentProperties)docProperties).HasHeadingPair)
            docProperties.ApplicationName = DocProp.EssentialXlsIO;
    }
    /// <summary>
    /// Extracts custom properties from reader and inserts it workbook.
    /// </summary>
    /// <param name="reader">XmlReader to extract custom properties from.</param>
    public void ParseCustomProperties( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      while( reader.NodeType != XmlNodeType.Element )
        reader.Read();

      if( reader.LocalName != DocProp.CustomFilePropertiesTagName )
        throw new XmlException( "Unexpected xml tag " + reader.LocalName );

      if( reader.IsEmptyElement )
        return;

      CustomDocumentProperties customProperties = ( CustomDocumentProperties )m_book.CustomDocumentProperties;

      reader.Read();

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.NodeType == XmlNodeType.Element )
        {
          if( reader.LocalName == DocProp.CustomFilePropertyTagName )
          {
            ParseCustomProperty( reader, customProperties );
          }
        }
        else
        {
          reader.Skip();
        }
      }
    }
    /// <summary>
    /// Extracts custom property from reader and inserts it into custom property implementation.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="customProperties">Custom property.</param>
    public void ParseCustomProperty( XmlReader reader, CustomDocumentProperties customProperties )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( customProperties == null )
        throw new ArgumentNullException( "customProperties" );

      if( reader.LocalName != DocProp.CustomFilePropertyTagName )
        throw new XmlException( "Unexpected xml tag " + reader.LocalName );

      DocumentPropertyImpl customProperty = null;

      if( reader.MoveToAttribute( DocProp.NameAttributeName ) )
      {
        customProperty = ( DocumentPropertyImpl )customProperties.Add( reader.Value );
      }

      reader.MoveToElement();

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case DocProp.LPWSTRVariantType:
                customProperty.PropertyType = PropertyType.String;
                customProperty.Text = GetReaderElementValue( reader );
                break;

              case DocProp.LPSTRVariantType:
                customProperty.PropertyType = PropertyType.AsciiString;
                customProperty.Text = GetReaderElementValue( reader );
                break;

              case DocProp.FileTimeVariantType:
                customProperty.PropertyType = PropertyType.DateTime;
                customProperty.DateTime = DateTime.Parse( GetReaderElementValue( reader ) );
                break;

              case DocProp.EightByteRealNumberVariantType:
                customProperty.PropertyType = PropertyType.Double;
                customProperty.Double = XmlConvert.ToDouble( GetReaderElementValue( reader ) );
                break;

              case DocProp.FourByteSignedIntegerVariantType:
                customProperty.PropertyType = PropertyType.Int32;
                customProperty.Int32 = XmlConvert.ToInt32( GetReaderElementValue( reader ) );
                break;

              case DocProp.IntegerVariantType:
                customProperty.PropertyType = PropertyType.Int;
                customProperty.Integer = XmlConvert.ToInt32( GetReaderElementValue( reader ) );
                break;

              case DocProp.BooleanVarianYype:
                customProperty.PropertyType = PropertyType.Bool;
                customProperty.Boolean = bool.Parse( GetReaderElementValue( reader ) );
                break;

              default:
                reader.Skip();
                break;
            }
          }
          else
          {
            reader.Read();
          }
        }
      }

      reader.Read();
    }
    /// <summary>
    /// Parses external link file. To set URL field we have to have access to
    /// relations, so it is better to do it in some other place.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="relations">Item's relations.</param>
    internal bool ParseExternalLink( XmlReader reader, RelationCollection relations )
    {
      bool removeRelation = true;
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      while( reader.NodeType != XmlNodeType.Element )
        reader.Read();

      if( reader.LocalName != ExternalLinks.ExternalLinkTag )
        throw new XmlException( "Unexpected xml tag." );

      reader.Read();

      switch( reader.LocalName )
      {
        case ExternalLinks.ExternalBookTag:
          ParseExternalWorkbook( reader, relations );
          break;

        case ExternalLinks.OleLink:
          ParseOleObjectLink( reader, relations );
          break;

        case ExternalLinks.DdeLink:
          reader.Skip();
          removeRelation = false;
          break;

        default:
          throw new XmlException( "Unsupported xml tag" );
      }
      return removeRelation;
    }
    /// <summary>
    /// Parses the OLE object link.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="relations">The relations.</param>
    private void ParseOleObjectLink( XmlReader reader, RelationCollection relations )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.LocalName != ExternalLinks.OleLink )
        throw new XmlException( "Unexpected xml tag." );

      string strUrlId = null;


      if( reader.MoveToAttribute( Excel2007Serializator.RelationAttribute, Excel2007Serializator.RelationNamespace ) )
        strUrlId = reader.Value;

      ExternWorkbookImpl result = CreateExternBook( relations, strUrlId, null );
      int iNameIndex = result.ExternNames.Add( "'" );

      if( reader.MoveToAttribute( Vml.ProgramID ) )
        result.ProgramId = reader.Value;

      ExternNameRecord name = result.ExternNames[ iNameIndex ].Record;
      name.OleLink = true;
      name.Ole = false;
      name.WantPicture = true;
      name.WantAdvise = true;
      name.BuiltIn = false;

      Relation relation = relations[ strUrlId ];
      string strTarget = Uri.UnescapeDataString( relation.Target );

      if( strTarget.StartsWith( Excel2007Serializator.FileHyperlinkStartString ) )
        strTarget = strTarget.Substring( Excel2007Serializator.FileHyperlinkStartString.Length );

      result.URL = strTarget;

      //m_book.ExternWorkbooks.Add( result );

      //if( !reader.IsEmptyElement )
      {
        reader.Skip();
        //reader.Read();

        //while( reader.NodeType != XmlNodeType.EndElement )
        //{
        //  if( reader.NodeType == XmlNodeType.Element )
        //  {
        //    switch( reader.LocalName )
        //    {
        //      case ExternalLinks.OleItems:
        //      default:
        //        reader.Skip();
        //        break;
        //    }
        //  }
        //  else
        //  {
        //    reader.Read();
        //  }
        //}
      }

      //reader.Read();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="relations"></param>
    private void ParseExternalWorkbook( XmlReader reader, RelationCollection relations )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.LocalName != ExternalLinks.ExternalBookTag )
        throw new XmlException( "Unexpected xml tag." );

      string strUrlId = null;

      if( reader.MoveToAttribute( Excel2007Serializator.RelationAttribute, Excel2007Serializator.RelationNamespace ) )
        strUrlId = reader.Value;

      List<string> arrSheetNames = null;
      ExternWorkbookImpl externBook = null;

      if( !reader.IsEmptyElement )
      {
        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case ExternalLinks.SheetNamesTag:
                arrSheetNames = ParseSheetNames( reader );
                externBook = CreateExternBook( relations, strUrlId, arrSheetNames );
                break;

              case ExternalLinks.SheetDataSetTag:
                ParseSheetDataSet( reader, externBook );
                break;

              case ExternalLinks.DefinedNamesTag:
                ParseExternalDefinedNames( reader, externBook );
                break;

              default:
                throw new XmlException( "Unexpected xml tag." );
            }
          }
          else
          {
            reader.Read();
          }
        }
      }

      reader.Read();
    }
    /// <summary>
    /// Extracts external names from the reader.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="externBook">External workbooks to put defined names into.</param>
    private void ParseExternalDefinedNames( XmlReader reader, ExternWorkbookImpl externBook )
    {
      if( reader.LocalName != ExternalLinks.DefinedNamesTag )
        throw new XmlException();

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case ExternalLinks.DefinedNameTag:
                ParseExternalName( reader, externBook );
                break;

              default:
                reader.Skip();
                break;
            }
          }
          else
          {
            reader.Skip();
          }
        }
      }

      reader.Read();
    }
    /// <summary>
    /// Extracts single external name from the reader.
    /// </summary>
    /// <param name="reader">XmlReader to get external name data from.</param>
    /// <param name="externBook">External workbook to put extracted name into.</param>
    private void ParseExternalName( XmlReader reader, ExternWorkbookImpl externBook )
    {
      if( reader.LocalName != ExternalLinks.DefinedNameTag )
        throw new XmlException();

      string name = null;
      string refersTo = null;

      if( reader.MoveToAttribute( ExternalLinks.NameAttribute ) )
        name = reader.Value;

      if (reader.MoveToAttribute(ExternalLinks.RefersToAttribute))
          refersTo = reader.Value;

      int index = externBook.ExternNames.Add( name );
      externBook.ExternNames[index].RefersTo = refersTo;
      if (reader.MoveToAttribute(ExternalLinks.SheetIdAttribute))
          externBook.ExternNames[index].sheetId = Convert.ToInt32(reader.Value);
    }
    /// <summary>
    /// Extracts cached external data.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <param name="externBook">External workbook to put cache into.</param>
    private void ParseSheetDataSet( XmlReader reader, ExternWorkbookImpl externBook )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( externBook == null )
        throw new ArgumentNullException( "externBook" );

      if( reader.LocalName != ExternalLinks.SheetDataSetTag )
        throw new XmlException();

      if( !reader.IsEmptyElement )
      {
        reader.Read();

        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element )
          {
            switch( reader.LocalName )
            {
              case Excel2007Serializator.SheetDataTagName:
                ParseExternalSheetData( reader, externBook );
                break;

              default:
                reader.Skip();
                break;
            }
          }
          else
          {
            reader.Skip();
          }
        }
      }

      reader.Skip();
    }
    /// <summary>
    /// Extracts single external worksheet cached data from XmlReader.
    /// </summary>
    /// <param name="reader">XmlReader to get cached data from.</param>
    /// <param name="externBook">External workbook to place extracted data into.</param>
    private void ParseExternalSheetData( XmlReader reader, ExternWorkbookImpl externBook )
    {
      if( reader == null )
        throw new ArgumentException( "reader" );

      if( externBook == null )
        throw new ArgumentNullException( "externBook" );

      if( reader.LocalName != Excel2007Serializator.SheetDataTagName )
        throw new XmlException();

      if( !reader.MoveToAttribute( ExternalLinks.SheetIdAttribute ) )
        throw new XmlException();

      int iSheetId = XmlConvert.ToInt32( reader.Value );
      ExternWorksheetImpl sheet = externBook.Worksheets[ iSheetId ];
      reader.MoveToElement();

      sheet.AdditionalAttributes = ParseSheetData( reader, sheet, null, ExternalLinks.CellTag );
    }
    /// <summary>
    /// Creates external workbook.
    /// </summary>
    /// <param name="relations">Relations collection that helps to locate external workbook.</param>
    /// <param name="strUrlId">Relation id of the target workbook.</param>
    /// <param name="arrSheetNames">Name of the workbook's worksheets.</param>
    /// <returns></returns>
    private ExternWorkbookImpl CreateExternBook( RelationCollection relations, string strUrlId,
      List<string> arrSheetNames )
    {
      Relation destinationRelation = relations[ strUrlId ];
      string strTarget = destinationRelation.Target;

      if( strTarget.StartsWith( Excel2007Serializator.FileHyperlinkStartString ) )
      {
        strTarget = strTarget.Substring( Excel2007Serializator.FileHyperlinkStartString.Length );
      }

      strTarget = Uri.UnescapeDataString( strTarget );
      string strFileName = Path.GetFileName( strTarget );
      string strFilePath = strTarget.Substring( 0, strTarget.Length - strFileName.Length );

      int index = m_book.ExternWorkbooks.Add( strFilePath, strFileName, arrSheetNames, null );
      return m_book.ExternWorkbooks[ index ];
    }
    /// <summary>
    /// Extract names of external worksheets from the reader.
    /// </summary>
    /// <param name="reader">XmlReader to get data from.</param>
    /// <returns>List with extracted names.</returns>
    private List<string> ParseSheetNames( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.LocalName != ExternalLinks.SheetNamesTag )
        throw new XmlException( "Unexpected xml tag" );

      List<string> arrSheetNames = new List<string>();

      if( !reader.IsEmptyElement )
      {
        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element &&
            reader.LocalName == ExternalLinks.SheetNameTag )
          {
            if( !reader.MoveToAttribute( ExternalLinks.ExternalSheetNameAttribute ) )
              throw new XmlException();

            string strSheetName = reader.Value;
            arrSheetNames.Add( strSheetName );
          }

          reader.Read();
        }
      }

      reader.Read();
      return arrSheetNames;
    }
    /// <summary>
    /// Extracts horizontal pagebreaks from reader.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="sheet">Worksheet to insert pagebreaks in.</param>
    private void ParseHorizontalPagebreaks( XmlReader reader, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

        if (!reader.IsEmptyElement)
        {
            reader.Read();

      HPageBreaksCollection hPagebreaks = ( HPageBreaksCollection )sheet.HPageBreaks;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.LocalName == Excel2007Serializator.BreakTagName )
        {
          int iRow = ( reader.MoveToAttribute( Excel2007Serializator.IdAttributeName ) )
            ? XmlConvert.ToInt32( reader.Value ) : 0;

          int iStartColumn = ( reader.MoveToAttribute( Excel2007Serializator.MinimumAttributeName ) )
            ? XmlConvert.ToUInt16( reader.Value ) : ( ushort )0;

          int iEndColumn = ( reader.MoveToAttribute( Excel2007Serializator.MaximumAttributeName ) )
            ? XmlConvert.ToUInt16( reader.Value ) : ( ushort )0;
          if (iEndColumn > sheet.Workbook.MaxColumnCount-1)
              iEndColumn = m_book.MaxColumnCount-1;
          HorizontalPageBreaksRecord.THPageBreak pageBreak = new HorizontalPageBreaksRecord.THPageBreak(
            ( ushort )iRow, ( ushort )iStartColumn, ( ushort )iEndColumn );

          HPageBreakImpl hPagebreak = new HPageBreakImpl( sheet.Application, sheet, pageBreak );

          if( reader.MoveToAttribute( Excel2007Serializator.ManualPageBreakAttributeName ) )
            hPagebreak.Type = ExcelPageBreak.PageBreakManual;

          hPagebreaks.Add( hPagebreak );
        }

        reader.Skip();
      }

      reader.Skip();
        }
        else
            reader.Skip();
    }
    /// <summary>
    /// Extracts vertical pagebreaks from reader.
    /// </summary>
    /// <param name="reader">XmlReader to extract data from.</param>
    /// <param name="sheet">Worksheet to insert pagebreaks in.</param>
    private void ParseVerticalPagebreaks( XmlReader reader, WorksheetImpl sheet )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      reader.Read();

      VPageBreaksCollection vPagebreaks = ( VPageBreaksCollection )sheet.VPageBreaks;

      while( reader.NodeType != XmlNodeType.EndElement )
      {
        if( reader.LocalName == Excel2007Serializator.BreakTagName )
        {
          int iColumn = ( reader.MoveToAttribute( Excel2007Serializator.IdAttributeName ) )
            ? XmlConvert.ToInt32( reader.Value ) : 0;

          int iStartRow = ( reader.MoveToAttribute( Excel2007Serializator.MinimumAttributeName ) )
            ? XmlConvert.ToInt32( reader.Value ) : 0;

          int iEndRow = ( reader.MoveToAttribute( Excel2007Serializator.MaximumAttributeName ) )
            ? XmlConvert.ToInt32( reader.Value ) : 0;

          VerticalPageBreaksRecord.TVPageBreak tVPagebreak = new VerticalPageBreaksRecord.TVPageBreak(
            ( ushort )iColumn, ( ushort )iStartRow, ( ushort )iEndRow );

          VPageBreakImpl vPagebreak = new VPageBreakImpl( sheet.Application, sheet, tVPagebreak );

          if( reader.MoveToAttribute( Excel2007Serializator.ManualPageBreakAttributeName ) )
            vPagebreak.Type = ExcelPageBreak.PageBreakManual;

          vPagebreaks.Add( vPagebreak );
        }

        reader.Skip();
      }

      reader.Skip();
    }
    /// <summary>
    /// Parses workbook part of the external links description.
    /// </summary>
    /// <param name="reader">Reader to get data from.</param>
    private void ParseExternalLinksWorkbookPart( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.LocalName != ExternalLinks.ExternalReferencesTag )
        throw new XmlException( "Unexpected xml tag." );

      bool bEmpty = reader.IsEmptyElement;
      reader.Read();

      if( !bEmpty )
      {
        while( reader.NodeType != XmlNodeType.EndElement )
        {
          if( reader.NodeType == XmlNodeType.Element && reader.LocalName == ExternalLinks.ExternalReferenceTag )
          {
            ParseExternalLinkWorkbookPart( reader );
          }
          else
          {
            reader.Read();
          }
        }

        reader.Read();
      }
    }
    /// <summary>
    /// Parses single external link from the workbook.
    /// </summary>
    /// <param name="reader">Reader to get link information from.</param>
    private void ParseExternalLinkWorkbookPart( XmlReader reader )
    {
      if( reader == null )
        throw new ArgumentNullException( "reader" );

      if( reader.LocalName != ExternalLinks.ExternalReferenceTag )
        throw new XmlException( "Unexpected xml tag." );

      if( !reader.MoveToAttribute( Excel2007Serializator.RelationAttribute, Excel2007Serializator.RelationNamespace ) )
        throw new XmlException();

      string strRelationId = reader.Value;
      m_book.DataHolder.ParseExternalLink( strRelationId );
    }
    /// <summary>
    /// Parse external connection
    /// </summary>
    /// <remarks></remarks>
    public void ParseConnections(XmlReader reader)
    {
        if( reader == null )
        throw new ArgumentNullException( "reader" );        

      if( reader.LocalName != Excel2007Serializator.ConnectionsTag )
        throw new XmlException( "Unexpected xml tag " + reader.LocalName );

      if (reader.NodeType != XmlNodeType.Element)
          throw new ArgumentException("Element is null");

      reader.Read();
        
      while (reader.NodeType != XmlNodeType.EndElement && reader.NodeType!=XmlNodeType.None)
      {
          if (reader.NodeType == XmlNodeType.Element)
          {
              switch (reader.LocalName)
              {
                  case Excel2007Serializator.ConnectionTag:
                      ParseConnection(reader);
                      break;
                  default:
                      reader.Skip();
                      break;
                      
              }
          }
      }
    }
      /// <summary>
    /// Parse external connection
    /// </summary>
    /// <remarks></remarks>
    public void ParseConnection(XmlReader reader)
    {
        ExternalConnectionCollection connections;
        ExternalConnection Connection = null;
        DataBaseProperty database = null;
        bool IsDeleted = false;
        ExcelConnectionsType connectionType=ExcelConnectionsType.ConnectionTypeODBC;
        
        if (reader == null)
            throw new ArgumentNullException("reader");
        
        if (reader.NodeType == XmlNodeType.Element)
        {
            if (reader.LocalName != Excel2007Serializator.ConnectionTag)
                throw new ArgumentException("Invalid XML");

            if (reader.LocalName != Excel2007Serializator.ConnectionTag)
                throw new XmlException("Unexpected xml tag " + reader.LocalName);
            if (reader.MoveToAttribute(Excel2007Serializator.DataBaseTypeAttribute))
            {
#if (!(SILVERLIGHT))
                connectionType = (ExcelConnectionsType)Enum.Parse(typeof(ExcelConnectionsType), reader.Value);
#else
                connectionType = (ExcelConnectionsType)Enum.Parse(typeof(ExcelConnectionsType), reader.Value, true);
#endif
            }
            else
                throw new ArgumentException("DataBase Type is Missing");

            if (reader.MoveToAttribute(Excel2007Serializator.Deleted))
                IsDeleted = ParseBoolean(reader, Excel2007Serializator.Deleted, false);

            if (IsDeleted)
            {
                connections = m_book.DeletedConnections as ExternalConnectionCollection;
                Connection = connections.Add(connectionType) as ExternalConnection;
                Connection.Deleted = IsDeleted;
            }
            else
            {
                connections = m_book.Connections as ExternalConnectionCollection;
                Connection = connections.Add(connectionType) as ExternalConnection;
            }            
            
           
            if (Connection.DataBaseType == ExcelConnectionsType.ConnectionTypeOLEDB)
                database = Connection.OLEDBConnection as DataBaseProperty;
            else if (Connection.DataBaseType == ExcelConnectionsType.ConnectionTypeODBC)
            {
                database = Connection.ODBCConnection;
                database.CommandType = ExcelCommandType.Sql;
            }
            if (reader.MoveToAttribute(Excel2007Serializator.ConnectionIdAttribute))
                Connection.ConncetionId = (uint)reader.ReadContentAsInt();
            if (reader.MoveToAttribute(Excel2007Serializator.SourceFile))
                Connection.SourceFile = reader.Value;
            if (reader.MoveToAttribute(Excel2007Serializator.OdbcFileAttribute))
                Connection.ConnectionFile = reader.Value;
            if (reader.MoveToAttribute(Excel2007Serializator.NameAttributeName))
                Connection.Name = reader.Value;
            if (reader.MoveToAttribute(Excel2007Serializator.DescriptionTag))
                Connection.Description = reader.Value;
            if (reader.MoveToAttribute(Excel2007Serializator.RefreshedVersionAttribute))
                Connection.RefershedVersion = (uint)reader.ReadContentAsInt();
            if (database != null)
            {
                if (reader.MoveToAttribute(Excel2007Serializator.SavePassword))
                    database.SavePassword = (bool)reader.ReadContentAsBoolean();
                if (reader.MoveToAttribute(Excel2007Serializator.OnlyUseConnectionFile))
                    database.AlwaysUseConnectionFile = (bool)reader.ReadContentAsBoolean();
                if (reader.MoveToAttribute(Excel2007Serializator.Interval))
                    database.RefreshPeriod = reader.ReadContentAsInt();
                if (reader.MoveToAttribute(Excel2007Serializator.Credentials))
                {
#if (!(SILVERLIGHT))
                    database.ServerCredentialsMethod = (ExcelCredentialsMethod)Enum.Parse(typeof(ExcelCredentialsMethod), reader.Value);
#else
                    database.ServerCredentialsMethod = (ExcelCredentialsMethod)Enum.Parse(typeof(ExcelCredentialsMethod), reader.Value, true);
#endif
                }
                if (reader.MoveToAttribute(ListObjects.RefreshOnLoad))
                    database.RefreshOnFileOpen = (bool)reader.ReadContentAsBoolean();
            }
                
            reader.Read();
        }
        while (reader.NodeType != XmlNodeType.EndElement && reader.NodeType!=XmlNodeType.None)
        {
            
            switch (reader.LocalName)
            {
                    
                case Excel2007Serializator.DataBasePrTag:                    
                    ParseDataBaseProperty(reader, database);
                    if (Connection.DataBaseType == ExcelConnectionsType.ConnectionTypeOLEDB)
                        Connection.DBConnectionString = checkconnection((string)database.ConnectionString);
                    else
                        Connection.DBConnectionString = (string)database.ConnectionString;
                    break;
                case Excel2007Serializator.WebPrTag:
                    ParseWebProperties(reader, Connection);
                    reader.Skip();
                    break;
                case Excel2007Serializator.OlapPrTag:
                    Connection.OlapProperty = ShapeParser.ReadNodeAsStream(reader,false);
                    break;
                case Excel2007Serializator.Extensionlist:
                    Connection.ExtLstProperty = ShapeParser.ReadNodeAsStream(reader);
                    break;
                case Excel2007Serializator.TextPr:
                    Connection.m_textPr = ShapeParser.ReadNodeAsStream(reader);
                    break;
                default:
                    reader.Skip();
                    break;
            }
        }
        reader.Read();
    }    
    #endregion
    public void ParseDataBaseProperty(XmlReader reader,DataBaseProperty DataBase)
    {
        if(reader==null)
            throw new ArgumentNullException("reader");
        if (reader.LocalName != Excel2007Serializator.DataBasePrTag)
            throw new ArgumentException("Tag is not proper");
       
            if (reader.NodeType == XmlNodeType.Element)
            {                
                    if (reader.MoveToAttribute(Excel2007Serializator.ConnectionTag))
                    {
                        DataBase.ConnectionString = reader.Value;
                    }
                    if (reader.MoveToAttribute(Excel2007Serializator.CommandTextAttribute))
                        DataBase.CommandText = reader.Value;
                    if (reader.MoveToAttribute(Excel2007Serializator.CommandTypeAttribute))
                    {
#if (!(SILVERLIGHT))
                        DataBase.CommandType = (ExcelCommandType)Enum.Parse(typeof(ExcelCommandType), reader.Value);
#else
                        DataBase.CommandType = (ExcelCommandType)Enum.Parse(typeof(ExcelCommandType), reader.Value, true);
#endif
                    }
                
            }
            //connection.OLEDBConnection.SourceDataFile.ToString();    
            reader.Skip();
       
    }

    public void ParseWebProperties(XmlReader reader, ExternalConnection Connection)
    {
        if (reader == null)
            throw new ArgumentNullException("reader");
        if (reader.LocalName != Excel2007Serializator.WebPrTag)
            throw new ArgumentException("Tag is not proper");
        if (reader.MoveToAttribute(Excel2007Serializator.Xml))
            Connection.IsXml = reader.ReadContentAsBoolean();
        if (reader.MoveToAttribute(Excel2007Serializator.URL))
            Connection.ConnectionURL = reader.Value;
        else
            throw new ArgumentException("The connection URL is missing");
    }
    private string checkconnection(string connection)
    {
        string change = connection.ToLower();
        string achange = change;
        string con = connection;
        if (change.Contains("provider"))
        {
            string jetOledb = "Provider=Microsoft.JET.OLEDB.4.0";
            int start = change.IndexOf("provider");
            int end = change.IndexOf(";", start);
            end.ToString();
            string provider = connection.Substring(start, end - start);
            change = change.Substring(start, end - start);
            if (provider != null && provider != "" && change.Contains("ace"))
            {
                connection = connection.Replace(provider, jetOledb);
                change = connection.ToLower();
                start = change.IndexOf(";", change.IndexOf("data source"));
                if (start > 0)
                    connection = connection.Remove(start);
            }
        }
        return connection;
    }

    #region Helper methods
    /// <summary>
    /// Creates cell record.
    /// </summary>
    /// <param name="type">Cell type.</param>
    /// <param name="strValue">Record value.</param>
    /// <param name="cells">Current cells collection.</param>
    /// <param name="iColumn">Represents column index.</param>
    /// <param name="iRow">Represents row index.</param>
    /// <param name="iXFIndex">Represents extended format index.</param>
    private void SetCellRecord( Excel2007Serializator.CellType type, string strValue, CellRecordCollection cells,
      int iRow, int iColumn, int iXFIndex )
    {
      if( strValue == null || strValue.Length == 0 )
        return;

      if( cells == null )
        throw new ArgumentNullException( "cells" );

      switch( type )
      {
        case Excel2007Serializator.CellType.n:
          cells.SetNumberValue( iRow, iColumn, XmlConvert.ToDouble( strValue ), iXFIndex );
          break;

        case Excel2007Serializator.CellType.b:
          cells.SetBooleanValue( iRow, iColumn, XmlConvert.ToBoolean( strValue ), iXFIndex );
          break;

        case Excel2007Serializator.CellType.e:
          cells.SetErrorValue( iRow, iColumn, strValue, iXFIndex );
          break;

        case Excel2007Serializator.CellType.s:
        case Excel2007Serializator.CellType.inlineStr:
          cells.SetSingleStringValue( iRow, iColumn, iXFIndex, XmlConvert.ToInt32( strValue ) );
          break;

        case Excel2007Serializator.CellType.str:
          cells.SetNonSSTString( iRow, iColumn, iXFIndex, strValue );
          break;

        default:
          break;
      }
    }
    /// <summary>
    /// Sets formula value.
    /// </summary>
    /// <param name="sheet">Worksheet to set formula value into.</param>
    /// <param name="cellType">Cell type.</param>
    /// <param name="strValue">Value to set.</param>
    /// <param name="iRowIndex">Represents row index.</param>
    /// <param name="iColumnIndex">Represents column index.</param>
    private void SetFormulaValue( IInternalWorksheet sheet, Excel2007Serializator.CellType cellType,
      string strValue, int iRowIndex, int iColumnIndex )
    {
      if( strValue == null )
        throw new NullReferenceException( "strValue" );

      switch( cellType )
      {
        case Excel2007Serializator.CellType.b:
          sheet.SetFormulaBoolValue( iRowIndex, iColumnIndex,
            XmlConvert.ToBoolean( strValue ) );
          break;

        case Excel2007Serializator.CellType.e:
          sheet.SetFormulaErrorValue( iRowIndex, iColumnIndex, strValue );
          break;

        case Excel2007Serializator.CellType.n:
          sheet.SetFormulaNumberValue( iRowIndex, iColumnIndex,
            XmlConvert.ToDouble( strValue ) );
          break;

        case Excel2007Serializator.CellType.str:
          sheet.SetFormulaStringValue( iRowIndex, iColumnIndex, strValue );
          break;

        default:
          break;
      }
    }
    /// <summary>
    /// Sets array formula record into worksheet.
    /// </summary>
    /// <param name="sheet">Worksheet to put data into.</param>
    /// <param name="strFormulaString">Formula string to insert.</param>
    /// <param name="strCellsRange">Cell range of the formula.</param>
    /// <param name="iXFIndex">Extended format index.</param>
    private void SetArrayFormula( WorksheetImpl sheet, string strFormulaString, string strCellsRange, int iXFIndex )
    {
      if( sheet == null )
        throw new ArgumentNullException( "sheet" );

      if( strFormulaString == null )
        throw new ArgumentNullException( "strFormulaString" );

      if( strCellsRange == null )
        throw new ArgumentNullException( "strCellRange" );

      ArrayRecord arrayRecord = ( ArrayRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Array );
      strFormulaString = UtilityMethods.RemoveFirstCharUnsafe( strFormulaString );
      arrayRecord.Formula = m_formulaUtil.ParseString( strFormulaString, sheet, null );

      string strFirstRow = null;
      string strFirstColumn = null;
      string strLastRow = null;
      string strLastColumn = null;

      if( m_formulaUtil.IsCellRange( strCellsRange, false, out strFirstRow, out strFirstColumn,
        out strLastRow, out strLastColumn ) )
      {
        arrayRecord.FirstRow = Convert.ToInt32( strFirstRow ) - 1;
        arrayRecord.FirstColumn = RangeImpl.GetColumnIndex( strFirstColumn ) - 1;
        arrayRecord.LastRow = Convert.ToInt32( strLastRow ) - 1;
        arrayRecord.LastColumn = RangeImpl.GetColumnIndex( strLastColumn ) - 1;
      }
      else
      {
        int iRowIndex = 0;
        int iColIndex = 0;
        RangeImpl.CellNameToRowColumn( strCellsRange, out iRowIndex, out iColIndex );
        arrayRecord.FirstRow = iRowIndex - 1;
        arrayRecord.FirstColumn = iColIndex - 1;
        arrayRecord.LastRow = iRowIndex - 1;
        arrayRecord.LastColumn = iColIndex - 1;
      }

      RangeImpl range = ( RangeImpl )sheet.Range[ strCellsRange ];
      range.SetFormulaArrayRecord( arrayRecord, iXFIndex );
    }
    /// <summary>
    /// Sets shared formula record into worksheet.
    /// </summary>
    /// <param name="sheet">Worksheet to put data into.</param>
    /// <param name="strFormulaString">Formula string to insert.</param>
    /// <param name="strCellsRange">Cell range of the formula.</param>
    /// <param name="uiSharedGroupIndex">Shared formula group index.</param>
    /// <param name="iRow">Current row index.</param>
    /// <param name="iCol">Current column index.</param>
    /// <param name="iXFIndex">Extended format index.</param>
    private void SetSharedFormula( WorksheetImpl sheet, string strFormulaString, string strCellsRange,
      uint uiSharedGroupIndex, int iRow, int iCol, int iXFIndex, bool bCalculateOnOpen )
    {
      CellRecordCollection cells = sheet.CellRecords;

      if( strCellsRange != null && strFormulaString != null )
      {
        string strFirstRow = null;
        string strFirstColumn = null;
        string strLastRow = null;
        string strLastColumn = null;

        SharedFormulaRecord recordSharedFormula = ( SharedFormulaRecord )BiffRecordFactory.
          GetRecord( TBIFFRecord.SharedFormula2 );
        bool bRange = false;

        if( ( bRange = m_formulaUtil.IsCellRange( strCellsRange, false, out strFirstRow, out strFirstColumn,
          out strLastRow, out strLastColumn ) ) )
        {
        }
        else if( ( bRange = FormulaUtil.IsCell( strCellsRange, false, out strFirstRow, out strFirstColumn ) ) )
        {
          strLastRow = strFirstRow;
          strLastColumn = strFirstColumn;
        }

        if( bRange )
        {
          int iFirstRow = Convert.ToInt32( strFirstRow );
          int iFirstColumn = RangeImpl.GetColumnIndex( strFirstColumn );
          recordSharedFormula.FirstRow = iFirstRow;
          recordSharedFormula.FirstColumn = iFirstColumn;
          recordSharedFormula.LastRow = Convert.ToInt32( strLastRow );
          recordSharedFormula.LastColumn = RangeImpl.GetColumnIndex( strLastColumn );
          strFormulaString = UtilityMethods.RemoveFirstCharUnsafe( strFormulaString );

          // Sometimes xlsx file can contain shared formula with range starting not in the current cell.
          recordSharedFormula.Formula = m_formulaUtil.
            ParseSharedString( strFormulaString, iRow, iCol, sheet );

          RecordTable table = cells.Table;
          int iCount = table.SharedFormulas.Count;
          // In Excel 2007 we should access shared formula by index.
          table.AddSharedFormula( 0, ( int )uiSharedGroupIndex, recordSharedFormula );
        }
      }

      FormulaRecord formulaRecord = ( FormulaRecord )BiffRecordFactory.GetRecord( TBIFFRecord.Formula );
      SharedFormulaRecord sharedFormula = ( SharedFormulaRecord )cells.Table.SharedFormulas[
        ( int )uiSharedGroupIndex ];
      formulaRecord.ParsedExpression = FormulaUtil.ConvertSharedFormulaTokens(
        sharedFormula, sheet.ParentWorkbook, iRow - 1, iCol - 1 );
      formulaRecord.Row = iRow - 1;
      formulaRecord.Column = iCol - 1;
      formulaRecord.ExtendedFormatIndex = ( ushort )iXFIndex;
      formulaRecord.CalculateOnOpen = bCalculateOnOpen;
      cells.SetCellRecord( iRow, iCol, formulaRecord );
    }
    /// <summary>
    /// Converts Excel 2007 data validation type to Excel 97-03.
    /// </summary>
    /// <param name="dataValidationType">Excel 2007 data validation type.</param>
    /// <returns>Excel 97-03 data validation type.</returns>
    private ExcelDataType ConvertDataValidationType( string dataValidationType )
    {
      if( dataValidationType == null || dataValidationType == string.Empty )
        throw new ArgumentNullException( "strErrorStyle" );

      switch( dataValidationType )
      {
        case DV.TypeCustom:
          return ExcelDataType.Formula;

        case DV.TypeDate:
          return ExcelDataType.Date;

        case DV.TypeDecimal:
          return ExcelDataType.Decimal;

        case DV.TypeList:
          return ExcelDataType.User;

        case DV.TypeNone:
          return ExcelDataType.Any;

        case DV.TypeTextLength:
          return ExcelDataType.TextLength;

        case DV.TypeTime:
          return ExcelDataType.Time;

        case DV.TypeWhole:
          return ExcelDataType.Integer;

        default:
          throw new ArgumentOutOfRangeException( "dataValidationType" );
      }
    }
    /// <summary>
    /// Returns DV error style.
    /// </summary>
    /// <param name="strErrorStyle">DV error style name.</param>
    /// <returns>DV error style.</returns>
    private ExcelErrorStyle ConvertDataValidationErrorStyle( string strErrorStyle )
    {
      if( strErrorStyle == null || strErrorStyle == string.Empty )
        throw new ArgumentNullException( "strErrorStyle" );

      switch( strErrorStyle )
      {
        case DV.ErrorStyleInformationIcon:
          return ExcelErrorStyle.Info;

        case DV.ErrorStyleStopIcon:
          return ExcelErrorStyle.Stop;

        case DV.ErrorStyleWarningIcon:
          return ExcelErrorStyle.Warning;

        default:
          throw new ArgumentOutOfRangeException( "strErrorStyle" );
      }
    }
    /// <summary>
    /// Returns DV compare operator.
    /// </summary>
    /// <param name="strOperator">DV compare operator name.</param>
    /// <returns></returns>
    private ExcelDataValidationComparisonOperator ConvertDataValidationOperator( string strOperator )
    {
      if( strOperator == null || strOperator == string.Empty )
        throw new ArgumentNullException( "strOperator" );

      switch( strOperator )
      {
        case DV.OperatorBetween:
          return ExcelDataValidationComparisonOperator.Between;

        case DV.OperatorEqual:
          return ExcelDataValidationComparisonOperator.Equal;

        case DV.OperatorGreaterThan:
          return ExcelDataValidationComparisonOperator.Greater;

        case DV.OperatorGreaterThanOrEqual:
          return ExcelDataValidationComparisonOperator.GreaterOrEqual;

        case DV.OperatorLessThan:
          return ExcelDataValidationComparisonOperator.Less;

        case DV.OperatorLessThanOrEqual:
          return ExcelDataValidationComparisonOperator.LessOrEqual;

        case DV.OperatorNotBetween:
          return ExcelDataValidationComparisonOperator.NotBetween;

        case DV.OperatorNotEqual:
          return ExcelDataValidationComparisonOperator.NotEqual;

        default:
          throw new ArgumentOutOfRangeException( "strOperator" );
      }
    }
    /// <summary>
    /// Returns DV ranges.
    /// </summary>
    /// <param name="strRange">All ranges for DV.</param>
    /// <returns>DV ranges array.</returns>
    private TAddr[] GetRangesForDataValidation( string strRange )
    {
      if( strRange == null || strRange == string.Empty )
        throw new ArgumentNullException( "strRange" );

      string[] strRanges = strRange.Split( ' ' );
      List<TAddr> ranges = new List<TAddr>();

      foreach( string strSingleRange in strRanges )
      {
        ranges.Add( GetRangeForDVOrAF( strSingleRange ) );
      }

      return ranges.ToArray();
    }
    /// <summary>
    /// Returns DV or AF range.
    /// </summary>
    /// <param name="strRange">DV or AF range string.</param>
    /// <returns>DV range.</returns>
    private TAddr GetRangeForDVOrAF( string strRange )
    {
      if( strRange == null || strRange == string.Empty )
        throw new ArgumentNullException( "strRange" );

      string strFirstRow = string.Empty;
      string strFirstColumn = string.Empty;
      string strLastRow = string.Empty;
      string strLastColumn = string.Empty;

      TAddr tAddr = new TAddr();

      if( FormulaUtil.IsCell( strRange, false, out strFirstRow, out strFirstColumn ) )
      {
        int iRow = Convert.ToInt32( strFirstRow ) - 1;
        int iCol = RangeImpl.GetColumnIndex( strFirstColumn ) - 1;
        tAddr = new TAddr( iRow, iCol, iRow, iCol );
      }
      else if( m_formulaUtil.IsCellRange( strRange, false, out strFirstRow, out strFirstColumn, out strLastRow, out strLastColumn ) )
      {
        int iFirstRow = Convert.ToInt32( strFirstRow ) - 1;
        int iFirstColumn = RangeImpl.GetColumnIndex( strFirstColumn ) - 1;
        int iLastRow = Convert.ToInt32( strLastRow ) - 1;
        int iLastColumn = RangeImpl.GetColumnIndex( strLastColumn ) - 1;
        tAddr = new TAddr( iFirstRow, iFirstColumn, iLastRow, iLastColumn );
      }

      return tAddr;
    }
    /// <summary>
    /// Returns excel filter condition.
    /// </summary>
    /// <param name="strCondition">Filter condition string name.</param>
    /// <returns>Excel filter condition.</returns>
    private ExcelFilterCondition ConvertAutoFormatFilterCondition( string strCondition )
    {
      switch( strCondition )
      {
        case AF.OperatorEqual:
          return ExcelFilterCondition.Equal;

        case AF.OperatorGreaterThan:
          return ExcelFilterCondition.Greater;

        case AF.OperatorGreaterThanOrEqual:
          return ExcelFilterCondition.GreaterOrEqual;

        case AF.OperatorLessThan:
          return ExcelFilterCondition.Less;

        case AF.OperatorLessThanOrEqual:
          return ExcelFilterCondition.LessOrEqual;

        case AF.OperatorNotEqual:
          return ExcelFilterCondition.NotEqual;

        default:
          throw new ArgumentOutOfRangeException( "strCondition" );
      }
    }
    /// <summary>
    /// Returns excel CF type value.
    /// </summary>
    /// <param name="strType">CF type sting name.</param>
    /// <param name="bIsSupportedType">Defines whether current type is currently supported.</param>|
    /// <returns>Excel CF type value.</returns>
    private ExcelCFType ConvertCFType( string strType, out bool bIsSupportedType )
    {
      bIsSupportedType = true;

      switch( strType )
      {
        case CF.TypeCellIs:
          return ExcelCFType.CellValue;
        case CF.EndsWith:
        case CF.BeginsWith:
        case CF.ContainsText:
        case CF.NotContainsText:
          return ExcelCFType.SpecificText;
        case CF.TypeExpression:
          return ExcelCFType.Formula;

        case CF.TypeDataBar:
          return ExcelCFType.DataBar;

        case CF.TypeIconSet:
          return ExcelCFType.IconSet;

        case CF.TypeColorScale:
          return ExcelCFType.ColorScale;

        case CF.TypeContainsBlank:
          return ExcelCFType.Blank;

        case CF.TypeNotContainsBlank:
          return ExcelCFType.NoBlank;
      
        case CF.TypeContainsError:
          return ExcelCFType.ContainsErrors;

        case CF.TypeNotContainsError:
          return ExcelCFType.NotContainsErrors;
        
        case CF.TimePeriodTypeName:
          return ExcelCFType.TimePeriod;

        default:
          bIsSupportedType = false;
          return ExcelCFType.CellValue;
      }
    }
    /// <summary>
    /// Returns excel comparison operator.
    /// </summary>
    /// <param name="strOperator">Operator string name.</param>
    /// <param name="bIsSupportedOperator">Defines whether  current operator is currently supported.</param>
    /// <returns>Excel operator value.</returns>
    private ExcelComparisonOperator ConvertCFOperator( string strOperator, out bool bIsSupportedOperator )
    {
      bIsSupportedOperator = true;

      switch( strOperator )
      {
        case CF.OperatorBetween:
          return ExcelComparisonOperator.Between;

        case CF.OperatorEqual:
          return ExcelComparisonOperator.Equal;

        case CF.OperatorGreaterThan:
          return ExcelComparisonOperator.Greater;

        case CF.OperatorGreaterThanOrEqual:
          return ExcelComparisonOperator.GreaterOrEqual;

        case CF.OperatorLessThan:
          return ExcelComparisonOperator.Less;

        case CF.OperatorLessThanOrEqual:
          return ExcelComparisonOperator.LessOrEqual;

        case CF.OperatorDoesNotContain:
          return ExcelComparisonOperator.NotContainsText;

        case CF.OperatorNotBetween:
          return ExcelComparisonOperator.NotBetween;

        case CF.OperatorNotEqual:
          return ExcelComparisonOperator.NotEqual;

        case CF.OperatorBeginsWith:
          return ExcelComparisonOperator.BeginsWith;
        case CF.OperatorContains:
          return ExcelComparisonOperator.ContainsText;
        case CF.OperatorEndsWith:
          return ExcelComparisonOperator.EndsWith;

        default:
          throw new ArgumentOutOfRangeException( "strOperator" );
      }
    }
      /// <summary>
      /// Returns the time period type
      /// </summary>
      /// <param name="timePeriod"></param>
      /// <param name="bIsSupportedOperator"></param>
      /// <returns></returns>
      private CFTimePeriods ConvertCFTimePeriods(string timePeriod,out bool bIsSupportedTimePeriod)
      {
          bIsSupportedTimePeriod = true;
          switch(timePeriod)
          {
              case CF.TimePeriodYesterday:
                  return CFTimePeriods.Yesterday;
              case CF.TimePeriodToday:
                  return CFTimePeriods.Today;
              case CF.TimePeriodTomorrow:
                  return CFTimePeriods.Tomorrow;
              case CF.TimePeriodLastsevenDays:
                  return CFTimePeriods.Last7Days;
              case CF.TimePeriodLastWeek:
                  return CFTimePeriods.LastWeek;
              case CF.TimePeriodThisWeek:
                  return CFTimePeriods.ThisWeek;
              case CF.TimePeriodNextWeek:
                  return CFTimePeriods.NextWeek;
              case CF.TimePeriodLastMonth:
                  return CFTimePeriods.LastMonth;
              case CF.TimePeriodThisMonth:
                  return CFTimePeriods.ThisMonth;
              case CF.TimePeriodNextMonth:
                  return CFTimePeriods.NextMonth;                 
              default:                  
                  throw new ArgumentOutOfRangeException( "timePeriod" );
          }
      }
    /// <summary>
    /// Returns xml element value.
    /// </summary>
    /// <param name="reader">XmlReader to get value from.</param>
    /// <returns>Xml element value.</returns>
    private string GetReaderElementValue( XmlReader reader )
    {
      if( reader.IsEmptyElement )
      {
        reader.Read();
        return string.Empty;
      }

      string strResult;

      reader.Read();

      if( reader.NodeType != XmlNodeType.EndElement )
      {
        strResult = reader.Value;
        reader.Skip();
      }
      else
      {
        strResult = string.Empty;
      }

      reader.Skip();

      return strResult;
    }
    /// <summary>
    /// Converts color by applying shade value.
    /// </summary>
    /// <param name="result">Color to process.</param>
    /// <param name="shade">Shade to apply.</param>
    /// <returns>Modified color.</returns>
    internal Color ConvertColorByShade( Color result, double shade )
    {
      byte a = ( byte )( result.A * shade );
      byte r = ( byte )( result.R * shade );
      byte g = ( byte )( result.G * shade );
      byte b = ( byte )( result.B * shade );
      return Color.FromArgb( a, r, g, b );
    }
    /// <summary>
    /// Converts Control unicode Character to ASCII .
    /// </summary>
    /// <param name="Value">String Value.</param>
    /// <returns>Modified ASCII String.</returns>
    private string ConvertToASCII(string value)
    {
        value = value.Replace(LineFeed, "\n");
        value = value.Replace(CarriageReturn, "\r");
        value = value.Replace(Tab, "\t");
        value = value.Replace(BackSpace, "\b");
        value = value.Replace(NullChar, "\0");

        return value;
    }
    #endregion

    #region Static methods
    /// <summary>
    /// Copies fill settings from fill object into extended format.
    /// </summary>
    /// <param name="fill">Fill to copy from.</param>
    /// <param name="extendedFormat">Extended format to copy into.</param>
    public static void CopyFillSettings( FillImpl fill, ExtendedFormatImpl extendedFormat )
    {
      // TODO: check this code for different patterns, maybe we have to swap colors in some or all cases.
      if( fill.FillType == ExcelFillType.Gradient )
      {
        extendedFormat.Gradient = new ShapeFillImpl( extendedFormat.Application, extendedFormat, ExcelFillType.Gradient );
        IGradient gradient = extendedFormat.Gradient;
        gradient.GradientStyle = fill.GradientStyle;
        gradient.GradientVariant = fill.GradientVariant;
        gradient.BackColorObject.CopyFrom( fill.PatternColorObject, true );
        gradient.ForeColorObject.CopyFrom( fill.ColorObject, true );
        extendedFormat.Record.AdtlFillPattern = ( ushort )ExcelPattern.Gradient;
      }
      else
      {
        //if( fill.Pattern != ExcelPattern.None )
        extendedFormat.IncludePatterns = true;

        extendedFormat.ColorObject.CopyFrom( fill.ColorObject, true );
        extendedFormat.PatternColorObject.CopyFrom( fill.PatternColorObject, true );
        extendedFormat.FillPattern = fill.Pattern;
        //record.FillForeground = ( ushort )fill.ColorObject.GetIndexed( m_book );
        //record.FillBackground = ( ushort )fill.PatternColorObject.GetIndexed( m_book );
        //record.AdtlFillPattern = ( ushort )fill.Pattern;
      }
    }
    #endregion
  }
}
