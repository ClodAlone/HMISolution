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
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;

using Syncfusion.XlsIO.Implementation.Collections;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation;
using Syncfusion.CompoundFile.XlsIO.Native;
using Syncfusion.CompoundFile.XlsIO.Net;
using Syncfusion.CompoundFile.XlsIO;
using Syncfusion.XlsIO.Implementation.Shapes;
using Syncfusion.XlsIO.Implementation.XmlSerialization;


namespace Syncfusion.XlsIO.Implementation
{
  public class OleObject : IOleObject
  {
    #region Constants
    /// <summary>
    /// 
    /// </summary>
    private const string DEF_OBJECT_POOL_NAME = "ObjectPool";
    /// <summary>
    /// 
    /// </summary>
    private const string DEF_OLE_STREAM_NAME = "Ole";
    /// <summary>
    /// 
    /// </summary>
    private const string DEF_INFO_STREAM_NAME = "ObjInfo";
    /// <summary>
    /// 
    /// </summary>
    private const string DEF_COMP_STREAM_NAME = "CompObj";
    /// <summary>
    /// 
    /// </summary>
    private const string DEF_NATIVE_STREAM_NAME = "Ole10Native";
    #endregion

    #region Members
    /// <summary>
    /// 
    /// </summary>
    private byte[] m_fileNativeData;
    /// <summary>
    /// 
    /// </summary>
    private byte[] m_file;
    /// <summary>
    /// 
    /// </summary>
    private CompObjectStream m_compObjectStream;
    /// <summary>
    /// 
    /// </summary>
    private OleStream m_oleStream;
    /// <summary>
    /// 
    /// </summary>
    private SizeF m_size;
    /// <summary>
    /// 
    /// </summary>
    private bool m_isIcon = true;
    /// <summary>
    /// 
    /// </summary>
    private string m_fileName = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    private Stream m_container;
    /// <summary>
    /// 
    /// </summary>
    private ObjectInfoStream m_objectInfoStream;
    /// <summary>
    /// 
    /// </summary>
    private Dictionary<string, int> m_location;
    /// <summary>
    /// 
    /// </summary>
    private string m_storageName;
    /// <summary>
    /// 
    /// </summary>
    private DVAspect m_dvAspect = DVAspect.DVASPECT_CONTENT;
    /// <summary>
    /// 
    /// </summary>
    //private int? m_shapeId;
    private ShapeImpl m_shape;
    /// <summary>
    /// 
    /// </summary>
    private string m_shapeRId;
    /// <summary>
    /// 
    /// </summary>
    private string m_oleFileName = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    private string m_objectType = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    private bool m_isContainer = false;
    /// <summary>
    /// 
    /// </summary>
    private OleLinkType m_oleLinkType = OleLinkType.Embed;
    /// <summary>
    /// 
    /// </summary>
    private bool m_isStream;
    /// <summary>
    /// 
    /// </summary>
    private WorksheetImpl m_sheet;
    /// <summary>
    /// Excel 2007 content type.
    /// </summary>
    private string m_strContentType = Excel2007Serializator.OleObjectContentType;
    /// <summary>
    /// Excel 2007 relation type.
    /// </summary>
    private string m_strRelationType = RelationTypes.OleObject;
    private OleObjectType m_oleObjectType = OleObjectType.Package;
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the location.
    /// </summary>
    /// <value>The location.</value>
    public IRange Location
    {
      get
      {
        return m_sheet[ m_shape.TopRow, m_shape.LeftColumn ];//m_rangeLocation;
      }
      set
      {
        //m_rangeLocation = value;
        m_shape.TopRow = value.Row;
        m_shape.LeftColumn = value.Column;
      }
    }
    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    /// <value>The size.</value>
    public Size Size
    {
      get
      {
        return new Size( m_shape.Width, m_shape.Height );
      }
      set
      {
        m_shape.Width = value.Width;
        m_shape.Height = value.Height;
      }
    }
    /// <summary>
    /// Gets or sets the picture.
    /// </summary>
    /// <value>The picture.</value>
    public Image Picture
    {
      get
      {
        IPictureShape picture = Shape;

        return ( picture != null ) ?
          picture.Picture :
          null;
      }
      set
      {
        if( value == null )
          throw new Exception( "Image" );

        throw new NotImplementedException();
        //Shape.Picture = value;
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether [display as icon].
    /// </summary>
    /// <value><c>true</c> if [display as icon]; otherwise, <c>false</c>.</value>
    public bool DisplayAsIcon
    {
      get
      {
        return m_isIcon;
      }
      set
      {
        if( value == true )
        {
          DvAspect = DVAspect.DVASPECT_ICON;
          m_isIcon = value;
        }
        else
        {
          DvAspect = DVAspect.DVASPECT_CONTENT;
          m_isIcon = value;
        }
      }
    }
    /// <summary>
    /// Gets or sets the type of the OLE.
    /// </summary>
    /// <value>The type of the OLE.</value>
    public OleLinkType OleType
    {
      get
      {
        return m_oleLinkType;
      }
      set
      {
        m_oleLinkType = value;
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is stream.
    /// </summary>
    /// <value><c>true</c> if this instance is stream; otherwise, <c>false</c>.</value>
    public bool IsStream
    {
      get
      {
        return m_isStream;
      }
      set
      {
        m_isStream = value;
      }
    }
    /// <summary>
    /// Gets or sets the index of the OLE sheet.
    /// </summary>
    /// <value>The index of the OLE sheet.</value>
    public WorksheetImpl OleSheet
    {
      get
      {
        return m_sheet;
      }
    }

    /// <summary>
    /// Gets or sets the file native data.
    /// </summary>
    /// <value>The file native data.</value>
    public byte[] FileNativeData
    {
      get
      {
        return m_fileNativeData;
      }
      set
      {
        m_fileNativeData = value;
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is container.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is container; otherwise, <c>false</c>.
    /// </value>
    public bool IsContainer
    {
      get
      {
        return m_isContainer;
      }
      set
      {
        m_isContainer = value;
      }
    }
    /// <summary>
    /// Gets or sets the type of the object.
    /// </summary>
    /// <value>The type of the object.</value>
    [Obsolete("This property has been depreceated. Use the OleObjectType property instead.")]
    public string ObjectType
    {
      get
      {
        return m_objectType;
      }
      set
      {
        m_objectType = value;
      }
    }
    /// <summary>
    /// Gets or sets the type of the OLE object.
    /// </summary>
    /// <value>The type of the OLE object.</value>
    public OleObjectType OleObjectType
    {
        get
        {
            return m_oleObjectType;
        }
        set
        {
            m_oleObjectType = value;
        }
    }
    /// <summary>
    /// Gets the locate.
    /// </summary>
    /// <value>The locate.</value>
    public Dictionary<string, int> Locate
    {
      get
      {
        return m_location;
      }
    }
    /// <summary>
    /// Gets or sets the container.
    /// </summary>
    /// <value>The container.</value>
    public Stream Container
    {
      get
      {
        if( m_container == null )
        {
          m_container = GetOleContainer();
        }

        return m_container;
      }
      set
      {
        m_container = value;
      }
    }
    /// <summary>
    /// Gets or sets the name of the storage.
    /// </summary>
    /// <value>The name of the storage.</value>
    public string StorageName
    {
      get
      {
        return m_storageName;
      }
      set
      {
        m_storageName = value;
      }
    }
    /// <summary>
    /// Gets or sets the name of the file.
    /// </summary>
    /// <value>The name of the file.</value>
    public string FileName
    {
      get
      {
        return m_fileName;
      }
      set
      {
        m_fileName = value;
      }
    }
    /// <summary>
    /// Gets or sets the dv aspect.
    /// </summary>
    /// <value>The dv aspect.</value>
    public DVAspect DvAspect
    {
      get
      {
        return m_dvAspect;
      }
      set
      {
        m_dvAspect = value;
      }
    }
    /// <summary>
    /// Gets or sets the shape ID.
    /// </summary>
    /// <value>The shape ID.</value>
    public int ShapeID
    {
      get
      {
        return m_shape.ShapeId;
      }
      set
      {
        m_shape = m_sheet.InnerShapes.GetShapeById( value ) as ShapeImpl;
        //m_shape.ShapeId = value;
      }
    }
    /// <summary>
    /// Gets or sets the shape R id.
    /// </summary>
    /// <value>The shape R id.</value>
    public string ShapeRId
    {
      get
      {
        return m_shapeRId;
      }
      set
      {
        m_shapeRId = value;
      }
    }
    /// <summary>
    /// Gets shape associated with this ole object.
    /// </summary>
    public IPictureShape Shape
    {
      get
      {
        return m_shape as IPictureShape;
      }
      set
      {
        m_shape = ( ShapeImpl )value;
        m_shape.VmlShape = true;
      }
    }
    /// <summary>
    /// Gets or sets xlsx content type.
    /// </summary>
    public string ContentType
    {
      get
      {
        return m_strContentType;
      }
      set
      {
        m_strContentType = value;
      }
    }
    /// <summary>
    /// Gets or sets xlsx relation type.
    /// </summary>
    public string RelationType
    {
      get
      {
        return m_strRelationType;
      }
      set
      {
        m_strRelationType = value;
      }
    }
    #endregion

    #region Methods

    #region Initialize Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="OleObject"/> class.
    /// </summary>
    public OleObject( WorksheetImpl sheet )
    {
      if( sheet == null )
        throw new ArgumentNullException();

      m_sheet = sheet;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="OleObject"/> class.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="image">The image.</param>
    public OleObject( string fileName, Image image )
    {
      if( File.Exists( fileName ) )
      {
        CheckFileName( fileName );
        if( image == null )
        {
          throw new ArgumentNullException( "Image" );
        }
        Picture = image;
        FileName = fileName;
        DisplayAsIcon = true;
        SetFile( fileName );
      }
      else
      {
        throw new Exception( "Values Should Be passed" );
      }
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="OleObject"/> class.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="image">The image.</param>
    /// <param name="oleLinkType">Type of the OLE link.</param>
    public OleObject( string fileName, Image image, OleLinkType oleLinkType )
    {
      if( File.Exists( fileName ) )
      {
        CheckFileName( fileName );
        if( image == null )
        {
          throw new ArgumentNullException( "Image" );
        }
        Picture = image;
        FileName = Path.GetFullPath( fileName );
        OleType = oleLinkType;
        DisplayAsIcon = true;
        SetFile( fileName );
      }
      else
      {
        throw new Exception( "Values Should Be passed" );
      }
    }
    public OleObject( string fileName, IPictureShape shape, OleLinkType linkType )
    {
      if( shape == null )
        throw new ArgumentNullException( "shape" );

      m_sheet = ( shape as ShapeImpl ).Worksheet as WorksheetImpl;

      if( File.Exists( fileName ) )
      {
        CheckFileName( fileName );

        Shape = shape;
        FileName = Path.GetFullPath( fileName );
        OleType = linkType;
        DisplayAsIcon = true;
        SetFile( fileName );
      }
      else
      {
        throw new ArgumentException( "File not found." );
      }
    }
    /// <summary>
    /// OLEs from file.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="image">The image.</param>
    /// <returns></returns>
    public static OleObject OleFromFile( string fileName, Image image )
    {
      OleObject oleObject = new OleObject( fileName, image );
      return oleObject;
    }
    /// <summary>
    /// OLEs from stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="image">The image.</param>
    /// <param name="extension">The extension.</param>
    /// <returns></returns>
    public static OleObject OleFromStream( Stream stream, Image image, string extension )
    {
      OleObject oleObject = new OleObject( stream, image, extension );
      return oleObject;
    }
    #endregion

    #region Temporarily Skipped(needs to be implemented)

    /// <summary>
    /// Initializes a new instance of the <see cref="OleObject"/> class.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <param name="image">The image.</param>
    /// <param name="extension">The extension.</param>
    public OleObject( Stream stream, Image image, string extension )
    {
      if( ( stream.CanSeek ) && ( stream.CanRead ) )
      {
        if( image == null )
        {
          throw new ArgumentNullException( "Image" );
        }
        IsStream = true;
        Picture = image;
        byte[] tempFile = new Byte[ stream.Length ];
        stream.Read( tempFile, 0, tempFile.Length );
        stream.Close();
        m_fileName = GetStreamFileName( tempFile, extension );
        using( FileStream fs = new FileStream( m_fileName, FileMode.Open, FileAccess.Read ) )
        {
          m_file = new Byte[ fs.Length ];
          fs.Read( m_file, 0, m_file.Length );
        }
        StorageName = OleTypeConvertor.GetOleFileName();
        OleType = OleLinkType.Embed;
        DisplayAsIcon = true;
        SetOleFile( m_fileName, StorageName );
      }
      else
      {
        throw new Exception( "Stream" );
      }
    }
    /// <summary>
    /// Gets the name of the stream file.
    /// </summary>
    /// <param name="readValue">The read value.</param>
    /// <param name="extension">The extension.</param>
    /// <returns></returns>
    internal string GetStreamFileName( byte[] readValue, string extension )
    {
      string path = System.IO.Path.ChangeExtension( System.IO.Path.GetTempPath() + Guid.NewGuid().ToString(), extension );
      path = path.Replace( '\\', '/' );
      FileStream fs = new FileStream( path, FileMode.Create, FileAccess.ReadWrite );
      fs.Write( readValue, 0, readValue.Length );
      fs.Close();
      return path;
    }

    #endregion

    #region Setting the Values
    /// <summary>
    /// Sets the file.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <returns></returns>
    internal string SetFile( string fileName )
    {
      if( File.Exists( fileName ) )
      {
        using( FileStream stream = new FileStream( fileName, FileMode.Open ) )
        {
          m_file = new Byte[ stream.Length ];
          stream.Read( m_file, 0, m_file.Length );
        }

        m_fileName = fileName;
      }
      else
      {
        throw new Exception( "The File does not exists in the path, please ensure the existence of the file" );
      }
      return fileName;
    }

    #endregion

    #region Extract Ole File From Exsisting Excel File
    /// <summary>
    /// Extracts the OLE data.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    public void ExtractOleData( string fileName )
    {
      CheckFileName( fileName );
      using( FileStream fs = new FileStream( fileName, FileMode.CreateNew, FileAccess.Write, FileShare.None ) )
      {
        Container.Position = 0;
        byte[] temp = new Byte[ Container.Length - 1536 ];
        Container.Read( temp, 1537, temp.Length );
        fs.Write( temp, 0, temp.Length );
      }
    }
    /// <summary>
    /// Copies the stream.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="output">The output.</param>
    private static void CopyStream( Stream input, Stream output )
    {
      byte[] buffer = new byte[ 2000 ];
      int len;
      while( ( len = input.Read( buffer, 0, 2000 ) ) > 0 )
      {
        output.Write( buffer, 0, len );
      }
      output.Flush();
    }
    #endregion

    #region Checking the File Validation

    /// <summary>
    /// Checks the name of the file.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    private void CheckFileName( string fileName )
    {
      bool nameTooLong = false;

      if( fileName.Length >= 252 )
        nameTooLong = true;
      else
      {
        string directoryName = Path.GetDirectoryName( fileName );
        if( directoryName.Length >= 248 )
          nameTooLong = true;
      }

      if( nameTooLong )
        throw new PathTooLongException( "The file name is too long. The fully qualified file name must be less " +
          "than 260 characters and the directory name must be less than 248 characters" );
    }

    #endregion

    #region Creation of OleObject
    /// <summary>
    /// Sets the OLE file.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="storageName">Name of the storage.</param>
    internal void SetOleFile( string fileName, string storageName )
    {
      if( storageName != null )
      {
        CreateOleObjContainer( fileName, storageName );
      }
    }
    /// <summary>
    /// Sets the OLE file.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="storageName">Name of the storage.</param>
    /// <param name="fileBytes">The file bytes.</param>
    internal void SetOleFile( string fileName, string storageName, byte[] fileBytes )
    {
      if( storageName != null )
      {
        m_file = fileBytes;
        CreateOleObjContainer( fileName, storageName );
      }
    }
    /// <summary>
    /// Creates the OLE obj container.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <param name="olestorageName">Name of the olestorage.</param>
    internal void CreateOleObjContainer( string filePath, string olestorageName )
    {
      byte[] objectPool = CreateOrGetObjPool( olestorageName );
      Save( objectPool, filePath, olestorageName );
      if( IsStream )
      {
        if( File.Exists( filePath ) )
        {
          File.Delete( filePath );
        }
      }
    }

    /// <summary>
    /// Saves the specified object pool.
    /// </summary>
    /// <param name="objectPool">The object pool.</param>
    /// <param name="dataPath">The data path.</param>
    /// <param name="storageName">Name of the storage.</param>
    /// <returns></returns>
    internal void Save( byte[] objectPool, string dataPath, string storageName )
    {
      MemoryStream objPoolStream = new MemoryStream( objectPool );
      StgStream baseStream = new StgStream( objPoolStream, StgStream.DEF_READWRITE );
      StgStream compStgStream = null;

      // Open storage
      compStgStream = baseStream.OpenSubStorage( "ObjectPool", StgStream.DEF_READWRITE );
      StgStream rootStg = compStgStream.OpenSubStorage( storageName, StgStream.DEF_READWRITE );

      // Write "Ole" stream if needed 
      WriteOleStream( rootStg, dataPath );
      WriteObjInfoStream( rootStg );
      WriteCompObjStream( rootStg );
      WritePackage( rootStg, dataPath );

      compStgStream.Flush();
      baseStream.Flush();
      rootStg.Close();
      rootStg.Dispose();      
      MemoryStream outStream = new MemoryStream();
      baseStream.SaveILockBytesIntoStream( outStream );
      outStream.Flush();
      m_fileNativeData = outStream.ToArray();
      
     //  Close streams

      outStream.Close();
      outStream.Dispose();
      baseStream.Close();
      baseStream.Dispose();
      compStgStream.Close();
      compStgStream.Dispose();
     
      
    }



    /// <summary>
    /// Creates the or get obj pool.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    private byte[] CreateOrGetObjPool( string name )
    {
      try
      {
        MemoryStream memStream = null;
        ICompoundStorage objPool = null;
        CompoundFile.XlsIO.Net.CompoundFile storage = null;

        StgStream pool = null;
        StgStream poolStg = null;

        if( FileNativeData == null || FileNativeData.Length == 0 )
        {
          pool = StgStream.CreateStorageOnILockBytes();
          poolStg = pool.CreateSubStorage( DEF_OBJECT_POOL_NAME );
        }
        else
        {
          memStream = new MemoryStream( FileNativeData );
          pool = new StgStream( memStream );
          poolStg = pool.OpenSubStorage( DEF_OBJECT_POOL_NAME, StgStream.DEF_READWRITE );
        }

        storage = new Syncfusion.CompoundFile.XlsIO.Net.CompoundFile();
        storage.RootStorage.CreateStorage( name );
        ( storage.Directory.Entries[ 1 ] as DirectoryEntry ).StorageGuid = OleTypeConvertor.GetGUID();
        MemoryStream storageStream = new MemoryStream();
        storage.Flush();
        storage.Save( storageStream );
        storage.Dispose();
        storageStream.Flush();
        byte[] bytes = storageStream.ToArray();
        storageStream.Close();
        storageStream = new MemoryStream( bytes );

        MemoryStream stream = new MemoryStream();
        StgStream stg = new StgStream( storageStream );
        StgStream storageStg = stg.OpenSubStorage( name );
        StgStream.CopySourceStorages( storageStg, poolStg );

        pool.Flush();
        pool.SaveILockBytesIntoStream( stream );
        stream.Position = 0;
        m_fileNativeData = stream.ToArray();

        stg.Close();
        stg.Dispose();
        storageStg.Close();
        storageStg.Dispose();
        pool.Close();
        pool.Dispose();
        poolStg.Close();
        poolStg.Dispose();

        stream.Close();
        stream.Dispose();
        storageStream.Close();
        storageStream.Dispose();
        if( memStream != null )
        {
          memStream.Close();
          memStream.Dispose();
        }
      }
      catch( Exception )
      {
      }
      return m_fileNativeData;
    }
    /// <summary>
    /// Writes the OLE stream.
    /// </summary>
    /// <param name="rootStg">The root STG.</param>
    /// <param name="dataPath">The data path.</param>
    private void WriteOleStream( StgStream rootStg, String dataPath )
    {
      rootStg.CreateStream( DEF_OLE_STREAM_NAME );
      m_oleStream = new OleStream( dataPath );
      m_oleStream.SaveTo( rootStg );
      rootStg.Close();
    }
    /// <summary>
    /// Writes the obj info stream.
    /// </summary>
    /// <param name="rootStg">The root STG.</param>
    private void WriteObjInfoStream( StgStream rootStg )
    {
      rootStg.CreateStream( DEF_INFO_STREAM_NAME );
      m_objectInfoStream = new ObjectInfoStream();
      m_objectInfoStream.SaveTo( rootStg );
      rootStg.Close();
    }
    /// <summary>
    /// Gets the OLE container.
    /// </summary>
    /// <returns></returns>
    private Stream GetOleContainer()
    {
      if( FileNativeData == null || FileNativeData.Length == 0 )
        return null;

      MemoryStream objPoolStream = new MemoryStream( FileNativeData );
      StgStream objPoolStg = null;
      StgStream objectPool = null;
      StgStream oleContainer = null;
      StgStream destStream = null;
      MemoryStream memStream = null;

      try
      {
        objPoolStream = new MemoryStream( FileNativeData );
        objPoolStg = new StgStream( objPoolStream );
        objectPool = objPoolStg.OpenSubStorage( "ObjectPool" );
        oleContainer = objectPool.OpenSubStorage( StorageName.ToString() );

        destStream = StgStream.CreateStorageOnILockBytes();
        StgStream.CopySourceStorages( oleContainer, destStream );

        memStream = new MemoryStream();
        destStream.SaveILockBytesIntoStream( memStream );
        memStream.Position = 0;
      }
      catch( Exception e )
      {
      }
      finally
      {
        if( objPoolStream != null )
        {
          objPoolStream.Close();
          objPoolStream.Dispose();
        }

        if( objPoolStg != null )
        {
          objPoolStg.Close();
          objectPool.Dispose();
        }

        if( objectPool != null )
        {
          objectPool.Close();
          objectPool.Dispose();
        }

        if( oleContainer != null )
        {
          oleContainer.Close();
          oleContainer.Dispose();
        }

        if( destStream != null )
        {
          destStream.Close();
          destStream.Dispose();
        }
      }

      return memStream as Stream;
    }
    /// <summary>
    /// Writes the comp obj stream.
    /// </summary>
    /// <param name="rootStg">The root STG.</param>
    private void WriteCompObjStream( StgStream rootStg )
    {
      if( !ContainStream( rootStg.Streams, DEF_COMP_STREAM_NAME ) )
      {
        rootStg.CreateStream( DEF_COMP_STREAM_NAME );
        m_compObjectStream = new CompObjectStream();
        m_compObjectStream.SaveTo( rootStg );
        rootStg.Close();
      }
    }
    /// <summary>
    /// Writes the package.
    /// </summary>
    /// <param name="rootStg">The root STG.</param>
    /// <param name="dataPath">The data path.</param>
    private void WritePackage( StgStream rootStg, string dataPath )
    {
      ASCIIEncoding asciiEnc = new ASCIIEncoding();
      string fileName = Path.GetFileName( dataPath );
      byte[] nameBytes = asciiEnc.GetBytes( fileName );
      byte[] pathBytes = asciiEnc.GetBytes( dataPath );
      byte[] marker1 = new byte[ 2 ] { 2, 0 };
      byte[] marker2 = new byte[ 4 ] { 0, 0, 3, 0 };

      // Data length 
      int dataLen = ExcelConstants.IntSize;
      // 2-bytes marker
      dataLen += marker1.Length;
      // File name length + "\0"
      dataLen += nameBytes.Length + 1;
      // File path length + "\0"
      dataLen += pathBytes.Length + 1;
      // 4-bytes marker
      dataLen += marker2.Length;
      // "DOS" file path length
      dataLen += ExcelConstants.IntSize;
      // "DOS" file path + "\0"
      dataLen += pathBytes.Length + 1;
      // Native data length
      dataLen += ExcelConstants.IntSize;
      dataLen += m_file.Length;
      // "\0\0" at the end
      dataLen += 2;

      int iOffset = 0;
      byte[] data = new byte[ dataLen ];
      // Write data length
      DataStructure.WriteInt32( data, ref iOffset, dataLen - ExcelConstants.IntSize );
      // Write marker 1
      DataStructure.WriteBytes( data, ref iOffset, marker1 );
      // Write file name
      DataStructure.WriteBytes( data, ref iOffset, nameBytes );
      iOffset += 1; // "\0"
      // Write path
      DataStructure.WriteBytes( data, ref iOffset, pathBytes );
      iOffset += 1; // "\0"
      // Write marker 2
      DataStructure.WriteBytes( data, ref iOffset, marker2 );
      // Write path name length
      DataStructure.WriteInt32( data, ref iOffset, pathBytes.Length + 1 );
      // Write path
      DataStructure.WriteBytes( data, ref iOffset, pathBytes );
      iOffset += 1; // "\0"
      // Write native data length
      DataStructure.WriteInt32( data, ref iOffset, m_file.Length );
      // Write native data
      DataStructure.WriteBytes( data, ref iOffset, m_file );
      GC.SuppressFinalize(m_file);
      m_file = null;
      // Write data to stream
      rootStg.CreateStream( DEF_NATIVE_STREAM_NAME );
      rootStg.Write( data, 0, data.Length );
      rootStg.Close();
      data = null;
    }
    /// <summary>
    /// Contains the stream.
    /// </summary>
    /// <param name="streamNames">The stream names.</param>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    private bool ContainStream( String[] streamNames, String name )
    {
      bool containStream = false;
      for( int i = 0, cnt = streamNames.Length; i < cnt; i++ )
      {
        if( streamNames[ i ] == name )
        {
          containStream = true;
          break;
        }
      }
      return containStream;
    }
    /// <summary>
    /// Gets index of external workbook referenced by this ole object.
    /// </summary>
    /// <returns></returns>
    public int GetWorkbookIndex()
    {
      ExternWorkbookImpl book = m_sheet.ParentWorkbook.ExternWorkbooks[ m_fileName ];
      return book.Index;
    }
    #endregion

    #endregion

  }

    //TODO:- Remove this below class once implemented the complete OLEOBJECT parsing and serialization.
    /// <summary>
    /// Used to preserve OleObjects for add copy method
    /// </summary>
  public class OleStorage
  {
      #region constants

      

      #endregion

      private Dictionary<string, MemoryStream> m_arrayStreams = new Dictionary<string, MemoryStream>();
      private List<string> m_streamNames = new List<string>();

      private string m_storageName;

      public string StorageName
      {
          get { return m_storageName; }
      }
      public Dictionary<string, MemoryStream> StorageStreams
      {
          get { return m_arrayStreams; }
      }

      public OleStorage(string Name)
      {
          m_storageName = Name;
      }
      public List<string> StreamNames
      {
          get { return m_streamNames; }
      }

      internal void ParseStream(CompoundStream oleStream)
      {
          MemoryStream m_controlsStream = new MemoryStream((int)oleStream.Length);
          UtilityMethods.CopyStreamTo(oleStream, m_controlsStream);

          oleStream.Position = 0;

          Add(oleStream.Name, m_controlsStream);
          m_streamNames.Add(oleStream.Name);
      }
      internal MemoryStream OpenStream(string streamName)
      {
          return m_arrayStreams[streamName];
      }

      private void Add(string StreamName, MemoryStream ControlStream)
      {
          m_arrayStreams.Add(StreamName, ControlStream);
      }
  }
 }

