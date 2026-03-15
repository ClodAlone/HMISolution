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
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Reports.Designer.Editors;

namespace Syncfusion.Windows.Reports.Designer.Dialogs
{
    /// <summary>
    /// Interaction logic for Background.xaml
    /// </summary>
    internal partial class ChartBackground : UserControl
    {
        public ChartBackground()
        {
            InitializeComponent();
        }

        #region Public Properties

        public CustomUIEditorDropDown ChartBackgroundFill
        {
            get
            {
                return this.clrpkr_ChartBackground;
            }

            set
            {
                this.clrpkr_ChartBackground = value;
            }
        }

        //public ColorPicker Interior2
        //{
        //    get
        //    {
        //        return this.clrpkr_ChartInterior2;
        //    }

        //    set
        //    {
        //        this.clrpkr_ChartInterior2 = value;
        //    }
        //}

        //public ComboBox GradientType
        //{
        //    get
        //    {
        //        return this.cmb_GradientType;
        //    }

        //    set
        //    {
        //        this.cmb_GradientType = value;
        //    }
        //}

        #endregion
    }
}
