// <copyright file="CustomLevel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Collections.Generic;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <property name="flag" value="Finished" />
    /// <summary>
    /// This class represents levels for auto-complete, where the
    /// source is Custom.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal class CustomLevel : Object, IAutocompleteLevel
    {
        #region Private member

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains level's items.
        /// </summary>
        private readonly AutocompleteItemCollection m_Items = new AutocompleteItemCollection();

        public List<object> m_NewItems = new List<object>();

        #endregion Private member

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomLevel"/> class.
        /// </summary>
        /// <param name="inputList">The input list.</param>
        internal CustomLevel(IList<object> inputList)
        {
            for (int i = 0, cnt = inputList.Count; i < cnt; ++i)
            {
                m_Items.Add(new CustomItem(inputList[i].ToString()));
                m_NewItems.Add(inputList[i]);
            }
        }

        #endregion Initialization

        #region Event

        /// <summary>
        /// Event that occurs when level's items were loaded. Invokes
        /// when items async loading completes.
        /// </summary>
        /// <property name="flag" value="Finished"/>
        public event EventHandler ItemsLoaded;

        #endregion Event

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the level items were loaded.
        /// </summary>
        /// <value></value>
        /// <property name="flag" value="Finished"/>
        public bool IsItemsLoaded
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets splitter for this level.This property contains level's splitter.
        /// </summary>
        /// <value></value>
        /// <property name="flag" value="Finished"/>
        public string Splitter
        {
            get
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Gets items for this level. This property contains level's items.
        /// </summary>
        /// <value></value>
        /// <property name="flag" value="Finished"/>
        public AutocompleteItemCollection Items
        {
            get
            {
                return m_Items;
            }
        }

        /// <summary>
        /// Gets parent for this level.This property contains item's parent.
        /// </summary>
        /// <value></value>
        /// <property name="flag" value="Finished"/>
        public IAutocompleteLevel Parent
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets item text. This property contains item's text.
        /// </summary>
        /// <value></value>
        /// <property name="flag" value="Finished"/>
        public string Text
        {
            get
            {
                return String.Empty;
            }
        }

        #endregion Properties

        #region Public method

        /// <summary>
        /// This method synchronously loads items.
        /// </summary>
        /// <property name="flag" value="Finished"/>
        public void LoadItems()
        {
        }

        /// <summary>
        /// This method loads async items, runs parallel thread.
        /// </summary>
        /// <property name="flag" value="Finished"/>
        public void LoadItemsAsync()
        {
            if (ItemsLoaded != null)
            { }
        }

        /// <summary>
        /// This method gets full way to this level.
        /// </summary>
        /// <returns>
        /// Full way to this level including all parents.
        /// </returns>
        /// <property name="flag" value="Finished"/>
        public string GetFullPath()
        {
            return String.Empty;
        }

        #endregion Public method
    }
}