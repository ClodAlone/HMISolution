using System;
using System.Windows;
using System.Windows.Data;

namespace ScreenSettings.Documents
{
    public class ExpressionValueEntry : IDisposable
    {
        #region Declarations
        readonly FrameworkElement fe;
        readonly DependencyProperty dp;

        BindingExpression expression;
        #endregion

        #region Constructors
        public ExpressionValueEntry(FrameworkElement fe, DependencyProperty dp)
        {
            this.fe = fe;
            this.dp = dp;
            this.expression = fe.GetBindingExpression(dp);
        }
        #endregion

        #region Methods
        public void UpdateTarget()
        {
            if (expression.Status == BindingStatus.Detached)
                expression = fe.GetBindingExpression(dp);
            expression.UpdateTarget();
        }
        #endregion

        #region Properties
        public BindingExpression Expression
        {
            get
            {
                return expression;
            }
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            if (expression != null && expression.ParentBinding.Converter is IDisposable)
            {
                var converter = expression.ParentBinding.Converter as IDisposable;
                converter.Dispose();
            }
        }
        #endregion
    }
}
