using System.Collections.Generic;
using System.Windows.Controls;
using MenuSettings.MenuModel;

namespace UFMenuEditor.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for MarkTagNamePropertyEditor.xaml
    /// </summary>
    public partial class MenuItemTypePropertyEditor : UserControl
    {
        public MenuItemTypePropertyEditor()
        {
            InitializeComponent();
            List<MenuType> atemp = new List<MenuType>();
            atemp.Add(MenuType.Item);
            atemp.Add(MenuType.Separator);
            combo.ItemsSource = atemp;
        }
    }
}
