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
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.IO.IsolatedStorage;

namespace Syncfusion.Windows.Controls.Gantt
{
    /// <summary>
    /// Represents the class that provide methods to perform Import and Export operation.
    /// </summary>
    public class MSProjectXMLExportImport
    {
        /// <summary>
        /// Converts to XML.
        /// </summary>
        /// <param name="taskCollection">The col.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endTime">The end time.</param>
        /// <returns></returns>
        public static bool ConvertToXML(IEnumerable<TaskDetails> taskCollection, DateTime startDate, DateTime endTime)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog{ Filter="XML Files (*.XML)|*.xml"};
            bool? dialogResult = saveFileDialog.ShowDialog();

            if (dialogResult == true)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder = defaultMSProjXML(taskCollection, startDate, endTime, saveFileDialog.SafeFileName);
                XDocument xDocument = XDocument.Parse(stringBuilder.ToString());
                using (Stream stream = saveFileDialog.OpenFile())
                {
                    StreamWriter streamWriter = new StreamWriter(stream, System.Text.Encoding.UTF8);
                    streamWriter.Write(xDocument.ToString());
                    streamWriter.Close();
                    stream.Close();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Generates the UID for resources.
        /// </summary>
        /// <param name="resCol">The res col.</param>
        /// <param name="col">The col.</param>
        /// <param name="initialUID">The initial UID.</param>
        internal static void GenerateUIDForResources(Dictionary<string, int> resCol, IEnumerable<IGanttTask> col, int initialUID)
        {
            foreach (IGanttTask detail in col)
            {
                if (detail.Resources != null && detail.Resources.Count() > 0)
                {
                    foreach (Resource res in detail.Resources)
                    {
                        if (!resCol.ContainsKey(res.Name))
                        {
                            resCol.Add(res.Name, ++initialUID);
                        }
                    }
                }
                if (detail.Child.Count > 0)
                {
                    GenerateUIDForResources(resCol, detail.Child, initialUID);
                }
            }
        }

        /// <summary>
        /// Adds the calendar.
        /// </summary>
        /// <param name="ResCollection">The res collection.</param>
        /// <param name="xmlString">The XML string.</param>
        internal static void AddCalendar(Dictionary<string, int> ResCollection, StringBuilder xmlString)
        {
            foreach (KeyValuePair<string, int> pair in ResCollection)
            {
                xmlString.Append("<Calendar>");
                xmlString.Append("<UID>").Append(pair.Value.ToString()).Append("</UID>");
                xmlString.Append("<Name>").Append(pair.Key.ToString()).Append("</Name>");
                xmlString.Append("<IsBaseCalendar>0</IsBaseCalendar>");
                xmlString.Append("<BaseCalendarUID>1</BaseCalendarUID>");
                xmlString.Append("</Calendar>");
            }
        }

        /// <summary>
        /// Adds the resources.
        /// </summary>
        /// <param name="ResCollection">The res collection.</param>
        /// <param name="xmlString">The XML string.</param>
        internal static void AddResources(Dictionary<string, int> ResCollection, StringBuilder xmlString)
        {
            foreach (KeyValuePair<string, int> pair in ResCollection)
            {
                xmlString.Append("<Resource>");
                xmlString.Append("<UID>").Append(pair.Value.ToString()).Append("</UID>"); xmlString.Append("<ID>").Append(pair.Value.ToString()).Append("</ID>"); xmlString.Append("<Name>").Append(pair.Key.ToString()).Append("</Name>"); xmlString.Append("<Type>1</Type>"); xmlString.Append("<IsNull>0</IsNull>"); xmlString.Append("<WorkGroup>0</WorkGroup>"); xmlString.Append("<MaxUnits>1.00</MaxUnits>"); xmlString.Append("<PeakUnits>0.00</PeakUnits>");
                xmlString.Append("<OverAllocated>0</OverAllocated>"); xmlString.Append("<CanLevel>1</CanLevel>"); xmlString.Append("<AccrueAt>3</AccrueAt>"); xmlString.Append("<Work>PT0H0M0S</Work>"); xmlString.Append("<RegularWork>PT0H0M0S</RegularWork>"); xmlString.Append("<OvertimeWork>PT0H0M0S</OvertimeWork>");
                xmlString.Append("<ActualWork>PT0H0M0S</ActualWork>"); xmlString.Append("<RemainingWork>PT0H0M0S</RemainingWork>"); xmlString.Append("<ActualOvertimeWork>PT0H0M0S</ActualOvertimeWork>"); xmlString.Append("<RemainingOvertimeWork>PT0H0M0S</RemainingOvertimeWork>");
                xmlString.Append("<PercentWorkComplete>0</PercentWorkComplete>"); xmlString.Append("<StandardRate>0</StandardRate>"); xmlString.Append("<StandardRateFormat>2</StandardRateFormat>"); xmlString.Append("<Cost>0</Cost>"); xmlString.Append("<OvertimeRate>0</OvertimeRate>");
                xmlString.Append("<OvertimeRateFormat>2</OvertimeRateFormat>"); xmlString.Append("<OvertimeCost>0</OvertimeCost>"); xmlString.Append("<CostPerUse>0</CostPerUse>"); xmlString.Append("<ActualCost>0</ActualCost>"); xmlString.Append("<ActualOvertimeCost>0</ActualOvertimeCost>");
                xmlString.Append("<RemainingCost>0</RemainingCost>"); xmlString.Append("<RemainingOvertimeCost>0</RemainingOvertimeCost>"); xmlString.Append("<WorkVariance>0.00</WorkVariance>"); xmlString.Append("<CostVariance>0</CostVariance>"); xmlString.Append("<SV>0.00</SV>");
                xmlString.Append("<CV>0.00</CV>"); xmlString.Append("<ACWP>0.00</ACWP>"); xmlString.Append("<CalendarUID>2</CalendarUID>"); xmlString.Append("<BCWS>0.00</BCWS>"); xmlString.Append("<BCWP>0.00</BCWP>"); xmlString.Append("<IsGeneric>0</IsGeneric>");
                xmlString.Append("<IsInactive>0</IsInactive>"); xmlString.Append("<IsEnterprise>0</IsEnterprise>"); xmlString.Append("<BookingType>0</BookingType>"); xmlString.Append("<CreationDate>2010-08-21T00:14:00</CreationDate>"); xmlString.Append("<IsCostResource>0</IsCostResource>");
                xmlString.Append("<IsBudget>0</IsBudget>");
                xmlString.Append("</Resource>");
            }
        }

        /// <summary>
        /// Adds the assignment to resources.
        /// </summary>
        /// <param name="resCollection">The res collection.</param>
        /// <param name="taskCollection">The col.</param>
        /// <param name="xmlString">The XML string.</param>
        internal static void AddAssignmentToResources(Dictionary<string, int> resCollection, IEnumerable<IGanttTask> taskCollection, StringBuilder xmlString)
        {
            Dictionary<string, int> resCol = resCollection;
            foreach (IGanttTask detail in taskCollection)
            {
                if (detail.Resources != null && detail.Resources.Count() > 0)
                {
                    foreach (Resource res in detail.Resources)
                    {
                        if (resCol.ContainsKey(res.Name))
                        {
                            int val;
                            string RemainWork = string.Format("PT{0:0}H{1:0}M{2:0}S", 100 - detail.Progress, detail.Duration.Minutes, detail.Duration.Seconds);

                            resCol.TryGetValue(res.Name, out val);
                            xmlString.Append("<Assignment>");
                            xmlString.Append("<ResourceUID>").Append(val.ToString()).Append("</ResourceUID>");
                            xmlString.Append("<TaskUID>").Append(detail.TaskId.ToString()).Append("</TaskUID>");
                            xmlString.Append("<Units>1</Units>");
                            xmlString.Append("</Assignment>");
                        }
                    }
                }
                if (detail.Child.Count > 0)
                {
                    AddAssignmentToResources(resCol, detail.Child.Cast<IGanttTask>(), xmlString);
                }
            }
        }

        /// <summary>
        /// Defaults the MS proj XML.
        /// </summary>
        /// <param name="taskCollection">The task collection.</param>
        /// <param name="StartDate">The start date.</param>
        /// <param name="EndTime">The end time.</param>
        /// <param name="projName">Name of the proj.</param>
        /// <returns></returns>
        public static StringBuilder defaultMSProjXML(IEnumerable<TaskDetails> taskCollection, DateTime StartDate, DateTime EndTime, string projName)
        {
            StringBuilder xmlString = new StringBuilder();
            Dictionary<string, int> ResCollection = new Dictionary<string, int>();
            GenerateUIDForResources(ResCollection, taskCollection.Cast<IGanttTask>(), 1);
            string s = "http://schemas.microsoft.com/project";
            string startDate = String.Format("{0:yyyy-MM-ddTHH:mm:ss}", StartDate);
            string EndDate = String.Format("{0:yyyy-MM-ddTHH:mm:ss}", EndTime);
            string todayDate = String.Format("{0:yyyy-MM-ddTHH:mm:ss}", DateTime.Now);

            xmlString.Append("<?xml version=\'1.0\' encoding=\'UTF-8\' standalone=\'yes\'?>");
            xmlString.Append("<Project xmlns=").Append("'").Append(s).Append("'").Append(">");
            xmlString.Append("<Name>").Append(projName).Append("</Name>");
            xmlString.Append("<Author>rpraveen</Author>");
            xmlString.Append("<CreationDate>").Append(startDate).Append("</CreationDate>");
            xmlString.Append("<LastSaved>").Append(todayDate).Append("</LastSaved>");
            xmlString.Append("<ScheduleFromStart>2</ScheduleFromStart>");
            xmlString.Append("<StartDate>").Append(startDate).Append("</StartDate>");
            xmlString.Append("<FYStartDate>1</FYStartDate>");
            xmlString.Append("<CriticalSlackLimit>0</CriticalSlackLimit>");
            xmlString.Append("<CurrencyDigits>2</CurrencyDigits>");
            xmlString.Append("<CurrencySymbol>$</CurrencySymbol>");
            xmlString.Append("<CurrencyCode>USD</CurrencyCode>");
            xmlString.Append("<CurrencySymbolPosition>0</CurrencySymbolPosition>");
            xmlString.Append("<CurrencySymbolPosition>0</CurrencySymbolPosition>");
            xmlString.Append("<CalendarUID>1</CalendarUID>"); xmlString.Append("<DefaultStartTime>08:00:00</DefaultStartTime>"); xmlString.Append("<DefaultFinishTime>17:00:00</DefaultFinishTime>"); xmlString.Append("<MinutesPerDay>480</MinutesPerDay>");
            xmlString.Append("<MinutesPerWeek>2400</MinutesPerWeek>"); xmlString.Append("<DaysPerMonth>20</DaysPerMonth>"); xmlString.Append("<DefaultTaskType>0</DefaultTaskType>"); xmlString.Append("<DefaultFixedCostAccrual>3</DefaultFixedCostAccrual>");
            xmlString.Append("<DefaultStandardRate>0</DefaultStandardRate>"); xmlString.Append("<DefaultOvertimeRate>0</DefaultOvertimeRate>"); xmlString.Append("<DurationFormat>7</DurationFormat>"); xmlString.Append("<WorkFormat>2</WorkFormat>");
            xmlString.Append("<EditableActualCosts>0</EditableActualCosts>"); xmlString.Append("<HonorConstraints>0</HonorConstraints>"); xmlString.Append("<InsertedProjectsLikeSummary>1</InsertedProjectsLikeSummary>"); xmlString.Append("<MultipleCriticalPaths>0</MultipleCriticalPaths>");
            xmlString.Append("<NewTasksEffortDriven>1</NewTasksEffortDriven>"); xmlString.Append("<NewTasksEstimated>1</NewTasksEstimated>"); xmlString.Append("<SplitsInProgressTasks>1</SplitsInProgressTasks>"); xmlString.Append("<SpreadActualCost>0</SpreadActualCost>");
            xmlString.Append("<SpreadPercentComplete>0</SpreadPercentComplete>"); xmlString.Append("<TaskUpdatesResource>1</TaskUpdatesResource>"); xmlString.Append("<FiscalYearStart>0</FiscalYearStart>"); xmlString.Append("<WeekStartDay>0</WeekStartDay>");
            xmlString.Append("<MoveCompletedEndsBack>0</MoveCompletedEndsBack>"); xmlString.Append("<MoveRemainingStartsBack>0</MoveRemainingStartsBack>"); xmlString.Append("<MoveRemainingStartsForward>0</MoveRemainingStartsForward>");
            xmlString.Append("<MoveCompletedEndsForward>0</MoveCompletedEndsForward>"); xmlString.Append("<BaselineForEarnedValue>0</BaselineForEarnedValue>"); xmlString.Append("<AutoAddNewResourcesAndTasks>1</AutoAddNewResourcesAndTasks>");
            xmlString.Append("<CurrentDate>").Append(todayDate).Append("</CurrentDate>"); xmlString.Append("<MicrosoftProjectServerURL>1</MicrosoftProjectServerURL>"); xmlString.Append("<Autolink>1</Autolink>"); xmlString.Append("<NewTaskStartDate>0</NewTaskStartDate>");
            xmlString.Append("<DefaultTaskEVMethod>0</DefaultTaskEVMethod>"); xmlString.Append("<ProjectExternallyEdited>0</ProjectExternallyEdited>"); xmlString.Append("<ExtendedCreationDate>1984-01-01T00:00:00</ExtendedCreationDate>");
            xmlString.Append("<ActualsInSync>1</ActualsInSync>"); xmlString.Append("<RemoveFileProperties>0</RemoveFileProperties>"); xmlString.Append("<AdminProject>0</AdminProject>"); xmlString.Append("<OutlineCodes />"); xmlString.Append("<WBSMasks />");
            xmlString.Append("<ExtendedAttributes />");
            xmlString.Append("<Calendars>");
            xmlString.Append("<Calendar>");
            xmlString.Append("<UID>1</UID>"); xmlString.Append("<Name>Standard</Name>"); xmlString.Append("<IsBaseCalendar>1</IsBaseCalendar>"); xmlString.Append("<BaseCalendarUID>-1</BaseCalendarUID>");
            xmlString.Append("<WeekDays>");
            xmlString.Append("<WeekDay>");
            xmlString.Append("<DayType>1</DayType>"); xmlString.Append("<DayWorking>0</DayWorking>");
            xmlString.Append("</WeekDay>");
            xmlString.Append("<WeekDay>");
            xmlString.Append("<DayType>2</DayType>"); xmlString.Append("<DayWorking>1</DayWorking>");
            xmlString.Append("<WorkingTimes>");
            xmlString.Append("<WorkingTime>");
            xmlString.Append("<FromTime>08:00:00</FromTime>"); xmlString.Append("<ToTime>12:00:00</ToTime>");
            xmlString.Append("</WorkingTime>");
            xmlString.Append("<WorkingTime>");
            xmlString.Append("<FromTime>13:00:00</FromTime>"); xmlString.Append("<ToTime>17:00:00</ToTime>");
            xmlString.Append("</WorkingTime>");
            xmlString.Append("</WorkingTimes>");
            xmlString.Append("</WeekDay>");
            xmlString.Append("<WeekDay>");
            xmlString.Append("<DayType>3</DayType>");
            xmlString.Append("<DayWorking>1</DayWorking>");
            xmlString.Append("<WorkingTimes>");
            xmlString.Append("<WorkingTime>");
            xmlString.Append("<FromTime>08:00:00</FromTime>"); xmlString.Append("<ToTime>12:00:00</ToTime>");
            xmlString.Append("</WorkingTime>");
            xmlString.Append("<WorkingTime>");
            xmlString.Append("<FromTime>13:00:00</FromTime>"); xmlString.Append("<ToTime>17:00:00</ToTime>");
            xmlString.Append("</WorkingTime>");
            xmlString.Append("</WorkingTimes>");
            xmlString.Append("</WeekDay>");
            xmlString.Append("<WeekDay>");
            xmlString.Append("<DayType>4</DayType>");
            xmlString.Append("<DayWorking>1</DayWorking>");
            xmlString.Append("<WorkingTimes>");
            xmlString.Append("<WorkingTime>");
            xmlString.Append("<FromTime>08:00:00</FromTime>"); xmlString.Append("<ToTime>12:00:00</ToTime>");
            xmlString.Append("</WorkingTime>");
            xmlString.Append("<WorkingTime>");
            xmlString.Append("<FromTime>13:00:00</FromTime>"); xmlString.Append("<ToTime>17:00:00</ToTime>");
            xmlString.Append("</WorkingTime>");
            xmlString.Append("</WorkingTimes>");
            xmlString.Append("</WeekDay>");
            xmlString.Append("<WeekDay>");
            xmlString.Append("<DayType>5</DayType>");
            xmlString.Append("<DayWorking>1</DayWorking>");
            xmlString.Append("<WorkingTimes>");
            xmlString.Append("<WorkingTime>");
            xmlString.Append("<FromTime>08:00:00</FromTime>"); xmlString.Append("<ToTime>12:00:00</ToTime>");
            xmlString.Append("</WorkingTime>");
            xmlString.Append("<WorkingTime>");
            xmlString.Append("<FromTime>13:00:00</FromTime>"); xmlString.Append("<ToTime>17:00:00</ToTime>");
            xmlString.Append("</WorkingTime>");
            xmlString.Append("</WorkingTimes>");
            xmlString.Append("</WeekDay>");
            xmlString.Append("<WeekDay>");
            xmlString.Append("<DayType>6</DayType>");
            xmlString.Append("<DayWorking>1</DayWorking>");
            xmlString.Append("<WorkingTimes>");
            xmlString.Append("<WorkingTime>");
            xmlString.Append("<FromTime>08:00:00</FromTime>"); xmlString.Append("<ToTime>12:00:00</ToTime>");
            xmlString.Append("</WorkingTime>");
            xmlString.Append("<WorkingTime>");
            xmlString.Append("<FromTime>13:00:00</FromTime>"); xmlString.Append("<ToTime>17:00:00</ToTime>");
            xmlString.Append("</WorkingTime>");
            xmlString.Append("</WorkingTimes>");
            xmlString.Append("</WeekDay>");
            xmlString.Append("<WeekDay>");
            xmlString.Append("<DayType>7</DayType>");
            xmlString.Append("<DayWorking>0</DayWorking>");
            xmlString.Append("</WeekDay>");
            xmlString.Append("</WeekDays>");
            xmlString.Append("</Calendar>");

            //add Calender value for resources
            AddCalendar(ResCollection,xmlString);

            xmlString.Append("</Calendars>");
            xmlString.Append("<Tasks>");
            xmlString.Append("<Task>");
            xmlString.Append("<UID>0</UID>"); xmlString.Append("<ID>0</ID>"); xmlString.Append("<Type>0</Type>"); xmlString.Append("<IsNull>0</IsNull>"); xmlString.Append("<CreateDate>").Append(startDate).Append("</CreateDate>");
            xmlString.Append("<WBS>0</WBS>"); xmlString.Append("<OutlineNumber>0</OutlineNumber>"); xmlString.Append("<Priority>500</Priority>"); xmlString.Append("<Start>").Append(startDate).Append("</Start>");
            xmlString.Append("<Finish>").Append(EndDate).Append("</Finish>");
            xmlString.Append("<Duration>PT0H0M0S</Duration>");
            xmlString.Append("<DurationFormat>53</DurationFormat>");
            xmlString.Append("<Work>PT0H0M0S</Work>");
            xmlString.Append("<ResumeValid>0</ResumeValid>");
            xmlString.Append("<EffortDriven>0</EffortDriven>");
            xmlString.Append("<Recurring>0</Recurring>");
            xmlString.Append("<OverAllocated>0</OverAllocated>");
            xmlString.Append("<Estimated>0</Estimated>");
            xmlString.Append("<Milestone>0</Milestone>");
            xmlString.Append("<Summary>1</Summary>");
            xmlString.Append("<Critical>1</Critical>");
            xmlString.Append("<IsSubproject>0</IsSubproject>");
            xmlString.Append("<IsSubprojectReadOnly>0</IsSubprojectReadOnly>");
            xmlString.Append("<ExternalTask>0</ExternalTask>");
            xmlString.Append("<EarlyStart>2010-08-12T08:00:00</EarlyStart>");
            xmlString.Append("<EarlyFinish>2010-08-12T08:00:00</EarlyFinish>");
            xmlString.Append("<LateStart>2010-08-12T08:00:00</LateStart>");
            xmlString.Append("<LateFinish>2010-08-12T08:00:00</LateFinish>");
            xmlString.Append("<StartVariance>0</StartVariance>");
            xmlString.Append("<FinishVariance>0</FinishVariance>");
            xmlString.Append("<WorkVariance>0.00</WorkVariance>");
            xmlString.Append("<FreeSlack>0</FreeSlack>"); xmlString.Append("<TotalSlack>0</TotalSlack>"); xmlString.Append("<FixedCost>0</FixedCost>"); xmlString.Append("<FixedCostAccrual>3</FixedCostAccrual>");
            xmlString.Append("<PercentComplete>0</PercentComplete>"); xmlString.Append("<Cost>0</Cost>"); xmlString.Append("<OvertimeCost>0</OvertimeCost>"); xmlString.Append("<OvertimeWork>PT0H0M0S</OvertimeWork>");
            xmlString.Append("<ActualDuration>PT0H0M0S</ActualDuration>"); xmlString.Append("<ActualCost>0</ActualCost>"); xmlString.Append("<ActualWork>PT0H0M0S</ActualWork>"); xmlString.Append("<ActualOvertimeWork>PT0H0M0S</ActualOvertimeWork>");
            xmlString.Append("<RegularWork>PT0H0M0S</RegularWork>"); xmlString.Append("<RemainingDuration>PT0H0M0S</RemainingDuration>"); xmlString.Append("<RemainingCost>0</RemainingCost>"); xmlString.Append("<RemainingWork>PT0H0M0S</RemainingWork>");
            xmlString.Append("<RemainingOvertimeCost>0</RemainingOvertimeCost>"); xmlString.Append("<RemainingOvertimeWork>PT0H0M0S</RemainingOvertimeWork>"); xmlString.Append("<ACWP>0.00</ACWP>");
            xmlString.Append("<CV>0.00</CV>"); xmlString.Append("<ConstraintType>0</ConstraintType>"); xmlString.Append("<CalendarUID>-1</CalendarUID>"); xmlString.Append("<LevelAssignments>1</LevelAssignments>"); xmlString.Append("<LevelingCanSplit>1</LevelingCanSplit>");
            xmlString.Append("<LevelingDelay>0</LevelingDelay>"); xmlString.Append("<LevelingDelayFormat>8</LevelingDelayFormat>"); xmlString.Append("<IgnoreResourceCalendar>0</IgnoreResourceCalendar>"); xmlString.Append("<HideBar>0</HideBar>");
            xmlString.Append("<Rollup>0</Rollup>"); xmlString.Append("<BCWS>0.00</BCWS>"); xmlString.Append("<BCWP>0.00</BCWP>"); xmlString.Append("<PhysicalPercentComplete>0</PhysicalPercentComplete>"); xmlString.Append("<EarnedValueMethod>0</EarnedValueMethod>");
            xmlString.Append("<IsPublished>1</IsPublished>"); xmlString.Append("<CommitmentType>0</CommitmentType>");
            xmlString.Append("</Task>");

            // Iterating the Task details
            ConvertTaskToXML(taskCollection.Cast<IGanttTask>(), 1,xmlString);
            
            xmlString.Append("</Tasks>");
            xmlString.Append("<Resources>");
            xmlString.Append("<Resource>");
            xmlString.Append("<UID>0</UID>"); xmlString.Append("<ID>0</ID>"); xmlString.Append("<Type>1</Type>"); xmlString.Append("<IsNull>0</IsNull>"); xmlString.Append("<WorkGroup>0</WorkGroup>"); xmlString.Append("<MaxUnits>1.00</MaxUnits>"); xmlString.Append("<PeakUnits>0.00</PeakUnits>");
            xmlString.Append("<OverAllocated>0</OverAllocated>"); xmlString.Append("<CanLevel>1</CanLevel>"); xmlString.Append("<AccrueAt>3</AccrueAt>"); xmlString.Append("<Work>PT0H0M0S</Work>"); xmlString.Append("<RegularWork>PT0H0M0S</RegularWork>"); xmlString.Append("<OvertimeWork>PT0H0M0S</OvertimeWork>");
            xmlString.Append("<ActualWork>PT0H0M0S</ActualWork>"); xmlString.Append("<RemainingWork>PT0H0M0S</RemainingWork>"); xmlString.Append("<ActualOvertimeWork>PT0H0M0S</ActualOvertimeWork>"); xmlString.Append("<RemainingOvertimeWork>PT0H0M0S</RemainingOvertimeWork>");
            xmlString.Append("<PercentWorkComplete>0</PercentWorkComplete>"); xmlString.Append("<StandardRate>0</StandardRate>"); xmlString.Append("<StandardRateFormat>2</StandardRateFormat>"); xmlString.Append("<Cost>0</Cost>"); xmlString.Append("<OvertimeRate>0</OvertimeRate>");
            xmlString.Append("<OvertimeRateFormat>2</OvertimeRateFormat>"); xmlString.Append("<OvertimeCost>0</OvertimeCost>"); xmlString.Append("<CostPerUse>0</CostPerUse>"); xmlString.Append("<ActualCost>0</ActualCost>"); xmlString.Append("<ActualOvertimeCost>0</ActualOvertimeCost>");
            xmlString.Append("<RemainingCost>0</RemainingCost>"); xmlString.Append("<RemainingOvertimeCost>0</RemainingOvertimeCost>"); xmlString.Append("<WorkVariance>0.00</WorkVariance>"); xmlString.Append("<CostVariance>0</CostVariance>"); xmlString.Append("<SV>0.00</SV>");
            xmlString.Append("<CV>0.00</CV>"); xmlString.Append("<ACWP>0.00</ACWP>"); xmlString.Append("<CalendarUID>2</CalendarUID>"); xmlString.Append("<BCWS>0.00</BCWS>"); xmlString.Append("<BCWP>0.00</BCWP>"); xmlString.Append("<IsGeneric>0</IsGeneric>");
            xmlString.Append("<IsInactive>0</IsInactive>"); xmlString.Append("<IsEnterprise>0</IsEnterprise>"); xmlString.Append("<BookingType>0</BookingType>"); xmlString.Append("<CreationDate>2010-08-21T00:14:00</CreationDate>"); xmlString.Append("<IsCostResource>0</IsCostResource>");
            xmlString.Append("<IsBudget>0</IsBudget>");
            xmlString.Append("</Resource>");

            //Add Resources Value in XML
            AddResources(ResCollection,xmlString);
            
            xmlString.Append("</Resources>");
            //Add Assignment to XML
            xmlString.Append("<Assignments>");
            AddAssignmentToResources(ResCollection, taskCollection.Cast<IGanttTask>(),xmlString);
            xmlString.Append("</Assignments>");
            xmlString.Append("</Project>");

            return xmlString;
        }

        /// <summary>
        /// Converts the task to XML.
        /// </summary>
        /// <param name="taskDetailCol">The task detail col.</param>
        /// <param name="childlevel">The childlevel.</param>
        /// <param name="xmlString">The XML string.</param>
        public static void ConvertTaskToXML(IEnumerable<IGanttTask> taskDetailCol, int childlevel, StringBuilder xmlString)
        {
            foreach (IGanttTask taskDetail in taskDetailCol)
            {
                string startDate = String.Format("{0:yyyy-MM-ddTHH:mm:ss}", taskDetail.StartDate);
                string endDate = String.Format("{0:yyyy-MM-ddTHH:mm:ss}", taskDetail.FinishDate);
                string dur = string.Format("PT{0:0}H{1:0}M{2:0}S", taskDetail.Duration.Days * 8, taskDetail.Duration.Minutes, taskDetail.Duration.Seconds);
                xmlString.Append("<Task>");
                xmlString.Append("<Type>0</Type>");
                xmlString.Append("<Name>").Append(taskDetail.TaskName).Append("</Name>");
                xmlString.Append("<Start>").Append(startDate).Append("</Start>");
                xmlString.Append("<Finish>").Append(endDate).Append("</Finish>");
                xmlString.Append("<ManualStart>").Append(startDate).Append("</ManualStart>");
                xmlString.Append("<ManualFinish>").Append(endDate).Append("</ManualFinish>");
                xmlString.Append("<ManualDuration>").Append(dur).Append("</ManualDuration>");
                xmlString.Append("<DurationFormat>53</DurationFormat>");
                xmlString.Append("<MileStone>").Append(taskDetail.IsMileStone.ToString()).Append("</MileStone>");
                xmlString.Append("<Work>PT0H0M0S</Work>");
                xmlString.Append("<Duration>").Append(dur).Append("</Duration>");
                xmlString.Append("<UID>").Append(taskDetail.TaskId.ToString()).Append("</UID>");
                xmlString.Append("<ConstraintDate>").Append(startDate).Append("</ConstraintDate>");
                xmlString.Append("<ConstraintType>4</ConstraintType>");
                xmlString.Append("<CreateDate>").Append(startDate).Append("</CreateDate>");
                xmlString.Append("<IsNull>0</IsNull>");
                xmlString.Append("<OutlineLevel>").Append(childlevel.ToString()).Append("</OutlineLevel>");
                xmlString.Append("<PercentComplete>").Append(taskDetail.Progress).Append("</PercentComplete>");
                if (taskDetail.Predecessor.Count > 0)
                {
                    foreach (Predecessor link in taskDetail.Predecessor)
                    {
                        xmlString.Append("<PredecessorLink>");
                        xmlString.Append("<PredecessorUID>").Append(link.GanttTaskIndex).Append("</PredecessorUID>");
                        xmlString.Append("<Type>").Append(link.GanttTaskRelationship.GetHashCode()).Append("</Type>");
                        xmlString.Append("</PredecessorLink>");
                    }
                }
                xmlString.Append("</Task>");

                // Iterating the child 
                if (taskDetail.Child.Count > 0)
                {
                    ConvertTaskToXML(taskDetail.Child, ++childlevel,xmlString);
                    --childlevel;
                }
            }
        }

        #region XMLImport

        /// <summary>
        /// Converts to task details.
        /// </summary>
        /// <returns></returns>
        internal static TaskDetailsCollection ConvertToTaskDetails()
        {
            TaskDetailsCollection ganttTasks = new TaskDetailsCollection();
            OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "XML Files (*.XML)|*.xml" };

            if (openFileDialog.ShowDialog() == true)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Project));

                // Reading the xml to deserialize it
                Stream stream = openFileDialog.File.OpenRead();
                Project deserializedProject = (Project)xmlSerializer.Deserialize(stream);

                List<ProjectTask> projectTasks = deserializedProject.Tasks;
                List<ProjectAssignment> ass = deserializedProject.Assignments;
                List<ProjectResourcesResource> res = deserializedProject.Resources;
                Dictionary<int, ProjectResourcesResource> AssignedResources = GenerateResourceAssignmentPair(ass, res);

                // Iterating through the project Tasks to create Gantt Tasks
                foreach (ProjectTask projectTask in projectTasks)
                {
                    if (projectTask == null)
                        continue;

                    if (projectTask.UID > 0)
                    {
                        TaskDetails ganttTask = new TaskDetails();

                        DateTime startDate;
                        DateTime endDate;
                        DateTime.TryParseExact(projectTask.Start, @"yyyy-MM-dd\THH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out startDate);
                        DateTime.TryParseExact(projectTask.Finish, @"yyyy-MM-dd\THH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out endDate);

                        ganttTask.TaskId = projectTask.UID;
                        if (projectTask.Name != null)
                            ganttTask.TaskName = projectTask.Name;

                        ganttTask.StartDate = startDate;
                        ganttTask.FinishDate = endDate;
                        ganttTask.Progress = projectTask.PercentComplete;
                        ganttTask.IsMileStone = Convert.ToBoolean(projectTask.IsMileStone);

                        foreach (KeyValuePair<int, ProjectResourcesResource> resource in AssignedResources)
                        {
                            if (resource.Key == ganttTask.TaskId)
                            {
                                ganttTask.Resources.Add(new Resource { ID = resource.Value.ID, Name = resource.Value.Name });
                            }
                        }

                        if (projectTask.PredecessorLink.Count > 0)
                        {
                            for (int i = 0; i < projectTask.PredecessorLink.Count; i++)
                            {
                                Predecessor predecessor = new Predecessor();
                                predecessor.GanttTaskIndex = projectTask.PredecessorLink[i].PredecessorUID;
                                predecessor.GanttTaskRelationship = (GanttTaskRelationship)projectTask.PredecessorLink[i].Type;
                                ganttTask.Predecessor.Add(predecessor);
                            }
                        }

                        if (projectTask.OutlineLevel == 1)
                        {
                            ganttTasks.Add(ganttTask);
                        }
                        else
                        {
                            // Get the corresponding parent to add the child.
                            ObservableCollection<IGanttTask> currentCollection = GetParentCollection(projectTask.OutlineLevel, new ObservableCollection<IGanttTask>(ganttTasks.Cast<IGanttTask>()));
                            currentCollection.Add(ganttTask);
                        }
                    }
                }
            }
            return ganttTasks;
        }

        /// <summary>
        /// Gets the parent collection.
        /// </summary>
        /// <param name="outLineLevel">The out line level.</param>
        /// <param name="currentCollection">The current collection.</param>
        /// <returns></returns>
        internal static ObservableCollection<IGanttTask> GetParentCollection(int outLineLevel, ObservableCollection<IGanttTask> currentCollection)
        {
            int index = currentCollection.Count >0 ? currentCollection.Count -1 : currentCollection.Count;
            return outLineLevel == 1 ? currentCollection : GetParentCollection(outLineLevel - 1, currentCollection[index].Child);
        }

        /// <summary>
        /// Generates the resource assignment pair.
        /// </summary>
        /// <param name="projAssignments">The proj assignments.</param>
        /// <param name="projResources">The proj resources.</param>
        /// <returns></returns>
        internal static Dictionary<int, ProjectResourcesResource> GenerateResourceAssignmentPair(List<ProjectAssignment> projAssignments, List<ProjectResourcesResource> projResources)
        {
            Dictionary<int, ProjectResourcesResource> resCol = new Dictionary<int, ProjectResourcesResource>();

            foreach (ProjectAssignment pass in projAssignments)
            {
                foreach (ProjectResourcesResource pres in projResources)
                {
                    if (pass.ResourceUID == pres.UID && !resCol.ContainsKey(pass.TaskUID))
                    {
                        resCol.Add(pass.TaskUID,pres);
                    }
                }
            }           
            return resCol;
        }
        
        #endregion
    }

    [XmlRoot("Project", Namespace = "http://schemas.microsoft.com/project")]
    public partial class Project
    {
        private List<ProjectTask> tasksField;
        private List<ProjectResourcesResource> resourcesField;
        private List<ProjectAssignment> assignmentsField;

        /// <summary>
        /// Gets or sets the tasks.
        /// </summary>
        /// <value>The tasks.</value>
        [XmlArray("Tasks")]
        [XmlArrayItem("Task")]
        public List<ProjectTask> Tasks
        {
            get
            {
                return this.tasksField;
            }
            set
            {
                this.tasksField = value;
            }
        }

        /// <summary>
        /// Gets or sets the resources.
        /// </summary>
        /// <value>The resources.</value>
        [XmlArray("Resources")]
        [XmlArrayItem("Resource")]
        public List<ProjectResourcesResource> Resources
        {
            get
            {
                return this.resourcesField;
            }
            set
            {
                this.resourcesField = value;
            }
        }

        /// <summary>
        /// Gets or sets the assignments.
        /// </summary>
        /// <value>The assignments.</value>
        [XmlArray("Assignments")]
        [XmlArrayItem("Assignment")]
        public List<ProjectAssignment> Assignments
        {
            get
            {
                return this.assignmentsField;
            }
            set
            {
                this.assignmentsField = value;
            }
        }
    }

    /// <summary>
    /// Represent the class that holds the infromation about the project calendar
    /// </summary>
    public partial class ProjectCalendars
    {
        private ProjectCalendarsCalendar calendarField;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectCalendars"/> class.
        /// </summary>
        public ProjectCalendars()
        {
            this.calendarField = new ProjectCalendarsCalendar();
        }

        /// <summary>
        /// Gets or sets the calendar.
        /// </summary>
        /// <value>The calendar.</value>
        public ProjectCalendarsCalendar Calendar
        {
            get
            {
                return this.calendarField;
            }
            set
            {
                this.calendarField = value;
            }
        }
    }

    /// <summary>
    /// Represent the class that holds the information about the complete project calendar
    /// </summary>
    public partial class ProjectCalendarsCalendar
    {
        private byte uIDField;
        private string nameField;
        private byte isBaseCalendarField;
        private byte isBaselineCalendarField;
        private sbyte baseCalendarUIDField;
        private List<ProjectCalendarsCalendarWeekDay> weekDaysField;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectCalendarsCalendar"/> class.
        /// </summary>
        public ProjectCalendarsCalendar()
        {
            this.weekDaysField = new List<ProjectCalendarsCalendarWeekDay>();
        }

        /// <summary>
        /// Gets or sets the UID.
        /// </summary>
        /// <value>The UID.</value>
        public byte UID
        {
            get
            {
                return this.uIDField;
            }
            set
            {
                this.uIDField = value;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <summary>
        /// Gets or sets the is base calendar.
        /// </summary>
        /// <value>The is base calendar.</value>
        public byte IsBaseCalendar
        {
            get
            {
                return this.isBaseCalendarField;
            }
            set
            {
                this.isBaseCalendarField = value;
            }
        }

        /// <summary>
        /// Gets or sets the is baseline calendar.
        /// </summary>
        /// <value>The is baseline calendar.</value>
        public byte IsBaselineCalendar
        {
            get
            {
                return this.isBaselineCalendarField;
            }
            set
            {
                this.isBaselineCalendarField = value;
            }
        }

        /// <summary>
        /// Gets or sets the base calendar UID.
        /// </summary>
        /// <value>The base calendar UID.</value>
        public sbyte BaseCalendarUID
        {
            get
            {
                return this.baseCalendarUIDField;
            }
            set
            {
                this.baseCalendarUIDField = value;
            }
        }
    }

    /// <summary>
    /// Represents the class that holds the informatio about week days of the project calendar
    /// </summary>
    public partial class ProjectCalendarsCalendarWeekDay
    {
        private byte dayTypeField;
        private byte dayWorkingField;
        private List<ProjectCalendarsCalendarWeekDayWorkingTime> workingTimesField;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectCalendarsCalendarWeekDay"/> class.
        /// </summary>
        public ProjectCalendarsCalendarWeekDay()
        {
            this.workingTimesField = new List<ProjectCalendarsCalendarWeekDayWorkingTime>();
        }

        /// <summary>
        /// Gets or sets the type of the day.
        /// </summary>
        /// <value>The type of the day.</value>
        public byte DayType
        {
            get
            {
                return this.dayTypeField;
            }
            set
            {
                this.dayTypeField = value;
            }
        }

        /// <summary>
        /// Gets or sets the day working.
        /// </summary>
        /// <value>The day working.</value>
        public byte DayWorking
        {
            get
            {
                return this.dayWorkingField;
            }
            set
            {
                this.dayWorkingField = value;
            }
        }

        /// <summary>
        /// Gets or sets the working times.
        /// </summary>
        /// <value>The working times.</value>
        [System.Xml.Serialization.XmlArrayAttribute(Order = 2)]
        [System.Xml.Serialization.XmlArrayItemAttribute("WorkingTime", IsNullable = false)]
        public List<ProjectCalendarsCalendarWeekDayWorkingTime> WorkingTimes
        {
            get
            {
                return this.workingTimesField;
            }
            set
            {
                this.workingTimesField = value;
            }
        }
    }

    /// <summary>
    /// Represents the class that holds the informatio about week days working date of the project calendar
    /// </summary>
    public partial class ProjectCalendarsCalendarWeekDayWorkingTime
    {
        private System.DateTime fromTimeField;
        private System.DateTime toTimeField;

        /// <summary>
        /// Gets or sets from time.
        /// </summary>
        /// <value>From time.</value>
        public System.DateTime FromTime
        {
            get
            {
                return this.fromTimeField;
            }
            set
            {
                this.fromTimeField = value;
            }
        }

        /// <summary>
        /// Gets or sets to time.
        /// </summary>
        /// <value>To time.</value>
        public System.DateTime ToTime
        {
            get
            {
                return this.toTimeField;
            }
            set
            {
                this.toTimeField = value;
            }
        }
    }

    /// <summary>
    /// Represents the class that holds the informatio about dependency link of the project
    /// </summary>
    [XmlRoot("PredecessorLink")]
    public partial class ProjectPredecessorLink
    {
        private byte PIDField;

        private byte typeField;
        /// <summary>
        /// Gets or sets the predecessor UID.
        /// </summary>
        /// <value>The predecessor UID.</value>
        [XmlElement("PredecessorUID")]
        public byte PredecessorUID
        {
            get
            {
                return this.PIDField;
            }
            set
            {
                this.PIDField = value;
            }
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [XmlElement("Type")]
        public byte Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <summary>
    /// Represents the class that holds the informatio about project task
    /// </summary>
    [XmlRoot("Task")]
    public partial class ProjectTask
    {
        private byte uIDField;
        private string NameField;
        private byte outlineLevelField;
        private string startField;
        private string finishField;
        private string durationField;
        private string mileStoneField;
        private double percentCompleteField;
        private List<ProjectPredecessorLink> ProcesserField;

        //[XmlArray("PredecessorLink")]
        //[XmlArrayItem("PredecessorLink")]
        /// <summary>
        /// Gets or sets the predecessor link.
        /// </summary>
        /// <value>The predecessor link.</value>
        [XmlElement("PredecessorLink")]
        public List<ProjectPredecessorLink> PredecessorLink
        {
            get
            {
                return this.ProcesserField;
            }
            set
            {
                this.ProcesserField = value;
            }
        }

        /// <summary>
        /// Gets or sets the UID.
        /// </summary>
        /// <value>The UID.</value>
        [XmlElement("UID")]
        public byte UID
        {
            get
            {
                return this.uIDField;
            }
            set
            {
                this.uIDField = value;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [XmlElement("Name")]
        public string Name
        {
            get
            {
                return this.NameField;
            }
            set
            {
                this.NameField = value;
            }
        }

        /// <summary>
        /// Gets or sets the outline level.
        /// </summary>
        /// <value>The outline level.</value>
        [XmlElement("OutlineLevel")]
        public byte OutlineLevel
        {
            get
            {
                return this.outlineLevelField;
            }
            set
            {
                this.outlineLevelField = value;
            }
        }

        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        /// <value>The start.</value>
        [XmlElement("Start")]
        public string Start
        {
            get
            {
                return this.startField;
            }
            set
            {
                this.startField = value;
            }
        }

        /// <summary>
        /// Gets or sets the finish.
        /// </summary>
        /// <value>The finish.</value>
        [XmlElement("Finish")]
        public string Finish
        {
            get
            {
                return this.finishField;
            }
            set
            {
                this.finishField = value;
            }
        }

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>The duration.</value>
        [XmlElement("Duration")]
        public string Duration
        {
            get
            {
                return this.durationField;
            }
            set
            {
                this.durationField = value;
            }
        }

        [XmlElement("MileStone")]
        public string IsMileStone
        {
            get
            {
                return mileStoneField;
            }
            set
            {
                mileStoneField = value;
            }
        }

        /// <summary>
        /// Gets or sets the percent complete.
        /// </summary>
        /// <value>The percent complete.</value>
        [XmlElement("PercentComplete")]
        public double PercentComplete
        {
            get
            {
                return this.percentCompleteField;
            }
            set
            {
                this.percentCompleteField = value;
            }
        }
    }

    /// <summary>
    /// Represents the class that holds the informatio about individual resources of the project
    /// </summary>
    public partial class ProjectResources
    {
        private ProjectResourcesResource resourceField;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectResources"/> class.
        /// </summary>
        public ProjectResources()
        {
            this.resourceField = new ProjectResourcesResource();
        }

        /// <summary>
        /// Gets or sets the resource.
        /// </summary>
        /// <value>The resource.</value>
        public ProjectResourcesResource Resource
        {
            get
            {
                return this.resourceField;
            }
            set
            {
                this.resourceField = value;
            }
        }
    }

    /// <summary>
    /// Represents the class that holds the informatio about Resource collection in project
    /// </summary>
    [XmlRoot("Resource")]
    public partial class ProjectResourcesResource
    {
        private byte uIDField;
        private byte idField;
        private string NameField;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [XmlElement("Name")]
        public string Name
        {
            get
            {
                return this.NameField;
            }
            set
            {
                this.NameField = value;
            }
        }

        /// <summary>
        /// Gets or sets the UID.
        /// </summary>
        /// <value>The UID.</value>
        [XmlElement("UID")]
        public byte UID
        {
            get
            {
                return this.uIDField;
            }
            set
            {
                this.uIDField = value;
            }
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [XmlElement("ID")]
        public byte ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <summary>
    /// Represents the class that holds the informatio about project assignement
    /// </summary>
     [XmlRoot("Assignment")]
    public partial class ProjectAssignment
    {
        private byte uIDField;
        private byte taskUIDField;
        private int resourceUIDField;     
        private List<ProjectAssignmentTimephasedData> timephasedDataField;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectAssignment"/> class.
        /// </summary>
        public ProjectAssignment()
        {
            this.timephasedDataField = new List<ProjectAssignmentTimephasedData>();
        }

        /// <summary>
        /// Gets or sets the UID.
        /// </summary>
        /// <value>The UID.</value>
        [XmlElement("UID")]
        public byte UID
        {
            get
            {
                return this.uIDField;
            }
            set
            {
                this.uIDField = value;
            }
        }

        /// <summary>
        /// Gets or sets the task UID.
        /// </summary>
        /// <value>The task UID.</value>
        [XmlElement("TaskUID")]
        public byte TaskUID
        {
            get
            {
                return this.taskUIDField;
            }
            set
            {
                this.taskUIDField = value;
            }
        }

        /// <summary>
        /// Gets or sets the resource UID.
        /// </summary>
        /// <value>The resource UID.</value>
        [XmlElement("ResourceUID")]
        public int ResourceUID
        {
            get
            {
                return this.resourceUIDField;
            }
            set
            {
                this.resourceUIDField = value;
            }
        }

        /// <summary>
        /// Gets or sets the timephased data.
        /// </summary>
        /// <value>The timephased data.</value>
        public List<ProjectAssignmentTimephasedData> TimephasedData
        {
            get
            {
                return this.timephasedDataField;
            }
            set
            {
                this.timephasedDataField = value;
            }
        }
    }

     /// <summary>
     /// Represents the class that holds the informatio about project assignment with assigned date
     /// </summary>
    public partial class ProjectAssignmentTimephasedData
    {
        private byte typeField;
        private byte uIDField;
        private System.DateTime startField;
        private System.DateTime finishField;
        private byte unitField;
        private string valueField;

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public byte Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <summary>
        /// Gets or sets the UID.
        /// </summary>
        /// <value>The UID.</value>
        public byte UID
        {
            get
            {
                return this.uIDField;
            }
            set
            {
                this.uIDField = value;
            }
        }

        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        /// <value>The start.</value>
        public System.DateTime Start
        {
            get
            {
                return this.startField;
            }
            set
            {
                this.startField = value;
            }
        }

        /// <summary>
        /// Gets or sets the finish.
        /// </summary>
        /// <value>The finish.</value>
        public System.DateTime Finish
        {
            get
            {
                return this.finishField;
            }
            set
            {
                this.finishField = value;
            }
        }

        /// <summary>
        /// Gets or sets the unit.
        /// </summary>
        /// <value>The unit.</value>
        public byte Unit
        {
            get
            {
                return this.unitField;
            }
            set
            {
                this.unitField = value;
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }
}
