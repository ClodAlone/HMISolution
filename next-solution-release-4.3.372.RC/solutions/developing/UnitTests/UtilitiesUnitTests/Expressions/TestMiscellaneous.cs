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
    public class TestMiscellaneous
    {
        const string TAG1 = "2:Tags/2:VariableByte";

        [TestMethod]
        public void Test_MixedTags()
        {
            using (var calcEngine = ExpressionValueConverter.CreateCalcEngine())
            {
                double dResult;
                object result;

                string formula = String.Format("=[{0}] / 10", TAG1);
                string reverseFormula = "=[x] * 10";
                var expression = new ExpressionValueConverter(formula, reverseFormula) { CalcEngine = calcEngine };
                Initialize(expression);
                result = Calculate(expression, 0, TAG1, 20);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 2.0);
                result = CalculateBack(expression, 2, TAG1, 0);
                Assert.IsTrue(double.TryParse(result.ToString(), out dResult) && dResult == 20.0);
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

        object CalculateBack(ExpressionValueConverter expression, object newValue)
        {
            return expression.ConvertBack(newValue, null, null, System.Threading.Thread.CurrentThread.CurrentCulture);
        }

        object CalculateBack(ExpressionValueConverter expression, object newValue, string varName, object varValue)
        {
            var map = new Dictionary<string, object>();
            map.Add(varName, varValue);
            return expression.ConvertBack(newValue, null, map, System.Threading.Thread.CurrentThread.CurrentCulture);
        }
    }
}
