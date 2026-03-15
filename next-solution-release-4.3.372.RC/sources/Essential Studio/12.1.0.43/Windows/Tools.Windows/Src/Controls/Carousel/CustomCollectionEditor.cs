#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel.Design;

namespace Syncfusion.Windows.Forms.Tools
{
    /// <summary>
    /// CustomCollectionEditor for carousel control 
    /// </summary>
	public class CustomCollectionEditor: System.Drawing.Design.UITypeEditor 
	{
        /// <summary>
        /// delegate for collection changed event
        /// </summary>
		public delegate void CollectionChangedEventHandler(object sender, object instance, object value);

        /// <summary>
        /// Coolection changed event
        /// </summary>
		public event CollectionChangedEventHandler CollectionChanged;

		private ITypeDescriptorContext _context;
		
		private IWindowsFormsEditorService edSvc = null;

		public CustomCollectionEditor()
		{}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) 
		{
            ItemCollection coll = value as ItemCollection;
            Carousel owner = coll.container;
			if (context != null	&& context.Instance != null	&& provider != null) 
			{
				object originalValue=value;
				_context=context;
				edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

				if (edSvc != null) 
				{
					CustomCollectionEditorForm collEditorFrm = CreateForm(owner);
					collEditorFrm.ItemAdded+= new CustomCollectionEditorForm.InstanceEventHandler(ItemAdded);
					collEditorFrm.ItemRemoved+= new  CustomCollectionEditorForm.InstanceEventHandler(ItemRemoved);

                    collEditorFrm.Collection = (IList)value;
						
			
					context.OnComponentChanging();
					if(edSvc.ShowDialog(collEditorFrm)==DialogResult.OK)
					{
						OnCollectionChanged(context.Instance, value);
						context.OnComponentChanged();
					}					
				}
			}

			return value;
		}

		
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) 
		{
			if (context != null && context.Instance != null) 
			{
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}

		
		private void ItemAdded(object sender, object item)
		{
			
			if(_context!=null && _context.Container!=null)
			{
				IComponent icomp=item as IComponent;
				if(icomp !=null )
				{	
					_context.Container.Add(icomp);									
				}
			}

		}
	
		private void ItemRemoved(object sender, object item)
		{			
			if(_context!=null && _context.Container!=null)
			{
				IComponent icomp=item as IComponent;
				if(icomp!=null)
				{						
					_context.Container.Remove(icomp);
				}
			}

		}

	
		protected virtual void OnCollectionChanged(object instance ,object value)
		{
			if(CollectionChanged !=null)
			{
				CollectionChanged(this, instance,value);
			}
		}
		

		protected virtual CustomCollectionEditorForm CreateForm(Carousel owner)
		{
			return new CustomCollectionEditorForm(owner);
		}

		
	}
}
