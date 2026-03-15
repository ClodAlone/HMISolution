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
    public class TestErrors
    {
        const string TAG1 = "2:Tags/2:VariableByte";

        [TestMethod]
        public void Test_DivideByZero()
        {
            using (var calcEngine = ExpressionValueConverter.CreateCalcEngine())
            {
                object result;

                string formula = String.Format("=[x] / [{0}]", TAG1);
                var expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, 0, TAG1, 0);
                Assert.IsTrue(result == null);

                Exception exception = null;
                try
                {
                    expression.ThrowExceptions = true;
                    result = Calculate(expression, 0, TAG1, 0);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
                finally
                {
                    expression.ThrowExceptions = false;
                }
                Assert.IsTrue(exception != null);
            }
        }

        [TestMethod]
        public void Test_InvalidFunctionNames()
        {
            using (var calcEngine = ExpressionValueConverter.CreateCalcEngine())
            {
                string formula = String.Format("=POWER([x], INVALID1([{0}], INVALID2([{0}])))", TAG1);
                var expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);
                var error = expression.GetParserError();
                Assert.IsTrue(!String.IsNullOrEmpty(error) && error.Contains("INVALID1") && error.Contains("INVALID2"));
            }
        }

        [TestMethod]
        public void Test_MissingParameter()
        {
            using (var calcEngine = ExpressionValueConverter.CreateCalcEngine())
            {
                object result;

                string formula = String.Format("=[x] * [{0}]", TAG1);
                var expression = new ExpressionValueConverter(formula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, 1);
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
