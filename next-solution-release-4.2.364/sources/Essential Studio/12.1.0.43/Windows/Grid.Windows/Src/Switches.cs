//-------------------------------------------------------------------------------------------------
// <copyright file="Switches.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    ///    Provides predefined switches for enabling/disabling trace
    ///    output or code instrumentation in the Grid library.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    sealed class Switches
    {
        public static TraceSwitch Timers = new TraceSwitch("Grid.Timers", "Enable Tracing for Timer Start and Stop");

        static Switches()
        {
    ////        Switches.CurrentCell.Level = TraceLevel.Verbose;
            ////Switches.CellRenderer.Level = TraceLevel.Verbose;
            ////Switches.GridPaint.Level = TraceLevel.Verbose;
    ////        Switches.TextBoxCellEvents.Level= TraceLevel.Verbose;
////WL            Switches.GridFocus.Level = TraceLevel.Verbose;
////WL            Switches.GridFocusEvents.Level = TraceLevel.Verbose;
            ////CurCellNestedGrid.Level = TraceLevel.Verbose;
        ////    Switches.MouseController.Level = TraceLevel.Verbose;
        }

        /// <summary>
        /// Gets General Tracing level for the Grid Library
        /// </summary>
        public static TraceSwitch General
        {
            get 
            {
                if (general == null)
                {
                    general = new TraceSwitch("Grid.General", "Enable Tracing for the Grid Library");
                }

                return general;
            }
        }

        public static TraceSwitch VolatileData
        {
            get 
            {
                if (volatileData == null)
                {
                    volatileData = new TraceSwitch("Grid.VolatileData", "Debug VolatileData");
                }

                return volatileData;
            }
        }

        public static TraceSwitch ValueConversion
        {
            get 
            {
                if (valueConversion == null)
                {
                    valueConversion = new TraceSwitch("Grid.ValueConversion", "Debug Cell Value Conversion");
                }

                return valueConversion;
            }
        }

        public static TraceSwitch FloatCells
        {
            get 
            {
                if (Switches.floatCells == null)
                {
                    Switches.floatCells = new TraceSwitch("Grid.FloatCells", "Debug Floating Cells");
                }

                return Switches.floatCells;
            }
        }

        public static TraceSwitch ButtonChild
        {
            get 
            {
                if (Switches.buttonChild == null)
                {
                    Switches.buttonChild = new TraceSwitch("Grid.ButtonChild", "Debug GridButtonChild");
                }

                return Switches.buttonChild;
            }
        }

        public static TraceSwitch KeyboardEvents
        {
            get 
            {
                if (Switches.keyboardEvents == null)
                {
                    Switches.keyboardEvents = new TraceSwitch("Grid.KeyboardEvents", "Debug keyboard events");
                }

                return Switches.keyboardEvents;
            }
        }

        public static TraceSwitch TextBoxCellEvents
        {
            get 
            {
                if (Switches.textBoxCellEvents == null)
                {
                    Switches.textBoxCellEvents = new TraceSwitch("Grid.TextBoxCellEvents", "Debug textbox cell events");
                }

                return Switches.textBoxCellEvents;
            }
        }

        public static TraceSwitch SelectRange
        {
            get 
            {
                if (Switches.selectRange == null)
                {
                    Switches.selectRange = new TraceSwitch("Grid.SelectRange", "Debug SelectRange operations");
                }

                return Switches.selectRange;
            }
        }

        public static TraceSwitch Printing
        {
            get 
            {
                if (Switches.printing == null)
                {
                    Switches.printing = new TraceSwitch("Grid.Printing", "Debug Printing operations");
                }

                return Switches.printing;
            }
        }

        public static TraceSwitch ResizeToFit
        {
            get 
            {
                if (Switches.resizeToFit == null)
                {
                    Switches.resizeToFit = new TraceSwitch("Grid.ResizeToFit(Syncfusion.Windows.Forms.Grid.GridRangeInfo)", "Debug ResizeToFit operations");
                }

                return Switches.resizeToFit;
            }
        }

        public static TraceSwitch DataExchange
        {
            get 
            {
                if (Switches.dataExchange == null)
                {
                    Switches.dataExchange = new TraceSwitch("Grid.DataExchange", "Debug DataExchange operations");
                }

                return Switches.dataExchange;
            }
        }

        public static TraceSwitch OutlineCurrentCellHeader
        {
            get 
            {
                if (Switches.markEdit == null)
                {
                    Switches.markEdit = new TraceSwitch("Grid.OutlineCurrentCellHeader", "Debug OutlineCurrentCellHeader operations");
                }

                return Switches.markEdit;
            }
        }

        public static TraceSwitch CustomStylePropertyLifetime
        {
            get 
            {
                if (Switches.userAttributeLifetime == null)
                {
                    Switches.userAttributeLifetime = new TraceSwitch("Grid.CustomStylePropertyLifetime", "Debug Userattribute creation and collection");
                }

                return Switches.userAttributeLifetime;
            }
        }

        public static TraceSwitch DelayedRange
        {
            get 
            {
                if (Switches.delayedRange == null)
                {
                    Switches.delayedRange = new TraceSwitch("Grid.DelayedRange", "Debug DelayedRange operations");
                }

                return Switches.delayedRange;
            }
        }

        public static TraceSwitch GridLayout
        {
            get 
            {
                if (Switches.gridLayout == null)
                {
                    Switches.gridLayout = new TraceSwitch("Grid.GridLayout", "Debug grid layout operations");
                }

                return Switches.gridLayout;
            }
        }

        public static TraceSwitch GridPaint
        {
            get 
            {
                if (Switches.gridPaint == null)
                {
                    Switches.gridPaint = new TraceSwitch("Grid.GridPaint", "Debug grid paint operations");
                }

                return Switches.gridPaint;
            }
        }

        public static TraceSwitch GridScrolling
        {
            get 
            {
                if (Switches.gridScrolling == null)
                {
                    Switches.gridScrolling = new TraceSwitch("Grid.GridScrolling", "Debug grid scrolling operations");
                }

                return Switches.gridScrolling;
            }
        }

        public static TraceSwitch Scrolling
        {
            get 
            {
                if (Switches.scrolling == null)
                {
                    Switches.scrolling = new TraceSwitch("Grid.Scrolling", "Debug scrolling operations");
                }

                return Switches.scrolling;
            }
        }

        public static TraceSwitch MouseEvents
        {
            get 
            {
                if (Switches.mouseEvents == null)
                {
                    Switches.mouseEvents = new TraceSwitch("Grid.MouseEvents", "Debug mouse events");
                }

                return Switches.mouseEvents;
            }
        }

        public static TraceSwitch TabBarControl
        {
            get 
            {
                if (Switches.tabBarControl == null)
                {
                    Switches.tabBarControl = new TraceSwitch("Grid.TabBarControl", "Debug TabBarControl operations");
                }

                return Switches.tabBarControl;
            }
        }

        public static TraceSwitch CheckBoxCellEvents
        {
            get 
            {
                if (Switches.checkBoxCellEvents == null)
                {
                    Switches.checkBoxCellEvents = new TraceSwitch("Grid.CheckBoxCellEvents", "Debug checkbox cell operations");
                }

                return Switches.checkBoxCellEvents;
            }
        }

        public static TraceSwitch BeginEndUpdate
        {
            get 
            {
                if (Switches.beginEndUpdate == null)
                {
                    Switches.beginEndUpdate = new TraceSwitch("Grid.BeginEndUpdate", "Debug BeginUpdate and EndUpdate operations");
                }

                return Switches.beginEndUpdate;
            }
        }

        public static TraceSwitch GroupingEngine = new TraceSwitch("Grid.GroupingEngine", "Debug Grouping Ending");
        public static TraceSwitch GroupingGrid = new TraceSwitch("Grid.GroupingGrid", "Debug Grouping Grid");
        public static TraceSwitch CurCellNestedGrid = new TraceSwitch("Grid.CurCellNestedGrid", "Debug Grouping Grid");
        
        public static TraceSwitch MergeCells = new TraceSwitch("Grid.MergeCells", "Debug Merge Cells");
        public static TraceSwitch Serialization = new TraceSwitch("Grid.Serialization", "Debug Grid Serialization");
        public static TraceSwitch CommandStack = new TraceSwitch("Grid.CommandStack", "Debug Undo and Redo operations");
        public static TraceSwitch CurrentCell = new TraceSwitch("Grid.CurrentCell", "Debug CurrentCell operations");
        public static TraceSwitch CellRenderer = new TraceSwitch("Grid.CellRenderer", "Debug Cell Renderer operations");
        public static TraceSwitch GridListControl = new TraceSwitch("Grid.GridListControl", "Debug GridListControl operations");
        public static TraceSwitch GridModel = new TraceSwitch("Grid.GridModel", "Debug GridModel operations");
        public static TraceSwitch MouseController = new TraceSwitch("Grid.MouseController", "Debug MouseController operations");
        public static TraceSwitch DragDrop = new TraceSwitch("Grid.DragDrop", "Debug DragDrop operations");
        public static TraceSwitch Development = new TraceSwitch("Grid.Development", "Trace Helper for Development of EG");
        public static TraceSwitch GridBorderPaint = new TraceSwitch("Grid.GridBorderPaint", "Trace Helper for GridBorderPaint methods");

        public static TraceSwitch GridControlBaseEvents = new TraceSwitch("Grid.GridControlBaseEvents", "Trace Helper for all GridControlBase events");
        public static TraceSwitch GridModelEvents = new TraceSwitch("Grid.GridModelEvents", "Trace Helper for all GridModel events");
        public static TraceSwitch GridDataBoundGridEvents = new TraceSwitch("Grid.GridDataBoundGridEvents", "Trace Helper for all GridDataBoundGrid events");
        public static TraceSwitch GridListControlEvents = new TraceSwitch("Grid.GridListControlEvents", "Trace Helper for all GridListControl events");
        public static TraceSwitch GridModelDataBinderEvents = new TraceSwitch("Grid.GridModelDataBinderEvents", "Trace Helper for all GridModelDataBinder events");
        public static TraceSwitch GridBoundColumnEvents = new TraceSwitch("Grid.GridBoundColumnEvents", "Trace Helper for all GridBoundColumn events");
        public static TraceSwitch GridCellModelBaseEvents = new TraceSwitch("Grid.GridCellModelBaseEvents", "Trace Helper for all GridCellModelBase events");
        public static TraceSwitch GridCellButtonEvents = new TraceSwitch("Grid.GridCellButtonEvents", "Trace Helper for all GridCellButton events");
        public static TraceSwitch GridFocus = new TraceSwitch("Grid.Focus", "Trace Helper for inside focus");
        public static TraceSwitch GridFocusEvents = new TraceSwitch("Grid.FocusEvents", "Trace Helper for all focus");

        public static TraceSwitch GridDataBoundGrid = new TraceSwitch("Grid.GridDataBoundGrid", "Trace Helper for various GridDataBoundGrid method calls");
        public static TraceSwitch FormulaCell = new TraceSwitch("Grid.FormulaCell", "Enable Tracing for Formula Cell");
        
        public static TraceSwitch GridDesigner = new TraceSwitch("Grid.GridDesigner", "Enable Tracing for Grid Control designer");
        
        private static TraceSwitch general;
        private static TraceSwitch beginEndUpdate;
        private static TraceSwitch volatileData;
        private static TraceSwitch valueConversion;
        private static TraceSwitch floatCells;
        private static TraceSwitch buttonChild;
        private static TraceSwitch keyboardEvents;
        private static TraceSwitch textBoxCellEvents;
        private static TraceSwitch selectRange;
        private static TraceSwitch printing;
        private static TraceSwitch resizeToFit;
        private static TraceSwitch dataExchange;
        private static TraceSwitch markEdit;
        private static TraceSwitch delayedRange;
        private static TraceSwitch gridLayout;
        private static TraceSwitch gridPaint;
        private static TraceSwitch gridScrolling;
        private static TraceSwitch scrolling;
        private static TraceSwitch mouseEvents;
        private static TraceSwitch tabBarControl;
        private static TraceSwitch checkBoxCellEvents;
        private static TraceSwitch userAttributeLifetime;
    }

    [Syncfusion.Documentation.DocumentationExclude()]
    internal sealed class GridTraceStack
    {
        public static void TraceExceptionCatched(Exception e)
        {
            StackTrace st = new StackTrace(1, true);
            StackFrame sf = st.GetFrame(StackTrace.METHODS_TO_SKIP);
            if (sf != null)
            {
                MethodBase mb = sf.GetMethod();
                Trace.WriteLine(e.ToString());
                StringBuilder sb = new StringBuilder();
                sb.Append("catched in File ");
                sb.Append(sf.GetFileName());
                sb.Append(", line ");
                sb.Append(sf.GetFileLineNumber());
                Trace.WriteLine(sb.ToString());
                sb = new StringBuilder();
                sb.Append("in method ");
                sb.Append(mb.ReflectedType.Name);
                sb.Append(".");
                sb.Append(mb.Name);
                sb.Append("(");
                bool first = true;
                foreach (ParameterInfo pi in mb.GetParameters())
                {
                    if (!first)
                    {
                        sb.Append(", ");
                    }

                    first = false;
                    sb.Append(pi.ParameterType.Name);
                    sb.Append(" ");
                    sb.Append(pi.Name);
                }   

                sb.Append(");");
                Trace.WriteLine(sb.ToString());
            }
        }

        public static object TraceMethodInfo()
        {
            if (Switches.General.TraceVerbose)
            {
                Trace.WriteLine("TraceVerbose");
            }
        {
            StackTrace st = new StackTrace(1, true);
            StackFrame sf = st.GetFrame(StackTrace.METHODS_TO_SKIP);
            if (sf != null)
            {
                MethodBase mb = sf.GetMethod();
                StringBuilder sb = new StringBuilder();
                sb.Append(mb.ReflectedType.Name);
                sb.Append(".");
                sb.Append(mb.Name);
                sb.Append("(");
                bool first = true;
                foreach (ParameterInfo pi in mb.GetParameters())
                {
                    if (!first)
                    {
                        sb.Append(", ");
                    }

                    first = false;
                    sb.Append(pi.ParameterType.Name);
                    sb.Append(" ");
                    sb.Append(pi.Name);
                }   

                sb.Append(");");

                int n = StackTrace.METHODS_TO_SKIP+1;
                sf = st.GetFrame(n);
                if (sf != null)
                {
                    sb.Append(" called from ");

                    do 
                    {
                        mb = sf.GetMethod();
                        sb.Append(mb.ReflectedType.Name);
                        sb.Append(".");
                        sb.Append(mb.Name);
                        sb.Append(":");
                        sf = st.GetFrame(++n);
                    }
                    while (sf != null);                        
                }

                Trace.WriteLine(sb.ToString());
            }
        }

            return null;
        }
    }
#if obsolete
    /// <summary>
    ///    Summary description for GridTrace.
    /// </summary>
     class GridTraceToFile
    {
        static bool traceToFile = false;
        static Stream file = null;
        static TextWriterTraceListener textListener = null;

        public GridTraceToFile()
        {
            Enabled = true;
        }

        public static bool Enabled
        {
            get
            {
                return traceToFile;
            }
            set
            {
                if (value != traceToFile)
                {
                    if (value)
                    {
                        try
                        {
                            file = File.Create(@"c:\styles2.txt");
                        }
                        catch (Exception ex)
                        {
                            TraceUtil.TraceExceptionCatched(ex);
                            return;
                        }

                        /* Create a new text writer using the output stream and add it to
                           the trace listeners. */
                        TextWriterTraceListener textListener = new TextWriterTraceListener(file);
                        Trace.Listeners.Add(textListener);
                    }
                    else
                    {
                        Trace.Listeners.Remove(textListener);
                        textListener.Flush();
                        textListener.Close();
                        file.Flush();
                        file.Close();
                        textListener = null;
                        file = null;
                    }
                    traceToFile = value;
                }
            }
        }
    }
#endif
}
