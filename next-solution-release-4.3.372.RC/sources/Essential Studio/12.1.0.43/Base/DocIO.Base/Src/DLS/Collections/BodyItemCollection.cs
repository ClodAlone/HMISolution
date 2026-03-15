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

using System;
using System.Text;
using Syncfusion.DocIO.DLS.XML;

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a collection for <see cref="Syncfusion.DocIO.DLS.WTextBody"/> child items.
    /// </summary>
    public class BodyItemCollection : EntityCollection
    {
        #region Class constants
        private static readonly System.Type[] DEF_ELEMENT_TYPES = new System.Type[4] { typeof(WTable), typeof(WParagraph), typeof(StructureDocumentTagBlock), typeof(AlternateChunk) };
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.Entity"/> at the specified index.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        new internal TextBodyItem this[int index]
        {
            get
            {
                return (TextBodyItem)base[index];
            }
        }
        protected override Type[] TypesOfElement
        {
            get
            {
                return DEF_ELEMENT_TYPES;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BodyItemCollection"/> class.
        /// </summary>
        /// <param name="body">The body.</param>
        public BodyItemCollection(WTextBody body)
            : base(body.Document, body)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BodyItemCollection"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal BodyItemCollection(WordDocument doc)
            : base(doc, null)
        {
        }
        #endregion
#if !SILVERLIGHT && !WP
        #region Implementation / xml
        /// <summary>
        /// Gets name of xml tag
        /// </summary>
        /// <returns></returns>
        protected override string GetTagItemName()
        {
            return XDLSConstants.ItemTag;
        }
        /// <summary>
        /// Creates the item.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        protected override OwnerHolder CreateItem(IXDLSContentReader reader)
        {
            string type = reader.GetAttributeValue(XDLSConstants.TypeTag);

            switch (type)
            {
                case XDLSConstants.ItemTypeTableValue:
                    return new WTable(Document);
                default:
                    return new WParagraph(Document);
            }
        }
        #endregion
#endif
    }
}
