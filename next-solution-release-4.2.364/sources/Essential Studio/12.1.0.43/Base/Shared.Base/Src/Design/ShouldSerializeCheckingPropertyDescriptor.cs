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

namespace Syncfusion.Design
{
	[Documentation.DocumentationExclude()]
	public class ShouldSerializeCheckingPropertyDescriptor : PropertyDescriptor
	{
		PropertyDescriptor originalDescriptor;
		public ShouldSerializeCheckingPropertyDescriptor(PropertyDescriptor originalDescriptor, Attribute[] atts)
			: base(originalDescriptor.Name, atts)
		{
			this.originalDescriptor = originalDescriptor;
		}
	
		public override bool CanResetValue(object component)
		{
			return this.originalDescriptor.CanResetValue(component);
		}
	
		public override object GetValue(object component)
		{
			if(this.ShouldSerializeValue(component) == false)
				return null;
			else
			{
				object value = this.originalDescriptor.GetValue(component);
				return value;
			}
		}
	
		public override void ResetValue(object component)
		{
			this.originalDescriptor.ResetValue(component);
		}
	
		public override void SetValue(object component, object value)
		{
			this.originalDescriptor.SetValue(component, value);
		}
	
		public override bool ShouldSerializeValue(object component)
		{
			return this.originalDescriptor.ShouldSerializeValue(component);
		}
	
		public override Type ComponentType
		{
			get
			{
				return this.originalDescriptor.ComponentType;
			}
		}
	
		public override bool IsReadOnly
		{
			get
			{
				return this.originalDescriptor.IsReadOnly;
			}
		}
	
		public override Type PropertyType
		{
			get
			{
				return this.originalDescriptor.PropertyType;
			}
		}
	}
}