using DevExpress.Xpf.LayoutControl;
using System;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using UFInterfaces.PropertyControl;
using UFRecipeLayout.Helpers;
using UFRecipeSettings.UFRecipeModel;
using UFUAModel.Extensions;

namespace UFRecipeLayout.LayoutItemControls
{
    public class RecipeEditValueLayoutItem : RecipeLayoutItem, IDataErrorInfo
    {
        #region Dependency Properties

        #region ControlType
        public static readonly DependencyProperty ControlTypeProperty = DependencyProperty.Register("ControlType", typeof(EditValueControlTypeEnum), typeof(RecipeEditValueLayoutItem), new UIPropertyMetadata(EditValueControlTypeEnum.EditDisplay, new PropertyChangedCallback(OnControlTypeChanged), new CoerceValueCallback(OnCoerceControlType)));

        private static object OnCoerceControlType(DependencyObject o, object value)
        {
            RecipeEditValueLayoutItem recipeControl = o as RecipeEditValueLayoutItem;
            if (recipeControl != null)
                return recipeControl.OnCoerceControlType((EditValueControlTypeEnum)value);
            else
                return value;
        }

        private static void OnControlTypeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeEditValueLayoutItem recipeControl = o as RecipeEditValueLayoutItem;
            if (recipeControl != null)
                recipeControl.OnControlTypeChanged((EditValueControlTypeEnum)e.OldValue, (EditValueControlTypeEnum)e.NewValue);
        }

        protected virtual EditValueControlTypeEnum OnCoerceControlType(EditValueControlTypeEnum value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnControlTypeChanged(EditValueControlTypeEnum oldValue, EditValueControlTypeEnum newValue)
        {
            if (oldValue != newValue)
            {
                var dataValue = Tag as UFDataValueEntity;
                if (dataValue != null)
                {
                    Content = LayoutControlHelper.CreateContentItemControl(dataValue, newValue);
                    SetContentValues();
                }

                OnPropertyVisiblityChanged("ControlType");
            }
        }

        public EditValueControlTypeEnum ControlType
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (EditValueControlTypeEnum)GetValue(ControlTypeProperty);
            }
            set
            {
                SetValue(ControlTypeProperty, value);
            }
        }
        #endregion

        #region EnumOptions
        public static readonly DependencyProperty EnumOptionsProperty = DependencyProperty.Register("EnumOptions", typeof(String[]), typeof(RecipeEditValueLayoutItem), new UIPropertyMetadata(null, new PropertyChangedCallback(OnEnumOptionsChanged), new CoerceValueCallback(OnCoerceEnumOptions)));

        private static object OnCoerceEnumOptions(DependencyObject o, object value)
        {
            RecipeEditValueLayoutItem recipeControl = o as RecipeEditValueLayoutItem;
            if (recipeControl != null)
                return recipeControl.OnCoerceEnumOptions((String[])value);
            else
                return value;
        }

        private static void OnEnumOptionsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeEditValueLayoutItem recipeControl = o as RecipeEditValueLayoutItem;
            if (recipeControl != null)
                recipeControl.OnEnumOptionsChanged((String[])e.OldValue, (String[])e.NewValue);
        }

        protected virtual String[] OnCoerceEnumOptions(String[] value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnEnumOptionsChanged(String[] oldValue, String[] newValue)
        {
            if (oldValue != newValue)
            {
                if (Content is IDataValueUI)
                {
                    var control = Content as IDataValueUI;
                    control.SetEnumOptions(newValue);
                }
            }
        }

        public String[] EnumOptions
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String[])GetValue(EnumOptionsProperty);
            }
            set
            {
                SetValue(EnumOptionsProperty, value);
            }
        }
        #endregion

        #region UnitName
        public static readonly DependencyProperty UnitNameProperty = DependencyProperty.Register("UnitName", typeof(String), typeof(RecipeEditValueLayoutItem), new UIPropertyMetadata(null, new PropertyChangedCallback(OnUnitNameChanged), new CoerceValueCallback(OnCoerceUnitName)));

        private static object OnCoerceUnitName(DependencyObject o, object value)
        {
            RecipeEditValueLayoutItem recipeControl = o as RecipeEditValueLayoutItem;
            if (recipeControl != null)
                return recipeControl.OnCoerceUnitName((String)value);
            else
                return value;
        }

        private static void OnUnitNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeEditValueLayoutItem recipeControl = o as RecipeEditValueLayoutItem;
            if (recipeControl != null)
                recipeControl.OnUnitNameChanged((String)e.OldValue, (String)e.NewValue);
        }

        protected virtual String OnCoerceUnitName(String value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnUnitNameChanged(String oldValue, String newValue)
        {
            if (oldValue != newValue)
            {
                if (Content is IDataValueUI)
                {
                    var control = Content as IDataValueUI;
                    control.UnitName = newValue;
                }
            }
        }

        public String UnitName
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(UnitNameProperty);
            }
            set
            {
                SetValue(UnitNameProperty, value);
            }
        }
        #endregion

        #region MinValue
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(double), typeof(RecipeEditValueLayoutItem), new UIPropertyMetadata(0.0, new PropertyChangedCallback(OnMinValueChanged), new CoerceValueCallback(OnCoerceMinValue)));

        private static object OnCoerceMinValue(DependencyObject o, object value)
        {
            RecipeEditValueLayoutItem recipeControl = o as RecipeEditValueLayoutItem;
            if (recipeControl != null)
                return recipeControl.OnCoerceMinValue((double)value);
            else
                return value;
        }

        private static void OnMinValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeEditValueLayoutItem recipeControl = o as RecipeEditValueLayoutItem;
            if (recipeControl != null)
                recipeControl.OnMinValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceMinValue(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMinValueChanged(double oldValue, double newValue)
        {
            if (oldValue != newValue)
            {
                if (Content is IDataValueUI)
                {
                    var control = Content as IDataValueUI;
                    control.MinValue = Convert.ToDecimal(newValue);
                }
            }
        }

        public double MinValue
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(MinValueProperty);
            }
            set
            {
                SetValue(MinValueProperty, value);
            }
        }
        #endregion

        #region MaxValue
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(RecipeEditValueLayoutItem), new UIPropertyMetadata(100.0, new PropertyChangedCallback(OnMaxValueChanged), new CoerceValueCallback(OnCoerceMaxValue)));

        private static object OnCoerceMaxValue(DependencyObject o, object value)
        {
            RecipeEditValueLayoutItem recipeControl = o as RecipeEditValueLayoutItem;
            if (recipeControl != null)
                return recipeControl.OnCoerceMaxValue((double)value);
            else
                return value;
        }

        private static void OnMaxValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeEditValueLayoutItem recipeControl = o as RecipeEditValueLayoutItem;
            if (recipeControl != null)
                recipeControl.OnMaxValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual double OnCoerceMaxValue(double value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMaxValueChanged(double oldValue, double newValue)
        {
            if (oldValue != newValue)
            {
                if (Content is IDataValueUI)
                {
                    var control = Content as IDataValueUI;
                    control.MaxValue = Convert.ToDecimal(newValue);
                }
            }
        }

        public double MaxValue
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(MaxValueProperty);
            }
            set
            {
                SetValue(MaxValueProperty, value);
            }
        }
        #endregion

        #region DecimalDigits
        public static readonly DependencyProperty DecimalDigitsProperty = DependencyProperty.Register("DecimalDigits", typeof(int), typeof(RecipeEditValueLayoutItem), new UIPropertyMetadata(2, new PropertyChangedCallback(OnDecimalDigitsChanged), new CoerceValueCallback(OnCoerceDecimalDigits)));

        private static object OnCoerceDecimalDigits(DependencyObject o, object value)
        {
            RecipeEditValueLayoutItem recipeControl = o as RecipeEditValueLayoutItem;
            if (recipeControl != null)
                return recipeControl.OnCoerceDecimalDigits((int)value);
            else
                return value;
        }

        private static void OnDecimalDigitsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeEditValueLayoutItem recipeControl = o as RecipeEditValueLayoutItem;
            if (recipeControl != null)
                recipeControl.OnDecimalDigitsChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceDecimalDigits(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            if (Tag is UFDataValueEntity && !(Tag as UFDataValueEntity).DataType.IsDecimalType())
                return 0;
            return value;
        }

        protected virtual void OnDecimalDigitsChanged(int oldValue, int newValue)
        {
            if (oldValue != newValue)
            {
                if (Content is IDataValueUI)
                {
                    var control = Content as IDataValueUI;
                    control.DecimalDigits = newValue;
                }
            }
        }

        public int DecimalDigits
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(DecimalDigitsProperty);
            }
            set
            {
                SetValue(DecimalDigitsProperty, value);
            }
        }
        #endregion

        #region MaxLength
        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register("MaxLength", typeof(int), typeof(RecipeEditValueLayoutItem), new UIPropertyMetadata(-1, new PropertyChangedCallback(OnMaxLengthChanged), new CoerceValueCallback(OnCoerceMaxLength)));

        private static object OnCoerceMaxLength(DependencyObject o, object value)
        {
            RecipeEditValueLayoutItem recipeControl = o as RecipeEditValueLayoutItem;
            if (recipeControl != null)
                return recipeControl.OnCoerceMaxLength((int)value);
            else
                return value;
        }

        private static void OnMaxLengthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RecipeEditValueLayoutItem recipeControl = o as RecipeEditValueLayoutItem;
            if (recipeControl != null)
                recipeControl.OnMaxLengthChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceMaxLength(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMaxLengthChanged(int oldValue, int newValue)
        {
            if (oldValue != newValue)
            {
                if (Content is IDataValueUI)
                {
                    var control = Content as IDataValueUI;
                    control.MaxLength = newValue;
                }
            }
        }

        public int MaxLength
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(MaxLengthProperty);
            }
            set
            {
                SetValue(MaxLengthProperty, value);
            }
        }
        #endregion

        #endregion

        #region Methods

        protected String PerformValidation(String propertyName)
        {
            if (Tag is UFRecipeSettings.UFRecipeModel.UFDataValueEntity)
            {
                var dataValue = Tag as UFRecipeSettings.UFRecipeModel.UFDataValueEntity;
                if (propertyName == "MinValue" || propertyName == "MaxValue")
                {
                    if (dataValue.DataType.IsMinMaxType())
                    {
                        double minvalue;
                        double maxvalue;
                        switch (dataValue.DataType)
                        {
                            case UFUAModel.DataType.Byte:
                                minvalue = Convert.ToDouble(Byte.MinValue);
                                maxvalue = Convert.ToDouble(Byte.MaxValue);
                                break;
                            case UFUAModel.DataType.SByte:
                                minvalue = Convert.ToDouble(SByte.MinValue);
                                maxvalue = Convert.ToDouble(SByte.MaxValue);
                                break;
                            case UFUAModel.DataType.UInt16:
                                minvalue = Convert.ToDouble(UInt16.MinValue);
                                maxvalue = Convert.ToDouble(UInt16.MaxValue);
                                break;
                            case UFUAModel.DataType.Int16:
                                minvalue = Convert.ToDouble(Int16.MinValue);
                                maxvalue = Convert.ToDouble(Int16.MaxValue);
                                break;
                            case UFUAModel.DataType.UInt32:
                                minvalue = Convert.ToDouble(UInt32.MinValue);
                                maxvalue = Convert.ToDouble(UInt32.MaxValue);
                                break;
                            case UFUAModel.DataType.Int32:
                                minvalue = Convert.ToDouble(Int32.MinValue);
                                maxvalue = Convert.ToDouble(Int32.MaxValue);
                                break;
                            case UFUAModel.DataType.UInt64:
                                minvalue = Convert.ToDouble(UInt64.MinValue);
                                maxvalue = Convert.ToDouble(UInt64.MaxValue);
                                break;
                            case UFUAModel.DataType.Int64:
                                minvalue = Convert.ToDouble(Int64.MinValue);
                                maxvalue = Convert.ToDouble(Int64.MaxValue);
                                break;
                            case UFUAModel.DataType.Float:
                                minvalue = Convert.ToDouble(Single.MinValue);
                                maxvalue = Convert.ToDouble(Single.MaxValue);
                                break;
                            default:
                                minvalue = Convert.ToDouble(Double.MinValue);
                                maxvalue = Convert.ToDouble(Double.MaxValue);
                                break;
                        }
                        if (propertyName == "MinValue" && (MinValue < minvalue || MinValue > maxvalue || MinValue > MaxValue))
                            return String.Format(Properties.Resources.LayoutDataValueInvalidMinValue, minvalue, Math.Min(maxvalue, MaxValue));
                        else if (propertyName == "MaxValue" && (MaxValue > maxvalue || MaxValue < minvalue || MaxValue < MinValue))
                            return String.Format(Properties.Resources.LayoutDataValueInvalidMaxValue, Math.Max(minvalue, MinValue), maxvalue);
                    }
                }
            }

            return null;
        }
        #endregion

        #region Override Methods
        public override void SetContentValues()
        {
            if (Tag is UFDataValueEntity && !(Tag as UFDataValueEntity).DataType.IsDecimalType())
                DecimalDigits = 0;

            var dataValueUI = Content as IDataValueUI;
            if (dataValueUI != null)
            {
                dataValueUI.SetEnumOptions(EnumOptions);
                dataValueUI.UnitName = UnitName;
                dataValueUI.MinValue = (decimal)MinValue;
                dataValueUI.MaxValue = (decimal)MaxValue;
                dataValueUI.DecimalDigits = DecimalDigits;
                dataValueUI.MaxLength = MaxLength;
            }
        }

        protected override bool CanShowProperty(string propertyName)
        {
            if (!base.CanShowProperty(propertyName))
                return false;

            if (propertyName == "EnumOptions")
            {
                if (ControlType != EditValueControlTypeEnum.ComboBox && ControlType != EditValueControlTypeEnum.RadioButton && ControlType != EditValueControlTypeEnum.ListView)
                    return false;
            }
            else if (propertyName == "UnitName" || propertyName == "MinValue" || propertyName == "MaxValue")
            {
                if (ControlType != EditValueControlTypeEnum.EditDisplay)
                    return false;

                if (Tag is UFRecipeSettings.UFRecipeModel.UFDataValueEntity)
                {
                    var dataValue = Tag as UFRecipeSettings.UFRecipeModel.UFDataValueEntity;
                    return dataValue.DataType.IsMinMaxType();
                }
            }
            else if (propertyName == "DecimalDigits")
            {
                if (ControlType != EditValueControlTypeEnum.EditDisplay)
                    return false;

                if (Tag is UFRecipeSettings.UFRecipeModel.UFDataValueEntity)
                {
                    var dataValue = Tag as UFRecipeSettings.UFRecipeModel.UFDataValueEntity;
                    return dataValue.DataType.IsDecimalType();
                }
            }
            else if (propertyName == "MaxLength")
            {
                if (ControlType != EditValueControlTypeEnum.EditDisplay)
                    return false;

                if (Tag is UFRecipeSettings.UFRecipeModel.UFDataValueEntity)
                {
                    var dataValue = Tag as UFRecipeSettings.UFRecipeModel.UFDataValueEntity;
                    return dataValue.DataType.IsVariableLenght();
                }
            }

            return true;
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

        [Browsable(false)]
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
        
    }
}
