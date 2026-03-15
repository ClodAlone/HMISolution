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
using System.Collections.ObjectModel;


namespace Syncfusion.Grid.WPF.VisualStudio.Design
{

    public enum DateTimePatternView
    {
        CustomPattern = 0,
        ShortDate = 1,
        LongDate = 2,
        ShortTime = 3,
        LongTime = 4,
        FullDateTime = 5,
        MonthDay = 6,
        RFC1123 = 7,
        SortableDateTime = 8,
        UniversalSortableDateTime = 9,
        YearMonth = 10,
    }

    public class RemoveVisibleColumn
      : EventArgs
    {
        public object DataContext { get; set; }
        public RemoveVisibleColumn()
        {
        }
    }
    /// <summary>
    /// Interaction logic for VisibleColumn.xaml
    /// </summary>
    public partial class VisibleColumn : UserControl
    {
        public VisibleColumn()
        {
            InitializeComponent();
        }

        public event EventHandler<RemoveVisibleColumn> RemoveColumn;


        private void PART_CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            var grid = b.Parent as System.Windows.Controls.Grid;
            var expander = grid.Children[0] as Expander;
            RemoveColumn(this, new RemoveVisibleColumn() { DataContext = expander.DataContext });
            grid.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.GVC.IsChecked = true;
            visibleColumnsListBox.Visibility = Visibility.Visible;

        }

        private Expander expander;

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            if (expander != null && expander != ((Expander)sender))
            {
                expander.IsExpanded = false;
            }

            expander = sender as Expander;
        }
    }

  


    public class VisiblityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return "Collapsed";
            }

            var item = value as ComboBoxItem;

            switch (value.ToString())
            {

                case "CurrencyEdit":
                    if (parameter.ToString().Equals("CurrencyEdit"))
                    {
                        return "Visible";
                    }
                    break;
                case "DateTimeEdit":
                    if (parameter.ToString().Equals("DateTimeEdit"))
                    {
                        return "Visible";
                    }
                    break;
                case "UpDownEdit":
                    if (parameter.ToString().Equals("UpDownEdit"))
                    {
                        return "Visible";
                    }
                    break;

            }

            return "Collapsed";

        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    public class ListBoxVisiblityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            bool? val = value as bool?;

            if (val == false)
            {
                return "Collapsed";
            }
            else
            {
                return "Visiblity";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }


    public enum CellTypes
    {
        Static,
        TextBox,
        TextBlock,
        CheckBox,
        Button,
        ComboBox,
    }
  
}
