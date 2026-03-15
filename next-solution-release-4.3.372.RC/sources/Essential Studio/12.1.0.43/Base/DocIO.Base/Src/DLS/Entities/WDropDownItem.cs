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
using System.Runtime.InteropServices;

using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO.DLS.XML;
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// Represents the item of drop down formfield.
    /// </summary>
    public class WDropDownItem
      : XDLSSerializableBase
    {
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        private string m_text = "";
        #endregion

        #region Class properties
        /// <summary>
        /// Gets / sets current DropDownItem text
        /// </summary>
        public string Text
        {
            get
            {
                return m_text;
            }
            set
            {
                m_text = value;
            }
        }
        #endregion

        #region Class initialize  / finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="WDropDownItem"/> class.
        /// </summary>
        /// <param name="doc">The document</param>
        public WDropDownItem(IWordDocument doc)
            : base((WordDocument)doc, null)
        { }
        #endregion

        #region Class overrides
//#if !SILVERLIGHT
        /// <summary>
        /// Writes object data as xml attributes.
        /// </summary>
        /// <param name="writer"></param>
        protected override void WriteXmlAttributes(IXDLSAttributeWriter writer)
        {
            base.WriteXmlAttributes(writer);

            writer.WriteValue(XDLSConstants.FormFieldDropDownItemTextAttr, m_text);
        }
        /// <summary>
        /// Reads object data from xml attributes.
        /// </summary>
        /// <param name="reader"></param>
        protected override void ReadXmlAttributes(IXDLSAttributeReader reader)
        {
            base.ReadXmlAttributes(reader);

            if (reader.HasAttribute(XDLSConstants.FormFieldDropDownItemTextAttr))
            {
                m_text = reader.ReadString(XDLSConstants.FormFieldDropDownItemTextAttr);
            }
        }
//#endif
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        internal WDropDownItem Clone()
        {
            return (WDropDownItem)CloneImpl();
        }
        #endregion
    }
}
