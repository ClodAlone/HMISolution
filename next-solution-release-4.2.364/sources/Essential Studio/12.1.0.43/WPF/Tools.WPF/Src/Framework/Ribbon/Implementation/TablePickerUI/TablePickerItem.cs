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
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Windows.Tools.Controls
{
    public class TablePickerItem : Control
    {
        public TablePickerItem()
        {
            DefaultStyleKey = typeof(TablePickerItem);
        }

        public int Row { get; set; }

        public int Column { get; set; }

        private Border border;

        internal TablePickerUI ParentPicker;



        public bool IsValidated
        {
            get { return (bool)GetValue(IsValidatedProperty); }
            set { SetValue(IsValidatedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsValidated.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsValidatedProperty =
            DependencyProperty.Register("IsValidated", typeof(bool), typeof(TablePickerItem), new PropertyMetadata(false));


        public override void OnApplyTemplate()
        {
            border = GetTemplateChild("border") as Border;
            if (border != null)
            {
                border.MouseEnter += new MouseEventHandler(border_MouseEnter);
                border.MouseMove += new MouseEventHandler(border_MouseMove);
            }
        }

        void border_MouseMove(object sender, MouseEventArgs e)
        {
            if (ParentPicker != null)
            {
                ParentPicker.ValidateCellInfo(new TableInfo() { Row = Row, Column = Column });
            }
        }

        void border_MouseEnter(object sender, MouseEventArgs e)
        {
            if (ParentPicker != null)
            {
                ParentPicker.ValidateCellInfo(new TableInfo() { Row = Row, Column = Column });
            }
        }

        
        
    }
}
