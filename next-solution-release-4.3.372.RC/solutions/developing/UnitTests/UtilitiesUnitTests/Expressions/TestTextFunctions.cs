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
    public class TestTextFunctions
    {
        const string TAG1 = "2:Tags/2:VariableString";
        const string TAG2 = "2:Tags/2:VariableBool";
        const string TAG3 = "2:Tags/2:VariableInteger";

        [TestMethod]
        public void Test_CONCAT()
        {
            using (var calcEngine = ExpressionValueConverter.CreateCalcEngine())
            {
                object result;

                string formula = String.Format("=CONCAT([x], [{0}])", TAG1);
                var expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, "A", TAG1, "B");
                Assert.IsTrue(result.ToString() == "AB");
                result = Calculate(expression, "0", TAG1, "B");
                Assert.IsTrue(result.ToString() == "0B");
                result = Calculate(expression, "A", TAG1, "0");
                Assert.IsTrue(result.ToString() == "A0");
                result = Calculate(expression, "0", TAG1, "1");
                Assert.IsTrue(result.ToString() == "01");

                formula = String.Format("=CONCAT([x], [{0}])", TAG2);
                expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, "A", TAG2, false);
                Assert.IsTrue(result.ToString() == "A0");
                result = Calculate(expression, "A", TAG2, true);
                Assert.IsTrue(result.ToString() == "A1");

                formula = String.Format("=CONCAT([x], [{0}])", TAG3);
                expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, 0, TAG3, 1);
                Assert.IsTrue(result.ToString() == "01");
                result = Calculate(expression, 1, TAG3, 0);
                Assert.IsTrue(result.ToString() == "10");
            }
        }

        [TestMethod]
        public void Test_REPLACE()
        {
            using (var calcEngine = ExpressionValueConverter.CreateCalcEngine())
            {
                object result;

                string formula = "=REPLACE([x],1,1,\"*\")";
                var expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, "ABC");
                Assert.IsTrue(result.ToString() == "*BC");
                result = Calculate(expression, 1234567890);
                Assert.IsTrue(result.ToString() == "*234567890");

                formula = String.Format("=REPLACE([x],1,1,[{0}])", TAG1);
                expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, "ABC", TAG1, "*");
                Assert.IsTrue(result.ToString() == "*BC");
                result = Calculate(expression, 1234567890, TAG1, "*");
                Assert.IsTrue(result.ToString() == "*234567890");
                result = Calculate(expression, 1234567890, TAG1, 0);
                Assert.IsTrue(result.ToString() == "0234567890");
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
