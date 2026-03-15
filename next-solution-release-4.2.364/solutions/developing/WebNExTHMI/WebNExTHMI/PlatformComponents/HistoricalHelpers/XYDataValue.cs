using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WPFPenHelpers
{
    public class XYDataValue
    {
        private MyDataValue _value1;
        private MyDataValue _value2;

        #region Properties
        public MyDataValue Value1
        {
            get { return _value1; }
            set
            {
                _value1 = value;
            }
        }
        public MyDataValue Value2
        {
            get { return _value2; }
            set
            {
                _value2 = value;
            }
        }
        #endregion
    }

    public class XYPoint
    {
        private object _value1;
        private object _value2;
        private DateTime _date;

        #region ctor
        public XYPoint(object value1, object value2, DateTime date)
        {
            _value1 = value1;
            _value2 = value2;
            _date = date;
        }
        #endregion

        #region Properties
        public object Value1
        {
            get { return _value1; }
            set
            {
                _value1 = value;
            }
        }
        public object Value2
        {
            get { return _value2; }
            set
            {
                _value2 = value;
            }
        }
        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
            }
        }
        #endregion
    }
    public enum PenTypeEnum : int
    {
        Undefined = 0,
        X = 1,
        Y = 2,
        XY = 3
    }
}
