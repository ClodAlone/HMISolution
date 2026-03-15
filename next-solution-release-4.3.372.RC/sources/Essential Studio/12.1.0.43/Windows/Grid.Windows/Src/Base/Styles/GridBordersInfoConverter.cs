//-------------------------------------------------------------------------------------------------
// <copyright file="GridBordersInfoConverter.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace Syncfusion.Windows.Forms.Grid
{
    // TODO: Might be obsolete.
#if oba
    internal class GridBordersInfoConverter : ExpandableObjectConverter
    {
        public GridBordersInfoConverter()
        {}

        /// <override/>
        public override bool CanConvertTo(ITypeDescriptorContext context, 
            Type destinationType)
        {
            if(destinationType == typeof(InstanceDescriptor))
                return true;
            
            return base.CanConvertTo(context, destinationType);
        }

        /// <override/>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, 
            object value, Type destinationType)
        {
            if(destinationType == typeof(InstanceDescriptor) && value is GridBordersInfo)
            {
                GridBordersInfo border = value as GridBordersInfo;

                ConstructorInfo constructorInfo = typeof(GridBordersInfo).GetConstructor(
                    new Type[]{});

                return new InstanceDescriptor(constructorInfo, new object[]{}, false);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
#endif
}
