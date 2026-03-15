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
    internal class TablixGroupBorder : DockPanel
    {
        public TablixHierarchyType tablixHierarchyType = TablixHierarchyType.TablixRowHierarchy;
        public Border border;
        public Rectangle r1;
        public Rectangle r2;

        public bool GroupFocus
        {
            get { return (bool)GetValue(GroupFocusProperty); }
            set { SetValue(GroupFocusProperty, value); }
        }

        public static readonly DependencyProperty GroupFocusProperty =
            DependencyProperty.Register("GroupFocus", typeof(bool), typeof(TablixGroupBorder),
            new PropertyMetadata(true, OnGroupFocusChanged));

        public TablixGroupBorder()
        {
            createGroupBorder();
        }

        public TablixGroupBorder(TablixHierarchyType type)
        {
            this.tablixHierarchyType = type;
            createGroupBorder();
        }

        public void createGroupBorder()
        {
            border = new Border();
            r1 = new Rectangle();
            r2 = new Rectangle();            
            
            if (tablixHierarchyType == TablixHierarchyType.TablixRowHierarchy)
            {
                this.border.BorderThickness = new Thickness(4, 0, 0, 0);
                r1.StrokeThickness = 4;
                r1.Fill = Brushes.Orange;
                r1.Height = 4;
                r1.Width = 4;
                r1.HorizontalAlignment = HorizontalAlignment.Left;
                r1.VerticalAlignment = System.Windows.VerticalAlignment.Top;

                r2.StrokeThickness = 4;
                r2.Fill = Brushes.Orange;
                r2.Height = 4;
                r2.Width = 4;
                r2.HorizontalAlignment = HorizontalAlignment.Left;
                r2.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;

                DockPanel.SetDock(border, Dock.Left);
                DockPanel.SetDock(r1, Dock.Top);
                DockPanel.SetDock(r2, Dock.Bottom);

                this.Children.Add(border);
                this.Children.Add(r1);
                this.Children.Add(r2);  
            }
            else
            {
                this.border.BorderThickness = new Thickness(0, 4, 0, 0);
                r1.StrokeThickness = 4;
                r1.Fill = Brushes.Orange;
                r1.Height = 4;
                r1.Width = 4;
                r1.HorizontalAlignment = HorizontalAlignment.Left;
                r1.VerticalAlignment = System.Windows.VerticalAlignment.Top;

                r2.StrokeThickness = 4;
                r2.Fill = Brushes.Orange;
                r2.Height = 4;
                r2.Width = 4;
                r2.HorizontalAlignment = HorizontalAlignment.Right;
                r2.VerticalAlignment = System.Windows.VerticalAlignment.Top;

                DockPanel.SetDock(border, Dock.Top);
                DockPanel.SetDock(r1, Dock.Left);
                DockPanel.SetDock(r2, Dock.Right);

                this.Children.Add(border);
                this.Children.Add(r1);
                this.Children.Add(r2);  
                
            }
            this.border.BorderBrush = Brushes.Orange;            
        }


        private static void OnGroupFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TablixGroupBorder)
            {
                TablixGroupBorder groupBorder = d as TablixGroupBorder;
                if (e.NewValue != null && e.NewValue is bool)
                {
                    bool hasGroupFocus = (bool)e.NewValue;
                    if (hasGroupFocus)
                    {
                        groupBorder.border.BorderBrush = Brushes.Orange;
                        groupBorder.r1.Fill = Brushes.Orange;
                        groupBorder.r2.Fill = Brushes.Orange;
                    }
                    else
                    {
                        groupBorder.border.BorderBrush = Brushes.Gray;
                        groupBorder.r1.Fill = Brushes.Gray;
                        groupBorder.r2.Fill = Brushes.Gray;
                    }
                }
            }
        }
    }
}
