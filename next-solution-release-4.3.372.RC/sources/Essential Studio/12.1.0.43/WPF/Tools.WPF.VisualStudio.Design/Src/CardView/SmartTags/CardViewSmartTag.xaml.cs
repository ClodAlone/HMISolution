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
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Tools;
using System.Diagnostics;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Design;

namespace Syncfusion.Tools.WPF.VisualStudio.Design
{
    /// <summary>
    /// Interaction logic for TabSplitter.xaml
    /// </summary>
    public partial class CardViewSmartTag : SmartTagBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckListBoxSmartTag"/> class.
        /// </summary>
        public CardViewSmartTag()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This method is called when the CardViewSmartTag Item template is initialized.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BindTextBox(this.textBox1, "Name");

            BindSelectorWithEnum(this.comboBox1, "Orientation", typeof(Orientation));
            BindCheckBox(this.checkBox1, "ShowHeader");

            BindCheckBox(this.checkBox2, "CanEdit");
            BindCheckBox(this.checkBox3, "CanSort");
            BindCheckBox(this.checkBox4, "CanGroup");
        }
    }
}
