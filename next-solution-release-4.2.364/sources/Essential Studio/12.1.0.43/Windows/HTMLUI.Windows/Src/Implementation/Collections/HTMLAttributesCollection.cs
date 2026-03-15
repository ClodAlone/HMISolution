#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using Syncfusion.HTMLUI.Base.Utility;
using Syncfusion.Windows.Forms.HTMLUI;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
using Syncfusion.Windows.Forms.HTMLUI.Implementation.Utility;

#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Collection of attributes in the tag elements.
    /// </summary>
    public class HTMLAttributesCollection
      : EventBaseCollection, IHTMLAttributesCollection
    {
        #region Class members

        /// <summary>
        /// Storage of Parent property.
        /// </summary>
        private IHTMLElement m_parent;

        /// <summary>
        /// Storage of name of attribute - to - attribute.
        /// </summary>
        private IDictionary m_dict = CollectionsUtil.CreateCaseInsensitiveHashtable();

        #endregion

        #region Class Properties

        /// <summary>
        /// Gets the parent element of the current collection.
        /// </summary>
        public IHTMLElement Parent
        {
            get
            {
                return m_parent;
            }
        }

        /// <summary>
        /// Gets or sets the attribute with the specified index.
        /// </summary>
        /// <param name="index">an int value</param>
        public IHTMLAttribute this[int index]
        {
            get
            {
                return (IHTMLAttribute)List[index];
            }
            set
            {
                List[index] = value;
            }
        }

        /// <summary>
        /// Gets or sets the attribute with the specified name. Name is case insensitive.
        /// </summary>
        /// <param name="name">String value</param>
        public IHTMLAttribute this[string name]
        {
            get
            {
                if (name == null)
                    throw new ArgumentNullException("name");

                if (name.Length == 0)
                    throw new ArgumentException("name can not be empty");

                if (!m_dict.Contains(name)) return null;

                return (IHTMLAttribute)m_dict[name];
            }
            set
            {
                if (name == null)
                    throw new ArgumentNullException("name");

                if (name.Length == 0)
                    throw new ArgumentException("name can not be empty");

                if (!m_dict.Contains(name))
                {
                    // add new
                    this.Add(value);
                }
                else
                {
                    // replace
                    List[InnerList.IndexOf(m_dict[name])] = value;
                }
            }
        }
        #endregion

        #region Class events

        /// <summary>
        /// Event raised on any attribute value change in the collection.
        /// </summary>
        public event BeforeValueChangeEventHandler Changed;
        #endregion

        #region Class Initialize/Finalize methods

        /// <summary>
        /// Prevents a default instance of the HTMLAttributesCollection class from being created
        /// </summary>
        private HTMLAttributesCollection()
        {
            this.Inserted += new CollectionEventHandler(HTMLAttributesCollection_Inserted);
            this.Removed += new CollectionEventHandler(HTMLAttributesCollection_Removed);
            this.Set += new CollectionEventHandler(HTMLAttributesCollection_Set);
            this.Cleared += new CollectionEventHandler(HTMLAttributesCollection_Cleared);
        }

        /// <summary>
        /// Initializes a new instance of the HTMLAttributesCollection class
        /// </summary>
        /// <param name="parent">Parent element.</param>
        public HTMLAttributesCollection(IHTMLElement parent)
            : this()
        {
            m_parent = parent;
        }

        /// <summary>
        /// Finalizes an instance of the HTMLAttributesCollection class
        /// </summary>
        ~HTMLAttributesCollection()
        {
            this.Inserted -= new CollectionEventHandler(HTMLAttributesCollection_Inserted);
            this.Removed -= new CollectionEventHandler(HTMLAttributesCollection_Removed);
            this.Set -= new CollectionEventHandler(HTMLAttributesCollection_Set);
            this.Cleared -= new CollectionEventHandler(HTMLAttributesCollection_Cleared);
        }
        #endregion

        #region Class Helper Methods

        /// <summary>
        /// Adds specified attribute into the collection.
        /// </summary>
        /// <param name="attr">Attribute for adding into collection.</param>
        /// <returns>Index in the collection.</returns>
        public int Add(IHTMLAttribute attr)
        {
            if (attr == null)
                throw new ArgumentNullException("attr");

            if (m_dict.Contains(attr.Name))
                throw new ArgumentException("Two attributes with the same name can not be added into collection.");
            return List.Add(attr);
        }

        /// <summary>
        /// Creates an attribute with specified name and returns the reference to it.
        /// </summary>
        /// <param name="name">Name of the attribute. Case insensitive.</param>
        /// <returns>Reference of created attribute.</returns>
        public IHTMLAttribute Add(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Length == 0)
                throw new ArgumentException("name of attribute can not be empty");

            if (m_dict.Contains(name))
                throw new ArgumentException("Two attributes with the same name can not be added into collection.");

            IHTMLAttribute attr = new HTMLAttributeImpl(this.Parent, name);
            this.Add(attr);
            return attr;
        }

        /// <summary>
        /// Creates attribute in the collection and returns the reference to it.
        /// </summary>
        /// <param name="name">Name of the attribute. Case insensitive.</param>
        /// <param name="value">Value of the attribute.</param>
        /// <returns>Reference of created attribute.</returns>
        public IHTMLAttribute Add(string name, string value)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Length == 0)
                throw new ArgumentException("name of attribute can not be empty");

            if (m_dict.Contains(name))
                throw new ArgumentException("Two attributes with the same name can not be added into collection.");

            IHTMLAttribute attr = new HTMLAttributeImpl(this.Parent, name);
            this.Add(attr);

            // NOTE: assign value for "ValueChanged" event raising.
            attr.Value = value;

            return attr;
        }

        /// <summary>
        /// Adds the range of attributes into the collection.
        /// </summary>
        /// <param name="attributes">Array of attributes.</param>
        public void AddRange(IHTMLAttribute[] attributes)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            for (int i = 0; i < attributes.Length; i++)
            {
                List.Add(attributes[i]);
            }
        }

        /// <summary>
        /// Overloaded. Indicates whether the collection contains the specified attribute.
        /// </summary>
        /// <param name="attr">Attribute reference for check.</param>
        /// <returns>True if collection contains such an attribute; false otherwise.</returns>
        public bool Contains(IHTMLAttribute attr)
        {
            if (attr == null) return false;

            return InnerList.Contains(attr);
        }

        /// <summary>
        /// Indicates whether collection contains the attribute with the specified name.
        /// </summary>
        /// <param name="name">Case insensitive name of the attribute.</param>
        /// <returns>True if collection contains such an attribute; false otherwise.</returns>
        public bool Contains(string name)
        {
            if (name == null || name.Length == 0) return false;

            return m_dict.Contains(name);
        }

        /// <summary>
        /// Returns the index of the specified attribute; if collection does not contain such attribute it 
        /// will return -1.
        /// </summary>
        /// <param name="attr">Attribute whose index is needed.</param>
        /// <returns>Zero-based index of the attribute; -1 otherwise.</returns>
        public int IndexOf(IHTMLAttribute attr)
        {
            if (attr == null) return -1;

            return InnerList.IndexOf(attr);
        }

        /// <summary>
        /// Returns the index of the attribute with the specified name.
        /// </summary>
        /// <param name="name">Case insensitive name of the attribute.</param>
        /// <returns>Zero-based index of the attribute; -1 otherwise.</returns>
        public int IndexOf(string name)
        {
            if (name == null || name.Length == 0) return -1;
            if (!m_dict.Contains(name)) return -1;

            return this.IndexOf(this[name]);
        }

        /// <summary>
        /// Overloaded. Removes the specified attribute from the collection, if it is belong to it.
        /// </summary>
        /// <param name="attr">Reference of the attribute that must be removed from the collection.</param>
        public void Remove(IHTMLAttribute attr)
        {
            if (attr == null)
                throw new ArgumentNullException("attr");

            List.Remove(attr);
            (attr as HTMLAttributeImpl).DetachAttribute();
        }

        /// <summary>
        /// Removes the attribute with the specified name.
        /// </summary>
        /// <param name="name">Case insensitive name of the attribute which must be removed.</param>
        public void Remove(string name)
        {
            this.Remove(this[name]);
        }
        #endregion

        #region Class utility methods - keep sync

        /// <summary>
        /// Catch Inserted items.
        /// </summary>
        /// <param name="sender">Source of the event</param>
        /// <param name="e">CollectionEventArgs instance</param>
        private void HTMLAttributesCollection_Inserted(object sender, CollectionEventArgs e)
        {
            HTMLAttributeImpl attr = (HTMLAttributeImpl)e.Value;
            attr.ValueChanged += new ValueChangedEventHandler(Attr_ValueChanged);

            m_dict.Add(attr.Name, attr);
        }

        /// <summary>
        /// Catch removed items.
        /// </summary>
        /// <param name="sender">Source of the event</param>
        /// <param name="e">CollectionEventArgs instance</param>
        private void HTMLAttributesCollection_Removed(object sender, CollectionEventArgs e)
        {
            HTMLAttributeImpl attr = (HTMLAttributeImpl)e.Value;
            attr.ValueChanged -= new ValueChangedEventHandler(Attr_ValueChanged);

            m_dict.Remove(attr.Name);
        }

        /// <summary>
        /// Catch set items.
        /// </summary>
        /// <param name="sender">Source of the event</param>
        /// <param name="e">CollectionEventArgs instance</param>
        private void HTMLAttributesCollection_Set(object sender, CollectionEventArgs e)
        {
            HTMLAttributeImpl attr = (HTMLAttributeImpl)e.Value;
            HTMLAttributeImpl attr2 = (HTMLAttributeImpl)e.OldValue;

            attr.ValueChanged += new ValueChangedEventHandler(Attr_ValueChanged);
            attr2.ValueChanged -= new ValueChangedEventHandler(Attr_ValueChanged);

            m_dict.Remove(attr2.Name);
            m_dict.Add(attr.Name, attr);
        }

        /// <summary>
        /// Catch collection cleaning.
        /// </summary>
        /// <param name="sender">Source of the event</param>
        /// <param name="e">CollectionEventArgs instance</param>
        private void HTMLAttributesCollection_Cleared(object sender, CollectionEventArgs e)
        {
            m_dict.Clear();
        }
        #endregion

        #region Class attributes value change catcher

        /// <summary>
        /// Catches value change of attribute in the collection and
        /// raises up information about this change if someone listens to it.
        /// </summary>
        /// <param name="sender">Sender which supports IHTMLAttribute interface.</param>
        /// <param name="e">Data of changed attribute. Old and New value.</param>
        private void Attr_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            IHTMLAttribute attr = (IHTMLAttribute)sender;

            ChangeRuntimeAttributes(attr, e);

            BaseElement parent = m_parent as BaseElement;
            if (Changed != null && !parent.QuietMode && !parent.Control.IsLoading)
            {
                BeforeValueChangedEventArgs args = new BeforeValueChangedEventArgs(attr.Name, e);

                Changed(this, args);
            }
        }

        /// <summary>
        /// Changes run-time attributes in the parent element.
        /// </summary>
        /// <param name="attr">Attribute for changing.</param>
        /// <param name="e">Event arguments.</param>
        private void ChangeRuntimeAttributes(IHTMLAttribute attr, ValueChangedEventArgs e)
        {
            if (attr == null)
                throw new ArgumentNullException("attr");

            if (e == null)
                throw new ArgumentNullException("e");

            if (e.OldValue.Equals(e.NewValue)) return;

            if (m_parent != null)
            {
                BaseElement parent = m_parent as BaseElement;
                if (!parent.IsAttributeRuntime(attr.Name)) return;

                switch (attr.Name)
                {
                    case BaseElement.DEF_RUNTIME_VISIBLE:
                        parent.IsVisible = Utilities.ConvertToBool((string)e.NewValue);
                        break;

                    case BaseElement.DEF_RUNTIME_SIZE:
                        parent.Size = Utilities.ConvertToSize((string)e.NewValue);
                        break;

                    case BaseElement.DEF_RUNTIME_LOCATION:
                        parent.Location = Utilities.ConvertToPoint((string)e.NewValue);
                        break;
                }
            }
        }
        #endregion
    }
}