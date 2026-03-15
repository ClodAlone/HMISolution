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
using System.ComponentModel;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for ConfigurationWizard.xaml
    /// </summary>
    internal partial class DataSetObjectWizard : ChromelessWindow
    {
        public CachedAssemblyInfos AssmeblyInfo { get; set; }

        public List<ObjectInfo> SelectedObject { get; set; }

        public List<Assembly> Assemblies { get; set; }

        public DataSetObjectWizard(CachedAssemblyInfos assmeblyInfo,List<Assembly> assemblies)
        {
            InitializeComponent();
            this.AssmeblyInfo = assmeblyInfo;
            this.Assemblies = new List<Assembly>();
            
            if (assemblies != null)
            {
                this.Assemblies.AddRange(assemblies);
            }

            this.Loaded += new RoutedEventHandler(DataSetObjectWizard_Loaded);
        }

        void DataSetObjectWizard_Loaded(object sender, RoutedEventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_UnSelectItems);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_UnSelectItemsCompleted);
            this.busyIndicator.Delay = new TimeSpan(0);
            this.busyIndicator.IsBusy = false;
            worker.RunWorkerAsync();
        }

        void worker_UnSelectItems(object sender, DoWorkEventArgs e)
        {
            foreach (var assembly in this.AssmeblyInfo)
            {
                this.CheckAssembly(assembly,false);
            }
        }

        void CheckAssembly(AssemblyInfo assembly,bool checkVal)
        {
            assembly.IsInternalCheckStateChange = true;
            assembly.IsChecked = checkVal;

            foreach (var nameSpace in assembly.NameSpaces)
            {
                this.CheckNameSpace(nameSpace,checkVal);                
            }

            assembly.IsInternalCheckStateChange = false;
        }

        void CheckNameSpace(AssemblyNameSpaceInfo nameSpace, bool checkVal)
        {
            nameSpace.IsInternalCheckStateChange = true;
            nameSpace.IsChecked = checkVal;

            foreach (var objectVal in nameSpace.Objects)
            {
                this.CheckObject(objectVal, checkVal);
            }

            nameSpace.IsInternalCheckStateChange = false;
        }

        void CheckObject(ObjectInfo objectVal, bool checkVal)
        {
            objectVal.IsInternalCheckStateChange = true;
            objectVal.IsChecked = checkVal;
            objectVal.IsInternalCheckStateChange = false;
        }

        void worker_UnSelectItemsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.DataContext = this.AssmeblyInfo;
            this.LoadAssemblies(this.Assemblies);            
        }

        void LoadAssemblies(List<Assembly> assemblies)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            this.busyIndicator.IsBusy = false;
            worker.RunWorkerAsync(assemblies);
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)        
        {
            this.AssmeblyInfo.AddRange(e.Result as List<AssemblyInfo>);
            this.busyIndicator.IsBusy = false;
            this.referenceList.ItemsSource = this.AssmeblyInfo;
            this.referenceList.UpdateLayout();
            this.referenceList.InvalidateVisual();
            this.referenceList.Items.Refresh();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<AssemblyInfo> assemblyInfos = new List<AssemblyInfo>();
            List<Assembly> assmeblies = (List<Assembly>) e.Argument;

            foreach (var assembly in assmeblies)
            {
                var count = (from assemInfo in this.AssmeblyInfo
                             where assemInfo.AssemblyName.Equals(assembly.FullName)
                             select assemInfo).Count();

                if (count == 0)
                {
                    assemblyInfos.Add(this.GetInfo(assembly));
                }
            }

            e.Result = assemblyInfos;
        }

        private AssemblyInfo GetInfo(Assembly assembly)
        {
            AssemblyInfo info = new AssemblyInfo();
            info.AssemblyName = assembly.FullName;
            info.NameSpaces = new List<AssemblyNameSpaceInfo>();

            foreach (var classValue in assembly.GetTypes())
            {
                var nameSpace = from nameInfo in info.NameSpaces
                                where nameInfo.NameSpace.Equals(classValue.Namespace)
                             select nameInfo;

                if (nameSpace.Count() == 0 && classValue.Namespace != null)
                {
                    AssemblyNameSpaceInfo nameSpaceInfo = new AssemblyNameSpaceInfo();
                    nameSpaceInfo.AssemblyInfo = info;
                    nameSpaceInfo.NameSpace = classValue.Namespace;
                    nameSpaceInfo.Objects = new List<ObjectInfo>();
                    ObjectInfo objinfo = this.GetInfo(classValue);
                    nameSpaceInfo.Objects.Add(objinfo);
                    objinfo.NameSpaceInfo = nameSpaceInfo;
                    info.NameSpaces.Add(nameSpaceInfo);
                }
                else if (classValue.Namespace != null)
                {
                    ObjectInfo objinfo = this.GetInfo(classValue);
                    objinfo.NameSpaceInfo = nameSpace.First();
                    nameSpace.First().Objects.Add(objinfo);
                }
            }

            return info;
        }

        private ObjectInfo GetInfo(Type type)
        {
            ObjectInfo info = new ObjectInfo();
            info.FullName = type.FullName;
            info.Name = type.Name;
            info.Fields = new ObjectFields();

            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var propInfo in propertyInfos)
            {
                ObjectField field = new ObjectField();
                field.FieldName = propInfo.Name;
                field.TypeName = propInfo.PropertyType.ToString();
                info.Fields.Add(field);
            }

            if (info.Fields.Count >1)
            {
                info.Fields.Sort(delegate(ObjectField field1, ObjectField field2)
                        { 
                            return field1.FieldName.CompareTo(field2.FieldName);
                        });
            }

            return info;
        }

        private void FileDialog(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog Dialog = new Microsoft.Win32.OpenFileDialog();
            Dialog.Filter = "DLL|*.dll|Executable|*.exe";             
            Dialog.ShowDialog();

            if (Dialog.FileName != string.Empty && Dialog.FileName != null)
            {
                Assembly assembly = Assembly.LoadFile(Dialog.FileName);
                List<Assembly> assemblies = new List<Assembly>();
                assemblies.Add(assembly);
                this.LoadAssemblies(assemblies);
            }
        }

        private void finishWizard_Click(object sender, RoutedEventArgs e)
        {
            this.SelectedObject = new List<ObjectInfo>();

            foreach (var assembly in this.AssmeblyInfo)
            {
                foreach (var nameSpace in assembly.NameSpaces)
                {
                    foreach (var objectVal in nameSpace.Objects)
                    {
                        if (objectVal.IsChecked == true)
                        {
                            this.SelectedObject.Add(objectVal);
                        }
                    }
                }
            }

            if (this.SelectedObject.Count > 0)
            {
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSelectObject"));
            }
        }

        private void cancelWizard_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            object dataValue = ((CheckBox)sender).DataContext;

            if (dataValue is AssemblyInfo)
            {
                var assembly = dataValue as AssemblyInfo;

                if (!assembly.IsInternalCheckStateChange)
                {
                    this.CheckAssembly(assembly, true);
                }
            }
            else if (dataValue is AssemblyNameSpaceInfo)
            {
                var nameSpace = dataValue as AssemblyNameSpaceInfo;

                if (!nameSpace.IsInternalCheckStateChange)
                {
                    this.CheckNameSpace(nameSpace, true);
                    nameSpace.AssemblyInfo.IsInternalCheckStateChange = true;
                    nameSpace.AssemblyInfo.IsChecked = null;
                    nameSpace.AssemblyInfo.IsInternalCheckStateChange = false;
                }
            }
            else if (dataValue is ObjectInfo)
            {
                var objectInfo = dataValue as ObjectInfo;

                if (!objectInfo.IsInternalCheckStateChange)
                {
                    objectInfo.NameSpaceInfo.IsInternalCheckStateChange = true;
                    objectInfo.NameSpaceInfo.IsChecked = null;
                    objectInfo.NameSpaceInfo.IsInternalCheckStateChange = false;
                    objectInfo.NameSpaceInfo.AssemblyInfo.IsInternalCheckStateChange = true;
                    objectInfo.NameSpaceInfo.AssemblyInfo.IsChecked = null;
                    objectInfo.NameSpaceInfo.AssemblyInfo.IsInternalCheckStateChange = false;
                }
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            object dataValue = ((CheckBox)sender).DataContext;

            if (dataValue is AssemblyInfo)
            {
                var assembly = dataValue as AssemblyInfo;

                if (!assembly.IsInternalCheckStateChange)
                {
                    this.CheckAssembly(assembly, false);
                }
            }
            else if (dataValue is AssemblyNameSpaceInfo)
            {
                var nameSpace = dataValue as AssemblyNameSpaceInfo;

                if (!nameSpace.IsInternalCheckStateChange)
                {
                    this.CheckNameSpace(nameSpace, false);
                    nameSpace.AssemblyInfo.IsInternalCheckStateChange = true;
                    nameSpace.AssemblyInfo.IsChecked = null;
                    nameSpace.AssemblyInfo.IsInternalCheckStateChange = false;
                }
            }
            else if (dataValue is ObjectInfo)
            {
                var objectInfo = dataValue as ObjectInfo;

                if (!objectInfo.IsInternalCheckStateChange)
                {
                    objectInfo.NameSpaceInfo.IsInternalCheckStateChange = true;
                    objectInfo.NameSpaceInfo.IsChecked = null;
                    objectInfo.NameSpaceInfo.IsInternalCheckStateChange = false;
                    objectInfo.NameSpaceInfo.AssemblyInfo.IsInternalCheckStateChange = true;
                    objectInfo.NameSpaceInfo.AssemblyInfo.IsChecked = null;
                    objectInfo.NameSpaceInfo.AssemblyInfo.IsInternalCheckStateChange = false;
                }
            }
        }
    }
}