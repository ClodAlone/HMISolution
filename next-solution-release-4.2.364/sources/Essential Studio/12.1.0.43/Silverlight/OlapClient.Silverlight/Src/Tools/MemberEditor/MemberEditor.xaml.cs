#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.OlapSilverlight.Manager;
using System.Collections.ObjectModel;
using Syncfusion.OlapSilverlight.Data;
using System.Windows.Controls.Primitives;
using Syncfusion.Windows.Tools.Controls;
using System.ComponentModel;
using Syncfusion.Silverlight.Client.Olap.Resources;
using System.Threading;
using Syncfusion.OlapSilverlight.Reports;

namespace Syncfusion.Silverlight.Tools.Olap
{
    /// <summary>
    /// Member editor to filter the members of a dimension.
    /// </summary>
    [DesignTimeVisible(false)]
    public partial class MemberEditor : WindowControl
    {
        #region Dependency Property

        #region AllowMultiMemberSelection
        /// <summary>
        /// Gets or sets a value indicating whether [show checkbox in MemberEditor TreeView].
        /// </summary>
        /// <value><c>true</c> if [show checkbox in MemberEditor TreeView]; otherwise, <c>false</c>.</value>

        public bool AllowMultiMemberSelection
        {
            get { return (bool)GetValue(MultiMemberSelectionProperty); }
            set { SetValue(MultiMemberSelectionProperty, value); }

        }
        internal static readonly DependencyProperty MultiMemberSelectionProperty = DependencyProperty.Register("AllowMultiMemberSelection", typeof(bool), typeof(MemberEditor), new PropertyMetadata(true, (dependencyObject, args) =>
        {
            MemberEditor memberEditor = dependencyObject as MemberEditor;
            if (memberEditor != null)
            {
                if ((bool)args.NewValue)
                {
                    memberEditor.MemberTree.ItemTemplate = memberEditor.Resources["DataTemplate"] as DataTemplate;
                    memberEditor.CheckAllNodes.Visibility = Visibility.Visible;
                }
                else
                {
                    memberEditor.MemberTree.ItemTemplate = memberEditor.Resources["WOChkDataTemplate"] as DataTemplate;
                    memberEditor.CheckAllNodes.Visibility = Visibility.Collapsed;
                    memberEditor.CheckAllNodes.IsChecked = false;
                }
            }
        }));
        #endregion
        #endregion

        #region Private Member

        private OlapDataManager _olapDataManager;

        private MemberEditorTreeViewItem _treeViewItem;

        private MetaTreeNode _tempNode;

        List<string> uniqueNames;

        private Popup _olapClientProgressPopup2;

        private BackgroundWorker _backgroundWorker;

        private MetaTreeNode dimensionNode;

        private MetaTreeNode childParentNode;

        private List<MetaTreeNode> childParentCollection = new List<MetaTreeNode>();

        private List<MetaTreeNode> getChildNodeCollection = new List<MetaTreeNode>();

        private bool isLoadingTree;

        private bool isCancelClicked;

        #endregion

        #region Public Properties

        private bool Isprocessing
        {
            get
            {
                if (this._olapClientProgressPopup2 != null)
                {
                    return this._olapClientProgressPopup2.IsOpen;
                }
                return false;
            }
            set
            {
                if (this._olapClientProgressPopup2 != null)
                {
                    this._olapClientProgressPopup2.IsOpen = value;
                    if (value == true)
                    {
                        Storyboard animation1 = _olapClientProgressPopup2.Resources["Storyboard1"] as Storyboard;
                        animation1.Begin();
                        Storyboard animation2 = _olapClientProgressPopup2.Resources["Storyboard2"] as Storyboard;
                        animation2.Begin();
                    }
                }
            }
        }

        /// <summary>d
        /// Gets or sets the OlapDataManager.
        /// </summary>
        /// <value>The olap data manager.</value>
        public OlapDataManager OlapDataManager
        {
            get
            {
                return _olapDataManager;
            }
            set
            {
                this._olapDataManager = value;

                this._olapDataManager.ChildMembersObtained += OlapDataManager_ChildMembersObtained;
                this._olapDataManager.LevelMembersObtained -= OlapDataManager_LevelMembersObtained;
                this._olapDataManager.LevelMembersObtained += OlapDataManager_LevelMembersObtained;
            }
        }

        /// <summary>
        /// Gets or sets the parent control.
        /// </summary>
        /// <value>The host.</value>
        public SplitButton Host { get; set; }

        /// <summary>
        /// Gets or sets the MetaTreeNodes.
        /// </summary>
        /// <value>The meta tree nodes.</value>
        public ObservableCollection<MetaTreeNode> MetaTreeNodes { get; set; }

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberEditor"/> class.
        /// </summary>
        /// <param name="host">The parent control(SplitButton).</param>
        /// <param name="metaTreeNodes">The MetaTreeNodes.</param>
        public MemberEditor(SplitButton host, ObservableCollection<MetaTreeNode> metaTreeNodes)
        {
            InitializeComponent();
            this.Title = SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "OlapClient_MemberEditor_Title");
            this.Host = host;

            if (metaTreeNodes.Count >= 100)
            {
                this.grid_condition.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.MetaTreeNodes = metaTreeNodes;
                this.grid_condition.Visibility = System.Windows.Visibility.Collapsed;
            }

            this.MemberTree.ItemsSource = this.MetaTreeNodes;
            this.MemberTree.MemberExpanded += MemberTree_MemberExpanded;
            this.SetCheckAllState();
            uniqueNames = new List<string>();
            this.Closed += MemberEditor_Closed;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberEditor"/> class.
        /// </summary>
        /// <param name="host">The parent control(SplitButton).</param>
        /// <param name="metaTreeNode">The MetaTreeNode.</param>
        public MemberEditor(SplitButton host, MetaTreeNode metaTreeNode)
        {
            InitializeComponent();
            this.Title = SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "OlapClient_MemberEditor_Title");
            this.Host = host;
            this.totalCount = metaTreeNode.ChildNodes.Count;
            this.MetaTreeNodes = new ObservableCollection<MetaTreeNode>();

            if (metaTreeNode.ChildNodes.Count >= 100)
            {
                this.grid_condition.Visibility = System.Windows.Visibility.Visible;
                _backgroundWorker = _backgroundWorker ?? new BackgroundWorker { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
                _backgroundWorker.DoWork += new DoWorkEventHandler(BackgroundWorker_DoWork);
                _backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(BackgroundWorker_ProgressChanged);
                _backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker_RunWorkerCompleted);
                _backgroundWorker.RunWorkerAsync(metaTreeNode.ChildNodes);
            }
            else
            {
                this.grid_condition.Visibility = System.Windows.Visibility.Collapsed;
                foreach (var metaTree in metaTreeNode.ChildNodes)
                {
                    this.MetaTreeNodes.Add(metaTree);
                }
            }

            this.MemberTree.ItemsSource = this.MetaTreeNodes;
            this.MemberTree.MemberExpanded += MemberTree_MemberExpanded;
            this.SetCheckAllState();
            uniqueNames = new List<string>();
            AxisElementBuilder aebuilder = host.Parent as AxisElementBuilder;
            Item reitem = aebuilder.ReportItems.List.Where(i => i.ElementValue.Name == metaTreeNode.Name).FirstOrDefault() as Item;
            if (aebuilder.IsSavedReport && reitem.ExcludedElementValue != null && aebuilder.OlapDataManager.ProviderName == Providers.SSAS)
            {
                DimensionElement dimel = reitem.ExcludedElementValue as DimensionElement;
                LevelElementCollection levcol = dimel.Hierarchy.LevelElements as LevelElementCollection;
                aebuilder.ExcludedMemberElements.Clear();
                foreach (LevelElement lev in levcol)
                {
                    MemberElementCollection memcol = lev.MemberElements as MemberElementCollection;
                    foreach (MemberElement mem in memcol)
                    {
                        aebuilder.ExcludedMemberElements.Add(mem);
                    }
                }
                LoadTree(metaTreeNode, reitem);
                isLoadingTree = true;
            }
            this.Opened += new RoutedEventHandler(MemberEditor_Opened);
            this.Closed += MemberEditor_Closed;
        }

        public static MetaTreeNodeCollection GetChildNodes(MetaTreeNode childNode, MetaTreeNodeCollection mtNodeCollection)
        {
            if (childNode.ChildNodes != null && childNode.ChildNodes.Count != 0 && childNode.ChildNodes[0].Caption != "(Blank)")
            {
                foreach (MetaTreeNode child in childNode.ChildNodes)
                {
                    if (child.Caption != "(Blank)")
                    {
                        mtNodeCollection.Add(child);
                        GetChildNodes(child, mtNodeCollection);
                    }
                }
            }
            return mtNodeCollection;
        }

        public static List<MemberEditorTreeViewItem> GetTreeViewItem(MemberEditorTreeViewItem parentItem, List<MemberEditorTreeViewItem> treeviewItemCollection)
        {
            if (parentItem.Items != null)
            {
                if(parentItem.Items.Count !=0 && parentItem.Items[0].ToString() != "(Blank)")
                {
                    for (int i = 0; i < parentItem.Items.Count; i++)
                    {
                        MemberEditorTreeViewItem treItem = parentItem.ItemContainerGenerator.ContainerFromIndex(i) as MemberEditorTreeViewItem;
                        if (treItem != null)
                        {
                            if (treItem.ToString() != "(Blank)")
                            {
                                treeviewItemCollection.Add(treItem);
                                GetTreeViewItem(treItem, treeviewItemCollection);
                            }
                        }
                    }
                }
            }
            return treeviewItemCollection;
        }

        private void LoadTree(MetaTreeNode parentparent, Item reportItem)
        {
            DependencyObject parelement = VisualTreeHelper.GetParent(this.Host.Parent.Parent);

            while (parelement != null && !(parelement is Syncfusion.Silverlight.Client.Olap.OlapClient))
            {
                parelement = VisualTreeHelper.GetParent(parelement);
            }

            Syncfusion.Silverlight.Client.Olap.OlapClient olapClient = parelement as Syncfusion.Silverlight.Client.Olap.OlapClient;

            this.dimensionNode = parentparent;

            MemberElementCollection reportElements = new MemberElementCollection();

            MemberElementCollection parentElements = new MemberElementCollection();

            MemberElementCollection getchildElements = new MemberElementCollection();

            DimensionElement element = reportItem.ElementValue as DimensionElement;

            MetaTreeNodeCollection nodeCollection = new MetaTreeNodeCollection(parentparent);

            if (element != null)
            {
                LevelElementCollection levelElementCollection = element.Hierarchy.LevelElements;
                foreach (LevelElement levlElmt in levelElementCollection)
                {
                    MemberElementCollection membElements = levlElmt.MemberElements;
                    foreach (MemberElement memElement in membElements)
                    {
                        reportElements.Add(memElement);
                        GetReportMemberElements(memElement, reportElements);
                    }

                }

                if (reportElements != null && reportElements.Count != 0)
                {

                    foreach (MemberElement memElemt in reportElements)
                    {
                        if (memElemt.ChildMemberElements.Count != 0 && memElemt.ChildMemberElements != null)
                        {
                            parentElements.Add(memElemt);
                        }
                    }
                }

                if (parentparent.ChildNodes != null && parentparent.ChildNodes.Count != 0 && parentparent.ChildNodes[0].Caption != "(Blank)")
                {
                    foreach (MetaTreeNode mtnode in parentparent.ChildNodes)
                    {
                        nodeCollection.Add(mtnode);
                        GetChildNodes(mtnode, nodeCollection);
                    }
                }

                if (parentElements != null && parentElements.Count != 0)
                {
                    foreach (MemberElement elemt in parentElements)
                    {
                        MetaTreeNode getchidNode = nodeCollection.Where(i => i.UniqueName == elemt.UniqueName).FirstOrDefault() as MetaTreeNode;
                        if (getchidNode != null && getchidNode.ChildNodes.Count <= 1)
                        {
                            if (getchidNode.ChildNodes.Count == 1 && getchidNode.ChildNodes[0].Caption == "(Blank)")
                            {
                                getChildNodeCollection.Add(getchidNode);
                                childParentCollection.Add(getchidNode);
                                string query = "SELECT ADDCALCULATEDMEMBERS({" + getchidNode.UniqueName + ".CHILDREN}) DIMENSION PROPERTIES MEMBER_NAME, MEMBER_TYPE ON 0, {} ON 1 FROM [" + olapClient.OlapDataManager.CurrentCubeName + "]";
                                olapClient.OlapDataManager.GetChildrenByMDX(query);
                                childParentNode = getchidNode;
                            }
                        }
                    }
                }
            }
            if (parentElements.Count == getChildNodeCollection.Count)
            {
                foreach (MetaTreeNode node in getChildNodeCollection)
                {
                    if (node.NodeCheckedType == MetaTreeNodeCheckedType.CurrentChecked && node.ChildNodes[0].Caption != "(Blank)")
                    {
                        node.IsSelected = null;
                        node.NodeCheckedType = MetaTreeNodeCheckedType.SomeChildChecked;
                        node.CheckedState = 1;
                    }
                }
                getChildNodeCollection.Clear();
            }

            List<MetaTreeNode> parentNodeCollection = new List<MetaTreeNode>();
            int loadedCount = 0;

            foreach (MemberElement membr in parentElements)
            {
                MetaTreeNode parentNode = nodeCollection.Where(i => i.UniqueName == membr.UniqueName).FirstOrDefault();
                if (parentNode != null)
                    parentNodeCollection.Add(parentNode);

            }

            foreach (MetaTreeNode parentNode in parentNodeCollection)
            {
                if (parentNode.ChildNodes.Count > 0 && parentNode.ChildNodes[0].Caption != "(Blank)")
                    loadedCount++;
            }
            if (loadedCount == parentElements.Count)
            {
                if (this.Isprocessing)
                    this.Isprocessing = false;
                else
                    this.isCancelClicked = true;
            }
        }

        void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.statusText.Text = e.ProgressPercentage.ToString(System.Globalization.CultureInfo.CurrentUICulture) + SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "OlapClient_MemberEditor_PercenageOfMemberLoaded");
        }

        void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.grid_condition.Visibility = System.Windows.Visibility.Collapsed;
            Isprocessing = false;
            totalCount = startIndex = 0;
            _backgroundWorker.DoWork -= new DoWorkEventHandler(BackgroundWorker_DoWork);
            _backgroundWorker.ProgressChanged -= new ProgressChangedEventHandler(BackgroundWorker_ProgressChanged);
            _backgroundWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(BackgroundWorker_RunWorkerCompleted);
            _backgroundWorker.CancelAsync();
            _backgroundWorker = null;
        }

        const int recordsPerWork = 100;
        const int totalPercentReach = 100;
        int startIndex, totalCount;
        void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is MetaTreeNodeCollection && startIndex < totalCount && !e.Cancel)
            {
                Thread.Sleep(1000);
                this.Dispatcher.BeginInvoke(delegate
                {
                    if (startIndex < totalCount)
                    {
                        var smallCollection = (e.Argument as MetaTreeNodeCollection).Skip(startIndex).Take(recordsPerWork);
                        startIndex += smallCollection.Count();
                        foreach (var item in smallCollection)
                        {
                            this.MetaTreeNodes.Add(item);
                        }
                        int percentReached = (int)(((float)startIndex / (float)totalCount) * 100);
                        if (percentReached < totalPercentReach)
                        {
                            _backgroundWorker.ReportProgress(percentReached);
                        }
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                });
                BackgroundWorker_DoWork(sender, e);
            }
        }

        void MemberEditor_Opened(object sender, RoutedEventArgs e)
        {
            this._olapClientProgressPopup2 = this.PART_ProgressPopup;
            if (this.MetaTreeNodes.Count == 0 || isLoadingTree)
            {
                if (!isCancelClicked)
                    this.Isprocessing = true;
            }
        }

        #endregion

        #region Events

        #region OnApplyTemplate

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            (this.GetTemplateChild("PART_MinimizeButton") as Button).Visibility = System.Windows.Visibility.Collapsed;
            (this.GetTemplateChild("PART_RestoreButton") as Button).Visibility = System.Windows.Visibility.Collapsed;
            (this.GetTemplateChild("PART_MaximizeButton") as Button).Visibility = System.Windows.Visibility.Collapsed;
            (this.GetTemplateChild("PART_Resizegrip") as Border).Visibility = System.Windows.Visibility.Collapsed;
            if (this._olapClientProgressPopup2 != null)
                this._olapClientProgressPopup2 = this.PART_ProgressPopup;
        }

        #endregion

        #region Button event handler

        /// <summary>
        /// Handles the Click event of the OKButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.MetaTreeNodes.Count > 0)
                {
                    //this.MetaTreeNodes[0].AcceptIsSelectedChanges(true);
                    foreach (MetaTreeNode metaTreeNode in this.MetaTreeNodes)
                    {
                        metaTreeNode.AcceptIsSelectedChanges(true);
                    }
                }
                if(this.MetaTreeNodes.Count>0)
                this.Host.Parent.UpdateChild(this.Host.Parent);
                this.Close();
            }
            catch (Exception ex)
            {
                WindowControl.ShowAlert(ex.Message, SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "OlapClient_Errors_ErrorWhileUpdating"), DialogIcon.Error, DialogButton.OK, null, Windows.Tools.Controls.AnimationType.Zoom);
            }
        }

        /// <summary>
        /// Handles the Click event of the CancelButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Unwire event

        void MemberEditor_Closed(object sender, ClosedEventArgs e)
        {
            this._olapDataManager.ChildMembersObtained -= OlapDataManager_ChildMembersObtained;
            this.Closed -= MemberEditor_Closed;
        }

        #endregion

        #region Expand event handler

        public MetaTreeNode GetRootNode(MetaTreeNode node)
        {
            if (node.ParentNode != null)
            {
                return node.ParentNode.GetRootNode();
            }
            else
            {
                return node;
            }
        }

        public MemberElementCollection GetReportMemberElements(MemberElement memElement, MemberElementCollection reportElementCollection)
        {
           
            if (memElement.ChildMemberElements.Count > 0)
            {
                foreach (MemberElement membElement in memElement.ChildMemberElements)
                {
                    reportElementCollection.Add(membElement);
                    GetReportMemberElements(membElement, reportElementCollection);
                }
            }
            return reportElementCollection;
        }

        /// <summary>
        /// Handles the MemberExpanded event of the MemberTreeView Item.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void MemberTree_MemberExpanded(object sender, RoutedEventArgs e)
        {
            this._treeViewItem = (sender as MemberEditorTreeViewItem);
            this._tempNode = (MetaTreeNode)(sender as MemberEditorTreeViewItem).DataContext;
            MetaTreeNode dimensionNode = GetRootNode(_tempNode);
            AxisElementBuilder axisElemntBuldr = dimensionNode.Properties.Where(i => i.Name == "AxisElements").FirstOrDefault().Value as AxisElementBuilder;
            this.Isprocessing = true;

            //// Get the count of nodes except the blank node
            int count = this._tempNode.ChildNodes.Count(child => !string.IsNullOrEmpty(child.Caption) && !string.IsNullOrEmpty(child.UniqueName));

            if (count == 0)
            {
                if (this.OlapDataManager.ProviderName == Providers.ActivePivot || this.OlapDataManager.ProviderName == Providers.Mondrian)
                {
                    //string[] tempNames = this._tempNode.UniqueName.Split('.');
                    List<string> tempNames = new List<string>();
                    System.Text.RegularExpressions.Regex regEx = new System.Text.RegularExpressions.Regex(@"\[(.*?)\]");
                    string[] uniqueNames = regEx.Split(this._tempNode.UniqueName);
                    foreach (var item in uniqueNames)
                    {
                        if (!string.IsNullOrEmpty(item) && !item.Equals("."))
                        {
                            tempNames.Add(GetUniqueName(item));
                        }
                    }
                    string tempLevelObjName = string.Empty;
                    if (this._tempNode.Properties.Count > 0)
                    {
                        if (this._tempNode.Properties.FindByName(PropertyConstants.Member).Value is Member)
                        {
                            tempLevelObjName = (this._tempNode.Properties.FindByName(PropertyConstants.Member).Value as Member).LevelUniqueName;
                        }

                        for (int j = 1; j < tempNames.Count; j++)
                        {
                            tempLevelObjName += "." + tempNames[j];
                        }
                        this.OlapDataManager.GetChildMembers(tempLevelObjName, this.OlapDataManager.CurrentCubeName);
                    }
                }
                else
                {

                    string mdxQuery = "SELECT ADDCALCULATEDMEMBERS({" + this._tempNode.UniqueName + ".CHILDREN}) DIMENSION PROPERTIES MEMBER_NAME, MEMBER_TYPE ON 0, {} ON 1 FROM [" + this.OlapDataManager.CurrentCubeName + "]";
                    this.OlapDataManager.GetChildrenByMDX(mdxQuery);
                    //this.OlapDataManager.GetChildMembers(this._tempNode.UniqueName, this.OlapDataManager.CurrentCubeName);
                }

                this._treeViewItem.IsExpanded = false;
            }
            else
            {
                foreach (var item in this._tempNode.ChildNodes)
                {
                    if (this.Host.Parent.ExcludedMemberElements != null)
                    {
                        if (_tempNode.NodeCheckedType == MetaTreeNodeCheckedType.CurrentChecked)
                        {
                            this.Host.Parent.ExcludedMemberElements.Remove(this.Host.Parent.ExcludedMemberElements.Where(i => i.UniqueName == item.UniqueName).FirstOrDefault());
                            item.IsSelected = true;
                            item.NodeCheckedType = MetaTreeNodeCheckedType.CurrentChecked;
                        }
                        else if (_tempNode.NodeCheckedType == MetaTreeNodeCheckedType.NoneSelected)
                        {
                            item.IsSelected = false;
                            item.NodeCheckedType = MetaTreeNodeCheckedType.NoneSelected;
                        }
                        else
                        {
                            Syncfusion.OlapSilverlight.Reports.MemberElement memberElement = this.Host.Parent.ExcludedMemberElements.Where(i => i.UniqueName == item.UniqueName).FirstOrDefault();
                            if (memberElement != null)
                            {
                                item.IsSelected = false;
                                this._tempNode.NodeCheckedType = MetaTreeNodeCheckedType.SomeChildChecked;
                                this._tempNode.IsSelected = null;
                            }
                        }
                    }
                }
                this.Isprocessing = false;
            }
        }

        private string GetUniqueName(string stringWithoutBrace)
        {
            return string.Format("[{0}]", stringWithoutBrace);
        }

        List<string> uniqNames = new List<string>();

        /// <summary>
        /// Handles the ChildMembersObtained event of the OlapDataManager.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Syncfusion.OlapSilverlight.Manager.ChildMembersObtaindedEventArgs"/> instance containing the event data.</param>
        void OlapDataManager_ChildMembersObtained(object sender, ChildMembersObtaindedEventArgs e)
        {
            if (this._treeViewItem != null)
            {
                this._tempNode = this._treeViewItem.DataContext as MetaTreeNode;
                this._tempNode.ChildNodes.Clear();
                this.GetUniqueNames();
                //this.uniqueNames.Add(this._tempNode.UniqueName);
                Member parentMember = null;
                if (this.OlapDataManager.ProviderName == Providers.ActivePivot || this.OlapDataManager.ProviderName == Providers.Mondrian)
                {
                    parentMember = this.OlapDataManager.CurrentCubeSchema.GetMemberByUniqueNameForNonSSAS(this.uniqueNames);
                }
                else
                {
                    parentMember = this.OlapDataManager.CurrentCubeSchema.GetMemberByUniqueName(this.uniqueNames);
                }

                if (parentMember != null)
                {
                    parentMember.ChildMembers = e.ChildMembers;
                }

                //// Adding the obtained child members to the treeview item.
                foreach (Member memberObj in e.ChildMembers)
                {
                    MetaTreeNode childNode = new MetaTreeNode();
                    MetaTreeHelper.FillMetaTreeNode(childNode, memberObj, true, true);
                    if (this.Host.Parent.ExcludedMemberElements != null)
                    {
                        Syncfusion.OlapSilverlight.Reports.MemberElement memberElement = this.Host.Parent.ExcludedMemberElements.Where(i => i.UniqueName == memberObj.UniqueName).FirstOrDefault();
                        if (memberElement != null)
                        {
                            childNode.IsSelected = false;
                            this._tempNode.NodeCheckedType = MetaTreeNodeCheckedType.SomeChildChecked;
                            this._tempNode.IsSelected = null;
                        }
                    }

                    if (this._tempNode.IsSelected == false)
                    {
                        childNode.IsSelected = false;
                    }
                    this._tempNode.ChildNodes.Add(childNode);
                }

                //// Expanding the treeview item after adding the child nodes
                this._treeViewItem.IsExpanded = true;

                //// Because of clearing the ChilNodes the expander button is getting collapsed to make it visible, get the expander by visualtree and chaning its visbility
                ToggleButton expander = (ToggleButton)(VisualTreeHelper.GetChild(this._treeViewItem as DependencyObject, 0) as System.Windows.Controls.Grid).Children[0];
                if (expander != null && expander.Visibility == Visibility.Collapsed)
                {
                    expander.Visibility = Visibility.Visible;
                }
            }
            else
            {
                MemberEditorTreeViewItem treeview = new MemberEditorTreeViewItem();
                List<MetaTreeNode> mtParentNodeCollection = new List<MetaTreeNode>();
                MetaTreeNodeCollection nodeCollection = new MetaTreeNodeCollection(dimensionNode);
                bool isparent = false;
                if (childParentCollection.Count > 0)
                {
                    foreach (MetaTreeNode node in childParentCollection)
                    {
                        string[] uniqpar = node.UniqueName.Split('.');
                        string[] unichi = e.ChildMembers[0].UniqueName.Split('.');
                        if (uniqpar[0] + uniqpar[1] == unichi[0] + unichi[1])
                            mtParentNodeCollection.Add(node);

                    }
                }
                
                if (mtParentNodeCollection != null)
                {
                    nodeCollection.Clear();
                    if (dimensionNode.ChildNodes != null && dimensionNode.ChildNodes.Count != 0 && dimensionNode.ChildNodes[0].Caption != "(Blank)")
                    {
                        foreach (MetaTreeNode mtnode in dimensionNode.ChildNodes)
                        {
                            nodeCollection.Add(mtnode);
                            GetChildNodes(mtnode, nodeCollection);
                        }
                    }

                    foreach (MetaTreeNode node in nodeCollection)
                    {
                        if (node.ChildNodes.Count > 0)
                        {
                            if (node.NodeCheckedType == MetaTreeNodeCheckedType.CurrentChecked && node.ChildNodes[0].Caption != "(Blank)")
                            {
                                if (node.ChildNodes[0].ChildNodes.Count != 0)
                                {
                                    node.IsSelected = null;
                                    node.NodeCheckedType = MetaTreeNodeCheckedType.SomeChildChecked;
                                    node.CheckedState = 1;
                                }
                            }
                        }
                    }

                    foreach (MetaTreeNode mtParentNode in mtParentNodeCollection)
                    {
                        MetaTreeNode mtnd = nodeCollection.Where(i => i.UniqueName == e.ChildMembers[0].UniqueName).FirstOrDefault();

                        if (mtParentNode != null && mtParentNode.ChildNodes.Count <= 1 && mtParentNode.Caption != "(Blank)" && mtnd == null&& mtParentNode.UniqueName == e.ChildMembers[0].ParentUniqueName)
                        {
                            uniqNames.Add(mtParentNode.UniqueName);
                            Member parentMember = null;
                            if (this.OlapDataManager.ProviderName == Providers.ActivePivot || this.OlapDataManager.ProviderName == Providers.Mondrian)
                            {
                                parentMember = this.OlapDataManager.CurrentCubeSchema.GetMemberByUniqueNameForNonSSAS(this.uniqNames);
                            }
                            else
                            {
                                parentMember = this.OlapDataManager.CurrentCubeSchema.GetMemberByUniqueName(this.uniqNames);
                            }

                            if (parentMember != null)
                            {
                                if (parentMember.ChildMembers.Count == 0)
                                    parentMember.ChildMembers = e.ChildMembers;
                            }
                            foreach (Member mem in e.ChildMembers)
                            {
                                if (mem.UniqueName == mtParentNode.UniqueName)
                                    isparent = true;
                            }
                            if (mtParentNode.ChildNodes[0].Caption == "(Blank)")
                            {
                                AxisElementBuilder aebuilder = this.Host.Parent;
                                Item reitem = aebuilder.ReportItems.List.Where(i => i.ElementValue.Name == dimensionNode.Name).FirstOrDefault() as Item;
                                DimensionElement element = reitem.ElementValue as DimensionElement;
                                MemberElementCollection reportElements = new MemberElementCollection();
                                MemberElementCollection parentElements = new MemberElementCollection();
                                mtParentNode.ChildNodes.Clear();
                                if (e.ChildMembers.Count <= 0)
                                    return;
                                MemberCollection childMemberCollection = e.ChildMembers;
                                if (true)
                                {
                                    if (!isparent)
                                    {
                                        foreach (Member memObj in e.ChildMembers)
                                        {
                                            MetaTreeNode childNode = new MetaTreeNode();
                                            MetaTreeHelper.FillMetaTreeNode(childNode, memObj, true, true);
                                            MemberElement member = null;
                                            if (aebuilder.ExcludedMemberElements != null)
                                                member = aebuilder.ExcludedMemberElements.Where(i => i.UniqueName == childNode.UniqueName).FirstOrDefault() as MemberElement;
                                            if (member != null)
                                            {
                                                childNode.IsSelected = false;
                                                childNode.NodeCheckedType = MetaTreeNodeCheckedType.NoneSelected;
                                                childNode.CheckedState = 3;
                                                mtParentNode.IsSelected = null;
                                                mtParentNode.NodeCheckedType = MetaTreeNodeCheckedType.SomeChildChecked;
                                                mtParentNode.CheckedState = 1;
                                            }
                                            mtParentNode.ChildNodes.Add(childNode);
                                        }
                                    }
                                    List<MemberEditorTreeViewItem> treeviewitemColle = new List<MemberEditorTreeViewItem>();
                                    for (int i = 0; i < this.MemberTree.Items.Count; i++)
                                    {
                                        MemberEditorTreeViewItem treeviewItem= this.MemberTree.ItemContainerGenerator.ContainerFromIndex(i) as MemberEditorTreeViewItem;
                                        treeviewitemColle.Add(treeviewItem);
                                        GetTreeViewItem(treeviewItem, treeviewitemColle);
                                    }
                                    if (treeviewitemColle != null)
                                    {
                                        if (treeviewitemColle.Count != 0)
                                        {
                                            foreach (MemberEditorTreeViewItem item in treeviewitemColle)
                                            {
                                                MetaTreeNode node = item.Header as MetaTreeNode;
                                                if (node.UniqueName == mtParentNode.UniqueName)
                                                {
                                                    ToggleButton expander = (ToggleButton)(VisualTreeHelper.GetChild(item as DependencyObject, 0) as System.Windows.Controls.Grid).Children[0];
                                                    if (expander != null && expander.Visibility == Visibility.Collapsed)
                                                    {
                                                        expander.Visibility = Visibility.Visible;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    childParentCollection.Remove(childParentCollection.Where(i => i.UniqueName == mtParentNode.UniqueName).FirstOrDefault());
                                }
                                LoadTree(dimensionNode, reitem);
                            }
                        }
                    }
                }
            }
        }


        void OlapDataManager_LevelMembersObtained(object sender, LevelMembersObtainedEventArgs e)
        {
            if (this.MetaTreeNodes.Count == 0)
            {
                MemberCollection leveMembers = e.LevelMembers;
                string dimnesionName = leveMembers[0].UniqueName.Substring(1, leveMembers[0].UniqueName.IndexOf("]") - 1);
                AxisElementBuilder axis = this.Host.Parent.FinDimensionAxis(dimnesionName);
                MetaTreeNode dimensionNode = this.Host.Parent.FindDimensionNode(axis.MetaTreeNodes, dimnesionName);

                if (dimensionNode.Name == this.Host.MetaTreeNode.Name)
                {
                    this.Host.Parent.OlapDataManager_LevelElementObtained(sender, e);
                    foreach (var metaTree in this.Host.MetaTreeNode.ChildNodes)
                    {
                        this.MetaTreeNodes.Add(metaTree);
                    }
                    this.MemberTree.ItemsSource = this.MetaTreeNodes;
                    this.Isprocessing = false;
                }
            }
        }

        private void GetUniqueNames()
        {
            this.uniqueNames.Clear();
            MetaTreeNode node = this._tempNode;
            if (this.OlapDataManager.ProviderName == Providers.ActivePivot || this.OlapDataManager.ProviderName == Providers.Mondrian)
            {
                while (node != null)
                {
                    string[] tempNames = node.UniqueName.Split('.');
                    string tempLevelObjName = string.Empty;
                    if (node.NodeType == MetaTreeNodeType.Member && node.Properties.Count > 0)
                    {
                        if (node.Properties.FindByName(PropertyConstants.Member).Value is Member)
                        {
                            tempLevelObjName = (node.Properties.FindByName(PropertyConstants.Member).Value as Member).LevelUniqueName;
                        }

                        for (int j = 1; j < tempNames.Length; j++)
                        {
                            tempLevelObjName += "." + tempNames[j];
                        }
                        this.uniqueNames.Insert(0, tempLevelObjName);
                    }
                    else
                    {
                        this.uniqueNames.Insert(0, node.UniqueName);
                    }
                    node = node.ParentNode;
                }
                this.uniqueNames.RemoveAt(0);
            }
            else
            {
                while (node != null)
                {
                    this.uniqueNames.Insert(0, node.UniqueName);
                    node = node.ParentNode;
                }
                this.uniqueNames.RemoveAt(0);
            }

        }

        #endregion

        #region Selection change handler

        void CheckAllNodes_Click(object sender, RoutedEventArgs e)
        {
            foreach (MetaTreeNode metaTree in this.MetaTreeNodes)
            {
                metaTree.IsSelected = (sender as CheckBox).IsChecked;
            }
        }

        #endregion

        #region Check/Unchek all state handler

        /// <summary>
        /// Handles the Click event of the CheckBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            SetCheckAllState();
        }

        /// <summary>
        /// Sets the state of the check/uncheck all node.
        /// </summary>
        private void SetCheckAllState()
        {
            int checkCount = this.MetaTreeNodes.Count(child => child.IsSelected == true);
            int unCheckCount = this.MetaTreeNodes.Count(child => child.IsSelected == false);

            if (checkCount == this.MetaTreeNodes.Count)
            {
                this.CheckAllNodes.IsChecked = true;
            }
            else if (unCheckCount == this.MetaTreeNodes.Count)
            {
                this.CheckAllNodes.IsChecked = false;
            }
            else
            {
                this.CheckAllNodes.IsChecked = null;
            }
        }

        #endregion

        #endregion
    }

    #region MemberEditorTreeView

    /// <summary>
    /// Tree View for Memeber Editor
    /// </summary>
    [DesignTimeVisible(false)]
    public class MemberEditorTreeView : TreeView
    {
        /// <summary>
        /// To tag the Expanded event of MemberEditorTreeViewItem with MemberTreeView.
        /// </summary>
        /// <returns></returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            MemberEditorTreeViewItem treeViewItem = new MemberEditorTreeViewItem();
            treeViewItem.Expanded += MemberExpanded;
            treeViewItem.MemberExpanded += MemberExpanded;
            return treeViewItem;
        }

        public event RoutedEventHandler MemberExpanded;
    }

    /// <summary>
    /// Tree View Item for Member Tree
    /// </summary>
    [DesignTimeVisible(false)]
    public class MemberEditorTreeViewItem : TreeViewItem
    {
        /// <summary>
        /// To tag the Expanded event for all the child nodes
        /// </summary>
        /// <returns></returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            MemberEditorTreeViewItem treeViewItem = new MemberEditorTreeViewItem();
            treeViewItem.Expanded += MemberExpanded;
            treeViewItem.MemberExpanded += MemberExpanded;
            return treeViewItem;
        }

        /// <summary>
        /// Occurs when [member expanded].
        /// </summary>
        public event RoutedEventHandler MemberExpanded;
    }

    #endregion
}

