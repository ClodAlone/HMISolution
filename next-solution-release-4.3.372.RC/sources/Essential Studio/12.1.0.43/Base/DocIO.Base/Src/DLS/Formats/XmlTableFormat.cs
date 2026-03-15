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
using System.Collections;
using System.Xml;
using System.Collections.Generic;
using System.IO;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Summary description for XmlTableFormat.
    /// </summary>
    public class XmlTableFormat
    {
        #region Fields
        private List<Stream> m_nodeArr;
        private string m_styleName;
        private WTable m_ownerTable;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the node array with unparsed table properties.
        /// </summary>
        /// <value>The node array.</value>
        internal List<Stream> NodeArray
        {
            get
            {
                if (m_nodeArr == null)
                {
                    m_nodeArr = new List<Stream>();
                }
                return m_nodeArr;
            }
            set
            {
                m_nodeArr = value;
            }
        }
        /// <summary>
        /// Gets or sets the name of the table style.
        /// </summary>
        /// <value>The name of the style.</value>
        internal string StyleName
        {
            get
            {
                return m_styleName;
            }
            set
            {
                m_styleName = value;
            }
        }
        /// <summary>
        /// Gets the format.
        /// </summary>
        /// <value>The format.</value>
        internal RowFormat Format
        {
            get
            {
                return m_ownerTable.TableFormat;
            }
        }
        /// <summary>
        /// Gets a value indicating whether this instance has format.
        /// </summary>
        /// <value>
        /// 	if this instance has format, set to <c>true</c>.
        /// </value>
        internal bool HasFormat
        {
            get
            {
                if (m_styleName != null 
                    || (m_nodeArr != null && m_nodeArr.Count > 0))
                {
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// Gets the owner.
        /// </summary>
        /// <value>The owner.</value>
        internal WTable Owner
        {
            get
            {
                return m_ownerTable;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlTableFormat"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        internal XmlTableFormat(WTable owner)
        {
            m_ownerTable = owner;
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Clones the specified owner table.
        /// </summary>
        /// <param name="ownerTable">The owner table.</param>
        /// <returns></returns>
        internal XmlTableFormat Clone(WTable ownerTable)
        {
            XmlTableFormat xmlTblFormat = new XmlTableFormat(ownerTable);
            xmlTblFormat.StyleName = m_styleName;
            xmlTblFormat.Owner.SetOwner(ownerTable.OwnerTextBody);
            xmlTblFormat.NodeArray = m_nodeArr;
            return xmlTblFormat;
        }
        /// <summary>
        /// Closes this instance.
        /// </summary>
        internal void Close()
        {
            if (m_nodeArr != null)
            {
                m_nodeArr.Clear();
                m_nodeArr = null;
            }
        }
        #endregion
    }
}

