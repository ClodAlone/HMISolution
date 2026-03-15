using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CommandExplorer.Controls
{
    /// <summary>
    /// Interaction logic for PropertyControl.xaml
    /// </summary>
    public partial class PropertyControl : UserControl, IDisposable
    {
        GridControl GridControl;
        CommandEditorUI Container;

        public PropertyControl(GridControl gridControl, CommandEditorUI container)
        {
            InitializeComponent();

            GridControl = gridControl;
            Container = container;
        }

        public void Dispose()
        {
            GridControl.Dispose();
            Container.Dispose();
        }
    }
}
