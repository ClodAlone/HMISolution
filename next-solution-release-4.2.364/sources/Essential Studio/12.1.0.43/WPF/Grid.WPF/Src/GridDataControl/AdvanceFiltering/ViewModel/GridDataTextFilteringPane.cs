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

#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class GridDataTextFilteringPane : GridDataFilteringPane
    {
        #region Ctor

        static GridDataTextFilteringPane()
        {
#if !SILVERLIGHT
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridDataTextFilteringPane), new FrameworkPropertyMetadata(typeof(GridDataTextFilteringPane)));
#endif
        }

        public GridDataTextFilteringPane()
        {
#if SILVERLIGHT
            this.DefaultStyleKey = typeof(GridDataTextFilteringPane);
#endif
        }

        #endregion

        #region DependencyProperties

#if SILVERLIGHT
        internal static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        "Text",
        typeof(string),
        typeof(GridDataTextFilteringPane), new PropertyMetadata(null));
#else
        internal static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(GridDataTextFilteringPane));
#endif



        #endregion

        #region Variables

        private TextBox filterTextBox;

        /// <summary>
        /// Gets or sets the OR radio button.
        /// </summary>
        /// <value>The OR radio button.</value>
        private RadioButton ORRadioButton
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the AND radio button.
        /// </summary>
        /// <value>The AND radio button.</value>
        private RadioButton ANDRadioButton
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the AND radio button.
        /// </summary>
        /// <value>The AND radio button.</value>
        private CheckBox CaseCheckBox
        {
            get;
            set;
        }
        

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        internal string Text
        {
            get
            {
                return (string)this.GetValue(GridDataTextFilteringPane.TextProperty);
            }

            set
            {
                this.SetValue(GridDataTextFilteringPane.TextProperty, value);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called when [apply template].
        /// </summary>
        public override void OnApplyTemplate()
        {           
            if (this.ClearButton != null)
                this.ClearButton.Click -= new RoutedEventHandler(ClearButton_Click);
            base.OnApplyTemplate();
            this.filterTextBox = this.GetTemplateChild("PART_FilterTextBox") as TextBox;
            this.PART_CloseButton = this.GetTemplateChild("PART_CloseButton") as Button;
            if (this.PART_CloseButton != null)
            {
                this.PART_CloseButton.Click -= new RoutedEventHandler(CloseButton_Click);
                this.PART_CloseButton.Click += new RoutedEventHandler(CloseButton_Click);
            }
            this.LoadOptions();
            if (ClearButton != null)
                this.ClearButton.Click += new RoutedEventHandler(ClearButton_Click);

            this.ORRadioButton = this.GetTemplateChild("PART_ORRadioButton") as RadioButton;
            this.ANDRadioButton = this.GetTemplateChild("PART_ANDRadioButton") as RadioButton;
            this.CaseCheckBox = this.GetTemplateChild("PART_CaseCheckBox") as CheckBox;
            UnLoadRadioButtons();
            LoadRadioButtons();          
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

            if (CaseCheckBox != null)
            {
                CaseCheckBox.Unchecked -= OnUnchecked;
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

            if (this.CaseCheckBox != null)
            {
                if (this.VisibleColumn != null)
                {
                    if (this.VisibleColumn.ColumnType == typeof(string))
                    {
                        this.CaseCheckBox.IsChecked = this.MatchCase;
                        this.CaseCheckBox.Checked += OnChecked;
                        this.CaseCheckBox.Unchecked += OnUnchecked;
                    }
                }
            }
        }

        /// <summary>
        /// Called when [unchecked].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void OnUnchecked(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;
            if (this.filterTextBox != null && this.filterTextBox.Text != string.Empty)
            {
                this.NeedsRefresh = true;
            }

            if (button != null && (string)button.Content == GridDataResourceWrapper.MatchCase)
            {
                this.MatchCase = false;
            }           
           
            if (this.NeedsRefresh)
            {
                this.NeedsRefresh = false;
            }
            this.OnPopupInvoked();
        }

        /// <summary>
        /// Called when [checked].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void OnChecked(object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;
            
            if (this.filterTextBox != null && this.filterTextBox.Text != string.Empty)
            {
                this.NeedsRefresh = true;
            }
            if (button != null && (string)button.Content == GridDataResourceWrapper.OR)
            {
                this.PredicateType = PredicateType.Or;
            }
            else if (button != null && (string)button.Content == GridDataResourceWrapper.AND)
            {
                this.PredicateType = PredicateType.And;
            }
            else if (button != null && (string)button.Content == GridDataResourceWrapper.MatchCase)
            {
                this.MatchCase = true;
            }            
            if (this.NeedsRefresh)
            {
                this.NeedsRefresh = false;
            }
            this.OnPopupInvoked();
        }

        /// <summary>
        /// Loads the options.
        /// </summary>
        private void LoadOptions()
        {
            Type currentColumnType = null;
            if (this.VisibleColumn != null)
                currentColumnType = this.VisibleColumn.ColumnType != null ? this.VisibleColumn.ColumnType : typeof(string);
            if (currentColumnType != null)
            {
                string[] items = null;
                if (currentColumnType == typeof(string))
                {
                    items = new string[]
                    {
                        GridDataResourceWrapper.StartsWith,
                        GridDataResourceWrapper.EndsWith,
                        GridDataResourceWrapper.AdvanceFilteringContainsString,
                        GridDataResourceWrapper.AdvanceFilteringEqualsString,
                        GridDataResourceWrapper.NotEquals
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
                        GridDataResourceWrapper.GreaterThanOrEqual
                    };
                }
                //This is to preserve the currentfiltertype which is set in the xmal
                string currentFilterTypeString = this.CurrentFilterType.ToString();
                if (this.OptionCombo != null)
                {
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

                        this.OptionCombo.SelectedIndex = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Called when [set data context].
        /// </summary>
        /// <param name="wrapperInstance">The wrapper instance.</param>
        protected override void OnSetDataContext(GridDataFilterWrapper wrapperInstance)
        {
            base.OnSetDataContext(wrapperInstance);
            var previousFiltervalue = wrapperInstance.FilterValue;
#if !SILVERLIGHT
            var filterValueBinding = new Binding("FilterValue") { Source = this.DataContext, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
#else
            var filterValueBinding = new Binding("FilterValue") { Source = this.DataContext, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.Default };
#endif
            this.SetBinding(GridDataTextFilteringPane.TextProperty, filterValueBinding);

            if (previousFiltervalue != null && wrapperInstance.FilterValue != previousFiltervalue)
            {
                wrapperInstance.FilterValue = previousFiltervalue;
                this.Text = previousFiltervalue.ToString();
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
            this.Text = string.Empty;
        }

        #endregion

    }   

}
