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
using MenuSettings.MenuModel;

namespace UFMenuEditor.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for MenuTypePropertyEditor.xaml
    /// </summary>
    public partial class MenuTypePropertyEditor : UserControl
    {
        public MenuTypePropertyEditor()
        {
            InitializeComponent();

            List<MenuType> atemp = new List<MenuType>();
            atemp.Add(MenuType.Item);
            atemp.Add(MenuType.Separator);
            comboMenuType.ItemsSource = atemp;
        }
    }
}
