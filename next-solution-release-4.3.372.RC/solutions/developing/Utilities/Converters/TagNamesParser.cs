using System;
using System.Collections.Generic;
using DevExpress.Spreadsheet.Formulas;

namespace Utilities.Converters
{
    internal class TagNamesParser : ExpressionVisitor
    {
        #region Declarations
        readonly bool distinctVariables;

        readonly List<String> listTagNames = new List<String>();
        readonly List<String> listUnknowFunctions = new List<String>();
        #endregion

        #region Constructors
        public TagNamesParser(bool distinctVariables)
        {
            this.distinctVariables = distinctVariables;
        }
        #endregion

        #region Overrides
        //public override void Visit(DevExpress.Spreadsheet.Formulas.DefinedNameReferenceExpression expression)
        //{
        //    base.Visit(expression);

        //    if (String.IsNullOrEmpty(expression.DefinedName))
        //        return;

        //    var tagName = expression.DefinedName;
        //    if (distinctVariables)
        //        ExpressionValueConverter.GetArrayIndex(expression.DefinedName, out tagName);

        //    if (!listTagNames.Contains(tagName) &&
        //        !String.Equals(tagName, ExpressionValueConverter.xTag, StringComparison.OrdinalIgnoreCase))
        //    {
        //        listTagNames.Add(tagName);
        //    }
        //}

        public override void Visit(ConstantExpression expression)
        {
            base.Visit(expression);

            if (!expression.Value.IsText || expression.Value.IsEmpty ||
                !expression.Value.TextValue.StartsWith(ExpressionValueConverter.openTag) ||
                !expression.Value.TextValue.EndsWith(ExpressionValueConverter.closeTag))
                return;

            var tagName = expression.Value.TextValue.Substring(ExpressionValueConverter.openTag.Length, expression.Value.TextValue.Length - ExpressionValueConverter.openTag.Length - ExpressionValueConverter.closeTag.Length);
            if (distinctVariables)
                ExpressionValueConverter.GetArrayIndex(tagName, out tagName);

            if (!listTagNames.Contains(tagName) &&
                !String.Equals(tagName, ExpressionValueConverter.xTag, StringComparison.OrdinalIgnoreCase))
            {
                listTagNames.Add(tagName);
            }
        }

        public override void Visit(UnknownFunctionExpression expression)
        {
            base.Visit(expression);

            if (!listUnknowFunctions.Contains(expression.FunctionName))
                listUnknowFunctions.Add(expression.FunctionName);
        }
        #endregion

        #region Properties
        public List<String> TagNames
        {
            get
            {
                return listTagNames;
            }
        }

        public List<String> UnknowFunctions
        {
            get
            {
                return listUnknowFunctions;
            }
        }
        #endregion
    }
}
