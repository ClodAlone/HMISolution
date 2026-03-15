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

namespace Syncfusion.Windows.Controls.Grid
{
    /// <summary>
    /// Holds the information that specifies a visible column in the GridTreeControl.
    /// </summary>
    public class GridTreeColumn : DependencyObject, IPercentWidth
    {

        static GridTreeColumn()
        {
            PropertyMetadata mData = new PropertyMetadata(new PropertyChangedCallback(OnMappingNameChanged));
            MappingNameProperty = DependencyProperty.Register("MappingName", typeof(string), typeof(GridTreeColumn), mData);

            mData = new PropertyMetadata(new PropertyChangedCallback(OnHeaderTextChanged));
            HeaderTextProperty = DependencyProperty.Register("HeaderText", typeof(string), typeof(GridTreeColumn), mData);

            mData = new PropertyMetadata(80d, new PropertyChangedCallback(OnWidthChanged));
            WidthProperty = DependencyProperty.Register("Width", typeof(double), typeof(GridTreeColumn), mData);

            mData = new PropertyMetadata(GridTreeColumnWidthSizer.NotSet, new PropertyChangedCallback(OnPercentWidthChanged));
            PercentWidthProperty = DependencyProperty.Register("PercentWidth", typeof(double), typeof(GridTreeColumn), mData);

            mData = new PropertyMetadata(true);
            AllowSortProperty = DependencyProperty.Register("AllowSort", typeof(bool), typeof(GridTreeColumn), mData);

            mData = new PropertyMetadata(string.Empty);
            ReferenceFieldsProperty = DependencyProperty.Register("ReferenceFields", typeof(string), typeof(GridTreeColumn), mData);
        }
      
        static void OnMappingNameChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
        }
        static void OnHeaderTextChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
        }
        static void OnWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
        }
        static void OnPercentWidthChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GridTreeColumn()
        {

        }

        /// <summary>
        /// Contructor.
        /// </summary>
        /// <param name="mappingName">The mapping name associated with this column.</param>
        /// <param name="headerText">The text to be displayed in the header cell.</param>
        /// <param name="width">The width of this column.</param>
        /// <remarks> The MappingName should uniquely identify a property in the items 
        /// for this tree. This is normally the PropertyName of the value.
        /// </remarks>
        public GridTreeColumn(string mappingName, string headerText, double width)
        {
            this.MappingName = mappingName;
            this.HeaderText = headerText;
            this.Width = width;

        }

        /// Contructor.
        /// </summary>
        /// <param name="mappingName">The mapping name associated with this column.</param>
        /// <param name="width">The width of this column.</param>
        /// <remarks> The MappingName should uniquely identify a property in the items 
        /// for this tree. This is normally the PropertyName of the value.
        /// </remarks>
        public GridTreeColumn(string mappingName, double width)
        {
            this.MappingName = mappingName;
            this.HeaderText = mappingName;
            this.Width = width;

        }

        /// Contructor.
        /// </summary>
        /// <param name="mappingName">The mapping name associated with this column.</param>
        /// <remarks> The MappingName should uniquely identify a property in the items 
        /// for this tree. This is normally the PropertyName of the value.
        /// </remarks>
        public GridTreeColumn(string mappingName)
        {
            this.MappingName = mappingName;
            this.HeaderText = mappingName;
            this.Width = double.NegativeInfinity;

        }

        #region IsUnbound

        internal bool IsUnbound
        {
            get { return (bool)GetValue(IsUnboundProperty); }
            set { SetValue(IsUnboundProperty, value); }
        }

        public static readonly DependencyProperty IsUnboundProperty =
            DependencyProperty.Register("IsUnbound", typeof(bool), typeof(GridTreeUnboundColumn), new PropertyMetadata(false));

        #endregion

        // Using a DependencyProperty as the backing store for ReferenceFields.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReferenceFieldsProperty;

        public string ReferenceFields
        {
            get { return (string)GetValue(ReferenceFieldsProperty); }
            set { SetValue(ReferenceFieldsProperty, value); }
        }

        /// <exclude/>
        public static readonly DependencyProperty AllowSortProperty;
        /// <summary>
        /// Gets or sets the width of the column.
        /// </summary>
        public bool AllowSort
        {
            get { return (bool)GetValue(AllowSortProperty); }
            set { SetValue(AllowSortProperty, value); }
        }

        /// <exclude/>
        public static readonly DependencyProperty MappingNameProperty;


        /// <summary>
        /// Gets or sets the property name of the object assoicated with this column.
        /// </summary>
        public string MappingName
        {
            get { return (string)GetValue(MappingNameProperty); }
            set { SetValue(MappingNameProperty, value); }
        }


        /// <exclude/>
        public static readonly DependencyProperty HeaderTextProperty;
        /// <summary>
        /// Gets or sets the text that you want to see in the columns headers if they are visible.
        /// </summary>
        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        /// <exclude/>
        public static readonly DependencyProperty WidthProperty;
        /// <summary>
        /// Gets or sets the width of the column.
        /// </summary>
        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        GridStyleInfo styleInfo;

        /// <summary>
        /// Gets or sets the GridStyleInfo object that defines style properties to be applied to
        /// cells in this column.
        /// </summary>
        public GridStyleInfo StyleInfo
        {
            get
            {
                if (styleInfo == null)
                    styleInfo = new GridStyleInfo();
                return styleInfo;
            }
            set { styleInfo = value; }
        }

        #region IPercentWidth Members

        /// <exclude/>
        public static readonly DependencyProperty PercentWidthProperty;
        /// <summary>
        /// Gets or sets the percentage weight for this column.
        /// </summary>
        /// <remarks>The default value is double.MinValue which indicates that this column is not
        /// to be included in the variable width columns whose size will change to fill the client area
        /// of the GridControl. Columns whose values are other than double.MinValue will have their
        /// columns widths changed so the entire client area is filled with columns. The algorithm
        /// that computes the width first sums the widths of all columns whose value PercentWidth value
        /// is double.MinValue. Using this sum, the width of the remaining client area is determined, 
        /// and that remaining width is allcated to the percent columns based on the PercentWeight value.
        /// So, if there are 3 columns whose PercentWeight values are set to 1, 1 and 2 respectively,
        /// then the widths of the first 2 columns will be each set to 25% of the remaining space, and the last
        /// column width will be set to 50% of the remaining space.</remarks>
        public double PercentWidth
        {
            get
            {
                { return (double)GetValue(PercentWidthProperty); }
            }
            set
            {
                { SetValue(PercentWidthProperty, value); }
            }
        }

        #endregion
    }
}
