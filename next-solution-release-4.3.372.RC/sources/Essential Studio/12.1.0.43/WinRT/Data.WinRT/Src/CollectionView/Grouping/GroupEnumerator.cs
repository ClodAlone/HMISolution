#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections.ObjectModel;
    using System.Collections;

    /// <summary>
    /// Enumerates any <see cref="Group"/> class and lists out all the elements in a one-dimensional array.
    /// </summary>
    public class GroupEnumerator : IEnumerator<NodeEntry>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupEnumerator"/> class.
        /// </summary>
        /// <param name="group">The group.</param>
        public GroupEnumerator(Group group)
        {
            this.Group = group;
            this.Helper = new TraversalHelper(true);
            if (group.GetGroupsCount() == 0)
            {
                this.next = null;
            }
            else
            {
                var firstGroup = group.Groups[0];
                int count = firstGroup.IsBottomLevel ? firstGroup.GetRecordCount() : firstGroup.GetGroupsCount();
                if (count == 0)
                {
                    this.next = this.Helper.GetNext(firstGroup);
                }
                else
                {
                    this.next = firstGroup;
                }
            }
        }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>The group.</value>
        public Group Group
        {
            get;
            private set;
        }

        private TraversalHelper Helper;

        private NodeEntry current;
        private NodeEntry next;

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The element in the collection at the current position of the enumerator.
        /// </returns>
        public NodeEntry Current
        {
            get
            {
                return ((IEnumerator)this).Current as NodeEntry;
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.current = null;
            this.Group = null;
        }

        #endregion

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The element in the collection at the current position of the enumerator.
        /// </returns>
        object IEnumerator.Current
        {
            get
            {
                return this.current;
            }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">
        /// The collection was modified after the enumerator was created.
        /// </exception>
        public bool MoveNext()
        {
            if (this.next == null)
            {
                return false;
            }
            this.current = this.next;
            this.next = Helper.GetNext(this.next);
            return true;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">
        /// The collection was modified after the enumerator was created.
        /// </exception>
        public void Reset()
        {
            this.next = this.Group.Groups[0];
            this.current = null;
        }
    }
}
