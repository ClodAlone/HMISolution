#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#region File using derectives

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Syncfusion.Documentation;

#endregion

namespace Syncfusion.Windows.Forms.Chart
{
    #region ChartTitlesListEditor
    /// <internalonly/>
    [DocumentationExclude()]
    class ChartTitlesListEditor : CollectionEditor
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartTitlesListEditor"/> class.
        /// </summary>
        public ChartTitlesListEditor()
            : base(typeof(ChartTitlesList))
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Indicates whether original members of the collection can be removed.
        /// </summary>
        /// <param name="value">The value to remove.</param>
        /// <returns>
        /// true if it is permissible to remove this value from the collection; otherwise, false. The default implementation always returns true.
        /// </returns>
        protected override bool CanRemoveInstance(object value)
        {
            return (value as ChartTitle).Name != ChartControl.c_defaultTitleName;
        }
        #endregion
    }
    #endregion

    /// <summary>
    /// Collection of <see cref="ChartTitle"/> instances.
    /// </summary>
    [Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
    public class ChartTitlesList : ChartBaseList
    {
        #region Properties
        /// <summary>
        /// Gets or sets the <see cref="ChartTitle"/> at the specified index.
        /// </summary>
        public ChartTitle this[int index]
        {
            get
            {
                return List[index] as ChartTitle;
            }

            set
            {
                List[index] = value;
            }
        }

        /// <summary>
        /// Gets <see cref="ChartTitle"/> by the name.
        /// </summary>
        /// <param name="name">Name of <see cref="ChartTitle"/>.</param>
        public ChartTitle this[string name]
        {
            get
            {
                ChartTitle result = null;

                foreach (ChartTitle title in this)
                {
                    if (title.Name == name)
                    {
                        result = title;
                    }
                }

                return result;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartTitlesList"/> class.
        /// </summary>
        public ChartTitlesList()
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds the specified title to collection.
        /// </summary>
        /// <param name="value">The title to add.</param>
        /// <returns>Index of the added title.</returns>
        public int Add(ChartTitle value)
        {
            return List.Add(value);
        }

        /// <summary>
        /// Determines whether collection contains the specified title.
        /// </summary>
        /// <param name="value">The title to check.</param>
        /// <returns>
        ///    <c>true</c> if collection contains the specified title; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(ChartTitle value)
        {
            return List.Contains(value);
        }

        /// <summary>
        /// Removes the specified title from collection.
        /// </summary>
        /// <param name="value">The title to remove.</param>
        public void Remove(ChartTitle value)
        {
            List.Remove(value);
        }

        /// <summary>
        /// Gets index of specified title.
        /// </summary>
        /// <param name="value">The title to get index.</param>
        /// <returns>Index of the <see cref="ChartTitle"/>.</returns>
        public int IndexOf(ChartTitle value)
        {
            return List.IndexOf(value);
        }

        /// <summary>
        /// Inserts <see cref="ChartTitle"/> at specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The <see cref="ChartTitle"/> to insert.</param>
        public void Insert(int index, ChartTitle value)
        {
            List.Insert(index, value);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Validates the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns>Returns true if the list contains this element otherwise false.</returns>
        protected override bool Validate(object obj)
        {
            return (obj != null) && (!this.List.Contains(obj));
        }
        #endregion
    }
}
