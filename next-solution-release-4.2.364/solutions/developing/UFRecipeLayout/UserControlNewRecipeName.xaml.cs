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
using System.Windows.Threading;
using UFRecipeLayout.LayoutItemControls;
using Utilities.WPF;

namespace UFRecipeLayout
{
    /// <summary>
    /// Interaction logic for UserControlNewRecipeName.xaml
    /// </summary>
    public partial class UserControlNewRecipeName : UserControl, IPadSupport, IDataErrorInfo
    {
        readonly internal String[] Recipes;
        public UserControlNewRecipeName(String[] recipes, int maxlenght = -1)
        {
            //RecipeName = Properties.Resources.EnterNewRecipeNameDefaultName;

            InitializeComponent();

            //textRecipeName.Text = RecipeName;
            //textRecipeName.Select(0, textRecipeName.Text.Length);
            Recipes = recipes;
            if (maxlenght > 0)
                textRecipeName.MaxLength = maxlenght;
        }

        #region IPadSupport
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
                int? maxLenght = null;
                if (textRecipeName.MaxLength > 0)
                    maxLenght = textRecipeName.MaxLength;

                var ret = Pads.Pads.ShowAlphaNumericPad(textRecipeName.Text, owner, max: maxLenght);
                if (ret != null)
                    textRecipeName.Text = ret;
            }
        }
        #endregion

        public string RecipeName { get; set; }

        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "RecipeName")
            {
                if (String.IsNullOrEmpty(RecipeName) || (Recipes != null && Recipes.Contains(RecipeName)))
                    return Properties.Resources.EnterNewRecipeNameInvalidName;
            }

            return null;
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
        
        #endregion
    }
}
