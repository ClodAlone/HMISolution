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
using Syncfusion.OlapSilverlight.Data;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.OlapSilverlight.Manager;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using Syncfusion.OlapSilverlight.Reports;
using Syncfusion.OlapSilverlight.Common;
using Syncfusion.Silverlight.Tools.Olap;
using System.ComponentModel;

namespace Syncfusion.Silverlight.Tools.Olap
{
     [DesignTimeVisible(false)]
    public partial class VirtualKpiEditor : WindowControl
    {
        private string mdxQuery;
        private int _flag;
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
        private DragDropManager DragDropManager
        {
            get
            {
                if (this.cubeDimensionBrowser.OlapDataManager != null)
                    return this.cubeDimensionBrowser.OlapDataManager.DragDropManager;
                else
                    return null;
            }
            set
            {
                if (this.cubeDimensionBrowser.OlapDataManager != null)
                    this.cubeDimensionBrowser.OlapDataManager.DragDropManager = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public VirtualKpiEditor()
        {
            InitializeComponent();
            this.Closing += new ClosedEventHandler(VirtualKpiEditor_Closing);
            this.Closing -= new ClosedEventHandler(VirtualKpiEditor_Closing);
            this.cubeDimensionBrowser.NodeClicked += new Syncfusion.Silverlight.Tools.Olap.NodeClickedHandler(cubeDimensionBrowser_NodeClicked);
            this.cubeDimensionBrowser.NodeClicked -= new Syncfusion.Silverlight.Tools.Olap.NodeClickedHandler(cubeDimensionBrowser_NodeClicked);
            this.cubeDimensionBrowser.MouseMove += new MouseEventHandler(cubeDimensionBrowser_MouseMove);
            this.cubeDimensionBrowser.MouseMove -= new MouseEventHandler(cubeDimensionBrowser_MouseMove);
            this.txtGoalExpression.RemoveHandler(TextBox.MouseLeftButtonUpEvent, new MouseButtonEventHandler(ExpressionText_MouseLeftButtonUp));
            this.txtGoalExpression.AddHandler(TextBox.MouseLeftButtonUpEvent, new MouseButtonEventHandler(ExpressionText_MouseLeftButtonUp), true);
            this.txtValueExpression.RemoveHandler(TextBox.MouseLeftButtonUpEvent, new MouseButtonEventHandler(ExpressionText_MouseLeftButtonUp));
            this.txtValueExpression.AddHandler(TextBox.MouseLeftButtonUpEvent, new MouseButtonEventHandler(ExpressionText_MouseLeftButtonUp), true);
            this.txtStatusExpression.RemoveHandler(TextBox.MouseLeftButtonUpEvent, new MouseButtonEventHandler(ExpressionText_MouseLeftButtonUp));
            this.txtStatusExpression.AddHandler(TextBox.MouseLeftButtonUpEvent, new MouseButtonEventHandler(ExpressionText_MouseLeftButtonUp), true);
            this.txtTrendExpression.RemoveHandler(TextBox.MouseLeftButtonUpEvent, new MouseButtonEventHandler(ExpressionText_MouseLeftButtonUp));
            this.txtTrendExpression.AddHandler(TextBox.MouseLeftButtonUpEvent, new MouseButtonEventHandler(ExpressionText_MouseLeftButtonUp), true);
        }
        public void ExpressionText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (DragDropManager != null)
            {
                if (this.DragDropManager.SelectedNode.NodeType != MetaTreeNodeType.KPI && this.DragDropManager.SelectedNode.NodeType != MetaTreeNodeType.KPI_Goal && this.DragDropManager.SelectedNode.NodeType != MetaTreeNodeType.KPI_Status && this.DragDropManager.SelectedNode.NodeType != MetaTreeNodeType.KPI_Trend && this.DragDropManager.SelectedNode.NodeType != MetaTreeNodeType.KPI_Value
                    && this.DragDropManager.SelectedNode.NodeType != MetaTreeNodeType.VirtualKPI_Goal && this.DragDropManager.SelectedNode.NodeType != MetaTreeNodeType.VirtualKPI_Status && this.DragDropManager.SelectedNode.NodeType != MetaTreeNodeType.VirtualKPI_Trend && this.DragDropManager.SelectedNode.NodeType != MetaTreeNodeType.VirtualKPI_Value && this.DragDropManager.SelectedNode.NodeType != MetaTreeNodeType.VirtualKPIMember)
                {
                    if (this._status.IsSelected)
                        this.txtStatusExpression.Text = this.DragDropManager.SelectedNode.UniqueName;
                    else if (this._goal.IsSelected)
                        this.txtGoalExpression.Text = this.DragDropManager.SelectedNode.UniqueName;
                    else if (this._value.IsSelected)
                        this.txtValueExpression.Text = this.DragDropManager.SelectedNode.UniqueName;
                    else if (this._trend.IsSelected)
                        this.txtTrendExpression.Text = this.DragDropManager.SelectedNode.UniqueName;
                }
                this.DragDropManager.DragDropPopup.IsOpen = false;
                this.DragDropManager = null;
            }
            if (_flag == 2)
            {
                _flag = 0;
                SetVisibility(Visibility.Collapsed);
            }
        }
        

        /// <summary>
        /// Mouse move event handler for drag and drop support
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (this.DragDropManager != null)
            {
                Point point = e.GetPosition(null);
                this.DragDropManager.DragDropPopup.Child.Visibility = System.Windows.Visibility.Visible;
                this.DragDropManager.DragDropPopup.HorizontalOffset = point.X - (this.DragDropManager.DragDropPopup.ActualWidth + 5);
                this.DragDropManager.DragDropPopup.VerticalOffset = point.Y + 15;
            }
        }

        /// <summary>
        /// Mouse left button up event handler for drag and drop support
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (this.DragDropManager != null)
            {
                this.DragDropManager.DragDropPopup.IsOpen = false;
                this.DragDropManager = null;
            }
        }
        void cubeDimensionBrowser_MouseMove(object sender, MouseEventArgs e)
        {
            if (_flag == 1)
            {
                _flag = 2;
                SetVisibility(Visibility.Visible);
            }
        }

        private void SetVisibility(System.Windows.Visibility visibility)
        {
            throw new NotImplementedException();
        }

        void cubeDimensionBrowser_NodeClicked(object sender, Syncfusion.Silverlight.Tools.Olap.NodeClickedEventArgs e)
        {
            if ((sender as CDTreeViewItem) != null)
            {
                _flag = 1;
            }
        }

        void VirtualKpiEditor_Closing(object sender, ClosedEventArgs e)
        {
            if (this.DragDropManager != null)
            {
                this.DragDropManager.DragDropPopup.IsOpen = false;
                this.DragDropManager = null;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualKpiEditor"/> class.
        /// </summary>
        /// <param name="OlapDataManager">The olapDataManager as <see cref="OlapDataManager"/>.</param>        
        public VirtualKpiEditor(OlapDataManager olapDataManager)
        {
            InitializeComponent();
            cubeDimensionBrowser.OlapDataManager = olapDataManager;
            this.OlapDataManager = olapDataManager;
            cmbStatusList.ItemsSource = new ObservableCollection<KpiImage>
            {
                new KpiImage
                {
                     ImagePath = new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Tools/Images/Shapes.png",UriKind.RelativeOrAbsolute)),
                      Title = "Shapes"
                },
                new KpiImage
                {
                    ImagePath = new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Tools/Images/Traffic lights.png",UriKind.RelativeOrAbsolute)),
                      Title = "Traffic light"
                }
            };
            cmbStatusList.SelectedIndex = 0;
            cmbTrendList.ItemsSource = new ObservableCollection<KpiImage>
            {
                new KpiImage
                {
                     ImagePath = new BitmapImage(new Uri("/Syncfusion.OlapClient.Silverlight;component/Tools/Images/Standard arrow.png",UriKind.RelativeOrAbsolute)),
                      Title = "Standard arrow"
                }
            };
            cmbTrendList.SelectedIndex = 0;
            this.Closing += new ClosedEventHandler(VirtualKpiEditor_Closing);
            this.Closing -= new ClosedEventHandler(VirtualKpiEditor_Closing);
            this.cubeDimensionBrowser.NodeClicked += new Syncfusion.Silverlight.Tools.Olap.NodeClickedHandler(cubeDimensionBrowser_NodeClicked);
            this.cubeDimensionBrowser.NodeClicked -= new Syncfusion.Silverlight.Tools.Olap.NodeClickedHandler(cubeDimensionBrowser_NodeClicked);
            this.cubeDimensionBrowser.MouseMove += new MouseEventHandler(cubeDimensionBrowser_MouseMove);
            this.cubeDimensionBrowser.MouseMove -= new MouseEventHandler(cubeDimensionBrowser_MouseMove);
            this.txtGoalExpression.RemoveHandler(TextBox.MouseLeftButtonUpEvent, new MouseButtonEventHandler(ExpressionText_MouseLeftButtonUp));
            this.txtGoalExpression.AddHandler(TextBox.MouseLeftButtonUpEvent, new MouseButtonEventHandler(ExpressionText_MouseLeftButtonUp), true);
            this.txtValueExpression.RemoveHandler(TextBox.MouseLeftButtonUpEvent, new MouseButtonEventHandler(ExpressionText_MouseLeftButtonUp));
            this.txtValueExpression.AddHandler(TextBox.MouseLeftButtonUpEvent, new MouseButtonEventHandler(ExpressionText_MouseLeftButtonUp), true);
            this.txtStatusExpression.RemoveHandler(TextBox.MouseLeftButtonUpEvent, new MouseButtonEventHandler(ExpressionText_MouseLeftButtonUp));
            this.txtStatusExpression.AddHandler(TextBox.MouseLeftButtonUpEvent, new MouseButtonEventHandler(ExpressionText_MouseLeftButtonUp), true);
            this.txtTrendExpression.RemoveHandler(TextBox.MouseLeftButtonUpEvent, new MouseButtonEventHandler(ExpressionText_MouseLeftButtonUp));
            this.txtTrendExpression.AddHandler(TextBox.MouseLeftButtonUpEvent, new MouseButtonEventHandler(ExpressionText_MouseLeftButtonUp), true);
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
                virtualKpiElement.StatusGraphic = (cmbStatusList.SelectedItem as KpiImage).Title;
            if (cmbTrendList.SelectedIndex > -1)
                virtualKpiElement.TrendGraphic = (cmbTrendList.SelectedItem as KpiImage).Title;
            if ((this.cubeDimensionBrowser.OlapDataManager as OlapDataManager)!=null && !isEdit)
            {
                (this.cubeDimensionBrowser.OlapDataManager as OlapDataManager).VirtualKpiElements.Add(virtualKpiElement);
            }
            else
            {
                if (!isEdit)
                    this.cubeDimensionBrowser.OlapDataManager.CurrentReport.VirtualKpiElements.Add(virtualKpiElement);
                else
                {
                    for (int i = 0; i < this.cubeDimensionBrowser.OlapDataManager.CurrentReport.VirtualKpiElements.Count; i++)
                    {
                        if (virtualKpiElement.ElementName == this.cubeDimensionBrowser.OlapDataManager.CurrentReport.VirtualKpiElements[i].ElementValue.ElementName)
                        {
                            this.cubeDimensionBrowser.OlapDataManager.CurrentReport.VirtualKpiElements.RemoveAt(i);
                        }
                    }
                    this.cubeDimensionBrowser.OlapDataManager.CurrentReport.VirtualKpiElements.Add(virtualKpiElement);
                }
            }

            if (canClose)
            {
                if ((this.cubeDimensionBrowser.OlapDataManager as OlapDataManager) != null)
                {
                    (this.cubeDimensionBrowser.OlapDataManager as OlapDataManager).NotifyReportChanged();
                }
                else
                    this.cubeDimensionBrowser.OlapDataManager.NotifyElementChanged();
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
    public class KpiImage
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
