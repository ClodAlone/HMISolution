using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Grid;
using Utilities.WPF;
using WPFUtilities;

namespace ADEditor.Controls
{
    /// <summary>
    /// Interaction logic for SelectUser.xaml
    /// </summary>
    public partial class SelectUser : UserControl
    {
        public SelectUser()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {

                var listroles = DataContext as List<UFUserModel.UFRole>;
                if (listroles == null)
                    return;

                treeListControl.BeginDataUpdate();

                treeListView.Nodes.Clear();
                //var itemRoot = treeListControl.AddNode(new TreeItemControl("Root"));
                foreach (UFUserModel.UFRole t in listroles)
                {
                    var roleNode = treeListControl.AddNode(new TreeItemControl(t.Name), null, t);
                    foreach (var u in t.UFUsers)
                    {
                        treeListControl.AddNode(new TreeItemControl(u.Name), roleNode, u);
                    }
                }

                treeListControl.EndDataUpdate();

                //itemRoot.IsExpanded = true;
            };
        }

        private void OnRowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            e.Handled = true;
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault();

            //Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            //{
            //    treeListControl.Focusable = true;
            //    treeListControl.Focus();
            //});

            if (selected != null)
            {
                if (selected.Tag is UFUserModel.UFRole || selected.Tag is UFUserModel.UFUser)
                {
                    var wnd = this.FindParent<Window>();
                    if (wnd != null)
                    {
                        e.Handled = true;
                        wnd.DialogResult = true;
                        wnd.Close();
                    }
                }
            }
        }
    }
}
