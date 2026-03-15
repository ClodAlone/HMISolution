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

#region File Using
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Windows.Forms;
using System.Reflection;
using Microsoft.Win32;

using Syncfusion.Windows.Forms.Tools.XPMenus;
#endregion

namespace Syncfusion.Windows.Forms.Tools.Design
{
    /// <exclude/>
    public sealed class BarManagerConverter : Syncfusion.Windows.Forms.ComponentModel.CustomPropertiesTypeConverter
	{
        #region Class Constants
        /// <summary>
        /// Name to Office2007Theme property of the Barmanager.
        /// </summary>
        private const string c_sOffice2007ThemeName = "Office2007Theme";
        #endregion

        #region Class Overrides
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor)
				&& (value is BarManager))
			{
				BarManager barManager = (BarManager)value;
				System.Type[] args;
				args = new System.Type[1];

				args[0] = typeof(Form);

				System.Reflection.ConstructorInfo constructorInfo;
				constructorInfo = value.GetType().GetConstructor(args);
				if (constructorInfo != null)
				{
					object[] argValues;
					argValues = (object[])new System.Object[1];

					argValues[0] = barManager.Form;

					// Verify if there is a corresponding constructor that can take a IContainer as arg.
					ConstructorInfo altConstInfo = value.GetType().GetConstructor(new Type[]{typeof(IContainer), typeof(Form)});
					if(altConstInfo == null)
					{
						// Then prevent insertion of the container param.
						FieldInfo fInfo = value.GetType().GetField("insertContainerWhileSerializing", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
						if(fInfo != null)
							fInfo.SetValue(value, false);
					}

					return (object)new InstanceDescriptor(constructorInfo,argValues);
				}
				else
				{
					constructorInfo = value.GetType().GetConstructor(new System.Type[]{});

					if(constructorInfo != null)
					{
						// Prevent inserting the container param.
						FieldInfo fInfo = value.GetType().GetField("insertContainerWhileSerializing", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
						if(fInfo != null)
							fInfo.SetValue(value, false);

						return (object)new InstanceDescriptor(constructorInfo, new object[]{});
					}
				}
				if(constructorInfo == null)
					throw new ApplicationException("Required constructor not found in the BarManager type. Make sure to provide atlest a default constructor for the BarManager.");
			}
			return base.ConvertTo(context, culture, value, destinationType);
        }
        public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType)
		{
			if(destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
				return true;

			return base.CanConvertTo(context, destinationType);
        }
        protected override Attribute[] GetPropertyAttributes( Component component, PropertyDescriptor property )
        {
            BarManager barManager = component as BarManager;

            if( barManager != null && property.Name == c_sOffice2007ThemeName )
            {
                switch( barManager.Style )
                {
                    case VisualStyle.Office2007Outlook :
                    case VisualStyle.Office2007 :
                        return new Attribute[] { BrowsableAttribute.Yes };

                    default:
                        return new Attribute[] { BrowsableAttribute.No };
                }
            }

            return EmptyAttributes;
        }
        #endregion
    }
    /// <exclude/>
	public sealed class BarItemsConverter : ArrayConverter
    {
        #region Class Overrides
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
		{
			if(sourceType == typeof(string))
				return true;

			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor)
				&& (value is BarItemsDesignTime))
			{
				BarItemsDesignTime barItems = (BarItemsDesignTime)value;
				System.Type[] args;
				args = new System.Type[1];

				args[0] = typeof(BarItem[]);

				System.Reflection.ConstructorInfo constructorInfo;
				constructorInfo = typeof(BarItemsDesignTime).GetConstructor(args);
				if (constructorInfo != null)
				{
					object[] argValues;
					argValues = (object[])new System.Object[1];
					BarItem[] barItemArray = new BarItem[barItems.Count];
					for(int i = 0; i < barItems.Count; i++)
					{
						barItemArray[i] = barItems[i];
					}
					argValues[0] = barItemArray;

					return (object)new InstanceDescriptor(constructorInfo,argValues);
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType)
		{
			if(destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
				return true;

			return base.CanConvertTo(context, destinationType);
        }
        #endregion
    }
    /// <exclude/>
	public sealed class BarTypeConverter : ExpandableObjectConverter
    {
        #region Class Overrides
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor)
				&& (value is Bar))
			{
				Bar bar = (Bar)value;
				if(bar.Manager != null)
					return this.ConvertInBarManagerContext(context, culture, value, destinationType);
				else
					return this.ConvertInXPToolBarContext(context, culture, value, destinationType);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType)
		{
			if(destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
				return true;

			return base.CanConvertTo(context, destinationType);
        }
        #endregion

        #region Class Utilyty Methods
        private object ConvertInXPToolBarContext(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            Bar bar = (Bar)value;
            System.Type[] args;
            args = new System.Type[5];

            args[0] = typeof(BarManager);
            args[1] = typeof(string);
            args[2] = typeof(BarStyle);
            args[3] = typeof(BarItemsDesignTime);
            args[4] = typeof(int[]);

            System.Reflection.ConstructorInfo constructorInfo;
            constructorInfo = typeof(Bar).GetConstructor(args);
            if (constructorInfo != null)
            {
                object[] argValues;
                argValues = (object[])new System.Object[5];
                argValues[0] = bar.Manager;
                argValues[1] = bar.BarName;
                argValues[2] = bar.BarStyle;
                // Not persisting items here, instead persisting via the XPToolBar.Items property, to aid VI mode.
                //BarItemsDesignTime barItems = new BarItemsDesignTime();
                //				foreach(BarItem item in bar.Items)
                //					barItems.Add(item);
                argValues[3] = null;

                // This will be done via the XPToolBar.SeperatorIndices property.
                // Compose separator indices
                //				int[] separators = new int[bar.SeparatorCount];
                //				int sepIndex = 0;
                //				if(bar.SeparatorCount > 0)
                //				{
                //					for(int i = 0; i < bar.Items.Count; i++)
                //					{
                //						if(bar.IsGroupBeginning(bar.Items[i]))
                //						{
                //							separators[sepIndex] = i;
                //							sepIndex++;
                //						}
                //					}
                //				}
                //				argValues[4] = separators;

                return (object)new InstanceDescriptor(constructorInfo, argValues, false);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
        private object ConvertInBarManagerContext(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            Bar bar = (Bar)value;
            System.Type[] args;
            args = new System.Type[2];

            args[0] = typeof(BarManager);
            args[1] = typeof(string);
            System.Reflection.ConstructorInfo constructorInfo;
            constructorInfo = typeof(Bar).GetConstructor(args);
            if (constructorInfo != null)
            {
                object[] argValues;
                argValues = (object[])new System.Object[2];
                argValues[0] = bar.Manager;
                argValues[1] = bar.BarName;
                return (object)new InstanceDescriptor(constructorInfo, argValues);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
        #endregion
    }
}
