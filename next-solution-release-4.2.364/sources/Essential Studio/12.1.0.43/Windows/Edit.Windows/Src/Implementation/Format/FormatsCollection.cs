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
using System.Drawing;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Collections.Specialized;

using Syncfusion.Windows.Forms.Edit.Implementation.Config;
using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Edit.Utils;
#endregion

namespace Syncfusion.Windows.Forms.Edit.Implementation.Formatting
{
  /// <summary>
  /// Collection of formats.
  /// </summary>
  public class FormatsCollection
    : EventBaseCollection
  {
    #region Class Properties
    /// <summary>
    /// Gets or sets the element at the specified index.
    /// </summary>
    public ISnippetFormat this[ int index ]
    {
      get
      {
        return ( ISnippetFormat )List[ index ];
      }
      set
      {
        List[ index ] = value;
      }
    }
    #endregion

    #region Class Public Methods
    /// <summary>
    /// Inserts an item to the IList at the specified position.
    /// </summary>
    /// <param name="index">The zero-based index at which value should be inserted.</param>
    /// <param name="value">Format to be inserted.</param>
    public void Insert( int index, ISnippetFormat value )
    {
      List.Insert( index, value );
    }

    /// <summary>
    /// Removes the first occurrence of a specific object from the IList.
    /// </summary>
    /// <param name="value">Format to be removed.</param>
    public void Remove( ISnippetFormat value )
    {
      List.Remove( value );
    }

    /// <summary>
    /// Determines whether the IList contains a specific value.
    /// </summary>
    /// <param name="value">Format to be found.</param>
		/// <returns>True if IList contains a specific value.</returns>
    public bool Contains( ISnippetFormat value )
    {
      return List.Contains( value );
    }

    /// <summary>
    /// Determines the index of a specific item in the IList.
    /// </summary>
    /// <param name="value">Format to be found.</param>
    /// <returns>Index of the format in collection or -1.</returns>
    public int IndexOf( ISnippetFormat value )
    {
      return List.IndexOf( value );
    }

    /// <summary>
    /// Adds an item to the IList.
    /// </summary>
    /// <param name="value">Value to be added.</param>
    /// <returns>Index of the format in collection.</returns>
    public int Add( ISnippetFormat value )
    {
      return List.Add( value );
    }

    /// <summary>
    /// Add collection of formats to the list.
    /// </summary>
    /// <param name="collection">Collection of SnippetFormats to be added.</param>
    public void AddRange( ICollection collection )
    {
      foreach( ISnippetFormat obj in collection )
      {
        Add( obj );
      }
    }
    #endregion
  }
}
