//-------------------------------------------------------------------------------------------------
// <copyright file="GridBoundColumnsCollectionEditor.cs" company="syncfusion">
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
using Syncfusion.ComponentModel;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Provides a collection editor that is tailored to adding and removing <see cref="GridBoundColumn"/> objects
    /// from the <see cref="GridDataBoundGrid.GridBoundColumns"/> collection in a <see cref="GridDataBoundGrid"/>.
    /// </summary>
    /// <seealso cref="GridBoundColumn"/>
    public class GridBoundColumnsCollectionEditor :
        CollectionEditor
    {
        private PropertyGridContextMenu pgMenu;
        // Constructors

        /// <summary>
        /// Initializes a new <see cref="GridBoundColumnsCollectionEditor"/> object.
        /// </summary>
        /// <param name="type">The type of the collection for this editor to edit.</param>
        public GridBoundColumnsCollectionEditor(Type type)
            : base(type)
        {
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

        object _EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            return base.EditValue(context, provider, value);
        }

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
            if (context == null || context.Instance == null || context.PropertyDescriptor == null
                || context.PropertyDescriptor.IsReadOnly)
            {
                return _EditValue(context, provider, value);
            }

            GridBoundColumnsCollectionEditor ce = new GridBoundColumnsCollectionEditor(this.CollectionType);
            WindowsFormsEditorServiceContainer esc = new WindowsFormsEditorServiceContainer(provider);
            PropertyDescriptor pd = context.PropertyDescriptor;
            object instance = context.Instance;
            TypeDescriptorContext tdc = new TypeDescriptorContext(instance, pd);
            tdc.ServiceProvider = esc;

            IDesignerHost designerHost = (IDesignerHost)provider.GetService(typeof(IDesignerHost));

            IComponentChangeService changeService = null;
            if (provider != null)
            {
                changeService = provider.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            }

            DesignerTransaction designerTransaction = null;
            if (designerHost != null)
            {
                designerTransaction = designerHost.CreateTransaction("Change " + pd.Name);
            }

            object oldValue = pd.GetValue(instance);
            object editValue = ((ICloneable)oldValue).Clone();
            object resultValue = ce._EditValue(tdc, esc, editValue);
            //// resultValue is normally the same as editValue
            //// Object.ReferenceEquals(resultValue, editValue) should be true

            if (esc.DialogResult == DialogResult.OK)
            {
                try
                {
                    ////changeService.OnComponentChanging(instance, pd);
                    pd.SetValue(instance, editValue);
                    ////changeService.OnComponentChanged(instance, pd, oldValue, editValue);
                }
                finally
                {
                    if (designerTransaction != null)
                    {
                        designerTransaction.Commit();
                    }
                }

                Control control = instance as Control;
                if (control != null)
                {
                    control.Invalidate();
                }
            }
            else
            {
                if (designerTransaction != null)
                {
                    designerTransaction.Cancel();
                }

                return oldValue;
            }

            return resultValue;
        }
    } // end of class 
}

