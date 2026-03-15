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
    public class TestOperators
    {
        const string TAG1 = "2:Tags/2:VariableByte";

        [TestMethod]
        public void Test_Plus()
        {
            using (var calcEngine = ExpressionValueConverter.CreateCalcEngine())
            {
                double dResult;
                object result;

                string formula = String.Format("=[x] + [{0}]", TAG1);
                var expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, 0, TAG1, 0);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, 0, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 1.0);
                result = Calculate(expression, 1, TAG1, 0);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 1.0);
                result = Calculate(expression, 1, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 2.0);

                // minus sign - unary
                formula = @"=+[x]";
                expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, 0);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 1.0);
                result = Calculate(expression, -1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == -1.0);
                result = Calculate(expression, false);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, true);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 1.0);
            }
        }

        [TestMethod]
        public void Test_Minus()
        {
            using (var calcEngine = ExpressionValueConverter.CreateCalcEngine())
            {
                double dResult;
                object result;

                // minus sign - binary
                string formula = String.Format("=[x] - [{0}]", TAG1);
                var expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, 0, TAG1, 0);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, 0, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == -1.0);
                result = Calculate(expression, 1, TAG1, 0);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 1.0);
                result = Calculate(expression, 1, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);

                // minus sign - unary
                formula = @"=-[x]";
                expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, 0);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == -1.0);
                result = Calculate(expression, -1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 1.0);
                result = Calculate(expression, false);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, true);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == -1.0);
            }
        }

        [TestMethod]
        public void Test_Asterisk()
        {
            using (var calcEngine = ExpressionValueConverter.CreateCalcEngine())
            {
                double dResult;
                object result;

                string formula = String.Format("=[x] * [{0}]", TAG1);
                var expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, 0, TAG1, 0);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, 0, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, 1, TAG1, 0);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, 1, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 1.0);
                result = Calculate(expression, 2, TAG1, 2);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 4.0);
            }
        }

        [TestMethod]
        public void Test_ForwardSlash()
        {
            using (var calcEngine = ExpressionValueConverter.CreateCalcEngine())
            {
                double dResult;
                object result;

                string formula = String.Format("=[x] / [{0}]", TAG1);
                var expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, 0, TAG1, 0);
                Assert.IsTrue(result == null);
                result = Calculate(expression, 0, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, 1, TAG1, 0);
                Assert.IsTrue(result == null);
                result = Calculate(expression, 1, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 1.0);
                result = Calculate(expression, 4, TAG1, 2);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 2.0);
            }
        }

        [TestMethod]
        public void Test_Caret()
        {
            using (var calcEngine = ExpressionValueConverter.CreateCalcEngine())
            {
                double dResult;
                object result;

                string formula = String.Format("=[x] ^ [{0}]", TAG1);
                var expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, 0, TAG1, 0);
                Assert.IsTrue(result == null);
                result = Calculate(expression, 0, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, 1, TAG1, 0);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 1.0);
                result = Calculate(expression, 1, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 1.0);
                result = Calculate(expression, 4, TAG1, 2);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 16.0);
            }
        }

        [TestMethod]
        public void Test_Equal()
        {
            using (var calcEngine = ExpressionValueConverter.CreateCalcEngine())
            {
                double dResult;
                object result;

                string formula = String.Format("=[x] = [{0}]", TAG1);
                var expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, 0, TAG1, 0);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 1.0);
                result = Calculate(expression, 0, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, 1, TAG1, 0);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, 1, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 1.0);
            }
        }

        [TestMethod]
        public void Test_NotEqual()
        {
            using (var calcEngine = ExpressionValueConverter.CreateCalcEngine())
            {
                double dResult;
                object result;

                string formula = String.Format("=[x] <> [{0}]", TAG1);
                var expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, 0, TAG1, 0);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, 0, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 1.0);
                result = Calculate(expression, 1, TAG1, 0);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 1.0);
                result = Calculate(expression, 1, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
            }
        }

        [TestMethod]
        public void Test_Greater()
        {
            using (var calcEngine = ExpressionValueConverter.CreateCalcEngine())
            {
                double dResult;
                object result;

                string formula = String.Format("=[x] > [{0}]", TAG1);
                var expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, 0, TAG1, 0);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, 0, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, 1, TAG1, 0);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 1.0);
                result = Calculate(expression, 1, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, -1, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, 1, TAG1, -1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 1.0);
            }
        }

        [TestMethod]
        public void Test_Less()
        {
            using (var calcEngine = ExpressionValueConverter.CreateCalcEngine())
            {
                double dResult;
                object result;

                string formula = String.Format("=[x] < [{0}]", TAG1);
                var expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, 0, TAG1, 0);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, 0, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 1.0);
                result = Calculate(expression, 1, TAG1, 0);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, 1, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, -1, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 1.0);
                result = Calculate(expression, 1, TAG1, -1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
            }
        }

        [TestMethod]
        public void Test_GreaterOrEqual()
        {
            using (var calcEngine = ExpressionValueConverter.CreateCalcEngine())
            {
                double dResult;
                object result;

                string formula = String.Format("=[x] >= [{0}]", TAG1);
                var expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, 0, TAG1, 0);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 1.0);
                result = Calculate(expression, 0, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, 1, TAG1, 0);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 1.0);
                result = Calculate(expression, 1, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 1.0);
                result = Calculate(expression, -1, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, 1, TAG1, -1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 1.0);
                result = Calculate(expression, -1, TAG1, -1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 1.0);
            }
        }

        [TestMethod]
        public void Test_LessOrEqual()
        {
            using (var calcEngine = ExpressionValueConverter.CreateCalcEngine())
            {
                double dResult;
                object result;

                string formula = String.Format("=[x] <= [{0}]", TAG1);
                var expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, 0, TAG1, 0);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 1.0);
                result = Calculate(expression, 0, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 1.0);
                result = Calculate(expression, 1, TAG1, 0);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, 1, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 1.0);
                result = Calculate(expression, -1, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 1.0);
                result = Calculate(expression, 1, TAG1, -1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, -1, TAG1, -1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 1.0);
            }
        }

        [TestMethod]
        public void Test_Concatenation()
        {
            using (var calcEngine = ExpressionValueConverter.CreateCalcEngine())
            {
                object result;

                string formula = String.Format("=[x] & [{0}]", TAG1);
                var expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, "Hello", TAG1, "World");
                Assert.IsTrue(result.ToString() == "HelloWorld");
                result = Calculate(expression, true, TAG1, false);
                Assert.IsTrue(result.ToString() == "10");
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
