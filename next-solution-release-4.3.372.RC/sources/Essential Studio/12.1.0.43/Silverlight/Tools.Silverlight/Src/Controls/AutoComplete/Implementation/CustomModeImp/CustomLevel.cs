#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <property name="flag" value="Finished" />
    /// <summary>
    /// This class represents levels for auto-complete, where the
    /// source is Custom.
    /// </summary>
    internal class CustomLevel : object, IAutocompleteLevel
    {
        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This member contains level's items.
        /// </summary>
        private AutocompleteItemCollection items = new AutocompleteItemCollection();

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Tools.Controls.CustomLevel">CustomLevel</see>
        /// class.
        /// </summary>
        /// <remarks>
        /// Constructor of the CustomLevel Control, which will initialize all events and
        /// properties.
        /// </remarks>
        /// <param name="inputList">Gets the inputlist to add into items.</param>
        internal CustomLevel(List<string> inputList)
        {
            for (int i = 0, cnt = inputList.Count; i < cnt; ++i)
            {
                this.items.Add(new CustomItem(inputList[i]));
            }
        }

        /// <summary>
        /// Gets a value indicating whether to Determines whether items are loaded or not.
        /// </summary>
        /// <remarks>
        /// <b>Xaml</b>
        /// <para></para>
        /// <para>&lt;Syncfusion:AutoComplete Name=&quot;AutoComplete&quot;  /&gt;</para>
        /// <para></para>
        /// <para><b>C#</b></para>
        /// <para></para>
        /// <para>AutoComplete AutoCompleteControl=new AutoComplete();</para>
        /// <para>AutoCompleteControl.=;</para>
        /// </remarks>
        /// <value>
        /// Type : bool
        /// </value>
        public bool IsItemsLoaded
        {
            get
            {
                return true;
            }
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets This property contains level's items. Gets items for this
        /// level.
        /// </summary>
        public AutocompleteItemCollection Items
        {
            get
            {
                return this.items;
            }
        }

        /// <summary>
        /// Gets This property contains item's parent. Gets parent for this
        /// level.
        /// </summary>
        public IAutocompleteLevel Parent
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets This property contains level's splitter. Gets splitter this level.
        /// </summary>
        /// <value>
        /// Type : string
        /// </value>
        public string Splitter
        {
            get
            {
                return String.Empty;
            }
        }

        /// <summary>
        ///  Gets the value of the Text. 
        /// </summary> 
        public string Text
        {
            get
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// This method gets full way to this level.
        /// </summary>
        /// <returns>
        /// Full way to this level including all parents.
        /// </returns>
        public string GetFullPath()
        {
            return String.Empty;
        }

        /// <summary>
        /// This method load the items in a synchronous way
        /// </summary>
        public void LoadItems()
        {
        }

        /// <summary>
        /// This method loads async items, runs parallel thread.
        /// </summary>
        public void LoadItemsAsync()
        {
            if (ItemsLoaded != null)
            { }
        }

        /// <summary>
        /// Event that occurs when level's items were loaded. Invokes
        /// when items async loading completes.
        /// </summary>
        public event EventHandler ItemsLoaded;
    }
}