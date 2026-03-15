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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

#if WINDOWS_PHONE||WINDOWS_PHONE_7
namespace Syncfusion.WP.Controls.Input
#else
namespace Syncfusion.UI.Xaml.Controls.Input
#endif
{
    /// <summary>
    /// Represents a collection of the DateTime
    /// </summary>
    public class DateTimeCollection : ICollection<DateTime>, INotifyCollectionChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Input.DateTimeCollection"/> class.
        /// </summary>
        public DateTimeCollection()
        {
            selecteddates = new Collection<DateTime>();
        }

        private Collection<DateTime> selecteddates;

        /// <summary>
        /// Returns the index of the item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(DateTime item)
        {
            return selecteddates.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, DateTime item)
        {
            selecteddates.Insert(index, item);
            RaiseCollectionChanged();
        }

        private void RaiseCollectionChanged()
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        /// <summary>
        /// Removes the object at particular index
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            selecteddates.RemoveAt(index);
            RaiseCollectionChanged();
        }

        /// <summary>
        /// Gets or sets the selected dates.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DateTime this[int index]
        {
            get
            {
                return selecteddates[index];
            }
            set
            {
                selecteddates[index] = value;
            }
        }

        /// <summary>
        /// Add an item
        /// </summary>
        /// <param name="item"></param>
        public void Add(DateTime item)
        {
            selecteddates.Add(item);
            RaiseCollectionChanged();
        }

        /// <summary>
        /// Clear the selected dates.
        /// </summary>
        public void Clear()
        {
            selecteddates.Clear();
            //This is not needed for current implementation...
            //RaiseCollectionChanged();
        }

        /// <summary>
        /// Returns true when set
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(DateTime item)
        {
            return selecteddates.Contains(item);
        }

        /// <summary>
        /// Returns true when date is present
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool ContainsDate(DateTime date)
        {
            var query = from DateTime _date in selecteddates
                        where _date.Date == date.Date
                        select date;

            return query.Any();
        }

        /// <summary>
        /// Copies the selected dates to the specified index
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(DateTime[] array, int arrayIndex)
        {
            selecteddates.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the counts of the selected dates
        /// </summary>
        public int Count
        {
            get { return selecteddates.Count; }
        }

        /// <summary>
        /// Returns a value when set
        /// </summary>
        public bool IsReadOnly
        {
            get { return (selecteddates as ICollection<DateTime>).IsReadOnly; }
        }

        /// <summary>
        /// Returns a true when item is removed
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(DateTime item)
        {
            bool value = false;
            List<int> query = (from DateTime _date in selecteddates
                        where _date.Date == item.Date
                               select selecteddates.IndexOf(_date)).ToList<int>();
            if(query.Count>0)
                value = selecteddates.Remove(selecteddates[query[0]]);
            if (value)
            {
                RaiseCollectionChanged();
            }
            return value;

        }

        /// <summary>
        /// Gets the enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<DateTime> GetEnumerator()
        {
            return selecteddates.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (selecteddates as IEnumerable).GetEnumerator();
        }

        /// <summary>
        /// Invoked when the collection is changed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
