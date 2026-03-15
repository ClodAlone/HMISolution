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
    /// Represents a collection of list style
    /// </summary>
    public class ListStyleCollection :
#if !SILVERLIGHT && !WP
     XDLSSerializableCollection
#else
      CollectionImpl
      ,IEnumerable
#endif
    {
        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.ListStyle"/> at the specified index.
        /// </summary>
        /// <value></value>
        public ListStyle this[int index]
        {
            get
            {
                return (ListStyle)InnerList[index];
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ListStyleCollection"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal ListStyleCollection(WordDocument doc)
            : base(doc, null)
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds the list style into collection.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <returns></returns>
        public int Add(ListStyle style)
        {
            if (style == null)
                throw new ArgumentNullException("style");
            //Sets new document relation to the cloned list style.
            style.CloneRelationsTo(Document, null);
            return InnerList.Add(style);

        }
        /// <summary>
        /// Finds list style by name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public ListStyle FindByName(string name)
        {
            ListStyle style = StyleCollection.FindStyleByName(InnerList, name) as ListStyle;
            return style;
        }
        #endregion
#if !SILVERLIGHT && !WP
        #region Implementation / xml
        /// <summary>
        /// Creates the item.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override OwnerHolder CreateItem(IXDLSContentReader reader)
        {
            return new ListStyle(Document);
        }
        /// <summary>
        /// Gets name of xml tag
        /// </summary>
        protected override string GetTagItemName()
        {
            return XDLSConstants.StyleItemTag;
        }
        #endregion
#endif
    }
}