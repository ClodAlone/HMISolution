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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;


namespace Syncfusion.Silverlight.Tools.Controls
{
    /// <summary>
    /// ChildWindow more More Color Options
    /// </summary>
    public partial class ChildWindow1 : ChildWindow
    {
        /// <summary>
        /// Private Variables
        /// </summary>
        double adj;
        double hyp = 10;
        double opp;
        double xp, yp;
        PolygonItem polygontemp;

        /// <summary>
        /// Internal Variables
        /// </summary>
        internal PolygonItem polygonitem;
        internal Binding bind;

        /// <summary>
        ///  Gets or sets the value of the Color dependency property
        /// </summary>
        internal Brush color { get; set; }

        /// <summary>
        /// Creates the instance of ChildWindow class
        /// </summary>
        public ChildWindow1()
        {
            InitializeComponent();
            this.KeyDown += new KeyEventHandler(ChildWindow1KeyDown);
            this.Title = "Colors";
            opp = Math.Sin((30 * Math.PI) / 180) * hyp;
            adj = Math.Cos((30 * Math.PI) / 180) * hyp;
            double x;
            double y;
            bind = new Binding();
            x = 90;
            y = 20;
            morecolorcollection = new ObservableCollection<PolygonItem>();
            morecolorcollection.Add(new PolygonItem() { Points= CalculatePoints(x, y), color = new SolidColorBrush(Color.FromArgb(255, 0, 51, 102)), RowIndex=1, ColumnIndex=1,ColorName="Dark Blue" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (2 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 51, 102, 153)), RowIndex = 1, ColumnIndex = 2 ,ColorName="Blue"});
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (4 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 51, 102, 204)), RowIndex=1, ColumnIndex=3 ,ColorName="Blue"});
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (6 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 0, 51, 153)), RowIndex=1, ColumnIndex=4,ColorName="Dark Blue" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (8 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 0, 0, 153)), RowIndex = 1, ColumnIndex = 5,ColorName="Dark Blue"});
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (10 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 0, 0, 204)), RowIndex = 1, ColumnIndex = 6,ColorName="Blue"});
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (12 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 0, 0, 102)), RowIndex = 1, ColumnIndex = 7,ColorName="Blue"});
            x = x - adj;
            y = y + opp + hyp;
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x, y), color = new SolidColorBrush(Color.FromArgb(255, 0, 102, 102)), RowIndex = 2, ColumnIndex = 1, ColorName = "Dark Teal" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (2 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 0, 102, 153)), RowIndex = 2, ColumnIndex = 2, ColorName = "Dark Teal" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (4 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 0, 153, 204)), RowIndex = 2, ColumnIndex = 3, ColorName = "Turquoise" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (6 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 0, 102, 204)), RowIndex = 2, ColumnIndex = 4, ColorName = "Blue" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (8 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 0, 51, 204)), RowIndex = 2, ColumnIndex = 5, ColorName = "Blue" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (10 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255)), RowIndex = 2, ColumnIndex = 6, ColorName = "Blue" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (12 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 51, 51, 255)), RowIndex = 2, ColumnIndex = 7, ColorName = "Blue" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (14 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 51, 51, 153)), RowIndex = 2, ColumnIndex = 8, ColorName = "Indigo" });
            x = x - adj;
            y = y + opp + hyp;
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x, y), color = new SolidColorBrush(Color.FromArgb(255, 0, 128, 128)), RowIndex = 3, ColumnIndex = 1, ColorName = "Dark Teal" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (2 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 0, 153, 153)), RowIndex = 3, ColumnIndex = 2, ColorName = "Dark Teal" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (4 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 51, 204, 204)), RowIndex = 3, ColumnIndex = 3, ColorName = "Teal" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (6 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 0, 204, 255)), RowIndex = 3, ColumnIndex = 4, ColorName = "Turquoise" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (8 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 0, 153, 255)), RowIndex = 3, ColumnIndex = 5, ColorName = "Blue" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (10 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 0, 102, 255)), RowIndex = 3, ColumnIndex = 6, ColorName = "Blue" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (12 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 51, 102, 255)), RowIndex = 3, ColumnIndex = 7, ColorName = "Blue" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (14 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 51, 51, 204)), RowIndex = 3, ColumnIndex = 8, ColorName = "Blue" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (16 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 102, 102, 153)), RowIndex = 3, ColumnIndex = 9, ColorName = "Indigo" });
            x = x - adj;
            y = y + opp + hyp;
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x, y), color = new SolidColorBrush(Color.FromArgb(255, 51, 153, 102)), RowIndex = 4, ColumnIndex = 1, ColorName = "Green" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (2 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 0, 204, 153)), RowIndex = 4, ColumnIndex = 2, ColorName = "Green" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (4 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 0, 255, 204)), RowIndex = 4, ColumnIndex = 3, ColorName = "Teal" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (6 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 0, 255, 255)), RowIndex = 4, ColumnIndex = 4, ColorName = "Aqua" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (8 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 51, 204, 255)), RowIndex = 4, ColumnIndex = 5, ColorName = "Turquoise" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (10 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 51, 153, 255)), RowIndex = 4, ColumnIndex = 6, ColorName = "Blue" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (12 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 102, 153, 255)), RowIndex = 4, ColumnIndex = 7, ColorName = "Light Blue" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (14 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 102, 102, 255)), RowIndex = 4, ColumnIndex = 8, ColorName = "Light Blue" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (16 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 102, 0, 255)), RowIndex = 4, ColumnIndex = 9 , ColorName = "Purple" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (18 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 102, 0, 204)), RowIndex = 4, ColumnIndex = 10, ColorName = "Purple" });
            x = x - adj;
            y = y + opp + hyp;
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x, y), color = new SolidColorBrush(Color.FromArgb(255, 51, 153, 51)), RowIndex = 5, ColumnIndex = 1, ColorName = "Green" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (2 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 0, 204, 102)), RowIndex = 5, ColumnIndex = 2, ColorName = "Green" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (4 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 0, 255, 153)), RowIndex = 5, ColumnIndex = 3, ColorName = "Green" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (6 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 102, 255, 204)), RowIndex = 5, ColumnIndex = 4, ColorName = "Light Green" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (8 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 102, 255, 255)), RowIndex = 5, ColumnIndex = 5, ColorName = "Sky Blue" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (10 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 102, 204, 255)), RowIndex = 5, ColumnIndex = 6, ColorName = "Light Turquoise" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (12 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 153, 204, 255)), RowIndex = 5, ColumnIndex = 7, ColorName = "Light Blue" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (14 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 153, 153, 255)), RowIndex = 5, ColumnIndex = 8, ColorName = "Light Blue" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (16 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 153, 102, 255)), RowIndex = 5, ColumnIndex = 9, ColorName = "Light Blue" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (18 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 153, 51, 255)), RowIndex = 5, ColumnIndex = 10, ColorName = "Lavender" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (20 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 153, 0, 255)), RowIndex = 5, ColumnIndex = 11, ColorName = "Purple" });
            x = x - adj;
            y = y + opp + hyp;
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x, y), color = new SolidColorBrush(Color.FromArgb(255, 0, 102, 0)), RowIndex = 6, ColumnIndex = 1, ColorName = "Dark Green" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (2 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 0, 204, 0)), RowIndex = 6, ColumnIndex = 2, ColorName = "Green" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (4 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0)), RowIndex = 6, ColumnIndex = 3, ColorName = "Green" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (6 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 102, 255, 153)), RowIndex = 6, ColumnIndex = 4, ColorName = "Light Green" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (8 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 153, 255, 204)), RowIndex = 6, ColumnIndex = 5, ColorName = "Light Green" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (10 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 204, 255, 255)), RowIndex = 6, ColumnIndex = 6, ColorName = "Sky Blue" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (12 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 204, 236, 255)), RowIndex = 6, ColumnIndex = 7, ColorName = "Light Blue" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (14 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 204, 204, 255)), RowIndex = 6, ColumnIndex = 8, ColorName = "Light Blue" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (16 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 204, 153, 255)), RowIndex = 6, ColumnIndex = 9, ColorName = "Lavender" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (18 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 204, 102, 255)), RowIndex = 6, ColumnIndex = 10, ColorName = "Lavender" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (20 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 204, 0, 255)), RowIndex = 6, ColumnIndex = 11, ColorName = "Purple" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (22 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 153, 0, 204)), RowIndex = 6, ColumnIndex = 12, ColorName = "Purple" });
            x = x - adj;
            y = y + opp + hyp;
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x, y), color = new SolidColorBrush(Color.FromArgb(255, 0, 51, 0)), RowIndex = 7, ColumnIndex = 1, ColorName = "Dark Green" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (2 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 0, 128, 0)), RowIndex = 7, ColumnIndex = 2, ColorName = "Dark Green" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (4 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 51, 204, 51)), RowIndex = 7, ColumnIndex = 3, ColorName = "Green" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (6 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 102, 255, 102)), RowIndex = 7, ColumnIndex = 4, ColorName = "Light Green" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (8 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 153, 255, 153)), RowIndex = 7, ColumnIndex = 5, ColorName = "Light Green" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (10 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 204, 255, 204)), RowIndex = 7, ColumnIndex = 6, ColorName = "Light Green" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (12 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)), RowIndex = 7, ColumnIndex = 7, ColorName = "White" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (14 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 255, 204, 255)), RowIndex = 7, ColumnIndex = 8, ColorName = "Lavender" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (16 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 255, 153, 255)), RowIndex = 7, ColumnIndex = 9, ColorName = "Lavender" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (18 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 255, 102, 255)), RowIndex = 7, ColumnIndex = 10, ColorName = "Lavender" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (20 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 255, 0, 255)), RowIndex = 7, ColumnIndex = 11, ColorName = "Purple" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (22 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 204, 0, 204)), RowIndex = 7, ColumnIndex = 12, ColorName = "Purple" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (24 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 102, 0, 102)), RowIndex = 7, ColumnIndex = 13, ColorName = "Dark Purple" });
            x = x +adj;
            y = y + opp + hyp;
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x, y), color = new SolidColorBrush(Color.FromArgb(255, 51, 102, 0)), RowIndex = 8, ColumnIndex = 1, ColorName = "Dark Green" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (2 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 0, 153, 0)), RowIndex = 8, ColumnIndex = 2, ColorName = "Dark Green" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (4 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 102, 255, 51)), RowIndex = 8, ColumnIndex = 3, ColorName = "Green" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (6 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 153, 255, 102)), RowIndex = 8, ColumnIndex = 4, ColorName = "Light Green" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (8 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 204, 255, 153)), RowIndex = 8, ColumnIndex = 5, ColorName = "Light Green" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (10 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 255, 255, 204)), RowIndex = 8, ColumnIndex = 6, ColorName = "Light Yellow" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (12 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 255, 204, 204)), RowIndex = 8, ColumnIndex = 7, ColorName = "Rose" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (14 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 255, 153, 204)), RowIndex = 8, ColumnIndex = 8, ColorName = "Pink" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (16 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 255, 102, 204)), RowIndex = 8, ColumnIndex = 9, ColorName = "Pink" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (18 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 255, 51, 204)), RowIndex = 8, ColumnIndex = 10, ColorName = "Pink" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (20 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 204, 0, 153)), RowIndex = 8, ColumnIndex = 11, ColorName = "Pink" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (22 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 128, 0, 128)), RowIndex = 8, ColumnIndex = 12, ColorName = "Dark Purple" });
            x = x + adj;
            y = y + opp + hyp;
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x, y), color = new SolidColorBrush(Color.FromArgb(255, 51, 51, 0)), RowIndex = 9, ColumnIndex = 1 ,ColorName = "Dark Yellow" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (2 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 102, 153, 0)), RowIndex = 9, ColumnIndex = 2, ColorName = "Dark Green" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (4 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 153, 255, 51)), RowIndex = 9, ColumnIndex = 3, ColorName = "Green" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (6 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 204, 255, 102)), RowIndex = 9, ColumnIndex = 4, ColorName = "Lime" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (8 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 255, 255, 153)), RowIndex = 9, ColumnIndex = 5, ColorName = "Light Yellow" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (10 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 255, 204, 153)), RowIndex = 9, ColumnIndex = 6, ColorName = "Light Orange" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (12 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 255, 153, 153)), RowIndex = 9, ColumnIndex = 7, ColorName = "Rose" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (14 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 255, 102, 153)), RowIndex = 9, ColumnIndex = 8, ColorName = "Rose" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (16 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 255, 51, 153)), RowIndex = 9, ColumnIndex = 9, ColorName = "Pink" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (18 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 204, 51, 153)), RowIndex = 9, ColumnIndex = 10 ,ColorName = "Pink" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (20 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 153, 0, 153)), RowIndex = 9, ColumnIndex = 11, ColorName = "Dark Purple" });
           x = x + adj;
            y = y + opp + hyp;
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x, y), color = new SolidColorBrush(Color.FromArgb(255, 102, 102, 51)), RowIndex = 10, ColumnIndex = 1, ColorName = "Brown" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (2 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 153, 204, 0)), RowIndex = 10, ColumnIndex = 2, ColorName = "Green" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (4 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 204, 255, 51)), RowIndex = 10, ColumnIndex = 3, ColorName = "Lime" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (6 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 255, 255, 102)), RowIndex = 10, ColumnIndex = 4, ColorName = "Light Yellow" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (8 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 255, 204, 102)), RowIndex = 10, ColumnIndex = 5, ColorName = "Light Yellow" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (10 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 255, 153, 102)), RowIndex = 10, ColumnIndex = 6, ColorName = "Light Orange" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (12 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 255, 124, 128)), RowIndex = 10, ColumnIndex = 7, ColorName = "Rose" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (14 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 255, 0, 102)), RowIndex = 10, ColumnIndex = 8, ColorName = "Pink" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (16 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 214, 0, 147)), RowIndex = 10, ColumnIndex = 9, ColorName = "Pink " });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (18 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 153, 51, 102)), RowIndex = 10, ColumnIndex = 10, ColorName = "Pink" });
            x = x + adj;
            y = y + opp + hyp;
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x, y), color = new SolidColorBrush(Color.FromArgb(255, 128, 128, 0)), RowIndex = 11, ColumnIndex = 1, ColorName = "Dark Yellow" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (2 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 204, 204, 0)), RowIndex = 11, ColumnIndex = 2, ColorName = "Dark Yellow" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (4 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0)), RowIndex = 11, ColumnIndex = 3, ColorName = "Yellow" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (6 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 255, 204, 0)), RowIndex = 11, ColumnIndex = 4, ColorName = "Gold" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (8 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 255, 153, 51)), RowIndex = 11, ColumnIndex = 5, ColorName = "Orange" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (10 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 255, 102, 0)), RowIndex = 11, ColumnIndex = 6, ColorName = "Orange" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (12 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 255, 80, 80)), RowIndex = 11, ColumnIndex = 7, ColorName = "Red" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (14 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 204, 0, 102)), RowIndex = 11, ColumnIndex = 8, ColorName = "Pink" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (16 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 102, 0, 51)), RowIndex = 11, ColumnIndex = 9, ColorName = "Plum" });
            x = x + adj;
            y = y + opp + hyp;
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x, y), color = new SolidColorBrush(Color.FromArgb(255, 153, 102, 51)), RowIndex = 12, ColumnIndex = 1, ColorName = "Tan" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (2 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 204, 153, 0)), RowIndex = 12, ColumnIndex = 2, ColorName = "Gold" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (4 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 255, 153, 0)), RowIndex = 12, ColumnIndex = 3, ColorName = "Orange" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (6 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 204, 102, 0)), RowIndex = 12, ColumnIndex = 4, ColorName = "Orange" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (8 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 255, 51, 0)), RowIndex = 12, ColumnIndex = 5, ColorName = "Red" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (10 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)), RowIndex = 12, ColumnIndex = 6, ColorName = "Red" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (12 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 204, 0, 0)), RowIndex = 12, ColumnIndex = 7, ColorName = "Red" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (14 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 153, 0, 51)), RowIndex = 12, ColumnIndex = 8, ColorName = "Dark Red" });
           x = x + adj;
            y = y + opp + hyp;
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x, y), color = new SolidColorBrush(Color.FromArgb(255, 102, 51, 0)), RowIndex = 13, ColumnIndex = 1, ColorName = "Brown" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (2 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 153, 102, 0)), RowIndex = 13, ColumnIndex = 2, ColorName = "Dark yellow" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (4 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 204, 51, 0)), RowIndex = 13, ColumnIndex = 3, ColorName = "Red" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (6 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 153, 51, 0)), RowIndex = 13, ColumnIndex = 4, ColorName = "Brown" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (8 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 153, 0, 0)), RowIndex = 13, ColumnIndex = 5, ColorName = "Dark Red" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (10 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 128, 0, 0)), RowIndex = 13, ColumnIndex = 6, ColorName = "Dark Red" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (12 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 165, 0, 33)), RowIndex = 13, ColumnIndex = 7, ColorName = "Dark Red" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x - (7 * adj), y + ((5 * opp) + hyp), 20), color = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)), RowIndex = 14, ColumnIndex = 1, ColorName = "White" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (19 * adj), y + ((5 * opp) + hyp), 20), color = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0)), RowIndex = 15, ColumnIndex = 8, ColorName = "Black" });
            x = x - adj;
            y = y+(2*((2*opp) + hyp));
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x, y), color = new SolidColorBrush(Color.FromArgb(255, 248, 248, 248)), RowIndex = 14, ColumnIndex = 2, ColorName = "White" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (2 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221)), RowIndex = 14, ColumnIndex = 3, ColorName = "Gray-25%" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (4 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 178, 178, 178)), RowIndex = 14, ColumnIndex = 4, ColorName = "Gray-25%" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (10 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 51, 51, 51)), RowIndex = 14, ColumnIndex = 6, ColorName = "Gray-50%" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (6 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 128, 128, 128)), RowIndex = 14, ColumnIndex = 7, ColorName = "Gray-80%" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (8 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 95, 95, 95)), RowIndex = 14, ColumnIndex = 8, ColorName = "Gray-80%" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (10 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 51, 51, 51)), RowIndex = 14, ColumnIndex = 9, ColorName = "Gray-80%" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (12 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 28, 28, 28)), RowIndex = 14, ColumnIndex = 10, ColorName = "Black" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (14 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 8, 8, 8)), RowIndex = 14, ColumnIndex = 11, ColorName = "Black" });
            x = x + adj;
            y = y + opp + hyp;
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x, y), color = new SolidColorBrush(Color.FromArgb(255, 234, 234, 234)), RowIndex = 15, ColumnIndex = 1, ColorName = "Gray-25%" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (2 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 192, 192, 192)), RowIndex = 15, ColumnIndex = 2, ColorName = "Gray-25%" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (4 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 150, 150, 150)), RowIndex = 15, ColumnIndex = 3, ColorName = "Gray-50%" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (6 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 119, 119, 119)), RowIndex = 15, ColumnIndex = 4, ColorName = "Gray-50%" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (8 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 77, 77, 77)), RowIndex = 15, ColumnIndex = 5, ColorName = "Gray-80%" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (10 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 41, 41, 41)), RowIndex = 15, ColumnIndex = 6, ColorName = "Gray-80%" });
            morecolorcollection.Add(new PolygonItem() { Points = CalculatePoints(x + (12 * adj), y), color = new SolidColorBrush(Color.FromArgb(255, 17, 17, 17)), RowIndex = 15, ColumnIndex = 7, ColorName = "Black" });
            Item.ItemsSource = morecolorcollection;
            polygonitem = new PolygonItem();
            this.color = new SolidColorBrush(Colors.Transparent);
        }

        /// <summary>
        /// Draw Path For Big White Polygon when small white polygon is selected
        /// </summary>
        void drawPath1()
        {
         var obj = from more in this.palette.child.morecolorcollection where (more.RowIndex==14 && more.ColumnIndex==1) select more; 
         foreach (PolygonItem poly in obj)
         {
           this.palette.child.path1.Stroke = new SolidColorBrush(Colors.Black);
           this.palette.child.path1.Fill = new SolidColorBrush(Colors.White);
           this.palette.child.path1.Data =this.palette.child.polygonitem.DrawPath(poly.Points);    
         }
      }

        /// <summary>
        /// Draw path for small white polygon when Big white polygon is selected
        /// </summary>
        void drawSmallPath1()
        {
            var obj = from more in this.palette.child.morecolorcollection where (more.RowIndex == 7 && more.ColumnIndex == 7) select more;
            foreach (PolygonItem poly in obj)
            {
                this.palette.child.path1.Stroke = new SolidColorBrush(Colors.Black);
                this.palette.child.path1.Fill = new SolidColorBrush(Colors.White);
                this.palette.child.path1.Data = this.palette.child.polygonitem.DrawPath(poly.Points);
            }
        }
        
        /// <summary>
        /// Method called when Key down event occurs in child window
        /// </summary>
        /// <param name="sender">Object of the sender</param>
        /// <param name="e">Handles key events</param>
        void ChildWindow1KeyDown(object sender, KeyEventArgs e)
        {
            PointCollection oldpoint=new PointCollection();
            PointCollection newpoint=new PointCollection();
            opp = Math.Sin((30 * Math.PI) / 180) * hyp;
            adj = Math.Cos((30 * Math.PI) / 180) * hyp;
            int flag = 0;
            int rindex = 0;
            int colindex = 0;
                if (e.Key == Key.Down)
                {
                    oldpoint = this.palette.child.polygonitem.Points;
                    if (this.palette.child.polygonitem.RowIndex + 1 == 7 && this.palette.child.polygonitem.ColumnIndex + 1 == 7)
                    {
                        drawPath1();
                    }
                    else
                    {
                        this.palette.child.path1.Data = null;
                    }

                    xp = oldpoint[1].X +adj; 
                    yp = oldpoint[1].Y + opp + hyp;
                    newpoint = CalculatePoints(xp, yp);
                    flag = GetPolygonItem(newpoint[1].X, newpoint[1].Y, out polygontemp);
                    if (flag == 1)
                    {
                        this.palette.child.polygonitem = polygontemp;
                        this.palette.child.path.Stroke = new SolidColorBrush(Colors.Black);
                        this.palette.child.path.Fill = new SolidColorBrush(Colors.White);
                        this.palette.child.path.Data = this.palette.child.polygonitem.DrawPath(newpoint);
                        this.New.Background = this.palette.child.polygonitem.color;  
                    }
                    else
                    {
                        rindex = this.palette.child.polygonitem.RowIndex;
                        colindex = this.palette.child.polygonitem.ColumnIndex;
                        switch (rindex)
                        {
                            case 7:
                                colindex = 12; 
                                break;
                            case 8:
                                colindex = 11; 
                                break;
                            case 9:
                                colindex = 10; 
                                break;
                            case 10:
                                colindex = 9; 
                                break;
                            case 11:
                                colindex = 8; 
                                break;
                            case 12:
                                colindex = 7; 
                                break;
                            case 13:
                                rindex = 12; 
                                colindex = colindex + 1;
                                break;
                        }

                        var obj = from more in this.palette.child.morecolorcollection where (more.RowIndex == rindex + 1 && more.ColumnIndex == colindex) select more;
                        foreach (PolygonItem poly in obj)
                        {
                            this.palette.child.polygonitem = poly;
                            xp = poly.Points[1].X;
                            yp = poly.Points[1].Y;
                            newpoint = CalculatePoints(xp, yp);
                            this.palette.child.path.Stroke = new SolidColorBrush(Colors.Black);
                            this.palette.child.path.Fill = new SolidColorBrush(Colors.White);
                            this.palette.child.path.Data = this.palette.child.polygonitem.DrawPath(newpoint);
                            this.New.Background = this.palette.child.polygonitem.color;
                        }
                    }
                 }

               if (e.Key == Key.Up)
                {
                    oldpoint = this.palette.child.polygonitem.Points;
                    if (this.palette.child.polygonitem.RowIndex - 1 == 7 && this.palette.child.polygonitem.ColumnIndex  == 7)
                    {
                        drawPath1();
                    }
                    else
                    {
                        this.palette.child.path1.Data = null;
                    }

                    xp = oldpoint[1].X - adj;
                    yp = oldpoint[1].Y - opp - hyp;
                    newpoint = CalculatePoints(xp, yp);
                    flag = GetPolygonItem(newpoint[1].X, newpoint[1].Y, out polygontemp);
                    if (flag == 1)
                    {
                        this.palette.child.polygonitem = polygontemp;
                        this.palette.child.path.Stroke = new SolidColorBrush(Colors.Black);
                        this.palette.child.path.Fill = new SolidColorBrush(Colors.White);
                        this.palette.child.path.Data = this.palette.child.polygonitem.DrawPath(newpoint);
                        this.New.Background = this.palette.child.polygonitem.color;
                    }
                    else
                    {
                        rindex = this.palette.child.polygonitem.RowIndex;
                        colindex = this.palette.child.polygonitem.ColumnIndex;
                        switch (rindex)
                        {
                            case 1:
                                rindex = 2; 
                                colindex = colindex - 1;
                                break;
                            case 15:
                                rindex = 14;
                                colindex = 7;
                                break;
                            default:
                                colindex = 1;
                                break;
                        }

                        var obj = from more in this.palette.child.morecolorcollection where (more.RowIndex == rindex - 1 && more.ColumnIndex == colindex) select more;
                        foreach (PolygonItem poly in obj)
                        {
                            this.palette.child.polygonitem = poly;

                            xp = poly.Points[1].X;
                            yp = poly.Points[1].Y;
                             newpoint = CalculatePoints(xp, yp);
                            this.palette.child.path.Stroke = new SolidColorBrush(Colors.Black);
                            this.palette.child.path.Fill = new SolidColorBrush(Colors.White);
                            this.palette.child.path.Data = this.palette.child.polygonitem.DrawPath(newpoint);
                            this.New.Background = this.palette.child.polygonitem.color;
                        }
                    }  
                }

                if (e.Key == Key.Left)
                {
                    oldpoint = this.palette.child.polygonitem.Points;
                    if (this.palette.child.polygonitem.RowIndex  == 7 && this.palette.child.polygonitem.ColumnIndex-1 == 7)
                    {
                        drawPath1();
                    }
                    else
                    {
                        this.palette.child.path1.Data = null;
                    }

                    if (this.palette.child.polygonitem.RowIndex == 14 && this.palette.child.polygonitem.ColumnIndex - 1 == 1)
                    {
                        var obj = from more in this.palette.child.morecolorcollection where (more.RowIndex == 14 && more.ColumnIndex ==1) select more;
                        foreach (PolygonItem poly in obj)
                        {
                            polygontemp = poly;
                            flag = 1;
                            newpoint = polygontemp.Points;
                            break;
                        }

                        drawSmallPath1();
                    }
                    else
                    {
                        xp = oldpoint[1].X - (2 * adj);
                        yp = oldpoint[1].Y;
                        newpoint = CalculatePoints(xp, yp);
                        flag = GetPolygonItem(newpoint[1].X, newpoint[1].Y, out polygontemp);
                    }

                    if (flag == 1)
                    {
                        this.palette.child.polygonitem = polygontemp;
                        this.palette.child.path.Stroke = new SolidColorBrush(Colors.Black);
                        this.palette.child.path.Fill = new SolidColorBrush(Colors.White);
                        this.palette.child.path.Data = this.palette.child.polygonitem.DrawPath(newpoint);
                        this.New.Background = this.palette.child.polygonitem.color;
                    }
                    else
                    {
                        rindex = this.palette.child.polygonitem.RowIndex;
                        colindex = this.palette.child.polygonitem.ColumnIndex;
                        switch (rindex)
                        {
                            case 1:
                                colindex = 0;
                                break;
                            case 2:
                                colindex = 7;
                                break;
                            case 3:
                                colindex = 8;
                                break;
                            case 4:
                                colindex = 9; 
                                break;
                            case 5:
                                colindex = 10; 
                                break;
                            case 6:
                                colindex = 11; 
                                break;
                            case 7:
                                colindex = 12; 
                                break;
                            case 8:
                                colindex = 13; 
                                break;
                            case 9:
                                colindex = 12; 
                                break;
                            case 10:
                                colindex = 11; 
                                break;
                            case 11:
                                colindex = 10; 
                                break;
                            case 12:
                                colindex = 9; 
                                break;
                            case 13:
                                colindex = 8; 
                                break;
                            case 14:
                                colindex = 7; 
                                break;
                            case 15:
                                if (colindex == 1)
                                {
                                    colindex = 11;
                                }
                                else
                                {
                                    rindex = 16; 
                                    colindex = 7;
                                } 

                                break;
                        }

                        var obj = from more in this.palette.child.morecolorcollection where (more.RowIndex == rindex -1 && more.ColumnIndex == colindex) select more;
                        foreach (PolygonItem poly in obj)
                        {
                            this.palette.child.polygonitem = poly;
                            xp = poly.Points[1].X;
                            yp = poly.Points[1].Y;
                            newpoint = CalculatePoints(xp, yp);
                            this.palette.child.path.Stroke = new SolidColorBrush(Colors.Black);
                            this.palette.child.path.Fill = new SolidColorBrush(Colors.White);
                            this.palette.child.path.Data = this.palette.child.polygonitem.DrawPath(newpoint);
                            this.New.Background = this.palette.child.polygonitem.color;
                        }
                    }
                }

                if (e.Key == Key.Right)
                {
                    oldpoint = this.palette.child.polygonitem.Points;
                    if (this.palette.child.polygonitem.RowIndex  == 7 && this.palette.child.polygonitem.ColumnIndex+1 == 7)
                    {
                        drawPath1();
                    }
                    else if (this.palette.child.polygonitem.RowIndex+1 == 14 && this.palette.child.polygonitem.ColumnIndex -6 == 1)
                    {
                        drawSmallPath1();
                    }
                    else
                    {
                        this.palette.child.path1.Data = null;
                    }

                    if (this.palette.child.polygonitem.RowIndex == 14 && this.palette.child.polygonitem.ColumnIndex == 1)
                    {
                        xp = oldpoint[1].X + (7 * adj) - adj;
                        yp = oldpoint[1].Y - ((5 * opp) + hyp) + (2 * ((2 * opp) + hyp));
                        newpoint = CalculatePoints(xp, yp);
                        flag = GetPolygonItem(newpoint[1].X, newpoint[1].Y, out polygontemp);
                        this.palette.child.path1.Data = null;
                    }
                    else if (this.palette.child.polygonitem.RowIndex == 15 && this.palette.child.polygonitem.ColumnIndex+1 == 8)
                    {
                        var obj = from more in this.palette.child.morecolorcollection where (more.RowIndex == 15 && more.ColumnIndex  == 8) select more;
                        foreach (PolygonItem poly in obj)
                        {
                            polygontemp = poly;
                            flag = 1;
                            newpoint = polygontemp.Points;
                            break;
                        }

                       this.palette.child.path1.Data = null;
                    }
                    else
                    {
                        xp = oldpoint[1].X + (2 * adj);
                        yp = oldpoint[1].Y;
                        newpoint = CalculatePoints(xp, yp);
                        flag = GetPolygonItem(newpoint[1].X, newpoint[1].Y, out polygontemp);
                    }
                    
                    if (flag == 1)
                    {
                        this.palette.child.polygonitem = polygontemp;
                        this.palette.child.path.Stroke = new SolidColorBrush(Colors.Black);
                        this.palette.child.path.Fill = new SolidColorBrush(Colors.White);
                        this.palette.child.path.Data = this.palette.child.polygonitem.DrawPath(newpoint);
                        this.New.Background = this.palette.child.polygonitem.color;
                    }
                    else
                    {
                        rindex = this.palette.child.polygonitem.RowIndex;
                        colindex = this.palette.child.polygonitem.ColumnIndex;
                       
                        var obj = from more in this.palette.child.morecolorcollection where (more.RowIndex == rindex + 1 && more.ColumnIndex == 1) select more;

                        foreach (PolygonItem poly in obj)
                        {
                            this.palette.child.polygonitem = poly;

                            xp = poly.Points[1].X;
                            yp = poly.Points[1].Y;
                            if (poly.RowIndex == 14 && poly.ColumnIndex == 1)
                            {
                                newpoint = CalculatePoints(xp, yp, 20);
                            }
                            else
                            {
                                newpoint = CalculatePoints(xp, yp);
                            }

                            this.palette.child.path.Stroke = new SolidColorBrush(Colors.Black);
                            this.palette.child.path.Fill = new SolidColorBrush(Colors.White);
                            this.palette.child.path.Data = this.palette.child.polygonitem.DrawPath(newpoint);
                            this.New.Background = this.palette.child.polygonitem.color;
                        }
                    }
                }
            }

        /// <summary>
        /// Method is used to return the instance of the PolygonItem based on points
        /// </summary>
        /// <param name="x">The x co-ordinate of the PolygonItem</param>
        /// <param name="y">The y co-ordinate of the PolygonItem</param>
        /// <param name="polygon">The PolygonItem is returned as an out parameter</param>
        /// <returns>Returns 1 or 0 based on Polygon whether polygon exists in x and y points</returns>
        int GetPolygonItem(double x, double y, out PolygonItem polygon)
        { 
            int presentflag=0;
            PolygonItem poly=null;
            foreach (PolygonItem polygon1 in this.palette.child.morecolorcollection)
            {
                if ((Math.Floor(polygon1.Points[1].Y) == Math.Floor(y)) && (Math.Floor(polygon1.Points[1].X) == Math.Floor(x)))
                {
                   poly=polygon1;
                   presentflag = 1;               
                }
            }

            if (presentflag == 1)
            {
                polygon = poly;
                return 1;
            }
            else
            {
                polygon = null;
                return 0;
            }          
        }

       /// <summary>
       /// Method called when OK button is pressed
       /// </summary>
       /// <param name="sender">Object of the sender</param>
       /// <param name="e">Handles RoutedEventArgs</param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.palette.SelectedItem != null)
            {
                this.palette.SelectedItem.IsSelected = false;
                this.palette.SelectedItem.UpdateVisualState(false);
            }

            if (tab.SelectedIndex == 0)
            {
                if (this.color != null)
                {
                    this.palette.IsSelected = false;
                    if (this.palette.child.polygonitem != null)
                    {
                        this.palette.ColorName = this.polygonitem.ColorName;
                        this.palette.SelectedColor = this.palette.child.polygonitem.color;
                        this.Current.Background = this.palette.child.polygonitem.color;
                        this.palette.SelectedMoreColor = this.palette.child.polygonitem;
                    }
                }
            }
            else
            {
                this.palette.IsSelected = false;
                this.palette.SelectedMoreColor = null;
                this.polygonitem = null;
                this.Current.Background = asd.SelectedBrush;
                this.palette.ColorName = "Unknown";
                this.palette.SelectedColor = asd.SelectedBrush;
                
            }

            this.palette.IsChecked = false;
            this.palette.IsColorBorderSelected = false;
            this.palette.IsUpDownSelected = false;
            this.palette.UpdateVisualState(false);
            this.DialogResult = true;          
        }

        /// <summary>
        /// Method called when Cancel button is pressed
        /// </summary>
        /// <param name="sender">Object of the sender</param>
        /// <param name="e">Handles RoutedEventArgs</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.palette.IsChecked = false;
            this.palette.IsColorBorderSelected = false;
            this.palette.IsUpDownSelected = false;
            this.palette.UpdateVisualState(false);
            this.DialogResult = false;
        }

        /// <summary>
        /// Method used to calculate the other 5 coordinates of polygon when one one coordinate is given
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <param name="side">The side lenth of the polygon</param>
        /// <returns>Returns points as point collection</returns>
        public PointCollection CalculatePoints(double x, double y, double side)
        {
            double adj;
            double hyp = side;
            double opp;
            opp = Math.Sin((30 * Math.PI) / 180) * hyp;
            adj = Math.Cos((30 * Math.PI) / 180) * hyp;
            Point point1 = new Point(x - adj, y + opp);        
            Point point2 = new Point(x, y);          
            Point point3 = new System.Windows.Point(x + adj, y + opp);          
            Point point4 = new Point(x + adj, y + opp + hyp);         
            Point point5 = new Point(x, y + (2 * opp) + hyp);           
            Point point6 = new Point(x - adj, y + opp + hyp);           
            PointCollection pointcollection = new PointCollection();
            pointcollection.Add(point1);
            pointcollection.Add(point2);
            pointcollection.Add(point3);
            pointcollection.Add(point4);
            pointcollection.Add(point5);
            pointcollection.Add(point6);          
            return pointcollection;
        }

        /// <summary>
        /// Method used to calculate the other 5 coordinates of polygon when one one coordinate is given
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <returns>Returns points as point collection</returns>
        public PointCollection CalculatePoints(double x, double y)
        {
            double adj;
            double hyp = 10;
            double opp;
            opp = Math.Sin((30 * Math.PI) / 180) * hyp;
            adj = Math.Cos((30 * Math.PI) / 180) * hyp;
            Point point1 = new Point(x - adj, y + opp);
            Point point2 = new Point(x, y);
            Point point3 = new System.Windows.Point(x + adj, y + opp);
            Point point4 = new Point(x + adj, y + opp + hyp);
            Point point5 = new Point(x, y + (2 * opp) + hyp);
            Point point6 = new Point(x - adj, y + opp + hyp);
            PointCollection pointcollection = new PointCollection();
            pointcollection.Add(point1);
            pointcollection.Add(point2);
            pointcollection.Add(point3);
            pointcollection.Add(point4);
            pointcollection.Add(point5);
            pointcollection.Add(point6);
            return pointcollection;
        }

        /// <summary>
        /// A collection of PolygonItem Class
        /// </summary>
        public ObservableCollection<PolygonItem> morecolorcollection;

        /// <summary>
        /// An instance of ColorPickerPalette Class
        /// </summary>
        public ColorPickerPalette palette = new ColorPickerPalette();

        /// <summary>
        /// Method called when Brush selection is changed
        /// </summary>
        /// <param name="d">BrushEdit object where the change occures on</param>
        /// <param name="e"> Property change details, such as old value and new value</param>
        private void asdSelectedBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            New.Background = asd.SelectedBrush;          
        }

        /// <summary>
        /// Method Called when radio button is clicked
        /// </summary>
        /// <param name="sender">Object of the sender</param>
        /// <param name="e">Handles RoutedEventArgs</param>
        private void RadioButtonClick(object sender, RoutedEventArgs e)
        {
            asd.VisualizationStyle = Syncfusion.Windows.Tools.Controls.ColorSelectionMode.RGB;
        }

        /// <summary>
        ///  Method Called when HSVradio button is clicked
        /// </summary>
        /// <param name="sender">Object of the sender</param>
        /// <param name="e">Handles RoutedEventArgs</param>
        private void HSVRadioButtonClick(object sender, RoutedEventArgs e)
        {
            asd.VisualizationStyle = Syncfusion.Windows.Tools.Controls.ColorSelectionMode.HSV;
        }
        
        /// <summary>
        /// Method called when Tab selection is changed
        /// </summary>
        /// <param name="sender">Object of the sender</param>
        /// <param name="e">Handles Selection changedEventArgs</param>
        private void tab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i = 0;
            if ((sender as TabControl).SelectedIndex == 0)
            {
                if (this.palette.child != null)
                {
                    this.palette.child.path.Data = null;
                    this.palette.child.path1.Data = null;
                    if (this.palette.child.morecolorcollection != null)
                    {
                        var obj = from more in this.palette.child.morecolorcollection where ((SolidColorBrush)more.color).Color.Equals(((SolidColorBrush)New.Background).Color) == true select more;                      
                        foreach (PolygonItem asde in obj)
                        {
                            if (i == 0)
                            {
                                this.palette.child.path.Stroke = new SolidColorBrush(Colors.Black);
                                this.palette.child.path.Fill = new SolidColorBrush(Colors.White);
                                this.palette.child.path.Data = asde.DrawPath(asde.Points);
                                i = 1;
                            }
                            else
                            {
                                this.palette.child.path1.Stroke = new SolidColorBrush(Colors.Black);
                                this.palette.child.path1.Fill = new SolidColorBrush(Colors.White);
                                this.palette.child.path1.Data = asde.DrawPath(asde.Points);
                            }
                        }
                    }
                }
            }
        }        
    }
}

