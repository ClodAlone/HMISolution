//-------------------------------------------------------------------------------------------------
// <copyright file="ScheduleAppearance.cs" company="syncfusion">
// Copyright (c) syncfusion.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Globalization;

namespace Syncfusion.Windows.Forms.Schedule
{
	/// <summary>
	/// A ScheduleAppearance class that specifies the colors and fonts used in a ScheduleControl
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[Serializable]
	public class ScheduleAppearance : ISerializable
	{
        /// <summary>
        /// Default Constructor. Only present to support XML serialization.
        /// </summary>
        public ScheduleAppearance()
        {
        }

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="schedule">The ScheduleControl associated with this appearance object.</param>
		public ScheduleAppearance(ScheduleControl schedule)
		{
			this.schedule = schedule;
		}

		#region Serialization Support
		/// <summary>
		/// Initializes a new <see cref="ScheduleAppearance"/> from a serialization stream.
		/// </summary>
		/// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
		/// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
		protected ScheduleAppearance(SerializationInfo info, StreamingContext context)
		{
			this.version = (int) info.GetValue("version", typeof(int));
			this.primeTimeCellColor = (Color) info.GetValue("primeTimeCellColor", typeof(Color));
			this.workWeekHeaderBackColor = (Color) info.GetValue("workWeekHeaderBackColor", typeof(Color));
			this.workWeekHeaderForeColor = (Color) info.GetValue("workWeekHeaderForeColor", typeof(Color));
			this.monthWeekHeaderBackColor = (Color) info.GetValue("monthWeekHeaderBackColor", typeof(Color));
			this.monthWeekHeaderForeColor = (Color) info.GetValue("monthWeekHeaderForeColor", typeof(Color));
			this.clickItemBorderColor = (Color) info.GetValue("clickItemBorderColor", typeof(Color));
			this.timeBackColor = (Color) info.GetValue("timeBackColor", typeof(Color));
			this.timeBigFontSize = (float) info.GetValue("timeBigFontSize", typeof(float));
			this.timeLittleFontSize = (float) info.GetValue("timeLittleFontSize", typeof(float));
			this.timeTextColor = (Color) info.GetValue("timeTextColor", typeof(Color));
			this.textColor = (Color) info.GetValue("textColor", typeof(Color));
			this.captionBackColor = (Color) info.GetValue("captionBackColor", typeof(Color));
			this.solidBorderColor = (Color) info.GetValue("solidBorderColor", typeof(Color));
			this.markColumnColor = (Color) info.GetValue("markColumnColor", typeof(Color));
			this.themesEnabled = (bool) info.GetValue("themesEnabled", typeof(bool));
			this.showCaption = (bool) info.GetValue("showCaption", typeof(bool));
			this.showCaptionButtons = (bool) info.GetValue("showCaptionButtons", typeof(bool));
			this.primeTimeStart = (int) info.GetValue("primeTimeStart", typeof(int));
			this.primeTimeEnd = (int) info.GetValue("primeTimeEnd", typeof(int));
			this.ScheduleAppointmentTipFormat = (string) info.GetValue("ScheduleAppointmentTipFormat", typeof(string));
			this.dayItemFormat = (string) info.GetValue("dayItemFormat", typeof(string));
			this.weekMonthItemFormat = (string) info.GetValue("weekMonthItemFormat", typeof(string));
			this.allDayItemFormat = (string) info.GetValue("allDayItemFormat", typeof(string));
			this.dateTimeFormat = (string) info.GetValue("dateTimeFormat", typeof(string));
			this.dateFormat = (string) info.GetValue("dateFormat", typeof(string));
			this.timeFormat = (string) info.GetValue("timeFormat", typeof(string));
			this.ScheduleAppointmentTipsEnabled = (bool) info.GetValue("ScheduleAppointmentTipsEnabled", typeof(bool));
			this.hours24 = (bool) info.GetValue("hours24", typeof(bool));
			this.dayMonthCutoff = (int) info.GetValue("dayMonthCutoff", typeof(int));
			this.showTime = (bool) info.GetValue("showTime", typeof(bool));
			this.navigationCalendarSelectionColor = (Color) info.GetValue("navigationCalendarSelectionColor", typeof(Color));
			this.navigationCalendarBackColor = (Color) info.GetValue("navigationCalendarBackColor", typeof(Color));
			this.navigationCalendarWeekNumberColor = (Color) info.GetValue("navigationCalendarWeekNumberColor", typeof(Color));
			this.navigationCalendarArrowColor = (Color) info.GetValue("navigationCalendarArrowColor", typeof(Color));
			this.navigationCalendarTodayColor = (Color) info.GetValue("navigationCalendarTodayColor", typeof(Color));
			this.navigationCalendarDisabledTextColor = (Color) info.GetValue("navigationCalendarDisabledTextColor", typeof(Color));
			this.navigationCalendarTextColor = (Color) info.GetValue("navigationCalendarTextColor", typeof(Color));
            this.navigationCalendarHeaderColor = (Color)info.GetValue("navigationCalendarHeaderColor", typeof(Color));                  
			this.splitterBackColor = (Color) info.GetValue("splitterBackColor", typeof(Color));
			this.dragColor = (Color) info.GetValue("dragColor", typeof(Color));

            if (version >= 1)
            {
                this.divisionsPerHour = (int)info.GetValue("divisionsPerHour", typeof(int));
                this.monthShowFullWeek = (bool)info.GetValue("monthShowFullWeek", typeof(bool));
                this.fullWeekHeaderFormat = (string)info.GetValue("fullWeekHeaderFormat", typeof(string));
                this.workWeekHeaderFormat = (string)info.GetValue("workWeekHeaderFormat", typeof(string));
                this.visualStyle = (GridVisualStyles)info.GetValue("VisualStyle", typeof(GridVisualStyles));
			
                ////used for new ScheduleAppearance schemas
            }

            if (version >= 2)
            {
                this.weekHeaderFormat = (string)info.GetValue("weekHeaderFormat", typeof(string));
            }

            if (version >= 3)
            {
                this.navigationCalendarStartDayOfWeek = (DayOfWeek)info.GetValue("navigationCalendarStartDayOfWeek", typeof(DayOfWeek));
                this.monthCalendarStartDayOfWeek = (DayOfWeek)info.GetValue("monthCalendarStartDayOfWeek", typeof(DayOfWeek));
                this.weekCalendarStartDayOfWeek = (DayOfWeek)info.GetValue("weekCalendarStartDayOfWeek", typeof(DayOfWeek));
            }

            if (version >= 4)
            {
                this.LongHeaderFormat = (string)info.GetValue("longHeaderFormat", typeof(string));
            }

            if (version >= 5)
            {
                this.spanItemFormatLeftText = (string)info.GetValue("spanItemFormatLeftText", typeof(string));
                this.spanItemFormatRightText = (string)info.GetValue("spanItemFormatRightText", typeof(string));
                this.spanItemFormatMiddleText = (string)info.GetValue("spanItemFormatMiddleText", typeof(string));
                this.spanItemFormatTerminalLeftText = (string)info.GetValue("spanItemFormatTerminalLeftText", typeof(string));
                this.spanItemFormatTerminalRightText = (string)info.GetValue("spanItemFormatTerminalRightText", typeof(string));       
            }

            if (version >= 6)
            {
                this.divisionsPerRow = (int)info.GetValue("divisionsPerRow", typeof(int));
            }
            if (version >= 7)
            {
                this.monthHeaderFormat = (string)info.GetValue("monthHeaderFormat", typeof(string));
                this.weekMonthNewMonth = (string)info.GetValue("weekMonthNewMonth", typeof(string));
                this.weekMonthFullFormat = (string)info.GetValue("weekMonthFullFormat", typeof(string));
            }

            if (version >= 8)
            {
                this.navigationCalendarTodayBackColor = (Color)info.GetValue("navigationCalendarTodayBackColor", typeof(Color));
                this.navigationCalendarTodayTextColor = (Color)info.GetValue("navigationCalendarTodayTextColor", typeof(Color));
                this.moreItemArrowColor = (Color)info.GetValue("moreItemArrowColor", typeof(Color));
                this.moreItemArrowHoverColor = (Color)info.GetValue("moreItemArrowHoverColor", typeof(Color));
                this.moreItemArrowBorderColor = (Color)info.GetValue("moreItemArrowBorderColor", typeof(Color));
            }      
            ////addproperty  - need to add code here when you add a property to ScheduleAppearance
		}

        ////private int version = 0;
        // //version = 0  - orignal version release as 4.4.0.54
        ////private int version = 1; 
        ////version = 1 changes 
        ////  - add support for divisionsPerHour.
        ////  - add support for monthShowFullWeek.
        ////private int version = 2;
       // //   - add support for weekHeaderFormat

        ////private int version = 3; //for 2008Vol4
        ////   - add support for navigationCalendarStartDayOfWeek
        ////   - add support for monthCalendarStartDayOfWeek
        ////   - add support for weekCalendarStartDayOfWeek

        ////private int version = 4; //for 2009Vol1
        ////   - add support for longHeaderFormat
        //// private int version = 5; ////for 2009Vol1
        ////   - add support for span item Formats

        //private int version = 6;  
        ////   - divisionsPerRow

        //private int version = 7;  //// for 2009 vol4

        private int version = 8;  //// for 2013 vol3
        ////   - add support for span item Formats
       
		/// <summary>
		/// Attaches an Appearance object to a particular ScheduleControl.
		/// </summary>
		/// <param name="schedule">The Appearance object.</param>
		public void AttachSchedule(ScheduleControl schedule)
		{
			this.schedule = schedule;
            if (schedule.GetScheduleHost().wired)
            {
                schedule.GetScheduleHost().UnwireAppearanceEvents(schedule.Appearance);
            }

			this.schedule.Appearance = this;
            this.schedule.GetScheduleHost().SetDataToDayPanels();
			schedule.GetScheduleHost().WireAppearanceEvents(this.schedule.Appearance);

			this.schedule.Calendar.appearance = this;

            if (PrimeTimeCellColorChanged != null)
            {
                PrimeTimeCellColorChanged(this, EventArgs.Empty);
            }

            if (WorkWeekHeaderBackColorChanged != null)
            {
                WorkWeekHeaderBackColorChanged(this, EventArgs.Empty);
            }

			if (WorkWeekHeaderForeColorChanged != null) 
            {
                WorkWeekHeaderForeColorChanged(this, EventArgs.Empty);
            }

            if (MonthWeekHeaderBackColorChanged != null)
            {
                MonthWeekHeaderBackColorChanged(this, EventArgs.Empty);
            }

            if (MonthWeekHeaderForeColorChanged != null)
            {
                MonthWeekHeaderForeColorChanged(this, EventArgs.Empty);
            }

            if (ClickItemBorderColorChanged != null)
            {
                ClickItemBorderColorChanged(this, EventArgs.Empty);
            }

            if (TimeBackColorChanged != null)
            {
                TimeBackColorChanged(this, EventArgs.Empty);
            }

            if (TimeBigFontSizeChanged != null)
            {
                TimeBigFontSizeChanged(this, EventArgs.Empty);
            }

            if (TimeLittleFontSizeChanged != null)
            {
                TimeLittleFontSizeChanged(this, EventArgs.Empty);
            }

            if (TimeTextColorChanged != null)
            {
                TimeTextColorChanged(this, EventArgs.Empty);
            }

            if (TextColorChanged != null)
            {
                TextColorChanged(this, EventArgs.Empty);
            }

            if (CaptionBackColorChanged != null)
            {
                CaptionBackColorChanged(this, EventArgs.Empty);
            }

            if (SolidBorderColorChanged != null)
            {
                SolidBorderColorChanged(this, EventArgs.Empty);
            }

            if (MarkColumnColorChanged != null)
            {
                MarkColumnColorChanged(this, EventArgs.Empty);
            }

            if (ThemesEnabledChanged != null)
            {
                ThemesEnabledChanged(this, EventArgs.Empty);
            }

            if (ShowCaptionChanged != null)
            {
                ShowCaptionChanged(this, EventArgs.Empty);
            }

            if (ShowCaptionButtonsChanged != null)
            {
                ShowCaptionButtonsChanged(this, EventArgs.Empty);
            }

            if (PrimeTimeStartChanged != null)
            {
                PrimeTimeStartChanged(this, EventArgs.Empty);
            }

            if (PrimeTimeEndChanged != null)
            {
                PrimeTimeEndChanged(this, EventArgs.Empty);
            }

            if (ScheduleAppointmentTipFormatChanged != null)
            {
                ScheduleAppointmentTipFormatChanged(this, EventArgs.Empty);
            }

            if (DayItemFormatChanged != null)
            {
                DayItemFormatChanged(this, EventArgs.Empty);
            }

            if (WeekMonthItemFormatChanged != null)
            {
                WeekMonthItemFormatChanged(this, EventArgs.Empty);
            }

            if (AllDayItemFormatChanged != null)
            {
                AllDayItemFormatChanged(this, EventArgs.Empty);
            }

            if (DateTimeFormatChanged != null)
            {
                DateTimeFormatChanged(this, EventArgs.Empty);
            }

            if (DateFormatChanged != null)
            {
                DateFormatChanged(this, EventArgs.Empty);
            }

            if (TimeFormatChanged != null)
            {
                TimeFormatChanged(this, EventArgs.Empty);
            }

            if (ScheduleAppointmentTipsEnabledChanged != null)
            {
                ScheduleAppointmentTipsEnabledChanged(this, EventArgs.Empty);
            }

            if (Hours24Changed != null)
            {
                Hours24Changed(this, EventArgs.Empty);
            }

            if (DayMonthCutoffChanged != null)
            {
                DayMonthCutoffChanged(this, EventArgs.Empty);
            }

            if (ShowTimeChanged != null)
            {
                ShowTimeChanged(this, EventArgs.Empty);
            }

            if (NavigationCalendarSelectionColorChanged != null)
            {
                NavigationCalendarSelectionColorChanged(this, EventArgs.Empty);
            }

            if (NavigationCalendarBackColorChanged != null)
            {
                NavigationCalendarBackColorChanged(this, EventArgs.Empty);
            }

            if (NavigationCalendarWeekNumberColorChanged != null)
            {
                NavigationCalendarWeekNumberColorChanged(this, EventArgs.Empty);
            }

            if (NavigationCalendarArrowColorChanged != null)
            {
                NavigationCalendarArrowColorChanged(this, EventArgs.Empty);
            }

            if (NavigationCalendarTodayColorChanged != null)
            {
                NavigationCalendarTodayColorChanged(this, EventArgs.Empty);
            }

            if (NavigationCalendarDisabledTextColorChanged != null)
            {
                NavigationCalendarDisabledTextColorChanged(this, EventArgs.Empty);
            }

            if (NavigationCalendarTextColorChanged != null)
            {
                NavigationCalendarTextColorChanged(this, EventArgs.Empty);
            }

            if (NavigationCalendarHeaderColorChanged != null)
            {
                NavigationCalendarHeaderColorChanged(this, EventArgs.Empty);
            }

            if (SplitterBackColorChanged != null)
            {
                SplitterBackColorChanged(this, EventArgs.Empty);
            }

            if (DragColorChanged != null)
            {
                DragColorChanged(this, EventArgs.Empty);
            }

            if (DivisionsPerHourChanged != null)
            {
                DivisionsPerHourChanged(this, EventArgs.Empty);
            }

            if (DivisionsPerRowChanged != null)
            {
                DivisionsPerRowChanged(this, EventArgs.Empty);
            }

            if (MonthShowFullWeekChanged != null)
            {
                MonthShowFullWeekChanged(this, EventArgs.Empty);
            }

            if (VisualStyleChanged != null)
            {
                VisualStyleChanged(this, EventArgs.Empty);
            }

            if (MonthCalendarStartDayOfWeekChanged != null)
            {
                MonthCalendarStartDayOfWeekChanged(this, EventArgs.Empty);
            }

            if (NavigationCalendarStartDayOfWeekChanged != null)
            {
                NavigationCalendarStartDayOfWeekChanged(this, EventArgs.Empty);
            }

            if (WeekCalendarStartDayOfWeekChanged != null)
            {
                WeekCalendarStartDayOfWeekChanged(this, EventArgs.Empty);
            }

            if (LongHeaderFormatChanged != null)
            {
                LongHeaderFormatChanged(this, EventArgs.Empty);
            }
            if (WeekMonthFullFormatChanged != null)
            {
                WeekMonthFullFormatChanged(this, EventArgs.Empty);
            }
            if (WeekMonthNewMonthChanged != null)
            {
                WeekMonthNewMonthChanged(this, EventArgs.Empty);
            }
            if (MonthHeaderFormatChanged != null)
            {
                MonthHeaderFormatChanged(this, EventArgs.Empty);
            }

            if (SpanItemFormatLeftTextChanged != null)
            {
                SpanItemFormatLeftTextChanged(this, EventArgs.Empty);
            }

            if (SpanItemFormatRightTextChanged != null)
            {
                SpanItemFormatRightTextChanged(this, EventArgs.Empty);
            }

            if (SpanItemFormatMiddleTextChanged != null)
            {
                SpanItemFormatMiddleTextChanged(this, EventArgs.Empty);
            }

            if (SpanItemFormatTerminalLeftTextChanged != null)
            {
                SpanItemFormatTerminalLeftTextChanged(this, EventArgs.Empty);
            }

            if (SpanItemFormatTerminalRightTextChanged != null)
            {
                SpanItemFormatTerminalRightTextChanged(this, EventArgs.Empty);
            }

            if (NavigationCalendarTodayBackColorChanged != null)
            {
                NavigationCalendarTodayBackColorChanged(this, EventArgs.Empty);
            }

            if (NavigationCalendarTodayTextColorChanged != null)
            {
                NavigationCalendarTodayTextColorChanged(this, EventArgs.Empty);
            }

            if (MoreItemArrowBorderColorChanged != null)
            {
                MoreItemArrowBorderColorChanged(this, EventArgs.Empty);
            }

            if (MoreItemArrowColorChanged != null)
            {
                MoreItemArrowColorChanged(this, EventArgs.Empty);
            }

            if (MoreItemArrowHoverColorChanged != null)
            {
                MoreItemArrowHoverColorChanged(this, EventArgs.Empty);
            }

            if (MoreItemsArrowDirectionChanged != null)
            {
                MoreItemsArrowDirectionChanged(this, EventArgs.Empty);
            }
            ////addproperty - need to add code here when you add a property to ScheduleAppearance
		}

		private ScheduleControl schedule;

		/// <summary>
		/// A static method that creates an instance of <see cref="ScheduleAppearance"/> that was
		/// previously serialized as a binary file.
		/// </summary>
		/// <param name="fileName">The serialized filename.</param>
		/// <returns>A ScheduleAppearance.</returns>
		public static ScheduleAppearance LoadBinary(string fileName)
		{
			Stream s = File.OpenRead(fileName);
			ScheduleAppearance appearance = null;
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(Syncfusion.ScheduleWindowsAssembly.AssemblyResolver);
				BinaryFormatter b = new BinaryFormatter();
				b.AssemblyFormat = FormatterAssemblyStyle.Simple;
				object obj = b.Deserialize(s);
				appearance = obj as ScheduleAppearance;
			}
			finally
			{
				s.Close();
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(Syncfusion.ScheduleWindowsAssembly.AssemblyResolver);
			}

			return appearance;
		}

        /// <summary>
        /// Creates an instance of <see cref="ScheduleAppearance"/> that was
        /// previously serialized as a XML file.
        /// </summary>
        /// <param name="fileName">The serialized filename.</param>
        /// <returns>A ScheduleAppearance.</returns>
        public static ScheduleAppearance LoadXML(string fileName)
        {
            ScheduleAppearance appearance = null;
            Stream s = File.OpenRead(fileName);

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ScheduleAppearance));
                appearance = serializer.Deserialize(s) as ScheduleAppearance;
            }
            finally
            {
                s.Close();
            }

            return appearance;
        }

        /// <summary>
        /// Saves this Appearance object in an XML file 
        /// with the specified filename.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        public void SaveXML(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ScheduleAppearance));
            TextWriter writer = new StreamWriter(fileName);
            serializer.Serialize(writer, this);
            writer.Close();
        }

		/// <summary>
		/// A method that saves this Appearance object in binary format to a file 
		/// with the specified filename.
		/// </summary>
        /// <param name="fileName">File name.</param>
		public void SaveBinary(string fileName)
		{
			Stream s = File.Create(fileName);
			SaveBinary(s);
			s.Close();
		}

		/// <summary>
		/// A method that saves this Appearance object to a stream in binary format.
		/// </summary>
        /// <param name="s">Stream to save the object.</param>
		public void SaveBinary(Stream s)
		{
			BinaryFormatter b = new BinaryFormatter();
			b.AssemblyFormat = FormatterAssemblyStyle.Simple;
			b.Serialize(s, this);
		}

		#region ISerializable Members

		/// <summary>
		/// Implements the ISerializable interface and returns the data needed to serialize the Appearance object.
		/// </summary>
		/// <param name="info">A SerializationInfo object containing the information required to serialize the Appearance object.</param>
		/// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
            int version1 = 8; ////force the version to be 8
			info.AddValue("version", version1);
			info.AddValue("primeTimeCellColor", primeTimeCellColor);
			info.AddValue("workWeekHeaderBackColor", workWeekHeaderBackColor);
			info.AddValue("workWeekHeaderForeColor", workWeekHeaderForeColor);
			info.AddValue("monthWeekHeaderBackColor", monthWeekHeaderBackColor);
			info.AddValue("monthWeekHeaderForeColor", monthWeekHeaderForeColor);
			info.AddValue("allDayBackColor", allDayBackColor);
			info.AddValue("clickItemBorderColor", clickItemBorderColor);
			info.AddValue("timeBackColor", timeBackColor);
			info.AddValue("timeBigFontSize", timeBigFontSize);
			info.AddValue("timeLittleFontSize", timeLittleFontSize);
			info.AddValue("timeTextColor", timeTextColor);
			info.AddValue("textColor", textColor);
			info.AddValue("captionBackColor", captionBackColor);
			info.AddValue("solidBorderColor", solidBorderColor);
			info.AddValue("markColumnColor", markColumnColor);
			info.AddValue("themesEnabled", themesEnabled);
			info.AddValue("showCaption", showCaption);
			info.AddValue("showCaptionButtons", showCaptionButtons);
			info.AddValue("primeTimeStart", primeTimeStart);
			info.AddValue("primeTimeEnd", primeTimeEnd);
			info.AddValue("ScheduleAppointmentTipFormat", ScheduleAppointmentTipFormat);
			info.AddValue("dayItemFormat", dayItemFormat);
			info.AddValue("weekMonthItemFormat", weekMonthItemFormat);
			info.AddValue("allDayItemFormat", allDayItemFormat);
			info.AddValue("dateTimeFormat", dateTimeFormat);
			info.AddValue("dateFormat", dateFormat);
			info.AddValue("timeFormat", timeFormat);
			info.AddValue("ScheduleAppointmentTipsEnabled", ScheduleAppointmentTipsEnabled);
			info.AddValue("hours24", hours24);
			info.AddValue("dayMonthCutoff", dayMonthCutoff);
			info.AddValue("showTime", showTime);
			info.AddValue("navigationCalendarSelectionColor", navigationCalendarSelectionColor);
			info.AddValue("navigationCalendarBackColor", navigationCalendarBackColor);
			info.AddValue("navigationCalendarWeekNumberColor", navigationCalendarWeekNumberColor);
			info.AddValue("navigationCalendarArrowColor", navigationCalendarArrowColor);
			info.AddValue("navigationCalendarTodayColor", navigationCalendarTodayColor);
			info.AddValue("navigationCalendarDisabledTextColor", navigationCalendarDisabledTextColor);
			info.AddValue("navigationCalendarTextColor", navigationCalendarTextColor);
			info.AddValue("navigationCalendarHeaderColor", navigationCalendarHeaderColor);
			info.AddValue("splitterBackColor", splitterBackColor);
			info.AddValue("dragColor", dragColor);
            if (version >= 1)
            {
                info.AddValue("divisionsPerHour", divisionsPerHour);
                info.AddValue("monthShowFullWeek", monthShowFullWeek);
                info.AddValue("fullWeekHeaderFormat", fullWeekHeaderFormat);
                info.AddValue("workWeekHeaderFormat", workWeekHeaderFormat);
                info.AddValue("VisualStyle", visualStyle);
                
                ////used for new ScheduleAppearance schemas
            }

            if (version >= 2)
            {
                info.AddValue("weekHeaderFormat", weekHeaderFormat);
            }

            if (version >= 3)
            {
                info.AddValue("navigationCalendarStartDayOfWeek", navigationCalendarStartDayOfWeek);
                info.AddValue("monthCalendarStartDayOfWeek", monthCalendarStartDayOfWeek);
                info.AddValue("weekCalendarStartDayOfWeek", weekCalendarStartDayOfWeek);
            }
            
            if (version >= 4)
            {
                info.AddValue("longHeaderFormat", longHeaderFormat);
            }

            if (version >= 5)
            {
                info.AddValue("spanItemFormatLeftText", spanItemFormatLeftText);
                info.AddValue("spanItemFormatRightText", spanItemFormatRightText);
                info.AddValue("spanItemFormatMiddleText", spanItemFormatMiddleText);
                info.AddValue("spanItemFormatTerminalLeftText", spanItemFormatTerminalLeftText);
                info.AddValue("spanItemFormatTerminalRightText", spanItemFormatTerminalRightText);
            }

            if (version >= 6)
            {
                info.AddValue("divisionsPerRow", divisionsPerRow);
            }
            
            if (version >= 7)
            {
                info.AddValue("monthHeaderFormat", monthHeaderFormat);
                info.AddValue("weekMonthNewMonth", weekMonthNewMonth);
                info.AddValue("weekMonthFullFormat", weekMonthFullFormat);
            }
			
            if (version >= 8)
            {
                info.AddValue("navigationCalendarTodayBackColor", navigationCalendarTodayBackColor);
                info.AddValue("navigationCalendarTodayTextColor", navigationCalendarTodayTextColor);
                info.AddValue("moreItemArrowColor", moreItemArrowColor);
                info.AddValue("moreItemArrowHoverColor", moreItemArrowHoverColor);
                info.AddValue("moreItemArrowBorderColor", moreItemArrowBorderColor);
            }

            ////addproperty  - need to add code here when you add a property to ScheduleAppearance

		}

		#endregion
		#endregion

		#region Properties

		#region PrimeTimeCellColor
		private Color primeTimeCellColor = Color.LightGoldenrodYellow;
		
        /// <summary>
		/// A property that gets / sets the prime time cells in the Calender.
		/// </summary>
		[Browsable(true)]
		[Description("The backcolor of prime time cells.")]
		[DefaultValue(typeof(System.Drawing.Color), "LightGoldenrodYellow")]
		[Category("Prime Time")]
        [XmlIgnore]
		public Color PrimeTimeCellColor
		{
            get
            {
                return primeTimeCellColor;
            }

			set
			{
				if (value != primeTimeCellColor)
				{
					primeTimeCellColor = value;
                    if (PrimeTimeCellColorChanged != null)
                    {
                        PrimeTimeCellColorChanged(this, EventArgs.Empty);
                    }
				}
			}
		}
		
        /// <summary>
		/// A notification event that is raised after PrimeTimeCellColor is modified. 
		/// </summary>
		public event EventHandler PrimeTimeCellColorChanged;

        /// <summary>Only for internal use.</summary>
        /// <remarks>
        ///     Internal only
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        [XmlElement("PrimeTimeCellColor")]
        public string PrimeTimeCellColorString
        {
            get
            {
                return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, PrimeTimeCellColor);
            }

            set
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
                try
                {
                    PrimeTimeCellColor = (Color)tc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, value);
                }
                catch
                {
                    PrimeTimeCellColor = (Color)tc.ConvertFromString(value);
                }
            }
        }

		#endregion

		#region NonPrimeTimeCellColor
		private Color nonPrimeTimeCellColor = Color.Khaki;
		
        /// <summary>
		/// A property that gets / sets the non-prime time cells in the Calender.
		/// </summary>
		[Browsable(true)]
		[Description("The backcolor of non-prime time cells.")]
		[DefaultValue(typeof(System.Drawing.Color), "Khaki")]
		[Category("Prime Time")]
        [XmlIgnore]
        public Color NonPrimeTimeCellColor
		{
            get
            {
                return nonPrimeTimeCellColor;
            }

			set
			{
				if (value != nonPrimeTimeCellColor)
				{
					nonPrimeTimeCellColor = value;
                    if (NonPrimeTimeCellColorChanged != null)
                    {
                        NonPrimeTimeCellColorChanged(this, EventArgs.Empty);
                    }
				}
			}
		}
		
        /// <summary>
		/// A notification event that is raised after PrimeTimeCellColor is modified. 
		/// </summary>
		public event EventHandler NonPrimeTimeCellColorChanged;

        /// <summary>Only for internal use.</summary>
        /// <remarks>
        ///     Internal only
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        [XmlElement("NonPrimeTimeCellColor")]
        public string NonPrimeTimeCellColorString
        {
            get
            {
                return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, NonPrimeTimeCellColor);
            }

            set
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
                try
                {
                    NonPrimeTimeCellColor = (Color)tc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, value);
                }
                catch 
                {
                    NonPrimeTimeCellColor = (Color)tc.ConvertFromString(value);
                }
            }
        }

		#endregion

		#region WorkWeekHeaderBackColor
		private Color workWeekHeaderBackColor = Color.LightGray;
		
        /// <summary>
		/// A property that gets / sets the backcolor of header cells in a WorkWeek view.
		/// </summary>
		[Browsable(true)]
		[Description("The backcolor of headers cells in a WorkWeek view.")]
		[DefaultValue(typeof(System.Drawing.Color), "LightGray")]
		[Category("Headers")]
        [XmlIgnore]
        public Color WorkWeekHeaderBackColor
		{
            get
            {
                return workWeekHeaderBackColor;
            }

			set
			{
				if (value != workWeekHeaderBackColor)
				{
					workWeekHeaderBackColor = value;
                    if (WorkWeekHeaderBackColorChanged != null)
                    {
                        WorkWeekHeaderBackColorChanged(this, EventArgs.Empty);
                    }
				}
			}
		}
		
        /// <summary>
		/// A notification event that is raised after WorkWeekHeaderBackColor is modified. 
		/// </summary>
		public event EventHandler WorkWeekHeaderBackColorChanged;

        /// <summary>Only for internal use.</summary>
        /// <remarks>
        ///     Internal only
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        [XmlElement("WorkWeekHeaderBackColor")]
        public string WorkWeekHeaderBackColorString
        {
            get
            {
                return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, WorkWeekHeaderBackColor);
            }

            set
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
                try
                {
                    WorkWeekHeaderBackColor = (Color)tc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, value);
                }
                catch 
                {
                    WorkWeekHeaderBackColor = (Color)tc.ConvertFromString(value);
                }
            }
        }

		#endregion

		#region WorkWeekHeaderForeColor
		private Color workWeekHeaderForeColor = Color.DarkGray;
		
        /// <summary>
		/// A property that gets / sets the forecolor of header cells in a WorkWeek view.
		/// </summary>
		[Browsable(true)]
		[Description("The forecolor of headers cells in a WorkWeek view.")]
		[DefaultValue(typeof(System.Drawing.Color), "DarkGray")]
		[Category("Headers")]
        [XmlIgnore]
        public Color WorkWeekHeaderForeColor
		{
            get
            {
                return workWeekHeaderForeColor;
            }

			set
			{
				if (value != workWeekHeaderForeColor)
				{
					workWeekHeaderForeColor = value;
                    if (WorkWeekHeaderForeColorChanged != null)
                    {
                        WorkWeekHeaderForeColorChanged(this, EventArgs.Empty);
                    }
				}
			}
		}
		
        /// <summary>
		/// A notification event that is raised after WorkWeekHeaderForeColor is modified. 
		/// </summary>
		public event EventHandler WorkWeekHeaderForeColorChanged;

        /// <summary>Only for internal use.</summary>
        /// <remarks>
        ///     Internal only
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        [XmlElement("WorkWeekHeaderForeColor")]
        public string WorkWeekHeaderForeColorString
        {
            get
            {
                return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, WorkWeekHeaderForeColor);
            }

            set
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
                try
                {
                    WorkWeekHeaderForeColor = (Color)tc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, value);
                }
                catch 
                {
                    WorkWeekHeaderForeColor = (Color)tc.ConvertFromString(value);
                }
            }
        }

		#endregion

		#region MonthWeekHeaderBackColor
		private Color monthWeekHeaderBackColor = Color.LightGray;
		
        /// <summary>
		/// A property that gets / sets the backcolor of header cells in a Month or Week view.
		/// </summary>
		[Browsable(true)]
		[Description("The backcolor of headers cells in a Month or Week view.")]
		[DefaultValue(typeof(System.Drawing.Color), "LightGray")]
		[Category("Headers")]
        [XmlIgnore]
        public Color MonthWeekHeaderBackColor
		{
            get
            {
                return monthWeekHeaderBackColor;
            }

			set
			{
				if (value != monthWeekHeaderBackColor)
				{
					monthWeekHeaderBackColor = value;
                    if (MonthWeekHeaderBackColorChanged != null)
                    {
                        MonthWeekHeaderBackColorChanged(this, EventArgs.Empty);
                    }
				}
			}
		}
		
        /// <summary>
		/// A notification event that is raised after MonthWeekHeaderBackColor is modified. 
		/// </summary>
		public event EventHandler MonthWeekHeaderBackColorChanged;

        /// <summary>Only for internal use.</summary>
        /// <remarks>
        ///     Internal only
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        [XmlElement("MonthWeekHeaderBackColor")]
        public string MonthWeekHeaderBackColorString
        {
            get
            {
                return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, MonthWeekHeaderBackColor);
            }

            set
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
                try
                {
                    MonthWeekHeaderBackColor = (Color)tc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, value);
                }
                catch 
                {
                    MonthWeekHeaderBackColor = (Color)tc.ConvertFromString(value);
                }
            }
        }

		#endregion

		#region MonthWeekHeaderForeColor
		private Color monthWeekHeaderForeColor = Color.DarkGray;
		
        /// <summary>
		/// A property that gets / sets the forecolor of header cells in a Month or Week view.
		/// </summary>
		[Browsable(true)]
		[Description("The forecolor of headers cells in a Month or Week view.")]
		[DefaultValue(typeof(System.Drawing.Color), "DarkGray")]
		[Category("Headers")]
        [XmlIgnore]
        public Color MonthWeekHeaderForeColor
		{
            get
            {
                return monthWeekHeaderForeColor;
            }

			set
			{
				if (value != monthWeekHeaderForeColor)
				{
					monthWeekHeaderForeColor = value;
                    if (MonthWeekHeaderForeColorChanged != null)
                    {
                        MonthWeekHeaderForeColorChanged(this, EventArgs.Empty);
                    }
				}
			}
		}
		
        /// <summary>
		/// A notification event that is raised after MonthWeekHeaderForeColor is modified. 
		/// </summary>
		public event EventHandler MonthWeekHeaderForeColorChanged;
        
        /// <summary>Only for internal use.</summary>
        /// <remarks>
        ///     Internal only
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        [XmlElement("MonthWeekHeaderForeColor")]
        public string MonthWeekHeaderForeColorString
        {
            get
            {
                return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, MonthWeekHeaderForeColor);
            }

            set
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
                try
                {
                    MonthWeekHeaderForeColor = (Color)tc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, value);
                }
                catch 
                {
                    MonthWeekHeaderForeColor = (Color)tc.ConvertFromString(value);
                }
            }
        }

		#endregion

		#region AllDayBackColor
		private Color allDayBackColor = Color.LightGray;
        private Color todayBackColor = Color.FromArgb(124, 204, 196);
		
        /// <summary>
		/// A property that gets / sets the backcolor of the AllDay row in the calendar.
		/// </summary>
		[Browsable(true)]
		[Description("The backcolor of the AllDay row in the calendar.")]
		[DefaultValue(typeof(System.Drawing.Color), "LightGray")]
		[Category("Headers")]
        [XmlIgnore]
        public Color AllDayBackColor
		{
            get
            {
                return allDayBackColor;
            }

			set
			{
				if (value != allDayBackColor)
				{
					allDayBackColor = value;
                    if (AllDayBackColorChanged != null)
                    {
                        AllDayBackColorChanged(this, EventArgs.Empty);
                    }
				}
			}
		}

        /// <summary>
        /// A property that gets / sets the backcolor of the AllDay row in the calendar.
        /// </summary>
        [Browsable(true)]
        [Description("The backcolor of the CurrentDay row in the calendar.")]
        [DefaultValue(typeof(System.Drawing.Color), "LightGray")]
        [Category("Headers")]
        [XmlIgnore]
        public Color TodayBackColor
        {
            get
            {
                return todayBackColor;
            }

            set
            {
                if (value != todayBackColor)
                {
                    todayBackColor = value;
                }
            }
        }
		
        /// <summary>
		/// A notification event that is raised after AllDayBackColor is modified. 
		/// </summary>
		public event EventHandler AllDayBackColorChanged;

        /// <summary>Only for internal use.</summary>
        /// <remarks>
        ///     Internal only
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        [XmlElement("AllDayBackColor")]
        public string AllDayBackColorString
        {
            get
            {
                return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, AllDayBackColor);
            }

            set
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
                try
                {
                    AllDayBackColor = (Color)tc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, value);
                }
                catch 
                {
                    AllDayBackColor = (Color)tc.ConvertFromString(value);
                }
            }
        }

		#endregion

		#region ClickItemBorderColor

		private Color clickItemBorderColor = Color.Red;
		
        /// <summary>
		/// A property that gets / sets the border color for a clicked item.
		/// </summary>
		[Browsable(true)]
		[Description("Gets/sets the border color for a clicked item.")]
		[DefaultValue(typeof(System.Drawing.Color), "Red")]
		[Category("Borders")]
        [XmlIgnore]
        public Color ClickItemBorderColor
		{
            get
            {
                return clickItemBorderColor;
            }

			set
			{
				if (value != clickItemBorderColor)
				{
					clickItemBorderColor = value;
                    if (ClickItemBorderColorChanged != null)
                    {
                        ClickItemBorderColorChanged(this, EventArgs.Empty);
                    }
				}
			}
		}
		
        /// <summary>
		/// A notification event that is raised after ClickItemBorderColor is modified. 
		/// </summary>
		public event EventHandler ClickItemBorderColorChanged;

        /// <summary>Only for internal use.</summary>
        /// <remarks>
        ///     Internal only
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        [XmlElement("ClickItemBorderColor")]
        public string ClickItemBorderColorString
        {
            get
            {
                return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, ClickItemBorderColor);
            }

            set
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
                try
                {
                    ClickItemBorderColor = (Color)tc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, value);
                }
                catch 
                {
                    ClickItemBorderColor = (Color)tc.ConvertFromString(value);
                }
            }
        }

		#endregion

		#region DragColor

		private Color dragColor = Color.Red;
		
        /// <summary>
		/// A property that gets / sets the drag border color for a dragging item.
		/// </summary>
		[Browsable(true)]
		[Description("Gets/sets the drag border color for a dragging item.")]
		[DefaultValue(typeof(System.Drawing.Color), "Red")]
		[Category("Borders")]
        [XmlIgnore]
        public Color DragColor
		{
            get
            {
                return dragColor;
            }

			set
			{
				if (value != dragColor)
				{
					dragColor = value;
                    if (DragColorChanged != null)
                    {
                        DragColorChanged(this, EventArgs.Empty);
                    }
				}
			}
		}
		
        /// <summary>
		/// A notification event that is raised after DragColor is modified. 
		/// </summary>
		public event EventHandler DragColorChanged;

        /// <summary>Only for internal use.</summary>
        /// <remarks>
        ///     Internal only
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        [XmlElement("DragColor")]
        public string DragColorString
        {
            get
            {
                return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, DragColor);
            }

            set
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
                try
                {
                    DragColor = (Color)tc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, value);
                }
                catch 
                {
                    DragColor = (Color)tc.ConvertFromString(value);
                }
            }
        }

		#endregion

		#region TimeBackColor
		private Color timeBackColor = Color.FromArgb(100, Color.LightGray);
		
        /// <summary>
		/// A property that gets / sets the backcolor of the Time column in the calendar.
		/// </summary>
		[Browsable(true)]
		[Description("The backcolor of the Time column in the calendar.")]
		[DefaultValue(typeof(System.Drawing.Color), "100,211,211,211")]
		[Category("Time Column")]
        [XmlIgnore]
        public Color TimeBackColor
		{
            get
            {
                return timeBackColor;
            }

			set
			{
				if (value != timeBackColor)
				{
					timeBackColor = value;
                    if (TimeBackColorChanged != null)
                    {
                        TimeBackColorChanged(this, EventArgs.Empty);
                    }
				}
			}
		}
		
        /// <summary>
		/// A notification event that is raised after TimeBackColor is modified. 
		/// </summary>
		public event EventHandler TimeBackColorChanged;

        /// <summary>Only for internal use.</summary>
        /// <remarks>
        ///     Internal only
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        [XmlElement("TimeBackColor")]
        public string TimeBackColorString
        {
            get
            {
                return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, TimeBackColor);
            }

            set
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
                try
                {
                    TimeBackColor = (Color)tc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, value);
                }
                catch 
                {
                    TimeBackColor = (Color)tc.ConvertFromString(value);
                }
            }
        }

		#endregion

		#region TimeBigFontSize

		/// <summary>
		/// size of the font for the large text in the time col display
		/// </summary>
		private float timeBigFontSize = 14.0f;
		
        /// <summary>
		/// A property that gets / sets the size of the larger font used in the Time column in the calendar.
		/// </summary>
		[Browsable(true)]
		[Description("The size of the larger font used in the Time column.")]
		[DefaultValue(14.0f)]
		[Category("Time Column")]
		public float TimeBigFontSize
		{
            get
            {
                return timeBigFontSize;
            }

			set
			{
				if (value != timeBigFontSize)
				{
					timeBigFontSize = value;
                    if (TimeBigFontSizeChanged != null)
                    {
                        TimeBigFontSizeChanged(this, EventArgs.Empty);
                    }
				}
			}
		}
		
        /// <summary>
		/// A notification event that is raised after TimeBigFontSize is modified. 
		/// </summary>
		public event EventHandler TimeBigFontSizeChanged;
		#endregion

		#region TimeLittleFontSize
		private float timeLittleFontSize = 8.25f;
		
        /// <summary>
		/// A property that gets / sets the size of the smaller font used in the Time column in the calendar.
		/// </summary>
		[Browsable(true)]
		[Description("The size of the smaller font used in the Time column.")]
		[DefaultValue(8.25f)]
		[Category("Time Column")]
		public float TimeLittleFontSize
		{
            get
            {
                return timeLittleFontSize;
            }

			set
			{
				if (value != timeLittleFontSize)
				{
					timeLittleFontSize = value;
                    if (TimeLittleFontSizeChanged != null)
                    {
                        TimeLittleFontSizeChanged(this, EventArgs.Empty);
                    }
				}
			}
		}
		
        /// <summary>
		/// A notification event that is raised after LittleFontSize is modified. 
		/// </summary>
		public event EventHandler TimeLittleFontSizeChanged;

		#endregion
		
		#region TimeTextColor
		private Color timeTextColor = Color.Black;
		
        /// <summary>
		/// A property that gets / sets the color of the text shown in the Time column in the calendar.
		/// </summary>
		[Browsable(true)]
		[Description("The color of the text shown in the Time column.")]
		[DefaultValue(typeof(System.Drawing.Color), "Black")]
		[Category("Time Column")]
        [XmlIgnore]
        public Color TimeTextColor
		{
            get
            {
                return timeTextColor;
            }

			set
			{
				if (value != timeTextColor)
				{
					timeTextColor = value;
                    if (TimeTextColorChanged != null)
                    {
                        TimeTextColorChanged(this, EventArgs.Empty);
                    }
				}
			}
		}
		
        /// <summary>
		/// A notification event that is raised after TimeTextColor is modified. 
		/// </summary>
		public event EventHandler TimeTextColorChanged;

        /// <summary>Only for internal use.</summary>
        /// <remarks>
        ///     Internal only
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        [XmlElement("TimeTextColor")]
        public string TimeTextColorString
        {
            get
            {
                return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, TimeTextColor);
            }

            set
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
                try
                {
                    TimeTextColor = (Color)tc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, value);
                }
                catch 
                {
                    TimeTextColor = (Color)tc.ConvertFromString(value);
                }
            }
        }

		#endregion

		#region TextColor
		private Color textColor = Color.Black;
		
        /// <summary>
		/// A property that gets / sets the color of the basic text shown in the calendar.
		/// </summary>
		[Browsable(true)]
		[Description("The color of the basic text shown in schedule.")]
		[DefaultValue(typeof(System.Drawing.Color), "Black")]
		[Category("Misc")]
        [XmlIgnore]
        public Color TextColor
		{
            get
            {
                return textColor;
            }

			set
			{
				if (value != textColor)
				{
					textColor = value;
                    if (TextColorChanged != null)
                    {
                        TextColorChanged(this, EventArgs.Empty);
                    }
				}
			}
		}
		
        /// <summary>
		/// A notification event that is raised after TextColor is modified. 
		/// </summary>
		public event EventHandler TextColorChanged;

        /// <summary>Only for internal use.</summary>
        /// <remarks>
        ///     Internal only
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        [XmlElement("TextColor")]
        public string TextColorString
        {
            get
            {
                return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, TextColor);
            }

            set
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
                try
                {
                    TextColor = (Color)tc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, value);
                }
                catch 
                {
                    TextColor = (Color)tc.ConvertFromString(value);
                }
            }
        }

		#endregion

        #region NavigationCalendarStartDayOfWeek
        private DayOfWeek navigationCalendarStartDayOfWeek = DayOfWeek.Sunday;
        
        /// <summary>
        /// A property that gets / sets the DayOfWeek that is shown in the left-most column of the calendar.
        /// </summary>
        [Browsable(true)]
        [Description("DayOfWeek that is shown in the left-most column of the calendar.")]
        [DefaultValue(typeof(DayOfWeek), "Sunday")]
        [Category("NavigationCalendar")]
        public DayOfWeek NavigationCalendarStartDayOfWeek
        {
            get
            {
                return navigationCalendarStartDayOfWeek;
            }

            set
            {
                if (value != navigationCalendarStartDayOfWeek)
                {
                    navigationCalendarStartDayOfWeek = value;
                    if (NavigationCalendarStartDayOfWeekChanged != null)
                    {
                        NavigationCalendarStartDayOfWeekChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// A notification event that is raised after NavigationCalendarStartDayOfWeek is modified. 
        /// </summary>
        public event EventHandler NavigationCalendarStartDayOfWeekChanged;

        #endregion

		#region NavigationCalendarBackColor
		private Color navigationCalendarBackColor = SystemColors.Window;
		
        /// <summary>
		/// A property that gets / sets the BackColor of the navigation calendar.
		/// </summary>
		[Browsable(true)]
		[Description("The BackColor of the of the navigation calendar.")]
		[DefaultValue(typeof(System.Drawing.Color), "White")]
		[Category("NavigationCalendar")]
        [XmlIgnore]
        public Color NavigationCalendarBackColor
		{
            get
            {
                return navigationCalendarBackColor;
            }

			set
			{
				if (value != navigationCalendarBackColor)
				{
					navigationCalendarBackColor = value;
                    if (NavigationCalendarBackColorChanged != null)
                    {
                        NavigationCalendarBackColorChanged(this, EventArgs.Empty);
                    }
				}
			}
		}

		/// <summary>
		/// A notification event that is raised after NavigationCalendaBackColor is modified. 
		/// </summary>
		public event EventHandler NavigationCalendarBackColorChanged;

        /// <summary>Only for internal use.</summary>
        /// <remarks>
        ///     Internal only
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        [XmlElement("NavigationCalendarBackColor")]
        public string NavigationCalendarBackColorString
        {
            get
            {
                return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, NavigationCalendarBackColor);
            }

            set
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
                try
                {
                    NavigationCalendarBackColor = (Color)tc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, value);
                }
                catch 
                {
                    NavigationCalendarBackColor = (Color)tc.ConvertFromString(value);
                }
            }
        }

		#endregion

		#region NavigationCalendarWeekNumberColor
		private Color navigationCalendarWeekNumberColor = Color.Black;
		
        /// <summary>
		/// A property that gets / sets the color of the week numbers in the navigation calendar.
		/// </summary>
		[Browsable(true)]
		[Description("The color of the week numbers in the navigation calendar.")]
		[DefaultValue(typeof(System.Drawing.Color), "Black")]
		[Category("NavigationCalendar")]
        [XmlIgnore]
        public Color NavigationCalendarWeekNumberColor
		{
            get
            {
                return navigationCalendarWeekNumberColor;
            }

			set
			{
				if (value != navigationCalendarWeekNumberColor)
				{
					navigationCalendarWeekNumberColor = value;
                    if (NavigationCalendarWeekNumberColorChanged != null)
                    {
                        NavigationCalendarWeekNumberColorChanged(this, EventArgs.Empty);
                    }
				}
			}
		}

		/// <summary>
		/// A notification event that is raised after NavigationCalendarWeekNumberColor is modified. 
		/// </summary>
		public event EventHandler NavigationCalendarWeekNumberColorChanged;

        /// <summary>Only for internal use.</summary>
        /// <remarks>
        ///     Internal only
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        [XmlElement("NavigationCalendarWeekNumberColor")]
        public string NavigationCalendarWeekNumberColorString
        {
            get
            {
                return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, NavigationCalendarWeekNumberColor);
            }

            set
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
                try
                {
                    NavigationCalendarWeekNumberColor = (Color)tc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, value);
                }
                catch 
                {
                    NavigationCalendarWeekNumberColor = (Color)tc.ConvertFromString(value);
                }
            }
        }

		#endregion

		#region NavigationCalendarArrowColor
		private Color navigationCalendarArrowColor = Color.Black;
		
        /// <summary>
		/// A property that gets / sets the color of the Arrows in the navigation calendar.
		/// </summary>
		[Browsable(true)]
		[Description("The color of the Arrows in the navigation calendar.")]
		[DefaultValue(typeof(System.Drawing.Color), "Black")]
		[Category("NavigationCalendar")]
        [XmlIgnore]
        public Color NavigationCalendarArrowColor
		{
            get
            {
                return navigationCalendarArrowColor;
            }

			set
			{
				if (value != navigationCalendarArrowColor)
				{
					navigationCalendarArrowColor = value;
                    if (NavigationCalendarArrowColorChanged != null)
                    {
                        NavigationCalendarArrowColorChanged(this, EventArgs.Empty);
                    }
				}
			}
		}

		/// <summary>
		/// A notification event that is raised after NavigationCalendarArrowColor is modified. 
		/// </summary>
		public event EventHandler NavigationCalendarArrowColorChanged;

        /// <summary>Only for internal use.</summary>
        /// <remarks>
        ///     Internal only
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        [XmlElement("NavigationCalendarArrowColor")]
        public string NavigationCalendarArrowColorString
        {
            get
            {
                return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, NavigationCalendarArrowColor);
            }

            set
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
                try
                {
                    NavigationCalendarArrowColor = (Color)tc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, value);
                }
                catch 
                {
                    NavigationCalendarArrowColor = (Color)tc.ConvertFromString(value);
                }
            }
        }

		#endregion

		#region NavigationCalendarTodayColor
		private Color navigationCalendarTodayColor = Color.Red;
		
        /// <summary>
		/// A property that gets / sets the color of the text of Today in the navigation calendar.
		/// </summary>
		[Browsable(true)]
		[Description("The color of the text of Today in the navigation calendar.")]
		[DefaultValue(typeof(System.Drawing.Color), "Red")]
		[Category("NavigationCalendar")]
        [XmlIgnore]
        public Color NavigationCalendarTodayColor
		{
            get
            {
                return navigationCalendarTodayColor;
            }

			set
			{
				if (value != navigationCalendarTodayColor)
				{
					navigationCalendarTodayColor = value;
                    if (NavigationCalendarTodayColorChanged != null)
                    {
                        NavigationCalendarTodayColorChanged(this, EventArgs.Empty);
                    }
				}
			}
		}

        private Color moreItemArrowColor = Color.FromArgb(181, 181, 181);
        private Color moreItemArrowHoverColor = Color.FromArgb(31, 113, 183);
        private Color moreItemArrowBorderColor = Color.FromArgb(255, 255, 255);
        /// <summary>
        /// A notification event that is raised after MoreItemArrowColor is modified. 
        /// </summary>
        public event EventHandler MoreItemArrowColorChanged;
        /// <remarks>
        ///     which is enable after setting metro theme only.
        /// </remarks>
        /// <summary>
        /// A property that gets / sets the color of the MoreItem Arrow Color.
        /// </summary>
        [Browsable(true)]
        [Description("The color of the MoreItem Arrow in the schedule.")]
        [Category("Misc")]
        [XmlIgnore]
        public Color MoreItemArrowColor
        {
            get
            {
                return moreItemArrowColor;
            }

            set
            {
                if (value != moreItemArrowColor)
                {
                    moreItemArrowColor = value;
                    if (MoreItemArrowColorChanged != null)
                    {
                        MoreItemArrowColorChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// A notification event that is raised after MoreItemArrowBorderColor is modified. 
        /// </summary>
        public event EventHandler MoreItemArrowBorderColorChanged;
        /// <remarks>
        ///     which is enable after setting metro theme only.
        /// </remarks>
        /// <summary>
        /// A property that gets / sets the border color of the more item bitmap.
        /// </summary>
        [Browsable(true)]
        [Description("The border color of the MoreItem Arrow in the schedule.")]
        [Category("Misc")]
        [XmlIgnore]
        public Color MoreItemArrowBorderColor
        {
            get
            {
                return moreItemArrowBorderColor;
            }

            set
            {
                if (value != moreItemArrowBorderColor)
                {
                    moreItemArrowBorderColor = value;
                    if (MoreItemArrowBorderColorChanged != null)
                    {
                        MoreItemArrowBorderColorChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// A notification event that is raised after MoreItemArrowHoverColor is modified. 
        /// </summary>
        public event EventHandler MoreItemArrowHoverColorChanged;
        /// <remarks>
        ///     which is enable after setting metro theme only.
        /// </remarks>
        /// <summary>
        /// A property that gets / sets the hover color of the more items bitmap.
        /// </summary>
        [Browsable(true)]
        [Description("The hover color of the MoreItem Arrow in the schedule.")]
        [Category("Misc")]
        [XmlIgnore]
        public Color  MoreItemArrowHoverColor
        {
            get
            {
                return moreItemArrowHoverColor;
            }

            set
            {
                if (value != moreItemArrowHoverColor)
                {
                    moreItemArrowHoverColor = value;
                    if (MoreItemArrowHoverColorChanged != null)
                    {
                        MoreItemArrowHoverColorChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// A notification event that is raised after NavigationCalendarTodayBackColor is modified. 
        /// </summary>
        public event EventHandler NavigationCalendarTodayBackColorChanged;

        /// <summary>
        /// A property that gets / sets the back color of the text of Today in the navigation calendar.
        /// </summary>
        [Browsable(true)]
        [Description("The color of the text of Today in the navigation calendar.")]
        [DefaultValue(typeof(System.Drawing.Color), "Red")]
        [Category("NavigationCalendar")]
        [XmlIgnore]
        public Color NavigationCalendarTodayBackColor
        {
            get
            {
                return navigationCalendarTodayBackColor;
            }

            set
            {
                if (value != navigationCalendarTodayBackColor)
                {
                    navigationCalendarTodayBackColor = value;
                    if (NavigationCalendarTodayBackColorChanged != null)
                    {
                        NavigationCalendarTodayBackColorChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

		/// <summary>
		/// A notification event that is raised after NavigationCalendarTodayColor is modified. 
		/// </summary>
		public event EventHandler NavigationCalendarTodayColorChanged;

        /// <summary>Only for internal use.</summary>
        /// <remarks>
        ///     Internal only
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        [XmlElement("NavigationCalendarTodayColor")]
        public string NavigationCalendarTodayColorString
        {
            get
            {
                return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, NavigationCalendarTodayColor);
            }

            set
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
                try
                {
                    NavigationCalendarTodayColor = (Color)tc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, value);
                }
                catch 
                {
                    NavigationCalendarTodayColor = (Color)tc.ConvertFromString(value);
                }
            }
        }

		#endregion

		#region NavigationCalendarDisabledTextColor
		private Color navigationCalendarDisabledTextColor = Color.Gray;
		
        /// <summary>
		/// A property that gets / sets the color of the disabled text in the navigation calendar.
		/// </summary>
		[Browsable(true)]
		[Description("The color of the disabled text in the navigation calendar.")]
		[DefaultValue(typeof(System.Drawing.Color), "Gray")]
		[Category("NavigationCalendar")]
        [XmlIgnore]
        public Color NavigationCalendarDisabledTextColor
		{
            get
            {
                return navigationCalendarDisabledTextColor;
            }

			set
			{
				if (value != navigationCalendarDisabledTextColor)
				{
					navigationCalendarDisabledTextColor = value;
                    if (NavigationCalendarDisabledTextColorChanged != null)
                    {
                        NavigationCalendarDisabledTextColorChanged(this, EventArgs.Empty);
                    }
				}
			}
		}

		/// <summary>
		/// A notification event that is raised after NavigationCalendarDisabledTextColor is modified. 
		/// </summary>
		public event EventHandler NavigationCalendarDisabledTextColorChanged;

        /// <summary>Only for internal use.</summary>
        /// <remarks>
        ///     Internal only
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        [XmlElement("NavigationCalendarDisabledTextColor")]
        public string NavigationCalendarDisabledTextColorString
        {
            get
            {
                return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, NavigationCalendarDisabledTextColor);
            }

            set
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
                try
                {
                    NavigationCalendarDisabledTextColor = (Color)tc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, value);
                }
                catch 
                {
                    NavigationCalendarDisabledTextColor = (Color)tc.ConvertFromString(value);
                }
            }
        }

		#endregion

		#region NavigationCalendarTextColor
		private Color navigationCalendarTextColor = Color.Black;
		
        /// <summary>
		/// A property that gets / sets the color of the header color in the navigation calendar.
		/// </summary>
		[Browsable(true)]
		[Description("The color of the header color of the navigation calendar.")]
		[DefaultValue(typeof(System.Drawing.Color), "Black")]
		[Category("NavigationCalendar")]
        [XmlIgnore]
        public Color NavigationCalendarTextColor
		{
            get
            {
                return navigationCalendarTextColor;
            }

			set
			{
				if (value != navigationCalendarTextColor)
				{
					navigationCalendarTextColor = value;
                    if (NavigationCalendarTextColorChanged != null)
                    {
                        NavigationCalendarTextColorChanged(this, EventArgs.Empty);
                    }
				}
			}
		}

		/// <summary>
		/// A notification event that is raised after NavigationCalendarTextColor is modified. 
		/// </summary>
		public event EventHandler NavigationCalendarTextColorChanged;

        /// <summary>Only for internal use.</summary>
        /// <remarks>
        ///     Internal only
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        [XmlElement("NavigationCalendarTextColor")]
        public string NavigationCalendarTextColorString
        {
            get
            {
                return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, NavigationCalendarTextColor);
            }

            set
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
                try
                {
                    NavigationCalendarTextColor = (Color)tc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, value);
                }
                catch 
                {
                    NavigationCalendarTextColor = (Color)tc.ConvertFromString(value);
                }
            }
        }

		#endregion

		#region NavigationCalendarHeaderColor
		private Color navigationCalendarHeaderColor = Color.FromArgb(215, 215, 229);
		
        /// <summary>
		/// A property that gets / sets the color of the header in the navigation calendar.
		/// </summary>
		[Browsable(true)]
		[Description("The color of the header in the navigation calendar.")]
		[DefaultValue(typeof(System.Drawing.Color), "215,215,229")]
		[Category("NavigationCalendar")]
        [XmlIgnore]
        public Color NavigationCalendarHeaderColor
		{
            get
            {
                return navigationCalendarHeaderColor;
            }

			set
			{
				if (value != navigationCalendarHeaderColor)
				{
					navigationCalendarHeaderColor = value;
                    if (NavigationCalendarHeaderColorChanged != null)
                    {
                        NavigationCalendarHeaderColorChanged(this, EventArgs.Empty);
                    }
				}
			}
		}

		/// <summary>
		/// A notification event that is raised after NavigationCalendarHeaderColor is modified. 
		/// </summary>
		public event EventHandler NavigationCalendarHeaderColorChanged;

        /// <summary>Only for internal use.</summary>
        /// <remarks>
        ///     Internal only
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        [XmlElement("NavigationCalendarHeaderColor")]
        public string NavigationCalendarHeaderColorString
        {
            get
            {
                return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, NavigationCalendarHeaderColor);
            }

            set
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
                try
                {
                    NavigationCalendarHeaderColor = (Color)tc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, value);
                }
                catch 
                {
                    NavigationCalendarHeaderColor = (Color)tc.ConvertFromString(value);
                }
            }
        }

		#endregion

		#region NavigationCalendarSelectionColor
        private Color navigationCalendarSelectionColor = Color.FromArgb(205, 230, 247);
        private Color navigationCalendarTodayBackColor = Color.FromArgb(0, 114, 198);
        private Color navigationCalendarTodayTextColor = Color.FromArgb(255, 255, 255);//Color.FromArgb(205, 230, 247);
        /// <summary>
		/// A property that gets / sets the color of the selection color in the navigation calendar.
		/// </summary>
		[Browsable(true)]
		[Description("The color of the selection color in the navigation calendar.")]
        [DefaultValue(typeof(System.Drawing.Color), "205, 230, 247")]
		[Category("NavigationCalendar")]
        [XmlIgnore]
        public Color NavigationCalendarSelectionColor
		{
            get
            {
                return navigationCalendarSelectionColor;
            }

			set
			{
				if (value != navigationCalendarSelectionColor)
				{
					navigationCalendarSelectionColor = value;
                    if (NavigationCalendarSelectionColorChanged != null)
                    {
                        NavigationCalendarSelectionColorChanged(this, EventArgs.Empty);
                    }
				}
			}
		}

        /// <summary>
        /// A notification event that is raised after NavigationCalendarTodayTextColor is modified. 
        /// </summary>
        public event EventHandler NavigationCalendarTodayTextColorChanged;

        /// <summary>
        /// A property that gets / sets the color of the current date text in the navigation calendar.
        /// </summary>
        [Browsable(true)]
        [Description("The color of the current date text in the navigation calendar.")]
        [DefaultValue(typeof(System.Drawing.Color), "255,255,255")]
        [Category("NavigationCalendar")]
        [XmlIgnore]
        public Color NavigationCalendarTodayTextColor
        {
            get
            {
                return navigationCalendarTodayTextColor;
            }

            set
            {
                if (value != navigationCalendarTodayTextColor)
                {
                    navigationCalendarTodayTextColor = value;
                    if (NavigationCalendarTodayTextColorChanged != null)
                    {
                        NavigationCalendarTodayTextColorChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

		/// <summary>
		/// A notification event that is raised after NavigationCalendarSelectionColor is modified. 
		/// </summary>
		public event EventHandler NavigationCalendarSelectionColorChanged;

        /// <summary>Only for internal use.</summary>
        /// <remarks>
        ///     Internal only
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        [XmlElement("NavigationCalendarSelectionColor")]
        public string NavigationCalendarSelectionColorString
        {
            get
            {
                return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, NavigationCalendarSelectionColor);
            }

            set
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
                try
                {
                    NavigationCalendarSelectionColor = (Color)tc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, value);
                }
                catch 
                {
                    NavigationCalendarSelectionColor = (Color)tc.ConvertFromString(value);
                }
            }
        }

		#endregion

		#region SplitterBackColor
		private Color splitterBackColor = Color.FromArgb(215, 215, 229);
		
        /// <summary>
		/// A property that gets / sets the back color of the two splitters in the ScheduleControl.
		/// </summary>
		[Browsable(true)]
		[Description("The back color of the two splitters.")]
		[DefaultValue(typeof(System.Drawing.Color), "215,215,229")]
		[Category("Misc")]
        [XmlIgnore]
        public Color SplitterBackColor
		{
            get
            {
                return splitterBackColor;
            }

			set
			{
				if (value != splitterBackColor)
				{
					splitterBackColor = value;
                    if (SplitterBackColorChanged != null)
                    {
                        SplitterBackColorChanged(this, EventArgs.Empty);
                    }
				}
			}
		}
		
        /// <summary>
		/// A notification event that is raised after SplitterBackColor is modified. 
		/// </summary>
		public event EventHandler SplitterBackColorChanged;

        /// <summary>Only for internal use.</summary>
        /// <remarks>
        ///     Internal only
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        [XmlElement("SplitterBackColor")]
        public string SplitterBackColorString
        {
            get
            {
                return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, SplitterBackColor);
            }

            set
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
                try
                {
                    SplitterBackColor = (Color)tc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, value);
                }
                catch 
                {
                    SplitterBackColor = (Color)tc.ConvertFromString(value);
                }
            }
        }

		#endregion

		#region CaptionBackColor
		private Color captionBackColor = Color.LightGray;
		
        /// <summary>
		/// A property that gets / sets the color of the caption area above the calendar.
		/// </summary>
		[Browsable(true)]
		[Description("The color of the caption area above the calendar.")]
		[DefaultValue(typeof(System.Drawing.Color), "LightGray")]
		[Category("Caption")]
        [XmlIgnore]
        public Color CaptionBackColor
		{
            get
            {
                return captionBackColor;
            }

			set
			{
				if (value != captionBackColor)
				{
					captionBackColor = value;
                    if (CaptionBackColorChanged != null)
                    {
                        CaptionBackColorChanged(this, EventArgs.Empty);
                    }
				}
			}
		}

        /// <summary>
        /// Gets the culture used for the date formatting.
        /// </summary>
        /// <remarks>Defaults to the CultureInfo.InvariantCulture setting.</remarks>
        public CultureInfo Culture
        {
            get
            {
                return this.schedule.Culture;
            }
        }

        private bool iso8601CalenderFormat = false;

        /// <summary>
        /// Gets or Sets a value indicating whether ISO 8601 calender format is applied or not.
        /// </summary>
        internal bool ISO8601CalenderFormat
        {
            get
            {
                return this.iso8601CalenderFormat;
            }
            set
            {
                if (this.iso8601CalenderFormat != value)
                {
                    this.iso8601CalenderFormat = value;
                }
            }
        }

        /// <summary>
		/// A notification event that is raised after CaptionBackColor is modified. 
		/// </summary>
		public event EventHandler CaptionBackColorChanged;

        /// <summary>Only for internal use.</summary>
        /// <remarks>
        ///     Internal only
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        [XmlElement("CaptionBackColor")]
        public string CaptionBackColorString
        {
            get
            {
                return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, CaptionBackColor);
            }

            set
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
                try
                {
                    CaptionBackColor = (Color)tc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, value);
                }
                catch 
                {
                    CaptionBackColor = (Color)tc.ConvertFromString(value);
                }
            }
        }

		#endregion

		#region SolidBorderColor
		private Color solidBorderColor = Color.Gray;
		
        /// <summary>
		/// A property that gets / sets the color of the solid lines in the calendar.
		/// </summary>
		[Browsable(true)]
		[Description("The color of the solid lines the calendar.")]
		[DefaultValue(typeof(System.Drawing.Color), "Gray")]
		[Category("Borders")]
        [XmlIgnore]
        public Color SolidBorderColor
		{
            get
            {
                return solidBorderColor;
            }

			set
			{
				if (value != solidBorderColor)
				{
					solidBorderColor = value;
                    if (SolidBorderColorChanged != null)
                    {
                        SolidBorderColorChanged(this, EventArgs.Empty);
                    }
				}
			}
		}
		
        /// <summary>
		/// A notification event that is raised after SolidBorderColor is modified. 
		/// </summary>
		public event EventHandler SolidBorderColorChanged;

        /// <summary>Only for internal use.</summary>
        /// <remarks>
        ///     Internal only
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        [XmlElement("SolidBorderColor")]
        public string SolidBorderColorString
        {
            get
            {
                return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, SolidBorderColor);
            }

            set
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
                try
                {
                    SolidBorderColor = (Color)tc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, value);
                }
                catch 
                {
                    SolidBorderColor = (Color)tc.ConvertFromString(value);
                }
            }
        }

		#endregion

		#region MarkColumnColor
		private Color markColumnColor = Color.FromArgb(50, Color.RoyalBlue);
		
        /// <summary>
		/// A property that gets / sets the color of the thick solid line next to the Time Column the Day View in the calendar.
		/// </summary>
		[Browsable(true)]
		[Description("The color of the thick solid line next to the Time Column in the Day View.")]
		[DefaultValue(typeof(System.Drawing.Color), "50,65,105,225")]
		[Category("Time Column")]
        [XmlIgnore]
        public Color MarkColumnColor
		{
            get
            {
                return markColumnColor;
            }

			set
			{
				if (value != markColumnColor)
				{
					markColumnColor = value;
                    if (MarkColumnColorChanged != null)
                    {
                        MarkColumnColorChanged(this, EventArgs.Empty);
                    }
				}
			}
		}
		
        /// <summary>
		/// A notification event that is raised after MarkColumnColor is modified. 
		/// </summary>
		public event EventHandler MarkColumnColorChanged;

        /// <summary>Only for internal use.</summary>
        /// <remarks>
        ///     Internal only
        /// </remarks>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Syncfusion.Documentation.DocumentationExclude()]
        [XmlElement("MarkColumnColor")]
        public string MarkColumnColorString
        {
            get
            {
                return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, MarkColumnColor);
            }

            set
            {
                TypeConverter tc = TypeDescriptor.GetConverter(typeof(Color));
                try
                {
                    MarkColumnColor = (Color)tc.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, value);
                }
                catch 
                {
                    MarkColumnColor = (Color)tc.ConvertFromString(value);
                }
            }
        }

		#endregion

		#region ThemesEnabled
		private bool themesEnabled = true;
		
        /// <summary>
		/// A property that gets / sets whether the Themes are enabled.
		/// </summary>
		[Browsable(true)]
		[Description("Indicates whether themes are enabled.")]
		[DefaultValue(true)]
		public bool ThemesEnabled
		{
            get
            {
                return themesEnabled;
            }

			set
			{
				if (value != themesEnabled)
				{
					themesEnabled = value;
                    if (ThemesEnabledChanged != null)
                    {
                        ThemesEnabledChanged(this, EventArgs.Empty);
                    }
				}
			}
		}
		
        /// <summary>
		/// A notification event that is raised after ThemesEnabled is modified. 
		/// </summary>
		public event EventHandler ThemesEnabledChanged;

        /// <summary>
        /// Specifies the position of the More Items Arrow Directions.
        /// </summary>
        public enum MoreItemsArrowDirections
        {
            Up,
            Right,
            Down,
            Left
        }

        private MoreItemsArrowDirections moreItemsArrowDirection = MoreItemsArrowDirections.Down;
        /// <summary>
        /// Gets / sets whether the arrow direction of the more items icon.
        /// </summary>
        [Browsable(true)]
        [Description("Indicates the arrow direction of the appointment.")]
        public MoreItemsArrowDirections MoreItemsArrowDirection
        {
            get
            {
                return moreItemsArrowDirection;
            }

            set
            {
                if (value != moreItemsArrowDirection)
                {
                    moreItemsArrowDirection = value;
                    if (MoreItemsArrowDirectionChanged != null)
                    {
                        MoreItemsArrowDirectionChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// A notification event that is raised after Arrow direction is modified. 
        /// </summary>
        public event EventHandler MoreItemsArrowDirectionChanged;

		#endregion

		#region ShowCaption
		private bool showCaption = true;
		
        /// <summary>
		/// A property that gets / sets whether the Caption Panel above the Calendar is visible.
		/// </summary>
		[Browsable(true)]
		[Description("Indicates whether the Caption Panel above the Calendar is visible.")]
		[DefaultValue(true)]
		[Category("Caption")]
		public bool ShowCaption
		{
            get
            {
                return showCaption;
            }

			set
			{
				if (value != showCaption)
				{
					showCaption = value;
                    if (ShowCaptionChanged != null)
                    {
                        ShowCaptionChanged(this, EventArgs.Empty);
                    }
				}
			}
		}
		
        /// <summary>
		/// A notification event that is raised after ShowCaption is modified. 
		/// </summary>
		public event EventHandler ShowCaptionChanged;

		#endregion

		#region ShowCaptionButtons
		private bool showCaptionButtons = true;
		
        /// <summary>
		/// A property that gets / sets whether navigation buttons are shown on the Caption Panel.
		/// </summary>
		[Browsable(true)]
		[Description("indicates whether navigation buttons are shown on the Caption Panel.")]
		[DefaultValue(true)]
		[Category("Caption")]
		public bool ShowCaptionButtons
		{
            get
            {
                return showCaptionButtons;
            }

			set
			{
				if (value != showCaptionButtons)
				{
					showCaptionButtons = value;
                    if (ShowCaptionButtonsChanged != null)
                    {
                        ShowCaptionButtonsChanged(this, EventArgs.Empty);
                    }
				}
			}
		}
		
        /// <summary>
		/// A notification event that is raised after ShowCaptionButtons is modified. 
		/// </summary>
		public event EventHandler ShowCaptionButtonsChanged;

		#endregion

		#region PrimeTimeStart
		private int primeTimeStart = 8;
		
        /// <summary>
		/// A property that gets / sets the time (0 to 23 in hours) when prime time color starts being used in the display.
		/// </summary>
		[Browsable(true)]
		[Description("the time (0 to 23 in hours) when prime time color starts being used in the display.")]
		[DefaultValue(8)]
		[Category("Prime Time")]
		public int PrimeTimeStart
		{
            get
            {
                return primeTimeStart;
            }

			set
			{
				if (value != primeTimeStart)
				{
					primeTimeStart = value;
                    if (PrimeTimeStartChanged != null)
                    {
                        PrimeTimeStartChanged(this, EventArgs.Empty);
                    }
				}
			}
		}
		
        /// <summary>
		/// A notification event that is raised after PrimeTimeStart is modified. 
		/// </summary>
		public event EventHandler PrimeTimeStartChanged;

		#endregion

		#region PrimeTimeEnd
		private int primeTimeEnd = 17;
		
        /// <summary>
		/// A property that gets / sets the time (0 to 23 in hours) when the prime time color stops being used in the display.
		/// </summary>
		[Browsable(true)]
		[Description("the time (0 to 23 in hours) when the prime time color stops being used in the display.")]
		[DefaultValue(17)]
		[Category("Prime Time")]
		public int PrimeTimeEnd
		{
            get
            {
                return primeTimeEnd;
            }

			set
			{
				if (value != primeTimeEnd)
				{
                    if (value == 24)
                    {
                        value = 23;
                    }

					primeTimeEnd = value;
                    if (PrimeTimeEndChanged != null)
                    {
                        PrimeTimeEndChanged(this, EventArgs.Empty);
                    }
				}
			}
		}
		
        /// <summary>
		/// A notification event that is raised after PrimeTimeEnd is modified. 
		/// </summary>
		public event EventHandler PrimeTimeEndChanged;

		#endregion

		#region formats

		#region ScheduleAppointmentTipFormat

		private string scheduleAppointmentTipFormat = "[subject]\r\n\r\n[content]";

		/// <summary>
		/// A property that defines the text that is displayed for schedule item tips.
		/// </summary>
		/// <remarks>You can use tokens defined in ScheduleGrid.DisplayItemFormatStrings
		/// to control the determine the text displayed. The default
		/// value is "[subject]\r\n\r\n[content]".</remarks>
		[Browsable(true)]
		[Description("Gets or sets whether item tips should be displayed.")]
		[DefaultValue("[subject]\r\n\r\n[content]")]
		[Category("Misc")]
		public string ScheduleAppointmentTipFormat
		{
            get
            {
                return scheduleAppointmentTipFormat;
            }

			set
			{
				if (scheduleAppointmentTipFormat != value)
				{
					scheduleAppointmentTipFormat = value;
                    if (ScheduleAppointmentTipFormatChanged != null)
                    {
                        ScheduleAppointmentTipFormatChanged(this, EventArgs.Empty);
                    }
				}
			}
		}

		/// <summary>
		/// A notification event that is raised after ScheduleAppointmentTipFormat is modified. 
		/// </summary>
		public event EventHandler ScheduleAppointmentTipFormatChanged;
		#endregion

		#region DayItemFormat

		private string dayItemFormat = "[subject] [starttime]";
		
        /// <summary>
		/// A property that gets/sets the display format of a schedule item displayed in a Day/WorkWeek view.
		/// </summary>
		/// <remarks>
		/// Choose from these tokens.
		/// "allday", "end", "label", "location", "marker", "owner", "reminder", "subject", "start", "no closing delimiter error", "starttime", "endtime", "startdate", "enddate" 
		/// </remarks>
		[Browsable(true)]
		[Description("The display format of a schedule item displayed in a Day/WorkWeek view.")]
		[DefaultValue("[subject] [starttime]")]
		[Category("Display Item Formats")]
		public string DayItemFormat
		{
            get
            {
                return dayItemFormat;
            }

			set
			{
				if (value != dayItemFormat)
				{
					dayItemFormat = value;
                    if (DayItemFormatChanged != null)
                    {
                        DayItemFormatChanged(this, EventArgs.Empty);
                    }
				}
			}
		}
		
        /// <summary>
		/// A notification event that is raised after DayItemFormat is modified. 
		/// </summary>
		public event EventHandler DayItemFormatChanged;
		#endregion

		#region WeekMonthItemFormat

		private string weekMonthItemFormat = "[subject] [starttime]";
		
        /// <summary>
		/// A property that gets / sets the display format of a schedule item shown in a Week/Month view.
        /// Note:
        /// New Line character ("\n") is not allowed and will be ignogered if entered any.
		/// </summary>
		/// <remarks>
		/// Choose from these tokens.
		/// "allday", "end", "label", "location", "marker", "owner", "reminder", "subject", "start", "no closing delimiter error", "starttime", "endtime", "startdate", "enddate" 
		/// </remarks>
		[Browsable(true)]
		[Description("The display format of a schedule item shown in a Week/Month view.")]
		[DefaultValue("[subject] [starttime]")]
		[Category("Display Item Formats")]
		public string WeekMonthItemFormat
		{
            get
            {
                return weekMonthItemFormat;
            }

			set
			{
				if (value != weekMonthItemFormat)
				{
					weekMonthItemFormat = value.Replace("\n","");
                    if (WeekMonthItemFormatChanged != null)
                    {
                        WeekMonthItemFormatChanged(this, EventArgs.Empty);
                    }
				}
			}
		}
		
        /// <summary>
		/// A notification event that is raised after DayItemFormat is modified. 
		/// </summary>
		public event EventHandler WeekMonthItemFormatChanged;
		#endregion	

        #region FullWeekHeaderFormat

        private string fullWeekHeaderFormat = "D";
        
        /// <summary>
        /// A property that gets / sets the display format of header of a day in a Week view.
        /// </summary>
        /// <remarks>
        /// The default format is "D", the long date format.
        /// </remarks>
        [Browsable(true)]
        [Description("The display format of header of a day in a Week view.")]
        [DefaultValue("D")]
        [Category("Display Item Formats")]
        public string FullWeekHeaderFormat
        {
            get
            {
                return fullWeekHeaderFormat;
            }

            set
            {
                if (value != fullWeekHeaderFormat)
                {
                    fullWeekHeaderFormat = value;
                    if (FullWeekHeaderFormatChanged != null)
                    {
                        FullWeekHeaderFormatChanged(this, EventArgs.Empty);
                    }
                }
            }
        }
       
        /// <summary>
        /// A notification event that is raised after FullWeekHeaderFormat is modified. 
        /// </summary>
        public event EventHandler FullWeekHeaderFormatChanged;
        #endregion	

        #region WorkWeekHeaderFormat

        private string workWeekHeaderFormat = "MM/dd/yyyy";
        
        /// <summary>
        /// A property that gets / sets the display format of header of a day in a WorkWeek view.
        /// </summary>
        /// <remarks>
        /// The default format is "MM/dd/yyyy".
        /// </remarks>
        [Browsable(true)]
        [Description("The display format of header of a day in a WorkWeek view.")]
        [DefaultValue("MM/dd/yyyy")]
        [Category("Display Item Formats")]
        public string WorkWeekHeaderFormat
        {
            get
            {
                return workWeekHeaderFormat;
            }

            set
            {
                if (value != workWeekHeaderFormat)
                {
                    workWeekHeaderFormat = value;
                    if (WorkWeekHeaderFormatChanged != null)
                    {
                        WorkWeekHeaderFormatChanged(this, EventArgs.Empty);
                    }
                }
            }
        }
        
        /// <summary>
        /// A notification event that is raised after WorkWeekHeaderFormat is modified. 
        /// </summary>
        public event EventHandler WorkWeekHeaderFormatChanged;
        #endregion	

        #region WeekHeaderFormat

        private string weekHeaderFormat = "MMM d";
        
        /// <summary>
        /// A property that gets / sets the date display format of header label in a WorkWeek view.
        /// </summary>
        /// <remarks>
        /// The default format is "MMM d".
        /// </remarks>
        [Browsable(true)]
        [Description("The date display format of header label in a WorkWeek view")]
        [DefaultValue("MMM d")]
        [Category("Display Item Formats")]
        public string WeekHeaderFormat
        {
            get
            {
                return weekHeaderFormat;
            }

            set
            {
                if (value != weekHeaderFormat)
                {
                    weekHeaderFormat = value;
                    if (WeekHeaderFormatChanged != null)
                    {
                        WeekHeaderFormatChanged(this, EventArgs.Empty);
                    }
                }
            }
        }
        
        /// <summary>
        /// A notification event that is raised after WeekHeaderFormat is modified. 
        /// </summary>
        public event EventHandler WeekHeaderFormatChanged;
        #endregion	

        #region LongHeaderFormat

        private string longHeaderFormat = "dddd, d MMMM yyyy";
        
        /// <summary>
        /// A property that gets / sets the display format of header of a Day view.
        /// </summary>
        /// <remarks>
        /// The default format is "dddd, d MMMM yyyy", the short date format.
        /// </remarks>
        [Browsable(true)]
        [Description("The display format of header of a Day view.")]
        [DefaultValue("dddd, d MMMM yyyy")]
        [Category("Display Item Formats")]
        public string LongHeaderFormat
        {
            get
            {
                return longHeaderFormat;
            }

            set
            {
                if (value != longHeaderFormat)
                {
                    longHeaderFormat = value;
                    if (LongHeaderFormatChanged != null)
                    {
                        LongHeaderFormatChanged(this, EventArgs.Empty);
                    }
                }
            }
        }
        
        /// <summary>
        /// A notification event that is raised after LongHeaderFormat is modified. 
        /// </summary>
        public event EventHandler LongHeaderFormatChanged;
        #endregion	

		#region AllDayItemFormat

		private string allDayItemFormat = "[subject] [starttime]";
		
        /// <summary>
		/// A property that gets / sets the display format of an AllDay item.
		/// </summary>
		/// <remarks>
		/// Choose from these tokens.
		/// "allday", "end", "label", "location", "marker", "owner", "reminder", "subject", "start", "no closing delimiter error", "starttime", "endtime", "startdate", "enddate" 
		/// </remarks>
		[Browsable(true)]
		[Description("The display format of an AllDay item.")]
		[DefaultValue("[subject] [starttime]")]
		[Category("Display Item Formats")]
		public string AllDayItemFormat
		{
            get
            {
                return allDayItemFormat;
            }

			set
			{
				if (value != allDayItemFormat)
				{
					allDayItemFormat = value;
                    if (AllDayItemFormatChanged != null)
                    {
                        AllDayItemFormatChanged(this, EventArgs.Empty);
                    }
				}
			}
		}
		
        /// <summary>
		/// A notification event that is raised after DayItemFormat is modified. 
		/// </summary>
		public event EventHandler AllDayItemFormatChanged;
		#endregion

        #region SpanItemFormatLeftText

        private string spanItemFormatLeftText = "[starttime]";
        
        /// <summary>
        /// A property that gets / sets the display format of a text displayed on an interior left-side of a multiday span.
        /// </summary>
        /// <remarks>
        /// Choose from these tokens.
        /// "allday", "end", "label", "location", "marker", "owner", "reminder", "subject", "start", "no closing delimiter error", "starttime", "endtime", "startdate", "enddate" 
        /// </remarks>
        [Browsable(true)]
        [Description("Gets/sets the display format of a text displayed on an interior left-side of a multiday span.")]
        [DefaultValue("[starttime]")]
        [Category("Display Item Formats")]
        public string SpanItemFormatLeftText
        {
            get
            {
                return spanItemFormatLeftText;
            }

            set
            {
                if (value != spanItemFormatLeftText)
                {
                    spanItemFormatLeftText = value;
                    if (SpanItemFormatLeftTextChanged != null)
                    {
                        SpanItemFormatLeftTextChanged(this, EventArgs.Empty);
                    }
                }
            }
        }
        
        /// <summary>
        /// A notification event that is raised after SpanItemFormatLeftText is modified. 
        /// </summary>
        public event EventHandler SpanItemFormatLeftTextChanged;
        #endregion

        #region WeekMonthFullFormat
        private string weekMonthFullFormat = "MMMM d, yyyy";

        /// <summary>
        /// Gets/sets the display format of the text displayed at the start of a new year in the Month or Week view, and in the cell header of the Week view.
        /// </summary>
        [Browsable(true)]
        [Description("Gets/sets the display format of the text displayed at the start of a new year in the Month or Week view, and in the cell header of the Week view.")]
        [DefaultValue("MMMM d, yyyy")]
        [Category("Display Item Formats")]
        public string WeekMonthFullFormat
        {
            get
            {
                return weekMonthFullFormat;
            }

            set
            {
                if (value != weekMonthFullFormat)
                {
                    weekMonthFullFormat = value;
                    if (WeekMonthFullFormatChanged != null)
                    {
                       WeekMonthFullFormatChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// A notification event that is raised after WeekMonthFullFormat is modified. 
        /// </summary>
        public event EventHandler WeekMonthFullFormatChanged;
        #endregion

        #region WeekMonthNewMonth
        private string weekMonthNewMonth = "MMMM d";

        /// <summary>
        /// Gets/sets the display format of the text displayed at the start of a new month in the Month view.
        /// </summary>
        [Browsable(true)]
        [Description("Gets/sets the display format of the text displayed at the start of a new month in the Month view.")]
        [DefaultValue("MMMM d")]
        [Category("Display Item Formats")]
        public string WeekMonthNewMonth
        {
            get
            {
                return weekMonthNewMonth;
            }

            set
            {
                if (value != weekMonthNewMonth)
                {
                    weekMonthNewMonth = value;
                    if (WeekMonthNewMonthChanged != null)
                    {
                        WeekMonthNewMonthChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// A notification event that is raised after WeekMonthNewMonth is modified. 
        /// </summary>
        public event EventHandler WeekMonthNewMonthChanged;
        #endregion

        #region MonthHeaderFormat

        private string monthHeaderFormat = "MMMM yyyy";

        /// <summary>
        /// Gets/sets the display format of the text displayed in the header label for the Month view.
        /// </summary>
         [Browsable(true)]
        [Description("Gets/sets the display format of the text displayed in the header label for the Month view.")]
        [DefaultValue("MMMM yyyy")]
        [Category("Display Item Formats")]
        public string MonthHeaderFormat
        {
            get
            {
                return monthHeaderFormat;
            }

            set
            {
                if (value != monthHeaderFormat)
                {
                    monthHeaderFormat = value;
                    if (MonthHeaderFormatChanged != null)
                    {
                        MonthHeaderFormatChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
         /// A notification event that is raised after MonthHeaderFormat is modified. 
        /// </summary>
         public event EventHandler MonthHeaderFormatChanged;
        #endregion

        #region SpanItemFormatRightText

        private string spanItemFormatRightText = "[endtime]";
       
        /// <summary>
        /// A property that gets / sets the display format of a text displayed on an interior right-side of a multiday span.
        /// </summary>
        /// <remarks>
        /// Choose from these tokens.
        /// "allday", "end", "label", "location", "marker", "owner", "reminder", "subject", "start", "no closing delimiter error", "starttime", "endtime", "startdate", "enddate" 
        /// </remarks>
        [Browsable(true)]
        [Description("Gets/sets the display format of a text displayed on an interior right-side of a multiday span.")]
        [DefaultValue("[endtime]")]
        [Category("Display Item Formats")]
        public string SpanItemFormatRightText
        {
            get
            {
                return spanItemFormatRightText;
            }

            set
            {
                if (value != spanItemFormatRightText)
                {
                    spanItemFormatRightText = value;
                    if (SpanItemFormatRightTextChanged != null)
                    {
                        SpanItemFormatRightTextChanged(this, EventArgs.Empty);
                    }
                }
            }
        }
        
        /// <summary>
        /// A notification event that is raised after SpanItemFormatRighttText is modified. 
        /// </summary>
        public event EventHandler SpanItemFormatRightTextChanged;
        #endregion

        #region SpanItemFormatMiddleText

        private string spanItemFormatMiddleText = "[subject]";
       
        /// <summary>
        /// A property that gets / sets the display format of a text displayed in the middle of a multiday span.
        /// </summary>
        /// <remarks>
        /// Choose from these tokens.
        /// "allday", "end", "label", "location", "marker", "owner", "reminder", "subject", "start", "no closing delimiter error", "starttime", "endtime", "startdate", "enddate" 
        /// </remarks>
        [Browsable(true)]
        [Description("Gets/sets the display format of a text displayed in the middle of a multiday span.")]
        [DefaultValue("[subject]")]
        [Category("Display Item Formats")]
        public string SpanItemFormatMiddleText
        {
            get
            {
                return spanItemFormatMiddleText;
            }

            set
            {
                if (value != spanItemFormatMiddleText)
                {
                    spanItemFormatMiddleText = value;
                    if (SpanItemFormatMiddleTextChanged != null)
                    {
                        SpanItemFormatMiddleTextChanged(this, EventArgs.Empty);
                    }
                }
            }
        }
        
        /// <summary>
        /// A notification event that is raised after SpanItemFormatMiddleText is modified. 
        /// </summary>
        public event EventHandler SpanItemFormatMiddleTextChanged;
        #endregion

        #region SpanItemFormatTerminalLeftText

        private string spanItemFormatTerminalLeftText = "[startdate]";
        
        /// <summary>
        /// A property that gets / sets the display format of a text displayed on an open left-side of a multiday span.
        /// </summary>
        /// <remarks>
        /// Choose from these tokens.
        /// "allday", "end", "label", "location", "marker", "owner", "reminder", "subject", "start", "no closing delimiter error", "starttime", "endtime", "startdate", "enddate" 
        /// </remarks>
        [Browsable(true)]
        [Description("Gets/sets the display format of a text displayed on an open left-side of a multiday span.")]
        [DefaultValue("[startdate]")]
        [Category("Display Item Formats")]
        public string SpanItemFormatTerminalLeftText
        {
            get
            {
                return spanItemFormatTerminalLeftText;
            }

            set
            {
                if (value != spanItemFormatTerminalLeftText)
                {
                    spanItemFormatTerminalLeftText = value;
                    if (SpanItemFormatTerminalLeftTextChanged != null)
                    {
                        SpanItemFormatTerminalLeftTextChanged(this, EventArgs.Empty);
                    }
                }
            }
        }
        
        /// <summary>
        /// A notification event that is raised after SpanItemFormatTerminalLeftText is modified. 
        /// </summary>
        public event EventHandler SpanItemFormatTerminalLeftTextChanged;
        #endregion

        #region SpanItemFormatTerminalRightText

        private string spanItemFormatTerminalRightText = "[enddate]";
        
        /// <summary>
        /// A property that gets / sets the display format of a text displayed on an open right-side of a multiday span.
        /// </summary>
        /// <remarks>
        /// Choose from these tokens.
        /// "allday", "end", "label", "location", "marker", "owner", "reminder", "subject", "start", "no closing delimiter error", "starttime", "endtime", "startdate", "enddate" 
        /// </remarks>
        [Browsable(true)]
        [Description("Gets/sets the display format of a text displayed on an open right-side of a multiday span.")]
        [DefaultValue("[enddate]")]
        [Category("Display Item Formats")]
        public string SpanItemFormatTerminalRightText
        {
            get
            {
                return spanItemFormatTerminalRightText;
            }

            set
            {
                if (value != spanItemFormatTerminalRightText)
                {
                    spanItemFormatTerminalRightText = value;
                    if (SpanItemFormatTerminalRightTextChanged != null)
                    {
                        SpanItemFormatTerminalRightTextChanged(this, EventArgs.Empty);
                    }
                }
            }
        }
        
        /// <summary>
        /// A notification event that is raised after SpanItemFormatRighttText is modified. 
        /// </summary>
        public event EventHandler SpanItemFormatTerminalRightTextChanged;
        #endregion

		#region DateTimeFormat

		private string dateTimeFormat = "g";
		
        /// <summary>
		/// A property that gets or sets the format string used when formatting any of
		/// the tokens from DisplayItemFormatStrings that represents combined 
		/// date and time values.
		/// </summary>
		[Browsable(true)]
		[Description("The display format of any of the DateTime tokens from DisplayItemFormatStrings.")]
		[DefaultValue("g")]
		[Category("Display Item Formats")]
		public string DateTimeFormat
		{
            get
            {
                return dateTimeFormat;
            }

			set
			{
				if (value != dateTimeFormat)
				{
					dateTimeFormat = value;
                    if (DateTimeFormatChanged != null)
                    {
                        DateTimeFormatChanged(this, EventArgs.Empty);
                    }
				}
			}
		}
		
        /// <summary>
		/// A notification event that is raised after DateTimeFormat is modified. 
		/// </summary>
		public event EventHandler DateTimeFormatChanged;
		#endregion

		#region DateFormat
		private string dateFormat = "d";
		
		/// <summary>
		/// A property that gets or sets the format string used when formatting any of
		/// the tokens from DisplayItemFormatStrings that represents a 
		/// date only value.
		/// </summary>
		[Browsable(true)]
		[Description("The display format of any of the Date tokens from DisplayItemFormatStrings.")]
		[DefaultValue("d")]
		[Category("Display Item Formats")]
		public string DateFormat
		{
            get
            {
                return dateFormat;
            }

			set
			{
				if (value != dateFormat)
				{
					dateFormat = value;
                    if (DateFormatChanged != null)
                    {
                        DateFormatChanged(this, EventArgs.Empty);
                    }
				}
			}
		}

		/// <summary>
		/// A notification event that is raised after DateFormat is modified. 
		/// </summary>
		public event EventHandler DateFormatChanged;

		#endregion

		#region TimeFormat

		private string timeFormat = "t";
		
		/// <summary>
		/// A property that gets or sets the format string used when formatting any of
		/// the tokens from DisplayItemFormatStrings that represents combined 
		/// a time only value.
		/// </summary>
		[Browsable(true)]
		[Description("The display format of any of the Time tokens from DisplayItemFormatStrings.")]
		[DefaultValue("t")]
		[Category("Display Item Formats")]
		public string TimeFormat
		{
            get
            {
                return timeFormat;
            }

			set
			{
				if (value != timeFormat)
				{
					timeFormat = value;
                    if (TimeFormatChanged != null)
                    {
                        TimeFormatChanged(this, EventArgs.Empty);
                    }
				}
			}
		}
		
        /// <summary>
		/// A notification event that is raised after TimeFormat is modified. 
		/// </summary>
		public event EventHandler TimeFormatChanged;
		#endregion

		#endregion

		#region fonts
		#endregion

		#region other properties

        #region WeekCalendarStartDayOfWeek
        private DayOfWeek weekCalendarStartDayOfWeek = DayOfWeek.Monday;
        
        /// <summary>
        /// A property that gets / sets the DayOfWeek that is shown in the first day of the week calendar.
        /// </summary>
        [Browsable(true)]
        [Description("DayOfWeek that is shown in the first day of the week calendar.")]
        [DefaultValue(typeof(DayOfWeek), "Monday")]
        [Category("Misc")]
        public DayOfWeek WeekCalendarStartDayOfWeek
        {
            get
            {
                return weekCalendarStartDayOfWeek;
            }

            set
            {
                if (value != weekCalendarStartDayOfWeek)
                {
                    weekCalendarStartDayOfWeek = value;
                    if (WeekCalendarStartDayOfWeekChanged != null)
                    {
                        WeekCalendarStartDayOfWeekChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// A notification event that is raised after WeekCalendarStartDayOfWeek is modified. 
        /// </summary>
        public event EventHandler WeekCalendarStartDayOfWeekChanged;

        #endregion

        #region MonthCalendarStartDayOfWeek
        private DayOfWeek monthCalendarStartDayOfWeek = DayOfWeek.Monday;
        
        /// <summary>
        /// A property that gets / sets the DayOfWeek that is shown in the left-most column of the month calendar.
        /// </summary>
        /// <remarks>
        /// This property is related to <see cref="MonthShowFullWeek"/>. Setting MonthCalendarStartDayOfWeek
        /// to any value other than Monday, will force MonthShowFullWeek to true. This is because when 
        /// MonthShowFullWeek is false, the MonthCalendar starts with Monday and shows a stacked
        /// Saturday/Sunday column. Telling the ScheduleControl to start the MonthCalendar with some day other
        /// than Monday will force the MonthCalendar to display full weeks.
        /// </remarks>
        [Browsable(true)]
        [Description("DayOfWeek that is shown in the left-most column of the month calendar.")]
        [DefaultValue(typeof(DayOfWeek), "Monday")]
        [Category("Misc")]
        public DayOfWeek MonthCalendarStartDayOfWeek
        {
            get
            {
                return monthCalendarStartDayOfWeek;
            }

            set
            {
                if (value != monthCalendarStartDayOfWeek)
                {
                    monthCalendarStartDayOfWeek = value;
                    if (value != DayOfWeek.Monday)
                    {
                        monthShowFullWeek = true; ////no changed event raised
                    }

                    if (MonthCalendarStartDayOfWeekChanged != null)
                    {
                        MonthCalendarStartDayOfWeekChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// A notification event that is raised after MonthCalendarStartDayOfWeek is modified. 
        /// </summary>
        public event EventHandler MonthCalendarStartDayOfWeekChanged;

        #endregion

        #region MonthShowFullWeek

        private bool monthShowFullWeek = false;
        
        /// <summary>
        /// A property that gets or sets whether Month view shows seven columns or six columns with Saturday/Sunday stacked."
        /// </summary>
        [Browsable(true)]
        [Description("Gets or sets whether Month view shows seven columns or six columns with Saturday/Sunday stacked.")]
        [DefaultValue(false)]
        [Category("Misc")]
        public bool MonthShowFullWeek
        {
            get
            {
                return monthShowFullWeek;
            }

            set
            {
                if (value != monthShowFullWeek)
                {
                    monthShowFullWeek = value;
                    if (MonthShowFullWeekChanged != null)
                    {
                        MonthShowFullWeekChanged(this, EventArgs.Empty);
                    }
                }
            }
        }
        
        /// <summary>
        /// A notification event that is raised after MonthShowFullWeek is modified. 
        /// </summary>
        public event EventHandler MonthShowFullWeekChanged;
        #endregion

		#region ScheduleAppointmentTipsEnabled
		private bool scheduleAppointmentTipsEnabled = true;

		/// <summary>
		/// A property that gets or sets whether tips should be displayed when you hoover over
		/// an item in a schedule.
		/// </summary>
		[Browsable(true)]
		[Description("Gets or sets whether item tips should be displayed.")]
		[DefaultValue(true)]
		[Category("Misc")]
		public bool ScheduleAppointmentTipsEnabled
		{
            get
            {
                return scheduleAppointmentTipsEnabled;
            }

			set
			{
				if (value != scheduleAppointmentTipsEnabled)
				{
					scheduleAppointmentTipsEnabled = value;
                    if (ScheduleAppointmentTipsEnabledChanged != null)
                    {
                        ScheduleAppointmentTipsEnabledChanged(this, EventArgs.Empty);
                    }
				}
			}
		}
		
        /// <summary>
		/// A notification event that is raised after ScheduleAppointmentTipsEnabled is modified. 
		/// </summary>
		public event EventHandler ScheduleAppointmentTipsEnabledChanged;
		#endregion

		#region Hours24
		private bool hours24;
		
        /// <summary>
		/// A property that gets / sets whether time column is displayed using a 24 hour format.
		/// </summary>
		[Browsable(true)]
		[Description("Gets/sets whether time column is displayed using a 24 hour format.")]
		[DefaultValue(false)]
		[Category("Time Column")]
		public bool Hours24
		{
            get
            {
                return hours24;
            }

			set
			{
				if (value != hours24)
				{
					hours24 = value;
                    if (Hours24Changed != null)
                    {
                        Hours24Changed(this, EventArgs.Empty);
                    }
				}
			}
		}
		
        /// <summary>
		/// A notification event that is raised after Hours24 is modified. 
		/// </summary>
		public event EventHandler Hours24Changed;
		#endregion

		#region DayMonthCutoff

		private int dayMonthCutoff = 9;
		
        /// <summary>
		/// Get or sets the maximum number of days that can appear side by side in a Day style calendar
		/// </summary>
		/// <remarks>
		/// The default value is 9. So, if you select more than 8 dates in the navigation calendar, the dates
		/// cannot be displayed side by side in the ScheduleControl.
		/// </remarks>
		[Browsable(true)]
		[Description("Maximum number of days that can appear side by side in a Day style calendar.")]
		[DefaultValue(9)]
		[Category("Misc")]
		public int DayMonthCutoff
		{
            get
            {
                return dayMonthCutoff;
            }

			set
			{
				if (value != dayMonthCutoff)
				{
					dayMonthCutoff = value;
                    if (DayMonthCutoffChanged != null)
                    {
                        DayMonthCutoffChanged(this, EventArgs.Empty);
                    }
				}
			}
		}
		
        /// <summary>
		/// A notification event that is raised after DayMonthCutoff is modified. 
		/// </summary>
		public event EventHandler DayMonthCutoffChanged;
		#endregion

        #region DivisionsPerRow
        private int divisionsPerRow = 1;
        
        /// <summary>
        /// Gets or sets the number of time divisions per row that appear in a Day, Custom or WorkWeek view. This property is used
        /// in conjunction with DivisionsPerHour to determine the number of grid rows that appear in an hour time slot.
        /// </summary>
        /// <remarks>
        /// The number of time divisions per row determines how many of the time divisions set by DivisionsPerHour appear in each
        /// grid row. 
        /// <para>So, if DivisionPerHour is 4 and DivisionsPerRow is 2, then you would see the hour split into 2 grid rows with
        /// the rows changing at the 30 minute mark, and you could enter appointments that start at 0, 15, 30 or 45 minutes. So, the
        /// hour is divided into 4 time slots on the 15 minute marks but you only see a grid row at the 30 minute mark. 
        /// </para><para>As another example, if DivisionPerHour is 6 and DivisionsPerRow is 2, then you would see the hour split into 2 grid rows with
        /// the rows changing at the 20 and 40 minute marks, and you could enter appointments that start at any 10 minute mark. So, the
        /// hour is divided into 6 time slots on the 10 minute marks and you only see a grid row at the 20 minute marks. 
        /// </para>
        /// The value of DivisionPerRow should be a factor of DivisionsPerHour. If it is not, then DivisionPerRow defaults to 1.
        /// The default values for DivisionPerRow is 1 (to go along with the default value of 2 for the DivisionsPerHour).
        /// </remarks>
        [Browsable(true)]
        [Description("The number of time divisions per grid row.")]
        [DefaultValue(1)]
        [Category("Misc")]
        public int DivisionsPerRow
        {
            get 
            {
                if (divisionsPerHour / divisionsPerRow * divisionsPerRow != divisionsPerHour)
                {
                    return 1;
                }
                
                return divisionsPerRow; 
            }

            set
            {
                if (value != divisionsPerRow)
                {
                    divisionsPerRow = value;

                    if (DivisionsPerRowChanged != null)
                    {
                        DivisionsPerRowChanged(this, EventArgs.Empty);
                    }
                }
            }
        }
       
        /// <summary>
        /// A notification event that is raised after DivisionsPerRow is modified. 
        /// </summary>
        public event EventHandler DivisionsPerRowChanged;

        #endregion

        #region DivisionsPerHour
        private int divisionsPerHour = 2;
        
        /// <summary>
        /// A property that gets or sets the number of time divisions per hour that appear in a Day, Custom or WorkWeek view.
        /// </summary>
        /// <remarks>
        /// The number of divisions determines the minimum time that can be used for an appointment. The default value
        /// is 2 indicating you can have two 30-minute appointments per hour. If you want to allow three 20-minutes, 
        /// or four 15-minutes appointments per hour, then set this value to 3 or 4. 
        /// This value must be 1, 2, 3, 4, 6, 10, 12, 15, 20 or 30.
        /// </remarks>
        [Browsable(true)]
        [Description("The number of time divisions.")]
        [DefaultValue(2)]
        [Category("Misc")]
        public int DivisionsPerHour
        {
            get
            {
                return divisionsPerHour;
            }

            set
            {
                if (value != divisionsPerHour)
                {
                    if (value < 1 || value > 30 || (60 % divisionsPerHour) != 0)
                    {
                        throw new ArgumentException("DivisionsPerHour must be a factor of 60 between 1 and 30");
                    }

                    divisionsPerHour = value;

                    if (DivisionsPerHourChanged != null)
                    {
                        DivisionsPerHourChanged(this, EventArgs.Empty);
                    }
                }
            }
        }
        
        /// <summary>
        /// A notification event that is raised after DivisionsPerHour is modified. 
        /// </summary>
        public event EventHandler DivisionsPerHourChanged;

        #endregion

		#region ShowTime

		private bool showTime = true;
		
        /// <summary>
		/// A property that gets or sets whether the Time Column should appear in the ScheduleControl.
		/// </summary>
		[Browsable(true)]
		[Description("Indicates whether the Time Column should appear in the ScheduleControl.")]
		[DefaultValue(true)]
		[Category("Time Column")]
		public bool ShowTime 
		{
            get
            {
                return showTime;
            }

			set
			{
				if (value != showTime)
				{
					showTime = value;
                    if (ShowTimeChanged != null)
                    {
                        ShowTimeChanged(this, EventArgs.Empty);
                    }
				}
			}
		}
		
        /// <summary>
		/// A notification event that is raised after ShowTime is modified. 
		/// </summary>
		public event EventHandler ShowTimeChanged;
		#endregion

        #region VisualStyle

        private GridVisualStyles visualStyle = GridVisualStyles.SystemTheme;
        
        /// <summary>
        /// A property that gets or sets the VisualStyle for this ScheduleControl. The setting only applies
        /// if <see cref="ThemesEnabled"/> is true.
        /// </summary>
        [Browsable(true)]
        [Description("Specifies the VisualStyle for this ScheduleControl.")]
        [DefaultValue(GridVisualStyles.SystemTheme)]
        [Category("VisualStyles")]
        public GridVisualStyles VisualStyle
        {
            get
            {
                return visualStyle;
            }

            set
            {
                if (value != visualStyle)
                {
                    visualStyle = value;
                    if (VisualStyleChanged != null)
                    {
                        VisualStyleChanged(this, EventArgs.Empty);
                    }

                    if (schedule != null && this.schedule.GetScheduleHost() != null)
                    {
                        this.schedule.GetScheduleHost().ApplyVisualStyle();
                    }
                }
            }
        }
        
        /// <summary>
        /// A notification event that is raised after VisualStyle is modified. 
        /// </summary>
        public event EventHandler VisualStyleChanged;
        #endregion

		#endregion

        bool enableAdvancedToolTip = false;
        /// <summary>
        /// Gets or Sets the AdvancedToolTip display while hovering over the schedule Appointments.
        /// </summary>
        [Browsable(true)]
        [Description("Gets or Sets the AdvancedToolTip display while hovering over the schedule Appointments.")]
        public bool EnableAdvancedToolTip
        {
            get
            {
                return enableAdvancedToolTip;
            }
            set
            {
                enableAdvancedToolTip = value;
            }
        }

        bool workWeekResize = false;

        /// <summary>
        /// Gets or Sets the propotional resizability option for the workweek scheduletype.
        /// </summary>
        public bool AllowProportionalColumnSizing
        {
            get
            {
                return workWeekResize;
            }
            set
            {
                workWeekResize = value;
            }
        }

        private bool transparentSpan = false;

        /// <summary>
        /// Gets or Sets the transparent Span appearance to schedule grid. 
        /// </summary>
        public bool EnableTransparentSpan
        {
            get
            {
                return transparentSpan;
            }
            set
            {
                transparentSpan = value;
            }
        }

        int alphaLevel = 40;

        /// <summary>
        /// Gets or sets the alpha level for transparent span.
        /// </summary>
        public int SpanTransparencyLevel
        {
            get
            {
                return alphaLevel;
            }
            set
            {
                alphaLevel = value;
            }
        }

		#endregion

        /// <summary>A method that returns a string holding the ScheduleAppearance object.</summary>
        /// <returns>String representaion of the object.</returns>
		/// <override />
		public override string ToString()
		{
			return string.Empty;
		}
	}
}
