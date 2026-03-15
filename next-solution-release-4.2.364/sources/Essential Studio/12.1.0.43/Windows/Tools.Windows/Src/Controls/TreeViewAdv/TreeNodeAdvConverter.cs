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
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using System.Reflection;

namespace Syncfusion.Windows.Forms.Tools
{
	public class TreeNodePrimitiveConverter : ExpandableObjectConverter
	{
		public TreeNodePrimitiveConverter():base()
		{
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			bool result = ( destinationType == typeof( InstanceDescriptor ) ) ? true : 
				base.CanConvertTo(context, destinationType);
			
			return result;
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			TreeNodePrimitive primitive;
			MemberInfo memberInfo;
			object[] props;

			System.Type[] types;
			object[] objs;

			if (destinationType == null) throw new ArgumentNullException("destinationType");

			primitive = value as TreeNodePrimitive;

			if (destinationType == typeof(InstanceDescriptor) && primitive != null )
			{
				types = new Type[0];
				memberInfo = typeof(TreeNodePrimitive).GetConstructor(types);
				objs = new Object[0];

				props = objs;

				if (memberInfo != null)
					return new InstanceDescriptor(memberInfo, props,false);
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}
	}

	/// <summary>
	/// Summary description for TreeNodeAdvConverter.
	/// </summary>
	[Documentation.DocumentationExclude()]
	public class TreeNodeAdvConverter : ExpandableObjectConverter
	{
		public TreeNodeAdvConverter():base()
		{
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) 
		{
			if (destinationType == typeof(InstanceDescriptor))
				return true;
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) 
		{
			TreeNodeAdv node;
			MemberInfo memberInfo;
			object[] props;

			System.Type[] types;
			object[] objs;

			if (destinationType == null)
				throw new ArgumentNullException("destinationType");
			if (destinationType == typeof(InstanceDescriptor) && value as TreeNodeAdv != null) 
			{
				node = (TreeNodeAdv) value;
				memberInfo = null;
				props = null;

				types = new Type[0];
				memberInfo = typeof(TreeNodeAdv).GetConstructor(types);
				objs = new Object[0];

				props = objs;

				 if (memberInfo != null)
					return new InstanceDescriptor(memberInfo, props,false);
			}
		return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	[Documentation.DocumentationExclude()]
	public class StyleNamePairConverter : ExpandableObjectConverter
	{
		public StyleNamePairConverter():base()
		{
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) 
		{
			if (destinationType == typeof(InstanceDescriptor))
				return true;
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) 
		{
			StyleNamePair pair;
			MemberInfo memberInfo;
			object[] props;

			System.Type[] types;
			object[] objs;

			if (destinationType == null)
				throw new ArgumentNullException("destinationType");
			if (destinationType == typeof(InstanceDescriptor) && value as StyleNamePair != null) 
			{
				pair = (StyleNamePair) value;
				memberInfo = null;
				props = null;

				types = new Type[2];
				types[0] = typeof(string);
				types[1] = typeof(TreeNodeAdvStyleInfo);
				memberInfo = typeof(StyleNamePair).GetConstructor(types);
				objs = new Object[2];
				objs[0] = pair.name;
				objs[1] = pair.style;

				props = objs;

				if (memberInfo != null)
					return new InstanceDescriptor(memberInfo, props);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}

	[Documentation.DocumentationExclude()]
	public class TreeNodeAdvStyleInfoConverter : ExpandableObjectConverter
	{
		public TreeNodeAdvStyleInfoConverter():base()
		{
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) 
		{
			if (destinationType == typeof(InstanceDescriptor))
				return true;
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) 
		{
			TreeNodeAdvStyleInfo styleInfo;
			MemberInfo memberInfo;
			object[] props;

			System.Type[] types;
			object[] objs;

			if (destinationType == null)
				throw new ArgumentNullException("destinationType");
			if (destinationType == typeof(InstanceDescriptor) && value as TreeNodeAdvStyleInfo != null) 
			{
				styleInfo = (TreeNodeAdvStyleInfo) value;
				memberInfo = null;
				props = null;

				types = new Type[0];
				memberInfo = typeof(TreeNodeAdvStyleInfo).GetConstructor(types);
				objs = new Object[0];

				props = objs;

				if (memberInfo != null)
					return new InstanceDescriptor(memberInfo, props,false);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
