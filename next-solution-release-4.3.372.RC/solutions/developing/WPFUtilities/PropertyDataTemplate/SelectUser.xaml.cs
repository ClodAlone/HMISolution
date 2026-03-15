using System.Collections.Generic;
using System.Windows.Controls;

namespace WPFUtilities.PropertyDataTemplate
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
    }
}
