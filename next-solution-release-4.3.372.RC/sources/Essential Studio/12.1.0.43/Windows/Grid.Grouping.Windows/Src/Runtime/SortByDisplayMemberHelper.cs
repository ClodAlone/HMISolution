//-------------------------------------------------------------------------------------------------
// <copyright file="SortByDisplayMemberHelper.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;

namespace Syncfusion.Windows.Forms.Grid.Grouping
{
    internal class SortByDisplayMemberHelper
    {
        public static object GetDisplayValue(GridTableModel grid, GridComboBoxListBoxHelper listBox, GridStyleInfo style, object value)
        {
            if (value == null || (value is string && value.Equals(string.Empty)))
            {
                return -1;
            }

            if (style.ChoiceList == null || style.ChoiceList.Count == 0)
            {
                bool exclusive;
                FillWithChoices(grid, listBox, style, out exclusive);
                if (listBox.BindingContext != null && listBox.DataSource != null)
                {
                    ////ListBox.SelectedIndex = -1;
                    ////ListBox.SelectedValue = value;
                    int index = listBox.FindValue(value);
                    if (index == -1)
                    {
                        return null;
                    }

                    return listBox.GetItemText(index);
                }
            }
            else if (style.ChoiceList != null && value is string)
            {
                int index = style.ChoiceList.IndexOf((string)value);
                return listBox.GetItemText(index);
            }

            return null;
        }

        public static void FillWithChoices(GridTableModel grid, ListBox listBox, GridStyleInfo style, out bool exclusive)
        {
            exclusive = style.ExclusiveChoiceList;
            listBox.Name = "Combobox";
            object dataSource = grid.GetStyleDataSource(style);
            if (style.ChoiceList == null || style.ChoiceList.Count == 0)
            {
                if (dataSource != null)
                {
                    ////                    listBox.BackColor = style.Interior.BackColor;
                    ////                    listBox.Font = style.Font.GdipFont;
                    ////                    listBox.ForeColor = style.TextColor;
                    ////listBox.ImageList = style.ImageList;

                    if (listBox.BindingContext == null
                        || dataSource != listBox.DataSource
                        || listBox.DisplayMember != style.DisplayMember
                        || listBox.ValueMember != style.ValueMember)
                    {
                        //// fill with Choices
                        listBox.DataSource = null;
                        listBox.DisplayMember = string.Empty;
                        listBox.ValueMember = string.Empty;
                        listBox.DataSource = dataSource;
                        listBox.DisplayMember = style.DisplayMember;
                        listBox.ValueMember = style.ValueMember;
                        listBox.BindingContext = grid.Table.BindingContext;
                    }
                }
                else
                {
                    Type valueType = style.CellValueType;
                    if (valueType != null)
                    {
                        listBox.DataSource = null;
                        listBox.Items.Clear();
                        TypeConverter tc = TypeDescriptor.GetConverter(valueType);
                        if (tc != null && tc.GetStandardValuesSupported())
                        {
                            ICollection collection = tc.GetStandardValues();
                            foreach (object item in collection)
                            {
                                listBox.Items.Add(item.ToString());
                            }

                            exclusive = tc.GetStandardValuesExclusive();
                        }
                    }
                    else
                    {
                        listBox.DataSource = null;
                        listBox.Items.Clear();
                    }
                }
            }
            else
            {
                listBox.DataSource = null;
                listBox.Items.Clear();
                if (style.ChoiceList != null)
                {
                    foreach (string item in style.ChoiceList)
                    {
                        listBox.Items.Add(item);
                    }
                }
            }
        }
    }
}
