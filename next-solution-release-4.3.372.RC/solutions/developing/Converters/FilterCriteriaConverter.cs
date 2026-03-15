using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using Utilities;
using System.Windows.Media;
using DevExpress.Data.Filtering;
using DevExpress.Mvvm.Native;
using System.Linq.Expressions;
using static System.Net.WebRequestMethods;

namespace Converters
{
    public class FilterCriteriaConverter : CustomValueConverter
    {
        bool bCamelCasePropertyName;
        public override object ConvertFromStorageType(object value, object sender)
        {
            throw new NotImplementedException();
        }

        public override object ConvertToStorageType(object value, object sender, object document, object property, object converterParameter)
        {
            var criteria = value as string;
            string jsFilter = "";
            if (String.IsNullOrEmpty(criteria))
                return jsFilter;

            bCamelCasePropertyName = (converterParameter as bool?) == true;
            OperandValue[] parameters;

            try
            {
                CriteriaOperator cr = CriteriaOperator.Parse(criteria, out parameters);
                TryParseOperator(ref jsFilter, cr);
            }
            catch
            {
                jsFilter = "";
            }
            return jsFilter;
        }
        public override Type StorageType
        {
            get
            {
                return typeof(Dictionary<string, object>);
            }
        }

        void AppendStr(ref string startStr, string app)
        {
            startStr = String.Format("{0}{1}", startStr, app);
        }

        string CamelCase(string str)
        {
            if (String.IsNullOrEmpty(str))
                return str;
            if (str.Length == 1)
                return str.ToLowerInvariant();

            return Char.ToLowerInvariant(str[0]) + str.Substring(1);
        }

        bool TryParseGroupOperator(ref string jsFilter, CriteriaOperator cr)
        {
            var ret = false;
            var gop = cr as GroupOperator;
            if (gop != null)
            {
                ret = true;
                AppendStr(ref jsFilter, "(");
                var groupOperand = "";
                switch (gop.OperatorType)
                {
                    case GroupOperatorType.And:
                        groupOperand = " && ";
                        break;
                    case GroupOperatorType.Or:
                        groupOperand = " || ";
                        break;
                }
                foreach (var op in gop.Operands)
                {
                    TryParseOperator(ref jsFilter, op);
                    if (gop.Operands.IndexOf(op) != gop.Operands.Count - 1)
                        AppendStr(ref jsFilter, groupOperand);
                }
                AppendStr(ref jsFilter, ")");
            }
            return ret;
        }

        bool TryParseFunctionOperator(ref string jsFilter, CriteriaOperator cr)
        {
            var ret = false;
            var fop = cr as FunctionOperator;
            if (fop != null)
            {
                ret = true;
                var propName = bCamelCasePropertyName ? CamelCase((fop.Operands[0] as OperandProperty).PropertyName) : (fop.Operands[0] as OperandProperty).PropertyName;
                var compareValue = (fop.Operands[1] as ConstantValue).Value.ToString();
                switch (fop.OperatorType)
                {
                    case FunctionOperatorType.Contains:
                        AppendStr(ref jsFilter, "itemData." + propName + ".indexOf(" + compareValue + ") !== -1");
                        break;
                    case FunctionOperatorType.StartsWith:
                        AppendStr(ref jsFilter, "itemData." + propName + ".indexOf(" + compareValue + ") === 0");
                        break;
                    case FunctionOperatorType.EndsWith:
                        AppendStr(ref jsFilter, "itemData." + propName + ".indexOf(" + compareValue + ") === itemData." + propName + ".length - " + compareValue.Length);
                        break;
                    case FunctionOperatorType.IsNull:
                        AppendStr(ref jsFilter, "itemData." + propName + " === null");
                        break;
                    case FunctionOperatorType.IsNullOrEmpty:
                        AppendStr(ref jsFilter, "(itemData." + propName + " === null || itemData." + propName + " === '')");
                        break;
                }
            }
            return ret;
        }

        bool TryParseBinaryOperator(ref string jsFilter, CriteriaOperator cr)
        {
            var ret = false;
            var opb = cr as BinaryOperator;
            if (opb != null)
            {
                ret = true;
                var binaryop = "";
                switch (opb.OperatorType)
                {
                    case BinaryOperatorType.Greater:
                        binaryop = " > ";
                        break;
                    case BinaryOperatorType.GreaterOrEqual:
                        binaryop = " >= ";
                        break;
                    case BinaryOperatorType.Less:
                        binaryop = " < ";
                        break;
                    case BinaryOperatorType.LessOrEqual:
                        binaryop = " <= ";
                        break;
                    case BinaryOperatorType.Equal:
                        binaryop = " == ";
                        break;
                    case BinaryOperatorType.NotEqual:
                        binaryop = " != ";
                        break;
                }
                var rightValue = (opb.RightOperand as ConstantValue).Value.ToString();
                if ((opb.RightOperand as ConstantValue).Value is DateTime)
                {
                    var date = (DateTime)(opb.RightOperand as ConstantValue).Value;
                    //long unixTimestamp = (date.Ticks - new DateTime(1970, 1, 1).Ticks) / TimeSpan.TicksPerSecond;
                    //rightValue = (unixTimestamp * 1000).ToString();
                    rightValue = date.Ticks.ToString();
                }
                AppendStr(ref jsFilter, "itemData." + (bCamelCasePropertyName ? CamelCase((opb.LeftOperand as OperandProperty).PropertyName) : (opb.LeftOperand as OperandProperty).PropertyName) + binaryop + rightValue);
            }
            return ret;
        }

        bool TryParseUnaryOperator(ref string jsFilter, CriteriaOperator cr)
        {
            var ret = false;
            var opb = cr as UnaryOperator;
            if (opb != null)
            {
                ret = true;
                var unaryop = "";
                switch (opb.OperatorType)
                {
                    case UnaryOperatorType.IsNull:
                        AppendStr(ref jsFilter, "itemData." + (bCamelCasePropertyName ? CamelCase((opb.Operand as OperandProperty).PropertyName) : (opb.Operand as OperandProperty).PropertyName) + " === null");
                        break;
                    case UnaryOperatorType.Not:
                        AppendStr(ref jsFilter, "!(");
                        TryParseOperator(ref jsFilter, opb.Operand);
                        AppendStr(ref jsFilter, ")");
                        break;
                }
            }
            return ret;
        }

        void TryParseOperator(ref string jsFilter, CriteriaOperator op)
        {
            if (!TryParseFunctionOperator(ref jsFilter, op))
                if (!TryParseBinaryOperator(ref jsFilter, op))
                    if (!TryParseUnaryOperator(ref jsFilter, op))
                        TryParseGroupOperator(ref jsFilter, op);
        }
    }
}
