using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Utilities;

namespace WizardPluginHelpers
{
    /// <summary>
    /// Interaction logic for HeaderControl.xaml
    /// </summary>
    public partial class HeaderControl : UserControl
    {
        #region DP

        #region ShowSteps
        public static readonly DependencyProperty ShowStepsProperty = DependencyProperty.Register("ShowSteps", typeof(bool), typeof(HeaderControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowStepsChanged), new CoerceValueCallback(OnCoerceShowSteps)));

        private static object OnCoerceShowSteps(DependencyObject o, object value)
        {
            HeaderControl control = o as HeaderControl;
            if (control != null)
                return control.OnCoerceShowSteps((bool)value);
            else
                return value;
        }

        private static void OnShowStepsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HeaderControl control = o as HeaderControl;
            if (control != null)
                control.OnShowStepsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowSteps(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowStepsChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            stepLabel.Visibility = newValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool ShowSteps
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowStepsProperty);
            }
            set
            {
                SetValue(ShowStepsProperty, value);
            }
        }

        #endregion

        #region SelectedStep
        public static readonly DependencyProperty SelectedStepProperty = DependencyProperty.Register("SelectedStep", typeof(int), typeof(HeaderControl), new UIPropertyMetadata(0, new PropertyChangedCallback(OnSelectedStepChanged), new CoerceValueCallback(OnCoerceSelectedStep)));

        private static object OnCoerceSelectedStep(DependencyObject o, object value)
        {
            HeaderControl control = o as HeaderControl;
            if (control != null)
                return control.OnCoerceSelectedStep((int)value);
            else
                return value;
        }

        private static void OnSelectedStepChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HeaderControl control = o as HeaderControl;
            if (control != null)
                control.OnSelectedStepChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceSelectedStep(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnSelectedStepChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            UpdateStepLabel();
        }

        public int SelectedStep
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(SelectedStepProperty);
            }
            set
            {
                SetValue(SelectedStepProperty, value);
            }
        }

        #endregion

        #region HeaderTitle
        public static readonly DependencyProperty HeaderTitleProperty = DependencyProperty.Register("HeaderTitle", typeof(String), typeof(HeaderControl), new UIPropertyMetadata(string.Empty));
        public String HeaderTitle
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (String)GetValue(HeaderTitleProperty);
            }
            set
            {
                SetValue(HeaderTitleProperty, value);
            }
        }
        #endregion


        #region StepNumber
        public static readonly DependencyProperty StepNumberProperty = DependencyProperty.Register("StepNumber", typeof(int), typeof(HeaderControl), new UIPropertyMetadata(1, new PropertyChangedCallback(OnStepNumberChanged), new CoerceValueCallback(OnCoerceStepNumber)));

        private static object OnCoerceStepNumber(DependencyObject o, object value)
        {
            HeaderControl control = o as HeaderControl;
            if (control != null)
                return control.OnCoerceStepNumber((int)value);
            else
                return value;
        }

        private static void OnStepNumberChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HeaderControl control = o as HeaderControl;
            if (control != null)
                control.OnStepNumberChanged((int)e.OldValue, (int)e.NewValue);
        }

        protected virtual int OnCoerceStepNumber(int value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnStepNumberChanged(int oldValue, int newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            UpdateStepLabel();
        }

        public int StepNumber
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (int)GetValue(StepNumberProperty);
            }
            set
            {
                SetValue(StepNumberProperty, value);
            }
        }

        #endregion

        #region StepTitles
        public static readonly DependencyProperty StepTitlesProperty = DependencyProperty.Register("StepTitles", typeof(List<String>), typeof(HeaderControl), new UIPropertyMetadata(null));
        public List<String> StepTitles
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (List<String>)GetValue(StepTitlesProperty);
            }
            set
            {
                SetValue(StepTitlesProperty, value);
            }
        }

        #endregion


        #region StepTooltips
        public static readonly DependencyProperty StepTooltipsProperty = DependencyProperty.Register("StepTooltips", typeof(List<String>), typeof(HeaderControl), new UIPropertyMetadata(null));
        public List<String> StepTooltips
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (List<String>)GetValue(StepTooltipsProperty);
            }
            set
            {
                SetValue(StepTooltipsProperty, value);
            }
        }

        #endregion


        #region Image
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(BitmapImage), typeof(HeaderControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnImageChanged), new CoerceValueCallback(OnCoerceImage)));

        private static object OnCoerceImage(DependencyObject o, object value)
        {
            HeaderControl control = o as HeaderControl;
            if (control != null)
                return control.OnCoerceImage((BitmapImage)value);
            else
                return value;
        }

        private static void OnImageChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            HeaderControl control = o as HeaderControl;
            if (control != null)
                control.OnImageChanged((BitmapImage)e.OldValue, (BitmapImage)e.NewValue);
        }

        protected virtual BitmapImage OnCoerceImage(BitmapImage value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnImageChanged(BitmapImage oldValue, BitmapImage newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public BitmapImage Image
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (BitmapImage)GetValue(ImageProperty);
            }
            set
            {
                SetValue(ImageProperty, value);
            }
        }

        #endregion

        #endregion

        bool bLoaded;
        int minColNumber = 7;
        System.Globalization.TextInfo textInfo;
        public HeaderControl()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;
                textInfo = new System.Globalization.CultureInfo(System.Globalization.CultureInfo.CurrentCulture.Name, false).TextInfo;
                UpdateStepLabel();
            };
        }

        private void UpdateStepLabel()
        {
            try
            {
                if(bLoaded)
                    stepLabel.Text = string.Format(textInfo.ToTitleCase(Properties.Resources.StepInfo), SelectedStep + 1, StepNumber + 1, textInfo.ToTitleCase(StepTooltips[SelectedStep]));
            }
            catch (Exception)
            {
            }
        }
    }
}
