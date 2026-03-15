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
using System.Collections;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents a collection of 
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class StyleCollection :
#if !SILVERLIGHT && !WP
      XDLSSerializableCollection
#else
      CollectionImpl
      ,IEnumerable
#endif
      ,IStyleCollection
    {
        #region Fields
        /// <summary>
        /// Returns true, if the fixed index 13 in stylesheet has style. (other than empty style)
        /// </summary>
        /// <remarks>Reserved styles are applicable only for *.doc format</remarks>
        internal bool m_reserved13HasStyle = false;
        /// <summary>
        /// Returns true, if the fixed index 14 in stylesheet has style. (other than empty style)
        /// </summary>
        /// <remarks>Reserved styles are applicable only for *.doc format</remarks>
        internal bool m_reserved14HasStyle = false;
        /// <summary>
        /// Represents the style name of the style present at the fixed index 13 in the stylesheet
        /// </summary>
        /// <remarks>Reserved styles are applicable only for *.doc format</remarks>
        internal string m_FixedIndex13StyleName = string.Empty;
        /// <summary>
        /// Represents the style name of the style present at the fixed index 14 in the stylesheet
        /// </summary>
        /// <remarks>Reserved styles are applicable only for *.doc format</remarks>
        internal string m_FixedIndex14StyleName = string.Empty;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.DocIO.DLS.IStyle"/> at the specified index.
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        public IStyle this[int index]
        {
            get
            {
                return (IStyle)InnerList[index];
            }
        }
        /// <summary>
        /// Returns true, if the fixed index 13 in stylesheet has style. (other than empty style)
        /// </summary>
        /// <remarks>Reserved styles are applicable only for *.doc format</remarks>
        public bool FixedIndex13HasStyle
        {
            get
            {
                return m_reserved13HasStyle;
            }

            set
            {
                m_reserved13HasStyle = value;
            }
        }
        /// <summary>
        /// Returns true, if the fixed index 14 in stylesheet has style. (other than empty style)
        /// </summary>
        /// <remarks>Reserved styles are applicable only for *.doc format</remarks>
        public bool FixedIndex14HasStyle
        {
            get
            {
                return m_reserved14HasStyle;
            }

            set
            {
                m_reserved14HasStyle = value;
            }
        }
        /// <summary>
        /// Represents the style name of the style present at the fixed index 13 in the stylesheet
        /// </summary>
        /// <remarks>Reserved styles are applicable only for *.doc format</remarks>
        public string FixedIndex13StyleName
        {
            get
            {
                return m_FixedIndex13StyleName;
            }
            set
            {
                m_FixedIndex13StyleName = value;
            }
        }
        /// <summary>
        /// Represents the style name of the style present at the fixed index 14 in the stylesheet
        /// </summary>
        /// <remarks>Reserved styles are applicable only for *.doc format</remarks>
        public string FixedIndex14StyleName
        {
            get
            {
                return m_FixedIndex14StyleName;
            }
            set
            {
                m_FixedIndex14StyleName = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="StyleCollection"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        internal StyleCollection(WordDocument doc)
            : base(doc, doc)
        {
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Adds Style to collection 
        /// </summary>
        /// <param name="style">The style.</param>
        /// <returns></returns>
        public int Add(IStyle style)
        {
            if (style == null)
                throw new ArgumentNullException("style");

            XDLSSerializableBase serBase = (XDLSSerializableBase)style;
            serBase.CloneRelationsTo(Document, null);
            serBase.SetOwner(Document);
            if ((style as Style).BaseStyle != null)
            {
                Style baseStyle = (style as Style).BaseStyle as Style;
                Style foundStyle = FindByName(baseStyle.Name, baseStyle.StyleType) as Style;
                if (foundStyle == null)
                    Add(baseStyle.Clone());
                if (style is WParagraphStyle)
                    (style as WParagraphStyle).ApplyBaseStyle(baseStyle.Name);
                else if (style is WTableStyle)
                    (style as WTableStyle).ApplyBaseStyle(baseStyle.Name);
                else
                    (style as Style).ApplyBaseStyle(baseStyle.Name);
            }
            return InnerList.Add(style);
        }
        /// <summary>
        /// Finds Style by name
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public IStyle FindByName(string name)
        {
            return FindStyleByName(InnerList, name);
        }
        /// <summary>
        /// Finds Style by name
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="styleType">Type of the style.</param>
        /// <returns></returns>
        public IStyle FindByName(string name, StyleType styleType)
        {
            return FindStyleByName(InnerList, name, styleType);
        }
        /// <summary>
        /// Finds Style by id
        /// </summary>
        /// <param name="styleId">The style id.</param>
        /// <returns></returns>
        public IStyle FindById(int styleId)
        {
            return FindStyleById(InnerList, styleId);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Clones to other collection.
        /// </summary>
        /// <param name="coll">The collection.</param>
#if SILVERLIGHT || WP
        internal void CloneToImpl(CollectionImpl coll)
#else
		internal override void CloneToImpl(CollectionImpl coll)
#endif
        {
            StyleCollection stColl = coll as StyleCollection;

            IStyle var = null;
            for (int i = 0, cnt = InnerList.Count; i < cnt; i++)
            {
                var = InnerList[i] as IStyle;
                stColl.Add(var.Clone());
            }
        }
        /// <summary>
        /// Finds the style with specified style name.
        /// </summary>
        /// <param name="styles">The styles.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        internal static IStyle FindStyleByName(IList styles, string name)
        {
            // Word takes the last found style with specified name. For example if document has
            // two "Normal" styles, the last in the list will be applied.
            IStyle retStyle = null;
            for (int i = 0; i < styles.Count; i++)
            {
                IStyle style = styles[i] as IStyle;

                if (style != null && style.Name == name)
                {
                    retStyle = style;
                }
            }

            return retStyle;
        }
        /// <summary>
        /// Finds the style with specified style name.
        /// </summary>
        /// <param name="styles">The styles.</param>
        /// <param name="name">The name.</param>
        /// <param name="styleType">Type of the style.</param>
        /// <returns></returns>
        internal static IStyle FindStyleByName(IList styles, string name, StyleType styleType)
        {
            if (name == null)
                return null;
            IStyle retStyle = null;
            for (int i = 0; i < styles.Count; i++)
            {
                Style style = styles[i] as Style;

                if (style != null && style.Name == name && style.StyleType == styleType)
                {
                    retStyle = style;
                }
                else
                {
                    if (style != null && style.StyleType == styleType && style.Name.Contains(name) && style.Name.Contains(","))
                    {
                        string[] styleNames = style.Name.Split(',');
                        foreach (string styleSubName in styleNames)
                        {
                            if (styleSubName.Trim() == name)
                                retStyle = style;
                        }
                    }
                }
            }

            return retStyle;
        }
        /// <summary>
        /// Finds the style with specified style name.
        /// </summary>
        /// <param name="styles">The styles.</param>
        /// <param name="name">The name.</param>
        /// <param name="styleType">Type of the style.</param>
        /// <returns></returns>
        internal static IStyle FindStyleById(IList styles, int styleId)
        {
            IStyle retStyle = null;
            for (int i = 0; i < styles.Count; i++)
            {
                Style style = styles[i] as Style;

                if (style != null && style.StyleId == styleId)
                {
                    retStyle = style;
                }
            }

            return retStyle;
        }
#if !SILVERLIGHT && !WP
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
                case "CharacterStyle":
                    return new CharacterStyle(Document);
                default:
                    return new WParagraphStyle(Document);
            }
        }
        /// <summary>
        /// Gets name of xml tag
        /// </summary>
        protected override string GetTagItemName()
        {
            return XDLSConstants.StyleItemTag;
        }
#endif
        #endregion
    }
}
