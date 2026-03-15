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

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;

namespace Syncfusion.Windows.Forms.Tools.Design
{
	class OfficeButtonTypeDescriptionProvider : TypeDescriptionProvider
	{
		#region Constructor
		public OfficeButtonTypeDescriptionProvider(Type type)
			: base(TypeDescriptor.GetProvider(type.BaseType))
		{
		}
		#endregion

		#region Overrides
		/// <summary>
		/// 
		/// </summary>
		/// <param name="objectType"></param>
		/// <param name="instance"></param>
		/// <returns></returns>
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			ICustomTypeDescriptor typeDesc = base.GetTypeDescriptor(objectType, instance);
			if (typeDesc != null)
			{
				typeDesc = new OfficeButtonTypeDescriptor(typeDesc);
			}
			return typeDesc;
		}
		#endregion

		#region *** OfficeButtonTypeDescriptor
		class OfficeButtonTypeDescriptor : ICustomTypeDescriptor
		{
			#region Constructor
			public OfficeButtonTypeDescriptor(ICustomTypeDescriptor baseDescriptor)
			{
				m_baseDescriptor = baseDescriptor;
			}
			#endregion

			#region ICustomTypeDescriptor Members
			AttributeCollection ICustomTypeDescriptor.GetAttributes()
			{
				AttributeCollection attributes = m_baseDescriptor.GetAttributes();
				
				Attribute[] buf = new Attribute[attributes.Count];
				attributes.CopyTo(buf, 0);

				for(int i=0; i<buf.Length; i++)
				{
					if (buf[i] is ToolStripItemDesignerAvailabilityAttribute)
					{
						buf[i] = new ToolStripItemDesignerAvailabilityAttribute(ToolStripItemDesignerAvailability.ToolStrip);
					}
				}

				return new AttributeCollection(buf);
			}
			string ICustomTypeDescriptor.GetClassName()
			{
				return m_baseDescriptor.GetClassName();
			}
			string ICustomTypeDescriptor.GetComponentName()
			{
				return m_baseDescriptor.GetComponentName();
			}
			TypeConverter ICustomTypeDescriptor.GetConverter()
			{
				return m_baseDescriptor.GetConverter();
			}
			EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
			{
				return m_baseDescriptor.GetDefaultEvent();
			}
			PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
			{
				return m_baseDescriptor.GetDefaultProperty();
			}
			object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
			{
				return m_baseDescriptor.GetEditor(editorBaseType);
			}
			EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
			{
				return m_baseDescriptor.GetEvents(attributes);
			}
			EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
			{
				return m_baseDescriptor.GetEvents();
			}
			PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
			{
				return m_baseDescriptor.GetProperties(attributes);
			}
			PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
			{
				return m_baseDescriptor.GetProperties();
			}
			object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
			{
				return m_baseDescriptor.GetPropertyOwner(pd);
			}
			#endregion

			#region Fields
			ICustomTypeDescriptor m_baseDescriptor;
			#endregion
		}
		#endregion
	}
}

#endif
