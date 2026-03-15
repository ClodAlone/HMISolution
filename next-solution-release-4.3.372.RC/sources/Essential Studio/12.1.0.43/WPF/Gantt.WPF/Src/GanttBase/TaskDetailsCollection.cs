#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Controls.Gantt
{
    /// <summary>
    /// [Obsolete] Represents the collection of Taks.
    /// </summary>
    public class TaskDetailsCollection : ObservableCollection<TaskDetails>
    {
        #region Overrides

        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert.</param>
        protected override void InsertItem(int index, TaskDetails item)
        {
            if (!Contains(item))
            {
                // VerifyItem(item, null);

                base.InsertItem(index, item);
            }
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index.</param>
        protected override void SetItem(int index, TaskDetails item)
        {
            if (!Contains(item))
            {
                // VerifyItem(item, this[index]);

                base.SetItem(index, item);
            }
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Determines whether this instance can contain the specified apps.
        /// </summary>
        /// <param name="apps">The apps.</param>
        /// <param name="app">The app.</param>
        /// <param name="appOrig">The app orig.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can contain the specified apps; otherwise, <c>false</c>.
        /// </returns>
        internal static bool CanContain(IEnumerable<TaskDetails> apps, TaskDetails app, TaskDetails appOrig)
        {
            var result = CanContainTaskDetails(apps, app, appOrig);
            return result;
        }

        /// <summary>
        /// Determines whether this instance can contain the specified app.
        /// </summary>
        /// <param name="app">The app.</param>
        /// <param name="appOrig">The app orig.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can contain the specified app; otherwise, <c>false</c>.
        /// </returns>
        internal bool CanContain(TaskDetails app, TaskDetails appOrig)
        {
            return CanContain(this, app, appOrig);
        }

        /// <summary>
        /// Determines whether this instance [can contain task details] the specified apps.
        /// </summary>
        /// <param name="apps">The apps.</param>
        /// <param name="app">The app.</param>
        /// <param name="appOrig">The app orig.</param>
        /// <returns>
        /// 	<c>true</c> if this instance [can contain task details] the specified apps; otherwise, <c>false</c>.
        /// </returns>
        private static bool CanContainTaskDetails(IEnumerable<TaskDetails> apps, TaskDetails app, TaskDetails appOrig)
        {
            return !apps.Any(a => a != app && a != appOrig /*&& a.Blocked*/ );
        }

        //private static bool CanContainBlockedTaskDetails(IEnumerable<TaskDetails> apps, TaskDetails app, TaskDetails appOrig)
        //{
        //    return !apps.Any(a => a != app && a != appOrig);
        //}

        //private void VerifyItem(TaskDetails item, TaskDetails appOrig)
        //{
        //    if (!CanContainTaskDetails(this, item, appOrig))
        //    {
        //        throw new InvalidOperationException("Can't insert TaskDetails.");
        //    }
        //}

        #endregion
    }
}
