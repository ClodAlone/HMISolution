#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

#region File using directives
using System;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for TextBoxCollection.
    /// </summary>
    public class TextBoxCollection : CollectionImpl
    {
        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        internal TextBoxCollection(WordDocument doc)
            : base(doc, doc)
        { }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the textbox at the specified index.
        /// </summary>
        /// <value></value>
        public WTextBox this[int index]
        {
            get
            {
                return InnerList[index] as WTextBox;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Removes a textbox at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            WTextBox textbox = InnerList[index] as WTextBox;
            textbox.OwnerParagraph.Items.Remove(textbox);
        }
        /// <summary>
        /// Removes all textboxes from the document. 
        /// </summary>
        public void Clear()
        {
            while (InnerList.Count > 0)
            {
                int lastIndex = InnerList.Count - 1;
                RemoveAt(lastIndex);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Adds the specified textbox.
        /// </summary>
        /// <param name="textbox">The textbox.</param>
        internal void Add(WTextBox textbox)
        {
            InnerList.Add(textbox);
        }
        /// <summary>
        /// Removes the specified textbox.
        /// </summary>
        /// <param name="textbox">The textbox.</param>
        internal void Remove(WTextBox textbox)
        {
            InnerList.Remove(textbox);
        }
        #endregion
    }
}
