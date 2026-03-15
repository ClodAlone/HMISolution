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
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a collection of <see cref="Syncfusion.DocIO.DLS.WTextBox"/> objects.
    /// </summary>
    public class WTextBoxCollection
    : EntityCollection,
      IWTextBoxCollection
    {
        #region Class constants
        private static readonly System.Type[] TYPES = new System.Type[1] { typeof(WTextBox) };
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.IWTextBox"/> at the specified index.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        new public IWTextBox this[int index]
        {
            get
            {
                return (IWTextBox)InnerList[index];
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected override System.Type[] TypesOfElement
        {
            get
            {
                return TYPES;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WTextBoxCollection"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        public WTextBoxCollection(IWordDocument doc)
            : base((WordDocument)doc, (WordDocument)doc)
        { }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds a textbox to the collection.
        /// </summary>
        /// <param name="textBox">The text box.</param>
        /// <returns></returns>
        public int Add(IWTextBox textBox)
        {
            return InnerList.Add(textBox);
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
            return Document.CreateParagraphItem(ParagraphItemType.TextBox);
        }
        /// <summary>
        /// Gets name of xml tag
        /// </summary>
        protected override string GetTagItemName()
        {
            return XDLSConstants.TextBoxesTag;
        }
        #endregion
#endif
    }
}
