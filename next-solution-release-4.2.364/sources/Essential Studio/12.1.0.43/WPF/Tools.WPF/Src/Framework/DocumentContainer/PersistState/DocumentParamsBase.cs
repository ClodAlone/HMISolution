// <copyright file="DocumentParamsBase.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows;
using System.Xml.Serialization;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Presents base class for persist state params.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [Serializable]
    [SoapInclude(typeof(ChildDocumentParams))]
    [XmlInclude(typeof(ChildDocumentParams))]
    [SoapInclude(typeof(MainDocumentParams))]
    [XmlInclude(typeof(MainDocumentParams))]
    public abstract class DocumentParamsBase : ISerializable
    {
        #region Constants
        /// <summary>
        /// Represents the String for making the window active
        /// </summary>
        protected const string ISACTIVE_PARAMNAME = "IsActive";
        #endregion

        #region Private members
        /// <summary>
        /// Presents PropertiesMode
        /// </summary>
        private readonly PropertiesMode m_PropertiesMode;
        
        /// <summary>
        /// Element to store parameters.
        /// </summary>
        private readonly FrameworkElement m_Element;
        
        /// <summary>
        /// Presents table of docking parameters.
        /// </summary>
        private readonly Hashtable m_DocParamsTable;
        #endregion

        #region Propeties
        /// <summary>
        /// Gets the properties mode.
        /// </summary>
        /// <value>The properties mode.</value>
        protected PropertiesMode PropertiesMode
        {
            get
            {
                return m_PropertiesMode;
            }
        }
        
        /// <summary>
        /// Gets the element.
        /// </summary>
        /// <value>The element.</value>
        protected FrameworkElement Element
        {
            get
            {
                return m_Element;
            }
        }
        
        /// <summary>
        /// Gets the doc params table.
        /// </summary>
        /// <value>The doc params table.</value>
        protected Hashtable DocParamsTable
        {
            get
            {
                return m_DocParamsTable;
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentParamsBase"/> class.
        /// </summary>
        protected DocumentParamsBase()
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentParamsBase"/> class.
        /// </summary>
        /// <param name="element">The element value.</param>
        /// <param name="mode">The mode element.</param>
        protected DocumentParamsBase(FrameworkElement element, PropertiesMode mode)
        {
            m_Element = element;
            m_PropertiesMode = mode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentParamsBase"/> class.
        /// </summary>
        /// <param name="info">The info DocumentParamsBase.</param>
        /// <param name="context">The context DocumentParamsBase.</param>
        protected DocumentParamsBase(SerializationInfo info, StreamingContext context)
        {
            PropertiesMode mode = 0 == ParamsTable.Params.Count ? PropertiesMode.Main : PropertiesMode.Child;
            List<DependencyProperty> depPropertyList = DocumentContainer.GetListSerializedProperties(mode);
            m_DocParamsTable = new Hashtable();

            foreach (DependencyProperty depProperty in depPropertyList)
            {
                object propValue = info.GetValue(depProperty.Name, typeof(object));
                m_DocParamsTable.Add(depProperty, propValue);
            }

            ParamsTable.Params.Add(Guid.NewGuid(), m_DocParamsTable);
        }
        #endregion

        #region ISerializable Members
        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            List<DependencyProperty> depPropertyList = DocumentContainer.GetListSerializedProperties(m_PropertiesMode);

            foreach (DependencyProperty depProperty in depPropertyList)
            {
                object propValue = m_Element.GetValue(depProperty);
                info.AddValue(depProperty.Name, propValue);
            }

            if (m_PropertiesMode == PropertiesMode.Child)
            {
                DocumentContainer container = DocumentContainer.GetDocumentContainer(m_Element);
                bool active = container.ActiveDocument == m_Element;
                info.AddValue(ISACTIVE_PARAMNAME, active);
            }
        }
        #endregion
    }
}
