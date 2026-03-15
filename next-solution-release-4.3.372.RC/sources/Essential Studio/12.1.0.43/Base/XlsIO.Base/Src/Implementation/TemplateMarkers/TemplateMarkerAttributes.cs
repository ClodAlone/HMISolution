#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;

namespace Syncfusion.XlsIO
{
    /// <summary>
    /// This class represents the marker attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TemplateMarkerAttributes: System.Attribute
    {
        /// <summary>
        /// Represents the header Names of the Property.
        /// </summary>
        private string m_headerName;
        /// <summary>
        /// Represents the number format of the property.
        /// </summary>
        private string m_numberFormat;
        /// <summary>
        /// Indicates whether to exclude the property or not.
        /// </summary>
        private bool m_bExclude;
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateMarkerAttributes"/> class.
        /// </summary>
        /// <param name="headerName">Name of the header.</param>
        public TemplateMarkerAttributes(string headerName)
        {
            m_headerName = headerName;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateMarkerAttributes"/> class.
        /// </summary>
        /// <param name="exclude">if set to <c>true</c> [exclude].</param>
        public TemplateMarkerAttributes(bool exclude)
        {
            m_headerName = null;
            m_numberFormat = null;
            m_bExclude = exclude;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateMarkerAttributes"/> class.
        /// </summary>
        /// <param name="exclude">Indicates whether to exclude the property.</param>
        /// <param name="headerName">Name of the header.</param>
        /// <param name="numberFormat">The number format of the property.</param>
        public TemplateMarkerAttributes(bool exclude, string headerName, string numberFormat)
        {
            if (exclude)
            {
                m_headerName = headerName;
                m_numberFormat = numberFormat;
            }
            else
            {
                m_headerName = null;
                m_numberFormat = null;
                m_bExclude = exclude;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateMarkerAttributes"/> class.
        /// </summary>
        /// <param name="exclude">Indicates whether to exclude the property.</param>
        /// <param name="numberFormat">The number format.</param>
        public TemplateMarkerAttributes(bool exclude, string numberFormat)
        {
            if (exclude)
            {
                m_headerName = null;
                m_numberFormat = numberFormat;
            }
            else
            {
                m_headerName = null;
                m_numberFormat = null;
                m_bExclude = exclude;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateMarkerAttributes"/> class.
        /// </summary>
        /// <param name="headerName">Name of the header.</param>
        /// <param name="numberFormat">The number format of the property.</param>
        public TemplateMarkerAttributes(string headerName,string numberFormat)
        {
            m_headerName = HeaderName;
            m_numberFormat = numberFormat;
        }
        /// <summary>
        /// Indicates whether to exclude the property or not.
        /// </summary>
        public bool Exclude
        {
            get
            {
                return m_bExclude;
            }
        }
        /// <summary>
        /// Represents the header Names of the Property.
        /// </summary>
        public string HeaderName
        {
            get
            {
                return m_headerName;
            }
        }
        /// <summary>
        /// Represents the number format of the property.
        /// </summary>
        public string NumberFormat
        {
            get
            {
                return m_numberFormat;
            }
        }
    }
}
