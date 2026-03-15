#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/
// The ProvideMetadata assembly-level attribute indicates to designers
// that this assembly contains a class that provides an attribute table. 
//using Microsoft.Windows.Design.Metadata;
using Syncfusion.Windows.Controls.Gantt;
using Syncfusion.Windows.Controls.Gantt.Chart;
using Syncfusion.Windows.Controls.Gantt.Grid;
using Syncfusion.Windows.Controls.Gantt.Schedule;

#if SyncfusionFramework4_0
// ReSharper disable RedundantUsingDirective
using Microsoft.Windows.Design.Features;
// ReSharper restore RedundantUsingDirective
using Microsoft.Windows.Design.Metadata;
using Syncfusion.Windows.Controls.Gantt;
using Syncfusion.Windows.Controls.Gantt;
using Syncfusion.Windows.Controls.Gantt.Grid;
using Syncfusion.Windows.Controls.Gantt.Schedule;

#endif
#if SyncfusionFramework4_0
[assembly: ProvideMetadata(typeof(Syncfusion.Gantt.WPF.VisualStudio.Design.Metadata))]
#endif
namespace Syncfusion.Gantt.WPF.VisualStudio.Design
{
    // ReSharper disable RedundantUsingDirective
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Windows.Design.Features;
    using System.ComponentModel;
    using System.Windows.Media;
    using System.Windows.Controls;
    using System.Windows;
    using Microsoft.Windows.Design;
    using Microsoft.Windows.Design.Interaction;
    using Microsoft.Windows.Design.Metadata;
    // ReSharper restore RedundantUsingDirective



#if SyncfusionFramework4_0
    public class Metadata : Microsoft.Windows.Design.Metadata.IProvideAttributeTable
    {
        // Accessed by the designer to register any design-time metadata.
        public Microsoft.Windows.Design.Metadata.AttributeTable AttributeTable
        {
            get
            {
// ReSharper disable SuggestUseVarKeywordEvident
                AttributeTableBuilder builder = new AttributeTableBuilder();
// ReSharper restore SuggestUseVarKeywordEvident
                // Add the menu provider to the design-time metadata.
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
                builder.AddCustomAttributes(typeof(XMLImportExport), new ToolboxBrowsableAttribute(false));

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

    internal class Metadata : IRegisterMetadata {

        // Called by Cider to register any design-time metadata
        public void Register() {
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
                builder.AddCustomAttributes(typeof(XMLImportExport), new ToolboxBrowsableAttribute(false));

                //Gantt Schedule
                builder.AddCustomAttributes(typeof(GanttSchedule), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GanttScheduleCell), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GanttScheduleRow), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GanttScheduleRowPanel), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(DependencyObjectExtensions), new ToolboxBrowsableAttribute(false));

            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
#endif
}
