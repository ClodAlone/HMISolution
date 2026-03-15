#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
#if WINDOWS_PHONE||WINDOWS_PHONE_7
namespace Syncfusion.WP.Controls
#else
#if SILVERLIGHT
namespace Syncfusion.Tools.Controls
#else
#if WPF
namespace Syncfusion.Windows.Controls
#else
using Windows.UI.Xaml;
namespace Syncfusion.UI.Xaml.Controls
#endif
#endif
#endif
{
    /// <summary>
    /// Represents a class to maintain the child collection
    /// </summary>
    public class ChildCollection : IList<object>
    {
        /// <summary>
        /// Initializes an instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.ChildCollection"/> class.
        /// </summary>
        public ChildCollection()
        {
            children = new Collection<object>();
        }

        private Collection<object> children;

        /// <summary>
        /// Gets the index of the item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(object item)
        {
            return children.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item in the particular index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, object item)
        {
            children.Insert(index, item);
        }

        /// <summary>
        /// Removes an item in the particular index
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            children.RemoveAt(index);
        }

        /// <summary>
        /// Gets or sets the children at the index position
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object this[int index]
        {
            get
            {
                return children[index];
            }
            set
            {
                children[index] = value;
            }
        }

        /// <summary>
        /// Add an item to children
        /// </summary>
        /// <param name="item"></param>
        public void Add(object item)
        {
            children.Add(item);
        }

        /// <summary>
        /// Clears the children
        /// </summary>
        public void Clear()
        {
            children.Clear();
        }

        /// <summary>
        /// Checks if the children contains the given item
        /// </summary>
        /// <param name="item"></param>
        /// <returns>
        /// <c>true</c> if it contains; otherwise, <c>false</c>
        /// </returns>
        public bool Contains(object item)
        {
            return children.Contains(item);
        }

        /// <summary>
        /// Copies an item to the children
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(object[] array, int arrayIndex)
        {
            children.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the count of the children
        /// </summary>
        public int Count
        {
            get { return children.Count; }
        }

        /// <summary>
        /// Returns true if set, otherwise false
        /// </summary>
        public bool IsReadOnly
        {
            get { return (children as ICollection<object>).IsReadOnly; }
        }

        /// <summary>
        /// Removes an item from the children
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(object item)
        {
            return children.Remove(item);
        }

        /// <summary>
        /// Gets the children enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<object> GetEnumerator()
        {
            return children.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (children as IEnumerable).GetEnumerator();
        }

    }
}