using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Converters;

namespace WPFUtilities.HistoricalHelpers
{
    public class MyDataValue
    {
        #region Properties

        public DateTime SourceTimestamp { get; set; }
        public ushort SourcePicoseconds { get; set; }
        public uint StatusCode { get; set; }

        public double? dValueConverted
        {
            get
            {
                if (dValue != null
#if !NET_STANDARD
                    && expressionValueConverter != null
#endif
                    )
                {
                    return ConvertDValueWithUnitConverter(dValue.Value);
                }
                return dValue;
            }
        }
        public double? dValue { get; set; }
        public object Value { get; set; }
        public bool bCompressed { get; set; }
        public bool bIsFakePoint { get; set; }
#if !NET_STANDARD
        public IExpressionValueConverter expressionValueConverter { get; set; }
#endif

#endregion

        #region Constructor

        public MyDataValue()
        {

        }

        public MyDataValue(object value)
        {
            Value = value;
        }

        public MyDataValue(DateTime argument, double? candidateValue)
        {
            InitValues(argument, candidateValue);
        }

        public MyDataValue(DateTime argument, double? candidateValue, IExpressionValueConverter expressionConverter)
        {
            #if !NET_STANDARD
            if (expressionConverter != null)
            {
                expressionValueConverter = expressionConverter;
            }
#endif

            InitValues(argument, candidateValue);
        }

#endregion

        #region Public Method

        public Opc.Ua.DataValue ToDataValue()
        {
            object ret = Value;
            try
            {
                var valueToDouble = Convert.ToDouble(Value, System.Globalization.CultureInfo.InvariantCulture);
                ret = ConvertDValueWithUnitConverter(valueToDouble);
            }
            catch { }

            return new Opc.Ua.DataValue()
            {
                Value = ret,
                SourceTimestamp = SourceTimestamp,
                SourcePicoseconds = SourcePicoseconds,
                StatusCode = StatusCode
            };
        }

        public static MyDataValue operator +(MyDataValue v1, MyDataValue v2)
        {
            return new MyDataValue()
            {
                dValue = v1.dValue + v2.dValue,
                SourceTimestamp = v1.SourceTimestamp
            };
        }

        public static MyDataValue operator /(MyDataValue v1, int div)
        {
            return new MyDataValue()
            {
                dValue = v1.dValue / div,
                SourceTimestamp = v1.SourceTimestamp
            };
        }

        #endregion

        #region Private Method

        private void InitValues(DateTime argument, double? candidateValue)
        {
            SourceTimestamp = argument;
            dValue = null;
            if (candidateValue.HasValue)
                dValue = candidateValue;
        }

        private double ConvertDValueWithUnitConverter(double dValue)
        {
#if !NET_STANDARD
            if(expressionValueConverter == null)
                return dValue;

            var exprValue = expressionValueConverter.Convert(dValue, typeof(double), null, CultureInfo.InvariantCulture);

            if(exprValue == null)
                return dValue;

            return Convert.ToDouble(exprValue, System.Globalization.CultureInfo.InvariantCulture);
#else
            return dValue;
#endif
        }

#endregion
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
        public double? minValue;
        public double? maxValue;
        public double median;
        public double variance;
        public double standardDeviation;
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
