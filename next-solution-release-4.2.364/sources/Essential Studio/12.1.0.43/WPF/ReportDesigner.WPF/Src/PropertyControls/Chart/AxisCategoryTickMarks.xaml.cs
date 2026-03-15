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
    /// Interaction logic for AxisCategoryTickMarks.xaml
    /// </summary>
    internal partial class AxisCategoryTickMarks : UserControl
    {
        public AxisCategoryTickMarks()
        {
            InitializeComponent();            
        }

        #region Public Properties

        public ExpressionComboBox HideCategoryMajorTick
        {
            get
            {
                return this.cmb_HideMajorTick;
            }
            set
            {
                this.cmb_HideMajorTick = value;
            }
        }

        public ExpressionComboBox CategoryMajorTickStyle
        {
            get
            {
                return this.cmb_MajorStyle;
            }
            set
            {
                this.cmb_MajorStyle = value;
            }
        }

        public CustomUIEditorDropDown CategoryMajorTickColor
        {
            get
            {
                return this.clrpkr_MajorColor;
            }
            set
            {
                this.clrpkr_MajorColor = value;
            }
        }

        public ExpressionComboBox CategoryMajorTickLength
        {
            get
            {
                return this.updwn_MajorLength;
            }
            set
            {
                this.updwn_MajorLength = value;
            }
        }

        public ExpressionComboBox CategoryMajorTickWidth
        {
            get
            {
                return this.updwn_MajorWidth;
            }
            set
            {
                this.updwn_MajorWidth = value;
            }
        }

        public ExpressionComboBox HideCategoryMinorTick
        {
            get
            {
                return this.cmb_HideMinorTick;
            }
            set
            {
                this.cmb_HideMinorTick = value;
            }
        }

        //public ComboBox CategoryMinorTickStyle
        //{
        //    get
        //    {
        //        return this.cmb_MinorStyle;
        //    }
        //    set
        //    {
        //        this.cmb_MinorStyle = value;
        //    }
        //}

        //public ColorPicker CategoryMinorTickColor
        //{
        //    get
        //    {
        //        return this.clrpkr_MinorColor;
        //    }
        //    set
        //    {
        //        this.clrpkr_MinorColor = value;
        //    }
        //}

        public ExpressionComboBox CategoryMinorTickLength
        {
            get
            {
                return this.updwn_MinorLength;
            }
            set
            {
                this.updwn_MinorLength = value;
            }
        }

        //public UpDown CategoryMinorTickWidth
        //{
        //    get
        //    {
        //        return this.updwn_MinorWidth;
        //    }
        //    set
        //    {
        //        this.updwn_MinorWidth = value;
        //    }
        //}

        #endregion
        
    }
}
