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
using System.Reflection;
using System.Windows.Media.Imaging;
using RESX = Syncfusion.Windows.Reports.Designer.Properties.Resources;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;
using System.ComponentModel;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    internal class DataSourceItem
    {
        public string Name { get; set; }

        public object Value { get; set; }

        public string TableName { get; set; }

        public string DataSetName { get; set; }

        public string ObjectDataSourceSelectMethod { get; set; }

        public string ObjectDataSourceSelectMethodSignature { get; set; }

        public string ObjectDataSourceType { get; set; }
    }

    /// <summary>
    /// Interaction logic for RdlcDataSourceUI.xaml
    /// </summary>
    
      
    internal partial class RdlcDataSourceUI : ChromelessWindow
    {
        IList<string> dataSetNames;
        List<Assembly> Assemblies;
        RDL.DOM.DataSets datasets;
        BindingList<DataSourceItem> dataSourceItems;

        public List<object> SelectedObjects { get; set; }

        public RdlcDataSourceUI(IList<string> dataSetNames,RDL.DOM.DataSets datasets, List<Assembly> assembliesList)
        {
            InitializeComponent();

            dataSourceItems = new BindingList<DataSourceItem>();
            this.dataSetNames = dataSetNames;
            this.datasets = datasets;
            this.Assemblies = assembliesList;
            this.Loaded += new RoutedEventHandler(RdlcDataSourceUI_Loaded);
            this.Title = SR.GetString(CultureInfo.CurrentUICulture, "titleDataSetProperties");
        }

        void RdlcDataSourceUI_Loaded(object sender, RoutedEventArgs e)
        {
            DataSourceItem selectItem = new DataSourceItem();
            selectItem.Name = "Select DataSource";
            selectItem.Value = null;
            this.dataSourceItems.Add(selectItem);

            int row = 0;
            foreach (var dset in this.datasets)
            {
                DataSourceItem dsitem = null;

                if (dset.DataSetInfo != null && !string.IsNullOrEmpty(dset.DataSetInfo.ObjectDataSourceSelectMethod)
                    && !string.IsNullOrEmpty(dset.DataSetInfo.ObjectDataSourceType))
                {
                    dsitem = new DataSourceItem();
                    string nameSpace = dset.DataSetInfo.ObjectDataSourceType.Split(',')[0];
                    dsitem.Name = nameSpace + "." + dset.DataSetInfo.ObjectDataSourceSelectMethod;
                    dsitem.Value = null;
                    this.dataSourceItems.Add(dsitem);
                }

                RowDefinition defintion = new RowDefinition();
                defintion.Height = new GridLength(25, GridUnitType.Pixel);
                this.dataSetGrid.RowDefinitions.Add(defintion);

                TextBlock dataSet = new TextBlock();
                Border textBorder = new Border();    
                textBorder.BorderBrush = new SolidColorBrush(Colors.Silver);                
                textBorder.BorderThickness = new Thickness(1);
                textBorder.Height = 25;
                textBorder.Width = 215;
                dataSet.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                dataSet.Text = dset.Name;
                dataSet.TextAlignment =TextAlignment.Left;
                textBorder.Child = dataSet;
                Grid.SetRow(textBorder, row);
                Grid.SetColumn(textBorder, 0);
                this.dataSetGrid.Children.Add(textBorder);
                ComboBox dataSetCombo = new ComboBox();
                dataSetCombo.Height = 25;
                dataSetCombo.Name = dset.Name;
                Grid.SetRow(dataSetCombo, row++);
                Grid.SetColumn(dataSetCombo, 1);
                dataSetCombo.ItemsSource = this.dataSourceItems;
                dataSetCombo.SelectedValuePath = "Value";
                dataSetCombo.DisplayMemberPath = "Name";
                dataSetCombo.SelectedItem = dsitem;
                dataSetCombo.SelectionChanged += new SelectionChangedEventHandler(dataSetCombo_SelectionChanged);
                this.dataSetGrid.Children.Add(dataSetCombo);
            }
        }

        void dataSetCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems.Count > 0)
                {
                    DataSourceItem item = e.AddedItems[0] as DataSourceItem;

                    if (item.Value == null && item.Name == "Select DataSource")
                    {
                        var comboBox = (sender as ComboBox).Name;
                        var dsSet = (from set in this.datasets where comboBox.Equals(set.Name) select set).FirstOrDefault();
                        List<Assembly> assembly = new List<Assembly>();

                        if (dsSet != null && dsSet.DataSetInfo != null && !string.IsNullOrEmpty(dsSet.DataSetInfo.ObjectDataSourceType))
                        {
                            foreach (var assemly in this.Assemblies)
                            {
                                if (assemly.FullName.Split(',')[0].Equals(dsSet.DataSetInfo.DataSetName))
                                {
                                    assembly.Add(assemly);
                                }
                            }
                        }
                        else
                        {
                            assembly.AddRange(this.Assemblies);
                        }

                        RdlcObjectMethods RdlcMethod = new RdlcObjectMethods(assembly);
                        SkinStorage.SetVisualStyle(RdlcMethod, SkinStorage.GetVisualStyle(this));
                        RdlcMethod.Owner = Window.GetWindow(this);

                        if (RdlcMethod.ShowDialog() == true)
                        {
                            DataSourceItem selectItem = new DataSourceItem();
                            selectItem.Name = RdlcMethod.MethodName;
                            selectItem.Value = RdlcMethod.Value;
                            selectItem.TableName = RdlcMethod.TableName;
                            selectItem.DataSetName = RdlcMethod.DataSetName;
                            selectItem.ObjectDataSourceType = RdlcMethod.ObjectDataSourceType;
                            selectItem.ObjectDataSourceSelectMethod = RdlcMethod.ObjectDataSourceSelectMethod;
                            selectItem.ObjectDataSourceSelectMethodSignature = RdlcMethod.ObjectDataSourceSelectMethodSignature;
                            this.dataSourceItems.Add(selectItem);
                        }
                    }
                }
            }
            catch { }
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            int nullCount = (from combo in this.dataSetGrid.Children.OfType<ComboBox>()
                             where combo.SelectedIndex <1
                             select combo).Count();

            if (nullCount == 0 )
            {
                this.SelectedObjects = new List<object>();

                foreach (string dataSetName in this.dataSetNames)
                {
                    ComboBox comboBox = (from combo in this.dataSetGrid.Children.OfType<ComboBox>()
                                         where combo.Name == dataSetName
                                         select combo).First();
                    var dtSet = (from set in this.datasets where dataSetName.Equals(set.Name) select set).FirstOrDefault();
                    DataSourceItem objVal = comboBox.SelectedItem as DataSourceItem;

                    if (dtSet != null && objVal != null && objVal.Value != null)
                    {
                        var info = new RDL.DOM.DataSetInfo();
                        info.TableName = objVal.TableName;
                        info.DataSetName = objVal.DataSetName;
                        info.ObjectDataSourceType = objVal.ObjectDataSourceType;
                        info.ObjectDataSourceSelectMethod = objVal.ObjectDataSourceSelectMethod;
                        info.ObjectDataSourceSelectMethodSignature = objVal.ObjectDataSourceSelectMethodSignature;
                        dtSet.DataSetInfo = info;
                    }
                    if (objVal.Value == null)
                    {
                        objVal.Value = this.GetObjectValues(dtSet);
                    }
                    this.SelectedObjects.Add((comboBox.SelectedItem as DataSourceItem).Value);
                }

                this.DialogResult = true;
                this.Close();        
            }
            else
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSelectInstance"), SR.GetString(CultureInfo.CurrentUICulture, "titleInstanceNotSelect"));
            }
        }

        private object GetObjectValues(RDL.DOM.DataSet dtSet)
        {
            try
            {
                if (dtSet.DataSetInfo != null && !string.IsNullOrEmpty(dtSet.DataSetInfo.ObjectDataSourceType))
                {
                    string datasource = dtSet.DataSetInfo.ObjectDataSourceType.Split(',')[0];
                    Assembly dataAssemly = null;
                    foreach (var assemly in this.Assemblies)
                    {
                        if (assemly.FullName.Split(',')[0].Equals(dtSet.DataSetInfo.DataSetName))
                        {
                            dataAssemly = assemly;
                            break;
                        }
                    }
                    if (dataAssemly != null)
                    {
                        BindingFlags flags = (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
                        Type Types = dataAssemly.GetType(datasource);
                        MethodInfo method = Types.GetMethod(dtSet.DataSetInfo.ObjectDataSourceSelectMethod, flags);
                        ConstructorInfo Constructor = Types.GetConstructor(Type.EmptyTypes);
                        object ClassObject = Constructor.Invoke(null);
                        object Value = method.Invoke(ClassObject, null);
                        return Value;
                    }
                }
            }
            catch { }
            return null;
        }

        private void ChromelessWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

    }
}