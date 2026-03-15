#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Tools.Controls;
using System.Windows.Controls.Primitives;
using System.Collections.Generic;

namespace Syncfusion.Windows.Tools.Controls
{
    public class TablePickerUI : ButtonBase
    {
        public TablePickerUI()
        {
            DefaultStyleKey = typeof(TablePickerUI);
        }
        private Grid items;

        public TableInfo CellCount
        {
            get { return (TableInfo)GetValue(CellCountProperty); }
            set { SetValue(CellCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellCountProperty =
            DependencyProperty.Register("CellCount", typeof(TableInfo), typeof(TablePickerUI), new PropertyMetadata(new TableInfo() { Row = 10, Column = 8 }, new PropertyChangedCallback(OnCellCountChanged)));


        private static void OnCellCountChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            TablePickerUI tablepicker = sender as TablePickerUI;
            if (tablepicker != null)
            {
                if (tablepicker.items != null)
                {
                    tablepicker.PopulateCells();
                }
            }
        }

        public event PropertyChangedCallback SelectedCellChanged;

        public TableInfo SelectedCell
        {
            get { return (TableInfo)GetValue(SelectedCellProperty); }
            set { SetValue(SelectedCellProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedCell.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedCellProperty =
            DependencyProperty.Register("SelectedCell", typeof(TableInfo), typeof(TablePickerUI), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedCellChanged)));

        private static void OnSelectedCellChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            TablePickerUI tablepicker = sender as TablePickerUI;
            if (tablepicker != null)
            {
                if (tablepicker.SelectedCellChanged != null)
                {
                    tablepicker.SelectedCellChanged(sender as DependencyObject, args);
                }
            }
        }

        public override void OnApplyTemplate()
        {
            items = GetTemplateChild("PART_Items") as Grid;
            if (items != null)
            {
                PopulateCells();
            }
        }
        
        private void PopulateCells()
        {
            int rowcount = CellCount.Row;
            int columncount = CellCount.Column;
            
            for (int i = 0; i < rowcount; i++)
            {
                items.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto});
                for (int j = 0; j < columncount; j++)
                {
                    items.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                    TablePickerItem info = new TablePickerItem() { Row = i, Column = j, ParentPicker = this };
                    Grid.SetColumn(info, j);
                    Grid.SetRow(info, i);
                    items.Children.Add(info);
                }
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            SelectedCell = null;
            foreach (TablePickerItem item in items.Children)
            {
                item.IsValidated = false;
            }
        }

        internal void ValidateCellInfo(TableInfo tableinfo)
        {
            SelectedCell = tableinfo;
            var validateditems = from UIElement item in items.Children
                                 where ((TablePickerItem)item).Row <= tableinfo.Row && ((TablePickerItem)item).Column <= tableinfo.Column
                                 select item;

            foreach (TablePickerItem item in items.Children)
            {
                item.IsValidated = false;
            }

            foreach (TablePickerItem item in validateditems)
            {
                item.IsValidated = true;
            }

        }

    }
}
