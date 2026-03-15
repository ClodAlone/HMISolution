//-------------------------------------------------------------------------------------------------
// <copyright file="Element.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Syncfusion.OlapSilverlight.Base.Report
{
    /// <summary>
    /// A class that enforces a boundray to the element objects
    /// </summary>
    /// <remarks>
    /// By Default member element will be marked as visible true, but HierarchyElement,
    /// LevelElement will be set as visible false, if the user wants to see this in the reslut set
    /// then he can set it to true
    /// </remarks>
    [XmlInclude(typeof(Element))]
    [XmlInclude(typeof(HierarchyElement))]
    [XmlInclude(typeof(DimensionElement))]
    [XmlInclude(typeof(LevelElement))]
    [XmlInclude(typeof(MeasureElement))]
    [XmlInclude(typeof(MeasureElements))]
    [XmlInclude(typeof(MemberElement))]
    [XmlInclude(typeof(FilterElement))]
    [XmlInclude(typeof(SortElement))]
    [XmlInclude(typeof(FilterValue))]
    [XmlInclude(typeof(KpiElements))]
    [DataContract]
    public class Element /*: ICloneable<Element>*/
    {

        #region Public Methods
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Element"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        [DataMember, DefaultValue(false)]
        public bool Visible { get; set; }


        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name of the Inherited Element</value>
        [DataMember, DefaultValue((string)null)]
        public string Name { get; set; }

        #endregion
    }
}
