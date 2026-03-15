using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Utilities;
using ViewModelLib;

namespace WizardPluginHelpers
{
    /// <summary>
    /// Interaction logic for HeaderControl.xaml
    /// </summary>
    public partial class FooterControl : UserControl
    {

        #region CanExecuteNext
        public static readonly DependencyProperty CanExecuteNextProperty = DependencyProperty.Register("CanExecuteNext", typeof(bool), typeof(FooterControl), new UIPropertyMetadata(true));
        public bool CanExecuteNext
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(CanExecuteNextProperty);
            }
            set
            {
                SetValue(CanExecuteNextProperty, value);
            }
        }
        #endregion

        #region CanExecutePrevious
        public static readonly DependencyProperty CanExecutePreviousProperty = DependencyProperty.Register("CanExecutePrevious", typeof(bool), typeof(FooterControl), new UIPropertyMetadata(true));
        public bool CanExecutePrevious
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(CanExecutePreviousProperty);
            }
            set
            {
                SetValue(CanExecutePreviousProperty, value);
            }
        }
        #endregion

        #region ShowHelpButton
        public static readonly DependencyProperty ShowHelpButtonProperty = DependencyProperty.Register("ShowHelpButton", typeof(bool), typeof(FooterControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowHelpButtonChanged), new CoerceValueCallback(OnCoerceShowHelpButton)));

        private static object OnCoerceShowHelpButton(DependencyObject o, object value)
        {
            FooterControl control = o as FooterControl;
            if (control != null)
                return control.OnCoerceShowHelpButton((bool)value);
            else
                return value;
        }

        private static void OnShowHelpButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FooterControl control = o as FooterControl;
            if (control != null)
                control.OnShowHelpButtonChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowHelpButton(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowHelpButtonChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            helpButton.Visibility = newValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool ShowHelpButton
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowHelpButtonProperty);
            }
            set
            {
                SetValue(ShowHelpButtonProperty, value);
            }
        }

        #endregion

        #region ShowNavButtons
        public static readonly DependencyProperty ShowNavButtonsProperty = DependencyProperty.Register("ShowNavButtons", typeof(bool), typeof(FooterControl), new UIPropertyMetadata(true, new PropertyChangedCallback(OnShowNavButtonsChanged), new CoerceValueCallback(OnCoerceShowNavButtons)));

        private static object OnCoerceShowNavButtons(DependencyObject o, object value)
        {
            FooterControl control = o as FooterControl;
            if (control != null)
                return control.OnCoerceShowNavButtons((bool)value);
            else
                return value;
        }

        private static void OnShowNavButtonsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FooterControl control = o as FooterControl;
            if (control != null)
                control.OnShowNavButtonsChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowNavButtons(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowNavButtonsChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            nextCommand.Visibility = newValue ? Visibility.Visible : Visibility.Collapsed;
            previousCommand.Visibility = newValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool ShowNavButtons
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowNavButtonsProperty);
            }
            set
            {
                SetValue(ShowNavButtonsProperty, value);
            }
        }

        #endregion

        #region ShowFinishButton
        public static readonly DependencyProperty ShowFinishButtonProperty = DependencyProperty.Register("ShowFinishButton", typeof(bool), typeof(FooterControl), new UIPropertyMetadata(false, new PropertyChangedCallback(OnShowFinishButtonChanged), new CoerceValueCallback(OnCoerceShowFinishButton)));

        private static object OnCoerceShowFinishButton(DependencyObject o, object value)
        {
            FooterControl control = o as FooterControl;
            if (control != null)
                return control.OnCoerceShowFinishButton((bool)value);
            else
                return value;
        }

        private static void OnShowFinishButtonChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FooterControl control = o as FooterControl;
            if (control != null)
                control.OnShowFinishButtonChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual bool OnCoerceShowFinishButton(bool value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnShowFinishButtonChanged(bool oldValue, bool newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public bool ShowFinishButton
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (bool)GetValue(ShowFinishButtonProperty);
            }
            set
            {
                SetValue(ShowFinishButtonProperty, value);
            }
        }

        #endregion

        #region SelectedStep
        public static readonly DependencyProperty SelectedStepProperty = DependencyProperty.Register("SelectedStep", typeof(int), typeof(FooterControl), new UIPropertyMetadata(0, new PropertyChangedCallback(OnSelectedStepChanged), new CoerceValueCallback(OnCoerceSelectedStep)));

        private static object OnCoerceSelectedStep(DependencyObject o, object value)
        {
            FooterControl control = o as FooterControl;
            if (control != null)
                return control.OnCoerceSelectedStep((int)value);
            else
                return value;
        }

        private static void OnSelectedStepChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FooterControl control = o as FooterControl;
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
        #region StepNumber
        public static readonly DependencyProperty StepNumberProperty = DependencyProperty.Register("StepNumber", typeof(int), typeof(FooterControl), new UIPropertyMetadata(1, new PropertyChangedCallback(OnStepNumberChanged), new CoerceValueCallback(OnCoerceStepNumber)));

        private static object OnCoerceStepNumber(DependencyObject o, object value)
        {
            FooterControl control = o as FooterControl;
            if (control != null)
                return control.OnCoerceStepNumber((int)value);
            else
                return value;
        }

        private static void OnStepNumberChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            FooterControl control = o as FooterControl;
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
        #region Events
        public event EventHandler Next;
        protected void OnNext()
        {
            Next?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler Prev;
        protected void OnPrev()
        {
            Prev?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler Cancel;
        protected void OnCancel()
        {
            Cancel?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler Finish;
        protected void OnFinish()
        {
            Finish?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler Help;
        protected void OnHelp()
        {
            Help?.Invoke(this, EventArgs.Empty);
        }
        #endregion
        #region command
        private void OnHelpCommand(object sender, ExecutedRoutedEventArgs e)
        {
            OnHelp();
        }
        private void OnFinishCommand(object sender, ExecutedRoutedEventArgs e)
        {
            OnFinish();
        }
        private void OnNextStep(object sender, ExecutedRoutedEventArgs e)
        {
            OnNext();
            finishButton.Visibility = SelectedStep == StepNumber || ShowFinishButton ? Visibility.Visible : Visibility.Hidden;
        }
        private void OnPrevStep(object sender, ExecutedRoutedEventArgs e)
        {
            OnPrev();
            finishButton.Visibility = SelectedStep == StepNumber || ShowFinishButton ? Visibility.Visible : Visibility.Hidden;
        }
        private void OnCancelCommand(object sender, ExecutedRoutedEventArgs e)
        {
            OnCancel();
        }
        void CanAlwaysExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        void CanNextStep(object sender, CanExecuteRoutedEventArgs e)
        {
            finishButton.Visibility = SelectedStep == StepNumber || ShowFinishButton ? Visibility.Visible : Visibility.Hidden;
            e.CanExecute = SelectedStep <  StepNumber && CanExecuteNext;
        }
        void CanPrevStep(object sender, CanExecuteRoutedEventArgs e)
        {
            finishButton.Visibility = SelectedStep == StepNumber || ShowFinishButton ? Visibility.Visible : Visibility.Hidden;
            e.CanExecute = SelectedStep > 0 && CanExecutePrevious;
        }
        void CanFinishCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        #endregion

        bool bLoaded;
        public FooterControl()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true; 
            };
        }
    }
}
