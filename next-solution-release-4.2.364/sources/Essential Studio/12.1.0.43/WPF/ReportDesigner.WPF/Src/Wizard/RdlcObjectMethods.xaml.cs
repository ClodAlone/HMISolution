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
using System.Reflection;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.ReportDesigner.Resources;
using System.Globalization;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for RdlcObjectMethods.xaml
    /// </summary>
    internal partial class RdlcObjectMethods : ChromelessWindow
    {
        Type Types;
        List<Type> typeList;
        BindingFlags flags;
        List<Assembly> assemblies;
        Assembly selectionAssembly;
        MethodInfo methodSelect;

        public string MethodName { get; set; }

        public object Value { get; set; }

        public string TableName { get; set; }

        public string DataSetName { get; set; }

        public string ObjectDataSourceSelectMethod { get; set; }

        public string ObjectDataSourceType { get; set; }

        public string ObjectDataSourceSelectMethodSignature { get; set; }

        public RdlcObjectMethods(List<Assembly> assembly)
        {
            InitializeComponent();

            this.assemblies = assembly;
            flags = (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            this.Loaded += new RoutedEventHandler(RdlcObjectMethods_Loaded);
            this.Title = Syncfusion.Windows.ReportDesigner.Resources.SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "titleDataSetProperties");
        }

        void RdlcObjectMethods_Loaded(object sender, RoutedEventArgs e)
        {
            typeList = new List<Type>();
            foreach (var assembly in this.assemblies)
            {
                typeList.AddRange(assembly.GetTypes());
                string nameSpace = null;
                foreach (Type name in typeList)
                {
                    if (!(nameSpace == name.Namespace))
                    {
                        nameSpace = name.Namespace;
                        NameSpaceCombo.Items.Add(name.Namespace);
                    }
                }
            }
        }

        private void SelectOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((Methodscombo.SelectedItem != null))
                {
                    methodSelect = Types.GetMethod(Methodscombo.SelectedValue.ToString(), flags);
                    try
                    {
                        ConstructorInfo Constructor = Types.GetConstructor(Type.EmptyTypes);
                        object ClassObject = Constructor.Invoke(null);
                        Value = methodSelect.Invoke(ClassObject, null);
                        if (Value != null)
                        {
                            this.ObjectDataSourceSelectMethod = Methodscombo.SelectedValue.ToString();
                            this.TableName = Types.Name;
                            this.DataSetName = Types.Assembly.FullName.Split(',')[0];
                            this.ObjectDataSourceSelectMethodSignature = methodSelect.ToString();
                            string str = Types.Assembly.FullName;
                            str = str.Replace(str.Split(',')[0], Types.FullName);
                            this.ObjectDataSourceType = str;
                            MethodName = Types.FullName + "." + Methodscombo.SelectedValue.ToString();
                            this.DialogResult = true;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show(methodSelect.Name + " returns null");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show(SR.GetString(CultureInfo.CurrentUICulture, "msgBoxSelectFields"), SR.GetString(CultureInfo.CurrentUICulture, "titleEmptySelection"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
        } 

        private void NameSpaceCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Methodscombo.Items.Clear();
            ClassCombo.Items.Clear();
            foreach (Type type in typeList)
            {
                if (type.Namespace == (NameSpaceCombo.SelectedValue.ToString()))
                {
                    ClassCombo.Items.Add(type);
                    this.selectionAssembly = type.Assembly;
                }
            }
        }

        private void ClassCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ClassCombo.SelectedItem != null && this.selectionAssembly != null)
            {
                Methodscombo.Items.Clear();
                Types = this.selectionAssembly.GetType(ClassCombo.SelectedValue.ToString());
                MethodInfo[] method = Types.GetMethods(flags);
                foreach (MethodInfo info in method)
                {
                    Methodscombo.Items.Add(info.Name);
                }
            }
        }

        private void Methodscombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SignatureText.Text="";
            if (this.Methodscombo.SelectedItem != null)
            {
                try
                {
                    methodSelect = Types.GetMethod(Methodscombo.SelectedValue.ToString(), flags);
                    ParameterInfo[] parameters = methodSelect.GetParameters();
                    string parameterString = null;
                    foreach (ParameterInfo infoparam in parameters)
                    {
                        parameterString = "<" + infoparam.ParameterType + ">" + infoparam.Name + " ";
                    }
                    SignatureText.Text = methodSelect.Name + "(" + parameterString + ")" + "\t returns\n" + methodSelect.ReturnType;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
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
