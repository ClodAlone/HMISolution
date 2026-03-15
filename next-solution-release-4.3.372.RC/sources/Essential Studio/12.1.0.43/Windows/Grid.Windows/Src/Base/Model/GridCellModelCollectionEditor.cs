//-------------------------------------------------------------------------------------------------
// <copyright file="GridCellModelCollectionEditor.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;

using Syncfusion.Styles;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Provides a collection editor that is tailored to adding, removing, and changing column styles from
    /// the <see cref="GridModel"/>.
    /// </summary>
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode=true)]
    internal class GridCellModelCollectionEditor :
        CollectionEditor
    {
        ////private object collection;
        private ArrayList list;
        private GridCellModelCollection cellModels;

        // Constructors

        /// <summary>
        /// Initializes a new <see cref="GridCellModelCollectionEditor"/> with a
        /// type to create instances for collection items.
        /// </summary>
        /// <param name="type">Type for collection item.</param>
        public GridCellModelCollectionEditor(Type type)
            : base(type)
        {
            this.cellModels = null;
        }

        /// <summary>
        /// Gets the data type that this collection contains.
        /// </summary>
        /// <returns>
        /// The data type of the items in the collection, or an <see cref="T:System.Object"/> if no Item property can be located on the collection.
        /// </returns>
        /// <override/>
        protected override Type CreateCollectionItemType()
        {
            return typeof(GridCellModelBase);
        } // end of method CreateCollectionItemType

        /// <summary>
        /// Indicates whether original members of the collection can be removed.
        /// </summary>
        /// <param name="value">The value to remove.</param>
        /// <returns>
        /// true if it is permissible to remove this value from the collection; otherwise, false. The default implementation always returns true.
        /// </returns>
        /// <override/>
        protected override bool CanRemoveInstance(object value)
        {
            return false;
        }

        /// <summary>
        /// Creates a new instance of the specified collection item type.
        /// </summary>
        /// <param name="itemType">The type of item to create.</param>
        /// <returns>A new instance of the specified object.</returns>
        /// <override/>
        protected override object CreateInstance(Type itemType)
        {
            return null;
        } //// end of method CreateInstance

        /// <summary>
        /// Edits the value of the specified object using the specified service provider and context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// <param name="provider">A service provider object through which editing services can be obtained.</param>
        /// <param name="value">The object to edit the value of.</param>
        /// <returns>
        /// The new value of the object. If the value of the object has not changed, this should return the same object it was passed.
        /// </returns>
        /// <override/>
        public override /*UITypeEditor*/ object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            this.cellModels = value as GridCellModelCollection;
            
            GridCellModelBase[] arr = new GridCellModelBase[this.cellModels.Count];
            this.cellModels.CopyTo(arr, 0);
            list = new ArrayList(arr);
            
            object obj = base.EditValue(context, provider, list);
////            
////            if (obj != null)
////            {
////                IList _list = obj as IList;
////                this.cellModels.Model.BeginUpdate(BeginUpdateOptions.Invalidate);
////                int i = 0;
////                foreach (GridCellModelBase sty in _list)
////                    this.cellModels[i++] = sty;
////                this.cellModels.Model.EndUpdate(false);
////            }

            return obj; ////new GridCellModel(this.cellModels.Model);
        } //// end of method EditValue        
    } //// end of class 
} 

