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
#endregion

namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    public class TreeNodeAdvSubItemConverter : ExpandableObjectConverter
    {
        #region Class Initialize/Finalize methods

        public TreeNodeAdvSubItemConverter()
            : base()
        {
        }
        #endregion

        #region Class overrides

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

            if (destinationType == typeof(InstanceDescriptor) &&
              value as TreeNodeAdvSubItem != null)
            {
                // get default constructor 
                MemberInfo memberInfo = typeof(TreeNodeAdvSubItem).GetConstructor(new Type[] { });

                // if we found constructor use it but mark that descriptior is not complete
                if (memberInfo != null)
                {
                    return new InstanceDescriptor(memberInfo, new object[] { }, false);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
        #endregion
    }

    public class TreeNodeAdvSubItemStyleInfoConverter : ExpandableObjectConverter
    {
        #region Class Initialize/Finalize methods

        public TreeNodeAdvSubItemStyleInfoConverter()
            : base()
        {
        }
        #endregion

        #region Class overrides

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

            if (destinationType == typeof(InstanceDescriptor) &&
              value as TreeNodeAdvSubItemStyleInfo != null)
            {
                // get default constructor 
                MemberInfo memberInfo = typeof(TreeNodeAdvSubItemStyleInfo).GetConstructor(new Type[] { });

                // if we found constructor use it but mark that descriptior is not complete
                if (memberInfo != null)
                {
                    return new InstanceDescriptor(memberInfo, new object[] { }, false);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
        #endregion
    }

    public class TreeNodeAdvSubItemStyleInfoStoreConverter : ExpandableObjectConverter
    {
        #region Class Initialize/Finalize methods

        public TreeNodeAdvSubItemStyleInfoStoreConverter()
            : base()
        {
        }
        #endregion

        #region Class overrides

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

            if (destinationType == typeof(InstanceDescriptor) &&
              value as TreeNodeAdvSubItemStyleInfoStore != null)
            {
                // get default constructor 
                MemberInfo memberInfo = typeof(TreeNodeAdvSubItemStyleInfoStore).GetConstructor(new Type[] { });

                // if we found constructor use it but mark that descriptior is not complete
                if (memberInfo != null)
                {
                    return new InstanceDescriptor(memberInfo, new object[] { }, false);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
        #endregion
    }
}