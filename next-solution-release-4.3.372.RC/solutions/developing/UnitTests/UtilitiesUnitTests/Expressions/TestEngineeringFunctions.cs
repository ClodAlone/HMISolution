using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Converters;

namespace UtilitiesUnitTests.Expressions
{
    [TestClass]
    public class TestEngineeringFunctions
    {
        const string TAG1 = "2:Tags/2:VariableUINT64";

        [TestMethod]
        public void Test_BIT_StandardFunctions()
        {
            using (var calcEngine = ExpressionValueConverter.CreateCalcEngine())
            {
                double dResult;
                object result;
                double dvalue = Math.Pow(2, 48)/* - 1.0*/;

                string formula = String.Format("=BITAND([x], {0})", dvalue);
                string reverseFormula = null;
                var expression = new ExpressionValueConverter(formula, reverseFormula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, dvalue);
                Assert.IsTrue(result == null); // Overflow

                formula = String.Format("=BITOR([x], {0})", dvalue);
                expression = new ExpressionValueConverter(formula, reverseFormula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, dvalue);
                Assert.IsTrue(result == null); // Overflow

                formula = String.Format("=BITXOR([x], {0})", dvalue);
                expression = new ExpressionValueConverter(formula, reverseFormula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, dvalue);
                Assert.IsTrue(result == null); // Overflow

                dvalue = Math.Pow(2, 48) - 1.0;

                formula = String.Format("=BITAND([x], {0})", dvalue);
                expression = new ExpressionValueConverter(formula, reverseFormula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, dvalue);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == dvalue);

                formula = String.Format("=BITOR([x], {0})", dvalue);
                expression = new ExpressionValueConverter(formula, reverseFormula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, dvalue);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == dvalue);

                formula = String.Format("=BITXOR([x], {0})", dvalue);
                expression = new ExpressionValueConverter(formula, reverseFormula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, dvalue);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
            }
        }

        [TestMethod]
        public void Test_BITANDQWORD_CustomFunctions()
        {
            using (var calcEngine = ExpressionValueConverter.CreateCalcEngine())
            {
                ulong uResult;
                object result;
                
                string formula = String.Format("=BITAND.QWORD([x], [{0}])", TAG1);
                var expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);

                var uvalue = Convert.ToUInt64(Math.Pow(2, 53) - 1);
                result = Calculate(expression, uvalue, TAG1, uvalue);
                try { uResult = Convert.ToUInt64(result); } catch { uResult = 0; };
                Assert.IsTrue(uResult == uvalue);

                uvalue = Convert.ToUInt64(Math.Pow(2, 53) - 1);
                result = Calculate(expression, uvalue, TAG1, Int16.MaxValue);
                try { uResult = Convert.ToUInt64(result); } catch { uResult = 0; };
                Assert.IsTrue(uResult == (uvalue & (UInt64)Int16.MaxValue));

                result = Calculate(expression, uvalue, TAG1, 0);
                try { uResult = Convert.ToUInt64(result); } catch { uResult = 0; };
                Assert.IsTrue(uResult == 0);

                result = Calculate(expression, 0, TAG1, uvalue);
                try { uResult = Convert.ToUInt64(result); } catch { uResult = 0; };
                Assert.IsTrue(uResult == 0);

                uvalue = Convert.ToUInt64(Math.Pow(2, 53));
                result = Calculate(expression, uvalue, TAG1, 0);
                Assert.IsTrue(result == null);
                result = Calculate(expression, 0, TAG1, uvalue);
                Assert.IsTrue(result == null);

                result = Calculate(expression, -1, TAG1, 0);
                Assert.IsTrue(result == null);
                result = Calculate(expression, 0, TAG1, -1);
                Assert.IsTrue(result == null);
            }
        }

        [TestMethod]
        public void Test_BITANDQWORD_CustomFunctionsWithString()
        {
            using (var calcEngine = ExpressionValueConverter.CreateCalcEngine())
            {
                ulong uResult;
                object result;

                string formula = String.Format(@"=BITAND.QWORD('[x]', '[{0}]')", TAG1);
                var expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);

                var uvalue = UInt64.MaxValue;
                result = Calculate(expression, uvalue, TAG1, uvalue);
                try { uResult = Convert.ToUInt64(result); } catch { uResult = 0; };
                Assert.IsTrue(uResult == uvalue);

                uvalue = UInt64.MaxValue;
                result = Calculate(expression, uvalue, TAG1, Int16.MaxValue);
                try { uResult = Convert.ToUInt64(result); } catch { uResult = 0; };
                Assert.IsTrue(uResult == (uvalue & (UInt64)Int16.MaxValue));

                result = Calculate(expression, uvalue, TAG1, 0);
                try { uResult = Convert.ToUInt64(result); } catch { uResult = 0; };
                Assert.IsTrue(uResult == 0);

                result = Calculate(expression, 0, TAG1, uvalue);
                try { uResult = Convert.ToUInt64(result); } catch { uResult = 0; };
                Assert.IsTrue(uResult == 0);

                result = Calculate(expression, -1, TAG1, 0);
                Assert.IsTrue(result == null);
                result = Calculate(expression, 0, TAG1, -1);
                Assert.IsTrue(result == null);
            }
        }

        [TestMethod]
        public void Test_BITORQWORD_CustomFunctions()
        {
            using (var calcEngine = ExpressionValueConverter.CreateCalcEngine())
            {
                ulong uResult;
                object result;

                string formula = String.Format("=BITOR.QWORD([x], [{0}])", TAG1);
                var expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);

                var uvalue = Convert.ToUInt64(Math.Pow(2, 53) - 1);
                result = Calculate(expression, uvalue, TAG1, uvalue);
                try { uResult = Convert.ToUInt64(result); } catch { uResult = 0; };
                Assert.IsTrue(uResult == uvalue);

                uvalue = Convert.ToUInt64(Math.Pow(2, 53) - 1);
                result = Calculate(expression, uvalue, TAG1, Int16.MaxValue);
                try { uResult = Convert.ToUInt64(result); } catch { uResult = 0; };
                Assert.IsTrue(uResult == (uvalue | (UInt64)Int16.MaxValue));

                result = Calculate(expression, uvalue, TAG1, 0);
                try { uResult = Convert.ToUInt64(result); } catch { uResult = 0; };
                Assert.IsTrue(uResult == uvalue);

                result = Calculate(expression, 0, TAG1, uvalue);
                try { uResult = Convert.ToUInt64(result); } catch { uResult = 0; };
                Assert.IsTrue(uResult == uvalue);

                uvalue = Convert.ToUInt64(Math.Pow(2, 53));
                result = Calculate(expression, uvalue, TAG1, 0);
                Assert.IsTrue(result == null);
                result = Calculate(expression, 0, TAG1, uvalue);
                Assert.IsTrue(result == null);

                result = Calculate(expression, -1, TAG1, 0);
                Assert.IsTrue(result == null);
                result = Calculate(expression, 0, TAG1, -1);
                Assert.IsTrue(result == null);
            }
        }

        [TestMethod]
        public void Test_BITORQWORD_CustomFunctionsWithString()
        {
            using (var calcEngine = ExpressionValueConverter.CreateCalcEngine())
            {
                ulong uResult;
                object result;

                string formula = String.Format(@"=BITOR.QWORD('[x]', '[{0}]')", TAG1);
                var expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);

                var uvalue = UInt64.MaxValue;
                result = Calculate(expression, uvalue, TAG1, uvalue);
                try { uResult = Convert.ToUInt64(result); } catch { uResult = 0; };
                Assert.IsTrue(uResult == uvalue);

                uvalue = UInt64.MaxValue;
                result = Calculate(expression, uvalue, TAG1, Int16.MaxValue);
                try { uResult = Convert.ToUInt64(result); } catch { uResult = 0; };
                Assert.IsTrue(uResult == (uvalue | (UInt64)Int16.MaxValue));

                result = Calculate(expression, uvalue, TAG1, 0);
                try { uResult = Convert.ToUInt64(result); } catch { uResult = 0; };
                Assert.IsTrue(uResult == uvalue);

                result = Calculate(expression, 0, TAG1, uvalue);
                try { uResult = Convert.ToUInt64(result); } catch { uResult = 0; };
                Assert.IsTrue(uResult == uvalue);

                result = Calculate(expression, -1, TAG1, 0);
                Assert.IsTrue(result == null);
                result = Calculate(expression, 0, TAG1, -1);
                Assert.IsTrue(result == null);
            }
        }

        [TestMethod]
        public void Test_BITXORQWORD_CustomFunctions()
        {
            using (var calcEngine = ExpressionValueConverter.CreateCalcEngine())
            {
                ulong uResult;
                object result;

                string formula = String.Format("=BITXOR.QWORD([x], [{0}])", TAG1);
                var expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);

                var uvalue = Convert.ToUInt64(Math.Pow(2, 53) - 1);
                result = Calculate(expression, uvalue, TAG1, uvalue);
                try { uResult = Convert.ToUInt64(result); } catch { uResult = 0; };
                Assert.IsTrue(uResult == 0);

                uvalue = Convert.ToUInt64(Math.Pow(2, 53) - 1);
                result = Calculate(expression, uvalue, TAG1, Int16.MaxValue);
                try { uResult = Convert.ToUInt64(result); } catch { uResult = 0; };
                Assert.IsTrue(uResult == (uvalue ^ (UInt64)Int16.MaxValue));

                result = Calculate(expression, uvalue, TAG1, 0);
                try { uResult = Convert.ToUInt64(result); } catch { uResult = 0; };
                Assert.IsTrue(uResult == uvalue);

                result = Calculate(expression, 0, TAG1, uvalue);
                try { uResult = Convert.ToUInt64(result); } catch { uResult = 0; };
                Assert.IsTrue(uResult == uvalue);

                uvalue = Convert.ToUInt64(Math.Pow(2, 53));
                result = Calculate(expression, uvalue, TAG1, 0);
                Assert.IsTrue(result == null);
                result = Calculate(expression, 0, TAG1, uvalue);
                Assert.IsTrue(result == null);

                result = Calculate(expression, -1, TAG1, 0);
                Assert.IsTrue(result == null);
                result = Calculate(expression, 0, TAG1, -1);
                Assert.IsTrue(result == null);
            }
        }

        [TestMethod]
        public void Test_BITXORQWORD_CustomFunctionsWithString()
        {
            using (var calcEngine = ExpressionValueConverter.CreateCalcEngine())
            {
                ulong uResult;
                object result;

                string formula = String.Format(@"=BITXOR.QWORD('[x]', '[{0}]')", TAG1);
                var expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);

                var uvalue = UInt64.MaxValue;
                result = Calculate(expression, uvalue, TAG1, uvalue);
                try { uResult = Convert.ToUInt64(result); } catch { uResult = 0; };
                Assert.IsTrue(uResult == 0);

                uvalue = UInt64.MaxValue;
                result = Calculate(expression, uvalue, TAG1, Int16.MaxValue);
                try { uResult = Convert.ToUInt64(result); } catch { uResult = 0; };
                Assert.IsTrue(uResult == (uvalue ^ (UInt64)Int16.MaxValue));

                result = Calculate(expression, uvalue, TAG1, 0);
                try { uResult = Convert.ToUInt64(result); } catch { uResult = 0; };
                Assert.IsTrue(uResult == uvalue);

                result = Calculate(expression, 0, TAG1, uvalue);
                try { uResult = Convert.ToUInt64(result); } catch { uResult = 0; };
                Assert.IsTrue(uResult == uvalue);

                result = Calculate(expression, -1, TAG1, 0);
                Assert.IsTrue(result == null);
                result = Calculate(expression, 0, TAG1, -1);
                Assert.IsTrue(result == null);
            }
        }


        void Initialize(ExpressionValueConverter expression)
        {
            expression.ParseFormula();
            int arrayIndex;
            int bitNumber;
            var type = ExpressionValueConverter.GetFormulaType(expression.Formula, out arrayIndex, out bitNumber);
        }

        object Calculate(ExpressionValueConverter expression, object newValue)
        {
            return expression.Convert(newValue, null, null, System.Threading.Thread.CurrentThread.CurrentCulture);
        }

        object Calculate(ExpressionValueConverter expression, object newValue, string varName, object varValue)
        {
            var map = new Dictionary<string, object>();
            map.Add(varName, varValue);
            return expression.Convert(newValue, null, map, System.Threading.Thread.CurrentThread.CurrentCulture);
        }
    }
}
