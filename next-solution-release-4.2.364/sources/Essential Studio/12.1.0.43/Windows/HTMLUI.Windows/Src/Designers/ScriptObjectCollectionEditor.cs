#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
////
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
    /// Description for ScriptObjectCollectionEditor.
    /// </summary>
    public sealed class ScriptObjectCollectionEditor : CollectionEditor
    {
        #region Class members
        /// <summary>
        /// Collection of the ScriptManagerExCollection objects.
        /// </summary>
        private ScriptManagerExCollection m_parent;

        /// <summary>
        /// Data types that this collection editor can contain.
        /// </summary>
        private Type[] m_types = new Type[] { typeof(ScriptManagerEx) };

        #endregion

        #region Class Initialize/Finalize methods

        /// <summary>
        /// Initializes a new instance of the ScriptObjectCollectionEditor class
        /// </summary>
        /// <param name="type">Type of element to edit.</param>
        public ScriptObjectCollectionEditor(Type type)
            : base(type)
        {
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Override CreateNewItemTypes() method
        /// </summary>
        /// <returns>Aray of types.</returns>
        protected override Type[] CreateNewItemTypes()
        {
            return m_types;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="itemType">Type of object for creating.</param>
        /// <returns>New created object.</returns>
        protected override object CreateInstance(Type itemType)
        {
            if (m_parent != null)
            {
                return m_parent.CreateItem();
            }

            return base.CreateInstance(itemType);
        }

        /// <summary>
        /// Edits the specified item's value in the collection.
        /// </summary>
        /// <param name="context">Instance of ITypeDescriptorContext</param>
        /// <param name="provider">Instance of IServiceProvider</param>
        /// <param name="value">An instance</param>
        /// <returns>An instance that is returned from EditValue method</returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            m_parent = value as ScriptManagerExCollection;

            return base.EditValue(context, provider, value);
        }

        /// <summary>
        /// Overridden. Destroys the specified item from the collection.
        /// </summary>
        /// <param name="instance">A instance to be destroyed</param>
        protected override void DestroyInstance(object instance)
        {
            base.DestroyInstance(instance);

            ScriptManagerEx obj = instance as ScriptManagerEx;

            if (obj != null)
            {
                m_parent.Remove(obj);
            }
        }

        #endregion
    }
}
