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

namespace Syncfusion.XlsIO.Implementation.Collections.Grouping
{
	/// <summary>
	/// Summary description for ConditionalFormatsGroup.
	/// </summary>
	public class ConditionalFormatsGroup
    : CommonObject
    , IConditionalFormats
	{
    #region Class members
    /// <summary>
    /// Parent range group.
    /// </summary>
    private RangeGroup m_range;
    #endregion

    #region Class Initialize/Finalize methods
    /// <summary>
    /// Creates new instance of the group.
    /// </summary>
    /// <param name="application">Application object for the new group.</param>
    /// <param name="parent">Parent object for the new group.</param>
    public ConditionalFormatsGroup( IApplication application, object parent )
      : base( application, parent )
    {
    }
    /// <summary>
    /// Searches for all necessary parent objects.
    /// </summary>
    private void FindParents()
    {
      m_range = FindParent( typeof( RangeGroup ) ) as RangeGroup;

      if( m_range == null )
      {
        throw new ArgumentOutOfRangeException( "parent", "Can't find parent range group." );
      }
    }
    #endregion

    #region Class methods
    /// <summary>
    /// Synchronizes all collections.
    /// </summary>
    private void SynchronizeCollection()
    {
      throw new NotImplementedException();
    }
    #endregion

    #region Class properties
    /// <summary>
    /// Returns single entry from the group. Read-only.
    /// </summary>
    public IConditionalFormats this[ int index ]
    {
      get
      {
        return m_range[ index ].ConditionalFormats;
      }
    }
    /// <summary>
    /// Returns number of elements in the group. Read-only.
    /// </summary>
    public int Count
    {
      get
      {
        return m_range.Count;
      }
    }
    #endregion

    #region IConditionalFormats Members

    /// <summary>
    /// Returns number of elements in the collection. Read-only.
    /// </summary>
    int Syncfusion.XlsIO.IConditionalFormats.Count
    {
      get
      {
        return 0;
      }
    }
    /// <summary>
    /// Returns single element from the collection. Read-only.
    /// </summary>
    IConditionalFormat Syncfusion.XlsIO.IConditionalFormats.this[ int index ]
    {
      get
      {
        throw new NotImplementedException();
//        int iCount = Count;
//
//        if( iCount == 0 ) return null;
//
//        if( index < 0 || index > iCount - 1 )
//          throw new ArgumentOutOfRangeException( "index", index, "Value cannot be less than 0 and greater than iCount - 1" );
//
//        return new ConditionalFormatGroup( Application, this, index );
      }
    }

    /// <summary>
    /// Adds new condition to the collection.
    /// </summary>
    /// <returns>Newly added condition.</returns>
    public IConditionalFormat AddCondition()
    {
      this[ 0 ].AddCondition();
      int iIndex = this[ 0 ].Count - 1;
      SynchronizeCollection();
      IConditionalFormats formats = ( IConditionalFormats )this;
      return formats[ iIndex ];
    }
    /// <summary>
    /// Removes the Condtional Format at the specified range
    /// </summary>
    public void Remove()
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Removes the Condtional Format at the Specified Index
    /// </summary>
    public void RemoveAt(int index)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region IEnumerable Members

    /// <summary>
    /// Returns an enumerator that can iterate through a collection.
    /// </summary>
    /// <returns>An IEnumerator that can iterate through the collection.</returns>
    public System.Collections.IEnumerator GetEnumerator()
    {
      // TODO:  Add ConditionalFormatsGroup.GetEnumerator implementation
      return null;
    }

    #endregion

    #region IOptimizedUpdate methods
    /// <summary>
    /// This method should be called before several updates to the object will take place.
    /// </summary>
    public void BeginUpdate()
    {
      throw new NotImplementedException();
    }
    /// <summary>
    /// This method should be called after several updates to the object took place.
    /// </summary>
    public void EndUpdate()
    {
      throw new NotImplementedException();
    }
    #endregion
  }
}
