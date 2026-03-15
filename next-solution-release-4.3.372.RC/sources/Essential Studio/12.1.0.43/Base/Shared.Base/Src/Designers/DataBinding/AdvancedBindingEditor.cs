#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Syncfusion.Windows.Forms.Design
{
	class AdvancedBindingEditor : 
		UITypeEditor
	{
        
		// Fields
		private AdvancedBindingPicker advancedBindingPicker;
        
		// Methods
        
        
		/// <summary>
		///   <para>Returns the edit style from the current context.</para>
		/// </summary>
		/// <param name="context">The context of the object the bindings provide values to.</param>
		/// <returns>
		///   <para>A <see cref="T:System.Drawing.Design.UITypeEditorEditStyle" /> value.</para>
		/// </returns>
		public override /*UITypeEditor*/ UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		} // end of method GetEditStyle
        
        
        
		/// <summary>
		///   <para>Edits the specified value using the specified provider
		///  within the specified context.</para>
		/// </summary>
		/// <param name="context">The context of the value.</param>
		/// <param name=" provider">The provider to use to provide values.</param>
		/// <param name=" value"> The object to be edited.</param>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) 
		{
			IWindowsFormsEditorService editorService;
			IComponentChangeService componentChangeService;
			Control ctl;
			AdvancedBindingObject advancedBindingObject;

			if (provider != null) 
			{
				editorService = (IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));
				if (editorService != null) 
				{
					if (this.advancedBindingPicker == null)
						this.advancedBindingPicker = new AdvancedBindingPicker(context);
					componentChangeService = (IComponentChangeService) provider.GetService(typeof(IComponentChangeService));
					ctl = ((ControlBindingsCollection) context.Instance).Control;
					if (componentChangeService != null)
						componentChangeService.OnComponentChanging(ctl, TypeDescriptor.GetProperties(ctl)["DataBindings"]);
					advancedBindingObject = (AdvancedBindingObject) value;
					advancedBindingObject.Changed = false;
					this.advancedBindingPicker.Value = advancedBindingObject;
					editorService.ShowDialog(this.advancedBindingPicker);
					this.advancedBindingPicker.End();
					if (advancedBindingObject.Changed) 
					{
						TypeDescriptor.Refresh(((ControlBindingsCollection) context.Instance).Control);
						if (componentChangeService != null)
							componentChangeService.OnComponentChanged(ctl, TypeDescriptor.GetProperties(ctl)["DataBindings"], null, null);
					}
				}
			}
			return value;
		}

        
	} // end of class System.Windows.Forms.Design.AdvancedBindingEditor
} // end of namespace System.Windows.Forms.Design

