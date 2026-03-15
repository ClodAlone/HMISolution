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

#if !SILVERLIGHT
using Syncfusion.Olap.Common;

namespace Syncfusion.Olap.Reports
{
    /// <summary>
    /// Collection of level elements.
    /// </summary>
    [Serializable]
    public class LevelElementCollection : CollectionBase, ICloneable<LevelElementCollection>
#else
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
namespace Syncfusion.OlapSilverlight.Reports
{
    /// <summary>
    /// Collection of level elements
    /// </summary>
    [CollectionDataContract]
    public class LevelElementCollection : Collection<LevelElement>
#endif
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

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the <see cref="LevelElement"/> at the specified index.
        /// </summary>
        /// <value></value>
        public LevelElement this[int index]
        {
            get
            {
                return (LevelElement)base.List[index];
            }
            set
            {
                base.List[index] = value;
            }
        }

        /// <summary>
        /// Adds the specified level element.
        /// </summary>
        /// <param name="levelElement">The level element.</param>
        /// <returns>Index of the levelElement added.</returns>
        public int Add(LevelElement levelElement)
        {
            return base.List.Add(levelElement);
        }

        /// <summary>
        /// Removes the specified level element.
        /// </summary>
        /// <param name="levelElement">The level element.</param>
        public void Remove(LevelElement levelElement)
        {
            base.List.Remove(levelElement);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A copy of <see cref="LevelElementCollection"/>.</returns>
        public LevelElementCollection Clone()
        {
            LevelElementCollection levelElementCollection = new LevelElementCollection();
            foreach (LevelElement levelElement in base.List)
            {
                levelElementCollection.Add(levelElement.Clone());
            }

            return levelElementCollection;
        }
#else
        /// <summary>
        /// Adds the specified level element.
        /// </summary>
        /// <param name="levelElement">The level element.</param>
        public void Add(LevelElement levelElement)
        {
            base.Items.Add(levelElement);
            this.UpdateParent(levelElement);
        }

#endif

        /// <summary>
        /// Gets the <see cref="Syncfusion.Olap.Reports.LevelElement"/> with the specified name.
        /// </summary>
        /// <value></value>
        public LevelElement this[string name]
        {
            get
            {
                return this.FindLevelElementByName(name);
            }
        }

        /// <summary>
        /// Finds the level element by getting its name
        /// </summary>
        /// <param name="levelName">Name of the level.</param>
        /// <returns>A <see cref="LevelElement"/>.</returns>
        public LevelElement FindLevelElementByName(string levelName)
        {
#if !SILVERLIGHT
            foreach (LevelElement levelElement in this.List)
#else
            foreach (LevelElement levelElement in this.Items)
#endif
            {
                if (levelElement.Name == levelName)
                {
                    return levelElement;
                }
            }

            return null;
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
            this.UpdateParent(value);
        }
        
        
        /// <summary>
        /// Performs additional custom processes after setting a value in the <see cref="T:System.Collections.CollectionBase"/> instance.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="oldValue"/> can be found.</param>
        /// <param name="oldValue">The value to replace with <paramref name="newValue"/>.</param>
        /// <param name="newValue">The new value of the element at <paramref name="index"/>.</param>
        protected override void OnSetComplete(int index, object oldValue, object newValue)
        {
            this.UpdateParent(newValue);
        }
#else
        protected override void InsertItem(int index, LevelElement item)
        {
            base.InsertItem(index, item);
            this.UpdateParent(item);
        }

        protected override void SetItem(int index, LevelElement item)
        {
            base.SetItem(index, item);
            this.UpdateParent(item);
        }
#endif
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
