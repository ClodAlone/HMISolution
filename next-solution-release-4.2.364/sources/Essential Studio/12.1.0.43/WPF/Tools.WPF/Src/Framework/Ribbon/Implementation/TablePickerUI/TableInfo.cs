#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Syncfusion.Windows.Tools.Controls
{
    [TypeConverter(typeof(TableInfoXamlConverter))]
    public class TableInfo 
    {
        public int Row { get; set; }

        public int Column { get; set; }

        public override string ToString()
        {
            return (Row + 1).ToString() + "X" + (Column +1).ToString() + " Table";
        }
    }

    public class TableInfoXamlConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value == null)
            {
                return new TableInfo();
            }

            if (value is string)
            {
                string returnvalue = value as string;
                if (returnvalue.Length >= 3)
                {
                    string[] args = returnvalue.Split(' ');
                    if (args.Length == 2)
                    {
                        return new TableInfo() { Row = Convert.ToInt16(args[0]), Column = Convert.ToInt16(args[1]) };
                    }
                    else
                    {
                        throw new ArgumentException("Attribute Value should contain row and column values.");
                    }
                }
                else
                {
                    throw new ArgumentException("Attribute Value should contain row and column values.");
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
