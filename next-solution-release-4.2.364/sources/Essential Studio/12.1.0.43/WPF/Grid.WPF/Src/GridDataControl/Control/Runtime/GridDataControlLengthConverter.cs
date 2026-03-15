#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.Windows.Controls.Scroll;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Syncfusion.Windows.Controls.Grid
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class GridDataControlLengthConverter : TypeConverter
    {
        private static string[] unitTypeStrings = { "auto", "sizetocells", "sizetoheader", "star", "none", "autowithlastcolumnfill" };
        private static GridDataControlLength[] unitTypeLengths = { GridDataControlLength.Auto, GridDataControlLength.SizeToCells, GridDataControlLength.SizeToHeader, GridDataControlLength.Star, GridDataControlLength.None, GridDataControlLength.AutoWithLastColumnFill };

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            TypeCode tc = Type.GetTypeCode(sourceType);
            switch (tc)
            {
                case TypeCode.String:
                case TypeCode.Decimal:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }

            string stringValue = value as string;
            if (stringValue != null)
            {
                stringValue = stringValue.Trim();
                int index = 0;
                while (index < unitTypeStrings.Length && !stringValue.EndsWith(unitTypeStrings[index], StringComparison.OrdinalIgnoreCase))
                {
                    index++;
                }
                if (index < unitTypeStrings.Length)
                {
                    return unitTypeLengths[index];
                }
            }

            if (stringValue.EndsWith("*"))
            {
                if (stringValue.Length == 1)
                {
                    stringValue = "1.0*";
                }
                double starValue = Convert.ToDouble(stringValue.Substring(0, stringValue.Length - 1), culture ?? CultureInfo.CurrentCulture);
                if (double.IsNaN(starValue))
                {
                    return GridControlLengthUnitType.Star;
                }
                else
                {
                    return new GridDataControlLength(starValue, GridControlLengthUnitType.Star);
                }
            }

            if (value.ToString().Equals(GridControlLengthUnitType.AutoWithLastColumnFill.ToString()))
            {
                return new GridDataControlLength(150d, GridControlLengthUnitType.AutoWithLastColumnFill);
            }

            double doubleValue = Convert.ToDouble(value, culture ?? CultureInfo.CurrentCulture);
            if (double.IsNaN(doubleValue))
            {
                return GridControlLengthUnitType.None;
            }
            else
            {
                return new GridDataControlLength(doubleValue);
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if (destinationType != typeof(string))
            {
                throw new ArgumentException("Type Mismatch");
            }

            GridDataControlLength gridDataControlLength = value as GridDataControlLength;
            if (gridDataControlLength == null)
            {
                throw new ArgumentException("Value not set");
            }
            else
            {
                switch (gridDataControlLength.UnitType)
                {
                    case GridControlLengthUnitType.Auto:
                        return "Auto";

                    case GridControlLengthUnitType.SizeToHeader:
                        return "SizeToHeader";

                    case GridControlLengthUnitType.SizeToCells:
                        return "SizeToCells";

                    case GridControlLengthUnitType.None:
                        return "None";

                    case GridControlLengthUnitType.Star:
                        return "Star";

                    case GridControlLengthUnitType.AutoWithLastColumnFill:
                        return "AutoWithLastColumnFill";

                    default:
                        return (Convert.ToString(gridDataControlLength.Value, culture ?? CultureInfo.CurrentCulture));
                }
            }
        }


    }

    [TypeConverter(typeof(GridDataControlLengthConverter))]
    public class GridDataControlLength
    {
        private double unitValue;
        private GridControlLengthUnitType unitType;

        private static readonly GridDataControlLength auto = new GridDataControlLength(defaultValue, GridControlLengthUnitType.Auto);
        private static readonly GridDataControlLength none = new GridDataControlLength(150d, GridControlLengthUnitType.None);
        private static readonly GridDataControlLength sizeToCells = new GridDataControlLength(defaultValue, GridControlLengthUnitType.SizeToCells);
        private static readonly GridDataControlLength sizeToHeader = new GridDataControlLength(defaultValue, GridControlLengthUnitType.SizeToHeader);
        private static readonly GridDataControlLength star = new GridDataControlLength(defaultValue, GridControlLengthUnitType.Star);
        private static readonly GridDataControlLength autowithlastcolumnfill = new GridDataControlLength(150d, GridControlLengthUnitType.AutoWithLastColumnFill);

        private const double defaultValue = 1.0d;

        public GridDataControlLength()
            : this(0d, GridControlLengthUnitType.None)
        {
        }

        public GridDataControlLength(double value)
            : this(value, GridControlLengthUnitType.None)
        {
        }

        public GridDataControlLength(double value, GridControlLengthUnitType type)
        {
            if (type != GridControlLengthUnitType.Auto &&
                type != GridControlLengthUnitType.SizeToCells &&
                type != GridControlLengthUnitType.SizeToHeader &&
                type != GridControlLengthUnitType.Star &&
                type != GridControlLengthUnitType.AutoWithLastColumnFill &&
                type != GridControlLengthUnitType.None)
            {
                throw new ArgumentException("Invalid Type Specified");
            }

            unitValue = value;
            unitType = type;
        }

        public static GridDataControlLength Auto
        {
            get
            {
                return auto;
            }
        }

        public bool IsAuto
        {
            get
            {
                return unitType == GridControlLengthUnitType.Auto;
            }
        }

        public static GridDataControlLength None
        {
            get
            {
                return none;
            }
        }

        internal static GridDataControlLength AutoWithLastColumnFill
        {
            get
            {
                return autowithlastcolumnfill;
            }
        }


        public bool IsNone
        {
            get
            {
                return unitType == GridControlLengthUnitType.None;
            }
        }

        public bool IsStar
        {
            get
            {
                return unitType == GridControlLengthUnitType.Star;
            }
        }

        public bool IsSizeToCells
        {
            get
            {
                return unitType == GridControlLengthUnitType.SizeToCells;
            }
        }

        public bool IsSizeToHeader
        {
            get
            {
                return unitType == GridControlLengthUnitType.SizeToHeader;
            }
        }

        public static GridDataControlLength SizeToCells
        {
            get
            {
                return sizeToCells;
            }
        }

        public static GridDataControlLength SizeToHeader
        {
            get
            {
                return sizeToHeader;
            }
        }

        public static GridDataControlLength Star
        {
            get
            {
                return star;
            }
        }

        public GridControlLengthUnitType UnitType
        {
            get
            {
                return unitType;
            }
            set
            {
                if (this.unitType != value)
                {
                    this.unitType = value;
                }
            }
        }

        public double Value
        {
            get
            {
                return unitValue;
            }
            set
            {
                if (this.unitValue != value)
                {
                    this.unitValue = value;
                }
            }
        }

        public override string ToString()
        {
            var valueString = string.Empty;
            switch (this.unitType)
            {
                case GridControlLengthUnitType.Auto:
                    valueString = String.Format("Auto");
                    break;
                case GridControlLengthUnitType.Star:
                    valueString = String.Format("{0}*", this.Value.ToString());
                    break;
                case GridControlLengthUnitType.SizeToCells:
                    valueString = String.Format("SizeToCells");
                    break;
                case GridControlLengthUnitType.SizeToHeader:
                    valueString = String.Format("SizeToHeader");
                    break;
                case GridControlLengthUnitType.AutoWithLastColumnFill:
                    valueString = String.Format("AutoWithLastColumnFill");
                    break;
                case GridControlLengthUnitType.None:
                    valueString = String.Format("{0}", this.Value.ToString());
                    break;
            }

            return valueString;
        }
    }
}
