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
using System.Collections.Generic;

namespace Syncfusion.Windows.Tools.Controls
{
    public class TableCellStyleHistoryInfo :HistoryInfo
    {
        Color cellbackground;
        Color bordercolor;
        double thickness;
        List<TableCellAdv> selected;
        TableAdv table;

        public TableCellStyleHistoryInfo()
        {
            selected = new List<TableCellAdv>();
        }

        public Color TableCellBackground
        {
            get
            {
                return cellbackground;
            }
            set
            {
                cellbackground = value;
            }
        }

        public Color BorderColor
        {
            get
            {
                return bordercolor;
            }
            set
            {
                bordercolor = value;
            }
        }

        public double BorderThickness
        {
            get
            {
                return thickness;
            }
            set
            {
                thickness = value;
            }
        }

        public List<TableCellAdv> SelectedCells
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
            }
        }

        public TableAdv Table
        {
            get
            {
                return table;
            }
            set
            {
                table = value;
            }
        }
    }
}
