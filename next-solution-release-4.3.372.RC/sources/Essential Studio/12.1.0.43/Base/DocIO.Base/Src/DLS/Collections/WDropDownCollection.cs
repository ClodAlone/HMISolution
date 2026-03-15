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

#region file using directives
using System;
using Syncfusion.DocIO.DLS;
using System.Collections;
using Syncfusion.DocIO.DLS.XML;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represent a collection of <see cref="Syncfusion.DocIO.DLS.WDropDownItem"/> objects.
    /// </summary>
    public class WDropDownCollection :
#if !SILVERLIGHT && !WP
     XDLSSerializableCollection
#else
      CollectionImpl
      , IEnumerable
#endif
    {
        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.WDropDownItem"/> at the specified index.
        /// </summary>
        /// <value></value>
        public WDropDownItem this[int index]
        {
            get
            {
                return (WDropDownItem)InnerList[index];
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WDropDownCollection"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        public WDropDownCollection(WordDocument doc)
            : base(doc, null)
        { }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public WDropDownItem Add(string text)
        {
            if (InnerList.Count > 24)
            {
                throw new ArgumentOutOfRangeException("InnerList","You can have no more than 25 items in your drop-down list box");
            }
            WDropDownItem item = new WDropDownItem(Document);
            item.Text = text;
            InnerList.Add(item);
            return item;
        }
        /// <summary>
        /// Removes DropDownItems by index.
        /// </summary>
        /// <param name="index">The index.</param>
        public void Remove(int index)
        {
            if (index >= InnerList.Count)
            {
                throw new ArgumentException("DropDownItem with such index doesn't exist.");
            }
            WDropDownItem dropDownItem = (WDropDownItem)InnerList[index];
            InnerList.Remove(dropDownItem);
        }
        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            InnerList.Clear();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <returns></returns>
        internal int Add(WDropDownItem item)
        {
            return InnerList.Add(item);
        }
        /// <summary>
        /// Clones items to.
        /// </summary>
        /// <param name="destColl">The destination collection.</param>
        internal void CloneTo(WDropDownCollection destColl)
        {
            for (int i = 0, cnt = Count; i < cnt; i++)
            {
                destColl.Add(this[i].Clone());
            }
        }
#if !SILVERLIGHT && !WP
        /// <summary>
        /// Creates the item.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        protected override OwnerHolder CreateItem(IXDLSContentReader reader)
        {
            return new WDropDownItem(Document);
        }
        /// <summary>
        /// Gets name of xml tag
        /// </summary>
        /// <returns></returns>
        protected override string GetTagItemName()
        {
            return XDLSConstants.FormFieldDropDownItemsTag;
        }
#endif
        #endregion
    }
}
