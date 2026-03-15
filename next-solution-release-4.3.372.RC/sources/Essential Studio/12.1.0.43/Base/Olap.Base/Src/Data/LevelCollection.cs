//-------------------------------------------------------------------------------------------------
// <copyright file="LevelCollection.cs" company="syncfusion">
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
using System.Runtime.Serialization;

#if !SILVERLIGHT

namespace Syncfusion.Olap.Data
{
    /// <summary>
    /// Collection of Level objects
    /// </summary>
    [Serializable]
    public class LevelCollection : CollectionBase, IEnumerable<Level>
    {
#else
using System.Collections.ObjectModel;
namespace Syncfusion.OlapSilverlight.Data
{
    /// <summary>
    /// Collection of Level objects
    /// </summary>
    [CollectionDataContract]
    public class LevelCollection: Collection<Level>
    {
#endif
        #region Private Variables
        private Hierarchy _parentHierarchy;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="LevelCollection"/> class.
        /// </summary>
        public LevelCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LevelCollection"/> class.
        /// </summary>
        /// <param name="parentHierarchy">The parent hierarchy.</param>
        public LevelCollection(Hierarchy parentHierarchy)
        {
            this._parentHierarchy = parentHierarchy;
        }
        #endregion

        #region Public Methods
 
      
#if !SILVERLIGHT

        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Olap.Data.Level"/> at the specified index.
        /// </summary>
        /// <value></value>
        public Level this[int index]
        {
            get { return (Level)List[index]; }
            set { List[index] = value; }
        }

        /// <summary>
        /// Adds the specified level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>returns the index of Level object in the current collection</returns>
        public int Add(Level level)
        {
            return List.Add(level);    
        }

        /// <summary>
        /// Determines whether [contains] [the specified level].
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>
        /// <c>true</c> if [contains] [the specified level]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(Level level)
        {
            return List.Contains(level);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="levelArray">The level array.</param>
        /// <param name="index">The index.</param>
        public void CopyTo(Level[] levelArray, int index)
        {
            List.CopyTo(levelArray, index);
        }

   

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public new IEnumerator<Level> GetEnumerator()
        {
            foreach (Level level in base.List)
            {
                yield return level;
            }
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>returns the index of level object in the collection</returns>
        public int IndexOf(Level level)
        {
            return List.IndexOf(level);
        }

        /// <summary>
        /// Inserts the level on specifed index
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="level">The level.</param>
        public void Insert(int index, Level level)
        {
            List.Insert(index, level);
        }

        /// <summary>
        /// Removes the specified level.
        /// </summary>
        /// <param name="level">The level.</param>
        public void Remove(Level level)
        {
            List.Remove(level);
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
            List.RemoveAt(index);
        }
#endif

        /// <summary>
        /// Finds the Level by it name
        /// </summary>
        /// <param name="name">The name of the level object</param>
        /// <returns>Level object</returns>
        public Level FindByName(string name)
        {
#if !SILVERLIGHT
            foreach (Level level in base.List)
            {
#else
            foreach (Level level in base.Items)
            {
#endif
                if (level.Name == name)
                {
                    return level;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the Level by its unique name
        /// </summary>
        /// <param name="uniqueName">Level unique name.</param>
        /// <returns>Level object</returns>
        public Level FindByUniqueName(string uniqueName)
        {
#if !SILVERLIGHT
            foreach (Level level in base.List)
            {
#else
            foreach (Level level in base.Items)
            {
#endif
                if (level.UniqueName == uniqueName)
                {
                    return level;
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
            this.UpdateParent(value, index);
        }

        /// <summary>
        /// Performs additional custom processes after setting a value in the <see cref="T:System.Collections.CollectionBase"/> instance.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="oldValue"/> can be found.</param>
        /// <param name="oldValue">The value to replace with <paramref name="newValue"/>.</param>
        /// <param name="newValue">The new value of the element at <paramref name="index"/>.</param>
        protected override void OnSetComplete(int index, object oldValue, object newValue)
        {
            this.UpdateParent(newValue, index);
        }
#else
        protected override void InsertItem(int index, Level item)
        {
            base.InsertItem(index, item);
            this.UpdateParent(item, index);
        }

        protected override void SetItem(int index, Level item)
        {
            base.SetItem(index, item);
            this.UpdateParent(item, index);
        }
#endif
        #endregion

        #region Private Methods
        /// <summary>
        /// Updates the parent.
        /// </summary>
        /// <param name="levelObj">The level obj.</param>
        /// <param name="index">The index.</param>
        void UpdateParent(object levelObj, int index)
        {
            if (levelObj is Level)
            {
                Level level = (Level)levelObj;
                level.ParentHierarchy = this._parentHierarchy;
                level.LevelDepth = index;
            }
        }
        #endregion
    }
}
