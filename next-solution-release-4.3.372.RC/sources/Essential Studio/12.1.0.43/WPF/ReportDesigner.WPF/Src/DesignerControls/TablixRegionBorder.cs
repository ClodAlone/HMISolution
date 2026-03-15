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
using Syncfusion.Windows.Reports.Designer.Dialogs;

namespace Syncfusion.Windows.Reports.Designer.Controls
{   
    internal class TablixRegionBorder : DockPanel
    {
        public TablixRegionBorderType tablixRegionBorderType;
        private Border whiteBorder;
        private Border blueBorder;

        public TablixRegionBorder()
        {
            tablixRegionBorderType = TablixRegionBorderType.Top;
            createBorder();
        }

        public TablixRegionBorder(TablixRegionBorderType type)
        {
            tablixRegionBorderType = type;
            createBorder();
        }

        public void createBorder()
        {
            this.whiteBorder = new Border();
            this.blueBorder = new Border();
            if (tablixRegionBorderType == TablixRegionBorderType.Left)
            {
                this.whiteBorder.BorderThickness = new Thickness(2, 0, 0, 0);
                this.blueBorder.BorderThickness = new Thickness(1, 0, 0, 0);
                DockPanel.SetDock(whiteBorder, Dock.Left);
                DockPanel.SetDock(blueBorder, Dock.Left);
            }
            else if (tablixRegionBorderType == TablixRegionBorderType.Top)
            {
                this.whiteBorder.BorderThickness = new Thickness(0, 2, 0, 0);
                this.blueBorder.BorderThickness = new Thickness(0, 1, 0, 0);
                DockPanel.SetDock(whiteBorder, Dock.Top);
                DockPanel.SetDock(blueBorder, Dock.Top);
            }
            else if (tablixRegionBorderType == TablixRegionBorderType.Right)
            {
                this.whiteBorder.BorderThickness = new Thickness(0, 0, 2, 0);
                this.blueBorder.BorderThickness = new Thickness(0, 0, 1, 0);
                DockPanel.SetDock(whiteBorder, Dock.Right);
                DockPanel.SetDock(blueBorder, Dock.Right);
            }
            else if (tablixRegionBorderType == TablixRegionBorderType.Bottom)
            {
                this.whiteBorder.BorderThickness = new Thickness(0, 0, 0, 2);
                this.blueBorder.BorderThickness = new Thickness(0, 0, 0, 1);
                DockPanel.SetDock(whiteBorder, Dock.Bottom);
                DockPanel.SetDock(blueBorder, Dock.Bottom);
            }
            this.whiteBorder.BorderBrush = Brushes.Transparent;
            this.blueBorder.BorderBrush = Brushes.LightGray;
            this.Children.Add(whiteBorder);
            this.Children.Add(blueBorder);
        }

        static TablixRegionBorder()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TablixRegionBorder), new FrameworkPropertyMetadata(typeof(TablixRegionBorder)));
        }
    }
}
