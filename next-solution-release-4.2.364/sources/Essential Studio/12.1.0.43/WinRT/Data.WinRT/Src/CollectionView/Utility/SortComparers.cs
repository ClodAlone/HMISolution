#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
#if !WinRT
using System.ComponentModel;
#endif
namespace Syncfusion.Data
{
    public class SortComparers : ObservableCollection<SortComparer>
    {
        public SortComparers()
        {

        }

        public IComparer<object> this[String propertyName]
        {
            get
            {
                var comparer = this.FirstOrDefault(comp => comp.PropertyName == propertyName);
                return comparer != null ? comparer.Comparer : null;
            }
        }
    }

    public class SortComparer
    {
        public string PropertyName { get; set; }
        public IComparer<object> Comparer { get; set; }

        public SortComparer()
        {

        }
    }

    public interface ISortDirection
    {
        ListSortDirection SortDirection
        {
            get;
            set;
        }
    }
}
