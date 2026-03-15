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


    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:AdvancedFilter_2008"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:AdvancedFilter_2008;assembly=AdvancedFilter_2008"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:CustomFilterPane/>
    ///
    /// </summary>
    public abstract class GridDataFilteringPane : Control, IGridDataFilterAction
    {

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataFilteringPane"/> class.
        /// </summary>
        public GridDataFilteringPane()
        {
            this.NeedsRefresh = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the type of the current filter.
        /// </summary>
        /// <value>The type of the current filter.</value>
        public FilterType CurrentFilterType
        {
            get
            {
                return (FilterType)this.GetValue(GridDataFilteringPane.FilterTypeEnumProperty);
            }

            set
            {
                this.SetValue(GridDataFilteringPane.FilterTypeEnumProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the visible column.
        /// </summary>
        /// <value>The visible column.</value>
        public GridDataVisibleColumn VisibleColumn
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the option combo.
        /// </summary>
        /// <value>The option combo.</value>
        protected ComboBox OptionCombo
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the PAR t_ close button.
        /// </summary>
        /// <value>The PAR t_ close button.</value>
        internal Button PART_CloseButton
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the clear button.
        /// </summary>
        /// <value>The clear button.</value>
        protected Button ClearButton
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or Sets NeedsRefresh. Set this values when the Filter has to be refreshed
        /// </summary>
        /// <value><c>true</c> if [needs refresh]; otherwise, <c>false</c>.</value>
        protected bool NeedsRefresh
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the predicate.
        /// </summary>
        /// <value>The type of the predicate.</value>
        public PredicateType PredicateType
        {
            get
            {
                return (PredicateType)this.GetValue(GridDataFilteringPane.PredicateTypeProperty);
            }

            set
            {
                this.SetValue(GridDataFilteringPane.PredicateTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [match case].
        /// </summary>
        /// <value><c>true</c> if [match case]; otherwise, <c>false</c>.</value>
        public bool MatchCase
        {
            get
            {
                return (bool)this.GetValue(GridDataFilteringPane.MatchCaseProperty);
            }

            set
            {
                this.SetValue(GridDataFilteringPane.MatchCaseProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is themed.
        /// </summary>
        /// <value><c>true</c> if this instance is themed; otherwise, <c>false</c>.</value>
        public bool IsThemed
        {
            get
            {
                return (bool)this.GetValue(GridDataFilteringPane.IsThemedProperty);
            }
            set
            {
                this.SetValue(GridDataFilteringPane.IsThemedProperty, value);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the filter wrapper.
        /// </summary>
        /// <returns></returns>
        internal GridDataFilterWrapper GetFilterWrapper()
        {
            if (filterWrapper == null)
            {
                filterWrapper = new GridDataFilterWrapper();
                this.OnFilterWrapperCreated(filterWrapper);
            }

            return filterWrapper;
        }
		
		/// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.filterWrapper != null)
            {
                this.filterWrapper.IsInSuspend = true;
                this.filterWrapper.FilterValue = null;
                this.filterWrapper.IsInSuspend = false;
                //this.filterWrapper = null;
            }
        }

        

        /// <summary>
        /// Called when [filter wrapper created].
        /// </summary>
        /// <param name="filterWrapper">The filter wrapper.</param>
        protected virtual void OnFilterWrapperCreated(GridDataFilterWrapper filterWrapper)
        {
        }

        /// <summary>
        /// Implement this method in the filter pane for focusing filtering controls. This would be called when the popup is opened.
        /// </summary>
        public void Invoke()
        {
            this.OnPopupInvoked();
        }

        /// <summary>
        /// Called when [popup invoked].
        /// </summary>
        public virtual void OnPopupInvoked()
        {
        }


        /// <summary>
        /// Set the DataContext of the FilteringPane with the wrapper instance value;
        /// </summary>
        /// <param name="wrapperInstance"></param>
        public void SetDataContext(GridDataFilterWrapper wrapperInstance)
        {
            wrapperInstance.IsInSuspend = true;
            this.DataContext = wrapperInstance;
            if (this.IsThemed)
            {
                var backgroundBinding = new Binding("Background") { Source = this.DataContext };
                this.SetBinding(Control.BackgroundProperty, backgroundBinding);
                var foregroundBinding = new Binding("Foreground") { Source = this.DataContext };
                this.SetBinding(Control.ForegroundProperty, foregroundBinding);
            }
            var previousFilterType = this.CurrentFilterType;
#if !SILVERLIGHT
            var filterTypeBinding = new Binding("FilterType") { Source = this.DataContext, Mode = BindingMode.OneWayToSource, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
            var predicateTypeBinding = new Binding("PredicateType") { Source = this.DataContext, Mode = BindingMode.OneWayToSource, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
            var matchCaseBinding = new Binding("MatchCase") { Source = this.DataContext, Mode = BindingMode.OneWayToSource, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
#else
            var filterTypeBinding = new Binding("FilterType") { Source = this.DataContext, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.Default };
            var predicateTypeBinding = new Binding("PredicateType") { Source = this.DataContext, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.Default };
            var matchCaseBinding = new Binding("MatchCase") { Source = this.DataContext, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.Default };
#endif
            this.SetBinding(GridDataFilteringPane.FilterTypeEnumProperty, filterTypeBinding);
            if (previousFilterType != wrapperInstance.FilterType)
            {
                wrapperInstance.FilterType = previousFilterType;
                this.CurrentFilterType = previousFilterType;
            }

            //when binding it resets the value, so just have a cached instance to restore it back
            var previousPredicateType = this.PredicateType;
            this.SetBinding(GridDataFilteringPane.PredicateTypeProperty, predicateTypeBinding);
            if (this.PredicateType != previousPredicateType)
            {
                this.PredicateType = previousPredicateType;
                wrapperInstance.PredicateType = previousPredicateType;
            }

            var previousMatchCase = this.MatchCase;
            
            this.SetBinding(GridDataFilteringPane.MatchCaseProperty, matchCaseBinding);
            if (this.MatchCase != previousMatchCase)
            {
                this.MatchCase = previousMatchCase;
                wrapperInstance.MatchCase = previousMatchCase;
            }

            this.OnSetDataContext(wrapperInstance);
            wrapperInstance.IsInSuspend = true;
        }

        /// <summary>
        /// Called when [set data context].
        /// </summary>
        /// <param name="wrapperInstance">The wrapper instance.</param>
        protected virtual void OnSetDataContext(GridDataFilterWrapper wrapperInstance)
        {
            
        }
        

        #endregion

        #region Variables

        private GridDataFilterWrapper filterWrapper;

        #endregion

        #region DependencyProperties

#if !SILVERLIGHT
          public static readonly DependencyProperty FilterTypeEnumProperty = DependencyProperty.Register(
            "FilterType",
            typeof(FilterType),
            typeof(GridDataFilteringPane),
            new FrameworkPropertyMetadata(FilterType.Undefined, OnFilterTypeEnumChanged));

        public static readonly DependencyProperty PredicateTypeProperty = DependencyProperty.Register(
            "PredicateType",
            typeof(PredicateType),
            typeof(GridDataFilteringPane),
            new FrameworkPropertyMetadata(PredicateType.Or, OnPredicateTypeChanged));

        public static readonly DependencyProperty MatchCaseProperty = DependencyProperty.Register(
            "MatchCase",
            typeof(bool),
            typeof(GridDataFilteringPane),
            new FrameworkPropertyMetadata(false, OnMatchCaseChanged));

        public static readonly DependencyProperty IsThemedProperty = DependencyProperty.Register(
            "IsThemed",
            typeof(bool),
            typeof(GridDataFilteringPane),
            new FrameworkPropertyMetadata(true)); 
#else
        public static readonly DependencyProperty FilterTypeEnumProperty = DependencyProperty.Register(
            "FilterType",
            typeof(FilterType),
            typeof(GridDataFilteringPane),
            new PropertyMetadata(FilterType.Undefined, OnFilterTypeEnumChanged));

        public static readonly DependencyProperty PredicateTypeProperty = DependencyProperty.Register(
            "PredicateType",
            typeof(PredicateType),
            typeof(GridDataFilteringPane),
            new PropertyMetadata(PredicateType.Or, OnPredicateTypeChanged));

        public static readonly DependencyProperty MatchCaseProperty = DependencyProperty.Register(
            "MatchCase",
            typeof(bool),
            typeof(GridDataFilteringPane),
            new PropertyMetadata(false, OnMatchCaseChanged));

        public static readonly DependencyProperty IsThemedProperty = DependencyProperty.Register(
            "IsThemed",
            typeof(bool),
            typeof(GridDataFilteringPane),
            new PropertyMetadata(true));
#endif




        #endregion

        #region DependencyProperties Changed Events

        /// <summary>
        /// Called when [match case changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMatchCaseChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var filteringPane = d as GridDataFilteringPane;
            if (filteringPane.NeedsRefresh)
            {
                var filterWrapper = filteringPane.GetFilterWrapper();
                filterWrapper.RefreshFilter();
            }
        }


        /// <summary>
        /// Called when [filter type enum changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnFilterTypeEnumChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var filteringPane = d as GridDataFilteringPane;
            var filterWrapper = filteringPane.GetFilterWrapper();
            filterWrapper.RefreshFilter();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            
            this.OptionCombo = this.GetTemplateChild("PART_ComboBox") as ComboBox;
            this.ClearButton = this.GetTemplateChild("PART_ClearButton") as Button;
        }

        /// <summary>
        /// Called when [predicate type changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnPredicateTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var filteringPane = d as GridDataFilteringPane;
            if (filteringPane.NeedsRefresh)
            {
                var filterWrapper = filteringPane.GetFilterWrapper();
                filterWrapper.RefreshFilter();
            }
        }

        #endregion   
        
    }

    

}
