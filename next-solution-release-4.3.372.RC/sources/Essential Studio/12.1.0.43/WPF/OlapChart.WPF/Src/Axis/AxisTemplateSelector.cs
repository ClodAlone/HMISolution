#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Chart.Olap
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Representing AxisTemplateSelector
    /// </summary>
    public class AxisTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the cartesian axis template.
        /// </summary>
        /// <value>The cartesian axis template.</value>
        public DataTemplate CartesianAxisTemplate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the olap axis template.
        /// </summary>
        /// <value>The olap axis template.</value>
        public DataTemplate OlapAxisTemplate
        {
            get;
            set;
        }

        #region overridden method
        /// <summary>
        /// When overridden in a derived class, returns a <see cref="T:System.Windows.DataTemplate"/> based on custom logic.
        /// </summary>
        /// <param name="item">The data object for which to select the template.</param>
        /// <param name="container">The data-bound object.</param>
        /// <returns>
        /// Returns a <see cref="T:System.Windows.DataTemplate"/> or null. The default value is null.
        /// </returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is OlapChartAxis)
            {
                return this.OlapAxisTemplate;
            }
            if (item is ChartAxis)
            {
                return this.CartesianAxisTemplate;
            }
            return base.SelectTemplate(item, container);
        }
        #endregion

        #region Properties

        #endregion

    }
}
