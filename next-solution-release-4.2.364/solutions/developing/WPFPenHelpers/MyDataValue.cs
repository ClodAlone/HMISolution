using System;
using System.Collections.Generic;

namespace WPFPenHelpers
{
    public class MyDataValue
    {
        public DateTime SourceTimeStamp { get; set; }
        public double? dValue { get; set; }
        public object Value { get; set; }
        public bool bCompressed { get; set; }
        public bool bIsFakePoint { get; set; }

        public MyDataValue()
        {

        }

        public MyDataValue(object value)
        {
            Value = value;
        }

        public MyDataValue(DateTime argument, double? candidateValue)
        {
            SourceTimeStamp = argument;
            dValue = null;
            if (candidateValue.HasValue)
                dValue = candidateValue;
        }

        public static MyDataValue operator +(MyDataValue v1, MyDataValue v2)
        {
            return new MyDataValue()
            {
                dValue = v1.dValue + v2.dValue,
                SourceTimeStamp = v1.SourceTimeStamp
            };
        }

        public static MyDataValue operator /(MyDataValue v1, int div)
        {
            return new MyDataValue()
            {
                dValue = v1.dValue / div,
                SourceTimeStamp = v1.SourceTimeStamp
            };
        }
    }
    public class AggregatedValues
    {
        List<MyDataValue> _Values;
        List<MyDataValue> _MinValues;
        List<MyDataValue> _MaxValues;
        List<MyDataValue> _AvgValues;
        public int NumPoints;
        public int NumCompressPoint;
        public int NumCompressRation;
        public bool bBlockingException;
        public List<MyDataValue> Values
        {
            get
            {
                if (_Values == null)
                    _Values = new List<MyDataValue>();
                return _Values;
            }
            set
            {
                _Values = value;
            }
        }
        public List<MyDataValue> MinValues
        {
            get
            {
                if (_MinValues == null)
                    _MinValues = new List<MyDataValue>();
                return _MinValues;
            }
            set
            {
                _MinValues = value;
            }
        }
        public List<MyDataValue> MaxValues
        {
            get
            {
                if (_MaxValues == null)
                    _MaxValues = new List<MyDataValue>();
                return _MaxValues;
            }
            set
            {
                _MaxValues = value;
            }
        }
        public List<MyDataValue> AvgValues
        {
            get
            {
                if (_AvgValues == null)
                    _AvgValues = new List<MyDataValue>();
                return _AvgValues;
            }
            set
            {
                _AvgValues = value;
            }
        }
    }
}
