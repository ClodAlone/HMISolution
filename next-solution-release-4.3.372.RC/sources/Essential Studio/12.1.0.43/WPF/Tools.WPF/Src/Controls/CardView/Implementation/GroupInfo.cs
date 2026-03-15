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
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace Syncfusion.Windows.Tools.Controls
{


    public class CardViewPanel : WrapPanel
    {

    }

    public class SortDirectionToVisibilityConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (((SortingDirection)value) == SortingDirection.Ascending || ((SortingDirection)value) == SortingDirection.Descending)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class SortDirectionToAngleConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (((SortingDirection)value) == SortingDirection.Ascending)
            {
                return 180;
            }
            else if (((SortingDirection)value) == SortingDirection.Descending)
            {
                return 0;
            }
            else
            {
                return 180;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }


    public class GroupInfo : DependencyObject
    {


        public GroupInfo()
        {
            FilterValues = new List<object>();
            CheckedValues = new List<object>();
        }

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }




        public List<object> CheckedValues
        {
            get;
            set;
        }
       
        // Using a DependencyProperty as the backing store for Name.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(GroupInfo), new UIPropertyMetadata(null));

        public List<object> FilterValues
        {
            get { return (List<object>)GetValue(FilterValuesProperty); }
            set { SetValue(FilterValuesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilterValues.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterValuesProperty =
            DependencyProperty.Register("FilterValues", typeof(List<object>), typeof(GroupInfo), new UIPropertyMetadata(null));

        public ObservableCollection<object> Items
        {
            get { return (ObservableCollection<object>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Items.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ObservableCollection<object>), typeof(GroupInfo), new UIPropertyMetadata(null));


        internal double InsertionPoint = 0.0;

        public bool CanInsert
        {
            internal set;

            get;
        }

        public int Level
        {
            internal set;

            get;
        }

        public bool CanInsertAfterThis
        {
            internal set;

            get;
        }


        public SortingDirection SortDirection
        {
            get { return (SortingDirection)GetValue(SortDirectionProperty); }
            set { SetValue(SortDirectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SortDirection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SortDirectionProperty =
            DependencyProperty.Register("SortDirection", typeof(SortingDirection), typeof(GroupInfo), new UIPropertyMetadata(SortingDirection.None));
        
    }

    public enum SortingDirection
    {
        Ascending,

        Descending,

        None
    }
}
