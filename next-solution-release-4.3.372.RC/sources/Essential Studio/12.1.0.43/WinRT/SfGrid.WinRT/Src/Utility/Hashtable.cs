#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !WPF
using Syncfusion.UI.Xaml.Grid;
using System.Collections.Generic;

namespace Syncfusion.UI.Xaml.Utility
{
    [ClassReference(IsReviewed = false)]
    public class Hashtable : Dictionary<object, object>
    {
        public new object this[object key]
        {
            get
            {
                object value = null;
                this.TryGetValue(key, out value);
                return value;
            }

            set
            {
                this.Add(key, value);
            }
        }

        public Hashtable Clone()
        {
            var newTable = new Hashtable();
            foreach (var kvp in this)
            {
                newTable.Add(kvp.Key, kvp.Value);
            }

            return newTable;
        }

        public bool Contains(object key)
        {
            return this.ContainsKey(key);
        }
    }
}
#endif