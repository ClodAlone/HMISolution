#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Windows.ReportDesigner.Resources;
using Syncfusion.Windows.Reports.Designer.Controls;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
using RESX = Syncfusion.Windows.Reports.Designer.Properties.Resources;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for TablixGroupingPanel.xaml
    /// </summary>
    public partial class TablixGroupingPanel : UserControl, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #region Dependency properties

        internal string VisualStyle
        {
            get
            {
                return (string)GetValue(VisualStyleProperty);
            }
            set
            {
                SetValue(VisualStyleProperty, value);
                Syncfusion.Windows.Shared.SkinStorage.SetVisualStyle(this, value);
            }
        }

        internal static readonly DependencyProperty VisualStyleProperty =
            DependencyProperty.Register("VisualStyle", typeof(string), typeof(ReportToolBox), new UIPropertyMetadata("Office2007Blue"));

        internal object SelectedReportItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        internal static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedReportItem", typeof(object), typeof(TablixGroupingPanel), new UIPropertyMetadata(null, OnSelectedItemChanged));

        internal static void OnSelectedItemChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            TablixGroupingPanel groupPanel = dependencyObject as TablixGroupingPanel;
            groupPanel.Panel = groupPanel.ReportDesignView.GetDisplayTabDesignPanel();
            groupPanel.UpdateSelectedItem(e.NewValue, true);
        }

        public ReportDesignView ReportDesignView
        {
            get { return (ReportDesignView)GetValue(ReportDesignViewProperty); }
            set { SetValue(ReportDesignViewProperty, value); }
        }

        public static readonly DependencyProperty ReportDesignViewProperty =
            DependencyProperty.Register("ReportDesignView", typeof(ReportDesignView), typeof(TablixGroupingPanel), new UIPropertyMetadata(null, OnReportDesignViewPropertyChanged));

        internal static void OnReportDesignViewPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            TablixGroupingPanel groupPanel = dependencyObject as TablixGroupingPanel;

            if (groupPanel != null)
            {
                if (e.NewValue != e.OldValue)
                {
                    if (e.OldValue != null)
                    {
                        ReportDesignView view = e.OldValue as ReportDesignView;
                        view.RemoveGrouping(groupPanel);
                    }

                    if (e.NewValue != null)
                    {
                        ReportDesignView view = e.NewValue as ReportDesignView;
                        view.AddGrouping(groupPanel);
                    }
                }
            }
        }

        #endregion
        
        #region Properties

        internal DesignPanel Panel
        {
            get;
            set;
        }

        internal TablixControl TablixControl
        {
            get;
            set;
        }

        internal RDL.DOM.Tablix TablixBase
        {
            get;
            set;
        }

        internal RDL.DOM.TablixMember TablixMember
        {
            get;
            set;
        }

        private bool m_advancemode;
        
        internal bool EnableAdvanceMode
        {
            get
            {
                return m_advancemode;
            }

            set
            {
                if (m_advancemode != value)
                {
                    this.m_advancemode = value;
                    this.PopulateTreeView();
                }
            }
        }

        bool m_itemSelected;
        internal bool IsReportItemSelected
        {
            get
            {
                return m_itemSelected;
            }
            set
            {
                m_itemSelected = value;
                this.stpnl_AdvanceMode.IsEnabled = value;
            }
        }

        internal List<TablixMemberInfo> MembersInfos
        {
            get;
            set;
        }

        internal List<TablixMemberInfo> RowMembersInfos
        {
            get;
            set;
        }

        internal List<TablixMemberInfo> ColumnMembersInfos
        {
            get;
            set;
        }

        internal List<string> MemberNameCollection
        {
            get;
            set;
        }

        internal Editors.TablixMemberProperties TablixMemberProperties
        {
            get;
            set;
        }

        private List<TablixMember> rowHierTablixMemberList;
        private List<TablixMember> colHierTablixMemberList;

        private int m_staticField;
        private int m_groupField;
        private int rowHierEnd;
        private int colHierEnd;
        private object m_borderColor;
        private object m_backGroudColor;
        private object propertyOldValue;
        private bool isRow = false;
        #endregion

        public TablixGroupingPanel()
        {
            InitializeComponent();
            UpdateCulture();
            this.Loaded += TablixGroupingPanel_Loaded;
            this.MembersInfos = new List<TablixMemberInfo>();
            this.MemberNameCollection = new List<string>();
            this.TablixMemberProperties = new Editors.TablixMemberProperties();
            this.TablixMemberProperties.PropertyChanged += new PropertyChangedEventHandler(TablixMemberProperties_PropertyChanged);
            this.TablixMemberProperties.PropertyChanging += new PropertyChangingEventHandler(TablixMemberProperties_PropertyChanging);
        }

        internal void UpdateCulture()
        {
            try
            {
                this.lbl_colgroup.Content = SR.GetString(CultureInfo.CurrentUICulture, "labelColumnGroup");
                this.lbl_rowgroup.Content = SR.GetString(CultureInfo.CurrentUICulture, "labelRowGroup");
            }
            catch { }
        }

        void TablixGroupingPanel_Loaded(object sender, RoutedEventArgs e)
        {
            m_borderColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB249"));
            m_backGroudColor = SetBackground("#FFEFFF", "#FAEBD7");
        }

        void TablixMemberProperties_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            object propertyValue = null;
            string propertyName = e.PropertyName.ToUpper();

            switch (propertyName)
            {
                case "NAME":
                    {
                        propertyValue = this.TablixMemberProperties.Name;
                        break;
                    }
                case "HIDDEN":
                    {
                        propertyValue = this.TablixMemberProperties.Hidden;
                        break;
                    }
                case "TOGGLEITEM":
                    {
                        propertyValue = this.TablixMemberProperties.ToggleItem;
                        break;
                    }
                case "DOCUMENTMAPLABEL":
                    {
                        propertyValue = this.TablixMemberProperties.DocumentMapLabel;
                        break;
                    }
                case "FIXEDDATA":
                    {
                        propertyValue = this.TablixMemberProperties.FixedData;
                        break;
                    }
                case "HIDEIFNOROWS":
                    {
                        propertyValue = this.TablixMemberProperties.HideIfNoRows;
                        break;
                    }
                case "KEEPTOGETHER":
                    {
                        propertyValue = this.TablixMemberProperties.KeepTogether;
                        break;
                    }
                case "KEEPWITHGROUP":
                    {
                        propertyValue = this.TablixMemberProperties.KeepWithGroup;
                        break;
                    }
                case "REPEATONNEWPAGE":
                    {
                        propertyValue = this.TablixMemberProperties.RepeatOnNewPage;
                        break;
                    }
                case "DATAELEMENTNAME":
                    {
                        propertyValue = this.TablixMemberProperties.DataElementName;
                        break;
                    }
                case "DATAELEMENTOUTPUT":
                    {
                        propertyValue = this.TablixMemberProperties.DataElementOutput;
                        break;
                    }
                case "GROUPDATAELEMENTNAME":
                    {
                        propertyValue = this.TablixMemberProperties.GroupDataElementName;
                        break;
                    }
                case "GROUPDATAELEMENTOUTPUT":
                    {
                        propertyValue = this.TablixMemberProperties.GroupDataElementOutput;
                        break;
                    }
                case "PAGEBREAK":
                    {
                        propertyValue = this.TablixMemberProperties.PageBreak;
                        break;
                    }
                case "PARENT":
                    {
                        propertyValue = this.TablixMemberProperties.Parent;
                        break;
                    }
                case "DOMAINSCOPE":
                    {
                        propertyValue = this.TablixMemberProperties.DomainScope;
                        break;
                    }
            }

            this.propertyOldValue = propertyValue;
        }

        void TablixMemberProperties_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            object propertyValue = null;
            string propertyName = e.PropertyName.ToUpper();

            switch (propertyName)
            {
                case "NAME":
                    {
                        propertyValue = this.TablixMemberProperties.Name;
                        if (this.TablixMember.Group != null)
                        {
                            this.TablixMember.Group.Name = this.TablixMemberProperties.Name;
                        }
                        break;
                    }
                case "HIDDEN":
                    {
                        propertyValue = this.TablixMemberProperties.Hidden;

                        if (this.TablixMember.Visibility == null && !this.TablixMemberProperties.IsInternalPropertyChange)
                        {
                            this.TablixMember.Visibility = new RDL.DOM.Visibility();
                            this.TablixMember.Visibility.Hidden = this.TablixMemberProperties.Hidden;
                        }
                        break;
                    }
                case "TOGGLEITEM":
                    {
                        propertyValue = this.TablixMemberProperties.ToggleItem;
                        if (this.TablixMember.Visibility == null && !this.TablixMemberProperties.IsInternalPropertyChange)
                        {
                            this.TablixMember.Visibility = new RDL.DOM.Visibility();
                            this.TablixMember.Visibility.ToggleItem = this.TablixMemberProperties.ToggleItem;
                        }
                        break;
                    }
                case "DOCUMENTMAPLABEL":
                    {
                        propertyValue = this.TablixMemberProperties.DocumentMapLabel;
                        if (this.TablixMember.Group != null)
                        {
                            this.TablixMember.Group.DocumentMapLabel = this.TablixMemberProperties.DocumentMapLabel;
                        }
                        break;
                    }
                case "FIXEDDATA":
                    {
                        propertyValue = this.TablixMemberProperties.FixedData;
                        this.TablixMember.FixedData = this.TablixMemberProperties.FixedData;
                        break;
                    }
                case "HIDEIFNOROWS":
                    {
                        propertyValue = this.TablixMemberProperties.HideIfNoRows;
                        this.TablixMember.HideIfNoRows = this.TablixMemberProperties.HideIfNoRows;
                        break;
                    }
                case "KEEPTOGETHER":
                    {
                        propertyValue = this.TablixMemberProperties.KeepTogether;
                        try
                        {
                            this.TablixMember.KeepTogether = Convert.ToBoolean(this.TablixMemberProperties.KeepTogether);
                        }
                        catch { }
                        break;
                    }
                case "KEEPWITHGROUP":
                    {
                        propertyValue = this.TablixMemberProperties.KeepWithGroup;
                        this.TablixMember.KeepWithGroup = (RDL.DOM.KeepWithGroup)Enum.Parse(typeof(RDL.DOM.KeepWithGroup), this.TablixMemberProperties.KeepWithGroup, true);
                        break;
                    }
                case "REPEATONNEWPAGE":
                    {
                        propertyValue = this.TablixMemberProperties.RepeatOnNewPage;
                        this.TablixMember.RepeatOnNewPage = this.TablixMemberProperties.RepeatOnNewPage;
                        break;
                    }
                case "DATAELEMENTNAME":
                    {
                        propertyValue = this.TablixMemberProperties.DataElementName;
                        this.TablixMember.DataElementName = this.TablixMemberProperties.DataElementName;
                        break;
                    }
                case "DATAELEMENTOUTPUT":
                    {
                        propertyValue = this.TablixMemberProperties.DataElementOutput;
                        this.TablixMember.DataElementOutput = (RDL.DOM.DataElementOutputs)Enum.Parse(typeof(RDL.DOM.DataElementOutputs), this.TablixMemberProperties.DataElementName, true);
                        break;
                    }
                case "GROUPDATAELEMENTNAME":
                    {
                        propertyValue = this.TablixMemberProperties.GroupDataElementName;
                        if (this.TablixMember.Group != null)
                        {
                            this.TablixMember.Group.DataElementName = this.TablixMemberProperties.GroupDataElementName;
                        }
                        break;
                    }
                case "GROUPDATAELEMENTOUTPUT":
                    {
                        propertyValue = this.TablixMemberProperties.GroupDataElementOutput;
                        break;
                    }
                case "PAGEBREAK":
                    {
                        propertyValue = this.TablixMemberProperties.PageBreak;
                        if (this.TablixMember.Group != null && !this.TablixMemberProperties.IsInternalPropertyChange)
                        {
                            if (this.TablixMember.Group.PageBreak == null)
                            {
                                this.TablixMember.Group.PageBreak = new RDL.DOM.PageBreak();
                            }

                            this.TablixMember.Group.PageBreak.BreakLocation = this.TablixMemberProperties.PageBreak;
                        }
                        break;
                    }
                case "PARENT":
                    {
                        propertyValue = this.TablixMemberProperties.Parent;
                        if (this.TablixMember.Group != null)
                        {
                            this.TablixMember.Group.Parent = this.TablixMemberProperties.Parent;
                        }
                        break;
                    }
                case "DOMAINSCOPE":
                    {
                        propertyValue = this.TablixMemberProperties.DomainScope;
                        if (this.TablixMember.Group != null)
                        {
                            this.TablixMember.Group.DomainScope = this.TablixMemberProperties.DomainScope;
                        }
                        break;
                    }
            }
            if (!this.TablixMemberProperties.IsInternalPropertyChange)
            {
                EditAction action = new EditAction();
                action.EditingType = EditActionType.ItemPropertyChanged;
                PropertyChanage change = new PropertyChanage();
                change.PropertyObject = this.TablixMemberProperties;

                change.PropertyName = e.PropertyName;
                change.OldValue = this.propertyOldValue;
                change.NewValue = propertyValue;
                action.PropertyChange = change;
                this.Panel.EditingManager.AddAction(action);
            }
        }

        internal void UpdateReportDesignerObject(ReportDesignView designview)
        {
            if (designview != null)
            {
                ReportDesignView = designview;
            }
        }

        private void UpdateSelectedItem(object selected, bool isNewItem)
        {
            try
            {
                if (selected != null)
                {
                    var tablixControl = (selected as TablixControl);

                    if (isNewItem)
                    {
                        tablixControl.ReportItemSizeChanged += TablixControl_ReportItemSizeChanged;
                    }

                    this.TablixControl = tablixControl;
                    this.TablixBase = tablixControl.tablix.TablixBase;
                    rowHierTablixMemberList = tablixControl.rowHierTablixMemberList;
                    colHierTablixMemberList = tablixControl.colHierTablixMemberList;
                    this.rowHierEnd = tablixControl.rowHierColEnd;
                    this.colHierEnd = tablixControl.colHierRowEnd;
                    PopulateTreeView();
                    this.IsReportItemSelected = true;
                }
                else
                {
                    this.ResetTreeView();
                }
            }
            catch { }
        }

        void TablixControl_ReportItemSizeChanged(object sender, EventArgs e)
        {
            if (!(sender as TablixControl).IsItemRemoved)
            {
                this.UpdateSelectedItem(sender, false);
            }
            else
            {
                this.SelectedReportItem = null;
            }
        }

        private void PopulateTreeView()
        {
            if (this.TablixBase != null)
            {
                this.MembersInfos.Clear();

                this.m_groupField = this.m_staticField = 0;

                if (this.TablixBase.TablixRowHierarchy != null && this.TablixBase.TablixRowHierarchy.TablixMembers != null)
                {
                    this.trv_RowMembersItems.Items.Clear();
                    this.PopulateTreeViewItems(this.TablixBase.TablixRowHierarchy.TablixMembers, null, true, -1, this.rowHierEnd);
                }
                if (this.TablixBase.TablixColumnHierarchy != null && this.TablixBase.TablixColumnHierarchy.TablixMembers != null)
                {
                    this.trv_ColumnMembersItems.Items.Clear();
                    int endLevel = this.TablixBase.TablixColumnHierarchy.TablixMembers.Count();
                    this.PopulateTreeViewItems(this.TablixBase.TablixColumnHierarchy.TablixMembers, null, false, -1, this.colHierEnd);
                }
            }
        }

        private void PopulateTreeViewItems(RDL.DOM.TablixMembers tablixMembers, TreeViewItem treeView, bool rowMember, int level, int endLevel)
        {
            if (tablixMembers != null)
            {
                foreach (var member in tablixMembers)
                {
                    level++;
                    TreeViewItem tree = new TreeViewItem();
                    bool addToView = false;

                    if ((member.Group != null || this.EnableAdvanceMode))
                    {
                        TablixMemberInfo info = new TablixMemberInfo() { TablixMember = member, IsRow = rowMember };
                        this.MembersInfos.Add(info);
                        addToView = true;

                        if (member.Group != null)
                        {
                            this.m_groupField++;
                            string tag = "ColumnGroup" + this.m_groupField;
                            
                            if (rowMember)
                            {
                                tag = "RowGroup" + this.m_groupField;
                            }

                            info.MemberName = tag;
                            tree.Tag = tag;
                            tree.Background = (Brush)this.m_backGroudColor;
                            tree.BorderBrush = (Brush)this.m_borderColor;
                            tree.Header = member.Group.Name;
                            
                            if (level == endLevel)
                            {
                                tree.Header = "(" + member.Group.Name + ")";
                            }
                            
                            if (!MemberNameCollection.Contains(member.Group.Name))
                            {
                                MemberNameCollection.Add(member.Group.Name);
                            }
                        }
                        else
                        {
                            this.m_staticField++;
                            string header = "static";
                            string tag = "ColumnStatic" + this.m_staticField;
                            
                            if (rowMember)
                            {
                                tag = "RowStatic" + this.m_staticField;
                            }
                            if (level == endLevel)
                            {
                                header = "(static)";
                            }

                            tree.Header = header;
                            tree.Tag = tag;
                            info.MemberName = tag;
                        }
                        
                        if (treeView != null)
                        {
                            treeView.Items.Add(tree);
                        }
                    }

                    if (member.TablixMembers != null)
                    {
                        if (addToView || this.EnableAdvanceMode)
                        {
                            PopulateTreeViewItems(member.TablixMembers, tree, rowMember, level, endLevel);
                        }
                        else
                        {
                            PopulateTreeViewItems(member.TablixMembers, treeView, rowMember, level, endLevel);
                        }
                    }
                    if (treeView == null && (addToView || tree.Items.Count > 0))
                    {
                        if (rowMember)
                        {
                            this.trv_RowMembersItems.Items.Add(tree);
                        }
                        else
                        {
                            this.trv_ColumnMembersItems.Items.Add(tree);
                        }
                    }
                    level--;
                }
            }
        }

        private void ResetTreeView()
        {
            this.trv_ColumnMembersItems.Items.Clear();
            this.trv_RowMembersItems.Items.Clear();
            this.IsReportItemSelected = false;
        }

        private LinearGradientBrush SetBackground(string color1, string color2)
        {
            LinearGradientBrush brush = new LinearGradientBrush();
            brush.StartPoint = new Point(0, 0);
            brush.EndPoint = new Point(0, 1);
            GradientStop gra1 = new GradientStop();
            gra1.Color = (Color)ColorConverter.ConvertFromString(color1);
            gra1.Offset = 1;
            GradientStop gra2 = new GradientStop();
            gra2.Color = (Color)ColorConverter.ConvertFromString(color2);
            gra2.Offset = 0;
            brush.GradientStops.Add(gra1);
            brush.GradientStops.Add(gra2);
            return brush;
        }

        private TreeViewItem ReturnMembersTreeNode(string itemName)
        {
            foreach (TreeViewItem item in this.trv_RowMembersItems.Items)
            {
                if (item.Header.ToString().ToLower() == itemName.ToString().ToLower())
                {
                    return item;
                }
            }

            foreach (TreeViewItem item in this.trv_ColumnMembersItems.Items)
            {
                if (item.Header.ToString().ToLower() == itemName.ToString().ToLower())
                {
                    return item;
                }
            }

            return null;
        }

        private void popupButton_Click(object sender, RoutedEventArgs e)
        {
            popupButton.IsChecked = false;
            popupContainer.IsOpen = true;
        }

        private void btn_AdvanceMode_Click(object sender, RoutedEventArgs e)
        {
            popupContainer.IsOpen = false;
            this.EnableAdvanceMode = !this.EnableAdvanceMode;

            if (chk_advEnable.IsChecked == true)
            {
                this.chk_advEnable.IsChecked = false;
            }
            else
            {
                this.chk_advEnable.IsChecked = true;
            }
        }

        private void SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.ReportDesignView != null && !e.Handled)
            {
                try
                {
                    this.TablixMemberProperties.IsInternalPropertyChange = true;
                    TreeViewItem treeitem = (TreeViewItem)(e.OriginalSource as TreeView).SelectedItem;
                    var tablixMember = GetTreeItemMember(treeitem);

                    if (tablixMember != null)
                    {
                        this.AddBorderToSelectedMember(tablixMember);
                        this.GetSelectedMemberProperties(tablixMember);
                        
                        foreach (var propertyGrid in this.ReportDesignView.ReportPropertyGrids)
                        {
                            propertyGrid.SelectedItem = null;
                            propertyGrid.SelectedItem = this.TablixMemberProperties;
                        }
                    }
                    this.TablixMemberProperties.IsInternalPropertyChange = false;
                    e.Handled = true;
                }
                catch { }
            }
        }

        private void AddBorderToSelectedMember(RDL.DOM.TablixMember tablixMember)
        {
        }

        private RDL.DOM.TablixMember GetTreeItemMember(TreeViewItem treeitem)
        {
            if (this.MembersInfos != null && treeitem != null)
            {
                var member = (from tree in this.MembersInfos
                              where string.Equals(tree.MemberName, treeitem.Tag)
                              select tree.TablixMember).FirstOrDefault();
                return member;
            }
            return null;
        }

        private TablixMember FindTablixMembersListInRowHierarchy(RDL.DOM.TablixMember tablixmember)
        {
            if (rowHierTablixMemberList != null && rowHierTablixMemberList.Count > 0)
            {
                var member = (from t in rowHierTablixMemberList
                                       where t.TablixMemberBase == tablixmember
                                       select t).FirstOrDefault();
                return member;
            }
            return null;
        }

        private TablixMember FindTablixMembersListInColumnHierarchy(RDL.DOM.TablixMember tablixmember)
        {
            if (colHierTablixMemberList != null && colHierTablixMemberList.Count > 0)
            {
                var member = (from t in colHierTablixMemberList
                             where t.TablixMemberBase == tablixmember
                             select t).FirstOrDefault();
                return member;
            }
            return null;
        }

        private CellContentsControl Findcell(int row, int col)
        {
            if (this.TablixControl.TablixGrid != null)
            {
                foreach (UIElement u in this.TablixControl.Children)
                {
                    if (u != null && u is CellContentsControl)
                    {
                        CellContentsControl t = u as CellContentsControl;
                        if (Grid.GetRow(t) == row && Grid.GetColumn(t) == col)
                        {
                            return t;
                        }
                    }
                }
            }

            return null;
        }
        
        private void GetSelectedMemberProperties(RDL.DOM.TablixMember tablixMember)
        {
            if (tablixMember != null)
            {
                this.TablixMember = tablixMember;
                this.TablixMemberProperties.TablixMember = tablixMember;
                this.TablixMemberProperties.DataElementName = tablixMember.DataElementName;
                this.TablixMemberProperties.DataElementOutput = tablixMember.DataElementOutput.ToString();
                this.TablixMemberProperties.FixedData = tablixMember.FixedData;
                this.TablixMemberProperties.HideIfNoRows = tablixMember.HideIfNoRows;
                this.TablixMemberProperties.KeepTogether = tablixMember.KeepTogether.ToString();
                this.TablixMemberProperties.KeepWithGroup = tablixMember.KeepWithGroup.ToString();
                this.TablixMemberProperties.RepeatOnNewPage = tablixMember.RepeatOnNewPage;

                if (tablixMember.Visibility != null)
                {
                    this.TablixMemberProperties.Hidden = tablixMember.Visibility.Hidden;
                    this.TablixMemberProperties.ToggleItem = tablixMember.Visibility.ToggleItem;
                }
                else
                {
                    this.TablixMemberProperties.Hidden = "false";
                    this.TablixMemberProperties.ToggleItem = "";
                }
                if (tablixMember.Group != null)
                {
                    this.TablixMemberProperties.HasGroup = true;
                    this.TablixMemberProperties.GroupDataElementName = tablixMember.Group.DataElementName;
                    this.TablixMemberProperties.DocumentMapLabel = tablixMember.Group.DocumentMapLabel;
                    this.TablixMemberProperties.Name = tablixMember.Group.Name;
                    this.TablixMemberProperties.Parent = tablixMember.Group.Parent;
                    this.TablixMemberProperties.DomainScope = tablixMember.Group.DomainScope;

                    if (tablixMember.Group.PageBreak != null)
                    {
                        this.TablixMemberProperties.PageBreak = tablixMember.Group.PageBreak.BreakLocation;
                    }
                    else
                    {
                        this.TablixMemberProperties.PageBreak = RDL.DOM.BreakLocation.None;
                    }
                }
                else
                {
                    this.TablixMemberProperties.HasGroup = false;
                    this.TablixMemberProperties.Name = "Tablix Member";
                }
            }
        }

        private void Tree_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeitem = e.Source as TreeViewItem;
            if (treeitem != null)
            {
                if (!(treeitem.Header.Equals("static") || treeitem.Header.Equals("(static)")))
                {
                    if (e.RightButton == MouseButtonState.Pressed)
                    {
                        this.ShowGroupContextMenu(treeitem);
                    }
                }
                else
                {
                    treeitem.ContextMenu = null;
                }
            }
        }

        private void ShowGroupContextMenu(TreeViewItem treeitem)
        {
            System.Drawing.Point point = System.Windows.Forms.Control.MousePosition;
            treeitem.ContextMenu = (ContextMenu)this.Resources["GroupContextMenu"];
        }

        void GroupProperties_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void DeleteGroup_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void AddBeforeTotalMember_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void AddAfterTotalMember_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }
        void AddParentGroup_Click(object sender, RoutedEventArgs e)
        {
            object cell = this.GetCellContent(sender);
            if (this.isRow)
            {
                this.TablixControl.RowParentGroup_Click(cell, e);
            }
            else
            {
                this.TablixControl.colParentGroup_Click(cell, e);
            }
        }

        void AddChildGroup_Click(object sender, RoutedEventArgs e)
        {
            object cell = this.GetCellContent(sender);
            if (this.isRow)
            {
                this.TablixControl.RowChildGroup_Click(cell, e);
            }
            else
            {
                this.TablixControl.ColChildGroup_Click(cell, e);
            }
        }

        private void TreeItems_PreviewDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(TreeObjectCollection)) is TreeObjectCollection)
            {
                TreeObjectCollection itemCollection = e.Data.GetData(typeof(TreeObjectCollection)) as TreeObjectCollection;
                TreeViewItemAdv treeViewItem = itemCollection[0] as TreeViewItemAdv;

                if (treeViewItem != null && treeViewItem.Tag != null && treeViewItem.Tag.GetType().Name != "DataSet" && !(treeViewItem.Tag is RDL.DOM.EmbeddedImage))
                {
                    try
                    {
                        if (e.Source.GetType().Name.Equals("TreeViewItem"))
                        {
                            TreeViewItem destNode = (TreeViewItem)e.Source;
                            if (this.FindIsDetailGroup(destNode))
                            {
                                object cell = this.GetDropedItemCellContent(destNode);
                                e.Source = "[" + treeViewItem.Header + "]";
                                if (this.isRow)
                                {
                                    this.TablixControl.RowChildGroup_Click(cell, e);
                                }
                                else
                                {
                                    this.TablixControl.ColChildGroup_Click(cell, e);
                                }
                            }
                            else
                            {
                                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxAddGroupError"), SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"),
                                     MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        
                    }
                    catch { }
                }
            }
        }

        private object GetDropedItemCellContent(TreeViewItem destNode)
        {
            RDL.DOM.TablixMember member = this.GetTreeItemMember(destNode);
            this.isRow = this.FindTreeView(destNode);
            TablixMember cellMember;
            if (this.isRow)
            {
                cellMember = FindTablixMembersListInRowHierarchy(member);
            }
            else
            {
                cellMember = FindTablixMembersListInColumnHierarchy(member);
            }
            return cellMember != null ? Findcell(cellMember.Row, cellMember.Column) : null;
        }

        private bool FindIsDetailGroup(TreeViewItem destNode)
        {
            try
            {
                string tag = destNode.Tag.ToString();
                string header = destNode.Header.ToString();

                if ((header.Equals("(static)")) || (tag.StartsWith("RowGroup") || tag.StartsWith("ColumnGroup")) && header.StartsWith("(") && header.EndsWith(")"))
                {
                    return false;
                }
            }
            catch { }
            return true;
        }

        private void TreeItems_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Link;
        }

        private void TreeItems_PreviewDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Link;
        }

        private CellContentsControl GetCellContent(object sender)
        {
            TreeViewItem treeitem = this.GetTreeItemForMenu(sender as MenuItem);
            RDL.DOM.TablixMember member = this.GetTreeItemMember(treeitem);
            this.isRow = this.FindTreeView(treeitem);
            TablixMember cellMember;
            if (this.isRow)
            {
                cellMember = FindTablixMembersListInRowHierarchy(member);
            }
            else
            {
                cellMember = FindTablixMembersListInColumnHierarchy(member);
            }

            return cellMember != null ? Findcell(cellMember.Row, cellMember.Column) : null;
        }

        private bool FindTreeView(TreeViewItem treeitem)
        {
            try
            {
                string tag = treeitem.Tag.ToString();
                if (tag.StartsWith("Row"))
                {
                    return true;
                }
            }
            catch { }

            return false;
        }

        private TreeViewItem GetTreeItemForMenu(MenuItem menuItem)
        {
            TreeViewItem treeItem = null;
            if (menuItem != null)
            {
                while (menuItem.Parent is MenuItem)
                {
                    menuItem = menuItem.Parent as MenuItem;
                }

                if (menuItem.Parent is ContextMenu)
                {
                    ContextMenu contextMenu = menuItem.Parent as ContextMenu;
                    if (contextMenu != null)
                    {
                        treeItem = contextMenu.PlacementTarget as TreeViewItem;
                    }
                }
            }
            return treeItem;
        }
    }

    internal class TablixMemberInfo
    {
        public int Level { get; set; }

        public bool IsGroup { get; set; }

        public string GroupName { get; set; }
        
        public bool IsRow { get; set; }

        public bool IsLastGroup { get; set; }

        public string MemberName { get; set; }

        public RDL.DOM.TablixMember TablixMember { get; set; }
    }
}
