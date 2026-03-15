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
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;

namespace Syncfusion.Windows.Tools.Olap
{
    public class ValueChangedEventArgs : EventArgs
    {
        public ValueChangedEventArgs()
        {
            this.NewValue = string.Empty;
        }

        public ValueChangedEventArgs(string newValue)
        {
            this.NewValue = newValue;
        }
        public string NewValue { get; set; }
    }

    public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs e);

    public class SubsetFilter : ComboBox
    {

        static SubsetFilter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SubsetFilter),
                new FrameworkPropertyMetadata(typeof(SubsetFilter)));
        }
        bool isValueChanged = false;

        public TextBox ValueBox { get; set; }

        public Popup PopupControl { get; set; }

        public event ValueChangedEventHandler ValueChanged;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ValueBox = GetTemplateChild("PART_ValueBox") as TextBox;
            ValueBox.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(ValueBox_PreviewKeyDown);
            this.PopupControl = GetTemplateChild("PART_Popup") as Popup;
            if (this.ValueBox != null)
            {
                this.ValueBox.TextChanged += (object sender, TextChangedEventArgs e) =>
                    {
                        isValueChanged = true;
                    };
            }
            if (this.PopupControl != null)
            {
                this.PopupControl.Opened += (object sender, EventArgs e) =>
                {
                    this.isValueChanged = false;
                };
                this.PopupControl.Closed += (object sender, EventArgs e) =>
                {
                    if (this.ValueChanged != null && isValueChanged)
                    {
                        this.ValueChanged(sender, new ValueChangedEventArgs(this.ValueBox.Text));
                    }
                };
            }
        }

        void ValueBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (((Convert.ToInt16(e.Key) > 33 && Convert.ToInt16(e.Key) < 44) || 
                (Convert.ToInt16(e.Key) > 73 && Convert.ToInt16(e.Key) < 84) ||
                (e.Key == System.Windows.Input.Key.Back || e.Key== System.Windows.Input.Key.Delete || 
                e.Key== System.Windows.Input.Key.Left || e.Key== System.Windows.Input.Key.Right) || e.Key== System.Windows.Input.Key.Enter) && 
                (System.Windows.Input.Keyboard.Modifiers != System.Windows.Input.ModifierKeys.Shift))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

    }
}
