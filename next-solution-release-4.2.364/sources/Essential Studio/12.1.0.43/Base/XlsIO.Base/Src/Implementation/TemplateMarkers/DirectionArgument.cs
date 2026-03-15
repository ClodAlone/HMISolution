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
using System.Text.RegularExpressions;
using System.Collections.Generic;

#if ( WINRT )
using Syncfusion.XlsIO.Implementation.WINRT;
#endif
#if  (SILVERLIGHT)
using Syncfusion.XlsIO.Implementation.Silverlight;
#elif (WP)
using Syncfusion.XlsIO.Implementation.WP;
#endif


namespace Syncfusion.XlsIO.Implementation.TemplateMarkers
{
  /// <summary>
  /// Class used for Direction argument.
  /// </summary>
  [ TemplateMarker ]
  public class DirectionArgument : MarkerArgument
  {
    #region Class constants
    /// <summary>
    /// Sorted list with Direction enumeration values,
    /// key - string representation of the current direction.
    /// </summary>
    private static readonly SortedList<string, MarkerDirection> s_lstStringToDirection;
    #endregion

    #region Class members
    /// <summary>
    /// Marker direction.
    /// </summary>
    private MarkerDirection m_direction;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Initializes static members of the DirectionArgument class.
    /// </summary>
    static DirectionArgument()
    {
      //MarkerDirection[] arrDirections = ( MarkerDirection[] )Enum.GetValues( typeof( MarkerDirection ) );
      //int iCount = arrDirections.Length;
      s_lstStringToDirection = new SortedList<string, MarkerDirection>( 2 );
      s_lstStringToDirection.Add( "vertical", MarkerDirection.Vertical );
      s_lstStringToDirection.Add( "horizontal", MarkerDirection.Horizontal );


      //for( int i = 0; i < iCount; i++ )
      //{
      //  MarkerDirection curDirection = arrDirections[ i ];
      //  s_lstStringToDirection.Add( curDirection.ToString().ToLower(), curDirection );
      //}
  }
    /// <summary>
    /// Initializes a new instance of the DirectionArgument class.
    /// </summary>
    public DirectionArgument()
    {
    }
    #endregion

    #region Class overrides
    /// <summary>
    /// Tries to parse argument string.
    /// </summary>
    /// <param name="strArgument">String to parse.</param>
    /// <returns>Marker direction.</returns>
    public override MarkerArgument TryParse( string strArgument )
    {
      if( strArgument == null || strArgument.Length == 0 )
        return null;

      DirectionArgument result = null;
      MarkerDirection direction;

      if( s_lstStringToDirection.TryGetValue( strArgument.ToLower(), out direction ) )
      {
        result = ( DirectionArgument )Clone();
        result.m_direction = direction;
      }

      return result;
    }

    /// <summary>
    /// Prepares options if necessary.
    /// </summary>
    /// <param name="options">Options to prepare.</param>
    public override void PrepareOptions( MarkerOptionsImpl options )
    {
      if( options == null )
        throw new ArgumentNullException( "options" );

      options.Direction = m_direction;
    }

    #endregion

    #region Class properties
    /// <summary>
    /// Indicates whether argument is used just to prepare some options before
    /// any values are proceed. Read-only.
    /// </summary>
    public override bool IsPreparing
    {
      get
      {
        return true;
      }
    }

    #endregion

    #region Method
    /// <summary>
    /// Tries and the get direction.
    /// </summary>
    /// <param name="argument">The marker arguments.</param>
    /// <returns>Direction in string</returns>
    internal string TryGetDirection(string argument)
    {
        IList<string> args = s_lstStringToDirection.Keys;
        string tmpArgument = argument;
        foreach (string arg in args)
        {
            if (argument.Contains(arg))
            {
                return arg;
            }
        }
        return null;
    }
    #endregion
  }
}
