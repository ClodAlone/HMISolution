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
    public class TestLogicalFunctions
    {
        const string TAG1 = "2:Tags/2:VariableByte";

        [TestMethod]
        public void Test_NOT()
        {
            using (var calcEngine = ExpressionValueConverter.CreateCalcEngine())
            {
                double dResult;
                object result;

                string formula =  @"=NOT([x])";
                var expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, 0);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult > 0.0);
                result = Calculate(expression, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, false);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult > 0.0);
                result = Calculate(expression, true);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);

                formula = String.Format("=NOT([{0}])", TAG1);
                expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, 0, TAG1, 0);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult > 0.0);
                result = Calculate(expression, 0, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, 0, TAG1, false);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult > 0.0);
                result = Calculate(expression, 0, TAG1, true);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
            }
        }

        [TestMethod]
        public void Test_AND()
        {
            using (var calcEngine = ExpressionValueConverter.CreateCalcEngine())
            {
                double dResult;
                object result;

                string formula = String.Format("=AND([x], [{0}])", TAG1);
                var expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, 0, TAG1, 0);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, 0, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, 1, TAG1, 0);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, 1, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult > 0.0);
            }
        }

        [TestMethod]
        public void Test_NESTED()
        {
            using (var calcEngine = ExpressionValueConverter.CreateCalcEngine())
            {
                double dResult;
                object result;

                string formula = String.Format("=AND(NOT([x]), NOT([{0}]))", TAG1);
                var expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, 0, TAG1, 0);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult > 0.0);
                result = Calculate(expression, 0, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, 1, TAG1, 0);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
                result = Calculate(expression, 1, TAG1, 1);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 0.0);
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
