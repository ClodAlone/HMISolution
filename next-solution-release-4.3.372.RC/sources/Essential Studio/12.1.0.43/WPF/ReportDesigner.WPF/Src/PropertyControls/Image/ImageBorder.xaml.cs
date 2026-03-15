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
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Reports.Designer.Editors;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for ImageBorder.xaml
    /// </summary>
    internal partial class ImageBorder : UserControl
    {
        public ImageBorder()
        {
            InitializeComponent();
            //this.updwn_BorderThickness.MinValue = 0;
        }

        #region Public Properties

        public CustomUIEditorDropDown drColor
        {
            get
            {
                return this.clrpkr_BorderColor;
            }
            set
            {
                this.clrpkr_BorderColor = value;
            }
        }

        public ExpressionComboBox ImgBrdrThick
        {
            get
            {
                return this.updwn_BorderThickness;
            }
            set
            {
                this.updwn_BorderThickness = value;
            }
        }

        #endregion
    }
}
