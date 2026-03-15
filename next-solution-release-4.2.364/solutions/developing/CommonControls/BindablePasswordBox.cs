using DevExpress.Xpf.Editors;
using System;
using System.Windows;
using System.Windows.Controls;
using Utilities;

namespace CommonControls
{
    public class BindablePasswordBox : Decorator
    {
        /// <summary>
        /// The password dependency property.
        /// </summary>
        public static readonly DependencyProperty PasswordProperty;
        public static readonly DependencyProperty MinCustomHeightProperty;

        private bool isPreventCallback;
        private EditValueChangedEventHandler savedCallback;

        /// <summary>
        /// Static constructor to initialize the dependency properties.
        /// </summary>
        static BindablePasswordBox()
        {
            PasswordProperty = DependencyProperty.Register(
                "Password",
                typeof(string),
                typeof(BindablePasswordBox),
                new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnPasswordPropertyChanged))
            );
            MinCustomHeightProperty = DependencyProperty.Register(
                "MinCustomHeight",
                typeof(double),
                typeof(BindablePasswordBox),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnMinCustomHeightPropertyChanged))
            );
        }

        /// <summary>
        /// The MinCustomHeight dependency property.
        /// </summary>
        public double MinCustomHeight
        {
            get { return (double)GetValue(MinCustomHeightProperty); }
            set { SetValue(MinCustomHeightProperty, value); }
        }

        private static void OnMinCustomHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs eventArgs)
        {
            BindablePasswordBox bindablePasswordBox = (BindablePasswordBox)d;
            PasswordBoxEdit passwordBox = (PasswordBoxEdit)bindablePasswordBox.Child;

            if (bindablePasswordBox.isPreventCallback)
            {
                return;
            }

            passwordBox.MinHeight = (double)eventArgs.NewValue;
        }

        /// <summary>
        /// Saves the password changed callback and sets the child element to the password box.
        /// </summary>
        public BindablePasswordBox()
        {
            savedCallback = HandlePasswordChanged;

            PasswordBoxEdit passwordBox = new PasswordBoxEdit() {
                //Background = ApplicationPropertiesHelper.GetProperty("CurrentSkinBackColor") as System.Windows.Media.Brush,
                //Foreground = ApplicationPropertiesHelper.GetProperty("CurrentSkinForeColor") as System.Windows.Media.Brush
            };
            passwordBox.EditValueChanged += savedCallback;
            Child = passwordBox;
        }

        /// <summary>
        /// The password dependency property.
        /// </summary>
        public string Password
        {
            get { return GetValue(PasswordProperty) as string; }
            set { SetValue(PasswordProperty, value); }
        }

        /// <summary>
        /// Handles changes to the password dependency property.
        /// </summary>
        /// <param name="d">the dependency object</param>
        /// <param name="eventArgs">the event args</param>
        private static void OnPasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs eventArgs)
        {
            BindablePasswordBox bindablePasswordBox = (BindablePasswordBox)d;
            PasswordBoxEdit passwordBox = (PasswordBoxEdit)bindablePasswordBox.Child;

            if (bindablePasswordBox.isPreventCallback)
            {
                return;
            }

            passwordBox.EditValueChanged -= bindablePasswordBox.savedCallback;
            passwordBox.Password = (eventArgs.NewValue != null) ? eventArgs.NewValue.ToString() : "";
            passwordBox.EditValueChanged += bindablePasswordBox.savedCallback;
        }

        /// <summary>
        /// Handles the password changed event.
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="eventArgs">the event args</param>
        private void HandlePasswordChanged(object sender, EditValueChangedEventArgs eventArgs)
        {
            PasswordBoxEdit passwordBox = (PasswordBoxEdit)sender;

            isPreventCallback = true;
            Password = passwordBox.Password;
            isPreventCallback = false;
        }

        /// <summary>
        /// Select content in PasswordBoxEdit.
        /// </summary>
        public void SelectAll()
        {
            PasswordBoxEdit passwordBox = (PasswordBoxEdit)Child;
            if (passwordBox != null)
                passwordBox.SelectAll();
        }
    }
}
