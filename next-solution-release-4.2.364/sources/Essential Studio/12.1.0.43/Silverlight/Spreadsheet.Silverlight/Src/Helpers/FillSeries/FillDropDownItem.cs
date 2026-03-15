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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Tools.Controls;
using System.ComponentModel;

namespace Syncfusion.Windows.Controls.Spreadsheet
{
    [DesignTimeVisible(false)]
    public class FillDropDownItem : Control
    {
        IFillOptionChanged FillOptionChanged = null;
        SplitButtonAdv SplitButton;
        public FillDropDownItem()
        {
            this.DefaultStyleKey = typeof(FillDropDownItem);
        }

        public FillDropDownItem(IFillOptionChanged ChangedHandler)
            : this()
        {
            this.FillOptionChanged = ChangedHandler;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SplitButton = this.GetTemplateChild("dropdownSplitter") as SplitButtonAdv;
            RadioButton FillSeriesButton = this.GetTemplateChild("FillSeries") as RadioButton;
            RadioButton CopySeriesButton = this.GetTemplateChild("CopySeries") as RadioButton;
            RadioButton FillFormatOnlyButton = this.GetTemplateChild("FillFormatOnly") as RadioButton;
            RadioButton FillWithoutFormatButton = this.GetTemplateChild("FillWithoutFormat") as RadioButton;
            if (FillSeriesButton != null)
                FillSeriesButton.Checked += RadioButtonChecked;
            if (CopySeriesButton != null)
                CopySeriesButton.Checked += RadioButtonChecked;
            if (FillFormatOnlyButton != null)
                FillFormatOnlyButton.Checked += RadioButtonChecked;
            if (FillWithoutFormatButton != null)
                FillWithoutFormatButton.Checked += RadioButtonChecked;
        }

        void RadioButtonChecked(object sender, RoutedEventArgs e)
        {
            if (FillOptionChanged != null)
                FillOptionChanged.FillOptionChanged((sender as RadioButton).Content.ToString());
            SplitButton.IsDropDownOpen = false;
        }
    }
}
