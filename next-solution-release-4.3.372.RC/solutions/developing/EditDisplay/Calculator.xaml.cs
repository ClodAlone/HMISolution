using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using WPFUtilities;
using WPFUtilities.Extensions;
using Utilities;

namespace EditDisplay
{
    /// <summary>
    /// Interaction logic for Calculator.xaml
    /// </summary>
    public partial class Calculator : UserControl, IDisposable
    {
        #region DP
        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(IsEnabledProperty, typeof(Calculator));
            dpd.AddValueChangedSafe(this, OnIsEnabledChanged);
        }

        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(IsEnabledProperty, typeof(Calculator));
            dpd.RemoveValueChangedSafe(this, OnIsEnabledChanged);
        }
        private void OnIsEnabledChanged(object sender, EventArgs e)
        {
            var control = sender as Calculator;
            if (control != null)
            {
                control.OnIsEnabledChanged();
            }
        }
        protected virtual void OnIsEnabledChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            grid.Opacity = IsEnabled ? 1.0 : 0.7;
        }

        #region EditValue
        public static readonly DependencyProperty EditValueProperty = DependencyProperty.Register("EditValue", typeof(decimal), typeof(Calculator), new UIPropertyMetadata((decimal)0, new PropertyChangedCallback(OnEditValueChanged), new CoerceValueCallback(OnCoerceEditValue)));

        private static object OnCoerceEditValue(DependencyObject o, object value)
        {
            Calculator control = o as Calculator;
            if (control != null)
                return control.OnCoerceEditValue((decimal)value);
            else
                return value;
        }

        private static void OnEditValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            Calculator control = o as Calculator;
            if (control != null)
                control.OnEditValueChanged((decimal)e.OldValue, (decimal)e.NewValue);
        }

        protected virtual decimal OnCoerceEditValue(decimal value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnEditValueChanged(decimal oldValue, decimal newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public decimal EditValue
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (decimal)GetValue(EditValueProperty);
            }
            set
            {
                SetValue(EditValueProperty, value);
            }
        }

        #endregion
                
        #endregion

        bool bLoaded;
        private DelayedSingleActionInvoker SizeChangedInvoker;
        public Calculator()
        {
            InitializeComponent();

            ScreenSettings.ScreenDocument.SetAutoForceDynamicOnClient(this, true);

            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;

                    
                    display.Width = ActualWidth;
                    display.Height = ActualHeight;

                    SizeChangedInvoker = new DelayedSingleActionInvoker(() =>
                    {
                        if (!bDisposed)
                        {
                            display.Width = ActualWidth;
                            display.Height = ActualHeight;
                        }
                    }, new TimeSpan(0, 0, 0, 0, 100));
                    SizeChanged += (ob, ev) =>
                    {
                        if (SizeChangedInvoker != null)
                            SizeChangedInvoker.BeginInvoke();
                    };

                    OverrideBaseProperties();
                }
            };
        }
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;
            SizeChangedInvoker = null;
            DetachOverrideBaseProperties();
        }
    }
}
