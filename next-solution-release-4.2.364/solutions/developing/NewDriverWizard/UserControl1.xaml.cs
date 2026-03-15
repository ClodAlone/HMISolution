using System;
using System.Collections.Generic;
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

namespace NewDriverWizard
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        #region DP
        public static readonly DependencyProperty SelectedStepProperty = DependencyProperty.Register("SelectedStep", typeof(int), typeof(UserControl1), new UIPropertyMetadata(0, new PropertyChangedCallback(OnSelectedStepChanged), new CoerceValueCallback(OnCoerceSelectedStep)));

        private static object OnCoerceSelectedStep(DependencyObject o, object value)
        {
            UserControl1 userControl1 = o as UserControl1;
            if (userControl1 != null)
                return userControl1.OnCoerceSelectedStep((int)value);
            else
                return value;
        }

        private static void OnSelectedStepChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            UserControl1 userControl1 = o as UserControl1;
            if (userControl1 != null)
                userControl1.OnSelectedStepChanged((int)e.OldValue, (int)e.NewValue);
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

        public UserControl1()
        {
            InitializeComponent();
        }
    }
}
