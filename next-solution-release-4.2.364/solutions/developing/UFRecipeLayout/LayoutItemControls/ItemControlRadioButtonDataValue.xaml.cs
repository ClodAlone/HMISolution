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
using Mindscape.WpfElements;
using UFRecipeLayout.Automations;
using Utilities.WPF;
using Utilities;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Threading;

namespace UFRecipeLayout.LayoutItemControls
{
    /// <summary>
    /// Interaction logic for ItemControlComboDataValue.xaml
    /// </summary>
    public partial class ItemControlRadioButtonDataValue : UserControl, IDataValueUI, IDataErrorInfo, IDisposable
    {
        #region Declarations
        bool bBindingValidated;
        bool bBindingInitialized;
        bool bBindingSetValueDisabled;
        bool bLoaded;
        DataSet ds;
        String TableName;
        String ColumnName;
        IValueConverter valueConverter;
        UFUAModel.DataType valueType;

        DispatcherOperation dpUpdateValue;
        #endregion

        public ItemControlRadioButtonDataValue()
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

                ((INotifyCollectionChanged)textDataValue.Items).CollectionChanged += textDataValue_CollectionChanged;

                InitializeSelectedItem();
            };

            Unloaded += (s, e) =>
            {
                if (!bLoaded)
                    return;
                bLoaded = false;

                if (bBindingValidated)
                    ds.Tables[TableName].DefaultView.ListChanged -= DefaultView_ListChanged;

                ((INotifyCollectionChanged)textDataValue.Items).CollectionChanged -= textDataValue_CollectionChanged;
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
                ds.Tables[TableName].DefaultView.ListChanged += DefaultView_ListChanged;
                BindingOperations.ClearAllBindings(textDataValue);
            }
        }

        bool InvalidBinding()
        {
            return !bBindingValidated || ds.Tables[TableName].DefaultView.Count == 0;
        }

        protected String PerformValidation(String propertyName)
        {
            if (InvalidBinding())
                return null;

            if (propertyName == "Value")
            {
                var type = Value.GetType();
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

        void Radio_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem sel = (e.Source as RadioButton).TemplatedParent as ListBoxItem;
            int newIndex = textDataValue.ItemContainerGenerator.IndexFromContainer(sel);
            textDataValue.SelectedIndex = newIndex;
        }

        void InitializeSelectedItem()
        {
            AbortUpdatePendingValue();

            dpUpdateValue = Dispatcher.BeginInvokeAsynchronouslyInRender(() =>
            {
                try
                {
                    bBindingSetValueDisabled = true;
                    textDataValue.SelectedItem = null;
                    var ivalue = Convert.ToInt32(Value);
                    if (ivalue < textDataValue.Items.Count && ivalue >= 0)
                        textDataValue.SelectedItem = textDataValue.Items[ivalue];
                }
                catch (Exception)
                {
                    if (textDataValue.Items.Count > 0)
                        textDataValue.SelectedIndex = 0;
                }
                finally
                {
                    bBindingSetValueDisabled = false;
                }
            });
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
            Dispatcher.BeginInvokeAsynchronouslyInInput(() =>
            {
                CheckRadioButtons(e.RemovedItems, false);
                CheckRadioButtons(e.AddedItems, true);
            });
        }

        private void CheckRadioButtons(System.Collections.IList radioButtons, bool isChecked)
        {
            foreach (object item in radioButtons)
            {
                ListBoxItem lbi = textDataValue.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;

                if (lbi != null)
                {
                    RadioButton radio = lbi.Template.FindName("radio", lbi) as RadioButton;
                    if (radio != null)
                        radio.IsChecked = isChecked;
                }
            }
        }

        void DefaultView_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.Reset)
            {
                InitializeSelectedItem();
            }
        }

        void textDataValue_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                InitializeSelectedItem();
            }
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
            ds = (DataContext as DataSet);
            this.valueType = valueType;
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
                    ValidatesOnDataErrors = true
                };

                BindingOperations.SetBinding(textDataValue, ListBox.SelectedIndexProperty, binding);

                int memValue = 0;

                try
                {
                    if (Value != null)
                        memValue = Convert.ToInt32(Value);
                    if (memValue < textDataValue.Items.Count && memValue >= 0)
                        textDataValue.SelectedItem = textDataValue.Items[memValue];
                }
                catch
                { }

                if (bLoaded)
                    ds.Tables[TableName].DefaultView.ListChanged += DefaultView_ListChanged;

                bBindingInitialized = true;
            }
            else
                bBindingValidated = false;
        }

        public void SetValue(string value)
        {
            try
            {
                var ivalue = Convert.ToInt32(value);
                if (textDataValue.SelectedIndex == ivalue)
                    return;

                Value = ivalue;
                bBindingSetValueDisabled = true;
                if (ivalue < textDataValue.Items.Count && ivalue >= 0)
                    textDataValue.SelectedItem = textDataValue.Items[ivalue];
            }
            catch
            { }
            finally
            {
                bBindingSetValueDisabled = false;
            }
        }

        public object Value 
        {
            get
            {
                if (InvalidBinding())
                    return (Int32)0;

                try
                {
                    if (valueConverter != null)
                    {
                        var newValue = valueConverter.Convert(ds.Tables[TableName].DefaultView[0][ColumnName], null, null, null);
                        if (valueType == UFUAModel.DataType.Boolean)
                            return Convert.ToBoolean(newValue) ? (decimal)1 : (decimal)0;
                        else
                            return Convert.ToInt32(newValue);
                    }
                    else
                        return Convert.ToInt32(ds.Tables[TableName].DefaultView[0][ColumnName]);
                }
                catch
                {
                    return (Int32)0;
                }
            }
            set
            {
                if (InvalidBinding() || !bBindingInitialized || bBindingSetValueDisabled)
                    return;

                try
                {
                    var ivalue = Convert.ToInt32(value);
                    if (ivalue < 0)
                        return;

                    var newValue = value;
                    if (valueConverter != null)
                    {
                        if (valueType == UFUAModel.DataType.Boolean)
                            newValue = ivalue != 0 ? true : false;
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
            textDataValue.ItemsSource = value;
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
