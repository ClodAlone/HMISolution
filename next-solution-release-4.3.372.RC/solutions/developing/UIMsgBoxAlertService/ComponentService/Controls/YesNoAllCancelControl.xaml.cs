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
using UIMsgBoxAlertService.ComponentService;
using Utilities;
using Utilities.WPF;

// FOGBUZ 11407
namespace UIMsgBoxAlertService.ComponentService.Controls
{
    /// <summary>
    /// Interaction logic for YesNoAllCancelControl.xaml
    /// </summary>
    public partial class YesNoAllCancelControl : UserControl
    {
        public YesNoAllCancelControl()
        {
            InitializeComponent();
        }

        void Yes_Click(object sender, RoutedEventArgs e)
        {
            ClickedButton = CustomDialogResults.Yes;
            CloseWindow();
        }

        void YesAll_Click(object sender, RoutedEventArgs e)
        {
            ClickedButton = CustomDialogResults.YesAll;
            CloseWindow();
        }

        void No_Click(object sender, RoutedEventArgs e)
        {
            ClickedButton = CustomDialogResults.No;
            CloseWindow();
        }

        void NoAll_Click(object sender, RoutedEventArgs e)
        {
            ClickedButton = CustomDialogResults.NoAll;
            CloseWindow();
        }

        void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ClickedButton = CustomDialogResults.Cancel;
            CloseWindow();
        }

        void CloseWindow()
        {
            var wnd = this.FindParent<Window>();
            if (wnd != null)
            {
                wnd.Close();
            }
        }

        #region Properties
        CustomDialogResults _ClickedButton = CustomDialogResults.Cancel;
        public CustomDialogResults ClickedButton
        {
            get
            {
                return _ClickedButton;
            }
            set
            {
                _ClickedButton = value;
            }
        }
        #endregion

    }
}
