using System;
using System.Collections.Generic;
using System.Globalization;
using DevExpress.Charts.Native;
using DevExpress.Spreadsheet;
using DevExpress.Spreadsheet.Functions;
using DevExpress.XtraSpreadsheet.Utils;

namespace Utilities.Converters
{
    internal class WorkbookExtended : Workbook
    {
        internal static double MAX_SAFE_INTEGER = Math.Pow(2, 53) - 1; // 9007199254740991


        public WorkbookExtended()
        {
            AddCustomFunction(new BitAndQWordFunction());
            AddCustomFunction(new BitOrQWordFunction());
            AddCustomFunction(new BitXorQWordFunction());

            Options.Culture = System.Globalization.CultureInfo.InvariantCulture;
        }

        void AddCustomFunction(ICustomFunction function)
        {
            try
            {
                CustomFunctions.Add(function);
            }
            catch (Exception ex)
            {
                Logger.Logger.GetDestinationLog(Logger.LoggerDestination.Application).ErrorFormat(Properties.Resources.ErrorAddingCustomFunction, function.Name, ex.Message);
            }
        }

        internal static ParameterValue TryGetUInt64ParameterValue(ParameterValue parameterValue, out UInt64 value)
        {
            value = 0;
            if (!parameterValue.IsText && !parameterValue.IsNumeric)
                return ParameterValue.ErrorInvalidValueInFunction;
            else if (parameterValue.IsText)
            {
                if (!UInt64.TryParse(parameterValue.TextValue, out value))
                    return ParameterValue.ErrorInvalidValueInFunction;
            }
            else if (parameterValue.IsNumeric && (parameterValue.NumericValue < 0 || parameterValue.NumericValue > WorkbookExtended.MAX_SAFE_INTEGER))
                return ParameterValue.ErrorNumber;
            else
                value = Convert.ToUInt64(parameterValue.NumericValue);

            return ParameterValue.Empty;
        }
    }

    #region Custom Functions

    internal class BitAndQWordFunction : ICustomFunction
    {
        #region Declarations
        const string functionName = "BITAND.QWORD";
        readonly ParameterInfo[] functionParameters;
        #endregion

        #region Constructor
        public BitAndQWordFunction()
        {
            functionParameters = new ParameterInfo[] 
            { 
                new ParameterInfo(ParameterType.Value, ParameterAttributes.Required),
                new ParameterInfo(ParameterType.Value, ParameterAttributes.Required)
            };
        }
        #endregion

        #region ICustomFunction
        public string Name => functionName;

        public ParameterType ReturnType => ParameterType.Value;

        public ParameterInfo[] Parameters => functionParameters;

        public bool Volatile => false;

        public ParameterValue Evaluate(IList<ParameterValue> parameters, EvaluationContext context)
        {
            if (parameters.Count < 2)
                return ParameterValue.ErrorInvalidValueInFunction;

            UInt64 value1;
            var ret = WorkbookExtended.TryGetUInt64ParameterValue(parameters[0], out value1);
            if (ret.IsError)
                return ret;

            UInt64 value2;
            ret = WorkbookExtended.TryGetUInt64ParameterValue(parameters[1], out value2);
            if (ret.IsError)
                return ret;

            try
            {
                var result = value1 & value2;
                return result.ToString();
            }
            catch
            {
                return ParameterValue.ErrorNumber;
            }
        }

        public string GetName(CultureInfo culture)
        {
            return functionName;
        }
        #endregion
    }

    internal class BitOrQWordFunction : ICustomFunction
    {
        #region Declarations
        const string functionName = "BITOR.QWORD";
        readonly ParameterInfo[] functionParameters;
        #endregion

        #region Constructor
        public BitOrQWordFunction()
        {
            functionParameters = new ParameterInfo[]
            {
                new ParameterInfo(ParameterType.Value, ParameterAttributes.Required),
                new ParameterInfo(ParameterType.Value, ParameterAttributes.Required)
            };
        }
        #endregion

        #region ICustomFunction
        public string Name => functionName;

        public ParameterType ReturnType => ParameterType.Value;

        public ParameterInfo[] Parameters => functionParameters;

        public bool Volatile => false;

        public ParameterValue Evaluate(IList<ParameterValue> parameters, EvaluationContext context)
        {
            if (parameters.Count < 2)
                return ParameterValue.ErrorInvalidValueInFunction;

            UInt64 value1;
            var ret = WorkbookExtended.TryGetUInt64ParameterValue(parameters[0], out value1);
            if (ret.IsError)
                return ret;

            UInt64 value2;
            ret = WorkbookExtended.TryGetUInt64ParameterValue(parameters[1], out value2);
            if (ret.IsError)
                return ret;

            try
            {
                var result = value1 | value2;
                return result.ToString();
            }
            catch
            {
                return ParameterValue.ErrorNumber;
            }
        }

        public string GetName(CultureInfo culture)
        {
            return functionName;
        }
        #endregion
    }

    internal class BitXorQWordFunction : ICustomFunction
    {
        #region Declarations
        const string functionName = "BITXOR.QWORD";
        readonly ParameterInfo[] functionParameters;
        #endregion

        #region Constructor
        public BitXorQWordFunction()
        {
            functionParameters = new ParameterInfo[]
            {
                new ParameterInfo(ParameterType.Value, ParameterAttributes.Required),
                new ParameterInfo(ParameterType.Value, ParameterAttributes.Required)
            };
        }
        #endregion

        #region ICustomFunction
        public string Name => functionName;

        public ParameterType ReturnType => ParameterType.Value;

        public ParameterInfo[] Parameters => functionParameters;

        public bool Volatile => false;

        public ParameterValue Evaluate(IList<ParameterValue> parameters, EvaluationContext context)
        {
            if (parameters.Count < 2)
                return ParameterValue.ErrorInvalidValueInFunction;

            UInt64 value1;
            var ret = WorkbookExtended.TryGetUInt64ParameterValue(parameters[0], out value1);
            if (ret.IsError)
                return ret;

            UInt64 value2;
            ret = WorkbookExtended.TryGetUInt64ParameterValue(parameters[1], out value2);
            if (ret.IsError)
                return ret;

            try
            {
                var result = value1 ^ value2;
                return result.ToString();
            }
            catch
            {
                return ParameterValue.ErrorNumber;
            }
        }

        public string GetName(CultureInfo culture)
        {
            return functionName;
        }
        #endregion
    }

    #endregion
}
