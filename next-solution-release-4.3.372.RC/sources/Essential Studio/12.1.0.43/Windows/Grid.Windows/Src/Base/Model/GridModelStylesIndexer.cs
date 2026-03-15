//-------------------------------------------------------------------------------------------------
// <copyright file="GridModelStylesIndexer.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;

using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// This class provides access to a grid column styles collection with
    /// an indexer.
    /// </summary>
    /// <remarks>
    /// You typically access this class from a grid with the <see cref="GridModel.ColStyles"/>
    /// property of a <see cref="GridModel"/>.
    /// </remarks>
    [Editor(typeof(GridColStylesUITypeEditor), typeof(UITypeEditor))]
    public class GridModelColStylesIndexer : GridModelBound, ICollection
    {
        internal GridModelColStylesIndexer(GridModel model)
            : base(model)
        {
        }

        /// <overload>
        /// Gives you access to the column style information of a column.
        /// </overload>
        /// <summary>
        /// Gives you access to the column style information of a column.
        /// </summary>
        /// <remarks>
        /// The indexer provides you with a very simple way to query and change column style contents.
        /// </remarks>
        /// <example>
        /// The following example make some changes to the grid using the indexer:
        /// <code lang="C#">
        ///             model.ColStyles[2].Font.Bold = true;
        ///             model.ColStyles[2].Font.Size = 16;
        ///             model.ColStyles[2].HorizontalAlignment = GridHorizontalAlignment.Center;
        ///             model.ColStyles[2].VerticalAlignment = GridVerticalAlignment.Middle;
        ///             model.ColStyles[2].CellType = "Static";
        ///             model.ColStyles[2].Borders.All = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(100, 238, 122, 3));
        ///             model.ColStyles[2].Interior = new BrushInfo(GradientStyle.PathEllipse, Color.FromArgb(100, 57, 73, 122), Color.FromArgb(237, 240, 247));
        /// </code>
        /// If you query for specific attributes in a cell and these attributes have not been explicitly set,
        /// the <see cref="GridStyleInfo"/> object that is return by the indexer is smart enough to query base styles for
        /// queried information.
        /// <code lang="C#">
        ///             model.ColStyles[1].TextColor = Color.FromArgb(0, 21, 84);
        ///                 Color color = model[1, 1].TextColor;
        ///                 // model[1, 1].TextColor will return Color.FromArgb(0, 21, 84));
        /// </code>
        /// </example>
        public GridStyleInfo this[int colIndex]
        {
            get
            {
                return model[-1, colIndex];
            }

            set
            {
                model[-1, colIndex] = value;
            }
        }

        /// <summary>
        /// Gives you access to the column style information of a column.
        /// </summary>
        /// <genoverload/>
        public GridStyleInfo this[string name]
        {
            get { return this[Model.NameToColIndex(name)]; }
            set { this[Model.NameToColIndex(name)] = value; }
        }

        /*
        /// <summary>
        /// Casts this as <see cref="ICollection"/>.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ICollection Collection
        {
            get
            {
                return this as ICollection;
            }
        }*/

        /// <summary>
        ///   <para>Returns an <see cref="IEnumerator" /> that can iterate through the column styles in the <see cref="GridModelColStylesIndexer" /> instance.</para>
        /// </summary>
        /// <returns>
        ///   <para>An <see cref="IEnumerator" /> for the <see cref="GridModelColStylesIndexer" /> instance.</para>
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            GridStyleInfo[] arr = new GridStyleInfo[this.Count];
            this.CopyTo(arr, 0);
            return arr.GetEnumerator();
        }

        /// <summary>
        ///   <para>Copies the <see cref="GridModelColStylesIndexer" /> elements to a one-dimensional <see cref="System.Array" /> at the specified index.</para>
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="System.Array" /> that is the destination of the <see cref="System.Collections.DictionaryEntry" /> objects copied from the <see cref="GridModelColStylesIndexer" /> instance. The <see cref="System.Array" /> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        public void CopyTo(GridStyleInfo[] array, int index)
        {
            int colCount = this.Count;
            for (int i = index + 1; i <= colCount; ++i)
            {
                array.SetValue(this[i], i - 1);
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((GridStyleInfo[])array, index);
        }

        /// <summary>
        ///   <para>Gets the number of columns in the grid <see cref="GridModel.ColCount"/>.</para>
        /// </summary>
        public int Count
        {
            get
            {
                return this.model.ColCount;
            }
        }

        /// <summary>
        /// Gets a value indicating whether Is Synchronized. Always false.
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a reference to the <see cref="GridModel.Data"/> property of the <see cref="GridModel"/>.
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return this.Model.Data;
            }
        }
    }

    /// <summary>
    /// This class provides access to a grid row styles collection with
    /// an indexer.
    /// </summary>
    /// <remarks>
    /// You typically access this class from a grid with the <see cref="GridModel.ColStyles"/>
    /// property of a <see cref="GridModel"/>
    /// </remarks>
    [Editor(typeof(GridRowsStylesUITypeEditor), typeof(UITypeEditor))]
    public class GridModelRowStylesIndexer : GridModelBound, ICollection
    {
        internal GridModelRowStylesIndexer(GridModel model)
            : base(model)
        {
        }

        /// <overload>
        /// Gives you access to the row style information of a row.
        /// </overload>
        /// <summary>
        /// Gives you access to the row style information of a row.
        /// </summary>
        /// <remarks>
        /// The indexer provides you with a very simple way to query and change row style contents.
        /// </remarks>
        /// <example>
        /// The following example make some changes to the grid using the indexer:
        /// <code lang="C#">
        ///             model.RowStyles[2].Font.Bold = true;
        ///             model.RowStyles[2].Font.Size = 16;
        ///             model.RowStyles[2].HorizontalAlignment = GridHorizontalAlignment.Center;
        ///             model.RowStyles[2].VerticalAlignment = GridVerticalAlignment.Middle;
        ///             model.RowStyles[2].CellType = "Static";
        ///             model.RowStyles[2].Borders.All = new GridBorder(GridBorderStyle.Solid, Color.FromArgb(100, 238, 122, 3));
        ///             model.RowStyles[2].Interior = new BrushInfo(GradientStyle.PathEllipse, Color.FromArgb(100, 57, 73, 122), Color.FromArgb(237, 240, 247));
        /// </code>
        /// If you query for specific attributes in a cell and these attributes have not been explicitly set for the cell,
        /// the <see cref="GridStyleInfo"/> object that is returned by the indexer is smart enough to query base styles for
        /// queried information.
        /// <code lang="C#">
        ///             model.RowStyles[1].TextColor = Color.FromArgb(0, 21, 84);
        ///                 Color color = model[1, 1].TextColor;
        ///                 // model[1, 1].TextColor will return Color.FromArgb(0, 21, 84));
        /// </code>
        /// </example>
        public GridStyleInfo this[int rowIndex]
        {
            get
            {
                return model[rowIndex, -1];
            }

            set
            {
                model[rowIndex, -1] = value;
            }
        }

        /// <summary>
        /// Gives you access to the row style information of a row.
        /// </summary>
        /// <genoverload/>
        public GridStyleInfo this[string name]
        {
            get { return this[Model.NameToRowIndex(name)]; }

            set { this[Model.NameToRowIndex(name)] = value; }
        }

        /*
        /// <summary>
        /// Casts this as <see cref="ICollection"/>.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ICollection Collection
        {
            get
            {
                return this as ICollection;
            }
        }*/

        /// <summary>
        ///   <para>Returns an <see cref="IEnumerator" /> that can iterate through the column styles in the <see cref="GridModelColStylesIndexer" /> instance.</para>
        /// </summary>
        /// <returns>
        ///   <para>An <see cref="IEnumerator" /> for the <see cref="GridModelColStylesIndexer" /> instance.</para>
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            GridStyleInfo[] arr = new GridStyleInfo[this.Count];
            this.CopyTo(arr, 0);
            return arr.GetEnumerator();
        }

        /// <summary>
        ///   <para>Copies the <see cref="GridModelColStylesIndexer" /> elements to a one-dimensional <see cref="System.Array" /> at the specified index.</para>
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="System.Array" /> that is the destination of the <see cref="System.Collections.DictionaryEntry" /> objects copied from the <see cref="GridModelColStylesIndexer" /> instance. The <see cref="System.Array" /> must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        public void CopyTo(GridStyleInfo[] array, int index)
        {
            int rowCount = this.Count;
            for (int i = index + 1; i <= rowCount; ++i)
            {
                array.SetValue(this[i], i - 1);
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((GridStyleInfo[])array, index);
        }
        
        /// <summary>
        ///   <para>Gets the number of rows in the grid <see cref="GridModel.RowCount"/>.</para>
        /// </summary>
        public int Count
        {
            get
            {
                return this.model.RowCount;
            }
        }

        /// <summary>
        /// Gets a value indicating whether Is Synchronized. Always false.
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a reference to the <see cref="GridModel.Data"/> property of the <see cref="GridModel"/>.
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return this.Model.Data;
            }
        }
    }
}
