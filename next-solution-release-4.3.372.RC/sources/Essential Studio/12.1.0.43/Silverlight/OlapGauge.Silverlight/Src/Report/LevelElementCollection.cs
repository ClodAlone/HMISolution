//-------------------------------------------------------------------------------------------------
// <copyright file="LevelElementCollection.cs" company="syncfusion">
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
    /// Collection of level elements
    /// </summary>
    [CollectionDataContract]
    public class LevelElementCollection : Collection<LevelElement>
    {
        #region Private Variables
        private HierarchyElement _parentHierarchy;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="LevelElementCollection"/> class.
        /// </summary>
        /// <param name="parentHierarchyElement">The parent hierarchy element.</param>
        public LevelElementCollection(HierarchyElement parentHierarchyElement)
        {
            this._parentHierarchy = parentHierarchyElement;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LevelElementCollection"/> class.
        /// </summary>
        public LevelElementCollection()
        {
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.OlapSilverlight.Base.Report.LevelElement"/> at the specified index.
        /// </summary>
        /// <value></value>
        public LevelElement this[int index]
        {
            get
            {
                return (LevelElement)base.Items[index];
            }

            set
            {
                base.Items[index] = value;
            }
        }

        public LevelElement this[string name]
        {
            get
            {
                return this.FindLevelElementByName(name);
            }
        }

        /// <summary>
        /// Adds the specified level element.
        /// </summary>
        /// <param name="levelElement">The level element.</param>
        /// <returns>index of the levelElement added</returns>
        public void Add(LevelElement levelElement)
        {
            base.Items.Add(levelElement);
        }

        public LevelElement FindLevelElementByName(string levelName)
        {
            foreach (LevelElement levelElement in this.Items)
            {
                if (levelElement.Name == levelElement.Name)
                {
                    return levelElement;
                }
            }

            return null;
        }

        /// <summary>
        /// Removes the specified level element.
        /// </summary>
        /// <param name="levelElement">The level element.</param>
        public void Remove(LevelElement levelElement)
        {
            base.Items.Remove(levelElement);
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
        //    this.UpdateParent(value);
        //}

        /// <summary>
        /// Performs additional custom processes after setting a value in the <see cref="T:System.Collections.CollectionBase"/> instance.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="oldValue"/> can be found.</param>
        /// <param name="oldValue">The value to replace with <paramref name="newValue"/>.</param>
        /// <param name="newValue">The new value of the element at <paramref name="index"/>.</param>
        //protected override void OnSetComplete(int index, object oldValue, object newValue)
        //{
        //    this.UpdateParent(newValue);
        //}
        #endregion

        #region Private Methods
        /// <summary>
        /// Updates the parent.
        /// </summary>
        /// <param name="levelObj">The level obj.</param>
        void UpdateParent(object levelObj)
        {
            if (levelObj is LevelElement)
            {
                LevelElement levelElement = (LevelElement)levelObj;
                levelElement.ParentHierarchy = _parentHierarchy;
            }
        }
        #endregion
    }
}
