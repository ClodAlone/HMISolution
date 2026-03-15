#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
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
using Microsoft.Windows.Design.Model;


namespace Syncfusion.Grid.WPF.VisualStudio.Design
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ViewDesign
        : UserControl
    {
        public ModelItem selectedControl;
        public DesignerModel Model;
        public ViewDesign()
        {
            this.InitializeComponent();
        }

        public void SetModel(DesignerModel model)
        {
            Model = model;
            this.DataContext = Model;
        }
        
        private void BasicPropertie_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            switch (btn.Content.ToString())
            {
                case "Basic Properties":
                    BasicProp.Visibility = Visibility.Visible;
                    VisibleColn.Visibility = Visibility.Collapsed;
                    break;
                case "Visible Columns":
                    BasicProp.Visibility = Visibility.Collapsed;
                    VisibleColn.Visibility = Visibility.Visible;
                    break;
            }
        }
    }
}