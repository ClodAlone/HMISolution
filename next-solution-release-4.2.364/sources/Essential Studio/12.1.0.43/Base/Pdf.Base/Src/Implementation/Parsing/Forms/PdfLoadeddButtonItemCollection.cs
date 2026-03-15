#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

/// <summary>
/// The Syncfusion.Pdf.Parsing namespace contains classes, which are used to load or modify an existing PDF document.
/// </summary>
namespace Syncfusion.Pdf.Parsing
{
    /// <summary>
    /// Represents state item collection.
    /// </summary>
    /// <seealso cref="PdfCollection"/> Class   
    public class PdfLoadeddStateItemCollection : PdfCollection
    {
        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.Pdf.Parsing.PdfLoadedStateItem"/> at the specified index.
        /// </summary>  
        /// <value>The index of specified <see cref="Syncfusion.Pdf.Parsing.PdfLoadedStateItem"/> item.</value>
        public PdfLoadedStateItem this[int index]
        {
            get
            {
                if ((index < 0) || (index >= Count))
                    throw new IndexOutOfRangeException("index");

                return (List[index] as PdfLoadedCheckBoxItem);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Index of the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The index of specified item</returns>
        internal int IndexOf(PdfLoadedStateItem item)
        {
            return List.IndexOf(item);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        internal void Add(PdfLoadedStateItem item)
        {
            if (item == null)
                throw new NullReferenceException("item");

            List.Add(item);
        }
        #endregion

    }
}
