#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Documentation;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Chart.Utils;
using Syncfusion.Windows.Forms.Chart.Localization;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// This class inherits the class <see cref="ContextMenu"/>.
    /// Contains the all logical of context menu of the <see cref="ChartSeries"/>.
    /// </summary>
    ///<internalonly/>
    [DocumentationExclude()]
    [ToolboxItem(false)]
    public class ChartSeriesContextMenu : ContextMenu
    {
        #region Members
        private ChartSeries m_series = null;
        private ChartControl m_chart = null;
        #endregion

        #region Public methods
        /// <summary>
        /// Displays the shortcut menu at the specified position.
        /// </summary>
        /// <param name="control">A <see cref="Control"/> that specifies the control with which this shortcut menu is associated</param>
        /// <param name="pos">A <see cref="Point"/> that specifies the coordinates at which to display the menu.</param>
        /// <param name="series">The series.</param>
        public void Show(Control control, Point pos, ChartSeries series)
        {
            Control chart = control;

            while (chart != null)
            {
                if (chart is ChartControl)
                {
                    m_chart = chart as ChartControl;
                    break;
                }

                chart = chart.Parent;
            }

            m_series = series;

            this.InitializeMenu();
            this.Show(control, pos);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Initializes necessary members and items of the <see cref="ChartSeriesContextMenu"/>.
        /// </summary>
        private void InitializeMenu()
        {
            TagMenuItem seriesItem = new TagMenuItem(m_series.Name);

            TagMenuItem editStyleItem = new TagMenuItem(m_chart.Localization.EditStyle, m_series, new EventHandler(OnEditStyleClick));

            
            seriesItem.Tag = m_series;
            editStyleItem.Image = ChartCommandsImages.Style;

            seriesItem.Image = new Bitmap(16, 16);
            seriesItem.Enabled = false;

            this.MenuItems.Clear();
            this.MenuItems.Add(seriesItem);
            this.MenuItems.Add(this.CreateSeriesTypesItem(m_series));
            this.MenuItems.Add(this.CreateSeparator());
            this.MenuItems.Add(editStyleItem);

            using (Graphics g = Graphics.FromImage(seriesItem.Image))
            {
                BrushPaint.FillRectangle(g, new Rectangle(0, 0, 16, 16), m_series.GetOfflineStyle().Interior);
                g.DrawRectangle(Pens.Black, 0, 0, 15, 15);
            }
        }

        #region Create items methods
        /// <summary>
        /// Creates the series types item.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>Returns <see cref="TagMenuItem"/> class.</returns>
        private TagMenuItem CreateSeriesTypesItem(ChartSeries series)
        {
            TagMenuItem itemTypes = new TagMenuItem(m_chart.Localization.Types);

            itemTypes.Image = ChartCommandsImages.SeriesType;
            itemTypes.Tag = series;

            foreach (ChartSeriesType type in Enum.GetValues(typeof(ChartSeriesType)))
            {
                TagMenuItem typeItem = new TagMenuItem(type, new EventHandler(OnSeriesTypeClick));                

                typeItem.Image = ChartSeriesTypeImages.GetImage(type);
                typeItem.Checked = series.Type == type;

                itemTypes.MenuItems.Add(typeItem);
            }

            return itemTypes;
        }

        /// <summary>
        /// Returns the separator of the menu items.
        /// </summary>
        /// <returns>The Instance of <see cref="MenuItem"/> class.</returns>
        private MenuItem CreateSeparator()
        {
            return new MenuItem("-");
        }
        #endregion

        #region Events handlers
        /// <summary>
        /// Called when the Type item of the Series item is clicked.
        /// </summary>
        /// <param name="sender">A sender of the event.</param>
        /// <param name="e">Argument.</param>
        private void OnSeriesTypeClick(object sender, System.EventArgs e)
        {
            TagMenuItem typeItem = sender as TagMenuItem;
            ChartSeries series = (typeItem.Parent as TagMenuItem).Tag as ChartSeries;

            if (typeItem != null)
            {
                series.Type = (ChartSeriesType)typeItem.Tag;
            }
        }

        /// <summary>
        /// Called when the Edit style item of the Series item item is clicked.
        /// </summary>
        /// <param name="sender">A sender of the event.</param>
        /// <param name="e">Argument.</param>
        private void OnEditStyleClick(object sender, System.EventArgs e)
        {
            TagMenuItem typeItem = sender as TagMenuItem;
            ChartSeries series = typeItem.Tag as ChartSeries;

            if (series != null)
            {
                m_chart.DisplayUserEditStylesDialog(m_chart.Series.IndexOf(series));
            }
        }
        #endregion

        #endregion
    }
}
