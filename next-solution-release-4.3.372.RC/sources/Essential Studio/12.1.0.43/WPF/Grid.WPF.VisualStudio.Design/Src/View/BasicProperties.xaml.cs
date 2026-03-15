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

namespace Syncfusion.Grid.WPF.VisualStudio.Design
{
    /// <summary>
    /// Interaction logic for BasicProperties.xaml
    /// </summary>
    public partial class BasicProperties : UserControl
    {
        public BasicProperties()
        {
            InitializeComponent();
        }

        private Expander expander;

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            if (expander != null && expander != ((Expander)sender))
            {
                expander.IsExpanded = false;
            }

            expander = sender as Expander;
        }
    }
}
