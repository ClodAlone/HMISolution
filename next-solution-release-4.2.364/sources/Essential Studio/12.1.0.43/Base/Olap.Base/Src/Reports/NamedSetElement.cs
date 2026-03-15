//-------------------------------------------------------------------------------------------------
// <copyright file="NamedSetElement.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Xml.Serialization;
#if !SILVERLIGHT
using Syncfusion.Olap.Common;
using Syncfusion.Olap.Data;

namespace Syncfusion.Olap.Reports
{
    /// <summary>
    /// Represents the named set element information.
    /// </summary>
    [Serializable]
    public class NamedSetElement : Element, ICloneable<NamedSetElement>
#else

using System.Runtime.Serialization;
using Syncfusion.OlapSilverlight.Common;
using Syncfusion.OlapSilverlight.Data;

namespace Syncfusion.OlapSilverlight.Reports
{
    [DataContract]
    public class NamedSetElement : Element
#endif
    {
        #region Private Properties
        private DimensionElement _parentDimension;
        string _uniquename;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="NamedSetElement"/> class.
        /// </summary>
        /// <param name="parentDimensionElement">The parent dimension element.</param>
        public NamedSetElement(DimensionElement parentDimensionElement)
        {
            this.Name = string.Empty;
            this.DimensionName = string.Empty;
            this.Properties = new PropertyCollection();

            this.ParentDimension = parentDimensionElement;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedSetElement"/> class.
        /// </summary>
        public NamedSetElement()
        {
            this.Name = string.Empty;
            this.DimensionName = string.Empty;
            this.Properties = new PropertyCollection();
        }
        #endregion

        #region Internal Properties
        /// <summary>
        /// Gets or sets the parent dimension.
        /// </summary>
        /// <value>The parent dimension.</value>
        [XmlIgnoreAttribute(), DefaultValue((string)null)]
        public DimensionElement ParentDimension 
        { 
            get 
            { 
                return _parentDimension; 
            } 

            set 
            { 
                _parentDimension = value; 
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the unique name.
        /// </summary>
        /// <value>The unique name of <see cref="NamedSetElement"/>.</value>
        [XmlIgnoreAttribute()]
        public string UniqueName
        {
            get
            {
                if (!string.IsNullOrEmpty(_uniquename))
                    return _uniquename;
                return Utils.QuoteIdentifier(this.Name);
            }
            set
            {
                _uniquename = value;
            }
        }

        /// <summary>
        /// Gets or sets the dimension unique name.
        /// </summary>
        /// <value>The dimension unique name of <see cref="NamedSetElement"/>.</value>
        [XmlIgnoreAttribute()]
        public string DimensionUniqueName
        {
            get
            {
                return Utils.QuoteIdentifier(this.DimensionName);
            }
            set
            {
                _uniquename = value;
            }
        }
#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets a value indicating whether this instance is query scoped.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is query scoped; otherwise, <c>false</c>.
        /// </value>
        public bool IsQueryScoped { get; set; }

#if SILVERLIGHT
        [DataMember]
#endif
        /// <summary>
        /// Gets or sets the set query.
        /// </summary>
        /// <value>The set query.</value>
        public string SetQuery { get; set; }

        /// <summary>
        /// Gets or sets the name of the dimension.
        /// </summary>
        /// <value>The name of the dimension.</value>
        public string DimensionName { get; set; }

#if !SILVERLIGHT
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A copy of <see cref="NamedSetElement"/>.</returns>
        public new NamedSetElement Clone()
        {
            NamedSetElement namedSetElement = new NamedSetElement(this.ParentDimension);
            namedSetElement.Name = this.Name;
            namedSetElement.DimensionName = this.DimensionName;
            namedSetElement.IsQueryScoped = this.IsQueryScoped;
            namedSetElement.SetQuery = this.SetQuery;
            namedSetElement.Properties = this.Properties.Clone();
            return namedSetElement;
        }
#endif
        #endregion
    }
}
