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
#endregion

namespace Syncfusion.Windows.Forms.Edit.Implementation.IO
{
  /// <summary>
  /// Sturcture, that keeps array of data windows and position in changes list
  /// </summary>
  internal class StreamState
  {
    #region Class constants
    /// <summary>
    /// Default reserve size for ArrayList
    /// </summary>
    private const int DEF_CAPACITY = 16;
    #endregion

    #region Class members
    /// <summary>
    /// Saved position in stream.
    /// </summary>
    private long m_lPosition;
    /// <summary>
    /// Saved clone of the list with DataWindows.
    /// </summary>
    private ArrayList m_arrWindowsList;
    /// <summary>
    /// Saved count of changes.
    /// </summary>
    private int m_iChangesCount;
    #endregion

    #region Class properties
    /// <summary>
    /// Get position
    /// </summary>
    public long Position
    {
      get
      {
        return m_lPosition;
      }
    }

    /// <summary>
    /// Get array of DataWindows
    /// </summary>
    public IList WindowsList
    {
      get
      {
        return m_arrWindowsList;
      }
    }

    /// <summary>
    /// Get quantity of changes in stack
    /// </summary>
    public int ChangesCount
    {
      get
      {
        return m_iChangesCount;
      }
    }
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Hide default contstructor
    /// </summary>
    private StreamState()
    {
    }

    /// <summary>
    /// Main constructor
    /// </summary>
    /// <param name="count">quantity of changes</param>
    /// <param name="position">Current Position</param>
    public StreamState( int count, long position )
      : this( count, position, DEF_CAPACITY )
    {
    }

    /// <summary>
    /// Main construsctor allow to set ChangesCount, Position and
    /// reserve some free space for ArrayList
    /// </summary>
    /// <param name="count">quantity of changes</param>
    /// <param name="position">Current Position</param>
    /// <param name="capacity">How must space to reserve</param>
    public StreamState( int count, long position, int capacity )
    {
      m_iChangesCount = count;
      m_lPosition = position;
      m_arrWindowsList = new ArrayList( capacity );
    }
    #endregion
  }
}