#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Collection event handler delegate.
    /// </summary>
    /// <param name="sender">The sender</param>
    /// <param name="evtArgs">Event Args</param>
    public delegate void CollectionEEventHandler(object sender, CollectionExEventArgs evtArgs);

    /// <summary>
    /// CollectionEx event args.
    /// </summary>
    public class CollectionExEventArgs
        : EventArgs
    {
        #region Class members
        private CollectionExChangeType m_changeType;
        private ICollection m_membersInvolved;
        private object m_memberInvolved;
        private int m_nIndex;
        private bool m_bCancel;
        private object m_collectionOwner;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionExEventArgs"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="changeType">Type of the change.</param>
        /// <param name="element">The element.</param>
        /// <param name="nIndex">Index of the n.</param>
        public CollectionExEventArgs(object owner, CollectionExChangeType changeType, object element, int nIndex)
        {
            m_changeType = changeType;
            m_memberInvolved = element;

            ArrayList arrTemp = new ArrayList(1);
            arrTemp.Add(element);
            m_membersInvolved = arrTemp;
            m_nIndex = nIndex;
            m_collectionOwner = owner;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionExEventArgs"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="changeType">Type of the change.</param>
        /// <param name="elements">The elements.</param>
        /// <param name="nIndex">Index of the n.</param>
        public CollectionExEventArgs(object owner, CollectionExChangeType changeType, ICollection elements, int nIndex)
        {
            m_changeType = changeType;
            m_membersInvolved = elements;
            m_nIndex = nIndex;
            m_collectionOwner = owner;
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets Owner of this collection change.
        /// </summary>
        public object Owner
        {
            get { return m_collectionOwner; }
        }

        /// <summary>
        /// Gets ColectionEx change type.
        /// </summary>
        public CollectionExChangeType ChangeType
        {
            get { return m_changeType; }
        }

        /// <summary>
        /// Gets index of the element involved in collection change.
        /// </summary>
        public int Index
        {
            get { return m_nIndex; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether cancels pending collection changes.
        /// </summary>
        public bool Cancel
        {
            get { return m_bCancel; }
            set { m_bCancel = value; }
        }

        /// <summary>
        /// Gets elements involved in collection change.
        /// </summary>
        public ICollection Elements
        {
            get { return m_membersInvolved; }
        }

        /// <summary>
        /// Gets element involved in collection change.
        /// </summary>
        public object Element
        {
            get { return m_memberInvolved; }
        }
        #endregion
    }
}
