#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.Collections.ObjectModel;
using Syncfusion.PivotAnalysis.Base.Silverlight;
using System.Windows.Controls.Primitives;
using Syncfusion.Windows.Shared.Controls;
using Syncfusion.Silverlight.Controls.PivotGrid.Resources;

namespace Syncfusion.Silverlight.Controls.PivotGrid
{
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public partial class PivotGridComputationListWindow : Syncfusion.Windows.Tools.Controls.WindowControl
    {
        #region [ Public Properties ]

        public ObservableCollection<PivotComputationInfo> PivotComputationFields { get; set; }

        internal string FormatText { get; set; }

        internal PivotComputationInfo DragPivotItem { get; set; }

        internal object DroppedItem { get; set; }

        internal PivotGridControl GridControl
        {
            get
            {
                return (PivotGridControl)GetValue(GridControlProperty);
            }

            set
            {
                SetValue(GridControlProperty, value);
            }
        }
        #endregion

        #region [ Dependency Property Implementation ]

        public static readonly DependencyProperty GridControlProperty =
           DependencyProperty.Register("GridControl", typeof(PivotGridControl), typeof(PivotGridComputationListWindow), new PropertyMetadata(null));
        #endregion

        #region [Initialize / Finalize ]

        /// <summary>
        /// Initializes a new instance of the <see cref="PivotGridFieldList"/> class.
        /// </summary>
        /// <param name="GridControl">The grid control.</param>        
        public PivotGridComputationListWindow(PivotGridControl GridControl)
        {
            InitializeComponent();
            this.GridControl = GridControl;
            this.Title =  SR.GetString(System.Globalization.CultureInfo.CurrentUICulture, "PivotGrid_PivotComputationList");
            this.PivotComputationFields = GridControl.PivotCalculations;
            this.ComputationListPanel.DataContext = this.PivotComputationFields;
            this.ComputationListPanel.ItemsSource = this.PivotComputationFields;
            this.wireEvents();
        }

        #endregion

        #region [ Private Methods ]

         /// <summary>
        /// Handles the LayoutUpdated event of the PivotGroupingItemsControl(ComputationListPanel).
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void ComputationListPanel_LayoutUpdated(object sender, EventArgs e)
        {
            this.GridControl.GroupingBar.FormatItems(this.ComputationListPanel);
            this.GridControl.GroupingBar.ContextMenuManipulation(this.ComputationListPanel);
        }

        /// <summary>
        /// Handles the event of Drag and drop operation of the items to and from the PivotGridComputationList Window.
        /// </summary>
        private void wireEvents()
        {
            if (this.ComputationListPanel != null)
            {
                this.ComputationListPanel.ArrangeOverrideExecute += new ArrangeOverrideExecuted(this.GridControl.GroupingBar.PivotItem_ArrangeOverrideExecute);
                this.ComputationListPanel.LayoutUpdated += new EventHandler(ComputationListPanel_LayoutUpdated);
            }

            DragAndDropManager.Drag -= new DragDropEventHandler(this.GridControl.GroupingBar.DragAndDropManager_Drag);
            DragAndDropManager.Drag += new DragDropEventHandler(this.GridControl.GroupingBar.DragAndDropManager_Drag);
            DragAndDropManager.DragStarted -= new DragDropEventHandler(this.GridControl.GroupingBar.DragAndDropManager_DragStarted);
            DragAndDropManager.DragStarted += new DragDropEventHandler(this.GridControl.GroupingBar.DragAndDropManager_DragStarted);
            DragAndDropManager.Drop -= new DragDropEventHandler(this.GridControl.GroupingBar.DragAndDropManager_Drop);
            DragAndDropManager.Drop += new DragDropEventHandler(this.GridControl.GroupingBar.DragAndDropManager_Drop);
        }

        #endregion
    }
}
