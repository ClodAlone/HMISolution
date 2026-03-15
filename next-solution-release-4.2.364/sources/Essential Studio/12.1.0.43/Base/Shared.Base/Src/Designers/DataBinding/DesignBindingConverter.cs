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
using System.Design;
using System.Globalization;

namespace Syncfusion.Windows.Forms.Design
{
	class DesignBindingConverter : 
		TypeConverter
	{
        
		// Constructors
		public DesignBindingConverter()
		{
		}        
        
		// Methods
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type sourceType) 
		{
			DesignBinding db;
			string name;
			IComponent dataSource;

			db = (DesignBinding) value;
			if (db.IsNull)
				return "Null";
			name = "";
			if (db.DataSource as IComponent != null) 
			{
				dataSource = (IComponent) db.DataSource;
				if (dataSource.Site != null)
					name = dataSource.Site.Name;
			}
			if (name.Length == 0)
				name = "(List)";
			name = name + " - " + db.DataMember;
			return name;
		}

 		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) 
		{
			string strValue;
			int n;
			string name;
			string s;
			IComponent iComponent;

			strValue = (string) value;
			if (strValue == null || strValue.Length == 0 || String.Compare(strValue,"Null", true) == 0)
				return DesignBinding.Null;
			n = strValue.IndexOf("-");
			if (n == -1)
				throw new ArgumentException("Couldn't parse binding string " + value);
			name = strValue.Substring(0, n - 1).Trim();
			s = strValue.Substring(n + 1).Trim();
			if (context == null || context.Container == null)
				throw new ArgumentException("Context required to parse string " + strValue);
			iComponent = context.Container.Components[name];
			if (iComponent == null) 
			{
				if (String.Compare(name, "(List)", true) == 0)
					return null;
				throw new ArgumentException("Can't find component " + name);
			}
			return new DesignBinding(iComponent, s);
		}
        
		public override /*TypeConverter*/ bool CanConvertTo(ITypeDescriptorContext context, Type sourceType)
		{
			return (typeof(string) == sourceType);
		}
        
        
		public override /*TypeConverter*/ bool CanConvertFrom(ITypeDescriptorContext context, Type destType)
		{
			return (typeof(string) == destType);
		}
        
	}
}

