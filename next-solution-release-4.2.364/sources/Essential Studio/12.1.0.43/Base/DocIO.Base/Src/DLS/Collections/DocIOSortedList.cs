#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;

namespace Syncfusion.DocIO.DLS
{
  public class DocIOSortedList<TKey, TValue> :
      TypedSortedListEx<TKey, TValue>
  where TKey : IComparable
  {
    public DocIOSortedList() :
      base()
    {
    }

    public DocIOSortedList( IComparer<TKey> comparer ) :
      base( comparer )
    {
    }

    public DocIOSortedList( int count ) :
      base( count )
    {
    }

    public DocIOSortedList( IDictionary<TKey, TValue> dictionary ) :
      base( dictionary )
    {
    }
  }

  public class SortedDictionary<TKey, TValue> :
    TypedSortedListEx<TKey, TValue>
    where TKey : IComparable
  {
  }
}
