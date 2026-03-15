using DocumentManager.ComponentService;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UFProjectManager.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for ScreenUriPropertyEditor.xaml
    /// </summary>
    public partial class StartTypePropertyEditor : UserControl
    {
        bool bInit = false;

        public StartTypePropertyEditor()
        {
            InitializeComponent();
        }

        private void cmbStartType_PopupOpening(object sender, DevExpress.Xpf.Editors.OpenPopupEventArgs e)
        {
            if (!bInit)
            {
                bInit = true;
                var startTypes = Enum.GetValues(typeof(StartType)).Cast<StartType>().ToList();
                startTypes.Remove(StartType.GalleryPage);
                cmbStartType.ItemsSource = startTypes;
            }
        }

        private void cmbStartType_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            //if (bInit)
            //    cmbStartType.Text = cmbStartType.SelectedItem.ToString();
        }
    }
}
