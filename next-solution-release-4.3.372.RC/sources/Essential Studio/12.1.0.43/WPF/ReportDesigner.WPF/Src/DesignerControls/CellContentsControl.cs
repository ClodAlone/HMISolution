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
using Syncfusion.Windows.Reports.Designer.Controls;

namespace Syncfusion.Windows.Reports.Designer.Controls
{  
    internal class CellContentsControl : Grid
    {
        public TablixRegion tablixRegion;
       
        public CellContents cellContents;

        public TablixControl TablixControl { get; set; }
        
        public UIElement Content
        {
            get { return (UIElement)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(UIElement), typeof(CellContentsControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnContentChanged)));

        static CellContentsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CellContentsControl), new FrameworkPropertyMetadata(typeof(CellContentsControl)));
        }

        public CellContentsControl(TablixControl tablixControl)
        {
            this.Visibility =System.Windows.Visibility.Visible;
            this.TablixControl = tablixControl;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CellContentsControl cell = (CellContentsControl)d;

            if (e.NewValue is IReportItemControl)
            {
                (e.NewValue as IReportItemControl).Panel = cell.TablixControl.Panel;
            }

            if (e.NewValue != null)
            {
                cell.Children.Clear();
                cell.Children.Add(e.NewValue as UIElement);
            }

            else
            {
                cell.Children.Clear();
            }
        }
    }
}
