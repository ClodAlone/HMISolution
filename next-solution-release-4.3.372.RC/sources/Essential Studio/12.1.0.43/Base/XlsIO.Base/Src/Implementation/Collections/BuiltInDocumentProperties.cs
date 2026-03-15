#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

#region file using directives
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics;

using Syncfusion.CompoundFile.XlsIO.Native;

using Syncfusion.CompoundFile.XlsIO;
using System.Collections.Generic;
using Syncfusion.CompoundFile.XlsIO.Net;
using System.Text;
#endregion

namespace Syncfusion.XlsIO.Implementation.Collections
{
  /// <summary>
  /// Represents the document properties in a MS Word document.
  /// </summary>
  public class BuiltInDocumentProperties
    : CollectionBaseEx<DocumentPropertyImpl>
    , IBuiltInDocumentProperties
  {
    #region Class constants
    /// <summary>
    /// Options for property storage creation.
    /// </summary>
    private const STGM DEF_PROPERTY_STORAGE_OPTIONS = STGM.STGM_CREATE
      | STGM.STGM_READWRITE | STGM.STGM_SHARE_EXCLUSIVE;
    #endregion

    #region Class static members
    /// <summary>
    /// Guid for parsing/serialization summary properties.
    /// </summary>
    public static readonly Guid GuidSummary = new Guid( "F29F85E0-4FF9-1068-AB91-08002B27B3D9" );
    /// <summary>
    /// Guid for parsing/serialization document properties.
    /// </summary>
    public static readonly Guid GuidDocument = new Guid( "D5CDD502-2E9C-101B-9397-08002B2CF9AE" );
    #endregion

    #region Class members
    /// <summary>
    /// Dictionary with document properties, key - property name/id, value - property object.
    /// </summary>
    private Dictionary<int, DocumentPropertyImpl> m_documentHash = new Dictionary<int, DocumentPropertyImpl>();
    /// <summary>
    /// Dictionary with summary document properties, key - property id, value - property object.
    /// </summary>
    private Dictionary<int, DocumentPropertyImpl> m_summaryHash = new Dictionary<int, DocumentPropertyImpl>();
    #endregion

    #region Class initialize/finalize methods
    /// <summary>
    /// Default constructor.
    /// </summary>
    public BuiltInDocumentProperties( IApplication application, object parent )
      : base( application, parent )
    {
    }
    #endregion

    #region Class Document properties
    /// <summary>
    /// Returns single entry from the collection. Read-only.
    /// </summary>
    public IDocumentProperty this[ ExcelBuiltInProperty index ]
    {
      get
      {
        int iIndex = ( int )index;
        IDictionary dicProperties = GetDictionary( index );

        if( dicProperties.Contains( iIndex ) )
        {
          return ( IDocumentProperty )dicProperties[ iIndex ];
        }
        else
        {
          DocumentPropertyImpl property = new DocumentPropertyImpl( ( BuiltInProperty )index, null );
          Add( property );
          return property;
        }
      }
    }
    /// <summary>
    /// Returns single entry from the collection. Read-only.
    /// </summary>
    public IDocumentProperty this[ int iIndex ]
    {
      get
      {
        if( iIndex < 0 || iIndex > Count - 1 )
        {
          throw new ArgumentOutOfRangeException( "iIndex",
            "Value cannot be less than 0 and greater than than Count - 1." );
        }

        return List[ iIndex ] as IDocumentProperty;
      }
    }
    #endregion

    #region Interface methods
    /// <summary>
    /// Indicates whether collection contains specified property.
    /// </summary>
    /// <param name="index">Property id.</param>
    /// <returns>True if collection contains required property.</returns>
    public bool Contains( ExcelBuiltInProperty index )
    {
      int iIndex = ( int )index;
      IDictionary dicProperties = GetDictionary( index );

      return dicProperties.Contains( iIndex );
    }
    #endregion

    #region Typed property values
    /// <summary>
    /// Title document property.
    /// </summary>
    public string Title
    {
      get
      {
        return ( Contains( ExcelBuiltInProperty.Title ) )
          ? this[ ExcelBuiltInProperty.Title ].Text
          : null;
      }
      set
      {
        this[ ExcelBuiltInProperty.Title ].Text = value;
      }
    }
    /// <summary>
    /// Subject document property.
    /// </summary>
    public string Subject
    {
      get
      {
        return ( Contains( ExcelBuiltInProperty.Subject ) )
          ? this[ ExcelBuiltInProperty.Subject ].Text
          : null;
      }
      set
      {
        this[ ExcelBuiltInProperty.Subject ].Text = value;
      }
    }
    /// <summary>
    /// Author document property.
    /// </summary>
    public string Author
    {
      get
      {
        return ( Contains( ExcelBuiltInProperty.Author ) )
          ? this[ ExcelBuiltInProperty.Author ].Text
          : null;
      }
      set
      {
        this[ ExcelBuiltInProperty.Author ].Text = value;
      }
    }
    /// <summary>
    /// Keywords document property.
    /// </summary>
    public string Keywords
    {
      get
      {
        return ( Contains( ExcelBuiltInProperty.Keywords ) )
          ? this[ ExcelBuiltInProperty.Keywords ].Text
          : null;
      }
      set
      {
        this[ ExcelBuiltInProperty.Keywords ].Text = value;
      }
    }
    /// <summary>
    /// Comments document property.
    /// </summary>
    public string Comments
    {
      get
      {
        return ( Contains( ExcelBuiltInProperty.Comments ) )
          ? this[ ExcelBuiltInProperty.Comments ].Text
          : null;
      }
      set
      {
        this[ ExcelBuiltInProperty.Comments ].Text = value;
      }
    }
    /// <summary>
    /// Template document property.
    /// </summary>
    public string Template
    {
      get
      {
        return ( Contains( ExcelBuiltInProperty.Template ) )
          ? this[ ExcelBuiltInProperty.Template ].Text
          : null;
      }
      set
      {
        this[ ExcelBuiltInProperty.Template ].Text = value;
      }
    }
    /// <summary>
    /// LastAuthor document property.
    /// </summary>
    public string LastAuthor
    {
      get
      {
        return ( Contains( ExcelBuiltInProperty.LastAuthor ) )
          ? this[ ExcelBuiltInProperty.LastAuthor ].Text
          : null;
      }
      set
      {
        this[ ExcelBuiltInProperty.LastAuthor ].Text = value;
      }
    }
    /// <summary>
    /// Revision number document property.
    /// </summary>
    public string RevisionNumber
    {
      get
      {
        return ( Contains( ExcelBuiltInProperty.RevisionNumber ) )
          ? this[ ExcelBuiltInProperty.RevisionNumber ].Text
          : null;
      }
      set
      {
        this[ ExcelBuiltInProperty.RevisionNumber ].Text = value;
      }
    }
    /// <summary>
    /// EditTime document property.
    /// </summary>
    public TimeSpan EditTime
    {
      get
      {
        return ( Contains( ExcelBuiltInProperty.EditTime ) )
          ? this[ ExcelBuiltInProperty.EditTime ].TimeSpan
          : TimeSpan.MinValue;
      }
      set
      {
        this[ ExcelBuiltInProperty.EditTime ].TimeSpan = value;
      }
    }
    /// <summary>
    /// LastPrinted document property.
    /// </summary>
    public DateTime LastPrinted
    {
      get
      {
        return ( Contains( ExcelBuiltInProperty.LastPrinted ) )
          ? this[ ExcelBuiltInProperty.LastPrinted ].DateTime
          : DateTime.MinValue;
      }
      set
      {
        this[ ExcelBuiltInProperty.LastPrinted ].DateTime = value;
      }
    }
    /// <summary>
    /// CreationDate document property.
    /// </summary>
    public DateTime CreationDate
    {
      get
      {
        return ( Contains( ExcelBuiltInProperty.CreationDate ) )
          ? this[ ExcelBuiltInProperty.CreationDate ].DateTime
          : DateTime.MinValue;
      }
      set
      {
        this[ ExcelBuiltInProperty.CreationDate ].DateTime = value;
      }
    }
    /// <summary>
    /// LastSaveDate document property.
    /// </summary>
    public DateTime LastSaveDate
    {
      get
      {
        return ( Contains( ExcelBuiltInProperty.LastSaveDate ) )
          ? this[ ExcelBuiltInProperty.LastSaveDate ].DateTime
          : DateTime.MinValue;
      }
      set
      {
        this[ ExcelBuiltInProperty.LastSaveDate ].DateTime = value;
      }
    }
    /// <summary>
    /// PageCount document property.
    /// </summary>
    public int PageCount
    {
      get
      {
        return ( Contains( ExcelBuiltInProperty.PageCount ) )
          ? this[ ExcelBuiltInProperty.PageCount ].Int32
          : int.MinValue;
      }
      set
      {
        this[ ExcelBuiltInProperty.PageCount ].Int32 = value;
      }
    }
    /// <summary>
    /// WordCount document property.
    /// </summary>
    public int WordCount
    {
      get
      {
        return ( Contains( ExcelBuiltInProperty.WordCount ) )
          ? this[ ExcelBuiltInProperty.WordCount ].Int32
          : int.MinValue;
      }
      set
      {
        this[ ExcelBuiltInProperty.WordCount ].Int32 = value;
      }
    }
    /// <summary>
    /// CharCount document property.
    /// </summary>
    public int CharCount
    {
      get
      {
        return ( Contains( ExcelBuiltInProperty.CharCount ) )
          ? this[ ExcelBuiltInProperty.CharCount ].Int32
          : int.MinValue;
      }
      set
      {
        this[ ExcelBuiltInProperty.CharCount ].Int32 = value;
      }
    }
//    /// <summary>
//    /// Thumbnail document property.
//    /// </summary>
//Thumbnail
//{
//get
//{
//}
//}
    /// <summary>
    /// ApplicationName document property.
    /// </summary>
    public string ApplicationName
    {
      get
      {
          if (((WorkbookImpl)Parent).Version == ExcelVersion.Excel97to2003)
          {
              if (!HasHeadingPair)
                  return Syncfusion.XlsIO.Implementation.XmlSerialization.Constants.DocProp.EssentialXlsIO;
          }
              return (Contains(ExcelBuiltInProperty.ApplicationName))
                ? this[ExcelBuiltInProperty.ApplicationName].Text
                : null;
      }
      set
      {
        this[ ExcelBuiltInProperty.ApplicationName ].Text = value;
      }
    }
    /// <summary>
    /// Indicates whether the file has Headpair tag.
    /// </summary>
    internal bool HasHeadingPair
    {
        get
        {
            return (Contains(ExcelBuiltInProperty.HeadingPair)?
                (this[ExcelBuiltInProperty.HeadingPair].Value==null)?false:true:false);
        }
        set
        {
            this[ExcelBuiltInProperty.HeadingPair].Value = value;
        }
    }
    /// <summary>
    /// Security document property.
    /// </summary>
    public int Security
    {
      get
      {
        return ( Contains( ExcelBuiltInProperty.Security ) )
          ? this[ ExcelBuiltInProperty.Security ].Int32
          : int.MinValue;
      }
      set
      {
        this[ ExcelBuiltInProperty.Security ].Int32 = value;
      }
    }

    /// <summary>
    /// Category.
    /// </summary>
    public string Category
    {
      get
      {
        return ( Contains( ExcelBuiltInProperty.Category ) )
          ? this[ ExcelBuiltInProperty.Category ].Text
          : null;
      }
      set
      {
        this[ ExcelBuiltInProperty.Category ].Text = value;
      }
    }
    /// <summary>
    /// Target format for presentation (35mm, printer, video, and so on).
    /// </summary>
    public string PresentationTarget
    {
      get
      {
        return ( Contains( ExcelBuiltInProperty.PresentationTarget ) )
          ? this[ ExcelBuiltInProperty.PresentationTarget ].Text
          : null;
      }
      set
      {
        this[ ExcelBuiltInProperty.PresentationTarget ].Text = value;
      }
    }
    /// <summary>
    /// ByteCount.
    /// </summary>
    public int ByteCount
    {
      get
      {
        return ( Contains( ExcelBuiltInProperty.ByteCount ) )
          ? this[ ExcelBuiltInProperty.ByteCount ].Int32
          : int.MinValue;
      }
      set
      {
        this[ ExcelBuiltInProperty.ByteCount ].Int32 = value;
      }
    }
    /// <summary>
    /// LineCount.
    /// </summary>
    public int LineCount
    {
      get
      {
        return ( Contains( ExcelBuiltInProperty.LineCount ) )
          ? this[ ExcelBuiltInProperty.LineCount ].Int32
          : int.MinValue;
      }
      set
      {
        this[ ExcelBuiltInProperty.LineCount ].Int32 = value;
      }
    }
    /// <summary>
    /// ParCount.
    /// </summary>
    public int ParagraphCount
    {
      get
      {
        return ( Contains( ExcelBuiltInProperty.PageCount ) )
          ? this[ ExcelBuiltInProperty.ParagraphCount ].Int32
          : int.MinValue;
      }
      set
      {
        this[ ExcelBuiltInProperty.ParagraphCount ].Int32 = value;
      }
    }
    /// <summary>
    /// SlideCount.
    /// </summary>
    public int SlideCount
    {
      get
      {
        return ( Contains( ExcelBuiltInProperty.SlideCount ) )
          ? this[ ExcelBuiltInProperty.SlideCount ].Int32
          : int.MinValue;
      }
      set
      {
        this[ ExcelBuiltInProperty.SlideCount ].Int32 = value;
      }
    }
    /// <summary>
    /// NoteCount.
    /// </summary>
    public int NoteCount
    {
      get
      {
        return ( Contains( ExcelBuiltInProperty.NoteCount ) )
          ? this[ ExcelBuiltInProperty.NoteCount ].Int32
          : int.MinValue;
      }
      set
      {
        this[ ExcelBuiltInProperty.NoteCount ].Int32 = value;
      }
    }
    /// <summary>
    /// HiddenCount.
    /// </summary>
    public int HiddenCount
    {
      get
      {
        return ( Contains( ExcelBuiltInProperty.HiddenCount ) )
          ? this[ ExcelBuiltInProperty.HiddenCount ].Int32
          : int.MinValue;
      }
      set
      {
        this[ ExcelBuiltInProperty.HiddenCount ].Int32 = value;
      }
    }
    /// <summary>
    /// MmclipCount.
    /// </summary>
    public int MultimediaClipCount
    {
      get
      {
        return ( Contains( ExcelBuiltInProperty.MultimediaClipCount ) )
          ? this[ ExcelBuiltInProperty.MultimediaClipCount ].Int32
          : int.MinValue;
      }
      set
      {
        this[ ExcelBuiltInProperty.MultimediaClipCount ].Int32 = value;
      }
    }
    /// <summary>
    /// Set to True when scaling of the thumbnail is desired. If not set, cropping is desired. 
    /// </summary>
    public bool ScaleCrop
    {
      get
      {
        return ( Contains( ExcelBuiltInProperty.ScaleCrop ) )
          ? this[ ExcelBuiltInProperty.ScaleCrop ].Boolean
          : false;
      }
      set
      {
        this[ ExcelBuiltInProperty.ScaleCrop ].Boolean = value;
      }
    }
    //    /// <summary>
//    /// HeadingPair.
//    /// </summary>
//HeadingPair
//{
//get
//{
//}
//}
//    /// <summary>
//    /// DocParts.
//    /// </summary>
//DocParts
//{
//get
//{
//}
//}
    /// <summary>
    /// Manager.
    /// </summary>
    public string Manager
    {
      get
      {
        return ( Contains( ExcelBuiltInProperty.Manager ) )
          ? this[ ExcelBuiltInProperty.Manager ].Text
          : null;
      }
      set
      {
        this[ ExcelBuiltInProperty.Manager ].Text = value;
      }
    }
    /// <summary>
    /// Company.
    /// </summary>
    public string Company
    {
      get
      {
        return ( Contains( ExcelBuiltInProperty.Company ) )
          ? this[ ExcelBuiltInProperty.Company ].Text
          : null;
      }
      set
      {
        this[ ExcelBuiltInProperty.Company ].Text = value;
      }
    }
    /// <summary>
    /// Boolean value to indicate whether the custom links are
    /// hampered by excessive noise, for all applications.
    /// </summary>
    public bool LinksDirty
    {
      get
      {
        return ( Contains( ExcelBuiltInProperty.LinksDirty ) )
          ? this[ ExcelBuiltInProperty.LinksDirty ].Boolean
          : false;
      }
      set
      {
        this[ ExcelBuiltInProperty.LinksDirty ].Boolean = value;
      }
    }

    #endregion

    #region Class helper methods
    /// <summary>
    /// Returns dictionary where property must be placed.
    /// </summary>
    /// <param name="propertyId">Property id.</param>
    /// <returns>Dictionary where property must be placed.</returns>
    private IDictionary GetDictionary( ExcelBuiltInProperty propertyId )
    {
      bool bSummary;
      DocumentPropertyImpl.CorrectIndex( ( BuiltInProperty )propertyId, out bSummary );

      return bSummary ? m_summaryHash : m_documentHash;
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// OnClear is invoked after Clear behavior.
    /// </summary>
    protected override void OnClearComplete()
    {
      m_documentHash.Clear();
      m_summaryHash.Clear();
    }

    #endregion

    #region Class parse / serialization methods
    /// <summary>
    /// Extract built-in document properties.
    /// </summary>
    /// <param name="setProp">IPropertySetStorage to extract properties from.</param>
    [CLSCompliant( false )]
    public void Parse( DocumentPropertyCollection properties )
    {
      List<PropertySection> lstSections = properties.Sections;

      for( int i = 0, len = lstSections.Count; i < len; i++ )
      {
        PropertySection section = lstSections[ i ];

        if( section.Id == GuidSummary )
        {
          ReadProperties( section, m_summaryHash, InnerList, true, true );
        }
        else if( section.Id == GuidDocument )
        {
          ReadProperties( section, m_documentHash, InnerList, false, true );
        }
      }
    }
    /// <summary>
    /// Reads properties from PropertySection.
    /// </summary>
    /// <param name="section">PropertySection to read data from.</param>
    /// <param name="dicProperties">Dictionary that will receive new properties.</param>
    /// <param name="lstProperties">List to add properties to.</param>
    /// <param name="bSummary">Indicates whether we are reading document summary properties.</param>
    /// <param name="bBuiltIn">Indicates whether property is built-in.</param>
    public static void ReadProperties( PropertySection section,
      IDictionary dicProperties,
      List<DocumentPropertyImpl> lstProperties, bool bSummary, bool bBuiltIn )
    {
      Dictionary<int, DocumentPropertyImpl> hashPropById = null;

      if( !bBuiltIn )
      {
        hashPropById = new Dictionary<int, DocumentPropertyImpl>();
      }

      List<PropertyData> arrProperties = section.Properties;

      for( int i = 0, len = arrProperties.Count; i < len; i++ )
      {
        PropertyData propertyData = arrProperties[ i ];

        if( propertyData.IsLinkToSource )
        {
          int parentId = propertyData.ParentId;
          DocumentPropertyImpl property = hashPropById[ parentId ];
          property.SetLinkSource( propertyData );
        }
        else
        {
          DocumentPropertyImpl property = new DocumentPropertyImpl( propertyData, bSummary );

          object key = bBuiltIn
            ? ( object )( int )property.PropertyId
            : ( object )property.Name;

          if( !bBuiltIn )
          {
            hashPropById.Add( propertyData.Id, property );
          }

          bool bContains = dicProperties.Contains( key );
          
          if (property.PropertyId == BuiltInProperty.HeadingPair)
              property.Boolean = true;
          dicProperties[key] = property;      
          if( !bContains )
          {
            lstProperties.Add( property );
          }
        }
      }
    }
    /// <summary>
    /// Writes properties into specified property section.
    /// </summary>
    /// <param name="section"></param>
    /// <param name="values"></param>
    public static void WriteProperties( PropertySection section, ICollection values )
    {
      int iPropertyId = 2;

      foreach( DocumentPropertyImpl property in values )
      {
          //TODO: HeadingPair skipped to identify the file is Saved by XlsIO.
          if (property.PropertyId == BuiltInProperty.HeadingPair)
              continue;
        PropertyData data = ConvertToPropertyData( property, iPropertyId );
        section.Properties.Add( data );
        iPropertyId++;
      }
    }
    /// <summary>
    /// Converts property into PropertyData structure.
    /// </summary>
    /// <param name="property">Property to convert.</param>
    /// <param name="iPropertyId">Proposed property id.</param>
    /// <returns>Converted structure.</returns>
    private static PropertyData ConvertToPropertyData( DocumentPropertyImpl property, int iPropertyId )
    {
      PropertyData result = new PropertyData();
      property.FillPropVariant( result, iPropertyId );

      if( property.InternalName != null )
        result.Name = property.InternalName;

      return result;
    }
#if !SILVERLIGHT && !WINRT && !WP
    /// <summary>
    /// Serializes built-in properties.
    /// </summary>
    /// <param name="setProp">IPropertySetStorage to serialize into.</param>
    [ CLSCompliant( false ) ]
    public void Serialize( IPropertySetStorage setProp )
    {
      ApplicationName = Syncfusion.XlsIO.Implementation.XmlSerialization.Constants.DocProp.EssentialXlsIO;
      WriteProperties( setProp, GuidSummary, m_summaryHash.Values );
      WriteProperties( setProp, GuidDocument, m_documentHash.Values );
    }
    /// <summary>
    /// Extract built-in document properties.
    /// </summary>
    /// <param name="setProp">IPropertySetStorage to extract properties from.</param>
    [ CLSCompliant( false ) ]
    public void Parse( IPropertySetStorage setProp )
    {
      ReadProperties( setProp, GuidSummary, m_summaryHash, InnerList, true, true );
      ReadProperties( setProp, GuidDocument, m_documentHash, InnerList, false, true );
    }
    /// <summary>
    /// Writes properties into IPropertySetStorage.
    /// </summary>
    /// <param name="setProp">IPRopertySetStorage to write properties into.</param>
    /// <param name="guid">Storage GUID.</param>
    /// <param name="colProperties">Collection of properties to write.</param>
    [ CLSCompliant( false ) ]
    public static void WriteProperties( IPropertySetStorage setProp, Guid guid, ICollection colProperties )
    {
      if( setProp == null )
        throw new ArgumentNullException( "setProp" );

      if( colProperties == null )
        throw new ArgumentNullException( "colProperties" );

      IPropertyStorage storProp = null;
      Guid guidEmpty = Guid.Empty;

#if  (SILVERLIGHT) || (WINRT) || (WP)
      short codePage = 1252;
#else
      short codePage = ( short )Encoding.Default.CodePage;
#endif

      try
      {
        setProp.Create( ref guid, ref guidEmpty, 0, DEF_PROPERTY_STORAGE_OPTIONS
          , out storProp );

        using( PropVariant variant = new PropVariant() )
        {
          variant.Id = 1;
          variant.SetValue( codePage, PropertyType.Int16 );
          variant.Write( storProp );

          int iPropertyId = 2;

          foreach( DocumentPropertyImpl property in colProperties )
          {
              if (property.PropertyId == BuiltInProperty.HeadingPair)
                  continue;
            property.Write( storProp, variant, iPropertyId );
            iPropertyId++;
          }
        }
      }
      catch( Exception ex )
      {
        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, ex.Message, "Exception" );
        //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, ex.StackTrace, "Stack trace" );
      }
      finally
      {
        if( storProp != null )
        {
          storProp.Commit( STGC.STGC_DEFAULT );
          System.Runtime.InteropServices.Marshal.FinalReleaseComObject( storProp );
        }
      }
    }
    /// <summary>
    /// Reads properties from IPropertySetStorage.
    /// </summary>
    /// <param name="setProp">IPropertySetStorage to read from</param>
    /// <param name="guid">Storage GUID.</param>
    /// <param name="dicProperties">Dictionary that will receive new properties.</param>
    /// <param name="lstProperties">List to add properties to.</param>
    /// <param name="bSummary">Indicates whether we are reading document summary properties.</param>
    /// <param name="bBuiltIn">Indicates whether property is built-in.</param>
    [ CLSCompliant( false ) ]
    public static void ReadProperties( IPropertySetStorage setProp, Guid guid,
      IDictionary dicProperties, IList<DocumentPropertyImpl> lstProperties, bool bSummary, bool bBuiltIn )
    {
      if( setProp == null )
        throw new ArgumentNullException( "setProp" );

      if( dicProperties == null )
        throw new ArgumentNullException( "dicProperties" );

      IPropertyStorage storProp = null;
      IEnumSTATPROPSTG enumStatPropStg = null;
      Dictionary<int, DocumentPropertyImpl> hashPropById = null;

      if( !bBuiltIn )
      {
        hashPropById = new Dictionary<int, DocumentPropertyImpl>();
      }

      int iResult = setProp.Open( ref guid, STGM.STGM_READ | STGM.STGM_SHARE_EXCLUSIVE, out storProp );

      if( iResult != 0 )
        return;

      try
      {
        storProp.Enum( out enumStatPropStg );

        while( true )
        {
          int iStructCount = 0;
          tagSTATPROPSTG propertyInfo = new tagSTATPROPSTG();
          enumStatPropStg.Next( 1, ref propertyInfo, out iStructCount );

          if( iStructCount == 0 )
          {
            return;
          }

          using( PropVariant variant = new PropVariant( propertyInfo, storProp, bBuiltIn ) )
          {
            if( variant.IsLinkToSource )
            {
              int parentId = variant.ParentId;
              DocumentPropertyImpl property = hashPropById[ parentId ];
              property.SetLinkSource( variant );
            }
            else
            {
              DocumentPropertyImpl property = new DocumentPropertyImpl( variant, bSummary );
              object key = bBuiltIn
                ? ( object )( int )property.PropertyId
                : ( object )property.Name;

              if( !bBuiltIn )
              {
                hashPropById.Add( ( int )propertyInfo.propid, property );
              }

              bool bContains = dicProperties.Contains( key );
              dicProperties[ key ] = property;

              if( !bContains )
              {
                lstProperties.Add( property );
              }
            }
          }
        }
      }
      finally
      {
        if( storProp != null )
        {
          Marshal.FinalReleaseComObject( storProp );
        }
        if( enumStatPropStg != null )
        {
          Marshal.FinalReleaseComObject( enumStatPropStg );
        }

      }
    }
#endif
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    protected override void OnInsertComplete(int index, DocumentPropertyImpl value)
    {
      base.OnInsertComplete (index, value);

      DocumentPropertyImpl property = ( DocumentPropertyImpl )value;
      ExcelBuiltInProperty propertyIndex = ( ExcelBuiltInProperty )property.PropertyId;
      IDictionary dicProperties = GetDictionary( propertyIndex );
      int iIndex = ( int )propertyIndex;
      dicProperties.Add( iIndex, property );
    }

    public void Serialize( PropertySection summarySection, PropertySection documentSection )
    {
      WriteProperties( summarySection, m_summaryHash.Values );
      WriteProperties( documentSection, m_documentHash.Values );
    }
    #endregion
  }
}
