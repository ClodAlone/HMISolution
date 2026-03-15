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
namespace WPFUtilities
{
    /// <summary>
    /// Interaction logic for YesNoAllCancelControl.xaml
    /// </summary>
    public partial class YesNoAllCancelControl : UserControl
    {
        public YesNoAllCancelControl(string contText)
        {
            InitializeComponent();
            _ClickedButton = CustomDialogResults.Cancel;
            _ControlText = contText;
            lblControlText.Text = _ControlText;
        }

        public void SetControlText(string contText)
        {
            _ControlText = contText;
            lblControlText.Text = _ControlText;
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            _ClickedButton = CustomDialogResults.Yes;
            Button button = (Button)sender;
            _OwnerWindow = button.FindParent<Window>();
            if (_OwnerWindow != null)
            {
                _OwnerWindow.Close();
            }
        }

        private void YesAll_Click(object sender, RoutedEventArgs e)
        {
            _ClickedButton = CustomDialogResults.YesAll;
            Button button = (Button)sender;
            _OwnerWindow = button.FindParent<Window>();
            if (_OwnerWindow != null)
            {
                _OwnerWindow.Close();
            }
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            _ClickedButton = CustomDialogResults.No;
            Button button = (Button)sender;
            _OwnerWindow = button.FindParent<Window>();
            if (_OwnerWindow != null)
            {
                _OwnerWindow.Close();
            }
        }

        private void NoAll_Click(object sender, RoutedEventArgs e)
        {
            _ClickedButton = CustomDialogResults.NoAll;
            Button button = (Button)sender;
            _OwnerWindow = button.FindParent<Window>();
            if (_OwnerWindow != null)
            {
                _OwnerWindow.Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            _ClickedButton = CustomDialogResults.Cancel;
            Button button = (Button)sender;
            _OwnerWindow = button.FindParent<Window>();
            if (_OwnerWindow != null)
            {
                _OwnerWindow.Close();
            }
        }

        #region Properties
        private CustomDialogResults _ClickedButton;
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

        private string _ControlText;
        public string ControlText
        {
            get
            {
                return _ControlText;
            }
            set
            {
                _ControlText = value;
            }
        }

        private Window _OwnerWindow;
        public Window OwnerWindow
        {
            get
            {
                return _OwnerWindow; 
            }
        }
        #endregion

    }
}
