#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using Syncfusion.Windows.Documents;

    partial class GridDataControl : IDisposable
    {
        private void EnsurePrintProperties()
        {
            var printPaginator = this.InternalGrid as IGridPrintPaginator;
            if (this.isPrintFooterHeightLoaded)
            {
                printPaginator.PrintFooterHeight = this.PrintFooterHeight;
            }

            if (this.isPrintFooterTemplateLoaded)
            {
                printPaginator.PrintFooterTemplate = this.PrintFooterTemplate;
            }

            if (this.isPrintHeaderHeightLoaded)
            {
                printPaginator.PrintHeaderHeight = this.PrintHeaderHeight;
            }

            if (this.isPrintHeaderTemplateLoaded)
            {
                printPaginator.PrintHeaderTemplate = this.PrintHeaderTemplate;
            }

            if (this.isPrintPageMarginLoaded)
            {
                printPaginator.PrintPageMargin = this.PrintPageMargin;
            }

        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.PrintHeaderTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty PrintHeaderTemplateProperty = DependencyProperty.Register("PrintHeaderTemplate", typeof(DataTemplate), typeof(GridDataControl), new FrameworkPropertyMetadata(OnPrintHeaderTemplateChanged));

        private static void OnPrintHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue == null)
            {
                return;
            }

            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                var printPaginator = grid.InternalGrid as IGridPrintPaginator;
                printPaginator.PrintHeaderTemplate = (DataTemplate)args.NewValue;
            }
            else
            {
                grid.isPrintHeaderTemplateLoaded = true;
            }
        }

        private bool isPrintHeaderTemplateLoaded = false;

        /// <summary>
        /// Gets or sets the print header template.
        /// </summary>
        /// <value>The print header template.</value>
        public DataTemplate PrintHeaderTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(GridDataControl.PrintHeaderTemplateProperty);
            }

            set
            {
                this.SetValue(GridDataControl.PrintHeaderTemplateProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="PrintFooterTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty PrintFooterTemplateProperty = DependencyProperty.Register("PrintFooterTemplate", typeof(DataTemplate), typeof(GridDataControl), new FrameworkPropertyMetadata(OnPrintFooterTemplateChanged));

        private static void OnPrintFooterTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                var printPaginator = grid.InternalGrid as IGridPrintPaginator;
                printPaginator.PrintFooterTemplate = (DataTemplate)args.NewValue;
            }
            else
            {
                grid.isPrintFooterTemplateLoaded = true;
            }
        }

        private bool isPrintFooterTemplateLoaded = false;

        /// <summary>
        /// Gets or sets the print footer template.
        /// </summary>
        /// <value>The print footer template.</value>
        public DataTemplate PrintFooterTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(GridDataControl.PrintFooterTemplateProperty);
            }

            set
            {
                this.SetValue(GridDataControl.PrintFooterTemplateProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.PrintHeaderHeight"/> property.
        /// </summary>
        public static readonly DependencyProperty PrintHeaderHeightProperty = DependencyProperty.Register("PrintHeaderHeight", typeof(double), typeof(GridDataControl), new FrameworkPropertyMetadata(OnPrintHeaderHeightChanged));

        private static void OnPrintHeaderHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                var printPaginator = grid.InternalGrid as IGridPrintPaginator;
                printPaginator.PrintHeaderHeight = (double)args.NewValue;
            }
            else
            {
                grid.isPrintHeaderHeightLoaded = true;
            }
        }

        private bool isPrintHeaderHeightLoaded = false;

        /// <summary>
        /// Gets or sets the height of the print header.
        /// </summary>
        /// <value>The height of the print header.</value>
        public double PrintHeaderHeight
        {
            get
            {
                return (double)this.GetValue(GridDataControl.PrintHeaderHeightProperty);
            }

            set
            {
                this.SetValue(GridDataControl.PrintHeaderHeightProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.PrintFooterHeight"/> property.
        /// </summary>
        public static readonly DependencyProperty PrintFooterHeightProperty = DependencyProperty.Register("PrintFooterHeight", typeof(double), typeof(GridDataControl), new FrameworkPropertyMetadata(OnPrintFooterHeightChanged));

        private static void OnPrintFooterHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                var printPaginator = grid.InternalGrid as IGridPrintPaginator;
                printPaginator.PrintFooterHeight = (double)args.NewValue;
            }
            else
            {
                grid.isPrintFooterHeightLoaded = true;
            }
        }

        private bool isPrintFooterHeightLoaded = false;
        /// <summary>
        /// Gets or sets the height of the print footer.
        /// </summary>
        /// <value>The height of the print footer.</value>
        public double PrintFooterHeight
        {
            get
            {
                return (double)this.GetValue(GridDataControl.PrintFooterHeightProperty);
            }

            set
            {
                this.SetValue(GridDataControl.PrintFooterHeightProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for <see cref="GridDataControl.PrintPageMargin"/> property.
        /// </summary>
        
        public static readonly DependencyProperty PrintPageProperty =DependencyProperty.Register("PrintPageMargin", typeof(Thickness), typeof(GridDataControl), new FrameworkPropertyMetadata(OnPrintPageMarginChanged));

        /// <summary>
        /// Gets or sets the margin of the printpage.
        /// </summary>
        public Thickness PrintPageMargin
        {
            get { return (Thickness)this.GetValue(GridDataControl.PrintPageProperty); }
            set { SetValue(GridDataControl.PrintPageProperty, value); }
        }
        private bool isPrintPageMarginLoaded = false;
        private static void OnPrintPageMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var grid = d as GridDataControl;
            if (grid.isGridLoaded)
            {
                var printPaginator = grid.InternalGrid as IGridPrintPaginator;
                printPaginator.PrintPageMargin = (Thickness)args.NewValue;
            }
            else
            {
                grid.isPrintPageMarginLoaded = true;
            }
        }

        void IDisposable.Dispose()
        {
            this.Dispose();
        }
    }
}
