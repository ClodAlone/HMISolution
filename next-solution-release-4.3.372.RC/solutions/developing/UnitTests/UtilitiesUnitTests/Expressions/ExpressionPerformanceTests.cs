using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Converters;

namespace UtilitiesUnitTests.Expressions
{
    [TestClass]
    public class ExpressionPerformanceTests
    {
        [TestMethod]
        public void TestManyExpressiosTags()
        {
            var listExpressions = new List<ExpressionValueConverter>();
            using (var calcEngine = ExpressionValueConverter.CreateCalcEngine())
            {
                string formula = "= [x] + 1";
                var text = String.Format("Create all expressions = '{0}'", formula) + " : {0}";
                using (var stopwatcher = new StopWatcher(text))
                {
                    for (int ii = 0; ii < 1000; ii++)
                        listExpressions.Add(new ExpressionValueConverter(formula) { CalcEngine = calcEngine});
                }

                text = "Initialize all expressions {0}";
                using (var stopwatcher = new StopWatcher(text))
                {
                    for (int ii = 0; ii < 1000; ii++)
                        Initialize(listExpressions[ii]);
                }

                text = "Calculate all expressions {0}";
                using (var stopwatcher = new StopWatcher(text))
                {
                    for (int ii = 0; ii < 1000; ii++)
                        Calculate(listExpressions[ii], 0);
                }
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

    }
}
