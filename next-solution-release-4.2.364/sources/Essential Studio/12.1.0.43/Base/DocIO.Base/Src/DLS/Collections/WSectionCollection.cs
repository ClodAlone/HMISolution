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

using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a collection of <see cref="Syncfusion.DocIO.DLS.WSection"/>. 
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class WSectionCollection
      : EntityCollection,
        IWSectionCollection
    {
        #region Class constants
        private static readonly System.Type[] DEF_ELEMENT_TYPES = new System.Type[1] { typeof(WSection) };
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.WSection"/> at the specified index.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        new public WSection this[int index]
        {
            get
            {
                return InnerList[index] as WSection;
            }
        }
        protected override System.Type[] TypesOfElement
        {
            get
            {
                return DEF_ELEMENT_TYPES;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WSectionCollection"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        public WSectionCollection(WordDocument doc)
            : base(doc, doc)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="WSectionCollection"/> class.
        /// </summary>
        internal WSectionCollection()
            : base(null, null)
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds a section to end of document.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <returns></returns>
        public int Add(IWSection section)
        {
            return base.Add(section);
        }
        /// <summary>
        /// Returns the zero-based index of the specified section.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <returns></returns>
        public int IndexOf(IWSection section)
        {
            return InnerList.IndexOf(section);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <returns></returns>
        internal string GetText()
        {
            string text = string.Empty;
            for (int i = 0; i < Count; i++)
            {
                text += this[i].GetText();
                if (!text.EndsWith(ControlChar.ParagraphBreak))
                    text += ControlChar.ParagraphBreak;
            }
            return text;
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
            return new WSection(Document);
        }
        /// <summary>
        /// Gets name of xml tag
        /// </summary>
        protected override string GetTagItemName()
        {
            return XDLSConstants.SectionItemTag;
        }
        #endregion
#endif
    }
}
