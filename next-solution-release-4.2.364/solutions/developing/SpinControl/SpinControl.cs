using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
using OPCUAViewModel;
using System.Windows.Automation.Peers;
using Utilities.WPF;
using Utilities;
using ViewModelLib;
using System.Windows.Media.Effects;
using UFInterfaces;
using ScreenSettings;
using System.IO;
using DynamicTagAwareHelper;
using RangeBaseControl;
using Converters;

namespace SpinControl
{
    /// <summary>
    /// Interaction logic for SpinControl.xaml
    /// </summary>
    public class SpinControl : RangeBaseControl.RangeBaseControl
    {
        #region Declarations
        bool previousClipToBounds;
        bool errorEffectOn;
        const Decimal DefaultMinimumValue = 0,
            DefaultMaximumValue = 100,
            DefaultValue = DefaultMinimumValue,
            DefaultChange = 1;
        const int DefaultDecimalPlaces = 0;
        #endregion
        #region ctor
        static SpinControl()
        {
            InitializeCommands();
            ////This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            ////This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SpinControl), new FrameworkPropertyMetadata(typeof(SpinControl)));
        }
        public SpinControl()
        {
            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            Loaded += (o, e) =>
            {
                if (!bLoaded && !bDispose)
                {
                    bLoaded = true;
                    bInit = true;
                }
            };
        }
        #endregion

        #region override Methods
        protected override void SetEntityError(String error)
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                Grid content = (Grid)this.Template.FindName("content", this);
                if (content == null)
                    return;

                if (String.IsNullOrEmpty(error))
                {
                    if (errorEffectOn)
                    {
                        (content as UIElement).Effect = previousEffect;
                        (content as UIElement).ClipToBounds = previousClipToBounds;
                        //(this as UIElement).Opacity = 1;
                        previousEffect = null;
                        errorEffectOn = false;
                    }
                }
                else
                {
                    if (!errorEffectOn)
                    {
                        errorEffectOn = true;
                        previousEffect = (content as UIElement).Effect;
                        previousClipToBounds = (content as UIElement).ClipToBounds;

                        var effect = new DropShadowEffect
                        {
                            ShadowDepth = 0,
                            BlurRadius = 10,
                            Color = Colors.Red
                        };
                        (content as UIElement).Effect = effect;
                        (content as UIElement).ClipToBounds = false;
                    }
                }
            });
        }
        protected override void UpdateScaleRanges()
        {

        }
        protected override void UpdateScaleStartEndValues()
        {
           
        }
        protected override void ShowMarker(bool showWarning)
        {
           
        }
        protected override void UpdateCustomElements()
        {
        }
        #endregion

        #region DP

        [Browsable(false)]
        public Brush LabelForeground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(LabelForegroundProperty);
            }
            set
            {
                SetValue(LabelForegroundProperty, value);
            }
        }
        [Browsable(false)]
        public Brush ValueForeground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(LabelForegroundProperty);
            }
            set
            {
                SetValue(LabelForegroundProperty, value);
            }
        }
        [Browsable(false)]
        public Brush EngeneeringUnitForeground
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(LabelForegroundProperty);
            }
            set
            {
                SetValue(LabelForegroundProperty, value);
            }
        }
        [Browsable(false)]
        [SvgValueConverter(typeof(ConvertFontSettings))]
        public FontSettings LabelFontSettings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (FontSettings)GetValue(LabelFontSettingsProperty);
            }
            set
            {
                SetValue(LabelFontSettingsProperty, value);
            }
        }
        [Browsable(false)]
        [SvgValueConverter(typeof(ConvertFontSettings))]
        public FontSettings ValueFontSettings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (FontSettings)GetValue(LabelFontSettingsProperty);
            }
            set
            {
                SetValue(LabelFontSettingsProperty, value);
            }
        }
        [Browsable(false)]
        [SvgValueConverter(typeof(ConvertFontSettings))]
        public FontSettings EngeneeringUnitFontSettings
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (FontSettings)GetValue(LabelFontSettingsProperty);
            }
            set
            {
                SetValue(LabelFontSettingsProperty, value);
            }
        }

        #region FormattedValue property

        private static readonly DependencyPropertyKey FormattedValuePropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("FormattedValue", typeof(string), typeof(SpinControl),
            new PropertyMetadata(DefaultValue.ToString()));

        private static readonly DependencyProperty FormattedValueProperty = FormattedValuePropertyKey.DependencyProperty;

        public string FormattedValue
        {
            get
            {
                return (string)GetValue(FormattedValueProperty);
            }
        }

        protected void UpdateFormattedValue(decimal newValue)
        {
            NumberFormatInfo numberFormatInfo = new NumberFormatInfo() { NumberDecimalDigits = DecimalPlaces };
            //  use fixed point, and the built-in NumberFormatInfo
            //  implementation of IFormatProvider
            var formattedValue = newValue.ToString("f", numberFormatInfo);

            SetValue(FormattedValuePropertyKey, formattedValue);
        }
        #endregion


        #region Value property
        [Category("SpinControl")]
        public decimal Value
        {
            get { return (decimal)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(decimal), typeof(SpinControl),
            new FrameworkPropertyMetadata(DefaultValue,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnValueChanged,
                CoerceValue
                ));

        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            SpinControl control = obj as SpinControl;
            if (control != null)
            {
                var newValue = (decimal)args.NewValue;
                var oldValue = (decimal)args.OldValue;

                control.UpdateFormattedValue(newValue);

                RoutedPropertyChangedEventArgs<decimal> e =
                    new RoutedPropertyChangedEventArgs<decimal>(oldValue, newValue, ValueChangedEvent);

                control.OnValueChanged(e);
            }
        }

        virtual protected void OnValueChanged(RoutedPropertyChangedEventArgs<decimal> e)
        {
            RaiseEvent(e);
        }

        private decimal LimitValueByBounds(decimal newValue)
        {
            newValue = (decimal)Math.Max(_StartValue, Math.Min(_EndValue, (double)newValue));
            //  then ensure the number of decimal places is correct.
            //newValue = Decimal.Round(newValue, control.DecimalPlaces);
            return newValue;
        }

        private static object CoerceValue(DependencyObject obj, object value)
        {
            decimal newValue = (decimal)value;
            SpinControl control = obj as SpinControl;

            //if (control != null)
            //{
            //    newValue = LimitValueByBounds(newValue, control);
            //}

            return newValue;
        }


        #endregion

        #region MinimumValue
        public static readonly DependencyProperty MinimumValueProperty = DependencyProperty.Register("Minimum", typeof(decimal), typeof(SpinControl), new UIPropertyMetadata(DefaultMinimumValue, new PropertyChangedCallback(OnMinimumValueChanged), new CoerceValueCallback(OnCoerceMinimumValue)));

        private static object OnCoerceMinimumValue(DependencyObject o, object value)
        {
            SpinControl control = o as SpinControl;
            if (control != null)
                return control.OnCoerceMinimumValue((decimal)value);
            else
                return value;
        }

        private static void OnMinimumValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SpinControl control = o as SpinControl;
            if (control != null)
                control.OnMinimumValueChanged((decimal)e.OldValue, (decimal)e.NewValue);
        }

        protected virtual decimal OnCoerceMinimumValue(decimal value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMinimumValueChanged(decimal oldValue, decimal newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && bLoaded && bInit)
            {
                MinValue = (double)newValue;
            }
        }

        [Category("SpinControl")]
        [Obsolete("Use MinValue instead")]
        public decimal Minimum
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (decimal)GetValue(MinimumValueProperty);
            }
            set
            {
                SetValue(MinimumValueProperty, value);
            }
        }

        #endregion

        #region MaximumValue
        public static readonly DependencyProperty MaximumValueProperty = DependencyProperty.Register("Maximum", typeof(decimal), typeof(SpinControl), new UIPropertyMetadata(DefaultMaximumValue, new PropertyChangedCallback(OnMaximumValueChanged), new CoerceValueCallback(OnCoerceMaximumValue)));

        private static object OnCoerceMaximumValue(DependencyObject o, object value)
        {
            SpinControl control = o as SpinControl;
            if (control != null)
                return control.OnCoerceMaximumValue((decimal)value);
            else
                return value;
        }

        private static void OnMaximumValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SpinControl control = o as SpinControl;
            if (control != null)
                control.OnMaximumValueChanged((decimal)e.OldValue, (decimal)e.NewValue);
        }

        protected virtual decimal OnCoerceMaximumValue(decimal value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnMaximumValueChanged(decimal oldValue, decimal newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if (oldValue != newValue && bLoaded && bInit)
            {
                MaxValue = (double)newValue;
            }
        }
        [Browsable(false)]
        [Obsolete("Use MaxValue instead")]
        public decimal Maximum
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (decimal)GetValue(MaximumValueProperty);
            }
            set
            {
                SetValue(MaximumValueProperty, value);
            }
        }

        #endregion

        #region DecimalPlaces property
        [Category("SpinControl")]
        public int DecimalPlaces
        {
            get { return (int)GetValue(DecimalPlacesProperty); }
            set { SetValue(DecimalPlacesProperty, value); }
        }

        private static readonly DependencyProperty DecimalPlacesProperty =
            DependencyProperty.Register("DecimalPlaces", typeof(int), typeof(SpinControl),
            new PropertyMetadata(DefaultDecimalPlaces));

        #endregion

        #region Change property
        [Category("SpinControl")]
        public decimal Change
        {
            get { return (decimal)GetValue(ChangeProperty); }
            set { SetValue(ChangeProperty, value); }
        }

        private static readonly DependencyProperty ChangeProperty =
            DependencyProperty.Register("Change", typeof(decimal), typeof(SpinControl),
            new PropertyMetadata(DefaultChange));

        #endregion
        #endregion

        #region Command Stuff
        bool bMouseDown;
        public static RoutedCommand IncreaseCommand { get; set; }

        protected static void OnIncreaseCommand(Object sender, ExecutedRoutedEventArgs e)
        {
            SpinControl control = sender as SpinControl;

            if (control != null)
            {
                control.OnIncrease();
            }
        }
        protected void OnIncrease()
        {
            if (bMouseDown == true)
            {
                bMouseDown = false;
                return;
            }

            Value = LimitValueByBounds(Value + Change);
            if (RunningOnServer)
                bMouseDown = true;
        }

        public static RoutedCommand DecreaseCommand { get; set; }

        protected static void OnDecreaseCommand(Object sender, ExecutedRoutedEventArgs e)
        {
            SpinControl control = sender as SpinControl;

            if (control != null)
            {
                control.OnDecrease();
            }
        }
        protected void OnDecrease()
        {
            if (bMouseDown == true)
            {
                bMouseDown = false;
                return;
            }

            Value = LimitValueByBounds(Value - Change);
            if (RunningOnServer)
                bMouseDown = true;
        }

        private static void InitializeCommands()
        {
            IncreaseCommand = new RoutedCommand("IncreaseCommand", typeof(SpinControl));
            DecreaseCommand = new RoutedCommand("DecreaseCommand", typeof(SpinControl));

            CommandManager.RegisterClassCommandBinding(typeof(SpinControl), new CommandBinding(IncreaseCommand, OnIncreaseCommand));
            CommandManager.RegisterClassCommandBinding(typeof(SpinControl), new CommandBinding(DecreaseCommand, OnDecreaseCommand));

            //CommandManager.RegisterClassInputBinding(typeof(SpinControl), new InputBinding(IncreaseCommand, new KeyGesture(Key.Up)));
            //CommandManager.RegisterClassInputBinding(typeof(SpinControl), new InputBinding(IncreaseCommand, new KeyGesture(Key.Right)));
            //CommandManager.RegisterClassInputBinding(typeof(SpinControl), new InputBinding(DecreaseCommand, new KeyGesture(Key.Down)));
            //CommandManager.RegisterClassInputBinding(typeof(SpinControl), new InputBinding(DecreaseCommand, new KeyGesture(Key.Left)));
        }
        #endregion

        #region Events

        private static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<decimal>), typeof(SpinControl));

        public event RoutedPropertyChangedEventHandler<decimal> ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }
        #endregion
    }
}
