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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Media;
    using Syncfusion.Olap.Data;
    using Syncfusion.Windows.Shared;
    using System.Linq;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for MeasureEditor.xaml
    /// </summary>
    public partial class MeasureEditor : ChromelessWindow
    {
        #region Initilize/Finilaize

        /// <summary>
        /// Initializes a new instance of the <see cref="MeasureEditor"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        public MeasureEditor(SplitButton host, ObservableCollection<MetaTreeNode> metaTreeNodes)
        {
            InitializeComponent();
            this.Host = host;
            this.HostParent = host.Host;
            this.MetaTreeNodes = metaTreeNodes;
            this.listBoxMeasureElements.ItemsSource = this.MetaTreeNodes;
            this.DataContext = this;
            this.Loaded += new RoutedEventHandler(MeasureEditor_Loaded);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        /// <value>The host.</value>
        public SplitButton Host { get; set; }

        public AxisElementBuilder HostParent
        {
            get;
            set;
        }

        public MetaTreeNode RemovedMetaTreeNode { get; set; }

        public ObservableCollection<MetaTreeNode> MetaTreeNodes { get; set; }

        #endregion 
       
        private void listBoxMeasureElements_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.listBoxMeasureElements.Items.Count == 1)
            {
                this.MoveDown.IsEnabled = false;
                this.MoveUp.IsEnabled = false;
            }
            else
            {
                if ((sender as ListBox).SelectedIndex == (this.listBoxMeasureElements.Items.Count - 1))
                {
                    this.MoveDown.IsEnabled = false;
                    this.MoveUp.IsEnabled = true;
                }
                else if ((sender as ListBox).SelectedIndex == 0)
                {
                    this.MoveUp.IsEnabled = false;
                    this.MoveDown.IsEnabled = true;
                }
                else
                {
                    this.MoveUp.IsEnabled = true;
                    this.MoveDown.IsEnabled = true;
                }
            }
        
        }

        private void MenuItem_Remove_Click(object sender, RoutedEventArgs e)
        {
            if (this.listBoxMeasureElements.SelectedIndex >= 0)
            {
                MetaTreeNode mtNode = this.MetaTreeNodes[this.listBoxMeasureElements.SelectedIndex];
                if (mtNode != null)
                {
                    this.MetaTreeNodes.Remove(mtNode);
                }
            }
        }

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            CheckBox check = sender as CheckBox;
            foreach (var mtNode in this.MetaTreeNodes)
            {
                mtNode.IsSelected = check.IsChecked;
            }
        }


        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (this.listBoxMeasureElements.SelectedItem != null)
            {
                this.MetaTreeNodes.Remove(this.listBoxMeasureElements.SelectedItem as MetaTreeNode);
            }
        }

        private void UpDown_Click(object sender, RoutedEventArgs e)
        {
            if (this.listBoxMeasureElements.SelectedItem != null)
            {
                MetaTreeNode reArrangeNode = (MetaTreeNode)(this.listBoxMeasureElements.SelectedItem as MetaTreeNode).Clone();
                int index = this.MetaTreeNodes.IndexOf(this.listBoxMeasureElements.SelectedItem as MetaTreeNode);
                this.MetaTreeNodes.Remove(this.listBoxMeasureElements.SelectedItem as MetaTreeNode);

                if ((sender as Button).Name.Equals("MoveUp", StringComparison.InvariantCultureIgnoreCase))
                {
                    this.MetaTreeNodes.Insert(index - 1, reArrangeNode);
                }
                else if ((sender as Button).Name.Equals("MoveDown", StringComparison.InvariantCultureIgnoreCase))
                {
                    this.MetaTreeNodes.Insert(index + 1, reArrangeNode);
                }
            }
        }

        #region Private Methods

        /// <summary>
        /// Handles the Click event of the btnPopupCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnPopupCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the btnPopupClose control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnPopupClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the btnPopupOK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnPopupOK_Click(object sender, RoutedEventArgs e)
        {
            if (this.Host.OlapDataManager != null)
            {
                WaitingDialog waitingDialog = new WaitingDialog();
                try
                {
                    waitingDialog.Owner = this.Owner;
                    waitingDialog.Show();

                    if (this.MetaTreeNodes.Count > 0)
                    {
                        foreach (MetaTreeNode metaTreeNode in this.MetaTreeNodes)
                        {
                            metaTreeNode.AcceptIsSelectedChanges(true);
                        }
                    }

                    //// Updating the host meamber nodes
                    this.Host.MetaTreeNode.ChildNodes.Clear();
                    foreach (var node in this.MetaTreeNodes)
                    {
                        this.Host.MetaTreeNode.ChildNodes.Add(node);
                    }

                    //// if no item exist then remove the object from element collection itself
                    if (this.Host.MetaTreeNode.ChildNodes.Count == 0)
                    {
                        this.Host.Host.MetaTreeNodes.Remove(this.Host.MetaTreeNode);
                    }

                    if (HostParent != null && this.Host.MetaTreeNode != null)
                    {
                        //// Refresh the elements
                        if (this.Host.Host.Axis == Syncfusion.Olap.Reports.AxisPosition.Slicer && this.Host.MetaTreeNode.ChildNodes.Where(i => i.NodeType == MetaTreeNodeType.Measure && i.NodeCheckedType == MetaTreeNodeCheckedType.CurrentChecked).Count() > 1)
                            this.HostParent.RefreshOlapDataManagerElementItems(null, false, AxisType.Slicer);
                        else
                            this.HostParent.RefreshOlapDataManagerElementItems(null, true, AxisType.Slicer);

                    }
                    if (waitingDialog != null)
                    {
                        waitingDialog.Close();
                    }

                    this.Close();
                }
                catch (Exception ex)
                {
                    if (waitingDialog.IsActive)
                    {
                        waitingDialog.Close();
                    }

                    throw ex;
                }
            }
            else
            {
                this.Close();
            }
        }

        private void CheckNodeState()
        {
            int check = 0, unCheck = 0;
            foreach (MetaTreeNode mt in this.MetaTreeNodes)
            {
                if (mt.CheckedState == 2)
                    check++;
                else if (mt.CheckedState == 3)
                    unCheck++;
            }

            if (check == this.MetaTreeNodes.Count)
            {
                this.btnCheck.IsChecked = true;
            }
            else if (unCheck == this.MetaTreeNodes.Count)
            {
                this.btnCheck.IsChecked = false;
            }
            else
            {
                this.btnCheck.IsChecked = null;

            }

        }

        void MeasureEditor_Loaded(object sender, RoutedEventArgs e)
        {
            CheckNodeState();
        }
        #endregion

      

        
    }
}