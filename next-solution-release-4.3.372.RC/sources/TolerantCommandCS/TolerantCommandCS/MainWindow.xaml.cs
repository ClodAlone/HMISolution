using System.Windows;
using Bornander.UI;

namespace Bornander.UI.Commands.Test
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel();
        }
    }
}
