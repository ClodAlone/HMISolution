using DocumentManager.ComponentService;
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
using Utilities;

namespace UFProjectManager.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for IntegerUpDownPropertyEditor.xaml
    /// </summary>
    public partial class ProjectTypePropertyEditor : UserControl
    {
        public ProjectTypePropertyEditor()
        {
            InitializeComponent();
        }

        bool bInit;
        List<string> itemSource = new List<string>() { string.Empty };
        private void cmbEnum_PopupOpening(object sender, DevExpress.Xpf.Editors.OpenPopupEventArgs e)
        {
            if (!bInit)
            {
                itemSource = Enum.GetValues(typeof(ProjectType)).Cast<ProjectType>().Select(v => v.ToString()).ToList();
                cmbEnum.ItemsSource = itemSource;
                bInit = true;
            }
        }

        private void cmbEnum_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            if (bInit && cmbEnum.SelectedIndex != -1 && itemSource.Count > cmbEnum.SelectedIndex)
                uriButton.Tag = itemSource[cmbEnum.SelectedIndex];
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            uriButton.Tag = string.Empty;
        }
    }
}
