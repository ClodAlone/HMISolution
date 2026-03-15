#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Syncfusion.Olap.Engine;
using System.Windows.Data;
using Syncfusion.Windows.Controls.Grid;

#if !SILVERLIGHT
namespace Syncfusion.Windows.Grid.Olap
#else
using Syncfusion.OlapSilverlight.Engine;
namespace Syncfusion.Silverlight.Grid.Olap
#endif
{
#if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif
    public class OlapGridTemplateCell : ContentControl
    {
        #region Initialize / Finalize

#if !SILVERLIGHT
        /// <summary>
        /// Initializes a new instance of the <see cref="OlapGridTemplateCell"/> class.
        /// </summary>
        static OlapGridTemplateCell()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OlapGridTemplateCell), new FrameworkPropertyMetadata(typeof(OlapGridTemplateCell)));            
        }
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="OlapGridTemplateCell"/> class.
        /// </summary>
        public OlapGridTemplateCell()
        {
             DefaultStyleKey = typeof(OlapGridTemplateCell);
        }
#endif
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the custom expander.
        /// </summary>
        /// <value>The custom expander.</value>
        public UIElement CustomExpander { get; set; }

        /// <summary>
        /// Gets or sets the grid control base.
        /// </summary>
        /// <value>The grid control base.</value>
        public OlapGridBase GridControlBase { get; set; }

        public PivotCellDescriptor CellDescriptor
        {
            get { return (PivotCellDescriptor)GetValue(CellDescriptorProperty); }
            set { SetValue(CellDescriptorProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Current Cell is expanded.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is expanded; otherwise, <c>false</c>.
        /// </value>
        public bool IsExpanded
        {
            get
            {
                if (this.CellDescriptor != null)
                {
                    if (CellDescriptor.ExpandableState == ExpandableState.Expanded)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                return false;
            }
            set
            {
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the header cell expander clicked.
        /// </summary>
        public event OlapGridDrillDownEventHander ExpanderClicked;

        #endregion

        #region Dependency Property Declaration

        public static readonly DependencyProperty CellDescriptorProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("CellDescriptor", typeof(PivotCellDescriptor), typeof(OlapGridTemplateCell), new UIPropertyMetadata(null));
#else
            DependencyProperty.Register("CellDescriptor", typeof(PivotCellDescriptor), typeof(OlapGridTemplateCell), new PropertyMetadata(null));
#endif

        #endregion

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            CustomExpander = GetTemplateChild("PART_Expander") as UIElement;
            if (CustomExpander != null)
            {
#if !SILVERLIGHT

                CustomExpander.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(CustomExpander_PreviewMouseLeftButtonDown);
#else
                //CustomExpander.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(CustomExpander_PreviewMouseLeftButtonDown);
                CustomExpander.AddHandler(UIElement.MouseLeftButtonDownEvent, new System.Windows.Input.MouseButtonEventHandler(CustomExpander_PreviewMouseLeftButtonDown), true);
#endif
            }
        }

        void CustomExpander_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.ExpanderClicked != null)
            {
                //// Triggering the event when mouse left button is down
                this.ExpanderClicked(this, new OlapGridDrillDownEventArgs
                {
                    CellDescriptor = this.CellDescriptor,
                    ShowDefaultIndicator = true,
                });
            }
        }

        #endregion
    }

    public class ValueCellToolTipConvertor : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            GridStyleInfo style = value as GridStyleInfo;
            if (style != null)
            {
                OlapGridCellStyleInfoIdentity olapCellStyle = style.Tag as OlapGridCellStyleInfoIdentity;
                if (olapCellStyle != null && olapCellStyle.CellDescriptor != null && parameter != null)
                {
                    PivotValueCellData pivotValueCellData = (style.CellIdentity.Data.Host as OlapGridModel).Engine.GetCellDataValue(olapCellStyle.RowIndex, olapCellStyle.ColumnIndex);
                    switch (parameter.ToString())
                    {
                        case "Measure":
                            {
                                return pivotValueCellData.Measure;
                            }
                        case "Columns":
                            {
                                return pivotValueCellData.Columns.ToString();
                            }
                        case "Rows":
                            {
                                return pivotValueCellData.Rows.ToString();
                            }
                        case "Value":
                            {
                                return pivotValueCellData.Value;
                            }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Converts the string type to visibility type.
    /// </summary>
    public class StringtoVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(string.IsNullOrEmpty(value.ToString()))
                return Visibility.Collapsed;
            return Visibility.Visible;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
