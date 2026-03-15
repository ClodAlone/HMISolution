#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Tools.Olap
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using Syncfusion.Olap.Data;
    using Syncfusion.Windows.Shared;
    using System.ComponentModel;
    using Syncfusion.Olap.Reports;
    using System.Linq;
    using Syncfusion.Olap.Manager;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for CalcMemberEditor.xaml
    /// </summary>
    [DesignTimeVisible(false)]
    public partial class CalcMemberEditor : ChromelessWindow
    {
        #region Private Members
        #endregion

        #region Properties

        internal string CurrentCubeName { get; set; }

        public bool AutoExecute { get; set; }

        internal SplitButton Host { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CalcMemberEditor"/> class.
        /// </summary>
        /// <param name="dataManager">The data manager.</param>
        public CalcMemberEditor(OlapDataManager dataManager)
        {
            InitializeComponent();
            this.CaptionText.Text = this.ExpressionText.Text = this.FormatText.Text = string.Empty;
            this.CaptionText.Focus();
            if (this.CalcMeasureTreeView.OlapDataManager == null || dataManager.CurrentCubeName != this.CurrentCubeName)
            {
                this.CurrentCubeName = dataManager.CurrentCubeName;
                this.CalcMeasureTreeView.OlapDataManager = dataManager;
                if (this.CalcMeasureTreeView.Items.Count > 0)
                {
                    this.MemberTypeText.ItemsSource = (this.CalcMeasureTreeView.Items[0] as MetaTreeNode).ChildNodes.Where(i => i.NodeType == MetaTreeNodeType.Dimension);
                    this.MemberTypeText.SelectedIndex = 0;
                }
            }

            #region Wire the events

            this.CalcMeasureTreeView.MouseMove -= new System.Windows.Input.MouseEventHandler(CalcMeasureTreeView_MouseMove);
            this.CalcMeasureTreeView.MouseMove += new System.Windows.Input.MouseEventHandler(CalcMeasureTreeView_MouseMove);
            this.MouseLeftButtonUp -= new MouseButtonEventHandler(CalcMemberEditor_MouseLeftButtonUp);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(CalcMemberEditor_MouseLeftButtonUp);

            #endregion
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalcMemberEditor"/> class.
        /// </summary>
        /// <param name="host">The host as <see cref="SplitButton"/>.</param>
        public CalcMemberEditor(SplitButton host)
        {
            InitializeComponent();
            this.Host = host;
            if (this.Host != null)
            {
                if (this.Host.OlapDataManager != null && this.CalcMeasureTreeView.OlapDataManager == null || this.Host.OlapDataManager.CurrentCubeName != this.CurrentCubeName)
                {
                    this.CurrentCubeName = this.Host.OlapDataManager.CurrentCubeName;
                    this.CalcMeasureTreeView.OlapDataManager = this.Host.OlapDataManager;
                    if (this.CalcMeasureTreeView.Items.Count > 0)
                    {
                        this.MemberTypeText.ItemsSource = (this.CalcMeasureTreeView.Items[0] as MetaTreeNode).ChildNodes.Where(i => i.NodeType == MetaTreeNodeType.Dimension);
                        this.MemberTypeText.SelectedItem = this.MemberTypeText.ItemsSource.OfType<MetaTreeNode>().Where(i => i.UniqueName == this.Host.MetaTreeNode.UniqueName.Substring(0, this.Host.MetaTreeNode.UniqueName.IndexOf(']') + 1)).FirstOrDefault();
                    }
                }

                if (this.Host.MetaTreeNode != null)
                {
                    Property prop = this.Host.MetaTreeNode.Properties.FindByName(PropertyConstants.CalculatedMemberNodeName);
                    if (prop != null && prop.Value is CalculatedMember)
                    {
                        CalculatedMember calcMember = prop.Value as CalculatedMember;
                        this.CaptionText.Text = calcMember.Name ?? string.Empty;
                        this.ExpressionText.Text = calcMember.Expression ?? string.Empty;
                        this.MemberTypeBox.SelectedIndex = (calcMember.Type == TypeOfMember.Measure) ? 0 : 1;
                        if (calcMember.FormatString != null)
                        {
                            if (calcMember.FormatString.Equals("Standard", StringComparison.InvariantCultureIgnoreCase))
                            {
                                this.FormatBox.SelectedIndex = 0;
                            }
                            else if (calcMember.FormatString.Equals("Currency", StringComparison.InvariantCultureIgnoreCase))
                            {
                                this.FormatBox.SelectedIndex = 1;
                            }
                            else if (calcMember.FormatString.Equals("Percent", StringComparison.InvariantCultureIgnoreCase))
                            {
                                this.FormatBox.SelectedIndex = 2;
                            }
                            else
                            {
                                this.FormatText.Text = calcMember.FormatString;
                                this.FormatBox.SelectedIndex = 3;
                            }
                        }
                    }
                }
            }

            #region Wire the events

            this.CalcMeasureTreeView.MouseMove -= new System.Windows.Input.MouseEventHandler(CalcMeasureTreeView_MouseMove);
            this.CalcMeasureTreeView.MouseMove += new System.Windows.Input.MouseEventHandler(CalcMeasureTreeView_MouseMove);
            this.MouseLeftButtonUp -= new MouseButtonEventHandler(CalcMemberEditor_MouseLeftButtonUp);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(CalcMemberEditor_MouseLeftButtonUp);

            #endregion

        }
        #endregion

        #region Overridden Methods

        
        #endregion

        #region Actions of wired events

        private void SetVisibility(System.Windows.Visibility visibility)
        {
            this.CalcMeasureTreeViewBorder.Visibility = visibility;
            this.CaptionTextBorder.Visibility = visibility;
            this.MemberTypeBorder.Visibility = visibility;
            if (this.MemberTypeText.Visibility == System.Windows.Visibility.Visible)
                this.MemberTypeTextBorder.Visibility = visibility;
            this.FormatBoxBorder.Visibility = visibility;
            if (this.FormatText.Visibility == System.Windows.Visibility.Visible)
                this.FormatTextBorder.Visibility = visibility;
        }

        /// <summary>
        /// Handles the Click event of the OK Button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            bool canPass = true;
            try
            {
                if ((this.CalcMeasureTreeView.OlapDataManager as OlapDataManager).UseSharedDataManager)
                {
                    canPass = OkClickForActiveReport(canPass);
                }
                else
                {
                    canPass = OkClickForCurrentReport(canPass);
                }

                if (canPass)
                {
                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool OkClickForActiveReport(bool canPass)
        {
            if (this.Host != null && this.Host.Host != null)
            {
                var olapDataManager = (this.CalcMeasureTreeView.OlapDataManager as OlapDataManager);
                ////Code here for modifying the existing calculated member at the specified axis.
                if (!string.IsNullOrEmpty(this.CaptionText.Text))
                {
                    var calcMemberItem = olapDataManager.CalculatedMembers.List.Select(i => i).Where(j => (j.ElementValue is CalculatedMember) && ((j.ElementValue as CalculatedMember).UniqueName == this.Host.MetaTreeNode.UniqueName)).FirstOrDefault();
                    if (calcMemberItem != null)
                    {
                        CalculatedMember calcMember = calcMemberItem.ElementValue as CalculatedMember;
                        if (!string.IsNullOrEmpty(this.ExpressionText.Text.Trim()))
                        {
                            calcMember.Name = this.CaptionText.Text.Trim();
                            calcMember.Expression = this.ExpressionText.Text.Trim();
                        }
                        else
                        {
                            ////Show aleart message as Expression text should be required for each calculated member.
                            canPass = false;
                            MessageBox.Show("Expression text should be non-empty.", "Calculated Member Editor", MessageBoxButton.OK, MessageBoxImage.Information);
                        }

                        calcMember.FormatString = this.FormatBox.SelectedIndex == 3 ? this.FormatText.Text.Trim() : (this.FormatBox.SelectedItem as ComboBoxItem).Content.ToString();
                        if (this.MemberTypeBox.SelectedIndex == 0)
                        {
                            MeasureElement measureElement = new MeasureElement { Name = "CalculatedMeasure" };
                            calcMember.AddElement(measureElement);
                        }
                        else if (this.MemberTypeText.SelectedItem != null && this.MemberTypeBox.SelectedItem.ToString() != string.Empty)
                        {
                            DimensionElement dimensionElement = new DimensionElement { Name = this.MemberTypeText.SelectedItem.ToString() };
                            dimensionElement.AddLevel(this.MemberTypeText.SelectedItem.ToString(), this.MemberTypeText.SelectedItem.ToString());
                            calcMember.AddElement(dimensionElement);
                        }
                        else
                        {
                            ////Show alert message as Parent dimension name should be specified for calculated member.
                            canPass = false;
                            MessageBox.Show("Please select the parent dimension.", "Calculated Member Editor", MessageBoxButton.OK, MessageBoxImage.Information);
                        }

                        if (this.Host.Host.Axis == AxisPosition.Categorical)
                        {
                            var reportItem = olapDataManager.ActiveReport.CategoricalElements.List.Select(i => i).Where(j => j.ElementValue is CalculatedMember && ((j.ElementValue as CalculatedMember).UniqueName == this.Host.MetaTreeNode.UniqueName)).FirstOrDefault();
                            if (reportItem != null)
                            {
                                reportItem.ElementValue = calcMember;
                            }
                        }
                        else if (this.Host.Host.Axis == AxisPosition.Series)
                        {
                            var reportItem = olapDataManager.ActiveReport.SeriesElements.List.Select(i => i).Where(j => j.ElementValue is CalculatedMember && ((j.ElementValue as CalculatedMember).UniqueName == this.Host.MetaTreeNode.UniqueName)).FirstOrDefault();
                            if (reportItem != null)
                            {
                                reportItem.ElementValue = calcMember;
                            }
                        }
                        else if (this.Host.Host.Axis == AxisPosition.Slicer)
                        {
                            var reportItem = olapDataManager.ActiveReport.SlicerElements.List.Select(i => i).Where(j => j.ElementValue is CalculatedMember && ((j.ElementValue as CalculatedMember).UniqueName == this.Host.MetaTreeNode.UniqueName)).FirstOrDefault();
                            if (reportItem != null)
                            {
                                reportItem.ElementValue = calcMember;
                            }
                        }

                        this.Host.Text = this.Host.MetaTreeNode.Caption = this.Host.MetaTreeNode.Description = this.Host.MetaTreeNode.Name = calcMember.Name;
                        this.Host.MetaTreeNode.UniqueName = calcMember.UniqueName;
                        this.Host.MetaTreeNode.Properties.Clear();
                        this.Host.MetaTreeNode.Properties.Add(new Property(PropertyConstants.CalculatedMemberNodeName, calcMember));
                    }

                    if (canPass)
                    {
                        this.Host.Host.RefreshElementItems(this.Host.Host);
                        if ((this.CalcMeasureTreeView.OlapDataManager as OlapDataManager).UseSharedDataManager)
                        {
                            (this.CalcMeasureTreeView.OlapDataManager as OlapDataManager).NotifyActiveReportChanged();
                        }
                        else
                        {
                            if (this.AutoExecute)
                            {
                                this.CalcMeasureTreeView.OlapDataManager.NotifyElementModified(this.Host.Host.Axis);
                            }
                        }
                    }
                }
                else
                {
                    ////Show alert message as Caption text should be specified.
                    canPass = false;
                    MessageBox.Show("Caption text should be non-empty.", "Calculated Member Editor", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                var olapDataManager = (this.CalcMeasureTreeView.OlapDataManager as OlapDataManager);
                ////Code here for creating the new calculated members add these items into Categorical axis by default.
                if (!string.IsNullOrEmpty(this.CaptionText.Text) && !olapDataManager.CalculatedMembers.List.Any(i => i.ElementValue.Name == this.CaptionText.Text.Trim()))
                {
                    ////Add the calculated member into report for specified information.
                    CalculatedMember calcMember = new CalculatedMember();
                    if (!string.IsNullOrEmpty(this.ExpressionText.Text.Trim()))
                    {
                        calcMember.Name = this.CaptionText.Text.Trim();
                        calcMember.Expression = this.ExpressionText.Text.Trim();
                    }
                    else
                    {
                        ////Show aleart message as Expression text should be required for each calculated member.
                        canPass = false;
                        MessageBox.Show("Expression text should be non-empty.", "Calculated Member Editor", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    if (this.FormatBox.SelectedIndex > 0)
                    {
                        calcMember.FormatString = this.FormatBox.SelectedIndex == 3 ? this.FormatText.Text.Trim() : (this.FormatBox.SelectedItem as ComboBoxItem).Content.ToString();
                    }
                    if (this.MemberTypeBox.SelectedIndex == 0)
                    {
                        MeasureElement measureElement = new MeasureElement { Name = "CalculatedMeasure" };
                        calcMember.AddElement(measureElement);
                    }
                    else if (this.MemberTypeText.SelectedItem != null && this.MemberTypeBox.SelectedItem.ToString() != string.Empty)
                    {
                        DimensionElement dimensionElement = new DimensionElement { Name = this.MemberTypeText.SelectedItem.ToString() };
                        dimensionElement.AddLevel(this.MemberTypeText.SelectedItem.ToString(), this.MemberTypeText.SelectedItem.ToString());
                        calcMember.AddElement(dimensionElement);
                    }
                    else
                    {
                        ////Show alert message as Parent dimension name should be specified for calculated member.
                        canPass = false;
                        MessageBox.Show("Please select the parent dimension.", "Calculated Member Editor", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    if (canPass)
                    {
                        olapDataManager.CalculatedMembers.Add(calcMember);
                        if ((this.CalcMeasureTreeView.OlapDataManager as OlapDataManager).UseSharedDataManager)
                        {
                            (this.CalcMeasureTreeView.OlapDataManager as OlapDataManager).NotifyActiveReportChanged();
                        }
                        else
                        {
                            if (this.AutoExecute)
                            {
                                this.CalcMeasureTreeView.OlapDataManager.NotifyElementModified();
                            }
                        }
                    }
                }
                else
                {
                    ////Show alert window with message as either caption text is empty or already exist.
                    canPass = false;
                    MessageBox.Show("Either the caption text is empty or already exist.", "Calculated Member Editor", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            return canPass;
        }

        private bool OkClickForCurrentReport(bool canPass)
        {
            if (this.Host != null && this.Host.Host != null)
            {
                var currentReport = this.CalcMeasureTreeView.OlapDataManager.CurrentReport;
                ////Code here for modifying the existing calculated member at the specified axis.
                if (!string.IsNullOrEmpty(this.CaptionText.Text))
                {
                    var calcMemberItem = currentReport.CalculatedMembers.List.Select(i => i).Where(j => (j.ElementValue is CalculatedMember) && ((j.ElementValue as CalculatedMember).UniqueName == this.Host.MetaTreeNode.UniqueName)).FirstOrDefault();
                    if (calcMemberItem != null)
                    {
                        CalculatedMember calcMember = calcMemberItem.ElementValue as CalculatedMember;
                        if (!string.IsNullOrEmpty(this.ExpressionText.Text.Trim()))
                        {
                            calcMember.Name = this.CaptionText.Text.Trim();
                            calcMember.Expression = this.ExpressionText.Text.Trim();
                        }
                        else
                        {
                            ////Show aleart message as Expression text should be required for each calculated member.
                            canPass = false;
                            MessageBox.Show("Expression text should be non-empty.", "Calculated Member Editor", MessageBoxButton.OK, MessageBoxImage.Information);
                        }

                        calcMember.FormatString = this.FormatBox.SelectedIndex == 3 ? this.FormatText.Text.Trim() : (this.FormatBox.SelectedItem as ComboBoxItem).Content.ToString();
                        if (this.MemberTypeBox.SelectedIndex == 0)
                        {
                            MeasureElement measureElement = new MeasureElement { Name = "CalculatedMeasure" };
                            calcMember.AddElement(measureElement);
                        }
                        else if (this.MemberTypeText.SelectedItem != null && this.MemberTypeBox.SelectedItem.ToString() != string.Empty)
                        {
                            DimensionElement dimensionElement = new DimensionElement { Name = this.MemberTypeText.SelectedItem.ToString() };
                            dimensionElement.AddLevel(this.MemberTypeText.SelectedItem.ToString(), this.MemberTypeText.SelectedItem.ToString());
                            calcMember.AddElement(dimensionElement);
                        }
                        else
                        {
                            ////Show alert message as Parent dimension name should be specified for calculated member.
                            canPass = false;
                            MessageBox.Show("Please select the parent dimension.", "Calculated Member Editor", MessageBoxButton.OK, MessageBoxImage.Information);
                        }

                        if (this.Host.Host.Axis == AxisPosition.Categorical)
                        {
                            var reportItem = currentReport.CategoricalElements.List.Select(i => i).Where(j => j.ElementValue is CalculatedMember && ((j.ElementValue as CalculatedMember).UniqueName == this.Host.MetaTreeNode.UniqueName)).FirstOrDefault();
                            if (reportItem != null)
                            {
                                reportItem.ElementValue = calcMember;
                            }
                        }
                        else if (this.Host.Host.Axis == AxisPosition.Series)
                        {
                            var reportItem = currentReport.SeriesElements.List.Select(i => i).Where(j => j.ElementValue is CalculatedMember && ((j.ElementValue as CalculatedMember).UniqueName == this.Host.MetaTreeNode.UniqueName)).FirstOrDefault();
                            if (reportItem != null)
                            {
                                reportItem.ElementValue = calcMember;
                            }
                        }
                        else if (this.Host.Host.Axis == AxisPosition.Slicer)
                        {
                            var reportItem = currentReport.SlicerElements.List.Select(i => i).Where(j => j.ElementValue is CalculatedMember && ((j.ElementValue as CalculatedMember).UniqueName == this.Host.MetaTreeNode.UniqueName)).FirstOrDefault();
                            if (reportItem != null)
                            {
                                reportItem.ElementValue = calcMember;
                            }
                        }

                        this.Host.Text = this.Host.MetaTreeNode.Caption = this.Host.MetaTreeNode.Description = this.Host.MetaTreeNode.Name = calcMember.Name;
                        this.Host.MetaTreeNode.UniqueName = calcMember.UniqueName;
                        this.Host.MetaTreeNode.Properties.Clear();
                        this.Host.MetaTreeNode.Properties.Add(new Property(PropertyConstants.CalculatedMemberNodeName, calcMember));
                    }

                    if (canPass)
                    {
                        this.Host.Host.RefreshElementItems(this.Host.Host);
                        if ((this.CalcMeasureTreeView.OlapDataManager as OlapDataManager).UseSharedDataManager)
                        {
                            (this.CalcMeasureTreeView.OlapDataManager as OlapDataManager).NotifyActiveReportChanged();
                        }
                        else
                        {
                            if (this.AutoExecute)
                            {
                                this.CalcMeasureTreeView.OlapDataManager.NotifyElementModified(this.Host.Host.Axis);
                            }
                        }
                    }
                }
                else
                {
                    ////Show alert message as Caption text should be specified.
                    canPass = false;
                    MessageBox.Show("Caption text should be non-empty.", "Calculated Member Editor", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                var currentReport = this.CalcMeasureTreeView.OlapDataManager.CurrentReport;
                ////Code here for creating the new calculated members add these items into Categorical axis by default.
                if (!string.IsNullOrEmpty(this.CaptionText.Text) && !currentReport.CalculatedMembers.List.Any(i => i.ElementValue.Name == this.CaptionText.Text.Trim()))
                {
                    ////Add the calculated member into report for specified information.
                    CalculatedMember calcMember = new CalculatedMember();
                    if (!string.IsNullOrEmpty(this.ExpressionText.Text.Trim()))
                    {
                        calcMember.Name = this.CaptionText.Text.Trim();
                        calcMember.Expression = this.ExpressionText.Text.Trim();
                    }
                    else
                    {
                        ////Show aleart message as Expression text should be required for each calculated member.
                        canPass = false;
                        MessageBox.Show("Expression text should be non-empty.", "Calculated Member Editor", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    if (this.FormatBox.SelectedIndex > 0)
                    {
                        calcMember.FormatString = this.FormatBox.SelectedIndex == 3 ? this.FormatText.Text.Trim() : (this.FormatBox.SelectedItem as ComboBoxItem).Content.ToString();
                    }
                    if (this.MemberTypeBox.SelectedIndex == 0)
                    {
                        MeasureElement measureElement = new MeasureElement { Name = "CalculatedMeasure" };
                        calcMember.AddElement(measureElement);
                    }
                    else if (this.MemberTypeText.SelectedItem != null && this.MemberTypeBox.SelectedItem.ToString() != string.Empty)
                    {
                        DimensionElement dimensionElement = new DimensionElement { Name = this.MemberTypeText.SelectedItem.ToString() };
                        dimensionElement.AddLevel(this.MemberTypeText.SelectedItem.ToString(), this.MemberTypeText.SelectedItem.ToString());
                        calcMember.AddElement(dimensionElement);
                    }
                    else
                    {
                        ////Show alert message as Parent dimension name should be specified for calculated member.
                        canPass = false;
                        MessageBox.Show("Please select the parent dimension.", "Calculated Member Editor", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    if (canPass)
                    {
                        currentReport.CalculatedMembers.Add(calcMember);
                        if (currentReport.SeriesElements.List.Any(i => i.ElementValue is MeasureElements))
                        {
                            currentReport.SeriesElements.Add(calcMember);
                        }
                        else if (currentReport.SlicerElements.List.Any(i => i.ElementValue is MeasureElements))
                        {
                            currentReport.SlicerElements.Add(calcMember);
                        }
                        else
                        {
                            currentReport.CategoricalElements.Add(calcMember);
                        }

                        if ((this.CalcMeasureTreeView.OlapDataManager as OlapDataManager).UseSharedDataManager)
                        {
                            (this.CalcMeasureTreeView.OlapDataManager as OlapDataManager).NotifyActiveReportChanged();
                        }
                        else
                        {
                            if (this.AutoExecute)
                            {
                                this.CalcMeasureTreeView.OlapDataManager.NotifyElementModified();
                            }
                        }
                    }
                }
                else
                {
                    ////Show alert window with message as either caption text is empty or already exist.
                    canPass = false;
                    MessageBox.Show("Either the caption text is empty or already exist.", "Calculated Member Editor", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            return canPass;
        }

        /// <summary>
        /// Handles the Click event of the Cancel Button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void FormatBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.FormatText != null)
            {
                if (this.FormatBox.SelectedIndex == 3)
                {
                    this.FormatText.Visibility = Visibility.Visible;
                    this.FormatText.Focus();
                }
                else
                {
                    this.FormatText.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        private void MemberTypeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.MemberTypeText != null)
            {
                if (this.MemberTypeBox.SelectedIndex == 1)
                {
                    this.MemberTypeText.Visibility = Visibility.Visible;
                    this.MemberTypeText.Focus();
                }
                else
                {
                    this.MemberTypeText.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        void CalcMemberEditor_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SetVisibility(Visibility.Collapsed);
        }

        void CalcMeasureTreeView_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                TreeView treeview = (TreeView)e.Source;
                if (!(e.OriginalSource is System.Windows.Controls.Primitives.Thumb))
                {
                    if (treeview.SelectedItem is MetaTreeNode)
                        SetVisibility(Visibility.Visible);
                }
            }
        }

        #endregion

        private void ExpressionText_PreviewDrop(object sender, DragEventArgs e)
        {
            SetVisibility(Visibility.Collapsed);
            if (e.Data.GetDataPresent(typeof(MetaTreeNode)))
            {
                MetaTreeNode clipBoardNode = (MetaTreeNode)e.Data.GetData(typeof(MetaTreeNode));
                this.ExpressionText.Text += clipBoardNode.UniqueName;
                this.ExpressionText.Focus();
            }
        }

        private void ExpressionText_PreviewDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }
    }
}