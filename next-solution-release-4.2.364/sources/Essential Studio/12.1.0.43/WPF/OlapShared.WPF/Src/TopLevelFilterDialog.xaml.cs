
#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Syncfusion.Olap.Data;
using Syncfusion.Olap.Manager;
using Syncfusion.Olap.Reports;


namespace Syncfusion.Windows.Shared.Olap
{
    /// <summary>
    /// Interaction logic for Essential WPF RibbonWindow1.xaml
    /// </summary>
    public partial class TopLevelFilter : ChromelessWindow
    {
        #region Private Variables
        OlapDataManager _cubeModel;       
        #endregion

        #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TopLevelFilter"/> class.
        /// </summary>
        /// <param name="cubeModel"></param>
        public TopLevelFilter(IOlapDataManager cubeModel)
        {             
            InitializeComponent();            
            System.Globalization.NumberFormatInfo number = new System.Globalization.NumberFormatInfo();
            number.NumberDecimalDigits = 0;            

            this.ResizeMode = ResizeMode.NoResize;
            this.Width = 300;
            this.Height = 300;
            this._cubeModel = (OlapDataManager)cubeModel;
            DisableFilterOneCombos();
            DisableFilterTwoCombos();
            this.Loaded += new RoutedEventHandler(TopLevelFilter_Loaded);
        }
        /// <summary>
        /// Gets the Metatreenodes property from the parent Window
        /// </summary>
        public static readonly DependencyProperty MetaTreeNodesProperty =
DependencyProperty.Register("MetaTreeNodes", typeof(MetaTreeNodeCollection), typeof(TopLevelFilter), new UIPropertyMetadata(new MetaTreeNodeCollection(null), TopLevelFilter.OnMetaTreeNodesChanged));

        /// <summary>
        /// Gets or sets the meta tree nodes.
        /// </summary>
        /// <value>Holds the metatreenodes value of type Measures</value>
        public MetaTreeNodeCollection MetaTreeNodes
        {
            get { return (MetaTreeNodeCollection)GetValue(MetaTreeNodesProperty); }
            set { SetValue(MetaTreeNodesProperty, value); }
        }
        #endregion  

        #region Binding Measure elements to all Combos
        /// <summary>
        /// Called when [meta tree node value is changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnMetaTreeNodesChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            TopLevelFilter topLevelFilter = dependencyObject as TopLevelFilter;
            if (topLevelFilter != null)
            {
                topLevelFilter.ddlMeasureNameone.ItemsSource = topLevelFilter.MetaTreeNodes;                
                topLevelFilter.ddlMeasureNametwo.ItemsSource = topLevelFilter.MetaTreeNodes;                
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the cube model.
        /// </summary>
        /// <value>Holds the OlapDataManager</value>
        public OlapDataManager OlapDataManager
        {
            get
            {
                return _cubeModel;
            }

            set
            {
                if (_cubeModel != value)
                {
                    _cubeModel = (OlapDataManager)value;
                }
            }
        }
        public AxisPosition Axis { get; set; }
        #endregion

        #region Cancelling Cubemodel
        private void CancelOlapDataManager(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Update OlapDataManager
        private void updateOlapDataManager(object sender, RoutedEventArgs e)
        {
            int rowValue = -1;
            int columnValue = -1;
            if ((this.TopfilterOneGroupCheckBox.IsChecked == true) && (this.TopfilterTwoGroupCheckBox.IsChecked == true))
            {
                if (!ValidateTopFilterCondition(ddlMeasureNameone))
                {
                    return;
                }
                if (!ValidateTopFilterCondition(ddlMeasureNametwo))
                {
                    return;
                }

                AddElements(AxisPosition.Categorical, ddlMeasureNameone.Text,Convert.ToInt32(fieldCountone.Value));
                AddElements(AxisPosition.Series, ddlMeasureNametwo.Text, Convert.ToInt32(fieldCounttwo.Value));
            }
            else if ((this.TopfilterOneGroupCheckBox.IsChecked == true) && (this.TopfilterTwoGroupCheckBox.IsChecked == false))
            {
                if (!ValidateTopFilterCondition(ddlMeasureNameone))
                {
                    return;
                }
                AddElements(AxisPosition.Categorical, ddlMeasureNameone.Text, Convert.ToInt32(fieldCountone.Value));
                rowValue = this.CheckTopFilterElements(this.OlapDataManager.CurrentReport.SeriesElements);
                if (rowValue > 0)
                {
                    this.OlapDataManager.CurrentReport.SeriesElements.RemoveAt(rowValue);
                }
            }
            else if ((this.TopfilterTwoGroupCheckBox.IsChecked == true) && (this.TopfilterOneGroupCheckBox.IsChecked==false))
            {
                if (!ValidateTopFilterCondition(ddlMeasureNametwo))
                {
                    return;
                }
                AddElements(AxisPosition.Series, ddlMeasureNametwo.Text, Convert.ToInt32(fieldCounttwo.Value));
                columnValue = this.CheckTopFilterElements(this.OlapDataManager.CurrentReport.CategoricalElements);
                if (columnValue > 0)
                {
                    this.OlapDataManager.CurrentReport.CategoricalElements.RemoveAt(columnValue);
                }
            }
            else
            {
                rowValue = this.CheckTopFilterElements(this.OlapDataManager.CurrentReport.SeriesElements);
                if (rowValue > 0)
                {
                    this.OlapDataManager.CurrentReport.SeriesElements.RemoveAt(rowValue);
                }
                columnValue = this.CheckTopFilterElements(this.OlapDataManager.CurrentReport.CategoricalElements);
                if (columnValue > 0)
                {
                    this.OlapDataManager.CurrentReport.CategoricalElements.RemoveAt(columnValue);
                }
            }                       
            try
            {
                this.OlapDataManager.NotifyElementModified();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error loading Data");
            }
            finally
            {
                this.Close();
            }
        }
        #endregion

        #region Adding Elements to OlapDataManager
        public void AddElements(AxisPosition axis, string measurename, int fieldcnt)
        {
            int rowValue=-1;
            int columnValue=-1;
            if (axis == AxisPosition.Series)
            {
                this.Axis = AxisPosition.Series;
                TopCountElement topCountElement = new TopCountElement(this.Axis, fieldcnt);
                topCountElement.MeasureName = measurename;
                rowValue = this.CheckTopFilterElements(this.OlapDataManager.CurrentReport.SeriesElements);
                if (rowValue > 0)
                {
                    this.OlapDataManager.CurrentReport.SeriesElements.RemoveAt(rowValue);
                }
                this.OlapDataManager.CurrentReport.SeriesElements.Add(new Item { Axis = axis, ElementValue = topCountElement });
            }
            else
            {
                this.Axis = AxisPosition.Categorical;
                TopCountElement topCountElement = new TopCountElement(this.Axis, fieldcnt);
                topCountElement.MeasureName = measurename;
                columnValue = this.CheckTopFilterElements(this.OlapDataManager.CurrentReport.CategoricalElements);
                if (columnValue > 0)
                {
                    this.OlapDataManager.CurrentReport.CategoricalElements.RemoveAt(columnValue);
                }
                this.OlapDataManager.CurrentReport.CategoricalElements.Add(new Item { Axis = axis, ElementValue = topCountElement });
            }
        }


        private int CheckTopFilterElements (Syncfusion.Olap.Reports.Items items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                Element topFilterElement = items[i].ElementValue;
                if (topFilterElement is TopCountElement)
                {
                    return i;
                }
            }

            return -1;
        }
        #endregion
        
        #region defaulting combos from OlapDataManager
        /// <summary>
        /// Loads the values from cube model.
        /// </summary>
        public void LoadValuesFromOlapDataManager()
        {
            Syncfusion.Olap.Reports.Items items = new Syncfusion.Olap.Reports.Items(); ;
            int j=-1,k=-1;

                items = this.OlapDataManager.CurrentReport.CategoricalElements;
                j = CheckTopFilterElements(items);
                if (j >= 0)
                {                    
                    showdatavalues(items,"categorical");
                }                

                items = this.OlapDataManager.CurrentReport.SeriesElements;
                k = CheckTopFilterElements(items);
                if (k >= 0)
                {                    
                    showdatavalues(items,"series");
                }
        }

        /// <summary>
        /// show the values
        /// </summary>
        /// <param name="items"></param>
        public void showdatavalues(Syncfusion.Olap.Reports.Items items,string ax)
        {
            TopCountElement top = null;
            for (int i = 0; i < items.Count; i++)
            {
                Item item = items[i];
                if (item.ElementValue is TopCountElement)
                {
                    top = (TopCountElement)item.ElementValue;                    
                    if (top != null)
                    {
                        if (ax == "categorical")
                        {
                            fieldCountone.Value = top.FieldCount;
                            PopulateMeasuresCombo(ddlMeasureNameone, top.MeasureName);
                            TopfilterOneGroupCheckBox.IsChecked = true;
                        }
                        else                        
                        {
                            fieldCounttwo.Value = top.FieldCount;
                            PopulateMeasuresCombo(ddlMeasureNametwo, top.MeasureName);
                            TopfilterTwoGroupCheckBox.IsChecked = true;
                        }
                    }
                }
            }
        }
        #endregion
        
        #region Populating Measures Combo   
        /// <summary>
        /// Populate the measure
        /// </summary>
        /// <param name="measuresBox"></param>
        /// <param name="uniquename"></param>
        private void PopulateMeasuresCombo(ComboBox measuresBox, string uniquename)
        {
            for (int i = 0; i < this.MetaTreeNodes.Count; i++)
            {
                MetaTreeNode mtNode = this.MetaTreeNodes[i];
                if ((mtNode.UniqueName.ToLower()) == (uniquename.ToLower()))
                {
                    measuresBox.SelectedIndex = i;
                    break;
                }
            }
        }
        #endregion

        #region Diabling Controls
        private void DisableFilterOneCombos()
        {
            this.ddlMeasureNameone.IsEnabled = false;            
            this.fieldCountone.IsEnabled = false;            
        }
        private void DisableFilterTwoCombos()
        {
            this.ddlMeasureNametwo.IsEnabled = false;
            this.fieldCounttwo.IsEnabled = false;
        }
        #endregion

        #region Enabling Controls
        private void EnableFilterOneCombos()
        {
            this.ddlMeasureNameone.IsEnabled = true;
            this.fieldCountone.IsEnabled = true;
        }

        private void EnableFilterTwoCombos()
        {
            this.ddlMeasureNametwo.IsEnabled = true;
            this.fieldCounttwo.IsEnabled = true;  
        }
        #endregion

        #region Validate Measures Combo
        private bool ValidateTopFilterCondition(ComboBox measurename)
        {
            if (measurename.SelectedItem == null)
            {
                MessageBox.Show("Please choose the Measure to Top Level filter the Cube", "Invalid Choice", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }            
            
            return true;
        }
        #endregion        

        #region Events

        private void TopLevelFilter_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                CubeSchema cubeSchema = OlapDataManager.DataProvider.GetCubeSchema(OlapDataManager.CurrentCubeName);
                MetaTreeNode mtNode = new MetaTreeNode();
                MetaTreeHelper.FillMetaTreeNode(mtNode, cubeSchema.Measures, false, true);
                MetaTreeNodes = mtNode.ChildNodes;
                LoadValuesFromOlapDataManager();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void TopfilterOneGroupCheckBox_Checked(object sender, RoutedEventArgs e)
        {            
            if ((bool)TopfilterOneGroupCheckBox.IsChecked)
            {               
                EnableFilterOneCombos();                
            }           
        }

        private void TopfilterOneGroupCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!(bool)TopfilterOneGroupCheckBox.IsChecked)
            {                
                DisableFilterOneCombos();                
            }
        }

        private void TopfilterTwoGroupCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)TopfilterTwoGroupCheckBox.IsChecked)
            {
                EnableFilterTwoCombos();                             
            }            
        }

        private void TopfilterTwoGroupCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {            
            if (!(bool)TopfilterTwoGroupCheckBox.IsChecked)
            {
                DisableFilterTwoCombos();
            }
        }
        #endregion

    }
}
