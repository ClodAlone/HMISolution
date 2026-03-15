#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 

#endregion

#region file using directives
using System;
using System.ComponentModel;
using System.ComponentModel.Design;

using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

using Syncfusion.Windows.Forms.HTMLUI;

#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class implements the base logic of attributes which is a part of the HTML element.
    /// </summary>
    public class HTMLAttributeImpl
    : IHTMLAttribute
    {
        #region Class constants
        /// <summary>
        /// Checks the name on validity.
        /// </summary>
        private static readonly Regex DEF_NAME_CHECK =
          new Regex(@"\w", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #endregion

        #region Class members
        /// <summary>
        /// Parent element of the current attribute.
        /// </summary>
        private IHTMLElement m_parent;

        /// <summary>
        /// Storage of IsRuntimeAttribute property.
        /// </summary>
        private bool m_bRuntime;

        /// <summary>
        /// XML storage in parent XML element.
        /// </summary>
        private XmlAttribute m_attribute;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the unique name of the attribute.
        /// </summary>
        [Description("Gets unique name of attribute.")]
        public string Name
        {
            get
            {
                return m_attribute.Name;
            }
        }

        /// <summary>
        /// Gets or sets the current value of the attribute.
        /// </summary>
        public string Value
        {
            get
            {
                return m_attribute.Value;
            }
            set
            {
                if (value != m_attribute.Value)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_attribute.Value, value);
                    m_attribute.Value = value;
                    OnValueChanged(args);
                }
            }
        }

        /// <summary>
        /// Gets the parent of the current attribute.
        /// </summary>
        [ReadOnly(true), Description("Gets parent of current attribute.")]
        public IHTMLElement Parent
        {
            get
            {
                return m_parent;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the property is run-time and must not be serialized to text.
        /// </summary>
        [Browsable(false)]
        public bool IsRuntimeAttribute
        {
            get
            {
                return m_bRuntime;
            }
            set
            {
                m_bRuntime = value;
            }
        }

        /// <summary>
        /// Gets the XML storage in parent XML element.
        /// </summary>
        protected internal XmlElement ParentStorage
        {
            get
            {
                return ((BaseElement)this.Parent).Storage;
            }
        }
        #endregion

        #region Class events
        /// <summary>
        /// Event raised after attribute value changes. To event handlers,
        /// send new and old value of attribute.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler ValueChanged;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Prevents a default instance of the HTMLAttributeImpl class from being created
        /// </summary>
        private HTMLAttributeImpl()
        {
        }

        /// <summary>
        /// Initializes a new instance of the HTMLAttributeImpl class
        /// </summary>
        /// <param name="parent">Parent of current attribute.</param>
        private HTMLAttributeImpl(IHTMLElement parent)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            m_parent = parent;
        }

        /// <summary>
        /// Initializes a new instance of the HTMLAttributeImpl class
        /// </summary>
        /// <param name="parent">Parent of the current attribute.</param>
        /// <param name="name">Name of the current attribute.</param>
        public HTMLAttributeImpl(IHTMLElement parent, string name)
            : this(parent)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw new ArgumentException("Attribute name can not be empty.");

            if (!IsNameGood(name))
                throw new ArgumentException("Name can not be used as attribute name in HTML.");

            m_bRuntime = Array.IndexOf(((BaseElement)this.Parent).RuntimeAttributes, name) >= 0;
            m_attribute = this.ParentStorage.Attributes[name];

            AttachAttribute(name);
        }

        /// <summary>
        /// Initializes a new instance of the HTMLAttributeImpl class
        /// </summary>
        /// <param name="parent">Parent of attribute.</param>
        /// <param name="name">Name of attribute.</param>
        /// <param name="value">Value of attribute.</param>
        public HTMLAttributeImpl(IHTMLElement parent, string name, string value)
            : this(parent, name)
        {
            this.Value = value;
        }
        #endregion

        #region Class event raisers
        /// <summary>
        /// Raises the ValueChanged event.
        /// </summary>
        /// <param name="args">New and old value of property.</param>
        protected void RaiseValueChanged(ValueChangedEventArgs args)
        {
            BaseElement parentEx = this.Parent as BaseElement;

            if (ValueChanged != null && !parentEx.QuietMode)
            {
                ValueChanged(this, args);
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// On value property change, this method is called by property set part.
        /// This is best place for own logic.
        /// </summary>
        /// <param name="args">Old and new value of property.</param>
        protected virtual void OnValueChanged(ValueChangedEventArgs args)
        {
            RaiseValueChanged(args);
        }

        /// <summary>
        /// Overridden. Returns the attribute as string suitable for HTML.
        /// </summary>
        /// <returns>Attribute string suitable for direct use in HTML as part of
        /// element declaration.</returns>
        public override string ToString()
        {
            return m_attribute.OuterXml;
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Indicates whether the name of the attribute is good for HTML.
        /// </summary>
        /// <param name="name">Name of the attribute.</param>
        /// <returns>True if name is good.</returns>
        protected bool IsNameGood(string name)
        {
            return DEF_NAME_CHECK.Match(name).Success;
        }

        /// <summary>
        /// Attaches attribute to the XML of the parent object.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        private void AttachAttribute(string attributeName)
        {
            if (attributeName == null)
                throw new ArgumentNullException("attributeName");
            if (attributeName.Length == 0)
                throw new ArgumentException("Attribute attributeName can not be empty.");

            m_attribute = this.ParentStorage.Attributes[attributeName];

            if (m_attribute == null)
            {
                this.ParentStorage.SetAttribute(attributeName, string.Empty);
                m_attribute = this.ParentStorage.Attributes[attributeName];

                if (m_attribute == null)
                    throw new NullReferenceException("Cannot create XML attrbiute for node.");
            }
        }

        /// <summary>
        /// Detaches attribute from the parent object XML.
        /// </summary>
        internal void DetachAttribute()
        {
            this.ParentStorage.RemoveAttribute(Name);
        }
        #endregion
    }
}