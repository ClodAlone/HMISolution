#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.Windows.Controls;
using Syncfusion.PivotAnalysis.Base;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.PivotAnalysis.Base.Silverlight;

namespace Syncfusion.Silverlight.Controls.PivotSchemaDesigner
{
    /// <summary>
    /// Interaction logic for ComputationInfoWindow.xaml
    /// </summary>
 #if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public partial class ComputationInfoWindow : WindowControl
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value><c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
        internal bool IsDirty { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is calculation changed.
        /// </summary>
        /// <value><c>true</c> if this instance is calculation changed; otherwise, <c>false</c>.</value>
        internal bool IsCalculationChanged { get; set; }

        /// <summary>
        /// Gets or Sets a value indicating whether this instance is headers changed
        /// </summary>
        internal bool IsHeadersChanged { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComputationInfoWindow" /> class.
        /// </summary>
        public ComputationInfoWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComputationInfoWindow" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public ComputationInfoWindow(object context)
        {
            InitializeComponent();
            InitializeSettings(context, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComputationInfoWindow" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="pivotRowAndColumns">The pivot row and columns.</param>
        public ComputationInfoWindow(object context, string[] pivotRowAndColumns)
        {
            InitializeComponent();
            InitializeSettings(context, pivotRowAndColumns);
        }

        /// <summary>
        /// Initializes the settings.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="pivotRowAndColumns">The pivot row and columns.</param>
        private void InitializeSettings(object context, string[] pivotRowAndColumns)
        {
            var array = Syncfusion.Silverlight.Controls.PivotGrid.Common.GetValues(typeof(SummaryType));
            foreach (var item in array)
            {
                this.cmbSummaryType.Items.Add(item);
            }

            array = Syncfusion.Silverlight.Controls.PivotGrid.Common.GetValues(typeof(CalculationType));
            foreach (var item in array)
            {
                this.cmbCalculationType.Items.Add(item);
            }
            this.cmbSummaryType.SelectionChanged += new SelectionChangedEventHandler(cmbSummaryType_SelectionChanged);
            this.cmbCalculationType.SelectionChanged += new SelectionChangedEventHandler(CmbCalculationType_SelectionChanged);
            this.cmbBaseField.SelectionChanged+=new SelectionChangedEventHandler(CmbBaseField_SelectionChanged);
            PivotComputationInfo computationInfo = context as PivotComputationInfo;
            if (computationInfo != null)
            {
                this.ComputationInfo = computationInfo;
                this.ComputationInfoClone = new PivotComputationInfo();
                this.ComputationInfoClone.CalculationName = computationInfo.CalculationName;
                this.ComputationInfoClone.Description = computationInfo.Description;
                this.ComputationInfoClone.FieldName = computationInfo.FieldName;
                this.ComputationInfoClone.FieldHeader = computationInfo.FieldHeader;
                this.ComputationInfoClone.Format = computationInfo.Format;
                this.ComputationInfoClone.SummaryType = computationInfo.SummaryType;
                this.ComputationInfoClone.CalculationType = computationInfo.CalculationType;
                this.ComputationInfo.AllowRunTimeGroupByField = computationInfo.AllowRunTimeGroupByField;
                this.DataContext = this.ComputationInfoClone;

                if (!(this.ComputationInfo.SummaryType == SummaryType.Custom))
                    this.cmbSummaryType.SelectedItem = this.ComputationInfo.SummaryType;
                this.cmbCalculationType.SelectedItem = this.ComputationInfo.CalculationType;
            }
            if (pivotRowAndColumns != null)
            {
                for (int i = 0; i < pivotRowAndColumns.Length; i++)
                {
                    this.cmbBaseField.Items.Add(pivotRowAndColumns[i]);
                }                
            }
        }
        
        /// <summary>
        /// Handles the SelectionChanged event of the BaseField change.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        void CmbBaseField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ComputationInfoClone.BaseField = (this.cmbBaseField.SelectedItem != null && this.ComputationInfoClone.CalculationType == CalculationType.PercentageOfParentTotal) ? this.cmbBaseField.SelectedItem.ToString() : null;
        }

        /// <summary>
        /// Handles the SelectionChanged event of the CalculationType change.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        void CmbCalculationType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ComputationInfoClone.CalculationType = (CalculationType)this.cmbCalculationType.SelectedItem;
            if (this.ComputationInfoClone.CalculationType == CalculationType.PercentageOfParentTotal)
            {
                this.cmbBaseField.IsEnabled = true;
                this.cmbBaseField.SelectedIndex = 0;
            }
            else
            {
                this.cmbBaseField.IsEnabled = false;
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the cmbSummaryType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        void cmbSummaryType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cmbSummaryType.SelectedItem is SummaryType)
                this.ComputationInfoClone.SummaryType = (SummaryType)this.cmbSummaryType.SelectedItem;
            else
                this.ComputationInfoClone.SummaryType = SummaryType.Custom;
        }

        /// <summary>
        /// Gets or sets the computation info object.
        /// </summary>
        /// <value>The computation info.</value>
        public PivotComputationInfo ComputationInfo
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the computation info clone object.
        /// </summary>
        /// <value>The computation info clone.</value>
        public PivotComputationInfo ComputationInfoClone
        {
            get;
            set;
        }

        /// <summary>
        /// Handles the Click event of the OK button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (this.ComputationInfo.SummaryType != this.ComputationInfoClone.SummaryType ||
                this.ComputationInfo.Format != this.ComputationInfoClone.Format)
            {
                this.ComputationInfo.FieldHeader = this.ComputationInfoClone.FieldHeader;
                this.ComputationInfo.Description = this.ComputationInfoClone.Description;
                this.ComputationInfo.Format = this.ComputationInfoClone.Format;
                this.ComputationInfo.SummaryType = this.ComputationInfoClone.SummaryType;
                this.ComputationInfo.CalculationType = this.ComputationInfoClone.CalculationType;
                this.ComputationInfo.BaseField =  this.ComputationInfoClone.BaseField;
                this.IsHeadersChanged = false;
                this.IsDirty = true;
            }
            else if (this.ComputationInfo.FieldHeader != this.ComputationInfoClone.FieldHeader ||
                this.ComputationInfo.Description != this.ComputationInfoClone.Description)
            {
                this.ComputationInfo.FieldHeader = this.ComputationInfoClone.FieldHeader;
                this.ComputationInfo.Description = this.ComputationInfoClone.Description;
                this.IsHeadersChanged = true;
                this.IsDirty = false;
            }
            else if (this.ComputationInfo.CalculationType != this.ComputationInfoClone.CalculationType ||
                (this.ComputationInfoClone.CalculationType == CalculationType.PercentageOfParentTotal && this.ComputationInfo.BaseField != this.ComputationInfoClone.BaseField))
            {
                this.ComputationInfo.CalculationType = this.ComputationInfoClone.CalculationType;
                this.ComputationInfo.BaseField = (this.ComputationInfoClone.CalculationType == CalculationType.PercentageOfParentTotal) ? this.ComputationInfoClone.BaseField : null;
                this.IsDirty = false;
                this.IsCalculationChanged = true;
                this.IsHeadersChanged = false;
            }
            else
            {
                this.IsDirty = false;
                this.IsCalculationChanged = false;
                this.IsHeadersChanged = false;
            }
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the Cancel button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
