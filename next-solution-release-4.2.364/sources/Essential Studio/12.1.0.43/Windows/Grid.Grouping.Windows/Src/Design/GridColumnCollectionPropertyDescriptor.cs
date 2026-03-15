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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Syncfusion.Grouping;
using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;

using Syncfusion.Windows.Forms.Grid.Grouping;

namespace Syncfusion.Windows.Forms.Grid.Grouping.Design
{
	#region TypeConverter
	public class GridColumnCollectionPropertyDescriptorConverter : ExpandableObjectConverter
	{
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			System.ComponentModel.PropertyDescriptorCollection pds
				= TypeDescriptor.GetProperties(value, attributes);

			GridColumnCollectionPropertyDescriptor cc = value as GridColumnCollectionPropertyDescriptor;
			if (cc != null)
			{
				ArrayList pdc = new ArrayList();
				foreach (GridColumnDescriptor column in cc.TableDescriptor.Columns)
					pdc.Add(column.Name);
				return pds.Sort((string[]) pdc.ToArray(typeof(string)));
			}

			return pds;
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}


	}
	#endregion

	/// <summary>
	/// Summary description for GridColumnDescriptorPropertyDescriptorCollection.
	/// </summary>
	[TypeConverter(typeof(GridColumnCollectionPropertyDescriptorConverter))]
	public class GridColumnCollectionPropertyDescriptor : ICustomTypeDescriptor
	{
		private GridTableDescriptor tableDescriptor;
		
		/// <summary>
		/// Property TableDescriptor (GridTableDescriptor)
		/// </summary>
		[BrowsableAttribute(false)]
		[System.Xml.Serialization.XmlIgnore]
		[DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
		public GridTableDescriptor TableDescriptor
		{
			get
			{
				return this.tableDescriptor;
			}
		}


		public GridColumnCollectionPropertyDescriptor(GridTableDescriptor tableDescriptor)
		{
			this.tableDescriptor = tableDescriptor;
		}

		public override string ToString()
		{
			return "Count = " + tableDescriptor.Columns.Count;
		}



		#region ICustomTypeDescriptor
		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		string ICustomTypeDescriptor.GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		string ICustomTypeDescriptor.GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return ((ICustomTypeDescriptor) this).GetProperties(null);
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			ArrayList pds = new ArrayList(TypeDescriptor.GetProperties(this, attributes, true));
			Attribute[] att = new Attribute[] {
												  new BrowsableAttribute(true),
												  new System.Xml.Serialization.XmlIgnoreAttribute(),
												  new RefreshPropertiesAttribute(RefreshProperties.All),
												  new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden),
												  new CategoryAttribute("ColumnDescriptors")
											  };
			foreach (GridColumnDescriptor column in this.tableDescriptor.Columns)
				pds.Add(new GridColumnDescriptorPropertyDescriptor(column.Name, column, att));
			return new PropertyDescriptorCollection((PropertyDescriptor[]) pds.ToArray(typeof(PropertyDescriptor)));
		}


		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		#endregion


	}
}
