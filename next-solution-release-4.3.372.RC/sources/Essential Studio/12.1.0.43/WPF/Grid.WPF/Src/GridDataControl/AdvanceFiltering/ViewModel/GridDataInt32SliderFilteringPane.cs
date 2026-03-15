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
    public class GridDataInt32SliderFilteringPane : GridDataSliderFilteringPane<int?>
    {
        #region Ctor

        static GridDataInt32SliderFilteringPane()
        {
#if !SILVERLIGHT
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridDataInt32SliderFilteringPane), new FrameworkPropertyMetadata(typeof(GridDataInt32SliderFilteringPane)));
            MinValueProperty.OverrideMetadata(typeof(GridDataInt32SliderFilteringPane), new PropertyMetadata(0));
            MaxValueProperty.OverrideMetadata(typeof(GridDataInt32SliderFilteringPane), new PropertyMetadata(100));
            IsSnapToTickEnabledProperty.OverrideMetadata(typeof(GridDataInt32SliderFilteringPane),new PropertyMetadata(true));
#endif
        }

        public GridDataInt32SliderFilteringPane()
        {
#if SILVERLIGHT
            this.DefaultStyleKey = typeof(GridDataInt32SliderFilteringPane);
            this.MinValue = 0;
            this.MaxValue = 100;
            this.IsFilterSetToNone = true;
            this.IsSnapToTickEnabled = true;
#endif
        }

        #endregion

        #region Properties

        internal Slider FilterSlider
        {
            get;
            set;
        }

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

        #region Methods

        public override void OnApplyTemplate()
        {
            
            if (this.ClearButton != null)
                this.ClearButton.Click -= new RoutedEventHandler(ClearButton_Click);
            base.OnApplyTemplate();
            this.FilterSlider = this.GetTemplateChild("PART_Slider") as Slider;
            this.PART_CloseButton = this.GetTemplateChild("PART_CloseButton") as Button;
            if (this.PART_CloseButton != null)
            {
                this.PART_CloseButton.Click -= new RoutedEventHandler(CloseButton_Click);
                this.PART_CloseButton.Click += new RoutedEventHandler(CloseButton_Click);
            }
            this.LoadOptions();
            
            if (this.ClearButton != null)
                this.ClearButton.Click += new RoutedEventHandler(ClearButton_Click);
            this.PredicateType = PredicateType.And;

            this.ORRadioButton = this.GetTemplateChild("PART_ORRadioButton") as RadioButton;
            this.ANDRadioButton = this.GetTemplateChild("PART_ANDRadioButton") as RadioButton;
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

        private void LoadOptions()
        {
#if SILVERLIGHT

            this.OptionCombo.Items.Clear();
#endif
            if (this.VisibleColumn != null && this.VisibleColumn.ColumnType != null)
            {
                string[] items = null;
                items = new string[]
                    {
                        GridDataResourceWrapper.None,
                        GridDataResourceWrapper.AdvanceFilteringGreaterThanString,
                        GridDataResourceWrapper.GreaterThanOrEqual,
                        GridDataResourceWrapper.LessThan ,
                        GridDataResourceWrapper.LessThanOrEqual,
                        GridDataResourceWrapper.AdvanceFilteringEqualsString,
                        GridDataResourceWrapper.NotEquals
                    };
                //This is to preserve the currentfiltertype which is set in the xmal
                string currentFilterTypeString = this.CurrentFilterType.ToString();
                this.OptionCombo.ItemsSource = items;
                //Here the currentfiltertype is set from the OptionComboselectionchanged, so if the currentfiltertype is not Undefined which means currentfiltertype is set from the Xaml. In this case the combobox selected value is set to that value.
                this.OptionCombo.SelectedItem = currentFilterTypeString == "Undefined" ? this.CurrentFilterType.ToString() : currentFilterTypeString;
                if (this.OptionCombo.SelectedIndex == -1)
                {
                    this.OptionCombo.SelectedIndex = 0;
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
        void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            this.Value = null;
            this.OptionCombo.SelectedValue = GridDataResourceWrapper.None;
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
            if (content == GridDataResourceWrapper.OR)
            {
                this.PredicateType = PredicateType.Or;
            }
            else if (content == GridDataResourceWrapper.AND)
            {
                this.PredicateType = PredicateType.And;
            }           
            this.OnPopupInvoked();
        }

        #endregion

    }
}
