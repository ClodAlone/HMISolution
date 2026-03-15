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
    using Syncfusion.Windows.Tools.Controls;

#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class GridDataDateTimeFilteringPane : GridDataFilteringPane
    {

        #region ctor

        /// <summary>
        /// Initializes the <see cref="GridDataDateTimeFilteringPane"/> class.
        /// </summary>
        static GridDataDateTimeFilteringPane()
        {
#if !SILVERLIGHT
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridDataDateTimeFilteringPane), new FrameworkPropertyMetadata(typeof(GridDataDateTimeFilteringPane)));
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataDateTimeFilteringPane"/> class.
        /// </summary>
        public GridDataDateTimeFilteringPane()
        {
#if SILVERLIGHT
            this.DefaultStyleKey = typeof(GridDataDateTimeFilteringPane);
#endif
        }

        #endregion

        #region DependencyProperties

        internal static readonly DependencyProperty StartDateProperty = DependencyProperty.Register(
            "StartDate",
            typeof(DateTime?),
            typeof(GridDataDateTimeFilteringPane), new PropertyMetadata(null));

        internal static readonly DependencyProperty EndDateProperty = DependencyProperty.Register(
          "EndDate",
          typeof(DateTime?),
          typeof(GridDataDateTimeFilteringPane), new PropertyMetadata(null));

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>The start date.</value>
        internal DateTime? StartDate
        {
            get
            {
                return (DateTime)this.GetValue(GridDataDateTimeFilteringPane.StartDateProperty);
            }

            set
            {
                this.SetValue(GridDataDateTimeFilteringPane.StartDateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>The end date.</value>
        internal DateTime? EndDate
        {
            get
            {
                return (DateTime?)this.GetValue(GridDataDateTimeFilteringPane.EndDateProperty);
            }

            set
            {
                this.SetValue(GridDataDateTimeFilteringPane.EndDateProperty, value);
            }
        }
        
        #endregion

        #region Variables

        private DateTimeEdit startDateTimeEdit;

        private DateTimeEdit endDateTimeEdit;

        private RadioButton ORRadioButton
        {
            get;
            set;
        }

        private RadioButton ANDRadioButton
        {
            get;
            set;
        }

        

        #endregion
              

        #region override Methods

        /// <summary>
        /// Called when [apply template].
        /// </summary>
        public override void OnApplyTemplate()
        {            
            if(this.ClearButton != null)
                this.ClearButton.Click -= new RoutedEventHandler(ClearButton_Click);
            base.OnApplyTemplate();
            this.startDateTimeEdit = this.GetTemplateChild("PART_StartDateTimeEdit") as DateTimeEdit;
            this.endDateTimeEdit = this.GetTemplateChild("PART_EndDateTimeEdit") as DateTimeEdit;
            this.PART_CloseButton = this.GetTemplateChild("PART_CloseButton") as Button;
            if (this.PART_CloseButton != null)
            {
                this.PART_CloseButton.Click -= new RoutedEventHandler(CloseButton_Click);
                this.PART_CloseButton.Click += new RoutedEventHandler(CloseButton_Click); 
            }

            if(this.startDateTimeEdit!=null)
                this.startDateTimeEdit.IsDropDownOpenChanged += new PropertyChangedCallback(startDateTimeEdit_IsDropDownOpenChanged);
            if(this.endDateTimeEdit!=null)
                this.endDateTimeEdit.IsDropDownOpenChanged += new PropertyChangedCallback(endDateTimeEdit_IsDropDownOpenChanged);
            
            this.LoadOptions();
            if (this.ClearButton != null)
                this.ClearButton.Click += new RoutedEventHandler(ClearButton_Click);
            this.ORRadioButton = this.GetTemplateChild("PART_ORRadioButton") as RadioButton;
            this.ANDRadioButton = this.GetTemplateChild("PART_ANDRadioButton") as RadioButton;
            UnLoadRadioButtons();
            LoadRadioButtons();

        }

        // These events are handled to close the calender popup when the another popup opens
        void endDateTimeEdit_IsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            this.startDateTimeEdit.ClosePopup();
        }

        void startDateTimeEdit_IsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            this.endDateTimeEdit.ClosePopup();
        }


        void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var headerCellControl = this.FindParentElementOfType<GridDataHeaderCellControl>();
            headerCellControl.CloseFilterDropDown(); 
        }

        /// <summary>
        /// Unload the radio buttons.
        /// </summary>
        private void UnLoadRadioButtons()
        {
            if (ORRadioButton != null)
            {                
                ORRadioButton.Checked -= OnChecked;
            }

            if (ANDRadioButton != null)
            {               
                ANDRadioButton.Checked -= OnChecked;
            }
        }

        /// <summary>
        /// Loads the radio buttons.
        /// </summary>
        private void LoadRadioButtons()
        {
            if (ORRadioButton != null)
            {
                ORRadioButton.IsChecked = this.PredicateType == PredicateType.Or ? true : false;
                ORRadioButton.Checked += OnChecked;
            }

            if (ANDRadioButton != null)
            {
                ANDRadioButton.IsChecked = this.PredicateType == PredicateType.And ? true : false;
                ANDRadioButton.Checked += OnChecked;
            }
        }

        /// <summary>
        /// Called when [set data context].
        /// </summary>
        /// <param name="wrapperInstance">The wrapper instance.</param>
        protected override void OnSetDataContext(GridDataFilterWrapper wrapperInstance)
        {
            base.OnSetDataContext(wrapperInstance);
            var previousStartFiltervalue = wrapperInstance.StartDateFilterValue;
            var previousEndFiltervalue = wrapperInstance.EndDateFilterValue;
#if !SILVERLIGHT
            var startDateFilterValueBinding = new Binding("StartDateFilterValue") { Source = this.DataContext, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
            var endDateFilterValueBinding = new Binding("EndDateFilterValue") { Source = this.DataContext, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
#else
            var startDateFilterValueBinding = new Binding("StartDateFilterValue") { Source = this.DataContext, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.Default };
            var endDateFilterValueBinding = new Binding("EndDateFilterValue") { Source = this.DataContext, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.Default };
#endif
            this.SetBinding(GridDataDateTimeFilteringPane.StartDateProperty, startDateFilterValueBinding);
            this.SetBinding(GridDataDateTimeFilteringPane.EndDateProperty, endDateFilterValueBinding);

            if (previousStartFiltervalue != null && wrapperInstance.StartDateFilterValue != previousStartFiltervalue && previousEndFiltervalue != null && wrapperInstance.EndDateFilterValue != previousEndFiltervalue)
            {
                wrapperInstance.StartDateFilterValue = previousStartFiltervalue;
                wrapperInstance.EndDateFilterValue = previousEndFiltervalue;
                this.StartDate = previousStartFiltervalue;
                this.EndDate = previousEndFiltervalue;
            }
        }

        #endregion

        #region Private Methods       

        /// <summary>
        /// Loads the options.
        /// </summary>
        private void LoadOptions()
        {
            if (this.VisibleColumn != null && this.VisibleColumn.ColumnType != null)
            {
                var currentColumnType = this.VisibleColumn.ColumnType;
                string[] items = null;
                if (currentColumnType == typeof(string))
                {
                    items = new string[]
                    {
                        GridDataResourceWrapper.StartsWith,
                        GridDataResourceWrapper.EndsWith,
                        GridDataResourceWrapper.AdvanceFilteringContainsString,
                        GridDataResourceWrapper.AdvanceFilteringEqualsString ,
                        GridDataResourceWrapper.NotEquals,
                        GridDataResourceWrapper.AdvnaceFilteringBetweenString
                    };
                }
                else if (currentColumnType == typeof(DateTime))
                {
                    items = new string[]
                    {
                        GridDataResourceWrapper.AdvanceFilteringEqualsString,
                        GridDataResourceWrapper.NotEquals,
                        GridDataResourceWrapper.LessThan,
                        GridDataResourceWrapper.LessThanOrEqual,
                        GridDataResourceWrapper.AdvanceFilteringGreaterThanString,
                        GridDataResourceWrapper.GreaterThanOrEqual,
                        GridDataResourceWrapper.AdvnaceFilteringBetweenString
                    };
                }
                else
                {
                    items = new string[]
                    {
                    GridDataResourceWrapper.AdvanceFilteringEqualsString,
                    GridDataResourceWrapper.NotEquals,
                    GridDataResourceWrapper.LessThan,
                    GridDataResourceWrapper.LessThanOrEqual,
                    GridDataResourceWrapper.AdvanceFilteringGreaterThanString,
                    GridDataResourceWrapper.GreaterThanOrEqual,
                    GridDataResourceWrapper.AdvnaceFilteringBetweenString

                    };
                }
                //This is to preserve the currentfiltertype which is set in the xmal
                string currentFilterTypeString = this.CurrentFilterType.ToString();
                this.OptionCombo.ItemsSource = items;
                //Here the currentfiltertype is set from the OptionComboselectionchanged, so if the currentfiltertype is not Undefined which means currentfiltertype is set from the Xaml. In this case the combobox selected value is set to that value.
                this.OptionCombo.SelectedItem = currentFilterTypeString == "Undefined" ? this.CurrentFilterType.ToString() : currentFilterTypeString;
                if (this.OptionCombo.SelectedIndex == -1)
                {
                    // if CurrentFilterType is not set proper or when it is not set, it will automatically set the first item
                    if (currentColumnType == typeof(string))
                    {
                        this.CurrentFilterType = FilterType.StartsWith;
                    }
                    else
                    {
                        this.CurrentFilterType = FilterType.Equals;
                    }
#if SILVERLIGHT
                    this.OptionCombo.SelectedIndex = 0;
#endif
                }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles the Click event of the ClearButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            this.StartDate = null;
            this.EndDate = null;
        }

       

        /// <summary>
        /// Called when [checked].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void OnChecked(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;
            var content = button.Content.ToString();
            if (this.startDateTimeEdit != null && this.startDateTimeEdit.Text != string.Empty)
            {
                this.NeedsRefresh = true;
            }

            if (content == GridDataResourceWrapper.OR)
            {
                this.PredicateType = PredicateType.Or;
            }
            else if (content == GridDataResourceWrapper.AND)
            {
                this.PredicateType = PredicateType.And;
            }           
          
            if (this.NeedsRefresh)
            {
                this.NeedsRefresh = false;
            }
            this.OnPopupInvoked();
        }

        

        #endregion
    }
}
