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
using UFInterfaces.Editors;
using Utilities;
using Utilities.WPF;

namespace SysVariables.Controls
{
    /// <summary>
    /// Interaction logic for ControlEditor.xaml
    /// </summary>
    public partial class ControlEditor : UserControl, ISelectEntityReference
    {
        readonly SysVariables sysVariables;
        bool bLoaded;
        public ControlEditor(SysVariables sysvar)
        {
            InitializeComponent();
            sysVariables = sysvar;

            Loaded += (o, e) =>
                {
                    if (!bLoaded)
                    {
                        bLoaded = true;

                        listBox.ItemsSource = sysVariables.GetVariables();
                        UpdateSelectedItem();
                        var wnd = this.FindParent<Window>();
                        if (wnd != null)
                        {
                            wnd.Closing += (s, c) =>
                            {
                                var selected = listBox.SelectedItem as String;
                                if (selected != null)
                                {
                                    SelectedReference = sysVariables.GetReference(selected);
                                    SelectedReferences = null;
                                }
                                else
                                {
                                    SelectedReference = null;
                                    SelectedReferences = null;
                                }
                            };
                        }
                    }
                };
        }

        private void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var wnd = this.FindParent<Window>();
            if (wnd != null)
            {
                wnd.DialogResult = true;
                wnd.Close();
            }
        }

        #region ISelectEntityReference
        public object SelectedReference { get; set; }

        public List<object> SelectedReferences { get; set; }
        string selectedInstanceOnEdit;
        public void BringIntoView(object selectedReference)
        {
            selectedInstanceOnEdit = (from t in sysVariables.GetVariables() where t == (selectedReference as OPCUAViewModel.OPCUAEntityReference)?.HumanReadableNoProject select t).FirstOrDefault();
            if (bLoaded)
                UpdateSelectedItem();
        }
        void UpdateSelectedItem()
        {
            if (bLoaded && selectedInstanceOnEdit != null)
                listBox.SelectedItem = selectedInstanceOnEdit;
        }
        #endregion
    }
}
