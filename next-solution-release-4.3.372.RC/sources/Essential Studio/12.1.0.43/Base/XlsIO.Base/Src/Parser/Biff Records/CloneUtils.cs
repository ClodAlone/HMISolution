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

using System;
using System.Collections;

using Syncfusion.XlsIO.Parser.Biff_Records.MsoDrawing;
using Syncfusion.XlsIO.Parser.Biff_Records.Formula;
using Syncfusion.XlsIO.Interfaces;

using System.Collections.Generic;
using System.IO;
#if ( WINRT )
using Syncfusion.XlsIO.Implementation.WINRT;
#endif

#if  (SILVERLIGHT)
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif WP
using Syncfusion.XlsIO.Implementation.WP;
#endif

namespace Syncfusion.XlsIO.Parser.Biff_Records
{
    ///<exclude/>
	/// <summary>
	/// Contains utility methods for object cloning.
	/// </summary>
	public sealed class CloneUtils
	{
    #region Class methods
    /// <summary>
    /// Clones int array.
    /// </summary>
    /// <param name="array">Array to clone</param>
    /// <returns>Returns cloned array.</returns>
    public static int[]    CloneIntArray( int[] array )
    {
      if( array == null )
        return null;

      int iLen = array.Length;
      int[] result = new int[ iLen ];

      for( int i = 0; i < iLen; i++ )
      {
        result[ i ] = array[ i ];
      }

      return result;
    }
    /// <summary>
    /// Clones ushort array.
    /// </summary>
    /// <param name="array">Array to clone.</param>
    /// <returns>Returns cloned array.</returns>
    [ CLSCompliant( false ) ]
    public static ushort[] CloneUshortArray( ushort[] array )
    {
      if( array == null )
        return null;

      int iLen = array.Length;
      ushort[] result = new ushort[ iLen ];

      for( int i = 0; i < iLen; i++ )
      {
        result[ i ] = array[ i ];
      }

      return result;
    }
    /// <summary>
    /// Clones string array.
    /// </summary>
    /// <param name="array">Array to clone.</param>
    /// <returns>Returns cloned array.</returns>
    public static string[] CloneStringArray( string[] array )
    {
      if( array == null ) return null;

      int len = array.Length;
      string[] result = new string[ len ];

      for( int i = 0; i < len; i++ )
      {
        result[ i ] = array[ i ];
      }

      return result;
    }
    /// <summary>
    /// Clones object array.
    /// </summary>
    /// <param name="array">Array to clone.</param>
    /// <returns>Returns cloned array.</returns>
    public static object[] CloneArray( object[] array )
    {
      if( array == null ) return null;

      int len = array.Length;
      object[] result = new object[ len ];

      for( int i = 0; i < len; i++ )
      {
        object item = array[ i ];
        ICloneable toClone = item as ICloneable;

        if( toClone != null )
        {
          item = toClone.Clone();
        }

        result[ i ] = item;
      }

      return result;
    }
    /// <summary>
    /// Clones object that implements ICloneable interface.
    /// </summary>
    /// <param name="toClone">Object to clone.</param>
    /// <returns>A clone of the object.</returns>
    public static List<T> CloneCloneable<T>( List<T> toClone )
    {
      if( toClone == null )
        return null;

      int iCount = toClone.Count;
      List<T> result = new List<T>( iCount );

      for( int i = 0; i < iCount; i++ )
      {
        ICloneable cloneable = toClone[ i ] as ICloneable;
        T toAdd = ( cloneable != null ) ?
          ( T )cloneable.Clone() :
          toClone[ i ];

        result.Add( toAdd );
      }

      return result;
    }
    /// <summary>
    /// Clones object that implements ICloneable interface.
    /// </summary>
    /// <param name="toClone">Object to clone.</param>
    /// <returns>A clone of the object.</returns>
    public static object CloneCloneable( ICloneable toClone )
    {
      if( toClone == null ) return null;

      return toClone.Clone();
    }
    /// <summary>
    /// Clones List with objects that implement ICloneable interface.
    /// </summary>
    /// <param name="toClone">List with objects to clone.</param>
    /// <returns>List with clone of the objects.</returns>
    public static List<BiffRecordRaw> CloneCloneable( List<BiffRecordRaw> toClone )
    {
      if( toClone == null )
        return null;

      int iCount = toClone.Count;
      List<BiffRecordRaw> arrResult = new List<BiffRecordRaw>( iCount );

      for( int i = 0; i < iCount; i++ )
      {
        ICloneable item = ( ICloneable )toClone[ i ];
        arrResult.Add( ( BiffRecordRaw )CloneCloneable( item ) );
      }

      return arrResult;
    }
    /// <summary>
    /// Clones List with objects that implement ICloneable interface.
    /// </summary>
    /// <param name="toClone">List with objects to clone.</param>
    /// <returns>List with clone of the objects.</returns>
    public static List<TextWithFormat> CloneCloneable( List<TextWithFormat> toClone )
    {
      if( toClone == null ) return null;

      int iCount = toClone.Count;
      List<TextWithFormat> arrResult = new List<TextWithFormat>( iCount );

      for( int i = 0; i < iCount; i++ )
      {
        TextWithFormat item = toClone[ i ].TypedClone() ;
        arrResult.Add( item );
      }

      return arrResult;
    }
    /// <summary>
    /// Clones SortedList with objects that implement ICloneable interface.
    /// </summary>
    /// <param name="toClone">SortedList with objects to clone.</param>
    /// <returns>SortedList with clone of the objects.</returns>
    public static SortedList<int, int> CloneSortedList( SortedList<int, int> toClone )
    {
      if( toClone == null ) return null;

      int iCount = toClone.Count;
      SortedList<int, int> arrResult = new SortedList<int, int>( iCount );
      IList< int > lstKeys = toClone.Keys;
      IList< int > lstValues = toClone.Values;

      for( int i = 0; i < iCount; i++ )
      {
        arrResult.Add( lstKeys[ i ], lstValues[ i ] );
      }

      return arrResult;
    }
    /// <summary>
    /// Clone SortedList.
    /// </summary>
    /// <param name="hash">SortedList to clone</param>
    /// <returns>Returns a copy of the SortedList.</returns>
    public static SortedList<TKey, TValue> CloneCloneable<TKey, TValue>( SortedList<TKey, TValue> list )
        where TKey : IComparable
    {
      if( list == null )
        throw new ArgumentNullException( "list" );

      int iLen = list.Count;
      SortedList<TKey, TValue> result = new SortedList<TKey, TValue>( iLen );

      foreach( KeyValuePair<TKey, TValue> entry in list )
      {
        TValue value = entry.Value;
        ICloneable toClone = value as ICloneable;

        if( toClone != null )
          value = ( TValue )toClone.Clone();

        TKey key = entry.Key;
        toClone = key as ICloneable;

        if( toClone != null )
          key = ( TKey )toClone.Clone();

        result.Add( key, value );
      }

      return result;
    }
    /// <summary>
    /// Clones List with objects that implement ICloneable interface.
    /// </summary>
    /// <param name="toClone">List with objects to clone.</param>
    /// <param name="parent">Parent object for the new items.</param>
    /// <returns>List with clone of the objects.</returns>
    public static List<T> CloneCloneable<T>( IList<T> toClone, object parent )
    {
      if( toClone == null )
        return null;

      int iCount = toClone.Count;
      List<T> arrResult = new List<T>( iCount );

      for( int i = 0; i < iCount; i++ )
      {
        ICloneParent item = ( ICloneParent )toClone[ i ];
        arrResult.Add( ( T )CloneCloneable( item, parent ) );
      }

      return arrResult;
    }
    /// <summary>
    /// Clones object that implements ICloneable interface.
    /// </summary>
    /// <param name="toClone">Object to clone.</param>
    /// <param name="parent">Parent object for the new object.</param>
    /// <returns>A clone of the object.</returns>
    public static object CloneCloneable( ICloneParent toClone, object parent )
    {
      if( toClone == null ) return null;

      return toClone.Clone( parent );
    }
    /// <summary>
    /// Clones MsoBase record.
    /// </summary>
    /// <param name="toClone">Object to clone.</param>
    /// <param name="parent">Parent object.</param>
    /// <returns>A clone of the object.</returns>
    [ CLSCompliant( false ) ]
    public static object CloneMsoBase( MsoBase toClone, MsoBase parent )
    {
      if( toClone == null ) return null;

      return toClone.Clone( parent );
    }
    /// <summary>
    /// Clones byte array.
    /// </summary>
    /// <param name="arr">Array to clone.</param>
    /// <returns>Return cloned array.</returns>
    public static byte[] CloneByteArray( byte[] arr )
    {
      if( arr == null )
        return null;

      int iLen = arr.Length;

      byte[] result = new byte[ iLen ];

      for( int i = 0; i < iLen; i++ )
      {
        result[ i ] = arr[ i ];
      }

      return result;
    }
    /// <summary>
    /// Clones formula tokens array.
    /// </summary>
    /// <param name="arrToClone">Array to clone.</param>
    /// <returns>Return cloned array.</returns>
    public static Ptg[] ClonePtgArray( Ptg[] arrToClone )
    {
      if( arrToClone == null ) return null;

      int iLen = arrToClone.Length;
      Ptg[] result = new Ptg[ iLen ];

      for( int i = 0; i < iLen; i++ )
      {
        result[ i ] = ( Ptg )( ( ICloneable )arrToClone[ i ] ).Clone();
      }

      return result;
    }
    /// <summary>
    /// Clones ColumnInfo array.
    /// </summary>
    /// <param name="arrToClone">Array to clone.</param>
    /// <returns>Return cloned array.</returns>
    [ CLSCompliant( false ) ]
    public static ColumnInfoRecord[] CloneArray( ColumnInfoRecord[] arrToClone )
    {
      if( arrToClone == null ) return null;

      int iLen = arrToClone.Length;
      ColumnInfoRecord[] result = new ColumnInfoRecord[ iLen ];

      for( int i = 0; i < iLen; i++ )
      {
        result[ i ] = ( ColumnInfoRecord )CloneCloneable( arrToClone[ i ] );
      }

      return result;
    }
    /// <summary>
    /// Clone Dictionary.
    /// </summary>
    /// <param name="hash">Dictionary to clone</param>
    /// <returns>Returns a copy of the Dictionary.</returns>
    public static Dictionary<TKey, TValue> CloneHash<TKey, TValue>( Dictionary<TKey, TValue> hash )
    {
      if( hash == null )
        throw new ArgumentNullException( "hash" );

      int iLen = hash.Count;
      Dictionary<TKey, TValue> result = new Dictionary<TKey,TValue>( iLen );

      foreach( KeyValuePair<TKey, TValue> entry in hash )
      {
        TValue value = entry.Value;
        ICloneable toClone = value as ICloneable;

        if( toClone != null )
          value = ( TValue )toClone.Clone();

        TKey key = entry.Key;
        toClone = key as ICloneable;

        if( toClone != null )
          key = ( TKey )toClone.Clone();

        result.Add( key, value );
      }

      return result;
    }
    /// <summary>
    /// Clone Dictionary.
    /// </summary>
    /// <param name="hash">Dictionary to clone</param>
    /// <returns>Returns a copy of the Dictionary.</returns>
    public static Dictionary<TextWithFormat, int> CloneHash( Dictionary<TextWithFormat, int> hash )
    {
      if( hash == null )
        throw new ArgumentNullException( "hash" );

      Dictionary<TextWithFormat, int> result = new Dictionary<TextWithFormat, int>();

      foreach( KeyValuePair<TextWithFormat, int> entry in hash )
      {
        TextWithFormat key = entry.Key.TypedClone();
        result.Add( key, entry.Value );
      }

      return result;
    }
    /// <summary>
    /// Clone Dictionary.
    /// </summary>
    /// <param name="hash">Dictionary to clone</param>
    /// <returns>Returns a copy of the Dictionary.</returns>
    public static Dictionary<object, int> CloneHash( Dictionary<object, int> hash )
    {
      if( hash == null )
        throw new ArgumentNullException( "hash" );

      Dictionary<object, int> result = new Dictionary<object, int>();

      foreach( KeyValuePair<object, int> entry in hash )
      {
        object key = entry.Key;//.TypedClone();
        TextWithFormat text = key as TextWithFormat;

        if( text != null )
        {
          key = text.Clone();
        }

        result.Add( key, entry.Value );
      }

      return result;
    }
    /// <summary>
    /// Clone Dictionary.
    /// </summary>
    /// <param name="hash">Dictionary to clone</param>
    /// <returns>Returns a copy of the Dictionary.</returns>
    public static Dictionary<int, int> CloneHash( Dictionary<int, int> hash )
    {
      if( hash == null )
        throw new ArgumentNullException( "hash" );

      Dictionary<int, int> result = new Dictionary<int, int>();

      foreach( KeyValuePair<int, int> entry in hash )
      {
        result.Add( entry.Key, entry.Value );
      }

      return result;
    }
    /// <summary>
    /// Clone Dictionary.
    /// </summary>
    /// <param name="hash">Dictionary to clone.</param>
    /// <param name="parent">Parent object for the new objects.</param>
    /// <returns>Returns a copy of the Dictionary.</returns>
    public static Dictionary<TKey, TValue> CloneHash<TKey, TValue>( Dictionary<TKey, TValue> hash, object parent )
    {
      if( hash == null )
        throw new ArgumentNullException( "hash" );

      int iLen = hash.Count;
      Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>( iLen );

      foreach( KeyValuePair<TKey, TValue> entry in hash )
      {
        TValue value = entry.Value;
        ICloneParent toClone = value as ICloneParent;

        if( toClone != null )
          value = ( TValue )toClone.Clone( parent );

        TKey key = entry.Key;
        toClone = key as ICloneParent;

        if( toClone != null )
          key = ( TKey )toClone.Clone( parent );

        result.Add( key, value );
      }

      return result;
    }
    /// <summary>
    /// Creates copy of the stream.
    /// </summary>
    /// <param name="stream">Stream to copy.</param>
    /// <returns>Created stream.</returns>
    public static Stream CloneStream( Stream stream )
    {
      if( stream == null )
        return null;

      long lStartPos = stream.Position;

      MemoryStream result = new MemoryStream( ( int )stream.Length );
      stream.Position = 0;
      const int BufferSize = 32768;
      byte[] arrBuffer = new byte[ BufferSize ];
      int iReadSize;

      while( ( iReadSize = stream.Read( arrBuffer, 0, BufferSize ) ) != 0 )
      {
        result.Write( arrBuffer, 0, iReadSize );
      }

      stream.Position = lStartPos;
      result.Position = lStartPos;

      return result;
    }
    /// <summary>
    /// Creates a copy of the array of boolean values.
    /// </summary>
    /// <param name="sourceArray">Array to clone.</param>
    /// <returns>Created object.</returns>
    public static bool[] CloneBoolArray( bool[] sourceArray )
    {
      bool[] result = null;

      if( sourceArray != null )
      {
        int iLength = sourceArray.Length;

        result = new bool[ iLength ];

        if( iLength > 0 )
          Buffer.BlockCopy( sourceArray, 0, result, 0, iLength );
      }

      return result;
    }
    #endregion
  }
}
