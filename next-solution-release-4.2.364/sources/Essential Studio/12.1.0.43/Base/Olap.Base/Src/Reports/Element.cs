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
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

#if !SILVERLIGHT
using System.Runtime.Serialization.Formatters.Binary;
using Syncfusion.Olap.Common;
using Syncfusion.Olap.Manager;
using Syncfusion.Olap.Data;

namespace Syncfusion.Olap.Reports
#else
using System.Runtime.Serialization;
using Syncfusion.OlapSilverlight.Data;
namespace Syncfusion.OlapSilverlight.Reports
#endif
{
    /// <summary>
    /// A class that enforces a boundary to the element objects
    /// </summary>
    /// <remarks>
    /// By Default member element will be marked as visible true, but HierarchyElement,
    /// LevelElement will be set as visible false, if the user wants to see this in the reslut set
    /// then he can set it to true
    /// </remarks>
#if !SILVERLIGHT
    [Serializable]
#else
    [DataContract]
#endif
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
    [XmlInclude(typeof(TopCountElement))]
    [XmlInclude(typeof(SubsetElement))]
    [XmlInclude(typeof(NamedSetElement))]
    [XmlInclude(typeof(CalculatedMembers))]
    [XmlInclude(typeof(CalculatedMember))]
    [XmlInclude(typeof(VirtualKpiElement))]

#if !SILVERLIGHT
    public class Element : ICloneable<Element>
#else
    public class Element
#endif
    {

        #region Private Variables
#if !SILVERLIGHT
        [NonSerialized]
#endif
        PropertyCollection __properties;
        #endregion


        #region Public Methods
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Element"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
#if SILVERLIGHT
        [DataMember]
#endif
        [DefaultValue(false)]
        public bool Visible { get; set; }
        /// <summary>
        /// Gets or sets the parent caption <see cref="Element"/> is visible.
        /// </summary>
        /// <value>string</value>
#if SILVERLIGHT
        [DataMember]
#endif
        [DefaultValue("")]
        public string RootNodeCaption { get; set; }

#if !SILVERLIGHT
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public Element Clone()
        {
            Element clone = null;
            MemoryStream memoryStream = null;
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                memoryStream = new MemoryStream();

                // Serialize the object in memory
                bf.Serialize(memoryStream, this);

                // Make sure all is loaded in the stream
                memoryStream.Flush();

                // Reset the position
                memoryStream.Position = 0;
                // Decoupled copy of the original object
                clone = (Element)bf.Deserialize(memoryStream);
            }
            catch (Exception ex)
            {
                throw new OlapDataManagerException("Cannot clone the object", ex);
            }

            return clone;
        }
#endif

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name of the Inherited Element</value>
#if SILVERLIGHT
        [DataMember]
#endif
        [DefaultValue((string)null)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets the Element Name
        /// </summary>
#if SILVERLIGHT
        [DataMember]
#endif
        [DefaultValue((string)null)]
        public string ElementName
        {
            get { return Name; }
            set { Name = value; }
        }

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>The properties.</value>
        [XmlIgnore(), DefaultValue((string)null)]
#if SILVERLIGHT
        //[DataMember]
#endif
        public PropertyCollection Properties 
        {
            get
            {
                if (__properties == null)
                {
                    __properties = new PropertyCollection();
                }

                return __properties;    
            }

            set
            {
                __properties = value;
            }
        }
        #endregion
    }
}
