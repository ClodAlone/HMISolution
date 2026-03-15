#region Copyright
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws.
#endregion Copyright

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#if DOCIO
using Syncfusion.CompoundFile.DocIO.Net;
#endif

#if !SILVERLIGHT && !WINRT && !WP
using Syncfusion.Compression.Zip;
#elif !DOCIO && WP
using Syncfusion.XlsIO.Implementation.WP;
using Syncfusion.XlsIO.Interfaces;
#elif !DOCIO && !WINRT
using Syncfusion.XlsIO.Implementation.Silverlight;
using Syncfusion.XlsIO.Interfaces;
#elif !DOCIO
using Syncfusion.XlsIO.Implementation.WINRT;
using Syncfusion.XlsIO.Interfaces;
#endif

#if !DOCIO && WINRT
using Syncfusion.XlsIO;
#endif

#if DOCIO
namespace Syncfusion.CompoundFile.DocIO
#else
namespace Syncfusion.CompoundFile.XlsIO
#endif
{
  ///<exclude/>
  /// <summary>
  /// Contains utility methods for object cloning.
  /// </summary>
  sealed class CloneUtils
  {
    #region Class methods
    /// <summary>
    /// Clones int array.
    /// </summary>
    /// <param name="array">Array to clone</param>
    /// <returns>Returns cloned array.</returns>
    public static int[] CloneIntArray( int[] array )
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
      if( array == null )
        return null;

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
      if( array == null )
        return null;

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
    public static object CloneCloneable( ICloneable toClone )
    {
      if( toClone == null )
        return null;

      return toClone.Clone();
    }
#if ( WINRT )
    /// <summary>
    /// Clones SortedList with objects that implement ICloneable interface.
    /// </summary>
    /// <param name="toClone">SortedList with objects to clone.</param>
    /// <returns>SortedList witn clone of the objects.</returns>
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
#endif
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
    /// Clone Dictionary.
    /// </summary>
    /// <param name="hash">Dictionary to clone</param>
    /// <returns>Returns a copy of the Dictionary.</returns>
    public static Dictionary<TKey, TValue> CloneHash<TKey, TValue>( Dictionary<TKey, TValue>  hash )
    {
      if( hash == null )
        throw new ArgumentNullException( "hash" );

      int iLen = hash.Count;
      Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>( iLen );

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
    /// Creates copy of the stream.
    /// </summary>
    /// <param name="stream">Stream to copy.</param>
    /// <returns>Created stream.</returns>
    public static Stream CloneStream( Stream stream )
    {
#if ( WINRT )
        if (stream == null)
            return null;

        MemoryStream ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms;
#else
        return Syncfusion.Compression.Zip.ZipArchiveItem.CloneStream( stream );
#endif
    }
    #endregion
  }
}
