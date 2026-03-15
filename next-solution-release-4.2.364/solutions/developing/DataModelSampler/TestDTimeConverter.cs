using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace DataModelSampler
{
    public class TestDTimeConverter : TypeConverter
    {
        // Methods
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (!(sourceType == typeof(string)))
            {
                return base.CanConvertFrom(context, sourceType);
            }
            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                string[] strArray = ((string)value).Split(new char[] { '/' });
                try
                {
                    int month = int.Parse(strArray[0]);
                    return new DateTime(int.Parse(strArray[2]), month, int.Parse(strArray[1]));
                }
                catch
                {
                    return null;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
