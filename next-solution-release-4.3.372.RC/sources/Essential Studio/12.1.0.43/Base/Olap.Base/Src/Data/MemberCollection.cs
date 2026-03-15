//-------------------------------------------------------------------------------------------------
// <copyright file="MemberCollection.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Syncfusion.Olap.Data
{
    /// <summary>
    /// Collection of Meamber objects
    /// </summary>
    /// <remarks>
    /// Although the MemberCollection externally represents a collection of Member objects, the 
    /// collection is internally loaded and managed in one of two ways, depending on the parent of 
    /// the collection:
    /// <para>
    /// * If the parent object was referenced as a result of a query, as in the case of a Tuple, the collection 
    /// represents the members referenced by that query.
    /// </para>
    /// <para>
    /// * If the parent object was referenced as a result of a request for metadata, the collection represents 
    /// the members referenced by the definition of the parent object.
    /// </para>
    /// </remarks>
    [Serializable]
    public class MemberCollection : Collection<Member>
    {
        #region Internal Variables
        internal object _parent;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberCollection"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public MemberCollection(object parent)
        {
            if (parent is MemberCollection)
            {
                this._parent = ((MemberCollection)_parent)._parent;
            }
            else
            {
                this._parent = parent;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberCollection"/> class.
        /// </summary>
        public MemberCollection()
        {
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds the collection of member to the list
        /// </summary>
        /// <param name="memberCollection">The member collection.</param>
        public void AddRange(MemberCollection memberCollection)
        {
            foreach (Member member in memberCollection)
            {
                this.Add(member);
            }
        }

        /// <summary>
        /// Finds the Member by its name
        /// </summary>
        /// <param name="name">The name\ of the member object</param>
        /// <returns>Member object</returns>
        public Member FindByName(string name)
        {
            foreach (Member member in this)
            {
                if (member.Name.ToUpper() == name.ToUpper() || member.UniqueName.ToUpper() == name.ToUpper())
                {
                    return member;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the Member by its unique name
        /// </summary>
        /// <param name="uniqueName">Name of the unique.</param>
        /// <returns>Member object returned if found else null is returned</returns>
        public Member FindByUniqueName(string uniqueName)
        {
            foreach (Member member in this)
            {
                if (member.UniqueName == uniqueName)
                {
                    return member;
                }
            }

            return null;
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Inserts an element into the <see cref="T:System.Collections.ObjectModel.Collection`1"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.-or-<paramref name="index"/> is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"/>.</exception>
        protected override void InsertItem(int index, Member item)
        {
            this.UpdateMemberParent(item);
            base.InsertItem(index, item);
        }
        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index. The value can be null for reference types.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.-or-<paramref name="index"/> is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"/>.</exception>
        protected override void SetItem(int index, Member item)
        {
            this.UpdateMemberParent(item);
            base.SetItem(index, item);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Updates the member parent.
        /// </summary>
        /// <param name="parent">The parent.</param>
        void UpdateMemberParent(Member parent)
        {
            Member member = parent;
            if (this._parent is Level)
            {
                member.ParentLevel = (Level)_parent;
            }
            else if (this._parent is Member)
            {
                member._ParentMember = (Member)_parent;
            }
        }
        #endregion
    }
}
