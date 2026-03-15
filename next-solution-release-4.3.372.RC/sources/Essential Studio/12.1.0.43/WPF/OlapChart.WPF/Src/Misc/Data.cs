#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Chart.Olap
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Syncfusion.Olap.Engine;

    /// <summary>
    /// Represent DataPointInfoProvider
    /// </summary>
    public class DataPointInfoProvider
    {
        #region Members
        PivotValueCellData m_representedCell;
        PivotEngine pivotEngine;
        PivotCellDescriptor pivotCellDescriptor;

        private PivotValueCellData CellData
        {
            get {
                if (m_representedCell == null)
                    m_representedCell = pivotEngine.GetCellData(pivotCellDescriptor);
                return m_representedCell;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DataPointInfoProvider"/> class.
        /// </summary>
        /// <param name="cell">The cell.</param>
        public DataPointInfoProvider(PivotValueCellData cell)
        {
            m_representedCell = cell;
        }

        public DataPointInfoProvider(PivotEngine pEngine, PivotCellDescriptor cellDescriptor)
        {
            pivotCellDescriptor = cellDescriptor;
            pivotEngine = pEngine;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the column.
        /// </summary>
        /// <value>The column.</value>
        public string Column
        {
            get
            {
                string retValue = String.Empty;
                if (CellData != null)
                {
                    for (int j = 0; j < CellData.Columns.Count; j++)
                    {
                        if (j > 0)
                        {
                            retValue += " - ";
                        }
                        retValue += CellData.Columns[j];
                    }
                }
                return retValue;
            }
        }

        /// <summary>
        /// Gets the measure.
        /// </summary>
        /// <value>The measure.</value>
        public string Measure
        {
            get
            {
                if (CellData!=null)
                {
                    return CellData.Measure; 
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the row.
        /// </summary>
        /// <value>The row.</value>
        public string Row
        {
            get
            {
                string retValue = string.Empty;
                if (CellData != null)
                {
                    for (int k = 0; k < CellData.Rows.Count; k++)
                    {
                        if (k > 0)
                        {
                            retValue += " - ";
                        }
                        retValue += CellData.Rows[k];
                    }
                }
                return retValue;
            }
        }

        public string Value
        {
            get
            {
                if (CellData != null)
                {
                    return CellData.Value;
                }
                return string.Empty;
            }
        }
        #endregion
    }

    /// <summary>
    /// Provides olap series tooltip depending on its chart type.
    /// </summary>
    internal class SeriesToolTipConverter : IValueConverter
    {
        #region Public Methods
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ChartTypes)
            {
                ChartTypes targetChartType = (ChartTypes)value;
                if (OlapArea.IsToolTipSupported(targetChartType) && parameter is OlapChart && ((OlapChart)parameter).OlapDataManager.ItemSource==null)
                {
                    ToolTip toolTip = new ToolTip();
                    toolTip.SetBinding(ToolTip.TemplateProperty, new Binding("SeriesToolTipTemplate") { Source = parameter });
                    return toolTip;
                }
            }
            return null;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert ToolTip to corresponding chart type");
        }
        #endregion
        #region IValueConverter Members

        #endregion
    }
}
