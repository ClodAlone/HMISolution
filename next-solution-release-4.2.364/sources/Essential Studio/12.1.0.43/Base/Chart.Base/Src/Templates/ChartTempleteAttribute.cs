#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Represents the type of property serialization.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public enum ChartTemplateSet
    {
        /// <summary>
        /// The value of property can be converted to the <see cref="String"/>.
        /// </summary>
        Simple,

        /// <summary>
        /// The value of property is collection.
        /// </summary>
        Collection,

        /// <summary>
        /// The value of property is collection.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        SimpleAndCollection,      

        /// <summary>
        /// The property contains sub-properties.
        /// </summary>
        Content,

        /// <summary>
        /// The value of property can be converted to the <see cref="String"/>. The property of this attribute is not related to appearance.
        /// </summary>
        SimpleBehavior,

        /// <summary>
        /// The value of property is collection. The property of this attribute is not related to appearance.
        /// </summary>
        CollectionBehavior, 

        /// <summary>
        /// The value of property is collection. The property of this attribute is not related to appearance.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        SimpleAndCollectionBehavior, 

        /// <summary>
        /// The property contains sub-properties. The property of this attribute is not related to appearance.
        /// </summary>
        ContentBehavior 
    }

    /// <summary>
    /// The ChartTemplateAttribute class.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ChartTemplateAttribute : Attribute
    {
        #region Members
        private Type m_itemType = null;
        private ChartTemplateSet m_setType = ChartTemplateSet.Simple;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <value>The type of the property.</value>
        public ChartTemplateSet SetType
        {
            get
            {
                return m_setType;
            }
        }

        /// <summary>
        /// Gets the type of the collection items.
        /// </summary>
        /// <value>The type of the item.</value>
        public Type ItemType
        {
            get
            {
                return m_itemType;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartTemplateAttribute"/> class.
        /// </summary>
        /// <param name="setType">Type of the set.</param>
        public ChartTemplateAttribute(ChartTemplateSet setType)
        {
            m_setType = setType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartTemplateAttribute"/> class.
        /// </summary>
        /// <param name="setType">Type of the set.</param>
        /// <param name="itemType">Type of the item.</param>
        public ChartTemplateAttribute(ChartTemplateSet setType, Type itemType)
        {
            m_itemType = itemType;
            m_setType = setType;
        }
        #endregion
    }
}