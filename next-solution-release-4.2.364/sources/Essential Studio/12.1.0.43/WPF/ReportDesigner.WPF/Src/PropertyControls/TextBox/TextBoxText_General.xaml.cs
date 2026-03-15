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
using Syncfusion.RDL.DOM;
using Syncfusion.Windows.Reports.Designer.Controls;
using Syncfusion.Windows.Reports.Sql;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for TextBoxText_General.xaml
    /// </summary>
    internal partial class TextBoxText_General : UserControl
    {
         
        #region Constructor
        public TextBoxText_General()
        {
            InitializeComponent();
            this.TextBox = new RDL.DOM.TextBox();
        }

        #endregion

        #region Public Properties
        public Syncfusion.RDL.DOM.TextBox TextBox { get; set; }

        public new System.Windows.Controls.TextBox ToolTip
        {
            get
            {
                return this.txt_ToolTip;
            }
            set
            {
                this.txt_ToolTip = value;
            }
        }

        public System.Windows.Controls.CheckBox AllowHeightIncrease
        {
            get
            {
                return this.chk_AllowHeightIncrease;
            }
            set
            {
                this.chk_AllowHeightIncrease = value;
            }
        }

        public System.Windows.Controls.CheckBox AllowHeightDecrease
        {
            get
            {
                return this.chk_AllowHeightDecrease;
            }
            set
            {
                this.chk_AllowHeightDecrease = value;
            }
        }
        #endregion
    }
    
}
