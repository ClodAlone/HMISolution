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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Editor for element attributes.
    /// </summary>
    public sealed class AttributesCollectionEditor : CollectionEditor
    {
        #region Class members

        /// <summary>
        ///  Data types that this collection editor can contain.
        /// </summary>
        private Type[] m_types = new Type[] { typeof(HTMLAttributesCollection) };

        /// <summary>
        /// Holds an array of invisible elements.
        /// </summary>
        private ArrayList m_invisibles = new ArrayList();
        #endregion

        #region Class Initialize/Finalize methods

        /// <summary>
        /// Initializes a new instance of the AttributesCollectionEditor class
        /// </summary>
        /// <param name="type">Type of element to edit.</param>
        public AttributesCollectionEditor(Type type)
            : base(type)
        {
        }
        #endregion

        #region Class overrides

        /// <summary>
        /// Overrides CreateNewItemTypes() method
        /// </summary>
        /// <returns>A array of types.</returns>
        protected override Type[] CreateNewItemTypes()
        {
            return m_types;
        }

        /// <summary>
        /// Overridden. Returns an array of objects containing the specified collection.
        /// </summary>
        /// <param name="editValue">The collection to edit. </param>
        /// <returns>An array containing the collection objects.</returns>
        protected override object[] GetItems(object editValue)
        {
            object[] parentArray = base.GetItems(editValue);
            ArrayList result = new ArrayList(parentArray);
            m_invisibles.Clear();

            IHTMLAttributesCollection collection = editValue as HTMLAttributesCollection;

            if (collection != null)
            {
                IHTMLAttribute attr;

                attr = collection[BaseElement.DEF_RUNTIME_LOCATION];

                if (attr != null)
                {
                    result.Remove(attr);
                    m_invisibles.Add(attr);
                }

                attr = collection[BaseElement.DEF_RUNTIME_SIZE];

                if (attr != null)
                {
                    result.Remove(attr);
                    m_invisibles.Add(attr);
                }

                attr = collection[BaseElement.DEF_RUNTIME_VISIBLE];

                if (attr != null)
                {
                    result.Remove(attr);
                    m_invisibles.Add(attr);
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Overridden. Indicates whether the specified element can be removed from the collection.
        /// </summary>
        /// <param name="value">Item of the collection.</param>
        /// <returns>bool value</returns>
        protected override bool CanRemoveInstance(object value)
        {
            return false;
        }

        /// <summary>
        /// Overridden. Destroys the specified item from the collection.
        /// </summary>
        /// <param name="instance">object to be destroyed</param>
        protected override void DestroyInstance(object instance)
        {
            base.DestroyInstance(instance);
        }

        /// <summary>
        /// Overridden. Sets the specified items to the collection.
        /// </summary>
        /// <param name="editValue">Editable value</param>
        /// <param name="value">Item values</param>
        /// <returns>The newly created collection object</returns>
        protected override object SetItems(object editValue, object[] value)
        {
            return editValue;
        }
        #endregion
    }
}
