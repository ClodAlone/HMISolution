using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace TestDynamics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        bool bDesign;
        public MainWindow()
        {
            InitializeComponent();
            //EPNextControls.ShiftDataGridControl.ShiftDataGrid shiftDataGrid = new EPNextControls.ShiftDataGridControl.ShiftDataGrid();
            try
            {
                var list = FindAndLoadDLL.LoadDLLs<UserControl>("E:\\SharedDocs\\Beppe\\TecnoEletronic\\Progea\\Controls\\", "EPNextControls.dll", false);
                if (list.Count > 0)
                {
                    list[1].ClearValue(FrameworkElement.WidthProperty);
                    list[1].ClearValue(FrameworkElement.HeightProperty);
                    Container.Content = list[1];
                } 
            }
            catch (Exception ex)
            {
                
                throw;
            }            
            if (DesignerProperties.GetIsInDesignMode(this) || bDesign)
            {
                bDesign = true;
                Container.IsEnabled = false;
            }
        }

        bool bDispose;
        public void Dispose()
        {
            if (bDispose)
                return;
            bDispose = true;
            if (Container.Content != null && Container.Content is IDisposable)
                (Container.Content as IDisposable).Dispose();
        }
    }
}
