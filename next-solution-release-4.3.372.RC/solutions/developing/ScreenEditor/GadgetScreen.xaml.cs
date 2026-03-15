using GadgetLibrary;
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

namespace ScreenManager
{
    /// <summary>
    /// Interaction logic for GadgetScreen.xaml
    /// </summary>
    public partial class GadgetScreen : UserControl, IGadget
    {
        private bool _IsGadgetLarge;

        public GadgetScreen()
        {
            InitializeComponent();
        }

        #region IGadget Members

        public void OnShowOptions(OptionButtonTypes type)
        {
            var fe = container.Content as FrameworkElement;
            if (fe == null)
                return;

            if (type.Equals(OptionButtonTypes.Resize))
            {
                if (_IsGadgetLarge == false)
                {
                    fe.Height = fe.ActualHeight * 2;
                    fe.Width = fe.ActualWidth * 2;
                    _IsGadgetLarge = true;
                }
                else
                {
                    fe.Height = fe.ActualHeight / 2;
                    fe.Width = fe.ActualWidth / 2;
                    _IsGadgetLarge = false;
                }
            }
        }

        #endregion
    }
}
