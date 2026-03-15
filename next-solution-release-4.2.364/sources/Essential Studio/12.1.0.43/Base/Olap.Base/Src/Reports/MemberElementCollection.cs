//-------------------------------------------------------------------------------------------------
// <copyright file="MemberElementCollection.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;


#if !SILVERLIGHT
using Syncfusion.Olap.Common;

namespace Syncfusion.Olap.Reports
{
    /// <summary>
    /// Collection of member elements
    /// </summary>
    [Serializable]
    public class MemberElementCollection : CollectionBase, ICloneable<MemberElementCollection>
#else

using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace Syncfusion.OlapSilverlight.Reports
{
    /// <summary>
    /// Collection of member elements
    /// </summary>
    [CollectionDataContract]
    public class MemberElementCollection : Collection<MemberElement>
#endif
    {
        #region Internal Variables
        internal object _parentElement;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberElementCollection"/> class.
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        public MemberElementCollection(object parentElement)
        {
            if (parentElement is MemberElementCollection)
            {
                this._parentElement = ((MemberElementCollection)parentElement)._parentElement;
            }
            else
            {
                this._parentElement = parentElement;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberElementCollection"/> class.
        /// </summary>
        public MemberElementCollection()
        {
        }
        #endregion

        #region Public Methods
#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Olap.Reports.MemberElement"/> at the specified index.
        /// </summary>
        /// <value></value>
        public MemberElement this[int index]
        {
            get
            {
                return (MemberElement)base.List[index];
            }
            set
            {
                base.List[index] = value;
            }
        }

        /// <summary>
        /// Removes the specified member element.
        /// </summary>
        /// <param name="memberElement">The member element.</param>
        public void Remove(MemberElement memberElement)
        {
            base.List.Remove(memberElement);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A copy of <see cref="MemberElementCollection"/>.</returns>
        public MemberElementCollection Clone()
        {
            MemberElementCollection memberElementCollection = new MemberElementCollection();
            foreach (MemberElement memberElement in base.List)
            {
                memberElementCollection.Add(memberElement.Clone());
            }

            return memberElementCollection;
        }

        /// <summary>
        /// Adds the specified member element.
        /// </summary>
        /// <param name="memberElement">The member element.</param>
        /// <returns>The index of the memberElement</returns>
        public int Add(MemberElement memberElement)
        {
            if (_parentElement is LevelElement)
            {
                memberElement.ParentLevelElement = _parentElement as LevelElement;
            }
            else if (_parentElement is MemberElement)
            {
                memberElement.ParentMemberElement = _parentElement as MemberElement;
            }

            return base.List.Add(memberElement);
        }
#else
        /// <summary>
        /// Adds the specified member element.
        /// </summary>
        /// <param name="memberElement">The member element.</param>
        /// <returns>the index of the memberElement</returns>
        public void Add(MemberElement memberElement)
        {
            if (_parentElement is LevelElement)
            {
                memberElement.ParentLevelElement = _parentElement as LevelElement;
            }
            else if (_parentElement is MemberElement)
            {
                memberElement.ParentMemberElement = _parentElement as MemberElement;
            }
            base.Items.Add(memberElement);
        }
#endif
        /// <summary>
        /// Gets the <see cref="Syncfusion.Olap.Reports.MemberElement"/> with the specified name.
        /// </summary>
        /// <value><see cref="MemberElement"/></value>
        public MemberElement this[string name]
        {
            get
            {
                return FindMemberElementByUniqueName(name);
            }
        }

        /// <summary>
        /// Finds the name of the member element by unique.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>A <see cref="MemberElement"/>.</returns>
        public MemberElement FindMemberElementByUniqueName(string name)
        {
#if !SILVERLIGHT
            foreach (MemberElement memberElement in this.List)
#else
            foreach (MemberElement memberElement in this.Items)
#endif
            {
                if (memberElement.UniqueName == name)
                {
                    return memberElement;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the name of the member element by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>A <see cref="MemberElement"/>.</returns>
        public MemberElement FindMemberElementByName(string name)
        {
#if !SILVERLIGHT
            foreach (MemberElement memberElement in this.List)
#else
            foreach (MemberElement memberElement in this.Items)
#endif
            {
                if (memberElement.Name == name)
                {
                    return memberElement;
                }
            }

            return null;
        }

        /// <summary>
        /// Updates the member element parent.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public void UpdateMemberElementParent(object parent)
        {
            if (parent is MemberElement)
            {
                MemberElement memberElement = (MemberElement)parent;
                if (_parentElement is MemberElement)
                {
                    if (memberElement.ParentMemberElement == null)
                    {
                        memberElement.ParentMemberElement = (MemberElement)_parentElement;
                    }
                }
                else if (_parentElement is LevelElement)
                {
                    memberElement.ParentLevelElement = (LevelElement)_parentElement;
                }
            }
        }
        #endregion

        #region Protected Methods

#if !SILVERLIGHT
        /// <summary>
        /// Performs additional custom processes after inserting a new element into the <see cref="T:System.Collections.CollectionBase"/> instance.
        /// </summary>
        /// <param name="index">The zero-based index at which to insert <paramref name="value"/>.</param>
        /// <param name="value">The new value of the element at <paramref name="index"/>.</param>
        protected override void OnInsertComplete(int index, object value)
        {
            this.UpdateMemberElementParent(value);
        }

        /// <summary>
        /// Performs additional custom processes after setting a value in the <see cref="T:System.Collections.CollectionBase"/> instance.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="oldValue"/> can be found.</param>
        /// <param name="oldValue">The value to replace with <paramref name="newValue"/>.</param>
        /// <param name="newValue">The new value of the element at <paramref name="index"/>.</param>
        protected override void OnSetComplete(int index, object oldValue, object newValue)
        {
            this.UpdateMemberElementParent(newValue);
        }
#else
        protected override void InsertItem(int index, MemberElement item)
        {
            base.InsertItem(index, item);
            this.UpdateMemberElementParent(item);
        }

        protected override void SetItem(int index, MemberElement item)
        {
            base.SetItem(index, item);
            this.UpdateMemberElementParent(item);
        }
#endif
        #endregion

    }
}
