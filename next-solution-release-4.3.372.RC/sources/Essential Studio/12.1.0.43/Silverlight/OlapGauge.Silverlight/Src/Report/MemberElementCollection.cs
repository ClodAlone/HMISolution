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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Base.Report
{
    /// <summary>
    /// Collection of member elements
    /// </summary>
    [CollectionDataContract]
    public class MemberElementCollection : Collection<MemberElement>
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
        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.OlapSilverlight.Base.Report.MemberElement"/> at the specified index.
        /// </summary>
        /// <value></value>
        public MemberElement this[int index]
        {
            get
            {
                return (MemberElement)base.Items[index];
            }

            set
            {
                base.Items[index] = value;
            }
        }

        public MemberElement this[string name]
        {
            get
            {
                return FindMemberElementByName(name);
            }
        }

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

        public MemberElement FindMemberElementByName(string name)
        {
            foreach (MemberElement memberElement in this.Items)
            {
                if (memberElement.Name == name)
                {
                    return memberElement;
                }
            }

            return null;
        }

        /// <summary>
        /// Removes the specified member element.
        /// </summary>
        /// <param name="memberElement">The member element.</param>
        public void Remove(MemberElement memberElement)
        {
            base.Items.Remove(memberElement);
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
                    memberElement.ParentMemberElement = (MemberElement)_parentElement;
                }
                else if (_parentElement is LevelElement)
                {
                    memberElement.ParentLevelElement = (LevelElement)_parentElement;
                }
            }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Performs additional custom processes after inserting a new element into the <see cref="T:System.Collections.CollectionBase"/> instance.
        /// </summary>
        /// <param name="index">The zero-based index at which to insert <paramref name="value"/>.</param>
        /// <param name="value">The new value of the element at <paramref name="index"/>.</param>
        //protected override void OnInsertComplete(int index, object value)
        //{
        //    this.UpdateMemberElementParent(value);
        //}

        /// <summary>
        /// Performs additional custom processes after setting a value in the <see cref="T:System.Collections.CollectionBase"/> instance.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="oldValue"/> can be found.</param>
        /// <param name="oldValue">The value to replace with <paramref name="newValue"/>.</param>
        /// <param name="newValue">The new value of the element at <paramref name="index"/>.</param>
        //protected override void OnSetComplete(int index, object oldValue, object newValue)
        //{
        //    this.UpdateMemberElementParent(newValue);
        //}
        #endregion
    }
}
