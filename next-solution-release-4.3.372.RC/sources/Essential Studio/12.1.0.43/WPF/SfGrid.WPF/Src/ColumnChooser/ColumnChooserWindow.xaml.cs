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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.Data.Extensions;
using Syncfusion.Windows.Shared;

namespace Syncfusion.UI.Xaml.Grid
{
    /// <summary>
    /// Column Chooser Window for DataGrid
    /// </summary>
    /// <remarks></remarks>
    public partial class ColumnChooser : ChromelessWindow, IColumnChooser
    {
        #region Field

        /// <summary>
        /// Gets DataGrid for the Column Chooser.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        protected SfDataGrid DataGrid { get; set; }

        #endregion

        #region Dependency properties
        /// <summary>
        /// Gets or sets WaterMarkText for Empty Column Chooser.
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public string WaterMarkText
        {
            get { return (string)GetValue(WaterMarkTextProperty); }
            set { SetValue(WaterMarkTextProperty, value); }
        }

        public static readonly DependencyProperty WaterMarkTextProperty =
            DependencyProperty.Register("WaterMarkText", typeof(string), typeof(ColumnChooser), new PropertyMetadata(string.Empty));
        #endregion

        #region Ctor
        public ColumnChooser(SfDataGrid dataGrid)
        {
            InitializeComponent();
            this.DataGrid = dataGrid;
            this.DataContext = this;
            this.Title = GridResourceWrapper.ColumnChooserTitle;
            this.WaterMarkText = GridResourceWrapper.ColumnChooserWaterMark;
        }
        #endregion

        #region Overrides
        //
        // Summary:
        //     Called when an internal process or application calls ApplyTemplate, which
        //     is used to build the current template's visual tree.
        public override void OnApplyTemplate()
        {
            this.DataGrid.Columns.ForEach(col =>
            {
                if (col.IsHidden)
                    AddChild(col);
            });
            base.OnApplyTemplate();
        }
        #endregion

        #region Virtual methods
        #region Child adding and removing

        /// <summary>
        /// Adds the Child for the column chooser whever the column gets hide
        /// </summary>
        /// <param name="column"></param>
        /// <remarks></remarks>
        public virtual void AddChild(GridColumn column)
        {
            if (this.PART_ChooserPanel == null)
                return;
            if (this.PART_ChooserPanel.Children.ToList<ColumnChooserItem>().All(item => (item as ColumnChooserItem).Column.MappingName != column.MappingName) && this.DataGrid.View != null)
            {
                var chooserItem = new ColumnChooserItem(column);
                chooserItem.Controller = this.DataGrid.GridColumnDragDropController;
                chooserItem.ColumnName = column.HeaderText;
                this.PART_ChooserPanel.Children.Add(chooserItem);
            }
            if (this.PART_ChooserPanel.Children.Count == 0)
                this.ColumnChooserAreaText.Visibility = Visibility.Visible;
            else
                this.ColumnChooserAreaText.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Remove the Child for the column chooser whever the column gets Unhide
        /// </summary>
        /// <param name="column"></param>
        /// <remarks></remarks>
        public virtual void RemoveChild(GridColumn column)
        {
            if (this.PART_ChooserPanel != null && this.PART_ChooserPanel.Children.Count > 0)
            {
                var element = this.PART_ChooserPanel.Children.ToList<ColumnChooserItem>().FirstOrDefault(item => (item as ColumnChooserItem).Column.MappingName == column.MappingName);
                if (element != null)
                    this.PART_ChooserPanel.Children.Remove(element);
            }
            if (this.PART_ChooserPanel.Children.Count == 0)
                this.ColumnChooserAreaText.Visibility = Visibility.Visible;
            else
                this.ColumnChooserAreaText.Visibility = Visibility.Collapsed;
        }

        #endregion

        /// <summary>
        /// Returns the Rect of the ColumnChooserControl
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public virtual Rect GetControlRect()
        {
            Point locationfromWindow = this.TranslatePoint(new Point(0, 0), this);
            Point locationfromScreen = this.PointToScreen(locationfromWindow);
            return new Rect((locationfromScreen.X - locationfromWindow.X), (locationfromScreen.Y - locationfromWindow.Y), this.ActualWidth, this.ActualHeight);
        }
        #endregion
    }
}
