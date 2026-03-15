//-------------------------------------------------------------------------------------------------
// <copyright file="GridBaseStyleCollectionEditor.cs" company="syncfusion">
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
    /// Provides a collection editor that is tailored to adding, removing, and changing base styles from
    /// the <see cref="GridModel.BaseStylesMap"/> in a <see cref="GridModel"/>.
    /// </summary>
    /// <example>
    /// The following code shows the <see cref="GridBaseStyleCollectionEditor"/> in a dialog using the
    /// <see cref="GridBaseStylesMap"/> method of the <see cref="GridBaseStylesMap"/>
    /// class:
    /// <code lang="C#">
    ///        GridControlBase grid = ActiveGrid;
    ///        if (grid != null)
    ///        {
    ///            GridBaseStylesMap.ShowGridBaseStylesMapDialog(grid.Model, "BaseStylesMap");
    ///            grid.Model.Refresh();
    ///        }
    /// </code>
    /// </example>
    /// <seealso cref="GridBaseStylesMap"/>
    public class GridBaseStyleCollectionEditor :
        CollectionEditor
    {
        ////private object collection;
        private ArrayList list;
        GridBaseStylesMap gsim;
        private PropertyGridContextMenu pgMenu;

        /// <summary>
        /// Initializes a new <see cref="GridBaseStyleCollectionEditor"/> with a
        /// type to create instances for collection items.
        /// </summary>
        /// <param name="type">Type for collection item.</param>
        public GridBaseStyleCollectionEditor(Type type)
            : base(type)
        {
            gsim = null;
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
            return typeof(GridBaseStyle);
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
            GridBaseStyle gbs = value as GridBaseStyle;
            if (gbs == null)
            {
                return base.CanRemoveInstance(value);
            }

            return !(gbs.IsSystem == true);
        }

        /// <summary>
        /// Creates a new instance of the specified collection item type.
        /// </summary>
        /// <param name="itemType">The type of item to create.</param>
        /// <returns>A new instance of the specified object.</returns>
        /// <override/>
        protected override object CreateInstance(Type itemType)
        {
            ////GridBaseStylesMap gsim = collection as GridBaseStylesMap;
            if (gsim != null)
            {
                string name = gsim.GetNewBaseStyleName();
                GridBaseStyle gbs = new GridBaseStyle(name, false, null, gsim);
                return gbs;
            }

            return base.CreateInstance(itemType);
        } // end of method CreateInstance

        /// <override/>
        /// <summary>
        /// Edits the value of the specified object using the specified service provider and
        /// context.
        /// </summary>
        /// <param name="context">An <see
        /// cref="T:System.ComponentModel.ITypeDescriptorContext" /> that can be used to
        /// gain additional context information. </param>
        /// <param name="provider">A service provider object through which editing services
        /// can be obtained. </param>
        /// <param name="value">The object to edit the value of. </param>       
        /// <returns>
        /// The new value of the object. If the value of the object has not changed, this
        /// should return the same object it was passed.
        /// </returns>
        public override /*UITypeEditor*/ object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context == null || context.Instance == null)
            { 
                return IntEditValue(context, provider, value);
            }

            DialogResult result = GridBaseStylesMap.ShowGridBaseStylesMapDialog(context.Instance, "BaseStylesMap", provider);
            return value;
        } // end of method EditValue

        /// <summary>
        /// For internal use.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="value">The value.</param>
        /// <returns>returns object</returns>
        /// <internalonly/>
        public object IntEditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            gsim = value as GridBaseStylesMap;
            list = new ArrayList(gsim.baseStyles.Values);
            bool flag = typeof(System.Collections.IList).IsAssignableFrom(CollectionType);
            object obj = base.EditValue(context, provider, list);
            if (obj != null)
            {
                IList _list = obj as IList;
                gsim.BeginUpdate();
                gsim.baseStyles.Clear();
                foreach (GridBaseStyle sty in _list)
                {
                    gsim[sty.Name] = sty;
                }

                gsim.EndUpdate(false);
            }

            if (context != null && context.Instance is Control)
            {
                ((Control)context.Instance).Refresh();
            }

            return gsim;
        }

        /// <summary>
        /// Creates a new form to display and edit the current collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.ComponentModel.Design.CollectionEditor.CollectionForm"/> to provide as the user interface for editing the collection.
        /// </returns>
        /// <override/>
        protected override CollectionForm CreateCollectionForm()
        {
            CollectionForm collectionForm = base.CreateCollectionForm();

            PropertyGrid pg = WinFormsUtils.GetPropertyGridInControl(collectionForm);

            if (pg != null)
            {
                this.pgMenu = new PropertyGridContextMenu(pg);
            }

            return collectionForm;
        }
    } // end of class 
}

