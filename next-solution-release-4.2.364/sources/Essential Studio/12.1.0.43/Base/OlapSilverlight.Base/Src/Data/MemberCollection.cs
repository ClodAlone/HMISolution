#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Data
{
    [KnownType(typeof(Member))]
    [CollectionDataContract]
    public class MemberCollection:Collection<Member>
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

        #region  Public Properties
        [DataMember]
        public Level ParentLevel
        {
            get
            {
                return this._parent as Level;
            }
            set
            {
                this._parent = value;
            }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Olap.Data.Member"/> at the specified index.
        /// </summary>
        /// <value></value>
        
        public Member this[int index]
        {
            get { return base.Items[index] as Member; }
            set { base.Items[index] = value; }
        }

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
        /// Determines whether [contains] [the specified member].
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>
        /// <c>true</c> if [contains] [the specified member]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Member member)
        {
            return this.Contains(member);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="memberArray">The member array.</param>
        /// <param name="index">The index.</param>
        public void CopyTo(Member[] memberArray, int index)
        {
            this.CopyTo(memberArray, index);
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
                if (member.Name == name)
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

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>index of the Member object in the current collection</returns>
        public int IndexOf(Member member)
        {
            return this.IndexOf(member);
        }

        /// <summary>
        /// Inserts the member to the specified index
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="member">The member.</param>
        public void Insert(int index, Member member)
        {
            this.Insert(index, member);
        }

        /// <summary>
        /// Removes the specified member.
        /// </summary>
        /// <param name="member">The member.</param>
        public void Remove(Member member)
        {
            this.Remove(member);
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="T:System.Collections.CollectionBase"/> instance. This method is not overridable.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than zero.
        /// -or-
        /// <paramref name="index"/> is equal to or greater than <see cref="P:System.Collections.CollectionBase.Count"/>.
        /// </exception>
        public new void RemoveAt(int index)
        {
            this.RemoveAt(index);
        }
        #endregion
   }    
}
