#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;

using Syncfusion.ComponentModel;
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    /// <summary>
    /// A collection that stores <see cref="TreeNodePrimitive"/> objects.
    /// </summary>
    [
    Editor(typeof(PrimitivesCollectionEditor), typeof(UITypeEditor)),
    Serializable
    ]
    public class TreeNodePrimitivesCollection :
      CollectionBase,
      ICloneable
    {
        #region Class members

        private Hashtable m_htItems = new Hashtable();

        /// <summary>Reference on parent node.</summary>
        private TreeNodeAdv m_node;

        /// <summary> flag for validating mode </summary>
        private bool m_isDeletingMode = false;
        #endregion

        #region Class properties

        /// <summary>Gets parent tree node.</summary>
        protected TreeNodeAdv TreeNode
        {
            get
            {
                return m_node;
            }
        }

        public TreeNodePrimitive this[int index]
        {
            get
            {
                return (TreeNodePrimitive)this.List[index];
            }
            set
            {
                TreeNodePrimitive oldValue = this[index];

                if (value.PrimitiveType != oldValue.PrimitiveType)
                {
                    OnValidate(value);
                }

                this.List[index] = value;
            }
        }
        public TreeNodePrimitive this[PredefinedPrimitiveTypes type]
        {
            get
            {
                return (TreeNodePrimitive)m_htItems[type];
            }
            set
            {
                TreeNodePrimitive old = this[type];
                m_htItems[type] = value;
                this[IndexOf(old)] = value;
            }
        }
        #endregion

        #region Class Events
        /// <summary>
        /// Raise by <see cref="OnCollectionChanged"/> method.
        /// </summary>
        public event CollectionChangeEventHandler CollectionChanged;
        #endregion

        #region Initialize/Finalize Method

        public TreeNodePrimitivesCollection(TreeNodeAdv node)
        {
            m_node = node;
        }
        #endregion

        #region Class Public Methods
 
        public bool IsValidPrimitiveType(PredefinedPrimitiveTypes primitiveType)
        {
            return m_htItems[primitiveType] == null;
        }

        /// <summary>
        /// Adds pt to collction.
        /// </summary>
        /// <param name="primitive">TreeNode Primitive</param>
        /// <returns>Returns Integer</returns>
        public int Add(TreeNodePrimitive primitive)
        {
            return base.List.Add(primitive);
        }

        /// <summary>
        /// Adds primitives to collection.
        /// </summary>
        /// <param name="arrPrimitives">TreeNode Primitive collection</param>
        public void AddRange(TreeNodePrimitive[] arrPrimitives)
        {
            if (arrPrimitives != null && arrPrimitives.Length > 0)
            {
                for (int i = 0, len = arrPrimitives.Length; i < len; i++)
                {
                    TreeNodePrimitive primitive = arrPrimitives[i];
                    this.List.Add(primitive);
                }
            }
        }

        /// <summary>
        /// Removes pt from collection.
        /// </summary>
        /// <param name="primitive">TreeNode Primitive</param>
        public void Remove(TreeNodePrimitive primitive)
        {
            if (m_htItems[primitive.PrimitiveType] != null)
            {
                m_isDeletingMode = true;
                this.List.Remove(primitive);
                m_isDeletingMode = false;
            }
        }

        public void Insert(int index, TreeNodePrimitive primitive)
        {
            this.List.Insert(index, primitive);
        }

        public int IndexOf(TreeNodePrimitive primitive)
        {
            return this.List.IndexOf(primitive);
        }

        public bool Contains(TreeNodePrimitive primitive)
        {
            return this.List.Contains(primitive);
        }
        #endregion

        #region Class Overrides

        protected void RaiseCollectionChanged(CollectionChangeEventArgs args)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, args);
            }
        }

        protected virtual void OnCollectionChanged(CollectionChangeEventArgs args)
        {
            if (this.TreeNode != null)
            {
                this.TreeNode.OnPrimitivesCollectionChanged(args);
            }

            RaiseCollectionChanged(args);
        }

        protected override void OnClear()
        {
            base.OnClear();

            for (int i = 0, len = this.Count; i < len; i++)
            {
                this[i].PropertyChanging -= new SyncfusionPropertyChangedEventHandler(OnPrimitivePropertyChanging);
            }

            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, this.InnerList.ToArray()));
            m_htItems.Clear();
        }

        protected override void OnRemoveComplete(int index, object value)
        {
            TreeNodePrimitive primitive = (TreeNodePrimitive)value;
            primitive.PropertyChanging -= new SyncfusionPropertyChangedEventHandler(OnPrimitivePropertyChanging);
            m_htItems.Remove(primitive.PrimitiveType);
            CollectionChangeEventArgs args = new CollectionChangeEventArgs(CollectionChangeAction.Remove, primitive);

            OnCollectionChanged(args);

            base.OnRemoveComplete(index, value);
        }

        protected override void OnInsertComplete(int index, object value)
        {
            TreeNodePrimitive primitive = (TreeNodePrimitive)value;
            primitive.PropertyChanging += new SyncfusionPropertyChangedEventHandler(OnPrimitivePropertyChanging);
            m_htItems[primitive.PrimitiveType] = primitive;

            CollectionChangeEventArgs args = new CollectionChangeEventArgs(CollectionChangeAction.Add, primitive);
            OnCollectionChanged(args);

            base.OnInsertComplete(index, value);
        }

        protected override void OnSetComplete(int index, object oldValue, object newValue)
        {
            TreeNodePrimitive primitiveOld = (TreeNodePrimitive)oldValue;
            TreeNodePrimitive primitiveNew = (TreeNodePrimitive)newValue;

            primitiveOld.PropertyChanging -= new SyncfusionPropertyChangedEventHandler(OnPrimitivePropertyChanging);
            primitiveNew.PropertyChanging += new SyncfusionPropertyChangedEventHandler(OnPrimitivePropertyChanging);

            base.OnSetComplete(index, oldValue, newValue);
        }

        protected override void OnValidate(object value)
        {
            if (!m_isDeletingMode)
            {
                TreeNodePrimitive primitive = (TreeNodePrimitive)value;

                if (!IsValidPrimitiveType(primitive.PrimitiveType))
                {
                    throw new ArgumentException("Primitive of this type already has been added.");
                }
            }

            base.OnValidate(value);
        }
        #endregion

        #region Supprot ICloneable

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        public virtual TreeNodePrimitivesCollection Clone()
        {
            TreeNodePrimitivesCollection clone = new TreeNodePrimitivesCollection(this.TreeNode);

            foreach (TreeNodePrimitive primitive in this.InnerList)
            {
                TreeNodePrimitive cloned = new TreeNodePrimitive(primitive.Index, primitive.PrimitiveType);

                clone.Add(cloned);
            }

            return clone;
        }
        #endregion

        #region Utility methods

        protected internal void SetParent(TreeNodeAdv nodeAdv)
        {
            m_node = nodeAdv;
        }

        private void OnPrimitivePropertyChanging(object sender, SyncfusionPropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "PrimitiveType":
                    if (!IsValidPrimitiveType((PredefinedPrimitiveTypes)e.NewValue))
                    {
                        throw new ArgumentException("Primitive of this type already has been added.");
                    }

                    PredefinedPrimitiveTypes newValue = (PredefinedPrimitiveTypes)e.NewValue;
                    m_htItems.Remove(e.OldValue);
                    m_htItems[newValue] = sender;
                    break;
            }
        }
        #endregion
    }
}