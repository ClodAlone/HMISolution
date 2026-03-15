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
using Utilities.WPF;

namespace AlarmWindow.PopUp
{
    /// <summary>
    /// Interaction logic for AddComment.xaml
    /// </summary>
    public partial class AddComment : UserControl, IDataErrorInfo
    {

        #region UserTempComment
        public static readonly DependencyProperty UserTempCommentProperty = DependencyProperty.Register("UserTempComment", typeof(string), typeof(AddComment), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnUserTempCommentChanged), new CoerceValueCallback(OnCoerceUserTempComment)));

        private static object OnCoerceUserTempComment(DependencyObject o, object value)
        {
            AddComment control = o as AddComment;
            if (control != null)
                return control.OnCoerceUserTempComment((string)value);
            else
                return value;
        }

        private static void OnUserTempCommentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            AddComment control = o as AddComment;
            if (control != null)
                control.OnUserTempCommentChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceUserTempComment(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnUserTempCommentChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
        }

        public string UserTempComment
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(UserTempCommentProperty);
            }
            set
            {
                SetValue(UserTempCommentProperty, value);
            }
        }

        #endregion
        private bool MandatoryComment { get; set; }
        public AddComment(bool mandatoryComment)
        {
            InitializeComponent();
            DataContext = this;
            MandatoryComment = mandatoryComment;
        }
        private void selectButton_Click(object sender, RoutedEventArgs e)
        {
            UserTempComment = Pads.Pads.ShowAlphaNumericPad(UserTempComment, this.FindParent<Window>());
        }
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
        bool IsValid(string name)
        {
            if (String.IsNullOrEmpty(name) && MandatoryComment)
                return false;

            return true;
        }
        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "UserTempComment")
            {
                if (!IsValid(UserTempComment))
                {
                    return Properties.Resources.CommentIsInvalid;
                }
                else
                    return null;
            }
            return null;
        }
        #endregion

    }
}
