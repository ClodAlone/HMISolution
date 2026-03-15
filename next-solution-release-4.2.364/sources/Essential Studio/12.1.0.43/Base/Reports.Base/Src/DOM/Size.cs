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
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Globalization;
using System.ComponentModel;

namespace Syncfusion.RDL.DOM
{
    public class Size:IEquatable<Size>
    {
        public Size()
        {
        }

        public Size(string size)
        {
            this.size = size;
        }

        public Size(double value)
        {
            this.size = ((value / 96)).ToString(CultureInfo.InvariantCulture) + "in";
        }

        public static implicit operator Size(string value)
        {
            return new Size(value);
        }       

        private string _Size
        {
            get;
            set;
        }

        private string _CmValue
        {
            get;
            set;
        }


        [XmlIgnore]
        internal bool IsExpression
        {
            get;
            set;
        }      

        [XmlText()]
        public string size
        {
            get
            {
                return _Size;
            }
            set
            {
                Initialize(value);
                if (IsExpression)
                {
                    _Size = value;
                }
            }
        }

        [XmlIgnore]
        public string CmValue
        {
            get
            {
                Regex _regexChar = new Regex("[a-zA-Z]+");
                _CmValue = this.size.Trim().Replace("NaN", "0");
                Match m = _regexChar.Match(_CmValue);
                if (m.Success)
                {
                    if (m.Value == "in")
                    {
                        FloatValue = Convert.ToSingle(Math.Round(float.Parse(this.size.TrimEnd(m.Value.ToCharArray()), CultureInfo.InvariantCulture), 5) *2.54);
                        _CmValue = FloatValue + "cm";
                    }

                }

                return _CmValue;
            }
            set
            {
                Regex _regexChar = new Regex("[a-zA-Z]+");
                _CmValue = value.Trim().Replace("NaN", "0");
                Match m = _regexChar.Match(_CmValue);
                if (m.Success)
                {
                    if (m.Value == "in")
                    {
                        _CmValue = _CmValue.Replace(m.Value, "cm");
                    }

                }
            }
        }


        [XmlIgnore]
        public double PixelValue { get; set; }

        [XmlIgnore]
        [DefaultValue(0)]
        public float FloatValue { get; set; }

        [XmlIgnore]
        public MeasurementUnits MeasurementUnit { get; set; }

        void Initialize(string sizeStr)
        {
            if ( sizeStr != null && !sizeStr.Trim().StartsWith("="))
            {
                Regex _regexChar = new Regex("[a-zA-Z]+");
                sizeStr = sizeStr.Trim().Replace("NaN", "0");
                Match m = _regexChar.Match(sizeStr);
                if (m.Success)
                {
                    if (m.Value == "in" || m.Value == "In")
                    {
                        MeasurementUnit = MeasurementUnits.In;
                    }

                    else if (m.Value == "cm" || m.Value == "Cm")
                    {
                        MeasurementUnit = MeasurementUnits.Cm;
                    }

                    else
                    {
                        MeasurementUnit = (MeasurementUnits)Enum.Parse(typeof(MeasurementUnits), m.Value, true);
                    }

                    if (!string.IsNullOrEmpty(m.Value))
                    {
                        FloatValue = Convert.ToSingle(Math.Round(float.Parse(sizeStr.TrimEnd(m.Value.ToCharArray()), CultureInfo.InvariantCulture), 5));
                        _Size = FloatValue + m.Value;
                    }
                }
                else
                {
                    float value;

                    if (!string.IsNullOrEmpty(sizeStr) && float.TryParse(sizeStr, out value))
                    {
                        FloatValue = Convert.ToSingle(Math.Round(value, 5));
                        _Size = FloatValue + m.Value;
                    }
                }

                InitializePixelValue();
            }
            else if (sizeStr != null)
            {
                this.IsExpression = true;
            }
        }

        void InitializePixelValue()
        {
            if (MeasurementUnit == MeasurementUnits.In)
            {
                PixelValue = FloatValue * 96;
            }
            else if (MeasurementUnit == MeasurementUnits.Pt)
            {
                PixelValue = FloatValue * 1.333333333;
            }
            else if (MeasurementUnit == MeasurementUnits.Cm)
            {
                PixelValue = FloatValue * 37.795275591;
            }
            else if (MeasurementUnit == MeasurementUnits.Mm)
            {
                PixelValue = FloatValue * 3.7795275591;
            }
            else if (MeasurementUnit == MeasurementUnits.Pc)
            {
                PixelValue = FloatValue * 16;
            }
            else
            {
                PixelValue = FloatValue;
            }
        }

        public String Getvalue(string unittype)
        {
            if (unittype.ToLower() == "cm")
            {
                return CmValue;
            }

            Regex _regexChar = new Regex("[a-zA-Z]+");
            _CmValue = this.size.Trim().Replace("NaN", "0");
            Match m = _regexChar.Match(_CmValue);
            if (m.Success)
            {
                if (m.Value == "cm")
                {
                    FloatValue = Convert.ToSingle(Math.Round(float.Parse(this.size.TrimEnd(m.Value.ToCharArray()), CultureInfo.InvariantCulture), 5) / 2.54);
                   return FloatValue + "in";
                }

            }

            return size;
        }

        public bool Equals(Size other)
        {
            return this.PixelValue.Equals(other.PixelValue);
        }

        public object Clone()
        {
            Size size = new Size();
            size.FloatValue = this.FloatValue;
            size.PixelValue = this.PixelValue;
            size.MeasurementUnit = this.MeasurementUnit;
            size.size = this.size;
            size.IsExpression = this.IsExpression;
            return size;
        }
    }
}
