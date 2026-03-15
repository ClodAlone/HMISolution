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

using System;
using System.Xml;

namespace Syncfusion.Pdf.Xmp
{
    /// <summary>
    /// Represents simple Xmp type.
    /// </summary>
    public class XmpSimpleType : XmpType
    {
        #region Properties
        /// <summary>
        /// Gets or sets value of the instance.
        /// </summary>
        public string Value
        {
            get
            {
                if (XmlData != null)
                    return XmlData.InnerXml;
                else
                    return string.Empty;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Value");
                }

                XmpUtils.SetTextValue(XmlData, value);
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates xmp simple type instance.
        /// </summary>
        /// <param name="xmp">Parent XmpMetadata.</param>
        /// <param name="parent">Parent xml node.</param>
        /// <param name="prefix">Namespace prefix.</param>
        /// <param name="localName">Name of the tag.</param>
        /// <param name="namespaceURI">Namespace URI.</param>
        internal XmpSimpleType(XmpMetadata xmp, XmlNode parent, string prefix, string localName, string namespaceURI)
            : base(xmp, parent, prefix, localName, namespaceURI)
        {
        }
        #endregion

        #region Class Set methods
        /// <summary>
        /// Sets bool value to the object.
        /// </summary>
        /// <param name="value">Bool value.</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal protected void SetBool(bool value)
        {
            XmpUtils.SetBoolValue(XmlData, value);
        }

        /// <summary>
        /// Gets boolean value.
        /// </summary>
        /// <returns>Boolean value.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal protected bool GetBool()
        {
            bool result = XmpUtils.GetBoolValue(this.Value);

            return result;
        }

        /// <summary>
        /// Sets real value to the object.
        /// </summary>
        /// <param name="value">Real value.</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal protected void SetReal(float value)
        {
            XmpUtils.SetRealValue(XmlData, value);
        }

        /// <summary>
        /// Gets real value.
        /// </summary>
        /// <returns>Real value.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal protected float GetReal()
        {
            float result = XmpUtils.GetRealValue(this.Value);

            return result;
        }

        /// <summary>
        /// Sets int value to the object.
        /// </summary>
        /// <param name="value">Int value.</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal protected void SetInt(int value)
        {
            XmpUtils.SetIntValue(XmlData, value);
        }

        /// <summary>
        /// Gets int value.
        /// </summary>
        /// <returns>Int value.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal protected int GetInt()
        {
            int result = XmpUtils.GetIntValue(this.Value);

            return result;
        }

        /// <summary>
        /// Sets unique resource identifier value to the object.
        /// </summary>
        /// <param name="value">Uri value.</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal protected void SetUri(Uri value)
        {
            XmpUtils.SetUriValue(XmlData, value);
        }

        /// <summary>
        /// Gets unique resource identifier value.
        /// </summary>
        /// <returns>Uri value.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal protected Uri GetUri()
        {
            Uri result = XmpUtils.GetUriValue(this.Value);

            return result;
        }

        /// <summary>
        /// Sets DateTime value to the object.
        /// </summary>
        /// <param name="value">DateTime value.</param>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal protected void SetDateTime(DateTime value)
        {
            XmpUtils.SetDateTimeValue(XmlData, value);
        }

        /// <summary>
        /// Gets DateTime value.
        /// </summary>
        /// <returns>DateTime value.</returns>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal protected DateTime GetDateTime()
        {
            DateTime result = XmpUtils.GetDateTimeValue(this.Value);

            return result;
        }

        #endregion

        #region Overrides
        /// <summary>
        /// Creates entity in the parent.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected override void CreateEntity()
        {
            XmlElement element = Xmp.CreateElement(EntityPrefix, EntityName, EntityNamespaceURI);
            EntityParent.AppendChild(element);
        }
        #endregion
    }
}
