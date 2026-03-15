using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Xpo;
using DevExpress.Web;
using DevExpress.Web.Data;
using DevExpress.Web.ASPxTreeList;
using System.Collections;
using System.Web.Security;
using Utilities;
using DriverSettingsInterfaces;

namespace UFUAWebEditor
{
    public partial class ASEditor : System.Web.UI.Page
    {
        private const string STR_Name = "Name";
        Session session = XpoHelper.GetNewSession();

        #region Theme
        /* Page PreInit */
        protected void Page_PreInit(object sender, EventArgs e)
        {
            string themeName = "Glass";
            if (Page.Request.Cookies[SiteMaster.GetThemeCookieName()] != null)
            {
                themeName = Page.Request.Cookies[SiteMaster.GetThemeCookieName()].Value;
            }

            string clientScriptBlock = "var DXCurrentThemeCookieName = \"" + SiteMaster.GetThemeCookieName() + "\";";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "DXCurrentThemeCookieName", clientScriptBlock, true);

            this.Theme = themeName;
        }

        #endregion

        protected void Page_Init(object sender, EventArgs e)
        {
            XpoDataSource1.Session = XpoHelper.GetNewSession();
            XpoDataSource2.Session = XpoHelper.GetNewSession();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            tree.DataBind();
            ASPxGridView1.DataBind();
            if (!IsPostBack) CustomizePopupControlAppearance();
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        { 
            
        }

        void CustomizePopupControlAppearance()
        {
            ASPxPopupControlConfirmDelete.Modal = true;

            //Make the ASPxPopupControl's elements invisible
            ASPxPopupControlConfirmDelete.BackColor = System.Drawing.Color.Transparent;
            ASPxPopupControlConfirmDelete.Border.BorderWidth = 0;
            ASPxPopupControlConfirmDelete.ContentStyle.Paddings.Padding = 0;
            ASPxPopupControlConfirmDelete.ShowHeader = false;
            ASPxPopupControlConfirmDelete.ShowSizeGrip = DevExpress.Web.ShowSizeGrip.False;
            ASPxPopupControlConfirmDelete.ShowShadow = false;

            //Specify that the ASPxPopupControl can only be invoked and closed programmatically
            ASPxPopupControlConfirmDelete.PopupAction = DevExpress.Web.PopupAction.None;
            ASPxPopupControlConfirmDelete.CloseAction = DevExpress.Web.CloseAction.None;

            //Make the ASPxPopupControl displayed centered within the ASPxGridView
            ASPxPopupControlConfirmDelete.PopupElementID = "ASPxGridView1";
            ASPxPopupControlConfirmDelete.PopupHorizontalAlign = DevExpress.Web.PopupHorizontalAlign.Center;
            ASPxPopupControlConfirmDelete.PopupVerticalAlign = DevExpress.Web.PopupVerticalAlign.Middle;
        }

        protected void ASPxPageControl1_ActiveTabChanging(object source, DevExpress.Web.TabControlCancelEventArgs e)
        {
            if (e.Tab.Name == "FlatModel")
            {
                ASPxGridViewFlat.DataBind();
            }
            else if (e.Tab.Name == "HierarchyModel")
            {
                tree.DataBind();
                ASPxGridView1.DataBind();
            }
        }

        protected void tree_VirtualModeCreateChildren(object sender, DevExpress.Web.ASPxTreeList.TreeListVirtualModeCreateChildrenEventArgs e)
        {
            if (e.NodeObject == null)
                e.Children = (from folder in new XPQuery<UFUAModel.UFUAFolder>(session)/*.AsParallel()*/
                              where folder.UFUAFolderAss == null
                              select folder).ToList();
            else
            {
                e.Children = (e.NodeObject as UFUAModel.UFUAFolder).UFUAFolders;
            }
        }

        protected void tree_VirtualModeNodeCreating(object sender, DevExpress.Web.ASPxTreeList.TreeListVirtualModeNodeCreatingEventArgs e)
        {
            UFUAModel.UFUAFolder folder = e.NodeObject as UFUAModel.UFUAFolder;
            e.NodeKeyValue = folder.Oid;

            e.SetNodeValue(STR_Name, folder.Name);
            e.IsLeaf = (folder.UFUAFolders.Count == 0);
        }

        protected void ASPxGridView1_DataBinding(object sender, EventArgs e)
        {
            ASPxGridView grid = (ASPxGridView)sender;
            if (tree.FocusedNode == null)
            {
                grid.DataSource = null;
            }
            else
            {
                var folders = (from folder in new XPQuery<UFUAModel.UFUAFolder>(session)/*.AsParallel()*/
                               where folder.Oid.ToString() == tree.FocusedNode.Key
                               select folder).ToList();
                if (folders.Count != 1)
                    grid.DataSource = null;
                else
                    grid.DataSource = folders[0].UFUATags;
            }
        }

        protected void ASPxGridViewFlat_DataBinding(object sender, EventArgs e)
        {
            ASPxGridViewFlat.DataSource = (from tag in new XPQuery<UFUAModel.UFUATag>(session)/*.AsParallel()*/ 
                                           where tag.UFUATagPrototype == null select tag).ToList();
        }

        #region Folder Editing
        protected void tree_NodeInserting(object sender, ASPxDataInsertingEventArgs e)
        {
            TreeListNode parentNode = tree.FindNodeByKeyValue(tree.NewNodeParentKey);
            EnsureNode(parentNode);

            UFUAModel.UFUAFolder newFolder = new UFUAModel.UFUAFolder(session)
            {
                Name = ReadName(e.NewValues),
                NodeId = Guid.NewGuid()
            };
            var user = Membership.GetUser();
            if (user != null)
                newFolder.User = user.UserName;

            if (!String.IsNullOrEmpty(tree.NewNodeParentKey))
            {
                var parentFolders = (from folder in new XPQuery<UFUAModel.UFUAFolder>(session)/*.AsParallel()*/
                                     where folder.Oid.ToString() == tree.NewNodeParentKey
                                     select folder).ToList();
                if (parentFolders.Count != 1)
                    throw new Exception("Node not found.");

                var folders = (from folder in parentFolders[0].UFUAFolders/*.AsParallel()*/
                               where folder.Name == newFolder.Name
                               select folder).ToList();
                if (folders.Count > 0)
                    throw new Exception("Folder exists.");
                parentFolders[0].UFUAFolders.Add(newFolder);
            }
            else
            {
                var folders = (from folder in new XPQuery<UFUAModel.UFUAFolder>(session)/*.AsParallel()*/
                               where folder.UFUAFolderAss == null && folder.Name == newFolder.Name
                                     select folder).ToList();
                if (folders.Count > 0)
                    throw new Exception("Folder exists.");
            }

            newFolder.Save();

            tree.RefreshVirtualTree(parentNode);
        }

        protected void tree_NodeUpdating(object sender, ASPxDataUpdatingEventArgs e)
        {
            string oldName = ReadName(e.OldValues);
            string newName = ReadName(e.NewValues);
            if (oldName == newName) return;
            string key = e.Keys[0].ToString();
            TreeListNode node = tree.FindNodeByKeyValue(key);
            EnsureNode(node);

            var folders = (from folder in new XPQuery<UFUAModel.UFUAFolder>(session)/*.AsParallel()*/
                           where folder.Oid.ToString() == key
                           select folder).ToList();
            if (folders.Count != 1)
                throw new Exception("Node not found.");

            if (folders[0].UFUAFolderAss == null)
            {
                var foldersFound = (from folder in new XPQuery<UFUAModel.UFUAFolder>(session)/*.AsParallel()*/
                                    where folder.UFUAFolderAss == null && folder.Name == newName && folders[0].Name != newName
                                    select folder).ToList();
                if (foldersFound.Count > 0)
                    throw new Exception("Folder exists.");
            }
            else
            {
                var foldersFound = (from folder in folders[0].UFUAFolderAss.UFUAFolders/*.AsParallel()*/
                                    where folder.Name == newName && folders[0].Name != newName
                                    select folder).ToList();
                if (foldersFound.Count > 0)
                    throw new Exception("Folder already exists.");
            }

            var user = Membership.GetUser();
            if (user != null)
                folders[0].User = user.UserName;

            folders[0].Name = newName;
            folders[0].Save();
        }

        protected void tree_NodeDeleting(object sender, ASPxDataDeletingEventArgs e)
        {
            string key = e.Keys[0].ToString();
            TreeListNode node = tree.FindNodeByKeyValue(key);
            EnsureNode(node);

            var folders = (from folder in new XPQuery<UFUAModel.UFUAFolder>(session)/*.AsParallel()*/
                           where folder.Oid.ToString() == key
                                    select folder).ToList();
            if (folders.Count != 1)
                throw new Exception("Node not found.");
            if (folders[0].UFUATags.Count > 0 || folders[0].UFUAFolders.Count > 0)
                throw new Exception("Node is Not Empty.");

            var user = Membership.GetUser();
            if (user != null)
                folders[0].User = user.UserName;
            folders[0].Save();
            folders[0].Delete();            

            tree.RefreshVirtualTree(node.ParentNode);
        }

        protected void tree_NodeValidating(object sender, TreeListNodeValidationEventArgs e)
        {
            object obj = e.NewValues[STR_Name];
            if (obj == null || !IsValidName(obj.ToString()))
                e.NodeError = "Invalid name.";
        }

        protected string GetNodeGlyph(TreeListDataCellTemplateContainer container)
        {
            string fmt = "~/Images/{0}.png";
            if (container.NodeKey == null)
                return string.Format(fmt, "closed");
            if (container.Expandable && container.Expanded)
                return string.Format(fmt, "opened");
            return string.Format(fmt, "closed");
        }
        void EnsureNode(TreeListNode node)
        {
            if (node == null)
                throw new Exception("Node not found.");
        }
        string ReadName(IDictionary values)
        {
            object obj = values[STR_Name];
            if (obj == null) return String.Empty;
            return obj.ToString().Trim();
        }
        bool IsValidName(string name)
        {
            name = name.Trim();
            return name.Length > 0 && !name.StartsWith(".") && !name.Contains("/") && !name.Contains("\\");
        }

        #endregion

        protected void ASPxGridView1_RowDeleting(object sender, ASPxDataDeletingEventArgs e)
        {
            UFUAModel.UFUATag tag = session.GetObjectByKey<UFUAModel.UFUATag>((int)e.Keys[0]);
            if (tag != null)
            {
                var user = Membership.GetUser();
                if (user != null)
                {
                    tag.User = user.UserName;
                    tag.Save();
                }
                tag.Delete();
            }
            e.Cancel = true;
        }

        protected void ASPxGridView1_RowInserting(object sender, ASPxDataInsertingEventArgs e)
        {
            var folders = (from folder in new XPQuery<UFUAModel.UFUAFolder>(session)/*.AsParallel()*/
                            where folder.Oid.ToString() == tree.FocusedNode.Key
                            select folder).ToList();
            if (folders.Count == 1)
            {
                String name = e.NewValues["Name"] as String;
                var tag = (from t in folders[0].UFUATags where t.Name == name select t).ToList();
                if (tag.Count > 0)
                    throw new Exception("Tag exists.");

                UFUAModel.UFUATag newTag = new UFUAModel.UFUATag(session)
                {
                    NodeId = Guid.NewGuid(),
                    Name = name,
                    DynamicSettings = e.NewValues["DynamicSettings"] as String,
                    DataType = (UFUAModel.DataType)e.NewValues["DataType"],
                    ModelType = (UFUAModel.ModelType)e.NewValues["ModelType"]

                    //steve 260511
                    , PrototypeModel = (((UFUAModel.ModelType)e.NewValues["ModelType"]) == UFUAModel.ModelType.ObjectType ? e.NewValues["PrototypeModel"] as string : null)    
                    //************
                };

                var user = Membership.GetUser();
                if (user != null)
                    newTag.User = user.UserName;

                folders[0].UFUATags.Add(newTag);
                newTag.Save();

                //Cancel automatic update
                e.Cancel = true;
                //back to the browse mode
                (sender as ASPxGridView).CancelEdit();
            }
        }

        protected void ASPxGridView1_RowUpdating(object sender, ASPxDataUpdatingEventArgs e)
        {
            UFUAModel.UFUATag tag = session.GetObjectByKey<UFUAModel.UFUATag>((int)e.Keys[0]);
            if (tag != null)
            {
                String name = e.NewValues["Name"] as String;
                var tagList = (from t in tag.UFUAFolder.UFUATags where t.Name == name && t.Name != tag.Name select t).ToList();
                if (tagList.Count > 0)
                    throw new Exception("Tag already exists.");

                tag.Name = name;
                tag.DataType = (UFUAModel.DataType)e.NewValues["DataType"];
                tag.ModelType = (UFUAModel.ModelType)e.NewValues["ModelType"];
                tag.DynamicSettings = e.NewValues["DynamicSettings"] as String;
                var user = Membership.GetUser();
                if (user != null)
                    tag.User = user.UserName;
                tag.Save();
            }
            //Cancel automatic update
            e.Cancel = true;
            //back to the browse mode
            (sender as ASPxGridView).CancelEdit();
        }

        protected void ASPxGridView1_RowValidating(object sender, ASPxDataValidationEventArgs e)
        {
            // Checks for null values.
            foreach (GridViewColumn column in ASPxGridView1.Columns)
            {
                GridViewDataColumn dataColumn = column as GridViewDataColumn;
                if (dataColumn == null || 
                    dataColumn.FieldName == "DynamicSettings" ||
                    dataColumn.FieldName == "Description" || 
                    dataColumn.FieldName == "InitialValue" ||
                    dataColumn.FieldName == "PrototypeModel" ||
                    dataColumn.FieldName == "HistorianSettings" ||
                    dataColumn.FieldName == "UFUAEngineeringUnit") 
                    continue;
                if (e.NewValues[dataColumn.FieldName] == null)
                    e.Errors[dataColumn] = "Value cannot be null.";
            }
            if (!IsValidName(e.NewValues["Name"].ToString()))
                AddError(e.Errors, ASPxGridView1.Columns["Name"],
                "Invalid Name.");

            //// Displays the error row if there is at least one error.
            //if (e.Errors.Count > 0) e.RowError = "Please, fill all fields.";

            //if (e.NewValues["ContactName"] != null &&
            //    e.NewValues["ContactName"].ToString().Length < 2)
            //{
            //    AddError(e.Errors, grid.Columns["ContactName"],
            //    "Contact Name must be at least two characters long.");
            //}
            //if (e.NewValues["CompanyName"] != null &&
            //e.NewValues["CompanyName"].ToString().Length < 2)
            //{
            //    AddError(e.Errors, grid.Columns["CompanyName"],
            //    "Company Name must be at least two characters long.");
            //}
            //if (string.IsNullOrEmpty(e.RowError) && e.Errors.Count > 0)
            //    e.RowError = "Please, correct all errors.";
            //foreach (var val in e.NewValues)
            //    System.Diagnostics.Debug.WriteLine("{0}", val);
            //var datatype = e.NewValues["DataType"];
            //var modeltype = e.NewValues["ModelType"];
        }

        void AddError(Dictionary<GridViewColumn, string> errors, GridViewColumn column, string errorText)
        {
            if (errors.ContainsKey(column)) return;
                errors[column] = errorText;
        }

        protected void ASPxGridView1_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            // Do not use this event to init values of a new row!
            // It can be only used to initialize visible editors on the EditForm.
            // To assign data row values, use the RowInserting event
        }

        protected void ASPxGridView1_CellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {
            ASPxComboBox comboBox = e.Editor as ASPxComboBox;
            if (comboBox != null && e.Column.FieldName == "DataType")
            {
                comboBox.DataSource = Enum.GetNames(typeof(UFUAModel.DataType));
                comboBox.DataBind();
            }
            else if (comboBox != null && e.Column.FieldName == "ModelType")
            {
                comboBox.DataSource = Enum.GetNames(typeof(UFUAModel.ModelType));
                comboBox.DataBind();
            }
            else if (comboBox != null && e.Column.FieldName == "PrototypeModel")
            {
                var list = (from p in new XPQuery<UFUAModel.UFUATagPrototype>(session)/*.AsParallel()*/
                            orderby p.Name
                            select p.Name).ToList();
                comboBox.DataSource = list;
                comboBox.DataBind();
            }
            else if (comboBox != null && e.Column.FieldName == "HistorianSettings")
            {
                var list = (from p in new XPQuery<UFUAModel.UFUAHistorianSettings>(session)/*.AsParallel()*/
                            orderby p.Name
                            select p.Name).ToList();
                comboBox.DataSource = list;
                comboBox.DataBind();
            }
            else if (comboBox != null && e.Column.FieldName == "UFUAEngineeringUnit")
            {
                var list = (from p in new XPQuery<UFUAModel.UFUAEngineeringUnit>(session)/*.AsParallel()*/
                            orderby p.Name
                            select p.Name).ToList();
                comboBox.DataSource = list;
                comboBox.DataBind();
            }
        }

        protected void ASPxGridView1_AutoFilterCellEditorInitialize(object sender, ASPxGridViewEditorEventArgs e)
        {
            ASPxGridView1_CellEditorInitialize(sender, e);
        }

        #region Prototypes Editor
        protected void ASPxGridViewPrototypeMembers_BeforePerformDataSelect(object sender, EventArgs e)
        {
            Session["UFUATagPrototypeKey"] = ((ASPxGridView)sender).GetMasterRowKeyValue(); ;
        }

        protected void ASPxGridViewPrototypeMembers_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            // Do not use this event to init values of a new row!
            // It can be only used to initialize visible editors on the EditForm.
            // To assign data row values, use the RowInserting event
        }

        protected void ASPxGridViewPrototypeMembers_RowInserting(object sender, DevExpress.Web.Data.ASPxDataInsertingEventArgs e)
        {
            e.NewValues["UFUATagPrototype!Key"] = ((ASPxGridView)sender).GetMasterRowKeyValue();
        }
        #endregion

    }
}