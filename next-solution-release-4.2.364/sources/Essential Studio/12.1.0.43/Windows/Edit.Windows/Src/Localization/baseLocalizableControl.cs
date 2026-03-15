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
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;
using System.Resources;

namespace Syncfusion.Windows.Forms.Localization
{
	/// <summary>
	/// Control, that supports localization of the properties.
	/// </summary>
	[ToolboxItem( false )]
	public class BaseLocalizableControl
		: UserControl
		, ICustomTypeDescriptor
		, ILocalizableTypeDescriptor
	{
		#region Class Initialization/Finalization
		/// <summary>
		/// Creates ant initializes new instance of the control.
		/// </summary>
		public BaseLocalizableControl()
		{
#if DEBUG
			Localizer.PullLocalizableTypeDescriptor( this );
			using( ResXResourceWriter writer = new ResXResourceWriter( this.GetType().FullName + "_resources.ResX" ) )
			{
				Localizer.WriteResourcesList( this, writer );
				writer.Generate();
			}
#endif
		}
		#endregion

		#region Interface Implementation: ICustomTypeDescriptor
		/// <summary>
		/// Gets TypeConverter.
		/// </summary>
		/// <returns>TypeConverter for this class.</returns>
		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return TypeDescriptor.GetConverter( this, true );
		}
		/// <summary>
		/// Gets event descriptor collection.
		/// </summary>
		/// <param name="attributes">An array of type Attribute that is used as a filter.</param>
		/// <returns>An EventDescriptorCollection that represents the filtered events for this component instance.</returns>
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents( Attribute[] attributes )
		{
			EventDescriptorCollection collection = TypeDescriptor.GetEvents( this, attributes, true );

			return collection;
		}
		/// <summary>
		/// Gets event descriptor collection.
		/// </summary>
		/// <returns>An EventDescriptorCollection that represents the events for this component instance.</returns>
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return ( ( ICustomTypeDescriptor )this ).GetEvents( null );
		}
		/// <summary>
		/// Gets component name.
		/// </summary>
		/// <returns>The name of the object, or a null reference (Nothing in Visual Basic) if the object does not have a name.</returns>
		string ICustomTypeDescriptor.GetComponentName()
		{
			return TypeDescriptor.GetComponentName( this, true );
		}
		/// <summary>
		/// Gets property owner.
		/// </summary>
		/// <param name="pd">A PropertyDescriptor that represents the property whose owner is to be found.</param>
		/// <returns>An Object that represents the owner of the specified property.</returns>
		object ICustomTypeDescriptor.GetPropertyOwner( PropertyDescriptor pd )
		{
			return this;
		}
		/// <summary>
		/// Gets attribute collection.
		/// </summary>
		/// <returns>An AttributeCollection containing the attributes for this object.</returns>
		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return TypeDescriptor.GetAttributes( this, true );
		}
		/// <summary>
		/// Gets property descriptor collection.
		/// </summary>
		/// <param name="attributes">An array of type Attribute that is used as a filter.</param>
		/// <returns>A PropertyDescriptorCollection that represents the filtered properties for this component instance.</returns>
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties( Attribute[] attributes )
		{
			PropertyDescriptorCollection collection =
				TypeDescriptor.GetProperties( this, attributes, true );

			return collection;
		}

		/// <summary>
		/// Gets property descriptor collection.
		/// </summary>
		/// <returns>A PropertyDescriptorCollection that represents the filtered properties for this component instance.</returns>
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return ( ( ICustomTypeDescriptor )this ).GetProperties( null );
		}
		/// <summary>
		/// Gets editor.
		/// </summary>
		/// <param name="editorBaseType">A Type that represents the editor for this object.</param>
		/// <returns>An Object of the specified type that is the editor for this object, or a null reference if the editor cannot be found.</returns>
		object ICustomTypeDescriptor.GetEditor( Type editorBaseType )
		{
			return TypeDescriptor.GetEditor( this, editorBaseType, true );
		}
		/// <summary>
		/// Gets gefault property.
		/// </summary>
		/// <returns>Default property.</returns>
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			PropertyDescriptor desc = TypeDescriptor.GetDefaultProperty( this, true );
			return desc;
		}
		/// <summary>
		/// Gets default event descriptor.
		/// </summary>
		/// <returns>Default event descriptor.</returns>
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			EventDescriptor desc = TypeDescriptor.GetDefaultEvent( this, true );
			return desc;
		}
		/// <summary>
		/// Gets class name.
		/// </summary>
		/// <returns>Class name.</returns>
		string ICustomTypeDescriptor.GetClassName()
		{
			return TypeDescriptor.GetClassName( this, true );
		}
		#endregion
	}
}