#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    internal class TreeNodeAdvSubItemCollectionEditor
      : CollectionEditor
    {
        #region Class members

        private TreeNodeAdvSubItemCollection m_parent;

        private Type[] m_types = new Type[] { typeof(TreeNodeAdvSubItem) };
        #endregion

        #region Class Initialize/Finalize methods

        public TreeNodeAdvSubItemCollectionEditor(Type type)
            : base(type)
        {
        }
        #endregion

        #region Class overrides

        protected override Type[] CreateNewItemTypes()
        {
            return m_types;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)                                    
        {
            m_parent = value as TreeNodeAdvSubItemCollection;

            return base.EditValue(context, provider, value);
        }

        protected override object[] GetItems(object editValue)
        {
            TreeNodeAdvSubItemCollection coll = editValue as TreeNodeAdvSubItemCollection;

            if (coll != null)
            {
                object firstItem = null;
                object[] values = new object[coll.Count];
                ((ICollection)coll).CopyTo(values, 0);

                if (values.Length > 0)
                {
                    firstItem = values[0];
                }

                ArrayList list = new ArrayList(values);

                if (null != firstItem)
                {
                    list.Remove(firstItem);
                }

                return (object[])list.ToArray(typeof(object));
            }

            return base.GetItems(editValue);
        }
        #endregion
    }
}