#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

using Syncfusion.Documentation;
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    [DocumentationExclude()]
    public class TreeNodePrimitiveConverter : ExpandableObjectConverter
    {
        public TreeNodePrimitiveConverter()
            : base()
        {
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            bool result = (destinationType == typeof(InstanceDescriptor)) ? true :
              base.CanConvertTo(context, destinationType);

            return result;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            TreeNodePrimitive primitive;
            MemberInfo memberInfo;
            object[] props;

            Type[] types;
            object[] objs;

            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }

            primitive = value as TreeNodePrimitive;

            if (destinationType == typeof(InstanceDescriptor) && primitive != null)
            {
                types = new Type[0];
                memberInfo = typeof(TreeNodePrimitive).GetConstructor(types);
                objs = new object[0];

                props = objs;

                if (memberInfo != null)
                {
                    return new InstanceDescriptor(memberInfo, props, false);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    [DocumentationExclude()]
    public class TreeNodeAdvConverter : ExpandableObjectConverter
    {
        public TreeNodeAdvConverter()
            : base()
        {
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            object[] props;
            Type[] types;

            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }

            if (destinationType == typeof(InstanceDescriptor) && value as TreeNodeAdv != null)
            {
                TreeNodeAdv node = (TreeNodeAdv)value;

                if (node.HasSubItems)
                {
                    types = new Type[] { typeof(TreeNodeAdvSubItem[]) };
                    props = new object[] { node.SubItems.ToArray() };
                }
                else
                {
                    types = new Type[0];
                    props = new object[0];
                }

                MemberInfo memberInfo = typeof(TreeNodeAdv).GetConstructor(types);

                if (memberInfo != null)
                {
                    return new InstanceDescriptor(memberInfo, props, false);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    [DocumentationExclude()]
    public class StyleNamePairConverter : ExpandableObjectConverter
    {
        public StyleNamePairConverter()
            : base()
        {
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }

            if (destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
            {
                if (null != value && value.GetType().FullName == "Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.StyleNamePair")
                {
                    Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.StyleNamePair pair = (Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.StyleNamePair)value;

                    Type[] types = new Type[2] { typeof(string), typeof(TreeNodeAdvStyleInfo) };
                    MemberInfo memberInfo = typeof(Syncfusion.Windows.Forms.Tools.MultiColumnTreeView.StyleNamePair).GetConstructor(types);
                    object[] props = new object[2] { pair.Name, pair.StyleGeneral };

                    if (memberInfo != null)
                    {
                        return new InstanceDescriptor(memberInfo, props);
                    }
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    [DocumentationExclude()]
    public class TreeNodeAdvStyleInfoConverter : ExpandableObjectConverter
    {
        public TreeNodeAdvStyleInfoConverter()
            : base()
        {
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            TreeNodeAdvStyleInfo styleInfo;
            MemberInfo memberInfo;
            object[] props;

            Type[] types;
            object[] objs;

            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if (destinationType == typeof(InstanceDescriptor) && value as TreeNodeAdvStyleInfo != null)
            {
                styleInfo = (TreeNodeAdvStyleInfo)value;
                memberInfo = null;
                props = null;

                types = new Type[0];
                memberInfo = typeof(TreeNodeAdvStyleInfo).GetConstructor(types);
                objs = new object[0];

                props = objs;

                if (memberInfo != null)
                {
                    return new InstanceDescriptor(memberInfo, props, false);
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}