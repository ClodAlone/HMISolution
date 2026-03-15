#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Windows.Controls.Spreadsheet
{
    [DesignTimeVisible(false)]
    public class PasteDropDownItem : Control
    {
        SplitButtonAdv SplitButton;
        public PasteDropDownItem()
        {
            DefaultStyleKey = typeof(PasteDropDownItem);
        }

        public PasteDropDownItem(IPasteOptionChanged ChangedHandler)
            : this()
        {
            PasteOptionChangedHandler = ChangedHandler;
        }

        public IPasteOptionChanged PasteOptionChangedHandler { get; set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SplitButton = this.GetTemplateChild("dropdownSplitter") as SplitButtonAdv;
            RadioButton frb = this.GetTemplateChild("FormulaButton") as RadioButton;
            RadioButton vrb = this.GetTemplateChild("ValueButton") as RadioButton;
            if (frb != null)
                frb.Checked += RadioButtonChecked;
            if (vrb != null)
                vrb.Checked += RadioButtonChecked;
        }

        void RadioButtonChecked(object sender, RoutedEventArgs e)
        {
            if (SplitButton != null)
                SplitButton.IsDropDownOpen = false;
            if (PasteOptionChangedHandler != null)
                PasteOptionChangedHandler.PasteOptionChanged((sender as RadioButton).Content.ToString());
        }
    }
}
