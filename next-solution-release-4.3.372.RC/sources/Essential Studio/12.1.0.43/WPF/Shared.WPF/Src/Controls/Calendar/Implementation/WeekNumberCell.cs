// <copyright file="WeekNumberCell.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents a week number cell of the <see cref="Syncfusion.Windows.Shared.CalendarEdit"/> control.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class WeekNumberCell : Cell
    {
        #region Private members

        /// <summary>
        /// Corner radius for the cell.
        /// </summary>
        private CornerRadius mcornerRadius;

        #endregion Private members

        #region Initilization

        /// <summary>
        /// Initializes static members of the <see cref="WeekNumberCell"/> class.  It overrides some dependency properties.
        /// </summary>
        static WeekNumberCell()
        {
            // This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            // This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WeekNumberCell), new FrameworkPropertyMetadata(typeof(WeekNumberCell)));
            ContentControl.FocusableProperty.OverrideMetadata(typeof(WeekNumberCell), new FrameworkPropertyMetadata(false));
            Border.CornerRadiusProperty.AddOwner(typeof(WeekNumberCell));
        }

        #endregion Initilization

        #region Properties

        /// <summary>
        /// Gets or sets m_CornerRadius.
        /// </summary>
        /// <value>
        /// Type: <see cref="CornerRadius"/>
        /// </value>
        /// <seealso cref="CornerRadius"/>
        public new CornerRadius CornerRadius
        {
            get
            {
                return this.mcornerRadius;
            }

            set
            {
                this.mcornerRadius = value;
            }
        }

        #endregion Properties

        #region Implementation

        /// <summary>
        /// Updates data template and data template selector of the cell.
        /// </summary>
        /// <param name="template">Data template to be set to the cell. If it is
        /// null the local value of data template would be cleared.</param>
        /// <param name="selector">Data template selector to be set to the cell.
        /// If it is null the local value would be cleared.</param>
        /// <remarks>
        /// Both template and selector can not be set at the same time.
        /// </remarks>
        protected internal void UpdateCellTemplateAndSelector(DataTemplate template, DataTemplateSelector selector)
        {
            Debug.Assert(template == null || selector == null, "Both template and selector can not be set at one time.");

            if (selector != null)
            {
                ContentTemplateSelector = selector;
            }
            else
            {
                ClearValue(DayNameCell.ContentTemplateSelectorProperty);
            }

            if (template != null)
            {
                ContentTemplate = template;
            }
            else
            {
                if (selector == null)
                {
                    ClearValue(DayNameCell.ContentTemplateProperty);
                }
                else
                {
                    ContentTemplate = null;
                }
            }
        }

        /// <summary>
        /// Sets style of the cell.
        /// </summary>
        /// <param name="style">Style to be set.</param>
        protected internal void SetStyle(Style style)
        {
            Style = style;
        }

        #endregion Implementation
    }
}