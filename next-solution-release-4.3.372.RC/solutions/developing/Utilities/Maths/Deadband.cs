using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Maths
{
    public class Deadband
    {
        #region Declarations
        readonly double previousValue;
        readonly double actualValue;
        readonly double deadband;
        
        bool treatEqualLikeFalse;
        bool absoluteDeadband;
        #endregion

        #region Constructors
        public Deadband(double previousValue, double actualValue, double deadband)
        {
            this.previousValue = previousValue;
            this.actualValue = actualValue;
            this.deadband = deadband;
            this.treatEqualLikeFalse = false;
            this.absoluteDeadband = false;
        }
        #endregion

        #region Methods
        public bool IsExceeded()
        {
            return IsExceeded(100.0);
        }

        public bool IsExceeded(double range)
        {
            if (double.IsNaN(previousValue) && !double.IsNaN(actualValue))
                return true;
            else if (!double.IsNaN(previousValue) && double.IsNaN(actualValue))
                return true;
            else if (double.IsNaN(actualValue) || double.IsNaN(deadband))
                return false;

            double baseline = range / 100;

            //if (baseline > 0)
            {
                var decimalsDeadband = Utilities.Maths.MathDecimals.GetDecimalPlaces(deadband);
                var decimalsactualValue = Utilities.Maths.MathDecimals.GetDecimalPlaces(actualValue);
                var decimalspreviousValue = Utilities.Maths.MathDecimals.GetDecimalPlaces(previousValue);

                var decimals = Math.Max(decimalsDeadband, decimalsactualValue);
                decimals = Math.Max(decimals, decimalspreviousValue);
                var power = decimals > 0 ? Math.Pow(10, decimals) : 1.0;

                var actualValuePow = actualValue * power;
                var previousValuePow = previousValue * power;
                var deadbandPow = deadband * power;

                var value = (actualValuePow - previousValuePow) / baseline;
                if (double.IsNaN(value))
                    return !double.IsNaN(actualValue) && !double.IsInfinity(actualValue);

                if (absoluteDeadband)
                {
                    value = Math.Abs(value);
                    deadbandPow = Math.Abs(deadbandPow);
                }

                if (TreatEqualLikeFalse && value == deadbandPow)
                    return false;
                else if (deadbandPow > 0 && value < deadbandPow)
                    return false;
                else if (deadbandPow < 0 && value > deadbandPow)
                    return false;
            }

            return true;
        }
        #endregion

        #region Properties
        public bool TreatEqualLikeFalse
        {
            get
            {
                return treatEqualLikeFalse;
            }
            set
            {
                if (value == treatEqualLikeFalse)
                    return;

                treatEqualLikeFalse = value;
            }
        }

        public bool AbsoluteDeadband
        {
            get
            {
                return absoluteDeadband;
            }
            set
            {
                if (value == absoluteDeadband)
                    return;

                absoluteDeadband = value;
            }
        }
        #endregion
    }
}
