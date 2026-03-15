using System;
using System.Collections.Generic;
#if !NET_STANDARD
using System.Windows.Data;
using System.Windows.Controls;
#endif
using DevExpress.Spreadsheet;

namespace Utilities.Converters
{
    public interface IExpressionValueConverter : IValueConverter, IDisposable
    {
        #region Properties

        bool AdjustOutOfRangeArrayIndex { get; set; }
        IWorkbook CalcEngine { get; set; }
        string Formula { get; set; }
        Dictionary<string, string> MapCurrentParameteItems { get; set; }
        string ReverseFormula { get; set; }
        bool ThrowExceptions { get; set; }
        ExpressionType Type { get; }
        event EventHandler ParserErrorEvent;

        #endregion

        #region Methods

        List<string> GetAllParsedVariables();
        string GetFormulaError(string formula);
        List<string> GetFormulaVariables(string formula);
        List<string> GetListVarInExpression(string expression);
        string GetParserError();
        void ParseFormula();

        #endregion
    }
}