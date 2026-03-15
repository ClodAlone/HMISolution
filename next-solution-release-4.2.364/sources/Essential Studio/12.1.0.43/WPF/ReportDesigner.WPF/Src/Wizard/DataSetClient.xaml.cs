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
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Syncfusion.Windows.Reports.Designer.Controls;
using System.ComponentModel;
using RESX = Syncfusion.Windows.Reports.Designer.Properties.Resources;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{

    internal class CachedAssemblyInfos: List<AssemblyInfo>
    {
    }

    internal class AssemblyInfo : INotifyPropertyChanged
    {
        private bool? isChecked = false;

        public bool? IsChecked 
        {
            get
            {
                return this.isChecked;
            }
            set
            {
                if (this.isChecked != value)
                {
                    this.isChecked = value;
                    this.OnPropertyChanged("IsChecked");
                }
            }
        }

        public bool IsInternalCheckStateChange { get; set; }

        public string AssemblyName { get; set; }

        public Assembly Assembly { get; set; }

        public List<AssemblyNameSpaceInfo> NameSpaces { get; set; }

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
    }

    internal class AssemblyNameSpaceInfo : INotifyPropertyChanged
    {
        private bool? isChecked = false;

        public bool? IsChecked
        {
            get
            {
                return this.isChecked;
            }
            set
            {
                if (this.isChecked != value)
                {
                    this.isChecked = value;
                    this.OnPropertyChanged("IsChecked");
                }
            }
        }

        public bool IsInternalCheckStateChange { get; set; }

        public AssemblyInfo AssemblyInfo { get; set; }

        public string NameSpace { get; set; }

        public List<ObjectInfo> Objects { get; set; }

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
    }

    internal class ObjectInfo : INotifyPropertyChanged
    {
        private bool? isChecked = false;

        public bool? IsChecked
        {
            get
            {
                return this.isChecked;
            }
            set
            {
                if (this.isChecked != value)
                {
                    this.isChecked = value;
                    this.OnPropertyChanged("IsChecked");
                }
            }
        }

        public AssemblyNameSpaceInfo NameSpaceInfo { get; set; }

        public bool IsInternalCheckStateChange { get; set; }

        public string NameSpace { get; set; }

        public string FullName { get; set; }

        public string Name { get; set; }

        public Type ObjectType { get; set; }

        public ObjectFields Fields { get; set; }

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
    }

    internal class ObjectFields : List<ObjectField>
    {
    }

    internal class ObjectField
    {
        public string FieldName { get; set; }

        public string TypeName { get; set; }
    }

    internal class CachedObjectInfos : List<ObjectInfo>
    {
    }
   
    internal partial class DataSetClient : ChromelessWindow
    {
        public CachedAssemblyInfos AssmeblyInfo { get; set; }

        public CachedObjectInfos ObjectInfos { get; set; }

        public RDL.DOM.DataSource DataSource { get; set; }

        public RDL.DOM.DataSource CreatedDataSource { get; set; }

        public RDL.DOM.DataSources DataSources { get; set; }

        public RDL.DOM.DataSet DataSet { get; set; }

        public RDL.DOM.DataSets DataSets { get; set; }

        public RDL.DOM.Fields Fields { get; set; }

        internal List<Assembly> Assemblies { get; set; }

        private bool isModifyDataset = false;

        public DataSetClient(RDL.DOM.DataSets dataSets, RDL.DOM.DataSources dataSources, 
                            CachedAssemblyInfos assmeblyInfo, CachedObjectInfos objectInfos,List<Assembly> Assemblies)
        {
            InitializeComponent();

            this.isModifyDataset = false;
            this.AssmeblyInfo = assmeblyInfo;
            this.ObjectInfos = objectInfos;
            this.datasourceComboBox.ItemsSource = this.UpdateDataSources();

            this.DataSets = dataSets;
            this.DataSources = dataSources;
            this.Assemblies = Assemblies;
            this.DataSet = new RDL.DOM.DataSet();

            if (this.DataSources.Count > 0)
            {
                this.DataSource = this.DataSources.First();
            }
            else
            {
                this.DataSource = null;
            }

            int availableDataSetsNamesCount = 1;

            string[] availableDataSetNames = (from dset in this.DataSets
                                              select dset.Name).ToArray<string>();

            foreach (string dsName in availableDataSetNames)
            {
                if (dsName.Equals("DataSet" + (availableDataSetsNamesCount)))
                {
                    availableDataSetsNamesCount++;
                }
            }

            this.DataSet.Name = "DataSet" + (availableDataSetsNamesCount).ToString();
            this.nameTextBox.Text = this.DataSet.Name;

            this.datasetComboBox.SelectionChanged += new SelectionChangedEventHandler(datasetBox_SelectionChanged);
        }

        public DataSetClient(RDL.DOM.DataSet reportDataSet, RDL.DOM.DataSets dataSets, RDL.DOM.DataSources dataSources,
                             CachedAssemblyInfos AssmeblyInfo, CachedObjectInfos objectInfos,List<Assembly> Assemblies)
        {
            this.InitializeComponent();
            this.datasetComboBox.SelectionChanged -= new SelectionChangedEventHandler(datasetBox_SelectionChanged);
            this.DataSet = reportDataSet;
            this.DataSources = dataSources;
            this.DataSets = dataSets;

            this.AssmeblyInfo = AssmeblyInfo;
            this.ObjectInfos = objectInfos;
            this.Assemblies = Assemblies;
            this.isModifyDataset = true;
            this.datasourceComboBox.ItemsSource = this.UpdateDataSources();

            this.DataSource = (from dataSource in this.DataSources
                               where dataSource.Name == this.DataSet.Query.DataSourceName
                               select dataSource).First();

            this.datasourceComboBox.Text = this.DataSource.Name;

            if (this.DataSet.DataSetObject != null)
            {
                var dataSetValues = from objects in this.ObjectInfos
                                    where objects.FullName.Equals(this.DataSet.DataSetObject)
                                    select objects;

                if (dataSetValues.Count() > 0)
                {
                    this.datasetComboBox.SelectedValue = dataSetValues.First().Name;
                }
            }

            this.nameTextBox.Text = this.DataSet.Name;
            this.Loaded += new RoutedEventHandler(DataSetClient_Loaded);
        }

        void DataSetClient_Loaded(object sender, RoutedEventArgs e)
        {
            this.datasetComboBox.SelectionChanged += new SelectionChangedEventHandler(datasetBox_SelectionChanged);
            this.SetItemSource();
        } 

        private void configWizardButton_Click(object sender, RoutedEventArgs e)
        {
            DataSetObjectWizard wizard = new DataSetObjectWizard(this.AssmeblyInfo,this.Assemblies);
            SkinStorage.SetVisualStyle(wizard, SkinStorage.GetVisualStyle(this));
            wizard.Owner = this;

            if (wizard.ShowDialog() == true)
            {
                foreach (var objectInfo in wizard.SelectedObject)
                {
                    if (!this.ObjectInfos.Contains(objectInfo))
                    {
                        this.ObjectInfos.Add(objectInfo);                        
                    }
                }

                this.datasourceComboBox.ItemsSource = this.UpdateDataSources();
            }
        }

        List<string> UpdateDataSources()
        {
            List<string> dataSources = new List<string>();
            var datas = (from objSource in this.ObjectInfos
                        select objSource.NameSpaceInfo.NameSpace).Distinct();

            if (datas.Count() > 0)
            {
                dataSources.AddRange(datas);
            }

            return dataSources;
        }

        private void datasourceBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataSetValues = from objects in this.ObjectInfos
                                where objects.NameSpaceInfo.NameSpace.Equals(this.datasourceComboBox.SelectedItem as string)
                                select objects;

            this.datasetComboBox.DisplayMemberPath = "Name";
            this.datasetComboBox.SelectedValuePath = "Name";
            this.datasetComboBox.ItemsSource = dataSetValues.ToList();
            this.datasetComboBox.UpdateLayout();
            this.datasetComboBox.InvalidateVisual();
        }

        private void datasetBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SetItemSource();
        }

        private void SetItemSource()
        {
            if (this.datasetComboBox.SelectedItem != null)
            {
                this.gridControl.ItemsSource = ((ObjectInfo)this.datasetComboBox.SelectedItem).Fields;
            }
            else
            {
                this.gridControl.ItemsSource = null;
            }
        }

        private void configWizardOKButton_Click(object sender, RoutedEventArgs e)
        {
            if (Common.Util.CheckNameWithRE(this.nameTextBox.Text.ToString()))
            {
                string[] availableDataSetNames = (from dset in this.DataSets
                                                  select dset.Name).ToArray<string>();
                if (Common.Util.CheckNameWithPreviousCollection(this.nameTextBox.Text.ToString(), availableDataSetNames) || isModifyDataset)
                {
                    if (!string.IsNullOrEmpty(this.datasourceComboBox.Text.Trim()) &&
                        this.datasetComboBox.SelectedIndex > -1)
                    {
                        RDL.DOM.Query Query = new RDL.DOM.Query();
                        Query.DataSourceName = this.datasourceComboBox.Text;
                        Query.CommandText = "/* Local Query */";
                        RDL.DOM.Fields Fields = new RDL.DOM.Fields();
                        ObjectInfo selectedObjet = this.datasetComboBox.SelectedItem as ObjectInfo;

                        int dataSourceCount = this.DataSources.Where(dataSource => dataSource.Name.Equals(Query.DataSourceName)).Count();

                        if (dataSourceCount == 0)
                        {
                            this.CreatedDataSource = new RDL.DOM.DataSource();
                            this.CreatedDataSource.Name = Query.DataSourceName;
                        }

                        foreach (var field in selectedObjet.Fields)
                        {
                            RDL.DOM.Field Field = new RDL.DOM.Field();
                            Field.DataField = field.FieldName;
                            Field.Name = field.FieldName;
                            Field.TypeName = field.TypeName;
                            Fields.Add(Field);
                        }

                        this.DataSet.Name = this.nameTextBox.Text;
                        this.DataSet.Query = Query;
                        this.DataSet.Fields = Fields;
                        this.DataSet.DataSetObject = selectedObjet.FullName;
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSelectSource"));
                    }
                }
                else
                {
                    MessageBox.Show("<Name:>\n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxDataSetExist"), SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"), MessageBoxButton.OK);
                }
            }
            else
            {
                MessageBox.Show("<Name:>\n" + SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSpecifyValidName"), SR.GetString(CultureInfo.CurrentUICulture, "titleReportDesigner"), MessageBoxButton.OK);
            }
        }

        private void configWizardCancelButton_Click(object sender,RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
