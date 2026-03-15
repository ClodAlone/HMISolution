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

using System.Windows.Forms.Design;
using System;
using System.Reflection;
using System.ComponentModel;
using System.Collections;
namespace Syncfusion.Windows.Forms.Design
{
	public class DesignTimeUtils
	{
		/// <summary>
		/// This should be called during design time when a component has been moved, sized or re-parented,
		/// but the change was not the result of a property change.  All property
		/// changes are monitored by the selection UI service, so this is automatic most
		/// of the time.  There are times, however, when a component may be moved without
		/// property change notification occurring.  Scrolling an auto scroll Win32
		/// form is an example of this.
		/// This method simply re-queries all currently selected components for their
		/// bounds and updates the selection handles for the ones that have changed.
		/// </summary>
		[Documentation.DocumentationExclude()]
		public static void SyncSelection(object selectionUIService)
		{
			if(selectionUIService != null)
			{
				// Calling SyncComponent could be efficient, but didn't work!
				MethodInfo mi = selectionUIService.GetType().GetMethod("System.Windows.Forms.Design.ISelectionUIService.SyncSelection", 
					BindingFlags.NonPublic
					| BindingFlags.Instance);
				if(mi != null)
				{
					mi.Invoke(selectionUIService, new object[]{});
				}
			}
		}

		/// <summary>
		/// Initializes the PersistenceModeAttribute type with the specified constant, using reflection, if the
		/// System.Web.dll is loaded.
		/// </summary>
		/// <returns></returns>
		public static Attribute GetPersistenceModeAttribute(string persistenceModeEnumConstant)
		{
			Type perModeAttType = Type.GetType("System.Web.UI.PersistenceModeAttribute", false, false);
			Type perModeEnumType = Type.GetType("System.Web.UI.PersistenceMode", false, false);
			if(perModeEnumType != null)
			{
				object innPropObject = Enum.Parse(perModeEnumType, persistenceModeEnumConstant, false);
				if(perModeAttType != null && innPropObject != null)
				{
					ConstructorInfo ci = perModeAttType.GetConstructor(new Type[]{perModeEnumType});
					return ci.Invoke(new object[]{innPropObject}) as Attribute;
				}
			}
			return null;
		}
	}
 	[AttributeUsage (AttributeTargets.Class , AllowMultiple = true )]

	[Documentation.DocumentationExclude()]	// to exclude this from generated class ref.
	public class CollectionItemTypesAttribute : Attribute
	{
		private Type collectionType;
		
		public CollectionItemTypesAttribute(Type collectionType)
		{
			this.collectionType = collectionType;
		}
	
		public Type CollectionType
		{
			get{return this.collectionType;}
		}
	}


	/// <summary>
	/// A dummy PropertyDescriptor that could be used to add custom attributes dynamically.
	/// </summary>
	public class AttributesAddingPropertyDescriptor : PropertyDescriptor
	{
		PropertyDescriptor originalDescriptor;
		public AttributesAddingPropertyDescriptor(PropertyDescriptor originalDescriptor, Attribute[] atts)
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
			return this.originalDescriptor.GetValue(component);
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