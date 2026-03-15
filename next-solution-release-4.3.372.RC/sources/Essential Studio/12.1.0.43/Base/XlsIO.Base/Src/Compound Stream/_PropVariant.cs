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
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;

using Syncfusion.XlsIO.IO.Stream.Win32;
#endregion

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// The PropVariant is used for defining the type tag and 
  /// the value of a property in a property set.
  /// </summary>
  [Syncfusion.Documentation.DocumentationExclude()]
  public class PropVariant : IDisposable
  {
    #region Class constants
    /// <summary>
    /// Size of the native windows PROPVARIANT structure.
    /// </summary>
    public static readonly int PropVariantSize = 8 + IntPtr.Size * 2;
    /// <summary>
    /// Offset to the type of the PropVariant.
    /// </summary>
    public const int TYPE_OFFSET = 0;
    /// <summary>
    /// Offset to the first int of the data.
    /// </summary>
    public const int FirstIntOffset = 8;
    /// <summary>
    /// Offset to the second int of the data.
    /// </summary>
    public static readonly int SecondIntOffset = FirstIntOffset + IntPtr.Size;
    /// <summary>
    /// Size of the integer.
    /// </summary>
    private const int IntSize = 4;
    /// <summary>
    /// Mask to get type of property or each element of the arrya (if property contains an array).
    /// </summary>
    private const int DEF_SHORT_PROPERTY_TYPE_MASK = 0xFF;
    /// <summary>
    /// Bit mask for lower int value.
    /// </summary>
    private const long DEF_LOW_INT_MASK = 0xFFFFFFFF;
    /// <summary>
    /// Bit mask for higher int value.
    /// </summary>
    private const ulong DEF_HIGH_INT_MASK = 0xFFFFFFFF00000000;
    /// <summary>
    /// Number of bits in every integer value.
    /// </summary>
    private const int DEF_INT_BITS = IntSize * 8;
    /// <summary>
    /// Difference in ticks of FILETIME and DateTime.
    /// </summary>
    private const long DEF_FILETIME_TICKS_DIFFERENCE = 0x701ce1722770000;
    /// <summary>
    /// Bit mask for LinkToContent property of the DocumentProperty class. 
    /// </summary>
    internal const int DEF_LINK_BIT = 0x1000000;
    #endregion

    #region Class members
    /// <summary>
    /// Array of IntPtr that should be freed on dispose using Marshal.FreeCoTaskMem.
    /// </summary>
    private ArrayList m_arrFree = new ArrayList();
    /// <summary>
    /// Array of IntPtr that should be freed on dispose using Marshal.FreeHGlobal.
    /// </summary>
    private ArrayList m_arrGlobalFree = new ArrayList();
    /// <summary>
    /// Pointer to the PropVariant.
    /// </summary>
    private IntPtr m_propVariant = Marshal.AllocHGlobal( PropVariantSize );
    /// <summary>
    /// Specifies a property by its property identifier (ID).
    /// </summary>
    private tagPROPSPEC m_prop = new tagPROPSPEC();
    /// <summary>
    /// Array of PropVariants that will be disposed in Dispose method.
    /// </summary>
    private ArrayList m_arrDispose = new ArrayList();
    /// <summary>
    /// If True, then memory for the structure was allocated by 
    /// this class and should be freed on Dispose;
    /// otherwise, memory was not allocated by this class and should not be freed.
    /// </summary>
    private bool m_bFreeVariant = true;
    #endregion

    #region Class constructors
    /// <summary>
    /// Default constructor.
    /// </summary>
    public PropVariant()
    {
      m_prop.ulKind = ( IntPtr )PRSPEC.PRSPEC_PROPID;
      // Zero memory.
      for( int i = 0, j=0; i <PropVariantSize / 4; i++, j+=4 )
      {
        Marshal.WriteInt32( m_propVariant, j, 0 );
      }
    }
    /// <summary>
    /// Creates PropVariant with data pointed by ptr.
    /// </summary>
    /// <param name="ptr"></param>
    public PropVariant( IntPtr ptr ) : this()
    {
      IntPtr = ptr;
    }
    /// <summary>
    /// Reads data from IPropertyStorage.
    /// </summary>
    /// <param name="propInfo">Property description.</param>
    /// <param name="propStorage">IPropertyStorage to read data from.</param>
    /// <param name="bBuiltIn">Indicates whether property is built-in or not.</param>
    [ CLSCompliant( false ) ]
    public PropVariant( tagSTATPROPSTG propInfo, IPropertyStorage propStorage, bool bBuiltIn )
      : this()
    {
      Read( propInfo, propStorage, bBuiltIn );
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Fills PropVariant with integer value.
    /// </summary>
    public int Int
    {
      get
      {
        return FirstInt.ToInt32();
      }
      set
      {
        FreeResources();
        Type = VarEnum.VT_INT;
        FirstInt = ( IntPtr )value;
        SecondInt = IntPtr.Zero;
      }
    }
    /// <summary>
    /// Fills PropVariant with integer value.
    /// </summary>
    public int Int32
    {
      get
      {
        return FirstInt.ToInt32();
      }
      set
      {
        FreeResources();
        Type = VarEnum.VT_I4;
        FirstInt = ( IntPtr )value;
        SecondInt = IntPtr.Zero;
      }
    }
    /// <summary>
    /// Gets / sets PropVariant memory.
    /// </summary>
    public IntPtr   IntPtr
    {
      get
      {
        return m_propVariant;
      }
      set
      {
        FreeResources();

        if( m_propVariant != value )
        {
          m_propVariant = value;
          m_bFreeVariant = false;
        }
      }
    }
    /// <summary>
    /// ID of the property that will be written into property storage.
    /// </summary>
    public PIDSI    PropId
    {
      get
      {
        return ( PIDSI )m_prop.propid;
      }
      set
      {
        m_prop.ulKind = ( IntPtr )PRSPEC.PRSPEC_PROPID;
        m_prop.propid = ( IntPtr )value;
      }
    }
    /// <summary>
    /// Same as PropId.
    /// </summary>
    public PIDDSI   PropId2
    {
      get
      {
        return (PIDDSI) m_prop.propid;
      }
      set
      {
        m_prop.propid = ( IntPtr ) value;
      }
    }
    /// <summary>
    /// Fills PropVariant with FILETIME value.
    /// </summary>
    public System.Runtime.InteropServices.ComTypes.FILETIME FileTime
    {
      get
      {
        System.Runtime.InteropServices.ComTypes.FILETIME result =
          new System.Runtime.InteropServices.ComTypes.FILETIME();
        long lTimeValue = Marshal.ReadInt64( m_propVariant, FirstIntOffset );
        result.dwLowDateTime = ( int )( lTimeValue & uint.MaxValue );
        result.dwHighDateTime = ( int )( ( lTimeValue >> 32 ) & uint.MaxValue );//( int )( ( lTimeValue & uint.MaxValue ) >> 32 );
        
        return result;
      }
      set
      {
        Type = VarEnum.VT_FILETIME;
        long lTimeValue = ( ( ( long )value.dwHighDateTime ) << 32 ) + ( uint )value.dwLowDateTime;
        Marshal.WriteInt64( m_propVariant, FirstIntOffset, lTimeValue );
      }
    }
    /// <summary>
    /// Fills PropVariant with bool value.
    /// </summary>
    public bool     Bool
    {
      get
      {
        return ( FirstInt != IntPtr.Zero );
      }
      set
      {
        Type = VarEnum.VT_BOOL;
        FirstInt = ( IntPtr )( value ? 1 : 0 );
        SecondInt = IntPtr.Zero;
      }
    }
    /// <summary>
    /// Fills PropVariant with string value.
    /// </summary>
    public string   String
    {
      get
      {
        return Marshal.PtrToStringUni( FirstInt );
      }
      set
      {
        //FreeResources();
        IntPtr temp = Marshal.StringToHGlobalUni( value );
        Type = VarEnum.VT_LPWSTR;
        FirstInt = temp;
        SecondInt = IntPtr.Zero;
        m_arrFree.Add( temp );
      }
    }
    /// <summary>
    /// Fills PropVariant with FILETIME value.
    /// </summary>
    public DateTime DateTime
    {
      get
      {
        System.Runtime.InteropServices.ComTypes.FILETIME fileTime = FileTime;
        long lFileTime = ( long )( ( ( ulong )fileTime.dwHighDateTime << DEF_INT_BITS ) + ( uint )fileTime.dwLowDateTime );
        lFileTime += DEF_FILETIME_TICKS_DIFFERENCE;

        DateTime result = new DateTime( lFileTime );

        if( PropId != PIDSI.EditTime )
          result = result.ToLocalTime();

        return result;
      }
      set
      {
        if( PropId != PIDSI.EditTime )
          value = value.ToUniversalTime();
        
        ulong ulFileTime = ( ulong )( value.Ticks - DEF_FILETIME_TICKS_DIFFERENCE );
        System.Runtime.InteropServices.ComTypes.FILETIME fileTime =
          new System.Runtime.InteropServices.ComTypes.FILETIME();
        fileTime.dwHighDateTime = ( int )( ( ulFileTime & DEF_HIGH_INT_MASK ) >> DEF_INT_BITS );
        fileTime.dwLowDateTime = ( int )( ( ulFileTime & DEF_LOW_INT_MASK ) );

        FileTime = fileTime;
      }
    }
    /// <summary>
    /// Fills PropVariant with double value.
    /// </summary>
    public double Double
    {
      get
      {
        long lValue = Marshal.ReadInt64( m_propVariant, FirstIntOffset );
        return BitConverter.Int64BitsToDouble( lValue );
      }
      set
      {
        Type = VarEnum.VT_R8;
        long lValue = BitConverter.DoubleToInt64Bits( value );
        Marshal.WriteInt64( m_propVariant, FirstIntOffset, lValue );
      }
    }
    /// <summary>
    /// Gets / sets property name.
    /// </summary>
    public string Name
    {
      get
      {
        if( m_prop.ulKind == ( IntPtr )PRSPEC.PRSPEC_LPWSTR )
        {
          IntPtr temp = ( IntPtr )m_prop.propid;
          return Marshal.PtrToStringUni( temp );
        }

        return null;
      }
      set
      {
        if( value == null )
          throw new ArgumentNullException( "value" );

        SetName( value );
      }
    }
    /// <summary>
    /// Returns value of the property. Read-only.
    /// </summary>
    public object Value
    {
      get
      {
        return GetValue();
      }
    }
    /// <summary>
    /// Indicates whether it is property or just link to source of some property. Read-only.
    /// </summary>
    public bool IsLinkToSource
    {
      get
      {
        return m_prop.ulKind == ( IntPtr )PRSPEC.PRSPEC_PROPID && ( ( m_prop.propid.ToInt32() & DEF_LINK_BIT ) != 0 );
      }
    }
    /// <summary>
    /// Returns id of the parent property. Read-only.
    /// </summary>
    public int ParentId
    {
      get
      {
        return ( int )( IsLinkToSource
          ? m_prop.propid.ToInt32() - DEF_LINK_BIT
          : m_prop.propid.ToInt32() );
      }
    }
    #endregion

    #region Class get methods
    /// <summary>
    /// Returns array of strings.
    /// </summary>
    public string[] GetStringArray()
    {
      int iCount = FirstInt.ToInt32();
      string[] arrResult = new string[ iCount ];
      IntPtr ptrStart = SecondInt;

      for( int i = 0; i < iCount; i++ )
      {
        IntPtr ptrString = Marshal.ReadIntPtr( ptrStart, i * IntPtr.Size );
        arrResult[ i ] = GetString( ptrString );
      }

      return arrResult;
    }
    /// <summary>
    /// Converts IntPtr to the string.
    /// </summary>
    /// <param name="ptrString">Value to convert.</param>
    /// <returns>Converted string.</returns>
    private string GetString( IntPtr ptrString )
    {
      if( ptrString == IntPtr.Zero )
        throw new ArgumentNullException( "ptrString" );

      int iType = ( int )Type & DEF_SHORT_PROPERTY_TYPE_MASK;
      int iDesiredMask = ( int )VarEnum.VT_LPSTR;

      return ( iType == iDesiredMask )
        ? GetShortString( ptrString )
        : Marshal.PtrToStringUni( ptrString );
    }
    /// <summary>
    /// Parses not unicode string.
    /// </summary>
    /// <param name="ptrString">Pointer to the sring to parse.</param>
    /// <returns></returns>
    private string GetShortString( IntPtr ptrString )
    {
      if( ptrString == IntPtr.Zero )
        throw new ArgumentNullException( "ptrString" );

      int iLength = 0;

      while( Marshal.ReadByte( ptrString, iLength ) != 0 ) iLength++;

      byte[] arrBuffer = new byte[ iLength ];
      Marshal.Copy( ptrString, arrBuffer, 0, iLength );

      string strResult = Encoding.Default.GetString( arrBuffer );
      return strResult;
    }
    /// <summary>
    /// Returns an array of objects.
    /// </summary>
    public  object[] GetObjectArray()
    {
      int iCount = FirstInt.ToInt32();
      object[] arrResult = new object[ iCount ];
      IntPtr ptrStartData = ( IntPtr )SecondInt;

      for( int i = 0, iOffset = 0; i < iCount; i++, iOffset += PropVariantSize )
      {
        PropVariant variant = new PropVariant( ( IntPtr )( SecondInt.ToInt64() + iOffset ) );
        arrResult[ i ] = variant.GetValue();
      }

      return arrResult;
    }
    #endregion

    #region Class Set methods
    /// <summary>
    /// Fills PropVariant with array of strings.
    /// </summary>
    public void SetStringArray( string[] value )
    {
      FreeResources();

      int count = value.Length;
      IntPtr[] lpstr = new IntPtr[ count ];
      IntPtr ptrStrArray = Marshal.AllocHGlobal( count * IntPtr.Size );
      m_arrGlobalFree.Add( ptrStrArray );

      // Allocating memory for all strings.
      for( int i = 0; i < count; i++ )
      {
        lpstr[ i ] = Marshal.StringToHGlobalUni( value[ i ] );
        m_arrFree.Add( lpstr[ i ] );
      }

      // Write all strings into vector.
      for( int i = 0, j = 0; j < count; i += IntPtr.Size, j++ )
      {
        Marshal.WriteIntPtr( ptrStrArray, i, lpstr[ j ] );
      }

      Type = VarEnum.VT_LPWSTR | VarEnum.VT_VECTOR;
      FirstInt = ( IntPtr )count;
      SecondInt = ptrStrArray;
    }
    /// <summary>
    /// Fills PropVariant with array of objects.
    /// </summary>
    public void SetObjectArray( object[] value )
    {
      FreeResources();

      Type = VarEnum.VT_VECTOR | VarEnum.VT_VARIANT;
      int count = value.Length;
      PropVariant variant;
      IntPtr ptrArray = Marshal.AllocHGlobal( PropVariantSize * count );
      m_arrGlobalFree.Add( ptrArray );
        
      for( int i = 0, j = 0; i < count; i++, j+= PropVariantSize )
      {
        if( value[ i ] is int )
        {
          variant = new PropVariant( ( IntPtr )( ptrArray.ToInt64() + j ) );
            
          variant.Int = ( int )value[ i ];
          variant.Type = VarEnum.VT_I4;
          m_arrDispose.Add( variant );
        }
        else if( value[ i ] is string )
        {
          variant = new PropVariant( ( IntPtr )( ptrArray.ToInt64() + j ) );

          variant.String = ( string)value[ i ];
          m_arrDispose.Add( variant );
        }
      }

      FirstInt = ( IntPtr )count;
      SecondInt = ptrArray;
    }
    /// <summary>
    /// Sets Blob property value.
    /// </summary>
    /// <param name="value">Value to set.</param>
    public void SetBlob( byte[] value )
    {
      if( value == null )
        throw new ArgumentNullException( "value" );

      FreeResources();

      Type = VarEnum.VT_BLOB;
      int iLength = value.Length;
      FirstInt = ( IntPtr )iLength;

      IntPtr ptrArray = Marshal.AllocHGlobal( iLength );
      Marshal.Copy( value, 0, ptrArray, iLength );
      SecondInt = ptrArray;

      m_arrGlobalFree.Add( ptrArray );
    }
    /// <summary>
    /// Sets property name.
    /// </summary>
    /// <param name="strName">Name to set.</param>
    public void SetName( string strName )
    {
      if( strName == null )
        throw new ArgumentNullException( "strName" );

      if( strName.Length == 0 )
        throw new ArgumentException( "strName - string cannot be empty." );

      FreeName();

      m_prop.ulKind = ( IntPtr )PRSPEC.PRSPEC_LPWSTR;
      IntPtr name = Marshal.StringToHGlobalUni( strName );
      m_prop.propid = name;
    }
    #endregion

    #region Class Not Public Properties
    /// <summary>
    /// Sets first integer value of the variant. Write-only.
    /// </summary>
    private IntPtr FirstInt
    {
      get
      {
        return Marshal.ReadIntPtr( m_propVariant, FirstIntOffset );
      }
      set
      {
        Marshal.WriteIntPtr( m_propVariant, FirstIntOffset, value );
      }
    }
    /// <summary>
    /// Sets second integer value of the variant. Write-only.
    /// </summary>
    private IntPtr SecondInt
    {
      get
      {
        return Marshal.ReadIntPtr( m_propVariant, SecondIntOffset );
      }
      set
      {
        Marshal.WriteIntPtr( m_propVariant, SecondIntOffset, value );
      }
    }
    /// <summary>
    /// Sets type of the variant. Write-only.
    /// </summary>
    internal VarEnum Type
    {
      get
      {
        return ( VarEnum )Marshal.ReadInt16( m_propVariant, TYPE_OFFSET );
      }
      set
      {
        Marshal.WriteInt16( m_propVariant, TYPE_OFFSET, ( short )value );
      }
    }
    #endregion

    #region Class Not Public Methods
    /// <summary>
    /// Frees all allocated resources.
    /// </summary>
    private void FreeResources()
    {
      IntPtr ptr;

      for( int i = 0, len = m_arrFree.Count; i < len; i++ )
      {
        ptr = ( IntPtr )m_arrFree[ i ];
        Marshal.FreeCoTaskMem( ptr );
      }

      m_arrFree.Clear();

      for( int i = 0, len = m_arrGlobalFree.Count; i < len; i++ )
      {
        ptr = ( IntPtr )m_arrGlobalFree[ i ];
        Marshal.FreeHGlobal( ptr );
      }

      m_arrGlobalFree.Clear();

      for( int i = 0; i < m_arrDispose.Count; i++ )
      {
        ( (PropVariant) m_arrDispose[ i ] ).Dispose();
      }

      m_arrDispose.Clear();
    }
    /// <summary>
    /// Frees resuources allocated for property name storage.
    /// </summary>
    private void FreeName()
    {
      int iPropId = ( int )m_prop.propid;

      if( m_prop.ulKind == ( IntPtr )PRSPEC.PRSPEC_LPWSTR && iPropId != 0 )
      {
        IntPtr name = ( IntPtr )iPropId;
        Marshal.FreeHGlobal( name );
        m_prop.propid = IntPtr.Zero;
      }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private object GetValue()
    {
      VarEnum varEnum = Type;
      IntPtr ptrValue = ( IntPtr )( m_propVariant.ToInt64() + FirstIntOffset );

      switch( varEnum )
      {
        case VarEnum.VT_R8:
          long lValue = Marshal.ReadInt64( ptrValue );
          return BitConverter.Int64BitsToDouble( lValue );

        case VarEnum.VT_I4:
        case VarEnum.VT_INT:
          return Marshal.ReadInt32( ptrValue );

        case VarEnum.VT_BOOL:
          return Marshal.ReadInt32( ptrValue ) != 0;

        case VarEnum.VT_LPSTR:
        case VarEnum.VT_LPWSTR:
          string strResult = GetString( FirstInt );

          if( varEnum == VarEnum.VT_LPSTR )
          {
            String = strResult;
          }
          return strResult;
      
        case VarEnum.VT_FILETIME:
          //long lFileTime = Marshal.ReadInt64( ptrValue );
          return DateTime;//DateTime.FromFileTime( lFileTime );

        case VarEnum.VT_BLOB:
        {
          int iLength = FirstInt.ToInt32();

          byte[] arrBuffer = new byte[ iLength ];
          IntPtr ptrBlob = SecondInt;
          Marshal.Copy( ptrBlob, arrBuffer, 0, iLength );
          return arrBuffer;
        }

        case VarEnum.VT_VECTOR | VarEnum.VT_LPWSTR:
        case VarEnum.VT_VECTOR | VarEnum.VT_LPSTR:
        {
          object result = GetStringArray();
          Type = VarEnum.VT_VECTOR | VarEnum.VT_LPWSTR;
          return result;
        }

        case VarEnum.VT_VECTOR | VarEnum.VT_VARIANT:
          return GetObjectArray();

        default: return null;
      }

    }
    #endregion

    #region Class methods
    /// <summary>
    /// Writes variant to the property storage.
    /// </summary>
    /// <param name="storProp">
    /// Property storage that will receive PropVariant value.
    /// </param>
    [ CLSCompliant( false ) ]
    public void Write( IPropertyStorage storProp )
    {
      PID pid = PID.PID_FIRST_USABLE;

      if( m_prop.ulKind == ( IntPtr )PRSPEC.PRSPEC_PROPID )
      {
        pid = ( PID )m_prop.propid;
      }

      storProp.WriteMultiple( 1, ref m_prop, this.IntPtr, pid );
    }
    /// <summary>
    /// Reads information from the storage.
    /// </summary>
    /// <param name="propInfo">Property information.</param>
    /// <param name="storProp">Storage to read from.</param>
    /// <param name="bBuiltIn">Indicates whether property is built-in.</param>
    [ CLSCompliant( false ) ]
    public void Read( tagSTATPROPSTG propInfo, IPropertyStorage storProp, bool bBuiltIn )
    {
      m_prop.propid = ( IntPtr )propInfo.propid;
      m_prop.ulKind = ( IntPtr )PRSPEC.PRSPEC_PROPID;
      storProp.ReadMultiple( 1, ref m_prop, m_propVariant );

      if( !bBuiltIn && ( ( m_prop.propid.ToInt32() & DEF_LINK_BIT ) == 0 ) )
      {
        Name = propInfo.lpwstrName;
      }
    }
    /// <summary>
    /// Reads information from the storage.
    /// </summary>
    /// <param name="storProp">Storage to read from.</param>
    /// <param name="bBuiltIn">Indicates whether property is built-in.</param>
    [ CLSCompliant( false ) ]
    public void Read( IPropertyStorage storProp, bool bBuiltIn )
    {
      storProp.ReadMultiple( 1, ref m_prop, m_propVariant );
    }
    #endregion

    #region IDisposable Members
    /// <summary>
    /// Performs tasks associated with freeing, releasing, 
    /// or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      if( m_propVariant != IntPtr.Zero )
      {
        FreeResources();
        FreeName();

        if( m_bFreeVariant )
        {
          Marshal.FreeHGlobal( m_propVariant );
          m_propVariant = IntPtr.Zero;
        }
      }

      GC.SuppressFinalize( this );
    }
    /// <summary>
    /// Finilizer.
    /// </summary>
    ~PropVariant()
    {
      Dispose();
    }
    #endregion
  }
}
