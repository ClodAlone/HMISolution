#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using Syncfusion.Linq;
    using System.Windows.Data;
    using System.Windows.Controls;
    using System.ComponentModel;
    using System.Windows.Controls.Primitives;
    using Syncfusion.Windows.Data;
    using Syncfusion.Windows.Shared;
    using System.Collections;
    using System.Collections.ObjectModel;

    #region Slider

    public class GridDataSliderFilteringPane<T> : GridDataFilteringPane
    {

        #region DependencyProperties

#if !SILVERLIGHT
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
            "MinValue",
            typeof(T),
            typeof(GridDataSliderFilteringPane<T>));

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            "MaxValue",
            typeof(T),
            typeof(GridDataSliderFilteringPane<T>));

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(T),
            typeof(GridDataSliderFilteringPane<T>));

        public static readonly DependencyProperty ClearOnOpenProperty = DependencyProperty.Register(
            "ClearOnOpen",
            typeof(bool),
            typeof(GridDataSliderFilteringPane<T>));
        public static readonly DependencyProperty IsSnapToTickEnabledProperty = DependencyProperty.Register(
            "IsSnapToTickEnabled", typeof(bool), typeof(GridDataSliderFilteringPane<T>));
#else
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
            "MinValue",
            typeof(T),
            typeof(GridDataSliderFilteringPane<T>), new PropertyMetadata(null));

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            "MaxValue",
            typeof(T),
            typeof(GridDataSliderFilteringPane<T>), new PropertyMetadata(null));

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(T),
            typeof(GridDataSliderFilteringPane<T>), new PropertyMetadata(null));

        public static readonly DependencyProperty ClearOnOpenProperty = DependencyProperty.Register(
            "ClearOnOpen",
            typeof(bool),
            typeof(GridDataSliderFilteringPane<T>), new PropertyMetadata(null));
        public static readonly DependencyProperty IsSnapToTickEnabledProperty = DependencyProperty.Register(
             "IsSnapToTickEnabled", 
             typeof(bool), typeof(GridDataSliderFilteringPane<T>), new PropertyMetadata(null));
#endif
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is filter set to none.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is filter set to none; otherwise, <c>false</c>.
        /// </value>
        public bool IsFilterSetToNone
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the min value.
        /// </summary>
        /// <value>The min value.</value>
        public T MinValue
        {
            get
            {
                return (T)this.GetValue(GridDataSliderFilteringPane<T>.MinValueProperty);
            }

            set
            {
                this.SetValue(GridDataSliderFilteringPane<T>.MinValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the max value.
        /// </summary>
        /// <value>The max value.</value>
        public T MaxValue
        {
            get
            {
                return (T)this.GetValue(GridDataSliderFilteringPane<T>.MaxValueProperty);
            }

            set
            {
                this.SetValue(GridDataSliderFilteringPane<T>.MaxValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public T Value
        {
            get
            {
                return (T)this.GetValue(GridDataSliderFilteringPane<T>.ValueProperty);
            }

            set
            {
                this.SetValue(GridDataSliderFilteringPane<T>.ValueProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [clear on open].
        /// </summary>
        /// <value><c>true</c> if [clear on open]; otherwise, <c>false</c>.</value>
        public bool ClearOnOpen
        {
            get
            {
                return (bool)this.GetValue(GridDataSliderFilteringPane<T>.ClearOnOpenProperty);
            }

            set
            {
                this.SetValue(GridDataSliderFilteringPane<T>.ClearOnOpenProperty, value);
            }
        }
        public bool IsSnapToTickEnabled
        {
            get
            {
                return (bool)this.GetValue(GridDataSliderFilteringPane<T>.IsSnapToTickEnabledProperty);
            }
            set
            {
                this.SetValue(GridDataSliderFilteringPane<T>.IsSnapToTickEnabledProperty, value);
            }
        }

                #endregion

        #region Methods

        public override void OnPopupInvoked()
        {
            if (this.ClearOnOpen)
            {
                var filterWrapper = this.DataContext as GridDataFilterWrapper;
                filterWrapper.FilterValue = null;
            }
        }

        protected override void OnSetDataContext(GridDataFilterWrapper wrapperInstance)
        {
            base.OnSetDataContext(wrapperInstance);
#if !SILVERLIGHT
            var filterValueBinding = new Binding("FilterValue") { Source = this.DataContext, Mode = BindingMode.OneWayToSource, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
#else
            var filterValueBinding = new Binding("FilterValue") { Source = this.DataContext, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.Default};
#endif

            this.SetBinding(GridDataSliderFilteringPane<T>.ValueProperty, filterValueBinding);
        }

       

        #endregion
        
    }

    #endregion

}
