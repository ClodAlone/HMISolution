#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.ObjectModel;
#if WinRT
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#else
using System.Linq;
using System.Windows;
using System.Windows.Data;
#endif


namespace Syncfusion.UI.Xaml.Grid
{
    [ClassReference(IsReviewed = false)]
    public class GroupColumnDescription : DependencyObject
    {
        #region Dependency Properties

        /// <summary>
        /// Gets or sets the Column Name for grouping
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string ColumnName
        {
            get { return (string)GetValue(ColumnNameProperty); }
            set { SetValue(ColumnNameProperty, value); }
        }

        public static readonly DependencyProperty ColumnNameProperty =
            DependencyProperty.Register("ColumnName", typeof(string), typeof(GroupColumnDescription), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the converter for Custom grouping.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public IValueConverter Converter
        {
            get { return (IValueConverter)GetValue(ConverterProperty); }
            set { SetValue(ConverterProperty, value); }
        }

        public static readonly DependencyProperty ConverterProperty =
            DependencyProperty.Register("Converter", typeof(IValueConverter), typeof(GroupColumnDescription), new PropertyMetadata(null));

        #endregion
    }

    public class GroupColumnDescriptions : ObservableCollection<GroupColumnDescription>
    {
        public GroupColumnDescriptions(): base()
        {
            
        }

        public GroupColumnDescription this[string columnName]
        {
            get
            {
                var column = this.FirstOrDefault(col => col.ColumnName == columnName);
                return column;
            }
        }

    }
}
