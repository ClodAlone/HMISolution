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
using Syncfusion.Windows.Shared;
using Syncfusion.Olap.Manager;
using Syncfusion.Olap.Reports;
using Syncfusion.Olap.Data;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Tools.Olap
{
    /// <summary>
    /// Interaction logic for KPIEditor.xaml
    /// </summary>
    public partial class KPIEditor : ChromelessWindow
    {
        private string mdxQuery;
        internal bool isEdit = false;

        public string MdxQuery
        {
            get { return mdxQuery; }
            set { mdxQuery = value; }
        }

        private OlapDataManager _olapDataManager;

        public OlapDataManager OlapDataManager
        {
            get { return _olapDataManager; }
            set { _olapDataManager = value; }
        }


        public KPIEditor(OlapDataManager olapDataManager)
        {
            InitializeComponent();
            vkpicubeDimensionBrowser.OlapDataManager = olapDataManager;
            this.OlapDataManager = olapDataManager;
            cmbStatusList.ItemsSource = new ObservableCollection<KpiGraphic>
            {
                new KpiGraphic
                {
                     ImagePath = new BitmapImage(new Uri("/Syncfusion.OlapTools.WPF;component/Images/Shapes.png",UriKind.RelativeOrAbsolute)),
                      Title = "Shapes"
                },
                new KpiGraphic
                {
                    ImagePath = new BitmapImage(new Uri("/Syncfusion.OlapTools.WPF;component/Images/Traffic lights.png",UriKind.RelativeOrAbsolute)),
                      Title = "Traffic light"
                }
            };
            cmbTrendList.ItemsSource = new ObservableCollection<KpiGraphic>
            {
                new KpiGraphic
                {
                     ImagePath = new BitmapImage(new Uri("/Syncfusion.OlapTools.WPF;component/Images/Standard arrow.png",UriKind.RelativeOrAbsolute)),
                      Title = "Standard arrow"
                }
            };
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            bool canClose = true;
            VirtualKpiElement virtualKpiElement = new VirtualKpiElement();

            if (!string.IsNullOrEmpty(txtKpiName.Text))
            {
                virtualKpiElement.Name = txtKpiName.Text;
            }
            else
            {
                canClose = false;
                MessageBox.Show("KPI name should be non empty", "Virtual KPI Editor", MessageBoxButton.OK);
            }
            if (!string.IsNullOrEmpty(txtValueExpression.Text))
            {
                virtualKpiElement.KpiValueExpression = txtValueExpression.Text;
            }
            else
            {
                canClose = false;
                MessageBox.Show("Value expression should be non empty", "Virtual KPI Editor", MessageBoxButton.OK);
            }
            if (!string.IsNullOrEmpty(txtGoalExpression.Text))
            {
                virtualKpiElement.KpiGoalExpression = txtGoalExpression.Text;
            }
            else
            {
                canClose = false;
                MessageBox.Show("Goal expression should be non empty", "Virtual KPI Editor", MessageBoxButton.OK);
            }
            if (!string.IsNullOrEmpty(txtStatusExpression.Text))
                virtualKpiElement.KpiStatusExpression = txtStatusExpression.Text;
            if (!string.IsNullOrEmpty(txtTrendExpression.Text))
                virtualKpiElement.KpiTrendExpression = txtTrendExpression.Text;
            if (cmbStatusList.SelectedIndex > -1)
                virtualKpiElement.StatusGraphic = (cmbStatusList.SelectedItem as KpiGraphic).Title;
            if (cmbTrendList.SelectedIndex > -1)
                virtualKpiElement.TrendGraphic = (cmbTrendList.SelectedItem as KpiGraphic).Title;
            if ((this.vkpicubeDimensionBrowser.OlapDataManager as OlapDataManager).UseSharedDataManager)
            {
                (this.vkpicubeDimensionBrowser.OlapDataManager as OlapDataManager).VirtualKpiElements.Add(virtualKpiElement);
            }
            else
            {
                if (!isEdit)
                    this.vkpicubeDimensionBrowser.OlapDataManager.CurrentReport.VirtualKpiElements.Add(virtualKpiElement);
                else
                {
                    for (int i = 0; i < this.vkpicubeDimensionBrowser.OlapDataManager.CurrentReport.VirtualKpiElements.Count; i++)
                    {
                        if (virtualKpiElement.ElementName == this.vkpicubeDimensionBrowser.OlapDataManager.CurrentReport.VirtualKpiElements[i].ElementValue.ElementName)
                        {
                            this.vkpicubeDimensionBrowser.OlapDataManager.CurrentReport.VirtualKpiElements.RemoveAt(i);
                        }
                    }
                    this.vkpicubeDimensionBrowser.OlapDataManager.CurrentReport.VirtualKpiElements.Add(virtualKpiElement);
                }
            }
            if (canClose)
            {
                if ((this.vkpicubeDimensionBrowser.OlapDataManager as OlapDataManager).UseSharedDataManager)
                {
                    (this.vkpicubeDimensionBrowser.OlapDataManager as OlapDataManager).NotifyActiveReportChanged();
                }
                else
                    this.vkpicubeDimensionBrowser.OlapDataManager.NotifyElementModified();
                this.DialogResult = true;
                this.Close();
            }
        }

        private void ExpressionText_PreviewDrop(object sender, DragEventArgs e)
        {
            TextBox textBox = (sender as TextBox);
            if (e.Data.GetDataPresent(typeof(MetaTreeNode)))
            {
                MetaTreeNode clipBoardNode = (MetaTreeNode)e.Data.GetData(typeof(MetaTreeNode));
                if (clipBoardNode.NodeType != MetaTreeNodeType.KPI && clipBoardNode.NodeType != MetaTreeNodeType.KPI_Goal && clipBoardNode.NodeType != MetaTreeNodeType.KPI_Trend && clipBoardNode.NodeType != MetaTreeNodeType.KPI_Value && clipBoardNode.NodeType != MetaTreeNodeType.KPI_Status && clipBoardNode.NodeType != MetaTreeNodeType.VirtualKPIMember && clipBoardNode.NodeType != MetaTreeNodeType.VirtualKPI_Goal && clipBoardNode.NodeType != MetaTreeNodeType.VirtualKPI_Status && clipBoardNode.NodeType != MetaTreeNodeType.VirtualKPI_Trend && clipBoardNode.NodeType != MetaTreeNodeType.VirtualKPI_Value)
                {
                    textBox.Text += clipBoardNode.UniqueName;
                    textBox.Focus();
                }
            }
        }

        private void ExpressionText_PreviewDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }

    public class KpiGraphic
    {
        private ImageSource imagePath;

        public ImageSource ImagePath
        {
            get { return imagePath; }
            set { imagePath = value; }
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }


    }
}
