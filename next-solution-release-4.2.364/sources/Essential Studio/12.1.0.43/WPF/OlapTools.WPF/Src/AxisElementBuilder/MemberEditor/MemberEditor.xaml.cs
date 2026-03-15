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
    using System.Windows.Controls;
    using System.Windows.Media;
    using Syncfusion.Olap.Data;
    using Syncfusion.Windows.Shared;

    /// <summary>
    /// Interaction logic for MemberEditor.xaml
    /// </summary>
    public partial class MemberEditor : ChromelessWindow
    {

        #region Dependency Property Implementation

        /// <summary>
        /// Popup Title Dependency Property Implementation
        /// </summary>
        public static readonly DependencyProperty PopupTittleProperty =
            DependencyProperty.Register("PopupTittle", typeof(string), typeof(MemberEditor), new UIPropertyMetadata(string.Empty));

        #endregion

        #region Initilize/Finilaize

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberEditor"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        public MemberEditor(SplitButton host, ObservableCollection<MetaTreeNode> metaTreeNodes)
        {
            InitializeComponent();
            this.Host = host;
            this.MetaTreeNodes = metaTreeNodes;
            this.tvPopupData.ItemsSource = this.MetaTreeNodes;
            this.Loaded += new RoutedEventHandler(this.MemberEditor_Loaded);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        /// <value>The host.</value>
        public SplitButton Host { get; set; }

        public ObservableCollection<MetaTreeNode> MetaTreeNodes { get; set; }

        /// <summary>
        /// Gets or sets the popup title.
        /// </summary>
        /// <value>The popup title.</value>
        public string PopupTitle
        {
            get { return (string)GetValue(PopupTittleProperty); }
            set { SetValue(PopupTittleProperty, value); }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Sets the is checked.
        /// </summary>
        /// <param name="mtNode">The mt node.</param>
        /// <param name="value">The value.</param>
        /// <param name="updateChildren">if set to <c>true</c> [update children].</param>
        /// <param name="updateParent">if set to <c>true</c> [update parent].</param>
        //internal void SetIsChecked(MetaTreeNode mtNode, bool? value, bool updateChildren, bool updateParent)
        //{
        //    if (mtNode.NodeType == MetaTreeNodeType.DisplayFolder ||
        //        mtNode.NodeType == MetaTreeNodeType.MeasureGroup ||
        //        mtNode.NodeType == MetaTreeNodeType.KPI_ROOT ||
        //        mtNode.NodeType == MetaTreeNodeType.KPI)
        //    {
        //        if (value == true)
        //        {
        //            mtNode.__IsSelected = null;
        //            mtNode.__CheckedState = 1;
        //        }
        //        else
        //        {
        //            mtNode.__IsSelected = value;
        //            mtNode.__CheckedState = (value == null) ? 1 : (value == true) ? 2 : 3;
        //        }
        //    }
        //    else
        //    {
        //        mtNode.__IsSelected = value;
        //        mtNode.__CheckedState = (value == null) ? 1 : (value == true) ? 2 : 3;
        //    }
        //    if (value == null)
        //    {
        //        if (mtNode.NodeCheckedType == MetaTreeNodeCheckedType.SomeChildChecked)
        //        {
        //            value = true;
        //        }
        //    }

        //    // Updating chid and parent nodes.
        //    if (updateChildren && value.HasValue)
        //    {
        //        if (mtNode.ChildNodes != null)
        //        {
        //            foreach (MetaTreeNode _mtNode in mtNode.ChildNodes)
        //            {
        //                this.SetIsChecked(_mtNode, value, true, false);
        //            }
        //        }
        //    }

        //    if (updateParent && mtNode.ParentNode != null)
        //    {
        //        if (mtNode.ParentNode.NodeType != MetaTreeNodeType.DisplayFolder || value != true)
        //        {
        //            mtNode.ParentNode.NodeCheckedType = MetaTreeNodeCheckedType.SomeChildChecked;
        //            this.VerifyCheckState(mtNode.ParentNode);
        //        }
        //        else if (value == true)
        //        {
        //            this.SetIsChecked(mtNode.ParentNode, null, false, true);
        //        }
        //    }
        //}

        /// <summary>
        /// Verifies the state of the check.
        /// </summary>
        /// <param name="mtNode">The mt node.</param>
        //internal void VerifyCheckState(MetaTreeNode mtNode)
        //{
        //    bool? state = null;
        //    //if (mtNode.NodeType != MetaTreeNodeType.DisplayFolder)
        //    {
        //        bool? current = null;
        //        for (int i = 0; i < mtNode.ChildNodes.Count; ++i)
        //        {
        //            current = mtNode.ChildNodes[i].__IsSelected;
        //            if (i == 0)
        //            {
        //                state = current;
        //            }
        //            else if (state != current)
        //            {
        //                state = null;
        //                break;
        //            }
        //        }
        //    }

        //    if (state == true)
        //    {
        //        state = null;
        //    }

        //    this.SetIsChecked(mtNode, state, false, true);
        //}

        #endregion

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
                    // Accepting the change done in the meta tree Node.
                    if (this.MetaTreeNodes.Count > 0)
                    {
                        foreach (MetaTreeNode metaTreeNode in this.MetaTreeNodes)
                        {
                            metaTreeNode.AcceptIsSelectedChanges(true);
                        }
                        this.Host.Host.RefreshOlapDataManagerElementItems(null);
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
            //else
            //{
            //    throw new Exception("Cube OlapDataManager is null in Member Editor");
            //}
        }


        /// <summary>
        /// Handles the Loaded event of the MemberEditor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void MemberEditor_Loaded(object sender, RoutedEventArgs e)
        {
            CheckeNodeState();
        }

        private void CheckeNodeState()
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
                //this.UpdateCheckImage();
            }
            else if (unCheck == this.MetaTreeNodes.Count)
            {
                this.btnCheck.IsChecked = false;
                //this.UpdateCheckImage();
            }
            else
            {
                this.btnCheck.IsChecked = null;
                //this.btnCheck.Content = new Canvas() { Background = (Brush)new BrushConverter().ConvertFromString("#FF25528E"), 
                //                                        Height = this.btnCheck.Height - 10, 
                //                                        Width = this.btnCheck.Width - 10, 
                //                                        IsHitTestVisible = false };

            }

        }
        #endregion

        #region Overriden Method
        

        #endregion

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            CheckBox check = sender as CheckBox;
            foreach (var mtNode in this.MetaTreeNodes)
            {
                mtNode.IsSelected = check.IsChecked;
            }
            //System.Windows.Controls.Primitives.ToggleButton button = sender as System.Windows.Controls.Primitives.ToggleButton;
            //foreach (var mtNode in this.MetaTreeNodes)
            //{
            //    mtNode.IsSelected = button.IsChecked;
            //}

            //this.UpdateCheckImage();
        }

        private void UpdateCheckImage()
        {
            if (this.btnCheck.IsChecked == true)
            {
                this.btnCheck.Content = this.FindResource("checked") as Image;
            }
            else if (this.btnCheck.IsChecked == false)
            {
                this.btnCheck.Content = this.FindResource("unChecked") as Image;
            }
        }

        private void btnUnCheckAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var mtNode in this.MetaTreeNodes)
            {
                mtNode.IsSelected = false;
            }
        }
    }
}