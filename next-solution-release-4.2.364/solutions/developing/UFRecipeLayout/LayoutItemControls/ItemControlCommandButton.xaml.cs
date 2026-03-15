using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UFRecipeLayout.LayoutItemControls
{
    /// <summary>
    /// Interaction logic for ItemControlCommandButton.xaml
    /// </summary>
    public partial class ItemControlCommandButton : UserControl, ICommandUI
    {
        internal ItemControlCommandButton()
        {
            InitializeComponent();
        }

        #region ICommandUI Members

        public void SetBinding(ICommand command)
        {
            btnRecipeCommand.Command = command;
        }

        public void SetCaption(string text)
        {
            btnText.Text = text;
        }

        public void SetIcon(BitmapImage bmp)
        {
            btnImage.Source = bmp;
        }

        public void SetToolTip(string text)
        {
            if (!String.IsNullOrEmpty(text))
                btnRecipeCommand.ToolTip = text;
            else
                btnRecipeCommand.ToolTip = null;
        }

        public void ShowIcon(bool showicon)
        {
            if (showicon)
            {
                btnText.Visibility = Visibility.Collapsed;
                btnImage.Visibility = Visibility.Visible;
            }
            else
            {
                btnText.Visibility = Visibility.Visible;
                btnImage.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

    }
}
