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
using StringManager.ComponentService;

namespace StringManager.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for CulturePropertyControlxaml.xaml
    /// </summary>
    public partial class CulturePropertyControlxaml : UserControl
    {
        public CulturePropertyControlxaml()
        {
            InitializeComponent();

            var doc = StringEditorManagerComponent.stringEditorManagerComponent.Workspace.ContextDocument;
            if (doc != null)
            {
                var list = StringEditorManagerComponent.stringEditorManagerComponent.GetListAvailableCultures(doc);
                cmbCultures.ItemsSource = list;
            }
        }
    }
}
