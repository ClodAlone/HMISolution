using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpo;
using System.Data;

namespace RecipeManagerControl
{
    /// <summary>
    /// Interaction logic for RecipeManager.xaml
    /// </summary>
    public partial class RecipeManager : UserControl
    {
        #region Dependency Properties
        public static readonly DependencyProperty DataSourceProperty = DependencyProperty.Register("DataSource", typeof(DataTable), typeof(RecipeManager), new UIPropertyMetadata(null, new PropertyChangedCallback(OnDataSourceChanged), new CoerceValueCallback(OnCoerceDataSource)));

        private static object OnCoerceDataSource(DependencyObject o, object value)
        {
            RecipeManager recipeManager = o as RecipeManager;
            if (recipeManager != null)
                return recipeManager.OnCoerceDataSource((DataTable)value);
            else
                return value;
        }

        private static void OnDataSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeManager recipeManager = o as RecipeManager;
            if (recipeManager != null)
                recipeManager.OnDataSourceChanged((DataTable)e.OldValue, (DataTable)e.NewValue);
        }

        protected virtual DataTable OnCoerceDataSource(DataTable value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnDataSourceChanged(DataTable oldValue, DataTable newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            gridControl.ItemsSource = newValue;
        }

        public DataTable DataSource
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (DataTable)GetValue(DataSourceProperty);
            }
            set
            {
                SetValue(DataSourceProperty, value);
            }
        }
        #endregion

        public RecipeManager()
        {
            InitializeComponent();
        }
    }
}
