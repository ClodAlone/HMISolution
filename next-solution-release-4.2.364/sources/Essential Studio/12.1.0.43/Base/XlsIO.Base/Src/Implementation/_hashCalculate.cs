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
using System.Collections;
using System.Reflection;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
#endregion

namespace Syncfusion.XlsIO.Implementation
{
  /// <summary>
  /// This is a helper class which creates an object 
  /// hash value using Reflection.
  /// </summary>
  sealed public class HashCalculate
  {
    #region Class members
    /// <summary>
    /// Member used to detect level recursion.
    /// </summary>
    static private int m_level = 0;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// To prevent creation of hash class, make its constructor private.
    /// </summary>
    private HashCalculate()
    {
    }

    #endregion
 
    #region Class Public Methods
    /// <summary>
    /// Method calculates hash for any object. Method serializes object properties
    /// to string using Reflection and returns hash of created string.
    /// </summary>
    /// <param name="value">Object which hash must be calculated.</param>
    /// <param name="skipProps">Properties to skip.</param>
    /// <returns>Calculated hash code.</returns>
    public static int CalculateHash( object value, string[] skipProps )
    {
      m_level = 0;

      //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, "-= Start hash =-" );
      StringBuilder builder = new StringBuilder( 8192 );

      List<string> list = new List<string>( skipProps );
      list.Sort();

      ObjectToString( builder, value, list );

      return builder.ToString().GetHashCode();
    }
/*
    /// <summary>
    /// Method converts object to its string representation.
    /// </summary>
    /// <param name="value">Object which must be converted to string.</param>
    /// <returns>String which represents object.</returns>
    private static string ObjectToString( object value, string[] skipProps )
    {
      m_level = 0;

      StringBuilder builder = new StringBuilder( 8192 );

      List<string> list = new List<string>( skipProps );
      list.Sort();

      ObjectToString( builder, value, list );

      return builder.ToString();
    }
*/
    /// <summary>
    /// Method in recursion builds string representation of the object.
    /// </summary>
    /// <param name="builder">Output string</param>
    /// <param name="value">Object which must be converted to string.</param>
    /// <param name="toSkip">Properties to skip.</param>
    private static void ObjectToString( StringBuilder builder, object value, List<string> toSkip )
    {
      m_level++;

      Type type = value.GetType();

      if( !
#if ( WINRT )
          type.GetTypeInfo().IsPrimitive
#else
          type.IsPrimitive 
#endif
          )
      {
          PropertyInfo[] props=
#if ( WINRT )
           type.GetTypeInfo().DeclaredProperties.ToArray<PropertyInfo>();
#else
            type.GetProperties();
#endif

        if( props.Length > 0 )
        {
          for( int i=0; i<props.Length; i++ )
          {
            if( !props[i].CanRead ) continue;
            if( toSkip.BinarySearch( props[i].Name ) >= 0 ) continue;
            
            //Debug.WriteIf( ApplicationImpl.IsDebugInfoEnabled, new string( '.', m_level ) );
            //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, props[i].Name );

            try
            {
              object obj = props[i].GetValue( value, new object[]{} );

              if( obj != null )
              {
                Type type2 = obj.GetType();

                if( obj is string )
                {
                  builder.Append( obj.ToString() );
                }
                else if( obj is ICollection )
                {
                  IEnumerator enm = ((ICollection)obj).GetEnumerator();

                  enm.Reset();
                  int iCount = 0;
                  while( enm.MoveNext() )
                  {
                    //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, props[i].Name + "[" + iCount + "]" );
                    ObjectToString( builder, enm.Current, toSkip );
                    iCount++;
                  }
                }
                else if( !type2
#if ( WINRT )
                    .GetTypeInfo().IsPrimitive )
#else
                    .IsPrimitive )
#endif
                {
                  ObjectToString( builder, obj, toSkip );
                }
                else 
                {
                  builder.Append( obj.ToString() );
                }
              }
              else
              {
                builder.Append( "null" );
              }
            }
            catch( Exception ex )
            {
              //Debug.WriteLineIf( ApplicationImpl.IsDebugInfoEnabled, ex.Message + Environment.NewLine + ex.StackTrace, "Hidden Exception" );

              // If we cannot get property value, skip it.
              continue;
            }
          }
        }
        else
        {
          builder.Append( value.ToString() );
        }
      }
      else
      {
        builder.Append( value.ToString() );
      }

      m_level--;
    }
    #endregion
  }
}