using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Grid;
using System.Windows.Data;
using WPFUtilities;
using Utilities;
using Utilities.WPF;
using System;
using System.Windows.Media.Imaging;

namespace OmronEthernetIP.UI
{
    /// <summary>
    /// Interaction logic for ImportTagsEditorTree.xaml
    /// </summary>
    public partial class PrototypeMemberStringLength : UserControl//, IDisposable
    {
        private bool alreadyLoaded = false;

        TreeListNode itemRoot;
        bool cellEditPendingError = false;

        public PrototypeMemberStringLength()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                Window window = Window.GetWindow(this);
                window.Closing += window_Closing;

                if (alreadyLoaded)
                    return;

                alreadyLoaded = true;              

                OmronEthernetIPStructStringLength.ProMemberView proto = this.DataContext as OmronEthernetIPStructStringLength.ProMemberView;
                treeListView.Nodes.Clear();
                FillItems(ref itemRoot, proto);                
            };

            Unloaded += (o, e) =>
            {
                Window window = Window.GetWindow(this);
                window.Closing -= window_Closing;
            };
        }

        private BitmapImage GetBitmapImageSourceFromMemberViewType(OmronEthernetIPStructStringLength.ProMemberView tag)
        {
            if (tag.ModelType == UFUAModel.ModelType.ObjectType)
                return GetBitmapImageSource("UFUASPrototypesSmall");
            else
                return GetBitmapImageSource("UFUASVariableStringSmall");
        }

        private BitmapImage GetBitmapImageSource(String image)
        {
            return SharedResources.Helpers.ResourceManager.GetCommonImage("UFUAServer", image, false);
        }

        void SetBindingOnProp(TreeListNode node, object bindingSource, string sourcePropName, DependencyProperty targetDP = null)
        {
            if (targetDP == null)
                targetDP = TreeItemControl.ItemHeaderProperty;
            var myBinding = new Binding(sourcePropName);
            myBinding.Source = bindingSource;
            myBinding.Mode = BindingMode.TwoWay; // BindingMode.OneWay;
            myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;            
            BindingOperations.SetBinding(node.Content as PrototypeTreeItemControl, targetDP, myBinding);
        }

        private TreeListNode AddRootTreeItem(OmronEthernetIPStructStringLength.ProMemberView tag)
        {
            return AddTreeItem(null, tag, false);
        }

        private TreeListNode AddTreeItem(TreeListNode parent, OmronEthernetIPStructStringLength.ProMemberView tag, bool includeChildren = true)
        {
            if (parent != null && !parent.IsExpanded)
            {
                parent.IsExpanded = true;
                var list = (from p in parent.Nodes where p.Tag == tag select p).ToList();
                if (list.Count > 0)
                    return list[0];
            }

            var ic = new PrototypeTreeItemControl(tag, GetBitmapImageSourceFromMemberViewType(tag));
            TreeListNode newItem = treeListControl.AddNode(ic, parent, tag);            

            var t = tag as OmronEthernetIPStructStringLength.ProMemberView;
            SetBindingOnProp(newItem, tag, nameof(t.Name), PrototypeTreeItemControl.NameCProperty);
            SetBindingOnProp(newItem, tag, nameof(t.DataType), PrototypeTreeItemControl.DataTypeProperty);
            SetBindingOnProp(newItem, tag, nameof(t.ModelType), PrototypeTreeItemControl.ModelTypeProperty);
            SetBindingOnProp(newItem, tag, nameof(t.StringLength), PrototypeTreeItemControl.StringLengthProperty);
            
            if (t.Members.Count > 0 && includeChildren)
                FillItems(ref newItem, tag);
            
            treeListControl.RefreshRow(newItem.RowHandle);

            return newItem;
        }

        private void FillItems(ref TreeListNode parent, OmronEthernetIPStructStringLength.ProMemberView tag)
        {
            treeListControl.BeginDataUpdate();

            if (parent == null)
            {
                if (tag != null)
                {
                    itemRoot = AddRootTreeItem(new OmronEthernetIPStructStringLength.ProMemberView(tag.Name, UFUAModel.ModelType.ObjectType));
                    parent = itemRoot;
                }
            }

            try
            {
                using (new WaitCursor())
                {
                    foreach (var member in tag.Members)
                        AddTreeItem(parent, member);
                }
            }
            finally
            {
                if (parent != null)
                    parent.IsExpanded = true;
                treeListControl.EndDataUpdate();
            }
        }

        private void treeListView_CustomColumnDisplayText(object sender, DevExpress.Xpf.Grid.TreeList.TreeListCustomColumnDisplayTextEventArgs e)
        {
            var ic = e.Node.Tag as OmronEthernetIPStructStringLength.ProMemberView;
            if (ic != null)
            {
                switch (e.Column.FieldName) {
                    case "DataType":
                    case "StringLength":
                        if (ic.ModelType == UFUAModel.ModelType.ObjectType)
                        {
                            e.DisplayText = string.Empty;
                        }
                        break;
                }
            }
        }

        private void treeListView_ValidateCell(object sender, DevExpress.Xpf.Grid.TreeList.TreeListCellValidationEventArgs e)
        {
            // e.CellValue --> Old
            // e.Value --> new value
            if (e.Column.FieldName == "StringLength") {
                if (e.Value==null || !UInt32.TryParse(e.Value.ToString(),out uint newValue))
                {
                    e.SetError(string.Format(Properties.Resources.ErrorInvalidStringLength, OmronEthernetIPProtocol.MAX_STRING_LENGTH));
                    e.IsValid = false;
                    cellEditPendingError = true;
                    return;
                }

                if (!OmronEthernetIPStructStringLength.IsStringLengthInRange(newValue)) {
                    e.SetError(string.Format(Properties.Resources.ErrorInvalidStringLength, OmronEthernetIPProtocol.MAX_STRING_LENGTH));
                    e.IsValid = false;
                    cellEditPendingError = true;
                    return;
                }                
            }
            cellEditPendingError = false;
        }


        private void treeListView_InvalidNodeException(object sender, DevExpress.Xpf.Grid.TreeList.TreeListInvalidNodeExceptionEventArgs e)
        {
            //Suppress displaying the error message box 
            e.ExceptionMode = ExceptionMode.NoAction;
        }

        // Handle request of cell's edit --> used to disable not editable fields
        private void treeListView_ShowingEditor(object sender, DevExpress.Xpf.Grid.TreeList.TreeListShowingEditorEventArgs e)
        {
            if (treeListView.FocusedNode != null)
            {
                var ic = e.Node.Tag as OmronEthernetIPStructStringLength.ProMemberView;
                if (ic != null)
                {
                    if (ic.DataType != UFUAModel.DataType.String)
                        e.Cancel = true;
                }
            }
        }

        private void treeListView_HiddenEditor(object sender, DevExpress.Xpf.Grid.TreeList.TreeListEditorEventArgs e)
        {
            treeListControl.DisableEditing();
        }

        void window_Closing(object sender, global::System.ComponentModel.CancelEventArgs e)
        {
            // cancel closing if last edited cell has invalid value
            Window window = (Window)((DependencyObject)sender);
            if (window.DialogResult == true)
                e.Cancel = cellEditPendingError;
        }
    }    
}
