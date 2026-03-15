using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Utilities;

namespace UFRecipeLayout.LayoutItemControls
{
    /// <summary>
    /// Interaction logic for ItemControlBoolDataValue.xaml
    /// </summary>
    public partial class ItemControlBoolDataValue : UserControl, IDataValueUI, IDataErrorInfo, IDisposable
    {
        #region Declarations
        bool bBindingValidated;
        bool bLoaded;
        DataSet ds;
        String TableName;
        String ColumnName;
        IValueConverter valueConverter;

        DispatcherOperation dpUpdateValue;
        #endregion

        public ItemControlBoolDataValue()
        {
            InitializeComponent();

            MinValue = MaxValue = 0.0m;
            DecimalDigits = 0;
            MaxLength = -1;

            Loaded += (s, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;

                if (bBindingValidated)
                    ds.Tables[TableName].DefaultView.ListChanged += DefaultView_ListChanged;

                UpdateTargetValue();
            };

            Unloaded += (s, e) =>
            {
                if (!bLoaded)
                    return;
                bLoaded = false;

                if (bBindingValidated)
                    ds.Tables[TableName].DefaultView.ListChanged -= DefaultView_ListChanged;
            };
        }

        void AbortUpdatePendingValue()
        {
            if (dpUpdateValue != null &&
                dpUpdateValue.Status != DispatcherOperationStatus.Aborted &&
                dpUpdateValue.Status != DispatcherOperationStatus.Completed)
                dpUpdateValue.Abort();
        }

        public void ClearBinding()
        {
            if (bBindingValidated)
            {
                bBindingValidated = false;
                ds.Tables[TableName].DefaultView.ListChanged += DefaultView_ListChanged;
                BindingOperations.ClearAllBindings(checkValue);
            }
        }

        bool InvalidBinding()
        {
            return !bBindingValidated || ds.Tables[TableName].DefaultView.Count == 0;
        }

        protected String PerformValidation(String propertyName)
        {
            if (InvalidBinding())
                return Properties.Resources.LayoutDataValueInvalidBinding;

            return null;
        }

        void DefaultView_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.Reset)
                UpdateTargetValue();
        }

        void UpdateTargetValue()
        {
            AbortUpdatePendingValue();

            dpUpdateValue = Dispatcher.BeginInvokeAsynchronouslyInRender(() =>
            {
                var binding = checkValue.GetBindingExpression(CheckBox.IsCheckedProperty);
                if (binding != null)
                    binding.UpdateTarget();
            });
        }

        #region IDataValueUI Members

        public void SetBinding(string path, UFUAModel.DataType valueType, IValueConverter valueConverter = null)
        {
            ds = (DataContext as DataSet);
            this.valueConverter = valueConverter;
            if (path.Contains('/'))
            {
                TableName = path.Substring(0, path.IndexOf('/'));
                ColumnName = path.Substring(path.IndexOf('/') + 1);
            }

            if (ds != null &&
                !String.IsNullOrEmpty(TableName) &&
                !String.IsNullOrEmpty(ColumnName) &&
                ds.Tables.Contains(TableName) &&
                ds.Tables[TableName].Columns.Contains(ColumnName))
            {
                bBindingValidated = true;

                var binding = new Binding()
                {
                    Path = new PropertyPath("Value"),
                    Source = this,
                    Mode = BindingMode.TwoWay,
                    Converter = /*valueConverter ?? */new CheckBoxValueConverter(valueType),
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    ValidatesOnDataErrors = false // not validation required for check box
                };

                BindingOperations.SetBinding(checkValue, CheckBox.IsCheckedProperty, binding);

                if (bLoaded)
                    ds.Tables[TableName].DefaultView.ListChanged += DefaultView_ListChanged;
            }
            else
                bBindingValidated = false;
        }

        public void SetValue(string value)
        {
            var bvalue = Convert.ToBoolean(value);
            if (checkValue.IsChecked.HasValue && checkValue.IsChecked.Value == bvalue)
                return;

            Value = bvalue;
            checkValue.IsChecked = bvalue;
        }

        public object Value
        {
            get
            {
                if (InvalidBinding())
                    return false;

                if (valueConverter != null)
                    return valueConverter.Convert(ds.Tables[TableName].DefaultView[0][ColumnName], null, null, null);
                else
                    return ds.Tables[TableName].DefaultView[0][ColumnName];
            }
            set
            {
                if (InvalidBinding())
                    return;

                var newValue = value;
                if (valueConverter != null)
                {
                    newValue = valueConverter.ConvertBack(newValue, null, ds.Tables[TableName].DefaultView[0][ColumnName], System.Globalization.CultureInfo.InvariantCulture);
                    if (newValue is ValidationResult)
                        return;
                }

                if (ds.Tables[TableName].DefaultView[0][ColumnName] != newValue)
                    ds.Tables[TableName].DefaultView[0][ColumnName] = newValue;
            }
        }

        public void SetEnumOptions(IEnumerable value)
        {
        }

        public decimal MinValue { get; set; }

        public decimal MaxValue { get; set; }

        public int MaxLength { get; set; }

        public int DecimalDigits { get; set; }

        public string UnitName { get; set; }

        public bool AllowEdit
        {
            get
            {
                return checkValue.IsEnabled;
            }
            set
            {
                if (checkValue.IsEnabled == value)
                    return;

                checkValue.IsEnabled = value;
            }
        }

        #endregion

        #region IDataErrorInfo
        [Browsable(false)]
        public string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        public string this[string propertyName]
        {
            get
            {
                String s = PerformValidation(propertyName);
                if (!String.IsNullOrEmpty(s))
                    return s;
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var propertyInfo = GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(this, null);

                    return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                        ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                        : null;
                }

                return null;
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            AbortUpdatePendingValue();

            ClearBinding();
        }

        #endregion
    }
}
