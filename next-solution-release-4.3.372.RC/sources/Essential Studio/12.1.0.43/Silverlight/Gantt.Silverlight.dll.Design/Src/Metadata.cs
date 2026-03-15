#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Microsoft.Windows.Design.Metadata;

#if SyncfusionFramework4_0 || SyncfusionSLFramework4_0
// ReSharper disable RedundantUsingDirective
using Microsoft.Windows.Design.Features;
// ReSharper restore RedundantUsingDirective

[assembly: ProvideMetadata(typeof(Syncfusion.Gantt.Silverlight.dll.Design.Metadata))]
#endif

namespace Syncfusion.Gantt.Silverlight.dll.Design
{
    // ReSharper disable RedundantUsingDirective
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Windows.Design.Metadata;
    using System.ComponentModel;
    using System.Windows.Media;
    using System.Windows.Controls;
    using System.Windows;
    using Microsoft.Windows.Design;
    // ReSharper disable RedundantNameQualifier
    using Syncfusion.Windows.Controls.Gantt;
    using Syncfusion.Windows.Controls.Gantt.Chart;
    using Syncfusion.Windows.Controls.Gantt.Grid;
    using Syncfusion.Windows.Controls.Gantt.Schedule;
    // ReSharper restore RedundantNameQualifier
    // ReSharper restore RedundantUsingDirective

#if SyncfusionFramework4_0 || SyncfusionSLFramework4_0
    internal class Metadata : IProvideAttributeTable
    {
        public AttributeTable AttributeTable
        {
            get
            {
                // ReSharper disable SuggestUseVarKeywordEvident
                AttributeTableBuilder builder = new AttributeTableBuilder();
                // ReSharper restore SuggestUseVarKeywordEvident
                // tool box filtering                
                // Gantt Base
                builder.AddCustomAttributes(typeof(TaskDetails), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Predecessor), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TaskAttributeMapping), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Resource), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(GanttTime), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TaskDetailsCollection), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(StripLineInfo), new ToolboxBrowsableAttribute(false));

                //Gantt Control
                builder.AddCustomAttributes(typeof(GanttControl), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(GanttModel), new ToolboxBrowsableAttribute(false));


                //Gantt Grid
                builder.AddCustomAttributes(typeof(GanttGrid), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PredecessorCellModel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ResourceCellRenderer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ResourceCellModel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ResourceCellRenderer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(AddNewColumnCellModel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(AddNewColumnCellRenderer), new ToolboxBrowsableAttribute(false));

                //Gantt Chart
                builder.AddCustomAttributes(typeof(GanttChart), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GanttChartRow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GanttChartItemsPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GanttChartBackgroundPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GanttChartRowItemsPresenter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GanttNodeConnector), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GanttNode), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GanttChartStripLinePanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(StripLine), new ToolboxBrowsableAttribute(false));

                //XML Import Export
                builder.AddCustomAttributes(typeof(MSProjectXMLExportImport), new ToolboxBrowsableAttribute(false));

                //Gantt Schedule
                builder.AddCustomAttributes(typeof(GanttSchedule), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GanttScheduleCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GanttScheduleRow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GanttScheduleRowPanel), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(DependencyObjectExtensions), new ToolboxBrowsableAttribute(false));
                
                return builder.CreateTable();
            }
        }
    }
#else

    internal class Metadata : IRegisterMetadata
    {

        // Called by Cider to register any design-time metadata
        public void Register()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();
            // tool box filtering                
              // Gantt Base
                builder.AddCustomAttributes(typeof(TaskDetails), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Predecessor), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TaskAttributeMapping), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Resource), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(GanttTime), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TaskDetailsCollection), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(StripLineInfo), new ToolboxBrowsableAttribute(false));

                //Gantt Control
                builder.AddCustomAttributes(typeof(GanttControl), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(GanttModel), new ToolboxBrowsableAttribute(false));


                //Gantt Grid
                builder.AddCustomAttributes(typeof(GanttGrid), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PredecessorCellModel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ResourceCellRenderer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ResourceCellModel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ResourceCellRenderer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(AddNewColumnCellModel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(AddNewColumnCellRenderer), new ToolboxBrowsableAttribute(false));

                //Gantt Chart
                builder.AddCustomAttributes(typeof(GanttChart), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GanttChartRow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GanttChartItemsPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GanttChartBackgroundPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GanttChartRowItemsPresenter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GanttNodeConnector), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GanttNode), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GanttChartStripLinePanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(StripLine), new ToolboxBrowsableAttribute(false));

                //XML Import Export
                builder.AddCustomAttributes(typeof(MSProjectXMLExportImport), new ToolboxBrowsableAttribute(false));

                //Gantt Schedule
                builder.AddCustomAttributes(typeof(GanttSchedule), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GanttScheduleCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GanttScheduleRow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GanttScheduleRowPanel), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(DependencyObjectExtensions), new ToolboxBrowsableAttribute(false));
            
            //// Category Editor
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
#endif
}