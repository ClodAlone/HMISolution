using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DataValidation.Helpers
{
    public static  class ColumnsBindingHelper
    {
        public static readonly DependencyProperty ColumnBindingExpressionProperty = DependencyProperty.RegisterAttached("ColumnBindingExpression", typeof(string), typeof(ColumnsBindingHelper), new PropertyMetadata(OnColumnBindingExpression));
        public static void SetColumnBindingExpression(DependencyObject o,string value)
        {
            if (o is DevExpress.Xpf.Grid.GridColumn col)
                col.SetValue(ColumnBindingExpressionProperty, value);
        }

        public static string GetColumnBindingExpression(DependencyObject o)
        {

            return o.GetValue(ColumnBindingExpressionProperty) as string;
        }

        private static void OnColumnBindingExpression(DependencyObject o,DependencyPropertyChangedEventArgs e)
        {
            if(o is DevExpress.Xpf.Grid.GridColumn col)
                col.Binding = new System.Windows.Data.Binding(e.NewValue as string);
            
        }
    }
}
