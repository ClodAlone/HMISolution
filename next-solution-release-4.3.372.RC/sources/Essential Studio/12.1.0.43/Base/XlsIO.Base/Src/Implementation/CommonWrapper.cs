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

using Syncfusion.XlsIO.Interfaces;

namespace Syncfusion.XlsIO.Implementation
{
	/// <summary>
	/// Common wrapper code. Implements BeginUpdate and EndUpdate methods.
	/// </summary>
	public class CommonWrapper
    : IOptimizedUpdate
    , ICloneParent
	{
    #region Class members
    /// <summary>
    /// Number of begin update calls that have no corresponding end update.
    /// </summary>
    private int m_iBeginCount;
    #endregion

    #region Class properties
    /// <summary>
    /// Returns number of begin update calls that have no corresponding end update. Read-only.
    /// </summary>
    protected int BeginCallsCount
    {
      get
      {
        return m_iBeginCount;
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// This method should be called before several updates to the object will take place.
    /// </summary>
    public virtual void BeginUpdate()
    {
      m_iBeginCount++;
    }

    /// <summary>
    /// This method should be called after several updates to the object.
    /// </summary>
    public virtual void EndUpdate()
    {
      if( m_iBeginCount > 0 )
      {
        m_iBeginCount--;
      }
    }
    /// <summary>
    /// Creates a copy of the current object.
    /// </summary>
    /// <param name="parent">Parent object for the new object.</param>
    /// <returns>A copy of the current object.</returns>
    public virtual object Clone( object parent )
    {
      return MemberwiseClone();
    }
    #endregion
  }
}
