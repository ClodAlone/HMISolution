#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Windows;

namespace Syncfusion.Windows.Controls.PivotGrid
{
    /// <summary>
    /// This class is used for setting header and footer properties while Printing
    /// </summary>
    public class PrintSettings 
    {
        /// <summary>
        /// Gets the value of the property PrintHeaderProperty
        /// </summary>
        public static bool GetPrintHeader(PivotGridControl element)
        {
            return (bool)element.GetValue(PrintHeaderProperty);
        }
        /// <summary>
        /// Sets the value of the property PrintHeaderProperty
        /// </summary>
        public static void SetPrintHeader(PivotGridControl element, bool value)
        {
            element.SetValue(PrintHeaderProperty, value);
        }
        /// <summary>
        /// Gets the value of the property PrintFooterProperty
        /// </summary>
        public static bool GetPrintFooter(PivotGridControl element)
        {
            return (bool)element.GetValue(PrintFooterProperty);
        }
        /// <summary>
        /// Gets the value of the property PrintFooterProperty
        /// </summary>
        public static void SetPrintFooter(PivotGridControl element, bool value)
        {
            element.SetValue(PrintFooterProperty, value);
        }

        // Using a DependencyProperty as the backing store for PrintHeader.  This enables animation, styling, binding, etc...

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PrintSettings.PrintHeader"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PrintSettings.PrintHeader"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty PrintHeaderProperty =
            DependencyProperty.RegisterAttached("PrintHeader", typeof(bool), typeof(PrintSettings), new UIPropertyMetadata(true,OnPrintHeaderChanged));

        private static void OnPrintHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PivotGridControl pivot = d as PivotGridControl;
            if ((bool)e.NewValue)
                pivot.PrintHeader = true;
            else
                pivot.PrintHeader = false;
        }


        // Using a DependencyProperty as the backing store for PrintFooter.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PrintSettings.PrintFooter"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PrintSettings.PrintFooter"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty PrintFooterProperty =
            DependencyProperty.RegisterAttached("PrintFooter", typeof(bool), typeof(PrintSettings), new UIPropertyMetadata(true, OnPrintFooterChanged));

        private static void OnPrintFooterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PivotGridControl pivot = d as PivotGridControl;
            if ((bool)e.NewValue)
                pivot.PrintFooter = true;
            else
                pivot.PrintFooter = false;
        }

        


    }
}
