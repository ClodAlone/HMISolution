#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !WITHOUTOWNTYPES
using System;
using Syncfusion.XlsIO.Implementation;
using System.Collections;
using System.Collections.Generic;

namespace Syncfusion.XlsIO.Implementation.Silverlight
{
  public class SortedList<TKey, TValue> :
      TypedSortedListEx<TKey, TValue>
  where TKey : IComparable
  {
    public SortedList() :
      base()
    {
    }

    public SortedList( IComparer<TKey> comparer ) :
      base( comparer )
    {
    }

    public SortedList( int count ) :
      base( count )
    {
    }

    public SortedList( IDictionary<TKey, TValue> dictionary ) :
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
#endif