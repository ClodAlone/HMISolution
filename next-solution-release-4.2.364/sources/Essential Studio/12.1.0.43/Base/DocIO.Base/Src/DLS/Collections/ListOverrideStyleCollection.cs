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
using Syncfusion.DocIO.DLS.XML;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for ListOverridesCollection.
    /// </summary>
    internal class ListOverrideStyleCollection : StyleCollection
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ListOverrideStyleCollection"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal ListOverrideStyleCollection(WordDocument doc)
            : base(doc)
        { }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets List level by index.
        /// </summary>
        new public ListOverrideStyle this[int index]
        {
            get
            {
                return (ListOverrideStyle)InnerList[index];
            }
        }
        #endregion

        #region Class methods
        /// <summary>
        /// Adds the List Override Style to collection. 
        /// </summary>
        /// <param name="listOverrideStyle"></param>
        /// <returns></returns>
        internal int Add(ListOverrideStyle listOverrideStyle)
        {
            //Sets new document relation to the cloned list override style.
            listOverrideStyle.CloneRelationsTo(Document, null);
            return InnerList.Add(listOverrideStyle);
        }
        /// <summary>
        /// Finds Style by name 
        /// </summary>
        /// <param name="name"></param>
        new public ListOverrideStyle FindByName(string name)
        {
            return base.FindByName(name) as ListOverrideStyle;
        }
        #endregion
#if !SILVERLIGHT && !WP
        #region XML serialization overrides
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override OwnerHolder CreateItem(IXDLSContentReader reader)
        {
            return new ListOverrideStyle(Document);
        }
        /// <summary>
        /// Gets name of xml tag
        /// </summary>
        protected override string GetTagItemName()
        {
            return XDLSConstants.OverrideListStyleTag;
        }
        #endregion
#endif
    }
}
