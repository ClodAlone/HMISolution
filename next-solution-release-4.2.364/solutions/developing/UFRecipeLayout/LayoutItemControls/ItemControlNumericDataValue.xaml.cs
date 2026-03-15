using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UFRecipeLayout.Automations;
using Utilities;
using Utilities.WPF;
using UFUAModel.Extensions;
using System.Collections;
using System.Windows.Threading;
using DevExpress.Xpf.Editors;

namespace UFRecipeLayout.LayoutItemControls
{
    /// <summary>
    /// Interaction logic for ItemControlNumericDataValue.xaml
    /// </summary>
    public partial class ItemControlNumericDataValue : UserControl, IDataValueUI, IPadSupport, IDataErrorInfo, IDisposable
    {
        #region Declarations
        bool bBindingValidated;
        bool bBindingInitialized;
        bool bLoaded;
        DataSet ds;
        String TableName;
        String ColumnName;
        IValueConverter valueConverter;
        UFUAModel.DataType valueType;

        DispatcherOperation dpUpdateValue;
        #endregion

        public ItemControlNumericDataValue()
        {
            InitializeComponent();

            MinValue = MaxValue = 0.0m;
            DecimalDigits = 2;
            MaxLength = -1;

            Loaded += (s, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;

                if (bBindingValidated)
                    ds.Tables[TableName].DefaultView.ListChanged += DefaultView_ListChanged;

                UpdateCultureInfo();
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
                bBindingInitialized = false;
                ds.Tables[TableName].DefaultView.ListChanged -= DefaultView_ListChanged;
                BindingOperations.ClearAllBindings(textDataValue);
            }
        }

        bool InvalidBinding()
        {
            return !bBindingValidated || ds.Tables[TableName].DefaultView.Count == 0;
        }

        protected String PerformValidation(String propertyName)
        {
            //if (InvalidBinding())
            //    return Properties.Resources.LayoutDataValueInvalidBinding;

            if (propertyName == "Value")
            {
                var type = Value.GetType();
                /*
                if (Value is String)
                {
                    if (MaxLength > 0 && (Value as String).Length > MaxLength)
                        return String.Format(Properties.Resources.LayoutDataValueInvalidStringLength, MaxLength);
                }
                */
                if (type.IsValueType)
                {
                    var dbvalue = Convert.ToDecimal(Value);
                    if (MinValue != MaxValue && (dbvalue < MinValue || dbvalue > MaxValue))
                        return String.Format(Properties.Resources.LayoutDataValueOutOfRange, MinValue, MaxValue);
                }
                else
                    return Properties.Resources.LayoutDataValueInvalidBinding;
            }

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
                var binding = textDataValue.GetBindingExpression(SpinEdit.EditValueProperty);
                if (binding != null)
                    binding.UpdateTarget();
            });
        }

        void UpdateCultureInfo()
        {
            var unitName = UnitName != null ? String.Format(" {0}", UnitName) : String.Empty;
            var displayFormatString = new StringBuilder("0");
            if (DecimalDigits > 0)
            {
                displayFormatString.Append(".");
                for (int ii = 0; ii < DecimalDigits; ii++)
                    displayFormatString.Append("0");
            }
            if (!String.IsNullOrWhiteSpace(UnitName))
                displayFormatString.AppendFormat(" {0}", UnitName);
            textDataValue.DisplayFormatString = displayFormatString.ToString();
        }

        #region Custom automation peers

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ValueAutomationPeer(this);
        }
        
        #endregion

        #region IDataValueUI Members

        public void SetBinding(string path, UFUAModel.DataType valueType, IValueConverter valueConverter = null)
        {
            textDataValue.IsFloatValue = valueType.IsDecimalType();
            this.valueType = valueType;
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
                bBindingInitialized = false;

                var binding = new Binding()
                {
                    Path = new PropertyPath("Value"),
                    Source = this,
                    Mode = BindingMode.TwoWay,
                    //Converter = valueConverter,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    ValidatesOnDataErrors = true,
                };

                BindingOperations.SetBinding(textDataValue, SpinEdit.EditValueProperty, binding);

                if (bLoaded)
                    ds.Tables[TableName].DefaultView.ListChanged += DefaultView_ListChanged;

                bBindingInitialized = true;
            }
            else
                bBindingValidated = false;
        }

        public void SetValue(string value)
        {
            var dvalue = Convert.ToDecimal(value);
            if (dvalue < MinValue)
                dvalue = MinValue;
            if (dvalue > MaxValue)
                dvalue = MaxValue;
            if (textDataValue.Value == dvalue)
                return;

            Value = dvalue;
            textDataValue.EditValue = dvalue;
        }

        public object Value 
        {
            get
            {
                if (InvalidBinding())
                    return (decimal)0;

                try
                {
                    if (valueConverter != null)
                    {
                        var newValue = valueConverter.Convert(ds.Tables[TableName].DefaultView[0][ColumnName], null, null, null);
                        if (valueType == UFUAModel.DataType.Boolean)
                            return Convert.ToBoolean(newValue) ? (decimal)1 : (decimal)0;
                        else 
                            return Convert.ToDecimal(newValue);
                    }
                    else
                        return Convert.ToDecimal(ds.Tables[TableName].DefaultView[0][ColumnName]);
                }
                catch
                {
                    return (decimal)0;
                }
            }
            set
            {
                if (InvalidBinding() || !bBindingInitialized)
                    return;

                try
                {
                    var newValue = value;
                    if (valueConverter != null)
                    {
                        if (valueType == UFUAModel.DataType.Boolean && value is Decimal)
                            newValue = (decimal)value != 0 ? true : false;
                        newValue = valueConverter.ConvertBack(newValue, null, ds.Tables[TableName].DefaultView[0][ColumnName], System.Globalization.CultureInfo.InvariantCulture);
                        if (newValue is ValidationResult)
                            return;
                    }

                    if (ds.Tables[TableName].DefaultView[0][ColumnName] != newValue)
                        ds.Tables[TableName].DefaultView[0][ColumnName] = newValue;
                }
                catch
                { }
            }
        }

        public void SetEnumOptions(IEnumerable value)
        {
        }

        decimal minValue;
        public decimal MinValue 
        {
            get
            {
                return minValue;
                //return textDataValue.Minimum;
            }
            set
            {
                if (minValue == value)
                    return;

                minValue = value;
                //if (textDataValue.Minimum == value)
                //    return;

                //textDataValue.Minimum = value;
            }
        }

        decimal maxValue;
        public decimal MaxValue
        {
            get
            {
                return maxValue;
                //return textDataValue.Maximum;
            }
            set
            {
                if (maxValue == value)
                    return;

                maxValue = value;
                //if (textDataValue.Maximum == value)
                //    return;

                //textDataValue.Maximum = value;
            }
        }

        int decimalDigits;
        public int DecimalDigits
        {
            get
            {
                return decimalDigits;
            }
            set
            {
                if (decimalDigits == value)
                    return;

                decimalDigits = value;
                UpdateCultureInfo();
            }
        }

        public int MaxLength { get; set; }

        string unitName; 
        public string UnitName 
        { 
            get
            {
                return unitName;
            }
            set
            {
                if (unitName == value)
                    return;

                unitName = value;
                UpdateCultureInfo();
            }
        }

        public bool AllowEdit
        {
            get
            {
                return textDataValue.IsEnabled;
            }
            set
            {
                if (textDataValue.IsEnabled == value)
                    return;
    
                textDataValue.IsEnabled = value;
            }
        }

        #endregion

        #region ISupportPad
        public void ShowPad()
        {
            var owner = this.FindParent<Window>();
            if (owner == null)
            {
                var ie = Keyboard.FocusedElement as DependencyObject;
                if (ie != null)
                    owner = Window.GetWindow(ie);
            }

            if (owner != null)
            {
                double? minValue = null;
                double? maxValue = null;
                if (MinValue != MaxValue)
                {
                    minValue = Convert.ToDouble(MinValue);
                    maxValue = Convert.ToDouble(MaxValue);
                }

                var ret = Pads.Pads.ShowNumericPad(textDataValue.Value.ToString(), owner, min: minValue, max: maxValue);
                if (ret != null)
                {
                    try
                    {
                        Value = Convert.ToDecimal(ret, System.Globalization.CultureInfo.CurrentCulture);
                        UpdateTargetValue();
                    }
                    catch
                    {
                    }
                }
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
