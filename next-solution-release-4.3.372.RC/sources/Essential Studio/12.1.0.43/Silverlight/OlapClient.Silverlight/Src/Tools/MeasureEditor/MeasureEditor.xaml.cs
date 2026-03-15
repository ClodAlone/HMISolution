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
using System.Collections.ObjectModel;
using Syncfusion.OlapSilverlight.Data;
using Syncfusion.OlapSilverlight.Reports;
using System.ComponentModel;
using Syncfusion.Silverlight.Client.Olap.Resources;

namespace Syncfusion.Silverlight.Tools.Olap
{
    [DesignTimeVisible(false)]
    public partial class MeasureEditor : Syncfusion.Windows.Tools.Controls.WindowControl
    {

        #region Private Members

        bool IsOrderChanged, IsCollectionChanged, IsDeleteExist;

        MeasureElementCollection tempMeasureCollection;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        /// <value>The host.</value>
        public SplitButton Host { get; set; }

        /// <summary>
        /// Gets or sets the meta tree nodes.
        /// </summary>
        /// <value>The meta tree nodes.</value>
        public ObservableCollection<MetaTreeNode> MetaTreeNodes { get; set; }
        
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MeasureEditor"/> class.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="metatTreeNodes">The metat tree nodes.</param>
        public MeasureEditor(SplitButton host, ObservableCollection<MetaTreeNode> metatTreeNodes)
        {
            InitializeComponent();
            this.Title = SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "OlapClient_MeasureEditor_Title");
            this.Host = host;
            this.MetaTreeNodes = metatTreeNodes;
            this.MeasureList.ItemsSource = this.MetaTreeNodes;
            this.MeasureList.SelectionChanged += MeasureList_SelectionChanged;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Update the report items based on the changes
        /// </summary>
        /// <param name="tempReportItems">Current report item</param>
        private void UpDateCheckedNode(Item measureItem)
        {
            Items tempReportItems = this.Host.Parent.ReportItems;
            
            if( measureItem != null && measureItem.ElementValue != null && tempMeasureCollection != null)
            {
                Element currentMeasureElement = measureItem.ElementValue as MeasureElements;
                (currentMeasureElement as MeasureElements).Elements.Clear();
                (currentMeasureElement as MeasureElements).ExcludedMeasures.Clear();
                if (this.MetaTreeNodes.Count == 0)
                {
                    tempReportItems.Remove(measureItem);
                }
                else
                {
                    foreach (MetaTreeNode child in this.MetaTreeNodes)
                    {
                        for (int i = 0; i < tempMeasureCollection.Count; i++)
                        {
                            if (child.UniqueName.Equals(tempMeasureCollection[i].UniqueName, StringComparison.InvariantCultureIgnoreCase))
                            {
                                if (child.IsSelected == true)
                                { (currentMeasureElement as MeasureElements).Elements.Add(tempMeasureCollection[i]); }
                                else
                                { (currentMeasureElement as MeasureElements).ExcludedMeasures.Add(tempMeasureCollection[i]); }
                                tempMeasureCollection.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Finds the measure items.
        /// </summary>
        /// <param name="reportItems">The current report items.</param>
        /// <returns></returns>
        private object FindMeasureItems(Items reportItems, bool onlyItem)
        {
            foreach (Item reportItem in reportItems)
            {
                if (reportItem.ElementValue is MeasureElements)
                {
                    MeasureElementCollection tempMeasures = new MeasureElementCollection();
                    if (onlyItem)
                    {
                        return reportItem;
                    }

                    foreach (MeasureElement measure in (reportItem.ElementValue as MeasureElements).Elements)
                    {
                        tempMeasures.Add(measure);
                    }

                    if ((reportItem.ElementValue as MeasureElements).ExcludedMeasures.Count > 0)
                    {
                        foreach (MeasureElement measure in (reportItem.ElementValue as MeasureElements).ExcludedMeasures)
                        {
                            tempMeasures.Add(measure);
                        }
                        return tempMeasures;
                    }
                    else
                    {
                        return tempMeasures;
                    }                   
                }
            }
            return null;
        }

        #endregion

        #region Event

        /// <summary>
        /// Called when [apply template].
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            (this.GetTemplateChild("PART_MinimizeButton") as Button).Visibility = System.Windows.Visibility.Collapsed;
            (this.GetTemplateChild("PART_RestoreButton") as Button).Visibility = System.Windows.Visibility.Collapsed;
            (this.GetTemplateChild("PART_MaximizeButton") as Button).Visibility = System.Windows.Visibility.Collapsed;
            (this.GetTemplateChild("PART_Resizegrip") as Border).Visibility = System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// Handles the SelectionChanged event of the Measures List control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        void MeasureList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.MeasureList.Items.Count == 1)
            {
                this.MoveDown.IsEnabled = false;
                this.MoveUp.IsEnabled = false;
            }
            else
            {
                if ((sender as ListBox).SelectedIndex == (this.MeasureList.Items.Count - 1))
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

        /// <summary>
        /// Handles the Click event of the OK Button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            //this.DialogResult = true;
            try
            {
                this.IsCollectionChanged = false;
                Item measureItem = FindMeasureItems(this.Host.Parent.ReportItems, true) as Item;
                MeasureElements measure = (MeasureElements)measureItem.ElementValue;
                tempMeasureCollection = (MeasureElementCollection)this.FindMeasureItems(this.Host.Parent.ReportItems, false);

                List<string> selectedElement = new List<string>();
                foreach (var item in (measureItem.ElementValue as MeasureElements).Elements)
                {
                    selectedElement.Add(item.UniqueName);
                }

                List<string> unSelectedElement = new List<string>();
                foreach (var item in (measureItem.ElementValue as MeasureElements).ExcludedMeasures)
                {
                    unSelectedElement.Add(item.UniqueName);
                }

                foreach (MetaTreeNode metaTreeNode in this.MetaTreeNodes)
                {
                    metaTreeNode.AcceptIsSelectedChanges(true);
                }

                this.UpDateCheckedNode(measureItem);

                int selectedCount = 0, unselectedCount = 0;

                foreach (var item in selectedElement)
                {
                    selectedCount += measure.Elements.Where(i => i.UniqueName == item).Count();
                }

                foreach (var item in unSelectedElement)
                {
                    unselectedCount += measure.ExcludedMeasures.Where(i => i.UniqueName == item).Count();
                }

                if (selectedCount != selectedElement.Count || unselectedCount != unSelectedElement.Count)
                {
                    IsCollectionChanged = true;
                }

                if (IsOrderChanged || IsDeleteExist || IsCollectionChanged)
                {
                    if ((measureItem.ElementValue as MeasureElements).Elements.Count == 0)
                        this.Host.Parent.ReportItems.Remove(measureItem);
                    if (this.Host.Parent.AutoExecute)
                        this.Host.OlapDataManager.NotifyElementChanged(this.Host.Parent.Axis);
                    else
                        this.Host.OlapDataManager.RefreshAxisElementBuilder();
                }
                this.Close();
            }
            catch (Exception ex)
            {
                Syncfusion.Windows.Tools.Controls.WindowControl.ShowAlert(ex.Message,SR.GetString(System.Globalization.CultureInfo.CurrentUICulture,"OlapClient_Errors_ErrorWhileUpdating"),Syncfusion.Windows.Tools.Controls.DialogIcon.Information, Windows.Tools.Controls.DialogButton.OK, null, Windows.Tools.Controls.AnimationType.Zoom);
            }
        }

        /// <summary>
        /// Handles the Click event of the UpDown Button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void UpDown_Click(object sender, RoutedEventArgs e)
        {
            if (this.MeasureList.SelectedItem != null)
            {
                MetaTreeNode reArrangeNode = (MetaTreeNode)(this.MeasureList.SelectedItem as MetaTreeNode).Clone();
                int index = this.MetaTreeNodes.IndexOf(this.MeasureList.SelectedItem as MetaTreeNode);
                this.MetaTreeNodes.Remove(this.MeasureList.SelectedItem as MetaTreeNode);

                if ((sender as Button).Name.Equals("MoveUp", StringComparison.InvariantCultureIgnoreCase))
                {
                    this.MetaTreeNodes.Insert(index - 1, reArrangeNode);
                }
                else if ((sender as Button).Name.Equals("MoveDown", StringComparison.InvariantCultureIgnoreCase))
                {
                    this.MetaTreeNodes.Insert(index + 1, reArrangeNode);
                }
                IsOrderChanged = true;
            }
        }

        /// <summary>
        /// Handles the Click event of the Delete Button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Delete_Click(object sender, RoutedEventArgs e)
        { 
            if (this.MeasureList.SelectedItem != null)
            {
                this.MetaTreeNodes.Remove(this.MeasureList.SelectedItem as MetaTreeNode);
                IsDeleteExist = true;
            } 
        }

        /// <summary>
        /// Handles the Click event of the Cancel Button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}

